using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using System.Net;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

namespace ActTiemposCaso
{
    class Program
    {

        private static IOrganizationService _servicioCRM;


        private static String _servidor = "CRMACCIONA01";
        private static String _organizacion = "Acciona2015";
        private static String _dominio = "crmenergy";
        private static String _usuario = "desa";
        private static String _pass = "awg.1234";


        static int Main(string[] args)
        {
            try
            {
                conectarCRM();

                Entity caso = getCasoPorCodigo("CAS-00071-F2T3V5");

                Entity casoModificado = getCasoPorCodigo("CAS-00071-F2T3V5");

                // estado
                int estado = 300000006;
                if (caso.Attributes.Contains("statuscode") && ((Microsoft.Xrm.Sdk.OptionSetValue)(caso.Attributes["statuscode"])).Value == estado)
                {
                    estado = 300000007;
                }

                  //fecha
                  DateTime fechaCambio = DateTime.Now;
                  if (caso.Attributes.Contains("atos_fechadecambioestado"))
                  {
                      fechaCambio = ((DateTime)caso.Attributes["atos_fechadecambioestado"]).AddDays(0);
                  }




                  if (caso.Attributes.Contains("statuscode"))
                {
                    ((Microsoft.Xrm.Sdk.OptionSetValue)(caso.Attributes["statuscode"])).Value = estado;
                }
                else
                {
                    caso.Attributes.Add("statuscode", estado);
                }

                  if (caso.Attributes.Contains("atos_fechadecambioestado"))
                {
                    caso.Attributes["atos_fechadecambioestado"] = DateTime.Parse(fechaCambio.ToShortDateString());
                }
                else
                {
                    caso.Attributes.Add("atos_fechadecambioestado", DateTime.Parse(fechaCambio.ToShortDateString()));
                }


                Configuracion _conf = new Configuracion("");
                ActCaso actoCaso = new ActCaso( casoModificado, "Update");
               caso =  actoCaso.ActualizarCaso(ref caso, _servicioCRM, _conf);

                _servicioCRM.Update(caso);

                }
            catch (Exception ex)
            {
                return -1;
            }

            return 0;
        }

        private static void conectarCRM()
        {
            try
            {
                Uri organizationUri = new Uri(String.Format("http://{0}/{1}/XrmServices/2011/Organization.svc", _servidor, _organizacion));

                NetworkCredential nc = new NetworkCredential
                {
                    Domain = _dominio,
                    UserName = _usuario,
                    Password = _pass
                };

                ClientCredentials cc = new ClientCredentials();
                cc.Windows.ClientCredential = nc;

                OrganizationServiceProxy orgProxy = new OrganizationServiceProxy(organizationUri, null, cc, null);
                _servicioCRM = (IOrganizationService)orgProxy;

            }
             catch (Exception ex)
            {
                throw new Exception(String.Format("Error en el acceso a CRM: {0}", ex.Message));
            }
        }


        private static Entity getCasoPorCodigo(string codigo)
        {
            EntityCollection casos = new EntityCollection();


            QueryExpression query = new QueryExpression()
            {
                Distinct = false,
                EntityName = "incident",
                ColumnSet = new ColumnSet("atos_distribuidoraid", "atos_instalacionid", "atos_razonsocialid", "atos_tienporesolucion", "atos_fechaincidencia", "atos_fechacierre", "statuscode", "atos_fechadecambioestado", "atos_tiempocliente", "atos_tiempocomercializadora", "atos_tiempodistribuidora", "atos_tiempootros"),
                Criteria =
                {
                    Filters = 
                            {
                                new FilterExpression
                                {
                                    FilterOperator = LogicalOperator.And,
                                    Conditions = 
                                    {
                                        new ConditionExpression("ticketnumber", ConditionOperator.Equal,codigo),
                                    },
                                }
                            }
                }
            };


            casos = _servicioCRM.RetrieveMultiple(query);


            return casos.Entities.FirstOrDefault(); ;

        }



    }
}
