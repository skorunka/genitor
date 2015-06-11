namespace Genitor.Library.Core.Configuration
{
	using System;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Configuration;
	using System.Reflection;

	using Genitor.Library.Core.Tools;

	public static class ConfigNameValueUtils
	{
		private static readonly Dictionary<string, WeakReference> SectionCache = new Dictionary<string, WeakReference>();

		public delegate T GetValueHandler<out T>(string value);

		#region AutFillFields methods
		/// <summary>
		/// Naplni vsechny staticke a instancni fieldy dane instance, ktere definuji atribut ConfigAutoFill
		/// </summary>
		public static void AutoFillFields(object instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			AutoFillFields(instance.GetType(), instance);
		}

		/// <summary>
		/// Naplni pouze staticke fieldy daneho typu, ktere definuji atribut ConfigAutoFill
		/// </summary>
		public static void AutoFillFields(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			AutoFillFields(type, null);
		}

		/// <summary>
		/// Naplni fieldy daneho typu/instance, ktere definuji atribut ConfigAutoFill
		/// </summary>
		private static void AutoFillFields(Type type, object instance)
		{
			//// pokusi se zjistit jestli existuje definice pro default section
			string defaultSection = null;
			var attrs = type.GetCustomAttributes(typeof(ConfigDefaultsAttribute), false);
			if (attrs.Length > 0)
			{
				defaultSection = ((ConfigDefaultsAttribute)attrs[0]).Section;
			}

			//// nakde vsechny fieldy pro typ a pripadni i instanci
			var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			if (instance != null)
			{
				flags |= BindingFlags.Instance;
			}

			var fields = type.GetFields(flags);

			//// fieldy proiteruje a pro kazdy zjisti handler metodu a zavola ji
			foreach (var fi in fields)
			{
				var autoFill = GetAutoFillAttribute(fi);
				if (autoFill == null)
				{
					continue;
				}

				if (defaultSection == null && string.IsNullOrEmpty(autoFill.Section))
				{
					throw new ConfigurationErrorsException(string.Format("ConfigAutoFillAttribute definition for '{0}.{1}' must declare 'Section' property when the class '{0}' is not decored by ConfigDefaultsAttribute", type.FullName, fi.Name));
				}

				//// if (!autoFill.IsRequired && autoFill.DefaultValue == null)
				////    throw new ConfigurationErrorsException(String.Format("ConfigAutoFillAttribute definition for '{0}.{1}' must declare 'DefaultValue' property when the 'IsRequired' property is false", type.FullName, fi.Name));

				var miHandler = GetHandler(autoFill, type, fi);

				object value;

				if (GetAutoFillValue(
					autoFill.Section ?? defaultSection,
					autoFill.Name ?? fi.Name,
					autoFill.IsRequired,
					autoFill.DefaultValue,
					fi.FieldType,
					miHandler,
					out value))
				{
					fi.SetValue(fi.IsStatic ? null : instance, value);
				}
			}
		}
		#endregion

		#region Public Get... Methods
		public static T GetValue<T>(string section, string key, bool isRequired, T defaultValue, GetValueHandler<T> handler)
		{
			if (section == null)
			{
				throw new ArgumentNullException(nameof(section));
			}

			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			if (handler == null)
			{
				throw new ArgumentNullException(nameof(handler));
			}

			if (!isRequired && defaultValue == null)
			{
				throw new ArgumentException("Parameter 'defaultValue' can not be null when parameter 'isRequired' == false");
			}

			var strValue = RetrieveString(section, key);
			if (strValue == null)
			{
				if (isRequired)
				{
					throw new ConfigMissingKeyException(section, key);
				}

				return defaultValue;
			}

			try
			{
				return handler(strValue);
			}
			catch (ConfigInvalidValueException exc)
			{
				throw new ConfigInvalidValueException(section, key, exc.Type);
			}
		}

		public static string GetString(string section, string key, bool isRequired, string defaultValue)
		{
			return GetValue(section, key, isRequired, defaultValue, ConfigHandlers.StringHandler);
		}
		#endregion // Protected Get... Methods

		#region Private Methods
		/// <summary>
		/// Zjisti hodnotu pro field
		/// </summary>
		/// <param name="section">Sekce v konfiguraci</param>
		/// <param name="key">Klic polozky v sekci</param>
		/// <param name="isRequired">Je polozka povinna</param>
		/// <param name="defaultValue">Vychozi hodnota</param>
		/// <param name="type">Typ polozky</param>
		/// <param name="handler">Odkaz (pres reflection) na handler</param>
		/// <param name="value">Hodnota ziskana pro dany field</param>
		/// <returns>true pokud se ma hodnota priradit, jinak false</returns>
		private static bool GetAutoFillValue(string section, string key, bool isRequired, string defaultValue, Type type, MethodInfo handler, out object value)
		{
			value = null;

			var strValue = RetrieveString(section, key);
			if (strValue == null)
			{
				if (isRequired)
				{
					throw new ConfigMissingKeyException(section, key);
				}

				if (defaultValue == null)
				{
					return false;
				}

				strValue = defaultValue;
			}

			try
			{
				// pokud ma handler jeden parametr predpokladame ze to je Handler(string) jinak Handler(string, Type)
				value = handler.GetParameters().Length == 1 ? handler.Invoke(null, new object[] { strValue }) : handler.Invoke(null, new object[] { strValue, type });

				return true;
			}
			catch (Exception exc)
			{
				if (exc is TargetInvocationException && exc.InnerException is ConfigInvalidValueException)
				{
					throw new ConfigInvalidValueException(section, key, (exc.InnerException as ConfigInvalidValueException).Type);
				}

				throw;
			}
		}

		/// <summary>
		/// Vytahne z konfiguracni sekce "section" hodnotu pro dany polozku
		/// </summary>
		private static string RetrieveString(string section, string key)
		{
			WeakReference weakRef;
			if (SectionCache.TryGetValue(section, out weakRef))
			{
				var obj = weakRef.Target;
				if (obj != null)
				{
					return ((NameValueCollection)obj)[key];
				}
			}

			var col = ConfigurationManager.GetSection(section) as NameValueCollection;
			if (col == null)
			{
				throw new ConfigurationErrorsException(string.Format("Configuration section '{0}' was not found. It is required for key '{1}'.", section, key));
			}

			SectionCache[section] = new WeakReference(col);
			return col[key];
		}

		/// <summary>
		/// Vybere handler pro dany field. Pokud neni uveden v atributu ConfigAutoFill, pokusi se ho odvodit z navratoveho typu
		/// </summary>
		private static MethodInfo GetHandler(ConfigAutoFillAttribute autoFill, Type type, FieldInfo fi)
		{
			// AutoFill ma handler
			if (!string.IsNullOrEmpty(autoFill.Handler))
			{
				var handlerMethod = autoFill.HandlerMethod;
				Type handlerType;

				if (string.IsNullOrEmpty(autoFill.HandlerTypeName) || autoFill.HandlerTypeName == "*config")
				{
					handlerType = typeof(ConfigHandlers);
				}
				else if (autoFill.HandlerTypeName == "*self")
				{
					handlerType = type;
				}
				else
				{
					handlerType = TypeUtils.FindType(autoFill.HandlerTypeName);
					if (handlerType == null)
					{
						throw new ConfigurationErrorsException(string.Format("ConfigAutoFillAttribute definition for '{0}.{1}' specifies 'HandlerType' '{2}' which was not found", type.FullName, fi.Name, autoFill.HandlerTypeName));
					}
				}

				var mi = handlerType.GetMethod(handlerMethod, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				if (mi == null)
				{
					throw new ConfigurationErrorsException(string.Format("ConfigAutoFillAttribute definition for '{0}.{1}' declares handler '{2}' of type '{3}' which was not found", type.FullName, fi.Name, handlerMethod, handlerType.FullName));
				}

				return mi;
			}
			else
			//// handler vydedukujeme
			{
				var handlerMethod = ConfigHandlers.GetHandlerName(fi.FieldType);

				if (handlerMethod == null)
				{
					throw new ConfigurationErrorsException(string.Format("ConfigAutoFillAttribute definition for '{0}.{1}' must declare 'Handler' property because field type '{2}' is not recognized", type.FullName, fi.Name, fi.FieldType.FullName));
				}

				var mi = typeof(ConfigHandlers).GetMethod(handlerMethod, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);

				return mi;
			}
		}

		private static ConfigAutoFillAttribute GetAutoFillAttribute(ICustomAttributeProvider fi)
		{
			var attrs = fi.GetCustomAttributes(typeof(ConfigAutoFillAttribute), false);
			if (attrs.Length > 0)
			{
				return (ConfigAutoFillAttribute)attrs[0];
			}

			return null;
		}

		//// MEGA BETA VERZE - NENI OTESTOVANO!!!!

		#endregion
	}
}
