
/**
// <summary>
// Valida un CUPS 20.
// </summary>
// <param name="CUPS20">Valor del cups 20.</param>
// <remarks>
// Comprueba que el valor de CUPS20 cumple el formato de los CUPS 20 
// - Devuelve true si es correcto, false si no lo es.
// </remarks>
 */
function ValidarCUPS20(CUPS20)
{
	var valido=false;
	var re = new RegExp(/ES\d{16}[A-Z]{2}/g);
	var test=CUPS20.match(re);

	if(test!=null){
		//siguiente validaciÃ³n CUPS
		var paso1=CUPS20.substr(2,16)%529;
		var coc = Math.floor(paso1/23);
		var resto = Math.floor(paso1%23);
		var cods= new Array("T","R","W","A","G","M","Y","F","P","D","X","B","N","J","Z","S","Q","V","H","L","C","K","E");
		var control= cods[coc] + cods[resto];
		if(CUPS20.substr(18,2)==control){ valido=true;}
	}
	return valido;
}


/**
// <summary>
// Valida un CUPS 22.
// </summary>
// <param name="CUPS22">Valor del cups 22.</param>
// <remarks>
// Comprueba que el valor de CUPS22 cumple el formato de los CUPS 22 
// - Devuelve true si es correcto, false si no lo es.
// </remarks>
 */
function ValidarCUPS22(CUPS22)
{
	var valido=new Boolean(0);
	var re =  new RegExp(/ES\d{16}[A-Z]{2}[0-9][A-Z]/g);
	var test=CUPS22.match(re);

	if(test!=null){
		//siguiente validaciÃ³n CUPS, primero quito 2 Ãºltimos caracteres
		valido = ValidarCUPS20(CUPS22.substr(0,20));
	}
	return valido;
}


/**
// <summary>
// Valida un CUPS segÃºn su tipo.
// </summary>
// <param name="CUPS">Valor del cups.</param>
// <param name="TipoCUPS">Tipo del cups (20 o 22).</param>
// <remarks>
// Valida el valor de CUPS llamando a ValidarCUPS20 o ValidarCUPS22 segÃºn el valor de TipoCUPS.
// </remarks>
 */
function ValidarCUPS(CUPS, TipoCUPS)
{
	if ( TipoCUPS == "20" )
		return ValidarCUPS20(CUPS);
	else
		return ValidarCUPS22(CUPS);
}
