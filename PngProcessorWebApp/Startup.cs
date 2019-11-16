using Autofac;
using Autofac.Integration.WebApi;
using Hangfire;
using Microsoft.Owin;
using Owin;
using PngProcessorWebApp.Services;
using System.Reflection;
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
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<ImageProcessingService>().As<IImageProcessingService>();
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
