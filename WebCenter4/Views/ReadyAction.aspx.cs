using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Globalization;
using System.Resources;
using System.Threading;
using WebCenter4.Classes;

namespace WebCenter4.Views
{
    public partial class ReadyAction : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                int publicationID = Session["SelectedPublication"] != null ? Globals.GetIDFromName("PublicationNameCache", (string)Session["SelectedPublication"]) : 0;
                DateTime pubDate = Session["SelectedPubDate"] != null ? (DateTime)Session["SelectedPubDate"] : DateTime.MinValue;
                int nEditionID = Globals.GetIDFromName("EditionNameCache", (string)Session["SelectedEdition"]);
                int nSectionID = Globals.GetIDFromName("SectionNameCache", (string)Session["SelectedSection"]);

                if (publicationID == 0 || pubDate.Year < 2000)
                {
                    InjectScript.Text = "<script>CloseOnReload()</" + "script>";
                    return;
                }

                string product = string.Format("{0} {1:00}-{2:00}", (string)Session["SelectedPublication"], pubDate.Day, pubDate.Month);
                if (nEditionID > 0)
                    product += " " + (string)Session["SelectedEdition"];
                if (nSectionID > 0)
                    product += " " + (string)Session["SelectedSection"];

                lblProductText.Text = product;
                
            }
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            lblProductLabel.Text = Global.rm.GetString("txtProduct");
            lblProductLabel.Text = Global.rm.GetString("txtProduct");
            lblAction.Text = Global.rm.GetString("txtReadyAction");
            lblComment.Text = Global.rm.GetString("txtComment");

            RadioButtonList1.Items[0].Text = Global.rm.GetString("txtTooltipReadyAction");
            RadioButtonList1.Items[1].Text = Global.rm.GetString("txtTooltipReadyActionCancel");

            bntApply.Text = Global.rm.GetString("txtApply");
            btnCancel.Text = Global.rm.GetString("txtCancel");

            txtMessage.Text = (bool)Application["ReadyActionSetComment"] ? Global.rm.GetString("txtReadyActionMessageDefault") : "";

            txtMessage.Visible = (bool)Application["ReadyActionSetComment"];
            lblComment.Visible = (bool)Application["ReadyActionSetComment"];

        }

        protected void bntApply_Click(object sender, EventArgs e)
        {
            int publicationID = Session["SelectedPublication"] != null ? Globals.GetIDFromName("PublicationNameCache", (string)Session["SelectedPublication"]) : 0;
            DateTime pubDate = Session["SelectedPubDate"] != null ? (DateTime)Session["SelectedPubDate"] : DateTime.MinValue;
            int nEditionID = Globals.GetIDFromName("EditionNameCache", (string)Session["SelectedEdition"]);
            int nSectionID = Globals.GetIDFromName("SectionNameCache", (string)Session["SelectedSection"]);


            string errmsg = "";
            CCDBaccess db = new CCDBaccess();

            int planVersion = (RadioButtonList1.SelectedIndex == 0) ? 2 : 1;               

            db.ReadyAction(publicationID, pubDate, nEditionID, nSectionID, txtMessage.Text, planVersion, (bool)Application["ReadyActionSetComment"], out errmsg);
            
            InjectScript.Text = "<script>RefreshParentPage()</" + "script>";

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            InjectScript.Text = "<script>CloseOnReload()</" + "script>";
        }

        protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RadioButtonList1.SelectedIndex == 1)
                txtMessage.Text = Global.rm.GetString("txtReadyActionMessageDefault2");
            else
                txtMessage.Text = Global.rm.GetString("txtReadyActionMessageDefault");

        }
    }
}