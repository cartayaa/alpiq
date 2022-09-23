/*
 File="TermPricing : IPlugin" 
 Copyright (c) Atos. All rights reserved.

 Plugin que se ejecuta cuando se crea un nuevo registro en atos_trigger con 
 los valores OfertaMP para accion y account para la entidad.

 Fecha 		Codigo  Version Descripcion                                     Autor
 05.09.2022 23866   no-lock Incorporacion del No-lock a Consultas           AC
*/

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Collections.Generic;
//using System.Data;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
//using System.Runtime.Serialization;
//using Microsoft.Xrm.Sdk.Client;
using System.Xml;
using System.Text;

namespace TermPricing
{
    public class Pricing : IPlugin
    {
        LocalPluginContext context;
        List<FormulaBase> variables = null;
        Formula formula = null;
        List<String> errores = new List<String>();

        // Tipos de Ofertas
        const int MULTIPUNTO = 300000000;
        const int SUBOFERTA  = 300000001;
        const int OFERTA     = 300000002;

        // Commodity 
        const int POWER = 300000000;
        const int GAS = 300000001;

        // Caterogir de la Fase
        const int IDENTIFICAR = 4;
        const int PROPONER = 2;
        const int DESARROLLAR = 1;
        const int CERRAR = 3;

        private Boolean esGas;
        //private bool _log = false;
        //private String urlwslog = "";
        //private String ficherolog = "TermPricing.txt"; // = "D:\\Tmp\\TermPricing.txt";
        //private String subcarpetalog = "";
        //private const Char SEPARADOR = '#';
        private bool tipoCalculoPromedio = false;
        private CommonWS.Log Log = null;
        private Log4cs logger = null;

        /*
         Dependiendo de la fase debe hacer lo siguiente:
         - Copia del producto base
         - Clonar los pricing inputs asociados a la oferta
         - Calcular los pricing output (hay que meter también pricing output para la fórmula final y para las variables intermedias, 
           en estos casos el pricing output no irá relacionado con un término de pricing)
        */
        public void Execute(IServiceProvider serviceProvider)
        {
            tipoCalculoPromedio = false;
            String NesGas = string.Empty;
            //bool debug = true;     // Debug
            int TipoOferta = 0;     // TipoOferta del Pabre
            Entity _oferta;         // Oferta Padre
            Entity processStage;    // Fase de proceso
            int stagecategory;      // Stage Category Id
            string _ofertaName;     // Nombre de la Oferta
            int statuscode;         // Razon de estado de OfertaPricing
            #region Programa principal
            errores.Clear();
      
            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");

            context = new LocalPluginContext(serviceProvider); // Construct the Local plug-in context.
            //Log.tracingService = context.TracingService;
            logger = new Log4cs(context.TracingService);

            /* 23866 -1 */
            // Log.writelog("TermPricing.Execute(), " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(), debug);
            logger.Info("TermPricing");
            #endregion

            try
            {
                #region Definiciones Target - PreImage

                if (context.PluginExecutionContext.MessageName != "Update")
                    return;

                if (context.PluginExecutionContext.PreEntityImages["PreEntityImage"] == null)
                    // logger.Info("No viene información de la entidad antes de los cambios");                    
                    return;

                Entity efpre = (Entity)context.PluginExecutionContext.PreEntityImages["PreEntityImage"];  // ORIGEN

                if (efpre.Attributes.Contains("bpf_atos_ofertaid") == false)
                    return;

                Entity _ofertaPricing = (Entity)context.PluginExecutionContext.InputParameters["Target"]; // DESTINO

                if (_ofertaPricing.LogicalName != "atos_bpf_1c3d1c4af29543429ee2b7465a2e6ee8")
                    return;

                /* 23866 -11 */
                if (_ofertaPricing.Attributes.Contains("activestageid") == false)
                {
                    if (_ofertaPricing.Attributes.Contains("statuscode") == true)
                        statuscode = ((OptionSetValue)_ofertaPricing.Attributes["statuscode"]).Value;

                    // Como se han completado todos los stages, se debe cerrar la regla de 
                    // negocio para todas las oportunidades                    
                    
                    // CLOSE_oferta( efpre,  _ofertaPricing,  context);
                    return;
                }

                string s_activeStageId = _ofertaPricing.Attributes.Contains("activestageid") ? _ofertaPricing.Attributes["activestageid"].ToString() : string.Empty;

                #endregion

                #region Validaciones de Target - PreImage

                _oferta = context.OrganizationService.Retrieve("atos_oferta", ((EntityReference)efpre.Attributes["bpf_atos_ofertaid"]).Id, new ColumnSet(true));
                TipoOferta = ((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value;
                _ofertaName = _oferta.Attributes.Contains("atos_name") ? _oferta.Attributes["atos_name"].ToString() : string.Empty;

                try
                {
                    processStage = context.OrganizationService.Retrieve("processstage", ((EntityReference)_ofertaPricing.Attributes["activestageid"]).Id, new ColumnSet(true));
                    stagecategory = ((OptionSetValue)processStage.Attributes["stagecategory"]).Value;
                }
                catch (Exception ex)
                {
                    Log.writelog("Exception:" + ex.Message, true);
                    throw ex;
                }

                esGas = false; // Pricing Gas
               
                if (_oferta.Attributes.Contains("atos_commodity"))  // Pricing gas
                {
                    if (((OptionSetValue)_oferta.Attributes["atos_commodity"]).Value == GAS)
                    {
                        esGas = true;
                        NesGas = "G";
                    }
                }
                else if (efpre.Attributes.Contains("atos_commodity"))  // Pricing Powert
                {
                    if (((OptionSetValue)efpre.Attributes["atos_commodity"]).Value == GAS) { 
                        esGas = true;
                        NesGas = "P";
                    }
                }

                /*------------------------------------------*/
                /* Ofera esta en el mismo Stage              */
                /*------------------------------------------*/
                if (efpre.Attributes.Contains("activestageid") == true)
                {        
                    Entity processStagePre = context.OrganizationService.Retrieve( "processstage", 
                                             ((EntityReference)efpre.Attributes["activestageid"]).Id, 
                                             new ColumnSet(true));

                    if (((OptionSetValue)processStagePre.Attributes["stagecategory"]).Value == 
                        ((OptionSetValue)processStage.Attributes["stagecategory"]).Value)
                    {
                        // string label = processStagePre.FormattedValues["stagecategory"].ToString();
                        // logger.Info("\tStage de Oferta alineada con respecto al padre");
                        return;
                    }
                }

                #endregion

                #region Commodity Gas unicamente

                Guid peajeGasId = Guid.Empty;
                if (esGas)
                {
                    if (_oferta.Attributes.Contains("atos_peajeid"))
                        peajeGasId = ((EntityReference)_oferta.Attributes["atos_peajeid"]).Id;
                }

                    // Si es de gas y no es de tipo Multipunto tiene que tener informado peaje
                if (esGas && peajeGasId == Guid.Empty &&
                    _oferta.Attributes.Contains("atos_tipooferta") &&
                    ((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value != MULTIPUNTO)
                {
                    //context.Trace(string.Format(CultureInfo.InvariantCulture, "La oferta no tiene informado el peaje"));
                    logger.Info( "The offer has not informed the toll - La oferta no tiene informado el peaje");
                    return;
                }
                // Fin Pricing gas

                #endregion

                if (TipoOferta == MULTIPUNTO) logger.Info(string.Format("Multipunto: {0} {1}", _ofertaName, NesGas));
                if (TipoOferta == SUBOFERTA) logger.Info(string.Format("SubOferta: {0} {1}", _ofertaName, NesGas));
                if (TipoOferta == OFERTA) logger.Info(string.Format("Oferta: {0} {1}", _ofertaName, NesGas));

                /*-------------------------------------
                 * Estados
                 *  4-Identificar
                 *  2-Proponer
                 *  1-Desarrollar
                 *  4-Cerrar
                 *-------------------------------------*/

                stagecategory = ((OptionSetValue)processStage.Attributes["stagecategory"]).Value; /* PRE-IMAGE */

                switch (stagecategory)
                {                   
                    case IDENTIFICAR:
                    #region IDENTIFICAR
                        logger.Info("IDENTIFICAR");

                        Guid _productofinalid = Guid.Empty;

                        if (_oferta.Attributes.Contains("atos_tipodeproductofinalid"))
                            _productofinalid = ((EntityReference)_oferta.Attributes["atos_tipodeproductofinalid"]).Id;

                        if (_productofinalid != Guid.Empty && TipoOferta != OFERTA)
                            DELETE_Productohijas(efpre, _oferta, _ofertaPricing, context, _productofinalid);

                        DELETE_producto(ref _oferta);
                        break;
                    #endregion 
                    case PROPONER:
                    #region PROPONER
                        logger.Info("PROPONER");

                        _oferta.Attributes["atos_tipoproductofinalrevisado"] = false;

                        if (TipoOferta == SUBOFERTA)
                            DELETE_pricinginput(ref _oferta);

                        if (efpre.Attributes.Contains("activestageid") == true)
                        {
                            Entity StagePrev = context.OrganizationService.Retrieve("processstage", ((EntityReference)efpre.Attributes["activestageid"]).Id, new ColumnSet(true));

                            if (((OptionSetValue)StagePrev.Attributes["stagecategory"]).Value == IDENTIFICAR) 
                            {
                                if (_oferta.Attributes.Contains("atos_ofertapadreid") &&
                                    !(TipoOferta == SUBOFERTA &&
                                      (_oferta.Attributes.Contains("atos_tipodeproductoid")) &&
                                      !_oferta.Attributes.Contains("atos_tipodeproductofinalid")))
                                {
                                    // Las ofertas hijas en las multipunto se actualiza el primer paso por Workflow.
                                    // Las subofertas se puede modificar el producto final
                                    return;
                                }

                                // Actualiza la oferta desasocialdo el producto base
                                DELETE_producto(ref _oferta);

                                // Actualiza la oferta con el nuevo producto
                                ADD_producto(ref _oferta);

                                if (TipoOferta == MULTIPUNTO || TipoOferta == SUBOFERTA)
                                    UPDATE_OfertaStaging(efpre, _oferta, _ofertaPricing, context, true);

                            }
                        }
                        break;
                    #endregion
                    case DESARROLLAR:
                    #region DESARROLLAR
                        logger.Info("DESARROLLAR");

                        _oferta.Attributes["atos_pricinginputsrevisados"] = false;
                        _oferta.Attributes["atos_tipodecalculo"] = null;

                        // AC Hoy
                        if (TipoOferta == SUBOFERTA)
                            DELETE_pricingoutput(ref _oferta);

                        if (efpre.Attributes.Contains("activestageid") == true)
                        {
                            Entity StagePrev = context.OrganizationService.Retrieve( "processstage", 
                                               ((EntityReference)efpre.Attributes["activestageid"]).Id, 
                                               new ColumnSet(true));

                            if (TipoOferta == MULTIPUNTO)
                                UPDATE_OfertaStaging(efpre, _oferta, _ofertaPricing, context);

                            if (((OptionSetValue)StagePrev.Attributes["stagecategory"]).Value == PROPONER)
                            {

                                if ((esGas && peajeGasId == Guid.Empty) ||
                                     (!esGas && (_oferta.Attributes.Contains("atos_tarifaid") == false ||
                                    _oferta.Attributes.Contains("atos_sistemaelectricoid") == false ||
                                    _oferta.Attributes.Contains("atos_subsistemaid") == false)))
                                    return;

                                if (_oferta.Attributes.Contains("atos_ofertapadreid") && TipoOferta == OFERTA)
                                    return;

                                // AC Hoy
                                //if (TipoOferta == SUBOFERTA)
                                    DELETE_pricinginput(ref _oferta);

                                // AC Hoy
                                //if (TipoOferta == SUBOFERTA)
                                    CREATE_pricinginputCollection(ref _oferta);

                                if (TipoOferta == SUBOFERTA)
                                    UPDATE_OfertaStaging(efpre, _oferta, _ofertaPricing, context);
                            }
                        }
                        
                        try
                        {
                            context.OrganizationService.Update(_oferta);
                        }
                        catch (Exception ex)
                        {
                            Log.writelog("Exception:" + ex.Message, true);
                        }
                        break;
                    #endregion
                    case CERRAR:
                        #region CERRAR
                        logger.Info("CERRAR");

                        if (_oferta.Attributes.Contains("atos_ofertapadreid") && TipoOferta == OFERTA)
                        //if (TipoOferta == OFERTA)
                            return;

                        if (_oferta.Attributes.Contains("atos_tipodecalculo"))
                            if (TipoOferta == MULTIPUNTO)
                                tipoCalculoPromedio = true;  // Tipo de calculo promedio

                        if (esGas) // Pricing Gas
                        {
                            // En gas el tipo de calculo siempre es directo
                            tipoCalculoPromedio = false;
                        }

                        //Log.writelog("tipoCalculoPromedio: " + tipoCalculoPromedio.ToString());

                        if (TipoOferta == MULTIPUNTO)
                            UPDATE_OfertaStaging(efpre, _oferta, _ofertaPricing, context, false, true);

                        if ((esGas && peajeGasId == Guid.Empty) ||
                            (!esGas && 
                            (_oferta.Attributes.Contains("atos_tarifaid") == false ||
                             _oferta.Attributes.Contains("atos_sistemaelectricoid") == false ||
                             _oferta.Attributes.Contains("atos_subsistemaid") == false)))
                            return;

                        Entity _tarifa;
                        if (esGas) // Pricing Gas
                        {
                            //logger.Info("buscando peaje: " + peajeGasId.ToString());
                            _tarifa = context.OrganizationService.Retrieve("atos_tablasatrgas",
                                      peajeGasId,
                                      new ColumnSet(new[] { "atos_name" }));

                            _tarifa.Attributes["atos_numeroperiodos"] = (Decimal)1;

                            //logger.Info("peaje encontrado");
                            //if (_tarifa.Attributes.Contains("atos_name"))
                            //    logger.Info("peaje: " + _tarifa.Attributes["atos_name"].ToString());
                        }
                        else
                        {
                            _tarifa = context.OrganizationService.Retrieve("atos_tarifa",
                                      ((EntityReference)_oferta.Attributes["atos_tarifaid"]).Id,
                                      new ColumnSet(new[] { "atos_name", "atos_numeroperiodos" }));
                        }


                        //logger.Info("Buscar tipo de producto");

                        // Fin Pricing Gas
                        if (!_oferta.Contains("atos_tipodeproductofinalid"))
                            logger.Error("no hay atos_tipodeproductofinalid");

                        Entity _tipoProducto = context.OrganizationService.Retrieve("atos_tipodeproducto",
                                                ((EntityReference)_oferta.Attributes["atos_tipodeproductofinalid"]).Id,
                                                new ColumnSet(new[] {
                                                    "atos_formula",    "atos_nombreems",
                                                    "atos_nombrevi1",  "atos_valorvi1",
                                                    "atos_nombrevi2",  "atos_valorvi2",
                                                    "atos_nombrevi3",  "atos_valorvi3",
                                                    "atos_nombrevi4",  "atos_valorvi4",
                                                    "atos_nombrevi5",  "atos_valorvi5",
                                                    "atos_nombrevi6",  "atos_valorvi6",
                                                    "atos_nombrevi7",  "atos_valorvi7",
                                                    "atos_nombrevi8",  "atos_valorvi8",
                                                    "atos_nombrevi9",  "atos_valorvi9",
                                                    "atos_nombrevi10", "atos_valorvi10" }));

                        logger.Info("-----construyeFormulas------");
                        construyeFormulas(_tipoProducto);

                        logger.Info("-----calcula-----");
                        calcula(ref _oferta, _oferta, _tipoProducto, _tarifa);

                        if (TipoOferta == SUBOFERTA)
                            UPDATE_OfertaStaging(efpre, _oferta, _ofertaPricing, context, false, true);

                        break;
                    #endregion
                }

                String _error = string.Empty;
                if (errores.Count > 0)
                {
                    bool containsSearchResult; 
                    string cat = "Object reference not set to an instance of an object.";

                    for (int i = 0; i < errores.Count; i++)
                    {
                        logger.Info(errores[i]);
                        containsSearchResult = errores[i].Contains(cat);
                        if (!containsSearchResult)
                            //_error += string.Format("{0}{1} <br/>", errores[i], Environment.NewLine);
                            _error += string.Format("{0}{1}", errores[i], Environment.NewLine);
                    }
                }

                if (_error != "")
                {
                    _error += Environment.NewLine;
                    throw new InvalidPluginExecutionException(OperationStatus.Canceled, _error);
                }

            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                logger.Error(string.Format("Exception: {0}", e.ToString()));
                throw;
            }
            finally
            {
                logger.End("-- FIN -- ");
            }
        }

        private void CLOSE_oferta(Entity preOferta,  Entity ofertaPricing, LocalPluginContext context)
        {
            // private void DELETE_Productohijas(Entity preOferta, Entity ofertaPadre, Entity ofertaPricing, LocalPluginContext context, Guid productofinalId)
            int TipoOferta = 0;     // TipoOferta del Pabre
            Entity ofertaPadre;         // Oferta Padre
            //int stagecategory;      // Stage Category Id
            string _ofertaName;     // Nombre de la Oferta
            //int statuscode;         // Razon de estado de OfertaPricing

            ofertaPadre = context.OrganizationService.Retrieve("atos_oferta", ((EntityReference)preOferta.Attributes["bpf_atos_ofertaid"]).Id, new ColumnSet(true));
            TipoOferta = ((OptionSetValue)ofertaPadre.Attributes["atos_tipooferta"]).Value;
            _ofertaName = ofertaPadre.Attributes.Contains("atos_name") ? ofertaPadre.Attributes["atos_name"].ToString() : string.Empty;


            // Promover atos_validadoparacontrato a True
            if (ofertaPadre.Attributes.Contains("atos_validadoparacontrato"))
            {
                ofertaPadre.Attributes["atos_validadoparacontrato"] = true;

                context.OrganizationService.Update(ofertaPadre);

                logger.Info("Oferta Stage terminado");
            }


            // Recupera ofertas hijas
            EntityCollection _resultado = Filter_OfertasHijas(ofertaPadre, context);

            int count = _resultado.Entities.Count;

            for (int i = 0; i < count; i++)
            {
                // Promover atos_validadoparacontrato a True
                if (_resultado.Entities[i].Attributes.Contains("atos_validadoparacontrato"))
                {
                    _resultado.Entities[i].Attributes["atos_validadoparacontrato"] = true;

                    context.OrganizationService.Update(_resultado.Entities[i]);

                    _ofertaName = _resultado.Entities[i].Attributes.Contains("atos_name") ? _resultado.Entities[i].Attributes["atos_name"].ToString() : string.Empty;
                    logger.Info("Oferta Stage terminado:" + _ofertaName);
                }
            }

            if (count > 0) logger.Info(string.Format("{0} Ofertas hijas terminadas", count));
            /*
                Entity _ofertaHija = new Entity("atos_oferta");
                _ofertaHija.Id = _resultado.Entities[i].Id;

                EntityCollection ofertasPricing = Filter_OfertaStaging(_resultado.Entities[i], context);

                foreach (Entity op in ofertasPricing.Entities)
                {
                    Entity opAct = new Entity("atos_bpf_1c3d1c4af29543429ee2b7465a2e6ee8");
                    opAct.Id = op.Id;
                    opAct.Attributes["statuscode"] = 2;

                    logger.Info("Id:" + op.Id);
                    logger.Info(string.Format("\t\tId: {0}", opAct.Id));

                    try
                    {
                        context.OrganizationService.Update(opAct);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Exception:" + ex.Message);
                    }
                }
            }

            if (count > 0) logger.Info(string.Format("{0} Ofertas hijas cerradas", count));
            */
        }

        #region Codigo Desconocido

        private void init(String parametros)
        {
            Log = new CommonWS.Log();
            if (parametros != "")
            {
                XmlDocument res = new XmlDocument();

                res.LoadXml(parametros);


                if (res.GetElementsByTagName("log").Count > 0)
                {
                    XmlNode logxml = res.GetElementsByTagName("log")[0];
                    bool _log = Convert.ToBoolean(logxml.Attributes["escribirlog"].Value);
                    String urlwslog = logxml.Attributes["urlwslog"].Value;
                    String ficherolog = logxml.Attributes["ficherolog"].Value;
                    String subcarpetalog = logxml.Attributes["subcarpetalog"].Value;
                    Log.setLog(_log, urlwslog, subcarpetalog, ficherolog, null);

                }
            }
        }
        
        public Pricing(String unsecureconfiguration, String secureconfiguration)
        {
            String parametros = "";
            if (secureconfiguration != "" && secureconfiguration != null)
                parametros = secureconfiguration;
            else if (unsecureconfiguration != "" && unsecureconfiguration != null)
                parametros = unsecureconfiguration;

            this.init(parametros);
            
        }

        #endregion

        private void construyeFormulas(Entity _tipoProducto)
        {
            formula = Formula.creaFormula(_tipoProducto, tipoCalculoPromedio, Log);
            formula.esInstalacionGas = esGas;

            variables = formula.construyeFormulas(_tipoProducto, context.OrganizationService, tipoCalculoPromedio);
        }

        private void calcula(ref Entity _oferta, Entity _ofpre, Entity _tipoProducto, Entity _tarifa)
        {
            //formula.calcula(ref variables, context.OrganizationService, _oferta, _ofpre, _tipoProducto, _tarifa, ref errores);
            logger.Info("-------------------------------- Pricing.calcula --------------------------------");
            formula.setLog(Log);
            if (tipoCalculoPromedio == true)
            {
                //Log.writelog("Antes de buscar matriz horaria",true);
                Guid _matrizhorariaid = matrizHoraria(_oferta);
                //Log.writelog("Antes de calculaCollectionPromedio");
                formula.calculaCollectionPromedio(_matrizhorariaid, ref variables, context.OrganizationService, ref _oferta, _ofpre, _tipoProducto, _tarifa, ref errores);
            }
            else
                formula.calculaCollection(ref variables, context.OrganizationService, _oferta, _ofpre, _tipoProducto, _tarifa, ref errores);
        }

        /*-----------------------------*/
        /* ADD_producto                */
        /*-----------------------------*/
        private void ADD_producto(ref Entity _oferta)
        {
            if (_oferta.Attributes.Contains("atos_tipodeproductoid") == false )
                return;

            Guid _tipoproductoid = ((EntityReference)_oferta.Attributes["atos_tipodeproductoid"]).Id;
         
            Entity _tipoProductoBase = context.OrganizationService.Retrieve("atos_tipodeproducto", _tipoproductoid, new ColumnSet(true));

            _tipoProductoBase.Attributes.Remove("atos_tipodeproductoid");
            _tipoProductoBase.Id = Guid.NewGuid();
            _tipoProductoBase.Attributes["atos_base"] = false;            
            _tipoProductoBase.Attributes.Remove("atos_name"); // El campo atos_name lo calcula la secuencia
            _tipoProductoBase.Attributes.Remove("atos_crearenems"); // Se eliminan los campos para ems
            _tipoProductoBase.Attributes.Remove("atos_tipodeproductocreadoenems");
            _tipoProductoBase.Attributes.Remove("atos_interfazformulaems");
            _tipoProductoBase.Attributes.Remove("atos_ultimowsformulaejecutado");
            _tipoProductoBase.Attributes.Remove("atos_ultimologwsformula");

            Guid _nuevotipoproducto = context.OrganizationService.Create(_tipoProductoBase);
            _oferta.Attributes["atos_tipodeproductofinalid"] = new EntityReference("atos_tipodeproducto", _nuevotipoproducto);
            context.OrganizationService.Update(_oferta);

            logger.Info("Producto Base creado");
        }

        private void ADD_pricinginput(ref Entity _oferta)
        {
            logger.Info("Crear Princing Inputs");

            if (_oferta.Attributes.Contains("atos_tipodeproductofinalid") == false )
                return;
            if (_oferta.Attributes.Contains("atos_tipoproductofinalrevisado") == false)
                return;
            if (_oferta.Attributes.Contains("atos_pricinginputsrevisados") == false)
            {
                errores.Add("Debe revisar antes los Pricing Inputs");
                return;
            }

            Guid _tipoproductoid = ((EntityReference)_oferta.Attributes["atos_tipodeproductofinalid"]).Id;
           
            Entity _tipoProducto = context.OrganizationService.Retrieve("atos_tipodeproducto", _tipoproductoid, new ColumnSet(true));
  
            // Copiamos los pricinginput
            construyeFormulas(_tipoProducto);

            for (int i = 0; i < formula.ComponentesFormula.Count; i++)
            {
                if (formula.ComponentesFormula[i].TipoComponente == "TermPricing")
                {
                    String msg = string.Empty;

                    msg = string.Format("{0} TermPricing: {1} {2}", 
                                i, 
                                formula.ComponentesFormula[i].NombreComponente, 
                                ((TermPricing)formula.ComponentesFormula[i]).NombreEms);

                    logger.Info(msg);

                    Entity _pricingInput = new Entity("atos_pricinginput");
                    try
                    {
                        Entity _pricingInputbase = formula.pricingInput(i, context.OrganizationService, _oferta, _oferta);

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
                    }
                    catch //(Exception e)
                    {
                        logger.Error("no existe pricinginput "); 
                    }

                    #region Validaciones

                    if (_oferta.Attributes.Contains("createdon"))
                        _pricingInput.Attributes["atos_fechainiciovigencia"] = _oferta.Attributes["createdon"];

                    if (_oferta.Attributes.Contains("atos_fechainicio"))
                        _pricingInput.Attributes["atos_fechainicioaplicacion"] = _oferta.Attributes["atos_fechainicio"];

                    /*if (_ofpre.Attributes.Contains("atos_fechafinvigenciaoferta"))
                        _pricingInput.Attributes["atos_fechafinvigencia"] = _ofpre.Attributes["atos_fechafinvigenciaoferta"];
                    else*/

                   if (_oferta.Attributes.Contains("atos_fechafin"))
                        _pricingInput.Attributes["atos_fechafinvigencia"] = _oferta.Attributes["atos_fechafin"];

                    if (_oferta.Attributes.Contains("atos_fechafin"))
                        _pricingInput.Attributes["atos_fechafinaplicacion"] = _oferta.Attributes["atos_fechafin"]; 

                    _pricingInput.Attributes["atos_sistemaelectricoid"] = new EntityReference("atos_sistemaelectrico", ((EntityReference)_oferta.Attributes["atos_sistemaelectricoid"]).Id);
                    _pricingInput.Attributes["atos_tarifaid"] = new EntityReference("atos_tarifa", ((EntityReference)_oferta.Attributes["atos_tarifaid"]).Id);

                    logger.Info("Asocia pricinginput a oferta");

                    _pricingInput.Attributes["atos_ofertaid"] = new EntityReference("atos_oferta", _oferta.Id);
                    _pricingInput.Id = Guid.NewGuid();

                    logger.Info("Asocia pricinginput a terminodepricing " + ((TermPricing)formula.ComponentesFormula[i]).TermpricingId.ToString());

                    _pricingInput.Attributes["atos_terminodepricingid"] = new EntityReference("atos_terminodepricing", ((TermPricing)formula.ComponentesFormula[i]).TermpricingId);

                    logger.Info("Actualiza nombre de pricinginput");

                    if (((TermPricing)formula.ComponentesFormula[i]).NombreEms != "")
                        _pricingInput.Attributes["atos_name"] = ((TermPricing)formula.ComponentesFormula[i]).NombreEms + "-";
                    else
                        _pricingInput.Attributes["atos_name"] = formula.ComponentesFormula[i].NombreComponente + "-";
                    _pricingInput.Attributes["atos_name"] += _oferta.Attributes["atos_name"].ToString();

                    logger.Info(string.Format("Pricing Input Name: {0}", _pricingInput.Attributes["atos_name"].ToString()));

                    _pricingInput.Attributes["atos_tipo"] = new OptionSetValue(((TermPricing)formula.ComponentesFormula[i]).FijoIndexado.Value);

                    context.OrganizationService.Create(_pricingInput);

                    #endregion
                }
            }
            logger.Info("Princing Inputs creados");
        }

        /*-----------------------------*/
        /* CREATE_pricinginputCollection */
        /*-----------------------------*/
        private void CREATE_pricinginputCollection(ref Entity _oferta)
        {
            int count = 0;

            if (_oferta.Attributes.Contains("atos_tipodeproductofinalid") == false )
                return;
            if (_oferta.Attributes.Contains("atos_tipoproductofinalrevisado") == false )
                return;
            if (_oferta.Attributes.Contains("atos_pricinginputsrevisados") == false )
            {
                errores.Add("Debe revisar antes los Pricing Inputs");
                return;
            }

            Guid _tipoproductoid = ((EntityReference)_oferta.Attributes["atos_tipodeproductofinalid"]).Id;
            Entity _tipoProducto = context.OrganizationService.Retrieve("atos_tipodeproducto", _tipoproductoid, new ColumnSet(true));

            // Copiamos los pricinginput
            construyeFormulas(_tipoProducto);

            for (int i = 0; i < formula.ComponentesFormula.Count; i++)
            {
                if (formula.ComponentesFormula[i].TipoComponente == "TermPricing")
                {
                    //String msg = string.Empty;

                    EntityCollection _pricingInputColl = formula.pricingInputCollection(i, context.OrganizationService, _oferta, _oferta);

                    count = _pricingInputColl.Entities.Count;

                    for (int j = 0; j < count; j++)
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

                        // Fechas
                        if (_pricingInputbase.Attributes.Contains("atos_fechainiciovigencia"))
                            _pricingInput.Attributes["atos_fechainiciovigencia"] = DateTime.SpecifyKind((DateTime)_pricingInputbase.Attributes["atos_fechainiciovigencia"], DateTimeKind.Utc);
                        if (_pricingInputbase.Attributes.Contains("atos_fechainicioaplicacion"))
                            _pricingInput.Attributes["atos_fechainicioaplicacion"] = DateTime.SpecifyKind((DateTime)_pricingInputbase.Attributes["atos_fechainicioaplicacion"], DateTimeKind.Utc);
                        if (_pricingInputbase.Attributes.Contains("atos_fechafinvigencia"))
                            _pricingInput.Attributes["atos_fechafinvigencia"] = DateTime.SpecifyKind((DateTime)_pricingInputbase.Attributes["atos_fechafinvigencia"], DateTimeKind.Utc);
                        if (_pricingInputbase.Attributes.Contains("atos_fechafinaplicacion"))
                            _pricingInput.Attributes["atos_fechafinaplicacion"] = DateTime.SpecifyKind((DateTime)_pricingInputbase.Attributes["atos_fechafinaplicacion"], DateTimeKind.Utc);

                        // DependeDeSubSistemaElectrico
                        if (((TermPricing)formula.ComponentesFormula[i]).DependeDeSubSistemaElectrico == true)
                             _pricingInput.Attributes["atos_subsistemaid"] = new EntityReference("atos_subsistema", ((EntityReference)_oferta.Attributes["atos_subsistemaid"]).Id);
                        if (((TermPricing)formula.ComponentesFormula[i]).DependeDeSistemaElectrico == true)
                            _pricingInput.Attributes["atos_sistemaelectricoid"] = new EntityReference("atos_sistemaelectrico", ((EntityReference)_oferta.Attributes["atos_sistemaelectricoid"]).Id);

                        // DependeDeTarifa
                        if (((TermPricing)formula.ComponentesFormula[i]).DependeDeTarifa == true)
                            _pricingInput.Attributes["atos_tarifaid"] = new EntityReference("atos_tarifa", ((EntityReference)_oferta.Attributes["atos_tarifaid"]).Id);
                        if (((TermPricing)formula.ComponentesFormula[i]).DependeDePeaje == true)
                            _pricingInput.Attributes["atos_peajeid"] = new EntityReference("atos_tablasatrgas", ((EntityReference)_oferta.Attributes["atos_peajeid"]).Id);

                        // DEBUG Log.writelog("\tAsocia pricingInput a oferta", true);
                        _pricingInput.Attributes["atos_ofertaid"] = new EntityReference("atos_oferta", _oferta.Id);
                        _pricingInput.Id = Guid.NewGuid();
                        // DEBUG Log.writelog("\tAsocia pricingInput a terminodepricing", true);
                        _pricingInput.Attributes["atos_terminodepricingid"] = new EntityReference("atos_terminodepricing", ((TermPricing)formula.ComponentesFormula[i]).TermpricingId);
                        // DEBUG Log.writelog("\tActualiza nombre de pricingInput", true);

                        if (((TermPricing)formula.ComponentesFormula[i]).NombreEms != "")
                            _pricingInput.Attributes["atos_name"] = ((TermPricing)formula.ComponentesFormula[i]).NombreEms + "-";
                        else
                            _pricingInput.Attributes["atos_name"] = formula.ComponentesFormula[i].NombreComponente + "-";

                        _pricingInput.Attributes["atos_name"] += _oferta.Attributes["atos_name"].ToString() + "-" + (j + 1);

                        logger.Info("Pricing Input: " + _pricingInput.Attributes["atos_name"].ToString());

                        try
                        {
                            context.OrganizationService.Create(_pricingInput);
                        }
                        catch (Exception ex)
                        {
                            logger.Error("Exception:" + ex.Message);
                        }
                    }

                    //if (count > 0) logger.Info(string.Format("\t{0} Pricing Input creados", count));
                }
            }

            logger.Info("Pricing Input creados");
        }

        /*******************************/
        /* DELETE_producto             */
        /*******************************/
        private void DELETE_producto(ref Entity _oferta)
        {
            if (_oferta.Attributes.Contains("atos_tipodeproductofinalid"))
            {
                _oferta.Attributes["atos_tipodeproductofinalid"] = null;
                _oferta.Attributes["atos_tipoproductofinalrevisado"] = false;

                context.OrganizationService.Update(_oferta);

                logger.Info("Producto eliminado");
            }
        }

        /*-----------------------------*/
        /* DELETE_pricinginput         */
        /*-----------------------------*/
        private void DELETE_pricinginput(ref Entity _oferta)
        {
            String msg = String.Empty;

            if (_oferta.Attributes.Contains("atos_tipodeproductofinalid") == false)
                    return;

            msg = _oferta.Attributes.Contains("atos_name") ? _oferta.Attributes["atos_name"].ToString() : String.Empty;

            QueryExpression _query = new QueryExpression("atos_pricinginput");
            _query.ColumnSet = new ColumnSet(true);
            _query.ColumnSet.AddColumns("atos_pricinginputid");
          
            FilterExpression _filter = new FilterExpression(LogicalOperator.And);
            ConditionExpression _condition1 = new ConditionExpression("atos_ofertaid", ConditionOperator.Equal, _oferta.Id.ToString());
            ConditionExpression _condition2 = new ConditionExpression("atos_cierreofertaid", ConditionOperator.Null);
            ConditionExpression _condition3 = new ConditionExpression("atos_facturacionestimada", ConditionOperator.NotEqual, true);

            _filter.AddCondition(_condition1);
            _filter.AddCondition(_condition2);
            _filter.AddCondition(_condition3);

            _query.Criteria.AddFilter(_filter);
            _query.NoLock = true;

            

            /*
            QueryExpression _consulta = new QueryExpression("atos_pricinginput");
                _consulta.NoLock = true;
                _consulta.ColumnSet = new ColumnSet(true); 
            FilterExpression _filtro = new FilterExpression();
                _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
                _condicion.AttributeName = "atos_ofertaid";
                _condicion.Operator = ConditionOperator.Equal;
                _condicion.Values.Add(_oferta.Id.ToString());

                _filtro.Conditions.Add(_condicion);
                _condicion = new ConditionExpression(); // Excluimos pricing input del cierre de la oferta
                _condicion.AttributeName = "atos_cierreofertaid";
                _condicion.Operator = ConditionOperator.Null;

                _filtro.Conditions.Add(_condicion);
                _condicion = new ConditionExpression(); // Excluimos pricing input de facturacion estimada
                _condicion.AttributeName = "atos_facturacionestimada";
                _condicion.Operator = ConditionOperator.NotEqual;
                _condicion.Values.Add(true);

                _filtro.Conditions.Add(_condicion);
                _consulta.Criteria.AddFilter(_filtro);
                _consulta.ColumnSet.AddColumns("atos_pricinginputid");
            
                EntityCollection _resultado;
            */
            try
            {
                EntityCollection _resultado = context.OrganizationService.RetrieveMultiple(_query);
                //_resultado = context.OrganizationService.RetrieveMultiple(_consulta);

                int count = _resultado.Entities.Count;

                for (int i = 0; i < count; i++)
                {
                    Guid _pricinginputid = (Guid)_resultado.Entities[i].Attributes["atos_pricinginputid"];
                    context.OrganizationService.Delete("atos_pricinginput", _pricinginputid);
                }

                if (count > 0) logger.Info(string.Format("\t{0} Pricing Output eliminados", count));

                _oferta.Attributes["atos_pricinginputsrevisados"] = false;
                context.OrganizationService.Update(_oferta);
            }
            catch (Exception ex)
            {
                logger.Info("Exception:" + ex.Message);
            }
            
            //try
            //{
            //    _oferta.Attributes["atos_pricinginputsrevisados"] = false;
            //    context.OrganizationService.Update(_oferta);
            //}
            //catch (Exception ex)
            //{
            //    Log.writelog("Exception:" + ex.Message, true);
            //}
        }

        /*-----------------------------*/
        /* DELETE_pricingoutput        */
        /*-----------------------------*/
        private void DELETE_pricingoutput(ref Entity _oferta)
        {
            String msg = String.Empty;

            QueryExpression _query = new QueryExpression("atos_pricingoutput");
            _query.ColumnSet = new ColumnSet(true);
            _query.ColumnSet.AddColumns("atos_pricingoutputid");

            FilterExpression _filter = new FilterExpression(LogicalOperator.And);
            ConditionExpression _condition1 = new ConditionExpression("atos_ofertaid", ConditionOperator.Equal, _oferta.Id.ToString());            
            ConditionExpression _condition2 = new ConditionExpression("atos_facturacionestimada", ConditionOperator.NotEqual, true);            

            _filter.AddCondition(_condition1);
            _filter.AddCondition(_condition2);

            _query.Criteria.AddFilter(_filter);
            _query.NoLock = true;

            /*
            QueryExpression _consulta = new QueryExpression("atos_pricingoutput");
                _consulta.NoLock = true;
            FilterExpression _filtro = new FilterExpression();
                _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
                _condicion.AttributeName = "atos_ofertaid";
                _condicion.Operator = ConditionOperator.Equal;
                _condicion.Values.Add(_oferta.Id.ToString());
            // DEBUG Log.writelog("Eliminar Id Oferta " + _oferta.Id.ToString());
                _filtro.Conditions.Add(_condicion);
                _condicion = new ConditionExpression(); // Excluimos pricing output de facturacion estimada
                _condicion.AttributeName = "atos_facturacionestimada";
                _condicion.Operator = ConditionOperator.NotEqual;
                _condicion.Values.Add(true);
            //Log.writelog("Facturacion estimada no true");
                _filtro.Conditions.Add(_condicion);
                _consulta.Criteria.AddFilter(_filtro);
                _consulta.ColumnSet.AddColumns("atos_pricingoutputid");
                */
            EntityCollection _resultado = context.OrganizationService.RetrieveMultiple(_query);

            int count = _resultado.Entities.Count;

            //msg = _oferta.Attributes.Contains("atos_name") ? _oferta.Attributes["atos_name"].ToString() : String.Empty;
            //if (count > 0) Log.writelog(string.Format("\tHay {0} Pricing Outputs a eliminar de la Oferta: {1}-{2}", count, msg, _oferta.Id.ToString()), true);

            for (int i = 0; i < count; i++)
            {
                Guid _pricingoutputid = (Guid)_resultado.Entities[i].Attributes["atos_pricingoutputid"];
                //msg = _resultado.Entities[i].Attributes.Contains("atos_name") ? _resultado.Entities[i].Attributes["atos_name"].ToString() : String.Empty;
                //Log.writelog(string.Format("\t\tPricing Output: {0}-{1} eliminado", i, msg), true);
                context.OrganizationService.Delete("atos_pricingoutput", _pricingoutputid);
            }

            if (count > 0) logger.Info(string.Format("{0} Pricing Output eliminados", count));
        }

        /*-----------------------------*/
        /* Filter_OfertasHijas         */
        /*-----------------------------*/
        private EntityCollection Filter_OfertasHijas(Entity ofertaPadre, LocalPluginContext context)
        {
            QueryExpression _query = new QueryExpression("atos_oferta");
            _query.ColumnSet = new ColumnSet(true);
            _query.ColumnSet.AddColumns("atos_tipooferta", "atos_tipodeproductofinalid");

            FilterExpression _filter = new FilterExpression(LogicalOperator.And);
            ConditionExpression _condition = new ConditionExpression("atos_ofertapadreid", ConditionOperator.Equal, ofertaPadre.Id.ToString());

            _filter.AddCondition(_condition);

            _query.Criteria.AddFilter(_filter);
            _query.NoLock = true;

            /*
            QueryExpression _consulta = new QueryExpression("atos_oferta");
                _consulta.NoLock = true;
                _consulta.ColumnSet = new ColumnSet(true); 
            FilterExpression _filtro = new FilterExpression();  
                _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
                _condicion.AttributeName = "atos_ofertapadreid";
                _condicion.Operator = ConditionOperator.Equal;
                _condicion.Values.Add(ofertaPadre.Id.ToString());
            //Log.writelog(string.Format("\t\tRecupera Id Oferta: {0}", ofertaPadre.Id.ToString()), true);
                _filtro.Conditions.Add(_condicion);
                _consulta.Criteria.AddFilter(_filtro);
                _consulta.ColumnSet.AddColumns("atos_tipooferta", "atos_tipodeproductofinalid");*/

            return context.OrganizationService.RetrieveMultiple(_query);
        }

        /*-----------------------------*/
        /* Filter_OfertaStaging        */
        /*-----------------------------*/
        private EntityCollection Filter_OfertaStaging(Entity oferta, LocalPluginContext context)
        {
            QueryExpression _query = new QueryExpression("atos_bpf_1c3d1c4af29543429ee2b7465a2e6ee8");
            _query.ColumnSet = new ColumnSet(true);
            _query.ColumnSet.AddColumns("businessprocessflowinstanceid", "activestageid", "traversedpath", "processid");

            FilterExpression _filter = new FilterExpression(LogicalOperator.And);
            ConditionExpression _condition = new ConditionExpression("bpf_atos_ofertaid", ConditionOperator.Equal, oferta.Id.ToString());

            _filter.AddCondition(_condition);

            _query.Criteria.AddFilter(_filter);
            _query.NoLock = true;

            /*
            QueryExpression _consulta = new QueryExpression("atos_bpf_1c3d1c4af29543429ee2b7465a2e6ee8");
                _consulta.NoLock = true;
            FilterExpression _filtro = new FilterExpression();
                _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
                _condicion.AttributeName = "bpf_atos_ofertaid";
                _condicion.Operator = ConditionOperator.Equal;
                _condicion.Values.Add(oferta.Id.ToString());
                _filtro.Conditions.Add(_condicion);
                _consulta.Criteria.AddFilter(_filtro);
                _consulta.ColumnSet.AddColumns("businessprocessflowinstanceid", "activestageid", "traversedpath", "processid");
            */
            //Log.writelog("\t\tRecupera los Pricing de Ofertas");

            return context.OrganizationService.RetrieveMultiple(_query);
        }

        /*-----------------------------*/
        /* UPDATE_OfertaStaging        */
        /*-----------------------------*/
        private void UPDATE_OfertaStaging(Entity preOferta, Entity ofertaPadre, Entity ofertaPricing, LocalPluginContext context, bool cambiaproducto = false, bool cambiaTipoCalculo = false)
        {
            String msg = String.Empty;
            
            if (((OptionSetValue)ofertaPadre.Attributes["atos_tipooferta"]).Value == OFERTA)
                return;

            Entity stage = context.OrganizationService.Retrieve("processstage", ((EntityReference)ofertaPricing.Attributes["activestageid"]).Id, new ColumnSet(true));
            //logger.Info("\t\tStage (PADRE) activestageid " + stage.Id.ToString());

            EntityReference processid;

            if (ofertaPricing.Attributes.Contains("proccessid"))
                processid = (EntityReference)ofertaPricing.Attributes["processid"];
            else
                processid = (EntityReference)preOferta.Attributes["processid"];

            EntityCollection _resultado = Filter_OfertasHijas(ofertaPadre, context);

            Guid _tipoproductoid = Guid.Empty;
            if (cambiaproducto)
            {
                if (ofertaPadre.Attributes.Contains("atos_tipodeproductoid"))
                    _tipoproductoid = ((EntityReference)ofertaPadre.Attributes["atos_tipodeproductoid"]).Id;
            }

            Object _precisiondecimalesoferta = null;
            if (ofertaPadre.Attributes.Contains("atos_precisiondecimalesoferta"))
                _precisiondecimalesoferta = ofertaPadre.Attributes["atos_precisiondecimalesoferta"];
            
            OptionSetValue optTipoCalculoPromedio = new OptionSetValue(300000000);
            OptionSetValue optTipoCalculoDirecto  = new OptionSetValue(300000001);

            int count = _resultado.Entities.Count;

            //if (count > 0) Log.writelog(string.Format("\tHay ({0}) subofertas", count), true);

            for (int i = 0; i < count; i++)
            {
                Entity _ofertaHija = new Entity("atos_oferta");
                _ofertaHija.Id = _resultado.Entities[i].Id;
                // logger.Info(string.Format("\t\tOferta hija: {0}{1} - {2}", i, _resultado.Entities[i]["atos_name"], _ofertaHija.Id), true);
                EntityCollection ofertasPricing = Filter_OfertaStaging(_resultado.Entities[i], context);

                foreach (Entity op in ofertasPricing.Entities)
                {
                    Entity opAct = new Entity("atos_bpf_1c3d1c4af29543429ee2b7465a2e6ee8");
                    opAct.Id = op.Id;

                    opAct.Attributes["activestageid"] = ofertaPricing.Attributes["activestageid"];
                    opAct.Attributes["processid"] = processid;

                    if (ofertaPricing.Attributes.Contains("traversedpath"))
                    {
                        logger.Info(string.Format("\tENTRA 1 :traversedpath: {0}", ofertaPricing.Attributes["traversedpath"].ToString()));
                        opAct.Attributes["traversedpath"] = ofertaPricing.Attributes["traversedpath"];
                    }
                    else
                    {
                        logger.Info(string.Format("\tENTRA 2 :traversedpath: {0}", ((EntityReference)ofertaPricing.Attributes["activestageid"]).Id.ToString()));
                        opAct.Attributes["traversedpath"] = ((EntityReference)ofertaPricing.Attributes["activestageid"]).Id.ToString();
                    }
                    logger.Info("Id:"  + op.Id);
                    logger.Info(string.Format("\t\tId: {0}", opAct.Id));

                    try
                    {
                        context.OrganizationService.Update(opAct);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Exception:" + ex.Message);
                    }
                }

                if (_precisiondecimalesoferta != null)
                    _ofertaHija.Attributes["atos_precisiondecimalesoferta"] = _precisiondecimalesoferta;

                if (cambiaproducto)
                {
                    if (_tipoproductoid != Guid.Empty)
                        _ofertaHija.Attributes["atos_tipodeproductoid"] = new EntityReference("atos_tipodeproducto", _tipoproductoid);

                    if (ofertaPadre.Attributes.Contains("atos_tipodeproductofinalid"))
                        _ofertaHija.Attributes["atos_tipodeproductofinalid"] = new EntityReference("atos_tipodeproducto", ((EntityReference)ofertaPadre.Attributes["atos_tipodeproductofinalid"]).Id);
                }
                else
                {
                    if (ofertaPadre.Attributes.Contains("atos_tipoproductofinalrevisado"))
                        _ofertaHija.Attributes["atos_tipoproductofinalrevisado"] = ofertaPadre.Attributes["atos_tipoproductofinalrevisado"];
                }

                if (cambiaTipoCalculo)
                {
                    if (tipoCalculoPromedio)
                        _ofertaHija.Attributes["atos_tipodecalculo"] = optTipoCalculoPromedio;
                    else
                        _ofertaHija.Attributes["atos_tipodecalculo"] = optTipoCalculoDirecto;

                    if (ofertaPadre.Attributes.Contains("atos_pricinginputsrevisados"))
                        _ofertaHija.Attributes["atos_pricinginputsrevisados"] = ofertaPadre.Attributes["atos_pricinginputsrevisados"];
                }

                try
                {
                    context.OrganizationService.Update(_ofertaHija);
                }
                catch (Exception ex)
                {
                    logger.Error("Exception:" + ex.Message);
                }
                
                if (((OptionSetValue)ofertaPadre.Attributes["atos_tipooferta"]).Value == MULTIPUNTO &&
                    ((OptionSetValue)_resultado.Entities[i].Attributes["atos_tipooferta"]).Value == SUBOFERTA)
                {
                    EntityCollection _hijas = Filter_OfertasHijas(_resultado.Entities[i], context);

                    int count_hija = _hijas.Entities.Count;

                    //if (countJ > 0) Log.writelog(string.Format("\t\tHay {0} Ofertas", countJ), true);

                    for (int j = 0; j < count_hija; j++)
                    {
                        _ofertaHija = new Entity("atos_oferta");
                        _ofertaHija.Id = _hijas.Entities[j].Id;

                        //logmsg.Append(string.Format("\t\t\t\t\t\tOferta: {0}-{1}", j, _hijas.Entities[j]["atos_name"]));

                        foreach (Entity op in ofertasPricing.Entities)
                        {
                            Entity opAct = new Entity("atos_bpf_1c3d1c4af29543429ee2b7465a2e6ee8");
                            opAct.Id = op.Id;

                            opAct.Attributes["activestageid"] = ofertaPricing.Attributes["activestageid"];
                            opAct.Attributes["processid"] = processid;

                            if (ofertaPricing.Attributes.Contains("traversedpath"))
                                opAct.Attributes["traversedpath"] = ofertaPricing.Attributes["traversedpath"];
                            else
                                opAct.Attributes["traversedpath"] = ((EntityReference)ofertaPricing.Attributes["activestageid"]).Id.ToString();

                            context.OrganizationService.Update(opAct);
                        }

                        if (cambiaproducto)
                        {
                            if (_tipoproductoid != Guid.Empty)
                                _ofertaHija.Attributes["atos_tipodeproductoid"] = new EntityReference("atos_tipodeproducto", _tipoproductoid);

                            if (ofertaPadre.Attributes.Contains("atos_tipodeproductofinalid"))
                                _ofertaHija.Attributes["atos_tipodeproductofinalid"] = new EntityReference("atos_tipodeproducto", ((EntityReference)ofertaPadre.Attributes["atos_tipodeproductofinalid"]).Id);

                            if (ofertaPadre.Attributes.Contains("atos_tipoproductofinalrevisado"))
                                _ofertaHija.Attributes["atos_tipoproductofinalrevisado"] = ofertaPadre.Attributes["atos_tipoproductofinalrevisado"];
                        }
                        else
                        {
                            if (ofertaPadre.Attributes.Contains("atos_tipoproductofinalrevisado"))
                                _ofertaHija.Attributes["atos_tipoproductofinalrevisado"] = ofertaPadre.Attributes["atos_tipoproductofinalrevisado"];
                        }

                        if (cambiaTipoCalculo)
                        {
                            if (tipoCalculoPromedio)
                                _ofertaHija.Attributes["atos_tipodecalculo"] = optTipoCalculoPromedio;
                            else
                                _ofertaHija.Attributes["atos_tipodecalculo"] = optTipoCalculoDirecto;

                            if (ofertaPadre.Attributes.Contains("atos_pricinginputsrevisados"))
                                _ofertaHija.Attributes["atos_pricinginputsrevisados"] = ofertaPadre.Attributes["atos_pricinginputsrevisados"];
                           
                        }

                        try
                        {
                            context.OrganizationService.Update(_ofertaHija);
                        }
                        catch (Exception ex)
                        {
                            logger.Error("Exception:" + ex.Message);
                        }                        
                    }
                }
            }

            logger.Info("Oferta cambia de etapa de proceso");
        }

        private void DELETE_Productohijas(Entity preOferta, Entity ofertaPadre, Entity ofertaPricing, LocalPluginContext context, Guid productofinalId)
        {
            if (((OptionSetValue)ofertaPadre.Attributes["atos_tipooferta"]).Value == OFERTA)
                return;

            // Recupera stage padre",true);
            Entity stage = context.OrganizationService.Retrieve("processstage", 
                          ((EntityReference)ofertaPricing.Attributes["activestageid"]).Id, new ColumnSet(true));

            EntityReference processid;
            if (ofertaPricing.Attributes.Contains("proccessid"))
                processid = (EntityReference)ofertaPricing.Attributes["processid"];
            else
                processid = (EntityReference)preOferta.Attributes["processid"];

            // Recupera ofertas hijas
             EntityCollection _resultado = Filter_OfertasHijas(ofertaPadre, context);

            int count = _resultado.Entities.Count;

            for (int i = 0; i < count; i++)
            {
                Entity _ofertaHija = new Entity("atos_oferta");
                _ofertaHija.Id = _resultado.Entities[i].Id;                
                //Log.writelog(string.Format("\t\tOferta hija: {0}-{1}", i, _resultado.Entities[i]["atos_name"]), true);
                EntityCollection ofertasPricing = Filter_OfertaStaging(_resultado.Entities[i], context);

                foreach (Entity op in ofertasPricing.Entities)
                {
                    Entity opAct = new Entity("atos_bpf_1c3d1c4af29543429ee2b7465a2e6ee8");
                    opAct.Id = op.Id;

                    opAct.Attributes["activestageid"] = ofertaPricing.Attributes["activestageid"];
                    opAct.Attributes["processid"] = processid;

                    if (ofertaPricing.Attributes.Contains("traversedpath"))
                        opAct.Attributes["traversedpath"] = ofertaPricing.Attributes["traversedpath"];
                    else
                        opAct.Attributes["traversedpath"] = ((EntityReference)ofertaPricing.Attributes["activestageid"]).Id.ToString();

                    context.OrganizationService.Update(opAct);
                }
            }

            if (count > 0) logger.Info(string.Format("{0} Productos de las ofertas hijas eliminadas", count));
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

            QueryByAttribute _consulta = new QueryByAttribute("atos_matrizportarifaysistemaelectrico");
            _consulta.ColumnSet = new ColumnSet(new String[] { "atos_matrizhorariaid" });
            _consulta.AddAttributeValue("atos_tarifaid", _tarifaid.ToString());
            _consulta.AddAttributeValue("atos_sistemaelectricoid", _sistemaelectricoid.ToString());

            EntityCollection _resConsulta = context.OrganizationService.RetrieveMultiple(_consulta);

            if (_resConsulta.Entities.Count > 0)
                _matrizhorariaid = ((EntityReference)_resConsulta.Entities[0].Attributes["atos_matrizhorariaid"]).Id;

            logger.Info("Matriz horaria aplicada");

            return _matrizhorariaid;
        }

        #region Class LocalPluginContext

        protected class LocalPluginContext
        {
            internal IServiceProvider ServiceProvider
            {
                get;

                private set;
            }

            internal IOrganizationService OrganizationService
            {
                get;

                private set;
            }

            internal IPluginExecutionContext PluginExecutionContext
            {
                get;

                private set;
            }

            internal ITracingService TracingService
            {
                get;

                private set;
            }

            private LocalPluginContext()
            {
            }

            internal LocalPluginContext(IServiceProvider serviceProvider)
            {
                if (serviceProvider == null)
                {
                    throw new ArgumentNullException("serviceProvider");
                }

                // Obtain the execution context service from the service provider.
                this.PluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                // Obtain the tracing service from the service provider.
                this.TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                // Obtain the Organization Service factory service from the service provider
                IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                // Use the factory to generate the Organization Service.
                this.OrganizationService = factory.CreateOrganizationService(this.PluginExecutionContext.UserId);
            }

           /*
            internal void Trace(string message)
            {

                if (string.IsNullOrWhiteSpace(message) || this.TracingService == null)
                {
                    return;
                }

                if (this.PluginExecutionContext == null)
                {
                    this.TracingService.Trace(message);
                }
                else
                {
                    this.TracingService.Trace(
                        "{0}, Correlation Id: {1}, Initiating User: {2}",
                        message,
                        this.PluginExecutionContext.CorrelationId,
                        this.PluginExecutionContext.InitiatingUserId);
                }
            } */
        }

        #endregion

    }
}