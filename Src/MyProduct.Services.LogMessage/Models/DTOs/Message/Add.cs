
using Newtonsoft.Json;

namespace MyProduct.Services.LogMessage.Models.DTOs.Message
{
	public class Add
	{
		private string _text;

		public string Text { get { return _text; } set { _text = value; } }

		public Add() { }

		[JsonConstructor]
		public Add(string text)
		{
			_text = text;
		}
	}
}