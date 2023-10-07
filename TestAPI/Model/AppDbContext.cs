using Microsoft.EntityFrameworkCore;
using TestAPI.Model.Entities;
using TestAPI.Model.Enums;

namespace TestAPI.Model
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User[]
                {
                    new User {Id=1, Name= "Egor", Age = 20, Email="egor.ivanovBYM@gmail.com" },
                    new User {Id=2, Name="Artem", Age = 19, Email="artem.kolyago@gmail.com"},
                    new User {Id=3, Name="Daniil", Age = 16, Email="daniil.petrov@gmail.com"},
                    new User {Id=4, Name="Arsen", Age = 25, Email="arsen.petrov@gmail.com"},
                    new User {Id=5, Name="Vova", Age = 41, Email="vova.petrov@gmail.com"},
                    
                });
            modelBuilder.Entity<Role>().HasData(
                new Role[]
                {
                    new Role {Id=1, Name=RoleName.SuperAdmin, UserId=1},
                    new Role {Id=2, Name=RoleName.Support, UserId=1},
                    new Role {Id=3, Name=RoleName.User, UserId=1},
                    new Role {Id=4, Name=RoleName.User, UserId=2},
                    new Role {Id=5, Name=RoleName.Admin, UserId=3},
                    new Role {Id=6, Name=RoleName.SuperAdmin, UserId=4},
                    new Role {Id=7, Name=RoleName.User, UserId=4}
                });
        }
    }
}
