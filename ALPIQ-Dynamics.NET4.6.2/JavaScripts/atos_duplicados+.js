/*
 File="atos_cierre.js" 
 Copyright (c) Atos. All rights reserved.

 Fecha 		Codigo  Version Descripcion
 16/03/2021 223231  1.0.1.1 Cambi el FechXml con sentencia no-lock='true'

*/

/**
// <summary>
// Comprueba si ya existe un registro duplicado.
// </summary>
// <param name="entidad">El nombre de la entidad donde va a comprobar si hay duplicados.</param>
// <param name="campo">El campo con el que debe comprobar si hay duplicados.</param>
// <param name="valor">El valor que debe validar en el campo anterior.</param>
// <param name="campoid">El nombre del campo id para el caso que tenga que excluir el registro actual.</param>
// <param name="camponame">El nombre del campo que vamos a mostrar (atos_name, fullname, ...) para informar que hay duplicados.</param>
// <remarks>
// Devuelve null si no hay duplicados o el valor del campo anterior si hay duplicados
// </remarks>
 */
async function controlDuplicados(formContext,entidad, campo, valor, campoid, camponame)
{
	return controlDuplicadosCA(formContext,entidad, campo, valor, campoid, camponame, "");
	/*var idencontrado = null;
	if ( valor != null )
	{
		var counter=1;
		var fetchXml = 
		"<fetch mapping='logical'>" +
		   "<entity name='" + entidad + "'>" +
			  "<attribute name='" + camponame + "' />" +
			  "<filter>" +
				 "<condition attribute='" + campo + "' operator='eq' value='" + valor + "' />";
		if ( formContext.data.entity.getId() != null && formContext.data.entity.getId() != "" )
			fetchXml = fetchXml + "<condition attribute='" + campoid + "' operator='ne' value='" + formContext.data.entity.getId() + "' />" ;
				 fetchXml = fetchXml +
			  "</filter>" +
		   "</entity>" +
		"</fetch>";
		
		var registros = XrmServiceToolkit.Soap.Fetch(fetchXml);
		if ( registros.length > 0 )
		{
			idencontrado = registros[0].attributes[camponame].value;
		}

	}
	return idencontrado;*/
}

/**
// <summary>
// Comprueba si ya existe un registro duplicado.
// </summary>
// <param name="entidad">El nombre de la entidad donde va a comprobar si hay duplicados.</param>
// <param name="campo">El campo con el que debe comprobar si hay duplicados.</param>
// <param name="valor">El valor que debe validar en el campo anterior.</param>
// <param name="campoid">El nombre del campo id para el caso que tenga que excluir el registro actual.</param>
// <param name="camponame">El nombre del campo que vamos a mostrar (atos_name, fullname, ...) para informar que hay duplicados.</param>
// <param name="condicionadicional">CondiciÃ³n adicional para incluir en el fetch.</param>
// <remarks>
// Devuelve null si no hay duplicados o el valor del campo anterior si hay duplicados
// </remarks>
 */
async function controlDuplicadosCA(formContext,entidad, campo, valor, campoid, camponame, condicionadicional)
{
	var idencontrado = null;
	if ( valor != null )
	{
		var counter=1;
		var fetchXml = 
/* 223231 +1*/ //"<fetch mapping='logical'>"+
		   "<fetch mapping='logical' no-lock='true'>" +
		   "<entity name='" + entidad + "'>" +
			  "<attribute name='" + camponame + "' />" +
			  "<filter>" +
				 "<condition attribute='" + campo + "' operator='eq' value='" + valor + "' />";
		if ( formContext.data.entity.getId() != null && formContext.data.entity.getId() != "" )
			fetchXml = fetchXml + "<condition attribute='" + campoid + "' operator='ne' value='" + formContext.data.entity.getId() + "' />" ;
		if ( condicionadicional != "" )
			fetchXml = fetchXml + condicionadicional ;
				 fetchXml = fetchXml +
			  "</filter>" +
		   "</entity>" +
		"</fetch>";
		
		var registros = null;

		await Xrm.WebApi.retrieveMultipleRecords(entidad, "?fetchXml=" + fetchXml).then(
            function success(records) {
                if (records != null && records.entities != null && records.entities.length > 0) {
                  registros = records;
            }
            }, function (error) {
                Xrm.Navigation.openErrorDialog({ detail: error.message });
            });

		if ( registros.length > 0 )
		{
			idencontrado = registros[0].attributes[camponame].value;
		}

	}
	return idencontrado;
}