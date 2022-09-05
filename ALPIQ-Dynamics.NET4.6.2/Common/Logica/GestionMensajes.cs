using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using System.Xml;
using Comun.Negocio.Utilidades;

namespace Comun.Negocio.Logica
{
    public class GestionMensajes : NegocioBase
    {
        public GestionMensajes(IOrganizationService servicioCrm) :
            base(servicioCrm)
        {
        }
        public void ProcesarCabecera(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, string agente)
        {
            try
            {
                Guid guid = Guid.Empty;
                XmlElement xElem = (XmlElement)xDoc.GetElementsByTagName("heading")[0];
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadCrm, "dispatchingcode", "atos_dispatchingcode", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "dispatchingcompany", "atos_dispatchingcompany", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "SUJETO" }, null, true);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "distinycompany", "atos_distinycompany", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "SUJETO" }, null, true);
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, new Entity(), "communicationsdate", "atos_communicationsdate", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadCrm, "communicationshour", "atos_communicationshour", fecha, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "processcode", "atos_processcode", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "PROCESO" }, null, true);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadCrm, "messagetype", "atos_messagetype", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "TIPO MENSAJE" }, null, true);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "ProcesarCabecera", ex);
                throw new Exception(message, ex);
            }
        }
    }
}
