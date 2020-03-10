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
using System.Web.Caching;
using System.Drawing.Imaging;
using WebCenter4.Classes;
using System.Text;
using System.Globalization;
using System.Resources;
using System.Threading;
using PDFlib_dotnet;
using System.Collections.Generic;

namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for Thumbnailview.
	/// </summary>
	public partial class Thumbnailview2 : System.Web.UI.Page
	{
		protected int nImagesPerRow;
		protected int nImageWidth;
		protected int nImageHeight;
		protected int nWindowWidth;
		protected int nRefreshTime;
        protected int updateTree;
		System.Drawing.Image ImageSmallLock;
		System.Drawing.Image ImageBigLock;

		public int nMinImageHeight  = 300;
		
        public int nScollPos = 0;
        public bool returnedFromZoom = false;

		public	string tooltipClickImage = "Click on page to show preview";
		
        protected void Page_Load(object sender, EventArgs e)
		{
            Page.MaintainScrollPositionOnPostBack = true;

			if ((string)Session["UserName"] == null)
				Response.Redirect("~/SessionTimeout.htm");

			if ((string)Session["UserName"] == "")
				Response.Redirect("/Denied.htm");

			// Clean heatmap
            if (Session["PopupImageInk"] != null /*&& txtReturnedFromPopup.Text == "1"*/)
			{
                string sInkFileUrl = (string)Session["PopupImageInk"];
                if (sInkFileUrl != null && sInkFileUrl != "")
                {
                    string inkImage = Request.MapPath(Request.ApplicationPath) + sInkFileUrl.Substring(2);
                    try
                    {
                        System.IO.File.Delete(inkImage);
                    }
                    catch
                    {
                    }
                    HiddenReturendFromPopup.Value = "0";
                    Session["PopupImageInk"] = "";
                }
			}
           
			// Test if this is a postback caused by a approval state change
			if (Request.QueryString["set"] != null) 
			{
				try 
				{
					string sepset = Request.QueryString["set"];
					string pageName = Request.QueryString["pagename"];
					string colors = Request.QueryString["colors"];
					string approval = Request.QueryString["approval"];

					PrepareZoom(Int32.Parse(sepset), colors, approval,pageName);
					
				}
				catch 
				{
					;
				}
			}

            /*  returnedFromZoom = false;
              if (Request.QueryString["Refresh"] != null) 
                  if (Request.QueryString["Refresh"] == "2")
                      returnedFromZoom = true;
              */

            updateTree = 0;

            if (!Page.IsPostBack)
			{
				SetLanguage();
                SetSplitButtonValues();
				
                Telerik.Web.UI.RadToolBarButton item = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("HideApproved");
                if (item != null)
                    item.Checked = (bool)Session["HideApproved"];
                item = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("HideCommon");
                if (item != null)
                    item.Checked = (bool)Session["HideCommonPages"];

                Session["PopupImageInk"] = "";

                updateTree = 0;

            }  

			nRefreshTime = (int)Session["RefreshTime"];
			
			ImageSmallLock = System.Drawing.Image.FromFile(Request.MapPath(Request.ApplicationPath) + "/Images/SmallRedLock.gif");
			ImageBigLock = System.Drawing.Image.FromFile(Request.MapPath(Request.ApplicationPath) + "/Images/BigLock.gif");

            SetScreenSize();
            RegisterScrollSaveScript();

            if (!Page.IsPostBack || HiddenReturendFromPopup.Value != "0")
			{
				DoDataBind(false);
            }

            if (HiddenReturendFromPopup.Value == "2")
            {
                Session["RefreshTree"] = true;
                updateTree = 1;
            }

            SetMessageIcon();
          //  RegisterScrollSaveScript();
            SetRefreshheader();

            HiddenReturendFromPopup.Value = "0";
		//	txtReturnedFromColor.Attributes.Add("style", "color:Transparent;background-color:Transparent;border-color:Transparent;border-style:None;height:2px;width:2px;");
		//	txtReturnedFromPopup.Attributes.Add("style", "color:Transparent;background-color:Transparent;border-color:Transparent;border-style:None;height:2px;width:2px;");

			//Loop through all windows in the WindowManager.Windows collection
			foreach (Telerik.Web.UI.RadWindow win in RadWindowManager1.Windows)
			{
				//Set whether the first window will be visible on page load
				win.VisibleOnPageLoad = false;
			}
		
		}

        private void RegisterScrollSaveScript()
        {
            String csname = "OnSubmitScript";
            Type cstype = this.GetType();

            // Get a ClientScriptManager reference from the Page class.
            ClientScriptManager cs = Page.ClientScript;

            // Check to see if the OnSubmit statement is already registered.
            if (!cs.IsOnSubmitStatementRegistered(cstype, csname))
            {
                String cstext = "GetScrollPosition();";
                cs.RegisterOnSubmitStatement(cstype, csname, cstext);
            }
        }

        private void SetRefreshheader()
        {
//            if (nRefreshTime > 0)
 //               Response.AddHeader("Refresh", nRefreshTime.ToString());

            if ((bool)Application["NoCache"])
            {
                //Response.ClearHeaders();
                Response.AppendHeader("Cache-Control", "no-cache"); 
                //Response.AppendHeader("Cache-Control", "private"); 
                Response.AppendHeader("Cache-Control", "no-store"); 
                Response.AppendHeader("Cache-Control", "must-revalidate"); 
                Response.AppendHeader("Cache-Control", "max-stale=0"); 
                Response.AppendHeader("Cache-Control", "post-check=0"); 
                Response.AppendHeader("Cache-Control", "pre-check=0"); 
                Response.AppendHeader("Pragma", "no-cache"); 
                Response.AppendHeader("Keep-Alive", "timeout=3, max=993"); 
                Response.AppendHeader("Expires", "Mon, 26 Jul 1997 05:00:00 GMT"); 
              //  Response.CacheControl = "Private";
                Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
                Response.Cache.SetCacheability(HttpCacheability.NoCache); 
                Response.Cache.SetNoStore();
                
            }
        }

        private void SetScreenSize()
        {
            int w = 0;
            int h = 0;
            if (HiddenX.Value != "" && HiddenY.Value != "")
            {
                w = Globals.TryParse(HiddenX.Value, 0);
                h = Globals.TryParse(HiddenY.Value, 0);
            }

            if (w <= 0 || h<= 0)
            {
                w = Globals.TryParseCookie(Request, "ScreenWidthPages", 600);
                h = Globals.TryParseCookie(Request, "ScreenHeightPages", 400);
            }       
          
			if (w <= 0)
				w =  (int)Session["WindowWidth"] > 0 ? (int)Session["WindowWidth"] : 600;
			if (h <= 0)
				h = (int)Session["WindowHeight"] > 0 ? (int)Session["WindowHeight"] : 400;

            // nScrollPos is exposed in aspx code used in clientcode to set scrillbar after load
			nScollPos = 0;
           // if (Page.IsPostBack || returnedFromZoom)
			//{
                if (HiddenScrollPos.Value != "")
                    nScollPos = Globals.TryParse(HiddenScrollPos.Value, 0);

                if (nScollPos <= 0)
                {
                    try
                    {
                        string s = Request.Cookies["ScrollY"].Value;
                        if (s != null)
                            if (s != "undefined")
                                nScollPos = Int32.Parse(s);
                    }
                    catch
                    {
                        nScollPos = 0;
                    }
                }
			//}

			Session["WindowHeight"] = h;
			Session["WindowWidth"] = w;
			nWindowWidth = w;
            
           
            if ((bool)Session["mobiledevice"])
            {
                if ((int)Session["PagesPerRow"] >= 4)
                    Session["PagesPerRow"] = 4;
            }

            nImagesPerRow = (int)Session["PagesPerRow"];
//            nImageWidth = ((int)Session["WindowWidth"] - 6 - 16 * nImagesPerRow ) / nImagesPerRow;
            nImageWidth = (w - 2 - 8 * nImagesPerRow) / nImagesPerRow;

            // Blank dummy image is 83x118 - set a default height used if no pages are avail. - otherwise height is set by first detected image.
            nImageHeight = nImageWidth * 118;
            nImageHeight /= 83;

        }

		private void SetMessageIcon()
		{
			if (Globals.MessageSystem)
			{
				CCDBaccess db = new CCDBaccess();
				string errmsg = "";

				int publicationID = Session["SelectedPublication"] != null ? Globals.GetIDFromName("PublicationNameCache",(string)Session["SelectedPublication"]) : 0;
				DateTime pubDate =  Session["SelectedPubDate"] != null ? (DateTime)Session["SelectedPubDate"] : DateTime.MinValue;
				int type = db.HasUnreadMessages((string)Session["UserName"], publicationID, pubDate, out errmsg);
				
			} 
		}

		private void SetLanguage()
		{
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            lblChooseProduct.Text = Global.rm.GetString("txtChooseProduct");
            tooltipClickImage = Global.rm.GetString("txtTooltipClickImages");
//            SetRadToolbarTooltip("Refresh", Global.rm.GetString("txtRefresh"));
            SetRadToolbarLabel("Refresh", Global.rm.GetString("txtRefresh"));

            SetRadToolbarLabel("HideApproved", Global.rm.GetString("txtHideApprovedPages"));
            SetRadToolbarTooltip("HideApproved", Global.rm.GetString("txtTooltipHideApprovedPages"));
            SetRadToolbarLabel("HideCommon", Global.rm.GetString("txtHideDuplicates"));
            SetRadToolbarTooltip("HideCommon", Global.rm.GetString("txtTooltipHideDuplicates"));

            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue("UploadFiles");
            if (item != null)
            {
                item.Text = Global.rm.GetString("txtUploadFiles");
                item.ToolTip = Global.rm.GetString("txtTooltipUploadFiles");
                item.Enabled = Globals.AllowUpload && (bool)Session["MayUpload"];
                item.Visible = Globals.AllowUpload && (bool)Session["MayUpload"];
                if ((bool)Session["mobiledevice"])
                    item.Visible = false;
            }

            item = RadToolBar1.FindItemByValue("Download");
            if (item != null)
            {
                item.Text = Global.rm.GetString("txtDownloadAll");
                item.ToolTip = Global.rm.GetString("txtTooltipDownloadAll");
                item.Enabled = Globals.HideDownload == false;
                item.Visible = Globals.HideDownload == false;

                if ((bool)Session["mobiledevice"])
                    item.Visible = false;
            }

            item = RadToolBar1.FindItemByValue("Messages");
            if (item != null)
            {
                item.Text = Global.rm.GetString("txtMessages");
                item.ToolTip = Global.rm.GetString("txtTooltipMessages");
                item.Enabled = Globals.MessageSystem;
                item.Visible = Globals.MessageSystem;
                if ((bool)Session["mobiledevice"])
                    item.Visible = false;
            }

            item = RadToolBar1.FindItemByValue("ApproveAll");
            if (item != null)
            {
                item.Text = Global.rm.GetString("txtApproveAll");
                item.ToolTip = Global.rm.GetString("txtTooltipApproveAll");
                item.Enabled = Globals.ShowApproveAllButton && (bool)Session["MayApprove"];
                item.Visible = Globals.ShowApproveAllButton && (bool)Session["MayApprove"];
            }

            item = RadToolBar1.FindItemByValue("CustomAction");
            if (item != null)
            {
                item.Text = Global.rm.GetString("txtCustomAction");
                item.ToolTip = Global.rm.GetString("txtTooltipCustomAction");
                item.Enabled = Globals.ShowCustomAction; 
                item.Visible = Globals.ShowCustomAction;
                if ((bool)Session["mobiledevice"])
                    item.Visible = false;
            }

            item = RadToolBar1.FindItemByValue("ReadyMessage");
            if (item != null)
            {
                item.Text = Global.rm.GetString("txtReadyAction");
                item.ToolTip = Global.rm.GetString("txtTooltipReadyAction");
                item.Enabled = Globals.ShowReadyAction;
                item.Visible = Globals.ShowReadyAction;
            }

            Telerik.Web.UI.RadToolBarSplitButton itemsb = (Telerik.Web.UI.RadToolBarSplitButton)RadToolBar1.FindItemByValue("PagesPerRowSelector");
            if (itemsb == null)
                return;
            itemsb.ToolTip = Global.rm.GetString("txtTooltipImagesPerRow");

            for (int i = 4; i <= 20; i+=2)
            {
                string s = "PagesPerRow" + i.ToString();
                Telerik.Web.UI.RadToolBarButton subitem = (Telerik.Web.UI.RadToolBarButton)itemsb.Buttons.FindItemByValue(s);
                if (subitem != null)
                    subitem.Text = Global.rm.GetString("txtImagesPerRow") + " " + i.ToString();
            }

            itemsb = (Telerik.Web.UI.RadToolBarSplitButton)RadToolBar1.FindItemByValue("RefreshtimeSelector");
            if (itemsb == null)
                return;
            itemsb.ToolTip = Global.rm.GetString("txtTooltipRefreshTime");

            for (int i = 10; i <= 120; i+=10)
            {
                string s = "RefreshTime" + i.ToString();
                Telerik.Web.UI.RadToolBarButton subitem = (Telerik.Web.UI.RadToolBarButton)itemsb.Buttons.FindItemByValue(s);
                if (subitem != null)
                    subitem.Text = Global.rm.GetString("txtRefreshTime") + " " + i.ToString();
            }  
        }

        protected void SetSplitButtonValues()
        {
            Telerik.Web.UI.RadToolBarSplitButton itemsb = (Telerik.Web.UI.RadToolBarSplitButton)RadToolBar1.FindItemByValue("PagesPerRowSelector");
            if (itemsb == null)
                return;

            if ((bool)Session["mobiledevice"])
            {
                if ((int)Session["PagesPerRow"] >= 6)
                    Session["PagesPerRow"] = 6;
            }

            if ((int)Session["PagesPerRow"] < 4 || (int)Session["PagesPerRow"] > 20)
                Session["PagesPerRow"] = 4;
            string s = "PagesPerRow" + ((int)Session["PagesPerRow"]).ToString();

            for (int i = 0; i < itemsb.Buttons.Count; i++)
            {
                if (itemsb.Buttons[i].Value == s)
                {
                    itemsb.DefaultButtonIndex = i;
                    break;
                }
            }

            itemsb = (Telerik.Web.UI.RadToolBarSplitButton)RadToolBar1.FindItemByValue("RefreshtimeSelector");
            if (itemsb == null)
                return;

            if ((int)Session["RefreshTime"] < 10 || (int)Session["RefreshTime"] > 120)
                Session["RefreshTime"] = 60;
            s = "RefreshTime" + ((int)Session["RefreshTime"]).ToString();

            for (int i = 0; i < itemsb.Buttons.Count; i++)
            {
                if (itemsb.Buttons[i].Value == s)
                {
                    itemsb.DefaultButtonIndex = i;
                    break;
                }
            }

        }

		public void DoDataBind(bool firstTime)
		{
			if ((string)Session["SelectedPublication"] == "")
			{
				lblChooseProduct.Visible = true;
                lblChooseProduct.Height = 20;
				return;
			}
			lblChooseProduct.Visible = false;
            lblChooseProduct.Height = 0;

			CCDBaccess db = new CCDBaccess();
		
			string errmsg = "";

            int pressID = Globals.GetIDFromName("PressNameCache", (string)Session["SelectedPress"]);
            if (Globals.UsePressGroups)
                pressID =  Globals.GetIDFromName("PressGroupNameCache", (string)Session["SelectedPress"]);

            Session["SelectedProduction"] =  db.GetProductionID(ref pressID, Globals.GetIDFromName("PublicationNameCache", (string)Session["SelectedPublication"]), (DateTime)Session["SelectedPubDate"], 0, out  errmsg);

    		bool hideApproved = (bool)Session["HideApproved"];
			bool hideCommon = (bool)Session["HideCommonPages"];
			DataTable dt = db.GetThumbnailPageCollection(hideApproved, hideCommon, firstTime, out errmsg);  
			if (dt == null) 
			{
				lblError.Text = errmsg;
                Global.logging.WriteLog("GetThumbnailPageCollection() " + errmsg);
				return;
			}
           // Global.logging.WriteLog("GetThumbnailPageCollection() returned " +dt.Rows.Count.ToString()+ " rows");

			DataTable dt2 = db.GetThumbnailPageCollection2(hideApproved, hideCommon, firstTime, out errmsg);  
			if (dt2 != null) 
			{
                //Global.logging.WriteLog("GetThumbnailPageCollection2() returned " + dt2.Rows.Count.ToString() + " rows");
                nMinImageHeight = 300;
					
				DateTime serverTime = db.GetDate(out errmsg);

				ICollection ic = CreateImageDataSource(pressID, dt, dt2, serverTime);
				if (ic != null)
				{
					datalistImages.DataSource = ic;
					datalistImages.DataBind();
				}
			}
			else 
			{
				lblError.Text = errmsg;
                Global.logging.WriteLog("GetThumbnailPageCollection2() " + errmsg);

			}
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
			this.datalistImages.ItemCreated += new System.Web.UI.WebControls.DataListItemEventHandler(this.datalistImages_ItemCreated);
			this.datalistImages.ItemCommand += new System.Web.UI.WebControls.DataListCommandEventHandler(this.Thumbnail_ItemCommand);
			this.datalistImages.ItemDataBound += new System.Web.UI.WebControls.DataListItemEventHandler(this.datalistImages_ItemdataBound);
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion

        public DataRow GenerateDummyPage(ref DataTable dt)
        {
            DataRow drfill = dt.NewRow();
            drfill["ImageName"] = "../Images/Spacer.gif";
            drfill["ImageDesc"] = "";
            drfill["ImageDesc2"] = "";
            drfill["ImageDesc3"] = "";
            drfill["ImageNumber"] = "0&0&0&0";
            drfill["ImageQueryString"] = "0&0&0&0";
            drfill["ImageBorder"] = " BORDER-RIGHT: white 4px solid; BORDER-TOP: white 4px solid; BORDER-LEFT: white 4px solid; BORDER-BOTTOM: white 4px solid;";

            return drfill;
        }

        public ICollection CreateImageDataSource(int pressID, DataTable dstable, DataTable ds2table, DateTime serverTime)
		{
            DataTable dt = new DataTable();

            DataColumn newColumn;
            newColumn = dt.Columns.Add("ImageName", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ImageDesc", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ImageDesc2", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ImageDesc3", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ImageNumber", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ImageQueryString", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ImageBorder", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("FTPStatus", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("FTPMessage", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("PreflightStatus", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("PreflightMessage", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("RIPStatus", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("RIPMessage", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ColorStatus", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ColorMessage", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("InkSaveStatus", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("InkSaveMessage", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("CanPrint", Type.GetType("System.Int32"));
            newColumn = dt.Columns.Add("IsColorLocked", Type.GetType("System.Int32"));
            newColumn = dt.Columns.Add("IsApprovalLocked", Type.GetType("System.Int32"));

            newColumn = dt.Columns.Add("ShowHistory", Type.GetType("System.Int32"));
            newColumn = dt.Columns.Add("History", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("DeadlineStatus", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("CanUpload", Type.GetType("System.Int32"));

            newColumn = dt.Columns.Add("UniquePage", Type.GetType("System.Int32"));
            newColumn = dt.Columns.Add("Locked", Type.GetType("System.Int32"));
            newColumn = dt.Columns.Add("Tooltip", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("Alert", Type.GetType("System.Int32"));
            

            string NoPagePath = Request.MapPath(Request.ApplicationPath) + "/Images/NoPage.gif";
            string NoPagePanoPath = Request.MapPath(Request.ApplicationPath) + "/Images/NoPage.gif";

            string PageComingPath = Request.MapPath(Request.ApplicationPath) + "/Images/PageComing.gif";
            string PageComingPanoPath = Request.MapPath(Request.ApplicationPath) + "/Images/PageComing.gif";

            string PageMissingPath = Request.MapPath(Request.ApplicationPath) + "/Images/PageMissing.gif";
            string PageMissingPanoPath = Request.MapPath(Request.ApplicationPath) + "/Images/PageMissing.gif";

            if ((bool)Application["FlatLook"])
            {
                NoPagePath = Request.MapPath(Request.ApplicationPath) + "/Images/NoPage_Flat.gif";
                NoPagePanoPath = Request.MapPath(Request.ApplicationPath) + "/Images/NoPagePano_Flat.gif";

                PageComingPath = Request.MapPath(Request.ApplicationPath) + "/Images/PageComing_Flat.gif";
                PageComingPanoPath = Request.MapPath(Request.ApplicationPath) + "/Images/PageComing_Flat.gif";

                PageMissingPath = Request.MapPath(Request.ApplicationPath) + "/Images/PageMissing_Flat.gif";
                PageMissingPanoPath = Request.MapPath(Request.ApplicationPath) + "/Images/PageMissing_Flat.gif";

            }


            String sSection = (String)Session["SelectedSection"];
            String sEdition = (String)Session["SelectedEdition"];
           
            String sPublication = (String)Session["SelectedPublication"];
            DateTime tPubDate = (DateTime)Session["SelectedPubDate"];

            bool hideEdition = Globals.GetCacheRowCount("EditionNameCache") < 2;
            bool hideSection = Globals.GetCacheRowCount("SectionNameCache") < 2;

            string previewGUID = Globals.MakePreviewGUID(Globals.GetIDFromName("PublicationNameCache", sPublication), tPubDate);

            DateTime prevPubDate = new DateTime(1975, 1, 1, 0, 0, 0);
            string prevPublication = "";
            string prevEdition = "";
            string prevSection = "";

            string realPath = "";

            bool bAllowPartialProofs = Globals.AllowPartialProofs;

            bool bDeadLineAlarm = false;

            DataView dv = dstable.DefaultView;

            int nImageNumber = 0;

            // Try to find image dimensions..	
            int nJpegImageWidth = nImageWidth;
            int nJpegImageHeight = nImageHeight;
            bool hasFoundDimensions = false;
            bool isSingleSectionProduct = true;
            string defaultSection = "";
            if (dv.Count > 0)
                defaultSection = (string)dv[0].Row["Section"];

            foreach (DataRowView row in dv)
            {
                if (isSingleSectionProduct && (string)row["Section"] != defaultSection)
                {
                    isSingleSectionProduct = false;
                    break;
                }
            }

            foreach (DataRowView row in dv)
            {

                int nStatus = Globals.GetStatusID((string)row["Status"], 0);

                if (nStatus >= 10 && hasFoundDimensions == false && ((int)row["ProofStatus"] >= 10 || ((int)row["ProofStatus"] >= 5 && bAllowPartialProofs)))
                {
                    string sMasterCopySeparationSet = row["MasterCopySeparationSet"].ToString();
                    //  realPath = Global.sRealImageFolder + "\\" + productionGUID + "====" + sMasterCopySeparationSet + ".jpg"; 
                    string fileTitle = previewGUID + "====" + sMasterCopySeparationSet;
                    realPath = Global.sRealImageFolder + "\\" + fileTitle + ".jpg";
                    bool hasGUIDfile = Globals.OldFileNames == false && System.IO.File.Exists(realPath);
                    if (hasGUIDfile == false)
                    {
                        fileTitle = sMasterCopySeparationSet;
                        realPath = Global.sRealImageFolder + "\\" + fileTitle + ".jpg";
                        hasGUIDfile = System.IO.File.Exists(realPath);
                    }

                    if (hasGUIDfile)
                    {
                        try
                        {
                            System.Drawing.Image TestImage = System.Drawing.Image.FromFile(realPath);
                            nJpegImageWidth = TestImage.Size.Width;
                            nJpegImageHeight = TestImage.Size.Height;
                            double ratio = (double)nImageWidth * (double)TestImage.Size.Height / (double)TestImage.Size.Width + 0.5;
                            TestImage.Dispose();
                            nImageHeight = (int)ratio;
                            hasFoundDimensions = true;
                        }
                        catch //(Exception e)
                        {
                            lblError.ForeColor = Color.Red;
                            lblError.Text = "IMAGE NOT FOUND -  (" + sMasterCopySeparationSet + ".jpg)";
                        }
                        break;
                    }
                }

            }
            ArrayList alReproofCandidates = new ArrayList();
            alReproofCandidates.Clear();

            foreach (DataRowView row in dv)
            {
                int nStatus = Globals.GetStatusID((string)row["Status"], 0);

                if (Globals.UseChannels)
                {
                    bool bHasChannelError = false;
                    bool bHasTransmitting = false;
                   
                    if (bHasChannelError)
                        nStatus = 26;
                    else if (bHasTransmitting)
                        nStatus = 25;
                }

                DateTime thisPubDate = (DateTime)row["PubDate"];
                if (thisPubDate != prevPubDate && prevPubDate.Year >= 2000)
                {
                    // New pubdate coming next! - make sure new pubdate starts on fresh line - fill old line with dummies
                    int nDummyImages = nImagesPerRow - nImageNumber % nImagesPerRow;
                    if (nDummyImages != nImagesPerRow)
                    {
                        for (int im = 0; im < nDummyImages; im++)
                        {
                            nImageNumber++;
                            dt.Rows.Add(GenerateDummyPage(ref dt));
                        }
                    }

                }
                prevPubDate = thisPubDate;

                string thisPublication = (string)row["Publication"];
                if (thisPublication != prevPublication && prevPublication != "")
                {
                    // New publication coming next! - make sure new publication starts on fresh line - fill old line with dummies
                    int nDummyImages = nImagesPerRow - nImageNumber % nImagesPerRow;
                    if (nDummyImages != nImagesPerRow)
                    {
                        for (int im = 0; im < nDummyImages; im++)
                        {
                            nImageNumber++;
                            dt.Rows.Add(GenerateDummyPage(ref dt));
                        }
                    }
                }
                prevPublication = thisPublication;

                string thisEdition = (string)row["Edition"];
                if (thisEdition != prevEdition && prevEdition != "")
                {
                    // New product coming next! - make sure new product starts on fresh line - fill old line with dummies
                    int nDummyImages = nImagesPerRow - nImageNumber % nImagesPerRow;
                    if (nDummyImages != nImagesPerRow)
                    {
                        for (int im = 0; im < nDummyImages; im++)
                        {
                            nImageNumber++;
                            dt.Rows.Add(GenerateDummyPage(ref dt));
                        }
                    }
                }

                prevEdition = thisEdition;

                string thisSection = (string)row["Section"];
                if (thisSection != prevSection && prevSection != "")
                {
                    // New product coming next! - make sure new product starts on fresh line - fill old line with dummies
                    int nDummyImages = nImagesPerRow - nImageNumber % nImagesPerRow;
                    if (nDummyImages != nImagesPerRow)
                    {
                        for (int im = 0; im < nDummyImages; im++)
                        {
                            nImageNumber++;
                            dt.Rows.Add(GenerateDummyPage(ref dt));
                        }
                    }
                }
                prevSection = thisSection;

                DataRow dr = dt.NewRow();

                string tInputTimeStamp = Globals.DateTime2TimeStamp((DateTime)row["InputTime"]);

                if ((int)row["PageType"] == 1 || (int)row["PageType"] == 2)
                {
                    realPath = NoPagePanoPath;
                    dr["ImageName"] = Globals.FlatLook ? "NoPagePano_Flat.gif" : "NoPagePano.gif";
                }
                else
                {
                    realPath = NoPagePath;
                    dr["ImageName"] = Globals.FlatLook ? "NoPage_Flat.gif" : "NoPage.gif";
                }

                int nMasterCopySeparationSet = (int)row["MasterCopySeparationSet"];
                string thisUniquePublication = Globals.GetNameFromID("PublicationNameCache", (int)row["UniquePublicationID"]);
                if (thisUniquePublication == "")
                    thisUniquePublication = thisPublication;

                int nPageType = (int)row["PageType"];
                int nMaxVersion = (int)row["Version"];

                if (nStatus >= 10 && (int)row["ProofStatus"] < 10)
                {
                    realPath = PageComingPath;
                    dr["ImageName"] = Globals.FlatLook ? "PageComing_Flat.gif"  : "PageComing.gif";
                    //Global.logging.WriteLog("File not polled or proofed - " + nMasterCopySeparationSet.ToString());
                }

                bool hasPageToPreview = false;
                
                if (nStatus >= 10 && (int)row["ProofStatus"] >= 10 || ((int)row["ProofStatus"] >= 5 && bAllowPartialProofs ))
                {
                    int thumbNailSize = 0;
                    foreach (DataRow ds2row in ds2table.Rows)
                    {
                        if (nMasterCopySeparationSet == (int)ds2row["MasterCopySeparationSet"])
                        {
                            thumbNailSize = (int)ds2row["ThumbnailSize"];
                            break;
                        }
                    }

                    bool bGotThumbnailError = false;
                    bool hasGUIDfile = false;
                    string fileTitle = previewGUID + "====" + nMasterCopySeparationSet.ToString() + "-" + nMaxVersion.ToString();
                    string sRealThumbnailPath = Global.sRealThumbnailFolder + "\\" + fileTitle + ".jpg";
                    string sFileToShow = Global.sVirtualThumbnailFolder + "/" + fileTitle + ".jpg";
                    string folderToTest = Global.sRealImageFolder + "\\" + fileTitle + "\\";

                    /*
                    if (Globals.OldFileNames )
                    {
                        if ((bool)Application["UseVersionPreviews"] && nMaxVersion > 0)
                            folderToTest = Global.sRealImageFolder + "\\" + previewGUID + "====" + nMasterCopySeparationSet.ToString() + "-" + nMaxVersion.ToString() + "\\";
                        else
                            folderToTest = Global.sRealImageFolder + "\\" + previewGUID + "====" + nMasterCopySeparationSet.ToString() + "-" + nMaxVersion.ToString() + "\\";
                    }
                    else
                    {
                        if ((bool)Application["UseVersionPreviews"] && nMaxVersion > 0)
                            folderToTest = Global.sRealImageFolder + "\\" + nMasterCopySeparationSet.ToString() + "-" + nMaxVersion.ToString() + "\\";
                        else
                            folderToTest = Global.sRealImageFolder + "\\" +  nMasterCopySeparationSet.ToString() + "-" + nMaxVersion.ToString() + "\\";
                    }
                    */

                    bool hasTimeStampFile = false;
                    if (Globals.UseInputTimeInThumbnailName)
                    {
                        fileTitle = nMasterCopySeparationSet.ToString() + "-" + tInputTimeStamp;

                        sRealThumbnailPath = Global.sRealThumbnailFolder + "\\" + fileTitle + ".jpg";
                        sFileToShow = Global.sVirtualThumbnailFolder + "/" + fileTitle + ".jpg";

                        hasTimeStampFile = System.IO.File.Exists(sRealThumbnailPath);
                       // Global.logging.WriteLog("TIMESTAMPTHUMBNAIL - File.Exists(" + sRealThumbnailPath + ") = " + hasTimeStampFile.ToString());
                        if (Globals.UseVersionPreviews)
                            folderToTest = Global.sRealImageFolder + "\\" + nMasterCopySeparationSet.ToString() + "-" + nMaxVersion.ToString() + "\\";
                        else
                            folderToTest = Global.sRealImageFolder + "\\" + nMasterCopySeparationSet.ToString() + "\\";

                        hasGUIDfile = hasTimeStampFile;
                    }

                    if (hasTimeStampFile == false)
                    {
                        if (Globals.UseVersionThumbnails && nMaxVersion > 0)
                        {
                            hasGUIDfile = Globals.OldFileNames == false && System.IO.File.Exists(sRealThumbnailPath);
                            if (hasGUIDfile == false)
                            {
                                fileTitle = nMasterCopySeparationSet.ToString() + "-" + nMaxVersion.ToString();

                                sRealThumbnailPath = Global.sRealThumbnailFolder + "\\" + fileTitle + ".jpg";
                                sFileToShow = Global.sVirtualThumbnailFolder + "/" + fileTitle + ".jpg";
                                if (Globals.UseVersionPreviews)
                                    folderToTest = Global.sRealImageFolder + "\\" + fileTitle + "\\";
                                else
                                    folderToTest = Global.sRealImageFolder + "\\" + nMasterCopySeparationSet.ToString() + "\\";

                                hasGUIDfile = System.IO.File.Exists(sRealThumbnailPath);

                                Global.logging.WriteLog("1 - File.Exists(" + sRealThumbnailPath + ") = " + hasGUIDfile.ToString());
                            }

                           /* if (hasGUIDfile && thumbNailSize > 0 && (bool)Application["CheckThumbnailSize"])
                            {
                                try
                                {
                                    System.IO.FileInfo f = new System.IO.FileInfo(sRealThumbnailPath);
                                    if (f.Length != thumbNailSize)
                                    {
                                        bGotThumbnailError = true;
                                        Global.logging.WriteLog("2 - Thumbnail size mismatch " + sRealThumbnailPath);
                                    }
                                }
                                catch
                                {
                                    Global.logging.WriteLog("Exception in FileInfo call for file " + sRealThumbnailPath);
                                }
                            }*/

                        }
                        else // if ((bool)Application["UseVersionThumbnails"] == false)
                        {
                            fileTitle = previewGUID + "====" + nMasterCopySeparationSet.ToString();
                            sRealThumbnailPath = Global.sRealThumbnailFolder + "\\" + fileTitle + ".jpg";
                            sFileToShow = Global.sVirtualThumbnailFolder + "/" + fileTitle + ".jpg";
                            folderToTest = Global.sRealImageFolder + "\\" + fileTitle + "\\";

                            hasGUIDfile = Globals.OldFileNames == false && System.IO.File.Exists(sRealThumbnailPath);
                            if (hasGUIDfile == false)
                            {
                                fileTitle = nMasterCopySeparationSet.ToString();
                                sRealThumbnailPath = Global.sRealThumbnailFolder + "\\" + fileTitle + ".jpg";
                                sFileToShow = Global.sVirtualThumbnailFolder + "/" + fileTitle + ".jpg";

                                if (Globals.UseVersionPreviews)
                                    folderToTest = Global.sRealImageFolder + "\\" + nMasterCopySeparationSet.ToString() + "-" + nMaxVersion.ToString() + "\\";
                                else
                                    folderToTest = Global.sRealImageFolder + "\\" + nMasterCopySeparationSet.ToString() + "\\";

                                hasGUIDfile = System.IO.File.Exists(sRealThumbnailPath);

                                Global.logging.WriteLog("3 - File.Exists(" + sRealThumbnailPath + ") = " + hasGUIDfile.ToString());

                            }

                            /*
                            if (hasGUIDfile && thumbNailSize > 0 && (bool)Application["CheckThumbnailSize"])
                            {
                                try
                                {
                                    System.IO.FileInfo f = new System.IO.FileInfo(sRealThumbnailPath);
                                    if (thumbNailSize != f.Length)
                                    {
                                        bGotThumbnailError = true;
                                        Global.logging.WriteLog("4 - Thumbnail size mismatch " + sRealThumbnailPath);
                                    }
                                }
                                catch
                                {
                                    Global.logging.WriteLog("Exception in FileInfo call for file " + sRealThumbnailPath);
                                }
                            }*/

                        }
                    }

                    if (bGotThumbnailError)
                    {
                        realPath = PageMissingPath;
                        alReproofCandidates.Add(nMasterCopySeparationSet);
                        dr["ImageName"] = Globals.FlatLook ? "PageMissing_Flat.gif" : "PageMissing.gif";
                    } 
                    else
                    {
                        if (hasGUIDfile == false)  // Meaning here if thumbnail exists! (guid or not..)
                        {
                            realPath = PageComingPath;
                            alReproofCandidates.Add(nMasterCopySeparationSet);
                            dr["ImageName"] = Globals.FlatLook ? "PageComing_Flat.gif" : "PageComing.gif";
                        }
                        else
                        {
                            if (System.IO.Directory.Exists(folderToTest) == false)
                            {

                                Global.logging.WriteLog("Directory.Exists(" + folderToTest + ") = false");
                                realPath = PageComingPath;
                                alReproofCandidates.Add(nMasterCopySeparationSet);
                                dr["ImageName"] = Globals.FlatLook ? "PageComing_Flat.gif" : "PageComing.gif";
                            }
                            else
                            {
                                Global.logging.WriteLog("Directory.Exists(" + folderToTest + ") = true");

                                if (Globals.CheckTileXML(folderToTest, thisUniquePublication, tPubDate))
                                {
                                    dr["ImageName"] = Globals.EncodePreviewName(sFileToShow);

                                    /* if ((bool)Application["CheckJpegComment"])
                                     {
                                         string thpath = Global.sRealThumbnailFolder + "\\" + fileTitle + ".jpg";
                                         if (Globals.CheckJPGComment(thpath, thisPublication, tPubDate) == false)
                                         {
                                             Global.logging.WriteLog("CheckJPGComment failed for " + thpath);
                                             realPath = PageMissingPath;
                                             dr["ImageName"] = (bool)Application["FlatLook"] ? "PageMissing_Flat.gif" : "PageMissing.gif";
                                         }
                                     }*/
                                }
                                else
                                { 
                                    Global.logging.WriteLog("CheckTileXML failed for folder" + folderToTest);
                                    alReproofCandidates.Add(nMasterCopySeparationSet);
                                }
                            }
                        }
                    }

                    hasPageToPreview = true;
                }


                if (Globals.ShowPdfIfExists && nStatus == 0 && hasPageToPreview == false)
                {
                    string realPdfPath = "";
                    string virtPdfPath = "";
                    if (Globals.HasPdfPage(nMasterCopySeparationSet, out realPdfPath, out virtPdfPath))
                    {
                        if (Globals.FlatLook)
                            dr["ImageName"] = (int)row["PageType"] == 0 ? "PdfPage_Flat.gif" : "PdfPagePano_Flat.gif";
                        else
                            dr["ImageName"] = (int)row["PageType"] == 0 ? "PdfPage.gif" : "PdfPagePano.gif";
                    }
                }


                dr["CanPrint"] = ((bool)Session["MayHardProof"] && nStatus >= 10 && (int)row["ProofStatus"] >= 10) ? 1 : 0;
                dr["ShowHistory"] = Globals.ShowHistory ? 1 : 0;

                dr["CanUpload"] = Globals.AllowUpload && (bool)Session["MayUpload"]; //&& (bool)Application["PlanPageNameIsFileName"];

                dr["IsColorLocked"] = ((uint)row["EmailStatus"] & 0x00000100) > 0 ? 1 : 0;
                dr["IsApprovalLocked"] = ((uint)row["EmailStatus"] & 0x00000200) > 0 ? 1 : 0;
                dr["UniquePage"] = (int)row["UniquePage"];
                dr["Tooltip"] = "";
                dr["Locked"] = (int)row["Locked"];
                dr["Alert"] = (int)row["Alert"];

                if ((int)row["Locked"]>0 && Globals.ShowBigLock)
                    dr["ImageName"] = Globals.FlatLook ? "PageLocked_Flat.gif" : "PageLockedgif";


                string sPageName = (string)row["Page"];
                bool bIsNumber = true;
                foreach (char c in sPageName)
                {
                    if (c < '0' || c > '9')
                    {
                        bIsNumber = false;
                        break;
                    }
                }

                // Make 'Spread' text

                if (nPageType == 1)
                {
                    if (bIsNumber)
                    {
                        int nPg = 1 + Int32.Parse(sPageName);
                        sPageName += " & " + nPg.ToString();
                    }
                    sPageName += " " + Global.rm.GetString("txtSpread");
                }

                if (thisSection != "" && hideSection == false && isSingleSectionProduct == false && bIsNumber)
                    dr["ImageDesc"] = thisSection + " " + sPageName;
                else
                    dr["ImageDesc"] = sPageName;

                if (Globals.ThumbnailsCommentAsPageNumber)
                {
                    if ((string)row["Comment"] != "")
                        dr["ImageDesc"] = (string)row["Comment"];
                }

                // JFM - show most advanced status for page..
                int maxStatus = 0;
                foreach (DataRow ds2row in ds2table.Rows)
                {
                    if (nMasterCopySeparationSet == (int)ds2row["MasterCopySeparationSet"])
                    {
                        maxStatus = (int)ds2row["MaxStatus"];
                        break;
                    }
                }

                if (maxStatus > 0 && maxStatus > nStatus)
                    nStatus = maxStatus;

                //dr["ImageDesc2"] = "V" + nMaxVersion.ToString() +"  " + (string)row["Status"];
                dr["ImageDesc2"] = "V" + nMaxVersion.ToString() + "  " + Globals.GetStatusName(nStatus, 0);
                DateTime deadline = (DateTime)row["Deadline"];
                DateTime inputtime = (DateTime)row["InputTime"];
                dr["ImageDesc3"] = "";

                if (Globals.ShowPlanPageName || Globals.ShowPlanPageNameAsTooltip)
                {
                    string finalPlanPageName = Globals.PlanPageNameFormat;
                    if (pressID == Globals.PlanPageNameFormatSpecialPressID)
                        finalPlanPageName = Globals.PlanPageNameFormatSpecial;
                    string finalPlanPageName2 = ""; 

                    // 90% chance it is just %I...
                    if (finalPlanPageName == "%I" || finalPlanPageName == "%i")
                    {
                        if (inputtime.Year > 2000)
                        {
                            if (Globals.PlanPageNameInputTimeFormat == 2)
                                finalPlanPageName = finalPlanPageName.Replace("%I", string.Format("{0:00}-{1:00} {2:00}:{3:00}", inputtime.Day, inputtime.Month, inputtime.Hour, inputtime.Minute));
                            else
                                finalPlanPageName = finalPlanPageName.Replace("%I", string.Format("{0:00}:{1:00}", inputtime.Hour, inputtime.Minute));

                            if (Globals.PlanPageNameInputTimeFormat == 2)
                                finalPlanPageName2 = string.Format("{0:00}-{1:00} {2:00}:{3:00}", inputtime.Day, inputtime.Month, inputtime.Hour, inputtime.Minute);
                            else
                                finalPlanPageName2 = string.Format("{0:00}:{1:00}", inputtime.Hour, inputtime.Minute);
                        }
                        else
                            finalPlanPageName = finalPlanPageName.Replace("%I", "");

                    }
                    else
                    {
                        string edName = (string)row["Edition"];
                        string edNameAlias = Globals.LookupInputAlias("Edition", edName);
                        string secName = (string)row["Section"];
                        string secNameAlias = Globals.LookupInputAlias("Section", secName);
                        string pubName = (string)row["Publication"];
                        string pubNameAlias = Globals.LookupInputAlias("Publication", pubName);

                        if (Globals.GetCacheRowCount("PublicationNamingConvensionCache") > 0)
                        {
                            int pubID = Globals.GetIDFromName("PublicationNameCache", pubName);
                            string s = Globals.GetNameFromID("PublicationNamingConvensionCache", pubID);
                            if (s != "")
                                finalPlanPageName = s;
                        }

                        finalPlanPageName = finalPlanPageName.Replace("%Q", (string)row["PlanPageName"]);

                        string pageName = (string)row["Page"];
                        int pageNumber = Globals.TryParse(pageName, 1);
                        finalPlanPageName = finalPlanPageName.Replace("%2N", pageNumber.ToString("00."));
                        finalPlanPageName = finalPlanPageName.Replace("%3N", pageNumber.ToString("000."));
                        finalPlanPageName = finalPlanPageName.Replace("%4N", pageNumber.ToString("0000."));
                        finalPlanPageName = finalPlanPageName.Replace("%N", pageName);

                        finalPlanPageName = finalPlanPageName.Replace("%K", (string)row["Comment"]);
                        int nnn = (int)row["Pagination"];
                        finalPlanPageName = finalPlanPageName.Replace("%A", nnn.ToString());
                        nnn = (int)row["PageIndex"];
                        finalPlanPageName = finalPlanPageName.Replace("%U", nnn.ToString());

                        finalPlanPageName = finalPlanPageName.Replace("%-1E", edName.Substring(edName.Length - 1, 1));
                        finalPlanPageName = finalPlanPageName.Replace("%E", edName);
                        edName += "          ";
                        finalPlanPageName = finalPlanPageName.Replace("%1E", edName.Substring(0, 1));
                        finalPlanPageName = finalPlanPageName.Replace("%2E", edName.Substring(0, 2));
                        finalPlanPageName = finalPlanPageName.Replace("%3E", edName.Substring(0, 3));
                        finalPlanPageName = finalPlanPageName.Replace("%4E", edName.Substring(0, 4));
                        finalPlanPageName = finalPlanPageName.Replace("%5E", edName.Substring(0, 5));
                        finalPlanPageName = finalPlanPageName.Replace("%6E", edName.Substring(0, 6));
                        finalPlanPageName = finalPlanPageName.Replace("%7E", edName.Substring(0, 7));
                        finalPlanPageName = finalPlanPageName.Replace("%8E", edName.Substring(0, 8));

                        finalPlanPageName = finalPlanPageName.Replace("%e", edNameAlias);
                        edNameAlias += "          ";

                        finalPlanPageName = finalPlanPageName.Replace("%-1e", edNameAlias.Substring(edNameAlias.Length - 1, 1));
                        finalPlanPageName = finalPlanPageName.Replace("%1e", edNameAlias.Substring(0, 1));

                        finalPlanPageName = finalPlanPageName.Replace("%2e", edNameAlias.Substring(0, 2));
                        finalPlanPageName = finalPlanPageName.Replace("%3e", edNameAlias.Substring(0, 3));
                        finalPlanPageName = finalPlanPageName.Replace("%4e", edNameAlias.Substring(0, 4));
                        finalPlanPageName = finalPlanPageName.Replace("%5e", edNameAlias.Substring(0, 5));
                        finalPlanPageName = finalPlanPageName.Replace("%6e", edNameAlias.Substring(0, 6));
                        finalPlanPageName = finalPlanPageName.Replace("%7e", edNameAlias.Substring(0, 7));
                        finalPlanPageName = finalPlanPageName.Replace("%8e", edNameAlias.Substring(0, 8));

                        finalPlanPageName = finalPlanPageName.Replace("%S", secName);
                        secName += "         ";
                        finalPlanPageName = finalPlanPageName.Replace("%1S", secName.Substring(0, 1));
                        finalPlanPageName = finalPlanPageName.Replace("%2S", secName.Substring(0, 2));
                        finalPlanPageName = finalPlanPageName.Replace("%3S", secName.Substring(0, 3));
                        finalPlanPageName = finalPlanPageName.Replace("%4S", secName.Substring(0, 4));
                        finalPlanPageName = finalPlanPageName.Replace("%5S", secName.Substring(0, 5));
                        finalPlanPageName = finalPlanPageName.Replace("%6S", secName.Substring(0, 6));
                        finalPlanPageName = finalPlanPageName.Replace("%7S", secName.Substring(0, 7));
                        finalPlanPageName = finalPlanPageName.Replace("%8S", secName.Substring(0, 8));


                        finalPlanPageName = finalPlanPageName.Replace("%s", secNameAlias);
                        secNameAlias += "         ";
                        finalPlanPageName = finalPlanPageName.Replace("%1s", secNameAlias.Substring(0, 1));
                        finalPlanPageName = finalPlanPageName.Replace("%2s", secNameAlias.Substring(0, 2));
                        finalPlanPageName = finalPlanPageName.Replace("%3s", secNameAlias.Substring(0, 3));
                        finalPlanPageName = finalPlanPageName.Replace("%4s", secNameAlias.Substring(0, 4));
                        finalPlanPageName = finalPlanPageName.Replace("%5s", secNameAlias.Substring(0, 5));
                        finalPlanPageName = finalPlanPageName.Replace("%6s", secNameAlias.Substring(0, 6));
                        finalPlanPageName = finalPlanPageName.Replace("%7s", secNameAlias.Substring(0, 7));
                        finalPlanPageName = finalPlanPageName.Replace("%8s", secNameAlias.Substring(0, 8));

                        finalPlanPageName = finalPlanPageName.Replace("%P", pubName);
                        pubName += "         ";
                        finalPlanPageName = finalPlanPageName.Replace("%1P", pubName.Substring(0, 1));
                        finalPlanPageName = finalPlanPageName.Replace("%2P", pubName.Substring(0, 2)); 
                        finalPlanPageName = finalPlanPageName.Replace("%3P", pubName.Substring(0, 3));
                        finalPlanPageName = finalPlanPageName.Replace("%4P", pubName.Substring(0, 4));
                        finalPlanPageName = finalPlanPageName.Replace("%5P", pubName.Substring(0, 5));
                        finalPlanPageName = finalPlanPageName.Replace("%6P", pubName.Substring(0, 6));
                        finalPlanPageName = finalPlanPageName.Replace("%7P", pubName.Substring(0, 7));
                        finalPlanPageName = finalPlanPageName.Replace("%8P", pubName.Substring(0, 8));

                        if (pubNameAlias.Length >= 3)
                            finalPlanPageName = finalPlanPageName.Replace("%-3p", pubNameAlias.Substring(pubNameAlias.Length - 3, 3));
                        if (pubNameAlias.Length >= 2)
                            finalPlanPageName = finalPlanPageName.Replace("%-2p", pubNameAlias.Substring(pubNameAlias.Length - 2, 2));
                        finalPlanPageName = finalPlanPageName.Replace("%-1p", pubNameAlias.Substring(pubNameAlias.Length - 1, 1));

                        finalPlanPageName = finalPlanPageName.Replace("%p", pubNameAlias);
                        pubNameAlias += "         ";
                        finalPlanPageName = finalPlanPageName.Replace("%1p", pubNameAlias.Substring(0, 1));
                        finalPlanPageName = finalPlanPageName.Replace("%2p", pubNameAlias.Substring(0, 2));
                        finalPlanPageName = finalPlanPageName.Replace("%3p", pubNameAlias.Substring(0, 3)); 
                        finalPlanPageName = finalPlanPageName.Replace("%4p", pubNameAlias.Substring(0, 4));
                        finalPlanPageName = finalPlanPageName.Replace("%5p", pubNameAlias.Substring(0, 5));
                        finalPlanPageName = finalPlanPageName.Replace("%6p", pubNameAlias.Substring(0, 6));
                        finalPlanPageName = finalPlanPageName.Replace("%7p", pubNameAlias.Substring(0, 7));
                        finalPlanPageName = finalPlanPageName.Replace("%8p", pubNameAlias.Substring(0, 8));

                      

                        nnn = (int)row["Version"];
                        finalPlanPageName = finalPlanPageName.Replace("%V", nnn.ToString());
                        DateTime pubDate = (DateTime)row["PubDate"];

                        finalPlanPageName = finalPlanPageName.Replace("%D[dd]", string.Format("{0:00}", pubDate.Day));

                        finalPlanPageName = finalPlanPageName.Replace("%D[ddmm]", string.Format("{0:00}", pubDate.Day) + string.Format("{0:00}", pubDate.Month));
                        finalPlanPageName = finalPlanPageName.Replace("%D[mmdd]", string.Format("{0:00}", pubDate.Month) + string.Format("{0:00}", pubDate.Day));

                        finalPlanPageName = finalPlanPageName.Replace("%D[yymmdd]", string.Format("{0:00}", pubDate.Year - 2000) + string.Format("{0:00}", pubDate.Month) + string.Format("{0:00}", pubDate.Day));
                        finalPlanPageName = finalPlanPageName.Replace("%D[yyyymmdd]", string.Format("{0:0000}", pubDate.Year) + string.Format("{0:00}", pubDate.Month) + string.Format("{0:00}", pubDate.Day));

                        finalPlanPageName = finalPlanPageName.Replace("%D[ddmmyy]", string.Format("{0:00}", pubDate.Day) + string.Format("{0:00}", pubDate.Month) + string.Format("{0:00}", pubDate.Year - 2000));
                        finalPlanPageName = finalPlanPageName.Replace("%D[ddmmyyyy]", string.Format("{0:00}", pubDate.Day) + string.Format("{0:00}", pubDate.Month) + string.Format("{0:0000}", pubDate.Year - 2000));

                        // Use default if no date format specifier in format string..
                        string pubDateString = Globals.PlanPageNameDateFormat;
                        pubDateString = pubDateString.Replace("YYYY", string.Format("{0:0000}", pubDate.Year));
                        pubDateString = pubDateString.Replace("YY", string.Format("{0:00}", pubDate.Year - 2000));
                        pubDateString = pubDateString.Replace("MM", string.Format("{0:00}", pubDate.Month));
                        pubDateString = pubDateString.Replace("DD", string.Format("{0:00}", pubDate.Day));

                        finalPlanPageName = finalPlanPageName.Replace("%D", pubDateString);
                        finalPlanPageName = finalPlanPageName.Replace("%W", string.Format("{0:00}-{1:00} {2:00}:{3:00}", deadline.Month, deadline.Day, deadline.Hour, deadline.Minute));

                        //		%I: InputTime format DD-MM HH:MM
                        //		%i: InputTime format HH:MM:SS
                        if (inputtime.Year > 2000)
                        {
                            if (Globals.PlanPageNameInputTimeFormat == 2)
                                finalPlanPageName = finalPlanPageName.Replace("%I", string.Format("{0:00}-{1:00} {2:00}:{3:00}", inputtime.Day, inputtime.Month, inputtime.Hour, inputtime.Minute));
                            else
                                finalPlanPageName = finalPlanPageName.Replace("%I", string.Format("{0:00}:{1:00}", inputtime.Hour, inputtime.Minute));

                            if (Globals.PlanPageNameInputTimeFormat == 2)
                                finalPlanPageName2 = string.Format("{0:00}-{1:00} {2:00}:{3:00}", inputtime.Day, inputtime.Month, inputtime.Hour, inputtime.Minute);
                            else
                                finalPlanPageName2 = string.Format("{0:00}:{1:00}", inputtime.Hour, inputtime.Minute);
                        }
                        else
                            finalPlanPageName = finalPlanPageName.Replace("%I", "");
                    }

                    if (Globals.ShowPlanPageNameAsTooltip == false)
                    {
                       // if (nImageWidth >= 7 && nImageWidth / 7 < finalPlanPageName.Length)
                        //    finalPlanPageName = finalPlanPageName.Substring(0, nImageWidth / 7);
                        dr["ImageDesc3"] = finalPlanPageName;
                    }
                    else
                    {
                        dr["Tooltip"] = finalPlanPageName;
                        dr["ImageDesc3"] = finalPlanPageName2; // hardcoded to input time...
                    }
                }
                    
                if ((int)row["UniquePage"] == 0 && Globals.CommonPageIndication)
                    dr["ImageDesc3"] = dr["ImageDesc3"] + " (Common)";

                if ((int)row["Locked"] == 1 && Globals.AllowPageLock)
                    dr["ImageDesc3"] = dr["ImageDesc3"] + " LOCKED";

             //   if ((int)row["Locked"] == 1 && (bool)Application["AllowPageLock"])
			//		dr["ImageDesc3"] = dr["ImageDesc3"] + " LOCKED";

                string sColor = (string)row["Color"];
                sColor = sColor.Replace(";", "_");

                string sColorStatus = (string)row["ColorStatus"];
                sColorStatus = sColorStatus.Replace(";", "_");

                int nApp = (int)row["Approval"];

                dr["ImageNumber"] = nMasterCopySeparationSet.ToString() + "&" + sColor + "&" + nApp.ToString() + "&" + (string)row["Page"] + "&" + nStatus.ToString() + "&" + (string)dr["ImageDesc3"] + "&" + sColorStatus;
                //		if (realPath !=  NoPagePath && realPath != NoPagePanoPath)
                dr["ImageQueryString"] = "set=" + nMasterCopySeparationSet.ToString() + "&colors=" + sColor + "&approval=" + nApp.ToString() + "&pagename=" + (string)row["Page"];
                //		else		
                //			dr["ImageQueryString"] = "set=0&colors=" + sColor + "&approval=" + nApp.ToString()+ "&pagename=" + (string)row["Page"];

                if (nApp != 1 && nApp != 2)
                {
                    dr["ImageBorder"] = " BORDER-RIGHT: gray 4px solid; BORDER-TOP: gray 4px solid; BORDER-LEFT: gray 4px solid; BORDER-BOTTOM: gray 4px solid;";
                }
                else if (nApp == 2)
                {
                    dr["ImageBorder"] = " BORDER-RIGHT: red 4px solid; BORDER-TOP: red 4px solid; BORDER-LEFT: red 4px solid; BORDER-BOTTOM: red 4px solid;";
                }
                else if (nApp == 1)
                {
                    dr["ImageBorder"] = " BORDER-RIGHT: green 4px solid; BORDER-TOP: green 4px solid; BORDER-LEFT: green 4px solid; BORDER-BOTTOM: green 4px solid;";
                }

                dr["DeadlineStatus"] = "0:19750101000000";

                if (nStatus < 10 && deadline.Year > 2000 && deadline < serverTime)
                {
                    dr["DeadlineStatus"] = "1:" + Globals.DateTime2String(deadline);
                    bDeadLineAlarm = true;
                }

                dr["FTPStatus"] = nStatus >= 10 ? Globals.GetNameFromID("EventNameCache", 130) : Global.rm.GetString("txtFTPStatusNotReceived");
                dr["PreflightStatus"] = Global.rm.GetString("txtPreflightNotPrefligted");
                dr["RIPStatus"] = Global.rm.GetString("txtRIPNotRipped");
                dr["ColorStatus"] = Global.rm.GetString("txtColorNotAnalyzed");
                dr["InkSaveStatus"] = Global.rm.GetString("txtColorNotAnalyzed");               
                dr["InkSaveStatus"] = 0;
                dr["FTPMessage"] = "";
                dr["RIPMessage"] = "";
                dr["PreflightMessage"] = "";
                dr["InkSaveMessage"] = "";
                dr["ColorMessage"] = "";
                
                dr["History"] = "";

                bool hasPreflightError = false;
                bool hasInkSaveError = false;
                foreach (DataRow ds2row in ds2table.Rows)
                {
                    if (nMasterCopySeparationSet == (int)ds2row["MasterCopySeparationSet"])
                    {
                        if ((int)ds2row["PreflightStatus"] >= 110 && (int)ds2row["PreflightStatus"] <= 119)
                            dr["PreflightStatus"] = Globals.GetNameFromID("EventNameCache", (int)ds2row["PreflightStatus"]);

                        if ((int)ds2row["PreflightStatus"] == 116)
                            hasPreflightError = true;

                        if ((int)ds2row["RIPStatus"] >= 120 && (int)ds2row["RIPStatus"] <= 129)
                            dr["RIPStatus"] = Globals.GetNameFromID("EventNameCache", (int)ds2row["RIPStatus"]);

                        if ((int)ds2row["FTPStatus"] >= 130 && (int)ds2row["FTPStatus"] <= 139)
                            dr["FTPStatus"] = Globals.GetNameFromID("EventNameCache", (int)ds2row["FTPStatus"]);

                        if ((int)ds2row["ColorStatus"] >= 140 && (int)ds2row["ColorStatus"] <= 149 && nStatus > 0)
                            dr["ColorStatus"] = Globals.GetNameFromID("EventNameCache", (int)ds2row["ColorStatus"]);

                        if ((int)ds2row["InkSaveStatus"] >= 210 && (int)ds2row["InkSaveStatus"] <= 219)
                            dr["InkSaveStatus"] = Globals.GetNameFromID("EventNameCache", (int)ds2row["InkSaveStatus"]);

                        if ((int)ds2row["InkSaveStatus"] == 216)
                            hasInkSaveError = true;

                        dr["History"] = nMasterCopySeparationSet.ToString() + "&" + sPageName;
                        dr["FTPMessage"] = (string)ds2row["FTPMessage"] + "&" + nMasterCopySeparationSet.ToString();
                        dr["RIPMessage"] = (string)ds2row["RIPMessage"] + "&" + nMasterCopySeparationSet.ToString(); ;
                        dr["PreflightMessage"] = Globals.ShowPitstopReports == false ? (string)ds2row["PreflightMessage"] + "&" + nMasterCopySeparationSet.ToString() : " &" + nMasterCopySeparationSet.ToString();
                        dr["InkSaveMessage"] = Globals.ShowInksaveReports == false ? (string)ds2row["InkSaveMessage"] + "&" + nMasterCopySeparationSet.ToString() : " &" + nMasterCopySeparationSet.ToString();
                        dr["ColorMessage"] = (string)ds2row["ColorMessage"] + "&" + nMasterCopySeparationSet.ToString();
                       
                        break;
                    }
                }

                if ((bool)Application["IndicatePreflightError"] && nStatus == 0 && hasPageToPreview == false && (hasPreflightError == true || hasInkSaveError == true))
                {
                    if ((bool)Application["FlatLook"])
                        dr["ImageName"] = (int)row["PageType"] == 0 ? "PdfErrorPage_Flat.gif" : "PdfErrorPagePano_Flat.gif";
                    else
                        dr["ImageName"] = (int)row["PageType"] == 0 ? "PdfErrorPage.gif" : "PdfErrorPagePano.gif";
                }

                nImageNumber++;
                dt.Rows.Add(dr);
            }

            if (alReproofCandidates.Count > 0)
            {
                RegisterReproofs(ref alReproofCandidates);
            }
			alReproofCandidates.Clear();

            if (bDeadLineAlarm && Globals.ShowDeadline)
            {
                 SetFilterLabel(Global.rm.GetString("txtOverDeadline"), Color.Red);
            }

            return dt.DefaultView;
        }

        private void RegisterReproofs(ref ArrayList alReproofCandidates)
        {
            if ((bool)Application["AutoReproofSystem"] == false)
                return;
            CCDBaccess db = new CCDBaccess();
            string errmsg = "";
            foreach (int masterCopySeparationSet in alReproofCandidates)
            {
                if (db.PeekFlashQueue(masterCopySeparationSet, out errmsg) == true)
                    continue;
                if (db.LookupReproofEvent(masterCopySeparationSet, out errmsg) == true)
                    continue;
                db.InsertPrepollLog(298, masterCopySeparationSet, "WebCenter forced reproof", out errmsg);
                db.Reproof(masterCopySeparationSet, out errmsg);
            }
        }

        private void SetFilterLabel(string text, Color color)
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue("Item3");
            if (item == null)
                return;
            Label label = (Label)item.FindControl("FilterLabel");
            if (label == null)
                return;
            label.Text = text;
            label.ForeColor = color;
        }

		private void PrepareZoom(int masterCopySeparationSet, string colors, string approval, string pageName)
		{
            Session["CurrentCopySeparationSet"] = masterCopySeparationSet;
            Session["CurrentPageName"] = pageName;
            Session["CurrentPageFormat"] = "";
            Session["CurrentVersion"] = 0;
            Session["ImageColors"] = colors;
            Session["ShowSep"] = "CMYK";

            Session["ImagePath"] = "";
            Session["ImagePathMask"] = "";
            Session["RealImagePath"] = "";
            Session["HasTiles"] = false;
            Session["HasRaster"] = false;

            string currentComment = "";
            int publicationID = 0;
            int version = 0;
            DateTime pubDate = DateTime.MinValue;
            bool hasMask = false;
            bool hasTiles = false;
            bool hasRaster = false;
            double xdim = 0.0;
            double ydim = 0.0;
            double bleed = 0.0;
            string errmsg = "";
            CCDBaccess db = new CCDBaccess();

            db.GetComment(masterCopySeparationSet, out currentComment, out publicationID, out pubDate, out errmsg);
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

            Session["CurrentComment"] = (bool)Session["SetCommentOnDisapproval"] ? currentComment : "";
            Session["CurrentApprovalState"] = Globals.TryParse(approval, -1);

            if ((bool)Application["UseVersionPreviews"])
            {
                db.GetPageVersion(masterCopySeparationSet, out version, out errmsg);
                Session["CurrentVersion"] = version;
            }

            if ((bool)Application["ThumbnailShowMask"])
                hasMask = db.HasMask(masterCopySeparationSet, out errmsg);

            string realPath = "";
            string virtualPath = "";
            if (Globals.HasTileFolder(masterCopySeparationSet, version, publicationID, pubDate, false, ref realPath, ref virtualPath))
            {
                hasTiles = true;
                Session["ImagePath"] = virtualPath;
                Session["HasTiles"] = true;
            }

            string realRasterPath = "";
            string virtualRasterPath = "";
            if (Globals.HasRasterTileFolder(masterCopySeparationSet, version, publicationID, pubDate, ref realRasterPath, ref virtualRasterPath))
            {
                hasRaster = true;
                Session["ImagePathRaster"] = virtualRasterPath;
                Session["HasRaster"] = true;
            }

            if (hasMask)
                if (Globals.HasTileFolder(masterCopySeparationSet, version, publicationID, pubDate, true, ref realPath, ref virtualPath))
                    Session["ImagePathMask"] = virtualPath;


            if (hasTiles == false)
            {
                if (Globals.HasPreview(masterCopySeparationSet, version, publicationID, pubDate, false, ref realPath, ref virtualPath, ""))
                {
                    Session["ImagePath"] = virtualPath;
                }
                if (hasMask)
                    if (Globals.HasPreview(masterCopySeparationSet, version, publicationID, pubDate, true, ref realPath, ref virtualPath, ""))
                        Session["ImagePathMask"] = virtualPath;
            }

            if ((string)Session["ImagePath"] != "")
            {
                Session["RealImagePath"] = realPath;	// For mail attachment only

                if ((string)Session["ImagePathMask"] != "" && (bool)Application["DefaultToMaskImage"] == true)
                    Session["ShowSep"] = "MASK";

                Response.Redirect("ZoomviewFlash2.aspx");
              
            }

            if (Globals.ShowPdfIfExists && Globals.HasPdfPage(masterCopySeparationSet, out realPath, out virtualPath))
            {
                Session["ShowSep"] = "PDF";
                Response.Redirect("ZoomviewFlash2.aspx");
            }

		}

		private void PrepareAcrobat(int masterCopySeparationSet, string colors, string approval, string pageName)
		{	
			Session["CurrentCopySeparationSet"] = masterCopySeparationSet;
			Session["CurrentPageName"] = pageName;
			Session["ImageColors"] = colors;
			Session["ImagePath"] = Global.sVirtualHiResFolder + "/" + masterCopySeparationSet.ToString() + ".pdf";
				
			Session["CurrentApprovalState"] = Globals.TryParse(approval, -1);
	
			string realPath = Global.sRealHiresFolder + "\\" + masterCopySeparationSet.ToString()  + ".pdf";
			if(System.IO.File.Exists(realPath) == false)
			{
				lblError.ForeColor = Color.Red;
				lblError.Text = "IMAGE NOT FOUND! (" + masterCopySeparationSet.ToString() + ".pdf)";
			} 
			else
			{
				Response.Redirect("Acrobatview.aspx");
			}
		}

		private void Thumbnail_ItemCommand(object source, System.Web.UI.WebControls.DataListCommandEventArgs e)
		{
          //  lblError.ForeColor = Color.Green;
            string scmd = (string)e.CommandArgument;
            Global.logging.WriteLog("CommandArgument for page = " + scmd);

            string[] sargs = scmd.Split('&');
            int masterCopySeparationSet = 0;
            string pageName = "";
            if (sargs.Length > 1)
                masterCopySeparationSet = Globals.TryParse(sargs[1], 0);

            if (e.CommandName == "FTPInfo" || e.CommandName == "PreflightInfo" || e.CommandName == "RipInfo" || e.CommandName == "ColorInfo" || e.CommandName == "InkSaveInfo")
            {
                string message = sargs[0];
                if (message != "" && masterCopySeparationSet > 0)
                {
                    Session["PopupMessage"] = message;

                    if (e.CommandName == "ColorInfo")
                    {
                        if (sargs.Length > 1)
                        {
                            GenerateHeatMap(masterCopySeparationSet);
                            Session["CurrentCopySeparationSet"] = masterCopySeparationSet;
                            Session["PopupImageInk"] = "../Images/Flats/Heatmap" + masterCopySeparationSet.ToString() + ".jpg";
                            Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowShowColorStatus");
                            mywindow.VisibleOnPageLoad = true;
                        }
                    }
                    else if (e.CommandName == "RipInfo")
                    {
                        Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowShowRipStatus");
                        mywindow.VisibleOnPageLoad = true;
                    }
                    else if (e.CommandName == "FTPInfo")
                    {
                        Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowShowFtpStatus");
                        mywindow.VisibleOnPageLoad = true;
                    }
                    else if (e.CommandName == "InkSaveInfo")
                    {
                        if (Globals.ShowInksaveReports == false)
                        {
                            //	doPopupMessageWindow();
                            Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowInkSaveReport");
                            mywindow.VisibleOnPageLoad = true;
                        }
                        else	// Alwan report directly
                        {
                            if (doPopupInkSaveReport(masterCopySeparationSet) == false)
                            {
                                Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowShowInkSaveStatus");
                                mywindow.VisibleOnPageLoad = true;
                            }
                        }
                    }
                    else if (e.CommandName == "PreflightInfo")
                    {       
                        if (Globals.ShowPitstopReports == false)
                        {
                            //	doPopupMessageWindow();
                            Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowShowPreflightStatus");
                            //mywindow.NavigateUrl=
                            mywindow.VisibleOnPageLoad = true;
                        }
                        else	// Pitstop report directly
                        {
                            if (doPopupPitstopReport(masterCopySeparationSet) == false)
                            {
                                // Fall back to standard popup message
                                Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowShowPreflightStatus");
                                mywindow.VisibleOnPageLoad = true;
                            }
                        }
                    }
                }

                ReBind(false);
                return;
            }

           
            if (e.CommandName == "History")
            {
                masterCopySeparationSet = Globals.TryParse(sargs[0], 0);
                pageName = sargs.Length > 1 ? sargs[1] : "";
                if (masterCopySeparationSet > 0)
                {
                    nRefreshTime = 16000;
                    Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowShowPageHistory");
                    mywindow.Title = Global.rm.GetString("txtPageHistory");
                    mywindow.NavigateUrl = "ShowPageHistory.aspx?mastercopyseparationset=" + masterCopySeparationSet.ToString() + "&page=" + pageName;
                    mywindow.VisibleOnPageLoad = true;

                    //	doPopupHistoryWindow();
                }
                ReBind(false);
                return;
            }


            if (e.CommandName == "Kill")
            {
                masterCopySeparationSet = Globals.TryParse(sargs[0], 0);
                pageName = sargs.Length > 3 ? sargs[3] : "";
                if (masterCopySeparationSet > 0)
                {
                    nRefreshTime = 16000;
                    Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowKill");
                    mywindow.Title = Global.rm.GetString("txtKillPage");
                    mywindow.NavigateUrl = "Pagekill.aspx?mastercopyseparationset=" + masterCopySeparationSet.ToString() + "&page=" + pageName;
                    mywindow.VisibleOnPageLoad = true;
                    //	doPopupHistoryWindow();
                }
                ReBind(false);
                return;
            }


            if (scmd == "")	// Re-bind because no viewstate maintained across postbacks
            {
                ReBind(false);
                return;
            }

            masterCopySeparationSet = Globals.TryParse(sargs[0], 0);

            if (e.CommandName == "View")
            {
                PrepareZoom(masterCopySeparationSet, sargs[1], sargs[2], sargs[3]);
            }

            if (e.CommandName == "Approve")
            {
                if ((bool)Session["MayApprove"] == true)
                {
                    string errmsg;
                    CCDBaccess db = new CCDBaccess();

                    if (db.UpdateApproval((string)Session["UserName"], masterCopySeparationSet, 1, out errmsg) == false)
                    {
                        lblError.ForeColor = Color.Red;
                        lblError.Text = "Could not update approve status";
                    }
                    if ((bool)Session["LogApprove"])
                    {
                        db.UpdateApproveLog(masterCopySeparationSet, 1, true, "", (string)Session["UserName"], out  errmsg);
                    }
                }

            }

            if (e.CommandName == "Disapprove")
            {
                if ((bool)Session["MayApprove"] == true)
                {
                    string errmsg;
                    CCDBaccess db = new CCDBaccess();

                    if (db.UpdateApproval((string)Session["UserName"], masterCopySeparationSet, 2, out errmsg) == false)
                    {
                        lblError.ForeColor = Color.Red;
                        lblError.Text = "Could not update approve status";
                    }

                    if ((bool)Session["LogDisapprove"])
                    {
                        db.UpdateApproveLog(masterCopySeparationSet, 1, false, "", (string)Session["UserName"], out  errmsg);
                    }
                }

                //DoDataBind(false);
                //	updateTree = true;
            }

            if (e.CommandName == "Resetapprove")
            {
                if ((bool)Session["MayApprove"] == true)
                {
                    string errmsg;
                    CCDBaccess db = new CCDBaccess();

                    if (db.UpdateApproval((string)Session["UserName"], masterCopySeparationSet, 0, out errmsg) == false)
                    {
                        lblError.ForeColor = Color.Red;
                        lblError.Text = "Could not update approve status";
                    }

                    if ((bool)Session["LogDisapprove"])
                    {
                        db.UpdateApproveLog(masterCopySeparationSet, 1, false, "Reset approval state", (string)Session["UserName"], out  errmsg);
                    }
                }

                //DoDataBind(false);
                //	updateTree = true;
            }


            if (e.CommandName == "Lock" && Globals.AllowPageLock)
            {
                string errmsg;
                CCDBaccess db = new CCDBaccess();

                if (db.LockPage(masterCopySeparationSet, true, out errmsg) == false)
                {
                    lblError.ForeColor = Color.Red;
                    lblError.Text = "Could not update status";
                }

            }

            if (e.CommandName == "Unlock" && Globals.AllowPageLock)
            {
                string errmsg;
                CCDBaccess db = new CCDBaccess();

                if (db.LockPage(masterCopySeparationSet, false, out errmsg) == false)
                {
                    lblError.ForeColor = Color.Red;
                    lblError.Text = "Could not update status";
                }

            }



            if (e.CommandName == "Color")
            {
                if ((bool)Session["MayKillColor"] == false)
                {
                    ReBind(false);
                    return;
                }

                Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowChangeColor");
                mywindow.Title = Global.rm.GetString("txtChangeColor");
            
                mywindow.NavigateUrl = "ChangeColor.aspx?mastercopyseparationset=" + masterCopySeparationSet.ToString() + "&mastercopyseparationset2=0";

                mywindow.VisibleOnPageLoad = true;

                ReBind(true);
                return;

                //doPopupColorWindow();
            }

            if (e.CommandName == "ViewPDF" && ((bool)Application["AllowPDFDownload"] || (bool)Application["AllowPDFView"]))
            {
                if (sargs[1] == "PDF" || sargs[1] == "PDFmono")
                {
                    if ((bool)Application["AllowPDFDownload"])
                        DownloadPDFPage(masterCopySeparationSet);
                    else
                        PrepareAcrobat(masterCopySeparationSet, sargs[1], sargs[2], sargs[3]);
                }
            }

            if (e.CommandName == "HardProof")
            {
                ReBind(false);
                Session["SelectedMasterSet"] = masterCopySeparationSet;
                Session["SelectedCopyFlatSeparationSet"] = 0;

                Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowHardProof");
                mywindow.Title = Global.rm.GetString("txtReproof");
                mywindow.VisibleOnPageLoad = true;

                return;
            }
         
            if (e.CommandName == "Upload")
            {
                //dr["ImageNumber"] = nMasterCopySeparationSet.ToString() + "&" + sColor + "&" + nApp.ToString() + "&" + (string)row["Page"] + "&" + nStatus.ToString() + "&" + (string)dr["ImageDesc3"];

                ReBind(false);
                Session["SelectedMasterSet"] = masterCopySeparationSet;
                pageName = sargs.Length > 3 ? sargs[3] : "";

                string fileNameToUse = "";
                if (sargs.Length >= 6)
                    fileNameToUse = sargs[5];
                if (sargs.Length >= 3 && (bool)Application["UsePageNameAsUploadFileName"])
                {
                    fileNameToUse = sargs[3];
                   
                }
    
                if (fileNameToUse.IndexOf('.') == -1)
                    fileNameToUse += "." + (string)Application["AddExtensionToUploadFileName"];
                Session["SelectedPlanPageName"] = fileNameToUse;

                if ((int)Application["CustomUploadFileNames"] == 1)
                {
                    string sectionName = "";
                    string editionName = "";
                    
                    string errmsg = "";
                    CCDBaccess db = new CCDBaccess();
                    db.GetPageNameFromMaster(ref pageName, ref sectionName, ref editionName, masterCopySeparationSet, out errmsg);
                 
                    string pubAlias = Globals.LookupInputAlias("Publication", (string)Session["SelectedPublication"]);
                    if (pubAlias == "")
                        pubAlias = (string)Session["SelectedPublication"];

                    string press = (string)Session["SelectedPress"];
                    if (press == "")
                        press = "UPL";

                    if ((string)(string)Session["SelectedSection"] != "")
                        sectionName = (string)(string)Session["SelectedSection"];

                    if ((string)(string)Session["SelectedEdition"] != "")
                        editionName = (string)(string)Session["SelectedEdition"];

                    DateTime pubDate = (DateTime)Session["SelectedPubDate"];
                    Session["SelectedPlanPageName"] = string.Format("{0}_{1}_T_{2:00}{3:00}{4:00}_{5}_1_{6}_{7}.pdf", pubAlias, press, pubDate.Day, pubDate.Month, pubDate.Year - 2000, editionName, sectionName, pageName);
                }
                nRefreshTime = 16000;

                Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowUploadFile");
                mywindow.VisibleOnPageLoad = true;

                return;
            }

            if (e.CommandName == "Retransmit")
            {
                string errmsg;
                CCDBaccess db = new CCDBaccess();

                if ((bool)Application["LocationIsPress"] && (bool)Application["UseChannels"] == false)
                {
                    //					string queryString = "\"RetransmitLocationss.aspx?MasterCopySeparationSet="+masterCopySeparationSet.ToString()+"\"";

                    nRefreshTime = 16000;
                    Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowRetransmitLocations");
                    mywindow.Title = Global.rm.GetString("txtRetransmit");
                    mywindow.NavigateUrl = "RetransmitLocations.aspx?MasterCopySeparationSet=" + masterCopySeparationSet.ToString();

                    mywindow.VisibleOnPageLoad = true;
                }
                else 
                {

                    if (db.Retransmit(masterCopySeparationSet, out errmsg) == false)
                    {
                        lblError.ForeColor = Color.Red;
                        lblError.Text = "Could not update status";
                    }
                }
            }

            if (e.CommandName == "Reprocess")
            {
                nRefreshTime = 16000;
                Session["SelectedMasterSet"] = masterCopySeparationSet;
                Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowReprocess");
                mywindow.Title = Global.rm.GetString("txtReprocessPage");
                mywindow.NavigateUrl = "ReprocessPage.aspx?MasterCopySeparationSet=" + masterCopySeparationSet.ToString();

                mywindow.VisibleOnPageLoad = true;
            }

            if (e.CommandName == "Reproof")
            {
                string errmsg;
                CCDBaccess db = new CCDBaccess();

                if (db.Reproof(masterCopySeparationSet, out errmsg) == false)
                {
                    lblError.ForeColor = Color.Red;
                    lblError.Text = "Could not update status";
                }
            }

            ReBind(true);
            return;
        }

		private void datalistImages_ItemCreated(object sender, System.Web.UI.WebControls.DataListItemEventArgs e)
		{
			//e.Item.Controls[1];
		}

		private void datalistImages_ItemdataBound(object sender, System.Web.UI.WebControls.DataListItemEventArgs e)
		{
            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // Retrieve the Label control in the current DataListItem.

                HtmlTable table = (HtmlTable)e.Item.FindControl("TableThumb");

                Panel thumbTxt = (Panel)e.Item.FindControl("pnlThumbnail");
                Panel thumbTxt2 = (Panel)e.Item.FindControl("pnlPlanPageName");
                thumbTxt2.Visible = Globals.ShowPlanPageName;
                ImageButton colorImage1 = (ImageButton)e.Item.FindControl("btnColor");
                Panel panelBottom = (Panel)e.Item.FindControl("pnlFooter");
                Panel panelBottom2 = (Panel)e.Item.FindControl("pnlFooter2");
                Panel panelBottom3 = (Panel)e.Item.FindControl("pnlFooter3");

                HtmlInputHidden hiddenFTPStatus = (HtmlInputHidden)e.Item.FindControl("hiddenFTPStatus");
                HtmlInputHidden hiddenInkSaveStatus = (HtmlInputHidden)e.Item.FindControl("hiddenInkSaveStatus");
                HtmlInputHidden hiddenPreflightStatus = (HtmlInputHidden)e.Item.FindControl("hiddenPreflightStatus");
                HtmlInputHidden hiddenRIPStatus = (HtmlInputHidden)e.Item.FindControl("hiddenRIPStatus");
                HtmlInputHidden hiddenUniquePage = (HtmlInputHidden)e.Item.FindControl("hiddenUniquePage");

                HtmlInputHidden hiddenColorStatus = (HtmlInputHidden)e.Item.FindControl("hiddenColorStatus");

                HtmlInputHidden hiddenCanUpload = (HtmlInputHidden)e.Item.FindControl("hiddenCanUpload");

                ImageButton btnUpload = (ImageButton)e.Item.FindControl("btnUpload");
                btnUpload.Visible = hiddenCanUpload.Value == "1";
                btnUpload.ToolTip = Global.rm.GetString("txtTooltipUploadFiles");

                ImageButton imgbtnFTP = (ImageButton)e.Item.FindControl("ImageButtonFTPInfo");
                imgbtnFTP.Visible = (bool)Application["ExtendedThumbnailViewShowFTP"];
                imgbtnFTP.ToolTip = Global.rm.GetString("txtHardproof");

                ImageButton imgbtnInkSave = (ImageButton)e.Item.FindControl("ImageButtonInkSaveInfo");
                imgbtnInkSave.Visible = (bool)Application["ExtendedThumbnailViewShowInkSave"];
                imgbtnInkSave.ToolTip = "";

                ImageButton printBtn = (ImageButton)e.Item.FindControl("btnPrinter");
                HtmlInputHidden hidden2 = (HtmlInputHidden)e.Item.FindControl("hiddenImageID2");
                printBtn.ToolTip = Global.rm.GetString("txtHardproof");
                printBtn.Visible = hidden2.Value == "1";

                ImageButton btnReprocess = (ImageButton)e.Item.FindControl("btnReprocess");
                btnReprocess.Visible = (bool)Application["AllowReprocess"] && (bool)Session["MayReprocess"];
                btnReprocess.ToolTip = Global.rm.GetString("txtReprocessPage");

                ImageButton btnResetapprove = (ImageButton)e.Item.FindControl("btnResetApproval");
                btnResetapprove.Visible = (bool)Application["ShowResetApprove"];
                btnResetapprove.ToolTip = Global.rm.GetString("txtResetApprove");
                
                ImageButton killBtn = (ImageButton)e.Item.FindControl("btnKill");
                if (killBtn != null)
                {
                    killBtn.Visible = (bool)Session["AllowPageDelete"] && (bool)Application["AllowPageDelete"];
                    killBtn.ToolTip = Global.rm.GetString("txtKillPageTooltip");
                }
                ImageButton lockBtn = (ImageButton)e.Item.FindControl("btnLock");
                if (lockBtn != null)
                {
                    lockBtn.Visible = Globals.AllowPageLock;
                    //lockBtn.ToolTip = Global.rm.GetString("txtLockPageTooltip");
                }
                ImageButton unlockBtn = (ImageButton)e.Item.FindControl("btnUnlock");
                if (unlockBtn != null)
                {
                    unlockBtn.Visible = Globals.AllowPageLock;
                    //unlockBtn.ToolTip = Global.rm.GetString("txtUnlockPageTooltip");
                }

                ImageButton btnReproof = (ImageButton)e.Item.FindControl("btnReproof");
                btnReproof.Visible = Globals.AllowReproof;



    
                // HARDWIRE OPTION FOR NOW
                //uploadBtn.Visible = false;

                HtmlInputHidden hidden3 = (HtmlInputHidden)e.Item.FindControl("hiddenImageID3");
                bool colorsLocked = hidden3.Value == "1";
                HtmlInputHidden hidden4 = (HtmlInputHidden)e.Item.FindControl("hiddenImageID4");
                bool approvalLocked = hidden4.Value == "1";


                ImageButton imgbtnPDF = (ImageButton)e.Item.FindControl("btnPDF");
                if (imgbtnPDF != null)
                    imgbtnPDF.Visible = (bool)Application["AllowPDFDownload"] || (bool)Application["AllowPDFView"];

              
                ImageButton historyBtn = (ImageButton)e.Item.FindControl("btnHistory");
                HtmlInputHidden hidden6 = (HtmlInputHidden)e.Item.FindControl("hiddenImageID6");
                historyBtn.Visible = hidden6.Value == "1";
                historyBtn.ToolTip = Global.rm.GetString("txtPageHistory");

                ImageButton btnRetransmit = (ImageButton)e.Item.FindControl("btnRetransmit");
                btnRetransmit.Visible = (bool)Application["ReTransmitButton"];
                btnRetransmit.ToolTip = Global.rm.GetString("txtRetransmit");

                HtmlInputHidden hiddenDeadlineStatus = (HtmlInputHidden)e.Item.FindControl("hiddenDeadlineStatus");
                ImageButton ImgbtnAlarm = (ImageButton)e.Item.FindControl("ImgbtnAlarm");
                if (ImgbtnAlarm != null && hiddenDeadlineStatus != null)
                    if (hiddenDeadlineStatus.Value.Length > 0)
                        ImgbtnAlarm.Visible = (bool)Application["ShowDeadline"] && hiddenDeadlineStatus.Value.Substring(0, 1) == "1";

                if (ImgbtnAlarm.Visible && hiddenDeadlineStatus.Value.Length > 2)
                {
                    DateTime t = Globals.String2DateTime(hiddenDeadlineStatus.Value.Substring(2));
                    if (t.Year > 2000)
                        ImgbtnAlarm.ToolTip = Global.rm.GetString("txtDeadLine") + " " + string.Format("{0:00}-{1:00} {2:00}:{3:00}", t.Month, t.Day, t.Hour, t.Minute);
                    else
                        ImgbtnAlarm.ToolTip = Global.rm.GetString("txtDeadLine");
                }

                bool showAlert = false;
                HtmlInputHidden hiddenAlert = (HtmlInputHidden)e.Item.FindControl("hiddenAlert");
                if (hiddenAlert != null)
                    if (hiddenAlert.Value != "" && hiddenAlert.Value != "0")
                        showAlert = true;
               

                if (imgbtnFTP.Visible)
                {
                    imgbtnFTP.ToolTip = "";
                    imgbtnFTP.Height = Globals.SmallEventIcons ? 12 : 18;
                    imgbtnFTP.ImageUrl = Globals.SmallEventIcons ? "../Images/FtpNone_small.gif" : "../Images/FtpNone.gif";
                    if (Globals.FlatLook)
                        imgbtnFTP.ImageUrl = Globals.SmallEventIcons ? "../Images/FtpNone_Flat_small.gif" : "../Images/FtpNone_Flat.gif";

                    if (hiddenFTPStatus.Value.Trim() == Globals.GetNameFromID("EventNameCache", 130))
                    {
                        //imgbtn.ToolTip = Global.rm.GetString("txtTooltipShowFtpInfo");
                        imgbtnFTP.ImageUrl = Globals.SmallEventIcons ? "../Images/FtpOK_small.gif" : "../Images/FtpOK.gif";
                        if (Globals.FlatLook)
                            imgbtnFTP.ImageUrl = Globals.SmallEventIcons ? "../Images/FtpOK_Flat_small.gif" : "../Images/FtpOK_Flat.gif";
                    
                        // Show as warning ...
                        if (showAlert)
                        {
                            imgbtnFTP.ToolTip = Global.rm.GetString("txtTooltipPageNotInPlan");
                            imgbtnFTP.ImageUrl = Globals.SmallEventIcons ? "../Images/FtpWarning_small.gif" : "../Images/FtpWarning.gif";
                            if (Globals.FlatLook)
                                imgbtnFTP.ImageUrl = Globals.SmallEventIcons ? "../Images/FtpWarning_Flat_small.gif" : "../Images/FtpWarning_Flat.gif";
                        }
                    }
                    else if (hiddenFTPStatus.Value.Trim() == Globals.GetNameFromID("EventNameCache", 136))
                    {
                        imgbtnFTP.ToolTip = Global.rm.GetString("txtTooltipShowFtpInfo");
                        imgbtnFTP.ImageUrl = Globals.SmallEventIcons ? "../Images/FtpError_small.gif" : "../Images/FtpError.gif";
                        if (Globals.FlatLook)
                            imgbtnFTP.ImageUrl = Globals.SmallEventIcons ? "../Images/FtpError_Flat_small.gif" : "../Images/FtpError_Flat.gif";
                    }
                    else if (hiddenFTPStatus.Value.Trim() == Globals.GetNameFromID("EventNameCache", 137))
                    {
                        imgbtnFTP.ToolTip = Global.rm.GetString("txtTooltipShowFtpInfo");
                        imgbtnFTP.ImageUrl = Globals.SmallEventIcons ? "../Images/FtpWarning_small.gif" : "../Images/FtpWarning.gif";
                        if (Globals.FlatLook)
                            imgbtnFTP.ImageUrl = Globals.SmallEventIcons ? "../Images/FtpWarning_Flat_small.gif"  : "../Images/FtpWarning_Flat.gif";
                    }
                }

                ImageButton imgbtnPre = (ImageButton)e.Item.FindControl("ImageButtonPreflightInfo");
                imgbtnPre.Visible = (bool)Application["ExtendedThumbnailViewShowPreflight"];

                if (imgbtnPre.Visible)
                {
                    imgbtnPre.Height = Globals.SmallEventIcons ? 12 : 18;
                    imgbtnPre.ToolTip = "";
                    imgbtnPre.ImageUrl = Globals.SmallEventIcons ? "../Images/PreNone_small.gif" : "../Images/PreNone.gif";
                    if (Globals.FlatLook)
                        imgbtnPre.ImageUrl = Globals.SmallEventIcons ? "../Images/PreNone_Flat_small.gif" : "../Images/PreNone_Flat.gif";

                    if (hiddenPreflightStatus.Value.Trim() == Globals.GetNameFromID("EventNameCache", 110))
                    {
                        //imgbtn.ToolTip = Global.rm.GetString("txtTooltipShowPreflightInfo");
                        imgbtnPre.ImageUrl = Globals.SmallEventIcons ? "../Images/PreOK_small.gif" : "../Images/PreOK.gif";
                        if (Globals.FlatLook)
                            imgbtnPre.ImageUrl = Globals.SmallEventIcons ? "../Images/PreOK_Flat_small.gif" : "../Images/PreOK_Flat.gif";
                    }
                    else if (hiddenPreflightStatus.Value.Trim() == Globals.GetNameFromID("EventNameCache", 116))
                    {
                        imgbtnPre.ToolTip = Global.rm.GetString("txtTooltipShowPreflightInfo");
                        imgbtnPre.ImageUrl = Globals.SmallEventIcons ? "../Images/PreError_small.gif" : "../Images/PreError.gif";
                        if (Globals.FlatLook)
                            imgbtnPre.ImageUrl = Globals.SmallEventIcons ? "../Images/PreError_Flat_small.gif"  : "../Images/PreError_Flat.gif";
                    }
                    else if (hiddenPreflightStatus.Value.Trim() == Globals.GetNameFromID("EventNameCache", 117))
                    {
                        imgbtnPre.ToolTip = Global.rm.GetString("txtTooltipShowPreflightInfo");
                        imgbtnPre.ImageUrl = Globals.SmallEventIcons ? "../Images/PreWarning_small.gif" : "../Images/PreWarning.gif";
                        if (Globals.FlatLook)
                            imgbtnPre.ImageUrl = Globals.SmallEventIcons ? "../Images/PreWarning_Flat_small.gif" : "../Images/PreWarning_Flat.gif";

                    }
                }

                ImageButton imgbtnInkSave2 = (ImageButton)e.Item.FindControl("ImageButtonInkSaveInfo");
                imgbtnInkSave2.Visible = (bool)Application["ExtendedThumbnailViewShowInkSave"];

                if (imgbtnInkSave2.Visible)
                {
                    imgbtnInkSave2.Height = Globals.SmallEventIcons ? 12 : 18; 
                    imgbtnInkSave2.ToolTip = "";
                    imgbtnInkSave2.ImageUrl = Globals.SmallEventIcons ? "../Images/InkNone_small.gif" : "../Images/InkNone.gif";
                    if (Globals.FlatLook)
                        imgbtnInkSave2.ImageUrl = Globals.SmallEventIcons ? "../Images/InkNone_Flat_small.gif" : "../Images/InkNone_Flat.gif";

                    if (hiddenInkSaveStatus.Value.Trim() == Globals.GetNameFromID("EventNameCache", 210))
                    {
                        //imgbtnInkSave.ToolTip = Global.rm.GetString("txtTooltipShowInkSaveInfo");
                        imgbtnInkSave2.ImageUrl = Globals.SmallEventIcons ? "../Images/InkOK_small.gif" : "../Images/InkOK.gif";
                        if (Globals.FlatLook)
                            imgbtnInkSave2.ImageUrl = Globals.SmallEventIcons ? "../Images/InkOK_Flat_small.gif" : "../Images/InkOK_Flat.gif";
                    }
                    else if (hiddenInkSaveStatus.Value.Trim() == Globals.GetNameFromID("EventNameCache", 216))
                    {
                        imgbtnInkSave2.ToolTip = Global.rm.GetString("txtTooltipShowInkSaveInfo");
                        imgbtnInkSave2.ImageUrl = Globals.SmallEventIcons ? "../Images/InkError_small.gif" : "../Images/InkError.gif";
                        if (Globals.FlatLook)
                            imgbtnInkSave2.ImageUrl = Globals.SmallEventIcons ? "../Images/InkError_Flat_small.gif" : "../Images/InkError_Flat.gif";
                    }
                    else if (hiddenInkSaveStatus.Value.Trim() == Globals.GetNameFromID("EventNameCache", 217))
                    {
                        imgbtnInkSave2.ToolTip = Global.rm.GetString("txtTooltipShowInkSaveInfo");
                        imgbtnInkSave2.ImageUrl = Globals.SmallEventIcons ? "../Images/InkWarning_small.gif" : "../Images/InkWarning.gif";
                        if (Globals.FlatLook)
                            imgbtnInkSave2.ImageUrl = Globals.SmallEventIcons ? "../Images/InkWarning_Flat_small.gif" : "../Images/InkWarning_Flat.gif";
                    }
                }

                ImageButton imgbtnRIP = (ImageButton)e.Item.FindControl("ImageButtonRIPInfo");

                imgbtnRIP.Visible = (bool)Application["ExtendedThumbnailViewShowRIP"];

                if (imgbtnRIP.Visible)
                {

                    imgbtnRIP.ToolTip = "";
                    imgbtnRIP.Height = Globals.SmallEventIcons ? 12 : 18; 
                    imgbtnRIP.ImageUrl = Globals.SmallEventIcons ? "../Images/RipNone_small.gif" : "../Images/RipNone.gif";
                    imgbtnRIP.CausesValidation = false;
                    if (Globals.FlatLook)
                        imgbtnRIP.ImageUrl = Globals.SmallEventIcons ? "../Images/RipNone_Flat_small.gif" : "../Images/RipNone_Flat.gif";

                    if (hiddenRIPStatus.Value.Trim() == Globals.GetNameFromID("EventNameCache", 120))
                    {
                        //imgbtnRIP.ToolTip = Global.rm.GetString("txtTooltipShowRIPInfo");
                        imgbtnRIP.ImageUrl = Globals.SmallEventIcons ? "../Images/RipOK_small.gif" : "../Images/RipOK.gif";
                        if (Globals.FlatLook)
                            imgbtnRIP.ImageUrl = Globals.SmallEventIcons ? "../Images/RipOK_Flat_small.gif" : "../Images/RipOK_Flat.gif";
                    }
                    else if (hiddenRIPStatus.Value.Trim() == Globals.GetNameFromID("EventNameCache", 126))
                    {
                        imgbtnRIP.ToolTip = Global.rm.GetString("txtTooltipShowRIPInfo");
                        imgbtnRIP.ImageUrl = Globals.SmallEventIcons ? "../Images/RipError_small.gif" : "../Images/RipError.gif";
                        imgbtnRIP.CausesValidation = true;
                        if (Globals.FlatLook)
                            imgbtnRIP.ImageUrl = Globals.SmallEventIcons ? "../Images/RipError_Flat_small.gif" : "../Images/RipError_Flat.gif";

                    }
                    else if (hiddenRIPStatus.Value.Trim() == Globals.GetNameFromID("EventNameCache", 127))
                    {
                        imgbtnRIP.ToolTip = Global.rm.GetString("txtTooltipShowRIPInfo");
                        imgbtnRIP.ImageUrl = Globals.SmallEventIcons ? "../Images/RipWarning_small.gif" : "../Images/RipWarning.gif";
                        imgbtnRIP.CausesValidation = true;
                        if (Globals.FlatLook)
                            imgbtnRIP.ImageUrl = Globals.SmallEventIcons ? "../Images/RipWarning_Flat_small.gif" : "../Images/RipWarning_Flat.gif";
                    }
                }


                ImageButton imgbtnInk = (ImageButton)e.Item.FindControl("ImageButtonColorInfo");

                imgbtnInk.Visible = (bool)Application["ExtendedThumbnailViewShowColorWarning"];

                if (imgbtnInk.Visible)
                {
                    imgbtnInk.ToolTip = "";
                    imgbtnInk.Height = Globals.SmallEventIcons ? 12 : 18;
                    imgbtnInk.ImageUrl = Globals.SmallEventIcons ? "../Images/TacNone_small.gif" : "../Images/TacNone.gif";
                    if (Globals.FlatLook)
                        imgbtnInk.ImageUrl = Globals.SmallEventIcons ? "../Images/TacNone_Flat_small.gif" : "../Images/TacNone_Flat.gif";

                    if (hiddenColorStatus.Value.Trim() == Globals.GetNameFromID("EventNameCache", 140))
                    {
                        //imgbtnInk.ToolTip = Global.rm.GetString("txtTooltipShowColorInfo");
                        imgbtnInk.ImageUrl = Globals.SmallEventIcons ? "../Images/TacOK_small.gif" : "../Images/TacOK.gif";
                        if (Globals.FlatLook)
                            imgbtnInk.ImageUrl = Globals.SmallEventIcons ? "../Images/TacOK_Flat_small.gif" : "../Images/TacOK_Flat.gif";
                    }
                    else if (hiddenColorStatus.Value.Trim() == Globals.GetNameFromID("EventNameCache", 146))
                    {
                        imgbtnInk.ToolTip = Global.rm.GetString("txtTooltipShowColorInfo");
                        imgbtnInk.ImageUrl = Globals.SmallEventIcons ?  "../Images/TacError_small.gif" : "../Images/TacError.gif";
                        if (Globals.FlatLook)
                            imgbtnInk.ImageUrl = Globals.SmallEventIcons ? "../Images/TacError_Flat_small.gif" : "../Images/TacError_Flat.gif";

                    }
                    else if (hiddenColorStatus.Value.Trim() == Globals.GetNameFromID("EventNameCache", 147))
                    {
                        imgbtnInk.ToolTip = Global.rm.GetString("txtTooltipShowColorInfo");
                        imgbtnInk.ImageUrl = Globals.SmallEventIcons ? "../Images/TacWarning_small.gif" : "../Images/TacWarning.gif";
                        if (Globals.FlatLook)
                            imgbtnInk.ImageUrl = Globals.SmallEventIcons ? "../Images/TacWarning_Flat_small.gif" : "../Images/TacWarning_Flat.gif";
                    }
                }

                HtmlInputHidden hidden = (HtmlInputHidden)e.Item.FindControl("hiddenImageID");
                string scmd = hidden.Value;
                scmd = scmd.Replace("&amp;", "&");
                string[] sargs = scmd.Split('&');

                //				dr["ImageNumber"] = nMasterCopySeparationSet.ToString() + "&" + sColor + "&" + nApp.ToString()+ "&" + (string)row["Page"] + "&" + nStatus.ToString();
                //             dr["ImageNumber"] = nMasterCopySeparationSet.ToString() + "&" + sColor + "&" + nApp.ToString() + "&" + (string)row["Page"] + "&" + nStatus.ToString() + "&" + (string)dr["ImageDesc3"] + "&" + sColorStatus;
                //0..6

                bool islocked = false;
                HtmlInputHidden hiddenLocked = (HtmlInputHidden)e.Item.FindControl("hiddenLocked");
                if (hiddenLocked != null)
                    if (hiddenLocked.Value == "1")
                        islocked = true;

                ImageButton ImgbtnLocked = (ImageButton)e.Item.FindControl("ImgbtnLocked");
                if (ImgbtnLocked != null)
                    ImgbtnLocked.Visible = islocked && Globals.AllowPageLock;



                if (sargs[0] == "0")
                {
                    // Place filler
                    if (table != null)
                       table.Style.Add("border", "none");
                    thumbTxt.Visible = false;
                    thumbTxt2.Visible = false;
                    colorImage1.Visible = false;
                    panelBottom.Visible = false;
                    panelBottom2.Visible = false;
                    panelBottom3.Visible = false;
                    ImageButton imgbtn = (ImageButton)e.Item.FindControl("btnApprove");
                    imgbtn.Visible = false;
                    imgbtn = (ImageButton)e.Item.FindControl("btnDisapprove");
                    imgbtn.Visible = false;

                    imgbtn = (ImageButton)e.Item.FindControl("ImageButtonRIPInfo");
                    if (imgbtn != null)
                        imgbtn.Visible = false;
                    imgbtn = (ImageButton)e.Item.FindControl("ImageButtonPreflightInfo");
                    if (imgbtn != null)
                        imgbtn.Visible = false;
                    imgbtn = (ImageButton)e.Item.FindControl("ImageButtonFTPInfo");
                    if (imgbtn != null)
                        imgbtn.Visible = false;

                    imgbtn = (ImageButton)e.Item.FindControl("ImageButtonColorInfo");
                    if (imgbtn != null)
                        imgbtn.Visible = false;

                    imgbtn = (ImageButton)e.Item.FindControl("ImageButtonInkSaveInfo");
                    if (imgbtn != null)
                        imgbtn.Visible = false;

                    imgbtn = (ImageButton)e.Item.FindControl("btnPrinter");
                    if (imgbtn != null)
                        imgbtn.Visible = false;
                    imgbtn = (ImageButton)e.Item.FindControl("btnRetransmit");
                    if (imgbtn != null)
                        imgbtn.Visible = false;

                    imgbtn = (ImageButton)e.Item.FindControl("btnReprocess");
                    if (imgbtn != null)
                        imgbtn.Visible = false;
                    

                    if (imgbtnPDF != null)
                        imgbtnPDF.Visible = false;

                    if (killBtn != null)
                        killBtn.Visible = false;

                    if (lockBtn != null)
                        lockBtn.Visible = false;

                    if (unlockBtn != null)
                        unlockBtn.Visible = false;

                }
                else
                {
                    ImageButton imgbtn = (ImageButton)e.Item.FindControl("btnApprove");
                    imgbtn.Visible = (bool)Session["MayApprove"] && ((bool)Application["HideApproveButton"] == false);
                    imgbtn.ToolTip = Global.rm.GetString("txtTooltipApprovePage");

                    imgbtn = (ImageButton)e.Item.FindControl("btnDisapprove");
                    imgbtn.Visible = (bool)Session["MayApprove"] && ((bool)Application["HideApproveButton"] == false);
                    imgbtn.ToolTip = Global.rm.GetString("txtTooltipDisapprovePage");

                    // Adjust color according to approved state

                    string approveState = sargs[2];
                    string colorString = sargs[1];
                    string[] sargscolors = colorString.Split('_');
                    string colorStatus = "";
                    if (sargs.Length > 6)
                        colorStatus = sargs[6];

                    string[] sargscolorstatus = colorStatus.Split('_');




                    if (approveState == "1" || ((bool)Application["ShowAutoapprovedGreen"] == true && approveState == "-1"))
                    {
                        thumbTxt.BackColor = Color.LawnGreen;
                        thumbTxt2.BackColor = Color.LawnGreen;
                        panelBottom.BackColor = Color.LawnGreen;
                        if (Globals.FlatLook == false)
                        {
                            thumbTxt.BackImageUrl = "../Images/greengradient2.gif";
                            thumbTxt2.BackImageUrl = "../Images/greengradient2.gif";
                            panelBottom.BackImageUrl = "../Images/greengradient2.gif";
                        }
                    }
                    else if (approveState == "2")
                    {
                        thumbTxt.BackColor = Color.Red;
                        thumbTxt2.BackColor = Color.Red;
                        panelBottom.BackColor = Color.Red;
                        if (Globals.FlatLook == false)
                        {
                            thumbTxt.BackImageUrl = "../Images/redgradient2.gif";
                            thumbTxt2.BackImageUrl = "../Images/redgradient2.gif";
                            panelBottom.BackImageUrl = "../Images/redgradient2.gif";
                        }                       
                    }
                    else if (approveState == "0")
                    {
                        thumbTxt.BackColor = Color.LightGray;
                        thumbTxt2.BackColor = Color.LightGray;
                        panelBottom.BackColor = Color.LightGray;

                        if (Globals.FlatLook == false)
                        {
                            thumbTxt.BackImageUrl = "../Images/graygradient2.gif";
                            thumbTxt2.BackImageUrl = "../Images/graygradient2.gif";
                            panelBottom.BackImageUrl = "../Images/graygradient2.gif";
                        }                       
                    }
                    else
                    {
                        thumbTxt.BackColor = Color.LightSkyBlue;
                        thumbTxt2.BackColor = Color.LightSkyBlue;
                        panelBottom.BackColor = Color.LightSkyBlue;
                        if (Globals.FlatLook == false)
                        {
                            thumbTxt.BackImageUrl = "../Images/bluegradient2.gif";
                            thumbTxt2.BackImageUrl = "../Images/bluegradient2.gif";
                            panelBottom.BackImageUrl = "../Images/bluegradient2.gif";
                        }
                    }

                    if (hiddenUniquePage.Value == "0")
                    {
                        thumbTxt2.BackColor = Color.MediumPurple;
                        if (Globals.FlatLook == false)
                            thumbTxt2.BackImageUrl = "../Images/purplegradient2.gif";
                    }

                    if (islocked)
                    {
                        thumbTxt2.BackColor = Color.LightSalmon;
                        if (Globals.FlatLook == false)
                            thumbTxt2.BackImageUrl = "../Images/redgradient2.gif";
                    }

                    if ((bool)Application["ThumbnailShowStatusColors"])
                    {
                        int nStatus = 0;
                        if (sargs.Length >= 5)
                            nStatus = Globals.TryParse(sargs[4], 0);
                        if (nStatus == 0)
                        {
                            panelBottom.BackColor = Color.LightGray;
                            if (Globals.FlatLook == false)
                                panelBottom.BackImageUrl = "../Images/graygradient2.gif";
                        }
                        else if (nStatus == 16 || nStatus == 26 || nStatus == 36)
                        {
                            panelBottom.BackColor = Color.Red;
                            if (Globals.FlatLook == false)
                                panelBottom.BackImageUrl = "../Images/redgradient2.gif";

                        }
                        else if (nStatus == 29)
                        {
                            panelBottom.BackColor = Color.Orange;
                            if (Globals.FlatLook == false)
                                panelBottom.BackImageUrl = "../Images/orangegradient2.gif";
                        }
                        else if ((nStatus > 0 && nStatus < 29) && Globals.LocationIsPress == false ||
                                 (nStatus > 0 && nStatus < 50) && Globals.LocationIsPress == true)
                        {
                            panelBottom.BackColor = Color.Yellow;
                            if (Globals.FlatLook == false)
                                panelBottom.BackImageUrl = "../Images/yellowgradient2.gif";

                            if (Globals.LocationIsPress && nStatus == 49)
                            {
                                panelBottom.BackColor = Color.Orange;
                                if (Globals.FlatLook == false)
                                    panelBottom.BackImageUrl = "../Images/orangegradient2.gif";
                            }
                        }
                        else if ((nStatus == 30) && Globals.LocationIsPress == false) {
                            panelBottom.BackColor = Color.Yellow;
                            if (Globals.FlatLook == false)
                                panelBottom.BackImageUrl = "../Images/yellowgradient2.gif";
                        
                        }
                        else // Done...
                        {
                            panelBottom.BackColor = Color.LawnGreen;
                            if (Globals.FlatLook == false)
                                panelBottom.BackImageUrl = "../Images/greengradient2.gif";
                        }
                    }

                    if (Globals.FlatLook)
                    {
                        panelBottom2.BackColor = thumbTxt.BackColor;
                        panelBottom3.BackColor = thumbTxt.BackColor;
                    }

                    if (showAlert)
                    {
                        //thumbTxt.BackColor = Color.BurlyWood;
                        thumbTxt2.BackColor = Color.BurlyWood;
                        panelBottom.BackColor = Color.BurlyWood;

                        panelBottom2.BackColor = Color.BurlyWood;
                        panelBottom3.BackColor = Color.BurlyWood;

                    }

                    bool colorok = false;

                    colorImage1.Visible = Globals.AllowColorChange;
                    if (Globals.AllowColorChange)
                    {
                        colorImage1.ToolTip = Global.rm.GetString("txtTooltipChangeColor");

                        if (sargscolors.Length == 1)
                        {
                            if (colorString == "PDF")
                            {
                                colorImage1.ImageUrl = Globals.FlatLook ? "../Images/colorPDFbig_Flat.gif" : "../Images/colorPDFbig.gif";
                                colorImage1.ToolTip = Global.rm.GetString("txtTooltipViewPDF");
                                //	imgbtn =(ImageButton)e.Item.FindControl("btnPDF"); 
                                //	if (imgbtn != null)
                                //		imgbtn.Visible = true;

                            }
                            else if (colorString == "PDFmono")
                            {
                                colorImage1.ImageUrl = Globals.FlatLook ? "../Images/monoPDFbig_Flat.gif": "../Images/monoPDFbig.gif";
                                colorImage1.ToolTip = Global.rm.GetString("txtTooltipViewPDF");
                                //	imgbtn =(ImageButton)e.Item.FindControl("btnPDF"); 
                                //	if (imgbtn != null)
                                //		imgbtn.Visible = true;
                            }
                            else if (Globals.IsBlackColor(colorString))
                            {
                                if (Globals.FlatLook == false)
                                    colorImage1.ImageUrl = colorsLocked ? "../Images/colorKbigLocked.gif" : "../Images/colorKbig.gif";
                                else
                                    colorImage1.ImageUrl = colorsLocked ? "../Images/colorKbigLocked_Flat.gif" : "../Images/colorKbig_Flat.gif";
                            }
                            else
                            {
                                if (Globals.FlatLook == false)
                                    colorImage1.ImageUrl = colorsLocked ? "../Images/colorKbigLocked.gif" : "../Images/colorKbig.gif";			// Just default to black....?
                                else
                                    colorImage1.ImageUrl = colorsLocked ? "../Images/colorKbigLocked_Flat.gif" : "../Images/colorKbig_Flat.gif";			// Just default to black....?
                            }
                            colorok = true;
                        }

                        if (sargscolors.Length == 4)
                        { 
                            if (Globals.IsProcessColor(sargscolors[0]) && Globals.IsProcessColor(sargscolors[1]) && Globals.IsProcessColor(sargscolors[2]) && Globals.IsProcessColor(sargscolors[3]))
                            {
                                colorok = true;
                                if (sargscolorstatus.Length == 4 && Globals.FlatLook)
                                {
                                    bool hasC = false;
                                    bool hasM = false;
                                    bool hasY = false;
                                    bool hasK = false;
                                    for (int ii=0; ii<4; ii++) {

                                        if (sargscolors[ii] == "C" && Globals.TryParse(sargscolorstatus[ii], 0) > 0)
                                            hasC = true;
                                        if (sargscolors[ii] == "M" && Globals.TryParse(sargscolorstatus[ii], 0) > 0)
                                            hasM = true;
                                        if (sargscolors[ii] == "Y" && Globals.TryParse(sargscolorstatus[ii], 0) > 0)
                                            hasY = true;
                                        if (sargscolors[ii] == "K" && Globals.TryParse(sargscolorstatus[ii], 0) > 0)
                                            hasK = true;
                                    }
                                    string finalimagename = "../Images/colorcmyk_";
                                    if (hasC)
                                        finalimagename += "O";
                                    else
                                        finalimagename += "X";
                                    if (hasM)
                                        finalimagename += "O";
                                    else
                                        finalimagename += "X";
                                    if (hasY)
                                        finalimagename += "O";
                                    else
                                        finalimagename += "X";
                                    if (hasK)
                                        finalimagename += "O";
                                    else
                                        finalimagename += "X";

                                    colorImage1.ImageUrl = finalimagename + ".gif";


                                }
                                else
                                {
                                    
                                    if (Globals.FlatLook == false)
                                        colorImage1.ImageUrl = colorsLocked ? "../Images/colorcmykLocked.gif" : "../Images/colorcmyk.gif";
                                    else
                                        colorImage1.ImageUrl = colorsLocked ? "../Images/colorcmykLocked_Flat.gif" : "../Images/colorcmyk_Flat.gif";
                                }
                            } 
                        }

                        if (colorok == false)
                        {
                            Bitmap bitmap = new Bitmap(8 * sargscolors.Length, 16, PixelFormat.Format32bppArgb);

                            Graphics g = Graphics.FromImage(bitmap);
                            SolidBrush whithBrush = new SolidBrush(Color.White);
                            g.FillRectangle(whithBrush, 0, 0, 8 * sargscolors.Length, 16);
                            Rectangle rectGrad = new Rectangle(0, 0, 8, 16);
                            for (int nColorNumber = 0; nColorNumber < sargscolors.Length; nColorNumber++)
                            {
                                SolidBrush brush = new SolidBrush(Globals.GetColorFromName(sargscolors[nColorNumber]));
                                //System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(rectGrad, Color.WhiteSmoke, Globals.GetColorFromName(sargscolors[nColorNumber]), 90.0F);

                                g.FillRectangle(brush, nColorNumber * 8, 0, 8, 16);
                            }
                            if (colorsLocked)
                                g.DrawImage(ImageSmallLock, new Point(sargscolors.Length > 1 ? sargscolors.Length * 8 / 2 - 8 : 0));

                            try
                            {
                                bitmap.Save(Request.MapPath(Request.ApplicationPath) + "/Images/thumbs/" + sargs[0] + "_th.gif", ImageFormat.Gif);
                            }
                            catch (Exception ee)
                            {
                                lblError.Text = ee.Message;
                                lblError.ForeColor = Color.Red;
                            }
                            colorImage1.ImageUrl = "../Images/thumbs/" + sargs[0] + "_th.gif";
                        }
                    }
                }
            }
        }


        protected void RadToolBar1_ButtonClick(object sender, Telerik.Web.UI.RadToolBarEventArgs e)
		{
            lblError.Text = "";

            if (e.Item.Value == "PagesPerRow4")
                Session["PagesPerRow"] = 4;
            else if (e.Item.Value == "PagesPerRow6")
                Session["PagesPerRow"] = 6;
            else if (e.Item.Value == "PagesPerRow8")
                Session["PagesPerRow"] = 8;
            else if (e.Item.Value == "PagesPerRow10")
                Session["PagesPerRow"] = 10;
            else if (e.Item.Value == "PagesPerRow12")
                Session["PagesPerRow"] = 12;
            else if (e.Item.Value == "PagesPerRow14")
                Session["PagesPerRow"] = 14;
            else if (e.Item.Value == "PagesPerRow16")
                Session["PagesPerRow"] = 16;
            else if (e.Item.Value == "PagesPerRow18")
                Session["PagesPerRow"] = 18;
            else if (e.Item.Value == "PagesPerRow20")
                Session["PagesPerRow"] = 20;

            if ((bool)Session["mobiledevice"])
            {
                if ((int)Session["PagesPerRow"] >= 6)
                    Session["PagesPerRow"] = 6;
            }

            else if (e.Item.Value == "RefreshTime10")
                Session["RefreshTime"] = 10;
            else if (e.Item.Value == "RefreshTime20")
                Session["RefreshTime"] = 20;
            else if (e.Item.Value == "RefreshTime30")
                Session["RefreshTime"] = 30;
            else if (e.Item.Value == "RefreshTime40")
                Session["RefreshTime"] = 40;
            else if (e.Item.Value == "RefreshTime50")
                Session["RefreshTime"] = 50;
            else if (e.Item.Value == "RefreshTime60")
                Session["RefreshTime"] = 60;
            else if (e.Item.Value == "RefreshTime70")
                Session["RefreshTime"] = 70;
            else if (e.Item.Value == "RefreshTime80")
                Session["RefreshTime"] = 80;
            else if (e.Item.Value == "RefreshTime90")
                Session["RefreshTime"] = 90;
            else if (e.Item.Value == "RefreshTime100")
                Session["RefreshTime"] = 100;
            else if (e.Item.Value == "RefreshTime110")
                Session["RefreshTime"] = 110;
            else if (e.Item.Value == "RefreshTime120")
                Session["RefreshTime"] = 120;



            else if (e.Item.Value == "CustomAction")
            {
                nRefreshTime = 16000;

                Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowCustomAction");
                mywindow.Title = Global.rm.GetString("txtCustomAction");
                mywindow.VisibleOnPageLoad = true;
                ReBind(true);
                return;
            }

            else if (e.Item.Value == "ReadyMessage")
            {
                nRefreshTime = 16000;
                Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowReadyAction");
                mywindow.Title = Global.rm.GetString("txtPagesReadyAction");
                mywindow.VisibleOnPageLoad = true;
                ReBind(true);
                return;

            }

            else if (e.Item.Value == "ApproveAll")
            {
                nRefreshTime = 16000;
                Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowApproveAll");
                mywindow.Title = Global.rm.GetString("txtApproveAll");
                mywindow.VisibleOnPageLoad = true;
                ReBind(true);
                return;
            }

            else if (e.Item.Value == "Messages")
            {
                nRefreshTime = 16000;
                Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowShowMessage");
                mywindow.Title = Global.rm.GetString("txtMessages");
                mywindow.VisibleOnPageLoad = true;
                ReBind(true);
                return;
            }

            else if (e.Item.Value == "UploadFiles")
            {
/*                if ((string)Session["SelectedPublication"] == "*" || (string)Session["SelectedPublication"] == "")
                {
                    lblError.Text = "Choose a product before upload";
                    ReBind(true);
                    return;
                }*/
                CCDBaccess db = new CCDBaccess();
                int customerID = 0;
                string errmsg = "";
                string emailRecipient = "";
                string emailCC = "";
                string emailSubject = "";
                string emailBody = "";
                string uploadFolder = "";
                string customerName = "";

                // Stage 1 of 2 - see if upload folder is defined for this publication

                /*
                if (db.GetPublicationEmail(Globals.GetIDFromName("PublicationNameCache", (string)Session["SelectedPublication"]), out customerID, out  uploadFolder, out  emailRecipient, out  emailCC, out  emailSubject, out  emailBody, out  errmsg) == false)
                {
                    lblError.Text = errmsg;
                    ReBind(true);
                    return;
                }*/

                if (uploadFolder == "")
                {
                    if (customerID == 0)
                        customerID = (int)Session["CustomerID"];

                    if (customerID > 0)
                    {
                        if (db.GetCustomerEmail(customerID, out customerName, out  uploadFolder, out  emailRecipient, out  emailCC, out  emailSubject, out  emailBody, out  errmsg) == false)
                        {
                            lblError.Text = errmsg;
                            ReBind(true);
                            return;
                        }
                    }
                }

                if (uploadFolder == "")
                    uploadFolder = Global.sVirtualUploadFolder;

                if (uploadFolder == "")
                {

                    lblError.Text = "No upload folder defined for customer";
                    ReBind(true);
                    return;
                }
                
                if ((string)Application["UploaderUrl"] != "")
                {
                    ReBind(true);
                    doPopupUploadWindow(uploadFolder);
                    return;
                }
               

                string virtualFolder = uploadFolder;
                string physicalFolder = uploadFolder == Global.sVirtualUploadFolder ? Global.sRealUploadFolder :  HttpContext.Current.Server.MapPath(virtualFolder);

                if (physicalFolder.EndsWith("/"))
                    physicalFolder = physicalFolder.Substring(0, physicalFolder.Length - 1);

                if (System.IO.Directory.Exists(physicalFolder) == false)
                {
                    lblError.Text = "Unreachable upload folder - unable to upload files";
                    Global.logging.WriteLog("Unreachable upload folder " + physicalFolder + " ("+virtualFolder+")");
                    ReBind(true);
                    return;
                }

                doPopupUploadWindow(uploadFolder);
            }

            else if (e.Item.Value == "Download")
            {
                GeneratePDF();
            }

            nRefreshTime = (int)Session["RefreshTime"];
            ReBind(true);
        }

		private void ReBind(bool includeRefresh)
		{

			ImageSmallLock = System.Drawing.Image.FromFile(Request.MapPath(Request.ApplicationPath) + "/Images/SmallRedLock.gif");
			ImageBigLock = System.Drawing.Image.FromFile(Request.MapPath(Request.ApplicationPath) + "/Images/BigLock.gif");

			
            Telerik.Web.UI.RadToolBarButton btn = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("HideApproved");
            if (btn != null)
                Session["HideApproved"] = btn.Checked;
            btn = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("HideCommon");
            if (btn != null)
                Session["HideCommonPages"] = btn.Checked;

            SetScreenSize();

            DoDataBind(false);
			
            if (includeRefresh)
                SetRefreshheader();

		}

        static readonly int PDFBOOK_INFOFONTSIZE = 14;
        static readonly int PDFBOOK_INFOXPOS = 20;
        static readonly int PDFBOOK_INFOYPOS = 5;
        static readonly string PDFBOOK_INFOFONTNAME = "Helvetica-Bold";
        static readonly string PDFBOOK_INFOFONTENCODING = "winansi";
        static readonly string PDFBOOK_INFOFONTOPTIONS = "fontwarning=false";

        private string GenerateTitle(string namemask, string dateformat, string publication,  DateTime pubdate, string edition, string section, string pagename)
        {
            string title = namemask;

            title = title.Replace("%P", publication);
            title = title.Replace("%E", edition);
            title = title.Replace("%S", section);
            title = title.Replace("%N", pagename);

            dateformat = dateformat.Replace("DD", string.Format("{0:00}", pubdate.Day));
            dateformat = dateformat.Replace("MM", string.Format("{0:00}", pubdate.Month));
            dateformat = dateformat.Replace("YYYY", string.Format("{0}", pubdate.Year));
            dateformat = dateformat.Replace("YY", string.Format("{0}", pubdate.Year-2000));
            title = title.Replace("%D", dateformat);
            title = title.Replace("%T", Globals.DateTime2StringShort(DateTime.Now));
            
            return title;
        }

        private bool GeneratePDF()
		{
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            String sPublication = (String)Session["SelectedPublication"];
            DateTime tPubDate = (DateTime)Session["SelectedPubDate"];

            string pdfFileName = "";
            bool hideEdition = Globals.GetCacheRowCount("EditionNameCache") < 2;
            bool hideSection = Globals.GetCacheRowCount("SectionNameCache") < 2;

            if (sPublication != "")
                pdfFileName += sPublication + "_";

            if (tPubDate.Year > 2000)
                pdfFileName += "_" + tPubDate.Month + tPubDate.Day + tPubDate.Year;

            if ((string)Session["SelectedEdition"] != "" && hideEdition == false)
                pdfFileName += "_" + (string)Session["SelectedEdition"];

            if ((string)Session["SelectedSection"] != "" && hideSection == false)
                pdfFileName += "_" + (string)Session["SelectedSection"];

            string previewGUID = Globals.MakePreviewGUID(Globals.GetIDFromName("PublicationNameCache", sPublication), tPubDate);

            bool hideApproved = (bool)Session["HideApproved"];
            bool hideCommon = (bool)Session["HideCommonPages"];
            List<int> masterList1 = new List<int>();
            List<string> pagenameList1 = new List<string>();
            db.GetMasterSetPageCollection(ref masterList1, ref pagenameList1,  hideApproved, hideCommon, false, out errmsg);
            if (errmsg != "")
            {
                lblError.Text = errmsg;
                return false;
            }

            if (masterList1.Count == 0)
            {
                lblError.Text = "No pages ready";
                return false;
            }

            List<string> fileNameList = new List<string>();
            List<string> pagenameList = new List<string>();

            int idx = 0;
            foreach (int masterCopySeparationSet in masterList1)
            {
                string fname = Global.sRealImageFolder + @"\" + previewGUID + @"====" + masterCopySeparationSet.ToString() + ".jpg";

                if (System.IO.File.Exists(fname) == false)
                    fname = Global.sRealImageFolder + @"\" + masterCopySeparationSet.ToString() + ".jpg";

                if (System.IO.File.Exists(fname))
                {
                    fileNameList.Add(fname);
                    pagenameList.Add(pagenameList1[idx]);
                }
                idx++;
            }

            if (fileNameList.Count == 0)
            {
                lblError.Text = "No previews ready";
                return false;
            }

            PDFlib p = new PDFlib();
            try
            {
                //p.set_parameter("warning", "false");

                //p.set_parameter("license", "W900202-010068-132518-7XKA62-BCJA82");
                p.set_option("errorpolicy=return");
                p.set_option("license=W900202-010068-132518-7XKA62-BCJA82");


                int font = p.load_font(PDFBOOK_INFOFONTNAME, PDFBOOK_INFOFONTENCODING, PDFBOOK_INFOFONTOPTIONS);


                if (p.begin_document("", "") == -1)
                {
                    lblError.Text = "PDF error " + p.get_errmsg();
                    return false;
                }

                //p.set_parameter("SearchPath", searchpath);

                p.set_info("Creator", (string)Application["PDFBookCreator"]);
                p.set_info("Author", (string)Application["PDFBookAuthor"]);
                p.set_info("Title", pdfFileName);

                // This line is required to avoid problems on Japanese systems 
                //				p.set_parameter("hypertextencoding", "host");

                for (int i = 0; i < fileNameList.Count; i++)
                {
                    int image = p.load_image("jpeg", fileNameList[i], "");
                    if (image == -1)
                    {
                        lblError.Text = "PDF error (load JPG)" + p.get_errmsg();
                        return false;
                    }

                    /* dummy page size, will be adjusted by PDF_fit_image() */
                    p.begin_page_ext(10, 10, "");
                    p.fit_image(image, 0.0, 0.0, "adjustpage");
                    int imagewidth = (int)p.info_image(image, "width", "adjustpage");
                    int imageheight = (int)p.info_image(image, "height", "adjustpage");
                    p.close_image(image);

                    if ((int)Application["PDFBookShowHeader"] > 0 && pdfFileName != "" && font >= 0)
                    {
                        if ((int)Application["PDFBookShowHeader"] == 1 || ((int)Application["PDFBookShowHeader"] == 2 && i == 0))
                        {
                            int l_PDFBOOK_INFOXPOS = (int)Application["PDFBookHeaderXpos"];
                            if (l_PDFBOOK_INFOXPOS == 0)
                                l_PDFBOOK_INFOXPOS = PDFBOOK_INFOXPOS;
                            int l_PDFBOOK_INFOYPOS = (int)Application["PDFBookHeaderYpos"];
                            if (l_PDFBOOK_INFOYPOS == 0)
                                l_PDFBOOK_INFOYPOS = PDFBOOK_INFOYPOS;
                            int l_PDFBOOK_INFOFONTSIZE = (int)Application["PDFBookHeaderFontSize"];
                            if (l_PDFBOOK_INFOFONTSIZE == 0)
                                l_PDFBOOK_INFOFONTSIZE = PDFBOOK_INFOFONTSIZE;

                                string slugLine = GenerateTitle((string)Application["PDFbookHeaderDefinition"],
                                                            (string)Application["PDFbookHeaderPubDateDefinition"], 
                                                            sPublication, 
                                                            tPubDate,
                                                            (string)Session["SelectedEdition"],
                                                            (string)Session["SelectedSection"], 
                                                            pagenameList[i]);
                            p.setfont(font, l_PDFBOOK_INFOFONTSIZE);
                            p.setcolor("fill", "rgb", 1.0, 0.0, 0.0, 0.0);
                            p.set_text_pos(l_PDFBOOK_INFOXPOS, imageheight - l_PDFBOOK_INFOYPOS - l_PDFBOOK_INFOFONTSIZE);
                            //p.show(pdfFileName + "-" + pagenameList[i] + "         " + Global.rm.GetString("txtGenerated")+ ": " + Globals.DateTime2StringShort(DateTime.Now));
                            p.show(slugLine);

                            if ((string)Application["PDFbookHeaderLogo"] != "" && (string)Application["PDFbookHeaderLogoPosition"] != "")
                            {
                                string ss = (string)Application["PDFbookHeaderLogoPosition"];
                                string[] positiondata = ss.Split(',');
                                if (positiondata.Length >= 2)
                                {
                                    if (System.IO.File.Exists((string)Application["PDFbookHeaderLogo"]))
                                    {
                                        int imagelogo = p.load_image("auto", (string)Application["PDFbookHeaderLogo"], "");
                                        if (imagelogo != -1)
                                        {
                                            string optionText = "";
                                            if (positiondata.Length >= 4)
                                            {
                                                double f1 = 72.0 * Int32.Parse(positiondata[2]) / 25.4;
                                                double f2 = 72.0 * Int32.Parse(positiondata[3]) / 25.4;

                                                optionText = "boxsize { " + f1.ToString() + " " + f2.ToString() + "} fitmethod meet";
                                            }
                                            p.fit_image(image, 72.0 * Int32.Parse(positiondata[0])/25.4, imageheight - 72.0 * Int32.Parse(positiondata[1]) / 25.4, optionText);
                                            p.close_image(imagelogo);
                                        }
                                    }
                                }

                            }

                          

                        }
                    }
                    p.end_page_ext("");
                }
                p.end_document("");

                Byte[] buf = p.get_buffer();


                pdfFileName += ".pdf";

                Encoding ascii = Encoding.ASCII;
                Encoding unicode = Encoding.Unicode;

                // Convert the string into a byte[].
                byte[] unicodeBytes = unicode.GetBytes(pdfFileName);

                // Perform the conversion from one encoding to the other.
                byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);

                // Convert the new byte[] into a char[] and then into a string.
                // This is a slightly different approach to converting to illustrate
                // the use of GetCharCount/GetChars.
                char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
                ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
                string asciiString = new string(asciiChars);

                // Send to browser..
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + asciiString);
                Response.AppendHeader("Content-Length", buf.Length.ToString());
                Response.BinaryWrite(buf);
                Response.End();

            }
            catch (PDFlibException epdf)
            {
                // caught exception thrown by PDFlib
                lblError.Text = "PDFlib exception occurred in memo generation " + epdf.get_errmsg();
                return false;
            }

            return true;
        }

        private bool DownloadPDFPage(int masterCopySeparationSet)
        {
            string fname = Global.sRealHiresFolder + @"\" + masterCopySeparationSet.ToString() + ".pdf";

            if (System.IO.File.Exists(fname) == false)
            {
                lblError.Text = fname + " not found";
                return false;
            }

            string errmsg;
            CCDBaccess db = new CCDBaccess();
            string filename = db.GetFileName(masterCopySeparationSet, out errmsg);
            if (filename == "")
                return false;

            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;
            // Convert the string into a byte[].
            byte[] unicodeBytes = unicode.GetBytes(filename);

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
                return true;
            }
            catch
            {
                return false;
            }
        }


        private string GenerateHeatMap(int mastercopyseparationset)
        {

            string finalImage = Request.MapPath(Request.ApplicationPath) + "/Images/Flats/Heatmap" + mastercopyseparationset.ToString() + ".jpg";

            string fname = Global.sRealImageFolder + @"\" + mastercopyseparationset.ToString() + "_dns.jpg";
            if (System.IO.File.Exists(fname))
            {
                System.IO.File.Copy(fname, finalImage, true);
                return finalImage;
            }

            Bitmap previewImageC = null;
            Bitmap previewImageM = null;
            Bitmap previewImageY = null;
            Bitmap previewImageK = null;
            BitmapData bmDataC = null;
            BitmapData bmDataM = null;
            BitmapData bmDataY = null;
            BitmapData bmDataK = null;
            System.IntPtr ScanC = System.IntPtr.Zero;
            System.IntPtr ScanM = System.IntPtr.Zero;
            System.IntPtr ScanY = System.IntPtr.Zero;
            System.IntPtr ScanK = System.IntPtr.Zero;
            int nThisImageWidth = 0;
            int nThisImageHeight = 0;
            int strideIncoming = 0;

            fname = Global.sRealImageFolder + @"\" + mastercopyseparationset.ToString() + "_C.jpg";

            if (System.IO.File.Exists(fname))
            {
                previewImageC = (Bitmap)Bitmap.FromFile(fname);
                if (previewImageC != null)
                {
                    bmDataC = previewImageC.LockBits(new Rectangle(0, 0, previewImageC.Width, previewImageC.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                    nThisImageWidth = previewImageC.Size.Width;
                    nThisImageHeight = previewImageC.Size.Height;
                    strideIncoming = bmDataC.Stride;
                    ScanC = bmDataC.Scan0;
                }
            }
            fname = Global.sRealImageFolder + @"\" + mastercopyseparationset.ToString() + "_M.jpg";
            if (System.IO.File.Exists(fname))
            {
                previewImageM = (Bitmap)Bitmap.FromFile(fname);
                if (previewImageM != null)
                {
                    bmDataM = previewImageM.LockBits(new Rectangle(0, 0, previewImageM.Width, previewImageM.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                    nThisImageWidth = previewImageM.Size.Width;
                    nThisImageHeight = previewImageM.Size.Height;
                    strideIncoming = bmDataM.Stride;
                    ScanM = bmDataM.Scan0;
                }
            }
            fname = Global.sRealImageFolder + @"\" + mastercopyseparationset.ToString() + "_Y.jpg";
            if (System.IO.File.Exists(fname))
            {
                previewImageY = (Bitmap)Bitmap.FromFile(fname);
                if (previewImageY != null)
                {
                    bmDataY = previewImageY.LockBits(new Rectangle(0, 0, previewImageY.Width, previewImageY.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                    nThisImageWidth = previewImageY.Size.Width;
                    nThisImageHeight = previewImageY.Size.Height;
                    strideIncoming = bmDataY.Stride;
                    ScanY = bmDataY.Scan0;

                }
            }
            fname = Global.sRealImageFolder + @"\" + mastercopyseparationset.ToString() + "_K.jpg";
            if (System.IO.File.Exists(fname))
            {
                previewImageK = (Bitmap)Bitmap.FromFile(fname);
                if (previewImageK != null)
                {
                    bmDataK = previewImageK.LockBits(new Rectangle(0, 0, previewImageK.Width, previewImageK.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                    nThisImageWidth = previewImageK.Size.Width;
                    nThisImageHeight = previewImageK.Size.Height;
                    strideIncoming = bmDataK.Stride;
                    ScanK = bmDataK.Scan0;

                }
            }

            if (nThisImageWidth == 0 || nThisImageHeight == 0)
                return "";


            Bitmap heatbitmap = new Bitmap(nThisImageWidth, nThisImageHeight, PixelFormat.Format24bppRgb);
            BitmapData heatbitmapData = heatbitmap.LockBits(new Rectangle(0, 0, previewImageY.Width, previewImageY.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            System.IntPtr ScanHeatmap = heatbitmapData.Scan0;

            unsafe
            {
                byte* pC = (byte*)(void*)ScanC;
                byte* pM = (byte*)(void*)ScanM;
                byte* pY = (byte*)(void*)ScanY;
                byte* pK = (byte*)(void*)ScanK;
                byte* pScanHeatmap = (byte*)(void*)ScanHeatmap;

                for (int y = 0; y < heatbitmap.Height; y++)
                {
                    for (int x = 0; x < heatbitmap.Width; x++)
                    {
                        int sum = 0;
                        if (pC != null)
                        {
                            sum += (int)255 - (int)(byte)pC[2];
                            pC += 3;
                        }

                        if (pM != null)
                        {
                            sum += (int)255 - (int)(byte)pM[1];
                            pM += 3;
                        }
                        if (pY != null)
                        {
                            sum += (int)255 - (int)(byte)pY[0];
                            pY += 3;
                        }
                        if (pK != null)
                        {
                            sum += (int)255 - (int)(byte)pK[0];
                            pK += 3;
                        }

                        sum *= 400;
                        sum /= 1020;

                        if (sum == 0)
                        {
                            pScanHeatmap[0] = (byte)255;
                            pScanHeatmap++;
                            pScanHeatmap[0] = (byte)255;
                            pScanHeatmap++;
                            pScanHeatmap[0] = (byte)255;
                            pScanHeatmap++;
                        }
                        else if (sum > 0 && sum < 100)
                        {
                            pScanHeatmap[0] = (byte)128;
                            pScanHeatmap++;
                            pScanHeatmap[0] = (byte)255;
                            pScanHeatmap++;
                            pScanHeatmap[0] = (byte)255;
                            pScanHeatmap++;
                        }
                        else if (sum >= 100 && sum < 200)
                        {
                            pScanHeatmap[0] = (byte)0;
                            pScanHeatmap++;
                            pScanHeatmap[0] = (byte)255;
                            pScanHeatmap++;
                            pScanHeatmap[0] = (byte)255;
                            pScanHeatmap++;
                        }
                        else if (sum >= 200 && sum < 225)
                        {
                            pScanHeatmap[0] = (byte)64;
                            pScanHeatmap++;
                            pScanHeatmap[0] = (byte)170;
                            pScanHeatmap++;
                            pScanHeatmap[0] = (byte)255;
                            pScanHeatmap++;
                        }
                        else if (sum >= 225 && sum < 250)
                        {
                            pScanHeatmap[0] = (byte)0;
                            pScanHeatmap++;
                            pScanHeatmap[0] = (byte)128;
                            pScanHeatmap++;
                            pScanHeatmap[0] = (byte)255;
                            pScanHeatmap++;
                        }
                        else if (sum >= 250 && sum < 275)
                        {
                            pScanHeatmap[0] = (byte)0;
                            pScanHeatmap++;
                            pScanHeatmap[0] = (byte)64;
                            pScanHeatmap++;
                            pScanHeatmap[0] = (byte)255;
                            pScanHeatmap++;
                        }
                        else //if  (sum >= 275)
                        {
                            pScanHeatmap[0] = (byte)0;
                            pScanHeatmap++;
                            pScanHeatmap[0] = (byte)0;
                            pScanHeatmap++;
                            pScanHeatmap[0] = (byte)255;
                            pScanHeatmap++;
                        }


                    }
                }
            }


            try
            {
                heatbitmap.Save(finalImage, ImageFormat.Jpeg);
            }
            catch
            {
                return "";
            }

            if (heatbitmap != null)
                heatbitmap.UnlockBits(heatbitmapData);

            if (previewImageC != null)
                previewImageC.UnlockBits(bmDataC);
            if (previewImageM != null)
                previewImageM.UnlockBits(bmDataM);
            if (previewImageY != null)
                previewImageY.UnlockBits(bmDataY);
            if (previewImageK != null)
                previewImageK.UnlockBits(bmDataK);

            heatbitmap.Dispose();

            return finalImage;


        }


		private void doPopupImageNotReady()
		{
			string popupScript =
				"<script language='javascript'>" +
				" alert('Flash images are not ready yet');" +
				"</script>";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "PopupScript", popupScript);
		}

		private void doPopupColorWindow()
		{
			string popupScript =
				"<script language='javascript'>" +
				"var xpos = 300;" + 
				"var ypos = 300;" +
				"if(window.screen) { xpos = (screen.width-280)/2; ypos = (screen.height-120)/2; }" + 
				"var s = 'status=no,top='+ypos+',left='+xpos+',width=280,height=120';" +
				"var PopupWindow = window.open('ChangeColor.aspx','Colors',s);" + 	
				"if (parseInt(navigator.appVersion) >= 4) PopupWindow.focus();" +
				"</script>";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "PopupScript", popupScript);
            
		}

		private void doPopupHardproofWindow()
		{
			string popupScript =
				"<script language='javascript'>" +
				"var xpos = 300;" + 
				"var ypos = 300;" +
				"if(window.screen) { xpos = (screen.width-310)/2; ypos = (screen.height-120)/2; }" + 
				"var s = 'status=no,top='+ypos+',left='+xpos+',width=310,height=120';" +
				"var PopupWindow = window.open('HardProof.aspx','Hardproof',s);" + 	
				"if (parseInt(navigator.appVersion) >= 4) PopupWindow.focus();" +
				"</script>";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "PopupScript", popupScript);

		}

		private void doPopupMessageColorWindow()
		{
			string popupScript =
				"<script language='javascript'>" +
				"var xpos = 300;" + 
				"var ypos = 300;" +
				"if(window.screen) { xpos = (screen.width-366)/2; ypos = (screen.height-370)/2; }" + 
				"var s = 'status=no,top='+ypos+',left='+xpos+',width=366,height=370';" +
				"var PopupWindow = window.open('Message.aspx','Message',s);" + 	
				"if (parseInt(navigator.appVersion) >= 4) PopupWindow.focus();" +
				"</script>";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "PopupScript", popupScript);
        }

        private void doPopupUploadWindow(string uploadFolder)
		{
            uploadFolder = uploadFolder.Replace('/', '!');

            //     string serverUrl = "http://" + HttpContext.Current.Request.ServerVariables["SERVER_NAME"] + "/" +
            //      (string)Application["UploaderUrl"];

            Session["RefreshTimeSaved"] = Session["RefreshTime"];
            Session["RefreshTime"] = 10000;

             Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowUploadFiles");
             mywindow.Title = Global.rm.GetString("txtTooltipUploadFiles");
            
             mywindow.NavigateUrl = "UploadFiles.aspx?folder="+uploadFolder;
             mywindow.VisibleOnPageLoad = true;

            /*            string popupScript = "";
                         if ((string)Application["UploaderUrl"] != "")
                        {
                             //if (uploadFolder != "")
                               // serverUrl += "?folder=" + uploadFolder;
                               popupScript =
                                    "<script language='javascript'>" +
                                    "var xpos = 300;" +
                                    "var ypos = 300;" +
                                    "if(window.screen) { xpos = (screen.width-610)/2; ypos = (screen.height-600)/2; }" +
                                    "var s = 'status=no,top='+ypos+',left='+xpos+',width=610,height=600';" +
                                    "var PopupWindow = window.open('" + serverUrl + "','Uploader',s);" +
                                    "if (parseInt(navigator.appVersion) >= 4) PopupWindow.focus();" +
                                    "</script>";
                        }
                        else
                            popupScript =
                                "<script language='javascript'>" +
                                "var xpos = 300;" +
                                "var ypos = 300;" +
                                "if(window.screen) { xpos = (screen.width-490)/2; ypos = (screen.height-440)/2; }" +
                                "var s = 'status=no,top='+ypos+',left='+xpos+',width=490,height=440';" +
                                "var PopupWindow = window.open('UploadFiles.aspx?folder="+uploadFolder+"','Uploader',s);" +
                                "if (parseInt(navigator.appVersion) >= 4) PopupWindow.focus();" +
                                "</script>";

                        Page.ClientScript.RegisterStartupScript(this.GetType(), "PopupScript", popupScript); */
        }

		private void doPopupDownloadWindow()
		{
			string popupScript =
				"<script language='javascript'>" +
				"var xpos = 300;" + 
				"var ypos = 300;" +
				"if(window.screen) { xpos = (screen.width-600)/2; ypos = (screen.height-400)/2; }" + 
				"var s = 'status=no,top='+ypos+',left='+xpos+',width=600,height=400';" +
				"var PopupWindow = window.open('DownloadPDF.aspx','Download',s);" + 	
				"if (parseInt(navigator.appVersion) >= 4) PopupWindow.focus();" +
				"</script>";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "PopupScript", popupScript);
		}

		private void doPopupMessageWindow()
		{
			string popupScript =
				"<script language='javascript'>" +
				"var xpos = 100;" + 
				"var ypos = 100;" +
				"if(window.screen) { xpos = (screen.width-608)/2; ypos = (screen.height-408)/2; }" + 
				"var s = 'status=no,top='+ypos+',left='+xpos+',width=608,height=408';" +
				"var PopupWindow = window.open('MessageSimple.aspx','Message',s);" + 	
				"if (parseInt(navigator.appVersion) >= 4) PopupWindow.focus();" +
				"</script>";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "PopupScript", popupScript);

		}

		private void doPopupMailWindow()
		{
			string popupScript =
				"<script language='javascript'>" +
				"var xpos = 100;" + 
				"var ypos = 100;" +
				"if(window.screen) { xpos = (screen.width-608)/2; ypos = (screen.height-408)/2; }" + 
				"var s = 'status=no,top='+ypos+',left='+xpos+',width=608,height=408';" +
				"var PopupWindow = window.open('ShowMessage.aspx','Message',s);" + 	
				"if (parseInt(navigator.appVersion) >= 4) PopupWindow.focus();" +
				"</script>";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "PopupScript", popupScript);
		}

		private void doPopupHistoryWindow()
		{
			string popupScript =
				"<script language='javascript'>" +
				"var xpos = 100;" + 
				"var ypos = 100;" +
				"if(window.screen) { xpos = (screen.width-608)/2; ypos = (screen.height-408)/2; }" + 
				"var s = 'status=no,top='+ypos+',left='+xpos+',width=608,height=408';" +
				"var PopupWindow = window.open('ShowPageHistory.aspx','History',s);" + 	
				"if (parseInt(navigator.appVersion) >= 4) PopupWindow.focus();" +
				"</script>";

		    Page.ClientScript.RegisterStartupScript(this.GetType(), "PopupScript", popupScript);
        }

		private bool doPopupPitstopReport(int mastercopyseparationset)
		{
			string realPath = "";
			string virtPath = "";
			if (Globals.HasPdfLog(mastercopyseparationset, out realPath, out virtPath) == false)
				return false;

            virtPath = HttpUtility.HtmlEncode(virtPath);
            virtPath = virtPath.Replace("#", "%23");
//            int h = (int)Session["WindowHeight"];
//            int w = (int)Session["WindowWidth"];

            Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowPitstopReport");
            //            mywindow.Width = w+105;
            //            mywindow.Height = h+30;
  

            //mywindow.AutoSize = true;
            mywindow.MaxHeight = 600;
            mywindow.MaxWidth = 700;
            mywindow.CenterIfModal = true;
           // mywindow.AutoSizeBehaviors = Telerik.Web.UI.WindowAutoSizeBehaviors.HeightProportional;
            mywindow.NavigateUrl = "PitstopReportview.aspx?Path="+virtPath;
            mywindow.VisibleOnPageLoad = true;

/*            string popupScript =
				"<script language='javascript'>" +
				"var xpos = 100;" + 
				"var ypos = 100;" +
				"if(window.screen) { xpos = (screen.width-"+w.ToString()+")/2; ypos = (screen.height-"+h.ToString()+")/2; }" + 
				"var s = 'status=yes,toolbar=yes,menubar=yes,location=yes,top='+ypos+',left='+xpos+',width="+w.ToString()+",height="+h.ToString()+"';" +
				"var PopupWindow = window.open('PitstopReportview.aspx?Path="+virtPath+"','_blank',s);" + 	
				"if (parseInt(navigator.appVersion) >= 4) PopupWindow.focus();" +
				"</script>";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "PopupScript", popupScript);*/
            return true;
		}

        private bool doPopupInkSaveReport(int mastercopyseparationset)
        {
            string realPath = "";
            string virtPath = "";
            if (Globals.HasAlwanLog(mastercopyseparationset, out realPath, out virtPath) == false)
                return false;

            virtPath = HttpUtility.HtmlEncode(virtPath);
            virtPath = virtPath.Replace("#", "%23");

            int w = Globals.TryParse(Request.Cookies["ScreenWidth"].Value, 608);
            int h = Globals.TryParse(Request.Cookies["ScreenHeight"].Value, 408);

            Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowAlwanReport");
            mywindow.AutoSize = true;
            mywindow.CenterIfModal = true;
            mywindow.AutoSizeBehaviors = Telerik.Web.UI.WindowAutoSizeBehaviors.HeightProportional;
            mywindow.NavigateUrl = "AlwanReportview.aspx?Path="+virtPath;
            mywindow.VisibleOnPageLoad = true;

/*            string popupScript =
                "<script language='javascript'>" +
                "var xpos = 100;" +
                "var ypos = 100;" +
                "if(window.screen) { xpos = (screen.width-" + w.ToString() + ")/2; ypos = (screen.height-" + h.ToString() + ")/2; }" +
                "var s = 'status=yes,toolbar=yes,menubar=yes,location=yes,top='+ypos+',left='+xpos+',width=" + w.ToString() + ",height=" + h.ToString() + "';" +
                "var PopupWindow = window.open('AlwanReportview.aspx?Path=" + virtPath + "','_blank',s);" +
                "if (parseInt(navigator.appVersion) >= 4) PopupWindow.focus();" +
                "</script>";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "PopupScript", popupScript); */
            return true;
        }


        
        private bool doPopupPitstopPDF(int mastercopyseparationset)
		{
			string realPath = "";
			string virtPath = "";

			if (Globals.HasPdfPage(mastercopyseparationset, out realPath, out virtPath) == false)
				return false;

			int w = Globals.TryParse(Request.Cookies["ScreenWidth"].Value, 608);
			int h = Globals.TryParse(Request.Cookies["ScreenHeight"].Value, 408);
			
			/*	
				Telerik.Web.UI.RadWindow mywindow = RadWindowManager1.Windows["radWindowPitstopReport"];
				mywindow.Width = w-200;
				mywindow.Height = h-200;
				mywindow.NavigateUrl = "PitstopReportview.aspx?Path="+Global.sVirtualPdfLogFolder + "/" + System.IO.Path.GetFileName(fileNameToShow);

				mywindow.VisibleOnPageLoad = true;*/

			string popupScript =
				"<script language='javascript'>" +
				"var xpos = 100;" + 
				"var ypos = 100;" +
				"if(window.screen) { xpos = (screen.width-"+w.ToString()+")/2; ypos = (screen.height-"+h.ToString()+")/2; }" + 
				"var s = 'status=yes,toolbar=yes,menubar=yes,location=yes,top='+ypos+',left='+xpos+',width="+w.ToString()+",height="+h.ToString()+"';" +
				"var PopupWindow = window.open('PitstopReportview.aspx?Path="+virtPath+"','_blank',s);" + 	
				"if (parseInt(navigator.appVersion) >= 4) PopupWindow.focus();" +
				"</script>";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "PopupScript", popupScript);
            return true;
		}

        private Telerik.Web.UI.RadWindow GetRadWindow(string name)
        {
            foreach (Telerik.Web.UI.RadWindow win in RadWindowManager1.Windows)
            {
                if (win.ID == name)
                    return win;
            }
            return RadWindowManager1.Windows[0];

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
