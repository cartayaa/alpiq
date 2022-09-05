/*
 File="atos_oferta.js" 
 Copyright (c) Atos. All rights reserved.

 Plugin que se ejecuta cuando se crea un nuevo registro en atos_trigger con los valores OfertaMP para accion y account para la entidad.

 Fecha 		Codigo  Version Descripcion                                     Autor
 05.09.2022 23866   no-lock Incorporacion del No-lock a Consultas           AC
 05.09.2022 23866   fecha   Actualiacion del campo atos_fechafin            Ac
                            por atos_fechafin_tza
*/

namespace OfertaMP
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
   // using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System.Text;
    using Microsoft.Crm.Sdk.Messages;


    public class OfertaMP : IPlugin
    {

        // Tipos de Ofertas
        const int MULTIPUNTO = 300000000;
        const int SUBOFERTA = 300000001;
        const int OFERTA = 300000002;

        // Commodity 
        const int POWER = 300000000;
        const int GAS = 300000001;

        #region Conexions CRM

        /**
        // <summary>
        // Clase a través de la cual se realizan las conexiones con CRM
        // </summary>
         */

        protected class LocalPluginContext
        {
            internal IServiceProvider ServiceProvider
            {
                get;

                private set;
            }

            internal IOrganizationServiceFactory OrganizationServiceFactory
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
                this.OrganizationServiceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                // Use the factory to generate the Organization Service.
                this.OrganizationService = this.OrganizationServiceFactory.CreateOrganizationService(this.PluginExecutionContext.UserId);
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

        /**
        // <summary>
        // Colección de eventos para los que el plugin se disparará
        // </summary>
        // <remarks>
        // Sin uso.
        // </remarks>
         */
        /*private Collection<Tuple<int, string, string, Action<LocalPluginContext>>> registeredEvents;*/


        /**
        // <summary>
        // Recibe la lista de eventos para los que el plugin se disparará
        // </summary>
        // <remarks>
        // Sin uso.
        // </remarks>
         */
        /*protected Collection<Tuple<int, string, string, Action<LocalPluginContext>>> RegisteredEvents
        {
            get
            {
                if (this.registeredEvents == null)
                {
                    this.registeredEvents = new Collection<Tuple<int, string, string, Action<LocalPluginContext>>>();
                }

                return this.registeredEvents;
            }
        }*/
        #endregion

        /*
		* Punto de entrada al plugin
		* serviceProvider
		* - Recuperamos las instalaciones de la razon social (o cuenta negociadora) y 
        *   las ordenamos por Sistema electrico y Tarifa
		* - Cada vez que cambie Sistema eléctrico y Tarifa creamos una suboferta.
		* - Por cada instalación creamos una oferta colgando de la suboferta.
        */
        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            // Construct the Local plug-in context.
            LocalPluginContext localcontext = new LocalPluginContext(serviceProvider);

            // AC localcontext.Trace(string.Format(CultureInfo.InvariantCulture, "Entered {0}.Execute()", this.GetType().ToString()));

            if (localcontext.PluginExecutionContext.MessageName != "Create")
                return;

            Entity ef = (Entity)localcontext.PluginExecutionContext.InputParameters["Target"];

            //if (ef.LogicalName != "atos_trigger")
            if (ef.LogicalName != "atos_oferta")
                return;

            /*if (!ef.Attributes.Contains("atos_accion") || (string) ef.Attributes["atos_accion"] != "OfertaMP" ||
                !ef.Attributes.Contains("atos_entity") || ((string)ef.Attributes["atos_entity"] != "account" && 
                (string)ef.Attributes["atos_entity"] != "atos_cuentanegociadora") ||
                !ef.Attributes.Contains("atos_guid") || (string) ef.Attributes["atos_guid"] == "")
                return;*/

            if (!ef.Attributes.Contains("atos_cuentanegociadoraid") ||
                !ef.Attributes.Contains("atos_tipooferta") ||
                (ef.Attributes.Contains("atos_tipooferta") && ((OptionSetValue)ef.Attributes["atos_tipooferta"]).Value != 300000000))
            {
                // Debe ser una oferta multipunto
                localcontext.Trace("No es una oferta Multipunto, Ok");
                return;
            }

            // Recuperamos el id de la Razón Social
            //Guid id = new Guid(ef.Attributes["atos_guid"].ToString());

            Entity padremultipunto;
            Guid _cuentanegociadoraId = Guid.Empty;
            Guid _razonsocialId = Guid.Empty;

            if (ef.Attributes.Contains("atos_razonsocialid"))
            {
                padremultipunto = localcontext.OrganizationService.Retrieve("account", ((EntityReference)ef.Attributes["atos_razonsocialid"]).Id, new ColumnSet(true));
                _razonsocialId = padremultipunto.Id;
                _cuentanegociadoraId = ((EntityReference)padremultipunto.Attributes["atos_cuentanegociadoraid"]).Id;
            }
            else
            {
                padremultipunto = localcontext.OrganizationService.Retrieve("atos_cuentanegociadora", ((EntityReference)ef.Attributes["atos_cuentanegociadoraid"]).Id, new ColumnSet(true));
                _cuentanegociadoraId = padremultipunto.Id;
            }

            //Entity padremultipunto = localcontext.OrganizationService.Retrieve((string)ef.Attributes["atos_entity"], id, new ColumnSet(true));

            //Guid _cuentanegociadoraId = Guid.Empty;
            //Guid _razonsocialId = Guid.Empty;
            Guid ofertaPadreId = Guid.Empty;

            //throw new Exception("Provocada");
            /*if (padremultipunto.LogicalName == "atos_cuentanegociadora")
                _cuentanegociadoraId = padremultipunto.Id;
            else
            {
                _razonsocialId = padremultipunto.Id;
                Entity _RazonSocial = localcontext.OrganizationService.Retrieve("account", _razonsocialId, new ColumnSet(new String[] { "atos_cuentanegociadoraid" }));
                _cuentanegociadoraId = ((EntityReference)_RazonSocial.Attributes["atos_cuentanegociadoraid"]).Id;
            }
            */
            // Creamos oferta padre colgando de la Razón Social
            //ofertaPadreId = creaOferta(localcontext, 300000000, _cuentanegociadoraId, _razonsocialId, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty);

            ofertaPadreId = ef.Id;

            //localcontext.Trace("Creada oferta padre");

            //throw new System.Exception("Provocada por usuario");



            //if (((OptionSetValue)ef.Attributes["atos_commodity"]).Value == 300000000)
            if (((OptionSetValue)ef.Attributes["atos_commodity"]).Value == POWER)
            {
                //Creacion de ofertas para instalaciones electricas
                recorerInstalacionesPowerRS(padremultipunto, _cuentanegociadoraId, 
                                _razonsocialId, ofertaPadreId, localcontext, ef);
            }
            else
            {
                //Creacion de ofertas para instalaciones de gas
                recorerInstalacionesGasRS(padremultipunto, _cuentanegociadoraId, 
                                _razonsocialId, ofertaPadreId, localcontext, ef);
            }
        }

        /*
		// Crea una oferta colgando de la instalación.
		// <param name="_localcontext">LocalPluginContext</param>
		// <param name="tipoOferta">Tipo de la oferta (300000000 - Multipunto, 300000001 - Suboferta, 300000002 - Oferta)</param>
		// <param name="_cuentanegociadoraId">Guid de la cuenta negociadora</param>
		// <param name="_razonSocialId">Guid de la razón social</param>
		// <param name="_ofertaPadreId">Guid de la cuenta padre (Empty si es la multipunto)</param>
		// <param name="_instalacionId">Guid de la instalación</param>
		// <param name="_sistemaElectricoId">Guid del sistema eléctrico</param>
		// <param name="_tarifaId">Guid de la tarifa</param>
		// 
		// La oferta creada puede ser de alguno de los siguientes tipos:
        // - Multipunto (cuelga de una razón social o de una cuenta negociadora)
        // - Suboferta (agrupación por Tarifa y Sistema Eléctrico
        // - Oferta (cuelga de una instalación)
         */
        private Guid creaOferta(LocalPluginContext _localcontext, int tipoOferta, Guid _cuentanegociadoraId, Guid _razonSocialId, 
            Guid _ofertaPadreId, Guid _instalacionId,
            Guid _subSistemaElectricoId, Guid _sistemaElectricoId, Guid _tarifaId, Entity _ofertaPadre, String _lote,
            String _nombreTarifa, String _nombreSistemaElectrico, String _nombreSubSistemaElectrico, String _nombreInstalacion)
        {
            Entity _oferta = new Entity("atos_oferta");

            _localcontext.Trace("Oferta: " + tipoOferta.ToString());
            _oferta.Attributes["atos_tipooferta"] = new OptionSetValue(tipoOferta);

            if (_cuentanegociadoraId != Guid.Empty)
                _oferta.Attributes["atos_cuentanegociadoraid"] = new EntityReference("atos_cuentanegociadora", _cuentanegociadoraId);
            if (_razonSocialId != Guid.Empty)
            {
                _oferta.Attributes["atos_razonsocialid"] = new EntityReference("account", _razonSocialId);

                Entity razonSocial = _localcontext.OrganizationService.Retrieve("account", _razonSocialId, 
                    new ColumnSet("atos_grupoempresarialid", "atos_formadepago", "atos_condicionpagoid", "atos_tipodeenvio", "atos_plazoenviofacturas", "atos_mandatosepa", "atos_swift", "atos_iban", "atos_entidadbancaria", "atos_sucursalbancaria", "atos_digitocontrol", "atos_cuenta", "atos_cuentabancaria", "atos_cuentabancariaapropia"));

                if (razonSocial.Contains("atos_grupoempresarialid"))
                    _oferta.Attributes["atos_grupoempresarialid"] = new EntityReference("atos_grupoempresarial", ((EntityReference)razonSocial.Attributes["atos_grupoempresarialid"]).Id);
                
                if (razonSocial.Contains("atos_formadepago"))
                    _oferta.Attributes["atos_formadepago"] = razonSocial.Attributes["atos_formadepago"];

                if (razonSocial.Contains("atos_condicionpagoid"))
                    _oferta.Attributes["atos_condicionpagoid"] = new EntityReference("atos_condiciondepago", ((EntityReference)razonSocial.Attributes["atos_condicionpagoid"]).Id);

                if (razonSocial.Contains("atos_tipodeenvio"))
                    _oferta.Attributes["atos_tipodeenvio"] = razonSocial.Attributes["atos_tipodeenvio"];

                if (razonSocial.Contains("atos_plazoenviofacturas"))
                    _oferta.Attributes["atos_plazoenviofacturas"] = razonSocial.Attributes["atos_plazoenviofacturas"];

                if (razonSocial.Contains("atos_mandatosepa"))
                    _oferta.Attributes["atos_mandatosepa"] = razonSocial.Attributes["atos_mandatosepa"];

                if (razonSocial.Contains("atos_swift"))
                    _oferta.Attributes["atos_swift"] = razonSocial.Attributes["atos_swift"];

                if (razonSocial.Contains("atos_iban"))
                    _oferta.Attributes["atos_iban"] = razonSocial.Attributes["atos_iban"];

                if (razonSocial.Contains("atos_entidadbancaria"))
                    _oferta.Attributes["atos_entidadbancaria"] = razonSocial.Attributes["atos_entidadbancaria"];

                if (razonSocial.Contains("atos_sucursalbancaria"))
                    _oferta.Attributes["atos_sucursalbancaria"] = razonSocial.Attributes["atos_sucursalbancaria"];

                if (razonSocial.Contains("atos_digitocontrol"))
                    _oferta.Attributes["atos_digitocontrol"] = razonSocial.Attributes["atos_digitocontrol"];

                if (razonSocial.Contains("atos_cuenta"))
                    _oferta.Attributes["atos_cuenta"] = razonSocial.Attributes["atos_cuenta"];

                if (razonSocial.Contains("atos_cuentabancaria"))
                    _oferta.Attributes["atos_cuentabancaria"] = razonSocial.Attributes["atos_cuentabancaria"];

                if (razonSocial.Contains("atos_cuentabancariaapropia"))
                    _oferta.Attributes["atos_cuentabancariaapropia"] = razonSocial.Attributes["atos_cuentabancariaapropia"];
            }
            if (_ofertaPadreId != Guid.Empty)
                _oferta.Attributes["atos_ofertapadreid"] = new EntityReference("atos_oferta", _ofertaPadreId);
            if (_instalacionId != Guid.Empty)
                _oferta.Attributes["atos_instalacionid"] = new EntityReference("atos_instalacion", _instalacionId);
            if (_subSistemaElectricoId != Guid.Empty)
                _oferta.Attributes["atos_subsistemaid"] = new EntityReference("atos_subsistema", _subSistemaElectricoId);
            if (_sistemaElectricoId != Guid.Empty)
                _oferta.Attributes["atos_sistemaelectricoid"] = new EntityReference("atos_sistemaelectrico", _sistemaElectricoId);
            if (_tarifaId != Guid.Empty)
                _oferta.Attributes["atos_tarifaid"] = new EntityReference("atos_tarifa", _tarifaId);
            if (_cuentanegociadoraId != Guid.Empty)
                _localcontext.Trace("Con Cuenta Negociadora");
            if (_razonSocialId != Guid.Empty)
                _localcontext.Trace("Con Razón Social");
            if (_ofertaPadreId != Guid.Empty)
                _localcontext.Trace("Con Oferta Padre");
            if (_instalacionId != Guid.Empty)
                _localcontext.Trace("Con Instalacion");
            if (_subSistemaElectricoId != Guid.Empty)
                _localcontext.Trace("Con Subsistema Electrico");
            if (_sistemaElectricoId != Guid.Empty)
                _localcontext.Trace("Con Sistema Electrico");
            if (_tarifaId != Guid.Empty)
                _localcontext.Trace("Con Tarifa");

            if (_ofertaPadre.Attributes.Contains("atos_agentecomercialid"))
                _oferta.Attributes["atos_agentecomercialid"] = new EntityReference("atos_agentecomercial", ((EntityReference)_ofertaPadre.Attributes["atos_agentecomercialid"]).Id);

            // El lote debe salir de la instalación
            //if (_ofertaPadre.Attributes.Contains("atos_lote"))
            //    _oferta.Attributes["atos_lote"] = _ofertaPadre.Attributes["atos_lote"];
            if (_lote != "")  _oferta.Attributes["atos_lote"] = _lote;

            String nombrePadre = _ofertaPadre.Attributes.Contains("atos_nombreoferta") ? _ofertaPadre.Attributes["atos_nombreoferta"].ToString() : String.Empty;
            //if (tipoOferta == 300000001)
            if (tipoOferta == SUBOFERTA)
                _oferta.Attributes["atos_nombreoferta"] = componerNombreSubOferta(nombrePadre, _nombreTarifa, _nombreSubSistemaElectrico, _lote);
            //else if (tipoOferta == 300000002)
            else if (tipoOferta == OFERTA)
            {
                if (String.IsNullOrEmpty(nombrePadre))
                {
                    _oferta.Attributes["atos_nombreoferta"] = _nombreInstalacion;
                }
                else
                {
                    _oferta.Attributes["atos_nombreoferta"] = String.Format("{0}-{1}", nombrePadre, _nombreInstalacion);
                }
            }

            if (_ofertaPadre.Attributes.Contains("atos_commodity"))
                _oferta.Attributes["atos_commodity"] = _ofertaPadre.Attributes["atos_commodity"];

            if (_ofertaPadre.Attributes.Contains("atos_numerodocumento"))
                _oferta.Attributes["atos_numerodocumento"] = _ofertaPadre.Attributes["atos_numerodocumento"];

            if (_ofertaPadre.Attributes.Contains("atos_gestionatr"))
                _oferta.Attributes["atos_gestionatr"] = _ofertaPadre.Attributes["atos_gestionatr"];

            if (_ofertaPadre.Attributes.Contains("atos_impuestoelectrico"))
                _oferta.Attributes["atos_impuestoelectrico"] = _ofertaPadre.Attributes["atos_impuestoelectrico"];

            if (_ofertaPadre.Attributes.Contains("atos_fechafinvigenciaoferta"))
                _oferta.Attributes["atos_fechafinvigenciaoferta"] = _ofertaPadre.Attributes["atos_fechafinvigenciaoferta"];

            if (_ofertaPadre.Attributes.Contains("atos_duracionmeses"))
                _oferta.Attributes["atos_duracionmeses"] = _ofertaPadre.Attributes["atos_duracionmeses"];

            if (_ofertaPadre.Attributes.Contains("atos_grupoempresarialid"))
                _oferta.Attributes["atos_grupoempresarialid"] = new EntityReference("atos_grupoempresarial", ((EntityReference)_ofertaPadre.Attributes["atos_grupoempresarialid"]).Id);

            if (_ofertaPadre.Attributes.Contains("atos_fechalimitepresentacionoferta"))
                _oferta.Attributes["atos_fechalimitepresentacionoferta"] = _ofertaPadre.Attributes["atos_fechalimitepresentacionoferta"];

            if (_ofertaPadre.Attributes.Contains("atos_fechainicio"))
                _oferta.Attributes["atos_fechainicio"] = _ofertaPadre.Attributes["atos_fechainicio"];

            /* 23866 -+2 */
            //if (_ofertaPadre.Attributes.Contains("atos_fechafin"))
            //    _oferta.Attributes["atos_fechafin"] = _ofertaPadre.Attributes["atos_fechafin"];
            if (_ofertaPadre.Attributes.Contains("atos_fechafin_tza"))
                _oferta.Attributes["atos_fechafin_tza"] = _ofertaPadre.Attributes["atos_fechafin_tza"];

            if (_ofertaPadre.Attributes.Contains("atos_penalizacionconsumo"))
                _oferta.Attributes["atos_penalizacionconsumo"] = _ofertaPadre.Attributes["atos_penalizacionconsumo"];

            if (_ofertaPadre.Attributes.Contains("atos_importepenalizacion"))
                _oferta.Attributes["atos_importepenalizacion"] = _ofertaPadre.Attributes["atos_importepenalizacion"];

            if (_ofertaPadre.Attributes.Contains("atos_rangoinferiorpenalizacion"))
                _oferta.Attributes["atos_rangoinferiorpenalizacion"] = _ofertaPadre.Attributes["atos_rangoinferiorpenalizacion"];

            if (_ofertaPadre.Attributes.Contains("atos_rangosuperiorpenalizacion"))
                _oferta.Attributes["atos_rangosuperiorpenalizacion"] = _ofertaPadre.Attributes["atos_rangosuperiorpenalizacion"];

            if (_ofertaPadre.Attributes.Contains("atos_bonificacionconsumo"))
                _oferta.Attributes["atos_bonificacionconsumo"] = _ofertaPadre.Attributes["atos_bonificacionconsumo"];

            if (_ofertaPadre.Attributes.Contains("atos_importebonificacion"))
                _oferta.Attributes["atos_importebonificacion"] = _ofertaPadre.Attributes["atos_importebonificacion"];

            if (_ofertaPadre.Attributes.Contains("atos_rangoinferiorbonificacion"))
                _oferta.Attributes["atos_rangoinferiorbonificacion"] = _ofertaPadre.Attributes["atos_rangoinferiorbonificacion"];

            if (_ofertaPadre.Attributes.Contains("atos_rangosuperiorbonificacion"))
                _oferta.Attributes["atos_rangosuperiorbonificacion"] = _ofertaPadre.Attributes["atos_rangosuperiorbonificacion"];

            if (_ofertaPadre.Attributes.Contains("atos_estadoriesgo"))
                _oferta.Attributes["atos_estadoriesgo"] = _ofertaPadre.Attributes["atos_estadoriesgo"];

            if (_ofertaPadre.Attributes.Contains("transactioncurrencyid"))
                _oferta.Attributes["transactioncurrencyid"] = new EntityReference("transactioncurrency", ((EntityReference)_ofertaPadre.Attributes["transactioncurrencyid"]).Id);

            if (_ofertaPadre.Attributes.Contains("atos_numclicsmensual"))
                _oferta.Attributes["atos_numclicsmensual"] = _ofertaPadre.Attributes["atos_numclicsmensual"];

            if (_ofertaPadre.Attributes.Contains("atos_numclicstrimestral"))
                _oferta.Attributes["atos_numclicstrimestral"] = _ofertaPadre.Attributes["atos_numclicstrimestral"];

            if (_ofertaPadre.Attributes.Contains("atos_numclicsanual"))
                _oferta.Attributes["atos_numclicsanual"] = _ofertaPadre.Attributes["atos_numclicsanual"];

            // campos de congelacion precios
            if (_ofertaPadre.Attributes.Contains("atos_congelacionpreciosente"))
                _oferta.Attributes["atos_congelacionpreciosente"] = _ofertaPadre.Attributes["atos_congelacionpreciosente"];

            if (_ofertaPadre.Attributes.Contains("atos_congelacionpreciosentp"))
                _oferta.Attributes["atos_congelacionpreciosentp"] = _ofertaPadre.Attributes["atos_congelacionpreciosentp"];

            //campo de motivo de rechazo
            if (_ofertaPadre.Attributes.Contains("atos_motivorechazoofertaid"))
                _oferta.Attributes["atos_motivorechazoofertaid"] = new EntityReference("atos_motivorechazooferta", ((EntityReference)_ofertaPadre.Attributes["atos_motivorechazoofertaid"]).Id);

            // campos de fechas relacionados con el estado
            // fecha aceptacion de oferta
            if (_ofertaPadre.Attributes.Contains("atos_fechaaceptacionoferta"))
                _oferta.Attributes["atos_fechaaceptacionoferta"] = _ofertaPadre.Attributes["atos_fechaaceptacionoferta"];
            // fecha rechazo de oferta
            if (_ofertaPadre.Attributes.Contains("atos_fecharechazodeoferta"))
                _oferta.Attributes["atos_fecharechazodeoferta"] = _ofertaPadre.Attributes["atos_fecharechazodeoferta"];
            // Estado de oferta
            if (_ofertaPadre.Contains("statuscode"))
                _oferta.Attributes.Add("statuscode", new OptionSetValue(((OptionSetValue)(_ofertaPadre.Attributes["statuscode"])).Value));

            _localcontext.Trace("Create _oferta");

            Guid ofertaId = _localcontext.OrganizationService.Create(_oferta);

            return ofertaId;
        }

        /*
        // Crea una oferta gas colgando de la instalación.
        // <param name="_localcontext">LocalPluginContext</param>
        // <param name="tipoOferta">Tipo de la oferta (300000000 - Multipunto, 300000001 - Suboferta, 300000002 - Oferta)</param>
        // <param name="_cuentanegociadoraId">Guid de la cuenta negociadora</param>
        // <param name="_razonSocialId">Guid de la razón social</param>
        // <param name="_ofertaPadreId">Guid de la cuenta padre (Empty si es la multipunto)</param>
        // <param name="_instalacionId">Guid de la instalación</param>
        // <param name="_sistemaElectricoId">Guid del sistema eléctrico</param>
        // <param name="_tarifaId">Guid de la tarifa</param>
        // 
        // La oferta creada puede ser de alguno de los siguientes tipos:
        // - Multipunto (cuelga de una razón social o de una cuenta negociadora)
        // - Suboferta (agrupación por Tarifa y Sistema Eléctrico
        // - Oferta (cuelga de una instalación)
         */
        private Guid creaOfertaGas(LocalPluginContext _localcontext, int tipoOferta, Guid _cuentanegociadoraId, 
            Guid _razonSocialId, Guid _ofertaPadreId, Guid _instalaciongasId, Guid _peajeId, Entity _ofertaPadre,
            String _nombrePeaje, String _nombreInstalacion)
        {
            Entity _oferta = new Entity("atos_oferta");
            _localcontext.Trace("Oferta: " + tipoOferta.ToString());
            _oferta.Attributes["atos_tipooferta"] = new OptionSetValue(tipoOferta);
            if (_cuentanegociadoraId != Guid.Empty)
                _oferta.Attributes["atos_cuentanegociadoraid"] = new EntityReference("atos_cuentanegociadora", _cuentanegociadoraId);
            if (_razonSocialId != Guid.Empty)
            {
                _oferta.Attributes["atos_razonsocialid"] = new EntityReference("account", _razonSocialId);
                Entity razonSocial = _localcontext.OrganizationService.Retrieve("account", _razonSocialId, new ColumnSet("atos_grupoempresarialid", "atos_formadepago", "atos_condicionpagoid", "atos_tipodeenvio", "atos_plazoenviofacturas", "atos_mandatosepa", "atos_swift", "atos_iban", "atos_entidadbancaria", "atos_sucursalbancaria", "atos_digitocontrol", "atos_cuenta", "atos_cuentabancaria", "atos_cuentabancariaapropia"));
                if (razonSocial.Contains("atos_grupoempresarialid"))
                    _oferta.Attributes["atos_grupoempresarialid"] = new EntityReference("atos_grupoempresarial", ((EntityReference)razonSocial.Attributes["atos_grupoempresarialid"]).Id);

                if (razonSocial.Contains("atos_formadepago"))
                    _oferta.Attributes["atos_formadepago"] = razonSocial.Attributes["atos_formadepago"];

                if (razonSocial.Contains("atos_condicionpagoid"))
                    _oferta.Attributes["atos_condicionpagoid"] = new EntityReference("atos_condiciondepago", ((EntityReference)razonSocial.Attributes["atos_condicionpagoid"]).Id);

                if (razonSocial.Contains("atos_tipodeenvio"))
                    _oferta.Attributes["atos_tipodeenvio"] = razonSocial.Attributes["atos_tipodeenvio"];

                if (razonSocial.Contains("atos_plazoenviofacturas"))
                    _oferta.Attributes["atos_plazoenviofacturas"] = razonSocial.Attributes["atos_plazoenviofacturas"];

                if (razonSocial.Contains("atos_mandatosepa"))
                    _oferta.Attributes["atos_mandatosepa"] = razonSocial.Attributes["atos_mandatosepa"];

                if (razonSocial.Contains("atos_swift"))
                    _oferta.Attributes["atos_swift"] = razonSocial.Attributes["atos_swift"];

                if (razonSocial.Contains("atos_iban"))
                    _oferta.Attributes["atos_iban"] = razonSocial.Attributes["atos_iban"];

                if (razonSocial.Contains("atos_entidadbancaria"))
                    _oferta.Attributes["atos_entidadbancaria"] = razonSocial.Attributes["atos_entidadbancaria"];

                if (razonSocial.Contains("atos_sucursalbancaria"))
                    _oferta.Attributes["atos_sucursalbancaria"] = razonSocial.Attributes["atos_sucursalbancaria"];

                if (razonSocial.Contains("atos_digitocontrol"))
                    _oferta.Attributes["atos_digitocontrol"] = razonSocial.Attributes["atos_digitocontrol"];

                if (razonSocial.Contains("atos_cuenta"))
                    _oferta.Attributes["atos_cuenta"] = razonSocial.Attributes["atos_cuenta"];

                if (razonSocial.Contains("atos_cuentabancaria"))
                    _oferta.Attributes["atos_cuentabancaria"] = razonSocial.Attributes["atos_cuentabancaria"];

                if (razonSocial.Contains("atos_cuentabancariaapropia"))
                    _oferta.Attributes["atos_cuentabancariaapropia"] = razonSocial.Attributes["atos_cuentabancariaapropia"];
            }
            if (_ofertaPadreId != Guid.Empty)
                _oferta.Attributes["atos_ofertapadreid"] = new EntityReference("atos_oferta", _ofertaPadreId);
            if (_instalaciongasId != Guid.Empty)
                _oferta.Attributes["atos_instalaciongasid"] = new EntityReference("atos_instalacion", _instalaciongasId);
            if (_peajeId != Guid.Empty)
                _oferta.Attributes["atos_peajeid"] = new EntityReference("atos_tablasatrgas", _peajeId);
            
            if (_cuentanegociadoraId != Guid.Empty)
                _localcontext.Trace("Con Cuenta Negociadora");
            if (_razonSocialId != Guid.Empty)
                _localcontext.Trace("Con Razón Social");
            if (_ofertaPadreId != Guid.Empty)
                _localcontext.Trace("Con Oferta Padre");
            if (_instalaciongasId != Guid.Empty)
                _localcontext.Trace("Con Instalacion gas");
            if (_peajeId != Guid.Empty)
                _localcontext.Trace("Con Peaje");

            if (_ofertaPadre.Attributes.Contains("atos_agentecomercialid"))
                _oferta.Attributes["atos_agentecomercialid"] = new EntityReference("atos_agentecomercial", ((EntityReference)_ofertaPadre.Attributes["atos_agentecomercialid"]).Id);
            
            String nombrePadre = _ofertaPadre.Attributes.Contains("atos_nombreoferta") ? _ofertaPadre.Attributes["atos_nombreoferta"].ToString() : String.Empty;
            if (tipoOferta == 300000001)
                _oferta.Attributes["atos_nombreoferta"] = componerNombreSubOferta(nombrePadre, _nombrePeaje, null, null);
            else if (tipoOferta == 300000002)
            {
                if (String.IsNullOrEmpty(nombrePadre))
                {
                    _oferta.Attributes["atos_nombreoferta"] = _nombreInstalacion;
                }
                else
                {
                    _oferta.Attributes["atos_nombreoferta"] = String.Format("{0}-{1}", nombrePadre, _nombreInstalacion);
                }
            }

            if (_ofertaPadre.Attributes.Contains("atos_commodity"))
                _oferta.Attributes["atos_commodity"] = _ofertaPadre.Attributes["atos_commodity"];

            if (_ofertaPadre.Attributes.Contains("atos_numerodocumento"))
                _oferta.Attributes["atos_numerodocumento"] = _ofertaPadre.Attributes["atos_numerodocumento"];

            if (_ofertaPadre.Attributes.Contains("atos_gestionatr"))
                _oferta.Attributes["atos_gestionatr"] = _ofertaPadre.Attributes["atos_gestionatr"];
           
            if (_ofertaPadre.Attributes.Contains("atos_fechafinvigenciaoferta"))
                _oferta.Attributes["atos_fechafinvigenciaoferta"] = _ofertaPadre.Attributes["atos_fechafinvigenciaoferta"];

            if (_ofertaPadre.Attributes.Contains("atos_duracionmeses"))
                _oferta.Attributes["atos_duracionmeses"] = _ofertaPadre.Attributes["atos_duracionmeses"];

            if (_ofertaPadre.Attributes.Contains("atos_grupoempresarialid"))
                _oferta.Attributes["atos_grupoempresarialid"] = new EntityReference("atos_grupoempresarial", ((EntityReference)_ofertaPadre.Attributes["atos_grupoempresarialid"]).Id);

            if (_ofertaPadre.Attributes.Contains("atos_fechalimitepresentacionoferta"))
                _oferta.Attributes["atos_fechalimitepresentacionoferta"] = _ofertaPadre.Attributes["atos_fechalimitepresentacionoferta"];

            if (_ofertaPadre.Attributes.Contains("atos_fechainicio"))
                _oferta.Attributes["atos_fechainicio"] = _ofertaPadre.Attributes["atos_fechainicio"];

            /* 23866 -+2 */
            //if (_ofertaPadre.Attributes.Contains("atos_fechafin"))
            //    _oferta.Attributes["atos_fechafin"] = _ofertaPadre.Attributes["atos_fechafin"];
            if (_ofertaPadre.Attributes.Contains("atos_fechafin_tza"))
                _oferta.Attributes["atos_fechafin_tza"] = _ofertaPadre.Attributes["atos_fechafin_tza"];

            if (_ofertaPadre.Attributes.Contains("atos_penalizacionconsumo"))
                _oferta.Attributes["atos_penalizacionconsumo"] = _ofertaPadre.Attributes["atos_penalizacionconsumo"];

            if (_ofertaPadre.Attributes.Contains("atos_importepenalizacion"))
                _oferta.Attributes["atos_importepenalizacion"] = _ofertaPadre.Attributes["atos_importepenalizacion"];

            if (_ofertaPadre.Attributes.Contains("atos_rangoinferiorpenalizacion"))
                _oferta.Attributes["atos_rangoinferiorpenalizacion"] = _ofertaPadre.Attributes["atos_rangoinferiorpenalizacion"];

            if (_ofertaPadre.Attributes.Contains("atos_rangosuperiorpenalizacion"))
                _oferta.Attributes["atos_rangosuperiorpenalizacion"] = _ofertaPadre.Attributes["atos_rangosuperiorpenalizacion"];

            if (_ofertaPadre.Attributes.Contains("atos_bonificacionconsumo"))
                _oferta.Attributes["atos_bonificacionconsumo"] = _ofertaPadre.Attributes["atos_bonificacionconsumo"];

            if (_ofertaPadre.Attributes.Contains("atos_importebonificacion"))
                _oferta.Attributes["atos_importebonificacion"] = _ofertaPadre.Attributes["atos_importebonificacion"];

            if (_ofertaPadre.Attributes.Contains("atos_rangoinferiorbonificacion"))
                _oferta.Attributes["atos_rangoinferiorbonificacion"] = _ofertaPadre.Attributes["atos_rangoinferiorbonificacion"];

            if (_ofertaPadre.Attributes.Contains("atos_rangosuperiorbonificacion"))
                _oferta.Attributes["atos_rangosuperiorbonificacion"] = _ofertaPadre.Attributes["atos_rangosuperiorbonificacion"];

            if (_ofertaPadre.Attributes.Contains("atos_estadoriesgo"))
                _oferta.Attributes["atos_estadoriesgo"] = _ofertaPadre.Attributes["atos_estadoriesgo"];

            if (_ofertaPadre.Attributes.Contains("transactioncurrencyid"))
                _oferta.Attributes["transactioncurrencyid"] = new EntityReference("transactioncurrency", ((EntityReference)_ofertaPadre.Attributes["transactioncurrencyid"]).Id);

            // campos de congelacion precios
            if (_ofertaPadre.Attributes.Contains("atos_congelacionpreciosente"))
                _oferta.Attributes["atos_congelacionpreciosente"] = _ofertaPadre.Attributes["atos_congelacionpreciosente"];

            if (_ofertaPadre.Attributes.Contains("atos_congelacionpreciosentp"))
                _oferta.Attributes["atos_congelacionpreciosentp"] = _ofertaPadre.Attributes["atos_congelacionpreciosentp"];

            //campo de motivo de rechazo
            if (_ofertaPadre.Attributes.Contains("atos_motivorechazoofertaid"))
                _oferta.Attributes["atos_motivorechazoofertaid"] = new EntityReference("atos_motivorechazooferta", ((EntityReference)_ofertaPadre.Attributes["atos_motivorechazoofertaid"]).Id);

            // campos de fechas relacionados con el estado
            // fecha aceptacion de oferta
            if (_ofertaPadre.Attributes.Contains("atos_fechaaceptacionoferta"))
                _oferta.Attributes["atos_fechaaceptacionoferta"] = _ofertaPadre.Attributes["atos_fechaaceptacionoferta"];
            // fecha rechazo de oferta
            if (_ofertaPadre.Attributes.Contains("atos_fecharechazodeoferta"))
                _oferta.Attributes["atos_fecharechazodeoferta"] = _ofertaPadre.Attributes["atos_fecharechazodeoferta"];
            // Estado de oferta
            if (_ofertaPadre.Contains("statuscode"))
                _oferta.Attributes.Add("statuscode", new OptionSetValue(((OptionSetValue)(_ofertaPadre.Attributes["statuscode"])).Value));

            _localcontext.Trace("Create _oferta");

            try
            {
                Guid ofertaId = _localcontext.OrganizationService.Create(_oferta);
                return ofertaId;
            }
            catch( Exception e)
            {
                throw new Exception(String.Format("componerNombreSuboferta:{0}", e));
            }
            

            
        }


        /*
        * Función que devuelve el nombre que tomará la oferta en función de los datos de su oferta padre.
        *
        * <param name="pNombreOfertaPadre"></param>
        * <param name="pOferta"></param>
        */
        private string componerNombreSubOferta(String pNombreOfertaPadre, String pTarifa, String pSistemaElectrico, String pLote)
        {
            String salida = String.Empty;
            try
            {
                StringBuilder nombreOferta = new StringBuilder();
                if (!String.IsNullOrEmpty(pNombreOfertaPadre))
                    nombreOferta.AppendFormat("{0}-", pNombreOfertaPadre);

                if (!String.IsNullOrEmpty(pTarifa))
                    nombreOferta.AppendFormat("{0}-", pTarifa);

                if (!String.IsNullOrEmpty(pSistemaElectrico))
                    nombreOferta.Append(pSistemaElectrico);

                if (!String.IsNullOrEmpty(pLote))
                    nombreOferta.AppendFormat("-{0}", pLote);

                salida = nombreOferta.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("componerNombreSuboferta:{0}", ex.Message));
            }
            return salida;
        }

        /*
         * Recuperamos las instalaciones power de la razon social y las ordenamos por Sistema electrico y Tarifa
         * Cada vez que cambie Sistema eléctrico y Tarifa creamos una suboferta.
         * Por cada instalación creamos una oferta colgando de la suboferta.
         */
        private void recorerInstalacionesPowerRS(Entity padremultipunto, Guid _cuentanegociadoraId, Guid _razonsocialId, Guid ofertaPadreId, LocalPluginContext localcontext, Entity ef) {

            FilterExpression filtro = new FilterExpression();
            filtro.FilterOperator = LogicalOperator.And;

            ConditionExpression condicion;

            if (padremultipunto.LogicalName == "account")
            {
                condicion = new ConditionExpression();
                condicion.AttributeName = "atos_razonsocialid";
                condicion.Operator = ConditionOperator.Equal;
                condicion.Values.Add(_razonsocialId.ToString());
                filtro.Conditions.Add(condicion);
            }

            condicion = new ConditionExpression();
            condicion.AttributeName = "atos_historico";
            condicion.Operator = ConditionOperator.NotEqual;
            condicion.Values.Add((true));
            filtro.Conditions.Add(condicion);

            condicion = new ConditionExpression();
            condicion.AttributeName = "atos_sistemaelectricoid";
            condicion.Operator = ConditionOperator.NotNull;
            filtro.Conditions.Add(condicion);

            condicion = new ConditionExpression();
            condicion.AttributeName = "atos_tarifaid";
            condicion.Operator = ConditionOperator.NotNull;
            filtro.Conditions.Add(condicion);

            condicion = new ConditionExpression();
            condicion.AttributeName = "statecode";
            condicion.Operator = ConditionOperator.Equal;
            condicion.Values.Add(0);
            filtro.Conditions.Add(condicion);

            QueryExpression consulta = new QueryExpression("atos_instalacion");
            consulta.ColumnSet.AddColumns("atos_name", "atos_instalacionid", "atos_subsistemaid", "atos_sistemaelectricoid", "atos_tarifaid",
                                           "atos_razonsocialid", "atos_lote");
            consulta.Criteria.AddFilter(filtro);
            /* 23866 +1 NoLock */
            consulta.NoLock = true;

            //Si estamos haciendo una oferta a traves de cuenta negociadora
            //recuperaremos todas las razones sociales que esten relacionadas
            //con las instalaciones obtenidas de la cuenta negociadora
            if (padremultipunto.LogicalName == "atos_cuentanegociadora")
            {
                LinkEntity _link = new LinkEntity();
                _link.JoinOperator = JoinOperator.Inner;
                _link.LinkFromAttributeName = "atos_razonsocialid";
                _link.LinkFromEntityName = consulta.EntityName;
                _link.LinkToAttributeName = "accountid";
                _link.LinkToEntityName = "account";
                _link.LinkCriteria.AddCondition(new ConditionExpression("atos_cuentanegociadoraid", ConditionOperator.Equal, _cuentanegociadoraId.ToString()));

                consulta.LinkEntities.Add(_link);
            }

            OrderExpression orden;

            /*if (padremultipunto.LogicalName == "atos_cuentanegociadora")
            {
                orden = new OrderExpression();
                orden.AttributeName = "atos_razonsocialid";
                orden.OrderType = OrderType.Ascending;
                consulta.Orders.Add(orden);
            }*/

            orden = new OrderExpression();
            orden.AttributeName = "atos_subsistemaid";
            orden.OrderType = OrderType.Ascending;
            consulta.Orders.Add(orden);

            orden = new OrderExpression();
            orden.AttributeName = "atos_sistemaelectricoid";
            orden.OrderType = OrderType.Ascending;
            consulta.Orders.Add(orden);

            orden = new OrderExpression();
            orden.AttributeName = "atos_tarifaid";
            orden.OrderType = OrderType.Ascending;
            consulta.Orders.Add(orden);

            // Tiene que agrupar también por lote
            orden = new OrderExpression();
            orden.AttributeName = "atos_lote";
            orden.OrderType = OrderType.Ascending;
            consulta.Orders.Add(orden);


            EntityCollection resultado = localcontext.OrganizationService.RetrieveMultiple(consulta);

            Guid subSistemaElectricoId = Guid.Empty;
            Guid sistemaElectricoId = Guid.Empty;
            Guid tarifaId = Guid.Empty;

            String subSistemaElectricoNombre = String.Empty;
            String sistemaElectricoNombre = String.Empty;
            String tarifaNombre = String.Empty;

            Guid subOfertaId = Guid.Empty;
            String loteant = "";
            String lote = "";

            localcontext.Trace("Recuperadas " + resultado.Entities.Count.ToString() + " instalaciones");

            foreach (Entity i in resultado.Entities)
            {
                if (!i.Attributes.Contains("atos_lote"))
                    lote = "";
                else
                    lote = i.Attributes["atos_lote"].ToString();

                if (((EntityReference)i.Attributes["atos_subsistemaid"]).Id != subSistemaElectricoId ||
                    ((EntityReference)i.Attributes["atos_sistemaelectricoid"]).Id != sistemaElectricoId ||
                     ((EntityReference)i.Attributes["atos_tarifaid"]).Id != tarifaId ||
                      lote != loteant)
                {
                    subSistemaElectricoId = ((EntityReference)i.Attributes["atos_subsistemaid"]).Id;
                    subSistemaElectricoNombre = ((EntityReference)i.Attributes["atos_subsistemaid"]).Name;
                    sistemaElectricoId = ((EntityReference)i.Attributes["atos_sistemaelectricoid"]).Id;
                    sistemaElectricoNombre = ((EntityReference)i.Attributes["atos_sistemaelectricoid"]).Name;
                    tarifaId = ((EntityReference)i.Attributes["atos_tarifaid"]).Id;
                    tarifaNombre = ((EntityReference)i.Attributes["atos_tarifaid"]).Name;

                    loteant = lote;

                    _razonsocialId = ((EntityReference)i.Attributes["atos_razonsocialid"]).Id;

                    //subOfertaId = creaOferta(localcontext, 300000001, _cuentanegociadoraId,
                    subOfertaId = creaOferta(localcontext, SUBOFERTA, _cuentanegociadoraId,
                                             padremultipunto.LogicalName == "atos_cuentanegociadora" ? Guid.Empty : _razonsocialId,
                                             ofertaPadreId, Guid.Empty, subSistemaElectricoId, sistemaElectricoId, 
                                             tarifaId, ef, lote, tarifaNombre, sistemaElectricoNombre, 
                                             subSistemaElectricoNombre, String.Empty);

                    localcontext.Trace("Creada suboferta");
                }

                _razonsocialId = ((EntityReference)i.Attributes["atos_razonsocialid"]).Id;
                //creaOferta(localcontext, 300000002, _cuentanegociadoraId, _razonsocialId, subOfertaId, i.Id,
                creaOferta(localcontext, OFERTA, _cuentanegociadoraId, _razonsocialId, subOfertaId, i.Id, 
                           subSistemaElectricoId, sistemaElectricoId, tarifaId, ef, lote, tarifaNombre, 
                           sistemaElectricoNombre, subSistemaElectricoNombre, i.Attributes["atos_name"].ToString());

                localcontext.Trace("Creada oferta");
            }
        }

        /*
         * Recuperamos las instalaciones gas de la razon social y las ordenamos por peaje
         * Cada vez que cambie peaje creamos una suboferta.
         * Por cada instalación creamos una oferta colgando de la suboferta.
         */
        private void recorerInstalacionesGasRS(Entity padremultipunto, Guid _cuentanegociadoraId, Guid _razonsocialId, Guid ofertaPadreId, LocalPluginContext localcontext, Entity ef) {

            FilterExpression filtro = new FilterExpression();
            filtro.FilterOperator = LogicalOperator.And;

            ConditionExpression condicion;

            /**
             * Modificar condiciones para añadir los filtros para atos_instalaciongas
             */
            if (padremultipunto.LogicalName == "account")
            {
                condicion = new ConditionExpression();
                condicion.AttributeName = "atos_razonsocialid";
                condicion.Operator = ConditionOperator.Equal;
                condicion.Values.Add(_razonsocialId.ToString());
                filtro.Conditions.Add(condicion);
            }

            //atos_historico no esta en la entidad
            /*
            condicion = new ConditionExpression();
            condicion.AttributeName = "atos_historico";
            condicion.Operator = ConditionOperator.NotEqual;
            condicion.Values.Add((true));
            filtro.Conditions.Add(condicion);
            */

            condicion = new ConditionExpression();
            condicion.AttributeName = "atos_peajeid";
            condicion.Operator = ConditionOperator.NotNull;
            filtro.Conditions.Add(condicion);

            //Instalacion gas activas
            condicion = new ConditionExpression();
            condicion.AttributeName = "statecode";
            condicion.Operator = ConditionOperator.Equal;
            condicion.Values.Add(0);
            filtro.Conditions.Add(condicion);

            //columnas que no existen atos_sistemaelectricoid, atos_subsistemaid, atos_tarifaid, atos_lote
            QueryExpression consulta = new QueryExpression("atos_instalaciongas");
            consulta.ColumnSet.AddColumns("atos_name", "atos_instalaciongasid", "atos_peajeid", "atos_razonsocialid");
            consulta.Criteria.AddFilter(filtro);
            /* 23866 +1 NoLock */
            consulta.NoLock = true;

            //Si estamos haciendo una oferta a traves de cuenta negociadora
            //recuperaremos todas las razones sociales que esten relacionadas
            //con las instalaciones obtenidas de la cuenta negociadora
            if (padremultipunto.LogicalName == "atos_cuentanegociadora")
            {
                LinkEntity _link = new LinkEntity();
                _link.JoinOperator = JoinOperator.Inner;
                _link.LinkFromAttributeName = "atos_razonsocialid";
                _link.LinkFromEntityName = consulta.EntityName;
                _link.LinkToAttributeName = "accountid";
                _link.LinkToEntityName = "account";
                _link.LinkCriteria.AddCondition(new ConditionExpression("atos_cuentanegociadoraid", ConditionOperator.Equal, _cuentanegociadoraId.ToString()));

                consulta.LinkEntities.Add(_link);
            }

            //Ordenamos la Query
            OrderExpression orden;
            /*
            if (padremultipunto.LogicalName == "atos_cuentanegociadora")
            {
                orden = new OrderExpression();
                orden.AttributeName = "atos_razonsocialid";
                orden.OrderType = OrderType.Ascending;
                consulta.Orders.Add(orden);
            }*/

            //atos_tarifaid no existe como parametro
            orden = new OrderExpression();
            orden.AttributeName = "atos_peajeid";
            orden.OrderType = OrderType.Ascending;
            consulta.Orders.Add(orden);

            EntityCollection resultado = localcontext.OrganizationService.RetrieveMultiple(consulta);
            
            Guid peajeId = Guid.Empty;
            String peajeNombre = String.Empty;
            Guid subOfertaId = Guid.Empty;

            localcontext.Trace("Recuperadas " + resultado.Entities.Count.ToString() + " instalaciones");
            
            foreach (Entity i in resultado.Entities)
            {
                if (((EntityReference)i.Attributes["atos_peajeid"]).Id != peajeId)
                {
                    peajeId = ((EntityReference)i.Attributes["atos_peajeid"]).Id;
                    peajeNombre = ((EntityReference)i.Attributes["atos_peajeid"]).Name;
                    _razonsocialId = ((EntityReference)i.Attributes["atos_razonsocialid"]).Id;

                   //subOfertaId = creaOfertaGas(localcontext, 300000001, _cuentanegociadoraId, padremultipunto.LogicalName == "atos_cuentanegociadora" ? Guid.Empty : _razonsocialId,
                   subOfertaId = creaOfertaGas(localcontext, SUBOFERTA, _cuentanegociadoraId, padremultipunto.LogicalName == "atos_cuentanegociadora" ? Guid.Empty : _razonsocialId,
                        ofertaPadreId, Guid.Empty, peajeId, ef, peajeNombre, String.Empty);

                    localcontext.Trace("Creada suboferta");
                }

                _razonsocialId = ((EntityReference)i.Attributes["atos_razonsocialid"]).Id;
                //creaOfertaGas(localcontext, 300000002, _cuentanegociadoraId, _razonsocialId, subOfertaId, i.Id,
                creaOfertaGas(localcontext, OFERTA, _cuentanegociadoraId, _razonsocialId, subOfertaId, i.Id,
                    peajeId, ef, peajeNombre, i.Attributes["atos_name"].ToString());

                localcontext.Trace("Creada oferta");
            }
            
        }
    }
}
