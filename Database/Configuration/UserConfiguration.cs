using RojakJelah.Database.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace RojakJelah.Database.Configuration
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration() : base()
        {
            // Table name
            ToTable("users");

            // Primary Key
            HasKey<int>(x => x.Id);

            // Column mappings
            Property(x => x.Id)
                .HasColumnName("user_id")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .IsRequired();
            Property(x => x.Username)
                .HasColumnName("username")
                .HasMaxLength(30)
                .IsRequired();
            Property(x => x.Password)
                .HasColumnName("password")
                .HasMaxLength(60)
                .IsRequired();
            Property(x => x.CreationDate)
                .HasColumnName("creation_date")
                .IsRequired();
            Property(x => x.ModificationDate)
                .HasColumnName("modification_Date")
                .IsRequired();

            // Indexes
            HasIndex(x => x.Username)
                .IsUnique();

            // Foreign Keys
            HasRequired(x => x.Role)
                .WithMany()
                .Map(x => x.MapKey("role_id"));
        }
    }
}