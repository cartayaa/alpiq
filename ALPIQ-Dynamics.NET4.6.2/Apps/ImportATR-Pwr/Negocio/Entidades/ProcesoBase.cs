using System;
using System.Collections.Generic;
using Negocio.Constantes;
using Microsoft.Xrm.Sdk;
using System.Xml;
using System.IO;
using Negocio.Logica;

namespace Negocio.Entidades
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
        protected IOrganizationService _servicioCRM;
        protected GestionSolicitudesATR _gestorSolicitud;
        protected Operaciones _operaciones;
        #endregion

        public List<string> NoResultado { get { return new List<string>(); } }
        protected string FicheroXsd
        {
            get
            {
                return _xDoc.DocumentElement.Name.Replace("Mensaje", string.Empty) + ".xsd";
            }
        }
        #region Miembros para Generacion de Xml
        protected ParametrosATR _parametrosATR;
        protected ITracingService _tracingService;
        protected string _nombreXml;
        protected Entity _datos01;
        protected AtosXml _atosXml;
        #endregion

        /// <summary>
        /// Constructor de clase que representa un proceso que sus pasos
        /// </summary>
        /// <param name="nombreFichero">Nombre del fichero xml sin la ruta para los mensajes de errores</param>
        /// <param name="log">Lista de string para capturar los errores que ocurren al gestionar el proceso y el paso</param>
        /// <param name="servicio">Servicio de trazas del framework CRM</param>
        /// <param name="xDoc">Clase XmlDocument que contiene el fichero xml del proceso y paso siendo tratado</param>
        /// <param name="gestor">Clase GestionSolicitudesATR que contiene la logica para parsear o procesar el fichero xml en cada paso del proceso</param>
        /// <param name="operaciones">Clase estatica que encapsula llamadas de GestionSolicitudesATR necesarias para procesar un paso</param>
        public ProcesoBase(string nombreFichero, List<String> log, IOrganizationService servicio, XmlDocument xDoc, GestionSolicitudesATR gestor, Operaciones operaciones)
        {
            _nombreFichero = nombreFichero;
            _rutaFichero = xDoc.BaseURI.Replace(@"file:///", string.Empty);
            _xDoc = xDoc;
            _mensajeATR = _xDoc.FirstChild.NextSibling;  //GetElementsByTagName("MensajeAceptacionModificacionDeATR");
            _log = log;
            _rechazo = _mensajeATR.Name.ToUpper().Contains("RECHAZO");
            _agente = _mensajeATR.Attributes["AgenteSolicitante"] == null ? null : _mensajeATR.Attributes["AgenteSolicitante"].InnerText;
            _proceso = _xDoc.GetElementsByTagName("CodigoDelProceso")[0].InnerText;
            _paso = _xDoc.GetElementsByTagName("CodigoDePaso")[0].InnerText;

            _servicioCRM = servicio;
            _gestorSolicitud = gestor;
            _operaciones = operaciones;
        }
        /// <summary>
        /// Constructor de clase que representa un proceso que sus pasos
        /// </summary>
        /// <param name="proceso">Proceso actual del fichero xml</param>
        /// <param name="paso">Paso actual del fichero xml</param>
        /// <param name="parametrosATR">Informacion agrupada del fichero xml procesado</param>
        /// <param name="tracingService">Servicio de trazas del framework CRM</param>
        /// <param name="nombreXml">Nombre del fichero xml sin la ruta para los mensajes de errores</param>
        /// <param name="datos01">Entidad CRM</param>
        /// <param name="operaciones">Clase estatica que encapsula llamadas de GestionSolicitudesATR necesarias para procesar un paso</param>
        public ProcesoBase(string proceso, string paso, ParametrosATR parametrosATR, ITracingService tracingService, string nombreXml, Entity datos01, Operaciones operaciones)
        {
            _proceso = proceso;
            _paso = paso;
            _parametrosATR = parametrosATR;
            _tracingService = tracingService;
            _nombreXml = nombreXml;
            _datos01 = datos01;
            _operaciones = operaciones;
        }
        /// <summary>
        /// Metodo virtual a ser sobre-escrito que llamará a clase con la logica para procesar el fichero xml en el paso 1
        /// </summary>
        /// <param name="entidadCRM">entidad CRM que contiene datos o contendrá del paso en cuestion</param>
        /// <returns>Lista de string que contiene errores que hayan ocurrido durante el procesado del fichero xml</returns>
        public virtual void ProcesarPaso01(Entity entidadCRM) { }
        /// <summary>
        /// Metodo virtual a ser sobre-escrito que llamará a clase con la logica para procesar el fichero xml en el paso 2
        /// </summary>
        /// <param name="entidadCRM">entidad CRM que contiene datos o contendrá del paso en cuestion</param>
        /// <returns>Lista de string que contiene errores que hayan ocurrido durante el procesado del fichero xml</returns>
        public virtual void ProcesarPaso02(Entity entidadCRM) { }
        /// <summary>
        /// Metodo virtual a ser sobre-escrito que llamará a clase con la logica para procesar el fichero xml en el paso 3
        /// </summary>
        /// <param name="entidadCRM">entidad CRM que contiene datos o contendrá del paso en cuestion</param>
        /// <returns>Lista de string que contiene errores que hayan ocurrido durante el procesado del fichero xml</returns>
        public virtual void ProcesarPaso03(Entity entidadCRM) { }
        /// <summary>
        /// Metodo virtual a ser sobre-escrito que llamará a clase con la logica para procesar el fichero xml en el paso 4
        /// </summary>
        /// <param name="entidadCRM">entidad CRM que contiene datos o contendrá del paso en cuestion</param>
        /// <returns>Lista de string que contiene errores que hayan ocurrido durante el procesado del fichero xml</returns>
        public virtual void ProcesarPaso04(Entity entidadCRM) { }
        /// <summary>
        /// Metodo virtual a ser sobre-escrito que llamará a clase con la logica para procesar el fichero xml en el paso 5
        /// </summary>
        /// <param name="entidadCRM">entidad CRM que contiene datos o contendrá del paso en cuestion</param>
        /// <returns>Lista de string que contiene errores que hayan ocurrido durante el procesado del fichero xml</returns>
        public virtual void ProcesarPaso05(Entity entidadCRM) { }
        /// <summary>
        /// Metodo virtual a ser sobre-escrito que llamará a clase con la logica para procesar el fichero xml en el paso 6
        /// </summary>
        /// <param name="entidadCRM">entidad CRM que contiene datos o contendrá del paso en cuestion</param>
        /// <returns>Lista de string que contiene errores que hayan ocurrido durante el procesado del fichero xml</returns>
        public virtual void ProcesarPaso06(Entity entidadCRM) { }
        /// <summary>
        /// Metodo virtual a ser sobre-escrito que llamará a clase con la logica para procesar el fichero xml en el paso 7
        /// </summary>
        /// <param name="entidadCRM">entidad CRM que contiene datos o contendrá del paso en cuestion</param>
        /// <returns>Lista de string que contiene errores que hayan ocurrido durante el procesado del fichero xml</returns>
        public virtual void ProcesarPaso07(Entity entidadCRM) { }
        /// <summary>
        /// Metodo virtual a ser sobre-escrito que llamará a clase con la logica para procesar el fichero xml en el paso 8
        /// </summary>
        /// <param name="entidadCRM">entidad CRM que contiene datos o contendrá del paso en cuestion</param>
        /// <returns>Lista de string que contiene errores que hayan ocurrido durante el procesado del fichero xml</returns>
        public virtual void ProcesarPaso08(Entity entidadCRM) { }
        /// <summary>
        /// Metodo virtual a ser sobre-escrito que llamará a clase con la logica para procesar el fichero xml en el paso 9
        /// </summary>
        /// <param name="entidadCRM">entidad CRM que contiene datos o contendrá del paso en cuestion</param>
        /// <returns>Lista de string que contiene errores que hayan ocurrido durante el procesado del fichero xml</returns>
        public virtual void ProcesarPaso09(Entity entidadCRM) { }
        /// <summary>
        /// Metodo virtual a ser sobre-escrito que llamará a clase con la logica para procesar el fichero xml en el paso 10
        /// </summary>
        /// <param name="entidadCRM">entidad CRM que contiene datos o contendrá del paso en cuestion</param>
        /// <returns>Lista de string que contiene errores que hayan ocurrido durante el procesado del fichero xml</returns>
        public virtual void ProcesarPaso10(Entity entidadCRM) { }
        /// <summary>
        /// Metodo virtual a ser sobre-escrito que llamará a clase con la logica para procesar el fichero xml en el paso 11
        /// </summary>
        /// <param name="entidadCRM">entidad CRM que contiene datos o contendrá del paso en cuestion</param>
        /// <returns>Lista de string que contiene errores que hayan ocurrido durante el procesado del fichero xml</returns>
        public virtual void ProcesarPaso11(Entity entidadCRM) { }
        /// <summary>
        /// Metodo virtual a ser sobre-escrito que llamará a clase con la logica para procesar el fichero xml en el paso 12
        /// </summary>
        /// <param name="entidadCRM">entidad CRM que contiene datos o contendrá del paso en cuestion</param>
        /// <returns>Lista de string que contiene errores que hayan ocurrido durante el procesado del fichero xml</returns>
        public virtual void ProcesarPaso12(Entity entidadCRM) { }
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
        /// Metodo que llama el metodo correspondiente para procesar el paso precisando el paso como parametro
        /// </summary>
        /// <param name="entidadCRM">entidad CRM que contiene datos o contendrá del paso en cuestion</param>
        /// <param name="paso">El paso a ser procesado</param>
        /// <returns>Valor que indica si el paso ha sido procesado con exito</returns>
        public ProcesoResultado EjecutarPaso(Entity entidadCRM, string paso)
        {
            ProcesoResultado resultado = new ProcesoResultado();
            switch (paso)
            {
                case Paso.N01: ProcesarPaso01(entidadCRM); break;
                case Paso.N02: ProcesarPaso02(entidadCRM); break;
                case Paso.N03: ProcesarPaso03(entidadCRM); break;
                case Paso.N04: ProcesarPaso04(entidadCRM); break;
                case Paso.N05: ProcesarPaso05(entidadCRM); break;
                case Paso.N06: ProcesarPaso06(entidadCRM); break;
                case Paso.N07: ProcesarPaso07(entidadCRM); break;
                case Paso.N08: ProcesarPaso08(entidadCRM); break;
                case Paso.N09: ProcesarPaso09(entidadCRM); break;
                case Paso.N10: ProcesarPaso10(entidadCRM); break;
                case Paso.N11: ProcesarPaso11(entidadCRM); break;
                case Paso.N12: ProcesarPaso12(entidadCRM); break;
            }

            Guid solicitudId = Guid.Empty; 
            solicitudId = GestionSolicitudesATR.UpsertSolicitudATR(entidadCRM);
            if (solicitudId == Guid.Empty && entidadCRM.Attributes.Count > 0)
                throw new Exception(string.Format(Mensajes.ErrorCreacionSolicitud, _proceso, _paso));
            else
                resultado.SolicitudId = solicitudId;
            return resultado;
        }
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
        public void AdjuntarXml(Guid solicitudId,string rutaFichero ,string nombreFichero)
        {
            if (solicitudId != Guid.Empty)
            {
                FileStream _stream = File.OpenRead(rutaFichero + "/" + nombreFichero);
                byte[] _bData = new byte[_stream.Length];
                _stream.Read(_bData, 0, _bData.Length);
                _stream.Close();
                string encodedData = System.Convert.ToBase64String(_bData);

                Logica.GestionSolicitudesATR.adjuntarXmlSolicitud(solicitudId, _paso, _proceso, _nombreFichero, encodedData);
            }
        }
    }
}
