/*---------------------------------------------------------------------------------
 atos_contrato.js 
 Atos IT
 Copyright (c) Atos. All rights reserved.

 Creación: XX/XX/XXXX ATOS
 Ultima actualizacion

 Fecha       redmine    Descripcion                                 Autor
 10.03.2021  23139      Evento LoadForm								ACR

---------------------------------------------------------------------------------*/

//#region Variables Funciones Globales 23139
var globalContext;
var formContext;
var codeLang;

//#region Evento onLoad 23139
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
//#endregion Evento onLoad 23139


/**
// <summary>
// Se ejecuta en el OnChange del campo Documento para validarlo.
// </summary>
// <remarks>
// Carga los javascripts necesarios:
// -# atos_mensajes.js
// -# atos_validadocumentos.js
// - Llama a la funciÃ³n validaDocumento para que haga la validaciÃ³n.
// </remarks>
 */
function Documento_OnChange() 
{	
    // 23139 var globalContext = Xrm.Utility.getGlobalContext();
    var serverUrl = globalContext.getClientUrl();
	// var serverUrl = Xrm.Page.context.getClientUrl();
	head.load(serverUrl + "/WebResources/atos_mensajes.js", 
			 serverUrl + "/WebResources/atos_validadocumentos.js", function() 
		{
			var mensaje = validaDocumento(formContext, "atos_tipodocumeto", "atos_numerodocumento", "ERROR");
		}
	);
}

function datosRS(clienteid)
{
	var cols = ["name", "atos_cuentanegociadoraid"];

	var regRS = XrmServiceToolkit.Soap.Retrieve("account", clienteid, cols);
	return regRS;
}


function cambiaCN(cuentanegociadoraid)
{
	var cols = ["atos_name"];
	if ( regCN.attributes["atos_name"] != null )
	{
		Xrm.Page.data.entity.attributes.get("atos_cuentanegociadoraid").setValue(construyeLookup("atos_cuentanegociadora",cuentanegociadoraid, regCN.attributes["atos_name"].value));
		Xrm.Page.getAttribute("atos_cuentanegociadoraid").setSubmitMode("always");
	}
}


function cambiaRS()
{
	if ( Xrm.Page.data.entity.attributes.get("parentcustomerid").getValue() != null )
	{
		var regRS = datosRS(Xrm.Page.data.entity.attributes.get("parentcustomerid").getValue()[0].id);
		if (regRS.attributes["atos_cuentanegociadoraid"] != null)
		{
			cambiaCN(regRS.attributes["atos_cuentanegociadoraid"].id);
		}
	}
}


function cambiaInst()
{
	if ( Xrm.Page.data.entity.attributes.get("atos_instalacionid").getValue() != null )
	{
		var cols = ["atos_razonsocialid"];

		var regIns = XrmServiceToolkit.Soap.Retrieve("atos_instalacion", Xrm.Page.data.entity.attributes.get("atos_instalacionid").getValue()[0].id, cols);
	
		if (regIns.attributes["atos_razonsocialid"] != null)
		{
			var regRS = datosRS(regIns.attributes["atos_razonsocialid"].id);
			if ( regRS.attributes["name"] != null )
			{
				Xrm.Page.data.entity.attributes.get("parentcustomerid").setValue(construyeLookup("account",regIns.attributes["atos_razonsocialid"].id, regRS.attributes["name"].value));
				//Xrm.Page.data.entity.attributes.get("parentcustomerid").setValue(regIns.attributes["atos_razonsocialid"].id);
				Xrm.Page.getAttribute("parentcustomerid").setSubmitMode("always");
			}
			
			if (regRS.attributes["atos_cuentanegociadoraid"] != null)
			{
				cambiaCN(regRS.attributes["atos_cuentanegociadoraid"].id);
			}
			
		}
	}

}