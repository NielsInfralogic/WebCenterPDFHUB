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
using Telerik.Web.UI;


namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for ChangePriority.
	/// </summary>
	public class UploadFiles : System.Web.UI.Page
	{
        protected System.Web.UI.WebControls.Literal ltrNoResults;
        protected System.Web.UI.WebControls.GridView GridView1;

        protected Telerik.Web.UI.RadToolBar RadToolBar1;
        protected Telerik.Web.UI.RadButton RadButton1;
        protected Telerik.Web.UI.RadButton btnClose;

        protected Telerik.Web.UI.RadAsyncUpload RadAsyncUpload1;

        

        protected HtmlInputHidden hiddenUploadPath;
        // javascript var
		public int doClose;

		private void Page_Load(object sender, System.EventArgs e)
		{
			doClose = 0;
            string uploadFolder = "";
            if (!this.IsPostBack)
            {
                if (Request.QueryString["folder"] != null)
                {
                    try
                    {
                        uploadFolder = Request.QueryString["folder"];
                        uploadFolder = uploadFolder.Replace('!', '/');
                    }
                    catch
                    {
                    }
                }

                
                uploadFolder = Global.sVirtualUploadFolder;
                hiddenUploadPath.Value = uploadFolder;

                Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);
                btnClose.Text = Global.rm.GetString("txtClose");
                SetRadToolbarLabel("Item1", "LabelUploadFiles", Global.rm.GetString("txtTooltipUploadFiles"));

            }
           // RadAsyncUpload1.FileUploaded += new Telerik.Web.UI.FileUploadedEventHandler(RadAsyncUpload1_FileUploaded);

			// allows the javascript function to do a postback and call the onClick method
			// associated with the linkButton LinkButton1.
		//	string jscript = 	"<script language='javascript'>" + 
		//						"function UploadComplete(){";
		//	jscript += string.Format("__doPostBack('{0}','');", LinkButton1.ClientID.Replace("_", "$"));
		//	jscript += "} </script>";
         //   Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "FileCompleteUpload", jscript, false);
		
		}

        void RadAsyncUpload1_FileUploaded(object sender, FileUploadedEventArgs e)
        {
            e.IsValid = true;
            return;
          //  string temppath = RadAsyncUpload1.TemporaryFolder;
             // !CheckUploadedFileValidity();
         //   string fileNameChanged = e.File.GetFieldValue("TextBox");
            
          //  Label fileName = new Label();
//
     //       fileName.Text = e.File.FileName;

        //    string physicalSourceFolder = HttpContext.Current.Server.MapPath(RadAsyncUpload1.TemporaryFolder);

       //     string sourceFile = physicalSourceFolder + @"\" + e.File.FileName;
         

       //     string physicalFolder = HttpContext.Current.Server.MapPath(hiddenUploadPath.Value);

        //    string destFile = physicalFolder + @"\" + e.File.FileName;

         //   e.IsValid = true;
            /*    if (System.IO.File.Exists(sourceFile) == false)
                    e.IsValid = false;
                else
                {
                    try
                    {
                        System.IO.File.Move(sourceFile, destFile);
                    }
                    catch (Exception ex)
                    {
                        string s = ex.Message;
                        e.IsValid = false;
                    }
                }
            

                if (e.IsValid)
                {
                    ValidFiles.Visible = true;
                    ValidFiles.Controls.Add(fileName);
                }
                else
                {
                    InvalidFiles.Visible = true;
                    InvalidFiles.Controls.Add(fileName);

                }
                */
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

        protected void RadButton_Click(object sender, EventArgs e)
        {
            if (RadAsyncUpload1.UploadedFiles.Count > 0)
            {
                DataTable dt = new DataTable();
               
                DataRow row; 
                dt.Columns.Add(new DataColumn("FileName",System.Type.GetType("System.String")));
                dt.Columns.Add(new DataColumn("Size",System.Type.GetType("System.Int32")));
                dt.Columns.Add(new DataColumn("Status",System.Type.GetType("System.String")));
                
                GridView1.Visible = true;
                ltrNoResults.Visible = false;


                string physicalDestFolder = HttpContext.Current.Server.MapPath(hiddenUploadPath.Value);
                foreach (UploadedFile f in RadAsyncUpload1.UploadedFiles)
                {
                    string fileName = f.GetName();
                    string title = fileName;

                    if (f.GetFieldValue("TextBox") != null)
                    {
                        title = f.GetFieldValue("TextBox");
                        if (title == "")
                            title = fileName;
                    }

                    if (title.IndexOf(',') >= 0)
                        title = title.Substring(title.IndexOf(',') + 1);

                    string physicalSourceFolder = HttpContext.Current.Server.MapPath(RadAsyncUpload1.TargetFolder);

                    string sourceFile = physicalSourceFolder + @"\" + fileName;
                    string destFile = physicalDestFolder + @"\" + title;
                    row = dt.NewRow();  
                    row["FileName"] = title;
                    row["Size"] = f.ContentLength;
                    try
                    {
                        File.Copy(sourceFile, destFile, true);
                        row["Status"] = "OK";                        
                    }
                    catch (Exception ex)
                    {
                        row["Status"] = "Error - " + ex.Message;
                    }

                    dt.Rows.Add(row);

                }
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
            else
            {
                ltrNoResults.Visible = true;
                GridView1.Visible = false;
            }
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            if ((int)Session["RefreshTimeSaved"] > 0)
                Session["RefreshTime"] = Session["RefreshTimeSaved"];
            doClose = 1;
        }

	}
}
