/*
 File="atos_oferta.js" 
 Copyright (c) Atos. All rights reserved.

 Plugin para las validaciones de las ofertas. 

 Fecha 		Codigo  Version Descripcion                                     Autor
 05.09.2022 23866   no-lock Incorporacion del No-lock a Consultas
*/

namespace ValidacionOferta
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    public class ValidacionOferta : IPlugin
    {

        private ITracingService tracingService;
        private IPluginExecutionContext PluginExecutionContext; ///< Contexto de ejecución del plugin
        private IOrganizationServiceFactory factory;
        private IOrganizationService service;

        private bool _log = false; ///< Indica si se activa o no el log. Esta variable debe inicializarse según el parámetro recibido en el constructor.
        private String ficherolog = "C:\\Users\\ValidacionOferta.txt";  ///< Fichero de log. Esta variable debe inicializarse según el parámetro recibido en el constructor.
        private const Char SEPARADOR = '#'; ///< Constante para el separador a usar en el parámetro que recibe el constructor
        private const String SALTO = "<br/>"; // + Environment.NewLine;

        /**
		// <summary>
		// Constructor de la clase
		// </summary>
        // <param name="parametros">Cadena en la que se indica si se escribe log y donde: LOG#C:\\RutaDelLog\\Fichero.log</param>
		// <remarks>
		// Recibe una cadena de texto incluyendo los parámetros (separados por el carácter #)
		// - El primer parámetro activa/desactiva la escritura del fichero log (LOG activa)
		// - El segundo parámetro es el nombre (incluyendo ruta) del fichero de log.
		// </remarks>
         */
        public ValidacionOferta(String parametros)
        {
            if (String.IsNullOrEmpty(parametros) == false)
            {
                String[] arrayPar = parametros.Split(SEPARADOR);
                if (arrayPar.Length > 0)
                {

                    if (arrayPar[0] == "LOG")
                        _log = true;
                    if (arrayPar.Length > 1)
                        ficherolog = arrayPar[1];
                }
            }
        }


        /**
        // <summary>
        // Función privada para escribir una traza
        // </summary>
        // <param name="texto">Texto a escribir en el fichero de log</param>
        // <remarks>
        // Si el log está activado escribe el mensaje en el fichero de log.
        // </remarks>
         */
        private void writelog(String texto)
        {
            //tracingService.Trace(texto);
            if (_log == true)
                System.IO.File.AppendAllText(ficherolog, texto + "\r\n");
        }

        /**
        // <summary>
        // Función privada que recupera las instalaciones de la razón social o de la cuenta negociadora recibidas
        // </summary>
        // <param name="_razonsocialId">Guid de la razón social</param>
        // <param name="_cuentanegociadoraId">Guid de la cuenta negociadora</param>
        // <remarks>
        // - Devuelve una colección de entidades de tipo instalación. 
        // - O bien devuelve las instalaciones de la razón social recibida por parámetro
        // - O bien devuelve las instalaciones de las razones sociales de la cuenta negociadora recibida por parámetro
        // </remarks>
         */
        private EntityCollection instalaciones(Guid _razonsocialId, Guid _cuentanegociadoraId)
        {
            FilterExpression filtro = new FilterExpression();
            filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression condicion1;
            condicion1 = new ConditionExpression();
            condicion1.AttributeName = "atos_historico";
            condicion1.Operator = ConditionOperator.NotEqual;
            condicion1.Values.Add((true));
            filtro.Conditions.Add(condicion1);

            condicion1 = new ConditionExpression();
            condicion1.AttributeName = "statecode";
            condicion1.Operator = ConditionOperator.Equal;
            condicion1.Values.Add(0);
            filtro.Conditions.Add(condicion1);
            
            if (_razonsocialId != Guid.Empty)
            {
                ConditionExpression condicion2;
                condicion2 = new ConditionExpression();
                condicion2.AttributeName = "atos_razonsocialid";
                condicion2.Operator = ConditionOperator.Equal;
                condicion2.Values.Add(_razonsocialId.ToString());
                filtro.Conditions.Add(condicion2);
            }

            /*condicion = new ConditionExpression();
            condicion.AttributeName = "atos_sistemaelectricoid";
            condicion.Operator = ConditionOperator.NotNull;
            filtro.Conditions.Add(condicion);

            condicion = new ConditionExpression();
            condicion.AttributeName = "atos_tarifaid";
            condicion.Operator = ConditionOperator.NotNull;
            filtro.Conditions.Add(condicion);*/


            ConditionExpression condicion = new ConditionExpression();
            condicion.AttributeName = "atos_sistemaelectricoid";
            condicion.Operator = ConditionOperator.NotNull;
            filtro.Conditions.Add(condicion);

            condicion = new ConditionExpression();
            condicion.AttributeName = "atos_tarifaid";
            condicion.Operator = ConditionOperator.NotNull;
            filtro.Conditions.Add(condicion);
            
            QueryExpression consulta = new QueryExpression("atos_instalacion");
            consulta.ColumnSet.AddColumns("atos_name", "atos_sistemaelectricoid", "atos_tarifaid", "atos_lote", "statecode");
            consulta.Criteria.AddFilter(filtro);
            /* 23866 +1 no-lock */
            consulta.NoLock = true;

            if (_razonsocialId == Guid.Empty)
            {
                LinkEntity _link = new LinkEntity();
                _link.JoinOperator = JoinOperator.Inner;
                _link.LinkFromAttributeName = "atos_razonsocialid";
                _link.LinkFromEntityName = consulta.EntityName;
                _link.LinkToAttributeName = "accountid";
                _link.LinkToEntityName = "account";
                _link.LinkCriteria.AddCondition(new ConditionExpression("atos_cuentanegociadoraid", ConditionOperator.Equal, _cuentanegociadoraId.ToString()));

                consulta.LinkEntities.Add(_link);
            }

            OrderExpression orden;

            orden = new OrderExpression();
            orden.AttributeName = "atos_lote";
            orden.OrderType = OrderType.Ascending;
            consulta.Orders.Add(orden);

            EntityCollection _resConsulta = service.RetrieveMultiple(consulta);
            return _resConsulta;
        }

        /**
       // <summary>
       // Función privada que recupera las instalaciones gas de la razón social o de la cuenta negociadora recibidas
       // </summary>
       // <param name="_razonsocialId">Guid de la razón social</param>
       // <param name="_cuentanegociadoraId">Guid de la cuenta negociadora</param>
       // <remarks>
       // - Devuelve una colección de entidades de tipo instalación. 
       // - O bien devuelve las instalaciones de la razón social recibida por parámetro
       // - O bien devuelve las instalaciones de las razones sociales de la cuenta negociadora recibida por parámetro
       // </remarks>
        */
        private EntityCollection instalacionesGas(Guid _razonsocialId, Guid _cuentanegociadoraId)
        {
            FilterExpression filtro = new FilterExpression();
            filtro.FilterOperator = LogicalOperator.And;

            ConditionExpression condicion1;
            /*condicion1 = new ConditionExpression();
            condicion1.AttributeName = "atos_historico";
            condicion1.Operator = ConditionOperator.NotEqual;
            condicion1.Values.Add((true));
            filtro.Conditions.Add(condicion1);*/

            condicion1 = new ConditionExpression();
            condicion1.AttributeName = "statecode";
            condicion1.Operator = ConditionOperator.Equal;
            condicion1.Values.Add(0);
            filtro.Conditions.Add(condicion1);

            if (_razonsocialId != Guid.Empty)
            {
                ConditionExpression condicion2;
                condicion2 = new ConditionExpression();
                condicion2.AttributeName = "atos_razonsocialid";
                condicion2.Operator = ConditionOperator.Equal;
                condicion2.Values.Add(_razonsocialId.ToString());
                filtro.Conditions.Add(condicion2);
            }

            ConditionExpression condicion = new ConditionExpression();
            condicion.AttributeName = "atos_peajeid";
            condicion.Operator = ConditionOperator.NotNull;
            filtro.Conditions.Add(condicion);

            condicion = new ConditionExpression();
            condicion.AttributeName = "atos_usodelgasid";
            condicion.Operator = ConditionOperator.NotNull;
            filtro.Conditions.Add(condicion);

            QueryExpression consulta = new QueryExpression("atos_instalaciongas");
            consulta.ColumnSet.AddColumns("atos_name", "atos_usodelgasid", "atos_peajeid", "statecode");
            consulta.Criteria.AddFilter(filtro);
            /* 23866 +1 no-lock */
            consulta.NoLock = true;


            if (_razonsocialId == Guid.Empty)
            {
                LinkEntity _link = new LinkEntity();
                _link.JoinOperator = JoinOperator.Inner;
                _link.LinkFromAttributeName = "atos_razonsocialid";
                _link.LinkFromEntityName = consulta.EntityName;
                _link.LinkToAttributeName = "accountid";
                _link.LinkToEntityName = "account";
                _link.LinkCriteria.AddCondition(new ConditionExpression("atos_cuentanegociadoraid", ConditionOperator.Equal, _cuentanegociadoraId.ToString()));

                consulta.LinkEntities.Add(_link);
            }

            EntityCollection _resConsulta = service.RetrieveMultiple(consulta);
            return _resConsulta;
        }



        /**
        // <summary>
        // Punto de entrada del plugin.
        // </summary>
        // <param name="serviceProvider">The service provider.</param>
        // <remarks>
        // - Se ejecuta en la creación y modificación de ofertas.
        // - Realiza las siguiente validaciones:
        // - La oferta debe tener el campo de instalación informado
        // - La oferta debe tener el campo de instalación informado
        // - La instalación debe tener tarifa
        // - La instalación debe tener sistema eléctrico
        // - La instalación debe estar activa
        // - Si la oferta es multipunto repite esas validaciones para cada una de las ofertas hijas
        // </remarks>
         */
        public void Execute(IServiceProvider serviceProvider)
        {

            tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            PluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the Organization Service factory service from the service provider
            factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            // Use the factory to generate the Organization Service.
            service = factory.CreateOrganizationService(PluginExecutionContext.UserId);

            writelog("-----------------------------------------");
            writelog(DateTime.Now.ToLocalTime().ToString());
            writelog("Plugin Validaciones de oferta");
            writelog("Mensaje: " + PluginExecutionContext.MessageName);
            if (PluginExecutionContext.MessageName == "Create")
            {
                Entity ef = (Entity)PluginExecutionContext.InputParameters["Target"];

                String nombreOF = " ";
                if (ef.Attributes.Contains("atos_name"))
                    nombreOF += ef.Attributes["atos_name"] + " ";

                if (!ef.Attributes.Contains("atos_tipooferta")) 
                    throw new Exception("La oferta" + nombreOF + "no está correctamente definida. No tiene tipo de oferta");

                if (ef.Attributes.Contains("atos_ofertapadreid"))
                    return; // Si es oferta hija (o suboferta) no validamos ya que se validará en la multipunto

                writelog("Comprueba si es multipunto Tipooferta: " + ((OptionSetValue)ef.Attributes["atos_tipooferta"]).Value.ToString());

                if (((OptionSetValue)ef.Attributes["atos_commodity"]).Value == 300000000)
                {
                    //Creacion de ofertas para instalaciones electricas
                    validarOfertaPower(ef);
                }
                else
                {
                    //Creacion de ofertas para instalaciones de gas
                    validarOfertaGas(ef);
                }

            }
        }

        private void validarOfertaPower (Entity ef)
        {
            if (((OptionSetValue)ef.Attributes["atos_tipooferta"]).Value == 300000000)
            {
                writelog("Oferta Multipunto");

                Guid _razonsocialId = Guid.Empty;
                Guid _cuentanegociadoraId = Guid.Empty;
                if (ef.Attributes.Contains("atos_razonsocialid"))
                {
                    _razonsocialId = ((EntityReference)ef.Attributes["atos_razonsocialid"]).Id;
                    Entity _razonsocial = service.Retrieve("account", _razonsocialId, new ColumnSet("atos_cuentanegociadoraid"));
                    _cuentanegociadoraId = ((EntityReference)_razonsocial.Attributes["atos_cuentanegociadoraid"]).Id;
                }
                else
                    _cuentanegociadoraId = ((EntityReference)ef.Attributes["atos_cuentanegociadoraid"]).Id;

                EntityCollection _instalaciones = instalaciones(_razonsocialId, _cuentanegociadoraId);

                String _error = "";
                String _salto = "";
                int _nlote = 0;
                writelog("Hay " + _instalaciones.Entities.Count.ToString() + " instalaciones");
                for (int i = 0; i < _instalaciones.Entities.Count; i++)
                {
                    writelog("validando instalación " + i.ToString());
                    if (_instalaciones.Entities[i].Attributes.Contains("atos_lote"))
                        _nlote++;
                    if (!_instalaciones.Entities[i].Attributes.Contains("atos_tarifaid"))
                    {
                        _error += string.Format("{0}La instalación {1} no tiene tarifa.", _salto, _instalaciones.Entities[i].Attributes["atos_name"].ToString());
                        _salto = SALTO;
                    }
                    if (!_instalaciones.Entities[i].Attributes.Contains("atos_sistemaelectricoid"))
                    {
                        _error += string.Format("{0}La instalación {1} no tiene sistema eléctrico.", _salto, _instalaciones.Entities[i].Attributes["atos_name"].ToString());
                        _salto = SALTO;
                    }

                    if (!_instalaciones.Entities[i].Attributes.Contains("statecode"))
                    {
                        _error += string.Format("{0}La instalación {1} no tiene estado.", _salto, _instalaciones.Entities[i].Attributes["atos_name"].ToString());
                        _salto = SALTO;
                    }
                    else if (((OptionSetValue)_instalaciones.Entities[i].Attributes["statecode"]).Value != 0)
                    {
                        _error += string.Format("{0}La instalación {1} no está activa.", _salto, _instalaciones.Entities[i].Attributes["atos_name"].ToString());
                        _salto = SALTO;
                    }

                }

                if (_nlote != 0 && _nlote != _instalaciones.Entities.Count)
                {
                    _error += string.Format("{0}Hay instalaciones con el lote informado y otras que no lo tienen.", _salto);
                    _salto = SALTO;
                }

                if (_instalaciones.Entities.Count == 0)
                {
                    _error += string.Format("{0}Debe existir al menos una instalación.", _salto);
                    _salto = SALTO;
                }

                if (_error != "")
                    throw new Exception(_error);
            }
            else if (((OptionSetValue)ef.Attributes["atos_tipooferta"]).Value == 300000002)
            {
                String _error = "";
                String _salto = "";

                writelog("Oferta NO multipunto");

                if (!ef.Attributes.Contains("atos_instalacionid"))
                {
                    _error += string.Format("{0}La oferta debe tener el campo instalación informado.", _salto);
                    _salto = SALTO;
                }
                else
                {
                    writelog("Busca datos de la instalación");

                    Entity _instalacion = service.Retrieve("atos_instalacion", ((EntityReference)ef.Attributes["atos_instalacionid"]).Id,
                        new ColumnSet(new String[] { "atos_name", "atos_tarifaid", "atos_sistemaelectricoid", "statecode" }));
                    if (!_instalacion.Attributes.Contains("atos_tarifaid"))
                    {
                        _error += string.Format("{0}La instalación {1} no tiene tarifa.", _salto, _instalacion.Attributes["atos_name"].ToString());
                        _salto = SALTO;
                    }
                    if (!_instalacion.Attributes.Contains("atos_sistemaelectricoid"))
                    {
                        _error += string.Format("{0}La instalación {1} no tiene sistema eléctrico.", _salto, _instalacion.Attributes["atos_name"].ToString());
                        _salto = SALTO;
                    }

                    if (!_instalacion.Attributes.Contains("statecode"))
                    {
                        _error += string.Format("{0}La instalación {1} no tiene estado.", _salto, _instalacion.Attributes["atos_name"].ToString());
                        _salto = SALTO;
                    }
                    else if (((OptionSetValue)_instalacion.Attributes["statecode"]).Value != 0)
                    {
                        _error += string.Format("{0}La instalación {1} no está activa.", _salto, _instalacion.Attributes["atos_name"].ToString());
                        _salto = SALTO;
                    }


                }

                if (_error != "")
                    throw new Exception(_error);
            }
        }

        private void validarOfertaGas (Entity ef)
        {
            if (((OptionSetValue)ef.Attributes["atos_tipooferta"]).Value == 300000000) //300000000 --> Multipunto
            {
                writelog("Oferta Multipunto");

                Guid _razonsocialId = Guid.Empty;
                Guid _cuentanegociadoraId = Guid.Empty;
                if (ef.Attributes.Contains("atos_razonsocialid"))
                {
                    _razonsocialId = ((EntityReference)ef.Attributes["atos_razonsocialid"]).Id;
                    Entity _razonsocial = service.Retrieve("account", _razonsocialId, new ColumnSet("atos_cuentanegociadoraid"));
                    _cuentanegociadoraId = ((EntityReference)_razonsocial.Attributes["atos_cuentanegociadoraid"]).Id;
                }
                else
                    _cuentanegociadoraId = ((EntityReference)ef.Attributes["atos_cuentanegociadoraid"]).Id;

                EntityCollection _instalaciones = instalacionesGas(_razonsocialId, _cuentanegociadoraId);

                String _error = "";
                String _salto = "";
                //int _nlote = 0;
                writelog("Hay " + _instalaciones.Entities.Count.ToString() + " instalaciones");
                for (int i = 0; i < _instalaciones.Entities.Count; i++)
                {
                    writelog("validando instalación " + i.ToString());
                    if (!_instalaciones.Entities[i].Attributes.Contains("atos_peajeid"))
                    {
                        _error += string.Format("{0}La instalación gas {1} no tiene peaje.", _salto, _instalaciones.Entities[i].Attributes["atos_name"].ToString());
                        _salto = SALTO;
                    }
                    if (!_instalaciones.Entities[i].Attributes.Contains("atos_usodelgasid"))
                    {
                        _error += string.Format("{0}La instalación gas {1} no tiene uso del gas.", _salto, _instalaciones.Entities[i].Attributes["atos_name"].ToString());
                        _salto = SALTO;
                    }

                    if (!_instalaciones.Entities[i].Attributes.Contains("statecode"))
                    {
                        _error += string.Format("{0}La instalación gas {1} no tiene estado.", _salto, _instalaciones.Entities[i].Attributes["atos_name"].ToString());
                        _salto = SALTO;
                    }
                    else if (((OptionSetValue)_instalaciones.Entities[i].Attributes["statecode"]).Value != 0)
                    {
                        _error += string.Format("{0}La instalación gas {1} no está activa.", _salto, _instalaciones.Entities[i].Attributes["atos_name"].ToString());
                        _salto = SALTO;
                    }

                }

                if (_instalaciones.Entities.Count == 0)
                {
                    _error += string.Format("{0}Debe existir al menos una instalación gas.", _salto);
                    _salto = SALTO;
                }

                if (_error != "")
                    throw new Exception(_error);
            }
            else if (((OptionSetValue)ef.Attributes["atos_tipooferta"]).Value == 300000002) //300000002 --> Oferta
            {
                String _error = "";
                String _salto = "";

                writelog("Oferta NO multipunto");

                if (!ef.Attributes.Contains("atos_instalaciongasid"))
                {
                    _error += string.Format("{0}La oferta debe tener el campo instalación informado.", _salto);
                    _salto = SALTO;
                }
                else
                {
                    writelog("Busca datos de la instalación");

                    Entity _instalaciongas = service.Retrieve("atos_instalaciongas", ((EntityReference)ef.Attributes["atos_instalaciongasid"]).Id,
                        new ColumnSet(new String[] { "atos_name", "atos_peajeid", "atos_usodelgasid", "statecode" }));
                    if (!_instalaciongas.Attributes.Contains("atos_peajeid"))
                    {
                        _error += string.Format("{0}La instalación {1} no tiene peaje.", _salto, _instalaciongas.Attributes["atos_name"].ToString());
                        _salto = SALTO;
                    }
                    if (!_instalaciongas.Attributes.Contains("atos_usodelgasid"))
                    {
                        _error += string.Format("{0}La instalación {1} no tiene uso del gas.", _salto, _instalaciongas.Attributes["atos_name"].ToString());
                        _salto = SALTO;
                    }

                    if (!_instalaciongas.Attributes.Contains("statecode"))
                    {
                        _error += string.Format("{0}La instalación {1} no tiene estado.", _salto, _instalaciongas.Attributes["atos_name"].ToString());
                        _salto = SALTO;
                    }
                    else if (((OptionSetValue)_instalaciongas.Attributes["statecode"]).Value != 0)
                    {
                        _error += string.Format("{0}La instalación {1} no está activa.", _salto, _instalaciongas.Attributes["atos_name"].ToString());
                        _salto = SALTO;
                    }
                }

                if (_error != "")
                    throw new Exception(_error);
            }
        }
    }
}