/*
 File="atos_oferta.js" 
 Copyright (c) Atos. All rights reserved.

 Funciones compartidas entre varios formularios

 Fecha 		Codigo  Version Descripcion                                     Autor
  13.11.2020                Se ejecuta en la llamada para cargar la
                            instalación segun instalacionid.                Lazaro Castro  
 16/03/2021  22323  1.0.1.1 Cambio el FechXml con sentencia no-lock='true'  ACR
 18.03.2021  22329          Correccion de lecturas cuando vienes de Instalaciones
                            Razones sociales                                ACR
                            creacion de un contrato desde oferta
 18.03.2021  22329          Documentacion                                   ACR   
 01.10.2021  23251          Envio de datos de Coste al contrato 
                            (Ribbon) Botón Contrato                         ACR
*/

//#region Variables Funciones Globales 22329
var globalContext;
var formContext;

function getGlobalContext() { return Xrm.Utility.getGlobalContext(); }
function getContext() { return executionContext.getFormContext(); } 
//function getOrg() { return globalContext.getOrgUniqueName(); }
//function getLcid() { return Xrm.Utility.getGlobalContext().userSettings.languageId.toString(); }
function getUserId() { return globalContext.getUserId(); }
function getUserRole() { return globalContext.getUserRoles(); }                          
//function getValidateRole() { return validator; }    
function getId() { return formContext.data.entity.getId(); }
function IsEmptyId() { if (getId() == null || getId() == "") return true; else return false; }  
function getClient() { return globalContext.client.getClient(); }
//function getStateCode() { return formContext.getAttribute("statecode").getValue(); }
function getFieldValue(field) { return formContext.getAttribute(field).getValue(); }
function getServerUrl() {  return Xrm.Page.context.getClientUrl(); } 
function getFormType() {  return Xrm.Page.ui.getFormType(); } // Form type  0-Undefined, 1-Create 2-Update 3-Read Only 4-Disabled  6-Bulk Edit
function getFormId() {  return formContext.ui.formSelector.getCurrentItem().getId(); } 
function getFormName() {  return formContext.ui.formSelector.getCurrentItem().getLabel(); } 


//#endregion Variables Funciones Globales 22329


//#region Evento onLoad 22339
/*
 * Function from OnLoad event
 * Aplicado al evento Onload de primero
 * 
 * @param {executionContext} Contexto de ejecucion del formulario
 */
function LoadForm(executionContext) {
    debugger;
    //Recupera contexto
    formContext = executionContext.getFormContext();
    // Referencia de la API de cliente
    globalContext = Xrm.Utility.getGlobalContext();
    //Idioma
    codeLang = globalContext.userSettings.languageId;
}
//#endregion Evento onLoad 22339

/*
 * Function from OfertaOnLoad event
 * Se ejecuta al abrir el formulario de la Oferta.
 * Si se entra en modo nueva oferta, comprueba si se ha llamado desde la instalación
 * o desde la razón social o desde la cuenta negociadora.
 * Si ha sido llamado desde alguna de ellas llama a la función correspondiente (cargaDesdeInstalacion, cargaDesdeInstalacionGas,
 * cargaDesdeRazonSocial, cargaDesdeCtaNegociadora) para cargar datos de la oferta a partir de la entidad llamante.
 * Modificado para UI -  19.11.2020 - LÃ¡zaro Castro
 * 
 * @param {*} executionContext 
 */
function OfertaOnLoad(executionContext) {
    debugger;
    // Recupera contexto
    formContext = executionContext.getFormContext();
    // Referencia de la API de cliente
    globalContext = Xrm.Utility.getGlobalContext();


/* 22329 */ //var globalContext = Xrm.Utility.getGlobalContext();
    var serverUrl = globalContext.getClientUrl();
/* 22329 */ //var formContext = executionContext.getFormContext();
/* 22329 */ //if (formContext.data.entity.getId() == null || formContext.data.entity.getId() == "") {
    if (IsEmptyId()) {
        
        var xrmObject = globalContext.getQueryStringParameters();
        if (xrmObject != null) {
            if (xrmObject["llamado_desde"] != null) {
                var llamado_desde = xrmObject["llamado_desde"].toString();
                //if ( llamado_desde == "Instalacion" )
                if (llamado_desde == "Instalacion Gas" || llamado_desde == "Instalacion"
                    || llamado_desde == "Razon Social Power" || llamado_desde == "Razon Social Gas"
                    || llamado_desde == "Cuenta Negociadora Power" || llamado_desde == "Cuenta Negociadora Gas") 
                {
                    if (xrmObject["llamante_id"] != null) {
                        //var instalacionid = xrmObject["llamante_id"].toString();
                        var llamanteid = xrmObject["llamante_id"].toString();

                        // var serverUrl = Xrm.Page.context.getClientUrl();

                        head.load(serverUrl + "/WebResources/atos_json2.js", serverUrl + "/WebResources/atos_jquery.js", function () {
                            //cargaDesdeInstalacion(instalacionid);
                            if (llamado_desde == "Instalacion") {
                                cargaDesdeInstalacion(formContext, llamanteid, 300000000);
                                //mostrarcampos();
                            } else if (llamado_desde == "Instalacion Gas") {
                                cargaDesdeInstalacionGas(formContext, llamanteid, 300000001);
                                //mostrarcampos();
                            } else if (llamado_desde == "Razon Social Power") {
                                cargaDesdeRazonSocial(formContext, llamanteid, 300000000);
                                //mostrarcampos();
                            } else if (llamado_desde == "Razon Social Gas") {
                                cargaDesdeRazonSocial(formContext, llamanteid, 300000001);
                                //mostrarcampos();
                            } else if (llamado_desde == "Cuenta Negociadora Power") {
                                cargaDesdeCtaNegociadora(formContext, llamanteid, 300000000);
                                habilitarCondiciones(formContext);
                                //mostrarcampos();
                            }
                            else {
                                cargaDesdeCtaNegociadora(formContext, llamanteid, 300000001);
                                habilitarCondiciones(formContext);
                                //mostrarcampos();
                            }
                            mostrarcampos(formContext);
                            setCustomLookupProductoBase(formContext);
                        });
                    }
                }
            }
        }
    } else {
        if (campoVacio(formContext, "atos_razonsocialid") == true && formContext.getAttribute("atos_tipooferta").getValue() == 300000000) { //300000000 --> Multipunto
            habilitarCondiciones(formContext);
        }
        setCustomLookupProductoBase(formContext);
    }
}

/**
// <summary>
// Carga datos desde la cuenta negociadora relacionada
// </summary>
// <param name="cuentanegociadoraid">Identificador de la cuenta negociadora.</param>
// <remarks>
// Accede a la cuenta negociadora con el identificador recibido por parÃ¡metro<br/>
// y rellena los campos de la oferta a partir de los siguientes campos de la cuenta negociadora:
// <ul>
// <li>Cuenta negociadora</li>
// <li>Agente comercial</li>
// </ul>
// </remarks>
*/
function cargaDesdeCtaNegociadora(formContext, cuentanegociadoraid, commodity) {
    // debugger;

    // Cargamos commodity del formulario segun el tipo de oferta marcada en la cuenta negociadora
    formContext.getAttribute("atos_commodity").setValue(commodity);
    formContext.getAttribute("atos_commodity").setSubmitMode("always");

    // Cargamos tipo de oferta Multipunto del formulario
    formContext.getAttribute("atos_tipooferta").setValue(300000000);
    formContext.getAttribute("atos_tipooferta").setSubmitMode("always");

    //Realizamos fetchXml para obtener parametros de la cuenta negociadora
    if (cuentanegociadoraid != null) {
        var fetchXml =
/* 22323 +1*/ //"<fetch mapping='logical'>"+
           "<fetch mapping='logical' no-lock='true'>" +
            "<entity name='atos_cuentanegociadora' > " +
            "<attribute name='atos_cuentanegociadoraid' alias='cuentanegociadoraid' /> " +
            "<attribute name='atos_name' alias='cuentanegociadora' /> " +
            "<attribute name='atos_agentecomercialid' alias='atos_agentecomercialid' /> " +
            "<link-entity name='atos_agentecomercial' from='atos_agentecomercialid' to='atos_agentecomercialid' link-type='outer' alias='ac' > " +
            "<attribute name='atos_name' alias='agentecomercial' /> " +
            "</link-entity> " +
            "<filter>" +
            "<condition attribute='atos_cuentanegociadoraid' operator='eq' value='" + cuentanegociadoraid + "' />" +
            "</filter>" +
            "</entity> " +
            "</fetch>";
        // var registros = XrmServiceToolkit.Soap.Fetch(fetchXml);
        Xrm.WebApi.retrieveMultipleRecords("atos_cuentanegociadora", "?fetchXml=" + fetchXml).then(
            function success(registros) {
                if (registros != null && registros.entities != null && registros.entities.length > 0) {
                    var cuentanegociadora = registros.entities[0];
                    if (cuentanegociadora.cuentanegociadora != null) {
                        var valorReferencia = new Array();
                        valorReferencia[0] = new Object();
                        valorReferencia[0].id = cuentanegociadora.cuentanegociadoraid;
                        valorReferencia[0].name = cuentanegociadora.cuentanegociadora;
                        valorReferencia[0].entityType = "atos_cuentanegociadora";
                        formContext.getAttribute("atos_cuentanegociadoraid").setValue(valorReferencia);
                        formContext.getAttribute("atos_cuentanegociadoraid").setSubmitMode("always");
                    }

                    if (cuentanegociadora._atos_agentecomercialid_value != null) {
                        var valorReferencia = new Array();
                        valorReferencia[0] = new Object();
                        valorReferencia[0].id = cuentanegociadora._atos_agentecomercialid_value;
                        valorReferencia[0].name = cuentanegociadora["_atos_agentecomercialid_value@OData.Community.Display.V1.FormattedValue"];
                        valorReferencia[0].entityType = "atos_agentecomercial";
                        formContext.getAttribute("atos_agentecomercialid").setValue(valorReferencia);
                        formContext.getAttribute("atos_agentecomercialid").setSubmitMode("always");
                    }

                }
            }, function (error) {
                Xrm.Navigation.openErrorDialog({ detail: error.message });
            });

    }
}

/**
// <summary>
// Carga datos desde la razÃ³n social relacionada
// </summary>
// <param name="razonsocialid">Identificador de la razÃ³n social.</param>
// <param name="commodity">Commodity 300000000: Power, 300000001: Gas.</param>
// <remarks>
// Accede a la razÃ³n social (account) con el identificador recibido por parÃ¡metro<br/>
// y rellena los campos de la oferta a partir de los siguientes campos de la razÃ³n social:
// <ul>
// <li>Razon Social</li>
// <li>Cuenta negociadora</li>
// <li>Agente comercial</li>
// <li>Numero de documento</li>
// Modificado para UI 19.11.2020. Lazaro Castro. 
//  El grupo empresarial no estÃ¡ en el formulario ni viene como resultado en e Fetch.
// </remarks>
*/
function cargaDesdeRazonSocial(formContext, razonsocialid, commodity) {

    // Cargamos commodity del formulario segun el tipo de oferta 
    formContext.getAttribute("atos_commodity").setValue(commodity);
    formContext.getAttribute("atos_commodity").setSubmitMode("always");

    // Cargamos tipo de oferta Multipunto del formulario
    formContext.getAttribute("atos_tipooferta").setValue(300000000);
    formContext.getAttribute("atos_tipooferta").setSubmitMode("always");

    // Realizamos fetchXml para obtener parametros de la razon social
    if (razonsocialid != null) {
        var fetchXml =
 /* 223231 +1*/ //"<fetch mapping='logical'>"+
           "<fetch mapping='logical' no-lock='true'>" +
            "<entity name='account' > " +
            "<attribute name='accountid' /> " +
            "<attribute name='name' alias='razonsocial' /> " +
            "<attribute name='atos_cuentanegociadoraid' alias='cuentanegociadoraid' /> " +
            "<attribute name='atos_numerodedocumento' alias='numerodedocumento' /> " +
            "<attribute name='atos_agentecomercialid' alias='atos_agentecomercialid' /> " +
            "<attribute name='atos_grupoempresarialid' alias='atos_grupoempresarialid' /> " +
            "<attribute name='atos_formadepago' alias='formadepago' />" +
            "<attribute name='atos_condicionpagoid' alias='condicionpagoid' />" +
            "<attribute name='atos_tipodeenvio' alias='tipodeenvio' />" +
            "<attribute name='atos_plazoenviofacturas' alias='plazoenviofacturas' />" +
            "<attribute name='atos_mandatosepa' alias='mandatosepa' />" +
            "<attribute name='atos_swift' alias='swift' />" +
            "<attribute name='atos_iban' alias='iban' />" +
            "<attribute name='atos_entidadbancaria' alias='entidadbancaria' />" +
            "<attribute name='atos_sucursalbancaria' alias='sucursalbancaria' />" +
            "<attribute name='atos_digitocontrol' alias='digitocontrol' />" +
            "<attribute name='atos_cuenta' alias='cuenta' />" +
            "<attribute name='atos_cuentabancaria' alias='cuentabancaria' />" +
            "<attribute name='atos_cuentabancariaapropia' alias='cuentabancariaapropia' />" +
            "<filter>" +
            "<condition attribute='accountid' operator='eq' value='" + razonsocialid + "' />" +
            "</filter>" +
            "</entity> " +
            "</fetch>";

        // var registros = XrmServiceToolkit.Soap.Fetch(fetchXml);
        Xrm.WebApi.retrieveMultipleRecords("account", "?fetchXml=" + fetchXml).then(
            function success(registros) {
                // debugger;
                if (registros != null && registros.entities != null && registros.entities.length > 0) {
                    var razonsocial = registros.entities[0];
                    if (razonsocial.formadepago != null) {
                        formContext.getAttribute("atos_formadepago").setValue(razonsocial.formadepago);
                        formContext.getAttribute("atos_formadepago").setSubmitMode("always");
                    }

                    if (razonsocial.condicionpagoid != null) {
                        var valorReferencia = new Array();
                        valorReferencia[0] = new Object();
                        valorReferencia[0].id = razonsocial.condicionpagoid;
                        valorReferencia[0].name = razonsocial["condicionpagoid@OData.Community.Display.V1.FormattedValue"];
                        valorReferencia[0].entityType = "atos_condiciondepago";
                        formContext.getAttribute("atos_condicionpagoid").setValue(valorReferencia);
                        formContext.getAttribute("atos_condicionpagoid").setSubmitMode("always");
                    }

                    if (razonsocial.tipodeenvio != null) {
                        formContext.getAttribute("atos_tipodeenvio").setValue(razonsocial.tipodeenvio);
                        formContext.getAttribute("atos_tipodeenvio").setSubmitMode("always");
                    }

                    if (razonsocial.plazoenviofacturas != null) {
                        formContext.getAttribute("atos_plazoenviofacturas").setValue(razonsocial.plazoenviofacturas);
                        formContext.getAttribute("atos_plazoenviofacturas").setSubmitMode("always");
                    }

                    if (razonsocial.mandatosepa != null) {
                        formContext.getAttribute("atos_mandatosepa").setValue(razonsocial.mandatosepa);
                        formContext.getAttribute("atos_mandatosepa").setSubmitMode("always");
                    }

                    if (razonsocial.swift != null) {
                        formContext.getAttribute("atos_swift").setValue(razonsocial.swift);
                        formContext.getAttribute("atos_swift").setSubmitMode("always");
                    }

                    if (razonsocial.iban != null) {
                        formContext.getAttribute("atos_iban").setValue(razonsocial.iban);
                        formContext.getAttribute("atos_iban").setSubmitMode("always");
                    }

                    if (razonsocial.entidadbancaria != null) {
                        formContext.getAttribute("atos_entidadbancaria").setValue(razonsocial.entidadbancaria);
                        formContext.getAttribute("atos_entidadbancaria").setSubmitMode("always");
                    }

                    if (razonsocial.sucursalbancaria != null) {
                        formContext.getAttribute("atos_sucursalbancaria").setValue(razonsocial.sucursalbancaria);
                        formContext.getAttribute("atos_sucursalbancaria").setSubmitMode("always");
                    }

                    if (razonsocial.digitocontrol != null) {
                        formContext.getAttribute("atos_digitocontrol").setValue(razonsocial.digitocontrol);
                        formContext.getAttribute("atos_digitocontrol").setSubmitMode("always");
                    }

                    if (razonsocial.cuenta != null) {
                        formContext.getAttribute("atos_cuenta").setValue(razonsocial.cuenta);
                        formContext.getAttribute("atos_cuenta").setSubmitMode("always");
                    }

                    if (razonsocial.cuentabancaria != null) {
                        formContext.getAttribute("atos_cuentabancaria").setValue(razonsocial.cuentabancaria);
                        formContext.getAttribute("atos_cuentabancaria").setSubmitMode("always");
                    }

                    if (razonsocial.cuentabancariaapropia != null) {
                        formContext.getAttribute("atos_cuentabancariaapropia").setValue(razonsocial.cuentabancariaapropia);
                        formContext.getAttribute("atos_cuentabancariaapropia").setSubmitMode("always");
                    }

                    if (razonsocial.razonsocial != null) {
                        // Crear funcion general
                        var valorReferencia = new Array();
                        valorReferencia[0] = new Object();
                        valorReferencia[0].id = razonsocialid;
                        valorReferencia[0].name = razonsocial.razonsocial;
                        valorReferencia[0].entityType = "account";
                        formContext.getAttribute("atos_razonsocialid").setValue(valorReferencia);
                        formContext.getAttribute("atos_razonsocialid").setSubmitMode("always");
                    }

                    if (razonsocial.cuentanegociadoraid != null) {
                        // Crear funcion general
                        var valorReferencia = new Array();
                        valorReferencia[0] = new Object();
                        valorReferencia[0].id = razonsocial.cuentanegociadoraid;
                        valorReferencia[0].name = razonsocial["cuentanegociadoraid@OData.Community.Display.V1.FormattedValue"];
                        valorReferencia[0].entityType = "atos_cuentanegociadora";
                        formContext.getAttribute("atos_cuentanegociadoraid").setValue(valorReferencia);
                        formContext.getAttribute("atos_cuentanegociadoraid").setSubmitMode("always");
                    }

                    if (razonsocial._atos_agentecomercialid_value != null) {
                        // Crear funcion general
                        var valorReferencia = new Array();
                        valorReferencia[0] = new Object();
                        valorReferencia[0].id = razonsocial._atos_agentecomercialid_value;
                        valorReferencia[0].name = razonsocial["_atos_agentecomercialid_value@OData.Community.Display.V1.FormattedValue"];
                        valorReferencia[0].entityType = "atos_agentecomercial";
                        formContext.getAttribute("atos_agentecomercialid").setValue(valorReferencia);
                        formContext.getAttribute("atos_agentecomercialid").setSubmitMode("always");
                    }

                    if (razonsocial.numerodedocumento != null) {
                        formContext.getAttribute("atos_numerodocumento").setValue(razonsocial.numerodedocumento);
                        formContext.getAttribute("atos_numerodocumento").setSubmitMode("always");
                    }

                    //if (razonsocal.atos_grupoempresarialid != null) {
                    //    var valorReferencia = new Array();
                    //    valorReferencia[0] = new Object();
                    //    valorReferencia[0].id = razonsocial.atos_grupoempresarialid;
                    //    valorReferencia[0].name = razonsocial["atos_grupoempresarialid@OData.Community.Display.V1.FormattedValue"]
                    //    valorReferencia[0].entityType = "atos_grupoempresarial";
                    //    formContext.getAttribute("atos_grupoempresarialid").setValue(valorReferencia);
                    //    formContext.getAttribute("atos_grupoempresarialid").setSubmitMode("always");
                    //}

                }
            }, function (error) {
                Xrm.Navigation.openErrorDialog({ detail: error.message });
            });

    }
}

/**
// <summary>
// Carga datos desde la instalaciÃ³n relacionada
// </summary>
// <param name="instalacionid">Identificador de la instalaciÃ³n.</param>
// <remarks>
// Accede a la instalaciÃ³n con el identificador recibido por parÃ¡metro<br/>
// y rellena los campos de la oferta a partir de los siguientes campos de la instalaciÃ³n:
// <ul>
// <li>InstalaciÃ³n</li>
// <li>RazÃ³n Social</li>
// <li>Cuenta negociadora</li>
// <li>Tarifa</li>
// <li>Sistema elÃ©ctrico</li>
// <li>Agente comercial</li>
// <li>Lote</li>
// <li>Consumo estimado total anual</li>
// <li>NÃºmero de documento</li>
// </ul>
// </remarks>
*/
function cargaDesdeInstalacion(formContext, instalacionid, commodity) {
    // debugger;

    //Cargamos commodity del formulario segun el tipo de oferta 
    formContext.getAttribute("atos_commodity").setValue(commodity);
    formContext.getAttribute("atos_commodity").setSubmitMode("always");

    if (instalacionid != null) {
        var fetchXml =
/* 223231 +1*/ //"<fetch mapping='logical'>"+
            "<fetch mapping='logical' no-lock='true'>" +
            "<entity name='atos_instalacion' > " +
            "<attribute name='atos_instalacionid' /> " +
            "<attribute name='atos_lote' /> " +
            "<attribute name='atos_name' /> " +
            "<attribute name='atos_razonsocialid' /> " +
            "<attribute name='atos_sistemaelectricoid' /> " +
            "<attribute name='atos_subsistemaid' /> " +
            "<attribute name='atos_tarifaid' /> " +
            "<attribute name='atos_consumoestimadototalanual' /> " +
            "<attribute name='atos_agentecomercialid' alias='atos_agentecomercialid' /> " +
            "<link-entity name='account' from='accountid' to='atos_razonsocialid' link-type='outer' alias='rz' > " +
            "<attribute name='atos_cuentanegociadoraid' alias='cuentanegociadoraid' /> " +
            "<attribute name='atos_numerodedocumento' alias='numerodedocumento'  /> " +
            "<attribute name='atos_grupoempresarialid' alias='atos_grupoempresarialid' /> " +
            "<attribute name='name' alias='razonsocial' /> " +
            "<attribute name='atos_formadepago' alias='formadepago' />" +
            "<attribute name='atos_condicionpagoid' alias='condicionpagoid' />" +
            "<attribute name='atos_tipodeenvio' alias='tipodeenvio' />" +
            "<attribute name='atos_plazoenviofacturas' alias='plazoenviofacturas' />" +
            "<attribute name='atos_mandatosepa' alias='mandatosepa' />" +
            "<attribute name='atos_swift' alias='swift' />" +
            "<attribute name='atos_iban' alias='iban' />" +
            "<attribute name='atos_entidadbancaria' alias='entidadbancaria' />" +
            "<attribute name='atos_sucursalbancaria' alias='sucursalbancaria' />" +
            "<attribute name='atos_digitocontrol' alias='digitocontrol' />" +
            "<attribute name='atos_cuenta' alias='cuenta' />" +
            "<attribute name='atos_cuentabancaria' alias='cuentabancaria' />" +
            "<attribute name='atos_cuentabancariaapropia' alias='cuentabancariaapropia' />" +
            "<link-entity name='atos_cuentanegociadora' from='atos_cuentanegociadoraid' to='atos_cuentanegociadoraid' link-type='outer' alias='cn' > " +
            "<attribute name='atos_name' alias='cuentanegociadora' /> " +
            "</link-entity> " +
            "<link-entity name='atos_condiciondepago' from='atos_condiciondepagoid' to='atos_condicionpagoid' link-type='outer' alias='cp' > " +
            "<attribute name='atos_name' alias='condiciondepago' /> " +
            "</link-entity> " +
            "</link-entity> " +
            "<link-entity name='atos_tarifa' from='atos_tarifaid' to='atos_tarifaid' link-type='outer' alias='ta' > " +
            "<attribute name='atos_name' alias='tarifa' /> " +
            "</link-entity> " +
            "<link-entity name='atos_sistemaelectrico' from='atos_sistemaelectricoid' to='atos_sistemaelectricoid' link-type='outer' alias='se' > " +
            "<attribute name='atos_name' alias='sistemaelectrico' /> " +
            "</link-entity> " +
            "<link-entity name='atos_subsistema' from='atos_subsistemaid' to='atos_subsistemaid' link-type='outer' alias='sse' > " +
            "<attribute name='atos_name' alias='subsistemaelectrico' /> " +
            "</link-entity> " +
            "<link-entity name='atos_agentecomercial' from='atos_agentecomercialid' to='atos_agentecomercialid' link-type='outer' alias='ac' > " +
            "<attribute name='atos_name' alias='agentecomercial' /> " +
            "</link-entity> " +
            "<filter>" +
            "<condition attribute='atos_instalacionid' operator='eq' value='" + instalacionid + "' />" +
            "</filter>" +
            "</entity> " +
            "</fetch>";

        // var registros = XrmServiceToolkit.Soap.Fetch(fetchXml);
        Xrm.WebApi.retrieveMultipleRecords("atos_instalacion", "?fetchXml=" + fetchXml).then(
            function success(registros) {
                // debugger;
                // <aqui se pone la logica validaciones para registros>
                if (registros != null && registros.entities != null && registros.entities.length > 0) {
                    var instalacion = registros.entities[0];
                    if (instalacion.atos_name != null) {
                        // Crear funcion general
                        var valorReferencia = new Array();
                        valorReferencia[0] = new Object();
                        valorReferencia[0].id = instalacion.atos_instalacionid; // instalacion.atos_instalacionid"].id; 
                        valorReferencia[0].name = instalacion.atos_name;
                        valorReferencia[0].entityType = "atos_instalacion";
                        formContext.getAttribute("atos_instalacionid").setValue(valorReferencia);
                        formContext.getAttribute("atos_instalacionid").setSubmitMode("always");
                    }

                    if (instalacion._atos_razonsocialid_value != null) {
                        // Crear funcion general
                        var valorReferencia = new Array();
                        valorReferencia[0] = new Object();
                        valorReferencia[0].id = instalacion._atos_razonsocialid_value;
                        valorReferencia[0].name = instalacion["_atos_razonsocialid_value@OData.Community.Display.V1.FormattedValue"];
                        valorReferencia[0].entityType = "account";
                        formContext.getAttribute("atos_razonsocialid").setValue(valorReferencia);
                        formContext.getAttribute("atos_razonsocialid").setSubmitMode("always");
                    }

                    if (instalacion.cuentanegociadoraid != null) {
                        // Crear funcion general
                        var valorReferencia = new Array();
                        valorReferencia[0] = new Object();
                        valorReferencia[0].id = instalacion.cuentanegociadoraid;
                        valorReferencia[0].name = instalacion["cuentanegociadoraid@OData.Community.Display.V1.FormattedValue"];
                        valorReferencia[0].entityType = "atos_cuentanegociadora";
                        formContext.getAttribute("atos_cuentanegociadoraid").setValue(valorReferencia);
                        formContext.getAttribute("atos_cuentanegociadoraid").setSubmitMode("always");
                    }

                    if (instalacion._atos_tarifaid_value != null) {
                        // Crear funcion general
                        var valorReferencia = new Array();
                        valorReferencia[0] = new Object();
                        valorReferencia[0].id = instalacion._atos_tarifaid_value;
                        valorReferencia[0].name = instalacion["_atos_tarifaid_value@OData.Community.Display.V1.FormattedValue"];
                        valorReferencia[0].entityType = "atos_tarifa";
                        formContext.getAttribute("atos_tarifaid").setValue(valorReferencia);
                        formContext.getAttribute("atos_tarifaid").setSubmitMode("always");
                    }

                    if (instalacion._atos_sistemaelectricoid_value != null) {
                        // Crear funcion general
                        var valorReferencia = new Array();
                        valorReferencia[0] = new Object();
                        valorReferencia[0].id = instalacion._atos_sistemaelectricoid_value;
                        valorReferencia[0].name = instalacion["_atos_sistemaelectricoid_value@OData.Community.Display.V1.FormattedValue"];
                        valorReferencia[0].entityType = "atos_sistemaelectrico";
                        formContext.getAttribute("atos_sistemaelectricoid").setValue(valorReferencia);
                        formContext.getAttribute("atos_sistemaelectricoid").setSubmitMode("always");
                    }

                    if (instalacion._atos_subsistemaid_value != null) {
                        // Crear funcion general
                        var valorReferencia = new Array();
                        valorReferencia[0] = new Object();
                        valorReferencia[0].id = instalacion._atos_subsistemaid_value;
                        valorReferencia[0].name = instalacion["_atos_subsistemaid_value@OData.Community.Display.V1.FormattedValue"];
                        valorReferencia[0].entityType = "atos_subsistema";
                        formContext.getAttribute("atos_subsistemaid").setValue(valorReferencia);
                        formContext.getAttribute("atos_subsistemaid").setSubmitMode("always");
                    }

                    if (instalacion._atos_agentecomercialid_value != null) {
                        // Crear funcion general
                        var valorReferencia = new Array();
                        valorReferencia[0] = new Object();
                        valorReferencia[0].id = instalacion._atos_agentecomercialid_value;
                        valorReferencia[0].name = instalacion["_atos_agentecomercialid_value@OData.Community.Display.V1.FormattedValue"];
                        valorReferencia[0].entityType = "atos_agentecomercial";
                        formContext.getAttribute("atos_agentecomercialid").setValue(valorReferencia);
                        formContext.getAttribute("atos_agentecomercialid").setSubmitMode("always");
                    }

                    if (instalacion.atos_lote != null) {
                        formContext.getAttribute("atos_lote").setValue(instalacion.atos_lote);
                        formContext.getAttribute("atos_lote").setSubmitMode("always");
                    }

                    if (instalacion.atos_consumoestimadototalanual != null) {
                        formContext.getAttribute("atos_consumoestimadoanual").setValue(instalacion.atos_consumoestimadototalanual);
                        formContext.getAttribute("atos_consumoestimadoanual").setSubmitMode("always");
                        formContext.getAttribute("atos_consumoagregadooferta").setValue(instalacion.atos_consumoestimadototalanual);
                        formContext.getAttribute("atos_consumoagregadooferta").setSubmitMode("always");
                    }

                    if (instalacion.numerodedocumento != null) {
                        formContext.getAttribute("atos_numerodocumento").setValue(instalacion.numerodedocumento);
                        formContext.getAttribute("atos_numerodocumento").setSubmitMode("always");
                    }

                    if (instalacion._atos_grupoempresarialid_value != null) {
                        // Crear funcion general
                        var valorReferencia = new Array();
                        valorReferencia[0] = new Object();
                        valorReferencia[0].id = instalacion._atos_grupoempresarialid_value;
                        valorReferencia[0].name = instalacion["_atos_grupoempresarialid_value@OData.Community.Display.V1.FormattedValue"];
                        valorReferencia[0].entityType = "atos_grupoempresarial";
                        formContext.getAttribute("atos_grupoempresarialid").setValue(valorReferencia);
                        formContext.getAttribute("atos_grupoempresarialid").setSubmitMode("always");
                    }

                    //Carga condiciones de pago y datos bancarios
                    if (instalacion.formadepago != null) {
                        formContext.getAttribute("atos_formadepago").setValue(instalacion.formadepago);
                        formContext.getAttribute("atos_formadepago").setSubmitMode("always");
                    }

                    if (instalacion.condicionpagoid != null) {
                        // Crear funcion general
                        var valorReferencia = new Array();
                        valorReferencia[0] = new Object();
                        valorReferencia[0].id = instalacion.condicionpagoid;
                        valorReferencia[0].name = instalacion["condicionpagoid@OData.Community.Display.V1.FormattedValue"];
                        valorReferencia[0].entityType = "atos_condiciondepago";
                        formContext.getAttribute("atos_condicionpagoid").setValue(valorReferencia);
                        formContext.getAttribute("atos_condicionpagoid").setSubmitMode("always");
                    }

                    if (instalacion.tipodeenvio != null) {
                        formContext.getAttribute("atos_tipodeenvio").setValue(instalacion.tipodeenvio);
                        formContext.getAttribute("atos_tipodeenvio").setSubmitMode("always");
                    }

                    if (instalacion.plazoenviofacturas != null) {
                        formContext.getAttribute("atos_plazoenviofacturas").setValue(instalacion.plazoenviofacturas);
                        formContext.getAttribute("atos_plazoenviofacturas").setSubmitMode("always");
                    }

                    if (instalacion.mandatosepa != null) {
                        formContext.getAttribute("atos_mandatosepa").setValue(instalacion.mandatosepa);
                        formContext.getAttribute("atos_mandatosepa").setSubmitMode("always");
                    }

                    if (instalacion.swift != null) {
                        formContext.getAttribute("atos_swift").setValue(instalacion.swift);
                        formContext.getAttribute("atos_swift").setSubmitMode("always");
                    }

                    if (instalacion.iban != null) {
                        formContext.getAttribute("atos_iban").setValue(instalacion.iban);
                        formContext.getAttribute("atos_iban").setSubmitMode("always");
                    }

                    if (instalacion.entidadbancaria != null) {
                        formContext.getAttribute("atos_entidadbancaria").setValue(instalacion.entidadbancaria);
                        formContext.getAttribute("atos_entidadbancaria").setSubmitMode("always");
                    }

                    if (instalacion.sucursalbancaria != null) {
                        formContext.getAttribute("atos_sucursalbancaria").setValue(instalacion.sucursalbancaria);
                        formContext.getAttribute("atos_sucursalbancaria").setSubmitMode("always");
                    }

                    if (instalacion.digitocontrol != null) {
                        formContext.getAttribute("atos_digitocontrol").setValue(instalacion.digitocontrol);
                        formContext.getAttribute("atos_digitocontrol").setSubmitMode("always");
                    }

                    if (instalacion.cuenta != null) {
                        formContext.getAttribute("atos_cuenta").setValue(instalacion.cuenta);
                        formContext.getAttribute("atos_cuenta").setSubmitMode("always");
                    }

                    if (instalacion.cuentabancaria != null) {
                        formContext.getAttribute("atos_cuentabancaria").setValue(instalacion.cuentabancaria);
                        formContext.getAttribute("atos_cuentabancaria").setSubmitMode("always");
                    }

                    if (instalacion.cuentabancariaapropia != null) {
                        formContext.getAttribute("atos_cuentabancariaapropia").setValue(instalacion.cuentabancariaapropia);
                        formContext.getAttribute("atos_cuentabancariaapropia").setSubmitMode("always");
                    }

                }
            }, function (error) {
                Xrm.Navigation.openErrorDialog({ detail: error.message });
            });
    }
}

/**
// <summary>
// Carga datos desde la instalaciÃ³n gas relacionada
// </summary>
// <param name="instalaciongasid">Identificador de la instalaciÃ³n gas.</param>
// <remarks>
// Accede a la instalaciÃ³n gas con el identificador recibido por parÃ¡metro<br/>
// y rellena los campos de la oferta a partir de los siguientes campos de la instalaciÃ³n:
// <ul>
// <li>InstalaciÃ³n gas</li>
// <li>RazÃ³n Social</li>
// <li>Cuenta negociadora</li>
// <li>Tarifa "gas"</li>
// <li>Agente comercial</li>
// <li>Consumo estimado total anual</li> "Por meter"
// <li>NÃºmero de documento</li> "CIF Razon Social"
// </ul>
// </remarks>
*/
function cargaDesdeInstalacionGas(formContext, instalaciongasid, commodity) {
    // debugger;

    //Cargamos commodity del formulario segun el tipo de oferta 
    formContext.getAttribute("atos_commodity").setValue(commodity);
    formContext.getAttribute("atos_commodity").setSubmitMode("always");

    if (instalaciongasid != null) {
        var fetchXml =
/* 223231 +1*/ //"<fetch mapping='logical'>"+
            "<fetch mapping='logical' no-lock='true'>" +
            "<entity name='atos_instalaciongas' > " +
            "<attribute name='atos_instalaciongasid' /> " +
            "<attribute name='atos_usodelgasid' /> " +
            "<attribute name='atos_name' /> " +
            "<attribute name='atos_razonsocialid' /> " +
            "<attribute name='atos_peajeid' /> " +
            "<link-entity name='account' from='accountid' to='atos_razonsocialid' link-type='outer' alias='rz' > " +
            "<attribute name='atos_cuentanegociadoraid' alias='cuentanegociadoraid' /> " +
            "<attribute name='atos_numerodedocumento' alias='numerodedocumento'  /> " +
            "<attribute name='atos_grupoempresarialid' alias='atos_grupoempresarialid' /> " +
            "<attribute name='name' alias='razonsocial' /> " +
            "<attribute name='atos_formadepago' alias='formadepago' />" +
            "<attribute name='atos_condicionpagoid' alias='condicionpagoid' />" +
            "<attribute name='atos_tipodeenvio' alias='tipodeenvio' />" +
            "<attribute name='atos_plazoenviofacturas' alias='plazoenviofacturas' />" +
            "<attribute name='atos_mandatosepa' alias='mandatosepa' />" +
            "<attribute name='atos_swift' alias='swift' />" +
            "<attribute name='atos_iban' alias='iban' />" +
            "<attribute name='atos_entidadbancaria' alias='entidadbancaria' />" +
            "<attribute name='atos_sucursalbancaria' alias='sucursalbancaria' />" +
            "<attribute name='atos_digitocontrol' alias='digitocontrol' />" +
            "<attribute name='atos_cuenta' alias='cuenta' />" +
            "<attribute name='atos_cuentabancaria' alias='cuentabancaria' />" +
            "<attribute name='atos_cuentabancariaapropia' alias='cuentabancariaapropia' />" +
            "<link-entity name='atos_condiciondepago' from='atos_condiciondepagoid' to='atos_condicionpagoid' link-type='outer' alias='cp' > " +
            "<attribute name='atos_name' alias='condiciondepago' /> " +
            "</link-entity> " +
            "<link-entity name='atos_cuentanegociadora' from='atos_cuentanegociadoraid' to='atos_cuentanegociadoraid' link-type='outer' alias='cn' > " +
            "<attribute name='atos_name' alias='cuentanegociadora' /> " +
            "</link-entity> " +
            "</link-entity> " +
            "<link-entity name='atos_tablasatrgas' from='atos_tablasatrgasid' to='atos_peajeid' link-type='outer' alias='tagp' > " +
            "<attribute name='atos_name' alias='peaje' /> " +
            "</link-entity> " +
            "<link-entity name='atos_tablasatrgas' from='atos_tablasatrgasid' to='atos_usodelgasid' link-type='outer' alias='tagug' > " +
            "<attribute name='atos_name' alias='usodelgas' /> " +
            "</link-entity> " +
            "<filter>" +
            "<condition attribute='atos_instalaciongasid' operator='eq' value='" + instalaciongasid + "' />" +
            "</filter>" +
            "</entity> " +
            "</fetch>";

        // var registros = XrmServiceToolkit.Soap.Fetch(fetchXml);
        Xrm.WebApi.retrieveMultipleRecords("atos_instalaciongas", "?fetchXml=" + fetchXml).then(
        function success(registros) {
            // debugger;
            // <aqui se pone la logica validaciones para registros>
            if (registros != null && registros.entities != null && registros.entities.length > 0) {
                var instalaciongas = registros.entities[0];

                if (instalaciongas.atos_name != null) {
                    // Crear funcion general
                    var valorReferencia = new Array();
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = instalaciongas.atos_instalaciongasid; // instalaciongas.atos_instalaciongasid.id; 
                    valorReferencia[0].name = instalaciongas.atos_name;
                    valorReferencia[0].entityType = "atos_instalaciongas";
                    formContext.getAttribute("atos_instalaciongasid").setValue(valorReferencia);
                    formContext.getAttribute("atos_instalaciongasid").setSubmitMode("always");
                }

                if (instalaciongas._atos_usodelgasid_value != null) {
                    // Crear funcion general
                    var valorReferencia = new Array();
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = instalaciongas._atos_usodelgasid_value;
                    valorReferencia[0].name = instalaciongas["_atos_usodelgasid_value@OData.Community.Display.V1.FormattedValue"];
                    valorReferencia[0].entityType = "atos_tablasatrgas";
                    formContext.getAttribute("atos_usodelgasid").setValue(valorReferencia);
                    formContext.getAttribute("atos_usodelgasid").setSubmitMode("always");
                }

                if (instalaciongas._atos_peajeid_value != null) {
                    // Crear funcion general
                    var valorReferencia = new Array();
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = instalaciongas._atos_peajeid_value;
                    valorReferencia[0].name = instalaciongas["_atos_peajeid_value@OData.Community.Display.V1.FormattedValue"];
                    valorReferencia[0].entityType = "atos_tablasatrgas";
                    formContext.getAttribute("atos_peajeid").setValue(valorReferencia);
                    formContext.getAttribute("atos_peajeid").setSubmitMode("always");
                }

                if (instalaciongas._atos_razonsocialid_value != null) {
                    // Crear funcion general
                    var valorReferencia = new Array();
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = instalaciongas._atos_razonsocialid_value;
                    valorReferencia[0].name = instalaciongas["_atos_razonsocialid_value@OData.Community.Display.V1.FormattedValue"];
                    valorReferencia[0].entityType = "account";
                    formContext.getAttribute("atos_razonsocialid").setValue(valorReferencia);
                    formContext.getAttribute("atos_razonsocialid").setSubmitMode("always");
                }

                if (instalaciongas.cuentanegociadoraid != null) {
                    // Crear funcion general
                    var valorReferencia = new Array();
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = instalaciongas.cuentanegociadoraid;
                    valorReferencia[0].name = instalaciongas["cuentanegociadoraid@OData.Community.Display.V1.FormattedValue"];
                    valorReferencia[0].entityType = "atos_cuentanegociadora";
                    formContext.getAttribute("atos_cuentanegociadoraid").setValue(valorReferencia);
                    formContext.getAttribute("atos_cuentanegociadoraid").setSubmitMode("always");
                }

                if (instalaciongas.numerodedocumento != null) {
                    formContext.getAttribute("atos_numerodocumento").setValue(instalaciongas.numerodedocumento);
                    formContext.getAttribute("atos_numerodocumento").setSubmitMode("always");
                }

                if (instalaciongas.atos_grupoempresarialid_value != null) {
                    // Crear funcion general
                    var valorReferencia = new Array();
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = instalaciongas.atos_grupoempresarialid_value;
                    valorReferencia[0].name = instalaciongas["atos_grupoempresarialid_value@OData.Community.Display.V1.FormattedValue"];
                    valorReferencia[0].entityType = "atos_grupoempresarial";
                    formContext.getAttribute("atos_grupoempresarialid").setValue(valorReferencia);
                    formContext.getAttribute("atos_grupoempresarialid").setSubmitMode("always");
                }

                //Carga condiciones de pago y datos bancarios
                if (instalaciongas.formadepago != null) {
                    formContext.getAttribute("atos_formadepago").setValue(instalaciongas.formadepago);
                    formContext.getAttribute("atos_formadepago").setSubmitMode("always");
                }

                if (instalaciongas.condicionpagoid != null) {
                    var valorReferencia = new Array();
                    // Crear funcion general
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = instalaciongas.condicionpagoid;
                    valorReferencia[0].name = instalaciongas["condicionpagoid@OData.Community.Display.V1.FormattedValue"];
                    valorReferencia[0].entityType = "atos_condiciondepago";
                    formContext.getAttribute("atos_condicionpagoid").setValue(valorReferencia);
                    formContext.getAttribute("atos_condicionpagoid").setSubmitMode("always");
                }

                if (instalaciongas.tipodeenvio != null) {
                    formContext.getAttribute("atos_tipodeenvio").setValue(instalaciongas.tipodeenvio);
                    formContext.getAttribute("atos_tipodeenvio").setSubmitMode("always");
                }

                if (instalaciongas.plazoenviofacturas != null) {
                    formContext.getAttribute("atos_plazoenviofacturas").setValue(instalaciongas.plazoenviofacturas);
                    formContext.getAttribute("atos_plazoenviofacturas").setSubmitMode("always");
                }

                if (instalaciongas.mandatosepa != null) {
                    formContext.getAttribute("atos_mandatosepa").setValue(instalaciongas.mandatosepa);
                    formContext.getAttribute("atos_mandatosepa").setSubmitMode("always");
                }

                if (instalaciongas.swift != null) {
                    formContext.getAttribute("atos_swift").setValue(instalaciongas.swift);
                    formContext.getAttribute("atos_swift").setSubmitMode("always");
                }

                if (instalaciongas.iban != null) {
                    formContext.getAttribute("atos_iban").setValue(instalaciongas.iban);
                    formContext.getAttribute("atos_iban").setSubmitMode("always");
                }

                if (instalaciongas.entidadbancaria != null) {
                    formContext.getAttribute("atos_entidadbancaria").setValue(instalaciongas.entidadbancaria);
                    formContext.getAttribute("atos_entidadbancaria").setSubmitMode("always");
                }

                if (instalaciongas.sucursalbancaria != null) {
                    formContext.getAttribute("atos_sucursalbancaria").setValue(instalaciongas.sucursalbancaria);
                    formContext.getAttribute("atos_sucursalbancaria").setSubmitMode("always");
                }

                if (instalaciongas.digitocontrol != null) {
                    formContext.getAttribute("atos_digitocontrol").setValue(instalaciongas.digitocontrol);
                    formContext.getAttribute("atos_digitocontrol").setSubmitMode("always");
                }

                if (instalaciongas.cuenta != null) {
                    formContext.getAttribute("atos_cuenta").setValue(instalaciongas.cuenta);
                    formContext.getAttribute("atos_cuenta").setSubmitMode("always");
                }

                if (instalaciongas.cuentabancaria != null) {
                    formContext.getAttribute("atos_cuentabancaria").setValue(instalaciongas.cuentabancaria);
                    formContext.getAttribute("atos_cuentabancaria").setSubmitMode("always");
                }

                if (instalaciongas.cuentabancariaapropia != null) {
                    formContext.getAttribute("atos_cuentabancariaapropia").setValue(instalaciongas.cuentabancariaapropia);
                    formContext.getAttribute("atos_cuentabancariaapropia").setSubmitMode("always");
                }

            }
        }, function (error) {
            Xrm.Navigation.openErrorDialog({ detail: error.message });
        });

    }
}

/*
 * Funcion para filtrar los productos base
 * 
 * @param {*} formContext 
 */
function setCustomLookupProductoBase(formContext) {

    var commodity = formContext.getAttribute("atos_commodity");
    var viewId = "{25986987-4065-A267-4889-637E6438B4E0}";
    var entityName = "atos_tipodeproducto";
    var viewDisplayName = "Productos Base del tipo " + commodity.getText();

/* 223231 +1*/ //var fetchXml = "<fetch mapping='logical' output-format='xml-platform' >" +
    var fetchXml = "<fetch mapping='logical' output-format='xml-platform' no-lock='true' >" +    
        "<entity name='atos_tipodeproducto'>" +
        "<attribute name='atos_name'/>" +
        "<attribute name='createdon'/>" +
        "<attribute name='atos_formula'/>" +
        "<attribute name='atos_descripcion'/>" +
        "<attribute name='atos_nombreems'/>" +
        "<attribute name='atos_tipodeproductoid'/>" +
        "<order descending='false' attribute='atos_name'/>" +
        "<filter type='and'>" +
        "<condition attribute='atos_commodity' value='" + commodity.getValue() + "' operator='eq'/>" +
        "<condition attribute='atos_base' value='1' operator='eq'/>" +
        "<condition attribute='statecode' value='0' operator='eq'/>" +
        "</filter>" +
        "</entity>" +
        "</fetch>";

    var layoutXml = "<grid name='resultset' " +
        "object='1' " +
        "jump='atos_name' " +
        "select='1' " +
        "icon='1' " +
        "preview='1'>" +
        "<row name='result' " +
        "id='atos_tipodeproductoid'>" +
        "<cell name='atos_name' " +
        "width='150' />" +
        "<cell name='atos_descripcion' " +
        "width='150' />" +
        "<cell name='atos_nombreems' " +
        "width='150' />" +
        "<cell name='atos_formula' " +
        "width='150' />" +
        "<cell name='createdon' " +
        "width='150' />" +
        "disableSorting='1' />" +
        "</row>" +
        "</grid>";
    //descripcion, nombre formula, formula, fecha de creacion

    formContext.getControl("atos_tipodeproductoid").addCustomView(viewId, entityName, viewDisplayName, fetchXml, layoutXml, true);
    formContext.getControl("header_process_atos_tipodeproductoid").addCustomView(viewId, entityName, viewDisplayName, fetchXml, layoutXml, true);
}

/*
 * Oculta los campos segun la commodity
 * 
 * @param {*} formContext 
 */
function mostrarcampos(formContext) {

    if (formContext.getAttribute("atos_commodity").getValue() == 300000000) {
        formContext.getControl("atos_usodelgasid").setVisible(false);
        formContext.getControl("atos_tipodepenalizacionid").setVisible(false);
        formContext.getControl("atos_peajeid").setVisible(false);
        formContext.getControl("atos_instalaciongasid").setVisible(false);
    }
    else if (formContext.getAttribute("atos_commodity").getValue() == 300000001) {
        formContext.getControl("atos_impuestoelectrico").setVisible(false);
        formContext.getControl("atos_instalacionid").setVisible(false);
        formContext.getControl("atos_tarifaid").setVisible(false);
        formContext.getControl("atos_sistemaelectricoid").setVisible(false);
        formContext.getControl("atos_subsistemaid").setVisible(false);
        formContext.getControl("atos_precioincluyeimpuestoelectrico").setVisible(false);
    }
    else {

    }
}


/*
 * Abre el formulario de nuevo contrato
 * Si la oferta estÃ¡ creada y el check validado para contrato estÃ¡ marcado abre el formulario de nuevo contrato indicandole que viene desde la oferta
 * Si el check validado para contrato no estÃ¡ marcado muestra el mensaje de que es necesario validar la oferta antes.
 * Modificado para funcionalidad D365 V9
 * Autor: LÃ¡zaro Castro  - 13.11.2020
 * 
 * @param {*} primaryControl 
 */
function nuevoContrato(primaryControl) { 
    debugger;
    if (primaryControl._entityReference.id["guid"] != null && primaryControl._entityReference.id["guid"] != "") {

        if (!primaryControl.getAttribute("atos_validadoparacontrato").getIsDirty() && primaryControl.getAttribute("atos_validadoparacontrato").getValue() != null) {
            if (!primaryControl.getAttribute("atos_validadoparacontrato").getIsDirty() && primaryControl.getAttribute("atos_validadoparacontrato").getValue() == true) {
                
                // comprobamos que la oferta esta ganada para lanzar el contrato
                if (!primaryControl.getAttribute("statuscode").getIsDirty() && primaryControl.getAttribute("statuscode").getValue() == 300000000) {
                    var entityFormOptions = {};
                    entityFormOptions["entityName"] = "atos_contrato";                    
/* #23251 +8 Begin */
                    // Costes por gestion de cierre
                    var costeGestionCierreMM = formContext.getAttribute("atos_costegestioncierresmensuales").getValue();
                    var costeGestionCierreQT = formContext.getAttribute("atos_costegestioncierrestrimestrales").getValue();
                    var costeGestionCierreYY = formContext.getAttribute("atos_costegestioncierresanuales").getValue();


                    var formParameters = {};
                    formParameters["atos_costegestioncierresmensuales"] = costeGestionCierreMM;
                    formParameters["atos_costegestioncierrestrimestrales"] = costeGestionCierreQT;
                    formParameters["atos_costegestioncierresanuales"] = costeGestionCierreYY;                    
/* #23251 End */
                    formParameters["llamado_desde"] = "Oferta";
                    formParameters["llamante_id"] = primaryControl._entityReference.id["guid"];
                    // Xrm.Utility.openEntityForm("atos_contrato", null, parameters); Deperecado a pasado a XRM.Nvigation.openForm
                    Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
                        function (success) {
                            // console.log(success);
                        }, function (error) {
                            Xrm.Navigation.openErrorDialog({ detail: error.message });
                        });
                } 
                else {
                    alert("Es necesario que la oferta este ganada para pasarla a contrato. Revise el Estado de oferta y guarde la oferta");
                }
            }
        }
    } 
    else {
        alert("Es necesario validar la oferta antes de pasarla a contrato. Revise el campo Validado para contrato");
    }
}


/*
 * Comprueba si el campo recibido por parÃ¡metro estÃ¡ vacÃ­o en el formulario
 * 
 * @param {*} formContext 
 * @param {*} campo 
 * @returns 
 */
function campoVacio(formContext, campo) {
    try {
        formContext = formContext.getFormContext();
    }
    catch {}

    if (formContext.getAttribute(campo).getValue() == null ||
        formContext.getAttribute(campo).getValue() == "")
        return true;

    return false;
}


/**
 * Se ejecuta al guardar los datos de la Oferta.
 * Si se ha modificado la fecha de inicio y/o fin de suministro/garantÃ­a/aval, comprueba que son correctas.
 * 
 * @param {*} executionContext 
 */
function Oferta_OnSave(executionContext) {

    var formContext = executionContext.getFormContext();
    if (formContext.getAttribute("atos_fechainicio").getIsDirty() == true ||
        formContext.getAttribute("atos_fechafin").getIsDirty() == true) {
        if (compruebaFechas(formContext,"atos_fechainicio", "atos_fechafin", "La fecha de inicio de suministro debe ser menor o igual que la fecha de fin de suministro.", "5") != "")
            if (executionContext.getEventArgs() != null)
                executionContext.getEventArgs().preventDefault();
    }

    if (formContext.getAttribute("atos_fechainiciogarantia").getIsDirty() == true ||
        formContext.getAttribute("atos_fechafingarantia").getIsDirty() == true) {
        if (compruebaFechas(formContext,"atos_fechainiciogarantia", "atos_fechafingarantia", "La fecha de inicio de garantÃ­a debe ser menor o igual que la fecha de fin de garantÃ­a.", "6") != "")
            if (executionContext.getEventArgs() != null)
                executionContext.getEventArgs().preventDefault();
    }

    if (formContext.getAttribute("atos_fechainicioavalprovisional").getIsDirty() == true ||
        formContext.getAttribute("atos_fechafinavalprovisional").getIsDirty() == true) {
        if (compruebaFechas(formContext,"atos_fechainicioavalprovisional", "atos_fechafinavalprovisional", "La fecha de inicio de aval debe ser menor o igual que la fecha de fin de aval.", "7") != "")
            if (executionContext.getEventArgs() != null)
                executionContext.getEventArgs().preventDefault();
    }
}



/*
 * Habilita/Deshabilita campos de penalizaciÃ³n consumo

 * Si estÃ¡ marcado el check de penalizaciÃ³n consumo, habilita los campos de penalizaciÃ³n y los pone requeridos
 *      atos_importepenalizacion
 *      atos_rangoinferiorpenalizacion
 *      atos_rangosuperiorpenalizacion
 * Si no estÃ¡ marcado el check de penalizaciÃ³n consumo, deshabilita los campos de penalizaciÃ³n, los vacÃ­a y los pone no requeridos
 * 
 * @param {*} executionContext 
 */
function penalizacionConsumo(executionContext) {
    // debugger;
    var formContext = executionContext.getFormContext();
    if (formContext.getAttribute("atos_penalizacionconsumo").getValue() == true) {
        formContext.getAttribute("atos_importepenalizacion").setRequiredLevel("required");
        formContext.getAttribute("atos_rangoinferiorpenalizacion").setRequiredLevel("required");
        formContext.getAttribute("atos_rangosuperiorpenalizacion").setRequiredLevel("required");
        formContext.getControl("atos_importepenalizacion").setDisabled(false);
        formContext.getControl("atos_rangoinferiorpenalizacion").setDisabled(false);
        formContext.getControl("atos_rangosuperiorpenalizacion").setDisabled(false);
        formContext.getControl("atos_tipodepenalizacionid").setDisabled(false);
        formContext.getControl("atos_referenciapenalizacion").setDisabled(false);
        formContext.getControl("atos_penalizacionporcentaje").setDisabled(false);
    }
    else {
        if (campoVacio(formContext, "atos_importepenalizacion") == false) // Si tiene valor lo vacÃ­a
        {
            formContext.getAttribute("atos_importepenalizacion").setValue(null);
            formContext.getAttribute("atos_importepenalizacion").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_rangoinferiorpenalizacion") == false) // Si tiene valor lo vacÃ­a
        {
            formContext.getAttribute("atos_rangoinferiorpenalizacion").setValue(null);
            formContext.getAttribute("atos_rangoinferiorpenalizacion").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_rangosuperiorpenalizacion") == false) // Si tiene valor lo vacÃ­a
        {
            formContext.getAttribute("atos_rangosuperiorpenalizacion").setValue(null);
            formContext.getAttribute("atos_rangosuperiorpenalizacion").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_tipodepenalizacionid") == false) {
            formContext.getAttribute("atos_tipodepenalizacionid").setValue(null);
            formContext.getAttribute("atos_tipodepenalizacionid").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_referenciapenalizacion") == false) {
            formContext.getAttribute("atos_referenciapenalizacion").setValue(null);
            formContext.getAttribute("atos_referenciapenalizacion").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_penalizacionporcentaje") == false) {
            formContext.getAttribute("atos_penalizacionporcentaje").setValue(null);
            formContext.getAttribute("atos_penalizacionporcentaje").setSubmitMode("always");
        }

        formContext.getAttribute("atos_importepenalizacion").setRequiredLevel("none");
        formContext.getAttribute("atos_rangoinferiorpenalizacion").setRequiredLevel("none");
        formContext.getAttribute("atos_rangosuperiorpenalizacion").setRequiredLevel("none");
        formContext.getControl("atos_importepenalizacion").setDisabled(true);
        formContext.getControl("atos_rangoinferiorpenalizacion").setDisabled(true);
        formContext.getControl("atos_rangosuperiorpenalizacion").setDisabled(true);
        formContext.getControl("atos_tipodepenalizacionid").setDisabled(true);
        formContext.getControl("atos_referenciapenalizacion").setDisabled(true);
        formContext.getControl("atos_penalizacionporcentaje").setDisabled(true);
    }
    if (formContext.getAttribute("atos_commodity").getValue() == 300000001) {
        referenciaPenalizacion(formContext);
    }
}

/**
// <summary>
// Habilita/Deshabilita los campos importe penalizaciÃ³n o penalizacion
// </summary>
// <remarks>
// Si el tipo de garantÃ­a no es Sin Garantia habilita los campos de garantÃ­a<br/>
// Si el tipo de garantÃ­a es Sin GarantÃ­a, deshabilita y vacÃ­a los campos de garantÃ­a
// </remarks>
*/
function referenciaPenalizacion(formContext) {

    try {
        formContext = formContext.getFormContext();
    }
    catch {}

    if (campoVacio(formContext, "atos_referenciapenalizacion") == true) {
        formContext.getAttribute("atos_importepenalizacion").setValue(null);
        formContext.getAttribute("atos_importepenalizacion").setSubmitMode("always");
        formContext.getAttribute("atos_importepenalizacion").setRequiredLevel("none");
        formContext.getControl("atos_importepenalizacion").setDisabled(true);
        formContext.getAttribute("atos_importepenalizacioninferior").setValue(null);
        formContext.getAttribute("atos_importepenalizacioninferior").setSubmitMode("always");
        formContext.getAttribute("atos_importepenalizacioninferior").setRequiredLevel("none");
        formContext.getControl("atos_importepenalizacioninferior").setDisabled(true);
        formContext.getAttribute("atos_penalizacionporcentaje").setValue(null);
        formContext.getAttribute("atos_penalizacionporcentaje").setSubmitMode("always");
        formContext.getAttribute("atos_penalizacionporcentaje").setRequiredLevel("none");
        formContext.getControl("atos_penalizacionporcentaje").setDisabled(true);
        formContext.getAttribute("atos_penalizacionporcentajeinferior").setValue(null);
        formContext.getAttribute("atos_penalizacionporcentajeinferior").setSubmitMode("always");
        formContext.getAttribute("atos_penalizacionporcentajeinferior").setRequiredLevel("none");
        formContext.getControl("atos_penalizacionporcentajeinferior").setDisabled(true);

        return;
    }

    if (formContext.getAttribute("atos_referenciapenalizacion").getValue() == 300000000) {
        if (campoVacio(formContext, "atos_penalizacionporcentaje") == false) {
            formContext.getAttribute("atos_penalizacionporcentaje").setValue(null);
            formContext.getAttribute("atos_penalizacionporcentaje").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_penalizacionporcentajeinferior") == false) {
            formContext.getAttribute("atos_penalizacionporcentajeinferior").setValue(null);
            formContext.getAttribute("atos_penalizacionporcentajeinferior").setSubmitMode("always");
        }
        formContext.getAttribute("atos_importepenalizacion").setRequiredLevel("required");
        formContext.getAttribute("atos_importepenalizacioninferior").setRequiredLevel("required");
        formContext.getAttribute("atos_penalizacionporcentaje").setRequiredLevel("none");
        formContext.getAttribute("atos_penalizacionporcentajeinferior").setRequiredLevel("none");
        formContext.getControl("atos_penalizacionporcentaje").setDisabled(true);
        formContext.getControl("atos_penalizacionporcentajeinferior").setDisabled(true);
        formContext.getControl("atos_importepenalizacion").setDisabled(false);
        formContext.getControl("atos_importepenalizacioninferior").setDisabled(false);

    } else {
        if (campoVacio(formContext, "atos_importepenalizacion") == false) {
            formContext.getAttribute("atos_importepenalizacion").setValue(null);
            formContext.getAttribute("atos_importepenalizacion").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_importepenalizacioninferior") == false) {
            formContext.getAttribute("atos_importepenalizacioninferior").setValue(null);
            formContext.getAttribute("atos_importepenalizacioninferior").setSubmitMode("always");
        }
        formContext.getAttribute("atos_importepenalizacion").setRequiredLevel("none");
        formContext.getAttribute("atos_importepenalizacioninferior").setRequiredLevel("none");
        formContext.getAttribute("atos_penalizacionporcentaje").setRequiredLevel("required");
        formContext.getAttribute("atos_penalizacionporcentajeinferior").setRequiredLevel("required");
        formContext.getControl("atos_penalizacionporcentaje").setDisabled(false);
        formContext.getControl("atos_penalizacionporcentajeinferior").setDisabled(false);
        formContext.getControl("atos_importepenalizacion").setDisabled(true);
        formContext.getControl("atos_importepenalizacioninferior").setDisabled(true);

    }
}


/*
 * Habilita/Deshabilita campos de bonificaciÃ³n
 * Si estÃ¡ marcado el check de bonificaciÃ³n consumo, habilita los campos de bonificaciÃ³n y los pone requeridos
 *   atos_importebonificacion
 *   atos_rangoinferiorbonificacion
 *   atos_rangosuperiorbonificacion
 * Si no estÃ¡ marcado el check de bonificaciÃ³n consumo, deshabilita los campos de bonificaciÃ³n, los vacÃ­a y los pone no requeridos
 * 
 * @param {*} executionContext 
 */
function bonificacionConsumo(executionContext) {
    // debugger;
    var formContext = executionContext.getFormContext();
    if (formContext.getAttribute("atos_bonificacionconsumo").getValue() == true) {
        formContext.getAttribute("atos_importebonificacion").setRequiredLevel("required");
        formContext.getAttribute("atos_rangoinferiorbonificacion").setRequiredLevel("required");
        formContext.getAttribute("atos_rangosuperiorbonificacion").setRequiredLevel("required");
        formContext.getControl("atos_importebonificacion").setDisabled(false);
        formContext.getControl("atos_rangoinferiorbonificacion").setDisabled(false);
        formContext.getControl("atos_rangosuperiorbonificacion").setDisabled(false);
    }
    else {
        if (campoVacio(formContext, "atos_importebonificacion") == false) // Si tiene valor lo vacÃ­a
        {
            formContext.getAttribute("atos_importebonificacion").setValue(null);
            formContext.getAttribute("atos_importebonificacion").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_rangoinferiorbonificacion") == false) // Si tiene valor lo vacÃ­a
        {
            formContext.getAttribute("atos_rangoinferiorbonificacion").setValue(null);
            formContext.getAttribute("atos_rangoinferiorbonificacion").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_rangosuperiorbonificacion") == false) // Si tiene valor lo vacÃ­a
        {
            formContext.getAttribute("atos_rangosuperiorbonificacion").setValue(null);
            formContext.getAttribute("atos_rangosuperiorbonificacion").setSubmitMode("always");
        }

        formContext.getAttribute("atos_importebonificacion").setRequiredLevel("none");
        formContext.getAttribute("atos_rangoinferiorbonificacion").setRequiredLevel("none");
        formContext.getAttribute("atos_rangosuperiorbonificacion").setRequiredLevel("none");
        formContext.getControl("atos_importebonificacion").setDisabled(true);
        formContext.getControl("atos_rangoinferiorbonificacion").setDisabled(true);
        formContext.getControl("atos_rangosuperiorbonificacion").setDisabled(true);
    }
}



/**
// <summary>
// Habilita/Deshabilita campos de garantÃ­a
// </summary>
// <remarks>
// Si es una oferta hija deshabilita los campos de garantÃ­a
// <ul>
// <li>atos_importegarantiasolicitada</li>
// <li>atos_fechasolicitudgarantiacliente</li>
// <li>atos_fecharealizaciongarantiacliente</li>
// <li>atos_fechainiciogarantia</li>
// <li>atos_fechafingarantia</li>
// </ul>
// Si el tipo de garantÃ­a no es Sin Garantia habilita los campos de garantÃ­a<br/>
// Si el tipo de garantÃ­a es Sin GarantÃ­a, deshabilita y vacÃ­a los campos de garantÃ­a
// </remarks>
*/
function tipoGarantia(executionContext) {
    // debugger;
    var formContext = executionContext.getFormContext();
    if (formContext.getAttribute("atos_ofertapadreid").getValue() != null) {
        formContext.getControl("atos_importegarantiasolicitada").setDisabled(true);
        formContext.getControl("atos_fechasolicitudgarantiacliente").setDisabled(true);
        formContext.getControl("atos_fecharealizaciongarantiacliente").setDisabled(true);
        formContext.getControl("atos_fechainiciogarantia").setDisabled(true);
        formContext.getControl("atos_fechafingarantia").setDisabled(true);
    }
    else {
        if (formContext.getAttribute("atos_tipogarantiasolicitadacliente").getValue() != 300000000) {
            formContext.getControl("atos_importegarantiasolicitada").setDisabled(false);
            formContext.getControl("atos_fechasolicitudgarantiacliente").setDisabled(false);
            formContext.getControl("atos_fecharealizaciongarantiacliente").setDisabled(false);
            formContext.getControl("atos_fechainiciogarantia").setDisabled(false);
            formContext.getControl("atos_fechafingarantia").setDisabled(false);
        }
        else {
            if (campoVacio(formContext, "atos_importegarantiasolicitada") == false) // Si tiene valor lo vacÃ­a
            {
                formContext.getAttribute("atos_importegarantiasolicitada").setValue(null);
                formContext.getAttribute("atos_importegarantiasolicitada").setSubmitMode("always");
            }
            if (campoVacio(formContext, "atos_fechasolicitudgarantiacliente") == false) // Si tiene valor lo vacÃ­a
            {
                formContext.getAttribute("atos_fechasolicitudgarantiacliente").setValue(null);
                formContext.getAttribute("atos_fechasolicitudgarantiacliente").setSubmitMode("always");
            }
            if (campoVacio(formContext, "atos_fecharealizaciongarantiacliente") == false) // Si tiene valor lo vacÃ­a
            {
                formContext.getAttribute("atos_fecharealizaciongarantiacliente").setValue(null);
                formContext.getAttribute("atos_fechainiciogarantia").setValue(null);
            }
            if (campoVacio(formContext, "atos_fechainiciogarantia") == false) // Si tiene valor lo vacÃ­a
            {
                formContext.getAttribute("atos_fechainiciogarantia").setValue(null);
                formContext.getAttribute("atos_fechainiciogarantia").setSubmitMode("always");
            }
            if (campoVacio(formContext, "atos_fechafingarantia") == false) // Si tiene valor lo vacÃ­a
            {
                formContext.getAttribute("atos_fechafingarantia").setValue(null);
                formContext.getAttribute("atos_fechafingarantia").setSubmitMode("always");
            }

            formContext.getControl("atos_importegarantiasolicitada").setDisabled(true);
            formContext.getControl("atos_fechasolicitudgarantiacliente").setDisabled(true);
            formContext.getControl("atos_fecharealizaciongarantiacliente").setDisabled(true);
            formContext.getControl("atos_fechainiciogarantia").setDisabled(true);
            formContext.getControl("atos_fechafingarantia").setDisabled(true);
        }
    }
}

/**
// <summary>
// Habilitar modificaciÃ³n de condiciones de pago y datos bancarios para ofertas padre 
//que vienen de una cuenta negociadora.
// </summary>
*/
function habilitarCondiciones(formContext) {
    try{
        formContext = formContext.getFormContext();
    }
    catch {}
    formContext.getAttribute("atos_formadepago").setRequiredLevel("required");
    formContext.getControl("atos_formadepago").setDisabled(false);
    formContext.getAttribute("atos_condicionpagoid").setRequiredLevel("required");
    formContext.getControl("atos_condicionpagoid").setDisabled(false);
    formContext.getAttribute("atos_tipodeenvio").setRequiredLevel("required");
    formContext.getControl("atos_tipodeenvio").setDisabled(false);
    formContext.getAttribute("atos_plazoenviofacturas").setRequiredLevel("required");
    formContext.getControl("atos_plazoenviofacturas").setDisabled(false);

    formContext.getControl("atos_mandatosepa").setDisabled(false);
    domiciliacionBancaria(formContext);
}


/*
 * Borramos el valor del campo mandato sepa
 * 
 * @param {*} formContext 
 */
function borrarMandatoSepa(formContext) {
    try{
        formContext = formContext.getFormContext();
    }
    catch{ }

    formContext.getAttribute("atos_mandatosepa").setValue("");
    formContext.getAttribute("atos_mandatosepa").setSubmitMode("always");
}

/*
 * Si la forma de pago es domiciliaciÃ³n bancaria habilita los campos de la cuenta bancaria
 * 
 * @param {*} formContext 
 */
function domiciliacionBancaria(formContext) {

    try {
        formContext = formContext.getFormContext();
    }
    catch{}

    if (formContext.getAttribute("atos_formadepago").getValue() == 300000001) // DomiciliaciÃ³n
    {
        formContext.getAttribute("atos_iban").setRequiredLevel("required");
        formContext.getAttribute("atos_entidadbancaria").setRequiredLevel("required");
        formContext.getAttribute("atos_sucursalbancaria").setRequiredLevel("required");
        formContext.getAttribute("atos_digitocontrol").setRequiredLevel("required");
        formContext.getAttribute("atos_cuenta").setRequiredLevel("required");
        formContext.getControl("atos_swift").setDisabled(false);
        formContext.getControl("atos_iban").setDisabled(false);
        formContext.getControl("atos_entidadbancaria").setDisabled(false);
        formContext.getControl("atos_sucursalbancaria").setDisabled(false);
        formContext.getControl("atos_digitocontrol").setDisabled(false);
        formContext.getControl("atos_cuenta").setDisabled(false);
        //formContext.getControl("atos_cuentabancaria").setDisabled(false);
    }
    else {
        formContext.ui.clearFormNotification("8"); // Limpiamos el posible error de validaciÃ³n de la cuenta
        formContext.getAttribute("atos_iban").setRequiredLevel("none");
        formContext.getAttribute("atos_entidadbancaria").setRequiredLevel("none");
        formContext.getAttribute("atos_sucursalbancaria").setRequiredLevel("none");
        formContext.getAttribute("atos_digitocontrol").setRequiredLevel("none");
        formContext.getAttribute("atos_cuenta").setRequiredLevel("none");
        if (campoVacio(formContext, "atos_swift") == false) // Si tiene valor lo vacÃ­a
        {
            formContext.getAttribute("atos_swift").setValue(null);
            formContext.getAttribute("atos_swift").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_iban") == false) // Si tiene valor lo vacÃ­a
        {
            formContext.getAttribute("atos_iban").setValue(null);
            formContext.getAttribute("atos_iban").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_entidadbancaria") == false) // Si tiene valor lo vacÃ­a
        {
            formContext.getAttribute("atos_entidadbancaria").setValue(null);
            formContext.getAttribute("atos_entidadbancaria").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_sucursalbancaria") == false) // Si tiene valor lo vacÃ­a
        {
            formContext.getAttribute("atos_sucursalbancaria").setValue(null);
            formContext.getAttribute("atos_sucursalbancaria").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_digitocontrol") == false) // Si tiene valor lo vacÃ­a
        {
            formContext.getAttribute("atos_digitocontrol").setValue(null);
            formContext.getAttribute("atos_digitocontrol").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_cuenta") == false) // Si tiene valor lo vacÃ­a
        {
            formContext.getAttribute("atos_cuenta").setValue(null);
            formContext.getAttribute("atos_cuenta").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_cuentabancaria") == false) // Si tiene valor lo vacÃ­a
        {
            formContext.getAttribute("atos_cuentabancaria").setValue(null);
            formContext.getAttribute("atos_cuentabancaria").setSubmitMode("always");
        }
        formContext.getControl("atos_swift").setDisabled(true);
        formContext.getControl("atos_iban").setDisabled(true);
        formContext.getControl("atos_entidadbancaria").setDisabled(true);
        formContext.getControl("atos_sucursalbancaria").setDisabled(true);
        formContext.getControl("atos_digitocontrol").setDisabled(true);
        formContext.getControl("atos_cuenta").setDisabled(true);
    }
}

/*
 * Funciones Generales, Analizar sacastas a otro apartado --------------------------------------------------------
 */


/*
 * Valida si la cuenta bancaria es correcta o no
 * @param {*} i_entidad 
 * @param {*} i_oficina 
 * @param {*} i_digito 
 * @param {*} i_cuenta 
 * @returns 
 */
function validaCuentaBancaria(i_entidad, i_oficina, i_digito, i_cuenta) {
    // Funcion recibe como parÃ¡metro la entidad, la oficina, 
    // el digito (concatenaciÃ³n del de control entidad-oficina y del de control entidad)
    // y la cuenta. Espera los valores con 0's a la izquierda.
    // Devuelve true o false.
    // NOTAS:
    // Formato deseado de los parÃ¡metros:
    // - i_entidad (4)
    // - i_oficina (4)
    // - i_digito (2)
    // - i_cuenta (10)
    var wtotal, wcociente, wresto;
    if (i_entidad.length != 4) {
        return false;
    }
    if (i_oficina.length != 4) {
        return false;
    }
    if (i_digito.length != 2) {
        return false;
    }
    if (i_cuenta.length != 10) {
        return false;
    }
    wtotal = i_entidad.charAt(0) * 4;
    wtotal += i_entidad.charAt(1) * 8;
    wtotal += i_entidad.charAt(2) * 5;
    wtotal += i_entidad.charAt(3) * 10;
    wtotal += i_oficina.charAt(0) * 9;
    wtotal += i_oficina.charAt(1) * 7;
    wtotal += i_oficina.charAt(2) * 3;
    wtotal += i_oficina.charAt(3) * 6;
    // busco el resto de dividir wtotal entre 11
    wcociente = Math.floor(wtotal / 11);
    wresto = wtotal - (wcociente * 11);
    //
    wtotal = 11 - wresto;
    if (wtotal == 11) {
        wtotal = 0;
    }
    if (wtotal == 10) {
        wtotal = 1;
    }
    if (wtotal != i_digito.charAt(0)) {
        return false;
    }
    //hemos validado la entidad y oficina
    //-----------------------------------
    wtotal = i_cuenta.charAt(0) * 1;
    wtotal += i_cuenta.charAt(1) * 2;
    wtotal += i_cuenta.charAt(2) * 4;
    wtotal += i_cuenta.charAt(3) * 8;
    wtotal += i_cuenta.charAt(4) * 5;
    wtotal += i_cuenta.charAt(5) * 10;
    wtotal += i_cuenta.charAt(6) * 9;
    wtotal += i_cuenta.charAt(7) * 7;
    wtotal += i_cuenta.charAt(8) * 3;
    wtotal += i_cuenta.charAt(9) * 6;

    // busco el resto de dividir wtotal entre 11
    wcociente = Math.floor(wtotal / 11);
    wresto = wtotal - (wcociente * 11);
    //
    wtotal = 11 - wresto;
    if (wtotal == 11) { wtotal = 0; }
    if (wtotal == 10) { wtotal = 1; }

    if (wtotal != i_digito.charAt(1)) {
        return false;
    }
    // hemos validado la cuenta corriente

    return true;
}


/*
 * Calcula el Iban de un nÃºmero de cuenta
 * @param {*} formContext 
 * @param {*} ccc 
 */
function calcularIban(formContext,ccc) {

    formContext.ui.clearFormNotification("9");
    //Limpiamos el numero de IBAN
    var cccaux = ccc.toUpperCase();  //Todo a Mayus
    cccaux = trim(cccaux); //Quitamos blancos de principio y final.
    cccaux = cccaux.replace(/\s/g, "");  //Quitamos blancos del medio.
    cccaux = cccaux + "142800"; // AÃ±adimos el cÃ³digo de EspaÃ±a

    var trozo = cccaux.substr(0, 5);
    var resto = trozo % 97;

    trozo = cccaux.substr(5, 5);
    var trozoaux = resto + trozo;
    resto = trozoaux % 97;

    trozo = cccaux.substr(10, 5);
    trozoaux = resto + trozo;
    resto = trozoaux % 97;

    trozo = cccaux.substr(15, 5);
    trozoaux = resto + trozo;
    resto = trozoaux % 97;

    trozo = cccaux.substr(20, 6);
    trozoaux = resto + trozo;
    resto = trozoaux % 97;

    digitocontrol = 98 - resto;
    if (digitocontrol < 10) digitocontrol = '0' + String(digitocontrol);
    formContext.getAttribute("atos_iban").setValue("ES" + digitocontrol);
}


/*
 * Elimina los espacios de una cadena
 * @param {*} myString 
 * @returns 
 */
function trim(myString) {
    return myString.replace(/^\s+/g, '').replace(/\s+$/g, '');
}


/*
 * Calcula el Iban si la cuenta bancaria es correcta
 * @param {*} formContext 
 * @returns 
 */
function validaCuenta(formContext) {
    try{
        formContext = formContext.getFormContext();
    }
    catch{}
    
    formContext.ui.clearFormNotification();
    var entidad = formContext.getAttribute("atos_entidadbancaria").getValue();
    var oficina = formContext.getAttribute("atos_sucursalbancaria").getValue();
    var dc = formContext.getAttribute("atos_digitocontrol").getValue();
    var cuenta = formContext.getAttribute("atos_cuenta").getValue();

    var validacion = true;

    if (entidad != null && oficina != null && dc != null && cuenta != null) {
        if (entidad.length == 4 && oficina.length == 4 && dc.length == 2 && cuenta.length == 10) {
            validacion = validaCuentaBancaria(entidad, oficina, dc, cuenta);
            if (!validacion)
                formContext.ui.setFormNotification("Cuenta bancaria incorrecta.", "ERROR", "8"); // alert("Cuenta bancaria incorrecta");
            else
                calcularIban(formContext,entidad + oficina + dc + cuenta);
        }
        else {
            validacion = false;
            formContext.ui.setFormNotification("Cuenta bancaria incorrecta.", "ERROR", "8");
            //alert("Cuenta bancaria incorrecta.");
        }

    }
    return validacion;
}