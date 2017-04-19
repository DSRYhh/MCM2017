using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using MySql.Data.MySqlClient;
using Path = System.IO.Path;

namespace CertDataReadOut
{
    class Program
    {
        static void Main(string[] args)
        {
            process();
            Console.ReadLine();
        }

        static async void process()
        {
            var paths = Directory.GetFiles(@"D:\Users Documents\Desktop\MCM2017Certs");

            List<PrizeInfo> list = new List<PrizeInfo>();
            foreach (var path in paths)
            {
                var info = await Task.Run(()=> ParseCert.Parse(path));
                await Task.Run(() =>
                {
                    var controlNumber = Path.GetFileNameWithoutExtension(path);
                    string ConnectionString = "server=localhost;uid=root;" +
                                      "pwd=notlove*;database=mcm2017;";
                    using (var conn = new MySqlConnection() { ConnectionString = ConnectionString })
                    {
                        conn.Open();
                        string command = string.Empty;
                        for (int i = 0; i < info.Member.Count; i++)
                        {
                            info.Member[i] = info.Member[i].Replace(@"'", @"\'");
                        }
                        info.Advisor = info.Advisor.Replace(@"'", @"\'");
                        info.School = info.School.Replace(@"'", @"\'");

                        if (info.Member.Count == 1)
                        {
                            command =
                                $"insert ignore into prize(id, school, advisor, prize, member1) values({controlNumber},'{info.School}', '{info.Advisor}', '{info.Prize}', '{info.Member[0]}'); ";
                        }
                        else if (info.Member.Count == 2)
                        {
                            command =
                                $"insert ignore into prize(id, school, advisor, prize, member1, member2) values({controlNumber},'{info.School}', '{info.Advisor}', '{info.Prize}', '{info.Member[0]}', '{info.Member[1]}'); ";
                        }
                        else if (info.Member.Count == 3)
                        {
                            command =
                                $"insert ignore into prize(id, school, advisor, prize, member1, member2, member3) values({controlNumber},'{info.School}', '{info.Advisor}', '{info.Prize}', '{info.Member[0]}', '{info.Member[1]}', '{info.Member[2]}'); ";
                        }
                        else
                        {
                            command =
                                   $"insert ignore into prize(id, school, advisor, prize) values({controlNumber},'{info.School}', '{info.Advisor}', '{info.Prize}'); ";
                        }

                        var cmd = new MySqlCommand
                        {
                            CommandText = command,
                            Connection = conn,
                            CommandType = CommandType.Text
                        };
                        cmd.ExecuteReader();
                        }
                });
            }

            
        }
    }
}
