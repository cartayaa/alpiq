function WarningEMS(executionContext)
{
	var formContext = executionContext.getFormContext();

	if ( formContext.data.entity.getId() != null && formContext.data.entity.getId() != "" )
	{
		if ( formContext.data.entity.attributes.get("atos_interfazpricingoutputems").getValue() == 300000000 ) // KO EMS
		{
			formContext.ui.setFormNotification("La última ejecución del WS de Pricing Output con EMS ha finalizado con errores", "WARNING", "1");
		}
				
	}
}