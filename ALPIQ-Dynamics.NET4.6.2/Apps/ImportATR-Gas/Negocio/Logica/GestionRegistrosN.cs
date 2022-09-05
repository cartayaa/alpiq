using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using System.Xml;
using System.Globalization;
using Negocio.Utilidades;
using Negocio.Entidades;

namespace Negocio.Logica
{
    public class GestionRegistrosN : NegocioBase
    {
        public GestionRegistrosN(IOrganizationService servicioCrm) :
            base(servicioCrm)
        {
        }


        public static void ProcesarRegistroR411800(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41180010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R41180020", "atos_sequencialregisto", null).XmlValor;
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41180030", "atos_funcaomedidaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14120" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41180040", "atos_unidademedidaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T10020" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R41180045", "atos_curvascarga", null, procesarRegistroR41180045);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R411800'", ex);
            }
        }
        private static RegistroResultado procesarRegistroR41180045(IOrganizationService servicioCrm, XmlElement xElem, Entity entidadCrm, string campoXml, string campoCrm, List<string> camposOmitidos)
        {
            string valorXml = string.Empty;
            try
            {
                string relacionN1 = "atos_atos_leiturasdiscriminacao_atos_curvasdecarga_codigoregistoleiturasdiscriminacaorpeid";
                //procesarRegistroCurvasDeCarga(servicioCrm, xElem, entidadCrm, campoXml, camposOmitidos, "R41180045", relacionN1);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R41180045'", ex);
            }
            return CrmHelper.InformarRegistroResultado(campoXml, valorXml, campoCrm, null, null, null, null, null);
        }

        public static void ProcesarContadador(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "countermodel", "atos_countermodel", null).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "countertype", "atos_countertype", null).XmlValor;
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "counternumber", "atos_counternumber", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "counterproperty", "atos_counterpropertyid", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Propiedad Contador-Corrector" }, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "reallecture", "atos_reallecture", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "counterpressure", "atos_counterpressure", null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                
                throw new Exception("Error al procesar el registro 'ProcesarContadador'", ex);
            }
        }

        public static void ProcesarCorrector(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "correctormodel", "atos_correctormodel", null).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "correctortype", "atos_correctortypeid", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipo Corrector" }, null).XmlValor;
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "correctornumber", "atos_correctornumber", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "correctorproperty", "atos_correctorpropertyid", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Propiedad Contador-Corrector" }, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "correctedlecture", "atos_correctedlecture", null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia + "-" + DateTime.Now.ToString("yyyyMMddHHmmss"));
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'ProcesarCorrector'", ex);
            }
        }

        public static void ProcesarConceptoFacturacion(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string secuencia = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "level", "atos_levelid", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Nivel Conceptos Facturación" }, null).XmlValor;
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "code", "atos_codeid", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Conceptos Facturación" }, null).XmlValor;
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "description", "atos_description", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Concepto Facturación" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "periodicity", "atos_periodicityid", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Periodicidad Facturación" }, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "units", "atos_units", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "unitimport", "atos_unitimport", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "import", "atos_import", null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia + "-" + DateTime.Now.ToString("yyyyMMddHHmmss"));
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'ProcesarCorrector'", ex);
            }
        }

        public static void ProcesarAnomalia(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = string.Empty;
                //string secuencia = string.Empty;
                codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "code", "atos_codeid", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Anomalía Instalación" }, null).XmlValor;
                //CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "description", "atos_description", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Anomalía Instalación" }, null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + DateTime.Now.ToString("yyyyMMddHHmmss"));
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'ProcesarCorrector'", ex);
            }
        }

        public static void ProcesarDocumento(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = string.Empty;
                string secuencia = string.Empty;
                DateTime? fecha = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadSubgrid, "date", "atos_date", null).Fecha;
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "doctype", "atos_doctypeid", "atos_tablasatrgas", "atos_tablasatrgasid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "Tipos de Documentos (Anexos)" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "url", "atos_url", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "extrainfo", "atos_extrainfo", null);
                entidadSubgrid.Attributes.Add("atos_name", fecha.Value.ToString("yyyyMMddHHmmss") + "-" + DateTime.Now.ToString("yyyyMMddHHmmss"));
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'ProcesarCorrector'", ex);
            }
        }
    }

}
