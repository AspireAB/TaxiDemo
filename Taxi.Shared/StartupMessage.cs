using Akka.Actor;

namespace TaxiBackend
{
   public class StartupMessage
   {
      public StartupMessage(IActorRef presenter)
      {
         Presenter = presenter;
      }

      public IActorRef Presenter { get; set; }
   }
}