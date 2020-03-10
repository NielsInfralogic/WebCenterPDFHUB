using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Globalization;
using System.Resources;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Drawing.Imaging;
using WebCenter4.Classes;
using System.Threading;
using Telerik.Web.UI;

namespace WebCenter4.Views
{
    public partial class ClassicTableView2 : System.Web.UI.Page
    {
        protected HtmlInputHidden HiddenX;
        protected HtmlInputHidden HiddenY;
        protected HtmlInputHidden HiddenScrollPos;


        protected int nRefreshTime;
        public int nScollPos = 0;


        public int ColumnIndex_Edition = 0;
        public int ColumnIndex_Section = 1;
        public int ColumnIndex_Page = 2;
        public int ColumnIndex_Color = 3;
        public int ColumnIndex_Status = 4;
        public int ColumnIndex_Version = 5;
        public int ColumnIndex_Approved = 6;
        public int ColumnIndex_Hold = 7;
        public int ColumnIndex_Priority = 8;
        public int ColumnIndex_Template = 9;
        public int ColumnIndex_Device = 10;
        public int ColumnIndex_ExternalStatus = 11;
        public int ColumnIndex_CopyNumber = 12;
        public int ColumnIndex_Pagination = 13;
        public int ColumnIndex_Press = 14;
        public int ColumnIndex_LastError = 15;
        public int ColumnIndex_Comment = 16;
        public int ColumnIndex_Deadline = 17;
        public int ColumnIndex_ProofStatus = 18;
        public int ColumnIndex_Location = 19;
        public int ColumnIndex_SheetNumber = 20;
        public int ColumnIndex_SheetSide = 21;
        public int ColumnIndex_PagePositions = 22;
        public int ColumnIndex_PageType = 23;
        public int ColumnIndex_PagesPerPlate = 24;
        public int ColumnIndex_InputTime = 25;
        public int ColumnIndex_ApprovalTime = 26;
        public int ColumnIndex_OutputTime = 27;
        public int ColumnIndex_VerifyTime = 28;
        public int ColumnIndex_Active = 29;
        public int ColumnIndex_Unique = 30;


        protected void Page_Load(object sender, EventArgs e)
        {
            if ((string)Session["UserName"] == null)
                Response.Redirect("~/SessionTimeout.htm");

            if ((string)Session["UserName"] == "")
                Response.Redirect("/Denied.htm");

            Session["SelectedSeps"] = null;
            SetScreenSize();

            if (!this.IsPostBack)
            {
                Telerik.Web.UI.RadToolBarButton btn = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("HideFinished");
                if (btn != null)
                    btn.Checked = (bool)Session["HideFinished"];

                btn = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("Separations");
                if (btn != null)
                    btn.Checked = (bool)Session["SelectedAllSeps"];

                 btn = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("HideApproved");
                 if (btn != null)
                     btn.Checked = (bool)Session["HideApproved"];

                btn = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("HideCommon");
                 if (btn != null)
                     btn.Checked = (bool)Session["HideCommon"];

                 btn = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("Reimage");
                 if (btn != null)
                     btn.Visible = (bool)Session["MayReimage"] || (bool)Session["IsAdmin"];

                 btn = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("HideCopies");
                 if (btn != null)
                     btn.Checked = (bool)Session["SelectedAllCopies"];

                SetLanguage();
                
                SetAccess();

                // Defaults
                ViewState["HideEditionColumn"] = false;
                ViewState["HideSectiontionColumn"] = false;
                ViewState["DataSortExpression"] = "";
                ViewState["MasterSetColumn"] = 1;
                ViewState["SetColumn"] = 1;
                ViewState["FlatSeparationColumn"] = 1;
                ViewState["FlatSeparationSetColumn"] = 1;
                ViewState["DefColumnOrder"] = (string)Application["ColumnOrder"];

                ViewState["SectionColumn"] = 1;
                ViewState["EditionColumn"] = 1;

                ViewState["ActiveColumn"] = 50;
                ViewState["PrioColumn"] = 50;
                ViewState["ColorColumn"] = 1;
                Session["TableSelectMode"] = "Page";


                lblError.Text = "";
                ReBind();
            }

            //Loop through all windows in the WindowManager.Windows collection
            foreach (Telerik.Web.UI.RadWindow win in RadWindowManager1.Windows)
            {
                //Set whether the first window will be visible on page load
                win.VisibleOnPageLoad = false;
            }

   
            if (HiddenReturendFromPopup.Value == "1")
            {
                HiddenReturendFromPopup.Value = "0";
                ReBind();
            }

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
                w = Globals.TryParseCookie(Request, "ScreenHeightList", 800);
                h = Globals.TryParseCookie(Request, "ScreenHeightList", 600);
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

         //   if (h>200)
           //     DataGrid1.Height = h - 30;

            RadGrid1.Height = h - 12;
//            RadGrid1.Width = (16 * 35) + (2 * 42) + (6 * 38) + 20 + 35; // nWindowWidth;

        }

        private void SetRefreshheader()
        {
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

            SetRadToolbarLabel("Item3", "FilterLabel", pubdate.ToShortDateString() + " " + (string)Session["SelectedPublication"] + " " + (string)Session["SelectedEdition"] + " " + (string)Session["SelectedSection"]);
        }

        protected void SetLanguage()
        {
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            SetRadToolbarLabel("Refresh", Global.rm.GetString("txtRefresh"));
            SetRadToolbarLabel("HideFinished", Global.rm.GetString("txtHideDone"));
            SetRadToolbarLabel("HideApproved", Global.rm.GetString("txtHideApprovedPages"));
            SetRadToolbarTooltip("HideApproved", Global.rm.GetString("txtTooltipHideApprovedPages"));
            SetRadToolbarLabel("HideCommon", Global.rm.GetString("txtHideDuplicates"));
            SetRadToolbarTooltip("HideCommon", Global.rm.GetString("txtTooltipHideDuplicates"));

            SetRadToolbarLabel("ShowCopies", Global.rm.GetString("txtAllCopies"));
            SetRadToolbarTooltip("ShowCopies", Global.rm.GetString("txtTooltipShowCopies"));

            SetRadToolbarLabel("Reimage", Global.rm.GetString("txtReImage"));
            SetRadToolbarTooltip("Reimage", Global.rm.GetString("txtTooltipReImage"));

            lblChooseProduct.Text = Global.rm.GetString("txtChooseProduct");
        }

        protected void SetAccess()
        {
        }

        public void OnSelChangeSelectMode(object sender, System.EventArgs e)
        {
            System.Web.UI.WebControls.DropDownList dropdown = (System.Web.UI.WebControls.DropDownList)sender;
            Session["TableSelectMode"] = dropdown.SelectedItem.Text;
            lblError.Text = "";
            ReBind();

        }


        private DataTable GetDataTable()
        {
            RegisterToolbarSettings();

            CCDBaccess db = new CCDBaccess();

            string errmsg = "";
            string s;
            DataTable tbl = null;
            if ((bool)Session["SelectedAllSeps"])
                tbl = db.GetTablePageSeparationCollection(false, (bool)Session["SelectedAllCopies"], out errmsg);
            else
                tbl = db.GetTablePageCollection(false, (bool)Session["SelectedAllCopies"], out errmsg);

            if (tbl == null)
            {
                lblError.Text = errmsg;
                return null;
            }

            lblError.Text = "";
            ViewState["MasterSetColumn"] = tbl.Columns["MasterCopySeparationSet"].Ordinal + 1;
            ViewState["FlatSeparationColumn"] = tbl.Columns["FlatSeparation"].Ordinal + 1;
            ViewState["FlatSeparationSetColumn"] = tbl.Columns["FlatSeparationSet"].Ordinal + 1;
            ViewState["PrioColumn"] = tbl.Columns["Priority"].Ordinal + 1;
            ViewState["ColorColumn"] = tbl.Columns["Color"].Ordinal + 1;
            ViewState["ActiveColumn"] = tbl.Columns["Active"].Ordinal + 1;
            ViewState["SetColumn"] = tbl.Columns["SeparationSet"].Ordinal + 1;
            ViewState["TemplateColumn"] = tbl.Columns["Template"].Ordinal + 1;
            ViewState["EditionColumn"] = tbl.Columns["Edition"].Ordinal + 1;
            ViewState["SectionColumn"] = tbl.Columns["Section"].Ordinal + 1;

          /*  string s = " ";
            foreach (DataColumn col in tbl.Columns)
            {
                if (s != "")
                    s += ",";
                s += col.ColumnName;
            }

            ViewState["DefColumnOrder"] = s;

            DataView dv = (DataView)RadGrid1.DataSource;
            foreach (DataColumn col in dv.Table.Columns)
            {
                string ss = Global.rm.GetString("txt" + col.ColumnName);
                if (ss != null && ss != "" && ss != " ")
                    col.ColumnName = ss;

            }*/

            List<string> uniqueSections = new List<string>();
            List<string> uniqueEditions = new List<string>();

            foreach (DataRow row in tbl.Rows)
            {
                s = (string)row["Section"];
                if (uniqueSections.Contains(s) == false)
                    uniqueSections.Add(s);
                s = (string)row["Edition"];
                if (uniqueEditions.Contains(s) == false)
                    uniqueEditions.Add(s);
            }

            ViewState["HideEditionColumn"] = uniqueEditions.Count < 2;
            ViewState["HideSectionColumn"] = uniqueSections.Count < 2;


            return tbl;

        }

        private void RegisterToolbarSettings()
        {
            Telerik.Web.UI.RadToolBarButton btn = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("HideFinished");
            if (btn != null)
                Session["HideFinished"] = btn.Checked;

            btn = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("Separations");
            if (btn != null)
                Session["SelectedAllSeps"] = btn.Checked;

            btn = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("HideApproved");
            if (btn != null)
                Session["HideApproved"] = btn.Checked;

            btn = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("HideCommon");
            if (btn != null)
                Session["HideCommon"] = btn.Checked;

            btn = (Telerik.Web.UI.RadToolBarButton)RadToolBar1.FindItemByValue("ShowCopies");
            if (btn != null)
                Session["SelectedAllCopies"] = btn.Checked;

        }

        private void ReBind()
		{
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            if ((string)Session["SelectedPublication"] == "")
			{
				lblChooseProduct.Visible = true;
				return;
			}
            lblChooseProduct.Visible = false;

            RadGrid1.DataSource = GetDataTable();
            RadGrid1.DataBind();

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

        protected void RadToolBar1_ButtonClick(object sender, Telerik.Web.UI.RadToolBarEventArgs e)
        {
            if (e.Item.Value == "ExternalStatus")
            {
                string selectedmasterSets = "";
                foreach (GridDataItem item in RadGrid1.SelectedItems)
                {
                    string s = item["MasterCopySeparationSet"].Text.ToString();
                    int n = 0;
                    Int32.TryParse(s, out n);
                    if (n > 0)
                    {
                        if (selectedmasterSets != "")
                            selectedmasterSets += ",";
                        selectedmasterSets += s;
                    }
                }

                if (selectedmasterSets != "")
                {
                    Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowExternalStatus");
                    mywindow.Title = Global.rm.GetString("txtSetExternalStatus");

                    mywindow.NavigateUrl = "SetExternalStatus.aspx?MasterCopySeparationSetList=" + selectedmasterSets;

                    mywindow.VisibleOnPageLoad = true; ;
                }
            }


         /*   if (e.Item.Value == "Reimage")
            {
                CCDBaccess db = new CCDBaccess();

                string errmsg = "";
                foreach (DataGridItem dataItem in DataGrid1.Items)
                {
                    CheckBox cb = (CheckBox)dataItem.Cells[0].Controls[1];
                    if (cb.Checked)
                    {
                        if ((bool)Session["SelectedAllSeps"] == false)
                        {
                            try
                            {
                                // All page colors
                                TableCell cell = dataItem.Cells[(int)ViewState["MasterSetColumn"]];
                                db.ReimageFlatSeparation(Int32.Parse(cell.Text), -1, "", out errmsg);
                            }
                            catch
                            { }
                        }
                        else
                        {
                            try
                            {
                                // Specific color separation
                                TableCell cell = dataItem.Cells[(int)ViewState["MasterSetColumn"]];
                                TableCell cell2 = dataItem.Cells[(int)ViewState["ColorColumn"]];
                                db.ReimageFlatSeparation(Int32.Parse(cell.Text), -1, cell2.Text, out errmsg);
                            }
                            catch
                            { }
                        }
                    }
                }


            }
        //    if (e.Item.Value == "Refresh" || e.Item.Value == "HideFinished" || e.Item.Value == "Separations")
          //  {
                ReBind();
            //}
            */

            ReBind();
        }


        /*
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
     
             protected void DataGrid1_ItemDataBound(object sender, DataGridItemEventArgs e)
             {
                 int m = e.Item.Cells.Count;
                 string colorder = (string)ViewState["DefColumnOrder"];

                 if (e.Item.ItemType == ListItemType.Header && colorder != "")
                 {
                     for (int i = 0; i < m; i++)
                         e.Item.Cells[i].Wrap = false;

                     string[] cols = colorder.Split(',');

                     for (int i = 0; i < cols.Length; i++)
                     {
                         if (cols[i] == "Edition")
                             ColumnIndex_Edition = i;
                         else if (cols[i] == "Section")
                             ColumnIndex_Section = i;
                         else if (cols[i] == "Page")
                             ColumnIndex_Page = i;
                         else if (cols[i] == "Color")
                             ColumnIndex_Color = i;
                         else if (cols[i] == "Status")
                             ColumnIndex_Status = i;
                         else if (cols[i] == "Version")
                             ColumnIndex_Version = i;
                         else if (cols[i] == "Approval")
                             ColumnIndex_Approved = i;
                         else if (cols[i] == "Hold")
                             ColumnIndex_Hold = i;
                         else if (cols[i] == "Priority")
                             ColumnIndex_Priority = i;
                         else if (cols[i] == "Template")
                             ColumnIndex_Template = i;
                         else if (cols[i] == "Device")
                             ColumnIndex_Device = i;
                         else if (cols[i] == "ExternalStatus")
                             ColumnIndex_ExternalStatus = i;
                         else if (cols[i] == "CopyNumber")
                             ColumnIndex_CopyNumber = i;
                         else if (cols[i] == "Pagination")
                             ColumnIndex_Pagination = i;
                         else if (cols[i] == "Press")
                             ColumnIndex_Press = i;
                         else if (cols[i] == "LastError")
                             ColumnIndex_LastError = i;
                         else if (cols[i] == "Comment")
                             ColumnIndex_Comment = i;
                         else if (cols[i] == "Deadline")
                             ColumnIndex_Deadline = i;
                         else if (cols[i] == "ProofStatus")
                             ColumnIndex_ProofStatus = i;
                         else if (cols[i] == "Location")
                             ColumnIndex_Location = i;
                         else if (cols[i] == "SheetNumber")
                             ColumnIndex_SheetNumber = i;
                         else if (cols[i] == "SheetSide")
                             ColumnIndex_SheetSide = i;
                         else if (cols[i] == "PagePositions")
                             ColumnIndex_PagePositions = i;
                         else if (cols[i] == "PageType")
                             ColumnIndex_PageType = i;
                         else if (cols[i] == "PagesPerPlate")
                             ColumnIndex_PagesPerPlate = i;
                         else if (cols[i] == "InputTime")
                             ColumnIndex_InputTime = i;
                         else if (cols[i] == "ApproveTime")
                             ColumnIndex_ApprovalTime = i;
                         else if (cols[i] == "OutputTime")
                             ColumnIndex_OutputTime = i;
                         else if (cols[i] == "VerifyTime")
                             ColumnIndex_VerifyTime = i;
                         else if (cols[i] == "Active")
                             ColumnIndex_Active = i;
                         else if (cols[i] == "Unique")
                             ColumnIndex_Unique = i;
                     }

                     CheckBox cb = (CheckBox)e.Item.Cells[0].Controls[1];
                     cb.ToolTip = Global.rm.GetString("txtTooltipSelectDeselctAll");

                 }


                 if ((e.Item.ItemType == ListItemType.Item) ||
                     (e.Item.ItemType == ListItemType.AlternatingItem) ||
                     (e.Item.ItemType == ListItemType.SelectedItem))
                 {
                     for (int i = 0; i < m; i++)
                         e.Item.Cells[i].Wrap = false;

                     CheckBox cbz = (CheckBox)e.Item.Cells[0].Controls[1];
                     cbz.ToolTip = Global.rm.GetString("txtTooltipSelectDeselct");

                     TableCell cell = (TableCell)e.Item.Cells[ColumnIndex_Active];

                     bool isHidden = (cell.Text == "False") ? true : false;

                     cell = (TableCell)e.Item.Cells[ColumnIndex_Status];
                     cell.BackColor = Globals.GetStatusColorFromName(cell.Text, 0);

                     cell = (TableCell)e.Item.Cells[ColumnIndex_ExternalStatus];
                     cell.BackColor = Globals.GetStatusColorFromName(cell.Text, 1);

                     cell = (TableCell)e.Item.Cells[ColumnIndex_Hold];
                     if (cell.Text == "Released")
                     {
                         cell.BackColor = Color.Green;
                         cell.ForeColor = Color.White;
                         cell.Text = Global.rm.GetString("txtReleased");
                     }
                     else
                     {
                         cell.BackColor = Color.Red;
                         cell.ForeColor = Color.White;
                         cell.Text = Global.rm.GetString("txtOnHold");
                     }

                     cell = (TableCell)e.Item.Cells[ColumnIndex_SheetSide];
                     if (cell.Text == "Front")
                     {
                         cell.Text = Global.rm.GetString("txtFrontSide");
                     }
                     else
                     {
                         cell.Text = Global.rm.GetString("txtBackSide");
                     }

                     cell = (TableCell)e.Item.Cells[ColumnIndex_Approved];
                     if (cell.Text == "2")
                     {
                         cell.Text = Global.rm.GetString("txtRejected");
                         cell.BackColor = Color.Red;
                         cell.ForeColor = Color.White;
                     }
                     else if (cell.Text == "0")
                     {
                         cell.Text = Global.rm.GetString("txtNotApproved");
                         cell.BackColor = Color.LightGray;
                         cell.ForeColor = Color.Black;
                     }
                     else if (cell.Text == "1" || cell.Text == "-1")
                     {
                         cell.Text = Global.rm.GetString("txtApproved");
                         cell.BackColor = Color.LightGreen;
                         cell.ForeColor = Color.Black;
                     }
                     else
                     {
                         cell.Text = "N/A";
                         cell.BackColor = Color.LightGray;
                         cell.ForeColor = Color.Black;
                     }

                     cell = (TableCell)e.Item.Cells[ColumnIndex_Color];
                     String sColorName = cell.Text;
                     //cell.Text = "";

                     if (sColorName.StartsWith(";"))
                         sColorName = sColorName.Substring(1);
                     cell.Text = sColorName;


                     if ((bool)Session["SelectedAllSeps"] == false)
                     {
                         string[] sargs = sColorName.Split(';');
                         if (sargs.Length == 4 && sColorName.IndexOf('C') != -1 && sColorName.IndexOf('M') != -1 && sColorName.IndexOf('Y') != -1 && sColorName.IndexOf('K') != -1)
                         {
                             cell.Text = "CMYK";
                             cell.BackColor = Color.Magenta;
                             cell.ForeColor = Color.Black;
                         }
                         else if (sargs.Length == 1 && sColorName.IndexOf('K') != -1)
                         {
                             cell.BackColor = Color.Black;
                             cell.ForeColor = Color.White;
                         }
                         else
                         {
                             cell.BackColor = Color.Cyan;
                             cell.ForeColor = Color.Black;
                         }
                     }
                     else
                     {
                         string[] sargs = sColorName.Split(';');

                         cell.BackColor = cell.BackColor = Globals.GetColorFromName((string)sargs[0]);
                         float f = cell.BackColor.GetBrightness();
                         cell.ForeColor = f >= 0.5F ? Color.Black : Color.White;

                     }

                     if (isHidden)
                     {
                         string[] cols = colorder.Split(',');
                         for (int i = 0; i < cols.Length; i++)
                         {
                             cell = (TableCell)e.Item.Cells[i];
                             cell.ForeColor = Color.Red;
                         }
                     }
                 }

             }


             protected void DataGrid1_SortCommand(object source, DataGridSortCommandEventArgs e)
             {
                 string[] colNames = {	"Edition",	"Section",	"Page",			"Color",		"Status",		"Version", 
                                         "Approval",		"Hold",		"Priority", "Template", "Device",	"ExternalStatus","CopyNumber", "Pagination",	"Press",	"LastError",	"Comment",
                                         "DeadLine",		"ProofStatus", "Location", "SheetNumber", "SheetSide", "PagePositions", "PageType", "PagesPerPlate",
                                         "InputTime", "ApproveTime", "OutputTime", "VerifyTime", "Active", "Unique" };
                 //"MasterCopySeparationSet", "SeparationSet",
                 //"FlatSeparationSet", "FlatSeparation"};


                 string newExpr = e.SortExpression;

                 DataView dv = (DataView)DataGrid1.DataSource;
                 foreach (string col in colNames)
                 {
                     string ss = Global.rm.GetString("txt" + col);
                     if (ss != null && ss != "" && ss != " ")
                     {
                         if (newExpr == ss)
                         {
                             newExpr = col;
                             break;
                         }
                     }
                 }

                 if (newExpr == "PageName" || newExpr == "Page")
                     newExpr = "Pagination";

                 string oldExpr = (string)ViewState["DataSortExpression"];
                 if (oldExpr == newExpr)
                     ViewState["DataSortExpression"] = newExpr + " DESC";
                 else
                     ViewState["DataSortExpression"] = newExpr;

                 ReBind();

             }

   
             protected void DataGrid1_ItemCreated(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
             {
                 switch (e.Item.ItemType)
                 {
                     case ListItemType.Item:
                         //DataGrid1.Columns[(int)ViewState["EditionColumn"]].Visible = (bool)ViewState["HideEditionColumn"] ? false : true;
                         //DataGrid1.Columns[(int)ViewState["SectionColumn"]].Visible = (bool)ViewState["HideSectionColumn"] ? false : true;
                         break;
                 }

             }

        
             private void SetupHeader(System.Web.UI.WebControls.DataGridItemEventArgs e)
             {
                 string sortExpr = (string)ViewState["DataSortExpression"];
                 bool isDesc = sortExpr.EndsWith(" DESC");

                 string pureSortExpr = sortExpr.Replace(" DESC", "");

                 for (int i = 0; i < DataGrid1.Columns.Count; i++)
                 {
                     string colSortExpr = DataGrid1.Columns[i].SortExpression;
                     if (pureSortExpr == colSortExpr && colSortExpr != "")
                     {
                         TableCell cell = e.Item.Cells[i];
                         Label lblSorted = new Label();
                         lblSorted.Font.Name = "webdings";
                         lblSorted.Font.Size = FontUnit.XSmall;
                         lblSorted.Text = (isDesc ? " 6" : " 5");
                         cell.Controls.Add(lblSorted);
                     }
                 }
             }*/


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

        protected void RadGrid1_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            (sender as RadGrid).DataSource = GetDataTable();
        }

        protected void RadGrid1_ColumnCreated(object sender, GridColumnCreatedEventArgs e)
        {
            
            switch (e.Column.UniqueName)
            {
                case "Edition":
                    e.Column.HeaderText = Global.rm.GetString("txtEdition");
                    e.Column.Visible = (bool)ViewState["HideEditionColumn"] ? false : true;
                    break;
                case "Section":
                    e.Column.HeaderText = Global.rm.GetString("txtSection");
                    e.Column.Visible = (bool)ViewState["HideSectionColumn"] ? false : true;
                    break;
                case "PageName":
                    e.Column.HeaderText = Global.rm.GetString("txtPageName");
                    break;

                case "PubDate":
                    e.Column.HeaderText = Global.rm.GetString("txtPageName");
                    break;
            }

            
        }

        protected void RadGrid1_ItemDataBound(object sender, GridItemEventArgs e)
        {
           
            if (e.Item is GridDataItem)
            {
                GridDataItem dataBoundItem = e.Item as GridDataItem;

                for (int i = 0; i < dataBoundItem.Cells.Count; i++)
                    dataBoundItem.Cells[i].Wrap = false;
                 

                // Color background

                String sColorName = dataBoundItem["Color"].Text;

                if (sColorName.StartsWith(";"))
                    sColorName = sColorName.Substring(1);
                dataBoundItem["Color"].Text = sColorName;

                string[] sargs = sColorName.Split(';');

                if ((bool)Session["SelectedAllSeps"] == false)
                {
                    if (sargs.Length == 4 && sColorName.IndexOf('C') != -1 && sColorName.IndexOf('M') != -1 && sColorName.IndexOf('Y') != -1 && sColorName.IndexOf('K') != -1)
                    {
                        dataBoundItem["Color"].Text = "CMYK";
                        dataBoundItem["Color"].BackColor = Color.Magenta;
                        dataBoundItem["Color"].ForeColor = Color.Black;
                    }
                    else if (sargs.Length == 1 && sColorName.IndexOf('K') != -1)
                    {
                        dataBoundItem["Color"].BackColor = Color.Black;
                        dataBoundItem["Color"].ForeColor = Color.White;
                    }
                    else
                    {
                        dataBoundItem["Color"].BackColor = Color.Cyan;
                        dataBoundItem["Color"].ForeColor = Color.Black;
                    }
                }
                else
                {
                    dataBoundItem["Color"].BackColor = Globals.GetColorFromName((string)sargs[0]);
                    float f = dataBoundItem["Color"].BackColor.GetBrightness();
                    dataBoundItem["Color"].ForeColor = f >= 0.5F ? Color.Black : Color.White;

                }
             
                // Color text red if hidden color

                if (dataBoundItem["Active"].Text == "False" || dataBoundItem["Active"].Text == "0")
                {
                    for (int i = 0; i < dataBoundItem.Cells.Count; i++)
                        dataBoundItem.Cells[i].ForeColor = Color.Red;
                }

                // Approval color


                string approvalValue = dataBoundItem["Approval"].Text;
                if (approvalValue == "2")
                {
                    dataBoundItem["Approval"].Text = Global.rm.GetString("txtRejected");
                    dataBoundItem["Approval"].BackColor = Color.Red;
                    dataBoundItem["Approval"].ForeColor = Color.White;
                }
                else if (approvalValue == "0")
                {
                    dataBoundItem["Approval"].Text = Global.rm.GetString("txtNotApproved");
                    dataBoundItem["Approval"].BackColor = Color.LightGray;
                    dataBoundItem["Approval"].ForeColor = Color.Black;
                }
                else if (approvalValue == "1" || approvalValue == "-1")
                {
                    dataBoundItem["Approval"].Text = Global.rm.GetString("txtApproved");
                    dataBoundItem["Approval"].BackColor = Color.LightGreen;
                    dataBoundItem["Approval"].ForeColor = Color.Black;
                }
                else
                {
                    dataBoundItem["Approval"].Text = "N/A";
                    dataBoundItem["Approval"].BackColor = Color.LightGray;
                    dataBoundItem["Approval"].ForeColor = Color.Black;
                }

                // Status color

                string statusValue = dataBoundItem["Status"].Text;
                dataBoundItem["Status"].BackColor = Globals.GetStatusColorFromName(statusValue, 0);

                statusValue = dataBoundItem["ExternalStatus"].Text;
                dataBoundItem["ExternalStatus"].BackColor = Globals.GetStatusColorFromName(statusValue, 1);

                // Hold/release

                string holdValue = dataBoundItem["Hold"].Text;
                if (holdValue == "Released")
                {
                    dataBoundItem["Hold"].BackColor = Color.Green;
                    dataBoundItem["Hold"].ForeColor = Color.White;
                    dataBoundItem["Hold"].Text = Global.rm.GetString("txtReleased");
                }
                else
                {
                    dataBoundItem["Hold"].BackColor = Color.Red;
                    dataBoundItem["Hold"].ForeColor = Color.White;
                    dataBoundItem["Hold"].Text = Global.rm.GetString("txtOnHold");
                }

                // Front/back
                string fbValue = dataBoundItem["SheetSide"].Text;
                dataBoundItem["SheetSide"].Text = fbValue == "Front" ? Global.rm.GetString("txtFrontSide") : Global.rm.GetString("txtBackSide");

            }


        }


    }
}