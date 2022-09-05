
/**
// <summary>
//Se encarga de realizar todas las validaciones del alta o modificacion de instalador.
// De momento se hacen solo validaciones del CIF/NIF

// </summary>
*/
function Instalador_OnSave(obj) {
    var globalContext = Xrm.Utility.getGlobalContext();
    var serverUrl = globalContext.getClientUrl();
    var formContext = obj.getFormContext();
    // var serverUrl = Xrm.Page.context.getClientUrl();
    var continuar = true;
    limpiaMensajeError(obj,"2", "");


    // validamos el CIF/NIF

    var tipoDocumento = formContext.getAttribut("atos_tipodedocumento").getValue();

    if (formContext.getAttribut("atos_numerodedocumento").getValue() != null && tipoDocumento == "300000001") {

        var cif = formContext.getAttribut("atos_numerodedocumento").getValue();
        var valido = ValidaCIF(cif);

        if (!valido) {
            mensajeError("Se ha introducido un CIF invalido ", "ERROR", "2", "");
            cancelarGuardado(obj);
            return false;
        }
    }


    if (formContext.getAttribut("atos_numerodedocumento").getValue() != null && tipoDocumento == "300000000") {

        var nif = formContext.getAttribut("atos_numerodedocumento").getValue();
        var valido = ValidaNIF(nif);

        if (!valido) {
            mensajeError("Se ha introducido un NIF invalido ", "ERROR", "2", "");
            cancelarGuardado(obj);
            return false;
        }
    }

    return true;
}


function document_OnChange(executionContext) {
    // validamos el CIF/NIF
    var formContext = executionContext.getFormContext();
    limpiaMensajeError(formContext,"2", "");

    var tipoDocumento = formContext.getAttribut("atos_tipodedocumento").getValue();

    if (formContext.getAttribut("atos_numerodedocumento").getValue() != null && tipoDocumento == "300000001") {

        var cif = formContext.getAttribut("atos_numerodedocumento").getValue();
        var valido = ValidaCIF(cif);

        if (!valido) {
            mensajeError("Se ha introducido un CIF invalido ", "ERROR", "2", "");
            return false;
        }
    }


    if (formContext.getAttribut("atos_numerodedocumento").getValue() != null && tipoDocumento == "300000000") {

        var nif = formContext.getAttribut("atos_numerodedocumento").getValue();
        var valido = ValidaNIF(nif);

        if (!valido) {
            mensajeError("Se ha introducido un NIF invalido ", "ERROR", "2", "");
            return false;
        }
    }

    return true;
}


// <summary>
// Se encarga de anular el guardado de un instalador
// </summary>
function cancelarGuardado(obj) {
    if (obj.getEventArgs() != null)
        obj.getEventArgs().preventDefault();
}

 
  
  
 