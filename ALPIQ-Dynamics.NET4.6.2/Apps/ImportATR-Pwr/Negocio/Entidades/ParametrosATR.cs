using System;

namespace Negocio.Entidades
{
    public class ParametrosATR
    {

        //private String inputDirectory;
        //private String archivedDirectory;
        private String xmlDirectory;
        private String xsdDirectory;
        private String sendDirectory;
        private String errorDirectory;
        private String target;
        private String logDirectory;
        private String fileLog;
        private Boolean log;


        public ParametrosATR(String _pathXml, String _pathXsd, String _pathSend, String _pathError, String _pathLog, String _fileLog, Boolean _log, String _target)
        {
            xmlDirectory = _pathXml;
            xsdDirectory = _pathXsd;
            sendDirectory = _pathSend;
            errorDirectory = _pathError;
            logDirectory = _pathLog;
            fileLog = _fileLog;
            log = _log;
            target = _target;
        }


        public String Xml
        {
            get
            {
                return this.xmlDirectory;
            }
            set
            {
                this.xmlDirectory = value;
            }
        }
        //public String Input
        //{
        //    get
        //    {
        //        return this.inputDirectory;
        //    }
        //    set
        //    {
        //        this.inputDirectory = value;
        //    }
        //}

        public String Send
        {
            get
            {
                return this.sendDirectory;
            }
            set
            {
                this.sendDirectory = value;
            }
        }


        public String Error
        {
            get
            {
                return this.errorDirectory;
            }
            set
            {
                this.errorDirectory = value;
            }
        }

        //public String Archived
        //{
        //    get
        //    {
        //        return this.archivedDirectory;
        //    }
        //    set
        //    {
        //        this.archivedDirectory = value;
        //    }
        //}

        public String Xsd
        {
            get
            {
                return this.xsdDirectory;
            }
            set
            {
                this.xsdDirectory = value;
            }
        }

        public String LogPath
        {
            get
            {
                return this.logDirectory;
            }
            set
            {
                this.logDirectory = value;
            }
        }

        public String Log
        {
            get
            {
                return this.fileLog;
            }
            set
            {
                this.fileLog = value;
            }
        }

        public String Target
        {
            get
            {
                return this.target;
            }
            set
            {
                this.target = value;
            }
        }

        public Boolean HayLog
        {
            get
            {
                return this.log;
            }
            set
            {
                this.log = value;
            }
        }

    }
}
