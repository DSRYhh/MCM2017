using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Path = System.IO.Path;

namespace CertDataReadOut
{
    class Program
    {
        static void Main(string[] args)
        {
            var paths = Directory.GetFiles(@"D:\Users Documents\Desktop\MCM2017Certs");

            foreach (var path in paths)
            {
                ParseCert.Parse(path);
            }
        }
    }
}
