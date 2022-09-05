using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;

namespace Comun.Negocio.Entidades
{
    interface IProceso
    {
        ProcesoResultado Guardar(Entity entidadCRM);
        ProcesoResultado EjecutarPaso(Entity entidadCRM, string paso);
        void ProcesarPaso(Entity entidadCRM, string paso);
        void AdjuntarXml(Guid solicitudId, string nombreEntidad);

        bool ValidarFichero();
        bool ValidarMensaje();
        bool ValidarEsquema();
    }
}
