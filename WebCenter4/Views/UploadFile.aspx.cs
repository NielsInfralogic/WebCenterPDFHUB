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
    public class UploadFile : System.Web.UI.Page
    {
        protected System.Web.UI.WebControls.Label lblTemplate;
 
        protected System.Web.UI.WebControls.Label InjectScript;
        protected System.Web.UI.HtmlControls.HtmlInputFile FileData;
        protected System.Web.UI.HtmlControls.HtmlInputButton Submit1;
        protected System.Web.UI.WebControls.Label Label1;
        protected System.Web.UI.WebControls.TextBox txtFilename;
 
        protected System.Web.UI.WebControls.Label lblInfo2;
        protected System.Web.UI.WebControls.Label lblFolder;
        protected global::Telerik.Web.UI.RadButton bntApply;

        private void Page_Load(object sender, System.EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (Session["SelectedMasterSet"] == null)
                {
                    InjectScript.Text = "<script>CloseOnReload()</" + "script>";
                    return;
                }

                if (Session["SelectedPlanPageName"] != null)
                    txtFilename.Text = (string)Session["SelectedPlanPageName"];
                /*
                if ((int)Application["CustomUploadFileNames"] == 1)
                {
                    string pubAlias = Globals.LookupInputAlias("Publication", (string)Session["SelectedPublication"]);
                    if (pubAlias == "")
                        pubAlias = (string)Session["SelectedPublication"];

                    string press = (string)Session["SelectedPress"];
                    if (press == "")
                        press = "SCH";

                    DateTime pubDate = (DateTime)Session["SelectedPubDate"];
                    txtFilename.Text = string.Format("{0}_{1}_T_{2:00}{3:00}{4:00}_{5}_1_{5:000}.pdf", pubAlias, press, pubDate.Day, pubDate.Month, 2000 - pubDate.Year, (string)Session["SelectedEdition"], (string)Session["SelectedSection"], pageName);



                }
                */
                /*if ((string)Session["SelectedPublication"] == "*" || (string)Session["SelectedPublication"] == "")
                {
                    lblInfo2.Text = "DB error retrieving upload folder";
                }*/

                // Retrieve special upload folder..
                CCDBaccess db = new CCDBaccess();
                /*int customerID = 0;
                string errmsg = "";
                string emailRecipient = "";
                string emailCC = "";
                string emailSubject = "";
                string emailBody = "";
                string folderToSave = "";
                */
                string virtualFolder = "";
                string physicalFolder = "";
               /* if (db.GetPublicationEmail(Globals.GetIDFromName("PublicationNameCache", (string)Session["SelectedPublication"]), out customerID, out  folderToSave, out  emailRecipient, out  emailCC, out  emailSubject, out  emailBody, out  errmsg) == false)
                {
                    lblInfo2.Text = "DB error retrieving upload folder";
                }
               
                if (folderToSave == "")
                {*/
                  //  lblInfo.Text = "Publication-specific upload folder not defined - using default folder";
                    virtualFolder = Global.sVirtualUploadFolder;
                    physicalFolder = Global.sRealUploadFolder;                    
             /*   }
                else
                {
                    //lblInfo.Text = "Using publication-specific upload folder";
                    virtualFolder = folderToSave;
                    physicalFolder = Server.MapPath(virtualFolder);
                }*/


                if (physicalFolder.EndsWith(";") || physicalFolder.EndsWith("/"))
                    physicalFolder = physicalFolder.Substring(0, physicalFolder.Length - 1);


                if (System.IO.Directory.Exists(physicalFolder) == false)
                    lblInfo2.Text = "Upload folder " + physicalFolder + " not found";

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
            if ((int)Session["RefreshTimeSaved"] > 0)
                Session["RefreshTime"] = Session["RefreshTimeSaved"];
            InjectScript.Text = "<script>CloseOnReload()</" + "script>";
        }

        protected void bntApply_Click(object sender, System.EventArgs e)
		{
            if ((int)Session["RefreshTimeSaved"] > 0)
                Session["RefreshTime"] = Session["RefreshTimeSaved"];
            InjectScript.Text="<script>CloseOnReload()</" + "script>";
		}


        protected void Submit1_ServerClick(object sender, System.EventArgs e)
		{
			HttpPostedFile myFile = FileData.PostedFile;
			if(myFile == null)
			{
				lblInfo2.Text = "Error - no file selected";
				return;
			}
			if(myFile.FileName == "" || myFile.ContentLength == 0)
			{
				lblInfo2.Text = "Error - no file selected";
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
					lblInfo2.Text = "Error uploading file";
					return;
				}

				System.IO.File.Move(SaveLocationTmp, SaveLocation);

				if (System.IO.File.Exists(SaveLocation) == false)
				{
					lblInfo2.Text = "Error renaming final file";
					return;
				}

				lblInfo2.Text = "The file has been uploaded";

				System.IO.FileInfo finfo = new System.IO.FileInfo(SaveLocation);

				lblInfo2.Text = "File length before " + fileSizeBefore.ToString() + " after " + finfo.Length.ToString();

			}
			catch ( Exception ex )
			{
				//Response.Write("Error: " + ex.Message);
				lblInfo2.Text = "Error uploading file " + SaveLocation + " \rError: " + ex.Message;
				//Note: Exception.Message returns a detailed message that describes the current exception. 
				//For security reasons, we do not recommend that you return Exception.Message to end users in 
				//production environments. It would be better to return a generic error message. 
			}
			
		}

    
    }
}
