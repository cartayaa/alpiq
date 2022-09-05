using System.Collections.Generic;
using System.Xml;
using Microsoft.Xrm.Sdk;
using Negocio.EntidadesCrm;

namespace Negocio.Logica
{
    public class Operaciones
    {
        private GestionSolicitudesATR _gestorSolicitud = null;
        private GestionFacturaEMS _gestorFactura = null;

        public Operaciones()
        {
        }

        public Operaciones(GestionSolicitudesATR facadeSolicitud, GestionFacturaEMS facadeFactura)
        {
            _gestorSolicitud = facadeSolicitud;
            _gestorFactura = facadeFactura;
        }

        public void AceptacionRechazo_Baja(string nombreFichero, XmlDocument xDoc, Entity entidadCRM, string proceso, bool rechazoSolicitud, string agente)
        {
            _gestorSolicitud.solicitudCabeceraPaso(nombreFichero, xDoc, entidadCRM, agente);
            _gestorSolicitud.solicitudCuerpoPaso02Baja(nombreFichero, xDoc, entidadCRM, proceso, rechazoSolicitud, agente);
            _gestorSolicitud.asignarSolicitudAsociada(xDoc, entidadCRM);
        }

        public void AceptacionRechazo_Comunicacion(string nombreFichero, XmlDocument xDoc, Entity entidadCRM, string proceso, bool rechazoSolicitud, string agente)
        {
            _gestorSolicitud.solicitudCabeceraPaso(nombreFichero, xDoc, entidadCRM, agente);
            _gestorSolicitud.solicitudCuerpoPaso02(nombreFichero, xDoc, entidadCRM, proceso, rechazoSolicitud, agente);
            _gestorSolicitud.asignarSolicitudAsociada(xDoc, entidadCRM);
        }

        public void Comunicacion_Incidencias(string nombreFichero, XmlDocument xDoc, Entity entidadCRM, string proceso, bool rechazoSolicitud, string agente)
        {
            _gestorSolicitud.solicitudCabeceraPaso(nombreFichero, xDoc, entidadCRM, agente);
            _gestorSolicitud.solicitudCuerpoPaso03(nombreFichero, xDoc, entidadCRM, proceso, true, agente);
            _gestorSolicitud.asignarSolicitudAsociada(xDoc, entidadCRM);
        }

        public void Rechazo_ActualizacionesEnCampo(string nombreFichero, XmlDocument xDoc, Entity entidadCRM, string proceso, bool rechazoSolicitud, string agente)
        {
            _gestorSolicitud.solicitudCabeceraPaso(nombreFichero, xDoc, entidadCRM, agente);
            _gestorSolicitud.solicitudCuerpoPaso02(nombreFichero, xDoc, entidadCRM, proceso, true, agente);
            _gestorSolicitud.asignarSolicitudAsociada(xDoc, entidadCRM);
        }

        public void Activacion_Alta(string nombreFichero, XmlDocument xDoc, Entity entidadCRM, string proceso, bool rechazoSolicitud, string agente)
        {
            _gestorSolicitud.solicitudCabeceraPaso(nombreFichero, xDoc, entidadCRM, agente);
            _gestorSolicitud.solicitudCuerpoPaso05(nombreFichero, xDoc, entidadCRM, agente);
            _gestorSolicitud.gestionarPuntosMedida(nombreFichero, xDoc, entidadCRM);
            _gestorSolicitud.asignarSolicitudAsociada(xDoc, entidadCRM);
        }

        public void Activacion_Baja(string nombreFichero, XmlDocument xDoc, Entity entidadCRM, string proceso, bool rechazoSolicitud, string agente)
        {
            _gestorSolicitud.solicitudCabeceraPaso(nombreFichero, xDoc, entidadCRM, agente);
            _gestorSolicitud.solicitudCuerpoPaso05Baja(nombreFichero, xDoc, entidadCRM, agente);
            _gestorSolicitud.gestionarPuntosMedida(nombreFichero, xDoc, entidadCRM);
            _gestorSolicitud.asignarSolicitudAsociada(xDoc, entidadCRM);
        }

        public void AceptacionRechazo_Anulacion(string nombreFichero, XmlDocument xDoc, Entity entidadCRM, string proceso, bool rechazoSolicitud, string agente)
        {
            _gestorSolicitud.solicitudCabeceraPaso(nombreFichero, xDoc, entidadCRM, agente);
            List<string> camposOmitidos = new List<string>(new string[] { "ModoControlPotencia", "TipoActivacionPrevista", "FechaActivacionPrevista" });
            //"ActuacionCampo", "TarifaATR", "atos_potenciap1",  "atos_potenciap2", "atos_potenciap3", "atos_potenciap4", "atos_potenciap5", "atos_potenciap6" });
            if (rechazoSolicitud)
                _gestorSolicitud.solicitudCuerpoPaso02(nombreFichero, xDoc, entidadCRM, proceso, rechazoSolicitud, agente, camposOmitidos);
            else
                _gestorSolicitud.solicitudProcesarFechas(nombreFichero, xDoc, entidadCRM, "FechaAceptacion");
            _gestorSolicitud.asignarSolicitudAsociada(xDoc, entidadCRM);
        }

        public void Aceptacion_ComercializadoraSaliente(string nombreFichero, XmlDocument xDoc, Entity entidadCRM, string proceso, bool rechazoSolicitud, string agente)
        {
            _gestorSolicitud.solicitudCabeceraPaso(nombreFichero, xDoc, entidadCRM, agente);
            _gestorSolicitud.solicitudProcesarFechas(nombreFichero, xDoc, entidadCRM, "FechaActivacionPrevista");
            _gestorSolicitud.asignarSolicitudAsociada(xDoc, entidadCRM);
        }

        public void Aceptacion_Rechazo(string nombreFichero, XmlDocument xDoc, Entity entidadCRM, string proceso, bool rechazoSolicitud, string agente)
        {
            _gestorSolicitud.solicitudCabeceraPaso(nombreFichero, xDoc, entidadCRM, agente);
            _gestorSolicitud.solicitudProcesarFechas(nombreFichero, xDoc, entidadCRM, "FechaRechazo");
            _gestorSolicitud.asignarSolicitudAsociada(xDoc, entidadCRM);
        }

        public void Activacion_CambioComercializadoraSaliente(string nombreFichero, XmlDocument xDoc, Entity entidadCRM, string proceso, bool rechazoSolicitud, string agente)
        {
            _gestorSolicitud.solicitudProcesarFechas(nombreFichero, xDoc, entidadCRM, "FechaActivacion");
            Activacion_Alta(nombreFichero, xDoc, entidadCRM, proceso, rechazoSolicitud, agente);
        }

        public void Importacion_Facturacion(string nombreFichero, XmlDocument xDoc, Entity entidadCRM, string proceso, bool rechazoSolicitud, string agente)
        {
            entidadCRM = new atos_facturadistribuidora();
            _gestorFactura.CargarFactura(xDoc, entidadCRM as atos_facturadistribuidora);
        }
    }
}
