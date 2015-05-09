using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Newtonsoft.Json;
using TaxiShared;

namespace TaxiBackend
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            System.Net.ServicePointManager.DefaultConnectionLimit = 500;
            using (var system = ActorSystem.Create("TaxiBackend"))
            {
                var publisher = system.ActorOf(Props.Create(() => new PublisherActor()), "publisher");
                dynamic regions = GetJson("http://ladotbus.com/Regions");
                foreach (var region in regions)
                {
                    var routes = GetJson("http://ladotbus.com/Region/" + region.ID + "/Routes");
                    if (routes == null)
                        continue;
                    
                    foreach (var route in routes)
                    {

                        Console.WriteLine("Found route {0}",route.ID);
                        RunFetchLoopAsync(publisher, "http://ladotbus.com/Route/" + route.ID + "/Vehicles/");
                    }
                }

                Console.ReadLine();
            }
        }

        private static async void RunFetchLoopAsync(IActorRef publisher, string url)
        {
            await Task.Yield();
            try
            {
                var c = new WebClient();
                while (true)
                {
                    var data = await c.DownloadDataTaskAsync(new Uri(url));
                    var str = Encoding.UTF8.GetString(data);
                    dynamic res = JsonConvert.DeserializeObject(str);
                    Console.WriteLine("Downloaded {0}",url);

                    foreach (var bus in res)
                    {
                        string id = bus.ID;
                        double lat = bus.Latitude;
                        double lon = bus.Longitude;

                        publisher.Tell(new Publisher.Position(lon, lat, id));                    
                    }

                    //how long should we wait before polling again?
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
            catch
            {
                Console.WriteLine("Missing Route {0}", url);
            }
        }

        private static dynamic GetJson(string url)
        {
            try
            {
                var c = new WebClient();
                var data = c.DownloadData(new Uri(url));
                var str = Encoding.UTF8.GetString(data);

                dynamic res = JsonConvert.DeserializeObject(str);
                return res;
            }
            catch
            {
                return null;
            }
        }
    }
}