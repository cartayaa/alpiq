using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Comun.Negocio.Constantes;
using Microsoft.Xrm.Sdk;
using System.Xml;
using System.IO;
using Comun.Negocio.Logica;
using System.Xml.Schema;
using Comun.Negocio.Enums;
using Comun.Negocio.Utilidades;

namespace Comun.Negocio.Entidades
{
    public class ProcesoBase : IProceso
    {
        #region Miembros para Carga de Xml
        protected bool _rechazo;
        protected string _agente;
        protected string _proceso;
        protected string _nombreFichero;
        protected string _rutaFichero;

        protected XmlDocument _xDoc = null;
        protected XmlNode _mensajeATR = null;
        protected List<String> _log = null;

        protected string _paso;
        protected Operaciones _operaciones;
        #endregion

        protected string FicheroXsd
        {
            get
            {
                return _xDoc.DocumentElement.Name.Replace("Mensaje", string.Empty) + ".xsd";
            }
        }
        public TipoEntidad TipoEntidad { get; set; }
        /// <summary>
        /// Constructor de clase que representa un proceso que sus pasos
        /// </summary>
        /// <param name="nombreFichero">Nombre del fichero xml sin la ruta para los mensajes de errores</param>
        /// <param name="log">Lista de string para capturar los errores que ocurren al gestionar el proceso y el paso</param>
        /// <param name="servicio">Servicio de trazas del framework CRM</param>
        /// <param name="xDoc">Clase XmlDocument que contiene el fichero xml del proceso y paso siendo tratado</param>
        /// <param name="operaciones">Clase estatica que encapsula llamadas de GestionSolicitudesATR y GestionFacturas necesarias para procesar un paso</param>
        public ProcesoBase(string nombreFichero, List<String> log, XmlDocument xDoc, Operaciones operaciones, TipoEntidad tipoEntidad)
        {
            _nombreFichero = nombreFichero;
            _rutaFichero = xDoc.BaseURI.Replace(@"file:///", string.Empty);
            _xDoc = xDoc;
            _mensajeATR = _xDoc.FirstChild.NextSibling;
            _log = log;
            _rechazo = _mensajeATR.Name.ToUpper().Contains("RECHAZO");
            _agente = _mensajeATR.Attributes["AgenteSolicitante"] == null ? null : _mensajeATR.Attributes["AgenteSolicitante"].InnerText; // TODO: TBR?
            _proceso = _xDoc.GetElementsByTagName(Configuracion.XmlCodigoProceso)[0].InnerText;
            _paso = _xDoc.GetElementsByTagName(Configuracion.XmlCodigoPaso)[0].InnerText;

            _operaciones = operaciones;
            TipoEntidad = tipoEntidad;
        }
        /// <summary>
        /// Metodo que llama el metodo correspondiente para procesar el paso sin precisar el paso como parametro
        /// </summary>
        /// <param name="entidadCRM">entidad CRM que contiene datos o contendrá del paso en cuestion</param>
        /// <returns>Valor que indica si el paso ha sido procesado con exito</returns>
        public ProcesoResultado EjecutarPaso(Entity entidadCRM)
        {
            return EjecutarPaso(entidadCRM, _paso);
        }
        /// <summary>
        /// Metodo que llama al metodo a ser implementado por la clase heredera para procesar el paso precisando el paso como parametro
        /// </summary>
        /// <param name="entidadCRM">entidad CRM que contiene datos o contendrá del paso en cuestion</param>
        /// <param name="paso">El paso a ser procesado</param>
        /// <returns>Valor que indica si el paso ha sido procesado con exito</returns>
        public ProcesoResultado EjecutarPaso(Entity entidadCRM, string paso)
        {
            ProcesarPaso(entidadCRM, paso);
            return Guardar(entidadCRM);
        }

        public ProcesoResultado Guardar(Entity entidadCRM)
        {
            return CrmHelper.GuardarEntiad(_operaciones.ServicioCrm, entidadCRM, _proceso, _paso);
        }
        /// <summary>
        /// Metodo a ser implementado por la clase heredera que llama paso correspondiente, precisando el paso como parametro
        /// </summary>
        /// <param name="entidadCRM">entidad CRM que contiene datos o contendrá del paso en cuestion</param>
        /// <param name="paso">El paso a ser procesado</param>
        /// <returns>Valor que indica si el paso ha sido procesado con exito</returns>
        public virtual void ProcesarPaso(Entity entidadCRM, string paso) { }
        /// <summary>
        /// Valida si el fichero procesado es xml
        /// </summary>
        /// <returns>Resultado de la validacion</returns>
        public bool ValidarFichero()
        {
            return _mensajeATR != null;
        }
        /// <summary>
        /// Valida si el fichero procesado contiene datos que corresponden a un paso
        /// </summary>
        /// <returns>Resultado de la validacion</returns>
        public bool ValidarMensaje()
        {
            return !string.IsNullOrEmpty(_proceso) && !string.IsNullOrEmpty(_paso);
        }
        public bool ValidarEsquema()
        {
            bool result = true;
            string xsdPath = @"C:\Users\A648190\Documents\Atos\Acciona CRM\Funcional\CNMC - E - V1.0 - 2016.12.20\CNMC - E - Anexos 2016.12.20\CNMC - E - XSD 2016.12.20\" + FicheroXsd;
            _xDoc.Schemas.Add("http://localhost/elegibilidad", xsdPath);

            _xDoc.Validate((o, e) =>
            {
                string error = e.Message;
                result = false;

            });
            return result;
        }
        /// <summary>
        /// Adjunta el fichero xml del paso procesado a la entidad CRM
        /// </summary>
        /// <param name="entidadCRM">entidad CRM que contiene datos o contendrá del paso en cuestion</param>
        public void AdjuntarXml(Guid solicitudId, string nombreEntidad)
        {
            if (solicitudId != Guid.Empty)
            {
                FileStream _stream = File.OpenRead(_rutaFichero);
                byte[] _bData = new byte[_stream.Length];
                _stream.Read(_bData, 0, _bData.Length);
                _stream.Close();
                string encodedData = System.Convert.ToBase64String(_bData);

                Logica.GestionSolicitudes.AdjuntarXmlSolicitud(_operaciones.ServicioCrm, solicitudId, _paso, _proceso, _nombreFichero, encodedData, nombreEntidad);
            }
        }
    }
}
