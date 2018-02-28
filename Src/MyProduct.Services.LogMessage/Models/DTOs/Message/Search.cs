
using Newtonsoft.Json;

namespace MyProduct.Services.LogMessage.Models.DTOs.Message
{
	public class Search
	{
		private string _text;
		private long? _messageTypeId;
		private long? _startDate;
		private long? _endDate;

		public string Text { get { return _text; } set { _text= value; } }
		public long? MessageTypeId { get { return _messageTypeId; } set { _messageTypeId= value; } }
		public long? StartDate { get { return _startDate; } set { _startDate= value; } }
		public long? EndDate { get { return _endDate; } set { _endDate= value; } }

		public Search() { }
		[JsonConstructor]
		public Search(string text = null, long? messageTypeId = null, long? startDate = null, long? endDate = null)
		{
			_text = text;
			_messageTypeId = messageTypeId;
			_startDate = startDate;
			_endDate = endDate;
		}
	}
}