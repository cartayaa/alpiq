/**
// <summary>
// Plugin para clonar registros de instalaciones y razones sociales a modo de histórico.
// </summary>
 */
namespace Historicos
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System.Collections.Generic;
    using Microsoft.Xrm.Sdk.Metadata;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Crm.Sdk.Messages;

    /**
    // <summary>
    // Clase base del Histórico de Instalaciones y Razones Sociales
    // </summary>
    // <remarks>
    // Deriva de IPlugin
    // </remarks>
     */    
    public class Historicos : IPlugin
    {

        private bool _log = false;
        private String ficherolog = "D:\\Tmp\\HistoricoInstalacion.txt";
        private const Char SEPARADOR = '#';
        private String[] campos = null;

        public Historicos(String parametros)
        {
            if (String.IsNullOrEmpty(parametros) == false)
            {
                String[] arrayPar = parametros.Split(SEPARADOR);
                if (arrayPar.Length > 0)
                {

                    if (arrayPar[0] == "LOG")
                        _log = true;
                    if (arrayPar.Length > 1)
                    {
                        ficherolog = arrayPar[1];
                        if (ficherolog == "")
                            _log = false;
                        if (arrayPar.Length > 2)
                        {
                            campos = new String[arrayPar.Length - 2];
                            for (int i = 2; i < arrayPar.Length; i++)
                            {
                                campos[i - 2] = arrayPar[i];
                            }
                        }
                    }
                }
            }
        }

        private void writelog(String texto)
        {
            if (_log == true)
                System.IO.File.AppendAllText(ficherolog, texto + "\r\n");
        }


        protected class LocalPluginContext
        {
            /**
            // <summary>
            // Devuelve el objeto ServiceProvider (IServiceProvider)
            // </summary>
             */
            internal IServiceProvider ServiceProvider
            {
                get;

                private set;
            }


            /**
            // <summary>
            // Devuelve el objeto OrganizationService (IOrganizationService)
            // </summary>
             */
            internal IOrganizationService OrganizationService
            {
                get;

                private set;
            }

            /**
            // <summary>
            // Devuelve el objeto PluginExecutionContext (IPluginExecutionContext)
            // </summary>
             */
            internal IPluginExecutionContext PluginExecutionContext
            {
                get;

                private set;
            }

            /**
            // <summary>
            // Devuelve el objeto TracingService (ITracingService)
            // </summary>
             */
            internal ITracingService TracingService
            {
                get;

                private set;
            }


            /**
            // <summary>
            // Constructor de LocalPluginContext
            // </summary>
            // <param name="serviceProvider">IServiceProvider</param>
             */
            internal LocalPluginContext(IServiceProvider serviceProvider)
            {
                if (serviceProvider == null)
                {
                    throw new ArgumentNullException("serviceProvider");
                }

                // Obtain the execution context service from the service provider.
                this.PluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                // Obtain the tracing service from the service provider.
                this.TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                // Obtain the Organization Service factory service from the service provider
                IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                // Use the factory to generate the Organization Service.
                this.OrganizationService = factory.CreateOrganizationService(this.PluginExecutionContext.UserId);
            }

            /**
            // <summary>
            // "Pinta" el mensaje recibido por parámetro
            // </summary>
            // <param name="message">Mensaje para mostrar en la traza</param>
             */
            internal void Trace(string message)
            {
                if (string.IsNullOrWhiteSpace(message) || this.TracingService == null)
                {
                    return;
                }

                if (this.PluginExecutionContext == null)
                {
                    this.TracingService.Trace(message);
                }
                else
                {
                    this.TracingService.Trace(
                        "{0}, Correlation Id: {1}, Initiating User: {2}",
                        message,
                        this.PluginExecutionContext.CorrelationId,
                        this.PluginExecutionContext.InitiatingUserId);
                }
            }
        }


        /**
        // <summary>
        // Desactiva un registro
        // </summary>
        // <param name="_entidad">Nombre de la entidad.</param>
        // <param name="_guid">Guid del registro a desactivar.</param>
        // <param name="_organizationService">IOrganizationService.</param>
        // <remarks>
        // Cambia el valor de statecode a 1 y de statuscode a 2.
        // </remarks>
         */
        private void DesactivaRegistro(string _entidad, Guid _guid, IOrganizationService _organizationService)
        {
            var cols = new ColumnSet(new[] { "statecode", "statuscode" });

            //Comprobamos si está activo o no
            var entity = _organizationService.Retrieve(_entidad, _guid, cols);

            if (entity != null && entity.GetAttributeValue<OptionSetValue>("statecode").Value == 0)
            {
                //StateCode = 1 y StatusCode = 2 para desactivar 
                SetStateRequest setStateRequest = new SetStateRequest()
                {
                    EntityMoniker = new EntityReference
                    {
                        Id = _guid,
                        LogicalName = _entidad,
                    },
                    State = new OptionSetValue(1),
                    Status = new OptionSetValue(2)
                };
                _organizationService.Execute(setStateRequest);
            }
        }


        /**
        // <summary>
        // Busca si hay algún contrato para la instalación
        // </summary>
        // <param name="_guid">Guid del registro "padre".</param>
        // <param name="_nbCampoId">Nombre del campo del registro "padre".</param>
        // <param name="_organizationService">IOrganizationService.</param>
        // <remarks>
        // Devuelve true si hay algún contrato y false si no hay contratos para instalación recibida por parámetro.
        // </remarks>
         */
        private bool tieneContratos(Guid _guid, String _nbCampoId, IOrganizationService _organizationService)
        {
            QueryByAttribute _consulta = new QueryByAttribute("atos_contrato");

            _consulta.ColumnSet = new ColumnSet("atos_contratoid");
            _consulta.AddAttributeValue(_nbCampoId, _guid);

            EntityCollection _resConsulta = _organizationService.RetrieveMultiple(_consulta);
            if (_resConsulta.Entities.Count > 0)
                return true;
            return false;
        }


        /**
        // <summary>
        // Indica si el atributo no se puede copiar.
        // </summary>
        // <param name="nbAtributo">Nombre del atributo.</param>
        // <remarks>
        // Devuelve false si el atributo es address1_addressid o address2_addressid o address3_addressid. Estos atributos son especiales y da problemas al copiarlos.
        // </remarks>
         */
        private bool excepcionAtributo(String nbAtributo)
        {
            bool ret = false;

            if (nbAtributo == "address1_addressid" ||
                 nbAtributo == "address2_addressid" ||
                 nbAtributo == "address3_addressid")
                ret = true;

            return ret;
        }


        /**
        // <summary>
        // Devuelve si el atributo pasado se copia o no se copia.
        // </summary>
        // <param name="nombreAtributo">Nombre del atributo que se va a determinar si se copia o no</param>
        // <param name="atributoId">Nombre del Id de la entidad</param>
        // <remarks>
        // Si nombreAtributo es statuscode, overriddencreatedon o es el Id de la entidad no se copia (devuelve false), en otro caso devuelve true
        // </remarks>
         */
        private bool copiarAtributo(String nombreAtributo, String atributoId)
        {
            if (nombreAtributo != "statuscode" &&
                 nombreAtributo != "overriddencreatedon" &&
                 nombreAtributo != atributoId)
                return true;
            return false;
        }

        /**
        // <summary>
        // Crea una copia del registro antes de modificar
        // </summary>
        // <param name="efpre">Registro antes de la modificación</param>
        // <param name="localcontext">Objeto de LocalPluginContext</param>
        // <remarks>
        // Crea una copia del registro recibido en efpre. Copia solo los artículos que tengan true en la propiedad IsValidForCreate y además no sea ninguna de las excepciones (según función excepcionAtributo).
        // </remarks>
         */
        private Entity copiaAtributos(Entity efpre, LocalPluginContext localcontext)
        {

            Entity historico = new Entity(efpre.LogicalName);

            RetrieveEntityRequest mdRequest = new RetrieveEntityRequest()
            {
                EntityFilters = EntityFilters.Attributes,
                LogicalName = efpre.LogicalName,
                RetrieveAsIfPublished = false
            };

            RetrieveEntityResponse entityResponse = (RetrieveEntityResponse)localcontext.OrganizationService.Execute(mdRequest);
            EntityMetadata entityMD = entityResponse.EntityMetadata;
            AttributeMetadata[] attMD = entityMD.Attributes;

            for (int i = 0; i < attMD.Length; i++)
            {
                localcontext.Trace("copiaAtributos: " + attMD[i].LogicalName + " Tipo: " + attMD[i].AttributeTypeName);
                if (attMD[i].IsValidForCreate == true && !excepcionAtributo(attMD[i].LogicalName))
                {
                    if (efpre.Attributes.Contains(attMD[i].LogicalName) && 
                        copiarAtributo(attMD[i].LogicalName, historico.LogicalName + "id") == true)
                    {
                        localcontext.Trace("clonaRazonSocial 4.1 - Copiando atributo " + attMD[i].LogicalName);
                        writelog("clonaRazonSocial 4.1 - Copiando atributo " + attMD[i].LogicalName);
                        
                        if (attMD[i].AttributeType == AttributeTypeCode.Lookup)
                            historico.Attributes[attMD[i].LogicalName] = new EntityReference(((EntityReference)efpre.Attributes[attMD[i].LogicalName]).LogicalName, ((EntityReference)efpre.Attributes[attMD[i].LogicalName]).Id);
                        else
                            historico.Attributes[attMD[i].LogicalName] = efpre.Attributes[attMD[i].LogicalName];
                    }

                }
            }

            /*historico.Attributes.Remove(historico.LogicalName + "id");
            historico.Attributes.Remove("statuscode");
            if (historico.Attributes.Contains("createdon"))
            {
                writelog("Fecha de creación " + ((DateTime) historico.Attributes["createdon"]).ToLocalTime().ToString());
                historico.Attributes.Remove("createdon");
            }
            if (historico.Attributes.Contains("createdby"))
                historico.Attributes.Remove("createdby");*/

            //historico.Attributes["createdon"] = DateTime.Now;
            //historico.Attributes["overriddencreatedon"] = DateTime.Now;
            return historico;
        }


        /**
        // <summary>
        // Crea una copia de la instalación
        // </summary>
        // <param name="efpre">Registro antes de la modificación</param>
        // <param name="efpost">Registro después de la modificación</param>
        // <param name="localcontext">Objeto de LocalPluginContext</param>
        // <remarks>
        // La copia la hace si se ha modificado alguno de los atributos recibidos por el plugin. si no reciba el plugin el nombre de los atributos comprueba que se ha modificado alguno de los siguientes:.
        // -# atos_tarifaid 
        // -# atos_interrumpibilidad 
        // -# atos_tipoexencionie 
        // -# atos_potenciacontratada1 
        // -# atos_potenciacontratada2 
        // -# atos_potenciacontratada3 
        // -# atos_potenciacontratada4 
        // -# atos_potenciacontratada5 
        // -# atos_potenciacontratada6 
        // -# atos_instalaciondireccionconcatenada 
        // -# atos_instalacioncodigopostalid 
        // -# atos_instalacionpoblacionid 
        // -# atos_instalacionmunicipioid 
        // -# atos_instalacionprovinciaid 
        // -# atos_instalacionccaaid 
        // -# atos_instalacionpaisid 
        // - Además la instalación debe tener al menos un contrato. 
        // - El registro copiado tiene como fecha de fin de vigencia la nueva fecha de inicio de vigencia menos 1 día
        // - También tiene activo el check de histórico e informado el lookup de instalación original
        // - Una vez creado se desactiva el registro.  
        // </remarks>
         */
        private void clonaInstalacion(Entity efpre, Entity efpost, LocalPluginContext localcontext)
        {

            if (campos == null)
            {
                campos = new String[16];
                campos[0] = "atos_tarifaid";
                campos[1] = "atos_interrumpibilidad";
                campos[2] = "atos_tipoexencionie";
                campos[3] = "atos_potenciacontratada1";
                campos[4] = "atos_potenciacontratada2";
                campos[5] = "atos_potenciacontratada3";
                campos[6] = "atos_potenciacontratada4";
                campos[7] = "atos_potenciacontratada5";
                campos[8] = "atos_potenciacontratada6";
                campos[9] = "atos_instalaciondireccionconcatenada";
                campos[10] = "atos_instalacioncodigopostalid";
                campos[11] = "atos_instalacionpoblacionid";
                campos[12] = "atos_instalacionmunicipioid";
                campos[13] = "atos_instalacionprovinciaid";
                campos[14] = "atos_instalacionccaaid";
                campos[15] = "atos_instalacionpaisid";
                
            }

            
            if ( campos != null )
            {
                localcontext.Trace("9.2");
                bool hayCambio = false;
                for (int i = 0; i < campos.Length; i++)
                {
                    bool esLookup = false;
                    bool esOption = false;

                    if (efpre.Attributes.Contains(campos[i]) && efpre.Attributes[campos[i]].ToString() =="Microsoft.Xrm.Sdk.EntityReference")
                    {
                        esLookup = true;
                        localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "es de tipo lookup el campo " + campos[i]));
                    }
                    else if (efpre.Attributes.Contains(campos[i]) && efpre.Attributes[campos[i]].ToString() == "Microsoft.Xrm.Sdk.OptionSetValue")
                    {
                        esOption = true;
                        localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "es de tipo option el campo " + campos[i]));
                    }

                    //if (efpost.Attributes.Contains(campos[i]))
                    writelog("Comprobando cambios del campo " + campos[i]);
                    if ((efpre.Attributes.Contains(campos[i]) && efpost.Attributes.Contains(campos[i])
                          && efpre.Attributes[campos[i]].ToString() != efpost.Attributes[campos[i]].ToString()) ||
                          (esLookup && efpre.Attributes.Contains(campos[i]) && efpost.Attributes.Contains(campos[i])
                          && ((EntityReference)efpre.Attributes[campos[i]]).Id != ((EntityReference)efpost.Attributes[campos[i]]).Id) || 
                          (esOption && efpre.Attributes.Contains(campos[i]) && efpost.Attributes.Contains(campos[i])
                          && ((OptionSetValue)efpre.Attributes[campos[i]]).Value != ((OptionSetValue)efpost.Attributes[campos[i]]).Value) ||
                         (efpre.Attributes.Contains(campos[i]) && !efpost.Attributes.Contains(campos[i])) ||
                         (!efpre.Attributes.Contains(campos[i]) && efpost.Attributes.Contains(campos[i])))
                    {
                        localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "Se ha modificado el campo " + campos[i] ));
                        writelog(string.Format(CultureInfo.InvariantCulture, "Se ha modificado el campo " + campos[i]));
                        hayCambio = true;
                        break;
                    }
                }
                if (hayCambio == false)
                {
                    localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "No se ha modificado ninguno de los campos que provocan la creación de un registro para el histórico"));
                    writelog(string.Format(CultureInfo.InvariantCulture, "No se ha modificado ninguno de los campos que provocan la creación de un registro para el histórico"));
                    return;
                }
            }
            localcontext.Trace("10");
            if (!tieneContratos(efpost.Id, "atos_instalacionid", localcontext.OrganizationService))
            {
                localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "La instalación no tiene contratos por tanto no se crea registro para el histórico"));
                writelog(string.Format(CultureInfo.InvariantCulture, "La instalación no tiene contratos por tanto no se crea registro para el histórico"));
                return;
            }
            localcontext.Trace("11");


            Entity historico = copiaAtributos(efpre, localcontext);

            historico.Attributes["atos_instalacionoriginalid"] = new EntityReference("atos_instalacion", efpost.Id);
            historico.Attributes["atos_historico"] = true;

            if (efpost.Attributes.Contains("atos_fechainiciovigencia") == true)
                historico.Attributes["atos_fechafinvigencia"] = ((DateTime)efpost.Attributes["atos_fechainiciovigencia"]).AddDays(-1);
            else
                historico.Attributes["atos_fechafinvigencia"] = ((DateTime)efpre.Attributes["atos_fechainiciovigencia"]).AddDays(-1);

            //localcontext.Trace("13");
            Guid histGuid = localcontext.OrganizationService.Create(historico);
            //localcontext.Trace("14");
            DesactivaRegistro("atos_instalacion", histGuid, localcontext.OrganizationService);
            //localcontext.Trace("15");
        }

        /**
        // <summary>
        // Crea una copia de la razón social
        // </summary>
        // <param name="efpre">Registro antes de la modificación</param>
        // <param name="efpost">Registro después de la modificación</param>
        // <param name="localcontext">Objeto de LocalPluginContext</param>
        // <remarks>
        // La copia la hace si se ha modificado alguno de los atributos recibidos por el plugin. si no reciba el plugin el nombre de los atributos comprueba que se ha modificado alguno de los siguientes:.
        // -# name
        // - Además la razón social debe tener al menos un contrato. 
        // - El registro copiado tiene como fecha de fin de vigencia la nueva fecha de inicio de vigencia menos 1 día
        // - También tiene activo el check de histórico e informado el lookup de razón social original
        // - Una vez creado se desactiva el registro.  
        // </remarks>
         */
        private void clonaRazonSocial(Entity efpre, Entity efpost, LocalPluginContext localcontext)
        {
            localcontext.Trace("clonaRazonSocial 1 " + efpost.Id.ToString());
            writelog("clonaRazonSocial 1");
            if (campos == null)
            {
                //localcontext.Trace("9.1");
                //if (!efpost.Attributes.Contains("name"))

                writelog("Comprobando si hay cambio para el campo name");
                if (efpre.Attributes.Contains("name") && efpost.Attributes.Contains("name"))
                    writelog("efpre-" + efpre.Attributes["name"].ToString() + " efpost-" + efpost.Attributes["name"].ToString());
                else if (!efpre.Attributes.Contains("name") && efpost.Attributes.Contains("name"))
                    writelog("efpre-No viene efpost-" + efpost.Attributes["name"].ToString());
                else if (efpre.Attributes.Contains("name") && !efpost.Attributes.Contains("name"))
                    writelog("efpre-" + efpre.Attributes["name"].ToString() + " efpost-No viene");
                else
                    writelog("efpre-No viene efpost-No viene");

                if (!((efpre.Attributes.Contains("name") && efpost.Attributes.Contains("name")
                        && efpre.Attributes["name"].ToString() != efpost.Attributes["name"].ToString()) ||
                        (efpre.Attributes.Contains("name") && !efpost.Attributes.Contains("name")) ||
                        (!efpre.Attributes.Contains("name") && efpost.Attributes.Contains("name"))))
                {
                    localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "No se ha modificado ninguno de los campos que provocan la creación de un registro para el histórico"));
                    writelog(string.Format(CultureInfo.InvariantCulture, "No se ha modificado ninguno de los campos que provocan la creación de un registro para el histórico"));
                    //throw new Exception("Campo == null sin cambios ");
                    return;
                }
            }
            else
            {
                //localcontext.Trace("9.2");
                bool hayCambio = false;
                for (int i = 0; i < campos.Length; i++)
                {
                    //if (efpost.Attributes.Contains(campos[i]))
                    writelog("Comprobando si hay cambio para el campo [" + campos[i] + "]");
                    if (efpre.Attributes.Contains(campos[i]) && efpost.Attributes.Contains(campos[i]))
                        writelog("efpre-" + efpre.Attributes[campos[i]].ToString() + " efpost-" + efpost.Attributes[campos[i]].ToString());
                    else if (!efpre.Attributes.Contains(campos[i]) && efpost.Attributes.Contains(campos[i]))
                        writelog("efpre-No viene efpost-" + efpost.Attributes[campos[i]].ToString());
                    else if (efpre.Attributes.Contains(campos[i]) && !efpost.Attributes.Contains(campos[i]))
                        writelog("efpre-" + efpre.Attributes[campos[i]].ToString() + " efpost-No viene");
                    else
                        writelog("efpre-No viene efpost-No viene");

                    if ((efpre.Attributes.Contains(campos[i]) && efpost.Attributes.Contains(campos[i])
                        && efpre.Attributes[campos[i]].ToString() != efpost.Attributes[campos[i]].ToString()) ||
                        (efpre.Attributes.Contains(campos[i]) && !efpost.Attributes.Contains(campos[i])) ||
                        (!efpre.Attributes.Contains(campos[i]) && efpost.Attributes.Contains(campos[i])))
                    {
                        localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "Se ha modificado el campo " + campos[i]));
                        writelog("Se ha modificado el campo " + campos[i]);
                        hayCambio = true;
                        break;
                    }
                }
                if (hayCambio == true)
                    writelog("hayCambio está a true");
                if (hayCambio == false)
                {
                    localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "No se ha modificado ninguno de los campos que provocan la creación de un registro para el histórico"));
                    writelog(string.Format(CultureInfo.InvariantCulture, "No se ha modificado ninguno de los campos que provocan la creación de un registro para el histórico"));
                    //throw new Exception("Campo != null sin cambios ");
                    return;
                }
            }
            localcontext.Trace("clonaRazonSocial 2");
            writelog("clonaRazonSocial 2");

            if (!tieneContratos(efpost.Id, "atos_razonsocialid", localcontext.OrganizationService))
            {
                localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "La razón social no tiene contratos por tanto no se crea registro para el histórico"));
                writelog(string.Format(CultureInfo.InvariantCulture, "La razón social no tiene contratos por tanto no se crea registro para el histórico"));
                //throw new Exception("No tiene contratos ");
                return;
            }
            localcontext.Trace("clonaRazonSocial 3");
            writelog("clonaRazonSocial 3");
            Entity historico = copiaAtributos(efpre, localcontext);

            historico.Attributes["atos_rsoriginalid"] = new EntityReference("account", efpost.Id);
            historico.Attributes["atos_historico"] = true;
            if (efpost.Attributes.Contains("atos_fechainiciovigencia") == true)
                historico.Attributes["atos_fechafinvigencia"] = ((DateTime)efpost.Attributes["atos_fechainiciovigencia"]).AddDays(-1);
            else
                historico.Attributes["atos_fechafinvigencia"] = ((DateTime)efpre.Attributes["atos_fechainiciovigencia"]).AddDays(-1);

            localcontext.Trace("clonaRazonSocial 5.");
            writelog("clonaRazonSocial 5");
            Guid histGuid = localcontext.OrganizationService.Create(historico);
            localcontext.Trace("clonaRazonSocial 6");
            writelog("clonaRazonSocial 6");
            DesactivaRegistro("account", histGuid, localcontext.OrganizationService);
            localcontext.Trace("clonaRazonSocial 6");
            writelog("clonaRazonSocial 7");
            //throw new Exception("Clona razón social ");
        }

        /**
        // <summary>
        // Punto de entrada del plugin.
        // </summary>
        // <param name="serviceProvider">The service provider.</param>
        // <remarks>
        // Se ejecuta en la modificación de una instalación. 
        // Busca en atos_secuencia la definición de la secuencia (asociada a la entidad del registro que se está creando). 
        // Con ese registro construye el valor de la secuencia.
        // </remarks>
         */
        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            // Construct the Local plug-in context.
            LocalPluginContext localcontext = new LocalPluginContext(serviceProvider);

            localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "Entrando a Execute()"));
            writelog(string.Format(CultureInfo.InvariantCulture, DateTime.Now.ToLocalTime().ToString() + " - Entrando a Execute()"));


            try
            {
                localcontext.Trace("1");
                if (localcontext.PluginExecutionContext.MessageName != "Update")
                    return;

                localcontext.Trace("2");
                if (localcontext.PluginExecutionContext.PreEntityImages["PreEntityImage"] == null)
                {
                    localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "No viene información de la entidad antes de los cambios"));
                    writelog(string.Format(CultureInfo.InvariantCulture, "No viene información de la entidad antes de los cambios"));
                    return;
                }
                localcontext.Trace("5");
                if (localcontext.PluginExecutionContext.PostEntityImages["PostEntityImage"] == null)
                {
                    localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "No viene información de la entidad después de los cambios"));
                    writelog(string.Format(CultureInfo.InvariantCulture, "No viene información de la entidad después de los cambios"));
                    return;
                }
                localcontext.Trace("6");

                Entity efpre = (Entity)localcontext.PluginExecutionContext.PreEntityImages["PreEntityImage"];

                if (efpre.Attributes.Contains("atos_historico"))
                {
                    if ((Boolean)efpre.Attributes["atos_historico"] == true)
                    {
                        String nombre = "";
                        if (efpre.Attributes.Contains("atos_name"))
                            nombre = efpre.Attributes["atos_name"].ToString();
                        localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "Modificación de un registro histórico. No se trata."));
                        writelog(string.Format(CultureInfo.InvariantCulture, "Modificación de un registro histórico. No se trata. [" + nombre + "]"));
                        return;
                    }
                }

                Entity efpost = (Entity)localcontext.PluginExecutionContext.PostEntityImages["PostEntityImage"];

                if (efpre.LogicalName == "atos_instalacion" && efpost.LogicalName == "atos_instalacion")
                {
                    if (efpre.Attributes.Contains("atos_instalacioncreadaenems") &&
                        (Boolean)efpre.Attributes["atos_instalacioncreadaenems"] == true) // Solo se crea en el histórico si la instalación se ha creado en EMS
                        clonaInstalacion(efpre, efpost, localcontext);
                }
                else if (efpre.LogicalName == "account" && efpost.LogicalName == "account")
                {
                    if (efpre.Attributes.Contains("atos_clientecreadoenems") &&
                        (Boolean)efpre.Attributes["atos_clientecreadoenems"] == true) // Solo se crea en el histórico si el cliente se ha creado en EMS
                        clonaRazonSocial(efpre, efpost, localcontext);
                }
                else
                {
                    localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "Este plugin solo se debe ejecutar para atos_instalacion o account. Ha entrando para la PreEntityImags {0} y PostEntityImage {1}", efpre.LogicalName, efpost.LogicalName));
                    writelog(string.Format(CultureInfo.InvariantCulture, "Este plugin solo se debe ejecutar para atos_instalacion o account. Ha entrando para la PreEntityImags {0} y PostEntityImage {1}", efpre.LogicalName, efpost.LogicalName));
                    return;
                }

                localcontext.Trace("8");
                
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "Exception: {0}", e.ToString()));
                writelog(string.Format(CultureInfo.InvariantCulture, "Exception: {0}", e.ToString()));
                // Handle the exception.
                throw;
            }
            finally
            {
                localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "Finalizando Execute()"));
                writelog(string.Format(CultureInfo.InvariantCulture, "Finalizando Execute()"));
            }
        }
    }
}