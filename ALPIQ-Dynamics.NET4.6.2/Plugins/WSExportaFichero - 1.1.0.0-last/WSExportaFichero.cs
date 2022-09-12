using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.Collections.ObjectModel;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Metadata;
using System.IO;
using CommonWS;

namespace WSExportaFichero
{
    
    public sealed partial class WSExportaFichero : CodeActivity
    {

        private const int OKFacturador = 300000001;
        private const int KOFacturador = 300000000;

        #region Propiedades

        [RequiredArgument]
        [Input("EntityId")] 
        public InArgument<string> Entityid { get; set; }

        [RequiredArgument]
        [Input("Name")]
        public InArgument<string> Name { get; set; }

        [RequiredArgument]
        [Input("Dictionary")]
        public InArgument<string> Dictionary { get; set; }

        [Output("State")]
        public OutArgument<int> State { get; set; }

        [Output("Result")]
        public OutArgument<int> Result { get; set; }

        [Output("Message")]
        public OutArgument<string> Message { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext executionContext)
        {
            IWorkflowContext context;
            IOrganizationServiceFactory serviceFactory;
            IOrganizationService service;
            ITracingService tracingService;

            bool _state = false;
            int _result = KOFacturador;
            int XXX = 0;

 //           String _rc = String.Empty;
            String _query = String.Empty;
 //           String _rcResult = String.Empty;
            String _message = String.Empty;
            String Response = String.Empty;
//            String _json = String.Empty;
            String _frame = String.Empty;


            context = executionContext.GetExtension<IWorkflowContext>();
            serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            service = serviceFactory.CreateOrganizationService(context.UserId);
            tracingService = (ITracingService)executionContext.GetExtension<ITracingService>();
            Logging logger = new Logging(tracingService);

            List<String> _errors = new List<String>();  

            try
            {
                String FileLog = string.Format("Log-{0}.txt", DateTime.Now.ToLocalTime().ToString("ddMM-yyyy"));
                logger.Info(FileLog);

                String _entityId = this.Entityid.Get(executionContext);
                logger.Info("Entity: " + _entityId.ToString());
                String _dictionary = this.Dictionary.Get(executionContext).ToString();
                logger.Info("Dictionary: " + _dictionary);                
                String _name = this.Name.Get(executionContext).ToString();
                logger.Info("Name: " + _name);

                Guid entityId = new Guid(_entityId);

                Exportar.ExportarXML _exportar = new Exportar.ExportarXML(tracingService, service, logger, FileLog); 

                /** 
                * Recupera Diccionario y Datos de connexion del Web Service
                **/
                bool dicc = _exportar.ReadDictionary(_dictionary);
                bool db = _exportar.ReadDBconnection();

                if (db && dicc)
                { 
                    /** 
                    * Recupera el Query del Dataverse
                    **/
                    if (entityId.ToString() == String.Empty)
                        _query = _exportar.query();
                    else
                        _query = _exportar.query(entityId, _name);

                    /** 
                    * Ejecuta el Fetch y luego alrma el XML
                    **/
                    if (!String.IsNullOrEmpty(_query))
                        _state = _exportar.RunFetch(_query, ref Response, ref _frame);
 
                    /** 
                     * reporte de estado de ejecucion
                     */
              
                    if (_state)
                    {
                        _result = OKFacturador;
                        _message = "Ejecución correcta";
                        XXX = 0;
                    } 
                    else
                    { 
                        _result = KOFacturador;
                        _message = Response;
                        XXX = 1;
                    }
                }
                else
                {
                    XXX = 0;
                    _result = KOFacturador;
                    _message = "No puede abrir la base de datos";
                }
            }
            catch (Exception ex)
            {
                _message = String.Format("Exception: {0}", ex.Message.ToString());
            }
   
            State.Set(executionContext, (int)XXX);
            Result.Set(executionContext, (int)_result);
            Message.Set(executionContext, _message);

            logger.Info(XXX.ToString());
            logger.Info(_result.ToString());
            logger.Info(_message);
            logger.Write();
        }
    }
}
