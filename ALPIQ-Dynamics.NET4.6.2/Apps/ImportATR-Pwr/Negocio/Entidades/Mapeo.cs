using Negocio.Constantes;
using Microsoft.Xrm.Sdk;
using System.Xml;

namespace Negocio.Entidades
{
    public class Mapeo
    {
        public string CampoXml { get; set; }
        public string CampoCRM { get; set; }
        public string LookupTabla { get; set; }
        private string _lookupClave = null;
        public string LookupClave 
        { 
            get 
            { 
                if(string.IsNullOrWhiteSpace(_lookupClave))
                    return CampoCRM;
                else
                    return _lookupClave;
            }
            set { _lookupClave = value; }
        }
        public string[] LookupFiltros { get; set; }
        public string[] LookupValores { get; set; }
        public decimal? CalculoMultiplicar { get; set; }
        public decimal? CalculoSumar { get; set; }
        public string AtributoSufijo { get; set; }
        public MapperTipo TipoMapeo { get; set; }

        public delegate string DelegateMapearCampo(XmlElement xElemento, Mapeo mapeo, Entity entidadCRM);
        public DelegateMapearCampo MapearCampoCustom { get; set; }
    }
}
