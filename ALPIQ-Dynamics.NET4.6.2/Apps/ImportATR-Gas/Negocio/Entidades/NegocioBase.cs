using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace Negocio.Entidades
{
    public class NegocioBase
    {
        // private OrganizationServiceProxy _serviceProxy = null;

        protected IOrganizationService ServicioCrm { get; set; }

        public NegocioBase(IOrganizationService servicioCrm)
        {
            ServicioCrm = servicioCrm;
        }        
    }
}
