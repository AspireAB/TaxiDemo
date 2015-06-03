using Akka.Actor;
using Serilog;
using TaxiShared;

namespace TaxiBackend
{
    public class TransitBackendService
    {
        private readonly ActorSystem _system;

        public TransitBackendService()
        {
            var logger = new LoggerConfiguration().WriteTo.ColoredConsole().MinimumLevel.Debug().CreateLogger();
            //var logger = new LoggerConfiguration()
            //    .WriteTo.Elasticsearch()
            //    .MinimumLevel.Debug()
            //    .CreateLogger();
            Log.Logger = logger;

            _system = ActorSystem.Create("TaxiBackend");
            var presenter = _system.ActorOf(Props.Create(() => new PresenterActor()), "presenter");
            var publisher = _system.ActorOf(Props.Create(() => new PublisherActor(presenter)), "publisher");

            //     RunLondon(publisher);

            //RunFoo(publisher);

            VästtrafikProvider.Run(publisher);

            LadotProvider.Run(publisher);
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