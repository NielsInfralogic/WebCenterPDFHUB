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
using System.Net;
using System.Net.Mail;

namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for Zoomview.
	/// </summary>
	public class ZoomviewReadFlash2 : System.Web.UI.Page
	{
        protected global::Telerik.Web.UI.RadToolBar RadToolBar1;
        
        public string sImagePath;

		public string sColors;
		public int nCurrentCopySeparationSet;
		public int nCurrentCopySeparationSet2;
		public string printimagepath;
		public int simpleFlash;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ((string)Session["UserName"] == null)
				Response.Redirect("~/SessionTimeout.htm");

			if ((string)Session["UserName"] == "")
				Response.Redirect("/Denied.htm");

			if (!Page.IsPostBack)
			{
				SetLanguage();

                Telerik.Web.UI.RadToolBarButton item = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("SendMail");
                if (item != null)
                {
                    item.Checked = (bool)Session["SendMail"];
                    item.Visible = (bool)Session["SendMail"];
                }
                item = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("Mask");
                if (item != null)
                    item.Visible = (bool)Application["ThumbnailShowMask"];

				PrepareZoom((int)Session["CurrentCopySeparationSet"], (int)Session["CurrentCopySeparationSet2"], 
					        (string)Session["CurrentPageName"],(string)Session["CurrentPageName2"], (int)Session["CurrentApprovalState"]);
			}
            if ((bool)Application["NoCache"])
            {
                Response.AppendHeader("cache-control", "private");
                Response.AppendHeader("pragma", "no-cache");
                Response.AppendHeader("expires", "Fri, 30 Oct 1998 14:19:41 GMT");
                Response.CacheControl = "Private";
                Response.Cache.SetNoStore();
            }
		}	

		private void SetLanguage()
		{
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            SetRadToolbarLabel("Approve", Global.rm.GetString("txtApprove"));
            SetRadToolbarTooltip("Approve", Global.rm.GetString("txtTooltipApprovePage"));
            SetRadToolbarEnable("Approve", (bool)Session["MayApprove"]);

            SetRadToolbarLabel("Disapprove", Global.rm.GetString("txtReject"));
            SetRadToolbarTooltip("Disapprove", Global.rm.GetString("txtTooltipDisapprovePage"));
            SetRadToolbarEnable("Disapprove", (bool)Session["MayApprove"]);

            SetRadToolbarLabel("SendMail", Global.rm.GetString("txtSendMailOnReject"));
            SetRadToolbarTooltip("SendMail", Global.rm.GetString("txtTooltipSendMailOnReject"));
            SetRadToolbarChecked("SendMail", (bool)Session["SendMail"]);

            SetRadToolbarLabel("Close", Global.rm.GetString("txtBack"));
            SetRadToolbarTooltip("Close", Global.rm.GetString("txtTooltipBack"));

            SetRadToolbarLabel("CommentLabel", Global.rm.GetString("txtComment"));

            SetRadToolbarLabel("Backward", Global.rm.GetString("txtReadviewBackward"));
            SetRadToolbarTooltip("Backward", Global.rm.GetString("txtTooltipBackward"));

            SetRadToolbarLabel("Forward", Global.rm.GetString("txtReadviewForward"));
            SetRadToolbarTooltip("Forward", Global.rm.GetString("txtTooltipForward"));

            SetRadToolbarLabel("Mask", Global.rm.GetString("txtMask"));
            SetRadToolbarTooltip("Mask", Global.rm.GetString("txtTooltipMask"));
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

        protected void RadToolBar1_ButtonClick(object sender, Telerik.Web.UI.RadToolBarEventArgs e)
		{

            Telerik.Web.UI.RadToolBarButton item1 = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("SendMail");
            if (item1 != null)
                Session["SendMail"] = item1.Checked;


            string comment = GetCommentBox();
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue("Item3");
            if (item == null)
                return;
            Label lbl = (Label)item.FindControl("PageName");

			string errmsg = "";
            if (e.Item.Value == "Forward")
			{
				// Are we at the end already?
				if ((int)Session["CurrentCopySeparationSet2"] == 0)
				{
					PrepareZoom((int)Session["CurrentCopySeparationSet"],(int)Session["CurrentCopySeparationSet2"],
						(string)Session["CurrentPageName"],(string)Session["CurrentPageName2"], (int)Session["CurrentApprovalState"]);

					return;	
				}

				CCDBaccess db = new CCDBaccess();

				string sPageNameLeft = "";
				int nApproveLeft = 0;
				nCurrentCopySeparationSet =  db.GetReadviewNextPage((int)Session["CurrentCopySeparationSet2"], out sPageNameLeft, out nApproveLeft, out errmsg);

				string sPageNameRight = "";
				int nApproveRight = 1;
				nCurrentCopySeparationSet2 =  db.GetReadviewNextPage(nCurrentCopySeparationSet, out sPageNameRight, out nApproveRight, out errmsg);

				if (nCurrentCopySeparationSet == 0 && nCurrentCopySeparationSet2 == 0)
				{
					PrepareZoom((int)Session["CurrentCopySeparationSet"],(int)Session["CurrentCopySeparationSet2"],
						(string)Session["CurrentPageName"],(string)Session["CurrentPageName2"], (int)Session["CurrentApprovalState"]);
					return;
				}
				
				if ((string)Session["ShowSep"] != "CMYK" && (string)Session["ShowSep"] != "MASK")
					Session["ShowSep"] = "CMYK";			

				PrepareZoom(nCurrentCopySeparationSet,nCurrentCopySeparationSet2, sPageNameLeft, sPageNameRight, nCurrentCopySeparationSet > 0 ? nApproveLeft : nApproveRight);
			}


            if (e.Item.Value == "Backward")
			{
                // Are we at the front already?
                if ((int)Session["CurrentCopySeparationSet"] == 0)
                {
                    PrepareZoom((int)Session["CurrentCopySeparationSet"], (int)Session["CurrentCopySeparationSet2"],
                        (string)Session["CurrentPageName"], (string)Session["CurrentPageName2"], (int)Session["CurrentApprovalState"]);
                    return;
                }
                CCDBaccess db = new CCDBaccess();

                string sPageNameLeft = "";
                int nApproveLeft = 0;
                string sPageNameRight = "";
                int nApproveRight = 1;

                nCurrentCopySeparationSet2 = db.GetReadviewPrevPage((int)Session["CurrentCopySeparationSet"], out sPageNameRight, out nApproveRight, out errmsg);
                nCurrentCopySeparationSet = db.GetReadviewPrevPage(nCurrentCopySeparationSet2, out sPageNameLeft, out nApproveLeft, out errmsg);
                if (nCurrentCopySeparationSet == 0 && nCurrentCopySeparationSet2 == 0)
                {
                    PrepareZoom((int)Session["CurrentCopySeparationSet"], (int)Session["CurrentCopySeparationSet2"],
                        (string)Session["CurrentPageName"], (string)Session["CurrentPageName2"], (int)Session["CurrentApprovalState"]);
                    return;
                }

                if ((string)Session["ShowSep"] != "CMYK" && (string)Session["ShowSep"] != "MASK")
                    Session["ShowSep"] = "CMYK";

                PrepareZoom(nCurrentCopySeparationSet, nCurrentCopySeparationSet2, sPageNameLeft, sPageNameRight, nCurrentCopySeparationSet > 0 ? nApproveLeft : nApproveRight);
			}

            if (e.Item.Value == "Approve")
            {
                if ((bool)Session["MayApprove"] == false)
                    return;
                CCDBaccess db = new CCDBaccess();

                nCurrentCopySeparationSet = (int)Session["CurrentCopySeparationSet"];
                nCurrentCopySeparationSet2 = (int)Session["CurrentCopySeparationSet2"];

                if (nCurrentCopySeparationSet > 0)
                {
                    if (db.UpdateApproval((string)Session["UserName"], nCurrentCopySeparationSet, 1, out errmsg) == false)
                    {
                        lbl.Text = "Could not update approve status";
                    }
                    else
                    {
                        Session["CurrentApprovalState"] = 1;
                        lbl.Text = "Page " + (string)Session["CurrentPageName"] + " is currently APPROVED";

                    }
                }

                if (nCurrentCopySeparationSet2 > 0)
                {
                    if (db.UpdateApproval((string)Session["UserName"], nCurrentCopySeparationSet2, 1, out errmsg) == false)
                    {
                        lbl.Text = "Could not update approve status";
                    }
                    else
                    {

                        Session["CurrentApprovalState"] = 1;
                        lbl.Text = "Page " + (string)Session["CurrentPageName"] + " is currently APPROVED";
                    }
                }

                if ((bool)Session["LogApprove"])
                {
                    if (nCurrentCopySeparationSet > 0)
                        db.UpdateApproveLog(nCurrentCopySeparationSet, 1, true, "", (string)Session["UserName"], out  errmsg);
                    if (nCurrentCopySeparationSet2 > 0)
                        db.UpdateApproveLog(nCurrentCopySeparationSet2, 1, true, "", (string)Session["UserName"], out  errmsg);
                }

                if ((bool)Session["CloseZoomAfterApprove"])
                    Response.Redirect("ReadView.aspx");
            }

            if (e.Item.Value == "Disapprove")
            {
                if ((bool)Session["MayApprove"] == false)
                    return;
                CCDBaccess db = new CCDBaccess();

                nCurrentCopySeparationSet = (int)Session["CurrentCopySeparationSet"];
                nCurrentCopySeparationSet2 = (int)Session["CurrentCopySeparationSet2"];

                if (nCurrentCopySeparationSet > 0)
                {
                    if (db.UpdateApproval((string)Session["UserName"], nCurrentCopySeparationSet, 2, out errmsg) == false)
                    {
                        lbl.Text = "Could not update approve status";
                    }
                    else
                    {
                        Session["CurrentApprovalState"] = 2;
                        lbl.Text = "Page " + (string)Session["CurrentPageName"] + " is currently REJECTED";

                        if ((bool)Session["LogDisapprove"])
                        {
                            db.UpdateApproveLog(nCurrentCopySeparationSet, 1, false, comment, (string)Session["UserName"], out  errmsg);
                        }
                        if ((bool)Session["SendMail"])
                        {
                            try
                            {
                                SmtpClient client = new SmtpClient((string)ConfigurationManager.AppSettings["SMTPServer"]);
                                MailAddress cc = new MailAddress((string)ConfigurationManager.AppSettings["MailCC"]);

                                MailMessage mailNew = new MailMessage((string)ConfigurationManager.AppSettings["MailFrom"], (string)ConfigurationManager.AppSettings["MailTo"]);
                                mailNew.CC.Add((string)ConfigurationManager.AppSettings["MailCC"]);
                                mailNew.Subject = (string)ConfigurationManager.AppSettings["MailSubject"];
                                mailNew.Attachments.Add(new Attachment((string)Session["RealImagePath"]));

                                DateTime tPubDate = (DateTime)Session["SelectedPubDate"];

                                string pagename = (string)Session["SelectedPublication"] + "-" + (string)Session["SelectedSection"] + "-" + (string)Session["SelectedEdition"] + "-" + (string)Session["CurrentPageName"];

                                string mailBody = "The following page was rejected by user " + (string)Session["UserName"] + "\n\n Page " + pagename + "\n\nComment: " + comment;
                                mailNew.Body = mailBody;
                                client.Send(mailNew);
                            }
                            catch // (System.Web.HttpException ehttp)
                            {
                            }
                        }
                    }
                }
                if (nCurrentCopySeparationSet2 > 0)
                {
                    if (db.UpdateApproval((string)Session["UserName"], nCurrentCopySeparationSet2, 2, out errmsg) == false)
                    {
                        lbl.Text = "Could not update approve status";
                    }
                    else
                    {
                        Session["CurrentApprovalState"] = 2;
                        lbl.Text = "Page " + (string)Session["CurrentPageName2"] + " is currently REJECTED";

                        if ((bool)Session["LogDisapprove"])
                        {
                            db.UpdateApproveLog(nCurrentCopySeparationSet2, 1, false, comment, (string)Session["UserName"], out  errmsg);
                        }
                        if ((bool)Session["SendMail"])
                        {
                            try
                            {
                                // MailMessage mailNew = (MailMessage)Session["CurrentMail"];
                                SmtpClient client = new SmtpClient((string)ConfigurationManager.AppSettings["SMTPServer"]);

                                MailAddress cc = new MailAddress((string)ConfigurationManager.AppSettings["MailCC"]);

                                MailMessage mailNew = new MailMessage((string)ConfigurationManager.AppSettings["MailFrom"], (string)ConfigurationManager.AppSettings["MailTo"]);
                                mailNew.CC.Add((string)ConfigurationManager.AppSettings["MailCC"]);
                                mailNew.Subject = (string)ConfigurationManager.AppSettings["MailSubject"];

                                mailNew.Attachments.Add(new Attachment((string)Session["RealImagePath"]));

                                DateTime tPubDate = (DateTime)Session["SelectedPubDate"];

                                string pagename = (string)Session["SelectedPublication"] + "-" + (string)Session["SelectedSection"] + "-" + (string)Session["SelectedEdition"] + "-" + (string)Session["CurrentPageName2"];

                                string mailBody = "The following page was rejected by user " + (string)Session["UserName"] + "\n\n Page " + pagename + "\n\nComment: " + comment;
                                mailNew.Body = mailBody;
                                client.Send(mailNew);
                            }
                            catch // (System.Web.HttpException ehttp)
                            {
                            }
                        }
                    }
                }
                if ((bool)Session["CloseZoomAfterApprove"])
                {
                    Response.Redirect("ReadView.aspx");
                }
            }

            if (e.Item.Value == "Close")
			{
				Response.Redirect("ReadView.aspx");
			}

            if (e.Item.Value == "CMYK" || e.Item.Value == "Mask")
			{
                Session["ShowSep"] = e.Item.Value.ToUpper();
				PrepareZoom((int)Session["CurrentCopySeparationSet"], (int)Session["CurrentCopySeparationSet2"], 
					(string)Session["CurrentPageName"],(string)Session["CurrentPageName2"], (int)Session["CurrentApprovalState"]);
			}
		}


		private void doPopupImageNotReady()
		{
			string popupScript =
				"<script language='javascript'>" +
				" alert('Flash images are not ready yet');" +
				"</script>";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "PopupScript", popupScript);
		}

		private void LogUserView(int masterCopySeparationSet, string userName)
		{
			string errMsg = "";
			CCDBaccess db = new CCDBaccess();
			db.InsertPrepollLog(402, masterCopySeparationSet, userName, out errMsg);
		}

        private void DisplayPageName()
		{
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue("Item3");
            if (item == null)
                return;
            Label lbl = (Label)item.FindControl("PageName");

            lbl.ForeColor = Color.Black;
            lbl.BackColor = Color.Transparent;
            string sApp = Global.rm.GetString("txtNoApprovalRequired");
            if ((int)Session["CurrentApprovalState"] == 0)
            {
                sApp = Global.rm.GetString("txtNotApproved");
                lbl.BackColor = Color.Yellow;
            }
            else if ((int)Session["CurrentApprovalState"] == 1)
            {
                sApp = Global.rm.GetString("txtApproved");
                lbl.BackColor = Color.LightGreen;
            }
            else if ((int)Session["CurrentApprovalState"] == 2)
            {
                sApp = Global.rm.GetString("txtRejected");
                lbl.BackColor = Color.Red;
            }

            if ((string)Session["CurrentPageName"] == "0")
                lbl.Text = "     " + (string)Session["CurrentPageName2"] + " - " + sApp;
            else if ((string)Session["CurrentPageName2"] == "0")
                lbl.Text = (string)Session["CurrentPageName"] + "     " + " - " + sApp;
            else
                lbl.Text = (string)Session["CurrentPageName"] + " & " + (string)Session["CurrentPageName2"] + " - " + sApp;
		
		}

        private void PrepareZoom(int masterCopySeparationSetLeft, int masterCopySeparationSetRight, string pageNameLeft, string pageNameRight, int approve)
        {
            Session["CurrentPageName"] = pageNameLeft;
            Session["CurrentPageName2"] = pageNameRight;
            Session["CurrentCopySeparationSet"] = masterCopySeparationSetLeft;
            Session["CurrentCopySeparationSet2"] = masterCopySeparationSetRight;
            Session["CurrentApprovalState"] = approve;
            Session["ImagePath"] = "";
            Session["ImagePathMask"] = "";
            Session["RealImagePath"] = "";
            Session["HasTiles"] = false;

            DisplayPageName();

            string currentComment = "";
            int publicationID = 0;
            DateTime pubDate = DateTime.MinValue;
            bool hasMask = false;
            bool hasTiles = false;
            string errmsg = "";
            CCDBaccess db = new CCDBaccess();

            db.GetComment(masterCopySeparationSetLeft > 0 ? masterCopySeparationSetLeft : masterCopySeparationSetRight, out currentComment, out publicationID, out pubDate, out errmsg);
            if ((bool)Application["SetCommentInPrePollPageTable"])
                currentComment = db.GetPrePollMessage(masterCopySeparationSetLeft > 0 ? masterCopySeparationSetLeft : masterCopySeparationSetRight, 350, out errmsg);

            /*Session["CurrentComment"] = (bool)Session["SetCommentOnDisapproval"] ? currentComment : "";
            Infragistics.WebUI.UltraWebToolbar.TBTextBox commentbox = (Infragistics.WebUI.UltraWebToolbar.TBTextBox)UltraWebToolbar1.Items.FromKey("Comment");
            if (commentbox != null)
                commentbox.Text = currentComment;
*/
            if ((bool)Application["ThumbnailShowMask"])
                hasMask = db.HasMask(masterCopySeparationSetLeft > 0 ? masterCopySeparationSetLeft : masterCopySeparationSetRight, out errmsg);

            string realPath = "";
            string virtualPath = "";

            if (Globals.HasTileFolderReadview(masterCopySeparationSetLeft, masterCopySeparationSetRight, publicationID, pubDate, false, ref realPath, ref virtualPath))
            {
                hasTiles = true;
                Session["ImagePath"] = virtualPath;
                Session["HasTiles"] = true;
            }

            if (hasMask)
                if (Globals.HasTileFolderReadview(masterCopySeparationSetLeft, masterCopySeparationSetRight, publicationID, pubDate, true, ref realPath, ref virtualPath))
                    Session["ImagePathMask"] = virtualPath;

           


            string folderToTest = Global.sRealReadViewImageFolder + "\\" + masterCopySeparationSetLeft.ToString() + "_" + masterCopySeparationSetRight.ToString() + "\\";

            if (hasTiles == false)
            {
                if (Globals.HasPreviewReadview(masterCopySeparationSetLeft, masterCopySeparationSetRight, publicationID, pubDate, false, ref realPath, ref virtualPath))
                {
                    Session["ImagePath"] = virtualPath;
                }
                if (hasMask)
                    if (Globals.HasPreviewReadview(masterCopySeparationSetLeft, masterCopySeparationSetRight, publicationID, pubDate, true, ref realPath, ref virtualPath))
                        Session["ImagePathMask"] = virtualPath;
            }

            if ((string)Session["ImagePath"] == "")
            {
                // Display error message..
                return;
            }

            Telerik.Web.UI.RadToolBarButton maskbutton = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("Mask");
            if (maskbutton != null)
                maskbutton.Visible = hasMask && (string)Session["ImagePathMask"] != "" && (bool)Application["ThumbnailShowMask"];


            if ((string)Session["ImagePath"] != "")
                Session["RealImagePath"] = realPath;	// For mail attachment only

            // Finally - set the global sImagePath var used by flash module

            sImagePath = (string)Session["ImagePath"];

            // Default to CMYK if missing file(s)
            if ((string)Session["ShowSep"] == "MASK" && (string)Session["ImagePathMask"] == "")
                Session["ShowSep"] = "CMYK";


         
            printimagepath = (string)Session["ShowSep"] == "MASK" ?
                Global.sVirtualImageFolder + "/" + masterCopySeparationSetLeft.ToString() + "_" + masterCopySeparationSetRight.ToString() + "_mask.jpg" :
                Global.sVirtualImageFolder + "/" + masterCopySeparationSetLeft.ToString() + "_" + masterCopySeparationSetRight.ToString() + ".jpg";


            if ((bool)Application["LogUserAccess"] == true)
                LogUserView(masterCopySeparationSetLeft > 0 ? masterCopySeparationSetLeft : masterCopySeparationSetRight, (string)Session["UserName"]);


            maskbutton.Checked = false;
            if (hasTiles && (string)Session["ShowSep"] == "MASK")
            {
                simpleFlash = 0;
                sImagePath = (string)Session["ImagePathMask"] + "/";
                maskbutton.Checked = true;
                return;
            }
            if (hasTiles && (string)Session["ShowSep"] == "CMYK")
            {
                simpleFlash = 0;
                sImagePath += "/";
                return;
            }

            // All other images are shown without tiles
            simpleFlash = 1;
            sImagePath += ".jpg";
        }

        private string GetCommentBox()
        {
            string commentbox = "";
            Telerik.Web.UI.RadToolBarItem item1 = RadToolBar1.FindItemByValue("Item4");
            if (item1 == null)
                return "";

            TextBox txt = (TextBox)item1.FindControl("Comment");
            if (txt != null)
                commentbox = txt.Text;

            return commentbox;
        }

        private void SetCommentBox(string comment)
        {
            Telerik.Web.UI.RadToolBarItem item1 = RadToolBar1.FindItemByValue("Item4");
            if (item1 == null)
                return;

            TextBox txt = (TextBox)item1.FindControl("Comment");
            if (txt != null)
            {
                txt.Text = comment;
                if (comment != "")
                    txt.BackColor = Color.Yellow;
                else
                    txt.BackColor = Color.White;
            }
        }



        private void SetRadToolbarLabel(string buttonID, string text)
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue(buttonID);
            if (item == null)
                return;
            item.Text = text;
        }
        private void SetRadToolbarTooltip(string buttonID, string text)
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue(buttonID);
            if (item == null)
                return;
            item.ToolTip = text;
        }

        private void SetRadToolbarEnable(string buttonID, bool enable)
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue(buttonID);
            if (item == null)
                return;
            item.Enabled = enable;
        }

        private void SetRadToolbarChecked(string buttonID, bool check)
        {
            Telerik.Web.UI.RadToolBarButton item = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue(buttonID);
            if (item == null)
                return;
            item.Checked = check;
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
	}
}
