namespace Genitor.Library.Core.Tools
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Net.Mail;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Web;
	using System.Web.UI;

	/// <summary>
	/// Helper class to send templated e-mails and embed images
	/// </summary>
	public class Mailer
	{
		#region Constants and Fields

		private readonly Control _templateControl;

		#endregion

		#region Constructors and Destructors

		public Mailer(Control templateControl = null, IDictionary<string, object> mailTemplateParameters = null)
		{
			this._templateControl = templateControl;
			this.MailTemplateParameters = mailTemplateParameters ?? new Dictionary<string, object>();
		}

		#endregion

		#region Public Properties

		public bool EnableSsl { get; set; }

		/// <summary>
		/// 	Gets or sets a Dictionary to hold parameters for the template control
		/// </summary>
		public IDictionary<string, object> MailTemplateParameters { get; set; }

		#endregion

		#region Public Methods and Operators

		public static IEnumerable<MailAddress> GetMailAddressCollection(string addresses)
		{
			var ret = new MailAddressCollection();
			addresses.Split(';').Where(a => !string.IsNullOrWhiteSpace(a)).ToList().ForEach(ret.Add);
			return ret;
		}

		public void Send(
			string from,
			string to,
			string subject,
			string body = null,
			string cc = null,
			string bcc = null,
			IList<Attachment> attachments = null)
		{
			Guard.IsNotNull(from, from);
			Guard.IsNotNull(to, to);
			Guard.IsNotNull(subject, subject);
			Guard.IsNotNull(body, body);

			var mailMessage = new MailMessage { From = new MailAddress(from), Subject = subject, Body = body };

			mailMessage.To.AddRange(GetMailAddressCollection(to));

			if (!string.IsNullOrWhiteSpace(cc))
			{
				mailMessage.CC.AddRange(GetMailAddressCollection(cc));
			}

			if (!string.IsNullOrWhiteSpace(bcc))
			{
				mailMessage.Bcc.AddRange(GetMailAddressCollection(bcc));
			}

			if (attachments != null && attachments.Any())
			{
				mailMessage.Attachments.AddRange(attachments);
			}

			this.Send(mailMessage);
		}

		public void Send(MailMessage mailMessage)
		{
			Guard.IsNotNull(mailMessage, "mailMessage");
			if (this._templateControl == null && string.IsNullOrWhiteSpace(mailMessage.Body))
			{
				throw new ArgumentException("Either mail body or Template must be set.");
			}

			using (mailMessage)
			{
				//// checks if a template control is set
				if (this._templateControl != null)
				{
					// gets the control's Type
					var templateType = this._templateControl.GetType();
					//// sets properties from the dictionary using reflection
					if (this.MailTemplateParameters != null)
					{
						foreach (var parameter in this.MailTemplateParameters)
						{
							var prop = templateType.GetProperty(parameter.Key);
							if (prop != null)
							{
								prop.SetValue(this._templateControl, parameter.Value, null);
							}
						}
					}

					//// renders the template control as a string
					var stringBuilder = new StringBuilder();
					using (var textWriter = new StringWriter(stringBuilder))
					{
						using (var htmlWriter = new HtmlTextWriter(textWriter))
						{
							this._templateControl.RenderControl(htmlWriter);
							mailMessage.Body = stringBuilder.ToString();
						}
					}
				}

				// embeds images for html messages
				if (mailMessage.IsBodyHtml)
				{
					var embedder = new ImagesEmbedder();
					embedder.EmbedImages(mailMessage);
				}

				// sends the message
				using (var client = new SmtpClient())
				{
					client.EnableSsl = this.EnableSsl;
					client.Send(mailMessage);
				}
			}
		}

		public bool TrySend(
			string from,
			string to,
			string subject,
			string body = null,
			string cc = null,
			string bcc = null,
			IList<Attachment> attachments = null)
		{
			try
			{
				this.Send(from, to, subject, body, cc, bcc, attachments);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool TrySend(MailMessage mailMessage)
		{
			try
			{
				this.Send(mailMessage);
				return true;
			}
			catch
			{
				return false;
			}
		}

		#endregion

		/// <summary>
		/// Helper class to embed images in html e-mail messages
		/// </summary>
		private class ImagesEmbedder
		{
			#region Constants and Fields

			/// <summary>
			/// 	Holds a list of linked resources
			/// </summary>
			private List<LinkedResource> _linkedResources;

			#endregion

			#region Public Methods and Operators

			/// <summary>
			/// 	Embeds images in the MailMessage
			/// </summary>
			/// <param name="mailMessage"> MailMessage instance </param>
			public void EmbedImages(MailMessage mailMessage)
			{
				// creates a new list of linked resources
				this._linkedResources = new List<LinkedResource>();

				// regular expression to find <img> tags within the message's body
				var regex = new Regex(
					"src=(?:\"|\')?(?<imgSrc>[^>]*[^/].(?:jpeg|jpg|bmp|gif|png|tif))(?:\"|\')?",
					RegexOptions.Compiled | RegexOptions.IgnoreCase);

				// replaces urls in <img> tag with corresponding unique ids
				var body = regex.Replace(mailMessage.Body, this.ImageHandler);

				// creates an alternate view with linked resources
				var htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
				foreach (var linkedResource in this._linkedResources)
				{
					htmlView.LinkedResources.Add(linkedResource);
				}

				mailMessage.AlternateViews.Add(htmlView);

				// release resources
				this._linkedResources.Clear();
				this._linkedResources = null;
			}

			#endregion

			#region Methods

			/// <summary>
			/// 	Updates imgs' src and adds linkedResources to the list
			/// </summary>
			/// <param name="match"> </param>
			/// <returns> </returns>
			private string ImageHandler(Match match)
			{
				// gets the imgs src attribute from the Regular Expression's match
				var src = match.Groups[1].Value;
				LinkedResource linkedResource = null;
				Uri uri;

				// if it's a valid, absolute Uri, download the image from remote source using a WebClient request
				if (Uri.TryCreate(src, UriKind.Absolute, out uri))
				{
					using (var webClient = new WebClient())
					{
						try
						{
							var buf = webClient.DownloadData(uri);
							var memoryStream = new MemoryStream(buf);
							linkedResource = new LinkedResource(memoryStream);
						}

						// ReSharper disable EmptyGeneralCatchClause
						catch
						{
						}

						// ReSharper restore EmptyGeneralCatchClause
					}
				}
				else
				{
					// otherwise, get the image from local server
					linkedResource = new LinkedResource(HttpContext.Current.Server.MapPath(src));
				}

				if (linkedResource != null)
				{
					// add the linked resource to the list
					linkedResource.ContentId = Guid.NewGuid().ToString();
					linkedResource.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
					this._linkedResources.Add(linkedResource);

					// update the img's src attribute with the linked resource's unique id
					return $"src='cid:{linkedResource.ContentId}'";
				}

				// leave src unchanged if we can't get the image
				return match.Value;
			}

			#endregion
		}
	}
}