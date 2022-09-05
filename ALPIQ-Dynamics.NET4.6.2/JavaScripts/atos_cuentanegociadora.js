/**
// <summary>
// Muestra un mensaje de carga.
// </summary>
 */
function showLoadingMessage() {
	//tdAreas.style.display = "none";
	var newdiv = document.createElement("div");
	newdiv.setAttribute("id", "msgDiv");
	newdiv.valign = "middle";
	newdiv.align = "center";
	var divInnerHTML = "<table height='50%' width='50%' style='cursor:wait'>";
	divInnerHTML += "<tr>";
	divInnerHTML += "<td valign='middle' align='center'>";
	divInnerHTML += "<img alt='' src='/_imgs/AdvFind/progress.gif'/>";
	divInnerHTML += "<div/><b>Workingâ€¦</b>";
	divInnerHTML += "</td></tr></table>";
	newdiv.innerHTML = divInnerHTML;
	newdiv.style.background = "#FFFFFF";
	newdiv.style.fontSize = "15px";
	newdiv.style.zIndex = "1010";
	newdiv.style.width = document.body.clientWidth;
	newdiv.style.height = document.body.clientHeight;
	newdiv.style.position = "absolute";
	document.body.insertBefore(newdiv, document.body.firstChild);
	document.all.msgDiv.style.visibility = "visible";
}



/**
// <summary>
// Se ejecuta en el Click del botÃ³n de creaciÃ³n de ofertas.
// </summary>
// <remarks>
// Carga los javascripts necesarios:
// -# atos_json2.js
// -# atos_json2.js
// -# atos_XrmServiceToolkit.js
// - Muestra un mensaje de confirmaciÃ³n para crear las ofertas para la cuenta negociadora y sus instalaciones.
// - Muestra el mensaje de "carga" (showLoadingMessage)
// - Provoca que se ejecute el plugin de creaciÃ³n de ofertas insertando un registro en atos_trigger
// - Oculta el mensaje de "carga"
// - Modificado al UI -  19.11.2020 - LÃ¡zaro Castro
// </remarks>
 */
function CreaOfertasPower_Click(primaryControl) 
{	
	if (primaryControl._entityReference.id["guid"] != null && primaryControl._entityReference.id["guid"] != "") {
			var entityFormOptions = {};
			entityFormOptions["entityName"] = "atos_oferta";
			var formParameters = {};
			formParameters["llamado_desde"] = "Cuenta Negociadora Power";
			formParameters["llamante_id"] = primaryControl._entityReference.id["guid"];
			// Xrm.Utility.openEntityForm("atos_oferta", null, parameters);
			Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
				function (success) {
					// console.log(success);
				}, function (error) {
					Xrm.Navigation.openErrorDialog({ detail: error.message });
				});
		}
	/*
	var serverUrl = Xrm.Page.context.getClientUrl();
	head.load(serverUrl + "/WebResources/atos_json2.js", serverUrl + "/WebResources/atos_jquery.js", serverUrl + "/WebResources/atos_XrmServiceToolkit.js", function() {
		if ( Xrm.Page.data.entity.getId() != null && Xrm.Page.data.entity.getId() != "" )
		{

			var continuar = false;
			Xrm.Utility.confirmDialog("Â¿Desea crear las ofertas para la cuenta negociadora y sus instalaciones?." ,
				function () {
					continuar = true;
				},
				function () {
					continuar = false;
				}
			);
			if ( continuar == true )
			{
				showLoadingMessage();
				Xrm.Page.ui.setFormNotification("Por favor, espere...",'INFO','2');
				var creaOfertas = new XrmServiceToolkit.Soap.BusinessEntity("atos_trigger");
				creaOfertas.attributes["atos_accion"] = "OfertaMP";
				creaOfertas.attributes["atos_entity"] = "atos_cuentanegociadora";
				creaOfertas.attributes["atos_guid"] = Xrm.Page.data.entity.getId();
				var creaOfertasId = XrmServiceToolkit.Soap.Create(creaOfertas);
				Xrm.Page.ui.clearFormNotification('2');
				document.all.msgDiv.style.visibility = 'hidden';
			}
		}
	});	*/
}



/**
// <summary>
// Se ejecuta en el Click del botÃ³n de creaciÃ³n de ofertas.
// </summary>
// <remarks>
// Carga los javascripts necesarios:
// -# atos_json2.js
// -# atos_json2.js
// -# atos_XrmServiceToolkit.js
// - Muestra un mensaje de confirmaciÃ³n para crear las ofertas para la cuenta negociadora y sus instalaciones.
// - Muestra el mensaje de "carga" (showLoadingMessage)
// - Provoca que se ejecute el plugin de creaciÃ³n de ofertas insertando un registro en atos_trigger
// - Oculta el mensaje de "carga"
// - Modificado al UI -  19.11.2020 - LÃ¡zaro Castro
// </remarks>
 */
function CreaOfertasGas_Click(primaryControl) 
{	
	if (primaryControl._entityReference.id["guid"] != null && primaryControl._entityReference.id["guid"] != "") 
	{
		var entityFormOptions = {};
		entityFormOptions["entityName"] = "atos_oferta";
		var formParameters = {};
		formParameters["llamado_desde"] = "Cuenta Negociadora Gas";
		formParameters["llamante_id"] = primaryControl._entityReference.id["guid"];
		// Xrm.Utility.openEntityForm("atos_oferta", null, parameters);
		Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
			function (success) {
				// console.log(success);
			}, function (error) {
				Xrm.Navigation.openErrorDialog({ detail: error.message });
			});
	}
}


function CuentaNegociadora_OnSave(executionContext)
{
	// var serverUrl = Xrm.Page.context.getClientUrl();
	var formContext = executionContext.getFormContext();
	// Xrm.Page.ui.clearFormNotification("5");
	formContext.ui.clearFormNotification("5")

	if (formContext.getAttribute("atos_fechainiciovigencia").getIsDirty() == true ||
		formContext.getAttribute("atos_fechafinvigencia").getIsDirty() == true )
	{	
		if (compruebaFechas(formContext, "atos_fechainiciovigencia", "atos_fechafinvigencia", "La fecha de inicio de vigencia debe ser menor o igual que la fecha de fin de vigencia.") != "" )
			if (executionContext.getEventArgs() != null)
				executionContext.getEventArgs().preventDefault();
	}

}