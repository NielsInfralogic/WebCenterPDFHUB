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
using System.Collections.Generic;

namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for PressRuns.
	/// </summary>
    public class PressRunsChannels : System.Web.UI.Page
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

       // static int PRESSRUN_FIELD_CONTROL = 0;
        static int PRESSRUN_FIELD_PRESS = 1;
        static int PRESSRUN_FIELD_STATE = 2;
     
        static int PRESSRUN_FIELD_PUBDATE = 3;
        static int PRESSRUN_FIELD_PUBLICATION = 4;
        static int PRESSRUN_FIELD_EDITION = 5;
        static int PRESSRUN_FIELD_SECTION = 6;

        static int PRESSRUN_FIELD_INPUT = 7;
        static int PRESSRUN_FIELD_APPROVED = 8;
        static int PRESSRUN_FIELD_OUTPUT = 9;
        static int PRESSRUN_FIELD_CHANNELOFFSET = 10;

        static int MAXCHANNELCOLUMNS = 20;

        static int PROGRESSBAR_WIDTH = 80;

        private void Page_Load(object sender, System.EventArgs e)
		{
			if ((string)Session["UserName"] == null)
				Response.Redirect("~/SessionTimeout.htm");

			if ((string)Session["UserName"] == "")
				Response.Redirect("/Denied.htm");

            Session["PressRunChannelList"] = null;

			SetLanguage();

            ofString = " " + Global.rm.GetString("txtOf") + " ";

            doPopupPrio = false;
            prioString = "";

			if (!this.IsPostBack) 
			{
                bool setPressRunDate = false;

                if ((bool)Application["SetPressRunPubDateToTreePubdate"] == true && Session["SelectedPubDate"] != null)
                    if (((DateTime)Session["SelectedPubDate"]).Year > 2000)
                        Session["PressRunPubDate"] = Session["SelectedPubDate"];

                if (Session["PressRunPubDate"] == null)
                    setPressRunDate = true;
                else if ((DateTime)Session["PressRunPubDate"] == DateTime.MinValue && (bool)Application["AlwaysSetPressRunPubDate"])
                    setPressRunDate = true;

                if (setPressRunDate)
                {
                    DateTime tToday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                    if (DateTime.Now.Hour > 5)
                        Session["PressRunPubDate"] = tToday.AddDays(1);
                    else
                        Session["PressRunPubDate"] = tToday;
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

        private void CreateDropDowns()
        {
            CCDBaccess db = new CCDBaccess();

            string errmsg = "";
           
            ArrayList al = db.GetPubDateList(GetCurrentPressIDList(), out errmsg);

            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue("Item1");
            if (item == null)
                return;
            DropDownList1 = (DropDownList)item.FindControl("PubDateFilter");

            string currentSelection = "";
            if (DropDownList1.Items.Count > 0)
                currentSelection = DropDownList1.SelectedItem.Value;
            DropDownList1.Items.Clear();
            
            DropDownList1.Items.Add(new ListItem("All", "All")); // 0
            foreach (DateTime dt in al)
            {
                DropDownList1.Items.Add(new ListItem(dt.ToShortDateString(), PubDate2String(dt))); // 0
            }
          
            string s = PubDate2String((DateTime)Session["PressRunPubDate"]);

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

		public void OnSelChangePubDate(object sender, System.EventArgs e) 
		{
			System.Web.UI.WebControls.DropDownList dropdown = (System.Web.UI.WebControls.DropDownList)sender;
	
			Session["PressRunPubDate"] = String2PubDate(dropdown.SelectedItem.Value);
			DataGrid1.CurrentPageIndex = 0;
			DoDataBind();
		}

		public void DoDataBind()
		{
			CCDBaccess db = new CCDBaccess();

			string errmsg = "";
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
			if (sdt == "All")
				return DateTime.MinValue;

			string [] sargs = sdt.Split('-');
            if (sargs.Length < 3)
                return DateTime.MinValue;

			DateTime dt = new DateTime(Int32.Parse(sargs[2]),Int32.Parse(sargs[1]),Int32.Parse(sargs[0]),0,0,0);
			return dt;
		}

        private string LookupChannelGroupAlias(int channelID)
        {
            DataTable dt = (DataTable) HttpContext.Current.Cache["ChannelNameCache"];
            foreach (DataRow row in dt.Rows)
			{
			    if ((int)row["ID"] == channelID) 
                {
                    return (string)row["ChannelNameAlias"];

                }
            }
      
            return "";
        }

        private void UniqueChannelsForView(DataView dv, ref ArrayList uniqueChannels)
        {
            uniqueChannels.Clear();
            foreach (DataRowView row in dv)
            {
                string channelListString = (string)row["OrderNumber"];
                string[] chns = channelListString.Split(',');
                foreach(string s in chns)
                {
                    s.Trim();
                    bool found = false;
                    foreach(string s1 in uniqueChannels)
                    {
                        if (s1 == s)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found == false)
                        uniqueChannels.Add(s);
                }
            }
        }

		public ICollection CreateRunDataSource(DataTable dstable)
		{			
			bool hideEdition = Globals.GetCacheRowCount("EditionNameCache") < 2;
			bool hideSection = Globals.GetCacheRowCount("SectionNameCache") < 2;
			bool hideLocation = Globals.GetCacheRowCount("LocationNameCache") < 2;

			DataTable dt = new DataTable();

			DataColumn newColumn;
            newColumn = dt.Columns.Add("Press", Type.GetType("System.String"));
			newColumn = dt.Columns.Add("State",Type.GetType("System.String"));
            
            newColumn = dt.Columns.Add("PubDate", Type.GetType("System.String"));
			newColumn = dt.Columns.Add("Publication",Type.GetType("System.String"));

			newColumn = dt.Columns.Add("Edition",Type.GetType("System.String"));
			newColumn = dt.Columns.Add("Section",Type.GetType("System.String"));
					
			newColumn = dt.Columns.Add("Input",Type.GetType("System.String"));
			newColumn = dt.Columns.Add("Approved",Type.GetType("System.String"));
			newColumn = dt.Columns.Add("Output",Type.GetType("System.String"));
          
           // newColumn = dt.Columns.Add("ChannelStatus", Type.GetType("System.String"));

			DataView dv = dstable.DefaultView;

            ArrayList uniqueChannels = new ArrayList();
            ArrayList channelColumnNames = new ArrayList();
            UniqueChannelsForView(dv, ref uniqueChannels);
            int nChannelColumn = 0;
            foreach (string s in uniqueChannels)
            {
                if (nChannelColumn >= MAXCHANNELCOLUMNS)
                    break;
                int nch = Globals.TryParse(s, 0);
                if (nch > 0)
                {
                    string ss = LookupChannelGroupAlias(nch);
                    channelColumnNames.Add(ss);
                    newColumn = dt.Columns.Add("C" + nChannelColumn.ToString(), Type.GetType("System.Int32"));		
                    newColumn.Caption = ss;
                    nChannelColumn++;
                }
            }

            for (int i = nChannelColumn; i < MAXCHANNELCOLUMNS; i++)
            {
                newColumn = dt.Columns.Add("C" + i.ToString(), Type.GetType("System.Int32"));
                newColumn.Caption = "";
            }
             
            Session["PressRunChannelList"] = channelColumnNames;


			//string	errstr = "";
			string	prevPress = "";
			string  prevPublication = "";
			string	prevEdition = "";
			string	prevSection = "";
			string	prevIssue = "";
			DateTime prevPubDate = new DateTime();
		//	string	prevDeadline = "";
			string  prevLocation = "";
            string prevOrderNumber = "";

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
                prevOrderNumber = (string)row["OrderNumber"];
                prevSection = (string)row["Section"];
                nPageCountTotal = (int)row["Pages"];
                
                dr["State"] = (int)row["Hold"] == 1 ? "On hold" : "Released";
                if (hideLocation)
                    dr["Press"] = prevPress;
                else
                    dr["Press"] = prevPress + " (" + prevLocation + ")";

                dr["PubDate"] = PubDate2String(prevPubDate);
                dr["Publication"] = prevPublication;

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
                int nInputFTPCount, nInputPRECount, nInputINKCount, nInputRIPCount;

                db.GetPageStat(prevLocation, prevPress, prevPubDate, prevPublication, prevIssue, prevEdition, prevSection, 
                    out nPageCountPolled, out nApprovalCount, out nPagesWithError,
                    out nInputFTPCount, out nInputPRECount, out nInputINKCount, out nInputRIPCount, out nPageCountTotal,
                    out  errmsg);


                dr["Input"] = nPageCountPolled.ToString() + ofString + nPageCountTotal.ToString();
                dr["Approved"] = nApprovalCount.ToString() + ofString + nPageCountTotal.ToString();
                devices = "";

                List<ChannelProgress> channelProgressList = new List<ChannelProgress>();
                db.GetExportStatus(prevPublication, prevPubDate, prevEdition, prevSection, ref channelProgressList, out errmsg);




                db.GetExportDone(prevPublication, prevPubDate, prevEdition, prevSection, out  nPlateCountTotal, out  nPlateCountImaged, out  nPlateWithError,out  errmsg);
                
              
                dr["Output"] = nPlateCountImaged.ToString() + ofString + nPlateCountTotal.ToString();
                int pressID = Globals.GetIDFromName("PressNameCache", prevPress);
                int productionID = db.GetProductionID(ref pressID,
                                                   Globals.GetIDFromName("PublicationNameCache", prevPublication),
                                                    prevPubDate,
                                                    Globals.GetIDFromName("EditionNameCache", prevEdition), out errmsg);

                for (int i=0; i<channelColumnNames.Count; i++)
                    dr["C"+i.ToString()] = -1;

                string[] chns = prevOrderNumber.Split(',');
                foreach (string sc in chns)
                {
                    int chID = Globals.TryParse(sc, 0);
                    if (chID > 0)
                    {
                        int chStatus = db.GetChannelExportStatus(productionID, 0, chID, out errmsg);
                        // Catch illegal status..!
                        if ((chStatus == 10 || chStatus == 9 ) && nPageCountPolled < nPageCountTotal)
                            chStatus = 5;
                       // if (chStatus == 0)
                        //    continue;
                        string alias = LookupChannelGroupAlias(chID);
                        for (int j = 0; j < channelColumnNames.Count; j++)
                        {
                            if ((string)channelColumnNames[j] == alias)
                            {
                                dr["C" + j.ToString()] = chStatus;
                                break;
                            }
                        }

                    }
                }
           
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
                e.Item.Cells[PRESSRUN_FIELD_PRESS].Text = Global.rm.GetString("txtPress"); 
                e.Item.Cells[PRESSRUN_FIELD_STATE].Text = Global.rm.GetString("txtState");
                
                e.Item.Cells[PRESSRUN_FIELD_PUBDATE].Text = Global.rm.GetString("txtPubDate");
                e.Item.Cells[PRESSRUN_FIELD_PUBLICATION].Text = Global.rm.GetString("txtPublication");
                e.Item.Cells[PRESSRUN_FIELD_EDITION].Text = Global.rm.GetString("txtEdition");
                e.Item.Cells[PRESSRUN_FIELD_SECTION].Text = Global.rm.GetString("txtSection");
              
                e.Item.Cells[PRESSRUN_FIELD_INPUT].Text = Global.rm.GetString("txtInput");
                e.Item.Cells[PRESSRUN_FIELD_APPROVED].Text = Global.rm.GetString("txtApproved");

                e.Item.Cells[PRESSRUN_FIELD_OUTPUT].Text = Global.rm.GetString("txtTransmitted");
                ArrayList channelColumnNames = (ArrayList)Session["PressRunChannelList"];

                for (int i = 0; i < channelColumnNames.Count; i++)
                {
                    e.Item.Cells[PRESSRUN_FIELD_CHANNELOFFSET + i].Text = (string)channelColumnNames[i];
                    e.Item.Cells[PRESSRUN_FIELD_CHANNELOFFSET + i].Width = 40;
                    e.Item.Cells[PRESSRUN_FIELD_CHANNELOFFSET + i].Visible = true;

                }
                for (int i = channelColumnNames.Count; i < MAXCHANNELCOLUMNS; i++)
                {
                    e.Item.Cells[PRESSRUN_FIELD_CHANNELOFFSET + i].Visible = false;
                }
            }

			if ((e.Item.ItemType == ListItemType.Item) ||
				(e.Item.ItemType == ListItemType.AlternatingItem) ||
				(e.Item.ItemType == ListItemType.SelectedItem))
			{

               /* string publication = e.Item.Cells[PRESSRUN_FIELD_PUBLICATION].Text;
                if (publication.IndexOf("#") != -1)
                {
                    e.Item.Cells[PRESSRUN_FIELD_PUBLICATION].Text = publication.Replace("#", "");
                    e.Item.Cells[PRESSRUN_FIELD_PUBLICATION].ForeColor = Color.DarkRed;
                    e.Item.Cells[PRESSRUN_FIELD_EDITION].ForeColor = Color.DarkRed;
                    e.Item.Cells[PRESSRUN_FIELD_SECTION].ForeColor = Color.DarkRed;
                    e.Item.Cells[PRESSRUN_FIELD_PUBDATE].ForeColor = Color.DarkRed;
                }*/
                
                Label label = (Label)e.Item.FindControl("labelState");
				//label.BackColor = (e.Item.Cells[2].Text == "Released") ? Color.Green : Color.Red;
				label.ForeColor = (label.Text == "Released") ? Color.Black : Color.White;
				Panel panel = (Panel)e.Item.FindControl("panelState");
				panel.Width = 80;

                if ((bool)Application["FlatLook"] == false)
                    panel.BackImageUrl = (label.Text == "Released") ? "../Images/greengradient2.gif" : "../Images/redgradient2.gif";
                else
                    label.BackColor = (label.Text == "Released") ? Color.LawnGreen : Color.Red;

			//	label.Text =  (label.Text == "Released") ? Global.rm.GetString("txtReleased") : Global.rm.GetString("txtOnHold");
                label.Text = (label.Text == "Released") ? Global.rm.GetString("txtApproved") : Global.rm.GetString("txtNotApproved");


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
				panel.Width = PROGRESSBAR_WIDTH;
				panel.BackImageUrl = "PressRunsImages.aspx?n1="+n1.ToString()+"&n2="+n2.ToString();

                string markupwhiteatart = "<span ><image src='../Images/ColorGray_Flat.gif' alt='red' /></span>";
                string markupyellowatart = "<span ><image src='../Images/ColorY_Flat.gif' alt='red' /></span>";
                string markupgreenatart = "<span ><image src='../Images/ColorGreen_Flat.gif' alt='red' /></span>";
				string markupgredatart = "<span><image src='../Images/ColorRed_Flat.gif' alt='red' /> </span>";
                ArrayList al = (ArrayList)Session["PressRunChannelList"];
                for (int i=0; i<al.Count; i++)
                {
                    string chstatus = e.Item.Cells[PRESSRUN_FIELD_CHANNELOFFSET+i].Text;
                    int nState = Globals.TryParse(chstatus, 0);
                    string finalstring = "";
                    if (nState == 5)
                        finalstring = markupyellowatart ;
                    else if (nState == 10 || nState == 9)
                      finalstring = markupgreenatart; 
                    else if (nState == 6)
                        finalstring = markupgredatart;
                    else if (nState == 0)
                        finalstring = markupwhiteatart;
                    e.Item.Cells[PRESSRUN_FIELD_CHANNELOFFSET+i].Text = finalstring;
                }

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
					if (e.CommandName == "Go")
				{
					if (db.ApproveAll(Globals.GetIDFromName("PublicationNameCache",Publication),dt,Globals.GetIDFromName("EditionNameCache",Edition),Globals.GetIDFromName("SectionNameCache",Section),(string)Session["Username"],"Bulk approval",true, out errmsg) == false)
					{
						lblError.Text = errmsg;
						lblError.ForeColor = Color.Red;
					}
					else
					{
						DoDataBind();
					}
				}
				else if (db.UnApproveAll(Globals.GetIDFromName("PublicationNameCache",Publication),dt,Globals.GetIDFromName("EditionNameCache",Edition),Globals.GetIDFromName("SectionNameCache",Section),(string)Session["Username"],"Bulk approval",true, out errmsg) == false)
				{
					lblError.Text = errmsg;
					lblError.ForeColor = Color.Red;
				}
				else
				{
					DoDataBind();
				}
			} 
			else if (e.CommandName == "Retransmit")
			{
				int nNextIndex = 1;
				string Press = Press = e.Item.Cells[nNextIndex++].Text;

				int m = Press.IndexOf("("); 
				if (m != -1)
					Press = Press.Substring(0,m).Trim();
				nNextIndex+=1; // Skip state
				
				
				DateTime dt = String2PubDate(e.Item.Cells[nNextIndex++].Text);
				string Publication = e.Item.Cells[nNextIndex++].Text;
				string Edition = e.Item.Cells[nNextIndex++].Text;
				string Section = e.Item.Cells[nNextIndex++].Text;
				nNextIndex++; // skip input
				nNextIndex++; // skip approved
				nNextIndex++; // skip output
				string channeList = e.Item.Cells[nNextIndex++].Text;

				string channelIDList = "";
				string [] sch = channeList.Split(',');
				foreach (string ch in sch)
				{
					int nch = Globals.GetIDFromName("ChannelNameCache", ch);
					if (nch > 0) 
					{
						if (channelIDList != "")
							channelIDList += ",";
						channelIDList += nch.ToString();
					}
				}
				CCDBaccess db = new CCDBaccess();

				string errmsg = "";
				int nPublicationID = Globals.GetIDFromName("PublicationNameCache",Publication);
				int nPressID = Globals.GetIDFromName("PressNameCache",Press);

				int nProductionID = db.GetProductionID(ref nPressID, nPublicationID, dt, Globals.GetIDFromName("EditionNameCache",Edition), out errmsg);

/*				if (db.RetransmitAll(Globals.GetIDFromName("PublicationNameCache",Publication),
									dt,
									Globals.GetIDFromName("EditionNameCache",Edition),
									Globals.GetIDFromName("SectionNameCache",Section), out errmsg) == false)
				{
					lblError.Text = errmsg;
					lblError.ForeColor = Color.Red;
				}
*/
				prioString = "\"RetransmitChannels.aspx?MasterCopySeparationSet=0&Channels="+channelIDList+"&ProductionID="+nProductionID.ToString()+"&PublicationID="+nPublicationID.ToString() + "\"";

				Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowRetransmitChannels");
				mywindow.NavigateUrl = "RetransmitChannels.aspx?Channels="+channelIDList+"&ProductionID="+nProductionID.ToString()+"&PublicationID="+nPublicationID.ToString()+"&MasterCopySeparationSet=0";

				mywindow.VisibleOnPageLoad = true;
				

				DoDataBind();
				


			}
			else if (e.CommandName == "ChangeChannels") 
			{
				if ((bool)Session["MayRelease"] == false) 
				{
					lblError.Text = "You do not have rights to change channels";
					lblError.ForeColor = Color.OrangeRed;
					return;
				}

				int nNextIndex = 1;
				string Press = e.Item.Cells[nNextIndex++].Text;

				int m = Press.IndexOf("("); 
				if (m != -1)
					Press = Press.Substring(0,m).Trim();
			

				
				nNextIndex+=1; // Skip state
				
				DateTime dt = String2PubDate(e.Item.Cells[nNextIndex++].Text);
				string Publication = e.Item.Cells[nNextIndex++].Text;				
				string Edition = e.Item.Cells[nNextIndex++].Text;
				string Section = e.Item.Cells[nNextIndex++].Text;
				nNextIndex++; // skip input
				nNextIndex++; // skip approved
				nNextIndex++; // skip output
				string channeList = e.Item.Cells[nNextIndex++].Text;

				string channelIDList = "";
				string [] sch = channeList.Split(',');
				foreach (string ch in sch)
				{
					int nch = Globals.GetIDFromName("ChannelNameCache", ch);
					if (nch > 0) 
					{
						if (channelIDList != "")
							channelIDList += ",";
						channelIDList += nch.ToString();
					}
				}
				//doPopupPrio = true;
				CCDBaccess db = new CCDBaccess();
				string errmsg = "";

				int nPublicationID = Globals.GetIDFromName("PublicationNameCache",Publication);
				int nPressID = Globals.GetIDFromName("PressNameCache",Press);
				int nProductionID = db.GetProductionID(ref nPressID,					
														nPublicationID,dt,Globals.GetIDFromName("EditionNameCache",Edition), out errmsg);
				prioString = "\"ChangeChannels.aspx?MasterCopySeparationSet=0&Channels="+channelIDList+"&ProductionID="+nProductionID.ToString()+"&PublicationID="+nPublicationID.ToString() + "\"";

				Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowChannels");
				mywindow.NavigateUrl = "ChangeChannels.aspx?Channels="+channelIDList+"&ProductionID="+nProductionID.ToString()+"&PublicationID="+nPublicationID.ToString()+"&MasterCopySeparationSet=0";

				mywindow.VisibleOnPageLoad = true;

				DoDataBind();

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

            DataGrid1.Columns[PRESSRUN_FIELD_PRESS].Visible = false;

            ArrayList channelColumnNames = (ArrayList)Session["PressRunChannelList"];

            for (int i = 0; i < channelColumnNames.Count; i++)
            {
                DataGrid1.Columns[PRESSRUN_FIELD_CHANNELOFFSET + i].Visible = true;
            }
            for (int i = channelColumnNames.Count; i < MAXCHANNELCOLUMNS; i++)
            {
                e.Item.Cells[PRESSRUN_FIELD_CHANNELOFFSET + i].Visible = false;
            }

			if (Globals.GetCacheRowCount("SectionNameCache") < 2)
                DataGrid1.Columns[PRESSRUN_FIELD_SECTION].Visible = false;

			if (Globals.GetCacheRowCount("EditionNameCache") < 2)
                DataGrid1.Columns[PRESSRUN_FIELD_EDITION].Visible = false;


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
