/**
// <summary>
// Oculta/muestra las pestaÃ±as correspondientes segÃºn el tipo de factura
// </summary>
// <remarks>
// Siempre se muestra la cabecera, conceptos repercutibles, registro fin, otros datos
// Si es tipo Factura ATR, se muestran secciones factura ATR, potencias, excesos, energia activa,
//      energia reactiva, conceptos de factura y las medidas
// Si es otro tipo de factura, solo se muestra otras facturas
// </remarks>
*/
function tab_tipo_factura(executionContext) {

    var formContext = executionContext.getFormContext();
    /*
    formContext.ui.tabs.get('tab_cabecera').setVisible(false);
    formContext.ui.tabs.get('tab_factura_atr').setVisible(false);
    formContext.ui.tabs.get('tab_potencia').setVisible(false);
    formContext.ui.tabs.get('tab_exceso_potencia').setVisible(false);
    formContext.ui.tabs.get('tab_energia_activa').setVisible(false);
    formContext.ui.tabs.get('tab_energia_reactiva').setVisible(false);
    formContext.ui.tabs.get('tab_conceptos_factura_atr').setVisible(false);
    formContext.ui.tabs.get('tab_otras_facturas').setVisible(false);
    formContext.ui.tabs.get('tab_conceptos_repercutibles').setVisible(false);
    formContext.ui.tabs.get('tab_medidas').setVisible(false);
    formContext.ui.tabs.get('tab_registro_fin').setVisible(false);
    formContext.ui.tabs.get('tab_otros_datos').setVisible(false);
    */

    var atos_facturatipoatr = formContext.data.entity.attributes.get("atos_facturatipoatr").getValue();
    if (atos_facturatipoatr == true) {
        formContext.ui.tabs.get('tab_factura_atr').setVisible(true);
        formContext.ui.tabs.get('tab_potencia').setVisible(true);
        formContext.ui.tabs.get('tab_exceso_potencia').setVisible(true);
        formContext.ui.tabs.get('tab_energia_activa').setVisible(true);
        formContext.ui.tabs.get('tab_energia_reactiva').setVisible(true);
        formContext.ui.tabs.get('tab_conceptos_factura_atr').setVisible(true);
        formContext.ui.tabs.get('tab_puntos_medida').setVisible(true);
    }
    else {
        formContext.ui.tabs.get('tab_otras_facturas').setVisible(true);
    }

    formContext.ui.tabs.get('tab_cabecera').setVisible(true);
    formContext.ui.tabs.get('tab_registro_fin').setVisible(true);
    formContext.ui.tabs.get('tab_otros_datos').setVisible(true);
    formContext.ui.tabs.get('tab_conceptos_repercutibles').setVisible(true);
}

    /*
    var codigoSolicitud = lookup[0].keyValues["atos_codigosolicitud"].value;
    var col = ["atos_codigosolicitud"];
    var solicitudatr = XrmServiceToolkit.Soap.Retrieve("atos_solicitudatr", lookup[0].id, col);
    if (solicitudatr.attributes["atos_codigosolicitud"] == null) { return; }
    var test = solicitudatr.attributes.atos_codigosolicitud.value;
    */