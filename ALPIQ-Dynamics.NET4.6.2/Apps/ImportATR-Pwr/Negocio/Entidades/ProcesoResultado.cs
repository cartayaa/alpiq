using System;
using System.Collections.Generic;

namespace Negocio.Entidades
{
    public class ProcesoResultado
    {
        public List<string> Errores { get; set; }
        public bool Exito { get { return Errores == null || Errores.Count == 0; } }
        public Guid SolicitudId { get; set; }
    }
}
