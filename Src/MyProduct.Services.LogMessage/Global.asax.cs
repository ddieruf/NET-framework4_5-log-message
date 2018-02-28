using System;
using System.Web.Http;
using System.Data.Entity;

namespace MyProduct.Services.LogMessage{
	public class WebApiApplication : System.Web.HttpApplication{
		protected void Application_Start(){
			Contexts.LogMessage_Initializer.Initialize(new Contexts.LogMessage());

			//AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			//RouteConfig.RegisterRoutes(RouteTable.Routes);
		}
	}
}
