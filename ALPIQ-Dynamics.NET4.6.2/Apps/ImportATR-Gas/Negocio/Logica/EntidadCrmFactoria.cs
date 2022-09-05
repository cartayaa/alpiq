using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Negocio.Enums;

namespace Negocio.Logica
{
    public class EntidadCrmFactoria
    {
        public static Entity CrearEntidad(TipoEntidad tipoEntidad)
        {
            Entity entidad = null;
            switch(tipoEntidad)
            {
                case TipoEntidad.Factura: entidad = new Entity("atos_facturaatrgas"); break;
                case TipoEntidad.Solicitud: entidad = new Entity("atos_solicitudatrgas"); break;
                case TipoEntidad.Reclamacion: entidad = new Entity("atos_reclamacionatrgas"); break;
                case TipoEntidad.Consumo: entidad = new Entity("atos_consumoatrgas"); break;
                case TipoEntidad.Otro: entidad = new Entity("atos_otrosatrgas"); break;

                default: entidad = new Entity(); break;
            }
            return entidad;
        }
    }
}
