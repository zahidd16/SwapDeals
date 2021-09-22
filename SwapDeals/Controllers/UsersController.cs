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
    public class UsersController : Controller
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

        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(User user)
        {
            if (ModelState.IsValid)
            {
                var checkUser = db.Users.Where(u => u.UserEmail.Equals(user.UserEmail)).FirstOrDefault();
                if (checkUser != null)
                {
                    ViewBag.errorMsg = "An account already exists with this email";
                }
                else
                {
                    user.Rating = 0;
                    try
                    {
                        db.Users.Add(user);
                        db.SaveChanges();
                        ViewBag.errorMsg = "Signup successful";
                        // return Content("Sign up successful");
                    }
                    catch (Exception e)
                    {
                        ViewBag.errorMsg = "e";

                    }
                }

            }
            return View();
        }
        public ActionResult Details()
        {
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
            HttpContext.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            HttpContext.Response.ExpiresAbsolute = DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0));
            HttpContext.Response.Expires = 0;
            HttpContext.Response.Cache.AppendCacheExtension("no-store, no-cache, must-revalidate, proxy-revalidate, post-check=0, pre-check=0");
            if (Session["user_id"] == null)
                return RedirectToAction("Index", "Home");
            int id = Convert.ToInt32(Session["user_id"]);
            var user = db.Users.Where(u => u.UserID == id).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        [HttpGet]
        public ActionResult Login()
        {
            if(Session["user_id"] !=null)
            return RedirectToAction("Index", "Home");
            return View();
        }
        [HttpPost]
        public ActionResult Login(TempUser tempUser)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Where(u => u.UserEmail.Equals(tempUser.UserEmail) && u.UserPassword.Equals(tempUser.UserPassword)).FirstOrDefault();
                if (user != null)
                {
                    ViewBag.msg = "Log in successful";
                    Session["user_id"] = user.UserID;
                    return RedirectToAction("Index", "Home");
                }
                else
                    ViewBag.msg = "Log in failed";
            }

            return View();
        }

        public ActionResult Logout()
        {

            Session.Abandon();
            //  Session.Clear();
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
            HttpContext.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            HttpContext.Response.ExpiresAbsolute = DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0));
            HttpContext.Response.Expires = 0;
            HttpContext.Response.Cache.AppendCacheExtension("no-store, no-cache, must-revalidate, proxy-revalidate, post-check=0, pre-check=0");

            return RedirectToAction("Login");
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
