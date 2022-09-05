using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using System.Xml;
using System.Globalization;
using Comun.Negocio.Utilidades;

namespace Comun.Negocio.Logica
{
    public class GestionRegistrosN : NegocioBase
    {
        public GestionRegistrosN(IOrganizationService servicioCrm) :
            base(servicioCrm)
        {
        }
        public static void ProcesarRegistroR004000(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R00400010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R00400020", "atos_sequencialregisto", null).XmlValor;
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R00400030", "atos_codigoregistoreferenciaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R00400040", "atos_sequencialregistoreferencia", null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R004000'", ex);
            }
        }

        public static void ProcesarRegistroR217000(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R21700010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R21700020", "atos_sequencialregisto", null).XmlValor;
                string cpe = CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R21700030", "atos_cpe", null).XmlValor;
                CrmHelper.ProcesarCUPS(ServicioCrm, entidadSubgrid, "atos_instalacionid", cpe);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R217000'", ex);
            }
        }

        public static void ProcesarRegistroR218000(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R21800010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R21800020", "atos_sequencialregisto", null).XmlValor;
                string cpe = CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R21800030", "atos_cpe", null).XmlValor;
                CrmHelper.ProcesarCUPS(ServicioCrm, entidadSubgrid, "atos_instalacionid", cpe);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R218000'", ex);
            }
        }

        public static void ProcesarRegistroR312400(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R31240010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R31240020", "atos_sequencialregisto", null).XmlValor;
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R31240030", "atos_numeroavisopagamento", null);
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadSubgrid, "R31240040", "atos_datadocumento", null);
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadSubgrid, "R31240050", "atos_datapagamento", null);
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadSubgrid, "R31240060", "atos_datavencimento", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R31240070", "atos_valortotal", null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R312400'", ex);
            }
        }

        public static void ProcesarRegistroR312700(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R31270010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R31270020", "atos_sequencialregisto", null).XmlValor;
                string cpe = CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R31270030", "atos_cpe", null).XmlValor;
                CrmHelper.ProcesarCUPS(ServicioCrm, entidadSubgrid, "atos_instalacionid", cpe);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R31270040", "atos_numerodocumento", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R31270050", "atos_valor", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R31270060", "atos_numerodocumentoexterno", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R31270070", "atos_motivoreclamacao", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R31270080", "atos_nomeproprio", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R31270090", "atos_sobrenome", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R31270100", "atos_morada", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R31270110", "atos_numeroporta", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R31270120", "atos_localidade", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R31270130", "atos_codigopostalid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T10210" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R31270140", "atos_pais", null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R312700'", ex);
            }
        }

        public static void ProcesarRegistroR314000(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                Entity tempFecha = new Entity();
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R31400010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R31400020", "atos_sequencialregisto", null).XmlValor;
                string cpe = CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R31400030", "atos_cpe", null, null, true).XmlValor;
                CrmHelper.ProcesarCUPS(ServicioCrm, entidadSubgrid, "atos_instalacionid", cpe);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R31400040", "atos_marcaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14050" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R31400050", "atos_numeroequipamento", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R31400060", "atos_registadorid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14420" }, null);
                DateTime? atos_dataleitura = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, tempFecha, "R31400070", "atos_dataleitura", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadSubgrid, "R31400080", "atos_datahoraleitura", atos_dataleitura, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R31400090", "atos_leitura", null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);	
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R314000'", ex);
            }
        }

        public static void ProcesarRegistroR314500(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                Entity tempFecha = new Entity();
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R31450010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R31450020", "atos_sequencialregisto", null).XmlValor;
                string cpe = CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R31450030", "atos_cpe", null, null, true).XmlValor;
                CrmHelper.ProcesarCUPS(ServicioCrm, entidadSubgrid, "atos_instalacionid", cpe);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R31450040", "atos_marcaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14050" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R31450050", "atos_numeroequipamento", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R31450060", "atos_registadorid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14420" }, null);
                DateTime? atos_dataleitura = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, tempFecha, "R31450070", "atos_dataleitura", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadSubgrid, "R31450080", "atos_datahoraleitura", atos_dataleitura, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R31450090", "atos_leitura", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R31450100", "atos_estadoregistoleituraid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14710" }, null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R314500'", ex);
            }
        }

        public static void ProcesarRegistroR316000(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R31600010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R31600020", "atos_sequencialregisto", null).XmlValor;
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R31600030", "atos_motivoincidenciaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T23160" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R31600040", "atos_ordinalincidenciaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T23165" }, null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R316000'", ex);
            }
        }

        public static void ProcesarRegistroR321000(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                Entity tempFecha = new Entity();
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R32100010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R32100020", "atos_sequencialregisto", null).XmlValor;
                string cpe = CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R32100030", "atos_cpe", null).XmlValor;
                CrmHelper.ProcesarCUPS(ServicioCrm, entidadSubgrid, "atos_instalacionid", cpe);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R32100040", "atos_niveltensaoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T12210" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R32100050", "atos_caracteristicainstalacaoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T12217" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R32100060", "atos_zonaqualidadeservicioid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T12110" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R32100070", "atos_tipocompensacaoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T23310" }, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R32100080", "atos_numerointerrupcoes", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R32100090", "atos_duracaointerrupcoes", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R32100100", "atos_identificadormesagem", null);
                DateTime? atos_datainicio = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, tempFecha, "R32100110", "atos_datainicio", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadSubgrid, "R32100120", "atos_datainicio", atos_datainicio, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R32100130", "atos_periodoagendadoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T22100" }, null);
                DateTime? atos_datafim = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, tempFecha, "R32100140", "atos_datafim", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadSubgrid, "R32100150", "atos_datafim", atos_datafim, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R32100160", "atos_motivoincumprimentoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T23320" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R32100170", "atos_motivoexclusaoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T23330" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R32100180", "atos_direirocompensacaoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T23340" }, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R32100190", "atos_valor", null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R321000'", ex);
            }
        }

        public static void ProcesarRegistroR411200(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41120010", "atos_codigoderegistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R41120020", "atos_sequencialregisto", null).XmlValor;
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41120030", "atos_marcaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14050" }, null, true);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R41120040", "atos_numeroequipamento", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41120050", "atos_tipoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14060" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41120060", "atos_propriedadeid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14070" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41120070", "atos_funcaoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14080" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41120080", "atos_funcoesmedidasuportadasid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14110" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R41120090", "atos_numperiodoshorariospermitidos", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R41120100", "atos_numregistadorespermitidos", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R41120110", "atos_perdasnoferro", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41120120", "atos_ciclospermitidosid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14210" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41120130", "atos_periodoshorariosid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T20100" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41120140", "atos_tiporecolhadadosid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14310" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41120150", "atos_tipodadospermitidosid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14320" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41120160", "atos_tipodadosrecolhidosid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14320" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41120170", "atos_tipoligacaotelefonicaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14330" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41120180", "atos_niveltensaomedidaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T12220" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41120190", "atos_periodohorariosprogramadosid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14410" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41120200", "atos_cicloprogramadoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14220" }, null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R411200'", ex);
            }
        }

        public static void ProcesarRegistroR412000(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41200010", "atos_codigoderegistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor; ;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R41200020", "atos_sequencialregisto", null).XmlValor; ;
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41200030", "atos_motivoobjeccaoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T24120" }, null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R412000'", ex);
            }
        }

        public static void ProcesarRegistroR411400(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41140010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R41140020", "atos_sequencialregisto", null).XmlValor;
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41140030", "atos_marcaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14050" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R41140040", "atos_numeroequipamento", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41140050", "atos_registadorid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14420" }, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R41140060", "atos_factormultiplicativo", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R41140070", "atos_numdigitosinteiros", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R41140080", "atos_numdigitosdecimais", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41140090", "atos_tiporegistadorid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14430" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41140100", "atos_unidademedidaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T10020" }, null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R411400'", ex);
            }
        }

        public static void ProcesarRegistroR411600(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                Entity tempFecha = new Entity();
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41160010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R41160020", "atos_sequencialregisto", null).XmlValor;
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41160030", "atos_marcaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14050" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R41160040", "atos_numeroequipamento", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41160045", "atos_tipodadosrecolhidosid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14320" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41160050", "atos_registadorid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14420" }, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R41160056", "atos_factormultiplicativofinal", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R41160057", "atos_numdigitosinteiros", null);
                DateTime? atos_dataleitura = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, tempFecha, "R41160060", "atos_dataleitura", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadSubgrid, "R41160070", "atos_dataleitura", atos_dataleitura, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R41160080", "atos_leitura", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41160090", "atos_unidademedidaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T10020" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41160100", "atos_tipoleituraid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14650" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41160110", "atos_motivoleituraid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14620" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41160120", "atos_estadoleituraid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14680" }, null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R411600'", ex);
            }
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
                procesarRegistroCurvasDeCarga(servicioCrm, xElem, entidadCrm, campoXml, camposOmitidos, "R41180045", relacionN1);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R41180045'", ex);
            }
            return CrmHelper.InformarRegistroResultado(campoXml, valorXml, campoCrm, null, null, null, null);
        }

        public static void ProcesarRegistroR414000(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41400010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R41400020", "atos_sequencialregisto", null).XmlValor;
                Guid guid = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41400030", "atos_origemservicocodigoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T10310" }, null).Guid;
                if (guid == Guid.Empty)
                    CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41400030", "atos_origemservicocodigoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T10320" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41400040", "atos_origemservicotipoentidadeid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T10300" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41400050", "atos_servicoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T25140" }, null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R414000'", ex);
            }
        }

        public static void ProcesarRegistroR415000(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41500010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R41500020", "atos_sequencialregisto", null).XmlValor;
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R41500030", "atos_motivorecusaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T24150" }, null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R415000'", ex);
            }
        }

        public static void ProcesarRegistroR511200(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51120010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R51120020", "atos_sequencialregisto", null).XmlValor;
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51120030", "atos_marcaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14050" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R51120040", "atos_numeroequipamento", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51120050", "atos_tipoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14060" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51120060", "atos_propiedadeid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14070" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51120070", "atos_funcaoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14080" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51120080", "atos_funcoesmedidasuportadasid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14110" }, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R51120090", "atos_numperiodoshorariospermitidos", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R51120100", "atos_numregistadorespermitidos", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R51120110", "atos_perdasnoferro", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51120120", "atos_ciclospermitidosid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14210" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51120130", "atos_periodoshorariosid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T20100" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51120140", "atos_tiporecolhadadosid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14310" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51120150", "atos_tipodadospermitidosid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14320" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51120160", "atos_tipodadosrecolhidosid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14320" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51120170", "atos_motivorecusaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14330" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51120180", "atos_niveltensaomedidaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T12220" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51120190", "atos_periodoshorariosprogramadosid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14410" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51120200", "atos_cicloprogramadoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14220" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51120210", "atos_tipomovimentoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14510" }, null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R511200'", ex);
            }
        }

        public static void ProcesarRegistroR511400(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            string codigo = null;
            string secuencia = null;
            try
            {
                codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51140010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R51140020", "atos_sequencialregisto", null).XmlValor;
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51140030", "atos_marcaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14050" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R51140040", "atos_numeroequipamento", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51140050", "atos_registadorid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14420" }, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R51140060", "atos_factormultiplicativofinal", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R51140070", "atos_numdigitosinteiros", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R51140080", "atos_numdigitosdecimais", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51140090", "atos_tiporegistadorid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14430" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51140100", "atos_unidademedidaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T10020" }, null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R511400'", ex);
            }
        }

        public static void ProcesarRegistroR511600(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                Entity tempFecha = new Entity();
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51160010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R51160020", "atos_sequencialregisto", null).XmlValor;
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51160030", "atos_marcaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14050" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R51160040", "atos_numeroequipamento", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51160045", "atos_tipodadosrecolhidosid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14320" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51160050", "atos_registadorid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14420" }, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R51160056", "atos_factormultiplicativofinal", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R51160057", "atos_numdigitosinteiros", null);
                DateTime? atos_dataleitura = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, tempFecha, "R51160060", "atos_dataleitura", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadSubgrid, "R51160070", "atos_dataleitura", atos_dataleitura, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R51160080", "atos_leitura", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51160090", "atos_unidademedidaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T10020" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51160100", "atos_tipoleituraid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14650" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51160110", "atos_motivoleituraid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14620" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51160120", "atos_estadoleituraid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14680" }, null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R511600'", ex);
            }
        }

        public static void ProcesarRegistroR511800(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51180010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R51180020", "atos_sequencialregisto", null).XmlValor;
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51180030", "atos_funcaomedidaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14120" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51180040", "atos_unidademedidaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T10020" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R51180045", "atos_curvascarga", null, procesarRegistroR51180045);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R511800'", ex);
            }
        }
        private static RegistroResultado procesarRegistroR51180045(IOrganizationService servicioCrm, XmlElement xElem, Entity entidadCrm, string campoXml, string campoCrm, List<string> camposOmitidos)
        {
            string valorXml = string.Empty;
            try
            {
                string relacionN1 = "atos_atos_leituradiscriminacaoquartohoraria_atos_curvasdecarga_codigoregistoleiturasdiscriminacaoid";
                procesarRegistroCurvasDeCarga(servicioCrm, xElem, entidadCrm, campoXml, camposOmitidos, "R51180045", relacionN1);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R51180045'", ex);
            }
            return CrmHelper.InformarRegistroResultado(campoXml, valorXml, campoCrm, null, null, null, null);
        }

        public static void ProcesarRegistroR514000(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51400010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R51400020", "atos_sequencialregisto", null).XmlValor;
                Guid guid = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51400030", "atos_origemservicocodigoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T10310" }, null).Guid;
                if (guid == Guid.Empty)
                    guid = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51400030", "atos_origemservicocodigoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T10320" }, null).Guid;
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51400040", "atos_origemservicotipoentidadeid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T10300" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R51400050", "atos_servicoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T26140" }, null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R514000'", ex);
            }
        }

        public static void ProcesarRegistroR610000(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                Entity tempFecha = new Entity();
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R61000010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R61000020", "atos_sequencialregisto", null).XmlValor;
                String cpe = CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R61000030", "atos_cpe", null).XmlValor;
                CrmHelper.ProcesarCUPS(ServicioCrm, entidadSubgrid, "atos_instalacionid", cpe);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R61000040", "atos_marcaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14050" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R61000050", "atos_numeroequipamento", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R61000060", "atos_registadorid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14420" }, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R61000070", "atos_numerodigitos", null);
                DateTime? atos_dataleitura = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, tempFecha, "R61000080", "atos_dataleitura", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadSubgrid, "R61000090", "atos_datahoraleitura", atos_dataleitura, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R61000100", "atos_leitura", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R61000110", "atos_unidademedidaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T10020" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R61000120", "atos_tipoleituraid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14650" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R61000130", "atos_motivoleituraid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14620" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R61000140", "atos_estadoleituraid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14680" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R61000150", "atos_incidencialeituraid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14630" }, null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R610000'", ex);
            }
        }

        public static void ProcesarRegistroR611000(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R61100010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R61100020", "atos_sequencialregisto", null).XmlValor;
                String cpe = CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R61100030", "atos_cpe", null).XmlValor;
                CrmHelper.ProcesarCUPS(ServicioCrm, entidadSubgrid, "atos_instalacionid", cpe);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R61100040", "atos_marcaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14050" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R61100050", "atos_numeroequipamento", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R61100060", "atos_registadorid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14420" }, null);
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadSubgrid, "R61100070", "atos_datainicioperiodo", null);
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadSubgrid, "R61100080", "atos_datafimperiodo", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R61100090", "atos_consumo", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R61100100", "atos_unidademedidaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T10020" }, null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R611000'", ex);
            }
        }

        public static void ProcesarRegistroR651200(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R65120010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R65120020", "atos_secuencialregisto", null).XmlValor;
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadSubgrid, "R65120023", "atos_perdiodode", null);
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadSubgrid, "R65120024", "atos_perdiodoatede", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R65120027", "atos_tipoitemcalculadoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T26150" }, null, true);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R65120030", "atos_descricaonsumopotenciaperodo", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R65120040", "atos_unidademedidaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T10020" }, null, true);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R65120050", "atos_quantidade", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R65120053", "atos_quotatempo", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R65120060", "atos_preco", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R65120070", "atos_valorsemiva", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R65120080", "atos_taxaivaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T26160" }, null, true);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R6512000'", ex);
            }

        }

        public static void ProcesarRegistroR652000(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R65200010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R65200020", "atos_secuencialregisto", null).XmlValor;
                string cpe = CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R65200030", "atos_cpe", null).XmlValor;
                CrmHelper.ProcesarCUPS(ServicioCrm, entidadSubgrid, "atos_instalacionid", cpe);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R65200040", "atos_numerosegmento", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R65200045", "atos_tipocalculoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T26175" }, null);
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadSubgrid, "R65200050", "atos_datadocumento", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R65200060", "atos_descricao", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R65200065", "atos_tiposervico", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T26140" }, null, true); // TODO: rename field "id" suffix
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadSubgrid, "R65200070", "atos_dataexecucaoservico", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R65200080", "atos_quantidade", null);   // 4 decimales
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R65200090", "atos_preco", null);        // 4 decimales
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R65200100", "atos_valorsemiva", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R65200110", "atos_taxaivaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T26140" }, null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R652000'", ex);
            }

        }

        public static void ProcesarRegistroR659000(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R65900010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R65900020", "atos_secuencialregisto", null).XmlValor;
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R65900030", "atos_codigomensagemid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T26990" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R65900035", "atos_numerosegmento", null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R65900040", "atos_descricao", null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R659000'", ex);
            }
        }

        public static void ProcesarRegistroR671200(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R67120010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R67120020", "atos_secuencialregisto", null).XmlValor;
                string cpe = CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R67120030", "atos_cpe", null).XmlValor;
                CrmHelper.ProcesarCUPS(ServicioCrm, entidadSubgrid, "atos_instalacionid", cpe);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R67120040", "atos_numerosegmento", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R67120050", "atos_descricaoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T26180" }, null);
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadSubgrid, "R67120060", "atos_periodofaturacaode", null);
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadSubgrid, "R67120070", "atos_periodofaturacaoa", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R67120080", "atos_moedaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T10010" }, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R67120090", "atos_parcelasugscmecfixa", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R67120100", "atos_parcelasugscmecacerto", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R67120120", "atos_valorsemiva", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R67120130", "atos_taxaivaaplicavelid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T26160" }, null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R671200'", ex);
            }
        }

        public static void ProcesarRegistroR674000(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R67400010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R67400020", "atos_secuencialregisto", null).XmlValor;
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R67400030", "atos_baseiva", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R67400040", "atos_taxaivaaplicavelid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T26160" }, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R67400050", "atos_valoriva", null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R674000'", ex);
            }
        }

        public static void ProcesarRegistroR712000(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                Entity tempFecha = new Entity();
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R71200010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R71200020", "atos_secuencialregisto", null).XmlValor;
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R71200030", "atos_motivoobjeccaoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T27120" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R71200040", "atos_marcaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14050" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R71200050", "atos_numeroequipamento", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R71200060", "atos_registadorid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T14420" }, null);
                DateTime? atos_dataleitura = CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, tempFecha, "R71200070", "atos_dataleitura", null).Fecha;
                CrmHelper.ProcesarCampoHora(ServicioCrm, xElem, entidadSubgrid, "R71200080", "atos_datahoraleitura", atos_dataleitura, null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R71200090", "atos_leituraobjectar", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R71200100", "atos_consumoobjectar", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R71200110", "atos_leiturajustificativa", null);
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadSubgrid, "R71200120", "atos_dataleiturajustificativa", null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R712000'", ex);
            }
        }
        
        public static void ProcesarRegistroR713000(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R71300010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R71300020", "atos_secuencialregisto", null).XmlValor;
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadSubgrid, "R71300030", "atos_dataresultado", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R71300040", "atos_necessidadeagendamentoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T20100" }, null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R71300050", "atos_resultadoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T27140" }, null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R713000'", ex);
            }
        }

        public static void ProcesarRegistroR715000(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R71500010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R71500020", "atos_sequencialregisto", null).XmlValor;
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R71500030", "atos_motivorecusaid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T27150" }, null);
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R71500040", "atos_comentario", null);
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadSubgrid, "R71500050", "atos_datacomentario", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R71500060", "atos_leitura", null);
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadSubgrid, "R71500070", "atos_dataleitura", null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R715000'", ex);
            }
        }

        public static void ProcesarRegistroR950000(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R95000010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R95000020", "atos_secuencialregisto", null).XmlValor;
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R95000030", "atos_pasoatrid", "atos_pasoatr", "atos_pasoatrid", "atos_codigopaso", null);
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadSubgrid, "R95000040", "atos_datalimite", null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R950000'", ex);
            }
        }

        private static void procesarRegistroCurvasDeCarga(IOrganizationService servicioCrm, XmlElement xElem, Entity entidadCrm, string campoXml, List<string> camposOmitidos, string registro, string relacionN1)
        {
            string valorXml = string.Empty;
            try
            {
                if (CrmHelper.EsCampoValido(xElem.OwnerDocument, campoXml, camposOmitidos))
                {
                    valorXml = xElem.GetElementsByTagName(campoXml)[0].InnerText;
                    string entidadHijo = "atos_curvasdecarga";

                    Relationship relationship = new Relationship(relacionN1);
                    EntityCollection relatedEntities = new EntityCollection();
                    foreach (string curva in valorXml.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (string.IsNullOrWhiteSpace(curva)) continue;

                        Entity entidadSubgrid = new Entity(entidadHijo);
                        var datosCurva = curva.Split('\t');

                        entidadSubgrid.Attributes.Add("atos_datahoradeleitura", Convert.ToDateTime(datosCurva[0].Trim().Insert(4, "-").Insert(7, "-").Insert(12, ":").Insert(15, ":00").Insert(10, " ")));
                        entidadSubgrid.Attributes.Add("atos_leitura", Convert.ToDecimal(datosCurva[1].Trim(), new CultureInfo("en-US").NumberFormat));
                        Guid guid = CrmHelper.ObtenerIdEntidad(servicioCrm, "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { datosCurva[2].Trim(), "T14670" });
                        entidadSubgrid.Attributes.Add("atos_satusdeleituraid", new EntityReference("atos_tablaatr", guid));
                        entidadSubgrid.Attributes.Add("atos_name", registro + "-" + datosCurva[0].Trim());

                        relatedEntities.Entities.Add(entidadSubgrid);
                    }
                    if (relatedEntities.Entities.Count > 0)
                        entidadCrm.RelatedEntities.Add(relationship, relatedEntities);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar las curvas de carga", ex);
            }
        }

        public static void ProcesarRegistroR005000(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R00500010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R00500020", "atos_sequencialregisto", null).XmlValor;
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R00500030", "atos_localizacaoerro", null);
                CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R00500040", "atos_codigoerroid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T05010" }, null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R005000'", ex);
            }
        }

        public static void ProcesarRegistroR005500(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R00550010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R00550020", "atos_sequencialregisto", null).XmlValor;
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R00550030", "atos_localizacaoerro", null);
                Guid guid = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R00550040", "atos_codigoerroid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T05020" }, null).Guid;
                if(guid == Guid.Empty)
                    guid = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R00550040", "atos_codigoerroid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T06020" }, null).Guid;
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R005500'", ex);
            }
        }

        public static void ProcesarRegistroR313100(IOrganizationService ServicioCrm, XmlElement xElem, Entity entidadSubgrid)
        {
            try
            {	
                string codigo = CrmHelper.ProcesarCampoBusqueda(ServicioCrm, xElem, entidadSubgrid, "R31310010", "atos_codigoregistoid", "atos_tablaatr", "atos_tablaatrid", new string[] { "atos_codigo", "atos_tabla" }, new object[] { "T00040" }, null, true).XmlValor;
                string secuencia = CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R31310020", "atos_sequencialregisto", null).XmlValor;
                CrmHelper.ProcesarCampoString(ServicioCrm, xElem, entidadSubgrid, "R31310030", "atos_numerodocumento", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R31310040", "atos_valordivida", null);
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadSubgrid, "R31310050", "atos_datainicio", null);
                CrmHelper.ProcesarCampoFecha(ServicioCrm, xElem, entidadSubgrid, "R31310060", "atos_datafim", null);
                CrmHelper.ProcesarCampoDecimal(ServicioCrm, xElem, entidadSubgrid, "R31310070", "atos_valor", null);
                entidadSubgrid.Attributes.Add("atos_name", codigo + "-" + secuencia);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el registro 'R313100'", ex);
            }
        }
    }
}
