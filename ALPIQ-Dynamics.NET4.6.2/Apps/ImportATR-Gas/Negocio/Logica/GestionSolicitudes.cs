using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Web.Services.Protocols;
using System.Xml;
using Negocio.Entidades;
using Negocio.Utilidades;

namespace Negocio.Logica
{
    public class GestionSolicitudes : NegocioBase
    {
        public GestionSolicitudes(IOrganizationService servicioCrm) :
            base(servicioCrm)
        {
        }

        public void AsignarSolicitudAsociada(XmlDocument xDoc, Entity entidadCrm)
        {
            try
            {
                List<string> errores = new List<string>();
                XmlElement datosCabecera = (XmlElement)xDoc.GetElementsByTagName("Cabecera").Item(0);

                string paso = datosCabecera.GetElementsByTagName("CodigoDePaso")[0].InnerText;
                string proceso = datosCabecera.GetElementsByTagName("CodigoDelProceso")[0].InnerText;
                string codigoSolicitud = datosCabecera.GetElementsByTagName("CodigoDeSolicitud")[0].InnerText;
                string pasoSolicitud = obtenerPasoParaSolicitudAsociada(paso);

                Guid procesoId = CrmHelper.ObtenerIdEntidad(ServicioCrm, "atos_procesoatr", "atos_procesoatrid", "atos_codigoproceso", proceso);
                Guid pasoId = CrmHelper.ObtenerIdEntidad(ServicioCrm, "atos_pasoatr", "atos_pasoatrid", new string[] { "atos_codigopaso", "atos_procesoatrid" }, new object[] { pasoSolicitud, procesoId });
                Guid solicitudId = CrmHelper.ObtenerIdEntidad(ServicioCrm, "atos_solicitudatr", "atos_solicitudatrid",
                    new string[] { "atos_pasoatrid", "atos_procesoatrid", "atos_codigosolicitud" },
                    new object[] { pasoId, procesoId, codigoSolicitud });
                if (pasoId != Guid.Empty && procesoId != Guid.Empty && solicitudId != Guid.Empty)
                    entidadCrm.Attributes.Add("atos_solicitudasociadaid", new EntityReference("atos_solicitudatr", solicitudId));
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "asignarSolicitudAsociada", ex);
                //throw new Exception(message, ex);
            }
        }

        private string obtenerPasoParaSolicitudAsociada(string paso)
        {
            switch (paso)
            {
                //case Constantes.Paso.N05: paso = Constantes.Paso.N01; break;
                default: paso = string.Format("{0:D2}", int.Parse(paso) - 1); break;
            }
            return paso;
        }
    
        internal void ProcesarPasoA102(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a102")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "comreferencenum", "atos_comreferencenum", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "reqdate", "atos_reqdate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "reqhour", "atos_reqdate", fecha, null);
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadCrm, "fecconformidadcliente", "atos_fecconformidadcliente", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "titulartype", "atos_titulartype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "TIPO PERSONA" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "nationality", "atos_nationality", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "NACIONALIDAD" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "documenttype", "atos_documenttype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "TIPO DOCUMENTO" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "documentnum", "atos_documentnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadCrm, "reqqd", "atos_reqqd", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadCrm, "reqestimatedqa", "atos_reqestimatedqa", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "modeffectdate", "atos_modeffectdate", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "MODELO FECHA EFECTO" }, null);
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadCrm, "reqtransferdate", "atos_reqtransferdate", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "disconnectedserviceaccepted", "atos_disconnectedserviceaccepted", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "INDICATIVO SI/NO" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA102", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA202(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a202")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "comreferencenum", "atos_comreferencenum", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "responsedate", "atos_responsedate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "responsehour", "atos_responsedate", fecha, null);
                fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadCrm, "fecconformidadcliente", "atos_fecconformidadcliente", null).Fecha;
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "result", "atos_result", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultdesc", "atos_resultdesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultreason", "atos_resultreason", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivos de Rechazo" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultreasondesc", "atos_resultreasondesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "MOTIVO RECHAZO OCSUM" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "nationality", "atos_nationality", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "NACIONALIDAD" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "documenttype", "atos_documenttype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "TIPO DOCUMENTO" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "documentnum", "atos_documentnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "tolltype", "atos_tolltype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "TIPO PEAJE" }, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadCrm, "qdgranted", "atos_qdgranted", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadCrm, "outgoingpressuregranted", "atos_outgoingpressuregranted", null);
                CrmHelper.ProcesarCampoOptionSet(ServicioCrm, xElem, entidadCrm, "singlenomination", "atos_singlenomination", null); // 300.000.000 S, N, A
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "netsituation", "atos_netsituation", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "newmodeffectdate", "atos_newmodeffectdate", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "MODELO FECHA EFECTO PREVISTA" }, null);
                fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadCrm, "foreseentransferdate", "atos_foreseentransferdate", null).Fecha;
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "StatusPS", "atos_statusps", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "ESTADO PUNTO DE SUMINISTRO" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);

            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA202", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA2S02(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a2s02")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "responsedate", "atos_responsedate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "responsehour", "atos_responsedate", fecha, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "nationality", "atos_nationality", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Nacionalidad" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "documenttype", "atos_documenttype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Documento" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "documentnum", "atos_documentnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadCrm, "foreseentransferdate", "atos_foreseentransferdate", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA2S02", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA302(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a302")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "comreferencenum", "atos_comreferencenum", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "responsedate", "atos_responsedate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "responsehour", "atos_responsedate", fecha, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "titulartype", "atos_titulartype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Persona" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "nationality", "atos_nationality", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Nacionalidad" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "documenttype", "atos_documenttype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Documento" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "documentnum", "atos_documentnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadCrm, "lastinspectionsdate", "atos_lastinspectionsdate", null).Fecha;
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "lastinspectionsresult", "atos_lastinspectionsresult", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado Inspección" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "atrcode", "atos_atrcode", null);
                fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadCrm, "transfereffectivedate", "atos_transfereffectivedate", null).Fecha;
                CrmHelper.ProcesarCampoBuleano(ServicioCrm, xElem, entidadCrm, "telemetering", "atos_telemetering", "Sí", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadCrm, "finalclientyearlyconsumption", "atos_finalclientyearlyconsumption", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "readingtype", "atos_readingtype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Lectura" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "gasusetype", "atos_gasusetype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla", "atos_procesotiposdeuso" }, new object[] { "Tipos de Uso del Gas", xDoc.GetElementsByTagName("processcode")[0].InnerText }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "caecode", "atos_caecode", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "canonircperiodicity", "atos_canonircperiodicity", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Periodicidad Canon IRC" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "StatusPS", "atos_statusps", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Estado Punto de Suministro" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "lectureperiodicity", "atos_lectureperiodicity", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Periodicidad Lectura" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);

                ProcesarContadores(nombreFichero, xElem, entidadCrm, rechazo, agente);
                ProcesarCorrectores(nombreFichero, xElem, entidadCrm, rechazo, agente);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA302", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarContadores(string nombreFichero, XmlElement xElem, Entity entidadCrm, bool rechazo, string agente)
        {
            try
            {
                string hijosXml = "counter";
                string entidadHijo = "atos_contadoratrgas";
                string relacionN1 = "atos_atos_solicitudatrgas_atos_contadoratrgas_solicitudatrgasid";
                CrmHelper.ProcesarRegistrosN(ServicioCrm, nombreFichero, xElem, entidadCrm, agente, relacionN1, hijosXml, entidadHijo, GestionRegistrosN.ProcesarContadador);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xElem.OwnerDocument, "ProcesarContardores", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarCorrectores(string nombreFichero, XmlElement xElem, Entity entidadCrm, bool rechazo, string agente)
        {
            try
            {
                string hijosXml = "corrector";
                string entidadHijo = "atos_correctoratrgas";
                string relacionN1 = "atos_atos_solicitudatrgas_atos_correctoratrgas_solicitudatrgasid";
                CrmHelper.ProcesarRegistrosN(ServicioCrm, nombreFichero, xElem, entidadCrm, agente, relacionN1, hijosXml, entidadHijo, GestionRegistrosN.ProcesarCorrector);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xElem.OwnerDocument, "ProcesarCorrectores", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA3S02(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a3s02")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "responsedate", "atos_responsedate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "responsehour", "atos_responsedate", fecha, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "nationality", "atos_nationality", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Nacionalidad" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "documenttype", "atos_documenttype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Documento" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "documentnum", "atos_documentnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "previousatrcode", "atos_previousatrcode", null);
                fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadCrm, "transfereffectivedate", "atos_transfereffectivedate", null).Fecha;
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "readingtype", "atos_readingtype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Lectura" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);

                ProcesarContadores(nombreFichero, xElem, entidadCrm, rechazo, agente);
                ProcesarCorrectores(nombreFichero, xElem, entidadCrm, rechazo, agente);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA3S02", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA402(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a402")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "comreferencenum", "atos_comreferencenum", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "responsedate", "atos_responsedate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "responsehour", "atos_responsedate", fecha, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "result", "atos_result", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultdesc", "atos_resultdesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultreason", "atos_resultreason", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivos de Rechazo" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultreasondesc", "atos_resultreasondesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivos de Rechazo" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "titulartype", "atos_titulartype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Persona" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "nationality", "atos_nationality", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Nacionalidad" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "documenttype", "atos_documenttype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Documento" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "documentnum", "atos_documentnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA402", ex);
                throw new Exception(message, ex);
            }
        }

        private void procesarCUPS(IOrganizationService ServicioCrm, Entity entidadCrm)
        {
            if (!entidadCrm.Attributes.Contains("atos_cups") || entidadCrm.Attributes["atos_cups"] == null) return;

            procesarCUPS(ServicioCrm, entidadCrm, entidadCrm.Attributes["atos_cups"].ToString());
        }

        private void procesarCUPS(IOrganizationService ServicioCrm, Entity entidadCrm, string cups)
        {
            try
            {
                Guid instalacionId = CrmHelper.ObtenerIdEntidad(ServicioCrm, "atos_instalaciongas", "atos_instalaciongasid", "atos_cups20", cups);
                if(instalacionId != Guid.Empty)
                    entidadCrm.Attributes.Add("atos_instalaciongasid", new EntityReference("atos_instalaciongas", instalacionId));
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el CUPS @procesarCUPS", ex);
            }
        }

        internal void ProcesarPasoA4S02(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a4s02")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "responsedate", "atos_responsedate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "responsehour", "atos_responsedate", fecha, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "nationality", "atos_nationality", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Nacionalidad" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "documenttype", "atos_documenttype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Documento" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "documentnum", "atos_documentnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "previousatrcode", "atos_previousatrcode", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "result", "atos_result", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultdesc", "atos_resultdesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultreason", "atos_resultreason", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivos de Rechazo" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultreasondesc", "atos_resultreasondesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivos de Rechazo" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA4S02", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA104(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a104")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "comreferencenum", "atos_comreferencenum", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "reqdate", "atos_reqdate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "reqhour", "atos_reqdate", fecha, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "titulartype", "atos_titulartype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Persona" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "nationality", "atos_nationality", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Nacionalidad" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "documenttype", "atos_documenttype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Documento" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "documentnum", "atos_documentnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "cancelreason", "atos_cancelreason", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivo Baja" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "modeffectdate", "atos_modeffectdate", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Modelo Fecha Efecto" }, null);
                fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadCrm, "reqcanceldate", "atos_reqcanceldate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "cancelhour", "atos_reqcanceldate", fecha, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "contactphonenumber", "atos_contactphonenumber", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA104", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA204(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a204")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "comreferencenum", "atos_comreferencenum", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "responsedate", "atos_responsedate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "responsehour", "atos_responsedate", fecha, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "result", "atos_result", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultdesc", "atos_resultdesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultreason", "atos_resultreason", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivos de Rechazo" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultreasondesc", "atos_resultreasondesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivos de Rechazo" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "nationality", "atos_nationality", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Nacionalidad" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "documenttype", "atos_documenttype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Documento" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "documentnum", "atos_documentnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "cancelreason", "atos_cancelreason", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivo Baja" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "newmodeffectdate", "atos_newmodeffectdate", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Modelo Fecha Efecto Prevista" }, null);
                fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadCrm, "foreseentransferdate", "atos_foreseentransferdate", null).Fecha;
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA204", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA2504(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a2504")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "comreferencenum", "atos_comreferencenum", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "visitdate", "atos_visitdate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "visithour", "atos_visitdate", fecha, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "informationtype", "atos_informationtype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo de Información-Incidencia" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "informationtypedesc", "atos_informationtypedesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo de Información - Incidencia" }, null);
                fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "interventiondate", "atos_interventiondate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "interventionhourfrom", "atos_interventionhourfrom", fecha, null);
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "interventionhourto", "atos_interventionhourto", fecha, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                CrmHelper.ProcesarCampoEntero(ServicioCrm, xElem, entidadCrm, "visitnumber", "atos_visitnumber", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "operationnum", "atos_operationnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);
                CrmHelper.ProcesarCampoEntero(ServicioCrm, xElem, entidadCrm, "conceptnumber", "atos_conceptnumber", null);

                ProcesarConceptosFacturacion(nombreFichero, xElem, entidadCrm, rechazo, agente);
                ProcesarAnomalias(nombreFichero, xElem, entidadCrm, rechazo, agente);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA2504", ex);
                throw new Exception(message, ex);
            }
        }
        internal void ProcesarConceptosFacturacion(string nombreFichero, XmlElement xElem, Entity entidadCrm, bool rechazo, string agente)
        {
            try
            {
                string hijosXml = "concept";
                string entidadHijo = "atos_conceptofacturacionatrgas";
                string relacionN1 = "atos_atos_solicitudatrgas_atos_conceptofacturacionatrgas_solicitudatrgasid";
                CrmHelper.ProcesarRegistrosN(ServicioCrm, nombreFichero, xElem, entidadCrm, agente, relacionN1, hijosXml, entidadHijo, GestionRegistrosN.ProcesarConceptoFacturacion);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xElem.OwnerDocument, "ProcesarConceptoFacturacion", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarAnomalias(string nombreFichero, XmlElement xElem, Entity entidadCrm, bool rechazo, string agente)
        {
            try
            {
                string hijosXml = "defect";
                string entidadHijo = "atos_anomaliaatrgas";
                string relacionN1 = "atos_atos_solicitudatrgas_atos_anomaliaatrgas_solicitudatrgasid";
                CrmHelper.ProcesarRegistrosN(ServicioCrm, nombreFichero, xElem, entidadCrm, agente, relacionN1, hijosXml, entidadHijo, GestionRegistrosN.ProcesarAnomalia);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xElem.OwnerDocument, "ProcesarAnomalia", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA304(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a304")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "comreferencenum", "atos_comreferencenum", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "responsedate", "atos_responsedate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "responsehour", "atos_responsedate", fecha, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "titulartype", "atos_titulartype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Persona" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "nationality", "atos_nationality", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Nacionalidad" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "documenttype", "atos_documenttype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Documento" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "documentnum", "atos_documentnum", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "result", "atos_result", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultdesc", "atos_resultdesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "activationtype", "atos_activationtype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo de Activación" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "activationtypedesc", "atos_activationtypedesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo de Activación" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "atrcode", "atos_atrcode", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "operationnum", "atos_operationnum", null);
                CrmHelper.ProcesarCampoBuleano(ServicioCrm, xElem, entidadCrm, "moreinformation", "atos_moreinformation", "Sí", null);
                fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadCrm, "transfereffectivedate", "atos_transfereffectivedate", null).Fecha;
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);

                ProcesarContadores(nombreFichero, xElem, entidadCrm, rechazo, agente);
                ProcesarCorrectores(nombreFichero, xElem, entidadCrm, rechazo, agente);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA304", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA404(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a404")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "comreferencenum", "atos_comreferencenum", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "responsedate", "atos_responsedate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "responsehour", "atos_responsedate", fecha, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "titulartype", "atos_titulartype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Persona" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "nationality", "atos_nationality", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Nacionalidad" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "documenttype", "atos_documenttype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Documento" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "documentnum", "atos_documentnum", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "result", "atos_result", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultdesc", "atos_resultdesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultreason", "atos_resultreason", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivos de Rechazo" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultreasondesc", "atos_resultreasondesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivos de Rechazo" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "operationnum", "atos_operationnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);

                ProcesarDocumentos(nombreFichero, xElem, entidadCrm, rechazo, agente);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA404", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarDocumentos(string nombreFichero, XmlElement xElem, Entity entidadCrm, bool rechazo, string agente)
        {
            try
            {
                string hijosXml = "registerdoc";
                string entidadHijo = "atos_documentoatrgas";
                string relacionN1 = "atos_atos_solicitudatrgas_atos_documentoatrgas_solicitudatrgasid";
                CrmHelper.ProcesarRegistrosN(ServicioCrm, nombreFichero, xElem, entidadCrm, agente, relacionN1, hijosXml, entidadHijo, GestionRegistrosN.ProcesarDocumento);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xElem.OwnerDocument, "ProcesarRegisterDoc", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA105(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a105")[numeroMensaje];

            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA105", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA205(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a205")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "comreferencenum", "atos_comreferencenum", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "responsedate", "atos_responsedate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "responsehour", "atos_responsedate", fecha, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "result", "atos_result", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultdesc", "atos_resultdesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultreason", "atos_resultreason", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivos de Rechazo" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultreasondesc", "atos_resultreasondesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivos de Rechazo" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "nationality", "atos_nationality", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Nacionalidad" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "documenttype", "atos_documenttype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Documento" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "documentnum", "atos_documentnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "updatereason", "atos_updatereason", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivo Modificación" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "finaltolltypegranted", "atos_finaltolltypegranted", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Peaje" }, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadCrm, "qdgranted", "atos_qdgranted", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "newmodeffectdate", "atos_newmodeffectdate", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Modelo Fecha Efecto Prevista" }, null);
                fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadCrm, "foreseentransferdate", "atos_foreseentransferdate", null).Fecha;
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA205", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA2505(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a2505")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "visitdate", "atos_visitdate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "visithour", "atos_visitdate", fecha, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "comreferencenum", "atos_comreferencenum", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "informationtype", "atos_informationtype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo de Información-Incidencia" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "informationtypedesc", "atos_informationtypedesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo de Información-Incidencia" }, null);
                fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "interventiondate", "atos_interventiondate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "interventionhourfrom", "atos_interventionhourfrom", fecha, null);
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "interventionhourto", "atos_interventionhourto", fecha, null);
                CrmHelper.ProcesarCampoEntero(ServicioCrm, xElem, entidadCrm, "visitnumber", "atos_visitnumber", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "operationnum", "atos_operationnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);
                CrmHelper.ProcesarCampoEntero(ServicioCrm, xElem, entidadCrm, "conceptnumber", "atos_conceptnumber", null);

                ProcesarConceptosFacturacion(nombreFichero, xElem, entidadCrm, rechazo, agente);
                ProcesarAnomalias(nombreFichero, xElem, entidadCrm, rechazo, agente);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA2505", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA305(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a305")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "comreferencenum", "atos_comreferencenum", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "responsedate", "atos_responsedate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "responsehour", "atos_responsedate", fecha, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "result", "atos_result", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultdesc", "atos_resultdesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadCrm, "transfereffectivedate", "atos_transfereffectivedate", null).Fecha;
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "atrcode", "atos_atrcode", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "updatereason", "atos_updatereason", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivo Modificación" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "tolltype", "atos_tolltype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Peaje" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "nationality", "atos_nationality", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Nacionalidad" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "documenttype", "atos_documenttype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Documento" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "documentnum", "atos_documentnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "firstname", "atos_firstname", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "familyname1", "atos_familyname1", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "familyname2", "atos_familyname2", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "telephone", "atos_telephone", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "fax", "atos_fax", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "email", "atos_email", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "caecode", "atos_caecode", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadCrm, "qdgranted", "atos_qdgranted", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "provinceowner", "atos_provinceowner", "atos_provincia", "atos_provinciaid", "atos_codigoprovincia", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "cityowner", "atos_cityowner", "atos_municipio", "atos_municipioid", "atos_codigo", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "zipcodeowner", "atos_zipcodeowner", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "streettypeowner", "atos_streettypeowner", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Vía" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "streetowner", "atos_streetowner", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "streetnumberowner", "atos_streetnumberowner", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "portalowner", "atos_portalowner", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "staircaseowner", "atos_staircaseowner", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "floorowner", "atos_floorowner", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "doorowner", "atos_doorowner", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "titulartype", "atos_titulartype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Persona" }, null);
                CrmHelper.ProcesarCampoBuleano(ServicioCrm, xElem, entidadCrm, "regularaddress", "atos_regularaddress", "Sí", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);

                ProcesarContadores(nombreFichero, xElem, entidadCrm, rechazo, agente);
                ProcesarCorrectores(nombreFichero, xElem, entidadCrm, rechazo, agente);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA305", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA405(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a405")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "comreferencenum", "atos_comreferencenum", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "responsedate", "atos_responsedate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "responsehour", "atos_responsedate", fecha, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "result", "atos_result", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultdesc", "atos_resultdesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultreason", "atos_resultreason", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivos de Rechazo" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultreasondesc", "atos_resultreasondesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivos de Rechazo" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "updatereason", "atos_updatereason", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivo Modificación" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA405", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA238(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a2")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "comreferencenum", "atos_comreferencenum", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "responsedate", "atos_responsedate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "responsehour", "atos_responsedate", fecha, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "result", "atos_result", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultdesc", "atos_resultdesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultreason", "atos_resultreason", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivos de Rechazo" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultreasondesc", "atos_resultreasondesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivos de Rechazo" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "nationality", "atos_nationality", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Nacionalidad" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "documenttype", "atos_documenttype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Documento" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "documentnum", "atos_documentnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadCrm, "qdgranted", "atos_qdgranted", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadCrm, "qhgranted", "atos_qhgranted", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadCrm, "outgoingpressuregranted", "atos_outgoingpressuregranted", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadCrm, "clientyearlyconsumption", "atos_clientyearlyconsumption", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "tolltype", "atos_tolltype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Peaje" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "newmodeffectdate", "atos_newmodeffectdate", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Modelo Fecha Efecto Prevista" }, null);
                fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadCrm, "foreseentransferdate", "atos_foreseentransferdate", null).Fecha;
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);
                CrmHelper.ProcesarCampoEntero(ServicioCrm, xElem, entidadCrm, "conceptnumber", "atos_conceptnumber", null);

                ProcesarConceptosFacturacion(nombreFichero, xElem, entidadCrm, rechazo, agente);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA238", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA2538(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a25")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "visitdate", "atos_visitdate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "visithour", "atos_visitdate", fecha, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "comreferencenum", "atos_comreferencenum", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "informationtype", "atos_informationtype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo de Información-Incidencia" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "informationtypedesc", "atos_informationtypedesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo de Información-Incidencia" }, null);
                fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "interventiondate", "atos_interventiondate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "interventionhourfrom", "atos_interventionhourfrom", fecha, null);
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "interventionhourto", "atos_interventionhourto", fecha, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultinspection", "atos_resultinspection", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado Inspección" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultinspectiondesc", "atos_resultinspectiondesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado Inspección" }, null);
                CrmHelper.ProcesarCampoEntero(ServicioCrm, xElem, entidadCrm, "visitnumber", "atos_visitnumber", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "operationnum", "atos_operationnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);
                CrmHelper.ProcesarCampoEntero(ServicioCrm, xElem, entidadCrm, "conceptnumber", "atos_conceptnumber", null);

                ProcesarConceptosFacturacion(nombreFichero, xElem, entidadCrm, rechazo, agente);
                ProcesarAnomalias(nombreFichero, xElem, entidadCrm, rechazo, agente);

            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA2538", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA338(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a3")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "comreferencenum", "atos_comreferencenum", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "responsedate", "atos_responsedate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "responsehour", "atos_responsedate", fecha, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "result", "atos_result", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultdesc", "atos_resultdesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "activationtype", "atos_activationtype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo de Activacíon" }, null);
                //CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "activationtypedesc", "atos_activationtypedesc", null);
                fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "interventiondate", "atos_interventiondate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "interventionhour", "atos_interventiondate", fecha, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "canonircperiodicity", "atos_canonircperiodicity", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Periodicidad Canon IRC" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultinspection", "atos_resultinspection", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado Inspección" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultinspectiondesc", "atos_resultinspectiondesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado Inspección" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "nationality", "atos_nationality", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Nacionalidad" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "documenttype", "atos_documenttype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Documento" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "documentnum", "atos_documentnum", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "titulartype", "atos_titulartype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Persona" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "firstname", "atos_firstname", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "familyname1", "atos_familyname1", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "familyname2", "atos_familyname2", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "province", "atos_province", "atos_provincia", "atos_provinciaid", "atos_codigoprovincia", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "city", "atos_city", "atos_municipio", "atos_municipioid", "atos_codigo", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "zipcode", "atos_zipcode", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "streettype", "atos_streettype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Vía" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "street", "atos_street", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "streetnumber", "atos_streetnumber", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "portal", "atos_portal", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "staircase", "atos_staircase", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "floor", "atos_floor", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "door", "atos_door", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "atr", "atos_atrcode", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "operationnum", "atos_operationnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "netsituation", "atos_netsituation", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "lectureperiodicity", "atos_lectureperiodicity", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Periodicidad Lectura" }, null);
                CrmHelper.ProcesarCampoEntero(ServicioCrm, xElem, entidadCrm, "visitnumber", "atos_visitnumber", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "tolltype", "atos_tolltype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Peaje" }, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadCrm, "finalqdgranted", "atos_finalqdgranted", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadCrm, "finalqhgranted", "atos_finalqhgranted", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadCrm, "finalclientyearlyconsumption", "atos_finalclientyearlyconsumption", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);
                CrmHelper.ProcesarCampoEntero(ServicioCrm, xElem, entidadCrm, "conceptnumber", "atos_conceptnumber", null);

                ProcesarConceptosFacturacion(nombreFichero, xElem, entidadCrm, rechazo, agente);
                ProcesarContadores(nombreFichero, xElem, entidadCrm, rechazo, agente);
                ProcesarCorrectores(nombreFichero, xElem, entidadCrm, rechazo, agente);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA338", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA438(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a4")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "comreferencenum", "atos_comreferencenum", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "responsedate", "atos_responsedate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "responsehour", "atos_responsedate", fecha, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "result", "atos_result", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultdesc", "atos_resultdesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultreason", "atos_resultreason", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivos de Rechazo" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultreasondesc", "atos_resultreasondesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivos de Rechazo" }, null);
                fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "interventiondate", "atos_interventiondate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "interventionhour", "atos_interventiondate", fecha, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "nationality", "atos_nationality", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Nacionalidad" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "documenttype", "atos_documenttype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Documento" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "documentnum", "atos_documentnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "operationnum", "atos_operationnum", null);
                CrmHelper.ProcesarCampoEntero(ServicioCrm, xElem, entidadCrm, "visitnumber", "atos_visitnumber", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);

                ProcesarAnomalias(nombreFichero, xElem, entidadCrm, rechazo, agente);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA438", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA241(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a241")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "responsedate", "atos_responsedate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "responsehour", "atos_responsedate", fecha, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "comreferencenum", "atos_comreferencenum", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "result", "atos_result", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultdesc", "atos_resultdesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "RESULTADO" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultreason", "atos_resultreason", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivos de Rechazo" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultreasondesc", "atos_resultreasondesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "MOTIVO RECHAZO OCSUM" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "nationality", "atos_nationality", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "NACIONALIDAD" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "documenttype", "atos_documenttype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "TIPO DOCUMENTO" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "documentnum", "atos_documentnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "updatereason", "atos_updatereason", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivo de Modificación en un cambio de Comercializador" }, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadCrm, "reqqd", "atos_reqqd", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadCrm, "reqqh", "atos_reqqh", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadCrm, "reqestimatedqa", "atos_reqestimatedqa", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadCrm, "reqoutgoingpressure", "atos_reqoutgoingpressure", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "tolltype", "atos_tolltype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "TIPO PEAJE" }, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadCrm, "qdgranted", "atos_qdgranted", null);
                CrmHelper.ProcesarCampoOptionSet(ServicioCrm, xElem, entidadCrm, "singlenomination", "atos_singlenomination", null); // 300.000.000 S, N, A
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "netsituation", "atos_netsituation", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "newmodeffectdate", "atos_newmodeffectdate", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "MODELO FECHA EFECTO PREVISTA" }, null);
                fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "foreseentransferdate", "atos_foreseentransferdate", null).Fecha;
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "StatusPS", "atos_statusps", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "ESTADO PUNTO DE SUMINISTRO" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA241", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA2S41(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a2S41")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "responsedate", "atos_responsedate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "responsehour", "atos_responsedate", fecha, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "nationality", "atos_nationality", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "NACIONALIDAD" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "documenttype", "atos_documenttype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "TIPO DOCUMENTO" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "documentnum", "atos_documentnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadCrm, "foreseentransferdate", "atos_foreseentransferdate", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);

            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA2S41", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA2541(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a2541")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "visitdate", "atos_visitdate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "visithour", "atos_visitdate", fecha, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "comreferencenum", "atos_comreferencenum", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "informationtype", "atos_informationtype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo de Información-Incidencia" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "informationtypedesc", "atos_informationtypedesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "RESULTADO INTERVENCIÓNTIPO DE INFORMACIÓN-INCIDENCIA" }, null);
                fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "interventiondate", "atos_interventiondate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "interventionhourfrom", "atos_interventionhourfrom", fecha, null);
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "interventionhourto", "atos_interventionhourto", fecha, null);
                CrmHelper.ProcesarCampoEntero(ServicioCrm, xElem, entidadCrm, "visitnumber", "atos_visitnumber", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "operationnum", "atos_operationnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);
                CrmHelper.ProcesarCampoEntero(ServicioCrm, xElem, entidadCrm, "conceptnumber", "atos_conceptnumber", null);

                ProcesarConceptosFacturacion(nombreFichero, xElem, entidadCrm, rechazo, agente);
                ProcesarAnomalias(nombreFichero, xElem, entidadCrm, rechazo, agente);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA2541", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA341(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a341")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "comreferencenum", "atos_comreferencenum", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "responsedate", "atos_responsedate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "responsehour", "atos_responsedate", fecha, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "nationality", "atos_nationality", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "NACIONALIDAD" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "documenttype", "atos_documenttype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "TIPO DOCUMENTO" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "documentnum", "atos_documentnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "atrcode", "atos_atrcode", null);
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadCrm, "transfereffectivedate", "atos_transfereffectivedate", null);
                CrmHelper.ProcesarCampoBuleano(ServicioCrm, xElem, entidadCrm, "telemetering", "atos_telemetering", "Sí", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadCrm, "finalqdgranted", "atos_finalqdgranted", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadCrm, "finalqhgranted", "atos_finalqhgranted", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadCrm, "finalclientyearlyconsumption", "atos_finalclientyearlyconsumption", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "gasusetype", "atos_gasusetype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla", "atos_procesotiposdeuso" }, new object[] { "Tipos de Uso del Gas", xDoc.GetElementsByTagName("processcode")[0].InnerText }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "updatereason", "atos_updatereason", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivo de Modificación en un cambio de Comercializador" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "result", "atos_result", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultdesc", "atos_resultdesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "activationtype", "atos_activationtype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo de Activación" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "activationtypedesc", "atos_activationtypedesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo de Activación" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "closingtype", "atos_closingtype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "TIPO CIERRE" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "closingtypedesc", "atos_closingtypedesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "TIPO CIERRE" }, null);
                fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadCrm, "interventiondate", "atos_interventiondate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "interventionhourfrom", "atos_interventionhourfrom", fecha, null);
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "interventionhourto", "atos_interventionhourto", fecha, null);
                CrmHelper.ProcesarCampoEntero(ServicioCrm, xElem, entidadCrm, "visitnumber", "atos_visitnumber", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "firstname", "atos_firstname", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "familyname1", "atos_familyname1", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "familyname2", "atos_familyname2", null);
                CrmHelper.ProcesarCampoBuleano(ServicioCrm, xElem, entidadCrm, "regularaddress", "atos_regularaddress", "Sí", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "telephone1", "atos_telephone1", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "telephone2", "atos_telephone2", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "email", "atos_email", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "language", "atos_language", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Idioma" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "provinceowner", "atos_provinceowner", "atos_provincia", "atos_provinciaid", "atos_codigoprovincia", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "cityowner", "atos_cityowner", "atos_municipio", "atos_municipioid", "atos_codigo", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "zipcodeowner", "atos_zipcodeowner", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "streettypeowner", "atos_streettypeowner", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "TIPO VÍA" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "streetowner", "atos_streetowner", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "streetnumberowner", "atos_streetnumberowner", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "portalowner", "atos_portalowner", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "staircaseowner", "atos_staircaseowner", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "floorowner", "atos_floorowner", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "doorowner", "atos_doorowner", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "canonircperiodicity", "atos_canonircperiodicity", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "PERIODICIDAD CANON IRC" }, null);
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadCrm, "lastinspectionsdate", "atos_lastinspectionsdate", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "lastinspectionsresult", "atos_lastinspectionsresult", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "RESULTADO INSPECCIÓN" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "StatusPS", "atos_statusps", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "ESTADO PUNTO DE SUMINISTRO" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "readingtype", "atos_readingtype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "TIPO LECTURA" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "lectureperiodicity", "atos_lectureperiodicity", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "PERIODICIDAD LECTURA" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);

                ProcesarContadores(nombreFichero, xElem, entidadCrm, rechazo, agente);
                ProcesarCorrectores(nombreFichero, xElem, entidadCrm, rechazo, agente);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA341", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA3S41(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a3s41")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "responsedate", "atos_responsedate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "responsehour", "atos_responsedate", fecha, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "nationality", "atos_nationality", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "NACIONALIDAD" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "documenttype", "atos_documenttype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "TIPO DOCUMENTO" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "documentnum", "atos_documentnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "previousatrcode", "atos_previousatrcode", null);
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadCrm, "transfereffectivedate", "atos_transfereffectivedate", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "readingtype", "atos_readingtype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "TIPO LECTURA" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);

                ProcesarContadores(nombreFichero, xElem, entidadCrm, rechazo, agente);
                ProcesarCorrectores(nombreFichero, xElem, entidadCrm, rechazo, agente);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA3S41", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA441(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a441")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "comreferencenum", "atos_comreferencenum", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "responsedate", "atos_responsedate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "responsehour", "atos_responsedate", fecha, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "nationality", "atos_nationality", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "NACIONALIDAD" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "documenttype", "atos_documenttype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "TIPO DOCUMENTO" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "documentnum", "atos_documentnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "updatereason", "atos_updatereason", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivo de Modificación en un cambio de Comercializador" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "result", "atos_result", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Resultado" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultdesc", "atos_resultdesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "RESULTADO" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultreason", "atos_resultreason", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivos de Rechazo" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultreasondesc", "atos_resultreasondesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "MOTIVO RECHAZO OCSUM" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "closingtype", "atos_closingtype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "TIPO CIERRE" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "closingtypedesc", "atos_closingtypedesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "TIPO CIERRE" }, null);
                fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "interventiondate", "atos_interventiondate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "interventionhourfrom", "atos_interventionhourfrom", fecha, null);
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "interventionhourto", "atos_interventionhourto", fecha, null);
                CrmHelper.ProcesarCampoEntero(ServicioCrm, xElem, entidadCrm, "visitnumber", "atos_visitnumber", null);               
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);

                ProcesarContadores(nombreFichero, xElem, entidadCrm, rechazo, agente);
                ProcesarCorrectores(nombreFichero, xElem, entidadCrm, rechazo, agente);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA441", ex);
                throw new Exception(message, ex);
            }
        }

        internal void ProcesarPasoA4S41(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("a4s41")[numeroMensaje];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "reqcode", "atos_reqcode", null);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "responsedate", "atos_responsedate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "responsehour", "atos_responsedate", fecha, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "nationality", "atos_nationality", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "NACIONALIDAD" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "documenttype", "atos_documenttype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "TIPO DOCUMENTO" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "documentnum", "atos_documentnum", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "cups", "atos_cups", null);
                procesarCUPS(ServicioCrm, entidadCrm);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "previousatrcode", "atos_previousatrcode", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "result", "atos_result", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "RESULTADO" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultdesc", "atos_resultdesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "RESULTADO" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultreason", "atos_resultreason", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Motivos de Rechazo" }, null);
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "resultreasondesc", "atos_resultreasondesc", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "MOTIVO RECHAZO OCSUM" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "extrainfo", "atos_extrainfo", null);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarPasoA4S41", ex);
                throw new Exception(message, ex);
            }
        }
    }
}
