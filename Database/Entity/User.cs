using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RojakJelah.Database.Entity
{
    public class User
    {
        public int Id { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public virtual Role Role { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}