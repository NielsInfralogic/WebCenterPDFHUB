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
	public partial class ChangeColorPDF : System.Web.UI.Page
	{

		private void Page_Load(object sender, System.EventArgs e)
		{
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);
            btnCancel.Text = Global.rm.GetString("txtCancel");
			bntApply.Text = Global.rm.GetString("txtApply");

			if (!this.IsPostBack)
			{	
				if (Session["SelectedMasterSet"] == null) 
				{
					lblError.Text = "Session variable error";
					InjectScript.Text="<script>CloseOnReload()</" + "script>";
					return;
				}

				CCDBaccess db = new CCDBaccess();

				string errmsg = "";
				string sPageName = "";
				ArrayList alColors = new ArrayList();
				ArrayList alActiveColors = new ArrayList();

				int nMasterCopySeparationSet = (int)Session["SelectedMasterSet"];

				if (db.GetPageColors(nMasterCopySeparationSet, out sPageName, ref  alColors, ref alActiveColors, out errmsg) == false)
				{
					lblError.Text = errmsg;
					return;
				}

				if (alColors.Count == 0)
				{
					lblError.Text = errmsg;
					return;
				}

				if ((string)alColors[0] == "PDFmono")
					RadioButtonListPDFcolors.SelectedIndex = 1;
				else
					RadioButtonListPDFcolors.SelectedIndex = 0;

				ViewState["PDFmode"] = RadioButtonListPDFcolors.SelectedIndex;
			
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
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion

        protected void bntApply_Click(object sender, System.EventArgs e)
		{
			int nMasterCopySeparationSet = (int)Session["SelectedMasterSet"];

			int nMasterCopySeparationSet2 = 0;
			
			if (Session["SelectedMasterSet2"] != null)
				nMasterCopySeparationSet2 = (int)Session["SelectedMasterSet2"];
				
			CCDBaccess db = new CCDBaccess();
			string errmsg = "";

			if ((int)ViewState["PDFmode"] != RadioButtonListPDFcolors.SelectedIndex) 
			{
				db.ChangePDFcolors(nMasterCopySeparationSet, RadioButtonListPDFcolors.SelectedIndex == 0 ? "PDF" : "PDFmono", out errmsg);

				if (nMasterCopySeparationSet2 > 0) 
					db.ChangePDFcolors(nMasterCopySeparationSet2, RadioButtonListPDFcolors.SelectedIndex == 0 ? "PDF" : "PDFmono", out errmsg);

				db.ResetApproval(nMasterCopySeparationSet, out errmsg);

				if (nMasterCopySeparationSet2 > 0)
					db.ResetApproval(nMasterCopySeparationSet2, out errmsg);
			}

			InjectScript.Text="<script>RefreshParentPage()</" + "script>";
		}

        protected void btnCancel_Click(object sender, System.EventArgs e)
		{
			InjectScript.Text="<script>CloseOnReload()</" + "script>";
		}
	}
}
