using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using TestingSystem.Models;

namespace TestingSystem.Controllers
{
    public class TestsController : Controller
    {
		private TestsContext db = new TestsContext();

		
		// Sets unique id to client
		
		private void SetClientID()
		{
			Guid clientID;
			if (HttpContext.Request.Cookies["ClientID"] != null)
			{
				Guid.TryParse(HttpContext.Request.Cookies["ClientID"].Value, out clientID);
			}
			else 
			{
				clientID = Guid.NewGuid();
			}
			ViewBag.ClientID = clientID;
			HttpCookie cookie = new HttpCookie("ClientID", clientID.ToString());
			cookie.Expires = DateTime.Now.AddYears(1);
			Response.Cookies.Add(cookie);
		}

        //
        // GET: /Tests/

        public ActionResult Index()
        {
			
			// Add test records to DB 
			var test1 = new Test { Name = "Жизнь на Марсе", };
			db.Tests.Add(test1);
			db.SaveChanges();

			var test2 = new Test { Name = "Концепции ООП", };
			db.Tests.Add(test2);
			db.SaveChanges();

			var test3 = new Test { Name = "Синтаксис C#", };
			db.Tests.Add(test3);
			db.SaveChanges();

			

			// Sets id to client

			SetClientID();

			// Caching of the tests list

			Dictionary<int, string> tests = null;
			if (HttpContext.Cache["Tests"] == null)
			{
				tests = db.Tests.ToDictionary(i => i.ID, i => i.Name);
				HttpContext.Cache.Add("Tests", tests, null, DateTime.Now.AddHours(1), TimeSpan.Zero, CacheItemPriority.Default, null);
			}
			else
			{
				tests = (Dictionary<int, string>) HttpContext.Cache["Tests"];
			}

			return View(tests);
		}

		//
		// GET: /Tests/Details/5

		public ActionResult Details(int id = 0)
		{
			Test test = db.Tests.Find(id);
			if (test == null)
			{
				return HttpNotFound();
			}
			return View(test);
		}

		protected override void Dispose(bool disposing)
		{
			db.Dispose();
			base.Dispose(disposing);
		}

    }
}
