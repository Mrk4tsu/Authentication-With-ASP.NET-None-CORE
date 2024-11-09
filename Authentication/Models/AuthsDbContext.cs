using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Authentication.Models
{
    public partial class AuthsDbContext : DbContext
    {
        public AuthsDbContext()
            : base("name=AuthsDbContext")
        {
        }

        public virtual DbSet<DeviceVerificationToken> DeviceVerificationToken { get; set; }
        public virtual DbSet<LoginLog> LoginLog { get; set; }
        public virtual DbSet<UserDevice> UserDevice { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DeviceVerificationToken>()
                .Property(e => e.Token)
                .IsUnicode(false);

            modelBuilder.Entity<LoginLog>()
                .Property(e => e.IpAddress)
                .IsUnicode(false);

            modelBuilder.Entity<UserDevice>()
                .Property(e => e.IpAddress)
                .IsUnicode(false);

            modelBuilder.Entity<UserDevice>()
                .HasMany(e => e.LoginLog)
                .WithRequired(e => e.UserDevice)
                .HasForeignKey(e => e.DeviceId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.Username)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.UserCODE)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.DeviceVerificationToken)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.LoginLog)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.UserDevice)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.UserId);
        }
    }
}
