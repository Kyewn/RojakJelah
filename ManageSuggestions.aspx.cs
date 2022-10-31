using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RojakJelah
{
    public partial class ManageSuggestions : System.Web.UI.Page
    {
        private void addListItem()
        {
            var listItemHTML = @"
                    <div class='listItem'>
                        <div class='topRow'>
                            <div class='itemDetail'>
                                <span>ID</span>
                                <span>Testvalue</span>
                            </div>
                            <div class='itemDetail'>
                                <span>Slang</span>
                                <span>Testvalue</span>
                            </div>
                            <div class='itemDetail'>
                                <span>Translation</span>
                                <span>Testvalue</span>
                            </div>
                            <div class='itemDetail'>
                                <span>Created by</span>
                                <span>Testvalue</span>
                            </div>
                            <div class='itemDetail'>
                                <span>Created at</span>
                                <span>Testvalue</span>
                            </div>
                        </div>
                        <div class='bottomRow'>
                            <div class='itemDetail'>
                                <span>Example</span>
                                <span>Testvalue</span>
                            </div>
                        </div>
                    </div>";

            var listItem = new LiteralControl(listItemHTML);
            listItemContainer.Controls.Add(listItem);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }
    }
}