using System.Collections.Generic;
using iTextSharp.text;

namespace CertDataReadOut
{
    public class PrizeInfo
    {
        public string School { get; set; }
        public string Advisor { get; set; }
        public string Prize { get; set; }

        public List<string> Member { get; set; }

        public PrizeInfo()
        {
            Member = new List<string>();
        }
    }
}