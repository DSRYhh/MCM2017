using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MCMCert
{
    class Program
    {
        static void Main(string[] args)
        {
            Task();
            Console.ReadLine();
        }

        private static void Task()
        {
            string connectionString = "server=localhost;uid=root;" +
    "pwd=notlove*;database=mcm2017;";
            HashSet<string> cache = new HashSet<string>();
            using (
                MySql.Data.MySqlClient.MySqlConnection conn = new MySqlConnection()
                {
                    ConnectionString = connectionString
                })
            {

                try
                {
                    conn.Open();
                }
                catch (MySql.Data.MySqlClient.MySqlException)
                {
                    throw;
                }
                MySqlCommand cmd = new MySqlCommand
                {
                    CommandText = "select entry from cache;",
                    Connection = conn,
                    CommandType = CommandType.Text
                };
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cache.Add((string) reader[0]);
                }
            }

            Queue<string> commands = new Queue<string>();
            int count = 0;
            for (int i = 50000; i < 80000; i++)
            {
                if (cache.Contains(i.ToString()))
                {
                    continue;
                }
                GetCert.SaveCert(i.ToString());
                count++;
                if (count % 1000 == 0 && i != 0)
                {
                    Thread.Sleep(20000);
                }
            }
        }
    }
}