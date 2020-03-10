using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;

namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for PDFview.
	/// </summary>
	/// 
	public partial class AlwanReportview : System.Web.UI.Page
	{

		public string sImagePath;

		private void Page_Load(object sender, System.EventArgs e)
		{
			//string pdfFilepath = "";
			if(Request.QueryString["Path"]==null)
				return;
            string path = Request.QueryString["Path"];

			Response.Clear();
			//Response.Buffer= true;
			//Response.Charset = "";
			Response.ContentType = "application/pdf";
			Response.AddHeader("Content-Type","application/pdf");
			string fileTitle = path;
			int n=fileTitle.LastIndexOf("/");
			if (n != -1)
				fileTitle = fileTitle.Substring(n+1);
			string s = "inline;filename="+fileTitle;
			Response.AddHeader("Content-Disposition",s);
            Response.WriteFile(path); 
			Response.Flush(); 
			Response.End();
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
