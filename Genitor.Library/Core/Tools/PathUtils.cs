namespace Genitor.Library.Core.Tools
{
	using System.IO;
	using System.Reflection;

	public static class PathUtils
	{
		public static string MapPath(string path)
		{
			if (path.StartsWith("~"))
			{
				if (System.Web.HttpContext.Current == null)
				{
					var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
					var appPath = Path.GetDirectoryName(assembly.Location);
					if (appPath != null)
					{
						path = Path.Combine(appPath, path.Substring(1));
					}
				}
				else
				{
					path = System.Web.HttpContext.Current.Server.MapPath(path);
				}
			}

			return path;
		}
	}
}