using Microsoft.Xrm.Sdk;

namespace ExportarFichero.ServiceInfo
{
    public static class ServiceInfo
    {
        public static ITracingService tracingService { get; set; }
        public static IOrganizationService service { get; set; }
    }
}
