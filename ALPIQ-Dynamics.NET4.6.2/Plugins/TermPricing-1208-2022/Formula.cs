using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using System.Data;
using Microsoft.Xrm.Sdk.Query;
using TermPricing;

namespace TermPricing
{

    public abstract class FormulaBase
    {
        protected String nombre;
        protected String expresion;
        protected String expresionfinal;
        protected String tipo;
        protected List<Componente> componentes = new List<Componente>();
        protected decimal periodo = -1;
        protected OptionSetValue fijoindexado = null;

        protected int precisiondecimales = 6;
        protected int precisiondecimalesterminos = 6;

        protected bool tipoCalculoPromedio = false;
        protected Boolean esGas = false; // Pricing Gas

        //protected ITracingService TracingService;
        protected CommonWS.Log Log = null;

        //protected bool log = false;
        //protected String ficherolog = "D:\\Tmp\\TermPricing.txt";
        protected Boolean facturacionEstimada = false;

        // Validación de la fórmula
        protected Boolean validar = false;

        public Boolean ValidarFormula
        {
            get { return this.validar; }
            set { this.validar = value; }
        }

        protected List<String> erroresValidacion = new List<String>();
        public List<String> ErroresValidacion
        {
            get { return this.erroresValidacion;  }
            set { this.erroresValidacion = value; }
        }
        // Fin de validación de la fórmula


        public Boolean FacturacionEstimada
        {
            get { return this.facturacionEstimada; }
            set { facturacionEstimada = value; }
        }

        public int PrecisionDecimales
        {
            get
            {
                return this.precisiondecimales;
            }
            set
            {
                this.precisiondecimales = value;
            }
        }

        public int PrecisionDecimalesTerminos
        {
            get
            {
                return this.precisiondecimalesterminos;
            }
            set
            {
                this.precisiondecimalesterminos = value;
            }
        }

        // Pricing Gas
        public Boolean esInstalacionGas
        {
            get
            {
                return this.esGas;
            }
            set
            {
                this.esGas = value;
            }
        }
        // Fin Pricing Gas

        public String TipoFormula
        {
            get
            {
                return this.tipo;
            }
        }

        public String NombreFormula
        {
            get
            {
                return this.nombre;
            }
        }

        public String ExpresionFormula
        {
            get
            {
                return this.expresion;
            }
        }


        public String ExpresionFinalFormula
        {
            get
            {
                return this.expresionfinal;
            }
        }

        public List<Componente> ComponentesFormula
        {
            get
            {
                return this.componentes;
            }
        }

        public OptionSetValue FijoOIndexado
        {
            get
            {
                return this.fijoindexado;
            }
        }


        public FormulaBase(String _nombre, String _expresion, bool _tipoCalculoPromedio = false)
        {
            nombre = _nombre;
            expresion = " " + _expresion.Replace("("," (").Replace(")"," )") + " ";
            expresionfinal = "";
            tipoCalculoPromedio = _tipoCalculoPromedio;
            //TracingService = null;
            Log = new CommonWS.Log();
            componentes = new List<Componente>();
            periodo = -1;
            fijoindexado = null;
            precisiondecimales = 6;
            precisiondecimalesterminos = 6;
            facturacionEstimada = false;
            fijoindexado = new OptionSetValue(300000000); // Por defecto ponemos Fijo, pero si algún término de pricing es indexado lo cambiamos.
            validar = false;
            erroresValidacion = new List<String>();
        }

        public void setTraza(ITracingService _traza)
        {
            if (Log == null)
                Log = new CommonWS.Log();
            Log.tracingService = _traza;
        }
        

        public void setLog(Boolean _log, String _uriWSlog, String _subCarpetaLog, String _ficheroLog, ITracingService _TracingService)
        {
            if (Log == null)
                Log = new CommonWS.Log(_log, _uriWSlog, _subCarpetaLog, _ficheroLog, _TracingService);
            else
                Log.setLog(_log, _uriWSlog, _subCarpetaLog, _ficheroLog, _TracingService);
        }

        public void setLog(CommonWS.Log _log)
        {
            if (Log == null)
                Log = new CommonWS.Log(_log);
            else
                Log.setLog(_log);
            

            for (int i = 0; i < componentes.Count; i++)
                componentes[i].setLog(Log.EscribirLog, Log.UriWSLog, Log.SubCarpetaLog, Log.FicheroLog, Log.tracingService);
        }


        /*
        public void setLog(bool _log, String _flog)
        {
            log = _log;
            ficherolog = _flog;

            for (int i = 0; i < componentes.Count; i++)
                componentes[i].setLog(_log, _flog);
        }
        */
        /*protected void traza(String message, bool _traza = false)
        {
            Log.writelog(message, _traza);*/

            /*if (log == true)
                System.IO.File.AppendAllText(ficherolog, message + "\r\n");
            if (_traza && TracingService != null)
                this.TracingService.Trace(message);*/

        /*}*/


        public int TipoComponente(String _nombre)
        {
            double _n;
            if (_nombre == "+" ||
                _nombre == "-" ||
                _nombre == "*" ||
                _nombre == "/")
                return -1;
            else if (Double.TryParse(_nombre, out _n) == true)
                return -2;
            else
            {
                //Log.writelog(tipo + ": " + nombre + " TipoComponente " + _nombre);
                //Log.writelog(tipo + ": " + nombre + " Numero Componentes " + componentes.Count);
                for (int i = 0; i < componentes.Count; i++)
                {
                    if (_nombre == componentes[i].NombreComponente)
                        return i;
                }
            }
            return -3;
        }

        static public decimal Evaluate(string _expresion)
        {

            var loDataTable = new DataTable();
            var loDataColumn = new DataColumn("Eval", typeof(decimal), _expresion.Replace(',', '.'));
            loDataTable.Columns.Add(loDataColumn);
            loDataTable.Rows.Add(0);
            return (decimal)(loDataTable.Rows[0]["Eval"]);

            /*
             DataTable dt = new DataTable();
             var v = dt.Compute(_expresion,"");
             * */
        }



        public void calculaTermPricing(int _termPricing, Entity _tarifa, Entity _pricinginput)
        {
            if (((TermPricing)componentes[_termPricing]).FijoIndexado == null)
                Log.writelog("Formula: " + nombre + " calculaTermPricing: " + ((TermPricing)componentes[_termPricing]).NombreComponente + " FijoIndexado a null");
            if (((TermPricing)componentes[_termPricing]).FijoIndexado.Value != 300000000)
                fijoindexado = new OptionSetValue(((TermPricing)componentes[_termPricing]).FijoIndexado.Value);
            ((TermPricing)componentes[_termPricing]).calcula(_tarifa, _pricinginput);
        }


        abstract public String expandeExpresion(ref List<FormulaBase> _variables, IOrganizationService _service);
    }


    public class Formula : FormulaBase
    {

        public Formula(String _nombre, String _expresion, bool _tipoCalculoPromedio= false)
            : base(_nombre, _expresion, _tipoCalculoPromedio)
        {
            base.tipo = "Formula";
        }

        override public String expandeExpresion(ref List<FormulaBase> _variables, IOrganizationService _service)
        {
            expresionfinal = expresion.Replace("(", "( ").Replace(")", " )");
            for (int i = 0; i < _variables.Count; i++)
                base.componentes.Add(new Componente(_variables[i].NombreFormula, "Variable"));

            String[] partes = base.expresion.Replace("(", "").Replace(")", "").Split(' ');
            for (int i = 0; i < partes.Length; i++)
            {
                if (partes[i] != "")
                {
                    int _componente = TipoComponente(partes[i]);
                    if (_componente == -3) // No existe. Tiene que ser TermPricing porque las variables se han añadido antes
                    {
                        Log.writelog("Formula: " + nombre + " expandeExpresion componente: " + partes[i], true);
                        TermPricing _tp = new TermPricing(partes[i], _service, Log);
                        //_tp.setLog(Log);
                        base.componentes.Add(_tp);
                    }
                }
            }
            Log.writelog("Formula: " + nombre + " expandeExpresion expresionfinal: " + expresionfinal);
            for (int i = 0; i < _variables.Count; i++)
            {
                String _valorvi = ((Variable)_variables[i]).expandeExpresion(ref _variables, _service);
                expresionfinal = expresionfinal.Replace("(","( ").Replace(")"," )").Replace(_variables[i].NombreFormula, "( " + _valorvi + " )");

                for (int j = 0; j < _variables[i].ComponentesFormula.Count; j++)
                {
                    if (_variables[i].ComponentesFormula[j].TipoComponente == "TermPricing")
                    {
                        int _componente = TipoComponente(_variables[i].ComponentesFormula[j].NombreComponente);
                        if (_componente == -3)
                        {
                            Log.writelog("Formula: " + nombre + " variable " + _variables[i].NombreFormula + " expandeExpresion componente: " + ((TermPricing)_variables[i].ComponentesFormula[j]).NombreComponente);
                            TermPricing _tp = new TermPricing(((TermPricing)_variables[i].ComponentesFormula[j]).NombreComponente, _service, Log);
                            base.componentes.Add(_tp);
                        }
                    }
                }
            }
            //Console.WriteLine("Formula " + base.nombre + " expresión: " + base.expresion + " expandida: " + base.expresionfinal);
            //base.expresionfinal = _formula;
            Log.writelog("Formula: " + nombre + " expandeExpresion expresionfinal: " + expresionfinal + " - FINAL");
            return expresionfinal;
        }

        private Entity generaPricingInput(int i, Entity _oferta, Entity _ofpre)
        {
            Entity _pricingInput = new Entity("atos_pricinginput");

            base.Log.writelog("Función pricingInput " + componentes[i].NombreComponente);

            if (_oferta.Attributes.Contains("atos_fechainicio"))
                _pricingInput.Attributes["atos_fechainicioaplicacion"] = _oferta.Attributes["atos_fechainicio"];
            else
                _pricingInput.Attributes["atos_fechainicioaplicacion"] = _ofpre.Attributes["atos_fechainicio"];

            if (_oferta.Attributes.Contains("atos_fechafin"))
                _pricingInput.Attributes["atos_fechafinaplicacion"] = _oferta.Attributes["atos_fechafin"];
            else
                _pricingInput.Attributes["atos_fechafinaplicacion"] = _ofpre.Attributes["atos_fechafin"];

            return _pricingInput;
        }

        public Entity pricingInput(int i, IOrganizationService _service,
                            Entity _oferta, Entity _ofpre, bool _ofertaid = false) //, Entity _tipoProducto, Entity _tarifa)
        {

            if ( _ofertaid == false && ((TermPricing)componentes[i]).Calculodinamico )
            {

                Entity _pricingInputDinamico = generaPricingInput(i, _oferta, _ofpre);
                _pricingInputDinamico.Attributes["atos_porcentajeoimporte"] = new OptionSetValue(300000001);
                _pricingInputDinamico.Attributes["atos_pfijo"] = ((TermPricing)componentes[i]).valorDinamico(_oferta, _ofpre, _service);
                return _pricingInputDinamico;
            }

            base.Log.writelog("1 pricingInput");
            QueryExpression _consulta = new QueryExpression("atos_pricinginput");
            /* 23866 +1 */
            _consulta.NoLock = true;
            base.Log.writelog("2 pricingInput");
            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_terminodepricingid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(((TermPricing)componentes[i]).TermpricingId.ToString());
            _filtro.Conditions.Add(_condicion);
            base.Log.writelog("3 pricingInput " + ((TermPricing)componentes[i]).TermpricingId.ToString());

            if (((TermPricing)componentes[i]).DependeDeTarifa == true)
            {
                _condicion = new ConditionExpression();
                _condicion.AttributeName = "atos_tarifaid";
                _condicion.Operator = ConditionOperator.Equal;
                if (_oferta.Attributes.Contains("atos_tarifaid"))
                    _condicion.Values.Add(((EntityReference)_oferta.Attributes["atos_tarifaid"]).Id.ToString());
                else
                    _condicion.Values.Add(((EntityReference)_ofpre.Attributes["atos_tarifaid"]).Id.ToString());
                base.Log.writelog("4 pricingInput");
                _filtro.Conditions.Add(_condicion);
            }


            if (((TermPricing)componentes[i]).DependeDeSistemaElectrico == true)
            {
                _condicion = new ConditionExpression();
                _condicion.AttributeName = "atos_sistemaelectricoid";
                _condicion.Operator = ConditionOperator.Equal;
                if (_oferta.Attributes.Contains("atos_sistemaelectricoid"))
                    _condicion.Values.Add(((EntityReference)_oferta.Attributes["atos_sistemaelectricoid"]).Id.ToString());
                else
                    _condicion.Values.Add(((EntityReference)_ofpre.Attributes["atos_sistemaelectricoid"]).Id.ToString());
                base.Log.writelog("5 pricingInput");
                _filtro.Conditions.Add(_condicion);
            }

            base.Log.writelog("6 pricingInput");
            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_fechainiciovigencia";
            _condicion.Operator = ConditionOperator.LessEqual;
            _condicion.Values.Add(DateTime.Now);
            _filtro.Conditions.Add(_condicion);

            base.Log.writelog("7 pricingInput");
            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_fechafinvigencia";
            _condicion.Operator = ConditionOperator.GreaterEqual;
            _condicion.Values.Add(DateTime.Now);
            _filtro.Conditions.Add(_condicion);

            /*base.Log.writelog("8 pricingInput");
            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_contratoid";
            _condicion.Operator = ConditionOperator.Null;
            _filtro.Conditions.Add(_condicion);*/

            if (_ofertaid == true)
            {
                base.Log.writelog("9 pricingInput " + _ofpre.Id.ToString());
                _condicion = new ConditionExpression();
                _condicion.AttributeName = "atos_ofertaid";
                _condicion.Operator = ConditionOperator.Equal;
                _condicion.Values.Add(_ofpre.Id.ToString());
                _filtro.Conditions.Add(_condicion);
            }
            else
            {
                base.Log.writelog("10 pricingInput");
                _condicion = new ConditionExpression();
                _condicion.AttributeName = "atos_ofertaid";
                _condicion.Operator = ConditionOperator.Null;
                _filtro.Conditions.Add(_condicion);

                base.Log.writelog("11 pricingInput");
                _condicion = new ConditionExpression();
                _condicion.AttributeName = "atos_fechainicioaplicacion";
                _condicion.Operator = ConditionOperator.LessThan;

                if (_oferta.Attributes.Contains("atos_fechafin"))
                    _condicion.Values.Add(_oferta.Attributes["atos_fechafin"]);
                else
                    _condicion.Values.Add(_ofpre.Attributes["atos_fechafin"]); 

                _filtro.Conditions.Add(_condicion);


                base.Log.writelog("12 pricingInput");
                _condicion = new ConditionExpression();
                _condicion.AttributeName = "atos_fechafinaplicacion";
                _condicion.Operator = ConditionOperator.GreaterThan;

                if (_oferta.Attributes.Contains("atos_fechainicio"))
                    _condicion.Values.Add(_oferta.Attributes["atos_fechainicio"]);
                else
                    _condicion.Values.Add(_ofpre.Attributes["atos_fechainicio"]);




                _filtro.Conditions.Add(_condicion);
                base.Log.writelog("13 pricingInput");

            }
            _consulta.Criteria.AddFilter(_filtro);
            base.Log.writelog("14 pricingInput");
            _consulta.ColumnSet.AddColumns("atos_pricinginputid", "atos_porcentajeoimporte", "atos_pfijo", "atos_p1", "atos_p2", "atos_p3",
                                    "atos_p4", "atos_p5", "atos_p6", "atos_tipo", "atos_fechainicioaplicacion", "atos_fechafinaplicacion");

            base.Log.writelog("15 pricingInput");
            EntityCollection _resultado = _service.RetrieveMultiple(_consulta);

            base.Log.writelog("16 pricingInput");
            if (_ofertaid == true)
            {
                if (_resultado.Entities.Count != 1)
                    throw new Exception("ERROR. Encontrados " + _resultado.Entities.Count.ToString() + " pricing input (" + ((TermPricing)componentes[i]).NombreComponente + ")");
                return _resultado.Entities[0];
            }

            base.Log.writelog("17 pricingInput");
            if (_resultado.Entities.Count < 1)
                throw new Exception("Encontrados " + _resultado.Entities.Count.ToString() + " pricing input (" + ((TermPricing)componentes[i]).NombreComponente + ")");
            Entity _pricingInput = generaPricingInput(i, _oferta, _ofpre);

            base.Log.writelog("pricingInput fechas de aplicación ");

            Decimal[] pfijo = new Decimal[_resultado.Entities.Count];
            Decimal[] p1 = new Decimal[_resultado.Entities.Count];
            Decimal[] p2 = new Decimal[_resultado.Entities.Count];
            Decimal[] p3 = new Decimal[_resultado.Entities.Count];
            Decimal[] p4 = new Decimal[_resultado.Entities.Count];
            Decimal[] p5 = new Decimal[_resultado.Entities.Count];
            Decimal[] p6 = new Decimal[_resultado.Entities.Count];
            Decimal[] duracion = new Decimal[_resultado.Entities.Count];
            Decimal duraciontotal = 0;

            Boolean haypfijo = false;
            Boolean hayp1 = false;
            Boolean hayp2 = false;
            Boolean hayp3 = false;
            Boolean hayp4 = false;
            Boolean hayp5 = false;
            Boolean hayp6 = false;
            base.Log.writelog("Encontrados " + _resultado.Entities.Count + " pricingintputs");

            for (int j = 0; j < _resultado.Entities.Count; j++)
            {
                base.Log.writelog("Tratando pricinginput " + j);
                DateTime _finicio = (DateTime)_resultado.Entities[j].Attributes["atos_fechainicioaplicacion"];
                DateTime _ffin = (DateTime)_resultado.Entities[j].Attributes["atos_fechafinaplicacion"];
                //duracion[j] = Decimal.Round(Decimal.Divide(((TimeSpan)(_ffin - _finicio)).Days + 1, 30) ,0);

                DateTimeSpan diferencia = DateTimeSpan.CompareDates(_finicio, _ffin);
                //Log.writelog("Meses DateTimeSpan " + (diferencia.Years * 12 + diferencia.Months + (diferencia.Days > 0 ? 1 : 0)).ToString());
                duracion[j] = diferencia.Years * 12 + diferencia.Months + (diferencia.Days > 0 ? 1 : 0);


                // ((Decimal) (((TimeSpan)(_ffin - _finicio)).Days + 1) / 30
                duraciontotal += duracion[j];

                base.Log.writelog("Fecha de inicio " + _finicio.ToString() + " - Fecha de fin " + _ffin.ToString());
                base.Log.writelog("Duracion " + duracion[j].ToString() + " total: " + duraciontotal.ToString());

                if (_resultado.Entities[j].Attributes.Contains("atos_pfijo"))
                {
                    pfijo[j] = (Decimal)_resultado.Entities[j].Attributes["atos_pfijo"];
                    base.Log.writelog("Precio fijo: " + pfijo[j].ToString());
                    haypfijo = true;
                }
                if (_resultado.Entities[j].Attributes.Contains("atos_p1"))
                {
                    p1[j] = (Decimal)_resultado.Entities[j].Attributes["atos_p1"];
                    base.Log.writelog("Precio p1: " + p1[j].ToString());
                    hayp1 = true;
                }
                if (_resultado.Entities[j].Attributes.Contains("atos_p2"))
                {
                    p2[j] = (Decimal)_resultado.Entities[j].Attributes["atos_p2"];
                    base.Log.writelog("Precio p2: " + p2[j].ToString());
                    hayp2 = true;
                }
                if (_resultado.Entities[j].Attributes.Contains("atos_p3"))
                {
                    p3[j] = (Decimal)_resultado.Entities[j].Attributes["atos_p3"];
                    base.Log.writelog("Precio p3: " + p3[j].ToString());
                    hayp3 = true;
                }
                if (_resultado.Entities[j].Attributes.Contains("atos_p4"))
                {
                    p4[j] = (Decimal)_resultado.Entities[j].Attributes["atos_p4"];
                    base.Log.writelog("Precio p4: " + p4[j].ToString());
                    hayp4 = true;
                }
                if (_resultado.Entities[j].Attributes.Contains("atos_p5"))
                {
                    p5[j] = (Decimal)_resultado.Entities[j].Attributes["atos_p5"];
                    base.Log.writelog("Precio p5: " + p5[j].ToString());
                    hayp5 = true;
                }
                if (_resultado.Entities[j].Attributes.Contains("atos_p6"))
                {
                    p6[j] = (Decimal)_resultado.Entities[j].Attributes["atos_p6"];
                    base.Log.writelog("Precio p6: " + p6[j].ToString());
                    hayp6 = true;
                }
                _pricingInput.Attributes["atos_porcentajeoimporte"] = _resultado.Entities[j].Attributes["atos_porcentajeoimporte"];
                base.Log.writelog("Tratado pricinginput " + j);
            }

            Decimal preciof = 0;
            Decimal precio1 = 0;
            Decimal precio2 = 0;
            Decimal precio3 = 0;
            Decimal precio4 = 0;
            Decimal precio5 = 0;
            Decimal precio6 = 0;

            for (int j = 0; j < _resultado.Entities.Count; j++)
            {
                preciof += (((Decimal)duracion[j] / (Decimal)duraciontotal) * pfijo[j]);
                if (haypfijo)
                    base.Log.writelog("Precio fijo " + j.ToString() + ": " + pfijo[j].ToString() + " total:" + preciof.ToString());
                precio1 += (((Decimal)duracion[j] / (Decimal)duraciontotal) * p1[j]);
                if (hayp1)
                    base.Log.writelog("Precio p1 " + j.ToString() + ": " + p1[j].ToString() + " total: " + precio1.ToString());
                precio2 += (((Decimal)duracion[j] / (Decimal)duraciontotal) * p2[j]);
                if (hayp2)
                    base.Log.writelog("Precio p2 " + j.ToString() + ": " + p2[j].ToString() + " total: " + precio2.ToString());
                precio3 += (((Decimal)duracion[j] / (Decimal)duraciontotal) * p3[j]);
                if (hayp3)
                    base.Log.writelog("Precio p3 " + j.ToString() + ": " + p3[j].ToString() + " total: " + precio3.ToString());
                precio4 += (((Decimal)duracion[j] / (Decimal)duraciontotal) * p4[j]);
                if (hayp4)
                    base.Log.writelog("Precio p4 " + j.ToString() + ": " + p4[j].ToString() + " total: " + precio4.ToString());
                precio5 += (((Decimal)duracion[j] / (Decimal)duraciontotal) * p5[j]);
                if (hayp5)
                    base.Log.writelog("Precio p5 " + j.ToString() + ": " + p5[j].ToString() + " total: " + precio5.ToString());
                precio6 += (((Decimal)duracion[j] / (Decimal)duraciontotal) * p6[j]);
                if (hayp6)
                    base.Log.writelog("Precio p6 " + j.ToString() + ": " + p6[j].ToString() + " total: " + precio6.ToString());
            }

            if (haypfijo)
            {
                base.Log.writelog("Precio fijo " + preciof.ToString());
                _pricingInput.Attributes["atos_pfijo"] = preciof;
            }
            if (hayp1)
            {
                base.Log.writelog("Precio p1 " + precio1.ToString());
                _pricingInput.Attributes["atos_p1"] = precio1;
            }
            if (hayp2)
            {
                base.Log.writelog("Precio p2 " + precio2.ToString());
                _pricingInput.Attributes["atos_p2"] = precio2;
            }
            if (hayp3)
            {
                base.Log.writelog("Precio p3 " + precio3.ToString());
                _pricingInput.Attributes["atos_p3"] = precio3;
            }
            if (hayp4)
            {
                base.Log.writelog("Precio p4 " + precio4.ToString());
                _pricingInput.Attributes["atos_p4"] = precio4;
            }
            if (hayp5)
            {
                base.Log.writelog("Precio p5 " + precio5.ToString());
                _pricingInput.Attributes["atos_p5"] = precio5;
            }
            if (hayp6)
            {
                base.Log.writelog("Precio p6 " + precio6.ToString());
                _pricingInput.Attributes["atos_p6"] = precio6;
            }

            base.Log.writelog("Devuelve pricinginput");
            return _pricingInput;
            //return _resultado.Entities[0];
        }


        private Entity nuevoPricingInput(DateTime _fechainicio, DateTime _fechafin, DateTime _createdon,
                                     int indicecomponente, Guid _tarifaid, Guid _sistemaelectricoid)
        {

            Entity _pricingInput = new Entity("atos_pricinginput");


            _pricingInput.Attributes["atos_fechainicioaplicacion"] = _fechainicio;
            _pricingInput.Attributes["atos_fechafinaplicacion"] = _fechafin;
            _pricingInput.Attributes["atos_terminodepricingid"] = new EntityReference("atos_terminodepricing", ((TermPricing)componentes[indicecomponente]).TermpricingId);
            _pricingInput.Attributes["atos_fechainiciovigencia"] = _createdon;

            _pricingInput.Attributes["atos_fechafinvigencia"] = new DateTime(4000, 1, 1); // Se pone fin de vigencia del pricing input al 1/01/4000 para los contratos a pasado y siempre que no encuentre pricing input base. _pricingInput.Attributes["atos_fechafinaplicacion"];

            if (((TermPricing)componentes[indicecomponente]).DependeDeTarifa == true)
                _pricingInput.Attributes["atos_tarifaid"] = new EntityReference("atos_tarifa", _tarifaid);

            _pricingInput.Attributes["atos_tipo"] = new OptionSetValue(((TermPricing)componentes[indicecomponente]).FijoIndexado.Value); ;

            if (((TermPricing)componentes[indicecomponente]).DependeDeSistemaElectrico == true)
                _pricingInput.Attributes["atos_sistemaelectricoid"] = new EntityReference("atos_sistemaelectrico", _sistemaelectricoid);
            return _pricingInput;
        }


        private EntityCollection construyePricingInputs(EntityCollection encontrados, DateTime _fechainicio, DateTime _fechafin,
                                                        int indicecomponente, Entity _oferta, Entity _ofpre)
        {
            DateTime _createdon;
            Guid _tarifaid = Guid.Empty;
            Guid _sistemaelectricoid = Guid.Empty;
            EntityCollection _pricingInputs = new EntityCollection();


            if (((TermPricing)componentes[indicecomponente]).DependeDeTarifa == true)
            {
                if (_oferta.Attributes.Contains("atos_tarifaid"))
                    _tarifaid = ((EntityReference)_oferta.Attributes["atos_tarifaid"]).Id;
                else
                    _tarifaid = ((EntityReference)_ofpre.Attributes["atos_tarifaid"]).Id;
            }

            if (((TermPricing)componentes[indicecomponente]).DependeDeSistemaElectrico == true)
            {

                if (_oferta.Attributes.Contains("atos_sistemaelectricoid"))
                    _sistemaelectricoid = ((EntityReference)_oferta.Attributes["atos_sistemaelectricoid"]).Id;
                else
                    _sistemaelectricoid = ((EntityReference)_ofpre.Attributes["atos_sistemaelectricoid"]).Id;
            }

            if (_oferta.Attributes.Contains("createdon"))
                _createdon = (DateTime)_oferta.Attributes["createdon"];
            else //if (_ofpre.Attributes.Contains("createdon"))
                _createdon = (DateTime)_ofpre.Attributes["createdon"];

            if (encontrados.Entities.Count > 0)
            {
                if ((DateTime)encontrados.Entities[0].Attributes["atos_fechainicioaplicacion"] > _fechainicio)
                {
                    _pricingInputs.Entities.Add(nuevoPricingInput(_fechainicio, ((DateTime)encontrados.Entities[0].Attributes["atos_fechainicioaplicacion"]).AddDays(-1), _createdon,
                                        indicecomponente, _tarifaid, _sistemaelectricoid));
                }

                for (int i = 0; i < encontrados.Entities.Count; i++)
                    _pricingInputs.Entities.Add(encontrados.Entities[i]);

                if ((DateTime)encontrados.Entities[encontrados.Entities.Count - 1].Attributes["atos_fechafinaplicacion"] < _fechafin)
                {
                    _pricingInputs.Entities.Add(nuevoPricingInput(((DateTime)encontrados.Entities[encontrados.Entities.Count - 1].Attributes["atos_fechafinaplicacion"]).AddDays(1), _fechafin, _createdon,
                                        indicecomponente, _tarifaid, _sistemaelectricoid));
                }
            }
            else
            {
                _pricingInputs.Entities.Add(nuevoPricingInput(_fechainicio, _fechafin, _createdon, indicecomponente, _tarifaid, _sistemaelectricoid));
            }


            return _pricingInputs;
        }

        public EntityCollection pricingInputCollection(int i, IOrganizationService _service,
                            Entity _oferta, Entity _ofpre, bool _ofertaid = false) //, Entity _tipoProducto, Entity _tarifa)
        {
            //PricingInput pricingInput = new PricingInput(_service, TracingService, log, ficherolog);
            PricingInput pricingInput = new PricingInput(_service, Log);
            pricingInput.FacturacionEstimada = this.facturacionEstimada;
            return pricingInput.coleccion(ref componentes, i, _oferta, _ofpre, _ofertaid);
        }

        private EntityCollection pricingInputCollectionNOVALE(int i, IOrganizationService _service,
                            Entity _oferta, Entity _ofpre, bool _ofertaid = false) //, Entity _tipoProducto, Entity _tarifa)
        {

            DateTime _fechaInicioOferta, _fechaFinOferta;

            // Fechas de la Oferta
            if (_oferta.Attributes.Contains("atos_fechainicio"))
                _fechaInicioOferta = (DateTime)_oferta.Attributes["atos_fechainicio"];
            else
                _fechaInicioOferta = (DateTime)_ofpre.Attributes["atos_fechainicio"];

            if (_oferta.Attributes.Contains("atos_fechafin"))
                _fechaFinOferta = (DateTime)_oferta.Attributes["atos_fechafin"];
            else
                _fechaFinOferta = (DateTime)_ofpre.Attributes["atos_fechafin"]; 

            Log.writelog("1 pricingInputCollection " + _fechaInicioOferta.ToLocalTime().ToShortDateString() + " - " + _fechaFinOferta.ToLocalTime().ToShortDateString());
            QueryExpression _consulta = new QueryExpression("atos_pricinginput");
            /* 23866 +1 */
            _consulta.NoLock = true;
            Log.writelog("2 pricingInputCollection");
            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_terminodepricingid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(((TermPricing)componentes[i]).TermpricingId.ToString());
            _filtro.Conditions.Add(_condicion);
            Log.writelog("3 pricingInputCollection " + ((TermPricing)componentes[i]).TermpricingId.ToString());

            //if (((TermPricing)componentes[i]).DependeDeTarifa == true)
            //{
            //    _condicion = new ConditionExpression();
            //    _condicion.AttributeName = "atos_tarifaid";
            //    _condicion.Operator = ConditionOperator.Equal;
            //    if (_oferta.Attributes.Contains("atos_tarifaid"))
            //        _condicion.Values.Add(((EntityReference)_oferta.Attributes["atos_tarifaid"]).Id.ToString());
            //    else
            //        _condicion.Values.Add(((EntityReference)_ofpre.Attributes["atos_tarifaid"]).Id.ToString());
            //    Log.writelog("4 pricingInputCollection");
            //    _filtro.Conditions.Add(_condicion);
            //}


            //if (((TermPricing)componentes[i]).DependeDeSistemaElectrico == true)
            //{
            //    _condicion = new ConditionExpression();
            //    _condicion.AttributeName = "atos_sistemaelectricoid";
            //    _condicion.Operator = ConditionOperator.Equal;
            //    if (_oferta.Attributes.Contains("atos_sistemaelectricoid"))
            //        _condicion.Values.Add(((EntityReference)_oferta.Attributes["atos_sistemaelectricoid"]).Id.ToString());
            //    else
            //        _condicion.Values.Add(((EntityReference)_ofpre.Attributes["atos_sistemaelectricoid"]).Id.ToString());
            //    Log.writelog("5 pricingInputCollection");
            //    _filtro.Conditions.Add(_condicion);
            //}

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_tarifaid";
            if (((TermPricing)componentes[i]).DependeDeTarifa == true)
            {
                _condicion.Operator = ConditionOperator.Equal;
                if (_oferta.Attributes.Contains("atos_tarifaid"))
                    _condicion.Values.Add(((EntityReference)_oferta.Attributes["atos_tarifaid"]).Id.ToString());
                else
                    _condicion.Values.Add(((EntityReference)_ofpre.Attributes["atos_tarifaid"]).Id.ToString());
            }
            else
                _condicion.Operator = ConditionOperator.Null;

            Log.writelog("4 pricingInputCollection");
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_sistemaelectricoid";
            if (((TermPricing)componentes[i]).DependeDeSistemaElectrico == true)
            {
                _condicion.Operator = ConditionOperator.Equal;
                if (_oferta.Attributes.Contains("atos_sistemaelectricoid"))
                    _condicion.Values.Add(((EntityReference)_oferta.Attributes["atos_sistemaelectricoid"]).Id.ToString());
                else
                    _condicion.Values.Add(((EntityReference)_ofpre.Attributes["atos_sistemaelectricoid"]).Id.ToString());
            }
            else
                _condicion.Operator = ConditionOperator.Null;

            Log.writelog("5 pricingInputCollection");
            _filtro.Conditions.Add(_condicion);

            Log.writelog("6 pricingInputCollection");
            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_fechainiciovigencia";
            _condicion.Operator = ConditionOperator.LessEqual;
            _condicion.Values.Add(DateTime.Now);
            _filtro.Conditions.Add(_condicion);

            Log.writelog("7 pricingInputCollection");
            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_fechafinvigencia";
            _condicion.Operator = ConditionOperator.GreaterEqual;
            _condicion.Values.Add(DateTime.Now);
            _filtro.Conditions.Add(_condicion);

            /*Log.writelog("8 pricingInput");
            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_contratoid";
            _condicion.Operator = ConditionOperator.Null;
            _filtro.Conditions.Add(_condicion);*/

            if (_ofertaid == true)
            {
                Log.writelog("9 pricingInputCollection " + _ofpre.Id.ToString());
                _condicion = new ConditionExpression();
                _condicion.AttributeName = "atos_ofertaid";
                _condicion.Operator = ConditionOperator.Equal;
                _condicion.Values.Add(_ofpre.Id.ToString());
                _filtro.Conditions.Add(_condicion);
            }
            else
            {
                Log.writelog("10 pricingInputCollection");
                _condicion = new ConditionExpression();
                _condicion.AttributeName = "atos_ofertaid";
                _condicion.Operator = ConditionOperator.Null;
                _filtro.Conditions.Add(_condicion);

                Log.writelog("11 pricingInputCollection");
                _condicion = new ConditionExpression();
                _condicion.AttributeName = "atos_fechainicioaplicacion";
                _condicion.Operator = ConditionOperator.LessEqual;
                _condicion.Values.Add(_fechaFinOferta);

                /*if (_oferta.Attributes.Contains("atos_fechafin"))
                    _condicion.Values.Add(_oferta.Attributes["atos_fechafin"]);
                else
                    _condicion.Values.Add(_ofpre.Attributes["atos_fechafin"]);*/
                _filtro.Conditions.Add(_condicion);

                Log.writelog("12 pricingInputCollection");
                _condicion = new ConditionExpression();
                _condicion.AttributeName = "atos_fechafinaplicacion";
                _condicion.Operator = ConditionOperator.GreaterEqual;
                _condicion.Values.Add(_fechaInicioOferta);
                /*if (_oferta.Attributes.Contains("atos_fechainicio"))
                    _condicion.Values.Add(_oferta.Attributes["atos_fechainicio"]);
                else
                    _condicion.Values.Add(_ofpre.Attributes["atos_fechainicio"]);*/
                _filtro.Conditions.Add(_condicion);
                Log.writelog("13 pricingInputCollection");

            }
            _consulta.Criteria.AddFilter(_filtro);

            OrderExpression _order = new OrderExpression();

            _order.AttributeName = "atos_terminodepricingid";
            _order.OrderType = OrderType.Ascending;
            _consulta.Orders.Add(_order);

            _order = new OrderExpression();
            _order.AttributeName = "atos_fechainicioaplicacion";
            _order.OrderType = OrderType.Ascending;

            _consulta.Orders.Add(_order);

            Log.writelog("14 pricingInputCollection");
            _consulta.ColumnSet.AddColumns("atos_pricinginputid", "atos_terminodepricingid", "atos_porcentajeoimporte", "atos_pfijo", "atos_p1", "atos_p2", "atos_p3",
                                    "atos_p4", "atos_p5", "atos_p6", "atos_tipo", "atos_fechainicioaplicacion", "atos_fechafinaplicacion", "atos_fechainiciovigencia", "atos_fechafinvigencia");

            Log.writelog("15 pricingInputCollection");
            EntityCollection _resultado = _service.RetrieveMultiple(_consulta);

            Log.writelog("16 pricingInputCollection");
            if (_resultado.Entities.Count > 0)
            {
                if ((DateTime)_resultado.Entities[0].Attributes["atos_fechainicioaplicacion"] <= _fechaInicioOferta
                    && (DateTime)_resultado.Entities[_resultado.Entities.Count - 1].Attributes["atos_fechafinaplicacion"] >= _fechaFinOferta)
                    return _resultado;
            }

            if (_ofertaid == true) // && _resultado.Entities.Count < 1)
                throw new Exception("ERROR. Encontrados " + _resultado.Entities.Count.ToString() + " pricing input (" + ((TermPricing)componentes[i]).NombreComponente + ")");

            Log.writelog("17 pricingInput");


            return construyePricingInputs(_resultado, _fechaInicioOferta, _fechaFinOferta, i, _oferta, _ofpre);

            /*
            EntityCollection _pricingInputs = new EntityCollection();

            Entity _pricingInput = new Entity("atos_pricinginput");

            if (_oferta.Attributes.Contains("atos_fechainicio"))
                _pricingInput.Attributes["atos_fechainicioaplicacion"] = _oferta.Attributes["atos_fechainicio"];
            else
                _pricingInput.Attributes["atos_fechainicioaplicacion"] = _ofpre.Attributes["atos_fechainicio"];


            if (_oferta.Attributes.Contains("atos_fechafin"))
                _pricingInput.Attributes["atos_fechafinaplicacion"] = _oferta.Attributes["atos_fechafin"];
            else
                _pricingInput.Attributes["atos_fechafinaplicacion"] = _ofpre.Attributes["atos_fechafin"];

            _pricingInput.Attributes["atos_terminodepricingid"] = new EntityReference("atos_terminodepricing", ((TermPricing)componentes[i]).TermpricingId);

            if (_oferta.Attributes.Contains("createdon"))
                _pricingInput.Attributes["atos_fechainiciovigencia"] = _oferta.Attributes["createdon"];
            else //if (_ofpre.Attributes.Contains("createdon"))
                _pricingInput.Attributes["atos_fechainiciovigencia"] = _ofpre.Attributes["createdon"];

            _pricingInput.Attributes["atos_fechafinvigencia"] = new DateTime(4000, 1, 1); // Se pone fin de vigencia del pricing input al 1/01/4000 para los contratos a pasado y siempre que no encuentre pricing input base. _pricingInput.Attributes["atos_fechafinaplicacion"];

            
            if (((TermPricing)componentes[i]).DependeDeTarifa == true)
            {
                if (_oferta.Attributes.Contains("atos_tarifaid"))
                    _pricingInput.Attributes["atos_tarifaid"] = new EntityReference("atos_tarifa", ((EntityReference)_oferta.Attributes["atos_tarifaid"]).Id);
                else
                    _pricingInput.Attributes["atos_tarifaid"] = new EntityReference("atos_tarifa", ((EntityReference)_ofpre.Attributes["atos_tarifaid"]).Id);
            }

            _pricingInput.Attributes["atos_tipo"] = new OptionSetValue(((TermPricing)componentes[i]).FijoIndexado.Value);

            if (((TermPricing)componentes[i]).DependeDeSistemaElectrico == true)
            {
                
                if (_oferta.Attributes.Contains("atos_sistemaelectricoid"))
                    _pricingInput.Attributes["atos_sistemaelectricoid"] = new EntityReference("atos_sistemaelectrico", ((EntityReference)_oferta.Attributes["atos_sistemaelectricoid"]).Id);
                else
                    _pricingInput.Attributes["atos_sistemaelectricoid"] = new EntityReference("atos_sistemaelectrico", ((EntityReference)_ofpre.Attributes["atos_sistemaelectricoid"]).Id);
            }

            _pricingInputs.Entities.Add(_pricingInput);

            // Crear entidades a partir del Termino de Pricing.
            return _pricingInputs;*/

        }


        public void calcula(ref List<FormulaBase> _variables, IOrganizationService _service,
                            Entity _oferta, Entity _ofpre, Entity _tipoProducto, Entity _tarifa, ref List<String> _errores)
        {
            bool _calculosintermedios = true;
            if (_tarifa.Attributes.Contains("atos_numeroperiodos") == true)
                periodo = (Decimal)_tarifa.Attributes["atos_numeroperiodos"];
            if (periodo < 0)
            {
                _errores.Add("La tarifa " + _tarifa.Attributes["atos_name"].ToString() + " no tiene periodo");
                return;
                //throw new Exception("Tarifa no tiene periodo");
            }


            String[] _formula = new String[7];
            for (int i = 0; i < 7; i++)
                _formula[i] = expresion;

            Entity _pricingOutPut;

            for (int i = 0; i < componentes.Count; i++)
            {
                if (componentes[i].TipoComponente == "TermPricing")
                {
                    try
                    {
                        this.Log.writelog("Componente: " + componentes[i].NombreComponente);
                        Entity _pricingInput = pricingInput(i, _service, _oferta, _ofpre, true); //, _tipoProducto, _tarifa);
                        
                        ((TermPricing)componentes[i]).setLog(Log);
                        ((TermPricing)componentes[i]).calcula(_tarifa, _pricingInput);

                        _pricingOutPut = new Entity("atos_pricingoutput");

                        _pricingOutPut.Attributes["atos_terminodepricingid"] = new EntityReference("atos_terminodepricing", ((TermPricing)componentes[i]).TermpricingId);
                        _pricingOutPut.Attributes["atos_ofertaid"] = new EntityReference("atos_oferta", _oferta.Id);

                        if (_pricingInput.Attributes.Contains("atos_tipo"))
                            _pricingOutPut.Attributes["atos_tipo"] = new OptionSetValue(((OptionSetValue)_pricingInput.Attributes["atos_tipo"]).Value);
                        else
                            _pricingOutPut.Attributes["atos_tipo"] = new OptionSetValue(((TermPricing)componentes[i]).FijoIndexado.Value);

                        Decimal _periodos = ((TermPricing)componentes[i]).NumeroPeriodos;
                        this.Log.writelog(componentes[i].NombreComponente + " tiene " + _periodos.ToString() + " periodos.");

                        OptionSetValue _preporc = ((TermPricing)componentes[i]).PrecioPorcentaje;
                        if (_preporc != null)
                            _pricingOutPut.Attributes["atos_porcentajeoimporte"] = new OptionSetValue(_preporc.Value);

                        if (((TermPricing)componentes[i]).FijoIndexado == null)
                            Log.writelog("Formula: " + nombre + " calcula: " + ((TermPricing)componentes[i]).NombreComponente + " FijoIndexado a null");
                        else
                            Log.writelog("Formula: " + nombre + " calcula: " + ((TermPricing)componentes[i]).NombreComponente + " FijoIndexado no es null");

                        if (((TermPricing)componentes[i]).FijoIndexado.Value == 300000000)
                        {
                            Log.writelog("Formula calcula, componente " + componentes[i].NombreComponente + " es fijo");
                            if (periodo > 0 && _periodos > 0 && _periodos < periodo)
                                throw new Exception("El término de pricing " + componentes[i].NombreComponente + " solo tiene precios definidos para " + _periodos.ToString() + ", sin embargo la tarifa " + _tarifa.Attributes["atos_name"].ToString() + " está definida para " + periodo.ToString());
                            if (periodo == 0 && _periodos > 0)
                                throw new Exception("El término de pricing " + componentes[i].NombreComponente + " tiene precios por periodos (" + _periodos.ToString() + "), sin embargo la tarifa " + _tarifa.Attributes["atos_name"].ToString() + " es de precio fijo");

                            if (periodo == 0)
                            {
                                _pricingOutPut.Attributes["atos_pfijo"] = decimal.Round(((TermPricing)componentes[i]).Valores[0], precisiondecimalesterminos, MidpointRounding.AwayFromZero);
                                _formula[0] = _formula[0].Replace(" " + componentes[i].NombreComponente + " ", " " + ((TermPricing)componentes[i]).Valores[0].ToString() + " ");
                            }
                            else
                            {
                                for (int j = 1; j <= periodo; j++)
                                {
                                    if (_periodos == 0)
                                    {
                                        _pricingOutPut.Attributes["atos_pfijo"] = decimal.Round(((TermPricing)componentes[i]).Valores[0], precisiondecimalesterminos, MidpointRounding.AwayFromZero);
                                        _formula[j] = _formula[j].Replace(" " + componentes[i].NombreComponente + " ", " " + ((TermPricing)componentes[i]).Valores[0].ToString() + " ");
                                    }
                                    else
                                    {
                                        _pricingOutPut.Attributes["atos_p" + j] = decimal.Round(((TermPricing)componentes[i]).Valores[j], precisiondecimales, MidpointRounding.AwayFromZero);
                                        _formula[j] = _formula[j].Replace(" " + componentes[i].NombreComponente + " ", " " + ((TermPricing)componentes[i]).Valores[j].ToString() + " ");
                                    }
                                }
                            }
                        }
                        else
                        {
                            Log.writelog("Formula calcula, componente " + componentes[i].NombreComponente + " es indexado");
                            fijoindexado = new OptionSetValue(((TermPricing)componentes[i]).FijoIndexado.Value);
                        }
                        Log.writelog("Formula calcula, componente " + componentes[i].NombreComponente + " generando pricing output");
                        /*
                        if (_periodos == 0)
                        {
                            _pricingOutPut.Attributes["atos_pfijo"] = decimal.Round(((TermPricing)componentes[i]).Valores[0], 6);
                            _formula[0] = _formula[0].Replace(componentes[i].NombreComponente, componentes[i].Valores[0].ToString());
                        }
                        else
                        {
                            for (int j = 1; j <= _periodos; j++)
                            {
                                _pricingOutPut.Attributes["atos_p" + j] = decimal.Round(((TermPricing)componentes[i]).Valores[j], 6);
                                _formula[j] = _formula[j].Replace(componentes[i].NombreComponente, ((TermPricing)componentes[i]).Valores[j].ToString());
                            }
                        }
                        */
                        if (((TermPricing)componentes[i]).NombreEms != "")
                            _pricingOutPut.Attributes["atos_terminoems"] = ((TermPricing)componentes[i]).NombreEms;
                        else
                            _pricingOutPut.Attributes["atos_terminoems"] = componentes[i].NombreComponente;

                        if (_oferta.Attributes.Contains("atos_name"))
                            _pricingOutPut.Attributes["atos_name"] = componentes[i].NombreComponente + "-" + _oferta.Attributes["atos_name"];
                        else if (_ofpre.Attributes.Contains("atos_name"))
                            _pricingOutPut.Attributes["atos_name"] = componentes[i].NombreComponente + "-" + _ofpre.Attributes["atos_name"];
                        //Console.WriteLine("Damos de alta pricingOutput para " + componentes[i].Nombre);


                        if (_oferta.Attributes.Contains("atos_fechainicio"))
                            _pricingOutPut.Attributes["atos_fechainicioaplicacion"] = _oferta.Attributes["atos_fechainicio"];
                        else if (_ofpre.Attributes.Contains("atos_fechainicio"))
                            _pricingOutPut.Attributes["atos_fechainicioaplicacion"] = _ofpre.Attributes["atos_fechainicio"];

                        if (_oferta.Attributes.Contains("atos_fechafin"))
                            _pricingOutPut.Attributes["atos_fechafinaplicacion"] = _oferta.Attributes["atos_fechafin"];
                        else if (_ofpre.Attributes.Contains("atos_fechafin"))
                            _pricingOutPut.Attributes["atos_fechafinaplicacion"] = _ofpre.Attributes["atos_fechafin"];

                        Log.writelog("Antes de Create pricing output");
                        _service.Create(_pricingOutPut);
                        Log.writelog("Despues de Create pricing output");
                    }
                    catch (Exception e)
                    {
                        _calculosintermedios = false;
                        Log.writelog(e.Message + " Formula: " + nombre + " Componente " + i.ToString());
                        _errores.Add(e.Message + " Formula: " + nombre + " Componente " + i.ToString());
                        //_errores.Add(e.Message);
                    }

                }
            }


            for (int i = 0; i < _variables.Count; i++)
            {
                for (int j = 0; j < componentes.Count; j++)
                {
                    if (componentes[j].TipoComponente == "Variable" && componentes[j].NombreComponente == _variables[i].NombreFormula)
                    {
                        try
                        {
                            this.Log.writelog("Variable: " + _variables[i].NombreFormula);
                            if (((Variable)_variables[i]).calcula(ref componentes, ref _variables, j, _service, _tipoProducto, _tarifa, ref _errores) == true)
                            {

                                _pricingOutPut = new Entity("atos_pricingoutput");
                                _pricingOutPut.Attributes["atos_ofertaid"] = new EntityReference("atos_oferta", _oferta.Id);

                                if (((Variable)_variables[i]).FijoOIndexado == null)
                                    Log.writelog("Formula: " + nombre + " calcula (variable): " + ((Variable)_variables[i]).NombreFormula + " FijoIndexado a null");

                                if (((Variable)_variables[i]).FijoOIndexado.Value == 300000000) // Solo se puede calcular si todos los términos son fijos (no indexados)
                                {
                                    this.Log.writelog("Tipo fijo");
                                    if (periodo == 0)
                                    {
                                        _pricingOutPut.Attributes["atos_pfijo"] = componentes[j].Valores[0];
                                        _formula[0] = _formula[0].Replace(" " + componentes[j].NombreComponente + " ", " " + componentes[j].Valores[0].ToString() + " ");
                                    }
                                    else
                                    {
                                        for (int k = 1; k <= periodo; k++)
                                        {
                                            _pricingOutPut.Attributes["atos_p" + k] = componentes[j].Valores[k];
                                            _formula[k] = _formula[k].Replace(" " + componentes[j].NombreComponente + " ", " " + componentes[j].Valores[k].ToString() + " ");
                                        }
                                    }
                                }
                                else
                                {
                                    this.Log.writelog("Tipo indexado " + ((Variable)_variables[i]).FijoOIndexado.Value.ToString());
                                    fijoindexado = new OptionSetValue(((Variable)_variables[i]).FijoOIndexado.Value);
                                }

                                _pricingOutPut.Attributes["atos_porcentajeoimporte"] = new OptionSetValue(300000002);
                                _pricingOutPut.Attributes["atos_terminoems"] = componentes[j].NombreComponente;

                                _pricingOutPut.Attributes["atos_tipo"] = new OptionSetValue(((Variable)_variables[i]).FijoOIndexado.Value);

                                if (_oferta.Attributes.Contains("atos_name"))
                                    _pricingOutPut.Attributes["atos_name"] = componentes[i].NombreComponente + "-" + _oferta.Attributes["atos_name"];
                                else if (_ofpre.Attributes.Contains("atos_name"))
                                    _pricingOutPut.Attributes["atos_name"] = componentes[i].NombreComponente + "-" + _ofpre.Attributes["atos_name"];
                                //Console.WriteLine("Damos de alta pricingOutput para Variable: " + componentes[j].Nombre);


                                if (_oferta.Attributes.Contains("atos_fechainicio"))
                                    _pricingOutPut.Attributes["atos_fechainicioaplicacion"] = _oferta.Attributes["atos_fechainicio"];
                                else if (_ofpre.Attributes.Contains("atos_fechainicio"))
                                    _pricingOutPut.Attributes["atos_fechainicioaplicacion"] = _ofpre.Attributes["atos_fechainicio"];

                                if (_oferta.Attributes.Contains("atos_fechafin"))
                                    _pricingOutPut.Attributes["atos_fechafinaplicacion"] = _oferta.Attributes["atos_fechafin"];
                                else if (_ofpre.Attributes.Contains("atos_fechafin"))
                                    _pricingOutPut.Attributes["atos_fechafinaplicacion"] = _ofpre.Attributes["atos_fechafin"];



                                _service.Create(_pricingOutPut);
                            }
                            else
                            {
                                Log.writelog("  calcula devuelve false");
                                _calculosintermedios = false;
                            }
                        }
                        catch (Exception e)
                        {
                            _calculosintermedios = false;
                            Log.writelog(e.Message + " Formula: " + nombre + " variable " + i.ToString() + " componente " + j.ToString());
                            _errores.Add(e.Message + " Formula: " + nombre + " variable " + i.ToString() + " componente " + j.ToString());
                            //_errores.Add(e.Message);
                        }
                    }
                }
            }

            if (_calculosintermedios == true)
            {
                Log.writelog("Calculando fórmula final");
                _pricingOutPut = new Entity("atos_pricingoutput");

                _pricingOutPut.Attributes["atos_ofertaid"] = new EntityReference("atos_oferta", _oferta.Id);
                if (fijoindexado == null)
                    Log.writelog("Formula " + nombre + " calcula " + nombre + " fijoindexado a nulo");
                else
                    Log.writelog("Formula " + nombre + " calcula " + nombre + " fijoindexado no es nulo");
                if (fijoindexado.Value == 300000000) // Solo se puede calcular si todos los términos de la fórmula son de tipo fijo
                {
                    if (periodo == 0)
                    {
                        this.Log.writelog("evaluando _formula[0]: [" + _formula[0] + "]");
                        _pricingOutPut.Attributes["atos_pfijo"] = decimal.Round(FormulaBase.Evaluate(_formula[0]), precisiondecimales, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        for (int i = 1; i <= periodo; i++)
                        {
                            this.Log.writelog("evaluando _formula[" + i.ToString() + "]: [" + _formula[i] + "]");
                            _pricingOutPut.Attributes["atos_p" + i] = decimal.Round(FormulaBase.Evaluate(_formula[i]), precisiondecimales, MidpointRounding.AwayFromZero);
                        }
                    }
                }

                _pricingOutPut.Attributes["atos_terminoems"] = nombre;

                if (_oferta.Attributes.Contains("atos_name"))
                    _pricingOutPut.Attributes["atos_name"] = nombre + "-" + _oferta.Attributes["atos_name"];
                else if (_ofpre.Attributes.Contains("atos_name"))
                    _pricingOutPut.Attributes["atos_name"] = nombre + "-" + _ofpre.Attributes["atos_name"];

                _pricingOutPut.Attributes["atos_tipo"] = new OptionSetValue(fijoindexado.Value);

                _pricingOutPut.Attributes["atos_porcentajeoimporte"] = new OptionSetValue(300000002);
                //Console.WriteLine("Damos de alta pricingOutput para Fórmula: " + nombre);


                if (_oferta.Attributes.Contains("atos_fechainicio"))
                    _pricingOutPut.Attributes["atos_fechainicioaplicacion"] = _oferta.Attributes["atos_fechainicio"];
                else if (_ofpre.Attributes.Contains("atos_fechainicio"))
                    _pricingOutPut.Attributes["atos_fechainicioaplicacion"] = _ofpre.Attributes["atos_fechainicio"];
    
                if (_oferta.Attributes.Contains("atos_fechafin"))
                    _pricingOutPut.Attributes["atos_fechafinaplicacion"] = _oferta.Attributes["atos_fechafin"];
                else if (_ofpre.Attributes.Contains("atos_fechafin"))
                    _pricingOutPut.Attributes["atos_fechafinaplicacion"] = _ofpre.Attributes["atos_fechafin"];

                _service.Create(_pricingOutPut);
            }
            else
                _errores.Add("No se han podido realizar todos los cálculos intermedios");
        }


        public decimal MaximoComunDivisor(decimal u, decimal v)
        {

            while (u > 0)
            {
                if (u < v)
                {   // intercambiamos valores
                    decimal aux = v;
                    v = u;
                    u = aux;
                }
                u -= v; // u = u-v por el algoritmo de Euclides
            }

            return v;
        }

        public void calculaCollection(ref List<FormulaBase> _variables, IOrganizationService _service,
                            Entity _oferta, Entity _ofpre, Entity _tipoProducto, Entity _tarifa, ref List<String> _errores)
        {
            Entity _pricingOutPut;
            Log.writelog("calculaCollection", true);
            bool _calculosintermedios = true;
            if (_tarifa.Attributes.Contains("atos_numeroperiodos") == true)
                periodo = (Decimal)_tarifa.Attributes["atos_numeroperiodos"];
            if (periodo <= 0)
            {
                _errores.Add("La tarifa " + _tarifa.Attributes["atos_name"].ToString() + " no tiene periodo");
                return;
                //throw new Exception("Tarifa no tiene periodo");
            }


            if (_oferta.Attributes.Contains("atos_precisiondecimalesoferta"))
                precisiondecimales = ((OptionSetValue) _oferta.Attributes["atos_precisiondecimalesoferta"]).Value - 300000000;
            else if (_ofpre.Attributes.Contains("atos_precisiondecimalesoferta"))
                precisiondecimales = ((OptionSetValue)_ofpre.Attributes["atos_precisiondecimalesoferta"]).Value - 300000000;


            Log.writelog("Tratando " + componentes.Count + " componentes de la fórmula.");
            decimal mesesmcd = -1;

            Log.writelog("Recuperando fechas de aplicación.");


            
            // Error de zona en fechas. No toma la zona local del usuario en el Online.
            // Se calcula en DateTimeSpan la zona del usuario y se suma el offset
            //DateTime fechainicio = ((DateTime)_ofpre.Attributes["atos_fechainicio"]).ToLocalTime();
            //DateTime fechafin = ((DateTime)_ofpre.Attributes["atos_fechafin"]).ToLocalTime();
            DateTime fechainicio = DateTimeSpan.DateTimeLocal((DateTime)_ofpre.Attributes["atos_fechainicio"], _service);
            DateTime fechafin = DateTimeSpan.DateTimeLocal((DateTime)_ofpre.Attributes["atos_fechafin"], _service);



            Log.writelog("fechainicio: " + fechainicio.Day + "/" + fechainicio.Month + "/" + fechainicio.Year + " " + fechainicio.Hour + ":" + fechainicio.Minute);
            
            
            for (int i = 0; i < componentes.Count; i++)
            {
                if (componentes[i].TipoComponente == "TermPricing")
                {
                    try
                    {
                        this.Log.writelog("Componente (Coll): " + componentes[i].NombreComponente, true);
                        EntityCollection _pricingintputs = pricingInputCollection(i, _service, _oferta, _ofpre, true);
                        
                        ((TermPricing)componentes[i]).setLog(Log);
                        ((TermPricing)componentes[i]).calculaCollection(_tarifa, _pricingintputs);

                        Log.writelog("Numero de PricingInputs " + ((TermPricing)componentes[i]).NumeroPricingInputs.ToString(), true);

                        /*for (int j = 0; j < ((TermPricing)componentes[i]).NumeroPricingInputs; j++)
                        {
                            if (_pricingintputs[j].Attributes.Contains("atos_tipo") &&
                                 ((OptionSetValue)_pricingintputs[j].Attributes["atos_tipo"]).Value != 300000000)
                                ((TermPricing)componentes[i]).FijoIndexado.Value = ((OptionSetValue)_pricingintputs[j].Attributes["atos_tipo"]).Value;
                        }*/
                        for (int j = 0; j < _pricingintputs.Entities.Count; j++)
                        {
                            if (_pricingintputs[j].Attributes.Contains("atos_tipo") &&
                                 ((OptionSetValue)_pricingintputs[j].Attributes["atos_tipo"]).Value != 300000000)
                                ((TermPricing)componentes[i]).FijoIndexado.Value = ((OptionSetValue)_pricingintputs[j].Attributes["atos_tipo"]).Value;
                        }

                        Log.writelog("PricingInput, FijoIndexado " + ((TermPricing)componentes[i]).FijoIndexado.Value.ToString());
                        if (((TermPricing)componentes[i]).FijoIndexado.Value != 300000000 &&
                            ((TermPricing)componentes[i]).NumeroPricingInputs == 0) // Es indexado
                        {
                            _pricingOutPut = new Entity("atos_pricingoutput");

                            _pricingOutPut.Attributes["atos_terminodepricingid"] = new EntityReference("atos_terminodepricing", ((TermPricing)componentes[i]).TermpricingId);
                            _pricingOutPut.Attributes["atos_ofertaid"] = new EntityReference("atos_oferta", _oferta.Id);

                            _pricingOutPut.Attributes["atos_tipo"] = new OptionSetValue(((TermPricing)componentes[i]).FijoIndexado.Value);

                            _pricingOutPut.Attributes["atos_fechainicioaplicacion"] = fechainicio;
                            _pricingOutPut.Attributes["atos_fechafinaplicacion"] = fechafin;

                            
                            if (((TermPricing)componentes[i]).NombreEms != "")
                                _pricingOutPut.Attributes["atos_terminoems"] = ((TermPricing)componentes[i]).NombreEms;
                            else
                                _pricingOutPut.Attributes["atos_terminoems"] = componentes[i].NombreComponente;

                            if (_oferta.Attributes.Contains("atos_name"))
                                _pricingOutPut.Attributes["atos_name"] = componentes[i].NombreComponente + "-" + _oferta.Attributes["atos_name"] + "-1";
                            else if (_ofpre.Attributes.Contains("atos_name"))
                                _pricingOutPut.Attributes["atos_name"] = componentes[i].NombreComponente + "-" + _ofpre.Attributes["atos_name"] + "-1";

                            _pricingOutPut.Attributes["atos_porcentajeoimporte"] = new OptionSetValue(300000002); //No aplica
                            _pricingOutPut.Attributes["atos_facturacionestimada"] = facturacionEstimada;

                            Log.writelog("Antes de Create pricing output");
                            _service.Create(_pricingOutPut);
                            Log.writelog("Despues de Create pricing output");

                        }
                        else
                        {
                            for (int j = 0; j < ((TermPricing)componentes[i]).NumeroPricingInputs; j++)
                            {
                                ValorPricingInput _vpi = ((TermPricing)componentes[i]).pricingInput(j);

                                _pricingOutPut = new Entity("atos_pricingoutput");


                                if (_pricingintputs[j].Attributes.Contains("atos_porcentajeoimporte"))
                                    _pricingOutPut.Attributes["atos_porcentajeoimporte"] = new OptionSetValue(((OptionSetValue)_pricingintputs[j].Attributes["atos_porcentajeoimporte"]).Value);
                                else
                                    _pricingOutPut.Attributes["atos_porcentajeoimporte"] = new OptionSetValue(300000002); //No aplica


                                _pricingOutPut.Attributes["atos_terminodepricingid"] = new EntityReference("atos_terminodepricing", ((TermPricing)componentes[i]).TermpricingId);
                                _pricingOutPut.Attributes["atos_ofertaid"] = new EntityReference("atos_oferta", _oferta.Id);

                                _pricingOutPut.Attributes["atos_tipo"] = new OptionSetValue(((TermPricing)componentes[i]).FijoIndexado.Value);

                                Decimal _periodos = _vpi.NumeroPeriodos; // ((TermPricing)componentes[i]).NumeroPeriodos;

                                if (_vpi.FechaInicioAplicacion < fechainicio)
                                    _pricingOutPut.Attributes["atos_fechainicioaplicacion"] = fechainicio;
                                else
                                    _pricingOutPut.Attributes["atos_fechainicioaplicacion"] = _vpi.FechaInicioAplicacion;

                                

                                if (_vpi.FechaFinAplicacion > fechafin)
                                    _pricingOutPut.Attributes["atos_fechafinaplicacion"] = fechafin;
                                else
                                    _pricingOutPut.Attributes["atos_fechafinaplicacion"] = _vpi.FechaFinAplicacion;


                                //decimal anyos = Decimal.Round(Decimal.Divide(((TimeSpan)((DateTime)_pricingOutPut.Attributes["atos_fechafinaplicacion"] - (DateTime)_pricingOutPut.Attributes["atos_fechainicioaplicacion"])).Days + 1, 365), 0);
                                //decimal dias = ((TimeSpan)((DateTime)_pricingOutPut.Attributes["atos_fechafinaplicacion"] - (DateTime)_pricingOutPut.Attributes["atos_fechainicioaplicacion"])).Days + 1;
                                decimal meses;//= (decimal)(12 * anyos) + Decimal.Round(Decimal.Divide((dias - (365 * anyos)), 30), 0);

                                DateTimeSpan diferencia = DateTimeSpan.CompareDates((DateTime)_pricingOutPut.Attributes["atos_fechainicioaplicacion"], (DateTime)_pricingOutPut.Attributes["atos_fechafinaplicacion"]);
                                //Log.writelog("Meses DateTimeSpan " + (diferencia.Years * 12 + diferencia.Months + (diferencia.Days > 0 ? 1 : 0)).ToString());
                                meses = diferencia.Years * 12 + diferencia.Months + (diferencia.Days > 0 ? 1 : 0);

                                //Log.writelog("Años: " + anyos.ToString() + ". Dias = " + dias.ToString());
                                if (mesesmcd == -1)
                                    mesesmcd = meses;
                                else
                                    mesesmcd = MaximoComunDivisor(meses, mesesmcd);

                                Log.writelog("Meses PI: " + _vpi.Meses.ToString() + " Meses: " + meses.ToString() + " Periodos: " + _periodos + " Periodo tarifa: " + periodo);

                                if (((TermPricing)componentes[i]).FijoIndexado.Value == 300000000) // Si es fijo se calcula el valor
                                {
                                    for (int k = 1; k <= periodo; k++)
                                    {
                                        Log.writelog("calculando periodo " + k.ToString());
                                        if (_periodos == 0)
                                        {
                                            // No hay que prorratear nunca los importes
                                            //if (_vpi.Porcentaje == true)
                                            _pricingOutPut.Attributes["atos_p" + k] = decimal.Round(_vpi.Valores[0], precisiondecimalesterminos, MidpointRounding.AwayFromZero);
                                            //else
                                            //    _pricingOutPut.Attributes["atos_p" + k] = decimal.Round(_vpi.Valores[0] * (meses / _vpi.Meses), 6);
                                        }
                                        else
                                        {
                                            // No hay que prorratear nunca los importes
                                            //if (_vpi.Porcentaje == true)
                                            _pricingOutPut.Attributes["atos_p" + k] = decimal.Round(_vpi.Valores[k], precisiondecimalesterminos, MidpointRounding.AwayFromZero);
                                            //else
                                            //    _pricingOutPut.Attributes["atos_p" + k] = decimal.Round(_vpi.Valores[k] * (meses / _vpi.Meses), 6);
                                        }
                                    }
                                }


                                if (((TermPricing)componentes[i]).NombreEms != "")
                                    _pricingOutPut.Attributes["atos_terminoems"] = ((TermPricing)componentes[i]).NombreEms;
                                else
                                    _pricingOutPut.Attributes["atos_terminoems"] = componentes[i].NombreComponente;

                                if (_oferta.Attributes.Contains("atos_name"))
                                    _pricingOutPut.Attributes["atos_name"] = componentes[i].NombreComponente + "-" + _oferta.Attributes["atos_name"] + "-" + (j + 1);
                                else if (_ofpre.Attributes.Contains("atos_name"))
                                    _pricingOutPut.Attributes["atos_name"] = componentes[i].NombreComponente + "-" + _ofpre.Attributes["atos_name"] + "-" + (j + 1);


                                _pricingOutPut.Attributes["atos_facturacionestimada"] = facturacionEstimada;

                                Log.writelog("Antes de Create pricing output");
                                _service.Create(_pricingOutPut);
                                Log.writelog("Despues de Create pricing output");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _calculosintermedios = false;
                        Log.writelog(e.Message + " Formula: " + nombre + " Componente " + i.ToString());
                        _errores.Add(e.Message + " Formula: " + nombre + " Componente " + i.ToString());
                    }
                }
            }
            Log.writelog("Meses MCD: " + mesesmcd.ToString());
            if (mesesmcd < 1)
                mesesmcd = 1;
            Log.writelog("Fechas de la oferta. Fechainicio " + fechainicio.ToShortDateString() + " - fecha fin " + fechafin.ToShortDateString());

            Log.writelog("Calculando fechas de aplicación del primer mes.");
            DateTime fini = fechainicio;
            DateTime ffin = fini.AddMonths((int)mesesmcd).AddDays(-1);
            Log.writelog("Fechas de aplicacion del primer mes " + fini.ToShortDateString() + " - fecha fin " + ffin.ToShortDateString());
            Log.writelog("Crear pricing output de términos de pricing");
            Log.writelog("Expresion: " + " " + expresion.Replace("(", " ( ").Replace(")", " ) ").Replace("+", " + ").Replace("-", " - ").Replace("/", " / ").Replace("*", " * ") + " ");
            int tramo = 0;
            
            //while (ffin <= fechafin)
            while (fini < fechafin ) // Chequeamos contra la fecha de inicio para tratar el último tramo cuando la duración no es un número entero.
            {
                if (ffin > fechafin)
                {
                    Log.writelog("Ultimo tramo");
                    ffin = fechafin;
                }
                ++tramo;
                String[] _formula = new String[7];
                for (int i = 0; i < 7; i++)
                    _formula[i] = " " + expresion.Replace("(", " ( ").Replace(")", " ) ").Replace("+", " + ").Replace("-", " - ").Replace("/", " / ").Replace("*", " * ") + " ";

                fijoindexado = new OptionSetValue(300000000);
                Log.writelog("Rango de fechas: " + fini.ToShortDateString() + " - " + ffin.ToShortDateString());
                for (int i = 0; i < componentes.Count; i++)
                {
                    if (componentes[i].TipoComponente == "TermPricing")
                    {
                        try
                        {
                            this.Log.writelog("Componente (Coll), recuperando valor para la fórmula: " + componentes[i].NombreComponente);
                            /*_pricingOutPut = new Entity("atos_pricingoutput");


                            _pricingOutPut.Attributes["atos_terminodepricingid"] = new EntityReference("atos_terminodepricing", ((TermPricing)componentes[i]).TermpricingId);
                            _pricingOutPut.Attributes["atos_ofertaid"] = new EntityReference("atos_oferta", _oferta.Id);

                            _pricingOutPut.Attributes["atos_tipo"] = new OptionSetValue(((TermPricing)componentes[i]).FijoIndexado.Value);
                            */
                            Decimal _periodos = ((TermPricing)componentes[i]).NumeroPeriodos;

                            if (((TermPricing)componentes[i]).FijoIndexado.Value == 300000000) // Si es fijo se calculan los valores
                            {
                                ValorPricingInput _valorPricingInput = ((TermPricing)componentes[i]).valorPricingInput(fini, ffin);
                                if (_valorPricingInput != null)
                                {
                                    _periodos = _valorPricingInput.NumeroPeriodos;

                                    DateTime _fechaIniPO;
                                    DateTime _fechaFinPO;
                                    if (_valorPricingInput.FechaInicioAplicacion < fechainicio)
                                        _fechaIniPO = fechainicio;
                                    else
                                        _fechaIniPO = _valorPricingInput.FechaInicioAplicacion;

                                    if (_valorPricingInput.FechaFinAplicacion > fechafin)
                                        _fechaFinPO = fechafin;
                                    else
                                        _fechaFinPO = _valorPricingInput.FechaFinAplicacion;


                                    /*decimal anyos = Decimal.Round(Decimal.Divide(((TimeSpan)(_fechaFinPO - _fechaIniPO)).Days + 1, 365), 0);
                                    decimal dias = ((TimeSpan)(_fechaFinPO - _fechaIniPO)).Days + 1;
                                    decimal meses = (decimal)(12 * anyos) + Decimal.Round(Decimal.Divide((dias - (365 * anyos)), 30), 0);*/

                                    Log.writelog("Sustituyendo en fórmula el valor del componente para " + mesesmcd.ToString() + " meses de un total de " + _valorPricingInput.Meses.ToString());

                                    for (int j = 1; j <= periodo; j++)
                                    {
                                        if (_periodos == 0)
                                        {
                                            //_pricingOutPut.Attributes["atos_p" + j] = decimal.Round(_valorPricingInput.ValorMes[0], 6);
                                            // No hay que prorratear nunca los importes
                                            //if (_valorPricingInput.Porcentaje == true) // Si es porcentaje no se prorratea al mes
                                            _formula[j] = _formula[j].Replace(" " + componentes[i].NombreComponente + " ", _valorPricingInput.Valores[0].ToString()); //  _valorPricingInput.ValorMes[0].ToString());
                                            //else
                                            //    _formula[j] = _formula[j].Replace(" " + componentes[i].NombreComponente + " ", (_valorPricingInput.Valores[0] * (mesesmcd / _valorPricingInput.Meses)).ToString()); //  _valorPricingInput.ValorMes[0].ToString());
                                        }
                                        else
                                        {
                                            //_pricingOutPut.Attributes["atos_p" + j] = decimal.Round(_valorPricingInput.ValorMes[j], 6);
                                            // No hay que prorratear nunca los importes
                                            //if (_valorPricingInput.Porcentaje == true) // Si es porcentaje no se prorratea al mes
                                            _formula[j] = _formula[j].Replace(" " + componentes[i].NombreComponente + " ", _valorPricingInput.Valores[j].ToString()); //  _valorPricingInput.ValorMes[0].ToString());
                                            //else
                                            //    _formula[j] = _formula[j].Replace(" " + componentes[i].NombreComponente + " ", (_valorPricingInput.Valores[j] * (mesesmcd / _valorPricingInput.Meses)).ToString()); //_valorPricingInput.ValorMes[j].ToString());
                                        }
                                    }


                                    if (((TermPricing)componentes[i]).FijoIndexado.Value != 300000000)
                                        fijoindexado = new OptionSetValue(((TermPricing)componentes[i]).FijoIndexado.Value);

                                    /*if (((TermPricing)componentes[i]).NombreEms != "")
                                        _pricingOutPut.Attributes["atos_terminoems"] = ((TermPricing)componentes[i]).NombreEms;
                                    else
                                        _pricingOutPut.Attributes["atos_terminoems"] = componentes[i].NombreComponente;

                                    if (_oferta.Attributes.Contains("atos_name"))
                                        _pricingOutPut.Attributes["atos_name"] = componentes[i].NombreComponente + "-" + _oferta.Attributes["atos_name"];
                                    else if (_ofpre.Attributes.Contains("atos_name"))
                                        _pricingOutPut.Attributes["atos_name"] = componentes[i].NombreComponente + "-" + _ofpre.Attributes["atos_name"];


                                    Log.writelog("Inicializando fechas");
                                    if (_oferta.Attributes.Contains("atos_fechainicio"))
                                        _pricingOutPut.Attributes["atos_fechainicioaplicacion"] = fini;
                                    else if (_ofpre.Attributes.Contains("atos_fechainicio"))
                                        _pricingOutPut.Attributes["atos_fechainicioaplicacion"] = _ofpre.Attributes["atos_fechainicio"];

                                    if (_oferta.Attributes.Contains("atos_fechafin"))
                                        _pricingOutPut.Attributes["atos_fechafinaplicacion"] = ffin;
                                    else if (_ofpre.Attributes.Contains("atos_fechafin"))
                                        _pricingOutPut.Attributes["atos_fechafinaplicacion"] = _ofpre.Attributes["atos_fechafin"];

                                    Log.writelog("Antes de Create pricing output");
                                    _service.Create(_pricingOutPut);
                                    Log.writelog("Despues de Create pricing output");*/
                                }
                                else
                                {
                                    _calculosintermedios = false;
                                    Log.writelog("No encuentra PricingInput para " + fini.ToShortDateString() + "-" + ffin.ToShortDateString() + ". Formula: " + nombre + " Componente " + componentes[i].NombreComponente);
                                    _errores.Add("No encuentra PricingInput para " + fini.ToShortDateString() + "-" + ffin.ToShortDateString() + ". Formula: " + nombre + " Componente " + componentes[i].NombreComponente);
                                }
                            }
                            else
                            {
                                Log.writelog("Formula calcula, componente " + componentes[i].NombreComponente + " es indexado");
                                fijoindexado = new OptionSetValue(((TermPricing)componentes[i]).FijoIndexado.Value);
                            }
                        }
                        catch (Exception e)
                        {
                            _calculosintermedios = false;
                            Log.writelog(e.Message + " Formula: " + nombre + " Componente " + componentes[i].NombreComponente);
                            _errores.Add(e.Message + " Formula: " + nombre + " Componente " + componentes[i].NombreComponente);
                        }
                    }
                }

                // cálculo de variables intermedias
                Log.writelog("Variables (" + _variables.Count + ")");
                for (int i = 0; i < _variables.Count; i++)
                {
                    Log.writelog("Buscando variable: " + _variables[i].NombreFormula + " entre los componentes " + componentes.Count);
                    for (int j = 0; j < componentes.Count; j++)
                    {
                        if (componentes[j].TipoComponente == "Variable" && componentes[j].NombreComponente == _variables[i].NombreFormula)
                        {
                            try
                            {
                                ((Variable)_variables[i]).PrecisionDecimales = precisiondecimales;
                                ((Variable)_variables[i]).PrecisionDecimalesTerminos = precisiondecimalesterminos;
                                this.Log.writelog("Variable " + j + ": " + _variables[i].NombreFormula);
                                if (((Variable)_variables[i]).calculaPorTramos(ref componentes, ref _variables, j, _service, _tipoProducto, _tarifa, ref _errores, fini, ffin, mesesmcd) == true)
                                {
                                    Log.writelog("Después de calculaPorTramos para " + _variables[i].NombreFormula + " en " + fini.ToShortDateString() + " - " + ffin.ToShortDateString());
                                    _pricingOutPut = new Entity("atos_pricingoutput");
                                    _pricingOutPut.Attributes["atos_ofertaid"] = new EntityReference("atos_oferta", _oferta.Id);

                                    if (((Variable)_variables[i]).FijoOIndexado == null)
                                        Log.writelog("Formula: " + nombre + " calcula (variable): " + ((Variable)_variables[i]).NombreFormula + " FijoIndexado a null");

                                    if (((Variable)_variables[i]).FijoOIndexado.Value == 300000000) // Solo se puede calcular si todos los términos son fijos (no indexados)
                                    {
                                        this.Log.writelog("Tipo fijo");
                                        if (periodo == 0)
                                        {
                                            Log.writelog("Variable " + ((Variable)_variables[i]).NombreFormula + " valor: " + componentes[j].Valores[0].ToString());
                                            _pricingOutPut.Attributes["atos_p1"] = componentes[j].Valores[0];
                                            _formula[0] = _formula[0].Replace(" " + componentes[j].NombreComponente + " ", componentes[j].Valores[0].ToString());
                                        }
                                        else
                                        {
                                            for (int k = 1; k <= periodo; k++)
                                            {
                                                Log.writelog("Variable " + ((Variable)_variables[i]).NombreFormula + " periodo " + k.ToString() + " valor: " + componentes[j].Valores[k].ToString());
                                                _pricingOutPut.Attributes["atos_p" + k] = componentes[j].Valores[k];
                                                _formula[k] = _formula[k].Replace(" " + componentes[j].NombreComponente + " ", componentes[j].Valores[k].ToString());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        this.Log.writelog("Tipo indexado " + ((Variable)_variables[i]).FijoOIndexado.Value.ToString());
                                        fijoindexado = new OptionSetValue(((Variable)_variables[i]).FijoOIndexado.Value);
                                    }

                                    _pricingOutPut.Attributes["atos_porcentajeoimporte"] = new OptionSetValue(300000002);
                                    _pricingOutPut.Attributes["atos_terminoems"] = componentes[j].NombreComponente;

                                    _pricingOutPut.Attributes["atos_tipo"] = new OptionSetValue(((Variable)_variables[i]).FijoOIndexado.Value);

                                    if (_oferta.Attributes.Contains("atos_name"))
                                        _pricingOutPut.Attributes["atos_name"] = componentes[i].NombreComponente + "-" + _oferta.Attributes["atos_name"] + "-" + tramo.ToString();
                                    else if (_ofpre.Attributes.Contains("atos_name"))
                                        _pricingOutPut.Attributes["atos_name"] = componentes[i].NombreComponente + "-" + _ofpre.Attributes["atos_name"] + "-" + tramo.ToString();
                                    //Console.WriteLine("Damos de alta pricingOutput para Variable: " + componentes[j].Nombre);


                                    /*if (_oferta.Attributes.Contains("atos_fechainicio"))
                                        _pricingOutPut.Attributes["atos_fechainicioaplicacion"] = _oferta.Attributes["atos_fechainicio"];
                                    else if (_ofpre.Attributes.Contains("atos_fechainicio"))
                                        _pricingOutPut.Attributes["atos_fechainicioaplicacion"] = _ofpre.Attributes["atos_fechainicio"];

                                    if (_oferta.Attributes.Contains("atos_fechafin"))
                                        _pricingOutPut.Attributes["atos_fechafinaplicacion"] = _oferta.Attributes["atos_fechafin"];
                                    else if (_ofpre.Attributes.Contains("atos_fechafin"))
                                        _pricingOutPut.Attributes["atos_fechafinaplicacion"] = _ofpre.Attributes["atos_fechafin"];*/

                                    _pricingOutPut.Attributes["atos_fechainicioaplicacion"] = fini;
                                    _pricingOutPut.Attributes["atos_fechafinaplicacion"] = ffin;
                                    _pricingOutPut.Attributes["atos_facturacionestimada"] = facturacionEstimada;
                                    Log.writelog("Antes de Create pricingOutput variable " + _variables[i].NombreFormula);
                                    _service.Create(_pricingOutPut);
                                    Log.writelog("Después de Create pricingOutput variable " + _variables[i].NombreFormula);
                                }
                                else
                                {
                                    Log.writelog("  calcula devuelve false");
                                    _calculosintermedios = false;
                                }
                            }
                            catch (Exception e)
                            {
                                _calculosintermedios = false;
                                Log.writelog(e.Message + " Formula: " + nombre + " variable " + i.ToString() + " componente " + j.ToString());
                                _errores.Add(e.Message + " Formula: " + nombre + " variable " + i.ToString() + " componente " + j.ToString());
                                //_errores.Add(e.Message);
                            }
                        }
                    }
                }


                // cálculo final de la fórmula
                if (_calculosintermedios == true)
                {
                    Log.writelog("Calculando fórmula final");
                    _pricingOutPut = new Entity("atos_pricingoutput");

                    _pricingOutPut.Attributes["atos_ofertaid"] = new EntityReference("atos_oferta", _oferta.Id);
                    if (fijoindexado == null)
                        Log.writelog("Formula " + nombre + " calcula " + nombre + " fijoindexado a nulo");
                    else
                        Log.writelog("Formula " + nombre + " calcula " + nombre + " fijoindexado no es nulo");
                    if (fijoindexado.Value == 300000000) // Solo se puede calcular si todos los términos de la fórmula son de tipo fijo
                    {
                        if (periodo == 0)
                        {
                            this.Log.writelog("evaluando _formula[0]: [" + _formula[0] + "]");
                            _pricingOutPut.Attributes["atos_p1"] = decimal.Round(FormulaBase.Evaluate(_formula[0]), precisiondecimales, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            for (int i = 1; i <= periodo; i++)
                            {
                                this.Log.writelog("evaluando _formula[" + i.ToString() + "]: [" + _formula[i] + "]");
                                _pricingOutPut.Attributes["atos_p" + i] = decimal.Round(FormulaBase.Evaluate(_formula[i]), precisiondecimales, MidpointRounding.AwayFromZero);
                            }
                        }
                    }
                    
                    _pricingOutPut.Attributes["atos_terminoems"] = nombre;

                    if (_oferta.Attributes.Contains("atos_name"))
                        _pricingOutPut.Attributes["atos_name"] = nombre + "-" + _oferta.Attributes["atos_name"] + "-" + tramo.ToString();
                    else if (_ofpre.Attributes.Contains("atos_name"))
                        _pricingOutPut.Attributes["atos_name"] = nombre + "-" + _ofpre.Attributes["atos_name"] + "-" + tramo.ToString();

                    _pricingOutPut.Attributes["atos_tipo"] = new OptionSetValue(fijoindexado.Value);

                    _pricingOutPut.Attributes["atos_porcentajeoimporte"] = new OptionSetValue(300000002);
                    //Console.WriteLine("Damos de alta pricingOutput para Fórmula: " + nombre);


                    /*if (_oferta.Attributes.Contains("atos_fechainicio"))
                        _pricingOutPut.Attributes["atos_fechainicioaplicacion"] = _oferta.Attributes["atos_fechainicio"];
                    else if (_ofpre.Attributes.Contains("atos_fechainicio"))
                        _pricingOutPut.Attributes["atos_fechainicioaplicacion"] = _ofpre.Attributes["atos_fechainicio"];

                    if (_oferta.Attributes.Contains("atos_fechafin"))
                        _pricingOutPut.Attributes["atos_fechafinaplicacion"] = _oferta.Attributes["atos_fechafin"];
                    else if (_ofpre.Attributes.Contains("atos_fechafin"))
                        _pricingOutPut.Attributes["atos_fechafinaplicacion"] = _ofpre.Attributes["atos_fechafin"];*/

                    _pricingOutPut.Attributes["atos_fechainicioaplicacion"] = fini;
                    _pricingOutPut.Attributes["atos_fechafinaplicacion"] = ffin;
                    _pricingOutPut.Attributes["atos_facturacionestimada"] = facturacionEstimada;
                    _service.Create(_pricingOutPut);
                }
                else
                    _errores.Add("No se han podido realizar todos los cálculos intermedios");

                fini = ffin.AddDays(1);
                ffin = fini.AddMonths((int)mesesmcd).AddDays(-1);
            }


        }


        /**
        // <summary>
        // Recupera la relación de horas por periodo para un intervalo de tiempo
        // </summary>
        // <param name="_service">Objeto IOrganizationService para recuperar datos de CRM.</param>
        // <param name="_matrizhorariaid">Id de la matriz horaria de la oferta.</param>
        // <param name="_fini">Fecha inicial del periodo a evaluar.</param>
        // <param name="_ffin">Fecha final del periodo a evaluar.</param>
        // <remarks>
        // Recupera de la entidad Horas Tarifas el número de días del intervalo y el número de horas
        // - Copia del producto base
        // - Clonar los pricing inputs asociados a la oferta
        // - Calcular los pricing output (hay que meter también pricing output para la fórmula final y para las variables intermedias, en estos casos el pricing output no irá relacionado con un término de pricing)
        // </remarks>
         */
        public Entity horasMatrizHoraria(IOrganizationService _service, Guid _matrizhorariaid, DateTime _fini, DateTime _ffin, Entity _horasTotal)
        {

            Entity horasMatriz = new Entity();
            decimal _vaux = 0;
            Log.writelog("horasMatrizHoraria " + _fini.ToLongDateString() + " - " + _ffin.ToLongDateString());
            /*horasMatriz.Attributes["horas"] = _vaux;
            horasMatriz.Attributes["horasp1"] = _vaux;
            horasMatriz.Attributes["horasp2"] = _vaux;
            horasMatriz.Attributes["horasp3"] = _vaux;
            horasMatriz.Attributes["horasp4"] = _vaux;
            horasMatriz.Attributes["horasp5"] = _vaux;
            horasMatriz.Attributes["horasp6"] = _vaux;

            Log.writelog("horasMatrizHoraria despues de inicializar horas y horasp-");*/
            horasMatriz.Attributes["ponderacionp1"] = _vaux;
            horasMatriz.Attributes["ponderacionp2"] = _vaux;
            horasMatriz.Attributes["ponderacionp3"] = _vaux;
            horasMatriz.Attributes["ponderacionp4"] = _vaux;
            horasMatriz.Attributes["ponderacionp5"] = _vaux;
            horasMatriz.Attributes["ponderacionp6"] = _vaux;
            Log.writelog("horasMatrizHoraria despues de inicializar ponderacionp-");

            QueryExpression _queryExp = new QueryExpression("atos_horastarifas");
            _queryExp.ColumnSet = new ColumnSet(new String[] { "atos_horasp1", "atos_horasp2", "atos_horasp3", "atos_horasp4", "atos_horasp5", "atos_horasp6" });
            /* 23866 +1 */
            _queryExp.NoLock = true;

            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_matrizhorariaid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(_matrizhorariaid.ToString());
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_dia";
            _condicion.Operator = ConditionOperator.GreaterEqual;
            _condicion.Values.Add(new DateTime(_fini.Year, _fini.Month, _fini.Day, 0, 0, 0));
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_dia";
            _condicion.Operator = ConditionOperator.LessEqual;
            _condicion.Values.Add(new DateTime(_ffin.Year, _ffin.Month, _ffin.Day, 23, 59, 59));
            _filtro.Conditions.Add(_condicion);

            _queryExp.Criteria.AddFilter(_filtro);

            EntityCollection _resultado = _service.RetrieveMultiple(_queryExp);
			decimal []  _horas = new decimal[6];
			for (int i=0;i<6;i++)
				_horas[i] = _vaux;

            for (int i = 0; i < _resultado.Entities.Count; i++)
            {
				for (int j = 1; j <= 6; j++)
                {
                    if (_resultado.Entities[i].Attributes.Contains("atos_horasp" + j.ToString()))
                    {
                        _horas[j - 1] += (Decimal)_resultado.Entities[i].Attributes["atos_horasp" + j.ToString()];
                    }
                }
            }
			
			for (int i = 1; i <= 6; i++)
			{
				decimal horasTotal = (decimal)_horasTotal.Attributes["horasp" + i.ToString()];
                Log.writelog("Horas total periodo " + i.ToString());
				if (horasTotal > 0)
				{
                    Log.writelog("Horas p" + i.ToString() + ": " + _horas[i-1]);
					decimal _vpond = _horas[i-1] / horasTotal;
                    Log.writelog("Ponderación " + i.ToString() + ": " + _vpond.ToString());
					horasMatriz.Attributes["ponderacionp" + i.ToString()] = _vpond;
				}
			}




            /* --- QUITANDO FECTH POR FORMATO FECHAS
            String _query = string.Format("<fetch aggregate='true' >" +
                                            " <entity name='atos_horastarifas' >" +
                                           // "     <attribute name='atos_dia' alias='dias' aggregate='count' />" +
                                            "     <attribute name='atos_horasp1' alias='horasp1' aggregate='sum' />" +
                                            " 	<attribute name='atos_horasp2' alias='horasp2' aggregate='sum' />" +
                                            " 	<attribute name='atos_horasp3' alias='horasp3' aggregate='sum' />" +
                                            " 	<attribute name='atos_horasp4' alias='horasp4' aggregate='sum' />" +
                                            " 	<attribute name='atos_horasp5' alias='horasp5' aggregate='sum' />" +
                                            " 	<attribute name='atos_horasp6' alias='horasp6' aggregate='sum' />" +
                                            " 	<filter type='and' >" +
                                            " 	    <condition attribute='atos_matrizhorariaid' operator='eq' value='{0}' />" +
                                            " 		<condition attribute='atos_dia' operator='between' >" +
                                            " 		    <value>{1}</value>" +
                                            " 			<value>{2}</value>" +
                                            " 		</condition>" +
                                            " 	</filter>" +
                                            " </entity>" +
                                            "</fetch>",
                                            _matrizhorariaid.ToString(), _fini.ToShortDateString(), _ffin.ToShortDateString());

            Log.writelog("horasMatrizHoraria _query " + _query);
            FetchExpression _expresionConsulta = new FetchExpression(_query);
            EntityCollection _resultado = _service.RetrieveMultiple(_expresionConsulta);
            if (_resultado.Entities.Count > 0)
            {
               --- QUITANDO FECTH POR FORMATO FECHAS  */
            /*if (_resultado.Entities[0].Attributes.Contains("dias"))
            {
                Log.writelog("calculando horas");

                Int32 _vdias = (Int32)((AliasedValue)_resultado.Entities[0]["dias"]).Value;

                _vaux = (decimal) (_vdias * 24);
                Log.writelog("Numero de horas " + _vaux.ToString());
                horasMatriz.Attributes["horas"] = _vaux;
                Log.writelog("Guardado el número de horas");
            }*/

            /* --- QUITANDO FECTH POR FORMATO FECHAS
                for (int i = 1; i <= 6; i++)
                {
                    decimal horasTotal = (decimal)_horasTotal.Attributes["horasp" + i.ToString()];
                    Log.writelog("Horas total periodo " + i.ToString());
                    if (horasTotal > 0)
                    {
                        if (_resultado.Entities[0].Attributes.Contains("horasp" + i.ToString()))
                        {
                            decimal _vhoras = (decimal)((AliasedValue)_resultado.Entities[0]["horasp" + i.ToString()]).Value;
                            Log.writelog("Horas p" + i.ToString() + ": " + _vhoras.ToString());
                            decimal _vpond = _vhoras / horasTotal;
                            Log.writelog("Ponderación " + i.ToString() + ": " + _vpond.ToString());
                            horasMatriz.Attributes["ponderacionp" + i.ToString()] = _vpond;
                        }
                    }
                }
             --- QUITANDO FECTH POR FORMATO FECHAS */
            /*
            if (_resultado.Entities[0].Attributes.Contains("horasp1"))
                horasMatriz.Attributes["horasp1"] = (decimal)_resultado.Entities[0].Attributes["horasp1"];
            if (_resultado.Entities[0].Attributes.Contains("horasp2"))
                horasMatriz.Attributes["horasp2"] = (decimal)_resultado.Entities[0].Attributes["horasp2"];
            if (_resultado.Entities[0].Attributes.Contains("horasp3"))
                horasMatriz.Attributes["horasp3"] = (decimal)_resultado.Entities[0].Attributes["horasp3"];
            if (_resultado.Entities[0].Attributes.Contains("horasp4"))
                horasMatriz.Attributes["horasp4"] = (decimal)_resultado.Entities[0].Attributes["horasp4"];
            if (_resultado.Entities[0].Attributes.Contains("horasp5"))
                horasMatriz.Attributes["horasp5"] = (decimal)_resultado.Entities[0].Attributes["horasp5"];
            if (_resultado.Entities[0].Attributes.Contains("horasp6"))
                horasMatriz.Attributes["horasp6"] = (decimal)_resultado.Entities[0].Attributes["horasp6"];
            if ((int)horasMatriz.Attributes["horas"] > 0)
            {
                if ((decimal)horasMatriz.Attributes["horasp1"] > 0)
                    horasMatriz.Attributes["ponderacionp1"] = (decimal)horasMatriz.Attributes["horasp1"] / (decimal)horasMatriz.Attributes["horas"];

                if ((decimal)horasMatriz.Attributes["horasp2"] > 0)
                    horasMatriz.Attributes["ponderacionp2"] = (decimal)horasMatriz.Attributes["horasp2"] / (decimal)horasMatriz.Attributes["horas"];

                if ((decimal)horasMatriz.Attributes["horasp3"] > 0)
                    horasMatriz.Attributes["ponderacionp3"] = (decimal)horasMatriz.Attributes["horasp3"] / (decimal)horasMatriz.Attributes["horas"];

                if ((decimal)horasMatriz.Attributes["horasp4"] > 0)
                    horasMatriz.Attributes["ponderacionp4"] = (decimal)horasMatriz.Attributes["horasp4"] / (decimal)horasMatriz.Attributes["horas"];

                if ((decimal)horasMatriz.Attributes["horasp5"] > 0)
                    horasMatriz.Attributes["ponderacionp5"] = (decimal)horasMatriz.Attributes["horasp5"] / (decimal)horasMatriz.Attributes["horas"];

                if ((decimal)horasMatriz.Attributes["horasp6"] > 0)
                    horasMatriz.Attributes["ponderacionp6"] = (decimal)horasMatriz.Attributes["horasp6"] / (decimal)horasMatriz.Attributes["horas"];
            }*/
            /* --- QUITANDO FECTH POR FORMATO FECHAS   
            }
             * --- QUITANDO FECTH POR FORMATO FECHAS */
            return horasMatriz;
        }


        /**
        // <summary>
        // Recupera la relación de horas por periodo para un intervalo de tiempo
        // </summary>
        // <param name="_service">Objeto IOrganizationService para recuperar datos de CRM.</param>
        // <param name="_matrizhorariaid">Id de la matriz horaria de la oferta.</param>
        // <param name="_fini">Fecha inicial del periodo a evaluar.</param>
        // <param name="_ffin">Fecha final del periodo a evaluar.</param>
        // <remarks>
        // Recupera de la entidad Horas Tarifas el número de días del intervalo y el número de horas
        // - Copia del producto base
        // - Clonar los pricing inputs asociados a la oferta
        // - Calcular los pricing output (hay que meter también pricing output para la fórmula final y para las variables intermedias, en estos casos el pricing output no irá relacionado con un término de pricing)
        // </remarks>
         */
        public Entity horasPeriodo(IOrganizationService _service, Guid _matrizhorariaid, DateTime _fini, DateTime _ffin)
        {

            Entity horasPeriodo = new Entity();
            decimal _vaux = 0;
            Log.writelog("horasPeriodo");
            /*horasMatriz.Attributes["horas"] = _vaux;
            horasMatriz.Attributes["horasp1"] = _vaux;
            horasMatriz.Attributes["horasp2"] = _vaux;
            horasMatriz.Attributes["horasp3"] = _vaux;
            horasMatriz.Attributes["horasp4"] = _vaux;
            horasMatriz.Attributes["horasp5"] = _vaux;
            horasMatriz.Attributes["horasp6"] = _vaux;

            Log.writelog("horasMatrizHoraria despues de inicializar horas y horasp-");*/
            horasPeriodo.Attributes["horasp1"] = _vaux;
            horasPeriodo.Attributes["horasp2"] = _vaux;
            horasPeriodo.Attributes["horasp3"] = _vaux;
            horasPeriodo.Attributes["horasp4"] = _vaux;
            horasPeriodo.Attributes["horasp5"] = _vaux;
            horasPeriodo.Attributes["horasp6"] = _vaux;
            Log.writelog("horasPeriodo despues de inicializar horasp-");



            QueryExpression _queryExp = new QueryExpression("atos_horastarifas");
            _queryExp.ColumnSet = new ColumnSet(new String[] { "atos_horasp1", "atos_horasp2", "atos_horasp3", "atos_horasp4", "atos_horasp5", "atos_horasp6" });
            /* 23866 +1 */
            _queryExp.NoLock = true;

            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_matrizhorariaid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(_matrizhorariaid.ToString());
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_dia";
            _condicion.Operator = ConditionOperator.GreaterEqual;
            _condicion.Values.Add(_fini);
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_dia";
            _condicion.Operator = ConditionOperator.LessEqual;
            _condicion.Values.Add(_ffin);
            _filtro.Conditions.Add(_condicion);

            _queryExp.Criteria.AddFilter(_filtro);

            EntityCollection _resultado = _service.RetrieveMultiple(_queryExp);
            decimal[] _horas = new decimal[6];
            for (int i = 0; i < 6; i++)
                _horas[i] = _vaux;
            Log.writelog("HorasPeriodo registros: " + _resultado.Entities.Count.ToString());
            for (int i = 0; i < _resultado.Entities.Count; i++)
            {
                for (int j = 1; j <= 6; j++)
                {
                    if (_resultado.Entities[i].Attributes.Contains("atos_horasp" + j.ToString()))
                    {
                        Log.writelog("horasPeriodo periodo " + j.ToString() + ": " + ((Decimal)_resultado.Entities[i].Attributes["atos_horasp" + j.ToString()]).ToString());
                        _horas[j - 1] += (Decimal)_resultado.Entities[i].Attributes["atos_horasp" + j.ToString()];
                    }
                }
            }

            for (int i = 1; i <= 6; i++)
            {
                Log.writelog("Horas total p" + i.ToString() + ": " + _horas[i-1].ToString());
                horasPeriodo.Attributes["horasp" + i.ToString()] = _horas[i-1];
            }


            /* --- QUITANDO FECTH POR FORMATO FECHAS
            String _query = string.Format("<fetch aggregate='true' >" +
                                            " <entity name='atos_horastarifas' >" +
                // "     <attribute name='atos_dia' alias='dias' aggregate='count' />" +
                                            "     <attribute name='atos_horasp1' alias='horasp1' aggregate='sum' />" +
                                            " 	<attribute name='atos_horasp2' alias='horasp2' aggregate='sum' />" +
                                            " 	<attribute name='atos_horasp3' alias='horasp3' aggregate='sum' />" +
                                            " 	<attribute name='atos_horasp4' alias='horasp4' aggregate='sum' />" +
                                            " 	<attribute name='atos_horasp5' alias='horasp5' aggregate='sum' />" +
                                            " 	<attribute name='atos_horasp6' alias='horasp6' aggregate='sum' />" +
                                            " 	<filter type='and' >" +
                                            " 	    <condition attribute='atos_matrizhorariaid' operator='eq' value='{0}' />" +
                                            " 		<condition attribute='atos_dia' operator='between' >" +
                                            " 		    <value>{1}</value>" +
                                            " 			<value>{2}</value>" +
                                            " 		</condition>" +
                                            " 	</filter>" +
                                            " </entity>" +
                                            "</fetch>",
                                            _matrizhorariaid.ToString(), _fini.ToShortDateString(), _ffin.ToShortDateString());

            Log.writelog("horasPeriodo _query " + _query);
            FetchExpression _expresionConsulta = new FetchExpression(_query);
            EntityCollection _resultado = _service.RetrieveMultiple(_expresionConsulta);
            if (_resultado.Entities.Count > 0)
            {
                    for (int i = 1; i <= 6; i++)
                    {
                        if (_resultado.Entities[0].Attributes.Contains("horasp" + i.ToString()))
                        {
                            decimal _vhoras = (decimal)((AliasedValue)_resultado.Entities[0]["horasp" + i.ToString()]).Value;
                            Log.writelog("Horas total p" + i.ToString() + ": " + _vhoras.ToString());
                            horasPeriodo.Attributes["horasp" + i.ToString()] = _vhoras;
                        }
                    }

            }
            --- QUITANDO FECTH POR FORMATO FECHAS */

            return horasPeriodo;
        }
		


        public void calculaCollectionPromedio(Guid _matrizhorariaid, ref List<FormulaBase> _variables, IOrganizationService _service,
                            ref Entity _oferta, Entity _ofpre, Entity _tipoProducto, Entity _tarifa, ref List<String> _errores)
        {
			EntityCollection _terminos = new EntityCollection(); // Un elemento por cada término de pricing, variable, fórmula que contiene nombre y una EntityCollection de _pricingoutputs
            //EntityCollection _pricingOutputs=null;
            Entity _pricingOutPut;
            Log.writelog("calculaCollection");
            bool _calculosintermedios = true;
            if (_tarifa.Attributes.Contains("atos_numeroperiodos") == true)
                periodo = (Decimal)_tarifa.Attributes["atos_numeroperiodos"];
            if (periodo <= 0)
            {
                _errores.Add("La tarifa " + _tarifa.Attributes["atos_name"].ToString() + " no tiene periodo");
                return;
            }
			
			if ( _matrizhorariaid == Guid.Empty )
            {
                _errores.Add("La tarifa y sistema eléctrico no tiene asociada una matriz horaria");
                return;
            }

            if (_oferta.Attributes.Contains("atos_precisiondecimalesoferta"))
                precisiondecimales = ((OptionSetValue) _oferta.Attributes["atos_precisiondecimalesoferta"]).Value - 300000000;
            else if (_ofpre.Attributes.Contains("atos_precisiondecimalesoferta"))
                precisiondecimales = ((OptionSetValue)_ofpre.Attributes["atos_precisiondecimalesoferta"]).Value - 300000000;


            Log.writelog("Tratando " + componentes.Count + " componentes de la fórmula");
            decimal mesesmcd = -1;

            Log.writelog("Recuperando fechas de aplicación.");

            // Error de zona en fechas. No toma la zona local del usuario en el Online.
            // Se calcula en DateTimeSpan la zona del usuario y se suma el offset
            //DateTime fechainicio = ((DateTime)_ofpre.Attributes["atos_fechainicio"]).ToLocalTime();
            //DateTime fechafin = ((DateTime)_ofpre.Attributes["atos_fechafin"]).ToLocalTime();
            DateTime fechainicio = DateTimeSpan.DateTimeLocal((DateTime)_ofpre.Attributes["atos_fechainicio"], _service);
            DateTime fechafin = DateTimeSpan.DateTimeLocal((DateTime)_ofpre.Attributes["atos_fechafin"], _service);



            //fechafin - fechainicio

            Entity _horasTotal = horasPeriodo(_service, _matrizhorariaid, fechainicio.ToLocalTime(), fechafin.ToLocalTime());


            for (int i = 0; i < componentes.Count; i++)
            {
                if (componentes[i].TipoComponente == "TermPricing")
                {
                    try
                    {
                        this.Log.writelog("Componente (Coll): " + componentes[i].NombreComponente);
						Entity _termino = new Entity();
						_termino.Attributes["nombre"] = componentes[i].NombreComponente;
                        _termino.Attributes["tipo"] = "TermPricing";
                        EntityCollection _pricingOutputs = new EntityCollection();
                        EntityCollection _pricingintputs = pricingInputCollection(i, _service, _oferta, _ofpre, true);
                        
                        ((TermPricing)componentes[i]).setLog(Log);
                        ((TermPricing)componentes[i]).calculaCollection(_tarifa, _pricingintputs);

                        Log.writelog("Numero de PricingInputs " + ((TermPricing)componentes[i]).NumeroPricingInputs.ToString());

                        for (int j = 0; j < _pricingintputs.Entities.Count; j++)
                        {
                            if (_pricingintputs[j].Attributes.Contains("atos_tipo") &&
                                 ((OptionSetValue)_pricingintputs[j].Attributes["atos_tipo"]).Value != 300000000)
                                ((TermPricing)componentes[i]).FijoIndexado.Value = ((OptionSetValue)_pricingintputs[j].Attributes["atos_tipo"]).Value;
                        }

                        Log.writelog("PricingInput, FijoIndexado " + ((TermPricing)componentes[i]).FijoIndexado.Value.ToString());
                        if (((TermPricing)componentes[i]).FijoIndexado.Value != 300000000 &&
                            ((TermPricing)componentes[i]).NumeroPricingInputs == 0) // Es indexado
                        {
                            _pricingOutPut = new Entity("atos_pricingoutput");

                            _pricingOutPut.Attributes["atos_terminodepricingid"] = new EntityReference("atos_terminodepricing", ((TermPricing)componentes[i]).TermpricingId);
                            _pricingOutPut.Attributes["atos_ofertaid"] = new EntityReference("atos_oferta", _oferta.Id);

                            _pricingOutPut.Attributes["atos_tipo"] = new OptionSetValue(((TermPricing)componentes[i]).FijoIndexado.Value);

                            _pricingOutPut.Attributes["atos_fechainicioaplicacion"] = fechainicio;
                            _pricingOutPut.Attributes["atos_fechafinaplicacion"] = fechafin;

                            if (((TermPricing)componentes[i]).NombreEms != "")
                                _pricingOutPut.Attributes["atos_terminoems"] = ((TermPricing)componentes[i]).NombreEms;
                            else
                                _pricingOutPut.Attributes["atos_terminoems"] = componentes[i].NombreComponente;

                            if (_oferta.Attributes.Contains("atos_name"))
                                _pricingOutPut.Attributes["atos_name"] = componentes[i].NombreComponente + "-" + _oferta.Attributes["atos_name"] + "-1";
                            else if (_ofpre.Attributes.Contains("atos_name"))
                                _pricingOutPut.Attributes["atos_name"] = componentes[i].NombreComponente + "-" + _ofpre.Attributes["atos_name"] + "-1";

                            _pricingOutPut.Attributes["atos_porcentajeoimporte"] = new OptionSetValue(300000002); //No aplica
                            _pricingOutPut.Attributes["atos_facturacionestimada"] = facturacionEstimada;

                            Log.writelog("Antes de Create pricing output");
							//_pricingOutPuts.Entities.Add(_pricingOutPut);
                            //_service.Create(_pricingOutPut);
							_pricingOutputs.Entities.Add(_pricingOutPut);
                            Log.writelog("Despues de Create pricing output");

                        }
                        else
                        {
                            for (int j = 0; j < ((TermPricing)componentes[i]).NumeroPricingInputs; j++)
                            {
                                ValorPricingInput _vpi = ((TermPricing)componentes[i]).pricingInput(j);

                                _pricingOutPut = new Entity("atos_pricingoutput");


                                if (_pricingintputs[j].Attributes.Contains("atos_porcentajeoimporte"))
                                    _pricingOutPut.Attributes["atos_porcentajeoimporte"] = new OptionSetValue(((OptionSetValue)_pricingintputs[j].Attributes["atos_porcentajeoimporte"]).Value);
                                else
                                    _pricingOutPut.Attributes["atos_porcentajeoimporte"] = new OptionSetValue(300000002); //No aplica


                                _pricingOutPut.Attributes["atos_terminodepricingid"] = new EntityReference("atos_terminodepricing", ((TermPricing)componentes[i]).TermpricingId);
                                _pricingOutPut.Attributes["atos_ofertaid"] = new EntityReference("atos_oferta", _oferta.Id);

                                _pricingOutPut.Attributes["atos_tipo"] = new OptionSetValue(((TermPricing)componentes[i]).FijoIndexado.Value);

                                Decimal _periodos = _vpi.NumeroPeriodos; // ((TermPricing)componentes[i]).NumeroPeriodos;

                                if (_vpi.FechaInicioAplicacion < fechainicio)
                                    _pricingOutPut.Attributes["atos_fechainicioaplicacion"] = fechainicio;
                                else
                                    _pricingOutPut.Attributes["atos_fechainicioaplicacion"] = _vpi.FechaInicioAplicacion;

                                if (_vpi.FechaFinAplicacion > fechafin)
                                    _pricingOutPut.Attributes["atos_fechafinaplicacion"] = fechafin;
                                else
                                    _pricingOutPut.Attributes["atos_fechafinaplicacion"] = _vpi.FechaFinAplicacion;

                                Log.writelog("Antes de llamar a horasMatrizHoraria");


                                Entity horasMatriz = horasMatrizHoraria(_service, _matrizhorariaid, ((DateTime)_pricingOutPut.Attributes["atos_fechainicioaplicacion"]).ToLocalTime(), ((DateTime)_pricingOutPut.Attributes["atos_fechafinaplicacion"]).ToLocalTime(), _horasTotal);

                                Log.writelog("Despues de llamar a horasMatrizHoraria");
                                //decimal anyos = Decimal.Round(Decimal.Divide(((TimeSpan)((DateTime)_pricingOutPut.Attributes["atos_fechafinaplicacion"] - (DateTime)_pricingOutPut.Attributes["atos_fechainicioaplicacion"])).Days + 1, 365), 0);
                                //decimal dias = ((TimeSpan)((DateTime)_pricingOutPut.Attributes["atos_fechafinaplicacion"] - (DateTime)_pricingOutPut.Attributes["atos_fechainicioaplicacion"])).Days + 1;
                                decimal meses;//= (decimal)(12 * anyos) + Decimal.Round(Decimal.Divide((dias - (365 * anyos)), 30), 0);

                                DateTimeSpan diferencia = DateTimeSpan.CompareDates((DateTime)_pricingOutPut.Attributes["atos_fechainicioaplicacion"], (DateTime)_pricingOutPut.Attributes["atos_fechafinaplicacion"]);
                                //Log.writelog("Meses DateTimeSpan " + (diferencia.Years * 12 + diferencia.Months + (diferencia.Days > 0 ? 1 : 0)).ToString());
                                meses = diferencia.Years * 12 + diferencia.Months + (diferencia.Days > 0 ? 1 : 0);

                                //Log.writelog("Años: " + anyos.ToString() + ". Dias = " + dias.ToString());
                                if (mesesmcd == -1)
                                    mesesmcd = meses;
                                else
                                    mesesmcd = MaximoComunDivisor(meses, mesesmcd);

                                Log.writelog("Meses PI: " + _vpi.Meses.ToString() + " Meses: " + meses.ToString() + " Periodos: " + _periodos + " Periodo tarifa: " + periodo);

                                if (((TermPricing)componentes[i]).FijoIndexado.Value == 300000000) // Si es fijo se calcula el valor
                                {
                                    for (int k = 1; k <= periodo; k++)
                                    {
                                        Log.writelog("calculando periodo " + k.ToString());
                                        if (_periodos == 0)
                                        {
                                            // No hay que prorratear nunca los importes
                                            //if (_vpi.Porcentaje == true)
                                            _pricingOutPut.Attributes["atos_p" + k] = _vpi.Valores[0] * (decimal) horasMatriz.Attributes["ponderacionp" + k.ToString()];
                                            Log.writelog("   valor(0): " + ((decimal)_pricingOutPut.Attributes["atos_p" + k]).ToString());
                                            //else
                                            //    _pricingOutPut.Attributes["atos_p" + k] = decimal.Round(_vpi.Valores[0] * (meses / _vpi.Meses), 6);
                                        }
                                        else
                                        {
                                            // No hay que prorratear nunca los importes
                                            //if (_vpi.Porcentaje == true)
                                            _pricingOutPut.Attributes["atos_p" + k] = _vpi.Valores[k] * (decimal) horasMatriz.Attributes["ponderacionp" + k.ToString()];
                                            Log.writelog("   valor(" + k.ToString() + "): " + ((decimal)_pricingOutPut.Attributes["atos_p" + k]).ToString());
                                            //else
                                            //    _pricingOutPut.Attributes["atos_p" + k] = decimal.Round(_vpi.Valores[k] * (meses / _vpi.Meses), 6);
                                        }
                                    }
                                }

                                if (((TermPricing)componentes[i]).NombreEms != "")
                                    _pricingOutPut.Attributes["atos_terminoems"] = ((TermPricing)componentes[i]).NombreEms;
                                else
                                    _pricingOutPut.Attributes["atos_terminoems"] = componentes[i].NombreComponente;

                                if (_oferta.Attributes.Contains("atos_name"))
                                    _pricingOutPut.Attributes["atos_name"] = componentes[i].NombreComponente + "-" + _oferta.Attributes["atos_name"] + "-" + (j + 1);
                                else if (_ofpre.Attributes.Contains("atos_name"))
                                    _pricingOutPut.Attributes["atos_name"] = componentes[i].NombreComponente + "-" + _ofpre.Attributes["atos_name"] + "-" + (j + 1);

                                _pricingOutPut.Attributes["atos_facturacionestimada"] = facturacionEstimada;
                                Log.writelog("Antes de Create pricing output");
								_pricingOutputs.Entities.Add(_pricingOutPut);
                                //_service.Create(_pricingOutPut);
                                Log.writelog("Despues de Create pricing output");
                            }
                        }
						_termino.Attributes["pricingoutputs"] = _pricingOutputs;
						_terminos.Entities.Add(_termino);
                    }
                    catch (Exception e)
                    {
                        _calculosintermedios = false;
                        Log.writelog(e.Message + " Formula: " + nombre + " Componente " + i.ToString());
                        _errores.Add(e.Message + " Formula: " + nombre + " Componente " + i.ToString());
                    }
                }
            }
            Log.writelog("Meses MCD: " + mesesmcd.ToString());
            if (mesesmcd < 1)
                mesesmcd = 1;
            Log.writelog("Fechas de la oferta. Fechainicio " + fechainicio.ToShortDateString() + " - fecha fin " + fechafin.ToShortDateString());

            Log.writelog("Calculando fechas de aplicación del primer mes.");
            DateTime fini = fechainicio;
            
            //DateTime ffin = fini.AddMonths((int)mesesmcd).AddDays(-1);
            // Calculamos la primera fecha de fin a partir del día 1 del mes de la fecha de inicio.
            DateTime fdom = new DateTime(fini.Year, fini.Month, 1);
            DateTime ffin = fdom.AddMonths((int)mesesmcd).AddDays(-1);
            Log.writelog("Fechas de aplicacion del primer mes " + fini.ToShortDateString() + " - fecha fin " + ffin.ToShortDateString());
            Log.writelog("Crear pricing output de términos de pricing");
            Log.writelog("Expresion: " + " " + expresion.Replace("(", " ( ").Replace(")", " ) ").Replace("+", " + ").Replace("-", " - ").Replace("/", " / ").Replace("*", " * ") + " ");
            int tramo = 0;
            
            //while (ffin <= fechafin)
            while (fini < fechafin ) // Chequeamos contra la fecha de inicio para tratar el último tramo cuando la duración no es un número entero.
            {
                if (ffin > fechafin)
                {
                    Log.writelog("Ultimo tramo");
                    ffin = fechafin;
                }
                ++tramo;
                String[] _formula = new String[7];
                for (int i = 0; i < 7; i++)
                    _formula[i] = " " + expresion.Replace("(", " ( ").Replace(")", " ) ").Replace("+", " + ").Replace("-", " - ").Replace("/", " / ").Replace("*", " * ") + " ";

                fijoindexado = new OptionSetValue(300000000);
                Log.writelog("Rango de fechas: " + fini.ToShortDateString() + " - " + ffin.ToShortDateString());
                for (int i = 0; i < componentes.Count; i++)
                {
                    if (componentes[i].TipoComponente == "TermPricing")
                    {
						
                        try
                        {
                            this.Log.writelog("Componente (Coll), recuperando valor para la fórmula: " + componentes[i].NombreComponente);

                            Decimal _periodos = ((TermPricing)componentes[i]).NumeroPeriodos;

                            if (((TermPricing)componentes[i]).FijoIndexado.Value == 300000000) // Si es fijo se calculan los valores
                            {
                                ValorPricingInput _valorPricingInput = ((TermPricing)componentes[i]).valorPricingInput(fini, ffin);
                                if (_valorPricingInput != null)
                                {
                                    _periodos = _valorPricingInput.NumeroPeriodos;

                                    DateTime _fechaIniPO;
                                    DateTime _fechaFinPO;
                                    if (_valorPricingInput.FechaInicioAplicacion < fechainicio)
                                        _fechaIniPO = fechainicio;
                                    else
                                        _fechaIniPO = _valorPricingInput.FechaInicioAplicacion;

                                    if (_valorPricingInput.FechaFinAplicacion > fechafin)
                                        _fechaFinPO = fechafin;
                                    else
                                        _fechaFinPO = _valorPricingInput.FechaFinAplicacion;

                                    Log.writelog("Sustituyendo en fórmula el valor del componente para " + mesesmcd.ToString() + " meses de un total de " + _valorPricingInput.Meses.ToString());

                                    for (int j = 1; j <= periodo; j++)
                                    {
                                        if (_periodos == 0)
                                        {
                                            _formula[j] = _formula[j].Replace(" " + componentes[i].NombreComponente + " ", _valorPricingInput.Valores[0].ToString());
                                        }
                                        else
                                        {
                                            _formula[j] = _formula[j].Replace(" " + componentes[i].NombreComponente + " ", _valorPricingInput.Valores[j].ToString()); 
                                        }
                                    }


                                    if (((TermPricing)componentes[i]).FijoIndexado.Value != 300000000)
                                        fijoindexado = new OptionSetValue(((TermPricing)componentes[i]).FijoIndexado.Value);

                                }
                                else
                                {
                                    _calculosintermedios = false;
                                    Log.writelog("No encuentra PricingInput para " + fini.ToShortDateString() + "-" + ffin.ToShortDateString() + ". Formula: " + nombre + " Componente " + componentes[i].NombreComponente);
                                    _errores.Add("No encuentra PricingInput para " + fini.ToShortDateString() + "-" + ffin.ToShortDateString() + ". Formula: " + nombre + " Componente " + componentes[i].NombreComponente);
                                }
                            }
                            else
                            {
                                Log.writelog("Formula calcula, componente " + componentes[i].NombreComponente + " es indexado");
                                fijoindexado = new OptionSetValue(((TermPricing)componentes[i]).FijoIndexado.Value);
                            }
                        }
                        catch (Exception e)
                        {
                            _calculosintermedios = false;
                            Log.writelog(e.Message + " Formula: " + nombre + " Componente " + componentes[i].NombreComponente);
                            _errores.Add(e.Message + " Formula: " + nombre + " Componente " + componentes[i].NombreComponente);
                        }
                    }
                }

                // cálculo de variables intermedias
                Log.writelog("Variables (" + _variables.Count + ")");
                for (int i = 0; i < _variables.Count; i++)
                {
                    Log.writelog("Buscando variable: " + _variables[i].NombreFormula + " entre los componentes " + componentes.Count);
                    for (int j = 0; j < componentes.Count; j++)
                    {
                        if (componentes[j].TipoComponente == "Variable" && componentes[j].NombreComponente == _variables[i].NombreFormula)
                        {
                            int t;
                            for (t = 0; t < _terminos.Entities.Count; t++)
                            {
                                if (_terminos.Entities[t].Attributes["nombre"].ToString() == componentes[j].NombreComponente)
                                {
                                    break;
                                }
                            }
                            if (t == _terminos.Entities.Count)
                            {
                                Entity _termino = new Entity();
                                _termino.Attributes["nombre"] = componentes[j].NombreComponente;
                                _termino.Attributes["pricingoutputs"] = new EntityCollection();
                                _termino.Attributes["tipo"] = "Variable";
                                _terminos.Entities.Add(_termino);
                            }
                            try
                            {
                                ((Variable)_variables[i]).PrecisionDecimales = precisiondecimalesterminos; // Vamos a redondear la variable al final
                                ((Variable)_variables[i]).PrecisionDecimalesTerminos = precisiondecimalesterminos;
                                this.Log.writelog("Variable " + j + ": " + _variables[i].NombreFormula);
                                if (((Variable)_variables[i]).calculaPorTramos(ref componentes, ref _variables, j, _service, _tipoProducto, _tarifa, ref _errores, fini, ffin, mesesmcd) == true)
                                {
                                    Log.writelog("Después de calculaPorTramos para " + _variables[i].NombreFormula + " en " + fini.ToShortDateString() + " - " + ffin.ToShortDateString());
                                    _pricingOutPut = new Entity("atos_pricingoutput");
                                    _pricingOutPut.Attributes["atos_ofertaid"] = new EntityReference("atos_oferta", _oferta.Id);

                                    if (((Variable)_variables[i]).FijoOIndexado == null)
                                        Log.writelog("Formula: " + nombre + " calcula (variable): " + ((Variable)_variables[i]).NombreFormula + " FijoIndexado a null");

                                    if (((Variable)_variables[i]).FijoOIndexado.Value == 300000000) // Solo se puede calcular si todos los términos son fijos (no indexados)
                                    {

                                        Entity horasMatriz = horasMatrizHoraria(_service, _matrizhorariaid, fini.ToLocalTime(), ffin.ToLocalTime(), _horasTotal);
                                        this.Log.writelog("Tipo fijo");
                                        if (periodo == 0)
                                        {
                                            Log.writelog("Variable " + ((Variable)_variables[i]).NombreFormula + " valor: " + componentes[j].Valores[0].ToString());
                                            _pricingOutPut.Attributes["atos_p1"] = componentes[j].Valores[0] * (decimal) horasMatriz.Attributes["ponderacionp1"];
                                            _formula[0] = _formula[0].Replace(" " + componentes[j].NombreComponente + " ", componentes[j].Valores[0].ToString());
                                        }
                                        else
                                        {
                                            for (int k = 1; k <= periodo; k++)
                                            {
                                                Log.writelog("Variable " + ((Variable)_variables[i]).NombreFormula + " periodo " + k.ToString() + " valor: " + componentes[j].Valores[k].ToString());
                                                _pricingOutPut.Attributes["atos_p" + k] = componentes[j].Valores[k] * (decimal) horasMatriz.Attributes["ponderacionp" + k.ToString()];
                                                _formula[k] = _formula[k].Replace(" " + componentes[j].NombreComponente + " ", componentes[j].Valores[k].ToString());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        this.Log.writelog("Tipo indexado " + ((Variable)_variables[i]).FijoOIndexado.Value.ToString());
                                        fijoindexado = new OptionSetValue(((Variable)_variables[i]).FijoOIndexado.Value);
                                    }
                                    _pricingOutPut.Attributes["atos_porcentajeoimporte"] = new OptionSetValue(300000002);
                                    _pricingOutPut.Attributes["atos_terminoems"] = componentes[j].NombreComponente;
                                    _pricingOutPut.Attributes["atos_tipo"] = new OptionSetValue(((Variable)_variables[i]).FijoOIndexado.Value);

                                    if (_oferta.Attributes.Contains("atos_name"))
                                        _pricingOutPut.Attributes["atos_name"] = componentes[i].NombreComponente + "-" + _oferta.Attributes["atos_name"] + "-" + tramo.ToString();
                                    else if (_ofpre.Attributes.Contains("atos_name"))
                                        _pricingOutPut.Attributes["atos_name"] = componentes[i].NombreComponente + "-" + _ofpre.Attributes["atos_name"] + "-" + tramo.ToString();

                                    _pricingOutPut.Attributes["atos_fechainicioaplicacion"] = fini;
                                    _pricingOutPut.Attributes["atos_fechafinaplicacion"] = ffin;
                                    _pricingOutPut.Attributes["atos_facturacionestimada"] = facturacionEstimada;
                                    Log.writelog("Antes de Create pricingOutput variable " + _variables[i].NombreFormula);

                                    //if (((Variable)_variables[i]).FijoOIndexado.Value == 300000000)
                                    //	_pricingOutputs.Entities.Add(_pricingOutPut);
                                    //else
                                    //_service.Create(_pricingOutPut);

                                    Log.writelog("Después de Create pricingOutput variable " + _variables[i].NombreFormula);
                                    ((EntityCollection)_terminos.Entities[t].Attributes["pricingoutputs"]).Entities.Add(_pricingOutPut);
                                }
                                else
                                {
                                    Log.writelog("  calcula devuelve false");
                                    _calculosintermedios = false;
                                }
                            }
                            catch (Exception e)
                            {
                                _calculosintermedios = false;
                                Log.writelog(e.Message + " Formula: " + nombre + " variable " + i.ToString() + " componente " + j.ToString());
                                _errores.Add(e.Message + " Formula: " + nombre + " variable " + i.ToString() + " componente " + j.ToString());
                                //_errores.Add(e.Message);
                            }
                        }
                    }
                }


                // cálculo final de la fórmula
                if (_calculosintermedios == true)
                {
                    Log.writelog("Calculando fórmula final");

                    int t;
                    for (t = 0; t < _terminos.Entities.Count; t++)
                    {
                        if (_terminos.Entities[t].Attributes["nombre"].ToString() == nombre)
                        {
                            break;
                        }
                    }
                    if (t == _terminos.Entities.Count)
                    {
                        Entity _termino = new Entity();
                        _termino.Attributes["nombre"] = nombre;
                        _termino.Attributes["pricingoutputs"] = new EntityCollection();
                        _termino.Attributes["tipo"] = "Formula";
                        _terminos.Entities.Add(_termino);
                    }


                    _pricingOutPut = new Entity("atos_pricingoutput");

                    _pricingOutPut.Attributes["atos_ofertaid"] = new EntityReference("atos_oferta", _oferta.Id);
                    if (fijoindexado == null)
                        Log.writelog("Formula " + nombre + " calcula " + nombre + " fijoindexado a nulo");
                    else
                        Log.writelog("Formula " + nombre + " calcula " + nombre + " fijoindexado no es nulo");
                    if (fijoindexado.Value == 300000000) // Solo se puede calcular si todos los términos de la fórmula son de tipo fijo
                    {

                        Entity horasMatriz = horasMatrizHoraria(_service, _matrizhorariaid, fini.ToLocalTime(), ffin.ToLocalTime(), _horasTotal);
                        if (periodo == 0)
                        {
                            this.Log.writelog("evaluando _formula[0]: [" + _formula[0] + "]");
                            _pricingOutPut.Attributes["atos_p1"] = FormulaBase.Evaluate(_formula[0]) * (decimal) horasMatriz.Attributes["ponderacionp1"];
                        }
                        else
                        {
                            for (int i = 1; i <= periodo; i++)
                            {
                                this.Log.writelog("evaluando _formula[" + i.ToString() + "]: [" + _formula[i] + "]*[" + horasMatriz.Attributes["ponderacionp" + i.ToString()].ToString() + ']');
                                _pricingOutPut.Attributes["atos_p" + i] = FormulaBase.Evaluate(_formula[i]) * (decimal) horasMatriz.Attributes["ponderacionp" + i.ToString()];
                            }
                        }
                    }

                    _pricingOutPut.Attributes["atos_terminoems"] = nombre;

                    if (_oferta.Attributes.Contains("atos_name"))
                        _pricingOutPut.Attributes["atos_name"] = nombre + "-" + _oferta.Attributes["atos_name"] + "-" + tramo.ToString();
                    else if (_ofpre.Attributes.Contains("atos_name"))
                        _pricingOutPut.Attributes["atos_name"] = nombre + "-" + _ofpre.Attributes["atos_name"] + "-" + tramo.ToString();

                    _pricingOutPut.Attributes["atos_tipo"] = new OptionSetValue(fijoindexado.Value);

                    _pricingOutPut.Attributes["atos_porcentajeoimporte"] = new OptionSetValue(300000002);

                    _pricingOutPut.Attributes["atos_fechainicioaplicacion"] = fini;
                    _pricingOutPut.Attributes["atos_fechafinaplicacion"] = ffin;
                    _pricingOutPut.Attributes["atos_facturacionestimada"] = facturacionEstimada;
                    
					/*if (fijoindexado.Value == 300000000)
						_pricingOutputs.Entities.Add(_pricingOutPut);
					else
						_service.Create(_pricingOutPut);*/
                    ((EntityCollection)_terminos.Entities[t].Attributes["pricingoutputs"]).Entities.Add(_pricingOutPut);
                }
                else
                    _errores.Add("No se han podido realizar todos los cálculos intermedios");

                fini = ffin.AddDays(1);
                ffin = fini.AddMonths((int)mesesmcd).AddDays(-1);
            }

            Entity _instalacion = null;
            decimal _facturacionestimadaoferta = 0, _beneficioestimadooferta = 0;
            if (facturacionEstimada)
            {
                Guid _instalacionid = Guid.Empty;
                if (_oferta.Attributes.Contains("atos_instalacionid") == true)
                    _instalacionid = ((EntityReference)_oferta.Attributes["atos_instalacionid"]).Id;
                else if (_ofpre.Attributes.Contains("atos_instalacionid") == true)
                    _instalacionid = ((EntityReference)_ofpre.Attributes["atos_instalacionid"]).Id;
                if (_instalacionid == Guid.Empty)
                {
                    _errores.Add("La oferta no tiene informado el campo de instalación");
                    return;
                }
                _instalacion = _service.Retrieve("atos_instalacion", _instalacionid, 
                                                 new ColumnSet(new String[] { "atos_consumoestimadoanual1", "atos_consumoestimadoanual2",
                                                                              "atos_consumoestimadoanual3", "atos_consumoestimadoanual4",
                                                                              "atos_consumoestimadoanual5", "atos_consumoestimadoanual6" }));
            }


            for (int i = 0; i < _terminos.Entities.Count; i++)
            {
                EntityCollection _pricingOutputs = (EntityCollection)_terminos.Entities[i].Attributes["pricingoutputs"];
                if ( _pricingOutputs.Entities.Count > 0 )
                {
                    _pricingOutPut = _pricingOutputs.Entities[0];
                    _pricingOutPut.Attributes["atos_fechainicioaplicacion"] = fechainicio;
                    _pricingOutPut.Attributes["atos_fechafinaplicacion"] = fechafin;
                    
                    for (int k = 1; k < _pricingOutputs.Entities.Count; k++)
                    {
                        for (int j = 1; j <= 6; j++)
                        {
                            if (_pricingOutputs.Entities[k].Attributes.Contains("atos_p" + j.ToString()))
                            {
                                //if (_terminos.Entities[i].Attributes["tipo"].ToString() == "TermPricing")
                                //{
                                Log.writelog("Acumulando Pricing Output " + _terminos.Entities[i].Attributes["nombre"].ToString());
                                Log.writelog("Periodo  " + j.ToString() + ": " + _pricingOutputs.Entities[k].Attributes["atos_p" + j.ToString()].ToString() + " acumulado: " + _pricingOutPut.Attributes["atos_p" + j.ToString()].ToString());
                                //}

                                if (_pricingOutPut.Attributes.Contains("atos_p" + j.ToString()))
                                    _pricingOutPut.Attributes["atos_p" + j.ToString()] = (decimal) _pricingOutPut.Attributes["atos_p" + j.ToString()] + (decimal)_pricingOutputs.Entities[k].Attributes["atos_p" + j.ToString()];
                                else
                                    _pricingOutPut.Attributes["atos_p" + j.ToString()] = (decimal)_pricingOutputs.Entities[k].Attributes["atos_p" + j.ToString()];
                            }
                        }
                    }
                    // Redondeamos
                    for (int j = 1; j <= 6; j++)
                    {
                        if (_pricingOutPut.Attributes.Contains("atos_p" + j.ToString()))
                        {
                            if (_terminos.Entities[i].Attributes["tipo"].ToString() == "TermPricing") // redondeamos a precisiondecimalesterminos
                                _pricingOutPut.Attributes["atos_p" + j.ToString()] = decimal.Round((decimal)_pricingOutPut.Attributes["atos_p" + j.ToString()], precisiondecimalesterminos, MidpointRounding.AwayFromZero);
                            else
                                _pricingOutPut.Attributes["atos_p" + j.ToString()] = decimal.Round((decimal)_pricingOutPut.Attributes["atos_p" + j.ToString()], precisiondecimales, MidpointRounding.AwayFromZero);
                        }
                    }
                    if (_terminos.Entities[i].Attributes["tipo"].ToString() == "TermPricing")
                    {
                        Log.writelog("Pricing Output " + _terminos.Entities[i].Attributes["nombre"].ToString());
                        for (int j = 1; j <= 6; j++)
                        {
                            Log.writelog("Periodo  " + j.ToString());
                            //Log.writelog("Periodo  " + j.ToString() + ": " + _pricingOutPut.Attributes["atos_p" + j.ToString()].ToString());
                        }
                    }
                    //decimal.Round(_vpi.Valores[0] * (decimal) horasMatriz.Attributes["ponderacionp" + k.ToString()], precisiondecimalesterminos)
                    _service.Create(_pricingOutPut);

                    if (facturacionEstimada == true && _terminos[i].Attributes["tipo"].ToString() == "TermPricing" && _pricingOutPut.Attributes["atos_terminoems"].ToString() == "MC")
                    {
                        for (int j = 1; j <= 6; j++)
                        {
                            if (_instalacion.Attributes.Contains("atos_consumoestimadoanual" + j.ToString()))
                            {
                                decimal _prorrateodias = (decimal)((fechafin - fechainicio).Days + 1) / (decimal)((fechainicio.AddYears(1) - fechainicio).Days);
                                Log.writelog("Dias oferta (beneficioestimado): " + ((decimal)(fechafin - fechainicio).Days + 1).ToString());
                                Log.writelog("Dias año (beneficioestimado): " + ((decimal)(fechainicio.AddYears(1) - fechainicio).Days).ToString());
                                Log.writelog("Prorrateo días (beneficioestimado): " + _prorrateodias.ToString());
                                decimal _consumo = (decimal)_instalacion.Attributes["atos_consumoestimadoanual" + j.ToString()] * _prorrateodias;
                                _beneficioestimadooferta += _consumo * (decimal)_pricingOutPut.Attributes["atos_p" + j.ToString()];
                            }
                        }
                    }
                    else if (facturacionEstimada == true && _terminos[i].Attributes["tipo"].ToString() == "Formula")
                    {
                        Entity _pricingConsumos = _pricingOutPut;
                        _pricingConsumos.Attributes["atos_terminoems"] = "C-" + _pricingOutPut.Attributes["atos_terminoems"].ToString();
                        _pricingConsumos.Attributes["atos_name"] = "Consumo-" + _pricingConsumos.Attributes["atos_name"].ToString();
                        _pricingConsumos.Attributes.Remove("atos_pricingoutputid");
                        _pricingConsumos.Id = Guid.Empty;
                        for (int j = 1; j <= 6; j++)
                        {
                            if (_instalacion.Attributes.Contains("atos_consumoestimadoanual" + j.ToString()))
                            {
                                decimal _prorrateodias = (decimal)((fechafin - fechainicio).Days + 1) / (decimal)((fechainicio.AddYears(1) - fechainicio).Days);
                                Log.writelog("Dias oferta (facturacion estimada): " + ((decimal)(fechafin - fechainicio).Days + 1).ToString());
                                Log.writelog("Dias año (facturacion estimada): " + ((decimal)(fechainicio.AddYears(1) - fechainicio).Days).ToString());
                                Log.writelog("Prorrateo días (facturacion estimada): " + _prorrateodias.ToString());

                                decimal _consumo = (decimal)_instalacion.Attributes["atos_consumoestimadoanual" + j.ToString()] * _prorrateodias;
                                _facturacionestimadaoferta += _consumo * (decimal)_pricingOutPut.Attributes["atos_p" + j.ToString()];
                                _pricingConsumos.Attributes["atos_p" + j.ToString()] = decimal.Round(_consumo, precisiondecimales, MidpointRounding.AwayFromZero);
                            }
                            else if (_pricingConsumos.Attributes.Contains("atos_p" + j.ToString()))
                                _pricingConsumos.Attributes.Remove("atos_p" + j.ToString());

                        }
                        Log.writelog("Antes de crear pricing output consumo estimado");
                        _service.Create(_pricingConsumos);
                    }
                }
            }
            if (facturacionEstimada)
            {
                _oferta.Attributes["atos_facturacionestimadaoferta"] = decimal.Round(_facturacionestimadaoferta, precisiondecimales, MidpointRounding.AwayFromZero);
                _oferta.Attributes["atos_beneficioestimadooferta"] = decimal.Round(_beneficioestimadooferta, precisiondecimales, MidpointRounding.AwayFromZero);
            }

        }

        static public Formula creaFormula(Entity _tipoProducto, bool tipoCalculoPromedio,
                                          CommonWS.Log _log, Boolean _facturacionEstimada = false)
        {
            if (_tipoProducto.Attributes.Contains("atos_formula") == false)
                return null;

            if (_tipoProducto.Attributes["atos_formula"].ToString() == "")
                return null;

            String _expresion = _tipoProducto.Attributes["atos_formula"].ToString();
            _log.writelog("creaFormula: " + _expresion, true);
            String _nbformula = "F";
            if (_tipoProducto.Attributes.Contains("atos_nombreems"))
                if (_tipoProducto.Attributes["atos_nombreems"].ToString() != "")
                    _nbformula = _tipoProducto.Attributes["atos_nombreems"].ToString();

            Formula formula = new Formula(_nbformula, _expresion, tipoCalculoPromedio);
            formula.setLog(_log);
            //formula.setTraza(tracingService);
            
            formula.facturacionEstimada = _facturacionEstimada;
            return formula;
        }

        public List<FormulaBase> construyeFormulas(Entity _tipoProducto, 
                                                   IOrganizationService OrganizationService,
                                                   bool tipoCalculoPromedio)
        {
            List<FormulaBase> variables = new List<FormulaBase>();
            for (int i = 1; i < 11; i++)
            {
                String _camponbvi = "atos_nombrevi" + i;
                String _campovavi = "atos_valorvi" + i;

                if (_tipoProducto.Attributes.Contains(_camponbvi) && _tipoProducto.Attributes.Contains(_campovavi))
                {
                    Log.writelog(i.ToString() + ".- Añadiendo a la lista de variables de la fórmula: " + _tipoProducto.Attributes[_camponbvi].ToString() + " = " + _tipoProducto.Attributes[_campovavi].ToString(), true);
                    Variable _v = new Variable(_tipoProducto.Attributes[_camponbvi].ToString(), _tipoProducto.Attributes[_campovavi].ToString(), tipoCalculoPromedio);
                    _v.FacturacionEstimada = this.facturacionEstimada;
                    _v.esInstalacionGas = this.esGas; // Pricing Gas
                    variables.Add(_v);
                }
            }

            for (int i = 0; i < variables.Count; i++)
            {
                //variables[i].setTraza(TracingService);
                //variables[i].setLog(this.log, this.ficherolog);
                variables[i].setLog(this.Log);
                variables[i].expandeExpresion(ref variables, OrganizationService);
            }
            this.expandeExpresion(ref variables, OrganizationService);
            return variables;
        }
    }


    public class Variable : FormulaBase
    {

        private bool[] calculado;
        private bool expandida = false;

        public Variable(String _nombre, String _expresion, bool _tipoCalculoPromedio = false)
            : base(_nombre, _expresion, _tipoCalculoPromedio)
        {
            base.tipo = "Variable";
            calculado = new bool[7];
            expandida = false;
            for (int i = 0; i < 7; i++)
                calculado[i] = false;
        }

        override public String expandeExpresion(ref List<FormulaBase> _variables, IOrganizationService _service)
        {
            Log.writelog("Variable " + nombre + ". Expresion = " + expresion, true);
            if (expandida)
                return expresionfinal;
            expresionfinal = expresion.Replace("(","( ").Replace(")"," )");
            Log.writelog("Variable " + nombre + ". expresionfinal = " + expresionfinal, true);
            String[] partes = base.expresion.Replace("(", "").Replace(")", "").Split(' ');
            for (int i = 0; i < partes.Length; i++)
            {
                if (partes[i] != "")
                {
                    int _componente = TipoComponente(partes[i]);
                    if (_componente == -3)
                    {
                        bool encontrado = false;
                        for (int j = 0; j < _variables.Count; j++)
                        {
                            if ( validar && partes[i] == _variables[j].NombreFormula && partes[i] == nombre )
                            {
                                erroresValidacion.Add("Variable " + nombre + "= " + expresion + " tiene un componente con el mismo nombre.");
                            }
                            // si el término a buscar coincide con el nombre de la variable que está evaluando entonces debe ser un TermPricing del mismo nombre
                            if (partes[i] == _variables[j].NombreFormula && (!validar || (validar && partes[i] != nombre)) )
                            //if (partes[i] == _variables[j].NombreFormula ) //&& partes[i] != nombre) 
                            {
                                encontrado = true;
                                Log.writelog("Variable: " + nombre + " expandeExpresion Variable: " + partes[i], true);
                                base.componentes.Add(new Componente(partes[i], "Variable"));
                                break;
                            }
                        }
                        if (encontrado == false)
                        {
                            Log.writelog("Variable: " + nombre + " expandeExpresion Termino de Pricing: " + partes[i]);
                            TermPricing _tp = new TermPricing(partes[i], _service, Log);
                            base.componentes.Add(_tp);
                        }

                    }
                }
            }

            for (int i = 0; i < base.componentes.Count; i++)
            {
                if (base.componentes[i].TipoComponente == "Variable")
                {
                    int j;
                    for (j = 0; j < _variables.Count; j++)
                    {
                        if (_variables[j].NombreFormula == base.componentes[i].NombreComponente)
                        {
                            Log.writelog("Variable: " + nombre + ". Hay que expandir la variable " + _variables[j].NombreFormula);
                            String _valorvi = ((Variable)_variables[j]).expandeExpresion(ref _variables, _service);
                            expresionfinal = expresionfinal.Replace(base.componentes[i].NombreComponente, "( " + _valorvi + " )");
                            Log.writelog("Variable: " + nombre + ". expresionfinal = " + expresionfinal);
                            Log.writelog("Variable: " + nombre + ". Componentes número: " + _variables[j].ComponentesFormula.Count.ToString());
                            // Se añaden los TermPricing de la variable que no estén en la fórmula original
                            for (int k = 0; k < _variables[j].ComponentesFormula.Count; k++)
                            {
                                Log.writelog("Variable: " + nombre + ". Componente: " + k.ToString());
                                Log.writelog("Variable: " + nombre + ". Componente: " + _variables[j].ComponentesFormula[k].NombreComponente);
                                int _componente = TipoComponente(_variables[j].ComponentesFormula[k].NombreComponente);
                                if (_componente == -3)
                                {
                                    Log.writelog("Variable: " + nombre + " de variable " + _variables[j].NombreFormula + " expandeExpresion " + _variables[j].ComponentesFormula[k].NombreComponente);

                                    TermPricing _tp = new TermPricing(_variables[j].ComponentesFormula[k].NombreComponente, _service, Log);
                                    base.componentes.Add(_tp);
                                }
                            }
                        }
                    }

                }
            }
            //Console.WriteLine("Variable " + base.nombre + " expresión: " + base.expresion + " expandida: " + base.expresionfinal);
            expandida = true;

            Log.writelog("Variable " + nombre + ". expresionfinal = " + expresionfinal + " - FIN", true);
            return expresionfinal;
        }


        private bool calculaperiodo(ref List<Componente> _componentes, ref List<FormulaBase> _variables, int _indcomponente, ref String _formula,
                                   int _periodo, IOrganizationService _service, Entity _tipoProducto, Entity _tarifa, ref List<String> _errores)
        {
            bool _ret = true;
            Log.writelog("calculaperiodo ");
            for (int i = 0; i < _componentes.Count; i++)
            {
                if (_componentes[i].TipoComponente == "TermPricing")
                {
                    if (((TermPricing)_componentes[i]).FijoIndexado == null)
                        Log.writelog("Variable: " + nombre + " calculaperiodo: " + ((TermPricing)_componentes[i]).NombreComponente + " FijoIndexado a null");

                    if (((TermPricing)_componentes[i]).FijoIndexado.Value == 300000000) // Solo se calcula si es de tipo fijo
                    {
                        int _p = _periodo;
                        if (_periodo == 0 && _componentes[i].NumeroPeriodos != 0)
                            throw new Exception("El termino de pricing " + _componentes[i].NombreComponente + " no tiene precio fijo");
                        if (_periodo > 0 && _componentes[i].NumeroPeriodos != 0 && _componentes[i].NumeroPeriodos < _periodo)
                            throw new Exception("El termino de pricing " + _componentes[i].NombreComponente + " no tiene precio para el periodo " + _periodo.ToString());
                        if (_periodo > 0 && _componentes[i].NumeroPeriodos == 0)
                            _p = 0;
                        _formula = _formula.Replace(" " + _componentes[i].NombreComponente + " "," " + _componentes[i].Valores[_p].ToString() + " ");
                    }
                    else
                        fijoindexado = new OptionSetValue(((TermPricing)_componentes[i]).FijoIndexado.Value);
                }
            }
            for (int i = 0; i < _componentes.Count; i++)
            {
                if (_componentes[i].TipoComponente == "Variable")
                {
                    if (TipoComponente(_componentes[i].NombreComponente) >= 0)
                    {
                        for (int j = 0; j < _variables.Count; j++)
                        {
                            if (_variables[j].NombreFormula == _componentes[i].NombreComponente && _variables[j].NombreFormula != nombre)
                            {
                                if (((Variable)_variables[j]).calculado[_periodo] == false)
                                {
                                    if (((Variable)_variables[j]).calcula(ref _componentes, ref _variables, i, _service, _tipoProducto, _tarifa, ref _errores) == true)
                                    {

                                        if (((Variable)_variables[j]).FijoOIndexado == null)
                                            Log.writelog("Variable: " + nombre + " calculaperiodo (variable): " + ((Variable)_variables[j]).NombreFormula + " FijoIndexado a null");

                                        if (((Variable)_variables[j]).FijoOIndexado.Value == 300000000) // Solo se calcula si es fijo (no indexado)
                                        {
                                            this.Log.writelog("Variable: " + this.NombreFormula + ". Componente: " + _componentes[i].NombreComponente);
                                            _formula = _formula.Replace(" " + _componentes[i].NombreComponente + " ", " " + _componentes[i].Valores[_periodo].ToString() + " ");
                                        }
                                        else
                                            fijoindexado = new OptionSetValue(((Variable)_variables[j]).FijoOIndexado.Value);
                                    }
                                    else
                                        _ret = false;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            return _ret;
        }

        public bool calcula(ref List<Componente> _componentes, ref List<FormulaBase> _variables, int _indcomponente, IOrganizationService _service,
                            Entity _tipoProducto, Entity _tarifa, ref List<String> _errores)
        {
            bool _ret = true;
            if (_tarifa.Attributes.Contains("atos_numeroperiodos") == true)
                periodo = (Decimal)_tarifa.Attributes["atos_numeroperiodos"];
            if (periodo < 0)
            {
                _errores.Add("La tarifa " + _tarifa.Attributes["atos_name"].ToString() + " no tiene periodo");
                return false;
            }

            String[] _formula = new String[7];
            for (int i = 0; i < 7; i++)
                _formula[i] = " " + expresion.Replace("(", " ( ").Replace(")", " ) ").Replace("+", " + ").Replace("-", " - ").Replace("/", " / ").Replace("*", " * ") + " ";
                //_formula[i] = expresion;
            //String _formula = expresion;
            Log.writelog("Función calcula de variable " + nombre + " periodos " + periodo.ToString());
            if (periodo == 0)
            {
                if (calculaperiodo(ref _componentes, ref _variables, _indcomponente, ref _formula[0], 0, _service, _tipoProducto, _tarifa, ref _errores) == true)
                {
                    if (fijoindexado == null)
                        Log.writelog("Variable: " + nombre + " calcula fijoindexado a nulo");
                    if (fijoindexado.Value == 300000000)
                    {
                        this.Log.writelog("evaluando variable _formula[0]: [" + _formula[0] + "]");
                        _componentes[_indcomponente].Valores[0] = decimal.Round(FormulaBase.Evaluate(_formula[0]), precisiondecimales, MidpointRounding.AwayFromZero);
                    }
                }
                else
                    _ret = false;
            }
            else
            {
                for (int i = 1; i <= periodo; i++)
                {

                    if (calculaperiodo(ref _componentes, ref _variables, _indcomponente, ref _formula[i], i, _service, _tipoProducto, _tarifa, ref _errores) == true)
                    {
                        if (fijoindexado == null)
                            Log.writelog("Variable: " + nombre + " calcula fijoindexado a nulo");
                        if (fijoindexado.Value == 300000000)
                        {
                            this.Log.writelog("evaluando variable _formula[" + i.ToString() + "]: [" + _formula[i] + "]");
                            _componentes[_indcomponente].Valores[i] = decimal.Round(FormulaBase.Evaluate(_formula[i]), precisiondecimales, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                        _ret = false;
                }
            }
            return _ret;
        }




        private bool calculaperiodoPorTramos(ref List<Componente> _componentes, ref List<FormulaBase> _variables, int _indcomponente, ref String _formula,
                                   int _periodo, IOrganizationService _service, Entity _tipoProducto, Entity _tarifa, ref List<String> _errores,
                                   DateTime _fechainicio, DateTime _fechafin, decimal _numeroMeses)
        {
            bool _ret = true;
            Log.writelog("calculaperiodoPorTramos " + _componentes.Count.ToString() + " componentes");
            for (int i = 0; i < _componentes.Count; i++)
            {
                if (_componentes[i].TipoComponente == "TermPricing")
                {
                    Log.writelog("TermPricing: " + _componentes[i].NombreComponente);
                    if (((TermPricing)_componentes[i]).FijoIndexado == null)
                        Log.writelog("Variable: " + nombre + " calculaperiodo: " + ((TermPricing)_componentes[i]).NombreComponente + " FijoIndexado a null");

                    if (((TermPricing)_componentes[i]).FijoIndexado.Value == 300000000) // Solo se calcula si es de tipo fijo
                    {
                        int _p = _periodo;
                        Log.writelog("El indice es " + _p.ToString() + " el componente tiene " + _componentes[i].NumeroPeriodos.ToString() + " periodos");

                        if (_periodo == 0 && _componentes[i].NumeroPeriodos != 0)
                            throw new Exception("El termino de pricing " + _componentes[i].NombreComponente + " no tiene precio fijo");
                        if (_periodo > 0 && _componentes[i].NumeroPeriodos != 0 && _componentes[i].NumeroPeriodos < _periodo)
                            throw new Exception("El termino de pricing " + _componentes[i].NombreComponente + " no tiene precio para el periodo " + _periodo.ToString());
                        //if (_periodo > 0 && _componentes[i].NumeroPeriodos == 0)
                        //    _p = 0;
                        ((TermPricing)_componentes[i]).setLog(Log);
                        ValorPricingInput _valorPricingInput = ((TermPricing)_componentes[i]).valorPricingInput(_fechainicio, _fechafin);
                        if (_valorPricingInput.NumeroPeriodos == 0)
                            _p = 0;
                        Log.writelog("Indice " + _p.ToString() + ". Lo busca en un array de " + _valorPricingInput.Valores.Length.ToString() + " elementos"); //.ValorMes.Length.ToString() + " elementos");
                        Log.writelog("PricingInput tiene " + _valorPricingInput.NumeroPeriodos.ToString());
                        // No hay que prorratear nunca los importes
                        //if (_valorPricingInput.Porcentaje == true) // Si es porcentaje no prorratea el valor
                        Log.writelog("Replace " + _componentes[i].NombreComponente + " por " + _valorPricingInput.Valores[_p].ToString() + ".");
                        _formula = _formula.Replace(" " + _componentes[i].NombreComponente + " ", " " + _valorPricingInput.Valores[_p].ToString() + " "); // .ValorMes[_p].ToString());
                        //else
                        //    _formula = _formula.Replace(_componentes[i].NombreComponente, (_valorPricingInput.ValorMes[_p] * _numeroMeses).ToString());
                        Log.writelog("Formula (variable " + nombre + "): " + _formula);
                    }
                    else if (TipoComponente(_componentes[i].NombreComponente) >= 0)
                        fijoindexado = new OptionSetValue(((TermPricing)_componentes[i]).FijoIndexado.Value);
                }
            }
            Log.writelog("calculaperiodoPorTramos variable " + nombre);
            for (int i = 0; i < _componentes.Count; i++)
            {
                if (_componentes[i].TipoComponente == "Variable")
                {
                    if (TipoComponente(_componentes[i].NombreComponente) >= 0)
                    {
                        for (int j = 0; j < _variables.Count; j++)
                        {
                            if (_variables[j].NombreFormula == _componentes[i].NombreComponente && _variables[j].NombreFormula != nombre)
                            {

                                if (((Variable)_variables[j]).calculado[_periodo] == false)
                                    _ret = ((Variable)_variables[j]).calculaPorTramos(ref _componentes, ref _variables, i, _service, _tipoProducto, _tarifa, ref _errores, _fechainicio, _fechafin, _numeroMeses);
                                else
                                    Log.writelog("Variable " + nombre + " Periodo " + _periodo + " ya calculado");

                                if (_ret)
                                {
                                    if (((Variable)_variables[j]).FijoOIndexado == null)
                                        Log.writelog("Variable: " + nombre + " calculaperiodo (variable): " + ((Variable)_variables[j]).NombreFormula + " FijoIndexado a null");

                                    if (((Variable)_variables[j]).FijoOIndexado.Value == 300000000) // Solo se calcula si es fijo (no indexado)
                                    {
                                        //ValorPricingInput _valorPricingInput = ((TermPricing)componentes[i]).valorPricingInput(_fechainicio, _fechafin);
                                        this.Log.writelog("Variable: " + this.NombreFormula + ". Componente: " + _componentes[i].NombreComponente);
                                        _formula = _formula.Replace(" " + _componentes[i].NombreComponente + " ", " " + _componentes[i].Valores[_periodo].ToString() + " ");
                                    }
                                    else
                                        fijoindexado = new OptionSetValue(((Variable)_variables[j]).FijoOIndexado.Value);
                                }
                                /*
                                if (((Variable)_variables[j]).calculado[_periodo] == false)
                                {
                                    if (((Variable)_variables[j]).calculaPorTramos(ref _componentes, ref _variables, i, _service, _tipoProducto, _tarifa, ref _errores, _fechainicio, _fechafin, _numeroMeses) == true)
                                    {

                                        if (((Variable)_variables[j]).FijoOIndexado == null)
                                            Log.writelog("Variable: " + nombre + " calculaperiodo (variable): " + ((Variable)_variables[j]).NombreFormula + " FijoIndexado a null");

                                        if (((Variable)_variables[j]).FijoOIndexado.Value == 300000000) // Solo se calcula si es fijo (no indexado)
                                        {
                                            ValorPricingInput _valorPricingInput = ((TermPricing)componentes[i]).valorPricingInput(_fechainicio, _fechafin);
                                            this.Log.writelog("Variable: " + this.NombreFormula + ". Componente: " + _componentes[i].NombreComponente);
                                            _formula = _formula.Replace(_componentes[i].NombreComponente, _componentes[i].Valores[_periodo].ToString());
                                        }
                                        else
                                            fijoindexado = new OptionSetValue(((Variable)_variables[j]).FijoOIndexado.Value);
                                    }
                                    else
                                        _ret = false;
                                }
                                else
                                {
                                    Log.writelog("Variable " + nombre + " Periodo " + _periodo + " ya calculado");
                                }*/
                                break;
                            }
                        }
                    }
                }
            }
            if (_ret)
                calculado[_periodo] = true;
            return _ret;
        }

        public bool calculaPorTramos(ref List<Componente> _componentes, ref List<FormulaBase> _variables, int _indcomponente, IOrganizationService _service,
                            Entity _tipoProducto, Entity _tarifa, ref List<String> _errores, DateTime _fechainicio, DateTime _fechafin, decimal _numeroMeses)
        {
            bool _ret = true;
            if (_tarifa.Attributes.Contains("atos_numeroperiodos") == true)
                periodo = (Decimal)_tarifa.Attributes["atos_numeroperiodos"];
            if (periodo < 0)
            {
                _errores.Add("La tarifa " + _tarifa.Attributes["atos_name"].ToString() + " no tiene periodo");
                return false;
            }

            String[] _formula = new String[7];
            for (int i = 0; i < 7; i++)
                _formula[i] = " " + expresion.Replace("(", " ( ").Replace(")", " ) ").Replace("+", " + ").Replace("-", " - ").Replace("/", " / ").Replace("*", " * ") + " ";
                //_formula[i] = expresion;
            //String _formula = expresion;
            Log.writelog("Función calculaPorTramos de variable " + nombre + " periodos " + periodo.ToString());
            if (periodo == 0)
            {
                if (calculaperiodoPorTramos(ref _componentes, ref _variables, _indcomponente, ref _formula[0], 0, _service, _tipoProducto, _tarifa, ref _errores, _fechainicio, _fechafin, _numeroMeses) == true)
                {
                    if (fijoindexado == null)
                        Log.writelog("Variable: " + nombre + " calcula fijoindexado a nulo");
                    if (fijoindexado.Value == 300000000)
                    {
                        this.Log.writelog("evaluando variable _formula[0]: [" + _formula[0] + "] decimales: " + precisiondecimales.ToString());
                        _componentes[_indcomponente].Valores[0] = decimal.Round(FormulaBase.Evaluate(_formula[0]), precisiondecimales, MidpointRounding.AwayFromZero);
                    }
                }
                else
                    _ret = false;
            }
            else
            {
                for (int i = 1; i <= periodo; i++)
                {

                    if (calculaperiodoPorTramos(ref _componentes, ref _variables, _indcomponente, ref _formula[i], i, _service, _tipoProducto, _tarifa, ref _errores, _fechainicio, _fechafin, _numeroMeses) == true)
                    {
                        if (fijoindexado == null)
                            Log.writelog("Variable: " + nombre + " calcula fijoindexado a nulo");
                        if (fijoindexado.Value == 300000000)
                        {
                            this.Log.writelog("evaluando variable _formula[" + i.ToString() + "]: [" + _formula[i] + "] decimales: " + precisiondecimales.ToString());
                            decimal _valor = FormulaBase.Evaluate(_formula[i]);
                            this.Log.writelog("Valor devuelto: " + _valor.ToString() + " Valor redondeado: " + decimal.Round(_valor, precisiondecimales, MidpointRounding.AwayFromZero).ToString() + " Redondedo con Math: " + Math.Round(_valor, precisiondecimales, MidpointRounding.AwayFromZero).ToString());
                            _componentes[_indcomponente].Valores[i] = decimal.Round(_valor, precisiondecimales, MidpointRounding.AwayFromZero);
                            //_componentes[_indcomponente].Valores[i] = decimal.Round(FormulaBase.Evaluate(_formula[i]), precisiondecimales);
                        }
                    }
                    else
                        _ret = false;
                }
            }
            return _ret;
        }
    }

}
