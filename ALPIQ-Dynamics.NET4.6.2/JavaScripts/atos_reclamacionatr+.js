/*
 File="atos_cierre.js" 
 Copyright (c) Atos. All rights reserved.

 Fecha 		Codigo  Version Descripcion
 16/03/2021 223231  1.0.1.1 Cambi el FechXml con sentencia no-lock='true'

*/

/**
// <summary>
//Se encarga de realizar todas las validaciones de la reclamación según el tipo,  antes de ejecutar el WF.
// De momento se hacen solo validaciones para las reclamaciones que sean  de envio de peticiones de reclamacion(R1-01) 
// y de envio de informacion adicional (R1-04)
// </summary>
*/
function Reclamacionatr_OnSave(obj) {
    var serverUrl = Xrm.Page.context.getClientUrl();
    var continuar = true;
    limpiaMensajeError("2", "");
    var proceso = "";
    var paso = "";
    if (Xrm.Page.data.entity.attributes.get("atos_procesoid").getValue() != null) {
        proceso = obtenerValorEntidad(Xrm.Page.data.entity.attributes.get("atos_procesoid").getValue()[0].id, "atos_procesoatr", "atos_procesoatrid", "atos_codigoproceso")
    }

    if (Xrm.Page.data.entity.attributes.get("atos_pasoid").getValue() != null) {
        paso = obtenerValorEntidad(Xrm.Page.data.entity.attributes.get("atos_pasoid").getValue()[0].id, "atos_pasoatr", "atos_pasoatrid", "atos_codigopaso")
    }


    // solo se validaran los casos descritos en la cabecera
    if (!(proceso == "R1" && (paso == "01" || paso == "04"))) {
        return true;
    }



    if (!validarReclamacion(proceso, paso, obj)) {
        continuar = false;
    }
    // si  dio algun fallo no continua
    if (continuar == false) {
        cancelarGuardado(obj);
    }


}

// <summary>
// Se encarga de validar los datos propios de la entidad reclamación
// </summary>
function validarReclamacion(proceso, paso, obj) {
    var validado = true;
    // si es tipo  peticiones de reclamacion(R1-01)
    if (paso == "01") {

        // se obtiene tipo y subtipo
        if (Xrm.Page.data.entity.attributes.get("atos_tipoid").getValue() == null) {
            mensajeError("Es obligatorio introducir el tipo y subtipo de la reclamación en : \"Envio reclamación/petición distribuidor\" ", "ERROR", "2", "atos_tipoid");
            cancelarGuardado(obj);
            return true;
        }

        if (Xrm.Page.data.entity.attributes.get("atos_subtipoid").getValue() == null) {
            mensajeError("Es obligatorio introducir el tipo  y subtipo de la reclamación en : \"Envio reclamación/petición distribuidor\" ", "ERROR", "2", "atos_subtipoid");
            cancelarGuardado(obj);
            return true;
        }

        var tipo = obtenerValorEntidad(Xrm.Page.data.entity.attributes.get("atos_tipoid").getValue()[0].id, "atos_tipodereclamacionopeticion", "atos_tipodereclamacionopeticionid", "atos_codigo")
        var subtipo = obtenerValorEntidad(Xrm.Page.data.entity.attributes.get("atos_subtipoid").getValue()[0].id, "atos_subtipodereclamacionopeticion", "atos_subtipodereclamacionopeticionid", "atos_codigo")

        //validamos subtipo 
        if (!validarSubtipo(tipo, subtipo)) {
            mensajeError("El tipo seleccionado en \"Envío reclamación/petición distribuidor\" no puede ser de este subtipo para una reclamación", "ERROR", "2", "atos_subtipoid");
            cancelarGuardado(obj);
            return true;
        }


        // validamos el cups segun subtipo
        if (hayQueValidarCampo("CUPS", subtipo)) {
            if (Xrm.Page.data.entity.attributes.get("atos_instalacionid").getValue() == null) {
                mensajeError("Es obligatorio seleccionar una instalación para este subtipo de la reclamación al ser necesario un CUPS", "ERROR", "2", "atos_instalacionid");
                cancelarGuardado(obj);
                return true;
            }
        }

        // validamos  comentarios
        if (hayQueValidarCampo("COMENTARIOS", subtipo)) {
            if (Xrm.Page.data.entity.attributes.get("atos_comentarios").getValue() == null) {
                mensajeError("Es obligatorio introducir un valor en comentarios ", "ERROR", "2", "atos_comentarios");
                cancelarGuardado(obj);
                return true;
            }
        }

        // validamos variables detalle reclamacion segun subtipo y lecturas dentro de clada detalle segun subtipo 
        if (!validarVariablesDetalleReclamacionInformacion(Xrm.Page.data.entity.getId(), subtipo)) {
            cancelarGuardado(obj);
            return true;
        }

        //validar cliente
        if (!validarCliente(subtipo)) {
            cancelarGuardado(obj);
            return true;
        }

        // validar reclamante
        if (!validarReclamante(subtipo)) {
            cancelarGuardado(obj);
            return true;
        }

        // validar documentos registrados 
        if (!validarDocumentos(Xrm.Page.data.entity.getId())) {
            cancelarGuardado(obj);
            return true;
        }

    }
    //si es tipo envio de informacion adicional (R1-04)
    if (paso == "04") {


        // validar Fecha informacion
        if (Xrm.Page.data.entity.attributes.get("atos_fechainformacion").getValue() == null) {
            mensajeError("Es obligatorio fecha de información en: \"Envío información adicional\" ", "ERROR", "2", "atos_fechainformacion");
            cancelarGuardado(obj);
            return true;
        }
        // el comentario obligatorio
        if (Xrm.Page.data.entity.attributes.get("atos_comentarios").getValue() == null) {
            mensajeError("Es obligatorio introducir un valor en comentarios ", "ERROR", "2", "atos_comentarios");
            cancelarGuardado(obj);
            return true;
        }

        // validar Variables de aportacion de informacion
        if (!validarVariablesAportacionInformacion(Xrm.Page.data.entity.getId())) {
            cancelarGuardado(obj);
            return true;
        }

        // validar documentos
        if (!validarDocumentos(Xrm.Page.data.entity.getId())) {
            cancelarGuardado(obj);
            return true;
        }

    }

    return validado;
}

// ********************************************************************************************************************************
// ********************************************************************************************************************************
// **** FUNCIONES GENERALES                                                                                              **********
// ********************************************************************************************************************************
// ********************************************************************************************************************************
function obtenerValorEntidad(entidadId, nombreEntidad, nombreCampoIdentificador, nombreCampoDescripcion) {

    var idencontrado = null;
    var fetchXml =
/* 223231 +1*/ //"<fetch mapping='logical'>"+
        "<fetch mapping='logical' no-lock='true'>" +
		  "<entity name='" + nombreEntidad + "' > " +
			"<attribute name='" + nombreCampoDescripcion + "' alias='" + nombreCampoDescripcion + "' /> " +
  			"<filter>" +
			   "<condition attribute='" + nombreCampoIdentificador + "' operator='eq' value='" + entidadId + "' />" +
			"</filter>" +
		  "</entity> " +
		"</fetch>";

    var registros = XrmServiceToolkit.Soap.Fetch(fetchXml);

    if (registros.length > 0) {
        valor = registros[0].attributes[nombreCampoDescripcion].value;
    }
    return valor;
}



// ********************************************************************************************************************************
// ********************************************************************************************************************************
// **** FUNCIONES PARA EL TIPO ENVIO DE INFORMACION ADICIONAL (R1-04)                                                    **********
// ********************************************************************************************************************************
// ********************************************************************************************************************************


// <summary>
// Se encarga de validar los datos propios de una  variable de aportacion de informacion y el numero de ellos (10 Max)
// </summary>
function validarVariablesAportacionInformacion(reclamacionId) {
    var validado = true;

    //obtenemos todos las variables que tiene la reclamacion (min =1 - max=10)
    var fetchXml =
/* 223231 +1*/ //"<fetch mapping='logical'>"+
        "<fetch mapping='logical' no-lock='true'>" +
		  "<entity name='atos_variablesaportacioninfreclamacion' > " +
			"<attribute name='atos_tipoinformacionid' alias='atos_tipoinformacionid' /> " +
			"<attribute name='atos_descripcionpeticioninformacion' alias='atos_descripcionpeticioninformacion' /> " +
			"<attribute name='atos_variableid' alias='atos_variableid' /> " +
            "<attribute name='atos_valor' alias='atos_valor' /> " +
			"<link-entity name='atos_reclamacion' from='atos_reclamacionid' to='atos_reclamacionasociadaid' link-type='outer' alias='atos_reclamacion' > " +
			  "<attribute name='atos_name' alias='reclamacion' /> " +
			"</link-entity> " +
  			"<filter>" +
			   "<condition attribute='atos_reclamacionasociadaid' operator='eq' value='" + reclamacionId + "' />" +
			"</filter>" +
		  "</entity> " +
		"</fetch>";

    var registros = XrmServiceToolkit.Soap.Fetch(fetchXml);

    if (registros.length > 10) {
        mensajeError("No se pueden introducir más de 10  valores para variables de aportación de informacion en : \"Envio información adicional\" ", "ERROR", "2", "");
        validado = false;
        return validado;
    }
    else {
        // validamos cada unos de las variables de aportacion de informacion

        for (i = 0; i < registros.length; i++) {

            if (registros[i].attributes["atos_tipoinformacionid"] == null) {
                mensajeError("Es obligatorio introducir el tipo de informacion en todos los \"Envio información adicional\" ", "ERROR", "2", "");
                validado = false;
                return validado;
            }
            else {

                var esTipo06 = esTipoInformacion06(registros[i].attributes["atos_tipoinformacionid"].id);

                // si el tipo es 06 es obligatorio que tenga descripcion
                if (esTipo06 && registros[i].attributes["atos_descripcionpeticioninformacion"].value == null) {

                    mensajeError("Es obligatorio introducir descripción en todos los registros de : \"Envio información adicional\" de tipo 06 ", "ERROR", "2", "");
                    validado = false;
                    return validado;
                }
            }

            if ((registros[i].attributes["atos_variableid"].id != null && registros[i].attributes["atos_valor"].value == null) ||
             (registros[i].attributes["atos_variableid"].id == null && registros[i].attributes["atos_valor"].value != null)) {

                mensajeError("Es obligatorio introducir el par Variable/Valor en todos los registros de: \"Envio información adicional\" si al menos uno de los valores esta relleno ", "ERROR", "2", "");
                validado = false;
                return validado;

            }

        }
    }
    return validado;
}


// <summary>
//comprueba si el tipo de informacion es 06
// </summary>
function esTipoInformacion06(tipoInformacionId) {

    var fetchXml =
/* 223231 +1*/ //"<fetch mapping='logical'>"+
        "<fetch mapping='logical' no-lock='true'>" +
		  "<entity name='atos_tipodeinformacionadicional' > " +
			"<attribute name='atos_codigo' alias='atos_codigo' /> " +
			"<link-entity name='atos_variablesaportacioninfreclamacion' from='atos_tipoinformacionid' to='atos_tipodeinformacionadicionalid' link-type='outer' alias='atos_variablesaportacioninfreclamacion' > " +
			  "<attribute name='atos_name' alias='variableinformacionreclamacion' /> " +
			"</link-entity> " +
  			"<filter>" +
			   "<condition attribute='atos_tipodeinformacionadicionalid' operator='eq' value='" + tipoInformacionId + "' />" +
			"</filter>" +
		  "</entity> " +
		"</fetch>";


    var registros = XrmServiceToolkit.Soap.Fetch(fetchXml);

    if (registros.length > 0) {

        if (registros[0].attributes["atos_codigo"].value == "06") {

            return true;
        }
    }
    return false;
}





// ********************************************************************************************************************************
// ********************************************************************************************************************************
// **** FUNCIONES PARA EL TIPO PETICION DE RECLAMACION (R1-01)                                                           **********
// ********************************************************************************************************************************
// ********************************************************************************************************************************

// <summary>
// Se encarga de validar el cliente de una peticion de reclamacion de tipo (R1-01)
// </summary>
function validarCliente(subtipo) {

    var validado = true;
    //validamos si es obligatorio que haya cliente segun tipo/subtipo 

    // se valida si tiene instalación los campos cif/nif y nombre y apellidos os razon social ya son obligatorios dentro de una instalacion
    if (hayQueValidarCampo("NIF", subtipo) || hayQueValidarCampo("NOMBRE", subtipo)) {
        if (Xrm.Page.data.entity.attributes.get("atos_instalacionid").getValue() == null) {
            mensajeError("Es obligatorio seleccionar una instalación para este subtipo de la reclamación al ser necesario un NIF/CIF y un nombre y apellidos/razón social", "ERROR", "2", "atos_instalacionid");
            return false;
        }
    }

    //validar TipoCIFNIF
    //validar Identificador

    //validar Nombre
    //validar PrimerApellido

    //validar RazonSocial


    //****************************************************************
    // ESTOS CAMPOS NOSE ESTAN VALIDADANDO POR QUE NO EXISTEN 
    //****************************************************************
    //validar PrefijoPais (FAX)
    //validar Numero(FAX)

    //validar PrefijoPais (TELEFONO)
    //validar Numero(TELEFONO)

    //Validar correo electronico   
    // direccion y Indicadortipo de dirreccion ( al no estar normalizada se pasa el tipo de direccion "F" que corresponde a no disponible y evitamos tener que meterla

    return validado;
}


// <summary>
// Se encarga de validar el reclamante de una peticion de reclamacion de tipo (R1-01)
// </summary>
function validarReclamante(subtipo) {
    var validado = true;

    //validar TipoReclamante

    if (Xrm.Page.data.entity.attributes.get("atos_tiporeclamanteid").getValue() == null) {
        mensajeError("Es obligatorio introducir el tipo de reclamante de la reclamación en : \"tipo de reclamante\" ", "ERROR", "2", "atos_tiporeclamanteid");
        validado = false;
        return validado;
    }


    var fetchXml =
/* 223231 +1*/ //"<fetch mapping='logical'>"+
        "<fetch mapping='logical' no-lock='true'>" +
		  "<entity name='atos_tiporeclamante' > " +
			  "<all-attributes />" +
			"<link-entity name='atos_reclamacion' from='atos_tiporeclamanteid' to='atos_tiporeclamanteid' link-type='outer' alias='atos_reclamacion' > " +
			  "<attribute name='atos_name' alias='reclamacion' /> " +
			"</link-entity> " +
  			"<filter>" +
			   "<condition attribute='atos_tiporeclamanteid' operator='eq' value='" + Xrm.Page.data.entity.attributes.get("atos_tiporeclamanteid").getValue()[0].id + "' />" +
			"</filter>" +
		  "</entity> " +
		"</fetch>";

    var registros = XrmServiceToolkit.Soap.Fetch(fetchXml);

    // si es distitinto de 06 comercializadora se tiene que validar todos los campos sino no son obligatorios
    if (registros[0].attributes["atos_codigo"].value == "06") {

        return true;
    }




    //validar TipoCIFNIF
    //validar Identificador
    if (Xrm.Page.data.entity.attributes.get("atos_tipodocumentoreclamante").getValue() == null || Xrm.Page.data.entity.attributes.get("atos_numerodocumentoreclamante").getValue() == null) {
        mensajeError("Es obligatorio introducir el tipo de documento y documento del reclamante de la reclamación en : \"tipo de reclamante\" ", "ERROR", "2", "atos_tipodocumentoreclamante");
        validado = false;
        return validado;
    }

    var numeroDocumento = Xrm.Page.data.entity.attributes.get("atos_numerodocumentoreclamante").getValue();
    var tipoDocumento = proceso = Xrm.Page.data.entity.attributes.get("atos_tipodocumentoreclamante").getValue();

    if (tipoDocumento == "300000000" && !ValidaNIF(numeroDocumento)) {
        mensajeError("El documento del reclamante de la reclamación no tiene el formato correcto en : \"tipo de reclamante\" ", "ERROR", "2", "atos_numerodocumentoreclamante");
        validado = false;
        return validado;
    }
    if (tipoDocumento == "300000001" && !ValidaCIF(numeroDocumento)) {
        mensajeError("El documento del reclamante de la reclamación  no tiene el formato correcto en : \"tipo de reclamante\" ", "ERROR", "2", "atos_numerodocumentoreclamante");
        validado = false;
        return validado;
    }

    if (tipoDocumento == "300000002" && !valida_NIE(numeroDocumento)) {
        mensajeError("El documento del reclamante de la reclamación no tiene el formato correcto en : \"tipo de reclamante\" ", "ERROR", "2", "atos_numerodocumentoreclamante");
        validado = false;
        return validado;
    }
    //validar Nombre
    //validar PrimerApellido
    if ((tipoDocumento == "300000000" || tipoDocumento == "300000002") && (Xrm.Page.data.entity.attributes.get("atos_nombrereclamante").getValue() == null || Xrm.Page.data.entity.attributes.get("atos_primerapellidoreclamante").getValue() == null)) {
        mensajeError("El nombre y  primer apellidos del reclamante de la reclamación es obligatorio en : \"tipo de reclamante\" ", "ERROR", "2", "atos_nombrereclamante");
        validado = false;
        return validado;
    }


    //validar RazonSocial
    if (tipoDocumento == "300000001" && Xrm.Page.data.entity.attributes.get("atos_sociedadreclamante").getValue() == null) {
        mensajeError("La razón social del reclamante de la reclamación es obligatorio en : \"tipo de reclamante\" ", "ERROR", "2", "atos_sociedadreclamante");
        validado = false;
        return validado;
    }

    //TODO: faltan teléfono , fax y email   no se tienen
    //validar PrefijoPais (FAX)
    //validar Numero(FAX) (formato)
    //validar PrefijoPais (TELEFONO)
    //validar Numero(TELEFONO) (formato)
    //Validar correo electronico  ( formato)  

    return validado;
}



// <summary>
// Se encarga de validar las  variables de detalle de reclamación segun el subtipo de reclamación
// </summary>
function validarVariablesDetalleReclamacionInformacion(reclamacionId, subtipo) {
    var validado = true;
    var variableDetalleReclamacionId;
    //obtenemos todos las variables que tiene la reclamacion ( max=10)
    var fetchXml =
/* 223231 +1*/ //"<fetch mapping='logical'>"+
        "<fetch mapping='logical' no-lock='true'>" +
		  "<entity name='atos_variablesdetallereclamacion' > " +
			  "<all-attributes />" +
			"<link-entity name='atos_reclamacion' from='atos_reclamacionid' to='atos_reclamacionasociadaid' link-type='outer' alias='atos_reclamacion' > " +
			  "<attribute name='atos_name' alias='reclamacion' /> " +
			"</link-entity> " +
  			"<filter>" +
			   "<condition attribute='atos_reclamacionasociadaid' operator='eq' value='" + reclamacionId + "' />" +
			"</filter>" +
		  "</entity> " +
		"</fetch>";

    var registros = XrmServiceToolkit.Soap.Fetch(fetchXml);

    if (registros.length > 10) {
        mensajeError("No se pueden introducir mas de 10 variables de detalles de reclamación en : \"Envio información adicional\" ", "ERROR", "2", "");
        validado = false;
        return validado;
    }

    var arrayDetalleReclamacion = ["001", "003", "004", "007", "008", "009", "010",
								   "011", "012", "013", "014", "020", "021", "022",
								   "023", "024", "025", "026", "031", "034", "036",
								   "037", "039", "040", "041", "046", "047", "049",
								   "055", "056"];


    if (registros.length == 0 && arrayDetalleReclamacion.indexOf(subtipo) > -1) {
        if (!(Xrm.Page.data.entity.getId() == null || Xrm.Page.data.entity.getId() == "")) {
            mensajeError("Se debe introducir entre 1 y 10 variables de detalles de reclamación en : \"Envio información adicional\" ", "ERROR", "2", "");
            validado = false;
            return validado;
        }
        else {
            mensajeError("Se debe introducir entre 1 y 10 variables de detalles de reclamación en : \"Envio información adicional\" ", "WARNING", "2", "");

        }

    }

    for (i = 0; i < registros.length; i++) {
        
        // validar  NumExpedienteAcometida
        if (hayQueValidarCampo("NUMEXPEDIENTEACOMETIDA", subtipo) && registros[i].attributes["atos_numeroexpedienteacometida"] == null) {
               mensajeError("El número de expediente de acomentida de una variable detalle reclamación en : \"Envio reclamación/petición al distribuidor\" es obligatorio para este tipo/subtipo ", "ERROR", "2", "");
            validado = false;
            return validado;
        }

        // validar  NumExpedienteFraude
        if (hayQueValidarCampo("NUMEXPEDIENTEFRAUDE", subtipo) && registros[i].attributes["atos_numeroexpedientefraude"] == null) {
            mensajeError("El número de expediente de fraude de una variable detalle reclamación en : \"Envio reclamación/petición al distribuidor\" es obligatorio para este tipo/subtipo ", "ERROR", "2", "");
            validado = false;
            return validado;
        }

        // validar FechaIncidente
        if (hayQueValidarCampo("FECHAINCIDENTE", subtipo) && registros[i].attributes["atos_fechaincidente"] == null) {
            mensajeError("la Fecha incidente de una variable detalle reclamación en : \"Envio reclamación/petición al distribuidor\" es obligatorio para este tipo/subtipo", "ERROR", "2", "");
            validado = false;
            return validado;
        }
        // validar NumFacturaATR
        if (hayQueValidarCampo("NUMEROFACTURAATR", subtipo) && registros[i].attributes["atos_numerofacturaatr"] == null) {
            mensajeError("El número de la factura de una variable detalle reclamación en : \"Envio reclamación/petición al distribuidor\" es obligatorio para este tipo/subtipo", "ERROR", "2", "");
            validado = false;
            return validado;
        }
        // validar TipoConceptoFacturado
        if (hayQueValidarCampo("TIPOCONCEPTOFACTURA", subtipo) && registros[i].attributes["atos_numerofacturaatr"] == null) {
            mensajeError("El tipo concepto de la factura de una variable detalle reclamación en : \"Envio reclamación/petición al distribuidor\" es obligatorio para este tipo/subtipo ", "ERROR", "2", "");
            validado = false;
            return validado;
        }

        // validar FechaLectura
        if (hayQueValidarCampo("FECHALECTURA", subtipo) && registros[i].attributes["atos_fechalectura"] == null) {
            mensajeError("La fecha de lectura de una variable detalle reclamación en : \"Envio reclamación/petición al distribuidor\" es obligatoriopara este tipo/subtipo ", "ERROR", "2", "");
            validado = false;
            return validado;
        }
        // NO ESTA
        // validar CodigoDH
        if (hayQueValidarCampo("CODIGODH", subtipo) && registros[i].attributes["atos_codigodh"] == null) {
            mensajeError("El código de discriminación horaria de una variable detalle reclamación en : \"Envio reclamación/petición al distribuidor\" es obligatorio para este tipo/subtipo ", "ERROR", "2", "");
            validado = false;
            return validado;
        }

        // validar CodigoIncidencia
        if (hayQueValidarCampo("CODIGOINCIDENCIA", subtipo) && registros[i].attributes["atos_codigoincidenciaid"] == null) {
            mensajeError("El código de incidencia de una variable detalle reclamación en : \"Envio reclamación/petición al distribuidor\" es obligatorio para este tipo/subtipo ", "ERROR", "2", "");
            validado = false;
            return validado;
        }

        // validar CodigoSolicitud
        if ((hayQueValidarCampo("CODIGOSOLICITUD", subtipo) && registros[i].attributes["atos_codigosolicitud"] == null)) {
            mensajeError("El código de solicitud  de una variable detalle reclamación en : \"Envio reclamación/petición al distribuidor\" es obligatorio para este tipo/subtipo y tiene que tener 12 caracteres ", "ERROR", "2", "");
            validado = false;
            return validado;
        }

        // validar ParametroContratacion
        if (hayQueValidarCampo("PARAMETROCONTRATACION", subtipo) && registros[i].attributes["atos_parametrocontratacion"] == null) {
            mensajeError("El parametro de contratación de una variable detalle reclamación en : \"Envio reclamación/petición al distribuidor\" es obligatorio para este tipo/subtipo ", "ERROR", "2", "");
            validado = false;
            return validado;
        }

        // validar ConceptoDisconformidad
        if (hayQueValidarCampo("CONCEPTODISCONFORMIDAD", subtipo) && registros[i].attributes["atos_conceptodisconformidad"] == null) {
            mensajeError("El concepto de disconformidad de una variable detalle reclamación en : \"Envio reclamación/petición al distribuidor\" es obligatorio para este tipo/subtipo ", "ERROR", "2", "");
            validado = false;
            return validado;
        }

        // validar TipoDeAtencionIncorrecta
        if (hayQueValidarCampo("TIPOANTENCIONINCORRECTO", subtipo) && registros[i].attributes["atos_tipodeatencionincorrectaid"] == null) {
            mensajeError("El tipo de incidencia incorrecto de una variable detalle reclamación en : \"Envio reclamación/petición al distribuidor\" es obligatorio para este tipo/subtipo ", "ERROR", "2", "");
            validado = false;
            return validado;
        }

        // validar IBAN
        if (hayQueValidarCampo("CTABANCO", subtipo) && registros[i].attributes["atos_iban"] == null) {
            mensajeError("El IBAN de una variable detalle reclamacion en : \"Envio reclamación/petición al distribuidor\" es obligatorio para este tipo/subtipo ", "ERROR", "2", "");
            validado = false;
            return validado;
        }
        // validar CodigoSolicitudReclamacion
        if (hayQueValidarCampo("CODIGOANTERIOR", subtipo) && registros[i].attributes["atos_codigosolicitudreclamacion"] == null) {
            mensajeError("El código de solicitid reclamación anterior de una variable detalle reclamación en : \"Envio reclamación/petición al distribuidor\" es obligatorio para este tipo/subtipo y tiene que tener 12 caracteres", "ERROR", "2", "");
            validado = false;
            return validado;
        }
        // validar FechaDesde
        if (hayQueValidarCampo("FECHADESDE", subtipo) && registros[i].attributes["atos_fechadesde"] == null) {
            mensajeError("La fecha desde de una variable detalle reclamación en : \"Envio reclamación/petición al distribuidor\" es obligatorio para este tipo/subtipo ", "ERROR", "2", "");
            validado = false;
            return validado;
        }
        // validar FechaHasta
        if (hayQueValidarCampo("FECHAHASTA", subtipo) && registros[i].attributes["atos_fechahasta"] == null) {
            mensajeError("La fecha hasta de una variable detalle reclamación en : \"Envio reclamación/petición al distribuidor\" es obligatorio para este tipo/subtipo ", "ERROR", "2", "");
            validado = false;
            return validado;
        }
        // validar ImporteReclamado

        if (hayQueValidarCampo("IMPORTERECLAMADO", subtipo) && registros[i].attributes["atos_importereclamado"] == null) {
            mensajeError("El importe reclamado de una variable detalle reclamación en : \"Envio reclamación/petición al distribuidor\" es obligatorio para este tipo/subtipo ", "ERROR", "2", "");
            validado = false;
            return validado;
        }

        //**** SE ELIMINA ESTA COMPROBACION PARA COGER LA UBICACION DE LA INSTALACION *****//
        // validar UbicacionIncidencia

        //if (hayQueValidarCampo("UBICACION", subtipo) && registros[i].attributes["atos_ubicacionincidencia"] == null) {
        //    mensajeError("La ubicación de una variable detalle reclamación en : \"Envio reclamación/petición al distribuidor\" es obligatoriopara este tipo/subtipo ", "ERROR", "2", "");
        //    validado = false;
        //    return validado;
        //}

        // validar lecturas segun subtipos 
        if (!validarLecturasReclamacion(registros[i].attributes["atos_variablesdetallereclamacionid"].value, subtipo)) {
            validado = false;
            return validado;
        }
        // validar contacto
        if (!validarContacto(registros[i], subtipo)) {
            validado = false;
            return validado;
        }
    }
    return validado;
}

// <summary>
// Se encarga de validar las lecturas asociadas a una variable de detalle de reclamación
// </summary>
function validarLecturasReclamacion(VariableDetalleReclamacionId, subtipo) {
    var validado = true;

    var fetchXml =
/* 223231 +1*/ //"<fetch mapping='logical'>"+
        "<fetch mapping='logical' no-lock='true'>" +
		  "<entity name='atos_lecturasaportadas' > " +
			  "<all-attributes />" +
			"<link-entity name='atos_variablesdetallereclamacion' from='atos_variablesdetallereclamacionid' to='atos_variabledetallereclamacionasociadaid' link-type='outer' alias='atos_variablesdetallereclamacion' > " +
			  "<attribute name='atos_name' alias='variableDetalleReclamacion' /> " +
			"</link-entity> " +
  			"<filter>" +
			   "<condition attribute='atos_variabledetallereclamacionasociadaid' operator='eq' value='" + VariableDetalleReclamacionId + "' />" +
			"</filter>" +
		  "</entity> " +
		"</fetch>";

    var registros = XrmServiceToolkit.Soap.Fetch(fetchXml);

    if (registros.length > 20) {
        mensajeError("No se pueden introducir mas de 20 lecturas por variable de detalle de reclamacion en : \"Envio información adicional\" ", "ERROR", "2", "");
        validado = false;
        return validado;
    }

    if (hayQueValidarCampo("UBICACION", subtipo) && registros.length == 0) {
        mensajeError("Hay que introducir al menos una lectura por variable de detalle de reclamación en : \"Envio información adicional\" para este tipo/subtipo ", "ERROR", "2", "");
        validado = false;
        return validado;
    }

    return validado;
}


// <summary>
// Se encarga de validar el contacto de una variable de detalle de reclamación
// </summary>
function validarContacto(registro, subtipo) {
    var validado = true;

    if (hayQueValidarCampo("CONTACTO", subtipo) && registro.attributes["atos_nombrecontacto"] == null && registro.attributes["atos_razonsocial"] == null) {
        mensajeError("Hay que introducir un contacto para una variable de detalle de reclamación en : \"Envio información adicional\" para este tipo/subtipo ", "ERROR", "2", "");
        validado = false;
        return validado;

    }

    //Validar NombreDePila
    if (registro.attributes["atos_nombrecontacto"] != null && registro.attributes["atos_nombrecontacto"] != false && registro.attributes["atos_nombrecontacto"] != true && registro.attributes["atos_razonsocial"] != null) {
        mensajeError("un contacto no puede tener nombre ,apellidos y razon social a la vez en una variable de detalle de reclamación en : \"Envio información adicional\" para este tipo/subtipo ", "ERROR", "2", "");
        validado = false;
        return validado;
    }
    //Validar PrimerApellido
    if ((registro.attributes["atos_nombrecontacto"] != null && registro.attributes["atos_nombrecontacto"] != false && registro.attributes["atos_nombrecontacto"] != true && registro.attributes["atos_primerapellidocontacto"] == null) ||
          (registro.attributes["atos_nombrecontacto"] == null && registro.attributes["atos_primerapellidocontacto"] != null)) {
        mensajeError("Se debe introducir nombre y apellidos si se introduce un contacto personal en una variable de detalle de reclamacion en : \"Envio información adicional\" para este tipo/subtipo ", "ERROR", "2", "");
        validado = false;
        return validado;
    }

    //Validar  PrefijoPais (se añade en en proceso de creación de xml 34(españa)
    //validar Numero telefono
    if ((registro.attributes["atos_nombrecontacto"] != null && registro.attributes["atos_nombrecontacto"] != false && registro.attributes["atos_nombrecontacto"] != true || registro.attributes["atos_razonsocial"] != null) && registro.attributes["atos_telefono"] == null) {
        mensajeError("Se debe introducir un número de teléfono de 9 cifras para todos los contacto personales  de  las variables de detalle de reclamación en : \"Envio información adicional\" para este tipo/subtipo ", "ERROR", "2", "");
        validado = false;
        return validado;
    }
    // formato
    if ((registro.attributes["atos_nombrecontacto"] != null && registro.attributes["atos_nombrecontacto"] != false && registro.attributes["atos_nombrecontacto"] != true || registro.attributes["atos_razonsocial"] != null) && registro.attributes["atos_telefono"] != null) {
        if (!(/\d{9}$/.test(registro.attributes["atos_telefono"].value))) {
            mensajeError("Se debe introducir un número de teléfono de 9 cifras para todos los contacto personales  de  las variables de detalle de reclamación en : \"Envio información adicional\" para este tipo/subtipo ", "ERROR", "2", "");
            validado = false;
            return validado;
        }
    }


    return validado;
}


// <summary>
// Se encarga de decirnos si hay que validar un campo si se este se encunetra en el subtipo que hay que valdiar
// Posibilidades para campo 
// "NIF" 
// "NOMBRE" 
// "TELEFONO" 
// "CUPS" 
// "FECHAINCIDENTE" 
// "COMENTARIOS" 
// "CODIGOINCIDENCIA" 
// "CONTACTO" 
// "NUMEROFACTURAATR" 
// "TIPOCONCEPTOFACTURA" 
// "LECTURA" 
// "FECHALECTURA" 
// "FECHADESDE" 
// "FECHAHASTA" 
// "UBICACION" 
// "CODIGOSOLICITUD" 
// "CTABANCO" 
// "SOLNUEVOSSUMINISTROS" 
// "CODIGOANTERIOR" 
// "IMPORTERECLAMADO" 
// "TIPOANTENCIONINCORRECTO" 
// </summary>
function hayQueValidarCampo(campo, subtipo) {

    // Hay 26 posibles validaciones  por cada una metemos el array de subtipos y tipos  de reclamacion que lanzan esta reclamacion 
    var arrayNIF = ["001", "002", "003", "004", "005", "006", "007", "008", "009", "010", "011", "012", "013", "014",, "033", "036", "038", "042", "051", "052", "057", "068"];
    var arrayNombreCliente = ["001", "002", "003", "004", "005", "006", "008", "009", "010", "011", "012", "014", "018", "019", "033", "036", "038", "040", "051", "052", "054", "057", , "068"];
    var arrayTelefonoContacto = ["001", "003", "004", "005", "014", "018", "019", "025", "033", "041", "042", "043", "044", "051", "052", "053", "054"];

    var arrayCUPS = ["001", "002", "003", "004", "005", "006", "007", "008", "009", "010",
                "011", "012", "013", "014", "015", "16", "17", "20",
                "021", "022", "023", "027", "028", "029",
                "032", "034", "035", "036", "037", "038", "039", "040",
                "042", "043", "044", "045", "046", "047", "048", "049", "055", "056"];
    var arrayNumexpedienteAcometida = ["018", "019", "030", "031"];
    var arrayNumexpedienteFraude = ["061"];
    var arrayFechaIncidente = ["001", "004"];
    var arrayNumeroFacturaATR = ["007", "008", "009", "010", "011", "012", "036", "040", "047", "055", "056"];
    var arrayFechaLectura = ["036"];
    var arrayLectura = ["036"];
    var arrayCodigoIncidencia = ["003"];
    var arrayCodigoSolicitud = ["013", "015", "016", "017", "032", "034", "035", "048"];
    var arrayConceptoCotratacion = ["034"];
    var arrayParametroContratacion = ["034"];
    var arrayConceptoDisconformidad = ["013"];
    var arrayTipoAtencionIncorrecto = ["001"];
    var arrayCtaBanco = ["014"];
    var arrayPersonaContacto = ["001", "014", "024", "025", "041", "042", "043", "044","053","067"];
    var arrayCodReclamacionAnterior = ["023", "029"];
    var arrayFechaDesde = ["020", "021", "022", "024", "025", "039", "046", "049"];
    var arrayFechaHasta = ["020", "021", "022", "024", "025", "039", "046", "049"];
    var arrayImporteReclamado = ["004", "021","033"];
    var arrayUbicacionIncidencia = ["024", "025", "026", "041"];

    var arrayComentarios = ["001", "002", "003", "004", "005", "006", "007", "008", "009", "010",
                        "011", "012", "013", "018", "019", "020",
                        "021", "022", "023", "024", "025", "026", "027", "028", "029", "030",
                        "031", "032", "036", "037", "038", "039", "040", "041",
                        "042", "043", "044", "045", "046", "047", "048", "049", "050",
                         "051", "052", "053", "054", "055", "056", "057",
                         "067", "068"];
    var arrayCodigoInicidencia = ["003"];
    var arrayTipoConceptoFactura = ["008"];

    switch (campo) {
        case "NIF":
            if (arrayNIF.indexOf(subtipo) > -1)
                return true;
            break;
        case "NOMBRE":
            if (arrayNombreCliente.indexOf(subtipo) > -1)
                return true;
            break;
        case "TELEFONO":
            if (arrayTelefonoContacto.indexOf(subtipo) > -1)
                return true;
            break;
        case "CUPS":
            if (arrayCUPS.indexOf(subtipo) > -1)
                return true;
            break;
        case "FECHAINCIDENTE":
            if (arrayFechaIncidente.indexOf(subtipo) > -1)
                return true;
            break;
        case "COMENTARIOS":
            if (arrayComentarios.indexOf(subtipo) > -1)
                return true;
            break;
        case "CODIGOINCIDENCIA":
            if (arrayCodigoInicidencia.indexOf(subtipo) > -1)
                return true;
            break;
        case "CONTACTO":
            if (arrayPersonaContacto.indexOf(subtipo) > -1)
                return true;
            break;
        case "NUMEROFACTURAATR":
            if (arrayNumeroFacturaATR.indexOf(subtipo) > -1)
                return true;
            break;
        case "TIPOCONCEPTOFACTURA":
            if (arrayTipoConceptoFactura.indexOf(subtipo) > -1)
                return true;
            break;
        case "LECTURA":
            if (arrayLectura.indexOf(subtipo) > -1)
                return true;
            break;
        case "FECHALECTURA":
            if (arrayFechaLectura.indexOf(subtipo) > -1)
                return true;
            break;
        case "FECHADESDE":
            if (arrayFechaDesde.indexOf(subtipo) > -1)
                return true;
            break;
        case "FECHAHASTA":
            if (arrayFechaHasta.indexOf(subtipo) > -1)
                return true;
            break;
        case "UBICACION":
            if (arrayUbicacionIncidencia.indexOf(subtipo) > -1)
                return true;
            break;
        case "CODIGOSOLICITUD":
            if (arrayCodigoSolicitud.indexOf(subtipo) > -1)
                return true;
            break;
        case "CTABANCO":
            if (arrayCtaBanco.indexOf(subtipo) > -1)
                return true;
            break;
        case "CODIGOANTERIOR":
            if (arrayCodReclamacionAnterior.indexOf(subtipo) > -1)
                return true;
            break;
        case "IMPORTERECLAMADO":
            if (arrayImporteReclamado.indexOf(subtipo) > -1)
                return true;
            break;
        case "TIPOANTENCIONINCORRECTO":
            if (arrayTipoAtencionIncorrecto.indexOf(subtipo) > -1)
                return true;
            break;
        case "CODIGOINCIDENCIA":
            if (arrayCodigoIncidencia.indexOf(subtipo) > -1)
                return true;
            break;
        case "PARAMETROCONTRATACION":
            if (arrayParametroContratacion.indexOf(subtipo) > -1)
                return true;
            break;
        case "CONCEPTODISCONFORMIDAD":
            if (arrayConceptoDisconformidad.indexOf(subtipo) > -1)
                return true;
            break;
        case "NUMEXPEDIENTEACOMETIDA":
            if (arrayConceptoDisconformidad.indexOf(subtipo) > -1)
                return true;
            break;
        case "NUMEXPEDIENTEFRAUDE":
            if (arrayConceptoDisconformidad.indexOf(subtipo) > -1)
                return true;
            break;
        default:
            return false;
            break;

    }

    return false;
}

// <summary>
// Se encarga de validar si el tipo y el subtipo son correctos
// </summary>
function validarSubtipo(tipo, subTipo) {
    var arrayTipo01 = ["001", "002", "038"];
    var arrayTipo02 = ["003", "004", "005", "006", "007", "008", "009", "010", "011", "012", "036", "037", "040", "046", "047", "049", "055", "056"];
    var arrayTipo03 = ["013", "014", "015", "016", "017", "034", "035", "045", "048"];
    var arrayTipo04 = ["018", "19"];
    var arrayTipo05 = ["020", "021", "022", "023", "039"];
    var arrayTipo06 = ["024", "025", "026", "042", "043", "044"];
    var arrayTipo07 = ["027", "028", "029", "030", "031", "032"];

    switch (tipo) {
        case "01":
            if (arrayTipo01.indexOf(subTipo) > -1) {
                return true;
            }
            break;
        case "02":
            if (arrayTipo02.indexOf(subTipo) > -1) {
                return true;
            }
            break;
        case "03":
            if (arrayTipo03.indexOf(subTipo) > -1) {
                return true;
            }
            break;
        case "04":
            if (arrayTipo04.indexOf(subTipo) > -1) {
                return true;
            }
            break;
        case "05":
            if (arrayTipo05.indexOf(subTipo) > -1) {
                return true;
            }
            break;
        case "06":
            if (arrayTipo06.indexOf(subTipo) > -1) {
                return true;
            }
            break;
        case "07":
            if (arrayTipo07.indexOf(subTipo) > -1) {
                return true;
            }
            break;

        default:
            return false;
            break;
    }

}


// ********************************************************************************************************************************
// ********************************************************************************************************************************
// **** FUNCIONES GENERALES                                                                                              **********
// ********************************************************************************************************************************
// ********************************************************************************************************************************


// <summary>
// Se encarga de validar los datos propios de un documento y el numero de ellos (50 Max)
// </summary>
function validarDocumentos(reclamacionId) {

    var validado = true;

    //obtenemos todos las variables que tiene la reclamacion (min =1 - max=10)
    var fetchXml =
/* 223231 +1*/ //"<fetch mapping='logical'>"+
        "<fetch mapping='logical' no-lock='true'>" +
		  "<entity name='atos_registrodocumentosreclamacion' > " +
			"<attribute name='atos_direccionurl' alias='atos_direccionurl' /> " +
			"<attribute name='atos_tipodocumentoaportadoid' alias='atos_tipodocumentoaportadoid' /> " +
			"<link-entity name='atos_reclamacion' from='atos_reclamacionid' to='atos_reclamacionasociadaid' link-type='outer' alias='atos_reclamacion' > " +
			  "<attribute name='atos_name' alias='reclamacion' /> " +
			"</link-entity> " +
  			"<filter>" +
			   "<condition attribute='atos_reclamacionasociadaid' operator='eq' value='" + reclamacionId + "' />" +
			"</filter>" +
		  "</entity> " +
		"</fetch>";

    var registros = XrmServiceToolkit.Soap.Fetch(fetchXml);

    if (registros.length > 50) {
        mensajeError("No se pueden introducir más de 50 documentos en : \"Envio información adicional\" ", "ERROR", "2", "");
        validado = false;
        return validado;
    }

    for (i = 0; i < registros.length; i++) {

        if (registros[i].attributes["atos_direccionurl"] == null) {
            mensajeError("Es obligatorio introducir la dirección url y el tipo de documento en todos los documentos enviados en : \"Envio información adicional\" ", "ERROR", "2", "");
            validado = false;
            return validado;
        }

        if (registros[i].attributes["atos_tipodocumentoaportadoid"] == null) {
            mensajeError("Es obligatorio introducir la dirección url y el tipo de documento en todos los documentos enviados en : \"Envio información adicional\" ", "ERROR", "2", "");
            validado = false;
            return validado;
        }
    }

    return validado;
}


// <summary>
// Se encarga de anular el guardado de una reclamacion
// </summary>
function cancelarGuardado(obj) {
    if (obj.getEventArgs() != null)
        obj.getEventArgs().preventDefault();
}

/**
// <summary>
// Si se abre la nueva solicitud desde la distribuidora rellena el campo distribuidora de la solicitud
// </summary>
// <remarks>
// Si el formulario llamante tiene un campo subdistribuidora con valor rellena el campo distribuidora de la solicitud con la subdistribuidora<br/>
// Si el formulario llamante no tiene subdistribuidora con valor pero sí distribuidora rellena el campo distribuidora de la solicitud con la distribuidora
// </remarks>
*/
function recuperaDistribuidora() {
    if (Xrm.Page.data.entity.getId() == "" || Xrm.Page.data.entity.getId() == null) {
        if (window.top.opener != null && window.top.opener.Xrm.Page.data != null) {
            var distribuidora = window.top.opener.Xrm.Page.data.entity.attributes.get("atos_subdistribuidoraid");
            if (distribuidora == null)
                distribuidora = window.top.opener.Xrm.Page.data.entity.attributes.get("atos_distribuidoraid");
            else if (distribuidora.getValue() == null)
                distribuidora = window.top.opener.Xrm.Page.data.entity.attributes.get("atos_distribuidoraid");
            if (distribuidora != null) {
                if (distribuidora.getValue() != null) {
                    var valor = new Array();
                    valor[0] = new Object();
                    valor[0].id = distribuidora.getValue()[0].id;
                    valor[0].name = distribuidora.getValue()[0].name;
                    valor[0].entityType = "atos_distribuidora";

                    Xrm.Page.data.entity.attributes.get("atos_distribuidoraid").setValue(valor);
                    Xrm.Page.getAttribute("atos_distribuidoraid").setSubmitMode("always");

                }
            }
        }
    }
}


/**
// <summary>
// Oculta/muestra las pestañas correspondientes según el paso de la solicitud
// </summary>
// <remarks>
// </remarks>
*/
function tab_pasos() {

    Xrm.Page.ui.tabs.get('tab_6').setVisible(false); //envio reclamación/peticion servidor
    Xrm.Page.ui.tabs.get('tab_7').setVisible(false); // recepción aceptacion/acuse recibo
    Xrm.Page.ui.tabs.get('tab_8').setVisible(false); // recepción rechazo y acuse del mismo 
    Xrm.Page.ui.tabs.get('tab_4').setVisible(false); // recepción información adicional
    Xrm.Page.ui.tabs.get('tab_5').setVisible(false); // envio informacion adicional
    Xrm.Page.ui.tabs.get('tab_9').setVisible(false); // recepcion resultado cierre


    if (Xrm.Page.data.entity.attributes.get("atos_pasoid").getValue() != null) {
        var cols = ["atos_codigopaso"];
        var pasoatr = XrmServiceToolkit.Soap.Retrieve("atos_pasoatr", Xrm.Page.data.entity.attributes.get("atos_pasoid").getValue()[0].id, cols);
        if (pasoatr.attributes["atos_codigopaso"] != null) {
            var pasoatrvalue = pasoatr.attributes["atos_codigopaso"].value;
            if (pasoatrvalue == "01") {
                Xrm.Page.ui.tabs.get('tab_6').setVisible(true);
            }
            else if (pasoatrvalue == "02") {
                if (Xrm.Page.data.entity.attributes.get("atos_fecharechazo").getValue() != null) {
                    Xrm.Page.ui.tabs.get('tab_8').setVisible(true);
                }
                else {
                    Xrm.Page.ui.tabs.get('tab_7').setVisible(true);
                }
            }
            else if (pasoatrvalue == "03") {
                Xrm.Page.ui.tabs.get('tab_4').setVisible(true);
            }
            else if (pasoatrvalue == "04") {
                Xrm.Page.ui.tabs.get('tab_5').setVisible(true);
            }
            else if (pasoatrvalue == "05") {
                Xrm.Page.ui.tabs.get('tab_9').setVisible(true);
            }


        }
    }
}
