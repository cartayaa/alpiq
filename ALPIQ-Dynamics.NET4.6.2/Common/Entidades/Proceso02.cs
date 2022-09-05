using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Comun.Negocio.Enums;
using Microsoft.Xrm.Sdk;
using Comun.Negocio.Constantes;
using Comun.Negocio.Logica;

namespace Comun.Negocio.Entidades
{
    public class Proceso02 : ProcesoBase
    {
        public Proceso02(string nombreFichero, List<String> log, XmlDocument xDoc, Operaciones operaciones, TipoEntidad tipoEntidad)
            : base(nombreFichero, log, xDoc, operaciones, tipoEntidad)
        {
        }
        
        public override void ProcesarPaso(Entity entidadCRM, string paso)
        {
            switch (paso)
            {
                case Paso.A1: procesarPasoA1(entidadCRM); break;
                case Paso.A2: procesarPasoA2(entidadCRM); break;
                case Paso.A2S: procesarPasoA2S(entidadCRM); break;
                case Paso.A3: procesarPasoA3(entidadCRM); break;
                case Paso.A3S: procesarPasoA3S(entidadCRM); break;
                case Paso.A4: procesarPasoA4(entidadCRM); break;
                case Paso.A4S: procesarPasoA4S(entidadCRM); break;
            }
        }

        private void procesarPasoA1(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA1(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente);
        }

        private void procesarPasoA2(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA2S(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente);
        }

        private void procesarPasoA2S(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA2S(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente);
        }

        private void procesarPasoA3(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA3(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente);
        }

        private void procesarPasoA3S(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA3S(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente);
        }

        private void procesarPasoA4(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA4(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente);
        }

        private void procesarPasoA4S(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA4S(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente);
        }

    }
}
