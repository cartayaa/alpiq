/*
 File="atos_instalcionpower_dialogo.js" 
 Copyright (c) Atos. All rights reserved.

<summary>
 Funciones asociadas a contratos de ALPIQ, entidad personalizada
</summary>

<remarks>
 Fecha 		Codigo  Version Descripcion								 		Autor
  13.11.2020                Se ejecuta en la llamada para cargar la
                            instalación segun instalacionid.                Lazaro Castro
 10.03.2021  22323          Lectura condiciones de pago cuando es           ACR
                            creacion de un contrato desde oferta
 17.06.221   22323          Documentacion                                   ACR  
</remarks>                       
*/

//#region Variables Funciones Globales 22323
var globalContext;
var formContext;
//#endregion Variables Funciones Globales 22323
var contexto;
var formContext;
var razonSocialER = "";
var contextRecordId = "";
var isConfirmed = false;

/*
 * Function from OnLoad event
 * @param {executionContext} Contexto de ejecucion del formulario
 * Aplicado al evento Onload de primero
 */
function LoadForm(executionContext) {
    //debugger;
    //Recupera contexto
    formContext = executionContext.getFormContext();
    // referencia de la API de cliente
    globalContext = Xrm.Utility.getGlobalContext();
    //Idioma
    codeLang = globalContext.userSettings.languageId;
}


/*
 * Function from OnLoad event
 * @param {*} context 
 */
function OnLoad(executionContext) {
    //debugger;
    //Recupera contexto
    formContext = executionContext.getFormContext();
    // referencia de la API de cliente
    globalContext = Xrm.Utility.getGlobalContext();
        
    var entityContext = formContext.data.entity.getEntityName();
    if (entityContext === "atos_cambiorazonsocial") {
        OnLoadForm();
    } 
}




/*
 * Function from OnSave event
 * @param {*} executionObj 
 * @returns 
 */
function onSave(executionObj) {

    var eventArgs = executionObj.getEventArgs();
    if (eventArgs.getSaveMode() == 70) { //AUTOSAVE
        eventArgs.preventDefault();
        return;
    }
         
    if (!isConfirmed) {
        eventArgs.preventDefault();
        Alert.show("Cambio de Razón Social", "<br>Se cambiará la Razón Social asociada a la instalacion. ¿Está seguro de que quiere continuar el proceso?", 
        [
            new Alert.Button("Continuar", confirmCallback, true),
            new Alert.Button("Cancelar")
        ], "WARNING");
    }
    else {
        //Reset the flag for the next save
        isConfirmed = false;
    }
}

function confirmCallback() {

    //to make sure we don't display alert dialog from onSave this time
    isConfirmed = true; 
    Xrm.Page.data.save();
}


/**
 * Function from OnLoad event
 * LLama a esta funcion su el JS corre para la entidad atos_cambiorazonsocial
 */
function OnLoadForm() {
    
    if (formContext.getAttribute("atos_instalacionpowerid").getValue() !== null) {
        formContext.getControl("atos_instalacionpowerid").setVisible(true);
        formContext.getControl("atos_instalaciongasid").setVisible(false);
    }
    else {
        formContext.getControl("atos_instalacionpowerid").setVisible(false);
        formContext.getControl("atos_instalaciongasid").setVisible(true);
    }

    ValidaVisibilidadSecciones();
}


/**
 * Function from OnLoadForm event
 * LLama a esta funcion su el JS corre para la entidad atos_cambiorazonsocial
 */
function ValidaVisibilidadSecciones() {

    var seccionDatosPago = formContext.ui.tabs.get("tab_general").sections.get("seccion_datospago");
    var seccionDatosDomiciliacion = formContext.ui.tabs.get("tab_general").sections.get("seccion_datosdomiciliacion");

    if (formContext.getAttribute("atos_razonsocialid").getValue() !== null && formContext.getAttribute("atos_fechacambio").getValue() !== null) {
        //Set record Name
        var fechacambio = new Date(formContext.getAttribute("atos_fechacambio").getValue());
        var name = fechacambio.format("dd-MM-yyy") + " - " + formContext.getAttribute("atos_razonsocialid").getValue()[0].name;
        formContext.getAttribute("atos_name").setValue(name);

        if (formContext.getAttribute("atos_mantenerdatospago").getValue() === false) {
            //set visible seccion: datos pago
            seccionDatosPago.setVisible(true);
            //set fields required in seccion: datos pago
            seccionDatosPago.controls.forEach(function (control, index) {
                control.getAttribute().setRequiredLevel("required");
            });


            if (formContext.getAttribute("atos_formadepago").getValue() === 300000001) { //domiciliacion
                //set visible seccion: datos domiciliacion
                seccionDatosDomiciliacion.setVisible(true);
                //set fields required in seccion: datos domiciliacion
                seccionDatosDomiciliacion.controls.forEach(function (control, index) {
                    control.getAttribute().setRequiredLevel("required");
                });
            }
            else {
                //remove visibility from seccion: datos domiciliacion
                seccionDatosDomiciliacion.setVisible(false);
                seccionDatosDomiciliacion.controls.forEach(function (control, index) {
                    control.getAttribute().setRequiredLevel("none");
                });
            }
        }
        else {
            //remove visibility from seccion: datos domiciliacion and datos pago
            seccionDatosPago.setVisible(false);
            seccionDatosDomiciliacion.setVisible(false);

            //remove required fields from seccion: datos domiciliacion and datos pago
            seccionDatosPago.controls.forEach(function (control, index) {
                control.getAttribute().setRequiredLevel("none");
            });
            seccionDatosDomiciliacion.controls.forEach(function (control, index) {
                control.getAttribute().setRequiredLevel("none");
            });
        }

        
    }
}

/**
 * openWeb
 */
function openWeb() {

    try {
        var inputDiv = "<br> Se va a proceder a realizar un cambio de razón social de la instalación. ¿Desea proseguir con la operación?";
        Alert.show("Cambio de Razón Social", inputDiv, 
        [
            new Alert.Button("Siguiente", function () {

                var entityFormOptions = {};
                entityFormOptions["entityName"] = "atos_cambiorazonsocial";
                entityFormOptions["openInNewWindow"] = true;

                // Set default values for the Contact form
                var formParameters = {};
                contextRecordId = formContext.data.entity.getId();

                if (formContext.data.entity.getEntityName() === "atos_instalacion") {
                    // Set lookup field
                    formParameters["atos_instalacionpowerid"] = contextRecordId // ID of the user.
                    formParameters["atos_instalacionpoweridname"] = formContext.getAttribute("atos_name").getValue(); // Name of the user.
                    formParameters["atos_instalacionpoweridtype"] = "atos_instalacion"; // Entity name. 
                }
                else {
                    // Set lookup field
                    formParameters["atos_instalaciongasid"] = contextRecordId; // ID of the user.
                    formParameters["atos_instalaciongasidname"] = formContext.getAttribute("atos_name").getValue(); // Name of the user.
                    formParameters["atos_instalaciongasidtype"] = "atos_instalaciongas"; // Entity name. 
                }            
                

                // Open the form.
                Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
                    function (success) {
                        console.log(success);
                    },
                    function (error) {
                        console.log(error);
                    });
            }, true),
            new Alert.Button("Cancelar")
        ], "QUESTION", 400, 200);
       

    } catch (e) {

        Xrm.Navigation.openAlertDialog(e.message);

    }
}
