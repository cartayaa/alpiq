using System;
using Microsoft.Xrm.Sdk;
using System.Web.Services.Protocols;
using Microsoft.Xrm.Sdk.Query;

namespace ActDeudaFacturas
{
    public class ActDeudaFacturas:IPlugin
    {
        private ITracingService tracingService;
        private IPluginExecutionContext PluginExecutionContext; ///< Contexto de ejecución del plugin
        private IOrganizationServiceFactory factory;
        private IOrganizationService _service;

        private bool _log = false; ///< Indica si se activa o no el log. Esta variable debe inicializarse según el parámetro recibido en el constructor.
        private String _ficheroLog = "D:\\Tmp\\DeudaFacturas.log";  ///< Fichero de log. Esta variable debe inicializarse según el parámetro recibido en el constructor.
        private const Char SEPARADOR = '#'; ///< Constante para el separador a usar en el parámetro que recibe el constructor
        private const int ESTADO_DEUDA_VENCIDA = 300000000;                                    ///
        private const int ESTADO_DEUDA_DEVUELTA = 300000001;


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
        public ActDeudaFacturas(String parametros)
        {
            if (!String.IsNullOrEmpty(parametros))
            {
                String[] arrayPar = parametros.Split(SEPARADOR);
                if (arrayPar.Length > 0)
                {
                    if (arrayPar[0] == "LOG")
                        _log = true;
                    if (arrayPar.Length > 1)
                        _ficheroLog = arrayPar[1];
                    else
                        _log = false;
                        
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
                System.IO.File.AppendAllText(_ficheroLog, texto + "\r\n");
        }

        /**
        // <summary>
        // Punto de entrada al plugin
        // </summary>
        // <param name="serviceProvider">IServiceProvider.</param>
        // <remarks>
        // El plugin se ejecuta en:
        // - Creación de instalación
        // - Creación de razón social
        // - Modificación de instalación de alguno de los campos que se propagan
        // - Modificación de razón social de alguno de los campos que se propagan
        // </remarks>
        */
        public void Execute(IServiceProvider serviceProvider)
        {

            tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            PluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the Organization Service factory service from the service provider
            factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            // Use the factory to generate the Organization Service.
            _service = factory.CreateOrganizationService(PluginExecutionContext.UserId);
            
            writelog("-----------------------------------------");
            writelog(DateTime.Now.ToLocalTime().ToString());
            writelog("Plugin Deuda Facturas");
            writelog("Mensaje: " + PluginExecutionContext.MessageName);

            Entity ef = new Entity();

            Boolean actualizarTotalFacturas = false;
            Boolean actualizarImporteDeuda = false;
            Boolean actualizarInstalacionAnterior = false;
            Boolean actualizarRSAnterior = false;

            Guid instalacionAnterior = new Guid();
            Guid razonSocialAnterior = new Guid();

            if (PluginExecutionContext.MessageName == "Create")
            {
                ef = (Entity)PluginExecutionContext.InputParameters["Target"];
                writelog("Entidad: " + ef.LogicalName);
                actualizarTotalFacturas = ef.Contains("atos_importetotalconimpuestos");
                actualizarImporteDeuda = ef.Contains("statuscode") || actualizarTotalFacturas;
            }
            else if (PluginExecutionContext.MessageName == "Update")
            {
                ef = (Entity)PluginExecutionContext.PostEntityImages["FacturaImage"];
                Entity target = (Entity)PluginExecutionContext.InputParameters["Target"];
                Entity preImage = (Entity)PluginExecutionContext.PreEntityImages["FacturaImage"];

                writelog("Entidad: " + ef.LogicalName);
                actualizarTotalFacturas = target.Contains("atos_importetotalconimpuestos") || target.Contains("statuscode");
                actualizarImporteDeuda = target.Contains("statuscode") || actualizarTotalFacturas;
                if (target.Contains("atos_instalacionid"))
                {
                    actualizarTotalFacturas = true;
                    actualizarImporteDeuda = true;
                    if (preImage.Contains("atos_instalacionid"))
                    {
                        writelog("EL usuario ha eliminado la instalación y le ha dado a guardar");
                        if (!ef.Contains("atos_instalacionid"))
                            throw new InvalidPluginExecutionException("La instalación no puede eliminarse si estaba previamente informada");

                        instalacionAnterior = ((EntityReference)preImage.Attributes["atos_instalacionid"]).Id;
                        actualizarInstalacionAnterior = instalacionAnterior != ((EntityReference)target.Attributes["atos_instalacionid"]).Id;
                    }
                }
                if (target.Contains("atos_razonsocialid"))
                {
                    actualizarTotalFacturas = true;
                    actualizarImporteDeuda = true;
                    if (preImage.Contains("atos_razonsocialid"))
                    {
                        razonSocialAnterior = ((EntityReference)preImage.Attributes["atos_razonsocialid"]).Id;
                        actualizarRSAnterior = razonSocialAnterior != ((EntityReference)target.Attributes["atos_razonsocialid"]).Id;
                    }
                }

            }
            else if (PluginExecutionContext.MessageName == "Delete")
            {
                ef = (Entity)PluginExecutionContext.PreEntityImages["PreDeleteImage"];
                actualizarTotalFacturas = true;
                actualizarImporteDeuda = true;
            }

            if (ef.LogicalName == "atos_facturas")
            {
                if (actualizarInstalacionAnterior)
                {
                    writelog("actualiza instalacion anterior");
                    actualizarTotalInstalacion(instalacionAnterior);
                    actualizarDeudaInstalacion(instalacionAnterior);
                }

                if (actualizarRSAnterior)
                {
                    writelog("actualiza RS anterior");
                    actualizarTotalRS(razonSocialAnterior);
                    actualizarDeudaRS(razonSocialAnterior);
                }
            }

            if (ef.LogicalName == "atos_facturas" && actualizarTotalFacturas)
            {
                

                if (ef.Attributes.Contains("atos_instalacionid"))
                {
                    writelog(String.Format ("actualizar instalacion: {0}", ((EntityReference)ef.Attributes["atos_instalacionid"]).Id.ToString()));
                    //actualizar con el valor de las facturas de la instalación
                    actualizarTotalInstalacion(((EntityReference)ef.Attributes["atos_instalacionid"]).Id);

                    if (ef.Attributes.Contains("atos_razonsocialid"))
                    {
                        writelog(String.Format("actualizar RS: {0}", ((EntityReference)ef.Attributes["atos_razonsocialid"]).Id.ToString()));

                        //actualizar con el valor de las facturas de la razón social
                       actualizarTotalRS(((EntityReference)ef.Attributes["atos_razonsocialid"]).Id);
                    }


                }
                else if (ef.Attributes.Contains("atos_razonsocialid"))
                {
                    writelog(String.Format("actualizar sólo RS: {0}", ((EntityReference)ef.Attributes["atos_razonsocialid"]).Id.ToString()));
                    
                    //actualizar con el valor de las facturas de la razón social
                    actualizarTotalRS(((EntityReference)ef.Attributes["atos_razonsocialid"]).Id);
                }
            }

            //Importe de deuda
            if (ef.LogicalName == "atos_facturas" && actualizarImporteDeuda)
            {
                if (ef.Attributes.Contains("atos_instalacionid"))
                {
                    writelog("actualizar deuda instalacion");
                    //actualizar con el valor de las facturas de la instalación
                    actualizarDeudaInstalacion(((EntityReference)ef.Attributes["atos_instalacionid"]).Id);
                    
                    if (ef.Attributes.Contains("atos_razonsocialid"))
                    {
                        writelog("actualizar deuda RS");
                        //actualizar con el valor de las facturas de la razón social
                        actualizarDeudaRS(((EntityReference)ef.Attributes["atos_razonsocialid"]).Id);
                    }
                }
                else if (ef.Attributes.Contains("atos_razonsocialid"))
                {
                    writelog("actualizar RS");
                    //actualizar con el valor de las facturas de la razón social
                    actualizarDeudaRS(((EntityReference)ef.Attributes["atos_razonsocialid"]).Id);
                }
            }
        }

        private void actualizarTotalInstalacion(Guid pId)
        {
            try
            {
                Nullable<decimal> totalFacturas = calcularTotalFacturas(pId, "atos_instalacionid");
                if (totalFacturas.HasValue)
                {
                    writelog("valor:" + totalFacturas.Value.ToString());
                    Entity instalacion = new Entity("atos_instalacion");
                    instalacion.Id = pId;
                    instalacion.Attributes["atos_facturaciontotal"] = new Money(totalFacturas.Value);
                    _service.Update(instalacion);
                }
            }
            catch (Exception ex)
            {
                writelog(ex.Message);
                throw ex;
            }
        }

        private void actualizarTotalRS(Guid pId)
        {
            try
            {
                Nullable<decimal> totalFacturas = calcularTotalFacturas(pId, "atos_razonsocialid");
                if (totalFacturas.HasValue)
                {
                    writelog("valor:" + totalFacturas.Value.ToString());
                    Entity razonSocial = new Entity("account");
                    razonSocial.Id = pId;
                    razonSocial.Attributes["atos_facturaciontotal"] = new Money(totalFacturas.Value);
                    _service.Update(razonSocial);
                }
            }
            catch (Exception ex)
            {
                writelog(ex.Message);
                throw ex;
            }
        }

        private void actualizarDeudaInstalacion(Guid pId)
        {
            try
            {
                Nullable<decimal> totalDeuda = calcularImporteDeuda(pId, "atos_instalacionid");
                if (totalDeuda.HasValue)
                {
                    writelog("deuda:" + totalDeuda.Value.ToString());
                    Entity instalacion = new Entity("atos_instalacion");
                    instalacion.Id = pId;
                    instalacion.Attributes["atos_deudaactualizada"] = new Money(totalDeuda.Value);
                    _service.Update(instalacion);
                }
            }
            catch (Exception ex)
            {
                writelog(ex.Message);
                throw ex;
            }
        }

        private void actualizarDeudaRS(Guid pId)
        {
            try
            {
                //actualizar con el valor de las facturas de la razón social
                Nullable<decimal> totalDeuda = calcularImporteDeuda(pId, "atos_razonsocialid");
                if (totalDeuda.HasValue)
                {
                    writelog("deuda:" + totalDeuda.Value.ToString());
                    Entity razonSocial = new Entity("account");
                    razonSocial.Id = pId;
                    razonSocial.Attributes["atos_deudaactualizada"] = new Money(totalDeuda.Value);
                    _service.Update(razonSocial);
                }
            }
            catch (Exception ex)
            {
                writelog(ex.Message);
                throw ex;
            }
        }
        private Nullable<decimal> calcularTotalFacturas(Guid pId, String pNombreCampoBusqueda)
        {
            Nullable<decimal> salida = 0;

            try
            {
                EntityCollection facturas = new EntityCollection();
                QueryByAttribute consulta = new QueryByAttribute("atos_facturas");
                consulta.AddAttributeValue(pNombreCampoBusqueda, pId);
                consulta.ColumnSet = new ColumnSet("atos_facturasid","atos_importetotalconimpuestos");
                facturas = _service.RetrieveMultiple(consulta);

                if (facturas.Entities.Count > 0)
                {
                    salida = 0;
                    foreach (Entity factura in facturas.Entities)
                    {
                        if (factura.Attributes.Contains("atos_importetotalconimpuestos"))
                            salida = salida + ((Money)factura.Attributes["atos_importetotalconimpuestos"]).Value;
                    }
                }
            }
            catch (SoapException soex)
            {
                writelog(soex.Detail.InnerText);
                salida = null;
            }
            catch (Exception ex)
            {
                writelog(ex.Message);
                salida = null;
            }

            return salida;
        }

        private Nullable<decimal> calcularImporteDeuda(Guid pId, String pNombreCampoBusqueda)
        {
            Nullable<decimal> salida = 0;

            try
            {
                EntityCollection facturas = new EntityCollection();
                QueryExpression query = new QueryExpression()
                {
                    Distinct = false,
                    EntityName = "atos_facturas",
                    ColumnSet = new ColumnSet("atos_facturasid", "atos_importetotalconimpuestos"),
                    Criteria =
                    {
                        Filters = 
                            {
                                new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions = 
                                    {
                                        new ConditionExpression(pNombreCampoBusqueda, ConditionOperator.Equal, pId),
                                    },
                                },
                                new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.Or,
                                    Conditions = 
                                    {
                                        new ConditionExpression("statuscode", ConditionOperator.Equal, ESTADO_DEUDA_VENCIDA),
                                        new ConditionExpression("statuscode", ConditionOperator.Equal, ESTADO_DEUDA_DEVUELTA)
                                    }
                                }
                            }
                    }
                }; 

                facturas = _service.RetrieveMultiple(query);

                if (facturas.Entities.Count > 0)
                {
                    salida = 0;
                    foreach (Entity factura in facturas.Entities)
                    {
                        if (factura.Attributes.Contains("atos_importetotalconimpuestos"))
                            salida = salida + ((Money)factura.Attributes["atos_importetotalconimpuestos"]).Value;
                    }
                }
            }
            catch (SoapException soex)
            {
                writelog(soex.Detail.InnerText);
                salida = null;
            }
            catch (Exception ex)
            {
                writelog(ex.Message);
                salida = null;
            }

            return salida;
        }
    }
}
