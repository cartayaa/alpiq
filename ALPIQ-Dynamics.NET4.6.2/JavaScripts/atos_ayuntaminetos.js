
/**
// <summary>
//Se encarga de realizar todas las validaciones del alta o modificacion de un ayuntamiento.
// De momento se hacen solo validaciones del CIF y de la cuenta bancaria

// </summary>
*/
function Ayuntamiento_OnSave(obj) {
    var serverUrl = Xrm.Page.context.getClientUrl();
    var continuar = true;
    limpiaMensajeError("2", "");


    // validamos el CIF

      if (Xrm.Page.data.entity.attributes.get("atos_cif").getValue() != null) {

          var cif = Xrm.Page.data.entity.attributes.get("atos_cif").getValue();
          var valido = ValidaCIF(cif);

          if (!valido) {
              mensajeError("Se ha introducido un CIF invalido ", "ERROR", "2", "");
              cancelarGuardado(obj);
              return false;
          }

     // validamos la cuenta bancaria
          var swift = "";
          var iban = "";
          var entidadBancaria = "";
          var sucursalBancaria = "";
          var digitoControl = "";
          var cuentaBancaria = "";
          if (Xrm.Page.data.entity.attributes.get("atos_swift").getValue() != null) {
              swift = Xrm.Page.data.entity.attributes.get("atos_swift").getValue()
          }

          if (Xrm.Page.data.entity.attributes.get("atos_iban").getValue() != null) {
              iban = Xrm.Page.data.entity.attributes.get("atos_iban").getValue();
          }

          if (Xrm.Page.data.entity.attributes.get("atos_entidadbancaria").getValue() != null) {
              entidadBancaria = Xrm.Page.data.entity.attributes.get("atos_entidadbancaria").getValue();
          }

          if (Xrm.Page.data.entity.attributes.get("atos_sucursalbancaria").getValue() != null) {
              sucursalBancaria = Xrm.Page.data.entity.attributes.get("atos_sucursalbancaria").getValue();
          }

          if (Xrm.Page.data.entity.attributes.get("atos_digitocontrol").getValue() != null) {
              digitoControl = Xrm.Page.data.entity.attributes.get("atos_digitocontrol").getValue();
          }

          if (Xrm.Page.data.entity.attributes.get("atos_cuentabancaria").getValue() != null) {
              cuentaBancaria = Xrm.Page.data.entity.attributes.get("atos_cuentabancaria").getValue();
          }


          if ((iban.length > 0 || entidadBancaria.length > 0 || sucursalBancaria.length > 0 || digitoControl.length > 0 || cuentaBancaria.length > 0)
               && (iban.length == 0 || entidadBancaria.length == 0 || sucursalBancaria.length == 0 || digitoControl.length == 0 || cuentaBancaria.length == 0)) {
              mensajeError("Si se quiere aÃ±adir datos bancarias,hay que rellenar todos los datos bancarios menos el swift que es optativo ", "ERROR", "2", "");
              cancelarGuardado(obj);
              return false;
          }

          if (iban.length >0)
		  {
			  if (!(validaCuentaBancaria(entidadBancaria,sucursalBancaria,digitoControl,cuentaBancaria)))
			  {
				  mensajeError("los datos de la cuenta son incorrectos ", "ERROR", "2", "");
				  cancelarGuardado(obj);
				  return false;
			  }

			  if (calcularIban(entidadBancaria + sucursalBancaria + digitoControl + cuentaBancaria) != iban) {
				  mensajeError("los datos de la cuenta son incorrectos ", "ERROR", "2", "");
				  cancelarGuardado(obj);
				  return false;
			  }
		  }
      }

      return true;
  }


 function Cif_OnChange() { 
  // validamos el CIF
     limpiaMensajeError("2", "");
      if (Xrm.Page.data.entity.attributes.get("atos_cif").getValue() != null) {

          var cif = Xrm.Page.data.entity.attributes.get("atos_cif").getValue();
          var valido = ValidaCIF(cif);

          if (!valido) {
              mensajeError("Se ha introducido un CIF invalido ", "ERROR", "2", "");
              return false;
          }
	  }
	  return true;
 }
  
  function cuentaBancaria_OnChange(obj) {
 
           var iban = "";
          var entidadBancaria = "";
          var sucursalBancaria = "";
          var digitoControl = "";
          var cuentaBancaria = "";
       

          if (Xrm.Page.data.entity.attributes.get("atos_iban").getValue() != null) {
              iban = Xrm.Page.data.entity.attributes.get("atos_iban").getValue();
          }

          if (Xrm.Page.data.entity.attributes.get("atos_entidadbancaria").getValue() != null) {
              entidadBancaria = Xrm.Page.data.entity.attributes.get("atos_entidadbancaria").getValue();
          }

          if (Xrm.Page.data.entity.attributes.get("atos_sucursalbancaria").getValue() != null) {
              sucursalBancaria = Xrm.Page.data.entity.attributes.get("atos_sucursalbancaria").getValue();
          }

          if (Xrm.Page.data.entity.attributes.get("atos_digitocontrol").getValue() != null) {
              digitoControl = Xrm.Page.data.entity.attributes.get("atos_digitocontrol").getValue();
          }

          if (Xrm.Page.data.entity.attributes.get("atos_cuentabancaria").getValue() != null) {
              cuentaBancaria = Xrm.Page.data.entity.attributes.get("atos_cuentabancaria").getValue();
          }

		   if (iban.length > 0 && entidadBancaria.length > 0 && sucursalBancaria.length > 0 && digitoControl.length > 0 && cuentaBancaria.length > 0)
           {
			    if ((validaCuentaBancaria(entidadBancaria,sucursalBancaria,digitoControl,cuentaBancaria)) &&
				(calcularIban(entidadBancaria + sucursalBancaria + digitoControl + cuentaBancaria) == iban) )
				{
					Xrm.Page.data.entity.attributes.get("atos_cuentabancariaconcatenada").setValue(iban + "-" + entidadBancaria + "-" + sucursalBancaria + "-" + digitoControl+ "-" + cuentaBancaria );			
					return true;
				}
		   }
		  
		 Xrm.Page.data.entity.attributes.get("atos_cuentabancariaconcatenada").setValue("");
 
  }
  // <summary>
  // Se encarga de anular el guardado de un ayuntamiento
  // </summary>
  function cancelarGuardado(obj) {
      if (obj.getEventArgs() != null)
          obj.getEventArgs().preventDefault();
  }

  /**
  // <summary>
  // Valida si la cuenta bancaria es correcta o no
  // </summary>
  */
  function validaCuentaBancaria(i_entidad, i_oficina, i_digito, i_cuenta) {
      // Funcion recibe como parÃ¡metro la entidad, la oficina, 
      // el digito (concatenaciÃ³n del de control entidad-oficina y del de control entidad)
      // y la cuenta. Espera los valores con 0's a la izquierda.
      // Devuelve true o false.
      // NOTAS:
      // Formato deseado de los parÃ¡metros:
      // - i_entidad (4)
      // - i_oficina (4)
      // - i_digito (2)
      // - i_cuenta (10)
      var wtotal, wcociente, wresto;
      if (i_entidad.length != 4) {
          return false;
      }
      if (i_oficina.length != 4) {
          return false;
      }
      if (i_digito.length != 2) {
          return false;
      }
      if (i_cuenta.length != 10) {
          return false;
      }
      wtotal = i_entidad.charAt(0) * 4;
      wtotal += i_entidad.charAt(1) * 8;
      wtotal += i_entidad.charAt(2) * 5;
      wtotal += i_entidad.charAt(3) * 10;
      wtotal += i_oficina.charAt(0) * 9;
      wtotal += i_oficina.charAt(1) * 7;
      wtotal += i_oficina.charAt(2) * 3;
      wtotal += i_oficina.charAt(3) * 6;
      // busco el resto de dividir wtotal entre 11
      wcociente = Math.floor(wtotal / 11);
      wresto = wtotal - (wcociente * 11);
      //
      wtotal = 11 - wresto;
      if (wtotal == 11) {
          wtotal = 0;
      }
      if (wtotal == 10) {
          wtotal = 1;
      }
      if (wtotal != i_digito.charAt(0)) {
          return false;
      }
      //hemos validado la entidad y oficina
      //-----------------------------------
      wtotal = i_cuenta.charAt(0) * 1;
      wtotal += i_cuenta.charAt(1) * 2;
      wtotal += i_cuenta.charAt(2) * 4;
      wtotal += i_cuenta.charAt(3) * 8;
      wtotal += i_cuenta.charAt(4) * 5;
      wtotal += i_cuenta.charAt(5) * 10;
      wtotal += i_cuenta.charAt(6) * 9;
      wtotal += i_cuenta.charAt(7) * 7;
      wtotal += i_cuenta.charAt(8) * 3;
      wtotal += i_cuenta.charAt(9) * 6;

      // busco el resto de dividir wtotal entre 11
      wcociente = Math.floor(wtotal / 11);
      wresto = wtotal - (wcociente * 11);
      //
      wtotal = 11 - wresto;
      if (wtotal == 11) { wtotal = 0; }
      if (wtotal == 10) { wtotal = 1; }

      if (wtotal != i_digito.charAt(1)) {
          return false;
      }
      // hemos validado la cuenta corriente

      return true;
  }


  function calcularIban(ccc) {

      //Limpiamos el numero de IBAN
      var cccaux = ccc.toUpperCase();  //Todo a Mayus
      cccaux = cccaux.trim(); //Quitamos blancos de principio y final.
      cccaux = cccaux.replace(/\s/g, "");  //Quitamos blancos del medio.
      cccaux = cccaux + "142800"; // AÃ±adimos el cÃ³digo de EspaÃ±a

      var trozo = cccaux.substr(0, 5);
      var resto = trozo % 97;

      trozo = cccaux.substr(5, 5);
      var trozoaux = resto + trozo;
      resto = trozoaux % 97;

      trozo = cccaux.substr(10, 5);
      trozoaux = resto + trozo;
      resto = trozoaux % 97;

      trozo = cccaux.substr(15, 5);
      trozoaux = resto + trozo;
      resto = trozoaux % 97;

      trozo = cccaux.substr(20, 6);
      trozoaux = resto + trozo;
      resto = trozoaux % 97;

      digitocontrol = 98 - resto;
      if (digitocontrol < 10) digitocontrol = '0' + String(digitocontrol);
     return ("ES" + digitocontrol);
  }

  
  
  /**
// <summary>
// Se ejecuta en el OnChange del campo de CÃ³digo Postal para rellenar los campos relacionados de la direcciÃ³n.
// </summary>
// <param name="campocpostal">Nombre del campo CÃ³digo Postal en el formulario.</param>
// <param name="campopoblacion">Nombre del campo PoblaciÃ³n en el formulario.</param>
// <param name="campomunicipio">Nombre del campo Municipio en el formulario.</param>
// <param name="campoprovincia">Nombre del campo Provincia en el formulario.</param>
// <param name="campoccaa">Nombre del campo CCAA en el formulario.</param>
// <param name="campopais">Nombre del campo pais en el formulario.</param>
// <remarks>
// Carga los javascripts necesarios:
// -# atos_mensajes.js
// -# atos_json2.js
// -# atos_jquery.js
// -# atos_XrmServiceToolkit.js
// -# atos_direccion.js
// - Finalmente llama a la funciÃ³n recuperaDireccionCPostal
// </remarks>
 */
function cpostal_OnChange(campocpostal, campopoblacion, campomunicipio, campoprovincia, campoccaa, campopais)
{
	var serverUrl = Xrm.Page.context.getClientUrl();
				
	head.load(serverUrl + "/WebResources/atos_mensajes.js", serverUrl + "/WebResources/atos_json2.js", serverUrl + "/WebResources/atos_jquery.js", serverUrl + "/WebResources/atos_XrmServiceToolkit.js", serverUrl + "/WebResources/atos_direccion.js", function() {
		recuperaDireccionCPostal(campocpostal, campopoblacion, campomunicipio, campoprovincia, campoccaa, campopais);
	});	
}
  