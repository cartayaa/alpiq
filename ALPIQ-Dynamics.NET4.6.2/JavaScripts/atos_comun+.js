/*
 File="atos_comun.js" 
 Copyright (c) Atos. All rights reserved.

<summary>
 Funciones compartidas entre varios formularios
</summary>

<remarks>
 Fecha 		Codigo  Version Descripcion                                     Autor
 16/03/2021 223231  1.0.1.1 Cambi el FechXml con sentencia no-lock='true'
 10.03.2021  22323      	Lectura condiciones de pago cuando es       	ACR
                            creacion de un contrato desde oferta
 17.06.221   22323          Documentacion                                   ACR
</remarks>                       
*/


/*
 * Construye un array para dar valor a un campo de tipo lookup.
 * @param {*} entidad 
 * @param {*} Guid
 * @param {*} Valor del campo principal (name) del lookup
 * @returns 
 */
function construyeLookup(entidad, valorid, valorname)
{	
	var valor = new Array();
	valor[0] = new Object();
	valor[0].id = valorid;
	valor[0].name = valorname;
	valor[0].entityType = entidad;
	return valor;
}


/*
 * Compara dos fechas.
 * 
 * @param {*} fecha1 
 * @param {*} fecha2 
 * @returns 
 * 
 * Devuelve 1 si la primera es mayor que la segunda.
 * Devuelve -1 si la primera es menor que la segunda.
 * Devuelve 0 si son iguales.
 */
function comparafechas(fecha1, fecha2)
{
        var t1 = fecha1.getTime();
        var t2 = fecha2.getTime();
        if ( t1 > t2 )
                return 1;
        if ( t1 < t2 )
                return -1;
        return 0;
}


/*
 * Compara fechas prescindiendo de las horas.
 * 
 * Se convierten los dos parÃ¡metros a tipo fecha.
 * Construye las fechas correspondientes a cada parÃ¡metros quitando la hora.
 * Compara las fechas resultantes mediante la funciÃ³n comparafechas.
 * @param {*} fecha1 
 * @param {*} fecha2 
 * @returns 
 */
function comparafechasSinHoras(fecha1, fecha2)
{
        var d1aux = new Date(fecha1);
        var d2aux = new Date(fecha2);
        // Para no considerar las horas a la hora de comparar.
        var d1 = new Date(d1aux.getFullYear(), d1aux.getMonth(), d1aux.getDate(), 0, 0, 0);
        var d2 = new Date(d2aux.getFullYear(), d2aux.getMonth(), d2aux.getDate(), 0, 0, 0);
	return comparafechas(d1, d2);
}


/**
 * Compara fechas teniendo en cuenta las horas.
 * 
 * Se convierten los dos parámetros a tipo fecha.
 * Compara las fechas resultantes mediante la función comparafecha
 * @param {*} fecha1 
 * @param {*} fecha2 
 * @returns 
 */
function comparafechasConHoras(fecha1, fecha2)
{
        var d1 = new Date(fecha1);
        var d2 = new Date(fecha2);
	return comparafechas(d1, d2);
}


/*
 * Refresca el Web Resources del formulario.
 * 
 * Nombre del Web Resources que va a refrescar.
 * Nombre del recurso web (html).
 * @param {*} nb_webresources 
 * @param {*} nb_html 
 */
function refresh_WebResources(nb_webresources, nb_html)
{
	var WResources = Xrm.Page.ui.controls.get(nb_webresources);
	
	var _Url = Xrm.Page.context.getClientUrl() + "/WebResources/" + nb_html;
	WResources.setSrc(null);
	WResources.setSrc(_Url);
	
}


/*
 * Function from FechaInicioContrato(atos_contrato) event
 * Function from FechaFinContrato(atos_contrato) event
 * Function from FechaInicioEfectiva(atos_contrato) event
 * Function from FechaFinDefinitiva(atos_contrato) event
 * Function from FechaInicioGarantia(atos_contrato) event
 * Function from FechaFinGarantia(atos_contrato) event
 * Function from FechaInicioAvalDefinitivo(atos_contrato) event
 * Function from FechaFinAvalDefinitivo(atos_contrato) event
 * Function from Fecha Inicio Garantia(atos_oferta) event
 * Function from Fecha Fin Garantia(atos_oferta) event
 * Function from FechaInicioAvalProvisional(atos_oferta) event
 * Function from FechaFinAvalProvisional(atos_oferta) event
 * Function from "Fecha Inicio Vigencia" event (Instalacion Power)
 * Function from "Fecha Fin Vigencia" event (Instalacion Power) 
 * Function from "Fecha Inicio Exencionn I.E" event (Instalacion Power) 
 * Function from "Fecha Fin Exencionn I.E" event (Instalacion Power) 
 * Function from "Fecha Inicio Envio Previsiones" event (Instalacion Power) 
 * Function from "Fecha Fin Envio Previsiones" event (Instalacion Power) 

 * Comprueba que la fecha de inicio sea menor o igual que la fecha de fin (según los campos recibidos por parámetro)
 * Modificado para UI - Lázaro Castro
 * Se espera siempre el formContext para poder recoger el valor del atributo pasado* 
 *
 * @param {formContext} Contexto de ejecucion del formulario
 * @param {campoinicio} Fecha inicio
 * @param {campofin} Fecha Fin
 * @param {mensaje} mensaje a desplegar
 * @param {nivelmensaje} ERROR
 */
function compruebaFechas(formContext, campoinicio, campofin, mensaje, nivelmensaje)
{
    try{
        formContext = formContext.getFormContext();
    }
    catch{
        
    }
	nivelmensaje = (nivelmensaje) ? nivelmensaje : "5";
	var fechainicio = formContext.getAttribute(campoinicio).getValue();
	var fechafin = formContext.getAttribute(campofin).getValue();
	var error = "";
	limpiaMensajeError(formContext, nivelmensaje, "" );
	
	if ( fechainicio != null && fechafin != null )
	{
			
		var res = comparafechasSinHoras(fechainicio, fechafin);
		if ( res == 1 )
		{
			//error = "La fecha de inicio de contrato debe ser menor o igual que la fecha de fin de contrato.";
			error = mensaje;
			mensajeError(formContext,error,'ERROR',nivelmensaje, "");
		}
	
	}
	return error;
}


/*
 *  Accede a la entidad de parámetros para recuperar los valores (cuenta bancaria)
 */
function parametrosComercializadora()
{
	debugger;

	try {																												/*22032021*/
		var fetchXml =
	/* 223231 +1*/ //"<fetch mapping='logical'>"+
		"<fetch no-lock='true'>" +
		"<entity name='atos_parametroscomercializadora' > " +
			"<attribute name='atos_cuentabancaria' /> " +
			"<attribute name='atos_name' /> " +
			"<filter>" +
			"<condition attribute='atos_name' operator='eq' value='Parámetros Comercializadora' />" +
			"</filter>" +
		"</entity> " +
		"</fetch>";

		var statement = "?fetchXml=" + encodeURIComponent(fetchXml);                                                    /*22032021 */
		Xrm.WebApi.retrieveMultipleRecords("emails", statement).then(													/*22032021 */
		function success(result) {
			return result;
		});
    }                                                                                       							/*22032021*/
    catch (error) {                                                                         							/*22032021*/
        console.log("Error cargaDesdeOferta: (" + error.description + ")");                 							/*22032021*/
    }                                                                                       							/*22032021*/
}



/*
 * Devuelve el número de días que tiene el mes-año recibido
 */
function diasmes(mes, anyo)
{
	switch (mes)
	{
		case 1 :
		case 3 :
		case 5 :
		case 7 :	
		case 8 :
		case 10 :
		case 12 :
				return 31;
		case 2 :
			if ( anyo%4 == 0 )
			{
				if ( anyo%100 == 0 && anyo%400 != 0 )
					return 28;
				else
					return 29;
			}
			else
				return 28;
	}
	return 30;
}


/*
 * Suma el número de meses recibidos a la fecha
 */
function sumaMesesFecha(fecha, meses)
{
debugger;
	var d = new Date(fecha);
	var dia = d.getDate();
	var mes = d.getMonth();
	var anyo = d.getFullYear();
	var suma = parseInt(meses + mes);
	mes = parseInt(parseInt(suma % 12));
	//if ( mes == 0 )
		//mes = 12;
	anyo = parseInt(anyo + parseInt(suma / 12));
	if ( dia > 28 )
	{
		dmes = parseInt(diasmes(mes+1, anyo));
		if ( dia > dmes )
			dia = dmes;
	}
	var dsuma = new Date();
	dsuma.setFullYear(anyo, parseInt(mes),dia);
	return dsuma;
	
}


/*
 * Suma el número de dÃ­as recibidos a la fecha
 */
function sumaDiasFecha(fecha, dias)
{
//debugger;
	var d = new Date(fecha);
	var milisegundos = parseInt(parseInt(dias)*24*60*60*1000);
	d.setTime(d.getTime()+milisegundos);
	return d;
}



/*
 * Muestra un mensaje de carga
 */
function muestraMensajeDeCarga() 
{
	//tdAreas.style.display = "none";
	var newdiv = document.createElement("div");
	newdiv.setAttribute("id", "mensajeDiv");
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
	document.all.mensajeDiv.style.visibility = "visible";
}



/*
 * Crea un registro en la entidad atos_trigger
 * 
 * @param {*} accion      - Accion para trigger (usado por los plugin para saber que tienen que hacer)
 * @param {*} entidad     - Nombre de la entidad sobre la que debe actuar el plugin
 * @param {*} tipomensaje - Si tiene valor (INFO, WARNIN o ERROR) se muestra un mensaje del tipo indicado con el valor del campo ato_respuesta
 * @returns 
 * Devuelve el valor del campo atos_respuesta de la entidad Trigger
 */
function creaTrigger(accion, entidad, tipomensaje)
{
	tipomensaje = typeof tipomensaje !== 'undefined' ?  tipomensaje : ' ';
	if ( Xrm.Page.data.entity.getId() != "" && Xrm.Page.data.entity.getId() != null )
	{
		muestraMensajeDeCarga();
		Xrm.Page.ui.setFormNotification("Por favor, espere...",'INFO','2');

		var trigger = new XrmServiceToolkit.Soap.BusinessEntity("atos_trigger");
	
		trigger.attributes["atos_accion"] = accion;
		trigger.attributes["atos_entity"] = entidad;
		trigger.attributes["atos_guid"] = Xrm.Page.data.entity.getId();
		
		try {
			var triggerId = XrmServiceToolkit.Soap.Create(trigger);
			Xrm.Page.ui.clearFormNotification('2');
			document.all.mensajeDiv.style.visibility = 'hidden';
			var colTrigger = [ "atos_respuesta" ];
			var recTrigger= XrmServiceToolkit.Soap.Retrieve("atos_trigger", triggerId, colTrigger);
			if ( recTrigger.attributes["atos_respuesta"] != null )
			{
                              

				if ( tipomensaje == "INFO" || tipomensaje == "WARNING" || tipomensaje == "ERROR" )
					Xrm.Page.ui.setFormNotification(recTrigger.attributes["atos_respuesta"].value,tipomensaje,'1');
				return recTrigger.attributes["atos_respuesta"].value;
			}
			return "";
		}
		catch (err) {
			Xrm.Page.ui.clearFormNotification('2');
			document.all.mensajeDiv.style.visibility = 'hidden';
			//Xrm.Page.ui.setFormNotification(err.message,'ERROR','1');
			alert(err.message);
			return err.message;
		}
	}
	return "";
}



/*
 * Comprueba si el contrato recibido por parámetro es hijo de un multipunto
 * Accede al contrato y comprueba si tiene relleno el campo contrato multipunto (devuelve true) o si no lo tiene (devuelve false)
 * 
 * @param {*} campocontratoid - Nombre del campo contrato del formulario
 * @returns 
 */
function esContratoMultipunto(campocontratoid)
{
	debugger;

	try {																												/*22032021 */
		if ( Xrm.Page.data.entity.attributes.get(campocontratoid).getValue() != null )
		{
			var contratoid = Xrm.Page.data.entity.attributes.get(campocontratoid).getValue()[0].id;
			var colContrato = [ "atos_contratomultipuntoid" ];
/*
			var fetchXml =
				"<fetch no-lock='true'>" +
					"<entity name='atos_parametroscomercializadora' > " +
					"<attribute name='atos_cuentabancaria' /> " +
					"<attribute name='atos_name' /> " +
					"<filter>" +
						"<condition attribute='atos_name' operator='eq' value='Parámetros Comercializadora' />" +
					"</filter>" +
					"</entity> " +
				"</fetch>";

			var statement = "?fetchXml=" + encodeURIComponent(fetchXml);                                                /*22032021 */
			//Xrm.WebApi.retrieveMultipleRecords("emails", statement).then(												/*22032021 */
			/*function success(result) {
				return result;
			});
*/

			var regContrato = XrmServiceToolkit.Soap.Retrieve("atos_contrato", contratoid, colContrato);

			if ( regContrato.attributes["atos_contratomultipuntoid"]  != null )
				return true;
		}
		return false;
    }                                                                                       							/*22032021*/
    catch (error) {                                                                         							/*22032021*/
        console.log("Error esContratoMultipunto: (" + error.description + ")");              							/*22032021*/
    }                                                                                       							/*22032021*/
}



/*
 * Provoca que se refresquen los datos del formulario desde el que se llama
 * Modificado para UI. 19.11.2020. Lazaro Castro
 * @param {*} primaryControl 
 */
function refreshForm(primaryControl){
	//Xrm.Page.data.refresh();
	debugger;

	if (primaryControl._entityReference.id["guid"] != null && primaryControl._entityReference.id["guid"] != "") {
		var entityFormOptions = {};
		entityFormOptions["entityName"] = "atos_oferta";
		// Xrm.Utility.openEntityForm(Xrm.Page.data.entity.getEntityName(), Xrm.Page.data.entity.getId());
		Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
			function (success) {
				// console.log(success);
			}, function (error) {
				Xrm.Navigation.openErrorDialog({ detail: error.message });
			});
	}
	else {
		Xrm.Page.data.refresh();
	}
}


/*
 * Función para añaadir filtros a lookup que dependen de otros campos
 * 
 * @param {*} campoHijo       - Nombre del campo del lookup en el formulario
 * @param {*} campoPadre      - Nombre del campo en el formulario del que va a depender campoHijo
 * @param {*} campoPadreQuery - Nombre del campo anterior en la query (no tiene por que coincidir)
 */
function preFiltroLookup(campoHijo, campoPadre, campoPadreQuery) {

	Xrm.Page.getControl(campoHijo).addPreSearch(function () 
	{
		addLookupFiltro(campoHijo, campoPadre, campoPadreQuery);
	});
}



/*
 * Función llamada por preFiltroLookup para añadir filtros a lookup que dependen de otros campos
 * @param {*} campoHijo       - Nombre del campo del lookup en el formulario
 * @param {*} campoPadre      - Nombre del campo en el formulario del que va a depender campoHijo
 * @param {*} campoPadreQuery - Nombre del campo anterior en la query (no tiene por que coincidir)
 */
function addLookupFiltro(campoHijo, campoPadre, campoPadreQuery) {	
	
	var attPadre = Xrm.Page.data.entity.attributes.get(campoPadre);
	if (attPadre.getValue() != null)
	{  
       	fetchXml = "<filter type='and'> " + 
		           "<condition attribute='" + campoPadreQuery + "' operator='eq' value='" + attPadre.getValue()[0].id + "' /> " + 
				   "</filter>";

		Xrm.Page.getControl(campoHijo).addCustomFilter(fetchXml);
	}
}
