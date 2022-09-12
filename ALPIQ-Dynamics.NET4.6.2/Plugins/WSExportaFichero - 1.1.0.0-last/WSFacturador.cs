using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace WSExportaFichero
{
    public delegate WSFacturador DelCreaWSFacturador();

    #region Environment (Select)

    public abstract class WSFacturador
    {
        #region Protected Var

        protected String nombre;
        protected String urlWS;
        protected String usuarioWS;
        protected String passwordWS;
        protected String uriLogWS = String.Empty;
        protected String nombreServicio;
        protected String nombreInterface;
        protected String nombreComercializadora;
        protected int numMaxIntentos;


        #endregion

        private const string salidaWSEmsOK = "OK";
        protected String nombreFicheroLog = String.Empty;
        protected Logging logger;
        protected Logging Errors;
        protected List<String> _errors = new List<String>();

        public static DelCreaWSFacturador WebServiceECP = creaWSFacturadorDummy;
        
        //public String Nombre { get { return this.nombre; } }
        
        public WSFacturador(String _nombre) { 
            nombre = _nombre; 
        }

         /**
         * Function iniDelCreaWSFacturador call from ReadConfigXML (ExportarXML.c)
         * 
         * Param(String) _entornoWS
         * Param(String) httpsWS
         * Param(String) _urlWS
         * Param(String) _usuarioWS
         * Param(String) _passwordWS
         * Param(String) _TracingService
         * Param(String) _Service
         */
              
        public static void Environment(String _entornoWS, Boolean httpsWS,  String _urlWS,  String _usuarioWS,  String _passwordWS)
        {

            if (!String.IsNullOrEmpty(_usuarioWS) && _entornoWS == "test" && httpsWS == true)
            {
                WSFacturador.WebServiceECP = creaWSFacturadorAuthHttpsTest;
            }
            else if (!String.IsNullOrEmpty(_usuarioWS) && _entornoWS == "pre" && httpsWS == true)
            {
                WSFacturador.WebServiceECP = creaWSFacturadorAuthHttpsPro; //esta es la llamada a los servicios de PRO
            }
            else if (!String.IsNullOrEmpty(_usuarioWS) && _entornoWS == "pre")
            {
                WSFacturador.WebServiceECP = creaWSFacturadorAuthHttpPre; //esta es la llamada a los servicios de PRO
            }
            else if (String.IsNullOrEmpty(_usuarioWS) && _entornoWS == "pro" && httpsWS == true)
            {
                WSFacturador.WebServiceECP = creaWSFacturadorNoAuthHttpsPro;
            }
            else if (String.IsNullOrEmpty(_usuarioWS) && _entornoWS == "pro")
            {
                WSFacturador.WebServiceECP = creaWSFacturadorNoAuthHttpPro; // (servicio no implementado)
            }
            else if (String.IsNullOrEmpty(_usuarioWS) && _entornoWS == "pre" && httpsWS == true)
            {
                WSFacturador.WebServiceECP = creaWSFacturadorNoAuthHttpsPre;
            }
            else if (String.IsNullOrEmpty(_usuarioWS) && _entornoWS == "pre")
            {
                WSFacturador.WebServiceECP = creaWSFacturadorNoAuthHttpPre;
            }
            else
            {
                WSFacturador.WebServiceECP = creaWSFacturadorDummy; //  (servicio no implementado)
            }
        }

        // Facturador dummy
        static public WSFacturador creaWSFacturadorDummy()
        {
            //WSFacturador wsfacturador = new WSFacturadorDummy();
            //return wsfacturador;
            return new WSFacturadorDummy();
        }

        // Pendiente incorporar WSDL (https y autenticación) con clase correspondiente
        // Entorno PRO, HTTP y con autenticación
        static public WSFacturador creaWSFacturadorAuthHttpsTest()
        {
            // WSFacturador wsfacturador = new WSFacturadorAuthSecuredTest();
            // return wsfacturador;
            return new WSFacturadorAuthSecuredTest();
        }

        // Entorno PRO, HTTPS y con autenticación
        static public WSFacturador creaWSFacturadorAuthHttpsPro()
        {
            // WSFacturador wsfacturador = new WSFacturadorAuthSecuredPro();
            // return wsfacturador;
            return new WSFacturadorAuthSecuredPro();
        }

        // Pendiente incorporar WSDL (https y no autenticación) con clase correspondiente
        // Entorno PRO, HTTPS y sin autenticación
        static public WSFacturador creaWSFacturadorNoAuthHttpsPro()
        {
            //WSFacturador wsfacturador = new WSFacturadorNoAuthSecuredPro();
            //return wsfacturador;
            return new WSFacturadorNoAuthSecuredPro();
        }

        // Entorno PRE, HTTPS y sin autenticación
        static public WSFacturador creaWSFacturadorNoAuthHttpsPre()
        {
            //WSFacturador wsfacturador = new WSFacturadorNoAuthSecuredPre();
            //return wsfacturador;
            return new WSFacturadorNoAuthSecuredPre();
        }

        // Entorno HTTP, http y con autenticación
        static public WSFacturador creaWSFacturadorAuthHttpPre()
        {
            // WSFacturador wsfacturador = new WSFacturadorAuthPre();
            // return wsfacturador;
            return new WSFacturadorAuthPre();
        }

        // Pendiente incorporar WSDL (http y sin autenticación) con clase correspondiente
        // Entorno PRO, HTTP y sin autenticación
        static public WSFacturador creaWSFacturadorNoAuthHttpPro()
        {
            // WSFacturador wsfacturador = new WSFacturadorNoAuthPro(); // (servicio no implementado)
            // return wsfacturador;
            return new WSFacturadorNoAuthPro();
        }

        // Entorno PRE, HTTP y sin autenticación
        static public WSFacturador creaWSFacturadorNoAuthHttpPre()
        {
            // WSFacturador wsfacturador = new WSFacturadorNoAuthPre();
            // return wsfacturador;
            return new WSFacturadorNoAuthPre();
        }

        abstract public void Logging(Logging _logger);
   
        public void setLog(String _uriLogWS, String _nombreFicheroLog)
        {
            uriLogWS = _uriLogWS;
            nombreFicheroLog = _nombreFicheroLog;
        }

        protected void writeLog(String mensaje)
        {
            if ( !String.IsNullOrEmpty(uriLogWS) && !String.IsNullOrEmpty(nombreFicheroLog) )
            {
                CommonWS.CommonWS.WriteLog(uriLogWS, nombreFicheroLog, String.Empty, mensaje);
            }
        }

        public void setConexion(String _urlWS, String _usuarioWS, String _passwordWS, String _service, String _interface, String _marketer, int _maxtry)
        {
        this.urlWS = _urlWS;
        this.usuarioWS = _usuarioWS;
        this.passwordWS = _passwordWS;
        this.nombreServicio = _service;
        this.nombreInterface = _interface;
        this.nombreComercializadora = _marketer;
        this.numMaxIntentos = _maxtry;
        }

        // Crea el String XML
        public String createXML(String xmlSalida)
        {
            // Abria falta pasrsear el xcmlSalida para que salta como un XML real
            // Esta colocado de esta manera para eliminat los Tab al inicio de cada linea
            /*
            string postData = String.Format(
@"<send:{0}>
    <arg0>
        <tipo>{1}</tipo>
        <xml>{2}</xml>
        <comercializadora>{3}</comercializadora>
    </arg0>
</send:{0}>",*/

            string postData = String.Format(@"<send:{0}><arg0><tipo>{1}</tipo><xml>{2}</xml><comercializadora>{3}</comercializadora></arg0></send:{0}>",
            nombreServicio, nombreInterface, xmlSalida, nombreComercializadora);

            return postData;
        }

        abstract public String Url();

        abstract public String TraceService(ITracingService _TracingService);

        //abstract public bool send(string nombreServicio, string nombreInterface, string nombreComercializadora, int numMaxIntentos, string xmlSalida);
        abstract public bool send(String xmlSalida, ref String result);
    }

    #endregion

    #region Procesos no usados
    // Entorno PRE sin autenticación y http
    public class WSFacturadorNoAuthPre : WSFacturador
    {
        ITracingService TracingService;

        public WSFacturadorNoAuthPre() : base("WSFacturadorNoAuthPre")
        {
        }

        public override string Url()
        {
            ServicioFacturadorNoAuthPre.BasicHttpBinding_AtosService wsfacturador = new ServicioFacturadorNoAuthPre.BasicHttpBinding_AtosService();
            return wsfacturador.Url;
        }

        public override String TraceService(ITracingService _TracingService)
        {
            TracingService = _TracingService;

            return "valid";
        }

        public override void Logging(Logging _logger)
        {
            logger = _logger;
        }

        override public bool send(String xmlSalida, ref String result)
        {
            bool rc = false;

            TracingService.Trace("Instance <WSFacturadorNoAuthPre> Active");

            ServicioFacturadorNoAuthPre.BasicHttpBinding_AtosService wsfacturador = new ServicioFacturadorNoAuthPre.BasicHttpBinding_AtosService();
            ServicioFacturadorNoAuthPre.arg0 args = new ServicioFacturadorNoAuthPre.arg0();

            args.comercializadora = nombreComercializadora;
            args.tipo = nombreInterface;
            args.xml = xmlSalida;

            if (!String.IsNullOrEmpty(urlWS))
                wsfacturador.Url = urlWS;

            result = wsfacturador.processSend(args);

            return rc;
        }
    }

    /**
     * Entorno PRE sin autenticación y https
     */
    public class WSFacturadorNoAuthSecuredPre : WSFacturador
    {
        ITracingService TracingService;

        public WSFacturadorNoAuthSecuredPre() : base("WSFacturadorNoAuthSecuredPre")
        {
        }

        public override string Url()
        {
            ServicioFacturadorNoAuthSecuredPre.BasicHttpsBinding_AtosService wsfacturador = new ServicioFacturadorNoAuthSecuredPre.BasicHttpsBinding_AtosService();
            return wsfacturador.Url;
        }

        public override String TraceService(ITracingService _TracingService)
        {
            TracingService = _TracingService;

            return "valid";
        }

        public override void Logging(Logging _logger)
        {
            logger = _logger;
        }

        override public bool send(String xmlSalida, ref String result)
        {
            bool rc = false;

            logger.Info(String.Format("WebService.send to ECP de: {0} Num-Max-Intentos: {1}", this.nombre, numMaxIntentos.ToString()));

            ServicioFacturadorNoAuthSecuredPre.BasicHttpsBinding_AtosService wsfacturador = new ServicioFacturadorNoAuthSecuredPre.BasicHttpsBinding_AtosService();
            ServicioFacturadorNoAuthSecuredPre.arg0 args = new ServicioFacturadorNoAuthSecuredPre.arg0();

            args.comercializadora = nombreComercializadora;
            args.tipo = nombreInterface;
            args.xml = xmlSalida;
            
            if (!String.IsNullOrEmpty(urlWS))
                wsfacturador.Url = urlWS;

            result = wsfacturador.processSend(args);

            return rc;
        }
    }

    /**
     * Entorno PRO con autenticación y https
     */
    public class WSFacturadorAuthSecuredPro : WSFacturador
    {
        ITracingService TracingService;

        public WSFacturadorAuthSecuredPro() : base("WSFacturadorAuthSecuredPro")
        {
        }

        public override string Url() { return urlWS; }

        public override String TraceService(ITracingService _TracingService)
        {
            TracingService = _TracingService;

            return "valid";
        }

        public override void Logging(Logging _logger)
        {
            logger = _logger;
        }

        override public bool send(String xmlSalida, ref String result)
        {
            bool rc = false;

            logger.Info("WSFacturadorAuthSecuredPro");
            
            try
            {
                //writeLog("Call ECP: " + this.nombre);
                //TracingService.Trace("Call ECP: " + this.nombre);

                StringBuilder traceWS = new StringBuilder();
                //traceWS.AppendFormat("--[LlamaWSFacturador] de:[{0}].numMaxIntentos:{1}", this.nombre, numMaxIntentos.ToString());
                //traceWS.AppendFormat("--[Call ECP] :[{0}] MaxIntentos:{1}", this.nombre, numMaxIntentos.ToString());
                logger.Info(String.Format("WebService.Send to ECP de: {0} Num-Max-Intentos: {1}", this.nombre, numMaxIntentos.ToString()));

                BasicHttpBinding myBinding = new BasicHttpBinding();

                myBinding.Name = "BasicHttpsBinding_AtosService";
                myBinding.Security.Mode = BasicHttpSecurityMode.Transport;
                myBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                myBinding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
                myBinding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;

                EndpointAddress endPointAddress = new EndpointAddress(urlWS);

                SrvFacturadorAuthSecuredTest.AtosServiceClient wsfacturador = new SrvFacturadorAuthSecuredTest.AtosServiceClient(myBinding, endPointAddress);
/*AC*/          //traceWS.AppendLine("llamaWSFacturador despues de new ServicioFacturadorAuthSecuredPro.AtosServiceClient\n");

                SrvFacturadorAuthSecuredTest.arg0 args = new SrvFacturadorAuthSecuredTest.arg0();
/*AC*/          //traceWS.AppendLine("llamaWSFacturador despues de new SrvFacturadorAuthSecuredPRO.arg0\n");

                args.comercializadora = nombreComercializadora;
                args.tipo             = nombreInterface;
                args.xml              = xmlSalida;

                wsfacturador.ClientCredentials.UserName.UserName = usuarioWS;
                wsfacturador.ClientCredentials.UserName.Password = passwordWS;

                System.ServiceModel.Channels.HttpRequestMessageProperty httpRequestProperty = new System.ServiceModel.Channels.HttpRequestMessageProperty();

                httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] = "Basic " +
                    Convert.ToBase64String(Encoding.ASCII.GetBytes(wsfacturador.ClientCredentials.UserName.UserName + ":" + 
                                                                   wsfacturador.ClientCredentials.UserName.Password));

                int numIntentos = 1;
                using (System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(wsfacturador.InnerChannel))
                {
                    System.ServiceModel.OperationContext.Current.OutgoingMessageProperties[System.ServiceModel.Channels.HttpRequestMessageProperty.Name] = httpRequestProperty;

                    while (String.IsNullOrEmpty(result) && numIntentos <= numMaxIntentos)
                    {
                        try
                        {
                            result = wsfacturador.processSend(args);
                            rc = true;
                        }
                        catch (TimeoutException timoutEx)
                        {
                            result = String.Empty;
                            traceWS.AppendFormat("TimeoutException:{0}. Intento:{0}\n", timoutEx.ToString(), numIntentos.ToString());
                        }
                        catch (System.Net.WebException webEx)
                        {
                            result = String.Empty;
                            traceWS.AppendFormat("WebException:{0}. Intento:{1}\n", webEx.Message, numIntentos.ToString());
                        }
                        catch (System.Web.Services.Protocols.SoapException soapEx)
                        {
                            result = String.Empty;
                            traceWS.AppendFormat("SoapException:{0}.Intento:{1}\n", soapEx.Message, numIntentos.ToString());
                        }
                        catch (Exception ex)
                        {
                            result = String.Empty;
                            traceWS.AppendFormat("Exception:{0}.Intento:{1}\n", ex.Message, numIntentos.ToString());
                        }

                        if (String.IsNullOrEmpty(result) && numIntentos == numMaxIntentos)
                        {
                            result = String.Format("IsNullOrEmpty & Número máximo de intentos ({0}) alcanzado\n", numMaxIntentos.ToString());
                            traceWS.AppendFormat("IsNullOrEmpty & Número máximo de intentos ({0}) alcanzado\n", numMaxIntentos.ToString());
                        }
                        numIntentos++;
                    }
                }
                //writeLog("llamaWSFacturador processSend: " + result);
                traceWS.AppendFormat("Call ECP: send result:{0}\n", result);
                // traceWS.AppendLine("-[llamaWSFacturador]--");

                TracingService.Trace(traceWS.ToString());
                writeLog(traceWS.ToString());
                logger.Error(traceWS.ToString());
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return rc;
        }

    }

    /**
    * Entorno PRO sin autenticación y http - (servicio no implementado)
    * Pendiente WSDL
    */
    public class WSFacturadorNoAuthPro : WSFacturador 
    {
        ITracingService TracingService;

        public WSFacturadorNoAuthPro() : base("WSFacturadorNoAuthPro")
        {
        }

        public override string Url() { return urlWS; }

        public override String TraceService(ITracingService _TracingService)
        {
            TracingService = _TracingService;

            return "valid";
        }

        public override void Logging(Logging _logger)
        {
            logger = _logger;
        }

        override public bool send(String xmlSalida, ref String result)
        {
            logger.Info(string.Format("Servicio no implementado: {0}", base.nombre));

            result = "Servicio no implementado";

            return false;
        }
        
    }

    /**
    * Entorno PRO sin autenticación y https - No implementado
    * Pendiente WSDL
    */
    public class WSFacturadorNoAuthSecuredPro : WSFacturador
    {
        ITracingService TracingService;

        public WSFacturadorNoAuthSecuredPro() : base("WSFacturadorNoAuthSecuredPro")
        {
        }

        public override string Url() { return urlWS; }

        public override String TraceService(ITracingService _TracingService)
        {
            TracingService = _TracingService;

            return "valid";
        }

        public override void Logging(Logging _logger)
        {
            logger = _logger;
        }

        override public bool send(String xmlSalida, ref String result)
        {
            logger.Info(String.Format("Servicio no implementado: {0}",nombreServicio));

            result = "Servicio no implementado";
            return false;
        }    
    }

    /**
    * Entorno PRE sin autenticación y http
    * Pendiente WSDL
    */
    public class WSFacturadorAuthPre : WSFacturador
    {
        ITracingService TracingService;

        public WSFacturadorAuthPre() : base("WSFacturadorAuthPre")
        {
        }

        public override string Url()
        {
            ServicioFacturadorAuthSecuredPre.BasicHttpBinding_AtosService wsfacturador = new ServicioFacturadorAuthSecuredPre.BasicHttpBinding_AtosService();
            return wsfacturador.Url;
        }

        public override String TraceService(ITracingService _TracingService)
        {
            TracingService = _TracingService;

            return "valid";
        }

        public override void Logging(Logging _logger)
        {
            logger = _logger;
        }

        override public bool send(String xmlSalida, ref String result)
        {
            bool rc = false;

            ServicioFacturadorAuthSecuredPre.BasicHttpBinding_AtosService wsfacturador = new ServicioFacturadorAuthSecuredPre.BasicHttpBinding_AtosService();

            // ServicioFacturadorPre.arg0 args = new ServicioFacturadorPre.arg0();
            ServicioFacturadorAuthSecuredPre.arg0 args = new ServicioFacturadorAuthSecuredPre.arg0();

            args.comercializadora = nombreComercializadora;
            args.tipo = nombreInterface;
            args.xml = xmlSalida;

            if (!String.IsNullOrEmpty(urlWS))
                wsfacturador.Url = urlWS;

            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential();
            credentials.UserName = usuarioWS;
            credentials.Password = passwordWS;
            wsfacturador.Credentials = credentials;

            result = wsfacturador.processSend(args);

            return rc;
        }
    }


    /**
    * Entorno dummy - (servicio vacío)
    * Pendiente WSDL
    */
    public class WSFacturadorDummy : WSFacturador
    {
        ITracingService TracingService;

        public WSFacturadorDummy() : base("WSFacturadorDummy")
        {
        }

        public override string Url() { return urlWS; }

        public override String TraceService(ITracingService _TracingService)
        {
            TracingService = _TracingService;

            return "valid";
        }

        public override void Logging(Logging _logger)
        {
            logger = _logger;
        }

        override public bool send(String xmlSalida, ref String result)
        {
            logger.Info(String.Format("Servicio vacío: {0}", base.nombre));

            return false;
        }
    }

    #endregion
    /**
    * Entorno TEST con autenticación y http
    * Pendiente WSDL
    */
    public class WSFacturadorAuthSecuredTest : WSFacturador
    {
        ITracingService TracingService;

        public WSFacturadorAuthSecuredTest() : base("WSFacturadorAuthSecuredTest")
        {
        }

        public override string Url() { return urlWS; }

        public override String TraceService(ITracingService _TracingService)
        {
            TracingService = _TracingService;

            return "valid";
        }

        public override void Logging(Logging _logger)
        {
            logger = _logger;
        }


        override public bool send(String xmlSalida, ref String result)
        {
            /* T logger.Info("WSFacturadorAuthSecuredTest"); */

            //------------------------------------------------
            //
            // ESTE es el que funciona
            //
            //------------------------------------------------

            bool rc = false;
            String Response = String.Empty;
            try
            {        
                BasicHttpBinding myBinding = new BasicHttpBinding();

                myBinding.Name = "BasicHttpsBinding_AtosService";
                myBinding.Security.Mode = BasicHttpSecurityMode.Transport;
                myBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                myBinding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
                myBinding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;

                //EndpointAddress endPointAddress = new EndpointAddress("https://ecp-test.alpiq.com/ECP.WebServices-RequestTesting/AtosService.svc");
                EndpointAddress endPointAddress = new EndpointAddress(urlWS);

                SrvFacturadorAuthSecuredTest.AtosServiceClient wsfacturador = new SrvFacturadorAuthSecuredTest.AtosServiceClient(myBinding, endPointAddress);

                SrvFacturadorAuthSecuredTest.arg0 args = new SrvFacturadorAuthSecuredTest.arg0();

                args.comercializadora = nombreComercializadora;
                args.tipo = nombreInterface;
                args.xml = xmlSalida;

                wsfacturador.ClientCredentials.UserName.UserName = usuarioWS;
                wsfacturador.ClientCredentials.UserName.Password = passwordWS;

                System.ServiceModel.Channels.HttpRequestMessageProperty httpRequestProperty = new System.ServiceModel.Channels.HttpRequestMessageProperty();

                httpRequestProperty.Headers[System.Net.HttpRequestHeader.Authorization] = "Basic " +
                    Convert.ToBase64String(Encoding.ASCII.GetBytes(wsfacturador.ClientCredentials.UserName.UserName + ":" + wsfacturador.ClientCredentials.UserName.Password));

                int numIntentos = 1;

                using (System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(wsfacturador.InnerChannel))
                {
                    System.ServiceModel.OperationContext.Current.OutgoingMessageProperties[System.ServiceModel.Channels.HttpRequestMessageProperty.Name] =
                        httpRequestProperty;

                    while (String.IsNullOrEmpty(Response) && numIntentos<=numMaxIntentos)
                    {
                        try
                        {
                            Response = wsfacturador.processSend(args);

                            //off logger.Error("Result:" + Response);

                            if (!String.IsNullOrEmpty(Response) && Response.Substring(0, 2).ToUpper() == "OK")
                            { 
                                result = Response.Substring(0, 2).ToUpper();
                                rc = true;
                            }
                        }
                        catch (TimeoutException timoutEx)
                        {
                            result = String.Format("TimeoutException: {0} Intention: {1} ", timoutEx.Message.ToString(), numIntentos.ToString()); 
                            logger.Error(result);       
                        }
                        catch (System.Net.WebException webEx)
                        {
                            result = String.Format("WebException: {0} Intention: {1} ", webEx.Message, numIntentos.ToString());
                            logger.Error(result);                           
                        }
                        catch (System.Web.Services.Protocols.SoapException soapEx)
                        {
                            result = String.Format("Protocols.SoapException: {0} Intention: {1} ", soapEx.Message, numIntentos.ToString());
                            logger.Error(result);                            
                        }
                        catch (Exception ex)
                        {
                            result = String.Format("Exception: {0} Intention: {1} ", ex.Message, numIntentos.ToString());
                            logger.Error(result);                            
                        }

                        if (String.IsNullOrEmpty(result) && numIntentos == numMaxIntentos)
                        {
                            result = String.Format("Maximum Intention: {0} ", numIntentos.ToString());
                            logger.Error(result);                            
                        }
                        numIntentos++;
                    }
                }
            }
            catch (Exception ex)
            {
                result = String.Format("Exception - send: {0} ", ex.Message.ToString());
            }

            return rc;
        }
    }
}
