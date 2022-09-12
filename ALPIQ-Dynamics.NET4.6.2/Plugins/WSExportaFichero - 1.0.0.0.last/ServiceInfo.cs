using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportarFichero.ServiceInfo
{
    public static class ServiceInfo
    {
        public static ITracingService tracingService { get; set; }
        public static IOrganizationService service { get; set; }
    }
}
