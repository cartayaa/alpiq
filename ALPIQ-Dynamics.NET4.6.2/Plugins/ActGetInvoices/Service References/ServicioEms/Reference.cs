﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ServicioEms {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://webclientes.ws.atos.com/", ConfigurationName="ServicioEms.WebClientes")]
    public interface WebClientes {
        
        // CODEGEN: Generating message contract since element name arg0 from namespace  is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        ServicioEms.getPdfFacturaResponse getPdfFactura(ServicioEms.getPdfFactura request);
        
        // CODEGEN: Generating message contract since element name arg0 from namespace  is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        ServicioEms.accesoWebPublicaResponse accesoWebPublica(ServicioEms.accesoWebPublica request);
        
        // CODEGEN: Generating message contract since element name arg0 from namespace  is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        ServicioEms.solicitudMedidasResponse solicitudMedidas(ServicioEms.solicitudMedidas request);
        
        // CODEGEN: Generating message contract since element name arg0 from namespace  is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="", ReplyAction="*")]
        ServicioEms.solicitudFacturaResponse solicitudFactura(ServicioEms.solicitudFactura request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class getPdfFactura {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="getPdfFactura", Namespace="http://webclientes.ws.atos.com/", Order=0)]
        public ServicioEms.getPdfFacturaBody Body;
        
        public getPdfFactura() {
        }
        
        public getPdfFactura(ServicioEms.getPdfFacturaBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="")]
    public partial class getPdfFacturaBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string arg0;
        
        public getPdfFacturaBody() {
        }
        
        public getPdfFacturaBody(string arg0) {
            this.arg0 = arg0;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class getPdfFacturaResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="getPdfFacturaResponse", Namespace="http://webclientes.ws.atos.com/", Order=0)]
        public ServicioEms.getPdfFacturaResponseBody Body;
        
        public getPdfFacturaResponse() {
        }
        
        public getPdfFacturaResponse(ServicioEms.getPdfFacturaResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="")]
    public partial class getPdfFacturaResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public byte[] @return;
        
        public getPdfFacturaResponseBody() {
        }
        
        public getPdfFacturaResponseBody(byte[] @return) {
            this.@return = @return;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class accesoWebPublica {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="accesoWebPublica", Namespace="http://webclientes.ws.atos.com/", Order=0)]
        public ServicioEms.accesoWebPublicaBody Body;
        
        public accesoWebPublica() {
        }
        
        public accesoWebPublica(ServicioEms.accesoWebPublicaBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="")]
    public partial class accesoWebPublicaBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string arg0;
        
        public accesoWebPublicaBody() {
        }
        
        public accesoWebPublicaBody(string arg0) {
            this.arg0 = arg0;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class accesoWebPublicaResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="accesoWebPublicaResponse", Namespace="http://webclientes.ws.atos.com/", Order=0)]
        public ServicioEms.accesoWebPublicaResponseBody Body;
        
        public accesoWebPublicaResponse() {
        }
        
        public accesoWebPublicaResponse(ServicioEms.accesoWebPublicaResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="")]
    public partial class accesoWebPublicaResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string @return;
        
        public accesoWebPublicaResponseBody() {
        }
        
        public accesoWebPublicaResponseBody(string @return) {
            this.@return = @return;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class solicitudMedidas {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="solicitudMedidas", Namespace="http://webclientes.ws.atos.com/", Order=0)]
        public ServicioEms.solicitudMedidasBody Body;
        
        public solicitudMedidas() {
        }
        
        public solicitudMedidas(ServicioEms.solicitudMedidasBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="")]
    public partial class solicitudMedidasBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string arg0;
        
        public solicitudMedidasBody() {
        }
        
        public solicitudMedidasBody(string arg0) {
            this.arg0 = arg0;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class solicitudMedidasResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="solicitudMedidasResponse", Namespace="http://webclientes.ws.atos.com/", Order=0)]
        public ServicioEms.solicitudMedidasResponseBody Body;
        
        public solicitudMedidasResponse() {
        }
        
        public solicitudMedidasResponse(ServicioEms.solicitudMedidasResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="")]
    public partial class solicitudMedidasResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string @return;
        
        public solicitudMedidasResponseBody() {
        }
        
        public solicitudMedidasResponseBody(string @return) {
            this.@return = @return;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class solicitudFactura {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="solicitudFactura", Namespace="http://webclientes.ws.atos.com/", Order=0)]
        public ServicioEms.solicitudFacturaBody Body;
        
        public solicitudFactura() {
        }
        
        public solicitudFactura(ServicioEms.solicitudFacturaBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="")]
    public partial class solicitudFacturaBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string arg0;
        
        public solicitudFacturaBody() {
        }
        
        public solicitudFacturaBody(string arg0) {
            this.arg0 = arg0;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class solicitudFacturaResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="solicitudFacturaResponse", Namespace="http://webclientes.ws.atos.com/", Order=0)]
        public ServicioEms.solicitudFacturaResponseBody Body;
        
        public solicitudFacturaResponse() {
        }
        
        public solicitudFacturaResponse(ServicioEms.solicitudFacturaResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="")]
    public partial class solicitudFacturaResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string @return;
        
        public solicitudFacturaResponseBody() {
        }
        
        public solicitudFacturaResponseBody(string @return) {
            this.@return = @return;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface WebClientesChannel : ServicioEms.WebClientes, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class WebClientesClient : System.ServiceModel.ClientBase<ServicioEms.WebClientes>, ServicioEms.WebClientes {
        
        public WebClientesClient() {
        }
        
        public WebClientesClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public WebClientesClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WebClientesClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WebClientesClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ServicioEms.getPdfFacturaResponse ServicioEms.WebClientes.getPdfFactura(ServicioEms.getPdfFactura request) {
            return base.Channel.getPdfFactura(request);
        }
        
        public byte[] getPdfFactura(string arg0) {
            ServicioEms.getPdfFactura inValue = new ServicioEms.getPdfFactura();
            inValue.Body = new ServicioEms.getPdfFacturaBody();
            inValue.Body.arg0 = arg0;
            ServicioEms.getPdfFacturaResponse retVal = ((ServicioEms.WebClientes)(this)).getPdfFactura(inValue);
            return retVal.Body.@return;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ServicioEms.accesoWebPublicaResponse ServicioEms.WebClientes.accesoWebPublica(ServicioEms.accesoWebPublica request) {
            return base.Channel.accesoWebPublica(request);
        }
        
        public string accesoWebPublica(string arg0) {
            ServicioEms.accesoWebPublica inValue = new ServicioEms.accesoWebPublica();
            inValue.Body = new ServicioEms.accesoWebPublicaBody();
            inValue.Body.arg0 = arg0;
            ServicioEms.accesoWebPublicaResponse retVal = ((ServicioEms.WebClientes)(this)).accesoWebPublica(inValue);
            return retVal.Body.@return;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ServicioEms.solicitudMedidasResponse ServicioEms.WebClientes.solicitudMedidas(ServicioEms.solicitudMedidas request) {
            return base.Channel.solicitudMedidas(request);
        }
        
        public string solicitudMedidas(string arg0) {
            ServicioEms.solicitudMedidas inValue = new ServicioEms.solicitudMedidas();
            inValue.Body = new ServicioEms.solicitudMedidasBody();
            inValue.Body.arg0 = arg0;
            ServicioEms.solicitudMedidasResponse retVal = ((ServicioEms.WebClientes)(this)).solicitudMedidas(inValue);
            return retVal.Body.@return;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ServicioEms.solicitudFacturaResponse ServicioEms.WebClientes.solicitudFactura(ServicioEms.solicitudFactura request) {
            return base.Channel.solicitudFactura(request);
        }
        
        public string solicitudFactura(string arg0) {
            ServicioEms.solicitudFactura inValue = new ServicioEms.solicitudFactura();
            inValue.Body = new ServicioEms.solicitudFacturaBody();
            inValue.Body.arg0 = arg0;
            ServicioEms.solicitudFacturaResponse retVal = ((ServicioEms.WebClientes)(this)).solicitudFactura(inValue);
            return retVal.Body.@return;
        }
    }
}
