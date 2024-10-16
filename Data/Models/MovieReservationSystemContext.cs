using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Data.Models;

public partial class MovieReservationSystemContext : DbContext
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
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=HP;Database=MovieReservationSystem;Trusted_Connection=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
