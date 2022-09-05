/**
// <summary>
// Plugin para las validaciones de los contratos. 
// </summary>
 */
namespace ValidacionContrato
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Base class for all Plugins.
    /// </summary>    
    public class ValidacionContrato : IPlugin
    {
        
        private ITracingService tracingService;
        private IPluginExecutionContext PluginExecutionContext; ///< Contexto de ejecución del plugin
        private IOrganizationServiceFactory factory;
        private IOrganizationService service;
        
        private bool _log = false; ///< Indica si se activa o no el log. Esta variable debe inicializarse según el parámetro recibido en el constructor.
        private String ficherolog = "C:\\Users\\log_ActualizarEnCascada.txt";  ///< Fichero de log. Esta variable debe inicializarse según el parámetro recibido en el constructor.
        private const Char SEPARADOR = '#'; ///< Constante para el separador a usar en el parámetro que recibe el constructor
        private const String SALTO = "<br/>"; // + Environment.NewLine;

        private OptionSetValue ContratoFormalizado = new OptionSetValue(300000005);
        private OptionSetValue ContratoFinalizado = new OptionSetValue(300000011);
        private OptionSetValue ContratoRechazado = new OptionSetValue(300000004); // Estado Rechazado. No se tienen en cuenta los contratos en este estado para la validación de cups.
        
        /**
		// <summary>
		// Constructor de la clase
		// </summary>
        // <param name="parametros">Cadena en la que se indica si se escribe log y donde: LOG#C:\\RutaDelLog\\Fichero.log</param>
		// <remarks>
		// Recibe una cadena de texto incluyendo los parámetros (separados por el carácter #)
		// - El primer parámetro activa/desactiva la escritura del fichero log (LOG activa)
		// - El segundo parámetro es el nombre (incluyendo ruta) del fichero de log.
		// </remarks>
         */
        public ValidacionContrato(String parametros)
        {
            if (String.IsNullOrEmpty(parametros) == false)
            {
                String[] arrayPar = parametros.Split(SEPARADOR);
                if (arrayPar.Length > 0)
                {

                    if (arrayPar[0] == "LOG")
                        _log = true;
                    if (arrayPar.Length > 1)
                        ficherolog = arrayPar[1];
                }
            }
        }


        /**
        // <summary>
        // Función privada para escribir una traza
        // </summary>
        // <param name="texto">Texto a escribir en el fichero de log</param>
        // <remarks>
        // Si el log está activado escribe el mensaje en el fichero de log.
        // </remarks>
         */
        private void writelog(String texto)
        {
            tracingService.Trace(texto);
            if (_log == true)
                System.IO.File.AppendAllText(ficherolog, texto + "\r\n");
        }


        /**
        // <summary>
        // Función privada que comprueba si para las fechas indicadas existen otros contratos con el mismo cups
        // </summary>
        // <param name="cups">Cups</param>
        // <param name="fIni">Fecha de inicio del contrato</param>
        // <param name="fFin">Fecha de fin del contrato</param>
        // <param name="id">Guid del contrato que se está validando</param>
         */
        private bool hayContratosConCupsFechas(String cups, DateTime fIni, DateTime fFin, Guid id)
        {
            QueryExpression _consulta = new QueryExpression("atos_contrato");
            _consulta.ColumnSet = new ColumnSet(new String[] { "atos_name", "atos_fechainicioefectiva", "atos_fechafindefinitiva" });

            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_cups";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(cups);
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_fechainicioefectiva";
            _condicion.Operator = ConditionOperator.NotNull;
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_fechainicioefectiva";
            _condicion.Operator = ConditionOperator.LessEqual;
            _condicion.Values.Add(fFin);
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_fechafindefinitiva";
            _condicion.Operator = ConditionOperator.NotNull;
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_fechafindefinitiva";
            _condicion.Operator = ConditionOperator.GreaterEqual;
            _condicion.Values.Add(fIni);
            _filtro.Conditions.Add(_condicion);

            // Quitamos los contratos en estado rechazado.
            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_estadocontrato";
            _condicion.Operator = ConditionOperator.NotEqual;
            _condicion.Values.Add(ContratoRechazado.Value); 
            _filtro.Conditions.Add(_condicion);

            if (id != Guid.Empty)
            {
                _condicion = new ConditionExpression();
                _condicion.AttributeName = "atos_contratoid";
                _condicion.Operator = ConditionOperator.NotEqual;
                _condicion.Values.Add(id.ToString());
                _filtro.Conditions.Add(_condicion);
            }

            //writelog("Antes de Criteria.AddFilter");
            _consulta.Criteria.AddFilter(_filtro);
            //writelog("Despues de Criteria.AddFilter");

            EntityCollection _resConsulta = service.RetrieveMultiple(_consulta);
            if (_resConsulta.Entities.Count > 0)
            {
                writelog("Las fechas " + fIni.ToShortDateString() + " - " +
                          fFin.ToShortDateString() + " se solapan con los contratos:");
                for (int i = 0; i < _resConsulta.Entities.Count; i++)
                {
                    writelog("Iteracion " + i.ToString() + " - contratos solapados " + _resConsulta.Entities.Count);
                    String texto = " Contrato: ";
                    if (_resConsulta.Entities[i].Attributes.Contains("atos_name"))
                        texto += _resConsulta.Entities[i].Attributes["atos_name"].ToString() + " ";
                    else
                        texto += "Sin nombre ";
                    
                    if (_resConsulta.Entities[i].Attributes.Contains("atos_fechainicioefectiva"))
                        texto += "(" + ((DateTime)_resConsulta.Entities[i].Attributes["atos_fechainicioefectiva"]).ToShortDateString() + ",";
                    else
                        texto += "(sin fecha inicio,";
                    if (_resConsulta.Entities[i].Attributes.Contains("atos_fechafindefinitiva"))
                        texto += ((DateTime)_resConsulta.Entities[i].Attributes["atos_fechafindefinitiva"]).ToShortDateString() + ")";
                    else
                        texto += "sin fecha fin)";
                    writelog(texto);
                    /*writelog(" Contrato: " +
                          _resConsulta.Entities[i].Attributes["atos_name"].ToString() +
                          " (" + ((DateTime)_resConsulta.Entities[i].Attributes["atos_fechainicioefectiva"]).ToShortDateString() +
                          "," + ((DateTime)_resConsulta.Entities[i].Attributes["atos_fechafindefinitiva"]).ToShortDateString() + ")");*/
                }
                return true;
            }
            return false;
        }

        /**
        // <summary>
        // Función privada que comprueba la instalación del contrato
        // </summary>
        // <param name="instalacion">Objeto de tipo Entity con los campos a validar de la instalación</param>
        // <param name="_error">String para devolver concatenado los posibles errores</param>
        // <param name="_salto">String que se inicializa con el carácter salto de línea cada vez que se incluye un error</param>
        // <remarks>
        // Realiza las siguiente comprobaciones
        // - La instalación debe estar activa
        // - La instalación debe tener unidad de programación
        // - La instalación debe tener fecha de inicio de envío de previsiones
        // - La instalación debe tener fecha de fin de envío de previsiones
        // - La instalación debe tener contacto de instalación
        // - La instalación debe tener dirección de instalación
        // - La instalación debe tener tarifa
        // - La instalación debe tener el mismo número de campos "potencia contratada" informados que el número de periodos de la tarifa
        // </remarks>
         */
        private void validaInstalacion(Entity instalacion, ref String _error, ref String _salto)
        {
            String _nombreIN = " ";
            if (instalacion.Attributes.Contains("atos_name"))
                _nombreIN += instalacion.Attributes["atos_name"].ToString() + " ";



            if (!instalacion.Attributes.Contains("statecode"))
            {
                _error += string.Format("{0}La instalación{1}no tiene estado.", _salto, _nombreIN);
                _salto = SALTO;
            }
            else if (((OptionSetValue)instalacion.Attributes["statecode"]).Value != 0)
            {
                _error += string.Format("{0}La instalación{1}no está activa.", _salto, _nombreIN);
                _salto = SALTO;
            }

            // Se quita la obligatoriedad de las fechas de previsiones
            /*if (!instalacion.Attributes.Contains("atos_fechainicioenvioprevisiones"))
            {
                _error += string.Format("{0}La instalación{1}no tiene fecha inicio envío previsiones.", _salto, _nombreIN);
                _salto = SALTO;
            }

            if (!instalacion.Attributes.Contains("atos_fechafinenvioprevisiones"))
            {
                _error += string.Format("{0}La instalación{1}no tiene fecha fin envío previsiones.", _salto, _nombreIN);
                _salto = SALTO;
            }*/

            // Facturación electrónica (se quita la obligatoriedad de tener contacto de facturación). 
            /*if (!instalacion.Attributes.Contains("atos_contactofacturacionid"))
            {
                _error += string.Format("{0}La instalación{1}no tiene contacto de facturación.", _salto, _nombreIN);
                _salto = SALTO;
            }
            */

            // Facturación electrónica (se quita la obligatoriedad de tener dirección de facturación). 
            /*if (!instalacion.Attributes.Contains("atos_facturaciondireccionconcatenada") || !instalacion.Attributes.Contains("atos_facturacioncodigopostalid"))
            {
                _error += string.Format("{0}La instalación{1}no tiene dirección de facturación.", _salto, _nombreIN);
                _salto = SALTO;
            }*/

            if (!instalacion.Attributes.Contains("atos_tarifaid"))
            {
                _error += string.Format("{0}La instalación{1}no tiene tarifa.", _salto, _nombreIN);
                _salto = SALTO;
            }
        }

        /**
        // <summary>
        // Función privada que comprueba la instalación del contrato
        // </summary>
        // <param name="instalacion">Objeto de tipo Entity con los campos a validar de la instalación</param>
        // <param name="_error">String para devolver concatenado los posibles errores</param>
        // <param name="_salto">String que se inicializa con el carácter salto de línea cada vez que se incluye un error</param>
        // <remarks>
        // Realiza las siguiente comprobaciones
        // - La instalación debe estar activa
        // - La instalación debe tener unidad de programación
        // - La instalación debe tener fecha de inicio de envío de previsiones
        // - La instalación debe tener fecha de fin de envío de previsiones
        // - La instalación debe tener contacto de instalación
        // - La instalación debe tener dirección de instalación
        // - La instalación debe tener tarifa
        // - La instalación debe tener el mismo número de campos "potencia contratada" informados que el número de periodos de la tarifa
        // </remarks>
         */
        private void validaInstalacionGas(Entity instalaciongas, ref String _error, ref String _salto)
        {
            String _nombreIN = " ";
            if (instalaciongas.Attributes.Contains("atos_name"))
                _nombreIN += instalaciongas.Attributes["atos_name"].ToString() + " ";

            if (!instalaciongas.Attributes.Contains("statecode"))
            {
                _error += string.Format("{0}La instalación gas {1}no tiene estado.", _salto, _nombreIN);
                _salto = SALTO;
            }
            else if (((OptionSetValue)instalaciongas.Attributes["statecode"]).Value != 0)
            {
                _error += string.Format("{0}La instalación gas{1}no está activa.", _salto, _nombreIN);
                _salto = SALTO;
            }

            if (!instalaciongas.Attributes.Contains("atos_peajeid"))
            {
                _error += string.Format("{0}La instalación gas{1}no tiene peaje.", _salto, _nombreIN);
                _salto = SALTO;
            }
        }

        /**
        // <summary>
        // Función privada que recupera la información de la instalación del contrato
        // </summary>
        // <param name="instalacionid">Guid de la instalación del contrato</param>
        // <remarks>
        // Devuelve un objeto de tipo Entity con los campos a validar de la instalación
        // </remarks>
         */
        private Entity Instalacion(Guid instalacionid)
        {
            Entity _instalacion = service.Retrieve("atos_instalacion", instalacionid,
                    new ColumnSet(new String[] { "atos_name", "atos_unidadprogramacionid", 
                                    "atos_fechainicioenvioprevisiones", "atos_fechafinenvioprevisiones", "atos_tarifaid",
                                    "atos_potenciacontratada1", "atos_potenciacontratada2", "atos_potenciacontratada3",
                                    "atos_potenciacontratada4", "atos_potenciacontratada5", "atos_potenciacontratada6",
                                    "atos_contactoinstalacionid", "atos_instalaciondireccionconcatenada", 
                                    "atos_instalacioncodigopostalid", "atos_contactofacturacionid", 
                                    "atos_facturaciondireccionconcatenada", "atos_facturacioncodigopostalid", "statecode"}));
            return _instalacion;
        }

        /**
        // <summary>
        // Función privada que recupera la información de la instalación gas del contrato
        // </summary>
        // <param name="instalacionid">Guid de la instalación gas del contrato</param>
        // <remarks>
        // Devuelve un objeto de tipo Entity con los campos a validar de la instalación
        // </remarks>
         */
        private Entity InstalacionGas(Guid instalaciongasid)
        {
            Entity _instalacion = service.Retrieve("atos_instalaciongas", instalaciongasid,
                    new ColumnSet(new String[] { "atos_name", "atos_peajeid", "atos_usodelgasid",
                                    "atos_fechainiciovigenciapeaje", "atos_contactoinstalacionid",
                                    "atos_instalaciondireccionconcatenada", "atos_instalacioncodigopostalid",
                                    "atos_contactofacturacionid", "atos_facturaciondireccionconcatenada",
                                    "atos_facturacioncodigopostalid", "statecode"}));
            return _instalacion;
        }

        /**
        // <summary>
        // Función privada que llama a la validación de la instalación después de recuperar la información de la instalación del contrato
        // </summary>
        // <param name="contrato">Objeto de tipo Entity con la información del contrato</param>
        // <param name="_error">String para devolver concatenado los posibles errores</param>
        // <param name="_salto">String que se inicializa con el carácter salto de línea cada vez que se incluye un error</param>
        // <remarks>
        // Utiliza las funciones Instalacion y validaInstalacion
        // </remarks>
         */
        private void validaInstalacionContrato(Entity contrato, ref String _error, ref String _salto)
        {
            if (!contrato.Attributes.Contains("atos_instalacionid"))
            {
                _error += string.Format("{0}El contrato debe estar asociado a una instalación.", _salto);
                _salto = SALTO;
            }
            else
            {
                writelog("validaInstalacionContrato 1");
                Entity _instalacion = Instalacion(((EntityReference)contrato.Attributes["atos_instalacionid"]).Id);
                writelog("validaInstalacionContrato 2");
                validaInstalacion(_instalacion, ref _error, ref _salto);
                writelog("validaInstalacionContrato 2");
            }
        }

        /**
        // <summary>
        // Función privada que llama a la validación de la instalación gas después de recuperar la información de la instalación gas del contrato
        // </summary>
        // <param name="contrato">Objeto de tipo Entity con la información del contrato</param>
        // <param name="_error">String para devolver concatenado los posibles errores</param>
        // <param name="_salto">String que se inicializa con el carácter salto de línea cada vez que se incluye un error</param>
        // <remarks>
        // Utiliza las funciones InstalacionGas y validaInstalacionGas
        // </remarks>
         */
        private void validaInstalacionContratoGas(Entity contrato, ref String _error, ref String _salto)
        {
            if (!contrato.Attributes.Contains("atos_instalaciongasid"))
            {
                _error += string.Format("{0}El contrato debe estar asociado a una instalación gas.", _salto);
                _salto = SALTO;
            }
            else
            {
                writelog("validaInstalacionContrato 1");
                Entity _instalaciongas = InstalacionGas(((EntityReference)contrato.Attributes["atos_instalaciongasid"]).Id);
                writelog("validaInstalacionContrato 2");
                validaInstalacionGas(_instalaciongas, ref _error, ref _salto);
                writelog("validaInstalacionContrato 2");
            }
        }


        /**
        // <summary>
        // Función privada que comprueba la razón social del contrato
        // </summary>
        // <param name="razonSocial">Objeto de tipo Entity con los campos a validar de la razón social</param>
        // <param name="_error">String para devolver concatenado los posibles errores</param>
        // <param name="_salto">String que se inicializa con el carácter salto de línea cada vez que se incluye un error</param>
        // <remarks>
        // Realiza las siguiente comprobaciones
        // - La razón social debe tener contacto
        // - La razón social debe tener dirección y código postal
        // </remarks>
         */
        private void validaRazonSocial(Entity razonSocial, ref String _error, ref String _salto)
        {
            String _nombreRS = " ";
            if (razonSocial.Attributes.Contains("name"))
                _nombreRS += razonSocial.Attributes["name"].ToString() + " ";
            if (!razonSocial.Attributes.Contains("primarycontactid"))
            {
                _error += string.Format("{0}La razón social{1}no tiene contacto.", _salto, _nombreRS);
                _salto = SALTO;
            }

            if (!razonSocial.Attributes.Contains("atos_rsdireccionconcatenada") || !razonSocial.Attributes.Contains("atos_rscodigopostalid"))
            {
                _error += string.Format("{0}La razón social{1}no tiene dirección.", _salto, _nombreRS);
                _salto = SALTO;
            }
        }

        /**
        // <summary>
        // Función privada que recupera la información de la razón social del contrato
        // </summary>
        // <param name="razonsocialid">Guid de la razón social del contrato</param>
        // <remarks>
        // Devuelve un objeto de tipo Entity con los campos a validar de la razón social
        // </remarks>
         */
        private Entity razonSocial(Guid razonsocialid)
        {
            Entity _razonSocial = service.Retrieve("account", razonsocialid,
                    new ColumnSet(new String[] { "name", "primarycontactid", "atos_rsdireccionconcatenada", "atos_rscodigopostalid" }));
            return _razonSocial;
        }

        /**
        // <summary>
        // Función privada que llama a la validación de la razón social después de recuperar la información de la razón social del contrato
        // </summary>
        // <param name="contrato">Objeto de tipo Entity con la información del contrato</param>
        // <param name="_error">String para devolver concatenado los posibles errores</param>
        // <param name="_salto">String que se inicializa con el carácter salto de línea cada vez que se incluye un error</param>
        // <remarks>
        // Utiliza las funciones razonSocial y validaRazonSocial
        // </remarks>
         */
        private void validaRazonSocialContrato(Entity contrato, ref String _error, ref String _salto)
        {
            if (!contrato.Attributes.Contains("atos_razonsocialid"))
            {
                _error = "El contrato debe estar asociado a una razón social";
                _salto = SALTO;
            }
        }



        /**
        // <summary>
        // Función privada que recupera las ofertas hijas de una oferta padre
        // </summary>
        // <param name="_ofertaPadre">Guid de la oferta padre</param>
        // <remarks>
        // Devuelve una colección de entidades de tipo oferta
        // </remarks>
         */
        private EntityCollection ofertasHijas(Guid _ofertaPadre)
        {
            QueryExpression _consulta = new QueryExpression("atos_oferta");
            writelog("ofertasHijas 1");
            _consulta.ColumnSet = new ColumnSet(new String[] { "atos_ofertaid", "atos_name", "atos_commodity", "atos_tipooferta", "atos_razonsocialid", 
                                                               "atos_cuentanegociadoraid", "atos_instalacionid", "atos_instalaciongasid" });

            writelog("ofertasHijas 2");
            LinkEntity _link = new LinkEntity("atos_oferta", "atos_oferta", "atos_ofertapadreid", "atos_ofertaid", JoinOperator.Inner);
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_ofertapadreid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(_ofertaPadre.ToString());

            writelog("ofertasHijas 3");
            _link.LinkCriteria.Conditions.Add(_condicion);

            _consulta.LinkEntities.Add(_link);

            writelog("ofertasHijas 4");
            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;

            writelog("ofertasHijas 5");
            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_tipooferta";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(new OptionSetValue(300000002));

            writelog("ofertasHijas 6");
            OrderExpression _orden = new OrderExpression();
            _orden.AttributeName = "atos_razonsocialid";
            _orden.OrderType = OrderType.Ascending;
            _consulta.Orders.Add(_orden);

            writelog("ofertasHijas 7");
            _orden = new OrderExpression();
            _orden.AttributeName = "atos_instalacionid";
            _orden.OrderType = OrderType.Ascending;
            _consulta.Orders.Add(_orden);

            writelog("ofertasHijas 7");
            _orden = new OrderExpression();
            _orden.AttributeName = "atos_instalaciongasid";
            _orden.OrderType = OrderType.Ascending;
            _consulta.Orders.Add(_orden);

            writelog("ofertasHijas 8");
            EntityCollection _resConsulta = service.RetrieveMultiple(_consulta);
            writelog("ofertasHijas 9");
            return _resConsulta;
        }

        private Boolean tieneCNAERazonSocial(Guid razonSocialId)
        {
            Entity _razonSocial = service.Retrieve("account", razonSocialId,
                           new ColumnSet(new String[] { "name", "atos_cnaeid" }));

            if (!_razonSocial.Attributes.Contains("atos_cnaeid"))
            {
                return false;
            }
            return true;
        }

        private void verSubcontratosTienenCNAERazonSocialSinInformar(Guid contratoPadreId, ref String _error, ref String _salto)
        {
            QueryExpression _consulta = new QueryExpression("atos_contrato");
            _consulta.ColumnSet = new ColumnSet(new String[] { "atos_name", "atos_contratoid", "atos_razonsocialid" });

            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;

            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_contratomultipuntoid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(contratoPadreId);
            _filtro.Conditions.Add(_condicion);

            _consulta.Criteria.AddFilter(_filtro);

            EntityCollection _resConsulta = service.RetrieveMultiple(_consulta);

            foreach (Entity fcontrato in _resConsulta.Entities)
            {
                EntityReference erRazonSocial = (EntityReference)fcontrato.Attributes["atos_razonsocialid"];
                if (!tieneCNAERazonSocial(erRazonSocial.Id))
                {
                    //throw new Exception("Tiene que informar el CNAE para la razon social: " + erRazonSocial.Name);
                    _error += "Tiene que informar el CNAE para la razon social: " + erRazonSocial.Name + _salto;
                }
            }
        }

        /**
        // <summary>
        // Punto de entrada del plugin.
        // </summary>
        // <param name="serviceProvider">The service provider.</param>
        // <remarks>
        // Se ejecuta en la creación y modificación de contratos.
        // </remarks>
         */
        public void Execute(IServiceProvider serviceProvider)
        {
            
            tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            PluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the Organization Service factory service from the service provider
            factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            // Use the factory to generate the Organization Service.
            service = factory.CreateOrganizationService(PluginExecutionContext.UserId);
            
            writelog("-----------------------------------------");
            writelog(DateTime.Now.ToLocalTime().ToString());
            writelog("Plugin Validaciones de contrato");
            writelog("Mensaje: " + PluginExecutionContext.MessageName);
            if (PluginExecutionContext.MessageName == "Update")
            {
                Entity _preImage = PluginExecutionContext.PreEntityImages["PreEntityImage"];
                Entity ef = (Entity)PluginExecutionContext.InputParameters["Target"];

                if (_preImage.Attributes.Contains("atos_estadocontrato") && 
                    ef.Attributes.Contains("atos_estadocontrato") &&
                    !ef.Attributes.Contains("atos_cups") &&
                    !ef.Attributes.Contains("atos_fechainicioefectiva") &&
                    !ef.Attributes.Contains("atos_fechafindefinitiva"))
                {
                    writelog("Solo ha cambiado el estado contrato de " + ((OptionSetValue)_preImage.Attributes["atos_estadocontrato"]).Value + " a " + ((OptionSetValue)ef.Attributes["atos_estadocontrato"]).Value);
                    if (((OptionSetValue)ef.Attributes["atos_estadocontrato"]).Value == ((OptionSetValue)_preImage.Attributes["atos_estadocontrato"]).Value ||
                         ((OptionSetValue)ef.Attributes["atos_estadocontrato"]).Value == ContratoRechazado.Value)
                        return;
                }

                String _error = "";
                String _salto = SALTO;

                //Si actualizamos un contrato a formalizado o finalizado este tiene que tener todas las RS con el CNAE informado
                if (((OptionSetValue)ef.Attributes["atos_estadocontrato"]).Value == ContratoFormalizado.Value ||
                    ((OptionSetValue)ef.Attributes["atos_estadocontrato"]).Value == ContratoFinalizado.Value)
                {
                    if (_preImage.Attributes.Contains("atos_instalacionid") || _preImage.Attributes.Contains("atos_instalaciongasid"))
                    {
                        EntityReference erRazonSocial = (EntityReference)_preImage.Attributes["atos_razonsocialid"];
                        if (!tieneCNAERazonSocial(erRazonSocial.Id))
                        {
                            _error = "Tiene que informar el CNAE para la razon social: " + erRazonSocial.Name + _salto;
                        }
                    }
                    else
                    {
                        
                        verSubcontratosTienenCNAERazonSocialSinInformar(_preImage.Id, ref _error, ref _salto);
                    }
                }

                //Comprobamos que los cups que pertenecen a las instalaciones no esten involucrados en otros contratos
                //que esten vigentes
                if (_preImage.Attributes.Contains("atos_estadocontrato") && 
                    _preImage.Attributes.Contains("atos_cups") &&
                    _preImage.Attributes.Contains("atos_fechainicioefectiva") &&
                    _preImage.Attributes.Contains("atos_fechafindefinitiva") &&
                    (ef.Attributes.Contains("atos_estadocontrato") || ef.Attributes.Contains("atos_cups") || ef.Attributes.Contains("atos_fechainicioefectiva") || ef.Attributes.Contains("atos_fechafindefinitiva")))
                {
                    String _cups;
                    DateTime _fInicio;
                    DateTime _fFin;

                    writelog("Ha modificado estado, cups o fechas");

                    if (ef.Attributes.Contains("atos_cups"))
                        _cups = ef.Attributes["atos_cups"].ToString();
                    else
                        _cups = _preImage.Attributes["atos_cups"].ToString();

                    if (ef.Attributes.Contains("atos_fechainicioefectiva"))
                        _fInicio = (DateTime)ef.Attributes["atos_fechainicioefectiva"];
                    else
                        _fInicio = (DateTime)_preImage.Attributes["atos_fechainicioefectiva"];

                    if (ef.Attributes.Contains("atos_fechafindefinitiva"))
                        _fFin = (DateTime)ef.Attributes["atos_fechafindefinitiva"];
                    else
                        _fFin = (DateTime)_preImage.Attributes["atos_fechafindefinitiva"];

                    writelog("cups: " + _cups + " fecha inicio: " + _fInicio.ToLongDateString() + " fecha fin: " + _fFin.ToLongDateString());
                    //writelog("Hay datos de cups y fechas para pre y post");
                    if ( ef.Attributes.Contains("atos_estadocontrato") ||
                        _preImage.Attributes["atos_cups"].ToString() != _cups ||
                        (DateTime)_preImage.Attributes["atos_fechainicioefectiva"] != _fInicio ||
                        (DateTime)_preImage.Attributes["atos_fechafindefinitiva"] != _fFin)
                    {
                        //writelog("Ha cambiado el cups o las fechas");

                        if (hayContratosConCupsFechas(_cups, _fInicio, _fFin, ef.Id))
                            _error = "Existen contratos con el mismo CUPS cuyas fechas de inicio y fin se solapan con las de este contrato";
                    }
                }

                if (_error != "")
                    throw new Exception(_error);
            }
            else if (PluginExecutionContext.MessageName == "Create")
            {
                Entity ef = (Entity)PluginExecutionContext.InputParameters["Target"];
                
                if (!ef.Attributes.Contains("atos_ofertaid")) // Debe colgar de una oferta
                    throw new Exception("El contrato debe crearse a partir de una oferta");
                if (!ef.Attributes.Contains("atos_commodity"))//Debe tener commodity informado
                    throw new Exception("El contrato no tiene commodity informado");
                writelog("Busca atos_name y atos_tipooferta de la oferta padre");
                Entity ofertaPadre = service.Retrieve("atos_oferta", ((EntityReference)ef.Attributes["atos_ofertaid"]).Id, 
                    new ColumnSet("atos_name", "atos_tipooferta", "atos_commodity"));

                if (!ofertaPadre.Attributes.Contains("atos_tipooferta"))
                {
                    String mensaje = "La oferta ";
                    if (ofertaPadre.Attributes.Contains("atos_name"))
                        mensaje += ofertaPadre.Attributes["atos_name"].ToString() + " ";
                    mensaje += "no está correctamente definida. No tiene tipo de oferta";
                    throw new Exception(mensaje);
                }
                
                if (!ofertaPadre.Attributes.Contains("atos_commodity"))
                {
                    String mensaje = "La oferta ";
                    if (ofertaPadre.Attributes.Contains("atos_name"))
                        mensaje += ofertaPadre.Attributes["atos_name"].ToString() + " ";
                    mensaje += "no está correctamente definida. No tiene commodity";
                    throw new Exception(mensaje);
                }

                int commodityContrato = ((OptionSetValue)ef.Attributes["atos_commodity"]).Value;

                if (commodityContrato != ((OptionSetValue)ofertaPadre.Attributes["atos_commodity"]).Value)
                {
                    String mensaje = "La oferta ";
                    if (ofertaPadre.Attributes.Contains("atos_name"))
                        mensaje += ofertaPadre.Attributes["atos_name"].ToString() + " ";
                    mensaje += "no está correctamente definida. La commodity oferta no coincide con la del contrato.";
                    throw new Exception(mensaje);
                }

                if (((OptionSetValue)ofertaPadre.Attributes["atos_tipooferta"]).Value == 300000000)
                {
                    writelog("buscando ofertas hijas");
                    EntityCollection _ofertas = ofertasHijas(ofertaPadre.Id);
                    Guid _razonsocialid = Guid.Empty;
                    Guid _instalacionid = Guid.Empty;
                    String _error = "";
                    String _salto = "";
                    writelog("Hay " + _ofertas.Entities.Count.ToString() + " ofertas hijas");
                    for (int i = 0; i < _ofertas.Entities.Count; i++)
                    {
                        writelog("validando oferta " + i.ToString());
                        if (!_ofertas.Entities[i].Attributes.Contains("atos_razonsocialid"))
                        {
                            _error += string.Format("{0}La oferta {1} no tiene razón social.", _salto, _ofertas.Entities[i].Attributes["atos_name"].ToString());
                            _salto = SALTO;
                        }

                        if (!_ofertas.Entities[i].Attributes.Contains("atos_commodity"))
                        {
                            _error += string.Format("{0}La oferta {1} no tiene commodity.", _salto, _ofertas.Entities[i].Attributes["atos_name"].ToString());
                            _salto = SALTO;
                        }

                        if (commodityContrato != ((OptionSetValue)_ofertas.Entities[i].Attributes["atos_commodity"]).Value)
                        {
                            _error += string.Format("{0}La oferta {1} tiene un commodity distinto a la de contrato.", _salto, _ofertas.Entities[i].Attributes["atos_name"].ToString());
                            _salto = SALTO;
                        }
                        else
                        {
                            if (((OptionSetValue)_ofertas.Entities[i].Attributes["atos_commodity"]).Value == 300000000)
                            {
                                if (_ofertas.Entities[i].Attributes.Contains("atos_instalacionid"))
                                {
                                    if (_instalacionid == Guid.Empty ||
                                            _instalacionid != ((EntityReference)_ofertas.Entities[i].Attributes["atos_instalacionid"]).Id)
                                    {
                                        _instalacionid = ((EntityReference)_ofertas.Entities[i].Attributes["atos_instalacionid"]).Id;
                                        Entity _instalacion = Instalacion(_instalacionid);
                                        writelog("validaInstalación");
                                        validaInstalacion(_instalacion, ref _error, ref _salto);
                                    }
                                }
                                else
                                {
                                    _error += string.Format("{0}La oferta {1} no tiene instalación.", _salto, _ofertas.Entities[i].Attributes["atos_name"].ToString());
                                    _salto = SALTO;
                                }
                            } else if (((OptionSetValue)_ofertas.Entities[i].Attributes["atos_commodity"]).Value == 300000001)
                            {
                                if (_ofertas.Entities[i].Attributes.Contains("atos_instalaciongasid"))
                                {
                                    if (_instalacionid == Guid.Empty ||
                                            _instalacionid != ((EntityReference)_ofertas.Entities[i].Attributes["atos_instalaciongasid"]).Id)
                                    {
                                        _instalacionid = ((EntityReference)_ofertas.Entities[i].Attributes["atos_instalaciongasid"]).Id;
                                        Entity _instalacion = InstalacionGas(_instalacionid);
                                        writelog("validaInstalación");
                                        validaInstalacionGas(_instalacion, ref _error, ref _salto);
                                    }
                                }
                                else
                                {
                                    _error += string.Format("{0}La oferta {1} no tiene instalación gas.", _salto, _ofertas.Entities[i].Attributes["atos_name"].ToString());
                                    _salto = SALTO;
                                }
                            } else
                            {
                                _error += string.Format("{0}La oferta {1} no tiene commodity correcto.", _salto, _ofertas.Entities[i].Attributes["atos_name"].ToString());
                                _salto = SALTO;
                            }
                                
                        }

                        
                    }
                    if (_error != "")
                        throw new Exception(_error);
                }
                else
                {
                    String _error = "";
                    String _salto = "";
                    if (!ef.Attributes.Contains("atos_contratomultipuntoid") &&
                         ((OptionSetValue)ofertaPadre.Attributes["atos_tipooferta"]).Value == 300000002)
                    {
                        writelog("Es una oferta 'no hija'");
                        validaRazonSocialContrato(ef, ref _error, ref _salto);
                        writelog("Despues de validaRazonSocialContrato");
                        
                        if (((OptionSetValue)ofertaPadre.Attributes["atos_commodity"]).Value == 300000000)
                        {
                            validaInstalacionContrato(ef, ref _error, ref _salto);
                            writelog("Despues de validaInstalacionPowerContrato");
                        }
                        else if(((OptionSetValue)ofertaPadre.Attributes["atos_commodity"]).Value == 300000001)
                        {
                            validaInstalacionContratoGas(ef, ref _error, ref _salto);
                            writelog("Despues de validaInstalacionGasContrato");
                        }
                        else
                        {
                            _error += string.Format("{0}Commodity no esta contemplado.", _salto);
                            _salto = SALTO;
                        }
                    }

                    if (ef.Attributes.Contains("atos_cups") &&
                        ef.Attributes.Contains("atos_fechainicioefectiva") &&
                        ef.Attributes.Contains("atos_fechafindefinitiva"))
                    {
                        //writelog("Hay datos de cups y fechas para pre y post");
                        if (hayContratosConCupsFechas(ef.Attributes["atos_cups"].ToString(), (DateTime)ef.Attributes["atos_fechainicioefectiva"], (DateTime)ef.Attributes["atos_fechafindefinitiva"], Guid.Empty))
                        {
                            _error += string.Format("{0}Existen contratos con el mismo CUPS cuyas fechas de inicio y fin se solapan con las de este contrato.", _salto);
                            _salto = SALTO;
                        }
                    }
                    if (_error != "")
                        throw new Exception(_error);
                }
            }
        }
    }
}