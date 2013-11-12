using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ProxyServer
{
	/// <summary>
	/// Сводное описание для URLHandler
	/// </summary>
	public class URLHandler : IHttpHandler
	{

		public void ProcessRequest(HttpContext context)
		{
			context.Request.Cookies.Clear();
			//context.Response.ContentType = "text/plain";
			//context.Response.Write(context.Request.Params["proxyURL"]);

			string url = context.Request.Params["proxyURL"];
			HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
			HttpWebResponse response = null;
			try
			{
				response = (HttpWebResponse) request.GetResponse();

				string encoding = GetEncoding(response.ContentType);
				string page_content;
				using (StreamReader stream = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding)))
				{
					page_content = stream.ReadToEnd();
				}

				////Regex regex = new Regex(@"(?<=href="")(\S)*(?="")");
				////MatchCollection matches = regex.Matches(page_content);
				////foreach (Match match in matches)
				////{
				////	if (match.Value.StartsWith("/"))
				////		page_content = regex.Replace(match.Value, response.ResponseUri + match.Value);
				////}

				  
				
				context.Response.Write(page_content);
			}
			catch (WebException ex)
			{
				context.Response.ContentType = "text/plain";
				context.Response.Write(ex.Message);
			}
			finally 
			{
				if (response != null)
					response.Close();
			}
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		private string GetEncoding(string pageContentType) 
		{
			Regex regex = new Regex(@"(?<=charset=)(\w*\-+\d*)(?=\b)");
			MatchCollection matches = regex.Matches(pageContentType);
			
			return matches.Count != 0 ? matches[0].Value : null;
		}

		private void ReplaceLinks(string pageContent) 
		{
			Regex regex = new Regex(@"(?<=href="")(\S)*(?="")");
			MatchCollection matches = regex.Matches(pageContent);

			Dictionary<string, string> replacedLinks = new Dictionary<string, string>();
			foreach (Match match in matches)
			{
				replacedLinks.Add(match.Value, match.Value + @"");
			}
		}
	}
}