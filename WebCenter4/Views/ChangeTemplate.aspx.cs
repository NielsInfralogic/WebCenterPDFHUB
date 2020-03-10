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
	public partial class ChangeTemplate : System.Web.UI.Page
	{


        protected int doCloseTemplate;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			doCloseTemplate = 0;
			if (!this.IsPostBack)
			{	
				if (Session["SelectedTemplate"] == null) 
					doCloseTemplate = 1;
				if (Session["SelectedSeps"] == null)
					doCloseTemplate = 1;
			
				DataTable table = (DataTable) Cache["TemplateNameCache"];
				
				foreach (DataRow row in table.Rows)
				{
					DropDownList1.Items.Add((string)row["Name"]);
				}
				DropDownList1.SelectedValue = (string)Session["SelectedTemplate"];
			}
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);
            CheckBoxAllCopies.Text = Global.rm.GetString("txtAllCopies");
			CheckBoxAllCopies.ToolTip = Global.rm.GetString("txtTooltipAllCopies");
			lblTemplate.Text = Global.rm.GetString("txtTemplate");
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
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion

        protected void bntApply_Click(object sender, System.EventArgs e)
		{		
			int nTemplateID = Globals.GetIDFromName("TemplateNameCache",DropDownList1.SelectedValue);	

			if (nTemplateID > 0) 
			{
				CCDBaccess db = new CCDBaccess();

				string errmsg = "";
				db.UpdateTemplate((ArrayList)Session["SelectedSeps"] , nTemplateID, CheckBoxAllCopies.Checked, out errmsg);
				
			}
			doCloseTemplate = 2;
		}

        protected void btnCancel_Click(object sender, System.EventArgs e)
		{
			doCloseTemplate = 1;
		}
	}
}
