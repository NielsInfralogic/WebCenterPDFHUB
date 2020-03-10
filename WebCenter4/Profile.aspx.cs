using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Configuration;
using WebCenter4.Classes;

namespace WebCenter4
{
	/// <summary>
	/// Summary description for Profile.
	/// </summary>
	public partial class Profile : System.Web.UI.Page
	{
		
		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
				Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
				Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);				

				lblProfileFor.Text = Global.rm.GetString("txtProfileFor"); 
				lblRealName.Text = Global.rm.GetString("txtRealName"); 
				lblEmail.Text = Global.rm.GetString("txtEmail"); 
				lblImagesPerRow.Text = Global.rm.GetString("txtImagesPerRow"); 
				lblRefreshTime.Text = Global.rm.GetString("txtRefreshTime"); 
				btnSave.Text = Global.rm.GetString("txtSave");
                lblPlateSize.Text = Global.rm.GetString("txtPlatesPerRow");

                lblDefaultViewer.Text = Global.rm.GetString("txtDefaultViewer");
                if ((bool)Application["FieldExists_UserNames_UseHTML5"] == false)
                {
                    DropDownListViewer.Enabled = false;
                    DropDownListViewer.SelectedIndex = 0;
                }
                SetRadToolbarLabel("Item1", "MyProfile", Global.rm.GetString("txtMyProfile"));

				btnReloadCaches.Text = Global.rm.GetString("txtReloadCaches"); 
				lblAdmin.Text = Global.rm.GetString("txtAdminTasks"); 

		 
				btnReloadCaches.ToolTip = Global.rm.GetString("txtTooltipReloadCaches");
				btnSave.ToolTip = Global.rm.GetString("txtTooltipSaveProfile");

				CCDBaccess db = new CCDBaccess();

                string userName = (string)Session["UserName"];

                db.GetUserProfile(userName, out string realName, out string email, out int nPagesPerRow, out int nRefreshTime,
                    out bool bMayApprove, out bool bMayReimage, out bool bMayRunProduction,
                    out int nFlatViewSize, out string columnOrder, out int customerID, out int defaultPressID,
                    out int defaultPublicationID, out bool bMayHardProof, out bool bMayFlatProof,
                    out bool bMayChangeColor, out bool bMayDeleteProduction, out bool bMayConfigure, out int nMaxPlanPages, out bool bHideOld, out bool bMayUpload, out bool bMayReprocess,
                    out string userGroup, out string errmsg);

                lblError.Text = errmsg;
				txtRealname.Text = realName;
				txtEmail.Text = email;

                if (nPagesPerRow == 4 || nPagesPerRow == 6 || nPagesPerRow == 8 || nPagesPerRow == 10 || nPagesPerRow == 12 || nPagesPerRow == 14 || nPagesPerRow == 16 || nPagesPerRow == 18)  
					DropdownlistImagePerRow.SelectedValue = nPagesPerRow.ToString();
				else 
					DropdownlistImagePerRow.SelectedValue = "8";

			//	txtImagePerRow.Text = nPagesPerRow.ToString();
				txtRefreshTime.Text = nRefreshTime.ToString();
				lblUserName.Text = userName;

				if (nFlatViewSize >= 2)
					DropDownListPlateSize.SelectedIndex = nFlatViewSize-2;
				else	
                    DropDownListPlateSize.SelectedIndex = 1;

                bool useHTML5 = false;
                if ((bool)Application["FieldExists_UserNames_UseHTML5"])
                {
                    db.GetUserDefaultViewer(userName, out useHTML5, out errmsg);
                }
                DropDownListViewer.SelectedIndex = useHTML5 ? 1 : 0;
                Session["UseHTML5"] = useHTML5;


                /*	string[] colNames = {	"Publication",	"PubDate",	"Issue",	"Edition",	"Section",	"Page",			"Color",		"Status",		"Version", 
                                            "Approval",		"Hold",		"Priority", "Template", "Device",	"ExternalStatus","CopyNumber", "Pagination",	"Press",	"LastError",	"Comment",
                                            "DeadLine",		"ProofStatus", "Location", "SheetNumber", "SheetSide", "PagePositions", "PageType", "PagesPerPlate",
                                            "InputTime", "ApproveTime", "OutputTime", "VerifyTime", "Active", "Unique", "MasterCopySeparationSet", "SeparationSet",
                                            "FlatSeparationSet", "FlatSeparation", "Separation"};
			
                    bool loadDefafaults= false;
                    if (Session["ColumnOrder"] == null)
                        loadDefafaults = true;
                    if (loadDefafaults == false && (string)Session["ColumnOrder"] == "")
                        loadDefafaults = true;
                    if (loadDefafaults)
                    {
                        string s = "";
                        foreach (string s1 in colNames)
                        {
                            if (s != "")
                                s += ",";
                            s += s1;
                        }
                        Session["ColumnOrder"] = s;
                    }

                    string colOrder = (string)Session["ColumnOrder"];
                    string[] cols = colOrder.Split(',');

                    foreach (string thiscol in cols)
                        ListBoxColumnOrder.Items.Add(thiscol);*/

				lblAdmin.Visible = (bool)Session["IsAdmin"];
				btnReloadCaches.Visible = (bool)Session["IsAdmin"];

			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

			this.btnReloadCaches.Click += new System.EventHandler(this.btnReloadCaches_Click);
			this.btnSave.Click += new System.EventHandler(this.Button1_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void Button1_Click(object sender, System.EventArgs e)
		{
            CCDBaccess db = new CCDBaccess();

            string userName = (string)Session["UserName"];

            int nPagesPerRow = 8;
            if (DropdownlistImagePerRow.SelectedIndex != -1)
                nPagesPerRow = Globals.TryParse(DropdownlistImagePerRow.SelectedValue, 8);

            int nRefreshTime = Globals.TryParse(txtRefreshTime.Text, 60);


            if (nPagesPerRow == 4 || nPagesPerRow == 6 || nPagesPerRow == 8 || nPagesPerRow == 10 || nPagesPerRow == 12 || nPagesPerRow == 14 || nPagesPerRow == 16 || nPagesPerRow == 18)
                DropdownlistImagePerRow.SelectedValue = nPagesPerRow.ToString();
            else
                DropdownlistImagePerRow.SelectedValue = "8";

            int nPlatesPerRow = 4;
            if (DropDownListPlateSize.SelectedIndex != -1)
                nPlatesPerRow = Globals.TryParse(DropDownListPlateSize.SelectedValue, 8);

            db.UpdateUserProfile(userName, txtRealname.Text, txtEmail.Text, nPagesPerRow, nRefreshTime, nPlatesPerRow, out string errmsg);
            lblError.Text = errmsg;
            Session["RefreshTime"] = nRefreshTime;
            Session["PagesPerRow"] = nPagesPerRow;
            Session["PlatesPerRow"] = nPlatesPerRow;
            Session["RefreshTime"] = nRefreshTime;

            bool useHTML5 = Globals.TryParse(DropDownListViewer.SelectedValue, 0) > 0;
             if ((bool)Application["FieldExists_UserNames_UseHTML5"])
                db.UpdateDefaultUserViewer(userName, useHTML5, out errmsg);

            Session["UseHTML5"] = useHTML5;
            

		}

		private void btnReloadCaches_Click(object sender, System.EventArgs e)
		{
			Globals.ForceCacheReloads();
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
