using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace ActTiemposCaso
{
    public class ActTiemposCaso : IPlugin
    {
        private ITracingService tracingService;
        private IPluginExecutionContext PluginExecutionContext; ///< Contexto de ejecución del plugin
        private IOrganizationServiceFactory factory;
        private IOrganizationService _service;
        private Configuracion _conf;

                
        // <summary>
		// Constructor de la clase
		// </summary>
        // <param name="parametros">Cadena en la que se indica si se escribe log y donde: LOG#C:\\RutaDelLog\\Fichero.log</param>
		// <remarks>
		// Recibe una cadena de texto incluyendo los parámetros (separados por el carácter #)
		// - El primer parámetro activa/desactiva la escritura del fichero log (LOG activa)
		// - El segundo parámetro es el nombre (incluyendo ruta) del fichero de log.
		// </remarks>
        public ActTiemposCaso(String parametros)
        {
            _conf = new Configuracion(parametros);
        }
        
        ///
        // <summary>
        // Punto de entrada al plugin
        // </summary>
        // <param name="serviceProvider">IServiceProvider.</param>
        // <remarks>
        // El plugin se ejecuta en:
        // - Creacion de Caso
        // - Modificación de estado de caso
        // </remarks>
        public void Execute(IServiceProvider serviceProvider)
        {

            tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            PluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the Organization Service factory service from the service provider
            factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            // Use the factory to generate the Organization Service.
            _service = factory.CreateOrganizationService(PluginExecutionContext.UserId);

            _conf.writelog("-----------------------------------------");
            _conf.writelog(DateTime.Now.ToLocalTime().ToString());
            _conf.writelog("Plugin Actualizar tiempos de casos");
            _conf.writelog("Mensaje: " + PluginExecutionContext.MessageName);

            Entity caso = (Entity)PluginExecutionContext.InputParameters["Target"];

           if (PluginExecutionContext.MessageName == "Update")
            {

                try
                {
                    Entity preUpdateCaso = (Entity)PluginExecutionContext.PreEntityImages["PreUpdateImage"];

                    ActCaso actCaso = new ActCaso(preUpdateCaso, PluginExecutionContext.MessageName);
                    caso = actCaso.ActualizarCaso(ref caso, _service,_conf);
                }
                catch (Exception ex)
                {
                    _conf.writelog("Error: " + ex.Message );
                    _conf.writelog("Error interno: " + ex.InnerException );
                    _conf.writelog("Error: " + ex.StackTrace );
                }
             
             
            }
        }


    }
}
