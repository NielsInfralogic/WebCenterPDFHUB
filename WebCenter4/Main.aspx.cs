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
using System.Configuration;
using System.Text;
using System.Globalization;
using System.Resources;
using System.Threading;
using WebCenter4.Classes;

namespace WebCenter4
{
	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	/// 
	
	public partial class Main : System.Web.UI.Page
	{
       
        protected int nWindowWidth;
		protected int nWindowHeight;

		private void Page_Load(object sender, System.EventArgs e)
		{

		    if (!IsPostBack) 
		    {
                RadTabStrip1.SelectedIndex = (int)Session["TabSelected"];
                RadMultiPage1.SelectedIndex = (int)Session["TabSelected"];
                SetLanguage();
            }

            SetScreenSize();
		}

        private void SetScreenSize()
        {
            int w = 0;
            int h = 0;
            if (HiddenX.Value != "" && HiddenY.Value != "")
            {
                w = Globals.TryParse(HiddenX.Value, 0);
                h = Globals.TryParse(HiddenY.Value, 0);
            }

            if (w <= 0 || h <= 0)
            {
                w = Globals.TryParseCookie(Request, "ScreenWidth", 600);
                h = Globals.TryParseCookie(Request, "ScreenHeight", 400);
            }

            if (w <= 0)
                w = (int)Session["WindowWidth"] > 0 ? (int)Session["WindowWidth"] : 600;
            if (h <= 0)
                h = (int)Session["WindowHeight"] > 0 ? (int)Session["WindowHeight"] : 400;

            int isMac = Globals.TryParseCookie(Request, "IsMac", 0);

            Session["WindowHeight"] = h;
            Session["WindowWidth"] = w;
            Session["IsMac"] = isMac == 1 ? true : false;

            RadMultiPage1.Height = h;
            if (!IsPostBack)
            {
                if ((bool)Session["IsMac"] == true)
                {
                    RadTabStrip1.Height = h;
                    RadTabStrip1.Width = w;
                }
            } 
        }

		protected void SetLanguage()
		{
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            CultureInfo curture = new CultureInfo((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = curture;

			RadTabStrip1.Tabs.FindTabByValue("Pages").Text = Global.rm.GetString("txtPages");
            RadTabStrip1.Tabs.FindTabByValue("Pages").ToolTip = Global.rm.GetString("txtTooltipPages");
            RadTabStrip1.Tabs.FindTabByValue("Spreads").Text = Global.rm.GetString("txtSpreads");
            RadTabStrip1.Tabs.FindTabByValue("Spreads").ToolTip = Global.rm.GetString("txtTooltipSpreads");
            RadTabStrip1.Tabs.FindTabByValue("Flats").Text = Global.rm.GetString("txtPlates");
            RadTabStrip1.Tabs.FindTabByValue("Flats").ToolTip = Global.rm.GetString("txtTooltipPlates");
            RadTabStrip1.Tabs.FindTabByValue("Status").Text = Global.rm.GetString("txtStatus");
            RadTabStrip1.Tabs.FindTabByValue("Status").ToolTip = Global.rm.GetString("txtStatusSeps");

            RadTabStrip1.Tabs.FindTabByValue("List").Text = Global.rm.GetString("txtList");
            RadTabStrip1.Tabs.FindTabByValue("List").ToolTip = Global.rm.GetString("txtTooltipList");

            RadTabStrip1.Tabs.FindTabByValue("Runs").Text = (bool)Application["UseChannels"] ? Global.rm.GetString("txtProgress") : Global.rm.GetString("txtRuns");
            RadTabStrip1.Tabs.FindTabByValue("Runs").ToolTip = (bool)Application["UseChannels"] ? Global.rm.GetString("txtProgress") : Global.rm.GetString("txtTooltipRuns");

            RadTabStrip1.Tabs.FindTabByValue("Plan").Text = Global.rm.GetString("txtPlan");
            RadTabStrip1.Tabs.FindTabByValue("Plan").ToolTip = Global.rm.GetString("txtTooltipPlan");
            RadTabStrip1.Tabs.FindTabByValue("Statistics").Text = Global.rm.GetString("txtStatistics");

            RadTabStrip1.Tabs.FindTabByValue("Statistics").ToolTip = Global.rm.GetString("txtTooltipStatistics");

            RadTabStrip1.Tabs.FindTabByValue("UnknownFiles").Text = Global.rm.GetString("txtUnknownFiles");
            RadTabStrip1.Tabs.FindTabByValue("UnknownFiles").ToolTip = Global.rm.GetString("txtTooltipUnknownFiles");

            RadTabStrip1.Tabs.FindTabByValue("Plan").Visible = (bool)Session["MayRunProducts"] && (bool)Application["HidePlanView"] == false;

            RadTabStrip1.Tabs.FindTabByValue("Spreads").Visible = (bool)Application["HideReadView"] == false;
            RadTabStrip1.Tabs.FindTabByValue("Status").Visible = (bool)Application["HideTableView"] == false;
            RadTabStrip1.Tabs.FindTabByValue("List").Visible = (bool)Application["HideListView"] == false;
            
            RadTabStrip1.Tabs.FindTabByValue("Flats").Visible = (bool)Application["HideFlatView"] == false;
            RadTabStrip1.Tabs.FindTabByValue("Runs").Visible = (bool)Application["HideRunView"] == false;
            RadTabStrip1.Tabs.FindTabByValue("Statistics").Visible = (bool)Application["HideStatistics"] == false;
            RadTabStrip1.Tabs.FindTabByValue("UnknownFiles").Visible = (bool)Application["HideUnknownFiles"] == false;
            RadTabStrip1.Tabs.FindTabByValue("Logs").Visible = (bool)Application["HideLogView"] == false;


            RadTabStrip1.Tabs.FindTabByValue("Upload").Visible = (bool)Application["AllowUpload"] && (bool)Session["MayUpload"];

            if ( (bool)Session["UserSpecificViews"] == true) 
			{
                if ((bool)Application["HideTableView"] == false)
                    RadTabStrip1.Tabs.FindTabByValue("Status").Visible = (bool)Session["UserView_ListView"];

                if ((bool)Application["HideReadView"] == false)
                    RadTabStrip1.Tabs.FindTabByValue("Spreads").Visible = (bool)Session["UserView_ReadView"];

                if ((bool)Application["HideFlatView"] == false)
                    RadTabStrip1.Tabs.FindTabByValue("Flats").Visible = (bool)Session["UserView_FlatView"];

                if ((bool)Application["HideRunView"] == false)
                    RadTabStrip1.Tabs.FindTabByValue("Runs").Visible = (bool)Session["UserView_PressView"];

                if ((bool)Application["HideStatistics"] == false)
                    RadTabStrip1.Tabs.FindTabByValue("Statistics").Visible = (bool)Session["UserView_ReportView"];

                if ((bool)Application["HideUnknownFiles"] == false)
                    RadTabStrip1.Tabs.FindTabByValue("UnknownFiles").Visible = (bool)Session["UserView_UnknownFiles"];

                if ((bool)Application["HidePlanView"] == false)
                    RadTabStrip1.Tabs.FindTabByValue("Plan").Visible = (bool)Session["UserView_PlanView"];
                
                if ((bool)Application["HideListView"] == false)
                    RadTabStrip1.Tabs.FindTabByValue("List").Visible = (bool)Session["UserView_ListView"];
            }
			else
			{
                if ((bool)Application["HideListView"] == false)
                {
                    if ((bool)Application["ListViewAdminOnly"] && (bool)Session["IsAdmin"] == false)
                        RadTabStrip1.Tabs.FindTabByValue("List").Visible = false;
                }

                
                if ((bool)Application["HideTableView"] == false)
				{
					if ((bool)Application["TableViewAdminOnly"] && (bool)Session["IsAdmin"] == false)
                        RadTabStrip1.Tabs.FindTabByValue("Status").Visible = false;
				}
                if ((bool)Application["HideReadView"] == false)
				{
					if ((bool)Application["ReadViewAdminOnly"] && (bool)Session["IsAdmin"] == false)
                        RadTabStrip1.Tabs.FindTabByValue("Spreads").Visible = false;
				}

                if ((bool)Application["HideFlatView"] == false)
				{
					if ((bool)Application["FlatViewAdminOnly"] && (bool)Session["IsAdmin"] == false)
                        RadTabStrip1.Tabs.FindTabByValue("Flats").Visible = false;
				}

                if ((bool)Application["HideRunView"] == false)
				{
					if ((bool)Application["RunViewAdminOnly"] && (bool)Session["IsAdmin"] == false)
                        RadTabStrip1.Tabs.FindTabByValue("Runs").Visible = false;
				}

                if ((bool)Application["HideStatistics"] == false)
				{
                    if ((bool)Application["StatisticsAdminOnly"] && (bool)Session["IsAdmin"] == false)
                        RadTabStrip1.Tabs.FindTabByValue("Statistics").Visible = false;
				}

                if ((bool)Application["HidePlanView"] == false)
				{
					if ((bool)Application["PlanViewAdminOnly"] && (bool)Session["IsAdmin"] == false)
                        RadTabStrip1.Tabs.FindTabByValue("Plan").Visible = false;
				}

                if ((bool)Application["HideUnknownFiles"] == false)
                {
                    if ((bool)Application["UnknownFilesAdminOnly"] && (bool)Session["IsAdmin"] == false)
                        RadTabStrip1.Tabs.FindTabByValue("UnknownFiles").Visible = false;
                }

                if ((bool)Application["HideLogView"] == false)
                {
                    if ((bool)Application["LogViewAdminOnly"] && (bool)Session["IsAdmin"] == false)
                        RadTabStrip1.Tabs.FindTabByValue("Logs").Visible = false;
                }

                if ((bool)Application["AllowUpload"] && (bool)Session["MayUpload"])
                {
                    if ((bool)Application["UploadAdminOnly"] && (bool)Session["IsAdmin"] == false)
                        RadTabStrip1.Tabs.FindTabByValue("Upload").Visible = false;
                }

			}

            if ((bool)Session["mobiledevice"])
            {
                RadTabStrip1.Tabs.FindTabByValue("UnknownFiles").Visible = false;
            }

            if ((bool)Application["spExists_spPageStatusList"] == false)
            {
                RadTabStrip1.Tabs.FindTabByValue("Status").Visible = false;
            }

            if ((string)Application["LogLocations"] == "")
            {
                RadTabStrip1.Tabs.FindTabByValue("Logs").Visible = false;
            }
			
          if ((bool)Application["SimpleFlatView"] == false)
              RadMultiPage1.FindPageViewByID("PageViewFlats").ContentUrl = "Views/Flatview2.aspx";

          if ((bool)Application["UseChannels"])
          {
              RadMultiPage1.FindPageViewByID("PageViewPages").ContentUrl = "Views/ThumbnailViewChannels.aspx";
              RadMultiPage1.FindPageViewByID("PageViewRuns").ContentUrl = "Views/PressRunsChannels.aspx";
          }
			
          if ((int)Application["UseExtendedPlanning"] == 1)
          {
              RadMultiPage1.FindPageViewByID("PageViewPlan").ContentUrl = "Views/PlanViewExtended.aspx";

          }
            if ((int)Application["UseExtendedPlanning"] == 2)
            {
                RadMultiPage1.FindPageViewByID("PageViewPlan").ContentUrl = "Views/PlanViewExtendedXML.aspx";

            }
            if ((int)Application["UseExtendedPlanning"] == 3)
            {
                RadMultiPage1.FindPageViewByID("PageViewPlan").ContentUrl = "Views/PlanViewPPM.aspx";

            }


        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		
			InitializeComponent();
//            RadTabStrip1.SelectedIndex = (int)Session["TabSelected"];
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

        protected void RadTabStrip1_TabClick(object sender, Telerik.Web.UI.RadTabStripEventArgs e)
		{
           // e.Tab.TabIndex
			if (e.Tab.Value.Equals("Pages"))
				Session["TabSelected"] = 0;
            else if (e.Tab.Value.Equals("Spreads"))
				Session["TabSelected"] = 1;
            else if (e.Tab.Value.Equals("Flats"))
				Session["TabSelected"] = 2;
            else if (e.Tab.Value.Equals("List"))
                Session["TabSelected"] =3;
            else if (e.Tab.Value.Equals("Status"))
                Session["TabSelected"] =4;
            else if (e.Tab.Value.Equals("Runs"))
				Session["TabSelected"] = 5;
            else if (e.Tab.Value.Equals("Plan"))
				Session["TabSelected"] = 6;
            else if (e.Tab.Value.Equals("Statistics"))
				Session["TabSelected"] = 7;
            else if (e.Tab.Value.Equals("UnknownFiles"))
                Session["TabSelected"] = 8;
            else if (e.Tab.Value.Equals("Logs"))
                Session["TabSelected"] = 9;
            else if (e.Tab.Value.Equals("Upload"))
                Session["TabSelected"] = 10;
            else
				Session["TabSelected"] = 0;

            RadMultiPage1.SelectedIndex = (int)Session["TabSelected"];
		}

	}
}
