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
            :base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           // modelBuilder.Entity<Friendship>().HasOne(e => e.User1).WithMany<Friendship>().HasForeignKey<Friendship>(e => e.ActionUserId);


            modelBuilder.Entity<Friendship>()
                .HasKey(t => new { t.UserId, t.FriendId });

            modelBuilder.Entity<Friendship>()
                .HasOne(pt => pt.User)
                .WithMany(p => p.UserFriends)
                .HasForeignKey(pt => pt.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasOne(pt => pt.Friend)
                .WithMany(t => t.Friends)
                .HasForeignKey(pt => pt.FriendId);
        }
    }
}
