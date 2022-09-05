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


// <summary>
// Se ejecuta en el OnChange del campo Documento para validarlo.
// </summary>
// <remarks>
// Carga los javascripts necesarios:
// -# atos_mensajes.js
// -# atos_validadocumentos.js
// - Llama a la funciÃ³n validaDocumento para que haga la validaciÃ³n.
// </remarks>
 
function Documento_OnChange(executionContext) 
{	
    var formContext = executionContext.getFormContext();

	var serverUrl = formContext.context.getClientUrl();
	head.load(serverUrl + "/WebResources/atos_mensajes.js", serverUrl + "/WebResources/atos_validadocumentos.js", function() {
		var mensaje = validaDocumento(formContext,"atos_tipodedocumento", "atos_numerodedocumento", "CAMPO");
	});
}


// <summary>
// Se ejecuta en el Click del botÃ³n de creaciÃ³n de ofertas con instalaciones de power.
// </summary>
// <remarks>
// Carga los javascripts necesarios:
// -# atos_json2.js
// -# atos_json2.js
// -# atos_XrmServiceToolkit.js
// - Muestra un mensaje de confirmaciÃ³n para crear las ofertas con instalaciones de power para la razÃ³n social y sus instalaciones.
// - Muestra el mensaje de "carga" (showLoadingMessage)
// - Provoca que se ejecute el plugin de creaciÃ³n de ofertas insertando un registro en atos_trigger
// - Oculta el mensaje de "carga"
// </remarks>
 
function CreaOfertasPower_Click(primaryControl) 
{	
    if (primaryControl._entityReference.id["guid"] != null && primaryControl._entityReference.id["guid"] != "") {
        var entityFormOptions = {};
        entityFormOptions["entityName"] = "atos_oferta";
        var formParameters = {};
        formParameters["llamado_desde"] = "Razon Social Power";
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
			Xrm.Utility.confirmDialog("Â¿Desea crear las ofertas para la razÃ³n social y sus instalaciones?." ,
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
				creaOfertas.attributes["atos_entity"] = "account";
				creaOfertas.attributes["atos_guid"] = Xrm.Page.data.entity.getId();
				var creaOfertasId = XrmServiceToolkit.Soap.Create(creaOfertas);
				Xrm.Page.ui.clearFormNotification('2');
				alert("Ha finalizado el proceso de generaciÃ³n de ofertas");
				document.all.msgDiv.style.visibility = 'hidden';
			}
		}
	});	*/
}

/**
// <summary>
// Se ejecuta en el Click del botÃ³n de creaciÃ³n de ofertas con instalaciones de gas.
// </summary>
// <remarks>
// Carga los javascripts necesarios:
// -# atos_json2.js
// -# atos_json2.js
// -# atos_XrmServiceToolkit.js
// - Muestra un mensaje de confirmaciÃ³n para crear las ofertas con instalaciones de gas para la razÃ³n social y sus instalaciones.
// - Muestra el mensaje de "carga" (showLoadingMessage)
// - Provoca que se ejecute el plugin de creaciÃ³n de ofertas insertando un registro en atos_trigger
// - Oculta el mensaje de "carga"
// </remarks>
 */
function CreaOfertasGas_Click(primaryControl) 
{	
    if (primaryControl._entityReference.id["guid"] != null && primaryControl._entityReference.id["guid"] != "") {
        var entityFormOptions = {};
        entityFormOptions["entityName"] = "atos_oferta";
        var formParameters = {};
        formParameters["llamado_desde"] = "Razon Social Gas";
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

/**
// <summary>
// Se ejecuta al guardar los datos de la razÃ³n social.
// </summary>
// <remarks>
// Si se ha modificado el nombre pero no se ha cambiado la fecha de inicio de vigencia se muestra un mensaje de confirmaciÃ³n.
// Modificado para UI. Lazaro Castro
// </remarks>
 */
function Cuenta_OnSave(executionContext)
{
    var formContext = executionContext.getFormContext();
    // var serverUrl = Xrm.Page.context.getClientUrl();
	// Xrm.Page.ui.clearFormNotification("5");
    formContext.ui.clearFormNotification("5")
	
	if ( formContext.data.entity.attributes.get("atos_clientecreadoenems").getValue() == true )
	{
        if (formContext.getAttribute("name").getIsDirty() == true &&
            formContext.getAttribute("atos_fechainiciovigencia").getIsDirty() == false)
		{
            Xrm.Navigation.openConfirmDialog("Ha modificado el nombre pero no ha modificado la fecha de inicio de vigencia, Â¿desea continuar?.\nSi continÃºa el registro del histÃ³rico que se genere quedarÃ¡ con fechas de vigencia inconsistentes(la fecha de fin anterior a la fecha de inicio).").then(
                function (success) {
                    if (success.confirmed) {
                    }
                    else {
                        if (executionContext.getEventArgs() != null)
                            executionContext.getEventArgs().preventDefault();
                    }
                });
		}
	}

	
    if (formContext.getAttribute("atos_fechainiciovigencia").getIsDirty() == true ||
        formContext.getAttribute("atos_fechafinvigencia").getIsDirty() == true )
	{
        if (compruebaFechas(formContext, "atos_fechainiciovigencia", "atos_fechafinvigencia", "La fecha de inicio de vigencia debe ser menor o igual que la fecha de fin de vigencia.") != "" )
            if (executionContext.getEventArgs() != null)
                executionContext.getEventArgs().preventDefault();
	}

    if (formContext.getAttribute("atos_numerodedocumento").getIsDirty() == true )
	{
		if ( DocumentoDuplicado(executionContext) != "" )
            if (executionContext.getEventArgs() != null)
                executionContext.getEventArgs().preventDefault();
	}

	// Si es domiciliaciÃ³n bancaria y han cambiado datos de la cuenta la validamos.
    if (formContext.getAttribute("atos_formadepago").getValue() == 300000001) // DomiciliaciÃ³n
    {
        if (formContext.getAttribute("atos_entidadbancaria").getIsDirty() == true ||
            formContext.getAttribute("atos_sucursalbancaria").getIsDirty() == true ||
            formContext.getAttribute("atos_digitocontrol").getIsDirty() == true ||
            formContext.getAttribute("atos_cuenta").getIsDirty() == true) {
            if (validaCuenta(executionContext) == false)
                if (executionContext.getEventArgs() != null)
                    executionContext.getEventArgs().preventDefault();
        }
    }
}

/**
// <summary>
// Se ejecuta en el Onload pasando el Contexto
// </summary>
// <remarks>
// Modificado para UI. Lazaro Castro
// </remarks>
 */
function WarningEMS(executionContext)
{
    var globalContext = Xrm.Utility.getGlobalContext();
    var serverUrl = globalContext.getClientUrl();
    var formContext = executionContext.getFormContext();
	head.load(serverUrl + "/WebResources/atos_mensajes.js", serverUrl + "/WebResources/atos_json2.js", serverUrl + "/WebResources/atos_jquery.js", serverUrl + "/WebResources/atos_duplicados.js", serverUrl + "/WebResources/atos_comun.js", function() {
	
        if (formContext.data.entity.getId() == null || formContext.data.entity.getId() == "" )
		{
            if (formContext.getAttribute("atos_interfazclienteems").getValue() == 300000000 ) // KO EMS
			{	
				//Xrm.Page.ui.setFormNotification("La Ãºltima ejecuciÃ³n del WS de Cliente con EMS ha finalizado con errores", "WARNING", "1");
				var error = "La Ãºltima ejecuciÃ³n del WS de Cliente con EMS ha finalizado con errores";
				mensajeError(error,"WARNING","1", "");
			}
		}
	});
}


function DocumentoDuplicado(executionContext)
{
    var formContext =executionContext.getFormContext();

	limpiaMensajeError(formContext,"2", "atos_numerodedocumento");
	var error = "";
	if (formContext.data.entity.attributes.get("atos_numerodedocumento").getValue() != "" )
	{
		var duplicado = controlDuplicadosCA(formContext,"account", "atos_numerodedocumento", formContext.data.entity.attributes.get("atos_numerodedocumento").getValue(), "accountid", "statecode", "<condition attribute='statecode' operator='eq' value='0' />");
		if ( duplicado == 0 )
		{
			valido = false;
			error = "Ya existe una cuenta activa con ese nÃºmero de documento en el sistema.";
			mensajeError(formContext,error, "CAMPO", "2", "atos_numerodedocumento");
		}
	}
	return error;
}


/**
// <summary>
// Se ejecuta en el OnChange del campo Name
// </summary>
// <remarks>
// Al modificar el nombre se copia al nombre de envÃ­o facturas.
// </remarks>
 */
function Name_OnChange(executionContext) 
{
    var formContext = executionContext.getFormContext();
    formContext.getAttribute("atos_nombreenviofacturas").setValue(formContext.getAttribute("name").getValue());
	formContext.getAttribute("atos_nombreenviofacturas").setSubmitMode("always");
}

/**
// <summary>
// Comprueba si el campo recibido por parÃ¡metro estÃ¡ vacÃ­o en el formulario
// </summary>
*/
function campoVacio(formContext, campo) {
    if (formContext.getAttribute(campo).getValue() == null ||
        formContext.getAttribute(campo).getValue() == "")
        return true;
    return false;
}

/**
// <summary>
// Borramos el valor del campo mandato sepa
// </summary>
*/
function borrarMandatoSepa(executionContext) {
    var formContext = executionContext.getFormContext();
    formContext.getAttribute("atos_mandatosepa").setValue("");
    formContext.getAttribute("atos_mandatosepa").setSubmitMode("always");
}

/**
// <summary>
// Si la forma de pago es domiciliaciÃ³n bancaria habilita los campos de la cuenta bancaria
// </summary>
*/
function domiciliacionBancaria(executionContext) {
    var formContext = executionContext.getFormContext();
    if (formContext.getAttribute("atos_formadepago").getValue() == 300000001) // DomiciliaciÃ³n
    {
        formContext.getAttribute("atos_iban").setRequiredLevel("required");
        formContext.getAttribute("atos_entidadbancaria").setRequiredLevel("required");
        formContext.getAttribute("atos_sucursalbancaria").setRequiredLevel("required");
        formContext.getAttribute("atos_digitocontrol").setRequiredLevel("required");
        formContext.getAttribute("atos_cuenta").setRequiredLevel("required");
        formContext.getControl("atos_swift").setDisabled(false);
        formContext.getControl("atos_iban").setDisabled(false);
        formContext.getControl("atos_entidadbancaria").setDisabled(false);
        formContext.getControl("atos_sucursalbancaria").setDisabled(false);
        formContext.getControl("atos_digitocontrol").setDisabled(false);
        formContext.getControl("atos_cuenta").setDisabled(false);
        //Xrm.Page.getControl("atos_cuentabancaria").setDisabled(false);
    }
    else {
        formContext.ui.clearFormNotification("8"); // Limpiamos el posible error de validaciÃ³n de la cuenta
        formContext.getAttribute("atos_iban").setRequiredLevel("none");
        formContext.getAttribute("atos_entidadbancaria").setRequiredLevel("none");
        formContext.getAttribute("atos_sucursalbancaria").setRequiredLevel("none");
        formContext.getAttribute("atos_digitocontrol").setRequiredLevel("none");
        formContext.getAttribute("atos_cuenta").setRequiredLevel("none");
        if (campoVacio(formContext, "atos_swift") == false) // Si tiene valor lo vacÃ­a
        {
            formContext.getAttribute("atos_swift").setValue(null);
            formContext.getAttribute("atos_swift").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_iban") == false) // Si tiene valor lo vacÃ­a
        {
            formContext.getAttribute("atos_iban").setValue(null);
            formContext.getAttribute("atos_iban").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_entidadbancaria") == false) // Si tiene valor lo vacÃ­a
        {
            formContext.getAttribute("atos_entidadbancaria").setValue(null);
            formContext.getAttribute("atos_entidadbancaria").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_sucursalbancaria") == false) // Si tiene valor lo vacÃ­a
        {
            formContext.getAttribute(formContext, "atos_sucursalbancaria").setValue(null);
            formContext.getAttribute("atos_sucursalbancaria").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_digitocontrol") == false) // Si tiene valor lo vacÃ­a
        {
            formContext.getAttribute("atos_digitocontrol").setValue(null);
            formContext.getAttribute("atos_digitocontrol").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_cuenta") == false) // Si tiene valor lo vacÃ­a
        {
            formContext.getAttribute("atos_cuenta").setValue(null);
            formContext.getAttribute("atos_cuenta").setSubmitMode("always");
        }
        if (campoVacio(formContext, "atos_cuentabancaria") == false) // Si tiene valor lo vacÃ­a
        {
            formContext.getAttribute("atos_cuentabancaria").setValue(null);
            formContext.getAttribute("atos_cuentabancaria").setSubmitMode("always");
        }
        formContext.getControl("atos_swift").setDisabled(true);
        formContext.getControl("atos_iban").setDisabled(true);
        formContext.getControl("atos_entidadbancaria").setDisabled(true);
        formContext.getControl("atos_sucursalbancaria").setDisabled(true);
        formContext.getControl("atos_digitocontrol").setDisabled(true);
        formContext.getControl("atos_cuenta").setDisabled(true);
    }
}



/**
// <summary>
// Valida si la cuenta bancaria es correcta o no
// </summary>
*/
function validaCuentaBancaria(i_entidad, i_oficina, i_digito, i_cuenta) {
    // Funcion recibe como parÃ¡metro la entidad, la oficina, 
    // el digito (concatenaciÃ³n del de control entidad-oficina y del de control entidad)
    // y la cuenta. Espera los valores con 0's a la izquierda.
    // Devuelve true o false.
    // NOTAS:
    // Formato deseado de los parÃ¡metros:
    // - i_entidad (4)
    // - i_oficina (4)
    // - i_digito (2)
    // - i_cuenta (10)
    var wtotal, wcociente, wresto;
    if (i_entidad.length != 4) {
        return false;
    }
    if (i_oficina.length != 4) {
        return false;
    }
    if (i_digito.length != 2) {
        return false;
    }
    if (i_cuenta.length != 10) {
        return false;
    }
    wtotal = i_entidad.charAt(0) * 4;
    wtotal += i_entidad.charAt(1) * 8;
    wtotal += i_entidad.charAt(2) * 5;
    wtotal += i_entidad.charAt(3) * 10;
    wtotal += i_oficina.charAt(0) * 9;
    wtotal += i_oficina.charAt(1) * 7;
    wtotal += i_oficina.charAt(2) * 3;
    wtotal += i_oficina.charAt(3) * 6;
    // busco el resto de dividir wtotal entre 11
    wcociente = Math.floor(wtotal / 11);
    wresto = wtotal - (wcociente * 11);
    //
    wtotal = 11 - wresto;
    if (wtotal == 11) {
        wtotal = 0;
    }
    if (wtotal == 10) {
        wtotal = 1;
    }
    if (wtotal != i_digito.charAt(0)) {
        return false;
    }
    //hemos validado la entidad y oficina
    //-----------------------------------
    wtotal = i_cuenta.charAt(0) * 1;
    wtotal += i_cuenta.charAt(1) * 2;
    wtotal += i_cuenta.charAt(2) * 4;
    wtotal += i_cuenta.charAt(3) * 8;
    wtotal += i_cuenta.charAt(4) * 5;
    wtotal += i_cuenta.charAt(5) * 10;
    wtotal += i_cuenta.charAt(6) * 9;
    wtotal += i_cuenta.charAt(7) * 7;
    wtotal += i_cuenta.charAt(8) * 3;
    wtotal += i_cuenta.charAt(9) * 6;

    // busco el resto de dividir wtotal entre 11
    wcociente = Math.floor(wtotal / 11);
    wresto = wtotal - (wcociente * 11);
    //
    wtotal = 11 - wresto;
    if (wtotal == 11) { wtotal = 0; }
    if (wtotal == 10) { wtotal = 1; }

    if (wtotal != i_digito.charAt(1)) {
        return false;
    }
    // hemos validado la cuenta corriente

    return true;
}

/**
// <summary>
// Calcula el Iban de un nÃºmero de cuenta
// </summary>
*/
function calcularIban(formContext, ccc) {

    formContext.ui.clearFormNotification("9");
    //Limpiamos el numero de IBAN
    var cccaux = ccc.toUpperCase();  //Todo a Mayus
    cccaux = trim(cccaux); //Quitamos blancos de principio y final.
    cccaux = cccaux.replace(/\s/g, "");  //Quitamos blancos del medio.
    cccaux = cccaux + "142800"; // AÃ±adimos el cÃ³digo de EspaÃ±a

    var trozo = cccaux.substr(0, 5);
    var resto = trozo % 97;

    trozo = cccaux.substr(5, 5);
    var trozoaux = resto + trozo;
    resto = trozoaux % 97;

    trozo = cccaux.substr(10, 5);
    trozoaux = resto + trozo;
    resto = trozoaux % 97;

    trozo = cccaux.substr(15, 5);
    trozoaux = resto + trozo;
    resto = trozoaux % 97;

    trozo = cccaux.substr(20, 6);
    trozoaux = resto + trozo;
    resto = trozoaux % 97;

    digitocontrol = 98 - resto;
    if (digitocontrol < 10) digitocontrol = '0' + String(digitocontrol);
    formContext.getAttribute("atos_iban").setValue("ES" + digitocontrol);
}

/**
// <summary>
// Elimina los espacios de una cadena
// </summary>
*/
function trim(myString) {
    return myString.replace(/^\s+/g, '').replace(/\s+$/g, '');
}

/**
// <summary>
// Calcula el Iban si la cuenta bancaria es correcta
// </summary>
*/
function validaCuenta(executionContext) {


       var  formContext = executionContext.getFormContext();
    
    formContext.ui.clearFormNotification("8");
    var entidad = formContext.getAttribute("atos_entidadbancaria").getValue();
    var oficina = formContext.getAttribute("atos_sucursalbancaria").getValue();
    var dc = formContext.getAttribute("atos_digitocontrol").getValue();
    var cuenta = formContext.getAttribute("atos_cuenta").getValue();

    var validacion = true;

    if (entidad != null && oficina != null && dc != null && cuenta != null) {
        if (entidad.length == 4 && oficina.length == 4 && dc.length == 2 && cuenta.length == 10) {
            validacion = validaCuentaBancaria(entidad, oficina, dc, cuenta);
            if (!validacion)
                formContext.ui.setFormNotification("Cuenta bancaria incorrecta.", "ERROR", "8"); // alert("Cuenta bancaria incorrecta");
            else
                calcularIban(formContext, entidad + oficina + dc + cuenta);
        }
        else {
            validacion = false;
            formContext.ui.setFormNotification("Cuenta bancaria incorrecta.", "ERROR", "8");
            //alert("Cuenta bancaria incorrecta.");
        }

    }
    return validacion;
}