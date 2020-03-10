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
	public class ZoomviewFlash2 : System.Web.UI.Page
	{
        protected global::Telerik.Web.UI.RadToolBar RadToolBar1;
        protected global::Telerik.Web.UI.RadToolBar RadToolBar2;
        
        public string sImagePath;

		public int nCurrentCopySeparationSet;
		public string sComments;

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

                item = (Telerik.Web.UI.RadToolBarButton)RadToolBar2.FindItemByValue("Forward");
                if (item != null)
                    item.Visible = (bool)Application["ApproveOnNextButton"] && (bool)Session["MayApprove"];

                item = (Telerik.Web.UI.RadToolBarButton)RadToolBar2.FindItemByValue("Backward");
                if (item != null)
                    item.Visible = (bool)Application["ApproveOnNextButton"] && (bool)Session["MayApprove"];

                item = (Telerik.Web.UI.RadToolBarButton)RadToolBar2.FindItemByValue("ForwardDis");
                if (item != null)
                    item.Visible = (bool)Application["ApproveOnNextButton"] && (bool)Session["MayApprove"] && (bool)Application["AllowDisapproveForward"];
                
                item = (Telerik.Web.UI.RadToolBarButton)RadToolBar2.FindItemByValue("BackwardDis");
                if (item != null)
                    item.Visible = (bool)Application["ApproveOnNextButton"] && (bool)Session["MayApprove"] && (bool)Application["AllowDisapproveForward"];

                item = (Telerik.Web.UI.RadToolBarButton)RadToolBar2.FindItemByValue("Mask");
                if (item != null)
                    item.Visible = (bool)Application["ThumbnailShowMask"];


//                item = (Telerik.Web.UI.RadToolBarButton)RadToolBar2.FindItemByValue("ApproveBeforeNext");
//                if (item != null)
//                    item.Checked = (bool)Application["ApproveOnNextButton"];

				PrepareZoom((int)Session["CurrentCopySeparationSet"], (string)Session["CurrentPageName"], (int)Session["CurrentApprovalState"], 
					(int)Session["CurrentVersion"], (string)Session["ImageColors"] == "K", (string)Session["ShowSep"] == "PDF");
			}

			if ((bool)Application["NoCache"]) 
			{
				Response.AppendHeader("cache-control", "private" );
				Response.AppendHeader("pragma", "no-cache" );
                Response.AppendHeader("expires", "Fri, 30 Oct 1998 14:19:41 GMT");
                Response.CacheControl = "Private";
				Response.Cache.SetNoStore();
			}
		}	

		private void DisplayPageName()
		{
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue("Item3");
            if (item == null)
                return;
            Label lbl = (Label)item.FindControl("PageName");

            if (lbl == null)
                return;
            lbl.ForeColor = Color.Black;
            lbl.BackColor = Color.Transparent;

            Response.ContentEncoding = Encoding.GetEncoding(Global.encoding);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Global.language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(Global.language);
            lbl.ForeColor = Color.Black;
            lbl.BackColor = Color.LightGray;
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

            lbl.Text = Global.rm.GetString("txtPage") + " " + (string)Session["CurrentPageName"] + " - " + sApp;

           item = RadToolBar1.FindItemByValue("Item5");
            if (item == null)
                return;
            lbl = (Label)item.FindControl("PageFormat");
            lbl.Text = " " + (string)Session["CurrentPageFormat"] + " ";
            lbl.ForeColor = Color.White;
            lbl.BackColor = Color.Gray;
            

		}

		private void SetLanguage()
		{
			Response.ContentEncoding = Encoding.GetEncoding(Global.encoding);
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(Global.language);
			Thread.CurrentThread.CurrentUICulture = new CultureInfo(Global.language);

            SetRadToolbarLabel("Approve", Global.rm.GetString("txtApprove"));
            SetRadToolbarTooltip("Approve", Global.rm.GetString("txtTooltipApprovePage"));
            SetRadToolbarEnable("Approve", (bool)Session["MayApprove"]);
            SetRadToolbarVisible("Approve", (bool)Session["MayApprove"]);

            SetRadToolbarLabel("Disapprove", Global.rm.GetString("txtReject"));
            SetRadToolbarTooltip("Disapprove", Global.rm.GetString("txtTooltipDisapprovePage"));
            SetRadToolbarEnable("Disapprove", (bool)Session["MayApprove"]);
            SetRadToolbarVisible("Disapprove", (bool)Session["MayApprove"]);

            SetRadToolbarLabel("SendMail", Global.rm.GetString("txtSendMailOnReject"));
            SetRadToolbarTooltip("SendMail", Global.rm.GetString("txtTooltipSendMailOnReject"));
            SetRadToolbarChecked("SendMail", (bool)Session["SendMail"]);

            SetRadToolbarLabel("Close", Global.rm.GetString("txtBack"));
            SetRadToolbarTooltip("Close", Global.rm.GetString("txtTooltipBack"));

            SetRadToolbarLabel("Item4", "CommentLabel", Global.rm.GetString("txtComment"));

            SetRadToolbarLabel2("Backward", Global.rm.GetString("txtReadviewBackward"));
            SetRadToolbarTooltip2("Backward", Global.rm.GetString("txtTooltipBackward"));
            SetRadToolbarEnable2("Backward", (bool)Session["MayApprove"]);
            SetRadToolbarVisible2("Backward", (bool)Session["MayApprove"]);

            SetRadToolbarLabel2("Forward", Global.rm.GetString("txtReadviewForward"));
            SetRadToolbarTooltip2("Forward", Global.rm.GetString("txtTooltipForward"));
            SetRadToolbarEnable2("Forward", (bool)Session["MayApprove"]);
            SetRadToolbarVisible2("Forward", (bool)Session["MayApprove"]);

            SetRadToolbarLabel2("BackwardOnly", Global.rm.GetString("txtBackward"));
            SetRadToolbarTooltip2("BackwardOnly", Global.rm.GetString("txtTooltipBackward"));

            SetRadToolbarLabel2("ForwardOnly", Global.rm.GetString("txtForward"));
            SetRadToolbarTooltip2("ForwardOnly", Global.rm.GetString("txtTooltipForward"));


            SetRadToolbarLabel2("BackwardDis", Global.rm.GetString("txtReadviewBackwardDis"));
            SetRadToolbarTooltip2("BackwardDis", Global.rm.GetString("txtTooltipBackward"));
            SetRadToolbarEnable2("BackwardDis", (bool)Session["MayApprove"] && (bool)Application["AllowDisapproveForward"]);
            SetRadToolbarVisible2("BackwardDis", (bool)Session["MayApprove"] && (bool)Application["AllowDisapproveForward"]);

            SetRadToolbarLabel2("ForwardDis", Global.rm.GetString("txtReadviewForwardDis"));
            SetRadToolbarTooltip2("ForwardDis", Global.rm.GetString("txtTooltipForward"));
            SetRadToolbarEnable2("ForwardDis", (bool)Session["MayApprove"] && (bool)Application["AllowDisapproveForward"]);
            SetRadToolbarVisible2("ForwardDis", (bool)Session["MayApprove"]&& (bool)Application["AllowDisapproveForward"]);

//            SetRadToolbarLabel2("ApproveBeforeNext", Global.rm.GetString("txtApproveBeforeNext"));
 //           SetRadToolbarTooltip2("ApproveBeforeNext", Global.rm.GetString("txtTooltipApproveBeforeNext"));

            SetRadToolbarLabel2("Mask", Global.rm.GetString("txtMask"));
            SetRadToolbarTooltip2("Mask", Global.rm.GetString("txtTooltipMask"));

            SetRadToolbarTooltip2("PDFCMYK", Global.rm.GetString("txtTooltipShowCMYKPDFSplit"));

            if ((bool)Application["ShowRasterImage"] == false)
            {
                SetRadToolbarVisible2("Raster", false);

            }


        }

        private void LogUserView(int masterCopySeparationSet, string userName)
		{
			string errMsg = "";
			CCDBaccess db = new CCDBaccess();
			db.InsertPrepollLog(402, masterCopySeparationSet, userName, out errMsg);
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



        protected void RadToolBar1_ButtonClick(object sender, Telerik.Web.UI.RadToolBarEventArgs e)
		{ 
            string commentbox = GetCommentBox(); 

            Telerik.Web.UI.RadToolBarButton item = (Telerik.Web.UI.RadToolBarButton)RadToolBar2.FindItemByValue("SendMail");
            if (item != null)
    			Session["SendMail"] = item.Checked;

           

			nCurrentCopySeparationSet = (int)Session["CurrentCopySeparationSet"];
		/*	if (e.Button.Key == "Print") {
				PrintImage((int)Session["CurrentCopySeparationSet"]);
			} */

			if (e.Item.Value == "Approve")
			{
				if ((bool)Session["MayApprove"] == false)
					return;
				string errmsg = "";
				CCDBaccess db = new CCDBaccess();

				if (db.UpdateApproval((string)Session["UserName"], nCurrentCopySeparationSet, 1, commentbox, out errmsg) == false) 
				{
					//lblError.Text = "Could not update approve status - "+errmsg;				
				} 
				else 
				{
					Session["CurrentApprovalState"] = 1;
					DisplayPageName();
				}

				if ((bool)Session["LogApprove"])
					db.UpdateApproveLog(nCurrentCopySeparationSet,	1, true, commentbox, (string)Session["UserName"], out  errmsg);

				if ((bool)Session["CloseZoomAfterApprove"]) 
				{
                     if ((bool)Application["UseChannels"])
                        Response.Redirect("ThumbnailViewChannels.aspx");
                    else
                        Response.Redirect("Thumbnailview2.aspx");
				}
			}



            if (e.Item.Value == "Disapprove")
			{
				string errmsg = "";
				if ((bool)Session["MayApprove"] == false)
					return;
				
				CCDBaccess db = new CCDBaccess();

				if (db.UpdateApproval((string)Session["UserName"],nCurrentCopySeparationSet, 2, commentbox, out errmsg) == false)
				{
					//lblError.Text = "Could not update approve status - "+errmsg;
				}
				else
				{
				
					Session["CurrentApprovalState"] = 2;
					DisplayPageName();

					if ((bool)Session["LogDisapprove"])
					{
						if ((bool)Session["SetCommentOnDisapproval"] == true)
							db.UpdateApproveLog(nCurrentCopySeparationSet,	1, false, commentbox, (string)Session["UserName"], out  errmsg);
					}
					if ((bool)Session["SendMail"])
					{
						try 
						{
                            SmtpClient client =  new SmtpClient((string)ConfigurationManager.AppSettings["SMTPServer"]);

                            MailAddress cc = new MailAddress((string)ConfigurationManager.AppSettings["MailCC"]);

                            MailMessage mailNew = new MailMessage((string)ConfigurationManager.AppSettings["MailFrom"], (string)ConfigurationManager.AppSettings["MailTo"]);
                            mailNew.CC.Add((string)ConfigurationManager.AppSettings["MailCC"]);
                            mailNew.Subject = (string)ConfigurationManager.AppSettings["MailSubject"];

                            mailNew.Attachments.Add(new Attachment((string)Session["RealImagePath"]));
							
							DateTime tPubDate = (DateTime)Session["SelectedPubDate"];

							string pagename = (string)Session["SelectedPublication"] + "-" + (string)Session["SelectedSection"] + "-" + (string)Session["SelectedEdition"] + "-" + (string)Session["CurrentPageName"];

							string mailBody = "The following page was rejected by user "+Session["UserName"]+ "\n\n Page " + pagename+ "\n\nComment: " + commentbox;
							mailNew.Body = mailBody;
                            client.Send(mailNew);
						}
						catch //(System.Web.HttpException ehttp)
						{
							//lblError.Text = "Error sending email: Cannot reach server or your e-mail address was rejected ("+ ehttp.Message+")";
						//	return;
						}
					}
				}
				if ((bool)Session["CloseZoomAfterApprove"]) 
				{
                    if ((bool)Application["UseChannels"])
                        Response.Redirect("ThumbnailViewChannels.aspx?Refresh=2");
                    else
                        Response.Redirect("Thumbnailview2.aspx?Refresh=2");
				}
			}

            if (e.Item.Value == "Close")
			{
                if ((bool)Application["UseChannels"])
                    Response.Redirect("ThumbnailViewChannels.aspx?Refresh=2");
                else
                    Response.Redirect("Thumbnailview2.aspx?Refresh=2");
			}
		}

	    protected void RadToolBar2_ButtonClick(object sender, Telerik.Web.UI.RadToolBarEventArgs e)
		{
		
		//	Application["ApproveOnNextButton"] = true; //Ultrawebtoolbar2.Items.FromKeyButton("ApproveBeforeNext").Selected;
            string commentbox = GetCommentBox();

            if (e.Item.Value == "Forward" || e.Item.Value == "ForwardDis" || e.Item.Value == "ForwardOnly")
			{
				string errmsg;
				CCDBaccess db = new CCDBaccess();

				nCurrentCopySeparationSet = (int)Session["CurrentCopySeparationSet"];	
				int nPrevCopySeparationSet = nCurrentCopySeparationSet;


                if ((bool)Application["ApproveOnNextButton"] && (bool)Session["MayApprove"] && e.Item.Value != "ForwardOnly") 
				{

                    if (db.UpdateApproval((string)Session["UserName"], nCurrentCopySeparationSet, e.Item.Value == "Forward" ? 1 : 2, commentbox, out errmsg) == false) 
					{
						//lblError.Text = "Could not update approve status - "+errmsg;
					} 
					else 
					{
						Session["CurrentApprovalState"] = 1;
					}

					if ((bool)Session["LogApprove"])
					{
                        db.UpdateApproveLog(nCurrentCopySeparationSet, 1, e.Item.Value == "Forward" ? true : false, "", (string)Session["UserName"], out  errmsg);
					}
				}

				string sPageName = "";
				int nApprove= 0;
				int version = 1;
				bool isMono = false;


				nCurrentCopySeparationSet =  db.GetNextPage(nCurrentCopySeparationSet, false, out version, out sPageName, out nApprove, out isMono, out errmsg);
			
				// Are we at the end already?
				if (nCurrentCopySeparationSet == 0)
				{
					nCurrentCopySeparationSet = nPrevCopySeparationSet;
					sPageName = (string)Session["CurrentPageName"];
					nApprove = (int)Session["CurrentApprovalState"];
					version = (int)Session["CurrentVersion"];	
				}

				if ((string)Session["ShowSep"] != "CMYK" && (string)Session["ShowSep"] != "MASK" && (string)Session["ShowSep"] != "DNS" && (string)Session["ShowSep"] != "PDFCMYK" && (string)Session["ShowSep"] != "PDF" && (string)Session["ShowSep"] != "Raster")
					Session["ShowSep"] = "CMYK";

				PrepareZoom(nCurrentCopySeparationSet, sPageName, nApprove, version, isMono, false);

				
			}


            if (e.Item.Value == "Backward" || e.Item.Value == "BackwardDis" || e.Item.Value == "BackwardOnly")
			{
				
				nCurrentCopySeparationSet = (int)Session["CurrentCopySeparationSet"];
				int nPrevCopySeparationSet = nCurrentCopySeparationSet;
				CCDBaccess db = new CCDBaccess();
				string errmsg = "";


                if ((bool)Application["ApproveOnNextButton"] && (bool)Session["MayApprove"] && e.Item.Value != "BackwardOnly") 
				{
                    if (db.UpdateApproval((string)Session["UserName"], nCurrentCopySeparationSet, e.Item.Value == "Backward" ? 1 : 2, out errmsg) == false) 
					{
						//lblError.Text = "Could not update approve status - "+errmsg;
					} 
					else 
					{
						Session["CurrentApprovalState"] = 1;
						
					}

					if ((bool)Session["LogApprove"])
					{
                        db.UpdateApproveLog(nCurrentCopySeparationSet, 1, e.Item.Value == "Backward" ? true : false, "", (string)Session["UserName"], out  errmsg);
					}
				}

				string sPageName = "";
				int nApprove= 0;
				int version = 1;
				bool isMono = false;
				
				nCurrentCopySeparationSet =  db.GetPrevPage(nCurrentCopySeparationSet, false, out version, out sPageName, out nApprove, out isMono, out errmsg);
			
				// Are we at the front already?
				if (nCurrentCopySeparationSet == 0)
				{
					nCurrentCopySeparationSet = nPrevCopySeparationSet;
					sPageName = (string)Session["CurrentPageName"];
					nApprove = (int)Session["CurrentApprovalState"];
					version = (int)Session["CurrentVersion"];
				 	
				}

                if ((string)Session["ShowSep"] != "CMYK" && (string)Session["ShowSep"] != "MASK" && (string)Session["ShowSep"] != "DNS" && (string)Session["ShowSep"] != "PDFCMYK" && (string)Session["ShowSep"] != "PDF" && (string)Session["ShowSep"] != "Raster")
					Session["ShowSep"] = "CMYK";
						
				PrepareZoom(nCurrentCopySeparationSet, sPageName, nApprove, version, isMono, false);

			}

            bool colorButton = (e.Item.Value == "CMYK" || e.Item.Value == "C" || e.Item.Value == "M" || e.Item.Value == "Y" || e.Item.Value == "K" || e.Item.Value == "Mask" || e.Item.Value == "Dns" || e.Item.Value == "PDF" || e.Item.Value == "PDFCMYK" || e.Item.Value == "Raster");

			if (colorButton)
			{
                Session["ShowSep"] = e.Item.Value.ToUpper();

				PrepareZoom((int)Session["CurrentCopySeparationSet"], (string)Session["CurrentPageName"], (int)Session["CurrentApprovalState"], (int)Session["CurrentVersion"], (string)Session["ImageColors"] == "K", false);
			}
		}

		private void PrepareZoom(int masterCopySeparationSet, string pageName, int approve, int version, bool isMono, bool forcePDF)
		{
            Session["CurrentPageName"] = pageName;
            Session["CurrentPageFormat"] = "";
            Session["CurrentCopySeparationSet"] = masterCopySeparationSet;
            Session["CurrentApprovalState"] = approve;
            Session["CurrentVersion"] = version;
            Session["ImagePath"] = "";
            Session["ImagePathMask"] = "";
            Session["ImagePathRaster"] = "";
            Session["RealImagePath"] = "";
            Session["HasTiles"] = false;
            Session["HasRaster"] = false;
            double xdim = 0.0;
            double ydim = 0.0;
            double bleed = 0.0;
            string currentComment = "";
            int publicationID = 0;
            DateTime pubDate = DateTime.MinValue;
            bool hasMask = false;
            bool hasTiles = false;
            bool hasRaster = false;
            string errmsg = "";
            CCDBaccess db = new CCDBaccess();

            string pagesizeinfo = "";
            db.GetSizeInfoForPage(masterCopySeparationSet, ref pagesizeinfo, out  errmsg);
            if (pagesizeinfo != "")
            {
                Session["CurrentPageFormat"] = pagesizeinfo;
            }
            else                                                                                                             
            {
                db.GetPageFormatForPage(masterCopySeparationSet, ref xdim, ref  ydim, ref  bleed, out  errmsg);
                if (xdim > 0.0 && ydim > 0.0)
                {
                    string s = string.Format("{0:F1} x {1:F1}", xdim, ydim);
                    if (bleed > 0)
                        s += string.Format(" bleed {0:F1}", bleed);
                    Session["CurrentPageFormat"] = s;
                }
            }

            DisplayPageName();
          
            bool showSep = (string)Session["ShowSep"] == "C" ||
                            (string)Session["ShowSep"] == "M" ||
                            (string)Session["ShowSep"] == "Y" ||
                            (string)Session["ShowSep"] == "K" ||
                            (string)Session["ShowSep"] == "DNS";

            db.GetComment(masterCopySeparationSet, out currentComment, out publicationID, out pubDate, out errmsg);
            if ((bool)Application["SetCommentInPrePollPageTable"])
                currentComment = db.GetPrePollMessage(masterCopySeparationSet, 350, out errmsg);

            string previewGUID = Globals.MakePreviewGUID(publicationID, pubDate);

            Session["CurrentComment"] = (bool)Session["SetCommentOnDisapproval"] ? currentComment : "";
            SetCommentBox(currentComment);

            if ((bool)Application["ThumbnailShowMask"])
                hasMask = db.HasMask(masterCopySeparationSet, out errmsg);

            string realRasterPath = "";
            string virtualRasterPath = "";

            if ((bool)Application["ShowRasterImage"])
            {
                if (Globals.HasRasterTileFolder(masterCopySeparationSet, version, publicationID, pubDate, ref realRasterPath, ref virtualRasterPath))
                {
                    hasRaster = true;
                    Session["ImagePathRaster"] = virtualRasterPath;
                    Session["HasRaster"] = true;
                }
            }

            string realPath = "";
            string virtualPath = "";
            if (Globals.HasTileFolder(masterCopySeparationSet, version, publicationID, pubDate, false, ref realPath, ref virtualPath))
            {
                hasTiles = true;
                Session["ImagePath"] = virtualPath;
                Session["HasTiles"] = true;
            }


            if (hasMask && hasTiles)
            {
                if (Globals.HasTileFolder(masterCopySeparationSet, version, publicationID, pubDate, true, ref realPath, ref virtualPath))
                    Session["ImagePathMask"] = virtualPath;
                else
                    hasMask = false;
            }

            bool hasGUIDname =  realPath.IndexOf("====") != -1;
            bool hasCyan, hasMagenta, hasYellow, hasBlack,hasDNS;

            if (hasGUIDname)
            {
                hasCyan = System.IO.File.Exists(Global.sRealImageFolder + "\\" + previewGUID + "====" + masterCopySeparationSet.ToString() + "_C.jpg");
                hasMagenta = System.IO.File.Exists(Global.sRealImageFolder + "\\" + previewGUID + "====" + masterCopySeparationSet.ToString() + "_M.jpg");
                hasYellow = System.IO.File.Exists(Global.sRealImageFolder + "\\" + previewGUID + "====" + masterCopySeparationSet.ToString() + "_Y.jpg");
                hasBlack = System.IO.File.Exists(Global.sRealImageFolder + "\\" + previewGUID + "====" + masterCopySeparationSet.ToString() + "_K.jpg");
                hasDNS = System.IO.File.Exists(Global.sRealImageFolder + "\\" + previewGUID + "====" + masterCopySeparationSet.ToString() + "_dns.jpg");
            }
            else
            {
                hasCyan = System.IO.File.Exists(Global.sRealImageFolder + "\\" + masterCopySeparationSet.ToString() + "_C.jpg");
                hasMagenta = System.IO.File.Exists(Global.sRealImageFolder + "\\" + masterCopySeparationSet.ToString() + "_M.jpg");
                hasYellow = System.IO.File.Exists(Global.sRealImageFolder + "\\" + masterCopySeparationSet.ToString() + "_Y.jpg");
                hasBlack = System.IO.File.Exists(Global.sRealImageFolder + "\\" + masterCopySeparationSet.ToString() + "_K.jpg");
                hasDNS = System.IO.File.Exists(Global.sRealImageFolder + "\\" + masterCopySeparationSet.ToString() + "_dns.jpg");
            }

            string realPdfPagePath;
            string virtualPdfPagePath;

            bool hasPDF = Globals.HasPdfPage(masterCopySeparationSet, out realPdfPagePath, out virtualPdfPagePath, (bool)Application["ShowAnnotatedPDF"]);

            bool hasPDFCMYKSplit = hasPDF && hasTiles;

            SetRadToolbarVisible2("C", isMono == false && hasCyan && forcePDF == false);
            SetRadToolbarVisible2("M", isMono == false && hasMagenta && forcePDF == false);
            SetRadToolbarVisible2("Y", isMono == false && hasYellow && forcePDF == false);
            SetRadToolbarVisible2("K", isMono == false && hasBlack && forcePDF == false);
            SetRadToolbarVisible2("Dns", isMono == false && hasDNS && forcePDF == false);
            SetRadToolbarVisible2("PDF", hasPDF);
            SetRadToolbarVisible2("CMYK", forcePDF == false);
            SetRadToolbarVisible2("PDFCMYK", hasPDFCMYKSplit);
            SetRadToolbarVisible2("Raster", hasRaster);

            if (hasTiles == false || showSep)
            {
                if (Globals.HasPreview(masterCopySeparationSet, version, publicationID, pubDate, false, ref realPath, ref virtualPath, showSep ? (string)Session["ShowSep"] : ""))
                {
                    Session["ImagePath"] = virtualPath;
                }
                if (hasMask)
                {
                    if (Globals.HasPreview(masterCopySeparationSet, version, publicationID, pubDate, true, ref realPath, ref virtualPath, showSep ? (string)Session["ShowSep"] : ""))
                        Session["ImagePathMask"] = virtualPath;
                    else
                        hasMask = false;
                }
            }

            SetRadToolbarVisible2("Mask",  forcePDF == false && hasMask && (string)Session["ImagePathMask"] != "" && (bool)Application["ThumbnailShowMask"]);

            if (forcePDF && hasPDF)
            {
                simpleFlash = 2;
                Session["ShowSep"] = "PDF";
                Session["ImagePath"] = virtualPdfPagePath;
                SetRadToolbarVisible2("Forward", false);
                SetRadToolbarVisible2("Backward", false);
                SetRadToolbarVisible2("ForwardOnly", false);
                SetRadToolbarVisible2("BackwardOnly", false);
                SetRadToolbarVisible2("ForwardDis", false);
                SetRadToolbarVisible2("BackwardDis", false);
                SetRadToolbarVisible2("PDF", false);
                SetRadToolbarVisible2("PDFCMYK", false);
                SetRadToolbarVisible2("Raster", false);
                return;
            }

            if ((string)Session["ImagePath"] == "")
            {
                // Display error message..
                return;
            }

            if ((string)Session["ImagePath"] != "")
                Session["RealImagePath"] = realPath;	// For mail attachment only

            // Finally - set the global sImagePath var used by flash module

            sImagePath = (string)Session["ImagePath"] + "/";

            // Default to CMYK if missing file(s)
            if ((string)Session["ShowSep"] == "C" && hasCyan == false ||
                (string)Session["ShowSep"] == "M" && hasMagenta == false ||
                (string)Session["ShowSep"] == "Y" && hasYellow == false ||
                (string)Session["ShowSep"] == "K" && hasBlack == false ||
                (string)Session["ShowSep"] == "DNS" && hasDNS == false ||
                (string)Session["ShowSep"] == "PDF" && hasPDF == false ||
                (string)Session["ShowSep"] == "PDFCMYK" && (hasPDF == false || hasTiles == false) ||
                (string)Session["ShowSep"] == "MASK" && (string)Session["ImagePathMask"] == "" ||
                (string)Session["ShowSep"] == "RASTER" && (string)Session["ImagePathRaster"] == "")

                    Session["ShowSep"] = "CMYK";

    /*        if ((bool)Application["NoCache"])
            {
                string randomnumber = "&randowm_number=" + DateTime.Now.Ticks.ToString();
             sFlashVars += randomnumber;
            }
            */
           

            if (hasGUIDname)
            {
                printimagepath = (string)Session["ShowSep"] == "MASK" ?
                    Global.sVirtualImageFolder + "/" + previewGUID + "====" + masterCopySeparationSet.ToString() + "_mask.jpg" :
                    Global.sVirtualImageFolder + "/" + previewGUID + "====" + masterCopySeparationSet.ToString() + ".jpg";
            }
            else
            {
                printimagepath = (string)Session["ShowSep"] == "MASK" ?
                      Global.sVirtualImageFolder + "/" + masterCopySeparationSet.ToString() + "_mask.jpg" :
                      Global.sVirtualImageFolder + "/" + masterCopySeparationSet.ToString() + ".jpg";
            }

            printimagepath = Globals.EncodePreviewName(printimagepath);

            if ((bool)Application["LogUserAccess"] == true)
                LogUserView(masterCopySeparationSet, (string)Session["UserName"]);


            if ((string)Session["ShowSep"] == "PDFCMYK" && hasPDF && hasTiles)
            {
                simpleFlash = 1;
                sImagePath = (string)Session["ImagePath"] + "/";
                Session["ImagePath2"] = virtualPdfPagePath;
                return;
            }

            if ((string)Session["ShowSep"] == "PDF" && hasPDF)
            {
                simpleFlash = 2;
                Session["ImagePath"] = virtualPdfPagePath;
                return;
            }

            SetRadToolbarChecked2("Mask", false);
            if (hasTiles && (string)Session["ShowSep"] == "MASK")
            {
                simpleFlash = 0;
                sImagePath = (string)Session["ImagePathMask"] +"/";
                SetRadToolbarChecked2("Mask", true);
                return;
            }

            SetRadToolbarChecked2("Raster", false);
            if (hasRaster && (string)Session["ShowSep"] == "RASTER" && (bool)Application["ShowRasterImage"])
            {
                simpleFlash = 0;
                sImagePath = (string)Session["ImagePathRaster"] + "/";
                SetRadToolbarChecked2("Raster", true);                
                return;
            }

            if (hasTiles && (string)Session["ShowSep"] == "CMYK")
            {
                simpleFlash = 0;
                sImagePath = (string)Session["ImagePath"] + "/";
                return;
            }

            // All other images are shown without tiles
            simpleFlash = 3; // (bool)Session["mobiledevice"] || (bool)Session["UseHTML5"] ? 3 : 1;
            sImagePath = (string)Session["ImagePath"];

        }

        /*private void PrintImage(int mastercopyseparationset)
		{
			//sImagePath = (string)Session["ImagePath"];

			string im = Global.sVirtualPreviewFolder + "/" + mastercopyseparationset.ToString() + ".jpg";

			Response.Redirect("PrintFrame.aspx?imagepath="+im);
		} */

        private void SetRadToolbarLabel(string buttonID, string text)
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue(buttonID);
            if (item == null)
                return;
            item.Text = text;
        }

        private void SetRadToolbarEnable(string buttonID, bool enable)
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue(buttonID);
            if (item == null)
                return;
            item.Enabled = enable;
        }

        private void SetRadToolbarEnable2(string buttonID, bool enable)
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar2.FindItemByValue(buttonID);
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
        private void SetRadToolbarChecked2(string buttonID, bool check)
        {
            Telerik.Web.UI.RadToolBarButton item = (Telerik.Web.UI.RadToolBarButton)RadToolBar2.FindItemByValue(buttonID);
            if (item == null)
                return;
            item.Checked = check;
        }


        private void SetRadToolbarVisible(string buttonID, bool visible)
        {
            Telerik.Web.UI.RadToolBarButton item = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue(buttonID);
            if (item == null)
                return;
            item.Visible = visible;
        }

        private void SetRadToolbarVisible2(string buttonID, bool visible)
        {
            Telerik.Web.UI.RadToolBarButton item = (Telerik.Web.UI.RadToolBarButton)RadToolBar2.FindItemByValue(buttonID);
            if (item == null)
                return;
            item.Visible = visible;
        }

        private void SetRadToolbarTooltip(string buttonID, string text)
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue(buttonID);
            if (item == null)
                return;
            item.ToolTip = text;
        }

        private void SetRadToolbarLabel2(string buttonID, string text)
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar2.FindItemByValue(buttonID);
            if (item == null)
                return;
            item.Text = text;
        }

        private void SetRadToolbarTooltip2(string buttonID, string text)
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar2.FindItemByValue(buttonID);
            if (item == null)
                return;
            item.ToolTip = text;
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

        private void SetRadToolbarTooltip(string buttonID, string labelID, string text)
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue(buttonID);
            if (item == null)
                return;
            Label label = (Label)item.FindControl(labelID);
            if (label == null)
                return;
            label.ToolTip = text;
        }

	}
}
