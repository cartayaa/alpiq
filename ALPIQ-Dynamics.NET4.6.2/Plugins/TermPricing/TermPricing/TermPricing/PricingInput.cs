using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TermPricing
{
    public class PricingInput
    {

        IOrganizationService _service;
        /*ITracingService TracingService;
        bool log = false;
        String ficherolog; */
        Boolean facturacionEstimada = false; 

        CommonWS.Log Log = null;

        public PricingInput(IOrganizationService _s, CommonWS.Log _log)
        {
            _service = _s;
            Log = new CommonWS.Log(_log);
            
            facturacionEstimada = false;
        }

        public PricingInput(IOrganizationService _s, ITracingService _t)
        {
            Log = new CommonWS.Log();
            Log.tracingService = _t;

            _service = _s;
            /*TracingService = _t;
            log = _log;
            ficherolog = _flog;*/
            facturacionEstimada = false;
        }

        public void setLog(CommonWS.Log _log)
        {
            if (Log == null)
                Log = new CommonWS.Log(_log);
            else
                Log.setLog(_log);
        }

        public Boolean FacturacionEstimada
        {
            get { return this.facturacionEstimada; }
            set { facturacionEstimada = value; }
        }

        
        /*private void traza(String message, bool _traza = false)
        {
            Log.writelog(message, _traza);*/
            /*if (log == true)
                System.IO.File.AppendAllText(ficherolog, message + "\r\n");
            if (_traza && TracingService != null)
                this.TracingService.Trace(message);*/

        /*}*/

        private Entity nuevo(DateTime _fechainicio, DateTime _fechafin, DateTime _createdon,
                             ref List<Componente> componentes, int indicecomponente, Guid _tarifaid, Guid _sistemaelectricoid, Guid _subSistemaelectricoid, Guid _peajeid,
                             Entity _oferta, Entity _ofpre)
        {
            Entity _pricingInput = new Entity("atos_pricinginput");

            // Error en fechas
            _pricingInput.Attributes["atos_fechainicioaplicacion"] = _fechainicio.ToUniversalTime();
            _pricingInput.Attributes["atos_fechafinaplicacion"] = _fechafin.ToUniversalTime();
            _pricingInput.Attributes["atos_terminodepricingid"] = new EntityReference("atos_terminodepricing", ((TermPricing)componentes[indicecomponente]).TermpricingId);
            _pricingInput.Attributes["atos_fechainiciovigencia"] = _createdon.ToUniversalTime();
            _pricingInput.Attributes["atos_facturacionestimada"] = facturacionEstimada;
            _pricingInput.Attributes["atos_fechafinvigencia"] = new DateTime(4000, 1, 1); // Se pone fin de vigencia del pricing input al 1/01/4000 para los contratos a pasado y siempre que no encuentre pricing input base. _pricingInput.Attributes["atos_fechafinaplicacion"];

            if (((TermPricing)componentes[indicecomponente]).DependeDeTarifa == true)
                _pricingInput.Attributes["atos_tarifaid"] = new EntityReference("atos_tarifa", _tarifaid);

            _pricingInput.Attributes["atos_tipo"] = new OptionSetValue(((TermPricing)componentes[indicecomponente]).FijoIndexado.Value); ;

            if (((TermPricing)componentes[indicecomponente]).DependeDeSistemaElectrico == true)
                _pricingInput.Attributes["atos_sistemaelectricoid"] = new EntityReference("atos_sistemaelectrico", _sistemaelectricoid);

            if (((TermPricing)componentes[indicecomponente]).DependeDePeaje == true)
                _pricingInput.Attributes["atos_peajeid"] = new EntityReference("atos_tablasatrgas", _peajeid);
            //_pricingInput.Attributes["atos_peajeid"] = new EntityReference("atos_peajeid", _peajeid);

            if (((TermPricing)componentes[indicecomponente]).Calculodinamico == true)
            {
                _pricingInput.Attributes["atos_porcentajeoimporte"] = new OptionSetValue(300000001);
                _pricingInput.Attributes["atos_pfijo"] = ((TermPricing)componentes[indicecomponente]).valorDinamico(_oferta, _ofpre, _service);
            }
            return _pricingInput;
        }


        private EntityCollection construye(EntityCollection encontrados, 
                                        DateTime _fechainicio, DateTime _fechafin,
                                        ref List<Componente> componentes, int indicecomponente, 
                                        Entity _oferta, Entity _ofpre)
        {
            DateTime _createdon;
            Guid _tarifaid = Guid.Empty;
            Guid _sistemaelectricoid = Guid.Empty;
            Guid _subsistemaelectricoid = Guid.Empty;
            Guid _peajeid = Guid.Empty;
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

            if (((TermPricing)componentes[indicecomponente]).DependeDeSubSistemaElectrico == true)
            {

                if (_oferta.Attributes.Contains("atos_subsistemaid"))
                    _subsistemaelectricoid = ((EntityReference)_oferta.Attributes["atos_subsistemaid"]).Id;
                else
                    _subsistemaelectricoid = ((EntityReference)_ofpre.Attributes["atos_subsistemaid"]).Id;
            }
            if (((TermPricing)componentes[indicecomponente]).DependeDePeaje == true)
            {

                if (_oferta.Attributes.Contains("atos_peajeid"))
                    _peajeid = ((EntityReference)_oferta.Attributes["atos_peajeid"]).Id;
                else
                    _peajeid = ((EntityReference)_ofpre.Attributes["atos_peajeid"]).Id;
            }
            if (_oferta.Attributes.Contains("createdon"))
                _createdon = (DateTime)_oferta.Attributes["createdon"];
            else //if (_ofpre.Attributes.Contains("createdon"))
                _createdon = (DateTime)_ofpre.Attributes["createdon"];

            Log.writelog("En construye. Entities.Count: " + encontrados.Entities.Count.ToString() + ". createdon " + _createdon.ToLongDateString() + " " + _createdon.ToLongTimeString(), true);
            Log.writelog("En construye. Entities.Count: " + encontrados.Entities.Count.ToString() + ". createdon.ToUniversalTime() " + _createdon.ToUniversalTime().ToLongDateString() + " " + _createdon.ToUniversalTime().ToLongTimeString(), true);


            if (encontrados.Entities.Count > 0)
            {
                if ((DateTime)encontrados.Entities[0].Attributes["atos_fechainicioaplicacion"] > _fechainicio)
                {
                    _pricingInputs.Entities.Add(nuevo(_fechainicio, ((DateTime)encontrados.Entities[0].Attributes["atos_fechainicioaplicacion"]).AddDays(-1), _createdon,
                                        ref componentes, indicecomponente, _tarifaid, _sistemaelectricoid, _subsistemaelectricoid, _peajeid,
                                        _oferta, _ofpre));
                }

                for (int i = 0; i < encontrados.Entities.Count; i++)
                {
                    Entity _pricingInput = encontrados.Entities[i];
                    _pricingInput.Attributes["atos_facturacionestimada"] = facturacionEstimada;
                    _pricingInputs.Entities.Add(_pricingInput);
                }

                if ((DateTime)encontrados.Entities[encontrados.Entities.Count - 1].Attributes["atos_fechafinaplicacion"] < _fechafin)
                {
                    _pricingInputs.Entities.Add(nuevo(((DateTime)encontrados.Entities[encontrados.Entities.Count - 1].Attributes["atos_fechafinaplicacion"]).AddDays(1), _fechafin, _createdon,
                                        ref componentes, indicecomponente, _tarifaid, _sistemaelectricoid, _subsistemaelectricoid, _peajeid,
                                        _oferta, _ofpre));
                }
            }
            else
            {
                _pricingInputs.Entities.Add(nuevo(_fechainicio, _fechafin, _createdon, ref componentes, indicecomponente, _tarifaid, _sistemaelectricoid, _subsistemaelectricoid, _peajeid,
                                                  _oferta, _ofpre));
            }


            return _pricingInputs;
        }

        public EntityCollection coleccion(ref List<Componente> componentes, int i, Entity _oferta, Entity _ofpre, bool _ofertaid = false)
        {

            DateTime _fechaInicioOferta, _fechaFinOferta;

            Boolean _traza = false;
            //if (((TermPricing)componentes[i]).NombreComponente == "CG" )
            { _traza = true;  }
            Log.writelog("Función coleccion. Componente: " + ((TermPricing)componentes[i]).NombreComponente, _traza);
            Log.writelog("Función coleccion. ofertaid: " + _ofertaid.ToString(), _traza);

            
            if (_oferta.Attributes.Contains("atos_fechainicio"))
                _fechaInicioOferta = (DateTime)_oferta.Attributes["atos_fechainicio"];
            else
                _fechaInicioOferta = (DateTime)_ofpre.Attributes["atos_fechainicio"];

            Log.writelog("Fecha inicio oferta " + _fechaInicioOferta.ToLongDateString() + " " + _fechaInicioOferta.ToLongTimeString(), _traza);

            if (_oferta.Attributes.Contains("atos_fechafin"))
                _fechaFinOferta = (DateTime)_oferta.Attributes["atos_fechafin"];
            else
                _fechaFinOferta = (DateTime)_ofpre.Attributes["atos_fechafin"];

            Log.writelog("Fecha fin  oferta " + _fechaFinOferta.ToLongDateString() + " " + _fechaFinOferta.ToLongTimeString(), _traza);

            DateTime fechaInicioOfertaUTC = DateTime.SpecifyKind(_fechaInicioOferta, DateTimeKind.Utc);
            DateTime fechaFinOfertaUTC = DateTime.SpecifyKind(_fechaFinOferta, DateTimeKind.Utc);

            Log.writelog("------------------------", _traza);
            Log.writelog("Fecha inicio oferta UTC " + fechaInicioOfertaUTC.ToLongDateString() + " " + fechaInicioOfertaUTC.ToLongTimeString(), _traza);
            Log.writelog("Fecha fin  oferta UTC " + fechaFinOfertaUTC.ToLongDateString() + " " + fechaFinOfertaUTC.ToLongTimeString(), _traza);
            Log.writelog("------------------------", _traza);


            Log.writelog("1 pricingInputCollection " + _fechaInicioOferta.ToLocalTime().ToShortDateString() + " - " + _fechaFinOferta.ToLocalTime().ToShortDateString(), _traza);

            QueryExpression _consulta = new QueryExpression("atos_pricinginput");
            Log.writelog("2 pricingInputCollection");
            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_terminodepricingid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(((TermPricing)componentes[i]).TermpricingId.ToString());
            _filtro.Conditions.Add(_condicion);
            Log.writelog("3 pricingInputCollection " + ((TermPricing)componentes[i]).TermpricingId.ToString());

            if (facturacionEstimada && _ofertaid == true) // 
            {
                _condicion = new ConditionExpression();
                _condicion.AttributeName = "atos_facturacionestimada";
                _condicion.Operator = ConditionOperator.Equal;
                _condicion.Values.Add(facturacionEstimada);
                _filtro.Conditions.Add(_condicion);
                Log.writelog("3.1 pricingInputCollection Facturacion Estimada true ");
            }
            else
            {
                _condicion = new ConditionExpression();
                _condicion.AttributeName = "atos_facturacionestimada";
                _condicion.Operator = ConditionOperator.NotEqual;
                _condicion.Values.Add(true);
                _filtro.Conditions.Add(_condicion);
                Log.writelog("3.2 pricingInputCollection Facturacion Estimada no true ");
            }
            
            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_tarifaid";
            if (((TermPricing)componentes[i]).DependeDeTarifa == true)
            {
                Log.writelog("4.0 pricingInputCollection. Depende de Tarifa");
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
                Log.writelog("5.0 pricingInputCollection. Depende de Sistema Eléctrico");
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

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_subsistemaid";
            if (((TermPricing)componentes[i]).DependeDeSubSistemaElectrico == true)
            {
                Log.writelog("5.1 pricingInputCollection. Depende de Subsistema Eléctrico");
                _condicion.Operator = ConditionOperator.Equal;
                if (_oferta.Attributes.Contains("atos_subsistemaid"))
                    _condicion.Values.Add(((EntityReference)_oferta.Attributes["atos_subsistemaid"]).Id.ToString());
                else
                    _condicion.Values.Add(((EntityReference)_ofpre.Attributes["atos_subsistemaid"]).Id.ToString());
            }
            else
                _condicion.Operator = ConditionOperator.Null;

            Log.writelog("5.1 pricingInputCollection");
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_peajeid";
            if (((TermPricing)componentes[i]).DependeDePeaje == true)
            {
                Log.writelog("5.1 atos_peajeid. Depende de Peaje");
                _condicion.Operator = ConditionOperator.Equal;
                if (_oferta.Attributes.Contains("atos_peajeid"))
                {
                    _condicion.Values.Add(((EntityReference)_oferta.Attributes["atos_peajeid"]).Id.ToString());
                    Log.writelog("5.1.1 atos_peajeid. " + ((EntityReference)_oferta.Attributes["atos_peajeid"]).Id.ToString());
                }
                else
                {
                    _condicion.Values.Add(((EntityReference)_ofpre.Attributes["atos_peajeid"]).Id.ToString());
                    Log.writelog("5.1.2 atos_peajeid. " + ((EntityReference)_ofpre.Attributes["atos_peajeid"]).Id.ToString());
                }
            }else
            {
                _condicion.Operator = ConditionOperator.Null;
            }

            Log.writelog("5.2 atos_peajeid");
            _filtro.Conditions.Add(_condicion);

            DateTime Ahora = DateTime.Now;
            DateTime utcNow = DateTime.UtcNow;
            //DateTime NowLocal = DateTimeSpan.DateTimeLocal(utcNow, _service);
            Log.writelog("6 pricingInputCollection. atos_fechainiciovigencia <= " + Ahora.ToLongDateString() + " " + Ahora.ToLongTimeString(), _traza);
            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_fechainiciovigencia";
            _condicion.Operator = ConditionOperator.LessEqual;
            _condicion.Values.Add(Ahora);
            _filtro.Conditions.Add(_condicion);

            Log.writelog("6.1 pricingInputCollection. UtcNow: " + utcNow.ToLongDateString() + " " + utcNow.ToLongTimeString(), _traza);
            //Log.writelog("6.2 pricingInputCollection. NowLocal: " + NowLocal.ToLongDateString() + " " + NowLocal.ToLongTimeString(), true);

            Log.writelog("7 pricingInputCollection. atos_fechafinvigencia >= " + Ahora.ToLongDateString() + " " + Ahora.ToLongTimeString(), _traza);
            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_fechafinvigencia";
            _condicion.Operator = ConditionOperator.GreaterEqual;
            _condicion.Values.Add(Ahora);
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

                _condicion = new ConditionExpression(); // Excluimos pricing input del cierre de la oferta
                _condicion.AttributeName = "atos_cierreofertaid";
                _condicion.Operator = ConditionOperator.Null;
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

                _filtro.Conditions.Add(_condicion);

                Log.writelog("12 pricingInputCollection");
                _condicion = new ConditionExpression();
                _condicion.AttributeName = "atos_fechafinaplicacion";
                _condicion.Operator = ConditionOperator.GreaterEqual;
                _condicion.Values.Add(_fechaInicioOferta);

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

            Log.writelog("16 pricingInputCollection: " + _resultado.Entities.Count.ToString() , _traza);
            // Error de zona en fechas. No toma la zona local del usuario en el Online.
            // Se calcula en DateTimeSpan la zona del usuario y se suma el offset
            /*if (_oferta.Attributes.Contains("atos_fechainicio"))
                _fechaInicioOferta = DateTimeSpan.DateTimeLocal((DateTime)_oferta.Attributes["atos_fechainicio"], _service);
            else
                _fechaInicioOferta = DateTimeSpan.DateTimeLocal((DateTime)_ofpre.Attributes["atos_fechainicio"], _service);

            //Log.writelog("Antes de 17 pric. Fecha inicio oferta " + _fechaInicioOferta.ToLongDateString() + " " + _fechaInicioOferta.ToLongTimeString(), true);

            if (_oferta.Attributes.Contains("atos_fechafin"))
                _fechaFinOferta = DateTimeSpan.DateTimeLocal((DateTime)_oferta.Attributes["atos_fechafin"], _service);
            else
                _fechaFinOferta = DateTimeSpan.DateTimeLocal((DateTime)_ofpre.Attributes["atos_fechafin"], _service);
            */
            //Log.writelog("Antes de 17 pric. Fecha fin  oferta " + _fechaFinOferta.ToLongDateString() + " " + _fechaFinOferta.ToLongTimeString(), true);

            if (_resultado.Entities.Count > 0)
            {

                DateTime FechaInicioAplicacionUTC = DateTime.SpecifyKind((DateTime)_resultado.Entities[0].Attributes["atos_fechainicioaplicacion"], DateTimeKind.Utc);
                DateTime FechaFinAplicacionUTC = DateTime.SpecifyKind((DateTime)_resultado.Entities[_resultado.Entities.Count - 1].Attributes["atos_fechafinaplicacion"], DateTimeKind.Utc);

                Log.writelog("17 pricingInputCollection atos_fechainicioaplicacion: " + ((DateTime)_resultado.Entities[0].Attributes["atos_fechainicioaplicacion"]).ToLongDateString() + " " + ((DateTime)_resultado.Entities[0].Attributes["atos_fechainicioaplicacion"]).ToLongTimeString(), _traza);
                Log.writelog("17 pricingInputCollection _fechaInicioOferta: " + _fechaInicioOferta.ToLongDateString() + " " + _fechaInicioOferta.ToLongTimeString(), _traza);
                Log.writelog("17 pricingInputCollection fechaInicioOfertaUTC: " + fechaInicioOfertaUTC.ToLongDateString() + " " + fechaInicioOfertaUTC.ToLongTimeString(), _traza);
                Log.writelog("17 pricingInputCollection FechaInicioAplicacionUTC: " + FechaInicioAplicacionUTC.ToLongDateString() + " " + FechaInicioAplicacionUTC.ToLongTimeString(), _traza);
                Log.writelog("17 pricingInputCollection atos_fechafinaplicacion: " + ((DateTime)_resultado.Entities[0].Attributes["atos_fechafinaplicacion"]).ToLongDateString() + " " + ((DateTime)_resultado.Entities[0].Attributes["atos_fechafinaplicacion"]).ToLongTimeString(), _traza);
                Log.writelog("17 pricingInputCollection _fechaFinOferta: " + _fechaFinOferta.ToLongDateString() + " " + _fechaFinOferta.ToLongTimeString(), _traza);
                Log.writelog("17 pricingInputCollection fechaFinOfertaUTC: " + fechaFinOfertaUTC.ToLongDateString() + " " + fechaFinOfertaUTC.ToLongTimeString(), _traza);
                Log.writelog("17 pricingInputCollection FechaFinAplicacionUTC: " + FechaFinAplicacionUTC.ToLongDateString() + " " + FechaFinAplicacionUTC.ToLongTimeString(), _traza);

                /*if ((DateTime)_resultado.Entities[0].Attributes["atos_fechainicioaplicacion"] <= _fechaInicioOferta
                    && (DateTime)_resultado.Entities[_resultado.Entities.Count - 1].Attributes["atos_fechafinaplicacion"] >= _fechaFinOferta)
                    return _resultado;*/

                if (FechaInicioAplicacionUTC <= fechaInicioOfertaUTC
                    && FechaFinAplicacionUTC >= fechaFinOfertaUTC)
                {
                    _resultado.Entities[0].Attributes["atos_fechainicioaplicacion"] = FechaInicioAplicacionUTC;
                    _resultado.Entities[_resultado.Entities.Count - 1].Attributes["atos_fechafinaplicacion"] = FechaFinAplicacionUTC;
                    return _resultado;
                }
                Log.writelog("17 pricingInputCollection. ERROR. Encontrados " + _resultado.Entities.Count.ToString() + " pricing input (" + ((TermPricing)componentes[i]).NombreComponente, _traza);
            }
            else if (_ofertaid == true)
            {
                Log.writelog("ERROR. Encontrados " + _resultado.Entities.Count.ToString() + " pricing input (" + ((TermPricing)componentes[i]).NombreComponente + ")", _traza);
            }

            if (_ofertaid == true) // && _resultado.Entities.Count < 1)
                throw new Exception("ERROR. Encontrados " + _resultado.Entities.Count.ToString() + " pricing input (" + ((TermPricing)componentes[i]).NombreComponente + ")");

            Log.writelog("18 pricingInput");

            


            return construye(_resultado, _fechaInicioOferta, _fechaFinOferta, ref componentes, i, _oferta, _ofpre);

        }
    }
}
