/*---------------------------------------------------------------------------------
 atos_contrato.js 
 Atos IT
 Copyright (c) Atos. All rights reserved.

 Creación: XX/XX/XXXX ATOS
 Ultima actualizacion

 Fecha       redmine    Descripcion                                 Autor
 13.11.2020             Se ejecuta en la llamada para cargar la
                        instalación segun instalacionid.            Lazaro Castro
 10.03.2021  22323      Lectura condiciones de pago cuando es       ACR
                        creacion de un contrato desde oferta
                       

---------------------------------------------------------------------------------*/

//#region Variables Funciones Globales 22323
var globalContext;
var formContext;
var codeLang;

function getGlobalContext() { return Xrm.Utility.getGlobalContext(); }
function getContext() { return formContext; }
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
function getFormType() {  return Xrm.Page.ui.getFormType(); } // Form type  0-Undefined, 1-Create 2-Update 3-Read Only 4-Disabled  6-Bulk Edit
function getServerUrl() {  return Xrm.Page.context.getClientUrl(); } 

//#endregion 22323 


//#region Evento onLoad 22323
/*
 * Function from OnLoad event
 * @param {executionContext} Contexto de ejecucion del formulario
 * Aplicado al evento Onload de primero
 */
function LoadForm(executionContext) {
    debugger;
    //Recupera contexto
    formContext = executionContext.getFormContext();
    // referencia de la API de cliente
    globalContext = Xrm.Utility.getGlobalContext();
    //Idioma
    codeLang = globalContext.userSettings.languageId;
}
//#endregion Evento onLoad 22323



/*
 * Function from OnLoad event
 * @param {executionContext} Contexto de ejecucion del formulario
 * Se ejecuta en el OnLoad del formulario para cargar datos de la oferta cuando se trata de nuevo contrato y 
 * para inicializar la cuenta bancaria propia.
 * Modificado para funcionalidad D365 V9
 * Autor: Lázaro Castro  - 13.11.2020
 */
function ContratoOnLoad(executionContext) {
debugger;

 /*223231*///var globalContext = Xrm.Utility.getGlobalContext();
    globalContext = getGlobalContext();
 /*223231*///var serverUrl = globalContext.getClientUrl();
 /*223231*///var formContext = executionContext.getFormContext();
    formContext = executionContext.getFormContext();
    // var serverUrl = Xrm.Page.context.getClientUrl();

 /*223231*///if (Xrm.Page.data.entity.getId() == null || Xrm.Page.data.entity.getId() == "") {
    //if ( getId() == null ||  getId() == "") {
    if (IsEmptyId()) {

        // var xrmObject = Xrm.Page.context.getQueryStringParameters();
        var xrmObject = globalContext.getQueryStringParameters();

        if (xrmObject != null) {

            if (xrmObject["llamado_desde"] != null) {

                var llamado_desde = xrmObject["llamado_desde"].toString();
                if (llamado_desde == "Oferta") {

                    if (xrmObject["llamante_id"] != null) {

                        var ofertaid = xrmObject["llamante_id"].toString();
                        /*223231*/ //head.load(serverUrl + "/WebResources/atos_json2.js", serverUrl + "/WebResources/atos_jquery.js", function () {
                        // Se comenta pus no hace falta cargar el atos_XrmServiceToolkit.js
                        // head.load(serverUrl + "/WebResources/atos_json2.js", serverUrl + "/WebResources/atos_jquery.js", serverUrl + "/WebResources/atos_XrmServiceToolkit.js", function () {

                        try {                                                                                   /*223231*/
                                head.load( getServerUrl() + "/WebResources/atos_json2.js", 
                                           getServerUrl() + "/WebResources/atos_jquery.js", function () {                            
                                    cargaDesdeOferta(formContext, ofertaid);
                                    mostrarcampos(formContext);
                                });
                        }                                                                                       /*223231*/
                        catch (error) {                                                                         /*223231*/
                            console.log("Error ContratoOnLoad(head.load1): (" + error.description + ")");       /*223231*/
                        } 
                        
                        return;
                    }
                }
            }
        }
    }

    if (formContext.getAttribute("atos_cuentabancariaapropia").getValue() == null) {
        /*223231*///head.load(serverUrl + "/WebResources/atos_json2.js", serverUrl + "/WebResources/atos_jquery.js", function () {
        //head.load(serverUrl + "/WebResources/atos_json2.js", serverUrl + "/WebResources/atos_jquery.js", serverUrl + "/WebResources/atos_XrmServiceToolkit.js", function () {
            
        /* Comentado por Aleix Ruiz día 30/11 - No se utiliza en producción
        var parametros = parametrosComercializadora();
            if (parametros.length > 0) {
                if (parametros[0].attributes["atos_cuentabancaria"] != null) {
                    formContext.getAttribute("atos_cuentabancariaapropia").setValue(parametros[0].attributes["atos_cuentabancaria"].value);
                    formContext.getAttributee("atos_cuentabancariaapropia").setSubmitMode("always");
                }
            }*/
 /*223231*///mostrarcampos(formContext);
 /*223231*///});

        try {                                                                                   /*223231*/
            head.load( getServerUrl() + "/WebResources/atos_json2.js", 
                       getServerUrl() + "/WebResources/atos_jquery.js", function () {

/*--- Inicio - Este codigo estaba en TEST ---------*/ 
 /*223231 begin */ 
                debugger;
                var fetchXml =
/*223231*/          //"<fetch mapping='logical'>" +
                    "<fetch mapping='logical' no-lock='true'>" +
                    "<entity name='atos_parametroscomercializadora' > " +
                        "<attribute name='atos_cuentabancaria' /> " +
                        "<attribute name='atos_name' /> " +
                        "<filter>" +
                        "<condition attribute='atos_name' operator='eq' value='Parámetros Comercializadora' />" +
                        "</filter>" +
                    "</entity> " +
                    "</fetch>";

                var statement = "?fetchXml=" + encodeURIComponent(fetchXml);                               
                Xrm.WebApi.retrieveMultipleRecords("atos_parametroscomercializadora", statement).then(      
                    function success(result) {

/*223231*/            //var result = parametrosComercializadora();
                        if (result.length > 0) {
                            if (result[0].attributes["atos_cuentabancaria"] != null) {
                                Xrm.Page.getAttribute("atos_cuentabancariaapropia").setValue(result[0].attributes["atos_cuentabancaria"].value);
                                Xrm.Page.getAttribute("atos_cuentabancariaapropia").setSubmitMode("always");
                            }
                        }
                        mostrarcampos(formContext);
                    });
 /*223231 end */                     
            });
        }                                                                                       /*223231*/

/*--- Fin - Este codigo estaba en TEST ---------*/         
        catch (error) {                                                                         /*223231*/
            console.log("Error ContratoOnLoad(head.load2): (" + error.description + ")");       /*223231*/
        }                                                                                       /*223231*/

    } 
    else {
        // head.load(serverUrl + "/WebResources/atos_json2.js", serverUrl + "/WebResources/atos_jquery.js", serverUrl + "/WebResources/atos_XrmServiceToolkit.js", function () {
        /*223231*///head.load(serverUrl + "/WebResources/atos_json2.js", serverUrl + "/WebResources/atos_jquery.js", function () {
        /*223231*///    mostrarcampos(formContext);
        /*223231*///});
        try {                                                                                   /*223231*/
            head.load( getServerUrl() + "/WebResources/atos_json2.js", 
                       getServerUrl() + "/WebResources/atos_jquery.js", function () {
                mostrarcampos();
            });
        }                                                                                       /*223231*/
        catch (error) {                                                                         /*223231*/
            console.log("Error ContratoOnLoad(head.load3): (" + error.description + ")");       /*223231*/
        }                                                                                       /*223231*/        
    }
}

/**
// <summary>
// Se ejecuta en la llamada para cargar la instalación segun instalacionid.
// Autor: Lazaro Castro - 13.11.2020
// </summary>
*/
function cargaInstalacion(formContext, instalacionid) {

    if (instalacionid == null) { return; }                                          /*223231*/
/*223231*///if (instalacionid != null) {
        var fetchXml =
/*223231*///"<fetch mapping='logical'>" +
            "<fetch mapping='logical' no-lock='true'>" +
            "<entity name='atos_instalacion' > " +
            "<attribute name='atos_tipoexencionie' /> " +
            "<attribute name='atos_fechainicioexencionie' /> " +
            "<attribute name='atos_fechafinexencionie' /> " +
            "<attribute name='atos_cie' /> " +
            "<attribute name='atos_baseimponibleexencion' /> " +
            "<attribute name='atos_porcentajeexencion' /> " +
            "<attribute name='atos_consumomaximoconexencion' /> " +
            "<attribute name='atos_consultoraid' /> " +
            "<link-entity name='atos_consultora' from='atos_consultoraid' to='atos_consultoraid' link-type='outer' alias='con' > " +
            "<attribute name='atos_name' alias='nameconsultora' /> " +
            "</link-entity> " +
            "<filter>" +
            "<condition attribute='atos_instalacionid' operator='eq' value='" + instalacionid + "' />" +
            "</filter>" +
            "</entity> " +
            "</fetch>";

debugger;
    try {                                                                                   /*223231*/
/*223231*///var registros = XrmServiceToolkit.Soap.Fetch(fetchXml);
/*223231*///Xrm.WebApi.retrieveMultipleRecords("atos_instalacion", "?fetchXml=" + fetchXml).then(        
        var statement = "?fetchXml=" + encodeURIComponent(fetchXml);                        /*223231*/
        Xrm.WebApi.online.retrieveMultipleRecords("atos_instalacion", statement).then(      /*223231*/
        function success(registros) {                                                       /*223231*/

            if (registros != null && registros.entities != null && registros.entities.length > 0) {
                var instalacion = registros.entities[0];
                if (instalacion.atos_tipoexencionie != null) {
                    formContext.getAttribute("atos_tipoexencionie").setValue(instalacion.atos_tipoexencionie);
                    formContext.getAttribute("atos_tipoexencionie").setSubmitMode("always");
                }
                if (instalacion._atos_consultoraid_value != null) {
                    var valorRefConsultora = new Array();
                    valorRefConsultora[0] = new Object();
                    valorRefConsultora[0].id = instalacion._atos_consultoraid_value;
                    valorRefConsultora[0].name = oferta["_atos_consultoraid_value@OData.Community.Display.V1.FormattedValue"];
                    valorRefConsultora[0].entityType = "atos_consultora";
                    formContext.getAttribute("atos_consultoraid").setValue(valorRefConsultora);
                    formContext.getAttribute("atos_consultoraid").setSubmitMode("always");
                }
                if (instalacion.atos_cie != null) {
                    formContext.getAttribute("atos_cie").setValue(instalacion.atos_cie);
                    formContext.getAttribute("atos_cie").setSubmitMode("always");
                }
                if (instalacion.atos_baseimponibleexencion != null) {
                    formContext.getAttribute("atos_baseimponibleexencion").setValue(instalacion.atos_baseimponibleexencion);
                    formContext.getAttribute("atos_baseimponibleexencion").setSubmitMode("always");
                }
                if (instalacion.atos_porcentajeexencion != null) {
                    formContext.getAttribute("atos_porcentajeexencion").setValue(instalacion.atos_porcentajeexencion);
                    formContext.getAttribute("atos_porcentajeexencion").setSubmitMode("always");
                }
                if (instalacion.atos_consumomaximoconexencion != null) {
                    formContext.getAttribute("atos_consumomaximoconexencion").setValue(instalacion.atos_consumomaximoconexencion);
                    formContext.getAttribute("atos_consumomaximoconexencion").setSubmitMode("always");
                }
                if (instalacion.atos_fechainicioexencionie != null) {
                    formContext.getAttribute("atos_fechainicioexencionie").setValue(instalacion.atos_fechainicioexencionie);
                    formContext.getAttribute("atos_fechainicioexencionie").setSubmitMode("always");
                }
                if (instalacion.atos_fechafinexencionie != null) {
                    formContext.getAttribute("atos_fechafinexencionie").setValue(instalacion.atos_fechafinexencionie);
                    formContext.getAttribute("atos_fechafinexencionie").setSubmitMode("always");
                }
            }
/*223231*/// }, function (error) {
/*223231*///     Xrm.Navigation.openErrorDialog({ detail: error.message });
        });
    }                                                                                       /*223231*/
    catch (error) {                                                                         /*223231*/
        console.log("Error cargaInstalacion: (" + error.description + ")");                 /*223231*/
    }                                                                                       /*223231*/
}

/**
// <summary>
// Se ejecuta en la llamada para cargar la información del contrato desde la oferta
// Autor: Lazaro Castro - 16.11.2020
// </summary>
 */
function cargaDesdeOferta(formContext, ofertaid) {

    if (ofertaid == null) { return; }                                                       /*223231*/

/*223231*///if (ofertaid != null) 
/*223231*///{

    var fetchXml =
/*223231*///"<fetch mapping='logical'>" +
        "<fetch mapping='logical' no-lock='true'>" +            
        "<entity name='atos_oferta' > " +
        "<attribute name='atos_commodity' /> " +
        "<attribute name='atos_instalacionid' /> " +
        "<attribute name='atos_instalaciongasid' /> " +
        "<attribute name='atos_name' /> " +
        "<attribute name='atos_cuentanegociadoraid' /> " +
        "<attribute name='atos_razonsocialid' /> " +
        "<attribute name='atos_importebonificacion' /> " +
        "<attribute name='atos_tipogarantiasolicitadacliente' /> " +
        "<attribute name='atos_importegarantiasolicitada' /> " +
        "<attribute name='atos_fechasolicitudgarantiacliente' /> " +
        "<attribute name='atos_fecharealizaciongarantiacliente' /> " +
        "<attribute name='atos_fechainiciogarantia' /> " +
        "<attribute name='atos_fechafingarantia' /> " +
        "<attribute name='atos_fechaaceptacionoferta' /> " +
        "<attribute name='atos_impuestoelectrico' /> " +
        "<attribute name='atos_renovaciontacita' /> " +
        "<attribute name='atos_gestionatr' /> " +
        "<attribute name='atos_penalizacionconsumo' /> " +
        "<attribute name='atos_importepenalizacion' /> " +
        "<attribute name='atos_rangoinferiorpenalizacion' /> " +
        "<attribute name='atos_rangosuperiorpenalizacion' /> " +
        "<attribute name='atos_bonificacionconsumo' /> " +
        "<attribute name='atos_rangoinferiorbonificacion' /> " +
        "<attribute name='atos_rangosuperiorbonificacion' /> " +
        "<attribute name='atos_fechainicio' /> " +
        "<attribute name='atos_fechafin' /> " +
        "<attribute name='atos_fechasolicitudavalprovisional' /> " +
        "<attribute name='atos_fecharealizacionavalprovisional' /> " +
        "<attribute name='atos_fechainicioavalprovisional' /> " +
        "<attribute name='atos_fechafinavalprovisional' /> " +
        "<attribute name='atos_importeavalprovisionalsolicitado' /> " +
        "<attribute name='atos_comentarios' /> " +
        "<attribute name='atos_precioincluyegestionatr' /> " +
        "<attribute name='atos_precioincluyeimpuestoelectrico' /> " +
        "<attribute name='atos_beneficioestimadooferta' /> " +
        "<attribute name='atos_tipodeproductofinalid' /> " +
        "<attribute name='atos_formadepago' alias='formadepago' />" +
        "<attribute name='atos_condicionpagoid' />" +
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
        "<attribute name='atos_numclicsmensual' alias='numclicsmensual' />" +
        "<attribute name='atos_numclicstrimestral' alias='numclicstrimestral' />" +
        "<attribute name='atos_numclicsanual' alias='numclicsanual' />" +
        "<attribute name='atos_costegestioncierremwh' alias='atos_costegestioncierremwh' />" +
        "<attribute name='atos_costegestioncierresmensuales' alias='atos_costegestioncierresmensuales' />" +
        "<attribute name='atos_costegestioncierrestrimestrales' alias='atos_costegestioncierrestrimestrales' />" +
        "<attribute name='atos_costegestioncierresanuales' alias='atos_costegestioncierresanuales' />" +
        "<link-entity name='account' from='accountid' to='atos_razonsocialid' link-type='outer' alias='rz' > " +
        "<attribute name='name' alias='razonsocial' /> " +
        "<attribute name='atos_numerodedocumento' alias='atos_numerodocumento' /> " +
        "<attribute name='atos_precisiondecimalesoferta' />" +
        "</link-entity> " +
        "<link-entity name='atos_instalacion' from='atos_instalacionid' to='atos_instalacionid' link-type='outer' alias='in' > " +
        "<attribute name='atos_name' alias='instalacion' /> " +
        "<attribute name='atos_cups20' alias='cups' /> " +
        "<attribute name='atos_contactocomercialid' alias='atos_contactocomercialid' /> " +
        "<attribute name='atos_canal' alias='atos_canal' /> " +
        "<attribute name='atos_agentefacturacionid' alias='atos_agentefacturacionid' /> " +
        "<attribute name='atos_fechaasignacionagentefacturacion' alias='atos_fechaasignacionagentefacturacion' /> " +
        "<attribute name='atos_tipopropiedadaparatoid' alias='atos_tipopropiedadaparatoid' /> " +
        "<attribute name='atos_importealquilerequipo' alias='atos_importealquilerequipo' /> " +
        "<attribute name='atos_modolecturapuntomedidaid' alias='atos_modolecturapuntomedidaid' /> " +
        "<attribute name='atos_instalacionccaaid' alias='atos_instalacionccaaid' /> " +
        "<attribute name='atos_tarifaid' alias='atos_tarifaid' /> " +
        "<attribute name='atos_sistemaelectricoid' alias='atos_sistemaelectricoid' /> " +
        "<attribute name='atos_subsistemaid' alias='atos_subsistemaid' /> " +
        "<attribute name='atos_lote' alias='atos_lote' /> " +
        "<attribute name='atos_agentecomercialid' alias='atos_agentecomercialid' /> " +
        "<attribute name='atos_fechaasignacionagente' alias='atos_fechaasignacionagente' /> " +
        "</link-entity> " +
        "<link-entity name='atos_instalaciongas' from='atos_instalaciongasid' to='atos_instalaciongasid' link-type='outer' alias='ingas'>" +
        "<attribute name='atos_name' alias='instalaciongas' />" +
        "<attribute name='atos_cups20' alias='cupsgas' />" +
        "<attribute name='atos_peajeid' alias='atos_peajeid' />" +
        "<attribute name='atos_usodelgasid' alias='atos_usodelgasid' />" +
        "<attribute name='atos_contactocomercialid' alias='contactocomercialgasid' /> " +
        "<attribute name='atos_fechainiciovigenciapeaje' alias='atos_fechainiciovigenciapeaje' />" +
        "<attribute name='atos_canal' alias='atos_canalgas' /> " +
        "<attribute name='atos_consultoraid' alias='atos_consultoraidgas' /> " +
        "<attribute name='atos_agentecomercialid' alias='atos_agentecomercialidgas' /> " +
        "<attribute name='atos_fechaasignacionagente' alias='atos_fechaasignacionagentegas' /> " +
        "<attribute name='atos_agentefacturacionid' alias='atos_agentefacturacionidgas' /> " +
        "<attribute name='atos_fechaasignacionagentefacturacion' alias='atos_fechaasignacionagentefacturaciongas' /> " +
        "</link-entity>" +
        "<filter>" +
        "<condition attribute='atos_ofertaid' operator='eq' value='" + ofertaid + "' />" +
        "</filter>" +
        "</entity> " +
        "</fetch>";

/*223231*///Xrm.WebApi.retrieveMultipleRecords("atos_oferta", "?$filter= atos_ofertaid eq '" + ofertaid + "'&$expand=atos_instalacionid,atos_instalaciongasid").then(
        // var registros = XrmServiceToolkit.Soap.Fetch(fetchXml);
        //Xrm.WebApi.retrieveMultipleRecords("atos_oferta", "?fetchXml=" + fetchXml).then(

    debugger;
    try {                                                                                   /*223231*/
        var statement = "?fetchXml=" + encodeURIComponent(fetchXml);                        /*223231*/
        Xrm.WebApi.online.retrieveMultipleRecords("atos_oferta", statement).then(           /*223231*/   
        function success(registros) {
 
            if (registros != null && registros.entities != null && registros.entities.length > 0) {
                //Carga de la parte comun de contrato indiferentemenete sea una oferta con instalacion power o gas
                oferta = registros.entities[0];
                if (oferta.atos_name != null) {
                    var valorReferencia = new Array();
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = ofertaid; //oferta.atos_instalacionid"].id; 
                    valorReferencia[0].name = oferta.atos_name;
                    valorReferencia[0].entityType = "atos_oferta";
                    formContext.getAttribute("atos_ofertaid").setValue(valorReferencia);
                    formContext.getAttribute("atos_ofertaid").setSubmitMode("always");
                }
                //Número de clics
                if (oferta.numclicsmensual != null) {
                    formContext.getAttribute("atos_numclicsmensual").setValue(oferta.numclicsmensual);
                    formContext.getAttribute("atos_numclicsmensual").setSubmitMode("always");
                }

                if (oferta.numclicstrimestral) {
                    formContext.getAttribute("atos_numclicstrimestral").setValue(oferta.numclicstrimestral);
                    formContext.getAttribute("atos_numclicstrimestral").setSubmitMode("always");
                }

                if (oferta.numclicsanual != null) {
                    formContext.getAttribute("atos_numclicsanual").setValue(oferta.numclicsanual);
                    formContext.getAttribute("atos_numclicsanual").setSubmitMode("always");
                }
                //Fin carga Número de clics

                //Condiciones de Pago y Datos Bancarios
                if (oferta.formadepago != null) {
                    formContext.getAttribute("atos_formadepago").setValue(oferta.formadepago);
                    formContext.getAttribute("atos_formadepago").setSubmitMode("always");
                }

                // Condición de Pago. Tratamiento de Guid desde Fecth
                if (oferta._atos_condicionpagoid_value != null) {
                    var valorReferencia = new Array();
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = oferta._atos_condicionpagoid_value;
                    valorReferencia[0].name = oferta["_atos_condicionpagoid_value@OData.Community.Display.V1.FormattedValue"];
                    // valorReferencia[0].entityType = "atos_condiciondepago"; Esto tambien vale, pero mejor sale procedente del campo.
                    valorReferencia[0].entityType = oferta["_atos_condicionpagoid_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
                    formContext.getAttribute("atos_condicionpagoid").setValue(valorReferencia);
                    formContext.getAttribute("atos_condicionpagoid").setSubmitMode("always");
                }

                if (oferta.tipodeenvio != null) {
                    formContext.getAttribute("atos_tipodeenvio").setValue(oferta.tipodeenvio);
                    formContext.getAttribute("atos_tipodeenvio").setSubmitMode("always");
                }

                if (oferta.plazoenviofacturas != null) {
                    formContext.getAttribute("atos_plazoenviofacturas").setValue(oferta.plazoenviofacturas);
                    formContext.getAttribute("atos_plazoenviofacturas").setSubmitMode("always");
                }

                if (oferta.mandatosepa != null) {
                    formContext.getAttribute("atos_mandatosepa").setValue(oferta.mandatosepa);
                    formContext.getAttribute("atos_mandatosepa").setSubmitMode("always");
                }

                if (oferta.swift != null) {
                    formContext.getAttribute("atos_swift").setValue(oferta.swift);
                    formContext.getAttribute("atos_swift").setSubmitMode("always");
                }

                if (oferta.iban != null) {
                    formContext.getAttribute("atos_iban").setValue(oferta.iban);
                    formContext.getAttribute("atos_iban").setSubmitMode("always");
                }

                if (oferta.entidadbancaria != null) {
                    formContext.getAttribute("atos_entidadbancaria").setValue(oferta.entidadbancaria);
                    formContext.getAttribute("atos_entidadbancaria").setSubmitMode("always");
                }

                if (oferta.sucursalbancaria != null) {
                    formContext.getAttribute("atos_sucursalbancaria").setValue(oferta.sucursalbancaria);
                    formContext.getAttribute("atos_sucursalbancaria").setSubmitMode("always");
                }

                if (oferta.digitocontrol != null) {
                    formContext.getAttribute("atos_digitocontrol").setValue(oferta.digitocontrol);
                    formContext.getAttribute("atos_digitocontrol").setSubmitMode("always");
                }

                if (oferta.cuenta != null) {
                    formContext.getAttribute("atos_cuenta").setValue(oferta.cuenta);
                    formContext.getAttribute("atos_cuenta").setSubmitMode("always");
                }

                if (oferta.cuentabancaria != null) {
                    formContext.getAttribute("atos_cuentabancaria").setValue(oferta.cuentabancaria);
                    formContext.getAttribute("atos_cuentabancaria").setSubmitMode("always");
                }

                if (oferta.cuentabancariaapropia != null) {
                    formContext.getAttribute("atos_cuentabancariaapropia").setValue(oferta.cuentabancariaapropia);
                    formContext.getAttribute("atos_cuentabancariaapropia").setSubmitMode("always");
                }
                //FIN Carga condiciones de pago y datos bancarios

                //FECHAS
                //Fechas: Fecha Inicio Contrato, Fin Contrato, Inicio Efectica, Fin Definitiva
                if (oferta.atos_fechainicio != null) {
                    formContext.getAttribute("atos_fechainiciocontrato").setValue(new Date(oferta.atos_fechainicio.substr(0, 10)));
                    formContext.getAttribute("atos_fechainiciocontrato").setSubmitMode("always");
                    formContext.getAttribute("atos_fechainiciovigenciaprecios").setValue(new Date(oferta.atos_fechainicio.substr(0, 10)));
                    formContext.getAttribute("atos_fechainiciovigenciaprecios").setSubmitMode("always");
                }

                if (oferta.atos_fechafin != null) {
                    formContext.getAttribute("atos_fechafincontrato").setValue(new Date(oferta.atos_fechafin.substr(0, 10)));
                    formContext.getAttribute("atos_fechafincontrato").setSubmitMode("always");
                }

                if (oferta.atos_fechainicio != null) {
                    formContext.getAttribute("atos_fechainicioefectiva").setValue(new Date(oferta.atos_fechainicio.substr(0, 10)));
                    formContext.getAttribute("atos_fechainicioefectiva").setSubmitMode("always");
                }

                if (oferta.atos_fechafin != null) {
                    formContext.getAttribute("atos_fechafindefinitiva").setValue(new Date(oferta.atos_fechafin.substr(0, 10)));
                    formContext.getAttribute("atos_fechafindefinitiva").setSubmitMode("always");
                }
                //Fin Fecha Inicio Contrato, Fin Contrato, Inicio Efectica, Fin Definitiva
                //FIN FECHAS

                //INSTALACION DE LAS CUENTAS
                if (oferta._atos_cuentanegociadoraid_value != null) {
                    var valorReferencia = new Array();
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = oferta._atos_cuentanegociadoraid_value;
                    valorReferencia[0].name = oferta["_atos_cuentanegociadoraid_value@OData.Community.Display.V1.FormattedValue"];
                    valorReferencia[0].entityType = "atos_cuentanegociadora";
                    formContext.getAttribute("atos_cuentanegociadoraid").setValue(valorReferencia);
                    formContext.getAttribute("atos_cuentanegociadoraid").setSubmitMode("always");
                }

                if (oferta._atos_razonsocialid_value != null) {
                    var valorReferencia = new Array();
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = oferta._atos_razonsocialid_value;
                    valorReferencia[0].name = oferta["_atos_razonsocialid_value@OData.Community.Display.V1.FormattedValue"];
                    valorReferencia[0].entityType = "account";
                    formContext.getAttribute("atos_razonsocialid").setValue(valorReferencia);
                    formContext.getAttribute("atos_razonsocialid").setSubmitMode("always");
                    cargarEmpresaDesdeRS(formContext, oferta._atos_razonsocialid_value);
                }

                if (oferta._atos_instalacionid_value) {
                    var valorReferencia = new Array();
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = oferta._atos_instalacionid_value;
                    valorReferencia[0].name = oferta["_atos_instalacionid_value@OData.Community.Display.V1.FormattedValue"];
                    valorReferencia[0].entityType = "atos_instalacion";
                    formContext.getAttribute("atos_instalacionid").setValue(valorReferencia);
                    formContext.getAttribute("atos_instalacionid").setSubmitMode("always");

                    cargaInstalacion(formContext, oferta._atos_instalacionid_value);
                    //formContext.getControl("atos_instalacionid").setVisible(true);
                }

                if (oferta._atos_instalaciongasid_value != null) {
                    var valorReferencia = new Array();
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = oferta._atos_instalaciongasid_value;
                    valorReferencia[0].name = oferta["_atos_instalaciongasid_value@OData.Community.Display.V1.FormattedValue"];
                    valorReferencia[0].entityType = "atos_instalaciongas";
                    formContext.getAttribute("atos_instalaciongasid").setValue(valorReferencia);
                    formContext.getAttribute("atos_instalaciongasid").setSubmitMode("always");

                    //formContext.getControl("atos_instalaciongasid").setVisible(true);
                }

                if (oferta.atos_numerodocumento != null) {
                    formContext.getAttribute("atos_numerodocumento").setValue(oferta.atos_numerodocumento);
                    formContext.getAttribute("atos_numerodocumento").setSubmitMode("always");
                }

                if(oferta.atos_instalacionid != null)
                {
                if (oferta.atos_instalacionid.atos_cups20 != null) {
                    formContext.getAttribute("atos_cups").setValue(oferta.atos_instalacionid.atos_cups20);
                    formContext.getAttribute("atos_cups").setSubmitMode("always");
                }
            }
                if(oferta.atos_instalaciongasid != null)
                {
                    if (oferta.atos_instalaciongasid.atos_cups20 != null) {
                        formContext.getAttribute("atos_cups").setValue(oferta.atos_instalaciongasid.atos_cups20);
                        formContext.getAttribute("atos_cups").setSubmitMode("always");
                    }
                }
                

                if (oferta._atos_tarifaid_value != null && formContext.getControl("atos_tarifaid").getVisible()) {
                    var valorReferencia = new Array();
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = oferta._atos_tarifaid_value;
                    valorReferencia[0].name = oferta["_atos_tarifaid_value@OData.Community.Display.V1.FormattedValue"];
                    valorReferencia[0].entityType = "atos_tarifa";
                    formContext.getAttribute("atos_tarifaid").setValue(valorReferencia);
                    formContext.getAttribute("atos_tarifaid").setSubmitMode("always");
                }

                if (oferta._atos_sistemaelectricoid_value != null && formContext.getControl("atos_sistemaelectricoid").getVisible()) {
                    var valorReferencia = new Array();
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = oferta._atos_sistemaelectricoid_value;
                    valorReferencia[0].name = oferta["_atos_sistemaelectricoid_value@OData.Community.Display.V1.FormattedValue"];
                    valorReferencia[0].entityType = "atos_sistemaelectrico";
                    formContext.getAttribute("atos_sistemaelectricoid").setValue(valorReferencia);
                    formContext.getAttribute("atos_sistemaelectricoid").setSubmitMode("always");
                }

                if (oferta._atos_subsistemaid_value != null && formContext.getControl("atos_subsistemaid").getVisible()) {
                    var valorReferencia = new Array();
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = oferta._atos_subsistemaid_value;
                    valorReferencia[0].name = oferta["_atos_subsistemaid_value@OData.Community.Display.V1.FormattedValue"];
                    valorReferencia[0].entityType = "atos_subsistema";
                    formContext.getAttribute("atos_subsistemaid").setValue(valorReferencia);
                    formContext.getAttribute("atos_subsistemaid").setSubmitMode("always");
                }

                if (oferta._atos_peajeid_value != null && formContext.getControl("atos_peajeid").getVisible()) {
                    var valorReferencia = new Array();
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = oferta._atos_peajeid_value;
                    valorReferencia[0].name = oferta["_atos_peajeid_value@OData.Community.Display.V1.FormattedValue"];
                    valorReferencia[0].entityType = "atos_tablasatrgas";
                    formContext.getAttribute("atos_peajeid").setValue(valorReferencia);
                    formContext.getAttribute("atos_peajeid").setSubmitMode("always");
                }

                if (oferta._atos_usodelgasid_value != null && formContext.getControl("atos_usodelgasid").getVisible()) {
                    var valorReferencia = new Array();
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = oferta._atos_usodelgasid_value;
                    valorReferencia[0].name = oferta["_atos_usodelgasid_value@OData.Community.Display.V1.FormattedValue"];
                    valorReferencia[0].entityType = "atos_tablasatrgas";
                    formContext.getAttribute("atos_usodelgasid").setValue(valorReferencia);
                    formContext.getAttribute("atos_usodelgasid").setSubmitMode("always");
                }

                if (oferta.atos_fechainiciovigenciapeaje != null && formContext.getControl("atos_fechainiciovigenciapeaje").getVisible()) { 
                    formContext.getAttribute("atos_fechainiciovigenciapeaje").setValue(new Date(oferta.atos_fechainiciovigenciapeaje.substr(0, 10)));
                    formContext.getAttribute("atos_fechainiciovigenciapeaje").setSubmitMode("always");
                }
                //Fin Carga INFORMACION DE LAS CUENTAS

                //INFORMACION COMERCIAL
                // contacto comercial
                if (oferta._atos_contactocomercialid_value != null || oferta._contactocomercialgasid_value != null) {
                    var valorReferencia = new Array();
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = (oferta._atos_contactocomercialid_value != null) ? oferta._atos_contactocomercialid_value : oferta._contactocomercialgasid_value;
                    valorReferencia[0].name = (oferta._atos_contactocomercialid_value != null) ? oferta["_atos_contactocomercialid_value@OData.Community.Display.V1.FormattedValue"] : oferta["_contactocomercialgasid_value@OData.Community.Display.V1.FormattedValue"];
                    valorReferencia[0].entityType = "contact";
                    formContext.getAttribute("atos_contactocomercialid").setValue(valorReferencia);
                    formContext.getAttribute("atos_contactocomercialid").setSubmitMode("always");
                }

                //canal
                if (oferta.atos_canal != null || oferta.atos_canalgas != null) {
                    formContext.getAttribute("atos_canal").setValue((oferta.atos_canal != null ? oferta.atos_canal : oferta.atos_canalgas));
                    formContext.getAttribute("atos_canal").setSubmitMode("always");
                }

                //Consultora
                if (oferta._atos_consultoraidgas_value != null) {
                    var valorRefConsultora = new Array();
                    valorRefConsultora[0] = new Object();
                    valorRefConsultora[0].id = oferta._atos_consultoraidgas_value;
                    valorRefConsultora[0].name = oferta["_atos_consultoraidgas_value@OData.Community.Display.V1.FormattedValue"];
                    valorRefConsultora[0].entityType = "atos_consultora";
                    formContext.getAttribute("atos_consultoraid").setValue(valorRefConsultora);
                    formContext.getAttribute("atos_consultoraid").setSubmitMode("always");
                }

                //Agente Comercial
                if (oferta._atos_agentecomercialid_value != null || oferta._atos_agentecomercialidgas_value != null) {
                    var valorReferencia = new Array();
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = oferta._atos_agentecomercialid_value != null ? oferta._atos_agentecomercialid_value : oferta._atos_agentecomercialidgas_value;
                    valorReferencia[0].name = oferta._atos_agentecomercialid_value != null ? oferta["_atos_agentecomercialid_value@OData.Community.Display.V1.FormattedValue"] : oferta["_atos_agentecomercialidgas_value@OData.Community.Display.V1.FormattedValue"];
                    valorReferencia[0].entityType = "atos_agentecomercial";
                    formContext.getAttribute("atos_agentecomercialid").setValue(valorReferencia);
                    formContext.getAttribute("atos_agentecomercialid").setSubmitMode("always");
                }

                //Fecha asignación agente
                if (oferta.atos_fechaasignacionagente != null || oferta.atos_fechaasignacionagentegas != null) {
                    formContext.getAttribute("atos_fechaasignacionagente").setValue((oferta.atos_fechaasignacionagente != null ? new Date(oferta.atos_fechaasignacionagente.substr(0, 10)) : new Date(oferta.atos_fechaasignacionagentegas.substr(0, 10))));
                    formContext.getAttribute("atos_fechaasignacionagente").setSubmitMode("always");
                }

                //Agente de Facturacion
                if (oferta._atos_agentefacturacionid_value != null || oferta._atos_agentefacturacionidgas_value != null) {
                    var valorReferencia = new Array();
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = oferta._atos_agentefacturacionid_value != null ? oferta._atos_agentefacturacionid_value : oferta._atos_agentefacturacionidgas_value;
                    valorReferencia[0].name = oferta._atos_agentefacturacionid_value != null ? oferta["_atos_agentefacturacionid_value@OData.Community.Display.V1.FormattedValue"] : oferta["_atos_agentefacturacionidgas_value@OData.Community.Display.V1.FormattedValue"];
                    valorReferencia[0].entityType = "systemuser";
                    formContext.getAttribute("atos_agentefacturacionid").setValue(valorReferencia);
                    formContext.getAttribute("atos_agentefacturacionid").setSubmitMode("always");
                }

                //Fecha asignación agente facturación
                if (oferta.atos_fechaasignacionagentefacturacion != null || oferta.atos_fechaasignacionagentefacturaciongas != null) {
                    formContext.getAttribute("atos_fechaasignacionagentefacturacion").setValue((oferta.atos_fechaasignacionagentefacturacion != null ? new Date(oferta.atos_fechaasignacionagentefacturacion.substr(0, 10)) : new Date(oferta.atos_fechaasignacionagentefacturaciongas.substr(0, 10))));
                    formContext.getAttribute("atos_fechaasignacionagentefacturacion").setSubmitMode("always");
                }
                //FIN INFORMACION COMERCIAL

                //INFORMACION FINANCIERA
                //Tipo Garantía solicitada al cliente
                if (oferta.atos_tipogarantiasolicitadacliente != null) {
                    formContext.getAttribute("atos_tipogarantiasolicitadacliente").setValue(oferta.atos_tipogarantiasolicitadacliente);
                    formContext.getAttribute("atos_tipogarantiasolicitadacliente").setSubmitMode("always");
                }

                //Importe Garantía Solicitada
                if (oferta.atos_importegarantiasolicitada != null) {
                    formContext.getAttribute("atos_importegarantiasolicitada").setValue(oferta.atos_importegarantiasolicitada);
                    formContext.getAttribute("atos_importegarantiasolicitada").setSubmitMode("always");
                }

                //Fecha solicitud Garantía cliente
                if (oferta.atos_fecharealizaciongarantiacliente != null) {
                    formContext.getAttribute("atos_fecharealizaciongarantiacliente").setValue(new Date(oferta.atos_fecharealizaciongarantiacliente.substr(0, 10)));
                    formContext.getAttribute("atos_fecharealizaciongarantiacliente").setSubmitMode("always");
                }

                //Fecha Inicio Garantía
                if (oferta.atos_fechainiciogarantia != null) {
                    formContext.getAttribute("atos_fechainiciogarantia").setValue(new Date(oferta.atos_fechainiciogarantia.substr(0, 10)));
                    formContext.getAttribute("atos_fechainiciogarantia").setSubmitMode("always");
                }

                //Fecha fin Garantía
                if (oferta.atos_fechafingarantia != null) {
                    formContext.getAttribute("atos_fechafingarantia").setValue(new Date(oferta.atos_fechafingarantia.substr(0, 10)));
                    formContext.getAttribute("atos_fechafingarantia").setSubmitMode("always");
                }

                //Importe Aval definitivo solicitado
                if (oferta.atos_importeavalprovisionalsolicitado != null) {
                    formContext.getAttribute("atos_importeavaldefinitivosolicitado").setValue(oferta.atos_importeavalprovisionalsolicitado);
                    formContext.getAttribute("atos_importeavaldefinitivosolicitado").setSubmitMode("always");
                }

                //Fecha solicitud Aval definitivo
                if (oferta.atos_fechasolicitudavalprovisional != null) {
                    formContext.getAttribute("atos_fechasolicitudavaldefinitivo").setValue(new Date(oferta.atos_fechasolicitudavalprovisional.substr(0, 10)));
                    formContext.getAttribute("atos_fechasolicitudavaldefinitivo").setSubmitMode("always");
                }

                //Fecha realización Aval definitivo
                if (oferta.atos_fecharealizacionavalprovisional != null) {
                    formContext.getAttribute("atos_fecharealizacionavaldefinitivo").setValue(new Date(oferta.atos_fecharealizacionavalprovisional.substr(0, 10)));
                    formContext.getAttribute("atos_fecharealizacionavaldefinitivo").setSubmitMode("always");
                }

                //Fecha inicio Aval definitivo
                if (oferta.atos_fechainicioavalprovisional != null) {
                    formContext.getAttribute("atos_fechainicioavaldefinitivo").setValue(new Date(oferta.atos_fechainicioavalprovisional.substr(0, 10)));
                    formContext.getAttribute("atos_fechainicioavaldefinitivo").setSubmitMode("always");
                }

                //Fecha fin Aval definitivo
                if (oferta.atos_fechafinavalprovisional != null) {
                    formContext.getAttribute("atos_fechafinavaldefinitivo").setValue(new Date(oferta.atos_fechafinavalprovisional.substr(0, 10)));
                    formContext.getAttribute("atos_fechafinavaldefinitivo").setSubmitMode("always");
                }

                //Comentarios
                if (oferta.atos_comentarios != null) {
                    formContext.getAttribute("atos_comentarios").setValue(oferta.atos_comentarios);
                    formContext.getAttribute("atos_comentarios").setSubmitMode("always");
                }
                //FIN INFORMACION FINANCIERA

                if (oferta.atos_lote != null) {
                    formContext.getAttribute("atos_lote").setValue(oferta.atos_lote);
                    formContext.getAttribute("atos_lote").setSubmitMode("always");
                }

                if (oferta.atos_importebonificacion != null) {
                    formContext.getAttribute("atos_importebonificacion").setValue(oferta.atos_importebonificacion);
                    formContext.getAttribute("atos_importebonificacion").setSubmitMode("always");
                }

                if (oferta.atos_fechasolicitudgarantiacliente != null) {
                    formContext.getAttribute("atos_fechasolicitudgarantiacliente").setValue(new Date(oferta.atos_fechasolicitudgarantiacliente.substr(0, 10)));
                    formContext.getAttribute("atos_fechasolicitudgarantiacliente").setSubmitMode("always");
                }

                if (oferta.atos_gestionatr != null) {
                    if (oferta.atos_gestionatr == true)
                        formContext.getAttribute("atos_atrincluidoenelcontrato").setValue(300000001);
                    else
                        formContext.getAttribute("atos_atrincluidoenelcontrato").setValue(300000000);
                    formContext.getAttribute("atos_atrincluidoenelcontrato").setSubmitMode("always");
                }

                if (oferta.atos_penalizacionconsumo != null) {
                    formContext.getAttribute("atos_penalizacionconsumo").setValue(oferta.atos_penalizacionconsumo);
                    formContext.getAttribute("atos_penalizacionconsumo").setSubmitMode("always");
                }

                if (oferta.atos_importepenalizacion != null) {
                    formContext.getAttribute("atos_importepenalizacion").setValue(oferta.atos_importepenalizacion);
                    formContext.getAttribute("atos_importepenalizacion").setSubmitMode("always");
                }

                if (oferta.atos_rangoinferiorpenalizacion != null) {
                    formContext.getAttribute("atos_rangoinferiorpenalizacion").setValue(oferta.atos_rangoinferiorpenalizacion);
                    formContext.getAttribute("atos_rangoinferiorpenalizacion").setSubmitMode("always");
                }

                if (oferta.atos_rangosuperiorpenalizacion != null) {
                    formContext.getAttribute("atos_rangosuperiorpenalizacion").setValue(oferta.atos_rangosuperiorpenalizacion);
                    formContext.getAttribute("atos_rangosuperiorpenalizacion").setSubmitMode("always");
                }

                if (oferta.atos_bonificacionconsumo != null) {
                    formContext.getAttribute("atos_bonificacionconsumo").setValue(oferta.atos_bonificacionconsumo);
                    formContext.getAttribute("atos_bonificacionconsumo").setSubmitMode("always");
                }

                if (oferta.atos_rangoinferiorbonificacion != null) {
                    formContext.getAttribute("atos_rangoinferiorbonificacion").setValue(oferta.atos_rangoinferiorbonificacion);
                    formContext.getAttribute("atos_rangoinferiorbonificacion").setSubmitMode("always");
                }

                if (oferta.atos_rangosuperiorbonificacion != null) {
                    formContext.getAttribute("atos_rangosuperiorbonificacion").setValue(oferta.atos_rangosuperiorbonificacion);
                    formContext.getAttribute("atos_rangosuperiorbonificacion").setSubmitMode("always");
                }

                if (oferta.atos_fechaaceptacionoferta != null) {
                    formContext.getAttribute("atos_fechaaceptacionoferta").setValue(new Date(oferta.atos_fechaaceptacionoferta.substr(0, 10)));
                    formContext.getAttribute("atos_fechaaceptacionoferta").setSubmitMode("always");

                }

                //Información Producto
                //Tipo de producto
                if (oferta._atos_tipodeproductofinalid_value != null) {
                    var valorReferencia = new Array();
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = oferta._atos_tipodeproductofinalid_value;
                    valorReferencia[0].name = oferta["_atos_tipodeproductofinalid_value@OData.Community.Display.V1.FormattedValue"] ;
                    valorReferencia[0].entityType = "atos_tipodeproducto";
                    formContext.getAttribute("atos_tipodeproductoid").setValue(valorReferencia);
                    formContext.getAttribute("atos_tipodeproductoid").setSubmitMode("always");
                }

                //Precio incluye gestión ATR
                if (oferta.atos_precioincluyegestionatr != null && formContext.getControl("atos_precioincluyegestionatr").getVisible()) {
                    formContext.getAttribute("atos_precioincluyegestionatr").setValue(oferta.atos_precioincluyegestionatr);
                    formContext.getAttribute("atos_precioincluyegestionatr").setSubmitMode("always");
                }

                //Precio incluye impuesto eléctrico
                if (oferta.atos_precioincluyeimpuestoelectrico != null && formContext.getControl("atos_precioincluyeimpuestoelectrico").getVisible()) {
                    formContext.getAttribute("atos_precioincluyeimpuestoelectrico").setValue(oferta.atos_precioincluyeimpuestoelectrico);
                    formContext.getAttribute("atos_precioincluyeimpuestoelectrico").setSubmitMode("always");
                }

                //Beneficio estimado (€)
                if (oferta.atos_beneficioestimadooferta != null) {
                    formContext.getAttribute("atos_beneficioestimado").setValue(oferta.atos_beneficioestimadooferta);
                    formContext.getAttribute("atos_beneficioestimado").setSubmitMode("always");
                }
                //FIN Información Producto

                //INFORMACION DE LA OFERTA
                // impuesto eléctrico
                //if (oferta.atos_impuestoelectrico != null && formContext.getControl("atos_impuestoelectrico").getVisible()) {
                //    formContext.getAttribute("atos_impuestoelectrico").setValue(atos_impuestoelectrico);
                //    formContext.getAttribute("atos_impuestoelectrico").setSubmitMode("always");
                //}

                // renovacion tácita
                if (oferta.atos_renovaciontacita != null) {
                    formContext.getAttribute("atos_renovaciontacita").setValue(oferta.atos_renovaciontacita);
                    formContext.getAttribute("atos_renovaciontacita").setSubmitMode("always");
                }

                //coste gestión cierre
                if (oferta.atos_costegestioncierresmensuales != null) {
                    formContext.getAttribute("atos_costegestioncierresmensuales").setValue(oferta.atos_costegestioncierresmensuales);
                    formContext.getAttribute("atos_costegestioncierresmensuales").setSubmitMode("always");
                }

                if (oferta.atos_costegestioncierrestrimestrales != null) {
                    formContext.getAttribute("atos_costegestioncierrestrimestrales").setValue(oferta.atos_costegestioncierrestrimestrales);
                    formContext.getAttribute("atos_costegestioncierrestrimestrales").setSubmitMode("always");
                }

                if (oferta.atos_costegestioncierresanuales != null) {
                    formContext.getAttribute("atos_costegestioncierresanuales").setValue(oferta.atos_costegestioncierresanuales);
                    formContext.getAttribute("atos_costegestioncierresanuales").setSubmitMode("always");
                }

                //PUNTOS DE MEDIDA
                // tipo propiedad aparato
                if (oferta._atos_tipopropiedadaparatoid_value != null) {
                    var valorReferencia = new Array();
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = oferta._atos_tipopropiedadaparatoid_value;
                    valorReferencia[0].name = oferta["_atos_tipopropiedadaparatoid_value@OData.Community.Display.V1.FormattedValue"];
                    valorReferencia[0].entityType = "atos_tipopropiedadaparato";
                    formContext.getAttribute("atos_tipopropiedadaparatoid").setValue(valorReferencia);
                    formContext.getAttribute("atos_tipopropiedadaparatoid").setSubmitMode("always");
                }

                // importe alquiler
                if (oferta.atos_importealquilerequipo != null) {
                    formContext.getAttribute("atos_importealquilerequipo").setValue(oferta.atos_importealquilerequipo);
                    formContext.getAttribute("atos_importealquilerequipo").setSubmitMode("always");
                }
                // modo de lectura punto de medida
                if (oferta._atos_modolecturapuntomedidaid_value != null) {

                    var valorReferencia = new Array();
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = oferta._atos_modolecturapuntomedidaid_value;
                    valorReferencia[0].name = oferta["_atos_modolecturapuntomedidaid_value@OData.Community.Display.V1.FormattedValue"];
                    valorReferencia[0].entityType = "atos_modolecturapuntomedida";
                    formContext.getAttribute("atos_modolecturapuntomedidaid").setValue(valorReferencia);
                    formContext.getAttribute("atos_modolecturapuntomedidaid").setSubmitMode("always");
                }

                if (oferta._atos_instalacionccaaid_value != null && oferta["_atos_instalacionccaaid_value@OData.Community.Display.V1.FormattedValue"] == "Canarias") {
                    formContext.getAttribute("atos_aplicaimpuesto").setValue(300000002);
                    formContext.getAttribute("atos_aplicaimpuesto").setSubmitMode("always");

                } else {
                    formContext.getAttribute("atos_aplicaimpuesto").setValue(300000000);
                    formContext.getAttribute("atos_aplicaimpuesto").setSubmitMode("always");
                }

                //Carga parte concreta de contrato si la oferta tiene una instalacion power o gas
                if (oferta.atos_commodity != null) {
                    formContext.getAttribute("atos_commodity").setValue(oferta.atos_commodity);
                    formContext.getAttribute("atos_commodity").setSubmitMode("always");
                }


                atos_precisiondecimalesoferta

                //precision de definales de la oferta
                if (oferta.atos_precisiondecimalesoferta != null) {
                    formContext.getAttribute("atos_precisiondecimalesoferta").setValue(oferta.atos_precisiondecimalesoferta);
                    formContext.getAttribute("atos_commodity").setSubmitMode("always");
                }
            }
 
/*223231*///}, function (error) {
/*223231*///Xrm.Navigation.openErrorDialog({ detail: error.message });
        });
    }                                                                                       /*223231*/
    catch (error) {                                                                         /*223231*/
        console.log("Error cargaDesdeOferta: (" + error.description + ")");                 /*223231*/
    }                                                                                       /*223231*/
}



function cargarEmpresaDesdeRS(formContext, razonsocialid) {

    var fetchXml =
/*223231*///"<fetch mapping='logical'>" +
        "<fetch mapping='logical' no-lock='true'>" + 
        "<entity name='account' > " +
        "<attribute name='atos_grupoempresarialid' /> " +
        "<filter>" +
        "<condition attribute='accountid' operator='eq' value='" + razonsocialid + "' />" +
        "</filter>" +
        "</entity> " +
        "</fetch>";

    // var registros = XrmServiceToolkit.Soap.Fetch(fetchXml);
    /*223231*///Xrm.WebApi.retrieveMultipleRecords("account", "?fetchXml=" + fetchXml).then(
    debugger;
    try {                                                                                   /*223231*/        
        var statement = "?fetchXml=" + encodeURIComponent(fetchXml);                        /*223231*/
        Xrm.WebApi.online.retrieveMultipleRecords("account", statement).then(               /*223231*/
        function success(registros) {
            if (registros != null && registros.entities != null && registros.entities.length > 0) {
                var cuenta = registros.entities[0];

                if (cuenta.atos_grupoempresarialid != null) {
                    var valorReferencia = new Array();
                    valorReferencia[0] = new Object();
                    valorReferencia[0].id = cuenta.atos_grupoempresarialid;
                    valorReferencia[0].name = cuenta.name;
                    valorReferencia[0].entityType = "atos_grupoempresarial";
                    formContext.getAttribute("atos_grupoempresarialid").setValue(valorReferencia);
                    formContext.getAttribute("atos_grupoempresarialid").setSubmitMode("always");
                }
        }
/*223231*///}, function (error) {
/*223231*///    Xrm.Navigation.openErrorDialog({ detail: error.message });
        });
    }                                                                                       /*223231*/
    catch (error) {                                                                         /*223231*/
        console.log("Error cargarEmpresaDesdeRS: (" + error.description + ")");             /*223231*/
    }                                                                                       /*223231*/    
}

/*
 * Function from FormaPago event
 * Function from DiasVecimientoFactura event
 * Function from SWIFT event
 * Function from IBAM event
 * Function from Entidad event
 * Function from Sucursal event
 * Function from DigitoControl event
 * Function from Cuenta event
 * @param {formContext} Contexto de ejecucion del formulario
 */
function borrarMandatoSepa(executionContext) {

    var formContext = executionContext.getFormContext();

    formContext.getAttribute("atos_mandatosepa").setValue("");
    formContext.getAttribute("atos_mandatosepa").setSubmitMode("always");
}


/**
// <summary>
// Oculta los campos segun la commodity
// </summary>
// <remarks>
// Si la
// </remarks>
*/
function mostrarcampos(executionContext) {

    var formContext = executionContext.getFormContext();

    if (formContext.getAttribute("atos_commodity").getValue() == 300000000) {
        formContext.getControl("atos_peajeid").setVisible(false);
        formContext.getControl("atos_usodelgasid").setVisible(false);
        formContext.getControl("atos_fechainiciovigenciapeaje").setVisible(false);
        formContext.getControl("atos_instalaciongasid").setVisible(false);

        formContext.getControl("atos_penalizacionporcentaje").setVisible(false);
        formContext.getAttribute("atos_penalizacionporcentaje").setRequiredLevel("none");
        formContext.getControl("atos_penalizacionporcentaje").setDisabled(true);

        formContext.getControl("atos_referenciapenalizacion").setVisible(false);
        formContext.getAttribute("atos_referenciapenalizacion").setRequiredLevel("none");
        formContext.getControl("atos_referenciapenalizacion").setDisabled(true);

        formContext.getControl("atos_tipodepenalizacionid").setVisible(false);
        formContext.getAttribute("atos_tipodepenalizacionid").setRequiredLevel("none");
        formContext.getControl("atos_tipodepenalizacionid").setDisabled(true);

    } else if (formContext.getAttribute("atos_commodity").getValue() == 300000001) {
        formContext.getControl("atos_matrizhorariaid").setVisible(false);
        formContext.getControl("atos_precioincluyegestionatr").setVisible(false);
        formContext.getControl("atos_precioincluyeimpuestoelectrico").setVisible(false);
        formContext.getControl("atos_tarifaid").setVisible(false);
        formContext.getControl("atos_sistemaelectricoid").setVisible(false);
        formContext.getControl("atos_subsistemaid").setVisible(false);
        formContext.getControl("atos_tipoliquidacion").setVisible(false);
        formContext.getControl("atos_medidashorarias").setVisible(false);
        formContext.getControl("atos_impuestoelectrico").setVisible(false);
        formContext.getControl("atos_tipoexencionie").setVisible(false);
        formContext.getControl("atos_cie").setVisible(false);
        formContext.getControl("atos_baseimponibleexencion").setVisible(false);
        formContext.getControl("atos_porcentajeexencion").setVisible(false);
        formContext.getControl("atos_fechainicioexencionie").setVisible(false);
        formContext.getControl("atos_fechafinexencionie").setVisible(false);
        formContext.getControl("atos_aplicacoeficienteperdidas").setVisible(false);
        formContext.getControl("atos_fechainiciocoeficienteperdidas").setVisible(false);
        formContext.getControl("atos_interrumpibilidadincluida").setVisible(false);
        formContext.getControl("atos_fechainiciointerrumpibilidad").setVisible(false);
        formContext.getControl("atos_instalacionid").setVisible(false);
    }
    else {
    }
}

/*
 * Function from OnLoad event
 * @param {executionContext} Contexto de ejecucion del formulario
 */
function WarningEMS(executionContext) {
    var formContext = executionContext.getFormContext();
/*223231*/ //if (formContext.data.entity.getId() == null || formContext.data.entity.getId() == "") {
    if (IsEmptyId()) {                                      /*223231*/

        if (formContext.getAttribute("atos_interfazcontratoems").getValue() == 300000000) // KO EMS
        {
            formContext.setFormNotification("La última ejecución del WS de Contrato con EMS ha finalizado con errores", "WARNING", "1");
        }
        if (formContext.getAttribute("atos_interfazprecioscontratoems").getValue() == 300000000) // KO EMS
        {
            formContext.setFormNotification("La última ejecución del WS de Precios Contrato con EMS ha finalizado con errores", "WARNING", "2");
        }
    }
}

function CambiarTipoLiquidacion(formContext) {

    var formContext = executionContext.getFormContext();
    
    if (formContext.getAttribute("atos_tipoliquidacion").getValue() != null && formContext.getControl("atos_tipoliquidacion").getVisible()) {
        if (formContext.getAttribute("atos_tipoliquidacion").getValue() == 300000000) {
            formContext.getControl("atos_alquilerequipo").setDisabled(false);
            formContext.getAttribute("atos_alquilerequipo").setValue(300000000);
            formContext.getAttribute("atos_alquilerequipo").setSubmitMode("always");
        }
        if (formContext.getAttribute("atos_tipoliquidacion").getValue() == 300000001) {
            formContext.getAttribute("atos_alquilerequipo").setValue(300000001);
            formContext.getAttribute("atos_alquilerequipo").setSubmitMode("always");
            formContext.getControl("atos_alquilerequipo").setDisabled(true);
        }
    }
}


/*
 * Function from OnSave event
 * @param {executionContext} Contexto de ejecucion del formulario
 * Se ejecuta al guardar los datos. Valida las fechas y la cuenta bancaria (si es domiciliación bancaria)
 */
function Contrato_OnSave(executionContext) {

    var formContext = executionContext.getFormContext();
    
    if (formContext.getAttribute("atos_fechainiciocontrato").getIsDirty() == true ||
        formContext.getAttribute("atos_fechafincontrato").getIsDirty() == true) {
        if (compruebaFechas(executionContext, "atos_fechainiciocontrato", "atos_fechafincontrato", "La fecha de inicio de contrato debe ser menor o igual que la fecha de fin de contrato.") != "")
            if (executionContext.getEventArgs() != null)
                executionContext.getEventArgs().preventDefault();
    }
    if (formContext.getAttribute("atos_fechainicioefectiva").getIsDirty() == true ||
        formContext.getAttribute("atos_fechafindefinitiva").getIsDirty() == true) {
        if (compruebaFechas(executionContext, "atos_fechainicioefectiva", "atos_fechafindefinitiva", "La fecha de inicio efectiva debe ser menor o igual que la fecha de fin definitiva.", "6") != "")
            if (executionContext.getEventArgs() != null)
                executionContext.getEventArgs().preventDefault();
    }
    if (formContext.getAttribute("atos_fechainiciogarantia").getIsDirty() == true ||
        formContext.getAttribute("atos_fechafingarantia").getIsDirty() == true) {
        if (compruebaFechas(executionContext, "atos_fechainiciogarantia", "atos_fechafingarantia", "La fecha de inicio de garantía debe ser menor o igual que la fecha de fin de garantía.", "7") != "")
            if (executionContext.getEventArgs() != null)
                executionContext.getEventArgs().preventDefault();
    }
    if (formContext.getAttribute("atos_fechainicioavaldefinitivo").getIsDirty() == true ||
        formContext.getAttribute("atos_fechafinavaldefinitivo").getIsDirty() == true) {
        if (compruebaFechas(executionContext, "atos_fechainicioavaldefinitivo", "atos_fechafinavaldefinitivo", "La fecha de inicio de aval debe ser menor o igual que la fecha de fin de aval.", "8") != "")
            if (executionContext.getEventArgs() != null)
                objexecutionContextgetEventArgs().preventDefault();
    }

    // Si es domiciliación bancaria y han cambiado datos de la cuenta la validamos.
    if (formContext.getAttribute("atos_formadepago").getValue() == 300000001) // Domiciliación
    {
        if (formContext.getAttribute("atos_entidadbancaria").getIsDirty() == true ||
            formContext.getAttribute("atos_sucursalbancaria").getIsDirty() == true ||
            formContext.getAttribute("atos_digitocontrol").getIsDirty() == true ||
            formContext.getAttribute("atos_cuenta").getIsDirty() == true) {
            if (validaCuenta() == false)
                if (executionContext.getEventArgs() != null)
                    executionContext.getEventArgs().preventDefault();
        }
    }


    // var continuar = true;

    if ((formContext.getAttribute("atos_precioalquilerequipo").getIsDirty() == true ||
        formContext.getAttribute("atos_preciomensualserviciolectura").getIsDirty() == true) &&
        formContext.getAttribute("atos_fechainiciovigenciaprecios").getIsDirty() == false) {

        var message = "Ha modificado el precio de alquiler de equipo o el del servicio de lectura\n" +
                      "sin modificar la fecha de inicio de vigencia de precios,\n¿desea continuar?.";
        //context.navigation.openConfirmDialog(confirmStrings, options)
        Xrm.Navigation.openConfirmDialog(message).then(
        function (success) {
            if (success.confirmed)
                if (executionContext.getEventArgs() != null)
                    executionContext.getEventArgs().preventDefault();
        });
    }
}

/**
// <summary>
// Comprueba si el campo recibido por parámetro está vacío en el formulario
// </summary>
*/
function campoVacio(formContext, campo) {

    try{
        formContext = formContext.getFormContext();
    }
    catch{

    }
    if(formContext.getAttribute(campo) != null)
    {
        if (formContext.getAttribute(campo).getValue() == null ||
        formContext.getAttribute(campo).getValue() == "")
        return true;
    }

    return false;
}

/*
 * Function from OnLoad event
 * Function from FechaInicioContrato event
 * Function from FechaFinContrato event
 * @param {executionContext} Contexto de ejecucion del formulario
 */
function copiaEfecDef(executionContext) {

    var formContext = executionContext.getFormContext();
    if (formContext.getAttribute("atos_fechainiciocontrato").getValue() != null &&
        formContext.getAttribute("atos_fechainicioefectiva").getValue() == null) {
        formContext.getAttribute("atos_fechainicioefectiva").setValue(formContext.getAttribute("atos_fechainiciocontrato").getValue());
    }

    if (formContext.getAttribute("atos_fechafincontrato").getValue() != null &&
        formContext.getAttribute("atos_fechafindefinitiva").getValue() == null) {
        formContext.getAttribute("atos_fechafindefinitiva").setValue(formContext.getAttribute("atos_fechafincontrato").getValue());
    }
}


/*
 * Function from OnLoad event
 * Function from PenalizacionConsumo event
 * @param {executionContext} Contexto de ejecucion del formulario
 */
function penalizacionConsumo(executionContext) {
    debugger;
    var formContext = executionContext.getFormContext();
    if (formContext.getAttribute("atos_penalizacionconsumo").getValue() == true) {

        formContext.getAttribute("atos_importepenalizacion").setRequiredLevel("required");
        formContext.getAttribute("atos_rangoinferiorpenalizacion").setRequiredLevel("required");
        formContext.getAttribute("atos_rangosuperiorpenalizacion").setRequiredLevel("required");
        formContext.getAttribute("atos_tipodepenalizacionid").setRequiredLevel("required");
        formContext.getAttribute("atos_referenciapenalizacion").setRequiredLevel("required");
        formContext.getAttribute("atos_penalizacionporcentaje").setRequiredLevel("required");
        formContext.getControl("atos_importepenalizacion").setDisabled(false);
        formContext.getControl("atos_rangoinferiorpenalizacion").setDisabled(false);
        formContext.getControl("atos_rangosuperiorpenalizacion").setDisabled(false);
        formContext.getControl("atos_tipodepenalizacionid").setDisabled(false);
        formContext.getControl("atos_referenciapenalizacion").setDisabled(false);
        formContext.getControl("atos_penalizacionporcentaje").setDisabled(false);
    }
    else {
        if (campoVacio(formContext,"atos_importepenalizacion") == false) // Si tiene valor lo vacía
        {
            formContext.getAttribute("atos_importepenalizacion").setValue(null);
            formContext.getAttribute("atos_importepenalizacion").setSubmitMode("always");
        }
        if (campoVacio(formContext,"atos_rangoinferiorpenalizacion") == false) // Si tiene valor lo vacía
        {
            formContext.getAttribute("atos_rangoinferiorpenalizacion").setValue(null);
            formContext.getAttribute("atos_rangoinferiorpenalizacion").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_rangosuperiorpenalizacion") == false) // Si tiene valor lo vacía
        {
            formContext.getAttribute("atos_rangosuperiorpenalizacion").setValue(null);
            formContext.getAttribute("atos_rangosuperiorpenalizacion").setSubmitMode("always");
        }

        // Este campo solo existe en el formulario Alpiq pero no en Informacion
        if (campoVacio(formContext, "atos_tipodepenalizacionid") == false) {
            formContext.getAttribute("atos_tipodepenalizacionid").setValue(null);
            formContext.getAttribute("atos_tipodepenalizacionid").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_referenciapenalizacion") == false) {
            formContext.getAttribute("atos_referenciapenalizacion").setValue(null);
            formContext.getAttribute("atos_referenciapenalizacion").setSubmitMode("always");
        }
        formContext.getAttribute("atos_importepenalizacion").setRequiredLevel("none");
        formContext.getAttribute("atos_rangoinferiorpenalizacion").setRequiredLevel("none");
        formContext.getAttribute("atos_rangosuperiorpenalizacion").setRequiredLevel("none");
        formContext.getAttribute("atos_tipodepenalizacionid").setRequiredLevel("none");
        formContext.getAttribute("atos_referenciapenalizacion").setRequiredLevel("none");
        formContext.getControl("atos_importepenalizacion").setDisabled(true);
        formContext.getControl("atos_rangoinferiorpenalizacion").setDisabled(true);
        formContext.getControl("atos_rangosuperiorpenalizacion").setDisabled(true);
        formContext.getControl("atos_tipodepenalizacionid").setDisabled(true);
        formContext.getControl("atos_referenciapenalizacion").setDisabled(true);
    }

    if (formContext.getAttribute("atos_commodity").getValue() == 300000001) {
        referenciaPenalizacion(formContext);
    } else {
        formContext.getAttribute("atos_penalizacionporcentaje").setRequiredLevel("none");
        formContext.getControl("atos_penalizacionporcentaje").setDisabled(true);
        formContext.getAttribute("atos_referenciapenalizacion").setRequiredLevel("none");
        formContext.getControl("atos_referenciapenalizacion").setDisabled(true);
        formContext.getAttribute("atos_tipodepenalizacionid").setRequiredLevel("none");
        formContext.getControl("atos_tipodepenalizacionid").setDisabled(true);
    }
}

/**
// <summary>
// Habilita/Deshabilita los campos importe penalización o penalizacion
// </summary>
// <remarks>
// <ul>
// <li></li>
// <li></li>
// <li></li>
// <li></li>
// <li></li>
// </ul>
// Si el tipo de garantía no es Sin Garantia habilita los campos de garantía<br/>
// Si el tipo de garantía es Sin Garantía, deshabilita y vacía los campos de garantía
// </remarks>
*/
function referenciaPenalizacion(formContext) {

    if (campoVacio(formContext, "atos_referenciapenalizacion") == true) {
        formContext.getAttribute("atos_importepenalizacion").setValue(null);
        formContext.getAttribute("atos_importepenalizacion").setSubmitMode("always");
        formContext.getAttribute("atos_penalizacionporcentaje").setValue(null);
        formContext.getAttribute("atos_penalizacionporcentaje").setSubmitMode("always");
        formContext.getAttribute("atos_importepenalizacion").setRequiredLevel("none");
        formContext.getAttribute("atos_penalizacionporcentaje").setRequiredLevel("none");
        formContext.getControl("atos_penalizacionporcentaje").setDisabled(true);
        formContext.getControl("atos_importepenalizacion").setDisabled(true);
        return;
    }

    if (formContext.getAttribute("atos_referenciapenalizacion").getValue() == 300000000) {
        if (campoVacio(formContext, "atos_penalizacionporcentaje") == false) {
            formContext.getAttribute("atos_penalizacionporcentaje").setValue(null);
            formContext.getAttribute("atos_penalizacionporcentaje").setSubmitMode("always");
        }
        formContext.getAttribute("atos_importepenalizacion").setRequiredLevel("required");
        formContext.getAttribute("atos_penalizacionporcentaje").setRequiredLevel("none");
        formContext.getControl("atos_penalizacionporcentaje").setDisabled(true);
        formContext.getControl("atos_importepenalizacion").setDisabled(false);
    } else {
        if (campoVacio(formContext, "atos_importepenalizacion") == false) {
            formContext.getAttribute("atos_importepenalizacion").setValue(null);
            formContext.getAttribute("atos_importepenalizacion").setSubmitMode("always");
        }
        formContext.getAttribute("atos_importepenalizacion").setRequiredLevel("none");
        formContext.getControl("atos_importepenalizacion").setDisabled(true);
        formContext.getAttribute("atos_penalizacionporcentaje").setRequiredLevel("required");
        formContext.getControl("atos_penalizacionporcentaje").setDisabled(false);
    }
}

/*
 * Function from OnLoad event
 * Function from BonificacionConsumo event
 * @param {executionContext} Contexto de ejecucion del formulario
 */
function bonificacionConsumo(executionContext) {
    
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
        if (campoVacio(formContext, "atos_importebonificacion") == false) // Si tiene valor lo vacía
        {
            formContext.getAttribute("atos_importebonificacion").setValue(null);
            formContext.getAttribute("atos_importebonificacion").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_rangoinferiorbonificacion") == false) // Si tiene valor lo vacía
        {
            formContext.getAttribute("atos_rangoinferiorbonificacion").setValue(null);
            formContext.getAttribute("atos_rangoinferiorbonificacion").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_rangosuperiorbonificacion") == false) // Si tiene valor lo vacía
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


/*
 * Function from OnLoad event
 * @param {executionContext} Contexto de ejecucion del formulario
 * Habilita o no los datos de garantía en función del tipo de garantía
 */
function tipoGarantia(executionContext) {

    var formContext = executionContext.getFormContext();
    if (formContext.getAttribute("atos_tipogarantiasolicitadacliente").getValue() != 300000000) {
        formContext.getControl("atos_importegarantiasolicitada").setDisabled(false);
        formContext.getControl("atos_fechasolicitudgarantiacliente").setDisabled(false);
        formContext.getControl("atos_fecharealizaciongarantiacliente").setDisabled(false);
        formContext.getControl("atos_fechainiciogarantia").setDisabled(false);
        formContext.getControl("atos_fechafingarantia").setDisabled(false);
    }
    else {
        if (campoVacio(formContext, "atos_importegarantiasolicitada") == false) // Si tiene valor lo vacía
        {
            formContext.getAttribute("atos_importegarantiasolicitada").setValue(null);
            formContext.getAttribute("atos_importegarantiasolicitada").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_fechasolicitudgarantiacliente") == false) // Si tiene valor lo vacía
        {
            formContext.getAttribute("atos_fechasolicitudgarantiacliente").setValue(null);
            formContext.getAttribute("atos_fechasolicitudgarantiacliente").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_fecharealizaciongarantiacliente") == false) // Si tiene valor lo vacía
        {
            formContext.getAttribute("atos_fecharealizaciongarantiacliente").setValue(null);
            formContext.getAttribute("atos_fecharealizaciongarantiacliente").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_fechainiciogarantia") == false) // Si tiene valor lo vacía
        {
            formContext.getAttribute("atos_fechainiciogarantia").setValue(null);
            formContext.getAttribute("atos_fechainiciogarantia").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_fechafingarantia") == false) // Si tiene valor lo vacía
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


/*
 * Function from OnLoad event
 * Function from FormaPago event
 * @param {executionContext} Contexto de ejecucion del formulario
 * Si la forma de pago es domiciliación bancaria habilita los campos de la cuenta bancaria
 */
function domiciliacionBancaria(executionContext) {

    var formContext = executionContext.getFormContext();
    if (formContext.getAttribute("atos_formadepago").getValue() == 300000001) // Domiciliación
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
        formContext.ui.clearFormNotification("8"); // Limpiamos el posible error de validación de la cuenta
        formContext.getAttribute("atos_iban").setRequiredLevel("none");
        formContext.getAttribute("atos_entidadbancaria").setRequiredLevel("none");
        formContext.getAttribute("atos_sucursalbancaria").setRequiredLevel("none");
        formContext.getAttribute("atos_digitocontrol").setRequiredLevel("none");
        formContext.getAttribute("atos_cuenta").setRequiredLevel("none");
        if (campoVacio(formContext, "atos_swift") == false) // Si tiene valor lo vacía
        {
            formContext.getAttribute("atos_swift").setValue(null);
            formContext.getAttribute("atos_swift").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_iban") == false) // Si tiene valor lo vacía
        {
            formContext.getAttribute("atos_iban").setValue(null);
            formContext.getAttribute("atos_iban").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_entidadbancaria") == false) // Si tiene valor lo vacía
        {
            formContext.getAttribute("atos_entidadbancaria").setValue(null);
            formContext.getAttribute("atos_entidadbancaria").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_sucursalbancaria") == false) // Si tiene valor lo vacía
        {
            formContext.getAttribute("atos_sucursalbancaria").setValue(null);
            formContext.getAttribute("atos_sucursalbancaria").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_digitocontrol") == false) // Si tiene valor lo vacía
        {
            formContext.getAttribute("atos_digitocontrol").setValue(null);
            formContext.getAttribute("atos_digitocontrol").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_cuenta") == false) // Si tiene valor lo vacía
        {
            formContext.getAttribute("atos_cuenta").setValue(null);
            formContext.getAttribute("atos_cuenta").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_cuentabancaria") == false) // Si tiene valor lo vacía
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
 * Function from FechaInicioContrato event
 * @param {executionContext} Contexto de ejecucion del formulario
 * Inicializa (si está vacía), la fecha inicio vigencia de precios con el valor de la fecha de inicio del contrato
 */
function fechaIniVigPrecios(formContext) {

    try {
        var formContext = formContext.getFormContext();
    }
    catch{

    }

    if ( //formContext.getAttribute("atos_fechainiciovigenciaprecios").getValue() == null &&
        formContext.getAttribute("atos_fechainiciocontrato").getValue() != null) {
        formContext.getAttribute("atos_fechainiciovigenciaprecios").setValue(formContext.getAttribute("atos_fechainiciocontrato").getValue());
        formContext.getAttribute("atos_fechainiciovigenciaprecios").setSubmitMode("always");
    }
}

/*
 * Function from FechaInicioContrato event
 * @param {executionContext} Contexto de ejecucion del formulario
 * Inicializa (si está vacía), la fecha inicio vigencia de precios con el valor de la fecha de inicio del contrato
 */
function fechaAceptacionOferta(formContext) {

    try {
        var formContext = formContext.getFormContext();
    }
    catch{

    }
 
    if (formContext.getAttribute("atos_fechaaceptacionoferta").getValue() == null &&
        formContext.getAttribute("atos_fechainiciocontrato").getValue() != null) {
        formContext.getAttribute("atos_fechaaceptacionoferta").setValue(formContext.getAttribute("atos_fechainiciocontrato").getValue());
        formContext.getAttribute("atos_fechaaceptacionoferta").setSubmitMode("always");
    }
}


/**
// <summary>
// Valida si la cuenta bancaria es correcta o no
// </summary>
*/
function validaCuentaBancaria(i_entidad, i_oficina, i_digito, i_cuenta) {
    // Funcion recibe como parámetro la entidad, la oficina, 
    // el digito (concatenación del de control entidad-oficina y del de control entidad)
    // y la cuenta. Espera los valores con 0's a la izquierda.
    // Devuelve true o false.
    // NOTAS:
    // Formato deseado de los parámetros:
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
 * Function from Entidad event
 * Function from Sucursal event
 * Function from DigitoControl event
 * Function from Cuenta event
 * @param {formContext} Contexto de ejecucion del formulario
 * Calcula el Iban si la cuenta bancaria es correcta
 */
function validaCuenta(formContext) {

    try {
        var formContext = executionContext.getFormContext();
    }
    catch{

    }

    formContext.ui.clearFormNotification("8");
    var entidad = formContext.getAttribute("atos_entidadbancaria").getValue();
    var oficina = formContext.getAttribute("atos_sucursalbancaria").getValue();
    var dc = formContext.getAttribute("atos_digitocontrol").getValue();
    var cuenta = formContext.getAttribute("atos_cuenta").getValue();

    var validacion = true;

    if (entidad != null && oficina != null && dc != null && cuenta != null) {
        if (entidad.length == 4 && oficina.length == 4 && dc.length == 2 && cuenta.length == 10) {
            validacion = validaCuentaBancaria(entidad, oficina, dc, cuenta);
            if (!validacion)
                formContext.setFormNotification("Cuenta bancaria incorrecta.", "ERROR", "8"); // alert("Cuenta bancaria incorrecta");
            else
                calcularIban(formContext, entidad + oficina + dc + cuenta);
        }
        else {
            validacion = false;
            formContext.setFormNotification("Cuenta bancaria incorrecta.", "ERROR", "8");
            //alert("Cuenta bancaria incorrecta.");
        }

    }
    return validacion;
}


/**
// <summary>
// Elimina los espacios de una cadena
// </summary>
*/
function trim(myString) {
    return myString.replace(/^\s+/g, '').replace(/\s+$/g, '');
}



/**
// <summary>
// Calcula el Iban de un número de cuenta
// </summary>
*/
function calcularIban(formContext,ccc) {

    
    try {
        var formContext = executionContext.getFormContext();
    }
    catch{

    }

    formContext.ui.clearFormNotification("9");
    //Limpiamos el numero de IBAN
    var cccaux = ccc.toUpperCase();  //Todo a Mayus
    cccaux = trim(cccaux); //Quitamos blancos de principio y final.
    cccaux = cccaux.replace(/\s/g, "");  //Quitamos blancos del medio.
    cccaux = cccaux + "142800"; // Añadimos el código de España

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
