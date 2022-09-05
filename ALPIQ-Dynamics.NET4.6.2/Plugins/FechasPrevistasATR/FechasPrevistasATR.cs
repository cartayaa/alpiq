using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace FechasPrevistasATR
{
    public class FechasPrevistasATR:IPlugin
    {
        private ITracingService tracingService;
        private IPluginExecutionContext PluginExecutionContext; ///< Contexto de ejecución del plugin
        private IOrganizationServiceFactory factory;
        private IOrganizationService service;

        private bool _log = false; ///< Indica si se activa o no el log. Esta variable debe inicializarse según el parámetro recibido en el constructor.
        private String _ficherolog = "D:\\Tmp\\log_FechasPrevistasATR.txt";  ///< Fichero de log. Esta variable debe inicializarse según el parámetro recibido en el constructor.
        private const Char SEPARADOR = '#'; ///< Constante para el separador a usar en el parámetro que recibe el constructor
        private const String SALTO = "<br/>"; // + Environment.NewLine;
        
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
        public FechasPrevistasATR(String parametros)
        {
            if (String.IsNullOrEmpty(parametros) == false)
            {
                String[] arrayPar = parametros.Split(SEPARADOR);
                if (arrayPar.Length > 0)
                {

                    if (arrayPar[0] == "LOG")
                        _log = true;
                    if (arrayPar.Length > 1)
                        _ficherolog = arrayPar[1];
                }
            }
        }

        public void Execute(IServiceProvider serviceProvider)
        {

            tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            PluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the Organization Service factory service from the service provider
            factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            // Use the factory to generate the Organization Service.
            service = factory.CreateOrganizationService(PluginExecutionContext.UserId);

            writelog(_ficherolog);

            writelog("-----------------------------------------");
            writelog(DateTime.Now.ToLocalTime().ToString());
            writelog("Plugin Fechas previstas ATR");
            writelog("Mensaje: " + PluginExecutionContext.MessageName);

            //"atos_pasoatrid"
            //    "atos_distribuidoraid"
            //        "atos_fechaactivacionprevista"
            //            "atos_fechaactivacionprevistatransformada"
            //                "atos_fechaprevistaaccion"
            //                    "atos_fechaprevistaaccionsintransformar"
            if (PluginExecutionContext.MessageName == "Update")
            {
                writelog("Update");

                Entity preImage = PluginExecutionContext.PreEntityImages["PreEntityImage"];
                Entity target = (Entity)PluginExecutionContext.InputParameters["Target"];
                writelog("Pre-Entity");
                writelog("Tiene paso:" + preImage.Attributes.Contains("atos_pasoatrid").ToString());
                writelog("Tient distr:" + preImage.Attributes.Contains("atos_distribuidoraid").ToString());
                writelog("Target");
                writelog("Tiene paso:" + target.Attributes.Contains("atos_pasoatrid").ToString());
                writelog("Tient distr:" + target.Attributes.Contains("atos_distribuidoraid").ToString());

                if (preImage.Attributes.Contains("atos_pasoatrid") && preImage.Attributes.Contains("atos_distribuidoraid") &&
                    (target.Attributes.Contains("atos_fechaactivacionprevista") || target.Attributes.Contains("atos_fechaprevistaaccionsintransformar"))) 
                {
                    Guid paso = ((EntityReference)preImage.Attributes["atos_pasoatrid"]).Id;
                    Guid distribuidora = ((EntityReference)preImage.Attributes["atos_distribuidoraid"]).Id;
                    
                    if (target.Attributes.Contains("atos_pasoatrid"))
                    {
                        paso = ((EntityReference)target.Attributes["atos_pasoatrid"]).Id;
                    } 

                    if (target.Attributes.Contains("atos_distribuidoraid"))
                    {
                        distribuidora = ((EntityReference)target.Attributes["atos_distribuidoraid"]).Id;
                    }

                    int diferencia = obtenerDiferenciaHoraria(paso, distribuidora);

                    writelog("diferencia" + diferencia.ToString());

                    DateTime fechaActivacion = new DateTime();
                    DateTime fechaAccionSinTr = new DateTime();

                    if (target.Attributes.Contains("atos_fechaprevistaaccionsintransformar"))
                    {
                        fechaAccionSinTr = (DateTime)target.Attributes["atos_fechaprevistaaccionsintransformar"];
                        writelog("Fecha Acción del Target");
                    }
                    else if (preImage.Attributes.Contains("atos_fechaprevistaaccionsintransformar"))
                    {
                        fechaAccionSinTr = (DateTime)preImage.Attributes["atos_fechaprevistaaccionsintransformar"];
                        writelog("Fecha Acción de la preImagen");
                    }
                    
                    if (target.Attributes.Contains("atos_fechaactivacionprevista"))
                    {
                        fechaActivacion = (DateTime)target.Attributes["atos_fechaactivacionprevista"];
                        writelog("Fecha Activación del Target");
                    }
                    else if (preImage.Attributes.Contains("atos_fechaactivacionprevista"))
                    {
                        fechaActivacion = (DateTime)preImage.Attributes["atos_fechaactivacionprevista"];
                        writelog("Fecha Activación de la PreImage");
                    }

                    //fecha acción
                    if (fechaAccionSinTr != DateTime.MinValue)
                    {
                        writelog("Fecha Acción no nula:" + fechaAccionSinTr.ToString());
                        if (target.Attributes.Contains("atos_fechaprevistaaccion"))
                        {
                            writelog("No debe actualizarse la fecha porque el usuario la ha modificado a mano");
                            //target.Attributes["atos_fechaprevistaaccion"] = fechaAccionSinTr.AddHours(diferencia);
                        }
                        else
                        {
                            target.Attributes.Add("atos_fechaprevistaaccion", fechaAccionSinTr.AddHours(diferencia));
                        }
                    }

                    //fecha activación
                    if (fechaActivacion != DateTime.MinValue)
                    {
                        writelog ("fecha Activacion no nula:" + fechaActivacion.ToString());
                        if (target.Attributes.Contains("atos_fechaactivacionprevistatransformada"))
                        {
                            writelog("No debe actualizarse la fecha porque el usuario la ha modificado a mano");
                            //target.Attributes["atos_fechaactivacionprevistatransformada"] = fechaActivacion.AddHours(diferencia);
                        }
                        else
                        {
                            target.Attributes.Add("atos_fechaactivacionprevistatransformada", fechaActivacion.AddHours(diferencia));
                        }
                    }

                }
            }
            else if (PluginExecutionContext.MessageName == "Create")
            {
                writelog("create");

                Entity target = (Entity)PluginExecutionContext.InputParameters["Target"];
                
                writelog("Tiene paso:" + target.Attributes.Contains("atos_pasoatrid").ToString());
                writelog("Tient distr:" + target.Attributes.Contains("atos_distribuidoraid").ToString());

                if (target.Attributes.Contains("atos_pasoatrid") && target.Attributes.Contains("atos_distribuidoraid") &&
                    (target.Attributes.Contains("atos_fechaactivacionprevista") || target.Attributes.Contains("atos_fechaprevistaaccionsintransformar")))
                {
                    Guid paso = ((EntityReference)target.Attributes["atos_pasoatrid"]).Id;
                    Guid distribuidora = ((EntityReference)target.Attributes["atos_distribuidoraid"]).Id;

                    int diferencia = obtenerDiferenciaHoraria(paso, distribuidora);
                    writelog("Diferencia:" + diferencia.ToString());
                    DateTime fechaActivacion = new DateTime();
                    DateTime fechaAccionSinTr = new DateTime();

                    if (target.Attributes.Contains("atos_fechaprevistaaccionsintransformar"))
                    {
                        fechaAccionSinTr = (DateTime)target.Attributes["atos_fechaprevistaaccionsintransformar"];
                        writelog("Fecha Acción del Target");
                    }

                    if (target.Attributes.Contains("atos_fechaactivacionprevista"))
                    {
                        fechaActivacion = (DateTime)target.Attributes["atos_fechaactivacionprevista"];
                        writelog("Fecha Activación del Target");
                    }

                    //fecha acción
                    if (fechaAccionSinTr != DateTime.MinValue)
                    {
                        writelog("Fecha Acción no nula:" + fechaAccionSinTr.ToString());
                        if (target.Attributes.Contains("atos_fechaprevistaaccion"))
                        {
                            writelog("No debe actualizarse la fecha porque el usuario la ha modificado a mano");
                            //target.Attributes["atos_fechaprevistaaccion"] = fechaAccionSinTr.AddHours(diferencia);
                        }
                        else
                        {
                            target.Attributes.Add("atos_fechaprevistaaccion", fechaAccionSinTr.AddHours(diferencia));
                        }
                    }

                    //fecha activación
                    if (fechaActivacion != DateTime.MinValue)
                    {
                        writelog("fecha Activacion no nula:" + fechaActivacion.ToString());
                        if (target.Attributes.Contains("atos_fechaactivacionprevistatransformada"))
                        {
                            writelog("No debe actualizarse la fecha porque el usuario la ha modificado a mano");
                            //target.Attributes["atos_fechaactivacionprevistatransformada"] = fechaActivacion.AddHours(diferencia);
                        }
                        else
                        {
                            target.Attributes.Add("atos_fechaactivacionprevistatransformada", fechaActivacion.AddHours(diferencia));
                        }
                    }
                }
            }

            //throw new InvalidPluginExecutionException ("TEST");

        }

        private int obtenerDiferenciaHoraria(Guid pIdPaso, Guid pIdDitribuidora)
        {
            int salida = 0;

            QueryExpression _consulta = new QueryExpression("atos_particularidadesdistribuidoras");
            _consulta.ColumnSet = new ColumnSet(new String[] { "atos_fechaprevistaccion"});

            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_pasoid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(pIdPaso);
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_distribuidoraid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(pIdDitribuidora);
            _filtro.Conditions.Add(_condicion);

            _consulta.Criteria.AddFilter(_filtro);
            //writelog("Despues de Criteria.AddFilter");

            EntityCollection _resConsulta = service.RetrieveMultiple(_consulta);
            if (_resConsulta.Entities.Count > 0)
            {
                writelog("Consulta de particularidades count:" + _resConsulta.Entities.Count);
                if (_resConsulta.Entities[0].Attributes.Contains("atos_fechaprevistaccion"))
                {
                    salida = Convert.ToInt32 ((decimal) _resConsulta.Entities[0].Attributes["atos_fechaprevistaccion"]);
                }

            }
           
            return salida;
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
            tracingService.Trace(texto + "\r\n");
            if (_log)
            {
                System.IO.File.AppendAllText(_ficherolog, texto + "\r\n");
            }
        }
    }
}
