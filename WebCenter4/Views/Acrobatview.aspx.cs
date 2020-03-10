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
using Telerik.Web.UI;

namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for Zoomview.
	/// </summary>
	public partial class Acrobatview : System.Web.UI.Page
	{
      

		public string sImagePath;
		public string sUseNotes;
		public string sColors;
		public int nCurrentCopySeparationSet;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ((string)Session["UserName"] == null)
				Response.Redirect("~/SessionTimeout.htm");

			if ((string)Session["UserName"] == "")
				Response.Redirect("/Denied.htm");


			sImagePath = (string)Session["ImagePath"];
            sUseNotes = (string)ConfigurationManager.AppSettings["UseNotes"];
			sColors = (string)Session["ImageColors"];
			nCurrentCopySeparationSet = (int)Session["CurrentCopySeparationSet"];

            DisplayPageName();
			
			if (!IsPostBack) 
			{	
				SetLanguage();
			}

            if ((bool)Application["NoCache"])
            {
                Response.AppendHeader("cache-control", "private");
                Response.AppendHeader("pragma", "no-cache");
                Response.AppendHeader("expires", "Fri, 30 Oct 1998 14:19:41 GMT");
                Response.CacheControl = "Private";
                Response.Cache.SetNoStore();
            }
		}

        private void DisplayPageName()
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue("Item3");
            if (item == null)
                return;
            Label lbl = (Label)item.FindControl("PageName");

            if (lbl == null)
                return;
            lbl.ForeColor = Color.Black;
            lbl.BackColor = Color.Transparent;
            string sApp = Global.rm.GetString("txtNoApprovalRequired");
            if ((int)Session["CurrentApprovalState"] == 0)
            {
                sApp = Global.rm.GetString("txtNotApproved");
                lbl.BackColor = Color.Yellow;
            }
            else if ((int)Session["CurrentApprovalState"] == 1)
            {
                sApp = Global.rm.GetString("txtApproved");
                lbl.BackColor = Color.LightGreen;
            }
            else if ((int)Session["CurrentApprovalState"] == 2)
            {
                sApp = Global.rm.GetString("txtRejected");
                lbl.BackColor = Color.Red;
            }

            lbl.Text = Global.rm.GetString("txtPage") + " " + (string)Session["CurrentPageName"] + " - " + sApp;

        }
		
		protected void SetLanguage()
		{
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            SetRadToolbarLabel("Approve", Global.rm.GetString("txtApprove"));
            SetRadToolbarTooltip("Approve", Global.rm.GetString("txtTooltipApprovePage"));
            SetRadToolbarEnable("Approve", (bool)Session["MayApprove"]);

            SetRadToolbarLabel("Disapprove", Global.rm.GetString("txtReject"));
            SetRadToolbarTooltip("Disapprove", Global.rm.GetString("txtTooltipDisapprovePage"));
            SetRadToolbarEnable("Disapprove", (bool)Session["MayApprove"]);

            SetRadToolbarLabel("Close", Global.rm.GetString("txtBack"));
            SetRadToolbarTooltip("Close", Global.rm.GetString("txtTooltipBack"));

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

        protected void RadToolBar1_ButtonClick(object sender, Telerik.Web.UI.RadToolBarEventArgs e)
		{
            if (e.Item.Value == "Approve")
			{
				string errmsg;
				CCDBaccess db = new CCDBaccess();

				if (db.UpdateApproval((string)Session["UserName"], nCurrentCopySeparationSet, 1, out errmsg) == false)
				{
                    ;
				}
				else
				{
					Session["CurrentApprovalState"] = 1;
                    DisplayPageName();
				}
			}

            if (e.Item.Value == "Disapprove")
			{
				string errmsg;
				CCDBaccess db = new CCDBaccess();

				if (db.UpdateApproval((string)Session["UserName"],nCurrentCopySeparationSet, 2, out errmsg) == false)
				{
                    ;
				}
				else
				{
					Session["CurrentApprovalState"] = 2;
                    DisplayPageName();
				}
			}

            if (e.Item.Value == "Close")
			{
				Response.Redirect("Thumbnailview2.aspx");
			}
		}

        private void SetRadToolbarLabel(string buttonID, string text)
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue(buttonID);
            if (item == null)
                return;
            item.Text = text;
        }

        private void SetRadToolbarEnable(string buttonID, bool enable)
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue(buttonID);
            if (item == null)
                return;
            item.Enabled = enable;
        }

        private void SetRadToolbarTooltip(string buttonID, string text)
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue(buttonID);
            if (item == null)
                return;
            item.ToolTip = text;
        }
	}
}
