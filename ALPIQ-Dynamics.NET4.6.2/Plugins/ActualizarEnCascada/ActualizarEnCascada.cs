/**
// <summary>
// Plugin para propagar modificaciones desde cuenta negociadora, razones sociales, instalaciones a sus ofertas y contratos 
// </summary>
 */
namespace ActualizarEnCascada
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
    public class ActualizarEnCascada : IPlugin
    {
        
        private ITracingService tracingService;
        private IPluginExecutionContext PluginExecutionContext; ///< Contexto de ejecución del plugin
        private IOrganizationServiceFactory factory;
        private IOrganizationService service;
        
        private bool _log = false; ///< Indica si se activa o no el log. Esta variable debe inicializarse según el parámetro recibido en el constructor.
        private String ficherolog = "C:\\Users\\log_ActualizarEnCascada.txt";  ///< Fichero de log. Esta variable debe inicializarse según el parámetro recibido en el constructor.
        private const Char SEPARADOR = '#'; ///< Constante para el separador a usar en el parámetro que recibe el constructor

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
        public ActualizarEnCascada(String parametros)
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

        private EntityReference valorEntityReference(Entity _ef, String _nombre, String _campo)
        {
            EntityReference _ret = null;

            if (_ef.Attributes[_campo] != null)
                _ret = new EntityReference(_nombre, ((EntityReference)_ef.Attributes[_campo]).Id);
            return _ret;
        }

        private void actualizaContratoIN(Entity ins, DateTime FechaActPrevista)
        {
            Entity _contrato = new Entity("atos_contrato");
            bool _actualizar = false;
            if (ins.Attributes.Contains("atos_agentecomercialid"))
            {
                _contrato.Attributes["atos_agentecomercialid"] = valorEntityReference(ins, "atos_agentecomercial", "atos_agentecomercialid");
                _actualizar = true;
            }

            if (ins.Attributes.Contains("atos_agentefacturacionid"))
            {
                _contrato.Attributes["atos_agentefacturacionid"] = valorEntityReference(ins, "systemuser", "atos_agentefacturacionid");
                _actualizar = true;
            }


            if (ins.Attributes.Contains("atos_fechaasignacionagente"))
            {
                _contrato.Attributes["atos_fechaasignacionagente"] = ins.Attributes["atos_fechaasignacionagente"];
                _actualizar = true;
            }

            if (ins.Attributes.Contains("atos_fechaasignacionagentefacturacion"))
            {
                _contrato.Attributes["atos_fechaasignacionagentefacturacion"] = ins.Attributes["atos_fechaasignacionagentefacturacion"];
                _actualizar = true;
            }

            if (ins.Attributes.Contains("atos_baseimponibleexencion"))
            {
                _contrato.Attributes["atos_baseimponibleexencion"] = ins.Attributes["atos_baseimponibleexencion"];
                _actualizar = true;
            }
          
            if (ins.Attributes.Contains("atos_consumomaximoconexencion"))
            {
                _contrato.Attributes["atos_consumomaximoconexencion"] = ins.Attributes["atos_consumomaximoconexencion"];
                _actualizar = true;
            }
            if (ins.Attributes.Contains("atos_fechafinexencionie"))
            {
                _contrato.Attributes["atos_fechafinexencionie"] = ins.Attributes["atos_fechafinexencionie"];
                _actualizar = true;
            }
            if (ins.Attributes.Contains("atos_fechainicioexencionie"))
            {
                _contrato.Attributes["atos_fechainicioexencionie"] = ins.Attributes["atos_fechainicioexencionie"];
                _actualizar = true;
            }
            if (ins.Attributes.Contains("atos_cie"))
            {
                _contrato.Attributes["atos_cie"] = ins.Attributes["atos_cie"];
                _actualizar = true;
            }
            if (ins.Attributes.Contains("atos_tipoexencionie"))
            {
                _contrato.Attributes["atos_tipoexencionie"] = new OptionSetValue(((OptionSetValue)ins.Attributes["atos_tipoexencionie"]).Value);
                _actualizar = true;
            }
            if (ins.Attributes.Contains("atos_porcentajeexencion"))
            {
                _contrato.Attributes["atos_porcentajeexencion"] = ins.Attributes["atos_porcentajeexencion"];
                _actualizar = true;
            }

            if (ins.Attributes.Contains("atos_tipopropiedadaparatoid"))
            {
                _contrato.Attributes["atos_tipopropiedadaparatoid"] = valorEntityReference(ins, "atos_tipopropiedadaparato", "atos_tipopropiedadaparatoid");
                _actualizar = true;
            }

            if (ins.Attributes.Contains("atos_modolecturapuntomedidaid"))
            {
                _contrato.Attributes["atos_modolecturapuntomedidaid"] = valorEntityReference(ins, "atos_modolecturapuntomedida", "atos_modolecturapuntomedidaid");
                _actualizar = true;
            }

            if (ins.Attributes.Contains("atos_importealquilerequipo"))
            {
                _contrato.Attributes["atos_importealquilerequipo"] = ins.Attributes["atos_importealquilerequipo"];
                _actualizar = true;
            }

            if (ins.Attributes.Contains("atos_peajeid"))
            {
                _contrato.Attributes["atos_peajeid"] = valorEntityReference(ins, "atos_tablasatrgas", "atos_peajeid");
                _actualizar = true;
            }

            if (ins.Attributes.Contains("atos_fechainiciovigenciapeaje"))
            {
                _contrato.Attributes["atos_fechainiciovigenciapeaje"] = ins.Attributes["atos_fechainiciovigenciapeaje"];
                _actualizar = true;
            }

            if (_actualizar == false)
                return;

            QueryExpression _consulta = new QueryExpression("atos_contrato");
            _consulta.ColumnSet = new ColumnSet(new String[] { "atos_contratoid", "atos_agentecomercialid", "atos_name" });

            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = ins.LogicalName.ToString() + "id";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(ins.Id.ToString());
            _filtro.Conditions.Add(_condicion);


            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_fechafindefinitiva";
            _condicion.Operator = ConditionOperator.GreaterEqual;
            _condicion.Values.Add(FechaActPrevista);
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "statecode";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(0);
            _filtro.Conditions.Add(_condicion);

            _consulta.Criteria.AddFilter(_filtro);
            EntityCollection _resConsulta = service.RetrieveMultiple(_consulta);
            foreach (Entity _c in _resConsulta.Entities)
            {
                if (_c.Attributes.Contains("atos_name"))
                    writelog("Instalación. Actualizando Contrato: " + _c.Attributes["atos_name"].ToString());
                else
                    writelog("Instalación. Actualizando Contrato, no tiene atos_name: " + _c.Id.ToString());
                _contrato.Id = _c.Id;

                service.Update(_contrato);
                writelog("Instalación. Actualizado Contrato");
            }
        }


        private void actualizaOfertaIN(Entity ins)
        {
            Entity _oferta = new Entity("atos_oferta");

            bool _actualizar = false;
            if (ins.Attributes.Contains("atos_agentecomercialid"))
            {
                _oferta.Attributes["atos_agentecomercialid"] = valorEntityReference(ins, "atos_agentecomercial", "atos_agentecomercialid"); // new EntityReference("atos_agentecomercial", ((EntityReference)ins.Attributes["atos_agentecomercialid"]).Id);
                _actualizar = true;
            }
         
            if (_actualizar == false)
                return;

            QueryExpression _consulta = new QueryExpression("atos_oferta");
            _consulta.ColumnSet = new ColumnSet(new String[] { "atos_ofertaid", "atos_agentecomercialid", "atos_name" });

            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = ins.LogicalName.ToString() + "id";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(ins.Id.ToString());
            _filtro.Conditions.Add(_condicion);

            writelog("_condicion.AttributeName: " + _condicion.AttributeName);
            writelog("Buscando ofertas de la instalación: " + ins.Id.ToString());
            _condicion = new ConditionExpression();
            _condicion.AttributeName = "statecode";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(0);
            _filtro.Conditions.Add(_condicion);

            _consulta.Criteria.AddFilter(_filtro);

            EntityCollection _resConsulta = service.RetrieveMultiple(_consulta);
            foreach (Entity _o in _resConsulta.Entities)
            {
                if ( _o.Attributes.Contains("atos_name") )
                    writelog("Instalación. Actualizando Oferta: " + _o.Attributes["atos_name"].ToString());
                else
                    writelog("Instalación. Actualizando Oferta, no tiene atos_name: " + _o.Id.ToString());

                _oferta.Id = _o.Id;
                service.Update(_oferta);
                writelog("Instalación. Actualizado Oferta");
            }
        }

        private void actualizaContratoRZ(Entity rz, DateTime FechaActPrevista)
        {
            Entity _contrato = new Entity("atos_contrato");

            bool _actualizar = false;
            if (rz.Attributes.Contains("atos_agentecomercialid"))
            {
                _contrato.Attributes["atos_agentecomercialid"] = valorEntityReference(rz, "atos_agentecomercial", "atos_agentecomercialid"); // new EntityReference("atos_agentecomercial", ((EntityReference)ins.Attributes["atos_agentecomercialid"]).Id);
                _actualizar = true;
            }

            if (rz.Attributes.Contains("atos_agentefacturacionid"))
            {
                _contrato.Attributes["atos_agentefacturacionid"] = valorEntityReference(rz, "systemuser", "atos_agentefacturacionid"); // new EntityReference("atos_agentecomercial", ((EntityReference)ins.Attributes["atos_agentecomercialid"]).Id);
                _actualizar = true;
            }

            if (rz.Attributes.Contains("atos_fechaasignacionagente"))
            {
                _contrato.Attributes["atos_fechaasignacionagente"] = rz.Attributes["atos_fechaasignacionagente"];
                _actualizar = true;
            }

            if (rz.Attributes.Contains("atos_fechaasignacionagentefacturacion"))
            {
                _contrato.Attributes["atos_fechaasignacionagentefacturacion"] = rz.Attributes["atos_fechaasignacionagentefacturacion"];
                _actualizar = true;
            }

            if (_actualizar == false)
                return;

            QueryExpression _consulta = new QueryExpression("atos_contrato");
            _consulta.ColumnSet = new ColumnSet(new String[] { "atos_contratoid", "atos_agentecomercialid", "atos_name" });

            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_razonsocialid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(rz.Id.ToString());
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_instalacionid";
            _condicion.Operator = ConditionOperator.Null;
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_instalaciongasid";
            _condicion.Operator = ConditionOperator.Null;
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_fechafindefinitiva";
            _condicion.Operator = ConditionOperator.GreaterEqual;
            _condicion.Values.Add(FechaActPrevista);
            _filtro.Conditions.Add(_condicion);
            
            _consulta.Criteria.AddFilter(_filtro);

            EntityCollection _resConsulta = service.RetrieveMultiple(_consulta);
            foreach (Entity _c in _resConsulta.Entities)
            {
                if ( _c.Attributes.Contains("atos_name") )
                    writelog("Razón social. Actualizando Contrato: " + _c.Attributes["atos_name"].ToString());
                else
                    writelog("Razón social. Actualizando Contrato, no tiene atos_name: " + _c.Id.ToString());

                _contrato.Id = _c.Id;

                service.Update(_contrato);
                writelog("Razón social. Actualizado Contrato");
            }
        }

        private void actualizaOfertaRZ(Entity rz)
        {
            Entity _oferta = new Entity("atos_oferta");

            bool _actualizar = false;
            if (rz.Attributes.Contains("atos_agentecomercialid"))
            {
                _oferta.Attributes["atos_agentecomercialid"] = valorEntityReference(rz, "atos_agentecomercial", "atos_agentecomercialid"); // new EntityReference("atos_agentecomercial", ((EntityReference)ins.Attributes["atos_agentecomercialid"]).Id);
                _actualizar = true;
            }



            if (_actualizar == false)
                return;

            QueryExpression _consulta = new QueryExpression("atos_oferta");
            _consulta.ColumnSet = new ColumnSet(new String[] { "atos_ofertaid", "atos_agentecomercialid", "atos_name" });

            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_razonsocialid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(rz.Id.ToString());
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_instalacionid";
            _condicion.Operator = ConditionOperator.Null;
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_instalaciongasid";
            _condicion.Operator = ConditionOperator.Null;
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "statecode";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(0);
            _filtro.Conditions.Add(_condicion);

            _consulta.Criteria.AddFilter(_filtro);

            EntityCollection _resConsulta = service.RetrieveMultiple(_consulta);
            foreach (Entity _o in _resConsulta.Entities)
            {
                if ( _o.Attributes.Contains("atos_name") )
                    writelog("Razón social. Actualizando Oferta: " + _o.Attributes["atos_name"].ToString());
                else
                    writelog("Razón social. Actualizando Oferta, no tiene atos_name: " + _o.Id.ToString());

                _oferta.Id = _o.Id;
                service.Update(_oferta);
                writelog("Razón social. Actualizado Oferta");
            }
        }

        private void actualizaInstalacion(Entity rz)
        {
            Entity _instalacion = new Entity("atos_instalacion");
            bool _actualizar = false;

            if (rz.Attributes.Contains("atos_agentecomercialid"))
            {
                _instalacion.Attributes["atos_agentecomercialid"] = valorEntityReference(rz, "atos_agentecomercial", "atos_agentecomercialid"); // new EntityReference("atos_agentecomercial", ((EntityReference)rz.Attributes["atos_agentecomercialid"]).Id);
                _actualizar = true;
            }


            if (rz.Attributes.Contains("atos_agentefacturacionid"))
            {
                _instalacion.Attributes["atos_agentefacturacionid"] = valorEntityReference(rz, "systemuser", "atos_agentefacturacionid"); // new EntityReference("atos_agentecomercial", ((EntityReference)rz.Attributes["atos_agentecomercialid"]).Id);
                _actualizar = true;
            }

            //if (rz.Attributes.Contains("atos_estadocomercial"))
            //{
            //    _instalacion.Attributes["atos_estadocomercial"] = new OptionSetValue(((OptionSetValue)rz.Attributes["atos_estadocomercial"]).Value);
            //    _actualizar = true;
            //}

            if (rz.Attributes.Contains("atos_canal"))
            {
                _instalacion.Attributes["atos_canal"] = new OptionSetValue(((OptionSetValue)rz.Attributes["atos_canal"]).Value);
                _actualizar = true;
            }

            if (rz.Attributes.Contains("atos_consultoraid"))
            {
                _instalacion.Attributes["atos_consultoraid"] = valorEntityReference(rz, "atos_consultora", "atos_consultoraid"); // new EntityReference("atos_consultora", ((EntityReference)rz.Attributes["atos_consultoraid"]).Id);
                _actualizar = true;
            }

            //if (rz.Attributes.Contains("atos_comercializadoraactualid"))
            //{
            //    _instalacion.Attributes["atos_comercializadoraactualid"] = valorEntityReference(rz, "atos_comercializadora", "atos_comercializadoraactualid"); // new EntityReference("atos_comercializadora", ((EntityReference)rz.Attributes["atos_comercializadoraactualid"]).Id);
            //    _actualizar = true;
            //}

            if (rz.Attributes.Contains("atos_contactocomercialid"))
            {
                _instalacion.Attributes["atos_contactocomercialid"] = valorEntityReference(rz, "contact", "atos_contactocomercialid"); //new EntityReference("contact", ((EntityReference)rz.Attributes["atos_contactocomercialid"]).Id);
                _actualizar = true;
            }

            if (_actualizar == false)
                return;
		
            QueryExpression _consulta = new QueryExpression("atos_instalacion");
            _consulta.ColumnSet = new ColumnSet(new String[] { "atos_instalacionid", "atos_agentecomercialid", "atos_name" });

            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_razonsocialid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(rz.Id.ToString());
            _filtro.Conditions.Add(_condicion);


            _condicion = new ConditionExpression();
            _condicion.AttributeName = "statecode";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(0);
            _filtro.Conditions.Add(_condicion);

            _consulta.Criteria.AddFilter(_filtro);

            EntityCollection _resConsulta = service.RetrieveMultiple(_consulta);
            foreach (Entity _i in _resConsulta.Entities)
            {
                if (_i.Attributes.Contains("atos_name"))
                    writelog("Razón social. Actualizando Instalación: " + _i.Attributes["atos_name"].ToString());
                else
                    writelog("Razón social. Actualizando Instalación, no tiene atos_name: " + _i.Id.ToString());

                _instalacion.Id = _i.Id;

                service.Update(_instalacion);
                writelog("Razón social. Actualizada instalación");
            }
        }

        private void actualizaInstalaciongas(Entity rz)
        {
            Entity _instalaciongas = new Entity("atos_instalaciongas");
            bool _actualizar = false;

            if (rz.Attributes.Contains("atos_agentecomercialid"))
            {
                _instalaciongas.Attributes["atos_agentecomercialid"] = valorEntityReference(rz, "atos_agentecomercial", "atos_agentecomercialid"); // new EntityReference("atos_agentecomercial", ((EntityReference)rz.Attributes["atos_agentecomercialid"]).Id);
                _actualizar = true;
            }


            if (rz.Attributes.Contains("atos_agentefacturacionid"))
            {
                _instalaciongas.Attributes["atos_agentefacturacionid"] = valorEntityReference(rz, "systemuser", "atos_agentefacturacionid"); // new EntityReference("atos_agentecomercial", ((EntityReference)rz.Attributes["atos_agentecomercialid"]).Id);
                _actualizar = true;
            }

            if (rz.Attributes.Contains("atos_canal"))
            {
                _instalaciongas.Attributes["atos_canal"] = new OptionSetValue(((OptionSetValue)rz.Attributes["atos_canal"]).Value);
                _actualizar = true;
            }

            if (rz.Attributes.Contains("atos_consultoraid"))
            {
                _instalaciongas.Attributes["atos_consultoraid"] = valorEntityReference(rz, "atos_consultora", "atos_consultoraid"); // new EntityReference("atos_consultora", ((EntityReference)rz.Attributes["atos_consultoraid"]).Id);
                _actualizar = true;
            }

            if (rz.Attributes.Contains("atos_contactocomercialid"))
            {
                _instalaciongas.Attributes["atos_contactocomercialid"] = valorEntityReference(rz, "contact", "atos_contactocomercialid"); //new EntityReference("contact", ((EntityReference)rz.Attributes["atos_contactocomercialid"]).Id);
                _actualizar = true;
            }

            if (_actualizar == false)
                return;

            QueryExpression _consulta = new QueryExpression("atos_instalaciongas");
            _consulta.ColumnSet = new ColumnSet(new String[] { "atos_instalaciongasid", "atos_agentecomercialid", "atos_name" });

            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_razonsocialid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(rz.Id.ToString());
            _filtro.Conditions.Add(_condicion);


            _condicion = new ConditionExpression();
            _condicion.AttributeName = "statecode";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(0);
            _filtro.Conditions.Add(_condicion);

            _consulta.Criteria.AddFilter(_filtro);

            EntityCollection _resConsulta = service.RetrieveMultiple(_consulta);
            foreach (Entity _i in _resConsulta.Entities)
            {
                if (_i.Attributes.Contains("atos_name"))
                    writelog("Razón social. Actualizando Instalación: " + _i.Attributes["atos_name"].ToString());
                else
                    writelog("Razón social. Actualizando Instalación, no tiene atos_name: " + _i.Id.ToString());

                _instalaciongas.Id = _i.Id;

                service.Update(_instalaciongas);
                writelog("Razón social. Actualizada instalación");
            }
        }

        private void actualizaContratoCN(Entity cn,DateTime FechaActPrevista)
        {
            Entity _contrato = new Entity("atos_contrato");

            bool _actualizar = false;
            if (cn.Attributes.Contains("atos_agentecomercialid"))
            {
                _contrato.Attributes["atos_agentecomercialid"] = valorEntityReference(cn, "atos_agentecomercial", "atos_agentecomercialid"); // new EntityReference("atos_agentecomercial", ((EntityReference)ins.Attributes["atos_agentecomercialid"]).Id);
                _actualizar = true;
            }

            if (cn.Attributes.Contains("atos_fechaasignacionagente"))
            {
                _contrato.Attributes["atos_fechaasignacionagente"] = cn.Attributes["atos_fechaasignacionagente"];  
                _actualizar = true;
            }

            if (cn.Attributes.Contains("atos_agentefacturacionid"))
            {
                _contrato.Attributes["atos_agentefacturacionid"] = valorEntityReference(cn, "systemuser", "atos_agentefacturacionid"); // new EntityReference("atos_agentecomercial", ((EntityReference)ins.Attributes["atos_agentecomercialid"]).Id);
                _actualizar = true;
            }

            if (cn.Attributes.Contains("atos_fechaasignacionagentefacturacion"))
            {
                _contrato.Attributes["atos_fechaasignacionagentefacturacion"] = cn.Attributes["atos_fechaasignacionagentefacturacion"]; 
                _actualizar = true;
            }

            if (_actualizar == false)
                return;

            QueryExpression _consulta = new QueryExpression("atos_contrato");
            _consulta.ColumnSet = new ColumnSet(new String[] { "atos_contratoid", "atos_agentecomercialid", "atos_name" });

            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_cuentanegociadoraid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(cn.Id.ToString());
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_razonsocialid";
            _condicion.Operator = ConditionOperator.Null;
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_instalacionid";
            _condicion.Operator = ConditionOperator.Null;
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_instalaciongasid";
            _condicion.Operator = ConditionOperator.Null;
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_fechafindefinitiva";
            _condicion.Operator = ConditionOperator.GreaterEqual;
            _condicion.Values.Add(FechaActPrevista);
            _filtro.Conditions.Add(_condicion);

            _consulta.Criteria.AddFilter(_filtro);

            EntityCollection _resConsulta = service.RetrieveMultiple(_consulta);
            foreach (Entity _c in _resConsulta.Entities)
            {
                if ( _c.Attributes.Contains("atos_name") )
                    writelog("Cuenta negociadora. Actualizando Contrato: " + _c.Attributes["atos_name"].ToString());
                else
                    writelog("Cuenta negociadora. Actualizando Contrato, no tiene atos_name: " + _c.Id.ToString());

                _contrato.Id = _c.Id;
                service.Update(_contrato);
                writelog("Cuenta negociadora. Actualizado Contrato");
            }
        }

        private void actualizaOfertaCN(Entity cn)
        {

            Entity _oferta = new Entity("atos_oferta");

            bool _actualizar = false;
            if (cn.Attributes.Contains("atos_agentecomercialid"))
            {
                _oferta.Attributes["atos_agentecomercialid"] = valorEntityReference(cn, "atos_agentecomercial", "atos_agentecomercialid"); // new EntityReference("atos_agentecomercial", ((EntityReference)ins.Attributes["atos_agentecomercialid"]).Id);
                _actualizar = true;
            }

            if (_actualizar == false)
                return;

            QueryExpression _consulta = new QueryExpression("atos_oferta");
            _consulta.ColumnSet = new ColumnSet(new String[] { "atos_ofertaid", "atos_agentecomercialid", "atos_name" });

            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_cuentanegociadoraid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(cn.Id.ToString());
            _filtro.Conditions.Add(_condicion);
            
            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_razonsocialid";
            _condicion.Operator = ConditionOperator.Null;
            _filtro.Conditions.Add(_condicion);
            
            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_instalacionid";
            _condicion.Operator = ConditionOperator.Null;
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_instalaciongasid";
            _condicion.Operator = ConditionOperator.Null;
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "statecode";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(0);
            _filtro.Conditions.Add(_condicion);

            _consulta.Criteria.AddFilter(_filtro);

            EntityCollection _resConsulta = service.RetrieveMultiple(_consulta);
            foreach (Entity _o in _resConsulta.Entities)
            {
                if ( _o.Attributes.Contains("atos_name") )
                    writelog("Cuenta negociadora. Actualizando Oferta: " + _o.Attributes["atos_name"].ToString());
                else
                    writelog("Cuenta negociadora. Actualizando Oferta, no tiene atos_name: " + _o.Id.ToString());

                _oferta.Id = _o.Id;
                service.Update(_oferta);
                writelog("Cuenta negociadora. Actualizado Oferta");
            }
        }


        private void actualizaRazonSocial(Entity cn)
        {
            Entity _razonsocial = new Entity("account");
            bool _actualizar = false;

            if (cn.Attributes.Contains("atos_agentecomercialid"))
            {
                _razonsocial.Attributes["atos_agentecomercialid"] = valorEntityReference(cn, "atos_agentecomercial", "atos_agentecomercialid"); // new EntityReference("atos_agentecomercial", ((EntityReference)cn.Attributes["atos_agentecomercialid"]).Id);
                _actualizar = true;
            }

            if (cn.Attributes.Contains("atos_agentefacturacionid"))
            {
                _razonsocial.Attributes["atos_agentefacturacionid"] = valorEntityReference(cn, "systemuser", "atos_agentefacturacionid"); // new EntityReference("atos_agentecomercial", ((EntityReference)cn.Attributes["atos_agentecomercialid"]).Id);
                _actualizar = true;
            }

            if (cn.Attributes.Contains("atos_fechaasignacionagente"))
            {
                _razonsocial.Attributes["atos_fechaasignacionagente"] = cn.Attributes["atos_fechaasignacionagente"];
                _actualizar = true;
            }

            if (cn.Attributes.Contains("atos_fechaasignacionagentefacturacion"))
            {
                _razonsocial.Attributes["atos_fechaasignacionagentefacturacion"] = cn.Attributes["atos_fechaasignacionagentefacturacion"];
                _actualizar = true;
            }

            //if (cn.Attributes.Contains("atos_estadocomercial"))
            //{
            //    _razonsocial.Attributes["atos_estadocomercial"] = new OptionSetValue(((OptionSetValue)cn.Attributes["atos_estadocomercial"]).Value);
            //    _actualizar = true;
            //}

            if (cn.Attributes.Contains("atos_canal"))
            {
                _razonsocial.Attributes["atos_canal"] = new OptionSetValue(((OptionSetValue)cn.Attributes["atos_canal"]).Value);
                _actualizar = true;
            }

            if (cn.Attributes.Contains("atos_consultoraid"))
            {
                _razonsocial.Attributes["atos_consultoraid"] = valorEntityReference(cn, "atos_consultora", "atos_consultoraid"); // new EntityReference("atos_consultora", ((EntityReference)cn.Attributes["atos_consultoraid"]).Id);
                _actualizar = true;
            }

            //if (cn.Attributes.Contains("atos_comercializadoraactualid"))
            //{
            //    _razonsocial.Attributes["atos_comercializadoraactualid"] = valorEntityReference(cn, "atos_comercializadora", "atos_comercializadoraactualid"); //new EntityReference("atos_comercializadora", ((EntityReference)cn.Attributes["atos_comercializadoraactualid"]).Id);
            //    _actualizar = true;
            //}

            if (cn.Attributes.Contains("atos_contactocomercialid"))
            {
                _razonsocial.Attributes["atos_contactocomercialid"] = valorEntityReference(cn, "contact", "atos_contactocomercialid"); // new EntityReference("contact", ((EntityReference)cn.Attributes["atos_contactocomercialid"]).Id);
                _actualizar = true;
            }

            if (_actualizar == false)
                return;

            QueryExpression _consulta = new QueryExpression("account");
            _consulta.ColumnSet = new ColumnSet(new String[] { "accountid" , "atos_agentecomercialid" , "name" });

            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_cuentanegociadoraid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(cn.Id.ToString());
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "statecode";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(0);
            _filtro.Conditions.Add(_condicion);
            writelog("Antes de Criteria.AddFilter");
            _consulta.Criteria.AddFilter(_filtro);
            writelog("Despues de Criteria.AddFilter");

            EntityCollection _resConsulta = service.RetrieveMultiple(_consulta);
            writelog("Resultado consulta " + _resConsulta.Entities.Count);
            foreach (Entity _r in _resConsulta.Entities)
            {
                if ( _r.Attributes.Contains("name") )
                    writelog("Cuenta negociadora. Actualizando RZ: " + _r.Attributes["name"].ToString());
                else
                    writelog("Cuenta negociadora. Actualizando RZ, no tiene name: " + _r.Id.ToString());

                _razonsocial.Id = _r.Id;

                service.Update(_razonsocial);
                writelog("Cuenta negociadora. Actualizado RZ");
            }
        }



        /**
        // <summary>
        // Punto de entrada del plugin.
        // </summary>
        // <param name="serviceProvider">The service provider.</param>
        // <remarks>
        // - Propaga cambios a determinados cambios a las ofertas y contratos hijos, dependiendo de qué entidad y campos se actualicen.
        // - Si se actualiza el agente comercial de la instalación propaga ese cambio a las ofertas y contratos de la instalación
        // - Si se actualiza el agente comercial de la razón social propaga ese cambio a sus instalaciones y ofertas y contratos
        // - Si se actualiza el agente comercial de la cuenta negociadora propaga ese cambio a sus razones sociales y ofertas y contratos
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
            writelog("Plugin Actualizar en cascada");
            writelog("Mensaje: " + PluginExecutionContext.MessageName);
            if (PluginExecutionContext.MessageName == "Update")
            {
                Entity ef = (Entity)PluginExecutionContext.InputParameters["Target"];


                // Si es instalación solo propaga el cambio del agente comercial
                writelog("P1");
                if (ef.LogicalName == "atos_instalacion" &&
                    ef.Attributes.Contains("atos_agentecomercialid") == false &&
                    ef.Attributes.Contains("atos_fechaasignacionagente") == false &&
                    ef.Attributes.Contains("atos_agentefacturacionid") == false &&
                    ef.Attributes.Contains("atos_baseimponibleexencion") == false &&
                    ef.Attributes.Contains("atos_cie") == false &&
                    ef.Attributes.Contains("atos_consumomaximoconexencion") == false &&
                    ef.Attributes.Contains("atos_fechafinexencionie") == false &&
                    ef.Attributes.Contains("atos_fechainicioexencionie") == false &&
                    ef.Attributes.Contains("atos_porcentajeexencion") == false &&
                    ef.Attributes.Contains("atos_tipopropiedadaparatoid") == false &&
                    ef.Attributes.Contains("atos_modolecturapuntomedidaid") == false &&
                    ef.Attributes.Contains("atos_importealquilerequipo") == false &&
                    ef.Attributes.Contains("atos_tipoexencionie") == false)
                    return;

                writelog("P2: ");
                writelog("atos_agentecomercialid: " + !ef.Attributes.Contains("atos_agentecomercialid"));
                writelog("atos_fechaasignacionagente: " + !ef.Attributes.Contains("atos_fechaasignacionagente"));
                writelog("atos_agentefacturacionid: " + !ef.Attributes.Contains("atos_agentefacturacionid"));
                writelog("atos_peajeid: " + !ef.Attributes.Contains("atos_peajeid"));
                writelog("atos_fechainiciovigenciapeaje: " + !ef.Attributes.Contains("atos_fechainiciovigenciapeaje"));
                //Si es instalación Gas y se han modificado los sigueintes campos
                if (ef.LogicalName == "atos_instalaciongas" &&
                    !ef.Attributes.Contains("atos_agentecomercialid") &&
                    !ef.Attributes.Contains("atos_fechaasignacionagente") &&
                    !ef.Attributes.Contains("atos_agentefacturacionid") &&
                    !ef.Attributes.Contains("atos_peajeid") &&
                    !ef.Attributes.Contains("atos_fechainiciovigenciapeaje"))
                {
                    return;
                }

                writelog("P3");
                // Si es cuenta negociadora o razón social propaga el cambio del agente comercial, estado comercial, canal, consultora, comercializadora actual y contacto comercial
                if ((ef.LogicalName == "atos_cuentanegociadora" || ef.LogicalName == "account") &&
                     (ef.Attributes.Contains("atos_agentecomercialid") == false &&
                      ef.Attributes.Contains("atos_fechaasignacionagente") == false &&
                      ef.Attributes.Contains("atos_canal") == false &&
                      ef.Attributes.Contains("atos_agentefacturacionid") == false &&
                      ef.Attributes.Contains("atos_fechaasignacionagentefacturacion") == false &&
                      ef.Attributes.Contains("atos_grupoempresarialid") == false &&
                      ef.Attributes.Contains("atos_consultoraid") == false &&
                      //ef.Attributes.Contains("atos_comercializadoraactualid") == false &&
                      ef.Attributes.Contains("atos_contactocomercialid") == false))
                    return;

                writelog("P4");
                if (ef.LogicalName == "atos_cuentanegociadora")
                {
                    writelog("atos_cuentanegociadora");
                    actualizaRazonSocial(ef); // Propaga el cambio a sus razones sociales (activas)
                    if (ef.Attributes.Contains("atos_agentecomercialid") || ef.Attributes.Contains("atos_fechaasignacionagente") || ef.Attributes.Contains("atos_agentefacturacionid") || ef.Attributes.Contains("atos_fechaasignacionagentefacturacion"))
                    {
                        actualizaOfertaCN(ef); // Propaga el cambio a sus ofertas que no tienen informado el campo atos_razonsocialid y atos_instalacionid
                        actualizaContratoCN(ef,DateTime.Now); // Propaga el cambio a sus contratos que no tienen informado el campo atos_razonsocialid y atos_instalacionid
                    }
                }
                else if (ef.LogicalName == "account")
                {
                    writelog("account");
                    actualizaInstalacion(ef); // Propaga el cambio a sus instalaciones power (activas)
                    actualizaInstalaciongas(ef); //Propaga el cambio a sus instalaciones gas (activas)
                    if (ef.Attributes.Contains("atos_agentecomercialid") || ef.Attributes.Contains("atos_fechaasignacionagente") || ef.Attributes.Contains("atos_agentefacturacionid") || ef.Attributes.Contains("atos_fechaasignacionagentefacturacion"))
                    {
                        actualizaOfertaRZ(ef); // Propaga el cambio a sus ofertas que no tienen informado el atos_instalacionid
                        actualizaContratoRZ(ef, DateTime.Now); // Propaga el cambio a sus contratos que no tienen informado el campo atos_instalacionid
                    }
                }
                else if (ef.LogicalName == "atos_instalacion" || ef.LogicalName == "atos_instalaciongas")
                {
                    writelog("Tipo de instalacion: " + ef.LogicalName);
                    if (ef.Attributes.Contains("atos_agentecomercialid") || ef.Attributes.Contains("atos_fechaasignacionagente") || ef.Attributes.Contains("atos_agentefacturacionid") || ef.Attributes.Contains("atos_fechaasignacionagentefacturacion"))
                    {
                        writelog("actualizaOfertaIN");
                        actualizaOfertaIN(ef); // Propaga el cambio a sus ofertas
                    }
                    actualizaContratoIN(ef, DateTime.Now); // Propaga el cambio a sus contratos vivos (statecode a 0 y fecha fin definitiva >= fecha ejecución del plugin
                }
                //throw new Exception("Error provocado. " + ef.LogicalName);
            }
        }
    }
}