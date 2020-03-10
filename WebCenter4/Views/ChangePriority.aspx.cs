using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
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
	public partial class ChangePriority : System.Web.UI.Page
	{
		

        private void Page_Load(object sender, System.EventArgs e)
		{
			if (!this.IsPostBack)
			{	
				if (Request.QueryString["Press"] != null) 
				{
					try 
					{
						RadNumerictxtPrioValue.Value = Globals.TryParse((string)Request.QueryString["Priority"],50);
						txtPress.Text = (string)Request.QueryString["Press"];	
						txtPublication.Text = (string)Request.QueryString["Publication"];
						txtPubDate.Text = (string)Request.QueryString["PubDate"];
						txtEdition.Text = (string)Request.QueryString["Edition"];
						txtSection.Text = (string)Request.QueryString["Section"];
					}
					catch 
					{
						;
					}

				} 
				else
				{
					InjectScript.Text="<script>CloseOnReload()</" + "script>";
					return;
				}
				
			}

            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);
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
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion

		protected void bntApply_Click(object sender, System.EventArgs e)
		{
			//int nPrio = Globals.TryParse(txtPrioValue.Text, -1);
			int nPrio = (int)RadNumerictxtPrioValue.Value;
			if (nPrio >= 0) 
			{
				CCDBaccess db = new CCDBaccess();

				string errmsg = "";
				string [] sargs = txtPubDate.Text.Split('-');
				DateTime dt = new DateTime(Int32.Parse(sargs[2]),Int32.Parse(sargs[1]),Int32.Parse(sargs[0]),0,0,0);

				db.UpdateProductionPriority(nPrio, txtPress.Text, txtPublication.Text, dt, "", txtEdition.Text, txtSection.Text, out errmsg);
				
			}

			InjectScript.Text="<script>RefreshParentPage()</" + "script>";
			
		}

		protected void btnCancel_Click(object sender, System.EventArgs e)
		{
			InjectScript.Text="<script>CloseOnReload()</" + "script>";
		}
	}
}
