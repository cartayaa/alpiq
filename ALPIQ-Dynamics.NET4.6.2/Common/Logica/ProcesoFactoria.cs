using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using System.Xml;
using Comun.Negocio.Constantes;
using Comun.Negocio.Logica;
using Comun.Negocio.Entidades;
using Comun.Negocio.Enums;

namespace Comun.Negocio.Logica
{
    public static class ProcesoFactoria
    {
        private static string _proceso;                 
        private static XmlDocument _xDoc = null;

        /// <summary>
        /// Crear el proceso correspondiente con los datos tratados desde el proceso de carga ImportATR
        /// </summary>
        /// <param name="rutaFichero">Ruta del fichero xml</param>
        /// <param name="nombreFichero">Nombre del fichero xml sin la ruta para los mensajes de errores</param>
        /// <param name="log">Lista de string para capturar los errores que ocurren al gestionar el proceso y el paso</param>
        /// <param name="servicio">Servicio de trazas del framework CRM</param>
        /// <returns></returns>
        public static ProcesoBase CrearProceso(string rutaFichero, string nombreFichero, List<String> log, IOrganizationService servicio)
        {
            CargarFicheroXml(rutaFichero);
            
            ColeccionGestores gestores = new ColeccionGestores();
            gestores.GestorMensaje = new GestionMensajes(servicio);
            //gestores.GestorFactura = new GestionFacturas(servicio);
            gestores.GestorSolicitud = new GestionSolicitudes(servicio);
            //gestores.GestorReclamacion = new GestionReclamaciones(servicio);
            //gestores.GestorConsumo = new GestionConsumos(servicio);
            //gestores.GestorOtro = new  GestionOtros(servicio);

            Operaciones operaciones = new Operaciones(servicio, gestores, _proceso);

            switch (_proceso)
            {
                case Proceso.P02: return new Proceso02(nombreFichero, log, _xDoc, operaciones, TipoEntidad.Solicitud);
            }

            return null;
        }
        /// <summary>
        /// Carga el fichero xml al objecto XmlDocument
        /// </summary>
        /// <param name="rutaFichero">Ruta del fichero xml</param>
        private static void CargarFicheroXml(string rutaFichero)
        {
            _xDoc = new XmlDocument();
            _xDoc.Load(rutaFichero);
            _proceso = _xDoc.GetElementsByTagName(Configuracion.XmlCodigoProceso)[0].InnerText;
        }
    }
}
