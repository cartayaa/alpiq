using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Web.Services.Protocols;

namespace Negocio.Logica
{
    public class GestionInstalaciones : NegocioBase
    {
        public GestionInstalaciones() :
            base()
        {

        }
        public GestionInstalaciones(IOrganizationService pServicioCrm) :
            base(pServicioCrm)
        {

        }

        public static Guid obtenerIdInstalacion(String pCUPS)
        {
            Guid salida = new Guid();
            try
            {

                QueryByAttribute consulta = new QueryByAttribute("atos_instalacion");

                consulta.ColumnSet = new ColumnSet("atos_instalacionid");
                consulta.Attributes.AddRange("atos_cups20", "atos_historico");
                consulta.Values.AddRange(pCUPS, false);

                EntityCollection resConsulta = ServicioCrm.RetrieveMultiple(consulta);

                if (resConsulta.Entities.Count > 0)
                {
                    salida = resConsulta.Entities[0].Id;
                }
            }
            catch (SoapException soapEx)
            {
                throw new Exception(String.Format("Error en la consulta de instalación:{0}", soapEx.Detail.InnerText));
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error en la consulta de instalación:{0}", ex.Message));

            }
            return salida;
        }
        public static Guid obtenerIdPuntoMedida(String pCodPM, Guid pIdInstalacion)
        {
            Guid salida = new Guid();
            try
            {

                QueryByAttribute consulta = new QueryByAttribute("atos_puntomedida");

                consulta.ColumnSet = new ColumnSet("atos_puntomedidaid");
                consulta.Attributes.AddRange("atos_codigopuntomedida", "atos_instalacionid");
                consulta.Values.AddRange(pCodPM, pIdInstalacion);

                EntityCollection resConsulta = ServicioCrm.RetrieveMultiple(consulta);

                if (resConsulta.Entities.Count > 0)
                {
                    salida = resConsulta.Entities[0].Id;
                }
            }
            catch (SoapException soapEx)
            {
                throw new Exception(String.Format("Error en la consulta de instalación del punto de medida:{0}", soapEx.Detail.InnerText));
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error en la consulta de instalación del punto de medida:{0}", ex.Message));

            }
            return salida;
        }


    }
}
