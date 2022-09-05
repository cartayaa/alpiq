using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptDecrypt;


namespace CryptConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Write("Formato: CryptConsole secret_key texto\n");
            }
            else
            {
                String secret = args[0];
                String texto = args[1];

                String enctexto = Crypt.Encrypt(texto, secret);
                String dectexto = Crypt.Decrypt(enctexto, secret);

                Console.Write("Clave secreta: [" + secret + "]\n");
                Console.Write("Texto: [" + texto + "]\n");
                Console.Write("Texto encriptado: [" + enctexto + "]\n");
                Console.Write("Texto desencriptado: [" + dectexto + "]\n");

            }
        }
    }
}