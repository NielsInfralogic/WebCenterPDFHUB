using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using WebCenter4.Classes;

namespace WebCenter4.Views
{
    public partial class InsertPages : System.Web.UI.Page
    {
        private int publicationID;
        private DateTime pubDate;
        private int editionID;
        private int sectionID;
        private int pressID;

        protected void Page_Load(object sender, EventArgs e)
        {
           

            /*if (publicationID == 0 || pubDate.Year < 2000)
            {
                InjectScript.Text = "<script>CloseOnReload()</" + "script>";
                return;
            } */



            if (!Page.IsPostBack)
            {
                publicationID = Session["SelectedPublication"] != null ? Globals.GetIDFromName("PublicationNameCache", (string)Session["SelectedPublication"]) : 0;
                pubDate = Session["SelectedPubDate"] != null ? (DateTime)Session["SelectedPubDate"] : DateTime.MinValue;
                editionID = Globals.GetIDFromName("EditionNameCache", (string)Session["SelectedEdition"]);
                sectionID = Globals.GetIDFromName("SectionNameCache", (string)Session["SelectedSection"]);
                pressID = Globals.GetIDFromName("PressNameCache", (string)Session["SelectedPress"]);

                if (pubDate.Year > 2000)
                {
                    RadDatePickerPubdateFrom.SelectedDate = pubDate;
                    RadDatePickerPubdateTo.SelectedDate = pubDate;
                }
                else
                {
                    DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    dt = dt.AddDays(1);
                    RadDatePickerPubdateFrom.SelectedDate = dt;
                    RadDatePickerPubdateTo.SelectedDate = dt;
                }
                FillPublicationCombo(Globals.GetNameFromID("PublicationNameCache", publicationID));

                if (publicationID == 0)
                {
                    RadRadioButtonList1.SelectedIndex = 3; // Special position if no existing production
                }
            }

            SetEditionsFromPublication();

            if (!Page.IsPostBack)
            {
                if (editionID > 0)
                    RadComboBoxEdition.SelectedValue = Globals.GetNameFromID("EditionNameCache", editionID);
            }

            SetSectionsFromPublication();

            if (!Page.IsPostBack)
            {
                if (sectionID > 0)
                    RadComboBoxSection.SelectedValue = Globals.GetNameFromID("SectionNameCache", sectionID);
            }    

            RadComboBoxSection.Enabled = RadComboBoxSection.Items.Count > 1;
            RadComboBoxEdition.Enabled = RadComboBoxEdition.Items.Count > 1;

            if (!Page.IsPostBack)
            {
                PopulateChannelCheckBoxList();
            }

            SetLanguage();
            if (!Page.IsPostBack)
            {
                Session["UploadDataTable"] = null;
            }

            //SetProductLabel();

            //RadTextBoxAfterPage.Enabled = true;  
            //lblAfterPage.Enabled = true;
        }


        private void SetEditionsFromPublication()
        {
            int publicationID = Globals.GetIDFromName("PublicationNameCache", RadComboBoxProduct.SelectedValue);

            string mainEdition = GetFirstMainEditionInProduct(publicationID, (DateTime)RadDatePickerPubdateFrom.SelectedDate);

            DataTable table = (DataTable)Cache["EditionNameCache"];
            RadComboBoxEdition.Items.Clear();

            if (Globals.HasAllowedEditions(publicationID) == false)
            {
                foreach (DataRow row in table.Rows)
                    RadComboBoxEdition.Items.Add(new Telerik.Web.UI.RadComboBoxItem((string)row["Name"], (string)row["Name"]));

                RadComboBoxEdition.SelectedIndex = 0;

                if (mainEdition != "")
                    RadComboBoxEdition.SelectedIndex = RadComboBoxEdition.Items.FindItemIndexByText(mainEdition);
                return;
            }

            foreach (DataRow row in table.Rows)
            {
                if (Globals.IsAllowedEdition(publicationID, (int)row["ID"]))
                    RadComboBoxEdition.Items.Add(new Telerik.Web.UI.RadComboBoxItem((string)row["Name"], (string)row["Name"]));
            }

            RadComboBoxEdition.SelectedIndex = 0;

            if (mainEdition != "")
                RadComboBoxEdition.SelectedIndex = RadComboBoxEdition.Items.FindItemIndexByText(mainEdition);
        }

        private void SetSectionsFromPublication()
        {
            int publicationID = Globals.GetIDFromName("PublicationNameCache", RadComboBoxProduct.SelectedValue);
            DataTable table = (DataTable)Cache["SectionNameCache"];
            RadComboBoxSection.Items.Clear();

            if (Globals.HasAllowedSections(publicationID) == false)
            {
                foreach (DataRow row in table.Rows)
                    RadComboBoxSection.Items.Add(new Telerik.Web.UI.RadComboBoxItem((string)row["Name"], (string)row["Name"]));
                return;
            }

            foreach (DataRow row in table.Rows)
            {
                if (Globals.IsAllowedSection(publicationID, (int)row["ID"]))
                    RadComboBoxSection.Items.Add(new Telerik.Web.UI.RadComboBoxItem((string)row["Name"], (string)row["Name"]));
            }

            RadComboBoxSection.SelectedIndex = 0;


        }

    private void FillPublicationCombo(string defaultPublication)
        {
            DataTable table = (DataTable)Cache["PublicationNameCache"];

            RadComboBoxProduct.Items.Clear();

            string pubsallowed = (string)Session["PublicationsAllowed"];
            string[] publist = pubsallowed.Split(',');

            foreach (DataRow row in table.Rows)
            {
                string thisPublication = (string)row["Name"];

                if (pubsallowed == "*")
                    RadComboBoxProduct.Items.Add(new Telerik.Web.UI.RadComboBoxItem(thisPublication, thisPublication));
                else
                {
                    foreach (string sp in publist)
                    {
                        if (sp == thisPublication)
                        {
                            RadComboBoxProduct.Items.Add(new Telerik.Web.UI.RadComboBoxItem(thisPublication, thisPublication));
                            break;
                        }
                    }
                }
            }

            if (defaultPublication != "")
                RadComboBoxProduct.SelectedValue = defaultPublication;
            else
                RadComboBoxProduct.SelectedIndex = 0;
        }

        private void SetProductLabel()
        {
            publicationID = Globals.GetIDFromName("PublicationNameCache", RadComboBoxProduct.SelectedValue);
            pubDate = GetSelectedPubdateFrom();
           
            string product = string.Format("{0} {1:00}-{2:00}", RadComboBoxProduct.SelectedValue, pubDate.Day, pubDate.Month);
             product += " " + RadComboBoxEdition.SelectedValue;
             product += " " + RadComboBoxSection.SelectedValue;
            //lblProduct.Text = product;
            lblProduct2.Text = product;
        }

        private void SetLanguage()
        {
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            SetRadToolbarLabel("Item1", "lblInsertPagesHeader", Global.rm.GetString("txtInsertPagesInProduct"));
   
            lblProductLabel.Text = Global.rm.GetString("txtProduct");
            //  lblProductLabel2.Text = Global.rm.GetString("txtProduct");

            lblPubDateFrom.Text = Global.rm.GetString("txtPubDate");
            lblPubDateTo.Text = "til"; //Global.rm.GetString("txtPubdate");
            lblChooseEdition.Text = Global.rm.GetString("txtEdition");
            lblChooseChannels.Text = Global.rm.GetString("txtChannels");
            lblChooseSection.Text = Global.rm.GetString("txtSection");
            lblPagesToInsert.Text = Global.rm.GetString("txtPagesToInsert");
            lblPositionOfPage.Text = Global.rm.GetString("txtPositionOfPage");
            lblLetter.Text = Global.rm.GetString("txtLetterToUse");

            RadRadioButtonList1.Items[0].Text = Global.rm.GetString("txtFirstInDocument");
            RadRadioButtonList1.Items[1].Text = Global.rm.GetString("txtLastInDocument");
            RadRadioButtonList1.Items[2].Text = Global.rm.GetString("txtMiddleOfDocument");
            RadRadioButtonList1.Items[3].Text = Global.rm.GetString("txtSpecialPosition");

            gridview1.Columns[0].HeaderText = Global.rm.GetString("txtPageNumber");
            gridview1.Columns[1].HeaderText = Global.rm.GetString("txtFinalName");
            lblAfterPage.Text = Global.rm.GetString("txtAfterPage");
            RadWizardStep1.Title = Global.rm.GetString("txtDefinePagesToInsert");
            RadWizardStep2.Title = Global.rm.GetString("txtUploadPagesToInsert");
            RadWizard1.Localization.Cancel = Global.rm.GetString("txtCancel");
            RadWizard1.Localization.Next = Global.rm.GetString("txtNext");
            RadWizard1.Localization.Finish = Global.rm.GetString("txtFinish");
            RadWizard1.Localization.Previous = Global.rm.GetString("txtPrevious");
                                        


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

        private int GetFirstEditionInProduct(int publicationID, DateTime pubDate)
        {
            string errmsg = "";
            CCDBaccess db = new CCDBaccess();
            ArrayList al = db.GetEditionsInProduction(publicationID, pubDate, out errmsg);
            if (al.Count > 0)
                return (int)al[0];
            return 0;
        }

        private List<int> GetSectionsInProduct(int publicationID, DateTime pubDate, int editionID)
        {
            string errmsg = "";
            CCDBaccess db = new CCDBaccess();
            return db.GetSectionsInEdition(publicationID, pubDate, editionID, out errmsg);
        }

        private int GetPageCountInSection()
        {
            string errmsg = "";
            CCDBaccess db = new CCDBaccess();

            int publicationID = Globals.GetIDFromName("PublicationNameCache", RadComboBoxProduct.Text);
            DateTime pubDate = GetSelectedPubdateFrom();
            int editionID = Globals.GetIDFromName("EditionNameCache", RadComboBoxEdition.Text);
            int sectionID = Globals.GetIDFromName("SectionNameCache", RadComboBoxSection.Text);
            ArrayList pageNameList = db.GetPagesInSection(publicationID, pubDate, editionID, sectionID, out errmsg);
            return pageNameList.Count;
        }

        private string GetFirstMainEditionInProduct(int publicationID, DateTime pubDate)
        {
            string errmsg = "";
            CCDBaccess db = new CCDBaccess();

            int editionID = db.GetMainEditionInProduct(publicationID, pubDate, out errmsg);
            return Globals.GetNameFromID("EditionNameCache", editionID);
        }
        private void PopulateChannelCheckBoxList()
        {
            string errmsg = "";
            CCDBaccess db = new CCDBaccess();
            ArrayList channelsForPublication = new ArrayList();
            int productionID = 0;
            int pressRunID = 0;
            int publicationID = Globals.GetIDFromName("PublicationNameCache", RadComboBoxProduct.SelectedValue);

            db.GetPressRunID(publicationID, pubDate, ref editionID, sectionID, pressID, out pressRunID, out productionID, out errmsg);

           /* if (pressRunID == 0 || productionID == 0)
            {
                InjectScript.Text = "<script>CloseOnReload()</" + "script>";
                return;
            }*/

            txtProductionID.Text = productionID.ToString();
            txtPressRunID.Text = pressRunID.ToString();
         

            DataTable dtchannelsdb = db.GetChannelCollection(out errmsg);
            
            db.GetPublicationChannelsForPublication(publicationID, ref channelsForPublication, out errmsg);

           // Session["ThisChannelList"] = channelsForPublication;

            CheckBoxListChannels.Items.Clear();

            foreach (DataRow row in dtchannelsdb.Rows)
            {  
                string channel = (string)row["Name"];
                int channelID = (int)row["ID"];
                bool use = false;
                //foreach (int pubchannelID in channelsForPublication)
                //{
                //    if (pubchannelID == channelID)
                //    {
                //        use = true;
                //        break;
                //    }
                //}
                CheckBoxListChannels.Items.Add(new ButtonListItem((string)row["Name"], channelID.ToString(), true, use));
            }
        }

        // returns string of comma-separated ChannelIDs
        private string GetSelectedChannels()
        {
            string channelIDList = "";

            
       //     int[] selectedindices = CheckBoxListChannels.Items;

            foreach (ButtonListItem item in CheckBoxListChannels.Items)
            {
               // ButtonListItem item = CheckBoxListChannels.Items[i];
                if (item.Selected)
                { 
                    if (channelIDList != "")
                        channelIDList += ",";
                    channelIDList += item.Value.ToString();
                }     
            }
            return channelIDList;
        }

        private int FindPageOffset()
        {
            if (RadRadioButtonList1.SelectedIndex == 0)
                return 0;
            else
            {
                int pagesInSection = GetPageCountInSection();
                if (pagesInSection == 0)
                    return Globals.TryParse(RadTextBoxAfterPage.Text, 0); 

                if (RadRadioButtonList1.SelectedIndex == 1)
                    return pagesInSection;
                if (RadRadioButtonList1.SelectedIndex == 2)
                    return pagesInSection / 2;
                else
                    return Globals.TryParse(RadTextBoxAfterPage.Text,1);
            }

            //return 0;
        }

        private DataTable GenerateDataTable()
        {
            DataTable uploadTable = new DataTable();
            DataColumn newColumn;
            DataRow newRow = null;
            newColumn = uploadTable.Columns.Add("PageNumber", Type.GetType("System.String"));
            newColumn = uploadTable.Columns.Add("FinalName", Type.GetType("System.String"));

            int numberOfPages = 0;
            Int32.TryParse(txtNumberOfPage.Text, out numberOfPages);
           // char c;
            for (int i = 1; i <= numberOfPages ; i++)
            {
                newRow = uploadTable.NewRow();
              //  c = (char)(i + 64);
                //  newRow[0] = string.Format("{0}{1}", FindPageOffset(), c);

                newRow[0] = string.Format("{0}{1}", FindPageOffset()+i-1, RadDropDownListLetters.SelectedText);

                newRow[1] = "(missing)";
                uploadTable.Rows.Add(newRow);
            }

            Session["UploadDataTable"] = uploadTable;

            return uploadTable;
        }

        private bool HasUploadedAllPages()
        {
            DataTable tbl = (DataTable)Session["UploadDataTable"];
            bool allFilsOK = true;
            foreach(DataRow row in tbl.Rows)
            {
                if ((string)row[1] == "" || (string)row[1] == "(missing)")
                {
                    allFilsOK = false;
                    break;
                }
            }

            return allFilsOK;
        }

        protected void RadWizard1_NextButtonClick(object sender, WizardEventArgs e)
        {
//            string channelIDList = GetSelectedChannels();
            DataView myDataView = GenerateDataTable().DefaultView;
            gridview1.DataSource = myDataView;
            gridview1.DataBind();
            Control ctrl = RadWizard1.FindControl("rwzFinish");
            if (ctrl != null)
                ctrl.Visible = HasUploadedAllPages();

            SetRadToolbarLabel("Item1", "lblInsertPagesHeader", Global.rm.GetString("txtUploadPagesToInsert"));

            SetProductLabel();


            if (GetSelectedChannels() == "")
            {
                UploadStatusLabel.Text = "No channels selected.!";
                UploadStatusLabel.ForeColor = System.Drawing.Color.Orange;
                return;
            }

            if (txtNumberOfPage.Text == "")
            {
                UploadStatusLabel.Text = "Number of pages not defined.!";
                UploadStatusLabel.ForeColor = System.Drawing.Color.Orange;
                return;

            }

            if (RadTextBoxAfterPage.Text == "" && RadRadioButtonList1.SelectedIndex == 3)
            {
                UploadStatusLabel.Text = "No page offset defined.!";
                UploadStatusLabel.ForeColor = System.Drawing.Color.Orange;
                return;
            }

            UploadStatusLabel.Text = "";



        }

        private string GetSavePath()
        {
            string savePath = Global.sRealUploadFolder;
            if (savePath.EndsWith(";") || savePath.EndsWith("/"))
                savePath = savePath.Substring(0, savePath.Length - 1);

            return savePath;
        }

        protected void Upload(object sender, EventArgs e)
        {
            List<string> fileNameList = new List<string>();
            bool success = false;
            Button btn = (Button)sender;
            GridViewRow gvr = (GridViewRow)btn.NamingContainer;
            if (gvr == null)
                return;
            FileUpload fu = gvr.Cells[2].FindControl("FileUpload1") as FileUpload;
            if (fu == null)
                return;
            if (fu.HasFile == false)
            {
                UploadStatusLabel.Text = "No file selected..";
                UploadStatusLabel.ForeColor = System.Drawing.Color.Orange;

                return;
            }
            string pageName = gvr.Cells[0].Text;

            DateTime fromDate = GetSelectedPubdateFrom();
            DateTime toDate = GetSelectedPubdateTo();

            string fileName = generateFileName(pageName, GetSelectedPubdateFrom());
            fileNameList.Add(fileName);
            DateTime dt = fromDate;
            while (dt != toDate)
            {
                dt = dt.AddDays(1);
                fileNameList.Add(generateFileName(pageName, dt));
            }

            // Specify the path to save the uploaded file to.
            string savePath = GetSavePath() + @"\park";

            // Create the path and file name to check for duplicates.
            string pathToCheck = savePath + @"\" + fileName;


            // Create a temporary file name to use for checking duplicates.
            // string tempfileName = "";

            // Check to see if a file already exists with the
            // same name as the file to upload.        
            /* if (System.IO.File.Exists(pathToCheck))
             {
                 int counter = 2;
                 while (System.IO.File.Exists(pathToCheck))
                 {
                     // if a file with this name already exists,
                     // postfix the filename with a number.
                     tempfileName = fileName + counter.ToString();
                     pathToCheck = savePath + @"\" + tempfileName;
                     counter++;
                 }

                 fileName = tempfileName;

                 // Notify the user that the file name was changed.
                 UploadStatusLabel.Text = "A file with the same name already exists - name will be " + fileName;
             }  */

            // Call the SaveAs method to save the uploaded
            // file to the specified directory.
            try
            {
                fu.SaveAs(savePath + @"\" + fileName);
                success = true;
                UploadStatusLabel.Text = "Your file was uploaded successfully.";
                UploadStatusLabel.ForeColor = System.Drawing.Color.Green;
            }
            catch (HttpException ex)
            {
                UploadStatusLabel.Text = "Error uploading file " + fileName + ". - " + ex.Message;
                UploadStatusLabel.ForeColor = System.Drawing.Color.Orange;
            }

            if (success)
            {
                string fileNamestring = fileName;
                if (fileNameList.Count > 1)
                    for (int i=1; i< fileNameList.Count; i++)
                    {
                        try
                        {
                            File.Copy(savePath + @"\" + fileName, savePath + @"\" + fileNameList[i], true);
                            fileNamestring += "\r\n" + fileNameList[i];
                        }
                        catch
                        { }
                    }


                DataTable tbl = (DataTable)Session["UploadDataTable"];
                if (tbl == null)
                    return;
                foreach (DataRow row in tbl.Rows)
                {
                    if ((string)row[0] == pageName)
                    {
                        row[1] = fileNamestring;
                        Session["UploadDataTable"] = tbl;
                        break;
                    }
                }
            }
            DataTable t = (DataTable)Session["UploadDataTable"];
            DataView myDataView = t.DefaultView;
            gridview1.DataSource = myDataView;
            gridview1.DataBind();
        }


        private DateTime GetSelectedPubdateFrom()
        {
            DateTime selectedDateTime = (DateTime)RadDatePickerPubdateFrom.SelectedDate;
            return new DateTime(selectedDateTime.Year, selectedDateTime.Month, selectedDateTime.Day, 0, 0, 0, 0);
        }

        private DateTime GetSelectedPubdateTo()
        {
            DateTime selectedDateTime = (DateTime)RadDatePickerPubdateTo.SelectedDate;
            return new DateTime(selectedDateTime.Year, selectedDateTime.Month, selectedDateTime.Day, 0, 0, 0, 0);
        }

        private string generateFileName(string pageName, DateTime pubDate)
        {
           // DateTime pubDate = GetSelectedPubdateFrom();

            string pubDateString = (string)Application["AllowInsertPagesFileNameDatePattern"];
            pubDateString = pubDateString.Replace("YYYY", string.Format("{0:0000}", pubDate.Year));
            pubDateString = pubDateString.Replace("YY", string.Format("{0:00}", pubDate.Year - 2000));
            pubDateString = pubDateString.Replace("MM", string.Format("{0:00}", pubDate.Month));
            pubDateString = pubDateString.Replace("DD", string.Format("{0:00}", pubDate.Day));

            string fileName = (string)Application["AllowInsertPagesFileNamePattern"];
            fileName = fileName.Replace("%P", Globals.LookupInputAlias("Publication", RadComboBoxProduct.Text));
            fileName = fileName.Replace("%D", pubDateString);
            fileName = fileName.Replace("%E", Globals.LookupInputAlias("Edition", RadComboBoxEdition.Text));
            fileName = fileName.Replace("%S", Globals.LookupInputAlias("Section", RadComboBoxSection.Text));
            fileName = fileName.Replace("%N", pageName);

            fileName = fileName.Replace("%X", "#"+GetSelectedChannels()+"#");


            return fileName;
        }

      

        protected void RadWizard1_FinishButtonClick(object sender, WizardEventArgs e)
        {
            if (HasUploadedAllPages() == false)
            {
                UploadStatusLabel.Text = "Not all pages are uploaded yet";
                UploadStatusLabel.ForeColor = System.Drawing.Color.Orange;
                return;
            }

            string errmsg;
            CCDBaccess db = new CCDBaccess();
            int productionID = 0;
            int pressRunID = 0;
            int editionID = 0;
            int numberOfPages = 0;
       

            Int32.TryParse(txtProductionID.Text, out productionID);
            Int32.TryParse(txtPressRunID.Text, out pressRunID);
            

         //   if (productionID == 0 || pressRunID == 0 || editionID == 0)
         //       return;
            
            Int32.TryParse(txtNumberOfPage.Text, out numberOfPages);

          
            int pressID = Globals.GetIDFromName("PressNameCache", (string)Session["SelectedPress"]);
            DateTime pubDate = GetSelectedPubdateFrom();
            DateTime pubDateTo = GetSelectedPubdateTo();
            int publicationID =  Globals.GetIDFromName("PublicationNameCache", RadComboBoxProduct.Text);
            int edtionID = Globals.GetIDFromName("EditionNameCache", RadComboBoxEdition.Text);
            int sectionID = Globals.GetIDFromName("SectionNameCache", RadComboBoxSection.Text);

        //    ArrayList pageNameList = db.GetPagesInSection(publicationID, pubDate, editionID, sectionID, out errmsg);

            db.spInsertPagesInSection(pressID, pubDate, publicationID, editionID, sectionID, numberOfPages, FindPageOffset(), GetSelectedChannels(), out errmsg);
            DateTime dt = pubDate;
            while (dt != pubDateTo)
            {
                dt = dt.AddDays(1);
                db.spInsertPagesInSection(pressID, dt, publicationID, editionID, sectionID, numberOfPages, FindPageOffset(), GetSelectedChannels(), out errmsg);
            }


            // Copy in newly uploaded pages

            Globals.MoveAllFiles(GetSavePath() + @"\park", GetSavePath(), out errmsg);

            InjectScript.Text = "<script>RefreshParentPage()</" + "script>";

        }

  /*      protected void RadWizard1_FinishButtonClickOLD(object sender, WizardEventArgs e)
        {
            if (HasUploadedAllPages() == false)
            {
                UploadStatusLabel.Text = "Not all pages are uploaded yet";
                UploadStatusLabel.ForeColor = System.Drawing.Color.Orange;
                return;
            }

            string errmsg;
            CCDBaccess db = new CCDBaccess();
            int productionID = 0;
            int pressRunID = 0;
            int editionID = 0;
            int numberOfPages = 0;
            int pageOffset = 0;

            Int32.TryParse(txtProductionID.Text, out productionID);
            Int32.TryParse(txtPressRunID.Text, out pressRunID);
            Int32.TryParse(txtEditionID.Text, out editionID);

            if (productionID == 0 || pressRunID == 0 || editionID == 0)
                return;

            Int32.TryParse(txtNumberOfPage.Text, out numberOfPages);

            string edition = Globals.GetNameFromID("EditionNameCache", editionID);
            if (edition.Length >= 3)
                edition = "Z_" + edition.Substring(2, 1);

            int newEditionID = Globals.GetIDFromName("EditionNameCache", edition);
            int pressID = Globals.GetIDFromName("PressNameCache", (string)Session["SelectedPress"]);
            DateTime pubDate = Session["SelectedPubDate"] != null ? (DateTime)Session["SelectedPubDate"] : DateTime.MinValue;
            int publicationID = Session["SelectedPublication"] != null ? Globals.GetIDFromName("PublicationNameCache", (string)Session["SelectedPublication"]) : 0;
            int sectionID = Globals.GetIDFromName("SectionNameCache", RadComboBoxSection.Text);

            // 00 Delete existing Z_x edition

            db.DeleteEdition(pressID, pubDate, publicationID, newEditionID, out errmsg);

            // 01 Create new edition

            int newPressRunID = db.InsertNewSubEdition(pressID, pubDate, publicationID, editionID, sectionID, newEditionID, out errmsg);

            if (newPressRunID <= 0)
                return;

            // 02 Insert empty page slots in new edition

            ArrayList pageNameList = db.GetPagesInSection(publicationID, pubDate, editionID, sectionID, out errmsg);
            string miscString3 = GetSelectedChannels();

            db.spInsertPagesInSection(pressID, pubDate, publicationID, newEditionID, sectionID, numberOfPages, FindPageOffset(), miscString3, out errmsg);
            if (newPressRunID <= 0)
                return;

            // 03 Adjust channels for new edition

           
           if (miscString3 != "")
            {
                db.UpdateMiscString3(productionID, editionID, miscString3, out errmsg);
                db.UpdateProductionOrderNumber(productionID, miscString3, out errmsg);
            }

            // Copy in newly uploaded pages

            Globals.MoveAllFiles(GetSavePath() + @"\park", GetSavePath(), out errmsg);

            InjectScript.Text = "<script>RefreshParentPage()</" + "script>";

        }         */

        protected void RadWizard1_CancelButtonClick(object sender, WizardEventArgs e)
        {
            InjectScript.Text = "<script>CloseOnReload()</" + "script>";
        }

        protected void RadWizard1_PreviousButtonClick(object sender, WizardEventArgs e)
        {
            SetRadToolbarLabel("Item1", "lblInsertPagesHeader", Global.rm.GetString("txtInsertPagesInProduct"));
        }
    }
}
 