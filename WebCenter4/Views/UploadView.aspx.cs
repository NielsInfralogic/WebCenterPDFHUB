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
using Telerik.Web.UI;
using WebCenter4.Classes;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.IO;

namespace WebCenter4.Views
{
    public partial class UploadView : System.Web.UI.Page
    {
        public int nWindowHeight;
        public int nWindowWidth;
        public int nRefreshTime;
        public int nScollPos = 0;

    
        private string uploadFolder;

        protected int customuploadNameFormat;
        protected void Page_Load(object sender, EventArgs e)
        {
            if ((string)Session["UserName"] == null)
                Response.Redirect("../SessionTimeout.htm");

            if ((string)Session["UserName"] == "")
                Response.Redirect("../Denied.htm");

            nRefreshTime = (int)Session["RefreshTime"];

            customuploadNameFormat = (int)Application["CustomUploadFileNames"];

            uploadFolder = Global.sVirtualUploadFolder;
            hiddenUploadPath.Value = uploadFolder;
            //RadAsyncUpload1.FileUploaded += new Telerik.Web.UI.FileUploadedEventHandler(RadAsyncUpload1_FileUploaded);

            SetLanguage();
            SetAccess();
            DisplayProductTitle();
            if (Page.IsPostBack == false)
            {
                CreatePublicationDropDown();
                SetEditionsFromPublication();
                SetSectionsFromPublication();

                DateTime tCurrentPubdate = (DateTime)Session["SelectedPubDate"];
                if (tCurrentPubdate != null && tCurrentPubdate.Year > 2000)
                    dateChooserPubDate.SelectedDate = tCurrentPubdate;
                else
                {
                    DateTime tDefault = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    tDefault.AddDays(1);
                    dateChooserPubDate.SelectedDate = tDefault;
                }

                ddPublicationList.Enabled = (bool)Application["UploadViewUseNameRule"];
                RadCheckBoxUseNameRule.Checked = (bool)Application["UploadViewUseNameRule"];
                dateChooserPubDate.Enabled = (bool)Application["UploadViewUseNameRule"];
                ddEditionList.Enabled = (bool)Application["UploadViewUseNameRule"];
                ddSectionList.Enabled = (bool)Application["UploadViewUseNameRule"];
            }

          //  HtmlGenericControl pageListDiv = (HtmlGenericControl)FindControl("PageList");
            //pageListDiv.Controls.Clear();

            ArrayList al = (ArrayList)Session["UploadPages"];
/*            int i = 1;
            string AllZones = "."; 
            foreach(string s in al)
            {
                HtmlGenericControl myDiv = new HtmlGenericControl("div");
                myDiv.InnerText="Page "+s;
                myDiv.ID = "DropZone" + i.ToString();
                pageListDiv.Controls.Add(myDiv);

                if (i > 1)
                    AllZones += ",#";
                AllZones += myDiv.ID;                
                i++;
            }*/
        }

        

        private string DisplayProductTitle()
        {
            DateTime pubDate =  (DateTime)Session["SelectedPubDate"];
            int publicationID = Globals.GetIDFromName("PublicationNameCache",(string)Session["SelectedPublication"]);
            int editionID = Globals.GetIDFromName("EditionNameCache", (string)Session["SelectedEdition"]);
            int sectionID = Globals.GetIDFromName("SecionNameCache", (string)Session["SelectedSection"]);

            string title = string.Format("{0:4}-{1:2}-{2:2} {3}", pubDate.Year, pubDate.Month, pubDate.Day, (string)Session["SelectedPublication"]);
            if (editionID > 0)
                title += " " + (string)Session["SelectedEdition"];
            if (sectionID > 0)
                title += " " + (string)Session["SelectedSection"];

            CCDBaccess db = new CCDBaccess();
            string errmsg = "";
            ArrayList al = new ArrayList();

            al = db.GetPagesInSection(publicationID, pubDate, editionID, sectionID, out errmsg);
            Session["UploadPages"] = al;


            return title;
        }


        private DataTable GetData()
        {
            System.Data.DataTable table = new DataTable();
            DataColumn column;
            DataRow row;

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "ID";
            column.ReadOnly = true;
            table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "Page";
            column.AutoIncrement = false;
            column.Caption = "Page";
            column.ReadOnly = false;
            column.Unique = false;
            column.AllowDBNull = true;
            table.Columns.Add(column);

            DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            PrimaryKeyColumns[0] = table.Columns["ID"];
            table.PrimaryKey = PrimaryKeyColumns;

            //Generating Data
            ArrayList al = (ArrayList)Session["UploadPages"];
            int i = 1;
            foreach (string s in al)
            {
                row = table.NewRow();
                row["ID"] = i.ToString();
                row["Page"] = s;
                table.Rows.Add(row);
                i++;
            }
            return table;
        }


        void RadAsyncUpload1_FileUploaded(object sender, FileUploadedEventArgs e)
        {
            e.IsValid = true;
            return;
/*            string temppath = RadAsyncUpload1.TemporaryFolder;
            e.IsValid = true; // !CheckUploadedFileValidity();

            Label fileName = new Label();

            fileName.Text = e.File.FileName;

            string physicalSourceFolder = HttpContext.Current.Server.MapPath(RadAsyncUpload1.TemporaryFolder);

            string sourceFile = physicalSourceFolder + @"\" + e.File.FileName;

            string physicalFolder = HttpContext.Current.Server.MapPath(hiddenUploadPath.Value);

            string destFile = physicalFolder + @"\" + e.File.FileName;

            e.IsValid = true;*/
            /*    if (System.IO.File.Exists(sourceFile) == false)
                    e.IsValid = false;
                else
                {
                    try
                    {
                        System.IO.File.Move(sourceFile, destFile);
                    }
                    catch (Exception ex)
                    {
                        string s = ex.Message;
                        e.IsValid = false;
                    }
                }
            

                if (e.IsValid)
                {
                    ValidFiles.Visible = true;
                    ValidFiles.Controls.Add(fileName);
                }
                else
                {
                    InvalidFiles.Visible = true;
                    InvalidFiles.Controls.Add(fileName);

                }
                */
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
        }

        protected void SetLanguage()
        {
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            lblPublication.Text = Global.rm.GetString("txtPublication");
            lblEdition.Text = Global.rm.GetString("txtEdition");
            lblSection.Text = Global.rm.GetString("txtSection");
            lblPubdate.Text = Global.rm.GetString("txtPubDate2");
        }

        private void CreatePublicationDropDown()
        {
            DataTable table = (DataTable)Cache["PublicationNameCache"];

            ddPublicationList.Items.Clear();

            string pubsallowed = (string)Session["PublicationsAllowed"];
            string[] publist = pubsallowed.Split(',');

            string currentPublication = (string)Session["SelectedPublication"];
            foreach (DataRow row in table.Rows)
            {
                string thisPublication = (string)row["Name"];
                string pubAlias = Globals.LookupInputAlias("Publication", thisPublication);
                if (pubsallowed == "*")
                    ddPublicationList.Items.Add(new Telerik.Web.UI.RadComboBoxItem(thisPublication + " [" + pubAlias + "]", thisPublication ));
                else
                {
                    foreach (string sp in publist)
                    {
                        if (sp == thisPublication)
                        {
                            ddPublicationList.Items.Add(new Telerik.Web.UI.RadComboBoxItem(thisPublication + " [" + pubAlias + "]", thisPublication));
                            break;
                        }
                    }
                }
            }

            if (currentPublication != "" && currentPublication != "*")
                ddPublicationList.SelectedValue = currentPublication;

        }

        private void SetEditionsFromPublication()
        {
            int publicationID = Globals.GetIDFromName("PublicationNameCache", ddPublicationList.SelectedValue);
            DataTable table = (DataTable)Cache["EditionNameCache"];
            ddEditionList.Items.Clear();

            if (Globals.HasAllowedEditions(publicationID) == false)
            {
                foreach (DataRow row in table.Rows)
                    ddEditionList.Items.Add(new Telerik.Web.UI.RadComboBoxItem((string)row["Name"], (string)row["Name"]));
                return;
            }

            foreach (DataRow row in table.Rows)
            {
                if (Globals.IsAllowedEdition(publicationID, (int)row["ID"]))
                    ddEditionList.Items.Add(new Telerik.Web.UI.RadComboBoxItem((string)row["Name"], (string)row["Name"]));
            }
        }

        private void SetSectionsFromPublication()
        {
            int publicationID = Globals.GetIDFromName("PublicationNameCache", ddPublicationList.SelectedValue);
            DataTable table = (DataTable)Cache["SectionNameCache"];
            ddSectionList.Items.Clear();

            if (Globals.HasAllowedSections(publicationID) == false)
            {
                foreach (DataRow row in table.Rows)
                    ddSectionList.Items.Add(new Telerik.Web.UI.RadComboBoxItem((string)row["Name"], (string)row["Name"]));
                return;
            }

            foreach (DataRow row in table.Rows)
            {
                if (Globals.IsAllowedSection(publicationID, (int)row["ID"]))
                    ddSectionList.Items.Add(new Telerik.Web.UI.RadComboBoxItem((string)row["Name"], (string)row["Name"]));
            }
        }



        protected void SetAccess()
        {

        }

        private void GetFilteredGrid()
        {
/*            //DataSet dataSet  =  (DataSet) Cache["PageTableCache"];

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
 */
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
                //string errmsg = "";
                /*
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
                GetFilteredGrid();*/
            }
            else if (e.Item.Value == "Delete")
            {

            }
            else if (e.Item.Value == "Rename")
            {
           }
		
        }



        protected void RadButton_Click(object sender, EventArgs e)
        {
            if (RadAsyncUpload1.UploadedFiles.Count > 0)
            {
                DataTable dt = new DataTable();

                DataRow row;
                dt.Columns.Add(new DataColumn("FileName", System.Type.GetType("System.String")));
                dt.Columns.Add(new DataColumn("Size", System.Type.GetType("System.Int32")));
                dt.Columns.Add(new DataColumn("Status", System.Type.GetType("System.String")));

                GridView1.Visible = true;
                ltrNoResults.Visible = false;

                string physicalDestFolder = HttpContext.Current.Server.MapPath(hiddenUploadPath.Value);
                foreach (UploadedFile f in RadAsyncUpload1.UploadedFiles)
                {
                    string fileName = f.GetName();
                    string title = fileName;

                    if (f.GetFieldValue("TextBox") != null)
                    {
                        title = f.GetFieldValue("TextBox");
                        if (title == "")
                            title = fileName;
                    }

                    if (title.IndexOf(',') >= 0)
                        title = title.Substring(title.IndexOf(',') + 1);
                    string physicalSourceFolder = HttpContext.Current.Server.MapPath(RadAsyncUpload1.TargetFolder);

                    string sourceFile = physicalSourceFolder + @"\" + fileName;
                    string destFile = physicalDestFolder + @"\" + title;

                    if (cbFullDocument.Checked == true)
                    {
                        if (title.IndexOf("-0.pdf") > 0  || title.IndexOf("_0.pdf") > 0)
                        {
                            title.Replace("-0.pdf", ".pdf");
                            title.Replace("_0.pdf", ".pdf");
                        }
                    }
                    row = dt.NewRow();
                    row["FileName"] = title;
                    row["Size"] = f.ContentLength;
                    try
                    {
                        File.Copy(sourceFile, destFile, true);
                        row["Status"] = "OK";
                    }
                    catch (Exception ex)
                    {
                        row["Status"] = "Error - " + ex.Message;
                    }

                    dt.Rows.Add(row);

                }
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
            else
            {
                ltrNoResults.Visible = true;
                GridView1.Visible = false;
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

        protected void ddPublicationList_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            SetEditionsFromPublication();
            SetSectionsFromPublication();         
        }

        protected void RadCheckBoxUseNameRule_Click(object sender, EventArgs e)
        {
            bool? checkd = RadCheckBoxUseNameRule.Checked;
            if (!checkd.HasValue)
                checkd = false;
            dateChooserPubDate.Enabled = (bool)checkd;
            ddPublicationList.Enabled = (bool)checkd;
            ddEditionList.Enabled = (bool)checkd;
            ddSectionList.Enabled = (bool)checkd;
        }
    }
}