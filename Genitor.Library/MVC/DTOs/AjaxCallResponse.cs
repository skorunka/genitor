namespace Genitor.Library.MVC.DTOs
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;

	public class AjaxCallResponse
	{
		#region ctors

		public AjaxCallResponse()
		{
			this.State = ResultStatus.Success;
		}

		public AjaxCallResponse(IEnumerable<KeyValuePair<string, ModelState>> modelStateDictionary, string errorText = null)
		{
			this.State = ResultStatus.Error;
			this.Data = from x in modelStateDictionary
						where x.Value.Errors.Count > 0
						select new { key = x.Key, errors = x.Value.Errors.Select(y => y.ErrorMessage).ToArray() };
			this.Message = errorText;
		}

		#endregion

		public enum ResultStatus
		{
			Empty = 0,
			Error = -1,
			Success = 1
		}

		public ResultStatus State { get; set; }

		public string Message { get; set; }

		public object Data { get; set; }
	}
}