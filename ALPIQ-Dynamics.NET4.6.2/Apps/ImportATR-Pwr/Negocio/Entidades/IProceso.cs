using System;
using Microsoft.Xrm.Sdk;

namespace Negocio.Entidades
{
    interface IProceso
    {
        ProcesoResultado EjecutarPaso(Entity entidadCRM, string paso);
        void AdjuntarXml(Guid solicitudId, string rutaFichero, string nombreFichero);
        bool ValidarFichero();
        bool ValidarMensaje();
    }
}
