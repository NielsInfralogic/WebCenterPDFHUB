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
	public partial class ManagePageFormats : System.Web.UI.Page
	{
      

        protected string newPageformatName;
        protected int doClose;
		private void Page_Load(object sender, System.EventArgs e)
		{
			doClose = 0;
			newPageformatName = "";

			if (!this.IsPostBack)
			{
                RadNumericEditBleed.Value = 0.0;
			
			}

            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);
            btnCancel.Text = Global.rm.GetString("txtCancel");
			btnSave.Text = Global.rm.GetString("txtSave");

			lblPageformatName.Text = Global.rm.GetString("txtNewPageformatName");
			lblPageformatWidth.Text = Global.rm.GetString("txtPageFormatWidth"); 
			lblPageformatHeight.Text = Global.rm.GetString("txtPageFormatHeight");
			lblPageformatBleed.Text = Global.rm.GetString("txtPageFormatBleed");
		
            SetRadToolbarLabel("Item1", "LabelPageformatHeader", Global.rm.GetString("txtAddNewPageformatName"));

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
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		
		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			doClose = 1;
		}

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			DataSet ds = (DataSet) Cache["PageFormatCache"];
			DataTable table = ds.Tables[0];
			bool exists = false;
			foreach (DataRow row in table.Rows)
			{
				if (((string)row["Name"]).ToLower() == txtPageformatName.Text.ToLower()) 
				{
					exists = true;
					break;
				}
			}
			if (exists)
			{
				lblError.Text = "Page format name already exists";
				return;
			}
			
			CCDBaccess db = new CCDBaccess();
			string errmsg = "";

            if (RadNumericEditWidth.Value <= 0 || RadNumericEditHeight.Value <= 0 || RadNumericEditBleed.Value < 0)
			{
				lblError.Text = "Invalid page format value(s)";
				return;
			}

            if (db.InsertPageformatName(txtPageformatName.Text, (double)RadNumericEditWidth.Value, (double)RadNumericEditHeight.Value, (double)RadNumericEditBleed.Value, out errmsg) == false) 
			{
				lblError.Text = errmsg;
				return;
			}

			Global.RefreshPageFormatCache(null,null,0);

			newPageformatName = txtPageformatName.Text;
			doClose = 1;
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
