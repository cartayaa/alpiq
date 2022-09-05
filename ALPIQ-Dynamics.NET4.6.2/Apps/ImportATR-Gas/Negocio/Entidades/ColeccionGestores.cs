using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Negocio.Logica;

namespace Negocio.Entidades
{
    public class ColeccionGestores
    {
        public GestionShared GestorMensaje { get; set; }
        //public GestionFacturas GestorFactura { get; set; }
        //public GestionReclamaciones GestorReclamacion { get; set; }
        public GestionSolicitudes GestorSolicitud { get; set; }
    //    public GestionConsumos GestorConsumo { get; set; }
    //    public GestionOtros GestorOtro{ get; set; }
    }
}
