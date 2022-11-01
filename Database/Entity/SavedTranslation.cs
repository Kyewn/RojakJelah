using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RojakJelah.Database.Entity
{
    public class SavedTranslation
    {
        public int Id { get; set; }
        public String Input { get; set; }
        public String Output { get; set; }
        public virtual User CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
    }
}