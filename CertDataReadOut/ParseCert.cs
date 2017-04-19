using System;
using System.Collections.Generic;
using System.Linq;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace CertDataReadOut
{
    public class ParseCert
    {
        public static PrizeInfo Parse(string path)
        {
            Console.WriteLine($"Begin to process {path}");

            //Create an instance of our strategy
            var t = new MyLocationTextExtractionStrategy();

            //Parse page 1 of the document above
            using (var r = new PdfReader(path))
            {
                var ex = PdfTextExtractor.GetTextFromPage(r, 1, t);
            }

            

            //Loop through each chunk found
            Dictionary<float, string> strings = new Dictionary<float, string>();
            foreach (var p in t.myPoints)
            {
                var bottom = p.Rect.Bottom;
                if (strings.ContainsKey(bottom))
                {
                    strings[bottom] += p.Text;
                }
                else
                {
                    strings.Add(bottom, p.Text);
                }
            }

            var list = strings.ToList();
            list.Sort((a, b) => b.Key.CompareTo(a.Key));
            try
            {
                var indexMember = list.IndexOf((from item in list
                                                where item.Value.Contains("Be It Known That The Team Of")
                                                select item).First());
                var indexAdvisor = list.IndexOf((from item in list
                                                 where item.Value.Contains("With Faculty Advisor")
                                                 select item).First());
                var indexUniversity = list.IndexOf((from item in list
                                                    where item.Value.Trim() == "Of"
                                                    select item).First());
                var indexPrize = list.IndexOf((from item in list
                                               where item.Value.Contains("Was Designated As")
                                               select item).First());
                PrizeInfo info = new PrizeInfo();
                for (int i = indexMember + 1; i < indexAdvisor; i++)
                {
                    info.Member.Add(list[i].Value);
                }
                info.Advisor = list[indexAdvisor + 1].Value;
                info.School = list[indexUniversity + 1].Value;
                info.Prize = list[indexPrize + 1].Value;

                return info;
            }
            catch (Exception e)
            {
                foreach (var pair in list)
                {
                    Console.WriteLine(pair);
                }
            }
            return null;
        }

        private static bool ContainNonsense(string str)
        {
            HashSet<string> nonsense = new HashSet<string>()
            {
                "Administered by",
                "With support from",
                "Certificate of Achievement",
                "Interdisciplinary Contest In Modeling",
                "?",
                "Be It Known That The Team Of",
                "With Faculty Advisor",
                "Of",
                "Was Designated As",
                "D. Chris Arney, Contest Director",
                "2017",
                "®",
                "Head Judge",
                "Mathematical Contest In Modeling"
            };

            foreach (var item in nonsense)
            {
                if (str.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }

        private class MyLocationTextExtractionStrategy : LocationTextExtractionStrategy
        {
            //Hold each coordinate
            public List<RectAndText> myPoints = new List<RectAndText>();

            //Automatically called for each chunk of text in the PDF
            public override void RenderText(TextRenderInfo renderInfo)
            {
                base.RenderText(renderInfo);

                //Get the bounding box for the chunk of text
                var bottomLeft = renderInfo.GetDescentLine().GetStartPoint();
                var topRight = renderInfo.GetAscentLine().GetEndPoint();

                //Create a rectangle from it
                var rect = new iTextSharp.text.Rectangle(
                                                        bottomLeft[Vector.I1],
                                                        bottomLeft[Vector.I2],
                                                        topRight[Vector.I1],
                                                        topRight[Vector.I2]
                                                        );

                //Add this to our main collection
                this.myPoints.Add(new RectAndText(rect, renderInfo.GetText()));
            }
        }

        //Helper class that stores our rectangle and text
        private class RectAndText
        {
            public iTextSharp.text.Rectangle Rect;
            public String Text;
            public RectAndText(iTextSharp.text.Rectangle rect, String text)
            {
                this.Rect = rect;
                this.Text = text;
            }
        }
    }
}