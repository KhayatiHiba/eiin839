using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        public async static Task Main()
        {
            HttpClient client = new HttpClient();
            String url = "http://localhost:8080/exercice3/substract?param1=2&param2=12";
            var response = client.GetAsync(url);
            String responseBody = await response.Result.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);
        }
    }
}
