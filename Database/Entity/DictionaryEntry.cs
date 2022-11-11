using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RojakJelah.Database.Entity
{
    [Serializable]
    public class DictionaryEntry
    {
        public int Id { get; set; }
        public virtual Word Slang { get; set; }
        public virtual Word Translation { get; set; }
        public String Example { get; set; }
        public virtual User CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public virtual User ModifiedBy { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}