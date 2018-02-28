using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyProduct.Services.LogMessage.Interfaces
{
	public class Message : IDisposable
	{
		private readonly Contexts.LogMessage _context;

		public Message() {
			_context = new Contexts.LogMessage();
		}
		public Message(Contexts.LogMessage context) {
			_context = context;
		}

		protected void Dispose(bool disposing)
		{
			if (disposing && _context != null) {
				_context.Dispose();
			}
		}
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Typed lambda expression for Select() method.
		private static readonly Expression<Func<Models.Message, Models.DTOs.Message.Get>> AsGetDto = x => new Models.DTOs.Message.Get() {
			Id = x.Id,
			InsertDate = x.InsertDate,
			MessageType = new Models.DTOs.MessageType.Get() {
				Id = x.MessageType.Id,
				Name = x.MessageType.Name
			},
			Text = x.Text
		};

		public IQueryable<Models.DTOs.Message.Get> Search(Models.DTOs.Message.Search sch){
			if(sch == null)
				throw new ArgumentNullException("Views.Message.Search");
			
			if(string.IsNullOrEmpty(sch.Text) && !sch.MessageTypeId.HasValue && !sch.StartDate.HasValue
					&& !sch.EndDate.HasValue)
				throw new ArgumentOutOfRangeException("Views.Message.Search");

			var ret = (from msg in _context.Messages.Include("Models.MessageType")
								 where (!string.IsNullOrEmpty(sch.Text)? (msg.Text.ToLower().Contains(sch.Text.ToLower())) : true)
										&& (sch.MessageTypeId.HasValue?(msg.MessageTypeId == sch.MessageTypeId):true)
										&& (sch.StartDate.HasValue?(msg.InsertDate >= sch.StartDate.Value):true)
										&& (sch.EndDate.HasValue?(msg.InsertDate <= sch.EndDate.Value):true)
								 select new Models.DTOs.Message.Get() {
									Id = msg.Id,
									InsertDate = msg.InsertDate,
									MessageType = new Models.DTOs.MessageType.Get() {
										Id = msg.MessageType.Id,
										Name = msg.MessageType.Name
									},
									Text = msg.Text
								 });

			return ret;
		}

		public long Add(Models.DTOs.Message.Add item, long messageTypeId){
			if(item == null)
				throw new NullReferenceException("Views.Message.Add");
			
			if(string.IsNullOrEmpty(item.Text))
				throw new ArgumentNullException("Views.Message.Add.Text");
			
			var msg = new Models.Message(){
				Text = item.Text,
				MessageTypeId = messageTypeId,
				InsertDate = (long)DateTimeOffset.Now.ToUniversalTime().Subtract(
															new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
															).TotalMilliseconds
			};
			
			_context.Messages.Add(msg);
			_context.SaveChanges();

			return msg.Id;
		}
			
		public async Task<Models.DTOs.Message.Get> Get(long messageId){
			var msg = _context.Messages.Include("Models.MessageType")
																	.Select(AsGetDto)
																	.SingleOrDefaultAsync(t => t.Id == messageId);
			return await msg;
		}
	}
}