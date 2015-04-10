namespace Genitor.Library.MVC.Controllers
{
	using System.Web.Mvc;

	public class ErrorController : Controller
	{
		public ViewResult Index()
		{
			return this.View("Error");
		}

		public ViewResult Forbidden()
		{
			Response.StatusCode = 403;
			return this.View("Forbidden");
		}

		public ViewResult NotFound()
		{
			Response.StatusCode = 404;
			return this.View("NotFound");
		}

		public ViewResult InternalServerError()
		{
			Response.StatusCode = 500;
			return this.View("InternalServerError");
		}

		public ViewResult ServiceUnavailable()
		{
			Response.StatusCode = 503;
			return this.View("ServiceUnavailable");
		}
	}
}
