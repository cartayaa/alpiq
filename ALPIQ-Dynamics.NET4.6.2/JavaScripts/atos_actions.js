if (typeof (SDK) == "undefined")
{ SDK = { __namespace: true }; }
SDK.Action = {
    _getClientUrl: function () {
        var ServicePath = "/XRMServices/2011/Organization.svc/web";
        var clientUrl = "";
        if (typeof GetGlobalContext == "function") {
            var context = GetGlobalContext();
            clientUrl = context.getClientUrl();
        }
        else {
            if (typeof Xrm.Page.context == "object") {
                clientUrl = Xrm.Page.context.getClientUrl();
            }
            else
            { throw new Error("Unable to access the server URL"); }
        }
        if (clientUrl.match(/\/$/)) {
            clientUrl = clientUrl.substring(0, clientUrl.length - 1);
        }
        return clientUrl + ServicePath;
    },
    parseResponse: function (httpRequest) {

        var xmlDoc = httpRequest.responseXML;

        if (!xmlDoc || !xmlDoc.documentElement) {
            if (window.DOMParser) {
                var parser = new DOMParser();
                try {
                    xmlDoc = parser.parseFromString(httpRequest.responseText, "text/xml");
                } catch (e) {
                    alert("XML parsing error");
                    return null;
                };
            }
            else {
                xmlDoc = CreateMSXMLDocumentObject();
                if (!xmlDoc) {
                    return null;
                }
                xmlDoc.loadXML(httpRequest.responseText);

            }
        }

        var errorMsg = null;
        if (xmlDoc.parseError && xmlDoc.parseError.errorCode != 0) {
            errorMsg = "XML Parsing Error: " + xmlDoc.parseError.reason
					  + " at line " + xmlDoc.parseError.line
					  + " at position " + xmlDoc.parseError.linepos;
        }
        else {
            if (xmlDoc.documentElement) {
                if (xmlDoc.documentElement.nodeName == "parsererror") {
                    errorMsg = xmlDoc.documentElement.childNodes[0].nodeValue;
                }
            }
        }
        if (errorMsg) {
            alert(errorMsg);
            return null;
        }

        return xmlDoc;
    },
    GetInvoices: function (idsInvoices) {
        //alert("SDK.Action.GetInvoices: " + idsInvoices);
        var requestMain = ""
        requestMain += "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">";
        requestMain += "  <s:Body>";
        requestMain += "    <Execute xmlns=\"http://schemas.microsoft.com/xrm/2011/Contracts/Services\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">";
        requestMain += "      <request xmlns:a=\"http://schemas.microsoft.com/xrm/2011/Contracts\">";
        requestMain += "        <a:Parameters xmlns:b=\"http://schemas.datacontract.org/2004/07/System.Collections.Generic\">";
        requestMain += "          <a:KeyValuePairOfstringanyType>";
        requestMain += "            <b:key>IdsInvoices</b:key>";
        requestMain += "            <b:value i:type='c:string' xmlns:c='http://www.w3.org/2001/XMLSchema' >" + idsInvoices + "</b:value>";
        requestMain += "          </a:KeyValuePairOfstringanyType>";
        requestMain += "        </a:Parameters>";
        requestMain += "        <a:RequestId i:nil=\"true\" />";
        requestMain += "        <a:RequestName>atos_GetInvoices</a:RequestName>";
        requestMain += "      </request>";
        requestMain += "    </Execute>";
        requestMain += "  </s:Body>";
        requestMain += "</s:Envelope>";
        //alert("requestMain: " + requestMain);
        var req = new XMLHttpRequest();
        var url = SDK.Action._getClientUrl();
        //alert('url: ' + url);
        req.open("POST", url, false)
        req.setRequestHeader("Accept", "application/xml, text/xml, */*");
        req.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
        req.setRequestHeader("SOAPAction", "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Execute");
        req.send(requestMain);
        //work with the response here
        //var strResponse = req.responseXML.xml;
        //debugger;
        var xmlRespuesta = null;
        var xmltext = "";
        if (req.responseXML == null) {
            alert('No se ha podido recuperar la factura');
        }
        else if (req.responseXML.xml) {
            //alert('XML: ' + req.responseXML.xml);
            xmltext = req.responseXML.xml;
            prompt('xmltext1: ', xmltext);
        }
        else if (XMLSerializer) {
            var xml_serializer = new XMLSerializer();
            xmltext = xml_serializer.serializeToString(req.responseXML);
            //prompt('xmltext2: ', xmltext);
            //alert('XML serializer: ' + xmltext);
        }
        else {
            alert('Error en respuesta del XML');
        }
        if (xmltext != "") {
            var xmlrespuesta;
            if (typeof window.DOMParser != "undefined") {
                xmlrespuesta = (new window.DOMParser()).parseFromString(xmltext, "text/xml");
            }
            else if (typeof window.ActiveXObject != "undefined" &&
						new window.ActiveXObject("Microsoft.XMLDOM")) {

                xmlrespuesta = new window.ActiveXObject("Microsoft.XMLDOM");
                xmlrespuesta.async = "false";
                xmlrespuesta.loadXML(xmltext);
            }

            //alert("childnodes0: " + xmlrespuesta.documentElement.childNodes[0].childNodes[0].childNodes[0].childNodes[1].childNodes[0].childNodes[0].textContent);
            //alert("childnodes1: " + xmlrespuesta.documentElement.childNodes[0].childNodes[0].childNodes[0].childNodes[1].childNodes[0].childNodes[1].textContent);
            var url = xmlrespuesta.documentElement.childNodes[0].childNodes[0].childNodes[0].childNodes[1].childNodes[0].childNodes[1].textContent;
            var mensaje = xmlrespuesta.documentElement.childNodes[0].childNodes[0].childNodes[0].childNodes[1].childNodes[1].childNodes[1].textContent;
            
            if (mensaje != "")
                alert(mensaje);

            if (url != "")
                window.open(url);
        }

        /*var urlzip = req.responseXML.children(":first").children(":first").children(":first").children(":first").children("a\\:Results").children("a\\:KeyValuePairOfstringanyType").children("b\\:value").text();
        alert('Urlzip: ' + urlzip);*/

    },
    __namespace: true
};

function getInvoices(idsInvoices) {
    //alert("getInvoices: " + idsInvoices);
    SDK.Action.GetInvoices(idsInvoices);
}