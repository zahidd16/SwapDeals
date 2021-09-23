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
    public class DealsController : Controller
    {
        private SwapDealsDBEntities db = new SwapDealsDBEntities();

        // GET: Deals
        public ActionResult Index()
        {
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
            HttpContext.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            HttpContext.Response.ExpiresAbsolute = DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0));
            HttpContext.Response.Expires = 0;
            HttpContext.Response.Cache.AppendCacheExtension("no-store, no-cache, must-revalidate, proxy-revalidate, post-check=0, pre-check=0");
            if (Session["admin"] == null)
                return RedirectToAction("Index","Home");
            var deals = db.Deals.Include(d => d.Booking).Include(d => d.User).Include(d => d.User1);
            return View(deals.ToList());
        }

        // GET: Deals/Details/5
        public ActionResult Details(int? id)
        {
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
            HttpContext.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            HttpContext.Response.ExpiresAbsolute = DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0));
            HttpContext.Response.Expires = 0;
            HttpContext.Response.Cache.AppendCacheExtension("no-store, no-cache, must-revalidate, proxy-revalidate, post-check=0, pre-check=0");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deal deal = db.Deals.Find(id);
            if (deal == null)
            {
                return HttpNotFound();
            }
            return View(deal);
        }

      
        [HttpGet]
        public ActionResult FinishPostedDeal(int? id)
        {
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
            HttpContext.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            HttpContext.Response.ExpiresAbsolute = DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0));
            HttpContext.Response.Expires = 0;
            HttpContext.Response.Cache.AppendCacheExtension("no-store, no-cache, must-revalidate, proxy-revalidate, post-check=0, pre-check=0");
            if (id!=null)
            Session["AdID"] = id;
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinishPostedDeal(Deal deal)
        {
            try
            {
                Booking b = db.Bookings.SqlQuery("Select * from Booking where AdID = " + Convert.ToInt32(Session["AdID"])).FirstOrDefault();
                if(b==null)
                    return RedirectToAction("Index");
                deal.BookingID = b.BookingID;
                deal.UserID1 = Convert.ToInt32(Session["user_id"]);
                deal.UserID2 = b.UserID;
                deal.User1Rating = 0;
                Advertisement a = db.Advertisements.SqlQuery("Select * from Advertisements where AdID = " + Convert.ToInt32(Session["AdID"])).FirstOrDefault();
                deal.Revenue = (int)a.Payment;
                db.Deals.Add(deal);
                db.SaveChanges();
                return RedirectToAction("Index");
            }catch(Exception e)
            {
                return Content(e.ToString());
            }

        }
        [HttpGet]
        public ActionResult FinishBookedDeal(int? id)
        {
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
            HttpContext.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            HttpContext.Response.ExpiresAbsolute = DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0));
            HttpContext.Response.Expires = 0;
            HttpContext.Response.Cache.AppendCacheExtension("no-store, no-cache, must-revalidate, proxy-revalidate, post-check=0, pre-check=0");
            if (id != null)
                Session["bookingID"] = id;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinishBookedDeal(Deal deal)
        {
            try
            {
                
                Deal d = db.Deals.SqlQuery("Select * from Deals where BookingID = " + Convert.ToInt32(Session["bookingID"])).FirstOrDefault();
                
                if (d != null)
                {
                    db.Database.ExecuteSqlCommand("Update Deals set User1Rating = " + deal.User1Rating + " where DealID = " + d.DealID);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return Content("Wait for Ad poster to finalize");
                }
                
            }
            catch (Exception e)
            {
                return Content(e.ToString());
            }

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
