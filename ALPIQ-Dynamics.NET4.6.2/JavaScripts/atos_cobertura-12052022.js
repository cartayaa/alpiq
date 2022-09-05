/*
 File="atos_cierrejs.js" 
 Copyright (c) Atos. All rights reserved.

 Fecha 		Codigo  Version Autor               Descripcion
 12.11.2020                 Lazaro Castro       Se ejecuta en el OnLoad del formulario para validar coeficiente de apuntamiento.
 16/03/2021 223231  1.0.1.1 A. Cartaya          Cambio el FechXml con sentencia no-lock='true'
 11705/2022 23680   1.0.1.2 A. Cartaya          Incorporacion de la funcio LoadForm
						                        y las variables globales globalContext, formContext

*/

//#region Variables Funciones Globales 23680
var globalContext;
var formContext;
var codeLang;

const  POWER = 300000000;
const  GAS = 300000001;


//#region General Functions
function getGlobalContext() { return Xrm.Utility.getGlobalContext(); }
function getContext() { return executionContext.getFormContext(); } 
function getUserId() { return globalContext.getUserId(); }
function getUserRole() { return globalContext.getUserRoles(); }                          
function getId() { return formContext.data.entity.getId(); }
function IsEmptyId() { if (getId() == null || getId() == "") return true; else return false; }  
function getClient() { return globalContext.client.getClient(); }
function getServerUrl() {  return Xrm.Page.context.getClientUrl(); } 
function getFormType() {  return Xrm.Page.ui.getFormType(); } // Form type  0-Undefined, 1-Create 2-Update 3-Read Only 4-Disabled  6-Bulk Edit
function getFormId() {  return formContext.ui.formSelector.getCurrentItem().getId(); } 
function getFormName() {  return formContext.ui.formSelector.getCurrentItem().getLabel(); } 
//#endregion 


/*
 * Function from OnLoad event
 * @param {executionContext} Contexto de ejecucion del formulario
 * Aplicado al evento OnLoad de primero
 */
function LoadForm(executionContext) {

    //Recupera contexto
    formContext = executionContext.getFormContext();
    // referencia de la API de cliente
    globalContext = Xrm.Utility.getGlobalContext();
    //Idioma
    codeLang = globalContext.userSettings.languageId;

	try {                                                                                   
		head.load( getServerUrl() + "/WebResources/wave1_timer.js", function() {});
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
   
    debugger;

    formContext.getAttribute("atos_interfazcoberturaems").setValue("");
    formContext.getAttribute("atos_ultimologwscobertura").setValue("");
}