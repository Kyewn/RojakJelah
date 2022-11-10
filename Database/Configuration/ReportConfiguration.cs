using RojakJelah.Database.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace RojakJelah.Database.Configuration
{
    public class ReportConfiguration : EntityTypeConfiguration<Report>
    {
        public ReportConfiguration() : base()
        {
            // Table name
            ToTable("report");

            // Primary Key
            HasKey<int>(x => x.Id);

            // Column mappings
            Property(x => x.Id)
                .HasColumnName("report_id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            Property(x => x.Description)
                .HasColumnName("description")
                .HasMaxLength(500)
                .IsOptional();

            Property(x => x.CreationDate)
                .HasColumnName("creation_date")
                .IsRequired();      

            Property(x => x.ModificationDate)
                .HasColumnName("modification_date")
                .IsRequired();

            // Foreign Keys
            HasOptional(x => x.DictionaryEntry)
                .WithMany()
                .Map(x => x.MapKey("dictionary_entry_id"));

            HasRequired(x => x.ReportCategory)
                .WithMany()
                .Map(x => x.MapKey("report_category_id"));

            HasRequired(x => x.ReportStatus)
                .WithMany()
                .Map(x => x.MapKey("report_status_id"));

            HasRequired(x => x.CreatedBy)
                .WithMany()
                .Map(x => x.MapKey("created_by"));   
            
            HasRequired(x => x.ModifiedBy)
                .WithMany()
                .Map(x => x.MapKey("modified_by"));
        }
    }
}