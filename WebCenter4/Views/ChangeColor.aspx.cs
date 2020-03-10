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
	public partial class ChangeColor : System.Web.UI.Page
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
                int nMasterCopySeparationSet = GetMasterNumber();
                if (nMasterCopySeparationSet == 0)
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


				string s = Global.rm.GetString("txtColorsForPage");
				LabelHeader.Text = s + " " + sPageName;
				CheckBoxC.Visible = false;
				CheckBoxM.Visible = false;
				CheckBoxY.Visible = false;
				CheckBoxK.Visible = false;
				CheckBoxC.Checked = false;
				CheckBoxM.Checked = false;
				CheckBoxY.Checked = false;
				CheckBoxK.Checked = false;
				
				for(int i=0; i<alColors.Count; i++)
				{
					if ((string)alColors[i] == "C")
					{
						CheckBoxC.Visible = true;
						if ((int)alActiveColors[i] > 0)
							CheckBoxC.Checked = true;
					}
					if ((string)alColors[i] == "M")
					{
						CheckBoxM.Visible = true;
						if ((int)alActiveColors[i] > 0)
							CheckBoxM.Checked = true;
					}
					if ((string)alColors[i] == "Y")
					{
						CheckBoxY.Visible = true;
						if ((int)alActiveColors[i] > 0)
							CheckBoxY.Checked = true;
					}
					if ((string)alColors[i] == "K")
					{
						CheckBoxK.Visible = true;
						if ((int)alActiveColors[i] > 0)
							CheckBoxK.Checked = true;
					}
				}
				ViewState["Cactive"] = CheckBoxC.Checked ;
				ViewState["Mactive"] = CheckBoxM.Checked;
				ViewState["Yactive"] = CheckBoxY.Checked ;
				ViewState["Kactive"] = CheckBoxK.Checked;
			
			}
		}

        private int GetMasterNumber()
        {
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

            if (Request.QueryString["mastercopyseparationset2"] != null)
            {
                try
                {
                    hiddenMasterCopySeparationSet2.Value = (string)Request.QueryString["mastercopyseparationset2"];
                }
                catch
                {
                   ;
                }
            }
           
            return Globals.TryParse(hiddenMasterCopySeparationSet.Value, 0);
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
            int nMasterCopySeparationSet = Globals.TryParse(hiddenMasterCopySeparationSet.Value, 0);
            int nMasterCopySeparationSet2 = Globals.TryParse(hiddenMasterCopySeparationSet2.Value, 0);
				
			CCDBaccess db = new CCDBaccess();

			bool anySelected = false;
			if (CheckBoxC.Visible && CheckBoxC.Checked)
				anySelected = true;
			if (CheckBoxM.Visible && CheckBoxM.Checked)
				anySelected = true;
			if (CheckBoxY.Visible && CheckBoxY.Checked)
				anySelected = true;
			if (CheckBoxK.Visible && CheckBoxK.Checked)
				anySelected = true;

			if (anySelected == false)
			{
				lblError.Text = "You cannot switch off all colors";
				return;
			}


			string errmsg = "";

			bool hasChanged = false;
			if (CheckBoxC.Visible)
			{
				db.UpdateActive(nMasterCopySeparationSet, CheckBoxC.Text, CheckBoxC.Checked ? 1 : 0, out errmsg);
				if ((bool)ViewState["Cactive"] != CheckBoxC.Checked)
					hasChanged = true;

			}
			if (CheckBoxM.Visible)
			{
				db.UpdateActive(nMasterCopySeparationSet, CheckBoxM.Text, CheckBoxM.Checked ? 1 : 0, out errmsg);
				if ((bool)ViewState["Mactive"] != CheckBoxM.Checked)
					hasChanged = true;
			}
			if (CheckBoxY.Visible)
			{
				db.UpdateActive(nMasterCopySeparationSet, CheckBoxY.Text, CheckBoxY.Checked ? 1 : 0, out errmsg);
				if ((bool)ViewState["Yactive"] != CheckBoxY.Checked)
					hasChanged = true;
			}
			if (CheckBoxK.Visible)
			{
				db.UpdateActive(nMasterCopySeparationSet, CheckBoxK.Text, CheckBoxK.Checked ? 1 : 0, out errmsg);
				if ((bool)ViewState["Kactive"] != CheckBoxK.Checked)
					hasChanged = true;
			}

			if (nMasterCopySeparationSet2 > 0) 
			{

				if (CheckBoxC.Visible)
				{
					db.UpdateActive(nMasterCopySeparationSet2, CheckBoxC.Text, CheckBoxC.Checked ? 1 : 0, out errmsg);
					if ((bool)ViewState["Cactive"] != CheckBoxC.Checked)
						hasChanged = true;

				}
				if (CheckBoxM.Visible)
				{
					db.UpdateActive(nMasterCopySeparationSet2, CheckBoxM.Text, CheckBoxM.Checked ? 1 : 0, out errmsg);
					if ((bool)ViewState["Mactive"] != CheckBoxM.Checked)
						hasChanged = true;
				}
				if (CheckBoxY.Visible)
				{
					db.UpdateActive(nMasterCopySeparationSet2, CheckBoxY.Text, CheckBoxY.Checked ? 1 : 0, out errmsg);
					if ((bool)ViewState["Yactive"] != CheckBoxY.Checked)
						hasChanged = true;
				}
				if (CheckBoxK.Visible)
				{
					db.UpdateActive(nMasterCopySeparationSet2, CheckBoxK.Text, CheckBoxK.Checked ? 1 : 0, out errmsg);
					if ((bool)ViewState["Kactive"] != CheckBoxK.Checked)
						hasChanged = true;
				}

			}

			if (hasChanged)
			{
				db.ResetProofed(nMasterCopySeparationSet, out errmsg);
				db.ResetApproval(nMasterCopySeparationSet, out errmsg);

				if (nMasterCopySeparationSet2 > 0)
				{
					db.ResetProofed(nMasterCopySeparationSet2, out errmsg);
					db.ResetApproval(nMasterCopySeparationSet2, out errmsg);
				}
			}

			InjectScript.Text="<script>RefreshParentPage()</" + "script>";
		}

        protected void btnCancel_Click(object sender, System.EventArgs e)
		{
			InjectScript.Text="<script>CloseOnReload()</" + "script>";
		}

	}
}
