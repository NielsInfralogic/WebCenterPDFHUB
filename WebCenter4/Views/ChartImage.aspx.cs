using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Resources;
using WebCenter4.Classes;

namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for PressRunsImages.
	/// </summary>
	public class ChartImages : System.Web.UI.Page
	{
		private int maxHoursToShow = 0;
		private int margin = 60;
		private void Page_Load(object sender, System.EventArgs e)
		{
            Response.ContentType = "image/jpeg";

            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            int chartWidth = Convert.ToInt32(Request.QueryString["width"]);
            int chartHeight = Convert.ToInt32(Request.QueryString["height"]);

            CCDBaccess db = new CCDBaccess();
            string errmsg = "";

            DateTime startInputTime = DateTime.MaxValue;
            DateTime stopInputTime = DateTime.MinValue;

            DateTime startApproveTime = DateTime.MaxValue;
            DateTime stopApproveTime = DateTime.MinValue;

            DateTime startOutputTime = DateTime.MaxValue;
            DateTime stopOutputTime = DateTime.MinValue;

            DateTime startReadyTime = DateTime.MaxValue;
            DateTime stopReadyTime = DateTime.MinValue;

            int maxYsample = 0;
            DateTime firstSampleTime = DateTime.MaxValue;
            DateTime lastSampleTime = DateTime.MinValue;

            DataTable dtInput = null;
            DataTable dtApprove = null;
            DataTable dtOutput = null;
            DataTable dtReady = null;

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
                        if (firstSampleTime > startApproveTime)
                            firstSampleTime = startApproveTime;
                        if (lastSampleTime < stopApproveTime)
                            lastSampleTime = stopApproveTime;
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

            if ((bool)Application["ReportShowReadyTime"])
            {
                dtReady = db.GetStatCollection(4, out errmsg);
                if (dtReady != null)
                {
                    if (dtReady.Rows.Count == 0)
                    {
                        dtReady = null;
                    }
                    else
                    {
                        if (maxYsample < dtReady.Rows.Count)
                            maxYsample = dtReady.Rows.Count;
                        foreach (DataRow row in dtReady.Rows)
                        {
                            DateTime dt = (DateTime)row["Time"];
                            if (dt.Year <= 2000)
                                continue;

                            if (dt < startReadyTime)
                                startReadyTime = dt;

                            if (dt > stopReadyTime)
                                stopReadyTime = dt;
                        }
                        if (firstSampleTime > startReadyTime)
                            firstSampleTime = startReadyTime;
                        if (lastSampleTime < stopReadyTime)
                            lastSampleTime = stopReadyTime;
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
            SolidBrush yellowbrush = new SolidBrush(Color.DarkGray);

            Pen redpen = new Pen(Color.Magenta, 1);
            Pen greenpen = new Pen(Color.Orange, 1);
            Pen greenpen2 = new Pen(Color.Green, 1);
            Pen yellowpen = new Pen(Color.DarkGray, 1);
            Pen bluepen = new Pen(Color.Blue, 1);
            Pen lightgraypen = new Pen(Color.LightGray);

            Pen whitepen = new Pen(Color.White, 1);
            Pen blackpen = new Pen(Color.Black, 1);
            Font fontLegend = new Font("Verdana", 10),
            fontTitle = new Font("Verdana", 15, FontStyle.Bold);

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
            int xtxtpos = 120;
            g.DrawString(Global.rm.GetString("txtInput"), fontLegend, greenbrush, xtxtpos + 10, margin - 10 - fontLegend.Height);
            g.FillRectangle(greenbrush, (int)xtxtpos, margin - 10 - fontLegend.Height / 2, 3, 3);
            xtxtpos += 130;
            g.DrawString(Global.rm.GetString("txtApproval"), fontLegend, bluebrush, xtxtpos + 10, margin - 10 - fontLegend.Height);
            g.FillRectangle(bluebrush, (int)xtxtpos, margin - 10 - fontLegend.Height / 2, 3, 3);
            xtxtpos += 130;

            if ((bool)Application["ReportShowReadyTime"])
            {
                g.DrawString(Global.rm.GetString("txtPlateReadyTime"), fontLegend, yellowbrush, xtxtpos + 10, margin - 10 - fontLegend.Height);
                g.FillRectangle(yellowbrush, (int)xtxtpos, margin - 10 - fontLegend.Height / 2, 3, 3);
                xtxtpos += 130;
            }

            string st = (bool)Application["LocationIsPress"] && (bool)Application["UsePressGroups"] == false ? Global.rm.GetString("txtTransmitted") : Global.rm.GetString("txtOutput");
            g.DrawString(st, fontLegend, redbrush, xtxtpos + 10, margin - 10 - fontLegend.Height);
            g.FillRectangle(redbrush, (int)xtxtpos, margin - 10 - fontLegend.Height / 2, 3, 3);
            xtxtpos += 130;
            g.DrawString(Global.rm.GetString("txtDeadLine"), fontLegend, greenbrush2, xtxtpos + 10, margin - 10 - fontLegend.Height);
            g.FillRectangle(greenbrush2, (int)xtxtpos, margin - 10 - fontLegend.Height / 2, 3, 3);

            TimeSpan ts1 = new TimeSpan();


            if (firstSampleTime == DateTime.MaxValue || lastSampleTime == DateTime.MinValue)
            {
                // no data
                dtInput = null;
                dtApprove = null;
                dtOutput = null;
                dtReady = null;
            }

            // Draw y-ticks
            int maxYsample2 = maxYsample;
            if (maxYsample < 100)
            {
                maxYsample2 = (maxYsample + 9) / 10;
                maxYsample2 *= 10;
            }
            maxYsample = maxYsample2;

            if (dtInput == null && dtApprove == null && dtOutput == null && dtReady == null)
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

                if (dtReady != null)
                {
                    int pages = 0;
                    double prevx = -1.0;
                    double prevy = -1.0;

                    DataView view = new DataView(dtReady, "", "Time", DataViewRowState.CurrentRows);
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

                        g.FillRectangle(yellowbrush, (int)x - 1, (int)y - 1, 3, 3);
                        if (prevx > 0)
                        {
                            g.DrawLine(yellowpen, (int)prevx, (int)prevy, (int)x, (int)y);
                        }
                        prevx = x;
                        prevy = y;
                    }
                    int xx = pages;
                }
            }

            bitmap.Save(Response.OutputStream, ImageFormat.Jpeg);

            g.Dispose();
            bitmap.Dispose();
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
	}
}
