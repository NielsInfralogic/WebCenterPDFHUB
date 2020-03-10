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
using WebCenter4.Classes;
using System.Text;
using System.Globalization;
using System.Resources;
using System.Threading;


namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for ChangePriority.
	/// </summary>
	public class ShowPageHistory : System.Web.UI.Page
	{
        protected Telerik.Web.UI.RadToolBar RadToolBar1;
        protected Telerik.Web.UI.RadGrid RadGridPageHistory;
		protected System.Web.UI.WebControls.Label lblError;
		protected System.Web.UI.WebControls.Label lblHeader;
        protected global::System.Web.UI.HtmlControls.HtmlInputHidden hiddenMasterCopySeparationSet;


		protected int doClose;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			doClose = 0;
			lblError.Text = "";

            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);
            SetRadToolbarLabel("Item1", "LabelPageHistoryHeader", Global.rm.GetString("txtPageHistorye"));

			if (!this.IsPostBack)
			{
                int masterCopySeparationSet = GetMasterNumber(); // Sets page number label
                if (masterCopySeparationSet == 0)
                {
                    lblError.Text = "Error getting page reference number";
                    return;
                }			
				
				RebindGrid(masterCopySeparationSet);
				
			}

		}

        private int GetMasterNumber()
        {
            if (Request.QueryString["page"] != null)
            {
                try
                {
                    lblHeader.Text = Global.rm.GetString("txtPage") + " " + (string)Request.QueryString["page"];
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

		private bool RebindGrid(int masterCopySeparationSet)
		{
			lblError.Text = "";
			CCDBaccess db = new CCDBaccess();
			string errmsg = "";

			DataTable dt = db.GetPageHistory(masterCopySeparationSet, false, out errmsg);
			if (errmsg != "")
			{
				lblError.Text = errmsg;
				return false;
			}
			if (dt.HasErrors)
			{
				lblError.Text = "Retrieved datatable contains errors";
				return false;
			}

			if (dt.Rows.Count == 0)
			{
				lblError.Text = "No history avaliable for this page";
				return false;
			}

			RadGridPageHistory.DataSource = dt;
			RadGridPageHistory.DataBind();

			return true;
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
