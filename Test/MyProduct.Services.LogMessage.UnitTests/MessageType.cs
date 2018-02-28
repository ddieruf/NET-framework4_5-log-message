using System;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

/*
The Unit Tests are focused on making sure the Interface class is working. This will in turn also test the data store.
 */
namespace MyProduct.Services.LogMessage.UnitTests
{
	[Collection("List message type collection")]
	public class ListMessageType : IDisposable
	{
		private readonly Interfaces.MessageType messageType;

		public ListMessageType()
		{
			Contexts.LogMessage context = new Contexts.LogMessage();
			Contexts.LogMessage_Initializer.Initialize(context);

			messageType = new Interfaces.MessageType(context);
		}
		public void Dispose()
		{
			messageType.Dispose();
		}
		[Fact(DisplayName = "List all message types successfully")]
		public void ListMessageTypes_Success()
		{
			// Arrange

			// Act
			var result = messageType.List();

			// Assert
			Assert.NotNull(result);
			Assert.IsAssignableFrom<IEnumerable<Models.DTOs.MessageType.Get>>(result); 
			Assert.Equal(2, result.Count());
		}
	}
	[Collection("Get message type collection")]
	public class GetMessageType : IDisposable
	{
		private readonly Interfaces.MessageType messageType;

		public GetMessageType()
		{
			Contexts.LogMessage context = new Contexts.LogMessage();
			Contexts.LogMessage_Initializer.Initialize(context);

			messageType = new Interfaces.MessageType(context);
		}
		public void Dispose()
		{
			messageType.Dispose();
		}
		[Fact(DisplayName = "Get a message type successfully")]
		public async Task GetInfoMessageType_Success()
		{
			// Arrange

			// Act
			var msgTy = await messageType.Get((long)Constants.MessageType.Info);

			// Assert
			Assert.NotNull(msgTy);
			Assert.IsType<Models.DTOs.MessageType.Get>(msgTy);
			Assert.Equal(msgTy.Name, Enum.GetName(typeof(Constants.MessageType), (long)Constants.MessageType.Info));
		}
	}
}