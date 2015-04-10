namespace Genitor.Library.MVC.Attributes
{
	using System.Web.Mvc;

	/// <summary>
	/// The model state temp data transfer.
	/// </summary>
	public abstract class ModelStateTempDataTransferAttribute : ActionFilterAttribute
	{
		/// <summary>
		/// The key.
		/// </summary>
		protected static readonly string Key = typeof(ModelStateTempDataTransferAttribute).FullName;
	}
}