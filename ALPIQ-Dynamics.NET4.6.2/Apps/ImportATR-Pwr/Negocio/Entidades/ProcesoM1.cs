using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using System.Xml;
using Negocio.Logica;

namespace Negocio.Entidades
{
    public class ProcesoM1 : ProcesoBase
    {
        public ProcesoM1(string nombreFichero, List<String> log, IOrganizationService servicio, XmlDocument xDoc, GestionSolicitudesATR gestor, Operaciones operaciones)
            : base(nombreFichero, log, servicio, xDoc, gestor, operaciones)
        {
        }

        public ProcesoM1(string proceso, string paso, ParametrosATR parametrosATR, ITracingService tracingService, string nombreXml, Entity datos01, Operaciones operaciones)
            : base(proceso, paso, parametrosATR, tracingService, nombreXml, datos01, operaciones)
        {
        }

        public override void ProcesarPaso02(Entity entidadCRM)
        {
            _operaciones.AceptacionRechazo_Comunicacion(_nombreFichero, _xDoc, entidadCRM, _proceso, _rechazo, _agente);
        }

        public override void ProcesarPaso03(Entity entidadCRM)
        {
            _operaciones.Comunicacion_Incidencias(_nombreFichero, _xDoc, entidadCRM, _proceso, _rechazo, _agente);
        }

        public override void ProcesarPaso04(Entity entidadCRM)
        {
            _operaciones.Rechazo_ActualizacionesEnCampo(_nombreFichero, _xDoc, entidadCRM, _proceso, _rechazo, _agente);
        }

        public override void ProcesarPaso05(Entity entidadCRM)
        {
            _operaciones.Activacion_Alta(_nombreFichero, _xDoc, entidadCRM, _proceso, _rechazo, _agente);
        }

        public override void ProcesarPaso07(Entity entidadCRM)
        {
            _operaciones.AceptacionRechazo_Anulacion(_nombreFichero, _xDoc, entidadCRM, _proceso, _rechazo, _agente);
        }
    }
}
