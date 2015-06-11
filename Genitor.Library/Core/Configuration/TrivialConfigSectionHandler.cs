namespace Genitor.Library.Core.Configuration
{
	using System.Configuration;
	using System.Xml;

	public class TrivialConfigSectionHandler : IConfigurationSectionHandler
	{
		public object Create(object parent, object configContext, XmlNode section)
		{
			return section;
		}
	}
}
