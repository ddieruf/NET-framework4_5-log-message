using System;
using Xunit;
using MyProduct.Services.LogMessage;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace MyProduct.Services.LogMessage.SmokeTests
{
	public class HttpClientFixture : IDisposable
	{
		public HttpClient client;

		public HttpClientFixture()
		{
			client = new HttpClient();

			string _appUrl = null;

			try {
				//_appUrl = Environment.GetEnvironmentVariable("APP_URL");
				_appUrl = "https://logmessage-stage.apps.cloudyaws.io";
				if (string.IsNullOrEmpty(_appUrl)) {
					Console.WriteLine("Error: Smoke test environment variable not set - APP_URL");
					throw new NullReferenceException("APP_URL");
				}
			} catch (Exception ex) {
				Console.WriteLine("Error: Smoke test initialize - " + ex.Message);
				throw;
			}

			client.BaseAddress = new Uri(_appUrl);
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}
		public void Dispose()
		{
		}
	}
	[Collection("Add message collection")]
	public class AddMessage: IClassFixture<HttpClientFixture>
	{
		private HttpClient _client;
		public AddMessage(HttpClientFixture fixture)
		{
			_client = fixture.client;
		}
		internal class NewMessage : Models.DTOs.Message.Add
		{
			public NewMessage(string text) : base(text) { }
			public StringContent AsJson()
			{
				return new StringContent(JsonConvert.SerializeObject(this), Encoding.UTF8, "application/json");
			}
		}
		
		[Fact(DisplayName = "POST a new error message successfully")]
		public async Task AddErrorMessage_Success()
		{
			// Arrange
			var msg = new NewMessage("Smoke testing add error message");

			// Act
			var response = await _client.PostAsync("/error", msg.AsJson());

			// Assert
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);
		}
		[Fact(DisplayName = "POST a new info message successfully")]
		public async Task AddInfoMessage_Success()
		{
			// Arrange
			var msg = new NewMessage("Smoke testing add info message");

			// Act
			var response = await _client.PostAsync("/info", msg.AsJson());

			// Assert
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);
		}
		[Fact(DisplayName = "POST a new info message without any text")]
		public async Task AddInfoMessage_MissingTextValue()
		{
			// Arrange
			var msg = new NewMessage("");
		
			// Act
			var response = await _client.PostAsync("/info", msg.AsJson());

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}
		[Fact(DisplayName = "POST a new error message without any text")]
		public async Task AddErrorMessage_MissingTextValue_Error()
		{
			// Arrange
			var msg = new NewMessage("");
			var req = new StringContent(JsonConvert.SerializeObject(msg), Encoding.UTF8, "application/json");

			// Act
			var response = await _client.PostAsync("/error", msg.AsJson());

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}
	}
	[Collection("Get message collection")]
	public class GetMessage : IClassFixture<HttpClientFixture>
	{
		private readonly HttpClient _client;
		private readonly HttpClientFixture _fixture;

		public GetMessage(HttpClientFixture fixture)
		{
			_fixture = fixture;
			_client = _fixture.client;
		}

		[Fact(DisplayName = "GET message without providing id")]
		public async Task GetMessage_MissingMessageId()
		{
			// Arrange

			// Act
			var response = await _client.GetAsync(string.Format("/{0}", ""));

			// Assert
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
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}
		[Fact(DisplayName = "GET message successfully")]
		public async Task GetMessage_Success()
		{
			// Arrange
			await new AddMessage(_fixture).AddInfoMessage_Success();
			var messageId = 1;

			// Act
			var response = await _client.GetAsync(string.Format("/{0}", messageId));

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var message = JsonConvert.DeserializeObject<Models.DTOs.Message.Get>(await response.Content.ReadAsStringAsync());
			Assert.IsType<Models.DTOs.Message.Get>(message);
		}
	}
	[Collection("Search message collection")]
	public class SearchMessage : IClassFixture<HttpClientFixture>
	{
		private readonly HttpClient _client;
		private readonly HttpClientFixture _fixture;

		public SearchMessage(HttpClientFixture fixture)
		{
			_fixture = fixture;
			_client = _fixture.client;
		}
		internal class NewSearch : Models.DTOs.Message.Search
		{
			public NewSearch(string text = null, long? messageTypeId = null, long? startDate = null, long? endDate = null) : base(text, messageTypeId, startDate, endDate) { }
			public StringContent AsJson()
			{
				return new StringContent(JsonConvert.SerializeObject(this), Encoding.UTF8, "application/json");
			}
		}
		[Fact(DisplayName = "POST search with no params and get error")]
		public async Task SearchMessage_NoParams()
		{
			// Arrange
			await new AddMessage(_fixture).AddInfoMessage_Success();
			var search = new NewSearch(null);

			// Act
			var response = await _client.PostAsync("/search", search.AsJson());

			// Assert
			Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
		}
		[Fact(DisplayName = "POST search with text and get no results")]
		public async Task SearchText_NoResults()
		{
			// Arrange
			await new AddMessage(_fixture).AddInfoMessage_Success();
			var searchText = "asdf112233";
			var search = new NewSearch(searchText, null, null);

			// Act
			var response = await _client.PostAsync("/search", search.AsJson());

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var messages = JsonConvert.DeserializeObject<List<Models.DTOs.Message.Search>>(await response.Content.ReadAsStringAsync());
			Assert.IsType<List<Models.DTOs.Message.Search>>(messages);
			Assert.Empty(messages);
		}
		[Fact(DisplayName = "POST search with text and get results")]
		public async Task SearchText_Success()
		{
			// Arrange
			await new AddMessage(_fixture).AddInfoMessage_Success();
			var searchText = "info";
			var search = new NewSearch(searchText);

			// Act
			var response = await _client.PostAsync("/search", search.AsJson());

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var messages = JsonConvert.DeserializeObject<List<Models.DTOs.Message.Search>>(await response.Content.ReadAsStringAsync());
			Assert.IsType<List<Models.DTOs.Message.Search>>(messages);
			Assert.NotEmpty(messages);
			Assert.True(messages.Exists(q => q.Text.Contains(searchText)));
		}
		[Fact(DisplayName = "POST search with typeId and get results")]
		public async Task SearchMessageId_Success()
		{
			// Arrange
			await new AddMessage(_fixture).AddInfoMessage_Success();
			var search = new NewSearch(null, (long)Constants.MessageType.Error);

			// Act
			var response = await _client.PostAsync("/search", search.AsJson());

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var messages = JsonConvert.DeserializeObject<List<Models.DTOs.Message.Search>>(await response.Content.ReadAsStringAsync());
			Assert.IsType<List<Models.DTOs.Message.Search>>(messages);
			Assert.NotEmpty(messages);
			Assert.True(messages.Exists(q => q.MessageTypeId == (long)Constants.MessageType.Error));
		}
		[Fact(DisplayName = "POST search with text and typeId and get results")]
		public async Task SearchTextAndMessageId_Success()
		{
			// Arrange
			await new AddMessage(_fixture).AddInfoMessage_Success();
			var search = new NewSearch("error", (long)Constants.MessageType.Error);

			// Act
			var response = await _client.PostAsync("/search", search.AsJson());

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var messages = JsonConvert.DeserializeObject<List<Models.DTOs.Message.Search>>(await response.Content.ReadAsStringAsync());
			Assert.IsType<List<Models.DTOs.Message.Search>>(messages);
			Assert.NotEmpty(messages);
			Assert.True(messages.Exists(q => q.MessageTypeId == (long)Constants.MessageType.Error && q.Text.Contains(search.Text)));
		}
		[Fact(DisplayName = "POST search with insertdate and get results")]
		public async Task SearchInsertDate_Success()
		{
			// Arrange
			long oneMinuteAgo = now();
			await new AddMessage(_fixture).AddInfoMessage_Success();
			var search = new NewSearch(null, null, oneMinuteAgo);

			// Act
			var response = await _client.PostAsync("/search", search.AsJson());

			// Assert
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var messages = JsonConvert.DeserializeObject<List<Models.DTOs.Message.Search>>(await response.Content.ReadAsStringAsync());
			Assert.IsType<List<Models.DTOs.Message.Search>>(messages);
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