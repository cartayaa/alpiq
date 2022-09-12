using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;


namespace WSExportaFichero
{
    public class GlobalParameter
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

        public GlobalParameter(ITracingService _tracingService, IOrganizationService _service, Logging _logger, ref List<String> errors)
        {
            tracingService = _tracingService;
            service = _service;
            logger = _logger;
            _errors = errors;
        }

        public Entity GetGlobalConnection()
        {
            EntityCollection resp = null; ;

            if (tracingService != null)
            {
                // off logger.Info("GetGlobalConnection - Inicio\r\n");
            }

            EntityCollection parametros = new EntityCollection();

            QueryExpression query = new QueryExpression()
            {
                Distinct = false,
                EntityName = "wave1_globalconnection",
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
                                new ConditionExpression("statecode", ConditionOperator.Equal, 0),
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
                    _errors.Add(String.Format("ERROR Retrieving global params: {0}", ex.Message));

                    throw new InvalidPluginExecutionException("ERROR: No CRM grobal parameters found.");
                }
                if (resp == null || resp.Entities.Count == 0)
                {
                    // No se encontraron los parámetros
                    _errors.Add(String.Format("ERROR No retrieve Global parameters."));

                    throw new InvalidPluginExecutionException("No retrieve Global parameters.");
                }
            }
            else
            {
                _errors.Add(String.Format("ERROR: service not found."));

                throw new InvalidPluginExecutionException("ERROR: service not found.");
            }

            return null;
        }
    }
}


