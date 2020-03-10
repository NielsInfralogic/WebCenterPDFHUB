using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Globalization;
using System.Resources;
using System.Threading;
using WebCenter4.Classes;

namespace WebCenter4.Views
{
    public partial class UnknownFiles : System.Web.UI.Page
    {
        public int nWindowHeight;
        public int nWindowWidth;
        public int nRefreshTime;
        public int nScollPos = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if ((string)Session["UserName"] == null)
                Response.Redirect("../SessionTimeout.htm");

            if ((string)Session["UserName"] == "")
                Response.Redirect("../Denied.htm");

            nRefreshTime = (int)Session["RefreshTime"];

            SetLanguage();
            SetAccess();
            if (!this.IsPostBack ||txtReturnedFromPriority.Text == "1")
            {
                txtReturnedFromPriority.Text = "";
                ReBind();
            }


        }

        private void SetWindowSize()
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
        }

        private void ReBind()
        {
            SetWindowSize();
            //DataGrid1.Height = Unit.Pixel(h-25);
            //DataGrid1.Width = Unit.Pixel(w-25);

            lblError.Text = "";
            GetFilteredGrid();
        }

        protected void SetLanguage()
        {
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            SetRadToolbarLabel("Refresh", Global.rm.GetString("txtRefresh"));

            SetRadToolbarLabel("Retry", Global.rm.GetString("txtRetry"));
            SetRadToolbarTooltip("Retry", Global.rm.GetString("txtTooltipRetry"));
            SetRadToolbarLabel("Delete", Global.rm.GetString("txtDelete"));
            SetRadToolbarTooltip("Delete", Global.rm.GetString("txtTooltipDelete"));
            SetRadToolbarLabel("Rename", Global.rm.GetString("txtRename"));
            
        }

        protected void SetAccess()
        {

        }

        private void GetFilteredGrid()
        {
            //DataSet dataSet  =  (DataSet) Cache["PageTableCache"];

            CCDBaccess db = new CCDBaccess();

            string errmsg = "";
            DataTable dt = db.GetUnknownFilesCollection(out errmsg);

            if (dt != null)
            {
                lblError.Text = "";

                DataGrid1.DataSource = dt.DefaultView;
                DataGrid1.DataBind();
            }
            else
                lblError.Text = errmsg;
        }

        public void DataGrid1_CheckedChanged(object sender, System.EventArgs e)
        {
            CheckBox chkTemp = (CheckBox)sender;
            DataGridItem dgi = (DataGridItem)chkTemp.Parent.Parent;
            if (chkTemp.Checked)
            {
                dgi.BackColor = DataGrid1.SelectedItemStyle.BackColor;
                dgi.ForeColor = DataGrid1.SelectedItemStyle.ForeColor;
            }
            else
            {
                dgi.BackColor = DataGrid1.ItemStyle.BackColor;
                dgi.ForeColor = DataGrid1.ItemStyle.ForeColor;
            }
        }

        protected void RadToolBar1_ButtonClick(object sender, Telerik.Web.UI.RadToolBarEventArgs e)
        {
            if (e.Item.Value == "Refresh")
            {
                //DataSet dataSet  =  (DataSet) Cache["PageTableCache"];
                ReBind();
            }
            else if (e.Item.Value == "Retry")
            {

                CCDBaccess db = new CCDBaccess();
                string errmsg = "";

                foreach (DataGridItem dataItem in DataGrid1.Items)
                {
                    CheckBox cb = (CheckBox)dataItem.Cells[0].Controls[1];
                    if (cb.Checked)
                    {
                        try
                        {
                            TableCell cell = dataItem.Cells[1];
                            TableCell cell2 = dataItem.Cells[2];
                            db.UpdateUnknownFiles(cell.Text, cell2.Text, 1, "", out errmsg);
                        }
                        catch
                        { }

                    }
                }
                GetFilteredGrid();
            }
            else if (e.Item.Value == "Delete")
            {

                CCDBaccess db = new CCDBaccess();
                string errmsg = "";

                foreach (DataGridItem dataItem in DataGrid1.Items)
                {
                    CheckBox cb = (CheckBox)dataItem.Cells[0].Controls[1];
                    if (cb.Checked)
                    {
                        try
                        {
                            TableCell cell = dataItem.Cells[1];
                            TableCell cell2 = dataItem.Cells[2];
                            db.UpdateUnknownFiles(cell.Text, cell2.Text, 3, "", out errmsg);
                        }
                        catch
                        { }

                    }
                }
                GetFilteredGrid();
            }
            else if (e.Item.Value == "Rename")
            {

                CCDBaccess db = new CCDBaccess();
                string errmsg = "";

                foreach (DataGridItem dataItem in DataGrid1.Items)
                {
                    CheckBox cb = (CheckBox)dataItem.Cells[0].Controls[1];
                    if (cb.Checked)
                    {
                        try
                        {
                            TableCell cell = dataItem.Cells[1];
                            TableCell cell2 = dataItem.Cells[2];
                            db.UpdateUnknownFiles(cell.Text, cell2.Text, 3, "", out errmsg);
                        }
                        catch
                        { }

                    }
                }
                GetFilteredGrid();
            }
		
        }

        protected void DataGrid1_SelectedIndexChanged(object sender, EventArgs e)
        {

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