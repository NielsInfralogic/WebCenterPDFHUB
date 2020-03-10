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
using System.Text;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Configuration;
using WebCenter4.Classes;

namespace WebCenter4
{
	/// <summary>
	/// Summary description for Menu.
	/// </summary>
	public partial class Menu : System.Web.UI.Page
	{
        protected bool updateTreeMenu { get; set; } = false;

		private void Page_Load(object sender, System.EventArgs e)
		{			
			if (!IsPostBack) 
			{	
				SetLanguage();
                Telerik.Web.UI.RadToolBarButton item = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("HideOld");
                if (item != null)
                    item.Checked = (bool)Session["HideOld"];


                if ((bool)Application["ShowCustomMenu"] && (bool)Session["IsAdmin"])
                {
                    item = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("CustomMenu");
                    if (item != null)
                    {
                        item.Visible = true;
                        item.Text = (string)Application["ShowCustomMenuName"];
                    }
                }
                RefreshPubDateFilter();
               /* bool setPressRunDate = false;
                if (Session["PubDateFilter"] == null)
                    setPressRunDate = true;
                else if ((DateTime)Session["PubDateFilter"] == DateTime.MinValue)
                    setPressRunDate = true;

                if (Session["PubDateFilter"] != null)
                {
                    ArrayList alFilterList = (ArrayList)Session["PubDateFilter"];
                    if (alFilterList != null)
                    {
                        foreach ()
                    }
                    DateTime tToday = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                    if (DateTime.Now.Hour > 5)
                        Session["PressRunPubDate"] = tToday.AddDays(1);
                    else
                        Session["PressRunPubDate"] = tToday;
                }*/



			}

		}

        private string LocalDateFormat(DateTime thisPubDate)
        {
            if ((int)Application["PubDateFormat"] == 1)
                return  string.Format("{0:00}.{1:00}.{2:0000}", thisPubDate.Day, thisPubDate.Month, thisPubDate.Year);
            else if ((int)Application["PubDateFormat"] == 3)
                return  string.Format("{0:00}.{1:00}.{2:00}", thisPubDate.Day, thisPubDate.Month, thisPubDate.Year - 2000);
            else if ((int)Application["PubDateFormat"] == 4)
                return  string.Format("{0:00}-{1:00}-{2:0000}", thisPubDate.Day, thisPubDate.Month, thisPubDate.Year - 2000);
            else if ((int)Application["PubDateFormat"] == 5)
                return  string.Format("{0:00}-{1:00}-{2:0000}", thisPubDate.Day, thisPubDate.Month, thisPubDate.Year);
            else if ((int)Application["PubDateFormat"] == 6)
                return  string.Format("{0:00}-{1:00}-{2:00}", thisPubDate.Day, thisPubDate.Month, thisPubDate.Year - 2000);
            else if ((int)Application["PubDateFormat"] == 7)
                return  string.Format("{0:00}/{1:00}/{2:0000}", thisPubDate.Day, thisPubDate.Month, thisPubDate.Year);
            else if ((int)Application["PubDateFormat"] == 8)
                return  string.Format("{0:00}/{1:00}/{2:00}", thisPubDate.Day, thisPubDate.Month, thisPubDate.Year - 200);

            return thisPubDate.ToShortDateString();
        }

        private void RefreshPubDateFilter()
        {
            string allString = Global.rm.GetString("txtAll");
            string weekString = Global.rm.GetString("txtWeek");
            string hideOldString = Global.rm.GetString("txtHideOld");
            CCDBaccess db = new CCDBaccess();

            string errmsg = "";
            ArrayList weekList = new ArrayList();
            ArrayList weekListDate = new ArrayList();


            ArrayList al = db.GetPubDateList(GetCurrentPressIDList(), false, ref weekList, ref weekListDate, out errmsg);

            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue("Item1");
            if (item == null)
                return;
            DropDownList DropDownList1 = (DropDownList)item.FindControl("PubDateFilter");

            string currentSelection = "";
            if (DropDownList1.Items.Count > 0)
                currentSelection = DropDownList1.SelectedItem.Value;
            DropDownList1.Items.Clear();

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
                for (int i=0; i<weekList.Count; i++)
                    DropDownList1.Items.Add(new ListItem(string.Format("{0} {1:00}", weekString, weekList[i]), PubDate2String((DateTime)weekListDate[i])));
            }

            string s = PubDate2String((DateTime)Session["PubDateFilter"]);

            if (DropDownList1.Items.FindByValue(currentSelection) != null)
                DropDownList1.SelectedValue = currentSelection;
            else if (DropDownList1.Items.FindByValue(s) != null)
                DropDownList1.SelectedValue = s;
            else
            {
                DateTime t = (DateTime)Session["PubDateFilter"];
                t = t.AddDays(-1);
                s = PubDate2String(t);
                if (DropDownList1.Items.FindByValue(s) != null)
                {
                    DropDownList1.SelectedValue = s;
                    Session["PubDateFilter"] = t;
                }
                else
                {
                    DropDownList1.SelectedIndex = 0;
                    Session["PubDateFilter"] = ( (bool)Session["HideOld"]) ? DateTime.MaxValue : DateTime.MinValue;                         
                }
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

            if (sPressIdList == "" || sPressIdList == "*")
                sPressIdList = "1";
            return sPressIdList;

        }

        string PubDate2String(DateTime dt)
        {
            if (dt == DateTime.MinValue)
                return "All";
            if (dt == DateTime.MaxValue)
                return "Hide old";
            return dt.Day.ToString() + "-" + dt.Month.ToString() + "-" + dt.Year.ToString();
        }

        DateTime String2PubDate(string sdt)
        {
            if (sdt == "All")
                return DateTime.MinValue;

            if (sdt == "Hide old")
                return DateTime.MaxValue;

            string[] sargs = sdt.Split('-');
            if (sargs.Length < 3)
                return DateTime.MinValue;

            DateTime dt = new DateTime(Int32.Parse(sargs[2]), Int32.Parse(sargs[1]), Int32.Parse(sargs[0]), 0, 0, 0);
            return dt;
        }

		protected void SetLanguage()
		{
			Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
			Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            SetRadToolbarLabel("User", Global.rm.GetString("txtMyProfile"));
            SetRadToolbarLabel("Help", Global.rm.GetString("txtHelp"));
            SetRadToolbarLabel("Logout", Global.rm.GetString("txtLogout"));
            SetRadToolbarLabel("About", Global.rm.GetString("txtAbout"));
            SetRadToolbarLabel("HideOld", Global.rm.GetString("txtHideOld"));

            SetRadToolbarTooltip("User", Global.rm.GetString("txtTooltipMyProfile"));
            SetRadToolbarTooltip("Help", Global.rm.GetString("txtTooltipHelp"));
            SetRadToolbarTooltip("Logout", Global.rm.GetString("txtTooltipLogout"));
            SetRadToolbarTooltip("About", Global.rm.GetString("txtTooltipAbout"));
            SetRadToolbarTooltip("HideOld", Global.rm.GetString("txtTooltipHideOld"));

            SetRadToolbarLabel("Label", "LabelContent", (string)Session["CurrentProduct"]);

            SetRadToolbarLabel("Item1", "txtPubDate", Global.rm.GetString("txtDate"));
            SetRadToolbarTooltip("Item1", "txtPubDate", Global.rm.GetString("txtPubDate"));
		}

        
        public void OnSelChangePubDate(object sender, System.EventArgs e)
        {
            System.Web.UI.WebControls.DropDownList dropdown = (System.Web.UI.WebControls.DropDownList)sender;

            Session["PubDateFilter"] = String2PubDate(dropdown.SelectedItem.Value);

            Session["HideOld"] = (DateTime)Session["PubDateFilter"] == DateTime.MaxValue;

            updateTreeMenu = true;
            Session["RefreshTree"] = true;
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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

	
		private void LogUserLogout(string userName)
		{
			string errMsg = "";
			CCDBaccess db = new CCDBaccess();
			db.InsertUserHistory(userName,0,"", out errMsg);

		}

        protected void RadToolBar1_ButtonClick(object sender, Telerik.Web.UI.RadToolBarEventArgs e)
        {
            if (e.Item.Value == "HideOld")
            {
                Telerik.Web.UI.RadToolBarButton btn = (Telerik.Web.UI.RadToolBarButton)e.Item;
                if (btn != null)
                    Session["HideOld"] = btn.Checked;
                Session["PubDateFilter"] = ((bool)Session["HideOld"]) ? DateTime.MaxValue : DateTime.MinValue;     
                updateTreeMenu = true;
                Session["RefreshTree"] = true;
            }

           if (e.Item.Value == "CustomMenu")
           {
               doPopupCustomWindow();

           }
        }

        private void doPopupCustomWindow()
        {
            string popupScript =
                "<script language='javascript'>" +
                "var xpos = 100;" +
                "var ypos = 100;" +
                "if(window.screen) { xpos = (screen.width-300)/2; ypos = (screen.height-150)/2; }" +
                "var s = 'status=no,top='+ypos+',left='+xpos+',width=300,height=150';" +
                "var PopupWindow = window.open('" + (string)Application["ShowCustomMenuScript"] + "','" + (string)Application["ShowCustomMenuName"] + "',s);" +
                "if (parseInt(navigator.appVersion) >= 4) PopupWindow.focus();" +
                "</script>";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "PopupScript", popupScript);
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
