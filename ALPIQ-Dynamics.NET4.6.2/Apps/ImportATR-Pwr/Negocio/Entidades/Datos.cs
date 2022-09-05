using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Negocio.Entidades
{
    /**
    // <summary>
    // Clase que recupera los "códigos ATR" de la solicitud. 
    // </summary>
    // <remarks>
    // Se crea una entidad en la que incluye todos los códigos atr de los campos lookup de la entidad Solicitud ATR
    // </remarks>
     */
    class Datos
    {
        private IOrganizationService service;
        private AtosLog atosLog;

        public Datos(IOrganizationService _service, AtosLog _atosLog)
        {
            service = _service;
            atosLog = new AtosLog(_atosLog);
        }

        public Entity datos01(Entity _solicitudATR)
        {
            Entity _datos = new Entity();

            if (_solicitudATR.Attributes.Contains("atos_lineadenegocioid"))
            {
                Entity _aux = service.Retrieve("atos_lineadenegocio", ((EntityReference)_solicitudATR.Attributes["atos_lineadenegocioid"]).Id, new ColumnSet("atos_codigo"));
                if (_aux.Attributes.Contains("atos_codigo"))
                    _datos.Attributes["atos_codigolineadenegocio"] = _aux.Attributes["atos_codigo"];
            }

            if (_solicitudATR.Attributes.Contains("atos_versionid"))
            {
                Entity _aux = service.Retrieve("atos_versionatr", ((EntityReference)_solicitudATR.Attributes["atos_versionid"]).Id, new ColumnSet("atos_codigo"));
                if (_aux.Attributes.Contains("atos_codigo"))
                    _datos.Attributes["atos_codigoversion"] = _aux.Attributes["atos_codigo"];
            }


            if (_solicitudATR.Attributes.Contains("atos_procesoatrid"))
            {
                Entity _aux = service.Retrieve("atos_procesoatr", ((EntityReference)_solicitudATR.Attributes["atos_procesoatrid"]).Id, new ColumnSet("atos_codigoproceso"));
                if (_aux.Attributes.Contains("atos_codigoproceso"))
                    _datos.Attributes["atos_codigoproceso"] = _aux.Attributes["atos_codigoproceso"];
            }

            if (_solicitudATR.Attributes.Contains("atos_pasoatrid"))
            {
                Entity _aux = service.Retrieve("atos_pasoatr", ((EntityReference)_solicitudATR.Attributes["atos_pasoatrid"]).Id, new ColumnSet("atos_codigopaso"));
                if (_aux.Attributes.Contains("atos_codigopaso"))
                    _datos.Attributes["atos_codigopaso"] = _aux.Attributes["atos_codigopaso"];
            }

            if (_solicitudATR.Attributes.Contains("atos_comercializadoraid"))
            {
                Entity _aux = service.Retrieve("atos_comercializadora", ((EntityReference)_solicitudATR.Attributes["atos_comercializadoraid"]).Id, new ColumnSet("atos_codigo"));
                if (_aux.Attributes.Contains("atos_codigo"))
                    _datos.Attributes["atos_codigocomercializadora"] = _aux.Attributes["atos_codigo"];
            }

            if (_solicitudATR.Attributes.Contains("atos_distribuidoraid"))
            {
                Entity _aux = service.Retrieve("atos_distribuidora", ((EntityReference)_solicitudATR.Attributes["atos_distribuidoraid"]).Id, new ColumnSet("atos_codigoocsum"));
                if (_aux.Attributes.Contains("atos_codigoocsum"))
                    _datos.Attributes["atos_codigodistribuidora"] = _aux.Attributes["atos_codigoocsum"];
            }


            if (_solicitudATR.Attributes.Contains("atos_indicadorsustitutomandatorioid"))
            {
                Entity _aux = service.Retrieve("atos_indicadorsustitutomandatorio", ((EntityReference)_solicitudATR.Attributes["atos_indicadorsustitutomandatorioid"]).Id, new ColumnSet("atos_codigo"));
                if (_aux.Attributes.Contains("atos_codigo"))
                    _datos.Attributes["atos_sustitutomandatorio"] = _aux.Attributes["atos_codigo"];
            }
            
            if (_solicitudATR.Attributes.Contains("atos_tipocontratoatrid"))
            {
                Entity _aux = service.Retrieve("atos_tipocontratoatr", ((EntityReference)_solicitudATR.Attributes["atos_tipocontratoatrid"]).Id, new ColumnSet("atos_codigo"));
                if (_aux.Attributes.Contains("atos_codigo"))
                    _datos.Attributes["atos_tipocontratoatr"] = _aux.Attributes["atos_codigo"];
            }


            if (_solicitudATR.Attributes.Contains("atos_solicitudadministrativacontractualid"))
            {
                Entity _aux = service.Retrieve("atos_solicitudadministrativacontractual", ((EntityReference)_solicitudATR.Attributes["atos_solicitudadministrativacontractualid"]).Id, new ColumnSet("atos_codigosolicitudadministrativacontractual"));
                if (_aux.Attributes.Contains("atos_codigosolicitudadministrativacontractual"))
                    _datos.Attributes["atos_codigosolicitudadministrativacontractual"] = _aux.Attributes["atos_codigosolicitudadministrativacontractual"];
            }

            
            if (_solicitudATR.Attributes.Contains("atos_indicativoactivacionlecturacicloid"))
            {
                Entity _aux = service.Retrieve("atos_indicativoactivacionlecturaciclo", ((EntityReference)_solicitudATR.Attributes["atos_indicativoactivacionlecturacicloid"]).Id, new ColumnSet("atos_codigo"));
                if (_aux.Attributes.Contains("atos_codigo"))
                    _datos.Attributes["atos_indicativoactivacionlecturaciclo"] = _aux.Attributes["atos_codigo"];
            }

            if (_solicitudATR.Attributes.Contains("atos_equipoaportadoporclientecomerid"))
            {
                Entity _aux = service.Retrieve("atos_equipoaportadoporclientecomer", ((EntityReference)_solicitudATR.Attributes["atos_equipoaportadoporclientecomerid"]).Id, new ColumnSet("atos_codigo"));
                if (_aux.Attributes.Contains("atos_codigo"))
                    _datos.Attributes["atos_equipoaportadoporcliente"] = _aux.Attributes["atos_codigo"];
            }

            
            if (_solicitudATR.Attributes.Contains("atos_tipoequipomedidaid"))
            {
                Entity _aux = service.Retrieve("atos_tipoequipomedida", ((EntityReference)_solicitudATR.Attributes["atos_tipoequipomedidaid"]).Id, new ColumnSet("atos_codigo"));
                if (_aux.Attributes.Contains("atos_codigo"))
                    _datos.Attributes["atos_tipoequipomedida"] = _aux.Attributes["atos_codigo"];
            }
             
            if (_solicitudATR.Attributes.Contains("atos_indicadordireccionid"))
            {
                Entity _aux = service.Retrieve("atos_direccioncorrespondencia", ((EntityReference)_solicitudATR.Attributes["atos_indicadordireccionid"]).Id, new ColumnSet("atos_codigo"));
                if (_aux.Attributes.Contains("atos_codigo"))
                    _datos.Attributes["atos_tipodireccion"] = _aux.Attributes["atos_codigo"];
            }


            if (_solicitudATR.Attributes.Contains("atos_instalacionid"))
            {
                Entity _aux = service.Retrieve("atos_instalacion", 
                                                ((EntityReference)_solicitudATR.Attributes["atos_instalacionid"]).Id, 
                                                new ColumnSet(new String [] {"atos_razonsocialid", "atos_cups22", "atos_cups20", 
                                                                            "atos_codigopuntosolicitudes", "atos_tipodocumentotitularatr",
                                                                            "atos_sociedadtitularatr", "atos_nombretitularatr", 
                                                                            "atos_primerapellidotitularatr", "atos_segundoapellidotitularatr", 
                                                                            "atos_numerodocumentotitularatr"}));
                if (_aux.Attributes.Contains("atos_codigopuntosolicitudes"))
                {
                    if (_aux.Attributes["atos_codigopuntosolicitudes"].ToString().Length == 20) // Si el cups 22 tiene solo 20 caracteres lo rellenamos también con dos blancos.
                        _datos.Attributes["atos_cups22"] = _aux.Attributes["atos_codigopuntosolicitudes"] + "  ";
                    else
                        _datos.Attributes["atos_cups22"] = _aux.Attributes["atos_codigopuntosolicitudes"];
                }
                /*else if (_aux.Attributes.Contains("atos_cups22"))
                {
                    if ( _aux.Attributes["atos_cups22"].ToString().Length == 20 ) // Si el cups 22 tiene solo 20 caracteres lo rellenamos también con dos blancos.
                        _datos.Attributes["atos_cups22"] = _aux.Attributes["atos_cups22"] + "  ";
                    else
                        _datos.Attributes["atos_cups22"] = _aux.Attributes["atos_cups22"];
                } */
                else if (_aux.Attributes.Contains("atos_cups20"))
                    _datos.Attributes["atos_cups22"] = _aux.Attributes["atos_cups20"] + "  ";



                if (_aux.Attributes.Contains("atos_tipodocumentotitularatr"))
                {
                    _datos.Attributes["tipodedocumento"] = _aux.Attributes["atos_tipodocumentotitularatr"];

                    if (((OptionSetValue)_aux.Attributes["atos_tipodocumentotitularatr"]).Value == 300000001) // CIF
                    {
                        if (_aux.Attributes.Contains("atos_sociedadtitularatr"))
                        {
                            _datos.Attributes["razonsocial"] = _aux.Attributes["atos_sociedadtitularatr"];
                        }
                    }
                    else if (_aux.Attributes.Contains("atos_nombretitularatr") && _aux.Attributes.Contains("atos_primerapellidotitularatr"))
                    {
                        _datos.Attributes["nombretitularatr"] = _aux.Attributes["atos_nombretitularatr"];
                        _datos.Attributes["primerapellidotitularatr"] = _aux.Attributes["atos_primerapellidotitularatr"];
                        if (_aux.Attributes.Contains("atos_segundoapellidotitularatr"))
                            _datos.Attributes["segundoapellidotitularatr"] = _aux.Attributes["atos_segundoapellidotitularatr"];

                    }

                }
                if (_aux.Attributes.Contains("atos_numerodocumentotitularatr"))
                {
                    _datos.Attributes["numerodedocumento"] = _aux.Attributes["atos_numerodocumentotitularatr"];
                }

                if (_aux.Attributes.Contains("atos_razonsocialid"))
                {
                    /*
                     * atos_tipodocumentotitularatr
                     * atos_numerodocumentotitularatr
                     * atos_sociedadtitularatr
                     * atos_nombretitularatr
                     * atos_primerapellidotitularatr
                     * atos_segundoapellidotitularatr
                     * */
                    
                    // ---
                    /*if (((EntityReference)_aux.Attributes["atos_razonsocialid"]).Name.Length > 45)
                        _datos.Attributes["razonsocial"] = ((EntityReference)_aux.Attributes["atos_razonsocialid"]).Name.Substring(0, 45);
                    else
                        _datos.Attributes["razonsocial"] = ((EntityReference)_aux.Attributes["atos_razonsocialid"]).Name;
                    Entity _account = service.Retrieve("account", ((EntityReference)_aux.Attributes["atos_razonsocialid"]).Id, new ColumnSet(new String [] {"atos_tipodedocumento", "atos_numerodedocumento", "address1_telephone1", "address1_fax"}));
                    if (_account.Attributes.Contains("atos_tipodedocumento"))
                    {
                        _datos.Attributes["tipodedocumento"] = _account.Attributes["atos_tipodedocumento"];
                    }
                    if (_account.Attributes.Contains("atos_numerodedocumento"))
                    {
                        _datos.Attributes["numerodedocumento"] = _account.Attributes["atos_numerodedocumento"];
                    }*/


                    Entity _account = service.Retrieve("account", ((EntityReference)_aux.Attributes["atos_razonsocialid"]).Id, new ColumnSet(new String[] { "address1_telephone1", "address1_fax" }));
                    if (_account.Attributes.Contains("address1_telephone1"))
                    {
                        _datos.Attributes["rz_telefono"] = _account.Attributes["address1_telephone1"];
                    }
                    if (_account.Attributes.Contains("address1_fax"))
                    {
                        _datos.Attributes["rz_fax"] = _account.Attributes["address1_fax"];
                    }
                }
            }
                
            if (_solicitudATR.Attributes.Contains("atos_modocontrolpotenciaid"))
            {
                Entity _aux = service.Retrieve("atos_modocontrolpotencia", ((EntityReference)_solicitudATR.Attributes["atos_modocontrolpotenciaid"]).Id, new ColumnSet("atos_codigo"));
                if (_aux.Attributes.Contains("atos_codigo"))
                    _datos.Attributes["modocontrolpotencia"] = _aux.Attributes["atos_codigo"];
            }

            if (_solicitudATR.Attributes.Contains("atos_contactoatrid"))
            {
                Entity _aux = service.Retrieve("contact", ((EntityReference)_solicitudATR.Attributes["atos_contactoatrid"]).Id,
                    new ColumnSet(new String[] { "firstname", "lastname", "fullname", "telephone1" }));
                if (_aux.Attributes.Contains("firstname") && _aux.Attributes.Contains("lastname"))
                {
                    _datos.Attributes["firstname_contact"] = _aux.Attributes["firstname"];
                    _datos.Attributes["lastname_contact"] = _aux.Attributes["lastname"];
                }
                else if (_aux.Attributes.Contains("fullname"))
                    _datos.Attributes["name_contact"] = _aux.Attributes["fullname"];
                if (_aux.Attributes.Contains("telephone1"))
                    _datos.Attributes["telephone_contact"] = _aux.Attributes["telephone1"];
            }

            if (_solicitudATR.Attributes.Contains("atos_instalacionicpid"))
            {
                Entity _aux = service.Retrieve("atos_indicativoinstalacionicp", ((EntityReference)_solicitudATR.Attributes["atos_instalacionicpid"]).Id, new ColumnSet("atos_codigo"));
                if (_aux.Attributes.Contains("atos_codigo"))
                    _datos.Attributes["instalacionicp"] = _aux.Attributes["atos_codigo"];
            }


            if (_solicitudATR.Attributes.Contains("atos_icpinstaladoycorrectoid"))
            {
                Entity _aux = service.Retrieve("atos_indicativoicpinstaladoycorrecto", ((EntityReference)_solicitudATR.Attributes["atos_icpinstaladoycorrectoid"]).Id, new ColumnSet("atos_codigo"));
                if (_aux.Attributes.Contains("atos_codigo"))
                    _datos.Attributes["icpinstaladoycorrecto"] = _aux.Attributes["atos_codigo"];
            }
            

            if (_solicitudATR.Attributes.Contains("atos_instalacionequipomedidaid"))
            {
                Entity _aux = service.Retrieve("atos_indicativoinstalacionequipodemedida", ((EntityReference)_solicitudATR.Attributes["atos_instalacionequipomedidaid"]).Id, new ColumnSet("atos_codigo"));
                if (_aux.Attributes.Contains("atos_codigo"))
                    _datos.Attributes["instalacionequipomedida"] = _aux.Attributes["atos_codigo"];
            }

            if (_solicitudATR.Attributes.Contains("atos_tipocambiotitularid"))
            {
                Entity _aux = service.Retrieve("atos_tipocambiotitular", ((EntityReference)_solicitudATR.Attributes["atos_tipocambiotitularid"]).Id, new ColumnSet("atos_codigocambiotitular"));
                if (_aux.Attributes.Contains("atos_codigocambiotitular"))
                    _datos.Attributes["codigocambiotitular"] = _aux.Attributes["atos_codigocambiotitular"];
            }



            if (_solicitudATR.Attributes.Contains("atos_tarifaid"))
            {
                Entity _aux = service.Retrieve("atos_tarifa", ((EntityReference)_solicitudATR.Attributes["atos_tarifaid"]).Id, new ColumnSet(new String[] { "atos_numeroperiodos", "atos_codigoocsum" }));
                if (_aux.Attributes.Contains("atos_numeroperiodos"))
                    _datos.Attributes["atos_numeroperiodos"] = _aux.Attributes["atos_numeroperiodos"];
                if (_aux.Attributes.Contains("atos_codigoocsum"))
                    _datos.Attributes["atos_codigoocsum"] = _aux.Attributes["atos_codigoocsum"];
            }
            
            if (_solicitudATR.Attributes.Contains("atos_esviviendahabitualid"))
            {
                Entity _aux = service.Retrieve("atos_viviendahabitual", ((EntityReference)_solicitudATR.Attributes["atos_esviviendahabitualid"]).Id, new ColumnSet(new String[] { "atos_codigo" }));
                if (_aux.Attributes.Contains("atos_codigo"))
                    _datos.Attributes["esviviendahabitual"] = _aux.Attributes["atos_codigo"];
            }

            if (_solicitudATR.Attributes.Contains("atos_tipoaparatoid"))
            {
                Entity _aux = service.Retrieve("atos_tipoaparato", ((EntityReference)_solicitudATR.Attributes["atos_tipoaparatoid"]).Id, new ColumnSet(new String[] { "atos_codigoaparato" }));
                if (_aux.Attributes.Contains("atos_codigoaparato"))
                    _datos.Attributes["tipoaparato"] = _aux.Attributes["atos_codigoaparato"];
            }

            if (_solicitudATR.Attributes.Contains("atos_marcaaparatoid"))
            {
                Entity _aux = service.Retrieve("atos_marcaaparato", ((EntityReference)_solicitudATR.Attributes["atos_marcaaparatoid"]).Id, new ColumnSet(new String[] { "atos_codigomarca" }));
                if (_aux.Attributes.Contains("atos_codigomarca"))
                    _datos.Attributes["marcaaparato"] = _aux.Attributes["atos_codigomarca"];
            }

            
            if (_solicitudATR.Attributes.Contains("atos_tensionsuministrocieid"))
            {
                Entity _aux = service.Retrieve("atos_tensionsuministroapm", ((EntityReference)_solicitudATR.Attributes["atos_tensionsuministrocieid"]).Id, new ColumnSet(new String[] { "atos_codigo" }));
                if (_aux.Attributes.Contains("atos_codigo"))
                    _datos.Attributes["tensionsuministrocie"] = _aux.Attributes["atos_codigo"];
            }

            if (_solicitudATR.Attributes.Contains("atos_ambitovalidezcieid"))
            {
                Entity _aux = service.Retrieve("atos_ambitovalidezcie", ((EntityReference)_solicitudATR.Attributes["atos_ambitovalidezcieid"]).Id, new ColumnSet(new String[] { "atos_codigo" }));
                if (_aux.Attributes.Contains("atos_codigo"))
                    _datos.Attributes["ambitovalidezcie"] = _aux.Attributes["atos_codigo"];
            }

            if (_solicitudATR.Attributes.Contains("atos_tensionsuministroapmid"))
            {
                Entity _aux = service.Retrieve("atos_tensionsuministroapm", ((EntityReference)_solicitudATR.Attributes["atos_tensionsuministroapmid"]).Id, new ColumnSet(new String[] { "atos_codigo" }));
                if (_aux.Attributes.Contains("atos_codigo"))
                    _datos.Attributes["tensionsuministroapm"] = _aux.Attributes["atos_codigo"];
            }


            if (_solicitudATR.Attributes.Contains("atos_instaladorcie2id"))
            {
                Entity _aux = service.Retrieve("atos_instalador", ((EntityReference)_solicitudATR.Attributes["atos_instaladorcie2id"]).Id,
                    new ColumnSet(new String[] { "atos_name", "atos_numerodedocumento", "atos_codigoinstalador" }));
                if (_aux.Attributes.Contains("atos_name"))
                    _datos.Attributes["name_instaladorcie"] = _aux.Attributes["atos_name"];
                if (_aux.Attributes.Contains("atos_numerodedocumento"))
                    _datos.Attributes["nif_instaladorcie"] = _aux.Attributes["atos_numerodedocumento"];
                if (_aux.Attributes.Contains("atos_codigoinstalador"))
                    _datos.Attributes["codigoinstaladorcie"] = _aux.Attributes["atos_codigoinstalador"];
            }

            if (_solicitudATR.Attributes.Contains("atos_instaladorapm2id"))
            {
                Entity _aux = service.Retrieve("atos_instalador", ((EntityReference)_solicitudATR.Attributes["atos_instaladorapm2id"]).Id,
                    new ColumnSet(new String[] { "atos_name", "atos_numerodedocumento", "atos_codigoinstalador" }));
                if (_aux.Attributes.Contains("atos_name"))
                    _datos.Attributes["name_instaladorapm"] = _aux.Attributes["atos_name"];
                if (_aux.Attributes.Contains("atos_numerodedocumento"))
                    _datos.Attributes["nif_instaladorapm"] = _aux.Attributes["atos_numerodedocumento"];
                if (_aux.Attributes.Contains("atos_codigoinstalador"))
                    _datos.Attributes["codigoinstaladorapm"] = _aux.Attributes["atos_codigoinstalador"];
            }
            //      

            if (_solicitudATR.Attributes.Contains("atos_motivobajaatrid"))
            {
                Entity _aux = service.Retrieve("atos_motivobajaatar", ((EntityReference)_solicitudATR.Attributes["atos_motivobajaatrid"]).Id,
                    new ColumnSet(new String[] { "atos_codigomotivobajaatr" }));
                if (_aux.Attributes.Contains("atos_codigomotivobajaatr"))
                    _datos.Attributes["codigomotivobajaatr"] = _aux.Attributes["atos_codigomotivobajaatr"];
            }

            return _datos;
        }
    }
}
