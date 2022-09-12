using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exportar
{
    public class Cabecera
    {
        private Columna[] columnas;

        public Cabecera(String _cabecera)
        {
            String [] _cols = _cabecera.Split(';');
            columnas = new Columna[_cols.Length];

            for (int i = 0; i < _cols.Length; i++)
                columnas[i] = new Columna(_cols[i]);
        }
        public int len()
        {
            return columnas.Length;
        }
        public Columna col(int i)
        {
            return columnas[i];
        }
    }
}
