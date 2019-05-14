using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NeYemekYapsam.BusinessLayer;
using NeYemekYapsam.BusinessLayer.Results;
using NeYemekYapsam.Entities;
using NeYemekYapsam.Filters;
using NeYemekYapsam.Models;

namespace NeYemekYapsam.Controllers
{
    [Auth]
    [AuthAdmin]
    [Exc]
    public class KitapleaksUserController : Controller
    {
        private NeYemekYapsamUserManager neYemekYapsamUserManager = new NeYemekYapsamUserManager();


        public ActionResult Index()
        {
            return View(neYemekYapsamUserManager.List());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NeYemekYapsamUser neYemekYapsamUser = neYemekYapsamUserManager.Find(x=>x.ID==id.Value);
            if (neYemekYapsamUser == null)
            {
                return HttpNotFound();
            }
            return View(neYemekYapsamUser);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NeYemekYapsamUser neYemekYapsamUser)
        {
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                BusinessLayerResult<NeYemekYapsamUser> res = neYemekYapsamUserManager.Insert(neYemekYapsamUser);

                if (res.Errors.Count>0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(neYemekYapsamUser);
                }
                return RedirectToAction("Index");
            }

            return View(neYemekYapsamUser);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NeYemekYapsamUser neYemekYapsamUser = neYemekYapsamUserManager.Find(x => x.ID == id.Value); 
            if (neYemekYapsamUser == null)
            {
                return HttpNotFound();
            }
            return View(neYemekYapsamUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NeYemekYapsamUser neYemekYapsamUser)
        {
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                BusinessLayerResult<NeYemekYapsamUser> res = neYemekYapsamUserManager.Update(neYemekYapsamUser);

                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(neYemekYapsamUser);
                }
              
                return RedirectToAction("Index");
            }
            return View(neYemekYapsamUser);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NeYemekYapsamUser neYemekYapsamUser = neYemekYapsamUserManager.Find(x => x.ID == id.Value);
            if (neYemekYapsamUser == null)
            {
                return HttpNotFound();
            }
            return View(neYemekYapsamUser);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NeYemekYapsamUser neYemekYapsamUser = neYemekYapsamUserManager.Find(x => x.ID == id);
            neYemekYapsamUserManager.Delete(neYemekYapsamUser);
            return RedirectToAction("Index");
        }

    }
}
