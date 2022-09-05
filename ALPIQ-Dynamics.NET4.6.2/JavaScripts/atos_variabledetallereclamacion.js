/**
// <summary>
//Se encarga de realizar todas las validaciones del la variable de detalle reclamacion 
// </summary>
*/
function VariableDetalleReclamacionatr_OnSave(formContext) {
    var serverUrl = executionContext.context.getClientUrl();
    var continuar = true;
    var executionContext = formContext.getFormContext();
    limpiaMensajeError("2", "");
    var proceso = "";
    var paso = "";
    if (executionContext.data.entity.attributes.get("atos_nombrecontacto").getValue() == null && executionContext.data.entity.attributes.get("atos_razonsocial").getValue() == null ) {
		mensajeError("Es obligatorio introducir nombre y  primer apellido o razón social para la sección contacto", "ERROR", "2", "atos_nombrecontacto");
		cancelarGuardado(formContext);
		return true; 
    }

    if (executionContext.data.entity.attributes.get("atos_nombrecontacto").getValue() != null && executionContext.data.entity.attributes.get("atos_razonsocial").getValue() != null ) {
		mensajeError("Solamente hay que introducir un  nombre y primer apellido o razón social  para la sección de contacto", "ERROR", "2", "atos_nombrecontacto");
		cancelarGuardado(formContext);
		return true; 
    }

  if(executionContext.data.entity.attributes.get("atos_nombrecontacto").getValue() != null && executionContext.data.entity.attributes.get("atos_primerapellidocontacto").getValue() == null ) {
		mensajeError("Es obligatorio introducir nombre y  primer apellido  para  la sección de contacto si se añade una persona física", "ERROR", "2", "atos_nombrecontacto");
		cancelarGuardado(formContext);
		return true; 
    }

	return true; 
}



// <summary>
// Se encarga de anular el guardado de una reclamacion
// </summary>
function cancelarGuardado(formContext) {
    if (formContext.getEventArgs() != null)
    formContext.getEventArgs().preventDefault();
}