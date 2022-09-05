/*
 File="atos_cierre.js" 
 Copyright (c) Atos. All rights reserved.

 Fecha 		Codigo  Version Descripcion
 16/03/2021 223231  1.0.1.1 Cambi el FechXml con sentencia no-lock='true'

*/

function validarCierresxMwh()
{
	Xrm.Page.getControl("atos_energiacierre").clearNotification('0')
		Xrm.Page.getControl("atos_energiacierre").clearNotification('1')
	if (Xrm.Page.data.entity.attributes.get("atos_variable").getValue() != null)
	{
		//Sumo energÃ­a variable de todos los cierres del contrato con el mismo EMS
		//Recogemos todos los cierres del contrato	
		var contrato = Xrm.Page.data.entity.attributes.get("atos_contratoid").getValue()[0].id;
		var idCierre = Xrm.Page.data.entity.attributes.get("atos_name").getValue();
		//Recogemos Mwh de cierre
		var fetchXml =
/* 223231 +1*/ //"<fetch mapping='logical'>"+
			"<fetch mapping='logical' no-lock='true'>" +
				"<entity name='atos_cierre'>"+
					"<attribute name='atos_energiacierre'/>"+
					"<filter type='and'>"+
						"<condition attribute='statecode' value='0' operator='eq'/>"+
						"<condition attribute='atos_contratoid' operator='eq' value='"+contrato+"' />"+
						"<condition attribute='atos_variable' operator='not-null'/>"+
						"<condition attribute='atos_name' operator='ne' value='"+idCierre+"'/>"+
					"</filter>"+
					"<link-entity name='atos_pricingoutput' alias='a_pricingO' to='atos_variable' from='atos_pricingoutputid'>"+
						"<attribute name='atos_terminoems'/>"+
						"<filter type='and'>"+
							"<condition attribute='atos_terminoems' operator='not-null'/>"+
						"</filter>"+
					"</link-entity>"+
				"</entity>"+
			"</fetch>";

		var registroMwh= XrmServiceToolkit.Soap.Fetch(fetchXml);		
		var totalMwh =  Xrm.Page.data.entity.attributes.get("atos_energiacierre").getValue();
		for (var i = 0;i<registroMwh.length;i++)
		{
			if (registroMwh[i].attributes["atos_energiacierre"] != null)
			{
				totalMwh = totalMwh + registroMwh[i].attributes["atos_energiacierre"].value;
			}		
		}
		
		//Comprobamos si el cierre de la oferta tiene asignado un valor a cierre por Mwh
		//Si es asÃ­, no podremos exceder ese valor (Â¿Â¿Â¿Â¿Â¿ y si no no podremos exceder el contratado??????)
		var maxMwh
		//Comprobamos si el contrato tiene una oferta asociada (si no no hacemos esta validaciÃ³n) 
		var fetchXml =
/* 223231 +1*/ //"<fetch mapping='logical'>"+			
			"<fetch mapping='logical' no-lock='true'>" +
				"<entity name='atos_contrato'>"+	
					"<attribute name='atos_ofertaid'/>"+						
					"<filter>" +
						   "<condition attribute='atos_contratoid' operator='eq' value='"+contrato+"' />" +
					"</filter>" +							
				"</entity>"+
			"</fetch>";
		var oferta = XrmServiceToolkit.Soap.Fetch(fetchXml);
		if (oferta.length > 0)
		{	
			if (oferta[0].attributes["atos_ofertaid"] != null )		
			{
				var ofertaId = oferta[0].attributes["atos_ofertaid"].id;
				//Comprobamos si la oferta estÃ¡ activa
			var fetchXml =
/* 223231 +1*/ 		//"<fetch mapping='logical'>"+			
					"<fetch mapping='logical' no-lock='true'>" +			
								"<entity name='atos_oferta'>"+	
									"<attribute name='statecode'/>"+						
									"<filter>" +
										   "<condition attribute='atos_ofertaid' operator='eq' value='"+ofertaId+"' />" +
									"</filter>" +							
								"</entity>"+
							"</fetch>";
				var activa = XrmServiceToolkit.Soap.Fetch(fetchXml);
				if (activa[0].attributes["statecode"].value == '0')
				{	
					var terminoEMSCierreContrato;
					//Comprobamos si la variable estÃ¡ informada
					var pricingOutput = Xrm.Page.data.entity.attributes.get("atos_variable").getValue()[0].id;
					
						var fetchXml =
/* 223231 +1*/ 				//"<fetch mapping='logical'>"+			
							"<fetch mapping='logical' no-lock='true'>" +
								"<entity name='atos_pricingoutput'>"+	
									"<attribute name='atos_terminoems'/>"+						
									"<filter>" +
										   "<condition attribute='atos_pricingoutputid' operator='eq' value='"+pricingOutput+"' />" +
									"</filter>" +							
								"</entity>"+
							"</fetch>";
						var reg = XrmServiceToolkit.Soap.Fetch(fetchXml);
						if (reg.length > 0)	
						{
							terminoEMSCierreContrato = reg[0].attributes["atos_terminoems"].value
							//Listamos los cierres de la oferta
							fetchXml =
/* 223231 +1*/ 				    //"<fetch mapping='logical'>"+			
								"<fetch mapping='logical' no-lock='true'>" +
									"<entity name='atos_cierreoferta'>"+
										"<attribute name='atos_energiacierre'/>"+								
										"<filter type='and'>"+
											"<condition attribute='statecode' value='0' operator='eq'/>"+
											"<condition attribute='atos_ofertaid' value='"+ofertaId+"' operator='eq'/>"+
										"</filter>"+
										"<link-entity name='atos_oferta' alias='aa' to='atos_ofertaid' from='atos_ofertaid'>"+
											"<link-entity name='atos_pricingoutput' alias='ab' to='atos_ofertaid' from='atos_ofertaid'>"+
												"<filter type='and'>"+
													"<condition attribute='atos_terminoems' value='"+terminoEMSCierreContrato+"' operator='eq'/>"+
												"</filter>"+
											"</link-entity>"+
										"</link-entity>"+
									"</entity>"+
								"</fetch>";
							var regCierresOferta = XrmServiceToolkit.Soap.Fetch(fetchXml);

							if(regCierresOferta.length >0)
							{
	if  ( regCierresOferta[0].attributes["atos_energiacierre"] != null)
								maxMwh = regCierresOferta[0].attributes["atos_energiacierre"].value
							}
						
					}
				}
			}
		}

	if (maxMwh != null)
{
		if (totalMwh > maxMwh)
		{

			Xrm.Page.getControl("atos_energiacierre").setNotification("Excede el Mwh","0");
				   //Limpiamos el campo
				  Xrm.Page.data.entity.attributes.get("atos_energiacierre").setValue();
			  Xrm.Page.getAttribute("atos_energiacierre").setSubmitMode("always");
		}		
}
	}
	else
	{	
			Xrm.Page.getControl("atos_energiacierre").setNotification("Variable-Picing Output debe estar informada","1");
  Xrm.Page.data.entity.attributes.get("atos_energiacierre").setValue();
			  Xrm.Page.getAttribute("atos_energiacierre").setSubmitMode("always");

	}
}

function validarCierresXPorcentaje()
{
	Xrm.Page.getControl("atos_porcentajecierre").clearNotification('0')
		Xrm.Page.getControl("atos_porcentajecierre").clearNotification('1')
if (Xrm.Page.data.entity.attributes.get("atos_variable").getValue() != null)
	{
		//Sumo los porcentajes de todos los cierres del contrato con el mismo EMS
		//Recogemos todos los cierres del contrato	
		var contrato = Xrm.Page.data.entity.attributes.get("atos_contratoid").getValue()[0].id;
		var idCierre = Xrm.Page.data.entity.attributes.get("atos_name").getValue();
		//Recogemos el % de cierre
		var fetchXml =
/* 223231 +1*/ //"<fetch mapping='logical'>"+			
			"<fetch mapping='logical' no-lock='true'>" +			
				"<entity name='atos_cierre'>"+
					"<attribute name='atos_porcentajecierre'/>"+
					"<filter type='and'>"+
						"<condition attribute='statecode' value='0' operator='eq'/>"+
						"<condition attribute='atos_contratoid' operator='eq' value='"+contrato+"' />"+
						"<condition attribute='atos_variable' operator='not-null'/>"+
						"<condition attribute='atos_name' operator='ne' value='"+idCierre+"'/>"+
					"</filter>"+
					"<link-entity name='atos_pricingoutput' alias='a_pricingO' to='atos_variable' from='atos_pricingoutputid'>"+
						"<attribute name='atos_terminoems'/>"+
						"<filter type='and'>"+
							"<condition attribute='atos_terminoems' operator='not-null'/>"+
						"</filter>"+
					"</link-entity>"+
				"</entity>"+
			"</fetch>";

		var registrocierresPorcent= XrmServiceToolkit.Soap.Fetch(fetchXml);		
		var totalPorcentaje =  Xrm.Page.data.entity.attributes.get("atos_porcentajecierre").getValue();
		for (var i = 0;i<registrocierresPorcent.length;i++)
		{
			if (registrocierresPorcent[i].attributes["atos_porcentajecierre"] != null)
			{
				totalPorcentaje = totalPorcentaje + registrocierresPorcent[i].attributes["atos_porcentajecierre"].value;
			}		
		}
		
		//Comprobamos si el cierre de la oferta tiene asignado un valor a cierre por porcentaje
		//Si es asÃ­, no podremos exceder ese valor y si no no podremos exceder el 100%
		var maxPorcentaje = 1
		//Comprobamos si el contrato tiene una oferta asociada (si no no hacemos esta validaciÃ³n) 
		var fetchXml =
/* 223231 +1*/ //"<fetch mapping='logical'>"+			
			"<fetch mapping='logical' no-lock='true'>" +
				"<entity name='atos_contrato'>"+	
					"<attribute name='atos_ofertaid'/>"+						
					"<filter>" +
						   "<condition attribute='atos_contratoid' operator='eq' value='"+contrato+"' />" +
					"</filter>" +							
				"</entity>"+
			"</fetch>";
		var oferta = XrmServiceToolkit.Soap.Fetch(fetchXml);
		if (oferta.length > 0)
		{	
			if (oferta[0].attributes["atos_ofertaid"] != null )		
			{
				var ofertaId = oferta[0].attributes["atos_ofertaid"].id;
				//Comprobamos si la oferta estÃ¡ activa
			var fetchXml =
/* 223231 +1*/ 				//"<fetch mapping='logical'>"+			
							"<fetch mapping='logical' no-lock='true'>" +
								"<entity name='atos_oferta'>"+	
									"<attribute name='statecode'/>"+						
									"<filter>" +
										   "<condition attribute='atos_ofertaid' operator='eq' value='"+ofertaId+"' />" +
									"</filter>" +							
								"</entity>"+
							"</fetch>";
				var activa = XrmServiceToolkit.Soap.Fetch(fetchXml);
				if (activa[0].attributes["statecode"].value == '0')
				{	
					var terminoEMSCierreContrato;
					//Comprobamos si la variable estÃ¡ informada
					var pricingOutput = Xrm.Page.data.entity.attributes.get("atos_variable").getValue()[0].id;
					
						var fetchXml =
/* 223231 +1*/ 				//"<fetch mapping='logical'>"+			
							"<fetch mapping='logical' no-lock='true'>" +
								"<entity name='atos_pricingoutput'>"+	
									"<attribute name='atos_terminoems'/>"+						
									"<filter>" +
										   "<condition attribute='atos_pricingoutputid' operator='eq' value='"+pricingOutput+"' />" +
									"</filter>" +							
								"</entity>"+
							"</fetch>";
						var reg = XrmServiceToolkit.Soap.Fetch(fetchXml);
						if (reg.length > 0)	
						{
							terminoEMSCierreContrato = reg[0].attributes["atos_terminoems"].value
							//Listamos los cierres de la oferta
							fetchXml =
/* 223231 +1*/ 					//"<fetch mapping='logical'>"+			
								"<fetch mapping='logical' no-lock='true'>" +
									"<entity name='atos_cierreoferta'>"+
										"<attribute name='atos_porcentajecierre'/>"+								
										"<filter type='and'>"+
											"<condition attribute='statecode' value='0' operator='eq'/>"+
											"<condition attribute='atos_ofertaid' value='"+ofertaId+"' operator='eq'/>"+
										"</filter>"+
										"<link-entity name='atos_oferta' alias='aa' to='atos_ofertaid' from='atos_ofertaid'>"+
											"<link-entity name='atos_pricingoutput' alias='ab' to='atos_ofertaid' from='atos_ofertaid'>"+
												"<filter type='and'>"+
													"<condition attribute='atos_terminoems' value='"+terminoEMSCierreContrato+"' operator='eq'/>"+
												"</filter>"+
											"</link-entity>"+
										"</link-entity>"+
									"</entity>"+
								"</fetch>";
							var regCierresOferta = XrmServiceToolkit.Soap.Fetch(fetchXml);

							if(regCierresOferta.length >0)
							{
	if  ( regCierresOferta[0].attributes["atos_porcentajecierre"] != null)
								maxPorcentaje = regCierresOferta[0].attributes["atos_porcentajecierre"].value
							}
						
					}
				}
			}
		}

		if (totalPorcentaje > maxPorcentaje)
		{
			Xrm.Page.getControl("atos_porcentajecierre").setNotification("Excede el porcentaje","0");
				   //Limpiamos el campo
				  Xrm.Page.data.entity.attributes.get("atos_porcentajecierre").setValue();
			  Xrm.Page.getAttribute("atos_porcentajecierre").setSubmitMode("always");
		}		
	}
	else
	{
			Xrm.Page.getControl("atos_porcentajecierre").setNotification("Variable-Picing Output debe estar informada","1");
  Xrm.Page.data.entity.attributes.get("atos_porcentajecierre").setValue();
			  Xrm.Page.getAttribute("atos_porcentajecierre").setSubmitMode("always");

	}
}

function validarCierresXTerminoEMS()
{	
Xrm.Page.getControl("atos_variable").clearNotification('0')
//Limpiamos los campos dependientes
Xrm.Page.data.entity.attributes.get("atos_porcentajecierre").setValue();
								Xrm.Page.getAttribute("atos_porcentajecierre").setSubmitMode("always");
Xrm.Page.data.entity.attributes.get("atos_energiacierre").setValue();
								Xrm.Page.getAttribute("atos_energiacierre").setSubmitMode("always");

	//Recogemos el id del contrato asociado al cierre
	var contrato = Xrm.Page.data.entity.attributes.get("atos_contratoid").getValue()[0].id;
	//Comprobamos si el contrato tiene una oferta asociada (si no no hacemos esta validaciÃ³n) 
	var fetchXml =
/* 223231 +1*/	//"<fetch mapping='logical'>"+			
		"<fetch mapping='logical' no-lock='true'>" +
			"<entity name='atos_contrato'>"+	
				"<attribute name='atos_ofertaid'/>"+						
				"<filter>" +
					   "<condition attribute='atos_contratoid' operator='eq' value='"+contrato+"' />" +
				"</filter>" +							
			"</entity>"+
		"</fetch>";
	var oferta = XrmServiceToolkit.Soap.Fetch(fetchXml);
	//Recogemos el nÃºmero mÃ¡ximo de cierres permitido (si el contrato estÃ¡ asociado a una oferta) en funciÃ³n a la variable EMS
	var maxCierres;
	if (oferta.length > 0)
	{	
		if (oferta[0].attributes["atos_ofertaid"] != null )		
		{
			var ofertaId = oferta[0].attributes["atos_ofertaid"].id;
			//Comprobamos si la oferta estÃ¡ activa
		var fetchXml =
/* 223231 +1*/ 			//"<fetch mapping='logical'>"+			
						"<fetch mapping='logical' no-lock='true'>" +
							"<entity name='atos_oferta'>"+	
								"<attribute name='statecode'/>"+						
								"<filter>" +
									   "<condition attribute='atos_ofertaid' operator='eq' value='"+ofertaId+"' />" +
								"</filter>" +							
							"</entity>"+
						"</fetch>";
			var activa = XrmServiceToolkit.Soap.Fetch(fetchXml);
			if (activa[0].attributes["statecode"].value == '0')
			{			
				//Comenzamos la validaciÃ³n
				var terminoEMSCierreContrato;
				//Comprobamos si la variable estÃ¡ informada
				var pricingOutput = Xrm.Page.data.entity.attributes.get("atos_variable").getValue()[0].id;
				if (pricingOutput != null)
				{
					var fetchXml =
/* 223231 +1*/ 			//"<fetch mapping='logical'>"+			
						"<fetch mapping='logical' no-lock='true'>" +
							"<entity name='atos_pricingoutput'>"+	
								"<attribute name='atos_terminoems'/>"+						
								"<filter>" +
									   "<condition attribute='atos_pricingoutputid' operator='eq' value='"+pricingOutput+"' />" +
								"</filter>" +							
							"</entity>"+
						"</fetch>";
					var reg = XrmServiceToolkit.Soap.Fetch(fetchXml);
					if (reg.length > 0)	
					{
						terminoEMSCierreContrato = reg[0].attributes["atos_terminoems"].value
						//Listamos los cierres de la oferta
						fetchXml =
/* 223231 +1*/ 				//"<fetch mapping='logical'>"+			
							"<fetch mapping='logical' no-lock='true'>" +
								"<entity name='atos_cierreoferta'>"+
									"<attribute name='atos_numerocierrespermitido'/>"+								
									"<filter type='and'>"+
										"<condition attribute='statecode' value='0' operator='eq'/>"+
										"<condition attribute='atos_ofertaid' value='"+ofertaId+"' operator='eq'/>"+
									"</filter>"+
									"<link-entity name='atos_oferta' alias='aa' to='atos_ofertaid' from='atos_ofertaid'>"+
										"<link-entity name='atos_pricingoutput' alias='ab' to='atos_ofertaid' from='atos_ofertaid'>"+
											"<filter type='and'>"+
												"<condition attribute='atos_terminoems' value='"+terminoEMSCierreContrato+"' operator='eq'/>"+
											"</filter>"+
										"</link-entity>"+
									"</link-entity>"+
								"</entity>"+
							"</fetch>";
						var regCierresOferta = XrmServiceToolkit.Soap.Fetch(fetchXml);

						if(regCierresOferta.length >0)
						{
if ( regCierresOferta[0].attributes["atos_numerocierrespermitido"] != null)
							maxCierres = regCierresOferta[0].attributes["atos_numerocierrespermitido"].value
						}
				
						if (maxCierres != null)
						{
							//Comprobamos si con este cierre nos excedemos del mÃ¡ximo permitido por el tÃ©rmino EMS
							var fetchXml =
/* 223231 +1*/ 					//"<fetch mapping='logical'>"+			
								"<fetch mapping='logical' no-lock='true'>" +
									"<entity name='atos_cierre'>"+	
										"<attribute name='atos_variable'/>"+						
										"<filter>" +
											   "<condition attribute='atos_contratoid' operator='eq' value='"+contrato+"' />" +
										"</filter>" +							
									"</entity>"+
								"</fetch>";
							var reg = XrmServiceToolkit.Soap.Fetch(fetchXml);
							if (reg.length >= maxCierres)
							{
Xrm.Page.getControl("atos_variable").setNotification("No se pueden aÃ±adir mÃ¡s cierres a este contrato con el tÃ©rmino EMS del Pricing Output dado","0");
								
								Xrm.Page.data.entity.attributes.get("atos_variable").setValue();
								Xrm.Page.getAttribute("atos_variable").setSubmitMode("always");
							}
						}
					}//reg.length
				}//PricinO	
			}//ofertaActiva
		}//ofertaid
	}//oferta.length	
}

function calcularPreciosCierre()
{
	var valorCierre = Xrm.Page.data.entity.attributes.get("atos_valorcierre").getValue();
	var coeficiente;
	
	if ( Xrm.Page.data.entity.attributes.get("atos_pricinginputid").getValue() != null)
		coeficiente = Xrm.Page.data.entity.attributes.get("atos_pricinginputid").getValue()[0].id;
	
	var costeGestion = Xrm.Page.data.entity.attributes.get("atos_costegestioncierre").getValue();
	
	if (costeGestion == null)
		costeGestion = 0;
	
	if (valorCierre != null && coeficiente != null)
	{
		limpiaCampos();

		var pCoeficienteFijo;
		var pCoeficiente1;
		var pCoeficiente2;
		var pCoeficiente3;
		var pCoeficiente4;
		var pCoeficiente5;		
		var pCoeficiente6;
		var precioSNFijo;
		var precioSN1;
		var precioSN1;
		var precioSN2;
		var precioSN3;
		var precioSN4;
		var precioSN5;
		var precioSN6;

		//Recogemos los periodos del coeficiente
		var fetchXml =
/* 223231 +1*/	//"<fetch mapping='logical'>"+			
			"<fetch mapping='logical' no-lock='true'>" +
			"<entity name='atos_pricinginput'>"+	
				"<attribute name='atos_pfijo'/>"+
				"<attribute name='atos_p1'/>"+
				"<attribute name='atos_p2'/>"+
				"<attribute name='atos_p3'/>"+
				"<attribute name='atos_p4'/>"+
				"<attribute name='atos_p5'/>"+
				"<attribute name='atos_p6'/>"+		
				"<filter>" +
					   "<condition attribute='atos_pricinginputid' operator='eq' value='"+coeficiente+"' />" +
				"</filter>" +							
			"</entity>"+
		"</fetch>";
	
		var registros = XrmServiceToolkit.Soap.Fetch(fetchXml);

		if ( registros.length > 0 )
		{
			if ( registros[0].attributes["atos_pfijo"] != null )		
				pCoeficienteFijo =  registros[0].attributes["atos_pfijo"].value;
			else
			{
				if ( registros[0].attributes["atos_p1"] != null )
					pCoeficiente1 =  registros[0].attributes["atos_p1"].value;				
				if ( registros[0].attributes["atos_p2"] != null )		
					pCoeficiente2 =  registros[0].attributes["atos_p2"].value;
				if ( registros[0].attributes["atos_p3"] != null )		
					pCoeficiente3 =  registros[0].attributes["atos_p3"].value;
				if ( registros[0].attributes["atos_p4"] != null )		
					pCoeficiente4 =  registros[0].attributes["atos_p4"].value;
				if ( registros[0].attributes["atos_p5"] != null )		
					pCoeficiente5 =  registros[0].attributes["atos_p5"].value;
				if ( registros[0].attributes["atos_p6"] != null )		
					pCoeficiente6 =  registros[0].attributes["atos_p6"].value;	
			}
		}
		
		if (pCoeficienteFijo != null)
		{
			precioSNFijo = parseFloat(valorCierre * pCoeficienteFijo);
		}else
		{
			if (pCoeficiente1 != null)
				precioSN1 = parseFloat(valorCierre * pCoeficiente1);
			if (pCoeficiente2 != null)
				precioSN2 = parseFloat(valorCierre * pCoeficiente2);
			if (pCoeficiente3 != null)
				precioSN3 = parseFloat(valorCierre * pCoeficiente3);
			if (pCoeficiente4 != null)
				precioSN4 = parseFloat(valorCierre * pCoeficiente4);
			if (pCoeficiente5 != null)
				precioSN5 = parseFloat(valorCierre * pCoeficiente5);
			if (pCoeficiente6 != null)
				precioSN6 = parseFloat(valorCierre * pCoeficiente6);
		}


		if (precioSNFijo != null)
			cargaPreciosFijos(precioSNFijo,costeGestion);			
		else
			cargaPreciosVariables(precioSN1,precioSN2,precioSN3,precioSN4,precioSN5,precioSN6,costeGestion);				
	}
}

function cargaPreciosFijos(precioSNFijo,costeGestion)
{
	Xrm.Page.data.entity.attributes.get("atos_preciosincostep1").setValue(precioSNFijo);
	Xrm.Page.getAttribute("atos_preciosincostep1").setSubmitMode("always");
	Xrm.Page.data.entity.attributes.get("atos_preciop1").setValue(parseFloat(precioSNFijo + costeGestion));
	Xrm.Page.getAttribute("atos_preciop1").setSubmitMode("always");

	Xrm.Page.data.entity.attributes.get("atos_preciosincostep2").setValue(precioSNFijo);
	Xrm.Page.getAttribute("atos_preciosincostep2").setSubmitMode("always");
	Xrm.Page.data.entity.attributes.get("atos_preciop2").setValue(parseFloat(precioSNFijo + costeGestion));
	Xrm.Page.getAttribute("atos_preciop2").setSubmitMode("always");

	Xrm.Page.data.entity.attributes.get("atos_preciosincostep3").setValue(precioSNFijo);
	Xrm.Page.getAttribute("atos_preciosincostep3").setSubmitMode("always");
	Xrm.Page.data.entity.attributes.get("atos_preciop3").setValue(parseFloat(precioSNFijo + costeGestion));
	Xrm.Page.getAttribute("atos_preciop3").setSubmitMode("always");

	Xrm.Page.data.entity.attributes.get("atos_preciosincostep4").setValue(precioSNFijo);
	Xrm.Page.getAttribute("atos_preciosincostep4").setSubmitMode("always");
	Xrm.Page.data.entity.attributes.get("atos_preciop4").setValue(parseFloat(precioSNFijo + costeGestion));
	Xrm.Page.getAttribute("atos_preciop4").setSubmitMode("always");

	Xrm.Page.data.entity.attributes.get("atos_preciosincostep5").setValue(precioSNFijo);
	Xrm.Page.getAttribute("atos_preciosincostep5").setSubmitMode("always");
	Xrm.Page.data.entity.attributes.get("atos_preciop5").setValue(parseFloat(precioSNFijo + costeGestion));
	Xrm.Page.getAttribute("atos_preciop5").setSubmitMode("always");

	Xrm.Page.data.entity.attributes.get("atos_preciosincostep6").setValue(precioSNFijo);
	Xrm.Page.getAttribute("atos_preciosincostep6").setSubmitMode("always");
	Xrm.Page.data.entity.attributes.get("atos_preciop6").setValue(parseFloat(precioSNFijo + costeGestion));
	Xrm.Page.getAttribute("atos_preciop6").setSubmitMode("always");
}

function cargaPreciosVariables(precioSN1,precioSN2,precioSN3,precioSN4,precioSN5,precioSN6,costeGestion)
{
	if (precioSN1 != null)
	{
		Xrm.Page.data.entity.attributes.get("atos_preciosincostep1").setValue(precioSN1);
		Xrm.Page.getAttribute("atos_preciosincostep1").setSubmitMode("always");
		Xrm.Page.data.entity.attributes.get("atos_preciop1").setValue(parseFloat(precioSN1 + costeGestion));
		Xrm.Page.getAttribute("atos_preciop1").setSubmitMode("always");
	}
	if (precioSN2 != null)
	{
		Xrm.Page.data.entity.attributes.get("atos_preciosincostep2").setValue(precioSN2);
		Xrm.Page.getAttribute("atos_preciosincostep2").setSubmitMode("always");
		Xrm.Page.data.entity.attributes.get("atos_preciop2").setValue(parseFloat(precioSN2 + costeGestion));
		Xrm.Page.getAttribute("atos_preciop2").setSubmitMode("always");
	}
	if (precioSN3 != null)
	{
		Xrm.Page.data.entity.attributes.get("atos_preciosincostep3").setValue(precioSN3);
		Xrm.Page.getAttribute("atos_preciosincostep3").setSubmitMode("always");
		Xrm.Page.data.entity.attributes.get("atos_preciop3").setValue(parseFloat(precioSN3 + costeGestion));
		Xrm.Page.getAttribute("atos_preciop3").setSubmitMode("always");
	}
	if (precioSN4 != null)
	{
		Xrm.Page.data.entity.attributes.get("atos_preciosincostep4").setValue(precioSN4);
		Xrm.Page.getAttribute("atos_preciosincostep4").setSubmitMode("always");
		Xrm.Page.data.entity.attributes.get("atos_preciop4").setValue(parseFloat(precioSN4 + costeGestion));
		Xrm.Page.getAttribute("atos_preciop4").setSubmitMode("always");
	}
	if (precioSN5 != null)
	{
		Xrm.Page.data.entity.attributes.get("atos_preciosincostep5").setValue(precioSN5);
		Xrm.Page.getAttribute("atos_preciosincostep5").setSubmitMode("always");
		Xrm.Page.data.entity.attributes.get("atos_preciop5").setValue(parseFloat(precioSN5 + costeGestion));
		Xrm.Page.getAttribute("atos_preciop5").setSubmitMode("always");
	}
	if (precioSN6 != null)
	{
		Xrm.Page.data.entity.attributes.get("atos_preciosincostep6").setValue(precioSN6);
		Xrm.Page.getAttribute("atos_preciosincostep6").setSubmitMode("always");
		Xrm.Page.data.entity.attributes.get("atos_preciop6").setValue(parseFloat(precioSN6 + costeGestion));
		Xrm.Page.getAttribute("atos_preciop6").setSubmitMode("always");
	}
}

function limpiaCampos()
{
	for (var i=1; i<=6; i++)
	{		
		Xrm.Page.data.entity.attributes.get("atos_preciosincostep" + i).setValue();
		Xrm.Page.getAttribute("atos_preciosincostep" + i).setSubmitMode("always");
		Xrm.Page.data.entity.attributes.get("atos_preciop" + i).setValue();
		Xrm.Page.getAttribute("atos_preciop" + i).setSubmitMode("always");
	}	
}