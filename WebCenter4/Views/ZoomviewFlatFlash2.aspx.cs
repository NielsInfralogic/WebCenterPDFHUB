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
using System.Web.Mail;
using System.Net;

namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for Zoomview.
	/// </summary>
	public class ZoomviewFlatFlash2 : System.Web.UI.Page
	{
        protected System.Web.UI.WebControls.TextBox txtReturnedFromPopup;
        protected Telerik.Web.UI.RadWindowManager RadWindowManager1;
        protected global::Telerik.Web.UI.RadToolBar RadToolBar1;

        public string sColors;
        public string sImagePath;

		public int nCurrentCopyFlatSeparationSet;
		public string printimagepath;
		public int simpleFlash;

		private void Page_Load(object sender, System.EventArgs e)
		{
            //Loop through all windows in the WindowManager.Windows collection
            foreach (Telerik.Web.UI.RadWindow win in RadWindowManager1.Windows)
            {
                //Set whether the first window will be visible on page load
                win.VisibleOnPageLoad = false;
            }
            
            if ((string)Session["UserName"] == null)
				Response.Redirect("~/SessionTimeout.htm");

			if ((string)Session["UserName"] == "")
				Response.Redirect("/Denied.htm");

            if (txtReturnedFromPopup.Text == "1")
            {
                if ((bool)Application["SimpleFlatView"])
                    Response.Redirect("Flatview3.aspx");
                 else
                   Response.Redirect("Flatview2.aspx");
                return;
            }

            int version = 1;
            if (Session["CurrentCopyFlatSeparationSetVersion"] != null)
                version = (int)Session["CurrentCopyFlatSeparationSetVersion"];

            if (!Page.IsPostBack)
			{
				SetLanguage();

                SetRadToolbarVisible("ReleaseBlack", (bool)Session["MayRelease"] && version > 1 && ((bool)Application["FlatHideRelease"] == false));
                SetRadToolbarEnable("ReleaseBlack", (bool)Session["MayRelease"] && version > 1 && ((bool)Application["FlatHideRelease"] == false));
                SetRadToolbarEnable("ReleaseSpecial", (bool)Session["MayRelease"] && ((bool)Application["FlatHideRelease"] == false));
                SetRadToolbarVisible("ReleaseSpecial", (bool)Session["MayRelease"] && ((bool)Application["FlatHideRelease"] == false));

                SetRadToolbarVisible("Release", (bool)Session["MayRelease"]  && ((bool)Application["FlatHideRelease"] == false));
                SetRadToolbarEnable("Release", (bool)Session["MayRelease"]  && ((bool)Application["FlatHideRelease"] == false));
                SetRadToolbarVisible("Hold", (bool)Session["MayRelease"]  && ((bool)Application["FlatHideRelease"] == false));
                SetRadToolbarEnable("Hold", (bool)Session["MayRelease"]  && ((bool)Application["FlatHideRelease"] == false));

                SetRadToolbarVisible("Approve", (bool)Session["MayApprove"] &&  (bool)Application["FlatViewShowApproveButton"]);
                SetRadToolbarEnable("Approve", (bool)Session["MayApprove"] &&  (bool)Application["FlatViewShowApproveButton"]);
                SetRadToolbarVisible("Disapprove", (bool)Session["MayApprove"] && (bool)Application["FlatViewShowApproveButton"]);
                SetRadToolbarEnable("Disapprove", (bool)Session["MayApprove"]  && (bool)Application["FlatViewShowApproveButton"]);

                PrepareZoom((int)Session["CurrentCopyFlatSeparationSet"]);
			}
            txtReturnedFromPopup.Text = "0";
            if ((bool)Application["NoCache"]) 
			{
				Response.AppendHeader("cache-control", "private" );
				Response.AppendHeader("pragma", "no-cache" );
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
            SetRadToolbarTooltip("Approve", Global.rm.GetString("txtTooltipApprovePages"));

            SetRadToolbarLabel("Disapprove", Global.rm.GetString("txtReject"));
            SetRadToolbarTooltip("Disapprove", Global.rm.GetString("txtTooltipDisapprovePages"));

            SetRadToolbarEnable("Release", (bool)Session["MayRelease"]);
            SetRadToolbarLabel("Release", Global.rm.GetString("txtRelease"));
            SetRadToolbarTooltip("Release", Global.rm.GetString("txtTooltipReleasePlates"));

            SetRadToolbarEnable("Hold", (bool)Session["MayRelease"]);
            SetRadToolbarLabel("Hold", Global.rm.GetString("txtHold"));
            SetRadToolbarTooltip("Hold", Global.rm.GetString("txtTooltipHoldPlates"));

            SetRadToolbarEnable("ReleaseBlack", (bool)Session["MayRelease"]);
            SetRadToolbarLabel("ReleaseBlack", Global.rm.GetString("txtReleaseBlack"));
            SetRadToolbarTooltip("ReleaseBlack", Global.rm.GetString("txtReleaseBlackForm"));

            SetRadToolbarEnable("ReleaseSpecial", (bool)Session["MayRelease"]);
            SetRadToolbarVisible("ReleaseSpecial", (bool)Session["MayRelease"]);
            SetRadToolbarLabel("ReleaseSpecial", Global.rm.GetString("txtReleaseSpecial"));
            SetRadToolbarTooltip("ReleaseSpecial", Global.rm.GetString("txtTooltipReleasePlatesSpecial"));

            SetRadToolbarLabel("Close", Global.rm.GetString("txtBack"));
            SetRadToolbarTooltip("Close", Global.rm.GetString("txtTooltipBack"));

            SetRadToolbarLabel("Backward", Global.rm.GetString("txtBackward"));
            SetRadToolbarLabel("Forward", Global.rm.GetString("txtForward"));

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
			string errmsg = "";
			if (e.Item.Value == "Forward")
			{
				// Are we at the end already?
				if ((int)Session["CurrentCopyFlatSeparationSet"] == 0)
				{
					PrepareZoom((int)Session["CurrentCopyFlatSeparationSet"]);
					return;	
				}

				CCDBaccess db = new CCDBaccess();

				nCurrentCopyFlatSeparationSet =  db.GetNextFlat((int)Session["CurrentCopyFlatSeparationSet"], out errmsg);
				
				if (nCurrentCopyFlatSeparationSet == 0)
				{
					PrepareZoom((int)Session["CurrentCopyFlatSeparationSet"]);
					return;	
				}

				if ((string)Session["ShowSep"] != "CMYK"  && (string)Session["ShowSep"] != "DNS")
					Session["ShowSep"] = "CMYK";


				PrepareZoom(nCurrentCopyFlatSeparationSet);
				
			}


			if (e.Item.Value == "Backward")
			{
				// Are we at the front already?
				if ((int)Session["CurrentCopyFlatSeparationSet"] == 0)
				{
					PrepareZoom((int)Session["CurrentCopyFlatSeparationSet"]);
					return;	
				}

				CCDBaccess db = new CCDBaccess();

				nCurrentCopyFlatSeparationSet =  db.GetPreviousFlat((int)Session["CurrentCopyFlatSeparationSet"], out errmsg);
				if (nCurrentCopyFlatSeparationSet == 0)
				{
					PrepareZoom((int)Session["CurrentCopyFlatSeparationSet"]);
					return;	
				}

				if ((string)Session["ShowSep"] != "CMYK"  && (string)Session["ShowSep"] != "DNS")
					Session["ShowSep"] = "CMYK";


				PrepareZoom(nCurrentCopyFlatSeparationSet);
		

			}

			if (e.Item.Value == "Release")
			{
				if ((bool)Session["MayRelease"] == false)
					return;
				CCDBaccess db = new CCDBaccess();

                nCurrentCopyFlatSeparationSet = (int)Session["CurrentCopyFlatSeparationSet"];
                if (nCurrentCopyFlatSeparationSet > 0)
                    db.UpdateCopyFlatHold(nCurrentCopyFlatSeparationSet, 0, 0, out errmsg);

                if ((bool)Session["CloseZoomAfterApprove"])
                    Response.Redirect("Flatview3.aspx");
			}

            if (e.Item.Value == "Approve")
            {
                if ((bool)Session["MayApprove"] == false)
                    return;
                CCDBaccess db = new CCDBaccess();

                nCurrentCopyFlatSeparationSet = (int)Session["CurrentCopyFlatSeparationSet"];
                if (nCurrentCopyFlatSeparationSet > 0)
                    db.UpdateFlatApproval((string)Session["UserName"], nCurrentCopyFlatSeparationSet, 1, out errmsg);

                if ((bool)Session["CloseZoomAfterApprove"])
                    Response.Redirect("Flatview3.aspx");
            }

            if (e.Item.Value == "Disapprove")
            {
                if ((bool)Session["MayApprove"] == false)
                    return;
                CCDBaccess db = new CCDBaccess();

                nCurrentCopyFlatSeparationSet = (int)Session["CurrentCopyFlatSeparationSet"];
                if (nCurrentCopyFlatSeparationSet > 0)
                    db.UpdateFlatApproval((string)Session["UserName"], nCurrentCopyFlatSeparationSet, 2, out errmsg);

                if ((bool)Session["CloseZoomAfterApprove"])
                    Response.Redirect("Flatview3.aspx");
            }

            if (e.Item.Value == "ReleaseBlack")
            {
                if ((bool)Session["MayRelease"] == false)
                    return;
                CCDBaccess db = new CCDBaccess();

                nCurrentCopyFlatSeparationSet = (int)Session["CurrentCopyFlatSeparationSet"];
                if (nCurrentCopyFlatSeparationSet > 0)
                    db.UpdateCopyFlatHold(nCurrentCopyFlatSeparationSet, 0, 4, out errmsg);

                if ((bool)Session["CloseZoomAfterApprove"])
                {
                    if ((bool)Application["SimpleFlatView"])
                        Response.Redirect("Flatview3.aspx");
                    else
                        Response.Redirect("Flatview2.aspx");
                }
            }
            if (e.Item.Value == "ReleaseSpecial")
            {
                if ((bool)Session["MayRelease"] == false)
                    return;
                CCDBaccess db = new CCDBaccess();

                nCurrentCopyFlatSeparationSet = (int)Session["CurrentCopyFlatSeparationSet"];
                if (nCurrentCopyFlatSeparationSet > 0)
                {

                    Telerik.Web.UI.RadWindow mywindow = RadWindowManager1.Windows[0]; // "radWindowReleaseLocations"
                    mywindow.NavigateUrl = "ReleasePresses.aspx?CopyFlatSeparationSet=" + nCurrentCopyFlatSeparationSet.ToString();

                    mywindow.VisibleOnPageLoad = true;
                }
            }

            if (e.Item.Value == "Hold")
            {
                if ((bool)Session["MayRelease"] == false)
                    return;
                CCDBaccess db = new CCDBaccess();

                nCurrentCopyFlatSeparationSet = (int)Session["CurrentCopyFlatSeparationSet"];

                if (nCurrentCopyFlatSeparationSet > 0)
                    db.UpdateCopyFlatHold(nCurrentCopyFlatSeparationSet, 1, 0, out errmsg);

                if ((bool)Session["CloseZoomAfterApprove"])
                {
                    if ((bool)Application["SimpleFlatView"])
                        Response.Redirect("Flatview3.aspx");
                    else
                        Response.Redirect("Flatview2.aspx");
                }
            }

			if (e.Item.Value == "Close")
                if ((bool)Application["SimpleFlatView"])
                    Response.Redirect("Flatview3.aspx");
                else
                    Response.Redirect("Flatview2.aspx");

            if (e.Item.Value == "CMYK" || e.Item.Value == "C" || e.Item.Value == "M" || e.Item.Value == "Y" || e.Item.Value == "K" ||
                e.Item.Value == "CZ" || e.Item.Value == "MZ" || e.Item.Value == "YZ" || e.Item.Value == "KZ" || e.Item.Value == "Dns")
			{
				Session["ShowSep"] = e.Item.Value.ToUpper();
				PrepareZoom((int)Session["CurrentCopyFlatSeparationSet"]);
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


		private void PrepareZoom(int nCopyFlatSeparationSet )
		{

            Session["CurrentCopyFlatSeparationSet"] = nCopyFlatSeparationSet;
            Session["ImagePath"] = "";
            Session["ImagePathMask"] = "";
            Session["RealImagePath"] = "";
            Session["HasTiles"] = false;

            bool hasTiles = false;
            string errmsg = "";
            CCDBaccess db = new CCDBaccess();

            bool showSep = (string)Session["ShowSep"] == "C" ||
                            (string)Session["ShowSep"] == "M" ||
                            (string)Session["ShowSep"] == "Y" ||
                            (string)Session["ShowSep"] == "K" ||
                            (string)Session["ShowSep"] == "DNS";
            bool showZone = (string)Session["ShowSep"] == "CZ" ||
                            (string)Session["ShowSep"] == "MZ" ||
                            (string)Session["ShowSep"] == "YZ" ||
                            (string)Session["ShowSep"] == "KZ";

            string realPath = "";
            string virtualPath = "";

            int publicationID = 0;
            int nVersion = 0;
            DateTime pubDate = DateTime.MinValue;
            db.GetFlatDetails(nCopyFlatSeparationSet, ref nVersion, ref publicationID, ref pubDate, out errmsg);

            if (Globals.HasTileFolderFlatview(nCopyFlatSeparationSet, nVersion, publicationID, pubDate, false, ref realPath, ref virtualPath))
            {
                hasTiles = true;
                Session["ImagePath"] = virtualPath;
                Session["HasTiles"] = true;
            }


            if (hasTiles == false || showSep || showZone)
            {
                if (Globals.HasPreviewFlatview(nCopyFlatSeparationSet, false, ref realPath, ref virtualPath, showSep || showZone ? (string)Session["ShowSep"] : ""))
                {
                    Session["ImagePath"] = virtualPath;
                }
            }

            if ((string)Session["ImagePath"] == "")
            {
                // Display error message..
                return;
            }

            bool hasCyan = System.IO.File.Exists(Global.sRealFlatImageFolder + "\\" + nCopyFlatSeparationSet.ToString() + "_C.jpg");
            bool hasMagenta = System.IO.File.Exists(Global.sRealFlatImageFolder + "\\" + nCopyFlatSeparationSet.ToString() + "_M.jpg");
            bool hasYellow = System.IO.File.Exists(Global.sRealFlatImageFolder + "\\" + nCopyFlatSeparationSet.ToString() + "_Y.jpg");
            bool hasBlack = System.IO.File.Exists(Global.sRealFlatImageFolder + "\\" + nCopyFlatSeparationSet.ToString() + "_K.jpg");

            bool hasCyanZone = System.IO.File.Exists(Global.sRealFlatImageFolder + "\\" + nCopyFlatSeparationSet.ToString() + "_CZ.jpg");
            bool hasMagentaZone = System.IO.File.Exists(Global.sRealFlatImageFolder + "\\" + nCopyFlatSeparationSet.ToString() + "_MZ.jpg");
            bool hasYellowZone = System.IO.File.Exists(Global.sRealFlatImageFolder + "\\" + nCopyFlatSeparationSet.ToString() + "_YZ.jpg");
            bool hasBlackZone = System.IO.File.Exists(Global.sRealFlatImageFolder + "\\" + nCopyFlatSeparationSet.ToString() + "_KZ.jpg");
            bool hasDNS = System.IO.File.Exists(Global.sRealFlatImageFolder + "\\" + nCopyFlatSeparationSet.ToString() + "_dns.jpg");

            SetRadToolbarVisible("C", hasCyan);
            SetRadToolbarVisible("M", hasMagenta);
            SetRadToolbarVisible("Y", hasYellow);
            SetRadToolbarVisible("K", hasBlack);
            SetRadToolbarVisible("Dns", hasDNS);
            SetRadToolbarVisible("CZ", hasCyanZone);
            SetRadToolbarVisible("MZ", hasMagentaZone);
            SetRadToolbarVisible("YZ", hasYellowZone);
            SetRadToolbarVisible("KZ", hasBlackZone);

            if ((string)Session["ImagePath"] != "")
                Session["RealImagePath"] = realPath;	// For mail attachment only

            // Finally - set the global sImagePath var used by flash module

            sImagePath = (string)Session["ImagePath"];

            // Default to CMYK if missing file(s)
            if ((string)Session["ShowSep"] == "C" && hasCyan == false ||
                (string)Session["ShowSep"] == "M" && hasMagenta == false ||
                (string)Session["ShowSep"] == "Y" && hasYellow == false ||
                (string)Session["ShowSep"] == "K" && hasBlack == false ||
                (string)Session["ShowSep"] == "CZ" && hasCyanZone == false ||
                (string)Session["ShowSep"] == "MZ" && hasMagentaZone == false ||
                (string)Session["ShowSep"] == "YZ" && hasYellowZone == false ||
                (string)Session["ShowSep"] == "KZ" && hasBlackZone == false ||
                (string)Session["ShowSep"] == "DNS" && hasDNS == false)
                Session["ShowSep"] = "CMYK";

        

            printimagepath = Global.sVirtualFlatImageFolder + "/" + nCopyFlatSeparationSet.ToString() + ".jpg";

            if (hasTiles && (string)Session["ShowSep"] == "CMYK")
            {
                simpleFlash = 0;
                sImagePath = (string)Session["ImagePath"] + "/";
                return;
            }

            // All other images are shown without tiles

            simpleFlash = 1;
            sImagePath = (string)Session["ImagePath"];

        }

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

        private void SetRadToolbarVisible(string buttonID, bool visible)
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue(buttonID);
            if (item == null)
                return;
            item.Visible = visible;
        }

        private void SetRadToolbarChecked(string buttonID, bool check)
        {
            Telerik.Web.UI.RadToolBarButton item = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue(buttonID);
            if (item == null)
                return;
            item.Checked = check;
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
