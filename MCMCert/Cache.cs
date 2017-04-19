using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MCMCert
{
    public class Cache
    {
        private const string ConnectionString = "server=localhost;uid=root;" +
                                                "pwd=notlove*;database=mcm2017;";

        public static Task InsertAsync(string command)
        {
            return Task.Run(() =>
            {
                using (var conn = new MySqlConnection() {ConnectionString = ConnectionString})
                {
                    conn.Open();
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