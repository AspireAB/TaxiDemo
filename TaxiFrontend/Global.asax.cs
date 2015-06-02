using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TaxiFrontend.Actors;
using TaxiShared;

namespace TaxiFrontend
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			RegisterActors();
			RegisterRoutes();
			RegisterBundles();
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

			RouteTable.Routes.MapRoute(
				 name: "Default",
				 url: "{controller}/{action}/{id}",
				 defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}