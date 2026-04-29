using _1292886_MVC_CRUD_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace _1292886_MVC_CRUD_Project.DAL
{
   
        public class AppDbContext : DbContext
        {
            public AppDbContext() : base("name=AppDbContext")
            {
            }

            public DbSet<TblUser> TblUsers { get; set; }
            public DbSet<TblRole> TblRoles { get; set; }

            public DbSet<Sale> Sales { get; set; }
            public DbSet<Property> Properties { get; set; }
            public DbSet<PaymentMethod> PaymentMethods { get; set; }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

  
                modelBuilder.Entity<Sale>()
                    .Property(x => x.TotalPrice)
                    .HasPrecision(18, 2);

                modelBuilder.Entity<Sale>()
                    .HasRequired(s => s.PaymentMethod)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(s => s.PaymentTypeId)
                    .WillCascadeOnDelete(false);

               modelBuilder.Entity<Property>()
                    .HasRequired(p => p.Sale)
                    .WithMany(s => s.Properties)
                    .HasForeignKey(p => p.SalesId)
                    .WillCascadeOnDelete(true);

                modelBuilder.Entity<TblUser>()
                    .HasRequired(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId)
                    .WillCascadeOnDelete(false);
            }
        }
}