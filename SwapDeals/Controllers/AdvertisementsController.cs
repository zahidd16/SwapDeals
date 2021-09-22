using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SwapDeals.Models;

namespace SwapDeals.Controllers
{
    public class AdvertisementsController : Controller
    {
        private SwapDealsDBEntities db = new SwapDealsDBEntities();

        public ActionResult Index()
        {

            if(Session["admin"]==null)
                return RedirectToAction("Index", "Home");
            var advertisements = db.Advertisements.Include(a => a.Product).Include(a => a.User);
            return View(advertisements.ToList());
        }
       
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
            if (Session["user_id"] == null)
            {
                if(Session["admin"] == null)
                  return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Advertisement advertisement = db.Advertisements.Find(id);
            if (advertisement == null)
            {
                return HttpNotFound();
            }
            return View(advertisement);
        }

       
        [HttpGet]
        public ActionResult Create()
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Advertisement ad)
        {
            if (Session["user_id"] == null)
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                ad.UserID = Convert.ToInt32(Session["user_id"]);
                
                
               var pid = db.Products
                                   .SqlQuery("Select * from Products where ProductName = @id", new SqlParameter("@id",ad.SellingProduct))
                                    .FirstOrDefault();
                if(pid==null)
                {
                    Product P = new Product();
                    P.ProductName =ad.SellingProduct;
                    P.ProductDetails = ad.ProductDescription;
                    P.ProductPrice = 0;
                    P.ProductBrand = "";
                    P.ProductCategory = "";
                    try
                    {
                        db.Products.Add(P);
                        db.SaveChanges();
                        pid = db.Products
                                   .SqlQuery("Select * from Products where ProductName = @id", new SqlParameter("@id", ad.SellingProduct))
                                    .FirstOrDefault();

                    }
                    catch(Exception e)
                    {
                        return Content(e.ToString());
                    }

                }
                ad.ProductID = Convert.ToInt32(pid.ProductID);
              // ad.ProductID=
                ad.PriorityStatus = -1;
                ad.Payment = 0;
                string fileName = Path.GetFileNameWithoutExtension(ad.ImageFile.FileName);
                string extension = Path.GetExtension(ad.ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                ad.Images = "~/Content/images/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/Content/images/"), fileName);
                ad.ImageFile.SaveAs(fileName);
               
                    try
                    {
                        db.Advertisements.Add(ad);
                        db.SaveChanges();
                        return Content("Ad posted successfully");
                    }
                    catch (Exception e)
                    {
                        return Content("Something went wrong");
                    }
              
            }
            return Content("Try again");


        }
        
        [HttpGet]
        public ActionResult Ads()
        {
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
            HttpContext.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            HttpContext.Response.ExpiresAbsolute = DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0));
            HttpContext.Response.Expires = 0;
            HttpContext.Response.Cache.AppendCacheExtension("no-store, no-cache, must-revalidate, proxy-revalidate, post-check=0, pre-check=0");
            using (db)
            {
               
                var ads = db.Advertisements.SqlQuery("Select *from Advertisements where PriorityStatus>-1")
                      .ToList<Advertisement>();
              
                ViewData["Ads"] = ads;
                return View();
            }
            
        }
        [HttpPost]

        public ActionResult Ads(TempAds ta)
        {
            if(ModelState.IsValid)
            {
                var ads = db.Advertisements.SqlQuery("Select *from Advertisements where TargatedProduct like '%"
                    +ta.SellingProduct+"%' and SellingProduct like '%"+ta.TargatedProduct+"%'")
                      .ToList<Advertisement>();
                if(ads==null)
                {
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    ViewData["Ads"] = ads;
                    return View();
                }

            }
             
             return View();
        }
        public ActionResult PostedAds()
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
            var a = db.Advertisements.SqlQuery("Select * from Advertisements where UserID = " + Convert.ToInt32(Session["user_id"])).ToList<Advertisement>();
            return View("Index", a);
        }
        public ActionResult Edit(int? id)
        {
            if (Session["admin"] == null)
                return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Advertisement advertisement = db.Advertisements.Find(id);
            if (advertisement == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", advertisement.ProductID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserName", advertisement.UserID);
            return View(advertisement);
        }
        // POST: Advertisements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AdID,ProductID,UserID,SellingProduct,TargatedProduct,Date,AdjustedValue,Images,ProductDescription,PriorityStatus,Payment,Warranty")] Advertisement advertisement)
        {
            if (Session["admin"] == null)
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                db.Entry(advertisement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", advertisement.ProductID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserName", advertisement.UserID);
            return View(advertisement);
        }

        // GET: Advertisements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["admin"] == null)
                return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Advertisement advertisement = db.Advertisements.Find(id);
            if (advertisement == null)
            {
                return HttpNotFound();
            }
            return View(advertisement);
        }

        // POST: Advertisements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["admin"] == null)
                return RedirectToAction("Index", "Home");
            Advertisement advertisement = db.Advertisements.Find(id);
            db.Advertisements.Remove(advertisement);
            db.SaveChanges();
            return RedirectToAction("Index");
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
