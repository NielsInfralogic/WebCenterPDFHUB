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
using WebCenter4.Classes;
using System.Text;
using System.Globalization;
using System.Resources;
using System.Threading;


namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for ChangePriority.
	/// </summary>
	public partial class Message : System.Web.UI.Page
	{
       
		protected int doClose;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
            doClose = 0;


            if (!this.IsPostBack)
            {
                lblText.Text = (string)Session["PopupMessage"];

                imgPreview.ImageUrl = (string)Session["PopupImageInk"];
            }

            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);
            btnCancel.Text = Global.rm.GetString("txtClose");
            LabelDensities.Text = Global.rm.GetString("txtDensityMap");

            //TextBox.Attributes.Add("style", "height:60px;width:356px;");

            string errmsg;
            CCDBaccess db = new CCDBaccess();

            int nLimit = db.GetInkLimitForPage((int)Session["CurrentCopySeparationSet"], out errmsg);

            int n1 = 0;
            int n2 = nLimit - 100;




            Label2.Text = "D <                 " + n2.ToString();
            n1 = n2;
            n2 = nLimit - 75;
            Label3.Text = n1.ToString() + " < D <                                                 " + n2.ToString() + " ";
            n1 = n2;
            n2 = nLimit - 50;
            Label4.Text = n1.ToString() + " < D <                                                 " + n2.ToString() + " ";
            n1 = n2;
            n2 = nLimit - 25;
            Label5.Text = n1.ToString() + " < D <                                                 " + n2.ToString() + " ";
            n1 = n2;
            n2 = nLimit;
            Label6.Text = n1.ToString() + " < D <                                                 " + n2.ToString() + " ";
            n1 = n2;
            n2 = nLimit + 25;
            Label7.Text = n1.ToString() + " < D <                                                 " + n2.ToString() + " ";
            n1 = n2;
            Label8.Text = n1.ToString() + " <                 D";

            n1 = 10;
            n2 = nLimit - 50;
            Label12.Text = n1.ToString() + " < D <                                                 " + n2.ToString() + " ";

            n1 = nLimit - 50;
            n2 = nLimit - 25;
            Label13.Text = n1.ToString() + " < D <                                                 " + n2.ToString() + " ";

            n1 = nLimit - 25;
            n2 = nLimit;
            Label14.Text = n1.ToString() + " < D <                                                 " + n2.ToString() + " ";

            n1 = nLimit;
            Label15.Text = n1.ToString() + " <                 D";

            if ((bool)Application["GrayModeHeatMap"])
            {
                Panel1.Visible = false;
                Panel2.Visible = true;
            }
            else
            {
                Panel1.Visible = true;
                Panel2.Visible = false;
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
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		
		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			string finalImage = Request.MapPath(Request.ApplicationPath) + imgPreview.ImageUrl.Substring(2);
			try
			{
				System.IO.File.Delete(finalImage);
			}
			catch 
			{
			}
			doClose = 1;
		}
	}
}
