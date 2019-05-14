using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NeYemekYapsam.BusinessLayer;
using NeYemekYapsam.Entities;
using NeYemekYapsam.Filters;
using NeYemekYapsam.Models;

namespace NeYemekYapsam.Controllers
{
    [Exc]
    public class NoteController : Controller
    {
        private NoteManager noteManager = new NoteManager();
        private CategoryManager categoryManager = new CategoryManager();
        private LikedManager likedManager = new LikedManager();

        [Auth]
        public ActionResult Index()
        {
            var notes = noteManager.ListQueryable().Include("Category").Include("Owner").Where(
                x => x.Owner.ID == CurrentSession.User.ID).OrderByDescending(
                x => x.ModifiedOn);
            return View(notes.ToList());
        }

        [Auth]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = noteManager.Find(x => x.ID == id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        [Auth]
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(CacheHelper.GetCategoriesFromCache(), "ID", "Title");
            return View();
        }

        [Auth]
        public ActionResult MyLikedNotes()
        {
            var notes = likedManager.ListQueryable().Include("LikedUser").Include("Note").Where(
                x => x.LikedUser.ID == CurrentSession.User.ID).Select(
                x => x.Note).Include("Category").Include("Owner").OrderByDescending(
                x => x.ModifiedOn);

            return View("Index", notes.ToList());
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Note note)
        {
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");


            if (ModelState.IsValid)
            {
                note.Owner = CurrentSession.User;
                noteManager.Insert(note);
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(CacheHelper.GetCategoriesFromCache(), "ID", "Title", note.CategoryID);
            return View(note);
        }

        [Auth]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = noteManager.Find(x => x.ID == id);
            if (note == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(CacheHelper.GetCategoriesFromCache(), "ID", "Title", note.CategoryID);
            return View(note);
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Note note)
        {

            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                Note db_note = noteManager.Find(x => x.ID == note.ID);
                db_note.IsDraft = note.IsDraft;
                db_note.CategoryID = note.CategoryID;
                db_note.Text = note.Text;
                db_note.Title = note.Title;

                noteManager.Update(db_note);

                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(CacheHelper.GetCategoriesFromCache(), "ID", "Title", note.CategoryID);
            return View(note);
        }

        [Auth]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = noteManager.Find(x => x.ID == id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        [Auth]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Note note = noteManager.Find(x => x.ID == id);
            noteManager.Delete(note);
            return RedirectToAction("Index");
        }

       
        [HttpPost]
        public ActionResult GetLiked(int[] ids)
        {
            if (CurrentSession.User != null)
            {
                List<int> likedNoteIds = likedManager.List(x => x.LikedUser.ID == CurrentSession.User.ID && ids.Contains(x.Note.ID)).Select(
                    x => x.Note.ID).ToList();
                return Json(new { result = likedNoteIds });
            }
            else
            {
                return Json(new { result = new List<int>() });
            }
        }

        
        [HttpPost]
        public ActionResult SetLikeState(int noteid,bool liked)
        {
            int res = 0;
            Liked like = likedManager.Find(x => x.Note.ID == noteid && x.LikedUser.ID == CurrentSession.User.ID);
            Note note = noteManager.Find(x => x.ID == noteid);

            if (like != null && liked == false)
            {
                res = likedManager.Delete(like);
            }
            else if (like == null && liked == true)
            {
                res = likedManager.Insert(new Liked()
                {
                    LikedUser = CurrentSession.User,
                    Note = note

                });

            }

            if (res>0)
            {
                if (liked)
                {
                    note.LikeCount++;
                }
                else
                {
                    note.LikeCount--;
                }

              res =  noteManager.Update(note);
                return Json(new
                {
                    hasError = false,
                    errorMessage= string.Empty,
                    result =
                note.LikeCount
                });
            }

            return Json(new
            {
                hasError = true,
                errorMessage = "Beğenme işlemi gerçekleştirilemedi.",
                result =
                note.LikeCount
            });
        }


        public ActionResult GetNoteText(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = noteManager.Find(x => x.ID == id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return PartialView("_PartialNoteText", note);
        }
    }

}