/*
 File="atos_instalacion.js" 
 Copyright (c) Atos. All rights reserved.

<summary>
 Funciones asociadas al manejo de instalacion Power
</summary>

<remarks>
 Fecha 		Codigo  Version Descripcion								 		Autor
 23.03.2021  22323          Corecciones Instalacion de Gas                  ACR
 23.03.2021  22323          Documentacion                                   ACR  
</remarks>                       
*/

//#region Variables Funciones Globales 22323
var globalContext;
var formContext;
//#endregion Variables Funciones Globales 22323

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
 * Valida el CUPS 20/22 comprobando también si hay duplicados.
 * 
 * - La validación de formato la hace llamando a ValidarCUPS de atos_cups.js
 * - El control de duplicados lo hace llamando a controlDuplicados de atos_duplicados.js
 * @param {*} formContext 
 * @param {*} TipoCups - >Tipo de CUPS. (20 o 22)
 * @param {*} campoCups 
 * @returns 
 */
function validaCups(formContext, TipoCups, campoCups) {
    //var campoCups = "atos_cups" + TipoCups;
    var valido = true;
    limpiaMensajeError(formContext,"1", "");

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
           mensajeError(formContext,"El valor introducido no es un CUPS" + TipoCups + " válido", "ERROR", "1", campoCups);
        }
        else if (TipoCups == "20") {
            duplicado = controlDuplicadosCA(formContext,"atos_instalacion", "atos_cups20", cups, "atos_instalacionid", "statecode", "<condition attribute='statecode' operator='eq' value='0' />");
            //if ( duplicado == 0 || duplicado == 1 ) // No se tienen en cuenta las inactivas por el histórico de instalaciones
            if (duplicado == 0) {
                valido = false;
                var msgestado = "";
                if (duplicado == 0)
                    msgestado = "activa";
                else
                    msgestado = "inactiva";
               mensajeError(formContext,"Ya existe una cuenta " + msgestado + " con ese CUPS en el sistema.", "ERROR", "1", campoCups);
            }
        }
        else if (TipoCups == "22") {
            var campoCups20 = "atos_cups20";
            var cups20 = formContext.getAttribute(campoCups20).getValue();
            if (cups20 != "" && cups20 != null && cups20 != cups.substr(0, 20)) {
                formContext.getAttribute(campoCups).setValue("");
                formContext.getAttribute(campoCups).setSubmitMode("always");
               mensajeError(formContext,"El CUPS 22 (" + cups + ") no coincide con el CUPS 20 (" + cups20 + ")", "WARNING", "1", "atos_cups22");
            }
            else if (cups20 == "" || cups20 == null) {
                formContext.getAttribute(campoCups20).setValue(cups.substr(0, 20));
                formContext.getAttribute(campoCups20).setSubmitMode("always");
                valido = validaCups(formContext,"20", "atos_cups20");
            }
        }
    }
    return valido;
}


/*
 * Function from CUPS event (InstalacionPower)
 * Function from "Codigo Punto Frontera" event (InstalacionPower)
 * 
 * Función que se ejecuta en el On Change de los campos CUPS para hacer la validación.
 * Carga los javascripts necesarios:
 * -# atos_mensajes.js
 * -# atos_cups.js
 * -# atos_json2.js
 * -# atos_jquery.js
 * -# atos_XrmServiceToolkit.js
 * -# atos_duplicados.js
 * Finalmente llama a la función validaCups
 * @param {*} executionContext 
 * @param {*} TipoCups - ipo de CUPS. (20 o 22)
 * @param {*} CampoCups 
 */
function cups_OnChange(executionContext,TipoCups, CampoCups) {

    var formContext = executionContext.getFormContext();
    var serverUrl = formContext.context.getClientUrl();

    head.load(serverUrl + "/WebResources/atos_mensajes.js", 
              serverUrl + "/WebResources/atos_cups.js", 
              serverUrl + "/WebResources/atos_json2.js", 
              serverUrl + "/WebResources/atos_jquery.js",
              serverUrl + "/WebResources/atos_XrmServiceToolkit.js",
              serverUrl + "/WebResources/atos_duplicados.js", function () {
        validaCups(formContext,TipoCups, CampoCups);
    });
}


/**
// <summary>
// Se ejecuta al guardar los datos de la instalación.
// </summary>
// <remarks>
// Si se ha modificado la tarifa, datos de dirección de instalación, potencia contratada o tipo de exencion ie pero no se ha cambiado la fecha de inicio de vigencia se muestra un mensaje de confirmación.
// </remarks>
*/
function comprueba_cambiavigencia(executionContext) {

    var formContext = executionContext.getFormContext();
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
			 formContext.getAttribute("atos_fechainiciovigencia").getIsDirty() == false) 
        {

            Xrm.Utility.confirmDialog("Ha modificado datos que implican la creación de un registro del histórico pero no ha modificado la fecha de inicio de vigencia,\n¿desea continuar?.\n\nSi continúa el registro del histórico que se genere quedará con fechas de vigencia inconsistentes (la fecha de fin anterior a la fecha de inicio).",
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
}



/*
 * Function from OnSave event
 * 
 *- Comprueba si los cups (20/22) son correctos.
 *- Comprueba también si las potencias contratadas son correctas
 *- Comprueba también si las potencias maximas son correctas
 *- Si son erróneos no permite guardar los datos
 *
 * @param {*} executionContext 
 */
function on_Save(executionContext) {

    var formContext = executionContext.getFormContext();

    var valido = true;
    var cups20 = formContext.getAttribute("atos_cups20").getValue();
    var cups22 = formContext.getAttribute("atos_cups22").getValue();
    var cupsatr = formContext.getAttribute("atos_codigopuntosolicitudes").getValue();

    if ((cups20 != null && cups20 != "" && formContext.getAttribute("atos_cups20").getIsDirty() == true) ||
	    (cups22 != null && cups22 != "" && formContext.getAttribute("atos_cups22").getIsDirty() == true) ||
	    (cupsatr != null && cupsatr != "" && formContext.getAttribute("atos_codigopuntosolicitudes").getIsDirty() == true)) {

        if (cups20 != null && cups20 != "")
            valido = validaCups(formContext,"20", "atos_cups20");
        if (valido == true)
            if (cups22 != null && cups22 != "")
                valido = validaCups(formContext,"22", "atos_cups22");
        if (valido == true)
            if (cupsatr != null && cupsatr != "")
                valido = validaCups(formContext,"22", "atos_codigopuntosolicitudes");
        if (valido == false)
            if (executionContext.getEventArgs() != null)
            executionContext.getEventArgs().preventDefault();
    }


    if (formContext.getAttribute("atos_potenciacontratada1").getIsDirty() == true ||
	     formContext.getAttribute("atos_potenciacontratada2").getIsDirty() == true ||
	     formContext.getAttribute("atos_potenciacontratada3").getIsDirty() == true ||
	     formContext.getAttribute("atos_potenciacontratada4").getIsDirty() == true ||
	     formContext.getAttribute("atos_potenciacontratada5").getIsDirty() == true ||
	     formContext.getAttribute("atos_potenciacontratada6").getIsDirty() == true) {
        valido = true;
        borraMensajesPotencia(formContext);
        for (var i = 1; i < 6; i++) {
            if (validaPotenciaAsc(formContext,i, "atos_potenciacontratada" + i) == false)
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
        borraMensajesPotenciaMaxima(formContext);
        for (var i = 1; i < 6; i++) {
            if (validaPotenciaMaximaAsc(formContext,i, "atos_potenciamaxima" + i) == false)
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
        if (compruebaFechas(formContext,"atos_fechainiciovigencia", "atos_fechafinvigencia", "La fecha de inicio de vigencia debe ser menor o igual que la fecha de fin de vigencia.") != "")

            if (obj.getEventArgs() != null)
                obj.getEventArgs().preventDefault();
    }


    if (formContext.getAttribute("atos_fechainicioexencionie").getIsDirty() == true ||
		 formContext.getAttribute("atos_fechafinexencionie").getIsDirty() == true) {
        if (compruebaFechas(formContext,"atos_fechainicioexencionie", "atos_fechafinexencionie", "La fecha de inicio de exención debe ser menor o igual que la fecha de fin de exención.", "7") != "")

            if (obj.getEventArgs() != null)
                obj.getEventArgs().preventDefault();
    }

    if (formContext.getAttribute("atos_fechainicioenvioprevisiones").getIsDirty() == true ||
	     formContext.getAttribute("atos_fechafinenvioprevisiones").getIsDirty() == true) {
        if (compruebaFechas(formContext,"atos_fechainicioenvioprevisiones", "atos_fechafinenvioprevisiones", "La fecha de inicio de envío previsiones debe ser menor o igual que la fecha de fin de envío previsiones.", "6") != "")

            if (obj.getEventArgs() != null)
                obj.getEventArgs().preventDefault();
    }

    if (comprueba_cambiavigencia(executionContext) == false)
    {
        if (obj.getEventArgs() != null)
           obj.getEventArgs().preventDefault();
    }
}


/*
 * Borra los mensajes de error en la validación de la potencia contratada.
 * @param {*} formContext 
 */
function borraMensajesPotencia(formContext) {
    for (var i = 1; i <= 6; i++) {
        for (var j = 1; j <= 6; j++) {
            var nivel = 10 * i + j;
            limpiaMensajeError(formContext,nivel.toString(), "");
        }
    }
}

/**
// <summary>
// Borra los mensajes de error en la validación de la potencia maxima.
// </summary>
*/
function borraMensajesPotenciaMaxima(formContext) {
    for (var i = 1; i <= 6; i++) {
        for (var j = 1; j <= 6; j++) {
            var nivel = 10 * i + j;
            limpiaMensajeError(formContext,nivel.toString(), "");
        }
    }
}


/*
 * Function from validaPotenciaAsc (InstalacionPower)
 * 
 * Comprueba que la potencia contratada Pi debe ser menor o igual que todas las Potencias Contratadas "superiores".
 * @param {*} formContext 
 * @param {*} numpotencia (1..6)
 * @param {*} campo 
 * @returns 
 */
function validaPotenciaAsc(formContext, numpotencia, campo) {

    try{
        formContext = formContext.getFormContext();
    }
    catch {}

    var potencia = formContext.getAttribute(campo);
    var potenciavalida = true;
    if (potencia.getValue() != null) 
    {
        var valorpotencia = potencia.getValue();
        if (numpotencia < 6) 
        {
            for (var i = numpotencia + 1; i <= 6; i++) {
                var campo_n = "atos_potenciacontratada" + i;
                var potencia_n = formContext.getAttribute(campo_n);
                if (potencia_n.getValue() != null) {
                    var valorpotencia_n = potencia_n.getValue();
                    if (valorpotencia_n < valorpotencia) {
                        potenciavalida = false;
                        var nivel = 10 * numpotencia + i;
                       mensajeError(formContext,"La Potencia Contratada P" + i + " debe ser mayor o igual que la Potencia Contratada P" + numpotencia, 'ERROR', nivel.toString(), "");
                    }
                }
            }
        }
    }
    return potenciavalida;
}

/*
 * Function from validaPotenciaMaxima (InstalacionPower)
 * 
 * Comprueba que la potencia contratada Pi debe ser menor o igual que todas las Potencias Contratadas "superiores".
 * @param {*} formContext 
 * @param {*} numpotencia (1..6)
 * @param {*} campo 
 * @returns 
 */
function validaPotenciaMaximaAsc(formContext, numpotencia, campo) {

    try{
        formContext = formContext.getFormContext();
    }
    catch {}

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
                       mensajeError(formContext,"La Potencia maxima P" + i + " debe ser mayor o igual que la Potencia maxima P" + numpotencia, 'ERROR', nivel.toString(), "");
                    }
                }
            }
        }
    }

    return potenciavalida;
}


/*
 * Function from validaPotencia (InstalacionPower)
 * 
 * Comprueba que la potencia contratada Pi debe ser menor o igual que todas las Potencias Contratadas "superiores".
 * @param {*} formContext 
 * @param {*} numpotencia (1..6)
 * @param {*} campo 
 * @returns 
 */
function validaPotenciaDes(formContext,numpotencia, campo) {
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
                       mensajeError(formContext,"La Potencia Contratada P" + numpotencia + " debe ser mayor o igual que la Potencia Contratada P" + i, 'ERROR', nivel.toString(), "");
                    }
                }
            }
        }
    }
    return potenciavalida;
}


/*
 * Function from validaPotenciaMaxima (InstalacionPower)
 * 
 * Comprueba que la potencia maxima Pi debe ser mayor o igual que todas las Potencias máximas "inferiores".
 * @param {*} formContext 
 * @param {*} numpotencia (1..6)
 * @param {*} campo 
 * @returns 
 */
function validaPotenciaMaximaDes(formContext,numpotencia, campo) {

    try{
        formContext = formContext.getFormContext();
    }
    catch{

    }
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
                       mensajeError(formContext,"La Potencia maxima P" + numpotencia + " debe ser mayor o igual que la Potencia maxima P" + i, 'ERROR', nivel.toString(), "");
                    }
                }
            }
        }
    }
    return potenciavalida; 
}


/*
 * Function from "Potencia COntratada P1 (kW)" event (InstalacionPower)
 * Function from "Potencia COntratada P2 (kW)" event (InstalacionPower)
 * Function from "Potencia COntratada P3 (kW)" event (InstalacionPower)
 * Function from "Potencia COntratada P4 (kW)" event (InstalacionPower)
 * Function from "Potencia COntratada P5 (kW)" event (InstalacionPower)
 * Function from "Potencia COntratada P6 (kW)" event (InstalacionPower)
 * 
 * Calcula la potencia máxima contratada.
 * Suma las potencias contratadas según el número de periodos y con el resultado modifica el campo potencia máxima contratada
 * @param {*} formContext 
 * @param {*} numeroperiodos 
 */
function calculaPotenciaMax(formContext,numeroperiodos) {

       
    try{
        formContext = formContext.getFormContext();
    }
    catch{

    }
    if (formContext.getAttribute("atos_potenciacontratada1").getIsDirty() == true ||
	   formContext.getAttribute("atos_potenciacontratada2").getIsDirty() == true ||
	   formContext.getAttribute("atos_potenciacontratada3").getIsDirty() == true ||
	   formContext.getAttribute("atos_potenciacontratada4").getIsDirty() == true ||
	   formContext.getAttribute("atos_potenciacontratada5").getIsDirty() == true ||
	   formContext.getAttribute("atos_potenciacontratada6").getIsDirty() == true) 
    {
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
            formContext.getAttribute("atos_potenciamaximacontratada").setSubmitMode("always");
        }
    }
}


/*
 * Function from "Potencia Contratada P1 (kW)" event (InstalacionPower)
 * Function from "Potencia Contratada P2 (kW)" event (InstalacionPower)
 * Function from "Potencia Contratada P3 (kW)" event (InstalacionPower)
 * Function from "Potencia Contratada P4 (kW)" event (InstalacionPower)
 * Function from "Potencia Contratada P5 (kW)" event (InstalacionPower)
 * Function from "Potencia Contratada P6 (kW)" event (InstalacionPower)
 * 
 * Valida la potencia contratada Pi llamando a validaPotenciaAsc y validaPotenciaDes.
 * Carga antes el javascript necesario (atos_mensajes.js).
 * @param {*} formContext 
 * @param {*} numpotencia 
 */
function validaPotencia(formContext, numpotencia) {

    debugger;
    try{
        formContext = formContext.getFormContext();
    }
    catch {}
    
    var serverUrl = formContext.context.getClientUrl();

    head.load(serverUrl + "/WebResources/atos_mensajes.js", function () {

        borraMensajesPotencia(formContext);
        var campo = "atos_potenciacontratada" + numpotencia;
        var potenciavalida = true;

        validaPotenciaAsc(formContext,numpotencia, campo);
        validaPotenciaDes(formContext,numpotencia, campo);
    });
}


/*
 * Function from validaPotencia (InstalacionPower)
 * Function from "Potencia Maxima P1..P6 (kW)" event (InstalacionPower)

 * Valida la potencia maxima Pi llamando a validaPotenciaMaximaAsc y validaPotenciaMaximaDes.
 * @param {*} formContext 
 * @param {*} numpotencia (1..6)
 */
function validaPotenciaMaxima(formContext, numpotencia) {

    try{
        formContext = formContext.getFormContext();
    }
    catch {}

    var serverUrl =formContext.context.getClientUrl();

    head.load(serverUrl + "/WebResources/atos_mensajes.js", function () {

        borraMensajesPotencia(formContext);
        var campo = "atos_potenciamaxima" + numpotencia;
        var potenciavalida = true;

        validaPotenciaMaximaAsc(formContext,numpotencia, campo);
        validaPotenciaMaximaDes(formContext,numpotencia, campo);

    });
}


/*
 * Function from "Consumo estimado anual P1..P6 (Mwh)" event (InstalacionPower)
 * 
 * Calcula el valor del consumo estimado anual a partir de los consumos estimados anuales por periodos.
 * @param {*} formContext 
 */
function acumulaConsumo(executionContext) {

    var formContext = executionContext.getFormContext();

    if (formContext.getAttribute("atos_consumoestimadoanual1").getIsDirty() == true ||
	     formContext.getAttribute("atos_consumoestimadoanual2").getIsDirty() == true ||
	     formContext.getAttribute("atos_consumoestimadoanual3").getIsDirty() == true ||
	     formContext.getAttribute("atos_consumoestimadoanual4").getIsDirty() == true ||
	     formContext.getAttribute("atos_consumoestimadoanual5").getIsDirty() == true ||
	     formContext.getAttribute("atos_consumoestimadoanual6").getIsDirty() == true) 
    {
        if (formContext.getAttribute("atos_consumoestimadoanual1").getValue() != null ||
			 formContext.getAttribute("atos_consumoestimadoanual2").getValue() != null ||
			 formContext.getAttribute("atos_consumoestimadoanual3").getValue() != null ||
			 formContext.getAttribute("atos_consumoestimadoanual4").getValue() != null ||
			 formContext.getAttribute("atos_consumoestimadoanual5").getValue() != null ||
			 formContext.getAttribute("atos_consumoestimadoanual6").getValue() != null) 
        {
            var total = 0.0;
            for (var i = 1; i <= 6; i++) {
                var campo = "atos_consumoestimadoanual" + i;
                if (formContext.getAttribute(campo).getValue() != null) {
                    total = parseFloat(total + parseFloat(formContext.getAttribute(campo).getValue()));
                }
            }
            // total = parseFloat(total / 1000.0); // No se transforma porque el total va en las mismas unidades que los periodos
            formContext.getAttribute("atos_consumoestimadototalanual").setValue(total);
            formContext.getAttribute("atos_consumoestimadototalanual").setSubmitMode("always");
        }
    }
}

/**
// <summary>
// Habilita/Deshabilita los consumos 0 potencionas por periodos según el número de periodos recibidos.
// </summary>
// <param name="campobase">atos_potenciacontratada o atos_consumoestimadoanual</param>
// <param name="numeroperiodos">Número de periodos</param>
// <param name="requerido">Indica si los campos habilitados son obligatorios ("required") u opcionales ("none")</param>
// <remarks>
// - Habilita todos los campos periodo (campobase + i, siendo i desde 1 hasta el número de periodos recibido) inferiores al número de periodos recibidos
// - Deshabilita todos los campos periodo (campobase + i, siendo i desde numero de periodos recibido + 1 hasta 6) mayores que el número de periodos recibidos
// </remarks>
*/
function habilitaPeriodos(formContext, campobase, numeroperiodos, requerido) {

    for (var i = 1; i <= numeroperiodos; i++) {
        formContext.getControl(campobase + i).setDisabled(false);
        formContext.getAttribute(campobase + i).setRequiredLevel(requerido);
    }

    for (var i = numeroperiodos + 1; i <= 6; i++) {
        formContext.getControl(campobase + i).setDisabled(true);
        formContext.getAttribute(campobase + i).setRequiredLevel("none");
        if (formContext.getAttribute(campobase + i).getValue() != null &&
		     formContext.getAttribute(campobase + i).getValue() != "") {
            formContext.getAttribute(campobase + i).setValue(null);
            formContext.getAttribute(campobase + i).setSubmitMode("always");
        }
    }
}

/**
// <summary>
// Habilita/Deshabilita potencias maximas por periodos según el número de periodos recibidos y de si es cogeneración o no
// </summary>
// <param name="campobase">atos_potenciamaxima
// <param name="numeroperiodos">Número de periodos</param>
// <param name="requerido">Indica si los campos habilitados son obligatorios ("required") u opcionales ("none")</param>
// <remarks>
// - Habilita todos los campos periodo (campobase + i, siendo i desde 1 hasta el número de periodos recibido) inferiores al número de periodos recibidos
// - Deshabilita todos los campos periodo (campobase + i, siendo i desde numero de periodos recibido + 1 hasta 6) mayores que el número de periodos recibidos
// </remarks>
*/
function habilitaPeriodosPotenciaMaxima(formContext,campobase, numeroperiodos, requerido) {

    if (formContext.getAttribute("atos_cogeneracion").getValue() == true) {
        for (var i = 1; i <= numeroperiodos; i++) {
            formContext.getControl(campobase + i).setDisabled(false);
            formContext.getAttribute(campobase + i).setRequiredLevel(requerido);
        }
        for (var i = numeroperiodos + 1; i <= 6; i++) {
            formContext.getControl(campobase + i).setDisabled(true);
            formContext.getAttribute(campobase + i).setRequiredLevel("none");
            formContext.getAttribute(campobase + i).setValue(null);
            formContext.getAttribute(campobase + i).setSubmitMode("always");
        }
    }
    else
        for (var i = 1; i <= 6; i++) {
            formContext.getControl(campobase + i).setDisabled(true);
            formContext.getAttribute(campobase + i).setRequiredLevel("none");
            // #16956 Ini
            //formContext.getAttribute(campobase + i).setValue(null);
            //formContext.getAttribute(campobase + i).setSubmitMode("always");
            // #16956 Fin
        }

}




/**
 * Function from Tarifa event (InstalacionPower)
 * Function from Cogeneracion event (InstalacionPower)
 * 
 * Habilita/Deshabilita los consumos y potencionas por periodos según el número de periodos de la tarifa.
 * Carga los javascripts necesarios:
 * -# atos_json2.js
 * -# atos_jquery.js
 * -# atos_XrmServiceToolkit.js
 * - Recupera el número de periodos de la tarifa mediante XrmServiceToolkit.Soap.Retrieve
 * - Llama a la función habilitaPeriodos para las potencias contratadas
 * - Si el campo Informacion de Consumos no es Por periodos considera que el número de periodos es 0 (para deshabilitar todos los consumos)
 * - Llama a la función habilitaPeriodos para los consumos estimados
 *
 * @param {*} executionContext 
 */
async function periodosSegunTarifa(executionContext) {
debugger;

    var formContext = executionContext.getFormContext();
    var tarifaid = formContext.getAttribute("atos_tarifaid").getValue();

    if (tarifaid != null) {
        var serverUrl = formContext.context.getClientUrl();       
        var recNumeroPeriodos = null;

        Xrm.WebApi.retrieveRecord("atos_tarifa", tarifaid[0].id,"?$select=atos_numeroperiodos").then(
        function success(record) {
            recNumeroPeriodos = record;

            var nperiodos = 0;
            if (recNumeroPeriodos.atos_numeroperiodos != null)
                nperiodos = parseInt(recNumeroPeriodos.atos_numeroperiodos);

            habilitaPeriodos(formContext,"atos_potenciacontratada", nperiodos, "none"); // las potencias contratadas no son obligatorias
            habilitaPeriodosPotenciaMaxima(formContext,"atos_potenciamaxima", nperiodos, "none"); // las potencias maximas no son obligatorias
            calculaPotenciaMax(formContext,nperiodos);

            if (formContext.getAttribute("atos_informaciondeconsumos").getValue() != null)
                if (formContext.getAttribute("atos_informaciondeconsumos").getValue() != 300000001)
                    nperiodos = 0;

            habilitaPeriodos(formContext,"atos_consumoestimadoanual", nperiodos, "required");  // los consumos son obligatorias
            acumulaConsumo(formContext);
        });
    }
}



/*
 * Function from "Informacion de consumos" event (InstalacionPower)
 * 
 * Deshabilita los consumos por periodos si el campo informacion de consumos no toma el valor Por Periodos.
 * @param {*} executionContext 
 */
async function consumosporperiodos(executionContext) {
debugger; /* revisar */
    
    var formContext = executionContext.getFormContext();
    var informacionconsumos = formContext.getAttribute("atos_informaciondeconsumos");

    if (informacionconsumos.getValue() != null) {
        var disabled = true;
        var disabledtotal = false;
        if (informacionconsumos.getValue() == 300000001) {
            disabled = false;
            disabledtotal = true;
        }

        formContext.getControl("atos_consumoestimadototalanual").setDisabled(disabledtotal);
        for (var i = 1; i <= 6; i++) {
            formContext.getControl("atos_consumoestimadoanual" + i).setDisabled(disabled);
        }

        await periodosSegunTarifa(executionContext);

        if (formContext.getAttribute("atos_informaciondeconsumos").getIsDirty() == true) {
            formContext.getAttribute("atos_consumoestimadototalanual").setValue(null);
            formContext.getAttribute("atos_consumoestimadototalanual").setSubmitMode("always");

            for (var i = 1; i <= 6; i++) {
                formContext.getAttribute("atos_consumoestimadoanual" + i).setValue(null);
                formContext.getAttribute("atos_consumoestimadoanual" + i).setSubmitMode("always");
            }
        }
    }


}

function getGlobalContext() { return Xrm.Utility.getGlobalContext(); }
function getContext() { return formContext; } 
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
            getContext().ui.clearFormNotification(id);
        },
        notificationTime
    );
}


/*
 * Function from Button "Contrato" Ribbon "Instalacion Power"
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
        formParameters["llamado_desde"] = "Instalacion";
        formParameters["llamante_id"] = getId();  /* 22323 */ // formContext._entityReference.id["guid"]; 
        
        Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
            function (success) {
                console.log(success);
            }, 
            function (error) {
                console.log(error);
            }
        );
    }
}
f


/**
 * Function from OnLoadForm event
 * 
 * Al entrar al formulario muestra un aviso si la interfaz con EMS ha devuelto un KO
 * Esta función no aplica en nueva instalación
 * Si el campo interfaz instalacion con EMS está con el valor KO muestra un mensaje
 * de aviso indicando que la última ejecución de WS de Instalación con EMS ha finalizado con errores.
 * 
 * Si el campo interfaz consumo con EMS está con el valor KO muestra un mensaje de aviso indicando
 * que la última ejecución de WS de Consumo con EMS ha finalizado con errores.
 * @param {*} executionContext 
 */
function WarningEMS(executionContext) {

    var formContext = executionContext.getFormContext();

    if (formContext.data.entity.getId() == null || formContext.data.entity.getId() == "" ) {

        if (formContext.getAttribute("atos_interfazinstalacionems").getValue() == 300000000) // KO EMS
        {
            formContext.setFormNotification("La última ejecución del WS de Instalación con EMS ha finalizado con errores", "WARNING", "1");
        }
        if (formContext.getAttribute("atos_interfazconsumoems").getValue() == 300000000) // KO EMS
        {
            formContext.setFormNotification("La última ejecución del WS de Consumo con EMS ha finalizado con errores", "WARNING", "2");
        }
    }
}


/**
 * Function from "Fecha Envio Inicio Previsiones" event (Instalacion Power) 
 * 
 * Inicializa la fecha fin de envío previsiones a partir de la fecha inicio envío previsiones
 * Si la fecha inicio envío previsiones tiene valor y la fecha de fin envío previsiones está 
 * vacía, rellena esta última con el resultado de sumar 12 meses a la fecha de inicio.
 * @param {*} executionContext 
 */
function fechaFinEnvioPrevisiones(executionContext) {

    var formContext = executionContext.getFormContext();

    if (formContext.getAttribute("atos_fechainicioenvioprevisiones").getValue() != null &&
	    formContext.getAttribute("atos_fechafinenvioprevisiones").getValue() == null) 
    {
        formContext.getAttribute("atos_fechafinenvioprevisiones").setValue(sumaMesesFecha(formContext.getAttribute("atos_fechainicioenvioprevisiones").getValue(), 12));
        formContext.getAttribute("atos_fechafinenvioprevisiones").setSubmitMode("always");
    }
}


/*
 * Function from "Tipo Documento ATR" event (InstalacionPower)
 * 
 *  Obligatoriedad de campos en función del tipo de documento ATR
 * @param {*} executionContext 
 */
function TipoDocumentoATR_OnChange(executionContext) {
// debugger;
    var formContext = executionContext.getFormContext();

    var tipoDocumento = formContext.getAttribute("atos_tipodocumentotitularatr");

/*22323*/ // if (tipoDocumento.getValue != null) { 
    if (tipoDocumento.getValue() != null) 
    { 
        switch (tipoDocumento.getValue())  {
            case "CIF":
                formContext.getAttribute("atos_sociedadtitularatr").setRequiredLevel("required");
                formContext.getAttribute("atos_nombretitularatr").setRequiredLevel("none");
                formContext.getAttribute("atos_primerapellidotitularatr").setRequiredLevel("none");
                break;
            case "NIF":
                formContext.getAttribute("atos_nombretitularatr").setRequiredLevel("required");
                formContext.getAttribute("atos_primerapellidotitularatr").setRequiredLevel("required");
                formContext.getAttribute("atos_sociedadtitularatr").setRequiredLevel("none");
                break;
            default:
                formContext.getAttribute("atos_sociedadtitularatr").setRequiredLevel("none");
                formContext.getAttribute("atos_nombretitularatr").setRequiredLevel("none");
                formContext.getAttribute("atos_primerapellidotitularatr").setRequiredLevel("none");                
        }
        /*
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
        } */
    }
}



/**
// <summary>
// Inicializa los campos de tipo y número de documento del titular atr.
// </summary>
// <remarks>
// Si el tipo de documento atr está vacío se inicializa con el valor CIF<br/>
// Si el tipo de documento atr es CIF y el número de documento del titular atr está vacío se inicializa con el documento de la razón social (si es cif)
// </remarks>
*/
function CamposDefectoATR(executionContext) {

    var globalContext = Xrm.Utility.getGlobalContext();
    var serverUrl = globalContext.getClientUrl();
    var formContext = executionContext.getFormContext();

    if (formContext.data.entity.getId() == null || formContext.data.entity.getId() == "" ) {
        var tipoDocumento = formContext.getAttribute("atos_tipodocumentotitularatr");

        if (formContext.getAttribute("atos_tipodocumentotitularatr").getValue() == null) {
            formContext.getAttribute("atos_tipodocumentotitularatr").setValue(300000001);
            formContext.getAttribute("atos_tipodocumentotitularatr").setSubmitMode("always");
        }

        if (formContext.getAttribute("atos_tipodocumentotitularatr").getText() == "CIF") {
            if (formContext.getAttribute("atos_numerodocumentotitularatr").getValue() == null ||
			    formContext.getAttribute("atos_sociedadtitularatr").getValue() == null) 
            {
                // var serverUrl = Xrm.Page.context.getClientUrl();
                head.load(serverUrl + "/WebResources/atos_json2.js", serverUrl + "/WebResources/atos_jquery.js", function () {
                    Xrm.WebApi.retrieveRecord("account", formContext.getAttribute("atos_razonsocialid").getValue()[0].id, "?$select=atos_tipodedocumento,atos_numerodedocumento,name").then(
                    function (result) {

                        debugger;
                        if (result.atos_tipodedocumento != null) {
                            if (result.atos_tipodedocumento == 300000001 && result.atos_numerodedocumento != null && formContext.getAttribute("atos_numerodocumentotitularatr").getValue() == null) {
                                formContext.getAttribute("atos_numerodocumentotitularatr").setValue(result.atos_numerodedocumento);
                                formContext.getAttribute("atos_numerodocumentotitularatr").setSubmitMode("always");
                            }

                            if (result.atos_tipodedocumento == 300000001 && result.name != null && formContext.getAttribute("atos_sociedadtitularatr").getValue() == null) {
                                var accountname = result.name;
                                if (accountname.length > 45)
                                    formContext.getAttribute("atos_sociedadtitularatr").setValue(accountname.substring(0, 45));
                                else { // AC {
                                    formContext.getAttribute("atos_sociedadtitularatr").setValue(accountname);
                                    formContext.getAttribute("atos_sociedadtitularatr").setSubmitMode("always");
                                } // AC }
                            }
                        }
                    },
                    function(error) {
                        Xrm.Navigation.openErrorDialog({ detail: error.message });
                    });
                    // var recRazonSocial = XrmServiceToolkit.Soap.Retrieve("account", formContext.getAttribute("atos_razonsocialid").getValue()[0].id, ["atos_tipodedocumento", "atos_numerodedocumento", "name"]);

                });
            }
        }
    }
}


/*
 * Function from "Tipo Documento ATR" event (InstalacionPower)
 * Function from "Nro Documento Titular ATR" event (InstalacionPower)
 * 
 * Carga los javascripts necesarios:
 * -# atos_mensajes.js
 * -# atos_validadocumentos.js
 * Llama a la función validaDocumento para que haga la validación.
 * @param {*} executionContext 
 */
function Documento_OnChange(executionContext) {

    var formContext = executionContext.getFormContext();
    var serverUrl = formContext.context.getClientUrl();

    head.load(serverUrl + "/WebResources/atos_mensajes.js", 
              serverUrl + "/WebResources/atos_validadocumentos.js", function () {

        var mensaje = validaDocumento("atos_tipodocumentotitularatr", "atos_numerodocumentotitularatr", "CAMPO");
    });
}


/*
 * Function from Cogeneracion event (InstalacionPower)
 * 
 * Muestra un mensaje si el valor del campo cambia
 * @param {*} executionContext 
 */
function cogeneracion_OnChange(executionContext) {

    var formContext = executionContext.getFormContext();
    var cogeneracion = formContext.getAttribute("atos_cogeneracion");

    if (cogeneracion.getValue() != null && cogeneracion.getIsDirty()) {
        /*
        Xrm.Utility.confirmDialog("Ha modificado el valor de cogeneración. ¿Desea continuar con el cambio?",
            function () {
                // Al pulsar OK.
            },
            function () {
                cogeneracion.setValue(formContext.getAttribute("atos_cogeneracion").getInitialValue());
                // Al pulsar Cancel.
            }
        ); */
        var confirmStrings = { text:"Ha modificado el valor de cogeneración.\n\n¿Desea continuar con el cambio?", title:"Confirmación" };
        var confirmOptions = { height: 200, width: 450 };
    
        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
        function (success) {    
            if (success.confirmed)
                console.log("Dialog closed using OK button.");
            else {
                cogeneracion.setValue(formContext.getAttribute("atos_cogeneracion").getInitialValue());
                console.log("Dialog closed using Cancel button or X.");
            }
        });
    }




}

/*
 * Function from Interrumpibilidad event (InstalacionPower)
 * 
 * @param {*} executionContext 
 */
function interrumpibilidad_OnChange(executionContext) {

    var formContext = executionContext.getFormContext();
    var interrumpibilidad = formContext.getAttribute("atos_interrumpibilidad");

    if (interrumpibilidad.getValue() != null && interrumpibilidad.getIsDirty()) {
        /*
        Xrm.Utility.confirmDialog("Ha modificado el valor de interrumpibilidad. ¿Desea continuar con el cambio?",
            function () {
                // Al pulsar OK.
            },
            function () {
                interrumpibilidad.setValue(formContext.getAttribute("atos_interrumpibilidad").getInitialValue());
                // Al pulsar Cancel.
            }
        );*/
        var confirmStrings = { text:"Ha modificado el valor de interrumpibilidad.\n\n¿Desea continuar con el cambio?", title:"Confirmación" };
        var confirmOptions = { height: 100, width: 450 };
    
        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
        function (success) {    
            if (success.confirmed)
                console.log("Dialog closed using OK button.");
            else {
                interrumpibilidad.setValue(formContext.getAttribute("atos_interrumpibilidad").getInitialValue());
                console.log("Dialog closed using Cancel button or X.");
            }
        });
    }
}


/*
 * Function from Punt de socorro event (InstalacionPower)
 * 
 * @param {*} executionContext 
 */
function puntodesocorro_OnChange(executionContext) {

    var formContext = executionContext.getFormContext();
    var puntodesocorro = formContext.getAttribute("atos_puntodesocorro");

    if (puntodesocorro.getValue() != null && puntodesocorro.getIsDirty()) {
        /*
        Xrm.Utility.confirmDialog("Ha modificado el valor de Punto de Socorro. ¿Desea continuar con el cambio?",
            function () {
                // Al pulsar OK.
            },
            function () {
                puntodesocorro.setValue(formContext.getAttribute("atos_puntodesocorro").getInitialValue());
                // Al pulsar Cancel.
            }
        );
        */
        var confirmStrings = { text:"Ha modificado el valor de Punto de Socorro.\n\n¿Desea continuar con el cambio?", title:"Confirmación" };
        var confirmOptions = { height: 100, width: 450 };
    
        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
        function (success) {    
            if (success.confirmed)
                console.log("Dialog closed using OK button.");
            else {
                puntodesocorro.setValue(formContext.getAttribute("atos_puntodesocorro").getInitialValue());
                console.log("Dialog closed using Cancel button or X.");
            }
        });
    }
}

/**
// <summary>
// Se ejecuta en el OnChange del campo punto esencial para mostrar un mensaje si el valor del campo cambia .
// </summary>
// <remarks>
// </remarks>
*/
function puntoesencial_OnChange(executionContext) {

    var formContext = executionContext.getFormContext();
    var puntoesencial = formContext.getAttribute("atos_puntoesencial");

    if (puntoesencial.getValue() != null && puntoesencial.getIsDirty()) {
        /*
        Xrm.Utility.confirmDialog("Ha modificado el valor de Punto Esencial. ¿Desea continuar con el cambio?",
            function () {
                // Al pulsar OK.
            },
            function () {
                puntoesencial.setValue(formContext.getAttribute("atos_puntoesencial").getInitialValue());
                // Al pulsar Cancel.
            }
        );
        */
        var confirmStrings = { text:"Ha modificado el valor de Punto Esencial.\n\n¿Desea continuar con el cambio?", title:"Confirmación" };
        var confirmOptions = { height: 100, width: 450 };
    
        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
        function (success) {    
            if (success.confirmed)
                console.log("Dialog closed using OK button.");
            else {
                puntoesencial.setValue(formContext.getAttribute("atos_puntoesencial").getInitialValue());
                console.log("Dialog closed using Cancel button or X.");
            }
        });
    }
}

/**
// <summary>
// Se ejecuta en el OnChange del campo aplica neteo de energia para mostrar un mensaje si el valor del campo cambia .
// </summary>
// <remarks>
// </remarks>
*/
function aplicaneteodeenergia_OnChange(executionContext) {

    var formContext = executionContext.getFormContext();
    var aplicaneteodeenergia = formContext.getAttribute("atos_aplicaneteodeenergia");

    if (aplicaneteodeenergia.getValue() != null && aplicaneteodeenergia.getIsDirty()) {
        /*
        Xrm.Utility.confirmDialog("Ha modificado el valor de Aplica neteo de energía. ¿Desea continuar con el cambio?",
            function () {
                // Al pulsar OK.
            },
            function () {
                aplicaneteodeenergia.setValue(formContext.getAttribute("atos_aplicaneteodeenergia").getInitialValue());
                // Al pulsar Cancel.
            }
        ); 
        */
        var confirmStrings = { text:"Ha modificado el valor de Aplica neteo de energía.\n\n¿Desea continuar con el cambio?", title:"Confirmación" };
        var confirmOptions = { height: 100, width: 450 };
    
        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
        function (success) {    
            if (success.confirmed)
                console.log("Dialog closed using OK button.");
            else {
                aplicaneteodeenergia.setValue(formContext.getAttribute("atos_aplicaneteodeenergia").getInitialValue());
                console.log("Dialog closed using Cancel button or X.");
            }
        });
    }
}
