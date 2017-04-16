using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MCMCert
{
    public class GetCert
    {
        private const int WaitingTime = 3000;

        private const string BaseAddress = @"http://www.comap-math.com/";
        private const string BasePath = @"D:\Users Documents\Desktop\MCM2017Certs";
        public static async Task<string> SaveCert(string controlNumber)
        {

            var path = Path.Combine(BasePath, $"{controlNumber}.pdf");
            if (File.Exists(path))
            {
                Console.WriteLine($"File {controlNumber}.pdf existed. Abort.");
                return $"insert ignore into cache (entry,type) values ('{controlNumber}','Exist');";
            }
            else
            {
                //return null;
                Console.WriteLine($"File {controlNumber}.pdf not found. Abort.");
            }

            using (var handler = new HttpClientHandler())
            using (var client = new HttpClient(handler) {BaseAddress = new Uri(BaseAddress)})
            {
                try
                {
                    var response = await client.GetAsync($"mcm/2017Certs/{controlNumber}.pdf");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStreamAsync();

                        if (!File.Exists(path))
                        {
                            using (var stream = File.Create(path))
                            {
                                content.CopyTo(stream);
                                Console.WriteLine($"Download {controlNumber} competed.");
                                return null;
                            }
                        }
                    }
                    else
                    {
                        if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                        {
                            Console.WriteLine($"{response.StatusCode} occured in {controlNumber}, retry.");
                            Thread.Sleep(WaitingTime);
                            return await SaveCert(controlNumber);
                        }
                        else if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            Console.WriteLine($"{response.StatusCode} occured in {controlNumber}, abort.");
                            return $"insert ignore into cache (entry,type) values ('{controlNumber}','NotFound');";
                        }
                        Console.WriteLine($"{response.StatusCode} occured in {controlNumber}, abort.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.Message} occured in {controlNumber}, retry.");
                    Thread.Sleep(WaitingTime);
                    return await SaveCert(controlNumber);
                }
                
            }
            return null;
        }
    }
}