using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xrm.Sdk;
using Negocio.Logica;

namespace Negocio.Entidades
{
    public class ProcesoC1 : ProcesoBase
    {
        public ProcesoC1(string nombreFichero, List<String> log, IOrganizationService servicio, XmlDocument xDoc, GestionSolicitudesATR gestor, Operaciones operaciones)
            : base(nombreFichero, log, servicio, xDoc, gestor, operaciones)
        {
        }

        public ProcesoC1(string proceso, string paso, ParametrosATR parametrosATR, ITracingService tracingService, string nombreXml, Entity datos01, Operaciones operaciones)
            : base(proceso, paso, parametrosATR, tracingService, nombreXml, datos01, operaciones)
        {
        }

        public override void ProcesarPaso02(Entity entidadCRM)
        {
            _operaciones.AceptacionRechazo_Comunicacion(_nombreFichero, _xDoc, entidadCRM, _proceso, _rechazo, _agente);
        }

        public override void ProcesarPaso04(Entity entidadCRM)
        {
            _operaciones.Rechazo_ActualizacionesEnCampo(_nombreFichero, _xDoc, entidadCRM, _proceso, _rechazo, _agente);
        }

        public override void ProcesarPaso05(Entity entidadCRM)
        {
            _operaciones.Activacion_Alta(_nombreFichero, _xDoc, entidadCRM, _proceso, _rechazo, _agente);
        }

        public override void ProcesarPaso06(Entity entidadCRM)
        {
            _operaciones.Activacion_CambioComercializadoraSaliente(_nombreFichero, _xDoc, entidadCRM, _proceso, _rechazo, _agente);
        }

        public override void ProcesarPaso09(Entity entidadCRM)
        {
            _operaciones.AceptacionRechazo_Anulacion(_nombreFichero, _xDoc, entidadCRM, _proceso, _rechazo, _agente);
        }

        public override void ProcesarPaso10(Entity entidadCRM)
        {
            _operaciones.AceptacionRechazo_Anulacion(_nombreFichero, _xDoc, entidadCRM, _proceso, _rechazo, _agente);
        }

        public override void ProcesarPaso11(Entity entidadCRM)
        {
            _operaciones.Aceptacion_ComercializadoraSaliente(_nombreFichero, _xDoc, entidadCRM, _proceso, _rechazo, _agente);
        }

        public override void ProcesarPaso12(Entity entidadCRM)
        {
            _operaciones.Aceptacion_Rechazo(_nombreFichero, _xDoc, entidadCRM, _proceso, _rechazo, _agente);
        }
    }
}
