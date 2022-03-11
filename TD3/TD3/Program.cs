using System.Device.Location;
using System.Text.Json;

namespace TD3
{

    public class Root
    {
        public int number { get; set; }
        public string contract_name { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public Position position { get; set; }
        public bool banking { get; set; }
        public bool bonus { get; set; }
        public int bike_stands { get; set; }
        public int available_bike_stands { get; set; }
        public int available_bikes { get; set; }
        public string status { get; set; }
        public object last_update { get; set; }
    }

    public class Position
    {
        public double lat { get; set; }
        public double lng { get; set; }
        public Position(double lat, double lng) { this.lat = lat; this.lng = lng; }
    }

    internal class JCDecaux
    {
        static readonly HttpClient client = new HttpClient();

        static async Task exercice1()
        {
            string api_key = "fd4e518c5cf2c25e0ae2b34e82b286365c05487b";
            HttpResponseMessage response = await client.GetAsync($"https://api.jcdecaux.com/vls/v1/stations?apiKey={api_key}");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            List<Root> jsonData = JsonSerializer.Deserialize<List<Root>>(responseBody);

            foreach (Root element in jsonData)
            {
                Console.WriteLine(element.contract_name);

            }
        }

        static async Task exercice2(String contract)
        {
            string api_key = "fd4e518c5cf2c25e0ae2b34e82b286365c05487b";
            // On précise ici le nom du contrat dans la requête GET avec l'args[0] de l'appel
            HttpResponseMessage response = await client.GetAsync($"https://api.jcdecaux.com/vls/v1/stations?contract={contract}&apiKey={api_key}");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            List<Root> jsonData = JsonSerializer.Deserialize<List<Root>>(responseBody);

            foreach (Root element in jsonData)
            {
                Console.WriteLine(element.name);
            }
        }

        static async Task exercice3(String contract, int station)
        {
            string api_key = "fd4e518c5cf2c25e0ae2b34e82b286365c05487b";
            HttpResponseMessage response = await client.GetAsync($"https://api.jcdecaux.com/vls/v1/stations/{station}?contract={contract}&apiKey={api_key}");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            Root jsonData = JsonSerializer.Deserialize<Root>(responseBody);

            Console.WriteLine(jsonData.name + " \n" + jsonData.contract_name + "\n" + jsonData.name + "\n" + jsonData.address + "\n" +
                jsonData.position + "\n" + jsonData.banking + "\n" + jsonData.bonus + "\n" + jsonData.bike_stands + "\n" +
                jsonData.available_bike_stands + "\n" + jsonData.available_bikes + "\n" + jsonData.status + "\n" + jsonData.last_update);
        }

        static async Task exercice4(String contract, double lat, double lng)
        {
            GeoCoordinate userCurrentPosition = new GeoCoordinate(lat, lng);
            string api_key = "fd4e518c5cf2c25e0ae2b34e82b286365c05487b";
            HttpResponseMessage response = await client.GetAsync($"https://api.jcdecaux.com/vls/v1/stations?contract={contract}&apiKey={api_key}");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            List<Root> jsonData = JsonSerializer.Deserialize<List<Root>>(responseBody);
            Root nearestStation = null;
            double distanceMin = 100000;

            foreach (Root element in jsonData)
            {
                GeoCoordinate posCurrentStation = new GeoCoordinate(element.position.lat, element.position.lng);
                if (posCurrentStation.GetDistanceTo(userCurrentPosition) < distanceMin)
                {
                    distanceMin = posCurrentStation.GetDistanceTo(userCurrentPosition);
                    nearestStation = element;
                }
            }
            if (nearestStation != null)
            {
                Console.WriteLine($"The nearest station to the position ({lat},{lng}) is {nearestStation.name} at ({nearestStation.position.lat},{nearestStation.position.lng}) situated {(distanceMin)}m from your current position.");
            }
            else { Console.WriteLine($"No station were found near your current position ({lat},{lng})"); }
        }

        static async Task Main(string[] args)
        {
            await exercice1();

            await exercice2("toulouse");
           
            await exercice3("toulouse",55);

            await exercice4("toulouse", 43.6089519605732, 1.4410035987314);
        }
    }
}