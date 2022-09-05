using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace Negocio.Logica
{
    public class NegocioBase
    {
        //private OrganizationServiceProxy _serviceProxy;

        private static IOrganizationService _servicioCrm;

        protected static IOrganizationService ServicioCrm
        {
            get { return _servicioCrm; }
            set { _servicioCrm = value; }
        }

        public NegocioBase()
        {

        }
        public NegocioBase(IOrganizationService pServicioCrm)
        {
            _servicioCrm = pServicioCrm;
        }
        
    }
}
