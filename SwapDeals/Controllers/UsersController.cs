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
