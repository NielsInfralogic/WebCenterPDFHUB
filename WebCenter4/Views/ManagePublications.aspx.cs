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
	public partial class ManagePublications : System.Web.UI.Page
	{
       
		
        protected string newPubName;
        protected int doClose;

        private void Page_Load(object sender, System.EventArgs e)
		{
			doClose = 0;
			newPubName = "";

			if (!this.IsPostBack)
			{	
				txtLatestHour.Text = "0";
				CreatePageFormatDropDown();
				CreateProofDropDown();

				
			}

            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);
            btnCancel.Text = Global.rm.GetString("txtCancel");
			btnSave.Text = Global.rm.GetString("txtSave");

			lblNewPublicationName.Text = Global.rm.GetString("txtNewPublicationName");
			lblPageFormat.Text = Global.rm.GetString("txtDefaultPageFormat"); 
			lblDefaultSoftproof.Text = Global.rm.GetString("txtDefaultSoftproof");
			lblDefaultHardproof.Text = Global.rm.GetString("txtDefaultHardproof");
			lblApproveMethod.Text = Global.rm.GetString("txtDefaultApprove");
			RadioButtonListApprove.Items[0].Text = Global.rm.GetString("txtMustApprove");
			RadioButtonListApprove.Items[1].Text = Global.rm.GetString("txtNoApprovalRequired");
			lbl0disabled.Text = Global.rm.GetString("txt0disabled");

			lblLatestHour.Text = Global.rm.GetString("txtProductionLockTime");

			LinkButtonAddNewPageformat.Text = Global.rm.GetString("txtAddNew"); 

			lblTrimToFormat.Text = Global.rm.GetString("txtTrimToPageFormat");

            SetRadToolbarLabel("Item1", "LabelPublicationsHeader", Global.rm.GetString("txtAddNewPublicationName"));

            if (HiddenNewPageformat.Value != "0")
			{
				CreatePageFormatDropDown();
				if (ddlPageFormat.Items.FindByText(HiddenNewPageformat.Value) != null) 
					ddlPageFormat.SelectedValue = HiddenNewPageformat.Value;
			}

			HiddenNewPageformat.Value = "0";

		}


		private void CreatePageFormatDropDown()
		{
			DataSet ds = (DataSet) Cache["PageFormatCache"];
			DataTable table = ds.Tables[0];
			ddlPageFormat.Items.Clear();


			foreach (DataRow row in table.Rows)
			{
				ddlPageFormat.Items.Add((string)row["Name"]);
			}

		}

		private void CreateProofDropDown()
		{
			DataSet ds = (DataSet) Cache["ProofNameCache"];
			DataTable table = ds.Tables[0];
			ddlProofer.Items.Clear();

			foreach (DataRow row in table.Rows)
			{
				ddlProofer.Items.Add((string)row["Name"]);
			}

			ddHardProofer.Items.Clear();
			ddHardProofer.Items.Add("none");

			foreach (DataRow row in table.Rows)
			{
				ddHardProofer.Items.Add((string)row["Name"]);
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
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.LinkButtonAddNewPageformat.Click += new System.EventHandler(this.LinkButtonAddNewPageformat_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		
		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			doClose = 1;
		}

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			DataSet ds = (DataSet) Cache["PublicationNameCache"];
			DataTable table = ds.Tables[0];
			bool exists = false;
			foreach (DataRow row in table.Rows)
			{
				if (((string)row["Name"]).ToLower() == txtPublicationName.Text.ToLower()) 
				{
					exists = true;
					break;
				}
			}
			if (exists)
			{
				lblError.Text = "Publication name already exists";
				return;
			}
			
			CCDBaccess db = new CCDBaccess();

			string errmsg = "";

			int nPageFormatID = ddlPageFormat.SelectedIndex >= 0 ? Globals.GetIDFromName("PageFormatCache", ddlPageFormat.SelectedValue) : 0;

			int nProoferID = ddlProofer.SelectedIndex >= 0 ? Globals.GetIDFromName("ProofNameCache", ddlProofer.SelectedValue) : 0;
			int nHardProoferID = ddHardProofer.SelectedIndex > 0 ? Globals.GetIDFromName("ProofNameCache", ddHardProofer.SelectedValue) : 0;

			double latestHour = 0.0;
			try
			{
				latestHour = double.Parse(txtLatestHour.Text);
			}
			catch
			{
			}


			if (db.InsertPublicationName(txtPublicationName.Text, nPageFormatID, cbTrimToFormat.Checked, latestHour, nProoferID, nHardProoferID, RadioButtonListApprove.Items[0].Selected, out errmsg) == false) 
			{
				lblError.Text = errmsg;
				return;
			}

			Global.RefreshPublicationNameCache(null,null,0);

			newPubName = txtPublicationName.Text;
			doClose = 1;
		}

		private void LinkButtonAddNewPageformat_Click(object sender, System.EventArgs e)
		{
			doPopupPageformatWindow();
		}

		private void doPopupPageformatWindow()
		{
			string popupScript =
				"<script language='javascript'>" +
				"var xpos = 300;" + 
				"var ypos = 300;" +
				"if(window.screen) { xpos = (screen.width-490)/2; ypos = (screen.height-250)/2; }" + 
				"var s = 'status=no,top='+ypos+',left='+xpos+',width=490,height=250';" +
				"var PopupWindow = window.open('ManagePageFormats.aspx','Pageformats',s);" + 	
				"if (parseInt(navigator.appVersion) >= 4) PopupWindow.focus();" +
				"</script>";

			
            Page.ClientScript.RegisterStartupScript(this.GetType(), "PopupScript", popupScript, false);
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
