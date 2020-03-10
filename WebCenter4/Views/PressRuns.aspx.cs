using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using WebCenter4.Classes;
using System.Text;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Configuration;

namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for PressRuns.
	/// </summary>
	public class PressRuns : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label lblError;
		protected System.Web.UI.WebControls.DataGrid DataGrid1;
		protected System.Web.UI.WebControls.TextBox txtReturnedFromPriority;
        protected System.Web.UI.WebControls.TextBox txtReturnedFromColor;
		protected System.Web.UI.WebControls.DropDownList DropDownList1;
        protected Telerik.Web.UI.RadWindowManager RadWindowManager1;
        protected global::Telerik.Web.UI.RadToolBar RadToolBar1;

        protected HtmlInputHidden HiddenReturendFromPopup;

		string ofString;
        protected bool doPopupPrio;

        protected string prioString;

      //  static int PRESSRUN_FIELD_CONTROL = 0;
        static int PRESSRUN_FIELD_STATE = 1;
        static int PRESSRUN_FIELD_PRESS = 2;
        static int PRESSRUN_FIELD_PUBDATE = 3;
        static int PRESSRUN_FIELD_PUBLICATION = 4;
        static int PRESSRUN_FIELD_EDITION = 5;
        static int PRESSRUN_FIELD_SECTION = 6;
        static int PRESSRUN_FIELD_COPIES = 7;
        static int PRESSRUN_FIELD_FTP = 8;
        static int PRESSRUN_FIELD_PRE = 9;
        static int PRESSRUN_FIELD_INK = 10;
        static int PRESSRUN_FIELD_RIP = 11;
        static int PRESSRUN_FIELD_INPUT = 12;
        static int PRESSRUN_FIELD_APPROVED = 13;
        static int PRESSRUN_FIELD_OUTPUT = 14;
        static int PRESSRUN_FIELD_PRIORITY = 15;
        static int PRESSRUN_FIELD_DEVICE = 16;
        static int PRESSRUN_FIELD_PRESETUP = 17;
        static int PRESSRUN_FIELD_INKSETUP = 18;
        static int PRESSRUN_FIELD_RIPSETUP = 19;

        static int PROGRESSBAR_WIDTH = 80;

        private void Page_Load(object sender, System.EventArgs e)
		{
			if ((string)Session["UserName"] == null)
				Response.Redirect("~/SessionTimeout.htm");

			if ((string)Session["UserName"] == "")
				Response.Redirect("/Denied.htm");

			SetLanguage();

            ofString = " " + Global.rm.GetString("txtOf") + " ";

            doPopupPrio = false;
            prioString = "";

			if (!this.IsPostBack) 
			{
                Session["PressRunHideOld"] = (bool)Session["HideOld"];
                bool setPressRunDate = false;

                if ((bool)Application["SetPressRunPubDateToTreePubdate"] == true && Session["SelectedPubDate"] != null)
                    if (((DateTime)Session["SelectedPubDate"]).Year > 2000)
                        Session["PressRunPubDate"] = Session["SelectedPubDate"];

                if (Session["PressRunPubDate"] == null)
                    setPressRunDate = true;
                else if ((DateTime)Session["PressRunPubDate"] == DateTime.MinValue && (bool)Application["AlwaysSetPressRunPubDate"] && (bool)Application["PressRunDefaultToAllDates"] == false)
                    setPressRunDate = true;

                if (setPressRunDate)
                {
                    DateTime tToday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                    if (DateTime.Now.Hour > 5)
                        Session["PressRunPubDate"] = tToday.AddDays(1);
                    else
                        Session["PressRunPubDate"] = tToday;
                }

                if ((bool)Application["PressRunDefaultToAllDates"])
                {
                    Session["PressRunPubDate"] = DateTime.MinValue;
                }
                lblError.Text = "";
            }

            CreateDropDowns();      // Do every time??...(if Hide old product selection changed

            if (!this.IsPostBack)
                DoDataBind();

            if (HiddenReturendFromPopup.Value == "1")
            {
                HiddenReturendFromPopup.Value = "0";
                lblError.Text = "";
                DoDataBind();
            }

			//Loop through all windows in the WindowManager.Windows collection
			foreach (Telerik.Web.UI.RadWindow win in RadWindowManager1.Windows)
			{
				//Set whether the first window will be visible on page load
				win.VisibleOnPageLoad = false;
			}

            SetRefreshheader();
		}

        private void SetRefreshheader()
        {
            int nRefreshTime = (int)Session["RefreshTime"];
//            if (nRefreshTime > 0)
//                Response.AddHeader("Refresh", nRefreshTime.ToString());

            if ((bool)Application["NoCache"])
            {
                Response.AppendHeader("cache-control", "private");
                Response.AppendHeader("pragma", "no-cache");
                Response.AppendHeader("expires", "Fri, 30 Oct 1998 14:19:41 GMT");
                Response.CacheControl = "Private";
                Response.Cache.SetNoStore();
            }

        }

		protected void SetLanguage()
		{
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            SetRadToolbarLabel("Refresh", Global.rm.GetString("txtRefresh"));
            SetRadToolbarLabel("Item1", "txtPubDate", Global.rm.GetString("txtPubDate2"));
 
			if ((int)Application["RunViewPageSize"] == 0)
			{
				DataGrid1.AllowPaging = false;
			}
			else
			{
				DataGrid1.AllowPaging = true;
				DataGrid1.PageSize = (int)Application["RunViewPageSize"];
			}
		}

        private string GetCurrentPressIDList()
        {
            string sPressIdList = "";
            if ((bool)Application["UsePressGroups"])
            {

                ArrayList presslist = Globals.GetPressIDListFromPressGroup(Globals.GetIDFromName("PressGroupNameCache", (string)Session["SelectedPress"]));
                foreach (int pressID in presslist)
                {
                    if (sPressIdList != "")
                        sPressIdList += ",";
                    sPressIdList += pressID.ToString(); ;
                }
            }
            else
            {
                string s = (string)Session["SelectedPress"];
                int pressID = Globals.GetIDFromName("PressNameCache", s);
                sPressIdList = pressID.ToString();
            }

            if (sPressIdList == "")
                sPressIdList = "1";
            return sPressIdList;

        }

        private string LocalDateFormat(DateTime thisPubDate)
        {
            if ((int)Application["PubDateFormat"] == 1)
                return string.Format("{0:00}.{1:00}.{2:0000}", thisPubDate.Day, thisPubDate.Month, thisPubDate.Year);
            else if ((int)Application["PubDateFormat"] == 3)
                return string.Format("{0:00}.{1:00}.{2:00}", thisPubDate.Day, thisPubDate.Month, thisPubDate.Year - 2000);
            else if ((int)Application["PubDateFormat"] == 4)
                return string.Format("{0:00}-{1:00}-{2:0000}", thisPubDate.Day, thisPubDate.Month, thisPubDate.Year - 2000);
            else if ((int)Application["PubDateFormat"] == 5)
                return string.Format("{0:00}-{1:00}-{2:0000}", thisPubDate.Day, thisPubDate.Month, thisPubDate.Year);
            else if ((int)Application["PubDateFormat"] == 6)
                return string.Format("{0:00}-{1:00}-{2:00}", thisPubDate.Day, thisPubDate.Month, thisPubDate.Year - 2000);
            else if ((int)Application["PubDateFormat"] == 7)
                return string.Format("{0:00}/{1:00}/{2:0000}", thisPubDate.Day, thisPubDate.Month, thisPubDate.Year);
            else if ((int)Application["PubDateFormat"] == 8)
                return string.Format("{0:00}/{1:00}/{2:00}", thisPubDate.Day, thisPubDate.Month, thisPubDate.Year - 200);

            return thisPubDate.ToShortDateString();
        }

        private void CreateDropDowns()
        {
            CCDBaccess db = new CCDBaccess();
            string allString = Global.rm.GetString("txtAll");
            string weekString = Global.rm.GetString("txtWeek");
            string hideOldString = Global.rm.GetString("txtHideOld");

            string errmsg = "";
            ArrayList weekList = new ArrayList();
            ArrayList weekListDate = new ArrayList();
            ArrayList al = db.GetPubDateList(GetCurrentPressIDList(), (bool)Session["PressRunHideOld"], ref weekList, ref weekListDate, out errmsg);

            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue("Item1");
            if (item == null)
                return;
            DropDownList1 = (DropDownList)item.FindControl("PubDateFilter");

            string currentSelection = "";
            if (DropDownList1.Items.Count > 0)
                currentSelection = DropDownList1.SelectedItem.Value;
            DropDownList1.Items.Clear();
            
/*            DropDownList1.Items.Add(new ListItem("All", "All")); // 0
            foreach (DateTime dt in al)
            {
                DropDownList1.Items.Add(new ListItem(LocalDateFormat(dt), PubDate2String(dt))); // 0
            }
            */

            DropDownList1.Items.Add(new ListItem(allString, "All")); // 0
            DropDownList1.Items.Add(new ListItem(hideOldString, "Hide old")); // 1
            if ((bool)Application["ShowWeeknumberInTreeFilter"] == false)
            {
                foreach (DateTime dt in al)
                    DropDownList1.Items.Add(new ListItem(LocalDateFormat(dt), PubDate2String(dt)));
            }
            else
            {
                //foreach (int w in weekList)
                for (int i = 0; i < weekList.Count; i++)
                    DropDownList1.Items.Add(new ListItem(string.Format("{0} {1:00}", weekString, weekList[i]), PubDate2String((DateTime)weekListDate[i])));
            }



            string s = PubDate2String((DateTime)Session["PressRunPubDate"]);
       /*     if (s == "All" || s == "Alle") 
            {
                DropDownList1.SelectedIndex = 0;
                Session["PressRunPubDate"] = DateTime.MinValue;
            }
            else   */
            {

                if (DropDownList1.Items.FindByValue(currentSelection) != null)
                    DropDownList1.SelectedValue = currentSelection;
                else if (DropDownList1.Items.FindByValue(s) != null)
                    DropDownList1.SelectedValue = s;
                else
                {
                    DateTime t = (DateTime)Session["PressRunPubDate"];
                    t = t.AddDays(-1);
                    s = PubDate2String(t);
                    if (DropDownList1.Items.FindByValue(s) != null)
                    {
                        DropDownList1.SelectedValue = s;
                        Session["PressRunPubDate"] = t;
                    }
                    else
                    {
                        DropDownList1.SelectedIndex = 0;
                        Session["PressRunPubDate"] = DateTime.MinValue;
                    }
                }
            }

            
        }

		public void OnSelChangePubDate(object sender, System.EventArgs e) 
		{
			System.Web.UI.WebControls.DropDownList dropdown = (System.Web.UI.WebControls.DropDownList)sender;
	
			Session["PressRunPubDate"] = String2PubDate(dropdown.SelectedItem.Value);
            Session["PressRunHideOld"] = dropdown.SelectedItem.Value.IndexOf("Hide") >= 0 || dropdown.SelectedItem.Value.IndexOf("Skjul") >= 0;

            DataGrid1.CurrentPageIndex = 0;
			DoDataBind();
		}

		public void DoDataBind()
		{
			CCDBaccess db = new CCDBaccess();

			string errmsg = "";
     //       DataTable dt = db.GetPressRunCollection((DateTime)Session["SelectedPubDate"], out errmsg);

         			DataTable dt = db.GetPressRunCollection((DateTime)Session["PressRunPubDate"], out errmsg);
	
			if (dt != null && errmsg == "") 
			{
				ICollection ic = CreateRunDataSource(dt);
				if (ic != null)
				{
					DataGrid1.DataSource = ic;
					DataGrid1.DataBind();
				}
			}
			else 
				lblError.Text = errmsg;	
		}

		string PubDate2String(DateTime dt)
		{
			if (dt == DateTime.MinValue)
				return "All";
			return dt.Day.ToString() + "-" + dt.Month.ToString() + "-" + dt.Year.ToString();
		}

		DateTime String2PubDate(string sdt)
		{
			if (sdt == "All" || sdt == "Alle")
				return DateTime.MinValue;

            if (sdt.IndexOf("Hide") >= 0  || sdt.IndexOf("Skjul") >= 0)
                return DateTime.MaxValue;

            string [] sargs = sdt.Split('-');
            if (sargs.Length < 3)
                return DateTime.MinValue;

			DateTime dt = new DateTime(Int32.Parse(sargs[2]),Int32.Parse(sargs[1]),Int32.Parse(sargs[0]),0,0,0);
			return dt;
		}

		public ICollection CreateRunDataSource(DataTable dstable)
		{			
			bool hideEdition = Globals.GetCacheRowCount("EditionNameCache") < 2;
			bool hideSection = Globals.GetCacheRowCount("SectionNameCache") < 2;
			bool hideLocation = Globals.GetCacheRowCount("LocationNameCache") < 2;

			DataTable dt = new DataTable();

			DataColumn newColumn;
			newColumn = dt.Columns.Add("State",Type.GetType("System.String"));
            newColumn = dt.Columns.Add("Press", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("PubDate", Type.GetType("System.String"));
			newColumn = dt.Columns.Add("Publication",Type.GetType("System.String"));

			newColumn = dt.Columns.Add("Edition",Type.GetType("System.String"));
			newColumn = dt.Columns.Add("Section",Type.GetType("System.String"));
			newColumn = dt.Columns.Add("Copies",Type.GetType("System.Int32"));
			//	newColumn = dt.Columns.Add("Deadline",Type.GetType("System.String"));
			newColumn = dt.Columns.Add("Input",Type.GetType("System.String"));
			newColumn = dt.Columns.Add("Approved",Type.GetType("System.String"));
			newColumn = dt.Columns.Add("Output",Type.GetType("System.String"));
            newColumn = dt.Columns.Add("Priority", Type.GetType("System.Int32"));

			newColumn = dt.Columns.Add("Devices",Type.GetType("System.String"));
			newColumn = dt.Columns.Add("Preflight",Type.GetType("System.String"));
			newColumn = dt.Columns.Add("Inksave",Type.GetType("System.String"));
			newColumn = dt.Columns.Add("Ripping",Type.GetType("System.String"));

            newColumn = dt.Columns.Add("InputFTP", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("InputPRE", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("InputINK", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("InputRIP", Type.GetType("System.String"));

            newColumn = dt.Columns.Add("Bend", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("Sorted", Type.GetType("System.String"));

			DataView dv = dstable.DefaultView;

			//string	errstr = "";
			string	prevPress = "";
			string  prevPublication = "";
			string	prevEdition = "";
			string	prevSection = "";
			string	prevIssue = "";
			DateTime prevPubDate = new DateTime();
		//	string	prevDeadline = "";
			string  prevLocation = "";

			int		nPageCountTotal = 0;
			int		nPageCountPolled = 0;
			int		nPagesWithError = 0;
			int		nApprovalCount = 0;
			int		nPlateCountTotal = 0;
			int		nPlateCountImaged = 0;
			int		nPriority = 0;
			int		nMinStatus = 100;
			//bool	bHold = false;
			bool	bHasError = false;
			bool	isApproved = true;
			string devices = "";


            int nInputFTPCount = 0;
            int nInputPRECount = 0;
            int nInputINKCount = 0;
            int nInputRIPCount = 0;

            int nBendCount = 0;
            int nSortedCount = 0;

            string errmsg;

			int nPlateWithError = 0;

			CCDBaccess db = new CCDBaccess();

			//DataSet ds = db.GetPressRunCollection(errmsg);

            foreach (DataRowView row in dv)
            {
                DataRow dr = dt.NewRow();

                prevLocation = (string)row["Location"];
                prevPress = (string)row["Press"];


                prevPublication = (string)row["Publication"];

                prevPubDate = (DateTime)row["PubDate"];
                prevEdition = (string)row["Edition"];
                //prevIssue = (string)row["Issue"];

                prevSection = (string)row["Section"];
                dr["Copies"] = (int)row["Copies"];

                nPageCountTotal = (int)row["Pages"];

                dr["Priority"] = (int)row["Priority"];
                dr["State"] = (int)row["Hold"] == 1 ? "On hold" : "Released";
             //   if (hideLocation)
                    dr["Press"] = prevPress;
              //  else
              //   dr["Press"] = prevPress + " (" + prevLocation + ")";

                dr["PubDate"] = PubDate2String(prevPubDate);
                dr["Publication"] = prevPublication;
                if ((int)row["PlanType"] == 0)
                    dr["Publication"] += "#";
                dr["Edition"] = prevEdition;

                dr["Section"] = prevSection;

                //	dr["Issue"] = prevIssue;

                //	DateTime dlt = (DateTime)row["PubDate"];
                //	dr["DeadLine"] = dlt.ToShortDateString() + " " + dlt.ToShortTimeString(); 

                if (prevPress.IndexOf('(') > 0)
                {
                    prevPress = prevPress.Substring(0, prevPress.IndexOf('('));
                    prevPress = prevPress.Trim();
                }

                db.GetPageStat(prevLocation, prevPress, prevPubDate, prevPublication, prevIssue, prevEdition, prevSection, 
                    out nPageCountPolled, out nApprovalCount, out nPagesWithError,
                    out nInputFTPCount, out nInputPRECount, out nInputINKCount, out nInputRIPCount, out nPageCountTotal,
                    out  errmsg);

                dr["Input"] = nPageCountPolled.ToString() + ofString + nPageCountTotal.ToString();
                dr["Approved"] = nApprovalCount.ToString() + ofString + nPageCountTotal.ToString();
                devices = "";


                dr["InputFTP"] = nInputFTPCount.ToString() + ofString + nPageCountTotal.ToString();
                dr["InputPRE"] = nInputPRECount.ToString() + ofString + nPageCountTotal.ToString();
                dr["InputINK"] = nInputINKCount.ToString() + ofString + nPageCountTotal.ToString();
                dr["InputRIP"] = nInputRIPCount.ToString() + ofString + nPageCountTotal.ToString();

                db.GetPlatesDone(prevLocation, prevPress, prevPublication, prevPubDate, prevIssue, prevEdition, prevSection, out  nPlateCountTotal, out  nPlateCountImaged, out  nPlateWithError, out devices, 
                    out nBendCount, out nSortedCount,out  errmsg);
                
                dr["Devices"] = devices;
                dr["Output"] = nPlateCountImaged.ToString() + ofString + nPlateCountTotal.ToString();

                dr["Bend"] = nBendCount.ToString() + ofString + nPlateCountTotal.ToString();
                dr["Sorted"] = nSortedCount.ToString() + ofString + nPlateCountTotal.ToString();

                int nRipSetupIDEx = 0;
                db.GetPageProcessingSettingsPressRun((int)row["PressRunID"], out nRipSetupIDEx, out errmsg);

                int nPreflightID = ((nRipSetupIDEx) & 0xFF00) >> 8;
                int nInksaveID = ((nRipSetupIDEx) & 0xFF0000) >> 16;
                int nRipSetupID = nRipSetupIDEx & 0x00FF;
                dr["Preflight"] = Globals.GetNameFromID("PreflightSetupNamesCache", nPreflightID); ;
                dr["Inksave"] = Globals.GetNameFromID("InksaveSetupNamesCache", nInksaveID);
                dr["Ripping"] = Globals.GetNameFromID("RipSetupNamesCache", nRipSetupID); ;

                dt.Rows.Add(dr);
            }
             
			return dt.DefaultView;
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
			this.DataGrid1.ItemCreated += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DataGrid1_ItemCreated);
			this.DataGrid1.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGrid1_ItemCommand);
			this.DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGrid1_PageIndexChanged);
			this.DataGrid1.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DataGrid1_ItemDataBound);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion



		private void DataGrid1_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{

			if (e.Item.ItemType == ListItemType.Header)
			{
                e.Item.Cells[PRESSRUN_FIELD_STATE].Text = Global.rm.GetString("txtState");
                e.Item.Cells[PRESSRUN_FIELD_PRESS].Text = Global.rm.GetString("txtPress");
                e.Item.Cells[PRESSRUN_FIELD_PUBDATE].Text = Global.rm.GetString("txtPubDate");
                e.Item.Cells[PRESSRUN_FIELD_PUBLICATION].Text = Global.rm.GetString("txtPublication");
                e.Item.Cells[PRESSRUN_FIELD_EDITION].Text = Global.rm.GetString("txtEdition");
                e.Item.Cells[PRESSRUN_FIELD_SECTION].Text = Global.rm.GetString("txtSection");
                e.Item.Cells[PRESSRUN_FIELD_COPIES].Text = Global.rm.GetString("txtCopies");
                e.Item.Cells[PRESSRUN_FIELD_INPUT].Text = Global.rm.GetString("txtInput");
                e.Item.Cells[PRESSRUN_FIELD_APPROVED].Text = Global.rm.GetString("txtApproved");

                e.Item.Cells[PRESSRUN_FIELD_OUTPUT].Text = (bool)Application["LocationIsPress"] && (bool)Application["UsePressGroups"] == false ? Global.rm.GetString("txtTransmitted") : Global.rm.GetString("txtOutput");
                e.Item.Cells[PRESSRUN_FIELD_PRIORITY].Text = Global.rm.GetString("txtPriority2");
                e.Item.Cells[PRESSRUN_FIELD_DEVICE].Text = Global.rm.GetString("txtDevice");
            }

			if ((e.Item.ItemType == ListItemType.Item) ||
				(e.Item.ItemType == ListItemType.AlternatingItem) ||
				(e.Item.ItemType == ListItemType.SelectedItem))
			{

                string publication = e.Item.Cells[PRESSRUN_FIELD_PUBLICATION].Text;
                if (publication.IndexOf("#") != -1)
                {
                    e.Item.Cells[PRESSRUN_FIELD_PUBLICATION].Text = publication.Replace("#", "");
                    e.Item.Cells[PRESSRUN_FIELD_PUBLICATION].ForeColor = Color.DarkRed;
                    e.Item.Cells[PRESSRUN_FIELD_EDITION].ForeColor = Color.DarkRed;
                    e.Item.Cells[PRESSRUN_FIELD_SECTION].ForeColor = Color.DarkRed;
                    e.Item.Cells[PRESSRUN_FIELD_PUBDATE].ForeColor = Color.DarkRed;
                }
                
                Label label = (Label)e.Item.FindControl("labelState");
				label.ForeColor = (label.Text == "Released") ? Color.Black : Color.White;
				Panel panel = (Panel)e.Item.FindControl("panelState");
				panel.Width = 80;
                if ((bool)Application["FlatLook"] == false)
    				panel.BackImageUrl =  (label.Text == "Released") ? "../Images/greengradient2.gif" : "../Images/redgradient2.gif";
                else
                    label.BackColor = (label.Text == "Released") ? Color.LawnGreen : Color.Red;

                label.Text = (label.Text == "Released") ? Global.rm.GetString("txtReleased") : Global.rm.GetString("txtOnHold");


				label = (Label)e.Item.FindControl("labelInputProgress");
				string s = label.Text;
				int n = s.IndexOf(ofString);
				int n1 = Int32.Parse(s.Substring(0,n));
				int n2 = 0;
				if (n != -1)
					n2 = Int32.Parse(s.Substring(n+ofString.Length));

				double f1 = (double)n1;
				double f2 = n2 > 0 ? (double)n2 : 1.0;
				panel = (Panel)e.Item.FindControl("PanelInputProgress");
				panel.BackImageUrl = "PressRunsImages.aspx?n1="+n1.ToString()+"&n2="+n2.ToString();
				panel.Width = PROGRESSBAR_WIDTH;
				
				
				label = (Label)e.Item.FindControl("labelApproveProgress");
				s = label.Text;
				n = s.IndexOf(ofString);
				n1 = Int32.Parse(s.Substring(0,n));
				n2 = 0;
				if (n != -1)
					n2 = Int32.Parse(s.Substring(n+ofString.Length));

				panel = (Panel)e.Item.FindControl("PanelApproveProgress");
				//	panel.BackImageUrl = "../Images/runs/" + e.Item.ItemIndex.ToString()+"-2.jpg";
				panel.BackImageUrl = "PressRunsImages.aspx?n1="+n1.ToString()+"&n2="+n2.ToString();
				panel.Width = PROGRESSBAR_WIDTH;
				
				
				label = (Label)e.Item.FindControl("labelOutputProgress");
				s = label.Text;
				n = s.IndexOf(ofString);
				n1 = Int32.Parse(s.Substring(0,n));
				n2 = 0;
				if (n != -1)
					n2 = Int32.Parse(s.Substring(n+ofString.Length));
				panel = (Panel)e.Item.FindControl("PanelOutputProgress");
				//panel.BackImageUrl = "../Images/runs/" + e.Item.ItemIndex.ToString()+"-3.jpg";
				panel.Width = PROGRESSBAR_WIDTH;
				panel.BackImageUrl = "PressRunsImages.aspx?n1="+n1.ToString()+"&n2="+n2.ToString();


                label = (Label)e.Item.FindControl("labelFTPProgress");
                s = label.Text;
                n = s.IndexOf(ofString);
                n1 = Int32.Parse(s.Substring(0, n));
                n2 = 0;
                if (n != -1)
                    n2 = Int32.Parse(s.Substring(n + ofString.Length));

                panel = (Panel)e.Item.FindControl("PanelFTPProgress");

                if ((bool)Application["ExtendedThumbnailViewShowFTP"] == false)
                {
                    panel.Visible = false;
                    panel.Width = 0;
                }
                else
                {
                    panel.Width = PROGRESSBAR_WIDTH;
                    panel.BackImageUrl = "PressRunsImages.aspx?n1=" + n1.ToString() + "&n2=" + n2.ToString();
                }

                label = (Label)e.Item.FindControl("labelPREProgress");
                s = label.Text;
                n = s.IndexOf(ofString);
                n1 = Int32.Parse(s.Substring(0, n));
                n2 = 0;
                if (n != -1)
                    n2 = Int32.Parse(s.Substring(n + ofString.Length));
                panel = (Panel)e.Item.FindControl("PanelPREProgress");
                //panel.BackImageUrl = "../Images/runs/" + e.Item.ItemIndex.ToString()+"-3.jpg";

                
                if ((bool)Application["ExtendedThumbnailViewShowPreflight"] == false)
                {
                    panel.Visible = false;
                    panel.Width = 0;
                }
                else
                {
                    panel.Width = PROGRESSBAR_WIDTH;
                    panel.BackImageUrl = "PressRunsImages.aspx?n1=" + n1.ToString() + "&n2=" + n2.ToString();
                }

                label = (Label)e.Item.FindControl("labelINKProgress");
                s = label.Text;
                n = s.IndexOf(ofString);
                n1 = Int32.Parse(s.Substring(0, n));
                n2 = 0;
                if (n != -1)
                    n2 = Int32.Parse(s.Substring(n + ofString.Length));
                panel = (Panel)e.Item.FindControl("PanelINKProgress");

                if ((bool)Application["ExtendedThumbnailViewShowInkSave"] == false)
                {
                    panel.Visible = false;
                    panel.Width = 0;
                }
                else
                {
                    panel.Width = PROGRESSBAR_WIDTH;
                    panel.BackImageUrl = "PressRunsImages.aspx?n1=" + n1.ToString() + "&n2=" + n2.ToString();
                }

                label = (Label)e.Item.FindControl("labelRIPProgress");
                s = label.Text;
                n = s.IndexOf(ofString);
                n1 = Int32.Parse(s.Substring(0, n));
                n2 = 0;
                if (n != -1)
                    n2 = Int32.Parse(s.Substring(n + ofString.Length));
                panel = (Panel)e.Item.FindControl("PanelRIPProgress");

                if ((bool)Application["ExtendedThumbnailViewShowRIP"] == false)
                {
                    panel.Visible = false;
                    panel.Width = 0;
                }
                else
                {
                    panel.Width = PROGRESSBAR_WIDTH;
                    panel.BackImageUrl = "PressRunsImages.aspx?n1=" + n1.ToString() + "&n2=" + n2.ToString();
                }
                
                ImageButton imgbtn = (ImageButton)e.Item.FindControl("ImageButtonGo"); 
                if (imgbtn != null)
                    imgbtn.Visible = (bool)Session["IsAdmin"] || (bool)Session["MayRelease"];

				imgbtn =(ImageButton)e.Item.FindControl("ImageButtonHold"); 
                if (imgbtn != null)
                    imgbtn.Visible = (bool)Session["IsAdmin"] || (bool)Session["MayRelease"];

				imgbtn =(ImageButton)e.Item.FindControl("ImageButtonPriority"); 
                if (imgbtn != null)
                    imgbtn.Visible = (bool)Session["IsAdmin"] || (bool)Session["MayRelease"];

                imgbtn = (ImageButton)e.Item.FindControl("ImageButtonReprocess"); 
                if (imgbtn != null)
                    imgbtn.Visible = (bool)Session["IsAdmin"] || ((bool)Application["AllowReprocess"] && (bool)Session["MayReprocess"]);

                
				
			}
		}

		private void DataGrid1_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			DataGrid1.SelectedIndex = e.Item.ItemIndex;

			if (e.CommandName == "Hold" || e.CommandName == "Go")
			{
				if ((bool)Session["MayRelease"] == false) 
				{
					lblError.Text = "You do not have rights to hold/release products";
					lblError.ForeColor = Color.OrangeRed;
					return;
				}
                string Press = e.Item.Cells[PRESSRUN_FIELD_PRESS].Text;

                int m = Press.IndexOf('(');
                if (m != -1)
                    Press = Press.Substring(0, m);
                Press = Press.Trim();

                DateTime dt = String2PubDate(e.Item.Cells[PRESSRUN_FIELD_PUBDATE].Text);
                string Publication = e.Item.Cells[PRESSRUN_FIELD_PUBLICATION].Text;
                string Edition = e.Item.Cells[PRESSRUN_FIELD_EDITION].Text;
                string Section = e.Item.Cells[PRESSRUN_FIELD_SECTION].Text;

				CCDBaccess db = new CCDBaccess();

				string errmsg = "";
				if (db.UpdateProductionHold(e.CommandName == "Hold" ? 1: 0, Press, Publication, dt, "", Edition, Section, out errmsg) == false)
				{
					lblError.Text = errmsg;
					lblError.ForeColor = Color.Red;
				}
				else
				{
					DoDataBind();
				}
			} 
			else if (e.CommandName == "Priority") 
			{
				if ((bool)Session["MayRelease"] == false) 
				{
					lblError.Text = "You do not have rights to change priority";
					lblError.ForeColor = Color.OrangeRed;
					return;
				}

				string Press = e.Item.Cells[PRESSRUN_FIELD_PRESS].Text;
                int m = Press.IndexOf('(');
                if (m != -1)
                    Press = Press.Substring(0, m);
                Press = Press.Trim();
				
				//				DateTime dt = String2PubDate(e.Item.Cells[nNextIndex++].Text);
                string sPubDate = e.Item.Cells[PRESSRUN_FIELD_PUBDATE].Text;
                string Publication = e.Item.Cells[PRESSRUN_FIELD_PUBLICATION].Text;				
                string Edition = e.Item.Cells[PRESSRUN_FIELD_EDITION].Text;
                string Section = e.Item.Cells[PRESSRUN_FIELD_SECTION].Text;
                string sPrio = e.Item.Cells[PRESSRUN_FIELD_PRIORITY].Text;

				Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowPriority");
                mywindow.Title = Global.rm.GetString("txtPriority");

				mywindow.NavigateUrl = "ChangePriority.aspx?Priority="+sPrio+"&Press="+Press+"&Publication="+Publication+"&PubDate="+sPubDate+"&Issue=1&Edition="+Edition+"&Section="+Section;

				mywindow.VisibleOnPageLoad = true;

			}
            else if (e.CommandName == "Reprocess")
            {
                string Press = e.Item.Cells[PRESSRUN_FIELD_PRESS].Text;
                int m = Press.IndexOf('(');
                if (m != -1)
                    Press = Press.Substring(0, m);
                Press = Press.Trim();

                string sPubDate = e.Item.Cells[PRESSRUN_FIELD_PUBDATE].Text;
                string Publication = e.Item.Cells[PRESSRUN_FIELD_PUBLICATION].Text;
                string Edition = e.Item.Cells[PRESSRUN_FIELD_EDITION].Text;
                string Section = e.Item.Cells[PRESSRUN_FIELD_SECTION].Text;
                int nPreflightID = Globals.GetIDFromName("PreflightSetupNamesCache", e.Item.Cells[PRESSRUN_FIELD_PRESETUP].Text);
                int nInksaveID = Globals.GetIDFromName("InksaveSetupNamesCache", e.Item.Cells[PRESSRUN_FIELD_INKSETUP].Text);
                int nRipSetupID = Globals.GetIDFromName("RipSetupNamesCache", e.Item.Cells[PRESSRUN_FIELD_RIPSETUP].Text);
                int ID = nRipSetupID + (nPreflightID << 8) + (nInksaveID << 16);

                Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowReprocess");
                mywindow.Title = Global.rm.GetString("txtReprocessPages");

                mywindow.NavigateUrl = "ReprocessPressRun.aspx?Press=" + Press + "&Publication=" + Publication + "&PubDate=" + sPubDate + "&Issue=1&Edition=" + Edition + "&Section=" + Section + "&RipSetup=" + ID.ToString();

                mywindow.VisibleOnPageLoad = true;

            }
        }

		private void DataGrid1_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			// For the DataGrid control to navigate to the correct page when
			// paging is allowed, the CurrentPageIndex property must be updated
			// programmatically. This process is usually accomplished in the
			// event-handling method for the PageIndexChanged event.

			// Set CurrentPageIndex to the page the user clicked.
			DataGrid1.CurrentPageIndex = e.NewPageIndex;

			// Rebind the data to refresh the DataGrid control. 
			DoDataBind();
		}

		private void DataGrid1_ItemCreated(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			bool	hideCopies = true;


            if ((bool)Application["RunViewHideDevices"])
                DataGrid1.Columns[PRESSRUN_FIELD_DEVICE].Visible = false;

			if (hideCopies)
                DataGrid1.Columns[PRESSRUN_FIELD_COPIES].Visible = false;

			if (Globals.GetCacheRowCount("SectionNameCache") < 2)
                DataGrid1.Columns[PRESSRUN_FIELD_SECTION].Visible = false;

			if (Globals.GetCacheRowCount("EditionNameCache") < 2)
                DataGrid1.Columns[PRESSRUN_FIELD_EDITION].Visible = false;



            if ((bool)Application["PressRunPrePollStatus"] == false)
            {
                DataGrid1.Columns[PRESSRUN_FIELD_FTP].Visible = false;
                DataGrid1.Columns[PRESSRUN_FIELD_PRE].Visible = false;
                DataGrid1.Columns[PRESSRUN_FIELD_INK].Visible = false;
                DataGrid1.Columns[PRESSRUN_FIELD_RIP].Visible = false;
            }

			//	DataGrid1.Columns[idx-3].Visible = false;
		}

        protected void RadToolBar1_ButtonClick(object sender, Telerik.Web.UI.RadToolBarEventArgs e)
        {
			DoDataBind();
		}

        private Telerik.Web.UI.RadWindow GetRadWindow(string name)
        {
            foreach (Telerik.Web.UI.RadWindow win in RadWindowManager1.Windows)
            {
                if (win.ID == name)
                    return win;
            }
            return RadWindowManager1.Windows[0];

        }

        private void SetRadToolbarLabel(string buttonID, string text)
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue(buttonID);
            if (item == null)
                return;
            item.Text = text;
        }

        private void SetRadToolbarTooltip(string buttonID, string text)
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue(buttonID);
            if (item == null)
                return;
            item.ToolTip = text;
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

        private void SetRadToolbarTooltip(string buttonID, string labelID, string text)
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue(buttonID);
            if (item == null)
                return;
            Label label = (Label)item.FindControl(labelID);
            if (label == null)
                return;
            label.ToolTip = text;
        }

	}
}
