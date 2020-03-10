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

namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for PrintFrame.
	/// </summary>
	public class PrintFrame : System.Web.UI.Page
	{

	
		public string imagePath = "";
		private void Page_Load(object sender, System.EventArgs e)
		{
			// Test if this is a postback caused by a approval state change
			if (Request.QueryString["imagepath"] != null) 
			{
				try 
				{
					imagePath = Request.QueryString["imagepath"];
				}
				catch 
				{
					;
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
	}
}
