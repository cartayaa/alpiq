/*
 File="atos_cierrejs.js" 
 Copyright (c) Atos. All rights reserved.

 Fecha 		Codigo  Version Autor               Descripcion
 12.11.2020                 Lazaro Castro       Se ejecuta en el OnLoad del formulario para validar coeficiente de apuntamiento.
 16/03/2021 223231  1.0.1.1 A. Cartaya          Cambio el FechXml con sentencia no-lock='true'
 11/05/2022 23680   1.0.1.2 A. Cartaya          Incorporacion de la funcion LoadForm
                                                y las variables globales globalContext, formContext
                                                Incorporacion del campo Valor Final de Coste
 01/07/2022 23794   1.0.1.3 A. Cartaya          Obtencion del pricing Input, el Periodo (Mensual, Trimestral o Anual)
                                                y las variables globales globalContext, formContext
                                                Obtiener desde el contrato asociado el coste por Gestion de Cierre
                                                Aplicarlo en e campo atos_pricinginputid                                                

*/

//#region Variables Funciones Globales 23680
var globalContext;
var formContext;
var codeLang;

const POWER = 300000000;
const GAS = 300000001;


//#region General Functions
function getGlobalContext() { return Xrm.Utility.getGlobalContext(); }
function getContext() { return executionContext.getFormContext(); }
function getUserId() { return globalContext.getUserId(); }
function getUserRole() { return globalContext.getUserRoles(); }
function getId() { return formContext.data.entity.getId(); }
function IsEmptyId() { if (getId() == null || getId() == "") return true; else return false; }
function getClient() { return globalContext.client.getClient(); }
function getServerUrl() { return Xrm.Page.context.getClientUrl(); }
function getFormType() { return Xrm.Page.ui.getFormType(); } // Form type  0-Undefined, 1-Create 2-Update 3-Read Only 4-Disabled  6-Bulk Edit
function getFormId() { return formContext.ui.formSelector.getCurrentItem().getId(); }
function getFormName() { return formContext.ui.formSelector.getCurrentItem().getLabel(); }
//#endregion 


/*
 * Function from OnLoad event
 * @param {executionContext} Contexto de ejecucion del formulario
 * Aplicado al evento OnLoad de primero
 */
function LoadForm(executionContext) {

    // Recupera contexto
    formContext = executionContext.getFormContext();
    // Referencia de la API de cliente
    globalContext = Xrm.Utility.getGlobalContext();
    // Idioma
    codeLang = globalContext.userSettings.languageId;

    try {
        head.load(getServerUrl() + "/WebResources/wave1_timer.js", function () { });
    }
    catch (error) {
        console.log("Error timer.js (head.load): (" + error.description + ")");
    }

}


/*
 * Function from OnSave event
 * @param {executionContext} Contexto de ejecucion del formulario
 * Aplicado al evento OnSave de primero
 * 
 * - Comprueba que esta relleno el Cierre o el Cierre por MWh (solo uno de los dos)
 * - Si es erroneo no permite guardar los datos
 */

function SaveForm(executionContext) {

    calcularPreciosCierre();

    if (formContext.getAttribute("atos_valorcierre").getIsDirty() == true ||
        formContext.getAttribute("atos_margen").getIsDirty() == true ||
        formContext.getAttribute("atos_costegestioncierre").getIsDirty() == true ||
        formContext.getAttribute("atos_porcentajecierre").getIsDirty() == true) {
        formContext.getAttribute("atos_interfazcierreems").setValue("");
        formContext.getAttribute("atos_ultimologwscierre").setValue("");
    }
}


function preFiltroCoeficienteApuntamiento(executionContext) {

    if (formContext.getAttribute("atos_contratoid").getValue() != null)
    {
        var contratoid = formContext.getAttribute("atos_contratoid").getValue();
        Xrm.WebApi.retrieveRecord("atos_contrato", contratoid[0].id, "?$select=atos_contratoid&$expand=atos_ofertaid($select=atos_ofertaid,atos_tipooferta)").then(function (result) {
            if (null != result["atos_ofertaid"]) {
                var ofertaid = result["atos_ofertaid"].atos_ofertaid;
                Xrm.WebApi.retrieveRecord("atos_oferta", ofertaid, "?$select=atos_ofertaid&$expand=atos_ofertapadreid($select=atos_ofertaid)").then(function (result) {
                    if (result["atos_ofertapadreid"] != null) {
                        ofertaid = result["atos_ofertapadreid"].atos_ofertaid;
                    }
                    if (formContext.getAttribute("atos_cierreofertaid").getValue() != null) {
                        formContext.getControl("atos_pricinginputid").addPreSearch(function () {
                            addFiltroCoeficienteApuntamientoCierre(formContext, ofertaid, formContext.getAttribute("atos_cierreofertaid").getValue()[0].id);
                        });
                    }
                    else {
                        formContext.getControl("atos_pricinginputid").addPreSearch(function () {
                            addFiltroCoeficienteApuntamiento(formContext, ofertaid);
                        });
                    }
                }, function (error) {
                    Xrm.Navigation.openErrorDialog({ detail: error.message });
                });
            }
        }, function (error) {
            Xrm.Navigation.openErrorDialog({ detail: error.message });
        });
    }
}
/**
// <summary>
// Llamada desde preFiltroCoeficienteApuntamiento para filtrar el Control atos_pricinginputid.
// Autor: Lazaro Castro - 12.11.2020
// </summary>
*/
function addFiltroCoeficienteApuntamiento(formContext, ofertaid) {
    if (ofertaid != null) {
        var fetchXml = "<filter type='and'><condition attribute='atos_ofertaid' operator='eq' value='" + ofertaid + "' />" +
            "<condition attribute='atos_cierreofertaid' operator='not-null' /></filter>";
        formContext.getControl("atos_pricinginputid").addCustomFilter(fetchXml);
    }
}

/**
// <summary>
// Llamada desde preFiltroCoeficienteApuntamiento para filtrar el Control atos_pricinginputid.
// Autor: Lazaro Castro - 12.11.2020
// </summary>
*/
function addFiltroCoeficienteApuntamientoCierre(formContext, ofertaid, cierreofertaid) {
    if (ofertaid != null) {
        var fetchXml = "<filter type='and'><condition attribute='atos_ofertaid' operator='eq' value='" + ofertaid + "' />" +
            "<condition attribute='atos_cierreofertaid' operator='eq' value='" + cierreofertaid + "' /></filter>";
        formContext.getControl("atos_pricinginputid").addCustomFilter(fetchXml);
    }
}

/**
// <summary>
// Llamada desde cierreOferta para setear el Control atos_pricinginputid.
// Autor: Lazaro Castro - 12.11.2020
// </summary>
*/
function setVariable(formContext, pricingoutputid)
{
    if (pricingoutputid == null) {
        formContext.getAttribute("atos_variable").setValue(null);
    }
    else {
        // var cols = ["atos_name"];
        // var pricingoutput = XrmServiceToolkit.Soap.Retrieve("atos_pricingoutput", pricingoutputid, cols);
        Xrm.WebApi.retrieveRecord("atos_pricingoutput", pricingoutputid, "?$select=atos_name").then(function (result) {
            //debugger;
            if (result["atos_name"] != "") {
                formContext.getAttribute("atos_variable").setValue(construyeLookup("atos_pricingoutput", pricingoutputid, result["atos_name"]));
            }
        }, function (error) {
            Xrm.Navigation.openErrorDialog({ detail: error.message });
        });
    }
    formContext.getAttribute("atos_variable").setSubmitMode("always");
}

function cierreOferta(executionContext)
{
    debugger;

    if (formContext.getAttribute("atos_cierreofertaid").getValue() != null)
    {
        cierraofertaid = formContext.getAttribute("atos_cierreofertaid").getValue();
        // var cols = ["atos_pricingoutputid", "atos_costegestioncierre"];
        // var cierreoferta = XrmServiceToolkit.Soap.Retrieve("atos_cierreoferta", Xrm.Page.data.entity.attributes.get("atos_cierreofertaid").getValue()[0].id, cols);
        Xrm.WebApi.retrieveRecord("atos_cierreoferta", cierraofertaid[0].id, "?$select=atos_costegestioncierre&$expand=atos_pricingoutputid($select=atos_pricingoutputid)").then(function (result) {
            debugger;
            if (result["atos_pricingoutputid"] != null) {
                setVariable(formContext, result["atos_pricingoutputid"].atos_pricingoutputid);
            }
            else {
                setVariable(formContext, null);
            }
            if (result["atos_costegestioncierre"] != null) {
                debugger;
                formContext.getAttribute("atos_costegestioncierre").setValue(result["atos_costegestioncierre"]);
            }
            else {
                setVariable(formContext, null);
                preFiltroCoeficienteApuntamiento();
            }
        }, function (error) {
            Xrm.Navigation.openErrorDialog({ detail: error.message });
        });
    }
}

/**
// <summary>
// Se ejecuta en el OnLoad del formulario para validar coeficiente de apuntamiento.
// Autor: Lazaro Castro - 12.11.2020
// </summary>
*/
function preFiltroCierreOferta(executionContext)
{
    var contratoid = formContext.getAttribute("atos_contratoid").getValue();

    if (formContext.getAttribute("atos_contratoid").getValue() != null)
    {    
        Xrm.WebApi.retrieveRecord("atos_contrato", contratoid[0].id, "?$select=atos_contratoid&$expand=atos_ofertaid($select=atos_ofertaid)").then(function (result) {
            if (null != result["atos_ofertaid"]) {
                var ofertaid = result["atos_ofertaid"].atos_ofertaid;
                Xrm.WebApi.retrieveRecord("atos_oferta", ofertaid, "?$select=atos_ofertaid&$expand=atos_ofertapadreid($select=atos_ofertaid,atos_tipooferta)").then(function (result) {
                    //debugger;
                    if (result["atos_ofertapadreid"] != null) {
                        ofertaid = result["atos_ofertapadreid"].atos_ofertaid;
                    }
                    formContext.getControl("atos_cierreofertaid").addPreSearch(function () {
                        addFiltroCierreOferta(formContext, ofertaid);
                    });
                }, function (error) {
                    Xrm.Navigation.openErrorDialog({ detail: error.message });
                });
            }
        }, function (error) {
            Xrm.Navigation.openErrorDialog({ detail: error.message });
        });
    }
}
/**
// Se ejecuta en el OnLoad del formulario para pasar el filtro dal ciere de oferta.
// Autor: Lazaro Castro - 12.11.2020
*/
function addFiltroCierreOferta(formContext, ofertaid) {
    if (ofertaid != null) {
        var fetchXml = "<filter type='and'><condition attribute='atos_ofertaid' operator='eq' value='" + ofertaid + "' /></filter>";
        formContext.getControl("atos_cierreofertaid").addCustomFilter(fetchXml);
    }
}


/**
// Esta funcion de llama desde %Cierre
// Función que comprueba que no estén rellenos los campos Cierre y el Cierre por MWh.
// Aplicar regla de negocio
*/

function validaCierre(executionContext)
{
    debugger;

    formContext.getControl("atos_porcentajecierre").clearNotification("5");
    formContext.getControl("atos_valorcierre").clearNotification("5");

    // formContext.clearFormNotification("5");
    formContext.ui.clearFormNotification("5");

    var valido = true;

    var porcentaje = formContext.getAttribute("atos_porcentajecierre").getValue();
    var energia = formContext.getAttribute("atos_valorcierre").getValue();

    //if (porcentaje != null && energia != null) {
    //    valido = false;
    //    menserror = "Tan solo puede estar informado uno de los dos siguientes campos: Valor cierre o Cierre(%)";

    //    Xrm.Page.ui.setFormNotification(menserror, "ERROR", '5');
    //}
    // if (porcentaje == null && energia == null) {
    //	   menserror = "Tiene que estar informado uno de los dos siguientes campos: Valor cierre o Cierre(%)";

    //    Xrm.Page.ui.setFormNotification(menserror, "ERROR", '5'); 
    // }
    return valido;
}

/*

*- Entidad Cierre de Contratos
*- Funcion llamada desde el Ribbon
*
* Funcion que si no se ha hecho una copia previa y en caso de que se haya hecho impide la copia. 
* en otro caso se pregunta al usuario si quiere continuar y se lanza el proceso.
*
* - No se ha encontrado la relacion con esta funcion en Cierre_Contratos
* - Autor: Lazaro Castro  - 13.11.2020

*/
function copiaCierreMP(executionContext)
{
    formContext.ui.clearFormNotification();
    var copiaLanzada = formContext.getAttribute("atos_copiaenmultipuntolanzada").getValue();

    if (copiaLanzada == true) {
        formContext.setFormNotification("La copia a multipunto ya ha sido realizada y no puede ejecutarse de nuevo", "ERROR");
    }
    else {

        var confirmStrings = { text: "¿Desea lanzar la copia de cierre en el multipunto?", title: "Copiar multipunto" };
        var confirmOptions = { height: 200, width: 450 };

        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
            function (success) {
                if (success.confirmed)
                    creaTriggerCopia(executionContext);
                else
                    console.log("Dialog closed using Cancel button or X.");
            });
    }
}

/*
* Al parecer esta funcon ya no se utiliza
* Autor: Lazaro Castro - 12.11.2020
*/
function creaTriggerCopia(executionContext)
{
    var formContext = executionContext;
    // define the data to create new account
    var creaCierre =
    {
        "atos_accion": "Cierre",
        "atos_entity": "atos_cierre",
        "atos_guid": Xrm.Page.data.entity.getId()
    }
    //var creaCierre = new XrmServiceToolkit.Soap.BusinessEntity("atos_trigger");
    //creaCierre.attributes["atos_accion"] = "Cierre";
    //creaCierre.attributes["atos_entity"] = "atos_cierre";
    //creaCierre.attributes["atos_guid"] = Xrm.Page.data.entity.getId();
    // create account record
    Xrm.WebApi.createRecord("atos_trigger", creaCierre).then(
        function success(result) {
            if (result != null) {
                creaCierreId = result.id;
                if (result["atos_respuesta"] != null) {
                    formContext.ui.setFormNotification(result["atos_respuesta"].value, "INFO", '1');
                }
                else {
                    formContext.getAttribute("atos_copiaenmultipuntolanzada").setValue(true);
                    formContext.ui.setFormNotification("Proceso de copia realizado", "INFO", "1");
                }
            }
        },
        function (error) {
            Xrm.Navigation.openErrorDialog({ detail: error.message });
            formContext.ui.clearFormNotification('2');
            formContext.ui.setFormNotification(err.message, 'ERROR', '1');
        });
}



/**
//  Función que comprueba fechas
//  Modificada: Lazaro Castro - 12.11.2020
//  No se ha detectado las dependencias de esta funciÃ³n
*/
function CheckDates(executionContext)
{
    LimpiarNotificacion("atos_fechainiciocierre");
    LimpiarNotificacion("atos_fechafincierre");

    var valorFechaInicio = formContext.getAttribute("atos_fechainiciocierre").getValue();
    var valorFechaFin = formContext.getAttribute("atos_fechafincierre").getValue();

    if (valorFechaInicio && valorFechaFin) {
        var inicioDate = new Date(valorFechaInicio);
        var finDate = new Date(valorFechaFin);

        var diaInicio = inicioDate.getDate();
        if (diaInicio == 1)
        {
            var resultadoComparacion = comparafechasSinHoras(inicioDate, finDate);
            if (resultadoComparacion == -1)
            {
                var diaFin = finDate.getDate();
                var mesFin = finDate.getMonth();
                var annoFin = finDate.getFullYear();

                var ultimoDiaMes = diasmes(mesFin + 1, annoFin);

                if (diaFin == ultimoDiaMes) {
                    var mesInicio = inicioDate.getMonth();
                    var annoInicio = inicioDate.getFullYear();

                    if (annoInicio == annoFin) {

                        if ((mesInicio + 1 == 1 && mesFin + 1 == 12)) {
                            LimpiarNotificacion("atos_fechainiciocierre");
                            LimpiarNotificacion("atos_fechafincierre");
                            AsignarCostesCierre("Anual");
                        }

                        else if (EsCierreTrimestral(mesInicio + 1, mesFin + 1)) {
                            LimpiarNotificacion("atos_fechainiciocierre");
                            LimpiarNotificacion("atos_fechafincierre");
                            AsignarCostesCierre("Trimestral");
                        }

                        else if (mesInicio == mesFin) {
                            LimpiarNotificacion("atos_fechainiciocierre");
                            LimpiarNotificacion("atos_fechafincierre");
                            AsignarCostesCierre("Mensual");
                        }

                        else {
                            ColocarNotificacion("atos_fechafincierre", "Los cierres deben delimitarse dentro del mismo mes, trimestre fiscal o añoo natural");
                            LimpiarCostesCierre();
                        }
                    }
                    else {
                        ColocarNotificacion("atos_fechafincierre", "Los cierres han de estar delimitados dentro del mismo añoo natural");
                        LimpiarCostesCierre();
                    }

                }
                else {
                    ColocarNotificacion("atos_fechafincierre", "El fin del cierre debe coincidir con el último dí­a del mes");
                    LimpiarCostesCierre();
                }
            }
            else {
                ColocarNotificacion("atos_fechafincierre", "La fecha de finalización ha de ser posterior a la de inicio");
                LimpiarCostesCierre();
            }

        }
        else {
            ColocarNotificacion("atos_fechainiciocierre", "Los cierres han de comenzar el primer dí­a del mes");
            LimpiarCostesCierre();
        }
    }
    else {
        LimpiarCostesCierre();

        if (!valorFechaInicio) {
            ColocarNotificacion("atos_fechainiciocierre", "Debe proporcionar un valor para la fecha de inicio");
        }
        if (!valorFechaFin) {
            ColocarNotificacion("atos_fechafincierre", "Debe proporcionar un valor para la fecha de fin");
        }
    }
}

function EsCierreTrimestral(mesInicio, mesFin) {
    var correcto = false;
    if (
        (mesInicio == 1 && mesFin == 3) ||
        (mesInicio == 4 && mesFin == 6) ||
        (mesInicio == 7 && mesFin == 9) ||
        (mesInicio == 10 && mesFin == 12)
    ) {
        correcto = true;
    }
    return correcto;
}

function LimpiarNotificacion(nombreCampo) {
    formContext.getControl(nombreCampo).clearNotification();
}

function ColocarNotificacion(nombreCampo, mensaje) {
    formContext.getControl(nombreCampo).setNotification(mensaje);
}

function AsignarCostesCierre(periodicidad) {

    if (formContext.getAttribute("atos_contratoid") && formContext.getAttribute("atos_contratoid").getValue() != null) {

        var contratoid = formContext.getAttribute("atos_contratoid").getValue()[0].id;
        var coste = GetCostesCierresContrato(contratoid, periodicidad);

        /*23794 -4 */
        /* if (coste && coste != 0) 
            formContext.getAttribute("atos_costegestioncierre").setValue(coste.value);
        else 
            LimpiarCostesCierre(formContext); 
        */
    }
    else {
        LimpiarCostesCierre();
    }
}

function LimpiarCostesCierre()
{
    formContext.getAttribute("atos_costegestioncierre").setValue(0);
}


/*
 Function: GetCostesCierresContrato()

 Fecha 		Codigo  Version Autor               Descripcion
 01/07/2022 23794   1.0.1.3 A. Cartaya          Obtencion del pricing Input, el Periodo (Mensual, Trimestral o Anual)
                                                y las variables globales globalContext, formContext
                                                Obtiener desde el contrato asociado el coste por Gestion de Cierre
                                                Aplicarlo en e campo atos_pricinginputid
*/

function GetCostesCierresContrato(contratoid, periodicidad) {

    // var cols = ["atos_costegestioncierresmensuales", "atos_costegestioncierrestrimestrales", "atos_costegestioncierresanuales"];
    // var contrato = XrmServiceToolkit.Soap.Retrieve("atos_contrato", idContrato, cols);
    var coste = 0;

    Xrm.WebApi.retrieveRecord("atos_contrato", contratoid, "?$select=atos_costegestioncierresmensuales,atos_costegestioncierrestrimestrales,atos_costegestioncierresanuales").then(
        function success(contrato) {

            debugger;

            switch (periodicidad) {
                case "Mensual":
                    /*23794 -2 */
                    //if (contrato.attributes["atos_costegestioncierresmensuales"] != "") { 
                    //coste = contrato.attributes["atos_costegestioncierresmensuales"];
                    coste = contrato.atos_costegestioncierresmensuales;
                    /*23794 -1 *///}
                    break;

                case "Trimestral":
                    /*23794 -2 */
                    //if (contrato.attributes["atos_costegestioncierrestrimestrales"] != "") {
                    //coste = contrato.attributes["atos_costegestioncierrestrimestrales"];
                    coste = contrato.atos_costegestioncierrestrimestrales;
                    /*23794 -1 *///}
                    break;

                case "Anual":
                    /*23794 -2 */
                    //if (contrato.attributes["atos_costegestioncierresanuales"] != "") {
                    //coste = contrato.attributes["atos_costegestioncierresanuales"];
                    coste = contrato.atos_costegestioncierresanuales;
                    /*23794 -1 *///}
                    break;
            }

            /*23794 +4 */
            if (coste && coste != 0)
                formContext.getAttribute("atos_costegestioncierre").setValue(coste);
            else
                LimpiarCostesCierre();

            return coste;

        }, function (error) {
            Xrm.Navigation.openErrorDialog({ detail: error.message });
        });

    return coste;
}

/*
 Function: calcularPreciosCierre()

 Fecha 		Codigo  Version Autor               Descripcion
 11/05/2022 23680   1.0.1.2 A. Cartaya          Incorporacion de la funcion LoadForm
                                                y las variables globales globalContext, formContext
                                                Incorporacion del campo Valor Final de Coste
*/

function calcularPreciosCierre()
{
    var margen = Xrm.Page.data.entity.attributes.get("atos_margen").getValue();
    if (margen == null)
        margen = 0;

    var valorCierre = Xrm.Page.data.entity.attributes.get("atos_valorcierre").getValue(); // 23680
    var coeficiente;

    if (Xrm.Page.data.entity.attributes.get("atos_pricinginputid").getValue() != null)
        coeficiente = Xrm.Page.data.entity.attributes.get("atos_pricinginputid").getValue()[0].id;

    var costeGestion = Xrm.Page.data.entity.attributes.get("atos_costegestioncierre").getValue();

    if (costeGestion == null)
        costeGestion = 0;

    if (valorCierre != null && coeficiente != null) // 23680
    {
        limpiaCampos();

        var pCoeficienteFijo;
        var pCoeficiente1; // 23680
        var pCoeficiente2; // 23680
        var pCoeficiente3; // 23680
        var pCoeficiente4; // 23680
        var pCoeficiente5; // 23680
        var pCoeficiente6; // 23680
        var precioSNFijo;
        var precio1 = 0;
        var precio2 = 0;
        var precio3 = 0;
        var precio4 = 0;
        var precio5 = 0;
        var precio6 = 0;
        var precioSC1 = 0;
        var precioSC2 = 0;
        var precioSC3 = 0;
        var precioSC4 = 0;
        var precioSC5 = 0;
        var precioSC6 = 0;

        //Recogemos los periodos del coeficiente		
        var fetchXml = "<fetch no-lock='true'>" +
            "<entity name='atos_pricinginput'>" +
            "<attribute name='atos_pfijo'/>" +
            "<attribute name='atos_p1'/>" +
            "<attribute name='atos_p2'/>" +
            "<attribute name='atos_p3'/>" +
            "<attribute name='atos_p4'/>" +
            "<attribute name='atos_p5'/>" +
            "<attribute name='atos_p6'/>" +
            "<filter>" +
            "<condition attribute='atos_pricinginputid' operator='eq' value='" + coeficiente + "' />" +
            "</filter>" +
            "</entity>" +
            "</fetch>";

        var registros = XrmServiceToolkit.Soap.Fetch(fetchXml);

        if (registros.length > 0) {
            if (registros[0].attributes["atos_pfijo"] != null)
                pCoeficienteFijo = registros[0].attributes["atos_pfijo"].value;
            else {
                if (registros[0].attributes["atos_p1"] != null) pCoeficiente1 = registros[0].attributes["atos_p1"].value; // 23680			
                if (registros[0].attributes["atos_p2"] != null) pCoeficiente2 = registros[0].attributes["atos_p2"].value; // 23680
                if (registros[0].attributes["atos_p3"] != null) pCoeficiente3 = registros[0].attributes["atos_p3"].value; // 23680
                if (registros[0].attributes["atos_p4"] != null) pCoeficiente4 = registros[0].attributes["atos_p4"].value; // 23680
                if (registros[0].attributes["atos_p5"] != null) pCoeficiente5 = registros[0].attributes["atos_p5"].value; // 23680
                if (registros[0].attributes["atos_p6"] != null) pCoeficiente6 = registros[0].attributes["atos_p6"].value; // 23680
            }
        }

        if (pCoeficienteFijo != null) {
            precioSNFijo = parseFloat(valorCierre * pCoeficienteFijo);
            precioSNFijo = parseFloat(((margen + valorCierre + costeGestion) * pCoeficienteFijo) / 100);
        } else {
            if (pCoeficiente1 != null) precio1 = parseFloat(((margen + valorCierre + costeGestion) * pCoeficiente1) / 100); // 23680
            if (pCoeficiente2 != null) precio2 = parseFloat(((margen + valorCierre + costeGestion) * pCoeficiente2) / 100); // 23680
            if (pCoeficiente3 != null) precio3 = parseFloat(((margen + valorCierre + costeGestion) * pCoeficiente3) / 100); // 23680
            if (pCoeficiente4 != null) precio4 = parseFloat(((margen + valorCierre + costeGestion) * pCoeficiente4) / 100); // 23680
            if (pCoeficiente5 != null) precio5 = parseFloat(((margen + valorCierre + costeGestion) * pCoeficiente5) / 100); // 23680
            if (pCoeficiente6 != null) precio6 = parseFloat(((margen + valorCierre + costeGestion) * pCoeficiente6) / 100); // 23680

            if (pCoeficiente1 != null) precioSC1 = parseFloat(((margen + valorCierre) * pCoeficiente1) / 100); // 23680
            if (pCoeficiente2 != null) precioSC2 = parseFloat(((margen + valorCierre) * pCoeficiente2) / 100); // 23680
            if (pCoeficiente3 != null) precioSC3 = parseFloat(((margen + valorCierre) * pCoeficiente3) / 100); // 23680
            if (pCoeficiente4 != null) precioSC4 = parseFloat(((margen + valorCierre) * pCoeficiente4) / 100); // 23680
            if (pCoeficiente5 != null) precioSC5 = parseFloat(((margen + valorCierre) * pCoeficiente5) / 100); // 23680
            if (pCoeficiente6 != null) precioSC6 = parseFloat(((margen + valorCierre) * pCoeficiente6) / 100); // 23680
        }

        if (precioSNFijo != null) {
            cargaPreciosFijos(precioSNFijo, costeGestion);
        }
        else
            cargaPreciosVariables(precio1, precio2, precio3, precio4, precio5, precio6, precioSC1, precioSC2, precioSC3, precioSC4, precioSC5, precioSC6, costeGestion);
    }
}

function cargaPreciosFijos(precioSNFijo, costeGestion) {
    Xrm.Page.data.entity.attributes.get("atos_preciosincostep1").setValue(precioSNFijo);
    Xrm.Page.getAttribute("atos_preciosincostep1").setSubmitMode("always");
    Xrm.Page.data.entity.attributes.get("atos_preciop1").setValue(parseFloat(precioSNFijo + costeGestion));
    Xrm.Page.getAttribute("atos_preciop1").setSubmitMode("always");

    Xrm.Page.data.entity.attributes.get("atos_preciosincostep2").setValue(precioSNFijo);
    Xrm.Page.getAttribute("atos_preciosincostep2").setSubmitMode("always");
    Xrm.Page.data.entity.attributes.get("atos_preciop2").setValue(parseFloat(precioSNFijo + costeGestion));
    Xrm.Page.getAttribute("atos_preciop2").setSubmitMode("always");

    Xrm.Page.data.entity.attributes.get("atos_preciosincostep3").setValue(precioSNFijo);
    Xrm.Page.getAttribute("atos_preciosincostep3").setSubmitMode("always");
    Xrm.Page.data.entity.attributes.get("atos_preciop3").setValue(parseFloat(precioSNFijo + costeGestion));
    Xrm.Page.getAttribute("atos_preciop3").setSubmitMode("always");

    Xrm.Page.data.entity.attributes.get("atos_preciosincostep4").setValue(precioSNFijo);
    Xrm.Page.getAttribute("atos_preciosincostep4").setSubmitMode("always");
    Xrm.Page.data.entity.attributes.get("atos_preciop4").setValue(parseFloat(precioSNFijo + costeGestion));
    Xrm.Page.getAttribute("atos_preciop4").setSubmitMode("always");

    Xrm.Page.data.entity.attributes.get("atos_preciosincostep5").setValue(precioSNFijo);
    Xrm.Page.getAttribute("atos_preciosincostep5").setSubmitMode("always");
    Xrm.Page.data.entity.attributes.get("atos_preciop5").setValue(parseFloat(precioSNFijo + costeGestion));
    Xrm.Page.getAttribute("atos_preciop5").setSubmitMode("always");

    Xrm.Page.data.entity.attributes.get("atos_preciosincostep6").setValue(precioSNFijo);
    Xrm.Page.getAttribute("atos_preciosincostep6").setSubmitMode("always");
    Xrm.Page.data.entity.attributes.get("atos_preciop6").setValue(parseFloat(precioSNFijo + costeGestion));
    Xrm.Page.getAttribute("atos_preciop6").setSubmitMode("always");
}


function cargaPreciosVariables(precio1, precio2, precio3, precio4, precio5, precio6, precioSC1, precioSC2, precioSC3, precioSC4, precioSC5, precioSC6, costeGestion) {
    // Precio sin Coste
    if (precioSC1 != null) {
        Xrm.Page.data.entity.attributes.get("atos_preciosincostep1").setValue(precioSC1);
        Xrm.Page.getAttribute("atos_preciosincostep1").setSubmitMode("always");
    }
    if (precioSC2 != null) {
        Xrm.Page.data.entity.attributes.get("atos_preciosincostep2").setValue(precioSC2);
        Xrm.Page.getAttribute("atos_preciosincostep2").setSubmitMode("always");
    }
    if (precioSC3 != null) {
        Xrm.Page.data.entity.attributes.get("atos_preciosincostep3").setValue(precioSC3);
        Xrm.Page.getAttribute("atos_preciosincostep3").setSubmitMode("always");
    }
    if (precioSC4 != null) {
        Xrm.Page.data.entity.attributes.get("atos_preciosincostep4").setValue(precioSC4);
        Xrm.Page.getAttribute("atos_preciosincostep4").setSubmitMode("always");
    }
    if (precioSC5 != null) {
        Xrm.Page.data.entity.attributes.get("atos_preciosincostep5").setValue(precioSC5);
        Xrm.Page.getAttribute("atos_preciosincostep5").setSubmitMode("always");
    }
    if (precioSC6 != null) {
        Xrm.Page.data.entity.attributes.get("atos_preciosincostep6").setValue(precioSC6);
        Xrm.Page.getAttribute("atos_preciosincostep6").setSubmitMode("always");
    }

    // Precio Con Coste
    if (precio1 != null) {
        Xrm.Page.data.entity.attributes.get("atos_preciop1").setValue(parseFloat(precio1));
        Xrm.Page.getAttribute("atos_preciop1").setSubmitMode("always");
    }
    if (precio2 != null) {
        Xrm.Page.data.entity.attributes.get("atos_preciop2").setValue(parseFloat(precio2));
        Xrm.Page.getAttribute("atos_preciop2").setSubmitMode("always");
    }
    if (precio3 != null) {
        Xrm.Page.data.entity.attributes.get("atos_preciop3").setValue(parseFloat(precio3));
        Xrm.Page.getAttribute("atos_preciop3").setSubmitMode("always");
    }
    if (precio4 != null) {
        Xrm.Page.data.entity.attributes.get("atos_preciop4").setValue(parseFloat(precio4));
        Xrm.Page.getAttribute("atos_preciop4").setSubmitMode("always");
    }
    if (precio5 != null) {
        Xrm.Page.data.entity.attributes.get("atos_preciop5").setValue(parseFloat(precio5));
        Xrm.Page.getAttribute("atos_preciop5").setSubmitMode("always");
    }
    if (precio6 != null) {
        Xrm.Page.data.entity.attributes.get("atos_preciop6").setValue(parseFloat(precio6));
        Xrm.Page.getAttribute("atos_preciop6").setSubmitMode("always");
    }
}

function limpiaCampos()
{
    for (var i = 1; i <= 6; i++) {
        Xrm.Page.data.entity.attributes.get("atos_preciosincostep" + i).setValue();
        Xrm.Page.getAttribute("atos_preciosincostep" + i).setSubmitMode("always");
        Xrm.Page.data.entity.attributes.get("atos_preciop" + i).setValue();
        Xrm.Page.getAttribute("atos_preciop" + i).setSubmitMode("always");
    }
}