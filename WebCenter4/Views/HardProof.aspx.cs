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

namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for ChangeTemplate.
	/// </summary>
	public partial class HardProof : System.Web.UI.Page
	{

		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!this.IsPostBack)
			{	
				if (Session["SelectedMasterSet"] == null)
				{
					InjectScript.Text="<script>CloseOnReload()</" + "script>";
					return;
				}

				DataTable table = (DataTable) Cache["HardProofNameCache"];
			
				foreach (DataRow row in table.Rows)
				{
					DropDownList1.Items.Add((string)row["Name"]);
				}
				if (DropDownList1.Items.Count > 0)
					DropDownList1.SelectedIndex = 0;
			}
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);
            CheckBoxIgnoreApproval.Text = Global.rm.GetString("txtIgnoreApproval");
			CheckBoxIgnoreApproval.ToolTip = Global.rm.GetString("txtTooltipIgnoreApproval");

            if ((int)Session["SelectedCopyFlatSeparationSet"] > 0)
                CheckBoxIgnoreApproval.Visible = false;
            
            lblTemplate.Text = Global.rm.GetString("txtHardproofConfig");
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
			int nHardProofID = Globals.GetIDFromName("HardProofNameCache", DropDownList1.SelectedValue);	

			if (nHardProofID > 0) 
			{
				CCDBaccess	db = new CCDBaccess();

				string errmsg = "";
                if ((int)Session["SelectedMasterSet"] > 0)
                    db.UpdateHardProof((int)Session["SelectedMasterSet"], nHardProofID, CheckBoxIgnoreApproval.Checked, out errmsg);
                else
                    db.QueueFlatProof((int)Session["SelectedCopyFlatSeparationSet"], nHardProofID, out errmsg);
				
			}
			InjectScript.Text="<script>RefreshParentPage()</" + "script>";

		}

        protected void btnCancel_Click(object sender, System.EventArgs e)
		{
			InjectScript.Text="<script>CloseOnReload()</" + "script>";
		}
	}
}
