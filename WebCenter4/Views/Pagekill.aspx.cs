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
    public partial class Pagekill : System.Web.UI.Page
    {
        protected int doClose;

        protected void Page_Load(object sender, EventArgs e)
        {
            doClose = 0;
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            if (!this.IsPostBack)
            {
                int masterCopySeparationSet = GetMasterNumber(); // Sets page number label
                if (masterCopySeparationSet == 0)
                {
                    InjectScript.Text = "<script>CloseOnReload()</" + "script>";
                    return;
                }

                cbPermanentKill.Enabled = (bool)Application["AllowPermanentDelete"];
                if ((bool)Application["AllowPermanentDelete"] == false)
                    cbPermanentKill.Checked = false;
            }

            btnCancel.Text = Global.rm.GetString("txtCancel");
            bntApply.Text = Global.rm.GetString("txtApply");
            cbPermanentKill.Text = Global.rm.GetString("txtKillPagePermanently");
        }

        private int GetMasterNumber()
        {
            if (Request.QueryString["page"] != null)
            {
                try
                {
                    lblKillPage.Text = Global.rm.GetString("txtReallyKillPage") + " " + (string)Request.QueryString["page"] + " ?";
                }
                catch
                {
                    ;
                }
            }

            if (Request.QueryString["mastercopyseparationset"] != null)
            {
                try
                {
                    hiddenMasterCopySeparationSet.Value = (string)Request.QueryString["mastercopyseparationset"];
                }
                catch
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
            return Globals.TryParse(hiddenMasterCopySeparationSet.Value, 0);
        }

        protected void bntApply_Click(object sender, EventArgs e)
        {
            CCDBaccess db = new CCDBaccess();

            string errmsg = "";

            int timeout = 40;

            int nMasterCopySeparationSet = Globals.TryParse(hiddenMasterCopySeparationSet.Value, 0);

            while (--timeout > 0)
            {
                int status = db.InProgressStatus(nMasterCopySeparationSet, out errmsg);

                if (status > 0)
                    System.Threading.Thread.Sleep(500);
                else
                {
                    db.UpdateMasterStatus(nMasterCopySeparationSet, 0, "Manually deleted by " + (string)Session["UserName"], out errmsg);

                    if (cbPermanentKill.Checked)
                        db.DeleteMasterCopySeparationSet(nMasterCopySeparationSet, out errmsg);

                    break;
                }
            }

            InjectScript.Text = "<script>RefreshParentPage()</" + "script>";
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            InjectScript.Text = "<script>CloseOnReload()</" + "script>";
        }
    }
}