using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ProxyServer
{
	public class ProxyUrlHandler : IHttpHandler
	{
		/// <summary>
		/// Handler that used to process all requests and responses
		/// </summary>
		#region IHttpHandler Members

		public void ProcessRequest(HttpContext context)
		{
			if (context.Request.Path == "/MainPage.html")
				context.Response.Close();
			context.Request.Cookies.Clear();
			context.Response.Cookies.Clear();


			//string url = context.Request.Params["proxyURL"];
			string url = context.Request.Path;
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
				//Regex regex = new Regex(@"(?<=(href=""|src=""))(\S)*(?="")");
				//Byte[] lnByte;
				//FileStream lxFS; 
				//String lsResponse = string.Empty;

				//using (BinaryReader reader = new BinaryReader(response.GetResponseStream()))
				//{
				//	lnByte = reader.ReadBytes((int) response.ContentLength);


				//	using (lxFS = new FileStream(@"D:\googla.png", FileMode.Create))
				//	{
				//		lxFS.Write(lnByte, 0, lnByte.Length);
				//	}
				//}


				Regex regex = new Regex(@"(?<=(http://|https://|//))(\S)*(?="")");
				MatchCollection matches = regex.Matches(page_content);
				//foreach (Match match in matches)
				//{
				//	//if (match.Value.StartsWith("/"))
				//	page_content.Replace(match.Value, match.Value + ".axd");
				//}

				context.Response.ContentType = "text/html";

				context.Response.Write(page_content);
				//context.Response.ContentType = "image/png";
				//context.Response.Write(lxFS);

				//string file = context.Server.MapPath(url);

				//
				//context.Response.ContentType = response.ContentType;

				//Bitmap bmp = new Bitmap(response.GetResponseStream());
				//bmp.Save(context.Response.OutputStream, ImageFormat.Png);
				//bmp.Dispose();



				//context.Response.ContentType = "image/png";
				//context.Response.Write("[hryn");


				//context.Response.ContentType = "image/png";
				//bmp.Save(context.Response.OutputStream, ImageFormat.Png);
				//bmp.Dispose();
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

		//private void ReplaceLinks(string pageContent)
		//{
		//	Regex regex = new Regex(@"(?<=href="")(\S)*(?="")");
		//	MatchCollection matches = regex.Matches(pageContent);

		//	Dictionary<string, string> replacedLinks = new Dictionary<string, string>();
		//	foreach (Match match in matches)
		//	{
		//		replacedLinks.Add(match.Value, match.Value + @"");
		//	}
		//}

		#endregion
	}
}
