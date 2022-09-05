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
    public class Proceso05 : ProcesoBase
    {
        public Proceso05(string nombreFichero, List<String> log, XmlDocument xDoc, Operaciones operaciones, TipoEntidad tipoEntidad)
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

        private void procesarPasoA2(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA205(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente, NumeroMensaje);
        }

        private void procesarPasoA25(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA2505(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente, NumeroMensaje);
        }

        private void procesarPasoA3(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA305(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente, NumeroMensaje);
        }

        private void procesarPasoA4(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA405(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente, NumeroMensaje);
        }
    }
}
