var formContext;

async function cambioTipoDocumento() {

    var pasoatr = null;
    var procesoatr = null;
    var pasoatrValue = null;
    var procesoatrValue = null;

    if (formContext.data.entity.attributes.get("atos_messagetype").getValue() != null &&
		formContext.data.entity.attributes.get("atos_messagetype").getValue()[0] != null) {
  
        await Xrm.WebApi.retrieveRecord("atos_tablasatrgas", formContext.data.entity.attributes.get("atos_messagetype").getValue()[0].id,"?$select=atos_codigo").then(
             function success(record) {
                 pasoatr = record;
                 pasoatrValue = pasoatr == null ? null : pasoatr.atos_codigo;

             }, function (error) {
                 Xrm.Navigation.openErrorDialog({ detail: error.message });
             });
 
    }
    if (formContext.data.entity.attributes.get("atos_processcode").getValue() != null &&
		formContext.data.entity.attributes.get("atos_processcode").getValue()[0] != null) {

        await Xrm.WebApi.retrieveRecord("atos_tablasatrgas", formContext.data.entity.attributes.get("atos_processcode").getValue()[0].id,"?$select=atos_codigo").then(
            function success(record) {
                procesoatr = record;
                procesoatrValue = procesoatr == null ? null : pasoatr.atos_codigo;

            }, function (error) {
                Xrm.Navigation.openErrorDialog({ detail: error.message });
            });

    }


    var total = pasoatrValue + procesoatrValue;


    if (total == "A102" || total == "A104" || total == "A138") {
        if (formContext.data.entity.attributes.get("atos_documenttype").getValue() != null &&
		    formContext.data.entity.attributes.get("atos_documenttype").getValue()[0] != null) {
            var tipoDoc = formContext.data.entity.attributes.get("atos_documenttype").getValue()[0].name.split(":")[0];
            if (tipoDoc == "07" || tipoDoc == "08") {
                formContext.getControl("atos_titulartype").setDisabled(false);
                formContext.getAttribute("atos_titulartype").setRequiredLevel("required");
                return;
            }
        }

        formContext.getControl("atos_titulartype").setDisabled(true);
        formContext.getAttribute("atos_titulartype").setRequiredLevel("none");
    }
}

async function cambioFechaEfectiva() {


    var pasoatr = null;
    var procesoatr = null;
    var pasoatrValue = null;
    var procesoatrValue = null;
    if (formContext.data.entity.attributes.get("atos_messagetype").getValue() != null &&
    formContext.data.entity.attributes.get("atos_messagetype").getValue()[0] != null) {

    await Xrm.WebApi.retrieveRecord("atos_tablasatrgas", formContext.data.entity.attributes.get("atos_messagetype").getValue()[0].id,"?$select=atos_codigo").then(
         function success(record) {
             pasoatr = record;
             pasoatrValue = pasoatr == null ? null : pasoatr.atos_codigo;

         }, function (error) {
             Xrm.Navigation.openErrorDialog({ detail: error.message });
         });

}
if (formContext.data.entity.attributes.get("atos_processcode").getValue() != null &&
    formContext.data.entity.attributes.get("atos_processcode").getValue()[0] != null) {

    await Xrm.WebApi.retrieveRecord("atos_tablasatrgas", formContext.data.entity.attributes.get("atos_processcode").getValue()[0].id,"?$select=atos_codigo").then(
        function success(record) {
            procesoatr = record;
            procesoatrValue = procesoatr == null ? null : pasoatr.atos_codigo;

        }, function (error) {
            Xrm.Navigation.openErrorDialog({ detail: error.message });
        });

}

    var total = pasoatrValue + procesoatrValue;

    if (total == "A102" || total == "A105" || total == "A141") {
        if (formContext.data.entity.attributes.get("atos_modeffectdate").getValue() != null) {
            var fechaEffect = formContext.data.entity.attributes.get("atos_modeffectdate").getValue()[0].name.split(":")[0];
            if (fechaEffect == "03" || fechaEffect == "04") {
                formContext.getAttribute("atos_reqtransferdate").setRequiredLevel("required");
                return;
            }
        }

        formContext.getAttribute("atos_reqtransferdate").setRequiredLevel("none");
    }


    if (total == "A138") {
        if (formContext.data.entity.attributes.get("atos_modeffectdate").getValue() != null &&
			formContext.data.entity.attributes.get("atos_modeffectdate").getValue()[0] != null) {
            var fechaEffect = formContext.data.entity.attributes.get("atos_modeffectdate").getValue()[0].name.split(":")[0];
            if (fechaEffect == "03" || fechaEffect == "04") {
                formContext.getAttribute("atos_reqactivationdate").setRequiredLevel("required");
                return;
            }
        }

        formContext.getAttribute("atos_reqactivationdate").setRequiredLevel("none");
    }

}

async function cambioDatosusuario() {

    var pasoatr = null;
    var procesoatr = null;
    var pasoatrValue = null;
    var procesoatrValue = null;

    if (formContext.data.entity.attributes.get("atos_messagetype").getValue() != null &&
    formContext.data.entity.attributes.get("atos_messagetype").getValue()[0] != null) {

    await Xrm.WebApi.retrieveRecord("atos_tablasatrgas", formContext.data.entity.attributes.get("atos_messagetype").getValue()[0].id,"?$select=atos_codigo").then(
         function success(record) {
             pasoatr = record;
             pasoatrValue = pasoatr == null ? null : pasoatr.atos_codigo;

         }, function (error) {
             Xrm.Navigation.openErrorDialog({ detail: error.message });
         });

}
if (formContext.data.entity.attributes.get("atos_processcode").getValue() != null &&
    formContext.data.entity.attributes.get("atos_processcode").getValue()[0] != null) {

    await Xrm.WebApi.retrieveRecord("atos_tablasatrgas", formContext.data.entity.attributes.get("atos_processcode").getValue()[0].id,"?$select=atos_codigo").then(
        function success(record) {
            procesoatr = record;
            procesoatrValue = procesoatr == null ? null : pasoatr.atos_codigo;

        }, function (error) {
            Xrm.Navigation.openErrorDialog({ detail: error.message });
        });

}

    var total = pasoatrValue + procesoatrValue;

    if (total == "A105") {
        if (formContext.data.entity.attributes.get("atos_updatereason").getValue() != null &&
		formContext.data.entity.attributes.get("atos_updatereason").getValue()[0] != null) {
            var updateReason = formContext.data.entity.attributes.get("atos_updatereason").getValue()[0].name.split(":")[0];

            var arrayTotal = ["01", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23"];

            var arraySurrogacy = ["01", "12", "14", "16", "17", "19", "21", "23"];
            var arrayPeaje = ["11", "12", "15", "16", "18", "19", "22", "23"];
            var arrayReqqd = ["13", "14", "15", "16", "20", "21", "22", "23"];
            var arrayDatosClientes = ["01", "12", "14", "16", "17", "19", "21", "23"];
            var arrayCae = ["09"];
            var arrayDireccion = ["10", "17", "18", "19", "20", "21", "22", "23"];

            if (arrayTotal.indexOf(updateReason) > -1) {
                // surrogacy
                if (arraySurrogacy.indexOf(updateReason) > -1) {
                    formContext.getAttribute("atos_surrogacy").setRequiredLevel("required");
                }
                else {
                    formContext.getAttribute("atos_surrogacy").setRequiredLevel("none");
                }
                // peaje
                if (arrayPeaje.indexOf(updateReason) > -1) {
                    formContext.getAttribute("atos_newtolltype").setRequiredLevel("required");
                }
                else {
                    formContext.getAttribute("atos_newtolltype").setRequiredLevel("none");
                }
                // caudal
                if (arrayReqqd.indexOf(updateReason) > -1) {
                    formContext.getAttribute("atos_newreqqd").setRequiredLevel("required");
                }
                else {
                    formContext.getAttribute("atos_newreqqd").setRequiredLevel("none");
                }

                // datos clientes
                if (arrayDatosClientes.indexOf(updateReason) > -1) {
                    formContext.getAttribute("atos_newnationality").setRequiredLevel("required");
                    formContext.getAttribute("atos_newdocumenttype").setRequiredLevel("required");
                    formContext.getAttribute("atos_newdocumentnum").setRequiredLevel("required");
                    if (formContext.data.entity.attributes.get("atos_newtitulartype").getValue() != null &&
					formContext.data.entity.attributes.get("atos_newtitulartype").getValue()[0] != null) {
                        var titularType = formContext.data.entity.attributes.get("atos_newtitulartype").getValue()[0].name.split(":")[0];
                        if (titularType == "F") {
                            formContext.getAttribute("atos_newfirstname").setRequiredLevel("required");
                        }
                    }
                    formContext.getAttribute("atos_newfamilyname1").setRequiredLevel("required");
                    formContext.getAttribute("atos_newfamilyname2").setRequiredLevel("required");
                    formContext.getAttribute("atos_newtitulartype").setRequiredLevel("required");
                    formContext.getAttribute("atos_newregularaddress").setRequiredLevel("required");
                    formContext.getAttribute("atos_newtelephone").setRequiredLevel("required");
                    formContext.getAttribute("atos_newfax").setRequiredLevel("required");
                    formContext.getAttribute("atos_newemail").setRequiredLevel("required");
                }
                else {
                    formContext.getAttribute("atos_newnationality").setRequiredLevel("none");
                    formContext.getAttribute("atos_newdocumenttype").setRequiredLevel("none");
                    formContext.getAttribute("atos_newdocumentnum").setRequiredLevel("none");
                    formContext.getAttribute("atos_newfirstname").setRequiredLevel("none");
                    formContext.getAttribute("atos_newfamilyname1").setRequiredLevel("none");
                    formContext.getAttribute("atos_newfamilyname2").setRequiredLevel("none");
                    formContext.getAttribute("atos_newtitulartype").setRequiredLevel("none");
                    formContext.getAttribute("atos_newregularaddress").setRequiredLevel("none");
                    formContext.getAttribute("atos_newtelephone").setRequiredLevel("none");
                    formContext.getAttribute("atos_newfax").setRequiredLevel("none");
                    formContext.getAttribute("atos_newemail").setRequiredLevel("none");
                }
                // cae
                if (arrayCae.indexOf(updateReason) > -1) {
                    formContext.getAttribute("atos_newcaecode").setRequiredLevel("required");
                }
                else {
                    formContext.getAttribute("atos_newcaecode").setRequiredLevel("none");
                }
                // Direccion
                if (arrayDireccion.indexOf(updateReason) > -1) {
                    formContext.getAttribute("atos_newgasusetype").setRequiredLevel("required");
                    formContext.getAttribute("atos_newprovinceowner").setRequiredLevel("required");
                    formContext.getAttribute("atos_newcityowner").setRequiredLevel("required");
                    formContext.getAttribute("atos_newzipcodeowner").setRequiredLevel("required");
                    formContext.getAttribute("atos_newstreettypeowner").setRequiredLevel("required");
                    formContext.getAttribute("atos_newstreetowner").setRequiredLevel("required");
                    formContext.getAttribute("atos_newstreetnumberowner").setRequiredLevel("required");

                }
                else {
                    formContext.getAttribute("atos_newgasusetype").setRequiredLevel("none");
                    formContext.getAttribute("atos_newprovinceowner").setRequiredLevel("none");
                    formContext.getAttribute("atos_newcityowner").setRequiredLevel("none");
                    formContext.getAttribute("atos_newzipcodeowner").setRequiredLevel("none");
                    formContext.getAttribute("atos_newstreettypeowner").setRequiredLevel("none");
                    formContext.getAttribute("atos_newstreetowner").setRequiredLevel("none");
                    formContext.getAttribute("atos_newstreetnumberowner").setRequiredLevel("none");
                }




                return;
            }

        }
        formContext.getAttribute("atos_surrogacy").setRequiredLevel("none");
        formContext.getAttribute("atos_newtolltype").setRequiredLevel("none");
        formContext.getAttribute("atos_newreqqd").setRequiredLevel("none");
        formContext.getAttribute("atos_newnationality").setRequiredLevel("none");
        formContext.getAttribute("atos_newdocumenttype").setRequiredLevel("none");
        formContext.getAttribute("atos_newdocumentnum").setRequiredLevel("none");
        formContext.getAttribute("atos_newfirstname").setRequiredLevel("none");
        formContext.getAttribute("atos_newfamilyname1").setRequiredLevel("none");
        formContext.getAttribute("atos_newfamilyname2").setRequiredLevel("none");
        formContext.getAttribute("atos_newtitulartype").setRequiredLevel("none");
        formContext.getAttribute("atos_newregularaddress").setRequiredLevel("none");
        formContext.getAttribute("atos_newtelephone").setRequiredLevel("none");
        formContext.getAttribute("atos_newfax").setRequiredLevel("none");
        formContext.getAttribute("atos_newemail").setRequiredLevel("none");
        formContext.getAttribute("atos_newcaecode").setRequiredLevel("none");
        formContext.getAttribute("atos_newgasusetype").setRequiredLevel("none");
        formContext.getAttribute("atos_newprovinceowner").setRequiredLevel("none");
        formContext.getAttribute("atos_newcityowner").setRequiredLevel("none");
        formContext.getAttribute("atos_newstreettypeowner").setRequiredLevel("none");
        formContext.getAttribute("atos_newstreetnumberowner").setRequiredLevel("none");

    }
}

async function cambioDatosUsuarioComercializadora() {

    var colPaso = ["atos_codigo"];
    var colProceso = ["atos_codigo"];

    var pasoatr = null;
    var procesoatr = null;
    var pasoatrValue = null;
    var procesoatrValue = null;

    if (formContext.data.entity.attributes.get("atos_messagetype").getValue() != null &&
    formContext.data.entity.attributes.get("atos_messagetype").getValue()[0] != null) {

    await Xrm.WebApi.retrieveRecord("atos_tablasatrgas", formContext.data.entity.attributes.get("atos_messagetype").getValue()[0].id,"?$select=atos_codigo").then(
         function success(record) {
             pasoatr = record;
             pasoatrValue = pasoatr == null ? null : pasoatr.atos_codigo;

         }, function (error) {
             Xrm.Navigation.openErrorDialog({ detail: error.message });
         });

}
if (formContext.data.entity.attributes.get("atos_processcode").getValue() != null &&
    formContext.data.entity.attributes.get("atos_processcode").getValue()[0] != null) {

    await Xrm.WebApi.retrieveRecord("atos_tablasatrgas", formContext.data.entity.attributes.get("atos_processcode").getValue()[0].id,"?$select=atos_codigo").then(
        function success(record) {
            procesoatr = record;
            procesoatrValue = procesoatr == null ? null : pasoatr.atos_codigo;

        }, function (error) {
            Xrm.Navigation.openErrorDialog({ detail: error.message });
        });

}

    var total = pasoatrValue + procesoatrValue;

    if (total == "A141") {
        if (formContext.data.entity.attributes.get("atos_updatereason").getValue() != null &&
		formContext.data.entity.attributes.get("atos_updatereason").getValue()[0] != null) {
            var updateReason = formContext.data.entity.attributes.get("atos_updatereason").getValue()[0].name.split(":")[0];

            var arrayTotal = ["01", "02", "03"];

            var arrayDatosCliente = ["02"];
            var arrayReqqd = ["02", "03"];
            var arrayDatos = ["01", "03"];
            ;

            if (arrayTotal.indexOf(updateReason) > -1) {

                if (arrayDatosCliente.indexOf(updateReason) > -1) {
                    formContext.getAttribute("atos_nationality").setRequiredLevel("required");
                    formContext.getAttribute("atos_documenttype").setRequiredLevel("required");
                    formContext.getAttribute("atos_documentnum").setRequiredLevel("required");
                }
                else {
                    formContext.getAttribute("atos_nationality").setRequiredLevel("none");
                    formContext.getAttribute("atos_documenttype").setRequiredLevel("none");
                    formContext.getAttribute("atos_documentnum").setRequiredLevel("none");
                }

                if (arrayReqqd.indexOf(updateReason) > -1) {
                    formContext.getAttribute("atos_newreqqd").setRequiredLevel("required");
                }
                else {
                    formContext.getAttribute("atos_newreqqd").setRequiredLevel("none");
                }

                if (arrayDatos.indexOf(updateReason) > -1) {
                    formContext.getAttribute("atos_surrogacy").setRequiredLevel("required");
                    if (formContext.data.entity.attributes.get("atos_newtitulartype").getValue() != null &&
					formContext.data.entity.attributes.get("atos_newtitulartype").getValue()[0] != null) {
                        var titularType = formContext.data.entity.attributes.get("atos_newtitulartype").getValue()[0].name.split(":")[0];
                        if (titularType == "F") {
                            formContext.getAttribute("atos_newfirstname").setRequiredLevel("required");
                        }
                        else {
                            formContext.getAttribute("atos_newfirstname").setRequiredLevel("none");
                        }
                    }

                    formContext.getAttribute("atos_newfamilyname1").setRequiredLevel("required");
                    formContext.getAttribute("atos_newtitulartype").setRequiredLevel("required");

                    formContext.getAttribute("atos_newregularaddress").setRequiredLevel("required");
                    formContext.getAttribute("atos_newtelephone1").setRequiredLevel("required");
                    formContext.getAttribute("atos_newprovinceowner").setRequiredLevel("required");
                    formContext.getAttribute("atos_newcityowner").setRequiredLevel("required");
                    formContext.getAttribute("atos_newzipcodeowner").setRequiredLevel("required");
                    formContext.getAttribute("atos_newstreettypeowner").setRequiredLevel("required");
                    formContext.getAttribute("atos_newstreetowner").setRequiredLevel("required");
                    formContext.getAttribute("atos_newstreetnumberowner").setRequiredLevel("required");
                    formContext.getAttribute("atos_newnationality").setRequiredLevel("required");
                    formContext.getAttribute("atos_newdocumenttype").setRequiredLevel("required");
                    formContext.getAttribute("atos_newdocumentnum").setRequiredLevel("none");
                }
                else {
                    formContext.getAttribute("atos_surrogacy").setRequiredLevel("none");
                    formContext.getAttribute("atos_newfirstname").setRequiredLevel("none");
                    formContext.getAttribute("atos_newfamilyname1").setRequiredLevel("none");
                    formContext.getAttribute("atos_newtitulartype").setRequiredLevel("none");
                    formContext.getAttribute("atos_newregularaddress").setRequiredLevel("none");
                    formContext.getAttribute("atos_newtelephone1").setRequiredLevel("none");
                    formContext.getAttribute("atos_newprovinceowner").setRequiredLevel("none");
                    formContext.getAttribute("atos_newcityowner").setRequiredLevel("none");
                    formContext.getAttribute("atos_newzipcodeowner").setRequiredLevel("none");
                    formContext.getAttribute("atos_newstreettypeowner").setRequiredLevel("none");
                    formContext.getAttribute("atos_newstreetowner").setRequiredLevel("none");
                    formContext.getAttribute("atos_newstreetnumberowner").setRequiredLevel("none");
                    formContext.getAttribute("atos_newnationality").setRequiredLevel("none");
                    formContext.getAttribute("atos_newdocumenttype").setRequiredLevel("none");
                    formContext.getAttribute("atos_newdocumentnum").setRequiredLevel("none");
                }

            }

        }
        else {
            // cambiar todos a none
            formContext.getAttribute("atos_nationality").setRequiredLevel("none");
            formContext.getAttribute("atos_documenttype").setRequiredLevel("none");
            formContext.getAttribute("atos_documentnum").setRequiredLevel("none");
            formContext.getAttribute("atos_newreqqd").setRequiredLevel("none");
            formContext.getAttribute("atos_surrogacy").setRequiredLevel("none");
            formContext.getAttribute("atos_newfirstname").setRequiredLevel("none");
            formContext.getAttribute("atos_newfamilyname1").setRequiredLevel("none");
            formContext.getAttribute("atos_newtitulartype").setRequiredLevel("none");
            formContext.getAttribute("atos_newregularaddress").setRequiredLevel("none");
            formContext.getAttribute("atos_newtelephone1").setRequiredLevel("none");
            formContext.getAttribute("atos_newprovinceowner").setRequiredLevel("none");
            formContext.getAttribute("atos_newcityowner").setRequiredLevel("none");
            formContext.getAttribute("atos_newzipcodeowner").setRequiredLevel("none");
            formContext.getAttribute("atos_newstreettypeowner").setRequiredLevel("none");
            formContext.getAttribute("atos_newstreetowner").setRequiredLevel("none");
            formContext.getAttribute("atos_newstreetnumberowner").setRequiredLevel("none");
            formContext.getAttribute("atos_newnationality").setRequiredLevel("none");
            formContext.getAttribute("atos_newdocumenttype").setRequiredLevel("none");
            formContext.getAttribute("atos_newdocumentnum").setRequiredLevel("none");
        }
    }
}

async function cambioDatosNuevoPS() {

    var colPaso = ["atos_codigo"];
    var colProceso = ["atos_codigo"];

    var pasoatr = null;
    var procesoatr = null;
    var pasoatrValue = null;
    var procesoatrValue = null;

  if (formContext.data.entity.attributes.get("atos_messagetype").getValue() != null &&
		formContext.data.entity.attributes.get("atos_messagetype").getValue()[0] != null) {
  
        await Xrm.WebApi.retrieveRecord("atos_tablasatrgas", formContext.data.entity.attributes.get("atos_messagetype").getValue()[0].id,"?$select=atos_codigo").then(
             function success(record) {
                 pasoatr = record;
                 pasoatrValue = pasoatr == null ? null : pasoatr.atos_codigo;

             }, function (error) {
                 Xrm.Navigation.openErrorDialog({ detail: error.message });
             });
 
    }
    if (formContext.data.entity.attributes.get("atos_processcode").getValue() != null &&
		formContext.data.entity.attributes.get("atos_processcode").getValue()[0] != null) {

        await Xrm.WebApi.retrieveRecord("atos_tablasatrgas", formContext.data.entity.attributes.get("atos_processcode").getValue()[0].id,"?$select=atos_codigo").then(
            function success(record) {
                procesoatr = record;
                procesoatrValue = procesoatr == null ? null : pasoatr.atos_codigo;

            }, function (error) {
                Xrm.Navigation.openErrorDialog({ detail: error.message });
            });

    }

    var total = pasoatrValue + procesoatrValue;

    if (total == "A138") {

        //segun tipo de persona
        if (formContext.data.entity.attributes.get("atos_titulartype").getValue() != null &&
		    formContext.data.entity.attributes.get("atos_titulartype").getValue()[0] != null) {
            var titularType = formContext.data.entity.attributes.get("atos_titulartype").getValue()[0].name.split(":")[0];
            if (titularType == "F") {
                formContext.getAttribute("atos_firstname").setRequiredLevel("required");
                formContext.getAttribute("atos_regularaddress").setRequiredLevel("required");
            }
            else {
                formContext.getAttribute("atos_firstname").setRequiredLevel("none");
                formContext.getAttribute("atos_regularaddress").setRequiredLevel("none");
            }
        }
        //segun tipo de peaje 		
        if (formContext.data.entity.attributes.get("atos_tolltype").getValue() != null &&
	        formContext.data.entity.attributes.get("atos_tolltype").getValue()[0] != null) {
            var toolType = formContext.data.entity.attributes.get("atos_tolltype").getValue()[0].name.split(":")[0];
            if (toolType == "3.5" || toolType == "M1" || toolType == "M2") {
                formContext.getAttribute("atos_reqqd").setRequiredLevel("required");
                formContext.getAttribute("atos_reqqh").setRequiredLevel("required");
                formContext.getAttribute("atos_reqestimatedqa").setRequiredLevel("required");
                formContext.getAttribute("atos_reqoutgoingpressure").setRequiredLevel("required");
            }
            else {
                formContext.getAttribute("atos_reqqd").setRequiredLevel("none");
                formContext.getAttribute("atos_reqqh").setRequiredLevel("none");
                formContext.getAttribute("atos_reqestimatedqa").setRequiredLevel("none");
                formContext.getAttribute("atos_reqoutgoingpressure").setRequiredLevel("none");
            }
        }

        // segun Indicador de transformaciÃ³n de aparatos
        if (formContext.data.entity.attributes.get("atos_aptransind").getValue() != null) {
            var indTrans = formContext.data.entity.attributes.get("atos_aptransind").getValue();
            if (indTrans == true) {
                formContext.getAttribute("atos_aptransnumber").setRequiredLevel("required");

            }
            else {
                formContext.getAttribute("atos_aptransnumber").setRequiredLevel("none");
            }
        }
    }
}


async function rellenarDireccion() {
    if (formContext.data.entity.getId() == "" &&
	formContext.data.entity.attributes.get("atos_instalaciongasid").getValue() != null &&
	formContext.data.entity.attributes.get("atos_instalaciongasid").getValue()[0] != null) {

        await Xrm.WebApi.retrieveRecord("atos_instalaciongas", formContext.data.entity.attributes.get("atos_instalaciongasid").getValue()[0].id,"?$select=atos_instalacioncodigopostalid,atos_instalaciontipodeviaid," +
        + "atos_instalaciondireccion,atos_instalacionnumero,atos_instalacionportal,atos_instalacionpuerta,atos_instalacionescalera,atos_instalacionpiso").then(
            function success(record) {
                instalacionGas = record;

            }, function (error) {
                Xrm.Navigation.openErrorDialog({ detail: error.message });
            });

    

        var codigoPostal = instalacionGas == "" || instalacionGas.atos_instalacioncodigopostalid == null ? null : instalacionGas.atos_instalacioncodigopostalid.atos_name;

        var tipoVia = instalacionGas == "" || instalacionGas.atos_instalaciontipodeviaid == null ? null : instalacionGas.atos_instalaciontipodeviaid.atos_name;
        var via = instalacionGas == "" || instalacionGas.atos_instalaciondireccion == null ? null : instalacionGas.atos_instalaciondireccion;
        var numeroVia = instalacionGas == "" || instalacionGas.atos_instalacionnumero == null ? null : instalacionGas.atos_instalacionnumero;
        var portal = instalacionGas == "" || instalacionGas.atos_instalacionportal == null ? null : instalacionGas.atos_instalacionportal;
        var puerta = instalacionGas == "" || instalacionGas.atos_instalacionpuerta == null ? null : instalacionGas.atos_instalacionpuerta;
        var escalera = instalacionGas == "" || instalacionGas.atos_instalacionescalera == null ? null : instalacionGas.atos_instalacionescalera;
        var piso = instalacionGas == "" || instalacionGas.atos_instalacionpiso == null ? null : instalacionGas.atos_instalacionpiso;

        if (tipoVia != null) {
            var fetchXml =
            "<fetch mapping='logical'>" +
               "<entity name='atos_tablasatrgas'>" +
                  "<attribute name='atos_tablasatrgasid' />" +
                    "<attribute name='atos_name' /> " +
                  "<filter>" +
                    "<condition attribute='atos_descripcion' operator='eq' value='" + tipoVia.toUpperCase() + "' />" +
                  "</filter>" +
               "</entity>" +
            "</fetch>";

            //Tipo de via
            var registros = null;
            await Xrm.WebApi.retrieveMultipleRecords("atos_tablasatrgas", "?fetchXml=" + fetchXml).then(
                function success(records) {
                    if (records != null && records.entities != null && records.entities.length > 0) {
                      registros = records;
                }
                }, function (error) {
                    Xrm.Navigation.openErrorDialog({ detail: error.message });
                });

            if (registros.length > 0) {
                var valorRef = new Array();
                valorRef[0] = new Object();
                valorRef[0].id = registros[0].attributes["atos_tablasatrgasid"].id;
                valorRef[0].name = registros[0].attributes["atos_name"];
                valorRef[0].entityType = "atos_tablasatrgas";

                formContext.data.entity.attributes.get("atos_streettype").setValue(valorRef);
                formContext.getAttribute("atos_streettype").setSubmitMode("always");

                formContext.data.entity.attributes.get("atos_streettypeowner").setValue(valorRef);
                formContext.getAttribute("atos_streettypeowner").setSubmitMode("always");
            }
        }
        formContext.data.entity.attributes.get("atos_zipcode").setValue(codigoPostal);
        //formContext.data.entity.attributes.get("atos_streettype").setValue(tipoVia);
        formContext.data.entity.attributes.get("atos_street").setValue(via);
        formContext.data.entity.attributes.get("atos_streetnumber").setValue(numeroVia);
        formContext.data.entity.attributes.get("atos_portal").setValue(portal);
        formContext.data.entity.attributes.get("atos_staircase").setValue(escalera);
        formContext.data.entity.attributes.get("atos_floor").setValue(piso);
        formContext.data.entity.attributes.get("atos_door").setValue(puerta);

        formContext.data.entity.attributes.get("atos_zipcodeowner").setValue(codigoPostal);
        //formContext.data.entity.attributes.get("atos_streettype").setValue(tipoVia);
        formContext.data.entity.attributes.get("atos_streetowner").setValue(via);
        formContext.data.entity.attributes.get("atos_streetnumberowner").setValue(numeroVia);
        formContext.data.entity.attributes.get("atos_portalowner").setValue(portal);
        formContext.data.entity.attributes.get("atos_staircaseowner").setValue(escalera);
        formContext.data.entity.attributes.get("atos_floorowner").setValue(piso);
        formContext.data.entity.attributes.get("atos_doorowner").setValue(puerta);
    }
}




/**
// <summary>
// Oculta/muestra las pestaÃ±as correspondientes segÃºn el proceso de factura 
// y dependiendo del proceso hace obligatorio unos u otros campos 
// </summary>
// <remarks>
// </remarks>p
*/
async function solicitudatrgas_Load(executionContext) {

    formContext = executionContext.getFormContext();

    formContext.getControl("atos_name").setDisabled(true);
    formContext.getControl("atos_cups").setDisabled(true);
    formContext.getControl("atos_instalaciongasid").setDisabled(true);
    formContext.getControl("atos_dispatchingcode").setDisabled(true);
    formContext.data.entity.attributes.get("atos_dispatchingcode").setValue("GML");
}

async function tab_pasos(executionContext) {
debugger;
    formContext = executionContext.getFormContext();

    limpiaMensajeError(formContext,"2", "");
    ocultarPestanas();



    var pasoatr = null;
    var procesoatr = null;

    if (formContext.data.entity.attributes.get("atos_messagetype").getValue() != null &&
    formContext.data.entity.attributes.get("atos_messagetype").getValue()[0] != null) {

    await Xrm.WebApi.retrieveRecord("atos_tablasatrgas", formContext.data.entity.attributes.get("atos_messagetype").getValue()[0].id,"?$select=atos_codigo").then(
         function success(record) {
             pasoatr = record;
             pasoatrValue = pasoatr == null ? null : pasoatr.atos_codigo;

         }, function (error) {
             Xrm.Navigation.openErrorDialog({ detail: error.message });
         });

}
if (formContext.data.entity.attributes.get("atos_processcode").getValue() != null &&
    formContext.data.entity.attributes.get("atos_processcode").getValue()[0] != null) {

    await Xrm.WebApi.retrieveRecord("atos_tablasatrgas", formContext.data.entity.attributes.get("atos_processcode").getValue()[0].id,"?$select=atos_codigo").then(
        function success(record) {
            procesoatr = record;
            procesoatrValue = procesoatr == null ? null : pasoatr.atos_codigo;

        }, function (error) {
            Xrm.Navigation.openErrorDialog({ detail: error.message });
        });

}

    procesarPasos(pasoatr, procesoatr);
}
/**
// <summary>
// Procesar el paso necesario segÃºn el valor del parametro 'pasoatrvalue'
// </summary>
*/
async function procesarPasos(pasoatr, procesoatr) {

    formContext.getControl("atos_name").setDisabled(true);

    var pasoatrValue = pasoatr == null ? null : pasoatr.atos_codigo;
    var procesoatrValue = procesoatr == null ? null : procesoatr.atos_codigo;

    var total = pasoatrValue + procesoatrValue;

    switch (total) {
        //SALIDAS      
        case "A101": procesarPasoA101(); break;
        case "A102": procesarPasoA102(); break;
        case "A104": procesarPasoA104(); break;
        case "A105": procesarPasoA105(); break;
        case "A141": procesarPasoA141(); break;
        case "A138": procesarPasoA138(); break;


        // ENTRADAS  
        //	cambio de comercializadora	  
        case "A202": procesarPasoA202(); break;
        case "A2S02": procesarPasoA2S02(); break;
        case "A302": procesarPasoA302(); break;
        case "A3S02": procesarPasoA3S02(); break;
        case "A402": procesarPasoA402(); break;
        case "A4S02": procesarPasoA4S02(); break;
        //	cambio de comercializadora	con modificacion  
        case "A241": procesarPasoA241(); break;
        case "A2S41": procesarPasoA2S41(); break;
        case "A2541": procesarPasoA2541(); break;
        case "A341": procesarPasoA341(); break;
        case "A3S41": procesarPasoA3S41(); break;
        case "A441": procesarPasoA441(); break;
        case "A4S41": procesarPasoA4S41(); break;
        //	baja  
        case "A204": procesarPasoA204(); break;
        case "A2504": procesarPasoA2504(); break;
        case "A304": procesarPasoA304(); break;
        case "A404": procesarPasoA404(); break;
        //	modificacion  
        case "A205": procesarPasoA205(); break;
        case "A2505": procesarPasoA2505(); break;
        case "A305": procesarPasoA305(); break;
        case "A405": procesarPasoA405(); break;
        //	modificacion  
        case "A238": procesarPasoA238(); break;
        case "A2538": procesarPasoA2538(); break;
        case "A338": procesarPasoA338(); break;
        case "A438": procesarPasoA438(); break;
        default:
    }
}

async function ocultarPestanas() {
    formContext.ui.tabs.get('tab_A102').setVisible(false);
    formContext.ui.tabs.get('tab_A104').setVisible(false);
    formContext.ui.tabs.get('tab_A105').setVisible(false);
    formContext.ui.tabs.get('tab_A141').setVisible(false);
    formContext.ui.tabs.get('tab_A138').setVisible(false);

    formContext.ui.tabs.get('tab_A202').setVisible(false);
    formContext.ui.tabs.get('tab_A2S02').setVisible(false);
    formContext.ui.tabs.get('tab_A302').setVisible(false);
    formContext.ui.tabs.get('tab_A3S02').setVisible(false);
    formContext.ui.tabs.get('tab_A402').setVisible(false);
    formContext.ui.tabs.get('tab_A4S02').setVisible(false);
    formContext.ui.tabs.get('tab_A241').setVisible(false);
    formContext.ui.tabs.get('tab_A2S41').setVisible(false);
    formContext.ui.tabs.get('tab_A2541').setVisible(false);
    formContext.ui.tabs.get('tab_A341').setVisible(false);
    formContext.ui.tabs.get('tab_A3S41').setVisible(false);
    formContext.ui.tabs.get('tab_A441').setVisible(false);
    formContext.ui.tabs.get('tab_A4S41').setVisible(false);
    formContext.ui.tabs.get('tab_A204').setVisible(false);
    formContext.ui.tabs.get('tab_A2504').setVisible(false);
    formContext.ui.tabs.get('tab_A304').setVisible(false);
    formContext.ui.tabs.get('tab_A404').setVisible(false);
    formContext.ui.tabs.get('tab_A205').setVisible(false);
    formContext.ui.tabs.get('tab_A2505').setVisible(false);
    formContext.ui.tabs.get('tab_A305').setVisible(false);
    formContext.ui.tabs.get('tab_A405').setVisible(false);
    formContext.ui.tabs.get('tab_A238').setVisible(false);
    formContext.ui.tabs.get('tab_A2538').setVisible(false);
    formContext.ui.tabs.get('tab_A338').setVisible(false);
    formContext.ui.tabs.get('tab_A438').setVisible(false);
}

//************************************************************************
//** SALIDAS 
//************************************************************************
async function procesarPasoA101() {
    formContext.ui.tabs.get('tab_A101').setVisible(true);

    //  formContext.getAttribute("atos_codigoregistopedidorpeid").setRequiredLevel("required");
}

async function procesarPasoA102() {
    formContext.ui.tabs.get('tab_A102').setVisible(true);

    //formContext.getAttribute("atos_comreferencenum").setRequiredLevel("required");
    formContext.getAttribute("atos_reqdate").setRequiredLevel("required");
    formContext.getAttribute("atos_nationality").setRequiredLevel("required");
    formContext.getAttribute("atos_documenttype").setRequiredLevel("required");
    formContext.getAttribute("atos_documentnum").setRequiredLevel("required");
    formContext.getAttribute("atos_reqestimatedqa").setRequiredLevel("required");
    formContext.getAttribute("atos_modeffectdate").setRequiredLevel("required");
    formContext.getAttribute("atos_disconnectedserviceaccepted").setRequiredLevel("required");

    cambioFechaEfectiva();
}

async function procesarPasoA104() {
    formContext.ui.tabs.get('tab_A104').setVisible(true);

    //formContext.getAttribute("atos_comreferencenum").setRequiredLevel("required");
    formContext.getAttribute("atos_reqdate").setRequiredLevel("required");
    formContext.getAttribute("atos_nationality").setRequiredLevel("required");
    formContext.getAttribute("atos_documenttype").setRequiredLevel("required");
    formContext.getAttribute("atos_documentnum").setRequiredLevel("required");
    formContext.getAttribute("atos_modeffectdate").setRequiredLevel("required");
    formContext.getAttribute("atos_cancelreason").setRequiredLevel("required");
    formContext.getAttribute("atos_contactphonenumber").setRequiredLevel("required");

    cambioTipoDocumento();

}

async function procesarPasoA105() {
    formContext.ui.tabs.get('tab_A105').setVisible(true);

    //formContext.getAttribute("atos_comreferencenum").setRequiredLevel("required");
    formContext.getAttribute("atos_reqdate").setRequiredLevel("required");
    formContext.getAttribute("atos_nationality").setRequiredLevel("required");
    formContext.getAttribute("atos_documenttype").setRequiredLevel("required");
    formContext.getAttribute("atos_documentnum").setRequiredLevel("required");
    formContext.getAttribute("atos_modeffectdate").setRequiredLevel("required");

    formContext.getAttribute("atos_updatereason").setRequiredLevel("required");


    cambioDatosusuario();

}

async function procesarPasoA141() {
    formContext.ui.tabs.get('tab_A141').setVisible(true);

    //formContext.getAttribute("atos_comreferencenum").setRequiredLevel("required");
    formContext.getAttribute("atos_reqdate").setRequiredLevel("required");

    formContext.getAttribute("atos_modeffectdate").setRequiredLevel("required");

    formContext.getAttribute("atos_updatereason").setRequiredLevel("required");
    formContext.getAttribute("atos_disconnectedserviceaccepted").setRequiredLevel("required");

    cambioDatosUsuarioComercializadora();
}

async function procesarPasoA138() {
    formContext.ui.tabs.get('tab_A138').setVisible(true);

    // formContext.getAttribute("atos_comreferencenum").setRequiredLevel("required");
    formContext.getAttribute("atos_reqdate").setRequiredLevel("required");
    formContext.getAttribute("atos_nationality").setRequiredLevel("required");
    formContext.getAttribute("atos_documenttype").setRequiredLevel("required");
    formContext.getAttribute("atos_documentnum").setRequiredLevel("required");
    formContext.getAttribute("atos_familyname1").setRequiredLevel("required");
    formContext.getAttribute("atos_titulartype").setRequiredLevel("required");
    formContext.getAttribute("atos_province").setRequiredLevel("required");
    formContext.getAttribute("atos_city").setRequiredLevel("required");
    formContext.getAttribute("atos_provinceowner").setRequiredLevel("required");
    formContext.getAttribute("atos_cityowner").setRequiredLevel("required");
    formContext.getAttribute("atos_zipcodeowner").setRequiredLevel("required");
    formContext.getAttribute("atos_streettypeowner").setRequiredLevel("required");
    formContext.getAttribute("atos_streetowner").setRequiredLevel("required");
    formContext.getAttribute("atos_streetnumberowner").setRequiredLevel("required");
    formContext.getAttribute("atos_gasusetype").setRequiredLevel("required");
    formContext.getAttribute("atos_tolltype").setRequiredLevel("required");
    formContext.getAttribute("atos_counterproperty").setRequiredLevel("required");
    formContext.getAttribute("atos_aptransind").setRequiredLevel("required");
    formContext.getAttribute("atos_modeffectdate").setRequiredLevel("required");

    cambioDatosNuevoPS();
    rellenarDireccion();

}


//************************************************************************
//** ENTRADAS 
//************************************************************************
//***************** cambio de comercializadora ***************************/

async function procesarPaso(tab){
    formContext.ui.tabs.get(tab).setVisible(true);
}
async function procesarPasoA202() {
    formContext.ui.tabs.get('tab_A202').setVisible(true);
}

async function procesarPasoA2S02() {
    formContext.ui.tabs.get('tab_A2S02').setVisible(true);
}

async function procesarPasoA302() {
    formContext.ui.tabs.get('tab_A302').setVisible(true);
}

async function procesarPasoA3S02() {
    formContext.ui.tabs.get('tab_A3S02').setVisible(true);
}

async function procesarPasoA402() {
    formContext.ui.tabs.get('tab_A402').setVisible(true);
}

async function procesarPasoA4S02() {
    formContext.ui.tabs.get('tab_A4S02').setVisible(true);
}

//***************** cambio de comercializadora  con modificacion *************/

async function procesarPasoA241() {
    formContext.ui.tabs.get('tab_A241').setVisible(true);
}

async function procesarPasoA2S41() {
    formContext.ui.tabs.get('tab_A2S41').setVisible(true);
}

async function procesarPasoA2541() {
    formContext.ui.tabs.get('tab_A2541').setVisible(true);
}

async function procesarPasoA341() {
    formContext.ui.tabs.get('tab_A341').setVisible(true);
}

async function procesarPasoA3S41() {
    formContext.ui.tabs.get('tab_A3S41').setVisible(true);
}

async function procesarPasoA441() {
    formContext.ui.tabs.get('tab_A441').setVisible(true);
}

async function procesarPasoA4S41() {
    formContext.ui.tabs.get('tab_A4S41').setVisible(true);
}

//********************************** baja ****************************/
async function procesarPasoA204() {
    formContext.ui.tabs.get('tab_A204').setVisible(true);
}

async function procesarPasoA2504() {
    formContext.ui.tabs.get('tab_A2504').setVisible(true);
}

async function procesarPasoA304() {
    formContext.ui.tabs.get('tab_A304').setVisible(true);
}

async function procesarPasoA404() {
    formContext.ui.tabs.get('tab_A404').setVisible(true);
}

//********************************** modificacion ****************************/
async function procesarPasoA205() {
    formContext.ui.tabs.get('tab_A205').setVisible(true);
}

async function procesarPasoA2505() {
    formContext.ui.tabs.get('tab_A2505').setVisible(true);
}

async function procesarPasoA305() {
    formContext.ui.tabs.get('tab_A305').setVisible(true);
}

async function procesarPasoA405() {
    formContext.ui.tabs.get('tab_A405').setVisible(true);
}
//********************************** alta ****************************/
async function procesarPasoA238() {
    formContext.ui.tabs.get('tab_A238').setVisible(true);
}

async function procesarPasoA2538() {
    formContext.ui.tabs.get('tab_A2538').setVisible(true);
}

async function procesarPasoA338() {
    formContext.ui.tabs.get('tab_A338').setVisible(true);
}

async function procesarPasoA438() {
    formContext.ui.tabs.get('tab_A438').setVisible(true);
}

//************************************************************************
//** GUARDADO 
//************************************************************************

async function solicitudatrgas_OnSave(executionContext) {

    formContext = executionContext.getFormContext();
    var serverUrl = formContext.context.getClientUrl();
    var continuar = true;
    limpiaMensajeError(formContext,"2", "");

    if (formContext.data.entity.attributes.get("atos_messagetype").getValue() != null &&
		formContext.data.entity.attributes.get("atos_messagetype").getValue()[0] != null) {
  
        await Xrm.WebApi.retrieveRecord("atos_tablasatrgas", formContext.data.entity.attributes.get("atos_messagetype").getValue()[0].id,"?$select=atos_codigo").then(
             function success(record) {
                 pasoatr = record;
                 pasoatrValue = pasoatr == null ? null : pasoatr.atos_codigo;

             }, function (error) {
                 Xrm.Navigation.openErrorDialog({ detail: error.message });
             });
 
    }
    if (formContext.data.entity.attributes.get("atos_processcode").getValue() != null &&
		formContext.data.entity.attributes.get("atos_processcode").getValue()[0] != null) {

        await Xrm.WebApi.retrieveRecord("atos_tablasatrgas", formContext.data.entity.attributes.get("atos_processcode").getValue()[0].id,"?$select=atos_codigo").then(
            function success(record) {
                procesoatr = record;
                procesoatrValue = procesoatr == null ? null : pasoatr.atos_codigo;

            }, function (error) {
                Xrm.Navigation.openErrorDialog({ detail: error.message });
            });

    }

    var total = pasoatrValue + procesoatrValue;


    switch (total) {
        //SALIDAS      
        case "A101": continuar = validarPasoA101(); break;
        case "A102": continuar = validarPasoA102(); break;
        case "A104": continuar = validarPasoA104(); break;
        case "A105": continuar = validarPasoA105(); break;
        case "A141": continuar = validarPasoA141(); break;
        case "A138": continuar = validarPasoA138(); break;


        default:



            // si  dio algun fallo no continua
            if (continuar == false) {
                cancelarGuardado(obj);
            }

    }
    // si  dio algun fallo no continua
    if (continuar == false) {
        cancelarGuardado(obj);
    }
}





//************************************************************************
//** VALIDACIONES 
//************************************************************************
async function validarPasoA101() {
    var continuar = true;
    continuar = validarCabecera();
    return continuar
}

async function validarPasoA102() {
    var continuar = true;
    continuar = validarCabecera();
    if (!continuar) {
        return false;
    }
    continuar = validarDocumento("atos_documenttype", "atos_documentnum");
    return continuar;
}

async function validarPasoA104() {
    var continuar = true;
    continuar = validarCabecera();
    if (!continuar) {
        return false;
    }
    continuar = validarDocumento("atos_documenttype", "atos_documentnum");
    return continuar;
}

async function validarPasoA105() {

    var continuar = true;

    continuar = validarCabecera();
    if (!continuar) {
        return false;
    }
    continuar = validarDocumento("atos_documenttype", "atos_documentnum");
    if (!continuar) {
        return false;
    }

    continuar = validarDocumento("atos_newdocumenttype", "atos_newdocumentnum");
    if (!continuar) {
        return false;
    }

    continuar = validarCamposOpcionalesA105();

    return continuar;

}

async function validarPasoA141() {
    var continuar = true;
    continuar = validarCabecera();
    if (!continuar) {
        return false;
    }

    if (formContext.data.entity.attributes.get("atos_documenttype").getValue() != null &&
        formContext.data.entity.attributes.get("atos_documentnum").getValue() != null) {
        continuar = validarDocumento("atos_documenttype", "atos_documentnum");
    }
    if (!continuar) {
        return false;
    }

    if (formContext.data.entity.attributes.get("atos_newdocumenttype").getValue() != null &&
        formContext.data.entity.attributes.get("atos_newdocumentnum").getValue() != null) {
        continuar = validarDocumento("atos_newdocumenttype", "atos_newdocumentnum");
    }

    continuar = validarCamposOpcionalesA141();

    return continuar

}

async function validarPasoA138() {
    var continuar = true;
    continuar = validarCabecera();
    if (!continuar) {
        return false;
    }

    // validamos que se ha introducido un telefono
    if (formContext.data.entity.attributes.get("atos_telephone1").getValue() == null &&
	  formContext.data.entity.attributes.get("atos_telephone2").getValue() == null) {
        mensajeError(formContext,"El obligatorio meter un tÃ©lefono fijo o movil para el titular", "ERROR", "2", "atos_telephone1");
        return false;
    }

    return continuar

}


//************************************************************************
//** AUXILIARES 
//************************************************************************




async function validarCabecera() {
    return true;
    //NÂº Referencia Solicitud
    if (!IsValidNumReg()) {
        // mensajeError(formContext,"El valor de NÂº Referencia Solicitud tiene que tener exactamente 12 caracteres nÃºmericos", "ERROR", "2", "atos_comreferencenum");
        return true;
    }

    // Cups
    if (!ValidarCUPS(formContext.data.entity.attributes.get("atos_cups").getValue(), "20")) {
        mensajeError(formContext,"El valor de CUPS en la cabecera tiene que tener 20 caracteres y ser correcto", "ERROR", "2", "atos_cups");
        return false;
    }
    return true;
}

async function validarCamposOpcionalesA105(updateReason) {
    var arrayPermitidos = ["10", "17", "18", "19", "20", "21", "22", "23"];
    var updateReason = formContext.data.entity.attributes.get("atos_updatereason").getValue()[0].name.split(":")[0];

    if (arrayPermitidos.indexOf(updateReason) == -1) {
        if (formContext.data.entity.attributes.get("atos_newfamilyname2").getValue() != null) {
            mensajeError(formContext,"El campo 'Nuevo Apellido 2' debe estar vacio", "ERROR", "2", "atos_cups");
            return false;
        }
        if (formContext.data.entity.attributes.get("atos_newfax").getValue() != null) {
            mensajeError(formContext,"El campo 'Nuevo Fax' debe estar vacio", "ERROR", "2", "atos_cups");
            return false;
        }
        if (formContext.data.entity.attributes.get("atos_newemail").getValue() != null) {
            mensajeError(formContext,"El campo 'Nuevo E-mail' debe estar vacio", "ERROR", "2", "atos_cups");
            return false;
        }

        if (formContext.data.entity.attributes.get("atos_newportalowner").getValue() != null) {
            mensajeError(formContext,"El campo 'Nuevo Portal del Titular' debe estar vacio", "ERROR", "2", "atos_cups");
            return false;
        }
        if (formContext.data.entity.attributes.get("atos_newstaircaseowner").getValue() != null) {
            mensajeError(formContext,"El campo 'Nueva Escalera del Titular' debe estar vacio", "ERROR", "2", "atos_cups");
            return false;
        }
        if (formContext.data.entity.attributes.get("atos_newfloorowner").getValue() != null) {
            mensajeError(formContext,"El campo 'Nueva Planta del Titular' debe estar vacio", "ERROR", "2", "atos_cups");
            return false;
        }
        if (formContext.data.entity.attributes.get("atos_newdoorowner").getValue() != null) {
            mensajeError(formContext,"El campo 'Nueva Puerta del Titular' debe estar vacio", "ERROR", "2", "atos_cups");
            return false;
        }
    }
    return true;
}

async function validarCamposOpcionalesA141(updateReason) {
    var arrayPermitidos = ["01", "03"];
    var updateReason = formContext.data.entity.attributes.get("atos_updatereason").getValue()[0].name.split(":")[0];

    if (arrayPermitidos.indexOf(updateReason) == -1) {
        if (formContext.data.entity.attributes.get("atos_newfamilyname2").getValue() != null) {
            mensajeError(formContext,"El campo 'Nuevo Apellido 2' debe estar vacio", "ERROR", "2", "atos_cups");
            return false;
        }
        if (formContext.data.entity.attributes.get("atos_newtelephone2").getValue() != null) {
            mensajeError(formContext,"El campo 'Nuevo TelÃ©fono 2' debe estar vacio", "ERROR", "2", "atos_cups");
            return false;
        }
        if (formContext.data.entity.attributes.get("atos_newemail").getValue() != null) {
            mensajeError(formContext,"El campo 'Nuevo Correo ElectrÃ³nico' debe estar vacio", "ERROR", "2", "atos_cups");
            return false;
        }
        if (formContext.data.entity.attributes.get("atos_newlanguage").getValue() != null) {
            mensajeError(formContext,"El campo 'Nuevo Idioma' debe estar vacio", "ERROR", "2", "atos_cups");
            return false;
        }
        if (formContext.data.entity.attributes.get("atos_newportalowner").getValue() != null) {
            mensajeError(formContext,"El campo 'Nuevo Portal' debe estar vacio", "ERROR", "2", "atos_cups");
            return false;
        }
        if (formContext.data.entity.attributes.get("atos_newstaircaseowner").getValue() != null) {
            mensajeError(formContext,"El campo 'Nueva Escalera' debe estar vacio", "ERROR", "2", "atos_cups");
            return false;
        }
        if (formContext.data.entity.attributes.get("atos_newfloorowner").getValue() != null) {
            mensajeError(formContext,"El campo 'Nueva Planta' debe estar vacio", "ERROR", "2", "atos_cups");
            return false;
        }
        if (formContext.data.entity.attributes.get("atos_newdoorowner").getValue() != null) {
            mensajeError(formContext,"El campo 'Nueva Puerta' debe estar vacio", "ERROR", "2", "atos_cups");
            return false;
        }
    }
    return true;
}
async function validarDocumento(campoTipo, campoDocumento) {
    if (formContext.data.entity.attributes.get(campoTipo).getValue() == null)
        return true;


    var tipo = null
    //"atos_documenttype", "atos_documentnum"
    var numDoc = formContext.data.entity.attributes.get(campoDocumento).getValue();

    //atos_documenttype
    tipo = formContext.data.entity.attributes.get(campoTipo).getValue()[0].name.split(':')[0];


    if (tipo == "01") //NIF y CIF
    {
        if (!ValidaCIF(numDoc)) {
			 if (!ValidaNIF(numDoc)) {
				mensajeError(formContext,"El valor del NIF/CIF del titular tiene que tener el formato correcto", "ERROR", "2", campoDocumento);
				return false;
			 }
        }
    }


    if (tipo == "04") //NIE 
    {
        if (!valida_NIE(numDoc)) {
            mensajeError(formContext,"El valor de CIF del titular tiene que tener el formato correcto", "ERROR", "2", campoDocumento);
            return false;
        }

    }

    return true;
}





function IsNumeric(ObjVal) {
    return /^\d+$/.test(ObjVal);
}

function IsValidMail(mail) {
    // Patron para el correo
    var patron = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,4})+$/;
    if (mail.search(patron) == 0) {
        //Mail correcto
        return true;
    }
    //Mail incorrecto
    return false;
}


function IsValidNumReg() {

    if (formContext.data.entity.attributes.get("atos_comreferencenum").getValue() != null) {
        var numReg = formContext.data.entity.attributes.get("atos_comreferencenum").getValue();
        var patron = /\d{12}/;
        if (numReg.search(patron) == 0) {
            //Mail correcto
            return true;
        }
    }
    return false;
}

// <summary>
// Se encarga de anular el guardado de una reclamacion
// </summary>
async function cancelarGuardado(obj) {
    if (obj.getEventArgs() != null)
        obj.getEventArgs().preventDefault();
}


function vaciarCampo(campo) {
    asignarValorCampo(campo, "");
}
function asignarValorCampo(campo, valor) {
    if (formContext.data.entity.attributes.get(campo).getValue() != null)
        formContext.data.entity.attributes.get(campo).setValue(valor);
}

function deshabilitarCampo(campo, habilitar) {
    if (formContext.getControl(campo) != null)
        formContext.getControl(campo).setDisabled(habilitar);
}
