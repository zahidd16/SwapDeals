﻿using System;
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
    public class BookingsController : Controller
    {
        private SwapDealsDBEntities db = new SwapDealsDBEntities();

        // GET: Bookings
        public ActionResult Index()
        {
            if (Session["admin"] == null)
                return RedirectToAction("Index","Home");
            var bookings = db.Bookings.Include(b => b.Advertisement).Include(b => b.User);
            return View(bookings.ToList());
        }
        
       

        // GET: Bookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // GET: Bookings/Create
        [HttpGet]
        public ActionResult Book(int? id)
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
            if (Session["user_id"]==null)
            {
                return RedirectToAction("Login","Users");
            }
            Session["adID"] = id;
            Advertisement advertisement = db.Advertisements.Find(id);
            if (advertisement == null)
            {
                return HttpNotFound();
            }
            ViewData["Ad"] = advertisement;
            return View();
           
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Book(Booking booking)
        {
            if (ModelState.IsValid)
            {
                booking.BookingStatus = 1;
                booking.UserID = Convert.ToInt32(Session["user_id"]);
                //booking.Date = DateTime.Today;
                booking.AdID = Convert.ToInt32(Session["adID"]);
                db.Bookings.Add(booking);
                db.SaveChanges();
                db.Database.ExecuteSqlCommand("Update Advertisements set PriorityStatus = -2 where AdID = " +
                    Convert.ToInt32(Session["adID"]));
                db.SaveChanges();
                
                return RedirectToAction("BookedAds","Bookings");
            }

            ViewBag.AdID = new SelectList(db.Advertisements, "AdID", "SellingProduct", booking.AdID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserName", booking.UserID);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdID = new SelectList(db.Advertisements, "AdID", "SellingProduct", booking.AdID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserName", booking.UserID);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookingID,BookingStatus,UserID,AdID,Location,Date")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdID = new SelectList(db.Advertisements, "AdID", "SellingProduct", booking.AdID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserName", booking.UserID);
            return View(booking);
        }
        public ActionResult BookedAds()
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
            int uid = Convert.ToInt32(Session["user_id"]);
            var b = db.Bookings.SqlQuery("Select * from Booking where UserID = "+uid).ToList<Booking>();

            return View("Index",b);
        }

        // GET: Bookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["admin"] == null && Session["user_id"] == null)
                return RedirectToAction("Index","Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = db.Bookings.Find(id);
            var ad = db.Advertisements.SqlQuery("Select *from Advertisements where AdID = "+booking.AdID).FirstOrDefault();
            int temp = (int)ad.Payment;
            int x;
            if (temp == 300)
                x = 1;
            else if (temp == 500)
                x = 2;
            else
                x = 0;
            db.Database.ExecuteSqlCommand("Update Advertisements set PriorityStatus = "+x+ " where AdID = " +
                   booking.AdID);
            db.SaveChanges();
            db.Bookings.Remove(booking);
            db.SaveChanges();
            if (Session["admin"] != null)
                return RedirectToAction("Index", "Bookings");
            else
                return RedirectToAction("BookedAds", "Bookings");

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