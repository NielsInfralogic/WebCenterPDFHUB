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
using System.Text;
using System.Globalization;
using System.Resources;
using System.Threading;
using WebCenter4.Classes;

namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for ChangeTemplate.
	/// </summary>
    public class UploaPlan : System.Web.UI.Page
    {
        protected System.Web.UI.WebControls.Label lblTemplate;
 
        protected System.Web.UI.WebControls.Label InjectScript;
        protected System.Web.UI.HtmlControls.HtmlInputFile FileData;
        protected System.Web.UI.HtmlControls.HtmlInputButton Submit1;
        protected System.Web.UI.WebControls.Label Label1;
        protected System.Web.UI.WebControls.TextBox txtFilename;
        protected System.Web.UI.WebControls.Label lblInfo;
        protected System.Web.UI.WebControls.Label lblInfo2;
        protected System.Web.UI.WebControls.Label Label2;
        protected System.Web.UI.WebControls.Label lblFolder;

        protected global::Telerik.Web.UI.RadButton bntApply;



        private void Page_Load(object sender, System.EventArgs e)
        {
            if (!this.IsPostBack)
            {

                string  physicalFolder = Global.sRealPlanFolder;

                if (System.IO.Directory.Exists(physicalFolder) == false)
                    lblInfo.Text = "Upload folder " + physicalFolder + " not found";
                else
                    lblFolder.Text = physicalFolder;

            }
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            bntApply.Text = Global.rm.GetString("txtClose");
        
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
            this.Submit1.ServerClick += new System.EventHandler(this.Submit1_ServerClick);
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion
      
        protected void btnCancel_Click(object sender, System.EventArgs e)
        {
            InjectScript.Text = "<script>CloseOnReload()</" + "script>";
        }

        protected void bntApply_Click(object sender, System.EventArgs e)
		{		
			InjectScript.Text="<script>CloseOnReload()</" + "script>";
		}


        protected void Submit1_ServerClick(object sender, System.EventArgs e)
		{
			HttpPostedFile myFile = FileData.PostedFile;
			if(myFile == null)
			{
				lblInfo.Text = "Error - no file selected";
				return;
			}
			if(myFile.FileName == "" || myFile.ContentLength == 0)
			{
				lblInfo.Text = "Error - no file selected";
				return;
			}

			long fileSizeBefore = (long)myFile.ContentLength;

			string fn = txtFilename.Text.Trim();
			if (fn == "")
			{
				//fn = System.IO.Path.GetFileName(File1.PostedFile.FileName);
				fn = System.IO.Path.GetFileName(myFile.FileName);
				txtFilename.Text = fn;
			}
			string SaveLocation = lblFolder.Text + "\\" +  fn;
			string SaveLocationTmp = lblFolder.Text + "\\" +  fn + ".tmp";
			try
			{
				myFile.SaveAs(SaveLocationTmp);
				if (System.IO.File.Exists(SaveLocationTmp) == false)
				{
					lblInfo.Text = "Error uploading file";
					return;
				}

				System.IO.File.Move(SaveLocationTmp, SaveLocation);

				if (System.IO.File.Exists(SaveLocation) == false)
				{
					lblInfo.Text = "Error renaming final file";
					return;
				}

				lblInfo.Text = "The file has been uploaded";

				System.IO.FileInfo finfo = new System.IO.FileInfo(SaveLocation);

				lblInfo2.Text = "File length before " + fileSizeBefore.ToString() + " after " + finfo.Length.ToString();

			}
			catch ( Exception ex )
			{
				//Response.Write("Error: " + ex.Message);
				lblInfo.Text = "Error uploading file " + SaveLocation + " \rError: " + ex.Message;
				//Note: Exception.Message returns a detailed message that describes the current exception. 
				//For security reasons, we do not recommend that you return Exception.Message to end users in 
				//production environments. It would be better to return a generic error message. 
			}
			
		}

    
    }
}
