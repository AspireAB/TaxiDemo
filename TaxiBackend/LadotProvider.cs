using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Newtonsoft.Json;
using TaxiShared;

namespace TaxiBackend
{
    public static class LadotProvider
    {
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

        public static void Run(IActorRef publisher)
        {
            dynamic regions = GetJson("http://ladotbus.com/Regions");
            foreach (var region in regions)
            {
                var routes = GetJson("http://ladotbus.com/Region/" + region.ID + "/Routes");
                if (routes == null)
                    continue;

                foreach (var route in routes)
                {
                    Console.WriteLine("Found route {0}", route.ID);
                    RunFetchLoopAsync(publisher, "http://ladotbus.com/Route/" + route.ID + "/Vehicles/", "LAdot");
                }
            }
        }

        private static async void RunFetchLoopAsync(IActorRef publisher, string url, string source)
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
                    //   Console.WriteLine("Downloaded {0}",url);

                    foreach (var bus in res)
                    {
                        string id = bus.ID;
                        double lat = bus.Latitude;
                        double lon = bus.Longitude;

                        publisher.Tell(new Presenter.Position(lon, lat, id, source));
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
    }
}