using RojakJelah.Database.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace RojakJelah.Database.Configuration
{
    public class ReportStatusConfiguration : EntityTypeConfiguration<ReportStatus>
    {
        public ReportStatusConfiguration() : base()
        {
            // Table name
            ToTable("report_status");

            // Primary Key
            HasKey<int>(x => x.Id);

            // Column mappings
            Property(x => x.Id)
                .HasColumnName("report_status_id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();

            Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(50)
                .IsRequired();

            // Indexes
            HasIndex(x => x.Name)
                .IsUnique();
        }
    }
}