using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyProduct.Services.LogMessage
{
	public class Constants
	{
		public enum MessageType : long
		{
			Error = 1,
			Info
		}
	}
}