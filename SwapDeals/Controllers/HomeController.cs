using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SwapDeals.Models;

namespace SwapDeals.Controllers
{
    public class HomeController : Controller
    {
        private SwapDealsDBEntities db = new SwapDealsDBEntities();
        public ActionResult Index()
        {

            using (db)
            {
                // if (Session["user_id"] != null)
                var ads = db.Advertisements.SqlQuery("Select *from Advertisements")
                      .ToList<Advertisement>();
                return View(ads);
            }
        }

            public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}