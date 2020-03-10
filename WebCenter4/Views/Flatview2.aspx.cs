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
using System.Drawing.Imaging;
using WebCenter4.Classes;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Text;
using System.IO;

namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for Flatview.
	/// </summary>
	public partial class Flatview2 : System.Web.UI.Page
	{


		public int PAGEWIDTH;
		public int PAGEWIDTHLANDSCAPE;
		static int FRAMEWIDTH = 5;

		public int nImageWidth;
        public int nImageHeight;
        public int nImagesPerRow;
		public int nScreenWidth;

        public int nMinImageHeight = 300;
        public int nRefreshTime;
        public int nScollPos = 0;

        public string tooltipClickImage = "Click on page to show preview";

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
                    int flatset = Globals.TryParse(sepset, 0);

                    string sver = Request.QueryString["ver"];
                    int version = Globals.TryParse(sver, 1);
                    if (flatset > 0)
                        PrepareZoom(flatset, version);

                }
                catch // (Exception e1)
                {
                    ;
                }
            }

         
            // Initial setup 
            if (!Page.IsPostBack)
            {
                if ((string)Session["SelectedPress"] == "")
                {
                    DataTable dt = (bool)Application["UsePressGroups"] ? (DataTable)Cache["PressGroupNameCache"] : (DataTable)Cache["PressNameCache"];
                    Session["SelectedPress"] = (string)dt.Rows[0]["name"];

                }

                SetLanguage();
                SetSplitButtonValue();
              
                Telerik.Web.UI.RadToolBarButton item = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("HideCommon");
                if (item != null)
                    item.Checked = (bool)Session["HideCommon"];
            }
         
            SetScreenSize();

            if (!Page.IsPostBack || HiddenReturendFromPopup.Value != "0")
			{
				DoDataBind(false);
			}

            HiddenReturendFromPopup.Value = "0";
          	
			// Loop through all windows in the WindowManager.Windows collection
			foreach (Telerik.Web.UI.RadWindow win in RadWindowManager1.Windows)
			{
				//Set whether the first window will be visible on page load
				win.VisibleOnPageLoad = false;
			}

            SetRefreshheader();
		}

        private void SetRefreshheader()
        {
           // if (nRefreshTime > 0)
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

        protected void 	SetScreenSize()
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
                w = Globals.TryParseCookie(Request, "ScreenWidthFlat", 800);
                h = Globals.TryParseCookie(Request, "ScreenHeightFlat", 600);
            }

            if (w <= 0)
                w = (int)Session["WindowWidth"] > 0 ? (int)Session["WindowWidth"] : 800;
            if (h <= 0)
                h = (int)Session["WindowHeight"] > 0 ? (int)Session["WindowHeight"] : 600;

            // nScrollPos is exposed in aspx code used in clientcode to set scrillbar after load
            nScollPos = 0;
           
            if (HiddenScrollPos.Value != "")
                nScollPos = Globals.TryParse(HiddenScrollPos.Value, 0);

            if (nScollPos <= 0)
            {
                try
                {
                    string s = Request.Cookies["ScrollFlatY"].Value;
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
			nScreenWidth = w;

            nImagesPerRow = (int)Session["PlatesPerRow"];
            nRefreshTime = (int)Session["RefreshTime"];

          //  nImageWidth = (w - 60) / nImagesPerRow - (nImagesPerRow - 1);

            nImageWidth = (w - 2 - 8 * nImagesPerRow) / nImagesPerRow;

            if (nImageWidth > 1)
                Session["FlatPageSize"] = nImageWidth;

            // Blank dummy image is 160x118 - set a default height used if no pages are avail. - otherwise height is set by first detected image.
            nImageHeight = nImageWidth * 118;
            nImageHeight /= 160;
        }
        
        protected void SetLanguage()
		{
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            SetRadToolbarLabel("Refresh", Global.rm.GetString("txtRefresh"));
//SetRadToolbarTooltip("Refresh", Global.rm.GetString("txtRefresh"));
            SetRadToolbarLabel("HideCommon", Global.rm.GetString("txtHideDuplicates"));
            SetRadToolbarTooltip("HideCommon", Global.rm.GetString("txtTooltipHideDuplicates"));

            lblChooseProduct.Text = Global.rm.GetString("txtChooseProduct");
    
           Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue("ReleaseAll");
            if (item != null)
            {
                item.Text = Global.rm.GetString("txtReleaseAll");
                item.ToolTip = Global.rm.GetString("txtTooltipReleaseAll");
                item.Enabled = (bool)Application["ShowApproveAllButton"] && (bool)Session["MayApprove"];
                item.Visible = (bool)Application["ShowApproveAllButton"] && (bool)Session["MayApprove"];
            }
            item = null;
            item = RadToolBar1.FindItemByValue("RetransmitAll");
            if (item != null)
            {
                item.Text = Global.rm.GetString("txtRetransmitAll");
                item.ToolTip = Global.rm.GetString("txtTooltipRetransmitAll");
                item.Enabled = false; // (bool)Application["ShowRetransmitAllButton"] && (bool)Session["MayApprove"];
                item.Visible = false; //(bool)Application["ShowRetransmitAllButton"] && (bool)Session["MayApprove"];
            }

            Telerik.Web.UI.RadToolBarSplitButton itemsb = (Telerik.Web.UI.RadToolBarSplitButton)RadToolBar1.FindItemByValue("PlatePerRowSelector");
            if (itemsb == null)
                return;
            itemsb.ToolTip = Global.rm.GetString("txtTooltipIPlatesPerRow");
            Telerik.Web.UI.RadToolBarButton subitem = (Telerik.Web.UI.RadToolBarButton)itemsb.Buttons.FindItemByValue("PlatePerRow2");
            subitem.Text = Global.rm.GetString("txtPlatesPerRow") + " 2";
            subitem = (Telerik.Web.UI.RadToolBarButton)itemsb.Buttons.FindItemByValue("PlatePerRow4");
            subitem.Text = Global.rm.GetString("txtPlatesPerRow") + " 4";
            subitem = (Telerik.Web.UI.RadToolBarButton)itemsb.Buttons.FindItemByValue("PlatePerRow6");
            subitem.Text = Global.rm.GetString("txtPlatesPerRow") + " 6";
            subitem = (Telerik.Web.UI.RadToolBarButton)itemsb.Buttons.FindItemByValue("PlatePerRow8");
            subitem.Text = Global.rm.GetString("txtPlatesPerRow") + " 8";
            subitem = (Telerik.Web.UI.RadToolBarButton)itemsb.Buttons.FindItemByValue("PlatePerRow10");
            subitem.Text = Global.rm.GetString("txtPlatesPerRow") + " 10";
		}

        protected void SetSplitButtonValue()
        {
            Telerik.Web.UI.RadToolBarSplitButton itemsb = (Telerik.Web.UI.RadToolBarSplitButton)RadToolBar1.FindItemByValue("PlatePerRowSelector");
            if (itemsb == null)
                return;

            if ((int)Session["PlatesPerRow"] < 2 || (int)Session["PlatesPerRow"] > 10)
                Session["PlatesPerRow"] = 4;
            string s = "PlatePerRow" + ((int)Session["PlatesPerRow"]).ToString();

            for (int i = 0; i < itemsb.Buttons.Count; i++)
            {
                if (itemsb.Buttons[i].Value == s)
                {
                    itemsb.DefaultButtonIndex = i;
                    break;
                }
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
			this.FlatList.ItemCreated += new System.Web.UI.WebControls.DataListItemEventHandler(this.FlatList_ItemCreated);
			this.FlatList.ItemCommand += new System.Web.UI.WebControls.DataListCommandEventHandler(this.FlatList_ItemCommand);
			this.FlatList.ItemDataBound += new System.Web.UI.WebControls.DataListItemEventHandler(this.FlatList_ItemDataBound);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

        private void ReBind(bool includeRefresh)
        {


            Telerik.Web.UI.RadToolBarButton btn = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("HideCommon");
            if (btn != null)
                Session["HideCommon"] = btn.Checked;

            SetScreenSize();

            DoDataBind(false);

            if (includeRefresh)
                SetRefreshheader();
        }

        public void DoDataBind(bool firstTime)
        {
            if ((string)Session["SelectedPublication"] == "")
            {
                lblChooseProduct.Visible = true;
                return;
            }
            lblChooseProduct.Visible = false;

            SetScreenSize();

            CCDBaccess db = new CCDBaccess();

            string errmsg = "";
            int maxCopyNumber = 1;
            DataTable dt = db.GetFlatPageCollection(false, false, false, true, out maxCopyNumber, out errmsg); // Give us all copies
            if (dt != null)
            {
                nMinImageHeight = 300;
                ICollection ic = CreateImageDataSource(dt, maxCopyNumber);
                if (ic != null)
                {
                    FlatList.DataSource = ic;
                    FlatList.DataBind();
                }
            }
            else
            {
                lblError.Text = errmsg;
            }
           
           // SetFilterLabel();
        }

        private void FlatList_ItemCommand(object source, System.Web.UI.WebControls.DataListCommandEventArgs e)
        {
            lblError.ForeColor = Color.Green;
            string scmd = (string)e.CommandArgument;

            string[] sargs = scmd.Split('&');

            string errmsg;
            CCDBaccess db = new CCDBaccess();

            int copyFlatSeparationSet = 0;
            int version = 1;
            bool isPDF = false;
            if (sargs.Length > 0)
                copyFlatSeparationSet = Globals.TryParse(sargs[0], 0);


            if (copyFlatSeparationSet == 0)
                return;

            db.GetPlateVersion(copyFlatSeparationSet, ref version, out errmsg);

            ArrayList aColors = new ArrayList();
            db.GetPlateColors(copyFlatSeparationSet, ref aColors, out errmsg);
            if (aColors.Count == 1)
                if ((string)aColors[0] == "PDF")
                    isPDF = true;

            if (e.CommandName == "View")
            {
                PrepareZoom(copyFlatSeparationSet, version);
            }

            if (e.CommandName == "HardProof")
            {
                Session["SelectedMasterSet"] = 0;
                Session["SelectedCopyFlatSeparationSet"] = copyFlatSeparationSet;
                Telerik.Web.UI.RadWindow mywindow = (Telerik.Web.UI.RadWindow)RadWindowManager1.Windows[2]; // "radWindowHardProof"
                mywindow.VisibleOnPageLoad = true;

            }

            if (e.CommandName == "Approve")
            {
                if ((bool)Session["MayApprove"] == true)
                {
                    if (db.UpdateFlatApproval((string)Session["UserName"], copyFlatSeparationSet, 1, out errmsg) == false)
                    {
                        lblError.ForeColor = Color.Red;
                        lblError.Text = "Could not update flat approve status";
                    }
                }
            }

            if (e.CommandName == "Disapprove")
            {
                if ((bool)Session["MayApprove"] == true)
                {
                    if (db.UpdateFlatApproval((string)Session["UserName"], copyFlatSeparationSet, 2, out errmsg) == false)
                    {
                        lblError.ForeColor = Color.Red;
                        lblError.Text = "Could not update flat approve status";
                    }
                }
            }

            if (e.CommandName == "Release")
            {
                if ((bool)Session["MayRelease"] == true)
                {

                    if (db.UpdateCopyFlatHold(copyFlatSeparationSet, 0, 0, out errmsg) == false)
                    {
                        lblError.ForeColor = Color.Red;
                        lblError.Text = "Could not update flat hold status";
                    }
                }
            }

            if (e.CommandName == "ReleaseBlack")
            {
                if ((bool)Session["MayRelease"] == true)
                {

                    if (db.UpdateCopyFlatHold(copyFlatSeparationSet, 0, 4, out errmsg) == false)
                    {
                        lblError.ForeColor = Color.Red;
                        lblError.Text = "Could not update flat black hold status";
                    }
                }
            }


            if (e.CommandName == "Hold")
            {
                if ((bool)Session["MayRelease"] == true)
                {

                    if (db.UpdateCopyFlatHold(copyFlatSeparationSet, 1, 0, out errmsg) == false)
                    {
                        lblError.ForeColor = Color.Red;
                        lblError.Text = "Could not update flat hold status";
                    }
                }
            }

            if (e.CommandName == "Reimage")
            {
                if ((bool)Session["MayReimage"] == true)
                {
                    Session["SelectedCopyFlatSeparationSet"] = copyFlatSeparationSet;



                    Telerik.Web.UI.RadWindow mywindow = isPDF ? RadWindowManager1.Windows[1] : RadWindowManager1.Windows[0];

                    mywindow.VisibleOnPageLoad = true;
                }
            }

            ReBind(false);
        }

        private void FlatList_ItemDataBound(object sender, System.Web.UI.WebControls.DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HtmlTable table = (HtmlTable)e.Item.FindControl("TableFlats");
                Panel panel = (Panel)e.Item.FindControl("pnlHeader");
                Panel ftpanel = (Panel)e.Item.FindControl("pnlFooter");

                HtmlInputHidden hidden = (HtmlInputHidden)e.Item.FindControl("hiddenImageID");

                string scmd = hidden.Value;
                string[] sargs = scmd.Split('&');

                Panel panelBottom = (Panel)e.Item.FindControl("pnlBottom");
                Panel panelHeader = (Panel)e.Item.FindControl("pnlHeader");

                ImageButton imgReimage = (ImageButton)e.Item.FindControl("imgReimage");
                if (imgReimage != null)
                    imgReimage.ToolTip = Global.rm.GetString((bool)Application["LocationIsPress"] ? "txtRetransmitForm" : "txtReimageForm");

                ImageButton imgRelease = (ImageButton)e.Item.FindControl("imgRelease");
                if (imgRelease != null)
                    imgRelease.ToolTip = Global.rm.GetString("txtReleaseForm");

                ImageButton imgReleaseBlack = (ImageButton)e.Item.FindControl("imgReleaseBlack");
                if (imgReleaseBlack != null)
                    imgReleaseBlack.ToolTip = Global.rm.GetString("txtReleaseBlackForm");


                ImageButton imgHold = (ImageButton)e.Item.FindControl("imgHold");
                if (imgHold != null)
                    imgHold.ToolTip = Global.rm.GetString("txtHoldForm");


                ImageButton imgApprove = (ImageButton)e.Item.FindControl("imgApprove");
                ImageButton imgDisapprove = (ImageButton)e.Item.FindControl("imgDisapprove");
                ImageButton btnPrinter = (ImageButton)e.Item.FindControl("btnPrinter");

                // CopyFlat&MinStatus&Hold&Colors&Approved&Unique&version%logstatus
                // Ex: 123&30&C;M;Y;K&0&1&2&30

                string sFlatNumber = sargs[0];

                if (sFlatNumber == "0")
                {
                    // Filler image
                    if (table != null)
                        table.Style.Add("border", "none");
                    panelBottom.Visible = false;
                    panelHeader.Visible = false;

                    if (imgApprove != null)
                        imgApprove.Visible = false;
                    if (imgDisapprove != null)
                        imgDisapprove.Visible = false;
                    if (imgReimage != null)
                        imgReimage.Visible = false;
                    if (imgHold != null)
                        imgHold.Visible = false;
                    if (imgRelease != null)
                        imgRelease.Visible = false;
                    if (imgReleaseBlack != null)
                        imgReleaseBlack.Visible = false;
                    if (btnPrinter != null)
                        btnPrinter.Visible = false;

                }
                else
                {
                    int nstatus = Globals.TryParse(sargs[1], 0);
                    string holdState = sargs[2];
                    string colorString = sargs[3];
                    string uniqueString = sargs[4];
                    int version = 1;
                    if (sargs.Length > 5)
                        version = Globals.TryParse(sargs[5], 1);
                    int nextstatus = 30;
                    if (sargs.Length > 6)
                        nextstatus = Globals.TryParse(sargs[6], 30);

                    string[] sargscolors = colorString.Split(';');

                    Color statusColor = Globals.GetStatusColor(nstatus, 0);

                    if (imgApprove != null)
                        imgApprove.Visible = (bool)Session["MayApprove"] && (bool)Application["FlatViewShowApproveButton"];
                    if (imgDisapprove != null)
                        imgDisapprove.Visible = (bool)Session["MayApprove"] && (bool)Application["FlatViewShowApproveButton"];
                    if (imgReimage != null)
                        imgReimage.Visible = (bool)Session["MayReimage"];
                    if (imgHold != null)
                        imgHold.Visible = (bool)Session["MayRelease"];
                    if (imgRelease != null)
                        imgRelease.Visible = (bool)Session["MayRelease"];
                    if (imgReleaseBlack != null)
                        imgReleaseBlack.Visible = (bool)Session["MayRelease"] && version > 1;
                    if (btnPrinter != null)
                        btnPrinter.Visible = (bool)Application["AllowFlatproof"];

                    panelHeader.BackImageUrl = "";
                    panelBottom.BackImageUrl = "";
                    if (holdState == "0")
                    {
                        panelBottom.BackColor = Color.Lime;
                        //panelBottom.BackImageUrl = "../Images/greengradient2.gif";
                    }
                    else // 1
                    {
                        panelBottom.BackColor = Color.Orange;
                        //panelBottom.BackImageUrl = "../Images/orangegradient2.gif";
                    }

                    if (nstatus == 50)
                    {
                        panelHeader.BackColor = Color.Lime;
                        //panelHeader.BackImageUrl = "../Images/greengradient2.gif";
                        e.Item.BackColor = Color.Lime;
                        if ((bool)Application["FlatDependOnExtStatus"] && (int)Application["FlatDependOnExtStatusNumber"] != nextstatus)
                        {
                            panelHeader.BackColor = Color.Orange;
                          //  panelHeader.BackImageUrl = "../Images/orangegradient2.gif";
                            e.Item.BackColor = Color.Orange;
                        }
                    }

                    else if (nstatus > 0 && nstatus <= 30)
                    {
                        panelHeader.BackColor = Color.Yellow;
                      //  panelHeader.BackImageUrl = "../Images/yellowgradient2.gif";
                        e.Item.BackColor = Color.Yellow;
                    }
                    else if (nstatus > 30 && nstatus < 50)
                    {
                        panelHeader.BackColor = Color.Orange;
                        //panelHeader.BackImageUrl = "../Images/orangegradient2.gif";
                        e.Item.BackColor = Color.Orange;
                    }
                    else if (nstatus == 70)
                    {
                        panelHeader.BackColor = Color.Red;
                        //panelHeader.BackImageUrl = "../Images/redgradient2.gif";
                        e.Item.BackColor = Color.Red;

                    }
                    else
                    {
                        panelHeader.BackColor = Color.Gray;
                        //panelHeader.BackImageUrl = "../Images/graygradient2.gif";
                        e.Item.BackColor = Color.Gray;
                    }

                    if (uniqueString == "0")
                        e.Item.BackColor = Color.DeepSkyBlue;

                }
            }
        }
        
        private void FlatList_ItemCreated(object sender, System.Web.UI.WebControls.DataListItemEventArgs e)
		{
		
		}



        public ICollection CreateImageDataSource(DataTable dstable, int maxCopyNumber)
        {
           

            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            CCDBaccess db = new CCDBaccess();
            string errmsg = "";

            bool hideCommon = (bool)Session["HideCommon"];

            CleanTempFlatImages();

            int numberOfCopies = maxCopyNumber;

            DataTable dt = new DataTable();
            DataColumn newColumn;
            newColumn = dt.Columns.Add("ImageName", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ImageDesc", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ImageDescB", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ImageDescC", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ImageDesc2", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ImageNumber", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ImageInfo", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ImageQueryString", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ImageBorder", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("ImageWidth", Type.GetType("System.Int32"));

            String sPublication = (String)Session["SelectedPublication"];
            DateTime tPubDate = (DateTime)Session["SelectedPubDate"];
            string previewGUID = Globals.MakePreviewGUID(Globals.GetIDFromName("PublicationNameCache", sPublication), tPubDate);

            //            string NoPagePath = "../Images/NoPage.gif";
            //           string DummyPagePath = "../Images/Dummy.gif";
            //         string FillerPagePath = "../Images/Flats/filler.jpg";
            string NoPagePath = Request.MapPath(Request.ApplicationPath) + "/Images/NoPage.gif";
            string DummyPagePath = Request.MapPath(Request.ApplicationPath) + "/Images/Dummy.gif";
            string FillerPagePath = Request.MapPath(Request.ApplicationPath) + "/Images/Flats/filler.jpg";

            bool rotateFlat = false; // (bool)Application["RotateFlats"];

            DataView dv = dstable.DefaultView;

            string[] PageName = new string[64];
            int[] PageType = new int[64];
            int[] PageRotation = new int[64];
            int[] Pagination = new int[64];
            bool[] IsApproved = new bool[64];
            string[] PageSection = new string[64];
            System.Drawing.Image[] PageImage = new System.Drawing.Image[64];

            PAGEWIDTH = (int)Session["FlatPageSize"];
            if (PAGEWIDTH == 0)
                PAGEWIDTH = 120;

            PAGEWIDTHLANDSCAPE = PAGEWIDTH * 100;
            PAGEWIDTHLANDSCAPE /= 70;

            int nImageNumber = 0;
           
            int nMinStatus = 100;
            int nMinExtStatus = 100;
            bool bHasError = false;
            bool bIsHeld = false;
            bool bIsApproved = false;
            int nPagesAcross = 1;
            int nPagesDown = 1;
            bool bIsUnique = false;
            bool bIsPlateUnique = false;
            bool bIsPlateForced = false;

            int thisSheetNumber = 1;
            int thisSheetSide = 0;
            int thisCopyNumber = 1;
            int thisFlatSeparationSet = 1;
            int thisCopyFlatSeparationSet = 1;
            int thisStatus = 0;
            int thisFlatProofStatus = 0;

            string prevPlateName = "";
            string prevPublication = "";
            DateTime prevPubDate = new DateTime(1975, 1, 1, 0, 0, 0);
            string prevEdition = "";
            string prevSection = "";
            string prevIssue = "";

            int prevPlateNumber = -1;
            int prevCopyNumber = -1;
            int prevFlatSeparationSet = -1;
            int prevCopyFlatSeparationSet = -1;
            int prevStatus = 0;
            int prevFlatProofStatus = 0;

            int prevFlatRotation = 0;
            int prevVersion = 0;
            bool prevUnique = true;
            bool prevPlateUnique = true;
            bool prevPlateForced = false;
            int prevExtStatus = 30;

            bool hideSection = Globals.GetCacheRowCount("SectionNameCache") < 2;
            string sSection = (String)Session["SelectedSection"];
            string thisSection = "";
            int nFinalImageHeight = 1000;





            Font f = new Font("Verdana", 10);
            Font f2 = new Font("Verdana", 13, System.Drawing.FontStyle.Bold);
            StringFormat drawFormat = new StringFormat();
            drawFormat.Alignment = StringAlignment.Center;

            Pen framepen = new Pen(Color.LightGray);
            SolidBrush pagebrush = new SolidBrush(Color.LightBlue);
            SolidBrush graybrush = new SolidBrush(Color.Gray);
            SolidBrush whitebrush = new SolidBrush(Color.White);
            SolidBrush blackbrush = new SolidBrush(Color.Black);
            SolidBrush redbrush = new SolidBrush(Color.Red);
            Pen lightredpen = new Pen(Color.Tomato, 1);
            Pen whitepen = new Pen(Color.White, 1);
            Pen blackpen = new Pen(Color.Black, 1);
            Pen redpen = new Pen(Color.DarkRed, 1);
            Pen bluepen = new Pen(Color.Blue, 2);
            Pen redapprovepen = new Pen(Color.DarkRed, 3);
            Pen greenapprovepen = new Pen(Color.Green, 3);
            PointF point1 = new PointF();
            PointF point2 = new PointF();
            PointF point3 = new PointF();
            PointF pointf1 = new PointF();
            PointF pointf2 = new PointF();
            PointF pointf3 = new PointF();

            System.Drawing.Image StopImage = System.Drawing.Image.FromFile(Request.MapPath(Request.ApplicationPath) + "/Images/stop.gif");

            nPagesAcross = 0;
            nPagesDown = 0;

            foreach (DataRowView row in dv)
            {
                //int thisPagePosition =  (int)row["PagePosition"];

                if ((int)row["PagesAcross"] > nPagesAcross)
                    nPagesAcross = (int)row["PagesAcross"];
                if ((int)row["PagesDown"] > nPagesDown)
                    nPagesDown = (int)row["PagesDown"];
                if (nPagesAcross >= 1 && nPagesDown >= 1)
                    break;
            }

            if (nPagesAcross == 0)
                nPagesAcross = 1;

            if (nPagesDown == 0)
                nPagesDown = 1;


            PAGEWIDTH = (int)Session["FlatPageSize"] / nPagesAcross;
            if (PAGEWIDTH == 0)
                PAGEWIDTH = 120;

            PAGEWIDTHLANDSCAPE = PAGEWIDTH * 100;
            PAGEWIDTHLANDSCAPE /= 70;



            // Determine page sizes
            bool hasGUIDfile = false;
            foreach (DataRowView row in dv)
            {
                //int thisPagePosition =  (int)row["PagePosition"];
                DateTime tInputTime = db.GetInputTime((int)row["MasterCopySeparationSet"], out errmsg);

                if ((int)row["PagesAcross"] > nPagesAcross)
                    nPagesAcross = (int)row["PagesAcross"];
                if ((int)row["PagesDown"] > nPagesDown)
                    nPagesDown = (int)row["PagesDown"];

                string realPath = NoPagePath;
                if ((int)row["PageType"] == 3)
                    realPath = DummyPagePath;
                else
                {
                    string sMasterCopySeparationSet = row["MasterCopySeparationSet"].ToString();

                    realPath = Global.sRealThumbnailFolder + "\\" + sMasterCopySeparationSet + ".jpg";
                    string sFileToShowWithVersion = Global.sRealThumbnailFolder + "\\" + sMasterCopySeparationSet + "-" + row["PageVersion"].ToString() + ".jpg";
                    string sFileToShowWithInputTime = Global.sRealThumbnailFolder + "\\" + sMasterCopySeparationSet + "-" + Globals.DateTime2TimeStamp(tInputTime) + ".jpg";
                    Global.logging.WriteLog(sFileToShowWithInputTime);
                    if ((bool)Application["OldFileNames"])
                    {
                        if ((bool)Application["UseInputTimeInThumbnailName"] && System.IO.File.Exists(sFileToShowWithInputTime))
                            realPath = sFileToShowWithInputTime;
                        else
                            if ((bool)Application["UseVersionThumbnails"] && System.IO.File.Exists(sFileToShowWithVersion))
                                realPath = sFileToShowWithVersion;

                        if (System.IO.File.Exists(realPath) == false || (int)row["ProofStatus"] < 10)
                            realPath = NoPagePath;

                    }
                    else
                    {
                        string fileTitle = previewGUID + "====" + sMasterCopySeparationSet;
                        realPath = Global.sRealThumbnailFolder + "\\" + fileTitle + ".jpg";
                        sFileToShowWithVersion = Global.sRealThumbnailFolder + "\\" + fileTitle + "-" + row["PageVersion"].ToString() + ".jpg";

                        if ((bool)Application["UseVersionThumbnails"] && System.IO.File.Exists(sFileToShowWithVersion))
                            realPath = sFileToShowWithVersion;

                        if (System.IO.File.Exists(realPath) == false || (int)row["ProofStatus"] < 10)
                            realPath = NoPagePath;
                    }
                }

                System.Drawing.Image TestImage = System.Drawing.Image.FromFile(realPath);
                int nThisImageWidth = TestImage.Size.Width;
                int nThisImageHeight = TestImage.Size.Height;
                TestImage.Dispose();

                string s = (string)row["PageRotations"];
                string[] ss = s.Split(',');
                int rot = Convert.ToInt32(ss[0]);
                rot += (int)row["IncomingPageRotationEven"];
                if (rot > 3)
                    rot = 4 - rot;
                if (rot == 1 || rot == 3)
                {
                    int t = nThisImageWidth;
                    nThisImageWidth = nThisImageHeight;
                    nThisImageHeight = t;
                    PAGEWIDTH = PAGEWIDTHLANDSCAPE;
                }
                if (rotateFlat)
                {
                    int t = nThisImageWidth;
                    nThisImageWidth = nThisImageHeight;
                    nThisImageHeight = t;
                }
                if (nThisImageWidth == 0)
                    nThisImageWidth = PAGEWIDTH;

                double scale = (double)PAGEWIDTH / (double)nThisImageWidth;
                double fThisImageHeight = (int)((double)nThisImageHeight * scale);

                if ((int)fThisImageHeight < nFinalImageHeight)
                    nFinalImageHeight = (int)fThisImageHeight;

                if (realPath != NoPagePath )//&& (bool)Application["UseFlatThumbnails"] && (bool)Application["OnlyUseFlatThumbnails"])
                    break;
            }

            float fFlipSize = (float)(PAGEWIDTH / 5.0);
            int nPlateWidth = PAGEWIDTH + 2 * FRAMEWIDTH;
            int nPlateHeight = PAGEWIDTH + 2 * FRAMEWIDTH;
            int TotalSizeX = PAGEWIDTH + 2 * FRAMEWIDTH;
            int TotalSizeY = PAGEWIDTH + 2 * FRAMEWIDTH;
            for (int i = 0; i < 64; i++)
            {
                PageType[i] = 3;
                PageImage[i] = System.Drawing.Image.FromFile(DummyPagePath);
                IsApproved[i] = false;
            }
            int nColorsOnFlat = 0;

            string[] colorNames = new string[16];
            int[] copiesDone = new int[16];
            int copyAreaHeight = 0;
            for (int i = 0; i < 16; copiesDone[i++] = 0) ;

            
          //  Global.logging.WriteLog(string.Format("Platedrawing init"));

            // Determine flat page sizes (if available)
            double imageRatio = 0.5;  // > 1 if portrait      < 1 landscape

            bool multipleEdition = false;
            string edition = "";
            if (dv.Count > 0)
                edition = (string)dv[0]["Edition"];

            foreach (DataRowView row in dv)
            {
                if ((string)row["Edition"] != edition)
                {
                    multipleEdition = true;
                    break;
                }
            }

            string thisColorString = "";
            string thisPageNames = "";
            string thisSectionName = "";
            string thisPlatename = "";

            bool isSingleSectionPressRun = true;
            string defaultSection = "";
            int defaultPressRunID = 0;
            if (dv.Count > 0)
            {
                defaultSection = (string)dv[0].Row["Section"];
                defaultPressRunID = (int)dv[0].Row["PressRunID"];
            }

            foreach (DataRowView row in dv)
            {
                if (isSingleSectionPressRun && (int)row["PressRunID"] == defaultPressRunID && (string)row["Section"] != defaultSection)
                {
                    isSingleSectionPressRun = false;
                    break;
                }
            }

            // MAIN LOOP BEGIN

            foreach (DataRowView row in dv)
            {
                // Submit finished flat now?
                thisSheetNumber = (int)row["SheetNumber"];
                thisSheetSide = (int)row["SheetSide"];
                thisCopyNumber = (int)row["CopyNumber"];
                thisColorString = (string)row["Color"];
                thisPlatename = (string)row["PlateName"];

                int nPlateNumber = thisSheetNumber * 2 + thisSheetSide;

                // Time to draw existing plate (because current row is for net plate)
                if ((prevPlateNumber != nPlateNumber && prevPlateNumber >= 0 && maxCopyNumber == prevCopyNumber) /* || (maxCopyNumber>1 && thisCopyNumber != prevCopyNumber && prevCopyNumber >= 0) */)
                {
                    Global.logging.WriteLog(string.Format("Platenumber {0}", prevPlateNumber));

                    // Draw plate based on previous data
                    if (bIsUnique || bIsUnique == false && hideCommon == false)					
                    //if (bIsPlateUnique || (bIsUnique == false && hideCommon == false))
                    {
                        // Init the plate drawing
                        DataRow dr = dt.NewRow();

                        int imgHeight = nFinalImageHeight;
                        nPlateWidth = PAGEWIDTH * nPagesAcross + FRAMEWIDTH * 2;
                        nImageWidth = nPlateWidth;
                        nPlateHeight = imgHeight * nPagesDown + FRAMEWIDTH * 2 + copyAreaHeight;
                        nImagesPerRow = nScreenWidth / nPlateWidth;
                        if (nImagesPerRow == 0)
                            nImagesPerRow = 1;

                        dr["ImageName"] = prevStatus == 0 ? NoPagePath : DummyPagePath;
                        dr["ImageWidth"] = nImageWidth;

                        if (bHasError)
                            nMinStatus = 70;

                        string imgdesc = prevPlateNumber / 2 + (prevPlateNumber % 2 == 0 ? " " + Global.rm.GetString("txtFrontSide") : " " + Global.rm.GetString("txtBackSide"));
                        dr["ImageDesc"] = imgdesc;


                        // Reverse page sequence?
                        if (((prevFlatRotation & 1) > 0 && prevPlateNumber % 2 == 0) || ((prevFlatRotation & 2) > 0 && prevPlateNumber % 2 == 1))
                        {
                            string[] pages = thisPageNames.Split(',');
                            string thisPageNames2 = "";

                            if (pages.Length > 0)
                            {
                                for (int i = pages.Length - 1; i >= 0; i--)
                                {
                                    if (thisPageNames2 != "")
                                        thisPageNames2 += ",";
                                    thisPageNames2 += pages[i];
                                }
                            }
                            thisPageNames = thisPageNames2;
                        }
                        string pageHeader = thisPageNames.Replace(",", "  ");

                        string[] sections = thisSectionName.Split(',');
                        string sectionHeader = sections[0];
                        bool hideSection3 = Globals.GetCacheRowCount("SectionNameCache") < 2;
                        if (hideSection3)
                            sectionHeader = "";

                        if (sections.Length > 1 && hideSection3 == false)
                        {
                            bool differentSectionNames = false;
                            foreach (string s in sections)
                            {
                                if (s != sections[0])
                                {
                                    differentSectionNames = true;
                                    break;
                                }
                            }
                            if (differentSectionNames)
                            {
                                string[] pages = thisPageNames.Split(' ');
                                pageHeader = "";
                                if (pages.Length > 0)
                                {
                                    for (int i = 0; i < pages.Length; i++)
                                    {
                                        if (pageHeader != "")
                                            pageHeader += " ";
                                        pageHeader += sections[i] + pages[i];
                                    }
                                }
                                sectionHeader = "";
                            }
                        }


                        Bitmap flatbitmap = new Bitmap(nPlateWidth, nPlateHeight, PixelFormat.Format32bppArgb);
                        Graphics g = Graphics.FromImage(flatbitmap);
                        if (bHasError)
                            nMinStatus = 70;

                        SolidBrush platebrush = new SolidBrush(bHasError ? Color.Red : (bIsUnique ? Globals.GetStatusColor(nMinStatus, 0) : Color.LightBlue));
                        g.FillRectangle(platebrush, 0, 0, nPlateWidth, nPlateHeight);
                        int nAntiPanoPos = 0;


                        bool hasFlat = false;
                        if ((bool)Application["UseFlatThumbnails"] == true)
                        {
                            // Draw the flat image
                            string flatRealPath = Global.sRealFlatThumbnailFolder + "\\" + prevCopyFlatSeparationSet.ToString() + ".jpg";

                            // FlatPreview
                            string flatRealPath2 = Global.sRealFlatImageFolder + "\\" + prevCopyFlatSeparationSet.ToString() + ".jpg";

                            hasFlat = System.IO.File.Exists(flatRealPath);
                            bool hasFlat2 = System.IO.File.Exists(flatRealPath2);

                            bool XMLOK = Globals.CheckTileXML(Global.sRealFlatImageFolder + "\\" + prevCopyFlatSeparationSet.ToString() + "\\", (string)Session["SelectedPublication"], (DateTime)Session["SelectedPubDate"]);

                            if (XMLOK == false || hasFlat == false || hasFlat2 == false || (prevFlatProofStatus < 10 && (bool)Application["CheckFlatProofStatus"])/* || prevStatus <=30  ||*/)
                            {
                                flatRealPath = DummyPagePath;
                                hasFlat = false;
                            }

                            System.Drawing.Image FlatImage = System.Drawing.Image.FromFile(flatRealPath);

                            Rectangle rSrcFlat = new Rectangle(0, 0, FlatImage.Size.Width, FlatImage.Size.Height);
                            //double ratioflat = (double)FlatImage.Size.Width/(double)FlatImage.Size.Height;
                            Rectangle rDstFlat = new Rectangle(FRAMEWIDTH, FRAMEWIDTH, nPlateWidth - 2 * FRAMEWIDTH, (int)nPlateHeight - 2 * FRAMEWIDTH);

                            g.DrawImage(FlatImage, rDstFlat, rSrcFlat, System.Drawing.GraphicsUnit.Pixel);

                            for (int p = 0; p < nPagesAcross * nPagesDown; p++)
                            {
                                int ypos = p / nPagesAcross;
                                int xpos = p - nPagesAcross * ypos;
                                Rectangle rDstTxt = new Rectangle(xpos * PAGEWIDTH + FRAMEWIDTH, ypos * imgHeight + FRAMEWIDTH, PAGEWIDTH, (int)imgHeight);

                                string sPageName = PageName[p];
                                string sSec = PageSection[p];
                                if (sSec != "" && hideSection == false)
                                    sPageName = sSec + " " + sPageName;

                                //g.FillRectangle(whitebrush,rDst);
                                g.DrawString(sPageName, f2, blackbrush, rDstTxt, drawFormat);
                            }

                        }

                        if (hasFlat == false && (bool)Application["OnlyUseFlatThumbnails"] == false)
                        {

                            for (int p = 0; p < nPagesAcross * nPagesDown; p++)
                            {
                                if (PageType[p] == 2)
                                    nAntiPanoPos = p;

                                if (PageType[p] != 2) // Skip 'anti-panorama' pages
                                {
                                    // Determine scaling
                                    int ypos = p / nPagesAcross;
                                    int xpos = p - nPagesAcross * ypos;

                                    if (p > 0)
                                    {
                                        if (PageRotation[p] == 2 && PageType[p] == 1 && PageType[p - 1] == 2)
                                            xpos--;

                                        if ((PageRotation[p] == 1 || PageRotation[p] == 3) && PageType[p] == 1 && nAntiPanoPos > 0)
                                            ypos--;
                                    }
                                    Rectangle rDst = new Rectangle(xpos * PAGEWIDTH + FRAMEWIDTH, ypos * imgHeight + FRAMEWIDTH, PAGEWIDTH, (int)imgHeight);

                                    // Make room for panorama page
                                    if (PageType[p] == 1)
                                    {
                                        if (PageRotation[p] == 0 || PageRotation[p] == 2)
                                            rDst.Width *= 2;
                                        else
                                            rDst.Height *= 2;
                                    }


                                    // Draw the page image
                                    Rectangle rSrc = new Rectangle(0, 0, PageImage[p].Size.Width, PageImage[p].Size.Height);

                                    g.DrawImage(PageImage[p], rDst, rSrc, System.Drawing.GraphicsUnit.Pixel);

                                    g.DrawRectangle(IsApproved[p] && PageType[p] != 3 ? greenapprovepen : framepen, rDst);

                                    if (PageType[p] != 3)
                                    {
                                        // Draw 'flips' on pages
                                        int rot = PageRotation[p];
                                        if (Pagination[p] % 2 == 1)
                                            rot += 1;
                                        if (rot == 4)
                                            rot = 0;
                                        switch (rot)
                                        {
                                            case 0:
                                                point1.X = rDst.X; point1.Y = rDst.Y;
                                                point2.X = rDst.X + fFlipSize; point2.Y = rDst.Y;
                                                point3.X = rDst.X; point3.Y = rDst.Y + fFlipSize;

                                                pointf1 = point2;
                                                pointf2.X = rDst.X + fFlipSize; pointf2.Y = rDst.Y + fFlipSize;
                                                pointf3 = point3;
                                                break;
                                            case 1:
                                                point1.X = rDst.X + rDst.Width; point1.Y = rDst.Y;
                                                point2.X = rDst.X + rDst.Width; point2.Y = rDst.Y + fFlipSize;
                                                point3.X = rDst.X + rDst.Width - fFlipSize; point3.Y = rDst.Y;

                                                pointf1 = point2;
                                                pointf2.X = rDst.X + rDst.Width - fFlipSize; pointf2.Y = rDst.Y + fFlipSize;
                                                pointf3 = point3;
                                                break;
                                            case 2:
                                                point1.X = rDst.X + rDst.Width; point1.Y = rDst.Y + rDst.Height;
                                                point2.X = rDst.X + rDst.Width - fFlipSize; point2.Y = rDst.Y + rDst.Height;
                                                point3.X = rDst.X + rDst.Width; point3.Y = rDst.Y + rDst.Height - fFlipSize;

                                                pointf1 = point2;
                                                pointf2.X = rDst.X + rDst.Width - fFlipSize; pointf2.Y = rDst.Y + rDst.Height - fFlipSize;
                                                pointf3 = point3;
                                                break;
                                            case 3:
                                                point1.X = rDst.X; point1.Y = rDst.Y + rDst.Height;
                                                point2.X = rDst.X; point2.Y = rDst.Y + rDst.Height - fFlipSize;
                                                point3.X = rDst.X + fFlipSize; point3.Y = rDst.Y + rDst.Height;

                                                pointf1 = point2;
                                                pointf2.X = rDst.X + fFlipSize; pointf2.Y = rDst.Y + rDst.Height - fFlipSize;
                                                pointf3 = point3;
                                                break;
                                        }
                                        PointF[] curvePoints = { point1, point2, point3 };
                                        PointF[] curvePointsf = { pointf1, pointf2, pointf3 };
                                        g.FillPolygon(platebrush, curvePoints);
                                        g.FillPolygon(graybrush, curvePointsf);

                                        //g.DrawString(PageName[p],f2, blackbrush,(float)(xpos*PAGEWIDTH + FRAMEWIDTH+PAGEWIDTH/2), (float)(ypos*imgHeight + FRAMEWIDTH+imgHeight/2), drawFormat);
                                        string sPageName = PageName[p];
                                        string sSec = PageSection[p];
                                        if (sSec != "" && hideSection == false)
                                            sPageName = sSec + " " + sPageName;

                                        //g.FillRectangle(whitebrush,rDst);
                                        g.DrawString(sPageName, f2, blackbrush, rDst, drawFormat);

                                    }

                                }
                            }
                        }

                         if (bIsHeld)
                            g.DrawImage(StopImage, nPlateWidth / 2 - 16, nPlateHeight / 2 - 16, 32, 32);

                        string timestr = string.Format("{0:0000}{1:00}{2:00}{3:00}{4:00}{5:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                        dr["ImageName"] = "../Images/Flats/" + (string)Session["UserName"] + "_" + prevCopyFlatSeparationSet.ToString() + "_" + timestr + ".jpg";
                        try
                        {
                            if (rotateFlat)
                                flatbitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);

                            flatbitmap.Save(Request.MapPath(Request.ApplicationPath) + "/Images/Flats/" + (string)Session["UserName"] + "_" + prevCopyFlatSeparationSet.ToString() + "_" + timestr + ".jpg", ImageFormat.Jpeg);
                        }
                        catch (Exception ee)
                        {
                            lblError.Text = ee.Message;
                            lblError.ForeColor = Color.Red;
                        }

                        flatbitmap.Dispose();
                        g.Dispose();
                        
                        dr["ImageNumber"] = prevCopyFlatSeparationSet.ToString() + "&" + nMinStatus.ToString();

                        dr["ImageQueryString"] = "set=" + prevCopyFlatSeparationSet.ToString() + "&ver=" + prevVersion.ToString();
                        if ((bool)Application["FlatViewShowSheetNumber"])
                        {
                            dr["ImageDesc"] = imgdesc;
                            dr["ImageDescB"] = pageHeader;
                        }
                        else
                        {
                            if (multipleEdition)
                                dr["ImageDesc"] = (string)row["Edition"] + " " + sectionHeader;
                            else
                                dr["ImageDesc"] = sectionHeader;
                            dr["ImageDescB"] = pageHeader;
                        }
                        if ((bool)Application["FlatViewShowPlateNumber"])
                        {
                            dr["ImageDesc"] = "[" + prevPlateName + "]";
                            dr["ImageDescB"] = pageHeader;
                        }

                        string statusName = Globals.GetStatusName(prevStatus, 0);
                        if (statusName == "Transmitted")
                            statusName = Global.rm.GetString("txtTransmitted");
                        else if (statusName == "Transmitting")
                            statusName = Global.rm.GetString("txtTransmitting");
                        else if (statusName == "Missing")
                            statusName = Global.rm.GetString("txtMissing");
                        else if (statusName == "Ready")
                            statusName = Global.rm.GetString("txtReady");
                        else if (statusName == "Paired")
                            statusName = Global.rm.GetString("txtPaired");
                        else if (statusName == "Imaged")
                            statusName = (int)Application["FlatDependOnExtStatusNumber"] == prevExtStatus ? Global.rm.GetString("txtBend") : Global.rm.GetString("txtOutput");

                        dr["ImageDescC"] = prevStatus == 70 ? Global.rm.GetString("txtError") : statusName;

                        dr["ImageDesc2"] = bIsHeld ? Global.rm.GetString("txtOnHold") : Global.rm.GetString("txtReleased");


                        if (bIsHeld)
                            dr["ImageNumber"] = prevCopyFlatSeparationSet.ToString() + "&" + prevStatus.ToString() + "&1";
                        else
                            dr["ImageNumber"] = prevCopyFlatSeparationSet.ToString() + "&" + prevStatus.ToString() + "&0";

                        //if (prevUnique)
                        if (prevPlateUnique)
                            dr["ImageInfo"] = (string)dr["ImageNumber"] + "&" + thisColorString + "&1&" + prevVersion.ToString() + "&" + prevExtStatus.ToString();
                        else if (prevPlateForced)
                            dr["ImageInfo"] = (string)dr["ImageNumber"] + "&" + thisColorString + "&2&" + prevVersion.ToString() + "&" + prevExtStatus.ToString();
                        else
                            dr["ImageInfo"] = (string)dr["ImageNumber"] + "&" + thisColorString + "&0&" + prevVersion.ToString() + "&" + prevExtStatus.ToString();
                        
                        nImageNumber++;
                        dt.Rows.Add(dr);

 
                        // Reset flags for next plate
                        bHasError = false;
                        bIsHeld = false;
                        bIsUnique = false;
                        bIsPlateUnique = false;
                        bIsPlateForced = false;
                        bIsApproved = true;

                        nMinStatus = 100;
                        thisPageNames = "";
                        thisSectionName = "";
                        thisColorString = "";

					    for (int i=0; i<64; i++)
					    {
						    PageType[i] = 3;
						    PageImage[i] = System.Drawing.Image.FromFile(DummyPagePath);
						    IsApproved[i] = false;
					    }
					    nColorsOnFlat = 0;
					    for(int i=0;i<16;copiesDone[i++] = 0);


                    }
                }

                #region Plate 'linefeeds'

                DateTime thisPubDate = (DateTime)row["PubDate"];
                if (thisPubDate != prevPubDate && prevPubDate.Year > 2000)
                {
                    // New product coming next! - make sure new product starts on fresh line - fill old line with dummies
                    int nDummyImages = nImagesPerRow - nImageNumber % nImagesPerRow;
                    if (nDummyImages != nImagesPerRow)
                    {
                        for (int im = 0; im < nDummyImages; im++)
                        {
                            DataRow drfill = dt.NewRow();
                            Bitmap flatbitmap = new Bitmap(10, 10, PixelFormat.Format32bppArgb);
                            Graphics g = Graphics.FromImage(flatbitmap);
                            drfill["ImageName"] = "../Images/Spacer.jpg";
                            try
                            {
                                if (rotateFlat)
                                    flatbitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                flatbitmap.Save(FillerPagePath, ImageFormat.Jpeg);
                            }
                            catch (Exception ee)
                            {
                                lblError.Text = ee.Message;
                                lblError.ForeColor = Color.Red;
                            }
                            flatbitmap.Dispose();
                            g.Dispose();

                            drfill["ImageDesc"] = "";
                            drfill["ImageDescB"] = "";
                            drfill["ImageNumber"] = "0&0&0";
                            drfill["ImageInfo"] = "0&0&0&0&0";
                            drfill["ImageWidth"] = nImageWidth;
                            drfill["ImageQueryString"] = "0";
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
                    // New product coming next! - make sure new product starts on fresh line - fill old line with dummies
                    int nDummyImages = nImagesPerRow - nImageNumber % nImagesPerRow;
                    if (nDummyImages != nImagesPerRow)
                    {
                        for (int im = 0; im < nDummyImages; im++)
                        {
                            DataRow drfill = dt.NewRow();
                            Bitmap flatbitmap = new Bitmap(10, 10, PixelFormat.Format32bppArgb);
                            Graphics g = Graphics.FromImage(flatbitmap);
                            drfill["ImageName"] = "../Images/Spacer.jpg";
                            try
                            {
                                if (rotateFlat)
                                    flatbitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                flatbitmap.Save(FillerPagePath, ImageFormat.Jpeg);
                            }
                            catch (Exception ee)
                            {
                                lblError.Text = ee.Message;
                                lblError.ForeColor = Color.Red;
                            }
                            flatbitmap.Dispose();
                            g.Dispose();

                            drfill["ImageDesc"] = "";
                            drfill["ImageDescB"] = "";
                            drfill["ImageNumber"] = "0&0&0";
                            drfill["ImageInfo"] = "0&0&0&0&0";
                            drfill["ImageWidth"] = nImageWidth;
                            drfill["ImageQueryString"] = "0";
                            drfill["ImageBorder"] = " BORDER-RIGHT: white 4px solid; BORDER-TOP: white 4px solid; BORDER-LEFT: white 4px solid; BORDER-BOTTOM: white 4px solid;";

                            nImageNumber++;
                            dt.Rows.Add(drfill);
                        }
                    }

                }
                prevPublication = thisPublication;

                string thisIssue = (string)row["Issue"];

                prevIssue = thisIssue;

                string thisEdition = (string)row["Edition"];
                if (thisEdition != prevEdition && prevEdition != "")
                {
                    int nDummyImages = nImagesPerRow - nImageNumber % nImagesPerRow;
                    if (nDummyImages != nImagesPerRow)
                    {
                        for (int im = 0; im < nDummyImages; im++)
                        {
                            DataRow drfill = dt.NewRow();
                            drfill["ImageDesc"] = "";
                            drfill["ImageDescB"] = "";

                            Bitmap flatbitmap = new Bitmap(10, 10, PixelFormat.Format32bppArgb);
                            Graphics g = Graphics.FromImage(flatbitmap);
                            drfill["ImageName"] = "../Images/Spacer.jpg";
                            try
                            {
                                if (rotateFlat)
                                    flatbitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                flatbitmap.Save(FillerPagePath, ImageFormat.Jpeg);
                            }
                            catch (Exception ee)
                            {
                                lblError.Text = ee.Message;
                                lblError.ForeColor = Color.Red;
                            }
                            flatbitmap.Dispose();
                            g.Dispose();

                            drfill["ImageNumber"] = "0&0&0";
                            drfill["ImageInfo"] = "0&0&0&0&0";
                            drfill["ImageWidth"] = nImageWidth;
                            drfill["ImageQueryString"] = "0";
                            drfill["ImageBorder"] = " BORDER-RIGHT: white 4px solid; BORDER-TOP: white 4px solid; BORDER-LEFT: white 4px solid; BORDER-BOTTOM: white 4px solid;";

                            nImageNumber++;
                            dt.Rows.Add(drfill);
                        }
                    }

                }
                prevEdition = thisEdition;

                thisSection = (string)row["Section"];
                if (isSingleSectionPressRun && thisSection != prevSection && prevSection != "")
                {
                    int nDummyImages = nImagesPerRow - nImageNumber % nImagesPerRow;
                    if (nDummyImages != nImagesPerRow)
                    {
                        for (int im = 0; im < nDummyImages; im++)
                        {
                            DataRow drfill = dt.NewRow();

                            Bitmap flatbitmap = new Bitmap(10, 10, PixelFormat.Format32bppArgb);
                            Graphics g = Graphics.FromImage(flatbitmap);
                            drfill["ImageName"] = "../Images/Spacer.jpg";
                            try
                            {
                                if (rotateFlat)
                                    flatbitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                flatbitmap.Save(FillerPagePath, ImageFormat.Jpeg);
                            }
                            catch (Exception ee)
                            {
                                lblError.Text = ee.Message;
                                lblError.ForeColor = Color.Red;
                            }
                            flatbitmap.Dispose();
                            g.Dispose();


                            drfill["ImageDesc"] = "";
                            drfill["ImageDescB"] = "";
                            drfill["ImageNumber"] = "0&0&0";
                            drfill["ImageInfo"] = "0&0&0&0&0";
                            drfill["ImageWidth"] = nImageWidth;
                            drfill["ImageQueryString"] = "0";
                            drfill["ImageBorder"] = " BORDER-RIGHT: white 4px solid; BORDER-TOP: white 4px solid; BORDER-LEFT: white 4px solid; BORDER-BOTTOM: white 4px solid;";

                            nImageNumber++;
                            dt.Rows.Add(drfill);
                        }
                    }

                }

                prevSection = thisSection;

                #endregion

                nPagesAcross = (int)row["PagesAcross"];
                if (nPagesAcross == 0)
                    nPagesAcross = 1;
                nPagesDown = (int)row["PagesDown"];
                if (nPagesDown == 0)
                    nPagesDown = 1;


                thisFlatSeparationSet = (int)row["FlatSeparationSet"];
                thisCopyFlatSeparationSet = (int)row["CopyFlatSeparationSet"];
                thisFlatProofStatus = (int)row["FlatProofStatus"];

                string[] colors = thisColorString.Split(';');
                nColorsOnFlat = colors.Length;

                if (prevFlatSeparationSet != thisFlatSeparationSet)
                {
                    // First page of flat..
                    for (int col = 0; col < colors.Length; col++)
                        colorNames[col] = colors[col];
                }	


                int thisPageType = (int)row["PageType"];

                int nStatus = Globals.GetStatusID((string)row["Status"], 0);
                if (nStatus < nMinStatus && thisPageType < 2)
                    nMinStatus = nStatus;
                if (nStatus == 70)
                    bHasError = true;

                int nExtStatus = (int)row["ExtStatus"];
                if (nExtStatus < nMinExtStatus && thisPageType < 2)
                    nMinExtStatus = nExtStatus;

                if ((int)row["Hold"] > 0)
                    bIsHeld = true;

                if ((int)row["UniquePage"] > 0)
                    bIsUnique = true;

                if ((int)row["UniquePlate"] > 0)
                    bIsPlateUnique = true;

                if ((int)row["ForcedPlate"] > 0 || bIsPlateUnique)
                    bIsPlateForced = true;

                if ((int)row["Approval"] == 0 || (int)row["Approval"] == 2)
                    bIsApproved = false;

                string s1 = (string)row["PagePositions"];
                string ss = (string)row["PageRotations"];
                string[] positionarray = s1.Split(',');
                string[] rotationarray = ss.Split(',');
                
                if (thisCopyNumber == 1)
                {
                    if (thisPageNames != "")
                        thisPageNames += ",";
                    thisPageNames += (string)row["Page"];

                    if (thisSectionName != "")
                        thisSectionName += ",";
                    thisSectionName += (string)row["Section"];
                }


                int mm = positionarray.Length;
                for (int q = 0; q < mm; q++)
                {
                    int thisPagePosition = Convert.ToInt32(positionarray[q]);

                    PageType[thisPagePosition - 1] = thisPageType;
                    PageRotation[thisPagePosition - 1] = Convert.ToInt32(rotationarray[q]) + (int)row["IncomingPageRotationEven"];
                    if (PageRotation[thisPagePosition - 1] > 3)
                        PageRotation[thisPagePosition - 1] = 4 - PageRotation[thisPagePosition - 1];

                    PageName[thisPagePosition - 1] = (string)row["Page"];
                    PageSection[thisPagePosition - 1] = (string)row["Section"];
                    Pagination[thisPagePosition - 1] = (int)row["Pagination"];

                    if ((int)row["UniquePage"] > 0)
                        bIsUnique = true;

                    IsApproved[thisPagePosition - 1] = (int)row["Approval"] > 0 ? true : false; ;
                    string realPath = "";

                    if (PageType[thisPagePosition - 1] == 3)
                        realPath = DummyPagePath;
                    else
                    {


                        string sMasterCopySeparationSet = row["MasterCopySeparationSet"].ToString();

                        realPath = Global.sRealThumbnailFolder + "\\" + sMasterCopySeparationSet + ".jpg";
                        string sFileToShowWithVersion = Global.sRealThumbnailFolder + "\\" + sMasterCopySeparationSet + "-" + row["PageVersion"].ToString() + ".jpg";
                        DateTime tInputTime = db.GetInputTime((int)row["MasterCopySeparationSet"], out errmsg);
                        string sFileToShowWithInputTime = Global.sRealThumbnailFolder + "\\" + sMasterCopySeparationSet + "-" + Globals.DateTime2TimeStamp(tInputTime) + ".jpg";

                        if ((bool)Application["OldFileNames"])
                        {

                            if ((bool)Application["UseInputTimeInThumbnailName"] && System.IO.File.Exists(sFileToShowWithInputTime))
                                realPath = sFileToShowWithInputTime;
                            else
                            {
                                if ((bool)Application["UseVersionThumbnails"] && System.IO.File.Exists(sFileToShowWithVersion))
                                    realPath = sFileToShowWithVersion;
                             }
                            if (System.IO.File.Exists(realPath) == false || (int)row["ProofStatus"] < 10)
                                realPath = NoPagePath;

                        }
                        else
                        {
                            string fileTitle = previewGUID + "====" + sMasterCopySeparationSet;
                            realPath = Global.sRealThumbnailFolder + "\\" + fileTitle + ".jpg";
                            sFileToShowWithVersion = Global.sRealThumbnailFolder + "\\" + fileTitle + "-" + row["PageVersion"].ToString() + ".jpg";

                            if ((bool)Application["UseVersionThumbnails"] && System.IO.File.Exists(sFileToShowWithVersion))
                                realPath = sFileToShowWithVersion;

                            if (System.IO.File.Exists(realPath) == false || (int)row["ProofStatus"] < 10)
                                realPath = NoPagePath;

                        }



                        
                    }
                    PageImage[thisPagePosition - 1] = System.Drawing.Image.FromFile(realPath);

                    if (PageRotation[thisPagePosition - 1] == 1)
                        PageImage[thisPagePosition - 1].RotateFlip(RotateFlipType.Rotate90FlipNone);
                    else if (PageRotation[thisPagePosition - 1] == 2)
                        PageImage[thisPagePosition - 1].RotateFlip(RotateFlipType.Rotate180FlipNone);
                    else if (PageRotation[thisPagePosition - 1] == 3)
                        PageImage[thisPagePosition - 1].RotateFlip(RotateFlipType.Rotate270FlipNone);
                }

                prevPlateName = (string)row["PlateName"];
                prevPlateNumber = thisSheetNumber * 2 + thisSheetSide;
                prevFlatSeparationSet = thisFlatSeparationSet;
                prevCopyFlatSeparationSet = thisCopyFlatSeparationSet;
                prevCopyNumber = thisCopyNumber;
                prevStatus = nMinStatus;
                prevFlatProofStatus = thisFlatProofStatus;
                prevFlatRotation = (int)row["FlatRotation"];
                prevUnique = (int)row["UniquePage"] > 0;
                prevPlateUnique = (int)row["UniquePlate"] > 0;
                prevPlateForced = (int)row["ForcedPlate"] > 0 || prevPlateUnique;
                prevVersion = (int)row["Version"];
                prevExtStatus = nMinExtStatus;

                
            }

            // Don't forget last flat..
            if (prevPlateNumber != -1)
            {
                //if (bIsPlateUnique || (bIsPlateUnique == false && hideCommon == false))
                if (bIsUnique || bIsUnique == false && hideCommon == false)
                {
                    DataRow dr = dt.NewRow();

                    int imgHeight = nFinalImageHeight;
                    nPlateWidth = PAGEWIDTH * nPagesAcross + FRAMEWIDTH * 2;
                    nImageWidth = nPlateWidth;

                    nPlateHeight = imgHeight * nPagesDown + FRAMEWIDTH * 2 + copyAreaHeight;

                    nImagesPerRow = nScreenWidth / nPlateWidth;
                    if (nImagesPerRow == 0)
                        nImagesPerRow = 1;
                    string imgdesc = prevPlateNumber / 2 + (prevPlateNumber % 2 == 0 ? " " + Global.rm.GetString("txtFrontSide") : " " + Global.rm.GetString("txtBackSide"));
                    dr["ImageDesc"] = imgdesc;

                    dr["ImageName"] = prevStatus == 0 ? NoPagePath : DummyPagePath;
                    dr["ImageWidth"] = nImageWidth;


                    Bitmap flatbitmap = new Bitmap(nPlateWidth, nPlateHeight, PixelFormat.Format32bppArgb);
                    Graphics g = Graphics.FromImage(flatbitmap);

                    if (bHasError)
                        nMinStatus = 70;

                    SolidBrush platebrush = new SolidBrush(bHasError ? Color.Red : (bIsUnique ? Globals.GetStatusColor(nMinStatus, 0) : Color.LightBlue));
                    g.FillRectangle(platebrush, 0, 0, nPlateWidth, nPlateHeight);

                    bool hasFlat = false;
                    if ((bool)Application["UseFlatThumbnails"] == true)
                    {
                        // Draw the flat image

                        string flatRealPath = Global.sRealFlatThumbnailFolder + "\\" + prevCopyFlatSeparationSet.ToString() + ".jpg";
                        // FlatPreview
                        string flatRealPath2 = Global.sRealFlatImageFolder + "\\" + prevCopyFlatSeparationSet.ToString() + ".jpg";
                        bool XMLOK = Globals.CheckTileXML(Global.sRealFlatImageFolder + "\\" + prevCopyFlatSeparationSet.ToString() + "\\", (string)Session["SelectedPublication"], (DateTime)Session["SelectedPubDate"]);

                        hasFlat = System.IO.File.Exists(flatRealPath);
                        bool hasFlat2 = System.IO.File.Exists(flatRealPath2);
                        if (XMLOK == false || hasFlat == false || hasFlat2 == false || (prevFlatProofStatus < 10 && (bool)Application["CheckFlatProofStatus"]) /*|| prevStatus <= 30 */)
                        {
                            flatRealPath = DummyPagePath;
                            hasFlat = false;
                        }

                        System.Drawing.Image FlatImage = System.Drawing.Image.FromFile(flatRealPath);

                        Rectangle rSrcFlat = new Rectangle(0, 0, FlatImage.Size.Width, FlatImage.Size.Height);
                        //double ratioflat = (double)FlatImage.Size.Width/(double)FlatImage.Size.Height;
                        Rectangle rDstFlat = new Rectangle(FRAMEWIDTH, FRAMEWIDTH, nPlateWidth - 2 * FRAMEWIDTH, (int)nPlateHeight - 2 * FRAMEWIDTH);

                        g.DrawImage(FlatImage, rDstFlat, rSrcFlat, System.Drawing.GraphicsUnit.Pixel);

                        for (int p = 0; p < nPagesAcross * nPagesDown; p++)
                        {
                            int ypos = p / nPagesAcross;
                            int xpos = p - nPagesAcross * ypos;
                            Rectangle rDstTxt = new Rectangle(xpos * PAGEWIDTH + FRAMEWIDTH, ypos * imgHeight + FRAMEWIDTH, PAGEWIDTH, (int)imgHeight);

                            string sPageName = PageName[p];
                            string sSec = PageSection[p];
                            if (sSec != "" && hideSection == false)
                                sPageName = sSec + " " + sPageName;

                            //g.FillRectangle(whitebrush,rDst);
                            g.DrawString(sPageName, f2, blackbrush, rDstTxt, drawFormat);
                        }

                    }
                    if (hasFlat == false && (bool)Application["OnlyUseFlatThumbnails"] == false)
                    {
                        for (int p = 0; p < nPagesAcross * nPagesDown; p++)
                        {

                            if (PageType[p] != 2) // Skip 'anti-panorama' pages
                            {
                                // Determine scaling
                                int ypos = p / nPagesAcross;
                                int xpos = p - nPagesAcross * ypos;

                                if (p > 0)
                                    if (PageRotation[p] == 2 && PageType[p] == 1 && PageType[p - 1] == 2)
                                        xpos--;

                                Rectangle rDst = new Rectangle(xpos * PAGEWIDTH + FRAMEWIDTH, ypos * imgHeight + FRAMEWIDTH,
                                    PAGEWIDTH, (int)imgHeight);

                                // Make room for panorama page
                                if (PageType[p] == 1)
                                {
                                    if (PageRotation[p] == 0 || PageRotation[p] == 2)
                                        rDst.Width *= 2;
                                    else
                                        rDst.Height *= 2;

                                    /*	if (p>0)
                                            if (PageRotation[p] == 2  && PageType[p] == 1 && PageType[p-1] == 2)
                                                rDst.X -= PAGEWIDTH;

                                        if ( p<nPagesAcross*nPagesDown-1)
                                            if (PageRotation[p] == 2 && PageType[p] == 1 && PageType[p+1] == 2)
                                                rDst.X += PAGEWIDTH;
							

                                        if (PageRotation[p] == 3)
                                            rDst.Y -= imgHeight;
                                    */
                                }

                                // Draw the page image
                                Rectangle rSrc = new Rectangle(0, 0, PageImage[p].Size.Width, PageImage[p].Size.Height);
                                g.DrawImage(PageImage[p], rDst, rSrc, System.Drawing.GraphicsUnit.Pixel);

                                g.DrawRectangle(IsApproved[p] && PageType[p] != 3 ? greenapprovepen : framepen, rDst);

                                // Draw 'flips' on pages
                                if (PageType[p] != 3)
                                {
                                    int rot = PageRotation[p];
                                    if (Pagination[p] % 2 == 1)
                                        rot += 1;
                                    if (rot == 4)
                                        rot = 0;
                                    switch (rot)
                                    {
                                        case 0:
                                            point1.X = rDst.X; point1.Y = rDst.Y;
                                            point2.X = rDst.X + fFlipSize; point2.Y = rDst.Y;
                                            point3.X = rDst.X; point3.Y = rDst.Y + fFlipSize;

                                            pointf1 = point2;
                                            pointf2.X = rDst.X + fFlipSize; pointf2.Y = rDst.Y + fFlipSize;
                                            pointf3 = point3;
                                            break;
                                        case 1:
                                            point1.X = rDst.X + rDst.Width; point1.Y = rDst.Y;
                                            point2.X = rDst.X + rDst.Width; point2.Y = rDst.Y + fFlipSize;
                                            point3.X = rDst.X + rDst.Width - fFlipSize; point3.Y = rDst.Y;

                                            pointf1 = point2;
                                            pointf2.X = rDst.X + rDst.Width - fFlipSize; pointf2.Y = rDst.Y + fFlipSize;
                                            pointf3 = point3;
                                            break;
                                        case 2:
                                            point1.X = rDst.X + rDst.Width; point1.Y = rDst.Y + rDst.Height;
                                            point2.X = rDst.X + rDst.Width - fFlipSize; point2.Y = rDst.Y + rDst.Height;
                                            point3.X = rDst.X + rDst.Width; point3.Y = rDst.Y + rDst.Height - fFlipSize;

                                            pointf1 = point2;
                                            pointf2.X = rDst.X + rDst.Width - fFlipSize; pointf2.Y = rDst.Y + rDst.Height - fFlipSize;
                                            pointf3 = point3;
                                            break;
                                        case 3:
                                            point1.X = rDst.X; point1.Y = rDst.Y + rDst.Height;
                                            point2.X = rDst.X; point2.Y = rDst.Y + rDst.Height - fFlipSize;
                                            point3.X = rDst.X + fFlipSize; point3.Y = rDst.Y + rDst.Height;

                                            pointf1 = point2;
                                            pointf2.X = rDst.X + fFlipSize; pointf2.Y = rDst.Y + rDst.Height - fFlipSize;
                                            pointf3 = point3;
                                            break;
                                    }
                                    PointF[] curvePoints = { point1, point2, point3 };
                                    PointF[] curvePointsf = { pointf1, pointf2, pointf3 };
                                    g.FillPolygon(platebrush, curvePoints);
                                    g.FillPolygon(graybrush, curvePointsf);

                                    // Draw page number/name in upper left corner box 
                                    //Rectangle pageNumberRect = new Rectangle(xpos*PAGEWIDTH + FRAMEWIDTH+PAGEWIDTH/2-PageName[p].Length*6+2, ypos*imgHeight + FRAMEWIDTH+5+1,PageName[p].Length*16,(int)f.GetHeight()+4);
                                    //g.FillRectangle(pagebrush, pageNumberRect);
                                    //g.DrawString(PageName[p],f2, blackbrush,(float)(xpos*PAGEWIDTH + FRAMEWIDTH+PAGEWIDTH/2-PageName[p].Length*6), (float)(ypos*imgHeight + FRAMEWIDTH+5));
                                    //g.DrawRectangle(framepen, pageNumberRect);

                                    string sPageName = PageName[p];
                                    string sSec = PageSection[p];
                                    if (sSec != "" && hideSection == false)
                                        sPageName = sSec + " " + sPageName;
                                    //g.FillRectangle(whitebrush,rDst);
                                    g.DrawString(sPageName, f2, blackbrush, rDst, drawFormat);
                                }


                            }
                        }
                        if (bIsHeld)
                             g.DrawImage(StopImage, nPlateWidth / 2 - 16, nPlateHeight / 2 - 16, 32, 32);

                    }

                    // Reverse page sequence?
                    if (((prevFlatRotation & 1) > 0 && prevPlateNumber % 2 == 0) || ((prevFlatRotation & 2) > 0 && prevPlateNumber % 2 == 1))
                    {
                        string[] pages = thisPageNames.Split(',');
                        string thisPageNames2 = "";
                        if (pages.Length > 0)
                        {
                            for (int i = pages.Length - 1; i >= 0; i--)
                            {
                                if (thisPageNames2 != "")
                                    thisPageNames2 += ",";
                                thisPageNames2 += pages[i];
                            }
                        }
                        thisPageNames = thisPageNames2;

                    }
                    string pageHeader = thisPageNames.Replace(",", "  ");
                    string[] sections = thisSectionName.Split(',');
                    string sectionHeader = sections[0];
                    bool hideSection2 = Globals.GetCacheRowCount("SectionNameCache") < 2;
                    if (hideSection2)
                        sectionHeader = "";

                    if (sections.Length > 1 && hideSection2 == false)
                        if (sections.Length > 1)
                        {
                            bool differentSectionNames = false;
                            foreach (string s in sections)
                            {
                                if (s != sections[0])
                                {
                                    differentSectionNames = true;
                                    break;
                                }
                            }
                            if (differentSectionNames)
                            {
                                string[] pages = thisPageNames.Split(' ');
                                pageHeader = "";
                                if (pages.Length > 0)
                                {
                                    for (int i = 0; i < pages.Length; i++)
                                    {
                                        if (pageHeader != "")
                                            pageHeader += " ";
                                        pageHeader += sections[i] + pages[i];
                                    }
                                }
                                sectionHeader = "";
                            }
                        }
                
                    if ((bool)Application["FlatViewShowSheetNumber"])
                    {
                        dr["ImageDesc"] = imgdesc;
                        dr["ImageDescB"] = pageHeader;
                    }
                    else
                    {
                        dr["ImageDesc"] = sectionHeader;
                        dr["ImageDescB"] = pageHeader;
                    }
                    if ((bool)Application["FlatViewShowPlateNumber"])
                    {
                        dr["ImageDesc"] = "[" + prevPlateName + "]";
                        dr["ImageDescB"] = pageHeader;
                    }




                    string statusName = Globals.GetStatusName(prevStatus, 0);
                    if (statusName == "Transmitted")
                        statusName = Global.rm.GetString("txtTransmitted");
                    else if (statusName == "Transmitting")
                        statusName = Global.rm.GetString("txtTransmitting");
                    else if (statusName == "Missing")
                        statusName = Global.rm.GetString("txtMissing");
                    else if (statusName == "Ready")
                        statusName = Global.rm.GetString("txtReady");
                    else if (statusName == "Paired")
                        statusName = Global.rm.GetString("txtPaired");
                    else if (statusName == "Imaged")
                        statusName = (int)Application["FlatDependOnExtStatusNumber"] == prevExtStatus ? Global.rm.GetString("txtBend") : Global.rm.GetString("txtOutput");



                    dr["ImageDescC"] = prevStatus == 70 ? Global.rm.GetString("txtError") : statusName;

                    dr["ImageDesc2"] = dr["ImageDesc2"] = bIsHeld ? Global.rm.GetString("txtOnHold") : Global.rm.GetString("txtReleased");



                    //				if (prevUnique)

                    string timestr = string.Format("{0:0000}{1:00}{2:00}{3:00}{4:00}{5:00}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                    dr["ImageName"] = "../Images/Flats/" + (string)Session["UserName"] + "_" + prevCopyFlatSeparationSet.ToString() + "_" + timestr + ".jpg";

                    try
                    {
                        if (rotateFlat)
                            flatbitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        flatbitmap.Save(Request.MapPath(Request.ApplicationPath) + "/Images/Flats/" + (string)Session["UserName"] + "_" + prevCopyFlatSeparationSet.ToString() + "_" + timestr + ".jpg", ImageFormat.Jpeg);
                    }
                    catch (Exception ee)
                    {
                        lblError.Text = ee.Message;
                        lblError.ForeColor = Color.Red;
                    }
                    flatbitmap.Dispose();
                    g.Dispose();

                    if (bIsHeld)
                        dr["ImageNumber"] = prevCopyFlatSeparationSet.ToString() + "&" + prevStatus.ToString() + "&1";
                    else
                        dr["ImageNumber"] = prevCopyFlatSeparationSet.ToString() + "&" + prevStatus.ToString() + "&0";

                    if (prevPlateUnique)
                        dr["ImageInfo"] = (string)dr["ImageNumber"] + "&" + thisColorString + "&1&" + prevVersion.ToString() + "&" + prevExtStatus.ToString();
                    else if (prevPlateForced)
                        dr["ImageInfo"] = (string)dr["ImageNumber"] + "&" + thisColorString + "&2&" + prevVersion.ToString() + "&" + prevExtStatus.ToString();
                    else
                        dr["ImageInfo"] = (string)dr["ImageNumber"] + "&" + thisColorString + "&0&" + prevVersion.ToString() + "&" + prevExtStatus.ToString();

                    dr["ImageQueryString"] = "set=" + prevCopyFlatSeparationSet.ToString() + "&ver=" + prevVersion.ToString();

                    nImageNumber++;
                    dt.Rows.Add(dr);

                }
            }

            return dt.DefaultView;
        }


        private void PrepareZoom(int nCopyFlatSeparationSet, int version)
        {
            Session["CurrentCopyFlatSeparationSet"] = nCopyFlatSeparationSet;
            Session["CurrentCopyFlatSeparationSetVersion"] = version;
            Session["ImagePath"] = "";
            Session["ImagePathMask"] = "";
            Session["HasTiles"] = false;
            Session["ShowSep"] = "CMYK";

            int publicationID = 0;
            DateTime pubDate = DateTime.MinValue;
            int nMaxVersion = 0;

            bool hasTiles = false;
            string errmsg = "";
            CCDBaccess db = new CCDBaccess();

            //	if ((bool)Application["ThumbnailShowMask"])
            //		hasMask = db.HasMaskFlat(nCopyFlatSeparationSet, out errmsg);
            int flatProofStatus = 0;
            int status = 0;

            if (db.GetFlatProofStatus(nCopyFlatSeparationSet, ref  status, ref  flatProofStatus, ref nMaxVersion, ref publicationID, ref pubDate, out errmsg) == true)
            {
                bool isReady = false;
                if (status >= 30) /*&& (bool)Application["LocationIsPress"] == false) ||
                    (status >= 47 && (bool)Application["LocationIsPress"]))*/
                    isReady = true;
                if (((bool)Application["CheckFlatProofStatus"] && flatProofStatus < 10) || isReady == false)
                    return;
            }

            if ((bool)Application["UseVersionFlats"] == false)
                nMaxVersion = 0;



            string realPath = "";
            string virtualPath = "";

            if (Globals.HasTileFolderFlatview(nCopyFlatSeparationSet, nMaxVersion, publicationID, pubDate, false, ref realPath, ref virtualPath))
            {
                hasTiles = true;
                Session["ImagePath"] = virtualPath;
                Session["HasTiles"] = true;
            }

            if (hasTiles == false)
            {
                if (Globals.HasPreviewFlatview(nCopyFlatSeparationSet, false, ref realPath, ref virtualPath, ""))
                {
                    Session["ImagePath"] = virtualPath;
                }
            }

            if ((string)Session["ImagePath"] != "")
            {
                Session["RealImagePath"] = realPath;    // For mail attachment only
                Response.Redirect("ZoomviewFlatFlash2.aspx");
               
            }
        }

        protected void RadToolBar1_ButtonClick(object sender, Telerik.Web.UI.RadToolBarEventArgs e)
		{

            if (e.Item.Value == "PlatePerRow2")
                Session["PlatesPerRow"] = 2;
            else if (e.Item.Value == "PlatePerRow4")
                Session["PlatesPerRow"] = 4;
            else if (e.Item.Value == "PlatePerRow6")
                Session["PlatesPerRow"] = 6;
            else if (e.Item.Value == "PlatePerRow8")
                Session["PlatesPerRow"] = 8;
            else if (e.Item.Value == "PlatePerRow10")
                Session["PlatesPerRow"] = 10;


            else if (e.Item.Value == "Refresh" || e.Item.Value == "HideCommon")
            {
                ;
            }

            else if (e.Item.Value == "ReleaseAll")
            {
                int publicationID = Session["SelectedPublication"] != null ? Globals.GetIDFromName("PublicationNameCache", (string)Session["SelectedPublication"]) : 0;
                DateTime pubDate = Session["SelectedPubDate"] != null ? (DateTime)Session["SelectedPubDate"] : DateTime.MinValue;

                if (publicationID > 0 && pubDate.Year > 2000)
                {
                    Telerik.Web.UI.RadWindow mywindow = RadWindowManager1.Windows[3]; // "radWindowReleaseAll"
                    mywindow.VisibleOnPageLoad = true;
                }
            }

            else if (e.Item.Value == "RetransmitAll")
            {
                int publicationID = Session["SelectedPublication"] != null ? Globals.GetIDFromName("PublicationNameCache", (string)Session["SelectedPublication"]) : 0;
                DateTime pubDate = Session["SelectedPubDate"] != null ? (DateTime)Session["SelectedPubDate"] : DateTime.MinValue;

                if (publicationID > 0 && pubDate.Year > 2000)
                {
                    Telerik.Web.UI.RadWindow mywindow = RadWindowManager1.Windows[4]; // "radWindowRetransmitAll"
                    mywindow.VisibleOnPageLoad = true;
                }


            }

            ReBind(true);

        }

        private void SetFilterLabel()
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue("Item3");
            if (item == null)
                return;
            Label label = (Label)item.FindControl("FilterLabel");
            if (label == null)
                return;

            string displayFilter = "  ";
            bool hideEdition = Globals.GetCacheRowCount("EditionNameCache") < 2;
            bool hideSection = Globals.GetCacheRowCount("SectionNameCache") < 2;

            if ((string)Session["SelectedPublication"] != "")
                displayFilter += " " + (string)Session["SelectedPublication"];

            DateTime selectedPubDate = (DateTime)Session["SelectedPubDate"];
            if (selectedPubDate.Year > 2000)
                displayFilter += " " + selectedPubDate.Month + "/" + selectedPubDate.Day + "/" + selectedPubDate.Year;

            if ((string)Session["SelectedEdition"] != "" && hideEdition == false)
                displayFilter += " " + (string)Session["SelectedEdition"];

            if ((string)Session["SelectedSection"] != "" && hideSection == false)
                displayFilter += " " + (string)Session["SelectedSection"];

            if (displayFilter == "  ")
                displayFilter = "  All";

            
            label.Text = displayFilter;
            label.ForeColor = Color.Blue;
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


        private void CleanTempFlatImages()
        {
            try
            {

                string[] filesToDelete = Directory.GetFiles(Request.MapPath(Request.ApplicationPath) + "/Images/Flats/", (string)Session["UserName"] + "_*.*");
                foreach (string fileName in filesToDelete)
                {
                    File.Delete(fileName);
                }
            }
            catch
            {
            }

        }
	}
}


