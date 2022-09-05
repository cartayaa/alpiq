if (typeof (SDK) == "undefined")
{ SDK = { __namespace: true }; }
SDK.Action = {
    _getClientUrl: function () {     
       var ServicePath = "/XRMServices/2011/Organization.svc/web";
        var clientUrl = "";
        if (typeof GetGlobalContext == "function") {
            var context = GetGlobalContext();
            clientUrl = context.getClientUrl();
        }
        else {
            if (typeof Xrm.Page.context == "object") {
                clientUrl = Xrm.Page.context.getClientUrl();
            }
            else
            { throw new Error("Unable to access the server URL"); }
        }
        if (clientUrl.match(/\/$/)) {
            clientUrl = clientUrl.substring(0, clientUrl.length - 1);
        }
        return clientUrl + ServicePath;
    },
	parseResponse: function(httpRequest) {
		
		var xmlDoc = httpRequest.responseXML;

		if (!xmlDoc || !xmlDoc.documentElement) {
			if (window.DOMParser) {
				var parser = new DOMParser();
				try {
					xmlDoc = parser.parseFromString (httpRequest.responseText, "text/xml");
				} catch (e) {
					alert ("XML parsing error");
					return null;
				};
			}
			else {
				xmlDoc = CreateMSXMLDocumentObject ();
				if (!xmlDoc) {
					return null;
				}
				xmlDoc.loadXML (httpRequest.responseText);

			}
		}

		var errorMsg = null;
		if (xmlDoc.parseError && xmlDoc.parseError.errorCode != 0) {
			errorMsg = "XML Parsing Error: " + xmlDoc.parseError.reason
					  + " at line " + xmlDoc.parseError.line
					  + " at position " + xmlDoc.parseError.linepos;
		}
		else {
			if (xmlDoc.documentElement) {
				if (xmlDoc.documentElement.nodeName == "parsererror") {
					errorMsg = xmlDoc.documentElement.childNodes[0].nodeValue;
				}
			}
		}
		if (errorMsg) {
			alert (errorMsg);
			return null;
		}

		return xmlDoc;
	},
    getInforme : function (periodo, tipoInforme) {

      // Construct a request object from the metadata
	  var generateReport = new SDK.ExecuteGenerarInformeAtr(periodo,tipoInforme);

	  Xrm.Utility.showProgressIndicator("Generando informe...");
	  // Use the request object to execute the function
	  Xrm.WebApi.online.execute(generateReport).then(
		  function(result) {
			  if (result.ok) {
				  Xrm.Utility.closeProgressIndicator();
	  
				  result.json().then(function(response){
					  handleResponse(response);
				  });
			  
			  }
		  },
		  function(error) {
			  Xrm.Utility.closeProgressIndicator();
			  var alertStrings = { confirmButtonLabel: "OK", text: error, title: "Se ha producido un error al generar el informe." };
	  var alertOptions = { height: 120, width: 260 };
	  Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
		  function (success) {
			  console.log("Alert dialog closed");
		  },
		  function (error) {
			  console.log(error.message);
		  }
	  );
		  }
	  );
		
	},
	
	
    __namespace: true
};

//ACTION CONSTRUCTOR
SDK.ExecuteGenerarInformeAtr = function(periodoGestionATR, tipoInformeATR) {
    this.periodoGestionATR = periodoGestionATR.toString();
    this.tipoInformeATR = tipoInformeATR;

};

// NOTE: The getMetadata property should be attached to the function prototype instead of the
// function object itself.
SDK.ExecuteGenerarInformeAtr.prototype.getMetadata = function () {
    return {
        boundParameter: null,
        parameterTypes: {
            "periodoGestionATR": {
                "typeName": "Edm.String",
                "structuralProperty": 1 // Primitive Type
            },
            "tipoInformeATR": {
                "typeName": "Edm.String",
                "structuralProperty": 1 // Primitive Type
            }            
        },
        operationType: 0, // This is an action. Use '1' for functions and '2' for CRUD
        operationName: "atos_InfGestionATR",
    };
};

//ACTION CONSTRUCTOR

function handleResponse(result)
{
 //work with the response here
        //var strResponse = req.responseXML.xml;
		var xmltext = "";

		if(result.correcto == false || result.urlDescarga == null){
			alert("Se ha producido un error al generar el informe. El servidor envía el siguiente mensaje: \n" + result.errorMessage);
			return;
		}
		
		else{
			xmltext = result.urlDescarga;
		}

		
		if ( xmltext != "" ){
			var xmlrespuesta;
			//alert("XML NO VACIO");
			if (typeof window.DOMParser != "undefined") {
				//alert('OPCION 1');
				xmlrespuesta = ( new window.DOMParser() ).parseFromString(xmltext, "text/xml");
			}
			else if (typeof window.ActiveXObject != "undefined" &&
						new window.ActiveXObject("Microsoft.XMLDOM")) {
				alert('OPCION 2');
				xmlrespuesta = new window.ActiveXObject("Microsoft.XMLDOM");
				xmlrespuesta.async = "false";
				xmlrespuesta.loadXML(xmltext);
				
			} 
			
			if(xmlrespuesta != null && xmlRespuesta != ''){
				//alert("XML NO VACIO");
				//prompt('salida', xmlRespuesta);
				
			
				var urlArchivo = xmlrespuesta.documentElement.childNodes[0].childNodes[0].childNodes[0].childNodes[0].textContent;
				
				//alert("Par 1: " + correcto);
				//alert("Par 2: " + urlArchivo);
				//alert("Par 3: " + errorMessage);
				
				//alert(correcto + ' -- ' + urlArchivo);
				if(correcto == "true" && urlArchivo != ""){
					//alert("Soy correcto");

					Xrm.Navigation.openUrl(urlArchivo);
				}else{
					alert("El servidor envía el siguiente mensaje: \n" + result.errorMessage);
				}
			}
			else{
				prompt('Error generado en la generación del informe de gestión ATR', "error");
			}
			
			
		}
		

}

function getInforme(tipoInforme) {
	debugger;
	var today = new Date();
	var mes = today.getMonth();
	if(mes < 10){
		mes = "0" + mes;
	}
	var valorDefecto = today.getFullYear() + mes;
	//alert(valorDefecto);
	var periodo = prompt("Introduzca el periodo del que quiere extraer el informe de gestión ATR: \nFormato Requerido: AAAAMM    Ejemplo: 201601\n", valorDefecto);
	//alert("Periodo: " + periodo + " tipoInforme: " + tipoInforme);
	if(periodo != null && periodo != '' && comprobarPeriodo(periodo)){
		SDK.Action.getInforme(periodo, tipoInforme);
	}
	
}


function comprobarPeriodo(fecha){
	if(fecha.length == 6){
		var anio = fecha.substring(0,4);
		var mes = fecha.substring(4,6);
		var d = new Date(anio+"-"+mes);
		var today = new Date();
		//alert("Aqui -- Año: " + anio + " mes: " + mes + " \nfecha completa: " + d + " \nhoy es: " + today);
		if(d != "Invalid Date" && d < today){
			return true;
		}else{
			alert("Fecha Incorrecta");
		}
		
	}else{
		alert("Las dimensiones del periodo no coinciden con los 6 caracteres requeridos");
	}
	
	return false;
}