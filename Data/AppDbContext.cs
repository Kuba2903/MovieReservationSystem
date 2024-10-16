using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlServer("Data Source=HP;Initial Catalog=MovieReservationSystem;Integrated Security=True;Trust Server Certificate=True");

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            // creating roles

            var adminRoleId = Guid.NewGuid().ToString();
            var userRoleId = Guid.NewGuid().ToString();
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = userRoleId, Name = "User", NormalizedName = "USER" }
            );

            // creating users
            var hasher = new PasswordHasher<IdentityUser>();


            var adminId = Guid.NewGuid().ToString();
            var user1 = new IdentityUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                EmailConfirmed = true,
                NormalizedUserName = "ADMIN@EXAMPLE.COM",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                Id = adminId
            };
            user1.PasswordHash = hasher.HashPassword(user1, "Password123!");


            var userId = Guid.NewGuid().ToString();
            var user2 = new IdentityUser
            {
                UserName = "user@example.com",
                Email = "user@example.com",
                EmailConfirmed = true,
                NormalizedUserName = "USER@EXAMPLE.COM",
                NormalizedEmail = "USER@EXAMPLE.COM",
                Id = userId
            };
            user2.PasswordHash = hasher.HashPassword(user2, "Password123!");

            // add users for the data
            builder.Entity<IdentityUser>().HasData(user1, user2);

            // set the roles to the users
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = adminId, RoleId = adminRoleId },
                new IdentityUserRole<string> { UserId = userId, RoleId = userRoleId } 
            );
        }
    }
}
