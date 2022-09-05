function vaciaCustomerid() {
    if (Xrm.Page.data.entity.getId() == null || Xrm.Page.data.entity.getId() == "") {
        if (Xrm.Page.data.entity.attributes.get("customerid").getValue() != null) {
            Xrm.Page.data.entity.attributes.get("customerid").setValue(null);
            Xrm.Page.getAttribute("customerid").setSubmitMode("always");
        }
    }
}

function datosRS(clienteid) {
    var cols = ["name", "atos_cuentanegociadoraid", "primarycontactid","atos_agentecomercialid","atos_consultoraid"];

    var regRS = XrmServiceToolkit.Soap.Retrieve("account", clienteid, cols);
    return regRS;
}


function datosCN(cuentanegociadoraid) {
    var cols = ["atos_name", "atos_contactocomercialid","atos_agentecomercialid","atos_consultoraid"];

    var regRS = XrmServiceToolkit.Soap.Retrieve("atos_cuentanegociadora", cuentanegociadoraid, cols);
    return regRS;
}

function datosContacto(contactoid) {
    var cols = ["fullname"];
    var regRS = XrmServiceToolkit.Soap.Retrieve("contact", contactoid, cols);
    return regRS;

}

function datosCorreo(correoid) {
    var cols = ["from"];
    var regRS = XrmServiceToolkit.Soap.Retrieve("email", correoid, cols);
    return regRS;

}



function datosAgenteComercial(agentecomercialid) {
    var cols = ["atos_name"];
    var regRS = XrmServiceToolkit.Soap.Retrieve("atos_agentecomercial", agentecomercialid, cols);
    return regRS;

}

function datosConsultora(consultoraid) {
    var cols = ["atos_name"];
    var regRS = XrmServiceToolkit.Soap.Retrieve("atos_consultora", consultoraid, cols);
    return regRS;

}


function datosInst(instalacionid) {
    var cols = ["atos_razonsocialid", "atos_contactoinstalacionid","atos_agentecomercialid","atos_consultoraid"];

    var regIns = XrmServiceToolkit.Soap.Retrieve("atos_instalacion", instalacionid, cols);
    return regIns;
}


function datosDist(distribuidoraid) {
    var cols = ["atos_contactogeneralid"];

    var regDis = XrmServiceToolkit.Soap.Retrieve("atos_distribuidora", distribuidoraid, cols);
    return regDis;
}

function cambiaContacto(contactoid) {
    if (Xrm.Page.data.entity.attributes.get("customerid").getValue() == null && contactoid != null) {
        var regCont = datosContacto(contactoid);
        if (regCont.attributes["fullname"] != null) {
            Xrm.Page.data.entity.attributes.get("customerid").setValue(construyeLookup("contact", contactoid, regCont.attributes["fullname"].value));
            Xrm.Page.getAttribute("customerid").setSubmitMode("always");
        }
    }
}
function cambiaAgenteComercial(agentecomercialid) {
    if (Xrm.Page.data.entity.attributes.get("atos_agentecomercialid").getValue() == null && agentecomercialid != null) {
        var regAgent = datosAgenteComercial(agentecomercialid);
        if (regAgent.attributes["atos_name"] != null) {
            Xrm.Page.data.entity.attributes.get("atos_agentecomercialid").setValue(construyeLookup("atos_agentecomercial", agentecomercialid, regAgent.attributes["atos_name"].value));
            Xrm.Page.getAttribute("atos_agentecomercialid").setSubmitMode("always");
        }
    }
}

function cambiaConsultora(consultoraid) {
    if (Xrm.Page.data.entity.attributes.get("atos_consultoraid").getValue() == null && consultoraid != null) {
        var regConsultora = datosConsultora(consultoraid);
        if (regConsultora.attributes["atos_name"] != null) {
            Xrm.Page.data.entity.attributes.get("atos_consultoraid").setValue(construyeLookup("atos_consultora", consultoraid, regConsultora.attributes["atos_name"].value));
            Xrm.Page.getAttribute("atos_consultoraid").setSubmitMode("always");
        }
    }
}


function cambiaCN(cuentanegociadoraid) {
    var regCN = datosCN(cuentanegociadoraid);
    if (regCN.attributes["atos_name"] != null) {
        Xrm.Page.data.entity.attributes.get("atos_cuentanegociadoraid").setValue(construyeLookup("atos_cuentanegociadora", cuentanegociadoraid, regCN.attributes["atos_name"].value));
        Xrm.Page.getAttribute("atos_cuentanegociadoraid").setSubmitMode("always");
    }

    if (Xrm.Page.data.entity.attributes.get("customerid").getValue() == null && regCN.attributes["atos_contactocomercialid"] != null)
        cambiaContacto(regCN.attributes["atos_contactocomercialid"].id);
	
    if (regCN.attributes["atos_agentecomercialid"] != null)
        cambiaAgenteComercial(regCN.attributes["atos_agentecomercialid"].id);
	else 
	    Xrm.Page.data.entity.attributes.get("atos_agentecomercialid").setValue("");
	
    if (regCN.attributes["atos_consultoraid"] != null)
        cambiaConsultora(regCN.attributes["atos_consultoraid"].id);
	else 
	    Xrm.Page.data.entity.attributes.get("atos_consultoraid").setValue("");
}

function cambiadaCN() {
    if (Xrm.Page.data.entity.attributes.get("atos_cuentanegociadoraid").getValue() != null) {
        cambiaCN(Xrm.Page.data.entity.attributes.get("atos_cuentanegociadoraid").getValue()[0].id);
    }
}

function cambiaRS(razonsocialid) {
    var regRS = datosRS(razonsocialid);
    if (regRS.attributes["name"] != null) {
        Xrm.Page.data.entity.attributes.get("atos_razonsocialid").setValue(construyeLookup("account", razonsocialid, regRS.attributes["name"].value));
        Xrm.Page.getAttribute("atos_razonsocialid").setSubmitMode("always");
    }
    if (Xrm.Page.data.entity.attributes.get("customerid").getValue() == null && regRS.attributes["primarycontactid"] != null)
        cambiaContacto(regRS.attributes["primarycontactid"].id);

    if (regRS.attributes["atos_cuentanegociadoraid"] != null) {
        cambiaCN(regRS.attributes["atos_cuentanegociadoraid"].id);
    }
	
	if (regRS.attributes["atos_agentecomercialid"] != null)
        cambiaAgenteComercial(regRS.attributes["atos_agentecomercialid"].id);
	else 
	    Xrm.Page.data.entity.attributes.get("atos_agentecomercialid").setValue("");
	
    if (regRS.attributes["atos_consultoraid"] != null)
        cambiaConsultora(regRS.attributes["atos_consultoraid"].id);	
	else 
	    Xrm.Page.data.entity.attributes.get("atos_consultoraid").setValue("");
}


function cambiadaRS() {
    if (Xrm.Page.data.entity.attributes.get("atos_razonsocialid").getValue() != null) {
        cambiaRS(Xrm.Page.data.entity.attributes.get("atos_razonsocialid").getValue()[0].id);
        Xrm.Page.getControl("atos_cuentanegociadoraid").setDisabled(true);
    }
    else {
        Xrm.Page.getControl("atos_cuentanegociadoraid").setDisabled(false);
    }
}


function cambiaInst(instalacionid) {
    var regIns = datosInst(instalacionid);

    if (Xrm.Page.data.entity.attributes.get("customerid").getValue() == null && regIns.attributes["atos_contactoinstalacionid"] != null)
        cambiaContacto(regIns.attributes["atos_contactoinstalacionid"].id);

    if (regIns.attributes["atos_razonsocialid"] != null)
        cambiaRS(regIns.attributes["atos_razonsocialid"].id);
	
	if (regIns.attributes["atos_agentecomercialid"] != null)
        cambiaAgenteComercial(regIns.attributes["atos_agentecomercialid"].id);
	else 
	    Xrm.Page.data.entity.attributes.get("atos_agentecomercialid").setValue("");
	
    if (regIns.attributes["atos_consultoraid"] != null)
        cambiaConsultora(regIns.attributes["atos_consultoraid"].id);	
	else 
	    Xrm.Page.data.entity.attributes.get("atos_consultoraid").setValue("");
}


function cambiadaInst() {
    if (Xrm.Page.data.entity.attributes.get("atos_instalacionid").getValue() != null) {
        cambiaInst(Xrm.Page.data.entity.attributes.get("atos_instalacionid").getValue()[0].id);
        Xrm.Page.getControl("atos_cuentanegociadoraid").setDisabled(true);
        Xrm.Page.getControl("atos_razonsocialid").setDisabled(true);
    }
    else {
        Xrm.Page.getControl("atos_razonsocialid").setDisabled(false);
    }

}


function preFilterLookup() {
    Xrm.Page.getControl("customerid").addPreSearch(addLookupFilter);
}

function addLookupFilter() {

    document.getElementById("customerid_i").setAttribute("lookuptypenames", "contact:2:Contact");
    document.getElementById("customerid_i").setAttribute("lookuptypes", "2");
    document.getElementById("customerid_i").setAttribute("defaulttype", "2");
    document.getElementById("customerid_i").setAttribute("lookuptypeIcons", "/_imgs/ico_16_2.gif");




}

function cambiaDist(distribuidoraid) {
    var regDist = datosDist(distribuidoraid);

    if (regDist.attributes["atos_contactogeneralid"] != null)
        cambiaContacto(regDist.attributes["atos_contactogeneralid"].id);
}

function cambiadaDist() {
    if (Xrm.Page.data.entity.attributes.get("atos_distribuidoraid").getValue() != null) {
        if (Xrm.Page.data.entity.attributes.get("customerid").getValue() == null) {
            cambiaDist(Xrm.Page.data.entity.attributes.get("atos_distribuidoraid").getValue()[0].id);
        }
    }
}

function cargaInicial() {
    //	preFiltroLookup("atos_razonsocialid", "atos_cuentanegociadoraid", "atos_cuentanegociadoraid");
    //	preFiltroLookup("atos_instalacionid", "atos_razonsocialid", "atos_razonsocialid");

    if (Xrm.Page.data.entity.getId() == null || Xrm.Page.data.entity.getId() == "") {
        if (Xrm.Page.data.entity.attributes.get("atos_distribuidoraid").getValue() != null) {
            cambiadaDist();
        }
        else if (Xrm.Page.data.entity.attributes.get("atos_instalacionid").getValue() != null) {
            cambiaInst(Xrm.Page.data.entity.attributes.get("atos_instalacionid").getValue()[0].id);
            // disabled instalaciÃ³n, razon social y cuenta negociadora
            Xrm.Page.getControl("atos_cuentanegociadoraid").setDisabled(true);
            Xrm.Page.getControl("atos_razonsocialid").setDisabled(true);
            //Xrm.Page.getControl("atos_instalacionid").setDisabled(true);
        }
        else if (Xrm.Page.data.entity.attributes.get("atos_razonsocialid").getValue() != null) {
            cambiaRS(Xrm.Page.data.entity.attributes.get("atos_razonsocialid").getValue()[0].id);
            // disabled razon social y cuenta negociadora
            Xrm.Page.getControl("atos_cuentanegociadoraid").setDisabled(true);
            //Xrm.Page.getControl("atos_razonsocialid").setDisabled(true);
        }
        else if (Xrm.Page.data.entity.attributes.get("atos_cuentanegociadoraid").getValue() != null) {
            cambiaCN(Xrm.Page.data.entity.attributes.get("atos_cuentanegociadoraid").getValue()[0].id);
            // disabled cuenta negociadora
            //Xrm.Page.getControl("atos_cuentanegociadoraid").setDisabled(true);
        }
    }
	 // carga si se viene desde correo
	 if (Xrm.Page.data.entity.getId() == null || Xrm.Page.data.entity.getId() == "") {
        var xrmObject = Xrm.Page.context.getQueryStringParameters();

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
						
                        var serverUrl = Xrm.Page.context.getClientUrl();

                        head.load(serverUrl + "/WebResources/atos_json2.js", serverUrl + "/WebResources/atos_jquery.js", serverUrl + "/WebResources/atos_XrmServiceToolkit.js", function () {
                         cargaDesdeCorreo(llamanteid,titulo,descripcion,referentea,referenteaName,referenteaEntityType);
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
// Accede correocon el identificador recibido por parÃ¡metro<br/>
// y rellena los campos de la tarea a partir de los siguientes campos del correo:
// </remarks>
 */
function cargaDesdeCorreo(correoId,titulo,descripcion,referentea,referenteaName,referenteaEntityType) {
	
     var regEmail = datosCorreo(correoId);
        if (regEmail.attributes["from"] != null) {
			
			var contactoid = regEmail.attributes["from"].value[0].id;
			var name = regEmail.attributes["from"].value[0].name;
            Xrm.Page.data.entity.attributes.get("customerid").setValue(construyeLookup("contact", contactoid, name));
            Xrm.Page.getAttribute("customerid").setSubmitMode("always");
        }	

	var valorReferencia = new Array();
        valorReferencia[0] = new Object();
        valorReferencia[0].id = correoId;
        //valorReferencia[0].name = categoria.attributes["atos_intervenciondistribuidorid"].name;
        valorReferencia[0].entityType = "email";
		
        Xrm.Page.data.entity.attributes.get("atos_correoorigendelcasoid").setValue(valorReferencia);
		Xrm.Page.data.entity.attributes.get("title").setValue(titulo);
		Xrm.Page.data.entity.attributes.get("description").setValue(descripcion);
		
		
		// ponemos le referentea
		if (referentea.trim()!="")
		{
			// segun el tipo ponemos  se rellean el campo correspondiente
			switch(referenteaEntityType) {
			case "account":
				//account-Razon Social
				Xrm.Page.data.entity.attributes.get("atos_razonsocialid").setValue(construyeLookup(referenteaEntityType, referentea, referenteaName));
				cambiaRS(referentea);
				break;
			case "atos_instalacion":
				//atos_instalacion-Instalacion
				Xrm.Page.data.entity.attributes.get("atos_instalacionid").setValue(construyeLookup(referenteaEntityType, referentea, referenteaName));
				cambiaInst(referentea);
				break;
			case "atos_cuentanegociadora":
				//atos-cuentanegociadora-CN
				Xrm.Page.data.entity.attributes.get("atos_cuentanegociadoraid").setValue(construyeLookup(referenteaEntityType, referentea, referenteaName));
				cambiaCN(referentea);
				break;
			case "atos_distribuidora":
				// atos_distribuidora-distribuidora
				Xrm.Page.data.entity.attributes.get("atos_distribuidoraid").setValue(construyeLookup(referenteaEntityType, referentea, referenteaName));
				cambiaDist(referentea);
				break;
			}
		}
}


function borrarFechaCambioEstado() {
    Xrm.Page.getAttribute('atos_fechadecambioestado').setValue();
}

function subcategoriaChange() {
    //debugger;
	
	if (Xrm.Page.data.entity.attributes.get("atos_subcategoriaid").getValue()==null)
   {
       return;
   }
	
    var subcategoriaid = Xrm.Page.data.entity.attributes.get("atos_subcategoriaid").getValue()[0].id;
    var categoria = datosSubcategoria(subcategoriaid);
    // campo interno/externo 
    if (categoria.attributes["atos_internooexterno"] != null) {
        Xrm.Page.data.entity.attributes.get("atos_internooexterno").setValue(categoria.attributes["atos_internooexterno"].value);
    }
    else {
        //Xrm.Page.getAttribute('atos_internooexterno').setValue(); 
    }

    //campo intervencion distribuidor
    if (categoria.attributes["atos_intervenciondistribuidorid"] != null) {
        var valorReferencia = new Array();
        valorReferencia[0] = new Object();
        valorReferencia[0].id = categoria.attributes["atos_intervenciondistribuidorid"].id;
        valorReferencia[0].name = categoria.attributes["atos_intervenciondistribuidorid"].name;
        valorReferencia[0].entityType = "atos_intervenciondeldistribuidor";

        Xrm.Page.data.entity.attributes.get("atos_intervenciondistribuidorid").setValue(valorReferencia);
    }
    else {
        //	Xrm.Page.getAttribute('atos_intervenciondistribuidorid').setValue(); 
    }
}


function datosSubcategoria(subcategoriaid) {
    var cols = ["atos_subcategoriasincidenciaid", "atos_internooexterno", "atos_intervenciondistribuidorid"];

    var regSub = XrmServiceToolkit.Soap.Retrieve("atos_subcategoriasincidencia", subcategoriaid, cols);
    return regSub;
}

