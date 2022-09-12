using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Microsoft.Xrm.Sdk;

namespace CommonWS
{
    public class Log
    {
        private Boolean escribirLog = false;
        private String uriWSLog = "";
        private String subCarpetaLog = "";
        private String ficheroLog = "";
        ITracingService TracingService;

        public Log()
        {
            this.setLog(false, "", "", "", null);
        }

        public Log(Boolean _log, String _uriWSLog, String _carpetaLog, String _ficheroLog)
        {
            this.setLog(_log, _uriWSLog, _carpetaLog, _ficheroLog, null);
        }

        public Log(Boolean _log, String _uriWSLog, String _carpetaLog, String _ficheroLog, ITracingService _TracingService)
        {
            this.setLog(_log, _uriWSLog, _carpetaLog, _ficheroLog, _TracingService);
        }

        public Log(Log _log)
        {
            this.setLog(_log);
        }

        public void setLog(Log _log)
        {
            escribirLog = _log.escribirLog;
            uriWSLog = _log.uriWSLog;
            subCarpetaLog = _log.subCarpetaLog;
            ficheroLog = _log.ficheroLog;
            TracingService = _log.TracingService;
        }


        public void setLog(Boolean _log, String _uriWSLog, String _carpetaLog, String _ficheroLog, ITracingService _TracingService)
        {
            escribirLog = _log;
            uriWSLog = _uriWSLog;
            subCarpetaLog = _carpetaLog;
            ficheroLog = _ficheroLog;
            TracingService = _TracingService;
        }

        public Boolean EscribirLog
        {
            get
            {
                return this.escribirLog;
            }
        }


        public String UriWSLog
        {
            get
            {
                return this.uriWSLog;
            }
        }


        public String SubCarpetaLog
        {
            get
            {
                return this.subCarpetaLog;
            }
        }

        public String FicheroLog
        {
            get
            {
                return this.ficheroLog;
            }
        }
        
        public ITracingService tracingService
        {
            get
            {
                return this.TracingService;
            }
            set
            {
                this.TracingService = value;
            }
        }

        public void traza(String texto, bool _traza = true)
        {
            if (_traza && TracingService != null)
                TracingService.Trace(texto);
        }

        public void writelog(String texto, bool _traza = false)
        {
            try
            {
                traza(texto, _traza);
                if (escribirLog == true)
                    CommonWS.WriteLog(uriWSLog, ficheroLog, subCarpetaLog, texto);
            }
            catch (Exception ex)
            {
                traza(ex.Message, _traza);
            }
        }



    }

    public class CommonWS
    {

        public const String UrlNamespace = "http://localhost/alpiqws/";
        public const String PrefNamespace = "alp";


        private static string callWS(String uriWS, String service, String body, String _prefNamespace, String _UrlNamespace)
        {

            string result = "";

            String postData = String.Format(@"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:{0}='{1}'>
                                   <soapenv:Header/>
                                   <soapenv:Body>
                                      <{0}:{2}>
                                         {3}
                                      </{0}:{2}>
                                   </soapenv:Body>
                                </soapenv:Envelope>", _prefNamespace, _UrlNamespace, service, body);


            /*String postData = String.Format(@"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:tem='http://tempuri.org/'>
                                <soapenv:Header/>
                                <soapenv:Body>
                                    <tem:{0}>
      	                            {1}
                                    </tem:{2}>
                                </soapenv:Body>
                            </soapenv:Envelope>", service, body, service);*/

            byte[] data = ASCIIEncoding.UTF8.GetBytes(postData);



            try
            {
                HttpWebRequest myRequest =
            (HttpWebRequest)WebRequest.Create(uriWS);
                myRequest.Method = "POST";

                myRequest.ContentType = "text/xml;charset=UTF-8";
                //myRequest.ContentType = "text/xml;charset=ISO-8859-1";
                myRequest.ContentLength = data.Length;


                myRequest.PreAuthenticate = false;
                myRequest.ProtocolVersion = HttpVersion.Version11;
                myRequest.ServicePoint.Expect100Continue = false;

                myRequest.Timeout = 7200000; // 120000;
                Stream newStream = myRequest.GetRequestStream();

                // Send the data.
                newStream.Write(data, 0, data.Length);
                newStream.Close();

                // Get response  
                using (HttpWebResponse response = myRequest.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    // Read the whole contents and return as a string  
                    result = reader.ReadToEnd();
                }

                return result;
            }
            catch (Exception e) {
                return e.Message;
            }
            

        }

        /*
        private static string callWS(String uriWS, String service, String body)
        {
            return callWS(uriWS, service, body, CommonWS.PrefNamespace, CommonWS.UrlNamespace);            
        }*/


        public static string WriteLog(String uriWS, String NombreFichero, String SubCarpeta, String Mensaje)
        {
            return WriteLog(uriWS, CommonWS.PrefNamespace, CommonWS.UrlNamespace, NombreFichero, SubCarpeta, Mensaje);

        }


        public static string WriteLog(String uriWS, String pref, String urlnamespc, String NombreFichero, String SubCarpeta, String Mensaje)
        {
            //String uriWS = "https://maquinavirtual678.westeurope.cloudapp.azure.com/GenerarInformes.asmx";

            String body = String.Format(@"<{0}:nombre>{1}</{0}:nombre>
    <{0}:Subcarpetas>{2}</{0}:Subcarpetas>
    <{0}:Contenido><![CDATA[{3}]]></{0}:Contenido>", pref, NombreFichero, SubCarpeta, Mensaje);
            
            return callWS(uriWS, "WriteLog", body, pref, urlnamespc);

        }

        public static string WriteLogFile(String uriWS, String NombreFichero, String SubCarpeta, String Mensaje)
        {
            //String uriWS = "https://maquinavirtual678.westeurope.cloudapp.azure.com/GenerarInformes.asmx";

            String body = String.Format(@"<{0}:nombre>{1}</{0}:nombre>
    <{0}:Subcarpetas>{2}</{0}:Subcarpetas>
    <{0}:Contenido><![CDATA[{3}]]></{0}:Contenido>", CommonWS.PrefNamespace, NombreFichero, SubCarpeta, Mensaje);

            return callWS(uriWS, "WriteLogFile", body, CommonWS.PrefNamespace, CommonWS.UrlNamespace);

        }

        public static string WriteFile(String uriWS, String NombreFichero, String Mensaje)
        {
            return WriteFile(uriWS, CommonWS.PrefNamespace, CommonWS.UrlNamespace, NombreFichero, Mensaje);
        }

        public static string WriteFile(String uriWS, String pref, String urlnamespc, String NombreFichero, String Mensaje)
        {
            //String uriWS = "https://maquinavirtual678.westeurope.cloudapp.azure.com/GenerarInformes.asmx";

            String body = String.Format(@"<{0}:nombreypath>{1}</{0}:nombreypath>
    <{0}:Contenido><![CDATA[{2}]]></{0}:Contenido>", pref, NombreFichero, Mensaje);

            return callWS(uriWS, "WriteFile", body, pref, urlnamespc);

        }

        public static string GenerarInforme(String uriWS, String OfertaID, String urlDescarga, String tipoInforme, String esPDF, bool hayLog)
        {
            return GenerarInforme(uriWS, CommonWS.PrefNamespace, CommonWS.UrlNamespace, OfertaID, urlDescarga, tipoInforme, esPDF, hayLog);
        }

        public static string GenerarInforme(String uriWS, String pref, String urlnamespc, String OfertaID, String urlDescarga, String tipoInforme, String esPDF, bool hayLog)
        {
            // <alp:url_maquina>?</alp:url_maquina>

            String body = String.Format(@"<{0}:idOferta>{1}</{0}:idOferta>
                                        <{0}:url_maquina>{2}</{0}:url_maquina>
                                        <{0}:hayLog>{3}</{0}:hayLog>
                                        <{0}:tipoInforme>{4}</{0}:tipoInforme>
                                        <{0}:pdf>{5}</{0}:pdf>"
                                        , pref, OfertaID, urlDescarga, hayLog.ToString(), tipoInforme, esPDF);

            //WriteLog(uriWS, "TEST.txt", "", body);

            return callWS(uriWS, "GenerateReport", body, pref, urlnamespc);

        }


        public static string MoveFile(String uriWS, String nombreFichero, String pathOrigen, String pathDestino)
        {
            return MoveFile(uriWS, CommonWS.PrefNamespace, CommonWS.UrlNamespace, nombreFichero, pathOrigen, pathDestino);
        }

        public static string MoveFile(String uriWS, String pref, String urlnamespc, String nombreFichero, String pathOrigen, String pathDestino)
        {
            String body = String.Format(@"<{0}:nombreFichero>{1}</{0}:nombreFichero>
    <{0}:pathOrigen>{2}</{0}:pathOrigen>
    <{0}:pathDestino>{3}</{0}:pathDestino>", pref, nombreFichero, pathOrigen, pathDestino);
            return callWS(uriWS, "MoveFile", body, pref, urlnamespc);

        }

        public static StreamReader RemoteFile2Stream(String uriFile)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(uriFile);
            StreamReader reader = new StreamReader(stream);
            return reader;
        }

        public static string ReadRemoteFile(String uriFile)
        {
            return CommonWS.RemoteFile2Stream(uriFile).ReadToEnd();
        }



        public static string[] ReadRemoteFileAllLines(String uriFile)
        {
            return CommonWS.RemoteFile2Stream(uriFile).ReadToEnd().Split(Environment.NewLine.ToCharArray());
        }
    }
}
