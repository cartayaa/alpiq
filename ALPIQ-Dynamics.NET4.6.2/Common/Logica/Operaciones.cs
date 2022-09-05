using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xrm.Sdk;
using Comun.Negocio.Logica;
using Comun.Negocio.Entidades;
using Comun.Negocio.Constantes;

namespace Comun.Negocio.Logica
{
    public class Operaciones
    {
        private ColeccionGestores _gestores = null;
        private IOrganizationService _servicioCrm = null;
        public IOrganizationService ServicioCrm { get { return _servicioCrm; } }
        public string _proceso = string.Empty;

        public Operaciones(IOrganizationService servicio, ColeccionGestores gestores, string proceso)
        {
            _servicioCrm = servicio;
            _proceso = proceso;
            _gestores = gestores;
        }

        internal void ProcesarPasoA1(string _nombreFichero, XmlDocument _xDoc, Entity entidadCRM, bool _rechazo, string _agente)
        {
            throw new NotImplementedException();
        }

        internal void ProcesarPasoA2(string _nombreFichero, XmlDocument _xDoc, Entity entidadCRM, bool _rechazo, string _agente)
        {
            throw new NotImplementedException();
        }

        internal void ProcesarPasoA2S(string _nombreFichero, XmlDocument _xDoc, Entity entidadCRM, bool _rechazo, string _agente)
        {
            throw new NotImplementedException();
        }

        internal void ProcesarPasoA3(string _nombreFichero, XmlDocument _xDoc, Entity entidadCRM, bool _rechazo, string _agente)
        {
            throw new NotImplementedException();
        }

        internal void ProcesarPasoA3S(string _nombreFichero, XmlDocument _xDoc, Entity entidadCRM, bool _rechazo, string _agente)
        {
            throw new NotImplementedException();
        }

        internal void ProcesarPasoA4(string _nombreFichero, XmlDocument _xDoc, Entity entidadCRM, bool _rechazo, string _agente)
        {
            throw new NotImplementedException();
        }

        internal void ProcesarPasoA4S(string _nombreFichero, XmlDocument _xDoc, Entity entidadCRM, bool _rechazo, string _agente)
        {
            throw new NotImplementedException();
        }
    }
}
