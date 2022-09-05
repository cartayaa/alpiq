/*
 File="atos_direccionesform.js" 
 Copyright (c) Atos. All rights reserved.

<summary>
 Funciones asociadas al manejo de direcciones en todo el CRM
</summary>

<remarks>
 Fecha 		Codigo  Version Descripcion								 		Autor
 23.06.221   22323          Documentacion                                   ACR  
</remarks>                       
*/


/*
  * Function from "Comunidad Autonona (Instalacion)" event (instalacion Gas-Instalacion Power)
 * Function from "Comunidad Autonona (Facturacion)" event  (instalacion Gas-Instalacion Power)
 * 
 * Rellena los campos relacionados de la dirección.
 * Carga los javascripts necesarios:
 * -# atos_mensajes.js
 * -# atos_json2.js
 * -# atos_jquery.js
 * -# atos_XrmServiceToolkit.js
 * -# atos_direccion.js
 * 
 * Finalmente llama a la función recuperaDireccionCCAA
 *
 * @param {*} executionContext 
 * @param {*} campoccaa - Nombre del campo CCAA en el formulario.
 * @param {*} campopais - Nombre del campo pais en el formulario.
 */
function ccaa_OnChange(executionContext,campoccaa, campopais)
{
	var formContext = executionContext.getFormContext();

	var serverUrl = formContext.context.getClientUrl();
				
	head.load(serverUrl + "/WebResources/atos_mensajes.js", 
	          serverUrl + "/WebResources/atos_json2.js", 
			  serverUrl + "/WebResources/atos_jquery.js", 
			  serverUrl + "/WebResources/atos_XrmServiceToolkit.js", 
			  serverUrl + "/WebResources/atos_direccion.js", function() {
		recuperaDireccionCCAA(formContext,campoccaa, campopais);
	});	
}


/*
 * Function from Provincia event 
 * Function from "Provincia (Instalacion)" event (instalacion Gas-Instalacion Power)
 * 
 * Rellena los campos relacionados de la dirección.
 * Carga los javascripts necesarios:
 * # atos_mensajes.js
 * # atos_json2.js
 * # atos_jquery.js
 * # atos_XrmServiceToolkit.js
 * # atos_direccion.js
 * Finalmente llama a la funciónn recuperaDireccionProvincia
 *
 * @param {*} executionContext 
 * @param {*} campoprovincia - Nombre del campo Provincia en el formulario
 * @param {*} campoccaa - Nombre del campo CCAA en el formulario
 * @param {*} campopais - >Nombre del campo pais en el formulario
 */
function provincia_OnChange(executionContext,campoprovincia, campoccaa, campopais)
{
	var formContext = executionContext.getFormContext();
	var serverUrl = formContext.context.getClientUrl();
				
	head.load(serverUrl + "/WebResources/atos_mensajes.js", 
			  serverUrl + "/WebResources/atos_json2.js", 
			  serverUrl + "/WebResources/atos_jquery.js", 
			  serverUrl + "/WebResources/atos_XrmServiceToolkit.js", 
			  serverUrl + "/WebResources/atos_direccion.js", function() {
		recuperaDireccionProvincia(formContext,campoprovincia, campoccaa, campopais);
	});	
}


/*
 * Function from Municipio event 
 * Function from "Municipio (Instalacion)" event (instalacion Gas-Instalacion Power)
 * 
 * Rellena los campos relacionados de la dirección.
 * Carga los javascripts necesarios:
 * # atos_mensajes.js
 * # atos_json2.js
 * # atos_jquery.js
 * # atos_XrmServiceToolkit.js
 * # atos_direccion.js
 * Finalmente llama a la funciónn recuperaDireccionMunicipio
 * 
 * @param {*} executionContext 
 * @param {*} campomunicipio - Nombre del campo Municipio en el formulario
 * @param {*} campoprovincia - Nombre del campo Provincia en el formulario
 * @param {*} campoccaa - Nombre del campo CCAA en el formulario
 * @param {*} campopais - Nombre del campo pais en el formulario
 */
function municipio_OnChange(executionContext,campomunicipio, campoprovincia, campoccaa, campopais)
{
	var formContext = executionContext.getFormContext();
	var serverUrl = formContext.context.getClientUrl();
				
	head.load(serverUrl + "/WebResources/atos_mensajes.js", 
			  serverUrl + "/WebResources/atos_json2.js", 
			  serverUrl + "/WebResources/atos_jquery.js", 
			  serverUrl + "/WebResources/atos_XrmServiceToolkit.js",
			  serverUrl + "/WebResources/atos_direccion.js", function() {
		recuperaDireccionMunicipio(formContext,campomunicipio, campoprovincia, campoccaa, campopais);
	});	
}


/*
 * Function from Pobracion event 
 * Function from "Poblacion (Instalacion)" event (instalacion Gas-Instalacion Power)
 * 
 * Rellena los campos relacionados de la dirección.
 * Carga los javascripts necesarios:
 * # atos_mensajes.js
 * # atos_json2.js
 * # atos_jquery.js
 * # atos_XrmServiceToolkit.js
 * # atos_direccion.js
 * Finalmente llama a la funciónn recuperaDireccionPoblacion
 * 
 * @param {*} executionContext 
 * @param {*} campopoblacion - Nombre del campo PoblaciÃ³n en el formulario
 * @param {*} campomunicipio - Nombre del campo Municipio en el formulario
 * @param {*} campoprovincia - Nombre del campo Provincia en el formulario
 * @param {*} campoccaa - Nombre del campo CCAA en el formulario
 * @param {*} campopais - Nombre del campo pais en el formulario
 */
function poblacion_OnChange(executionContext,campopoblacion, campomunicipio, campoprovincia, campoccaa, campopais)
{
	var formContext = executionContext.getFormContext();
	var serverUrl = formContext.context.getClientUrl();
				
	head.load(serverUrl + "/WebResources/atos_mensajes.js", 
		      serverUrl + "/WebResources/atos_json2.js", 
			  serverUrl + "/WebResources/atos_jquery.js", 
			  serverUrl + "/WebResources/atos_XrmServiceToolkit.js", 
			  serverUrl + "/WebResources/atos_direccion.js", function() {
		recuperaDireccionPoblacion(formContext,campopoblacion, campomunicipio, campoprovincia, campoccaa, campopais);
	});	
}


/*
 * Function from "Codigo postal" event 
 * Function from "Codigo Postal (Instalacion)" event (instalacion Gas) 
 * Function from "Codigo Postal (Instalacion)" event  (instalacion Gas-Instalacion Power)
 * Function from "Codigo Postal (Facturacion)" event  (instalacion Gas-Instalacion Power)


 * Rellena los campos relacionados de la dirección.
 * Carga los javascripts necesarios:
 * # atos_mensajes.js
 * # atos_json2.js
 * # atos_jquery.js
 * # atos_XrmServiceToolkit.js
 * # atos_direccion.js
 * Finalmente llama a la funciónn recuperaDireccionCPostal
 * 
 * @param {*} executionContext 
 * @param {*} campocpostal 
 * @param {*} campopoblacion 
 * @param {*} campomunicipio 
 * @param {*} campoprovincia 
 * @param {*} campoccaa 
 * @param {*} campopais 
 */
function cpostal_OnChange(executionContext,campocpostal, campopoblacion, campomunicipio, campoprovincia, campoccaa, campopais)
{
	var formContext = executionContext.getFormContext();
	var serverUrl = formContext.context.getClientUrl();
				
	head.load(serverUrl + "/WebResources/atos_mensajes.js", 
			  serverUrl + "/WebResources/atos_json2.js", 
			  serverUrl + "/WebResources/atos_jquery.js", 
			  serverUrl + "/WebResources/atos_XrmServiceToolkit.js", 
			  serverUrl + "/WebResources/atos_direccion.js", function() {
		recuperaDireccionCPostal(formContext,campocpostal, campopoblacion, campomunicipio, campoprovincia, campoccaa, campopais);
	});	
}


/*
 * Construye la dirección concatenada a partir de los campos del formulario.
  *  - Tipo de VÃ­a Calle, NÃºmero, Escalera, Piso, Puerta, PolÃ­gono
 * @param {*} executionContext 
 * @param {*} campotipovia 
 * @param {*} campodireccion 
 * @param {*} camponumero 
 * @param {*} campoportal 
 * @param {*} campoescalera 
 * @param {*} campopiso 
 * @param {*} campopuerta 
 * @param {*} campopoligono 
 * @returns 
 */
function construyeDireccion(executionContext,campotipovia, campodireccion, camponumero,campoportal, campoescalera, campopiso, campopuerta, campopoligono)
{
	var formContext = executionContext.getFormContext();
	var direccionconcatenada="";
	var sep="";
	if ( formContext.data.entity.attributes.get(campotipovia).getValue() != null && formContext.data.entity.attributes.get(campodireccion).getValue() != null)
	{
		direccionconcatenada = formContext.data.entity.attributes.get(campotipovia).getValue()[0].name;
		sep = " ";
	}
	
	if ( formContext.data.entity.attributes.get(campodireccion).getValue() != null )
	{
		direccionconcatenada += sep;
		direccionconcatenada += formContext.data.entity.attributes.get(campodireccion).getValue();
		sep = ", ";
	}
	
	if ( formContext.data.entity.attributes.get(camponumero).getValue() != null )
	{
		direccionconcatenada += sep;
		direccionconcatenada += formContext.data.entity.attributes.get(camponumero).getValue();
		sep = ", ";
	}

if (formContext.data.entity.attributes.get(campoportal).getValue() != null) {
    direccionconcatenada += sep;
    direccionconcatenada += formContext.data.entity.attributes.get(campoportal).getValue();
    sep = ", ";
}


	if ( formContext.data.entity.attributes.get(campoescalera).getValue() != null )
	{
		direccionconcatenada += sep;
		direccionconcatenada += formContext.data.entity.attributes.get(campoescalera).getValue();
		sep = ", ";
	}
	
	if ( formContext.data.entity.attributes.get(campopiso).getValue() != null )
	{
		direccionconcatenada += sep;
		direccionconcatenada += formContext.data.entity.attributes.get(campopiso).getValue();
		sep = ", ";
	}
	
	if ( formContext.data.entity.attributes.get(campopuerta).getValue() != null )
	{
		direccionconcatenada += sep;
		direccionconcatenada += formContext.data.entity.attributes.get(campopuerta).getValue();
		sep = ", ";
	}
	
	if ( formContext.data.entity.attributes.get(campopoligono).getValue() != null )
	{
		direccionconcatenada += sep;
		direccionconcatenada += formContext.data.entity.attributes.get(campopoligono).getValue();
		sep = ", ";
	}
	
	return direccionconcatenada;
}


/*
 * Function from Direccion event (intalacion Gas)
 * Function from "Tipo de Via (Instalacion)" event(instalacion Gas-Instalacion Power)
 * Function from "Tipo de Via (Facturacion)" event (instalacion Gas-Instalacion Power)
 * Function from "Nombre de Via (Instalacion)" event (instalacion Gas-Instalacion Power)
 * Function from "Nombre de Via (Facturacion)" event (instalacion Gas-Instalacion Power)
 * Function from "Km (Instalacion)" event (instalacion Gas) 
 * Function from "Km (Factuacion)" event (instalacion Gas)
 * Function from "Portal (Instalacion)" event (instalacion Gas-Instalacion Power)
 * Function from "Portal (Facturacion)" event (instalacion Gas-Instalacion Power)
 * Function from "Escalera (Instalacion)" event (instalacion Gas-Instalacion Power)
 * Function from "Escalera (Facturacion)" event(instalacion Gas-Instalacion Power)
 * Function from "Piso (Instalacion)" event (instalacion Gas-Instalacion Power)
 * Function from "Piso (Facturacion)" event (instalacion Gas-Instalacion Power)
 * Function from "Puerta (Instalacion)" event (instalacion Gas-Instalacion Power)
 * Function from "Puerta (Factuacion)" event (instalacion Gas-Instalacion Power)
 * Function from "Poligono (Instalacion)" event(instalacion Gas-Instalacion Power)
 * Function from "Poligono (Facturacion)" event (instalacion Gas-Instalacion Power)
  * 
 * Llama a construyeDireccion para actualizar el campo campodireccioncontat
 * 
 * @param {*} executionContext 
 * @param {*} campotipovia 
 * @param {*} campodireccion 
 * @param {*} camponumero 
 * @param {*} campoportal 
 * @param {*} campoescalera 
 * @param {*} campopiso 
 * @param {*} campopuerta 
 * @param {*} campopoligono 
 * @param {*} campodireccionconcat 
 */
function direccion_OnChange(executionContext,campotipovia, campodireccion, camponumero, campoportal, campoescalera, campopiso, campopuerta, campopoligono, campodireccionconcat)
{
	debugger;
	var formContext = executionContext.getFormContext();
	formContext.getControl(campodireccionconcat).clearNotification("2");
	var direccionconcatenada=construyeDireccion(executionContext,campotipovia, campodireccion, camponumero, campoportal, campoescalera, campopiso, campopuerta, campopoligono);

	var tamMax = 150;
	if (direccionconcatenada.length > tamMax)
	{
		formContext.getControl(campodireccionconcat).setNotification("La direcciónn concatenada excede de " + tamMax + " caracteres (" + direccionconcatenada.length + "). Se ha cortado al tamañoo máximo (" + tamMax + ").","2");
		//alert("La direcciÃ³n concatenada excede de " + tamMax + " caracteres (" + direccionconcatenada.length + "). Se ha cortado al tamaÃ±o mÃ¡ximo (" + tamMax + ").");
		direccionconcatenada = direccionconcatenada.substring(0,tamMax);
	}
	
	formContext.data.entity.attributes.get(campodireccionconcat).setValue(direccionconcatenada);
	formContext.getAttribute(campodireccionconcat).setSubmitMode("always");
}



/*
 * 
 * Refresca el iframe de Google Maps para que posicione el mapa en función de la dirección seleccionada.
 * @param {*} executionContext 
 */
function refreshIFrame(executionContext)
{
	var formContext = executionContext.getFormContext();

	var IFrame = formContext.ui.controls.get("IFRAME_GMaps");
	
	var _Url = formContext.context.getClientUrl() + "/WebResources/atos_gmaps.html";
	IFrame.setSrc(null);
	IFrame.setSrc(_Url);
	//IFrame.setSrc(IFrame.getSrc());
}


/*
 * Function from tab "Informacion Direccion y Contacto de Instalacion" event (instalacion Gas)
 * Function from tab "Informacion Direccion y Contacto de Factuacion" event  (instalacion Gas)
 * Function from tab "Informacion Direccion y Contacto de Instalacion" event (Instalacion Power)
 * Function from tab "Informacion Direccion y Contacto de Factuacion" event  (Instalacion Power)
 * Function from "Tipo de Via (Instalacion)" event (instalacion Gas-Instalacion Power)
 * Function from "Tipo de Via (Facturacion)" event (instalacion Gas-Instalacion Power) 
 * Function from "Tipo de Via (Facturacion)" event (Instalacion Power-Instalacion Power) 
 * Function from "Nombre de Via (Instalacion)" event (instalacion Gas-Instalacion Power)
 * Function from "Nombre de Via (Facturacion)" event (instalacion Gas-Instalacion Power)
 * Function from "Km (Instalacion)" event (instalacion Gas-Instalacion Power)
 * Function from "Km (Facturacion)" event (instalacion Gas-Instalacion Power)
 * Function from "Codigo Postal (Instalacion)" event  (instalacion Gas-Instalacion Power)
 * Function from "Codigo Postal (Facturacion)" event  (instalacion Gas-Instalacion Power)
 * Function from "Poblacion (Instalacion)" event (instalacion Gas-Instalacion Power)
 * Function from "Poblacion (Facturacion)" event (instalacion Gas-Instalacion Power)
 * Function from "Municipio (Instalacion)" event (instalacion Gas-Instalacion Power)
 * Function from "Municipio (Facturacion)" event (instalacion Gas-Instalacion Power)
 * Function from "Provincia (Instalacion)" event (instalacion Gas-Instalacion Power)
 * Function from "Provincia (Facturacion)" event (instalacion Gas-Instalacion Power)
 * Function from "Comunidad Autonona (Instalacion)" event (instalacion Gas)  
 * Function from "Comunidad Autonona (Facturacion)" event (instalacion Gas)
 * Function from "Pais (Instalacion)" event (instalacion Gas-Instalacion Power)
 * Function from "Pais (Facturacion)" event (instalacion Gas-Instalacion Power)
 * Function from "Poligono (Instalacion)" event (instalacion Gas-Instalacion Power) 
 * Function from "Poligono (Facturacion)" event (instalacion Gas-Instalacion Power)
 * 
 * Refresca el iframe de Google Maps para que posicione el mapa en funciónn de la dirección seleccionada.
 * @param {*} executionContext 
 * @param {*} nb_iframe 
 * @param {*} nb_html - resourceWeb
 */
function refresh_IFrame(executionContext,nb_iframe, nb_html)
{
	var formContext = executionContext.getFormContext();
	var IFrame = formContext.ui.controls.get(nb_iframe);
	
	var _Url = formContext.context.getClientUrl() + "/WebResources/" + nb_html;
	IFrame.setSrc(null);	
	IFrame.setSrc(_Url);
	//IFrame.setSrc(IFrame.getSrc());
}
