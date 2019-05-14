using MyEvernote.Common.Helpers;
using NeYemekYapsam.BusinessLayer.Abstract;
using NeYemekYapsam.BusinessLayer.Results;
using NeYemekYapsam.Common.Helpers;
using NeYemekYapsam.DataAccessLayer.EntityFramework;
using NeYemekYapsam.Entities;
using NeYemekYapsam.Entities.Messages;
using NeYemekYapsam.Entities.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeYemekYapsam.BusinessLayer
{
    public class NeYemekYapsamUserManager: ManagerBase<NeYemekYapsamUser>
    {

        public BusinessLayerResult<NeYemekYapsamUser> RegisterUser(RegisterViewModel data)
        {

            NeYemekYapsamUser user =Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<NeYemekYapsamUser> res = new BusinessLayerResult<NeYemekYapsamUser>();

            if (user != null)
            {
                if (user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExist, "Kullanıcı adı kayıtlı");
                }

                if (user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExist, "Email kayıtlı");
                }

            }
            else
            {
                int dbResult =base.Insert(new NeYemekYapsamUser()
                {
                    Username = data.Username,
                    Email = data.Email,
                    Password = data.Password,
                    ProfileImageFilename = "user.png",
                    ActivateGuid = Guid.NewGuid(),
                    IsActive = false,
                    IsAdmin = false

                });

                if (dbResult > 0)
                {
                    res.Result = Find(x => x.Email == data.Email && x.Username == data.Username);

                    string SiteUri = ConfigHelper.Get<string>("SiteRootUri");
                    string ActivateUri = $"{SiteUri}/Home/UserActivate/{res.Result.ActivateGuid}";
                    string body = $"Merhaba {res.Result.Username};<br><br> Hesabınızı aktifleştirmek için <a href='{ActivateUri}' target='_blank'>tıklayınız.</a>";
                    MailHelper.SendMail(body, res.Result.Email, "Kitapleaks Hesap Aktifleştirme");
                }
            }
            return res;
        }

        public BusinessLayerResult<NeYemekYapsamUser> getUserById(int id)
        {
            BusinessLayerResult<NeYemekYapsamUser> res = new BusinessLayerResult<NeYemekYapsamUser>();
            res.Result = Find(x => x.ID == id);

            if (res.Result == null)
            {
                res.AddError(ErrorMessageCode.UserNotFound, "Kullanıcı bulunamadı");
            }
            return res;
        }

        public BusinessLayerResult<NeYemekYapsamUser> LoginUser(LoginViewModel data)
        {

            BusinessLayerResult<NeYemekYapsamUser> res = new BusinessLayerResult<NeYemekYapsamUser>();
            res.Result = Find(x => x.Username == data.Username && x.Password == data.Password);

            if (res.Result != null)
            {
                if (!res.Result.IsActive)
                {
                    res.AddError(ErrorMessageCode.UserIsNotActive, "Kullanıcı aktifleştirilmemiştir.");
                    res.AddError(ErrorMessageCode.CheckYourEmail, "Lütfen e-posta adresinizi kontrol ediniz.");
                }
            }
            else
            {
                res.AddError(ErrorMessageCode.UsernameOrPassWrong, "Kullanıcı adı yada şifre uyuşmuyor.");
            }

            return res;
        }

        public BusinessLayerResult<NeYemekYapsamUser> UpdateProfile(NeYemekYapsamUser data)
        {
            NeYemekYapsamUser db_user = Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<NeYemekYapsamUser> res = new BusinessLayerResult<NeYemekYapsamUser>();
            res.Result = data;

            if (db_user != null && db_user.ID != data.ID)
            {
                if (db_user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExist, "Kullanıcı adı kayıtlı.");
                }

                if (db_user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExist, "E-posta adresi kayıtlı.");
                }

                return res;
            }

            res.Result = Find(x => x.ID == data.ID);
            res.Result.Email = data.Email;
            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Password = data.Password;
            res.Result.Username = data.Username;
            res.Result.IsActive = data.IsActive;
            res.Result.IsAdmin = data.IsAdmin;

            if (base.Update(res.Result) == 0)
            {
                res.AddError(ErrorMessageCode.UserCouldNotUpdated, "Kullanıcı güncellenemedi.");
            }

            return res;
        }

        public BusinessLayerResult<NeYemekYapsamUser> ActivateUser(Guid ActivateID)
        {
            BusinessLayerResult<NeYemekYapsamUser> res = new BusinessLayerResult<NeYemekYapsamUser>();
            res.Result = Find(x => x.ActivateGuid == ActivateID);

            if (res.Result != null)
            {
                if (res.Result.IsActive)
                {
                    res.AddError(ErrorMessageCode.UserAlreadyActive, "Kullanıcı zaten aktif edilmiştir.");
                    return res;
                }

                res.Result.IsActive = true;
                Update(res.Result);
            }
            else
            {
                res.AddError(ErrorMessageCode.ActivateIDDoesNotExist, "Aktifleştirilecek kullanıcı bulunamadı.");
            }

            return res;
        }

        public BusinessLayerResult<NeYemekYapsamUser> RemoveUserById(int id)
        {
            BusinessLayerResult<NeYemekYapsamUser> res = new BusinessLayerResult<NeYemekYapsamUser>();
            NeYemekYapsamUser user = Find(x => x.ID == id);

            if (user!=null)
            {
                if (Delete(user)==0)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotRemove, "Kullanıcı silinemedi.");
                    return res;
                }
            }
            else
            {
                res.AddError(ErrorMessageCode.UserCouldNotFound, "Kullanıcı bulunamadı.");
            }
            return res;
        }

        public new BusinessLayerResult<NeYemekYapsamUser> Insert(NeYemekYapsamUser data)
        {
            NeYemekYapsamUser user = Find(x => x.Username == data.Username || x.Email == data.Email);
            BusinessLayerResult<NeYemekYapsamUser> res = new BusinessLayerResult<NeYemekYapsamUser>();

            res.Result = data;
            if (user != null)
            {
                if (user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExist, "Kullanıcı adı kayıtlı");
                }

                if (user.Email == data.Email)
                {
                   
                }

            }
            else
            {
                res.Result.ProfileImageFilename = "user.png";
                res.Result.ActivateGuid = Guid.NewGuid();

                if (base.Insert(res.Result)==0)
                {
                    res.AddError(ErrorMessageCode.UserCouldNotInserted, "Kullanıcı eklenemedi.");
                };

            }
            return res;
        }

        public new BusinessLayerResult<NeYemekYapsamUser> Update(NeYemekYapsamUser data)
        {
            NeYemekYapsamUser db_user = Find(x => x.ID != data.ID && (x.Username == data.Username || x.Email == data.Email));

            BusinessLayerResult<NeYemekYapsamUser> res = new BusinessLayerResult<NeYemekYapsamUser>();
            res.Result = data;

            if (db_user != null && db_user.ID != data.ID)
            {
                if (db_user.Username == data.Username)
                {
                    res.AddError(ErrorMessageCode.UsernameAlreadyExist, "Kullanıcı adı kayıtlı.");
                }

                if (db_user.Email == data.Email)
                {
                    res.AddError(ErrorMessageCode.EmailAlreadyExist, "E-posta adresi kayıtlı.");
                }

                return res;
            }


            res.Result = Find(x => x.ID == data.ID);
            res.Result.Email = data.Email;
            res.Result.Name = data.Name;
            res.Result.Surname = data.Surname;
            res.Result.Password = data.Password;
            res.Result.Username = data.Username;
            res.Result.IsActive = data.IsActive;
            res.Result.IsAdmin = data.IsAdmin;

            if (base.Update(res.Result) == 0)
            {
                res.AddError(ErrorMessageCode.UserCouldNotUpdated, "Kullanıcı Güncellenemedi.");
            }
            return res;
        }
    }
}
