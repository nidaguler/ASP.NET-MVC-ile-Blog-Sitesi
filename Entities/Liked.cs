using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeYemekYapsam.Entities
{
    [Table("Likes")]
   public class Liked
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public virtual Note Note { get; set; }
        public virtual NeYemekYapsamUser LikedUser { get; set; }
    }
}
