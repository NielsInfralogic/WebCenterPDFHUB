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

namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for PressRunsImages.
	/// </summary>
	public class PressRunsImages : System.Web.UI.Page
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
            System.Drawing.Image ProgressImageGreen;
            System.Drawing.Image ProgressImageYellow;
            System.Drawing.Image ProgressImageGray;

            if ((bool)Application["FlatLook"] == false)
            {
                ProgressImageGreen = System.Drawing.Image.FromFile(Request.MapPath(Request.ApplicationPath) + "/Images/greengradient2.gif");
                ProgressImageYellow = System.Drawing.Image.FromFile(Request.MapPath(Request.ApplicationPath) + "/Images/yellowgradient2.gif");
                ProgressImageGray = System.Drawing.Image.FromFile(Request.MapPath(Request.ApplicationPath) + "/Images/graygradient2.gif");
            }
            else
            {
                ProgressImageGreen = System.Drawing.Image.FromFile(Request.MapPath(Request.ApplicationPath) + "/Images/colorGreen_Flat.gif");
                ProgressImageYellow = System.Drawing.Image.FromFile(Request.MapPath(Request.ApplicationPath) + "/Images/colorY_Flat.gif");
                ProgressImageGray = System.Drawing.Image.FromFile(Request.MapPath(Request.ApplicationPath) + "/Images/colorGray_Flat.gif");
            }



			int nWidth = 80;
			int nHeight = 18;
			int n1 = Convert.ToInt32(Request.QueryString["n1"]);
			int n2 = Convert.ToInt32(Request.QueryString["n2"]);
			double f1 = (double)n1;
			double f2 = n2 > 0 ? (double)n2 : 1.0;
			f1 = (double)nWidth * f1/f2;
			
			Bitmap bitmap = new Bitmap(nWidth,16,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			Graphics g = Graphics.FromImage(bitmap);
			System.Drawing.Rectangle DstRect = new System.Drawing.Rectangle(0,0,(int)f1,nHeight);

			System.Drawing.Rectangle SrcRect = new System.Drawing.Rectangle(0, 0, ProgressImageGray.Width, ProgressImageGray.Height);

            g.DrawImage(n1 == n2 ? ProgressImageGreen : ProgressImageYellow, DstRect, SrcRect, System.Drawing.GraphicsUnit.Pixel);
			DstRect.X = (int)f1;
			DstRect.Width = nWidth - (int)f1;
			g.DrawImage(ProgressImageGray, DstRect,SrcRect, System.Drawing.GraphicsUnit.Pixel);
			
			MemoryStream memStream = new MemoryStream();
			
			Response.ContentType = "image/jpeg";
			bitmap.Save(memStream,ImageFormat.Jpeg);
			memStream.WriteTo(Response.OutputStream);
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
