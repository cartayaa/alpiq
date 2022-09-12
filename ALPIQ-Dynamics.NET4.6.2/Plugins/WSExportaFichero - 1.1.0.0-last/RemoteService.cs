using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Web.Services.Protocols;
using Microsoft.Xrm.Sdk;


    public class Remote
    {
        private ITracingService TracingService;
        private IOrganizationService Service;
        public const String UrlNamespace = "http://localhost/alpiqws/";
        public const String PrefNamespace = "alp";
        //private String urlWS;
        //private String usuarioWS;
        //private String passwordWS;
        private Stream streamIO;
        //private DateTime dateLog;

        public Remote(ITracingService _TracingService, IOrganizationService _Service)
        {
            TracingService = _TracingService;
            Service = _Service;
        }

        public string ReadRemoteFile(String uriFile)
        {
            return RemoteFile2Stream(uriFile).ReadToEnd();
        }

        public StreamReader RemoteFile2Stream(String uriFile)
        {
            WebClient WebClient = new WebClient();

            streamIO = WebClient.OpenRead(uriFile);
            StreamReader reader = new StreamReader(streamIO);
            return reader;
        }

        public string[] ReadRemoteFileAllLines(String uriFile)
        {
            return RemoteFile2Stream(uriFile).ReadToEnd().Split(Environment.NewLine.ToCharArray());
        }

        public void CloseRemoteFile()
        {
            streamIO.Close();
        }

        /**
         * Function WriteRemoteLogFile
         * String uriWS
         * String NombreFichero
         * String SubCarpeta
         * String Mensaje
         */
        public string WriteRemoteLogFile(String uriWS, String NombreFichero, String SubCarpeta, String Mensaje)
        {
            String body = String.Format(@"<{0}:nombre>{1}</{0}:nombre>" +
                                         "<{0}:Subcarpetas>{2}</{0}:Subcarpetas>" +
                                         "<{0}:Contenido><![CDATA[{3}]]></{0}:Contenido>",
                                         PrefNamespace, NombreFichero, SubCarpeta, Mensaje);

            return callWS(uriWS, "WriteLogFile", body, PrefNamespace, UrlNamespace);
        }


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

            byte[] data = ASCIIEncoding.UTF8.GetBytes(postData);

            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(uriWS);
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
            catch (Exception e)
            {
                return e.Message;
            }
        }

    }
