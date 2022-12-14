// <copyright file="Plugin.cs" company="Licence Owner">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Licence Owner</author>
// <date>2/2/2017 1:22:44 PM</date>
// <summary>Implements the Plugin Workflow Activity.</summary>
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
// </auto-generated>
namespace ActFechasPOContrato
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Base class for all Plugins.
    /// </summary>    
    public class ActFechasPOContrato : IPlugin
    {

        private ITracingService tracingService;
        private IPluginExecutionContext PluginExecutionContext; ///< Contexto de ejecución del plugin
        private IOrganizationServiceFactory factory;
        private IOrganizationService service;

        private bool _log = false; ///< Indica si se activa o no el log. Esta variable debe inicializarse según el parámetro recibido en el constructor.
        private String ficherolog = "C:\\Users\\log_ActFechasPOContrato.txt";  ///< Fichero de log. Esta variable debe inicializarse según el parámetro recibido en el constructor.
        private const Char SEPARADOR = '#'; ///< Constante para el separador a usar en el parámetro que recibe el constructor
        
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
        public ActFechasPOContrato(String parametros)
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
        private void writelog(String texto)
        {
            tracingService.Trace(texto);
            if (_log == true)
                System.IO.File.AppendAllText(ficherolog, texto + "\r\n");
        }

        private void modificaPricingOutputs(Entity _contratoPre, Entity _contratoPost)
        {
            QueryExpression _consulta = new QueryExpression("atos_pricingoutput");
            _consulta.ColumnSet = new ColumnSet(new String[] { "atos_fechainicioaplicacion" , "atos_fechafinaplicacion" });

            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_contratoid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(_contratoPre.Id.ToString());
            _filtro.Conditions.Add(_condicion);
            writelog("Contrato: " + _contratoPre.Id.ToString());
            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_ofertaid";
            _condicion.Operator = ConditionOperator.Null;
            _filtro.Conditions.Add(_condicion);

            if (_contratoPost.Attributes.Contains("atos_fechainicioefectiva") && _contratoPre.Attributes.Contains("atos_fechainicioefectiva"))
            {
                if ((DateTime)_contratoPre.Attributes["atos_fechainicioefectiva"] > (DateTime)_contratoPost.Attributes["atos_fechainicioefectiva"])
                {
                    writelog("fechainicioefectiva: " + ((DateTime)_contratoPre.Attributes["atos_fechainicioefectiva"]).ToString());
                    _condicion = new ConditionExpression();
                    _condicion.AttributeName = "atos_fechainicioaplicacion";
                    _condicion.Operator = ConditionOperator.Equal;
                    _condicion.Values.Add((DateTime)_contratoPre.Attributes["atos_fechainicioefectiva"]);
                    _filtro.Conditions.Add(_condicion);
                }
            }


            if (_contratoPost.Attributes.Contains("atos_fechafindefinitiva") && _contratoPre.Attributes.Contains("atos_fechafindefinitiva"))
            {
                if ((DateTime)_contratoPost.Attributes["atos_fechafindefinitiva"] > (DateTime)_contratoPre.Attributes["atos_fechafindefinitiva"])
                {
                    writelog("fechafindefinitiva: " + ((DateTime)_contratoPre.Attributes["atos_fechafindefinitiva"]).ToString());
                    _condicion = new ConditionExpression();
                    _condicion.AttributeName = "atos_fechafinaplicacion";
                    _condicion.Operator = ConditionOperator.Equal;
                    _condicion.Values.Add((DateTime)_contratoPre.Attributes["atos_fechafindefinitiva"]);
                    _filtro.Conditions.Add(_condicion);
                }
            }
            _consulta.Criteria.AddFilter(_filtro);

            _consulta.ColumnSet = new ColumnSet(true);

            EntityCollection _resultado = service.RetrieveMultiple(_consulta);

            for (int i = 0; i < _resultado.Entities.Count; i++)
            {
                Entity _pricingOutput = _resultado.Entities[i];
                bool actualiza = false;
                if (_contratoPost.Attributes.Contains("atos_fechainicioefectiva") && 
                    _contratoPre.Attributes.Contains("atos_fechainicioefectiva") &&
                    _pricingOutput.Attributes.Contains("atos_fechainicioaplicacion"))
                {
                    if ((DateTime)_contratoPre.Attributes["atos_fechainicioefectiva"] > (DateTime)_contratoPost.Attributes["atos_fechainicioefectiva"] &&
                        (DateTime)_pricingOutput.Attributes["atos_fechainicioaplicacion"] == (DateTime)_contratoPre.Attributes["atos_fechainicioefectiva"])
                    {
                        _pricingOutput.Attributes["atos_fechainicioaplicacion"] = (DateTime)_contratoPost.Attributes["atos_fechainicioefectiva"];
                        actualiza = true;
                    }
                    else
                        _pricingOutput.Attributes.Remove("atos_fechainicioaplicacion");
                }
                else
                    _pricingOutput.Attributes.Remove("atos_fechainicioaplicacion");

                if (_contratoPost.Attributes.Contains("atos_fechafindefinitiva") &&
                    _contratoPre.Attributes.Contains("atos_fechafindefinitiva") &&
                    _pricingOutput.Attributes.Contains("atos_fechafinaplicacion"))
                {
                    if ((DateTime)_contratoPost.Attributes["atos_fechafindefinitiva"] > (DateTime)_contratoPre.Attributes["atos_fechafindefinitiva"] &&
                        (DateTime)_pricingOutput.Attributes["atos_fechafinaplicacion"] == (DateTime)_contratoPre.Attributes["atos_fechafindefinitiva"])
                    {
                        _pricingOutput.Attributes["atos_fechafinaplicacion"] = (DateTime)_contratoPost.Attributes["atos_fechafindefinitiva"];
                        actualiza = true;
                    }
                    else
                        _pricingOutput.Attributes.Remove("atos_fechafinaplicacion");
                }
                else
                    _pricingOutput.Attributes.Remove("atos_fechafinaplicacion");

                if (actualiza)
                    service.Update(_pricingOutput);
            }
        }

        /**
        // <summary>
        // Punto de entrada del plugin.
        // </summary>
        // <param name="serviceProvider">The service provider.</param>
        // <remarks>
        // - Si se cambia la fecha de inicio efectiva o la fecha fin definitiva del contrato, modifica las fechas correspondientes en sus pricing outputs.
        // </remarks>
         */
        public void Execute(IServiceProvider serviceProvider)
        {
            tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            PluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the Organization Service factory service from the service provider
            factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            // Use the factory to generate the Organization Service.
            service = factory.CreateOrganizationService(PluginExecutionContext.UserId);
            
            writelog("-----------------------------------------");
            writelog(DateTime.Now.ToLocalTime().ToString());
            writelog("Plugin Actualizar fechas Pricing Output para un contrato");
            writelog("Mensaje: " + PluginExecutionContext.MessageName);
            if (PluginExecutionContext.MessageName == "Update")
            {
                Entity ef = (Entity)PluginExecutionContext.InputParameters["Target"];

                // Si no es contrato o no se han modificado las fechas inicio efectiva ni la fecha fin definitiva no debe hacer nada
                if (ef.LogicalName != "atos_contrato" ||
                    (ef.Attributes.Contains("atos_fechainicioefectiva") == false &&
                    ef.Attributes.Contains("atos_fechafindefinitiva") == false))
                    return;
                writelog("Está cambiando fecha inicio efectiva o fecha fin definitiva");
                Entity _preImage = PluginExecutionContext.PreEntityImages["PreEntityImage"];

                if (_preImage.Attributes.Contains("atos_fechainicioefectiva") == false ||
                    _preImage.Attributes.Contains("atos_fechafindefinitiva") == false)
                    return;
                writelog("Existe fecha inicio efectiva y fecha fin definitiva en los valores anteriores");
                // Si no cambian la fecha de inicio a una fecha anterior o no cambian la fecha de fin a una fecha posterior no se hace nada

                bool actualizarFechas = false;
                if (ef.Attributes.Contains("atos_fechainicioefectiva"))
                {
                    if ((DateTime)_preImage.Attributes["atos_fechainicioefectiva"] > (DateTime)ef.Attributes["atos_fechainicioefectiva"])
                    {
                        actualizarFechas = true;
                    }
                }


                if (ef.Attributes.Contains("atos_fechafindefinitiva"))
                {
                    if ((DateTime)ef.Attributes["atos_fechafindefinitiva"] > (DateTime)_preImage.Attributes["atos_fechafindefinitiva"])
                    {
                        actualizarFechas = true;
                    }
                }

                if ( actualizarFechas == false)
                    return;

                modificaPricingOutputs(_preImage, ef);

            }
        }
    }
}