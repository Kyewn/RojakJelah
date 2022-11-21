using RojakJelah.Database.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace RojakJelah.Database.Configuration
{
    public class SuggestionConfiguration : EntityTypeConfiguration<Suggestion>
    {
        public SuggestionConfiguration() : base()
        {
            // Table name
            ToTable("suggestion");

            // Primary Key
            HasKey<int>(x => x.Id);

            // Column mappings
            Property(x => x.Id)
                .HasColumnName("suggestion_id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            Property(x => x.Slang)
                .HasColumnName("slang")
                .HasMaxLength(50)
                .IsRequired();

            Property(x => x.Translation)
                .HasColumnName("translation")
                .HasMaxLength(50)
                .IsRequired();

            Property(x => x.Example)
                .HasColumnName("example")
                .HasMaxLength(100)
                .IsOptional();

            Property(x => x.CreationDate)
                .HasColumnName("creation_date")
                .IsRequired();

            Property(x => x.ModificationDate)
                .HasColumnName("modification_date")
                .IsRequired();

            // Foreign Keys
            HasRequired(x => x.Language)
                .WithMany()
                .Map(x => x.MapKey("slang_language_id"));

            HasRequired(x => x.SuggestionStatus)
                .WithMany()
                .Map(x => x.MapKey("suggestion_status_id"));

            HasOptional(x => x.CreatedBy)
                .WithMany()
                .Map(x => x.MapKey("created_by"));

            HasOptional(x => x.ModifiedBy)
                .WithMany()
                .Map(x => x.MapKey("modified_by"));
        }
    }
}