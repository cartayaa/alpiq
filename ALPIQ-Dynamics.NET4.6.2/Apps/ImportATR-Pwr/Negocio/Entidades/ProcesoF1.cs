using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using System.Xml;
using Negocio.Logica;

namespace Negocio.Entidades
{
    public class ProcesoF1 : ProcesoBase
    {
        public ProcesoF1(string nombreFichero, List<String> log, IOrganizationService servicio, XmlDocument xDoc, Operaciones operaciones)
            : base(nombreFichero, log, servicio, xDoc, null, operaciones)
        {
        }

        public ProcesoF1(string proceso, string paso, ParametrosATR parametrosATR, ITracingService tracingService, string nombreXml, Entity datos01, Operaciones operaciones)
            : base(proceso, paso, parametrosATR, tracingService, nombreXml, datos01, operaciones)
        {
        }

        public override void ProcesarPaso01(Entity entidadCRM)
        {
            _operaciones.Importacion_Facturacion(_nombreFichero, _xDoc, entidadCRM, _proceso, _rechazo, _agente);
        }
    }
}
