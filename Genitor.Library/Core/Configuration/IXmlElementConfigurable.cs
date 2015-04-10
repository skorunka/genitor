using System;
using System.Xml;

namespace Genitor.Library.Core.Configuration
{
	public interface IXmlElementConfigurable
	{
		// Typickym pozadavkem dale muze byt ctor IXmlElementConfigurable(), ale neni vynucen

		void LoadConfig(XmlElement configElement);
	}
}
