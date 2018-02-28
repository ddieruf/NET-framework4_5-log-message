using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace MyProduct.Services.LogMessage.Models
{
	public class MessageType
	{
		[Key]
		[Required]
		public long Id { get; set; }

		[Required]
		[StringLength(50)]
		public string Name { get; set; }

		public ICollection<Models.Message> Messages { get; set; }
	}
}