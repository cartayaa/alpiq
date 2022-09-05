using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Comun.Negocio.Constantes;

namespace Comun.Negocio.Utilidades
{
    public class ErrorHandler
    {
        public static string UnfoldInnerExceptions(Exception ex)
        {
            if (ex.InnerException != null)
                return ex.Message + "\n" + UnfoldInnerExceptions(ex.InnerException);
            return ex.Message;
        }

        public static string GetErrorMessage(XmlDocument xDoc, string metodo)
        {
            string proceso = xDoc.GetElementsByTagName(Configuracion.XmlCodigoProceso)[0].InnerText;
            string paso = xDoc.GetElementsByTagName(Configuracion.XmlCodigoPaso)[0].InnerText;
            return string.Format("Error procesando Proceso:{0} Paso:{1} Método:{2}\n", proceso, paso, metodo);
        }

        public static string ConstructErrorMessage(XmlDocument xDoc, string metodo, Exception ex)
        {
            return ErrorHandler.GetErrorMessage(xDoc, metodo) + "\n" + UnfoldInnerExceptions(ex);
        }
    }
}
