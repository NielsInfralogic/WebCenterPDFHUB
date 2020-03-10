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
	public partial class ChangePrioritySeps : System.Web.UI.Page
	{

        protected int doClosePrio;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			doClosePrio = 0;
			if (!this.IsPostBack)
			{	
				if (Session["SelectedSeps"] == null) 
					doClosePrio = 1;
				if (Session["SelectedPrio"] == null) 
					doClosePrio = 1;
				if (Session["SelectedCopySeps"] == null)
					doClosePrio = 1;

				txtPrioValue.Text = ((int)Session["SelectedPrio"]).ToString();
				CheckBoxAllColors.Checked = (bool)Session["SelectedAllSeps"] == false;
				CheckBoxAllCopies.Checked = true;
			}
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);
            CheckBoxAllCopies.Text = Global.rm.GetString("txtAllCopies");
			CheckBoxAllCopies.ToolTip = Global.rm.GetString("txtTooltipAllCopies");

			CheckBoxAllPages.Text = Global.rm.GetString("txtAllPages");
			CheckBoxAllPages.ToolTip = Global.rm.GetString("txtTooltipAllPages");

			CheckBoxAllColors.Text = Global.rm.GetString("txtAllColors");
			CheckBoxAllColors.ToolTip = Global.rm.GetString("txtTooltipAllColors");

			lblPriority.Text = Global.rm.GetString("txtPriority");
			btnCancel.Text = Global.rm.GetString("txtCancel");
			bntApply.Text = Global.rm.GetString("txtApply");
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
			this.bntApply.Click += new System.EventHandler(this.bntApply_Click);
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

        protected void bntApply_Click(object sender, System.EventArgs e)
		{
			int nPrio = Globals.TryParse(txtPrioValue.Text, -1);	

			if (nPrio >= 0) 
			{
				CCDBaccess db = new CCDBaccess();

				string errmsg = "";
				db.UpdatePriorityEx(CheckBoxAllCopies.Checked ? (ArrayList)Session["SelectedCopySeps"] :(ArrayList)Session["SelectedSeps"] ,(ArrayList)Session["SelectedColors"], nPrio, CheckBoxAllColors.Checked, CheckBoxAllPages.Checked, CheckBoxAllCopies.Checked, out errmsg);
				
			}
			doClosePrio = 2;
		}

        protected void btnCancel_Click(object sender, System.EventArgs e)
		{
			doClosePrio = 1;
		}
	}
}
