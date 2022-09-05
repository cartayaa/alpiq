using System;
using System.Collections.Generic;
using System.Linq;
using Negocio.Constantes;
using Microsoft.Xrm.Sdk;
using System.Xml;
using Negocio.Entidades;
using Microsoft.Xrm.Sdk.Query;
using System.Web.Services.Protocols;

namespace Negocio.Logica
{
    public class Mapper
    {
        private List<Mapeo> _mapeos = new List<Mapeo>();
        private IOrganizationService _servicioCRM = null;

        public Mapper(IOrganizationService servicio)
        {
            _servicioCRM = servicio;
        }

        public string RecorerMapearCampo(XmlElement xElemento, Entity entidadCRM)
        {
            return ParsearXml(xElemento, entidadCRM);
        }

        public string MapearCampo(XmlElement xElemento, Entity entidadCRM)
        {
            Mapeo mapeo = _mapeos.FirstOrDefault(x => x.CampoXml.ToLower() == xElemento.Name.ToLower());
            if (mapeo != null)
                MapearCampo(xElemento, mapeo, entidadCRM);
            return null;
        }
        public string MapearCampo(XmlElement xElemento, Mapeo mapeo, Entity entidadCRM)
        {
            if(mapeo.MapearCampoCustom == null)
                switch (mapeo.TipoMapeo)
                {
                    case MapperTipo.String:     MapearString(xElemento, mapeo, entidadCRM); break;
                    case MapperTipo.Decimal:    MapearDecimal(xElemento, mapeo, entidadCRM); break;
                    case MapperTipo.Fecha:      MapearFecha(xElemento, mapeo, entidadCRM); break;
                    case MapperTipo.Lookup:     MapearLookup(xElemento, mapeo, entidadCRM); break;
                }
            else
                mapeo.MapearCampoCustom(xElemento, mapeo, entidadCRM);

            return null;
        }

        public void AgregarMapeo(Mapeo mapeo)
        {
            if (!_mapeos.Exists(x => x.CampoXml.ToLower() == mapeo.CampoXml.ToLower()))
                _mapeos.Add(mapeo);
        }
        public void AgregarMapeo(List<Mapeo> mapeos)
        {
            foreach (Mapeo mapeo in mapeos)
                AgregarMapeo(mapeo);
        }

        public string MapearString(XmlElement elementoXml, Mapeo mapeo, Entity entidadCRM)
        {
            entidadCRM.Attributes.Add(mapeo.CampoCRM, elementoXml.InnerText);
            return null;
        }
        public string MapearFecha(XmlElement elementoXml, Mapeo mapeo, Entity entidadCRM)
        {
            entidadCRM.Attributes.Add(mapeo.CampoCRM, Convert.ToDateTime(elementoXml.InnerText));
            return null;
        }
        public string MapearDecimal(XmlElement elementoXml, Mapeo mapeo, Entity entidadCRM)
        {
            decimal decimalValue = calculateDecimal(elementoXml, mapeo);
            entidadCRM.Attributes.Add(mapeo.CampoCRM, decimalValue);
            return null;
        }

        private decimal calculateDecimal(XmlElement elementoXml, Mapeo mapeo)
        {
            decimal decimalValue = 0;
            decimal.TryParse(elementoXml.InnerText, out decimalValue);
            if (mapeo.CalculoMultiplicar.HasValue)
                decimalValue *= mapeo.CalculoMultiplicar.Value;
            if (mapeo.CalculoSumar.HasValue)
                decimalValue += mapeo.CalculoSumar.Value;
            return decimalValue;
        }

        public string MapearLookup(XmlElement elementoXml, Mapeo mapeo, Entity entidadCRM)
        {
            if (elementoXml.Name == mapeo.CampoXml)
            {
                List<string> valores = new List<string>();
                valores.Add(elementoXml.InnerText);
                if(mapeo.LookupValores != null)
                    valores.AddRange(mapeo.LookupValores);
                
                Guid guid = ObtenerIdEntidad(mapeo.LookupTabla, mapeo.LookupClave, mapeo.LookupFiltros, valores.ToArray());
                if (guid != Guid.Empty)
                    entidadCRM.Attributes.Add(mapeo.CampoCRM, new EntityReference(mapeo.LookupTabla, guid));
            }
            return null;
        }

        public string ParseXml(XmlDocument xDoc, Entity entidadCRM, List<Mapeo> camposOmitidos = null, List<Mapeo> camposEspecificos = null)
        {
            XmlElement root = xDoc.DocumentElement;
            ParsearXml(root, entidadCRM, camposOmitidos, camposEspecificos);
            //if (root.SelectNodes("*").Count > 0)
            //    foreach (XmlNode xNode in root.SelectNodes("*"))
            //        ParseXml(xNode, entidadCRM, camposOmitidos, camposEspecificos);

            return null;
        }
        public string ParsearXml(XmlNode xParentNode, Entity entidadCRM, List<Mapeo> camposOmitidos = null, List<Mapeo> camposEspecificos = null)
        {
            // traverse
            if (xParentNode.SelectNodes("*").Count > 0)
                foreach (XmlNode xNode in xParentNode.SelectNodes("*"))
                    ParsearXml(xNode, entidadCRM, camposOmitidos, camposEspecificos);

            // parse
            Mapeo mapeo = _mapeos.FirstOrDefault(x => x.CampoXml.ToLower() == xParentNode.Name.ToLower());
            if(mapeo != null)
            {
                MapearCampo(xParentNode as XmlElement, mapeo, entidadCRM);
            }
            return null;
        }

        public Guid ObtenerIdEntidad(string entidadCRM, string campoCRM, string[] lookupFiltros, object[] lookupValores)
        {
            Guid salida = new Guid();
            try
            {

                QueryByAttribute consulta = new QueryByAttribute(entidadCRM);

                consulta.ColumnSet = new ColumnSet(campoCRM);
                consulta.Attributes.AddRange(lookupFiltros);
                consulta.Values.AddRange(lookupValores);

                EntityCollection resConsulta = _servicioCRM.RetrieveMultiple(consulta);

                if (resConsulta.Entities.Count > 0)
                {
                    salida = resConsulta.Entities[0].Id;
                }
            }
            catch (SoapException soapEx)
            {
                throw new Exception(String.Format("Error en la consulta de {0}:{1}", entidadCRM, soapEx.Detail.InnerText));
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error en la consulta de {0}:{1}", entidadCRM, ex.Message));

            }
            return salida;
        }
    }
}
