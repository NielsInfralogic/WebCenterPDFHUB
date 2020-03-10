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

namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for Flatview.
	/// </summary>
	public partial class Flatview3 : System.Web.UI.Page
	{
		

		public int PAGEWIDTH;
		public int PAGEWIDTHLANDSCAPE;
		//static int FRAMEWIDTH = 5;
		//static int COPYSHOWERHEIGHT = 36;

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
                catch //(Exception e1)
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
                Panel ftpanel = (Panel)e.Item.FindControl("pnlFooter");


                HtmlTable table = (HtmlTable)e.Item.FindControl("TableFlats");
                HtmlInputHidden hidden = (HtmlInputHidden)e.Item.FindControl("hiddenImageID");

                string scmd = hidden.Value;
                string[] sargs = scmd.Split('&');

                Panel panelBottom = (Panel)e.Item.FindControl("pnlBottom");
                Panel panelHeader = (Panel)e.Item.FindControl("pnlHeader");
                Panel panelBottom2 = (Panel)e.Item.FindControl("pnlBottom2");

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
                    string approveState = "-1";
                    string colorString = sargs[3];
                    string uniqueString = sargs[4];
                    int version = 1;
                    if (sargs.Length > 5)
                        version = Globals.TryParse(sargs[5], 1);
                    int nextstatus = 30;
                    if (sargs.Length > 6)
                        nextstatus = Globals.TryParse(sargs[6], 30);
                    if (sargs.Length > 7)
                        approveState = sargs[7];

                    string[] sargscolors = colorString.Split(';');

                    Color statusColor = Globals.GetStatusColor(nstatus, 0);

                    if (imgApprove != null)
                        imgApprove.Visible = (bool)Session["MayApprove"] && (bool)Application["FlatViewShowApproveButton"];
                    if (imgDisapprove != null)
                        imgDisapprove.Visible = (bool)Session["MayApprove"] && (bool)Application["FlatViewShowApproveButton"];
                    if (imgReimage != null)
                        imgReimage.Visible = (bool)Session["MayReimage"] ;
                    if (imgHold != null)
                        imgHold.Visible = (bool)Session["MayRelease"] && ((bool)Application["FlatHideRelease"] == false);
                    if (imgRelease != null)
                        imgRelease.Visible = (bool)Session["MayRelease"] && ((bool)Application["FlatHideRelease"] == false);
                    if (imgReleaseBlack != null)
                        imgReleaseBlack.Visible = (bool)Session["MayRelease"] && version > 1 && ((bool)Application["FlatHideRelease"] == false);
                    if (btnPrinter != null)
                        btnPrinter.Visible = (bool)Application["AllowFlatproof"];

                   

                    if (nstatus == 50)
                    {
                        panelHeader.BackColor = Color.LawnGreen;
                        if ((bool)Application["FlatLook"] == false)
                            panelHeader.BackImageUrl = "../Images/greengradient2.gif";
                        e.Item.BackColor = Color.LawnGreen;
                        if ((bool)Application["FlatDependOnExtStatus"] && (int)Application["FlatDependOnExtStatusNumber"] != nextstatus)
                        {
                            panelHeader.BackColor = Color.Orange;
                            if ((bool)Application["FlatLook"] == false)
                                panelHeader.BackImageUrl = "../Images/orangegradient2.gif";
                            e.Item.BackColor = Color.Orange;
                        }
                    }

                    else if (nstatus > 0 && nstatus <= 30)
                    {
                        panelHeader.BackColor = Color.Yellow;
                        if ((bool)Application["FlatLook"] == false)
                            panelHeader.BackImageUrl = "../Images/yellowgradient2.gif";
                        e.Item.BackColor = Color.Yellow;
                    }
                    else if (nstatus > 30 && nstatus < 50)
                    {
                        panelHeader.BackColor = Color.Orange;
                        if ((bool)Application["FlatLook"] == false)
                            panelHeader.BackImageUrl = "../Images/orangegradient2.gif";
                        e.Item.BackColor = Color.Orange;
                    }
                    else if (nstatus == 70)
                    {
                        panelHeader.BackColor = Color.Red;
                        if ((bool)Application["FlatLook"] == false)
                            panelHeader.BackImageUrl = "../Images/redgradient2.gif";
                        e.Item.BackColor = Color.Red;

                    }
                    else
                    {
                        panelHeader.BackColor = Color.LightGray;
                        if ((bool)Application["FlatLook"] == false)
                            panelHeader.BackImageUrl = "../Images/graygradient2.gif";
                        e.Item.BackColor = Color.LightGray;
                    }

                    if (uniqueString == "0")
                        e.Item.BackColor = Color.LightSkyBlue;
                    panelBottom2.BackColor = panelHeader.BackColor;

                    if ((bool)Application["FlatHideRelease"] == false) //&& (bool)Application["FlatViewShowApproveButton"] == false)
                    {
                        if (holdState == "0")
                        {
                            panelBottom.BackColor = Color.LightGreen;
                            if ((bool)Application["FlatLook"] == false)
                                panelBottom.BackImageUrl = "../Images/greengradient2.gif";
                        }
                        else // 1
                        {
                            panelBottom.BackColor = Color.Orange;
                            if ((bool)Application["FlatLook"] == false)
                                panelBottom.BackImageUrl = "../Images/orangegradient2.gif";
                        }
                    }

                    if ((bool)Application["FlatViewShowApproveButton"])
                    {
                        if ((bool)Application["FlatHideRelease"])
                        {
                            if (approveState == "1")
                            {
                                panelBottom.BackColor = Color.LightGreen;
                                if ((bool)Application["FlatLook"] == false)
                                    panelBottom.BackImageUrl = "../Images/greengradient2.gif";
                            }
                            else if (approveState == "2")
                            {
                                panelBottom.BackColor = Color.Red;
                                if ((bool)Application["FlatLook"] == false)
                                    panelBottom.BackImageUrl = "../Images/redgradient2.gif";
                            }
                            else if (approveState == "-1")
                            {
                                panelBottom.BackColor = Color.LightSkyBlue;
                                if ((bool)Application["FlatLook"] == false)
                                    panelBottom.BackImageUrl = "../Images/bluegradient2.gif";
                            }
                            else
                            {
                                panelBottom.BackColor = Color.LightGray;
                                if ((bool)Application["FlatLook"] == false)
                                    panelBottom.BackImageUrl = "../Images/graygradient2.gif";
                            }
                        }
                        else
                        {
                            if (approveState == "1")
                            {
                                panelBottom2.BackColor = Color.LightGreen;
                                if ((bool)Application["FlatLook"] == false)
                                    panelBottom2.BackImageUrl = "../Images/greengradient2.gif";
                            }
                            else if (approveState == "2")
                            {
                                panelBottom2.BackColor = Color.Red;
                                if ((bool)Application["FlatLook"] == false)
                                    panelBottom2.BackImageUrl = "../Images/redgradient2.gif";
                            }
                            else if (approveState == "-1")
                            {
                                panelBottom2.BackColor = Color.LightSkyBlue;
                                if ((bool)Application["FlatLook"] == false)
                                    panelBottom2.BackImageUrl = "../Images/bluegradient2.gif";
                            }
                            else
                            {
                                panelBottom2.BackColor = Color.LightGray;
                                if ((bool)Application["FlatLook"] == false)
                                    panelBottom2.BackImageUrl = "../Images/graygradient2.gif";
                            }
                        }
                    }

                    

                   
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

            bool hideCommon = (bool)Session["HideCommon"];

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

            string NoPagePath = (bool)Application["FlatLook"] ? "../Images/NoPage_Flat.gif": "../Images/NoPage.gif";
            string DummyPagePath = "../Images/Dummy.gif";

            DataView dv = dstable.DefaultView;
            int nImageNumber = 0;
            int nMinStatus = 100;
            int nMinExtStatus = 100;
            bool bHasError = false;
            bool bIsHeld = false;
            bool bIsApproved = true;
            bool bIsAutoApproved = true;
            bool bIsDisApproved = false;
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

            // Try to find image dimensions..	
            int nJpegImageWidth = nImageWidth;
            int nJpegImageHeight = nImageHeight;


            /*
            nImagesPerRow = (int)Session["PagesPerRow"] / 2;
            nRefreshTime = (int)Session["RefreshTime"];
            nImageWidth = ((int)Session["WindowWidth"]-60) /nImagesPerRow - (nImagesPerRow-1); 

            // Blank dummy image is 160x118 - set a default height used if no pages are avail. - otherwise height is set by first detected image.
            nImageHeight = nImageWidth*118;
            nImageHeight /=160;
*/
            // Find pages on plate
            if (dv.Count > 0)
            {
                nPagesAcross = (int)dv[0]["PagesAcross"];
                nPagesDown = (int)dv[0]["PagesDown"];
            }
            if (nPagesDown == 0)
                nPagesDown = 1;
            if (nPagesAcross == 0)
                nPagesAcross = 1;

            // Correct default imagewidth if 1-up production
            if (nPagesAcross == 1 && nPagesDown == 1)
            {
                nImagesPerRow = (int)Session["PagesPerRow"];
                nRefreshTime = (int)Session["RefreshTime"];
                nImageWidth = ((int)Session["WindowWidth"] - 60) / nImagesPerRow - (nImagesPerRow - 1);
                nImageHeight = nImageWidth * 160;
                nImageHeight /= 118;
            }

       /*     if ((bool)Application["RotateFlats"])
            {
                int tmp = nPagesDown;
                nPagesDown = nPagesAcross;
                nPagesAcross = tmp;
            }
        */
            if (nPagesAcross == 2 && nPagesDown == 1)
                NoPagePath = (bool)Application["FlatLook"] ?  "../Images/NoPagePano_Flat.gif" : "../Images/NoPagePano.gif";
            else if (nPagesAcross == 2 && nPagesDown == 2)
            {
                string srot = (string)dv[0]["PageRotations"];
                if (srot[0] == '0' || srot[0] == '2')
                    NoPagePath = (bool)Application["FlatLook"] ? "../Images/NoPage4lying_Flat.gif" : "../Images/NoPage4lying.gif";
                else
                    NoPagePath = (bool)Application["FlatLook"] ? "../Images/NoPage4standing_Flat.gif" : "../Images/NoPage4standing.gif";
            }
            else if (nPagesAcross == 4 && nPagesDown == 2)
                NoPagePath = (bool)Application["FlatLook"] ?  "../Images/NoPage8standing_Flat.gif" : "../Images/NoPage8standing.gif";
            else if (nPagesAcross == 1 && nPagesDown == 2)
                NoPagePath = (bool)Application["FlatLook"] ? "../Images/NoPagePanoStanding.gif" : "../Images/NoPagePanoStanding_Flat.gif";

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

            // Find ratio height vs width of page
            foreach (DataRowView row in dv)
            {
                int flatNumber = (int)row["CopyFlatSeparationSet"];

                string flatRealPath = Global.sRealFlatThumbnailFolder + "\\" + flatNumber.ToString() + ".jpg";
                if (System.IO.File.Exists(flatRealPath))
                {
                    try
                    {
                        System.Drawing.Image TestImage = System.Drawing.Image.FromFile(flatRealPath);
                        nJpegImageWidth = TestImage.Size.Width;
                        nJpegImageHeight = TestImage.Size.Height;
                        imageRatio = (double)TestImage.Size.Height / (double)TestImage.Size.Width;
                        double ratio = (double)(nImageWidth) * (double)TestImage.Size.Height / (double)(TestImage.Size.Width) + 0.5;
                        TestImage.Dispose();
                        nImageHeight = (int)ratio;
                    }
                    catch
                    {
                        lblError.ForeColor = Color.Red;
                        lblError.Text = "IMAGE NOT FOUND -  (" + flatNumber.ToString() + ".jpg)";
                    }
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
                    if (bIsPlateUnique || (bIsUnique == false && hideCommon == false))
                    {
                        // Init the plate drawing
                        DataRow dr = dt.NewRow();

                        dr["ImageName"] = prevStatus == 0 ? NoPagePath : DummyPagePath;
                        dr["ImageWidth"] = nImageWidth;

                        if (bHasError)
                            nMinStatus = 70;

                        string imgdesc = prevPlateNumber / 2 + (prevPlateNumber % 2 == 0 ? " " + Global.rm.GetString("txtFrontSide") : " " + Global.rm.GetString("txtBackSide"));

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
                        if ((bool)Application["FlatHideRelease"] && (bool)Application["FlatViewShowApproveButton"])
                        {
                            if (bIsDisApproved)
                                dr["ImageDesc2"] = Global.rm.GetString("txtRejected");
                            else if (bIsAutoApproved && bIsApproved)
                                dr["ImageDesc2"] = Global.rm.GetString("txtApproved");
                            else
                                dr["ImageDesc2"] = Global.rm.GetString("txtNotApproved");
                        }

                        string flatRealThumbnailPath = Global.sRealFlatThumbnailFolder + "\\" + prevCopyFlatSeparationSet.ToString() + ".jpg";
                        // FlatPreview
                        string flatRealPath = Global.sRealFlatImageFolder + "\\" + prevCopyFlatSeparationSet.ToString() + ".jpg";

                        string flatVirtualThumbnailPath = Global.sVirtualFlatThumbnailsFolder + "/" + prevCopyFlatSeparationSet.ToString() + ".jpg";

                        string flatRealThumbnailPathWithVersion = Global.sRealFlatThumbnailFolder + "\\" + prevCopyFlatSeparationSet.ToString() + "-" + prevVersion.ToString() + ".jpg";

                        bool bGotVesionThumbnail = false;
                        Global.logging.WriteLog("Testing FileExists + " + flatRealThumbnailPathWithVersion + " ..");
                        if (System.IO.File.Exists(flatRealThumbnailPathWithVersion))
                        {
                            flatVirtualThumbnailPath = Global.sVirtualFlatThumbnailsFolder + "/" + prevCopyFlatSeparationSet.ToString() + "-" + prevVersion.ToString() + ".jpg";
                            flatRealThumbnailPath = flatRealThumbnailPathWithVersion;
                            bGotVesionThumbnail = true;
                            Global.logging.WriteLog("FileExists + " + flatRealThumbnailPathWithVersion + " OK");
                        }
                        if ((bool)Application["UseVersionThumbnails"] && bGotVesionThumbnail == false)
                            flatVirtualThumbnailPath = DummyPagePath;

                        bool hasFlat = false;
                        Global.logging.WriteLog("Testing Flat " + prevCopyFlatSeparationSet.ToString() + " prevStatus=" + prevStatus.ToString());
                        if (prevStatus >= 30 /*&& (bool)Application["LocationIsPress"] == false*/)
                         //   ||  (prevStatus >= 47 && (bool)Application["LocationIsPress"]))
                        {
                            hasFlat = System.IO.File.Exists(flatRealThumbnailPath);
                            bool hasFlat2 = System.IO.File.Exists(flatRealPath);

                            bool XMLOK = Globals.CheckTileXML((bool)Application["UseVersionFlats"] ?
                                Global.sRealFlatImageFolder + "\\" + prevCopyFlatSeparationSet.ToString() + "-" + prevVersion.ToString() + "\\"
                                : Global.sRealFlatImageFolder + "\\" + prevCopyFlatSeparationSet.ToString() + "\\",
                                (string)Session["SelectedPublication"], (DateTime)Session["SelectedPubDate"]);
                            Global.logging.WriteLog("Testing Flat " + prevCopyFlatSeparationSet.ToString() + " XMLOK=" + XMLOK.ToString() + ",hasFlat=" + hasFlat.ToString() + ",hasFlat2=" + hasFlat2.ToString() + ",prevFlatProofStatus=" + prevFlatProofStatus.ToString());
                            if (XMLOK == false || hasFlat == false || hasFlat2 == false || (prevFlatProofStatus < 10 && (bool)Application["CheckFlatProofStatus"])/* || prevStatus <=30  ||*/)
                            {
                                flatRealThumbnailPath = DummyPagePath;
                                hasFlat = false;
                            }
                        }

                        if (hasFlat)
                            dr["ImageName"] = flatVirtualThumbnailPath;

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

                        if (bIsDisApproved)
                            dr["ImageInfo"] = (string)dr["ImageNumber"] + "&" + thisColorString + "&1&" + prevVersion.ToString() + "&" + prevExtStatus.ToString() + "&2";
                        else if (bIsAutoApproved)
                            dr["ImageInfo"] = (string)dr["ImageNumber"] + "&" + thisColorString + "&1&" + prevVersion.ToString() + "&" + prevExtStatus.ToString() + "&-1";
                        else if (bIsApproved)
                            dr["ImageInfo"] = (string)dr["ImageNumber"] + "&" + thisColorString + "&1&" + prevVersion.ToString() + "&" + prevExtStatus.ToString() + "&1";
                        else
                            dr["ImageInfo"] = (string)dr["ImageNumber"] + "&" + thisColorString + "&1&" + prevVersion.ToString() + "&" + prevExtStatus.ToString() + "&0";





                        if (flatRealPath != NoPagePath && flatRealPath != DummyPagePath && hasFlat == true)
                            dr["ImageQueryString"] = "set=" + prevCopyFlatSeparationSet.ToString() + "&ver=" + prevVersion.ToString();
                        else
                            dr["ImageQueryString"] = "set=0&ver=1";

                        nImageNumber++;
                        dt.Rows.Add(dr);
                    }
                    // Reset flags for next plate
                    bHasError = false;
                    bIsHeld = false;
                    bIsUnique = false;
                    bIsPlateUnique = false;
                    bIsPlateForced = false;

                    bIsApproved = true;
                    bIsAutoApproved = true;
                    bIsDisApproved = false;

                    nMinStatus = 100;
                    thisPageNames = "";
                    thisSectionName = "";
                    thisColorString = "";

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
                            drfill["ImageName"] = "../Images/Spacer.gif";
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
                            drfill["ImageName"] = "../Images/Spacer.gif";
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
                            drfill["ImageName"] = "../Images/Spacer.gif";
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
                            drfill["ImageName"] = "../Images/Spacer.gif";
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

                thisFlatSeparationSet = (int)row["FlatSeparationSet"];
                thisCopyFlatSeparationSet = (int)row["CopyFlatSeparationSet"];
                thisFlatProofStatus = (int)row["FlatProofStatus"];

                string[] colors = thisColorString.Split(';');

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
                if ((int)row["Approval"] != -1)
                    bIsAutoApproved = false;
                if ((int)row["Approval"] == 2)
                    bIsDisApproved = true;

                if (thisCopyNumber == 1)
                {
                    if (thisPageNames != "")
                        thisPageNames += ",";
                    thisPageNames += (string)row["Page"];

                    if (thisSectionName != "")
                        thisSectionName += ",";
                    thisSectionName += (string)row["Section"];
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
                if (bIsPlateUnique || (bIsPlateUnique == false && hideCommon == false))
                //				if (bIsUnique || bIsUnique == false && hideCommon == false)
                {
                    DataRow dr = dt.NewRow();

                    dr["ImageName"] = prevStatus == 0 ? NoPagePath : DummyPagePath;
                    dr["ImageWidth"] = nImageWidth;

                    string imgdesc = prevPlateNumber / 2 + (prevPlateNumber % 2 == 0 ? " " + Global.rm.GetString("txtFrontSide") : " " + Global.rm.GetString("txtBackSide"));

                    if (bHasError)
                        nMinStatus = 70;

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

                    string flatRealThumbnailPath = Global.sRealFlatThumbnailFolder + "\\" + prevCopyFlatSeparationSet.ToString() + ".jpg";
                    // FlatPreview
                    string flatRealPath = Global.sRealFlatImageFolder + "\\" + prevCopyFlatSeparationSet.ToString() + ".jpg";

                    string flatVirtualThumbnailPath = Global.sVirtualFlatThumbnailsFolder + "/" + prevCopyFlatSeparationSet.ToString() + ".jpg";

                    string flatRealThumbnailPathWithVersion = Global.sRealFlatThumbnailFolder + "\\" + prevCopyFlatSeparationSet.ToString() + "-" + prevVersion.ToString() + ".jpg";

                    bool bGotVesionThumbnail = false;
                    if (System.IO.File.Exists(flatRealThumbnailPathWithVersion))
                    {
                        flatVirtualThumbnailPath = Global.sVirtualFlatThumbnailsFolder + "/" + prevCopyFlatSeparationSet.ToString() + "-" + prevVersion.ToString() + ".jpg"; ;
                        flatRealThumbnailPath = flatRealThumbnailPathWithVersion;
                        bGotVesionThumbnail = true;
                    }
                    if ((bool)Application["UseVersionThumbnails"] && bGotVesionThumbnail == false)
                        flatVirtualThumbnailPath = DummyPagePath;

                    bool hasFlat = false;
                    if (prevStatus >= 30 ) /*&& (bool)Application["LocationIsPress"] == false) ||
                        (prevStatus >= 47 && (bool)Application["LocationIsPress"]))*/
                    {
                        hasFlat = System.IO.File.Exists(flatRealThumbnailPath);
                        bool hasFlat2 = System.IO.File.Exists(flatRealPath);

                        bool XMLOK = Globals.CheckTileXML((bool)Application["UseVersionFlats"] ?
                                                            Global.sRealFlatImageFolder + "\\" + prevCopyFlatSeparationSet.ToString() + "-" + prevVersion.ToString() + "\\"
                                                            : Global.sRealFlatImageFolder + "\\" + prevCopyFlatSeparationSet.ToString() + "\\",
                                                            (string)Session["SelectedPublication"], (DateTime)Session["SelectedPubDate"]);

                        if (XMLOK == false || hasFlat == false || hasFlat2 == false || (prevFlatProofStatus < 10 && (bool)Application["CheckFlatProofStatus"])/* || prevStatus <=30  ||*/)
                        {
                            flatRealThumbnailPath = DummyPagePath;
                            hasFlat = false;
                        }
                    }

                    if (hasFlat)
                        dr["ImageName"] = flatVirtualThumbnailPath;

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


                    if (bIsDisApproved)
                        dr["ImageInfo"] = (string)dr["ImageNumber"] + "&" + thisColorString + "&1&" + prevVersion.ToString() + "&" + prevExtStatus.ToString() + "&2";
                    else if (bIsAutoApproved)
                        dr["ImageInfo"] = (string)dr["ImageNumber"] + "&" + thisColorString + "&1&" + prevVersion.ToString() + "&" + prevExtStatus.ToString() + "&-1";
                    else if (bIsApproved)
                        dr["ImageInfo"] = (string)dr["ImageNumber"] + "&" + thisColorString + "&1&" + prevVersion.ToString() + "&" + prevExtStatus.ToString() + "&1";
                    else
                        dr["ImageInfo"] = (string)dr["ImageNumber"] + "&" + thisColorString + "&1&" + prevVersion.ToString() + "&" + prevExtStatus.ToString() + "&0";

                    if (flatRealPath != NoPagePath && flatRealPath != DummyPagePath && hasFlat == true)
                        dr["ImageQueryString"] = "set=" + prevCopyFlatSeparationSet.ToString() + "&ver=" + prevVersion.ToString();
                    else
                        dr["ImageQueryString"] = "set=0&ver=1";


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
	}
}


