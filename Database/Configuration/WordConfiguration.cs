using RojakJelah.Database.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace RojakJelah.Database.Configuration
{
    public class WordConfiguration : EntityTypeConfiguration<Word>
    {
        public WordConfiguration() : base()
        {
            // Table name
            ToTable("word");

            // Primary Key
            HasKey<int>(x => x.Id);

            // Column mappings
            Property(x => x.Id)
                .HasColumnName("word_id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            Property(x => x.WordValue)
                .HasColumnName("word")
                .HasMaxLength(50)
                .IsRequired();

            // Indexes
            HasIndex(x => x.WordValue)
                .IsUnique();

            // Foreign Keys
            HasRequired(x => x.Language)
                .WithMany()
                .Map(x => x.MapKey("language_id"));
        }
    }
}