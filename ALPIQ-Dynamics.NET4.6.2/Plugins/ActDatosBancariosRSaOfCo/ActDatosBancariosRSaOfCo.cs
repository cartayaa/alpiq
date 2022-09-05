using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActDatosBancariosRSaOfCo
{
    public class ActDatosBancariosRSaOfCo : PluginBase
    {
        public ActDatosBancariosRSaOfCo(string unsecure, string secure) : base(typeof(ActDatosBancariosRSaOfCo))
        {

        }

        private LocalPluginContext localcontext { get; set; }

        /// <summary>
        /// Placeholder for a custom plug-in implementation. 
        /// </summary>
        /// <param name="localcontext">Context for the current plug-in.</param>
        protected override void ExecuteCrmPlugin(LocalPluginContext localcontext)
        {
            this.localcontext = localcontext;
            Entity ef = (Entity)localcontext.PluginExecutionContext.InputParameters["Target"];
            localcontext.Trace("Actualizamos ofertas activas y que coincidan con la razon social, ademas que la fecha fin de videncia de oferta sea null, igual o mayor a la fecha actual de ejecucion");
            actualizaOfertas(ef);
            localcontext.Trace("Actualizamos contratos activos y cuya fecha fin definitiva de contrato sea superio o igual a la fecha de ejecuacion del pluging y coincidan con la razon social");
            actualizaContratos(ef);
        }

        private EntityReference valorEntityReference(Entity _ef, string _nombre, string _campo)
        {
            EntityReference _ret = null;

            if (_ef.Attributes[_campo] != null)
                _ret = new EntityReference(_nombre, ((EntityReference)_ef.Attributes[_campo]).Id);
            return _ret;
        }

        private void actualizaOfertas(Entity rs)
        {
            Entity _oferta = new Entity("atos_oferta");

            bool _actualizar = false;

            if (rs.Attributes.Contains("atos_formadepago"))
            {
                _oferta.Attributes["atos_formadepago"] = rs.Attributes["atos_formadepago"];
                _actualizar = true;
            }


            if (rs.Attributes.Contains("atos_condicionpagoid"))
            {
                _oferta.Attributes["atos_condicionpagoid"] = valorEntityReference(rs, "atos_condiciondepago", "atos_condicionpagoid");
                _actualizar = true;
            }

            if (rs.Attributes.Contains("atos_tipodeenvio"))
            {
                _oferta.Attributes["atos_tipodeenvio"] = rs.Attributes["atos_tipodeenvio"];
                _actualizar = true;
            }

            if (rs.Attributes.Contains("atos_plazoenviofacturas"))
            {
                _oferta.Attributes["atos_plazoenviofacturas"] = rs.Attributes["atos_plazoenviofacturas"];
                _actualizar = true;
            }

            if (rs.Attributes.Contains("atos_mandatosepa"))
            {
                _oferta.Attributes["atos_mandatosepa"] = rs.Attributes["atos_mandatosepa"];
                _actualizar = true;
            }

            if (rs.Attributes.Contains("atos_swift"))
            {
                _oferta.Attributes["atos_swift"] = rs.Attributes["atos_swift"];
                _actualizar = true;
            }

            if (rs.Attributes.Contains("atos_iban"))
            {
                _oferta.Attributes["atos_iban"] = rs.Attributes["atos_iban"];
                _actualizar = true;
            }

            if (rs.Attributes.Contains("atos_entidadbancaria"))
            {
                _oferta.Attributes["atos_entidadbancaria"] = rs.Attributes["atos_entidadbancaria"];
                _actualizar = true;
            }

            if (rs.Attributes.Contains("atos_sucursalbancaria"))
            {
                _oferta.Attributes["atos_sucursalbancaria"] = rs.Attributes["atos_sucursalbancaria"];
                _actualizar = true;
            }

            if (rs.Attributes.Contains("atos_digitocontrol"))
            {
                _oferta.Attributes["atos_digitocontrol"] = rs.Attributes["atos_digitocontrol"];
                _actualizar = true;
            }

            if (rs.Attributes.Contains("atos_cuenta"))
            {
                _oferta.Attributes["atos_cuenta"] = rs.Attributes["atos_cuenta"];
                _actualizar = true;
            }

            if (rs.Attributes.Contains("atos_cuentabancaria"))
            {
                _oferta.Attributes["atos_cuentabancaria"] = rs.Attributes["atos_cuentabancaria"];
                _actualizar = true;
            }

            if (rs.Attributes.Contains("atos_cuentabancariaapropia"))
            {
                _oferta.Attributes["atos_cuentabancariaapropia"] = rs.Attributes["atos_cuentabancariaapropia"];
                _actualizar = true;
            }

            if (_actualizar == false)
                return;

            QueryExpression _consulta = new QueryExpression("atos_oferta");
            _consulta.ColumnSet = new ColumnSet(new String[] { "atos_ofertaid", "atos_name" });
            _consulta.Criteria.FilterOperator = LogicalOperator.And;

            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_razonsocialid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(rs.Id.ToString());
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "statecode";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(0);
            _filtro.Conditions.Add(_condicion);

            _consulta.Criteria.AddFilter(_filtro);

            _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.Or;
            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_fechafinvigenciaoferta";
            _condicion.Operator = ConditionOperator.GreaterEqual;
            _condicion.Values.Add(DateTime.Now);
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_fechafinvigenciaoferta";
            _condicion.Operator = ConditionOperator.Null;
            _filtro.Conditions.Add(_condicion);

            _consulta.Criteria.AddFilter(_filtro);

            EntityCollection _resConsulta = localcontext.OrganizationService.RetrieveMultiple(_consulta);
            localcontext.Trace("Razón social. Actualizamos todas las ofertas encontradas, total a actualizar: " + _resConsulta.Entities.Count());
            foreach (Entity _o in _resConsulta.Entities)
            {
                //localcontext.Trace("Razón social. Actualizacion Oferta");
                if (_o.Attributes.Contains("atos_name"))
                {
                    //localcontext.Trace("Razón social. Actualizando Oferta: " + _o.Attributes["atos_name"].ToString());
                }
                else
                {
                    localcontext.Trace("Razón social. Actualizando Oferta, no tiene atos_name: " + _o.Id.ToString());
                }

                _oferta.Id = _o.Id;
                try {
                    localcontext.OrganizationService.Update(_oferta);
                } catch (Exception e)
                {
                    localcontext.Trace("Error al actualizar la oferta " + _o.Attributes["atos_name"] + ", ERROR: " + e);
                    throw new Exception("Error al actualizar una de las ofertas " + _o.Attributes["atos_name"] + ", ERROR: " + e.Message);
                }
                
                //localcontext.Trace("Razón social. Fin Actualizacion Oferta");
            }
            localcontext.Trace("Razón social. Fin de actualizar todas las ofertas encontradas");

        }

        private void actualizaContratos(Entity rs)
        {
            Entity _contrato = new Entity("atos_contrato");

            bool _actualizar = false;

            if (rs.Attributes.Contains("atos_formadepago"))
            {
                _contrato.Attributes["atos_formadepago"] = rs.Attributes["atos_formadepago"];
                _actualizar = true;
            }


            if (rs.Attributes.Contains("atos_condicionpagoid"))
            {
                _contrato.Attributes["atos_condicionpagoid"] = valorEntityReference(rs, "atos_condiciondepago", "atos_condicionpagoid");
                _actualizar = true;
            }

            if (rs.Attributes.Contains("atos_tipodeenvio"))
            {
                _contrato.Attributes["atos_tipodeenvio"] = rs.Attributes["atos_tipodeenvio"];
                _actualizar = true;
            }

            if (rs.Attributes.Contains("atos_plazoenviofacturas"))
            {
                _contrato.Attributes["atos_plazoenviofacturas"] = rs.Attributes["atos_plazoenviofacturas"];
                _actualizar = true;
            }

            if (rs.Attributes.Contains("atos_mandatosepa"))
            {
                _contrato.Attributes["atos_mandatosepa"] = rs.Attributes["atos_mandatosepa"];
                _actualizar = true;
            }

            if (rs.Attributes.Contains("atos_swift"))
            {
                _contrato.Attributes["atos_swift"] = rs.Attributes["atos_swift"];
                _actualizar = true;
            }

            if (rs.Attributes.Contains("atos_iban"))
            {
                _contrato.Attributes["atos_iban"] = rs.Attributes["atos_iban"];
                _actualizar = true;
            }

            if (rs.Attributes.Contains("atos_entidadbancaria"))
            {
                _contrato.Attributes["atos_entidadbancaria"] = rs.Attributes["atos_entidadbancaria"];
                _actualizar = true;
            }

            if (rs.Attributes.Contains("atos_sucursalbancaria"))
            {
                _contrato.Attributes["atos_sucursalbancaria"] = rs.Attributes["atos_sucursalbancaria"];
                _actualizar = true;
            }

            if (rs.Attributes.Contains("atos_digitocontrol"))
            {
                _contrato.Attributes["atos_digitocontrol"] = rs.Attributes["atos_digitocontrol"];
                _actualizar = true;
            }

            if (rs.Attributes.Contains("atos_cuenta"))
            {
                _contrato.Attributes["atos_cuenta"] = rs.Attributes["atos_cuenta"];
                _actualizar = true;
            }

            if (rs.Attributes.Contains("atos_cuentabancaria"))
            {
                _contrato.Attributes["atos_cuentabancaria"] = rs.Attributes["atos_cuentabancaria"];
                _actualizar = true;
            }

            if (rs.Attributes.Contains("atos_cuentabancariaapropia"))
            {
                _contrato.Attributes["atos_cuentabancariaapropia"] = rs.Attributes["atos_cuentabancariaapropia"];
                _actualizar = true;
            }

            if (_actualizar == false)
                return;

            QueryExpression _consulta = new QueryExpression("atos_contrato");
            _consulta.ColumnSet = new ColumnSet(new String[] { "atos_contratoid", "atos_name" });

            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;
            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_razonsocialid";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(rs.Id.ToString());
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "atos_fechafindefinitiva";
            _condicion.Operator = ConditionOperator.GreaterEqual;
            _condicion.Values.Add(DateTime.Now);
            _filtro.Conditions.Add(_condicion);

            _condicion = new ConditionExpression();
            _condicion.AttributeName = "statecode";
            _condicion.Operator = ConditionOperator.Equal;
            _condicion.Values.Add(0);
            _filtro.Conditions.Add(_condicion);

            _consulta.Criteria.AddFilter(_filtro);
            

            EntityCollection _resConsulta = localcontext.OrganizationService.RetrieveMultiple(_consulta);
            localcontext.Trace("Razón social. Actualizamos todas los contratos encontrados, total a actualizar: " + _resConsulta.Entities.Count());
            foreach (Entity _o in _resConsulta.Entities)
            {
                //localcontext.Trace("Razón social. Actualizacion Contato");
                if (_o.Attributes.Contains("atos_name"))
                {
                    //localcontext.Trace("Razón social. Actualizando Contrato: " + _o.Attributes["atos_name"].ToString());
                }
                else
                {
                    localcontext.Trace("Razón social. Actualizando Contrato, no tiene atos_name: " + _o.Id.ToString());
                }

                _contrato.Id = _o.Id;
                try
                {
                    localcontext.OrganizationService.Update(_contrato);
                }
                catch (Exception e)
                {
                    localcontext.Trace("Error al actualizar el contrato " + _o.Attributes["atos_name"] + ", ERROR: " + e);
                    throw new Exception("Error al actualizar unos de los contratos " + _o.Attributes["atos_name"] + ", ERROR: " + e.Message);
                }

                //localcontext.Trace("Razón social. Fin Actualizacion Contrato");
            }
            localcontext.Trace("Razón social. Fin de actualizar todos los contratos encontrados");

        }
    }
}
