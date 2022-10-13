using RojakJelah.Database.Models.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RojakJelah.Database
{
    [DbConfigurationType(typeof(MySql.Data.EntityFramework.MySqlEFConfiguration))]
    public class DataContext : DbContext
    {
        public DataContext(string connectionString) : base(connectionString)
        {

        }

        // Mapping
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Language
            // Table name
            modelBuilder.Entity<Language>().ToTable("languages");
            // Columns
            modelBuilder.Entity<Language>().Property(x => x.Id).HasColumnName("language_id");
            modelBuilder.Entity<Language>().Property(x => x.Name).HasColumnName("name");

            // Word
            // Table Name
            modelBuilder.Entity<Word>().ToTable("word");
            // Columns
            modelBuilder.Entity<Word>().Property(x => x.Id).HasColumnName("word_id");
            modelBuilder.Entity<Word>().Property(x => x.WordValue).HasColumnName("word");
            // Relationships
            modelBuilder.Entity<Word>().HasRequired(x => x.Language);
        }

        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<Word> Words { get; set; }
    }
}