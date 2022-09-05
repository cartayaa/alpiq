

namespace ComprobarEstadoContrato
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

 
    public class ComprobarEstadoContrato : IPlugin
    {

        private ITracingService tracingService;
        private IPluginExecutionContext PluginExecutionContext;
        private IOrganizationServiceFactory factory;
        private IOrganizationService service;
        private bool _log = false;
        private String ficherolog = "C:\\Users\\ComprobarEstadoContrato.txt";
        private const Char SEPARADOR = '#';

        private int PRECONTRATO = 300000000;
        private int PENDIENTEDEDATOS = 300000001;
        private int FORMALIZADO = 300000005;
        private int FINALIZADO = 300000011;

        public ComprobarEstadoContrato(String parametros)
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

        public void Execute(IServiceProvider serviceProvider)
        {
            bool errorEstado = false;
            tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            PluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the Organization Service factory service from the service provider
            factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            // Use the factory to generate the Organization Service.
            service = factory.CreateOrganizationService(PluginExecutionContext.UserId);

            writelog(ficherolog, "===============================");
            writelog(ficherolog, "Plugin ComprobarEstadoContrato " + DateTime.Now.ToLocalTime().ToString());


            Entity preImage = PluginExecutionContext.PreEntityImages["PreEntityImage"];
            Entity target = (Entity)PluginExecutionContext.InputParameters["Target"];

            tracingService.Trace("Plugin ComprobarEstadoContrato");
            tracingService.Trace("Entity Target " + target.LogicalName);

            if (target.LogicalName != "atos_contrato")
            {
                writelog(ficherolog, "Error en el tipo de entidad");
                tracingService.Trace("La entidad de comprobación no es del tipo contrato: " + target.LogicalName);
                throw new InvalidPluginExecutionException("La entidad de comprobación no es del tipo contrato.");
                return;
            }


            if (target.Attributes.Contains("atos_estadocontrato") && preImage.Attributes.Contains("atos_estadocontrato"))
            {
                // comprobamos que va del los estados Pendiente de datos, Formalizado  o Finalizado a estado precontrato
                // precontrato -->300000000
                // Pendiente de datos -->300000001
                // Formalizado -->300000005
                // Finalizado -->300000011


                int estadoInicial = ((OptionSetValue)preImage.Attributes["atos_estadocontrato"]).Value;
                int estadoFinal = ((OptionSetValue)target.Attributes["atos_estadocontrato"]).Value;
                writelog(ficherolog, "estado inicial: " + estadoInicial.ToString() );
                writelog(ficherolog, "estado final: " + estadoFinal.ToString());
                
                if (estadoFinal == PRECONTRATO && (estadoInicial == PENDIENTEDEDATOS || estadoInicial == FORMALIZADO || estadoInicial == FINALIZADO))
                {
                    errorEstado = true;
                }
            }

            writelog(ficherolog, "error estado: " + errorEstado.ToString());
            writelog(ficherolog, "-------------------------------");
            if (errorEstado)
                throw new InvalidPluginExecutionException("No se puede cambiar el estado de contrato de  Pendiente de datos, Formalizado  o Finalizado a  el estado Pre-contrato.");


        }

        private void writelog(String fichero, String texto)
        {
            tracingService.Trace(texto);
            if (_log == true)
                System.IO.File.AppendAllText(fichero, texto + "\r\n");
        }


    }
}
