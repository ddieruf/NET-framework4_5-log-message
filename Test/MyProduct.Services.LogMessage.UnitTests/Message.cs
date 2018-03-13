using System;
using Xunit;
using MyProduct.Services.LogMessage.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

/*
The Unit Tests are focused on making sure the Interface class is working. This will in turn also test the data store.
The program and startup classes are not used.
 */
namespace MyProduct.Services.LogMessage.UnitTests
{
	[Collection("Add message collection")]
	public class AddMessage : IDisposable{
		private readonly Interfaces.Message message;
		private readonly Interfaces.MessageType messageType;

		public AddMessage(){
			Contexts.LogMessage context = new Contexts.LogMessage();
			Contexts.LogMessage_Initializer.Initialize(context);

			messageType = new Interfaces.MessageType(context);
			message = new Interfaces.Message();
		}
		public void Dispose(){
			message.Dispose();
		}

		private Models.DTOs.Message.Add TestInfoMessage
		{
			get { return new Models.DTOs.Message.Add("This is some info"); }
		}
		private Models.DTOs.Message.Add TestErrorMessage
		{
			get { return new Models.DTOs.Message.Add("This is an error"); }
		}
		
		[Fact(DisplayName = "Info message saved successfully")]
		public void AddInfoMessage_Success()
		{
			// Arrange
			
			// Act
			var msgId = message.Add(TestInfoMessage, (long)Constants.MessageType.Info);

			// Assert
			Assert.IsType<long>(msgId);
			Assert.True(msgId > 0);
		}
		[Fact(DisplayName = "Error message saved successfully")]
		public void AddErrorMessage_Success()
		{
			// Arrange

			// Act
			var msgId = message.Add(TestErrorMessage, (long)Constants.MessageType.Error);

			// Assert
			Assert.IsType<long>(msgId);
			Assert.True(msgId > 0);
		}
		[Fact(DisplayName = "Message object missing and error returned")]
		public void NullMessageObject_ErrorReturned()
		{
			// Arrange

			// Act
			var exception = Record.Exception(() => message.Add(null, (long)Constants.MessageType.Error));

			// Assert
			Assert.NotNull(exception);
			Assert.IsType<NullReferenceException>(exception);
		}
		[Fact(DisplayName = "Message text missing and error returned")]
		public void BlankMessageText_ErrorReturned()
		{
			// Arrange
			var newMessage = new Models.DTOs.Message.Add("");

			// Act
			var exception = Record.Exception(() => message.Add(newMessage, (long)Constants.MessageType.Error));

			// Assert
			Assert.NotNull(exception);
			Assert.IsType<ArgumentNullException>(exception);
		}
	}
	[Collection("Get message collection")]
	public class GetMessage : IDisposable
	{
		private readonly Interfaces.Message message;

		public GetMessage()
		{
			message = new Interfaces.Message();
		}
		public void Dispose()
		{
			message.Dispose();
		}

		private Models.DTOs.Message.Add TestInfoMessage {
			get { return new Models.DTOs.Message.Add("This is some info"); }
		}
		private Models.DTOs.Message.Add TestErrorMessage {
			get { return new Models.DTOs.Message.Add("This is an error"); }
		}
		
		[Fact(DisplayName = "Get message successfully")]
		public async Task GetMessage_Success()
		{
			// Arrange
			var msgId = message.Add(TestInfoMessage, (long)Constants.MessageType.Info);

			// Act
			var msg = await message.Get(msgId);

			// Assert
			Assert.NotNull(msg);
			Assert.IsType<Models.DTOs.Message.Get>(msg);
			Assert.Equal(msg.Text, TestInfoMessage.Text);
			Assert.Equal(msg.Id, msgId);
			Assert.IsType<Models.DTOs.MessageType.Get>(msg.MessageType);
			Assert.Equal(msg.MessageType.Id, (long)Constants.MessageType.Info);
			Assert.Equal(msg.MessageType.Name, Enum.GetName(typeof(Constants.MessageType), (long)Constants.MessageType.Info));
		}
		[Fact(DisplayName = "Get message with invalid id")]
		public async Task GetMessage_InvalidId()
		{
			// Arrange
			var msgId = message.Add(TestInfoMessage, (long)Constants.MessageType.Info);

			// Act
			var msg = await message.Get(0);

			// Assert
			Assert.Null(msg);
		}
	}
	[Collection("Search message collection")]
	public class SearchMessage : IDisposable
	{
		private readonly Interfaces.Message message;
		private readonly Interfaces.MessageType messageType;

		public SearchMessage()
		{
			Contexts.LogMessage context = new Contexts.LogMessage();
			Contexts.LogMessage_Initializer.Initialize(context);

			messageType = new Interfaces.MessageType(context);
			message = new Interfaces.Message();

			context.Messages.RemoveRange(context.Messages.Select(q => q));
			context.SaveChanges();
		}
		public void Dispose()
		{
			message.Dispose();
		}

		private Models.DTOs.Message.Add TestInfoMessage {
			get { return new Models.DTOs.Message.Add("This is some info"); }
		}
		private Models.DTOs.Message.Add TestErrorMessage {
			get { return new Models.DTOs.Message.Add("This is an error"); }
		}
		
		[Fact(DisplayName = "Search messages by Constants.MessageType.Info and get result")]
		public void SearchMessageByType_Success()
		{
			// Arrange
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestErrorMessage, (long)Constants.MessageType.Error);
			message.Add(TestErrorMessage, (long)Constants.MessageType.Error);

			// Act
			Models.DTOs.Message.Search search = new Models.DTOs.Message.Search(null, (long)Constants.MessageType.Info);
			var result = message.Search(search);

			// Assert
			Assert.NotNull(result);
			Assert.IsAssignableFrom<IEnumerable<Models.DTOs.Message.Get>>(result);
			Assert.Equal(3, result.Count());
		}
		[Fact(DisplayName = "Search messages by partial text and get result")]
		public void SearchMessageByPartialText_Success()
		{
			// Arrange
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestErrorMessage, (long)Constants.MessageType.Error);
			message.Add(TestErrorMessage, (long)Constants.MessageType.Error);

			// Act
			Models.DTOs.Message.Search search = new Models.DTOs.Message.Search("this");
			var result = message.Search(search);

			// Assert
			Assert.NotNull(result);
			Assert.IsAssignableFrom<IEnumerable<Models.DTOs.Message.Get>>(result);
			Assert.Equal(5, result.Count());
		}
		[Fact(DisplayName = "Search messages by exact text and get result")]
		public void SearchMessageByExactText_Success()
		{
			// Arrange
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestErrorMessage, (long)Constants.MessageType.Error);
			message.Add(TestErrorMessage, (long)Constants.MessageType.Error);

			// Act
			Models.DTOs.Message.Search search = new Models.DTOs.Message.Search(TestErrorMessage.Text);
			var result = message.Search(search);

			// Assert
			Assert.NotNull(result);
			Assert.IsAssignableFrom<IEnumerable<Models.DTOs.Message.Get>>(result);
			Assert.Equal(2, result.Count());
		}
		[Fact(DisplayName = "Search messages by partial text and get no result")]
		public void SearchMessageByPartialText_Empty()
		{
			// Arrange
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestErrorMessage, (long)Constants.MessageType.Error);
			message.Add(TestErrorMessage, (long)Constants.MessageType.Error);

			// Act
			Models.DTOs.Message.Search search = new Models.DTOs.Message.Search("no in there");
			var result = message.Search(search);

			// Assert
			Assert.NotNull(result);
			Assert.IsAssignableFrom<IEnumerable<Models.DTOs.Message.Get>>(result);
			Assert.Equal(0, result.Count());
		}
		[Fact(DisplayName = "Search messages with null object and get error")]
		public void SearchMessageNullCriteria_Error()
		{
			// Arrange
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestErrorMessage, (long)Constants.MessageType.Error);
			message.Add(TestErrorMessage, (long)Constants.MessageType.Error);

			// Act
			var exception = Record.Exception(() => message.Search(null));

			// Assert
			Assert.NotNull(exception);
			Assert.IsType<ArgumentNullException>(exception);
		}
		[Fact(DisplayName = "Search messages with no criteria and get error")]
		public void SearchMessageNoCriteria_Error()
		{
			// Arrange
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestErrorMessage, (long)Constants.MessageType.Error);
			message.Add(TestErrorMessage, (long)Constants.MessageType.Error);

			// Act
			Models.DTOs.Message.Search search = new Models.DTOs.Message.Search("", null, null, null);
			var exception = Record.Exception(() => message.Search(search));

			// Assert
			Assert.NotNull(exception);
			Assert.IsType<ArgumentOutOfRangeException>(exception);
		}
		[Fact(DisplayName = "Search messages by startDate and get result")]
		public void SearchMessageStartDate_Success()
		{
			// Arrange
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);

			System.Threading.Thread.Sleep(200);
			long between = now();
			System.Threading.Thread.Sleep(200);

			message.Add(TestErrorMessage, (long)Constants.MessageType.Error);
			message.Add(TestErrorMessage, (long)Constants.MessageType.Error);

			// Act
			Models.DTOs.Message.Search search = new Models.DTOs.Message.Search(null, null, between);
			var result = message.Search(search);

			// Assert
			Assert.NotNull(result);
			Assert.IsAssignableFrom<IEnumerable<Models.DTOs.Message.Get>>(result);
			Assert.Equal(2, result.Count());
		}
		[Fact(DisplayName = "Search messages by endDate and get result")]
		public void SearchMessageEndDate_Success()
		{
			// Arrange
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestErrorMessage, (long)Constants.MessageType.Error);

			System.Threading.Thread.Sleep(200);
			long here = now();
			System.Threading.Thread.Sleep(200);

			message.Add(TestErrorMessage, (long)Constants.MessageType.Error);

			// Act
			Models.DTOs.Message.Search search = new Models.DTOs.Message.Search(null, null, null, here);
			var result = message.Search(search);

			// Assert
			Assert.NotNull(result);
			Assert.IsAssignableFrom<IEnumerable<Models.DTOs.Message.Get>>(result);
			Assert.Equal(4, result.Count());
		}
		[Fact(DisplayName = "Search messages by startDate and endDate and get result")]
		public void SearchMessageStartAndEndDate_Success()
		{
			// Arrange
			long oneMinuteAgo = now();
			System.Threading.Thread.Sleep(200);
			
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);

			System.Threading.Thread.Sleep(200);
			long here = now();
			System.Threading.Thread.Sleep(200);

			message.Add(TestInfoMessage, (long)Constants.MessageType.Info);
			message.Add(TestErrorMessage, (long)Constants.MessageType.Error);
			message.Add(TestErrorMessage, (long)Constants.MessageType.Error);

			// Act
			Models.DTOs.Message.Search search = new Models.DTOs.Message.Search(null, null, oneMinuteAgo, here);
			var result = message.Search(search);

			// Assert
			Assert.NotNull(result);
			Assert.IsAssignableFrom<IEnumerable<Models.DTOs.Message.Get>>(result);
			Assert.Equal(2, result.Count());
		}
	
		private long now()
		{
			return (long)DateTimeOffset.Now.ToUniversalTime().Subtract(
															new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
															).TotalMilliseconds;
		}
	}

}