using Negocio.Constantes;
using Negocio.Entidades;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.Services.Protocols;
using Negocio.Logica;
using System.Diagnostics;
using System.Net;

namespace ImportATR
{
    class Program
    {
        private static IOrganizationService _servicioCRM;
        private static String _dirFichs = ConfigurationManager.AppSettings[Configuracion.CarpetaEntrada].ToString();
        private static String _dirFichsNoUri = ConfigurationManager.AppSettings[Configuracion.CarpetaEntradaNoUri].ToString();
        private static String _dirLog = ConfigurationManager.AppSettings[Configuracion.CarpetaLog].ToString();
        private static String _dirFichsProcesados = ConfigurationManager.AppSettings[Configuracion.CarpetaProcesados].ToString();
        private static String _dirFichsErrores = ConfigurationManager.AppSettings[Configuracion.CarpetaErroneos].ToString();

        private static String _servidor = ConfigurationManager.AppSettings[Configuracion.Servidor].ToString();
        private static String _organizacion = ConfigurationManager.AppSettings[Configuracion.Organizacion].ToString();
        private static String _usuario = ConfigurationManager.AppSettings[Configuracion.Usuario].ToString();
        private static String _pass = ConfigurationManager.AppSettings[Configuracion.Contrasena].ToString();
        private static String _claveEncriptacion = ConfigurationManager.AppSettings[Configuracion.ClaveEncriptacion].ToString();

        private static String _atrMap = ConfigurationManager.AppSettings[Configuracion.AtrMap].ToString();

        private static List<String> _logFichero = new List<string>();

        private static void mapeaUnidad()
        {
            if (_atrMap != "" && !Directory.Exists(_dirFichs))
            {
                System.Diagnostics.Process objProcess = new Process();
                objProcess.StartInfo.FileName = _atrMap;
                objProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;    // to hide the command window popping up
                objProcess.Start();
                objProcess.WaitForExit(1000); // Gives time for the process to complete operation.
                                              // After code is executed, call the dispose() method
                objProcess.Dispose();
            }
        }

        [STAThread]
        static int Main(string[] args)
        {
            if (args.Length > 0)
            {
                try
                {

                    if (!String.IsNullOrEmpty(_claveEncriptacion))
                    {
                        _servidor = CryptDecrypt.Crypt.Decrypt(_servidor, _claveEncriptacion);
                        _organizacion = CryptDecrypt.Crypt.Decrypt(_organizacion, _claveEncriptacion);
                        _usuario = CryptDecrypt.Crypt.Decrypt(_usuario, _claveEncriptacion);
                        _pass = CryptDecrypt.Crypt.Decrypt(_pass, _claveEncriptacion);
                    }
                    conectarCRM();
                    mapeaUnidad();
                    if (!leerFichero(args[0]))
                    {
                        return -1;
                    }
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
            else
            {

                if (!String.IsNullOrEmpty(_claveEncriptacion))
                {
                    _servidor = CryptDecrypt.Crypt.Decrypt(_servidor, _claveEncriptacion);
                    _organizacion = CryptDecrypt.Crypt.Decrypt(_organizacion, _claveEncriptacion);
                    _usuario = CryptDecrypt.Crypt.Decrypt(_usuario, _claveEncriptacion);
                    _pass = CryptDecrypt.Crypt.Decrypt(_pass, _claveEncriptacion);
                }
                conectarCRM();
                mapeaUnidad();
                leerFicheros();
            }
            return 0;
        }

        private static bool leerFichero(string pFicheroEntrada)
        {
            bool salida = true;

            String nombreFicheroLog = String.Format("ATR_{0}.log", DateTime.Now.ToString("yyyyMMdd"));

            try
            {
                List<string> listaFichTratados = new List<string>();
                List<string> listaFichErroneos = new List<string>();

                System.IO.File.AppendAllText(System.IO.Path.Combine(_dirLog, nombreFicheroLog),
                    String.Format("\r\n{0} ---- Leer directorio {1}\r\n", DateTime.Now.ToString(), _dirFichs));

                string[] fich = Directory.GetFiles(_dirFichs, "*.xml");
                //StreamReader objStreamReader;

                System.IO.File.AppendAllText(System.IO.Path.Combine(_dirLog, nombreFicheroLog),
                    String.Format("\r\n{0} ---- Comienzo del proceso de lectura ATR\r\n", DateTime.Now.ToString()));

                System.IO.File.AppendAllText(System.IO.Path.Combine(_dirLog, nombreFicheroLog), String.Format("LEYENDO FICHERO ATR - {0}\r\n", pFicheroEntrada));
                bool? result = tratarFichero(pFicheroEntrada, System.IO.Path.GetFileName(pFicheroEntrada));
                if (result == true)
                {
                    String nombreFicheroTratado = pFicheroEntrada;
                    String nombreFicheroOut = DateTime.Now.ToString("yyyyMMdd_HHmmsss_") + pFicheroEntrada;
                    System.IO.File.Copy(System.IO.Path.Combine(_dirFichs, nombreFicheroTratado), System.IO.Path.Combine(_dirFichsProcesados, nombreFicheroOut), true);
                    System.IO.File.Delete(System.IO.Path.Combine(_dirFichs, nombreFicheroTratado));

                    System.IO.File.AppendAllText(System.IO.Path.Combine(_dirLog, nombreFicheroLog),
                        String.Format("ARCHIVO {0} MOVIDO A LA RUTA DE FICHEROS PROCESADOS \r\n", pFicheroEntrada));
                }
                else if(result == false)
                {
                    salida = false;
                    volcarLogFichero(System.IO.Path.Combine(_dirLog, nombreFicheroLog));
                    String nombreFicheroErroneo = pFicheroEntrada;
                    String nombreFicheroOut = DateTime.Now.ToString("yyyyMMdd_HHmmsss_") + pFicheroEntrada;

                    System.IO.File.Copy(System.IO.Path.Combine(_dirFichs, nombreFicheroErroneo), System.IO.Path.Combine(_dirFichsErrores, nombreFicheroOut), true);
                    System.IO.File.Delete(System.IO.Path.Combine(_dirFichs, nombreFicheroErroneo));

                    System.IO.File.AppendAllText(System.IO.Path.Combine(_dirLog, nombreFicheroLog),
                                String.Format("ARCHIVO {0} MOVIDO A LA RUTA DE FICHEROS ERRONEOS \r\n", pFicheroEntrada));
                    System.IO.File.AppendAllText(System.IO.Path.Combine(_dirLog, nombreFicheroLog),
                        String.Format("{0} ---- fin del proceso de lectura ATR\r\n", DateTime.Now.ToString()));
                }

                String.Format("{0} ---- fin del proceso de lectura ATR\r\n", DateTime.Now.ToString());
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(System.IO.Path.Combine(_dirLog, nombreFicheroLog),
                                String.Format("Error en el proceso de importación de ATR:{0}\n", ex.Message));
                salida = false;
            }
            return salida;
        }

        private static void conectarCRM()
        {
            try
            {
                string uri365 = _servidor;
                string user365 = _usuario;
                string pass365 = _pass;

                string connectionString = string.Format("Url={0}; Username={1}; Password={2}; authtype=Office365", uri365, user365, pass365);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                CrmServiceClient conn = new Microsoft.Xrm.Tooling.Connector.CrmServiceClient(connectionString);
                _servicioCRM = (IOrganizationService)conn.OrganizationWebProxyClient != null ? (IOrganizationService)conn.OrganizationWebProxyClient : (IOrganizationService)conn.OrganizationServiceProxy;
            }
            catch (SoapException soapEx)
            {
                throw new Exception(String.Format("Error en el acceso a CRM: {0}", soapEx.Detail.InnerText));
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error en el acceso a CRM: {0}", ex.Message));
            }

            //formatoDatos = CultureInfo.CreateSpecificCulture("es-ES"); ;
        }

        public static void leerFicheros()
        {
            String nombreFicheroLog = String.Format("ATR_{0}.log", DateTime.Now.ToString("yyyyMMdd"));
            //DateTime debugStart = DateTime.Now;

            try
            {
                List<string> listaFichTratados = new List<string>();
                List<string> listaFichErroneos = new List<string>();

                System.IO.File.AppendAllText(System.IO.Path.Combine(_dirLog, nombreFicheroLog),
                    String.Format("\r\n{0} ---- Leer directorio {1}\r\n", DateTime.Now.ToString(), _dirFichs));


                string[] fich = Directory.GetFiles(_dirFichs, "*.xml");
                //StreamReader objStreamReader;

                System.IO.File.AppendAllText(System.IO.Path.Combine(_dirLog, nombreFicheroLog),
                        String.Format("\r\n{0} ---- Comienzo del proceso de lectura ATR\r\n", DateTime.Now.ToString()));

                for (int numFichero = 0; numFichero <= fich.Length - 1; numFichero++)
                {
                    //Console.WriteLine("LEYENDO FICHERO INFORBOE - {0}\r\n", fich[numFichero].ToString());
                    System.IO.File.AppendAllText(System.IO.Path.Combine(_dirLog, nombreFicheroLog),
                        String.Format("LEYENDO FICHERO ATR - {0}\r\n", fich[numFichero].ToString()));

                    bool? result = tratarFichero(fich[numFichero].ToString(), System.IO.Path.GetFileName(fich[numFichero].ToString()));
                    if (result == true)
                    {
                        if (!listaFichTratados.Contains(System.IO.Path.GetFileName(fich[numFichero].ToString())))
                            listaFichTratados.Add(System.IO.Path.GetFileName(fich[numFichero].ToString()));
                    }
                    else if(result == false)
                    {
                        volcarLogFichero(System.IO.Path.Combine(_dirLog, nombreFicheroLog));
                        if (!listaFichErroneos.Contains(System.IO.Path.GetFileName(fich[numFichero].ToString())))
                            listaFichErroneos.Add(System.IO.Path.GetFileName(fich[numFichero].ToString()));
                    }
                }

                for (int i = 0; i <= listaFichTratados.Count - 1; i++)
                {
                    if (!listaFichErroneos.Contains(listaFichTratados[i].ToString()))
                    {
                        String nombreFicheroTratado = listaFichTratados[i].ToString();
                        String nombreFicheroOut = DateTime.Now.ToString("yyyyMMdd_HHmmsss_") + listaFichTratados[i].ToString();
                        System.IO.File.Copy(System.IO.Path.Combine(_dirFichs, nombreFicheroTratado), System.IO.Path.Combine(_dirFichsProcesados, nombreFicheroOut), true);
                        System.IO.File.Delete(System.IO.Path.Combine(_dirFichs, nombreFicheroTratado));

                        System.IO.File.AppendAllText(System.IO.Path.Combine(_dirLog, nombreFicheroLog),
                                String.Format("ARCHIVO {0} MOVIDO A LA RUTA DE FICHEROS PROCESADOS \r\n", listaFichTratados[i].ToString()));
                        //Console.WriteLine("ARCHIVO {0} MOVIDO A LA RUTA DE FICHEROS PROCESADOS \r\n", listaFichTratados[i].ToString());
                    }
                }
                for (int i = 0; i <= listaFichErroneos.Count - 1; i++)
                {
                    String nombreFicheroErroneo = listaFichErroneos[i].ToString();
                    String nombreFicheroOut = DateTime.Now.ToString("yyyyMMdd_HHmmsss_") + listaFichErroneos[i].ToString();

                    System.IO.File.Copy(System.IO.Path.Combine(_dirFichs, nombreFicheroErroneo), System.IO.Path.Combine(_dirFichsErrores, nombreFicheroOut), true);
                    System.IO.File.Delete(System.IO.Path.Combine(_dirFichs, nombreFicheroErroneo));

                    System.IO.File.AppendAllText(System.IO.Path.Combine(_dirLog, nombreFicheroLog),
                                String.Format("ARCHIVO {0} MOVIDO A LA RUTA DE FICHEROS ERRONEOS \r\n", listaFichErroneos[i].ToString()));
                }
                System.IO.File.AppendAllText(System.IO.Path.Combine(_dirLog, nombreFicheroLog),
                        String.Format("{0} ---- fin del proceso de lectura ATR\r\n", DateTime.Now.ToString()));
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(System.IO.Path.Combine(_dirLog, nombreFicheroLog),
                                String.Format("Error en el proceso de importación de ATR:{0}", ex.Message));
            }
            //DateTime debugEnd = DateTime.Now;
            //TimeSpan ts = new TimeSpan(debugEnd.Ticks - debugStart.Ticks);
        }

        private static void volcarLogFichero(String pRutaFicheroLog)
        {
            foreach (String mensaje in _logFichero)
            {
                System.IO.File.AppendAllText(pRutaFicheroLog, mensaje + Environment.NewLine);
            }
            _logFichero.Clear();
        }

        private static bool? tratarFichero(string pFicheroEntrada, string pNombreFichero)
        {
            bool salida = false;
            try
            {
                Entity datosSolicitudATR = new Entity("atos_solicitudatr");

                #region Mapper Test Code
                //XmlDocument xDoc = new XmlDocument();
                //xDoc.Load(pFicheroEntrada);
                //Mapper mapper = new Mapper(_servicioCRM);
                //mapper.AgregarMapeo(new Mapeo { CampoXml = "CodigoREEEmpresaEmisora", CampoCRM = "atos_distribuidoraid", LookupTabla = "atos_distribuidora", LookupFiltros = new string[] { "atos_codigoocsum" }, TipoMapeo = MapperTipo.Lookup });
                //mapper.ParseXml(xDoc, datosSolicitudATR);
                #endregion

                ProcesoBase proceso = ProcesoFactoria.CrearProceso(pFicheroEntrada, pNombreFichero, _logFichero, _servicioCRM);

                if (proceso == null) return null;

                if (proceso.ValidarFichero() && proceso.ValidarMensaje())
                {
                    ProcesoResultado resultado = proceso.EjecutarPaso(datosSolicitudATR);
                    salida = true;
                    proceso.AdjuntarXml(resultado.SolicitudId, _dirFichsNoUri, pNombreFichero);
                }
                else
                    _logFichero.Add(String.Format("Error en la lectura del fichero:{0};{1}", pNombreFichero, "El proceso y paso son datos obligatorios"));
            }
            catch (Exception ex)
            {
                _logFichero.Add(String.Format("Error en la lectura del fichero:{0};{1}", pNombreFichero, ex.Message));
                salida = false;
            }
            return salida;
        }
    }
}
