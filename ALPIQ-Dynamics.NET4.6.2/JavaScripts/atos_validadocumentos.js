/**
// <summary>
// Valida un documento de tipo cif.
// </summary>
// <param name="cif">NÃºmero de documento a validar.</param>
// <remarks>
// Comprueba si cif cumple el formato de los documentos de tipo CIF
// - Si cumple el formato devuelve true
// - Si no cumple el formato devuelve false
// </remarks>
 */
function ValidaCIF(cif)
{
	//Primero validamos el formato y luego comprobamos que el Ãºltimo dÃ­gito es correcto.
	var resul = true;
	// pasar a mayÃºsculas
	var temp = cif.toUpperCase(); 
	if (!/^[A-Za-z0-9]{9}$/.test(temp)) // Son 9 dÃ­gitos?
	{
		resul = false;
	}
	else if (!/^[ABCDEFGHKLMNPQSUJRVW]/.test(temp)) 
	{
		// Es una letra de las admitidas ?
		resul = false;
	}	
	if(resul)
	{
		// Sumamos los A y B
		a = 0;
		b = 0;
		var calculo = new Array(0,2,4,6,8,1,3,5,7,9);
		for(x=2;x<=6;x+=2)
		{
			a = a + parseInt(cif.substr(x,1));  
			b = b + calculo[parseInt(cif.substr(x-1,1))];           
		}           
		b = b + calculo[parseInt(cif.substr(x-1,1))];		
		// C como suma de a+b
		c = a + b;
		// El calculo de D
		d = (10 -(c%10));
		d = (d==10?0:d); //icamacho: 12/07/2011 ,ayayay , pÃ¡haro
		var codigos = new Array('J','A','B','C','D','E','F','G','H','I','J');
		if(cif.substr(8,9) != d && cif.substr(8,9) != codigos[d])
		{
			//Si el dÃ­gito de control no es el nÃºmero o la letra correcta nos ponemos serios y no dejamos guardar.
			resul = false;
		}
	}
	return resul;
}


/**
// <summary>
// Valida un documento de tipo nif.
// </summary>
// <param name="dni">NÃºmero de documento a validar.</param>
// <remarks>
// Comprueba si dni cumple el formato de los documentos de tipo NIF
// - Si cumple el formato devuelve true
// - Si no cumple el formato devuelve false
// </remarks>
 */
function ValidaNIF(dni) 
/*Funcion que valida el DNI. Recibe como parametro un alfanumerico*/
{
  var resul = true;
  dni= dni.toUpperCase();
  numero = parseInt(dni.substr(0,dni.length-1),10);
  if(!isNaN(numero))
  {
    let = dni.substr(dni.length-1,1);
    numero = numero % 23;
    letra='TRWAGMYFPDXBNJZSQVHLCKET';
    letra=letra.substring(numero,numero+1);
    if (letra!=let) resul = false;
  }
  else resul = false;
  return resul;
}

/**
 * Recibe por parÃ¡metro el nie a validar
 * - Devuelve false si es un nie incorrecto
 */
/**
// <summary>
// Valida un documento de tipo nie.
// </summary>
// <param name="a">NÃºmero de documento a validar.</param>
// <remarks>
// Comprueba si a cumple el formato de los documentos de tipo NIE
// - Si cumple el formato devuelve true
// - Si no cumple el formato devuelve false
// </remarks>
 */
function valida_NIE( a )
{
/*FunciÃ³n que valida el NIE. Recibe como parÃ¡metro un alfanumÃ©rico*/
	var temp = a.toUpperCase();
	var cadenadni = "TRWAGMYFPDXBNJZSQVHLCKE";
	if( temp!= '' )
	{
		//si no tiene un formato valido devuelve error
		if( ( !/^[A-Z]{1}[0-9]{7}[A-Z0-9]{1}$/.test( temp ) && !/^[T]{1}[A-Z0-9]{8}$/.test( temp ) ) && !/^[0-9]{8}[A-Z]{1}$/.test( temp ) )
		{
			return 0;
		}
		//comprobacion de NIEs
		//T
		if( /^[T]{1}[A-Z0-9]{8}$/.test( temp ) )
		{
			if( a.charAt( 8 ) == /^[T]{1}[A-Z0-9]{8}$/.test( temp ) )
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		//XYZ
		if( /^[XYZ]{1}/.test( temp ) )
		{
			temp = temp.replace( 'X','0' )
			temp = temp.replace( 'Y','1' )
			temp = temp.replace( 'Z','2' )
			pos = temp.substring(0, 8) % 23;
 
			if( a.toUpperCase().charAt( 8 ) == cadenadni.substring( pos, pos + 1 ) )
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
	else
	{
		return false;
	}
}

/**
// <summary>
// Valida un documento teniendo en cuenta su tipo.
// </summary>
// <param name="campoTipoDocumento">Campo del tipo de documento.</param>
// <param name="campoNumDocumento">Campo del nÃºmero de documento.</param>
// <param name="tipoAviso">Tipo de aviso (error, warning, informativo o "CAMPO").</param>
// <remarks>
// - Si tipoAviso es "CAMPO" borra los mensajes de "nivel" "3" que hay bajo el campo campoNumDocumento
// - Si tipoAviso no es "CAMPO" borra los mensjes de "nivel" "3" que hubiera al inicio del formulario
// - Llama a ValidaCIF, ValidaNIF o valida_NIE segÃºn el valor del campo indicado por campoTipoDocumento
// - Si no es correcto muestra un mensaje mediante mensajeError de atos_mensajes.js 
// - Devuelve el mensaje mostrado (vacÃ­o si todo es correcto).
// </remarks>
 */
function validaDocumento(formContext, campoTipoDocumento, campoNumDocumento, tipoAviso)
{
	//var formContext = formContext.getFormContext();

	//--limpiaMensajeError(tipoAviso, '3', campoNumDocumento);
	//if ( tipoAviso == "CAMPO" )
	//	limpiaMensajeError(formContext,'3', campoNumDocumento);
	//else
		limpiaMensajeError(formContext,'3', "");	

	var tipoDocumento = formContext.data.entity.attributes.get(campoTipoDocumento);
	var resultado = true;
	var mensaje = "";

	if ( tipoDocumento.getValue() !=null && formContext.getAttribute(campoNumDocumento).getValue() != null )
	{
		var strTipoDocumento = tipoDocumento.getText();
		if ( tipoDocumento.getText() == "CIF" )
		{
			resultado=ValidaCIF(formContext.getAttribute(campoNumDocumento).getValue());
		}
		//NIF
		else if ( tipoDocumento.getText() == "NIF" )
		{
			resultado=ValidaNIF(formContext.getAttribute(campoNumDocumento).getValue());
		}
		//NIE
		else if( tipoDocumento.getText() == "Documento Extranjero" )
		{
			strTipoDocumento = "NIE";
			resultado=valida_NIE(formContext.getAttribute(campoNumDocumento).getValue());
		}
		if ( resultado == false )
		{
			mensaje = "El " + strTipoDocumento + " " + formContext.getAttribute(campoNumDocumento).getValue() + " no es correcto.";
			mensajeError(formContext,mensaje, tipoAviso, "3", campoNumDocumento);
		}
	}

	return mensaje;
}
s