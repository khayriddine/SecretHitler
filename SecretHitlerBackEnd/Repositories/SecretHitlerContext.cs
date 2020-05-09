using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Repositories
{
    public class SecretHitlerContext : DbContext
    {
        public SecretHitlerContext(DbContextOptions<SecretHitlerContext> options)
            : base(options)
        {
            //Database.EnsureCreated();
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Room> Rooms { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Friendship>()
                .HasOne<User>(fs => fs.User)
                .WithMany(u => u.Friendships)
                .HasForeignKey(fs => fs.UserId);

            modelBuilder.Entity<User>()
                .HasOne<Room>(u => u.Room)
                .WithMany(r => r.UsersJoining);
        }
    }
}
