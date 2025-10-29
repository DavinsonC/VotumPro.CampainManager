using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotumPro.CampainManeger.Domain.Models
{
    [Table("Menus")]
    public class Menu
    {
        [Key]
        public int IdMenu { get; set; }
        public string Nombre { get; set; }
        public string Ruta { get; set; }
        public string Icono { get; set; }
        public bool EsActivo { get; set; }

        public int? IdMenuPadre { get; set; }
        public Menu MenuPadre { get; set; }
        public ICollection<Menu> SubMenus { get; set; }

        public ICollection<RolMenu> RolMenus { get; set; }
    }
}
