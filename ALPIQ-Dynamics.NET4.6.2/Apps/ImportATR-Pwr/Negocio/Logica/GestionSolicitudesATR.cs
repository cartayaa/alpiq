using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Web.Services.Protocols;
using System.Xml;
using Negocio.Entidades;
using Negocio.Constantes;

namespace Negocio.Logica
{
    public class GestionSolicitudesATR : NegocioBase
    {
        public GestionSolicitudesATR() :
            base()
        {

        }
        public GestionSolicitudesATR(IOrganizationService pServicioCrm) :
            base(pServicioCrm)
        {

        }

        // Hay que controlar que no se metan más potencias que periodos tenga la tarifa
        private decimal periodosDeLaTarifa(Guid tarifaId)
        {
            decimal periodosTarifa = -1;
            if (tarifaId != Guid.Empty)
            {
                Entity tarifa = ServicioCrm.Retrieve("atos_tarifa", tarifaId, new ColumnSet("atos_numeroperiodos"));
                if (tarifa.Attributes.Contains("atos_numeroperiodos"))
                    periodosTarifa = (decimal)tarifa.Attributes["atos_numeroperiodos"];
            }
            return periodosTarifa;
        }

        //Hay que controlar que no se metan más potencias que periodos tenga la tarifa
        private Boolean dentroDeLosPeriodos(int periodo, decimal periodosTarifa)
        {
            return (periodo <= 6 && (periodosTarifa == -1 || periodo <= periodosTarifa));
        }

        public delegate object DelegateProcesarCampo(XmlElement xElem, Entity entidadCrm, string campoXml, string campoCrm, List<string> camposOmitidos);
        public static Guid ProcesarCampoBusqueda(XmlElement xElem, Entity entidadCrm, string campoXml, string campoCrm, string entidadBusqueda, string entidadBusquedaId, string filtroCampo, List<string> camposOmitidos, bool esObligatorio = false)
        {
            return ProcesarCampoBusqueda(xElem, entidadCrm, campoXml, campoCrm, entidadBusqueda, entidadBusquedaId, new string[] { filtroCampo }, new object[] { }, camposOmitidos, esObligatorio);
        }
        public static Guid ProcesarCampoBusqueda(XmlElement xElem, Entity entidadCrm, string campoXml, string campoCrm, string entidadBusqueda, string entidadBusquedaId, string[] filtroCampos, object[] filtroValores, List<string> camposOmitidos, bool esObligatorio = false)
        {
            Guid guid = Guid.Empty;
            if (EsCampoValido(xElem, campoXml, camposOmitidos))
            {
                List<object> valores = new List<object>(filtroValores);
                valores.Insert(0, xElem.GetElementsByTagName(campoXml)[0].InnerText);
                guid = obtenerIdEntidad(entidadBusqueda, entidadBusquedaId, filtroCampos, valores.ToArray());
                if (guid != Guid.Empty)
                    entidadCrm.Attributes.Add(campoCrm, new EntityReference(entidadBusqueda, guid));
            }
            if (esObligatorio && guid == Guid.Empty)
                throw new Exception(string.Format("Error al recuperar la entidad para '{0}'", entidadBusqueda));
            else
                return guid;
        }
        public static void ProcesarCampoString(XmlElement xElem, Entity entidadCrm, string campoXml, string campoCrm, List<string> camposOmitidos, DelegateProcesarCampo procesarCustom = null, bool esObligatorio = false)
        {
            if (procesarCustom != null)
                procesarCustom(xElem, entidadCrm, campoXml, campoCrm, camposOmitidos);
            else
                if (EsCampoValido(xElem, campoXml, camposOmitidos))
            {
                string cadena = xElem.GetElementsByTagName(campoXml)[0].InnerText;
                if (!string.IsNullOrEmpty(cadena))
                    entidadCrm.Attributes.Add(campoCrm, cadena);
                else if (esObligatorio)
                    throw new Exception(string.Format("Error al recuperar el valor para el campo '{0}'", campoXml));
            }
            else if (esObligatorio)
                throw new Exception(string.Format("Error al recuperar el campo '{0}'", campoXml));
        }
        public static void ProcesarCampoEntero(XmlElement xElem, Entity entidadCrm, string campoXml, string campoCrm, List<string> camposOmitidos, DelegateProcesarCampo procesarCustom = null, bool esObligatorio = false)
        {
            if (procesarCustom != null)
                procesarCustom(xElem, entidadCrm, campoXml, campoCrm, camposOmitidos);
            else
                if (EsCampoValido(xElem, campoXml, camposOmitidos))
            {
                string entero = xElem.GetElementsByTagName(campoXml)[0].InnerText;
                if (!string.IsNullOrEmpty(entero))
                    entidadCrm.Attributes.Add(campoCrm, Convert.ToInt32(entero));
                else if (esObligatorio)
                    throw new Exception(string.Format("Error al recuperar el valor para el campo '{0}'", campoXml));
            }
            else if (esObligatorio)
                throw new Exception(string.Format("Error al recuperar el campo '{0}'", campoXml));
        }
        public static void ProcesarCampoDecimal(XmlElement xElem, Entity entidadCrm, string campoXml, string campoCrm, List<string> camposOmitidos, DelegateProcesarCampo procesarCustom = null, bool esObligatorio = false)
        {
            if (procesarCustom != null)
                procesarCustom(xElem, entidadCrm, campoXml, campoCrm, camposOmitidos);
            else
                if (EsCampoValido(xElem, campoXml, camposOmitidos))
            {
                string valorDecimal = xElem.GetElementsByTagName(campoXml)[0].InnerText;
                if (!string.IsNullOrEmpty(valorDecimal))
                    entidadCrm.Attributes.Add(campoCrm, Convert.ToDecimal(valorDecimal));
                else if (esObligatorio)
                    throw new Exception(string.Format("Error al recuperar el valor para el campo '{0}'", campoXml));
            }
            else if (esObligatorio)
                throw new Exception(string.Format("Error al recuperar el campo '{0}'", campoXml));
        }
        public static void ProcesarCampoFecha(XmlElement xElem, Entity entidadCrm, string campoXml, string campoCrm, List<string> camposOmitidos, DelegateProcesarCampo procesarCustom = null, bool esObligatorio = false)
        {
            if (procesarCustom != null)
                procesarCustom(xElem, entidadCrm, campoXml, campoCrm, camposOmitidos);
            else
                if (EsCampoValido(xElem, campoXml, camposOmitidos))
            {
                string fecha = xElem.GetElementsByTagName(campoXml)[0].InnerText;
                if (!string.IsNullOrEmpty(fecha))
                    entidadCrm.Attributes.Add(campoCrm, Convert.ToDateTime(fecha));
                else if (esObligatorio)
                    throw new Exception(string.Format("Error al recuperar el valor para el campo '{0}'", campoXml));
            }
            else if (esObligatorio)
                throw new Exception(string.Format("Error al recuperar el campo '{0}'", campoXml));
        }

        public void asignarSolicitudAsociada(XmlDocument xDoc, Entity entidadCRM)
        {
            try
            {
                List<string> errores = new List<string>();
                XmlElement datosCabecera = (XmlElement)xDoc.GetElementsByTagName("Cabecera").Item(0);

                string paso = datosCabecera.GetElementsByTagName("CodigoDePaso")[0].InnerText;
                string proceso = datosCabecera.GetElementsByTagName("CodigoDelProceso")[0].InnerText;
                string codigoSolicitud = datosCabecera.GetElementsByTagName("CodigoDeSolicitud")[0].InnerText;
                string pasoSolicitud = obtenerPasoParaSolicitudAsociada(paso);

                Guid procesoId = obtenerIdEntidad("atos_procesoatr", "atos_procesoatrid", "atos_codigoproceso", proceso);
                Guid pasoId = obtenerIdEntidad("atos_pasoatr", "atos_pasoatrid", new string[] { "atos_codigopaso", "atos_procesoatrid" }, new object[] { pasoSolicitud, procesoId });
                Guid solicitudId = obtenerIdEntidad("atos_solicitudatr", "atos_solicitudatrid",
                    new string[] { "atos_pasoatrid", "atos_procesoatrid", "atos_codigosolicitud" },
                    new object[] { pasoId, procesoId, codigoSolicitud });
                if (pasoId != Guid.Empty && procesoId != Guid.Empty && solicitudId != Guid.Empty)
                    entidadCRM.Attributes.Add("atos_solicitudasociadaid", new EntityReference("atos_solicitudatr", solicitudId));
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
                case Constantes.Paso.N05: paso = Constantes.Paso.N01; break;
                default: paso = string.Format("{0:D2}", int.Parse(paso) - 1); break;
            }
            return paso;
        }

        private void gestionarAceptacion(XmlDocument xDoc, Entity solicitudAtr, List<string> camposOmitidos)
        {
            try
            {
                ProcesarCampoFecha(xDoc.DocumentElement, solicitudAtr, "FechaAceptacion", "atos_fechaaceptacion", camposOmitidos);
                ProcesarCampoBusqueda(xDoc.DocumentElement, solicitudAtr, "ActuacionCampo", "atos_actuacionencampoid", "atos_actuacionencampo", "atos_actuacionencampoid", "atos_codigo", camposOmitidos);
                ProcesarCampoFecha(xDoc.DocumentElement, solicitudAtr, "FechaUltimaLecturaFirme", "atos_fechaultimalectura", camposOmitidos);

                // Hay que controlar que no se metan más potencias que periodos tenga la tarifa
                Guid tarifaATRId = ProcesarCampoBusqueda(xDoc.DocumentElement, solicitudAtr, "TarifaATR", "atos_tarifaid", "atos_tarifa", "atos_tarifaid", "atos_codigoocsum", camposOmitidos);
                decimal periodosTarifa = this.periodosDeLaTarifa(tarifaATRId);

                ProcesarCampoBusqueda(xDoc.DocumentElement, solicitudAtr, "TipoActivacionPrevista", "atos_tipoactivacionprevistaid", "atos_tipoactivacionprevista", "atos_tipoactivacionprevistaid", "atos_codigotipoactivacionprevista", camposOmitidos);
                ProcesarCampoFecha(xDoc.DocumentElement, solicitudAtr, "FechaActivacionPrevista", "atos_fechaactivacionprevista", camposOmitidos);

                if (EsCampoValido(xDoc.DocumentElement, "PotenciasContratadas", camposOmitidos))
                {
                    XmlNodeList potencias = xDoc.GetElementsByTagName("PotenciasContratadas");
                    int periodo = 1;
                    foreach (XmlElement potencia in potencias[0].ChildNodes)
                        if (dentroDeLosPeriodos(periodo, periodosTarifa))
                            solicitudAtr.Attributes.Add("atos_potenciap" + periodo++, Convert.ToDecimal(potencia.InnerText) / 1000);
                }
                ProcesarCampoBusqueda(xDoc.DocumentElement, solicitudAtr, "ModoControlPotencia", "atos_modocontrolpotenciaid", "atos_modocontrolpotencia", "atos_modocontrolpotenciaid", "atos_codigo", camposOmitidos);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar la aceptacion", ex);
            }
        }

        private void gestionarRechazos(XmlDocument xDoc, Entity solicitudAtr, List<string> camposOmitidos)
        {
            try
            {
                EntityCollection relatedEntities = new EntityCollection { EntityName = "atos_rechazosolicitud" };
                Relationship relationship = new Relationship("atos_atos_solicitudatr_atos_rechazosolicitud_solicitudatrid");

                foreach (XmlElement xRechazo in xDoc.GetElementsByTagName("Rechazo"))
                {
                    if (relatedEntities.Entities.Count == 0)
                    {
                        ProcesarCampoFecha(xDoc.DocumentElement, solicitudAtr, "FechaRechazo", "atos_fecharechazo", camposOmitidos);
                        ProcesarCampoDecimal(xRechazo, solicitudAtr, "Secuencial", "atos_secuencialrechazo", camposOmitidos);
                        ProcesarCampoString(xRechazo, solicitudAtr, "Comentarios", "atos_comentariosrechazo", camposOmitidos);
                        ProcesarCampoBusqueda(xRechazo, solicitudAtr, "CodigoMotivo", "atos_motivorechazoatrid", "atos_motivosrechazoatr", "atos_motivosrechazoatrid", "atos_codigomotivorechazoatr", camposOmitidos);
                    }
                    Entity atos_rechazosolicitud = new Entity("atos_rechazosolicitud");
                    gestionarRechazo(xDoc, xRechazo, atos_rechazosolicitud, camposOmitidos);
                    relatedEntities.Entities.Add(atos_rechazosolicitud);
                }

                solicitudAtr.RelatedEntities.Add(relationship, relatedEntities);
                gestionarRegistrosDocumento(xDoc, solicitudAtr, camposOmitidos);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar los rechazos", ex);
            }
        }

        private void gestionarRechazo(XmlDocument xDoc, XmlElement xRechazo, Entity atos_rechazosolicitud, List<string> camposOmitidos)
        {
            try
            {
                ProcesarCampoFecha(xDoc.DocumentElement, atos_rechazosolicitud, "FechaRechazo", "atos_fecharechazo", camposOmitidos);
                ProcesarCampoDecimal(xRechazo, atos_rechazosolicitud, "Secuencial", "atos_secuencial", camposOmitidos);
                ProcesarCampoString(xRechazo, atos_rechazosolicitud, "Comentarios", "atos_comentariosmemo", camposOmitidos);
                ProcesarCampoBusqueda(xRechazo, atos_rechazosolicitud, "CodigoMotivo", "atos_motivosrechazoatrid", "atos_motivosrechazoatr", "atos_motivosrechazoatrid", "atos_codigomotivorechazoatr", camposOmitidos);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el rechazo", ex);
            }
        }

        private void gestionarRegistrosDocumento(XmlDocument xDoc, Entity solicitudAtr, List<string> camposOmitidos)
        {
            try
            {
                if (xDoc.GetElementsByTagName("RegistroDoc").Count > 0)
                {
                    EntityCollection relatedEntities = new EntityCollection { EntityName = "atos_registrodocumento" };
                    Relationship relation = new Relationship("atos_atos_solicitudatr_atos_registrodocumento_solicitudatrid");

                    foreach (XmlElement elemento in xDoc.GetElementsByTagName("RegistroDoc"))
                    {
                        Entity atos_registrodocumento = new Entity("atos_registrodocumento");
                        gestionarRegistroDocumento(elemento, atos_registrodocumento, camposOmitidos);
                        relatedEntities.Entities.Add(atos_registrodocumento);
                    }
                    if (relatedEntities.Entities.Count > 0)
                        solicitudAtr.RelatedEntities.Add(relation, relatedEntities);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar registros de documentos", ex);
            }
        }

        private void gestionarRegistroDocumento(XmlElement elemento, Entity atos_registrodocumento, List<string> camposOmitidos)
        {
            try
            {
                ProcesarCampoString(elemento, atos_registrodocumento, "DireccionUrl", "atos_direccionurl", camposOmitidos);
                ProcesarCampoBusqueda(elemento, atos_registrodocumento, "TipoDocAportado", "atos_tiposdedocumentacionid", "atos_tiposdedocumentacion", "atos_tiposdedocumentacionid", "atos_codigo", camposOmitidos);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar un registro documento", ex);
            }
        }

        public static bool EsCampoValido(XmlElement xDoc, string campo, List<string> camposOmitidos)
        {
            return xDoc.GetElementsByTagName(campo).Count > 0 && NoEsCampoOmitido(campo, camposOmitidos);
        }

        public static bool NoEsCampoOmitido(string campo, List<string> camposOmitidos)
        {
            return camposOmitidos == null || !camposOmitidos.Exists(x => x.ToLower() == campo.ToLower());
        }

        private static String obtenerOtrosMotivosRechazo(XmlNodeList listaRechazos)
        {
            StringBuilder salida = new StringBuilder();
            for (int motRechazo = 1; motRechazo < listaRechazos.Count; motRechazo++)
            {
                XmlNode rechazo = listaRechazos.Item(motRechazo);
                foreach (XmlElement elemento in rechazo.ChildNodes)
                {
                    if (elemento.Name == "CodigoMotivo")
                    {
                        salida.Append(GestionSolicitudesATR.obtenerDescripcionMotivoRechazo(elemento.InnerText) + " ");
                    }
                    else
                    {
                        salida.Append(elemento.InnerText + " ");
                    }
                }
                salida.Append(Environment.NewLine);
            }
            return salida.ToString();
        }

        private static string obtenerDescripcionMotivoRechazo(string pCodMotivoRechazo)
        {
            String salida = String.Empty;
            try
            {

                QueryByAttribute consulta = new QueryByAttribute("atos_motivosrechazoatr");

                consulta.ColumnSet = new ColumnSet("atos_name");
                consulta.Attributes.AddRange("atos_codigomotivorechazoatr");
                consulta.Values.AddRange(pCodMotivoRechazo);

                EntityCollection resConsulta = ServicioCrm.RetrieveMultiple(consulta);

                if (resConsulta.Entities.Count > 0)
                {
                    salida = resConsulta.Entities[0].Attributes["atos_name"].ToString();
                }
            }
            catch (SoapException soapEx)
            {
                throw new Exception(String.Format("Error en la consulta de motivos de rechazo:{0}", soapEx.Detail.InnerText));
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error en la consulta de motivos de rechazo:{0}", ex.Message));

            }
            return salida;
        }

        public void solicitudCabeceraPaso(string pNombreFichero, XmlDocument xDoc, Entity solicitudAtr, string agente)
        {
            List<String> salida = new List<String>();
            try
            {
                if (xDoc.GetElementsByTagName("Cabecera").Count > 0)
                {
                    XmlElement xCabecera = (XmlElement)xDoc.GetElementsByTagName("Cabecera")[0];
                    ProcesarCampoBusqueda(xCabecera, solicitudAtr, "CodigoREEEmpresaEmisora", "atos_distribuidoraid", "atos_distribuidora", "atos_distribuidoraid", "atos_codigoocsum", null, true);
                    ProcesarCampoBusqueda(xCabecera, solicitudAtr, "CodigoREEEmpresaDestino", "atos_comercializadoraid", "atos_comercializadora", "atos_comercializadoraid", "atos_codigo", null, true);
                    Guid procesoId = ProcesarCampoBusqueda(xCabecera, solicitudAtr, "CodigoDelProceso", "atos_procesoatrid", "atos_procesoatr", "atos_procesoatrid", "atos_codigoproceso", null, true);
                    ProcesarCampoBusqueda(xCabecera, solicitudAtr, "CodigoDePaso", "atos_pasoatrid", "atos_pasoatr", "atos_pasoatrid", new string[] { "atos_codigopaso", "atos_procesoatrid" }, new object[] { procesoId }, null, true);
                    ProcesarCampoString(xCabecera, solicitudAtr, "SecuencialDeSolicitud", "atos_secuenciadesolicitud", null, null, true);
                    ProcesarCampoString(xCabecera, solicitudAtr, "CodigoDeSolicitud", "atos_codigosolicitud", null, null, true);

                    Guid instalacionId = GestionInstalaciones.obtenerIdInstalacion(xCabecera.GetElementsByTagName("CUPS")[0].InnerText.Substring(0, 20));
                    if (instalacionId == Guid.Empty)
                        throw new Exception("Error al recuperar la entidad para 'atos_instalacion'");
                    solicitudAtr.Attributes.Add("atos_instalacionid", new EntityReference("atos_instalacion", instalacionId));

                    ProcesarCampoFecha(xCabecera, solicitudAtr, "FechaSolicitud", "atos_fechasolicitud", null, null, true);
                }
                else
                    throw new Exception("No se ha podido localizar la cabecera");
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "solicitudCabeceraPaso", ex);
                throw new Exception(message, ex);
            }
        }

        public void solicitudCuerpoPaso02(string nombreFichero, XmlDocument xDoc, Entity solicitudAtr, string proceso, bool rechazoSolicitud, string agente = null, List<string> camposOmitidos = null)
        {
            try
            {
                if (rechazoSolicitud)
                    gestionarRechazos(xDoc, solicitudAtr, camposOmitidos);
                else
                    gestionarAceptacion(xDoc, solicitudAtr, camposOmitidos);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "solicitudCuerpoPaso02", ex);
                throw new Exception(message, ex);
            }
        }

        public void solicitudCuerpoPaso02Baja(string nombreFichero, XmlDocument xDoc, Entity solicitudAtr, string proceso, bool rechazoSolicitud, string agente = null, List<string> camposOmitidos = null)
        {
            try
            {
                solicitudCuerpoPaso02(nombreFichero, xDoc, solicitudAtr, proceso, rechazoSolicitud, agente);
                if (!solicitudAtr.Contains("atos_fechaultimalectura"))
                    ProcesarCampoFecha(xDoc.DocumentElement, solicitudAtr, "FechaUltimaLecturaFirme", "atos_fechaultimalectura", camposOmitidos);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "solicitudCuerpoPaso02Baja", ex);
                throw new Exception(message, ex);
            }
        }

        public void solicitudCuerpoPaso03(string nombreFichero, XmlDocument xDoc, Entity solicitudAtr, string proceso, bool rechazoSolicitud, string agente = null, List<string> camposOmitidos = null)
        {
            try
            {
                EntityCollection relatedEntities = new EntityCollection { EntityName = "atos_incidencia" };
                Relationship relation = new Relationship("atos_atos_solicitudatr_atos_incidencia_solicitudatrid");
                XmlNodeList incidencias = xDoc.GetElementsByTagName("Incidencia");
                //Guid guid = CreateSolicitudATR(pSolicitudAtr);
                foreach (XmlElement xElement in incidencias)
                {
                    if (relatedEntities.Entities.Count == 0)
                    {
                        ProcesarCampoDecimal(xElement, solicitudAtr, "Secuencial", "atos_secuencialincidencia", camposOmitidos);
                        ProcesarCampoFecha(xDoc.DocumentElement, solicitudAtr, "FechaIncidencia", "atos_fechaincidencia", camposOmitidos);
                        ProcesarCampoBusqueda(xElement, solicitudAtr, "CodigoMotivo", "atos_codigodeincidenciaenedmid", "atos_codigodeincidenciaenedm", "atos_codigodeincidenciaenedmid", "atos_codigo", camposOmitidos);
                        ProcesarCampoString(xElement, solicitudAtr, "Comentarios", "atos_comentariosincidencia", camposOmitidos);
                    }
                    Entity atos_incidencia = new Entity("atos_incidencia");
                    ProcesarCampoFecha(xDoc.DocumentElement, atos_incidencia, "FechaIncidencia", "atos_fechaincidencia", camposOmitidos);
                    ProcesarCampoFecha(xDoc.DocumentElement, atos_incidencia, "FechaPrevistaAccion", "atos_fechaprevistaaccion", camposOmitidos);
                    ProcesarCampoEntero(xElement, atos_incidencia, "Secuencial", "atos_secuencial", camposOmitidos);
                    ProcesarCampoBusqueda(xElement, atos_incidencia, "CodigoMotivo", "atos_codigodeincidenciaenedmid", "atos_codigodeincidenciaenedm", "atos_codigodeincidenciaenedmid", "atos_codigo", camposOmitidos);
                    ProcesarCampoString(xElement, atos_incidencia, "Comentarios", "atos_comentarios", camposOmitidos);
                    //atos_incidencia.Attributes.Add("atos_solicitudatrid", new EntityReference("atos_solicitudatr", guid));
                    //ServicioCrm.Create(atos_incidencia);
                    relatedEntities.Entities.Add(atos_incidencia);
                }
                solicitudAtr.RelatedEntities.Add(relation, relatedEntities);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "solicitudCuerpoPaso03", ex);
                throw new Exception(message, ex);
            }
        }

        public void solicitudCuerpoPaso05(string pombreFichero, XmlDocument xDoc, Entity solicitudAtr, string agente = null, List<string> camposOmitidos = null)
        {
            List<String> salida = new List<String>();

            try
            {
                ProcesarCampoFecha(xDoc.DocumentElement, solicitudAtr, "Fecha", "atos_fechaactivacionprevista", camposOmitidos);
                ProcesarCampoString(xDoc.DocumentElement, solicitudAtr, "CodContrato", "atos_codigocontrato", camposOmitidos);
                ProcesarCampoBusqueda(xDoc.DocumentElement, solicitudAtr, "TipoAutoconsumo", "atos_tipoautoconsumoid", "atos_tipodeautoconsumo", "atos_tipodeautoconsumoid", "atos_codigo", camposOmitidos);
                ProcesarCampoBusqueda(xDoc.DocumentElement, solicitudAtr, "TipoContratoATR", "atos_tipocontratoatrid", "atos_tipocontratoatr", "atos_tipocontratoatrid", "atos_codigo", camposOmitidos);

                Guid tarifaATRId = ProcesarCampoBusqueda(xDoc.DocumentElement, solicitudAtr, "TarifaATR", "atos_tarifaid", "atos_tarifa", "atos_tarifaid", "atos_codigoocsum", camposOmitidos);
                if (tarifaATRId == Guid.Empty)
                    salida.Add(String.Format("Error en la lectura del fichero:{0};{1}", pombreFichero, "La tarifa es obligatoria"));
                // Hay que controlar que no se metan más potencias que periodos tenga la tarifa
                decimal periodosTarifa = this.periodosDeLaTarifa(tarifaATRId);

                ProcesarCampoBusqueda(xDoc.DocumentElement, solicitudAtr, "PeriodicidadFacturacion", "atos_periodicidadfacturacionid", "atos_periodicidadfacturacion", "atos_periodicidadfacturacionid", "atos_codigoperiodicidadfacturacion", camposOmitidos);
                ProcesarCampoBusqueda(xDoc.DocumentElement, solicitudAtr, "TipodeTelegestion", "atos_tipodetelegestionid", "atos_tipodetelegestion", "atos_tipodetelegestionid", "atos_codigo", camposOmitidos);

                XmlNodeList potenciasContratadas = xDoc.GetElementsByTagName("PotenciasContratadas");
                if (potenciasContratadas.Count > 0)
                {
                    int periodo = 1;
                    foreach (XmlElement potencia in potenciasContratadas[0])
                        if (this.dentroDeLosPeriodos(periodo, periodosTarifa) == true)
                            solicitudAtr.Attributes.Add("atos_potenciap" + periodo++, Convert.ToDecimal(potencia.InnerText) / 1000);
                }

                ProcesarCampoBusqueda(xDoc.DocumentElement, solicitudAtr, "ModoControlPotencia", "atos_modocontrolpotenciaid", "atos_modocontrolpotencia", "atos_modocontrolpotenciaid", "atos_codigo", camposOmitidos);
                ProcesarCampoBusqueda(xDoc.DocumentElement, solicitudAtr, "MarcaMedidaConPerdidas", "atos_marcamedidaconperdidasid", "atos_marcamedidaconperdidas", "atos_marcamedidaconperdidasid", "atos_codigo", camposOmitidos);
                ProcesarCampoBusqueda(xDoc.DocumentElement, solicitudAtr, "TensionDelSuministro", "atos_tensionsuministroapmid", "atos_tensionsuministroapm", "atos_tensionsuministroapmid", "atos_codigo", camposOmitidos);
                ProcesarCampoDecimal(xDoc.DocumentElement, solicitudAtr, "VAsTrafo", "atos_vastrafo", camposOmitidos);
                ProcesarCampoString(xDoc.DocumentElement, solicitudAtr, "PorcentajePerdidas", "atos_porcentajeperdidaspactadas", camposOmitidos);
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "solicitudCuerpoPaso05", ex);
                throw new Exception(message, ex);
            }
        }

        public void solicitudCuerpoPaso05Baja(string nombreFichero, XmlDocument xDoc, Entity solicitudAtr, string agente = null)
        {
            try
            {
                String infoActivacionFecha = xDoc.GetElementsByTagName("FechaActivacion")[0].InnerText;
                // String infoActivacionHora = xDoc.GetElementsByTagName("Hora")[0].InnerText;
                if (!String.IsNullOrEmpty(infoActivacionFecha))
                {

                    solicitudAtr.Attributes.Add("atos_fechaactivacionprevista", Convert.ToDateTime(infoActivacionFecha));
                }

                String codContrato = xDoc.GetElementsByTagName("CodContrato")[0].InnerText;
                if (!String.IsNullOrEmpty(codContrato))
                {
                    solicitudAtr.Attributes.Add("atos_codigocontrato", codContrato);
                }

                // Guid tipoContratoId = obtenerIdEntidad("atos_tipocontratoatr", "atos_tipocontratoatrid", "atos_codigo", xDoc.GetElementsByTagName("TipoContratoATR")[0].InnerText);
                //if (tipoContratoId != Guid.Empty)
                // {
                //     solicitudAtr.Attributes.Add("atos_tipocontratoatrid", new EntityReference("atos_tipocontratoatr", tipoContratoId));
                // }
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "solicitudCuerpoPaso05Baja", ex);
                throw new Exception(message, ex);
            }
        }

        public void solicitudProcesarFechas(string nombreFichero, XmlDocument xDoc, Entity solicitudAtr, string camposFecha)
        {
            solicitudProcesarFechas(nombreFichero, xDoc, solicitudAtr, new string[] { camposFecha });
        }

        public void solicitudProcesarFechas(String nombreFichero, XmlDocument xDoc, Entity solicitudAtr, string[] camposFecha)
        {
            try
            {
                foreach (string campoXml in camposFecha)
                {
                    string campoCrm = null;
                    switch (campoXml)
                    {
                        case "FechaRechazo": campoCrm = "atos_fecharechazo"; break;
                        case "FechaAceptacion": campoCrm = "atos_fechaaceptacion"; break;
                        case "FechaActivacion": campoCrm = "atos_fechaactivacionprevista"; break;
                        case "FechaActivacionPrevista": campoCrm = "atos_fechaactivacionprevista"; break;
                        default: throw new Exception(string.Format("Campo fecha '{0}'para el  fichero xml '{1}' desconocido.", campoXml, nombreFichero));
                    }
                    ProcesarCampoFecha(xDoc.DocumentElement, solicitudAtr, campoXml, campoCrm, null);
                }
            }
            catch (Exception ex)
            {
                string message = ErrorHandler.ConstructErrorMessage(xDoc, "solicitudProcesarFechas", ex);
                throw new Exception(message, ex);
            }
        }

        public void gestionarPuntosMedida(String pNombreFichero, XmlDocument xDoc, Entity solicitudAtr, List<string> camposOmitidos = null)
        {
            try
            {
                foreach (XmlElement xPunto in xDoc.GetElementsByTagName("PuntoDeMedida"))
                {
                    Entity atos_puntomedida = new Entity("atos_puntomedida");
                    gestionarPuntoMedida(xPunto, atos_puntomedida, camposOmitidos);
                    gestionarAparatos(xPunto, atos_puntomedida, camposOmitidos);
                    atos_puntomedida.Attributes.Add("atos_instalacionid", solicitudAtr.Attributes["atos_instalacionid"]);

                    ServicioCrm.Create(atos_puntomedida);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar los puntos de medida", ex);
            }
        }

        private void gestionarPuntoMedida(XmlElement xPunto, Entity puntoCrm, List<string> camposOmitidos)
        {
            try
            {
                ProcesarCampoString(xPunto, puntoCrm, "CodPM", "atos_codigopuntomedida", camposOmitidos);
                ProcesarCampoBusqueda(xPunto, puntoCrm, "TipoMovimiento", "atos_tipodemovimientopuntodemedidaid", "atos_tipodemovimientopuntodemedida", "atos_tipodemovimientopuntodemedidaid", "atos_codigo", camposOmitidos);
                ProcesarCampoBusqueda(xPunto, puntoCrm, "TipoPM", "atos_tipopuntomedidaid", "atos_tipopuntomedida", "atos_tipopuntomedidaid", "atos_codigo", camposOmitidos);
                ProcesarCampoString(xPunto, puntoCrm, "CodPMPrincipal", "atos_codpmprincipal", camposOmitidos);
                ProcesarCampoBusqueda(xPunto, puntoCrm, "ModoLectura", "atos_modolecturaid", "atos_modolecturapuntomedida", "atos_modolecturapuntomedidaid", "atos_codigo", camposOmitidos);
                ProcesarCampoBusqueda(xPunto, puntoCrm, "Funcion", "atos_funcionpuntomedidaid", "atos_funcionpuntomedida", "atos_funcionpuntomedidaid", "atos_codigo", camposOmitidos);
                ProcesarCampoString(xPunto, puntoCrm, "DireccionEnlace", "atos_direccionenlace", camposOmitidos);
                ProcesarCampoString(xPunto, puntoCrm, "DireccionPuntoMedida", "atos_direccionpuntomedida", camposOmitidos);
                ProcesarCampoString(xPunto, puntoCrm, "NumLinea", "atos_numlinea", camposOmitidos);
                ProcesarCampoString(xPunto, puntoCrm, "TelefonoTelemedida", "atos_telefonotelemedida", camposOmitidos);
                ProcesarCampoBusqueda(xPunto, puntoCrm, "EstadoTelefono", "atos_estadotelefonoid", "atos_estadotelefono", "atos_estadotelefonoid", "atos_codigo", camposOmitidos);
                ProcesarCampoString(xPunto, puntoCrm, "ClaveAcceso", "atos_claveacceso", camposOmitidos);
                ProcesarCampoDecimal(xPunto, puntoCrm, "TensionPM", "atos_tensionpm", camposOmitidos);
                ProcesarCampoFecha(xPunto, puntoCrm, "FechaVigor", "atos_fechavigor", camposOmitidos);
                ProcesarCampoFecha(xPunto, puntoCrm, "FechaAlta", "atos_fechaalta", camposOmitidos);
                ProcesarCampoFecha(xPunto, puntoCrm, "FechaBaja", "atos_fechabaja", camposOmitidos);
                ProcesarCampoString(xPunto, puntoCrm, "Comentarios", "atos_comentarios", camposOmitidos);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el punto de medida", ex);
            }
        }

        public void gestionarMedidas(XmlElement xPunto, Entity puntoCrm, List<string> camposOmitidos = null)
        {
            try
            {
                Relationship relationship = new Relationship("atos_atos_aparato_atos_medidaaparato_aparatoid");
                EntityCollection relatedEntites = new EntityCollection { EntityName = "atos_medidaaparato" };

                foreach (XmlElement xMedida in xPunto.GetElementsByTagName("Medida"))
                {
                    Entity atos_medida = new Entity("atos_medidaaparato");
                    gestionarMedida(xMedida, atos_medida, camposOmitidos);
                    relatedEntites.Entities.Add(atos_medida);
                }
                if (relatedEntites.Entities.Count > 0)
                    puntoCrm.RelatedEntities.Add(relationship, relatedEntites);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar las medidas para el aparato", ex);
            }
        }

        private void gestionarMedida(XmlElement xMedida, Entity medidaCrm, List<string> camposOmitidos)
        {
            try
            {
                ProcesarCampoBusqueda(xMedida, medidaCrm, "TipoDHEdM", "atos_tipodhid", "atos_tipodediscriminacionhoraria", "atos_tipodediscriminacionhorariaid", "atos_codigo", camposOmitidos);
                ProcesarCampoBusqueda(xMedida, medidaCrm, "Periodo", "atos_periodoid", "atos_codigoperiododiscriminacionhoraria", "atos_codigoperiododiscriminacionhorariaid", "atos_codigo", camposOmitidos);
                ProcesarCampoBusqueda(xMedida, medidaCrm, "MagnitudMedida", "atos_magnitudmedidaid", "atos_magnitudintegrador", "atos_magnitudintegradorid", "atos_codigo", camposOmitidos);
                ProcesarCampoBusqueda(xMedida, medidaCrm, "Procedencia", "atos_procedenciaid", "atos_procedencia", "atos_procedenciaid", "atos_codigo", camposOmitidos);
                ProcesarCampoDecimal(xMedida, medidaCrm, "UltimaLecturaFirme", "atos_lecturaanterior", camposOmitidos);
                ProcesarCampoFecha(xMedida, medidaCrm, "FechaLecturaFirme", "atos_fechalecturaanterior", camposOmitidos);
                ProcesarCampoBusqueda(xMedida, medidaCrm, "Anomalia", "atos_anomaliaid", "atos_codigodeanomalia", "atos_codigodeanomaliaid", "atos_codigo", camposOmitidos);
                ProcesarCampoString(xMedida, medidaCrm, "Comentarios", "atos_textoanomalia", camposOmitidos);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar la medida para el aparato", ex);
            }
        }

        public void gestionarAparatos(XmlElement xPunto, Entity puntoCrm, List<string> camposOmitidos = null)
        {
            try
            {
                Relationship relationship = new Relationship("atos_atos_puntomedida_atos_aparato_puntodemedidaid");
                EntityCollection relatedEntites = new EntityCollection { EntityName = "atos_aparato" };

                foreach (XmlElement xAparato in xPunto.GetElementsByTagName("Aparato"))
                {
                    Entity atos_aparato = new Entity("atos_aparato");
                    gestionarAparato(xAparato, atos_aparato, camposOmitidos);
                    gestionarMedidas(xPunto, atos_aparato, camposOmitidos);
                    relatedEntites.Entities.Add(atos_aparato);
                }
                if (relatedEntites.Entities.Count > 0)
                    puntoCrm.RelatedEntities.Add(relationship, relatedEntites);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar los aparatos para el punto de medida", ex);
            }
        }

        private void gestionarAparato(XmlElement xAparato, Entity aparatoCrm, List<string> camposOmitidos)
        {
            try
            {
                ProcesarCampoBusqueda(xAparato, aparatoCrm, "TipoAparato", "atos_tipoaparatoid", "atos_tipoaparato", "atos_tipoaparatoid", "atos_codigoaparato", camposOmitidos);
                ProcesarCampoBusqueda(xAparato, aparatoCrm, "MarcaAparato", "atos_marcaaparatoid", "atos_marcaaparato", "atos_marcaaparatoid", "atos_codigomarca", camposOmitidos);
                ProcesarCampoString(xAparato, aparatoCrm, "ModeloMarca", "atos_modelomarca", camposOmitidos);
                ProcesarCampoBusqueda(xAparato, aparatoCrm, "TipoMovimiento", "atos_tipomovimientoid", "atos_tipodemovimientodelaparato", "atos_tipodemovimientodelaparatoid", "atos_codigo", camposOmitidos);
                ProcesarCampoBusqueda(xAparato, aparatoCrm, "TipoEquipoMedida", "atos_tipoequipomedidaid", "atos_tipoequipomedida", "atos_tipoequipomedidaid", "atos_codigo", camposOmitidos);
                ProcesarCampoBusqueda(xAparato, aparatoCrm, "TipoPropiedadAparato", "atos_tipopropiedadaparatoid", "atos_tipopropiedadaparato", "atos_tipopropiedadaparatoid", "atos_codigo", camposOmitidos);
                ProcesarCampoString(xAparato, aparatoCrm, "Propietario", "atos_propietario", camposOmitidos);
                ProcesarCampoBusqueda(xAparato, aparatoCrm, "TipoDHEdM", "atos_tipodhid", "atos_tipodediscriminacionhoraria", "atos_tipodediscriminacionhorariaid", "atos_codigo", camposOmitidos);
                ProcesarCampoBusqueda(xAparato, aparatoCrm, "ModoMedidaPotencia", "atos_modofacturacionpotenciaid", "atos_modofacturacionpotencia", "atos_modofacturacionpotenciaid", "atos_codigo", camposOmitidos);
                ProcesarCampoBusqueda(xAparato, aparatoCrm, "LecturaDirecta", "atos_lecturadirectaid", "atos_indicativodelecturadirectanoacumulativa", "atos_indicativodelecturadirectanoacumulativaid", "atos_codigo", camposOmitidos);
                ProcesarCampoString(xAparato, aparatoCrm, "CodPrecinto", "atos_codprecinto", camposOmitidos);
                ProcesarCampoDecimal(xAparato, aparatoCrm, "PeriodoFabricacion", "atos_periodofabricacion", camposOmitidos);
                ProcesarCampoString(xAparato, aparatoCrm, "NumeroSerie", "atos_numeroserie", camposOmitidos);
                ProcesarCampoBusqueda(xAparato, aparatoCrm, "FuncionAparato", "atos_funcionaparatoid", "atos_funciondelaparato", "atos_funciondelaparatoid", "atos_codigo", camposOmitidos);
                ProcesarCampoDecimal(xAparato, aparatoCrm, "NumIntegradores", "atos_numerointegradores", camposOmitidos);
                ProcesarCampoDecimal(xAparato, aparatoCrm, "ConstanteEnergia", "atos_constanteenergia", camposOmitidos);
                ProcesarCampoDecimal(xAparato, aparatoCrm, "ConstanteMaximetro", "atos_constantemaximetro", camposOmitidos);
                ProcesarCampoDecimal(xAparato, aparatoCrm, "RuedasEnteras", "atos_ruedasenteras", camposOmitidos);
                ProcesarCampoDecimal(xAparato, aparatoCrm, "RuedasDecimales", "atos_ruedasdecimales", camposOmitidos);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el aparato", ex);
            }
        }

        public List<String> gestionarPuntosMedida_old(String pNombreFichero, XmlNodeList pPuntosMedida)
        {
            List<String> salida = new List<String>();
            try
            {
                GestionInstalaciones gestInstalaciones = new GestionInstalaciones(ServicioCrm);

                for (int contadorPunto = 0; contadorPunto < pPuntosMedida.Count; contadorPunto++)
                {
                    Entity punto = new Entity("atos_puntomedida");
                    foreach (XmlElement infoPunto in pPuntosMedida.Item(contadorPunto))
                    {
                        if (infoPunto.Name == "CodPM")
                        {
                            punto.Attributes.Add("atos_codigopuntomedida", infoPunto.InnerText);

                            Guid idInstalacion = GestionInstalaciones.obtenerIdInstalacion(infoPunto.InnerText.Substring(0, 20));
                            if (idInstalacion == Guid.Empty)
                            {
                                salida.Add(String.Format("Error en la creación de puntos de medida: CUPS {0} no encontrado en el sistema", infoPunto.InnerText));

                                return salida;
                            }
                            else
                            {
                                Guid idPuntoMedida = GestionInstalaciones.obtenerIdPuntoMedida(infoPunto.InnerText, idInstalacion);
                                if (idPuntoMedida != Guid.Empty)
                                {
                                    punto.Attributes.Add("atos_puntomedidaid", idPuntoMedida);
                                }
                            }

                            punto.Attributes.Add("atos_instalacionid", new EntityReference("atos_instalacion", idInstalacion));
                        }
                        else if (infoPunto.Name == "TipoPM")
                        {
                            Guid tipoPuntomedidaId = obtenerIdEntidad("atos_tipopuntomedida", "atos_tipopuntomedidaid", "atos_codigo", infoPunto.InnerText);
                            if (tipoPuntomedidaId != Guid.Empty)
                            {
                                punto.Attributes.Add("atos_tipopuntomedidaid", new EntityReference("atos_tipopuntomedida", tipoPuntomedidaId));
                            }
                        }
                        else if (infoPunto.Name == "ModoLectura")
                        {
                            Guid tipoModoLecturaId = obtenerIdEntidad("atos_modolecturapuntomedida", "atos_modolecturapuntomedidaid", "atos_codigo", infoPunto.InnerText);
                            if (tipoModoLecturaId != Guid.Empty)
                            {
                                punto.Attributes.Add("atos_modolecturaid", new EntityReference("atos_modolecturapuntomedida", tipoModoLecturaId));
                            }
                        }
                        else if (infoPunto.Name == "Funcion")
                        {
                            Guid funcionId = obtenerIdEntidad("atos_funcionpuntomedida", "atos_funcionpuntomedidaid", "atos_codigo", infoPunto.InnerText);
                            if (funcionId != Guid.Empty)
                            {
                                punto.Attributes.Add("atos_funcionpuntomedidaid", new EntityReference("atos_funcionpuntomedida", funcionId));
                            }
                        }
                        else if (infoPunto.Name == "DireccionEnlace")
                        {
                            punto.Attributes.Add("atos_direccionenlace", infoPunto.InnerText);
                        }
                        else if (infoPunto.Name == "DireccionPuntoMedida")
                        {
                            punto.Attributes.Add("atos_direccionpuntomedida", infoPunto.InnerText);
                        }
                        else if (infoPunto.Name == "TelefonoTelemedida")
                        {
                            punto.Attributes.Add("atos_telefonotelemedida", infoPunto.InnerText);
                        }
                        else if (infoPunto.Name == "EstadoTelefono")
                        {
                            Guid estadoTfno = obtenerIdEntidad("atos_estadotelefono", "atos_estadotelefonoid", "atos_codigo", infoPunto.InnerText);
                            if (estadoTfno != Guid.Empty)
                            {
                                punto.Attributes.Add("atos_estadotelefonoid", new EntityReference("atos_estadotelefono", estadoTfno));
                            }
                        }
                        else if (infoPunto.Name == "ClaveAcceso")
                        {
                            punto.Attributes.Add("atos_claveacceso", infoPunto.InnerText);
                        }
                        else if (infoPunto.Name == "FechaVigor")
                        {
                            punto.Attributes.Add("atos_fechavigor", Convert.ToDateTime(infoPunto.InnerText));
                        }
                        else if (infoPunto.Name == "FechaAlta")
                        {
                            punto.Attributes.Add("atos_fechaalta", Convert.ToDateTime(infoPunto.InnerText));
                        }
                        else if (infoPunto.Name == "Aparatos")
                        {
                            String errores = incluirInfoAparatos_old(infoPunto, punto);
                            if (!String.IsNullOrEmpty(errores))
                            {
                                throw new Exception(errores);
                            }
                        }
                    }
                    if (punto.Attributes.Contains("atos_instalacionid"))
                    {
                        if (punto.Attributes.Contains("atos_puntomedidaid"))
                        {
                            ServicioCrm.Update(punto);
                        }
                        else
                        {
                            Guid divisaId = obtenerIdEntidad("transactioncurrency", "transactioncurrencyid", "currencyname", "euro");
                            punto.Attributes.Add("transactioncurrencyid", new EntityReference("transactioncurrency", divisaId));

                            ServicioCrm.Create(punto);
                        }
                    }
                    else
                    {
                        salida.Add(String.Format("Error en la creación de puntos de medida: La instalación no es correcta"));
                    }
                }
            }
            catch (SoapException soex)
            {
                salida.Add(String.Format("Error en la creación de puntos de medida:{0}", soex.Detail.InnerText));
            }
            catch (Exception ex)
            {
                salida.Add(String.Format("Error en la creación de puntos de medida:{0}", ex.Message));
            }

            return salida;
        }

        public List<String> gestionarPuntosMedida(String pNombreFichero, XmlNodeList pPuntosMedida)
        {
            List<String> salida = new List<String>();
            try
            {
                GestionInstalaciones gestInstalaciones = new GestionInstalaciones(ServicioCrm);

                for (int contadorPunto = 0; contadorPunto < pPuntosMedida.Count; contadorPunto++)
                {
                    Entity punto = new Entity("atos_puntomedida");
                    foreach (XmlElement infoPunto in pPuntosMedida.Item(contadorPunto))
                    {
                        Guid idPuntoMedida = Guid.Empty;
                        if (infoPunto.Name == "CodPM")
                        {
                            punto.Attributes.Add("atos_codigopuntomedida", infoPunto.InnerText);

                            Guid idInstalacion = GestionInstalaciones.obtenerIdInstalacion(infoPunto.InnerText.Substring(0, 20));
                            if (idInstalacion == Guid.Empty)
                            {
                                salida.Add(String.Format("Error en la creación de puntos de medida: CUPS {0} no encontrado en el sistema", infoPunto.InnerText));
                                return salida;
                            }
                            else
                            {
                                idPuntoMedida = GestionInstalaciones.obtenerIdPuntoMedida(infoPunto.InnerText, idInstalacion);
                                if (idPuntoMedida != Guid.Empty)
                                {
                                    punto.Attributes.Add("atos_puntomedidaid", idPuntoMedida);
                                }
                            }

                            punto.Attributes.Add("atos_instalacionid", new EntityReference("atos_instalacion", idInstalacion));
                            // *** se necesita crear la entidad "padre" CUANTO ANTES para poder vincular las entidades hijas ***
                            if (punto.Attributes.Contains("atos_instalacionid"))
                            {
                                if (!punto.Attributes.Contains("atos_puntomedidaid"))
                                {
                                    Guid divisaId = obtenerIdEntidad("transactioncurrency", "transactioncurrencyid", "currencyname", "euro");
                                    punto.Attributes.Add("transactioncurrencyid", new EntityReference("transactioncurrency", divisaId));

                                    idPuntoMedida = ServicioCrm.Create(punto);
                                    punto.Attributes.Add("atos_puntomedidaid", idPuntoMedida); // TODO: es necesario??
                                }
                            }
                            else
                            {
                                salida.Add(String.Format("Error en la creación de puntos de medida: La instalación no es correcta"));
                                return salida;
                            }
                        }
                        else if (infoPunto.Name == "TipoPM")
                        {
                            Guid tipoPuntomedidaId = obtenerIdEntidad("atos_tipopuntomedida", "atos_tipopuntomedidaid", "atos_codigo", infoPunto.InnerText);
                            if (tipoPuntomedidaId != Guid.Empty)
                            {
                                punto.Attributes.Add("atos_tipopuntomedidaid", new EntityReference("atos_tipopuntomedida", tipoPuntomedidaId));
                            }
                        }
                        else if (infoPunto.Name == "ModoLectura")
                        {
                            Guid tipoModoLecturaId = obtenerIdEntidad("atos_modolecturapuntomedida", "atos_modolecturapuntomedidaid", "atos_codigo", infoPunto.InnerText);
                            if (tipoModoLecturaId != Guid.Empty)
                            {
                                punto.Attributes.Add("atos_modolecturaid", new EntityReference("atos_modolecturapuntomedida", tipoModoLecturaId));
                            }
                        }
                        else if (infoPunto.Name == "Funcion")
                        {
                            Guid funcionId = obtenerIdEntidad("atos_funcionpuntomedida", "atos_funcionpuntomedidaid", "atos_codigo", infoPunto.InnerText);
                            if (funcionId != Guid.Empty)
                            {
                                punto.Attributes.Add("atos_funcionpuntomedidaid", new EntityReference("atos_funcionpuntomedida", funcionId));
                            }
                        }
                        else if (infoPunto.Name == "DireccionEnlace")
                        {
                            punto.Attributes.Add("atos_direccionenlace", infoPunto.InnerText);
                        }
                        else if (infoPunto.Name == "DireccionPuntoMedida")
                        {
                            punto.Attributes.Add("atos_direccionpuntomedida", infoPunto.InnerText);
                        }
                        else if (infoPunto.Name == "TelefonoTelemedida")
                        {
                            punto.Attributes.Add("atos_telefonotelemedida", infoPunto.InnerText);
                        }
                        else if (infoPunto.Name == "EstadoTelefono")
                        {
                            Guid estadoTfno = obtenerIdEntidad("atos_estadotelefono", "atos_estadotelefonoid", "atos_codigo", infoPunto.InnerText);
                            if (estadoTfno != Guid.Empty)
                            {
                                punto.Attributes.Add("atos_estadotelefonoid", new EntityReference("atos_estadotelefono", estadoTfno));
                            }
                        }
                        else if (infoPunto.Name == "ClaveAcceso")
                        {
                            punto.Attributes.Add("atos_claveacceso", infoPunto.InnerText);
                        }
                        else if (infoPunto.Name == "FechaVigor")
                        {
                            punto.Attributes.Add("atos_fechavigor", Convert.ToDateTime(infoPunto.InnerText));
                        }
                        else if (infoPunto.Name == "FechaAlta")
                        {
                            punto.Attributes.Add("atos_fechaalta", Convert.ToDateTime(infoPunto.InnerText));
                        }
                        else if (infoPunto.Name == "Aparatos")
                        {
                            String errores = incluirInfoAparatos(infoPunto, punto);
                            if (!String.IsNullOrEmpty(errores))
                            {
                                throw new Exception(errores);
                            }
                        }
                    }
                    ServicioCrm.Update(punto);
                }
            }
            catch (SoapException soex)
            {
                salida.Add(String.Format("Error en la creación de puntos de medida:{0}", soex.Detail.InnerText));
            }
            catch (Exception ex)
            {
                salida.Add(String.Format("Error en la creación de puntos de medida:{0}", ex.Message));
            }

            return salida;
        }

        private string incluirInfoAparatos_old(XmlElement pInfoAparatos, Entity punto)
        {
            String salida = String.Empty;
            try
            {
                foreach (XmlElement infoAparato in pInfoAparatos.FirstChild)
                {
                    if (infoAparato.Name == "TipoEquipoMedida")
                    {
                        Guid tipoEquipoMedidaId = obtenerIdEntidad("atos_tipoequipomedida", "atos_tipoequipomedidaid", "atos_codigo", infoAparato.InnerText);
                        if (tipoEquipoMedidaId != Guid.Empty)
                        {
                            punto.Attributes.Add("atos_tipoequipomedidaid", new EntityReference("atos_tipoequipomedida", tipoEquipoMedidaId));
                        }
                    }
                    else if (infoAparato.Name == "TipoPropiedadAparato")
                    {
                        Guid tipoPropiedadAparatoId = obtenerIdEntidad("atos_tipopropiedadaparato", "atos_tipopropiedadaparatoid", "atos_codigo", infoAparato.InnerText);
                        if (tipoPropiedadAparatoId != Guid.Empty)
                        {
                            punto.Attributes.Add("atos_tipopropiedadaparatoid", new EntityReference("atos_tipopropiedadaparato", tipoPropiedadAparatoId));
                        }
                    }
                    else if (infoAparato.Name == "ExtraccionLecturas")
                    {
                        Guid extraccionLecturasId = obtenerIdEntidad("atos_extraccionlecturas", "atos_extraccionlecturasid", "atos_codigo", infoAparato.InnerText);
                        if (extraccionLecturasId != Guid.Empty)
                        {
                            punto.Attributes.Add("atos_extraccionlecturasid", new EntityReference("atos_extraccionlecturas", extraccionLecturasId));
                        }
                    }
                    else if (infoAparato.Name == "Modelo")
                    {
                        String msgError = incluirInfoModelo(infoAparato, punto);

                        if (!String.IsNullOrEmpty(msgError))
                        {
                            throw new Exception(msgError);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                salida = String.Format("Error en el cálculo de información de aparatos: {0}", ex.Message);
            }
            return salida;
        }

        private string incluirInfoAparatos(XmlElement aparatos, Entity punto)
        {
            String salida = String.Empty;
            try
            {
                Mapper mapper = new Mapper(ServicioCrm);
                definirMapeosAparatos(mapper);

                foreach (XmlElement aparatoXml in aparatos.SelectNodes("*"))
                {
                    Entity aparatoCRM = new Entity("atos_aparato");
                    List<Entity> medidasCRM = new List<Entity>();

                    foreach (XmlElement detalleAparatoXml in aparatoXml.SelectNodes("*"))
                    {
                        if (detalleAparatoXml.Name == "Medidas")
                        {
                            foreach (XmlElement medidaXml in detalleAparatoXml.SelectNodes("*"))
                            {
                                Entity medidaCRM = new Entity("atos_medidaaparato");
                                mapper.RecorerMapearCampo(medidaXml, medidaCRM);
                                medidasCRM.Add(medidaCRM);
                            }
                        }
                        else if (detalleAparatoXml.Name == "DatosAparatoNoICP" || detalleAparatoXml.Name == "DatosAparatoICP")
                        {
                            int datosAparatoICP = detalleAparatoXml.Name == "DatosAparatoNoICP" ? 300000000 : 300000001;
                            aparatoCRM.Attributes.Add("atos_datosaparato", new OptionSetValue(datosAparatoICP));
                            mapper.RecorerMapearCampo(detalleAparatoXml, aparatoCRM);
                        }
                        else
                            mapper.RecorerMapearCampo(detalleAparatoXml, aparatoCRM);
                    }

                    // join aparato al punto
                    aparatoCRM.Attributes.Add("atos_puntodemedidaid", new EntityReference("atos_puntomedida", (Guid)punto.Attributes["atos_puntomedidaid"]));
                    Guid aparatoId = ServicioCrm.Create(aparatoCRM);

                    // join medida al aparato
                    // createParentAttachChildren()
                    foreach (Entity medidaCRM in medidasCRM)
                    {
                        medidaCRM.Attributes.Add("atos_aparatoid", new EntityReference("atos_aparato", aparatoId));
                        ServicioCrm.Create(medidaCRM);
                    }
                    // informa datos del aparato principal al punto de medida (4 campos)
                    if (medidasCRM.Count > 0)
                    {
                        punto.Attributes["atos_tipoaparatoid"] = aparatoCRM.Attributes["atos_tipoaparatoid"];
                        punto.Attributes["atos_marcaaparatoid"] = aparatoCRM.Attributes["atos_marcaaparatoid"];
                        punto.Attributes["atos_tipoequipomedidaid"] = aparatoCRM.Attributes["atos_tipoequipomedidaid"];
                        punto.Attributes["atos_tipopropiedadaparatoid"] = aparatoCRM.Attributes["atos_tipopropiedadaparatoid"];
                    }
                }
            }
            catch (Exception ex)
            {
                salida = String.Format("Error en el cálculo de información de aparatos: {0}", ex.Message);
            }
            return salida;
        }

        private void definirMapeosAparatos(Mapper mapper)
        {
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.Lookup, CampoXml = "Tipo", CampoCRM = "atos_tipoaparatoid", LookupTabla = "atos_tipoaparato", LookupFiltros = new string[] { "atos_codigoaparato" } });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.Lookup, CampoXml = "Marca", CampoCRM = "atos_marcaaparatoid", LookupTabla = "atos_marcaaparato", LookupFiltros = new string[] { "atos_codigomarca" } });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.String, CampoXml = "ModeloMarca", CampoCRM = "atos_modelomarca" });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.Lookup, CampoXml = "TipoMovimiento", CampoCRM = "atos_tipomovimientoid", LookupTabla = "atos_tipodemovimientodelaparato", LookupClave = "atos_tipodemovimientodelaparatoid", LookupFiltros = new string[] { "atos_codigo" } });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.Lookup, CampoXml = "TipoEquipoMedida", CampoCRM = "atos_tipoequipomedidaid", LookupTabla = "atos_tipoequipomedida", LookupFiltros = new string[] { "atos_codigo" } });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.Lookup, CampoXml = "TipoPropiedadAparato", CampoCRM = "atos_tipopropiedadaparatoid", LookupTabla = "atos_tipopropiedadaparato", LookupFiltros = new string[] { "atos_codigo" } });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.String, CampoXml = "Propietario", CampoCRM = "atos_propietario" });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.Lookup, CampoXml = "ExtraccionLecturas", CampoCRM = "atos_extraccionlecturasid", LookupTabla = "atos_extraccionlecturas", LookupFiltros = new string[] { "atos_codigo" } });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.Lookup, CampoXml = "DiscriminacionHorariaActiva", CampoCRM = "atos_discriminacionhorariaactivaid", LookupTabla = "atos_tipodediscriminacionhoraria", LookupClave = "atos_tipodediscriminacionhorariaid", LookupFiltros = new string[] { "atos_codigo" } });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.Lookup, CampoXml = "DiscriminacionHorariaMaximas", CampoCRM = "atos_discriminacionhorariamaximasid", LookupTabla = "atos_modofacturacionpotencia", LookupClave = "atos_modofacturacionpotenciaid", LookupFiltros = new string[] { "atos_codigo" } });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.Lookup, CampoXml = "LecturaDirecta", CampoCRM = "atos_lecturadirectaid", LookupTabla = "atos_indicativodelecturadirectanoacumulativa", LookupClave = "atos_indicativodelecturadirectanoacumulativaid", LookupFiltros = new string[] { "atos_codigo" } });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.String, CampoXml = "CodPrecinto", CampoCRM = "atos_codprecinto" });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.Decimal, CampoXml = "PeriodoFabricacion", CampoCRM = "atos_periodofabricacion" });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.String, CampoXml = "NumeroSerie", CampoCRM = "atos_numeroserie" });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.Lookup, CampoXml = "FuncionAparato", CampoCRM = "atos_funcionaparatoid", LookupTabla = "atos_funciondelaparato", LookupClave = "atos_funciondelaparatoid", LookupFiltros = new string[] { "atos_codigo" } });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.Decimal, CampoXml = "NumIntegradores", CampoCRM = "atos_numerointegradores" });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.Decimal, CampoXml = "ConstanteEnergia", CampoCRM = "atos_constanteenergia" });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.Decimal, CampoXml = "ConstanteMaximetro", CampoCRM = "atos_constantemaximetro" });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.Decimal, CampoXml = "RuedasEnteras", CampoCRM = "atos_ruedasenteras" });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.Decimal, CampoXml = "RuedasDecimales", CampoCRM = "atos_ruedasdecimales" });

            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.Lookup, CampoXml = "TipoDH", CampoCRM = "atos_tipodhid", LookupTabla = "atos_tipodediscriminacionhoraria", LookupClave = "atos_tipodediscriminacionhorariaid", LookupFiltros = new string[] { "atos_codigo" } });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.Lookup, CampoXml = "Periodo", CampoCRM = "atos_periodoid", LookupTabla = "atos_codigoperiododiscriminacionhoraria", LookupClave = "atos_codigoperiododiscriminacionhorariaid", LookupFiltros = new string[] { "atos_codigo" } });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.Lookup, CampoXml = "MagnitudMedida", CampoCRM = "atos_magnitudmedidaid", LookupTabla = "atos_magnitudintegrador", LookupClave = "atos_magnitudintegradorid", LookupFiltros = new string[] { "atos_codigo" } });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.Lookup, CampoXml = "Procedencia", CampoCRM = "atos_procedenciaid", LookupTabla = "atos_procedencia", LookupFiltros = new string[] { "atos_codigo" } });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.Decimal, CampoXml = "LecturaAnterior", CampoCRM = "atos_lecturaanterior" });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.Lookup, CampoXml = "Anomalia", CampoCRM = "atos_anomaliaid", LookupTabla = "atos_codigodeanomalia", LookupClave = "atos_codigodeanomaliaid", LookupFiltros = new string[] { "atos_codigo" } });
            mapper.AgregarMapeo(new Mapeo { TipoMapeo = MapperTipo.String, CampoXml = "TextoAnomalia", CampoCRM = "atos_textoanomalia" });
        }

        private String incluirInfoModelo(XmlElement infoAparato, Entity punto)
        {
            String salida = string.Empty;

            try
            {
                foreach (XmlElement infoMarca in infoAparato)
                {
                    if (infoMarca.Name == "Tipo")
                    {
                        Guid tipoID = obtenerIdEntidad("atos_tipoaparato", "atos_tipoaparatoid", "atos_codigoaparato", infoMarca.InnerText);
                        if (tipoID != Guid.Empty)
                        {
                            punto.Attributes.Add("atos_tipoaparatoid", new EntityReference("atos_tipoaparato", tipoID));
                        }
                    }
                    else if (infoMarca.Name == "Marca")
                    {
                        Guid marcaID = obtenerIdEntidad("atos_marcaaparato", "atos_marcaaparatoid", "atos_codigomarca", infoMarca.InnerText);
                        if (marcaID != Guid.Empty)
                        {
                            punto.Attributes.Add("atos_marcaaparatoid", new EntityReference("atos_marcaaparato", marcaID));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                salida = String.Format("Error en el cálculo de información de marca y modelo:{0}", ex.Message);
            }
            return salida;
        }

        public static Guid obtenerIdProcesoATR(String pCodProceso)
        {
            Guid salida = new Guid();
            try
            {
                QueryByAttribute consulta = new QueryByAttribute("atos_procesoatr");

                consulta.ColumnSet = new ColumnSet("atos_procesoatrid");
                consulta.Attributes.AddRange("atos_codigoproceso");
                consulta.Values.AddRange(pCodProceso);

                EntityCollection resConsulta = ServicioCrm.RetrieveMultiple(consulta);

                if (resConsulta.Entities.Count > 0)
                {
                    salida = resConsulta.Entities[0].Id;
                }
            }
            catch (SoapException soapEx)
            {
                throw new Exception(String.Format("Error en la consulta de proceso de ATR:{0}", soapEx.Detail.InnerText));
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error en la consulta de proceso de ATR:{0}", ex.Message));

            }
            return salida;
        }

        public static Guid obtenerIdPasoATR(String pCodPaso, Guid pIdProcesoATR)
        {
            Guid salida = new Guid();
            try
            {

                QueryByAttribute consulta = new QueryByAttribute("atos_pasoatr");

                consulta.ColumnSet = new ColumnSet("atos_pasoatrid");
                consulta.Attributes.AddRange("atos_codigopaso", "atos_procesoatrid");
                consulta.Values.AddRange(pCodPaso, pIdProcesoATR);

                EntityCollection resConsulta = ServicioCrm.RetrieveMultiple(consulta);

                if (resConsulta.Entities.Count > 0)
                {
                    salida = resConsulta.Entities[0].Id;
                }
            }
            catch (SoapException soapEx)
            {
                throw new Exception(String.Format("Error en la consulta de paso de ATR:{0}", soapEx.Detail.InnerText));
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error en la consulta de paso de ATR:{0}", ex.Message));

            }
            return salida;
        }

        public static Guid obtenerIdComercializadora(string pCodigoREE)
        {
            Guid salida = new Guid();
            try
            {

                QueryByAttribute consulta = new QueryByAttribute("atos_comercializadora");

                consulta.ColumnSet = new ColumnSet("atos_comercializadoraid");
                consulta.Attributes.AddRange("atos_codigo");
                consulta.Values.AddRange(pCodigoREE);

                EntityCollection resConsulta = ServicioCrm.RetrieveMultiple(consulta);

                if (resConsulta.Entities.Count > 0)
                {
                    salida = resConsulta.Entities[0].Id;
                }
            }
            catch (SoapException soapEx)
            {
                throw new Exception(String.Format("Error en la consulta de comercializadora:{0}", soapEx.Detail.InnerText));
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error en la consulta de comercializadora:{0}", ex.Message));
            }
            return salida;
        }

        public static Guid obtenerIdDistribuidora(string pCodigoREE)
        {
            Guid salida = new Guid();
            try
            {

                QueryByAttribute consulta = new QueryByAttribute("atos_distribuidora");

                consulta.ColumnSet = new ColumnSet("atos_distribuidoraid");
                consulta.Attributes.AddRange("atos_codigoocsum");
                consulta.Values.AddRange(pCodigoREE);

                EntityCollection resConsulta = ServicioCrm.RetrieveMultiple(consulta);

                if (resConsulta.Entities.Count > 0)
                {
                    salida = resConsulta.Entities[0].Id;
                }
            }
            catch (SoapException soapEx)
            {
                throw new Exception(String.Format("Error en la consulta de distribuidora:{0}", soapEx.Detail.InnerText));
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error en la consulta de distribuidora:{0}", ex.Message));

            }
            return salida;
        }

        public static Guid obtenerIdVersion(string pVersion)
        {
            Guid salida = new Guid();
            try
            {

                QueryByAttribute consulta = new QueryByAttribute("atos_versionatr");

                consulta.ColumnSet = new ColumnSet("atos_versionatrid");
                consulta.Attributes.AddRange("atos_codigo");
                consulta.Values.AddRange(pVersion);

                EntityCollection resConsulta = ServicioCrm.RetrieveMultiple(consulta);

                if (resConsulta.Entities.Count > 0)
                {
                    salida = resConsulta.Entities[0].Id;
                }
            }
            catch (SoapException soapEx)
            {
                throw new Exception(String.Format("Error en la consulta de versión ATR:{0}", soapEx.Detail.InnerText));
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error en la consulta de versión ATR:{0}", ex.Message));

            }
            return salida;
        }

        public static Guid obtenerIdEntidad(string nombreEntidad, string nombreCampoId, string filtroCampo, string filtroValor)
        {
            return obtenerIdEntidad(nombreEntidad, nombreCampoId, new string[] { filtroCampo }, new object[] { filtroValor });
        }
        public static Guid obtenerIdEntidad(string nombreEntidad, string nombreCampoId, string[] filtroCampos, object[] filtroValores)
        {
            Guid salida = new Guid();
            try
            {

                QueryByAttribute consulta = new QueryByAttribute(nombreEntidad);

                consulta.ColumnSet = new ColumnSet(nombreCampoId);
                consulta.Attributes.AddRange(filtroCampos);
                consulta.Values.AddRange(filtroValores);

                EntityCollection resConsulta = ServicioCrm.RetrieveMultiple(consulta);

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
                throw new Exception(String.Format("Error en la consulta de {0}:{1}", nombreEntidad, ex.Message));
            }
            return salida;
        }

        public static Guid CreateSolicitudATR(Entity solicitudATR)
        {
            Guid guid = new Guid();
            guid = ServicioCrm.Create(solicitudATR);
            solicitudATR.Attributes.Add("atos_solicitudatrid", guid);
            return guid;
        }

        public static Guid UpsertSolicitudATR(Entity solicitudATR)
        {
            Guid salida = new Guid();
            try
            {
                if (solicitudATR.Attributes.Count == 0)
                    return salida;
                else if (solicitudATR.Contains("atos_solicitudatrid"))
                {
                    ServicioCrm.Update(solicitudATR);
                    salida = (Guid)solicitudATR.Attributes["atos_solicitudatrid"];
                }
                else
                    salida = CreateSolicitudATR(solicitudATR);
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

        public static Guid adjuntarXmlSolicitud(Guid pIdSolicitud, String pCodProceso, String pCodPaso, String pNombreFichero, String pEncodedData)
        {
            Guid salida = new Guid();
            try
            {
                string hoy = DateTime.Now.ToLocalTime().Year.ToString("0000") + "-" + DateTime.Now.ToLocalTime().Month.ToString("00") + "-" +
                        DateTime.Now.ToLocalTime().Day.ToString("00") + " " + DateTime.Now.ToLocalTime().Hour.ToString("00") + ":" +
                        DateTime.Now.ToLocalTime().Minute.ToString("00") + ":" + DateTime.Now.ToLocalTime().Second.ToString("00");

                Entity _annotation = new Entity("annotation");
                _annotation.Attributes["objectid"] = new EntityReference("atos_solicitudatr", pIdSolicitud);
                _annotation.Attributes["objecttypecode"] = "atos_solicitudatr";
                _annotation.Attributes["subject"] = "Solicitud ATR " + pCodProceso + "/" + pCodPaso + " " + hoy;
                _annotation.Attributes["documentbody"] = pEncodedData;
                _annotation.Attributes["mimetype"] = @"application/xml";
                _annotation.Attributes["notetext"] = "Solicitud ATR " + pCodProceso + "/" + pCodPaso;
                //if (!String.IsNullOrEmpty(pDescripcion))
                //{
                //    _annotation.Attributes["notetext"] += " " + pDescripcion;
                //}
                _annotation.Attributes["filename"] = pNombreFichero;

                salida = ServicioCrm.Create(_annotation);
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

        public static Guid obtenerIdTarifaATR(string pTarifaOCSUM)
        {
            Guid salida = new Guid();
            try
            {

                QueryByAttribute consulta = new QueryByAttribute("atos_tarifa");

                consulta.ColumnSet = new ColumnSet("atos_tarifaid");
                consulta.Attributes.AddRange("atos_codigoocsum");
                consulta.Values.AddRange(pTarifaOCSUM);

                EntityCollection resConsulta = ServicioCrm.RetrieveMultiple(consulta);

                if (resConsulta.Entities.Count > 0)
                {
                    salida = resConsulta.Entities[0].Id;
                }
            }
            catch (SoapException soapEx)
            {
                throw new Exception(String.Format("Error en la consulta de tarifa ATR:{0}", soapEx.Detail.InnerText));
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error en la consulta de tarifa ATR:{0}", ex.Message));

            }
            return salida;
        }

        public static Guid obtenerIdActuacionCampo(string pCodActuacionCampo)
        {
            Guid salida = new Guid();
            try
            {

                QueryByAttribute consulta = new QueryByAttribute("atos_actuacionencampo");

                consulta.ColumnSet = new ColumnSet("atos_actuacionencampoid");
                consulta.Attributes.AddRange("atos_codigo");
                consulta.Values.AddRange(pCodActuacionCampo);

                EntityCollection resConsulta = ServicioCrm.RetrieveMultiple(consulta);

                if (resConsulta.Entities.Count > 0)
                {
                    salida = resConsulta.Entities[0].Id;
                }
            }
            catch (SoapException soapEx)
            {
                throw new Exception(String.Format("Error en la consulta de actuación en campo:{0}", soapEx.Detail.InnerText));
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error en la consulta de actuación en campo:{0}", ex.Message));

            }
            return salida;
        }

        public static Guid obtenerIdModoControlPotencia(string pCodModoControlPot)
        {
            Guid salida = new Guid();
            try
            {

                QueryByAttribute consulta = new QueryByAttribute("atos_modocontrolpotencia");

                consulta.ColumnSet = new ColumnSet("atos_modocontrolpotenciaid");
                consulta.Attributes.AddRange("atos_codigo");
                consulta.Values.AddRange(pCodModoControlPot);

                EntityCollection resConsulta = ServicioCrm.RetrieveMultiple(consulta);

                if (resConsulta.Entities.Count > 0)
                {
                    salida = resConsulta.Entities[0].Id;
                }
            }
            catch (SoapException soapEx)
            {
                throw new Exception(String.Format("Error en la consulta de modo de control de potencia:{0}", soapEx.Detail.InnerText));
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error en la consulta de modo de control de potencia:{0}", ex.Message));

            }
            return salida;
        }

        public static Guid obtenerIdMotivoRechazo(string pCodMotivoRechazo)
        {
            Guid salida = new Guid();
            try
            {

                QueryByAttribute consulta = new QueryByAttribute("atos_motivosrechazoatr");

                consulta.ColumnSet = new ColumnSet("atos_motivosrechazoatrid");
                consulta.Attributes.AddRange("atos_codigomotivorechazoatr");
                consulta.Values.AddRange(pCodMotivoRechazo);

                EntityCollection resConsulta = ServicioCrm.RetrieveMultiple(consulta);

                if (resConsulta.Entities.Count > 0)
                {
                    salida = resConsulta.Entities[0].Id;
                }
            }
            catch (SoapException soapEx)
            {
                throw new Exception(String.Format("Error en la consulta de motivos de rechazo:{0}", soapEx.Detail.InnerText));
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error en la consulta de motivos de rechazo:{0}", ex.Message));

            }
            return salida;
        }
    }

}
