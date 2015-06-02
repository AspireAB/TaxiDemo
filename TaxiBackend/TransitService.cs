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
            var presenter = _system.ActorOf(Props.Create(() => new PresenterActor()), "presenter");
            var publisher = _system.ActorOf(Props.Create(() => new PublisherActor(presenter)), "publisher");

            //RunLondon(publisher);

            //RunFoo(publisher);

            VästtrafikProvider.Run(publisher);

     //       LadotProvider.Run(publisher);

        }

        public void Start()
        {

        }

        public void Stop()
        {
            _system.Shutdown();
        }
    }
}
