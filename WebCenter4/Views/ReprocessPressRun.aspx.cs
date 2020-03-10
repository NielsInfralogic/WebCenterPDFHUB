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
    public partial class ReprocessPressRun : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            btnCancel.Text = Global.rm.GetString("txtCancel");
            bntApply.Text = Global.rm.GetString("txtApply");

//            LabelHeader.Text = Global.rm.GetString("txtReprocessPages");
            lblApplyToAllEditions.Text = Global.rm.GetString("txtApplyToAllEditions");
            lblApplyToAllSections.Text = Global.rm.GetString("txtApplyToAllSections");

            if (!this.IsPostBack)
            {
                if (Request.QueryString["Press"] != null)
                {
                    try
                    {
                        string s = (string)Request.QueryString["Press"];
                        if (s != "")
                            txtPress.Text = s;
                        txtPublication.Text = (string)Request.QueryString["Publication"];
                        txtPubDate.Text = (string)Request.QueryString["PubDate"];
                        txtEdition.Text = (string)Request.QueryString["Edition"];
                        txtSection.Text = (string)Request.QueryString["Section"];
                        txtRipSetupID.Text = (string)Request.QueryString["RipSetup"];
                        
                    }
                    catch
                    {
                        ;
                    }

                }
                else
                {
                    InjectScript.Text = "<script>CloseOnReload()</" + "script>";
                    return;
                }

                DataTable table = (DataTable)Cache["PreflightSetupNamesCache"];
                DropDownListPreflight.Items.Clear();
                foreach (DataRow row in table.Rows)
                {
                    DropDownListPreflight.Items.Add((string)row["Name"]);
                }

                table = (DataTable)Cache["InksaveSetupNamesCache"];
                DropDownListInksave.Items.Clear();
                foreach (DataRow row in table.Rows)
                {
                    DropDownListInksave.Items.Add((string)row["Name"]);
                }

                table = (DataTable)Cache["RipSetupNamesCache"];
                DropDownListRipSetup.Items.Clear();
                foreach (DataRow row in table.Rows)
                {
                    DropDownListRipSetup.Items.Add((string)row["Name"]);
                }

                table = (DataTable)Cache["PressNameCache"];
                DropDownListPress.Items.Clear();

                int inxPress = 0;
                int i = 0;
                foreach (DataRow row in table.Rows)
                {

                    DropDownListPress.Items.Add((string)row["Name"]);
                    if ((string)row["Name"] == txtPress.Text)
                        inxPress = i;
                    i++;
                }

                DropDownListPress.SelectedIndex = inxPress;

                
                int nRipSetupIDEx = Globals.TryParse(txtRipSetupID.Text , 0);

                int nRipSetupID = nRipSetupIDEx & 0x00FF;
                int nPreflightID = (nRipSetupIDEx & 0xFF00) >> 8;
                int nInksaveID = (nRipSetupIDEx & 0xFF0000) >> 16;
               

                string preflightSetupName = Globals.GetNameFromID("PreflightSetupNamesCache", nPreflightID);
                string inksaveSetupName = Globals.GetNameFromID("InksaveSetupNamesCache", nInksaveID);
                string ripSetupName = Globals.GetNameFromID("RipSetupNamesCache", nRipSetupID);

                if (nRipSetupIDEx == 0)
                {

                    CCDBaccess db = new CCDBaccess();
                    string errmsg = "";
                    string ripSetupString = "";
                    if (db.GetDefaultPageProcessingSettingsPublication(Globals.GetIDFromName("PressNameCache", txtPress.Text), Globals.GetIDFromName("PublicationNameCache", txtPublication.Text), out ripSetupString, out errmsg) == false)
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

                if (preflightSetupName != "")
                    DropDownListPreflight.SelectedValue = preflightSetupName;
                else if (DropDownListPreflight.Items.Count > 0)
                    DropDownListPreflight.SelectedIndex = 0;

                if (inksaveSetupName != "")
                    DropDownListInksave.SelectedValue = inksaveSetupName;
                else if (DropDownListInksave.Items.Count > 0)
                    DropDownListInksave.SelectedIndex = 0;

                if (ripSetupName != "")
                    DropDownListRipSetup.SelectedValue = ripSetupName;
                else if (DropDownListRipSetup.Items.Count > 0)
                    DropDownListRipSetup.SelectedIndex = 0;
            }

                 
        }

        protected void bntApply_Click(object sender, EventArgs e)
        {
            CCDBaccess db = new CCDBaccess();
            string errmsg = "";

            int nPreflightID = Globals.GetIDFromName("PreflightSetupNamesCache", DropDownListPreflight.SelectedValue);
            int nInksaveID = Globals.GetIDFromName("InksaveSetupNamesCache", DropDownListInksave.SelectedValue);
            int nRipSetupID = Globals.GetIDFromName("RipSetupNamesCache", DropDownListRipSetup.SelectedValue);

            int processingID = nRipSetupID + (nPreflightID<<8) + (nInksaveID<<16);

            string[] sargs = txtPubDate.Text.Split('-');
            DateTime dt = new DateTime(Int32.Parse(sargs[2]), Int32.Parse(sargs[1]), Int32.Parse(sargs[0]), 0, 0, 0);

        

            int editionID = Globals.GetIDFromName("EditionNameCache", txtEdition.Text);
            if (CheckBoxAllEditions.Checked)
                editionID = 0;
            int sectionID = Globals.GetIDFromName("SectionNameCache", txtSection.Text);
            if (CheckBoxAllSections.Checked)
                sectionID = 0;

            List<int> al = db.GetMasterListFromProduction(Globals.GetIDFromName("PressNameCache", txtPress.Text),
                                                          Globals.GetIDFromName("PublicationNameCache", txtPublication.Text),
                                                          dt, editionID, sectionID, out errmsg);

            foreach (int masterCopySeparationSet in al)
            {
                if (processingID > 0)
                    db.UpdateProcessingParameter(masterCopySeparationSet, processingID, out errmsg);
                db.QueueFileRetryRequest(masterCopySeparationSet, out  errmsg);
            }

            InjectScript.Text = "<script>RefreshParentPage()</" + "script>";
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            InjectScript.Text = "<script>CloseOnReload()</" + "script>";
        }
    }
}