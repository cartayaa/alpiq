using System;
using System.Xml;

namespace Negocio.Logica
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
            string proceso = xDoc.GetElementsByTagName("CodigoDelProceso")[0].InnerText;
            string paso = xDoc.GetElementsByTagName("CodigoDePaso")[0].InnerText;
            return string.Format("Error procesando Proceso:{0} Paso:{1} Método:{2}", proceso, paso, metodo);
        }

        public static string ConstructErrorMessage(XmlDocument xDoc, string metodo, Exception ex)
        {
            return ErrorHandler.GetErrorMessage(xDoc, metodo) + "\n" + UnfoldInnerExceptions(ex);
        }
    }
}
