using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace Exportar
{
    public class EntidadCRM
    {
        
		private IOrganizationService service;
		private ITracingService trace;
        private Entity ef;
        private const string formatoFecha = "yyyy-MM-dd";
        private const string formatoFechaCompleto = "yyyy-MM-dd HH:mm:ss.fff";
        // DateTime myDate = DateTime.ParseExact("2009-05-08 14:40:52,531", formatoFechaCompleto, System.Globalization.CultureInfo.InvariantCulture);

		public EntidadCRM(Entity _ef, IOrganizationService _service, ITracingService _trace)
		{
			ef = _ef;
			service = _service;
			trace = _trace;
            
		}
		
        public IOrganizationService Service
        {
            get
            {
                return this.service;
            }
        }
        public ITracingService Trace
        {
            get
            {
                return this.trace;
            }
        }
		public Entity Ef
        {
            get
            {
                return this.ef;
            }
        }
		
        /*private AttributeTypeCode tipoAtributo(String _campo)
        {
            RetrieveAttributeRequest attributeRequest = new RetrieveAttributeRequest
            {
                EntityLogicalName = ef.LogicalName,
                LogicalName = _campo,
                RetrieveAsIfPublished = true
            };

            // Execute the request
            RetrieveAttributeResponse attributeResponse = (RetrieveAttributeResponse)service.Execute(attributeRequest);
            return attributeResponse.AttributeMetadata.AttributeType.Value;
        }*/


        public AttributeTypeCode tipoAtributo(String _campo)
        {
            AttributeTypeCode _tipo = AttributeTypeCode.Virtual;
            if (ef.Attributes.ContainsKey(_campo))
            {
                String _nombreTipo = ef.Attributes[_campo].GetType().Name;
                if (ef.Attributes[_campo].GetType().Name == "AliasedValue")
                    _nombreTipo = ((AliasedValue)ef.Attributes[_campo]).Value.GetType().Name;

                switch (_nombreTipo)
                {
                    case "OptionSetValue": _tipo = AttributeTypeCode.Picklist;
                        break;
                    case "Money": _tipo = AttributeTypeCode.Money;
                        break;
                    case "EntityReference": _tipo = AttributeTypeCode.Lookup;
                        break;
                    case "DateTime": _tipo = AttributeTypeCode.DateTime;
                        break;
                    case "Decimal": _tipo = AttributeTypeCode.Decimal;
                        break;
                    case "Double": _tipo = AttributeTypeCode.Double;
                        break;
                    case "String": _tipo = AttributeTypeCode.String;
                        break;
                    case "Int64": _tipo = AttributeTypeCode.BigInt;
                        break;
                    case "Int32": _tipo = AttributeTypeCode.Integer;
                        break;
                    case "Boolean": _tipo = AttributeTypeCode.Boolean;
                        break;
                    case "Guid": _tipo = AttributeTypeCode.Uniqueidentifier;
                        break;
                }
            }
            
            return _tipo;

        }
        /*
		private String formateaAtributo(String _campo, AttributeTypeCode _tipoAtributo)
		{
			String _valorFormateado="";
			if (ef.Attributes.Contains(_campo))
            {
				if (_tipoAtributo == AttributeTypeCode.Lookup)
					_valorFormateado = ((EntityReference)ef.Attributes[_campo]).Id.ToString();
				else if (_tipoAtributo == AttributeTypeCode.Picklist)
					_valorFormateado = ef.FormattedValues[_campo];
				else if (_tipoAtributo == AttributeTypeCode.Money )
					_valorFormateado = ((Money) ef.Attributes[_campo]).Value.ToString();
				else if (_tipoAtributo == AttributeTypeCode.DateTime )
					_valorFormateado = ((DateTime) ef.Attributes[_campo]).ToString();
				else
                    _valorFormateado = ef.Attributes[_campo].ToString();
				}
			return _valorFormateado;
		}

        		
                private String valorAtributoForm(String _campo)
                {
                    String _value="";
                    if (ef.Attributes.Contains(_campo))
                    {
                        AttributeTypeCode _type = tipoAtributo( _campo );
                        return formateaAtributo(_campo, _type);
				
                    }
                    return _value;
                }

                private String valorAtributo(String _campo)
                {
                    String _value = "";
                    if (ef.Attributes.Contains(_campo))
                    {
                        AttributeTypeCode _type = tipoAtributo(_campo);

                        if (ef.Attributes.Contains(_campo))
                        {
                            if (_type == AttributeTypeCode.Lookup)
                                _value = ((EntityReference)ef.Attributes[_campo]).Id.ToString();
                            else if (_type == AttributeTypeCode.Money)
                                _value = ((Money)ef.Attributes[_campo]).Value.ToString();
                            else if (_type == AttributeTypeCode.DateTime)
                                _value = ((DateTime)ef.Attributes[_campo]).ToString();
                            else
                                _value = ef.Attributes[_campo].ToString();
                        }

                    }
                    return _value;
                }*/



        private String valorCampo(Object _objCampo, String _campo, AttributeTypeCode _tipoAtributo)
        {
            String _valor = String.Empty;

            switch (_tipoAtributo)
            {
                case AttributeTypeCode.Picklist:
                    _valor = ((OptionSetValue)_objCampo).Value.ToString();
                    break;
                case AttributeTypeCode.DateTime:
                    /*if (ef.FormattedValues.Contains(_campo))
                    {
                        _valor = DateTime.ParseExact(ef.FormattedValues[_campo], formatoFechaCompleto, System.Globalization.CultureInfo.InvariantCulture).ToString(formatoFecha);
                        //_valor = Convert.ToDateTime(ef.FormattedValues[_campo]).ToString(formatoFecha);
                    }
                    else
                    {*/
                    //IFormatProvider c_es_es = new System.Globalization.CultureInfo("es-ES");
                    
                    //_valor = ((DateTime)_objCampo).ToString(formatoFecha, c_es_es ); // System.Globalization.CultureInfo.CurrentCulture); //new System.Globalization.CultureInfo("es-ES"));
                    //}
                    //_valor = Convert.ToDateTime(ef.Attributes[_campo].ToString()).ToString(c_es_es); //formatoFecha);


                    TimeZoneInfo spZone = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
                    DateTime spTime = TimeZoneInfo.ConvertTimeFromUtc((DateTime)_objCampo, spZone);
                    _valor = spTime.ToString(formatoFecha);

                    
                    break;
                case AttributeTypeCode.Money:
                    _valor = ((Money)_objCampo).Value.ToString();
                    break;
                case AttributeTypeCode.Lookup:
                    _valor = ((EntityReference)_objCampo).Name;
                    break;
                default:
                    _valor = _objCampo.ToString();
                    break;
            }
            return _valor;
        }

        private String valorAlias(String _campo)
        {
            //String _valor = "";

            AliasedValue _valorAlias = (AliasedValue)ef.Attributes[_campo];
			return valorCampo(_valorAlias.Value, _campo, tipoAtributo( _campo ));

            /*
            switch (_valorAlias.Value.GetType().Name)
            {
                case "OptionSetValue":
                    _valor = ((OptionSetValue)_valorAlias.Value).Value.ToString();
                    break;
                case "DateTime":
                    if (ef.FormattedValues.Contains(_campo))
                    {
                        _valor = Convert.ToDateTime(ef.FormattedValues[_campo]).ToString(formatoFecha);
                    }
                    else
                    {
                        _valor = ((DateTime)_valorAlias.Value).ToString(formatoFecha);
                    }
                    break;
                case "Money":
                    _valor = ((Money)_valorAlias.Value).Value.ToString();
                    break;
                case "EntityReference":
                    _valor = ((EntityReference)_valorAlias.Value).Name;
                    break;

                default:
                    _valor = _valorAlias.Value.ToString();
                    break;
            }
            return _valor;*/
        }

        public String valorCampo(String _campo)
        {
            String _valor = "";
            if (ef.Attributes.ContainsKey(_campo))
            {
                if (ef.Attributes[_campo].GetType().Name == "AliasedValue")
                    return valorAlias(_campo);

                return valorCampo(ef.Attributes[_campo], _campo, tipoAtributo(_campo));

                /*switch (ef.Attributes[_campo].GetType().Name)
                {
                    case "OptionSetValue":
                        _valor = ((OptionSetValue)ef.Attributes[_campo]).Value.ToString();
                        break;
                    case "DateTime":
                        if (ef.FormattedValues.Contains(_campo))
                        {
                            _valor = Convert.ToDateTime(ef.FormattedValues[_campo]).ToString(formatoFecha);
                        }
                        else
                        {
                            _valor = ((DateTime)ef.Attributes[_campo]).ToString(formatoFecha);
                        }
                        break;
                    case "AliasedValue":
                        _valor = valorAlias( _campo);
                        break;
                    case "Money":
                        _valor = ((Money)ef.Attributes[_campo]).Value.ToString();
                        break;
                    case "EntityReference":
                        _valor = ((EntityReference)ef.Attributes[_campo]).Name;
                        break;
                    default:
                        _valor = ef.Attributes[_campo].ToString();
                        break;

                }*/
            }
            return _valor;
        }

        public String valorPicklist(String _campo)
        {

            String _valor = "";
            if (ef.Attributes.ContainsKey(_campo))
            {
                AttributeTypeCode _tipoAtributo = tipoAtributo(_campo);
                if (_tipoAtributo == AttributeTypeCode.Picklist)
                    return ef.FormattedValues[_campo];
                return valorCampo(_campo);
            }
            return _valor;
        }

		public Boolean contiene(String _campo)
		{
			//return ef.Attributes.Contains(_campo);
            return ef.Attributes.ContainsKey(_campo);

		}
    }
}
