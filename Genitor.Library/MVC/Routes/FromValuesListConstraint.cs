namespace Genitor.Library.MVC.Routes
{
	using System.Linq;
	using System.Web;
	using System.Web.Routing;

	public class FromValuesListConstraint : IRouteConstraint
	{
		private readonly string[] _values;

		#region ctors

		public FromValuesListConstraint(params string[] values)
		{
			this._values = values;
		}

		#endregion

		public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
		{
			return this._values.Contains(values[parameterName].ToString().ToLower());
		}
	}
}