
using Newtonsoft.Json;
using System;

namespace MyProduct.Services.LogMessage.Models.DTOs.Message
{
	public class Get
	{
		private long _id;
		private string _text;
		private long _insertDate;
		private Models.DTOs.MessageType.Get _messageType;

		public long Id { get { return _id; } set { _id = value; } }
		public string Text { get { return _text; } set { _text = value; } }
		public long InsertDate { get { return _insertDate; } set { _insertDate = value; } }
		public Models.DTOs.MessageType.Get MessageType { get { return _messageType; } set { _messageType = value; } }

		public Get() { }
		[JsonConstructor]
		public Get(long id, string text, long messageTypeId, long insertDate)
		{
			_id = id;
			_text = text;
			_insertDate = insertDate;
			_messageType = new DTOs.MessageType.Get(messageTypeId, Enum.GetName(typeof(Constants.MessageType), messageTypeId));
		}
		public Get(Models.Message msg)
		{
			_id = msg.Id;
			_text = msg.Text;
			_insertDate = msg.InsertDate;
			_messageType = new DTOs.MessageType.Get(msg.MessageType);
		}
	}
}