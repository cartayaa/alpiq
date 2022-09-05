/*
// <summary>
// Borra los mensajes mostrados.
// </summary>
// <param name="nivelAviso">Un identificador del aviso (suele ser un nÃºmero).</param>
// <param name="campoAviso">El campo bajo el que se ha mostrado el aviso (si viene vacÃ­o es un aviso a nivel del formulario).</param>
// <remarks>
// - Si campoAviso es vacÃ­o borra todas las notificaciones mostradas al comienzo del formulario para el identificador indicado por nivelAviso
// - Si campoAviso no es vacÃ­o borra todas las notificaciones mostradas bajo el campo campoAviso para el identificador indicado por nivelAviso
// </remarks>
 */
function limpiaMensajeError(formContext, nivelAviso, campoAviso)
{
	//var formContext = formContext.getFormContext();

	//if ( campoAviso != "" )
	//	formContext.getControl(campoAviso).clearNotification(nivelAviso);
	//else 
		formContext.ui.clearFormNotification(nivelAviso);
	
}


/*
// <summary>
// Muestra un mensaje informativo, de error o un warning.
// </summary>
// <param name="mensaje">El mensaje que se debe mostrar.</param>
// <param name="tipoAviso">Tipo de aviso (error, warning, informativo o "CAMPO").</param>
// <param name="nivelAviso">Un identificador del aviso (suele ser un nÃºmero).</param>
// <param name="campoAviso">El campo bajo el que se ha mostrado el aviso (si viene vacÃ­o es un aviso a nivel del formulario).</param>
// <remarks>
// Si tipo de aviso es "CAMPO" muestra el mensaje bajo el campo campoAviso
// Si tipo de aviso no es "CAMPO" muestra el mensaje (error, warning o informativo) al inicio del formulario
// Si campoAviso no viene vacÃ­o coloca el foco en el campo campoAviso
// </remarks>
 */
function mensajeError(formContext, message, level, uniqueId, campoAviso)
{
	/*
	The level of the message, which defines how the message will be displayed. Specify one of the following values:
	ERROR : Notification will use the system error icon.
	WARNING : Notification will use the system warning icon.
	INFO : Notification will use the system info icon.
	*/

	//var formContext = formContext.getFormContext();
    
	//if ( tipoAviso == "CAMPO" )
	//	formContext.getControl(campoAviso).setNotification(message, uniqueId)
	//else if ( tipoAviso != "" )
		formContext.ui.setFormNotification(message, level, uniqueId);

		//formContext.ui.setFormNotification(message, level, uniqueId);
		//formContext.ui.setFormNotification("Example of an INFORMATION notification.", "INFO");  

	if ( campoAviso != "" )
		formContext.getControl(campoAviso).setFocus(true);
}