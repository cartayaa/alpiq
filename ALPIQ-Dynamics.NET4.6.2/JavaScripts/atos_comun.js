/**
// <summary>
// Funciones compartidas entre varios formularios
// </summary>
// <remarks>
// Conjunto de funciones que se usan en distintos formularios.<br/>
// Incluyendo este javascript se reutiliza su cÃ³digo.
// </remarks>
 */

debugger;
/**
// <summary>
// Construye un array para dar valor a un campo de tipo lookup.
// </summary>
// <param name="entidad">Nombre de la entidad del campo lookup.</param>
// <param name="valorid">Valor del Gid.</param>
// <param name="valorname">Valor del campo principal (name) del lookup.</param>
// <remarks>
// - Construye un array con una ocurrencia en la que mete un Object con los atributos id, name y entityType
// - Devuelve el array creado.
// </remarks>
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

/**
// <summary>
// Compara dos fechas.
// </summary>
// <param name="fecha1">Primera fecha a comparar.</param>
// <param name="fecha2">Segunda fecha a comparar.</param>
// <remarks>
// - Devuelve 1 si la primera es mayor que la segunda.
// - Devuelve -1 si la primera es menor que la segunda.
// - Devuelve 0 si son iguales.
// </remarks>
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


/**
// <summary>
// Compara fechas prescindiendo de las horas.
// </summary>
// <param name="fecha1">Primera fecha a comparar.</param>
// <param name="fecha2">Segunda fecha a comparar.</param>
// <remarks>
// - Se convierten los dos parÃ¡metros a tipo fecha.
// - Construye las fechas correspondientes a cada parÃ¡metros quitando la hora.
// - Compara las fechas resultantes mediante la funciÃ³n comparafechas.
// </remarks>
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
// <summary>
// Compara fechas teniendo en cuenta las horas.
// </summary>
// <param name="fecha1">Primera fecha a comparar.</param>
// <param name="fecha2">Segunda fecha a comparar.</param>
// <remarks>
// - Se convierten los dos parÃ¡metros a tipo fecha.
// - Compara las fechas resultantes mediante la funciÃ³n comparafechas.
// </remarks>
 */
function comparafechasConHoras(fecha1, fecha2)
{
        var d1 = new Date(fecha1);
        var d2 = new Date(fecha2);
	return comparafechas(d1, d2);
}

 /**
// <summary>
// Refresca el Web Resources del formulario.
// </summary>
// <param name="nb_webresources">Nombre del Web Resources que va a refrescar.</param>
// <param name="nb_html">Nombre del recurso web (html).</param>
 */
function refresh_WebResources(nb_webresources, nb_html)
{
	var WResources = Xrm.Page.ui.controls.get(nb_webresources);
	
	var _Url = Xrm.Page.context.getClientUrl() + "/WebResources/" + nb_html;
	WResources.setSrc(null);
	WResources.setSrc(_Url);
	
}


/**
// <summary>
// Comprueba que la fecha de inicio sea menor o igual que la fecha de fin (segÃºn los campos recibidos por parÃ¡metro)
// </summary>
// <remarks>
// - Modificado para UI - LÃ¡zaro castro
// - Se espera siempre el formContext para poder recoger el valor del atributo pasado
// </remarks>
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



/**
// <summary>
// Accede a la entidad de parÃ¡metros para recuperar los valores (cuenta bancaria)
// </summary>
 */
function parametrosComercializadora()
{
	debugger;
	var fetchXml =
    "<fetch mapping='logical'>"+
	  "<entity name='atos_parametroscomercializadora' > " +
		"<attribute name='atos_cuentabancaria' /> " +
		"<attribute name='atos_name' /> " +
		"<filter>" +
		   "<condition attribute='atos_name' operator='eq' value='ParÃ¡metros Comercializadora' />" +
		"</filter>" +
	  "</entity> " +
	"</fetch>";
	// return XrmServiceToolkit.Soap.Fetch(fetchXml);
	Xrm.WebApi.retrieveMultipleRecords("emails", "?fetchXml=" + fetchXml).then(
		function success(result) {
			return result;
		});

}



/**
// <summary>
// Devuelve el nÃºmero de dÃ­as que tiene el mes-aÃ±o recibido
// </summary>
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


/**
// <summary>
// Suma el nÃºmero de meses recibidos a la fecha
// </summary>
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


/**
// <summary>
// Suma el nÃºmero de dÃ­as recibidos a la fecha
// </summary>
 */
function sumaDiasFecha(fecha, dias)
{
//debugger;
	var d = new Date(fecha);
	var milisegundos = parseInt(parseInt(dias)*24*60*60*1000);
	d.setTime(d.getTime()+milisegundos);
	return d;
}



/**
// <summary>
// Muestra un mensaje de carga
// </summary>
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


/**
// <summary>
// Crea un registro en la entidad atos_trigger
// </summary>
// <param name="accion">Accion para trigger (usado por los plugin para saber que tienen que hacer).</param>
// <param name="entidad">Nombre de la entidad sobre la que debe actuar el plugin.</param>
// <param name="tipomensaje">Si tiene valor (INFO, WARNIN o ERROR) se muestra un mensaje del tipo indicado con el valor del campo ato_respuesta.</param>
// <remarks>
// - Devuelve el valor del campo atos_respuesta de la entidad Trigger
// </remarks>
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


/**
// <summary>
// Comprueba si el contrato recibido por parÃ¡metro es hijo de un multipunto
// </summary>
// <param name="campocontratoid">Nombre del campo contrato del formulario.</param>
// <remarks>
// - Accede al contrato y comprueba si tiene relleno el campo contrato multipunto (devuelve true) o si no lo tiene (devuelve false)
// </remarks>
 */
function esContratoMultipunto(campocontratoid)
{
	if ( Xrm.Page.data.entity.attributes.get(campocontratoid).getValue() != null )
	{
		var contratoid = Xrm.Page.data.entity.attributes.get(campocontratoid).getValue()[0].id;
		var colContrato = [ "atos_contratomultipuntoid" ];
		var regContrato= XrmServiceToolkit.Soap.Retrieve("atos_contrato", contratoid, colContrato);
		if ( regContrato.attributes["atos_contratomultipuntoid"]  != null )
			return true;
	}
	return false;
}



/**
// <summary>
// Provoca que se refresquen los datos del formulario desde el que se llama
// </summary>
// <remarks>
// - Modificado para UI. 19.11.2020. Lazaro Castro
// </remarks>
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


/**
// <summary>
// FunciÃ³n para aÃ±adir filtros a lookup que dependen de otros campos
// </summary>
// <param name="campoHijo">Nombre del campo del lookup en el formulario.</param>
// <param name="campoPadre">Nombre del campo en el formulario del que va a depender campoHijo.</param>
// <param name="campoPadreQuery">Nombre del campo anterior en la query (no tiene por quÃ© coincidir)</param>
 */
function preFiltroLookup(campoHijo, campoPadre, campoPadreQuery) {
	Xrm.Page.getControl(campoHijo).addPreSearch(function () {
		addLookupFiltro(campoHijo, campoPadre, campoPadreQuery);
	});
}


/**
// <summary>
// FunciÃ³n llamada por preFiltroLookup para aÃ±adir filtros a lookup que dependen de otros campos
// </summary>
// <param name="campoHijo">Nombre del campo del lookup en el formulario.</param>
// <param name="campoPadre">Nombre del campo en el formulario del que va a depender campoHijo.</param>
// <param name="campoPadreQuery">Nombre del campo anterior en la query (no tiene por quÃ© coincidir)</param>
 */
function addLookupFiltro(campoHijo, campoPadre, campoPadreQuery) {	
	var attPadre = Xrm.Page.data.entity.attributes.get(campoPadre);
	if (attPadre.getValue()!=null)
	{  
        	fetchXml = "<filter type='and'><condition attribute='" + campoPadreQuery + "' operator='eq' value='" + attPadre.getValue()[0].id + "' /></filter>";
		Xrm.Page.getControl(campoHijo).addCustomFilter(fetchXml);
	}
}
