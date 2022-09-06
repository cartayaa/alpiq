using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;

namespace TermPricing
{
    public class ValorPricingInput
    {
        private Decimal[] valor = new Decimal[7];
        private Decimal[] valorMes = new Decimal[7];
        private DateTime fechainicio;
        private DateTime fechafin;
        private OptionSetValue precioPorcentaje = null;
        private Decimal periodos = 0;
        private Decimal meses = 0;
        private IOrganizationService service;
        //private ITracingService TracingService;
        
        //private bool log = false;
        //private String ficherolog = "D:\\Tmp\\TermPricing.txt";
        
        private CommonWS.Log Log;

        private void inicializa(Entity _pricingInput, Entity _tarifa, String _nombre, IOrganizationService _service)
        {
            service = _service;
            // Se debe controlar que el campo atos_fijo del término de pricing sea fijo. Si no no debe entrar aquí
            
            if (_pricingInput.Attributes.Contains("atos_porcentajeoimporte") == false)
                throw new Exception("Pricing Input (" + _nombre + ") no contiene campo Porcentaje o Importe");
            if (_pricingInput.Attributes.Contains("atos_fechainicioaplicacion") == false)
                throw new Exception("Pricing Input (" + _nombre + ") no contiene campo Fecha de inicio de aplicación");
            if (_pricingInput.Attributes.Contains("atos_fechafinaplicacion") == false)
                throw new Exception("Pricing Input (" + _nombre + ") no contiene campo Fecha de fin de aplicación");

            // Error de zona en fechas. No toma la zona local del usuario en el Online.
            // Se calcula en DateTimeSpan la zona del usuario y se suma el offset
            //fechainicio = (DateTime)_pricingInput.Attributes["atos_fechainicioaplicacion"];
            //fechafin = (DateTime)_pricingInput.Attributes["atos_fechafinaplicacion"];
            fechainicio = DateTimeSpan.DateTimeLocal((DateTime)_pricingInput.Attributes["atos_fechainicioaplicacion"], service);
            fechafin = DateTimeSpan.DateTimeLocal((DateTime)_pricingInput.Attributes["atos_fechafinaplicacion"], service);



            //decimal anyos = Decimal.Round(Decimal.Divide(((TimeSpan)(fechafin - fechainicio)).Days + 1, 365), 0);
            //decimal dias = ((TimeSpan)(fechafin - fechainicio)).Days + 1;
            //meses = (decimal)(12 * anyos) + Decimal.Round(Decimal.Divide((dias - (365 * anyos)), 30), 0);

            DateTimeSpan diferencia = DateTimeSpan.CompareDates(fechainicio, fechafin);
            //Log.writelog("Meses DateTimeSpan " + (diferencia.Years * 12 + diferencia.Months + (diferencia.Days > 0 ? 1 : 0)).ToString());
            meses = diferencia.Years * 12 + diferencia.Months + (diferencia.Days > 0 ? 1 : 0);

            //meses = Decimal.Round(Decimal.Divide(((TimeSpan)(fechafin - fechainicio)).Days + 1, 30), 0);
            precioPorcentaje = (OptionSetValue)_pricingInput.Attributes["atos_porcentajeoimporte"];
            if (_pricingInput.Attributes.Contains("atos_pfijo"))
            {
                Decimal _precio = (Decimal)_pricingInput.Attributes["atos_pfijo"];
                if (precioPorcentaje.Value == 300000000)
                    _precio /= (decimal)100;
                Log.writelog("Precio fijo: " + _precio.ToString());
                valor[0] = _precio;
                periodos = 0;
                if (precioPorcentaje.Value == 300000000)
                    valorMes[0] = valor[0];
                else
                    valorMes[0] = valor[0] / meses;
            }
            else
            {
                if (_tarifa.Attributes.Contains("atos_numeroperiodos") == false)
                    throw new Exception("Tarifa " + _tarifa.Attributes["atos_name"].ToString() + " no contiene número de periodos");

                periodos = (Decimal)_tarifa.Attributes["atos_numeroperiodos"];
                Log.writelog("Periodos: " + periodos.ToString());
                for (int i = 1; i <= periodos; i++)
                {

                    if (_pricingInput.Attributes.Contains("atos_p" + i) == false)
                        throw new Exception("Pricing Input (" + _nombre + ") no contiene valor para el periodo " + i);
                    Decimal _precio = (Decimal)_pricingInput.Attributes["atos_p" + i];
                    if (precioPorcentaje.Value == 300000000)
                        _precio /= (decimal)100;
                    Log.writelog("Precio (VPI) " + i.ToString() + ": " + _precio.ToString());
                    valor[i] = _precio;
                    if (precioPorcentaje.Value == 300000000)
                        valorMes[i] = valor[i];
                    else
                        valorMes[i] = valor[i] / meses;
                }
            }

        }

        public ValorPricingInput(Entity _pricingInput, Entity _tarifa, String _nombre, IOrganizationService _service)
        {
            // Se debe controlar que el campo atos_fijo del término de pricing sea fijo. Si no no debe entrar aquí
            //TracingService = _traza;
            Log = new CommonWS.Log();
            this.inicializa(_pricingInput, _tarifa, _nombre, _service);

        }

        public ValorPricingInput(Entity _pricingInput, Entity _tarifa, String _nombre, ITracingService _traza, IOrganizationService _service)
        {
            // Se debe controlar que el campo atos_fijo del término de pricing sea fijo. Si no no debe entrar aquí
            //TracingService = _traza;
            Log = new CommonWS.Log();
            Log.tracingService = _traza;
            this.inicializa(_pricingInput, _tarifa, _nombre, _service);

        }

        public DateTime FechaInicioAplicacion
        {
            get
            {
                return this.fechainicio;
            }
        }

        public DateTime FechaFinAplicacion
        {
            get
            {
                return this.fechafin;
            }
        }

        public Decimal NumeroPeriodos
        {
            get
            {
                return this.periodos;
            }
        }

        public Decimal[] Valores
        {
            get
            {
                return this.valor;
            }
        }

        public Decimal[] ValorMes
        {
            get
            {
                return this.valorMes;
            }
        }

        public bool Porcentaje
        {
            get
            {
                return (precioPorcentaje.Value == 300000000);
            }
        }

        public Decimal Meses
        {
            get
            {
                return meses;
            }
        }

        /*private void traza(String message, bool _traza = false)
        {
            Log.writelog(message, _traza);*/
            /*if (log == true)
                System.IO.File.AppendAllText(ficherolog, message + "\r\n");

            if (_traza && TracingService != null)
                this.TracingService.Trace(message);*/

        /*}*/

        public void setLog(CommonWS.Log _log)
        {
            if (Log == null)
                Log = new CommonWS.Log(_log);
            else
                Log.setLog(_log);
        }

        /*public void setLog(bool _log, String _flog)
        {
            log = _log;
            ficherolog = _flog;
        }*/

    }
}
