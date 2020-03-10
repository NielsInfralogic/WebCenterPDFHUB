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
using System.Resources;
using System.Threading;
using WebCenter4.Classes;

namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for PressRunsImages.
	/// </summary>
	public class ChartImages2 : System.Web.UI.Page
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

                for (int i = 0; i < (int)tsTotal.TotalHours + 1; i++)
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
            Pen redpen = new Pen(Color.Magenta, 1);
            Pen greenpen = new Pen(Color.Orange, 1);
            Pen greenpen2 = new Pen(Color.Green, 1);

            Pen bluepen = new Pen(Color.Blue, 1);
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
            g.DrawString(Global.rm.GetString("txtPagesPerHour"), fontLegend, blackbrush, margin - 20, margin - 10 - fontLegend.Height);
            g.DrawString(Global.rm.GetString("txtTime"), fontLegend, blackbrush, chartWidth - margin + 20, chartHeight - margin - fontLegend.Height / 2);

            // Draw legend

            g.DrawString(Global.rm.GetString("txtInput"), fontLegend, greenbrush, 170, margin - 10 - fontLegend.Height);
            g.FillRectangle(greenbrush, (int)160, margin - 10 - fontLegend.Height / 2, 3, 3);
            g.DrawString(Global.rm.GetString("txtApproval"), fontLegend, bluebrush, 300, margin - 10 - fontLegend.Height);
            g.FillRectangle(bluebrush, (int)290, margin - 10 - fontLegend.Height / 2, 3, 3);

            string st = (bool)Application["LocationIsPress"] && (bool)Application["UsePressGroups"] == false ? Global.rm.GetString("txtTransmitted") : Global.rm.GetString("txtOutput");

            g.DrawString(st, fontLegend, redbrush, 430, margin - 10 - fontLegend.Height);
            g.FillRectangle(redbrush, (int)420, margin - 10 - fontLegend.Height / 2, 3, 3);
            g.DrawString(Global.rm.GetString("txtDeadLine"), fontLegend, greenbrush2, 560, margin - 10 - fontLegend.Height);
            g.FillRectangle(greenbrush2, (int)550, margin - 10 - fontLegend.Height / 2, 3, 3);



            maxYsample = 0;
            if (dtInput != null || dtApprove != null || dtOutput != null)
            {
                for (int i = 0; i < (int)tsTotal.TotalHours + 1; i++)
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
                    maxYsample2 = (maxYsample + 9) / 10;
                    maxYsample2 *= 10;
                }
                maxYsample = maxYsample2;
            }

            if (maxYsample > 0)
            {

                double ystepscale = (double)(chartHeight - 2 * margin - 10) / (double)maxYsample;

                ts = lastSampleTime - firstSampleTime;

                if (maxHoursToShow > 0)
                {
                    if (ts.TotalHours > maxHoursToShow)
                    {
                        firstSampleTime = lastSampleTime.AddHours(-1.0 * (double)maxHoursToShow);
                    }
                    ts = lastSampleTime - firstSampleTime;
                }
                double xstepscale = (double)(chartWidth - 2 * margin /* - 10 */);
                if (ts.TotalSeconds > 0)
                    xstepscale = (double)(chartWidth - 2 * margin /* - 10 */) / ts.TotalSeconds;

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


                if (dtInput != null)
                {
                    int pages = 0;
                    double prevx = -1.0;
                    double prevy = -1.0;
                    DateTime sampleTime = firstSampleTime.AddHours(0.5);
                    for (int i = 0; i < (int)tsTotal.TotalHours + 1; i++)
                    {
                        ts1 = sampleTime - firstSampleTime;
                        double x = (double)margin + ts1.TotalSeconds * xstepscale;
                        int iy = (int)inputpagesstarthour[i];
                        double y = (double)(chartHeight - margin) - (double)iy * ystepscale;
                        g.FillRectangle(greenbrush, (int)x - 1, (int)y - 1, 3, 3);
                        if (prevx > 0)
                        {
                            g.DrawLine(greenpen, (int)prevx, (int)prevy, (int)prevx, (int)y);
                            g.DrawLine(greenpen, (int)prevx, (int)y, (int)x, (int)y);
                        }
                        else if (prevx <= 0)
                        {
                            g.DrawLine(greenpen, (int)margin, (int)y, (int)x, (int)y);
                        }

                        prevx = x;
                        prevy = y;
                        sampleTime = sampleTime.AddHours(1.0);

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

                    DateTime sampleTime = firstSampleTime.AddHours(0.5);
                    for (int i = 0; i < (int)tsTotal.TotalHours + 1; i++)
                    {

                        ts1 = sampleTime - firstSampleTime;
                        double x = (double)margin + ts1.TotalSeconds * xstepscale;
                        int iy = (int)approvepagesstarthour[i];
                        double y = (double)(chartHeight - margin) - (double)iy * ystepscale;
                        //g.DrawEllipse(bluepen,(int)x, (int)y,3,3);

                        g.FillRectangle(bluebrush, (int)x - 1, (int)y - 1, 3, 3);
                        if (prevx > 0)
                        {
                            g.DrawLine(bluepen, (int)prevx, (int)prevy, (int)prevx, (int)y);
                            g.DrawLine(bluepen, (int)prevx, (int)y, (int)x, (int)y);
                        }
                        else if (prevx <= 0)
                        {
                            g.DrawLine(bluepen, (int)margin, (int)y, (int)x, (int)y);
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
                    for (int i = 0; i < (int)tsTotal.TotalHours + 1; i++)
                    {
                        ts1 = sampleTime - firstSampleTime;
                        double x = (double)margin + ts1.TotalSeconds * xstepscale;
                        int iy = (int)outputpagesstarthour[i];
                        double y = (double)(chartHeight - margin) - (double)iy * ystepscale;
                        //g.DrawEllipse(redpen,(int)x, (int)y,3,3);

                        g.FillRectangle(redbrush, (int)x - 1, (int)y - 1, 3, 3);
                        if (prevx > 0)
                        {
                            g.DrawLine(redpen, (int)prevx, (int)prevy, (int)prevx, (int)y);
                            g.DrawLine(redpen, (int)prevx, (int)y, (int)x, (int)y);
                        }
                        else if (prevx <= 0)
                        {
                            g.DrawLine(redpen, (int)margin, (int)y, (int)x, (int)y);
                        }
                        prevx = x;
                        prevy = y;
                        sampleTime = sampleTime.AddHours(1.0);
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
