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
            ServicePointManager.DefaultConnectionLimit = 500;
            using (var system = ActorSystem.Create("TaxiBackend"))
            {
                var publisher = system.ActorOf(Props.Create(() => new PublisherActor()), "publisher");

                RunGöteborgFetchLoopAsync(publisher);

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

        private static async void RunGöteborgFetchLoopAsync(IActorRef publisher)
        {
            var url =
                "http://crossorigin.me/http://reseplanerare.vasttrafik.se/bin/query.exe/dny?&look_minx=10044745&look_maxx=12040389&look_miny=57025027&look_maxy=58406811&tpl=trains2json&look_productclass=1023&look_json=yes&performLocating=1&look_nv=zugposmode|2|get_ageofreport|yes|get_rtmsgstatus|yes|get_linenumber|yes|interval|10000|intervalstep|10000|&unique=1399449664000&";
            var c = new WebClient();
            while (true)
            {
                var data = await c.DownloadDataTaskAsync(new Uri(url));
                var str = Encoding.UTF8.GetString(data);
                dynamic res = JsonConvert.DeserializeObject(str);

                foreach (var bus in res.look.trains)
                {
                    string id = bus.trainid;
                    double lat = bus.y / 1000000d;
                    double lon = bus.x / 1000000d;

                    publisher.Tell(new Publisher.Position(lon, lat, id));
                }

                //how long should we wait before polling again?
                await Task.Delay(TimeSpan.FromSeconds(1));
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
                 //   Console.WriteLine("Downloaded {0}",url);

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