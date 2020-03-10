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
using System.Data;

namespace WebCenter4.Views
{
    public partial class ReprocessPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);
            btnCancel.Text = Global.rm.GetString("txtCancel");
            bntApply.Text = Global.rm.GetString("txtApply");

            LabelHeader.Text = Global.rm.GetString("txtReprocessPage");

            if (!this.IsPostBack)
            {
                int masterCopySeparationSet = GetMasterNumber();
                if (masterCopySeparationSet == 0)
                {
                    InjectScript.Text = "<script>CloseOnReload()</" + "script>";
                    return;
                }
               

                DataTable table = (DataTable)Cache["PreflightSetupNamesCache"];

                DropDownListPreflight.Enabled = true;
                DropDownListInksave.Enabled = true;
                DropDownListRipSetup.Enabled = true;

                DropDownListPreflight.Items.Clear();
                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        DropDownListPreflight.Items.Add((string)row["Name"]);
                    }
                }
                else
                    DropDownListPreflight.Enabled = false;

                table = (DataTable)Cache["InksaveSetupNamesCache"];
                DropDownListInksave.Items.Clear();
                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        DropDownListInksave.Items.Add((string)row["Name"]);
                    }
                }
                else
                    DropDownListInksave.Enabled = false;

                table = (DataTable)Cache["RipSetupNamesCache"];
                DropDownListRipSetup.Items.Clear();
                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        DropDownListRipSetup.Items.Add((string)row["Name"]);
                    }
                }
                else
                    DropDownListRipSetup.Enabled = false;

                CCDBaccess db = new CCDBaccess();
                string errmsg = "";
                int nRipSetupIDEx = 0;

                if (db.GetPageProcessingSettings(masterCopySeparationSet, out nRipSetupIDEx, out errmsg) == false)
                {
                    lblError.Text = errmsg;
                    return;
                }

                int nPreflightID = ((nRipSetupIDEx) & 0xFF00) >> 8;
                int nInksaveID = ((nRipSetupIDEx) & 0xFF0000) >> 16;
                int nRipSetupID = nRipSetupIDEx & 0x00FF;

                string preflightSetupName = Globals.GetNameFromID("PreflightSetupNamesCache", nPreflightID);
                string inksaveSetupName = Globals.GetNameFromID("InksaveSetupNamesCache", nInksaveID);
                string ripSetupName = Globals.GetNameFromID("RipSetupNamesCache", nRipSetupID);

                if (nRipSetupIDEx == 0)
                {
                    string ripSetupString = "";
                    if (db.GetDefaultPageProcessingSettings(masterCopySeparationSet, out ripSetupString, out errmsg) == false)
                    {
                        lblError.Text = errmsg;
                        return;
                    }

                    if (ripSetupString != "")
                    {
                        string[] aS = ripSetupString.Split(',');
                        if (aS.Length > 0)
                            ripSetupName = aS[0];
                        if (aS.Length > 1)
                            preflightSetupName = aS[1];
                        if (aS.Length > 2)
                            inksaveSetupName = aS[2];
                    }
                }


                if (preflightSetupName != "" && DropDownListPreflight.Enabled)
                    DropDownListPreflight.SelectedValue = preflightSetupName;
                else if (DropDownListPreflight.Items.Count > 0)
                    DropDownListPreflight.SelectedIndex = 0;

                if (inksaveSetupName != "" && DropDownListInksave.Enabled)
                    DropDownListInksave.SelectedValue = inksaveSetupName;
                else if (DropDownListInksave.Items.Count > 0)
                    DropDownListInksave.SelectedIndex = 0;

                if (ripSetupName != "" && DropDownListRipSetup.Enabled)
                    DropDownListRipSetup.SelectedValue = ripSetupName;
                else if (DropDownListRipSetup.Items.Count > 0)
                    DropDownListRipSetup.SelectedIndex = 0;
            }
        }

        private int GetMasterNumber()
        {
            if (Request.QueryString["MasterCopySeparationSet"] != null)
            {
                try
                {
                    hiddenMasterCopySeparationSet.Value = (string)Request.QueryString["MasterCopySeparationSet"];
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
            int nMasterCopySeparationSet = Globals.TryParse(hiddenMasterCopySeparationSet.Value, 0);
            CCDBaccess db = new CCDBaccess();
            string errmsg = "";

            int nPreflightID = Globals.GetIDFromName("PreflightSetupNamesCache", DropDownListPreflight.SelectedValue);
            int nInksaveID = Globals.GetIDFromName("InksaveSetupNamesCache", DropDownListInksave.SelectedValue);
            int nRipSetupID = Globals.GetIDFromName("RipSetupNamesCache", DropDownListRipSetup.SelectedValue);

            int ID = nRipSetupID + (nPreflightID<<8) + (nInksaveID<<16);

            if (ID > 0)
                db.UpdateProcessingParameter(nMasterCopySeparationSet, ID, out errmsg);

            db.QueueFileRetryRequest(nMasterCopySeparationSet, out  errmsg);

            InjectScript.Text = "<script>RefreshParentPage()</" + "script>";
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            InjectScript.Text = "<script>CloseOnReload()</" + "script>";
        }
    }
}