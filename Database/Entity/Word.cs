using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RojakJelah.Database.Entity
{
    [Serializable]
    public class Word
    {
        public int Id { get; set; }
        public String WordValue { get; set; }
        public virtual Language Language { get; set; }
    }
}