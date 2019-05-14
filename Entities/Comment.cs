using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeYemekYapsam.Entities
{
    [Table("Comments")]
    public class Comment:NYYEntityBase
    {
        [Required, StringLength(300)]
        public string Text { get; set; }
        public virtual Note Note { get; set; }
        public virtual NeYemekYapsamUser Owner { get; set; }
    }
}
