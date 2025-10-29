using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using VotumPro.CampainManeger.Domain.Models;

namespace VotumPro.CampainManeger.Infraestructure.Context
{
    public class VotumProDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<RolMenu> RolMenus { get; set; }

        public VotumProDbContext(DbContextOptions<VotumProDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().HasKey(u => u.IdUsuario);

            modelBuilder.Entity<RolMenu>()
                .HasKey(rm => new { rm.IdRol, rm.IdMenu });

            modelBuilder.Entity<RolMenu>()
                .HasOne(rm => rm.Rol)
                .WithMany(r => r.RolMenus)
                .HasForeignKey(rm => rm.IdRol);

            modelBuilder.Entity<RolMenu>()
                .HasOne(rm => rm.Menu)
                .WithMany(m => m.RolMenus)
                .HasForeignKey(rm => rm.IdMenu);

            // Relación Menú Padre - Submenús
            modelBuilder.Entity<Menu>()
                .HasOne(m => m.MenuPadre)
                .WithMany(m => m.SubMenus)
                .HasForeignKey(m => m.IdMenuPadre)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
