using System;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;
using System.Xml.Schema;
using Microsoft.Xrm.Sdk;


namespace Negocio.Entidades
{
    /**
    // <summary>
    // Clase que se utiliza para generar el xml a partir de la clase generada por xsd.exe y para validarlo contra el xsd
    // </summary>
    // <remarks>
    // El método xml2class sirve para construir el fichero xml<br/>
    // El método validateXml sirve para verificar que el fichero xml cumple con el xsd.
    // </remarks>
     */
    public class AtosXml
    {
        private String pathXml;
        private String pathXsd;
        private AtosLog atosLog;
        private String target;
        private String errores;

        public AtosLog Log
        {
            get
            {
                return this.atosLog;
            }
            set
            {
                this.atosLog = value;
            }
        }

        public String Errores
        {
            get
            {
                return errores;
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

        /*public AtosXml()
        {
            this.pathXml = "";
            this.pathXsd = "";
            this.Target = "";
            this.errores = "";
            atosLog = new AtosLog();
        }*/
        /*public AtosXml(String _pathXml, String _pathXsd, String _target, String _pathLog, String _fileLog)
        {
            this.pathXml = _pathXml;
            this.pathXsd = _pathXsd;
            this.Target = _target;
            this.errores = "";
            atosLog = new AtosLog(_pathLog, _fileLog);
        }

        public AtosXml(String _pathXml, String _pathXsd, String _target, String _pathLog, String _fileLog, Boolean _log)
        {
            this.pathXml = _pathXml;
            this.pathXsd = _pathXsd;
            this.Target = _target;
            this.errores = "";
            atosLog = new AtosLog(_pathLog, _fileLog, _log);
        }

        public AtosXml(String _pathXml, String _pathXsd, String _target, String _pathLog, String _fileLog, ITracingService _traza)
        {
            this.pathXml = _pathXml;
            this.pathXsd = _pathXsd;
            this.Target = _target;
            this.errores = "";
            atosLog = new AtosLog(_pathLog, _fileLog, _traza);
        }

        public AtosXml(String _pathXml, String _pathXsd, String _target, String _pathLog, String _fileLog, Boolean _log, ITracingService _traza)
        {
            this.pathXml = _pathXml;
            this.pathXsd = _pathXsd;
            this.Target = _target;
            this.errores = "";
            atosLog = new AtosLog(_pathLog, _fileLog, _log, _traza);
        }*/

        public AtosXml(ParametrosATR _parametrosATR, ITracingService _traza)
        {
            this.pathXml = _parametrosATR.Xml;
            this.pathXsd = _parametrosATR.Xsd;
            this.target = _parametrosATR.Target;
            this.errores = "";
            atosLog = new AtosLog(_parametrosATR.LogPath, _parametrosATR.Log, _parametrosATR.HayLog, _traza);
        }

        public String documentRootName(String _xml)
        {
            String _rootName;
            XDocument _document = XDocument.Load(pathXml + _xml);
            _rootName = _document.Root.Name.LocalName;
            return _rootName;
        }

        /*
        public bool validateXml(String _xsd, String _xml)
        {
            return validateXml(_xsd, _xml, pathXml);
        }
        */


        public bool validateXml(String _xsd, String _xml)
        {
            atosLog.writeLog("Xsd: " + _xsd + " Xml: " + _xml + " PathXml: " + pathXml);
            XmlSchemaSet _schemas = new XmlSchemaSet();
            XmlSchema _schema = _schemas.Add(target, pathXsd + _xsd);
            XDocument _document = XDocument.Load(pathXml + _xml);
            atosLog.writeLog("Cargado documento en XDocument");

            bool _errors = false;
            String _strErrors = "";
            errores = "";
            // _strErrors = _document.Root.Name + "\r\n";
            _document.Validate(_schemas, (o, e) =>
            {
                _strErrors = _strErrors + e.Message + "\r\n";
                _errors = true;
            });

            if (_errors)
            {
                atosLog.writeLog("\r\n-----------------------\r\n");
                atosLog.writeLog("Errores en validateXml\r\n");
                atosLog.writeLog(_strErrors);
                atosLog.writeLog("=======================\r\n");
                errores = _strErrors;
            }

            return (_errors == false);
        }



        public Object xml2class(Type _type, String _filexml)
        {

            XmlSerializer _serializer = new XmlSerializer(_type);
            StreamReader _streamReader = new StreamReader(pathXml + _filexml);
            
            Object _ret = _serializer.Deserialize(_streamReader);
            _streamReader.Close();
            return _ret;
        }

        public void class2xml(Type _type, Object _object, String _filexml)
        {
            //Create our own namespaces for the output
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

            //Add an empty namespace and empty value
            ns.Add("", "http://localhost/elegibilidad");

            XmlSerializer _serializer = new XmlSerializer(_type);
            using (StreamWriter _stream = new StreamWriter(pathXml + _filexml))
                _serializer.Serialize(_stream, _object, ns);
        }

    }
}
