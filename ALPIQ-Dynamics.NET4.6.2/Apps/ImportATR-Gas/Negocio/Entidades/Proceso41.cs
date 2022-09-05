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
    public class Proceso41 : ProcesoBase
    {
        public Proceso41(string nombreFichero, List<String> log, XmlDocument xDoc, Operaciones operaciones, TipoEntidad tipoEntidad)
            : base(nombreFichero, log, xDoc, operaciones, tipoEntidad)
        {
        }
        
        public override void ProcesarPaso(Entity entidadCRM, string paso)
        {
            switch (paso)
            {
                case Paso.A2: procesarPasoA2(entidadCRM); break;
                case Paso.A2S: procesarPasoA2S(entidadCRM); break;
                case Paso.A25: procesarPasoA25(entidadCRM); break;
                case Paso.A3: procesarPasoA3(entidadCRM); break;
                case Paso.A4: procesarPasoA4(entidadCRM); break;
                case Paso.A3S: procesarPasoA3S(entidadCRM); break;
                case Paso.A4S: procesarPasoA4S(entidadCRM); break;
            }
        }

        public override int ObtenerNumeroMensajes()
        {
            try
            {
                string nombreElemento = (_paso + _proceso).ToLower();
                if (_paso.Contains("A2S"))
                {
                    char[] nombreArray = nombreElemento.ToCharArray();
                    nombreArray[2] -= (char)32;
                    nombreElemento = string.Join("", nombreArray);
                }
                return _xDoc.GetElementsByTagName(nombreElemento).Count;
            }
            catch(Exception ex)
            {
                throw new Exception("Error al obtener el número de mensajes @ObtenerNumeroMensajes", ex);
            }
        }

        private void procesarPasoA2(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA241(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente, NumeroMensaje);
        }

        private void procesarPasoA2S(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA2S41(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente, NumeroMensaje);
        }

        private void procesarPasoA25(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA2541(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente, NumeroMensaje);
        }

        private void procesarPasoA3(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA341(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente, NumeroMensaje);
        }

        private void procesarPasoA4(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA441(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente, NumeroMensaje);
        }

        private void procesarPasoA3S(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA3S41(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente, NumeroMensaje);
        }

        private void procesarPasoA4S(Entity entidadCRM)
        {
            _operaciones.ProcesarPasoA4S41(_nombreFichero, _xDoc, entidadCRM, _rechazo, _agente, NumeroMensaje);
        }
    }
}
