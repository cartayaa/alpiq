(function (global)
{
    "use strict";
    global.atosALPIQ = global.atosALPIQ || {};
    global.atosALPIQ.Ribbon = global.atosALPIQ.Ribbon || {};
    global.atosALPIQ.Ribbon.oferta = (function ()
    {
        /**
        * @function callWorkFlow
        * @description esta funcion se utilizara para realizar llamadas a workflows bajo demanda.
        */
        var mostrarBoton = false;
        var isPromiseCompleted = false;
        function callWorkFlow(primaryControl)
        {
            var baseUrl = primaryControl.context.getClientUrl();
            var version = primaryControl.context.getVersion().substring(0, 3);
            var workflowId = "3E2801B5-E7A1-44C6-8944-294EEDAC1A7F";
            var objectId = primaryControl.data.entity.getId();
            var entity = {
                "EntityId": objectId.substring(1, objectId.length - 1) // accountId
            };

            var req = new XMLHttpRequest();
            req.open("POST", baseUrl + "/api/data/v" + version + "/workflows(" + workflowId + ")/Microsoft.Dynamics.CRM.ExecuteWorkflow", true);
            req.setRequestHeader("OData-MaxVersion", "4.0");
            req.setRequestHeader("OData-Version", "4.0");
            req.setRequestHeader("Accept", "application/json");
            req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            req.onreadystatechange = function ()
            {
                if (this.readyState === 4)
                {
                    req.onreadystatechange = null;
                    switch (this.status)
                    {
                        case 200:
                        case 204:
                            var alertStrings = { confirmButtonLabel: "Ok", text: "Se ha ejecutado el workflow correctamente." };
                            var alertOptions = { height: 120, width: 260 };
                            Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                                function success(result)
                                {
                                    debugger;
                                    primaryControl.data.refresh();
                                    //The values can be active, aborted, or finished
                                    var status = "active";
                                    primaryControl.data.process.setStatus(status, setPreviousStage);
                                    //Movemos dos steps atras para dejarlo en la descicion.
                                    setTimeout(function ()
                                    {
                                        primaryControl.data.process.movePrevious();
                                        primaryControl.data.refresh();
                                    }, 500);
                                },
                                function (error)
                                {
                                    console.log(error.message);
                                }
                            );
                            break;
                        default:
                            var alertStrings = { confirmButtonLabel: "Ok", text: "Ha ocurrido un error. " + this.responseText };
                            var alertOptions = { height: 120, width: 260 };
                            Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                                function success(result)
                                {
                                    primaryControl.data.refresh();
                                },
                                function (error)
                                {
                                    console.log(error.message);
                                }
                            );
                            break;
                    }
                }
            };
            req.send(JSON.stringify(entity));
        }

        function setPreviousStage(newStatus)
        {
            if (newStatus == "active")
            {
                //no usa primaryControl porque se llamada desde el contexto del formulario y no desde de el del ribbon
                formContext.data.process.movePrevious();
            }
        }

        function showHideAbrirOfertaButton(formContext)
        {
            if (isPromiseCompleted)
                return mostrarBoton;

            var baseUrl = formContext.context.getClientUrl();
            var version = formContext.context.getVersion().split('.');

            //var fullUrl = baseUrl + "/api/data/v" + version[0] + "." + version[1] + "/roles?$select=roleid&$filter=name eq 'RI Oficina' or name eq 'RI gran cuenta' or name eq 'RI CGA'";
            //var fullUrl = baseUrl + "/api/data/v" + version[0] + "." + version[1] + "/roles?$select=roleid&$filter=name eq 'Borrar actividades completadas' or name eq 'borrar actividades completadas'";
            var fullUrl = baseUrl + "/api/data/v" + version[0] + "." + version[1] + "/roles?$select=roleid&$filter=contains(name,'Borrar actividades completada') or contains(name,'borrar actividades completada')";

            var rolesId = formContext.context.getUserRoles();

            executeGetHttpRequest(fullUrl).then(
                function (response)
                {
                    isPromiseCompleted = true;
                    for (var t = 0; t < response.value.length; t++)
                    {
                        for (var i = 0; i < rolesId.length; i++)
                        {
                            if (rolesId[i].toLowerCase() == response.value[t].roleid.toLowerCase())
                                mostrarBoton = true;
                        }
                    }
                    formContext.ui.refreshRibbon();
                }, function (error)
                {
                    debugger;
                    isPromiseCompleted = true;
                }
            );
            return false;
        }

        function executeGetHttpRequest(url)
        {
            return new Promise(function (resolve, reject)
            {
                var req = new XMLHttpRequest();
                req.open("GET", url, false);
                req.setRequestHeader("OData-MaxVersion", "4.0");
                req.setRequestHeader("OData-Version", "4.0");
                req.onreadystatechange = function ()
                {
                    if (this.readyState === 4)
                    {
                        req.onreadystatechange = null;
                        switch (this.status)
                        {
                            case 200:
                            case 204:
                                var result = JSON.parse(req.response);
                                resolve(result)
                                break;
                            default:
                                var error;
                                try
                                {
                                    error = JSON.parse(req.response).error;
                                }
                                catch (e)
                                {
                                    error = new Error("Unexpected Error");
                                }
                                reject(error);
                                break;
                        }
                    }
                };
                req.send();
            });
        }

        return {
            callWorkFlow: callWorkFlow,
            showHideAbrirOfertaButton: showHideAbrirOfertaButton
        };
    }());
}(this));