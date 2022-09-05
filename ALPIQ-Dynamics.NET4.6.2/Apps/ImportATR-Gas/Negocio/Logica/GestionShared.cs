using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using System.Xml;
using Negocio.Utilidades;
using Negocio.Entidades;
using System.Web.Services.Protocols;
using Negocio.Enums;

namespace Negocio.Logica
{
    public class GestionShared : NegocioBase
    {
        public GestionShared(IOrganizationService servicioCrm) :
            base(servicioCrm)
        {
        }

        public void ProcesarCabecera(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, string agente)
        {
            try
            {
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("heading")[0];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "dispatchingcode", "atos_dispatchingcode", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "dispatchingcompany", "atos_dispatchingcompany", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "SUJETO" }, null, true);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "destinycompany", "atos_destinycompany", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "SUJETO" }, null, true);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "communicationsdate", "atos_communicationsdate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "communicationshour", "atos_communicationsdate", fecha, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "processcode", "atos_processcode", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "PROCESO" }, null, true);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "messagetype", "atos_messagetype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "TIPO MENSAJE" }, null, true);
                CrmHelper.ProcesarCampoString(ServicioCrm, xDoc.DocumentElement, entidadCrm, "comreferencenum", "atos_name", null);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarCabecera", ex);
                throw new Exception(message, ex);
            }
        }

        public static Guid AdjuntarXml(IOrganizationService servicioCrm, Guid entidadId, string codProceso, string codPaso, string nombreFichero, string encodedData, string nombreEntidad, string sujeto)
        {
            Guid salida = new Guid();
            try
            {
                string hoy = DateTime.Now.ToLocalTime().Year.ToString("0000") + "-" + DateTime.Now.ToLocalTime().Month.ToString("00") + "-" +
                        DateTime.Now.ToLocalTime().Day.ToString("00") + " " + DateTime.Now.ToLocalTime().Hour.ToString("00") + ":" +
                        DateTime.Now.ToLocalTime().Minute.ToString("00") + ":" + DateTime.Now.ToLocalTime().Second.ToString("00");

                Entity _annotation = new Entity("annotation");
                _annotation.Attributes["objectid"] = new EntityReference(nombreEntidad, entidadId);
                _annotation.Attributes["objecttypecode"] = nombreEntidad;
                _annotation.Attributes["subject"] = sujeto + " " + codProceso + "/" + codPaso + " " + hoy;
                _annotation.Attributes["documentbody"] = encodedData;
                _annotation.Attributes["mimetype"] = @"application/xml";
                _annotation.Attributes["notetext"] = sujeto + " " + codProceso + "/" + codPaso;
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

        internal static string ObtenerSujetoNotas(TipoEntidad tipoEntidad)
        {
            switch(tipoEntidad)
            {
                case TipoEntidad.Factura: return "Factura";
                case TipoEntidad.Solicitud: return "Solicitud";
                case TipoEntidad.Reclamacion: return "Reclamación";
                case TipoEntidad.Consumo: return "Consumo";
                case TipoEntidad.Otro: return "Otros";
                default: return "Entidad Desconocida";
            }
        }
    }
}
