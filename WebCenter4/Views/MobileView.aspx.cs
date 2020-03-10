using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace WebCenter4.Views
{
    public partial class MobileView : System.Web.UI.Page
    {
        public string sImagePath;

        protected void Page_Load(object sender, EventArgs e)
        {
            sImagePath = (string)Session["ImagePath"];
            Response.Clear();
            //Response.Buffer= true;
            //Response.Charset = "";
            Response.ContentType = "image/JPEG";
            Response.AddHeader("Content-Type", "image/JPEG");
            string fileTitle = sImagePath;
            int n = fileTitle.LastIndexOf("/");
            if (n != -1)
                fileTitle = fileTitle.Substring(n + 1);
            string s = "inline;filename=" + fileTitle;
            Response.AddHeader("Content-Disposition", s);
            Response.WriteFile(sImagePath);
            Response.Flush();
            Response.End();
        }
    }
}