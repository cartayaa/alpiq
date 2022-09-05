using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using System.Xml;
using Negocio.Constantes;
using Negocio.EntidadesXsd;
using Negocio.EntidadesCrm;
using System.Xml.Serialization;
using System.IO;

namespace Negocio.Logica
{
    public class GestionFacturaEMS : NegocioBase
    {
        private Mapper _mapper = null;

        public GestionFacturaEMS() :
            base()
        {

        }
        public GestionFacturaEMS(IOrganizationService servicioCrm) :
            base(servicioCrm)
        {
            _mapper = new Mapper(servicioCrm);
        }

        public List<String> CargarFactura_old(XmlDocument xDoc, atos_facturadistribuidora facturaCRM)
        {
            List<string> errores = new List<string>();

            MensajeFacturacion mensajeFacturacion = serializeXmlObject<MensajeFacturacion>(xDoc);
            FacturasFacturaATR facturaXSD = mensajeFacturacion.Facturas.Item as FacturasFacturaATR;
            FacturasRegistroFin registroFin = mensajeFacturacion.Facturas.RegistroFin;

            cargarCabecera(facturaCRM, mensajeFacturacion);
            cargarDireccionSuministroATR(facturaCRM, facturaXSD.DatosGeneralesFacturaATR, xDoc);
            cargarClienteATR(facturaCRM, facturaXSD.DatosGeneralesFacturaATR, xDoc);
            cargarDatosGeneralesFacturaATR(facturaCRM, facturaXSD.DatosGeneralesFacturaATR.DatosGeneralesFactura, xDoc);
            cargarDatosFacturaATR(facturaCRM, facturaXSD.DatosGeneralesFacturaATR.DatosFacturaATR, xDoc);

            cargarPotencia(facturaCRM, facturaXSD.Potencia, xDoc);
            cargarExcesoPotencia(facturaCRM, facturaXSD.ExcesoPotencia);
            cargarEnergiaActiva(facturaCRM, facturaXSD.EnergiaActiva);
            cargarEnergiaReactiva(facturaCRM, facturaXSD.EnergiaReactiva);
            cargarConceptosFacturaATR(facturaCRM, facturaXSD, xDoc);
            cargarConceptosRepercutiblesATR(facturaCRM, facturaXSD, xDoc);
            cargarMedidasAparatoIntegrador(facturaCRM, facturaXSD);
            cargarRegistroFin(facturaCRM, registroFin, xDoc);
            cargarOtrosDatosFactura(facturaCRM, mensajeFacturacion.OtrosDatosFactura);

            ServicioCrm.Create(facturaCRM);

            return errores;
        }

        /// <summary>
        /// Método publico que carga los datos de la factura via XmlDocument a la entidad tipada de crm 'atos_facturadistribuidora'
        /// </summary>
        /// <param name="xDoc">XmlDocument que almacena todos los datos de la factura cargados del fichero xml</param>
        /// <param name="facturaCRM">Entidad CRM donde se almaceneran los datos de la factura</param>
        /// <returns>Coleccion de cadenas de errores</returns>
        public List<String> CargarFactura(XmlDocument xDoc, atos_facturadistribuidora facturaCRM)
        {
            List<string> errores = new List<string>();

            MensajeFacturacion mensajeFacturacion = serializeXmlObject<MensajeFacturacion>(xDoc);
            FacturasRegistroFin registroFin = mensajeFacturacion.Facturas.RegistroFin;

            if (mensajeFacturacion.Facturas.Item is FacturasFacturaATR)
                cargarFacturaATR(facturaCRM, mensajeFacturacion.Facturas.Item as FacturasFacturaATR, xDoc);
            else
                cargarFacturaOtras(facturaCRM, mensajeFacturacion.Facturas.Item as FacturasOtrasFacturas, xDoc);

            cargarCabecera(facturaCRM, mensajeFacturacion);
            cargarRegistroFin(facturaCRM, registroFin, xDoc);
            cargarOtrosDatosFactura(facturaCRM, mensajeFacturacion.OtrosDatosFactura);
            cargarFirma(facturaCRM, xDoc);
            marcarFacturaPendienteEnvio(facturaCRM);

            ServicioCrm.Create(facturaCRM);

            return errores;
        }

        private void marcarFacturaPendienteEnvio(atos_facturadistribuidora facturaCRM)
        {
            facturaCRM.atos_pendienteenvio = new OptionSetValue(300000001);
            facturaCRM.atos_pendienteenviodesde = DateTime.Now;
        }

        private void cargarFirma(atos_facturadistribuidora facturaCRM, XmlDocument xDoc)
        {
            string tagFirma = obtenerElementoFirma(xDoc);
            if (xDoc.GetElementsByTagName(tagFirma).Count > 0)
            {
                string firmaBloque = xDoc.GetElementsByTagName(tagFirma)[0].OuterXml;
                facturaCRM.atos_firmabloque1 = extraerBloque(ref firmaBloque, 4000);
                facturaCRM.atos_firmabloque2 = extraerBloque(ref firmaBloque, 4000);
                facturaCRM.atos_firmabloque3 = extraerBloque(ref firmaBloque, 4000);
            }
        }

        private string obtenerElementoFirma(XmlDocument xDoc)
        {
            foreach (XmlNode node in xDoc.GetElementsByTagName("MensajeFacturacion")[0].ChildNodes)
            {
                if (node.LocalName.ToLower().Contains("signature"))
                    return node.Name;
            }

            return string.Empty;
        }
        private string extraerBloque(ref string bloque, int longitud)
        {
            if (bloque == null) return null;

            string extraido = null;
            if (longitud <= bloque.Length)
            {
                extraido = bloque.Substring(0, longitud);
                bloque = bloque.Remove(0, longitud);
            }
            else
            {
                extraido = bloque;
                bloque = null;
            }
            return extraido;
        }

        /// <summary>
        /// Método privado que carga todos los datos de 'Otras Facturas'
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="xsdFacturaOtras">Clase tipada construida a través del fichero xsd que almacena todos los datos de la factura según el tipo de factura</param>
        private void cargarFacturaOtras(atos_facturadistribuidora facturaCRM, FacturasOtrasFacturas xsdFacturaOtras, XmlDocument xDoc)
        {
            facturaCRM.atos_facturatipoatr = false;
            cargarDireccionSuministroOtras(facturaCRM, xsdFacturaOtras.DatosGeneralesOtrasFacturas, xDoc);
            cargarClienteOtras(facturaCRM, xsdFacturaOtras.DatosGeneralesOtrasFacturas, xDoc);
            cargarDatosGeneralesFacturaOtras(facturaCRM, xsdFacturaOtras.DatosGeneralesOtrasFacturas.DatosGeneralesFactura, xDoc);
            cargarConceptosFacturaOtras(facturaCRM, xsdFacturaOtras);
            cargarConceptosRepercutiblesOtras(facturaCRM, xsdFacturaOtras, xDoc);
        }
        /// <summary>
        /// /// Método privado que carga todos los datos de 'Facturas ATR'
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="xsdFacturaATR">Clase tipada construida a través del fichero xsd que almacena todos los datos de la factura según el tipo de factura</param>
        private void cargarFacturaATR(atos_facturadistribuidora facturaCRM, FacturasFacturaATR xsdFacturaATR, XmlDocument xDoc)
        {
            facturaCRM.atos_facturatipoatr = true;
            cargarDireccionSuministroATR(facturaCRM, xsdFacturaATR.DatosGeneralesFacturaATR, xDoc);
            cargarClienteATR(facturaCRM, xsdFacturaATR.DatosGeneralesFacturaATR, xDoc);
            cargarDatosGeneralesFacturaATR(facturaCRM, xsdFacturaATR.DatosGeneralesFacturaATR.DatosGeneralesFactura, xDoc);
            cargarDatosFacturaATR(facturaCRM, xsdFacturaATR.DatosGeneralesFacturaATR.DatosFacturaATR, xDoc);
            cargarPotencia(facturaCRM, xsdFacturaATR.Potencia, xDoc);
            cargarExcesoPotencia(facturaCRM, xsdFacturaATR.ExcesoPotencia);
            cargarEnergiaActiva(facturaCRM, xsdFacturaATR.EnergiaActiva);
            cargarEnergiaReactiva(facturaCRM, xsdFacturaATR.EnergiaReactiva);
            cargarConceptosFacturaATR(facturaCRM, xsdFacturaATR, xDoc);
            cargarMedidasAparatoIntegrador(facturaCRM, xsdFacturaATR);
            cargarConceptosRepercutiblesATR(facturaCRM, xsdFacturaATR, xDoc);
        }

        /// <summary>
        /// Método privado que carga los datos de la 'Cabecera'
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="mensajeFacturacion">Clase tipada construida a través del fichero xsd que almacena los datos de cabecera de la factura según el tipo de factura</param>
        /// <returns>Coleccion de cadenas de errores</returns>
        private List<string> cargarCabecera(atos_facturadistribuidora facturaCRM, MensajeFacturacion mensajeFacturacion)
        {
            List<string> errores = new List<string>();

            facturaCRM.atos_distribuidoraid = createEntityReference("atos_distribuidora", "atos_distribuidoraid", "atos_codigoocsum", mensajeFacturacion.Cabecera.CodigoREEEmpresaEmisora);
            facturaCRM.atos_comercializadoraid = createEntityReference("atos_comercializadora", "atos_comercializadoraid", "atos_codigo", mensajeFacturacion.Cabecera.CodigoREEEmpresaDestino);
            facturaCRM.atos_codigosolicitud = mensajeFacturacion.Cabecera.CodigoDeSolicitud;
            facturaCRM.atos_secuencialsolicitud = mensajeFacturacion.Cabecera.SecuencialDeSolicitud;
            facturaCRM.atos_fechasolicitud = Convert.ToDateTime(mensajeFacturacion.Cabecera.FechaSolicitud);
            facturaCRM.atos_instalacionid = createEntityReference("atos_instalacion", "atos_instalacionid", new string[] { "atos_cups20", "atos_historico" }, new object[] { mensajeFacturacion.Cabecera.CUPS.Substring(0, 20), false });
            facturaCRM.atos_cups22 = mensajeFacturacion.Cabecera.CUPS;

            return errores;
        }

        /// <summary>
        /// Método privado que carga los datos de la sección 'Direccion Suministro' para facturas tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los 'datos generales' de la factura</param>
        /// <param name="datosGenerales">Clase tipada construida a través del fichero xsd que almacena los 'datos generales' de la factura según el tipo de factura</param>
        /// <returns>Coleccion de cadenas de errores</returns>
        private List<string> cargarDireccionSuministroATR(atos_facturadistribuidora facturaCRM, FacturasFacturaATRDatosGeneralesFacturaATR datosGenerales, XmlDocument xDoc)
        {
            List<string> errores = new List<string>();

            facturaCRM.atos_pais = datosGenerales.DireccionSuministro.Pais;
            facturaCRM.atos_codigopais = datosGenerales.DireccionSuministro.Pais.Substring(0, datosGenerales.DireccionSuministro.Pais.Length > 5 ? 5 : datosGenerales.DireccionSuministro.Pais.Length);
            facturaCRM.atos_codigoprovincia = datosGenerales.DireccionSuministro.Provincia;
            facturaCRM.atos_codigomunicipiotexto = datosGenerales.DireccionSuministro.Municipio;
            facturaCRM.atos_codigopoblacion = datosGenerales.DireccionSuministro.Poblacion;
            if (xDoc.GetElementsByTagName("TipoVia").Count > 0)
                facturaCRM.atos_tipovia = parseEnum(datosGenerales.DireccionSuministro.TipoVia);
            facturaCRM.atos_codigopostal = datosGenerales.DireccionSuministro.CodPostal;
            facturaCRM.atos_dirsuministro = datosGenerales.DireccionSuministro.Calle;
            facturaCRM.atos_numerofinca = datosGenerales.DireccionSuministro.NumeroFinca;
            facturaCRM.atos_duplicadorfinca = datosGenerales.DireccionSuministro.DuplicadorFinca;
            facturaCRM.atos_escalera = datosGenerales.DireccionSuministro.Escalera;
            facturaCRM.atos_piso = datosGenerales.DireccionSuministro.Piso;
            facturaCRM.atos_puerta = datosGenerales.DireccionSuministro.Puerta;
            if (xDoc.GetElementsByTagName("TipoAclaradorFinca").Count > 0)
                facturaCRM.atos_tipoaclaradorfinca = parseEnum(datosGenerales.DireccionSuministro.TipoAclaradorFinca);
            facturaCRM.atos_aclaradorfinca = datosGenerales.DireccionSuministro.AclaradorFinca;

            return errores;
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Direccion Suministro' para otras facturas NO tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los 'datos generales' de la factura</param>
        /// <param name="datosGenerales">Clase tipada construida a través del fichero xsd que almacena los 'datos generales' de la factura según el tipo de factura</param>
        /// <returns>Coleccion de cadenas de errores</returns>
        private List<string> cargarDireccionSuministroOtras(atos_facturadistribuidora facturaCRM, FacturasOtrasFacturasDatosGeneralesOtrasFacturas datosGenerales, XmlDocument xDoc)
        {
            List<string> errores = new List<string>();

            facturaCRM.atos_pais = datosGenerales.DireccionSuministro.Pais;
            facturaCRM.atos_codigopais = datosGenerales.DireccionSuministro.Pais.Substring(0, datosGenerales.DireccionSuministro.Pais.Length > 5 ? 5 : datosGenerales.DireccionSuministro.Pais.Length);
            facturaCRM.atos_codigoprovincia = datosGenerales.DireccionSuministro.Provincia;
            facturaCRM.atos_codigomunicipiotexto = datosGenerales.DireccionSuministro.Municipio;
            facturaCRM.atos_codigopoblacion = datosGenerales.DireccionSuministro.Poblacion;
            if (xDoc.GetElementsByTagName("TipoVia").Count > 0)
                facturaCRM.atos_tipovia = parseEnum(datosGenerales.DireccionSuministro.TipoVia);
            facturaCRM.atos_codigopostal = datosGenerales.DireccionSuministro.CodPostal;
            facturaCRM.atos_dirsuministro = datosGenerales.DireccionSuministro.Calle;
            facturaCRM.atos_numerofinca = datosGenerales.DireccionSuministro.NumeroFinca;
            facturaCRM.atos_duplicadorfinca = datosGenerales.DireccionSuministro.DuplicadorFinca;
            facturaCRM.atos_escalera = datosGenerales.DireccionSuministro.Escalera;
            facturaCRM.atos_piso = datosGenerales.DireccionSuministro.Piso;
            facturaCRM.atos_puerta = datosGenerales.DireccionSuministro.Puerta;
            if (xDoc.GetElementsByTagName("TipoAclaradorFinca").Count > 0)
                facturaCRM.atos_tipoaclaradorfinca = parseEnum(datosGenerales.DireccionSuministro.TipoAclaradorFinca);
            facturaCRM.atos_aclaradorfinca = datosGenerales.DireccionSuministro.AclaradorFinca;

            return errores;
        }

        /// <summary>
        /// Método privado que carga los datos de la sección 'Cliente' para facturas tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los 'datos de cliente' de la factura</param>
        /// <param name="datosGenerales">Clase tipada construida a través del fichero xsd que almacena los 'datos generales' de la factura para facturas tipo ATR</param>
        /// <returns>Coleccion de cadenas de errores</returns>
        private List<string> cargarClienteATR(atos_facturadistribuidora facturaCRM, FacturasFacturaATRDatosGeneralesFacturaATR datosGenerales, XmlDocument xDoc)
        {
            List<string> errores = new List<string>();

            facturaCRM.atos_razonsocialid = createEntityReference("account", "accountid", "atos_numerodedocumento", datosGenerales.Cliente.Identificador);
            facturaCRM.atos_codigocontratoatr = datosGenerales.CodContrato.ToString();
            if (xDoc.GetElementsByTagName("TipoPersona").Count > 0)
                facturaCRM.atos_tipopersonaid = createEntityReference("atos_tipopersona", "atos_tipopersonaid", "atos_codigo", datosGenerales.Cliente.TipoPersona.ToString());
            facturaCRM.atos_tipoidentificadorid = createEntityReference("atos_tipoidentificador", "atos_tipoidentificadorid", "atos_codigo", datosGenerales.Cliente.TipoIdentificador.ToString());
            facturaCRM.atos_numerodedocumento = datosGenerales.Cliente.Identificador;

            return errores;
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Direccion Suministro' para otras facturas NO tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de cliente de la factura</param>
        /// <param name="datosGenerales">Clase tipada construida a través del fichero xsd que almacena los 'datos generales' de la factura para otras factura NO tipo ATR</param>
        /// <returns>Coleccion de cadenas de errores</returns>
        private List<string> cargarClienteOtras(atos_facturadistribuidora facturaCRM, FacturasOtrasFacturasDatosGeneralesOtrasFacturas datosGenerales, XmlDocument xDoc)
        {
            List<string> errores = new List<string>();

            facturaCRM.atos_razonsocialid = createEntityReference("account", "accountid", "atos_numerodedocumento", datosGenerales.Cliente.Identificador);
            facturaCRM.atos_codigocontratoatr = datosGenerales.CodContrato.ToString();
            facturaCRM.atos_numerodedocumento = datosGenerales.Cliente.Identificador;
            if (xDoc.GetElementsByTagName("TipoPersona").Count > 0)
                facturaCRM.atos_tipopersonaid = createEntityReference("atos_tipopersona", "atos_tipopersonaid", "atos_codigo", datosGenerales.Cliente.TipoPersona.ToString());
            facturaCRM.atos_tipoidentificadorid = createEntityReference("atos_tipoidentificador", "atos_tipoidentificadorid", "atos_codigo", datosGenerales.Cliente.TipoIdentificador.ToString());
            facturaCRM.atos_fechaboe = convertToNullDateTime(datosGenerales.FechaBOE);

            return errores;
        }

        /// <summary>
        /// Método privado que carga los datos de la sección 'Datos Generales' para facturas tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los 'datos generales' de la factura</param>
        /// <param name="datosGenerales">Clase tipada construida a través del fichero xsd que almacena los 'datos generales' de la factura para facturas tipo ATR</param>
        /// <returns>Coleccion de cadenas de errores</returns>
        private List<string> cargarDatosGeneralesFacturaATR(atos_facturadistribuidora facturaCRM, TipoDatosGeneralesFactura datosGenerales, XmlDocument xDoc)
        {
            List<string> errores = new List<string>();

            facturaCRM.atos_codigofiscalfactura = datosGenerales.CodigoFiscalFactura;
            facturaCRM.atos_tipofacturadistribuidoraid = createEntityReference("atos_tipofacturadistribuidora", "atos_tipofacturadistribuidoraid", "atos_codigo", parseEnum(datosGenerales.TipoFactura));
            facturaCRM.atos_motivofacturacionid = createEntityReference("atos_motivofacturaciondistribuidora", "atos_motivofacturaciondistribuidoraid", "atos_codigo", parseEnum(datosGenerales.MotivoFacturacion));
            if (!string.IsNullOrWhiteSpace(datosGenerales.CodigoFacturaRectificadaAnulada))
            {
                facturaCRM.atos_numerofacturarectificada = datosGenerales.CodigoFacturaRectificadaAnulada;
                facturaCRM.atos_facturarectificadaanulada = createEntityReference("atos_facturadistribuidora", "atos_facturadistribuidoraid", "atos_codigofiscalfactura", datosGenerales.CodigoFacturaRectificadaAnulada);
            }
            facturaCRM.atos_numeroexpediente = datosGenerales.Expediente == null ? null : datosGenerales.Expediente.NumeroExpediente;
            facturaCRM.atos_codigosolicitudexpediente = datosGenerales.Expediente == null ? null : datosGenerales.Expediente.CodigoSolicitud;
            facturaCRM.atos_fechafactura = datosGenerales.FechaFactura;
            facturaCRM.atos_cifemisora = datosGenerales.IdentificadorEmisora;
            facturaCRM.atos_observaciones = datosGenerales.Comentarios;
            facturaCRM.atos_importetotalfactura = new Money(datosGenerales.ImporteTotalFactura);
            facturaCRM.atos_saldofactura = convertToNullMoney(datosGenerales.SaldoFactura, xDoc, "SaldoFactura");

            return errores;
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Datos Generales' para otras facturas NO tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los 'datos generales' de la factura</param>
        /// <param name="datosGenerales">Clase tipada construida a través del fichero xsd que almacena los 'datos generales' de la factura para otras facturas NO tipo ATR</param>
        /// <returns>Coleccion de cadenas de errores</returns>
        private List<string> cargarDatosGeneralesFacturaOtras(atos_facturadistribuidora facturaCRM, TipoDatosGeneralesOtraFactura datosGenerales, XmlDocument xDoc)
        {
            List<string> errores = new List<string>();

            facturaCRM.atos_codigofiscalfactura = datosGenerales.CodigoFiscalFactura;
            facturaCRM.atos_tipofacturadistribuidoraid = createEntityReference("atos_tipofacturadistribuidora", "atos_tipofacturadistribuidoraid", "atos_codigo", parseEnum(datosGenerales.TipoFactura));
            facturaCRM.atos_motivofacturacionid = createEntityReference("atos_motivofacturaciondistribuidora", "atos_motivofacturaciondistribuidoraid", "atos_codigo", parseEnum(datosGenerales.MotivoFacturacion));
            if (!string.IsNullOrWhiteSpace(datosGenerales.CodigoFacturaRectificadaAnulada))
            {
                facturaCRM.atos_numerofacturarectificada = datosGenerales.CodigoFacturaRectificadaAnulada;
                facturaCRM.atos_facturarectificadaanulada = createEntityReference("atos_facturadistribuidora", "atos_facturadistribuidoraid", "atos_codigofiscalfactura", datosGenerales.CodigoFacturaRectificadaAnulada);
            }
            facturaCRM.atos_fechafactura = datosGenerales.FechaFactura;
            facturaCRM.atos_cifemisora = datosGenerales.IdentificadorEmisora;
            facturaCRM.atos_observaciones = datosGenerales.Comentarios;
            facturaCRM.atos_importetotalfactura = new Money(datosGenerales.ImporteTotalFactura);
            facturaCRM.atos_saldofactura = convertToNullMoney(datosGenerales.SaldoFactura, xDoc, "SaldoFactura");

            return errores;
        }

        /// <summary>
        /// Método privado que carga los datos de la sección 'Datos Factura ATR' especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="datosFactura">Clase tipada construida a través del fichero xsd que almacena los 'datos factura' de la factura para otras facturas NO tipo ATR</param>
        /// <returns>Coleccion de cadenas de errores</returns>
        private List<string> cargarDatosFacturaATR(atos_facturadistribuidora facturaCRM, FacturasFacturaATRDatosGeneralesFacturaATRDatosFacturaATR datosFactura, XmlDocument xDoc)
        {
            List<string> errores = new List<string>();

            facturaCRM.atos_fechaboe = datosFactura.FechaBOE;
            facturaCRM.atos_tarifaid = createEntityReference("atos_tarifa", "atos_tarifaid", "atos_codigoocsum", parseEnum(datosFactura.TarifaATRFact));
            if (xDoc.GetElementsByTagName("ModoControlPotencia").Count > 0)
                facturaCRM.atos_modocontrolpotenciaid = createEntityReference("atos_modocontrolpotencia", "atos_modocontrolpotenciaid", "atos_codigo", parseEnum(datosFactura.ModoControlPotencia));
            facturaCRM.atos_marcamedidaconperdidasid = createEntityReference("atos_marcamedidaconperdidas", "atos_marcamedidaconperdidasid", "atos_codigo", datosFactura.MarcaMedidaConPerdidas.ToString());
            if (xDoc.GetElementsByTagName("VAsTrafo").Count > 0)
                facturaCRM.atos_vastrafotexto = datosFactura.VAsTrafo.ToString();
            facturaCRM.atos_porcentajeperdidas = convertToNullDecimal(datosFactura.PorcentajePerdidas, xDoc, "PorcentajePerdidas");
            facturaCRM.atos_indicacurvacargaid = createEntityReference("atos_indicativocurvadecarga", "atos_indicativocurvadecargaid", "atos_codigo", parseEnum(datosFactura.IndicativoCurvaCarga));
            facturaCRM.atos_fechadesdecch = datosFactura.PeriodoCCH == null ? null : (DateTime?)Convert.ToDateTime(datosFactura.PeriodoCCH.FechaDesdeCCH);
            facturaCRM.atos_fechahastacch = datosFactura.PeriodoCCH == null ? null : (DateTime?)Convert.ToDateTime(datosFactura.PeriodoCCH.FechaHastaCCH);
            facturaCRM.atos_fechadesdefactura = datosFactura.Periodo == null ? null : (DateTime?)Convert.ToDateTime(datosFactura.Periodo.FechaDesdeFactura);
            facturaCRM.atos_fechahastafactura = datosFactura.Periodo == null ? null : (DateTime?)Convert.ToDateTime(datosFactura.Periodo.FechaHastaFactura);
            facturaCRM.atos_numeromeses = datosFactura.Periodo.NumeroDias;

            return errores;
        }

        /// <summary>
        /// Método privado que carga los datos de la sección 'Potencia' especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="potencia">Clase tipada construida a través del fichero xsd que almacena los datos de 'Potencia' de la factura para facturas tipo ATR</param>
        private void cargarPotencia(atos_facturadistribuidora facturaCRM, FacturasFacturaATRPotencia potencia, XmlDocument xDoc)
        {
            if (potencia == null || potencia.TerminoPotencia == null || potencia.TerminoPotencia.Length == 0) return;

            for (int idxTP = 0; idxTP < potencia.TerminoPotencia.Length; idxTP++)
            {
                facturaCRM.atos_fechadesdeterminopotencia = potencia.TerminoPotencia[idxTP].FechaDesde;
                facturaCRM.atos_fechahastaterminopotencia = potencia.TerminoPotencia[idxTP].FechaHasta;
                for (int idxP = 0; idxP < potencia.TerminoPotencia[idxTP].Periodo.Length; idxP++)
                {
                    if (idxP == 0)
                        cargarPotenciaP1(facturaCRM, potencia.TerminoPotencia[idxTP].Periodo[idxP]);
                    else if (idxP == 1)
                        cargarPotenciaP2(facturaCRM, potencia.TerminoPotencia[idxTP].Periodo[idxP]);
                    else if (idxP == 2)
                        cargarPotenciaP3(facturaCRM, potencia.TerminoPotencia[idxTP].Periodo[idxP]);
                    else if (idxP == 3)
                        cargarPotenciaP4(facturaCRM, potencia.TerminoPotencia[idxTP].Periodo[idxP]);
                    else if (idxP == 4)
                        cargarPotenciaP5(facturaCRM, potencia.TerminoPotencia[idxTP].Periodo[idxP]);
                    else if (idxP == 5)
                        cargarPotenciaP6(facturaCRM, potencia.TerminoPotencia[idxTP].Periodo[idxP]);
                }
                if (xDoc.GetElementsByTagName("PenalizacionNoICP").Count > 0)
                    facturaCRM.atos_penalizacionnoicp = new OptionSetValue((int)potencia.PenalizacionNoICP + Proceso.IndicativoBase);
                facturaCRM.atos_importetotalteminopotencia = Convert.ToDecimal(potencia.ImporteTotalTerminoPotencia);
            }
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Potencia' para el periodo 1, especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="Periodo">Clase tipada construida a través del fichero xsd que almacena los datos de 'Potencia' de la factura para facturas tipo ATR</param>
        private void cargarPotenciaP1(atos_facturadistribuidora facturaCRM, FacturasFacturaATRPotenciaTerminoPotenciaPeriodo Periodo)
        {
            facturaCRM.atos_potenciacontratadatextop1 = Periodo.PotenciaContratada;
            facturaCRM.atos_potenciamaximademandadatextop1 = Periodo.PotenciaMaxDemandada;
            facturaCRM.atos_potenciaafacturartextop1 = Periodo.PotenciaAFacturar;
            facturaCRM.atos_preciopotenciap1 = Convert.ToDecimal(Periodo.PrecioPotencia);
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Potencia' para el periodo 2, especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="Periodo">Clase tipada construida a través del fichero xsd que almacena los datos de 'Potencia' de la factura para facturas tipo ATR</param>
        private void cargarPotenciaP2(atos_facturadistribuidora facturaCRM, FacturasFacturaATRPotenciaTerminoPotenciaPeriodo Periodo)
        {
            facturaCRM.atos_potenciacontratadatextop2 = Periodo.PotenciaContratada;
            facturaCRM.atos_potenciamaximademandadatextop2 = Periodo.PotenciaMaxDemandada;
            facturaCRM.atos_potenciaafacturartextop2 = Periodo.PotenciaAFacturar;
            facturaCRM.atos_preciopotenciap2 = Convert.ToDecimal(Periodo.PrecioPotencia);
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Potencia' para el periodo 3, especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="Periodo">Clase tipada construida a través del fichero xsd que almacena los datos de 'Potencia' de la factura para facturas tipo ATR</param>
        private void cargarPotenciaP3(atos_facturadistribuidora facturaCRM, FacturasFacturaATRPotenciaTerminoPotenciaPeriodo Periodo)
        {
            facturaCRM.atos_potenciacontratadatextop3 = Periodo.PotenciaContratada;
            facturaCRM.atos_potenciamaximademandadatextop3 = Periodo.PotenciaMaxDemandada;
            facturaCRM.atos_potenciaafacturartextop3 = Periodo.PotenciaAFacturar;
            facturaCRM.atos_preciopotenciap3 = Convert.ToDecimal(Periodo.PrecioPotencia);
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Potencia' para el periodo 4, especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="Periodo">Clase tipada construida a través del fichero xsd que almacena los datos de 'Potencia' de la factura para facturas tipo ATR</param>
        private void cargarPotenciaP4(atos_facturadistribuidora facturaCRM, FacturasFacturaATRPotenciaTerminoPotenciaPeriodo Periodo)
        {
            facturaCRM.atos_potenciacontratadatextop4 = Periodo.PotenciaContratada;
            facturaCRM.atos_potenciamaximademandadatextop4 = Periodo.PotenciaMaxDemandada;
            facturaCRM.atos_potenciaafacturartextop4 = Periodo.PotenciaAFacturar;
            facturaCRM.atos_preciopotenciap4 = Convert.ToDecimal(Periodo.PrecioPotencia);
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Potencia' para el periodo 5, especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="Periodo">Clase tipada construida a través del fichero xsd que almacena los datos de 'Potencia' de la factura para facturas tipo ATR</param>
        private void cargarPotenciaP5(atos_facturadistribuidora facturaCRM, FacturasFacturaATRPotenciaTerminoPotenciaPeriodo Periodo)
        {
            facturaCRM.atos_potenciacontratadatextop5 = Periodo.PotenciaContratada;
            facturaCRM.atos_potenciamaximademandadatextop5 = Periodo.PotenciaMaxDemandada;
            facturaCRM.atos_potenciaafacturartextop5 = Periodo.PotenciaAFacturar;
            facturaCRM.atos_preciopotenciap5 = Convert.ToDecimal(Periodo.PrecioPotencia);
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Potencia' para el periodo 6, especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="Periodo">Clase tipada construida a través del fichero xsd que almacena los datos de 'Potencia' de la factura para facturas tipo ATR</param>
        private void cargarPotenciaP6(atos_facturadistribuidora facturaCRM, FacturasFacturaATRPotenciaTerminoPotenciaPeriodo Periodo)
        {
            facturaCRM.atos_potenciacontratadatextop6 = Periodo.PotenciaContratada;
            facturaCRM.atos_potenciamaximademandadatextop6 = Periodo.PotenciaMaxDemandada;
            facturaCRM.atos_potenciaafacturartextop6 = Periodo.PotenciaAFacturar;
            facturaCRM.atos_preciopotenciap6 = Convert.ToDecimal(Periodo.PrecioPotencia);
        }

        /// <summary>
        /// Método privado que carga los datos de la sección 'Exceso Potencia' especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="excesoPotencia">Clase tipada construida a través del fichero xsd que almacena los datos de 'Excesos de Potencia' de la factura para facturas tipo ATR</param>
        private void cargarExcesoPotencia(atos_facturadistribuidora facturaCRM, FacturasFacturaATRExcesoPotencia excesoPotencia)
        {
            if (excesoPotencia == null || excesoPotencia.Periodo == null || excesoPotencia.Periodo.Length == 0) return;

            for (int idxP = 0; idxP < excesoPotencia.Periodo.Length; idxP++)
            {
                facturaCRM.atos_importetotalexcesos = Convert.ToDecimal(excesoPotencia.ImporteTotalExcesos);

                if (idxP == 0)
                    facturaCRM.atos_valorexcesopotenciap1 = Convert.ToDecimal(Convert.ToDecimal(excesoPotencia.Periodo[idxP].ValorExcesoPotencia));
                else if (idxP == 1)
                    facturaCRM.atos_valorexcesopotenciap2 = Convert.ToDecimal(Convert.ToDecimal(excesoPotencia.Periodo[idxP].ValorExcesoPotencia));
                else if (idxP == 2)
                    facturaCRM.atos_valorexcesopotenciap3 = Convert.ToDecimal(Convert.ToDecimal(excesoPotencia.Periodo[idxP].ValorExcesoPotencia));
                else if (idxP == 3)
                    facturaCRM.atos_valorexcesopotenciap4 = Convert.ToDecimal(Convert.ToDecimal(excesoPotencia.Periodo[idxP].ValorExcesoPotencia));
                else if (idxP == 4)
                    facturaCRM.atos_valorexcesopotenciap5 = Convert.ToDecimal(Convert.ToDecimal(excesoPotencia.Periodo[idxP].ValorExcesoPotencia));
                else if (idxP == 5)
                    facturaCRM.atos_valorexcesopotenciap6 = Convert.ToDecimal(Convert.ToDecimal(excesoPotencia.Periodo[idxP].ValorExcesoPotencia));
            }
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Energia Activa' especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="energiaActiva"></param>
        private void cargarEnergiaActiva(atos_facturadistribuidora facturaCRM, FacturasFacturaATREnergiaActiva energiaActiva)
        {
            if (energiaActiva == null || energiaActiva.TerminoEnergiaActiva == null || energiaActiva.TerminoEnergiaActiva.Length == 0) return;

            facturaCRM.atos_importetotalenergiaactiva = Convert.ToDecimal(energiaActiva.ImporteTotalEnergiaActiva);

            for (int idxTEA = 0; idxTEA < energiaActiva.TerminoEnergiaActiva.Length; idxTEA++)
            {
                facturaCRM.atos_fechadesdeenergiaactiva = energiaActiva.TerminoEnergiaActiva[idxTEA].FechaDesde;
                facturaCRM.atos_fechahastaenergiaactiva = energiaActiva.TerminoEnergiaActiva[idxTEA].FechaHasta;

                for (int idxP = 0; idxP < energiaActiva.TerminoEnergiaActiva[idxTEA].Periodo.Length; idxP++)
                {
                    if (idxP == 0)
                        cargarEnergiaActivaP1(facturaCRM, energiaActiva.TerminoEnergiaActiva[idxTEA].Periodo[idxP]);
                    else if (idxP == 1)
                        cargarEnergiaActivaP2(facturaCRM, energiaActiva.TerminoEnergiaActiva[idxTEA].Periodo[idxP]);
                    else if (idxP == 2)
                        cargarEnergiaActivaP3(facturaCRM, energiaActiva.TerminoEnergiaActiva[idxTEA].Periodo[idxP]);
                    else if (idxP == 3)
                        cargarEnergiaActivaP4(facturaCRM, energiaActiva.TerminoEnergiaActiva[idxTEA].Periodo[idxP]);
                    else if (idxP == 4)
                        cargarEnergiaActivaP5(facturaCRM, energiaActiva.TerminoEnergiaActiva[idxTEA].Periodo[idxP]);
                    else if (idxP == 5)
                        cargarEnergiaActivaP6(facturaCRM, energiaActiva.TerminoEnergiaActiva[idxTEA].Periodo[idxP]);
                }
            }
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Energia Activa' para el periodo 1, especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="Periodo">Clase tipada construida a través del fichero xsd que almacena los datos para un periodo de 'Energia Activa' de la factura para facturas tipo ATR</param>
        private void cargarEnergiaActivaP1(atos_facturadistribuidora facturaCRM, FacturasFacturaATREnergiaActivaTerminoEnergiaActivaPeriodo periodo)
        {
            facturaCRM.atos_valorenergiaactivap1 = periodo.ValorEnergiaActiva;
            facturaCRM.atos_precioenergiap1 = periodo.PrecioEnergia;
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Energia Activa' para el periodo 2, especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="Periodo">Clase tipada construida a través del fichero xsd que almacena los datos para un periodo de 'Energia Activa' de la factura para facturas tipo ATR</param>
        private void cargarEnergiaActivaP2(atos_facturadistribuidora facturaCRM, FacturasFacturaATREnergiaActivaTerminoEnergiaActivaPeriodo periodo)
        {
            facturaCRM.atos_valorenergiaactivap2 = periodo.ValorEnergiaActiva;
            facturaCRM.atos_precioenergiap2 = periodo.PrecioEnergia;
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Energia Activa' para el periodo 3, especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="Periodo">Clase tipada construida a través del fichero xsd que almacena los datos para un periodo de 'Energia Activa' de la factura para facturas tipo ATR</param>
        private void cargarEnergiaActivaP3(atos_facturadistribuidora facturaCRM, FacturasFacturaATREnergiaActivaTerminoEnergiaActivaPeriodo periodo)
        {
            facturaCRM.atos_valorenergiaactivap3 = periodo.ValorEnergiaActiva;
            facturaCRM.atos_precioenergiap3 = periodo.PrecioEnergia;
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Energia Activa' para el periodo 4, especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="Periodo">Clase tipada construida a través del fichero xsd que almacena los datos para un periodo de 'Energia Activa' de la factura para facturas tipo ATR</param>
        private void cargarEnergiaActivaP4(atos_facturadistribuidora facturaCRM, FacturasFacturaATREnergiaActivaTerminoEnergiaActivaPeriodo periodo)
        {
            facturaCRM.atos_valorenergiaactivap4 = periodo.ValorEnergiaActiva;
            facturaCRM.atos_precioenergiap4 = periodo.PrecioEnergia;
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Energia Activa' para el periodo 5, especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="Periodo">Clase tipada construida a través del fichero xsd que almacena los datos para un periodo de 'Energia Activa' de la factura para facturas tipo ATR</param>
        private void cargarEnergiaActivaP5(atos_facturadistribuidora facturaCRM, FacturasFacturaATREnergiaActivaTerminoEnergiaActivaPeriodo periodo)
        {
            facturaCRM.atos_valorenergiaactivap5 = periodo.ValorEnergiaActiva;
            facturaCRM.atos_precioenergiap5 = periodo.PrecioEnergia;
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Energia Activa' para el periodo 6, especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="Periodo">Clase tipada construida a través del fichero xsd que almacena los datos de para un periodo de 'Energia Activa' de la factura para facturas tipo ATR</param>
        private void cargarEnergiaActivaP6(atos_facturadistribuidora facturaCRM, FacturasFacturaATREnergiaActivaTerminoEnergiaActivaPeriodo periodo)
        {
            facturaCRM.atos_valorenergiaactivap6 = periodo.ValorEnergiaActiva;
            facturaCRM.atos_precioenergiap6 = periodo.PrecioEnergia;
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Energia Reactiva' especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="energiaReactiva"></param>
        private void cargarEnergiaReactiva(atos_facturadistribuidora facturaCRM, FacturasFacturaATREnergiaReactiva energiaReactiva)
        {
            if (energiaReactiva == null || energiaReactiva.TerminoEnergiaReactiva == null) return;

            facturaCRM.atos_importetotalenergiareactiva = Convert.ToDecimal(energiaReactiva.ImporteTotalEnergiaReactiva);

            for (int idxTER = 0; idxTER < energiaReactiva.TerminoEnergiaReactiva.Length; idxTER++)
            {
                facturaCRM.atos_fechadesdenergiareactiva = energiaReactiva.TerminoEnergiaReactiva[idxTER].FechaDesde;
                facturaCRM.atos_fechahastaenergiareactiva = energiaReactiva.TerminoEnergiaReactiva[idxTER].FechaHasta;

                for (int idxP = 0; idxP < energiaReactiva.TerminoEnergiaReactiva[idxTER].Periodo.Length; idxP++)
                {
                    if (idxP == 0)
                        cargarEnergiaReactivaP1(facturaCRM, energiaReactiva.TerminoEnergiaReactiva[idxTER].Periodo[idxP]);
                    else if (idxP == 1)
                        cargarEnergiaReactivaP2(facturaCRM, energiaReactiva.TerminoEnergiaReactiva[idxTER].Periodo[idxP]);
                    else if (idxP == 2)
                        cargarEnergiaReactivaP3(facturaCRM, energiaReactiva.TerminoEnergiaReactiva[idxTER].Periodo[idxP]);
                    else if (idxP == 3)
                        cargarEnergiaReactivaP4(facturaCRM, energiaReactiva.TerminoEnergiaReactiva[idxTER].Periodo[idxP]);
                    else if (idxP == 4)
                        cargarEnergiaReactivaP5(facturaCRM, energiaReactiva.TerminoEnergiaReactiva[idxTER].Periodo[idxP]);
                    else if (idxP == 5)
                        cargarEnergiaReactivaP6(facturaCRM, energiaReactiva.TerminoEnergiaReactiva[idxTER].Periodo[idxP]);
                }
            }
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Energia Reactiva' para el periodo 1, especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="Periodo">Clase tipada construida a través del fichero xsd que almacena los datos para un periodo de 'Energia Reactiva' de la factura para facturas tipo ATR</param>
        private void cargarEnergiaReactivaP1(atos_facturadistribuidora facturaCRM, FacturasFacturaATREnergiaReactivaTerminoEnergiaReactivaPeriodo periodo)
        {
            facturaCRM.atos_valorenergiareactivap1 = periodo.ValorEnergiaReactiva;
            facturaCRM.atos_precionenergiareactivap1 = periodo.PrecioEnergiaReactiva;
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Energia Reactiva' para el periodo 2, especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="Periodo">Clase tipada construida a través del fichero xsd que almacena los datos para un periodo de 'Energia Reactiva' de la factura para facturas tipo ATR</param>
        private void cargarEnergiaReactivaP2(atos_facturadistribuidora facturaCRM, FacturasFacturaATREnergiaReactivaTerminoEnergiaReactivaPeriodo periodo)
        {
            facturaCRM.atos_valorenergiareactivap2 = periodo.ValorEnergiaReactiva;
            facturaCRM.atos_precionenergiareactivap2 = periodo.PrecioEnergiaReactiva;
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Energia Reactiva' para el periodo 3, especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="Periodo">Clase tipada construida a través del fichero xsd que almacena los datos para un periodo de 'Energia Reactiva' de la factura para facturas tipo ATR</param>
        private void cargarEnergiaReactivaP3(atos_facturadistribuidora facturaCRM, FacturasFacturaATREnergiaReactivaTerminoEnergiaReactivaPeriodo periodo)
        {
            facturaCRM.atos_valorenergiareactivap3 = periodo.ValorEnergiaReactiva;
            facturaCRM.atos_precionenergiareactivap3 = periodo.PrecioEnergiaReactiva;
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Energia Reactiva' para el periodo 4, especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="Periodo">Clase tipada construida a través del fichero xsd que almacena los datos para un periodo de 'Energia Reactiva' de la factura para facturas tipo ATR</param>
        private void cargarEnergiaReactivaP4(atos_facturadistribuidora facturaCRM, FacturasFacturaATREnergiaReactivaTerminoEnergiaReactivaPeriodo periodo)
        {
            facturaCRM.atos_valorenergiareactivap4 = periodo.ValorEnergiaReactiva;
            facturaCRM.atos_precionenergiareactivap4 = periodo.PrecioEnergiaReactiva;
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Energia Reactiva' para el periodo 5, especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="Periodo">Clase tipada construida a través del fichero xsd que almacena los datos para un periodo de 'Energia Reactiva' de la factura para facturas tipo ATR</param>
        private void cargarEnergiaReactivaP5(atos_facturadistribuidora facturaCRM, FacturasFacturaATREnergiaReactivaTerminoEnergiaReactivaPeriodo periodo)
        {
            facturaCRM.atos_valorenergiareactivap5 = periodo.ValorEnergiaReactiva;
            facturaCRM.atos_precionenergiareactivap5 = periodo.PrecioEnergiaReactiva;
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Energia Reactiva' para el periodo 6, especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="Periodo">Clase tipada construida a través del fichero xsd que almacena los datos para un periodo de 'Energia Reactiva' de la factura para facturas tipo ATR</param>
        private void cargarEnergiaReactivaP6(atos_facturadistribuidora facturaCRM, FacturasFacturaATREnergiaReactivaTerminoEnergiaReactivaPeriodo periodo)
        {
            facturaCRM.atos_valorenergiareactivap6 = periodo.ValorEnergiaReactiva;
            facturaCRM.atos_precionenergiareactivap6 = periodo.PrecioEnergiaReactiva;
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Conceptos Factura' para facturas tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="facturaXSD">Clase tipada construida a través del fichero xsd que almacena los datos de 'Concepto Factura' para facturas tipo ATR</param>
        private void cargarConceptosFacturaATR(atos_facturadistribuidora facturaCRM, FacturasFacturaATR facturaXSD, XmlDocument xDoc)
        {
            facturaCRM.atos_baseimponibleimpuestoelectrico = facturaXSD.ImpuestoElectrico.BaseImponible;
            facturaCRM.atos_porcentajeimpuestoeletrico = facturaXSD.ImpuestoElectrico.Porcentaje;
            facturaCRM.atos_importeimpuestoelectrico = facturaXSD.ImpuestoElectrico.Importe;

            facturaCRM.atos_baseimponibleiva1 = facturaXSD.IVA[0].BaseImponible;
            facturaCRM.atos_porcentajeiva1 = facturaXSD.IVA[0].Porcentaje;
            facturaCRM.atos_importeiva1 = facturaXSD.IVA[0].Importe;

            if (facturaXSD.IVA.Length > 1)
            {
                facturaCRM.atos_baseimponibleiva2 = facturaXSD.IVA[1].BaseImponible;
                facturaCRM.atos_porcentajeiva2 = facturaXSD.IVA[1].Porcentaje;
                facturaCRM.atos_importeiva2 = facturaXSD.IVA[1].Importe;
            }
            if (facturaXSD.IVAReducido != null && facturaXSD.IVAReducido.Length > 0)
            {
                facturaCRM.atos_baseimponibleivareducido1 = facturaXSD.IVAReducido[0].BaseImponible;
                facturaCRM.atos_porcentajeivareducido1 = facturaXSD.IVAReducido[0].Porcentaje;
                facturaCRM.atos_importeivareducido1 = facturaXSD.IVAReducido[0].Importe;
            }
            if (facturaXSD.IVAReducido != null && facturaXSD.IVAReducido.Length > 1)
            {
                facturaCRM.atos_baseimponibleivareducido2 = facturaXSD.IVAReducido[1].BaseImponible;
                facturaCRM.atos_porcentajeivareducido2 = facturaXSD.IVAReducido[1].Porcentaje;
                facturaCRM.atos_importeivareducido2 = facturaXSD.IVAReducido[1].Importe;
            }
            cargarAlquileres(facturaCRM, facturaXSD, xDoc);
            facturaCRM.atos_importeinteresesalquileres = convertToNullDecimal(facturaXSD.ImporteIntereses, xDoc, "ImporteIntereses");
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Conceptos Factura' para otras facturas NO tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="facturaXSD">Clase tipada construida a través del fichero xsd que almacena los datos de 'Concepto Factura' para otras facturas NO tipo ATR</param>
        private void cargarConceptosFacturaOtras(atos_facturadistribuidora facturaCRM, FacturasOtrasFacturas facturaXSD)
        {
            facturaCRM.atos_baseimponibleiva1 = facturaXSD.IVA.BaseImponible;
            facturaCRM.atos_porcentajeiva1 = facturaXSD.IVA.Porcentaje;
            facturaCRM.atos_importeiva1 = facturaXSD.IVA.Importe;
            if (facturaXSD.IVAReducido != null)
            {
                facturaCRM.atos_baseimponibleivareducido1 = facturaXSD.IVAReducido.BaseImponible;
                facturaCRM.atos_porcentajeivareducido1 = facturaXSD.IVAReducido.Porcentaje;
                facturaCRM.atos_importeivareducido1 = facturaXSD.IVAReducido.Importe;
            }
        }

        /// <summary>
        /// Método privado que carga los datos de la sección 'Alquileres' especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="facturaXSD">Clase tipada construida a través del fichero xsd que almacena los datos de 'Alquileres' para facturas tipo ATR</param>
        private void cargarAlquileres(atos_facturadistribuidora facturaCRM, FacturasFacturaATR facturaXSD, XmlDocument xDoc)
        {
            if (facturaXSD.Alquileres == null || facturaXSD.Alquileres.PrecioDiarioAlquiler == null || facturaXSD.Alquileres.PrecioDiarioAlquiler.Length == 0) return;

            facturaCRM.atos_importefacturacionalquileres = convertToNullDecimal(facturaXSD.Alquileres.ImporteFacturacionAlquileres, xDoc, "ImporteFacturacionAlquileres");

            for (int idxA = 0; idxA < facturaXSD.Alquileres.PrecioDiarioAlquiler.Length; idxA++)
            {
                if (idxA == 0)
                    cargarAlquiler1(facturaCRM, facturaXSD.Alquileres.PrecioDiarioAlquiler[idxA]);
                if (idxA == 1)
                    cargarAlquiler2(facturaCRM, facturaXSD.Alquileres.PrecioDiarioAlquiler[idxA]);
                if (idxA == 2)
                    cargarAlquiler3(facturaCRM, facturaXSD.Alquileres.PrecioDiarioAlquiler[idxA]);
                if (idxA == 3)
                    cargarAlquiler4(facturaCRM, facturaXSD.Alquileres.PrecioDiarioAlquiler[idxA]);
                if (idxA == 4)
                    cargarAlquiler5(facturaCRM, facturaXSD.Alquileres.PrecioDiarioAlquiler[idxA]);
                if (idxA == 5)
                    cargarAlquiler6(facturaCRM, facturaXSD.Alquileres.PrecioDiarioAlquiler[idxA]);
            }
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Alquileres' de un alquiler, especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="facturaXSD">Clase tipada construida a través del fichero xsd que almacena los datos de 'Alquileres' para facturas tipo ATR</param>
        private void cargarAlquiler1(atos_facturadistribuidora facturaCRM, PrecioDiarioAlquiler alquiler)
        {
            facturaCRM.atos_preciodiaalquiler1 = alquiler.PrecioDia;
            facturaCRM.atos_numerodiasalquiler1 = Convert.ToDecimal(alquiler.NumeroDias);
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Alquileres' de un alquiler, especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="facturaXSD">Clase tipada construida a través del fichero xsd que almacena los datos de 'Alquileres' para facturas tipo ATR</param>
        private void cargarAlquiler2(atos_facturadistribuidora facturaCRM, PrecioDiarioAlquiler alquiler)
        {
            facturaCRM.atos_preciodiaalquiler2 = alquiler.PrecioDia;
            facturaCRM.atos_numerodiasalquiler2 = Convert.ToDecimal(alquiler.NumeroDias);
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Alquileres' de 1 alquiler, especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="facturaXSD">Clase tipada construida a través del fichero xsd que almacena los datos de 'Alquileres' para facturas tipo ATR</param>
        private void cargarAlquiler3(atos_facturadistribuidora facturaCRM, PrecioDiarioAlquiler alquiler)
        {
            facturaCRM.atos_preciodiaalquiler3 = alquiler.PrecioDia;
            facturaCRM.atos_numerodiasalquiler3 = Convert.ToDecimal(alquiler.NumeroDias);
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Alquileres' de 1 alquiler, especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="facturaXSD">Clase tipada construida a través del fichero xsd que almacena los datos de 'Alquileres' para facturas tipo ATR</param>
        private void cargarAlquiler4(atos_facturadistribuidora facturaCRM, PrecioDiarioAlquiler alquiler)
        {
            facturaCRM.atos_preciodiaalquiler4 = alquiler.PrecioDia;
            facturaCRM.atos_numerodiasalquiler4 = Convert.ToDecimal(alquiler.NumeroDias);
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Alquileres' de 1 alquiler, especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="facturaXSD">Clase tipada construida a través del fichero xsd que almacena los datos de 'Alquileres' para facturas tipo ATR</param>
        private void cargarAlquiler5(atos_facturadistribuidora facturaCRM, PrecioDiarioAlquiler alquiler)
        {
            facturaCRM.atos_preciodiaalquiler5 = alquiler.PrecioDia;
            facturaCRM.atos_numerodiasalquiler5 = Convert.ToDecimal(alquiler.NumeroDias);
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Alquileres' de 1 alquiler, especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="facturaXSD">Clase tipada construida a través del fichero xsd que almacena los datos de 'Alquileres' para facturas tipo ATR</param>
        private void cargarAlquiler6(atos_facturadistribuidora facturaCRM, PrecioDiarioAlquiler alquiler)
        {
            facturaCRM.atos_preciodiaalquiler6 = alquiler.PrecioDia;
            facturaCRM.atos_numerodiasalquiler6 = Convert.ToDecimal(alquiler.NumeroDias);
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Conceptos Repercutibles' para facturas tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="facturaXSD">Clase tipada construida a través del fichero xsd que almacena los datos de 'Conceptros Repercutbiles' para facturas tipo ATR</param>
        private void cargarConceptosRepercutiblesATR(atos_facturadistribuidora facturaCRM, FacturasFacturaATR facturaXSD, XmlDocument xDoc)
        {
            if (facturaXSD.ConceptoRepercutible == null) return;

            List<atos_conceptorepercutible> conceptosCRM = new List<atos_conceptorepercutible>();
            foreach (var conceptoXSD in facturaXSD.ConceptoRepercutible)
            {
                atos_conceptorepercutible conceptoCRM = new atos_conceptorepercutible();
                conceptoCRM.atos_tipoconceptofacturadistid = createEntityReference("atos_tipoconceptofacturadistribuidora", "atos_tipoconceptofacturadistribuidoraid", "atos_codigo", parseEnum(conceptoXSD.ConceptoRepercutible));
                conceptoCRM.atos_tipoimpositivoconceptorepercutible = new OptionSetValue((int)conceptoXSD.TipoImpositivoConceptoRepercutible + Proceso.IndicativoBase);
                if (xDoc.GetElementsByTagName("FechaOperacion").Count == 1)
                    conceptoCRM.atos_fechaoperacion = conceptoXSD.Items[0];
                else
                {
                    conceptoCRM.atos_fechadesde = conceptoXSD.Items[0];
                    conceptoCRM.atos_fechahasta = conceptoXSD.Items[1];
                }
                conceptoCRM.atos_unidadesconceptorepercutible = conceptoXSD.UnidadesConceptoRepercutible;
                conceptoCRM.atos_preciounidadeconceptorepercutible = conceptoXSD.PrecioUnidadConceptoRepercutible;
                conceptoCRM.atos_importetotalconceptorepercutible = conceptoXSD.ImporteTotalConceptoRepercutible;
                conceptoCRM.atos_comentarios = conceptoXSD.Comentarios;

                conceptosCRM.Add(conceptoCRM);
            }
            facturaCRM.atos_atos_facturadistribuidora_atos_conceptorepercutible_facturadistribuidoraid = conceptosCRM; // 1:N binding            
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Conceptos Repercutibles' para otras facturas NO tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="facturaXSD">Clase tipada construida a través del fichero xsd que almacena los datos de 'Conceptos Repercutibles' para otras facturas NO tipo ATR</param>
        private void cargarConceptosRepercutiblesOtras(atos_facturadistribuidora facturaCRM, FacturasOtrasFacturas facturaXSD, XmlDocument xDoc)
        {
            List<atos_conceptorepercutible> conceptosCRM = new List<atos_conceptorepercutible>();

            int i = 0;
            foreach (var conceptoXSD in facturaXSD.ConceptoRepercutible)
            {
                atos_conceptorepercutible conceptoCRM = new atos_conceptorepercutible();
                conceptoCRM.atos_tipoconceptofacturadistid = createEntityReference("atos_tipoconceptofacturadistribuidora", "atos_tipoconceptofacturadistribuidoraid", "atos_codigo", parseEnum(conceptoXSD.ConceptoRepercutible));
                conceptoCRM.atos_tipoimpositivoconceptorepercutible = new OptionSetValue((int)conceptoXSD.TipoImpositivoConceptoRepercutible + Proceso.IndicativoBase);
                XmlNodeList conceptosXml = xDoc.SelectNodes("//*[local-name()='OtrasFacturas']/*[local-name()='ConceptoRepercutible']");
                if (((XmlElement)(conceptosXml.Item(i))).GetElementsByTagName("FechaOperacion").Count == 1)
                    conceptoCRM.atos_fechaoperacion = conceptoXSD.Items[0];
                else
                {
                    conceptoCRM.atos_fechadesde = conceptoXSD.Items[0];
                    conceptoCRM.atos_fechahasta = conceptoXSD.Items[1];
                }
                conceptoCRM.atos_unidadesconceptorepercutible = Convert.ToDecimal(conceptoXSD.UnidadesConceptoRepercutible);
                conceptoCRM.atos_preciounidadeconceptorepercutible = Convert.ToDecimal(conceptoXSD.PrecioUnidadConceptoRepercutible);
                conceptoCRM.atos_importetotalconceptorepercutible = Convert.ToDecimal(conceptoXSD.ImporteTotalConceptoRepercutible);
                conceptoCRM.atos_comentarios = conceptoXSD.Comentarios;

                conceptosCRM.Add(conceptoCRM);
                i++;
            }
            facturaCRM.atos_atos_facturadistribuidora_atos_conceptorepercutible_facturadistribuidoraid = conceptosCRM; // 1:N binding            
        }

        private bool esFechaOperacion(ConceptoRepercutibleFact concepto)
        {
            switch (parseEnum(concepto.ConceptoRepercutible))
            {
                case "01":
                case "02":
                case "03":
                case "04":
                case "05":
                case "06":
                case "07":
                case "08":
                case "09":
                case "12":
                case "13":
                case "15":
                case "19":
                case "20":
                case "33": return true;
                default: return false;
            }
        }

        /// <summary>
        /// Método privado que carga los datos de las secciones 'Medidas' e 'Integradores' especificos a una factura tipo ATR
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="facturaXSD">Clase tipada construida a través del fichero xsd que almacena los datos de 'Medidas' e 'Integradores' para facturas tipo ATR</param>
        private void cargarMedidasAparatoIntegrador(atos_facturadistribuidora facturaCRM, FacturasFacturaATR facturaXSD)
        {
            if (facturaXSD.Medidas == null) return;

            List<atos_puntomedidafactura> puntosCRM = new List<atos_puntomedidafactura>();
            foreach (var medidaXSD in facturaXSD.Medidas)
            {
                atos_puntomedidafactura puntoCRM = new atos_puntomedidafactura();
                puntoCRM.atos_codpm = medidaXSD.CodPM;
                puntoCRM.atos_puntomedidaid = createEntityReference("atos_puntomedida", "atos_puntomedidaid", "atos_codigopuntomedida", medidaXSD.CodPM);

                List<atos_medidasdistribuidora> aparatosCRM = new List<atos_medidasdistribuidora>();
                foreach (var aparatoxSD in medidaXSD.ModeloAparato)
                {
                    atos_medidasdistribuidora aparatoCRM = new atos_medidasdistribuidora();
                    aparatoCRM.atos_instalacionid = createEntityReference("atos_instalacion", "atos_instalacionid", new string[] { "atos_cups20", "atos_historico" }, new object[] { medidaXSD.CodPM, false }); ;
                    aparatoCRM.atos_tipoaparatoid = createEntityReference("atos_tipoaparato", "atos_tipoaparatoid", "atos_codigoaparato", parseEnum(aparatoxSD.TipoAparato));
                    aparatoCRM.atos_marcaaparatoid = createEntityReference("atos_marcaaparato", "atos_marcaaparatoid", "atos_codigomarca", aparatoxSD.MarcaAparato);
                    aparatoCRM.atos_numerodeserie = aparatoxSD.NumeroSerie;
                    aparatoCRM.atos_codigodhid = createEntityReference("atos_tipodediscriminacionhoraria", "atos_tipodediscriminacionhorariaid", "atos_codigo", parseEnum(aparatoxSD.TipoDHEdM));

                    List<atos_integradoraparato> integradoresCRM = new List<atos_integradoraparato>();
                    foreach (var integradorXSD in aparatoxSD.Integrador)
                    {
                        atos_integradoraparato integradorCRM = new atos_integradoraparato();
                        integradorCRM.atos_magnitudintegradorid = createEntityReference("atos_magnitudintegrador", "atos_magnitudintegradorid", "atos_codigo", parseEnum(integradorXSD.Magnitud));
                        integradorCRM.atos_codigoperiodoid = createEntityReference("atos_codigoperiododiscriminacionhoraria", "atos_codigoperiododiscriminacionhorariaid", "atos_codigo", parseEnum(integradorXSD.CodigoPeriodo));
                        integradorCRM.atos_constantemultiplicadora = Convert.ToDecimal(integradorXSD.ConstanteMultiplicadora);
                        integradorCRM.atos_numeroruedasenteras = Convert.ToDecimal(integradorXSD.NumeroRuedasEnteras);
                        integradorCRM.atos_numeroruedasdecimales = Convert.ToDecimal(integradorXSD.NumeroRuedasDecimales);
                        integradorCRM.atos_consumocalculado = Convert.ToDecimal(integradorXSD.ConsumoCalculado);
                        integradorCRM.atos_fechahoralecturadesde = Convert.ToDateTime(integradorXSD.LecturaDesde.Fecha);
                        integradorCRM.atos_procedencialecturadesdeid = createEntityReference("atos_procedencia", "atos_procedenciaid", "atos_codigo", parseEnum(integradorXSD.LecturaDesde.Procedencia));
                        integradorCRM.atos_lecturadesdelectura = Convert.ToDecimal(integradorXSD.LecturaDesde.Lectura);
                        integradorCRM.atos_fechahoralecturahasta = Convert.ToDateTime(integradorXSD.LecturaHasta.Fecha);
                        integradorCRM.atos_procedencialecturahastaid = createEntityReference("atos_procedencia", "atos_procedenciaid", "atos_codigo", parseEnum(integradorXSD.LecturaHasta.Procedencia));
                        integradorCRM.atos_lecturahastalectura = Convert.ToDecimal(integradorXSD.LecturaHasta.Lectura);
                        integradorCRM.atos_codigomotivoajusteid = integradorXSD.Ajuste == null ? null : createEntityReference("atos_codigomotivoajuste", "atos_codigomotivoajusteid", "atos_codigo", parseEnum(integradorXSD.Ajuste.CodigoMotivoAjuste));
                        integradorCRM.atos_ajusteporintegrador = integradorXSD.Ajuste == null ? null : (decimal?)Convert.ToDecimal(integradorXSD.Ajuste.AjustePorIntegrador);
                        integradorCRM.atos_comentariosajuste = integradorXSD.Ajuste == null ? null : integradorXSD.Ajuste.Comentarios;
                        integradorCRM.atos_tipoanomaliaid = integradorXSD.Anomalia == null ? null : createEntityReference("atos_codigodeanomalia", "atos_codigodeanomaliaid", "atos_codigo", parseEnum(integradorXSD.Anomalia.TipoAnomalia));
                        integradorCRM.atos_textoanomalia = integradorXSD.Anomalia == null ? null : integradorXSD.Anomalia.Comentarios;
                        integradorCRM.atos_fechahoramaximetro = convertToNullDateTime(integradorXSD.FechaHoraMaximetro);

                        integradoresCRM.Add(integradorCRM);
                    }
                    aparatoCRM.atos_atos_medidasdistribuidora_atos_integradoraparato_medidadistribuidoraid = integradoresCRM;
                    aparatosCRM.Add(aparatoCRM);
                }
                puntoCRM.atos_atos_puntomedidafactura_atos_medidasdistribuidora_puntomedidafacturaid = aparatosCRM;
                puntosCRM.Add(puntoCRM);
            }
            facturaCRM.atos_atos_facturadistribuidora_atos_puntomedidafactura_facturadistribuidoraid = puntosCRM;
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Registro Fin' para una factura
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="registroFin">Clase tipada construida a través del fichero xsd que almacena los datos de 'Registro Fin' para facturas</param>
        private void cargarRegistroFin(atos_facturadistribuidora facturaCRM, FacturasRegistroFin registroFin, XmlDocument xDoc)
        {
            facturaCRM.atos_importetotalfacturaregistrofin = new Money(registroFin.ImporteTotal);
            facturaCRM.atos_saldototalfacturacionregistrofin = convertToNullMoney(registroFin.SaldoTotalFacturacion, xDoc, "SaldoTotalFacturacion");
            facturaCRM.atos_tipomonedaid = createEntityReference("atos_tipomoneda", "atos_tipomonedaid", "atos_codigo", parseEnum(registroFin.TipoMoneda));
            facturaCRM.atos_totalrecibos = Convert.ToDecimal(registroFin.TotalRecibos);
            facturaCRM.atos_fechavalor = Convert.ToDateTime(registroFin.FechaValor);
            facturaCRM.atos_fechalimitepago = Convert.ToDateTime(registroFin.FechaLimitePago);
            facturaCRM.atos_iban = registroFin.IBAN;
            facturaCRM.atos_idremesa = registroFin.IdRemesa;
        }
        /// <summary>
        /// Método privado que carga los datos de la sección 'Otros Datos de la Factura' para una factura
        /// </summary>
        /// <param name="facturaCRM">Clase tipada que representa la factura desde CRM y que almacenerá los datos de la factura</param>
        /// <param name="otrosDatos">Clase tipada construida a través del fichero xsd que almacena los datos de 'Otros Datos de la Factura' para facturas</param>
        private void cargarOtrosDatosFactura(atos_facturadistribuidora facturaCRM, OtrosDatosFactura otrosDatos)
        {
            if (otrosDatos == null) return;

            facturaCRM.atos_sociedadmercantilemisora = otrosDatos.SociedadMercantilEmisora;
            facturaCRM.atos_sociedadmercantildestino = otrosDatos.SociedadMercantilDestino;
            facturaCRM.atos_direccionemisora = otrosDatos.DireccionEmisora;
            facturaCRM.atos_direcciondestino = otrosDatos.DireccionDestino;
        }

        private EntityReference createEntityReference(string nombreEntidad, string nombreCampo, string lookupFiltro, object lookupValor)
        {
            return createEntityReference(nombreEntidad, nombreCampo, new string[] { lookupFiltro }, new object[] { lookupValor });
        }
        private EntityReference createEntityReference(string nombreEntidad, string nombreCampo, string[] lookupFiltros, object[] lookupValores)
        {
            Guid guid = _mapper.ObtenerIdEntidad(nombreEntidad, nombreCampo, lookupFiltros, lookupValores);
            return guid == Guid.Empty ? null : new EntityReference(nombreEntidad, guid);
        }
        private T serializeXmlObject<T>(XmlDocument xDoc)
        {
            T result;
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(xDoc.InnerXml))
            {
                result = (T)serializer.Deserialize(sr);
            }
            return result;
        }
        private decimal? convertToNullDecimal(decimal decimalValue, XmlDocument xDoc = null, string nombreCampo = null)
        {
            //return decimalValue == 0 ? null : (decimal?)decimalValue;
            return ((decimalValue == 0 && xDoc == null) || (nombreCampo != null && xDoc.GetElementsByTagName(nombreCampo).Count == 0)) ? null : (decimal?)decimalValue;
        }
        private decimal? convertToNullDecimal(string decimalValue)
        {
            decimal convert;
            decimal.TryParse(decimalValue, out convert);
            return convertToNullDecimal(convert);
        }
        private DateTime? convertToNullDateTime(DateTime dateValue)
        {
            return dateValue == DateTime.MinValue || dateValue == DateTime.MaxValue ? null : (DateTime?)dateValue;
        }
        private int? convertToNullInt(int intValue)
        {
            return intValue == 0 ? null : (int?)intValue;
        }
        private Money convertToNullMoney(decimal decimalValue, XmlDocument xDoc = null, string nombreCampo = null)
        {
            //return decimalValue == 0 ? null : new Money(decimalValue);
            return ((decimalValue == 0 && xDoc == null) || (nombreCampo != null && xDoc.GetElementsByTagName(nombreCampo).Count == 0)) ? null : new Money(decimalValue);
        }
        private string parseEnum(object enumValue)
        {
            string stringValue = enumValue.ToString();
            for (int idx = 0; idx < stringValue.Length; idx++)
            {
                if (stringValue[idx] >= '0' && stringValue[idx] <= '9' && stringValue.Contains("Item"))
                    return stringValue.Substring(idx);
            }
            return stringValue;
        }
        private int parseEnumToInt(object enumValue)
        {
            string stringValue = parseEnum(enumValue);
            int intValue = 0;
            int.TryParse(stringValue, out intValue);
            return intValue;
        }
    }
}
