using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Data.Models;

namespace Data;

public partial class MovieReservationSystemContext : IdentityDbContext<IdentityUser>
{
    public MovieReservationSystemContext()
    {
    }

    public MovieReservationSystemContext(DbContextOptions<MovieReservationSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<SeatReservation> SeatReservations { get; set; }

    public virtual DbSet<ShowTime> ShowTimes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=HP;Database=MovieReservationSystem;Trusted_Connection=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Genres__3213E83F22D4C0C1");

            entity.HasIndex(e => e.Name, "UQ__Genres__72E12F1B694D79C7").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Movies__3213E83F38A8C3BC");

            entity.HasIndex(e => e.Title, "UQ__Movies__E52A1BB30B7CADF2").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.GenreId).HasColumnName("genre_id");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("title");

            entity.HasOne(d => d.Genre).WithMany(p => p.Movies)
                .HasForeignKey(d => d.GenreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Movies__genre_id__2D27B809");
        });

        modelBuilder.Entity<SeatReservation>(entity =>
        {
            entity.HasKey(e => new { e.Sector, e.Seat, e.ShowTimeId }).HasName("PK__SeatRese__BC3B8207A64E8167");

            entity.Property(e => e.Sector)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("sector");
            entity.Property(e => e.Seat)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("seat");
            entity.Property(e => e.ShowTimeId).HasColumnName("showTimeId");

            entity.HasOne(d => d.ShowTime).WithMany(p => p.SeatReservations)
                .HasForeignKey(d => d.ShowTimeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SeatReser__showT__38996AB5");
        });

        modelBuilder.Entity<ShowTime>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ShowTime__3213E83F05613AA1");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MovieId).HasColumnName("movie_id");
            entity.Property(e => e.ShowDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("showDate");

            entity.HasOne(d => d.Movie).WithMany(p => p.ShowTimes)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ShowTimes__movie__33D4B598");
        });

        //create sample data

        modelBuilder.Entity<Genre>()
            .HasData(
                new Genre() { Id = 1 ,Name = "Action" },
                new Genre() { Id = 2 ,Name = "Adventure" },
                new Genre() { Id = 3 ,Name = "Comedy" },
                new Genre() { Id = 4 ,Name = "Drama" },
                new Genre() { Id = 5 ,Name = "Horror" },
                new Genre() { Id = 6 ,Name = "Science Fiction" },
                new Genre() { Id = 7 ,Name = "Romance" },
                new Genre() { Id = 8 ,Name = "Thriller" },
                new Genre() { Id = 9 ,Name = "Fantasy" },
                new Genre() { Id = 10 ,Name = "Documentary" }
            );


        // creating roles

        var adminRoleId = Guid.NewGuid().ToString();
        var userRoleId = Guid.NewGuid().ToString();
        modelBuilder.Entity<IdentityRole>().HasData(
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
        modelBuilder.Entity<IdentityUser>().HasData(user1, user2);

        // set the roles to the users
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string> { UserId = adminId, RoleId = adminRoleId },
            new IdentityUserRole<string> { UserId = userId, RoleId = userRoleId }
        );

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
