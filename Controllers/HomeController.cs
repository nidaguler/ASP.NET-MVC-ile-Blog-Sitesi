using NeYemekYapsam.BusinessLayer;
using NeYemekYapsam.BusinessLayer.Results;
using NeYemekYapsam.Entities;
using NeYemekYapsam.Entities.Messages;
using NeYemekYapsam.Entities.ValueObjects;
using NeYemekYapsam.Filters;
using NeYemekYapsam.Models;
using NeYemekYapsam.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NeYemekYapsam.Controllers
{
    [Exc]
    public class HomeController : Controller
    {

        private NoteManager noteManager = new NoteManager();
        private CategoryManager categoryManager = new CategoryManager();
        private NeYemekYapsamUserManager userManager = new NeYemekYapsamUserManager();
        private LikedManager likedManager = new LikedManager();
        // GET: Home
        public ActionResult Index()
        {
   
            return View(noteManager.ListQueryable().Where(x=>x.IsDraft==false).OrderByDescending(x => x.ModifiedOn).ToList());
        }

        public ActionResult Search(string Ara=null)
        {

            return View(noteManager.ListQueryable().Where(x => x.Title.Contains(Ara)).OrderByDescending(x => x.ModifiedOn).ToList());
        }

        public ActionResult ByCategory(int? ID)
        {
            if (ID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Category cat = categoryManager.Find(x => x.ID == ID.Value);

            if (cat == null)
            {
                //return HttpNotFound();
                return RedirectToAction("Index", "Home");
            }


            return View("Index", cat.Notes.Where(x=>x.IsDraft==false).OrderByDescending(x => x.ModifiedOn).ToList());
        }

        public ActionResult MostLiked()
        {
            return View("Index", noteManager.ListQueryable().OrderByDescending(x => x.LikeCount).ToList());
        }

        public ActionResult About()
        {
            return View();
        }


        [Auth]
        public ActionResult ShowProfile(int? id)
        {
            if (id==null)
            {
                int idd = CurrentSession.User.ID;
                BusinessLayerResult<NeYemekYapsamUser> res = userManager.getUserById(idd);

                if (res.Errors.Count > 0)
                {
                    ErrorViewModel errorNotifyObj = new ErrorViewModel()
                    {
                        Title = "Hata Oluştu",
                        Items = res.Errors
                    }; 
                    return View("Error", errorNotifyObj);
                }

                return View(res.Result);

            }
            else
            {
                BusinessLayerResult<NeYemekYapsamUser> res = userManager.getUserById(id.Value);
                if (res.Errors.Count > 0)
                {
                    ErrorViewModel errorNotifyObj = new ErrorViewModel()
                    {
                        Title = "Hata Oluştu",
                        Items = res.Errors
                    };

                    return View("Error", errorNotifyObj);
                }
                  
               
                 return View(res.Result);
               
            }
        }


        [Auth]
        public ActionResult EditProfile()
        {
           
            BusinessLayerResult<NeYemekYapsamUser> res = userManager.getUserById(CurrentSession.User.ID);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Title = "Hata Oluştu.",
                    Items = res.Errors

                };

                return View("Error", errorNotifyObj);
            }

            return View(res.Result);
        }


        [Auth]
        [HttpPost]
        public ActionResult EditProfile(NeYemekYapsamUser model, HttpPostedFile ProfileImage)
        {
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                if (ProfileImage != null &&
                    (ProfileImage.ContentType == "image/jpeg" ||
                    ProfileImage.ContentType == "image/jpg" ||
                    ProfileImage.ContentType == "image/png"))
                {

                    string filename = $"user_{model.ID}.{ProfileImage.ContentType.Split('/')[1]}";
                    ProfileImage.SaveAs(Server.MapPath($"~/Images/{filename}"));
                    model.ProfileImageFilename = filename;
                }
 
                BusinessLayerResult<NeYemekYapsamUser> res = userManager.UpdateProfile(model);

                if (res.Errors.Count > 0)
                {
                    ErrorViewModel errorNotifyObj = new ErrorViewModel()
                    {
                        Items = res.Errors,
                        Title = "Profil Güncellenemedi.",
                        RedirectingUrl = "/Home/EditProfile"

                    };
                    return View("Error", errorNotifyObj);
                }

                CurrentSession.Set<NeYemekYapsamUser>("login",res.Result);

                return RedirectToAction("Index");
            }
            return View(model);

        }

        [Auth]
        public ActionResult DeleteProfile()
        {
           
            BusinessLayerResult<NeYemekYapsamUser> res = userManager.RemoveUserById(CurrentSession.User.ID);

            if (res.Errors.Count>0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Items = res.Errors,
                    Title ="Profil Silinemedi.",
                    RedirectingUrl="/Home/ShowProfile"
                };
                return View("Error", errorNotifyObj);
            }
            Session.Clear();
            return RedirectToAction("Index");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {

            if (ModelState.IsValid)
            { 
                BusinessLayerResult<NeYemekYapsamUser> res = userManager.LoginUser(model);

                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(model);
                }

                CurrentSession.Set<NeYemekYapsamUser>("login",res.Result);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            { 
                BusinessLayerResult<NeYemekYapsamUser> res = userManager.RegisterUser(model);

                if (res.Errors.Count > 0)
                {
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(model);
                }

                OkViewModel notifyObj = new OkViewModel()
                {
                    Title = "Kayıt Başarılı",
                    RedirectingUrl = "/Home/Login",
                    RedirectingTimeout = 3000
                };

                notifyObj.Items.Add("Lütfen e-posta adresinize gönderdiğimiz aktivasyon linkine tıklayarak hesabınızı aktive ediniz. Hesabınızı aktive etmeden not ekleyemez ve beğenme yapamazsınız.");

                return View("Ok", notifyObj);
            }
            return View(model);

        }

        public ActionResult UserActivate(Guid id)
        {
            BusinessLayerResult<NeYemekYapsamUser> res = userManager.ActivateUser(id);

            if (res.Errors.Count > 0)
            {
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Title = "Geçersiz İşlem",
                    Items = res.Errors

                };

                return View("Error", errorNotifyObj);
            }

            OkViewModel okNotifyObj = new OkViewModel()
            {
                Title = "Hesap Aktifleştirildi.",
                RedirectingUrl = "/Home/Login",
            };

            okNotifyObj.Items.Add("Hesabınız aktifleştirildi. Artık inceleme ekleyebilirsiniz ve eklenmiş incelemeleri beğenebilirsiniz.");


            return View("Ok", okNotifyObj);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult HasError()
        {
            return View();
        }
    }
}