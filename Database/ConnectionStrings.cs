using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace RojakJelah.Database
{
    public static class ConnectionStrings
    {
        public static string RojakJelahConnection
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["RojakJelahConnection"].ConnectionString;
            }
        }
    }
}