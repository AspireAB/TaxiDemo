using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Serilog;
using TaxiFrontend.Actors;
using TaxiShared;

namespace TaxiFrontend
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            RegisterLoggers();
            RegisterActors();
            RegisterRoutes();
            RegisterBundles();
        }

        private void RegisterLoggers()
        {
            var logger = new LoggerConfiguration().WriteTo.ColoredConsole().MinimumLevel.Debug().CreateLogger();
            //var logger = new LoggerConfiguration()
            //    .WriteTo.Elasticsearch()
            //    .MinimumLevel.Debug()
            //    .CreateLogger();

            Log.Logger = logger;
        }

        private static void RegisterActors()
        {
            FrontActorSystem.Presenter.Tell(new Presenter.Initialize(FrontActorSystem.SignalRActor));
        }

        private static void RegisterBundles()
        {
            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/js")
                .IncludeDirectory("~/Scripts", "*.js")
                .IncludeDirectory("~/App", "*.js"));
        }

        public static void RegisterRoutes()
        {
            RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            RouteTable.Routes.MapRoute("Default", "{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional}
                );
        }
    }
}