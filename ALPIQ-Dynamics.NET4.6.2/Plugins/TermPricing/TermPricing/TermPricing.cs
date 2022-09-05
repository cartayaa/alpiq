/*
 * Copyright (c) 2015 All Rights Reserved Atos Spain
 * Project: TermPricing.org 
 * Program: TermPricing.cs
 * Implements the Plugin Workflow Activity.
 * 
 * CreateOn 4/15/2015 5:34:31 PM
 * Autor            Version     Descricion
 * Andres Cartaya   1108-2022   Ultima actualizacion
 *                              Comentarios de algunos Traces.Log
 *                              
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


        /**
        // <summary>
        // Executes the plug-in.
        // </summary>
        // <param name="serviceProvider">The service provider.</param>
        // <remarks>
        // Dependiendo de la fase debe hacer lo siguiente:
        // - Copia del producto base
        // - Clonar los pricing inputs asociados a la oferta
        // - Calcular los pricing output (hay que meter también pricing output para la fórmula final y para las variables intermedias, en estos casos el pricing output no irá relacionado con un término de pricing)
        // </remarks>
         */
        public void Execute(IServiceProvider serviceProvider)
        {
            tipoCalculoPromedio = false;
            bool debug = true;      // Debug
            int TipoOferta = 0;     // TipoOferta del Pabre
            Entity _oferta;         // Oferta Padre
            Entity processStage;    // Fase de proceso
            int stagecategory;      // Stage Category Id
            esGas = false;

            errores.Clear();
      
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            context = new LocalPluginContext(serviceProvider); // Construct the Local plug-in context.
            Log.tracingService = context.TracingService;

            /* 1108-2022 -1 */
            //Log.writelog("TermPricing.Execute: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(), debug);

            try
            {
                #region Definiciones Target - PreImage

                if (context.PluginExecutionContext.MessageName != "Update")
                {
                    /* 1108-2022 -1 */
                    //Log.writelog("== END ==", debug);
                    return;
                }

                Entity efpre = (Entity)context.PluginExecutionContext.PreEntityImages["PreEntityImage"]; // ORIGEN

                //AC Log.writelog("\nPRE-IMAGE", debug);
                //AC Log.writelog(string.Format("\tLogicalName: {0}", efpre.LogicalName), debug);
                //AC Log.writelog(string.Format("\tbpf_name: {0}", efpre.Contains("bpf_name")), debug);
                //AC Log.writelog(string.Format("\tbpf_atos_ofertaid: {0} - {1}", efpre.Contains("bpf_atos_ofertaid"), ((EntityReference)efpre.Attributes["bpf_atos_ofertaid"]).Id), debug);
                //AC Log.writelog(string.Format("\tactivestageid: {0} - {1}", efpre.Contains("activestageid"), ((EntityReference)efpre.Attributes["activestageid"]).Id), debug);
                //AC Log.writelog(string.Format("\tstatecode: {0}", efpre.Contains("statecode")), debug);
                //AC Log.writelog(string.Format("\tstatuscode: {0}", efpre.Contains("statuscode")), debug);

                Entity _ofertaPricing = (Entity)context.PluginExecutionContext.InputParameters["Target"]; // DESTINO

                //AC Log.writelog("\nTARGET", debug);
                string s_activeStageId = _ofertaPricing.Attributes.Contains("activestageid") ? _ofertaPricing.Attributes["activestageid"].ToString() : string.Empty;

                //AC Log.writelog(string.Format("\tLogicalName: {0}", _ofertaPricing.LogicalName), true);
                //AC Log.writelog(string.Format("\tbpf_name: {0}", _ofertaPricing.Contains("bpf_name")), true);
                //AC Log.writelog(string.Format("\tbpf_atos_ofertaid: {0}", _ofertaPricing.Contains("bpf_atos_ofertaid")), true);
                //AC Log.writelog(string.Format("\tactivestageid: {0} - {1}", _ofertaPricing.Contains("activestageid"), s_activeStageId), debug);
                //AC Log.writelog(string.Format("\tstatecode: {0}", _ofertaPricing.Contains("statecode")), true);
                //AC Log.writelog(string.Format("\tstatuscode: {0}", _ofertaPricing.Contains("statuscode")), true);

                if (_ofertaPricing.LogicalName != "atos_bpf_1c3d1c4af29543429ee2b7465a2e6ee8")
                {
                    /* 1108-2022 -1 */
                    // Log.writelog("== END ==", debug);
                    return;
                }

                if (_ofertaPricing.Attributes.Contains("activestageid") == false)
                {
                    /* 1108-2022 -1 */
                    // Log.writelog("== END ==", debug);
                    return;
                }

                /* 1108-2022 -1 */
                //Log.writelog("---------------------------------------------------------------", debug);

                #endregion

                #region Validaciones de Target - PreImage

                if (context.PluginExecutionContext.PreEntityImages["PreEntityImage"] == null)
                {
                    string msg1 = "No viene información de la entidad antes de los cambios";
                    context.Trace(string.Format(CultureInfo.InvariantCulture, msg1));
                    Log.writelog(string.Format(CultureInfo.InvariantCulture, msg1), debug);
                    throw new InvalidPluginExecutionException(OperationStatus.Canceled, msg1);
                    /* 1108-2022 -1 */
                    //Log.writelog("== END ==", debug);
                    //return;
                }

                if (efpre.Attributes.Contains("bpf_atos_ofertaid") == false)
                    return;

                _oferta = context.OrganizationService.Retrieve("atos_oferta", ((EntityReference)efpre.Attributes["bpf_atos_ofertaid"]).Id, new ColumnSet(true));
                TipoOferta = ((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value;
                //AC Log.writelog(string.Format("\tOferta Padre: {0}", _oferta.Attributes["atos_name"]), true);
                string msg = _oferta.Attributes.Contains("atos_name") ? _oferta.Attributes["atos_name"].ToString() : string.Empty;
                //AC Log.writelog(string.Format("\t\tEliminando el Pricing Output: {0} - {1} {2}", msg), true);
                /* 1108-2022 +1 */
                // Log.writelog(string.Format("Oferta: {0}", msg), true);

                try
                {
                    //AC Log.writelog("\tObtiene (activestageid) del (processStage)", debug);
                    /*TARGET*/
                    processStage = context.OrganizationService.Retrieve("processstage", ((EntityReference)_ofertaPricing.Attributes["activestageid"]).Id, new ColumnSet(true));
                    //stagecategory = ((OptionSetValue)processStage.Attributes["stagecategory"]).Value;
                }

                catch (Exception ex)
                {
                    Log.writelog("Exception:" + ex.Message, true);
                    throw new InvalidPluginExecutionException(OperationStatus.Canceled, ex.Message);
                    //throw ex;
                }

                if (efpre.Attributes.Contains("activestageid") == true)
                {
                    //AC Log.writelog("\tRecupera processStagePre", debug);
                    /*PRE-IMAGE*/
                    Entity processStagePre = context.OrganizationService.Retrieve( "processstage", 
                                                                                   ((EntityReference)efpre.Attributes["activestageid"]).Id, 
                                                                                   new ColumnSet(true));

                    if (((OptionSetValue)processStagePre.Attributes["stagecategory"]).Value == ((OptionSetValue)processStage.Attributes["stagecategory"]).Value)
                    {
                        /*
                         * string label = processStagePre.FormattedValues["stagecategory"].ToString();

                        Log.writelog(string.Format("(PRE-IMAGE) processStage --> stagecategory: {0} - {1}",
                                                   ((OptionSetValue)processStagePre.Attributes["stagecategory"]).Value,
                                                   label), debug);

                        label = processStage.FormattedValues["stagecategory"].ToString();

                        Log.writelog(string.Format("(TARGET) processStage --> stagecategory: {0} - {1}",
                                                   ((OptionSetValue)processStage.Attributes["stagecategory"]).Value,
                                                   label), debug);
                        */
                        /* 1108-2022 -1 */
                        // Log.writelog("== END ==", debug);
                        return;
                    }
                }
                #endregion

                #region Commodity

                /* 1108-2022 +1 */
                Log.writelog("TermPricing.Execute: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(), debug);
                esGas = false; // Pricing Gas

                // Pricing gas
                if (_oferta.Attributes.Contains("atos_commodity"))
                {
                    if (((OptionSetValue)_oferta.Attributes["atos_commodity"]).Value == GAS)
                        esGas = true;
                    /* 1108-2022 +2 */
                    //if (esGas)  Log.writelog("\tCommodity: Gas: " + esGas.ToString(), debug);
                    //else Log.writelog("\tCommodity: Power: " + esGas.ToString(), debug);
                }
                else if (efpre.Attributes.Contains("atos_commodity"))
                {
                    if (((OptionSetValue)efpre.Attributes["atos_commodity"]).Value == GAS)
                        esGas = true;
                    /* 1108-2022 -1 */
                    //Log.writelog("\tCommodity: Gas: " + esGas.ToString(), debug);
                    /* 1108-2022 +2 */
                    //if (esGas) Log.writelog("\tCommodity: Gas: " + esGas.ToString(), debug);
                    //else Log.writelog("\tCommodity: Power: " + esGas.ToString(), debug);
                }

                /* 1108-2022 +1 */
                Log.writelog(string.Format("Oferta: {0} Commodity: {1}", msg, esGas ? "Gas" : "Power"), true);

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
                    context.Trace(string.Format(CultureInfo.InvariantCulture, "La oferta no tiene informado el peaje"));
                    Log.writelog(string.Format(CultureInfo.InvariantCulture, "\tLa oferta no tiene informado el peaje"));
                    /* 1108-2022 +2 */
                    Log.writelog("La oferta no tiene informado el peaje", true);
                    throw new InvalidPluginExecutionException(OperationStatus.Canceled, "La oferta no tiene informado el peaje");
                    /* 1108-2022 -2 */
                    // Log.writelog("== END ==", debug);
                    // return; 
                }
                // Fin Pricing gas

                #endregion

                /*-------------------------------------
                 * Estados
                 *  Identificar
                 *  Proponer
                 *  Desarrollar
                 *  3-Cerrar
                 *-------------------------------------*/

                stagecategory = ((OptionSetValue)processStage.Attributes["stagecategory"]).Value;

                switch (stagecategory)
                {
                    case IDENTIFICAR:
                        {
                            Log.writelog("Identificar - Seleccion De Producto Base", true);

                            Guid _productofinalid = Guid.Empty;

                            if (_oferta.Attributes.Contains("atos_tipodeproductofinalid"))
                                _productofinalid = ((EntityReference)_oferta.Attributes["atos_tipodeproductofinalid"]).Id;

                            if (_productofinalid != Guid.Empty && ((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value != OFERTA)
                                DELETE_Productohijas(efpre, _oferta, _ofertaPricing, context, _productofinalid);

                            DELETE_producto(ref _oferta);
                            /* 01208-2022 -1 */
                            // Log.writelog("== END ==", debug);
                        }
                        return;

                    case PROPONER:
                        {
                            Log.writelog("Proponer - Creación y Revisión de Producto Oferta", true);

                            _oferta.Attributes["atos_tipoproductofinalrevisado"] = false;

                            DELETE_pricinginput(ref _oferta);

                            if (efpre.Attributes.Contains("activestageid") == true)
                            {
                                Entity processStagePrev = context.OrganizationService.Retrieve("processstage", ((EntityReference)efpre.Attributes["activestageid"]).Id, new ColumnSet(true));

                                //Log.writelog("\tFin processStagePrev--->" + ((OptionSetValue)processStagePrev.Attributes["stagecategory"]).Value.ToString(), true);

                                //if (((OptionSetValue)processStagePrev.Attributes["stagecategory"]).Value == 4) // IDENTIFICAR
                                if (((OptionSetValue)processStagePrev.Attributes["stagecategory"]).Value == IDENTIFICAR)
                                {
                                    // Estamos retrocediendo

                                    //Log.writelog("stagecategory == 2--> llega 1", true);

                                    //if (_oferta.Attributes.Contains("atos_ofertapadreid") &&
                                    //    !(((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value == 300000001 &&
                                    //      (_oferta.Attributes.Contains("atos_tipodeproductoid")) &&
                                    //      !_oferta.Attributes.Contains("atos_tipodeproductofinalid")))

                                    TipoOferta = ((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value;

                                    if (_oferta.Attributes.Contains("atos_ofertapadreid") &&
                                        !(TipoOferta == SUBOFERTA &&
                                          (_oferta.Attributes.Contains("atos_tipodeproductoid")) &&
                                          !_oferta.Attributes.Contains("atos_tipodeproductofinalid")))
                                    {
                                        // Las ofertas hijas en las multipunto se actualiza el primer paso por Workflow.
                                        // Las subofertas se puede modificar el producto final
                                        /* 1108-2022 -1 */
                                        //Log.writelog("== END ==", debug);
                                        return;
                                    }

                                    // Actualiza la oferta desasociando el producto base
                                    DELETE_producto(ref _oferta);

                                    // Actualiza la oferta con el nuevo produto
                                    ADD_producto(ref _oferta); // efpre.Attributes["atos_name"].ToString());

                                    TipoOferta = ((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value;

                                    //if (((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value == 300000000 ||
                                    //    ((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value == 300000001)
                                    if (TipoOferta == MULTIPUNTO || TipoOferta == SUBOFERTA)
                                    {
                                        // Si es multipunto actualiza stage de las hijas (también las subofertas)
                                        UPDATE_PasoHijas(efpre, _oferta, _ofertaPricing, context, true);
                                    }
                                }
                            }

                            /* 01208-2022 -1 */
                            // Log.writelog("== END ==", debug);
                        }
                        return;

                    case DESARROLLAR:
                        {
                            Log.writelog("Desarrollar - Creación y Revisión de Pricings Inputs", true);

                            _oferta.Attributes["atos_pricinginputsrevisados"] = false;
                            _oferta.Attributes["atos_tipodecalculo"] = null;

                            DELETE_pricingoutput(ref _oferta);

                            if (efpre.Attributes.Contains("activestageid") == true)
                            {
                                Entity processStagePrev = context.OrganizationService.Retrieve("processstage",
                                                                                                ((EntityReference)efpre.Attributes["activestageid"]).Id,
                                                                                                new ColumnSet(true));

                                if (((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value == MULTIPUNTO)
                                {
                                    // Si es multipunto actualiza stage de las hijas
                                    UPDATE_PasoHijas(efpre, _oferta, _ofertaPricing, context);
                                }
                                //Log.writelog("stagecategory == 1-->llega 1 ", true);

                                if (((OptionSetValue)processStagePrev.Attributes["stagecategory"]).Value == PROPONER) // 2
                                {
                                    // Estamos retrocediendo
                                    //Log.writelog("stagecategory == 1-->llega 2 ", true);
                                    // Pricing gas
                                    if ((esGas && peajeGasId == Guid.Empty) ||
                                         (!esGas && (_oferta.Attributes.Contains("atos_tarifaid") == false ||
                                        _oferta.Attributes.Contains("atos_sistemaelectricoid") == false ||
                                        _oferta.Attributes.Contains("atos_subsistemaid") == false)))
                                    {
                                        /* 1108-2022 -1 */
                                        // Log.writelog("\n== END ==", debug);
                                        return;
                                    }

                                    TipoOferta = ((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value;

                                    //if (((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value == 300000002 &&
                                    if (_oferta.Attributes.Contains("atos_ofertapadreid") && TipoOferta == OFERTA)
                                    {
                                        // Si es oferta hija no hace nada
                                        /* 1108-2022 -1 */
                                        // Log.writelog("\n== END ==", debug);
                                        return;
                                    }

                                    // Avanzamos un stage
                                    //Log.writelog("stagecategory == 1-->llega 3 ", true);
                                    DELETE_pricinginput(ref _oferta);

                                    //Log.writelog("CREATE_pricinginputCollection", true);
                                    CREATE_pricinginputCollection(ref _oferta);

                                    TipoOferta = ((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value;

                                    // if (((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value == 300000001)
                                    if (TipoOferta == SUBOFERTA)
                                    {
                                        // Si es suboferta actualiza stage de las hijas
                                        UPDATE_PasoHijas(efpre, _oferta, _ofertaPricing, context);
                                    }
                                    //throw new Exception("Error provocado");
                                }
                            }

                            context.OrganizationService.Update(_oferta);
                            /* 01208-2022 -1 */
                            // Log.writelog("== END ==", debug);
                        }
                        return;
  
                    case CERRAR:
                        {
                            Log.writelog("Cerrar - Calcular Pricing Outputs", true);

                            TipoOferta = ((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value;

                            if (_oferta.Attributes.Contains("atos_tipodecalculo"))
                            {
                                if (TipoOferta == MULTIPUNTO)
                                {
                                    // Tipo de calculo promedio
                                    tipoCalculoPromedio = true;
                                }
                            }

                            if (esGas) // Pricing Gas
                            {
                                // En gas el tipo de calculo siempre es directo
                                tipoCalculoPromedio = false;
                            }

                            Log.writelog("tipoCalculoPromedio: " + tipoCalculoPromedio.ToString());

                            //if (((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value == 300000000)
                            if (TipoOferta == MULTIPUNTO)
                            {
                                // Si es multipunto actualiza stage de las hijas
                                UPDATE_PasoHijas(efpre, _oferta, _ofertaPricing, context, false, true);
                            }

                            if ((esGas && peajeGasId == Guid.Empty) ||
                                (!esGas &&
                                (_oferta.Attributes.Contains("atos_tarifaid") == false ||
                                 _oferta.Attributes.Contains("atos_sistemaelectricoid") == false ||
                                 _oferta.Attributes.Contains("atos_subsistemaid") == false)))
                            {
                                // Si es peaje es null o tarifa es null no hace nada
                                /* 1108-2022 -1 */
                                //Log.writelog("\n== END ==", debug);
                                return;
                            }

                            //if (((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value == OFERTA && _oferta.Attributes.Contains("atos_ofertapadreid"))
                            if (_oferta.Attributes.Contains("atos_ofertapadreid") && TipoOferta == OFERTA)
                            {
                                // Si es oferta hija no hace nada
                                /* 1108-2022 -1 */
                                //Log.writelog("\n== END ==", debug);
                                return;
                            }

                            Entity _tarifa;
                            if (esGas) // Pricing Gas
                            {
                                Log.writelog("buscando peaje: " + peajeGasId.ToString());
                                _tarifa = context.OrganizationService.Retrieve("atos_tablasatrgas",
                                          peajeGasId,
                                          new ColumnSet(new[] { "atos_name" }));

                                _tarifa.Attributes["atos_numeroperiodos"] = (Decimal)1;

                                Log.writelog("peaje encontrado");
                                if (_tarifa.Attributes.Contains("atos_name"))
                                    Log.writelog("peaje: " + _tarifa.Attributes["atos_name"].ToString());
                            }
                            else // Pricing Power
                            {
                                Log.writelog("buscando tarifa ");
                                _tarifa = context.OrganizationService.Retrieve("atos_tarifa",
                                          ((EntityReference)_oferta.Attributes["atos_tarifaid"]).Id,
                                          new ColumnSet(new[] { "atos_name", "atos_numeroperiodos" }));
                                Log.writelog(" tarifa encontrada");
                            }


                            Log.writelog("Buscar tipo de producto", true);

                            // Fin Pricing Gas
                            if (!_oferta.Contains("atos_tipodeproductofinalid"))
                                Log.writelog("no hay atos_tipodeproductofinalid", true);

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

                            // Antes de calcular los pricing output borramos los que puedan existir.
                            // DELETE_pricingoutput(ref _oferta);
                            // No debería hacer falta, solo necesario si se actualiza a stage 3 estando en stage 3, pero en ese caso no debería llegar hasta aquí.
                            Log.writelog("-----construyeFormulas------", true);
                            construyeFormulas(_tipoProducto);

                            Log.writelog("-----calcula-----", true);
                            calcula(ref _oferta, _oferta, _tipoProducto, _tarifa);

                            //if (((OptionSetValue)_oferta.Attributes["atos_tipooferta"]).Value == SUBOFERTA)
                            if (TipoOferta == SUBOFERTA)
                            {
                                // Si es suboferta actualiza stage de las hijas
                                Log.writelog("----actualiza Paso de las Hijas-----", true);
                                UPDATE_PasoHijas(efpre, _oferta, _ofertaPricing, context, false, true);
                            }
                        }
                        break;
                }

                String _error = string.Empty;
                if (errores.Count > 0)
                {
                    Log.writelog("\n", true);
                    Log.writelog("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~", true);
                    Log.writelog("Se han encontrado los siguientes errores:", true);

                    for (int i = 0; i < errores.Count; i++)
                    {
                        Log.writelog(" - " + errores[i], true);
                        _error += string.Format("{1} {0}<br>\n", errores[i], Environment.NewLine);
                    }
                    Log.writelog("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~", true);
                }

                if (_error != "")
                {
                    _error += Environment.NewLine;
                    throw new InvalidPluginExecutionException(OperationStatus.Canceled, _error);
                }
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                context.Trace(string.Format(CultureInfo.InvariantCulture, "\n\nException: {0}", e.ToString()));
                throw;
            }
            finally
            {
                /* 1108-2022 -1 */
                // context.Trace(string.Format(CultureInfo.InvariantCulture, "\nExiting TermPricing.Execute()"));
            }
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

        /*
        public Pricing(String parametros)
        {

            if (String.IsNullOrEmpty(parametros) == false)
            {
                String[] arrayPar = parametros.Split(SEPARADOR);
                if (arrayPar.Length > 0)
                {

                    if (arrayPar[0] == "LOG")
                        _log = true;
                    if (arrayPar.Length > 1)
                    {
                        ficherolog = arrayPar[1];
                        if (ficherolog == "")
                            _log = false;
                    }
                }
            }
        }
        */

        #endregion

        private void construyeFormulas(Entity _tipoProducto)
        {
            Log.writelog("\t\tConstruye Formulas 1",true);
            formula = Formula.creaFormula(_tipoProducto, tipoCalculoPromedio, Log);
            formula.esInstalacionGas = esGas;

            Log.writelog("\t\tConstruye Formulas 2", true);
            variables = formula.construyeFormulas(_tipoProducto, context.OrganizationService, tipoCalculoPromedio);

            Log.writelog("\t\tConstruye Formulas 3", true);
        }

        private void calcula(ref Entity _oferta, Entity _ofpre, Entity _tipoProducto, Entity _tarifa)
        {
            //formula.calcula(ref variables, context.OrganizationService, _oferta, _ofpre, _tipoProducto, _tarifa, ref errores);
            Log.writelog("-------------------------------- Pricing.calcula --------------------------------", true);
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

        private void ADD_producto(ref Entity _oferta)
        {
            /* 1208-2022 -1 */
            // Log.writelog("\tCrear Producto", true);

            if (_oferta.Attributes.Contains("atos_tipodeproductoid") == false )
                return;

            Guid _tipoproductoid = ((EntityReference)_oferta.Attributes["atos_tipodeproductoid"]).Id;
         
            Entity _tipoProductoBase = context.OrganizationService.Retrieve("atos_tipodeproducto",
                                        _tipoproductoid,
                                        new ColumnSet(true));

            _tipoProductoBase.Attributes.Remove("atos_tipodeproductoid");
            _tipoProductoBase.Id = Guid.NewGuid();
            _tipoProductoBase.Attributes["atos_base"] = false;
            // El campo atos_name lo calcula la secuencia
            _tipoProductoBase.Attributes.Remove("atos_name"); 
            // Se eliminan los campos para ems
            _tipoProductoBase.Attributes.Remove("atos_crearenems");
            _tipoProductoBase.Attributes.Remove("atos_tipodeproductocreadoenems");
            _tipoProductoBase.Attributes.Remove("atos_interfazformulaems");
            _tipoProductoBase.Attributes.Remove("atos_ultimowsformulaejecutado");
            _tipoProductoBase.Attributes.Remove("atos_ultimologwsformula");

            //_tipoProductoBase.Attributes["atos_name"] += "-" + _ofpre.Attributes["atos_name"].ToString();
            // atos_base poner a No

            Guid _nuevotipoproducto = context.OrganizationService.Create(_tipoProductoBase);
            //Log.writelog("\tantes de InputParameters");

            //Log.writelog("Después de InputParameters",true);
            _oferta.Attributes["atos_tipodeproductofinalid"] = new EntityReference("atos_tipodeproducto", _nuevotipoproducto);

            context.OrganizationService.Update(_oferta);

            /* 1208-2022 -1 */
            // Log.writelog("\tProducto creado", true);
        }

        private void ADD_pricinginput(ref Entity _oferta)
        {
            /* 1208-2022 -1 */
            // Log.writelog("\tAdicionar Princing Inputs", true);

            if (_oferta.Attributes.Contains("atos_tipodeproductofinalid") == false )
                return;
            if (_oferta.Attributes.Contains("atos_tipoproductofinalrevisado") == false)
                return;
            if (_oferta.Attributes.Contains("atos_pricinginputsrevisados") == false)
            {
                errores.Add("Debe revisar antes los Pricing Inputs");
                return;
            }

            Guid _tipoproductoid;
             _tipoproductoid = ((EntityReference)_oferta.Attributes["atos_tipodeproductofinalid"]).Id;
           
            Entity _tipoProducto = context.OrganizationService.Retrieve("atos_tipodeproducto",
                                                                             _tipoproductoid,
                                                                             new ColumnSet(true));
            /* 1208-2022 -1 */
            // Log.writelog("Antes de construyeFormulas");
            construyeFormulas(_tipoProducto); // Copiamos los pricinginput
            /* 1208-2022 -1 */
            // Log.writelog("Despues de construyeFormulas");

            for (int i = 0; i < formula.ComponentesFormula.Count; i++)
            {
                if (formula.ComponentesFormula[i].TipoComponente == "TermPricing")
                {
                    String msg = string.Empty;

                    /* 1208-2022 -4 */
                    //msg = string.Format("{0} Creando TermPricing: {1} {2}", i, 
                    //                                                formula.ComponentesFormula[i].NombreComponente, 
                    //                                                ((TermPricing)formula.ComponentesFormula[i]).NombreEms);
                    //Log.writelog(msg);

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
                        //_pricingInput.Attributes.Remove("atos_pricinginputid");
                        // DEBUG Log.writelog("existe pricinginput");
                    }
                    catch //(Exception e)
                    {
                        //_pricingInput = new Entity("atos_pricinginput");
                        Log.writelog("no existe pricinginput "); // + e.Message);

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

                    /* 1208-2022 -1 */
                    //Log.writelog("\t\tAsocia pricinginput a oferta");

                    _pricingInput.Attributes["atos_ofertaid"] = new EntityReference("atos_oferta", _oferta.Id);
                    _pricingInput.Id = Guid.NewGuid();

                    /* 1208-2022 -1 */
                    //Log.writelog("\t\tAsocia pricinginput a terminodepricing " + ((TermPricing)formula.ComponentesFormula[i]).TermpricingId.ToString());

                    _pricingInput.Attributes["atos_terminodepricingid"] = new EntityReference("atos_terminodepricing", ((TermPricing)formula.ComponentesFormula[i]).TermpricingId);

                    /* 1208-2022 -1 */
                    //Log.writelog("\t\tActualiza nombre de pricinginput");

                    if (((TermPricing)formula.ComponentesFormula[i]).NombreEms != "")
                        _pricingInput.Attributes["atos_name"] = ((TermPricing)formula.ComponentesFormula[i]).NombreEms + "-";
                    else
                        _pricingInput.Attributes["atos_name"] = formula.ComponentesFormula[i].NombreComponente + "-";
                    _pricingInput.Attributes["atos_name"] += _oferta.Attributes["atos_name"].ToString();

                    /* 1208-2022 -1 */
                    //Log.writelog(string.Format("Pricing Input Name: {0}", _pricingInput.Attributes["atos_name"].ToString()), true);

                    _pricingInput.Attributes["atos_tipo"] = new OptionSetValue(((TermPricing)formula.ComponentesFormula[i]).FijoIndexado.Value);

                // Log.writelog("Antes de Create pricingInput");

                    context.OrganizationService.Create(_pricingInput);

                    /* 1208-2022 -1 */
                    //Log.writelog("\t\tPricingInput adicionado");

                    /* 1208-2022 +4 */
                    msg = string.Format("{0} TermPricing creado: {1} {2}", i,
                                                                    formula.ComponentesFormula[i].NombreComponente,
                                                                    ((TermPricing)formula.ComponentesFormula[i]).NombreEms);
                    Log.writelog(msg);

                    #endregion
                }
            }
            /* 1208-2022 +4 */
            // Log.writelog("\tPrincing Inputs adicionados", true);
        }

        private void CREATE_pricinginputCollection(ref Entity _oferta)
        {
            /* 1208-2022 -1 */
            // Log.writelog("CREATE_pricinginputCollection", true);
            if (_oferta.Attributes.Contains("atos_tipodeproductofinalid") == false )
                return;

            /* 1208-2022 -1 */
            //Log.writelog("CREATE_pricinginputCollection 2", true);
            if (_oferta.Attributes.Contains("atos_tipoproductofinalrevisado") == false )
                return;

            /* 1208-2022 -1 */
            //Log.writelog("CREATE_pricinginputCollection 3", true);
            if (_oferta.Attributes.Contains("atos_pricinginputsrevisados") == false )
            {
                errores.Add("Debe revisar antes los Pricing Inputs");
                return;
            }

            //Guid _tipoproductoid;
            //Log.writelog("CREATE_pricinginputCollection 4", true);
            Guid _tipoproductoid = ((EntityReference)_oferta.Attributes["atos_tipodeproductofinalid"]).Id;

            //Log.writelog("CREATE_pricinginputCollection 5 " + _tipoproductoid.ToString(), true);
            /*
             * TO DO: Filtro por fecha
             */
            Entity _tipoProducto = context.OrganizationService.Retrieve("atos_tipodeproducto", _tipoproductoid, new ColumnSet(true));

            /* 1208-2022 -1 */
            Log.writelog(string.Format("Tipo de Producto: {0}", _tipoProducto.ToString()), true);

            construyeFormulas(_tipoProducto); // Copiamos los pricinginput

            for (int i = 0; i < formula.ComponentesFormula.Count; i++)
            {
                if (formula.ComponentesFormula[i].TipoComponente == "TermPricing")
                {

                    /* 1208-2022 -1 */
                    //String msg = string.Empty;
                    //msg = string.Format("{0} TermPricing: {1} {2}", i,
                    //                                                formula.ComponentesFormula[i].NombreComponente,
                    //                                                ((TermPricing)formula.ComponentesFormula[i]).NombreEms);
                    // Log.writelog(msg);

                    // DEBUG Log.writelog("TermPricing: " + formula.ComponentesFormula[i].NombreComponente + " " + ((TermPricing)formula.ComponentesFormula[i]).NombreEms, true);
                    //Entity _pricingInput = new Entity("atos_pricinginput");

                    EntityCollection _pricingInputColl = formula.pricingInputCollection(i, context.OrganizationService, _oferta, _oferta);
                    /* 1208-2022 -1 */
                    // Log.writelog("Encontrados " + _pricingInputColl.Entities.Count.ToString() + " pricinginputs para " + formula.ComponentesFormula[i].NombreComponente);

                    for (int j = 0; j < _pricingInputColl.Entities.Count; j++)
                    {
                        Entity _pricingInput     = new Entity("atos_pricinginput");
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
                        if (_pricingInputbase.Attributes.Contains("atos_fechainiciovigencia"))
                            _pricingInput.Attributes["atos_fechainiciovigencia"] = DateTime.SpecifyKind((DateTime)_pricingInputbase.Attributes["atos_fechainiciovigencia"], DateTimeKind.Utc);
                        if (_pricingInputbase.Attributes.Contains("atos_fechainicioaplicacion"))
                            _pricingInput.Attributes["atos_fechainicioaplicacion"] = DateTime.SpecifyKind((DateTime)_pricingInputbase.Attributes["atos_fechainicioaplicacion"], DateTimeKind.Utc);
                        if (_pricingInputbase.Attributes.Contains("atos_fechafinvigencia"))
                            _pricingInput.Attributes["atos_fechafinvigencia"] = DateTime.SpecifyKind((DateTime)_pricingInputbase.Attributes["atos_fechafinvigencia"], DateTimeKind.Utc);
                        if (_pricingInputbase.Attributes.Contains("atos_fechafinaplicacion"))
                            _pricingInput.Attributes["atos_fechafinaplicacion"] = DateTime.SpecifyKind((DateTime)_pricingInputbase.Attributes["atos_fechafinaplicacion"], DateTimeKind.Utc);
                        if (((TermPricing)formula.ComponentesFormula[i]).DependeDeSubSistemaElectrico == true)
                             _pricingInput.Attributes["atos_subsistemaid"] = new EntityReference("atos_subsistema", ((EntityReference)_oferta.Attributes["atos_subsistemaid"]).Id);
                        if (((TermPricing)formula.ComponentesFormula[i]).DependeDeSistemaElectrico == true)
                            _pricingInput.Attributes["atos_sistemaelectricoid"] = new EntityReference("atos_sistemaelectrico", ((EntityReference)_oferta.Attributes["atos_sistemaelectricoid"]).Id);
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
                        Log.writelog(string.Format("Pricing Input {0}: {1}", j, _pricingInput.Attributes["atos_name"].ToString()), true);

                        //Log.writelog("\tAntes de Create pricingInput", true);
                        context.OrganizationService.Create(_pricingInput);
                        //Log.writelog("\tDespués de Create pricingInput", true);

                        /* 1208-2022 -1 */
                        // Log.writelog("\tPricing Input Creado", true);
                    }
                }
            }

            /* 1208-2022 -1 */
            // Log.writelog("\tPrincing Inputs creados", true);
        }

        /*
         Elimina el producto de la Oferta
         <param name="_oferta">Referencia a la entidad oferta (registro que se está modificando)</param>         
         Blanquea el campo de producto de la Oferta
         */
        private void DELETE_producto(ref Entity _oferta)
        {
            /* 1208-2022 -1 */
            // Log.writelog("\tEliminar Productos");

            if (_oferta.Attributes.Contains("atos_tipodeproductofinalid"))
            {
                Guid _tipoProductoId = ((EntityReference)_oferta.Attributes["atos_tipodeproductofinalid"]).Id;

                _oferta.Attributes["atos_tipodeproductofinalid"] = null;
                _oferta.Attributes["atos_tipoproductofinalrevisado"] = false;

                context.OrganizationService.Update(_oferta);
            }

            /* 1208-2022 -1 */
            // Log.writelog("\tOferta Guardada");
        }

        public static EntityCollection GetAccounts(IOrganizationService service, string accountName)
        {
            string fetchXML = string.Format(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' no-lock='true' distinct='false'>
                                                       <entity name='account'>                                                                
                                                           <attribute name='accountid' />
                                                           <attribute name='name' />
                                                           <filter type='and'>
                                                               <condition attribute='name' operator='like' value='{0}' />                                                                                                    
                                                           </filter>
                                                       </entity>
                                                   </fetch>", accountName);

            var fetchExp = new FetchExpression(fetchXML);

            var accounts = service.RetrieveMultiple(fetchExp);

            return accounts;
        }
        /*
         Elimina los registros de pricing input para una oferta.

         <param name="_oferta">Referencia a la entidad oferta (registro que se está modificando)</param>
         <param name="_ofpre">Imagen de la entidad oferta antes de la modificación</param>

         Borra todos los registros de Pricing Input de la oferta recibida.
         - Actualiza en la oferta el campo atos_pricinginputsrevisados a false
         */
        private void DELETE_pricinginput(ref Entity _oferta)
        {
            /* 01208-2022 -5 */
            //Log.writelog("\tEliminar Pricing Input", true);
            //String msg = String.Empty;
            //String trace = String.Empty;
            //StringBuilder t = new StringBuilder();
            //int count = 0;

            if (_oferta.Attributes.Contains("atos_tipodeproductofinalid") == false)
            {
                Log.writelog("\tNo existe tipo de producto final", true);
                return;
            }

            /* 01208-2022 +1 */
            //msg = _oferta.Attributes.Contains("atos_name") ? _oferta.Attributes["atos_name"].ToString() : String.Empty;
            //Log.writelog(string.Format("\tOferta: {0} ", msg), true);

            QueryExpression _consulta = new QueryExpression("atos_pricinginput");
            /* 01208-2022 +1 */
            _consulta.NoLock = true;
            _consulta.ColumnSet = new ColumnSet(true);
                _consulta.NoLock = true;
            FilterExpression _filtro = new FilterExpression();
                _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
                _condicion.AttributeName = "atos_ofertaid";
                _condicion.Operator = ConditionOperator.Equal;
                _condicion.Values.Add(_oferta.Id.ToString());
            // DEBUG Log.writelog(string.Format("\tId Oferta: {0} ", _oferta.Id.ToString()), true);
                _filtro.Conditions.Add(_condicion);
                _condicion = new ConditionExpression(); // Excluimos pricing input del cierre de la oferta
                _condicion.AttributeName = "atos_cierreofertaid";
                _condicion.Operator = ConditionOperator.Null;
            //Log.writelog("\tCierre Oferta a null");
                _filtro.Conditions.Add(_condicion);
                _condicion = new ConditionExpression(); // Excluimos pricing input de facturacion estimada
                _condicion.AttributeName = "atos_facturacionestimada";
                _condicion.Operator = ConditionOperator.NotEqual;
                _condicion.Values.Add(true);
            //Log.writelog("\tFacturacion estimada True");
                _filtro.Conditions.Add(_condicion);
                _consulta.Criteria.AddFilter(_filtro);
                _consulta.ColumnSet.AddColumns("atos_pricinginputid");

            EntityCollection _resultado = context.OrganizationService.RetrieveMultiple(_consulta);

            //int count = _resultado.Entities.Count;

            /* 01208-2022 -1 */
            // Log.writelog(string.Format("\tHay {0} Pricing Imputs a eliminar de la Oferta: {1} {2}", count, msg, _oferta.Id.ToString()), true);

            for (int i = 0; i < _resultado.Entities.Count; i++)
            {
                Guid _pricinginputid = (Guid)_resultado.Entities[i].Attributes["atos_pricinginputid"];

                /* 01208-2022 +2 */
                //msg = _resultado.Entities[i].Attributes.Contains("atos_name") ? _resultado.Entities[i].Attributes["atos_name"].ToString() : String.Empty;
                //Log.writelog(string.Format("\t\tPricing Input: {0} {1} eliminado", i, msg), true);
                context.OrganizationService.Delete("atos_pricinginput", _pricinginputid);
                Log.writelog(string.Format("\t\tPricing Input: {0}/{1} eliminado", i, _resultado.Entities.Count), true);
            }

            _oferta.Attributes["atos_pricinginputsrevisados"] = false;

            /* 01208-2022 -1 */
            // Log.writelog(string.Format("\t{0} Pricing Output eliminados", count), true);

            context.OrganizationService.Update(_oferta);

            /* 01208-2022 -1 */
            // Log.writelog("\tOferta Guardada", true);
        }

        /*
         Elimina los registros de pricing output para una oferta.
         Paramertro: _oferta = Referencia a la entidad oferta
         
         Borra todos los registros de Pricing Output de la oferta recibida.
        */
        private void DELETE_pricingoutput(ref Entity _oferta)
        {
            // Log.writelog("\tEliminar Pricing Output", true);
            // String msg = String.Empty;
            //int count = 0;

            // msg = _oferta.Attributes.Contains("atos_name") ? _oferta.Attributes["atos_name"].ToString() : String.Empty;
            //Log.writelog(string.Format("\tEliminar Pricing Outputs de la Oferta: {0}-{1}", msg, _oferta.Id.ToString()), true);

            QueryExpression _consulta = new QueryExpression("atos_pricingoutput");
                _consulta.ColumnSet = new ColumnSet(true);
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

            EntityCollection _resultado = context.OrganizationService.RetrieveMultiple(_consulta);

            //if (_resultado.Entities.Count > 0) Log.writelog(string.Format("\t\tHay {0} Pricing Outputs a eliminar", _resultado.Entities.Count), true);

            int count = _resultado.Entities.Count;
            //msg = _oferta.Attributes.Contains("atos_name") ? _oferta.Attributes["atos_name"].ToString() : String.Empty;

            /* 0811-2022 -1 */
            // if (count > 0) Log.writelog(string.Format("\tHay {0} Pricing Outputs a eliminar de la Oferta: {1}  {2}", count, msg, _oferta.Id.ToString()), true);

            for (int i = 0; i < _resultado.Entities.Count; i++)
            {
                Guid _pricingoutputid = (Guid)_resultado.Entities[i].Attributes["atos_pricingoutputid"];

                /* 0811-2022 -1 */
                // msg = _resultado.Entities[i].Attributes.Contains("atos_name") ? _resultado.Entities[i].Attributes["atos_name"].ToString() : String.Empty;
                // Log.writelog(string.Format("\t\tPricing Output: {0} {1} eliminado", i, msg), true);
                Log.writelog(string.Format("\t\tPricing Output: {0}/{1} eliminado", i, count), true);

                context.OrganizationService.Delete("atos_pricingoutput", _pricingoutputid);
            }

            /* 0811-2022 -1 */
            // Log.writelog(string.Format("\t{0} Pricing Output eliminados", count), true);
        }

        private EntityCollection GET_Ofertas(Entity oferta, LocalPluginContext context)
        {
            QueryExpression _consulta = new QueryExpression("atos_oferta");           
                _consulta.ColumnSet = new ColumnSet(true); 
                _consulta.NoLock = true;
            FilterExpression _filtro = new FilterExpression();  
                _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
                _condicion.AttributeName = "atos_ofertapadreid";
                _condicion.Operator = ConditionOperator.Equal;
                _condicion.Values.Add(oferta.Id.ToString());
            //Log.writelog(string.Format("\t\tRecupera Id Oferta: {0}", ofertaPadre.Id.ToString()), true);
                _filtro.Conditions.Add(_condicion);
                _consulta.Criteria.AddFilter(_filtro);
                _consulta.ColumnSet.AddColumns("atos_tipooferta", "atos_tipodeproductofinalid");

            /* 0811-2022 -1 */
            //Log.writelog("\t\t\t\tRecupera Ofertas");

            return context.OrganizationService.RetrieveMultiple(_consulta);
        }        

        private EntityCollection GET_EstadoProceso_Oferta(Entity oferta, LocalPluginContext context)
        {
            QueryExpression _consulta = new QueryExpression("atos_bpf_1c3d1c4af29543429ee2b7465a2e6ee8");
                _consulta.ColumnSet = new ColumnSet(true); 
                _consulta.NoLock = true;
            FilterExpression _filtro = new FilterExpression();
                _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
                _condicion.AttributeName = "bpf_atos_ofertaid";
                _condicion.Operator = ConditionOperator.Equal;
                _condicion.Values.Add(oferta.Id.ToString());
                //Log.writelog(string.Format("\t\tRecupera Id Pricing: {0}", oferta.Id.ToString()), true);
                _filtro.Conditions.Add(_condicion);
                _consulta.Criteria.AddFilter(_filtro);
                _consulta.ColumnSet.AddColumns("businessprocessflowinstanceid", "activestageid", "traversedpath", "processid");

            /* 0811-2022 -1 */
            // Log.writelog("\t\t\t\tRecupera los Pricing de Ofertas");

            return context.OrganizationService.RetrieveMultiple(_consulta);
        }

        private void UPDATE_PasoHijas(Entity preOferta, Entity ofertaPadre, Entity ofertaPricing, LocalPluginContext context, bool cambiaproducto = false, bool cambiaTipoCalculo = false)
        {
            String msg = String.Empty;

            if (((OptionSetValue)ofertaPadre.Attributes["atos_tipooferta"]).Value == OFERTA) //  == 300000002)
                return;

            Log.writelog("\t\tActualiza (Stage) Paso Ofertas hijas: " + ofertaPadre.Id.ToString(), true);
            Entity stage = context.OrganizationService.Retrieve("processstage", ((EntityReference)ofertaPricing.Attributes["activestageid"]).Id, new ColumnSet(true));
            Log.writelog("\t\tStage (PADRE) activestageid: " + stage.Id.ToString());

            EntityReference processid;

            if (ofertaPricing.Attributes.Contains("proccessid"))
                processid = (EntityReference)ofertaPricing.Attributes["processid"];
            else
                processid = (EntityReference)preOferta.Attributes["processid"];

            Log.writelog(String.Format("\t\tprocessid: {0}", processid.ToString()), true);

            /* Manejo de Sub-Ofertas */

/*>>>>>>>*/ EntityCollection _resultado = GET_Ofertas(ofertaPadre, context);

            Guid _tipoproductoid = Guid.Empty;
            if (cambiaproducto)
            {
                if (ofertaPadre.Attributes.Contains("atos_tipodeproductoid"))
                {
                    _tipoproductoid = ((EntityReference)ofertaPadre.Attributes["atos_tipodeproductoid"]).Id;
                }
            }

            Object _precisiondecimalesoferta = null;
            if (ofertaPadre.Attributes.Contains("atos_precisiondecimalesoferta"))
            {
                //AC Log.writelog("10");
                _precisiondecimalesoferta = ofertaPadre.Attributes["atos_precisiondecimalesoferta"];
            }

            OptionSetValue optTipoCalculoPromedio = new OptionSetValue(300000000);
            OptionSetValue optTipoCalculoDirecto  = new OptionSetValue(300000001);

            Log.writelog(string.Format("\tHay ({0}) ofertas hijas", _resultado.Entities.Count), true);

            for (int i = 0; i < _resultado.Entities.Count; i++)
            {
                Entity _ofertaHija = new Entity("atos_oferta");
                _ofertaHija.Id = _resultado.Entities[i].Id;

                String __oferta = String.Empty;
                __oferta = _resultado.Entities[i].Attributes.Contains("atos_name") ? _resultado.Entities[i].Attributes["atos_name"].ToString() : String.Empty;

                /* DEBUG */ Log.writelog(string.Format("\t\tOferta hija: {0}-{1}", i, __oferta), true);

/*>>>>>>>*/     EntityCollection ofertasPricing = GET_EstadoProceso_Oferta(_resultado.Entities[i], context);

                foreach (Entity op in ofertasPricing.Entities)
                {
                    Entity opAct = new Entity("atos_bpf_1c3d1c4af29543429ee2b7465a2e6ee8");
                    opAct.Id = op.Id;
                    opAct.Attributes["activestageid"] = ofertaPricing.Attributes["activestageid"];
                    opAct.Attributes["processid"] = processid;

                     if (ofertaPricing.Attributes.Contains("traversedpath"))
                    {
                        Log.writelog(string.Format("\tENTRA 1 :traversedpath: {0}", ofertaPricing.Attributes["traversedpath"].ToString()), false);
                        opAct.Attributes["traversedpath"] = ofertaPricing.Attributes["traversedpath"];
                    }
                    else
                    {
                        Log.writelog(string.Format("\tENTRA 2 :traversedpath: {0}", ((EntityReference)ofertaPricing.Attributes["activestageid"]).Id.ToString()), false);
                        opAct.Attributes["traversedpath"] = ((EntityReference)ofertaPricing.Attributes["activestageid"]).Id.ToString();
                    }

                    //Log.writelog(string.Format("\t\tOp Id: {0}", op.Id), true);
                    //Log.writelog(string.Format("\t\tAct Id: {0}", opAct.Id), true);
                    context.OrganizationService.Update(opAct);

                    /* 1208-2022 -1 */
                    //Log.writelog("Guardado");
                }

                if (_precisiondecimalesoferta != null)
                {
                    _ofertaHija.Attributes["atos_precisiondecimalesoferta"] = _precisiondecimalesoferta;
                }

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

                context.OrganizationService.Update(_ofertaHija);
                Log.writelog(string.Format("\t\t\tOferta {0} Grabada", _resultado.Entities[i].Attributes["atos_name"].ToString()), true);
                /* 1208-2022 -1 */
                // Log.writelog("\t\t\tOferta Grabada", true);

                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

                if (((OptionSetValue)ofertaPadre.Attributes["atos_tipooferta"]).Value == MULTIPUNTO && //== 300000000 &&
                    ((OptionSetValue)_resultado.Entities[i].Attributes["atos_tipooferta"]).Value == SUBOFERTA) // == 300000001)
                {
/*>>>>>>>*/         EntityCollection _SubOferta = GET_Ofertas(_resultado.Entities[i], context);

                    // Manejo de SubOfertas

                    /*DEBUG */ Log.writelog(string.Format("\t\tHay {0} Subofertas", _SubOferta.Entities.Count), true);

                    for (int j = 0; j < _SubOferta.Entities.Count; j++)
                    {
                        _ofertaHija = new Entity("atos_oferta");
                        _ofertaHija.Id = _SubOferta.Entities[j].Id;

                        /* DEBUG */ Log.writelog(string.Format("\t\t\tSubOferta: {0} {1} -> {2}", j, _SubOferta.Entities[j]["atos_name"], _ofertaHija.Id), true);

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

                            //Log.writelog(string.Format("\t\tOp Id: {0}", op.Id), true); // NUEvo
                            //Log.writelog(string.Format("\t\tAct Id: {0}", opAct.Id), true); // NUEvo
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

                        context.OrganizationService.Update(_ofertaHija);
                        /* 1208-2022 -1 */
                        // Log.writelog(string.Format("\t\t\tSubOferta {0} Grabada", _SubOferta.Entities[j].Id.ToString()), true);
                    }
                }
            }
        }

        private void DELETE_Productohijas(Entity preOferta, Entity ofertaPadre, Entity ofertaPricing, LocalPluginContext context, Guid productofinalId)
        {
            /* 1208-2022 -1 */
            // Log.writelog("\tEliminar productos de las ofertas hijas", true);

            if (((OptionSetValue)ofertaPadre.Attributes["atos_tipooferta"]).Value == OFERTA)
            {
                Log.writelog("\tNo elimina los productos --> Tipo OFerta = OFERTA", true);
                return;
            }

            //Log.writelog("\tRecupera stage padre",true);
            Entity stage = context.OrganizationService.Retrieve("processstage", ((EntityReference)ofertaPricing.Attributes["activestageid"]).Id, new ColumnSet(true));
            Log.writelog(string.Format("\t\tRecupera stage padre (activestageid): {0} ", stage.Id.ToString()), true);

            EntityReference processid;

            if (ofertaPricing.Attributes.Contains("proccessid"))
                processid = (EntityReference)ofertaPricing.Attributes["processid"];
            else
                processid = (EntityReference)preOferta.Attributes["processid"];

            EntityCollection _resultado = GET_Ofertas(ofertaPadre, context);                

            for (int i = 0; i < _resultado.Entities.Count; i++)
            {
                Entity _ofertaHija = new Entity("atos_oferta");
                _ofertaHija.Id = _resultado.Entities[i].Id;
               
                Log.writelog(string.Format("\t\tOferta hija: {0} {1}", i, _resultado.Entities[i]["atos_name"]), true);

                EntityCollection ofertasPricing = GET_EstadoProceso_Oferta(_resultado.Entities[i], context);

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
            /* 1208-2022 -1 */
            // Log.writelog("\tProductos de las ofertas hijas eliminadas", true);
        }

        private Guid matrizHoraria(Entity _oferta)
        {
            Guid _matrizhorariaid = Guid.Empty;
            Guid _sistemaelectricoid = Guid.Empty;
            Guid _tarifaid = Guid.Empty;

            Log.writelog("\tAplicar matriz horaria", true);

            if (_oferta.Attributes.Contains("atos_sistemaelectricoid"))
                _sistemaelectricoid = ((EntityReference)_oferta.Attributes["atos_sistemaelectricoid"]).Id;

            if (_oferta.Attributes.Contains("atos_tarifaid"))
                _tarifaid = ((EntityReference)_oferta.Attributes["atos_tarifaid"]).Id;
          
            if (_tarifaid == Guid.Empty || _sistemaelectricoid == Guid.Empty)
                return _matrizhorariaid;

            //Log.writelog("matrizHoraria - antes de QueryByAttribute");
            QueryByAttribute _consulta = new QueryByAttribute("atos_matrizportarifaysistemaelectrico");
            //Log.writelog("matrizHoraria - antes de definir columnas");
                _consulta.ColumnSet = new ColumnSet(new String[] { "atos_matrizhorariaid" });
                _consulta.AddAttributeValue("atos_tarifaid", _tarifaid.ToString());
                _consulta.AddAttributeValue("atos_sistemaelectricoid", _sistemaelectricoid.ToString());

            EntityCollection _resConsulta = context.OrganizationService.RetrieveMultiple(_consulta);
            if (_resConsulta.Entities.Count > 0)
                _matrizhorariaid = ((EntityReference)_resConsulta.Entities[0].Attributes["atos_matrizhorariaid"]).Id;

            Log.writelog("\rMatriz horaria aplicada", true);

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
                }
            }

            #endregion

    }
}
 