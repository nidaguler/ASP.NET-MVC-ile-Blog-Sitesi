using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeYemekYapsam.Entities
{
    [Table("NeYemekYapsamUsers")]
    public class NeYemekYapsamUser : NYYEntityBase
    {
        [DisplayName("Ad: "),
            StringLength(25, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Name { get; set; }

        [DisplayName("Soyad: "),
            StringLength(25, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Surname { get; set; }

        [DisplayName("Kullanıcı Adı: "),
            Required(ErrorMessage = "{0} alanı gereklidir."),
            StringLength(25, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Username { get; set; }

        [DisplayName("Email: "),
            Required(ErrorMessage = "{0} alanı gereklidir."),
            StringLength(255, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Email { get; set; }

        [DisplayName("Hakkımda:"),
            StringLength(255, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Hakkımda { get; set; }

        [DisplayName("Yetenekler:"),
           StringLength(300, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Yetenekler { get; set; }

        //[DisplayName("Takipçiler:")]
        //public int Followers { get; set; }

        //[DisplayName("Takip Edilenler:")]
        //public int Followings { get; set; }


        [DisplayName("Meslek:"),
            StringLength(255, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Meslek { get; set; }


        [DisplayName("Twitter:"),
             StringLength(255, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Twitter { get; set; }

        [DisplayName("Facebook:"),
             StringLength(255, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Facebook { get; set; }

        [DisplayName("Instagram:"),
            StringLength(255, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Instagram { get; set; }




        [DisplayName("Şifre: "),
            Required(ErrorMessage ="{0} alanı gereklidir."),
            StringLength(100,ErrorMessage ="{0} alanı max. {1} karakter olmalıdır.")]
        public string Password { get; set; }

        [StringLength(30,ErrorMessage ="{0} alanı max. {1} karakter olmalıdır."),
            ScaffoldColumn(false)]
        public string ProfileImageFilename { get; set; }

        [DisplayName("Aktif")]
        public bool IsActive { get; set; }

        [DisplayName("Admin")]
        public bool IsAdmin { get; set; }

        [Required,
            ScaffoldColumn(false)]
        public Guid ActivateGuid { get; set; }
        
        public virtual List<Note> Notes { get; set; }
        public virtual List<Comment> Comments { get; set; }
        public virtual List<Liked> Likes { get; set; }

    }
}
