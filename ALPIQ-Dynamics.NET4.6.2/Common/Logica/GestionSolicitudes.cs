using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Web.Services.Protocols;
using System.Xml;
using Comun.Negocio.Entidades;
using Comun.Negocio.Utilidades;

namespace Comun.Negocio.Logica
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

        public static Guid AdjuntarXmlSolicitud(IOrganizationService servicioCrm, Guid solicitudId, string codProceso, string codPaso, string nombreFichero, string encodedData, string nombreEntidad)
        {
            Guid salida = new Guid();
            try
            {
                string hoy = DateTime.Now.ToLocalTime().Year.ToString("0000") + "-" + DateTime.Now.ToLocalTime().Month.ToString("00") + "-" +
                        DateTime.Now.ToLocalTime().Day.ToString("00") + " " + DateTime.Now.ToLocalTime().Hour.ToString("00") + ":" +
                        DateTime.Now.ToLocalTime().Minute.ToString("00") + ":" + DateTime.Now.ToLocalTime().Second.ToString("00");

                Entity _annotation = new Entity("annotation");
                _annotation.Attributes["objectid"] = new EntityReference(nombreEntidad, solicitudId);
                _annotation.Attributes["objecttypecode"] = nombreEntidad;
                _annotation.Attributes["subject"] = "Solicitação ATR " + codProceso + "/" + codPaso + " " + hoy;
                _annotation.Attributes["documentbody"] = encodedData;
                _annotation.Attributes["mimetype"] = @"application/xml";
                _annotation.Attributes["notetext"] = "Solicitação ATR " + codProceso + "/" + codPaso;
                //if (!String.IsNullOrEmpty(pDescripcion))
                //{
                //    _annotation.Attributes["notetext"] += " " + pDescripcion;
                //}
                _annotation.Attributes["filename"] = nombreFichero;

                salida = servicioCrm.Create(_annotation);
            }
            catch (SoapException soapEx)
            {
                throw new Exception(String.Format("Error al adjuntar el fichero a la solicitud de ATR:{0}", soapEx.Detail.InnerText));
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error en la creación de la solicitud de ATR:{0}", ex.Message));

            }
            return salida;
        }

        //public void ProcesarR413000(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, string agente)
        //{
        //    try
        //    {
        //        Entity tempFecha = new Entity();
        //        XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("R413000")[0];
        //        CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "R41300010", "atos_codigoregistoaceitacaoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true);
        //        CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadCrm, "R41300020", "atos_sequencialregistoaceitacao", null);
        //        DateTime? atos_dataprevistaaceitacaoid = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, tempFecha, "R41300030", "atos_dataprevistaaceitacaoid", null).Fecha;
        //        CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "R41300040", "atos_necessidadeagendamentoaceitacaoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T20101" }, null);
        //        CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "R41300050", "atos_servicosaceitacaoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T20100" }, null);
        //        CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "R41300060", "atos_dataprevistaaceitacaoid", atos_dataprevistaaceitacaoid, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarR413000", ex);
        //        throw new Exception(message, ex);
        //    }
        //}
        //public void ProcesarR414000(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, string agente)
        //{
        //    try
        //    {
        //        string hijosXml = "R414000";
        //        string entidadHijo = "atos_servicosefectuar";
        //        string relacionN1 = "atos_atos_solicitudatrpt_atos_servicosefectuar_solicitudatrptid";
        //        CrmHelper.ProcesarRegistrosN(ServicioCrm, nombreFichero, xDoc, entidadCrm, agente, relacionN1, hijosXml, entidadHijo, GestionRegistrosN.ProcesarRegistroR414000);
        //    }
        //    catch (Exception ex)
        //    {
        //        string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarR414000", ex);
        //        throw new Exception(message, ex);
        //    }
        //}
    }
}
