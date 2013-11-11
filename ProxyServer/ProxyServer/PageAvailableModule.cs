using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProxyServer
{
	public class PageAvailableModule : IHttpModule
	{
		public void Init(HttpApplication context)
		{
			context.BeginRequest += contextBeginRequest;
		}
		
		public void Dispose()
		{
		}

		private void contextBeginRequest(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;
			if (app.Request.Path == "/")
			{
				app.Response.Redirect("MainPage.html");
				app.CompleteRequest();
			}
		}
	}
}