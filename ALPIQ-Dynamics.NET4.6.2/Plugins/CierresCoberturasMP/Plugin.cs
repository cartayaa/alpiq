﻿// <copyright file="Plugin.cs" company="Licence Owner">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Licence Owner</author>
// <date>8/24/2015 11:22:04 AM</date>
// <summary>Implements the Plugin Workflow Activity.</summary>
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
// </auto-generated>
namespace CierresCoberturasMP
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System.Collections.Generic;

    /// <summary>
    /// Base class for all Plugins.
    /// </summary>    
    public class CierresCoberturasMP : IPlugin
    {
        
        private ITracingService tracingService;
        private IPluginExecutionContext PluginExecutionContext; ///< Contexto de ejecución del plugin
        private IOrganizationServiceFactory factory;
        private IOrganizationService service;
        
        private bool _log = false; ///< Indica si se activa o no el log. Esta variable debe inicializarse según el parámetro recibido en el constructor.
        private String ficherolog = "C:\\Users\\log_CierresCoberturasMP.txt";  ///< Fichero de log. Esta variable debe inicializarse según el parámetro recibido en el constructor.
        private const Char SEPARADOR = '#'; ///< Constante para el separador a usar en el parámetro que recibe el constructor

        List<String> errores = new List<String>();

        /**
		// <summary>
		// Constructor de la clase
		// </summary>
        // <param name="parametros">Cadena en la que se indica si se escribe log y donde: LOG#C:\\RutaDelLog\\Fichero.log</param>
		// <remarks>
		// Recibe una cadena de texto incluyendo los parámetros (separados por el carácter #)
		// - El primer parámetro activa/desactiva la escritura del fichero log (LOG activa)
		// - El segundo parámetro es el nombre (incluyendo ruta) del fichero de log.
		// </remarks>
         */
        public CierresCoberturasMP(String parametros)
        {
            if (String.IsNullOrEmpty(parametros) == false)
            {
                String[] arrayPar = parametros.Split(SEPARADOR);
                if (arrayPar.Length > 0)
                {

                    if (arrayPar[0] == "LOG")
                        _log = true;
                    if (arrayPar.Length > 1)
                        ficherolog = arrayPar[1];
                }
            }
        }


        /**
        // <summary>
        // Función privada para escribir una traza
        // </summary>
        // <param name="texto">Texto a escribir en el fichero de log</param>
        // <remarks>
        // Si el log está activado escribe el mensaje en el fichero de log.
        // </remarks>
         */
        private void writelog(String texto, bool _traza = false)
        {
            if ( _traza )
                tracingService.Trace(texto);
            if (_log == true)
                System.IO.File.AppendAllText(ficherolog, texto + "\r\n");
        }

        private EntityCollection contratosHijos(Guid contratopadreid, Guid contratoid, Guid subofertaid)
        {
            writelog("contratosHijos");
            QueryExpression _consulta = new QueryExpression("atos_contrato");
            _consulta.ColumnSet = new ColumnSet("atos_name");
             
            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_contratomultipuntoid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(contratopadreid.ToString());
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "statecode";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(0);
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_contratoid";
            _condicion.Operator = ConditionOperator.NotEqual;
            _condicion.Values.Add(contratoid.ToString());
            _filtro.Conditions.Add(_condicion);

            if (subofertaid != Guid.Empty)
            {
                writelog("contratosHijos suboferta");
                // Filtramos los contratos de la misma suboferta
                LinkEntity _link = new LinkEntity();
                _link.JoinOperator = JoinOperator.Inner;
                _link.LinkFromAttributeName = "atos_ofertaid";
                _link.LinkFromEntityName = "atos_contrato";
                _link.LinkToAttributeName = "atos_ofertaid";
                _link.LinkToEntityName = "atos_oferta";
                _link.LinkCriteria.AddCondition(new ConditionExpression("atos_ofertapadreid", ConditionOperator.Equal, subofertaid.ToString()));

                writelog("Filtramos por la suboferta " + subofertaid.ToString());

                _consulta.LinkEntities.Add(_link);
            }

            _consulta.Criteria.AddFilter(_filtro);
            writelog("contratosHijos antes del retrieve multiple");
            EntityCollection _resConsulta = service.RetrieveMultiple(_consulta);
            writelog("contratosHijos despues del retrieve multiple");
            return _resConsulta;
        }

        private Guid recuperaVariable(Guid _idContrato, String _terminoEms)
        {
            Guid variable = Guid.Empty;

            QueryExpression _consulta = new QueryExpression("atos_pricingoutput");
            _consulta.ColumnSet = new ColumnSet("atos_pricingoutputid");

            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_contratoid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(_idContrato.ToString());
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_terminoems";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(_terminoEms);
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "statecode";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(0);
            _filtro.Conditions.Add(_condicion);


            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_terminodepricingid";
            _condicion.Operator = ConditionOperator.Null;
            _filtro.Conditions.Add(_condicion);

            _consulta.Criteria.AddFilter(_filtro);

            EntityCollection _resConsulta = service.RetrieveMultiple(_consulta);

            if (_resConsulta.Entities.Count == 1)
                variable = _resConsulta.Entities[0].Id;

            return variable;
        }



        public void Execute(IServiceProvider serviceProvider)
        {

            errores.Clear();
            tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            PluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the Organization Service factory service from the service provider
            factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            // Use the factory to generate the Organization Service.
            service = factory.CreateOrganizationService(PluginExecutionContext.UserId);

            writelog("-----------------------------------------");
            writelog(DateTime.Now.ToLocalTime().ToString());
            writelog("Plugin CierresCoberturasMP");
            writelog("Mensaje: " + PluginExecutionContext.MessageName);
            if (PluginExecutionContext.MessageName == "Create")
            {
                Entity ef = (Entity)PluginExecutionContext.InputParameters["Target"];

                if (ef.LogicalName != "atos_trigger")
                    return;

                if (ef.Attributes.Contains("atos_accion") == false ||
                    ef.Attributes.Contains("atos_entity") == false ||
                    ef.Attributes.Contains("atos_guid") == false)
                    return;

                String entidad = ef.Attributes["atos_entity"].ToString();
                if ((ef.Attributes["atos_accion"].ToString() != "Cierre" || entidad != "atos_cierre") &&
                     (ef.Attributes["atos_accion"].ToString() != "Cobertura" || entidad != "atos_cobertura"))
                    return;

                
                Guid idcierrecobertura = new Guid(ef.Attributes["atos_guid"].ToString());
                writelog("Guid " + idcierrecobertura.ToString());
                Entity cierrecobertura = service.Retrieve(entidad, idcierrecobertura, new ColumnSet(true)); // Se recuperan todas las columnas porque hay que crear duplicados.
                
                
                if (cierrecobertura.Attributes.Contains("atos_contratoid") == false)
                {
                    writelog(entidad + " no contiene lookup a contrato");
                    return;
                }
                writelog("Contrato del cierre/cobertura " + ((EntityReference)cierrecobertura.Attributes["atos_contratoid"]).Id.ToString());
                Entity contrato = service.Retrieve("atos_contrato", ((EntityReference)cierrecobertura.Attributes["atos_contratoid"]).Id, new ColumnSet(new String[] {"atos_contratomultipuntoid", "atos_ofertaid"}));
                if (contrato.Attributes.Contains("atos_contratomultipuntoid") == false)
                {
                    writelog("El contrato de " + entidad + " no es un contrato hijo");
                    return;
                }

                writelog("Contrato multipunto del cierre/cobertura " + ((EntityReference)contrato.Attributes["atos_contratomultipuntoid"]).Id.ToString());
                if (contrato.Attributes.Contains("atos_ofertaid") == false)
                {
                    writelog("¡ERROR!. El contrato de " + entidad + " no está asociado a ninguna oferta");
                    throw new Exception("¡ERROR!. El contrato de " + entidad + " no está asociado a ninguna oferta");
                }

                writelog("Oferta del cierre/cobertura " + ((EntityReference)contrato.Attributes["atos_ofertaid"]).Id.ToString());
                Guid subofertaid = Guid.Empty;
                if (entidad == "atos_cierre")
                {
                    Entity oferta = service.Retrieve("atos_oferta", ((EntityReference)contrato.Attributes["atos_ofertaid"]).Id, new ColumnSet(new String[] {"atos_ofertapadreid"}));
                    if (oferta.Attributes.Contains("atos_ofertapadreid"))
                    {
                        subofertaid = ((EntityReference)oferta.Attributes["atos_ofertapadreid"]).Id;
                    }
                    if (subofertaid != Guid.Empty)
                        writelog("Suboferta del cierre/cobertura " + subofertaid.ToString());
                    else
                        writelog("Suboferta vacía");
                }

                EntityCollection contratos = contratosHijos(((EntityReference)contrato.Attributes["atos_contratomultipuntoid"]).Id,
                                                            contrato.Id, subofertaid);


                if (entidad == "atos_cobertura")
                {
                    for (int i = 0; i < contratos.Entities.Count; i++)
                    {
                        Entity nuevacobertura = cierrecobertura;
                        nuevacobertura.Attributes.Remove("atos_name");
                        nuevacobertura.Attributes.Remove("atos_coberturaid");
                        nuevacobertura.Id = Guid.Empty;
                        nuevacobertura.Attributes["atos_contratoid"] = new EntityReference("atos_contrato", contratos.Entities[i].Id);
                        writelog("Crea cobertura para contrato " + contratos.Entities[i].Attributes["atos_name"].ToString());


                        try
                        {
                            service.Create(nuevacobertura);
                        }
                        catch (Exception e)
                        {
                            writelog("Error creando cobertura para contrato " + contratos.Entities[i].Attributes["atos_name"].ToString() + ": " + e.Message);
                            errores.Add("Error creando cobertura para contrato " + contratos.Entities[i].Attributes["atos_name"].ToString() + ": " + e.Message);
                        }
                    }
                }
                else if (entidad == "atos_cierre")
                {
                    writelog(entidad);
                    if (cierrecobertura.Attributes.Contains("atos_variable") == false)
                    {
                        writelog(entidad + " no contiene lookup a pricing output");
                        return;
                    }
                    writelog("Tiene atos_variable");
                    if (cierrecobertura.Attributes.Contains("atos_pricinginputid") == false)
                    {
                        writelog(entidad + " no contiene lookup a pricing input");
                        return;
                    }
                    writelog("Tiene atos_pricinginputid");
                    Entity pricingoutput = service.Retrieve("atos_pricingoutput", ((EntityReference)cierrecobertura.Attributes["atos_variable"]).Id, new ColumnSet("atos_terminoems"));
                    if ( pricingoutput.Attributes.Contains("atos_terminoems") == false)
                    {
                        writelog("La variable del cierre no contiene termino ems");
                        return;
                    }
                    writelog("Tiene atos_terminoems");
                    for (int i = 0; i < contratos.Entities.Count; i++)
                    {
                        Entity nuevocierre = cierrecobertura;
                        nuevocierre.Attributes.Remove("atos_name");
                        nuevocierre.Attributes.Remove("atos_cierreid");
                        nuevocierre.Id = Guid.Empty;
                        nuevocierre.Attributes["atos_contratoid"] = new EntityReference("atos_contrato", contratos.Entities[i].Id);
                        nuevocierre.Attributes["atos_pricinginputid"] = new EntityReference("atos_pricinginput", ((EntityReference)cierrecobertura.Attributes["atos_pricinginputid"]).Id);
                        Guid variable = recuperaVariable(contratos.Entities[i].Id, pricingoutput.Attributes["atos_terminoems"].ToString());
                        nuevocierre.Attributes["atos_copiaenmultipuntolanzada"] = true;
                        /*if (variable != Guid.Empty)
                        {
                            writelog("Crea cierre para contrato " + contratos.Entities[i].Attributes["atos_name"].ToString());
                            */
                            try
                            {
                                service.Create(nuevocierre);
                            }
                            catch (Exception e)
                            {
                                writelog("Error creando cierre para contrato " + contratos.Entities[i].Attributes["atos_name"].ToString() + ": " + e.Message);
                                errores.Add("Error creando cierre para contrato " + contratos.Entities[i].Attributes["atos_name"].ToString() + ": " + e.Message);
                            }
                        /*}
                        else
                        {
                            writelog("Contrato " + contratos.Entities[i].Attributes["atos_name"] + " no tiene variable con término ems: " + pricingoutput.Attributes["atos_terminoems"].ToString());
                            errores.Add("Contrato " + contratos.Entities[i].Attributes["atos_name"] + " no tiene variable con término ems: " + pricingoutput.Attributes["atos_terminoems"].ToString());
                        }*/
                    }
                    try
                    {
                        Entity cierreActualizar = new Entity("atos_cierre");
                        cierreActualizar.Id = idcierrecobertura;
                        cierreActualizar.Attributes["atos_copiaenmultipuntolanzada"] = true;
                        service.Update(cierreActualizar);
                    }
                    catch (Exception e)
                    {
                        writelog("Error actualizando cierre: " + e.Message);
                        errores.Add("Error actualizando cierre:" + e.Message);
                    }

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
                    throw new InvalidPluginExecutionException(OperationStatus.Canceled, _error);
                }
            }
            writelog("=========================================");

        }
    }
}