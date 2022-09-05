using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using System.Xml;
using Negocio.Constantes;
using Negocio.Entidades;

namespace Negocio.Logica
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
            GestionSolicitudesATR gestorSolicitud = new GestionSolicitudesATR(servicio);
            GestionFacturaEMS gestorFactura = new GestionFacturaEMS(servicio);
            Operaciones operaciones = new Operaciones(gestorSolicitud, gestorFactura);

            switch (_proceso)
            {
                case Proceso.A3: return new ProcesoA3(nombreFichero, log, servicio, _xDoc, gestorSolicitud, operaciones);
                case Proceso.C1: return new ProcesoC1(nombreFichero, log, servicio, _xDoc, gestorSolicitud, operaciones);
                case Proceso.C2: return new ProcesoC2(nombreFichero, log, servicio, _xDoc, gestorSolicitud, operaciones);
                case Proceso.M1: return new ProcesoM1(nombreFichero, log, servicio, _xDoc, gestorSolicitud, operaciones);
                case Proceso.B1: return new ProcesoB1(nombreFichero, log, servicio, _xDoc, gestorSolicitud, operaciones);
                //case Proceso.F1: return new ProcesoF1(nombreFichero, log, servicio, _xDoc, operaciones);
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
            if (_xDoc.GetElementsByTagName("CodigoDelProceso").Count > 0)
                _proceso = _xDoc.GetElementsByTagName("CodigoDelProceso")[0].InnerText;
            else
                _proceso = null;
        }
        /// <summary>
        /// Crear el proceso correspondiente con los datos tratados desde el plugin WFSolicitudesATR
        /// </summary>
        /// <param name="proceso">Proceso actual del fichero xml</param>
        /// <param name="paso">Paso actual del fichero xml</param>
        /// <param name="parametrosATR">Informacion agrupada del fichero xml procesado</param>
        /// <param name="tracingService">Servicio de trazas del framework CRM</param>
        /// <param name="nombreXml">Nombre del fichero xml sin la ruta para los mensajes de errores</param>
        /// <param name="datos01">Entidad CRM</param>
        /// <returns></returns>
        public static ProcesoBase CrearProceso(string proceso, string paso, ParametrosATR parametrosATR, ITracingService tracingService, string nombreXml, Entity datos01)
        {
            Operaciones operaciones = new Operaciones();
            AtosXml atosXml = new AtosXml(parametrosATR, tracingService);
            
            switch (proceso)
            {
                case Proceso.A3: return new ProcesoA3(proceso, paso, parametrosATR, tracingService, nombreXml, datos01, operaciones, atosXml);
                case Proceso.C1: return new ProcesoC1(proceso, paso, parametrosATR, tracingService, nombreXml, datos01, operaciones);
                case Proceso.C2: return new ProcesoC2(proceso, paso, parametrosATR, tracingService, nombreXml, datos01, operaciones);
                case Proceso.M1: return new ProcesoM1(proceso, paso, parametrosATR, tracingService, nombreXml, datos01, operaciones);
                case Proceso.B1: return new ProcesoB1(proceso, paso, parametrosATR, tracingService, nombreXml, datos01, operaciones);
            }

            return null;
        }
    }
}
