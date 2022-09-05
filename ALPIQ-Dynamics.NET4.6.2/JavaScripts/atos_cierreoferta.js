/**
// <summary>
// Se ejecuta al abrir el formulario de la Tarea.
// </summary>
// <remarks>
// Si se entra en modo nueva tarea, comprueba si se ha llamado desde correo<br/>
// Si ha sido llamado desde  ella llama a la función cargaDesdeCorreo,
//  para cargar datos de la tarea a partir de la entidad llamante.
// </remarks>
*/
function OfertaOnLoad(executionContext) {

    var formContext = executionContext.getFormContext();
    if (formContext.data.entity.getId() == null || formContext.data.entity.getId() == "") {
        var xrmObject = formContext.context.getQueryStringParameters();

        if (xrmObject != null) {
            if (xrmObject["llamado_desde"] != null) {
                var llamado_desde = xrmObject["llamado_desde"].toString();
                //if ( llamado_desde == "Instalacion" )
                if (llamado_desde == "Correo") {
                    if (xrmObject["llamante_id"] != null) {
                        var llamanteid = xrmObject["llamante_id"].toString();
						var titulo;
						 if (xrmObject["titulo_correo"] != null)
							titulo = xrmObject["titulo_correo"].toString();
						
						var descripcion;
						 if (xrmObject["descripcion_correo"] != null)
							descripcion = xrmObject["descripcion_correo"].toString();

						
						var referentea="";
						var referenteaName="";
						var referenteaEntityType="";
						 if (xrmObject["referentea_correo"] != null)
						 {
							referentea = xrmObject["referentea_correo"].toString();
							referenteaName = xrmObject["referenteaname_correo"].toString();
							referenteaEntityType = xrmObject["referenteaentitytype_correo"].toString();
						 }
                        var serverUrl = formContext.context.getClientUrl();

                        head.load(serverUrl + "/WebResources/atos_json2.js", serverUrl + "/WebResources/atos_jquery.js", serverUrl + "/WebResources/atos_XrmServiceToolkit.js", function () {
                         cargaDesdeCorreo(formContext,llamanteid,titulo,descripcion,referentea,referenteaName,referenteaEntityType);
                        });

                    }
                }
            }
        }
    }
}


/**
// <summary>
// Carga datos desde el correo relacionado
// </summary>
// <param name="instalacionid">Identificador del  correo.</param>
// <remarks>
// Accede correocon el identificador recibido por parámetro<br/>
// y rellena los campos de la tarea a partir de los siguientes campos del correo:
// </remarks>
 */
function cargaDesdeCorreo(formContext,correoId,titulo,descripcion,referentea,referenteaName,referenteaEntityType) {
  
 var valorReferencia = new Array();
        valorReferencia[0] = new Object();
        valorReferencia[0].id = correoId;
        //valorReferencia[0].name = categoria.attributes["atos_intervenciondistribuidorid"].name;
        valorReferencia[0].entityType = "email";

        formContext.data.entity.attributes.get("atos_correoorigenid").setValue(valorReferencia);
		formContext.data.entity.attributes.get("subject").setValue(titulo);
		formContext.data.entity.attributes.get("description").setValue(descripcion);
		
		// ponemos le referentea
		if (referentea.trim()!="")
		{
				formContext.data.entity.attributes.get("regardingobjectid").setValue(construyeLookup(referenteaEntityType, referentea, referenteaName));
				formContext.getAttribute("regardingobjectid").setSubmitMode("always");
		}
		
  
}


