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
    public class Proceso04 : ProcesoBase
    {
        public Proceso04(string nombreFichero, List<String> log, XmlDocument xDoc, Operaciones operaciones, TipoEntidad tipoEntidad)
            : base(nombreFichero, log, xDoc, operaciones, tipoEntidad)
        {
        }

        public override void ProcesarPaso(Entity entidadCRM, string paso)
        {
            switch (paso)
            {
                case Paso.A2: procesarPasoA2(entidadCRM); break;
                case Paso.A25: procesarPasoA25(entidadCRM); break;
                case Paso.A3: procesarPasoA3(entidadCRM); break;
                case Paso.A4: procesarPasoA4(entidadCRM); break;
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

        private void procesarPasoA1(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA104(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente, NumeroMensaje);
        }

        private void procesarPasoA2(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA204(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente, NumeroMensaje);
        }

        private void procesarPasoA25(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA2504(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente, NumeroMensaje);
        }

        private void procesarPasoA3(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA304(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente, NumeroMensaje);
        }

        private void procesarPasoA4(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA404(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente, NumeroMensaje);
        }
    }
}
