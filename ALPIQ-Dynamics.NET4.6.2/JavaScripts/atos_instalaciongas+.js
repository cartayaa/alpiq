/*
 File="atos_instalaciogas.js" 
 Copyright (c) Atos. All rights reserved.

<summary>
 Funciones asociadas al manejo de instalacion Gas
</summary>

<remarks>
 Fecha 		Codigo  Version Descripcion								 		Autor
 23.03.2021  22323          Corecciones Instalacion de Gas                  ACR
 23.03.2021  22323          Documentacion                                   ACR  
</remarks>                       
*/


/**
// <summary>
// Valida el CUPS 20/22 comprobando tambiï¿½n si hay duplicados.
// </summary>
// <param name="TipoCups">Tipo de CUPS. (20 o 22)</param>
// <remarks>
// - La validaciï¿½n de formato la hace llamando a ValidarCUPS de atos_cups.js
// - El control de duplicados lo hace llamando a controlDuplicados de atos_duplicados.js
// </remarks>
*/
/*function validaCups(TipoCups, campoCups) {
    //var campoCups = "atos_cups" + TipoCups;
    var valido = true;
    limpiaMensajeError("1", "");

    var cups = formContext.getAttribute(campoCups).getValue();
    if (cups != null && cups != "") {
        if (TipoCups == "22" && cups.length == 20) {
            var cups20 = formContext.getAttribute("atos_cups20").getValue();
            if (cups20 != "" && cups20 != null && cups20 == cups) {
                // Admitimos cups22 de longitud 20 si coincide con el cups20.
                return valido;
            }
        }
        valido = ValidarCUPS(cups, TipoCups);
        if (valido == false) {
            mensajeError("El valor introducido no es un CUPS" + TipoCups + " vï¿½lido", "ERROR", "1", campoCups);
        }
        else if (TipoCups == "20") {
            duplicado = controlDuplicadosCA("atos_instalacion", "atos_cups20", cups, "atos_instalacionid", "statecode", "<condition attribute='statecode' operator='eq' value='0' />");
            //if ( duplicado == 0 || duplicado == 1 ) // No se tienen en cuenta las inactivas por el histï¿½rico de instalaciones
            if (duplicado == 0) {
                valido = false;
                var msgestado = "";
                if (duplicado == 0)
                    msgestado = "activa";
                else
                    msgestado = "inactiva";
                mensajeError("Ya existe una cuenta " + msgestado + " con ese CUPS en el sistema.", "ERROR", "1", campoCups);
            }
        }
        else if (TipoCups == "22") {
            var campoCups20 = "atos_cups20";
            var cups20 = formContext.getAttribute(campoCups20).getValue();
            if (cups20 != "" && cups20 != null && cups20 != cups.substr(0, 20)) {
                formContext.getAttribute(campoCups).setValue("");
                formContext.getAttribute(campoCups).setSubmitMode("always");
                mensajeError("El CUPS 22 (" + cups + ") no coincide con el CUPS 20 (" + cups20 + ")", "WARNING", "1", "atos_cups22");
            }
            else if (cups20 == "" || cups20 == null) {
                formContext.getAttribute(campoCups20).setValue(cups.substr(0, 20));
                formContext.getAttribute(campoCups20).setSubmitMode("always");
                valido = validaCups("20", "atos_cups20");
            }
        }
    }
    return valido;
}*/


/**
// <summary>
// Funciï¿½n que se ejecuta en el On Change de los campos CUPS para hacer la validaciï¿½n.
// </summary>
// <param name="TipoCups">Tipo de CUPS. (20 o 22)</param>
// <remarks>
// Carga los javascripts necesarios:
// -# atos_mensajes.js
// -# atos_cups.js
// -# atos_json2.js
// -# atos_jquery.js
// -# atos_XrmServiceToolkit.js
// -# atos_duplicados.js
// - Finalmente llama a la funciï¿½n validaCups
// </remarks>
*/
/*function cups_OnChange(TipoCups, CampoCups) {
    var serverUrl = Xrm.Page.context.getClientUrl();

    head.load(serverUrl + "/WebResources/atos_mensajes.js", serverUrl + "/WebResources/atos_cups.js", serverUrl + "/WebResources/atos_json2.js", serverUrl + "/WebResources/atos_jquery.js", serverUrl + "/WebResources/atos_XrmServiceToolkit.js", serverUrl + "/WebResources/atos_duplicados.js", function () {
        validaCups(TipoCups, CampoCups);
    });
}*/


/**
// <summary>
// Se ejecuta al guardar los datos de la instalaciï¿½n.
// </summary>
// <remarks>
// Si se ha modificado la tarifa, datos de direcciï¿½n de instalaciï¿½n, potencia contratada o tipo de exencion ie pero no se ha cambiado la fecha de inicio de vigencia se muestra un mensaje de confirmaciï¿½n.
// </remarks>
*/
/*function comprueba_cambiavigencia() {
    continuar = true;

    if (formContext.getAttribute("atos_instalacioncreadaenems").getValue() == true) {
        if ((formContext.getAttribute("atos_tarifaid").getIsDirty() == true ||
			  formContext.getAttribute("atos_interrumpibilidad").getIsDirty() == true ||
			  formContext.getAttribute("atos_tipoexencionie").getIsDirty() == true ||
			  formContext.getAttribute("atos_potenciacontratada1").getIsDirty() == true ||
			  formContext.getAttribute("atos_potenciacontratada2").getIsDirty() == true ||
			  formContext.getAttribute("atos_potenciacontratada3").getIsDirty() == true ||
			  formContext.getAttribute("atos_potenciacontratada4").getIsDirty() == true ||
			  formContext.getAttribute("atos_potenciacontratada5").getIsDirty() == true ||
			  formContext.getAttribute("atos_potenciacontratada6").getIsDirty() == true ||
			  formContext.getAttribute("atos_cogeneracion").getIsDirty() == true ||
			  formContext.getAttribute("atos_potenciamaxima1").getIsDirty() == true ||
			  formContext.getAttribute("atos_potenciamaxima2").getIsDirty() == true ||
			  formContext.getAttribute("atos_potenciamaxima3").getIsDirty() == true ||
			  formContext.getAttribute("atos_potenciamaxima4").getIsDirty() == true ||
			  formContext.getAttribute("atos_potenciamaxima5").getIsDirty() == true ||
			  formContext.getAttribute("atos_potenciamaxima6").getIsDirty() == true ||
			  formContext.getAttribute("atos_instalaciondireccionconcatenada").getIsDirty() == true ||
			  formContext.getAttribute("atos_instalacioncodigopostalid").getIsDirty() == true ||
			  formContext.getAttribute("atos_instalacionpoblacionid").getIsDirty() == true ||
			  formContext.getAttribute("atos_instalacionmunicipioid").getIsDirty() == true ||
			  formContext.getAttribute("atos_instalacionprovinciaid").getIsDirty() == true ||
			  formContext.getAttribute("atos_instalacionccaaid").getIsDirty() == true ||
			  formContext.getAttribute("atos_instalacionpaisid").getIsDirty() == true ||
			  formContext.getAttribute("atos_oficinacontabledir3").getIsDirty() == true ||
			  formContext.getAttribute("atos_organogestordir3").getIsDirty() == true ||
			  formContext.getAttribute("atos_unidadtramitadoradir3").getIsDirty() == true ||
			  formContext.getAttribute("atos_organoproponentedir3").getIsDirty() == true ||
			  formContext.getAttribute("atos_oficinacontabledescripcion").getIsDirty() == true ||
			  formContext.getAttribute("atos_organogestordescripcion").getIsDirty() == true ||
			  formContext.getAttribute("atos_unidadtramitadoradescripcion").getIsDirty() == true ||
			  formContext.getAttribute("atos_organoproponentedescripcion").getIsDirty() == true) &&
			 formContext.getAttribute("atos_fechainiciovigencia").getIsDirty() == false) {

            Xrm.Utility.confirmDialog("Ha modificado datos que implican la creaciï¿½n de un registro del histï¿½rico pero no ha modificado la fecha de inicio de vigencia,\nï¿½desea continuar?.\n\nSi continï¿½a el registro del histï¿½rico que se genere quedarï¿½ con fechas de vigencia inconsistentes (la fecha de fin anterior a la fecha de inicio).",
				function () {
				    continuar = true;
				},
				function () {
				    continuar = false;
				}
			);
        }
    }
    return continuar;
}*/

/**
// <summary>
// Funciï¿½n que se ejecuta al guardar en el formulario de Instalaciï¿½n.
// </summary>
// <param name="obj">Contexto del objeto. Para poder "abortar" el guardado si no se cumplen las validaciones.</param>
// <remarks>
// - Comprueba si los cups (20/22) son correctos.
// - Comprueba tambiï¿½n si las potencias contratadas son correctas
// - Comprueba tambiï¿½n si las potencias maximas son correctas
// - Si son errï¿½neos no permite guardar los datos
// </remarks>
*/
/*function on_Save(obj) {
    var valido = true;
    var cups20 = formContext.getAttribute("atos_cups20").getValue();
    var cups22 = formContext.getAttribute("atos_cups22").getValue();
    var cupsatr = formContext.getAttribute("atos_codigopuntosolicitudes").getValue();
    if ((cups20 != null && cups20 != "" && formContext.getAttribute("atos_cups20").getIsDirty() == true) ||
	     (cups22 != null && cups22 != "" && formContext.getAttribute("atos_cups22").getIsDirty() == true) ||
	     (cupsatr != null && cupsatr != "" && formContext.getAttribute("atos_codigopuntosolicitudes").getIsDirty() == true)) {

        if (cups20 != null && cups20 != "")
            valido = validaCups("20", "atos_cups20");
        if (valido == true)
            if (cups22 != null && cups22 != "")
                valido = validaCups("22", "atos_cups22");
        if (valido == true)
            if (cupsatr != null && cupsatr != "")
                valido = validaCups("22", "atos_codigopuntosolicitudes");
        if (valido == false)
            if (obj.getEventArgs() != null)
                obj.getEventArgs().preventDefault();
    }


    if (formContext.getAttribute("atos_potenciacontratada1").getIsDirty() == true ||
	     formContext.getAttribute("atos_potenciacontratada2").getIsDirty() == true ||
	     formContext.getAttribute("atos_potenciacontratada3").getIsDirty() == true ||
	     formContext.getAttribute("atos_potenciacontratada4").getIsDirty() == true ||
	     formContext.getAttribute("atos_potenciacontratada5").getIsDirty() == true ||
	     formContext.getAttribute("atos_potenciacontratada6").getIsDirty() == true) {
        valido = true;
        borraMensajesPotencia();
        for (var i = 1; i < 6; i++) {
            if (validaPotenciaAsc(i, "atos_potenciacontratada" + i) == false)
                valido = false;
        }
       // AAC: comentado por el ticket 15781 ( no se quiere validar las potencias al guardar)
       // fecha: 14/07/2016
       // if (valido == false)
       //     if (obj.getEventArgs() != null)
       //         obj.getEventArgs().preventDefault();
    }

    if (formContext.getAttribute("atos_potenciamaxima1").getIsDirty() == true ||
	     formContext.getAttribute("atos_potenciamaxima2").getIsDirty() == true ||
	     formContext.getAttribute("atos_potenciamaxima3").getIsDirty() == true ||
	     formContext.getAttribute("atos_potenciamaxima4").getIsDirty() == true ||
	     formContext.getAttribute("atos_potenciamaxima5").getIsDirty() == true ||
	     formContext.getAttribute("atos_potenciamaxima6").getIsDirty() == true) {
        valido = true;
        borraMensajesPotenciaMaxima();
        for (var i = 1; i < 6; i++) {
            if (validaPotenciaMaximaAsc(i, "atos_potenciamaxima" + i) == false)
                valido = false;
        }
      // AAC: comentado por el ticket 15781 ( no se quiere validar las potencias al guardar) 
      // fecha: 14/07/2016
      //  if (valido == false)
      //      if (obj.getEventArgs() != null)
      //          obj.getEventArgs().preventDefault();
    }

    if (formContext.getAttribute("atos_fechainiciovigencia").getIsDirty() == true ||
		 formContext.getAttribute("atos_fechafinvigencia").getIsDirty() == true) {
        if (compruebaFechas("atos_fechainiciovigencia", "atos_fechafinvigencia", "La fecha de inicio de vigencia debe ser menor o igual que la fecha de fin de vigencia.") != "")
            if (obj.getEventArgs() != null)
                obj.getEventArgs().preventDefault();
    }


    if (formContext.getAttribute("atos_fechainicioexencionie").getIsDirty() == true ||
		 formContext.getAttribute("atos_fechafinexencionie").getIsDirty() == true) {
        if (compruebaFechas("atos_fechainicioexencionie", "atos_fechafinexencionie", "La fecha de inicio de exenciï¿½n debe ser menor o igual que la fecha de fin de exenciï¿½n.", "7") != "")
            if (obj.getEventArgs() != null)
                obj.getEventArgs().preventDefault();
    }

    if (formContext.getAttribute("atos_fechainicioenvioprevisiones").getIsDirty() == true ||
	     formContext.getAttribute("atos_fechafinenvioprevisiones").getIsDirty() == true) {
        if (compruebaFechas("atos_fechainicioenvioprevisiones", "atos_fechafinenvioprevisiones", "La fecha de inicio de envï¿½o previsiones debe ser menor o igual que la fecha de fin de envï¿½o previsiones.", "6") != "")
            if (obj.getEventArgs() != null)
                obj.getEventArgs().preventDefault();
    }

    if (comprueba_cambiavigencia() == false)
    {
        if (obj.getEventArgs() != null)
           obj.getEventArgs().preventDefault();
    }
}*/

/**
// <summary>
// Borra los mensajes de error en la validaciï¿½n de la potencia contratada.
// </summary>
*/
/*function borraMensajesPotencia() {
    for (var i = 1; i <= 6; i++) {
        for (var j = 1; j <= 6; j++) {
            var nivel = 10 * i + j;
            limpiaMensajeError(nivel.toString(), "");
        }
    }
}*/

/**
// <summary>
// Borra los mensajes de error en la validaciï¿½n de la potencia maxima.
// </summary>
*/
/*function borraMensajesPotenciaMaxima() {
    for (var i = 1; i <= 6; i++) {
        for (var j = 1; j <= 6; j++) {
            var nivel = 10 * i + j;
            limpiaMensajeError(nivel.toString(), "");
        }
    }
}*/


/**
// <summary>
// Comprueba que la potencia contratada Pi debe ser menor o igual que todas las Potencias Contratadas "superiores".
// </summary>
// <param name="numpotencia">Nï¿½mero de la potencia que se va a validar (1..6).</param>
// <param name="campo">Nombre del campo de la potencia que se va a validar.</param>
// <remarks>
// Por ejemplo, P3 debe ser menor o igual que P4, P5 y P6.
// </remarks>
*/
/*function validaPotenciaAsc(numpotencia, campo) {
    var potencia = formContext.getAttribute(campo);
    var potenciavalida = true;
    if (potencia.getValue() != null) {
        var valorpotencia = potencia.getValue();
        if (numpotencia < 6) {
            for (var i = numpotencia + 1; i <= 6; i++) {
                var campo_n = "atos_potenciacontratada" + i;
                var potencia_n = formContext.getAttribute(campo_n);
                if (potencia_n.getValue() != null) {
                    var valorpotencia_n = potencia_n.getValue();
                    if (valorpotencia_n < valorpotencia) {
                        potenciavalida = false;
                        var nivel = 10 * numpotencia + i;
                        mensajeError("La Potencia Contratada P" + i + " debe ser mayor o igual que la Potencia Contratada P" + numpotencia, 'ERROR', nivel.toString(), "");
                    }
                }
            }
        }
    }
    return potenciavalida;
}*/

/**
// <summary>
// Comprueba que la potencia contratada Pi debe ser menor o igual que todas las Potencias mï¿½ximas "superiores".
// </summary>
// <param name="numpotencia">Nï¿½mero de la potencia que se va a validar (1..6).</param>
// <param name="campo">Nombre del campo de la potencia que se va a validar.</param>
// <remarks>
// Por ejemplo, P3 debe ser menor o igual que P4, P5 y P6.
// </remarks>
*/
/*function validaPotenciaMaximaAsc(numpotencia, campo) {
    var potencia = formContext.getAttribute(campo);
    var potenciavalida = true;
    if (potencia.getValue() != null) {
        var valorpotencia = potencia.getValue();
        if (numpotencia < 6) {
            for (var i = numpotencia + 1; i <= 6; i++) {
                var campo_n = "atos_potenciamaxima" + i;
                var potencia_n = formContext.getAttribute(campo_n);
                if (potencia_n.getValue() != null) {
                    var valorpotencia_n = potencia_n.getValue();
                    if (valorpotencia_n < valorpotencia) {
                        potenciavalida = false;
                        var nivel = 10 * numpotencia + i;
                        mensajeError("La Potencia maxima P" + i + " debe ser mayor o igual que la Potencia maxima P" + numpotencia, 'ERROR', nivel.toString(), "");
                    }
                }
            }
        }
    }
    return potenciavalida;
}*/


/**
// <summary>
// Comprueba que la potencia contratada Pi debe ser mayor o igual que todas las Potencias Contratadas "inferiores".
// </summary>
// <param name="numpotencia">Nï¿½mero de la potencia que se va a validar (1..6).</param>
// <param name="campo">Nombre del campo de la potencia que se va a validar.</param>
// <remarks>
// Por ejemplo, P3 debe ser mayor o igual que P2 y que P1.
// </remarks>
*/
/*function validaPotenciaDes(numpotencia, campo) {
    var potencia = formContext.getAttribute(campo);
    var potenciavalida = true;
    if (potencia.getValue() != null) {
        var valorpotencia = potencia.getValue();

        if (numpotencia > 1) {
            var valorpotencia = potencia.getValue();
            for (var i = numpotencia - 1; i >= 1; i--) {
                var campo_n = "atos_potenciacontratada" + i;
                var potencia_n = formContext.getAttribute(campo_n);
                if (potencia_n.getValue() != null) {
                    var valorpotencia_n = potencia_n.getValue();
                    if (valorpotencia < valorpotencia_n) {
                        potenciavalida = false;
                        var nivel = 10 * numpotencia + i;
                        mensajeError("La Potencia Contratada P" + numpotencia + " debe ser mayor o igual que la Potencia Contratada P" + i, 'ERROR', nivel.toString(), "");
                    }
                }
            }
        }
    }
    return potenciavalida;
}*/

/**
// <summary>
// Comprueba que la potencia maxima Pi debe ser mayor o igual que todas las Potencias mï¿½ximas "inferiores".
// </summary>
// <param name="numpotencia">Nï¿½mero de la potencia que se va a validar (1..6).</param>
// <param name="campo">Nombre del campo de la potencia que se va a validar.</param>
// <remarks>
// Por ejemplo, P3 debe ser mayor o igual que P2 y que P1.
// </remarks>
*/
/*function validaPotenciaMaximaDes(numpotencia, campo) {
    var potencia = formContext.getAttribute(campo);
    var potenciavalida = true;
    if (potencia.getValue() != null) {
        var valorpotencia = potencia.getValue();

        if (numpotencia > 1) {
            var valorpotencia = potencia.getValue();
            for (var i = numpotencia - 1; i >= 1; i--) {
                var campo_n = "atos_potenciamaxima" + i;
                var potencia_n = formContext.getAttribute(campo_n);
                if (potencia_n.getValue() != null) {
                    var valorpotencia_n = potencia_n.getValue();
                    if (valorpotencia < valorpotencia_n) {
                        potenciavalida = false;
                        var nivel = 10 * numpotencia + i;
                        mensajeError("La Potencia maxima P" + numpotencia + " debe ser mayor o igual que la Potencia maxima P" + i, 'ERROR', nivel.toString(), "");
                    }
                }
            }
        }
    }
    return potenciavalida;
}*/


/**
// <summary>
// Calcula la potencia mï¿½xima contratada.
// </summary>
// <param name="numeroperiodos">Nï¿½mero de periodos</param>
// <remarks>
// Suma las potencias contratadas segï¿½n el nï¿½mero de periodos y con el resultado modifica el campo potencia mï¿½xima contratada
// </remarks>
*/
/*function calculaPotenciaMax(numeroperiodos) {
    if (formContext.getAttribute("atos_potenciacontratada1").getIsDirty() == true ||
	 formContext.getAttribute("atos_potenciacontratada2").getIsDirty() == true ||
	 formContext.getAttribute("atos_potenciacontratada3").getIsDirty() == true ||
	 formContext.getAttribute("atos_potenciacontratada4").getIsDirty() == true ||
	 formContext.getAttribute("atos_potenciacontratada5").getIsDirty() == true ||
	 formContext.getAttribute("atos_potenciacontratada6").getIsDirty() == true) {
        var campobase = "atos_potenciacontratada";
        var maximo = 0.0;
        var haypotencia = false;
        for (var i = 1; i <= numeroperiodos; i++) {
            if (formContext.getAttribute(campobase + i).getValue() != null) {
                haypotencia = true;
                if (maximo < parseFloat(formContext.getAttribute(campobase + i).getValue()))
                    maximo = parseFloat(formContext.getAttribute(campobase + i).getValue());
            }
        }
        if (haypotencia == true) {
            formContext.getAttribute("atos_potenciamaximacontratada").setValue(maximo);
            Xrm.Page.getAttribute("atos_potenciamaximacontratada").setSubmitMode("always");
        }
    }
}*/

/**
// <summary>
// Valida la potencia contratada Pi llamando a validaPotenciaAsc y validaPotenciaDes.
// </summary>
// <param name="numpotencia">Nï¿½mero de la potencia que se va a validar (1..6).</param>
// <remarks>
// Carga antes el javascript necesario (atos_mensajes.js).
// </remarks>
*/
/*function validaPotencia(numpotencia) {

    var serverUrl = Xrm.Page.context.getClientUrl();

    head.load(serverUrl + "/WebResources/atos_mensajes.js", function () {
        borraMensajesPotencia();
        var campo = "atos_potenciacontratada" + numpotencia;
        var potenciavalida = true;

        validaPotenciaAsc(numpotencia, campo);
        validaPotenciaDes(numpotencia, campo);


    });
}*/

/**
// <summary>
// Valida la potencia maxima Pi llamando a validaPotenciaMaximaAsc y validaPotenciaMaximaDes.
// </summary>
// <param name="numpotencia">Nï¿½mero de la potencia que se va a validar (1..6).</param>
// <remarks>
// Carga antes el javascript necesario (atos_mensajes.js).
// </remarks>
*/
/*function validaPotenciaMaxima(numpotencia) {

    var serverUrl = Xrm.Page.context.getClientUrl();

    head.load(serverUrl + "/WebResources/atos_mensajes.js", function () {
        borraMensajesPotencia();
        var campo = "atos_potenciamaxima" + numpotencia;
        var potenciavalida = true;

        validaPotenciaMaximaAsc(numpotencia, campo);
        validaPotenciaMaximaDes(numpotencia, campo);

    });
}*/



/*
 * Function from Consumo Enero KWh(Ano) event (instalacion Gas)
 * Function from Consumo Febrero KWh(Ano) event (instalacion Gas)
 * Function from Consumo Marzo KWh(Ano) event (instalacion Gas)
 * Function from Consumo Abril KWh(Ano) event (instalacion Gas)
 * Function from Consumo MAyo KWh(Ano) event (instalacion Gas)
 * Function from Consumo Junio KWh(Ano) event (instalacion Gas)
 * Function from Consumo Julio KWh(Ano) event (instalacion Gas)
 * Function from Consumo Agosto KWh(Ano) event (instalacion Gas)
 * Function from Consumo Semtiembre KWh(Ano) event (instalacion Gas)
 * Function from Consumo Octubre KWh(Ano) event (instalacion Gas)
 * Function from Consumo Noviembre KWh(Ano) event (instalacion Gas)
 * Function from Consumo Diciembre KWh(Ano) event (instalacion Gas)
 * 
 * Calcula el valor del consumo estimado anual a partir de los consumos estimados anuales por periodos.
 * @param {*} formContext 
 */

function acumulaConsumo(executionContext) {

    var formContext = executionContext.getFormContext();

    if (formContext.getAttribute("atos_consumomes1").getIsDirty() == true ||
        formContext.getAttribute("atos_consumomes2").getIsDirty() == true ||
        formContext.getAttribute("atos_consumomes3").getIsDirty() == true ||
        formContext.getAttribute("atos_consumomes4").getIsDirty() == true ||
        formContext.getAttribute("atos_consumomes5").getIsDirty() == true ||
        formContext.getAttribute("atos_consumomes6").getIsDirty() == true ||
        formContext.getAttribute("atos_consumomes7").getIsDirty() == true ||
        formContext.getAttribute("atos_consumomes8").getIsDirty() == true ||
        formContext.getAttribute("atos_consumomes9").getIsDirty() == true ||
        formContext.getAttribute("atos_consumomes10").getIsDirty() == true ||
        formContext.getAttribute("atos_consumomes11").getIsDirty() == true ||
        formContext.getAttribute("atos_consumomes12").getIsDirty() == true) 
    {
        if (formContext.getAttribute("atos_consumomes1").getValue() != null ||
            formContext.getAttribute("atos_consumomes2").getValue() != null ||
            formContext.getAttribute("atos_consumomes3").getValue() != null ||
            formContext.getAttribute("atos_consumomes4").getValue() != null ||
            formContext.getAttribute("atos_consumomes5").getValue() != null ||
            formContext.getAttribute("atos_consumomes6").getValue() != null ||
            formContext.getAttribute("atos_consumomes7").getValue() != null ||
            formContext.getAttribute("atos_consumomes8").getValue() != null ||
            formContext.getAttribute("atos_consumomes9").getValue() != null ||
            formContext.getAttribute("atos_consumomes10").getValue() != null ||
            formContext.getAttribute("atos_consumomes11").getValue() != null ||
            formContext.getAttribute("atos_consumomes12").getValue() != null) 
        {

            var total = 0.0;
            for (var i = 1; i <= 12; i++) {
                var campo = "atos_consumomes" + i;
                if (formContext.getAttribute(campo).getValue() != null) {
                    total = parseFloat(total + parseFloat(formContext.getAttribute(campo).getValue()));
                }
            }
            // total = parseFloat(total / 1000.0); // No se transforma porque el total va en las mismas unidades que los periodos
            formContext.getAttribute("atos_consumoanual").setValue(total);
            formContext.getAttribute("atos_consumoanual").setSubmitMode("always");
        }
    }
}

/**
// <summary>
// Habilita/Deshabilita los consumos 0 potencionas por periodos segï¿½n el nï¿½mero de periodos recibidos.
// </summary>
// <param name="campobase">atos_potenciacontratada o atos_consumoestimadoanual</param>
// <param name="numeroperiodos">Nï¿½mero de periodos</param>
// <param name="requerido">Indica si los campos habilitados son obligatorios ("required") u opcionales ("none")</param>
// <remarks>
// - Habilita todos los campos periodo (campobase + i, siendo i desde 1 hasta el nï¿½mero de periodos recibido) inferiores al nï¿½mero de periodos recibidos
// - Deshabilita todos los campos periodo (campobase + i, siendo i desde numero de periodos recibido + 1 hasta 6) mayores que el nï¿½mero de periodos recibidos
// </remarks>
*/
/*function habilitaPeriodos(campobase, numeroperiodos, requerido) {
    for (var i = 1; i <= numeroperiodos; i++) {
        Xrm.Page.getControl(campobase + i).setDisabled(false);
        formContext.getAttribute(campobase + i).setRequiredLevel(requerido);
    }
    for (var i = numeroperiodos + 1; i <= 6; i++) {
        Xrm.Page.getControl(campobase + i).setDisabled(true);
        formContext.getAttribute(campobase + i).setRequiredLevel("none");
        if (formContext.getAttribute(campobase + i).getValue() != null &&
		     formContext.getAttribute(campobase + i).getValue() != "") {
            formContext.getAttribute(campobase + i).setValue(null);
            Xrm.Page.getAttribute(campobase + i).setSubmitMode("always");
        }
    }
}*/

/**
// <summary>
// Habilita/Deshabilita potencias maximas por periodos segï¿½n el nï¿½mero de periodos recibidos y de si es cogeneraciï¿½n o no
// </summary>
// <param name="campobase">atos_potenciamaxima
// <param name="numeroperiodos">Nï¿½mero de periodos</param>
// <param name="requerido">Indica si los campos habilitados son obligatorios ("required") u opcionales ("none")</param>
// <remarks>
// - Habilita todos los campos periodo (campobase + i, siendo i desde 1 hasta el nï¿½mero de periodos recibido) inferiores al nï¿½mero de periodos recibidos
// - Deshabilita todos los campos periodo (campobase + i, siendo i desde numero de periodos recibido + 1 hasta 6) mayores que el nï¿½mero de periodos recibidos
// </remarks>
*/
/*function habilitaPeriodosPotenciaMaxima(campobase, numeroperiodos, requerido) {
    if (formContext.getAttribute("atos_cogeneracion").getValue() == true) {
        for (var i = 1; i <= numeroperiodos; i++) {
            Xrm.Page.getControl(campobase + i).setDisabled(false);
            formContext.getAttribute(campobase + i).setRequiredLevel(requerido);
        }
        for (var i = numeroperiodos + 1; i <= 6; i++) {
            Xrm.Page.getControl(campobase + i).setDisabled(true);
            formContext.getAttribute(campobase + i).setRequiredLevel("none");
            formContext.getAttribute(campobase + i).setValue(null);
            Xrm.Page.getAttribute(campobase + i).setSubmitMode("always");
        }
    }
    else
        for (var i = 1; i <= 6; i++) {
            Xrm.Page.getControl(campobase + i).setDisabled(true);
            formContext.getAttribute(campobase + i).setRequiredLevel("none");
            // #16956 Ini
            //formContext.getAttribute(campobase + i).setValue(null);
            //Xrm.Page.getAttribute(campobase + i).setSubmitMode("always");
            // #16956 Fin
        }

}*/



/**
// <summary>
// Habilita/Deshabilita los consumos y potencionas por periodos segï¿½n el nï¿½mero de periodos de la tarifa.
// </summary>
// <remarks>
// Carga los javascripts necesarios:
// -# atos_json2.js
// -# atos_jquery.js
// -# atos_XrmServiceToolkit.js
// - Recupera el nï¿½mero de periodos de la tarifa mediante XrmServiceToolkit.Soap.Retrieve
// - Llama a la funciï¿½n habilitaPeriodos para las potencias contratadas
// - Si el campo Informacion de Consumos no es Por periodos considera que el nï¿½mero de periodos es 0 (para deshabilitar todos los consumos)
// - Llama a la funciï¿½n habilitaPeriodos para los consumos estimados
// </remarks>
*/
/*function periodosSegunTarifa() {
    var tarifaid = formContext.getAttribute("atos_tarifaid").getValue();
    if (tarifaid != null) {
        var serverUrl = Xrm.Page.context.getClientUrl();

        head.load(serverUrl + "/WebResources/atos_json2.js", serverUrl + "/WebResources/atos_jquery.js", serverUrl + "/WebResources/atos_XrmServiceToolkit.js", function () {
            var recNumeroPeriodos = XrmServiceToolkit.Soap.Retrieve("atos_tarifa", tarifaid[0].id, ["atos_numeroperiodos"]);
            var nperiodos = 0;
            if (recNumeroPeriodos.attributes["atos_numeroperiodos"].value != null)
                nperiodos = parseInt(recNumeroPeriodos.attributes["atos_numeroperiodos"].value);
            habilitaPeriodos("atos_potenciacontratada", nperiodos, "none"); // las potencias contratadas no son obligatorias
            habilitaPeriodosPotenciaMaxima("atos_potenciamaxima", nperiodos, "none"); // las potencias maximas no son obligatorias
            calculaPotenciaMax(nperiodos);
            if (formContext.getAttribute("atos_informaciondeconsumos").getValue() != null)
                if (formContext.getAttribute("atos_informaciondeconsumos").getValue() != 300000001)
                    nperiodos = 0;
            habilitaPeriodos("atos_consumoestimadoanual", nperiodos, "required");  // los consumos son obligatorias
            acumulaConsumo();
        });
    }
}*/


/**
// <summary>
// Deshabilita los consumos por periodos si el campo informacion de consumos no toma el valor Por Periodos.
// </summary>
// <remarks>
// Vacï¿½a tantos los campos por periodo como el total con cualquier cambio en el campo de informaciï¿½n de consumos.
// </remarks>
*/
/*function consumosporperiodos() {

    var informacionconsumos = formContext.getAttribute("atos_informaciondeconsumos");
    if (informacionconsumos.getValue() != null) {
        var disabled = true;
        var disabledtotal = false;
        if (informacionconsumos.getValue() == 300000001) {
            disabled = false;
            disabledtotal = true;
        }

        Xrm.Page.getControl("atos_consumoestimadototalanual").setDisabled(disabledtotal);
        for (var i = 1; i <= 6; i++) {
            Xrm.Page.getControl("atos_consumoestimadoanual" + i).setDisabled(disabled);
        }

        periodosSegunTarifa();

        if (formContext.getAttribute("atos_informaciondeconsumos").getIsDirty() == true) {
            formContext.getAttribute("atos_consumoestimadototalanual").setValue(null);
            Xrm.Page.getAttribute("atos_consumoestimadototalanual").setSubmitMode("always");

            for (var i = 1; i <= 6; i++) {
                formContext.getAttribute("atos_consumoestimadoanual" + i).setValue(null);
                Xrm.Page.getAttribute("atos_consumoestimadoanual" + i).setSubmitMode("always");
            }
        }
    }


}*/

function getId() { return formContext.data.entity.getId(); }
function IsEmptyId() { if (getId() == null || getId() == "") return true; else return false; }  

/*
 * Funcion de Notificacion con Tecmporizador 6 sec
 * 
 * @param {*} msg 
 * @param {*} level 
 * @param {*} id 
 */
function setFormNotification6s(msg, level, id) {

    var notificationTime = 6000;

    formContext.ui.setFormNotification(msg, level, id); 
    setTimeout(
        function () {
            formContext.ui.clearFormNotification(id);
        },
        notificationTime
    );
}


/*
 * Function from Button "Contrato" Ribbon "Instalacion Gas"
 * 
 * Abre el formulario de nueva oferta
 * 
 * @param {*} formContext 
 */
function nuevaOferta(formContext) {
debugger;

// 22323-1 begin 
    if (formContext.data.entity.getIsDirty()) {      
        setFormNotification6s("Debe almacenar primero la instalación.", "ERROR", "1"); 
        return;                                         
    }

    if (formContext.data.getIsDirty()) {
        setFormNotification6s("Debe almacenar primero la instalación.", "ERROR", "1"); 
        return;
    }

    if (!formContext.data.entity.isValid()) {
        setFormNotification6s("Debe almacenar primero la instalación.", "ERROR", "1"); 
        return;
    }

    // if (formContext._entityReference.id["guid"] != null && formContext._entityReference.id["guid"] != "") 
    if (!IsEmptyId())     
    {
// 22323-1 end        
        var entityFormOptions = {};
        entityFormOptions["entityName"] = "atos_oferta";

        var formParameters = {};
        formParameters["llamado_desde"] = "Instalacion Gas";
        formParameters["llamante_id"] =  getId();  /* 22323 */ // formContext._entityReference.id["guid"];
        /* Incorporar nuevos parametros */


        Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
            function (success) {
                console.log(success);
            }, 
            function (error) {
                console.log(error);
        });
    }
}



/**
// <summary>
// Al entrar al formulario muestra un aviso si la interfaz con EMS ha devuelto un KO
// </summary>
// <remarks>
// Esta funciï¿½n no aplica en nueva instalaciï¿½n.<br/>
// Si el campo interfaz instalacion con EMS estï¿½ con el valor KO muestra un mensaje de aviso indicando que la ï¿½ltima ejecuciï¿½n de WS de Instalaciï¿½n con EMS ha finalizado con errores.<br/>
// Si el campo interfaz consumo con EMS estï¿½ con el valor KO muestra un mensaje de aviso indicando que la ï¿½ltima ejecuciï¿½n de WS de Consumo con EMS ha finalizado con errores.
// </remarks>
*/
/*function WarningEMS() {

    if (Xrm.Page.data.entity.getId() != null && Xrm.Page.data.entity.getId() != "") {
        if (formContext.getAttribute("atos_interfazinstalacionems").getValue() == 300000000) // KO EMS
        {
            Xrm.Page.ui.setFormNotification("La ï¿½ltima ejecuciï¿½n del WS de Instalaciï¿½n con EMS ha finalizado con errores", "WARNING", "1");
        }
        if (formContext.getAttribute("atos_interfazconsumoems").getValue() == 300000000) // KO EMS
        {
            Xrm.Page.ui.setFormNotification("La ï¿½ltima ejecuciï¿½n del WS de Consumo con EMS ha finalizado con errores", "WARNING", "2");
        }
    }
}*/



/**
// <summary>
// Inicializa la fecha fin de envï¿½o previsiones a partir de la fecha inicio envï¿½o previsiones
// </summary>
// <remarks>
// Si la fecha inicio envï¿½o previsiones tiene valor y la fecha de fin envï¿½o previsiones estï¿½ vacï¿½a, rellena esta ï¿½ltima con el resultado de sumar 12 meses a la fecha de inicio.
// </remarks>
*/
/*function fechaFinEnvioPrevisiones() {
    if (formContext.getAttribute("atos_fechainicioenvioprevisiones").getValue() != null &&
	    formContext.getAttribute("atos_fechafinenvioprevisiones").getValue() == null) {
        formContext.getAttribute("atos_fechafinenvioprevisiones").setValue(sumaMesesFecha(formContext.getAttribute("atos_fechainicioenvioprevisiones").getValue(), 12));
        formContext.getAttribute("atos_fechafinenvioprevisiones").setSubmitMode("always");
    }
}*/


/**
// <summary>
// Obligatoriedad de campos en funciï¿½n del tipo de documento ATR
// </summary>
// <remarks>
// Si es un CIF es obligatorio el campo sociedad titular atr<br/>
// Si es un NIF son obligatorios los campos nombre y primer apellido del titular atr
// </remarks>
*/
/*function TipoDocumentoATR_OnChange() {

    var tipoDocumento = formContext.getAttribute("atos_tipodocumentotitularatr");

    if (tipoDocumento.getValue != null) {
        if (tipoDocumento.getText() == "CIF") {
            formContext.getAttribute("atos_sociedadtitularatr").setRequiredLevel("required");
            formContext.getAttribute("atos_nombretitularatr").setRequiredLevel("none");
            formContext.getAttribute("atos_primerapellidotitularatr").setRequiredLevel("none");
        }
        else if (tipoDocumento.getText() == "NIF") {
            formContext.getAttribute("atos_nombretitularatr").setRequiredLevel("required");
            formContext.getAttribute("atos_primerapellidotitularatr").setRequiredLevel("required");
            formContext.getAttribute("atos_sociedadtitularatr").setRequiredLevel("none");
        }
        else {
            formContext.getAttribute("atos_sociedadtitularatr").setRequiredLevel("none");
            formContext.getAttribute("atos_nombretitularatr").setRequiredLevel("none");
            formContext.getAttribute("atos_primerapellidotitularatr").setRequiredLevel("none");
        }
    }
}*/



/**
// <summary>
// Inicializa los campos de tipo y nï¿½mero de documento del titular atr.
// </summary>
// <remarks>
// Si el tipo de documento atr estï¿½ vacï¿½o se inicializa con el valor CIF<br/>
// Si el tipo de documento atr es CIF y el nï¿½mero de documento del titular atr estï¿½ vacï¿½o se inicializa con el documento de la razï¿½n social (si es cif)
// </remarks>
*/
/*function CamposDefectoATR() {

    if (Xrm.Page.data.entity.getId() == null || Xrm.Page.data.entity.getId() == "") {
        var tipoDocumento = formContext.getAttribute("atos_tipodocumentotitularatr");

        if (formContext.getAttribute("atos_tipodocumentotitularatr").getValue() == null) {
            formContext.getAttribute("atos_tipodocumentotitularatr").setValue(300000001);
            formContext.getAttribute("atos_tipodocumentotitularatr").setSubmitMode("always");
        }
        if (formContext.getAttribute("atos_tipodocumentotitularatr").getText() == "CIF") {
            if (formContext.getAttribute("atos_numerodocumentotitularatr").getValue() == null ||
			     formContext.getAttribute("atos_sociedadtitularatr").getValue() == null) {
                var serverUrl = Xrm.Page.context.getClientUrl();
                head.load(serverUrl + "/WebResources/atos_json2.js", serverUrl + "/WebResources/atos_jquery.js", serverUrl + "/WebResources/atos_XrmServiceToolkit.js", function () {
                    var recRazonSocial = XrmServiceToolkit.Soap.Retrieve("account", formContext.getAttribute("atos_razonsocialid").getValue()[0].id, ["atos_tipodedocumento", "atos_numerodedocumento", "name"]);
                    if (recRazonSocial.attributes["atos_tipodedocumento"].value != null) {
                        if (recRazonSocial.attributes["atos_tipodedocumento"].value == 300000001 && recRazonSocial.attributes["atos_numerodedocumento"].value != null && formContext.getAttribute("atos_numerodocumentotitularatr").getValue() == null) {
                            formContext.getAttribute("atos_numerodocumentotitularatr").setValue(recRazonSocial.attributes["atos_numerodedocumento"].value);
                            formContext.getAttribute("atos_numerodocumentotitularatr").setSubmitMode("always");
                        }
                        if (recRazonSocial.attributes["atos_tipodedocumento"].value == 300000001 && recRazonSocial.attributes["name"].value != null && formContext.getAttribute("atos_sociedadtitularatr").getValue() == null) {
                            var accountname = recRazonSocial.attributes["name"].value;
                            if (accountname.length > 45)
                                formContext.getAttribute("atos_sociedadtitularatr").setValue(accountname.substring(0, 45));
                            else
                                formContext.getAttribute("atos_sociedadtitularatr").setValue(accountname);
                            formContext.getAttribute("atos_sociedadtitularatr").setSubmitMode("always");
                        }
                    }

                });
            }
        }
    }
}*/

/**
// <summary>
// Se ejecuta en el OnChange del campo Documento para validarlo.
// </summary>
// <remarks>
// Carga los javascripts necesarios:
// -# atos_mensajes.js
// -# atos_validadocumentos.js
// - Llama a la funciï¿½n validaDocumento para que haga la validaciï¿½n.
// </remarks>
*/
/*function Documento_OnChange() {
    var serverUrl = Xrm.Page.context.getClientUrl();
    head.load(serverUrl + "/WebResources/atos_mensajes.js", serverUrl + "/WebResources/atos_validadocumentos.js", function () {
        var mensaje = validaDocumento("atos_tipodocumentotitularatr", "atos_numerodocumentotitularatr", "CAMPO");
    });
}*/

/**
// <summary>
// Se ejecuta en el OnChange del campo cogeneraciï¿½n para mostrar un mensaje si el valor del campo cambia .
// </summary>
// <remarks>
// </remarks>
*/
/*function cogeneracion_OnChange() {
    var cogeneracion = formContext.getAttribute("atos_cogeneracion");

    if (cogeneracion.getValue() != null && cogeneracion.getIsDirty()) {
        Xrm.Utility.confirmDialog("Ha modificado el valor de cogeneraciï¿½n. ï¿½Desea continuar con el cambio?",
                    function () {
                        // Al pulsar OK.
                    },
                    function () {
                        cogeneracion.setValue(Xrm.Page.getAttribute("atos_cogeneracion").getInitialValue());
                        // Al pulsar Cancel.
                    });
    }
}*/

/**
// <summary>
// Se ejecuta en el OnChange del campo interrumpibilidad para mostrar un mensaje si el valor del campo cambia .
// </summary>
// <remarks>
// </remarks>
*/
/*function interrumpibilidad_OnChange() {
    var interrumpibilidad = formContext.getAttribute("atos_interrumpibilidad");

    if (interrumpibilidad.getValue() != null && interrumpibilidad.getIsDirty()) {
        Xrm.Utility.confirmDialog("Ha modificado el valor de interrumpibilidad. ï¿½Desea continuar con el cambio?",
                    function () {
                        // Al pulsar OK.
                    },
                    function () {
                        interrumpibilidad.setValue(Xrm.Page.getAttribute("atos_interrumpibilidad").getInitialValue());
                        // Al pulsar Cancel.
                    });
    }
}*/

/**
// <summary>
// Se ejecuta en el OnChange del campo punto de socorro para mostrar un mensaje si el valor del campo cambia .
// </summary>
// <remarks>
// </remarks>
*/
/*function puntodesocorro_OnChange() {
    var puntodesocorro = formContext.getAttribute("atos_puntodesocorro");

    if (puntodesocorro.getValue() != null && puntodesocorro.getIsDirty()) {
        Xrm.Utility.confirmDialog("Ha modificado el valor de Punto de Socorro. ï¿½Desea continuar con el cambio?",
                    function () {
                        // Al pulsar OK.
                    },
                    function () {
                        puntodesocorro.setValue(Xrm.Page.getAttribute("atos_puntodesocorro").getInitialValue());
                        // Al pulsar Cancel.
                    });
    }
}*/

/**
// <summary>
// Se ejecuta en el OnChange del campo punto esencial para mostrar un mensaje si el valor del campo cambia .
// </summary>
// <remarks>
// </remarks>
*/
/*function puntoesencial_OnChange() {
    var puntoesencial = formContext.getAttribute("atos_puntoesencial");

    if (puntoesencial.getValue() != null && puntoesencial.getIsDirty()) {
        Xrm.Utility.confirmDialog("Ha modificado el valor de Punto Esencial. ï¿½Desea continuar con el cambio?",
                    function () {
                        // Al pulsar OK.
                    },
                    function () {
                        puntoesencial.setValue(Xrm.Page.getAttribute("atos_puntoesencial").getInitialValue());
                        // Al pulsar Cancel.
                    });
    }
}*/

/**
// <summary>
// Se ejecuta en el OnChange del campo aplica neteo de energia para mostrar un mensaje si el valor del campo cambia .
// </summary>
// <remarks>
// </remarks>
*/
/*function aplicaneteodeenergia_OnChange() {
    var aplicaneteodeenergia = formContext.getAttribute("atos_aplicaneteodeenergia");

    if (aplicaneteodeenergia.getValue() != null && aplicaneteodeenergia.getIsDirty()) {
        Xrm.Utility.confirmDialog("Ha modificado el valor de Aplica neteo de energï¿½a. ï¿½Desea continuar con el cambio?",
                    function () {
                        // Al pulsar OK.
                    },
                    function () {
                        aplicaneteodeenergia.setValue(Xrm.Page.getAttribute("atos_aplicaneteodeenergia").getInitialValue());
                        // Al pulsar Cancel.
                    });
    }
}*/

/*
 * Funcion par Borra los mensajes mostrados.
 * 
 * @param {*} formContext 
 * @param {*} nivelAviso 
 * @param {*} campoAviso 
 */
function clearNotificationError(formContext, nivelAviso, campoAviso)
{
	try{
        formContext = formContext.getFormContext();
    }
    catch{ }

	if ( campoAviso != "" )
		formContext.getControl(campoAviso).clearNotification(nivelAviso);
	else 
		formContext.ui.clearFormNotification(nivelAviso);
	
}


/*
 * Function from OnSaveAlpiq 
 * 
 * @param {*} executionContext 
 * @returns 
 */
function validaPorcentajesUsoGas(executionContext) {

    var formContext = executionContext.getFormContext();

    var pctg1 = formContext.getAttribute("atos_porcentajeusodelgas1");
    var pctg2 = formContext.getAttribute("atos_porcentajeusodelgas2");
    var pctg3 = formContext.getAttribute("atos_porcentajeusodelgas3");
    var pctgTotal = 0;
    var pctgValido = true;
    if (pctg1.getValue() != null) {
        pctgTotal = pctgTotal + pctg1.getValue();
    }
    if (pctg2.getValue() != null) {
        pctgTotal = pctgTotal + pctg2.getValue();
    }
    if (pctg3.getValue() != null) {
        pctgTotal = pctgTotal + pctg3.getValue();
    }
    if (pctgTotal !=100) {
        pctgValido = false;
        mensajeError(formContext,"Los porcentajes de uso de gas no son correctos. Han de sumar 100%",'ERROR', "1", "");
    }

    return pctgValido;
}


/*
 * Function from OnSave event
 * Funcion que se ejecuta al guardar en el formulario de Instalacion.
 * 
 * - Comprueba si los cups (20/22) son correctos.
 * - Comprueba tambien si las potencias contratadas son correctas
 * - Comprueba tambien si las potencias maximas son correctas
 * - Si son errï¿½neos no permite guardar los datos

 * @param {*} executionContext 
 */
function OnSaveAlpiq(executionContext) {

    var formContext = executionContext.getFormContext();

/*22323-1*/ // limpiaMensajeError(formContext,"1",""); 
    clearNotificationError(formContext, "1", "");     /*22323-1*/

    if (validaPorcentajesUsoGas(executionContext) == false)
        if (executionContext.getEventArgs() != null)
            executionContext.getEventArgs().preventDefault();
    
}