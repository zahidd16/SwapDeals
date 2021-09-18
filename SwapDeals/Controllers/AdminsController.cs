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
    public class AdminsController : Controller
    {
        private SwapDealsDBEntities db = new SwapDealsDBEntities();

        public ActionResult Index()
        {
            return View();
        }

        // GET: Admins
        public ActionResult Details()
        {
            string adminEmail = Convert.ToString(Session["admin"]);
            var admin = db.Admins.Where(u => u.AdminEmail.Equals(adminEmail)).FirstOrDefault();
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(TempAdmin admin)
        {
            if (ModelState.IsValid)
            {
                var ad = db.Admins.Where(u => u.AdminEmail.Equals(admin.AdminEmail) && u.AdminPassword.Equals(admin.AdminPassword)).FirstOrDefault();
                if (ad != null)
                {
                    ViewBag.msg = "Log in successful";
                    Session["admin"] = ad.AdminEmail;
                    return RedirectToAction("Details");
                }
                else
                    ViewBag.msg = "Log in failed";
            }

            return View();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
