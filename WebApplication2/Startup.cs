using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Owin;
using WebApplication2;

[assembly: OwinStartup(typeof(Startup))]
namespace WebApplication2
{
   public class Startup
   {
      public void Configuration(IAppBuilder app)
      {
         app.MapSignalR();
      }
   }
}