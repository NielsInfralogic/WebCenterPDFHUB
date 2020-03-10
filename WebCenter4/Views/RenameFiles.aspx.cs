using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebCenter4.Classes;
using System.Text;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace WebCenter4.Views
{
    public partial class RenameFiles : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);
            btnCancel.Text = Global.rm.GetString("txtCancel");
            btnSave.Text = Global.rm.GetString("txtSave");

            SetRadToolbarLabel("Item1", "LabelRenameFilesHeader", Global.rm.GetString("txtRenameFiles"));

            DoDataBind();

        }

        private void DoDataBind()
        {

         //   RadGridRename.DataSource = dt;
         //   RadGridRename.DataBind();

        }


        protected void btnSave_Click(object sender, System.EventArgs e)
        {
           // CopyGridToSession();
            InjectScript.Text = "<script>CloseOnReload()</" + "script>";
        }

        protected void btnCancel_Click(object sender, System.EventArgs e)
        {
            InjectScript.Text = "<script>CloseOnReload()</" + "script>";

        }

        private void SetRadToolbarLabel(string buttonID, string labelID, string text)
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue(buttonID);
            if (item == null)
                return;
            Label label = (Label)item.FindControl(labelID);
            if (label == null)
                return;
            label.Text = text;
        }

    }
}