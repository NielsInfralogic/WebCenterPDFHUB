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
using System.IO;

namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for ChangePriority.
	/// </summary>
	public partial class DownloadPDF : System.Web.UI.Page
	{

      

		private void Page_Load(object sender, System.EventArgs e)
		{
		
			if (!this.IsPostBack)
			{
                lblError.Text = "";
                if (Request.QueryString["mastercopyseparationset"] != null)
                {
                    try
                    {
                        hiddenMasterCopySeparationSet.Value = (string)Request.QueryString["mastercopyseparationset"];
                    }
                    catch
                    {
                        InjectScript.Text = "<script>CloseOnReload()</" + "script>";
                        return;
                    }
                }

                CCDBaccess db = new CCDBaccess();

                string filenameFromDatabase = db.GetFileName(Globals.TryParse(hiddenMasterCopySeparationSet.Value, 0), out string errmsg);
                if (filenameFromDatabase == "")
                {
                    InjectScript.Text = "<script>CloseOnReload()</" + "script>";
                    return;
                }
                int nn = filenameFromDatabase.IndexOf("#");
                if (nn != -1)
                    filenameFromDatabase = filenameFromDatabase.Substring(0, nn) + ".pdf";


                lblFileName.Text = filenameFromDatabase;

            }


            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            btnClose.Text = Global.rm.GetString("txtClose");

            SetRadToolbarLabel("Item1", "lblDownloadPDF", Global.rm.GetString("txtDownloadPDF"));
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


        protected void btnSave_Click(object sender, System.EventArgs e)
		{
            string errmsg = "";
            CCDBaccess db = new CCDBaccess();

            int nMasterCopySeparationSet = Globals.TryParse(hiddenMasterCopySeparationSet.Value, 0);

            if (nMasterCopySeparationSet > 0)
            {
                
                string folder;
                if (RadioButtonListPreviewtype.SelectedIndex == 1)
                    folder = Global.sRealHiresFolder;
                else if (RadioButtonListPreviewtype.SelectedIndex == 0)
                    folder = Global.sRealHiresFolder.Replace("CCFilesHires", "CCFilesLowres");
                else
                    folder = Global.sRealHiresFolder.Replace("CCFilesHires", "CCFilesPrint");

                string fname;
                


                fname = folder + @"\" + lblFileName.Text.Replace(".pdf","").Replace(".PDF", "") + "#" + nMasterCopySeparationSet.ToString() + ".pdf";
                Global.logging.WriteLog("DownloadPDF - filename = " + fname);
                if (System.IO.File.Exists(fname))
                {
                    Encoding ascii = Encoding.ASCII;
                    Encoding unicode = Encoding.Unicode;
                    // Convert the string into a byte[].
                    byte[] unicodeBytes = unicode.GetBytes(lblFileName.Text);

                    // Perform the conversion from one encoding to the other.
                    byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);

                    // Convert the new byte[] into a char[] and then into a string.
                    // This is a slightly different approach to converting to illustrate
                    // the use of GetCharCount/GetChars.
                    char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
                    ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
                    string asciiString = new string(asciiChars);

                    // Send to browser..
                    try
                    {
                        Response.Buffer = true;
                        Response.ContentType = "application/pdf";
                        Response.AppendHeader("Content-Disposition", "attachment; filename=" + asciiString);
                        //Response.AppendHeader("Content-Length", buf.Length.ToString());
                        Response.TransmitFile(fname);
                        Response.End();

                    }
                    catch (Exception ex)
                    {
                        lblError.Text = "Exception sending PDF " + ex.Message;
                        Global.logging.WriteLog("DownloadPDF - Exception sending PDF - " + ex.Message);
                    }
                }
                else
                {
                    Global.logging.WriteLog("DownloadPDF - File " + fname + " not found");
                    lblError.Text = "File " + fname + " not found";
                }

            }
            else
                 Global.logging.WriteLog("DownloadPDF - MasterCopySeparationSet is 0");

            InjectScript.Text = "<script>RefreshParentPage()</" + "script>";
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

        protected void btnClose_Click(object sender, EventArgs e)
        {
            InjectScript.Text = "<script>CloseOnReload()</" + "script>";
        }
	}
}
