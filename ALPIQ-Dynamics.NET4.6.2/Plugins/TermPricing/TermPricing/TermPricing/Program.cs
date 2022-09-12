
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;




namespace TermPricing
{
    static class Program
    {

        private static IOrganizationService orgService;
        private static CrmServiceClient conn;
        private static String _servidor = "CRMACCIONA01";
        private static String _organizacion = "Acciona2015";
        private static String _dominio = "crmenergy";
        private static String _usuario = "desa";
        private static String _pass = "awg.1234";
        static private String _rutaLog;
       



        // variables externas

       private static Boolean esGas;
       private static bool tipoCalculoPromedio = false;
       static List<FormulaBase> variables = null;
       static Formula formula = null;
       static private CommonWS.Log Log = new CommonWS.Log();
       static List<String> errores = new List<String>();



        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static int Main()
        {
            try
            {
                Conectar();
                Entity _oferta = getOfertaPorCodigo("S/S00017-2.5-20190125-1");

                // comprobacion paso 4

                if (_oferta.Attributes.Contains("atos_tipodecalculo"))
                {
                    if (((OptionSetValue)_oferta.Attributes["atos_tipodecalculo"]).Value == 300000000) // Tipo de calculo promedio
                        tipoCalculoPromedio = true;
                }


                // Pricing Gas
                if (esGas) // En gas el tipo de calculo siempre es directo
                {
                    tipoCalculoPromedio = false;
                }
                // Fin Pricing Gas

                Guid peajeGasId = Guid.Empty;
                if (esGas)
                {
                    if (_oferta.Attributes.Contains("atos_peajeid"))
                    {
                        peajeGasId = ((EntityReference)_oferta.Attributes["atos_peajeid"]).Id;
                    }
                }


                if (((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value == 300000000) // Si es multipunto actualiza stage de las hijas
                    //actualizaPasoHijas(efpre, _oferta, _ofertaPricing, localcontext, false, true);

                if ((esGas && peajeGasId == Guid.Empty) ||
                    (!esGas && (_oferta.Attributes.Contains("atos_tarifaid") == false ||
                                _oferta.Attributes.Contains("atos_sistemaelectricoid") == false ||
                                _oferta.Attributes.Contains("atos_subsistemaid") == false)))
                {
                   
                    return 0;
                }

                if (((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value == 300000002 && _oferta.Attributes.Contains("atos_ofertapadreid")) // Si es oferta hija no hace nada
                {
                    return 0;
                }

                // Pricing Gas
                Entity _tarifa;
                if (esGas)
                {
                  
                    _tarifa = orgService.Retrieve("atos_tablasatrgas",
                                                  peajeGasId,
                                                new ColumnSet(new[] { "atos_name" }));
                    _tarifa.Attributes["atos_numeroperiodos"] = (Decimal)1;
                }
                else
                {
                    _tarifa = orgService.Retrieve("atos_tarifa",
                                                                                ((EntityReference)_oferta.Attributes["atos_tarifaid"]).Id,
                                                                                new ColumnSet(new[] { "atos_name", "atos_numeroperiodos" }));
                }

                // Fin Pricing Gas
                Entity _tipoProducto = orgService.Retrieve("atos_tipodeproducto",
                                                                                ((EntityReference)_oferta.Attributes["atos_tipodeproductofinalid"]).Id,
                                                                                new ColumnSet(new[] {"atos_formula",    "atos_nombreems",
																					                 "atos_nombrevi1",  "atos_valorvi1",
																					                 "atos_nombrevi2",  "atos_valorvi2", 
																					                 "atos_nombrevi3",  "atos_valorvi3", 
																					                 "atos_nombrevi4",  "atos_valorvi4", 
																					                 "atos_nombrevi5",  "atos_valorvi5", 
																					                 "atos_nombrevi6",  "atos_valorvi6", 
																					                 "atos_nombrevi7",  "atos_valorvi7", 
																					                 "atos_nombrevi8",  "atos_valorvi8", 
																					                 "atos_nombrevi9",  "atos_valorvi9", 
																					                 "atos_nombrevi10", "atos_valorvi10" }));
                // Antes de calcular los pricing output borramos los que puedan existir.
                // deletepricingoutput(ref _oferta);
                // No debería hacer falta, solo necesario si se actualiza a stage 3 estando en stage 3, pero en ese caso no debería llegar hasta aquí.
              
                construyeFormulas(_tipoProducto);
               calcula(ref _oferta, _oferta, _tipoProducto, _tarifa);
               








            }
            catch (Exception ex)
            {
                return -1;
            }
            return 0;
        }


        public static void Conectar()
        {
            try
            {
                
                String connectionString = "AuthType=Office365;Username=admin@AlpiqEnergia.onmicrosoft.com; Password=Alpiq.2017.bis;Url=https://alpiqenergia.crm4.dynamics.com/";
                //String connectionString = "AuthType=Office365;Username=admin@AlpiqEnergia.onmicrosoft.com; Password=Alpiq.2017.bis;Url=https://alpiqeneregiapre.crm4.dynamics.com/";
                //String connectionString = "AuthType=Office365;Username=admin@AlpiqEnergiaTest.onmicrosoft.com; Password=Atos.2017.bis;Url=https://alpiqtestatos.crm4.dynamics.com/";

             

                //Conectamos al CRM con una cadena de conexión
                conn = new Microsoft.Xrm.Tooling.Connector.CrmServiceClient(connectionString);

                // Cast the proxy client to the IOrganizationService interface.
                orgService = (IOrganizationService)conn.OrganizationWebProxyClient != null ? (IOrganizationService)conn.OrganizationWebProxyClient : (IOrganizationService)conn.OrganizationServiceProxy;

                // Retrieve the version of Microsoft Dynamics CRM.
                RetrieveVersionRequest versionRequest = new RetrieveVersionRequest();
                RetrieveVersionResponse versionResponse = (RetrieveVersionResponse)orgService.Execute(versionRequest);
            }
            catch (Exception e)
            {
                throw new Exception("Error al intentar conectar con CRM:" + e.Message);
            }
        }

   
        private static Entity getOfertaPorCodigo(string codigo)
        {
            EntityCollection ofertas = new EntityCollection();


            QueryExpression query = new QueryExpression()
            {
                Distinct = false,
                EntityName = "atos_oferta",
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
                                        new ConditionExpression("atos_name", ConditionOperator.Equal,codigo),
                                    },
                                }
                            }
                }
            };


            ofertas = orgService.RetrieveMultiple(query);


            return ofertas.Entities.FirstOrDefault(); ;

        }



        private static void construyeFormulas(Entity _tipoProducto)
        {

            try
            {
                formula = Formula.creaFormula(_tipoProducto, tipoCalculoPromedio, Log);
                formula.esInstalacionGas = esGas;
                variables = formula.construyeFormulas(_tipoProducto, orgService, tipoCalculoPromedio);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            
        }

        private static void calcula(ref Entity _oferta, Entity _ofpre, Entity _tipoProducto, Entity _tarifa)
        {
            
            //formula.calcula(ref variables, localcontext.OrganizationService, _oferta, _ofpre, _tipoProducto, _tarifa, ref errores);
          
            formula.setLog(Log);
            if (tipoCalculoPromedio == true)
            {
                Guid _matrizhorariaid = matrizHoraria(_oferta);
                formula.calculaCollectionPromedio(_matrizhorariaid, ref variables, orgService, ref _oferta, _ofpre, _tipoProducto, _tarifa, ref errores);
            }
            else
                formula.calculaCollection(ref variables, orgService, _oferta, _ofpre, _tipoProducto, _tarifa, ref errores);


        }

        private static Guid matrizHoraria(Entity _oferta)
        {
            Guid _matrizhorariaid = Guid.Empty;
            Guid _sistemaelectricoid = Guid.Empty;
            Guid _tarifaid = Guid.Empty;
            if (_oferta.Attributes.Contains("atos_sistemaelectricoid"))
                _sistemaelectricoid = ((EntityReference)_oferta.Attributes["atos_sistemaelectricoid"]).Id;

            if (_oferta.Attributes.Contains("atos_tarifaid"))
                _tarifaid = ((EntityReference)_oferta.Attributes["atos_tarifaid"]).Id;


            if (_tarifaid == Guid.Empty || _sistemaelectricoid == Guid.Empty)
                return _matrizhorariaid;
            Log.writelog("matrizHoraria - antes de QueryByAttribute");
            QueryByAttribute _consulta = new QueryByAttribute("atos_matrizportarifaysistemaelectrico");

            Log.writelog("matrizHoraria - antes de definir columnas");
            _consulta.ColumnSet = new ColumnSet(new String[] { "atos_matrizhorariaid" });
            _consulta.AddAttributeValue("atos_tarifaid", _tarifaid.ToString());
            _consulta.AddAttributeValue("atos_sistemaelectricoid", _sistemaelectricoid.ToString());


            EntityCollection _resConsulta = orgService.RetrieveMultiple(_consulta);
            if (_resConsulta.Entities.Count > 0)
                _matrizhorariaid = ((EntityReference)_resConsulta.Entities[0].Attributes["atos_matrizhorariaid"]).Id;
            return _matrizhorariaid;
        }




    }
}
