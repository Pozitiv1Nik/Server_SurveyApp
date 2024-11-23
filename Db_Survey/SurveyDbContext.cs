﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestEntitySurvey.Models;
using Db_Survey.Models;

namespace TestEntitySurvey
{
    public class SurveyDbContext: DbContext
    {
        public DbSet<Survey> Surveys { get; set; }

        public DbSet<Question> Question { get; set; }

        public DbSet<Answer>Answers { get; set; }

        public DbSet<Role>Roles { get; set; }

        public DbSet<User> Users { get; set; }


        public SurveyDbContext(DbContextOptions<SurveyDbContext> option):base(option)
        {
           Database.EnsureDeleted();

           Database.EnsureCreated();


            this.Roles.Add(new Role { UserType = Models.Roles.Customer });


        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(r => r.UserType)
                      .HasConversion(
                          v => v.ToString(),
                          v => (Roles)Enum.Parse(typeof(Roles), v)
                      )
                      .IsRequired();
            });


        }





    }
}
