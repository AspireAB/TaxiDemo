using System;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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

           //     RunBar(publisher);
            //    publisher.Tell(new Publisher.Initialize(ActorRefs.Nobody));
 
             //   RunSL(publisher);

              //  Spam(publisher);

                RunLondon(publisher);

                RunFoo(publisher);

                RunGöteborg(publisher);

                RunLadotBus(publisher);

                Console.ReadLine();
            }
        }

        private static async void Spam(IActorRef publisher)
        {
            await Task.Yield();
            while (true)
            {
                for (int i = 0; i < 1000; i++)
                {
                    publisher.Tell(new Publisher.Position(0, 0, "foo"));
                }
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private static void RunBar(IActorRef publisher)
        {
            for (int i = 0; i < 500; i++)
            {
                RunBar(publisher,i);
            }
        }
        private static async void RunBar(IActorRef publisher,int agencyId)
        {
            await Task.Yield();
            try
            {
                var url = "http://feeds.transloc.com/3/vehicle_statuses.jsonp?agencies=" + agencyId;
                var c = new WebClient();
                while (true)
                {
                    var data = await c.DownloadDataTaskAsync(new Uri(url));
                    var str = Encoding.UTF8.GetString(data);
                    str = str.Substring(6, str.Length - 8);
                    dynamic res = JsonConvert.DeserializeObject(str);

                    foreach (var bus in res.vehicles)
                    {
                        string id = "transloc" + agencyId + "-" + bus.id;
                        double lat = bus.position[0];
                        double lon = bus.position[1];


                        publisher.Tell(new Publisher.Position(lon, lat, id));
                    }

                    //how long should we wait before polling again?
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
            catch
            {
                
            }
        }

        //http://feeds.transloc.com/3/routes.jsonp?agencies=116

        private static void RunLadotBus(IActorRef publisher)
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
                    RunFetchLoopAsync(publisher, "http://ladotbus.com/Route/" + route.ID + "/Vehicles/");
                }
            }
        }

        //7e18baf233d24ad2b9d5ca539f8a7298

        //private static async void RunSL(IActorRef publisher)
        //{
        //    var url =
        //        "http://crossorigin.me/http://reseplanerare.vasttrafik.se/bin/query.exe/dny?&look_minx=10044745&look_maxx=12040389&look_miny=57025027&look_maxy=58406811&tpl=trains2json&look_productclass=1023&look_json=yes&performLocating=1&look_nv=zugposmode|2|get_ageofreport|yes|get_rtmsgstatus|yes|get_linenumber|yes|interval|10000|intervalstep|10000|&unique=1399449664000&";
        //    var c = new WebClient();
        //    while (true)
        //    {
        //        var data = await c.DownloadDataTaskAsync(new Uri(url));
        //        var str = Encoding.UTF8.GetString(data);
        //        dynamic res = JsonConvert.DeserializeObject(str);

        //        foreach (var bus in res.look.trains)
        //        {
        //            string id = bus.trainid;
        //            double lat = bus.y / 1000000d;
        //            double lon = bus.x / 1000000d;

        //            publisher.Tell(new Publisher.Position(lon, lat, id));
        //        }

        //        //how long should we wait before polling again?
        //        await Task.Delay(TimeSpan.FromSeconds(1));
        //    }
        //}

        private static async void RunGöteborg(IActorRef publisher)
        {
            var url ="http://reseplanerare.vasttrafik.se/bin/query.exe/dny?&look_minx=0&look_maxx=99999999&look_miny=0&look_maxy=99999999&tpl=trains2json&performLocating=1";
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

        //http://text90947.com/bustracking/wavetransit/m/businfo.jsp?refine=&iefix=82607
        private static async void RunFoo(IActorRef publisher)
        {
            var c = new WebClient();
            while (true)
            {
                try
                {
                    var data =
                        await
                            c.DownloadDataTaskAsync(
                                new Uri("http://text90947.com/bustracking/wavetransit/m/businfo.jsp?refine=&iefix=82607"));
                    var str = Encoding.UTF8.GetString(data);
                    var doc = new XmlDocument();
                    doc.LoadXml(str);
                    foreach (XmlElement bus in doc["buses"].ChildNodes)
                    {
                        var slat = bus["latitude"].InnerText;
                        var slon = bus["longitude"].InnerText;
                        var id = "foo" + bus["vehicleId"].InnerText;
                        var lat = double.Parse(slat, NumberFormatInfo.InvariantInfo);
                        var lon = double.Parse(slon, NumberFormatInfo.InvariantInfo);

                        publisher.Tell(new Publisher.Position(lon, lat, id));
                    }
                }
                catch
                {
                    
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private static async void RunLondon(IActorRef publisher)
        {
            

            var c = new WebClient();
            while (true)
            {
                var data = await c.DownloadDataTaskAsync(new Uri("http://traintimes.org.uk/map/london-buses/data/2"));
                var str = Encoding.UTF8.GetString(data);
                dynamic res = JsonConvert.DeserializeObject(str);
                //   Console.WriteLine("Downloaded {0}",url);

                foreach (var bus in res.trains)
                {
                    string id = bus.id;
                    double lat = bus.point[0];
                    double lon = bus.point[1];

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