using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace MyProduct.Services.LogMessage.Interfaces
{
	public class MessageType : IDisposable
	{
		private readonly Contexts.LogMessage _context = new Contexts.LogMessage();
		private static readonly Expression<Func<Models.MessageType, Models.DTOs.MessageType.Get>> AsGetDto = x => new Models.DTOs.MessageType.Get() {
			Id=x.Id,
			Name=x.Name
		};

		public MessageType() { }
		public MessageType(Contexts.LogMessage context)
		{
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

		public IQueryable<Models.DTOs.MessageType.Get> List()
		{
			return _context.MessageTypes.Select(AsGetDto);
		}
		public async Task<Models.DTOs.MessageType.Get> Get(long id)
		{
			return await _context.MessageTypes.Select(AsGetDto)
															.SingleOrDefaultAsync(t => t.Id == id);
		}
	}
}