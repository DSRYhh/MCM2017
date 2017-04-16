using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        static async void Task()
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
            for (int i = 50000; i < 80000; i++)
            {
                if (cache.Contains(i.ToString()))
                {
                    continue;
                }
                var log = await GetCert.SaveCert(i.ToString());
                if (i % 1000 == 0 && i != 50000)
                {
                    //Thread.Sleep(40000);
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
                        while (commands.Count != 0)
                        {
                            var command = commands.Dequeue();
                            MySqlCommand cmd = new MySqlCommand
                            {
                                CommandText = command,
                                Connection = conn,
                                CommandType = CommandType.Text
                            };
                            cmd.ExecuteReader();
                        }
                    }
                }
                if (log != null)
                {
                    commands.Enqueue(log);
                }
            }
        }
    }
}