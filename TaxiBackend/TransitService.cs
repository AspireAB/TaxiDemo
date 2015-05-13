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
    public class TransitBackendService
    {
        private readonly ActorSystem _system;

        public TransitBackendService()
        {
            _system = ActorSystem.Create("TaxiBackend");
            var publisher = _system.ActorOf(Props.Create(() => new PublisherActor()), "publisher");

            //RunLondon(publisher);

            //RunFoo(publisher);

            VästtrafikProvider.Run(publisher);

     //       LadotProvider.Run(publisher);

        }

        //private static void RunBar(IActorRef publisher)
        //{
        //    for (var i = 0; i < 500; i++)
        //    {
        //        RunBar(publisher, i);
        //    }
        //}

        //private static async void RunBar(IActorRef publisher, int agencyId)
        //{
        //    await Task.Yield();
        //    try
        //    {
        //        var url = "http://feeds.transloc.com/3/vehicle_statuses.jsonp?agencies=" + agencyId;
        //        var c = new WebClient();
        //        while (true)
        //        {
        //            var data = await c.DownloadDataTaskAsync(new Uri(url));
        //            var str = Encoding.UTF8.GetString(data);
        //            str = str.Substring(6, str.Length - 8);
        //            dynamic res = JsonConvert.DeserializeObject(str);

        //            foreach (var bus in res.vehicles)
        //            {
        //                string id = "transloc" + agencyId + "-" + bus.id;
        //                double lat = bus.position[0];
        //                double lon = bus.position[1];


        //                publisher.Tell(new Publisher.Position(lon, lat, id, "TransLoc" + agencyId));
        //            }

        //            //how long should we wait before polling again?
        //            await Task.Delay(TimeSpan.FromSeconds(1));
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}





        //private static async void RunFoo(IActorRef publisher)
        //{
        //    var c = new WebClient();
        //    while (true)
        //    {
        //        try
        //        {
        //            var data =
        //                await
        //                    c.DownloadDataTaskAsync(
        //                        new Uri(
        //                            "http://text90947.com/bustracking/wavetransit/m/businfo.jsp?refine=&iefix=82607"));
        //            var str = Encoding.UTF8.GetString(data);
        //            var doc = new XmlDocument();
        //            doc.LoadXml(str);
        //            foreach (XmlElement bus in doc["buses"].ChildNodes)
        //            {
        //                var slat = bus["latitude"].InnerText;
        //                var slon = bus["longitude"].InnerText;
        //                var id = "foo" + bus["vehicleId"].InnerText;
        //                var lat = double.Parse(slat, NumberFormatInfo.InvariantInfo);
        //                var lon = double.Parse(slon, NumberFormatInfo.InvariantInfo);

        //                publisher.Tell(new Publisher.Position(lon, lat, id, "WaveTransit"));
        //            }
        //        }
        //        catch
        //        {
        //        }

        //        await Task.Delay(TimeSpan.FromSeconds(1));
        //    }
        //}

        //private static async void RunLondon(IActorRef publisher)
        //{
        //    var c = new WebClient();
        //    while (true)
        //    {
        //        var data =
        //            await c.DownloadDataTaskAsync(new Uri("http://traintimes.org.uk/map/london-buses/data/2"));
        //        var str = Encoding.UTF8.GetString(data);
        //        dynamic res = JsonConvert.DeserializeObject(str);
        //        //   Console.WriteLine("Downloaded {0}",url);

        //        foreach (var bus in res.trains)
        //        {
        //            string id = bus.id;
        //            double lat = bus.point[0];
        //            double lon = bus.point[1];

        //            publisher.Tell(new Publisher.Position(lon, lat, id, "London"));
        //        }

        //        //how long should we wait before polling again?
        //        await Task.Delay(TimeSpan.FromSeconds(1));
        //    }
        //}

        public void Start()
        {

        }

        public void Stop()
        {
            _system.Shutdown();
        }
    }
}
