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
	/// Summary description for PDFview.
	/// </summary>
	/// 
	public partial class PDFview : System.Web.UI.Page
	{
		
		public string sImagePath;
        public string sImagePath2;
        public string pdfDoc;

		private void Page_Load(object sender, System.EventArgs e)
		{
			sImagePath = (string)Session["ImagePath"];
            sImagePath2 = (string)Session["ImagePath2"];
            string mode = "1";
            if (Request.QueryString["mode"] != null)
            {
                try
                {
                    mode = Request.QueryString["mode"];
                }
                catch
                {
                }
            }
            pdfDoc = (mode == "2" && sImagePath2 != "") ? sImagePath2 : sImagePath;
/*			Response.Clear();
			//Response.Buffer= true;
			//Response.Charset = "";
			Response.ContentType = "application/pdf";
			Response.AddHeader("Content-Type","application/pdf");
			string fileTitle = sImagePath;
			int n=fileTitle.LastIndexOf("/");
			if (n != -1)
				fileTitle = fileTitle.Substring(n+1);
			string s = "inline;filename="+fileTitle;
			Response.AddHeader("Content-Disposition",s);
			Response.WriteFile(sImagePath); 
			Response.Flush(); 
			Response.End();*/
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
