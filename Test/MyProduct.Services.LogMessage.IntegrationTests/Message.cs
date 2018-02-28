using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using MyProduct.Services.LogMessage;
using Newtonsoft.Json;
using Xunit;
using Microsoft.Owin.Hosting;

/*
The Integrations Tests are focused on making sure the Controller class is working. To do this, a webserver is needed.
The TestFixture class emulates a web server as http://localhost, in memory, and runs requests on the Controller class.
Unlike the unit tests using a temp datastore, these tests will set up a real datastore.
*/

namespace MyProduct.Services.LogMessage.IntegrationTests
{
	[Collection("Add message collection")]
	public class AddMessageControllerTests : IDisposable
	{
		private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		internal class NewMessage : Models.DTOs.Message.Add
		{
			public NewMessage(string text) : base(text) { }
			public StringContent AsJson()
			{
				return new StringContent(JsonConvert.SerializeObject(this), Encoding.UTF8, "application/json");
			}
		}
		private readonly HttpClient _client;
		private readonly IDisposable _webApp;

		public AddMessageControllerTests()
		{
			string baseAddress = "http://localhost";
			_webApp = WebApp.Start<Startup>(url: baseAddress);
			_client = new HttpClient();
		}
		public void Dispose()
		{
			_webApp.Dispose();
		}

		[Fact(DisplayName = "POST a new error message successfully")]
		public async Task AddErrorMessage_Success()
		{
			// Arrange
			var msg = new NewMessage("An error message");

			// Act
			var response = await _client.PostAsync("http://localhost/error", msg.AsJson());

			// Assert
			Assert.IsType<HttpResponseMessage>(response);
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);
		}
		[Fact(DisplayName = "POST a new info message successfully")]
		public async Task AddInfoMessage_Success()
		{
			// Arrange
			var msg = new NewMessage("A new info message");

			// Act
			var response = await _client.PostAsync("/info", msg.AsJson());

			// Assert
			Assert.IsType<HttpResponseMessage>(response);
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);
		}
		[Fact(DisplayName = "POST a new message without any text")]
		public async Task AddMessage_MissingTextValue()
		{
			// Arrange
			var msg = new NewMessage("");

			// Act
			var response = await _client.PostAsync("/info", msg.AsJson());

			// Assert
			Assert.IsType<HttpResponseMessage>(response);
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}
	}
	[Collection("Get message collection")]
	public class GetMessageControllerTests
	{
		private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private readonly HttpClient _client;

		public GetMessageControllerTests()
		{
		}
		
		[Fact(DisplayName = "GET message without providing id")]
		public async Task GetMessage_MissingMessageId()
		{
			// Arrange

			// Act
			var response = await _client.GetAsync("/");

			// Assert
			Assert.IsType<HttpResponseMessage>(response);
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
		}
		[Fact(DisplayName = "GET message with an id that doesn't exist")]
		public async Task GetMessage_InvalidMessageId()
		{
			// Arranges
			var messageId = 0;

			// Act
			var response = await _client.GetAsync(string.Format("/{0}", messageId));

			// Assert
			Assert.IsType<HttpResponseMessage>(response);
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		}
		[Fact(DisplayName = "GET message with a character")]
		public async Task GetMessage_CharMessageId()
		{
			// Arrange
			string messageId = "abc";

			// Act
			var response = await _client.GetAsync(string.Format("/{0}", messageId));

			// Assert
			Assert.IsType<HttpResponseMessage>(response);
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}
		[Fact(DisplayName = "GET message success")]
		public async Task GetMessage_Success()
		{
			// Arrange
			await new AddMessageControllerTests().AddInfoMessage_Success();
			var messageId = 1;

			// Act
			var response = await _client.GetAsync(string.Format("/{0}", messageId));

			// Assert
			Assert.IsType<HttpResponseMessage>(response);
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var message = JsonConvert.DeserializeObject<Models.DTOs.Message.Get>(await response.Content.ReadAsStringAsync());
			Assert.IsType<Models.DTOs.Message.Get>(message);
			Assert.Equal(message.Id, messageId);
		}
	}
	[Collection("Search message collection")]
	public class SearchMessageControllerTests
	{
		private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		internal class NewSearch : Models.DTOs.Message.Search
		{
			public NewSearch(string text, long? messageTypeId = null, long? startDate = null, long? endDate = null) : base(text, messageTypeId, startDate, endDate) { }
			public StringContent AsJson()
			{
				return new StringContent(JsonConvert.SerializeObject(this), Encoding.UTF8, "application/json");
			}
		}
		private readonly HttpClient _client;

		public SearchMessageControllerTests()
		{
		}
		
		[Fact(DisplayName = "POST search with no params and get error")]
		public async Task SearchMessage_NoParams()
		{
			// Arrange
			await new AddMessageControllerTests().AddInfoMessage_Success();
			var search = new NewSearch(null);

			// Act
			var response = await _client.PostAsync("/search", search.AsJson());

			// Assert
			Assert.IsType<HttpResponseMessage>(response);
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}
		[Fact(DisplayName = "POST search with text and get no results")]
		public async Task SearchText_NoResults()
		{
			// Arrange
			await new AddMessageControllerTests().AddInfoMessage_Success();
			var searchText = "asdf";
			var search = new NewSearch(searchText);

			// Act
			var response = await _client.PostAsync("/search", search.AsJson());

			// Assert
			Assert.IsType<HttpResponseMessage>(response);
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var messages = JsonConvert.DeserializeObject<List<Models.DTOs.Message.Search>>(await response.Content.ReadAsStringAsync());
			Assert.IsType<List<Models.DTOs.Message.Search>>(messages);
			Assert.Empty(messages);
		}
		[Fact(DisplayName = "POST search with text and get results then use message in results to do a get")]
		public async Task SearchTextAndGetMessage_Success()
		{
			// Arrange
			await new AddMessageControllerTests().AddInfoMessage_Success(); //load at least one message
			var searchText = "info";
			var search = new NewSearch(searchText);

			// Act
			var response = await _client.PostAsync("/search", search.AsJson());

			// Assert
			Assert.IsType<HttpResponseMessage>(response);
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var messages = JsonConvert.DeserializeObject<List<Models.DTOs.Message.Get>>(await response.Content.ReadAsStringAsync());
			Assert.IsType<List<Models.DTOs.Message.Get>>(messages);
			Assert.NotEmpty(messages);
			Assert.True(messages.Exists(q => q.Text.Contains(searchText)));
		}
		[Fact(DisplayName = "POST search with typeId and get results")]
		public async Task SearchMessageId_Success()
		{
			// Arrange
			await new AddMessageControllerTests().AddInfoMessage_Success(); //load at least one message
			var search = new NewSearch(null, (long)Constants.MessageType.Info);

			// Act
			var response = await _client.PostAsync("/search", search.AsJson());

			// Assert
			Assert.IsType<HttpResponseMessage>(response);
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var messages = JsonConvert.DeserializeObject<List<Models.DTOs.Message.Get>>(await response.Content.ReadAsStringAsync());
			Assert.IsType<List<Models.DTOs.Message.Get>>(messages);
			Assert.NotEmpty(messages);
			Assert.True(messages.Exists(q => q.MessageType.Id == (long)Constants.MessageType.Info));
		}
		[Fact(DisplayName = "POST search with text and typeId and get results")]
		public async Task SearchTextAndMessageTypeId_Success()
		{
			// Arrange
			await new AddMessageControllerTests().AddErrorMessage_Success(); //load at least one message
			var search = new NewSearch("error", (long)Constants.MessageType.Error);

			// Act
			var response = await _client.PostAsync("/search", search.AsJson());

			// Assert
			Assert.IsType<HttpResponseMessage>(response);
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var messages = JsonConvert.DeserializeObject<List<Models.DTOs.Message.Get>>(await response.Content.ReadAsStringAsync());
			Assert.IsType<List<Models.DTOs.Message.Get>>(messages);
			Assert.NotEmpty(messages);
			Assert.True(messages.Exists(q => q.MessageType.Id == (long)Constants.MessageType.Error && q.Text.Contains(search.Text)));
		}
		[Fact(DisplayName = "POST search with startDate and get results")]
		public async Task SearchStartDate_Success()
		{
			// Arrange
			long oneMinuteAgo = now();
			await new AddMessageControllerTests().AddInfoMessage_Success(); //load at least one message
			var search = new NewSearch(null, null, oneMinuteAgo, null);

			// Act
			var response = await _client.PostAsync("/search", search.AsJson());

			// Assert
			Assert.IsType<HttpResponseMessage>(response);
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var messages = JsonConvert.DeserializeObject<List<Models.DTOs.Message.Get>>(await response.Content.ReadAsStringAsync());
			Assert.IsType<List<Models.DTOs.Message.Get>>(messages);
			Assert.NotEmpty(messages);
		}
		[Fact(DisplayName = "POST search with startDate and endDate and get results")]
		public async Task SearchStartAndEndDate_Success()
		{
			// Arrange
			long oneMinuteAgo = now();
			await new AddMessageControllerTests().AddInfoMessage_Success(); //load at least one message
			long here = now();
			var search = new NewSearch(null, null, oneMinuteAgo, here);

			// Act
			var response = await _client.PostAsync("/search", search.AsJson());

			// Assert
			Assert.IsType<HttpResponseMessage>(response);
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var messages = JsonConvert.DeserializeObject<List<Models.DTOs.Message.Get>>(await response.Content.ReadAsStringAsync());
			Assert.IsType<List<Models.DTOs.Message.Get>>(messages);
			Assert.NotEmpty(messages);
		}

		private long now()
		{
			return (long)DateTimeOffset.Now.ToUniversalTime().Subtract(
															new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
															).TotalMilliseconds;
		}
	}
}