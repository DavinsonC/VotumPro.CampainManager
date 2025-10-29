using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotumPro.CampainManeger.Domain.Models
{
    [Table("RolMenus")]
    public class RolMenu
    {
        [Key]
        public int IdRol { get; set; }
        public Rol Rol { get; set; }

        public int IdMenu { get; set; }
        public Menu Menu { get; set; }
    }
}
