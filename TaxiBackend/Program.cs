using System;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Akka.Actor;
using Newtonsoft.Json;
using TaxiShared;
using Topshelf;

namespace TaxiBackend
{
    public class TransitBackendService
    {
        private IActorRef publisher;
        private ActorSystem system;

        public TransitBackendService()
        {
            system = ActorSystem.Create("TaxiBackend");
            publisher = system.ActorOf(Props.Create(() => new PublisherActor()), "publisher");

            RunLondon(publisher);

            RunFoo(publisher);

            RunGöteborg(publisher);

            RunLadotBus(publisher);

        }

        private static void RunBar(IActorRef publisher)
        {
            for (var i = 0; i < 500; i++)
            {
                RunBar(publisher, i);
            }
        }

        private static async void RunBar(IActorRef publisher, int agencyId)
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


                        publisher.Tell(new Publisher.Position(lon, lat, id, "TransLoc" + agencyId));
                    }

                    //how long should we wait before polling again?
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
            catch
            {
            }
        }

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
                    RunFetchLoopAsync(publisher, "http://ladotbus.com/Route/" + route.ID + "/Vehicles/", "LAdot");
                }
            }
        }

        private static async void RunGöteborg(IActorRef publisher)
        {
            var url =
                "http://reseplanerare.vasttrafik.se/bin/query.exe/dny?&look_minx=0&look_maxx=99999999&look_miny=0&look_maxy=99999999&tpl=trains2json&performLocating=1";
            var c = new WebClient();
            while (true)
            {
                var data = await c.DownloadDataTaskAsync(new Uri(url));
                var str = Encoding.UTF8.GetString(data);
                dynamic res = JsonConvert.DeserializeObject(str);

                foreach (var bus in res.look.trains)
                {
                    string id = bus.trainid;
                    double lat = bus.y/1000000d;
                    double lon = bus.x/1000000d;

                    publisher.Tell(new Publisher.Position(lon, lat, id, "Västtrafik"));
                }

                //how long should we wait before polling again?
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

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
                                new Uri(
                                    "http://text90947.com/bustracking/wavetransit/m/businfo.jsp?refine=&iefix=82607"));
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

                        publisher.Tell(new Publisher.Position(lon, lat, id, "WaveTransit"));
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
                var data =
                    await c.DownloadDataTaskAsync(new Uri("http://traintimes.org.uk/map/london-buses/data/2"));
                var str = Encoding.UTF8.GetString(data);
                dynamic res = JsonConvert.DeserializeObject(str);
                //   Console.WriteLine("Downloaded {0}",url);

                foreach (var bus in res.trains)
                {
                    string id = bus.id;
                    double lat = bus.point[0];
                    double lon = bus.point[1];

                    publisher.Tell(new Publisher.Position(lon, lat, id, "London"));
                }

                //how long should we wait before polling again?
                await Task.Delay(TimeSpan.FromSeconds(1));
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

                        publisher.Tell(new Publisher.Position(lon, lat, id, source));
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

        public void Start()
        {
           
        }

        public void Stop()
        {
            system.Shutdown();
        }
    }

    public class Program
    {
        public static void Main()
        {
            HostFactory.Run(x => //1
            {
                x.Service<TransitBackendService>(s => //2
                {
                    s.ConstructUsing(name => new TransitBackendService()); //3
                    s.WhenStarted(tc => tc.Start()); //4
                    s.WhenStopped(tc => tc.Stop()); //5
                });
                x.RunAsLocalSystem(); //6

                x.SetDescription("TransitService"); //7
                x.SetDisplayName("TransitService"); //8
                x.SetServiceName("TransitService"); //9
            }); //10
        }
    }
}