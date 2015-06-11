namespace Genitor.Library.Core.Configuration
{
	using System.Xml;

	public interface IXmlElementConfigurable
	{
		//// Typickym pozadavkem dale muze byt ctor IXmlElementConfigurable(), ale neni vynucen

		void LoadConfig(XmlElement configElement);
	}
}
