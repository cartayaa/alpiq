using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Negocio.Enums;
using Microsoft.Xrm.Sdk;
using Negocio.Constantes;
using Negocio.Logica;

namespace Negocio.Entidades
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
                case Paso.A2: procesarPasoA2(entidadCRM); break;
                case Paso.A2S: procesarPasoA2S(entidadCRM); break;
                case Paso.A3: procesarPasoA3(entidadCRM); break;
                case Paso.A3S: procesarPasoA3S(entidadCRM); break;
                case Paso.A4: procesarPasoA4(entidadCRM); break;
                case Paso.A4S: procesarPasoA4S(entidadCRM); break;
            }
        }

        public override int ObtenerNumeroMensajes()
        {
            try
            {
                string nombreElemento = (_paso + _proceso).ToLower();
                return _xDoc.GetElementsByTagName(nombreElemento).Count;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el número de mensajes @ObtenerNumeroMensajes", ex);
            }
        }

        private void procesarPasoA2(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA202(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente, NumeroMensaje);
        }

        private void procesarPasoA2S(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA2S02(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente, NumeroMensaje);
        }

        private void procesarPasoA3(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA302(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente, NumeroMensaje);
        }

        private void procesarPasoA3S(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA3S02(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente, NumeroMensaje);
        }

        private void procesarPasoA4(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA402(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente, NumeroMensaje);
        }

        private void procesarPasoA4S(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA4S02(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente, NumeroMensaje);
        }

    }
}
