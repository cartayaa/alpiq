/**
// <summary>
// Se ejecuta en el OnLoad del formulario para validar coeficiente de apuntamiento.
// Autor: Lazaro Castro - 12.11.2020
// </summary>
 */
function preFiltroCoeficienteApuntamiento(executionContext) {
    var formContext = executionContext.getFormContext();
   if (formContext.getAttribute("atos_contratoid").getValue() != null) {
       var contratoid = formContext.getAttribute("atos_contratoid").getValue();
       Xrm.WebApi.retrieveRecord("atos_contrato",contratoid[0].id,"?$select=atos_contratoid&$expand=atos_ofertaid($select=atos_ofertaid,atos_tipooferta)").then(function(result) {
               if (null != result["atos_ofertaid"]){
                      var ofertaid = result["atos_ofertaid"].atos_ofertaid;
                      Xrm.WebApi.retrieveRecord("atos_oferta",ofertaid,"?$select=atos_ofertaid&$expand=atos_ofertapadreid($select=atos_ofertaid)").then(function(result){
                      if (result["atos_ofertapadreid"] != null) {
                       ofertaid = result["atos_ofertapadreid"].atos_ofertaid;
                      }
                      if (formContext.getAttribute("atos_cierreofertaid").getValue() != null){
                           formContext.getControl("atos_pricinginputid").addPreSearch(function () {
                               addFiltroCoeficienteApuntamientoCierre(formContext, ofertaid, formContext.getAttribute("atos_cierreofertaid").getValue()[0].id);
                           });
                      }
                      else{
                           formContext.getControl("atos_pricinginputid").addPreSearch(function () {
                               addFiltroCoeficienteApuntamiento(formContext, ofertaid);
                           });
                      }
                  },function(error) {
                       Xrm.Navigation.openErrorDialog({ detail: error.message });
                   });
               }
           },function(error) {
               Xrm.Navigation.openErrorDialog({ detail: error.message });
        });
   }
}
/**
// <summary>
// Llamada desde preFiltroCoeficienteApuntamiento para filtrar el Control atos_pricinginputid.
// Autor: Lazaro Castro - 12.11.2020
// </summary>
*/
function addFiltroCoeficienteApuntamiento(formContext, ofertaid) {
   if (ofertaid != null) {
       var fetchXml = "<filter type='and'><condition attribute='atos_ofertaid' operator='eq' value='" + ofertaid + "' />" +
           "<condition attribute='atos_cierreofertaid' operator='not-null' /></filter>";
       formContext.getControl("atos_pricinginputid").addCustomFilter(fetchXml);
   }
}

/**
// <summary>
// Llamada desde preFiltroCoeficienteApuntamiento para filtrar el Control atos_pricinginputid.
// Autor: Lazaro Castro - 12.11.2020
// </summary>
*/
function addFiltroCoeficienteApuntamientoCierre(formContext, ofertaid, cierreofertaid) {
   if (ofertaid != null) {
       var fetchXml = "<filter type='and'><condition attribute='atos_ofertaid' operator='eq' value='" + ofertaid + "' />" +
           "<condition attribute='atos_cierreofertaid' operator='eq' value='" + cierreofertaid + "' /></filter>";
       formContext.getControl("atos_pricinginputid").addCustomFilter(fetchXml);
   }
}

/**
// <summary>
// Llamada desde cierreOferta para setear el Control atos_pricinginputid.
// Autor: Lazaro Castro - 12.11.2020
// </summary>
*/
function setVariable(formContext, pricingoutputid) {
   if (pricingoutputid == null) {
       formContext.getAttribute("atos_variable").setValue(null);
   }
   else {
       // var cols = ["atos_name"];
       // var pricingoutput = XrmServiceToolkit.Soap.Retrieve("atos_pricingoutput", pricingoutputid, cols);
       Xrm.WebApi.retrieveRecord("atos_pricingoutput",pricingoutputid,"?$select=atos_name").then(function(result){
       debugger;
       if (result["atos_name"] != "") {
           formContext.getAttribute("atos_variable").setValue(construyeLookup("atos_pricingoutput", pricingoutputid, result["atos_name"]));
           }
       },function(error) {
           Xrm.Navigation.openErrorDialog({ detail: error.message });
       });
   }
   formContext.getAttribute("atos_variable").setSubmitMode("always");
}

function cierreOferta(executionContext) {
   debugger;
    var formContext = executionContext.getFormContext();
    if (formContext.getAttribute("atos_cierreofertaid").getValue() != null) {
       cierraofertaid = formContext.getAttribute("atos_cierreofertaid").getValue();
       // var cols = ["atos_pricingoutputid", "atos_costegestioncierre"];
       // var cierreoferta = XrmServiceToolkit.Soap.Retrieve("atos_cierreoferta", Xrm.Page.data.entity.attributes.get("atos_cierreofertaid").getValue()[0].id, cols);
       Xrm.WebApi.retrieveRecord("atos_cierreoferta",cierraofertaid[0].id,"?$select=atos_costegestioncierre&$expand=atos_pricingoutputid($select=atos_pricingoutputid)").then(function(result){
      debugger;
          if (result["atos_pricingoutputid"] != null) {
               setVariable(formContext, result["atos_pricingoutputid"].atos_pricingoutputid);
          }
          else{
               setVariable(formContext, null);
          }
          if (result["atos_costegestioncierre"] != null){
          debugger;
               formContext.getAttribute("atos_costegestioncierre").setValue(result["atos_costegestioncierre"]);
           }
           else{
               setVariable(formContext, null);
               preFiltroCoeficienteApuntamiento();
          }
       },function(error) {
           Xrm.Navigation.openErrorDialog({ detail: error.message });
       });
   }
}

/**
// <summary>
// Se ejecuta en el OnLoad del formulario para validar coeficiente de apuntamiento.
// Autor: Lazaro Castro - 12.11.2020
// </summary>
*/
function preFiltroCierreOferta(executionContext) {
    var formContext = executionContext.getFormContext();
   if (formContext.getAttribute("atos_contratoid").getValue() != null) {
        // var cols = ["atos_ofertaid"];
       var contratoid = formContext.getAttribute("atos_contratoid").getValue();
       Xrm.WebApi.retrieveRecord("atos_contrato",contratoid[0].id,"?$select=atos_contratoid&$expand=atos_ofertaid($select=atos_ofertaid)").then(function(result) {
               if (null != result["atos_ofertaid"]){
                      var ofertaid = result["atos_ofertaid"].atos_ofertaid;
                      Xrm.WebApi.retrieveRecord("atos_oferta",ofertaid,"?$select=atos_ofertaid&$expand=atos_ofertapadreid($select=atos_ofertaid,atos_tipooferta)").then(function(result){
                      debugger;
                      if (result["atos_ofertapadreid"] != null) {
                       ofertaid = result["atos_ofertapadreid"].atos_ofertaid;
                      }
                       formContext.getControl("atos_cierreofertaid").addPreSearch(function () {
                           addFiltroCierreOferta(formContext, ofertaid);
                       });
                       },function(error) {
                           Xrm.Navigation.openErrorDialog({ detail: error.message });
                   });
               }
           },function(error) {
               Xrm.Navigation.openErrorDialog({ detail: error.message });
        });
   }
}
/**
// <summary>
// Se ejecuta en el OnLoad del formulario para pasar el filtro dal ciere de oferta.
// Autor: Lazaro Castro - 12.11.2020
// </summary>
*/
function addFiltroCierreOferta(formContext, ofertaid) {
   if (ofertaid != null) {
       var fetchXml = "<filter type='and'><condition attribute='atos_ofertaid' operator='eq' value='" + ofertaid + "' /></filter>";
       formContext.getControl("atos_cierreofertaid").addCustomFilter(fetchXml);
   }
}


/**
// <summary>
// FunciÃ³n que comprueba que no estÃ©n rellenos los campos Cierre y el Cierre por MWh.
// </summary>
*/
function validaCierre(executionContext) {
   var formContext = executionContext.getFormContext();
   formContext.getControl("atos_porcentajecierre").clearNotification("5");
   formContext.getControl("atos_valorcierre").clearNotification("5");
   // formContext.clearFormNotification("5");
   formContext.ui.clearFormNotification("5");
   var valido = true;
   var porcentaje = formContext.getAttribute("atos_porcentajecierre").getValue();
   var energia = formContext.getAttribute("atos_valorcierre").getValue();
   //if (porcentaje != null && energia != null) {
   //    valido = false;
   //    menserror = "Tan solo puede estar informado uno de los dos siguientes campos: Valor cierre o Cierre(%)";

   //    Xrm.Page.ui.setFormNotification(menserror, "ERROR", '5');
   //}
   // if (porcentaje == null && energia == null) {
   //	   menserror = "Tiene que estar informado uno de los dos siguientes campos: Valor cierre o Cierre(%)";

   //    Xrm.Page.ui.setFormNotification(menserror, "ERROR", '5'); 
   // }
   return valido;
}

/*

*- Entidad Cierre de Contratos
*- Funcion llamada desde el Ribbon
*
* Funcion que si no se ha hecho una copia previa y en caso de que se haya hecho impide la copia. 
* en otro caso se pregunta al usuario si quiere continuar y se lanza el proceso.
*
* - No se ha encontrado la relacion con esta funcion en Cierre_Contratos
* - Autor: Lazaro Castro  - 13.11.2020

*/
function copiaCierreMP(executionContext) {

   var formContext = executionContext;
   formContext.ui.clearFormNotification();
   var copiaLanzada = formContext.getAttribute("atos_copiaenmultipuntolanzada").getValue();

   if (copiaLanzada == true) {
       formContext.setFormNotification("La copia a multipunto ya ha sido realizada y no puede ejecutarse de nuevo", "ERROR");
   }
   else 
   {

        var confirmStrings = { text:"¿Desea lanzar la copia de cierre en el multipunto?", title:"Copiar multipunto" };
        var confirmOptions = { height: 200, width: 450 };

        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
        function (success) {    
            if (success.confirmed)
            creaTriggerCopia(executionContext);
            else
                console.log("Dialog closed using Cancel button or X.");
        });
   }
}

/*
* Al parecer esta funcon ya no se utiliza
* Autor: Lazaro Castro - 12.11.2020
*/
function creaTriggerCopia(executionContext) {
   var formContext = executionContext;
   // define the data to create new account
   var creaCierre =
   {
       "atos_accion": "Cierre",
       "atos_entity":"atos_cierre",
       "atos_guid": Xrm.Page.data.entity.getId()
   }
   //var creaCierre = new XrmServiceToolkit.Soap.BusinessEntity("atos_trigger");
   //creaCierre.attributes["atos_accion"] = "Cierre";
   //creaCierre.attributes["atos_entity"] = "atos_cierre";
   //creaCierre.attributes["atos_guid"] = Xrm.Page.data.entity.getId();
   // create account record
   Xrm.WebApi.createRecord("atos_trigger", creaCierre).then(
       function success(result) {
           if (result != null) {
               creaCierreId = result.id;
               if (result["atos_respuesta"] != null) {
                   formContext.ui.setFormNotification(result["atos_respuesta"].value, "INFO", '1');
               }
               else {
                   formContext.getAttribute("atos_copiaenmultipuntolanzada").setValue(true);
                   formContext.ui.setFormNotification("Proceso de copia realizado","INFO","1");
               }
           }
       }, 
       function(error) {
           Xrm.Navigation.openErrorDialog({ detail: error.message });
           formContext.ui.clearFormNotification('2');
           formContext.ui.setFormNotification(err.message, 'ERROR', '1');
   });
}

/**
// <summary>
// FunciÃ³n que se ejecuta al guardar en el formulario de Cierre de Contrato.
// </summary>
// <param name="obj">Contexto del objeto. Para poder "abortar" el guardado si no se cumplen las validaciones.</param>
// Modificada: Lazaro Castro - 12.11.2020
// <remarks>
// - Comprueba que estÃ© relleno el Cierre o el Cierre por MWh (solo uno de los dos)
// - Si es errÃ³neo no permite guardar los datos
// </remarks>
*/
function on_Save(executionContext) {
   debugger;
   var formContext = executionContext.getFormContext();
   if (formContext.getAttribute("atos_porcentajecierre").getIsDirty() == true ||
       formContext.getAttribute("atos_valorcierre").getIsDirty() == true) {

       if (validaCierre(executionContext) == false) {

           if (executionContext.getEventArgs() != null) {

               executionContext.getEventArgs().preventDefault();
           }
       }
   }
}

/**
// <summary>
//  FunciÃ³n que comprueba fechas
// </summary>
// <remarks>
//  Modificada: Lazaro Castro - 12.11.2020
//  No se ha detectado las dependencias de esta funciÃ³n
// </remarks>
*/
function CheckDates(executionContext) {
   var formContext = executionContext.getFormContext();
   LimpiarNotificacion(formContext,"atos_fechainiciocierre");
   LimpiarNotificacion(formContext,"atos_fechafincierre");

   var valorFechaInicio = formContext.getAttribute("atos_fechainiciocierre").getValue();
   var valorFechaFin = formContext.getAttribute("atos_fechafincierre").getValue();

   if (valorFechaInicio && valorFechaFin) {
       var inicioDate = new Date(valorFechaInicio);
       var finDate = new Date(valorFechaFin);

       var diaInicio = inicioDate.getDate();
       if (diaInicio == 1) {
           var resultadoComparacion = comparafechasSinHoras(inicioDate, finDate);

           if (resultadoComparacion == -1) {
               var diaFin = finDate.getDate();
               var mesFin = finDate.getMonth();
               var annoFin = finDate.getFullYear();

               var ultimoDiaMes = diasmes(mesFin + 1, annoFin);

               if (diaFin == ultimoDiaMes) {
                   var mesInicio = inicioDate.getMonth();
                   var annoInicio = inicioDate.getFullYear();

                   if (annoInicio == annoFin) {

                       if ((mesInicio + 1 == 1 && mesFin + 1 == 12)) {
                           LimpiarNotificacion(formContext,"atos_fechainiciocierre");
                           LimpiarNotificacion(formContext,"atos_fechafincierre");
                           AsignarCostesCierre(formContext,"Anual");
                       }

                       else if (EsCierreTrimestral(mesInicio + 1, mesFin + 1)) {
                           LimpiarNotificacion(formContext,"atos_fechainiciocierre");
                           LimpiarNotificacion(formContext,"atos_fechafincierre");
                           AsignarCostesCierre(formContext,"Trimestral");
                       }

                       else if (mesInicio == mesFin) {
                           LimpiarNotificacion(formContext,"atos_fechainiciocierre");
                           LimpiarNotificacion(formContext,"atos_fechafincierre");
                           AsignarCostesCierre(formContext,"Mensual");
                       }

                       else {
                           ColocarNotificacion(formContext,"atos_fechafincierre", "Los cierres deben delimitarse dentro del mismo mes, trimestre fiscal o aÃ±o natural");
                           LimpiarCostesCierre(formContext);
                       }
                   }
                   else {
                       ColocarNotificacion(formContext,"atos_fechafincierre", "Los cierres han de estar delimitados dentro del mismo aÃ±o natural");
                       LimpiarCostesCierre(formContext);
                   }

               }
               else {
                   ColocarNotificacion(formContext,"atos_fechafincierre", "El fin del cierre debe coincidir con el Ãºltimo dÃ­a del mes");
                   LimpiarCostesCierre(formContext);
               }
           }
           else {
               ColocarNotificacion(formContext,"atos_fechafincierre", "La fecha de finalizaciÃ³n ha de ser posterior a la de inicio");
               LimpiarCostesCierre(formContext);
           }

       }
       else {
           ColocarNotificacion(formContext,"atos_fechainiciocierre", "Los cierres han de comenzar el primer dÃ­a del mes");
           LimpiarCostesCierre(formContext);
       }
   }
   else {
       LimpiarCostesCierre(formContext);

       if (!valorFechaInicio) {
           ColocarNotificacion(formContext,"atos_fechainiciocierre", "Debe proporcionar un valor para la fecha de inicio");
       }
       if (!valorFechaFin) {
           ColocarNotificacion(formContext,"atos_fechafincierre", "Debe proporcionar un valor para la fecha de fin");
       }
   }
}

function EsCierreTrimestral(mesInicio, mesFin) {
   var correcto = false;
   if (
       (mesInicio == 1 && mesFin == 3) ||
       (mesInicio == 4 && mesFin == 6) ||
       (mesInicio == 7 && mesFin == 9) ||
       (mesInicio == 10 && mesFin == 12)
   ) {
       correcto = true;
   }
   return correcto;
}

function LimpiarNotificacion(formContext,nombreCampo) {
   formContext.getControl(nombreCampo).clearNotification();
}

function ColocarNotificacion(formContext,nombreCampo, mensaje) {
    try{
        formContext = formContext.getFormContext();
    }
    catch{

    }
   formContext.getControl(nombreCampo).setNotification(mensaje);
}

function AsignarCostesCierre(formContext,periodicidad) {

   var idContrato;

   if (formContext.getAttribute("atos_contratoid") && formContext.getAttribute("atos_contratoid").getValue() != null) {

       idContrato = formContext.getAttribute("atos_contratoid").getValue()[0].id;

       var coste = GetCostesCierresContrato(idContrato, periodicidad);
       if (coste && coste != 0) {
           formContext.getAttribute("atos_costegestioncierre").setValue(coste.value);
       }
       else {
           LimpiarCostesCierre(formContext);
       }
   }
   else {
       LimpiarCostesCierre(formContext);
   }
}

function LimpiarCostesCierre(formContext) {
   formContext.getAttribute("atos_costegestioncierre").setValue(0);
}

function GetCostesCierresContrato(idContrato, periodicidad) {

   // var cols = ["atos_costegestioncierresmensuales", "atos_costegestioncierrestrimestrales", "atos_costegestioncierresanuales"];
   // var contrato = XrmServiceToolkit.Soap.Retrieve("atos_contrato", idContrato, cols);

    Xrm.WebApi.retrieveRecord("atos_contrato", idContrato,"?$select=atos_costegestioncierresmensuales,atos_costegestioncierrestrimestrales,atos_costegestioncierresanuales").then(
    function success(contrato) {
        
        switch (periodicidad) {
            case "Mensual":
                if (contrato.attributes["atos_costegestioncierresmensuales"] != "") {
                    coste = contrato.attributes["atos_costegestioncierresmensuales"];
                }
                break;
     
            case "Trimestral":
                if (contrato.attributes["atos_costegestioncierrestrimestrales"] != "") {
                    coste = contrato.attributes["atos_costegestioncierrestrimestrales"];
                }
                break;
     
            case "Anual":
                if (contrato.attributes["atos_costegestioncierresanuales"] != "") {
                    coste = contrato.attributes["atos_costegestioncierresanuales"];
                }
                break;
        }
        return coste;


    }, function (error) {
        Xrm.Navigation.openErrorDialog({ detail: error.message });
    });

   var coste = 0;

}