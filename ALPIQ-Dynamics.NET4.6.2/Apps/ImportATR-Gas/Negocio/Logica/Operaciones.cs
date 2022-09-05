using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xrm.Sdk;
using Negocio.Entidades;
using Negocio.Constantes;

namespace Negocio.Logica
{
    public class Operaciones
    {
        private ColeccionGestores _gestores = null;
        private IOrganizationService _servicioCrm = null;
        public IOrganizationService ServicioCrm { get { return _servicioCrm; } }
        public string _proceso = string.Empty;

        public Operaciones(IOrganizationService servicio, ColeccionGestores gestores, string proceso)
        {
            _servicioCrm = servicio;
            _proceso = proceso;
            _gestores = gestores;
        }

        internal void ProcesarPasoA102(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);

        }

        internal void ProcesarPasoA202(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA202(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA2S02(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA2S02(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA302(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA302(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA3S02(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA3S02(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA402(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA402(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA4S02(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA4S02(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA104(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA104(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA204(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA204(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA2504(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA2504(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA304(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA304(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA404(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA404(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA105(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA105(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA205(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA205(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA2505(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA2505(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA305(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA305(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA405(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA405(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA238(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA238(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA2538(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA2538(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA338(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA338(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA438(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA438(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA241(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA241(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA2S41(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA2S41(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA2541(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA2541(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA341(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA341(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA3S41(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA3S41(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA441(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA441(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }

        internal void ProcesarPasoA4S41(string nombreFichero, XmlDocument xDoc, Entity entidadCrm, bool rechazo, string agente, int numeroMensaje)
        {
            _gestores.GestorMensaje.ProcesarCabecera(nombreFichero, xDoc, entidadCrm, agente);
            _gestores.GestorSolicitud.ProcesarPasoA4S41(nombreFichero, xDoc, entidadCrm, rechazo, agente, numeroMensaje);
        }
    }
}
