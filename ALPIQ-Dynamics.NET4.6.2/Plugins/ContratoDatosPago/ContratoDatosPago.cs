using System;
using System.Globalization;
using Microsoft.Xrm.Sdk;
using System.Web.Services.Protocols;
using Microsoft.Xrm.Sdk.Query;

namespace ContratoDatosPago
{
    public class ContratoDatosPago:IPlugin
    {
        private ITracingService tracingService;
        private IPluginExecutionContext PluginExecutionContext; ///< Contexto de ejecución del plugin
        private IOrganizationServiceFactory factory;
        private IOrganizationService service;

        public void Execute(IServiceProvider serviceProvider)
        {
            String errMsg = String.Empty;

            tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            PluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the Organization Service factory service from the service provider
            factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            // Use the factory to generate the Organization Service.
            service = factory.CreateOrganizationService(PluginExecutionContext.UserId);

            tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Entered {0}.Execute()", this.GetType().ToString()));

            if (PluginExecutionContext.MessageName != "Create")
                return;

            if (PluginExecutionContext.InputParameters.Contains("Target") &&
                PluginExecutionContext.InputParameters["Target"] is Entity)
            {
                try
                {
                    Entity contratoModificado = (Entity)PluginExecutionContext.InputParameters["Target"];

                    

                    //Comprobar si hay información que propagar.
                    if (!contratoModificado.Contains("atos_cups"))
                        return; //con esto controlamos tb que no sea multipunto.

                    tracingService.Trace(String.Format("Cups del contrato:{0}", contratoModificado.Attributes["atos_cups"].ToString()));

                    Entity contratoOrigenDP = obtenerContratoDatosPago(contratoModificado.Attributes["atos_cups"].ToString());
                    Guid idCondicionPagoInd = obtenerCondicionPagoIndeterminado();

                    tracingService.Trace(String.Format("Guid Indeterminado:{0}", idCondicionPagoInd.ToString()));

                    if (contratoOrigenDP.Id != Guid.Empty)
                    {
                        if (!contratoModificado.Attributes.Contains("atos_condicionpagoid") && contratoOrigenDP.Attributes.Contains("atos_condicionpagoid"))
                        {
                            contratoModificado.Attributes["atos_condicionpagoid"]=contratoOrigenDP.Attributes["atos_condicionpagoid"];
                            tracingService.Trace("Actualizado atos_condicionpagoid");
                        }
                        else if (contratoOrigenDP.Attributes.Contains("atos_condicionpagoid") && contratoModificado.Attributes.Contains("atos_condicionpagoid")
                            && (((EntityReference)contratoModificado.Attributes["atos_condicionpagoid"]).Id == idCondicionPagoInd))
                        {
                             contratoModificado.Attributes["atos_condicionpagoid"]= contratoOrigenDP.Attributes["atos_condicionpagoid"];
                             tracingService.Trace("Actualizado atos_condicionpagoid. El usuario informó indeterminado");
                        }

                        if (!contratoModificado.Attributes.Contains("atos_cuenta") && contratoOrigenDP.Attributes.Contains("atos_cuenta"))
                        {
                            contratoModificado.Attributes["atos_cuenta"] = contratoOrigenDP.Attributes["atos_cuenta"];
                            tracingService.Trace("Actualizado atos_cuenta");
                        }

                        if (!contratoModificado.Attributes.Contains("atos_cuentabancaria") && contratoOrigenDP.Attributes.Contains("atos_cuentabancaria"))
                        {
                            contratoModificado.Attributes["atos_cuentabancaria"] = contratoOrigenDP.Attributes["atos_cuentabancaria"];
                            tracingService.Trace("Actualizado atos_cuentabancaria");
                        }

                        if (!contratoModificado.Attributes.Contains("atos_digitocontrol") && contratoOrigenDP.Attributes.Contains("atos_digitocontrol"))
                        {
                            contratoModificado.Attributes["atos_digitocontrol"] = contratoOrigenDP.Attributes["atos_digitocontrol"];
                            tracingService.Trace("Actualizado atos_digitocontrol");
                        }

                        if (!contratoModificado.Attributes.Contains("atos_entidadbancaria") && contratoOrigenDP.Attributes.Contains("atos_entidadbancaria"))
                        {
                            contratoModificado.Attributes["atos_entidadbancaria"] = contratoOrigenDP.Attributes["atos_entidadbancaria"];
                            tracingService.Trace("Actualizado atos_entidadbancaria");
                        }

                        if (!contratoModificado.Attributes.Contains("atos_formadepago") && contratoOrigenDP.Attributes.Contains("atos_formadepago"))
                        {
                            contratoModificado.Attributes["atos_formadepago"] = contratoOrigenDP.Attributes["atos_formadepago"];
                            tracingService.Trace("Actualizado atos_formadepago");
                        }

                        if (!contratoModificado.Attributes.Contains("atos_iban") && contratoOrigenDP.Attributes.Contains("atos_iban"))
                        {
                            contratoModificado.Attributes["atos_iban"] = contratoOrigenDP.Attributes["atos_iban"];
                            tracingService.Trace("Actualizado atos_iban");
                        }

                        if (!contratoModificado.Attributes.Contains("atos_mandatosepa") && contratoOrigenDP.Attributes.Contains("atos_mandatosepa"))
                        {
                            contratoModificado.Attributes["atos_mandatosepa"] = contratoOrigenDP.Attributes["atos_mandatosepa"];
                            tracingService.Trace("Actualizado atos_mandatosepa");
                        }

                        if (!contratoModificado.Attributes.Contains("atos_plazoenviofacturas") && contratoOrigenDP.Contains("atos_plazoenviofacturas"))
                        {
                            contratoModificado.Attributes["atos_plazoenviofacturas"] = contratoOrigenDP.Attributes["atos_plazoenviofacturas"];
                            tracingService.Trace("Actualizado atos_plazoenviofacturas");
                        }

                        if (!contratoModificado.Attributes.Contains("atos_precioalquilerequipo") && contratoOrigenDP.Attributes.Contains("atos_precioalquilerequipo"))
                        {
                            contratoModificado.Attributes["atos_precioalquilerequipo"] = contratoOrigenDP.Attributes["atos_precioalquilerequipo"];
                            tracingService.Trace("Actualizado atos_precioalquilerequipo");
                        }

                        if (!contratoModificado.Attributes.Contains("atos_preciomensualserviciolectura") && contratoOrigenDP.Attributes.Contains("atos_preciomensualserviciolectura"))
                        {
                            contratoModificado.Attributes["atos_preciomensualserviciolectura"] = contratoOrigenDP.Attributes["atos_preciomensualserviciolectura"];
                            tracingService.Trace("Actualizado atos_preciomensualserviciolectura");
                        }

                        if (!contratoModificado.Attributes.Contains("atos_tipodeenvio") && contratoOrigenDP.Attributes.Contains("atos_tipodeenvio"))
                        {
                            contratoModificado.Attributes["atos_tipodeenvio"] = contratoOrigenDP.Attributes["atos_tipodeenvio"];
                            tracingService.Trace("Actualizado atos_tipodeenvio");
                        }

                        if (!contratoModificado.Attributes.Contains("atos_sucursalbancaria") && contratoOrigenDP.Attributes.Contains("atos_sucursalbancaria"))
                        {
                            contratoModificado.Attributes["atos_sucursalbancaria"] = contratoOrigenDP.Attributes["atos_sucursalbancaria"];
                            tracingService.Trace("Actualizado atos_sucursalbancaria");
                        }

                        if (!contratoModificado.Attributes.Contains("atos_swift") && contratoOrigenDP.Attributes.Contains("atos_swift"))
                        {
                            contratoModificado.Attributes["atos_swift"] = contratoOrigenDP.Attributes["atos_swift"];
                            tracingService.Trace("Actualizado atos_swift");
                        }

                    }
                }
                catch (SoapException soex)
                {
                    errMsg = soex.Detail.InnerText;
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }

                tracingService.Trace(String.Format("Errores reportados:{0}", errMsg));

                if (!String.IsNullOrEmpty(errMsg))
                {
                    //throw new InvalidPluginExecutionException("Se ha producido un error en la propagación de información de ofertas MP a ofertas hijas y subofertas");
                }


                //throw new InvalidPluginExecutionException("Debugando");
            }
        }

        private Entity obtenerContratoDatosPago(String pCups)
        {
            Entity salida = new Entity();
            try
            {
                QueryByAttribute consulta = new QueryByAttribute("atos_contrato");
                
                consulta.ColumnSet = new ColumnSet("atos_condicionpagoid", "atos_cuenta", "atos_cuentabancaria", "atos_digitocontrol", "atos_entidadbancaria", "atos_formadepago", 
                    "atos_fechafindefinitiva", "atos_iban", "atos_mandatosepa", "atos_plazoenviofacturas", "atos_precioalquilerequipo", "atos_preciomensualserviciolectura", 
                    "atos_tipodeenvio", "atos_sucursalbancaria", "atos_swift");

                consulta.Attributes.AddRange("atos_cups");
                consulta.Values.AddRange(pCups);
                consulta.AddOrder("atos_fechafindefinitiva", OrderType.Descending);

                EntityCollection resConsulta = service.RetrieveMultiple(consulta);

                if (resConsulta.Entities.Count > 0)
                {
                    salida = resConsulta.Entities[0];
                }
            }
            catch (SoapException soapEx)
            {
                throw new Exception(String.Format("Error en la consulta de Contratos:{0}", soapEx.Detail.InnerText));
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error en la consulta de Contratos:{0}", ex.Message));

            }
            return salida;
        }

        private Guid obtenerCondicionPagoIndeterminado()
        {
            Guid salida = new Guid();
            try
            {
                QueryByAttribute consulta = new QueryByAttribute("atos_condiciondepago");

                consulta.Attributes.AddRange("atos_name");
                consulta.Values.AddRange("Indeterminado");

                EntityCollection resConsulta = service.RetrieveMultiple(consulta);

                if (resConsulta.Entities.Count > 0)
                {
                    salida = resConsulta.Entities[0].Id;
                }
            }
            catch (SoapException soapEx)
            {
                throw new Exception(String.Format("Error en la consulta de Condiciones de pago:{0}", soapEx.Detail.InnerText));
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error en la consulta de Condiciones de pago:{0}", ex.Message));

            }
            return salida;
        }
    }
}
