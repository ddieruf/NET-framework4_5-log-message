using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProduct.Services.LogMessage.Models
{
	public class Message
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long Id { get; set; }

		[Required]
		[StringLength(2000)]
		public string Text { get; set; }

		[Required]
		public long InsertDate { get; set; }

		[Required]
		public long MessageTypeId { get; set; }
		public Models.MessageType MessageType { get; set; }
	}
}	