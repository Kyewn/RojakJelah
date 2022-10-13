using RojakJelah.Database;
using RojakJelah.Database.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RojakJelah
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DataContext dataContext = new DataContext("server=localhost;port=3306;database=rojakjelahv3;uid=root;password=2020twz05!8MSQL");

            var languageList = dataContext.Languages.ToList();
            var wordList = dataContext.Words.ToList();

            Word newWord = new Word()
            {
                WordValue = "test",
                Language = dataContext.Languages.SingleOrDefault(x => x.Name == "Cantonese")
            };

            dataContext.Words.Add(newWord);

            dataContext.SaveChanges();
        }
    }
}