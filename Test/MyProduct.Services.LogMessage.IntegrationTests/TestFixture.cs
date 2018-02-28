using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using Microsoft.Owin.Testing;

namespace MyProduct.Services.LogMessage.IntegrationTests
{
	/// <summary>
	/// A test fixture which hosts the target project (project we wish to test) in an in-memory server.
	/// </summary>
	/// <typeparam name="TStartup">Target project's startup type</typeparam>
	public class TestFixture<TStartup> : IDisposable
	{
		private readonly TestServer _server;

		public TestFixture() : this(Path.Combine("."))
		{
		}

		protected TestFixture(string relativeTargetProjectParentDir)
		{
			_server = TestServer.Create<TStartup>();
			Client = _server.HttpClient;
			Client.BaseAddress = new Uri("http://localhost");
		}
		public HttpClient Client { get; }
		public void Dispose()
		{
			Client.Dispose();
			_server.Dispose();
		}
	}
}