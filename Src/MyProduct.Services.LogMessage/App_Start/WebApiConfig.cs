using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace MyProduct.Services.LogMessage
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services

			// Use camel case for JSON data.
			config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

			// Web API routes
			config.MapHttpAttributeRoutes();

			/*config.Routes.MapHttpRoute(
					name: "GetMessage",
					routeTemplate: "{id}",
					defaults: new { id = RouteParameter.Optional } //leave this optional so we can catch the error and return correct code
			);

			config.Routes.MapHttpRoute(
					name: "SearchMessage",
					routeTemplate: "search",
					defaults: new { controller="Message" }
			);

			config.Routes.MapHttpRoute(
					name: "AddMessage",
					routeTemplate: "",
					defaults: new { message = RouteParameter.Optional } //leave this optional so we can catch the error and return correct code
			);*/
		}
	}
}
