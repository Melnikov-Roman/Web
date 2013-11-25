using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using HtmlAgilityPack;

namespace ProxyServer
{
	/// <summary>
	/// Handler that used to process requests from main page
	/// </summary>
	public class MainPageHandler : IHttpHandler, IRequiresSessionState
	{

		public void ProcessRequest(HttpContext context)
		{
			context.Request.Cookies.Clear();
			context.Response.Cookies.Clear();

			string url = String.Empty;
			if (context.Request.Path == "/MainPageHandler.ashx")
			{
				url = context.Request.Params["proxyUrl"];
			}
			else
			{	
				url = context.Request.Params["url"];
				if (url.Contains("proxyserverlink"))
				{
					url = GetOriginalLink(context, url);
				}
			}
 
			HttpWebResponse response = null;
			try
			{
				HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
				response = (HttpWebResponse) request.GetResponse();

				if (context.Session["currentHost"] == null)
					context.Session.Add("currentHost", url);

				string contentType = GetContentType(response.ContentType);

				if (contentType == "text/html")
				{
					string encoding = GetContentEncoding(response.ContentType);
					string pageContent;
					
					using (StreamReader stream = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding)))
					{
						pageContent = stream.ReadToEnd();
					}

					ChangeResponse(context, ref pageContent);
					context.Response.ContentType = contentType;
					context.Response.Write(pageContent);
				}
				else if (contentType == "image/png")
				{
					context.Response.ContentType = contentType;
					Bitmap bmp = new Bitmap(response.GetResponseStream());
					bmp.Save(context.Response.OutputStream, ImageFormat.Png);
					bmp.Dispose();
				}
				else
				{
					//
				}
				
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

		private string GetOriginalLink(HttpContext context, string url) 
		{
			string originLink = String.Empty;
			Dictionary<string, string> savedLinks = (Dictionary<string, string>) context.Session["pageLinks"];
			if (savedLinks.ContainsValue(url) && context.Session["pageLinks"] != null)
			{
				url = savedLinks.FirstOrDefault(x => x.Value == url).Key;
				if (!url.Contains("http:") && !url.Contains("https:"))
				{
					originLink = context.Session["currentHost"] + url;
				}
				else
				{
					originLink = url;
				}
			}

			return originLink;
		}

		private string GetContentEncoding(string pageContentType)
		{
			Regex regex = new Regex(@"(?<=charset=)(\w*\-+\d*)(?=\b)");
			MatchCollection matches = regex.Matches(pageContentType);

			return matches.Count != 0 ? matches[0].Value : "1251";
		}

		private string GetContentType(string pageContentType)
		{
			Regex regex = new Regex(@"(?<=\b)(\w+/\w+)(?=\b)");
			MatchCollection matches = regex.Matches(pageContentType);

			return matches.Count != 0 ? matches[0].Value : null;
		}

		private void ChangeResponse(HttpContext context, ref string pageContent) 
		{
			ReplaceLinks(context, ref pageContent, "a", "href");
			ReplaceLinks(context, ref pageContent, "img", "src");
		}
				
		private void ReplaceLinks(HttpContext context, ref string pageContent, string replacedTag, string replacedAttribute)
		{
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(pageContent);

			HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(String.Format("//{0}[@{1}]",replacedTag,replacedAttribute));

			if (nodes == null)
				return;

			Dictionary<string, string> pageLinks = new Dictionary<string,string>();
						
			foreach (HtmlNode node in nodes)
			{
				string pageLink = node.Attributes[replacedAttribute].Value;

				if (String.IsNullOrEmpty(pageLink) || pageLink.StartsWith("#"))
				{ 
					continue; 
				}

				string newPageLink = String.Empty;

				if (pageLink.Contains("/?"))
				{
					newPageLink = pageLink.Replace("/?", "/proxyserverlink?");
				}
				else if (pageLink.Contains("?"))
				{
					newPageLink = pageLink.Replace("?", "/proxyserverlink?");
				}
				else if (pageLink.EndsWith("/"))
				{
					newPageLink = pageLink + "proxyserverlink";
				}
				else
				{
					newPageLink = pageLink + ".proxyserverlink";
				}

				if (!pageLinks.ContainsKey(pageLink))
					pageLinks.Add(pageLink, newPageLink);
			}

			foreach (string link in pageLinks.Keys)
			{
				string newPageLink;
				if (pageLinks.TryGetValue(link, out newPageLink))
					pageContent = pageContent.Replace(link, newPageLink);
			}

			if (context.Session["pageLinks"] == null)
			{
				context.Session.Add("pageLinks", pageLinks); 
			}
			else 
			{
				Dictionary<string,string> savedLinks = (Dictionary<string,string>) context.Session["pageLinks"];
				foreach (string pageLink in pageLinks.Keys)
				{
					if (!savedLinks.ContainsKey(pageLink))
					{
						savedLinks.Add(pageLink,pageLinks[pageLink]);
					}
				}
				context.Session["pageLinks"] = savedLinks; 
			}
		}
	}
}