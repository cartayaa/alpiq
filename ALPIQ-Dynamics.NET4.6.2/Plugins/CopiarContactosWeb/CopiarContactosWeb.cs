using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CopiarContactosWeb
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    public class CopiarContactosWeb:IPlugin
    {
        
        private ITracingService tracingService;
        private IPluginExecutionContext PluginExecutionContext; ///< Contexto de ejecución del plugin
        private IOrganizationServiceFactory factory;
        private IOrganizationService service;
        
        private bool _log = false; ///< Indica si se activa o no el log. Esta variable debe inicializarse según el parámetro recibido en el constructor.
        private String ficherolog = "C:\\Users\\log_CopiaContactosWeb.log";  ///< Fichero de log. Esta variable debe inicializarse según el parámetro recibido en el constructor.
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
        public CopiarContactosWeb(String parametros)
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

        private void copiaContactosWeb(Guid contratoId, Guid pInstaladoraId, Guid pConsultoraId)
        {
            writelog(String.Format("[CopiaContactosWeb] {0},{1},{2} ", contratoId.ToString(), pInstaladoraId.ToString(), pConsultoraId.ToString()));
            QueryExpression _consulta = new QueryExpression("atos_contactoweb");
            _consulta.ColumnSet = new ColumnSet(new String[] { "atos_contactoid", "atos_contactoconsultora" ,"atos_esadministrador"});

            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            if (pInstaladoraId != Guid.Empty)
            {
                _condicion.AttributeName = "atos_instalacionid";
                _condicion.Operator = ConditionOperator.Equal;
                _condicion.Values.Add(pInstaladoraId.ToString());
            }
            else if (pConsultoraId != Guid.Empty)
            {
                _condicion.AttributeName = "atos_consultoraid";
                _condicion.Operator = ConditionOperator.Equal;
                _condicion.Values.Add(pConsultoraId.ToString());
            }
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "statecode";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(0);
            _filtro.Conditions.Add(_condicion);
            _consulta.Criteria.AddFilter(_filtro);

            writelog(String.Format("[CopiaContactosWeb] {0}", "--consulta contactos web"));
            int i = 0;
            EntityCollection _resContactosWeb = service.RetrieveMultiple(_consulta);
            writelog(String.Format("[CopiaContactosWeb] {0}  contactos", _resContactosWeb.Entities.Count.ToString()));
            foreach (Entity contactoWeb in _resContactosWeb.Entities)
            {
                i++;
                writelog(String.Format("[CopiaContactosWeb] contacto {0}", i.ToString()));
                Entity nuevoContacto = contactoWeb;
                nuevoContacto.Attributes.Remove("atos_contactowebid");
                nuevoContacto.Attributes["atos_contratoid"] = new EntityReference("atos_contrato", contratoId);
                nuevoContacto.Attributes["atos_contactoconsultora"] = pConsultoraId!=Guid.Empty; 
                writelog("Antes de NewGuid");
                nuevoContacto.Id = Guid.NewGuid();

                service.Create(nuevoContacto);
                writelog("copiaContactos Despues Create");
            }
            writelog("[CopiaContactosWeb] fin copiar contactos");
        }

     
        /// <summary>
        /// Executes the plug-in.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <remarks>
        /// For improved performance, Microsoft Dynamics CRM caches plug-in instances. 
        /// The plug-in's Execute method should be written to be stateless as the constructor 
        /// is not called for every invocation of the plug-in. Also, multiple system threads 
        /// could execute the plug-in at the same time. All per invocation state information 
        /// is stored in the context. This means that you should not use global variables in plug-ins.
        /// </remarks>
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
            writelog("Plugin Copia Contactos Web");
            writelog("Mensaje: " + PluginExecutionContext.MessageName);
            if (PluginExecutionContext.MessageName == "Create")
            {
                Entity ef = (Entity)PluginExecutionContext.InputParameters["Target"];

                writelog("copiar contacto instalaciones");
                if (ef.Attributes.Contains("atos_instalacionid") && ef.Attributes["atos_instalacionid"]!=null)
                    copiaContactosWeb(ef.Id,((EntityReference)ef.Attributes["atos_instalacionid"]).Id, Guid.Empty);
                writelog("copiar contacto consultora");
                writelog("copiar contacto consultora: " + ef.Attributes.Contains("atos_consultoraid").ToString());

                if (ef.Attributes.Contains("atos_consultoraid") && ef.Attributes["atos_consultoraid"]!=null)
                {
                    writelog("copiar contacto consultora1: Entra");
                    
                    copiaContactosWeb(ef.Id, Guid.Empty, ((EntityReference)ef.Attributes["atos_consultoraid"]).Id);
                }
                

                writelog(" Fin del plugin copia contactos Web MP");
                writelog("============================");
                //throw new Exception("Error provocado");
            }
        }
    }
}
