using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;


namespace WSExportaFichero
{
    public class Dictionary
    {
        #region Propiedades

        // Se sustituye la variable private ITracingService por 
        // una propiedad
        private ITracingService tracingService;

        // Se sustituye la variable private IOrganizationService por 
        // una propiedad
        private IOrganizationService service;

        Logging logger;
        private List<String> _errors;

        #endregion

        public Dictionary(ITracingService _tracingService, IOrganizationService _service, Logging _logger, ref List<String> errors)
        {
            tracingService = _tracingService;
            service = _service;
            logger = _logger;
            _errors = errors;
        }

        public Entity GetDictionary(String DictionayId)
        {
            EntityCollection resp = null; ;

            if (tracingService != null)
            {
                // off logger.Info("GetDictionary - Inicio\r\n");
            }

            EntityCollection parametros = new EntityCollection();

            QueryExpression query = new QueryExpression()
            {
                Distinct = false,
                EntityName = "wave1_dictionary",
                ColumnSet = new ColumnSet(true),
                Criteria =
                {
                    Filters =
                    {
                        new FilterExpression
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions =
                            {
                                new ConditionExpression("wave1_name", ConditionOperator.Equal, DictionayId.ToString()),
                            },
                        }
                    }
                }
            };


            if (service != null)
            {
                try
                {
                    resp = service.RetrieveMultiple(query);

                    if (resp != null && resp.Entities.Count > 0)
                    {
                        return resp.Entities.FirstOrDefault();
                    }
                }
                catch (Exception ex)
                {
                    // Error en consulta CRM

                    //tracingService.Trace("ERROR Retrieving dictionary: " + ex.Message);
                    _errors.Add(String.Format("ERROR Retrieving dictionary: {0}", ex.Message));
                
                    throw new InvalidPluginExecutionException("ERROR: No CRM dictionary found.");
                }
                if (resp == null || resp.Entities.Count == 0)
                {
                    //tracingService.Trace("ERROR: No CRM dictionary found.");
                    _errors.Add(String.Format("ERROR No Retrieve dictionary"));
                    
                    throw new InvalidPluginExecutionException("ERROR: No CRM dictionary found.");
                }
            }
            else
            {                
                _errors.Add(String.Format("ERROR No service not found."));

                throw new InvalidPluginExecutionException("ERROR: service not found.");
            }

            return null;
        }
    }
}
