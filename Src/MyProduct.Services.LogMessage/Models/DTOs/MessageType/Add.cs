
namespace MyProduct.Services.LogMessage.Models.DTOs.MessageType
{
	public class Add
	{
		private long _id;
		private string _name;
		public Add() { }
		public Add(long id, string name){
			_id = id;
			_name = name;
		}
		public long Id { get { return _id; } set { _id = value; } }
		public string Name { get { return _name; } set { _name = value; } }
	}
}