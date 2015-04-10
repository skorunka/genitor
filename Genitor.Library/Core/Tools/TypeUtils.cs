namespace Genitor.Library.Core.Tools
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Reflection;
	using System.Xml;

	/// <summary>
	/// Howto:
	/// Obecne slouzi k ulehceni specifikace type. V konfiguraci lze definovat tzn. wellknown typ, 
	/// nebo wellknown namespace+assembly. Pri odkazovani na takove typy pak neni treba uvadet plne jmeno
	/// (namespace.typ, assembly), ale staci budto $Typ nebo Typ,$Assembly
	/// 
	/// TypeFinder lze pouzit pro nasledujici pripady:
	/// 1) nalezeni typu s pripadnou preferenci NS pripedne assembly (pokud neni u typu uvedeno)
	/// 2) nalezeni wellknown typu
	/// 3) nalezeni typu z wellknown [namespace, assembly]
	/// 
	/// Obecne se proste zavola nektere z pretizeni TypeFinder.FindType(...).
	/// Pokud typ neni nalezen, knihovna se pokusi najit tento typ v preferredNS a preferredAssembly.
	/// Pokud je typ ve tvaru "$Typ", hleda se podle konfigurace Genitor.Library.reflection/wellknownTypes.
	/// Pokud je typ ve tvaru "Typ, $Assembly", hleda se v NS a v assembly podle genitor.library.reflection/wellknownNamespaces.
	/// </summary>
	public class TypeUtils
	{
		#region static
		private static readonly TypeUtils _sInstance = new TypeUtils();

		public bool UseCache
		{
			get
			{
				return _sInstance._typeCache != null;
			}
			set
			{
				lock (_sInstance._thisLock)
				{
					if (_sInstance._typeCache == null && value)
						_sInstance._typeCache = new Dictionary<string, Type>();
					else if (_sInstance._typeCache != null && value == false)
						_sInstance._typeCache = null;
				}
			}
		}

		public static Type FindType(string typeName)
		{
			return _sInstance.FindTypeInner(typeName, null, null, true);
		}

		public static Type FindType(string typeName, bool throwOnError)
		{
			return _sInstance.FindTypeInner(typeName, null, null, throwOnError);
		}

		public static Type FindType(string typeName, string preferredNS, bool throwOnError)
		{
			return _sInstance.FindTypeInner(typeName, preferredNS, null, throwOnError);
		}

		public static Type FindType(string typeName, string preferredNS, string preferredAssembly, bool throwOnError)
		{
			return _sInstance.FindTypeInner(typeName, preferredNS, preferredAssembly, throwOnError);
		}

		public static Type FindType(string typeName, string preferredNS, Assembly preferredAssembly, bool throwOnError)
		{
			return _sInstance.FindTypeInner(typeName, preferredNS, preferredAssembly.FullName, throwOnError);
		}

		//http://geekswithblogs.net/marcel/archive/2007/03/24/109722.aspx
		public static object CreateGeneric(Type generic, Type innerType, params object[] args)
		{
			var specificType = generic.MakeGenericType(new[] { innerType });
			return Activator.CreateInstance(specificType, args);
		}
		#endregion

		private Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();
		private readonly Dictionary<string, string> _wellKnownTypes = new Dictionary<string, string>();
		private readonly Dictionary<string, Pair<string, string>> _wellKnownNamespaces = new Dictionary<string, Pair<string, string>>();
		private readonly object _thisLock = new object();

		private TypeUtils()
		{
			var configElement = ConfigurationManager.GetSection("Genitor.Library.Shared.reflection") as XmlElement;

			if (configElement == null)
				return;

			foreach (XmlElement wkTypeElement in configElement.SelectNodes("wellknownTypes/add"))
			{
				var key = wkTypeElement.GetAttribute("key");
				var type = wkTypeElement.GetAttribute("type");

				_wellKnownTypes.Add(key, type);
			}

			foreach (XmlElement wkTypeElement in configElement.SelectNodes("wellknownNamespaces/add"))
			{
				var key = wkTypeElement.GetAttribute("key");
				var ns = wkTypeElement.GetAttribute("namespace");
				var asm = wkTypeElement.GetAttribute("assembly");

				_wellKnownNamespaces.Add(key, new Pair<string, string>(ns, asm));
			}
		}

		private Type FindTypeInner(string name, string preferredNS, string preferredAssembly, bool throwOnError)
		{
			Type type;

			var key = String.Concat(name, "#", preferredNS, "#", preferredAssembly);
			lock (_thisLock)
			{
				if (_typeCache != null && _typeCache.TryGetValue(key, out type))
					return type;
			}

			type = FindTypeInner(name, preferredNS, preferredAssembly);
			if (type == null)
			{
				if (throwOnError)
					throw new TypeLoadException("Type '" + name + "' was not found.");
			}
			else
			{
				lock (_thisLock)
				{
					if (_typeCache != null)
						_typeCache[key] = type;
				}

			}
			return type;
		}

		private Type FindTypeInner(string name, string preferredNS, string preferredAssembly)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			name = name.Trim().Replace(" ", "");

			var commaPos = name.IndexOf(',');
			var dollarPos = name.IndexOf('$');

			// wellknown type
			if (dollarPos == 0)
			{
				string typeName;
				return _wellKnownTypes.TryGetValue(name, out typeName) ? Type.GetType(typeName) : null;
			}

			// pouze type bez assembly - zkusime najit a pak zkusime pripadne preferred srace
			if (commaPos < 0)
			{
				var type = Type.GetType(name);
				if (type != null)
					return type;

				string typeName;
				if (name.IndexOf('.') < 0 && !String.IsNullOrEmpty(preferredNS))
				{
					typeName = preferredNS + "." + name;
					type = Type.GetType(typeName);
					if (type != null)
						return type;
				}

				if (String.IsNullOrEmpty(preferredAssembly))
					return null;

				typeName = name + "," + preferredAssembly;
				type = Type.GetType(typeName);
				if (type != null)
					return type;

				if (name.IndexOf('.') < 0 && !String.IsNullOrEmpty(preferredNS))
				{
					typeName = preferredNS + "." + typeName;
					type = Type.GetType(typeName);
					if (type != null)
						return type;
				}

				return null;
			}

			// type s wellknown assembly (a namespace NS)
			if (dollarPos > 0)
			{
				Pair<string, string> wellknownPair;
				if (_wellKnownNamespaces.TryGetValue(name.Substring(dollarPos), out wellknownPair))
				{
					name = name.Remove(dollarPos);
					return Type.GetType(String.Concat(wellknownPair.First, name, ",", wellknownPair.Second));
				}

				return null;
			}

			// nic neni wellknown a je uveden type i assembly - zadna extra logika
			return Type.GetType(name, true);
		}
	}
}
