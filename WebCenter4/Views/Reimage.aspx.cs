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
	public class Reimage : System.Web.UI.Page
	{
        protected global::Telerik.Web.UI.RadButton bntApply;
        protected global::Telerik.Web.UI.RadButton btnCancel;
		protected System.Web.UI.WebControls.CheckBox CheckBoxC;
		protected System.Web.UI.WebControls.CheckBox CheckBoxM;
		protected System.Web.UI.WebControls.CheckBox CheckBoxY;
		protected System.Web.UI.WebControls.CheckBox CheckBoxK;
		protected System.Web.UI.WebControls.Label LabelHeader;
		protected System.Web.UI.WebControls.Label lblError;
		protected System.Web.UI.WebControls.CheckBox CheckboxCopy1;
		protected System.Web.UI.WebControls.CheckBox CheckboxCopy2;
		protected System.Web.UI.WebControls.CheckBox CheckboxCopy3;
		protected System.Web.UI.WebControls.CheckBox CheckboxCopy4;
		protected System.Web.UI.WebControls.Label InjectScript;
        protected System.Web.UI.WebControls.CheckBox CheckboxSendToAll;
        protected System.Web.UI.WebControls.CheckBox CheckBoxRelease;


	
		private void Page_Load(object sender, System.EventArgs e)
		{
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);
            btnCancel.Text = Global.rm.GetString("txtCancel");
			bntApply.Text = Global.rm.GetString("txtApply");
            LabelHeader.Text = Global.rm.GetString((bool)Application["LocationIsPress"] ? "txtRetransmitForm" : "txtReimageForm");
            CheckBoxRelease.Text = Global.rm.GetString("txtRelease");

            if ((bool)Application["LocationIsPress"])
            {
                CheckboxCopy1.Visible = false;
                CheckboxCopy2.Visible = false;
                CheckboxCopy3.Visible = false;
                CheckboxCopy4.Visible = false;
            }
            else
                CheckboxSendToAll.Visible = false;

          

            if (!this.IsPostBack)
			{	
				if (Session["SelectedCopyFlatSeparationSet"] == null) 
				{
					lblError.Text = "Session variable error";
					InjectScript.Text="<script>CloseOnReload()</" + "script>";
					return;
				}

				CCDBaccess db = new CCDBaccess();

				string errmsg = "";
				//string sPageName = "";
				ArrayList alColors = new ArrayList();
				ArrayList alActiveColors = new ArrayList();

				int copyFlatSeparationSet = (int)Session["SelectedCopyFlatSeparationSet"];

				if (db.GetPlateColors(copyFlatSeparationSet, ref  alColors, out errmsg) == false)
				{
					lblError.Text = errmsg;
					return;
				}

				if (alColors.Count == 0)
				{
					lblError.Text = errmsg;
					return;
				}

				int nCopies = 1;
				if (db.GetPlateCopies(copyFlatSeparationSet, ref  nCopies, out errmsg) == false)
				{
					lblError.Text = errmsg;
					return;
				}

				if (nCopies == 0)
				{
					lblError.Text = errmsg;
					return;
				}
				
				CheckboxCopy1.Visible = true;
				CheckboxCopy1.Checked = true;
				CheckboxCopy2.Visible = false;
				CheckboxCopy2.Checked = false;
				CheckboxCopy3.Visible = false;
				CheckboxCopy3.Checked = false;
				CheckboxCopy4.Visible = false;
				CheckboxCopy4.Checked = false;
				if (nCopies > 1)
					CheckboxCopy2.Visible = true;
				if (nCopies > 2)
					CheckboxCopy3.Visible = true;
				if (nCopies > 3)
					CheckboxCopy4.Visible = true;
				
				for(int i=0; i<alColors.Count; i++)
				{
					if ((string)alColors[i] == "C")
					{
						CheckBoxC.Visible = true;
						CheckBoxC.Checked = alColors.Count == 1;
					}
					if ((string)alColors[i] == "M")
					{
						CheckBoxM.Visible = true;
						CheckBoxM.Checked = alColors.Count == 1;;
					}
					if ((string)alColors[i] == "Y")
					{
						CheckBoxY.Visible = true;
						CheckBoxY.Checked = alColors.Count == 1;;
					}
					if ((string)alColors[i] == "K")
					{
						CheckBoxK.Visible = true;
						CheckBoxK.Checked = alColors.Count == 1;;
					}
				}
				ViewState["Cactive"] = CheckBoxC.Checked;
				ViewState["Mactive"] = CheckBoxM.Checked;
				ViewState["Yactive"] = CheckBoxY.Checked;
				ViewState["Kactive"] = CheckBoxK.Checked;

                CheckBoxRelease.Checked = true;

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
			int nCopyFlatSeparationSet = (int)Session["SelectedCopyFlatSeparationSet"];
				
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
				lblError.Text = "No colors selected";
				return;
			}

			string errmsg = "";

            bool hasChanged = false;
            if (CheckBoxC.Checked)
            {
                if ((bool)Application["LocationIsPress"])
                    db.ReimageFlatSeparationSet(nCopyFlatSeparationSet, 0, CheckBoxC.Text, out  errmsg);
                else
                {
                    if (CheckboxCopy1.Visible && CheckboxCopy1.Checked)
                        db.ReimageFlatSeparationSet(nCopyFlatSeparationSet, 1, CheckBoxC.Text, out  errmsg);

                    if (CheckboxCopy2.Visible && CheckboxCopy2.Checked)
                        db.ReimageFlatSeparationSet(nCopyFlatSeparationSet, 2, CheckBoxC.Text, out  errmsg);

                    if (CheckboxCopy3.Visible && CheckboxCopy3.Checked)
                        db.ReimageFlatSeparationSet(nCopyFlatSeparationSet, 3, CheckBoxC.Text, out  errmsg);

                    if (CheckboxCopy4.Visible && CheckboxCopy4.Checked)
                        db.ReimageFlatSeparationSet(nCopyFlatSeparationSet, 4, CheckBoxC.Text, out  errmsg);
                }

                if ((bool)ViewState["Cactive"] != CheckBoxC.Checked)
                    hasChanged = true;
            }
            if (CheckBoxM.Checked)
            {
                if ((bool)Application["LocationIsPress"])
                    db.ReimageFlatSeparationSet(nCopyFlatSeparationSet, 0, CheckBoxM.Text, out  errmsg);
                else
                {
                    if (CheckboxCopy1.Visible && CheckboxCopy1.Checked)
                        db.ReimageFlatSeparationSet(nCopyFlatSeparationSet, 1, CheckBoxM.Text, out  errmsg);

                    if (CheckboxCopy2.Visible && CheckboxCopy2.Checked)
                        db.ReimageFlatSeparationSet(nCopyFlatSeparationSet, 2, CheckBoxM.Text, out  errmsg);

                    if (CheckboxCopy3.Visible && CheckboxCopy3.Checked)
                        db.ReimageFlatSeparationSet(nCopyFlatSeparationSet, 3, CheckBoxM.Text, out  errmsg);

                    if (CheckboxCopy4.Visible && CheckboxCopy4.Checked)
                        db.ReimageFlatSeparationSet(nCopyFlatSeparationSet, 4, CheckBoxM.Text, out  errmsg);
                }

                if ((bool)ViewState["Mactive"] != CheckBoxM.Checked)
                    hasChanged = true;
            }

            if (CheckBoxY.Checked)
            {
                if ((bool)Application["LocationIsPress"])
                    db.ReimageFlatSeparationSet(nCopyFlatSeparationSet, 0, CheckBoxY.Text, out  errmsg);
                else
                {
                    if (CheckboxCopy1.Visible && CheckboxCopy1.Checked)
                        db.ReimageFlatSeparationSet(nCopyFlatSeparationSet, 1, CheckBoxY.Text, out  errmsg);

                    if (CheckboxCopy2.Visible && CheckboxCopy2.Checked)
                        db.ReimageFlatSeparationSet(nCopyFlatSeparationSet, 2, CheckBoxY.Text, out  errmsg);

                    if (CheckboxCopy3.Visible && CheckboxCopy3.Checked)
                        db.ReimageFlatSeparationSet(nCopyFlatSeparationSet, 3, CheckBoxY.Text, out  errmsg);

                    if (CheckboxCopy4.Visible && CheckboxCopy4.Checked)
                        db.ReimageFlatSeparationSet(nCopyFlatSeparationSet, 4, CheckBoxY.Text, out  errmsg);
                }
                if ((bool)ViewState["Yactive"] != CheckBoxY.Checked)
                    hasChanged = true;
            }
            if (CheckBoxK.Checked)
            {
                if ((bool)Application["LocationIsPress"])
                    db.ReimageFlatSeparationSet(nCopyFlatSeparationSet, 0, CheckBoxK.Text, out  errmsg);
                else
                {
                    if (CheckboxCopy1.Visible && CheckboxCopy1.Checked)
                        db.ReimageFlatSeparationSet(nCopyFlatSeparationSet, 1, CheckBoxK.Text, out  errmsg);

                    if (CheckboxCopy2.Visible && CheckboxCopy2.Checked)
                        db.ReimageFlatSeparationSet(nCopyFlatSeparationSet, 2, CheckBoxK.Text, out  errmsg);

                    if (CheckboxCopy3.Visible && CheckboxCopy3.Checked)
                        db.ReimageFlatSeparationSet(nCopyFlatSeparationSet, 3, CheckBoxK.Text, out  errmsg);

                    if (CheckboxCopy4.Visible && CheckboxCopy4.Checked)
                        db.ReimageFlatSeparationSet(nCopyFlatSeparationSet, 4, CheckBoxK.Text, out  errmsg);
                }

                if ((bool)ViewState["Kactive"] != CheckBoxK.Checked)
                    hasChanged = true;
            }

            /*
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
            */
            InjectScript.Text = "<script>RefreshParentPage()</" + "script>";
		}

        protected void btnCancel_Click(object sender, System.EventArgs e)
		{
			InjectScript.Text="<script>CloseOnReload()</" + "script>";
		}
	}
}
