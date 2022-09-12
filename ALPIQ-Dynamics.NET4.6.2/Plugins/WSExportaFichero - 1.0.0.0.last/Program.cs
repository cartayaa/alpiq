using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.Web.Services.Protocols;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk;
using System.ServiceModel.Description;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm;
//using System.IO;
//using System.Xml.Linq;
//using System.Xml.Serialization;

using ExportarFichero.ServiceInfo;

namespace WSExportaFichero
{
    class Program
    {
        // Miguel Angel 03/03/2021
        // Se llamará dentro del metodo conectarCRM

       //private static IOrganizationService service;
        
        static void Main(string[] args)
        {
            ////directoFacturadorRoundtrip();

            //String nombreServicio = "processSend";
            //String comercializadora = "Alpiq";
            //String nombreInterface = "CUSTOMER";
            //String xml = "<RegistroEntrada><registro><c_cif>C0952810J</c_cif><c_razon_social>Test Manu RS1</c_razon_social><c_duns>111</c_duns><c_calle>Calle Test</c_calle><c_poblacion>Población Test</c_poblacion><c_municipio>MunicipioTest</c_municipio><c_cod_postal>xxxxx</c_cod_postal><c_provincia>Provincia Test</c_provincia><c_cod_pais>xxx</c_cod_pais><c_telefono/><c_fax/><c_email/><c_contacto/><c_grupo_negociador>Test Manu CN1</c_grupo_negociador><c_cnae>011</c_cnae></registro></RegistroEntrada>";

            ////WSFacturador.iniDelCreaWSFacturador("pre", false, "http://ecp-test.alpiq.com:4431/ECP.WebServices-NoAuth/AtosService.svc", "", "");
            //WSFacturador.iniDelCreaWSFacturador("test", true, "https://ecp-test.alpiq.com/ECP.WebServices-RequestTesting/AtosService.svc", "ECP_Atos", "V/4*wE-Jqd1U");
            //WSFacturador wsf = WSFacturador.creaWSFacturador();

            //String postData = wsf.construyeMensajeXml(nombreServicio, nombreInterface, comercializadora, xml);
            //wsf.setLog("http://localhost/InterfaceAlpiq/Alpiq.asmx", "WSExportaFicheros.txt");

            //wsf.setConexion("https://ecp-test.alpiq.com/ECP.WebServices-RequestTesting/AtosService.svc", "ECP_Atos", "V/4*wE-Jqd1U");
            //String result = wsf.llamaWSFacturador(nombreServicio, nombreInterface, comercializadora,2, xml);

            //CommonWS.CommonWS.WriteLog("http://localhost/InterfaceAlpiq/Alpiq.asmx", "WSExportaFicheros.txt", "", DateTime.Now.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss.ff") + " (WSExportaFicheros)" + " - wsf.Url(): " + wsf.Url());
            //////WSFacturador.iniDelCreaWSFacturador("pre", false, "http://ecp-test.alpiq.com:4431/ECP.WebServices-NoAuth/AtosService.svc", "", "");

            ////WSFacturador.iniDelCreaWSFacturador("test", true, "https://ecp-test.alpiq.com/ECP.WebServices-RequestTesting/AtosService.svc", "ECP_Atos", "V/4*wE-Jqd1U");
            ////WSFacturador wsf2 = WSFacturador.creaWSFacturador();
            ////wsf2.construyeMensajeXml(nombreServicio, nombreInterface, comercializadora, xml);
            ////wsf2.setConexion("https://ecp-test.alpiq.com/ECP.WebServices-RequestTesting/AtosService.svc", "ECP_Atos", "V/4*wE-Jqd1U");
            ////result = wsf2.llamaWSFacturador(nombreServicio, nombreInterface, comercializadora, xml);

            try
            {
                // Miguel Angel 03/03/2021
                // declaro la variable dentro del metodo
    
                //ITracingService tracingService = null;

                // String _pathRegistro = "http://www.services.aeeportal.es:1025/Consultas";
                String _pathRegistro = "C:/Alpiq/Consultas/";
                String _pathDiccionario = "diccionario_con_cobertura";
                String _fQuery = "consulta_cobertura.cons.txt";

                String urlWSLog = "http://www.services.aeeportal.es:1025/Alpiq.aspx";

                // Miguel Angel 03/03/2021
                // Credenciales de producción
                //String uri365 = "https://alpiqenergia.crm4.dynamics.com";
                //String user365 = "admin@AlpiqEnergia.onmicrosoft.com";
                //String pass365 = "Alpiq.2017.bis";

                // Miguel Angel 03/03/2021
                // Credenciales de Pre
                String uri365 = "https://alpiqeneregiapre.crm4.dynamics.com";
                String user365 = "admin@AlpiqEnergia.onmicrosoft.com";
                String pass365 = "Alpiq.2017.bis";

                conectarCRM(uri365, user365, pass365);

                // Miguel Angel 03/03/2021
                // Llamo al metodo con el constructor vacio en vez de pasarlo como parametro
                // y comento el que había.

                Logging logger = new Logging(null); // OPEN LOG

                //Exportar.ExportarXML _exportar = new Exportar.ExportarXML(ServiceInfo tracingService, ServiceInfo service, logger);
                Exportar.ExportarXML _exportar = new Exportar.ExportarXML(null, null, null, null);
                //Exportar.ExportarXML _exportar = new Exportar.ExportarXML(tracingService, service);

                _exportar.SetPath(_pathRegistro, _pathDiccionario, _fQuery, urlWSLog);
                bool _resultadoExportar;

                _resultadoExportar = _exportar.exporta(_exportar.query());

                Entity cliente = getCliente("S00417");

                _resultadoExportar = _exportar.exporta(_exportar.query(cliente.Id));
            }
            catch (Exception ex)
            {
                string error = ex.Message; 
            }


        }

        private static void conectarCRM(String uri365, String user365, String pass365)
        {
            try
            {
                String connectionString = String.Format("Url={0}; Username={1}; Password={2}; authtype=Office365", uri365, user365, pass365);

                CrmServiceClient conn = new Microsoft.Xrm.Tooling.Connector.CrmServiceClient(connectionString);

                // Miguel Angel 03/03/2021
                // Creo una variable local y comentó la que se declaraba a nivel de clase
                
                //IOrganizationService service = (IOrganizationService)conn.OrganizationWebProxyClient != null ? (IOrganizationService)conn.OrganizationWebProxyClient : (IOrganizationService)conn.OrganizationServiceProxy;
                // Guardo la variable service es una propiedad dentro de la clase ServiceInfo;

                ServiceInfo.service = (IOrganizationService)conn.OrganizationWebProxyClient != null ? (IOrganizationService)conn.OrganizationWebProxyClient : (IOrganizationService)conn.OrganizationServiceProxy;
                //service = (IOrganizationService)conn.OrganizationWebProxyClient != null ? (IOrganizationService)conn.OrganizationWebProxyClient : (IOrganizationService)conn.OrganizationServiceProxy;
            }
            catch (SoapException soapEx)
            {
                throw new Exception(String.Format("Error en el acceso a CRM: {0}", soapEx.Detail.InnerText));
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error en el acceso a CRM: {0}", ex.Message));
            }


        }

        private static Entity getCliente(string identificador)
        {
            EntityCollection Solicitudes = new EntityCollection();

            QueryExpression query = new QueryExpression()
            {
                Distinct = false,
                EntityName = "account",
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
                                        new ConditionExpression("accountnumber", ConditionOperator.Equal,identificador),
                                    },
                                }
                            }
                }
            };

            // Miguel Angel 03/03/2021
            // Creo una variable local y comentó la que se declaraba a nivel de clase
            IOrganizationService service = null;
            Solicitudes = service.RetrieveMultiple(query);
            
            return Solicitudes.Entities.FirstOrDefault(); ;

        }

        static void directoFacturadorTestSrv ()
        {
            String nombreComercializadora = "Alpiq";
            String nombreInterface = "CUSTOMER";
            String xmlSalida = "<RegistroEntrada><registro><c_cif>C0952810J</c_cif><c_razon_social>Test Manu RS1</c_razon_social><c_duns>111</c_duns><c_calle>Calle Test</c_calle><c_poblacion>Población Test</c_poblacion><c_municipio>MunicipioTest</c_municipio><c_cod_postal>xxxxx</c_cod_postal><c_provincia>Provincia Test</c_provincia><c_cod_pais>xxx</c_cod_pais><c_telefono/><c_fax/><c_email/><c_contacto/><c_grupo_negociador>Test Manu CN1</c_grupo_negociador><c_cnae>011</c_cnae></registro></RegistroEntrada>";

            
            SrvFacturadorAuthSecuredTest.AtosServiceClient wsfacturador = null;
            wsfacturador = new SrvFacturadorAuthSecuredTest.AtosServiceClient("BasicHttpsBinding_AtosService", "https://ecp-test.alpiq.com/ECP.WebServices-RequestTesting/AtosService.svc");
            wsfacturador.Endpoint.Binding.Namespace = "ecp-test.alpiq.com";
            wsfacturador.ClientCredentials.UserName.UserName = "ECP_ATOS";
            wsfacturador.ClientCredentials.UserName.Password = "V/4*wE-Jqd1U";
            
            SrvFacturadorAuthSecuredTest.arg0 args = new SrvFacturadorAuthSecuredTest.arg0();

            args.comercializadora = nombreComercializadora;
            args.tipo = nombreInterface;
            args.xml = xmlSalida;


            ////QUITAR ESTA LINEA
            //wsfacturador.ClientCredentials.Windows.ClientCredential = new System.Net.NetworkCredential();
            wsfacturador.ClientCredentials.UserName.UserName = "ECP_ATOS";
            wsfacturador.ClientCredentials.UserName.Password = "V/4*wE-Jqd1U";
            //-------------------------------------------------------------
            //wsfacturador.ClientCredentials.Windows.ClientCredential = new System.Net.NetworkCredential(usuarioWS, passwordWS);
            if (wsfacturador.ClientCredentials.UserName!=null)
            { 
                String result = wsfacturador.processSend (args);
                
            }
            wsfacturador.Close();
        }

        static void directoFacturadorRoundtrip ()
        {
            String nombreComercializadora = "Alpiq";
            String nombreInterface = "CUSTOMER";
            String xmlSalida = "<RegistroEntrada><registro><c_cif>C0952810J</c_cif><c_razon_social>Test Manu RS1</c_razon_social><c_duns>111</c_duns><c_calle>Calle Test</c_calle><c_poblacion>Población Test</c_poblacion><c_municipio>MunicipioTest</c_municipio><c_cod_postal>xxxxx</c_cod_postal><c_provincia>Provincia Test</c_provincia><c_cod_pais>xxx</c_cod_pais><c_telefono/><c_fax/><c_email/><c_contacto/><c_grupo_negociador>Test Manu CN1</c_grupo_negociador><c_cnae>011</c_cnae></registro></RegistroEntrada>";


            SrvFacturadorAuthSecuredTest.AtosServiceClient wsfacturador = null;
            wsfacturador = new SrvFacturadorAuthSecuredTest.AtosServiceClient("BasicHttpsBinding_AtosService", "https://ecp-test.alpiq.com/ECP.WebServices-RequestTesting/AtosService.svc");


            SrvFacturadorAuthSecuredTest.arg0 args = new SrvFacturadorAuthSecuredTest.arg0();


            args.comercializadora = nombreComercializadora;
            args.tipo = nombreInterface;
            args.xml = xmlSalida;


            ////QUITAR ESTA LINEA
            //wsfacturador.ClientCredentials.Windows.ClientCredential = new System.Net.NetworkCredential();
            wsfacturador.ClientCredentials.UserName.UserName = "ECP_ATOS";
            wsfacturador.ClientCredentials.UserName.Password = "V/4*wE-Jqd1U";

            System.ServiceModel.Channels.HttpRequestMessageProperty httpRequestProperty = new System.ServiceModel.Channels.HttpRequestMessageProperty();

            httpRequestProperty.Headers[HttpRequestHeader.Authorization] = "Basic " +
                Convert.ToBase64String(Encoding.ASCII.GetBytes(wsfacturador.ClientCredentials.UserName.UserName + ":" +
                wsfacturador.ClientCredentials.UserName.Password));

            using (System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(wsfacturador.InnerChannel))
            {
                System.ServiceModel.OperationContext.Current.OutgoingMessageProperties[System.ServiceModel.Channels.HttpRequestMessageProperty.Name] =
                    httpRequestProperty;

                // Invoke client
                String resultado = wsfacturador.processSend(args);
            }
        }
    }
}

