/*
 File="PropagarFechasOfertaMP : IPlugin" 
 Copyright (c) Atos. All rights reserved.

 Plugin que se ejecuta cuando se crea un nuevo registro en atos_trigger con 
 los valores OfertaMP para accion y account para la entidad.

 Fecha 		Codigo  Version Descripcion                                     Autor
 05.09.2022 23866   no-lock Incorporacion del No-lock a Consultas           AC
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;

namespace OfertaMP
{
    public class PropagarFechasOfertaMP : IPlugin   
    {
        private ITracingService tracingService;
        private IPluginExecutionContext PluginExecutionContext; // Contexto de ejecución del plugin
        private IOrganizationServiceFactory factory;
        private IOrganizationService service;

        // Tipos de Ofertas
        const int MULTIPUNTO = 300000000;
        const int SUBOFERTA = 300000001;
        const int OFERTA = 300000002;

        // Commodity 
        const int POWER = 300000000;
        const int GAS = 300000001;

        public void Execute(IServiceProvider serviceProvider)
        {
            String errMsg = String.Empty;
            string ofertaNombre = "";
           
            tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            PluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the Organization Service factory service from the service provider
            factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            // Use the factory to generate the Organization Service.
            service = factory.CreateOrganizationService(PluginExecutionContext.UserId);

            // AC tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Entered {0}.Execute()", this.GetType().ToString()));

            //if (PluginExecutionContext.MessageName != "Update")
            //    return;

            if (PluginExecutionContext.InputParameters.Contains("Target") &&
                PluginExecutionContext.InputParameters["Target"] is Entity)
            {
                try
                {
                    Entity ofertaModificada = (Entity)PluginExecutionContext.InputParameters["Target"];

                    Entity ofertaModificadaTotal = service.Retrieve("atos_oferta", ofertaModificada.Id, new ColumnSet(true));

                    ofertaNombre = ofertaModificadaTotal.Attributes["atos_name"].ToString();

                    string msg = ofertaModificada.Attributes.Contains("atos_name") ? ofertaModificada.Attributes["atos_name"].ToString() : string.Empty;
                    //Log.writelog(string.Format("\t\tEliminando el Pricing Output: {0} - {1} {2}", msg), true);
                    tracingService.Trace(string.Format("Oferta: {0}", msg));


                    //tracingService.Trace(String.Format("{0}:{1}",PluginExecutionContext.PreEntityImages.Keys.Count.ToString()
                    //    ,PluginExecutionContext.PreEntityImages.Keys.ElementAt(0)));

                    //tracingService.Trace(String.Format("{0}:{1}",PluginExecutionContext.PostEntityImages.Keys.Count.ToString()
                    //    ,PluginExecutionContext.PostEntityImages.Keys.ElementAt(0)));

                    Entity postImagen = (Entity)PluginExecutionContext.PostEntityImages["OfertaImage"];

                    Int32 tipoOferta = postImagen.Contains("atos_tipooferta") ? ((OptionSetValue)postImagen.Attributes["atos_tipooferta"]).Value : 0;

                    ////ha de ser una oferta multipunto o suboferta para modificar las hijas.
                    if (tipoOferta == 300000002)
                    {
                        tracingService.Trace("No procesa la Oferta, Ok");
                        return;
                    }

                    //Comprobar si hay información que propagar.
                    if (!(ofertaModificada.Contains("atos_nombreoferta") || ofertaModificada.Contains("atos_fechalimitepresentacionoferta") ||
                        ofertaModificada.Contains("atos_fechainicio") || ofertaModificada.Contains("atos_duracionmeses") ||
                            ofertaModificada.Contains("atos_fechafin")   || ofertaModificada.Contains("atos_fechafinvigenciaoferta") ||
                            ofertaModificada.Contains("atos_impuestoelectrico") || ofertaModificada.Contains("atos_congelacionpreciosente") ||
                            ofertaModificada.Contains("atos_congelacionpreciosentp") || ofertaModificada.Contains("atos_renovaciontacita") ||
                            ofertaModificada.Contains("atos_gestionatr") || 
                            ofertaModificada.Contains("atos_fechaaceptacionoferta") ||  ofertaModificada.Contains("atos_fecharechazodeoferta") ||
                            ofertaModificada.Contains("statuscode") || ofertaModificada.Contains("statecode") || ofertaModificada.Contains("atos_motivorechazoofertaid")))
                    {
                        tracingService.Trace("No hay informacion a propagar hacia las ofertas hijas, Ok");
                        return;
                    }

                    tracingService.Trace(String.Format("Se ha modificado atos_nombreoferta:{0}, tipoOferta:{1}, fecha ini suministro:{2}, fecha fin suministro:{3}, fecha fin vigencia:{4}, , fecha limite presentacion oferta:{5}",
                        ofertaModificada.Contains("atos_nombreoferta"), tipoOferta.ToString(), ofertaModificada.Contains("atos_fechainicio"),
                        ofertaModificada.Contains("atos_fechafin"), ofertaModificada.Contains("atos_fechafinvigenciaoferta"), ofertaModificada.Contains("atos_fechalimitepresentacionoferta")));

                    EntityCollection ofertasHijas = obtenerSubOfertas(ofertaModificada.Id);

                    tracingService.Trace(String.Format("Numero de Ofertas a modificar:{0}", ofertasHijas.Entities != null ? ofertasHijas.Entities.Count.ToString() : "0"));

                    int contOferta = 1;
                    foreach (Entity oferta in ofertasHijas.Entities)
                    {
                        Entity ofertaMod = new Entity("atos_oferta");
                        ofertaMod.Id = oferta.Id;

                        if (oferta.Contains("atos_tipooferta") && ((OptionSetValue)oferta.Attributes["atos_tipooferta"]).Value==300000001)
                        {
                            if (ofertaModificada.Contains("atos_nombreoferta"))
                            {
                                String nombreOferta = componerNombreSubOferta(ofertaModificada.Attributes["atos_nombreoferta"].ToString(), oferta);
                                ofertaMod.Attributes.Add("atos_nombreoferta", nombreOferta);
                                //if (oferta.Contains("atos_nombreoferta"))
                                //    oferta.Attributes["atos_nombreoferta"] = nombreOferta;
                                //else
                                //    oferta.Attributes.Add("atos_nombreoferta", nombreOferta);
                            }
                        }
                        else if (oferta.Contains("atos_tipooferta") && ((OptionSetValue)oferta.Attributes["atos_tipooferta"]).Value==300000002)
                        {
                            String nombreOferta = obtenerNombreOfertaMP(ofertaModificada.Id);
                            String nombreInstalacion = oferta.Contains("atos_instalacionid") ? ((EntityReference)oferta.Attributes["atos_instalacionid"]).Name : String.Empty;

                            if (String.IsNullOrEmpty(nombreOferta))
                            {
                                ofertaMod.Attributes.Add("atos_nombreoferta", nombreInstalacion);
                            }
                            else
                            {
                                ofertaMod.Attributes.Add("atos_nombreoferta", String.Format("{0}-{1}", nombreOferta, nombreInstalacion));
                            }

                        }
                        if (ofertaModificada.Contains("atos_duracionmeses"))
                        {
                            ofertaMod.Attributes.Add("atos_duracionmeses", ofertaModificada.Attributes["atos_duracionmeses"]);
                        }
                        if (ofertaModificada.Contains("atos_fechafinvigenciaoferta"))
                        {
                            ofertaMod.Attributes.Add("atos_fechafinvigenciaoferta", ofertaModificada.Attributes["atos_fechafinvigenciaoferta"]);
                        }
                        if (ofertaModificada.Contains("atos_fechalimitepresentacionoferta"))
                        {
                            ofertaMod.Attributes.Add("atos_fechalimitepresentacionoferta", ofertaModificada.Attributes["atos_fechalimitepresentacionoferta"]);
                        }
                        if (ofertaModificada.Contains("atos_fechainicio"))
                        {
                            ofertaMod.Attributes.Add("atos_fechainicio", ofertaModificada.Attributes["atos_fechainicio"]);
                        }

                        if (ofertaModificada.Contains("atos_fechafin"))
                        {
                            ofertaMod.Attributes.Add("atos_fechafin", ofertaModificada.Attributes["atos_fechafin"]);
                        }

                        if (ofertaModificada.Contains("atos_fechaaceptacionoferta"))
                        {
                            ofertaMod.Attributes.Add("atos_fechaaceptacionoferta", ofertaModificada.Attributes["atos_fechaaceptacionoferta"]);
                        }
                        if (ofertaModificada.Contains("atos_fecharechazodeoferta"))
                        {
                            ofertaMod.Attributes.Add("atos_fecharechazodeoferta", ofertaModificada.Attributes["atos_fecharechazodeoferta"]);
                        }
                        if (ofertaModificada.Contains("atos_motivorechazoofertaid"))
                        {
                            ofertaMod.Attributes.Add("atos_motivorechazoofertaid", new EntityReference("atos_motivorechazooferta", ((EntityReference)ofertaModificada.Attributes["atos_motivorechazoofertaid"]).Id));
                        }
                        if (ofertaModificada.Contains("atos_impuestoelectrico"))
                        {
                            ofertaMod.Attributes.Add("atos_impuestoelectrico", ofertaModificada.Attributes["atos_impuestoelectrico"]);
                        }
                        if (ofertaModificada.Contains("atos_congelacionpreciosentp"))
                        {
                            ofertaMod.Attributes.Add("atos_congelacionpreciosentp", ofertaModificada.Attributes["atos_congelacionpreciosentp"]);
                        }
                        if (ofertaModificada.Contains("atos_congelacionpreciosente"))
                        {
                            ofertaMod.Attributes.Add("atos_congelacionpreciosente", ofertaModificada.Attributes["atos_congelacionpreciosente"]);
                        }
                        if (ofertaModificada.Contains("atos_renovaciontacita"))
                        {
                            ofertaMod.Attributes.Add("atos_renovaciontacita", ofertaModificada.Attributes["atos_renovaciontacita"]);
                        }
                        if (ofertaModificada.Contains("atos_gestionatr"))
                        {
                            ofertaMod.Attributes.Add("atos_gestionatr", ofertaModificada.Attributes["atos_gestionatr"]);
                        }
                        if (ofertaModificada.Contains("atos_numclicsmensual"))
                        {
                            ofertaMod.Attributes.Add("atos_numclicsmensual", ofertaModificada.Attributes["atos_numclicsmensual"]);
                        }
                        if (ofertaModificada.Contains("atos_numclicstrimestral"))
                        {
                            ofertaMod.Attributes.Add("atos_numclicstrimestral", ofertaModificada.Attributes["atos_numclicstrimestral"]);
                        }
                        if (ofertaModificada.Contains("atos_numclicsanual"))
                        {
                            ofertaMod.Attributes.Add("atos_numclicsanual", ofertaModificada.Attributes["atos_numclicsanual"]);
                        }
                        if (ofertaModificada.Contains("statecode") || ofertaModificada.Contains("statuscode"))
                        {

                            //StateCode = 1 y StatusCode = 2 para desactivar 
                            SetStateRequest setStateRequest = new SetStateRequest()
                            {
                                EntityMoniker = new EntityReference
                                {
                                    Id = ofertaMod.Id,
                                    LogicalName = "atos_oferta",
                                },
                                State = new OptionSetValue(((OptionSetValue)(ofertaModificadaTotal.Attributes["statecode"])).Value),
                                Status = new OptionSetValue(((OptionSetValue)(ofertaModificadaTotal.Attributes["statuscode"])).Value)
                            };
                            service.Execute(setStateRequest);
                        }

                        service.Update(ofertaMod);

                        tracingService.Trace(String.Format("Modificada oferta {0}",contOferta.ToString()));
                    }
                    contOferta++;
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
                
                tracingService.Trace(String.Format("Errores reportados:{0}",errMsg));

                if (!String.IsNullOrEmpty(errMsg))
                {
                    throw new InvalidPluginExecutionException("Se ha producido un error en la propagación de información de ofertas MP a ofertas hijas y subofertas, OFERTA-->" + ofertaNombre +  "---->EXCEPCION-->" + errMsg);
                }
            }
        }

        private string obtenerNombreOfertaMP(Guid pOfertaId)
        {
            String salida = String.Empty;
            try
            {
                QueryExpression consulta = new QueryExpression("atos_oferta");
                /* 23866 1+ */
                consulta.NoLock = true;
                consulta.Criteria.AddCondition("atos_ofertaid", ConditionOperator.Equal, pOfertaId);
                LinkEntity join = new LinkEntity("atos_oferta","atos_oferta", "atos_ofertapadreid", "atos_ofertaid",JoinOperator.Natural);
                join.Columns.AddColumn("atos_nombreoferta");
                join.EntityAlias = "ofertaMP";
                consulta.LinkEntities.Add(join);

                EntityCollection resConsulta = service.RetrieveMultiple(consulta);

                if (resConsulta.Entities.Count > 0)
                {
                    //tracingService.Trace(String.Format("DEvuelve datos:{0} y atributos {1}", resConsulta.Entities.Count.ToString(), resConsulta.Entities[0].Attributes.Count.ToString()));
                    //for (int numAtr = 0; numAtr < resConsulta.Entities[0].Attributes.Count; numAtr++ )
                    //{
                    //    tracingService.Trace(resConsulta.Entities[0].Attributes.ElementAt(numAtr).Key + Environment.NewLine);
                    //}
                    if (resConsulta[0].Attributes.Contains("ofertaMP.atos_nombreoferta"))
                    {
                        salida = ((AliasedValue)resConsulta[0].Attributes["ofertaMP.atos_nombreoferta"]).Value.ToString();
                    }
                }
            }
            //catch (SoapException soapEx)
            //catch (System.Web.Services.Protocols.SoapException soapEx)
            //{
            //    throw new Exception(String.Format("Error en la obtención de oferta MP:{0}", soapEx.Detail.InnerText));
            //}
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error en la obtención de oferta MP:{0}", ex.Message));
            }
            return salida;
        }
        /// <summary>
        /// función que devuelve el nombre que tomará la oferta en función de su oferta padre.
        /// </summary>
        /// <param name="pNombreOfertaPadre"></param>
        /// <param name="pOferta"></param>
        /// <returns></returns>
        private string componerNombreSubOferta(String pNombreOfertaPadre, Entity pOferta)
        {
            String salida = String.Empty;
            String tarifa = String.Empty;
            String sistemaElectrico = String.Empty;
            String lote = String.Empty;
            String peaje = String.Empty;

            try
            {
                if (pOferta.Contains("atos_tarifaid"))
                    tarifa = ((EntityReference)pOferta.Attributes["atos_tarifaid"]).Name;

                if (pOferta.Contains("atos_sistemaelectricoid"))
                    sistemaElectrico = ((EntityReference)pOferta.Attributes["atos_sistemaelectricoid"]).Name;

                if (pOferta.Contains("atos_lote"))
                    lote = pOferta.Attributes["atos_lote"].ToString();

                if (pOferta.Contains("atos_peajeid"))
                {
                    peaje = ((EntityReference)pOferta.Attributes["atos_peajeid"]).Name;
                }

                StringBuilder nombreOferta = new StringBuilder();
                if (!String.IsNullOrEmpty(pNombreOfertaPadre))
                    nombreOferta.AppendFormat("{0}-", pNombreOfertaPadre);
                
                if(!String.IsNullOrEmpty(peaje))
                    nombreOferta.AppendFormat("{0}-", peaje);

                if (!String.IsNullOrEmpty(tarifa))
                    nombreOferta.AppendFormat("{0}-", tarifa);

                if (!String.IsNullOrEmpty(sistemaElectrico))
                    nombreOferta.Append(sistemaElectrico);

                if (!String.IsNullOrEmpty(lote))
                    nombreOferta.AppendFormat("-{0}", lote);

                salida = nombreOferta.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("componerNombreSuboferta:{0}", ex.Message));
            }
            return salida;
        }

        /// <summary>
        /// Devuelve todas las subofertas de las que es padre la oferta que se pasa por parámetro.
        /// </summary>
        /// <param name="pOfertaPadre">Id de la oferta de la que se buscan las hijas</param>
        /// <returns></returns>
        private EntityCollection obtenerSubOfertas(Guid pOfertaPadre)
        {
            EntityCollection salida = new EntityCollection();
            try
            {
                QueryByAttribute consulta = new QueryByAttribute("atos_oferta");

                consulta.ColumnSet = new ColumnSet("atos_ofertaid", "atos_instalacionid", "atos_lote", "atos_nombreoferta", "atos_sistemaelectricoid", "atos_tarifaid",
                    "atos_tipooferta", "atos_peajeid");
                consulta.Attributes.AddRange("atos_ofertapadreid");
                consulta.Values.AddRange(pOfertaPadre);

                EntityCollection resConsulta = service.RetrieveMultiple(consulta);

                if (resConsulta.Entities.Count > 0)
                {
                    salida = resConsulta;
                }
            }
            //catch (SoapException soapEx)
            //catch (System.Web.Services.Protocols.SoapException soapEx)
            //{
            //    throw new Exception(String.Format("Error en la consulta de Ofertas:{0}", soapEx.Detail.InnerText));
            //}
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error en la consulta de Ofertas:{0}", ex.Message));

            }
            return salida;
        }


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

        private void ActivaRegistro(string _entidad, Guid _guid, IOrganizationService _organizationService)
        {
            var cols = new ColumnSet(new[] { "statecode", "statuscode" });

            //Comprobamos si está activo o no
            var entity = _organizationService.Retrieve(_entidad, _guid, cols);

            if (entity != null && entity.GetAttributeValue<OptionSetValue>("statecode").Value == 1)
            {
                //StateCode = 1 y StatusCode = 2 para desactivar 
                SetStateRequest setStateRequest = new SetStateRequest()
                {
                    EntityMoniker = new EntityReference
                    {
                        Id = _guid,
                        LogicalName = _entidad,
                    },
                    State = new OptionSetValue(0),
                    Status = new OptionSetValue(1)
                };
                _organizationService.Execute(setStateRequest);
            }
        }


    }
}
