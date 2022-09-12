using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk;
using System.Globalization;

namespace Exportar
{

    public delegate String DelCalculo(EntidadCRM _ef);

    public class Funcion
    {
        private String funcion;
        private String formatoFecha;

        public DelCalculo calcula;

        public Funcion(String _funcion)
        {
            funcion = _funcion;
            this.asignaDelegado();
        }

        public String FormatoFecha
        {
            get
            {
                return this.formatoFecha;
            }
            set
            {
                this.formatoFecha = value;
            }
        }

        private void asignaDelegado()
        {
            //if (funcion.Length >= 4 && funcion.Substring(0, "AÑO(".Length) == "AÑO(")
            //    this.calcula = this.Anyo;
            if (funcion.Length >= 4 && funcion.Substring(0, "NVL(".Length) == "NVL(")
                this.calcula = this.Nvl;
            else if (funcion.Length >= 5 && funcion.Substring(0, "ANYO(".Length) == "ANYO(")
                this.calcula = this.Anyo;
            else if (funcion.Length >= 6 && funcion.Substring(0, "SELEC(".Length) == "SELEC(")
                this.calcula = this.Selec;
            else if (funcion.Length >= 6 && funcion.Substring(0, "FECHA(".Length) == "FECHA(")
                this.calcula = this.Fecha;
            else if (funcion.Length >= 7 && funcion.Substring(0, "OPERAR(".Length) == "OPERAR(")
                this.calcula = this.Operar;
            else if (funcion.Length >= 7 && funcion.Substring(0, "PKLIST(".Length) == "PKLIST(")
                this.calcula = this.PKList;
            else if (funcion.Length >= 8 && funcion.Substring(0, "SYSDATE(".Length) == "SYSDATE(")
                this.calcula = this.Sysdate;
            else if (funcion.Length >= 10 && funcion.Substring(0, "SUMATORIO(".Length) == "SUMATORIO(")
                this.calcula = this.Sumatorio;
            else if (funcion.Length >= 10 && funcion.Substring(0, "CONSTANTE(".Length) == "CONSTANTE(")
                this.calcula = this.Constante;
            else if (funcion.Length >= 11 && funcion.Substring(0, "CONCATENAR(".Length) == "CONCATENAR(")
                this.calcula = this.Concatenar;
            else if (funcion.Length >= 11 && funcion.Substring(0, "PORCENTAJE(".Length) == "PORCENTAJE(")
                this.calcula = this.Porcentaje;
            else if (funcion.Length >= 12 && funcion.Substring(0, "CORTACADENA(".Length) == "CORTACADENA(")
                this.calcula = this.CortaCadena;
            else if (funcion.Length >= 12 && funcion.Substring(0, "FORMACADENA(".Length) == "FORMACADENA(")
                this.calcula = this.FormaCadena;
            else if (funcion.Length >= 12 && funcion.Substring(0, "FORMATEANUM(".Length) == "FORMATEANUM(")
                this.calcula = this.FormateaNumero;
            else
                this.calcula = this.Nada;
                
        }

        public String Nada(EntidadCRM _ef)
        {
            String _valor = "";
            if (_ef.contiene(funcion))
                _valor = _ef.valorCampo(funcion); // _ef.valorAtributo(funcion);
            return _valor;
        }

        public String Sysdate(EntidadCRM _ef)
        {
            return DateTime.Now.ToString(formatoFecha);
        }

        public String Constante(EntidadCRM _ef)
        {
            const String FUNCION = "CONSTANTE(";
            if (funcion.Substring(0, FUNCION.Length) != FUNCION)
                throw new System.Exception("Error en la parametrización de la función CONSTANTE. [" + funcion + "]");
            return funcion.Remove(0, FUNCION.Length).TrimEnd(')');
        }

        public String Fecha(EntidadCRM _ef)
        {
            const String FUNCION = "FECHA(";
            if (funcion.Substring(0, FUNCION.Length) != FUNCION)
                throw new System.Exception("Error en la parametrización de la función FECHA. [" + funcion + "]");
            String _nbCampo = funcion.Remove(0, FUNCION.Length).TrimEnd(')');
            if (_ef.contiene(_nbCampo))
                return _ef.valorCampo(_nbCampo);
            return String.Empty;
        }

        public String PKList(EntidadCRM _ef)
        {
            const String FUNCION = "PKLIST(";
            if (funcion.Substring(0, FUNCION.Length) != FUNCION)
                throw new System.Exception("Error en la parametrización de la función PKLIST. [" + funcion + "]");
            String _nbCampo = funcion.Remove(0, FUNCION.Length).TrimEnd(')');

            if ( _ef.tipoAtributo(_nbCampo) == AttributeTypeCode.Picklist )
                if (_ef.contiene(_nbCampo))
                    return _ef.valorPicklist(_nbCampo);
            return String.Empty;
        }

        public String Selec(EntidadCRM _ef)
        {
            const String FUNCION = "SELEC(";
            if (funcion.Substring(0, FUNCION.Length) != FUNCION)
                throw new System.Exception("Error en la parametrización de la función SELEC. [" + funcion + "]");

            String _nbFuncion = funcion.Substring(0, FUNCION.Length - 1);
            String[] _parametros = funcion.Remove(funcion.Length - 1).Remove(0, FUNCION.Length).Split(',');

            if (_parametros.Length % 2 != 1 || _parametros.Length < 3)
                throw new System.Exception("Número incorrecto de parámetros en la función SELEC (impar mayor o igual a 3)");

            String _valor = "";
            if (_ef.contiene(_parametros[0]))
                _valor = _ef.valorCampo(_parametros[0]);

            for (int i = 1; i < _parametros.Length - 1; i += 2)
            {
                if (_valor == _parametros[i])
                    return /*(i+1).ToString() + " " +*/ _parametros[i + 1];
            }
            return ""; // funcion.Remove(funcion.Length - 1).Remove(0, FUNCION.Length); //""; _valor; 
        }

        public String Anyo(EntidadCRM _ef)
        {
            //const String FUNCION = "AÑO(";
            const String FUNCION = "ANYO(";
            if (funcion.Substring(0, FUNCION.Length) != FUNCION)
                throw new System.Exception("Error en la parametrización de la función ANYO. [" + funcion + "]");

            String _nbCampo = funcion.Remove(0, FUNCION.Length).TrimEnd(')');

            if (_ef.contiene(_nbCampo))
            {
                if (_ef.tipoAtributo(_nbCampo) != AttributeTypeCode.DateTime)
                    throw new System.Exception("El parámetro campo debe ser de tipo DateTime en la función AÑO");

                TimeZoneInfo spZone = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
                DateTime spTime = TimeZoneInfo.ConvertTimeFromUtc((DateTime)_ef.Ef.Attributes[_nbCampo], spZone);
                return spTime.ToString("yyyy");

                //return _ef.valorCampo(_nbCampo).Substring(0, 4);
            }
                
            return String.Empty;

        }

        public String Nvl(EntidadCRM _ef)
        {
            const String FUNCION = "NVL(";
            if (funcion.Substring(0, FUNCION.Length) != FUNCION)
                throw new System.Exception("Error en la parametrización de la función NVL. [" + funcion + "]");

            String[] _parametros = funcion.Remove(funcion.Length - 1).Remove(0, FUNCION.Length).Split(',');

            if (_parametros.Length != 2)
                throw new System.Exception("La función NVL debe tener dos parámetros, el campo del que devuelve el valor si no es nulo y el campo del que devuelve su valor si el anterior es nulo.");

            String _valor = "";
            if (_ef.contiene(_parametros[0]))
                _valor = _ef.valorCampo(_parametros[0]);

            if (_valor == "")
            {
                if (_ef.contiene(_parametros[1]))
                    _valor = _ef.valorCampo(_parametros[1]);
            }

            return _valor; 
        }

        public double convierteNumero(EntidadCRM _ef, String _campo, AttributeTypeCode _tipoAtributo)
        {
            double _numero = 0;
            if (_tipoAtributo == AttributeTypeCode.Money)
                _numero += Convert.ToDouble((decimal)((Money)_ef.Ef.Attributes[_campo]).Value);
            else if (_tipoAtributo == AttributeTypeCode.Decimal)
                _numero +=  Convert.ToDouble((Decimal)_ef.Ef.Attributes[_campo]);
            else if (_tipoAtributo == AttributeTypeCode.Integer)
                _numero += (int)_ef.Ef.Attributes[_campo];
            else if (_tipoAtributo == AttributeTypeCode.Double)
                _numero += (double)_ef.Ef.Attributes[_campo];
            else if (_tipoAtributo == AttributeTypeCode.BigInt)
                _numero += Convert.ToDouble( (long) _ef.Ef.Attributes[_campo]);
            return _numero;
        }

        public String Operar(EntidadCRM _ef)
        {
            const String FUNCION = "OPERAR(";
            String _valor = "";
            if (funcion.Substring(0, FUNCION.Length) != FUNCION)
                throw new System.Exception("Error en la parametrización de la función OPERAR. [" + funcion + "]");

            String[] _parametros = funcion.Remove(funcion.Length - 1).Remove(0, FUNCION.Length).Split(',');
            if (_parametros.Length != 3)
                throw new System.Exception("Número incorrecto de parámetros en la función OPERAR.");

            if (!_ef.contiene(_parametros[0]))
                return _valor;

            AttributeTypeCode _tipoAtributo = _ef.tipoAtributo(_parametros[0]);
            double _val = convierteNumero(_ef, _parametros[0], _tipoAtributo);

            switch (_parametros[1])
            {
                case "*":
                    _valor = (_val * double.Parse(_parametros[2])).ToString();
                    break;
                case "+":
                    _valor = (_val + double.Parse(_parametros[2])).ToString();
                    break;

                case "-":
                    _valor = (_val - double.Parse(_parametros[2])).ToString();
                    break;

                case "/":
                    _valor = (_val / double.Parse(_parametros[2])).ToString();
                    break;
                case "%":
                    _valor = (_val % double.Parse(_parametros[2])).ToString();
                    break;
            }
            return _valor;
        }

        public String Sumatorio(EntidadCRM _ef)
        {
            const String FUNCION = "SUMATORIO(";

            if (funcion.Substring(0, FUNCION.Length) != FUNCION)
                throw new System.Exception("Error en la parametrización de la función SUMATORIO. [" + funcion + "]");

            String[] _parametros = funcion.Remove(funcion.Length - 1).Remove(0, FUNCION.Length).Split(',');
            if (_parametros.Length < 2)
                throw new System.Exception("Número incorrecto de parámetros en la función SUMATORIO.");

            double _total = 0;
            for (int i = 0; i < _parametros.Length; i++)
            {
                if (_ef.contiene(_parametros[i]))
                {
                    AttributeTypeCode _tipoAtributo = _ef.tipoAtributo(_parametros[i]);
                    _total += convierteNumero(_ef, _parametros[i], _tipoAtributo);
                }
            }
            return _total.ToString();
        }

        public String Concatenar(EntidadCRM _ef)
		{
			const String FUNCION = "CONCATENAR(";
			
			if ( funcion.Substring(0,FUNCION.Length) != FUNCION )
				throw new System.Exception("Error en la parametrización de la función CONCATENAR. [" + funcion + "]");
			
			String [] _parametros = funcion.Remove(funcion.Length - 1).Remove(0, FUNCION.Length).Split(',');
			if (_parametros.Length != 3)
				throw new System.Exception("Número incorrecto de parámetros en la función CONCATENAR.");
			if ( _ef.contiene(_parametros[0]) && _ef.contiene(_parametros[1]) )
				return _ef.valorCampo(_parametros[0]) + _parametros[2] + _ef.valorCampo(_parametros[1]);
			if ( _ef.contiene(_parametros[0]) )
				return _ef.valorCampo(_parametros[0]);
			if ( _ef.contiene(_parametros[1]) )
				return _ef.valorCampo(_parametros[1]);
			return "";
		}
        
		public String CortaCadena( EntidadCRM _ef)
        {
			const String FUNCION = "CORTACADENA(";
            String _salida = "";
            String [] _parametros = funcion.Remove(funcion.Length - 1).Remove(0, FUNCION.Length).Split(',');

            if (_parametros.Length != 3) 
				throw new Exception("Número incorrecto de parámetros en la función CORTACADENA.");
            if (_ef.Ef.Attributes.ContainsKey(_parametros[0]))
            {
                String _valorCampo = _ef.valorCampo(_parametros[0]);

                Char _caracterSeparador = Convert.ToChar(_parametros[1]);
                int _elemento = Convert.ToInt32(_parametros[2]);

                _salida = _valorCampo.Split(new Char[] { _caracterSeparador })[_elemento];
            }

            return _salida;
        }

        private String FormaCadena(EntidadCRM _ef)
        {
			const String FUNCION = "FORMACADENA(";
			if (funcion.Substring(0, FUNCION.Length) != FUNCION)
				throw new System.Exception("Error en la parametrización de la función FORMACADENA. [" + funcion + "]");
				
            //sintaxis funcion : FORMACADENA(separador,campo1...campon)
            // campoi: si va entre "" se considera literal
            //          en caso contrario se considera campo de result.
            String _salida = String.Empty;
            String[] _parametros = funcion.Remove(0, FUNCION.Length).TrimEnd(')').Split(new Char[] { ',' });

            String _separador = _parametros[0];

            for (int i = 1; i <= _parametros.Length - 1; i++)
            {
                String _valorParametro = String.Empty;

                if (_parametros[i].StartsWith("'"))
                    _valorParametro = _parametros[i].Remove(_parametros[i].Length - 1).Remove(0, 1);
                else
                    _valorParametro = _ef.valorCampo( _parametros[i]);
				
                _salida = _salida + _valorParametro;
                if (i < _parametros.Length-1)
                    _salida = _salida + _separador;
            }

            return _salida;
        }

        private String FormateaNumero(EntidadCRM _ef)
        {
            const String FUNCION = "FORMATEANUM(";
            if (funcion.Substring(0, FUNCION.Length) != FUNCION)
                throw new System.Exception("Error en la parametrización de la función FORMATEANUM. [" + funcion + "]");

            //sintaxis funcion : FORMATEANUM(campo,numerodecimales)

            String[] _parametros = funcion.Remove(funcion.Length - 1).Remove(0, FUNCION.Length).Split(',');
            if (_parametros.Length < 2 || _parametros.Length > 3)
                throw new System.Exception("Número incorrecto de parámetros en la función FORMATEANUM.");


            AttributeTypeCode _tipoAtributo = _ef.tipoAtributo(_parametros[0]);
            double _valor = convierteNumero(_ef, _parametros[0], _tipoAtributo);

            String _formato = _parametros[1];
            // El formato normal de porcentaje que es con un % al final, además de multiplicar por 100 (automáticamente) le añade el % al final de la cadena formateada
            // Si el formato lo comenzamos con P (en ese caso no deberiamos poner el % al final) lo único que hacemos es multiplicar por 100
            // 0.00% -> valor = 0,153 se traduce en 15,3%
            // P0.00 -> valor = 0,153 se traduce en 15,3
            // P0.00% daría un resultado erróneo, la P multiplicaría por 100 y el % final también -> valor = 0,153 se traduce en 1530,0%
            if (_formato.Substring(0, 1) == "P" && _formato.Length > 1)
            {
                _formato = _formato.Remove(0, 1);
                _valor *= 100;
            }
            _formato = _formato.Replace("M", ","); // Como la , se usa como separador de los parámetros, si se quiere usar el formato con separador de miles se usa la M
            String _salida = String.Empty;

            if (_parametros.Length > 2)
                _salida = _valor.ToString(_formato, CultureInfo.CreateSpecificCulture(_parametros[2]));
            else
                _salida = _valor.ToString(_formato);

            /*String _formato = "{0:0";

            int numerodecimales = Convert.ToInt32(_parametros[1]);
            if (numerodecimales > 0)
            {
                _formato += ".";
                for (int i = 0; i < numerodecimales; i++)
                    _formato += "0";
            }
            _formato += "}";

            String _salida = String.Empty;
            _salida = String.Format(_formato, _valor);*/

            return _salida;
        }

        private String Porcentaje(EntidadCRM _ef)
        {
            const String FUNCION = "PORCENTAJE(";
            if (funcion.Substring(0, FUNCION.Length) != FUNCION)
                throw new System.Exception("Error en la parametrización de la función PORCENTAJE. [" + funcion + "]");

            //sintaxis funcion : FORMATEANUM(campo,numerodecimales)

            String[] _parametros = funcion.Remove(funcion.Length - 1).Remove(0, FUNCION.Length).Split(',');
            if (_parametros.Length != 2)
                throw new System.Exception("Número incorrecto de parámetros en la función PORCENTAJE.");


            AttributeTypeCode _tipoAtributo = _ef.tipoAtributo(_parametros[0]);
            double _valor = convierteNumero(_ef, _parametros[0], _tipoAtributo) * 100;
            String _formato = "{0:0";

            int numerodecimales = Convert.ToInt32(_parametros[1]);
            if (numerodecimales > 0)
            {
                _formato += ".";
                for (int i = 0; i < numerodecimales; i++)
                    _formato += "0";
            }
            _formato += "}";

            String _salida = String.Empty;
            _salida = String.Format(_formato, _valor);

            return _salida;
        }
    }
}
