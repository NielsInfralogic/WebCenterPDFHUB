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
    public partial class CustomAction : System.Web.UI.Page
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

                string errmsg = "";
                CCDBaccess db = new CCDBaccess();

                int dummypressID = 0;

                int productionID = db.GetProductionID(ref dummypressID, publicationID, pubDate, 0, out errmsg);
                int planVersion = db.GetPlanVersion(productionID, out errmsg);
                txtVersion.Text = planVersion > 0 ? planVersion.ToString() : "1";

            }
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            lblProductLabel.Text = Global.rm.GetString("txtProduct");
            lblVersion.Text = Global.rm.GetString("txtVersion");
            bntApply.Text = Global.rm.GetString("txtApply");
            btnCancel.Text = Global.rm.GetString("txtCancel");

        }

        protected void bntApply_Click(object sender, EventArgs e)
        {
            int publicationID = Session["SelectedPublication"] != null ? Globals.GetIDFromName("PublicationNameCache", (string)Session["SelectedPublication"]) : 0;
            DateTime pubDate = Session["SelectedPubDate"] != null ? (DateTime)Session["SelectedPubDate"] : DateTime.MinValue;
            int nEditionID = Globals.GetIDFromName("EditionNameCache", (string)Session["SelectedEdition"]);
            int nSectionID = Globals.GetIDFromName("SectionNameCache", (string)Session["SelectedSection"]);

            string errmsg = "";
            CCDBaccess db = new CCDBaccess();

            string channelList = db.GetChannelsForProduction(publicationID, pubDate, out errmsg);

            if (channelList == "")
            {
                lblInfo.Text = "Unable to retrieve channels for product";
                return;
            }
            int planVersion = Globals.TryParse(txtVersion.Text, 1);

            db.CustomAction(publicationID, pubDate, nEditionID, nSectionID, channelList, planVersion, out errmsg);

            int dummyPressID = 0;
            int productionID = db.GetProductionID(ref dummyPressID, publicationID, pubDate, 0, out errmsg);
            db.SetPlanVersion(productionID, planVersion + 1, out errmsg);
            db.SetPlanVisiolinkStatus(productionID, 1,txtPushMessage.Text,planVersion + 1, out errmsg);

            

            InjectScript.Text = "<script>RefreshParentPage()</" + "script>";

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            InjectScript.Text = "<script>CloseOnReload()</" + "script>";
        }
    }
}