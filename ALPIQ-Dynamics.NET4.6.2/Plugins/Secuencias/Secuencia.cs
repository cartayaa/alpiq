    /**
// <summary>
// Plugin para la gestión de secuencias. 
// </summary>
// <remarks>
// La clase principal desde la que se inician los cálculos es Secuencia (en la clase hay ejemplos de construcción de fórmulas)<br/>
// Las funciones disponibles para utilizar en las fórmulas de secuencias son:<br/>
// <ul>
//   <li>Secuencias.Funcion.ToLower  (se aplica sobre el valor de un campo)
//   <li>Secuencias.Funcion.ToUpper  (se aplica sobre el valor de un campo)
//   <li>Secuencias.Funcion.PadZero  (se aplica sobre el valor de un campo)
//   <li>Secuencias.Funcion.PadLeft  (se aplica sobre el valor de un campo)
//   <li>Secuencias.Funcion.PadRight  (se aplica sobre el valor de un campo)
//   <li>Secuencias.Funcion.First  (se aplica sobre el valor de un campo)
//   <li>Secuencias.Funcion.Last  (se aplica sobre el valor de un campo)
//   <li>Secuencias.Funcion.Substring  (se aplica sobre el valor de un campo)
//   <li>Secuencias.Funcion.Now   (Global, no aplica a campos)
//   <li>Secuencias.Funcion.FormatDate  (se aplica sobre el valor de un campo)
//   <li>Secuencias.Funcion.LlaveDerecha   (Global, no aplica a campos)
//   <li>Secuencias.Funcion.LlaveIzquierda   (Global, no aplica a campos)
//   <li>Secuencias.Funcion.Almohadilla  (Global, no aplica a campos)
//   <li>Secuencias.Funcion.Punto   (Global, no aplica a campos)
//   <li>Secuencias.Funcion.Barra   (Global, no aplica a campos)
//   <li>Secuencias.Funcion.Lookup  (se aplica sobre el valor de un campo)
//   <li>Secuencias.Funcion.Iif  (se aplica sobre el valor de un campo)
//   <li>Secuencias.Funcion.NVL  (se aplica sobre el valor de un campo)
//   <li>Secuencias.Funcion.Concatenate  (se aplica sobre el valor de un campo)
//   <li>Secuencias.Funcion.PreConcatenate   (se aplica sobre el valor de un campo)
//   <li>Secuencias.Funcion.ConcatIfNotNull  (se aplica sobre el valor de un campo)
//   <li>Secuencias.Funcion.PreConcatIfNotNull   (se aplica sobre el valor de un campo)
//   <li>Secuencias.Funcion.Literal  (Global, no aplica a campos)
// </ul>
// </remarks>
 */
namespace Secuencias
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System.Collections.Generic;
    using Microsoft.Xrm.Sdk.Metadata;
    using Microsoft.Xrm.Sdk.Messages;


    /**
    // <summary>
    // Clase para construir el valor de una secuencia a partir de una regla asociada a la entidad.
    // </summary>
	// <remarks>
	// Deriva de IPlugin.<br/>
    // La regla se compone de campos  (Secuencias.Campo) a los que se le pueden aplicar funciones (Secuencias.Funcion) y/o funciones globales (Secuencias.Globales)<br/>
    // Los campos van encerrados entre {} y las funciones que se aplican a su valor se ponen a continuación del campo unidas por un "."<br/>
    // Si el campo es un EntityReference se puede recuperar otro campo de la entidad referencia poniendo en lugar del nombre del campo lo siguiente:<br>
    //  {nombre_del_campo_id!nombre_de_la_entidad_referenciada!nombre_del_campo_de_la_entidad_referenciada}  por ejemplo con:<br/>
    //  {atos_instalacionid!atos_instalacion!atos_codigoinstalacion}  se obtiene el código de instalación de la entidad instalación (a través del lookup atos_instalacionid)<br/>
    // Las funciones globales no se pueden aplicar a campos, van fuera de las llaves y no se pueden poner dos funciones seguidas
    // Ejemplos:
    // <ul>
    //   <li>{atos_campo1.ToUpper().PadRight(4,+)}{atos_entidad1id!atos_entidad1!atos_campoentidad1}Now(){Secuencial.PadZero(6)}
    //     <ul>
    //      <li>Construye el valor de la siguiente manera:
    //        <ol>
    //          <li>Recupera el valor de atos_campo1 (por ejemplo c1), lo convierte a mayúsculas y lo normaliza a cuatro caracteres con + por la derecha -> C1++
    //          <li>Recupera el valor de atos_campoentidad1 de la entidad atos_entidad1 a la que accede con el valor de atos_entidad1id (por ejemplo Look2)
    //          <li>Calcula la fecha del día en formato yyyyMMdd (por ejemplo 20150325)
    //          <li>Calcula el siguiente número secuencial y lo normaliza a 6 caracteres con 0's por la izquierda (por ejemplo 000023)
    //          <li>El valor resultante es la unión de todo lo anterior (C1++Look220150325000023)
    //        </ol>
    //     </ul>
    //   <li>Literal(Contrato-){atos_opcion.Iif(Opcion1,O1,Opcion2,O2,Ot)}Literal(-){Secuencial.PadZero(4)}
    //     <ul>
    //      <li>Construye el valor de la siguiente manera:
    //        <ol>
    //          <li>Comienza con el literal "Contrato-"
    //          <li>Si el valor del campo atos_opcion1 es "Opcion1" toma "O1", si es "Opcion2" considera "O2" y sino toma "Ot" (por ejemplo el campo es "Opcion2" por tanto de aquí sale "O2")
    //          <li>Continúa con el literal "-"
    //          <li>Calcula el siguiente número secuencial y lo normaliza a 4 caracteres con 0's por la izquierda (por ejemplo 0152)
    //          <li>El valor resultante es la unión de todo lo anterior (Contrato-O2-0152)
    //        </ol>
    //     </ul>
    // </ul>
	// </remarks>  
     */
    public class Secuencia : IPlugin
    {
        LocalPluginContext localContext;
        protected class LocalPluginContext
        {
            internal IServiceProvider ServiceProvider
            {
                get;

                private set;
            }

            internal IOrganizationServiceFactory OrganizationServiceFactory
            {
                get;

                private set;
            }

            internal IOrganizationService OrganizationService
            {
                get;

                private set;
            }

            internal IPluginExecutionContext PluginExecutionContext
            {
                get;

                private set;
            }

            internal ITracingService TracingService
            {
                get;

                private set;
            }

            private LocalPluginContext()
            {
            }

            internal LocalPluginContext(IServiceProvider serviceProvider)
            {
                if (serviceProvider == null)
                {
                    throw new ArgumentNullException("serviceProvider");
                }

                // Obtain the execution context service from the service provider.
                this.PluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                // Obtain the tracing service from the service provider.
                this.TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                // Obtain the Organization Service factory service from the service provider
                this.OrganizationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                // Use the factory to generate the Organization Service.
                this.OrganizationService = this.OrganizationServiceFactory.CreateOrganizationService(this.PluginExecutionContext.UserId);
            }

            internal void Trace(string message)
            {
                if (string.IsNullOrWhiteSpace(message) || this.TracingService == null)
                {
                    return;
                }

                if (this.PluginExecutionContext == null)
                {
                    this.TracingService.Trace(message);
                }
                else
                {
                    //AC this.TracingService.Trace(
                    //AC     "{0}, Correlation Id: {1}, Initiating User: {2}",
                    //AC message,
                    //AC this.PluginExecutionContext.CorrelationId,
                    //AC this.PluginExecutionContext.InitiatingUserId);
                    this.TracingService.Trace("{0}", message);
                }
            }
        }



        /*
        private Entity secuencia(String _entidad)
        {
            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;

            ConditionExpression _condicion;

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_entidad";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(_entidad);
            _filtro.Conditions.Add(_condicion);

            QueryExpression _consulta = new QueryExpression("atos_secuencia");
            _consulta.ColumnSet.AddColumns("atos_secuenciaid", "atos_entidad", "atos_campo", "atos_formato", 
                                           "atos_entidadcontador", "atos_campobusqueda", "atos_camporegla", "atos_campocontador");
            _consulta.Criteria.AddFilter(_filtro);
            localContext.Trace("Antes de buscar " + _entidad + " en atos_secuencia");
            EntityCollection _resultado = localContext.OrganizationService.RetrieveMultiple(_consulta);
            if (_resultado.Entities.Count > 0)
                return _resultado.Entities[0];
            return null;
        }*/

        private EntityCollection secuencias(String _entidad)
        {
            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;

            ConditionExpression _condicion;

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_entidad";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(_entidad);
            _filtro.Conditions.Add(_condicion);

            QueryExpression _consulta = new QueryExpression("atos_secuencia");
            _consulta.ColumnSet.AddColumns("atos_secuenciaid", "atos_entidad", "atos_campo", "atos_formato",
                                           "atos_entidadcontador", "atos_campobusqueda", "atos_camporegla", "atos_campocontador",
                                           "atos_controlduplicados");
            _consulta.Criteria.AddFilter(_filtro);

            OrderExpression _orden = new OrderExpression();
            _orden.AttributeName = "atos_orden";
            _orden.OrderType = OrderType.Ascending;

            _consulta.Orders.Add(_orden);

            // AC localContext.Trace("Antes de buscar " + _entidad + " en atos_secuencia");
            EntityCollection _resultado = localContext.OrganizationService.RetrieveMultiple(_consulta);
            return _resultado;
        }


        private Boolean estaDuplicado(String _entidad, String _campo, String _valor)
        {
            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;

            ConditionExpression _condicion;

            _condicion = new ConditionExpression();
            _condicion.AttributeName = _campo;
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(_valor);
            _filtro.Conditions.Add(_condicion);

            QueryExpression _consulta = new QueryExpression(_entidad);
            _consulta.ColumnSet.AddColumns(_campo);
            _consulta.Criteria.AddFilter(_filtro);
            // AC localContext.Trace("Antes de buscar " + _valor + " en " + _entidad);
            EntityCollection _resultado = localContext.OrganizationService.RetrieveMultiple(_consulta);
            if (_resultado.Entities.Count > 0)
                return true;
            return false;
        }

        private Boolean estaDuplicado(Entity _entidad)
        {
            UpdateRequest req = new UpdateRequest();
            //CreateRequest req = new CreateRequest();
            req.Parameters.Add("SuppressDuplicateDetection", false);
            req.Target = _entidad;
            try
            {
                localContext.OrganizationService.Execute(req);
                return false;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                if (ex.Detail.ErrorCode == -2147220685)
                {
                    return true;
                }
                else if (ex.Detail.ErrorCode == -2147220969)
                {
                    return false;
                }
                else
                {
                    throw new System.Exception("Control de duplicado ErrorCode:" + ex.Detail.ErrorCode.ToString() + " Message: " + ex.Detail.Message);
                }
            }
        }

        List<Componente> reglas(String _regla)
        {
            List<Componente> _listaComponente = new List<Componente>();
            String[] _partes = _regla.Split('{');
            if (_partes.Length > 0)
            {
                // Si en la primera parte hay algo se trata siempre de funciones globales ya que si fuera un campo lo primero de la 
                // cadena formato al dividir por { la primera ocurrencia estaría vacía
                if (_partes[0].Length > 0)
                {
                    // AC localContext.Trace("_partes[0]: " + _partes[0]);

                    String[] _globalesConcatenadas = _partes[0].Split('#');
                    for (int j = 0; j < _globalesConcatenadas.Length; j++)
                        _listaComponente.Add(new Globales(_globalesConcatenadas[j], localContext.OrganizationService, localContext.TracingService));

                }
            }
            for (int i = 1; i < _partes.Length; i++)
            {
                String[] _subpartes = _partes[i].Split('}');
                if (_subpartes.Length != 2)
                    throw new System.Exception("Error procesando " + _partes[i]);
                if (_subpartes[0].Length > 0)
                {
                    // AC localContext.Trace("[" + i.ToString() + "] _subpartes[0]: " + _subpartes[0]);
                    _listaComponente.Add(new Campo(_subpartes[0], localContext.OrganizationService, localContext.TracingService));
                }
                if (_subpartes[1].Length > 0)
                {
                    // AC localContext.Trace("[" + i.ToString() + "] _subpartes[1]: " + _subpartes[1]);
                    String[] _globalesConcatenadas = _subpartes[1].Split('#');
                    for (int j = 0; j < _globalesConcatenadas.Length; j++)
                        _listaComponente.Add(new Globales(_globalesConcatenadas[j], localContext.OrganizationService, localContext.TracingService));
                }
            }
            return _listaComponente;
        }

        String calculaValorRegla(Entity _secuencia, Entity _ef, List<Componente> _listaComponente, String _valorRegla)
        {
            String _valor = "";

            _valor = "";
            //AC localContext.Trace("Calculando valor");
            for (int i = 0; i < _listaComponente.Count; i++)
            {
                //AC localContext.Trace("Calculando valor de " + _listaComponente[i].Cadena);
                if (_listaComponente[i].Tipo == 'G')
                    _valor += ((Globales)_listaComponente[i]).calculaValor(_ef);
                else if (_listaComponente[i].Tipo == 'C')
                {
                    String _campo = ((Campo)_listaComponente[i]).Nb_Campo;
                    for (int j = 0; j < ((Campo)_listaComponente[i]).Funciones.Count; j++)
                    {
                        _campo += "." + ((Campo)_listaComponente[i]).Funciones[j].Nb_Funcion + "(";
                        if (((Campo)_listaComponente[i]).Funciones[j].Parametros != null)
                        {
                            _campo += ((Campo)_listaComponente[i]).Funciones[j].Parametros[0];
                            for (int k = 1; k < ((Campo)_listaComponente[i]).Funciones[j].Parametros.Length; k++)
                                _campo += "," + ((Campo)_listaComponente[i]).Funciones[j].Parametros[k];
                        }
                        _campo += ")";

                    }
                    //AC localContext.Trace("Campo " + _campo);
                    _valor += ((Campo)_listaComponente[i]).calculaValor(_secuencia, _ef, _valorRegla);
                }
                //AC localContext.Trace("_valor = " + _valor);
            }

            localContext.Trace("Secuencia = " + _valor);

            return _valor;
        }

        /**
        // <summary>
        // Punto de entrada del plugin.
        // </summary>
        // <param name="serviceProvider">The service provider.</param>
        // <remarks>
        // Se ejecuta en la creación de registros de aquellas entidades para las que se haya definido un paso en este plugin. 
        // Busca en atos_secuencia la definición de la secuencia (asociada a la entidad del registro que se está creando). 
        // Con ese registro construye el valor de la secuencia.
        // </remarks>
         */
        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            // Construct the Local plug-in context.
            localContext = new LocalPluginContext(serviceProvider);

            //Ac localContext.Trace(string.Format(CultureInfo.InvariantCulture, "Entered {0}.Execute()", this.GetType().ToString()));

            // 1108-2022 -1 */
            // localContext.Trace(string.Format("Plugin Secuencias {0}", DateTime.Now.ToLocalTime().ToString()));

            /* PRO-XXX -1 */
            //localContext.Trace("Mensaje: " + localContext.PluginExecutionContext.MessageName);

            if (localContext.PluginExecutionContext.MessageName != "Create")
                return;

            Entity ef = (Entity)localContext.PluginExecutionContext.InputParameters["Target"];
            if (ef.LogicalName == "atos_secuencia")
                return;
            //AC localContext.Trace("No es atos_secuencia: " + ef.LogicalName);

            EntityCollection _secuencias = secuencias(ef.LogicalName);

            for (int nsecuencia = 0; nsecuencia < _secuencias.Entities.Count; nsecuencia++)
            {
                //Entity _secuencia = secuencia(ef.LogicalName);
                Entity _secuencia = _secuencias.Entities[nsecuencia];
                /*if (_secuencia == null)
                    return;
                if (!_secuencia.Attributes.Contains("atos_campo")) // Secuencia no configurada
                    return;
                if (ef.Attributes.Contains(_secuencia.Attributes["atos_campo"].ToString())) // Si viene ya un valor que no calcule nada
                    return;*/

                if (_secuencia.Attributes.Contains("atos_campo") &&
                    !ef.Attributes.Contains(_secuencia.Attributes["atos_campo"].ToString())) // Si viene con un valor no calcula nada
                {
                    //Entity _entidad = localContext.OrganizationService.Retrieve(ef.LogicalName, ef.Id, new ColumnSet(_secuencia.Attributes["atos_campo"].ToString()));

                    //AC localContext.Trace("Formato: " + _secuencia.Attributes["atos_formato"].ToString());
                    List<Componente> _listaComponente = reglas(_secuencia.Attributes["atos_formato"].ToString());
                    List<Componente> _reglasSecuencia = reglas(_secuencia.Attributes["atos_camporegla"].ToString());

                    String _valorReglaSecuencia = calculaValorRegla(_secuencia, ef, _reglasSecuencia, null);
                    String _valorSecuencia = "";
                    Boolean _duplicado = true;
                    Boolean _controlDuplicados = true;

                    if (_secuencia.Attributes.Contains("atos_controlduplicados"))
                    {
                        _controlDuplicados = (Boolean)_secuencia.Attributes["atos_controlduplicados"];
                    }
                    for (int intentos = 0; intentos < 10; intentos++)
                    {
                        _valorSecuencia = calculaValorRegla(_secuencia, ef, _listaComponente, _valorReglaSecuencia);
                        //ef.Attributes[_secuencia.Attributes["atos_campo"].ToString()] = _valorSecuencia;
                        if (!_controlDuplicados)
                        {
                            ef.Attributes[_secuencia.Attributes["atos_campo"].ToString()] = _valorSecuencia;
                            _duplicado = false;
                            break;
                        }
                        if (!_controlDuplicados ||
                            (_controlDuplicados && !estaDuplicado(ef.LogicalName, _secuencia.Attributes["atos_campo"].ToString(), _valorSecuencia)))
                        {
                            ef.Attributes[_secuencia.Attributes["atos_campo"].ToString()] = _valorSecuencia;
                            _duplicado = false;
                            break;
                        }

                        /*_entidad.Attributes[_secuencia.Attributes["atos_campo"].ToString()] = _valorSecuencia;
                        localContext.Trace("Valor secuencia " + _valorSecuencia);
                        if (!estaDuplicado(_entidad))
                        {
                            _duplicado = false;
                            break;
                        } */
                    }

                    //    throw new System.Exception("Error provocado");

                    if (_duplicado)
                        throw new System.Exception("Codigo duplicado: " + _valorSecuencia + " en la entidad " + ef.LogicalName);
                    //ef.Attributes[_secuencia.Attributes["atos_campo"].ToString()] = _valorSecuencia;
                    // ¿En estaDuplicado ya se está actualizando?
                    //_entidad.Attributes[_secuencia.Attributes["atos_campo"].ToString()] = _valorSecuencia;
                    //localContext.OrganizationService.Update(_entidad);
                }
            }
        }
    }
    /**
    // <summary>
    // Método delegado para llamar a la función que corresponda. El miembro de la clase se inicializa en el constructor.
    // </summary>
     */
    public delegate String DelCalculoFuncion(Entity _ef, String _valor);

    /**
    // <summary>
    // Clase que "parsea" las distintas funciones disponibles para generar la secuencia.
    // </summary>
    // <remarks>
    // Las funciones disponibles para utilizar en las fórmulas de secuencias son:<br/>
    // <ul>
    //   <li>Secuencias.Funcion.ToLower
    //   <li>Secuencias.Funcion.ToUpper
    //   <li>Secuencias.Funcion.PadZero
    //   <li>Secuencias.Funcion.PadLeft
    //   <li>Secuencias.Funcion.PadRight
    //   <li>Secuencias.Funcion.First
    //   <li>Secuencias.Funcion.Last
    //   <li>Secuencias.Funcion.Substring
    //   <li>Secuencias.Funcion.Now   (Global, no aplica a campos)
    //   <li>Secuencias.Funcion.FormatDate  
    //   <li>Secuencias.Funcion.LlaveDerecha   (Global, no aplica a campos)
    //   <li>Secuencias.Funcion.LlaveIzquierda   (Global, no aplica a campos)
    //   <li>Secuencias.Funcion.Almohadilla  (Global, no aplica a campos)
    //   <li>Secuencias.Funcion.Punto   (Global, no aplica a campos)
    //   <li>Secuencias.Funcion.Barra   (Global, no aplica a campos)
    //   <li>Secuencias.Funcion.Lookup  
    //   <li>Secuencias.Funcion.Iif
    //   <li>Secuencias.Funcion.NVL
    //   <li>Secuencias.Funcion.Concatenate
    //   <li>Secuencias.Funcion.PreConcatenate 
    //   <li>Secuencias.Funcion.ConcatIfNotNull
    //   <li>Secuencias.Funcion.PreConcatIfNotNull 
    //   <li>Secuencias.Funcion.Literal  (Global, no aplica a campos)
    // </ul>
    // </remarks>
     */
    class Funcion
    {
        private String nb_funcion;
        private String[] parametros;
        private IOrganizationService service;
        private ITracingService trace;
        public DelCalculoFuncion calcula;


        /**
        // <summary>
        // Recupera el tipo de un campo. Función interna.
        // </summary>
        // <param name="_entity">Entidad de la que se recupera el valor del campo</param>
        // <param name="_field">Nombre del campo del que se quiere recuperar el valor</param>
        // <param name="_service">IOrganizationService.</param>
         */
        static public AttributeTypeCode retrieveAttributeType(String _entity, String _field, IOrganizationService _service)
        {
            RetrieveAttributeRequest attributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = _entity,
                LogicalName = _field,
                RetrieveAsIfPublished = true
            };

            // Execute the request
            RetrieveAttributeResponse attributeResponse = (RetrieveAttributeResponse)_service.Execute(attributeRequest);
            return attributeResponse.AttributeMetadata.AttributeType.Value;
        }

        /**
        // <summary>
        // Recupera el valor de un campo. Función interna
        // </summary>
        // <param name="_id">Guid del registro</param>
        // <param name="_entity">Entidad de la que se recupera el valor del campo</param>
        // <param name="_field">Nombre del campo del que se quiere recuperar el valor</param>
        // <param name="_service">IOrganizationService.</param>
        // <param name="_trace">ITracingService</param>
         */
        static public String retrieveValue(Guid _id, String _entity, String _field, IOrganizationService _service, ITracingService _trace)
        {
            String _value="";
            Guid _lookupId = _id;
            Entity _lookup = _service.Retrieve(_entity, _lookupId, new ColumnSet(new String[] { _field }));

            if (_lookup.Attributes.Contains(_field))
            {
                AttributeTypeCode _type = Funcion.retrieveAttributeType(_entity, _field, _service);
                //AC _trace.Trace("retrieveValue field: " + _field + " _type: " + _type.ToString());
                if (_type == AttributeTypeCode.Lookup)
                    _value = ((EntityReference)_lookup.Attributes[_field]).Id.ToString();
                else if (_type == AttributeTypeCode.Picklist)
                    _value = _lookup.FormattedValues[_field];
                else
                    _value = _lookup.Attributes[_field].ToString();
                
            }
            return _value;
        }


        /**
        // <summary>
        // Recupera el valor de un campo. Función interna.
        // </summary>
        // <param name="_entity">Entidad de la que se recupera el valor del campo</param>
        // <param name="_field">Nombre del campo del que se quiere recuperar el valor</param>
        // <param name="_service">IOrganizationService.</param>
        // <param name="_trace">ITracingService</param>
         */
        static public String retrieveValue(Entity _entity, String _field, IOrganizationService _service, ITracingService _trace)
        {
            String _value = "";
            
            if (_entity.Attributes.Contains(_field))
            {
                AttributeTypeCode _type = Funcion.retrieveAttributeType(_entity.LogicalName, _field, _service);
                //AC _trace.Trace("retrieveValue field: " + _field + " _type: " + _type.ToString());
                if (_type == AttributeTypeCode.Lookup)
                    _value = ((EntityReference)_entity.Attributes[_field]).Id.ToString();
                else if (_type == AttributeTypeCode.Picklist)
                    _value = _entity.FormattedValues[_field];
                else
                    _value = _entity.Attributes[_field].ToString();

            }
            return _value;
        }

        /**
        // <summary>
        // Devuelve el objeto service (IOrganizationService). Función interna.
        // </summary>
         */
        public IOrganizationService Service
        {
            get
            {
                return this.service;
            }
        }

        /**
        // <summary>
        // Devuelve el objeto trace (ITracingService). Función interna.
        // </summary>
         */
        public ITracingService Trace
        {
            get
            {
                return this.trace;
            }
        }

        /**
        // <summary>
        // Devuelve el nombre de la función. Función interna.
        // </summary>
         */
        public String Nb_Funcion
        {
            get
            {
                return this.nb_funcion;
            }
        }

        /**
        // <summary>
        // Devuelve el array de parámetros. Función interna.
        // </summary>
         */
        public String[] Parametros
        {
            get
            {
                return this.parametros;
            }
        }


        /**
        // <summary>
        // Recupera el valor de un campo. Función interna.
        // </summary>
        // <param name="_ef">Entidad de la que se recupera el valor del campo</param>
        // <param name="_valor">Literal o campo del que debe recuperar el valor</param>
        // <remarks>
        // - Si el parámetro _valor es un campo debe ser de la forma:
        //   - campolookup!entidad!camporecuperar (campolookup es un campo de _ef, con el que se accede a entidad para obtener el valor de camporecuperar)
        //   - campo!! (campo es un campo de _ef)
        // - Si no es un campo (no contiene !) se devuelve directamente el contenido del parámetro
        // </remarks>
         */
        private String valor(Entity _ef, String _valor)
        {
            if (_valor == "")
                return _valor;
            String[] _campo = _valor.Split('!');
            if (_campo.Length == 1)
                return _valor;

            if (_campo.Length != 3 || _campo[0] == "")
                throw new System.Exception("El parámetro debe ser un literal o bien un campo con el formato campoid!entidad!campomostrar si es un lookup o campo!! si no lo es");

            if (!_ef.Attributes.Contains(_campo[0]))
                return "";

            if (_campo[1] == "" || _campo[2] == "")
                return Funcion.retrieveValue(_ef, _campo[0], service, trace);

            return Funcion.retrieveValue(((EntityReference)_ef.Attributes[_campo[0]]).Id, _campo[1], _campo[2], service, trace);
        }


        /**
        // <summary>
        // Constructor de la clase Funcion. Función interna.
        // </summary>
        // <param name="_cadena">Cadena que contiene el nombre de la función a aplicar y sus parámetros</param>
        // <param name="_service">IOrganizationService</param>
        // <param name="_trace">ITracingService</param>
        // <remarks>
        // - Extrae de _cadena el nombre de la función (todo lo que hay antes de '(') e inicializa el atributo nb_funcion
        // - Recupera los parámetros de _cadena (se encuentran entre '(' y ')' y separados por ,) y los almacena en el array parametros
        // - Asigna la función correspondiente (asignaDelegado) en función del nombre de la función
        // </remarks>
         */
        public Funcion(String _cadena, IOrganizationService _service, ITracingService _trace)
        {
            nb_funcion = _cadena.Substring(0, _cadena.IndexOf('('));
            parametros = null;
            service = _service;
            trace = _trace;

            if (_cadena.IndexOf(')') > _cadena.IndexOf('(') + 1)
                parametros = _cadena.Substring(_cadena.IndexOf('(') + 1, _cadena.IndexOf(')') - _cadena.IndexOf('(') - 1).Split(',');
			this.asignaDelegado();
        }

        /**
        // <summary>
        // Convierte a minúsculas. 
        // </summary>
        // <param name="_ef">Entity</param>
        // <param name="_valor">Texto que se debe convertir a minúsculas</param>
        // <remarks>
        // <ul>
        // <li> Si el parámetro _valor es un campo de la entidad _ef recupera el valor del campo (función valor)
        // <li> Convierte a minúsculas el valor anterior (si fuera un campo) o lo recibido en el parámetro _valor
        // <li> Ejemplo:
        //   <ul>
        //      <li>{atos_cadena.ToLower()}
        //      <ul>
        //        <li>Devuelve el valor de atos_cadena convertido a minúsculas
        //        <li>Si el valor de atos_cadena fuera "Mi CaDeNa", lo que devuelve es "mi cadena"
        //      </ul>
        //   </ul>
        // </ul>
        // </remarks>
         */
        private String ToLower(Entity _ef, String _valor)
        {
            String _valoraux = this.valor(_ef, _valor);
            return _valoraux.ToLower();
			//return _valor.ToLower();
		}

        /**
        // <summary>
        // Convierte a mayúsculas
        // </summary>
        // <param name="_ef">Entity</param>
        // <param name="_valor">Texto que se debe convertir a mayúsculas</param>
        // <remarks>
        // <ul>
        // <li>Si el parámetro _valor es un campo de la entidad _ef recupera el valor del campo (función valor)
        // <li>Convierte a mayúsculas el valor anterior (si fuera un campo) o lo recibido en el parámetro _valor
        // <li>Ejemplo:
        //   <ul>
        //   <li>{atos_cadena.ToUpper()}
        //      <ul>
        //          <li>Devuelve el valor de atos_cadena convertido a mayúsculas
        //          <li>Si el valor de atos_cadena fuera "Mi Cadena", lo que devuelve es "MI CADENA"
        //      </ul>
        //   </ul>
        // </ul>
        // </remarks>
         */
        private String ToUpper(Entity _ef, String _valor)
		{
            String _valoraux = this.valor(_ef, _valor);
            return _valoraux.ToUpper();
			//return _valor.ToUpper();
		}

        /**
        // <summary>
        // Normaliza con 0's por la izquierda
        // </summary>
        // <param name="_ef">Entity</param>
        // <param name="_valor">Texto que se va a normalizar</param>
        // <remarks>
        // <ul>
        // <li>El array parametros de la clase debe tener una ocurrencia y además debe contener un valor entero mayor que 0. Este valor es la longitud a la que se va a normalizar la cadena de texto.
        // <li>Si el parámetro _valor es un campo de la entidad _ef recupera el valor del campo (función valor)
        // <li>Normaliza con ceros por la izquierda a la longitud indicada por la primera ocurrencia de parámetros el valor anterior (si fuera un campo) o lo recibido en el parámetro _valor
        // <li>Ejemplo:
        //     <ul>
        //     <li>{atos_cadena.PadZero(10)}
        //         <ul>
        //         <li>Devuelve el valor de atos_cadena, con 0's por la izquierda hasta totalizar 10 caracteres
        //         <li>Si el valor de atos_cadena fuera 47, lo que devuelve es "0000000047"
        //         </ul>
        //     </ul>
        // </ul>
        // </remarks>
         */
        private String PadZero(Entity _ef, String _valor)
		{
			if (parametros.Length != 1)
				throw new System.Exception("Función PadZero debe tener un parámetro");
			int _tam = System.Convert.ToInt32(parametros[0]);
			if (_tam <= 0)
				throw new System.Exception("Función PadZero debe tener un parámetro numérico mayor que 0");

            String _valoraux = this.valor(_ef, _valor);

            return _valoraux.PadLeft(_tam, '0');
            
			//return _valor.PadLeft(_tam, '0');
		}

        /**
        // <summary>
        // Normaliza por la izquierda
        // </summary>
        // <param name="_ef">Entity</param>
        // <param name="_valor">Texto que se va a normalizar</param>
        // <remarks>
        // <ul>
        // <li>El array parametros de la clase debe tener dos ocurrencias
        //   <ol>
        //   <li>La primera debe contener un valor entero mayor que 0. Este valor es la longitud a la que se va a normalizar la cadena de texto.
        //   <li>La segunda debe contener un único carácter. Este valor es el carácter de "relleno" para la normalización
        //   </ol>
        // <li>Si el parámetro _valor es un campo de la entidad _ef recupera el valor del campo (función valor)
        // <li>Normaliza con el carácter de relleno por la izquierda a la longitud indicada por la primera ocurrencia de parámetros el valor anterior (si fuera un campo) o lo recibido en el parámetro _valor
        // <li>Ejemplo:
        //   <ul>
        //   <li>{atos_cadena.PadLeft(10,+)}
        //     <ul>
        //     <li>Devuelve el valor de atos_cadena, con el carácter '+' por la izquierda hasta totalizar 10 caracteres
        //     <li>Si el valor de atos_cadena fuera "C23", lo que devuelve es "+++++++C23"
        //     </ul>
        //   </ul>
        // </ul>
        // </remarks>
         */
        private String PadLeft(Entity _ef, String _valor)
		{
			if (parametros.Length != 2)
				throw new System.Exception("Función PadLeft debe tener dos parámetros");
			int _tam = System.Convert.ToInt32(parametros[0]);
			if (_tam <= 0)
				throw new System.Exception("Función PadLeft. El primer parámetro debe ser numérico y mayor que 0");
			if (parametros[1].Length > 1)
				throw new System.Exception("Función PadLeft. El segundo parámetro debe ser un carácter");
			char _relleno = System.Convert.ToChar(parametros[1]);

            String _valoraux = this.valor(_ef, _valor);

            return _valoraux.PadLeft(_tam, _relleno);
			//return _valor.PadLeft(_tam, _relleno);
		}

        /**
        // <summary>
        // Normaliza por la derecha
        // </summary>
        // <param name="_ef">Entity</param>
        // <param name="_valor">Texto que se va a normalizar</param>
        // <remarks>
        // <ul>
        // <li>El array parametros de la clase debe tener dos ocurrencias
        //   <ol>
        //   <li>La primera debe contener un valor entero mayor que 0. Este valor es la longitud a la que se va a normalizar la cadena de texto.
        //   <li>La segunda debe contener un único carácter. Este valor es el carácter de "relleno" para la normalización
        //   </ol>
        // <li>Si el parámetro _valor es un campo de la entidad _ef recupera el valor del campo (función valor)
        // <li>Normaliza con el carácter de relleno por la derecha a la longitud indicada por la primera ocurrencia de parámetros el valor anterior (si fuera un campo) o lo recibido en el parámetro _valor
        // <li>Ejemplo:
        //   <ul>
        //   <li>{atos_cadena.PadRight(10,-)}
        //     <ul>
        //     <li>Devuelve el valor de atos_cadena, con el carácter '-' por la derecha hasta totalizar 10 caracteres
        //     <li>Si el valor de atos_cadena fuera "Prueba, lo que devuelve es "Prueba----"
        //     </ul>
        //   </ul>
        // </ul>
        // </remarks>
         */
        private String PadRight(Entity _ef, String _valor)
		{
			if (parametros.Length != 2)
				throw new System.Exception("Función PadRight debe tener dos parámetros");
			int _tam = System.Convert.ToInt32(parametros[0]);
			if (_tam <= 0)
				throw new System.Exception("Función PadRight. El primer parámetro debe ser numérico y mayor que 0");
			if (parametros[1].Length > 1)
				throw new System.Exception("Función PadRight. El segundo parámetro debe ser un carácter");
			char _relleno = System.Convert.ToChar(parametros[1]);

            String _valoraux = this.valor(_ef, _valor);

            return _valoraux.PadRight(_tam, _relleno);

			// return _valor.PadRight(_tam, _relleno);
		}

        /**
        // <summary>
        // Recupera los primeros caracteres de una cadena de texto
        // </summary>
        // <param name="_ef">Entity</param>
        // <param name="_valor">Texto del que se van a recuperar los primeros caracteres</param>
        // <remarks>
        // <ul>
        // <li>El array parametros de la clase debe tener una ocurrencia y además debe contener un valor entero mayor que 0. Este valor es el número de caracteres que se van a recuperar.
        // <li>Si el parámetro _valor es un campo de la entidad _ef recupera el valor del campo (función valor)
        // <li>Recupera los primeros n caracteres del valor anterior (si fuera un campo) o lo recibido en el parámetro _valor, siendo n lo indicado por la primera ocurrencia de parámetros
        // <li>Ejemplo:
        //   <ul>
        //   <li>{atos_name.First(10)}
        //     <ul>
        //     <li>Devuelve los primeros 10 caracteres de atos_name
        //     <li>Si el valor de atos_name fuera "Instalación de prueba 25", lo que devuelve es "Instalació"
        //     </ul>
        //   </ul>
        // </ul>
        // </remarks>
         */
        private String First(Entity _ef, String _valor)
		{
			if (parametros.Length != 1)
				throw new System.Exception("Función First debe tener un parámetro");
			int _tam = System.Convert.ToInt32(parametros[0]);
			if (_tam <= 0)
				throw new System.Exception("Función First debe tener un parámetro numérico mayor que 0");


            String _valoraux = this.valor(_ef, _valor);
            if (_valoraux.Length < _tam)
                return _valoraux;

            return _valoraux.Substring(0, _tam);

			/*if (_valor.Length < _tam)
				return _valor;

			return _valor.Substring(0, _tam);*/
		}

        /**
        // <summary>
        // Recupera los últimos caracteres de una cadena de texto
        // </summary>
        // <param name="_ef">Entity</param>
        // <param name="_valor">Texto del que se van a recuperar los últimos caracteres</param>
        // <remarks>
        // <ul>
        // <li>El array parametros de la clase debe tener una ocurrencia y además debe contener un valor entero mayor que 0. Este valor es el número de caracteres que se van a recuperar.
        // <li>Si el parámetro _valor es un campo de la entidad _ef recupera el valor del campo (función valor)
        // <li>Recupera los últimos n caracteres del valor anterior (si fuera un campo) o lo recibido en el parámetro _valor, siendo n lo indicado por la primera ocurrencia de parámetros
        // <li>Ejemplo:
        //   <ul>
        //   <li>{atos_name.Last(2)}
        //     <ul>
        //     <li>Devuelve los últimos 2 caracteres de atos_name
        //     <li>Si el valor de atos_name fuera "Instalación de prueba 25", lo que devuelve es "25"
        //     </ul>
        //   </ul>
        // </ul>
        // </remarks>
         */
        private String Last(Entity _ef, String _valor)
		{
			if (parametros.Length != 1)
				throw new System.Exception("Función Last debe tener un parámetro");
			int _tam = System.Convert.ToInt32(parametros[0]);
			if (_tam <= 0)
				throw new System.Exception("Función Last debe tener un parámetro numérico mayor que 0");


            String _valoraux = this.valor(_ef, _valor);
            if (_valoraux.Length < _tam)
                return _valoraux;

            return _valoraux.Substring(_valoraux.Length - _tam, _tam);

			/*if (_valor.Length < _tam)
				return _valor;

			return _valor.Substring(_valor.Length - _tam, _tam);*/
		}

        /**
        // <summary>
        // Recupera un "trozo" de una cadena de texto
        // </summary>
        // <param name="_ef">Entity</param>
        // <param name="_valor">Texto del que se va a recuperar el "trozo"</param>
        // <remarks>
        // <ul>
        // <li>El array parametros de la clase debe tener dos ocurrencias, la primera indica la posición donde cogeremos la subcadena y la segunda indica el tamaño de la subcadena.
        // <li>Si el parámetro _valor es un campo de la entidad _ef recupera el valor del campo (función valor)
        // <li>La posición debe ser mayor o igual que cero y menor que la longitud de la cadena de la que tiene que "recortar" la subcadena y el tamaño debe ser mayor que 0
        // <li>Recupera los n caracteres del valor anterior (si fuera un campo) o lo recibido en el parámetro _valor, comenzando en lo indicado por el "parámetro" posición, siendo n lo indicado por la segunda ocurrencia de parámetros
        // <li>Ejemplo:
        //   <ul>
        //   <li>{atos_name.Substring(15,6)}
        //     <ul>
        //     <li>Devuelve el "trozo" de 6 caracteres de atos_name que comienza en la posición 15 (decimosexta pues la numeración comienza en 0)
        //     <li>Si el valor de atos_name fuera "Instalación de prueba 25", lo que devuelve es "prueba"
        //     </ul>
        //   </ul>
        // </ul>
        // </remarks>
         */
        private String Substring(Entity _ef, String _valor)
		{
			if (parametros.Length != 2)
				throw new System.Exception("Función Substring debe tener dos parámetros");

			int _pos = System.Convert.ToInt32(parametros[0]);
			int _tam = System.Convert.ToInt32(parametros[1]);
			if (_pos < 0)
				throw new System.Exception("Función Substring. El primer parámetro (posición) debe ser numérico y mayor o igual que 0");
			if (_tam <= 0)
				throw new System.Exception("Función Substring. El segundo parámetro (longitud) debe ser numérico y mayor que 0");

            String _valoraux = this.valor(_ef, _valor);
            if (_valoraux.Length < _pos)
                return _valoraux;

            return _valoraux.Substring(_pos, _tam);
			/*if (_valor.Length < _pos)
				return _valor;

			return _valor.Substring(_pos, _tam);*/
		}


        /**
        // <summary>
        // Da formato a una fecha recibida (como string) por parámetro
        // </summary>
        // <param name="_ef">Entity. Sin uso. Necesario porque se llama desde un método delegado</param>
        // <param name="_valor">String que contiene una fecha/hora</param>
        // <remarks>
        // <ul>
        // <li>El array parametros de la clase debe tener una ocurrencia, que es el formato de la fecha.
        // <li>Si el parámetro _valor es un campo de la entidad _ef recupera el valor del campo (función valor)
        // <li>Transforma el valor anterior (si fuera un campo) o lo recibido en el parámetro _valor a una variable de tipo DateTime y lo formatea (ToString) según lo indicado por la primera ocurrencia de parámetros
        // <li>Ejemplos:
        //   <ol>
        //   <li>{createdon.FormatDate(yyyyMMdd)}
        //     <ul>
        //     <li>Devuelve la fecha del día en formato yyyyMMdd
        //     </ul>
        //   <li>{createdon.FormatDate(yyyyMM)}
        //     <ul>
        //     <li>Devuelve la fecha del día en formato yyyyMM
        //     </ul>
        //   </ol>
        // </ul>
        // </remarks>
         */
        private String FormatDate(Entity _ef, String _valor)
		{
			if (parametros != null)
				if (parametros.Length == 1)
					return Convert.ToDateTime(_valor).ToString(parametros[0]);
			return _valor;

		}


        /**
        // <summary>
        // Devuelve la fecha del día
        // </summary>
        // <param name="_ef">Entity. Sin uso. Necesario porque se llama desde un método delegado</param>
        // <param name="_valor">Sin uso. Necesario porque se llama desde un método delegado</param>
        // <remarks>
        // <ul>
        // <li>Si el array parametros de la clase tiene una ocurrencia, entonces es el formato de la fecha, si no el formato que se aplica es yyyyMMdd
        // <li>Calcula la fecha del día y la convierte a un string utilizando el formato anterior (el de parametros o si no existe el por defecto)
        // <li>Ejemplos:
        //   <ol>
        //   <li>Now()
        //     <ul>
        //     <li>Devuelve la fecha del día en formato yyyyMMdd
        //     </ul>
        //   <li>Now(yyyyMM)
        //     <ul>
        //     <li>Devuelve la fecha del día en formato yyyyMM
        //     </ul>
        //   </ol>
        // </ul>
        // </remarks>
         */
        private String Now(Entity _ef, String _valor)
		{
			String _formatoFecha = "yyyyMMdd";
			if (parametros != null)
				if (parametros.Length == 1)
					_formatoFecha = parametros[0];
			return DateTime.Now.ToString(_formatoFecha);
		}


        /**
        // <summary>
        // Devuelve una constante
        // </summary>
        // <param name="_ef">Entity. Sin uso. Necesario porque se llama desde un método delegado</param>
        // <param name="_valor">Sin uso. Necesario porque se llama desde un método delegado</param>
        // <remarks>
        // <ul>
        // <li>El array parametros de la clase debe tener una ocurrencia que contenga el texto que debe devolver esta función
        // <li>Ejemplo:
        //   <ul>
        //   <li>Literal(Contrato)
        //     <ul>
        //     <li>Devuelve el texto "Contrato"
        //     </ul>
        //   </ul>
        // </ul>
        // </remarks>
         */
        private String Literal(Entity _ef, String _valor)
		{
			if (parametros != null)
				if (parametros.Length == 1)
					return parametros[0];
			return _valor;
		}

        /**
        // <summary>
        // Función para obtener el carácter }
        // </summary>
        // <param name="_ef">Entity. Sin uso. Necesario porque se llama desde un método delegado</param>
        // <param name="_valor">Sin uso. Necesario porque se llama desde un método delegado</param>
        // <remarks>
        // <ul>
        // <li>El carácter } es reservado para la creación de las "fórmulas" de secuencias y no se puede poner explícitamente, por eso se crea esta función
        // <li>Ejemplo:
        //   <ul>
        //   <li>LlaveDerecha()
        //     <ul>
        //     <li>Devuelve "}"
        //     </ul>
        //   </ul>
        // </ul>
        // </remarks>
         */
        private String LlaveDerecha(Entity _ef, String _valor)
		{
			return "}";
		}

        /**
        // <summary>
        // Función para obtener el carácter {
        // </summary>
        // <param name="_ef">Entity. Sin uso. Necesario porque se llama desde un método delegado</param>
        // <param name="_valor">Sin uso. Necesario porque se llama desde un método delegado</param>
        // <remarks>
        // <ul>
        // <li>El carácter { es reservado para la creación de las "fórmulas" de secuencias y no se puede poner explícitamente, por eso se crea esta función
        // <li>Ejemplo:
        //   <ul>
        //   <li>LlaveIzquierda()
        //     <ul>
        //     <li>Devuelve "{"
        //     </ul>
        //   </ul>
        // </ul>
        // </remarks>
         */
        private String LlaveIzquierda(Entity _ef, String _valor)
		{
			return "{";
		}

        /**
        // <summary>
        // Función para obtener el carácter #
        // </summary>
        // <param name="_ef">Entity. Sin uso. Necesario porque se llama desde un método delegado</param>
        // <param name="_valor">Sin uso. Necesario porque se llama desde un método delegado</param>
        // <remarks>
        // <ul>
        // <li>El carácter # es reservado para la creación de las "fórmulas" de secuencias y no se puede poner explícitamente, por eso se crea esta función
        // <li>Ejemplo:
        //   <ul>
        //   <li>Almohadilla()
        //     <ul>
        //     <li>Devuelve "#"
        //     </ul>
        //   </ul>
        // </ul>
        // </remarks>
         */
        private String Almohadilla(Entity _ef, String _valor)
		{
			return "#";
		}

        /**
        // <summary>
        // Función para obtener el carácter .
        // </summary>
        // <param name="_ef">Entity. Sin uso. Necesario porque se llama desde un método delegado</param>
        // <param name="_valor">Sin uso. Necesario porque se llama desde un método delegado</param>
        // <remarks>
        // <ul>
        // <li>El carácter . es reservado para la creación de las "fórmulas" de secuencias y no se puede poner explícitamente, por eso se crea esta función
        // <li>Ejemplo:
        //   <ul>
        //   <li>Punto()
        //     <ul>
        //     <li>Devuelve "."
        //     </ul>
        //   </ul>
        // </ul>
        // </remarks>
         */
        private String Punto(Entity _ef, String _valor)
		{
			return ".";
		}

        /**
        // <summary>
        // Función para obtener el carácter |
        // </summary>
        // <param name="_ef">Entity. Sin uso. Necesario porque se llama desde un método delegado</param>
        // <param name="_valor">Sin uso. Necesario porque se llama desde un método delegado</param>
        // <remarks>
        // <ul>
        // <li>El carácter | es reservado para la creación de las "fórmulas" de secuencias y no se puede poner explícitamente, por eso se crea esta función
        // <li>Ejemplo:
        //   <ul>
        //   <li>Barra()
        //     <ul>
        //     <li>Devuelve "|"
        //     </ul>
        //   </ul>
        // </ul>
        // </remarks>
         */
        private String Barra(Entity _ef, String _valor)
		{
			return "|";
		}

        /**
        // <summary>
        // Función para obtener el carácter !
        // </summary>
        // <param name="_ef">Entity. Sin uso. Necesario porque se llama desde un método delegado</param>
        // <param name="_valor">Sin uso. Necesario porque se llama desde un método delegado</param>
        // <remarks>
        // <ul>
        // <li>El carácter ! es reservado para la creación de las "fórmulas" de secuencias y no se puede poner explícitamente, por eso se crea esta función
        // <li>Ejemplo:
        //   <ul>
        //   <li>Exclamacion()
        //     <ul>
        //     <li>Devuelve "!"
        //     </ul>
        //   </ul>
        // </ul>
        // </remarks>
         */
        private String Exclamacion(Entity _ef, String _valor)
		{
			return "!";
		}

        /**
        // <summary>
        // Evalúa unas condiciones para el valor recibido por parámetro devolviendo otros valores según la condición que se cumpla
        // </summary>
        // <param name="_ef">Entity.</param>
        // <param name="_valor">Valor sobre el que se aplica la función</param>
        // <remarks>
        // <ul>
        // <li>Función similar al Decode.
        // <li>El array parametros debe contener un número impar de elementos (como mínimo deben ser 3).
        // <li>Los elementos impares son las "condiciones" (el valor con el que se compara el parámetro _valor)
        // <li>Los elementos pares son lo que debe devolver si se cumple la "condición" anterior
        // <li>El último elemento es lo que devuelve si no se cumple ninguna condición
        // <li>Ejemplos:
        //   <ol>
        //   <li>{atos_tipooferta.Iif(Multipunto,M/,Suboferta,S/,O/)}
        //     <ul>
        //     <li>Si atos_tipooferta es Multipunto devuelve el texto "M/"
        //     <li>Si atos_tipooferta es Suboferta devuelve el texto "S/"
        //     <li>Si atos_tipooferta no es ni Multipunto ni Suboferta devuelve el texto "O/"
        //     </ul>
        //   <li>{atos_tipooferta.Iif(Oferta,atos_instalacionid!atos_instalacion!atos_codigoinstalacion,atos_razonsocialid!account!accountnumber)}
        //     <ul>
        //     <li>Si atos_tipooferta es Oferta devuelve el código de instalación
        //     <li>Si atos_tipooferta no es Oferta devuelve el código de razón social
        //     </ul>
        //   </ol>
        // </ul>
        // </remarks>
         */
        private String Iif(Entity _ef, String _valor)
		{
			if ( parametros.Length < 3 )
				throw new System.Exception("Función Iif debe tener al menos tres parámetros");
			if ( parametros.Length % 2 != 1 )
				throw new System.Exception("Función Iif debe tener un número de parámetros impar");

			for (int i = 0; i < parametros.Length - 1; i += 2)
			{
                if (_valor == parametros[i])
                {
                    //AC trace.Trace("Parámetro (" + (i + 1).ToString() + "): " + parametros[i + 1]);
                    return valor(_ef, parametros[i + 1]);
                }
					//return parametros[i + 1];
			}

            //AC trace.Trace("Parámetro (" + (parametros.Length - 1).ToString() + "): " + parametros[parametros.Length - 1]);
            return valor(_ef, parametros[parametros.Length - 1]);
			//return parametros[parametros.Length - 1];
		}

        /**
        // <summary>
        // Añade a continuación del valor recibido por parámetro, los valores de los parámetros de la función
        // </summary>
        // <param name="_ef">Entity.</param>
        // <param name="_valor">Valor sobre el que se aplica la función</param>
        // <remarks>
        // <ul>
        // <li>El array parametros debe contener al menos un elemento.
        // <li>Para cada uno de los elementos de parametros calcula su valor y lo añade a continuación de _valor
        // <li>Ejemplo:
        //   <ul>
        //   <li>{atos_cadena.Concatenate(/,atos_campo2!!,-,atos_razonsocialid!account!atos_name)}
        //     <ul>
        //     <li>Si el valor de atos_cadena fuera "Cadena 1", el de atos_campo2 fuera "Campo 2" y el nombre de la razón social fuera "Cliente 1" devolvería la cadena:
        //       <ul>
        //       <li>"Cadena 1/Campo 2-Cliente 1"
        //       </ul>
        //     </ul>
        //   </ul>
        // </ul>
        // </remarks>
         */
        private String Concatenate(Entity _ef, String _valor)
        {
            String _valoraux = _valor;
            if (parametros.Length < 1)
                throw new System.Exception("Función Concatenate debe tener al menos un parámetro");
            for (int i = 0; i < parametros.Length; i++)
            {
                _valoraux += this.valor(_ef, parametros[i]);
            }
            return _valoraux;
        }


        /**
        // <summary>
        // Añade a continuación del valor recibido por parámetro, los valores de los parámetros de la función siempre que el valor recibido no sea nulo
        // </summary>
        // <param name="_ef">Entity.</param>
        // <param name="_valor">Valor sobre el que se aplica la función</param>
        // <remarks>
        // <ul>
        // <li>El array parametros debe contener al menos un elemento.
        // <li>Para cada uno de los elementos de parametros calcula su valor y lo añade a continuación de _valor si _valor no es nulo
        // <li>Ejemplo:
        //   <ul>
        //   <li>{atos_cadena.ConcatIfNotNull(/,atos_campo2!!,-,atos_razonsocialid!account!atos_name)}
        //     <ul>
        //     <li>Si el valor de atos_cadena fuera "Cadena 1", el de atos_campo2 fuera "Campo 2" y el nombre de la razón social fuera "Cliente 1" devolvería la cadena:
        //       <ul>
        //       <li>"Cadena 1/Campo 2-Cliente 1"
        //       </ul>
        //     </ul>
        //   </ul>
        // </ul>
        // </remarks>
         */
        private String ConcatIfNotNull(Entity _ef, String _valor)
        {
            if (_valor == "")
                return _valor;
            return Concatenate(_ef, _valor);
        }


        /**
        // <summary>
        // Añade antes del valor recibido por parámetro, los valores de los parámetros de la función
        // </summary>
        // <param name="_ef">Entity.</param>
        // <param name="_valor">Valor sobre el que se aplica la función</param>
        // <remarks>
        // <ul>
        // <li>El array parametros debe contener al menos un elemento.
        // <li>Para cada uno de los elementos de parametros calcula su valor y lo añade antes de _valor
        // <li>Ejemplo:
        //   <ul>
        //   <li>{atos_cadena.PreConcatenate(atos_campo2!!,-,atos_razonsocialid!account!atos_name,/)}
        //     <ul>
        //     <li>Si el valor de atos_cadena fuera "Cadena 1", el de atos_campo2 fuera "Campo 2" y el nombre de la razón social fuera "Cliente 1" devolvería la cadena:
        //       <ul>
        //       <li>"Campo 2-Cliente 1/Cadena 1"
        //       </ul>
        //     </ul>
        //   </ul>
        // </ul>
        // </remarks>
         */
        private String PreConcatenate(Entity _ef, String _valor)
        {
            String _valoraux = "";
            if (parametros.Length < 1)
                throw new System.Exception("Función Concatenate debe tener al menos un parámetro");
            for (int i = 0; i < parametros.Length; i++)
            {
                _valoraux += this.valor(_ef, parametros[i]);
            }
            return _valoraux + _valor;
        }


        /**
        // <summary>
        // Añade antes del valor recibido por parámetro, los valores de los parámetros de la función siempre que el valor recibido no sea nulo
        // </summary>
        // <param name="_ef">Entity.</param>
        // <param name="_valor">Valor sobre el que se aplica la función</param>
        // <remarks>
        // <ul>
        // <li>El array parametros debe contener al menos un elemento.
        // <li>Para cada uno de los elementos de parametros calcula su valor y lo añade antes de _valor si _valor no es nulo
        // <li>Ejemplo:
        //   <ul>
        //   <li>{atos_cadena.PreConcatIfNotNull(atos_campo2!!,-,atos_razonsocialid!account!atos_name,/)}
        //     <ul>
        //     <li>Si el valor de atos_cadena fuera "Cadena 1", el de atos_campo2 fuera "Campo 2" y el nombre de la razón social fuera "Cliente 1" devolvería la cadena:
        //       <ul>
        //       <li>"Campo 2-Cliente 1/Cadena 1"
        //       </ul>
        //     </ul>
        //   </ul>
        // </ul>
        // </remarks>
         */
        private String PreConcatIfNotNull(Entity _ef, String _valor)
        {
            if (_valor != "")
                return _valor;
            return PreConcatenate(_ef, _valor);
        }

        /**
        // <summary>
        // Busca un campo en otra entidad relacionada a partir del Guid (recibido por parámetro).
        // </summary>
        // <param name="_ef">Entity.</param>
        // <param name="_valor">Valor sobre el que se aplica la función. Debe ser un Guid (transformado a string)</param>
        // <remarks>
        // <ul>
        // <li>El array parametros debe contener dos elementos, el primero el nombre de la entidad donde buscar y el segundo el campo de la entidad anterior a recuperar.
        // <li>Ejemplo:
        //   <ul>
        //   <li>{atos_razonsocialid!account!atos_cuentanegociadoraid.Lookup(atos_cuentanegociadora,createdon)}
        //     <ul>
        //     <li>Devuelve la fecha de creación de la cuenta negociadora accediendo desde el campo lookup de la razón social.
        //     </ul>
        //   </ul>
        // </ul>
        // </remarks>
         */
        private String Lookup(Entity _ef, String _valor)
		{
			if (parametros.Length != 2)
				throw new System.Exception("Función Lookup debe tener dos parámetros");
			if (_valor != "")
			{
				String _value = Funcion.retrieveValue(new Guid(_valor), parametros[0], parametros[1], service, trace);
				if (_value != "")
					return _value;
			}
			return "";
		}


        /**
        // <summary>
        // Comprueba si el valor recibido es nulo para en caso afirmativo calcular el valor del parámetro de la función
        // </summary>
        // <param name="_ef">Entity.</param>
        // <param name="_valor">Valor sobre el que se aplica la función</param>
        // <remarks>
        // <ul>
        // <li>Devuelve _valor si no está vacío
        // <li>Si _valor está vacío, entonces devuelve el valor del parámetro de la función (aplica la función interna valor al primer elemento del array parametros)
        // <li>Ejemplo:
        //   <ol>
        //   <li>{atos_razonsocialid!account!accountnumber).NVL(atos_cuentanegociadoraid!atos_cuentanegociadora!atos_codigocuentanegociadora)}
        //     <ul>
        //     <li>Si el código de razón social no es nulo devuelve el código de razón social
        //     <li>Si el código de razón social es nulo devuelve el código de la cuenta negociadora
        //     </ul>
        //   <li>{atos_tipooferta.NVL(Oferta)}
        //     <ul>
        //     <li>Si atos_tipooferta no es nulo devuelve el valor del campo tipo de oferta
        //     <li>Si atos_tipooferta es nulo devuelve "Oferta"
        //     </ul>
        //   </ol>
        // </ul>
        // </remarks>
         */
        private String NVL(Entity _ef, String _valor)
		{
			if ( _valor != "" )
				return _valor;
			if (parametros.Length != 1)
				throw new System.Exception("Función NVL debe tener un parámetro");

            return this.valor(_ef, parametros[0]);

		}

        /**
        // <summary>
        // Función para determinar qué función interna se utiliza para el cálculo. Función interna.
        // </summary>
        // <remarks>
        // Esta función se llama en el constructor e inicializa el método delegado calcula con la función que corresponda en función del nombre de la función que tiene que aplicar.
        // </remarks>
         */
        private void asignaDelegado()
		{
			if (nb_funcion == "ToLower")
				this.calcula = this.ToLower;
			else if (nb_funcion == "ToUpper")
				this.calcula = this.ToUpper;
			else if (nb_funcion == "PadZero")
				this.calcula = this.PadZero;
			else if (nb_funcion == "PadLeft")
				this.calcula = this.PadLeft;
			else if (nb_funcion == "PadRight")
				this.calcula = this.PadRight;
			else if (nb_funcion == "First")
				this.calcula = this.First;
			else if (nb_funcion == "Last")
				this.calcula = this.Last;
			else if (nb_funcion == "Substring")
				this.calcula = this.Substring;
			else if (nb_funcion == "Now")
				this.calcula = this.Now;
			else if (nb_funcion == "FormatDate")
				this.calcula = this.FormatDate;
			else if (nb_funcion == "LlaveDerecha")
				this.calcula = this.LlaveDerecha;
			else if (nb_funcion == "LlaveIzquierda")
				this.calcula = this.LlaveIzquierda;
			else if (nb_funcion == "Almohadilla")
				this.calcula = this.Almohadilla;
			else if (nb_funcion == "Punto")
				this.calcula = this.Punto;
			else if (nb_funcion == "Barra")
				this.calcula = this.Barra;
			else if (nb_funcion == "Lookup")
				this.calcula = this.Lookup;
			else if (nb_funcion == "Iif")
				this.calcula = this.Iif;
			else if (nb_funcion == "NVL")
                this.calcula = this.NVL;
            else if (nb_funcion == "Concatenate")
                this.calcula = this.Concatenate;
            else if (nb_funcion == "PreConcatenate")
                this.calcula = this.PreConcatenate;
            else if (nb_funcion == "ConcatIfNotNull")
                this.calcula = this.ConcatIfNotNull;
            else if (nb_funcion == "PreConcatIfNotNull")
                this.calcula = this.PreConcatIfNotNull;
            else
				this.calcula = this.Literal;
		}


        /**
        // <summary>
        // Calcula el valor correspondiente tras aplicarle la función a través del método delegado calcula. Función interna.
        // </summary>
        // <param name="_ef">Entity. Si _valor es un campo, este parámetro contiene su entidad</param>
        // <param name="_valor">Valor sobre el que se aplica la función</param>
         */
        public String calculaValor(Entity _ef, String _valor)
		{
			return this.calcula(_ef, _valor);			
		}
		
    }

    /**
    // <summary>
    // Clase que contiene la lista de funciones que tiene que aplicar para generar la secuencia.
    // </summary>
    // <remarks>
    // Es una clase base que puede tener dos derivadas:
    // -# Componente de tipo C-Campo. Se utiliza cuando el componente es un campo de una entidad al que se le van a aplicar un conjunto de funciones.
    // -# Componente de tipo G-Global. Se utiliza cuando el componente es una función que no se aplica a un campo de una entidad (por ejemplo para la función Now)
    // </remarks>
     */
    class Componente
    {
        private char tipo; // C - Campo, G - Función global
        private String cadena;
        private List<Funcion> funciones;
        private IOrganizationService service;
        private ITracingService trace;

        public IOrganizationService Service
        {
            get
            {
                return this.service;
            }
            set
            {
                this.service = value;
            }
        }
        public ITracingService Trace
        {
            get
            {
                return this.trace;
            }
            set
            {
                this.trace = value;
            }
        }
        public char Tipo
        {
            get
            {
                return this.tipo;
            }
            set
            {
                this.tipo = value;
            }
        }


        public String Cadena
        {
            get
            {
                return this.cadena;
            }
            set
            {
                this.cadena = value;
            }
        }

        public List<Funcion> Funciones
        {
            get
            {
                return this.funciones;
            }
            set
            {
                this.funciones = value;
            }
        }


        public String calculaValor(Entity _ef, String _valor)
        {
            String _valorFormateado = _valor;
            //AC trace.Trace("calculaValor: " + this.Tipo.ToString() + "(" + this.Cadena + "): " + _valor);
            for (int i = 0; i < funciones.Count; i++)
                _valorFormateado = funciones[i].calculaValor(_ef, _valorFormateado);
            //AC trace.Trace("calculaValor: " + this.Tipo.ToString() + "(" + this.Cadena + "): " + _valorFormateado);
            return _valorFormateado;
        }

        public void init(String _cadena)
        {
            String[] _listafunciones = _cadena.Split('.');
            for (int i = 0; i < _listafunciones.Length; i++)
            {
                this.Funciones.Add(new Funcion(_listafunciones[i], service, trace));
            }
        }
    }

	/**
	// <summary>
	// Clase que sirve para obtener el valor del campo de una entidad después de aplicarle la lista de funciones definida en la secuencia.
	// </summary>
	// <remarks>
	// Deriva de Componente
	// </remarks>
	 */
    class Campo : Componente
    {
        private String nb_campo;
        private String nb_entidad;
        private String nb_lookupdisplay;

		/**
		// <summary>
		// Método-propiedad que devuelve el nombre del campo.
		// </summary>
		 */
        public String Nb_Campo
        {
            get
            {
                return this.nb_campo;
            }
        }

		/**
		// <summary>
		// Constructor por defecto.
		// </summary>
		 */
        public Campo()
        {
            nb_campo = "";
            nb_entidad = "";
            nb_lookupdisplay = "";
            this.Funciones = new List<Funcion>();

            this.Tipo = 'C';
            this.Cadena = "";
        }

        public Campo(String _cadena, IOrganizationService _service, ITracingService _trace)
        {
            nb_campo = "";
            nb_entidad = "";
            nb_lookupdisplay = "";
            this.Funciones = new List<Funcion>();

            this.Tipo = 'C';
            this.Cadena = _cadena;
            this.Service = _service;
            this.Trace = _trace;
            initCampo(_cadena);
        }

        /**
        // <summary>
        // Función que inicializa los atributos de la clase a partir del trozo correspondiente de la definición de la cadena.
        // </summary>
        // <param name="_cadena">Cadena que contiene el nombre del campo a mostrar.</param>
        // <remarks>
        // - El parámetro _cadena puede ser:
        //   -# CampoId!Entidad!CampoMostrar.Funcion
        //     - Se trata de un Lookup, CampoId es el nombre del lookup, Entidad es la entidad donde debe buscar el id anterior y CampoMostrar el campo del que debe obtener el valor
        //   -# Secuencial!CampoId!Entidad!CampoContador.Funcion
        //     - Se trata de un valor secuencial. CampoId es el Id del secuencial, Entidad es la entidad donde se almacena el secuencial y CampoContador es el campo numérico con el valor del secuencial.
        //   -# Campo.Funcion
        //     - Campo es el nombre del campo del que se recupera su valor
        // - En todos los casos Funcion será una función (o lista de funciones) que se aplicarán al valor calculado con los datos anteriores.
        // </remarks>
         */
        public void initCampo(String _cadena)
        {
            int _posFinCampo = _cadena.IndexOf('.');
            
            if ( _posFinCampo == -1)
                _posFinCampo = _cadena.Length;

            String[] _partesLookup = _cadena.Substring(0, _posFinCampo).Split('!');

            if (_partesLookup.Length == 3)  // Lookup. CampoId!Entidad!CampoMostrar
            {
                nb_campo = _partesLookup[0];
                nb_entidad = _partesLookup[1];
                nb_lookupdisplay = _partesLookup[2];
            }
            else if (_partesLookup.Length == 4) // Contador que se guarda en una tabla padre. Secuencial!CampoId!Entidad!CampoContador
            {
                nb_campo = _partesLookup[0];
                nb_entidad = _partesLookup[2] + "!" + _partesLookup[1]; //
                nb_lookupdisplay = _partesLookup[3];
            }
            else
                nb_campo = _cadena.Substring(0, _posFinCampo);

            if (_posFinCampo != _cadena.Length)
            {                
                init(_cadena.Substring(_posFinCampo + 1));
            }

        }

		/**
		// <summary>
		// Calcula el siguiente valor para una secuencia. Recibe por parámetro lo siguiente:
		// </summary>
		// <param name="_nombreEntidad">Nombre de la entidad donde se guarda la secuencia (por ejemplo atos_secuencia).</param>
		// <param name="_campoBusqueda">Nombre del campo utilizado para buscar la secuencia (por ejemplo atos_name).</param>
		// <param name="_valorBusqueda">Valor para buscar la secuencia (por ejemplo Instalaciones).</param>
		// <param name="_campoContador">Campo de donde se recupera (e incrementa) el contador (por ejemplo atos_contador).</param>
		// <remarks>
		// Una vez recuperado incrementa el contador y actualiza la secuencia con el nuevo valor
		// </remarks>  
		 */
        private Entity secuencia(String _nombreEntidad, String _campoBusqueda, String _valorBusqueda, String _campoContador)
        {
            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;

            ConditionExpression _condicion;

            _condicion = new ConditionExpression();
            _condicion.AttributeName = _campoBusqueda;
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(_valorBusqueda);
            _filtro.Conditions.Add(_condicion);

            QueryExpression _consulta = new QueryExpression(_nombreEntidad);
            _consulta.ColumnSet = new ColumnSet(true);
            _consulta.Criteria.AddFilter(_filtro);
            //AC this.Trace.Trace("Antes de buscar " + _campoBusqueda + " en " + _nombreEntidad + " valor " + _valorBusqueda );
            EntityCollection _resultado = this.Service.RetrieveMultiple(_consulta);
            if (_resultado.Entities.Count > 0)
                return _resultado.Entities[0];
            return null;
        
        }

		/**
		// <summary>
		// Calcula el valor del campo.
		// </summary>
		// <remarks>
		// - Si el nombre del campo es "Secuencial" calcula la secuencia
		// - Si es un lookup recupera el registro relacionado de la entidad para calcular el valor del campo
		// </remarks>  
		 */
        public String ValorCampo(Entity _secuencia, Entity _ef, String _valorRegla)
        {
            if (nb_campo == "Secuencial")
            {
                decimal _valor = 1;
                if (_valorRegla == null)
                    throw new System.Exception("En la regla para calcular el valor de búsqueda de las secuencias no se puede utilizar Secuencial");
                //AC this.Trace.Trace("En ValorCampo, calculando Secuencial " + _valor.ToString() + 
                //AC " Buscando en: " + _secuencia.Attributes["atos_entidadcontador"].ToString() +
                //AC                  " Campobusqueda: " + _secuencia.Attributes["atos_campobusqueda"].ToString() +
                //AC                  " ValorRegla: " + _valorRegla +
                //AC                  " CampoContador: " + _secuencia.Attributes["atos_campocontador"].ToString());
                Entity _contador = secuencia(_secuencia.Attributes["atos_entidadcontador"].ToString(),
                                            _secuencia.Attributes["atos_campobusqueda"].ToString(), _valorRegla, 
                                            _secuencia.Attributes["atos_campocontador"].ToString());
                if ( _contador == null )
                {
                    //AC this.Trace.Trace("No existe el contador, hay que crearlo");
                    Entity nuevoContador = new Entity(_secuencia.Attributes["atos_entidadcontador"].ToString());
                    nuevoContador.Attributes.Add(_secuencia.Attributes["atos_campobusqueda"].ToString(), _valorRegla);
                    nuevoContador.Attributes.Add(_secuencia.Attributes["atos_campocontador"].ToString(), (decimal) 0);
                    //AC this.Trace.Trace("Antes de llamar al Create");
                    Guid cId = this.Service.Create(nuevoContador);
                    //AC this.Trace.Trace("Creado registro " + cId.ToString());
                    _contador = this.Service.Retrieve(_secuencia.Attributes["atos_entidadcontador"].ToString(), cId, new ColumnSet(true));
                }
                //AC this.Trace.Trace("Incrementamos el contador. Campo: " + _secuencia.Attributes["atos_campocontador"].ToString());
                _valor += (decimal) _contador.Attributes[_secuencia.Attributes["atos_campocontador"].ToString()];
                //AC this.Trace.Trace("Contador incrementado, valor: " + _valor.ToString());
                _contador.Attributes[_secuencia.Attributes["atos_campocontador"].ToString()] = _valor;
                this.Service.Update(_contador);
                
                return ((Int32)_valor).ToString();

            }
            else if (nb_entidad == "" || nb_lookupdisplay == "")
            {
                //AC Trace.Trace("ValorCampo campo: " + nb_campo + " entidad: " + nb_entidad + " display: " + nb_lookupdisplay);
                if (_ef.Attributes.Contains(nb_campo))
                {
                    AttributeTypeCode _type = Funcion.retrieveAttributeType(_ef.LogicalName, nb_campo, this.Service);
                    //AC this.Trace.Trace("retrieveValue field: " + nb_campo + " _type: " + _type.ToString());
                    if (_type == AttributeTypeCode.Lookup)
                        return ((EntityReference)_ef.Attributes[nb_campo]).Id.ToString();
                    else if (_type == AttributeTypeCode.Picklist)
                        return _ef.FormattedValues[nb_campo];
                    else
                        return _ef.Attributes[nb_campo].ToString();
                
                }
            }
            else
            {
                //AC Trace.Trace("ValorCampo campo: " + nb_campo + " entidad: " + nb_entidad + " display: " + nb_lookupdisplay);
                if (_ef.Attributes.Contains(nb_campo))
                {
                    String _value = Funcion.retrieveValue(((EntityReference)_ef.Attributes[nb_campo]).Id, nb_entidad, nb_lookupdisplay, this.Service, this.Trace);
                    Trace.Trace("Valor " + _value);
                    if (_value != "")
                        return _value;
                }
            }
            return "";
        }

        public String calculaValor(Entity _secuencia, Entity _ef, String _valorRegla)
        {
            String _valordelcampo = this.ValorCampo(_secuencia, _ef, _valorRegla);
            return this.calculaValor(_ef, _valordelcampo);
        }

    }

	
	/**
	// <summary>
	// Clase que sirve para calcular valores de funciones globales que no se aplican a campos (por ejemplo la fecha del día).
	// </summary>
	// <remarks>
	// Deriva de Componente
	// </remarks>
	 */
    class Globales : Componente
    {
        public Globales()
        {
            this.Tipo = 'G';
            this.Cadena = "";
            this.Funciones = new List<Funcion>();
        }

        public Globales(String _cadena, IOrganizationService _service, ITracingService _trace)
        {
            this.Tipo = 'G';
            this.Cadena = _cadena;
            this.Service = _service;
            this.Trace = _trace;
            this.Funciones = new List<Funcion>();
            init(_cadena);
        }

        public String calculaValor(Entity _ef)
        {
            return calculaValor(_ef, this.Cadena);
        }
    }

}