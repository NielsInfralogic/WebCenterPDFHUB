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
	public partial class PitstopReportview : System.Web.UI.Page
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
	}
}
