using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Web.Services.Protocols;
using Microsoft.Xrm.Sdk;

namespace WSExportaFichero
{
    public class Logging
    {
        private ITracingService TracingService;
        public const String UrlNamespace = "http://localhost/alpiqws/";
        public const String PrefNamespace = "alp";

        private String UriLog = "";
        private String DirectoryLog = "";
        private String FileLog = "";

        StringBuilder buffer;

        public Logging(ITracingService _TracingService)
        {
            buffer = new StringBuilder();
            TracingService = _TracingService;
        }

        public void Parameters(String _uriWSLog, String _carpetaLog, String _ficheroLog)
        {
            UriLog = _uriWSLog;
            DirectoryLog = _carpetaLog;
            FileLog = _ficheroLog;
        }

        public void Trace(String message)
        {
            String dt = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss,fff");

            TracingService.Trace(String.Format("{0}  INFO {1}", dt.ToString(), message));
        }

        public void Null(String message)
        {
            String dt = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss,fff");
            buffer.AppendFormat("\r{0}       ", message);
        }

        public void Info(String message)
        {
            String dt = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss,fff");

            buffer.AppendFormat("\r{0}  INFO {1}", dt.ToString(), message);
            TracingService.Trace(String.Format("{0}  INFO {1}", dt.ToString(), message));
        }

        public void Error(String message)
        {
            String dt = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss,fff");

            buffer.AppendFormat("\r{0} ERROR {1}", dt.ToString(), message);
            TracingService.Trace(String.Format("{0} ERROR {1}", dt.ToString(), message));
        }


        public void Write()
        {
            this.Info("----------------------------------------------------------------------------");
            CommonWS.CommonWS.WriteLog(UriLog, FileLog, DirectoryLog, buffer.ToString());

            buffer.Clear();
        }
    }
}