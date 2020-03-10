﻿using System;
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
    public partial class LogView : System.Web.UI.Page
    {
        public int nWindowHeight;
        public int nWindowWidth;
        public int nRefreshTime;
        public int nScollPos = 0;

        private DateTime oldDate;

        protected global::System.Web.UI.WebControls.DropDownList DropDownList1;

        protected void Page_Load(object sender, EventArgs e)
        {
            if ((string)Session["UserName"] == null)
                Response.Redirect("../SessionTimeout.htm");

            if ((string)Session["UserName"] == "")
                Response.Redirect("../Denied.htm");

            oldDate = new DateTime(1975, 1, 1, 0, 0, 0);

            nRefreshTime = (int)Session["RefreshTime"];

            SetLanguage();
            SetAccess();
            if (!this.IsPostBack )
            {
                CreateLocationDropDownTelerik();
                SetDefaultSelection();
                Session["LogLastDateTime"] = oldDate;
                ReBind();
            }

        }

        private void CreateLocationDropDownTelerik()
        {          
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue("Item1");
            if (item == null)
                return;
            DropDownList1 = (DropDownList)item.FindControl("PressSelector");
            if (DropDownList1 == null)
                return;

            string locationsallowed = (string)Application["LogLocations"];
            string[] loclist = locationsallowed.Split(',');
            foreach (string s in loclist)
                DropDownList1.Items.Add(s);

            if ((string)Session["SelectedLogLocation"] == "")
                DropDownList1.SelectedIndex = 0;
            else
                DropDownList1.SelectedValue = (string)Session["SelectedLogLocation"];
        }

        public void OnSelChangeLocation(object sender, System.EventArgs e)
        {
            System.Web.UI.WebControls.DropDownList dropdown = (System.Web.UI.WebControls.DropDownList)sender;
            Session["SelectedLogLocation"] = dropdown.SelectedItem.Text;
            if ((string)Session["SelectedLogLocation"] != "")
                ReBind();
        }

        private void SetDefaultSelection()
        {
            if ((string)Session["SelectedLogLocation"] == null)
                Session["SelectedLogLocation"] = "";

            if ((string)Session["SelectedLogLocation"] == "")
            {
                string locationsallowed = (string)Application["LogLocations"];
                string[] loclist = locationsallowed.Split(',');
                if (loclist.Count() > 0)
                    Session["SelectedLogLocation"] = loclist[0];
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
            SetRadToolbarLabel("Item1", "txtPress", (bool)Application["LocationIsPress"] ? Global.rm.GetString("txtPress") : Global.rm.GetString("txtLocation"));

            //            SetRadToolbarLabel("Delete", Global.rm.GetString("txtDelete"));

        }

        protected void SetAccess()
        {

        }

        private void GetFilteredGrid()
        {
            //DataSet dataSet  =  (DataSet) Cache["PageTableCache"];

            string errmsg = "";
            DateTime tTimeStamp = new DateTime();
            CCDBaccess db = new CCDBaccess();

            tTimeStamp = (DateTime)Session["LogLastDateTime"];


            DataTable dt = db.GetLogCollection((string)Session["SelectedLogLocation"], ref tTimeStamp, out errmsg);

            if (dt != null)
            {
                lblError.Text = "";
                Session["LogLastDateTime"] = tTimeStamp;
                DataGrid1.DataSource = dt.DefaultView;
                DataGrid1.DataBind();
            }
            else
                lblError.Text = errmsg;
        }

        protected void RadToolBar1_ButtonClick(object sender, Telerik.Web.UI.RadToolBarEventArgs e)
        {
            if (e.Item.Value == "Refresh")
            {
                //DataSet dataSet  =  (DataSet) Cache["PageTableCache"];
                ReBind();
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