using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using System.Net;

namespace InformesAlpiq
{
    class Program
    {
        private static IOrganizationService _servicioCRM;
     
        static int Main(string[] args)
        {
            //string uriWS_llamada = "http://maquinavirtual678.westeurope.cloudapp.azure.com:1025/";
            string uriWS_llamada = "http://localhost:56529/Alpiq.asmx";
            string uriWS_recogerFichero = "http://maquinavirtual678.westeurope.cloudapp.azure.com:1025/";
            string guidOferta = "{47EE5ED8-356D-E711-80E5-3863BB35FF48}";
            string TipoInforme = "CondicionesContrato";
           string  esPDF = "true";
           bool   generarLog = false ;
            try
                {
                    string result = CommonWS.CommonWS.GenerarInforme(uriWS_llamada, guidOferta, uriWS_recogerFichero, TipoInforme, esPDF, generarLog);

                    if (result != null)
                    {
                        return 0; 
                    }
                    else
                    {
                        return -1;
                    }

                }
                catch (Exception ex)
                {
                    return -1;
                }
            
            return 0;
        }

        private static string serializarMensaje(List<String> listadoMensajes)
        {
            try
            {
                String mensaje = "";
                for (int i = 0; i < listadoMensajes.Count; i++)
                {
                    mensaje += listadoMensajes[i] + ".";

                }

                return mensaje;
            }
            catch (Exception ex)
            {
                return "error al serializar el mensaje de vuelta";
            }
        }

    }
}
