using System;
using System.Linq;
using System.Data.Entity;

namespace MyProduct.Services.LogMessage.Contexts
{
	public class LogMessage : DbContext
	{
		public LogMessage() : base() { }
		public DbSet<Models.Message> Messages { get; set; }
		public DbSet<Models.MessageType> MessageTypes { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Models.Message>()
						.HasRequired<Models.MessageType>(s => s.MessageType)
						.WithMany(g => g.Messages)
						.HasForeignKey<long>(s => s.MessageTypeId);
		}
	}
	public static class LogMessage_Initializer
	{
		public static void Initialize(Contexts.LogMessage context)
		{
			if (context.MessageTypes.Any()) {
				return;   // DB has been seeded
			}

			//Prefill message types
			var messageTypes = new Models.MessageType[]{
				new Models.MessageType(){
					Id = (long)Constants.MessageType.Error,
					 Name = Enum.GetName(typeof(Constants.MessageType), Constants.MessageType.Error)
				},
				new Models.MessageType(){
					Id = (long)Constants.MessageType.Info,
					 Name = Enum.GetName(typeof(Constants.MessageType), Constants.MessageType.Info)
				}
			};

			context.MessageTypes.AddRange(messageTypes);
			context.SaveChanges();
		}
	}
}