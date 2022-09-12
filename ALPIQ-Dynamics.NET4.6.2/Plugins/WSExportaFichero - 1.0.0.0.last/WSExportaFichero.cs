using System;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace WSExportaFichero
{

    public sealed partial class WSExportaFichero : CodeActivity
    {
        //private const String FileLog = "WSExportaFicheros.txt";
        private const String FileXMLLog = null;

        #region Propiedades

        [RequiredArgument]
        [Input("Directorio principal de los ficheros de registros")]
        public InArgument<string> Registro { get; set; }

        [RequiredArgument]
        [Input("Directorio de los diccionarios de los registros")]
        public InArgument<string> Diccionarios { get; set; }

        [RequiredArgument]
        [Input("Consulta a las entidades de CRM")]
        public InArgument<string> Consulta { get; set; }
        
        [RequiredArgument]
        [Input("Direccion del WS que escribe los log")]
        public InArgument<string> UrlWSLog { get; set; }
        
        [Output("Ejecucion correcta")]
        public OutArgument<Boolean> EjecucionCorrecta { get; set; } 

        [Output("Mensaje salida")]
        public OutArgument<string> MensajeSalida { get; set; }

        #endregion


        /*/
         * Function Execute
         * 
         * Param executionContext
         */
        protected override void Execute(CodeActivityContext executionContext)
        {
            IWorkflowContext context;
            IOrganizationServiceFactory serviceFactory;
            IOrganizationService service;
            ITracingService tracingService;

            context = executionContext.GetExtension<IWorkflowContext>();
            serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            service = serviceFactory.CreateOrganizationService(context.UserId);
            tracingService = (ITracingService)executionContext.GetExtension<ITracingService>();

            Logging logger = new Logging(tracingService);

            try
            {
                String _pathDiccionario = this.Diccionarios.Get(executionContext).ToString();    
                String _fQuery          = this.Consulta.Get(executionContext).ToString();
                String _pathRegistro    = this.Registro.Get(executionContext).ToString();
                String urlWSLog         = this.UrlWSLog.Get(executionContext).ToString();

                //--String FileLog = string.Format("WSExportaFicheros-{0}" + ".txt", DateTime.Now.ToLocalTime().ToString("ddMM-yyyy-HH"));
                String FileLog = string.Format("Log-{0}.txt", DateTime.Now.ToLocalTime().ToString("ddMM-yyyy"));

                logger.Info("Path 0(in): " + _pathDiccionario);
                logger.Info("Path 0(out): " + _pathRegistro);
                logger.Info("WS Log 0(out): " + this.UrlWSLog.Get(executionContext).ToString());
                logger.Info("LogFile 0(out): " + FileLog );
                logger.Info("-----------------------------------------");

                //   public void Parameters(String _uriWSLog, String _carpetaLog, String _ficheroLog)
                // logger.Parameters(urlWSLog, "", urlWSLog); //  FileLog);
                logger.Parameters(urlWSLog, "", FileLog); //  FileLog);

                // Instancia ExportarXML
                Exportar.ExportarXML _exportar = new Exportar.ExportarXML(tracingService, service, logger, FileLog);

                // Arma la ruta para la lectura del <conf.xml> y <server>/consultas/<entity>dicc_XXXXXXXXXX
                _exportar.SetPath(_pathRegistro, _pathDiccionario, _fQuery, urlWSLog);
             
                bool _resultadoExportar;

                // Al Disco WSExportaFicheros.txt
                //CommonWS.CommonWS.WriteLog(urlWSLog, FileLog, "", DateTime.Now.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss.ff") + " - " + urlWSLog + " - " + _exportar.RutaLog + " - " + _exportar.PrefijoLog + "WS.txt");
                //CommonWS.CommonWS.WriteLog(urlWSLog, FileLog, String.Empty, urlWSLog + " - " + _exportar.RutaLog + " - " + _exportar.PrefijoLog + "WS.txt");

                //-- logger.Trace("WSExportaFicheros: "  + urlWSLog + " - " + _exportar.RutaLog + " - " + _exportar.PrefijoLog + "WS.txt");
                //logger.Trace("WSExportaFicheros: " + FileLog);
                logger.Info("FileLog: " + FileLog);


                if (context.PrimaryEntityId == Guid.Empty)
                { 
                    // Entidad no reconocida
                    _resultadoExportar = _exportar.exporta(_exportar.query());
                }
                else
                {
                    // Recupera datos desde el CRM (usa exportarDatosEms para su funcion)
                    _resultadoExportar = _exportar.exporta(_exportar.query(context.PrimaryEntityId));
                }

                /** 
                 * reporte de estado de ejecucion
                 **/
                if (!_resultadoExportar)
                {
/*T*/               _exportar.volcarError(_exportar.PrefijoLog + "WS.txt" /*"AWG_Gdf_ExportacionesWS.log"*/, this.DisplayName);
                    if (!String.IsNullOrEmpty(_exportar.MensajeError))
                    {
                        // Envia mensaje de Salida Error al WF
                        this.MensajeSalida.Set(executionContext, _exportar.MensajeError.Length <= 2000 ? _exportar.MensajeError : _exportar.MensajeError.Substring(0, 2000));
                    }
                   
                    // Envia resultado Error al WF
                    this.EjecucionCorrecta.Set(executionContext, false);
                }
                else
                {
                    //CommonWS.CommonWS.WriteLog(urlWSLog, "WSExportaFicheros.txt", "", DateTime.Now.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss.ff") + "Execution Ok" + _exportar.MensajeError);
                    //CommonWS.CommonWS.WriteLog(urlWSLog, "WSExportaFicheros.txt", String.Empty, "End: Execution Ok" + _exportar.MensajeError);
                    logger.Info("Execution End" + _exportar.MensajeError);

                    if (!String.IsNullOrEmpty(_exportar.MensajeError)) {
                        // Envia mensaje de Salida OK al WF
                        this.MensajeSalida.Set(executionContext, _exportar.MensajeError);
                    }

                    // Envia resultado OK al WF
                    this.EjecucionCorrecta.Set(executionContext, true);
                }

                
                logger.Write(); 

            }
            catch (Exception ex)
            {
                logger.Error("Execution Error" + ex.Message);
                logger.Write();
                throw new Exception("Execution Error" + ex.Message);
            }
        }

    }
}
