/*
 File="atos_cierre.js" 
 Copyright (c) Atos. All rights reserved.

 Fecha 		Codigo  Version Descripcion
 16/03/2021 223231  1.0.1.1 Cambi el FechXml con sentencia no-lock='true'

*/

/**
// <summary>
// Si se abre la nueva solicitud desde la distribuidora rellena el campo distribuidora de la solicitud
// </summary>
// <remarks>
// Si el formulario llamante tiene un campo subdistribuidora con valor rellena el campo distribuidora de la solicitud con la subdistribuidora<br/>
// Si el formulario llamante no tiene subdistribuidora con valor pero sÃ­ distribuidora rellena el campo distribuidora de la solicitud con la distribuidora
// </remarks>
*/

var formContext; //This code is terrible and redoing this passing context to all functions a mess. A. Ruiz 19/11

async function recuperaDistribuidora(executionContext) {
//This function is currently not going to work, because formContext replacement for formContext is going to be null accessing it through window.
//Also looks like window.top is always null even from subgrid A. Ruiz 19/11
    formContext = executionContext.getFormContext();
    debugger;
    if (formContext.data.entity.getId() == "" || formContext.data.entity.getId() == null) {
        if (window.top.opener != null && window.top.opener.formContext.data != null) {
            var distribuidora = window.top.opener.formContext.data.entity.attributes.get("atos_subdistribuidoraid");
            if (distribuidora == null)
                distribuidora = window.top.opener.formContext.data.entity.attributes.get("atos_distribuidoraid");
            else if (distribuidora.getValue() == null)
                distribuidora = window.top.opener.formContext.data.entity.attributes.get("atos_distribuidoraid");
            if (distribuidora != null) {
                if (distribuidora.getValue() != null) {
                    var valor = new Array();
                    valor[0] = new Object();
                    valor[0].id = distribuidora.getValue()[0].id;
                    valor[0].name = distribuidora.getValue()[0].name;
                    valor[0].entityType = "atos_distribuidora";

                    formContext.data.entity.attributes.get("atos_distribuidoraid").setValue(valor);
                    formContext.getAttribute("atos_distribuidoraid").setSubmitMode("always");

                }
            }
        }
    }
}

/**
// <summary>
// Oculta/muestra las pestaÃ±as correspondientes segÃºn el paso de la solicitud
// </summary>
// <remarks>
// Si el paso es 01 muestra la pestaÃ±a de solicitud y oculta las restantes<br/>
// Si el paso es 02 y tiene motivo de rechazo muestra la pestaÃ±a de rechazo y oculta las restantes<br/>
// Si el paso es 02 y no tiene motivo de rechazo muestra la pestaÃ±a de aceptaciÃ³n y oculta las restantes<br/>
// Si el paso es 05, 06 o 07 muestra la pestaÃ±a de activaciÃ³n y oculta las restantes
// </remarks>p
*/
async function tab_pasos(executionContext) {

    formContext = executionContext.getFormContext();

    formContext.ui.tabs.get('tab_solicitud').setVisible(false);
    formContext.ui.tabs.get('tab_aceptacion').setVisible(false);
    //formContext.ui.tabs.get('tab_rechazo').setVisible(false);          // LA VERSION VIEJA SE OCULTA
    formContext.ui.tabs.get('tab_rechazos').setVisible(false);
   // formContext.ui.tabs.get('tab_incidencias_old').setVisible(false);  // LA VERSION VIEJA SE OCULTA
    formContext.ui.tabs.get('tab_incidencias').setVisible(false);
    formContext.ui.tabs.get('tab_solicitud_anulacion').setVisible(false);
    formContext.ui.tabs.get('tab_aceptacion_anulacion').setVisible(false);
    formContext.ui.tabs.get('tab_activacion').setVisible(false);
    formContext.ui.tabs.get('tab_anulacion_baja').setVisible(false);
    formContext.ui.tabs.get('tab_ficha_oculta').setVisible(false);
    formContext.ui.tabs.get('tab_registro_documentos').setVisible(false);
//    formContext.ui.tabs.get('tab_aceptacion').sections.get('tab_aceptacion_potencias').setVisible(false);
//    formContext.ui.tabs.get('tab_activacion').sections.get('tab_activacion_potencias').setVisible(false);
//    formContext.ui.tabs.get('tab_aceptacion_anulacion').sections.get('tab_anulacion_potencias').setVisible(false);

    if (formContext.data.entity.attributes.get("atos_pasoatrid").getValue() != null) {

        var pasoatr;
        await Xrm.WebApi.retrieveRecord("atos_pasoatr", formContext.data.entity.attributes.get("atos_pasoatrid").getValue()[0].id,"?$select=atos_codigopaso").then(
             function success(record) {
                 pasoatr = record;
             }, function (error) {
                 Xrm.Navigation.openErrorDialog({ detail: error.message });
             });
 
             var procesoatr;
 
             await Xrm.WebApi.retrieveRecord("atos_procesoatr", formContext.data.entity.attributes.get("atos_procesoatrid").getValue()[0].id,"?$select=atos_codigoproceso").then(
                 function success(record) {
                     procesoatr = record;
                 }, function (error) {
                     Xrm.Navigation.openErrorDialog({ detail: error.message });
                 });

        if (pasoatr.atos_codigopaso != null && procesoatr.atos_codigoproceso != null) {
            var pasoatrvalue = pasoatr.atos_codigopaso;
            var procesoatrvalue = procesoatr.atos_codigoproceso;

            procesarPasos(procesoatrvalue, pasoatrvalue);
        }
    }
}

/**
// <summary>
// Procesar el paso necesario segÃºn el valor del parametro 'pasoatrvalue'
// </summary>
*/
 async function procesarPasos(procesoatrvalue, pasoatrvalue) {

    switch (pasoatrvalue) {
        case "01": procesarPaso01(procesoatrvalue); break;
        case "02": procesarPaso02(procesoatrvalue); break;
        case "03": procesarPaso03(procesoatrvalue); break;
        case "04": procesarPaso04(procesoatrvalue); break;
        case "05": procesarPaso05(procesoatrvalue); break;
        case "06": procesarPaso06(procesoatrvalue); break;
        case "07": procesarPaso07(procesoatrvalue); break;
        case "08": procesarPaso08(procesoatrvalue); break;
        case "09": procesarPaso09(procesoatrvalue); break;
        case "10": procesarPaso10(procesoatrvalue); break;
        case "11": procesarPaso11(procesoatrvalue); break;
        case "12": procesarPaso12(procesoatrvalue); break;
        default:
    }
}

/**
// <summary>
// Procesar el paso 01 para todos los procesos
// </summary>
*/
 async function procesarPaso01(procesoatrvalue, pasoatrvalue) {
    formContext.ui.tabs.get('tab_solicitud').setVisible(true);
    formContext.ui.tabs.get('tab_registro_documentos').setVisible(true);


    formContext.getAttribute("atos_duracion").setRequiredLevel("none");
    if (procesoatrvalue == "A3") {
        // se requiere que exista el campo contacto y CNAE
        formContext.getAttribute("atos_cnaeid").setRequiredLevel("required");
        formContext.getAttribute("atos_contactoatrid").setRequiredLevel("required");
        formContext.getAttribute("atos_tipoautoconsumoid").setRequiredLevel("required");
        // se deshabilitad 
        formContext.getAttribute("atos_icpinstaladoycorrectoid").setRequiredLevel("none");
        formContext.getAttribute("atos_instalacionicpid").setRequiredLevel("none");
        formContext.getAttribute("atos_equipoaportadoporclientecomerid").setRequiredLevel("none");
        formContext.getAttribute("atos_instalacionequipomedidaid").setRequiredLevel("none");
        formContext.getAttribute("atos_tipoequipomedidaid").setRequiredLevel("none");

    }
    else if (procesoatrvalue == "B1") {

    }
    else if (procesoatrvalue == "C1") {
        formContext.getAttribute("atos_contratacionincondicionalps").setRequiredLevel("required");
    }
    else if (procesoatrvalue == "C2") {
        // se requiere que exista el campo tipo autoconsumo
        formContext.getAttribute("atos_tipoautoconsumoid").setRequiredLevel("required");

    }
    else if (procesoatrvalue == "M1") {

    }
}
/**
// <summary>
// Procesar el paso 03 para todos los procesos
// </summary>
*/
 async function procesarPaso02(procesoatrvalue, pasoatrvalue) {
    if (subgridTotalRegistros("grid_rechazos") > 0) {
        formContext.ui.tabs.get('tab_rechazos').setVisible(true);
        formContext.ui.tabs.get('tab_registro_documentos').setVisible(true);
    }
    else if (procesoatrvalue == "B1") {
        formContext.ui.tabs.get('tab_aceptacion').setVisible(true);
    }
    else {
        formContext.ui.tabs.get('tab_aceptacion').setVisible(true);
    }
}
/**
// <summary>
// Procesar el paso 03 para todos los procesos
// </summary>
*/
async function procesarPaso03(procesoatrvalue, pasoatrvalue) {
    if (procesoatrvalue == "B1")
        formContext.ui.tabs.get('tab_solicitud_anulacion').setVisible(true);
    else
        formContext.ui.tabs.get('tab_incidencias').setVisible(true);
}
/**
// <summary>
// Procesar el paso 04 para todos los procesos
// </summary>
*/
async function procesarPaso04(procesoatrvalue, pasoatrvalue) {
    if (procesoatrvalue == "B1" && subgridTotalRegistros("grid_rechazos") == 0)
        formContext.ui.tabs.get('tab_aceptacion_anulacion').setVisible(true);
    else {
        formContext.ui.tabs.get('tab_rechazos').setVisible(true);
        formContext.ui.tabs.get('tab_registro_documentos').setVisible(true);
    }
}
/**
// <summary>
// Procesar el paso 05 para todos los procesos
// </summary>
*/
async function procesarPaso05(procesoatrvalue, pasoatrvalue) {
    formContext.ui.tabs.get('tab_activacion').setVisible(true);
}
/**
// <summary>
// Procesar el paso 06 para todos los procesos
// </summary>
*/
async function procesarPaso06(procesoatrvalue, pasoatrvalue) {
    if (procesoatrvalue == "C1" || procesoatrvalue == "C2")
        formContext.ui.tabs.get('tab_activacion').setVisible(true);
    else if (procesoatrvalue == "A3")
        formContext.ui.tabs.get('tab_solicitud_anulacion').setVisible(true);
    else if (procesoatrvalue == "M1")
        formContext.ui.tabs.get('tab_solicitud_anulacion').setVisible(true);
}
/**
// <summary>
// Procesar el paso 07 para todos los procesos
// </summary>
*/
async function procesarPaso07(procesoatrvalue, pasoatrvalue) {
    if (procesoatrvalue == "C2")
        formContext.ui.tabs.get('tab_activacion').setVisible(true);
    else {
        if (subgridTotalRegistros("grid_rechazos") > 0) {
            formContext.ui.tabs.get('tab_rechazos').setVisible(true);
            formContext.ui.tabs.get('tab_registro_documentos').setVisible(true);
        }
        else
            formContext.ui.tabs.get('tab_aceptacion_anulacion').setVisible(true);
    }
}
/**
// <summary>
// Procesar el paso 08 para todos los procesos
// </summary>
*/
async function procesarPaso08(procesoatrvalue, pasoatrvalue) {
    formContext.ui.tabs.get('tab_solicitud_anulacion').setVisible(true);
}
/**
// <summary>
// Procesar el paso 09 para todos los procesos
// </summary>
*/
async function procesarPaso09(procesoatrvalue, pasoatrvalue) {
    if (subgridTotalRegistros("grid_rechazos") > 0) {
        formContext.ui.tabs.get('tab_rechazos').setVisible(true);
        formContext.ui.tabs.get('tab_registro_documentos').setVisible(true);
    }
    else
        formContext.ui.tabs.get('tab_aceptacion_anulacion').setVisible(true);
}
/**
// <summary>
// Procesar el paso 10 para todos los procesos
// </summary>
*/
async function procesarPaso10(procesoatrvalue, pasoatrvalue) {
    formContext.ui.tabs.get('tab_aceptacion').setVisible(true);
}
/**
// <summary>
// Procesar el paso 11 para todos los procesos
// </summary>
*/
async function procesarPaso11(procesoatrvalue, pasoatrvalue) {
    formContext.ui.tabs.get('tab_aceptacion').setVisible(true);
}
/**
// <summary>
// Procesar el paso 12 para todos los procesos
// </summary>
*/
async function procesarPaso12(procesoatrvalue, pasoatrvalue) {
    formContext.ui.tabs.get('tab_rechazos').setVisible(true);
}


/**
// <summary>
// En creaciÃ³n recupera el CNAE de la razÃ³n social de la instalaciÃ³n
// </summary>
*/
async function recupera_cnae(executionContext) {

    formContext = executionContext.getFormContext();

    if (formContext.data.entity.attributes.get("atos_instalacionid").getValue() != null && (formContext.data.entity.getId() == "" || formContext.data.entity.getId() == null)) {
        var fetchXml =
/* 223231 +1*/ //"<fetch mapping='logical'>"+
        "<fetch mapping='logical' no-lock='true'>" +
		  "<entity name='atos_instalacion' > " +
			"<attribute name='atos_razonsocialid' /> " +
			"<link-entity name='account' from='accountid' to='atos_razonsocialid' link-type='outer' alias='rz' > " +
			  "<attribute name='atos_cnaeid' alias='cnaeid' /> " +
			  "<link-entity name='atos_cnae' from='atos_cnaeid' to='atos_cnaeid' link-type='outer' alias='cn' > " +
				"<attribute name='atos_name' alias='cnae' /> " +
			  "</link-entity> " +
			"</link-entity> " +
  			"<filter>" +
			   "<condition attribute='atos_instalacionid' operator='eq' value='" + formContext.data.entity.attributes.get("atos_instalacionid").getValue()[0].id + "' />" +
			"</filter>" +
		  "</entity> " +
		"</fetch>";


        var registros;

        await Xrm.WebApi.retrieveMultipleRecords("atos_instalacion", "?fetchXml=" + fetchXml).then(
            function success(records) {
                if (records != null && records.entities != null && records.entities.length > 0) {
                  registros = records;
            }
            }, function (error) {
                Xrm.Navigation.openErrorDialog({ detail: error.message });
            });


        if (registros.length > 0) {
            if (registros[0].attributes["atos_razonsocialid"] != null && registros[0].attributes["cnaeid"] != null && registros[0].attributes["cnae"] != null) {

                var valorCNAE = new Array();
                valorCNAE[0] = new Object();
                valorCNAE[0].id = registros[0].attributes["cnaeid"].id;
                valorCNAE[0].name = registros[0].attributes["cnae"].value;
                valorCNAE[0].entityType = "atos_cnae";
                formContext.data.entity.attributes.get("atos_cnaeid").setValue(valorCNAE);
                formContext.getAttribute("atos_cnaeid").setSubmitMode("always");
            }
        }
    }
}

/**
// <summary>
// Consulta la subcradicula indicada como parametro de la solicitud para ver si tiene registros
// </summary>
// <remarks>
// </remarks>
*/
async function subgridTotalRegistros(subgrid) {
    var fetchXml = '';
    var totalRegistros = 0;

    if (subgrid = 'grid_rechazos')
    {
        fetchXml =
/* 223231 +1*/ //"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
        "<fetch output-format='xml-platform' mapping='logical' no-lock='true' distinct='false'>" +        
          "<entity name='atos_rechazosolicitud'>" +
            "<attribute name='atos_rechazosolicitudid' />" +
            "<attribute name='atos_name' />" +
            "<attribute name='createdon' />" +
            "<order attribute='atos_name' descending='false' />" +
            "<filter type='and'>" +
              "<condition attribute='atos_solicitudatrid' operator='eq' uiname='' uitype='atos_solicitudatr' value='" + formContext.data.entity.getId() + "' />" +
            "</filter>" +
          "</entity>" +
        "</fetch>";

        await Xrm.WebApi.retrieveMultipleRecords("atos_rechazosolicitud", "?fetchXml=" + fetchXml).then(
            function success(records) {
                if (records != null && records.entities != null && records.entities.length > 0) {
                    totalRegistros = records.entities.length;
            }
            }, function (error) {
                Xrm.Navigation.openErrorDialog({ detail: error.message });
            });


    }
     
    else if (subgrid = 'grid_incidencias'){

        fetchXml =
/* 223231 +1*/ //"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
        "<fetch output-format='xml-platform' mapping='logical'  no-lock='true' distinct='false'>" +        
          "<entity name='atos_incidencia'>" +
            "<attribute name='atos_incidenciaid' />" +
            "<attribute name='atos_name' />" +
            "<attribute name='createdon' />" +
            "<order attribute='atos_name' descending='false' />" +
            "<filter type='and'>" +
              "<condition attribute='atos_solicitudatrid' operator='eq' uiname='' uitype='atos_solicitudatr' value='" + formContext.data.entity.getId() + "' />" +
            "</filter>" +
          "</entity>" +
        "</fetch>";

        await Xrm.WebApi.retrieveMultipleRecords("atos_incidencia", "?fetchXml=" + fetchXml).then(
            function success(records) {
                if (records != null && records.entities != null && records.entities.length > 0) {
                    totalRegistros = records.entities.length;
            }
            }, function (error) {
                Xrm.Navigation.openErrorDialog({ detail: error.message });
            });

    }
      

    return totalRegistros;
}

/**
// <summary>
// Actualiza de 'codigo de solicitud' de la solicitud en pantalla con el nuevo codigo de la 'solicitud asociada' seleccionada
// </summary>
// <remarks>
// </remarks>
*/
async function actualizarCodigoSolicitud() {

    if (formContext.data.entity.attributes.get("atos_solicitudasociadaid").getValue() == null) { return; }

    var lookup = formContext.getAttribute("atos_solicitudasociadaid").getValue();

    if (lookup == null) { return; }

    var codigoSolicitud = lookup[0].keyValues.atos_codigosolicitud.value; /** typed-attribute approach **/

    if (codigoSolicitud == null || codigoSolicitud == "") { return; }

    formContext.data.entity.attributes.get("atos_codigosolicitud").setValue(codigoSolicitud);

    /*
    var codigoSolicitud = lookup[0].keyValues["atos_codigosolicitud"].value;
    var col = ["atos_codigosolicitud"];
    var solicitudatr = XrmServiceToolkit.Soap.Retrieve("atos_solicitudatr", lookup[0].id, col);
    if (solicitudatr.attributes["atos_codigosolicitud"] == null) { return; }
    var test = solicitudatr.attributes.atos_codigosolicitud.value;
    */
}


// <summary>
// Se encarga de anular el guardado de una reclamacion
// </summary>
async function cancelarGuardado(executionContext) {
    if (executionContext.getEventArgs() != null)
    executionContext.getEventArgs().preventDefault();
}



/**
// <summary>
//Se encarga de realizar todas las validaciones de la reclamaciÃ³n segÃºn el tipo,  antes de ejecutar el WF.
// De momento se hacen solo validaciones para las reclamaciones que sean  de envio de peticiones de reclamacion(R1-01) 
// y de envio de informacion adicional (R1-04)
// </summary>
*/
async function solicitudatr_OnSave(executionContext) {

    debugger;
    formContext = executionContext.getFormContext();
    var serverUrl = formContext.context.getClientUrl();
    var continuar = true;
    limpiaMensajeError(formContext,"2", "");
	var pasoatrvalue ;
	var procesoatrvalue ;
	
	 if (formContext.data.entity.attributes.get("atos_pasoatrid").getValue() != null) {

        var pasoatr;
       await Xrm.WebApi.retrieveRecord("atos_pasoatr", formContext.data.entity.attributes.get("atos_pasoatrid").getValue()[0].id,"?$select=atos_codigopaso").then(
            function success(record) {
                pasoatr = record;
            }, function (error) {
                Xrm.Navigation.openErrorDialog({ detail: error.message });
            });

            var procesoatr;

            await Xrm.WebApi.retrieveRecord("atos_procesoatr", formContext.data.entity.attributes.get("atos_procesoatrid").getValue()[0].id,"?$select=atos_codigoproceso").then(
                function success(record) {
                    procesoatr = record;
                }, function (error) {
                    Xrm.Navigation.openErrorDialog({ detail: error.message });
                });
 
        if (pasoatr.atos_codigopaso != null && procesoatr.attributesatos_codigoproceso != null) {
             pasoatrvalue = pasoatr.atos_codigopaso;
             procesoatrvalue = procesoatr.atos_codigoproceso;

        }
    }
	
	// solo se comprueba las validaciones en casos que sea  paso de salida
	if ((pasoatrvalue !="01")
		&& (pasoatrvalue !="06" && procesoatrvalue=="A3" )  
		&& (pasoatrvalue !="04" && procesoatrvalue!="B1" )  
		&& (pasoatrvalue !="08" && procesoatrvalue!="C1" )  
		&& (pasoatrvalue !="08" && procesoatrvalue!="C2" )  
		&& (pasoatrvalue !="06" && procesoatrvalue!="M1" )){
		return;
	}
	
	
    //validamos datos de cie
    if (formContext.data.entity.attributes.get("atos_cieelectronico").getValue() == false) {
        if ((formContext.data.entity.attributes.get("atos_codigocie").getValue() != null
	     || formContext.data.entity.attributes.get("atos_potenciainstalacionbt").getValue() != null
		 || formContext.data.entity.attributes.get("atos_fechaemisioncie").getValue() != null
		 || formContext.data.entity.attributes.get("atos_fechacaducidadcie2").getValue() != null
		 || formContext.data.entity.attributes.get("atos_potencianointerrumpible").getValue() != null
		 || formContext.data.entity.attributes.get("atos_tensionsuministrocieid").getValue() != null
		 || formContext.data.entity.attributes.get("atos_tiposuministrocieid").getValue() != null
		 || formContext.data.entity.attributes.get("atos_instaladorcie2id").getValue() != null
		 )
		 &&
		 (formContext.data.entity.attributes.get("atos_codigocie").getValue() == null
	     || formContext.data.entity.attributes.get("atos_potenciainstalacionbt").getValue() == null
		 || formContext.data.entity.attributes.get("atos_fechaemisioncie").getValue() == null
		 || formContext.data.entity.attributes.get("atos_tensionsuministrocieid").getValue() == null
		 || formContext.data.entity.attributes.get("atos_tiposuministrocieid").getValue() == null
		 || formContext.data.entity.attributes.get("atos_instaladorcie2id").getValue() == null)) {
            mensajeError(formContext,"Es obligatorio introducir Código CIE, Potencia máxima instalación BT, Fecha emisión CIE, tensión de suminstro , tipo de suministro e instalador  para un CIE de papel", "ERROR", "2", "atos_codigocie");
            continuar = false;
        }
    }
    else {
        if ((formContext.data.entity.attributes.get("atos_codigocie").getValue() != null
	     || formContext.data.entity.attributes.get("atos_selloelectronicocie").getValue() != null)
		 &&
		 (formContext.data.entity.attributes.get("atos_codigocie").getValue() == null
		 || formContext.data.entity.attributes.get("atos_selloelectronicocie").getValue() == null)) {
            mensajeError(formContext,"Es obligatorio introducir Código CIE y  Sello electrónico para un CIE electrónico", "ERROR", "2", "atos_codigocie");             
            continuar = false;
        }
    }

    // validamos datos de apm
    if ((formContext.data.entity.attributes.get("atos_codigoapm").getValue() != null
		 || formContext.data.entity.attributes.get("atos_fechaemisionapm").getValue() != null
		 || formContext.data.entity.attributes.get("atos_fechacaducidadapm").getValue() != null
		 || formContext.data.entity.attributes.get("atos_tensionsuministroapmid").getValue() != null
	     || formContext.data.entity.attributes.get("atos_potenciainstalacionat").getValue() != null
		 || formContext.data.entity.attributes.get("atos_instaladorapm2id").getValue() != null)
		 &&
		 (formContext.data.entity.attributes.get("atos_codigoapm").getValue() == null
		 || formContext.data.entity.attributes.get("atos_fechaemisionapm").getValue() == null
		 || formContext.data.entity.attributes.get("atos_tensionsuministroapmid").getValue() == null
		 || formContext.data.entity.attributes.get("atos_potenciainstalacionat").getValue() == null
		 || formContext.data.entity.attributes.get("atos_instaladorapm2id").getValue() == null)) {
        mensajeError(formContext,"Es obligatorio especificar el Código,Fecha de emisión,Tensión del suministro,Potencia instalada AT e Instalador  del equipo de medida,si se introduce algún dato los datos de APM", "ERROR", "2", "atos_codigoapm");             
        continuar = false;
    }
    // validamos datos de medida
    if ((formContext.data.entity.attributes.get("atos_tipoequipomedidaid").getValue() != null
	     || formContext.data.entity.attributes.get("atos_equipoaportadoporclientecomerid").getValue() != null)
		 &&
		 (formContext.data.entity.attributes.get("atos_equipoaportadoporclientecomerid").getValue() == null)) {
        mensajeError(formContext,"Es obligatorio especificar la propiedad del equipo de medida,si se introduce algún dato del la  medida", "ERROR", "2", "atos_equipoaportadoporclientecomerid");             
        continuar = false;
    }

    // validamos datos de aparato
    if ((formContext.data.entity.attributes.get("atos_marcaaparatoid").getValue() != null
	     || formContext.data.entity.attributes.get("atos_tipoaparatoid").getValue() != null)
		 &&
		 (formContext.data.entity.attributes.get("atos_marcaaparatoid").getValue() == null
	     || formContext.data.entity.attributes.get("atos_tipoaparatoid").getValue() == null)) {
        mensajeError(formContext,"Es obligatorio especificar la marca y el tipo de aparato. si se introduce algún dato del aparato de medida", "ERROR", "2", "atos_marcaaparatoid");        
        continuar = false;
    }

    // si  dio algun fallo no continua
    if (continuar == false) {
        cancelarGuardado(executionContext);
    }
    return

}