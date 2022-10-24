using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RojakJelah.Database.Entity
{
    public class Suggestion
    {
        public int Id { get; set; }
        public String Slang { get; set; }
        public String Translation { get; set; }
        public virtual Language Language { get; set; }
        public String Example { get; set; }
        public virtual SuggestionStatus SuggestionStatus { get; set; }
        public virtual User CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
    }
}