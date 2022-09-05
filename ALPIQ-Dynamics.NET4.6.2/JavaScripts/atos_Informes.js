if (typeof SDK == "undefined")
{ SDK = { __namespace: true }; }


SDK.Action = {

    getInforme: function (ID_Oferta, tipoInforme, esPDF) {

      // Construct a request object from the metadata
        var generateReport = new SDK.ExecuteGenerarInforme(esPDF,tipoInforme,ID_Oferta);

        Xrm.Utility.showProgressIndicator("Generando informe...");
        // Use the request object to execute the function
        Xrm.WebApi.online.execute(generateReport).then(
            function(result) 
            {
                if (result.ok) {
                    Xrm.Utility.closeProgressIndicator();

                    result.json().then(function(response){
                        handleResponse(response);
                    });
                
                }
            },
            function(error) 
            {
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
SDK.ExecuteGenerarInforme = function(esPDF, TipoInforme,Entity_ID) {
    this.esPDF = esPDF.toString();
    this.TipoInforme = TipoInforme;
    this.Entity_ID = Entity_ID;
};

// NOTE: The getMetadata property should be attached to the function prototype instead of the
// function object itself.
SDK.ExecuteGenerarInforme.prototype.getMetadata = function () {
    return {
        boundParameter: null,
        parameterTypes: {
            "esPDF": {
                "typeName": "Edm.String",
                "structuralProperty": 1 // Primitive Type
            },
            "TipoInforme": {
                "typeName": "Edm.String",
                "structuralProperty": 1 // Primitive Type
            },
            "Entity_ID": {
                "typeName": "Edm.String",
                "structuralProperty": 1 // Primitive Type
            }
            
        },
        operationType: 0, // This is an action. Use '1' for functions and '2' for CRUD
        operationName: "atos_Act_GenerateReport",
    };
};

//ACTION CONSTRUCTOR

//Split from main action call, for clearer code. 19/11 A. Ruiz

function handleResponse(result)
{
    var xmltext = "";
    
    if (result.URL_Descarga) {
        //console.log('XML: ' + req.responseXML.xml);
        xmltext = result.URL_Descarga;
    }
    else if (XMLSerializer) {
        var xml_serializer = new XMLSerializer();
        xmltext = xml_serializer.serializeToString(result.URL_Descarga);
        //console.log('XML serializer: ' + xmltext);
    }
    else {
        alert('Error en respuesta del XML');
    }

    if (xmltext != "") {
        var xmlrespuesta;
        //console.log("XML NO VACIO");
        if (typeof window.DOMParser != "undefined") {
            //console.log('OPCION 1');
            xmlrespuesta = (new window.DOMParser()).parseFromString(xmltext, "text/xml");
        }
        else if (typeof window.ActiveXObject != "undefined" && new window.ActiveXObject("Microsoft.XMLDOM")) {
            //console.log('OPCION 2');
            xmlrespuesta = new window.ActiveXObject("Microsoft.XMLDOM");
            xmlrespuesta.async = "false";
            xmlrespuesta.loadXML(xmltext);

        }

        if (xmlrespuesta != null && xmlrespuesta != '') {
            console.log('Respuesta ', xmlrespuesta);
            var urlArchivo = xmlrespuesta.documentElement.childNodes[0].childNodes[0].childNodes[0].childNodes[0].textContent;
                //if (urlArchivo != null && (urlArchivo.includes("http://") || urlArchivo.includes("https://"))) {
            if (urlArchivo != null && (urlArchivo.indexOf("http://") >= 0 || urlArchivo.indexOf("https://") >= 0)) {
                
                addScriptResource(urlArchivo);
                console.log("Soy correcto: " + urlArchivo);
                Xrm.Navigation.openUrl(urlArchivo);
                //window.open(urlArchivo, _blank, "", true);
            } else {
                alert("El servidor envía el siguiente mensaje: \n" + urlArchivo);
            }
        }
        else {
            prompt('Error generado en la generación del informe de gestión ATR', "Error");
        }


    }

}

//MAIN ENTRY POINT. Not reordered just in case. 19/11/2020 Aleix Ruiz

/*
 * Function from Button "Informe" Ribbon "Oferta"
 * @param {*} executionContext 
 * @param {*} tipoInforme 
 * @returns 
 */
function getInforme(executionContext, tipoInforme) {

    var formContext = executionContext;

    debugger;
        if (formContext.data.entity.getEntityName() == "atos_oferta" && 
           (formContext.data.entity.attributes.get("statuscode").getValue() == null || 
            formContext.data.entity.attributes.get("statuscode").getValue() == 1)) {
            alert("No se puede generar un informe si el estado de la oferta es \"Pendiente\".");
            return;
        }
    
        /*
        var esPDF = dialogoConfirmacion();
        oferta_guid = formContext.data.entity.getId();
        console.log("Id de oferta " + oferta_guid + " tipoInforme " + tipoInforme + " ES PDF " + esPDF);
        SDK.Action.getInforme(oferta_guid, tipoInforme, esPDF);
        */
        
        oferta_guid = formContext.data.entity.getId();
        Alert.show("Formato del informe, Excel o Pdf", "¿En qué formato desea obtener el informe (Excel o Pdf)?",
            [new Alert.Button("Excel", function () {
                console.log("Id de oferta " + oferta_guid + " tipoInforme " + tipoInforme + " ES PDF false");
                SDK.Action.getInforme(oferta_guid, tipoInforme, false);
            }, true),
               new Alert.Button("PDF", function () {
                   console.log("Id de oferta " + oferta_guid + " tipoInforme " + tipoInforme + " ES PDF true");
                   SDK.Action.getInforme(oferta_guid, tipoInforme, true);
               })
            ]
            , "QUESTION");
    }

    //No idea what is this for. 19/11 A.Ruiz. 
    function addScriptResource(urlFichero) {
        var script = document.createElement("script");
        script.setAttribute("src", urlFichero);
        document.getElementsByTagName('head')[0].appendChild(script);
    }
    
    
    function dialogoConfirmacion() {
        return confirm("¿Desea obtener el informe en PDF (Aceptar) o en Excel (Cancelar)?");
    }