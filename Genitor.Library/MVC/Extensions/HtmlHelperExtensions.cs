// ReSharper disable CheckNamespace
namespace Genitor.Library.MVC
// ReSharper restore CheckNamespace
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Web.Mvc;
	using System.Web.Mvc.Html;

	using Genitor.Library.Core;

	public static class HtmlHelperExtensions
	{
		#region Lambda Expression helpers

		/// <summary>
		/// Vrati ID pro html tag z expression.
		/// </summary>
		public static string GetExpressionElementId<TModel, TProperty>(this HtmlHelper html, Expression<Func<TModel, TProperty>> expression)
		{
			return html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(expression));
		}

		/// <summary>
		/// Vrati Name pro html tag z expression.
		/// </summary>
		public static string GetExpressionElementName<TModel, TProperty>(this HtmlHelper html, Expression<Func<TModel, TProperty>> expression)
		{
			return html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));
		}

		/// <summary>
		/// Vrati hodnotu z expression.
		/// </summary>
		public static object GetExpressionValue<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
		{
			return ModelMetadata.FromLambdaExpression(expression, html.ViewData).Model;
		}

		#endregion

		#region messages

		private static readonly IDictionary<MessageTypes, string> _messageTypeFormats = new Dictionary<MessageTypes, string>
		                                                                                	{
		                                                                                		{ MessageTypes.Error, "<div class=\"alert alert-error\"><a class=\"close\" data-dismiss=\"alert\" href=\"#\">×</a>{0}</div>" },
		                                                                                		{ MessageTypes.Info, "<div class=\"alert alert-info\"><a class=\"close\" data-dismiss=\"alert\" href=\"#\">×</a>{0}</div>" },
		                                                                                		{ MessageTypes.Success, "<div class=\"alert alert-success\"><a class=\"close\" data-dismiss=\"alert\" href=\"#\">×</a>{0}</div>" },
		                                                                                		{ MessageTypes.Warning, "<div class=\"alert\"><a class=\"close\" data-dismiss=\"alert\" href=\"#\">×</a>{0}</div>" }
		                                                                                	};

		public static MvcHtmlString RenderFlashMessages(this HtmlHelper html)
		{
			var sb = new System.Text.StringBuilder();

			var workContext = DependencyResolver.Current.GetService<IWorkContextCore>();

			Guard.IsNotNull(workContext, "workContext");

			var flashMessages = workContext.GetFlashMessages();

			if (flashMessages != null)
			{
				foreach (var messageType in Enum.GetValues(typeof(MessageTypes)).Cast<MessageTypes>())
				{
					if (!flashMessages.ContainsKey(messageType))
					{
						continue;
					}

					var type = messageType;

					((List<string>)flashMessages[messageType]).ForEach(x => sb.AppendFormat(_messageTypeFormats[type], x));
				}
			}

			return new MvcHtmlString(sb.ToString());
		}


		public static MvcHtmlString RenderFlashMessage(this HtmlHelper html, string text, MessageTypes messageType)
		{
			return new MvcHtmlString(string.Format(_messageTypeFormats[messageType], text));
		}

		#endregion

		public static MvcHtmlString GetUrl(this HtmlHelper html, string actionName, string controllerName = null, object routeValues = null)
		{
			var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
			var url = urlHelper.Action(actionName, controllerName, routeValues);

			return MvcHtmlString.Create(url);
		}

		#region NamedValidationSummary

		public static readonly string SubmittedFormName = "NamedValidationSummary.SubmittedFormName";

		public static bool IsFormValid(this HtmlHelper htmlHelper, string formName)
		{
			return htmlHelper.ViewData.ModelState.IsValid || !StringExtensions.AreEqual(htmlHelper.ViewData[SubmittedFormName] as string, formName);
		}

		public static MvcHtmlString NamedValidationSummary(this HtmlHelper htmlHelper, string formName)
		{
			return NamedValidationSummary(htmlHelper, formName, false);
		}

		public static MvcHtmlString NamedValidationSummary(this HtmlHelper htmlHelper, string formName, bool excludePropertyErrors)
		{
			return NamedValidationSummary(htmlHelper, formName, excludePropertyErrors, null);
		}

		public static MvcHtmlString NamedValidationSummary(this HtmlHelper htmlHelper, string formName, string message)
		{
			return NamedValidationSummary(htmlHelper, formName, false, message, (object)null);
		}

		public static MvcHtmlString NamedValidationSummary(this HtmlHelper htmlHelper, string formName, bool excludePropertyErrors, string message)
		{
			return NamedValidationSummary(htmlHelper, formName, excludePropertyErrors, message, (object)null);
		}

		public static MvcHtmlString NamedValidationSummary(this HtmlHelper htmlHelper, string formName, string message, object htmlAttributes)
		{
			return NamedValidationSummary(htmlHelper, formName, false, message, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
		}

		public static MvcHtmlString NamedValidationSummary(this HtmlHelper htmlHelper, string formName, bool excludePropertyErrors, string message, object htmlAttributes)
		{
			return NamedValidationSummary(htmlHelper, formName, excludePropertyErrors, message, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
		}

		public static MvcHtmlString NamedValidationSummary(this HtmlHelper htmlHelper, string formName, string message, IDictionary<string, object> htmlAttributes)
		{
			return NamedValidationSummary(htmlHelper, formName, false, message, htmlAttributes);
		}

		public static MvcHtmlString NamedValidationSummary(this HtmlHelper htmlHelper, string formName, bool excludePropertyErrors, string message, IDictionary<string, object> htmlAttributes)
		{
			return StringExtensions.AreEqual(htmlHelper.ViewData[SubmittedFormName] as string, formName) ? htmlHelper.ValidationSummary(excludePropertyErrors, message, htmlAttributes) : new MvcHtmlString(string.Empty);
		}

		#endregion
	}
}