using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Comun.Negocio.Utilidades
{
    public class RegistroResultado
    {        
        public string XmlCampo { get; set; }
        public string XmlValor { get; set; }
        public string CrmCampo { get; set; }
        public Guid Guid { get; set; }
        public decimal Decimal { get; set; }        
        public int Entero { get; set; }
        public DateTime Fecha { get; set; }
    }
}
