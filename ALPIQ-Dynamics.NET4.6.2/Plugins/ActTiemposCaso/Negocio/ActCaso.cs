using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Collections;


namespace ActTiemposCaso
{
    public class ActCaso
    {

        public Entity PreUpdateCaso;
        public string TipoOperacion;


        private TipoCliente tipoCliente = TipoCliente.Ninguno ;

        enum TipoCliente
        {
            Ninguno,
            Distribuidora,
            RazonSocial,
            Instalacion

        }


        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="caso">entidad caso que se va a tratar</param>
        /// <param name="preCaso"> estado anterior de la entidad caso si es que existe</param>
        /// <param name="estado">si es una actualización o una creación de caso</param>
        public ActCaso( Entity preUpdateCaso, string tipoOperacion)
        {
            PreUpdateCaso = preUpdateCaso;
            TipoOperacion = tipoOperacion; 
        }

        #region "METODOS PUBLICOS"

        public Entity ActualizarCaso(ref Entity Caso, IOrganizationService service, Configuracion _conf)
        {

             Entity cliente =null;

             Guid paisClienteId =Guid.Empty;
             Guid ccaaClienteId =Guid.Empty;
             Guid provinciaClienteId =Guid.Empty;
             Guid municipioClienteId =Guid.Empty;



             Entity casoTmp = getCasoPorCodigo((Guid)(Caso.Attributes["incidentid"]), service); 

             
             // obtenemos cual es el cliente (distribuidora,instalacion,razon social   
             if (casoTmp.Attributes.Contains("atos_distribuidoraid"))
             {
                 cliente = service.Retrieve("atos_distribuidora", ((EntityReference)casoTmp.Attributes["atos_distribuidoraid"]).Id, new ColumnSet(true));
                 tipoCliente =TipoCliente.Distribuidora;

                 if (cliente.Attributes.Contains("atos_paisid"))
                     paisClienteId = ((Microsoft.Xrm.Sdk.EntityReference)(cliente.Attributes["atos_paisid"])).Id;

                 if (cliente.Attributes.Contains("atos_comunidadautonomaid"))
                     ccaaClienteId = ((Microsoft.Xrm.Sdk.EntityReference)(cliente.Attributes["atos_comunidadautonomaid"])).Id;

                 if (cliente.Attributes.Contains("atos_provinciaid"))
                     provinciaClienteId = ((Microsoft.Xrm.Sdk.EntityReference)(cliente.Attributes["atos_provinciaid"])).Id;

                 if (cliente.Attributes.Contains("atos_municipioid"))
                     municipioClienteId = ((Microsoft.Xrm.Sdk.EntityReference)(cliente.Attributes["atos_municipioid"])).Id;


             }
             else if (casoTmp.Attributes.Contains("atos_instalacionid"))
             {
                 cliente = service.Retrieve("atos_instalacion", ((EntityReference)casoTmp.Attributes["atos_instalacionid"]).Id, new ColumnSet(true));
                 tipoCliente =TipoCliente.Instalacion;


                 if (cliente.Attributes.Contains("atos_instalacionpaisid"))
                    paisClienteId = ((Microsoft.Xrm.Sdk.EntityReference)(cliente.Attributes["atos_instalacionpaisid"])).Id;

                 if (cliente.Attributes.Contains("atos_instalacionccaaid"))
                     ccaaClienteId = ((Microsoft.Xrm.Sdk.EntityReference)(cliente.Attributes["atos_instalacionccaaid"])).Id;

                 if (cliente.Attributes.Contains("atos_instalacionprovinciaid"))
                     provinciaClienteId = ((Microsoft.Xrm.Sdk.EntityReference)(cliente.Attributes["atos_instalacionprovinciaid"])).Id;

                 if (cliente.Attributes.Contains("atos_instalacionmunicipioid"))
                     municipioClienteId = ((Microsoft.Xrm.Sdk.EntityReference)(cliente.Attributes["atos_instalacionmunicipioid"])).Id;


             }
             else if (casoTmp.Attributes.Contains("atos_razonsocialid"))
             {
                 cliente = service.Retrieve("account", ((EntityReference)casoTmp.Attributes["atos_razonsocialid"]).Id, new ColumnSet(true));
                 tipoCliente =TipoCliente.RazonSocial ;

                 if (cliente.Attributes.Contains("atos_rspaisid"))
                     paisClienteId = ((Microsoft.Xrm.Sdk.EntityReference)(cliente.Attributes["atos_rspaisid"])).Id;

                 if (cliente.Attributes.Contains("atos_rsccaaid"))
                     ccaaClienteId = ((Microsoft.Xrm.Sdk.EntityReference)(cliente.Attributes["atos_rsccaaid"])).Id;

                 if (cliente.Attributes.Contains("atos_rsprovinciaid"))
                     provinciaClienteId = ((Microsoft.Xrm.Sdk.EntityReference)(cliente.Attributes["atos_rsprovinciaid"])).Id;

                 if (cliente.Attributes.Contains("atos_rsmunicipioid"))
                     municipioClienteId = ((Microsoft.Xrm.Sdk.EntityReference)(cliente.Attributes["atos_rsmunicipioid"])).Id;

             }
           
             DateTime? fechaInicioCaso= null;
             DateTime? fechaResolucionCaso = null;
             // si es update se actualizan los posibles tiempos por cambio de estado
             if (TipoOperacion == "Update" && PreUpdateCaso !=null )
             {

                 if (casoTmp.Attributes.Contains("atos_fechaincidencia"))
                 {
                     fechaInicioCaso = ((DateTime)casoTmp.Attributes["atos_fechaincidencia"]).ToLocalTime();
                 }

                 if (Caso.Attributes.Contains("atos_fecharesolucion"))
                 {
                     fechaResolucionCaso = ((DateTime)Caso.Attributes["atos_fecharesolucion"]).ToLocalTime();
                 }

                if (fechaInicioCaso!=null && fechaResolucionCaso!=null)
                {
                   decimal tiempoResolucion = diasEntreFechas(service ,(DateTime)fechaInicioCaso,(DateTime)fechaResolucionCaso,paisClienteId,ccaaClienteId,provinciaClienteId,municipioClienteId);
                    if (tiempoResolucion>=0)
                    {
                        if (Caso.Attributes.Contains("atos_tienporesolucion"))
                         {
                             Caso.Attributes.Remove("atos_tienporesolucion");
                             Caso.Attributes.Add("atos_tienporesolucion", tiempoResolucion);
                         }
                         else 
                         {
                             Caso.Attributes.Add("atos_tienporesolucion", tiempoResolucion);
                         }
                    }
             

                }
                int estadoInicial = 0; 
                int estadoFinal = 0; 
                if (PreUpdateCaso.Attributes.Contains("statuscode"))
                {
                    estadoInicial = ((Microsoft.Xrm.Sdk.OptionSetValue)(PreUpdateCaso.Attributes["statuscode"])).Value;
                    _conf.writelog("estadoInicial :" +((Microsoft.Xrm.Sdk.OptionSetValue)(PreUpdateCaso.Attributes["statuscode"])).Value);
                }
                if (Caso.Attributes.Contains("statuscode"))
                {
                    estadoFinal = ((Microsoft.Xrm.Sdk.OptionSetValue)(Caso.Attributes["statuscode"])).Value;
                    _conf.writelog("estadoFinal :" + ((Microsoft.Xrm.Sdk.OptionSetValue)(Caso.Attributes["statuscode"])).Value);
                }
                // resolvemos el cambio de estado 
                if (estadoInicial!=0 && estadoInicial!=estadoFinal)
                {
                   DateTime? fechaInicioCambioEstado =null;
                   DateTime? fechaFinalCambioEstado =null;
                    decimal diasCambioDeEstado=0;

                    if (PreUpdateCaso.Attributes.Contains("atos_fechadecambioestado"))
                    {
                        fechaInicioCambioEstado  = ((DateTime)PreUpdateCaso.Attributes["atos_fechadecambioestado"]).ToLocalTime() ;
                        _conf.writelog("fechaInicioCambioEstado :" + ((DateTime)fechaInicioCambioEstado).ToLocalTime().ToShortDateString());
                    }


                    if (Caso.Attributes.Contains("atos_fechadecambioestado"))
                    {
                        fechaFinalCambioEstado = ((DateTime)Caso.Attributes["atos_fechadecambioestado"]).ToLocalTime();
                        _conf.writelog("fechaFinalCambioEstado :" + ((DateTime)fechaFinalCambioEstado).ToLocalTime().ToShortDateString());
                    }
                    else
                    {

                        fechaFinalCambioEstado = ((DateTime)PreUpdateCaso.Attributes["atos_fechadecambioestado"]).ToLocalTime();
                        _conf.writelog("fechaFinalCambioEstado :" + ((DateTime)fechaFinalCambioEstado).ToLocalTime().ToShortDateString());
                    }
                   


                    if (fechaInicioCambioEstado != null )
                    {

                        switch (estadoInicial)
                        {
                            //pendiente de cliente
                            case 300000006:
                                diasCambioDeEstado = diasEntreFechas(service, (DateTime)fechaInicioCambioEstado, (DateTime)fechaFinalCambioEstado, paisClienteId, ccaaClienteId, provinciaClienteId, municipioClienteId);
                                Caso = añadirDias(Caso, "atos_tiempocliente", diasCambioDeEstado, casoTmp);
                                break;
                            //pendiente de comercializadora
                            case 300000007:

                                Guid paisComercializadoraId = Guid.Empty;
                                Guid ccaaComercializadoraId = Guid.Empty;
                                Guid provinciaComercializadoraId = Guid.Empty;
                                Guid municipioComercializadoraId = Guid.Empty;

                                parametrosComercializadora(service,ref paisComercializadoraId,ref ccaaComercializadoraId,ref provinciaComercializadoraId,ref municipioComercializadoraId);
                                //obtengo los datos de la comercialzadora

                                diasCambioDeEstado = diasEntreFechas(service, (DateTime)fechaInicioCambioEstado, (DateTime)fechaFinalCambioEstado, paisClienteId, ccaaClienteId, provinciaClienteId, municipioClienteId);
                                Caso = añadirDias(Caso, "atos_tiempocomercializadora", diasCambioDeEstado, casoTmp);
                                break;
                            // pediente de distribuidora
                            case 300000008:

                                Guid paisDistribuidoraId = Guid.Empty;
                                Guid ccaaDistribuidoraId = Guid.Empty;
                                Guid provinciaDistribuidoraId = Guid.Empty;
                                Guid municipioDistribuidoraId = Guid.Empty;

                                if (tipoCliente == TipoCliente.Instalacion)
                                {
                                    Entity distribuidora = service.Retrieve("atos_distribuidora", ((EntityReference)cliente.Attributes["atos_distribuidoraid"]).Id, new ColumnSet(true));
                                  
                                    if (distribuidora.Attributes.Contains("atos_paisid"))
                                        paisClienteId = ((Microsoft.Xrm.Sdk.EntityReference)(distribuidora.Attributes["atos_paisid"])).Id;

                                    if (distribuidora.Attributes.Contains("atos_comunidadautonomaid"))
                                        ccaaClienteId = ((Microsoft.Xrm.Sdk.EntityReference)(distribuidora.Attributes["atos_comunidadautonomaid"])).Id;

                                    if (distribuidora.Attributes.Contains("atos_provinciaid"))
                                        provinciaClienteId = ((Microsoft.Xrm.Sdk.EntityReference)(distribuidora.Attributes["atos_provinciaid"])).Id;

                                    if (distribuidora.Attributes.Contains("atos_municipioid"))
                                        municipioClienteId = ((Microsoft.Xrm.Sdk.EntityReference)(distribuidora.Attributes["atos_municipioid"])).Id;
                                }
                                diasCambioDeEstado = diasEntreFechas(service, (DateTime)fechaInicioCambioEstado, (DateTime)fechaFinalCambioEstado, paisDistribuidoraId, ccaaDistribuidoraId, provinciaDistribuidoraId, municipioDistribuidoraId);
                                Caso = añadirDias(Caso, "atos_tiempodistribuidora", diasCambioDeEstado ,casoTmp );
                                break;
                            //pendiente otro
                            case 300000009:
                                diasCambioDeEstado = diasEntreFechas(service, (DateTime)fechaInicioCambioEstado, (DateTime)fechaFinalCambioEstado, paisClienteId, Guid.Empty, Guid.Empty, Guid.Empty);
                                Caso = añadirDias(Caso, "atos_tiempootros", diasCambioDeEstado, casoTmp);
                                break;

                        }

                        _conf.writelog("diasCambioDeEstado :" + diasCambioDeEstado.ToString()); 
                    }
                }
            }

             return Caso ;
        }

        #endregion


        #region "METODOS PRIVADOS"


       private Entity añadirDias(  Entity entidad,string atributo,decimal dias,Entity  entidadTmp )
       {

           if (entidadTmp.Attributes.Contains(atributo))
            {
                entidad.Attributes[atributo] = decimal.Parse(entidadTmp.Attributes[atributo].ToString()) + dias;
            }
            else 
            {
              entidad.Attributes.Add(atributo,dias);
            }

           return entidad;
       }


        /// <summary>
        /// Calcula  la diferencia de dias entre dos fechas y elimina los dias festivos
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio </param>
        /// <param name="fechaFin">Fecha de fin </param>
        /// <param name="PaisId">GUID del pais para saber las fiestas nacionales</param>
        /// <param name="ComunidadId">GUID de la CC.AA, para saber las fiestas autonomicas</param>
        /// <param name="ProvinciaId">GUID de la provincia para saber las fiestas provinciales</param>
        /// <param name="MunicipioId">GUID del minicipio para saber las fiestas municipales</param>
        /// <returns></returns>
        private decimal diasEntreFechas(IOrganizationService service,DateTime fechaInicio, DateTime fechaFin, Guid PaisId, Guid ComunidadId, Guid ProvinciaId, Guid MunicipioId )
        {
            decimal dias = 0;

            fechaInicio = DateTime.Parse(fechaInicio.ToShortDateString());
            fechaFin = DateTime.Parse(fechaFin.ToShortDateString());

            dias = (decimal)Math.Floor(fechaFin.Subtract(fechaInicio).TotalDays);

            if(dias <0) 
            {
                throw new Exception ("La fecha de cambio de estado es menor que la fecha de cambio de estado anterior");
            }
            DateTime fechaInicioTmp = fechaInicio;
            while (fechaInicioTmp <= fechaFin)
            {

                if (fechaInicioTmp.DayOfWeek == DayOfWeek.Saturday || fechaInicioTmp.DayOfWeek == DayOfWeek.Sunday)
                    dias= dias-1;

                fechaInicioTmp = fechaInicioTmp.AddDays(1);
            }


            // obtenemos los dias festivos para restarlos 

             EntityCollection diasFestivos = new EntityCollection();
                QueryExpression query = new QueryExpression()
                {
                    Distinct = false,
                    EntityName = "atos_festivos",
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
                                        new ConditionExpression("atos_diafestivo", ConditionOperator.GreaterEqual ,  DateTime.Parse(fechaInicio.ToShortDateString()) ),
                                         new ConditionExpression("atos_diafestivo", ConditionOperator.LessEqual  , DateTime.Parse(fechaFin.ToShortDateString()))
                                    },
                                },
                                new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.Or,
                                    Conditions = 
                                    {
                                        new ConditionExpression("atos_municipioid", ConditionOperator.Equal, MunicipioId),
                                        new ConditionExpression("atos_provinciaid", ConditionOperator.Equal, ProvinciaId),
                                         new ConditionExpression("atos_comunidadautonomaid", ConditionOperator.Equal, ComunidadId),
                                          new ConditionExpression("atos_paisid", ConditionOperator.Equal, PaisId)
                                    }
                                }
                            }
                    }
                }; 

                diasFestivos = service.RetrieveMultiple(query);
               Hashtable diasTotales = new Hashtable();

                if (diasFestivos.Entities.Count > 0)
                {
                     
                    foreach (Entity diafestivo in diasFestivos.Entities)
                    {
                        if (diafestivo.Attributes.Contains("atos_diafestivo") && ((DateTime)diafestivo.Attributes["atos_diafestivo"]).DayOfWeek != DayOfWeek.Saturday && ((DateTime)diafestivo.Attributes["atos_diafestivo"]).DayOfWeek != DayOfWeek.Sunday )
                        {

                            if (!(MunicipioId == Guid.Empty && diafestivo.Attributes.Contains("atos_municipioid")) && 
                                !(ComunidadId == Guid.Empty && diafestivo.Attributes.Contains("atos_comunidadautonomaid")) &&
                                !(PaisId == Guid.Empty && diafestivo.Attributes.Contains("atos_paisid")))

                            if (!diasTotales.ContainsKey(diafestivo.Attributes.Contains("atos_diafestivo")))  
                                    diasTotales.Add (diafestivo.Attributes.Contains("atos_diafestivo"),diafestivo.Attributes.Contains("atos_diafestivo"));
                        }

                    }
                }


                if (dias - diasTotales.Count < 0)
                {
                    return 0;
                }


            return dias -diasTotales.Count ;
        }


        private bool parametrosComercializadora(IOrganizationService service, ref Guid paisId,ref Guid comunidadId,ref Guid provinciaId,ref Guid municipioId)
        {
            

            // obtenemos los identificadores de localizacion

            EntityCollection parametros = new EntityCollection();
            QueryExpression query = new QueryExpression()
            {
                Distinct = false,
                EntityName = "atos_parametroscomercializadora",
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
                                        new ConditionExpression("atos_name", ConditionOperator.Like ,  "Parámetros Comercializadora" )
                                         
                                    },
                                }
                            }
                }
            };

            parametros = service.RetrieveMultiple(query);
            Hashtable diasTotales = new Hashtable();

            if (parametros.Entities.Count > 0)
            {
                // obtenmos todos los valores
                foreach (Entity parametro in parametros.Entities)
                {
                  municipioId = ((Microsoft.Xrm.Sdk.EntityReference)(parametro.Attributes["atos_municipioid"])).Id;

                  Entity municipio = service.Retrieve("atos_municipio", ((EntityReference)parametro.Attributes["atos_municipioid"]).Id, new ColumnSet(true));

                  Entity provincia = service.Retrieve("atos_provincia", ((EntityReference)municipio.Attributes["atos_provinciaid"]).Id, new ColumnSet(true));
                  provinciaId = provincia.Id;

                  Entity ccaa = service.Retrieve("atos_comunidadautonoma", ((EntityReference)provincia.Attributes["atos_comunidadautonomaid"]).Id, new ColumnSet(true));
                  comunidadId = ccaa.Id;

                  Entity pais = service.Retrieve("atos_pais", ((EntityReference)ccaa.Attributes["atos_paisid"]).Id, new ColumnSet(true));
                  paisId = pais.Id;
                }
               

            }

            return true; 
        }



            #region "OBTENER ENTIDADES"
                private Entity obtenerInstalacion(Guid instalacionId)
                {
                    Entity instalacion = null;

                    return instalacion;
                }

                private Entity obtenerRazonSocial(Guid razonSocialId)
                {
                    Entity razonSocial = null;

                    return razonSocial;
                }

                private Entity obtenerDistribuidora(Guid distribuidoraId)
                {
                    Entity distribuidora = null;

                    return distribuidora;
                }
            #endregion


         private  Entity getCasoPorCodigo(Guid  incidentId,IOrganizationService service)
        {
            EntityCollection casos = new EntityCollection();


            QueryExpression query = new QueryExpression()
            {
                Distinct = false,
                EntityName = "incident",
                ColumnSet = new ColumnSet("atos_distribuidoraid", "atos_instalacionid","atos_razonsocialid", "atos_tienporesolucion", "atos_fechaincidencia", "atos_fecharesolucion", "statuscode", "atos_fechadecambioestado", "atos_tiempocliente", "atos_tiempocomercializadora", "atos_tiempodistribuidora", "atos_tiempootros"),
                Criteria =
                {
                    Filters = 
                            {
                                new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions = 
                                    {
                                        new ConditionExpression("incidentid", ConditionOperator.Equal,incidentId),
                                    },
                                }
                            }
                }
            };
            casos = service.RetrieveMultiple(query);
            return casos.Entities.FirstOrDefault(); ;
        }


        #endregion




    }

}
