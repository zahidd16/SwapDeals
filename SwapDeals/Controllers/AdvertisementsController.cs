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
            var advertisements = db.Advertisements.Include(a => a.Product).Include(a => a.User);
            return View(advertisements.ToList());
        }

        public ActionResult Details(int? id)
        {
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
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Advertisement ad)
        {
           
            if (ModelState.IsValid)
            {
                ad.UserID = Convert.ToInt32(Session["user_id"]);
                
                
               var pid = db.Products
                                   .SqlQuery("Select * from Products where ProductName = @id", new SqlParameter("@id",ad.SellingProduct))
                                    .FirstOrDefault();
                ad.ProductID = Convert.ToInt32(pid.ProductID);
              // ad.ProductID=
                ad.PriorityStatus = 0;
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
