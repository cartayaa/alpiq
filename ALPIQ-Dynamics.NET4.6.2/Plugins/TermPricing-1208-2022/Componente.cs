using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TermPricing
{
    public class Componente
    {
        protected String nombre;
        protected String tipo;
        protected Decimal[] valor = new Decimal[7]; // atos_pfijo, atos_p1, ... , atos_p6
        protected Decimal periodos = 0;
        
        //protected ITracingService TracingService;
        //protected bool log = false;
        //protected String ficherolog = "D:\\Tmp\\TermPricing.txt";

        public CommonWS.Log Log = null;

        public String NombreComponente
        {
            get
            {
                return this.nombre;
            }
        }

        public String TipoComponente
        {
            get
            {
                return this.tipo;
            }
        }

        public Decimal NumeroPeriodos
        {
            get
            {
                return this.periodos;
            }
            set
            {
                this.periodos = value;
            }
        }

        public Decimal[] Valores
        {
            get
            {
                return this.valor;
            }
            set
            {
                this.valor = value;
            }
        }

        public Componente(String _nombre, String _tipo)
        {
            nombre = _nombre;
            tipo = _tipo;
        }

        public void setTraza(ITracingService _traza)
        {
            if (Log != null)
                Log.tracingService = _traza;
        }


        public void setLog(Boolean _log, String _uriWSlog, String _subCarpetaLog, String _ficheroLog, ITracingService _TracingService)
        {
            Log = new CommonWS.Log(_log, _uriWSlog, _subCarpetaLog, _ficheroLog, _TracingService);
        }

        public void setLog(CommonWS.Log _log)
        {
            if (Log == null)
                Log = new CommonWS.Log(_log);
            else
                Log.setLog(_log);

        }

    }

    public class TermPricing : Componente
    {
        private Guid termpricing = Guid.Empty;
        private OptionSetValue precioPorcentaje = null;
        private String nombreems;
        private Boolean dependeTarifa;
        private Boolean dependeSistemaElectrico;
        private Boolean dependeSubSistemaElectrico;

        // Pricing Gas
        private Boolean dependePeaje;
        private Boolean calculoDinamico;
        private String entidadCalculo;
        private String campoAcceso;
        private String campoCalculo;
        // Fin Pricing Gas

        private IOrganizationService service;

        private OptionSetValue fijoindexado = null;
        private List<ValorPricingInput> valores = new List<ValorPricingInput>();

        public TermPricing(TermPricing _termpricing)
            : base(_termpricing.NombreComponente, _termpricing.tipo)
        {
            this.termpricing = _termpricing.termpricing;
            this.nombreems = _termpricing.nombreems;
            dependeTarifa = _termpricing.dependeTarifa;
            dependeSistemaElectrico = _termpricing.dependeSistemaElectrico;
            dependeSubSistemaElectrico = _termpricing.dependeSubSistemaElectrico;

            // Pricing Gas
            dependePeaje = _termpricing.dependePeaje;
            calculoDinamico = _termpricing.calculoDinamico;
            entidadCalculo = _termpricing.entidadCalculo;
            campoAcceso = _termpricing.campoAcceso;
            campoCalculo = _termpricing.campoCalculo;
            // Fin Pricing Gas

            if (_termpricing.Log != null)
                Log = new CommonWS.Log(_termpricing.Log);
            else
                Log = new CommonWS.Log();
            //TracingService = _termpricing.TracingService;
            if (_termpricing.precioPorcentaje != null)
                precioPorcentaje = new OptionSetValue(_termpricing.precioPorcentaje.Value);
            fijoindexado = new OptionSetValue(_termpricing.fijoindexado.Value);
        }

        /*private void traza(String message, bool _traza = false)
        {
            Log.writelog(message, _traza);*/
            /*if (log == true)
                System.IO.File.AppendAllText(ficherolog, message + "\r\n");

            if (_traza && TracingService != null)
                this.TracingService.Trace(message);*/
        /*}*/

        public String NombreEms
        {
            get
            {
                return this.nombreems;
            }
        }

        public Guid TermpricingId
        {
            get
            {
                return this.termpricing;
            }
        }

        public OptionSetValue PrecioPorcentaje
        {
            get
            {
                return this.precioPorcentaje;
            }
        }

        public OptionSetValue FijoIndexado
        {
            get
            {
                return this.fijoindexado;
            }
            set
            {
                this.fijoindexado = value;
            }
        }

        public Boolean DependeDeTarifa
        {
            get
            {
                return this.dependeTarifa;
            }
        }

        public Boolean DependeDeSistemaElectrico
        {
            get
            {
                return this.dependeSistemaElectrico;
            }
        }

        public Boolean DependeDeSubSistemaElectrico
        {
            get
            {
                return this.dependeSubSistemaElectrico;
            }
        }

        public Boolean DependeDePeaje
        {
            get
            {
                return this.dependePeaje;
            }
        }

        public Boolean Calculodinamico
        {
            get
            {
                return this.calculoDinamico;
            }
        }

        public int NumeroPricingInputs
        {
            get
            {
                return this.valores.Count;
            }
        }

        public ValorPricingInput pricingInput(int _indice)
        {
            return this.valores[_indice];
        }

        private void inicializa()
        {
            nombreems = "";
            dependeTarifa = false;
            dependeSistemaElectrico = false;
            dependeSubSistemaElectrico = false;

            // Pricing Gas
            dependePeaje = false;
            calculoDinamico = false;
            entidadCalculo = "";
            campoAcceso = "";
            campoCalculo = "";
            Log = new CommonWS.Log();
            // Fin Pricing Gas

            fijoindexado = new OptionSetValue(300000001); // Por defecto lo ponemos indexado.
        }

        public Decimal? valorDinamico(Entity _oferta, Entity _ofpre, IOrganizationService _service)
        {
            Decimal? valor = null;

            Guid id = Guid.Empty;
            if (_oferta.Attributes.Contains(campoAcceso))
                id = ((EntityReference)_oferta.Attributes[campoAcceso]).Id;
            else if (_ofpre.Attributes.Contains(campoAcceso))
                id = ((EntityReference)_ofpre.Attributes[campoAcceso]).Id;

            if ( id != Guid.Empty )
            {
                Entity _EntidadValor = _service.Retrieve(entidadCalculo, id,
                                                         new ColumnSet(new String[] { campoCalculo }));
                if (_EntidadValor.Attributes.Contains(campoCalculo))
                {
                    valor = (Decimal)_EntidadValor.Attributes[campoCalculo];
                }
            }
            return valor;
        }

        public TermPricing(String _nombre)
            : base(_nombre, "TermPricing")
        {
            inicializa();
        }

        public TermPricing(String _nombre, IOrganizationService _service)
            : base(_nombre, "TermPricing")
        {
            inicializa();
            buscaTerminoPricing(_service);
        }


        public TermPricing(String _nombre, IOrganizationService _service, CommonWS.Log _log)
            : base(_nombre, "TermPricing")
        {
            inicializa();
            Log.setLog(_log);
            buscaTerminoPricing(_service);
        }

        public TermPricing(String _nombre, IOrganizationService _service, ITracingService _traza)
            : base(_nombre, "TermPricing")
        {
            inicializa();
            Log.tracingService = _traza;
            //TracingService = _traza;
            buscaTerminoPricing(_service);
        }

        public bool buscaTerminoPricing(IOrganizationService _service)
        {
            service = _service;
            bool _ret = false;
            QueryByAttribute _consulta = new QueryByAttribute("atos_terminodepricing");
            Log.writelog("buscaTerminoPricing 1");
            _consulta.ColumnSet = new ColumnSet(new String[] { "atos_terminodepricingid", "atos_terminoems", "atos_dependenciadetarifa", "atos_dependenciadesistemaelectrico", "atos_dependenciadesubsistemaelectrico", "atos_dependenciadepeaje", "atos_terminopricingdinamicoid", "atos_tipo" });
            _consulta.AddAttributeValue("atos_denominacionbreve", nombre);
            Log.writelog("buscaTerminoPricing " + base.nombre);

            EntityCollection _resConsulta = _service.RetrieveMultiple(_consulta);

            if (_resConsulta.Entities.Count > 0)
            {
                Log.writelog("buscaTerminoPricing encontrado: " + base.nombre);
                termpricing = (Guid)_resConsulta.Entities[0].Attributes["atos_terminodepricingid"];
                if (_resConsulta.Entities[0].Attributes.Contains("atos_terminoems"))
                    nombreems = _resConsulta.Entities[0].Attributes["atos_terminoems"].ToString();
                else
                    nombreems = "";

                if (_resConsulta.Entities[0].Attributes.Contains("atos_dependenciadetarifa"))
                    dependeTarifa = (Boolean)_resConsulta.Entities[0].Attributes["atos_dependenciadetarifa"];

                if (_resConsulta.Entities[0].Attributes.Contains("atos_dependenciadesistemaelectrico"))
                    dependeSistemaElectrico = (Boolean)_resConsulta.Entities[0].Attributes["atos_dependenciadesistemaelectrico"];

                if (_resConsulta.Entities[0].Attributes.Contains("atos_dependenciadesubsistemaelectrico"))
                    dependeSubSistemaElectrico = (Boolean)_resConsulta.Entities[0].Attributes["atos_dependenciadesubsistemaelectrico"];

                if (_resConsulta.Entities[0].Attributes.Contains("atos_dependenciadepeaje"))
                    dependePeaje = (Boolean)_resConsulta.Entities[0].Attributes["atos_dependenciadepeaje"];

                if (_resConsulta.Entities[0].Attributes.Contains("atos_tipo"))
                {
                    fijoindexado = new OptionSetValue(((OptionSetValue)_resConsulta.Entities[0].Attributes["atos_tipo"]).Value);
                    Log.writelog("buscaTerminoPricing " + base.nombre + " Tipo: " + fijoindexado.Value.ToString());
                }
                if (_resConsulta.Entities[0].Attributes.Contains("atos_terminopricingdinamicoid"))
                {
                    Entity _PricingDinamico = _service.Retrieve("atos_terminopricingdinamico",
                                                                        ((EntityReference)_resConsulta.Entities[0].Attributes["atos_terminopricingdinamicoid"]).Id,
                                                                        new ColumnSet(new String[] { "atos_entidadcalculo", "atos_campocalculo", "atos_campoacceso" }));
                    if ( _PricingDinamico.Attributes.Contains("atos_entidadcalculo"))
                    {
                        entidadCalculo = (String) _PricingDinamico.Attributes["atos_entidadcalculo"];
                    }
                    if (_PricingDinamico.Attributes.Contains("atos_campocalculo"))
                    {
                        campoCalculo = (String)_PricingDinamico.Attributes["atos_campocalculo"];
                    }
                    if (_PricingDinamico.Attributes.Contains("atos_campoacceso"))
                    {
                        campoAcceso = (String)_PricingDinamico.Attributes["atos_campoacceso"];
                    }
                    if ( entidadCalculo != "" && campoCalculo != "")
                    {
                        calculoDinamico = true;
                        if (campoAcceso == "")
                            campoAcceso = entidadCalculo + "id";
                    }
                    Log.writelog("buscaTerminoPricing " + base.nombre + " Termino Pricing Dinamico " + ((EntityReference)_resConsulta.Entities[0].Attributes["atos_terminopricingdinamicoid"]).Name);
                }
                _ret = true;
            }

            return _ret;
        }

        public void calcula(Entity _tarifa, Entity _pricinginput)
        {
            Log.writelog("TermPricing: " + base.nombre);
            if (fijoindexado == null)
                throw new Exception("TermPricing: " + base.nombre + " no tiene informado el campo Fijo/Indexado");

            if (fijoindexado.Value != 300000000) // Si el tipo no es Fijo no se calcula precio.
                return;
            if (_pricinginput.Attributes.Contains("atos_porcentajeoimporte") == false)
                throw new Exception("Pricing Input (" + base.nombre + ") no contiene campo Porcentaje o Importe");

            precioPorcentaje = (OptionSetValue)_pricinginput.Attributes["atos_porcentajeoimporte"];
            if (_pricinginput.Attributes.Contains("atos_pfijo"))
            {
                Decimal _precio = (Decimal)_pricinginput.Attributes["atos_pfijo"];
                //if (precioPorcentaje.Value == 300000000)
                //    _precio /= 100;
                Log.writelog("Precio fijo: " + _precio.ToString());
                valor[0] = _precio;
                periodos = 0;
            }
            else
            {
                if (_tarifa.Attributes.Contains("atos_numeroperiodos") == false)
                    throw new Exception("Tarifa " + _tarifa.Attributes["atos_name"].ToString() + " no contiene número de periodos");
                periodos = (Decimal)_tarifa.Attributes["atos_numeroperiodos"];

                for (int i = 1; i <= periodos; i++)
                {
                    if (_pricinginput.Attributes.Contains("atos_p" + i) == false)
                        throw new Exception("Pricing Input (" + base.nombre + ") no contiene valor para el periodo " + i);
                    Decimal _precio = (Decimal)_pricinginput.Attributes["atos_p" + i];
                    //if (precioPorcentaje.Value == 300000000)
                    //    _precio /= 100;
                    Log.writelog("Precio " + i.ToString() + ": " + _precio.ToString());
                    valor[i] = _precio;
                }
            }
        }

        public void calculaCollection(Entity _tarifa, EntityCollection _pricinginputs)
        {
            Log.writelog("TermPricing (Coll): " + base.nombre + " _pricinginputs.Entities.Count: " + _pricinginputs.Entities.Count.ToString());
            if (fijoindexado == null)
                throw new Exception("TermPricing: " + base.nombre + " no tiene informado el campo Fijo/Indexado");

            if (fijoindexado.Value != 300000000) // Si el tipo no es Fijo no se calcula precio.
                return;
            for (int i = 0; i < _pricinginputs.Entities.Count; i++)
            {
                Entity _pricinginput = _pricinginputs.Entities[i];

                if (_pricinginput.Attributes.Contains("atos_porcentajeoimporte") == false)
                    throw new Exception("Pricing Input (" + base.nombre + ") no contiene campo Porcentaje o Importe");

                ValorPricingInput _valorPI = new ValorPricingInput(_pricinginput, _tarifa, base.nombre, service);
                _valorPI.setLog(Log);
                //if (log)
                //    _valorPI.setLog(log, ficherolog);

                valores.Add(_valorPI);
                Log.writelog("Despues de añadir VPI a valores");
            }
        }

        public ValorPricingInput valorPricingInput(DateTime _fini, DateTime _ffin)
        {
            if (Log == null )
            {
                Log = new CommonWS.Log();
            }
            Log.writelog(" - ValorPricingInput (" + nombre + ") " + _fini.ToLocalTime().ToShortDateString() + " - " + _ffin.ToLocalTime().ToShortDateString());
            for (int i = 0; i < valores.Count; i++)
            {
                Log.writelog(" --- ValorPricingInput " + valores[i].FechaInicioAplicacion.ToLocalTime().ToShortDateString() + " - " + valores[i].FechaFinAplicacion.ToLocalTime().ToShortDateString());
                if (_ffin.Date >= valores[i].FechaInicioAplicacion.Date && _fini.Date <= valores[i].FechaFinAplicacion.Date)
                    return valores[i];
            }

            return null;
        }
    }
}