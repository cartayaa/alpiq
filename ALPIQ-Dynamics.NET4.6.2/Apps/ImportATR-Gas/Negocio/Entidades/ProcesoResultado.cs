using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Negocio.Entidades
{
    public class ProcesoResultado
    {
        public List<string> Errores { get; set; }
        public bool Exito { get { return Errores == null || Errores.Count == 0; } }
        public Guid CampoID { get; set; }
    }
}
