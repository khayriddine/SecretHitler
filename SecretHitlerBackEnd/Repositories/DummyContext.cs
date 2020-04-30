using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Repositories
{
    public class DummyContext : DbContext
    {
        public DummyContext(DbContextOptions<DummyContext> options): base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Dummy> Dummies { get; set; }
        public DbSet<Bag> Bags { get; set; }
    }
}
