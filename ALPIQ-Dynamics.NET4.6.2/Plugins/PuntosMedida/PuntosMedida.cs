using System;
using Microsoft.Xrm.Sdk;
using System.Web.Services.Protocols;
using Microsoft.Xrm.Sdk.Query;

namespace PuntosMedida
{
    public class PuntosMedida:IPlugin
    {

        private ITracingService tracingService;
        private IPluginExecutionContext PluginExecutionContext; ///< Contexto de ejecución del plugin
        private IOrganizationServiceFactory factory;
        private IOrganizationService _service;

        private bool _log = false; ///< Indica si se activa o no el log. Esta variable debe inicializarse según el parámetro recibido en el constructor.
        private String _ficheroLog = "D:\\Tmp\\DeudaFacturas.log";  ///< Fichero de log. Esta variable debe inicializarse según el parámetro recibido en el constructor.
        private const Char SEPARADOR = '#'; ///< Constante para el separador a usar en el parámetro que recibe el constructor

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

            try
            {
                tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                PluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                // Obtain the Organization Service factory service from the service provider
                factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                // Use the factory to generate the Organization Service.
                _service = factory.CreateOrganizationService(PluginExecutionContext.UserId);

                writelog("-----------------------------------------");
                writelog(DateTime.Now.ToLocalTime().ToString());
                writelog("Plugin Puntos Medida");
                writelog("Mensaje: " + PluginExecutionContext.MessageName);

                Entity ef = new Entity();

                Entity instalacion = new Entity("atos_instalacion");

                if (PluginExecutionContext.MessageName == "Create")
                {
                    ef = (Entity)PluginExecutionContext.InputParameters["Target"];
                    writelog(String.Format("Entidad:{0} - Mensaje:{1}", ef.LogicalName, PluginExecutionContext.MessageName));

                    if (ef.Attributes.Contains("atos_instalacionid") && ef.Contains("atos_codigopuntomedida"))
                    {
                        instalacion.Id = ((EntityReference)ef.Attributes["atos_instalacionid"]).Id;

                        Nullable<int> periodo = periodoCodPuntoMedida(ef.Attributes["atos_codigopuntomedida"].ToString());

                        if (periodo.HasValue && esPrincipal(instalacion.Id, periodo.Value))
                        {
                            tracingService.Trace("Modificar instalación");

                            if (ef.Contains("atos_tipopuntomedidaid"))
                                instalacion.Attributes["atos_tipopuntomedidaid"] = new EntityReference("atos_tipopuntomedida", ((EntityReference)ef.Attributes["atos_tipopuntomedidaid"]).Id);
                            else
                                instalacion.Attributes["atos_tipopuntomedidaid"] = null;
                            if (ef.Contains("atos_modolecturaid"))
                                instalacion.Attributes["atos_modolecturapuntomedidaid"] = new EntityReference("atos_modolecturapuntomedida", ((EntityReference)ef.Attributes["atos_modolecturaid"]).Id);
                            else
                                instalacion.Attributes["atos_modolecturapuntomedidaid"] = null;

                            if (ef.Contains("atos_tipopropiedadaparatoid"))
                                instalacion.Attributes["atos_tipopropiedadaparatoid"] = new EntityReference("atos_tipopropiedadaparato", ((EntityReference)ef.Attributes["atos_tipopropiedadaparatoid"]).Id);
                            else
                                instalacion.Attributes["atos_tipopropiedadaparatoid"] = null;

                            if (ef.Contains("atos_importealquilerequipo"))
                                instalacion.Attributes["atos_importealquilerequipo"] = ef.Attributes["atos_importealquilerequipo"];
                            else
                                instalacion.Attributes["atos_importealquilerequipo"] = null;

                            _service.Update(instalacion);

                            writelog("fin modificar instalación");
                        }
                    }
                }
                else if (PluginExecutionContext.MessageName == "Update")
                {


                    Entity postImage = (Entity)PluginExecutionContext.PostEntityImages["PuntoMedidaImage"];
                    Entity preImage = (Entity)PluginExecutionContext.PreEntityImages["PuntoMedidaImage"];
                    Entity target = (Entity)PluginExecutionContext.InputParameters["Target"];


                    //Entity preImage = (Entity)PluginExecutionContext.PreEntityImages["PuntoMedidaImage"];
                    writelog(String.Format("Entidad:{0} - Mensaje:{1}", postImage.LogicalName, PluginExecutionContext.MessageName));

                    if (postImage.Contains("atos_instalacionid") && postImage.Contains("atos_codigopuntomedida"))
                    {
                        instalacion.Id = ((EntityReference)postImage.Attributes["atos_instalacionid"]).Id;

                        Entity anterior = _service.Retrieve("atos_puntomedida", postImage.Id, new ColumnSet(true));

                        Nullable<int> periodo = periodoCodPuntoMedida(postImage.Attributes["atos_codigopuntomedida"].ToString());
                        Nullable<int> periodoAnt = periodoCodPuntoMedida(preImage.Attributes["atos_codigopuntomedida"].ToString());

                        writelog("periodo antes de actualizar-->" + (periodoAnt.HasValue ? periodoAnt.Value.ToString() : string.Empty));
                        writelog("periodo durante la actualizacion-->" + (periodo.HasValue ? periodo.Value.ToString() : string.Empty));

                        if (periodo.HasValue && esPrincipal(instalacion.Id, periodo.Value))
                        {
                            //se cogerán los valores de la postImage, puesto que hay que volcar todos los datos por si hubiera cambio 
                            //-si se modifica la instalación hay que buscar la anterior instalación y modificar? No porque se puede ir al infinito.
                            tracingService.Trace("Modificar instalación");


                            if (postImage.Contains("atos_tipopuntomedidaid"))
                            {
                                instalacion.Attributes["atos_tipopuntomedidaid"] = new EntityReference("atos_tipopuntomedida", ((EntityReference)postImage.Attributes["atos_tipopuntomedidaid"]).Id);
                            }
                            else
                            {
                                instalacion.Attributes["atos_tipopuntomedidaid"] = null;
                            }

                            if (postImage.Contains("atos_modolecturaid"))
                            {
                                instalacion.Attributes["atos_modolecturapuntomedidaid"] = new EntityReference("atos_modolecturapuntomedida", ((EntityReference)postImage.Attributes["atos_modolecturaid"]).Id);
                            }
                            else
                            {
                                instalacion.Attributes["atos_modolecturapuntomedidaid"] = null;
                            }

                            if (postImage.Contains("atos_tipopropiedadaparatoid"))
                            {
                                instalacion.Attributes["atos_tipopropiedadaparatoid"] = new EntityReference("atos_tipopropiedadaparato", ((EntityReference)postImage.Attributes["atos_tipopropiedadaparatoid"]).Id);
                            }
                            else
                            {
                                instalacion.Attributes["atos_tipopropiedadaparatoid"] = null;
                            }

                            if (postImage.Contains("atos_importealquilerequipo"))
                            {
                                instalacion.Attributes["atos_importealquilerequipo"] = postImage.Attributes["atos_importealquilerequipo"];
                            }
                            else
                            {
                                instalacion.Attributes["atos_importealquilerequipo"] = null;
                            }

                            _service.Update(instalacion);

                            writelog("fin modificar instalación");
                        }

                    }
                }
                else if (PluginExecutionContext.MessageName == "Delete")
                {
                    Entity preImage = (Entity)PluginExecutionContext.PreEntityImages["PreDeleteImageRS"];

                    if (preImage.Attributes.Contains("atos_instalacionid") && preImage.Contains("atos_codigopuntomedida"))
                    {
                        instalacion.Id = ((EntityReference)preImage.Attributes["atos_instalacionid"]).Id;

                        Nullable<int> periodo = periodoCodPuntoMedida(preImage.Attributes["atos_codigopuntomedida"].ToString());
                        if (periodo.HasValue && esPrincipal(instalacion.Id, periodo.Value))
                        {
                            Guid puntoMedidaPrincipalId = buscarPrincipal(instalacion.Id, preImage.Id);
                            if (puntoMedidaPrincipalId != Guid.Empty)
                            {
                                //en caso de eliminar un Punto de medida:
                                //      - Si no es el principal no se hace nada.
                                //      - Si es el principal: buscar el nuevo principal y volcar la información
                                Entity puntoMedidaPrincipal = _service.Retrieve("atos_puntomedida", puntoMedidaPrincipalId, new ColumnSet(true));
                                tracingService.Trace("Modificar instalación");

                                if (puntoMedidaPrincipal.Contains("atos_tipopuntomedidaid"))
                                    instalacion.Attributes["atos_tipopuntomedidaid"] = new EntityReference("atos_tipopuntomedida", ((EntityReference)puntoMedidaPrincipal.Attributes["atos_tipopuntomedidaid"]).Id);
                                else
                                    instalacion.Attributes["atos_tipopuntomedidaid"] = null;

                                if (puntoMedidaPrincipal.Contains("atos_modolecturaid"))
                                    instalacion.Attributes["atos_modolecturapuntomedidaid"] = new EntityReference("atos_modolecturapuntomedida", ((EntityReference)puntoMedidaPrincipal.Attributes["atos_modolecturaid"]).Id);
                                else
                                    instalacion.Attributes["atos_modolecturapuntomedidaid"] = null;

                                if (puntoMedidaPrincipal.Contains("atos_tipopropiedadaparatoid"))
                                    instalacion.Attributes["atos_tipopropiedadaparatoid"] = new EntityReference("atos_tipopropiedadaparato", ((EntityReference)puntoMedidaPrincipal.Attributes["atos_tipopropiedadaparatoid"]).Id);
                                else
                                    instalacion.Attributes["atos_tipopropiedadaparatoid"] = null;

                                if (puntoMedidaPrincipal.Contains("atos_importealquilerequipo"))
                                    instalacion.Attributes["atos_importealquilerequipo"] = puntoMedidaPrincipal.Attributes["atos_importealquilerequipo"];
                                else
                                    instalacion.Attributes["atos_importealquilerequipo"] = null;

                                _service.Update(instalacion);

                                writelog("fin modificar instalación");
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                writelog("Error --> Mensaje:" + ex.Message);
                throw ex;
            }
        }

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
        public PuntosMedida(String parametros)
        {
            _log = false;
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


        private Nullable<int> periodoCodPuntoMedida(String pCodPunto)
        {
            Nullable<int> salida = null;

            if (!String.IsNullOrEmpty(pCodPunto))
            {
                int valPeriodo;
                if (int.TryParse(pCodPunto.Substring(pCodPunto.Length - 2, 1), out valPeriodo))
                    salida = valPeriodo;
            }

         
            return salida;
        }

        private bool esPrincipal(Guid pIdInstalacion, int pPeriodosActual)
        {
            bool salida = false;

            tracingService.Trace(String.Format("[esPrincipal]:{0},{1}",pIdInstalacion.ToString(),pPeriodosActual.ToString()));
            writelog(String.Format("[esPrincipal]:{0},{1}", pIdInstalacion.ToString(), pPeriodosActual.ToString()));
            try
            {
                EntityCollection puntosMedida = new EntityCollection();
                QueryExpression query = new QueryExpression()
                {
                    Distinct = false,
                    EntityName = "atos_puntomedida",
                    ColumnSet = new ColumnSet("atos_codigopuntomedida"),
                    Criteria =
                    {
                        Filters = 
                            {
                                new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions = 
                                    {
                                        new ConditionExpression("atos_instalacionid", ConditionOperator.Equal, pIdInstalacion),
                                    },
                                },
                            }
                    }
                };

                puntosMedida = _service.RetrieveMultiple(query);

                if (puntosMedida.Entities.Count > 1)
                {
                    salida = true;
                    foreach (Entity puntoMedida in puntosMedida.Entities)
                    {
                        if (puntoMedida.Attributes.Contains("atos_codigopuntomedida"))
                        {
                            Nullable<int> periodoPM = periodoCodPuntoMedida(puntoMedida.Attributes["atos_codigopuntomedida"].ToString());
                            writelog("punto de medida actual-->" + pPeriodosActual.ToString() + " punto de medida de comparacion-->" + (periodoPM.HasValue ? periodoPM.Value.ToString() : string.Empty)); 
                            salida = salida && periodoPM.HasValue && (pPeriodosActual <= periodoPM.Value);
                            if (salida == true)
                                break;
                        }
                    }
                }
                else
                {
                    salida = true;
                }

            }
            catch (SoapException soex)
            {
                writelog(soex.Detail.InnerText);
                salida = false;
            }
            catch (Exception ex)
            {
                writelog(ex.Message);
                salida = false;
            }

            writelog("es punto de medida principal:" + salida.ToString());
            return salida;
        }
        
        // busca el punto de medida principal excluyendo el que esta el guid que le pasemos 
        private Guid buscarPrincipal(Guid pIdInstalacion, Guid puntoMedioExcluido)
        {
            Guid salida = Guid.Empty;

           // tracingService.Trace(String.Format("[esPrincipal]:{0},{1}", pIdInstalacion.ToString()));
           // writelog(String.Format("[esPrincipal]:{0},{1}", pIdInstalacion.ToString()));
            try
            {
                EntityCollection puntosMedida = new EntityCollection();
                QueryExpression query = new QueryExpression()
                {
                    Distinct = false,
                    EntityName = "atos_puntomedida",
                    ColumnSet = new ColumnSet("atos_codigopuntomedida"),
                    Criteria =
                    {
                        Filters = 
                            {
                                new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions = 
                                    {
                                        new ConditionExpression("atos_instalacionid", ConditionOperator.Equal, pIdInstalacion),
                                    },
                                },
                            }
                    }
                };

                puntosMedida = _service.RetrieveMultiple(query);

                if (puntosMedida.Entities.Count > 0)
                {
                    int periodoMin = int.MaxValue;
                    foreach (Entity puntoMedida in puntosMedida.Entities)
                    {
                        if (puntoMedida.Attributes.Contains("atos_codigopuntomedida"))
                        {
                            Nullable<int> periodoPM = periodoCodPuntoMedida(puntoMedida.Attributes["atos_codigopuntomedida"].ToString());

                            if (periodoPM.HasValue && (periodoPM < periodoMin) && puntoMedida.Id != puntoMedioExcluido)
                            {
                                periodoMin = periodoPM.Value;
                                salida = puntoMedida.Id;
                            }
                        }
                    }
                }
                else
                {
                    salida = Guid.Empty;
                }

            }
            catch (SoapException soex)
            {
                writelog(soex.Detail.InnerText);
                salida = Guid.Empty;
            }
            catch (Exception ex)
            {
                writelog(ex.Message);
                salida = Guid.Empty;
            }
            writelog("nuevo punto de medida principal--> " +salida.ToString());
            return salida;
        }
    }
}
