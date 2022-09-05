/**
// <summary>
// Contador de instalaciones, consumos e importes
// </summary>
// <remarks>
// Actualiza en razon social y en cuenta negociadora unos contadores y campos de importe a partir de sus "hijos".
// - La razón social la actualiza a partir de sus instalaciones
// - La cuenta negociadora la actualiza a partir de sus razones sociales.
// </remarks>
 */
namespace ContadorInstalaciones
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /**
	// <summary>
	// Clase ContadorInstalaciones. Deriva de IPlugin
    // </summary>
    //<remarks>
    // Actualiza contadores y campos de importe
    // </remarks>
     */
    public class ContadorInstalaciones : IPlugin
    {
        
        private ITracingService tracingService;
        private IPluginExecutionContext PluginExecutionContext; ///< Contexto de ejecución del plugin
        private IOrganizationServiceFactory factory;
        private IOrganizationService service;
        
        private bool _log = false; ///< Indica si se activa o no el log. Esta variable debe inicializarse según el parámetro recibido en el constructor.
        private String ficherolog = "C:\\Users\\log_ContadorInstalaciones.txt";  ///< Fichero de log. Esta variable debe inicializarse según el parámetro recibido en el constructor.
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
        public ContadorInstalaciones(String parametros)
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

		
        /**
		// <summary>
		// Actualiza la cuenta negociadora a partir de los campos correspondientes de sus razones sociales
        // </summary>
        // <param name="_razonsocial">Entidad Razón social.</param>
        // <remarks>
        // Para todas las razones sociales de la cuenta negociadora de la razón social recibida, acumula los siguientes campos:
        // -# Número de instalaciones
        // -# Consumo anual
        // -# Consumo agregado
        // -# Deuda actualizada
        // -# Facturación total	
        // -# Importes garantía cliente (pendiente y actual)
        // -# Importes aval acciona (pendiente y actual)	
        // </remarks>
         */
        private void actualizaCuentaNegociadora(Entity _razonsocial)
        {

            if (_razonsocial.Attributes.Contains("atos_cuentanegociadoraid"))
            {
                writelog("actualizaCuentaNegociadora 1");
                QueryExpression _consulta = new QueryExpression("account");
                _consulta.ColumnSet = new ColumnSet(new String[] { "atos_numeroinstalaciones", "atos_consumoanualagregado", "atos_consumohistoricoanual",
                                                                   "atos_numeroinstalacionesgas", "atos_reqqd", "atos_deudaactualizada", "atos_facturaciontotal" });
                                                                   //"atos_importependientegarantiacliente", "atos_importependienteaval", // "atos_importependienteavalacciona", 
                                                                   //"atos_importeactualgarantiacliente", "atos_importeactualaval" }); // "atos_importeactualavalacciona" }); // new ColumnSet(true);
                FilterExpression _filtro = new FilterExpression();
                _filtro.FilterOperator = LogicalOperator.And;
                ConditionExpression _condicion;

                _condicion = new ConditionExpression();
                _condicion.AttributeName = "atos_cuentanegociadoraid";
                _condicion.Operator = ConditionOperator.Equal;
                _condicion.Values.Add(((EntityReference)_razonsocial.Attributes["atos_cuentanegociadoraid"]).Id.ToString());
                _filtro.Conditions.Add(_condicion);

                /*
                 *Pendiente de eleminar el campo atos_historico en razon social 
                _condicion = new ConditionExpression();
                _condicion.AttributeName = "atos_historico";
                _condicion.Operator = ConditionOperator.NotEqual;
                _condicion.Values.Add(true);
                _filtro.Conditions.Add(_condicion);*/
                _consulta.Criteria.AddFilter(_filtro);

                decimal _ninstalaciones = 0;
                decimal _consumoanual = 0;
                decimal _consumohistorico = 0;
                decimal _ninstalacionesgas = 0;
                decimal _caudalDiariohistorico = 0;
                decimal _deudaActualizada = 0;
                decimal _facturacionTotal = 0;


                //decimal _deudaactualizada = 0;
                //decimal _facturaciontotal = 0;
                //decimal _caudalDiariohistorico = 0;
                //decimal _importepdtegarantia = 0;
                //decimal _importeactualgarantia = 0;
                //decimal _importepdteaval = 0;
                //decimal _importeactualaval = 0;

                bool _hayInstalaciones = false;
                bool _hayConsumoAnual = false;
                bool _hayConsumoHistorico = false;
                bool _hayInstalacionesgas = false;
                bool _hayCaudalDiarioHistorico = false;
                bool _hayDeudaActualizada = false;
                bool _hayFacturacionTotal = false;

                // bool _hayDeudaActualizada = false;
                //bool _hayFacturacionTotal = false;
                //bool _hayImportePdteGarantia = false;
                //bool _hayImportePdteAval = false;
                //bool _hayImporteActualGarantia = false;
                //bool _hayImporteActualAval = false;

                writelog("actualizaCuentaNegociadora 2");
                EntityCollection _resConsulta = service.RetrieveMultiple(_consulta);
                foreach (Entity _r in _resConsulta.Entities)
                {
                    writelog("actualizaCuentaNegociadora 3");
                    if (_r.Attributes.Contains("atos_numeroinstalaciones"))
                    {
                        _ninstalaciones += (decimal)_r.Attributes["atos_numeroinstalaciones"];
                        _hayInstalaciones = true;
                    }
                    if (_r.Attributes.Contains("atos_consumoanualagregado"))
                    {
                        _consumoanual += (decimal)_r.Attributes["atos_consumoanualagregado"];
                        _hayConsumoAnual = true;
                    }
                    if (_r.Attributes.Contains("atos_consumohistoricoanual"))
                    {
                        _consumohistorico += (decimal)_r.Attributes["atos_consumohistoricoanual"];
                        _hayConsumoHistorico = true;
                    }
                    if (_r.Attributes.Contains("atos_numeroinstalacionesgas"))
                    {
                        _ninstalacionesgas += (decimal)_r.Attributes["atos_numeroinstalacionesgas"];
                        _hayInstalacionesgas = true; 
                    }
                    if (_r.Attributes.Contains("atos_reqqd"))
                    {
                        _caudalDiariohistorico += (decimal)_r.Attributes["atos_reqqd"];
                        _hayCaudalDiarioHistorico = true;
                    }
                    if (_r.Attributes.Contains("atos_deudaactualizada"))
                    {
                        _deudaActualizada += ((Money)_r.Attributes["atos_deudaactualizada"]).Value;
                        _hayDeudaActualizada = true;
                    }
                    if (_r.Attributes.Contains("atos_facturaciontotal"))
                    {
                        _facturacionTotal += ((Money)_r.Attributes["atos_facturaciontotal"]).Value;
                        _hayFacturacionTotal = true;
                    }
                    //writelog("actualizaCuentaNegociadora 3.1");
                    //if (_r.Attributes.Contains("atos_deudaactualizada"))
                    //{
                    //    _deudaactualizada += (decimal)((Money)_r.Attributes["atos_deudaactualizada"]).Value;
                    //    _hayDeudaActualizada = true;
                    //}
                    //writelog("actualizaCuentaNegociadora 3.2");
                    //if (_r.Attributes.Contains("atos_facturaciontotal"))
                    //{
                    //    _facturaciontotal += (decimal)((Money)_r.Attributes["atos_facturaciontotal"]).Value;
                    //    _hayFacturacionTotal = true;
                    //}
                    /*writelog("actualizaCuentaNegociadora 3.3");
                    if (_r.Attributes.Contains("atos_importependientegarantiacliente"))
                    {
                        _importepdtegarantia += (decimal)((Money)_r.Attributes["atos_importependientegarantiacliente"]).Value;
                        _hayImportePdteGarantia = true;
                    }
                    writelog("actualizaCuentaNegociadora 3.4");
                    //if (_r.Attributes.Contains("atos_importependienteavalacciona"))
                    if (_r.Attributes.Contains("atos_importependienteaval"))
                    {
                        //_importepdteaval += (decimal)((Money)_r.Attributes["atos_importependienteavalacciona"]).Value;
                        _importepdteaval += (decimal)((Money)_r.Attributes["atos_importependienteaval"]).Value;
                        _hayImportePdteAval = true;
                    }
                    writelog("actualizaCuentaNegociadora 3.5");
                    if (_r.Attributes.Contains("atos_importeactualgarantiacliente"))
                    {
                        _importeactualgarantia += (decimal)((Money)_r.Attributes["atos_importeactualgarantiacliente"]).Value;
                        _hayImporteActualGarantia = true;
                    }
                    writelog("actualizaCuentaNegociadora 3.6");
                    //if (_r.Attributes.Contains("atos_importeactualavalacciona"))
                    if (_r.Attributes.Contains("atos_importeactualaval"))
                    {
                        //_importeactualaval += (decimal)((Money)_r.Attributes["atos_importeactualavalacciona"]).Value;
                        _importeactualaval += (decimal)((Money)_r.Attributes["atos_importeactualaval"]).Value;
                        _hayImporteActualAval = true;
                    }*/
                    writelog("actualizaCuentaNegociadora 4 " + _ninstalaciones.ToString() + " " + _consumoanual.ToString() 
                        + " " + _consumohistorico.ToString() + " " + _ninstalacionesgas.ToString() + " " + _caudalDiariohistorico.ToString());
                }
                bool cnteniainstalaciones = false;
                //writelog("actualizaCuentaNegociadora 5 " + _importepdtegarantia.ToString() + " " + _importepdteaval.ToString());
                Entity _ctanegociadora = service.Retrieve("atos_cuentanegociadora", ((EntityReference)_razonsocial.Attributes["atos_cuentanegociadoraid"]).Id, 
                       new ColumnSet((new String[] {"atos_numeroinstalaciones", "atos_consumoanualagregado", "atos_consumohistoricoanual",
                                                    "atos_numeroinstalacionesgas", "atos_reqqd", "atos_deudaactualizada", "atos_facturaciontotal" })));

                if (_ctanegociadora.Attributes.Contains("atos_numeroinstalaciones"))
                    cnteniainstalaciones = true;
                if (_hayInstalaciones)
                    _ctanegociadora.Attributes["atos_numeroinstalaciones"] = _ninstalaciones;
                else
                {
                    if (cnteniainstalaciones)
                        _ctanegociadora.Attributes["atos_numeroinstalaciones"] = null;
                    else
                        _ctanegociadora.Attributes.Remove("atos_numeroinstalaciones");
                }

                if (_hayConsumoAnual)
                {
                    writelog("Actualiza consumo anual agregado " + _consumoanual.ToString());
                    _ctanegociadora.Attributes["atos_consumoanualagregado"] = _consumoanual;
                }
                else
                {
                    if (cnteniainstalaciones)
                        _ctanegociadora.Attributes["atos_consumoanualagregado"] = null;
                    else
                        _ctanegociadora.Attributes.Remove("atos_consumoanualagregado");
                }

                if (_hayConsumoHistorico)
                {
                    writelog("Actualiza consumo histórico anual " + _consumohistorico.ToString());
                    _ctanegociadora.Attributes["atos_consumohistoricoanual"] = _consumohistorico;
                }
                else
                {
                    if (cnteniainstalaciones)
                        _ctanegociadora.Attributes["atos_consumohistoricoanual"] = null;
                    else
                        _ctanegociadora.Attributes.Remove("atos_consumohistoricoanual");
                }

                bool cnteniainstalacionesgas = false;
                if (_ctanegociadora.Attributes.Contains("atos_numeroinstalacionesgas"))
                    cnteniainstalacionesgas = true;
                if (_hayInstalacionesgas)
                {
                    _ctanegociadora.Attributes["atos_numeroinstalacionesgas"] = _ninstalacionesgas;
                }
                else
                {
                    if (cnteniainstalacionesgas)
                    {
                        _ctanegociadora.Attributes["atos_numeroinstalacionesgas"] = null;
                    }
                    else
                    {
                        _ctanegociadora.Attributes.Remove("atos_numeroinstalacionesgas");
                    }
                }

                if (_hayCaudalDiarioHistorico)
                {
                    _ctanegociadora.Attributes["atos_reqqd"] = _caudalDiariohistorico;
                }
                else
                {
                    if (cnteniainstalacionesgas)
                    {
                        _ctanegociadora.Attributes["atos_reqqd"] = null;
                    }
                    else
                    {
                        _ctanegociadora.Attributes.Remove("atos_reqqd");
                    }
                }

                if (_hayDeudaActualizada)
                {
                    _ctanegociadora.Attributes["atos_deudaactualizada"] = new Money(_deudaActualizada);
                }
                else
                {
                    _ctanegociadora.Attributes["atos_deudaactualizada"] = null;
                }

                if (_hayFacturacionTotal)
                {
                    _ctanegociadora.Attributes["atos_facturaciontotal"] = new Money(_facturacionTotal);
                }
                else
                {
                    _ctanegociadora.Attributes["atos_facturaciontotal"] = null;
                }

                //if (_hayDeudaActualizada)
                //    _ctanegociadora.Attributes["atos_deudaactualizada"] = new Money(_deudaactualizada);
                //else
                //{
                //    if (cnteniainstalaciones)
                //        _ctanegociadora.Attributes["atos_deudaactualizada"] = null;
                //    else
                //        _ctanegociadora.Attributes["atos_deudaactualizada"] = null;
                //}
                //if (_hayFacturacionTotal)
                //    _ctanegociadora.Attributes["atos_facturaciontotal"] = new Money(_facturaciontotal);
                //else
                //{
                //    if (cnteniainstalaciones)
                //        _ctanegociadora.Attributes["atos_facturaciontotal"] = null;
                //    else
                //        _ctanegociadora.Attributes["atos_facturaciontotal"] = null;
                //}

                /*
                if (_hayImportePdteGarantia==true)
                    _ctanegociadora.Attributes["atos_importependientegarantiacliente"] = new Money(_importepdtegarantia);
                else
                    _ctanegociadora.Attributes["atos_importependientegarantiacliente"] = null;
                if (_hayImportePdteAval)
                    _ctanegociadora.Attributes["atos_importependienteaval"] = new Money(_importepdteaval); //_ctanegociadora.Attributes["atos_importependienteavalacciona"] = new Money(_importepdteaval);
                else
                    _ctanegociadora.Attributes["atos_importependienteaval"] = null;  //_ctanegociadora.Attributes["atos_importependienteavalacciona"] = null;
                if (_hayImporteActualGarantia)
                    _ctanegociadora.Attributes["atos_importeactualgarantiacliente"] = new Money(_importeactualgarantia);
                else
                    _ctanegociadora.Attributes["atos_importeactualgarantiacliente"] = null;
                if (_hayImporteActualAval)
                    _ctanegociadora.Attributes["atos_importeactualaval"] = new Money(_importeactualaval); //_ctanegociadora.Attributes["atos_importeactualavalacciona"] = new Money(_importeactualaval);
                else
                    _ctanegociadora.Attributes["atos_importeactualaval"] = null;  //_ctanegociadora.Attributes["atos_importeactualavalacciona"] = null;
                */
                writelog("actualizaCuentaNegociadora 6");
                service.Update(_ctanegociadora);
                writelog("actualizaCuentaNegociadora 7");
            }
		
		
        }

		
        /**
        // <summary>
        // Actualiza la razón social a partir de los campos correspondientes de sus instalaciones
        // </summary>
        // <param name="_instalacion">Entidad Instalación.</param>
        // <remarks>
        // Para todas las instalaciones de la razón social de la instalación recibida, acumula los siguientes campos:
        // -# Número de instalaciones
        // -# Consumo anual agregado
        // -# Consumo histórico anual
        // </remarks>
         */
        private void actualizaRazonSocial(Entity _instalacion, bool _eliminainstalacion = false)
        {
            if (_instalacion.Attributes.Contains("atos_razonsocialid"))
            {
                Entity razonsocial = service.Retrieve("account", ((EntityReference)_instalacion.Attributes["atos_razonsocialid"]).Id, new ColumnSet(new String[] { "atos_numeroinstalaciones", "atos_cuentanegociadoraid" }));
                writelog("actualizaRazonSocial 1");
                QueryExpression _consulta = new QueryExpression((_instalacion.LogicalName == "atos_instalacion" ?  "atos_instalacion" : "atos_instalaciongas"));

                _consulta.ColumnSet = _instalacion.LogicalName == "atos_instalacion" ? 
                    new ColumnSet(new String[] { "atos_consumoanual", "atos_consumoestimadototalanual", "atos_deudaactualizada", "atos_facturaciontotal" }) :
                    new ColumnSet(new String[] { "atos_reqqd", "atos_deudaactualizada", "atos_facturaciontotal" });
                    
                    // "atos_consumohistoricoanual", // Se cambia el campo de acumulación del consumo 20150325
                    //      "atos_deudaactualizada", "atos_facturaciontotal" }); // new ColumnSet(true); // No se propaga deuda y facturación 20150325
                FilterExpression _filtro = new FilterExpression();
                _filtro.FilterOperator = LogicalOperator.And;
                ConditionExpression _condicion;

                _condicion = new ConditionExpression();
                _condicion.AttributeName = "atos_razonsocialid";
                _condicion.Operator = ConditionOperator.Equal;
                _condicion.Values.Add(((EntityReference)_instalacion.Attributes["atos_razonsocialid"]).Id.ToString());
                _filtro.Conditions.Add(_condicion);

                /*
                 * Pendiente de quitar atos_historico en razon social
                _condicion = new ConditionExpression();
                _condicion.AttributeName = "atos_historico";
                _condicion.Operator = ConditionOperator.NotEqual;
                _condicion.Values.Add(true);
                _filtro.Conditions.Add(_condicion);*/
                _consulta.Criteria.AddFilter(_filtro);

                writelog("actualizaRazonSocial 2");
                if (PluginExecutionContext.MessageName == "Delete" || _eliminainstalacion)
                {
                    _condicion = new ConditionExpression();
                    _condicion.AttributeName = _instalacion.LogicalName == "atos_instalacion" ? "atos_instalacionid" : "atos_instalaciongasid";
                    _condicion.Operator = ConditionOperator.NotEqual;
                    _condicion.Values.Add(_instalacion.Id.ToString());
                    _filtro.Conditions.Add(_condicion);
                }
                writelog("actualizaRazonSocial 3");
                _consulta.Criteria.AddFilter(_filtro);
                EntityCollection _resConsulta = service.RetrieveMultiple(_consulta);

                decimal _ninstalaciones = 0;
                decimal _consumoanual = 0;
                decimal _consumohistorico = 0;
                decimal _caudalDiariohistorico = 0;
                decimal _deudaActualizada = 0;
                decimal _facturacionTotal = 0;

                bool _hayInstalaciones = false;
                bool _hayConsumoAnual = false;
                bool _hayConsumoHistorico = false;
                bool _hayCaudalDiarioHistorico = false;
                bool _hayDeudaActualizada = false;
                bool _hayFacturacionTotal = false;
                /*
                // No se sube ni la deuda ni la facturación a la razón social. 20150325
                decimal _deudaactualizada = 0;
                decimal _facturaciontotal = 0;
                bool _hayDeudaActualizada = false;
                bool _hayFacturacionTotal = false;
                **/

                foreach (Entity _r in _resConsulta.Entities)
                {
                    _ninstalaciones ++;
                    _hayInstalaciones = true;
                    writelog("actualizaRazonSocial 4 " + _ninstalaciones.ToString());
                    if (_r.Attributes.Contains("atos_consumoanual"))
                    {
                        _consumoanual += (decimal)_r.Attributes["atos_consumoanual"];
                        _hayConsumoAnual = true;
                    }
                    if (_r.Attributes.Contains("atos_consumoestimadototalanual"))
                    {
                        _consumohistorico += (decimal)_r.Attributes["atos_consumoestimadototalanual"];
                        _hayConsumoHistorico = true;
                    }
                    if (_r.Attributes.Contains("atos_reqqd"))
                    {
                        _caudalDiariohistorico += (decimal)_r.Attributes["atos_reqqd"];
                        _hayCaudalDiarioHistorico = true;
                    }
                    if (_r.Attributes.Contains("atos_deudaactualizada"))
                    {
                        _deudaActualizada += ((Money)_r.Attributes["atos_deudaactualizada"]).Value;
                        _hayDeudaActualizada = true;
                    }
                    if (_r.Attributes.Contains("atos_facturaciontotal"))
                    {
                        _facturacionTotal += ((Money)_r.Attributes["atos_facturaciontotal"]).Value;
                        _hayFacturacionTotal = true;
                    }
                    /*
                    // No se sube ni la deuda ni la facturación a la razón social. 20150325
                    if (_r.Attributes.Contains("atos_deudaactualizada"))
                    {
                        _deudaactualizada += (decimal)((Money)_r.Attributes["atos_deudaactualizada"]).Value;
                        _hayDeudaActualizada = true;
                    }
                    if (_r.Attributes.Contains("atos_facturaciontotal"))
                    {
                        _facturaciontotal += (decimal)((Money)_r.Attributes["atos_facturaciontotal"]).Value;
                        _hayFacturacionTotal = true;
                    }
                     */

                    writelog("actualizaRazonSocial 5 " + _consumoanual.ToString() + " " + _consumohistorico.ToString() + " " + _caudalDiariohistorico.ToString());
                }

                Entity _razonsocial = service.Retrieve("account", ((EntityReference)_instalacion.Attributes["atos_razonsocialid"]).Id,
                    new ColumnSet(new String[] { "atos_numeroinstalaciones", "atos_consumoanualagregado", "atos_consumohistoricoanual",
                    "atos_numeroinstalacionesgas", "atos_reqqd", "atos_deudaactualizada", "atos_facturaciontotal"}));



                bool _rzteniainstalaciones = false;
                bool _rzteniainstalacionesgas = false;
                if (_razonsocial.Attributes.Contains("atos_numeroinstalaciones"))
                    _rzteniainstalaciones = true;
                if (_razonsocial.Attributes.Contains("atos_numeroinstalacionesgas"))
                    _rzteniainstalacionesgas = true;

                if (_hayInstalaciones)
                    _razonsocial.Attributes[(_instalacion.LogicalName == "atos_instalacion" ? "atos_numeroinstalaciones" : "atos_numeroinstalacionesgas")] = _ninstalaciones;
                else
                {
                    if (_rzteniainstalaciones)
                        _razonsocial.Attributes[(_instalacion.LogicalName == "atos_instalacion" ? "atos_numeroinstalaciones" : "atos_numeroinstalacionesgas")] = null;
                    else
                        _razonsocial.Attributes.Remove((_instalacion.LogicalName == "atos_instalacion" ? "atos_numeroinstalaciones" : "atos_numeroinstalacionesgas"));
                }                
                
                if (_instalacion.LogicalName == "atos_instalacion")
                {
                    if (_hayConsumoAnual)
                        _razonsocial.Attributes["atos_consumoanualagregado"] = _consumoanual;
                    else
                    {
                        if (_rzteniainstalaciones)
                            _razonsocial.Attributes["atos_consumoanualagregado"] = null;
                        else
                            _razonsocial.Attributes.Remove("atos_consumoanualagregado");
                    }

                    if (_hayConsumoHistorico)
                        _razonsocial.Attributes["atos_consumohistoricoanual"] = _consumohistorico;
                    else
                    {
                        if (_rzteniainstalaciones)
                            _razonsocial.Attributes["atos_consumohistoricoanual"] = null;
                        else
                            _razonsocial.Attributes.Remove("atos_consumohistoricoanual");
                    }
                }
                else
                {
                    if (_hayCaudalDiarioHistorico)
                    {
                        _razonsocial.Attributes["atos_reqqd"] = _caudalDiariohistorico;
                    }
                    else
                    {
                        if (_rzteniainstalacionesgas)
                            _razonsocial.Attributes["atos_reqqd"] = null;
                        else
                            _razonsocial.Attributes.Remove("atos_reqqd");
                    }
                }

                if (_hayDeudaActualizada || _hayFacturacionTotal || PluginExecutionContext.MessageName == "Delete" || _eliminainstalacion) { 
                    QueryExpression _consultaSum = new QueryExpression((_instalacion.LogicalName == "atos_instalacion" ? "atos_instalaciongas" : "atos_instalacion"));
                    _consultaSum.ColumnSet = new ColumnSet(new String[] { "atos_deudaactualizada", "atos_facturaciontotal" });

                    FilterExpression _filtroSum = new FilterExpression();
                    _filtroSum.FilterOperator = LogicalOperator.And;

                    ConditionExpression _condicionSum = new ConditionExpression();
                    _condicionSum.AttributeName = "atos_razonsocialid";
                    _condicionSum.Operator = ConditionOperator.Equal;
                    _condicionSum.Values.Add(((EntityReference)_instalacion.Attributes["atos_razonsocialid"]).Id.ToString());
                    _filtroSum.Conditions.Add(_condicionSum);

                    _consultaSum.Criteria.AddFilter(_filtroSum);

                    EntityCollection _resConsultaSum = service.RetrieveMultiple(_consultaSum);

                    foreach (Entity _r in _resConsultaSum.Entities)
                    {
                        if (_r.Attributes.Contains("atos_deudaactualizada"))
                        {
                            _deudaActualizada += ((Money)_r.Attributes["atos_deudaactualizada"]).Value;
                        }
                        if (_r.Attributes.Contains("atos_facturaciontotal"))
                        {
                            _facturacionTotal += ((Money)_r.Attributes["atos_facturaciontotal"]).Value;
                        }
                    }

                    if (_deudaActualizada > 0)
                    {
                        _razonsocial.Attributes["atos_deudaactualizada"] = new Money(_deudaActualizada);
                    }
                    else
                    {
                        _razonsocial.Attributes["atos_deudaactualizada"] = null;
                    }
                    if (_facturacionTotal > 0)
                    {
                        _razonsocial.Attributes["atos_facturaciontotal"] = new Money(_facturacionTotal);
                    }
                    else
                    {
                        _razonsocial.Attributes["atos_facturaciontotal"] = null;
                    }
                }

                /*
                // No se sube ni la deuda ni la facturación a la razón social. 20150325
                if ( _hayDeudaActualizada )
                    _razonsocial.Attributes["atos_deudaactualizada"] = new Money(_deudaactualizada);
                else
                    _razonsocial.Attributes["atos_deudaactualizada"] = null;
                if ( _hayFacturacionTotal )
                    _razonsocial.Attributes["atos_facturaciontotal"] = new Money(_facturaciontotal);
                else
                    _razonsocial.Attributes["atos_facturaciontotal"] = null;
                 */



                writelog("actualizaRazonSocial 6");
                service.Update(_razonsocial);
                actualizaCuentaNegociadora(razonsocial);
                writelog("actualizaRazonSocial 7");
            }
        }

        /**
		// <summary>
		// Punto de entrada al plugin
		// </summary>
        // <param name="serviceProvider">IServiceProvider.</param>
		// <remarks>
		// El plugin se ejecuta en:
        // - Creación de instalación
        // - Creación de razón social
        // - Modificación de instalación de alguno de los campos que se propagan
        // - Modificación de razón social de alguno de los campos que se propagan
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
            writelog("Plugin Contador Instalaciones");
            writelog("Mensaje: " + PluginExecutionContext.MessageName);
            if (PluginExecutionContext.MessageName == "Create")
            {
                Entity ef = (Entity)PluginExecutionContext.InputParameters["Target"];
                writelog("Entidad: " + ef.LogicalName);
                if (ef.LogicalName == "atos_instalacion" || ef.LogicalName == "atos_instalaciongas")
                {
                    /*
                     * Pendiente de elminar el campo en instalacion power
                    if ((Boolean)ef.Attributes["atos_historico"] == true)
                        return;
                    */
                    if (ef.Attributes.Contains("atos_razonsocialid"))
                        actualizaRazonSocial(ef);
                }
                else if (ef.LogicalName == "account")
                {
                    /*
                     * Pendiente de eliminar el campo en Razon Social
                    if ((Boolean)ef.Attributes["atos_historico"] == true)
                        return;
                     */
                    if (ef.Attributes.Contains("atos_cuentanegociadoraid") ||
                         ef.Attributes.Contains("atos_consumoanual") || ef.Attributes.Contains("atos_consumoanualagregado")
                         || ef.Attributes.Contains("atos_deudaactualizada") || ef.Attributes.Contains("atos_facturaciontotal"))
                        actualizaCuentaNegociadora(ef);
                    //modificaContadores(ef, true);
                    //modificacontador(ef, 1);
                }
            }
            else if (PluginExecutionContext.MessageName == "Update")
            {
                Entity ef = (Entity)PluginExecutionContext.InputParameters["Target"];
                writelog("Entidad: " + ef.LogicalName);
                if (ef.LogicalName == "atos_instalacion" || ef.LogicalName == "atos_instalaciongas")
                {
                    Entity preUpdateImage = (Entity)PluginExecutionContext.PreEntityImages["PreUpdateImage"];
                    /*
                     *Pendiente de quitar campo historico instalacion power
                    writelog("Antes de comprobar historico");

                    if (preUpdateImage.Attributes.Contains("atos_historico") == true && (Boolean)preUpdateImage.Attributes["atos_historico"] == true)
                        return;*/
                    /*writelog("Retrieve atos_instalacion");
                    Entity preUpdate = service.Retrieve("atos_instalacion", ef.Id, new ColumnSet(new String[] { "atos_historico" }));

                    if (preUpdate.Attributes.Contains("atos_historico") == true && (Boolean)preUpdate.Attributes["atos_historico"] == true)
                        return;*/
                    writelog("Despues de comprobar historico. Razón social anterior: " + ((EntityReference)preUpdateImage.Attributes["atos_razonsocialid"]).Name + " Id." + ((EntityReference)preUpdateImage.Attributes["atos_razonsocialid"]).Id);
                    if (ef.Attributes.Contains("atos_consumoanual") || ef.Attributes.Contains("atos_consumoestimadototalanual")
                        || ef.Attributes.Contains("atos_razonsocialid") || ef.Attributes.Contains("atos_reqqd")
                        || ef.Attributes.Contains("atos_deudaactualizada") || ef.Attributes.Contains("atos_facturaciontotal")) // Cuando cambia de razón social también debe actualizar
                     //   ef.Attributes.Contains("atos_deudaactualizada") || ef.Attributes.Contains("atos_facturaciontotal")) // Desde instalación no se actualiza en RS ni la deuda ni la facturación
                    {
                        writelog("Actualiza la razón social");
                        
                        if (ef.Attributes.Contains("atos_razonsocialid")) // Si se modifica razon social hay que actualizar las dos
                        {
                            actualizaRazonSocial(ef);
                            writelog("Se ha actualizado la razón social de la instalación a " + ((EntityReference)ef.Attributes["atos_razonsocialid"]).Name + " Id." + ((EntityReference)ef.Attributes["atos_razonsocialid"]).Id);
                        
                            if (preUpdateImage.Attributes.Contains("atos_razonsocialid"))
                                if (((EntityReference)preUpdateImage.Attributes["atos_razonsocialid"]).Id != ((EntityReference)ef.Attributes["atos_razonsocialid"]).Id)
                                    actualizaRazonSocial(preUpdateImage, true);
                        }
                        else
                            actualizaRazonSocial(preUpdateImage);

                        /*if (preUpdateImage.Attributes.Contains("atos_razonsocialid"))
                            actualizaRazonSocial(preUpdateImage, true);*/
                        /*
                        Entity _instalacion = service.Retrieve("atos_instalacion", ef.Id, new ColumnSet(true));
                        if (_instalacion.Attributes.Contains("atos_razonsocialid"))
                            actualizaRazonSocial(_instalacion);
                        if (ef.Attributes.Contains("atos_razonsocialid")) // Cuando cambia de razón social también debe actualizar
                            actualizaRazonSocial(ef);*/
                    }
                }
                else if (ef.LogicalName == "account")
                {
                    /*
                     * Pendiente de quitar historico en razon social
                    writelog("Antes de comprobar historico");
                    Entity preUpdate = service.Retrieve("account", ef.Id, new ColumnSet(new String [] { "atos_historico"} ));
                    
                    if (preUpdate.Attributes.Contains("atos_historico") == true && (Boolean)preUpdate.Attributes["atos_historico"] == true)
                        return;
                    */
                    //writelog("Despues de comprobar historico");
                    //if (ef.Attributes.Contains("atos_deudaactualizada") ||
                    //    ef.Attributes.Contains("atos_facturaciontotal")) // || 
                    //    //ef.Attributes.Contains("atos_importependientegarantiacliente") ||
                    //    //ef.Attributes.Contains("atos_importependienteaval") ||
                    //    //ef.Attributes.Contains("atos_importependienteavalacciona") ||
                    //    //ef.Attributes.Contains("atos_importeactualgarantiacliente") ||
                    //    //ef.Attributes.Contains("atos_importeactualaval"))
                    //    //ef.Attributes.Contains("atos_importeactualavalacciona"))
                    //{
                    //    Entity _razonSocial = service.Retrieve("account", ef.Id, new ColumnSet(true));
                    //    actualizaCuentaNegociadora(_razonSocial);
                    //    //throw new System.Exception();
                    //}
                }
                /*
                Entity PreUpdate = (Entity)PluginExecutionContext.PreEntityImages["PreUpdateImage"];
                Entity PostUpdate = (Entity)PluginExecutionContext.PostEntityImages["PostUpdateImage"];
                if (PostUpdate.LogicalName == "atos_instalacion")
                {
                    if (PostUpdate.Attributes.Contains("atos_razonsocialid"))
                    {
                        //modificacontador(PreUpdate, -1);
                        //modificacontador(PostUpdate, 1);
                        modificaContadores(PreUpdate, false);
                        modificaContadores(PostUpdate, true);
                    }
                }
                else if (PostUpdate.LogicalName == "account")
                {
                    if (PostUpdate.Attributes.Contains("atos_cuentanegociadoraid"))
                    {
                        Entity razonsocial = service.Retrieve("account", PostUpdate.Id, new ColumnSet(true));
                        if (razonsocial.Attributes.Contains("atos_numeroinstalaciones"))
                        {
                            modificaContadoresCN(PreUpdate, false);
                            modificaContadoresCN(PostUpdate, true);
                            //modificaContadorCta(PreUpdate, -1 * (decimal)razonsocial.Attributes["atos_numeroinstalaciones"]);
                            modificaContadorCta(PostUpdate, (decimal)razonsocial.Attributes["atos_numeroinstalaciones"]);
                        }
                    }
                }*/
            }
            else if (PluginExecutionContext.MessageName == "Delete")
            {
                Entity PreDelete = (Entity)PluginExecutionContext.PreEntityImages["PreDeleteImage"];

                if (PreDelete.LogicalName == "atos_instalacion" || PreDelete.LogicalName == "atos_instalaciongas")
                {
                    /*
                     *Pendiente de quitar atos_historico en instalacion power 
                    if ((Boolean)PreDelete.Attributes["atos_historico"] == true)
                        return;
                     */
                    if (PreDelete.Attributes.Contains("atos_razonsocialid"))
                    {
                        actualizaRazonSocial(PreDelete);
                        // modificaContadores(PreDelete, false);
                        // // modificacontador(PreDelete, -1);
                    }
                }
                else if (PreDelete.LogicalName == "account")
                {
                    /*
                     *Pendiente de quitar campo  atos_historico en razon social
                    if ((Boolean)PreDelete.Attributes["atos_historico"] == true)
                        return;
                    */
                    if (PreDelete.Attributes.Contains("atos_cuentanegociadoraid"))
                    {
                        actualizaCuentaNegociadora(PreDelete);
                        // modificaContadores(PreDelete, false);
                        // // modificacontador(PreDelete, -1);
                    }
                }
            }
        }
    }
}
