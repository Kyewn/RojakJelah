using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RojakJelah.Database.Models.Entity
{
    public class Word
    {
        [ForeignKey("Language")]
        public int Id { get; set; }
        public String WordValue { get; set; }
        public virtual Language Language { get; set; } = new Language();
    }
}