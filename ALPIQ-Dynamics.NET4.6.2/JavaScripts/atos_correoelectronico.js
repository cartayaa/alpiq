function cargaInicial() {
    // si es un formulario de creación
	//debugger;
    if (Xrm.Page.data.entity.getId() == null || Xrm.Page.data.entity.getId() == "") {
       
       if (Xrm.Page.data.entity.attributes.get("regardingobjectid").getValue()!= null &&  Xrm.Page.data.entity.attributes.get("regardingobjectid").getValue()[0].typename == 'incident') {
            // buscamos si es de tipo caso 
               var usuario = obtenerUsuarioComercializadora();
			    var nombre = obtenerValorEntidad(usuario, "systemuser", "systemuserid", "fullname");
			   	var valorReferencia = new Array();
				valorReferencia[0] = new Object();
				valorReferencia[0].id = usuario; 
				valorReferencia[0].name = nombre;
				valorReferencia[0].entityType = "systemuser";
			   
			   Xrm.Page.data.entity.attributes.get("from").setValue(valorReferencia);
			   
               //var correo = obtenerValorEntidad(usuario, "systemuser", "systemuserid", "internalemailaddress");
              
           }

        }
    }



// ********************************************************************************************************************************
// ********************************************************************************************************************************
// **** FUNCIONES GENERALES                                                                                              **********
// ********************************************************************************************************************************
// ********************************************************************************************************************************
function obtenerValorEntidad(entidadId, nombreEntidad, nombreCampoIdentificador, nombreCampoDescripcion) {

    var idencontrado = null;
    var valor = null;
    var fetchXml =
		"<fetch mapping='logical'>" +
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


function obtenerUsuarioComercializadora() {

    var idencontrado = null;
    var valor = null;
    var fetchXml =
		"<fetch mapping='logical'>" +
		  "<entity name='atos_parametroscomercializadora' > " +
			"<attribute name='new_usuarioasignado' alias='new_usuarioasignado' /> " +
		  "</entity> " +
		"</fetch>";

    var registros = XrmServiceToolkit.Soap.Fetch(fetchXml);

    if (registros.length > 0) {
        valor = registros[0].attributes['new_usuarioasignado'].id;
    }
    return valor;
}



/**
// <summary>
// Abre el formulario de nuevo Caso
// </summary>
// <remarks>
// Si el correo estÃ¡ creado (es decir no estamos en nuevo correo) abre el formulario de nuevo caso indicandole que viene desde el correo.
// </remarks>
*/
function crearCaso() {
	 
    if (Xrm.Page.data.entity.getId() != null && Xrm.Page.data.entity.getId() != "") {
		if (comprobarContacto())
		{
			var parameters = {};
			parameters["llamado_desde"] = "Correo";
			parameters["llamante_id"] = Xrm.Page.data.entity.getId();
			if ( Xrm.Page.data.entity.attributes.get("subject").getValue() != null ){
				parameters["titulo_correo"] = Xrm.Page.data.entity.attributes.get("subject").getValue();
			}
			else {
				parameters["titulo_correo"] =" ";	
			}
			
			if ( Xrm.Page.data.entity.attributes.get("description").getValue() != null ){
				
				var regex = /(<([^>]+)>)/ig
				,   body = Xrm.Page.data.entity.attributes.get("description").getValue()
				,   result = body.replace(regex, "");
				
				result = result.replace(/&#160;/g, " ");
				parameters["descripcion_correo"] = "Ver correo adjunto dentro del apartado de Notas.";
			}
			else  {
				parameters["descripcion_correo"] = "Ver correo adjunto dentro del apartado de Notas.";
			}
			
			if (Xrm.Page.data.entity.attributes.get("regardingobjectid").getValue() != null) {
				
				parameters["referentea_correo"] = Xrm.Page.data.entity.attributes.get("regardingobjectid").getValue()[0].id;
				parameters["referenteaname_correo"] = Xrm.Page.data.entity.attributes.get("regardingobjectid").getValue()[0].name;
				parameters["referenteaentitytype_correo"] = Xrm.Page.data.entity.attributes.get("regardingobjectid").getValue() [0].entityType;
			}
			else {
				parameters["referentea_correo"] = " ";
				parameters["referenteaname_correo"] =" ";
				parameters["referenteaentitytype_correo"] ="";
			}
			
			Xrm.Utility.openEntityForm("incident", null, parameters);
		}
    }
}


/**
// <summary>
// Abre el formulario de nueva Tarea
// </summary>
// <remarks>
// Si el correo estÃ¡ creado (es decir no estamos en nuevo correo) abre el formulario de nueva Tarea indicandole que viene desde el correo.
// </remarks>
*/
function crearTarea() {
	
    if (Xrm.Page.data.entity.getId() != null && Xrm.Page.data.entity.getId() != "") {
		if (comprobarContacto())
		{
			var parameters = {};
			parameters["llamado_desde"] = "Correo";
			parameters["llamante_id"] = Xrm.Page.data.entity.getId();
			if ( Xrm.Page.data.entity.attributes.get("subject").getValue() != null ){
				parameters["titulo_correo"] = Xrm.Page.data.entity.attributes.get("subject").getValue();
			}
			else {
				parameters["titulo_correo"] =" ";	
			}
			
			if ( Xrm.Page.data.entity.attributes.get("description").getValue() != null ){
				
					var regex = /(<([^>]+)>)/ig
				,   body = Xrm.Page.data.entity.attributes.get("description").getValue()
				,   result = body.replace(regex, "");
				
				result = result.replace(/&#160;/g, " ");
				
				parameters["descripcion_correo"] ="Ver correo origen";
			}
			else  {
				parameters["descripcion_correo"] = "Ver correo origen";
			}
			
			
			if (Xrm.Page.data.entity.attributes.get("regardingobjectid").getValue() != null) {
				
				parameters["referentea_correo"] = Xrm.Page.data.entity.attributes.get("regardingobjectid").getValue()[0].id;
				parameters["referenteaname_correo"] = Xrm.Page.data.entity.attributes.get("regardingobjectid").getValue()[0].name;
				parameters["referenteaentitytype_correo"] = Xrm.Page.data.entity.attributes.get("regardingobjectid").getValue() [0].entityType;
			}
			else {
				parameters["referentea_correo"] = " ";
				parameters["referenteaname_correo"] =" ";
				parameters["referenteaentitytype_correo"] ="";
			}
			
			Xrm.Utility.openEntityForm("task", null, parameters);
		}
    }
}


function comprobarContacto()
{
	 limpiaMensajeError("2", "");
		if (  Xrm.Page.data.entity.attributes.get("from").getValue()==null)
		{
		    mensajeError("No se puede convetir si no se intrduce un contacto valido en CRM del emisor del correo (De:)", "ERROR", "2", "");
			return false;
		}
		return true;
}
