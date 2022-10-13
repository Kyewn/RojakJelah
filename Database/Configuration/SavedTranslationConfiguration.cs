using RojakJelah.Database.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace RojakJelah.Database.Configuration
{
    public class SavedTranslationConfiguration : EntityTypeConfiguration<SavedTranslation>
    {
        public SavedTranslationConfiguration() : base()
        {
            // Table name
            ToTable("saved_translation");

            // Primary Key
            HasKey<int>(x => x.Id);

            // Column mappings
            Property(x => x.Id)
                .HasColumnName("saved_translation_id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            Property(x => x.Input)
                .HasColumnName("input")
                .HasMaxLength(5000)
                .IsRequired();

            Property(x => x.Output)
                .HasColumnName("output")
                .HasMaxLength(5000)
                .IsRequired();

            Property(x => x.CreationDate)
                .HasColumnName("creation_date")
                .IsRequired();

            // Foreign Keys
            HasRequired(x => x.CreatedBy)
                .WithMany()
                .Map(x => x.MapKey("created_by"));
        }
    }
}