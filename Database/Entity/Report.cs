using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RojakJelah.Database.Entity
{
    [Serializable]
    public class Report
    {
        public int Id { get; set; }
        public virtual DictionaryEntry DictionaryEntry { get; set; }
        public virtual ReportCategory ReportCategory { get; set; }
        public String Description { get; set; }
        public virtual ReportStatus ReportStatus { get; set; }
        public virtual User CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
    }
}