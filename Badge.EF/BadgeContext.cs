using Badge.EF.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Badge.EF
{
    public class BadgeContext: DbContext
    {
        public DbSet<Machine> Machines { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Swipe> Swipe { get; set; }
        public DbSet<Entity.PopulateBadge> Badges { get; set; }

        public BadgeContext(DbContextOptions<BadgeContext> options)
            : base(options)
        {
            this.Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}


