using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using System.Threading.Tasks;
using System.Web.Http.Description;

namespace MyProduct.Services.LogMessage.Controllers
{
	[RoutePrefix("")]
	public class Message : ApiController
	{
		private Interfaces.Message _message;
		private static log4net.ILog _logger;

		public Message() {
			_message = new Interfaces.Message();
			_logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		}
		public Message(Contexts.LogMessage context)
		{
			_message = new Interfaces.Message(context);
		}
		public Message(Contexts.LogMessage context, log4net.ILog logger)
		{
			_message = new Interfaces.Message(context);
			_logger = logger;
		}
		public Message(Interfaces.Message message)
		{
			_message = message;
		}
		public Message(Interfaces.Message message, log4net.ILog logger)
		{
			_message = message;
			_logger = logger;
		}

		protected override void Dispose(bool disposing){
			if (disposing) {
				_message.Dispose();
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Verify that the app is running and healthy. Consider the app healthy only if all it's dependencies are healthy.
		/// </summary>
		/// <returns>200 Success</returns>
		[Route("health")]
		[HttpGet]
		public OkResult HealthCheck()
		{
			return Ok();
		}

		/// <summary>
		/// Search saved messages
		/// </summary>
		/// <param name="search">The search params.</param>
		/// <returns>Collection of messages matching search criteria</returns>
		[ResponseType(typeof(IEnumerable<Models.DTOs.Message.Get>))]
		[HttpPost]
		[Route("search")]
		public async Task<IHttpActionResult> Search([FromBody] Models.DTOs.Message.Search search){
			if(!ModelState.IsValid){
				return base.ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));
			}
		
			var task1 = Task.Run(() => _message.Search(search));
			
			try{
				await Task.WhenAll(task1);
			}catch(ArgumentNullException){
				return BadRequest(Events.MissingParameter.ToString());
			}catch(ArgumentOutOfRangeException){
				return BadRequest(Events.MissingObject.ToString());
			}catch(AggregateException ae){
				var ex = ae.Flatten().InnerException;
				_logger.Error(String.Format("Error searching log messages [{0}]", Events.GetError),ex);
				return StatusCode(HttpStatusCode.InternalServerError);
			}

			if(task1.Status != TaskStatus.RanToCompletion) {
				_logger.Error(String.Format("Search log messages task did not complet [{0}]", Events.SearchError), task1.Exception);
				return StatusCode(HttpStatusCode.InternalServerError);
			}
			
			return Ok(task1.Result);
		}

		/// <summary>
		/// Get an individual message
		/// </summary>
		/// <param name="id">Message Id</param>
		/// <returns>The matching message</returns>
		[ResponseType(typeof(Models.DTOs.Message.Get))]
		[HttpGet]
		[Route("{id:long?}")]
		public async Task<IHttpActionResult> Get(long? id){
			if (!id.HasValue) {
				return base.ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, Events.MissingParameter.ToString()));
			}

			var task1 = await _message.Get(id.Value);

			if (task1 == null) {
				return base.ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound, Events.GetItemNotFound.ToString()));
			}

			return Ok(task1);
		}

		/// <summary>
		/// Add a message of type error
		/// </summary>
		/// <param name="message">Message details</param>
		/// <returns>201 Created</returns>
		[HttpPost]
		[Route("error")]
		public async Task<IHttpActionResult> Add_Error([FromBody] Models.DTOs.Message.Add message)
		{
			return await Add(message, (long)Constants.MessageType.Error);
		}

		/// <summary>
		/// Add a message of tpye info
		/// </summary>
		/// <param name="message">Message details</param>
		/// <returns>201 Created</returns>
		[HttpPost]
		[Route("info")]
		public async Task<IHttpActionResult> Add_Info([FromBody] Models.DTOs.Message.Add message){
			return await Add(message, (long)Constants.MessageType.Info);
		}
		
		private async Task<IHttpActionResult> Add(Models.DTOs.Message.Add message, long messageTypeId){
			if(!ModelState.IsValid) {
				return base.ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));
			}

			var task1 = Task.Run(() => _message.Add(message, messageTypeId));
			
			try{
				await Task.WhenAll(task1);
			}catch(ArgumentNullException){
				return BadRequest(Events.MissingParameter.ToString());
			}catch(NullReferenceException){
				return BadRequest(Events.MissingObject.ToString());
			}catch(AggregateException ae){
				var ex = ae.Flatten().InnerException;
				
				if(ex is ArgumentNullException)
					return BadRequest(Events.MissingParameter.ToString());

				_logger.Error(String.Format("Error adding log message [{0}]", Events.AddError), ex);
				return StatusCode(HttpStatusCode.InternalServerError);
			}

			if(task1.Status != TaskStatus.RanToCompletion) {
				_logger.Error(String.Format("Add log message task did not complete [{0}]", Events.AddError), task1.Exception);
				return StatusCode(HttpStatusCode.InternalServerError);
			}

			return StatusCode(HttpStatusCode.Created);
		}
	}
}