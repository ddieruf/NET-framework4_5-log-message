using System;
using Microsoft.Owin;
using MyProduct.Services.LogMessage.IntegrationTests;
using System.Web.Http;
using Newtonsoft.Json.Serialization;
using Owin;
using System.Linq;

[assembly: OwinStartup(typeof(Startup))]
namespace MyProduct.Services.LogMessage.IntegrationTests
{
	public class Startup
	{
		// This code configures Web API. The Startup class is specified as a type
		// parameter in the WebApp.Start method.
		public void Configuration(IAppBuilder appBuilder)
		{
			// Configure Web API for self-host. 
			HttpConfiguration config = new HttpConfiguration();
			config.MapHttpAttributeRoutes();
			appBuilder.UseWebApi(config);
			
		}
	}
}
