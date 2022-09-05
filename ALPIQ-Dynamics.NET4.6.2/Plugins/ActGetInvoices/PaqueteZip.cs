using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Packaging;
using System.IO;

namespace ActGetInvoices
{
    class PaqueteZip
    {
        private FileStream zipStream;
        private Package paquete;

        public PaqueteZip(String nombreZip)
        {
            zipStream = File.Open(nombreZip, FileMode.Create);
            paquete = ZipPackage.Open(zipStream, FileMode.Create);
            
        }


        public void AddFile(Stream stream, string Name)
        {
            Uri partUriDocument = PackUriHelper.CreatePartUri(new Uri(Name, UriKind.Relative));
            PackagePart packagePartDocument = paquete.CreatePart(partUriDocument, "application/pdf", CompressionOption.Normal);
            CopyStream(stream, packagePartDocument.GetStream());
            stream.Close();
        }


        public void AddFile(String fichero, string Name)
        {
            AddFile(File.OpenRead(fichero), Name);
        }

        public void Close()
        {
            //paquete.DeletePart(PackUriHelper.CreatePartUri(new Uri(@"/[Content_Types].xml", UriKind.Relative)));
            paquete.Close();
            zipStream.Close();
        }


        private static void CopyStream(Stream source, Stream target)
        {
            const int bufSize = 0x1000;
            byte[] buf = new byte[bufSize];
            int bytesRead = 0;
            while ((bytesRead = source.Read(buf, 0, bufSize)) > 0)
                target.Write(buf, 0, bytesRead);
        }  
    }
}
