using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk;
//using TermPricing;
using Microsoft.Xrm.Sdk.Query;

namespace TermPricing
{
    
    public sealed partial class FacturacionEstimada : CodeActivity
    {
        #region Propiedades


        [RequiredArgument]
        [Input("Accion a realizar (Input/Output)")]
        public InArgument<string> Accion { get; set; }
        
        [RequiredArgument]
        [Input("Indica si debe escribir el log")]
        public InArgument<Boolean> EscribeLog { get; set; }

        [RequiredArgument]
        [Input("Direccion del web service (para escribir log)")]
        public InArgument<string> UriWS { get; set; }

        [RequiredArgument]
        [Input("Nombre del fichero Log")]
        public InArgument<string> FicheroLog { get; set; }


        [Output("Ejecucion correcta")]
        public OutArgument<Boolean> EjecucionCorrecta { get; set; }

        [Output("Mensaje salida")]
        public OutArgument<string> MensajeSalida { get; set; }

        #endregion

        IWorkflowContext context;
        IOrganizationServiceFactory serviceFactory;
        IOrganizationService service;
        ITracingService tracingService;
        List<String> errores = new List<String>();
        Formula formula;
        List<FormulaBase> variables;

        private CommonWS.Log Log = null;

        //private bool _log = false;
        //private String ficherolog = "D:\\Tmp\\TermPricing.txt";

        private void writelog(String texto, bool _traza = false)
        {
            Log.writelog(texto, _traza);
            /*if (_traza)
                tracingService.Trace(texto);
            if (_log == true)
                System.IO.File.AppendAllText(ficherolog, texto + "\r\n");*/
        }

        private Guid productoFacturacionEstimada()
        {
            Guid _productoId = Guid.Empty;

            QueryExpression _consulta = new QueryExpression("atos_parametroscomercializadora");
            _consulta.ColumnSet.AddColumns("atos_productofacturacionestimada");
            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "statecode";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(0);
            _filtro.Conditions.Add(_condicion);
            _consulta.Criteria.AddFilter(_filtro);
            EntityCollection _resultado = service.RetrieveMultiple(_consulta);

            if (_resultado.Entities.Count > 0)
            {
                if (_resultado.Entities[0].Attributes.Contains("atos_productofacturacionestimada"))
                    _productoId = ((EntityReference)_resultado.Entities[0].Attributes["atos_productofacturacionestimada"]).Id;
            }

            return _productoId;
        }

        private void creaPricingInput(ref Entity _oferta, Entity _ofpre)
        {

            writelog("Despues de construyeFormulas");
            for (int i = 0; i < formula.ComponentesFormula.Count; i++)
            {
                if (formula.ComponentesFormula[i].TipoComponente == "TermPricing")
                {
                    writelog("TermPricing: " + formula.ComponentesFormula[i].NombreComponente + " " + ((TermPricing)formula.ComponentesFormula[i]).NombreEms);
                    //Entity _pricingInput = new Entity("atos_pricinginput");

                    EntityCollection _pricingInputColl = formula.pricingInputCollection(i, service, _oferta, _ofpre);
                    writelog("Encontrados " + _pricingInputColl.Entities.Count.ToString() + " pricinginputs para " + formula.ComponentesFormula[i].NombreComponente);
                    for (int j = 0; j < _pricingInputColl.Entities.Count; j++)
                    {
                        Entity _pricingInput = new Entity("atos_pricinginput");
                        Entity _pricingInputbase = _pricingInputColl.Entities[j];

                        if (_pricingInputbase.Attributes.Contains("atos_porcentajeoimporte"))
                            _pricingInput.Attributes["atos_porcentajeoimporte"] = _pricingInputbase.Attributes["atos_porcentajeoimporte"];
                        if (_pricingInputbase.Attributes.Contains("atos_pfijo"))
                            _pricingInput.Attributes["atos_pfijo"] = _pricingInputbase.Attributes["atos_pfijo"];
                        if (_pricingInputbase.Attributes.Contains("atos_p1"))
                            _pricingInput.Attributes["atos_p1"] = _pricingInputbase.Attributes["atos_p1"];
                        if (_pricingInputbase.Attributes.Contains("atos_p2"))
                            _pricingInput.Attributes["atos_p2"] = _pricingInputbase.Attributes["atos_p2"];
                        if (_pricingInputbase.Attributes.Contains("atos_p3"))
                            _pricingInput.Attributes["atos_p3"] = _pricingInputbase.Attributes["atos_p3"];
                        if (_pricingInputbase.Attributes.Contains("atos_p4"))
                            _pricingInput.Attributes["atos_p4"] = _pricingInputbase.Attributes["atos_p4"];
                        if (_pricingInputbase.Attributes.Contains("atos_p5"))
                            _pricingInput.Attributes["atos_p5"] = _pricingInputbase.Attributes["atos_p5"];
                        if (_pricingInputbase.Attributes.Contains("atos_p6"))
                            _pricingInput.Attributes["atos_p6"] = _pricingInputbase.Attributes["atos_p6"];
                        if (_pricingInputbase.Attributes.Contains("atos_tipo"))
                            _pricingInput.Attributes["atos_tipo"] = _pricingInputbase.Attributes["atos_tipo"];


                        /*if (_pricingInputbase.Attributes.Contains("atos_fechainiciovigencia"))
                            _pricingInput.Attributes["atos_fechainiciovigencia"] = _pricingInputbase.Attributes["atos_fechainiciovigencia"];*/

                        if (_pricingInputbase.Attributes.Contains("atos_fechainiciovigencia"))
                            _pricingInput.Attributes["atos_fechainiciovigencia"] = _pricingInputbase.Attributes["atos_fechainiciovigencia"];
                        else if (_ofpre.Attributes.Contains("createdon"))
                            _pricingInput.Attributes["atos_fechainiciovigencia"] = _ofpre.Attributes["createdon"];

                        if (_pricingInputbase.Attributes.Contains("atos_fechainicioaplicacion"))
                            _pricingInput.Attributes["atos_fechainicioaplicacion"] = _pricingInputbase.Attributes["atos_fechainicioaplicacion"];

                        //Se deja como fecha fin de vigencia la fecha fin de vigencia del pricing input que encuentre o el 1/01/4000 si no encuentra (no se toma la fecha de fin de la oferta por los contratos a pasado).
                        //if (_ofpre.Attributes.Contains("atos_fechafin"))
                        //    _pricingInput.Attributes["atos_fechafinvigencia"] = _ofpre.Attributes["atos_fechafin"];

                        if (_pricingInputbase.Attributes.Contains("atos_fechafinvigencia"))
                            _pricingInput.Attributes["atos_fechafinvigencia"] = _pricingInputbase.Attributes["atos_fechafinvigencia"];

                        if (_pricingInputbase.Attributes.Contains("atos_fechafinaplicacion"))
                            _pricingInput.Attributes["atos_fechafinaplicacion"] = _pricingInputbase.Attributes["atos_fechafinaplicacion"];

                        if (((TermPricing)formula.ComponentesFormula[i]).DependeDeSistemaElectrico == true)
                            _pricingInput.Attributes["atos_sistemaelectricoid"] = new EntityReference("atos_sistemaelectrico", ((EntityReference)_ofpre.Attributes["atos_sistemaelectricoid"]).Id);
                        if (((TermPricing)formula.ComponentesFormula[i]).DependeDeTarifa == true)
                            _pricingInput.Attributes["atos_tarifaid"] = new EntityReference("atos_tarifa", ((EntityReference)_ofpre.Attributes["atos_tarifaid"]).Id);
                        writelog("Asocia pricinginput a oferta");
                        _pricingInput.Attributes["atos_ofertaid"] = new EntityReference("atos_oferta", _oferta.Id);

                        // Se marca el pricingInput como de facturación estimada.
                        _pricingInput.Attributes["atos_facturacionestimada"] = true;

                        _pricingInput.Id = Guid.NewGuid();
                        writelog("Asocia pricinginput a terminodepricing");
                        _pricingInput.Attributes["atos_terminodepricingid"] = new EntityReference("atos_terminodepricing", ((TermPricing)formula.ComponentesFormula[i]).TermpricingId);
                        writelog("Actualiza nombre de pricinginput");

                        if (((TermPricing)formula.ComponentesFormula[i]).NombreEms != "")
                            _pricingInput.Attributes["atos_name"] = ((TermPricing)formula.ComponentesFormula[i]).NombreEms + "-";
                        else
                            _pricingInput.Attributes["atos_name"] = formula.ComponentesFormula[i].NombreComponente + "-";
                        _pricingInput.Attributes["atos_name"] += _ofpre.Attributes["atos_name"].ToString() + "-" + (j + 1);

                        writelog("Pricing Input Name: " + _pricingInput.Attributes["atos_name"].ToString());

                        writelog("Antes de Create pricingInput");
                        service.Create(_pricingInput);
                        writelog("Después de Create pricingInput");
                    }

                }
            }
        }


        private Guid matrizHoraria(Entity _oferta)
        {
            Guid _matrizhorariaid = Guid.Empty;
            Guid _sistemaelectricoid = Guid.Empty;
            Guid _tarifaid = Guid.Empty;
            if (_oferta.Attributes.Contains("atos_sistemaelectricoid"))
                _sistemaelectricoid = ((EntityReference)_oferta.Attributes["atos_sistemaelectricoid"]).Id;

            if (_oferta.Attributes.Contains("atos_tarifaid"))
                _tarifaid = ((EntityReference)_oferta.Attributes["atos_tarifaid"]).Id;

            if (_tarifaid == Guid.Empty || _sistemaelectricoid == Guid.Empty)
                return _matrizhorariaid;
            writelog("matrizHoraria - antes de QueryByAttribute");

            QueryByAttribute _consulta = new QueryByAttribute("atos_matrizportarifaysistemaelectrico");

            writelog("matrizHoraria - antes de definir columnas");
            _consulta.ColumnSet = new ColumnSet(new String[] { "atos_matrizhorariaid" });
            _consulta.AddAttributeValue("atos_tarifaid", _tarifaid.ToString());
            _consulta.AddAttributeValue("atos_sistemaelectricoid", _sistemaelectricoid.ToString());


            EntityCollection _resConsulta = service.RetrieveMultiple(_consulta);
            if (_resConsulta.Entities.Count > 0)
                _matrizhorariaid = ((EntityReference)_resConsulta.Entities[0].Attributes["atos_matrizhorariaid"]).Id;
            return _matrizhorariaid;
        }

        /**
        // <summary>
        // Elimina los registros de pricing input de facturación estimada para una oferta.
        // </summary>
        // <param name="_oferta">Referencia a la entidad oferta </param>
        // <param name="_ofpre">Imagen de la entidad oferta antes de la modificación</param>
        // <remarks>
        // Borra todos los registros de Pricing Input de la oferta recibida.
        // - Actualiza en la oferta el campo atos_pricinginputsrevisados a false
        // </remarks>
         */
        private void deletepricinginput(Entity _oferta)
        {
            writelog("Borra Pricing Input");

            QueryExpression _consulta = new QueryExpression("atos_pricinginput");
            /* 23866 +1 */
            _consulta.NoLock = true;
            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_ofertaid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(_oferta.Id.ToString());
            writelog("Id Oferta " + _oferta.Id.ToString());
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression(); // Excluimos pricing input del cierre de la oferta
            _condicion.AttributeName = "atos_cierreofertaid";
            _condicion.Operator = ConditionOperator.Null;
            writelog("Cierre Oferta a null");
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression(); // Solo seleccionamos pricing input de facturacion estimada
            _condicion.AttributeName = "atos_facturacionestimada";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(true);
            writelog("Facturacion estimada true");
            _filtro.Conditions.Add(_condicion);
            _consulta.Criteria.AddFilter(_filtro);        
            _consulta.ColumnSet.AddColumns("atos_pricinginputid");


            EntityCollection _resultado = service.RetrieveMultiple(_consulta);

            for (int i = 0; i < _resultado.Entities.Count; i++)
            {
                Guid _pricinginputid = (Guid)_resultado.Entities[i].Attributes["atos_pricinginputid"];
                service.Delete("atos_pricinginput", _pricinginputid);
            }

            writelog("Borra Pricing Input Final");
        }


        /**
        // <summary>
        // Elimina los registros de pricing output para una oferta.
        // </summary>
        // <param name="_oferta">Referencia a la entidad oferta</param>
        // <remarks>
        // Borra todos los registros de Pricing Output de la oferta recibida.
        // </remarks>
         */
        private void deletepricingoutput(Entity _oferta)
        {
            writelog("Borra Pricing Output");
            QueryExpression _consulta = new QueryExpression("atos_pricingoutput");
            /* 23866 +1 */
            _consulta.NoLock = true;

            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_ofertaid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(_oferta.Id.ToString());
            writelog("Id Oferta " + _oferta.Id.ToString());
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression(); // Seleccionamos pricing output de facturacion estimada
            _condicion.AttributeName = "atos_facturacionestimada";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(true);
            writelog("Facturacion estimada true");
            _filtro.Conditions.Add(_condicion);

            _consulta.Criteria.AddFilter(_filtro);

            _consulta.ColumnSet.AddColumns("atos_pricingoutputid");

            EntityCollection _resultado = service.RetrieveMultiple(_consulta);
            for (int i = 0; i < _resultado.Entities.Count; i++)
            {
                Guid _pricingoutputid = (Guid)_resultado.Entities[i].Attributes["atos_pricingoutputid"];
                service.Delete("atos_pricingoutput", _pricingoutputid);
            }
            writelog("Borra Pricing Output Final");
        }

        private void recalculaPadre(Entity _oferta,bool llamadaDesdeOutput)
        {
            if (_oferta.Attributes.Contains("atos_ofertapadreid") == false)
                return;
            writelog("recalculaPadre");
            QueryExpression _consulta = new QueryExpression("atos_oferta");
            /* 23866 +1 */
            _consulta.NoLock = true;

            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_ofertapadreid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(((EntityReference)_oferta.Attributes["atos_ofertapadreid"]).Id.ToString());

            _filtro.Conditions.Add(_condicion);

            FilterExpression _filtro2 = new FilterExpression();
            _filtro2.FilterOperator = LogicalOperator.Or;

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_beneficioestimadooferta";
            _condicion.Operator = ConditionOperator.NotNull;
            
            _filtro2.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_facturacionestimadaoferta";
            _condicion.Operator = ConditionOperator.NotNull;
            
            _filtro2.Conditions.Add(_condicion);


            _consulta.Criteria.AddFilter(_filtro);
            _consulta.Criteria.AddFilter(_filtro2);

            _consulta.ColumnSet.AddColumns(new String[] { "atos_beneficioestimadooferta", "atos_facturacionestimadaoferta", "atos_fechaestimacion", "atos_beneficioestimadoanualizado", "atos_facturacionestimadaanualizada" });
            Decimal? _beneficioestimado = null;
            Decimal? _facturacionestimada = null;
            writelog("Buscando ofertas hijas de la oferta padre");
            EntityCollection _resultado = service.RetrieveMultiple(_consulta);
            for (int i = 0; i < _resultado.Entities.Count; i++)
            {
                writelog("Oferta " + i.ToString());
                if (_resultado.Entities[i].Attributes.Contains("atos_beneficioestimadooferta"))
                {
                    writelog("Tiene beneficio estimado");
                    if (_beneficioestimado == null)
                        _beneficioestimado = (Decimal)_resultado.Entities[i].Attributes["atos_beneficioestimadooferta"];
                    else
                        _beneficioestimado += (Decimal)_resultado.Entities[i].Attributes["atos_beneficioestimadooferta"];
                }

                if (_resultado.Entities[i].Attributes.Contains("atos_facturacionestimadaoferta"))
                {
                    writelog("Tiene facturación");
                    if (_facturacionestimada == null)
                        _facturacionestimada = (Decimal)_resultado.Entities[i].Attributes["atos_facturacionestimadaoferta"];
                    else
                        _facturacionestimada += (Decimal)_resultado.Entities[i].Attributes["atos_facturacionestimadaoferta"];
                }
            }
            //if (_beneficioestimado != null || _facturacionestimada != null)
            //{
                writelog("Actualiza");
                writelog("llega0");
                if (_oferta.Contains("atos_name"))
                {
                    writelog("Oferta-->" + _oferta.Attributes["atos_name"].ToString());
                }
                else
                {
                    writelog("Oferta--> no existe oferta" );
                    writelog("Oferta-->" + _oferta.Id);
                }

                if (_oferta.Contains("atos_duracionmeses"))
                    writelog("duracion-->" + _oferta.Attributes["atos_duracionmeses"].ToString());
                else
                    writelog("duracion--> no tiene duracion");
                Entity _of = new Entity("atos_oferta");
             
                decimal duracion = (decimal)_oferta.Attributes["atos_duracionmeses"];
                _of.Id = ((EntityReference)_oferta.Attributes["atos_ofertapadreid"]).Id;
              //  if (_beneficioestimado != null)
                    _of.Attributes["atos_beneficioestimadooferta"] = _beneficioestimado;
                    _of.Attributes["atos_beneficioestimadoanualizado"] = (_beneficioestimado * 12) / duracion;
              //  if (_facturacionestimada != null)
                    _of.Attributes["atos_facturacionestimadaoferta"] = _facturacionestimada;
                    _of.Attributes["atos_facturacionestimadaanualizada"] = (_facturacionestimada * 12) / duracion;
                    if (llamadaDesdeOutput)
                        _of.Attributes["atos_fechaestimacion"] = DateTime.Now;


                service.Update(_of);
                _of = service.Retrieve("atos_oferta", _of.Id, new ColumnSet(true));
                writelog("llama desde1");
                recalculaPadre(_of, llamadaDesdeOutput);
            //}
        }


        private EntityCollection ofertasHijas(Entity ofertaPadre)
        {
            QueryExpression _consulta = new QueryExpression("atos_oferta");
            /* 23866 +1 */
            _consulta.NoLock = true;

            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_ofertapadreid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(ofertaPadre.Id.ToString());
            writelog("Id Oferta " + ofertaPadre.Id.ToString());
            _filtro.Conditions.Add(_condicion);

            _consulta.Criteria.AddFilter(_filtro);
            _consulta.ColumnSet = new ColumnSet(true); // "atos_tipooferta", "stageid", "processid", "atos_tipodeproductofinalid");
            return service.RetrieveMultiple(_consulta);
        }

        private void inicializaFormula(Entity _tipoProducto)
        {
            formula = Formula.creaFormula(_tipoProducto, true, Log, true); // (_tipoProducto, tracingService, true, _log, ficherolog, true);
            variables = formula.construyeFormulas(_tipoProducto, service, true);
        }

        private void borraPricingInput(ref Entity _oferta)
        {
            
            deletepricingoutput(_oferta);
            deletepricinginput(_oferta);

            Entity _of = new Entity("atos_oferta");
            _of.Id = _oferta.Id;
            _of.Attributes["atos_facturacionestimadaoferta"] = null;
            _of.Attributes["atos_beneficioestimadooferta"] = null;
            _of.Attributes["atos_beneficioestimadoanualizado"] = null;
            _of.Attributes["atos_facturacionestimadaanualizada"] = null;
            _of.Attributes["atos_fechaestimacion"] = null;

            service.Update(_of);
            writelog("llama desde2");
            recalculaPadre(_oferta,false);
        }


        private void pricingInputOferta(ref Entity _oferta, Entity _tipoProducto)
        {
            inicializaFormula(_tipoProducto);
            borraPricingInput(ref _oferta);
            creaPricingInput(ref _oferta, _oferta);
        }

        private void pricingInputSubOferta(ref Entity _oferta, Entity _tipoProducto)
        {
            EntityCollection _ofertasHijas = ofertasHijas(_oferta);
            for (int i = 0; i < _ofertasHijas.Entities.Count; i++)
            {
                Entity _ofertaH = _ofertasHijas[i];
                borraPricingInput(ref _ofertaH);
                //pricingInputOferta(ref _ofertaH, _tipoProducto);
            }
            pricingInputOferta(ref _oferta, _tipoProducto);
        }


        private void pricingInputOPadre(ref Entity _oferta, Entity _tipoProducto)
        {
            EntityCollection _ofertasHijas = ofertasHijas(_oferta);
            for (int i = 0; i < _ofertasHijas.Entities.Count; i++)
            {
                Entity _ofertaH = _ofertasHijas[i];
                pricingInputSubOferta(ref _ofertaH, _tipoProducto);
            }
        }

        private EntityCollection pricingInputsDeLaOferta(Guid _ofertaid)
        {

            QueryExpression _consulta = new QueryExpression("atos_pricinginput");
            /* 23866 +1 */
            _consulta.NoLock = true;

            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_ofertaid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(_ofertaid.ToString());

            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_facturacionestimada";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(true);
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression(); // Excluimos pricing input del cierre de la oferta
            _condicion.AttributeName = "atos_cierreofertaid";
            _condicion.Operator = ConditionOperator.Null;
            _filtro.Conditions.Add(_condicion);

            _consulta.Criteria.AddFilter(_filtro);

            _consulta.ColumnSet = new ColumnSet(true);

            EntityCollection _resultado = service.RetrieveMultiple(_consulta);
            return _resultado;
        }


        private void copiaPricingInputPadre(Entity _oferta)
        {
            if (((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value == 300000002) // Tipo oferta
            {
                if (_oferta.Attributes.Contains("atos_ofertapadreid")) // Oferta hija
                {
                    EntityCollection _resultado = pricingInputsDeLaOferta(_oferta.Id);
                    if (_resultado.Entities.Count <= 0) // si la oferta hija no tiene pricing inputs copiamos los de la suboferta
                    {
                        _resultado = pricingInputsDeLaOferta(((EntityReference)_oferta.Attributes["atos_ofertapadreid"]).Id);

                        for (int i = 0; i < _resultado.Entities.Count; i++)
                        {
                            Entity _pricingInput = _resultado.Entities[i];
                            _pricingInput.Attributes.Remove("atos_pricinginputid");
                            _pricingInput.Id = Guid.NewGuid();
                            _pricingInput.Attributes["atos_ofertaid"] = new EntityReference("atos_oferta", _oferta.Id);
                            service.Create(_pricingInput);
                        }
                    }
                }
            }

        }


        private void pricingOutputOferta(Entity _oferta, Entity _tipoProducto)
        {
            if (_oferta.Attributes.Contains("atos_tarifaid") == false)
                errores.Add("La oferta no tiene tarifa");
            else
            {
                copiaPricingInputPadre(_oferta); // Si es una oferta hija que no tiene pricing inputs los copia de la suboferta padre.

                formula = Formula.creaFormula(_tipoProducto, true, Log, true); //(_tipoProducto, tracingService, true, _log, ficherolog, true);

                variables = formula.construyeFormulas(_tipoProducto, service, true);

                Entity _tarifa = service.Retrieve("atos_tarifa", ((EntityReference)_oferta.Attributes["atos_tarifaid"]).Id,
                                                                   new ColumnSet(new[] { "atos_name", "atos_numeroperiodos" }));
                Guid _matrizhorariaid = matrizHoraria(_oferta);
                if (_matrizhorariaid == Guid.Empty)
                    errores.Add("La oferta no tiene asociada matriz horaria");
                else
                {
                    deletepricingoutput(_oferta);
                    formula.calculaCollectionPromedio(_matrizhorariaid, ref variables, service, ref _oferta, _oferta, _tipoProducto, _tarifa, ref errores);
                    if (_oferta.Attributes.Contains("atos_facturacionestimadaoferta"))
                    {
                        Entity _of = new Entity("atos_oferta");
                        decimal duracion = (decimal)_oferta.Attributes["atos_duracionmeses"];
                        _of.Id = _oferta.Id;
                        _of.Attributes["atos_facturacionestimadaoferta"] = _oferta.Attributes["atos_facturacionestimadaoferta"];
                        _of.Attributes["atos_facturacionestimadaanualizada"] = ((Decimal)_oferta.Attributes["atos_facturacionestimadaoferta"] * 12) / duracion;
                        _of.Attributes["atos_beneficioestimadooferta"] = _oferta.Attributes["atos_beneficioestimadooferta"];
                        _of.Attributes["atos_beneficioestimadoanualizado"] = ((Decimal)_oferta.Attributes["atos_beneficioestimadooferta"] * 12) / duracion;
                        _of.Attributes["atos_fechaestimacion"] = DateTime.Now;

                        
                       
                        service.Update(_of);
                        writelog("llama desde3");
                        recalculaPadre(_oferta,true);
                        
                    }
                }
            }
        }


        private void pricingOutputSubOferta(Entity _oferta, Entity _tipoProducto)
        {
            EntityCollection _ofertasHijas = ofertasHijas(_oferta);
            for (int i = 0; i < _ofertasHijas.Entities.Count; i++)
            {
                pricingOutputOferta(_ofertasHijas[i], _tipoProducto);
            }
        }


        private void pricingOutputOPadre(Entity _oferta, Entity _tipoProducto)
        {
            EntityCollection _ofertasHijas = ofertasHijas(_oferta);
            for (int i = 0; i < _ofertasHijas.Entities.Count; i++)
            {
                pricingOutputSubOferta(_ofertasHijas[i], _tipoProducto);
            }
        }



        protected override void Execute(CodeActivityContext executionContext)
        {
            errores.Clear();

            context = executionContext.GetExtension<IWorkflowContext>();
            serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            service = serviceFactory.CreateOrganizationService(context.UserId);
            tracingService = (ITracingService)executionContext.GetExtension<ITracingService>();
            tracingService.Trace("Inicio del workflow");

            if (context.PrimaryEntityName != "atos_oferta")
            {
                this.EjecucionCorrecta.Set(executionContext, true);
                return;
            }

            Guid _productoFacturacionEstimada = productoFacturacionEstimada();
            if (_productoFacturacionEstimada == Guid.Empty)
                throw new System.Exception("No hay configurado ningún producto de facturación estimada");

            Entity _tipoProducto = service.Retrieve("atos_tipodeproducto", _productoFacturacionEstimada, new ColumnSet(true));

            Entity _oferta = service.Retrieve("atos_oferta", context.PrimaryEntityId, new ColumnSet(true)); // revisar para poner solo las columnas necesarias

            Boolean _log = this.EscribeLog.Get(executionContext);
            String uriws = this.UriWS.Get(executionContext);
            String ficherolog = this.FicheroLog.Get(executionContext);
            if (ficherolog == "" || uriws == "")
                _log = false;


            Log = new CommonWS.Log();
            Log.setLog(_log, uriws, "", ficherolog, tracingService);

            writelog("----------------------------------------------");
            writelog("Workflow Facturacion Estimada: " + DateTime.Now.ToLocalTime().ToString());
            //formula = Formula.creaFormula(_tipoProducto, tracingService, true, _log, ficherolog, true);
            
            //variables = formula.construyeFormulas(_tipoProducto, service, true);
            
            //Entity _parametros = service.Retrieve("atos_parametroscomercializadora",
            //atos_productofacturacionestimada
            String _accion = this.Accion.Get(executionContext).ToString();

            if (_accion == "Input")
            {
                if (_oferta.Attributes.Contains("atos_tipooferta"))
                {
                    if (((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value == 300000000) // Oferta padre
                    {
                        pricingInputOPadre(ref _oferta, _tipoProducto);
                    }
                    else if (((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value == 300000001) // Suboferta
                    {
                        pricingInputSubOferta(ref _oferta, _tipoProducto);
                    }
                    else
                    {
                        pricingInputOferta(ref _oferta, _tipoProducto);
                    }
                }
                else
                    errores.Add("La oferta no tiene tipo");
                
            }
            else if (_accion == "Output")
            {
                // Genera pricing outputs
                
                if (_oferta.Attributes.Contains("atos_tipooferta"))
                {
                    if (((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value == 300000000) // Oferta padre
                    {
                        pricingOutputOPadre(_oferta, _tipoProducto);
                    }
                    else if (((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value == 300000001) // Suboferta
                    {
                        pricingOutputSubOferta(_oferta, _tipoProducto);
                    }
                    else
                    {
                        pricingOutputOferta(_oferta, _tipoProducto);
                    }
                }
                else
                    errores.Add("La oferta no tiene tipo");
                
            }
            else
            {
                this.EjecucionCorrecta.Set(executionContext, false);
                this.MensajeSalida.Set(executionContext, "Incorrecta configuración del Workflow. La acción debe ser Input o Output");
            }

            String _error = "";
            if (errores.Count > 0)
            {
                writelog("=========================================", true);
                writelog("Se han encontrado los siguientes errores:", true);

                for (int i = 0; i < errores.Count; i++)
                {
                    writelog(" - " + errores[i], true);
                    _error += string.Format("{1}{0}.<br/>", errores[i], Environment.NewLine);
                }
                writelog("=========================================", true);
            }

            if (_error != "")
            {
                _error += Environment.NewLine;
                this.EjecucionCorrecta.Set(executionContext, false);
                this.MensajeSalida.Set(executionContext, _error);
            }
            else
                this.EjecucionCorrecta.Set(executionContext, true);

        }
    }
}
