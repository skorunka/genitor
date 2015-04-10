using System;
using System.Configuration;
using System.Xml;

namespace Genitor.Library.Core.Configuration
{
	public class TrivialConfigSectionHandler : IConfigurationSectionHandler
	{
		public object Create(object parent, object configContext, XmlNode section)
		{
			return section;
		}
	}
}
