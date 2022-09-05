﻿// <copyright file="ValidacionRazonSocial.cs" company="Licence Owner">
// Copyright (c) 2017 All Rights Reserved
// </copyright>
// <author>Licence Owner</author>
// <date>16/08/2017 11:11:40</date>
// <summary>Implements the ValidacionRazonSocial Workflow Activity.</summary>
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
// </auto-generated>
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using System.Xml;
using Microsoft.Xrm.Sdk.Query;

namespace ValidacionRazonSocial
{
    /// <summary>
    /// Base class for all plug-in classes.
    /// </summary>    
    public class ValidacionRazonSocial : IPlugin
    {

        private CommonWS.Log Log = null;

        /// <summary>
        /// Plug-in context object. 
        /// </summary>
        protected class LocalPluginContext
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "LocalPluginContext")]
            internal IServiceProvider ServiceProvider { get; private set; }

            /// <summary>
            /// The Microsoft Dynamics 365 organization service.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "LocalPluginContext")]
            internal IOrganizationService OrganizationService { get; private set; }

            /// <summary>
            /// IPluginExecutionContext contains information that describes the run-time environment in which the plug-in executes, information related to the execution pipeline, and entity business information.
            /// </summary>
            internal IPluginExecutionContext PluginExecutionContext { get; private set; }

            /// <summary>
            /// Synchronous registered plug-ins can post the execution context to the Microsoft Azure Service Bus. <br/> 
            /// It is through this notification service that synchronous plug-ins can send brokered messages to the Microsoft Azure Service Bus.
            /// </summary>
            internal IServiceEndpointNotificationService NotificationService { get; private set; }

            /// <summary>
            /// Provides logging run-time trace information for plug-ins. 
            /// </summary>
            internal ITracingService TracingService { get; private set; }

            private LocalPluginContext() { }

            /// <summary>
            /// Helper object that stores the services available in this plug-in.
            /// </summary>
            /// <param name="serviceProvider"></param>
            internal LocalPluginContext(IServiceProvider serviceProvider)
            {
                if (serviceProvider == null)
                {
                    throw new InvalidPluginExecutionException("serviceProvider");
                }

                // Obtain the execution context service from the service provider.
                PluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

                // Obtain the tracing service from the service provider.
                TracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                // Get the notification service from the service provider.
                NotificationService = (IServiceEndpointNotificationService)serviceProvider.GetService(typeof(IServiceEndpointNotificationService));

                // Obtain the organization factory service from the service provider.
                IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                // Use the factory to generate the organization service.
                OrganizationService = factory.CreateOrganizationService(PluginExecutionContext.UserId);
            }

            /// <summary>
            /// Writes a trace message to the CRM trace log.
            /// </summary>
            /// <param name="message">Message name to trace.</param>
            internal void Trace(string message)
            {
                if (string.IsNullOrWhiteSpace(message) || TracingService == null)
                {
                    return;
                }

                if (PluginExecutionContext == null)
                {
                    TracingService.Trace(message);
                }
                else
                {
                    TracingService.Trace(
                        "{0}, Correlation Id: {1}, Initiating User: {2}",
                        message,
                        PluginExecutionContext.CorrelationId,
                        PluginExecutionContext.InitiatingUserId);
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the child class.
        /// </summary>
        /// <value>The name of the child class.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "ValidacionRazonSocial")]
        protected string ChildClassName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidacionRazonSocial"/> class.
        /// </summary>
        /// <param name="childClassName">The <see cref=" cred="Type"/> of the derived class.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "ValidacionRazonSocial")]
        public ValidacionRazonSocial(String unsecureconfiguration, String secureconfiguration)
        {
            //ChildClassName = childClassName.ToString();

            Log = new CommonWS.Log();
            String parametros = "";
            if (secureconfiguration != "")
                parametros = secureconfiguration;
            else if (unsecureconfiguration != "")
                parametros = unsecureconfiguration;


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

        /// <summary>
        /// Main entry point for he business logic that the plug-in is to execute.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <remarks>
        /// For improved performance, Microsoft Dynamics 365 caches plug-in instances. 
        /// The plug-in's Execute method should be written to be stateless as the constructor 
        /// is not called for every invocation of the plug-in. Also, multiple system threads 
        /// could execute the plug-in at the same time. All per invocation state information 
        /// is stored in the context. This means that you should not use global variables in plug-ins.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "CrmVSSolution411.NewProj.ValidacionRazonSocial+LocalPluginContext.Trace(System.String)", Justification = "Execute")]
        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new InvalidPluginExecutionException("serviceProvider");
            }

            // Construct the local plug-in context.
            LocalPluginContext localcontext = new LocalPluginContext(serviceProvider);

            Log.tracingService = localcontext.TracingService;

            //localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "Entered TermPricing.Execute()"));
            writelog("---------------------------------------------------------------", true);
            writelog(string.Format(CultureInfo.InvariantCulture, "Entered {0}.Execute()", this.ChildClassName), true);

            try
            {
                // Invoke the custom implementation 
                ExecuteCrmPlugin(localcontext);
                // now exit - if the derived plug-in has incorrectly registered overlapping event registrations,
                // guard against multiple executions.
                return;
            }
            catch (FaultException<OrganizationServiceFault> e)
            {
                writelog(string.Format(CultureInfo.InvariantCulture, "Exception: {0}", e.ToString()), true);

                // Handle the exception.
                throw new InvalidPluginExecutionException("OrganizationServiceFault", e);
            }
            finally
            {
                writelog(string.Format(CultureInfo.InvariantCulture, "Exiting {0}.Execute()", this.ChildClassName), true);
            }
        }

        /// <summary>
        /// Placeholder for a custom plug-in implementation. 
        /// </summary>
        /// <param name="localcontext">Context for the current plug-in.</param>
        protected virtual void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            Entity _updateRS = (Entity)localcontext.PluginExecutionContext.InputParameters["Target"];

            // Do.
            if (localcontext.PluginExecutionContext.MessageName == "Update")
            {
                
                if (_updateRS.Attributes.Contains("atos_cnaeid") 
                    && _updateRS.Attributes["atos_cnaeid"] == null 
                    && razonSocialTieneContratosAsociados(_updateRS.Id, localcontext.OrganizationService)
                    )
                {
                    throw new InvalidPluginExecutionException("Las razones sociales que tiene asociado contratos no pueden tener CNAE sin informar");
                }

            }
        }

        private bool razonSocialTieneContratosAsociados(Guid accountId, IOrganizationService service)
        {
            QueryExpression _consulta = new QueryExpression("account");
            _consulta.ColumnSet.AddColumns("accountid");

            LinkEntity link = _consulta.AddLink("atos_contrato", "accountid", "atos_razonsocialid", JoinOperator.Inner);


            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;

            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "accountid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(accountId.ToString());
            _filtro.Conditions.Add(_condicion);

            _consulta.Criteria.AddFilter(_filtro);

            EntityCollection _resConsulta =  service.RetrieveMultiple(_consulta);

            if (_resConsulta.Entities.Count() > 0)
            {
                return true;
            }else
            {
                return false;
            }
        }

        private void writelog(String texto, bool _traza = false)
        {

            Log.writelog(texto, _traza);
            /*if (_traza)
                localcontext.Trace(texto);
            if (_log == true)
                CommonWS.CommonWS.WriteLog(urlwslog, ficherolog, subcarpetalog, texto);*/
        }
    }
}