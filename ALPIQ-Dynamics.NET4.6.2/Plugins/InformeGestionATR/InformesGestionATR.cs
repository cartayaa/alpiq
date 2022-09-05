namespace InformeGestionATR
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using System.IO;
    using Microsoft.Xrm.Sdk.Query;
    using System.Diagnostics;
    using System.Text;
    using System.Xml.Linq;
    using System.Net;
    using System.Collections;

    using System.Collections.Generic;
    using System.Threading;


    public class InformesGestionATR : IPlugin
    {
        #region DECLARACIÓN DE CONSTANTES, VARIABLES Y ESTRUCTURAS DE DATOS

        struct objetoATR
        {
            public string provincia;
            public string distribuidor;
            public string comerEntrante;
            public string comerSaliente;
            public string tipoCambio;
            public string tipoPunto;
            public string tarifaATR;
            public string tipoRetraso;
            public string motivoRechazo;

            public int numeroSolicitudes;
        }

        /*  struct objetoContrato
          {
              public string provincia;
              public string distribuidor;
              public string comerEntrante;
              public string tarifaATR;

              public int numeroContratos;
          }*/

        private ITracingService tracingService;
        private IPluginExecutionContext PluginExecutionContext; ///< Contexto de ejecución del plugin
        private IOrganizationServiceFactory factory;
        private IOrganizationService service, servicioConsultas;

        private char SEPARADOR = '#';
        private Boolean createLog = false;
        private String rutaLOG = "";

        private Boolean hayErrores = false;

        private const String nombrePlantillaSI = "\\Acciona_InformeATR_SI.xlsx"; //Acciona_InformeATR_SI.xls
        private const String nombrePlantillaPS = "\\Acciona_InformeATR_PS.xlsx";
        private String URIsalida = "";
        private String rutaPlantilla = "";
        private String rutaSalidaArchivo = "";
        private const String codigoComercializadora = "R2-255";
        private const String codigoDistribuidora = "R1-xxx";
        private const int numeroFilasRecorrer = 728;

        private String periodo = "";
        private String tipoInforme = "";

        private DateTime fechaMinima;

        #endregion DECLARACIÓN DE CONSTANTES Y VARIABLES

        #region CONSTRUCTOR Y EXECUTE

        public InformesGestionATR(String parametros)
        {
            if (String.IsNullOrEmpty(parametros))
            {
                throw new InvalidPluginExecutionException("Conexión mal configurada.");
            }
            else
            {
                String[] arrayPar = parametros.Split(SEPARADOR);
                if (arrayPar[0] == "SI")
                {
                    createLog = true;
                    rutaLOG = (String)arrayPar[1];
                }
                if (arrayPar.Length > 1)
                {
                    rutaPlantilla = arrayPar[2] + @"Plantillas";
                    rutaSalidaArchivo = arrayPar[2] + @"InformesATR";
                }
                if (arrayPar.Length > 2)
                {
                    URIsalida = arrayPar[3];
                }
                if (arrayPar.Length > 3)
                {
                    //Fecha mínima de búsqueda de registros
                    //Para no buscar "desde el inicio de los tiempos" y evitar inconsistencias

                    String fechMin = arrayPar[4];
                    if (fechMin != null && fechMin != "")
                    {
                        String format = "dd-MM-yyyy";
                        IFormatProvider provider = new CultureInfo("es-ES");
                        String fecha = "01-" + fechMin.Substring(4, 2) + "-" + fechMin.Substring(0, 4);
                        fechaMinima = DateTime.ParseExact(fecha, format, provider);
                    }
                    else
                    {
                        fechaMinima = DateTime.Now;
                    }

                }
            }
        }


        public void Execute(IServiceProvider serviceProvider)
        {
            //Variables para actualizar caché
            String _rutaSalidaArchivo = rutaSalidaArchivo;
            String _rutaPlantilla = rutaPlantilla;

            tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            PluginExecutionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            //service = factory.CreateOrganizationService(PluginExecutionContext.InitiatingUserId);
            service = factory.CreateOrganizationService(PluginExecutionContext.UserId);
            servicioConsultas = factory.CreateOrganizationService(PluginExecutionContext.UserId);

            // Use the factory to generate the Organization Service.
            //service = factory.CreateOrganizationService(PluginExecutionContext.UserId);

            //variables iniciales 
            DateTime fecha = DateTime.Now.ToLocalTime();
            String sFecha = fecha.ToString("yyyyMMdd_hhMss");
            String mensajeDeError = "";
            String aux_rutaSalidaArchivo = "";
            String nuevoNombre = "";
            writelog("------------------------------------------------------------");
            writelog(fecha.ToString());
            writelog("Plugin Informes Gestión ATR");
            //writelog("Mensaje: " + PluginExecutionContext.MessageName);

            //Control de errores iniciales
            if ((PluginExecutionContext.MessageName != "atos_InfGestionATR") || (_rutaPlantilla == "") || (_rutaSalidaArchivo == ""))
            {
                writelog("Genero Error");
                return;
            }

            periodo = PluginExecutionContext.InputParameters["periodoGestionATR"] as String;
            tipoInforme = PluginExecutionContext.InputParameters["tipoInformeATR"] as String;

            //Lectura de plantilla y escritura de un primer campo

            ClosedXML.Excel.XLWorkbook libro = null;
            writelog("Variables recibidas. Periodo: " + periodo + " TipoInforme: " + tipoInforme);
            //Seleccionamos el tipo de informe que vamos a tratar
            if (tipoInforme == "SI")
            {
                writelog("Ruta plantilla: " + _rutaPlantilla + nombrePlantillaSI);
                libro = llenarDatos(service, _rutaPlantilla + nombrePlantillaSI, ref hayErrores);
                //Cambiamos el nombre de salida del excel
                nuevoNombre = "\\" + tipoInforme + "_" + codigoComercializadora + "_E_" + periodo + "_00.xlsx";
            }
            /*else if (tipoInforme == "PS")
            {
                writelog("Ruta plantilla: " + _rutaPlantilla + nombrePlantillaPS);
                libro = llenarDatosPS(service, _rutaPlantilla + nombrePlantillaPS, ref hayErrores);
                //Cambiamos el nombre de salida del excel
                nuevoNombre = "\\" + tipoInforme + "_" + codigoDistribuidora + "_E_" + periodo + "_00.xlsx";
            }*/


            if (hayErrores)
            {
                hayErrores = true;
                mensajeDeError = "La consulta no ha devuelto ningún resultado, por lo que no se ha generado el informe solicitado. \nRevise el periodo seleccionado y si sigue teniendo problemas contacte con el administrador del sistema.";
                writelog("hayErrores = " + hayErrores + "; " + mensajeDeError);
            }



            writelog("Ruta Salida Archivo: " + _rutaSalidaArchivo + nuevoNombre);
            libro.SaveAs(_rutaSalidaArchivo + nuevoNombre);
            libro.Dispose();
            //aux_rutaSalidaArchivo = URIsalida + @"Atos-Informes/InformesATR" + nombrePlantilla.Replace(".xlsx", sFecha + ".xlsx");
            aux_rutaSalidaArchivo = URIsalida + @"Atos-Informes/InformesATR" + nuevoNombre;

            writelog("Ruta Salida: " + aux_rutaSalidaArchivo);

            //RETORNO AL JAVASCRIPT
            if (!hayErrores)
            {
                PluginExecutionContext.OutputParameters["correcto"] = true;
                PluginExecutionContext.OutputParameters["urlDescarga"] = aux_rutaSalidaArchivo;
                PluginExecutionContext.OutputParameters["errorMessage"] = "";
            }
            else
            {
                PluginExecutionContext.OutputParameters["correcto"] = false;
                PluginExecutionContext.OutputParameters["urlDescarga"] = "";
                PluginExecutionContext.OutputParameters["errorMessage"] = mensajeDeError;
            }


        }

        #endregion CONSTRUCTOR Y EXECUTE

        #region FUNCIONES AUXILIARES

        /*
        private ClosedXML.Excel.XLWorkbook llenarDatosPS(IOrganizationService _servicioConsultas, String _rutaPlantilla, ref Boolean hayErrores)
        {
            writelog("Inicio Introducción Datos en Excel - PS\n");
            ClosedXML.Excel.XLWorkbook libro;
            try
            {
                libro = new ClosedXML.Excel.XLWorkbook(_rutaPlantilla);
            }
            catch (Exception e)
            {
                writelog("Excepcion: " + e.Message + " - " + e.StackTrace);
                throw new Exception(e.Message);
            }

            //Ponemos la cabecera
            var hoja1 = libro.Worksheet("CABECERA");
            hoja1.Cell(2, 2).Value = "R1-284-AFRODISIO PASCUAL ALONSO, S.L.";
            hoja1.Cell(2, 6).Value = periodo;

            crearHoja2PS(libro, _servicioConsultas);

            return libro;
        }
        */
        private ClosedXML.Excel.XLWorkbook llenarDatos(IOrganizationService _servicioConsultas, String _rutaPlantilla, ref Boolean hayErrores)
        {
            writelog("Inicio Introducción Datos en Excel - SI\n");
            writelog("Valor del libro: " + _rutaPlantilla);
            Hashtable agentes = new Hashtable();
            Hashtable provincias = new Hashtable();
            Hashtable motivosError = new Hashtable();

            ClosedXML.Excel.XLWorkbook libro;
            try
            {
                libro = new ClosedXML.Excel.XLWorkbook(_rutaPlantilla);
                if (libro != null)
                {
                    //Si el libro ya tiene datos, cargo los agentes en la tabla
                    var hojaMaestros = libro.Worksheet("MAESTROS");
                    for (int i = 1; i < numeroFilasRecorrer; i++)
                    {
                        String codigo = (String)hojaMaestros.Cell(i, 20).Value;
                        String valor = (String)hojaMaestros.Cell(i, 19).Value;
                        agentes.Add(codigo, valor);

                        if (i < 60)
                        {
                            if (i < 55)
                            {
                                String codProv = ((String)hojaMaestros.Cell(i, 2).Value).Substring(0, 2);
                                String nombreProv = (String)hojaMaestros.Cell(i, 1).Value;
                                provincias.Add(codProv, nombreProv);

                            }

                            String codError = (String)hojaMaestros.Cell(i, 17).Value;
                            String nombreError = (String)hojaMaestros.Cell(i, 16).Value;
                            motivosError.Add(codError, nombreError);
                            //writelog( codProv  + " - " +   (String)provincias[codProv]);
                        }



                    }
                }
            }
            catch (Exception e)
            {
                writelog("Excepcion: " + e.Message + " - " + e.StackTrace);
                throw new Exception(e.Message);
            }

            //Ponemos la cabecera
            var hoja1 = libro.Worksheet("CABECERA");
            hoja1.Cell(2, 2).Value = "R2-255-ACCIONA GREEN ENERGY DEVELOPMENTS, S.L.";
            hoja1.Cell(2, 6).Value = periodo;

            //Creamos el resto de hojas
            crearHoja2(libro, _servicioConsultas, agentes, provincias);
            crearHoja3(libro, _servicioConsultas, agentes, provincias);
            crearHoja4(libro, _servicioConsultas, agentes, provincias);
            crearHoja5(libro, _servicioConsultas, agentes, provincias, motivosError);
            crearHoja6(libro, _servicioConsultas, agentes, provincias);
            crearHoja7(libro, _servicioConsultas, agentes, provincias);

            return libro;
        }


        /*
        private void crearHoja2PS(ClosedXML.Excel.XLWorkbook libro, IOrganizationService _servicioConsultas)
        {
            var hoja = libro.Worksheet("DATOS_PTOS_SUMINISTRO");
            writelog("Inicio Llenado hoja 2. DATOS_PTOS_SUMINISTRO.");
            int filaInicio = 2;
            //Buscamos los registros por "fecha prevista acción"
            EntityCollection registros = consultaContratos(_servicioConsultas, ref hayErrores, periodo, "atos_fechainiciocontrato");

            Hashtable _Tregistros = ordenarRegistrosPS(_servicioConsultas, registros);

            writelog("Inicio Trato registros Registros. Hay " + _Tregistros.Count + " Registros");
            foreach (String regContrato in _Tregistros.Keys)
            {
                writelog("Trabajamos con el registro: " + regContrato);
                objetoContrato obj = (objetoContrato)_Tregistros[regContrato];
                //Llenamos el excel
                hoja.Cell(filaInicio, 7).Value = obj.provincia;
                hoja.Cell(filaInicio, 8).Value = obj.distribuidor;
                hoja.Cell(filaInicio, 9).Value = obj.comerEntrante;
                hoja.Cell(filaInicio, 10).Value = obj.tarifaATR;
                hoja.Cell(filaInicio, 11).Value = obj.numeroContratos;

                filaInicio++;
            }



            writelog("FINAL Llenado hoja 2. DATOS_PTOS_SUMINISTRO " + "\n");
        }
        */
        private void crearHoja2(ClosedXML.Excel.XLWorkbook libro, IOrganizationService _servicioConsultas, Hashtable tablaAgentes, Hashtable tablaProvincias)
        {
            var hoja = libro.Worksheet("DATOSSOLICITUDES");
            writelog("Inicio Llenado hoja 2. DATOS SOLICITUDES.");
            int filaInicio = 2;
            //Buscamos los registros por "fecha prevista acción"
            EntityCollection registros = consultaATR(servicioConsultas, ref hayErrores, periodo, "atos_fechaprevistaaccion");
            if (registros != null)
            {
                //Con todos los registros, los ordeno en una tabla y compruebo los códigos
                Hashtable _Tregistros = ordenarRegistros(_servicioConsultas, registros, "01", tablaAgentes, tablaProvincias); //Para guardar Id solicitud, Lista de solicitudes 01
                //Ya tengo todos los registros que necesito, entonces recorro toda la matriz, y los pongo

                foreach (String k in _Tregistros.Keys)
                {
                    objetoATR obj = (objetoATR)_Tregistros[k];
                    hoja.Cell(filaInicio, 10).Value = obj.provincia;
                    hoja.Cell(filaInicio, 11).Value = obj.distribuidor;
                    hoja.Cell(filaInicio, 12).Value = obj.comerEntrante;
                    hoja.Cell(filaInicio, 13).Value = obj.comerSaliente;
                    hoja.Cell(filaInicio, 14).Value = obj.tipoCambio;
                    hoja.Cell(filaInicio, 15).Value = obj.tipoPunto;
                    hoja.Cell(filaInicio, 16).Value = obj.tarifaATR;
                    hoja.Cell(filaInicio, 16).DataType = ClosedXML.Excel.XLCellValues.Text;
                    hoja.Cell(filaInicio, 17).Value = obj.numeroSolicitudes;
                    //CAMPOS POR DEFECTO
                    hoja.Cell(filaInicio, 18).Value = "0";
                    hoja.Cell(filaInicio, 19).Value = "0";
                    hoja.Cell(filaInicio, 20).Value = "0";
                    hoja.Cell(filaInicio, 21).Value = "0";

                    filaInicio++;
                }



            }


            writelog("FINAL Llenado hoja 2. DATOS SOLICITUDES " + "\n");
        }

        private void crearHoja3(ClosedXML.Excel.XLWorkbook libro, IOrganizationService _servicioConsultas, Hashtable tablaAgentes, Hashtable tablaProvincias)
        {
            writelog("Inicio Llenado hoja 3. DETALLE PENDIENTES RESPUESTA");
            var hoja = libro.Worksheet("DETALLEPENDIENTESRESPUESTA");
            int filaInicio = 2;
            EntityCollection registros = consultaATRsinFecha(servicioConsultas, ref hayErrores, periodo, "atos_fechaprevistaaccion");
            //Buscamos los registros por "fecha prevista acción"
            //EntityCollection registros = consultaATR(servicioConsultas, ref hayErrores, periodo, "atos_fechaprevistaaccion");
            if (registros != null)
            {
                //reviso toda la consulta para ver cuales quitar
                registros = mirarPendientesActivacion(servicioConsultas, registros, periodo, "atos_fechaaceptacion", "02");
                registros = mirarPendientesActivacion(servicioConsultas, registros, periodo, "atos_fecharechazo", "02");
                registros = mirarPendientesActivacion(servicioConsultas, registros, periodo, "atos_fechaactivacionprevistatransformada", "05");
                //Con todos los registros, los ordeno en una tabla y compruebo los códigos
                Hashtable _Tregistros = ordenarRegistros(_servicioConsultas, registros, "01", tablaAgentes, tablaProvincias); //Para guardar Id solicitud, Lista de solicitudes 01
                //Ya tengo todos los registros que necesito, entonces recorro toda la matriz, y los pongo

                foreach (String k in _Tregistros.Keys)
                {
                    objetoATR obj = (objetoATR)_Tregistros[k];
                    hoja.Cell(filaInicio, 11).Value = obj.provincia;
                    hoja.Cell(filaInicio, 12).Value = obj.distribuidor;
                    hoja.Cell(filaInicio, 13).Value = obj.comerEntrante;
                    hoja.Cell(filaInicio, 14).Value = obj.comerSaliente;
                    hoja.Cell(filaInicio, 15).Value = obj.tipoCambio;
                    hoja.Cell(filaInicio, 16).Value = obj.tipoPunto;
                    hoja.Cell(filaInicio, 17).Value = obj.tarifaATR;
                    hoja.Cell(filaInicio, 17).DataType = ClosedXML.Excel.XLCellValues.Text;
                    hoja.Cell(filaInicio, 18).Value = obj.tipoRetraso;
                    hoja.Cell(filaInicio, 19).Value = obj.numeroSolicitudes;

                    filaInicio++;
                }
            }



            writelog("FINAL Llenado hoja 3. Pendientes Respuesta" + "\n");
        }

        private void crearHoja4(ClosedXML.Excel.XLWorkbook libro, IOrganizationService _servicioConsultas, Hashtable tablaAgentes, Hashtable tablaProvincias)
        {
            writelog("Inicio Llenado hoja 4. DETALLE ACEPTADAS");
            var hoja = libro.Worksheet("DETALLEACEPTADAS");
            int filaInicio = 2;
            //Buscamos los registros por "fecha prevista acción"
            EntityCollection registros = consultaATR(servicioConsultas, ref hayErrores, periodo, "atos_fechaaceptacion");
            if (registros != null)
            {
                //Con todos los registros, los ordeno en una tabla y compruebo los códigos
                Hashtable _Tregistros = ordenarRegistros(_servicioConsultas, registros, "02", tablaAgentes, tablaProvincias); //Para guardar Id solicitud, Lista de solicitudes 01
                //Ya tengo todos los registros que necesito, entonces recorro toda la matriz, y los pongo

                foreach (String k in _Tregistros.Keys)
                {

                    objetoATR obj = (objetoATR)_Tregistros[k];
                    hoja.Cell(filaInicio, 11).Value = obj.provincia;
                    hoja.Cell(filaInicio, 12).Value = obj.distribuidor;
                    hoja.Cell(filaInicio, 13).Value = obj.comerEntrante;
                    hoja.Cell(filaInicio, 14).Value = obj.comerSaliente;
                    hoja.Cell(filaInicio, 15).Value = obj.tipoCambio;
                    hoja.Cell(filaInicio, 16).Value = obj.tipoPunto;
                    hoja.Cell(filaInicio, 17).Value = obj.tarifaATR;
                    hoja.Cell(filaInicio, 17).DataType = ClosedXML.Excel.XLCellValues.Text;
                    hoja.Cell(filaInicio, 18).Value = obj.tipoRetraso;
                    hoja.Cell(filaInicio, 19).Value = "15 Días";
                    hoja.Cell(filaInicio, 20).Value = obj.numeroSolicitudes;

                    filaInicio++;
                }



            }



            writelog("FINAL Llenado hoja 4. DETALLE ACEPTADAS" + "\n");
        }

        private void crearHoja5(ClosedXML.Excel.XLWorkbook libro, IOrganizationService _servicioConsultas, Hashtable tablaAgentes, Hashtable tablaProvincias, Hashtable tablaMotivos)
        {
            writelog("Inicio Llenado hoja 5. DETALLE RECHAZADAS");
            var hoja = libro.Worksheet("DETALLERECHAZADAS");
            int filaInicio = 2;
            //Buscamos los registros por "fecha prevista acción"
            EntityCollection registros = consultaATR(servicioConsultas, ref hayErrores, periodo, "atos_fecharechazo");
            if (registros != null)
            {
                //Con todos los registros, los ordeno en una tabla y compruebo los códigos
                Hashtable _Tregistros = ordenarRegistros(_servicioConsultas, registros, "02", tablaAgentes, tablaProvincias, tablaMotivos); //Para guardar Id solicitud, Lista de solicitudes 01
                //Ya tengo todos los registros que necesito, entonces recorro toda la matriz, y los pongo

                foreach (String k in _Tregistros.Keys)
                {

                    objetoATR obj = (objetoATR)_Tregistros[k];
                    hoja.Cell(filaInicio, 12).Value = obj.provincia;
                    hoja.Cell(filaInicio, 13).Value = obj.distribuidor;
                    hoja.Cell(filaInicio, 14).Value = obj.comerEntrante;
                    hoja.Cell(filaInicio, 15).Value = obj.comerSaliente;
                    hoja.Cell(filaInicio, 16).Value = obj.tipoCambio;
                    hoja.Cell(filaInicio, 17).Value = obj.tipoPunto;
                    hoja.Cell(filaInicio, 18).Value = obj.tarifaATR;
                    hoja.Cell(filaInicio, 18).DataType = ClosedXML.Excel.XLCellValues.Text;
                    hoja.Cell(filaInicio, 19).Value = obj.tipoRetraso;
                    hoja.Cell(filaInicio, 20).Value = obj.motivoRechazo;
                    hoja.Cell(filaInicio, 21).Value = "15 Días";
                    hoja.Cell(filaInicio, 22).Value = obj.numeroSolicitudes;

                    filaInicio++;
                }
            }



            writelog("FINAL Llenado hoja 5. DETALLE RECHAZADAS" + "\n");
        }

        private void crearHoja6(ClosedXML.Excel.XLWorkbook libro, IOrganizationService _servicioConsultas, Hashtable tablaAgentes, Hashtable tablaProvincias)
        {
            writelog("Inicio Llenado hoja 6. DETALLE PENDIENTE ACTIVACION");
            var hoja = libro.Worksheet("DETALLEPDTEACTIVACION");
            int filaInicio = 2;
            //Buscamos los registros por "fecha prevista acción"

            EntityCollection registros = consultaATRsinFecha(servicioConsultas, ref hayErrores, periodo, "atos_fechaaceptacion");
            if (registros != null)
            {
                //reviso toda la consulta para ver cuales quitar
                registros = mirarPendientesActivacion(servicioConsultas, registros, periodo, "atos_fechaactivacionprevistatransformada", "05");
                //Con todos los registros, los ordeno en una tabla y compruebo los códigos
                Hashtable _Tregistros = ordenarRegistros(_servicioConsultas, registros, "02", tablaAgentes, tablaProvincias); //Para guardar Id solicitud, Lista de solicitudes 01
                //Ya tengo todos los registros que necesito, entonces recorro toda la matriz, y los pongo

                foreach (String k in _Tregistros.Keys)
                {

                    objetoATR obj = (objetoATR)_Tregistros[k];
                    hoja.Cell(filaInicio, 11).Value = obj.provincia;
                    hoja.Cell(filaInicio, 12).Value = obj.distribuidor;
                    hoja.Cell(filaInicio, 13).Value = obj.comerEntrante;
                    hoja.Cell(filaInicio, 14).Value = obj.comerSaliente;
                    hoja.Cell(filaInicio, 15).Value = obj.tipoCambio;
                    hoja.Cell(filaInicio, 16).Value = obj.tipoPunto;
                    hoja.Cell(filaInicio, 17).Value = obj.tarifaATR;
                    hoja.Cell(filaInicio, 17).DataType = ClosedXML.Excel.XLCellValues.Text;
                    hoja.Cell(filaInicio, 18).Value = obj.tipoRetraso;
                    hoja.Cell(filaInicio, 19).Value = "0";
                    hoja.Cell(filaInicio, 20).Value = obj.numeroSolicitudes;

                    filaInicio++;
                }
            }



            writelog("FINAL Llenado hoja 6. DETALLE PENDIENTE ACTIVACION" + "\n");
        }

        private void crearHoja7(ClosedXML.Excel.XLWorkbook libro, IOrganizationService _servicioConsultas, Hashtable tablaAgentes, Hashtable tablaProvincias)
        {
            writelog("Inicio Llenado hoja 7. DETALLE ACTIVADAS");
            var hoja = libro.Worksheet("DETALLEACTIVADAS");
            int filaInicio = 2;
            //Buscamos los registros por "fecha prevista acción"
            EntityCollection registros = consultaATR(servicioConsultas, ref hayErrores, periodo, "atos_fechaactivacionprevistatransformada");
            if (registros != null)
            {
                //Con todos los registros, los ordeno en una tabla y compruebo los códigos
                Hashtable _Tregistros = ordenarRegistros(_servicioConsultas, registros, "05", tablaAgentes, tablaProvincias); //Para guardar Id solicitud, Lista de solicitudes 01
                //Ya tengo todos los registros que necesito, entonces recorro toda la matriz, y los pongo

                foreach (String k in _Tregistros.Keys)
                {

                    objetoATR obj = (objetoATR)_Tregistros[k];
                    hoja.Cell(filaInicio, 11).Value = obj.provincia;
                    hoja.Cell(filaInicio, 12).Value = obj.distribuidor;
                    hoja.Cell(filaInicio, 13).Value = obj.comerEntrante;
                    hoja.Cell(filaInicio, 14).Value = obj.comerSaliente;
                    hoja.Cell(filaInicio, 15).Value = obj.tipoCambio;
                    hoja.Cell(filaInicio, 16).Value = obj.tipoPunto;
                    hoja.Cell(filaInicio, 17).Value = obj.tarifaATR;
                    hoja.Cell(filaInicio, 17).DataType = ClosedXML.Excel.XLCellValues.Text;
                    hoja.Cell(filaInicio, 18).Value = obj.tipoRetraso;
                    hoja.Cell(filaInicio, 19).Value = "15 Días";
                    hoja.Cell(filaInicio, 20).Value = "0";
                    hoja.Cell(filaInicio, 21).Value = obj.numeroSolicitudes;

                    filaInicio++;
                }
            }



            writelog("FINAL Llenado hoja 7. DETALLE ACTIVADAS" + "\n");
        }

        private EntityCollection mirarPendientesActivacion(IOrganizationService servicioConsultas, EntityCollection registros, String periodo, String campo, String codPaso)
        {
            //Tengo todos los que son del tipo marcado en codPaso con fecha de aceptación. ahora busco las activadas y las ordeno por instalación -  

            EntityCollection registrosActivados = consultaATRsinFecha(servicioConsultas, ref hayErrores, periodo, campo);
            if (registrosActivados != null)
            {
                //Si la consulta devuelve valores, es porque puede que algún registro ya haya sido activado
                Hashtable instalacionesConActivacion = new Hashtable();
                //Ordeno las instalaciones que tienen fechaActivacionPrevista por instalación, si su paso es 05
                foreach (Entity regActivado in registrosActivados.Entities)
                {
                    writelog("trabajando con el registro: " + regActivado.Id);
                    String Paso = GetPaso(servicioConsultas, regActivado).Attributes["atos_codigopaso"].ToString();
                    writelog("trabajando con el registro: " + regActivado.Id + " y tiene el paso: " + Paso);
                    if (Paso == codPaso && !instalacionesConActivacion.ContainsKey(regActivado.Attributes["atos_instalacionid"]))
                    {
                        writelog("Con todo, lo pongo en la tabla");
                        instalacionesConActivacion.Add(regActivado.Attributes["atos_instalacionid"], Paso);
                    }
                    
                }

                ArrayList regEliminar = new ArrayList();
                foreach (Entity regATR in registros.Entities)
                {
                    writelog("trabajando con el registro: " + regATR.Id + " Tiene instalacion: " + regATR.Contains("atos_instalacionid"));
                    if (!regATR.Contains("atos_instalacionid") || instalacionesConActivacion.ContainsKey(regATR.Attributes["atos_instalacionid"]))
                    {
                        writelog("Este registro si que tiene la instalación en la tabla");
                        //Si el registro está en la tabla, lo quito del mi colección
                        regEliminar.Add(regATR);
                        //registros.Entities.Remove(regATR);
                    }
                }

                if (regEliminar.Count > 0)
                {
                    foreach (Entity r in regEliminar)
                    {
                        registros.Entities.Remove(r);
                    }
                }

            }


            return registros;
        }

        private Hashtable ordenarRegistros(IOrganizationService servicioConsultas, EntityCollection registros, String codigoPaso, Hashtable tablaAgentes, Hashtable tablaProvincias, Hashtable tablaMotivos = null)
        {
            writelog("Inicio ordenación registros: " + registros.Entities.Count);
            Hashtable _Tabla = new Hashtable();

            foreach (Entity regATR in registros.Entities)
            {
                //la colección viene de solicitud atr, entonces busco su paso y el proceso de dicho paso
                Entity paso = GetPaso(servicioConsultas, regATR);
                writelog("Tratando el paso: " + paso.Attributes["atos_codigopaso"].ToString());

                if (paso.Attributes.Contains("atos_codigopaso") && paso.Attributes["atos_codigopaso"].ToString() == codigoPaso)
                {
                    //Si el paso es del tipo 01, buscamos el proceso para discriminar el resto de solicitudes
                    String ProcesoATR = GetProcesoATR(servicioConsultas, paso);
                    writelog("Estamos ya tratando el registro, y buscamos su proceso ATR: " + ProcesoATR);
                    if (ProcesoATR == "C1" || ProcesoATR == "C2" || ProcesoATR == "A3")
                    {
                        //ya sabemos que la solicitud debe estar entre las elegidas, entonces trabajamos con el registro
                        Entity instalacion = null;
                        String clave = "";
                        String Provincia = "";
                        String Distribuidor = "";
                        String ComerEntrante = "R2-255-ACCIONA GREEN ENERGY DEVELOPMENTS, S.L.";
                        String ComerSaliente = "0-NO DOCUMENTADO";
                        String TipoCambio = "CAMBIO COMERCIALIZADOR";
                        String TipoPunto = "TIPO 1";
                        String TarifaATR = GetTarifa(servicioConsultas, regATR) + "";
                        String TipoRetraso = "EN PLAZO";
                        String MotivoRechazo = "";

                        if (regATR.Contains("atos_instalacionid")) {
                            writelog("Tengo instalación: " + regATR.Attributes["atos_instalacionid"]);
                            instalacion = GetInstalacion(servicioConsultas, ((EntityReference)regATR.Attributes["atos_instalacionid"]).Id);
                            writelog("Instalación devuelta: " + instalacion.Id);
                            writelog("Inicio Busqueda Provincias ");
                            Provincia = (String)tablaProvincias[GetProvincia(servicioConsultas, instalacion)];
                            writelog("Inicio Busqueda Distribuidora");
                            Distribuidor = (String)tablaAgentes[GetDistribuidoraActual(servicioConsultas, instalacion)];
                            writelog("final Busqueda");

                            if (instalacion.Attributes.Contains("atos_reecodigotipopuntomedida") && instalacion.Attributes["atos_reecodigotipopuntomedida"].ToString() != "")
                            {
                                TipoPunto = "TIPO " + instalacion.Attributes["atos_reecodigotipopuntomedida"].ToString();
                            }

                            if (ProcesoATR == "A3")
                            {
                                TipoCambio = "ALTA DIRECTA";
                            }

                            //Vamos a hacer una clave para que el mapa las pueda agrupar
                            clave = Provincia + "&" + Distribuidor + "&" + TipoCambio + "&" + TipoPunto + "&" + TarifaATR;


                            //writelog(" - contenido: " + regATR.Attributes["atos_motivorechazoatrid"]);
                            if (regATR.Attributes.Contains("atos_motivorechazoatrid") && regATR.Attributes["atos_motivorechazoatrid"] != null && tablaMotivos != null)
                            {
                                writelog("hay motivo de rechazo. Dentro de IF");
                                MotivoRechazo = GetMotivoRechazo(servicioConsultas, regATR);
                                writelog("Ya he cogido mi motivo de rechazo: " + MotivoRechazo);
                                MotivoRechazo = (String)tablaMotivos[MotivoRechazo];

                                clave = Provincia + "&" + Distribuidor + "&" + TipoCambio + "&" + TipoPunto + "&" + TarifaATR + "&" + MotivoRechazo;
                            }
                            writelog("Clave: " + clave);

                            objetoATR obj;
                            if (_Tabla.ContainsKey(clave))
                            {
                                //Si la clave existe, es porque ya hay un registro con las mismas características. Sumo uno
                                writelog("El registro esta en la tabla");
                                obj = (objetoATR)_Tabla[clave];
                                obj.numeroSolicitudes = obj.numeroSolicitudes + 1;

                                //Lo borro de la tabla, para que no haya problemas
                                _Tabla.Remove(clave);
                            }
                            else
                            {
                                writelog("El registro NO esta en la tabla");
                                //Si no existe el registro, lo creo
                                obj = new objetoATR();
                                obj.provincia = Provincia;
                                obj.distribuidor = Distribuidor;
                                obj.comerEntrante = ComerEntrante;
                                obj.comerSaliente = ComerSaliente;
                                obj.tipoCambio = TipoCambio;
                                obj.tipoPunto = TipoPunto;
                                obj.tarifaATR = TarifaATR;
                                obj.tipoRetraso = TipoRetraso;
                                obj.motivoRechazo = MotivoRechazo;
                                obj.numeroSolicitudes = 1;
                            }

                            writelog("El pongo el registro en tabla: " + clave + " " + obj.numeroSolicitudes);

                            _Tabla.Add(clave, obj);

                        }
                    }
                }
            }
            writelog("Final Ordenación Registros\n");
            return _Tabla;
        }

        /*
        
        private Hashtable ordenarRegistrosPS(IOrganizationService _servicioConsultas, EntityCollection registros)
        {
            writelog("Inicio ordenación registros PS: " + registros.Entities.Count);
            Hashtable _Tabla = new Hashtable();

            foreach (Entity regContrato in registros.Entities)
            {
                String clave = "";
                String Provincia = "";
                String Distribuidor = "";
                String Comercializadora = "R2-255-ACCIONA GREEN ENERGY DEVELOPMENTS, S.L.";
                String Tarifa = "";

                if (regContrato.Contains("atos_instalacionid"))
                {
                    //Si existe la instalación -> debería
                    Entity instalacion = GetInstalacion(_servicioConsultas, ((EntityReference)regContrato.Attributes["atos_instalacionid"]).Id);
                    Provincia = GetProvincia(_servicioConsultas, instalacion).ToUpper();
                    Provincia = Provincia.Replace("Á","A");
                    Provincia = Provincia.Replace("É", "E");
                    Provincia = Provincia.Replace("Í", "I");
                    Provincia = Provincia.Replace("Ó", "O");
                    //Provincia = Provincia.Replace("ú", "u");
                    Distribuidor = GetDistribuidoraActual(_servicioConsultas, instalacion);
                }
                if (regContrato.Contains("atos_tarifaid"))
                {
                    Tarifa = GetTarifa(_servicioConsultas, regContrato);
                }

                //Creamos una clave
                clave = Provincia + "&" + Distribuidor + "&" + Tarifa;
                writelog("Clave: " + clave);

                objetoContrato obj;
                if (_Tabla.ContainsKey(clave))
                {
                    //Si la clave existe, es porque ya hay un registro con las mismas características. Sumo uno
                    writelog("El registro SI esta en la tabla");
                    obj = (objetoContrato)_Tabla[clave];
                    obj.numeroContratos = obj.numeroContratos + 1;

                    //Lo borro de la tabla, para que no haya problemas
                    _Tabla.Remove(clave);
                }
                else
                {
                    writelog("El registro NO esta en la tabla");
                    //Si no existe el registro, lo creo
                    obj = new objetoContrato();
                    obj.provincia = Provincia;
                    obj.distribuidor = Distribuidor;
                    obj.comerEntrante = Comercializadora;
                    obj.tarifaATR = Tarifa;
                    obj.numeroContratos = 1;
                }

                writelog("Pongo el registro en tabla: " + obj.provincia + " " + obj.numeroContratos);

                _Tabla.Add(clave, obj);

            }


            writelog("Final Ordenación RegistrosPS\n");
            return _Tabla;
        }
        */
        #region CONSULTAS

        //private String GetComercializadoraActual(IOrganizationService servicioConsultas, Entity instalacion)
        //{
        //    //writelog("Inicio Búsqueda Comercializadora Actual");
        //    if (instalacion.Contains("atos_comercializadoraactualid"))
        //    {
        //        ColumnSet colSet = new ColumnSet("atos_name", "atos_codigocnmc");
        //        Entity ComercialActual = servicioConsultas.Retrieve("atos_comercializadora", ((EntityReference)instalacion.Attributes["atos_comercializadoraactualid"]).Id, colSet);
        //        // writelog("FINAL Comercializadora Actual " + ComercialActual.Attributes["atos_name"].ToString());
        //        String codigo = "";
        //        if (ComercialActual.Attributes.Contains("atos_codigocnmc"))
        //        {
        //            codigo = ComercialActual.Attributes["atos_codigocnmc"].ToString();
        //        }

        //        return codigo + "-" + ComercialActual.Attributes["atos_name"].ToString();
        //    }
        //    return "";
        //}

        private String GetDistribuidoraActual(IOrganizationService servicioConsultas, Entity instalacion)
        {
            //writelog("Inicio Búsqueda Distribuidora Actual");
            if (instalacion.Contains("atos_distribuidoraid"))
            {
                ColumnSet colSet = new ColumnSet("atos_unidad");
                Entity ComercialActual = servicioConsultas.Retrieve("atos_distribuidora", ((EntityReference)instalacion.Attributes["atos_distribuidoraid"]).Id, colSet);
                //writelog("FINAL Distribuidora Actual " + ComercialActual.Attributes["atos_name"].ToString());
                if (ComercialActual.Contains("atos_unidad"))
                {
                    return ComercialActual.Attributes["atos_unidad"].ToString();
                }
                else {
                    return "0";
                }
                
            }
            return "0";
        }

        private String GetProvincia(IOrganizationService servicioConsultas, Entity instalacion)
        {
            if (instalacion.Contains("atos_instalacionprovinciaid"))
            {
                //writelog("Inicio Búsqueda Provincias");
                ColumnSet colSet = new ColumnSet("atos_codigoprovincia");
                Entity provincia = servicioConsultas.Retrieve("atos_provincia", ((EntityReference)instalacion.Attributes["atos_instalacionprovinciaid"]).Id, colSet);
                // writelog("FINAL GetProvincia " + provincia.Attributes["atos_name"].ToString());
                return provincia.Attributes["atos_codigoprovincia"].ToString();
            }
            return "00";
        }

        private String GetMotivoRechazo(IOrganizationService servicioConsultas, Entity registroATR)
        {
            if (registroATR.Contains("atos_motivorechazoatrid"))
            {
                //writelog("Inicio Búsqueda Provincias");
                ColumnSet colSet = new ColumnSet("atos_codigomotivorechazoatr");
                Entity provincia = servicioConsultas.Retrieve("atos_motivosrechazoatr", ((EntityReference)registroATR.Attributes["atos_motivorechazoatrid"]).Id, colSet);
                // writelog("FINAL GetProvincia " + provincia.Attributes["atos_name"].ToString());
                return provincia.Attributes["atos_codigomotivorechazoatr"].ToString();
            }
            return "99";
        }

        private String GetTarifa(IOrganizationService servicioConsultas, Entity entidad)
        {
            if (entidad.Contains("atos_tarifaid"))
            {
                ColumnSet colSet = new ColumnSet("atos_name");
                Entity tarifa = servicioConsultas.Retrieve("atos_tarifa", ((EntityReference)entidad.Attributes["atos_tarifaid"]).Id, colSet);
                //writelog("Lanzando GetTarifa " + tarifa.Attributes["atos_name"].ToString() + "\r\n");
                return tarifa.Attributes["atos_name"].ToString();
            }
            return "";
        }

        private String GetProcesoATR(IOrganizationService servicioConsultas, Entity entidad)
        {
            ColumnSet colSet = new ColumnSet("atos_codigoproceso");
            Entity CodProcesoATR = servicioConsultas.Retrieve("atos_procesoatr", ((EntityReference)entidad.Attributes["atos_procesoatrid"]).Id, colSet);
            //writelog("Lanzando GetTarifa " + tarifa.Attributes["atos_name"].ToString() + "\r\n");
            return CodProcesoATR.Attributes["atos_codigoproceso"].ToString();
        }

        private Entity GetPaso(IOrganizationService servicioConsultas, Entity entidad)
        {
            ColumnSet colSet = new ColumnSet("atos_name", "atos_codigopaso", "atos_procesoatrid");
            Entity paso = servicioConsultas.Retrieve("atos_pasoatr", ((EntityReference)entidad.Attributes["atos_pasoatrid"]).Id, colSet);
            //writelog("Lanzando GetPaso " + paso.Attributes["atos_name"].ToString() + "\r\n");
            return paso;
        }

        private Entity GetInstalacion(IOrganizationService servicioConsultas, Guid entidad)
        {
            writelog("Inicio consulta Instalacion " + entidad);
            ColumnSet colSet = new ColumnSet("atos_fechainiciovigencia", "atos_instalacionprovinciaid", 
                "atos_distribuidoraid", "atos_tarifaid", "atos_historico", "atos_reecodigotipopuntomedida");
            Entity instalacion = servicioConsultas.Retrieve("atos_instalacion", entidad, colSet);
            writelog("Lanzando GetInstalacion ");
            writelog("Tiene instalacion:" + (instalacion != null));
            return instalacion;
        }


        private EntityCollection consultaATRsinFecha(IOrganizationService _servicioConsultas, ref Boolean hayErrores, String _periodo, String campo)
        {
            writelog("Inicio creación consulta de solicitudes ATR del periodo: SIN PERIODO, respecto al campo: " + campo);

            String format = "dd-MM-yyyy";
            IFormatProvider provider = new CultureInfo("es-ES");
            String fecha = "01-" + _periodo.Substring(4, 2) + "-" + _periodo.Substring(0, 4);
            DateTime fechaDT = DateTime.ParseExact(fecha, format, provider);
           
            QueryExpression _consulta = new QueryExpression("atos_solicitudatr");
            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;

            ConditionExpression _condicion1 = new ConditionExpression();
            _condicion1.AttributeName = campo;//"atos_fechasolicitud";
            _condicion1.Operator = ConditionOperator.LessEqual;
            _condicion1.Values.Add(fechaDT.AddMonths(1));
            _filtro.Conditions.Add(_condicion1);

            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = campo;//"atos_fechasolicitud"; 
            _condicion.Operator = ConditionOperator.GreaterEqual;
            _condicion.Values.Add(fechaMinima);
            _filtro.Conditions.Add(_condicion);

            _consulta.Criteria.AddFilter(_filtro);
            _consulta.ColumnSet = new ColumnSet("atos_tarifaid", "atos_comercializadoraid", "atos_procesoatrid",
                "atos_pasoatrid", "atos_distribuidoraid", "atos_instalacionid", "atos_secuenciadesolicitud", "atos_motivorechazoatrid",
                "atos_fechasolicitud", "atos_fechaactivacionprevistatransformada", "atos_fechaaceptacion", "atos_fecharechazo", "atos_fechaprevistaaccion");


            EntityCollection _resultado = _servicioConsultas.RetrieveMultiple(_consulta);
            if (_resultado != null && _resultado.Entities.Count == 0)
            {
                writelog("Final consulta ATR sin resultado" + "\n");
                //hayErrores = true;
                return null;
            }
            writelog("1- Final consulta de solicitudes ATR. Número de Registros devueltos: " + _resultado.Entities.Count + "\n");
            return _resultado;

        }

        private EntityCollection consultaATR(IOrganizationService _servicioConsultas, ref Boolean hayErrores, String _periodo, String campo)
        {
            writelog("Inicio creación consulta de solicitudes ATR del periodo: " + _periodo + ", respecto al campo: " + campo);

            String format = "dd-MM-yyyy";
            IFormatProvider provider = new CultureInfo("es-ES");
            String fecha = "01-" + _periodo.Substring(4, 2) + "-" + _periodo.Substring(0, 4);
            DateTime fechaDT = DateTime.ParseExact(fecha, format, provider);
            writelog("Periodo: " + fechaDT.ToString("dd/MM/yyy") + " Fecha hasta: " + fechaDT.AddMonths(1).ToString("dd/MM/yyy"));

            QueryExpression _consulta = new QueryExpression("atos_solicitudatr");
            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;

            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = campo;//"atos_fechasolicitud"; 
            _condicion.Operator = ConditionOperator.GreaterEqual;
            _condicion.Values.Add(fechaDT);
            _filtro.Conditions.Add(_condicion);

            ConditionExpression _condicion1 = new ConditionExpression();
            _condicion1.AttributeName = campo;//"atos_fechasolicitud";
            _condicion1.Operator = ConditionOperator.LessEqual;
            _condicion1.Values.Add(fechaDT.AddMonths(1));
            _filtro.Conditions.Add(_condicion1);

            _consulta.Criteria.AddFilter(_filtro);
            _consulta.ColumnSet = new ColumnSet("atos_tarifaid", "atos_comercializadoraid", "atos_procesoatrid",
                "atos_pasoatrid", "atos_distribuidoraid", "atos_instalacionid", "atos_secuenciadesolicitud", "atos_motivorechazoatrid",
                "atos_fechasolicitud", "atos_fechaactivacionprevistatransformada", "atos_fechaaceptacion", "atos_fecharechazo", "atos_fechaprevistaaccion");

            /*LinkEntity _link = new LinkEntity();
            _link.JoinOperator = JoinOperator.Inner;
            _link.LinkFromAttributeName = "atos_procesoatrid";
            _link.LinkFromEntityName = _consulta.EntityName;
            _link.LinkToAttributeName = "atos_codigoproceso";
            _link.LinkToEntityName = "atos_procesoatr";

            ConditionExpression _lc = new ConditionExpression();
            _lc.AttributeName = "atos_codigoproceso";
            _lc.Operator = ConditionOperator.Equal;
            _lc.Values.Add("C1");
            _link.LinkCriteria.AddCondition(_lc);
            
            ConditionExpression _lc2 = new ConditionExpression();
            _lc2.AttributeName = "atos_codigoproceso";
            _lc2.Operator = ConditionOperator.Equal;
            _lc2.Values.Add("C2");
            _link.LinkCriteria.AddCondition(_lc2);

            ConditionExpression _lc3 = new ConditionExpression();
            _lc3.AttributeName = "atos_codigoproceso";
            _lc3.Operator = ConditionOperator.Equal;
            _lc3.Values.Add("A3");
            _link.LinkCriteria.AddCondition(_lc3);


            _consulta.LinkEntities.Add(_link);
            */
            EntityCollection _resultado = _servicioConsultas.RetrieveMultiple(_consulta);
            if (_resultado != null && _resultado.Entities.Count == 0)
            {
                writelog("Final consulta ATR sin resultado" + "\n");
                //hayErrores = true;
                return null;
            }
            writelog("1- Final consulta de solicitudes ATR. Número de Registros devueltos: " + _resultado.Entities.Count + "\n");
            return _resultado;

        }

        private EntityCollection consultaContratos(IOrganizationService _servicioConsultas, ref Boolean hayErrores, String _periodo, String campo)
        {
            writelog("1- Inicio creación consulta instalaciones desde hace un mes");
            String format = "dd-MM-yyyy";
            IFormatProvider provider = new CultureInfo("es-ES");
            String fecha = "01-" + _periodo.Substring(4, 2) + "-" + _periodo.Substring(0, 4);
            DateTime fechaDT = DateTime.ParseExact(fecha, format, provider);
            writelog("Periodo: " + fechaDT.ToString("dd/MM/yyy") + " Fecha desde: " + fechaDT.AddMonths(-1).ToString("dd/MM/yyy"));

            QueryExpression _consulta = new QueryExpression("atos_contrato"); //poner entidad de informes
            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;

            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = campo;//"atos_fechasolicitud"; 
            _condicion.Operator = ConditionOperator.GreaterEqual;
            _condicion.Values.Add(fechaDT.AddMonths(-1));
            _filtro.Conditions.Add(_condicion);

            ConditionExpression _condicion1 = new ConditionExpression();
            _condicion1.AttributeName = campo;//"atos_fechasolicitud";
            _condicion1.Operator = ConditionOperator.LessEqual;
            _condicion1.Values.Add(fechaDT);
            _filtro.Conditions.Add(_condicion1);

            _consulta.Criteria.AddFilter(_filtro);

            _consulta.ColumnSet.AddColumns("atos_instalacionid", "atos_tarifaid"); //Poner las columnas
            EntityCollection _resultado = _servicioConsultas.RetrieveMultiple(_consulta);
            if (_resultado.Entities.Count == 0)
            {
                writelog("1- Final consulta Ínstalaciones desde hace un mes CON ERRORES");
                //hayErrores = true;
                return null;
            }
            writelog("1- Final consulta Ínstalaciones desde hace un mes. Número de Registros devueltos: " + _resultado.Entities.Count);
            return _resultado;
        }

        /*private EntityCollection consultaInstalaciones(IOrganizationService _servicioConsultas, ref Boolean hayErrores, String _periodo, String campo)
        {
            writelog("1- Inicio creación consulta instalaciones desde hace un mes");
            String format = "dd-MM-yyyy";
            IFormatProvider provider = new CultureInfo("es-ES");
            String fecha = "01-" + _periodo.Substring(4, 2) + "-" + _periodo.Substring(0, 4);
            DateTime fechaDT = DateTime.ParseExact(fecha, format, provider);
            writelog("Periodo: " + fechaDT.ToString("dd/MM/yyy") + " Fecha desde: " + fechaDT.AddMonths(-1).ToString("dd/MM/yyy"));

            QueryExpression _consulta = new QueryExpression("atos_instalacion"); //poner entidad de informes
            FilterExpression _filtro = new FilterExpression();
            _filtro.FilterOperator = LogicalOperator.And;

            ConditionExpression _condicion = new ConditionExpression();
            _condicion.AttributeName = campo;//"atos_fechasolicitud"; 
            _condicion.Operator = ConditionOperator.GreaterEqual;
            _condicion.Values.Add(fechaDT.AddMonths(-1));
            _filtro.Conditions.Add(_condicion);

            ConditionExpression _condicion1 = new ConditionExpression();
            _condicion1.AttributeName = campo;//"atos_fechasolicitud";
            _condicion1.Operator = ConditionOperator.LessEqual;
            _condicion1.Values.Add(fechaDT);
            _filtro.Conditions.Add(_condicion1);

            _consulta.Criteria.AddFilter(_filtro);

            _consulta.ColumnSet.AddColumns("atos_fechainiciovigencia", "atos_instalacionprovinciaid", "atos_comercializadoraactualid",
                "atos_distribuidoraid", "atos_tarifaid", "atos_historico"); //Poner las columnas
            EntityCollection _resultado = _servicioConsultas.RetrieveMultiple(_consulta);
            if (_resultado.Entities.Count == 0)
            {
                writelog("1- Final consulta Ínstalaciones desde hace un mes CON ERRORES");
                //hayErrores = true;
                return null;
            }
            writelog("1- Final consulta Ínstalaciones desde hace un mes. Número de Registros devueltos: " + _resultado.Entities.Count);
            return _resultado;
        }
        */
        #endregion


        #endregion FUNCIONES AUXILIARES

        #region UTILIDADES
        private void writelog(String texto)
        {
            tracingService.Trace(texto);
            if (createLog == true)
            {
                System.IO.File.AppendAllText(this.rutaLOG + "//logInformesATR.txt", "\n" + texto);
            }
        }





        #endregion UTILIDADES
    }

}