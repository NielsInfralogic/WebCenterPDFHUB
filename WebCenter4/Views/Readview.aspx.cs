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


namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for Thumbnailview.
	/// </summary>
	public class Readview : System.Web.UI.Page
	{
        protected global::System.Web.UI.WebControls.DataList datalistImages;
        protected global::System.Web.UI.WebControls.Label lblError;
        protected global::System.Web.UI.WebControls.DropDownList DropDownList1;
        protected global::System.Web.UI.WebControls.DropDownList DropDownList2;
        protected global::System.Web.UI.WebControls.Label lblChooseProduct;
        protected global::Telerik.Web.UI.RadWindowManager RadWindowManager1;
	    protected global::Telerik.Web.UI.RadToolBar RadToolBar1;

        protected HtmlInputHidden HiddenX;
        protected HtmlInputHidden HiddenY;
        protected HtmlInputHidden HiddenScrollPos;
        protected HtmlInputHidden HiddenReturendFromPopup;

	
		protected int nImagesPerRow;
		protected int nImageWidth;
		protected int nImageHeight;
		protected int nWindowWidth;
		protected int nRefreshTime;

		System.Drawing.Image ImageC;
		System.Drawing.Image ImageM;
		System.Drawing.Image ImageY;
		System.Drawing.Image ImageK;
		System.Drawing.Image ImageS;

		public int nMinImageHeight  = 300;

		public string tooltipClickImage = "Click page to view preview";

		public int nScollPos = 0;
        public bool doPopupColor;
        public bool updateTree;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ((string)Session["UserName"] == null)
				Response.Redirect("~/SessionTimeout.htm");

			if ((string)Session["UserName"] == "")
				Response.Redirect("/Denied.htm");

			// Test if this is a postback caused by a approval state change
			if (Request.QueryString["set"] != null) 
			{
				try 
				{
					string sepset = Request.QueryString["set"];
					string [] sepsets = sepset.Split('_');
					string pageName = Request.QueryString["pagename"];
					string [] pageNames = pageName.Split('_');
					string colors = Request.QueryString["colors"];
					string approval = Request.QueryString["approval"];

					if (sepsets[0] != "0" || sepsets[1] != "0")
						PrepareZoom(Int32.Parse( sepsets[0]), Int32.Parse( sepsets[1]), colors, approval, pageNames[0], pageNames[1]);
				}
				catch 
				{
					;
				}
			}

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

            }

			nImagesPerRow = (int)Session["PagesPerRow"];
            nRefreshTime = (int)Session["RefreshTime"];

            SetScreenSize();
			
			// Cache color-images for thumbnails
			ImageC = System.Drawing.Image.FromFile(Request.MapPath(Request.ApplicationPath) + "/Images/colorC.bmp");
			ImageM = System.Drawing.Image.FromFile(Request.MapPath(Request.ApplicationPath) + "/Images/colorM.bmp");
			ImageY = System.Drawing.Image.FromFile(Request.MapPath(Request.ApplicationPath) + "/Images/colorY.bmp");
			ImageK = System.Drawing.Image.FromFile(Request.MapPath(Request.ApplicationPath) + "/Images/colorK.bmp");
			ImageS = System.Drawing.Image.FromFile(Request.MapPath(Request.ApplicationPath) + "/Images/colorS.bmp");

            if (!Page.IsPostBack || HiddenReturendFromPopup.Value != "0")
			{
				DoDataBind(false);
			}

            SetRefreshheader();
            RegisterScrollSaveScript();

            HiddenReturendFromPopup.Value = "0";

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
                Response.AppendHeader("cache-control", "private");
                Response.AppendHeader("pragma", "no-cache");
                Response.AppendHeader("expires", "Fri, 30 Oct 1998 14:19:41 GMT");
                Response.CacheControl = "Private";
                Response.Cache.SetNoStore();
            }

        }

        protected void SetScreenSize()
        {
            int w = 0;
            int h = 0;
            if (HiddenX.Value != "" && HiddenY.Value != "")
            {
                w = Globals.TryParse(HiddenX.Value, 0);
                h = Globals.TryParse(HiddenY.Value, 0);
            }

            if (w <= 0 || h <= 0)
            {
                w = Globals.TryParseCookie(Request, "ScreenWidthReadview", 800);
                h = Globals.TryParseCookie(Request, "ScreenHeightReadview", 600);
            }

            if (w <= 0)
                w = (int)Session["WindowWidth"] > 0 ? (int)Session["WindowWidth"] : 800;
            if (h <= 0)
                h = (int)Session["WindowHeight"] > 0 ? (int)Session["WindowHeight"] : 600;

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
                    string s = Request.Cookies["ScrollReadviewY"].Value;
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
                if ((int)Session["PagesPerRow"] >= 6)
                    Session["PagesPerRow"] = 6;
            }


            nImagesPerRow = (int)Session["PagesPerRow"];

            nImageWidth = (w - 2 - 8 * nImagesPerRow) / nImagesPerRow;

            //nImageWidth = (w - 60) / nImagesPerRow - (nImagesPerRow - 1);

            // Blank dummy image is 160x118 - set a default height used if no pages are avail. - otherwise height is set by first detected image.
            nImageHeight = nImageWidth * 118;
            nImageHeight /= 80;
        }

		protected void SetLanguage()
		{
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            //          SetRadToolbarTooltip("Refresh", Global.rm.GetString("txtRefresh"));
            SetRadToolbarLabel("Refresh", Global.rm.GetString("txtRefresh"));

            SetRadToolbarLabel("HideApproved", Global.rm.GetString("txtHideApprovedPages"));
            SetRadToolbarTooltip("HideApproved", Global.rm.GetString("txtTooltipHideApprovedPages"));
            SetRadToolbarLabel("HideCommon", Global.rm.GetString("txtHideDuplicates"));
            SetRadToolbarTooltip("HideCommon", Global.rm.GetString("txtTooltipHideDuplicates"));

            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue("Download");
            if (item != null)
            {
                item.Text = Global.rm.GetString("txtDownloadAll");
                item.ToolTip = Global.rm.GetString("txtTooltipDownloadAll");
                item.Enabled = (bool)Application["HideDownload"] == false;
                item.Visible = (bool)Application["HideDownload"] == false;
            }
			lblChooseProduct.Text = Global.rm.GetString("txtChooseProduct");

			tooltipClickImage = Global.rm.GetString("txtTooltipClickImages");


            Telerik.Web.UI.RadToolBarSplitButton itemsb = (Telerik.Web.UI.RadToolBarSplitButton)RadToolBar1.FindItemByValue("PagesPerRowSelector");
            if (itemsb == null)
                return;
            itemsb.ToolTip = Global.rm.GetString("txtTooltipImagesPerRow");

            for (int i = 4; i <= 20; i += 2)
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

            for (int i = 10; i <= 120; i += 10)
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

		public void OnSelChangeImagesPerRow(object sender, System.EventArgs e) 
		{
			System.Web.UI.WebControls.DropDownList dropdown = (System.Web.UI.WebControls.DropDownList)sender;
			Session["PagesPerRow"] = Convert.ToInt32(dropdown.SelectedItem.Text);
			ReBind();
		}

		public void OnSelChangeRefreshTime(object sender, System.EventArgs e) 
		{
			System.Web.UI.WebControls.DropDownList dropdown = (System.Web.UI.WebControls.DropDownList)sender;
			Session["RefreshTime"] = Convert.ToInt32(dropdown.SelectedValue);
			ReBind();
		}

        public void DoDataBind(bool firstTime)
        {
            if ((string)Session["SelectedPublication"] == "")
            {
                lblChooseProduct.Visible = true;
                return;
            }
            lblChooseProduct.Visible = false;
            CCDBaccess db = new CCDBaccess();
            string errmsg = "";

            bool hideApproved = (bool)Session["HideApproved"];
            bool hideCommon = (bool)Session["HideCommonPages"];
            DataSet ds = db.GetReadViewCollection(hideApproved, hideCommon, firstTime, out errmsg);
            if (ds != null)
            {
                nMinImageHeight = 300;
                ICollection ic = CreateImageDataSource(ds);
                if (ic != null)
                {
                    datalistImages.DataSource = ic;
                    datalistImages.DataBind();
                }
            }
            else
            {
                lblError.Text = errmsg;
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


        public ICollection CreateImageDataSource(DataSet ds)
        {

            DataTable dt = new DataTable();

            DataColumn newColumn;
            newColumn = dt.Columns.Add("ImageName", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ImageName2", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ImageDesc", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ImageDescB", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ImageDesc2", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ImageNumber", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ImageNumber2", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ImageNumbers", Type.GetType("System.String"));

            newColumn = dt.Columns.Add("ImageQueryString", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ImageBorder", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ImageWidth", Type.GetType("System.Int32"));
            newColumn = dt.Columns.Add("ImageWidth2", Type.GetType("System.Int32"));
            string NoPagePath = Request.MapPath(Request.ApplicationPath) + "/Images/BlankPage2.gif";

            String sSection = (String)Session["SelectedSection"];
            String sEdition = (String)Session["SelectedEdition"];
           int nProductionID = (int)Session["SelectedProduction"];

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


            bool bAllowPartialProofs = (bool)Application["AllowPartialProofs"];

            DataView dv = GetFilteredTable(ds);
            int nImageNumber = 0;

            // Try to find image dimensions..	
            int nJpegImageWidth = nImageWidth;
            int nJpegImageHeight = nImageHeight;
            bool bHasDimensions = false;
            foreach (DataRowView row in dv)
            {
                if ((int)row["ProofStatus"] >= 10 && bHasDimensions == false)
                {
                    string sMasterCopySeparationSet  = row["MasterCopySeparationSet"].ToString();
                    string sMasterCopySeparationSet2 = row["MasterCopySeparationSet2"].ToString();
                    
                    string fileTitle = previewGUID + "====" + sMasterCopySeparationSet + "_" + sMasterCopySeparationSet2;
                    realPath = Global.sRealReadViewImageFolder + "\\" + fileTitle + ".jpg";

                    bool hasGUIDfile = (bool)Application["OldFileNames"] == false && System.IO.File.Exists(realPath);
                    if (hasGUIDfile == false)
                    {
                        fileTitle = sMasterCopySeparationSet + "_" + sMasterCopySeparationSet2;
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
                            double ratio = 2.0 * (double)(nImageWidth) * (double)TestImage.Size.Height / (double)(TestImage.Size.Width) + 0.5;
                            TestImage.Dispose();
                            nImageHeight = (int)ratio;
                            bHasDimensions = true;
                        }
                        catch
                        {
                            lblError.ForeColor = Color.Red;
                            lblError.Text = "IMAGE NOT FOUND -  (" + fileTitle + ".jpg)";
                        }
                        break;
                    }
                }
            }

            foreach (DataRowView row in dv)
            {

                DateTime thisPubDate = (DateTime)row["PubDate"];
                if (thisPubDate != prevPubDate && prevPubDate.Year >= 2000)
                {
                    // New pubdate coming next! - make sure new pubdate starts on fresh line - fill old line with dummies
                    int nDummyImages = nImagesPerRow - nImageNumber % nImagesPerRow;
                    if (nDummyImages != nImagesPerRow)
                    {
                        for (int im = 0; im < nDummyImages; im++)
                        {
                            DataRow drfill = dt.NewRow();
                            drfill["ImageName"] = "../Images/Spacer.gif";
                            drfill["ImageName2"] = "../Images/Spacer.gif";
                            drfill["ImageDesc"] = "";
                            drfill["ImageDescB"] = "";
                            drfill["ImageDesc2"] = "";
                            drfill["ImageNumber"] = "0&0&0&0";
                            drfill["ImageNumber2"] = "0&0&0&0";
                            drfill["ImageNumbers"] = "0_0&0&0&0_0";
                            drfill["ImageWidth"] = nImageWidth;
                            drfill["ImageWidth2"] = nImageWidth;

                            drfill["ImageQueryString"] = "0_0&0&0&0_0";
                            drfill["ImageBorder"] = " BORDER-RIGHT: white 4px solid; BORDER-TOP: white 4px solid; BORDER-LEFT: white 4px solid; BORDER-BOTTOM: white 4px solid;";

                            nImageNumber++;
                            dt.Rows.Add(drfill);
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
                            DataRow drfill = dt.NewRow();
                            drfill["ImageName"] = "../Images/Spacer.gif";
                            drfill["ImageName2"] = "../Images/Spacer.gif";
                            drfill["ImageDesc"] = "";
                            drfill["ImageDescB"] = "";
                            drfill["ImageDesc2"] = "";
                            drfill["ImageNumber"] = "0&0&0&0";
                            drfill["ImageNumber2"] = "0&0&0&0";
                            drfill["ImageNumbers"] = "0_0&0&0&0_0";
                            drfill["ImageWidth"] = nImageWidth;
                            drfill["ImageWidth2"] = nImageWidth;
                            drfill["ImageQueryString"] = "0_0&0&0&0_0";
                            drfill["ImageBorder"] = " BORDER-RIGHT: white 4px solid; BORDER-TOP: white 4px solid; BORDER-LEFT: white 4px solid; BORDER-BOTTOM: white 4px solid;";

                            nImageNumber++;
                            dt.Rows.Add(drfill);
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
                            DataRow drfill = dt.NewRow();
                            drfill["ImageName"] = "../Images/Spacer.gif";
                            drfill["ImageName2"] = "../Images/Spacer.gif";
                            drfill["ImageDesc2"] = "";
                            drfill["ImageDesc"] = "";
                            drfill["ImageDescB"] = "";
                            drfill["ImageNumber"] = "0&0&0&0";
                            drfill["ImageNumber2"] = "0&0&0&0";
                            drfill["ImageNumbers"] = "0_0&0&0&0_0";
                            drfill["ImageWidth"] = nImageWidth;
                            drfill["ImageWidth2"] = nImageWidth;
                            drfill["ImageQueryString"] = "0_0&0&0&0_0";
                            drfill["ImageBorder"] = " BORDER-RIGHT: white 4px solid; BORDER-TOP: white 4px solid; BORDER-LEFT: white 4px solid; BORDER-BOTTOM: white 4px solid;";

                            nImageNumber++;
                            dt.Rows.Add(drfill);
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
                            DataRow drfill = dt.NewRow();
                            drfill["ImageName"] = "../Images/Spacer.gif";
                            drfill["ImageName2"] = "../Images/Spacer.gif";
                            drfill["ImageDesc"] = "";
                            drfill["ImageDescB"] = "";
                            drfill["ImageDesc2"] = "";
                            drfill["ImageNumber"] = "0&0&0&0";
                            drfill["ImageNumber2"] = "0&0&0&0";
                            drfill["ImageNumbers"] = "0_0&0&0&0_0";
                            drfill["ImageWidth"] = nImageWidth;
                            drfill["ImageWidth2"] = nImageWidth;
                            drfill["ImageQueryString"] = "0_0&0&0&0_0";
                            drfill["ImageBorder"] = " BORDER-RIGHT: white 4px solid; BORDER-TOP: white 4px solid; BORDER-LEFT: white 4px solid; BORDER-BOTTOM: white 4px solid;";

                            nImageNumber++;
                            dt.Rows.Add(drfill);
                        }
                    }
                }
                prevSection = thisSection;

                DataRow dr = dt.NewRow();
                realPath = NoPagePath;
                dr["ImageName"] = (int)row["MasterCopySeparationSet"] > 0 ? ((bool)Application["FlatLook"] ? "NoPage_Flat.gif" : "NoPage.gif") : "BlankPage.gif";
                dr["ImageName2"] = (int)row["MasterCopySeparationSet2"] > 0 ? ((bool)Application["FlatLook"] ? "NoPage_Flat.gif" : "NoPage.gif") : "BlankPage.gif";
                dr["ImageWidth"] = nImageWidth;
                dr["ImageWidth2"] = nImageWidth; 

                if ((int)row["PageType"] == 1 || (int)row["PageType"] == 2)
                {
                    dr["ImageWidth"] = 2 * nImageWidth;
                    dr["ImageWidth2"] = 0;
                }

                int nStatus = Globals.GetStatusID((string)row["Status"], 0);
                bool bGotThumbnailError = false;
                bool hasGUIDfile = false;
                
                if ((int)row["ProofStatus"] == 20 || ((int)row["ProofStatus"] >= 15 && bAllowPartialProofs))
                {

                    string fileTitle = previewGUID + "====" + row["MasterCopySeparationSet"].ToString() + "_" + row["MasterCopySeparationSet2"].ToString(); 
                   
                    realPath = Global.sRealReadViewImageFolder + "\\" + fileTitle + ".jpg";
                    hasGUIDfile = (bool)Application["OldFileNames"] == false && System.IO.File.Exists(realPath);

                    if (hasGUIDfile == false)
                    {
                        fileTitle = row["MasterCopySeparationSet"].ToString() + "_" + row["MasterCopySeparationSet2"].ToString();
                        realPath = Global.sRealReadViewImageFolder + "\\" + fileTitle + ".jpg";
                        hasGUIDfile = System.IO.File.Exists(realPath);
                    }

                    // Load single thumbs into 2x1 table
                    if (hasGUIDfile && System.IO.Directory.Exists(Global.sRealReadViewImageFolder + "\\" + fileTitle + "\\"))
                    {


                        if ((int)row["MasterCopySeparationSet"] > 0)
                        {
                            int nMaxVersion = (int)row["Version"];
                            string sFileToTest;
                            string sFileToShow;
                            if ((bool)Application["UseVersionThumbnails"] && nMaxVersion > 0)
                            {
                                fileTitle = previewGUID + "====" + row["MasterCopySeparationSet"].ToString() + "-" + nMaxVersion.ToString();
                                sFileToTest = Global.sRealThumbnailFolder + "\\" + fileTitle + ".jpg";
                                sFileToShow = Global.sVirtualThumbnailFolder + "/" + fileTitle + ".jpg";

                                hasGUIDfile = (bool)Application["OldFileNames"] == false && System.IO.File.Exists(sFileToTest);
                                if (hasGUIDfile == false)
                                {
                                    fileTitle =  row["MasterCopySeparationSet"].ToString() + "-" + nMaxVersion.ToString();
                                    sFileToTest = Global.sRealThumbnailFolder + "\\" + fileTitle + ".jpg";
                                    sFileToShow = Global.sVirtualThumbnailFolder + "/" + fileTitle + ".jpg";

                                    hasGUIDfile = System.IO.File.Exists(sFileToTest);
                                }
                            }
                            else
                            {
                                fileTitle = previewGUID + "====" + row["MasterCopySeparationSet"].ToString();
                                sFileToTest = Global.sRealThumbnailFolder + "\\" + fileTitle + ".jpg";
                                sFileToShow = Global.sVirtualThumbnailFolder + "/" + fileTitle + ".jpg";

                                hasGUIDfile = (bool)Application["OldFileNames"] == false && System.IO.File.Exists(sFileToTest);
                                if (hasGUIDfile == false)
                                {
                                    fileTitle = row["MasterCopySeparationSet"].ToString();
                                    sFileToTest = Global.sRealThumbnailFolder + "\\" + fileTitle + ".jpg";
                                    sFileToShow = Global.sVirtualThumbnailFolder + "/" + fileTitle + ".jpg";

                                    hasGUIDfile = System.IO.File.Exists(sFileToTest);
                                }
                            }

                            dr["ImageName"] = hasGUIDfile ? Globals.EncodePreviewName(sFileToShow)  : ((bool)Application["FlatLook"] ? "PageMissing_Flat.gif" : "PageMissing.gif");
                        }
                        else
                            dr["ImageName"] = "BlankPage.gif";

                        if ((int)row["MasterCopySeparationSet2"] > 0)
                        {
                            int nMaxVersion = (int)row["Version"];
                            string sFileToTest;
                            string sFileToShow;
                            if ((bool)Application["UseVersionThumbnails"] && nMaxVersion > 0)
                            {
                                fileTitle = previewGUID + "====" + row["MasterCopySeparationSet2"].ToString() + "-" + nMaxVersion.ToString();
                                sFileToTest = Global.sRealThumbnailFolder + "\\" + fileTitle + ".jpg";
                                sFileToShow = Global.sVirtualThumbnailFolder + "/" + fileTitle + ".jpg";

                                hasGUIDfile = (bool)Application["OldFileNames"] == false && System.IO.File.Exists(sFileToTest);
                                if (hasGUIDfile == false)
                                {
                                    fileTitle = row["MasterCopySeparationSet2"].ToString() + "-" + nMaxVersion.ToString();
                                    sFileToTest = Global.sRealThumbnailFolder + "\\" + fileTitle + ".jpg";
                                    sFileToShow = Global.sVirtualThumbnailFolder + "/" + fileTitle + ".jpg";

                                    hasGUIDfile = System.IO.File.Exists(sFileToTest);
                                }
                            }
                            else
                            {
                                fileTitle = previewGUID + "====" + row["MasterCopySeparationSet2"].ToString();
                                sFileToTest = Global.sRealThumbnailFolder + "\\" + fileTitle + ".jpg";
                                sFileToShow = Global.sVirtualThumbnailFolder + "/" + fileTitle + ".jpg";

                                hasGUIDfile = (bool)Application["OldFileNames"] == false && System.IO.File.Exists(sFileToTest);
                                if (hasGUIDfile == false)
                                {
                                    fileTitle = row["MasterCopySeparationSet2"].ToString();
                                    sFileToTest = Global.sRealThumbnailFolder + "\\" + fileTitle + ".jpg";
                                    sFileToShow = Global.sVirtualThumbnailFolder + "/" + fileTitle + ".jpg";

                                    hasGUIDfile = System.IO.File.Exists(sFileToTest);
                                }
                            }

                            dr["ImageName2"] = hasGUIDfile ? Globals.EncodePreviewName(sFileToShow) : ((bool)Application["FlatLook"] ? "PageMissing_Flat.gif" : "PageMissing.gif");
                        }
                        else
                            dr["ImageName2"] = "BlankPage.gif";
                           
                            
                    }
                    else
                        realPath = NoPagePath;
                }

                string sPageName = (string)row["Page"];
                string sPageName2 = (string)row["Page2"];

                bool bIsNumber = true;
                foreach (char c in sPageName)
                {
                    if (c < '0' || c > '9')
                    {
                        bIsNumber = false;
                        break;
                    }
                }
                /*				if (sPageName[0] < '0' || sPageName[0] > '9')
                                    bIsNumber = false;
                                if (sPageName[sPageName.Length-1] < '0' || sPageName[sPageName.Length-1] > '9')
                                    bIsNumber = false;
                */
                if (sSection == "" && hideSection == false && bIsNumber)
                {
                    dr["ImageDesc"] = thisSection + " " + sPageName;
                    dr["ImageDescB"] = thisSection + " " + sPageName2;
                }
                else
                {
                    dr["ImageDesc"] = sPageName;
                    dr["ImageDescB"] = sPageName2;
                }

                if (sPageName == "0")
                    dr["ImageDesc"] = "";
                if (sPageName2 == "0")
                    dr["ImageDescB"] = "";


                dr["ImageDesc2"] = "V" + ((int)row["Version"]).ToString() + "  " + (string)row["Status"];

                string sColor = (string)row["Color"];
                sColor = sColor.Replace(";", "_");

                int nApp = (int)row["Approval"];


                dr["ImageNumber"] = row["MasterCopySeparationSet"].ToString() + "&" + sColor + "&" + nApp.ToString() + "&" + (string)row["Page"];
                dr["ImageNumber2"] = row["MasterCopySeparationSet2"].ToString() + "&" + sColor + "&" + nApp.ToString() + "&" + (string)row["Page"];

                dr["ImageNumbers"] = row["MasterCopySeparationSet"].ToString() + "_" + row["MasterCopySeparationSet2"].ToString() + "&" + sColor + "&" + nApp.ToString() + "&" + (string)row["Page"] + "_" + (string)row["Page2"] + "&" + nStatus.ToString();

                if (realPath != NoPagePath)
                    dr["ImageQueryString"] = "set=" + row["MasterCopySeparationSet"].ToString() + "_" + row["MasterCopySeparationSet2"].ToString() + "&colors=" + sColor + "&approval=" + nApp.ToString() + "&pagename=" + (string)row["Page"] + "_" + (string)row["Page2"];
                else
                    dr["ImageQueryString"] = "set=0_0&colors=" + sColor + "&approval=" + nApp.ToString() + "&pagename=" + (string)row["Page"] + "_" + (string)row["Page2"];

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

                nImageNumber++;
                dt.Rows.Add(dr);
            }
            return dt.DefaultView;
        }


		private DataView GetFilteredTable(DataSet dataset)
		{
		/*	bool hideEdition = Globals.GetCacheRowCount("EditionNameCache") < 2;
			bool hideSection = Globals.GetCacheRowCount("SectionNameCache") < 2;
			bool hideIssue = Globals.GetCacheRowCount("IssueNameCache") < 2;
			
			string displayFilter = "  ";

			if ((string)Session["SelectedPublication"] != "")
				displayFilter += " " + (string)Session["SelectedPublication"];

			DateTime selectedPubDate = (DateTime)Session["SelectedPubDate"];
			if (selectedPubDate.Year > 2000)
				displayFilter += " " + selectedPubDate.Month + "/" + selectedPubDate.Day + "/" + selectedPubDate.Year;

			if ((string)Session["SelectedIssue"] != "" && hideIssue == false)
				displayFilter += " " + (string)Session["SelectedIssue"];

			if ((string)Session["SelectedEdition"] != "" && hideEdition == false)
				displayFilter += " " + (string)Session["SelectedEdition"];
	
			if ((string)Session["SelectedSection"] != "" && hideSection == false)
				displayFilter += " " + (string)Session["SelectedSection"];

			if (displayFilter == "  ")
				displayFilter = "  All";


            SetFilterLabel(displayFilter, Color.Blue); */
			return dataset.Tables[0].DefaultView;
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

        private void PrepareZoom(int masterCopySeparationSetLeft, int masterCopySeparationSetRight, string colors, string approval, string pageName, string pageName2)
        {
            Session["CurrentCopySeparationSet"] = masterCopySeparationSetLeft;
            Session["CurrentCopySeparationSet2"] = masterCopySeparationSetRight;
            Session["CurrentPageName"] = pageName;
            Session["CurrentPageName2"] = pageName2;
            Session["ImageColors"] = colors;
            Session["ImagePath"] = Global.sVirtualReadViewImageFolder + "/" + masterCopySeparationSetLeft.ToString() + "_" + masterCopySeparationSetRight.ToString() + ".jpg";
            Session["ImagePathMask"] = "";
            Session["ShowSep"] = "CMYK";
            Session["HasTiles"] = false;

            bool hasMask = false;
            bool hasTiles = false;
            string errmsg = "";
            CCDBaccess db = new CCDBaccess();

            int publicationID = 0;
            //int version = 0;
            string currentComment = "";
            DateTime pubDate = DateTime.MinValue;
            db.GetComment(masterCopySeparationSetLeft > 0 ? masterCopySeparationSetLeft : masterCopySeparationSetRight, out currentComment, out publicationID, out pubDate, out errmsg);

            Session["CurrentComment"] = (bool)Session["SetCommentOnDisapproval"] ? currentComment : "";
            Session["CurrentApprovalState"] = Globals.TryParse(approval, -1);

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

/*            bool isMobileDevice = false;
            if (Globals.IsMobileClient(HttpContext.Current.Request, HttpContext.Current.Response))
            {
                hasTiles = false;
                isMobileDevice = true;
            }
*/
            //			string folderToTest = Global.sRealReadViewImageFolder + "\\" + masterCopySeparationSetLeft.ToString()+ "_" + masterCopySeparationSetRight.ToString() + "\\";

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

            if ((string)Session["ImagePath"] != "")
            {
                Session["RealImagePath"] = realPath;	// For mail attachment only

                Response.Redirect("ZoomviewReadFlash2.aspx");
               
            }

        }

		private void Thumbnail_ItemCommand(object source, System.Web.UI.WebControls.DataListCommandEventArgs e)
		{
            doPopupColor = false;
            lblError.ForeColor = Color.Green;
            string scmd = (string)e.CommandArgument;

            if (scmd == "")
                return;

            string[] sargs = scmd.Split('&');
            string[] masterargs = sargs[0].Split('_');
            int masterCopySeparationSetLeft = Int32.Parse(masterargs[0]);
            int masterCopySeparationSetRight = Int32.Parse(masterargs[1]);


            string[] pagenameargs = sargs[3].Split('_');


            if (e.CommandName == "View")
            {
                PrepareZoom(masterCopySeparationSetLeft, masterCopySeparationSetRight, sargs[1], sargs[2], pagenameargs[0], pagenameargs[1]);
            }

            if (e.CommandName == "Approve")
            {
                if ((bool)Session["MayApprove"] == true)
                {
                    string errmsg;

                    CCDBaccess db = new CCDBaccess();

                    if (masterCopySeparationSetLeft > 0)
                    {
                        if (db.UpdateApproval((string)Session["UserName"], masterCopySeparationSetLeft, 1, out errmsg) == false)
                        {
                            lblError.ForeColor = Color.Red;
                            lblError.Text = "Could not update approve status";
                        }
                        if ((bool)Session["LogApprove"])
                        {
                            db.UpdateApproveLog(masterCopySeparationSetLeft, 1, true, "", (string)Session["UserName"], out  errmsg);
                        }
                    }
                    if (masterCopySeparationSetRight > 0)
                    {
                        if (db.UpdateApproval((string)Session["UserName"], masterCopySeparationSetRight, 1, out errmsg) == false)
                        {
                            lblError.ForeColor = Color.Red;
                            lblError.Text = "Could not update approve status";
                        }
                        if ((bool)Session["LogApprove"])
                        {
                            db.UpdateApproveLog(masterCopySeparationSetRight, 1, true, "", (string)Session["UserName"], out  errmsg);
                        }
                    }
                }
                DoDataBind(false);

                //	updateTree = true;
            }

            if (e.CommandName == "Disapprove")
            {
                if ((bool)Session["MayApprove"] == true)
                {
                    string errmsg;
                    CCDBaccess db = new CCDBaccess();

                    if (masterCopySeparationSetLeft > 0)
                    {
                        if (db.UpdateApproval((string)Session["UserName"], masterCopySeparationSetLeft, 2, out errmsg) == false)
                        {
                            lblError.ForeColor = Color.Red;
                            lblError.Text = "Could not update approve status";
                        }

                        if ((bool)Session["LogDisapprove"])
                        {
                            db.UpdateApproveLog(masterCopySeparationSetLeft, 1, false, "", (string)Session["UserName"], out  errmsg);
                        }
                    }

                    if (masterCopySeparationSetRight > 0)
                    {
                        if (db.UpdateApproval((string)Session["UserName"], masterCopySeparationSetRight, 2, out errmsg) == false)
                        {
                            lblError.ForeColor = Color.Red;
                            lblError.Text = "Could not update approve status";
                        }

                        if ((bool)Session["LogDisapprove"])
                        {
                            db.UpdateApproveLog(masterCopySeparationSetRight, 1, false, "", (string)Session["UserName"], out  errmsg);
                        }
                    }
                }

                DoDataBind(false);
                //	updateTree = true;
            }


            if (e.CommandName == "Resetapprove")
            {
                if ((bool)Session["MayApprove"] == true)
                {
                    string errmsg;
                    CCDBaccess db = new CCDBaccess();

                    if (masterCopySeparationSetLeft > 0)
                    {
                        if (db.UpdateApproval((string)Session["UserName"], masterCopySeparationSetLeft, 0, out errmsg) == false)
                        {
                            lblError.ForeColor = Color.Red;
                            lblError.Text = "Could not update approve status";
                        }

                        if ((bool)Session["LogDisapprove"])
                        {
                            db.UpdateApproveLog(masterCopySeparationSetLeft, 1, false, "Reset to not approved", (string)Session["UserName"], out  errmsg);
                        }
                    }

                    if (masterCopySeparationSetRight > 0)
                    {
                        if (db.UpdateApproval((string)Session["UserName"], masterCopySeparationSetRight, 0, out errmsg) == false)
                        {
                            lblError.ForeColor = Color.Red;
                            lblError.Text = "Could not update approve status";
                        }

                        if ((bool)Session["LogDisapprove"])
                        {
                            db.UpdateApproveLog(masterCopySeparationSetRight, 1, false, "Reset to not approved", (string)Session["UserName"], out  errmsg);
                        }
                    }
                }

                DoDataBind(false);
                //	updateTree = true;
            }



            if (e.CommandName == "Color")
            {
                if (sargs[1] != "PDF" && sargs[1] != "PDFmono")
                {
                    ReBind();
                    //Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowChangeColor");
                    Telerik.Web.UI.RadWindow mywindow = RadWindowManager1.Windows[0];
                    mywindow.NavigateUrl = "ChangeColor.aspx?mastercopyseparationset=" + masterCopySeparationSetLeft.ToString() + "&mastercopyseparationset2=" + masterCopySeparationSetRight.ToString();

                    //					doPopupColor = true;
                    //					doPopupColorWindow();
                    mywindow.VisibleOnPageLoad = true;

                }
            }
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

            Page.ClientScript.RegisterStartupScript(this.GetType(),"PopupScript", popupScript, false);
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
                ImageButton colorImage1 = (ImageButton)e.Item.FindControl("btnColor");
                Panel panelBottom = (Panel)e.Item.FindControl("pnlFooter");
                Panel pnlFooter2 = (Panel)e.Item.FindControl("pnlFooter2");

                ImageButton btnResetapprove = (ImageButton)e.Item.FindControl("btnResetApproval");
                btnResetapprove.Visible = (bool)Application["ShowResetApprove"];
                btnResetapprove.ToolTip = Global.rm.GetString("txtResetApprove");


                HtmlInputHidden hidden = (HtmlInputHidden)e.Item.FindControl("hiddenImageID");
                string scmd = hidden.Value;
                string[] sargs = scmd.Split('&');


                if (sargs[0] == "0")
                {
                    // Place filler
                    if (table != null)
                        table.Style.Add("border", "none");
                    thumbTxt.Visible = false;
                    colorImage1.Visible = false;
                    panelBottom.Visible = false;
                    ImageButton imgbtn = (ImageButton)e.Item.FindControl("btnApprove");
                    imgbtn.Visible = false;
                    imgbtn = (ImageButton)e.Item.FindControl("btnDisapprove");
                    imgbtn.Visible = false;
                }
                else
                {
                    string[] masterargs = sargs[0].Split('_');
                    int masterCopySeparationSetLeft = Int32.Parse(masterargs[0]);
                    int masterCopySeparationSetRight = Int32.Parse(masterargs[1]);

                    ImageButton imgbtn = (ImageButton)e.Item.FindControl("btnApprove");
                    imgbtn.Visible = (bool)Session["MayApprove"];
                    imgbtn = (ImageButton)e.Item.FindControl("btnDisapprove");
                    imgbtn.Visible = (bool)Session["MayApprove"];
                    // Adjust color according to approved state

                    string approveState = sargs[2];
                    string colorString = sargs[1];
                    string[] sargscolors = colorString.Split('_');


                    if (approveState == "1")
                    {
                        thumbTxt.BackColor = Color.LawnGreen;
                        panelBottom.BackColor = Color.LawnGreen;

                        if ((bool)Application["FlatLook"] == false)
                        {
                            thumbTxt.BackImageUrl = "../Images/greengradient2.gif";
                            panelBottom.BackImageUrl = "../Images/greengradient2.gif";
                        }
                    }
                    else if (approveState == "2")
                    {
                        thumbTxt.BackColor = Color.Red;
                        panelBottom.BackColor = Color.Red;

                        if ((bool)Application["FlatLook"] == false)
                        {
                            thumbTxt.BackImageUrl = "../Images/redgradient2.gif";
                            panelBottom.BackImageUrl = "../Images/redgradient2.gif";
                        }
                    }
                    else if (approveState == "0")
                    {
                        thumbTxt.BackColor = Color.LightGray;
                        panelBottom.BackColor = Color.LightGray;
                        if ((bool)Application["FlatLook"] == false)
                        {
                            thumbTxt.BackImageUrl = "../Images/graygradient2.gif";
                            panelBottom.BackImageUrl = "../Images/graygradient2.gif";
                        }
                    }
                    else
                    {
                        thumbTxt.BackColor = Color.LightSkyBlue;
                        panelBottom.BackColor = Color.LightSkyBlue;
                        if ((bool)Application["FlatLook"] == false)
                        {
                            thumbTxt.BackImageUrl = "../Images/bluegradient2.gif";
                            panelBottom.BackImageUrl = "../Images/bluegradient2.gif";
                        }
                    }

                    pnlFooter2.BackColor = thumbTxt.BackColor;
                    colorImage1.Visible = false;
                    if ((bool)Application["AllowColorChange"])
                    {
                        bool colorok = false;
                        colorImage1.Visible = true;

                        if (sargscolors.Length == 1)
                        {
                            if (colorString == "PDF")
                            {
                                colorImage1.ImageUrl = "../Images/colorPDFbig.gif";
                            }
                            else if (colorString == "PDFmono")
                            {
                                colorImage1.ImageUrl = "../Images/monoPDFbig.gif";
                            }
                            else if (Globals.IsBlackColor(colorString))
                            {
                                colorImage1.ImageUrl = "../Images/colorKbig.gif";
                            }
                            else
                            {
                                colorImage1.ImageUrl = "../Images/colorKbig.gif";			// Just default to black....?
                            }
                            colorok = true;
                        }

                        if (sargscolors.Length == 4)
                        {
                            if (Globals.IsProcessColor(sargscolors[0]) && Globals.IsProcessColor(sargscolors[1]) && Globals.IsProcessColor(sargscolors[2]) && Globals.IsProcessColor(sargscolors[3]))
                            {
                                colorok = true;
                                colorImage1.ImageUrl = "../Images/colorcmyk.gif";
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
                                //SolidBrush brush = new SolidBrush(Globals.GetColorFromName(sargscolors[nColorNumber]));
                                System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(rectGrad, Color.WhiteSmoke, Globals.GetColorFromName(sargscolors[nColorNumber]), 90.0F);
                                g.FillRectangle(brush, nColorNumber * 8, 0, 8, 16);

                            }

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

                    if ((bool)Application["ThumbnailShowStatusColors"])
                    {

                        int nStatus = 0;
                        if (sargs.Length >= 5)
                            nStatus = Globals.TryParse(sargs[4], 0);
                        if (nStatus == 0)
                        {
                            panelBottom.BackColor = Color.LightGray;
                            if ((bool)Application["FlatLook"] == false)
                                panelBottom.BackImageUrl = "../Images/graygradient2.gif";
                        }

                        else if (nStatus > 0 && nStatus < 28)
                        {
                            panelBottom.BackColor = Color.Yellow;

                            if ((bool)Application["FlatLook"] == false)
                                panelBottom.BackImageUrl = "../Images/yellowgradient2.gif";
                        }
                        else if (nStatus == 28)	// Special channle done - rest still not done
                        {
                            panelBottom.BackColor = Color.MediumPurple;
                            if ((bool)Application["FlatLook"] == false)
                                panelBottom.BackImageUrl = "../Images/purplegradient2.gif";
                        }

                        else if (nStatus == 29)
                        {
                            panelBottom.BackColor = Color.Orange;
                            if ((bool)Application["FlatLook"] == false)
                                panelBottom.BackImageUrl = "../Images/orangegradient2.gif";
                        }
                        else if (nStatus == 30)
                        {
                            panelBottom.BackColor = Color.Yellow;
                            if ((bool)Application["FlatLook"] == false)
                                panelBottom.BackImageUrl = "../Images/yellowgradient2.gif";
                        }

                        else if (nStatus == 16 || nStatus == 26 || nStatus == 36)
                        {
                            panelBottom.BackColor = Color.Red;
                            if ((bool)Application["FlatLook"] == false)
                                panelBottom.BackImageUrl = "../Images/redgradient2.gif";
                        }
                        else // Done...
                        {
                            panelBottom.BackColor = Color.LawnGreen;
                            if ((bool)Application["FlatLook"] == false)
                                panelBottom.BackImageUrl = "../Images/greengradient2.gif";
                        }
                    }
                }
            }
        }
	
        protected void RadToolBar1_ButtonClick(object sender, Telerik.Web.UI.RadToolBarEventArgs e)
		{
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

            else if (e.Item.Value == "Download")			
				GeneratePDF();

            nRefreshTime = (int)Session["RefreshTime"];

			ReBind();
		}

		private void ReBind()
		{
			ImageC = System.Drawing.Image.FromFile(Request.MapPath(Request.ApplicationPath) + "/Images/colorC.bmp");
			ImageM = System.Drawing.Image.FromFile(Request.MapPath(Request.ApplicationPath) + "/Images/colorM.bmp");
			ImageY = System.Drawing.Image.FromFile(Request.MapPath(Request.ApplicationPath) + "/Images/colorY.bmp");
			ImageK = System.Drawing.Image.FromFile(Request.MapPath(Request.ApplicationPath) + "/Images/colorK.bmp");
			ImageS = System.Drawing.Image.FromFile(Request.MapPath(Request.ApplicationPath) + "/Images/colorS.bmp");
				
             Telerik.Web.UI.RadToolBarButton btn = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("HideApproved");
            if (btn != null)
                Session["HideApproved"] = btn.Checked;
            btn = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("HideCommon");
            if (btn != null)
                Session["HideCommonPages"] = btn.Checked;

            SetScreenSize();
            
            DoDataBind(false);

            if ((bool)Application["NoCache"])
            {
                Response.AppendHeader("cache-control", "private");
                Response.AppendHeader("pragma", "no-cache");
                Response.CacheControl = "Private";
            }

        //    Response.AddHeader("Refresh", "");

		}

		private bool GeneratePDF()
		{
			string errmsg;
			CCDBaccess db =  new CCDBaccess();

			bool hideApproved = (bool)Session["HideApproved"];
			bool hideCommon = (bool)Session["HideCommonPages"];
			ArrayList al = db.GetReadViewMasterSetCollection(hideApproved, hideCommon, false, out errmsg);  
			if (errmsg != "")
			{
				lblError.Text = errmsg;
				return false;
			}

			if (al.Count == 0)
			{
				lblError.Text = "No pages ready";
				return false;
			}

			ArrayList al2 = new ArrayList();

			foreach (string s in al)
			{
				string fname = Global.sRealImageFolder + @"\" + s + ".jpg";

				if (System.IO.File.Exists(fname))
				{
					al2.Add(fname);
				}
			}

			if (al2.Count == 0)
			{
				lblError.Text = "No spread previews ready";
				return false;
			}

			PDFlib p = new PDFlib();
			try	
			{
				p.set_parameter("warning", "false");
                p.set_parameter("license", "W900202-010068-132518-7XKA62-BCJA82");


                if (p.begin_document("", "") == -1)
				{
					lblError.Text = "PDF error " + p.get_errmsg();            
					return false;
				}

				//p.set_parameter("SearchPath", searchpath);
				

				p.set_info("Creator", "InfraLogic");
				p.set_info("Author", "Niels Andersen");
				p.set_info("Title", "WebCenter preview document");

				// This line is required to avoid problems on Japanese systems 
				//				p.set_parameter("hypertextencoding", "host");


				for (int i=0; i<al2.Count; i++) 
				{
				
					int image = p.load_image("jpeg", (string)al2[i], "");
					if (image == -1) 
					{
						lblError.Text = "PDF error (load JPG)" + p.get_errmsg();            
						return false;
					}

					/* dummy page size, will be adjusted by PDF_fit_image() */
					p.begin_page_ext(10, 10, "");
					p.fit_image(image, 0.0, 0.0, "adjustpage");
					p.close_image(image);
					p.end_page_ext("");
				}
				p.end_document("");

				Byte [] buf = p.get_buffer();

				string pdfFileName = "";
				bool hideEdition = Globals.GetCacheRowCount("EditionNameCache") < 2;
				bool hideSection = Globals.GetCacheRowCount("SectionNameCache") < 2;


				if ((string)Session["SelectedPublication"] != "")
					pdfFileName += (string)Session["SelectedPublication"] + "_";

				DateTime selectedPubDate = (DateTime)Session["SelectedPubDate"];
				if (selectedPubDate.Year > 2000)
					pdfFileName += "_" + selectedPubDate.Month  + selectedPubDate.Day + selectedPubDate.Year ;

				if ((string)Session["SelectedEdition"] != "" && hideEdition == false)
					pdfFileName +=   "_" + (string)Session["SelectedEdition"];
	
				if ((string)Session["SelectedSection"] != "" && hideSection == false)
					pdfFileName +=  "_" + (string)Session["SelectedSection"];


				pdfFileName += "_spreads.pdf";

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
				Response.AppendHeader("Content-Disposition", "attachment; filename="+asciiString);
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
