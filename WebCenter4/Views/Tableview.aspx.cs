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
using System.Web.Caching;
using System.Drawing.Imaging;
using System.Text;
using System.Globalization;
using System.Resources;
using System.Threading;
using Telerik.Web.UI;
namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for Tableview.
	/// </summary>
	public class Tableview : System.Web.UI.Page
	{
        protected global::Telerik.Web.UI.RadToolBar RadToolBar1;
        protected Telerik.Web.UI.RadGrid RadGrid1;

        protected System.Web.UI.WebControls.Label lblChooseProduct;
		protected System.Web.UI.WebControls.Label lblError;

        protected HtmlInputHidden HiddenX;
        protected HtmlInputHidden HiddenY;
        protected HtmlInputHidden HiddenScrollPos;


        protected int nRefreshTime;
        public int nScollPos = 0;
	

        public int ColumnIndex_Ed = 2;
		public int ColumnIndex_Sec = 3;
		public int ColumnIndex_Page = 4;  
        public int ColumnIndex_FTP = 5; 
        public int ColumnIndex_PRE = 6;
        public int ColumnIndex_INK = 7;
        public int ColumnIndex_RIP = 8;
        public int ColumnIndex_Rdy = 9;
        public int ColumnIndex_Appr = 10;
        public int ColumnIndex_CTP = 11;
        public int ColumnIndex_Bend = 12;
        public int ColumnIndex_Preset = 13;
        public int ColumnIndex_Divider = 14;

        public int ColumnIndex_Ed2 = 15;
        public int ColumnIndex_Sec2 = 16;
        public int ColumnIndex_Page2 = 17;
        public int ColumnIndex_FTP2 = 18;
        public int ColumnIndex_PRE2 = 19;
        public int ColumnIndex_INK2 = 20;
        public int ColumnIndex_RIP2 = 21;
        public int ColumnIndex_Rdy2 = 22;
        public int ColumnIndex_Appr2 = 23;
        public int ColumnIndex_CTP2 = 24;
        public int ColumnIndex_Bend2 = 25;
        public int ColumnIndex_Preset2 = 26;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ((string)Session["UserName"] == null)
				Response.Redirect("~/SessionTimeout.htm");

			if ((string)Session["UserName"] == "")
				Response.Redirect("/Denied.htm");


          
            Telerik.Web.UI.RadToolBarButton btn = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("HideFinished");
            if (btn != null)
                Session["HideFinished"] = btn.Checked;

			if (!this.IsPostBack) 
			{
                SetLanguage();
                SetAccess();
			}

            SetScreenSize();

            if (!this.IsPostBack)
            {
                lblError.Text = "";
                GetFilteredGrid();
            }
           

          //  SetToolbarLabel();

            SetRefreshheader();

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
                w = Globals.TryParseCookie(Request, "ScreenWidth", 800);
                h = Globals.TryParseCookie(Request, "ScreenHeight", 600);
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
                    string s = Request.Cookies["ScrollPosY"].Value;
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

            RadGrid1.Height = h - 6;
            RadGrid1.Width = (16 * 35) + (2 * 42) + (6 * 38) + 20 + 35; // nWindowWidth;
            
        }

        private void SetRefreshheader()
        {
            // int nRefreshTime = (int)Session["RefreshTime"];
            //if (nRefreshTime > 0)
            //  Response.AddHeader("Refresh", nRefreshTime.ToString());

            if ((bool)Application["NoCache"])
            {
                Response.AppendHeader("cache-control", "private");
                Response.AppendHeader("pragma", "no-cache");
                Response.AppendHeader("expires", "Fri, 30 Oct 1998 14:19:41 GMT");
                Response.CacheControl = "Private";
                Response.Cache.SetNoStore();
            }

        }
        private void SetToolbarLabel()
        {
           
            DateTime pubdate = (DateTime)Session["SelectedPubDate"];

            SetRadToolbarLabel("Item3","FilterLabel", pubdate.ToShortDateString() + " " + (string)Session["SelectedPublication"] + " " + (string)Session["SelectedEdition"] + " " + (string)Session["SelectedSection"]);
        }


		private void ReBind()
		{
            Telerik.Web.UI.RadToolBarButton btn = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("HideFinished");
            if (btn != null)
                Session["HideFinished"] = btn.Checked;

            SetScreenSize();

			GetFilteredGrid();
		}

	
		protected void SetLanguage()
		{
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            SetRadToolbarLabel("Refresh", Global.rm.GetString("txtRefresh"));
            SetRadToolbarLabel("HideFinished", Global.rm.GetString("txtHideDone"));

			lblChooseProduct.Text = Global.rm.GetString("txtChooseProduct");
		}

		protected void SetAccess()
		{
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
	
	
		private void GetFilteredGrid()
		{
			//DataSet dataSet  =  (DataSet) Cache["PageTableCache"];

			if ((string)Session["SelectedPublication"] == "")
			{
				lblChooseProduct.Visible = true;
				return;
			}
			lblChooseProduct.Visible = false;

			CCDBaccess db = new CCDBaccess();

			string errmsg = "";
			DataTable tbl = null;
 
            tbl = db.GetTablePageStatusCollection((bool)Session["HideFinished"], out errmsg);

            if (tbl != null)
            {
                int rowCount = tbl.Rows.Count;

                if (rowCount > 20)
                {
                    int n2 = rowCount / 2;

                    for (int i=0; i<n2; i++) 
                        for (int j=0; j<12; j++)
                           tbl.Rows[i][j+13] = tbl.Rows[i+n2][j];

                    for (int i = rowCount - 1; i >= n2; i--)
                        tbl.Rows[i].Delete();
                }
			
				lblError.Text = "";

                RadGrid1.DataSource = tbl;
                RadGrid1.DataBind();


			}
			else
				lblError.Text = errmsg;
		}

         protected void RadToolBar1_ButtonClick(object sender, Telerik.Web.UI.RadToolBarEventArgs e)
		{
            if (e.Item.Value == "Refresh" || e.Item.Value == "HideFinished")
			{
				ReBind();
			}
		}


        private System.Drawing.Color GetCellColor(string s)
        {
            int i = s.IndexOf(';');
            if (i != -1)
                s = s.Substring(0, i);
            int n = Globals.TryParse(s, 0);
            n = n & 0xFF;
            if (n == 1)
                return System.Drawing.Color.Yellow;
            if (n == 2)
                return System.Drawing.Color.Green;
            if (n == 3)
                return System.Drawing.Color.Red;
            if (n == 4)
                return System.Drawing.Color.Orange;

            return System.Drawing.Color.Transparent;
        }

        private string GetCellVersion(String s)
        {
            int i = s.IndexOf(';');
            if (i != -1)
                s = s.Substring(0, i);
            int n = Globals.TryParse(s, 0);
            n = (n & 0xFF00) >> 8;

            return n > 0 ? n.ToString() : "";
        }

        private string GetCellMessage(String s)
        {
            int i = s.IndexOf(';');
            if (i == -1)
                return "";
            if (s.Length > i + 1)
            {
                string s2 = s.Substring(i + 1);
                int j = s2.IndexOf(';');
                if (j != -1)
                    return s2.Substring(0, j);
                else
                    return s2;
            }

            return "";

        }

        private string GetCellEventTime(String s)
        {
            int i = s.LastIndexOf(';');
            if (i == -1)
                return "";
            if (s.Length > i + 1)
            {
                string s1 = s.Substring(i + 1);
                if (s1 != "01-01 00:00:00")
                    return s1 != "01-01 00:00:00" ? s1 : "";
            }
            return "";

        }

        protected void RadGrid1_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            int m = e.Item.Cells.Count;
            if (e.Item.ItemType == GridItemType.Header)
            {
                for (int i = 0; i < m; i++) 
                {
                    e.Item.Cells[i].Wrap = false;
                    //e.Item.Cells[i].Text = e.Item.Cells[i].Text.Replace('2',' ');
                    if (e.Item.Cells[i].Text == "XXX")
                        e.Item.Cells[i].Text = "   ";
                }


            }

            if ((e.Item.ItemType == GridItemType.Item) ||
                (e.Item.ItemType == GridItemType.AlternatingItem) ||
                (e.Item.ItemType == GridItemType.SelectedItem))
            {
                for (int i = 0; i < m; i++)
                    e.Item.Cells[i].Wrap = false;

              
                TableCell cell = (TableCell)e.Item.Cells[ColumnIndex_FTP];
                cell.BackColor = GetCellColor(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Green)
                    cell.ForeColor = System.Drawing.Color.White;                
                string t = GetCellEventTime(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Orange || cell.BackColor == System.Drawing.Color.Red)
                    cell.ToolTip = t + "\r\n" + GetCellMessage(cell.Text);
                else if (cell.BackColor != System.Drawing.Color.White)
                    cell.ToolTip = t;
                cell.Text = GetCellVersion(cell.Text);

                cell = (TableCell)e.Item.Cells[ColumnIndex_PRE];
                cell.BackColor = GetCellColor(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Green)
                    cell.ForeColor = System.Drawing.Color.White;
                t = GetCellEventTime(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Orange || cell.BackColor == System.Drawing.Color.Red)
                    cell.ToolTip = t + "\r\n" + GetCellMessage(cell.Text);
                else if (cell.BackColor != System.Drawing.Color.White)
                    cell.ToolTip = t;
                cell.Text = GetCellVersion(cell.Text);

                cell = (TableCell)e.Item.Cells[ColumnIndex_INK];
                cell.BackColor = GetCellColor(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Green)
                    cell.ForeColor = System.Drawing.Color.White;
                t = GetCellEventTime(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Orange || cell.BackColor == System.Drawing.Color.Red)
                    cell.ToolTip = t + "\r\n" + GetCellMessage(cell.Text);
                else if (cell.BackColor != System.Drawing.Color.White)
                    cell.ToolTip = t;                 
                cell.Text = GetCellVersion(cell.Text);

                cell = (TableCell)e.Item.Cells[ColumnIndex_RIP];
                cell.BackColor = GetCellColor(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Green)
                    cell.ForeColor = System.Drawing.Color.White;
                t = GetCellEventTime(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Orange || cell.BackColor == System.Drawing.Color.Red)
                    cell.ToolTip = t + "\r\n" + GetCellMessage(cell.Text);
                else if (cell.BackColor != System.Drawing.Color.White)
                    cell.ToolTip = t;
                cell.Text = GetCellVersion(cell.Text);
               
                cell = (TableCell)e.Item.Cells[ColumnIndex_Rdy];
                cell.BackColor = GetCellColor(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Green)
                    cell.ForeColor = System.Drawing.Color.White;                
                t = GetCellEventTime(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Orange || cell.BackColor == System.Drawing.Color.Red)
                    cell.ToolTip = t + "\r\n" + GetCellMessage(cell.Text);
                else if (cell.BackColor != System.Drawing.Color.White)
                    cell.ToolTip = t;
                cell.Text = GetCellVersion(cell.Text);

                cell = (TableCell)e.Item.Cells[ColumnIndex_Appr];
                cell.BackColor = GetCellColor(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Green)
                    cell.ForeColor = System.Drawing.Color.White;
                t = GetCellEventTime(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Orange || cell.BackColor == System.Drawing.Color.Red)
                    cell.ToolTip = t + "\r\n" + GetCellMessage(cell.Text);
                else if (cell.BackColor != System.Drawing.Color.White)
                    cell.ToolTip = t;
                cell.Text = GetCellVersion(cell.Text);

                cell = (TableCell)e.Item.Cells[ColumnIndex_CTP];
                cell.BackColor = GetCellColor(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Green)
                    cell.ForeColor = System.Drawing.Color.White;
                t = GetCellEventTime(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Orange || cell.BackColor == System.Drawing.Color.Red)
                    cell.ToolTip = t + "\r\n" + GetCellMessage(cell.Text);
                else if (cell.BackColor != System.Drawing.Color.White)
                    cell.ToolTip = t;
                cell.Text = GetCellVersion(cell.Text);

                cell = (TableCell)e.Item.Cells[ColumnIndex_Bend];
                cell.BackColor = GetCellColor(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Green)
                    cell.ForeColor = System.Drawing.Color.White;
                cell.Text = GetCellVersion(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Orange || cell.BackColor == System.Drawing.Color.Red)
                    cell.ToolTip = t + "\r\n" + GetCellMessage(cell.Text);
                else if (cell.BackColor != System.Drawing.Color.White)
                    cell.ToolTip = t;
                t = GetCellEventTime(cell.Text);

                cell = (TableCell)e.Item.Cells[ColumnIndex_Preset];
                cell.BackColor = GetCellColor(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Green)
                    cell.ForeColor = System.Drawing.Color.White;                
                t = GetCellEventTime(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Orange || cell.BackColor == System.Drawing.Color.Red)
                    cell.ToolTip = t + "\r\n" + GetCellMessage(cell.Text);
                else if (cell.BackColor != System.Drawing.Color.White)
                    cell.ToolTip = t;
                cell.Text = GetCellVersion(cell.Text);

                cell = (TableCell)e.Item.Cells[ColumnIndex_Divider];
                cell.BackColor = System.Drawing.Color.LightBlue;


                cell = (TableCell)e.Item.Cells[ColumnIndex_FTP2];
                cell.BackColor = GetCellColor(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Green)
                    cell.ForeColor = System.Drawing.Color.White;                
                t = GetCellEventTime(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Orange || cell.BackColor == System.Drawing.Color.Red)
                    cell.ToolTip = t + "\r\n" + GetCellMessage(cell.Text);
                else if (cell.BackColor != System.Drawing.Color.White)
                    cell.ToolTip = t;
                cell.Text = GetCellVersion(cell.Text);

                cell = (TableCell)e.Item.Cells[ColumnIndex_PRE2];
                cell.BackColor = GetCellColor(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Green)
                    cell.ForeColor = System.Drawing.Color.White;                
                t = GetCellEventTime(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Orange || cell.BackColor == System.Drawing.Color.Red)
                    cell.ToolTip = t + "\r\n" + GetCellMessage(cell.Text);
                else if (cell.BackColor != System.Drawing.Color.White)
                    cell.ToolTip = t;
                cell.Text = GetCellVersion(cell.Text);

                cell = (TableCell)e.Item.Cells[ColumnIndex_INK2];
                cell.BackColor = GetCellColor(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Green)
                    cell.ForeColor = System.Drawing.Color.White; 
                t = GetCellEventTime(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Orange || cell.BackColor == System.Drawing.Color.Red)
                    cell.ToolTip = t + "\r\n" + GetCellMessage(cell.Text);
                else if (cell.BackColor != System.Drawing.Color.White)
                    cell.ToolTip = t;
                cell.Text = GetCellVersion(cell.Text);

                cell = (TableCell)e.Item.Cells[ColumnIndex_RIP2];
                cell.BackColor = GetCellColor(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Green)
                    cell.ForeColor = System.Drawing.Color.White;                
                t = GetCellEventTime(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Orange || cell.BackColor == System.Drawing.Color.Red)
                    cell.ToolTip = t + "\r\n" + GetCellMessage(cell.Text);
                else if (cell.BackColor != System.Drawing.Color.White)
                    cell.ToolTip = t;
                cell.Text = GetCellVersion(cell.Text);

                cell = (TableCell)e.Item.Cells[ColumnIndex_Rdy2];
                cell.BackColor = GetCellColor(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Green)
                    cell.ForeColor = System.Drawing.Color.White;                
                t = GetCellEventTime(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Orange || cell.BackColor == System.Drawing.Color.Red)
                    cell.ToolTip = t + "\r\n" + GetCellMessage(cell.Text);
                else if (cell.BackColor != System.Drawing.Color.White)
                    cell.ToolTip = t;
                cell.Text = GetCellVersion(cell.Text);

                cell = (TableCell)e.Item.Cells[ColumnIndex_Appr2];
                cell.BackColor = GetCellColor(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Green)
                    cell.ForeColor = System.Drawing.Color.White;
                t = GetCellEventTime(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Orange || cell.BackColor == System.Drawing.Color.Red)
                    cell.ToolTip = t + "\r\n" + GetCellMessage(cell.Text);
                else if (cell.BackColor != System.Drawing.Color.White)
                    cell.ToolTip = t;
                cell.Text = GetCellVersion(cell.Text);

                cell = (TableCell)e.Item.Cells[ColumnIndex_CTP2];
                cell.BackColor = GetCellColor(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Green)
                    cell.ForeColor = System.Drawing.Color.White;
                t = GetCellEventTime(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Orange || cell.BackColor == System.Drawing.Color.Red)
                    cell.ToolTip = t + "\r\n" + GetCellMessage(cell.Text);
                else if (cell.BackColor != System.Drawing.Color.White)
                    cell.ToolTip = t;
                cell.Text = GetCellVersion(cell.Text);

                cell = (TableCell)e.Item.Cells[ColumnIndex_Bend2];
                cell.BackColor = GetCellColor(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Green)
                    cell.ForeColor = System.Drawing.Color.White;
                t = GetCellEventTime(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Orange || cell.BackColor == System.Drawing.Color.Red)
                    cell.ToolTip = t + "\r\n" + GetCellMessage(cell.Text);
                else if (cell.BackColor != System.Drawing.Color.White)
                    cell.ToolTip = t;
                cell.Text = GetCellVersion(cell.Text);

                cell = (TableCell)e.Item.Cells[ColumnIndex_Preset2];
                cell.BackColor = GetCellColor(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Green)
                    cell.ForeColor = System.Drawing.Color.White;                
                t = GetCellEventTime(cell.Text);
                if (cell.BackColor == System.Drawing.Color.Orange || cell.BackColor == System.Drawing.Color.Red)
                    cell.ToolTip = t + "\r\n" + GetCellMessage(cell.Text);
                else if (cell.BackColor != System.Drawing.Color.White)
                    cell.ToolTip = t;
                cell.Text = GetCellVersion(cell.Text);

            }
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
