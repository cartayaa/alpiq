using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptDecrypt
{
    class Crypt
    {
        static CipherMode Mode = System.Security.Cryptography.CipherMode.ECB;

        /*private static String CryptSecretKey(String p_secretKey)
        {
            return Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(p_secretKey));
        }*/

        private static String EncryptInternal(String p_InputString, String p_SecretKey)
        {
            try
            {
                TripleDESCryptoServiceProvider Des = new TripleDESCryptoServiceProvider();
                //Put the string into a byte array
                byte[] InputbyteArray = Encoding.UTF8.GetBytes(p_InputString);
                //Create the crypto objects, with the key, as passed in
                MD5CryptoServiceProvider hashMD5 = new MD5CryptoServiceProvider();
                Des.Key = hashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(p_SecretKey));
                Des.Mode = Crypt.Mode;


                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, Des.CreateEncryptor(), CryptoStreamMode.Write);
                //Write the byte array into the crypto stream
                //(It will end up in the memory stream)
                cs.Write(InputbyteArray, 0, InputbyteArray.Length);
                cs.FlushFinalBlock();
                //Get the data back from the memory stream, and into a string
                StringBuilder ret = new StringBuilder();
                byte[] b = ms.ToArray();
                ms.Close();
                int I = 0;
                for (I = 0; I <= b.GetUpperBound(0); I++)
                {
                    //Format as hex
                    ret.AppendFormat("{0:X2}", b[I]);
                }
                return ret.ToString();
            }
            catch (System.Security.Cryptography.CryptographicException generatedExceptionName)
            {
                return "";
            }
        }

        private static String DecryptInternal(String p_InputString, String p_SecretKey)
        {
            if (String.IsNullOrEmpty(p_InputString))
            {
                return String.Empty;
            }
            else {
                StringBuilder ret = new StringBuilder();
                byte[] InputbyteArray = new byte[Convert.ToInt32(p_InputString.Length) / 2];
                TripleDESCryptoServiceProvider Des = new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider hashMD5 = new MD5CryptoServiceProvider();

                try
                {
                    Des.Key = hashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(p_SecretKey));
                    Des.Mode = Crypt.Mode;
                    for (int X = 0; X <= InputbyteArray.Length - 1; X++)
                    {
                        Int32 IJ = (Convert.ToInt32(p_InputString.Substring(X * 2, 2), 16));
                        ByteConverter BT = new ByteConverter();
                        InputbyteArray[X] = new byte();
                        InputbyteArray[X] = Convert.ToByte(BT.ConvertTo(IJ, typeof(byte)));
                    }

                    MemoryStream ms = new MemoryStream();
                    CryptoStream cs = new CryptoStream(ms, Des.CreateDecryptor(), CryptoStreamMode.Write);

                    //Flush the data through the crypto stream into the memory stream
                    cs.Write(InputbyteArray, 0, InputbyteArray.Length);
                    cs.FlushFinalBlock();

                    //Get the decrypted data back from the memory stream
                    byte[] B = ms.ToArray();
                    ms.Close();
                    for (int I = 0; I <= B.GetUpperBound(0); I++)
                    {
                        ret.Append(Convert.ToChar(B[I]));
                    }
                }
                catch (Exception ex)
                {
                    //   ME.Publish("SF.Utils.Utils", "DecryptString", ex, ManageException_Enumerators.ErrorLevel.FatalError);
                    return String.Empty;
                }
                return ret.ToString();
            }
        } 
        
        private static InfoCrypt EncryptDecrypt(InfoCrypt p_infoCrypt)
        {

            if (String.IsNullOrEmpty(p_infoCrypt.ClaveEncriptada))
                p_infoCrypt.ClaveEncriptada = Crypt.EncryptInternal(p_infoCrypt.Clave, p_infoCrypt.Clave);

            if (String.IsNullOrEmpty(p_infoCrypt.TextoEncriptado))
            {
                p_infoCrypt.TextoEncriptado = Crypt.EncryptInternal(p_infoCrypt.Texto, p_infoCrypt.ClaveEncriptada);
            }
            else if (String.IsNullOrEmpty(p_infoCrypt.Texto) && !String.IsNullOrEmpty(p_infoCrypt.TextoEncriptado))
            {
                p_infoCrypt.Texto = Crypt.DecryptInternal(p_infoCrypt.TextoEncriptado, p_infoCrypt.ClaveEncriptada);
            }
            return p_infoCrypt;
        }

        public static String Encrypt(string p_InputString, string p_SecretKey)
        {
            try
            {
                InfoCrypt infoCrypt = new InfoCrypt();
                infoCrypt.Clave = p_SecretKey;
                infoCrypt.Texto = p_InputString;
                infoCrypt = Crypt.EncryptDecrypt(infoCrypt);
                return infoCrypt.TextoEncriptado;                
            }
            catch (System.Security.Cryptography.CryptographicException generatedExceptionName)
            {
                return "";
            }
        }

        public static String Decrypt(string p_InputString, string p_SecretKey)
        {
            if (String.IsNullOrEmpty(p_InputString))
            {
                return String.Empty;
            }
            else {
                InfoCrypt infoCrypt = new InfoCrypt();
                infoCrypt.Clave = p_SecretKey;
                infoCrypt.TextoEncriptado = p_InputString;
                infoCrypt = Crypt.EncryptDecrypt(infoCrypt);
                return infoCrypt.Texto;                
            }

        }
    }


    public class InfoCrypt
    {
        private String clave;
        private String texto;
        private String claveencriptada;
        private String textoencriptado;

        public String Clave
        {
            get
            {
                return this.clave;
            }
            set
            {
                this.clave = value;
            }
        }

        public String Texto
        {
            get
            {
                return this.texto;
            }
            set
            {
                this.texto = value;
            }
        }

        public String ClaveEncriptada
        {
            get
            {
                return this.claveencriptada;
            }
            set
            {
                this.claveencriptada = value;
            }
        }

        public String TextoEncriptado
        {
            get
            {
                return this.textoencriptado;
            }
            set
            {
                this.textoencriptado = value;
            }
        }
    }
}
