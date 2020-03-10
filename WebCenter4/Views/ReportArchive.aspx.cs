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
using System.IO;
using System.Text;
using System.Resources;
using System.Threading;
using System.Configuration;
using System.Globalization;

namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for ReportArchive.
	/// </summary>
	public class ReportArchive : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label LblError;
		protected System.Web.UI.WebControls.DataGrid DataGrid1;
	
		protected int doClose;

		private void Page_Load(object sender, System.EventArgs e)
		{
			doClose = 0;
			LblError.Text = "";

			GetReportFiles();


            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

        }


		private bool GetReportFiles()
		{
			string pubsallowed = (string)Session["PublicationsAllowed"];
			string [] publist = pubsallowed.Split(',');

			DataTable dt = new DataTable();
			DataColumn newColumn;
			newColumn = dt.Columns.Add("Product",Type.GetType("System.String"));

			try
			{
				string[] files = Directory.GetFiles(Global.sRealReportFolder,"*.*");
				foreach (string filePath in files)
				{
					string file  = Path.GetFileName(filePath);

                    if (file.Substring(0, 1) == ".")
                        continue;
                    if (Path.GetExtension(file) != "xls" && Path.GetExtension(file) != "dat")
                        continue;
     
					// Isolate publication name
					string pubName = "";
					string [] substring = file.Split('-');
					if (substring.Length < 2)
						return false;
					if (substring.Length == 2 || substring.Length == 3)
						pubName = substring[0];
					else if (substring.Length == 4)
						pubName = substring[0] + "-" + substring[1];
					else if (substring.Length == 5)
						pubName = substring[0] + "-" + substring[1] + "-" + substring[2];

					if (pubsallowed != "*")
					{
						bool found = false;
						foreach (string sp in publist)
						{
							if (sp == pubName)
							{
								found = true;
								break;
							}
						}
						if (found == false)
							continue;
					}

					DataRow newRow = dt.NewRow();
					newRow[0] = file;
					dt.Rows.Add(newRow);
				}

				DataGrid1.DataSource = dt;
				DataGrid1.DataBind();
			}

			catch
			{
				return false;
			}

			return true;

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
			this.DataGrid1.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGrid1_ItemCommand);
			this.DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGrid1_PageIndexChanged);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void DataGrid1_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
				
			if (e.CommandName == "Delete")
			{
				string sFileName = e.Item.Cells[2].Text;
				try
				{
					File.Delete(Global.sRealReportFolder + @"\" + sFileName);
				
				}
				catch
				{
				}
				GetReportFiles();

			}
			if (e.CommandName == "Download")
			{
				string sFileName = e.Item.Cells[2].Text;
				Encoding ascii = Encoding.ASCII;
				Encoding unicode = Encoding.Unicode;
				byte[] unicodeBytes = unicode.GetBytes(sFileName);
				// Perform the conversion from one encoding to the other.
				byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);
				char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
				ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
				string asciiString = new string(asciiChars);

				// Send to browser..

				Response.Clear();
				Response.Buffer = false;
				Response.Charset = "";
				Response.ContentType = "application/ms-excel";
				Response.AppendHeader("Content-Disposition", "attachment; filename="+asciiString);
				Response.WriteFile(HttpContext.Current.Server.MapPath(Global.sVirtualReportFolder + "\\" + sFileName)); 
				Response.Flush(); 
				Response.End();

			}
		}

		private void DataGrid1_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			// Set CurrentPageIndex to the page the user clicked.
			DataGrid1.CurrentPageIndex = e.NewPageIndex;

			// Rebind the data to refresh the DataGrid control. 
			GetReportFiles();

		}

		
	}
}
