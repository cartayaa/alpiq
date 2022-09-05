/**
// <summary>
// Se ejecuta en el OnChange del campo Documento para validarlo.
// </summary>
// <remarks>
// Carga los javascripts necesarios:
// -# atos_mensajes.js
// -# atos_validadocumentos.js
// - Llama a la funcion validaDocumento para que haga la validacion.
// </remarks>
 */
function Documento_OnChange() 
{	
	var serverUrl = Xrm.Page.context.getClientUrl();
	head.load(serverUrl + "/WebResources/atos_mensajes.js", serverUrl + "/WebResources/atos_validadocumentos.js", function() {
		var mensaje = validaDocumento("atos_tipodocumento", "atos_numerodocumento", "CAMPO");
	});
}