/**
// <summary>
// Recupera un campo de otra entidad para actualizar un campo lookup del formulario.
// </summary>
// <param name="entidad">Entidad en la que busca.</param>
// <param name="camponombre">Campo cuyo valor mostrarÃ¡ en el lookup.</param>
// <param name="campoform">Nombre del campo del lookup en el formulario.</param>
// <param name="campoid">Id del registro que tiene que buscar en "entidad".</param>
// <remarks>
// - Mediante XrmServiceToolkit.Soap.Retrieve accede a entidad con el id de campoid, para recuperar el campo camponombre.
// - Construye un array de una ocurrencia que inicializa con un objeto construido con el id de campoid y el valor recuperado de camponombre.
// - Modifica el campo del formulario campoform con el array anterior.
// </remarks>
 */
 

async function recuperanombre(formContext,entidad, camponombre, campoform, campoid)
{

	var recNombre = null;

	Xrm.WebApi.retrieveRecord(entidad, campoid,"?$select=" + camponombre).then(
		function success(record) {
			recNombre = record;
			var valor = new Array();
	valor[0] = new Object();
	valor[0].id = campoid;
	valor[0].name = recNombre.atos_name;
	valor[0].entityType = entidad;
	
	formContext.data.entity.attributes.get(campoform).setValue(valor);
	formContext.getAttribute(campoform).setSubmitMode("always");
		}, function (error) {
			Xrm.Navigation.openErrorDialog({ detail: error.message });
		});

	
}

/**
// <summary>
// Obtiene el pais de la CCAA seleccionada para rellenar el campo automÃ¡ticamente.
// </summary>
// <param name="campoccaa">Nombre del campo CCAA en el formulario.</param>
// <param name="campopais">Nombre del campo pais en el formulario.</param>
// <remarks>
// Utiliza recuperanombre para rellenar el campo correspondiente del formulario
// </remarks>
 */ 

async function recuperaDireccionCCAA(formContext,campoccaa, campopais)
{
	var ccaaid = formContext.data.entity.attributes.get(campoccaa);
	if (ccaaid.getValue() !=null) 
	{
		var ccaavalue= ccaaid.getValue();

		var recuperaDireccion= null;

		Xrm.WebApi.retrieveRecord("atos_comunidadautonoma", ccaavalue[0].id,"").then(
            function success(record) {
                recuperaDireccion = record;
				if (  recuperaDireccion._atos_paisid_value != undefined )
		{
			recuperanombre(formContext,"atos_pais","atos_name", campopais, recuperaDireccion._atos_paisid_value);
		}	
            }, function (error) {
                Xrm.Navigation.openErrorDialog({ detail: error.message });
			});
			

			
	}
}


/**
// <summary>
// Obtiene la CCAA y el pais de la Provincia seleccionada para rellenar los campos automÃ¡ticamente.
// </summary>
// <param name="campoprovincia">Nombre del campo Provincia en el formulario.</param>
// <param name="campoccaa">Nombre del campo CCAA en el formulario.</param>
// <param name="campopais">Nombre del campo pais en el formulario.</param>
// <remarks>
// Utiliza recuperanombre para rellenar los campos correspondientes del formulario
// </remarks>
 */ 
async function recuperaDireccionProvincia(formContext,campoprovincia, campoccaa, campopais)
{
	var provinciaid = formContext.data.entity.attributes.get(campoprovincia);
	if (provinciaid.getValue() !=null) 
	{
		var provinciavalue= provinciaid.getValue();

		var recuperaDireccion = null;

		Xrm.WebApi.retrieveRecord("atos_provincia", provinciavalue[0].id,"").then(
            function success(record) {
                recuperaDireccion = record;
				if (  recuperaDireccion._atos_comunidadautonomaid_value != undefined )
		{
			recuperanombre(formContext,"atos_comunidadautonoma","atos_name", campoccaa, recuperaDireccion._atos_comunidadautonomaid_value);
		}
		if (  recuperaDireccion._atos_paisid_value != undefined )
		{
			recuperanombre(formContext,"atos_pais","atos_name", campopais, recuperaDireccion._atos_paisid_value);
		}
            }, function (error) {
                Xrm.Navigation.openErrorDialog({ detail: error.message });
			});
			

		
		
	}
}

/**
// <summary>
// Obtiene la provincia, CCAA y el pais del municipio seleccionado para rellenar los campos automÃ¡ticamente.
// </summary>
// <param name="campomunicipio">Nombre del campo Municipio en el formulario.</param>
// <param name="campoprovincia">Nombre del campo Provincia en el formulario.</param>
// <param name="campoccaa">Nombre del campo CCAA en el formulario.</param>
// <param name="campopais">Nombre del campo pais en el formulario.</param>
// <remarks>
// Utiliza recuperanombre para rellenar los campos correspondientes del formulario
// </remarks>
 */
async function recuperaDireccionMunicipio(formContext,campomunicipio, campoprovincia, campoccaa, campopais)
{

	var municipioid = formContext.data.entity.attributes.get(campomunicipio);
	if (municipioid.getValue() !=null) 
	{
		var municipiovalue= municipioid.getValue();
		
		var recuperaDireccion= null;

		Xrm.WebApi.retrieveRecord("atos_municipio", municipiovalue[0].id,"").then(
            function success(record) {
                recuperaDireccion = record;
				if (  recuperaDireccion._atos_provinciaid_value != undefined )
		{
			recuperanombre(formContext,"atos_provincia","atos_name", campoprovincia, recuperaDireccion._atos_provinciaid_value);
		}
		if (  recuperaDireccion._atos_comunidadautonomaid_value != undefined )
		{
			recuperanombre(formContext,"atos_comunidadautonoma","atos_name", campoccaa, recuperaDireccion._atos_comunidadautonomaid_value);
		}
		if (  recuperaDireccion._atos_paisid_value != undefined )
		{
			recuperanombre(formContext,"atos_pais","atos_name", campopais, recuperaDireccion._atos_paisid_value);		
		}
            }, function (error) {
                Xrm.Navigation.openErrorDialog({ detail: error.message });
			});
			

		
	}
}


/**
// <summary>
// Obtiene la poblaciÃ³n, municipio, CCAA y el pais de la poblaciÃ³n seleccionada para rellenar los campos automÃ¡ticamente.
// </summary>
// <param name="campopoblacion">Nombre del campo PoblaciÃ³n en el formulario.</param>
// <param name="campomunicipio">Nombre del campo Municipio en el formulario.</param>
// <param name="campoprovincia">Nombre del campo Provincia en el formulario.</param>
// <param name="campoccaa">Nombre del campo CCAA en el formulario.</param>
// <param name="campopais">Nombre del campo pais en el formulario.</param>
// <remarks>
// Utiliza recuperanombre para rellenar los campos correspondientes del formulario
// </remarks>
 */
async function recuperaDireccionPoblacion(formContext,campopoblacion, campomunicipio, campoprovincia, campoccaa, campopais)
{

	var poblacionid = formContext.data.entity.attributes.get(campopoblacion);
	if (poblacionid.getValue() !=null) 
	{
		var poblacionvalue= poblacionid.getValue();

		var recuperaDireccion= null;
		Xrm.WebApi.retrieveRecord("atos_poblacion", poblacionvalue[0].id,"").then(
            function success(record) {
                recuperaDireccion = record;
				if (  recuperaDireccion._atos_municipioid_value != undefined )
		{
			recuperanombre(formContext,"atos_municipio","atos_name", campomunicipio, recuperaDireccion._atos_municipioid_value);
		}
		if (  recuperaDireccion._atos_provinciaid_value != undefined )
		{
			recuperanombre(formContext,"atos_provincia","atos_name", campoprovincia, recuperaDireccion._atos_provinciaid_value);
		}
		if (  recuperaDireccion._atos_comunidadautonomaid_value != undefined )
		{
			recuperanombre(formContext,"atos_comunidadautonoma","atos_name", campoccaa, recuperaDireccion._atos_comunidadautonomaid_value);
		}
		if (  recuperaDireccion._atos_paisid_value != undefined )
		{
			recuperanombre(formContext,"atos_pais","atos_name", campopais, recuperaDireccion._atos_paisid_value);		
		}
            }, function (error) {
                Xrm.Navigation.openErrorDialog({ detail: error.message });
			});
			

		
	}
}


/**
// <summary>
// Obtiene la poblaciÃ³n, municipio, provincia, CCAA y el pais del cÃ³digo postal seleccionado para rellenar los campos automÃ¡ticamente.
// </summary>
// <param name="campocpostal">Nombre del campo CÃ³digo Postal en el formulario.</param>
// <param name="campopoblacion">Nombre del campo PoblaciÃ³n en el formulario.</param>
// <param name="campomunicipio">Nombre del campo Municipio en el formulario.</param>
// <param name="campoprovincia">Nombre del campo Provincia en el formulario.</param>
// <param name="campoccaa">Nombre del campo CCAA en el formulario.</param>
// <param name="campopais">Nombre del campo pais en el formulario.</param>
// <remarks>
// Utiliza recuperanombre para rellenar los campos correspondientes del formulario
// </remarks>
 */
async function recuperaDireccionCPostal(formContext,campocpostal, campopoblacion, campomunicipio, campoprovincia, campoccaa, campopais)
{

	var codigopostalid = formContext.data.entity.attributes.get(campocpostal);
	if (codigopostalid.getValue() !=null) 
	{
		var codigopostal= codigopostalid.getValue();

		var recuperaDireccion= null;

		Xrm.WebApi.retrieveRecord("atos_codigopostal", codigopostal[0].id,"").then(
            function success(record) {
                recuperaDireccion = record;
		if (  recuperaDireccion._atos_poblacionid_value != undefined )
		{
			recuperanombre(formContext,"atos_poblacion","atos_name", campopoblacion, recuperaDireccion._atos_poblacionid_value);
		}
		if (  recuperaDireccion._atos_municipioid_value != undefined )
		{
			recuperanombre(formContext,"atos_municipio","atos_name", campomunicipio, recuperaDireccion._atos_municipioid_value);
		}
		if (  recuperaDireccion._atos_provinciaid_value != undefined )
		{
			recuperanombre(formContext,"atos_provincia","atos_name", campoprovincia, recuperaDireccion._atos_provinciaid_value);
		}
		if (  recuperaDireccion._atos_comunidadautonomaid_value != undefined )
		{
			recuperanombre(formContext,"atos_comunidadautonoma","atos_name", campoccaa, recuperaDireccion._atos_comunidadautonomaid_value);
		}
		if (  recuperaDireccion._atos_paisid_value != undefined )
		{
			recuperanombre(formContext,"atos_pais","atos_name", campopais, recuperaDireccion._atos_paisid_value);		
		}
            }, function (error) {
                Xrm.Navigation.openErrorDialog({ detail: error.message });
			});

		
	}
}
