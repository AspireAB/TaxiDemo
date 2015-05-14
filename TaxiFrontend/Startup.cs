using Microsoft.Owin;
using Owin;
using TaxiFrontend;

[assembly: OwinStartup(typeof(Startup))]
namespace TaxiFrontend
{
   public class Startup
   {
      public void Configuration(IAppBuilder app)
      {
         app.MapSignalR();
      }
   }
}