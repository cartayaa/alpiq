using System;
using System.Text;
using System.IO;
using Microsoft.Xrm.Sdk;
using System.Globalization;

namespace Negocio.Entidades
{
    public class AtosLog
    {
        private String pathLog;
        private String fileLog;
        private bool log;
        private ITracingService TracingService;

        public String Log
        {
            get
            {
                return this.fileLog;
            }
            set
            {
                this.log = (value != "") ? true : false;
                this.fileLog = value;
            }
        }


        public ITracingService Traza
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

        public void init(String _pathLog, String _fileLog, ITracingService _traza)
        {
            this.pathLog = _pathLog;
            this.Log = _fileLog;
            this.TracingService = _traza;
        }

        public AtosLog()
        {
            this.Log = "";
            this.TracingService = null;

        }

        public AtosLog(String _pathLog, String _fileLog)
        {
            this.Log = _fileLog;
            this.pathLog = _pathLog;
            this.TracingService = null;
        }

        public AtosLog(String _pathLog, String _fileLog, Boolean _log)
        {
            this.fileLog = _fileLog;
            this.pathLog = _pathLog;
            this.log = _log;
            this.TracingService = null;
        }

        public AtosLog(String _pathLog, String _fileLog, ITracingService _traza)
        {
            this.Log = _fileLog;
            this.pathLog = _pathLog;
            this.TracingService = _traza;
        }

        public AtosLog(String _pathLog, String _fileLog, Boolean _log, ITracingService _traza)
        {
            this.fileLog = _fileLog;
            this.pathLog = _pathLog;
            this.log = _log;
            this.TracingService = _traza;
        }
        public AtosLog(AtosLog _atosLog)
        {
            this.fileLog = _atosLog.fileLog;
            this.pathLog = _atosLog.pathLog;
            this.log = _atosLog.log;
            this.TracingService = _atosLog.TracingService;
        }

        public void writeLog(String _msg)
        {
            if (log)
            {
                StreamWriter _streamWriter = new StreamWriter(pathLog + fileLog, true, Encoding.UTF8);
                _streamWriter.Write(DateTime.Now.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss.ff") + "; " + _msg + "\n");
                _streamWriter.Flush();
                _streamWriter.Close();
            }
            if (TracingService != null)
                TracingService.Trace(string.Format(CultureInfo.InvariantCulture, _msg));
        }

    }
}
