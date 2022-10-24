using RojakJelah.Database.Configuration;
using RojakJelah.Database.Entity;
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
            System.Data.Entity.Database.SetInitializer<DataContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new LanguageConfiguration());
            modelBuilder.Configurations.Add(new WordConfiguration());
            modelBuilder.Configurations.Add(new RoleConfiguration());
            modelBuilder.Configurations.Add(new UserConfiguration());
            modelBuilder.Configurations.Add(new DictionaryEntryConfiguration());
            modelBuilder.Configurations.Add(new SuggestionStatusConfiguration());
            modelBuilder.Configurations.Add(new SuggestionConfiguration());
            modelBuilder.Configurations.Add(new ReportStatusConfiguration());
            modelBuilder.Configurations.Add(new ReportCategoryConfiguration());
            modelBuilder.Configurations.Add(new ReportConfiguration());
            modelBuilder.Configurations.Add(new SavedTranslationConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<Word> Words { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<DictionaryEntry> DictionaryEntries { get; set; }
        public virtual DbSet<SuggestionStatus> SuggestionStatuses { get; set; }
        public virtual DbSet<Suggestion> Suggestions { get; set; }
        public virtual DbSet<ReportCategory> ReportCategories { get; set; }
        public virtual DbSet<ReportStatus> ReportStatuses { get; set; }
        public virtual DbSet<Report> Reports { get; set; }
        public virtual DbSet<SavedTranslation> SavedTranslations { get; set; }
    }
}