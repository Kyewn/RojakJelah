using RojakJelah.Database.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace RojakJelah.Database.Configuration
{
    public class DictionaryEntryConfiguration : EntityTypeConfiguration<DictionaryEntry>
    {
        public DictionaryEntryConfiguration() : base()
        {
            // Table Name
            ToTable("dictionary_entry");         

            // Primary Key
            HasKey<int>(x => x.Id);

            // Column mappings
            Property(x => x.Id)
                .HasColumnName("dictionary_entry_id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
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
            HasRequired(x => x.Slang)
                .WithMany()
                .Map(x => x.MapKey("slang_id"));

            HasRequired(x => x.Translation)
                .WithMany()
                .Map(x => x.MapKey("translation_id"));

            HasOptional(x => x.CreatedBy)
                .WithMany()
                .Map(x => x.MapKey("created_by"));

            HasOptional(x => x.ModifiedBy)
                .WithMany()
                .Map(x => x.MapKey("modified_by"));
        }
    }
}