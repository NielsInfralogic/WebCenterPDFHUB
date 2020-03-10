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
using WebCenter4.Classes;
using System.Text;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Configuration;
//using Aspose.Excel;
using System.IO;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Docs.Excel;

namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for PlanView.
	/// </summary>
	public class ReportView : System.Web.UI.Page
	{
        protected global::System.Web.UI.WebControls.Label lblError;
        protected global::System.Web.UI.HtmlControls.HtmlImage chart;
        protected global::System.Web.UI.HtmlControls.HtmlImage chart2;
        protected global::System.Web.UI.WebControls.Label lblChooseProduct;
        protected global::System.Web.UI.WebControls.Label lblNumberOfPages;
        protected global::System.Web.UI.WebControls.Label lblNumberOfPagesDate;
        protected global::System.Web.UI.WebControls.Label lblPagesInput;
        protected global::System.Web.UI.WebControls.Label lblPagesInputData;
        protected global::System.Web.UI.WebControls.Label lblPagesApproved;
        protected global::System.Web.UI.WebControls.Label lblPagesApprovedData;
        protected global::System.Web.UI.WebControls.Label lblPagesOutput;
        protected global::System.Web.UI.WebControls.Label lblPagesOutputData;
        protected global::System.Web.UI.WebControls.Label lblFirstPageInput;
        protected global::System.Web.UI.WebControls.Label lblLastPageInput;
        protected global::System.Web.UI.WebControls.Label lblFirstPageApproved;
        protected global::System.Web.UI.WebControls.Label lblLastPageApproved;
        protected global::System.Web.UI.WebControls.Label lblFirstPageOutput;
        protected global::System.Web.UI.WebControls.Label lblLastPageOutput;
        protected global::System.Web.UI.WebControls.Label lblFirstPageInputData;
        protected global::System.Web.UI.WebControls.Label lblLastPageInputData;
        protected global::System.Web.UI.WebControls.Label lblFirstPageApprovedData;
        protected global::System.Web.UI.WebControls.Label lblLastPageApprovedData;
        protected global::System.Web.UI.WebControls.Label lblFirstPageOutputData;
        protected global::System.Web.UI.WebControls.Label lblLastPageOutputData;
        protected global::System.Web.UI.WebControls.Label lblNumberOfPlates;
        protected global::System.Web.UI.WebControls.Label lblNumberOfPlatesData;
        protected global::System.Web.UI.WebControls.Label lblPlatesOutput;
        protected global::System.Web.UI.WebControls.Label lblPlatesOutputData;
        protected global::System.Web.UI.WebControls.DataGrid DataGrid1;
        protected global::System.Web.UI.WebControls.Label lblPageVersionStat;
        protected global::System.Web.UI.WebControls.Label lblPlatesUsed;
        protected global::System.Web.UI.WebControls.Label lblPlatesUsedData;
        protected global::System.Web.UI.WebControls.Label lblPlatesUsedUpdates;
        protected global::System.Web.UI.WebControls.Label lblPlatesUsedDamaged;
        protected global::System.Web.UI.WebControls.Label lblPlatesUsedUpdatesData;
        protected global::System.Web.UI.WebControls.Label lblPlatesUsedDamagedData;
        protected global::Telerik.Web.UI.RadButton btnExcelReport;
        protected global::System.Web.UI.WebControls.CheckBox CheckBoxOrderByPlate;
        protected global::System.Web.UI.WebControls.Label lblDeadline;
        protected global::System.Web.UI.WebControls.Label LblDeadlineData;
        protected global::Telerik.Web.UI.RadButton btnShowArchive;
        protected global::Telerik.Web.UI.RadButton btnShowMonthlyReport;
        protected global::Telerik.Web.UI.RadWindowManager RadWindowManager1;
        protected global::Telerik.Web.UI.RadToolBar RadToolBar1;
		
		protected int imageHeight;
		protected int imageWidth;
		public string chartTitle = "Graphs show accumulared page input/approval/output progress over time";
		public string chartTitle2 = "Graphs show throughput for page input/approval/output over time as pages/hour";

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ((string)Session["UserName"] == null)
				Response.Redirect("~/SessionTimeout.htm");

			if ((string)Session["UserName"] == "")
				Response.Redirect("/Denied.htm");

            Session["HideInputTime"] = false;
            Session["HideApproveTime"] = false;
            Session["HideOutputTime"] = false;

			SetLanguage();

            imageWidth = Globals.TryParseCookie(Request, "ScreenWidth", 800);
            imageHeight = Globals.TryParseCookie(Request, "ScreenHeight", 600);

			if (imageWidth <= 0)
				imageWidth = 800;
			if (imageHeight <= 0)
				imageHeight = 600;

            DoDataBind();

			if (!this.IsPostBack) 
			{
				lblError.Text = "";
				btnShowArchive.Visible = (bool)Application["ReportShowArchive"] && Global.sVirtualReportFolder.Trim() != "";

                btnShowMonthlyReport.Visible = (bool)Application["ReportShowMonthly"] && (bool)Session["IsAdmin"] && (bool)Application["spExists_spFullProductionStatistics"];
			}

			//Loop through all windows in the WindowManager.Windows collection
			foreach (Telerik.Web.UI.RadWindow win in RadWindowManager1.Windows)
			{
				win.VisibleOnPageLoad = false;
			}

		}

        private void DoDataBind()
        {
            if ((string)Session["SelectedPublication"] == "")
            {
                chart.Visible = false;
                chart2.Visible = false;
                lblChooseProduct.Visible = true;
                btnExcelReport.Visible = false;
                return;
            }
            else
            {
                btnExcelReport.Visible = true;
                lblChooseProduct.Visible = false;
                chart.Height = imageHeight / 2 - 20;
                chart.Width = imageWidth - 272;
                chart.Src = "ChartImage.aspx?width=" + chart.Width.ToString() + "&height=" + chart.Height.ToString();

                chart2.Height = imageHeight / 2 - 20;
                chart2.Width = imageWidth - 272;
                chart2.Src = "ChartImage2.aspx?width=" + chart2.Width.ToString() + "&height=" + chart2.Height.ToString();

                int nPages = 0;
                int nPagesArrived = 0;
                DateTime tFirstArrivedPage;
                DateTime tLastArrivedPage;
                int nPagesApproved;
                DateTime tFirstApprovedPage;
                DateTime tLastApprovedPage;
                int nPagesOutput;
                DateTime tFirstOutputPage;
                DateTime tLastOutputPage;
                int nTotalPlates;
                int nTotalPlatesDone;
                int nTotalPlatesUsed;

                int nTotalPlatesUpdated = 0;
                int nTotalPlatesDamaged = 0;
                DateTime deadLine = DateTime.MinValue;
                DateTime deadLine2 = DateTime.MinValue;
                string errmsg = "";

                CCDBaccess db = new CCDBaccess();

                if (db.GetReportStat("",
                    out nPages,
                    out nPagesArrived, out tFirstArrivedPage, out tLastArrivedPage,
                    out nPagesApproved, out tFirstApprovedPage, out tLastApprovedPage,
                    out nPagesOutput, out tFirstOutputPage, out tLastOutputPage,
                    out nTotalPlates, out nTotalPlatesDone, out nTotalPlatesUsed, out deadLine, out deadLine2,
                     out  nTotalPlatesUpdated, out  nTotalPlatesDamaged, out errmsg) == false)
                {
                    lblError.Text = errmsg;
                }
                else
                {
                    lblNumberOfPagesDate.Text = nPages.ToString();
                    lblPagesInputData.Text = nPagesArrived.ToString();
                    lblPagesApprovedData.Text = nPagesApproved.ToString();
                    lblPagesOutputData.Text = nPagesOutput.ToString();

                    lblFirstPageInputData.Text = tFirstArrivedPage.Year > 2000 ? DateToString(tFirstArrivedPage) : "na";
                    lblLastPageInputData.Text = tLastArrivedPage.Year > 2000 ? DateToString(tLastArrivedPage) : "na";
                    lblFirstPageApprovedData.Text = tFirstApprovedPage.Year > 2000 ? DateToString(tFirstApprovedPage) : "na";
                    lblLastPageApprovedData.Text = tLastApprovedPage.Year > 2000 ? DateToString(tLastApprovedPage) : "na";
                    lblFirstPageOutputData.Text = tFirstOutputPage.Year > 2000 ? DateToString(tFirstOutputPage) : "na";
                    lblLastPageOutputData.Text = tLastOutputPage.Year > 2000 ? DateToString(tLastOutputPage) : "na";
                    lblNumberOfPlatesData.Text = nTotalPlates.ToString();
                    lblPlatesOutputData.Text = nTotalPlatesDone.ToString();
                    lblPlatesUsedData.Text = nTotalPlatesUsed.ToString();
                    lblPlatesUsedUpdatesData.Text = nTotalPlatesUpdated.ToString();
                    lblPlatesUsedDamagedData.Text = nTotalPlatesDamaged.ToString();
                    LblDeadlineData.Text = deadLine.Year > 2000 ? deadLine.ToString() : "na";
                }

                errmsg = "";
                DataTable dtable = db.GetReportStatVersions(out errmsg);
                if (errmsg != "")
                    lblError.Text = errmsg;
                else
                {
                    DataGrid1.DataSource = dtable;
                    DataGrid1.DataBind();

                }

              //  SetToolbarLabel();

            }
        }

        private void SetToolbarLabel()
        {
            DateTime pubdate = (DateTime)Session["SelectedPubDate"];

            SetFilterLabel(pubdate.ToShortDateString() + " " + (string)Session["SelectedPublication"] + " " + (string)Session["SelectedEdition"] + " " + (string)Session["SelectedSection"]);
        }

        private void SetFilterLabel(string text)
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue("Item3");
            if (item == null)
                return;
            Label label = (Label)item.FindControl("FilterLabel");
            if (label == null)
                return;
            label.Text = text;
            //label.ForeColor = color;
        }


        private void SetRadToolbarLabel(string buttonID, string text)
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue(buttonID);
            if (item == null)
                return;
            item.Text = text;
        }


		protected void SetLanguage()
		{
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            SetRadToolbarLabel("Refresh", Global.rm.GetString("txtRefresh"));

            lblChooseProduct.Text = Global.rm.GetString("txtChooseProduct");
			lblNumberOfPages.Text = Global.rm.GetString("txtStatNumberOfPages");	
			lblPagesInput.Text = Global.rm.GetString("txtStatPagesInput");
			lblFirstPageInput.Text = Global.rm.GetString("txtStatFirstPageInput");
			lblLastPageInput.Text = Global.rm.GetString("txtStatLastPageInput");

			lblPagesApproved.Text = Global.rm.GetString("txtStatPagesApproved");
			lblFirstPageApproved.Text = Global.rm.GetString("txtStatFirstPageApproved");
			lblLastPageApproved.Text = Global.rm.GetString("txtStatLastPageApproved");
			
			lblPagesOutput.Text = Global.rm.GetString("txtStatPagesOutput");
			lblFirstPageOutput.Text = Global.rm.GetString("txtStatFirstPageOutput");
			lblLastPageOutput.Text = Global.rm.GetString("txtStatLastPageOutput");

			lblNumberOfPlates.Text = Global.rm.GetString("txtStatNumberOfPlates");
			lblPlatesOutput.Text = Global.rm.GetString("txtStatNumberOfPlatesOutput");
			lblPlatesUsed.Text = Global.rm.GetString("txtStatNumberOfPlatesUsed");

            lblPlatesUsedUpdates.Text = Global.rm.GetString("txtStatNumberOfPlatesUpdated");
            lblPlatesUsedDamaged.Text = Global.rm.GetString("txtStatNumberOfPlatesDamaged");


            lblPageVersionStat.Text = Global.rm.GetString("txtStatPageVersionStat");

			btnExcelReport.Text = Global.rm.GetString("txtDownloadExcel");

			btnShowMonthlyReport.Text = Global.rm.GetString("txtShowMonthlySummary");
			
			chartTitle = Global.rm.GetString("txtTooltipStatGraph1");
			chartTitle2 = Global.rm.GetString("txtTooltipStatGraph2");

			lblDeadline.Text = Global.rm.GetString("txtDeadLine");

			btnShowArchive.Text = Global.rm.GetString("txtShowReportArchive");
			chart.Attributes.Add("Title", chartTitle);
			chart2.Attributes.Add("Title", chartTitle2);
			Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowReportArchive");
			mywindow.Title =  Global.rm.GetString("txtReportArchive");

		}

		private string DateToString(DateTime dt)
		{
			return dt.Day.ToString("00")+"."+dt.Month.ToString("00")+"."+dt.Year.ToString("0000")+" "+dt.Hour.ToString("00")+":"+dt.Minute.ToString("00")+":"+dt.Second.ToString("00");
		}
		
		private string ShortDateToString(DateTime dt)
		{
			return dt.Day.ToString("00")+"."+dt.Month.ToString("00");
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
			lblError.Text = "";
		}
/*
        protected void btnExcelReportOLD_Click(object sender, System.EventArgs e)
		{
            CCDBaccess db = new CCDBaccess();
            int nPages = 0;
            int nPagesArrived = 0;
            DateTime tFirstArrivedPage;
            DateTime tLastArrivedPage;
            int nPagesApproved;
            DateTime tFirstApprovedPage;
            DateTime tLastApprovedPage;
            int nPagesOutput;
            DateTime tFirstOutputPage;
            DateTime tLastOutputPage;
            int nTotalPlates;
            int nTotalPlatesDone;
            int nTotalPlatesUsed;
            string errmsg = "";
            DateTime deadLine = DateTime.MinValue;
            if (db.GetReportStat("",
                out nPages,
                out nPagesArrived, out tFirstArrivedPage, out tLastArrivedPage,
                out nPagesApproved, out tFirstApprovedPage, out tLastApprovedPage,
                out nPagesOutput, out tFirstOutputPage, out tLastOutputPage,
                out nTotalPlates, out nTotalPlatesDone, out nTotalPlatesUsed, out deadLine, out errmsg) == false)
            {
                lblError.Text = errmsg;
                return;
            }

            string publication = (string)Session["SelectedPublication"];
            int publicationID = Globals.GetIDFromName("PublicationNameCache", publication);
            DateTime pubDate = (DateTime)Session["SelectedPubDate"];
            string edition = (string)Session["SelectedEdition"];
            string section = (string)Session["SelectedSection"];
            bool hideEdition = Globals.GetCacheRowCount("EditionNameCache") < 2;// || (string)Session["SelectedEdition"] != "";
            bool hideSection = Globals.GetCacheRowCount("SectionNameCache") < 2 ;//|| (string)Session["SelectedSection"] != "";
            int Cinkmean = 0;
            int Minkmean = 0;
            int Yinkmean = 0;
            int Kinkmean = 0;
            DataTable dtab = db.GetStatCollectionExcel(hideEdition == false, hideSection == false, (bool)Application["ExtendedThumbnailViewShowFTP"],
                                                    (bool)Application["ExtendedThumbnailViewShowPreflight"], (bool)Application["ExtendedThumbnailViewShowRIP"],
                                                    (bool)Application["ExtendedThumbnailViewShowColorWarning"], (bool)Application["ReportShowReadyTime"],
                                                    (bool)Application["ReportShowViewTime"], CheckBoxOrderByPlate.Checked, (bool)Application["ReportShowAfterDeadline"],
                                                    (bool)Application["ReportShowCMYKInk"], ref  Cinkmean, ref  Minkmean, ref  Yinkmean, ref  Kinkmean, out errmsg);
            if (dtab == null || errmsg != "")
            {
                lblError.Text = errmsg;
                Global.logging.WriteLog("btnExcelReport_Click exception - " + errmsg);

                return;
            }
            if (dtab.Rows.Count == 0)
            {
                lblError.Text = "No data available";
                Global.logging.WriteLog("btnExcelReport_Click No data available");
            }


            string title = publication + " " + ShortDateToString(pubDate);
            if (hideEdition)
                title += " " + edition;
            if (hideSection)
                title += " " + section;

            //Set Aspose.Excel License
            Aspose.Excel.License licExcel = new Aspose.Excel.License();
            licExcel.SetLicense((System.IO.Stream)null);

            Excel excel = new Aspose.Excel.Excel();
            excel.Worksheets.Add();
            excel.Worksheets[0].Name = title;
            Cells cells = excel.Worksheets[0].Cells;


            Cell cell = cells[0, 0];

            cell.PutValue("WebCenter " + Global.rm.GetString("txtStatistics"));
            cell.Style.Font.IsBold = true;
            cell.Style.Font.Size = 18;
            cells.SetRowHeight(0, 20);


            cell = cells[2, 0];
            cell.PutValue(Global.rm.GetString("txtProduct"));
            cell.Style.Font.IsBold = true;
            cell = cells[2, 2];
            cell.PutValue(title);

            cell.Style.HorizontalAlignment = TextAlignmentType.Top;
            //			cells.SetColumnWidth(2,20);

            cells.SetColumnWidth(0, 24);
            cells.SetColumnWidth(1, 14);

            int y = 3;
            cell = cells[++y, 0];
            cell.PutValue(Global.rm.GetString("txtStatNumberOfPages"));
            cell.Style.Font.IsBold = true;
            cell = cells[y, 2];
            cell.PutValue(nPages);

            y++;

            if ((bool)Application["ReportShowDeadline"])
            {
                cell = cells[++y, 0];
                cell.PutValue(Global.rm.GetString("txtDeadLine"));
                cell.Style.Font.IsBold = true;
                cell = cells[y, 2];
                cell.PutValue(deadLine.Year > 2000 ? deadLine.ToString() : "na");

                y++;
            }

            cell = cells[++y, 0];
            cell.PutValue(Global.rm.GetString("txtStatPagesInput"));
            cell.Style.Font.IsBold = true;
            cell = cells[y, 2];
            cell.PutValue(nPagesArrived);

            cell = cells[++y, 0];
            cell.PutValue(Global.rm.GetString("txtStatFirstPageInput"));
            cell.Style.Font.IsBold = true;
            cell = cells[y, 2];
            cell.PutValue(tFirstArrivedPage.Year > 2000 ? tFirstArrivedPage.ToString() : "na");

            cell = cells[++y, 0];
            cell.PutValue(Global.rm.GetString("txtStatLastPageInput"));
            cell.Style.Font.IsBold = true;
            cell = cells[y, 2];
            cell.PutValue(tLastArrivedPage.Year > 2000 ? tLastArrivedPage.ToString() : "na");


            y++;
            cell = cells[++y, 0];
            cell.PutValue(Global.rm.GetString("txtStatPagesApproved"));
            cell.Style.Font.IsBold = true;
            cell = cells[y, 2];
            cell.PutValue(nPagesApproved);

            cell = cells[++y, 0];
            cell.PutValue(Global.rm.GetString("txtStatFirstPageApproved"));
            cell.Style.Font.IsBold = true;
            cell = cells[y, 2];
            cell.PutValue(tFirstApprovedPage.Year > 2000 ? tFirstApprovedPage.ToString() : "na");

            cell = cells[++y, 0];
            cell.PutValue(Global.rm.GetString("txtStatLastPageApproved"));
            cell.Style.Font.IsBold = true;
            cell = cells[y, 2];
            cell.PutValue(tLastApprovedPage.Year > 2000 ? tLastApprovedPage.ToString() : "na");

            y++;
            cell = cells[++y, 0];
            cell.PutValue(Global.rm.GetString("txtStatPagesOutput"));
            cell.Style.Font.IsBold = true;
            cell = cells[y, 2];
            cell.PutValue(nPagesOutput);


            cell = cells[++y, 0];
            cell.PutValue(Global.rm.GetString("txtStatFirstPageOutput"));
            cell.Style.Font.IsBold = true;
            cell = cells[y, 2];
            cell.PutValue(tFirstOutputPage.Year > 2000 ? tFirstOutputPage.ToString() : "na");

            cell = cells[++y, 0];
            cell.PutValue(Global.rm.GetString("txtStatLastPageOutput"));
            cell.Style.Font.IsBold = true;
            cell = cells[y, 2];
            cell.PutValue(tLastOutputPage.Year > 2000 ? tLastOutputPage.ToString() : "na");

            y++;
            cell = cells[++y, 0];
            cell.PutValue(Global.rm.GetString("txtStatNumberOfPlates"));
            cell.Style.Font.IsBold = true;
            cell = cells[y, 2];
            cell.PutValue(nTotalPlates);

            cell = cells[++y, 0];
            cell.PutValue(Global.rm.GetString("txtStatNumberOfPlatesOutput"));
            cell.Style.Font.IsBold = true;
            cell = cells[y, 2];
            cell.PutValue(nTotalPlatesDone);

            cell = cells[++y, 0];
            cell.PutValue(Global.rm.GetString("txtStatNumberOfPlatesUsed"));
            cell.Style.Font.IsBold = true;
            cell = cells[y, 2];
            cell.PutValue(nTotalPlatesUsed);

            if ((bool)Application["ReportShowCMYKInk"])
            {
                y++;

                cell = cells[++y, 0];
                cell.PutValue("C ink mean (%)");
                cell.Style.Font.IsBold = true;
                cell = cells[y, 2];
                cell.PutValue(Cinkmean);

                cell = cells[++y, 0];
                cell.PutValue("M ink mean (%)");
                cell.Style.Font.IsBold = true;
                cell = cells[y, 2];
                cell.PutValue(Minkmean);

                cell = cells[++y, 0];
                cell.PutValue("Y ink mean (%)");
                cell.Style.Font.IsBold = true;
                cell = cells[y, 2];
                cell.PutValue(Yinkmean);

                cell = cells[++y, 0];
                cell.PutValue("K ink mean (%)");
                cell.Style.Font.IsBold = true;
                cell = cells[y, 2];
                cell.PutValue(Kinkmean);
            }

            y += 2;
            int x = 0;

            // table headers

            if (CheckBoxOrderByPlate.Checked)
            {
                cell = cells[y, x];
                //cell.PutValue(Global.rm.GetString("txtSheetNumber")); 
                cell.PutValue("Sheet");
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 14);
                x++;

                cell = cells[y, x];
                //cell.PutValue(Global.rm.GetString("txtSheetSide")); 
                cell.PutValue("SheetSide");
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 14);
                x++;

            }

            if (hideEdition == false)
            {
                cell = cells[y, x];
                cell.PutValue(Global.rm.GetString("txtEdition"));
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 14);
                x++;
            }

            if (hideSection == false)
            {
                cell = cells[y, x];
                cell.PutValue(Global.rm.GetString("txtSection"));
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 14);
                x++;
            }

            cell = cells[y, x];
            cell.PutValue(Global.rm.GetString("txtPage"));
            cell.Style.Font.IsBold = true;
            cells.SetColumnWidth((byte)x, 8);
            x++;

            cell = cells[y, x];
            cell.PutValue(Global.rm.GetString("txtFinalVersion"));
            cell.Style.Font.IsBold = true;
            cells.SetColumnWidth((byte)x, 12);
            x++;

            cell = cells[y, x];
            cell.PutValue(Global.rm.GetString("txtStatus"));
            cell.Style.Font.IsBold = true;
            cells.SetColumnWidth((byte)x, 14);
            x++;

            //filter += "Page,Version,Status,InputTime,ViewTime,Approved,ApproveUser,ApproveTime,OutputTime,FTPstatus,FTPmessage,Preflightstatus, Preflightmessage,RIPstatus,RIPmessage,Colorstatus,Colormessage";

            cell = cells[y, x];
            cell.PutValue(Global.rm.GetString("txtInputTime"));
            cell.Style.Font.IsBold = true;
            cells.SetColumnWidth((byte)x, 20);
            x++;

            if ((bool)Application["ReportShowAfterDeadline"])
            {
                cell = cells[y, x];
                cell.PutValue(Global.rm.GetString("txtInputTimeAfterDeadline"));
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 20);
                x++;
            }

            if ((bool)Application["ReportShowViewTime"])
            {
                cell = cells[y, x];
                cell.PutValue(Global.rm.GetString("txtViewTime"));
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 20);
                x++;

                cell = cells[y, x];
                cell.PutValue(Global.rm.GetString("txtViewedBy"));
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 12);
                x++;
            }

            cell = cells[y, x];
            cell.PutValue(Global.rm.GetString("txtApproval"));
            cell.Style.Font.IsBold = true;
            cells.SetColumnWidth((byte)x, 14);
            x++;

            cell = cells[y, x];
            cell.PutValue(Global.rm.GetString("txtApprovedBy"));
            //cell.PutValue("Approved by"); 
            cell.Style.Font.IsBold = true;
            cells.SetColumnWidth((byte)x, 12);
            x++;

            cell = cells[y, x];
            cell.PutValue(Global.rm.GetString("txtApproveTime"));
            cell.Style.Font.IsBold = true;
            cells.SetColumnWidth((byte)x, 20);
            x++;

            if ((bool)Application["ReportShowReadyTime"])
            {
                cell = cells[y, x];
                cell.PutValue(Global.rm.GetString("txtPlateReadyTime"));
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 20);
                x++;
            }


            cell = cells[y, x];
            cell.PutValue((bool)Application["LocationIsPress"] /* && (bool)Application["UsePressGroups"] == false ? Global.rm.GetString("txtTransmitted") : Global.rm.GetString("txtOutput"));
            cell.Style.Font.IsBold = true;
            cells.SetColumnWidth((byte)x, 20);
            x++;

            string msgstring = Global.rm.GetString("txtMessage");
            if ((bool)Application["ExtendedThumbnailViewShowFTP"])
            {
                cell = cells[y, x];
                cell.PutValue("FTP status");
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 13);
                x++;

                cell = cells[y, x];
                cell.PutValue("FTP " + msgstring);
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 50);
                cell.Style.IsTextWrapped = true;
                x++;
            }
            if ((bool)Application["ExtendedThumbnailViewShowPreflight"])
            {
                cell = cells[y, x];
                cell.PutValue("Preflight status");
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 13);
                x++;

                cell = cells[y, x];
                cell.PutValue("Preflight " + msgstring);
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 100);
                cell.Style.IsTextWrapped = true;
                x++;
            }
            if ((bool)Application["ExtendedThumbnailViewShowRIP"])
            {
                cell = cells[y, x];
                cell.PutValue("RIP status");
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 13);
                x++;

                cell = cells[y, x];
                cell.PutValue("RIP " + msgstring);
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 60);
                cell.Style.IsTextWrapped = true;
                x++;

            }
            if ((bool)Application["ExtendedThumbnailViewShowColorWarning"])
            {
                cell = cells[y, x];
                cell.PutValue("Color status");
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 13);
                x++;

                cell = cells[y, x];
                cell.PutValue("Color " + msgstring);
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 60);
                cell.Style.IsTextWrapped = true;
                x++;
            }

            if ((bool)Application["ReportShowCMYKInk"])
            {
                cell = cells[y, x];
                cell.PutValue("C ink");
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 13);
                x++;

                cell = cells[y, x];
                cell.PutValue("M ink");
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 13);
                x++;

                cell = cells[y, x];
                cell.PutValue("Y ink");
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 13);
                x++;

                cell = cells[y, x];
                cell.PutValue("K ink");
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 13);
                x++;

            }


            y++;
            string startcell = "A" + (y + 1).ToString();
            excel.Worksheets.Add();
            if (dtab.Rows.Count > 0)
                excel.Worksheets[0].Cells.ImportDataTable(dtab, false, startcell);

            y += dtab.Rows.Count + 2;

            

            string chart1filename = Request.PhysicalApplicationPath + (string)Session["UserName"] + "_chart.jpg";
            if (SaveChart1(chart1filename, chart.Width, chart.Height))
                excel.Worksheets[0].Pictures.Add(y, 1, chart1filename);


            if ((bool)Application["IncludePageHistory"])
            {

                DataTable dtHistory = new DataTable();
                DataColumn newColumn;
                if (hideEdition == false)
                    newColumn = dtHistory.Columns.Add("Edition", Type.GetType("System.String"));
                if (hideSection == false)
                    newColumn = dtHistory.Columns.Add("Section", Type.GetType("System.String"));

                newColumn = dtHistory.Columns.Add("Page", Type.GetType("System.Int32"));
                newColumn = dtHistory.Columns.Add("Version", Type.GetType("System.Int32"));
                newColumn = dtHistory.Columns.Add("InputTime", Type.GetType("System.String"));
                newColumn = dtHistory.Columns.Add("Approved", Type.GetType("System.String"));
                newColumn = dtHistory.Columns.Add("ApproveUser", Type.GetType("System.String"));
                newColumn = dtHistory.Columns.Add("ApproveTime", Type.GetType("System.String"));
                newColumn = dtHistory.Columns.Add("Message", Type.GetType("System.String"));

                excel.Worksheets.Add();
                excel.Worksheets[1].Name = Global.rm.GetString("txtPageHistory");
                cells = excel.Worksheets[1].Cells;

                y = 3;
                x = 0;

                cell = cells[0, 0];

                cell.PutValue(Global.rm.GetString("txtPageHistory"));
                cell.Style.Font.IsBold = true;
                cell.Style.Font.Size = 18;
                cells.SetRowHeight(0, 20);



                if (hideEdition == false)
                {
                    cell = cells[y, x];
                    cell.PutValue(Global.rm.GetString("txtEdition"));
                    cell.Style.Font.IsBold = true;
                    cells.SetColumnWidth((byte)x, 14);
                    x++;
                }

                if (hideSection == false)
                {
                    cell = cells[y, x];
                    cell.PutValue(Global.rm.GetString("txtSection"));
                    cell.Style.Font.IsBold = true;
                    cells.SetColumnWidth((byte)x, 14);
                    x++;
                }


                cell = cells[y, x];
                cell.PutValue(Global.rm.GetString("txtPage"));
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 8);
                x++;

                cell = cells[y, x];
                cell.PutValue(Global.rm.GetString("txtVersion"));
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 12);
                x++;

                cell = cells[y, x];
                cell.PutValue(Global.rm.GetString("txtInputTime"));
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 20);
                x++;

                cell = cells[y, x];
                cell.PutValue(Global.rm.GetString("txtApproval"));
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 14);
                x++;

                cell = cells[y, x];
                cell.PutValue(Global.rm.GetString("txtApprovedBy"));
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 12);
                x++;

                cell = cells[y, x];
                cell.PutValue(Global.rm.GetString("txtApproveTime"));
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 20);
                x++;

                cell = cells[y, x];
                cell.PutValue("");
                cell.Style.Font.IsBold = true;
                cells.SetColumnWidth((byte)x, 20);
                x++;

                y++;



                DataRow newRow = null;
                ArrayList editionList = db.GetEditionsInProduction(publicationID, pubDate, out errmsg);

                foreach (int editionID in editionList)
                {
                    List<int> sectionList = db.GetSectionsInEdition(publicationID, pubDate, editionID, out errmsg);

                    foreach (int sectionID in sectionList)
                    {
                        ArrayList pageList = db.GetPagesInSection(publicationID, pubDate, editionID, sectionID, out  errmsg);

                        int pageIndex = 0;

                        PageHistory[] versionHistory = new PageHistory[100];
                        for (int i = 0; i < versionHistory.Length; i++)
                            versionHistory[i] = new PageHistory();
//                        PageHistory[] versionHistory = new PageHistory[100];

                        foreach (string pageName in pageList)
                        {
                            pageIndex++;
                            for (int i = 0; i < 100; i++)
                            {
                                versionHistory[i].version = 0;
                                versionHistory[i].approveState = -1;
                            }

                            //	int versions = db.GetPageHistory(ref versionHistory, job, editionID, sectionID,pageName, deadLine, out errmsg);


                            int versions = db.GetPageHistoryExcel(ref versionHistory, publicationID, pubDate, editionID, sectionID, pageName, deadLine, out errmsg);
                            if (versions == 0)
                                continue;

                            for (int v = 0; v < versions; v++)
                            {
                                if (versionHistory[v].version == 0)
                                    continue;

                                newRow = dtHistory.NewRow();

                                if (hideEdition == false)
                                    newRow["Edition"] = Globals.GetNameFromID("EditionNameCache", editionID);
                                if (hideSection == false)
                                    newRow["Section"] = Globals.GetNameFromID("SectionnameCache", sectionID);

                                if (Globals.TryParse(pageName, 0) == 0)
                                    newRow["Page"] = pageIndex;
                                else
                                    newRow["Page"] = Globals.TryParse(pageName, 0);

                                newRow["Version"] = versionHistory[v].version;

                                newRow["InputTime"] = versionHistory[v].inputTime.Year >= 2000 ? versionHistory[v].inputTime.ToString() : ""; ;

                                newRow["ApproveTime"] = versionHistory[v].approveTime.Year >= 2000 ? versionHistory[v].approveTime.ToString() : "";
                                switch (versionHistory[v].approveState)
                                {
                                    case 2:
                                        newRow["Approved"] = Global.rm.GetString("txtRejected");
                                        break;
                                    case 1:
                                        newRow["Approved"] = Global.rm.GetString("txtApproved");
                                        break;
                                    default:
                                        newRow["Approved"] = "";
                                        break;

                                }
                                newRow["ApproveUser"] = versionHistory[v].approveUser;
                                newRow["Message"] = versionHistory[v].message;

                                dtHistory.Rows.Add(newRow);
                            }
                        }
                    }

                }

                startcell = "A" + (y + 1).ToString();
                if (dtHistory.Rows.Count > 0)
                    excel.Worksheets[1].Cells.ImportDataTable(dtHistory, false, startcell);


            }




            string excelName = title + "_report.xls";
            excelName = excelName.Replace("æ", "ae");
            excelName = excelName.Replace("ä", "ae");
            excelName = excelName.Replace("Æ", "AE");
            excelName = excelName.Replace("Ä", "AE");
            excelName = excelName.Replace("ø", "oe");
            excelName = excelName.Replace("ö", "oe");
            excelName = excelName.Replace("Ø", "OE");
            excelName = excelName.Replace("Ö", "OE");
            excelName = excelName.Replace("å", "aa");
            excelName = excelName.Replace("ä", "aa");
            excelName = excelName.Replace("Å", "AA");
            excelName = excelName.Replace("Ä", "AA");

            try
            {
                if ((int)Application["ExcelSavetype"] == 0)
                    excel.Save(excelName, FileFormatType.Excel2000, SaveType.OpenInBrowser, Response);
                else if ((int)Application["ExcelSavetype"] == 1)
                    excel.Save(excelName, FileFormatType.Excel2000, SaveType.OpenInExcel, Response);
                else if ((int)Application["ExcelSavetype"] == 2)
                    excel.Save(excelName, FileFormatType.Excel2000, SaveType.Default, Response);
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }
  */
        protected void btnExcelReport_Click(object sender, System.EventArgs e)
        {
            CCDBaccess db = new CCDBaccess();
            int nPages = 0;
            int nPagesArrived = 0;
            DateTime tFirstArrivedPage;
            DateTime tLastArrivedPage;
            int nPagesApproved;
            DateTime tFirstApprovedPage;
            DateTime tLastApprovedPage;
            int nPagesOutput;
            DateTime tFirstOutputPage;
            DateTime tLastOutputPage;
            int nTotalPlates;
            int nTotalPlatesDone;
            int nTotalPlatesUsed;
            int nTotalPlatesUpdated = 0;
            int nTotalPlatesDamaged = 0;
            string errmsg = "";
            DateTime deadLine = DateTime.MinValue;
            DateTime deadLine2 = DateTime.MinValue;
            if (db.GetReportStat("",
                out nPages,
                out nPagesArrived, out tFirstArrivedPage, out tLastArrivedPage,
                out nPagesApproved, out tFirstApprovedPage, out tLastApprovedPage,
                out nPagesOutput, out tFirstOutputPage, out tLastOutputPage,
                out nTotalPlates, out nTotalPlatesDone, out nTotalPlatesUsed, out deadLine, out deadLine2,
                     out nTotalPlatesUpdated, out nTotalPlatesDamaged,
                out errmsg) == false)
            {
                lblError.Text = errmsg;
                return;
            }

            string publication = (string)Session["SelectedPublication"];
            int publicationID = Globals.GetIDFromName("PublicationNameCache", publication);
            DateTime pubDate = (DateTime)Session["SelectedPubDate"];
            string edition = (string)Session["SelectedEdition"];
            string section = (string)Session["SelectedSection"];
            bool hideEdition = Globals.GetCacheRowCount("EditionNameCache") < 2;// || (string)Session["SelectedEdition"] != "";
            bool hideSection = Globals.GetCacheRowCount("SectionNameCache") < 2;//|| (string)Session["SelectedSection"] != "";
            int Cinkmean = 0;
            int Minkmean = 0;
            int Yinkmean = 0;
            int Kinkmean = 0;
          //  double fAreaMean = 0.0;
            DataTable dtab = db.GetStatCollectionExcel(hideEdition == false, hideSection == false, (bool)Application["ExtendedThumbnailViewShowFTP"],
                                                    (bool)Application["ExtendedThumbnailViewShowPreflight"], (bool)Application["ExtendedThumbnailViewShowRIP"],
                                                    (bool)Application["ExtendedThumbnailViewShowColorWarning"], (bool)Application["ReportShowReadyTime"],
                                                    (bool)Application["ReportShowViewTime"], CheckBoxOrderByPlate.Checked, (bool)Application["ReportShowAfterDeadline"],
                                                    (bool)Application["ReportShowCMYKInk"], ref Cinkmean, ref Minkmean, ref Yinkmean, ref Kinkmean/*, ref fAreaMean*/, out errmsg);
            if (dtab == null || errmsg != "")
            {
                lblError.Text = errmsg;
                Global.logging.WriteLog("btnExcelReport_Click exception - " + errmsg);

                return;
            }
            if (dtab.Rows.Count == 0)
            {
                lblError.Text = "No data available";
                Global.logging.WriteLog("btnExcelReport_Click No data available");
            }

            string title = publication + " " + ShortDateToString(pubDate);
            if (hideEdition)
                title += " " + edition;
            if (hideSection)
                title += " " + section;

            ExcelWorkbook.SetLicenseCode("S2215N-661233-01BL5C-119A00");
            ExcelWorkbook Wbook = new ExcelWorkbook();
         
            Wbook.Worksheets.Add(title);

            Wbook.Worksheets[0].Cells[0, 0].Value = "WebCenter " + Global.rm.GetString("txtStatistics");
            Wbook.Worksheets[0].Cells[0, 0].Style.Font.Size = 18;
            Wbook.Worksheets[0].Cells[0, 0].Style.Font.Bold = true;
            Wbook.Worksheets[0].Rows[0].Height = 30;

            Wbook.Worksheets[0].Cells[2, 0].Value = Global.rm.GetString("txtProduct");
            Wbook.Worksheets[0].Cells[2, 0].Style.Font.Bold = true;
            Wbook.Worksheets[0].Cells[2, 2].Value = title;
            Wbook.Worksheets[0].Cells[2, 2].Style.VerticalAlignment = TypeOfVAlignment.Top;

            int y = 4;
            Wbook.Worksheets[0].Cells[y, 0].Value = Global.rm.GetString("txtStatNumberOfPages");
            Wbook.Worksheets[0].Cells[y, 0].Style.Font.Bold = true;
            Wbook.Worksheets[0].Cells[y, 2].Value = nPages;           

            y+=2;

            if ((bool)Application["ReportShowDeadline"])
            {
                Wbook.Worksheets[0].Cells[y, 0].Value = Global.rm.GetString("txtDeadLine");
                Wbook.Worksheets[0].Cells[y, 0].Style.Font.Bold = true;
                if (deadLine.Year > 2000)
                {
                    Wbook.Worksheets[0].Cells[y, 2].Value = deadLine;
                    Wbook.Worksheets[0].Cells[y, 2].Style.StringFormat = (string)Application["ExcelTimeFormat"];
                }
                y+=2;
            }

            Wbook.Worksheets[0].Cells[y, 0].Value = Global.rm.GetString("txtStatPagesInput");
            Wbook.Worksheets[0].Cells[y, 0].Style.Font.Bold = true;
            Wbook.Worksheets[0].Cells[y, 2].Value = nPagesArrived;
            ++y;
            Wbook.Worksheets[0].Cells[y, 0].Value = Global.rm.GetString("txtStatFirstPageInput");
            Wbook.Worksheets[0].Cells[y, 0].Style.Font.Bold = true;
            if (tFirstArrivedPage.Year > 2000)
            {
                Wbook.Worksheets[0].Cells[y, 2].Value = tFirstArrivedPage;
                Wbook.Worksheets[0].Cells[y, 2].Style.StringFormat = (string)Application["ExcelTimeFormat"];
            }
            ++y;
            Wbook.Worksheets[0].Cells[y, 0].Value = Global.rm.GetString("txtStatLastPageInput");
            Wbook.Worksheets[0].Cells[y, 0].Style.Font.Bold = true;
            if (tLastArrivedPage.Year > 2000)
            {
                Wbook.Worksheets[0].Cells[y, 2].Value = tLastArrivedPage;
                Wbook.Worksheets[0].Cells[y, 2].Style.StringFormat = (string)Application["ExcelTimeFormat"];
            }
            y += 2;
            Wbook.Worksheets[0].Cells[y, 0].Value = Global.rm.GetString("txtStatPagesApproved");
            Wbook.Worksheets[0].Cells[y, 0].Style.Font.Bold = true;
            Wbook.Worksheets[0].Cells[y, 2].Value = nPagesApproved;
            ++y;
            Wbook.Worksheets[0].Cells[y, 0].Value = Global.rm.GetString("txtStatFirstPageApproved");
            Wbook.Worksheets[0].Cells[y, 0].Style.Font.Bold = true;
            if (tFirstApprovedPage.Year > 2000)
            {
                Wbook.Worksheets[0].Cells[y, 2].Value = tFirstApprovedPage;
                Wbook.Worksheets[0].Cells[y, 2].Style.StringFormat = (string)Application["ExcelTimeFormat"];
            }
            ++y;
            Wbook.Worksheets[0].Cells[y, 0].Value = Global.rm.GetString("txtStatLastPageApproved");
            Wbook.Worksheets[0].Cells[y, 0].Style.Font.Bold = true;
            if (tLastApprovedPage.Year > 2000)
            {
                Wbook.Worksheets[0].Cells[y, 2].Value = tLastApprovedPage;
                Wbook.Worksheets[0].Cells[y, 2].Style.StringFormat = (string)Application["ExcelTimeFormat"];
            }
            y += 2;

            Wbook.Worksheets[0].Cells[y, 0].Value = Global.rm.GetString("txtStatPagesOutput");
            Wbook.Worksheets[0].Cells[y, 0].Style.Font.Bold = true;
            Wbook.Worksheets[0].Cells[y, 2].Value = nPagesOutput;
            ++y;
            Wbook.Worksheets[0].Cells[y, 0].Value = Global.rm.GetString("txtStatFirstPageOutput");
            Wbook.Worksheets[0].Cells[y, 0].Style.Font.Bold = true;
            if (tFirstOutputPage.Year > 2000)
            {
                Wbook.Worksheets[0].Cells[y, 2].Value = tFirstOutputPage;
                Wbook.Worksheets[0].Cells[y, 2].Style.StringFormat = (string)Application["ExcelTimeFormat"];
            }
            ++y;
            Wbook.Worksheets[0].Cells[y, 0].Value = Global.rm.GetString("txtStatLastPageOutput");
            Wbook.Worksheets[0].Cells[y, 0].Style.Font.Bold = true;
            if (tLastOutputPage.Year > 2000)
            {
                Wbook.Worksheets[0].Cells[y, 2].Value = tLastOutputPage;
                Wbook.Worksheets[0].Cells[y, 2].Style.StringFormat = (string)Application["ExcelTimeFormat"];
            }
            y += 2;

            Wbook.Worksheets[0].Cells[y, 0].Value = Global.rm.GetString("txtStatNumberOfPlates");
            Wbook.Worksheets[0].Cells[y, 0].Style.Font.Bold = true;
            Wbook.Worksheets[0].Cells[y, 2].Value = nTotalPlates;
            ++y;
            Wbook.Worksheets[0].Cells[y, 0].Value = Global.rm.GetString("txtStatNumberOfPlatesOutput");
            Wbook.Worksheets[0].Cells[y, 0].Style.Font.Bold = true;
            Wbook.Worksheets[0].Cells[y, 2].Value = nTotalPlatesDone;
            ++y;
            Wbook.Worksheets[0].Cells[y, 0].Value = Global.rm.GetString("txtStatNumberOfPlatesUsed");
            Wbook.Worksheets[0].Cells[y, 0].Style.Font.Bold = true;
            Wbook.Worksheets[0].Cells[y, 2].Value = nTotalPlatesUsed;
            ++y;
            Wbook.Worksheets[0].Cells[y, 0].Value = Global.rm.GetString("txtStatNumberOfPlatesUpdated");
            Wbook.Worksheets[0].Cells[y, 0].Style.Font.Bold = true;
            Wbook.Worksheets[0].Cells[y, 2].Value = nTotalPlatesUpdated;

            ++y;
            Wbook.Worksheets[0].Cells[y, 0].Value = Global.rm.GetString("txtStatNumberOfPlatesDamaged");
            Wbook.Worksheets[0].Cells[y, 0].Style.Font.Bold = true;
            Wbook.Worksheets[0].Cells[y, 2].Value = nTotalPlatesDamaged;

         
            if ((bool)Application["ReportShowCMYKInk"])
            {
                y+=2;
                Wbook.Worksheets[0].Cells[y, 0].Value = "C ink mean (%)";
                Wbook.Worksheets[0].Cells[y, 0].Style.Font.Bold = true;
                Wbook.Worksheets[0].Cells[y, 2].Value = Cinkmean;
                if (Globals.InkUsagePlateImageSize > 0.0 && Globals.InkUsagePer1000Copies > 0.0)
                {
                    double f = ((double)nTotalPlatesUsed/4.0) * Globals.InkUsagePlateImageSize * (double)Cinkmean / 100.0;
                    Wbook.Worksheets[0].Cells[y, 4].Value = "C ink usage";
                    Wbook.Worksheets[0].Cells[y, 4].Style.Font.Bold = true;
                    Wbook.Worksheets[0].Cells[y, 5].Value = Globals.DoubleToString(f * Globals.InkUsagePer1000Copies, 2);
                    Wbook.Worksheets[0].Cells[y, 6].Value = "(gram pr. 1000 copies)";
                }

                ++y;
                Wbook.Worksheets[0].Cells[y, 0].Value = "M ink mean (%)";
                Wbook.Worksheets[0].Cells[y, 0].Style.Font.Bold = true;
                Wbook.Worksheets[0].Cells[y, 2].Value = Minkmean;
                if (Globals.InkUsagePlateImageSize > 0.0 && Globals.InkUsagePer1000Copies > 0.0)
                {
                    double f = ((double)nTotalPlatesUsed / 4.0) * Globals.InkUsagePlateImageSize * (double)Minkmean / 100.0;
                    Wbook.Worksheets[0].Cells[y, 4].Value = "M ink usage";
                    Wbook.Worksheets[0].Cells[y, 4].Style.Font.Bold = true;
                    Wbook.Worksheets[0].Cells[y, 5].Value = Globals.DoubleToString(f * Globals.InkUsagePer1000Copies, 2);
                    Wbook.Worksheets[0].Cells[y, 6].Value = "(gram pr. 1000 copies)";
                   
                }
                ++y;
                Wbook.Worksheets[0].Cells[y, 0].Value = "Y ink mean (%)";
                Wbook.Worksheets[0].Cells[y, 0].Style.Font.Bold = true;
                Wbook.Worksheets[0].Cells[y, 2].Value = Yinkmean;
                if (Globals.InkUsagePlateImageSize > 0.0 && Globals.InkUsagePer1000Copies > 0.0)
                {
                    double f = ((double)nTotalPlatesUsed / 4.0) * Globals.InkUsagePlateImageSize * (double)Yinkmean / 100.0;
                    Wbook.Worksheets[0].Cells[y, 4].Value = "Y ink usage";
                    Wbook.Worksheets[0].Cells[y, 4].Style.Font.Bold = true;
                    Wbook.Worksheets[0].Cells[y, 5].Value = Globals.DoubleToString(f * Globals.InkUsagePer1000Copies, 2);
                    Wbook.Worksheets[0].Cells[y, 6].Value = "(gram pr. 1000 copies)";
                }
                ++y;
                Wbook.Worksheets[0].Cells[y, 0].Value = "K ink mean (%)";
                Wbook.Worksheets[0].Cells[y, 0].Style.Font.Bold = true;
                Wbook.Worksheets[0].Cells[y, 2].Value = Kinkmean;
                if (Globals.InkUsagePlateImageSize > 0.0 && Globals.InkUsagePer1000Copies > 0.0)
                {
                    double f = ((double)nTotalPlatesUsed / 4.0) * Globals.InkUsagePlateImageSize * (double)Kinkmean / 100.0;
                    Wbook.Worksheets[0].Cells[y, 4].Value = "K ink usage";
                    Wbook.Worksheets[0].Cells[y, 4].Style.Font.Bold = true;
                    Wbook.Worksheets[0].Cells[y, 5].Value = Globals.DoubleToString(f * Globals.InkUsagePer1000Copies, 2);
                    Wbook.Worksheets[0].Cells[y, 6].Value = "(gram pr. 1000 copies)";
                }

            }

            y += 2;
            int x = 0;

            // table headers

            int xwidthnarrow = 50;
            int xwidthnormal = 80;
            int xwidthwide = 130;
            int xwidthverywide = 160;

            if (CheckBoxOrderByPlate.Checked)
            {
                Wbook.Worksheets[0].Cells[y, x].Value = "Sheet";
                Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[0].Columns[x].Width = xwidthnormal;
                x++;
                Wbook.Worksheets[0].Cells[y, x].Value = "SheetSide";
                Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[0].Columns[x].Width = xwidthnormal;
                x++;
            }

            if (hideEdition == false)
            {
                Wbook.Worksheets[0].Cells[y, x].Value = Global.rm.GetString("txtEdition");
                Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[0].Columns[x].Width = xwidthnormal;
                x++;
            }

            if (hideSection == false)
            {
                Wbook.Worksheets[0].Cells[y, x].Value = Global.rm.GetString("txtSection");
                Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[0].Columns[x].Width = xwidthnormal;
                x++;
            }

            Wbook.Worksheets[0].Cells[y, x].Value = Global.rm.GetString("txtPage");
            Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
            Wbook.Worksheets[0].Columns[x].Width = xwidthnarrow;
            x++;

            Wbook.Worksheets[0].Columns[2].Width = xwidthwide;

            Wbook.Worksheets[0].Cells[y, x].Value = Global.rm.GetString("txtFinalVersion");
            Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
            Wbook.Worksheets[0].Columns[x].Width = xwidthnormal;
            x++;

            Wbook.Worksheets[0].Cells[y, x].Value = Global.rm.GetString("txtStatus");
            Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
            Wbook.Worksheets[0].Columns[x].Width = xwidthnormal;
            x++;

            Wbook.Worksheets[0].Cells[y, x].Value = Global.rm.GetString("txtInputTime");
            Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
            Wbook.Worksheets[0].Columns[x].Style.HorizontalAlignment = TypeOfHAlignment.Right;
            Wbook.Worksheets[0].Cells[y, x].Style.HorizontalAlignment = TypeOfHAlignment.Left;
            Wbook.Worksheets[0].Columns[x].Width = xwidthwide;           
            x++;

            if ((bool)Application["ReportShowAfterDeadline"])
            {
                Wbook.Worksheets[0].Cells[y, x].Value = Global.rm.GetString("txtInputTimeAfterDeadline");
                Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[0].Columns[x].Style.HorizontalAlignment = TypeOfHAlignment.Right;
                Wbook.Worksheets[0].Cells[y, x].Style.HorizontalAlignment = TypeOfHAlignment.Left;
                Wbook.Worksheets[0].Columns[x].Width = xwidthwide+30;
                x++;
            }

            if ((bool)Application["ReportShowViewTime"])
            {
                Wbook.Worksheets[0].Cells[y, x].Value = Global.rm.GetString("txtViewTime");
                Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[0].Columns[x].Style.HorizontalAlignment = TypeOfHAlignment.Right;
                Wbook.Worksheets[0].Cells[y, x].Style.HorizontalAlignment = TypeOfHAlignment.Left;
                Wbook.Worksheets[0].Columns[x].Width = xwidthwide;
                x++;
                Wbook.Worksheets[0].Cells[y, x].Value = Global.rm.GetString("txtViewedBy");
                Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[0].Columns[x].Width = xwidthnormal;
                x++;
            }

            Wbook.Worksheets[0].Cells[y, x].Value = Global.rm.GetString("txtApproval");
            Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
            Wbook.Worksheets[0].Columns[x].Width = xwidthnormal;
            x++;
            Wbook.Worksheets[0].Cells[y, x].Value = Global.rm.GetString("txtApprovedBy");
            Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
            Wbook.Worksheets[0].Columns[x].Width = xwidthnormal;
            x++;
            Wbook.Worksheets[0].Cells[y, x].Value = Global.rm.GetString("txtApproveTime");
            Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
            Wbook.Worksheets[0].Columns[x].Style.HorizontalAlignment = TypeOfHAlignment.Right;
            Wbook.Worksheets[0].Cells[y, x].Style.HorizontalAlignment = TypeOfHAlignment.Left;
            Wbook.Worksheets[0].Columns[x].Width = xwidthwide;
            x++;

            if ((bool)Application["ReportShowReadyTime"])
            {
                Wbook.Worksheets[0].Cells[y, x].Value = Global.rm.GetString("txtPlateReadyTime");
                Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[0].Columns[x].Style.HorizontalAlignment = TypeOfHAlignment.Right;
                Wbook.Worksheets[0].Cells[y, x].Style.HorizontalAlignment = TypeOfHAlignment.Left;
                Wbook.Worksheets[0].Columns[x].Width = xwidthwide;
                x++;
            }

            Wbook.Worksheets[0].Cells[y, x].Value = (bool)Application["LocationIsPress"] /* && (bool)Application["UsePressGroups"] == false */? Global.rm.GetString("txtTransmitted") : Global.rm.GetString("txtOutput");
            Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
            Wbook.Worksheets[0].Columns[x].Width = xwidthwide;
            x++;

            string msgstring = Global.rm.GetString("txtMessage");
            if ((bool)Application["ExtendedThumbnailViewShowFTP"])
            {
                Wbook.Worksheets[0].Cells[y, x].Value = "FTP status";
                Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[0].Columns[x].Width = xwidthwide;
                x++;

                Wbook.Worksheets[0].Cells[y, x].Value = "FTP " + msgstring;
                Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[0].Columns[x].Width = xwidthverywide;
                Wbook.Worksheets[0].Cells[y, x].Style.WrapText = true;
                x++;                
            }
            if ((bool)Application["ExtendedThumbnailViewShowPreflight"])
            {
                Wbook.Worksheets[0].Cells[y, x].Value = "Preflight status";
                Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[0].Columns[x].Width = xwidthwide;
                x++;

                Wbook.Worksheets[0].Cells[y, x].Value = "Preflight " + msgstring;
                Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[0].Columns[x].Width = xwidthverywide;
                Wbook.Worksheets[0].Cells[y, x].Style.WrapText = true;
                x++;
            }
            if ((bool)Application["ExtendedThumbnailViewShowRIP"])
            {
                Wbook.Worksheets[0].Cells[y, x].Value = "RIP status";
                Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[0].Columns[x].Width = xwidthwide;
                x++;

                Wbook.Worksheets[0].Cells[y, x].Value = "RIP " + msgstring;
                Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[0].Columns[x].Width = xwidthverywide;
                Wbook.Worksheets[0].Cells[y, x].Style.WrapText = true;
                x++;
            }
            if ((bool)Application["ExtendedThumbnailViewShowColorWarning"])
            {
                Wbook.Worksheets[0].Cells[y, x].Value = "Color status";
                Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[0].Columns[x].Width = xwidthwide;
                x++;

                Wbook.Worksheets[0].Cells[y, x].Value = "Color " + msgstring;
                Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[0].Columns[x].Width = xwidthverywide;
                Wbook.Worksheets[0].Cells[y, x].Style.WrapText = true;
                x++;
            }

            if ((bool)Application["ReportShowCMYKInk"])
            {
                Wbook.Worksheets[0].Cells[y, x].Value = "C ink";
                Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[0].Columns[x].Width = xwidthnormal;
                x++;
                Wbook.Worksheets[0].Cells[y, x].Value = "M ink";
                Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[0].Columns[x].Width = xwidthnormal;
                x++;
                Wbook.Worksheets[0].Cells[y, x].Value = "Y ink";
                Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[0].Columns[x].Width = xwidthnormal;
                x++;
                Wbook.Worksheets[0].Cells[y, x].Value = "K ink";
                Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[0].Columns[x].Width = xwidthnormal;
                x++;
           }
            y++;

            if (dtab.Rows.Count > 0)
                Wbook.Worksheets[0].ReadFromDataTable(y, 0, dtab, false, false, true);

            y += dtab.Rows.Count + 2;

            string chart1filename = Request.PhysicalApplicationPath + (string)Session["UserName"] + "_chart.jpg";
            if (SaveChart1(chart1filename, chart.Width, chart.Height))
            {
                Wbook.Worksheets[0].Pictures.Add(chart1filename);
                Wbook.Worksheets[0].Pictures[0].SetPosition(y, 1);
            }


            if ((bool)Application["IncludePageHistory"])
            {

                DataTable dtHistory = new DataTable();
                DataColumn newColumn;
                if (hideEdition == false)
                    newColumn = dtHistory.Columns.Add("Edition", Type.GetType("System.String"));
                if (hideSection == false)
                    newColumn = dtHistory.Columns.Add("Section", Type.GetType("System.String"));

                newColumn = dtHistory.Columns.Add("Page", Type.GetType("System.Int32"));
                newColumn = dtHistory.Columns.Add("Version", Type.GetType("System.Int32"));
                newColumn = dtHistory.Columns.Add("InputTime", Type.GetType("System.String"));
                newColumn = dtHistory.Columns.Add("Approved", Type.GetType("System.String"));
                newColumn = dtHistory.Columns.Add("ApproveUser", Type.GetType("System.String"));
                newColumn = dtHistory.Columns.Add("ApproveTime", Type.GetType("System.String"));
                newColumn = dtHistory.Columns.Add("Message", Type.GetType("System.String"));

                Wbook.Worksheets.Add(Global.rm.GetString("txtPageHistory"));

                Wbook.Worksheets[1].Cells[0, 0].Value = Global.rm.GetString("txtPageHistory");
                Wbook.Worksheets[1].Cells[0, 0].Style.Font.Bold = true;
                Wbook.Worksheets[1].Cells[0, 0].Style.Font.Size = 18;
                Wbook.Worksheets[1].Rows[0].Height = 30;

                y = 3;
                x = 0;

                if (hideEdition == false)
                {
                    Wbook.Worksheets[1].Cells[y, x].Value = Global.rm.GetString("txtEdition");
                    Wbook.Worksheets[1].Cells[y, x].Style.Font.Bold = true;
                    Wbook.Worksheets[1].Columns[x].Width = xwidthnormal;
                    x++;
                }

                if (hideSection == false)
                {
                    Wbook.Worksheets[1].Cells[y, x].Value = Global.rm.GetString("txtSection");
                    Wbook.Worksheets[1].Cells[y, x].Style.Font.Bold = true;
                    Wbook.Worksheets[1].Columns[x].Width = xwidthnormal;
                    x++;
                }
                Wbook.Worksheets[1].Cells[y, x].Value = Global.rm.GetString("txtPage");
                Wbook.Worksheets[1].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[1].Columns[x].Width = xwidthnarrow;
                x++;

                Wbook.Worksheets[1].Cells[y, x].Value = Global.rm.GetString("txtVersion");
                Wbook.Worksheets[1].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[1].Columns[x].Width = xwidthnormal;
                x++;

                Wbook.Worksheets[1].Cells[y, x].Value = Global.rm.GetString("txtInputTime");
                Wbook.Worksheets[1].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[1].Columns[x].Width = xwidthwide;
                x++;

                Wbook.Worksheets[1].Cells[y, x].Value = Global.rm.GetString("txtApproval");
                Wbook.Worksheets[1].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[1].Columns[x].Width = xwidthwide;
                x++;

                Wbook.Worksheets[1].Cells[y, x].Value = Global.rm.GetString("txtApprovedBy");
                Wbook.Worksheets[1].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[1].Columns[x].Width = xwidthnormal;
                x++;

                Wbook.Worksheets[1].Cells[y, x].Value = Global.rm.GetString("txtApproveTime");
                Wbook.Worksheets[1].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[1].Columns[x].Width = xwidthwide;
                x++;

                Wbook.Worksheets[1].Cells[y, x].Value = "";
                Wbook.Worksheets[1].Cells[y, x].Style.Font.Bold = true;
                Wbook.Worksheets[1].Columns[x].Width = xwidthwide;
                x++;
                y++;

                DataRow newRow = null;
                ArrayList editionList = db.GetEditionsInProduction(publicationID, pubDate, out errmsg);

                foreach (int editionID in editionList)
                {
                    List<int> sectionList = db.GetSectionsInEdition(publicationID, pubDate, editionID, out errmsg);

                    foreach (int sectionID in sectionList)
                    {
                        ArrayList pageList = db.GetPagesInSection(publicationID, pubDate, editionID, sectionID, out errmsg);

                        int pageIndex = 0;

                        PageHistory[] versionHistory = new PageHistory[100];
                        for (int i = 0; i < versionHistory.Length; i++)
                            versionHistory[i] = new PageHistory();
                        //                        PageHistory[] versionHistory = new PageHistory[100];

                        foreach (string pageName in pageList)
                        {
                            pageIndex++;
                            for (int i = 0; i < 100; i++)
                            {
                                versionHistory[i].version = 0;
                                versionHistory[i].approveState = -1;
                            }

                            //	int versions = db.GetPageHistory(ref versionHistory, job, editionID, sectionID,pageName, deadLine, out errmsg);

                            int versions = db.GetPageHistoryExcel(ref versionHistory, publicationID, pubDate, editionID, sectionID, pageName, deadLine, out errmsg);
                            if (versions == 0)
                                continue;

                            for (int v = 0; v < versions; v++)
                            {
                                if (versionHistory[v].version == 0)
                                    continue;

                                newRow = dtHistory.NewRow();

                                if (hideEdition == false)
                                    newRow["Edition"] = Globals.GetNameFromID("EditionNameCache", editionID);
                                if (hideSection == false)
                                    newRow["Section"] = Globals.GetNameFromID("SectionnameCache", sectionID);

                                if (Globals.TryParse(pageName, 0) == 0)
                                    newRow["Page"] = pageIndex;
                                else
                                    newRow["Page"] = Globals.TryParse(pageName, 0);

                                newRow["Version"] = versionHistory[v].version;

                                newRow["InputTime"] = versionHistory[v].inputTime.Year >= 2000 ? versionHistory[v].inputTime.ToString() : ""; ;

                                newRow["ApproveTime"] = versionHistory[v].approveTime.Year >= 2000 ? versionHistory[v].approveTime.ToString() : "";
                                switch (versionHistory[v].approveState)
                                {
                                    case 2:
                                        newRow["Approved"] = Global.rm.GetString("txtRejected");
                                        break;
                                    case 1:
                                        newRow["Approved"] = Global.rm.GetString("txtApproved");
                                        break;
                                    default:
                                        newRow["Approved"] = "";
                                        break;
                                }
                                newRow["ApproveUser"] = versionHistory[v].approveUser;
                                newRow["Message"] = versionHistory[v].message;

                                dtHistory.Rows.Add(newRow);
                            }
                        }
                    }
                }
                if (dtab.Rows.Count > 0)
                    Wbook.Worksheets[1].ReadFromDataTable(y, 0, dtHistory, false, false, false);
            }

            string excelName = title + "_report";
            if ((int)Application["ExcelSavetype"] == 0)
                excelName += ".xls";
            else
                excelName += ".xlsx";

            excelName = excelName.Replace("æ", "ae");
            excelName = excelName.Replace("ä", "ae");
            excelName = excelName.Replace("Æ", "AE");
            excelName = excelName.Replace("Ä", "AE");
            excelName = excelName.Replace("ø", "oe");
            excelName = excelName.Replace("ö", "oe");
            excelName = excelName.Replace("Ø", "OE");
            excelName = excelName.Replace("Ö", "OE");
            excelName = excelName.Replace("å", "aa");
            excelName = excelName.Replace("ä", "aa");
            excelName = excelName.Replace("Å", "AA");
            excelName = excelName.Replace("Ä", "AA");

            try
            {
                Encoding ascii = Encoding.ASCII;
                Encoding unicode = Encoding.Unicode;
                byte[] unicodeBytes = unicode.GetBytes(excelName);
                byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);
                char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
                ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
                string asciiString = new string(asciiChars);

                // XLS
                // Save Excel Workbook (XLS) to MemoryStream

                if ((int)Application["ExcelSavetype"] == 0)
                {
                    byte[] bytes = Wbook.WriteXLS().ToArray();

                    // Show Excel (XLS) in Browser window without saving on disk
                    Response.Buffer = true;

                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + asciiString);
                    Response.AppendHeader("Content-Length", bytes.Length.ToString());
                    Response.BinaryWrite(bytes);
                    Response.Flush();
                    Response.End();
                }
                else
                {
                    byte[] bytes = Wbook.WriteXLSX().ToArray();

                    // Show Open Office (XLSX) in Browser window without saving on disk
                    Response.Buffer = true;
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + asciiString);
                    Response.AppendHeader("Content-Length", bytes.Length.ToString());
                    Response.BinaryWrite(bytes);
                    Response.Flush();
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }

        private int maxHoursToShow = 0;
		private int margin = 60;
		private bool SaveChart1(string fileName, int chartWidth, int chartHeight)
		{


            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);
            bool ret = true;

            CCDBaccess db = new CCDBaccess();
            string errmsg = "";

            DateTime startInputTime = DateTime.MaxValue;
            DateTime stopInputTime = DateTime.MinValue;

            DateTime startApproveTime = DateTime.MaxValue;
            DateTime stopApproveTime = DateTime.MinValue;

            DateTime startOutputTime = DateTime.MaxValue;
            DateTime stopOutputTime = DateTime.MinValue;

            int maxYsample = 0;
            DateTime firstSampleTime = DateTime.MaxValue;
            DateTime lastSampleTime = DateTime.MinValue;

            DataTable dtInput = null;
            DataTable dtApprove = null;
            DataTable dtOutput = null;

            if ((bool)Session["HideInputTime"] == false)
            {
                dtInput = db.GetStatCollection(1, out errmsg);
                if (dtInput != null)
                {
                    if (dtInput.Rows.Count == 0)
                    {
                        dtInput = null;
                    }
                    else
                    {
                        if (maxYsample < dtInput.Rows.Count)
                            maxYsample = dtInput.Rows.Count;
                        foreach (DataRow row in dtInput.Rows)
                        {
                            DateTime dt = (DateTime)row["Time"];
                            if (dt.Year <= 2000)
                                continue;

                            if (dt < startInputTime)
                                startInputTime = dt;

                            if (dt > stopInputTime)
                                stopInputTime = dt;
                        }

                        if (firstSampleTime > startInputTime)
                            firstSampleTime = startInputTime;
                        if (lastSampleTime < stopInputTime)
                            lastSampleTime = stopInputTime;
                    }
                }
            }

            if ((bool)Session["HideOutputTime"] == false)
            {
                dtOutput = db.GetStatCollection(3, out errmsg);
                if (dtOutput != null)
                {
                    if (dtOutput.Rows.Count == 0)
                    {
                        dtOutput = null;
                    }
                    else
                    {
                        if (maxYsample < dtOutput.Rows.Count)
                            maxYsample = dtOutput.Rows.Count;
                        foreach (DataRow row in dtOutput.Rows)
                        {
                            DateTime dt = (DateTime)row["Time"];
                            if (dt.Year <= 2000)
                                continue;

                            if (dt < startOutputTime)
                                startOutputTime = dt;

                            if (dt > stopOutputTime)
                                stopOutputTime = dt;
                        }
                        if (firstSampleTime > startOutputTime)
                            firstSampleTime = startOutputTime;
                        if (lastSampleTime < stopOutputTime)
                            lastSampleTime = stopOutputTime;
                    }
                }

            }

            if ((bool)Session["HideApproveTime"] == false)
            {
                dtApprove = db.GetStatCollection(2, out errmsg);
                if (dtApprove != null)
                {
                    if (dtApprove.Rows.Count == 0)
                    {
                        dtApprove = null;
                    }
                    else
                    {
                        if (maxYsample < dtApprove.Rows.Count)
                            maxYsample = dtApprove.Rows.Count;
                        foreach (DataRow row in dtApprove.Rows)
                        {
                            DateTime dt = (DateTime)row["Time"];
                            if (dt.Year <= 2000)
                                continue;

                            if (dt < startInputTime)
                            {
                                dt = startInputTime;
                                row["Time"] = dt;
                            }
                            if (dt > stopOutputTime)
                                dt = stopOutputTime;
                        }

                        if (startApproveTime < startInputTime)
                            startApproveTime = startInputTime;

                        if (stopApproveTime > stopOutputTime)
                            stopApproveTime = stopOutputTime;

                        if (firstSampleTime > startApproveTime)
                            firstSampleTime = startApproveTime;
                        if (lastSampleTime < stopApproveTime)
                            lastSampleTime = stopApproveTime;
                    }


                }
            }

            SolidBrush graybrush = new SolidBrush(Color.LightGray);
            SolidBrush bgbrush = new SolidBrush(Color.FromArgb(240, 244, 248));
            SolidBrush whitebrush = new SolidBrush(Color.White);
            SolidBrush blackbrush = new SolidBrush(Color.Black);
            SolidBrush redbrush = new SolidBrush(Color.Magenta);
            SolidBrush greenbrush = new SolidBrush(Color.Orange);
            SolidBrush greenbrush2 = new SolidBrush(Color.Green);
            SolidBrush bluebrush = new SolidBrush(Color.Blue);

            Pen redpen = new Pen(Color.Magenta, 1);
            Pen greenpen = new Pen(Color.Orange, 1);
            Pen greenpen2 = new Pen(Color.Green, 1);
            Pen bluepen = new Pen(Color.Blue, 1);
            Pen lightgraypen = new Pen(Color.LightGray);

            Pen whitepen = new Pen(Color.White, 1);
            Pen blackpen = new Pen(Color.Black, 1);
            System.Drawing.Font fontLegend = new System.Drawing.Font("Verdana", 10),
                fontTitle = new System.Drawing.Font("Verdana", 15, FontStyle.Bold);

            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, chartWidth, chartHeight);

            Bitmap bitmap = new Bitmap(chartWidth, chartHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bitmap);

            // Background
            g.FillRectangle(bgbrush, rect);

            // Draw axis
            g.DrawLine(blackpen, margin, chartHeight - margin + 10, margin, margin);
            g.DrawLine(blackpen, margin - 10, chartHeight - margin, chartWidth - margin + 10, chartHeight - margin);

            // Draw axis titles
            g.DrawString(Global.rm.GetString("txtPages"), fontLegend, blackbrush, margin - 20, margin - 10 - fontLegend.Height);
            g.DrawString(Global.rm.GetString("txtTime"), fontLegend, blackbrush, chartWidth - margin + 20, chartHeight - margin - fontLegend.Height / 2);

            // Draw legend
            g.DrawString(Global.rm.GetString("txtInput"), fontLegend, greenbrush, 170, margin - 10 - fontLegend.Height);
            g.FillRectangle(greenbrush, (int)160, margin - 10 - fontLegend.Height / 2, 3, 3);
            g.DrawString(Global.rm.GetString("txtApproval"), fontLegend, bluebrush, 300, margin - 10 - fontLegend.Height);
            g.FillRectangle(bluebrush, (int)290, margin - 10 - fontLegend.Height / 2, 3, 3);
            g.DrawString(Global.rm.GetString("txtOutput"), fontLegend, redbrush, 430, margin - 10 - fontLegend.Height);
            g.FillRectangle(redbrush, (int)420, margin - 10 - fontLegend.Height / 2, 3, 3);
            g.DrawString(Global.rm.GetString("txtDeadLine"), fontLegend, greenbrush2, 560, margin - 10 - fontLegend.Height);
            g.FillRectangle(greenbrush2, (int)550, margin - 10 - fontLegend.Height / 2, 3, 3);

            TimeSpan ts1 = new TimeSpan();

            if (firstSampleTime == DateTime.MaxValue || lastSampleTime == DateTime.MinValue)
            {
                // no data
                dtInput = null;
                dtApprove = null;
                dtOutput = null;
                Global.logging.WriteLog("SaveChart1: no data..");
            }

            // Draw y-ticks
            int maxYsample2 = maxYsample;
            if (maxYsample < 100)
            {
                maxYsample2 = (maxYsample + 9) / 10;
                maxYsample2 *= 10;
            }
            maxYsample = maxYsample2;

            if (dtInput == null && dtApprove == null && dtOutput == null)
                maxYsample = 0;

            if (maxYsample > 0)
            {
                double ystepscale = (double)(chartHeight - 2 * margin - 10) / (double)maxYsample;
                TimeSpan ts = new TimeSpan();
                ts = lastSampleTime - firstSampleTime;


                if (maxHoursToShow > 0)
                {
                    if (ts.TotalHours > maxHoursToShow)
                    {
                        firstSampleTime = lastSampleTime.AddHours(-1.0 * (double)maxHoursToShow);
                    }
                    ts = lastSampleTime - firstSampleTime;
                }

                double xstepscale = (double)(chartWidth - 2 * margin /*- 10 */);
                if (ts.TotalSeconds > 0)
                    xstepscale = (double)(chartWidth - 2 * margin /*- 10 */) / ts.TotalSeconds;

                DateTime firstWholeHour = new DateTime(firstSampleTime.Year, firstSampleTime.Month, firstSampleTime.Day, firstSampleTime.Hour, 0, 0);
                firstWholeHour.AddHours(1);


                // Draw x-ticks
                int inc = 1;
                if (ts.TotalHours > 24.0)
                    inc = (int)ts.TotalHours / 24 + 1;

                double prevxx = 0;
                int day = 0;
                int month = 0;


                for (int i = 0; i < ts.TotalHours; i += inc)
                {
                    Global.logging.WriteLog(string.Format("SaveChart1: i {0}", i));
                    DateTime tx = firstWholeHour.AddHours(i);
                    ts1 = tx - firstSampleTime;

                    double x = ts1.TotalSeconds * xstepscale;
                    g.DrawLine(blackpen, (int)x + margin, chartHeight - margin + 3, (int)x + margin, chartHeight - margin - 3);

                    if (x - prevxx > 50)
                    {
                        //					if (i%(10*inc) == 0) 
                        g.DrawString(tx.Hour.ToString("00") + ":" + tx.Minute.ToString("00"), fontLegend, blackbrush, (int)x + margin - 15, chartHeight - margin - 10 + fontLegend.Height);
                        g.DrawLine(blackpen, (int)x + margin, chartHeight - margin + 6, (int)x + margin, chartHeight - margin - 6);
                        if (tx.Day > day || tx.Month > month)
                        {
                            g.DrawString(tx.Day.ToString("00") + "." + tx.Month.ToString("00"), fontLegend, blackbrush, (int)x + margin - 15, chartHeight - margin + 5 + fontLegend.Height);
                            day = tx.Day;
                            month = tx.Month;
                        }
                        prevxx = x;
                    }
                }

                Global.logging.WriteLog("SaveChart1: debug 2");

                double stepsbetweenticks = (double)maxYsample / 10.0;
                for (double i = 1; i <= 10; i++)
                {
                    double y = (double)(chartHeight - margin) - (double)(i * stepsbetweenticks) * ystepscale;

                    g.DrawLine(blackpen, margin - 3, (int)y, margin + 3, (int)y);
                    int yn = (int)(i * stepsbetweenticks);

                    g.DrawString(yn.ToString(), fontLegend, blackbrush, margin - 30, (int)y - (int)(fontLegend.Height / 2.0));
                    if (i % 2 == 0)
                        g.DrawLine(lightgraypen, margin + 4, (int)y, (int)(chartWidth - margin + 10), (int)y);

                }

                Global.logging.WriteLog("SaveChart1: debug 3");

                DateTime deadline = DateTime.MinValue;
                if (dtInput != null)
                {
                    int pages = 0;
                    double prevx = -1.0;
                    double prevy = -1.0;

                    DataView view = new DataView(dtInput, "", "Time", DataViewRowState.CurrentRows);

                    foreach (DataRow row in view.Table.Rows)
                    {
                        deadline = (DateTime)row["DeadLine"];

                        DateTime dt = (DateTime)row["Time"];
                        if (dt.Year <= 2000)
                            continue;

                        //		if ((int)row["MinStatus"] < 10)
                        //			continue;

                        pages++;

                        if (dt < firstSampleTime)
                            continue;

                        ts1 = lastSampleTime - dt;
                        double x = (double)(chartWidth - margin) - ts1.TotalSeconds * xstepscale;
                        double y = (double)(chartHeight - margin) - (double)pages * ystepscale;
                        //g.DrawEllipse(greenpen,(int)x, (int)y,3,3);
                        g.FillRectangle(greenbrush, (int)x - 1, (int)y - 1, 3, 3);
                        if (prevx > 0)
                        {
                            g.DrawLine(greenpen, (int)prevx, (int)prevy, (int)x, (int)y);
                        }
                        prevx = x;
                        prevy = y;

                    }
                    int xx = pages;
                }
                if (deadline.Year > 2000)
                {
                    if (deadline > firstSampleTime && deadline < lastSampleTime)
                    {
                        ts1 = lastSampleTime - deadline;
                        double x = (double)(chartWidth - margin) - ts1.TotalSeconds * xstepscale;

                        g.DrawLine(greenpen2, (int)x, (int)margin, (int)x, (int)chartHeight - margin);

                    }
                }

                if (dtApprove != null)
                {
                    int pages = 0;
                    double prevx = -1.0;
                    double prevy = -1.0;

                    DataView view = new DataView(dtApprove, "", "Time", DataViewRowState.CurrentRows);
                    foreach (DataRow row in view.Table.Rows)
                    {
                        DateTime dt = (DateTime)row["Time"];
                        if (dt.Year <= 2000)
                            continue;

                        //		if ((int)row["MinStatus"] < 10 || (int)row["MinApprove"] < 1 )
                        //			continue;

                        pages++;

                        if (dt < firstSampleTime)
                            continue;

                        ts1 = lastSampleTime - dt;
                        double x = (double)(chartWidth - margin) - ts1.TotalSeconds * xstepscale;
                        double y = (double)(chartHeight - margin) - (double)pages * ystepscale;
                        //g.DrawEllipse(bluepen,(int)x, (int)y,3,3);

                        g.FillRectangle(bluebrush, (int)x - 1, (int)y - 1, 3, 3);
                        if (prevx > 0)
                        {
                            g.DrawLine(bluepen, (int)prevx, (int)prevy, (int)x, (int)y);
                        }
                        prevx = x;
                        prevy = y;

                    }
                    int xx = pages;
                }

                if (dtOutput != null)
                {
                    int pages = 0;
                    double prevx = -1.0;
                    double prevy = -1.0;

                    DataView view = new DataView(dtOutput, "", "Time", DataViewRowState.CurrentRows);
                    foreach (DataRow row in view.Table.Rows)
                    {
                        DateTime dt = (DateTime)row["Time"];
                        if (dt.Year <= 2000)
                            continue;

                        //if ((int)row["MinStatus"] < 50 || (int)row["MinStatus"] == 56 || (int)row["MinApprove"] < 1 )
                        //	continue;

                        pages++;

                        if (dt < firstSampleTime)
                            continue;

                        ts1 = lastSampleTime - dt;
                        double x = (double)(chartWidth - margin) - ts1.TotalSeconds * xstepscale;
                        double y = (double)(chartHeight - margin) - (double)pages * ystepscale;
                        //g.DrawEllipse(redpen,(int)x, (int)y,3,3);

                        g.FillRectangle(redbrush, (int)x - 1, (int)y - 1, 3, 3);
                        if (prevx > 0)
                        {
                            g.DrawLine(redpen, (int)prevx, (int)prevy, (int)x, (int)y);
                        }
                        prevx = x;
                        prevy = y;
                    }
                    int xx = pages;
                }
            }

            try
            {
                bitmap.Save(fileName, ImageFormat.Jpeg);
            }
            catch // (Exception ex)
            {
                lblError.Text = errmsg;
                Global.logging.WriteLog("SaveChart1: exception: " + errmsg);
                ret = false;
            }

            g.Dispose();
            bitmap.Dispose();

            return ret;
        }


		private bool SaveChart2(string fileName, int chartWidth, int chartHeight)
		{
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            CCDBaccess db = new CCDBaccess();
			string errmsg = "";
			bool ret = true;

			DateTime startInputTime = DateTime.MaxValue;
			DateTime stopInputTime = DateTime.MinValue;
			

			DateTime startApproveTime = DateTime.MaxValue;
			DateTime stopApproveTime = DateTime.MinValue;

			DateTime startOutputTime = DateTime.MaxValue;
			DateTime stopOutputTime = DateTime.MinValue;

			int maxYsample = 0;
			DateTime firstSampleTime = DateTime.MaxValue;
			DateTime lastSampleTime = DateTime.MinValue;

			DataTable dtInput = null;
			DataTable dtApprove = null;
			DataTable dtOutput = null;

			if ((bool)Session["HideInputTime"] == false) 
			{
				dtInput = db.GetStatCollection(1, out errmsg);  
				if (dtInput != null) 
				{
					if (dtInput.Rows.Count == 0)
					{
						dtInput = null;
					}
					else
					{
						if (maxYsample < dtInput.Rows.Count)
							maxYsample = dtInput.Rows.Count;
						foreach (DataRow row in dtInput.Rows)
						{
							DateTime dt = (DateTime)row["Time"];
							if (dt.Year <= 2000)
								continue;

							if (dt < startInputTime)
								startInputTime = dt;

							if (dt > stopInputTime)
								stopInputTime = dt;
						}
				
						if (firstSampleTime > startInputTime)
							firstSampleTime = startInputTime;
						if (lastSampleTime < stopInputTime)
							lastSampleTime = stopInputTime;
					}
				}
			}
			

			if ((bool)Session["HideOutputTime"] == false) 
			{
				dtOutput = db.GetStatCollection(3, out errmsg);  
				if (dtOutput != null) 
				{
					if (dtOutput.Rows.Count == 0)
					{
						dtOutput = null;
					}
					else
					{
						if (maxYsample < dtOutput.Rows.Count)
							maxYsample = dtOutput.Rows.Count;
						foreach (DataRow row in dtOutput.Rows)
						{
							DateTime dt = (DateTime)row["Time"];
							if (dt.Year <= 2000)
								continue;

							if (dt < startOutputTime)
								startOutputTime = dt;

							if (dt > stopOutputTime)
								stopOutputTime = dt;
						}
						if (firstSampleTime > startOutputTime)
							firstSampleTime = startOutputTime;
						if (lastSampleTime < stopOutputTime)
							lastSampleTime = stopOutputTime;
					}
				}
			}

			if ((bool)Session["HideApproveTime"] == false) 
			{
				dtApprove = db.GetStatCollection(2, out errmsg);  
				if (dtApprove != null) 
				{
					if (dtApprove.Rows.Count == 0)
					{
						dtApprove = null;
					}
					else
					{
						if (maxYsample < dtApprove.Rows.Count)
							maxYsample = dtApprove.Rows.Count;
						foreach (DataRow row in dtApprove.Rows)
						{
							DateTime dt = (DateTime)row["Time"];
							if (dt.Year <= 2000)
								continue;

							if (dt < startApproveTime)
								startApproveTime = dt;

							if (dt > stopApproveTime)
								stopApproveTime = dt;
						}
						if (startApproveTime < startInputTime)
							startApproveTime = startInputTime;

						if (stopApproveTime > stopOutputTime)
							stopApproveTime = stopOutputTime;

						if (firstSampleTime > startApproveTime)
							firstSampleTime = startApproveTime;
						if (lastSampleTime < stopApproveTime)
							lastSampleTime = stopApproveTime;
					}
				}
			}

			if (firstSampleTime == DateTime.MaxValue || lastSampleTime == DateTime.MinValue)
			{
				// no data
				dtInput = null;
				dtApprove = null;
				dtOutput = null;
			}

			ArrayList inputpagesstarthour = new ArrayList();
			ArrayList approvepagesstarthour = new ArrayList();
			ArrayList outputpagesstarthour = new ArrayList();

			TimeSpan tsTotal = new TimeSpan();
			TimeSpan ts = new TimeSpan();
			TimeSpan ts1 = new TimeSpan();
			DateTime dtIntervalStart = new DateTime();
			DateTime dtIntervalEnd = new DateTime();

			if (dtInput != null || dtApprove != null || dtOutput != null)
			{
				tsTotal = lastSampleTime - firstSampleTime;

				for(int i=0; i<(int)tsTotal.TotalHours+1; i++)
				{
					inputpagesstarthour.Add(0);
					approvepagesstarthour.Add(0);
					outputpagesstarthour.Add(0);
				}				

				dtIntervalStart = firstSampleTime;
				dtIntervalEnd = firstSampleTime.AddHours(1.0);
			}	
			
			DateTime deadline = DateTime.MinValue;
			if (dtInput != null) 
			{
				foreach (DataRow row in dtInput.Rows)
				{
					deadline = (DateTime)row["DeadLine"];

					DateTime dt = (DateTime)row["Time"];
					if (dt.Year <= 2000)
						continue;

					ts = dt - firstSampleTime;

					if (dt >= dtIntervalStart && dt < dtIntervalEnd)
						inputpagesstarthour[(int)ts.TotalHours] = (int)inputpagesstarthour[(int)ts.TotalHours] + 1;

					if (dt >= dtIntervalEnd) 
					{
						dtIntervalStart = dtIntervalEnd;
						dtIntervalEnd = dtIntervalEnd.AddHours(1.0);
					}
				}
			}

			if (dtInput != null || dtApprove != null || dtOutput != null)
			{
				dtIntervalStart = firstSampleTime;
				dtIntervalEnd = firstSampleTime.AddHours(1.0);
			}	

			if (dtApprove != null) 
			{
				foreach (DataRow row in dtApprove.Rows)
				{
					DateTime dt = (DateTime)row["Time"];
					if (dt.Year <= 2000)
						continue;
					
					if (dt > stopOutputTime)
						dt = stopOutputTime;
					if (dt < startInputTime)
						dt = startInputTime;

					ts = dt - firstSampleTime;

					if (dt >= dtIntervalStart && dt < dtIntervalEnd)
						approvepagesstarthour[(int)ts.TotalHours] = (int)approvepagesstarthour[(int)ts.TotalHours] + 1;

					if (dt >= dtIntervalEnd) 
					{
						dtIntervalStart = dtIntervalEnd;
						dtIntervalEnd = dtIntervalEnd.AddHours(1.0);
					}
				}
			}

			if (dtInput != null || dtApprove != null || dtOutput != null)
			{
				dtIntervalStart = firstSampleTime;
				dtIntervalEnd = firstSampleTime.AddHours(1.0);
			}
	
			if (dtOutput != null) 
			{
				foreach (DataRow row in dtOutput.Rows)
				{
					DateTime dt = (DateTime)row["Time"];
					if (dt.Year <= 2000)
						continue;

					ts = dt - firstSampleTime;

					if (dt >= dtIntervalStart && dt < dtIntervalEnd)
						outputpagesstarthour[(int)ts.TotalHours] = (int)outputpagesstarthour[(int)ts.TotalHours] + 1;

					if (dt >= dtIntervalEnd) 
					{
						dtIntervalStart = dtIntervalEnd;
						dtIntervalEnd = dtIntervalEnd.AddHours(1.0);
					}

				}
			}
			
			SolidBrush graybrush = new SolidBrush(Color.LightGray);		
			SolidBrush bgbrush = new SolidBrush(Color.FromArgb(240, 244, 248));	
			SolidBrush whitebrush = new SolidBrush(Color.White);
			SolidBrush blackbrush = new SolidBrush(Color.Black);
			SolidBrush redbrush = new SolidBrush(Color.Magenta);
			SolidBrush greenbrush = new SolidBrush(Color.Orange);
			SolidBrush greenbrush2 = new SolidBrush(Color.Green);
			SolidBrush bluebrush = new SolidBrush(Color.Blue);

			Pen lightgraypen = new Pen(Color.LightGray);
			Pen redpen = new Pen(Color.Magenta,1);
			Pen greenpen = new Pen(Color.Orange,1);
			Pen greenpen2 = new Pen(Color.Green,1);
			Pen bluepen = new Pen(Color.Blue, 1);
			Pen whitepen = new Pen(Color.White, 1);
			Pen blackpen = new Pen(Color.Black, 1);
			System.Drawing.Font fontLegend = new System.Drawing.Font("Verdana", 10),

			fontTitle = new System.Drawing.Font("Verdana", 15, FontStyle.Bold);
			
			System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0,0,chartWidth, chartHeight);

			Bitmap bitmap = new Bitmap(chartWidth, chartHeight,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			Graphics g = Graphics.FromImage(bitmap);

			// Background
			g.FillRectangle(bgbrush, rect);
			
			// Draw axis
			g.DrawLine(blackpen, margin, chartHeight-margin+10, margin, margin);
			g.DrawLine(blackpen, margin-10, chartHeight-margin, chartWidth-margin+10, chartHeight-margin);
					
			// Draw axis titles
			g.DrawString(Global.rm.GetString("txtPagesPerHour"), fontLegend, blackbrush, margin-20, margin-10 - fontLegend.Height);
			g.DrawString(Global.rm.GetString("txtTime"), fontLegend, blackbrush, chartWidth-margin+20, chartHeight- margin - fontLegend.Height/2);

			// Draw legend
			g.DrawString(Global.rm.GetString("txtInput"), fontLegend, greenbrush, 170, margin-10 - fontLegend.Height);
			g.FillRectangle(greenbrush,(int)160, margin-10 - fontLegend.Height/2,3,3);
			g.DrawString(Global.rm.GetString("txtApproval"), fontLegend, bluebrush, 300, margin-10 - fontLegend.Height);
			g.FillRectangle(bluebrush,(int)290, margin-10 - fontLegend.Height/2,3,3);
			g.DrawString(Global.rm.GetString("txtOutput"), fontLegend, redbrush, 430, margin-10 - fontLegend.Height);
			g.FillRectangle(redbrush,(int)420, margin-10 - fontLegend.Height/2,3,3);
			g.DrawString(Global.rm.GetString("txtDeadLine"), fontLegend, greenbrush2, 560, margin-10 - fontLegend.Height);
			g.FillRectangle(greenbrush2,(int)550, margin-10 - fontLegend.Height/2,3,3);

			maxYsample = 0;
			if (dtInput != null || dtApprove != null || dtOutput != null)
			{
				for(int i=0; i<(int)tsTotal.TotalHours+1; i++)
				{
					if ((int)inputpagesstarthour[i] > maxYsample)
						maxYsample = (int)inputpagesstarthour[i];
					if ((int)approvepagesstarthour[i] > maxYsample)
						maxYsample = (int)approvepagesstarthour[i];
					if ((int)outputpagesstarthour[i] > maxYsample)
						maxYsample = (int)outputpagesstarthour[i];
				}

				// Draw y-ticks
				int maxYsample2 = maxYsample;
				if (maxYsample < 100)
				{
					maxYsample2 = (maxYsample+9)/10;
					maxYsample2 *= 10;
				}
				maxYsample = maxYsample2;
			}

			if (maxYsample > 0)
			{				
				double ystepscale = (double)(chartHeight - 2*margin - 10)/(double)maxYsample;
				
				ts = lastSampleTime - firstSampleTime;

				if (maxHoursToShow > 0)
				{
					if (ts.TotalHours > maxHoursToShow)
					{
						firstSampleTime = lastSampleTime.AddHours(-1.0 * (double)maxHoursToShow);
					}
					ts = lastSampleTime - firstSampleTime;
				}
				double xstepscale = (double)(chartWidth - 2*margin);
				if (ts.TotalSeconds > 0)
					xstepscale = (double)(chartWidth - 2*margin)/ts.TotalSeconds;

				DateTime firstWholeHour = new DateTime(firstSampleTime.Year, firstSampleTime.Month, firstSampleTime.Day, firstSampleTime.Hour,0,0);
				firstWholeHour.AddHours(1);
				
				// Draw x-ticks

				int inc = 1;
				if (ts.TotalHours > 24.0)
					inc = (int)ts.TotalHours/24 +1;
				
				double prevxx = 0;
				int day = 0;
				int month = 0;
				for (int i=0; i<ts.TotalHours; i+=inc)
				{
					DateTime tx = firstWholeHour.AddHours(i);
					ts1 = tx - firstSampleTime;

					double x = ts1.TotalSeconds * xstepscale;
					g.DrawLine(blackpen, (int)x+margin, chartHeight-margin+3, (int)x+margin, chartHeight-margin-3);

					if (x - prevxx > 50)
					{
						//					if (i%(10*inc) == 0) 
						g.DrawString(tx.Hour.ToString("00")+":"+tx.Minute.ToString("00"), fontLegend, blackbrush, (int)x+margin-15, chartHeight - margin - 10 +	 fontLegend.Height);
						g.DrawLine(blackpen, (int)x+margin, chartHeight-margin+6, (int)x+margin, chartHeight-margin-6);
						if (tx.Day > day || tx.Month > month)
						{
							g.DrawString(tx.Day.ToString("00")+"."+tx.Month.ToString("00"), fontLegend, blackbrush, (int)x+margin-15, chartHeight - margin + 5 +	 fontLegend.Height);
							day = tx.Day;
							month = tx.Month;
						}
						prevxx = x;
					}
				}

				double stepsbetweenticks  = (double)maxYsample/10.0;
				for (double i=1; i<=10; i++)
				{
					double y = (double)(chartHeight - margin)- (double)(i*stepsbetweenticks) * ystepscale;
					
					g.DrawLine(blackpen, margin-3, (int)y,  margin+3, (int)y);
					int yn = (int)(i*stepsbetweenticks);
					
					g.DrawString(yn.ToString(), fontLegend, blackbrush, margin-30, (int)y - (int)(fontLegend.Height/2.0));

					if (i%2 == 0)
						g.DrawLine(lightgraypen, margin+4, (int)y, (int)(chartWidth-margin+10), (int)y);
				}

				
				if (dtInput != null) 
				{
					int pages = 0;
					double prevx = -1.0;
					double prevy = -1.0;
					DateTime sampleTime = firstSampleTime.AddHours(0.5);
					for(int i=0; i<(int)tsTotal.TotalHours+1; i++)
					{
						ts1 = sampleTime - firstSampleTime;
						double x = (double)margin +  ts1.TotalSeconds * xstepscale;
						int iy = (int)inputpagesstarthour[i];
						double y = (double)(chartHeight - margin)- (double)iy * ystepscale;
						g.FillRectangle(greenbrush,(int)x-1, (int)y-1,3,3);
						if (prevx > 0)
						{
							g.DrawLine(greenpen,(int)prevx, (int)prevy, (int)prevx, (int)y);
							g.DrawLine(greenpen,(int)prevx, (int)y, (int)x, (int)y);
						}
						else if (prevx <= 0)
						{
							g.DrawLine(greenpen,(int)margin, (int)y, (int)x, (int)y);
						}

						prevx = x;
						prevy = y;
						sampleTime = sampleTime.AddHours(1.0);
						
					}
					int xx = pages;
				}
				if (deadline.Year > 2000)
				{
					if ( deadline > firstSampleTime && deadline < lastSampleTime)
					{
						ts1 = lastSampleTime - deadline;
						double x = (double)(chartWidth - margin) - ts1.TotalSeconds * xstepscale;
							
						g.DrawLine(greenpen2, (int)x, (int)margin, (int)x, (int)chartHeight - margin);

					}
				}

				if (dtApprove != null) 
				{
					int pages = 0;
					double prevx = -1.0;
					double prevy = -1.0;

					DateTime sampleTime = firstSampleTime.AddHours(0.5);
					for(int i=0; i<(int)tsTotal.TotalHours+1; i++)
					{

						ts1 = sampleTime - firstSampleTime;
						double x = (double)margin +  ts1.TotalSeconds * xstepscale;
						int iy = (int)approvepagesstarthour[i];
						double y = (double)(chartHeight - margin)- (double)iy * ystepscale;
						//g.DrawEllipse(bluepen,(int)x, (int)y,3,3);

						g.FillRectangle(bluebrush,(int)x-1, (int)y-1,3,3);
						if (prevx > 0)
						{
							g.DrawLine(bluepen,(int)prevx, (int)prevy, (int)prevx, (int)y);
							g.DrawLine(bluepen,(int)prevx, (int)y, (int)x, (int)y);
						}
						else if (prevx <= 0)
						{
							g.DrawLine(bluepen,(int)margin, (int)y, (int)x, (int)y);
						}

						prevx = x;
						prevy = y;
						sampleTime = sampleTime.AddHours(1.0);

					}
					int xx = pages;
				}

				if (dtOutput != null) 
				{
					int pages = 0;
					double prevx = -1.0;
					double prevy = -1.0;

					DateTime sampleTime = firstWholeHour;
					for(int i=0; i<(int)tsTotal.TotalHours+1; i++)
					{
						ts1 = sampleTime - firstSampleTime;
						double x = (double)margin +  ts1.TotalSeconds * xstepscale;
						int iy = (int)outputpagesstarthour[i];
						double y = (double)(chartHeight - margin)- (double)iy * ystepscale;
						//g.DrawEllipse(redpen,(int)x, (int)y,3,3);

						g.FillRectangle(redbrush,(int)x-1, (int)y-1,3,3);
						if (prevx > 0)
						{
							g.DrawLine(redpen,(int)prevx, (int)prevy, (int)prevx, (int)y);
							g.DrawLine(redpen,(int)prevx, (int)y, (int)x, (int)y);
						} 
						else if (prevx <= 0)
						{
							g.DrawLine(redpen,(int)margin, (int)y, (int)x, (int)y);
						}
						prevx = x;
						prevy = y;
						sampleTime = sampleTime.AddHours(1.0);
					}
					int xx = pages;
				}
			}
				
			try
			{
				bitmap.Save(fileName,ImageFormat.Jpeg);
			}
			catch //(Exception ex)
			{
				lblError.Text = errmsg;
				ret = false;
			}

			g.Dispose();
			bitmap.Dispose();

			return ret;
    	}

		private void doPopupArchiveWindow()
		{
			string popupScript =
				"<script language='javascript'>" +
				"var xpos = 300;" + 
				"var ypos = 300;" +
				"if(window.screen) { xpos = (screen.width-500)/2; ypos = (screen.height-500)/2; }" + 
				"var s = 'status=no,top='+ypos+',left='+xpos+',width=500,height=500';" +
				"var PopupWindow = window.open('../Views/ReportArchive.aspx','Archive',s);" + 	
				"if (parseInt(navigator.appVersion) >= 4) PopupWindow.focus();" +
				"</script>";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "PopupScript", popupScript, false);
		}

        protected void btnShowArchive_Click(object sender, System.EventArgs e)
		{
//			doPopupArchiveWindow();

			Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowReportArchive");
			mywindow.Title =  Global.rm.GetString("txtReportArchive");

			mywindow.VisibleOnPageLoad = true;

		}

  /*      protected void btnShowMonthlyReportOLD_Click(object sender, System.EventArgs e)
		{
			CCDBaccess db = new CCDBaccess();
			string errmsg = "";


			DataTable dt = db.GetMonthlyCollectionExcel(out errmsg);  
			if (dt == null)
			{
				lblError.Text = errmsg;
				return;
			}
			if (errmsg != "")
			{
				lblError.Text = errmsg;
				return;
			}
			if (dt.Rows.Count == 0)
			{
				lblError.Text = "No data available";
			}

			//Set Aspose.Excel License
			Aspose.Excel.License licExcel = new Aspose.Excel.License();
			licExcel.SetLicense((System.IO.Stream)null);

			Excel excel = new Aspose.Excel.Excel();
			excel.Worksheets.Add();		
			excel.Worksheets[0].Name = Global.rm.GetString("txtShowMonthlySummary");
			Cells cells = excel.Worksheets[0].Cells;


			Cell cell = cells[0, 0];

			cell.PutValue("WebCenter "+Global.rm.GetString("txtShowMonthlySummary"));
			cell.Style.Font.IsBold = true;
			cell.Style.Font.Size = 18;
			cells.SetRowHeight(0,20);
			
			cell.Style.HorizontalAlignment = TextAlignmentType.Top;

//			cells.SetColumnWidth(0,24);
//			cells.SetColumnWidth(1,14);

			cell = cells[2, 0];
			cell.PutValue(DateTime.Now.ToString());
			cell.Style.Font.IsBold = true;
			cell.Style.Font.Size = 16;
			cells.SetRowHeight(2,18);

			int y = 4;
			int x = 0;
			cell = cells[y, x];
			cell.PutValue(Global.rm.GetString("txtProduct")); 
			cell.Style.Font.IsBold = true;
			cells.SetColumnWidth((byte)x,18);
			x++;

			cell = cells[y, x];
			cell.PutValue(Global.rm.GetString("txtPubDate")); 
			cell.Style.Font.IsBold = true;
			cells.SetColumnWidth((byte)x,14);
			x++;

			cell = cells[y, x];
			cell.PutValue(Global.rm.GetString("txtStatNumberOfPages")); 
			cell.Style.Font.IsBold = true;
			cells.SetColumnWidth((byte)x,14);
			x++;

			cell = cells[y, x];
			cell.PutValue(Global.rm.GetString("txtStatLastPageInput")); 
			cell.Style.Font.IsBold = true;
			cells.SetColumnWidth((byte)x,18);
			x++;

			cell = cells[y, x];
			cell.PutValue(Global.rm.GetString("txtStatNumberOfPlates"));
			cell.Style.Font.IsBold = true;
			cells.SetColumnWidth((byte)x,14);
			x++;

			cell = cells[y, x];
			cell.PutValue(Global.rm.GetString("txtStatNumberOfPlatesUsed"));
			cell.Style.Font.IsBold = true;
			cells.SetColumnWidth((byte)x,18);
			x++;

			cell = cells[y, x];
			cell.PutValue(Global.rm.GetString("txtStatLastPageOutput"));
			cell.Style.Font.IsBold = true;
			cells.SetColumnWidth((byte)x,18);
			x++;

			cell = cells[y, x];
			cell.PutValue(Global.rm.GetString("txtStatMonosets"));
			cell.Style.Font.IsBold = true;
			cells.SetColumnWidth((byte)x,14);
			x++;

			cell = cells[y, x];
			cell.PutValue(Global.rm.GetString("txtStatColorsets"));
			cell.Style.Font.IsBold = true;
			cells.SetColumnWidth((byte)x,14);

			excel.Worksheets.Add();		
		//	string startcell = "A"+(y+2).ToString();
			//	if (dt.Rows.Count > 0)
		//		excel.Worksheets[0].Cells.ImportDataTable(dt, false, startcell);
			
			
			foreach (DataRow row in dt.Rows)
			{	
				x = 0;
				y++;				
				cell = cells[y, x++];
				cell.PutValue((string)row["ProductionName"]);
				cell.Style.Font.IsBold = false;
				cell = cells[y, x++];
				cell.PutValue((string)row["PublDate"]);
				cell = cells[y, x++];
				cell.PutValue((int)row["Pages"]);
				cell = cells[y, x++];
				DateTime lastPageTime = (DateTime)row["LastPage"];
				if (lastPageTime.Year > 2000)
					cell.PutValue(lastPageTime.Year > 2000 ? (string)lastPageTime.ToString("dd-MM hh:mm:ss") : " ");
				cell = cells[y, x++];
				cell.PutValue((int)row["PlatesToProduce"]);
				cell = cells[y, x++];
				cell.PutValue((int)row["TotalPlatesUsed"]);
				cell = cells[y, x++];
				lastPageTime = (DateTime)row["LastPlate"];
				if (lastPageTime.Year > 2000)
					cell.PutValue(lastPageTime.Year > 2000 ? (string)lastPageTime.ToString("dd-MM hh:mm:ss") : " ");

				cell = cells[y, x++];
				cell.PutValue((int)row["MonoPlatesUsed"]);
				cell = cells[y, x++];
				cell.PutValue((int)row["ColorPlatesUsed"]);

			}

			

			string excelName = "WebCenter_report_" + (string)Session["SelectedPress"] + "_" +DateTime.Now.Year.ToString()+"-" + DateTime.Now.Month.ToString("0:00") + DateTime.Now.Day.ToString("0:00") + ".xls";


            try { 
                if ((int)Application["ExcelSavetype"] == 0)
                    excel.Save(excelName, FileFormatType.Excel2000, SaveType.OpenInBrowser, Response);
                else if ((int)Application["ExcelSavetype"] == 1)
                    excel.Save(excelName, FileFormatType.Excel2000, SaveType.OpenInExcel, Response);
                else if ((int)Application["ExcelSavetype"] == 2)
                    excel.Save(excelName, FileFormatType.Excel2000, SaveType.Default, Response);
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }

        }
*/
        // 2016-02-22 
        private DateTime String2Date(string s)
        {
            try
            {
                if (s.Length == 10)
                {
                    if (s[4] == '-' && s[7] == '-')
                        return new DateTime(Int32.Parse(s.Substring(0, 4)), Int32.Parse(s.Substring(5, 2)), Int32.Parse(s.Substring(8, 2)));
                }
            }
            catch
            {  
            }

            return DateTime.MinValue;
        }

        protected void btnShowMonthlyReport_Click(object sender, System.EventArgs e)
        {
            CCDBaccess db = new CCDBaccess();
            string errmsg = "";


            DataTable dt = db.GetMonthlyCollectionExcel(out errmsg);
            if (dt == null)
            {
                lblError.Text = errmsg;
                return;
            }
            if (errmsg != "")
            {
                lblError.Text = errmsg;
                return;
            }
            if (dt.Rows.Count == 0)
            {
                lblError.Text = "No data available";
            }

            ExcelWorkbook.SetLicenseCode("S2215N-661233-01BL5C-119A00");
            ExcelWorkbook Wbook = new ExcelWorkbook();
            Wbook.Worksheets.Add(Global.rm.GetString("txtShowMonthlySummary"));

            Wbook.Worksheets[0].Cells[0, 0].Value = "WebCenter " + Global.rm.GetString("txtShowMonthlySummary");
            Wbook.Worksheets[0].Cells[0, 0].Style.Font.Size = 18;
            Wbook.Worksheets[0].Cells[0, 0].Style.Font.Bold = true;
            Wbook.Worksheets[0].Rows[0].Autofit();
            Wbook.Worksheets[0].Cells[0, 0].Style.VerticalAlignment = TypeOfVAlignment.Top;

            //			cells.SetColumnWidth(0,24);
            //			cells.SetColumnWidth(1,14);

            Wbook.Worksheets[0].Cells[2, 0].Value = DateTime.Now;
            Wbook.Worksheets[0].Cells[2, 0].Style.StringFormat = (string)Application["ExcelTimeFormat"];
            Wbook.Worksheets[0].Rows[2].Autofit();

            int y = 4;
            int x = 0;
            int xwidthwide = 130;

            Wbook.Worksheets[0].Cells[y, x].Value = Global.rm.GetString("txtProduct");
            Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
            Wbook.Worksheets[0].Columns[x].Width = xwidthwide; //Autofit();
            x++;
            Wbook.Worksheets[0].Cells[y, x].Value = Global.rm.GetString("txtPubDate");
            Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
            Wbook.Worksheets[0].Columns[x].Autofit(); // = xwidthnormal;
            x++;
            Wbook.Worksheets[0].Cells[y, x].Value = Global.rm.GetString("txtStatNumberOfPages");
            Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
            Wbook.Worksheets[0].Columns[x].Autofit(); // = xwidthnormal;
            x++;
            Wbook.Worksheets[0].Cells[y, x].Value = Global.rm.GetString("txtStatLastPageInput");
            Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
            Wbook.Worksheets[0].Columns[x].Autofit(); // = xwidthwide;
            x++;

            Wbook.Worksheets[0].Cells[y, x].Value = Global.rm.GetString("txtStatNumberOfPlates");
            Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
            Wbook.Worksheets[0].Columns[x].Autofit(); // = xwidthnormal;
            x++;

            Wbook.Worksheets[0].Cells[y, x].Value = Global.rm.GetString("txtStatNumberOfPlatesUsed");
            Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
            Wbook.Worksheets[0].Columns[x].Autofit(); // = xwidthwide;
            x++;

            Wbook.Worksheets[0].Cells[y, x].Value = Global.rm.GetString("txtStatLastPageOutput");
            Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
            Wbook.Worksheets[0].Columns[x].Autofit(); // = xwidthwide;
            x++;

            Wbook.Worksheets[0].Cells[y, x].Value = Global.rm.GetString("txtStatMonosets");
            Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
            Wbook.Worksheets[0].Columns[x].Autofit(); // = xwidthnormal;
            x++;

            Wbook.Worksheets[0].Cells[y, x].Value = Global.rm.GetString("txtStatColorsets");
            Wbook.Worksheets[0].Cells[y, x].Style.Font.Bold = true;
            Wbook.Worksheets[0].Columns[x].Autofit(); // = xwidthnormal;
            x++;

            foreach (DataRow row in dt.Rows)
            {
                x = 0;
                y++;

                Wbook.Worksheets[0].Cells[y, x++].Value = (string)row["ProductionName"];

                DateTime pubDate = String2Date((string)row["PublDate"]);
                if (pubDate.Year > 2000)
                {
                    Wbook.Worksheets[0].Cells[y, x].Value = pubDate;
                    Wbook.Worksheets[0].Cells[y, x].Style.StringFormat = (string)Application["ExcelDateFormat"];
                }
                x++;

                Wbook.Worksheets[0].Cells[y, x++].Value = (int)row["Pages"];
                DateTime lastPageTime = (DateTime)row["LastPage"];
                if (lastPageTime.Year > 2000)
                {
                    Wbook.Worksheets[0].Cells[y, x].Value = lastPageTime;
                    Wbook.Worksheets[0].Cells[y, x].Style.StringFormat = (string)Application["ExcelTimeFormat"];
                }                
                x++;

                Wbook.Worksheets[0].Cells[y, x++].Value = (int)row["PlatesToProduce"];
                Wbook.Worksheets[0].Cells[y, x++].Value = (int)row["TotalPlatesUsed"];
                lastPageTime = (DateTime)row["LastPlate"];
                if (lastPageTime.Year > 2000)
                {
                    Wbook.Worksheets[0].Cells[y, x].Value = lastPageTime;
                    Wbook.Worksheets[0].Cells[y, x].Style.StringFormat = (string)Application["ExcelTimeFormat"];
                }
                x++;

                Wbook.Worksheets[0].Cells[y, x++].Value = (int)row["MonoPlatesUsed"];
                Wbook.Worksheets[0].Cells[y, x++].Value = (int)row["ColorPlatesUsed"];
            }

            string excelName = "WebCenter_report_" + (string)Session["SelectedPress"] + "_" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("0:00") + DateTime.Now.Day.ToString("0:00");
            if ((int)Application["ExcelSavetype"] == 0)
                excelName += ".xls";
            else
                excelName += ".xlsx";

            try
            {
                Encoding ascii = Encoding.ASCII;
                Encoding unicode = Encoding.Unicode;
                byte[] unicodeBytes = unicode.GetBytes(excelName);
                byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);
                char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
                ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
                string asciiString = new string(asciiChars);

                // XLS
                // Save Excel Workbook (XLS) to MemoryStream

                if ((int)Application["ExcelSavetype"] == 0)
                {
                    byte[] bytes = Wbook.WriteXLS().ToArray();

                    // Show Excel (XLS) in Browser window without saving on disk
                    Response.Buffer = true;

                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + asciiString);
                    Response.AppendHeader("Content-Length", bytes.Length.ToString());

                    Response.BinaryWrite(bytes);
                    Response.Flush();
                    Response.End();
                }
                else
                {
                    byte[] bytes = Wbook.WriteXLSX().ToArray();

                    // Show Open Office (XLSX) in Browser window without saving on disk
                    Response.Buffer = true;
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AppendHeader("Content-Disposition", "attachment; filename=" + asciiString);
                    Response.AppendHeader("Content-Length", bytes.Length.ToString());
                    Response.BinaryWrite(bytes);
                    Response.Flush();
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }

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
	}
}
