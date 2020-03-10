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
	public class ReimagePDF : System.Web.UI.Page
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

                if (db.GetPlateColors(copyFlatSeparationSet, ref alColors, out errmsg) == false)
                {
                    lblError.Text = errmsg;
                    return;
                }

                if (alColors.Count == 0)
                {
                    lblError.Text = errmsg;
                    return;
                }
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

            string errmsg = "";

            if ((bool)Application["LocationIsPress"])
            {
                if (CheckboxSendToAll.Checked)
                    db.RetransmitCustom(nCopyFlatSeparationSet, "PDF", out errmsg);
                else
                    db.RetransmitFlatSeparationSet(nCopyFlatSeparationSet, "PDF", out errmsg);

                if (CheckBoxRelease.Checked)
                {
                    if (CheckboxSendToAll.Checked)
                        db.CustomRelease(nCopyFlatSeparationSet, 0, 0, out errmsg);
                    else
                        db.ReleaseCopyFlatSeparationSetTX(nCopyFlatSeparationSet, out errmsg);
                }
            }
            else
            {
                db.ReimageFlatSeparationSet(nCopyFlatSeparationSet, 1, "PDF", out errmsg);
            }

            InjectScript.Text = "<script>RefreshParentPage()</" + "script>";
        }

        protected void btnCancel_Click(object sender, System.EventArgs e)
        {
            InjectScript.Text = "<script>CloseOnReload()</" + "script>";
        }
    }
}
