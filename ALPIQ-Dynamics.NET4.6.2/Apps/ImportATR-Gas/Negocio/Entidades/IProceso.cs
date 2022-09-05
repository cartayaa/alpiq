using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;

namespace Negocio.Entidades
{
    interface IProceso
    {
        ProcesoResultado Guardar(Entity entidadCRM);
        ProcesoResultado EjecutarPaso(Entity entidadCRM, string paso);
        void ProcesarPaso(Entity entidadCRM, string paso);
        void AdjuntarXml(Guid solicitudId, string nombreEntidad, string rutaFichero, string nombreFichero);

        bool ValidarFichero();
        bool ValidarMensaje();
        bool ValidarEsquema();
    }
}
