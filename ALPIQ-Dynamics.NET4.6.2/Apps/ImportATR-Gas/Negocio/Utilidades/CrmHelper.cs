using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Web.Services.Protocols;
using Negocio.Entidades;
using Negocio.Constantes;
using System.Globalization;
using Negocio.Enums;

namespace Negocio.Utilidades
{
    public class CrmHelper
    {
        public delegate void DelegateProcesarRegistro(IOrganizationService servicioCrm, XmlElement xElem, Entity entidadCrm);
        public delegate RegistroResultado DelegateProcesarCampo(IOrganizationService servicioCrm, XmlElement xElem, Entity entidadCrm, string campoXml, string campoCrm, List<string> camposOmitidos);
        public static void ProcesarRegistrosN(IOrganizationService servicioCrm, string nombreFichero, XmlElement xElem, Entity entidadCrm, string agente, string relacionN1, string hijosXml, string nombreEntidad, CrmHelper.DelegateProcesarRegistro procesarRegistro)
        {
            try
            {
                Relationship relationship = new Relationship(relacionN1);
                EntityCollection relatedEntities = new EntityCollection();
                foreach (XmlElement elem in xElem.GetElementsByTagName(hijosXml))
                {
                    Entity entidadSubgrid = new Entity(nombreEntidad);
                    procesarRegistro(servicioCrm, elem, entidadSubgrid);
                    relatedEntities.Entities.Add(entidadSubgrid);
                }
                if (relatedEntities.Entities.Count > 0)
                    entidadCrm.RelatedEntities.Add(relationship, relatedEntities);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error al procesar los registros hijos '{0}'", hijosXml), ex);
            }
        }
        public static RegistroResultado ProcesarCampoBusqueda(IOrganizationService servicioCrm, XmlElement xElem, Entity entidadCrm, string campoXml, string campoCrm, string entidadBusqueda, string entidadBusquedaId, string filtroCampo, List<string> camposOmitidos, bool esObligatorio = false)
        {
            return ProcesarCampoBusqueda(servicioCrm, xElem, entidadCrm, campoXml, campoCrm, entidadBusqueda, entidadBusquedaId, new string[] { filtroCampo }, new object[] { }, camposOmitidos, esObligatorio);
        }
        public static RegistroResultado ProcesarCampoBusqueda(IOrganizationService servicioCrm, XmlElement xElem, Entity entidadCrm, string campoXml, string campoCrm, string entidadBusqueda, string entidadBusquedaId, string[] filtroCampos, object[] filtroValores, List<string> camposOmitidos, bool esObligatorio = false)
        {
            Guid guid = Guid.Empty;
            string valorXml = string.Empty;
            if (EsCampoValido(xElem.OwnerDocument, campoXml, camposOmitidos))
            {
                valorXml = xElem.GetElementsByTagName(campoXml)[0].InnerText;
                List<object> valores = new List<object>(filtroValores);
                valores.Insert(0, valorXml);
                guid = ObtenerIdEntidad(servicioCrm, entidadBusqueda, entidadBusquedaId, filtroCampos, valores.ToArray());
                if (guid != Guid.Empty)
                    entidadCrm.Attributes.Add(campoCrm, new EntityReference(entidadBusqueda, guid));
            }
            if (esObligatorio && guid == Guid.Empty)
                throw new Exception(string.Format("Error al recuperar la entidad para '{0}'", entidadBusqueda));
            else
                return InformarRegistroResultado(campoXml, valorXml, campoCrm, guid, null, null, null, null);
        }
        public static RegistroResultado ProcesarCampoString(IOrganizationService servicioCrm, XmlElement xElem, Entity entidadCrm, string campoXml, string campoCrm, List<string> camposOmitidos, DelegateProcesarCampo procesarCustom = null, bool esObligatorio = false)
        {
            string valorXml = string.Empty;
            if (procesarCustom != null)
                return procesarCustom(servicioCrm, xElem, entidadCrm, campoXml, campoCrm, camposOmitidos);
            else
                if (EsCampoValido(xElem.OwnerDocument, campoXml, camposOmitidos))
            {
                valorXml = xElem.GetElementsByTagName(campoXml)[0].InnerText;
                if (!string.IsNullOrEmpty(valorXml))
                    entidadCrm.Attributes.Add(campoCrm, valorXml);
                else if (esObligatorio)
                    throw new Exception(string.Format("Error al recuperar el valor para el campo '{0}'", campoXml));
            }
            else if (esObligatorio)
                throw new Exception(string.Format("Error al recuperar el campo '{0}'", campoXml));
            return InformarRegistroResultado(campoXml, valorXml, campoCrm, null, null, null, null, null);
        }
        public static RegistroResultado ProcesarCampoBuleano(IOrganizationService servicioCrm, XmlElement xElem, Entity entidadCrm, string campoXml, string campoCrm, string valorVerdadero, List<string> camposOmitidos, DelegateProcesarCampo procesarCustom = null, bool esObligatorio = false)
        {
            string valorXml = string.Empty;
            if (procesarCustom != null)
                return procesarCustom(servicioCrm, xElem, entidadCrm, campoXml, campoCrm, camposOmitidos);
            else
                if (EsCampoValido(xElem.OwnerDocument, campoXml, camposOmitidos))
            {
                valorXml = xElem.GetElementsByTagName(campoXml)[0].InnerText;
                if (!string.IsNullOrEmpty(valorXml))
                    entidadCrm.Attributes.Add(campoCrm, valorXml == valorVerdadero);
                else if (esObligatorio)
                    throw new Exception(string.Format("Error al recuperar el valor para el campo '{0}'", campoXml));
            }
            else if (esObligatorio)
                throw new Exception(string.Format("Error al recuperar el campo '{0}'", campoXml));
            return InformarRegistroResultado(campoXml, valorXml, campoCrm, null, null, null, null, valorXml == valorVerdadero);
        }
        public static RegistroResultado ProcesarCampoEntero(IOrganizationService servicioCrm, XmlElement xElem, Entity entidadCrm, string campoXml, string campoCrm, List<string> camposOmitidos, DelegateProcesarCampo procesarCustom = null, bool esObligatorio = false)
        {
            string valorXml = string.Empty;
            if (procesarCustom != null)
                return procesarCustom(servicioCrm, xElem, entidadCrm, campoXml, campoCrm, camposOmitidos);
            else
                if (EsCampoValido(xElem.OwnerDocument, campoXml, camposOmitidos))
            {
                valorXml = xElem.GetElementsByTagName(campoXml)[0].InnerText;
                if (!string.IsNullOrEmpty(valorXml))
                    entidadCrm.Attributes.Add(campoCrm, Convert.ToInt32(valorXml));
                else if (esObligatorio)
                    throw new Exception(string.Format("Error al recuperar el valor para el campo '{0}'", campoXml));
            }
            else if (esObligatorio)
                throw new Exception(string.Format("Error al recuperar el campo '{0}'", campoXml));

            int? valorXmlConvertido = string.IsNullOrWhiteSpace(valorXml) ? (int?)null : Convert.ToInt32(valorXml);
            return InformarRegistroResultado(campoXml, valorXml, campoCrm, null, null, valorXmlConvertido, null, null);
        }
        public static RegistroResultado ProcesarCampoOptionSet(IOrganizationService servicioCrm, XmlElement xElem, Entity entidadCrm, string campoXml, string campoCrm, List<string> camposOmitidos, DelegateProcesarCampo procesarCustom = null, bool esObligatorio = false)
        {
            string valorXml = string.Empty;
            if (procesarCustom != null)
                return procesarCustom(servicioCrm, xElem, entidadCrm, campoXml, campoCrm, camposOmitidos);
            else
                if (EsCampoValido(xElem.OwnerDocument, campoXml, camposOmitidos))
            {
                valorXml = xElem.GetElementsByTagName(campoXml)[0].InnerText;
                if (!string.IsNullOrEmpty(valorXml))
                    entidadCrm.Attributes.Add(campoCrm, new OptionSetValue(Convert.ToInt32(Enum.Parse(typeof(NominacionIndividual), valorXml))));
                else if (esObligatorio)
                    throw new Exception(string.Format("Error al recuperar el valor para el campo '{0}'", campoXml));
            }
            else if (esObligatorio)
                throw new Exception(string.Format("Error al recuperar el campo '{0}'", campoXml));

            int? valorXmlConvertido = string.IsNullOrWhiteSpace(valorXml) ? (int?)null : Convert.ToInt32(Enum.Parse(typeof(NominacionIndividual), valorXml));
            return InformarRegistroResultado(campoXml, valorXml, campoCrm, null, null, valorXmlConvertido, null, null);
        }
        public static RegistroResultado ProcesarCampoDecimal(IOrganizationService servicioCrm, XmlElement xElem, Entity entidadCrm, string campoXml, string campoCrm, List<string> camposOmitidos, DelegateProcesarCampo procesarCustom = null, bool esObligatorio = false)
        {
            string valorXml = string.Empty;
            if (procesarCustom != null)
                return procesarCustom(servicioCrm, xElem, entidadCrm, campoXml, campoCrm, camposOmitidos);
            else
                if (EsCampoValido(xElem.OwnerDocument, campoXml, camposOmitidos))
            {
                valorXml = xElem.GetElementsByTagName(campoXml)[0].InnerText;
                if (!string.IsNullOrEmpty(valorXml))
                    entidadCrm.Attributes.Add(campoCrm, Convert.ToDecimal(valorXml.Replace('.',',')));
                else if (esObligatorio)
                    throw new Exception(string.Format("Error al recuperar el valor para el campo '{0}'", campoXml));
            }
            else if (esObligatorio)
                throw new Exception(string.Format("Error al recuperar el campo '{0}'", campoXml));

            decimal? valorXmlConvertido = string.IsNullOrWhiteSpace(valorXml) ? (decimal?)null : Convert.ToDecimal(valorXml, new CultureInfo("en-US").NumberFormat);
            return InformarRegistroResultado(campoXml, valorXml, campoCrm, null, valorXmlConvertido, null, null, null);
        }
        public static RegistroResultado ProcesarCampoFecha(IOrganizationService servicioCrm, XmlElement xElem, Entity entidadCrm, string campoXml, string campoCrm, List<string> camposOmitidos, DelegateProcesarCampo procesarCustom = null, bool esObligatorio = false)
        {
            string valorXml = string.Empty;
            string fechaString = string.Empty;
            if (procesarCustom != null)
                return procesarCustom(servicioCrm, xElem, entidadCrm, campoXml, campoCrm, camposOmitidos);
            else
                if (EsCampoValido(xElem.OwnerDocument, campoXml, camposOmitidos))
            {
                valorXml = xElem.GetElementsByTagName(campoXml)[0].InnerText;
                valorXml = valorXml.Replace("-", string.Empty);
                if (!string.IsNullOrEmpty(valorXml) && Convert.ToUInt64(valorXml) > 0)
                {
                    if (valorXml.Length == 14)
                        fechaString = valorXml.Insert(4, "-").Insert(7, "-").Insert(12, ":").Insert(15, ":").Insert(10, " ");
                    else if (valorXml.Length == 8)
                        fechaString = valorXml.Insert(4, "-").Insert(7, "-");
                    else
                        fechaString = valorXml;
                    entidadCrm.Attributes.Add(campoCrm, Convert.ToDateTime(fechaString));
                }
                else if (esObligatorio)
                    throw new Exception(string.Format("Error al recuperar el valor para el campo '{0}'", campoXml));
            }
            else if (esObligatorio)
                throw new Exception(string.Format("Error al recuperar el campo '{0}'", campoXml));

            DateTime? valorXmlConvertido = string.IsNullOrWhiteSpace(fechaString) ? (DateTime?)null : Convert.ToDateTime(fechaString);
            return InformarRegistroResultado(campoXml, valorXml, campoCrm, null, null, null, valorXmlConvertido, null);
        }
        public static RegistroResultado ProcesarCampoHora(IOrganizationService servicioCrm, XmlElement xElem, Entity entidadCrm, string campoXml, string campoCrm, DateTime? fecha, List<string> camposOmitidos, DelegateProcesarCampo procesarCustom = null, bool esObligatorio = false)
        {
            string valorXml = string.Empty;
            string horaString = string.Empty;
            DateTime? hora = null;
            if (procesarCustom != null)
                return procesarCustom(servicioCrm, xElem, entidadCrm, campoXml, campoCrm, camposOmitidos);
            else
                if (EsCampoValido(xElem.OwnerDocument, campoXml, camposOmitidos))
            {
                valorXml = xElem.GetElementsByTagName(campoXml)[0].InnerText;
                valorXml = valorXml.Replace(":", string.Empty);
                if (!string.IsNullOrEmpty(valorXml))
                {
                    if (!fecha.HasValue)
                        hora = DateTime.MinValue;
                    hora = new DateTime(
                        fecha.Value.Year,
                        fecha.Value.Month,
                        fecha.Value.Day,
                        Convert.ToInt32(valorXml.Substring(0, 2)),
                        Convert.ToInt32(valorXml.Substring(2, 2)),
                        Convert.ToInt32(valorXml.Substring(4, 2)));
                    entidadCrm.Attributes.Add(campoCrm, hora);
                }
                else if (esObligatorio)
                    throw new Exception(string.Format("Error al recuperar el valor para el campo '{0}'", campoXml));
            }
            else if (esObligatorio)
                throw new Exception(string.Format("Error al recuperar el campo '{0}'", campoXml));
            else
            {
                hora = fecha;
                entidadCrm.Attributes.Add(campoCrm, hora);
            }

            return InformarRegistroResultado(campoXml, valorXml, campoCrm, null, null, null, hora, null);
        }
        public static RegistroResultado InformarRegistroResultado(string xmlCampo, string xmlValor, string crmCampo, Guid? guid, decimal? valorDecimal, int? valorEntero, DateTime? valorFecha, bool? valorBuleano)
        {
            RegistroResultado resultado = new RegistroResultado();
            resultado.XmlCampo = xmlCampo;
            resultado.XmlValor = xmlValor;
            resultado.CrmCampo = crmCampo;
            resultado.Guid = guid == null ? Guid.Empty : guid.Value;
            resultado.Decimal = valorDecimal == null ? 0 : valorDecimal.Value;
            resultado.Entero = valorEntero == null ? 0 : valorEntero.Value;
            resultado.Fecha = valorFecha == null ? DateTime.MaxValue : valorFecha.Value;
            resultado.Buleano = valorBuleano == null ? false : valorBuleano.Value;
            return resultado;
        }
        public static void ProcesarCampo<T>(IOrganizationService servicioCrm, XmlElement xElem, Entity entidadCrm, string campoXml, string campoCrm, List<string> camposOmitidos, DelegateProcesarCampo procesarCustom = null, bool esObligatorio = false)
            where T : struct
        {
            if (procesarCustom != null)
                procesarCustom(servicioCrm, xElem, entidadCrm, campoXml, campoCrm, camposOmitidos);
            else
                if (EsCampoValido(xElem.OwnerDocument, campoXml, camposOmitidos))
            {
                string cadena = xElem.GetElementsByTagName(campoXml)[0].InnerText;
                if (!string.IsNullOrEmpty(cadena))
                {
                    if (typeof(T) == typeof(string))
                        entidadCrm.Attributes.Add(campoCrm, cadena);
                    else if (typeof(T) == typeof(DateTime))
                        entidadCrm.Attributes.Add(campoCrm, Convert.ToDateTime(cadena));
                    else if (typeof(T) == typeof(int))
                        entidadCrm.Attributes.Add(campoCrm, Convert.ToInt32(cadena));
                    else if (typeof(T) == typeof(decimal))
                        entidadCrm.Attributes.Add(campoCrm, Convert.ToDecimal(cadena));
                }
                else if (esObligatorio)
                    throw new Exception(string.Format("Error al recuperar el valor para el campo '{0}'", campoXml));
            }
            else if (esObligatorio)
                throw new Exception(string.Format("Error al recuperar el campo '{0}'", campoXml));
        }
        public static bool EsCampoValido(XmlDocument xDoc, string campo, List<string> camposOmitidos)
        {
            return xDoc.GetElementsByTagName(campo).Count > 0 && NoEsCampoOmitido(campo, camposOmitidos);
        }
        public static bool NoEsCampoOmitido(string campo, List<string> camposOmitidos)
        {
            return camposOmitidos == null || !camposOmitidos.Exists(x => x.ToLower() == campo.ToLower());
        }
        public static Guid ObtenerIdEntidad(IOrganizationService servicioCrm, string nombreEntidad, string nombreCampoId, string filtroCampo, string filtroValor)
        {
            return ObtenerIdEntidad(servicioCrm, nombreEntidad, nombreCampoId, new string[] { filtroCampo }, new object[] { filtroValor });
        }
        public static Guid ObtenerIdEntidad(IOrganizationService servicioCrm, string nombreEntidad, string nombreCampoId, string[] filtroCampos, object[] filtroValores)
        {
            Guid salida = new Guid();
            try
            {
                QueryByAttribute consulta = new QueryByAttribute(nombreEntidad);

                consulta.ColumnSet = new ColumnSet(nombreCampoId);
                consulta.Attributes.AddRange(filtroCampos);
                consulta.Values.AddRange(filtroValores);

                EntityCollection resConsulta = servicioCrm.RetrieveMultiple(consulta);

                if (resConsulta.Entities.Count > 0)
                {
                    salida = resConsulta.Entities[0].Id;
                }
            }
            catch (SoapException soapEx)
            {
                throw new Exception(String.Format("Error en la consulta de {0}:{1}", nombreEntidad, soapEx.Detail.InnerText));
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error en la consulta de {0}, con los campos[{1}] y valores[{2}].\r\nExcepción: {3}", nombreEntidad, string.Join(",", filtroCampos), string.Join(",", filtroValores), ex.Message));
            }
            return salida;
        }

        public static ProcesoResultado GuardarEntiad(IOrganizationService servicio, Entity entidadCRM, string proceso, string paso)
        {
            ProcesoResultado resultado = new ProcesoResultado();
            Guid guid = Guid.Empty;
            guid = UpsertEntidadCrm(servicio, entidadCRM);
            if (guid == Guid.Empty && entidadCRM.Attributes.Count > 0)
                throw new Exception(string.Format(Mensajes.ErrorCreacionEntidadCrm, proceso, paso));
            else
                resultado.CampoID = guid;

            return resultado;
        }
        public static Guid CreateSolicitudATR(IOrganizationService servicio, Entity entidadCrm)
        {
            Guid guid = new Guid();
            guid = servicio.Create(entidadCrm);
            entidadCrm.Attributes.Add(entidadCrm.LogicalName + "id", guid);
            return guid;
        }

        public static Guid UpsertEntidadCrm(IOrganizationService servicio, Entity entidadCrm)
        {
            Guid salida = new Guid();
            try
            {
                if (entidadCrm.Attributes.Count == 0)
                    return salida;
                else if (entidadCrm.Contains(entidadCrm.LogicalName + "id"))
                {
                    servicio.Update(entidadCrm);
                    salida = (Guid)entidadCrm.Attributes[entidadCrm.LogicalName + "id"];
                }
                else
                    salida = CreateSolicitudATR(servicio, entidadCrm);
            }
            catch (SoapException soapEx)
            {
                throw new Exception(String.Format("Error en la creación de la solicitud de ATR:{0}", soapEx.Detail.InnerText));
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error en la creación de la solicitud de ATR:{0}", ex.Message));
            }
            return salida;
        }

        public static void ProcesarCUPS(IOrganizationService ServicioCrm, Entity entidadCrm, string campoCrm, string cpe)
        {
            Guid guid = ObtenerIdEntidad(ServicioCrm, "atos_instalacion", "atos_instalacionid", new string[] { "atos_cups20", "atos_historico" }, new object[] { cpe, false });
            if (guid != Guid.Empty)
                entidadCrm.Attributes.Add(campoCrm, new EntityReference("atos_instalacion", guid));
        }
    }
}
