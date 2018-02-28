using System;

namespace MyProduct.Services.LogMessage.Models.DTOs.MessageType
{
	public class Get
	{
		private long _id;
		private string _name;
		public Get() { }
		public Get(long id, string name)
		{
			_id = id;
			_name = name;
		}
		public Get(Models.MessageType itm)
		{
			_id = itm.Id;
			_name = itm.Name;
		}
		public long Id { get { return _id; } set { _id = value; } }
		public string Name { get { return _name; } set { _name = value; } }
	}
}