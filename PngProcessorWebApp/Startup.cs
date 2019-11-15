using Hangfire;
using Microsoft.Owin;
using Owin;
using System.Web.Http;

[assembly: OwinStartup(typeof(PngProcessorWebApp.Startup))]

namespace PngProcessorWebApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            Hangfire.GlobalConfiguration.Configuration.UseSqlServerStorage("default");
            app.UseHangfireDashboard();
            app.UseHangfireServer();
            app.UseWebApi(config);
        }
    }
}
