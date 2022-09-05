using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Comun.Negocio.Enums;

namespace Comun.Negocio.Logica
{
    public class EntidadCrmFactoria
    {
        public static Entity CrearEntidad(TipoEntidad tipoEntidad)
        {
            Entity entidad = null;
            switch(tipoEntidad)
            {
                case TipoEntidad.Factura: entidad = new Entity("atos_facturaatr"); break;
                case TipoEntidad.Solicitud: entidad = new Entity("atos_solicitudatrpt"); break;
                case TipoEntidad.Reclamacion: entidad = new Entity("atos_reclamacionatrpt"); break;
                case TipoEntidad.Consumo: entidad = new Entity("atos_consumoatrpt"); break;
                case TipoEntidad.Otro: entidad = new Entity("atos_otrosatrpt"); break;

                default: entidad = new Entity(); break;
            }
            return entidad;
        }
    }
}
