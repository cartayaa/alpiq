using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exportar
{
    public class Columna
    {
        private String nbColumna;
        private Funcion calculo;

        public Columna(String _columna)
        {
            String[] _componentes = _columna.Split(':');
            nbColumna = _componentes[0];
            calculo = new Funcion(_componentes[1]);
        }
        public String NbColumna
        {
            get
            {
                return this.nbColumna;
            }
        }
        public Funcion Calculo
        {
            get
            {
                return this.calculo;
            }
        }
    }
}
