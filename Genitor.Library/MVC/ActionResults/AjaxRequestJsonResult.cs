// ReSharper disable CheckNamespace
namespace Genitor.Library.MVC
// ReSharper restore CheckNamespace
{
	using System.Collections.Generic;
	using System.Web.Mvc;

	using Genitor.Library.MVC.DTOs;

	public class AjaxRequestJsonResult : JsonResult
	{
		#region ctors

		public AjaxRequestJsonResult(AjaxCallResponse.ResultStatus state, string message)
		{
			this.Status = state;
			this.Message = message;
		}

		#endregion

		public AjaxCallResponse.ResultStatus Status { get; set; }

		public string Message { get; set; }

		public static AjaxRequestJsonResult Success(string message, object data = null)
		{
			return new AjaxRequestJsonResult(AjaxCallResponse.ResultStatus.Success, message) { Data = data };
		}

		public static AjaxRequestJsonResult Error(string message, object data = null)
		{
			return new AjaxRequestJsonResult(AjaxCallResponse.ResultStatus.Error, message) { Data = data };
		}

		public static AjaxRequestJsonResult Error(IEnumerable<KeyValuePair<string, ModelState>> modelStateDictionary, string message = null)
		{
			return new AjaxRequestJsonResult(AjaxCallResponse.ResultStatus.Error, message);
		}

		public override void ExecuteResult(ControllerContext context)
		{
			this.Data = new AjaxCallResponse { State = this.Status, Message = this.Message, Data = this.Data };

			base.ExecuteResult(context);
		}
	}
}