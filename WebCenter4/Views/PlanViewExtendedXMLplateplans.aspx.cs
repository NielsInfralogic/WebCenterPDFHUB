using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;                                                                                    
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using WebCenter4.Classes;
using System.Text;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Configuration;
using System.Collections.Generic;

namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for PlanView.
	/// </summary>
	public partial class PlanViewExtendedXMLplateplans : System.Web.UI.Page
	{

        // Variables for javascript communication

        protected int updateTree;
        protected int existingProductionPrompt;
        protected int existingProductionPrompt2;
        protected bool hasOnlyOnePerssAndEdition;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ((string)Session["UserName"] == null)
				Response.Redirect("~/SessionTimeout.htm");

			if ((string)Session["UserName"] == "")
				Response.Redirect("/Denied.htm");

			SetLanguage();
			
            updateTree = 0;

			Session["RefreshTree"] = false;

			if (!this.IsPostBack) 
			{
                dateChooserPubDate.Culture = CultureInfo.CurrentCulture;
				Globals.ForceCacheReloadsSmall();

				Session["RefreshTree"] = false;
                Session["SubEditions"] = null;                             

                bool hideEdition = Globals.GetCacheRowCount("EditionNameCache") < 2;
                bool hideSection = Globals.GetCacheRowCount("SectionNameCache") < 2;


                if (hideEdition)
                {
                    lblEdition.Visible = false;
                    ddEditionList.Visible = false;
                    CheckBoxListSubEditions.Visible = false;
                }               

				existingProductionPrompt = 0;
				existingProductionPrompt2 = 0;

				saveConfirm.Value = "0";
                saveConfirm2.Value = "0";

                updateTree = 0;

                if ((string)Session["SelectedPress"] != "")
                    Session["SelectedPlanPress"] = (string)Session["SelectedPress"];
                else
                {
                    if ((int)Session["DefaultPressID"] > 0)
                        Session["SelectedPlanPress"] = (bool)Application["UsePressGroups"] ? Globals.GetNameFromID("PressGroupNameCache", (int)Session["DefaultPressID"]) : Globals.GetNameFromID("PressNameCache", (int)Session["DefaultPressID"]);
                    if ((string)Session["SelectedPlanPress"] == "")
                    {
                        DataTable dt = (bool)Application["UsePressGroups"] ? (DataTable)Cache["PressGroupNameCache"] : (DataTable)Cache["PressNameCache"];
                        Session["SelectedPlanPress"] = (string)dt.Rows[0]["name"];
                    }
                }

				CreatePublicationDropDown();

                DateTime t = DateTime.Now;
                t = t.AddDays(1.0);
                dateChooserPubDate.SelectedDate = t;

                if ((int)Session["DefaultPublicationID"] > 0)
				{
					string defaultPublicationName = Globals.GetNameFromID("PublicationNameCache", (int)Session["DefaultPublicationID"]);
					if (defaultPublicationName != "")
						ddPublicationList.SelectedValue = defaultPublicationName;
   				}

                lblImpositionSection1.Visible = true;
                lblImpositionSection1.Enabled = true;

                lblImpositionSection1.Visible = true;
                ddSectionList1.Visible = true;
                lblImpositionSection2.Visible = true;
                ddSectionList2.Visible = true;
                lblImpositionSection3.Visible = true;
                ddSectionList3.Visible = true;

                if (hideSection)
                {
                    lblImpositionSection2.Visible = false;
                    ddImpositionList2.Visible = false;
                    lblImpositionSection3.Visible = false;
                    ddImpositionList2.Visible = false;
                }


                SetEditionsFromPublication();
                SetSectionsFromPublication();

                CreatePageFormatDropDown();

                ddPublicationList.Enabled = true;

                ResetInputs();

            }

            foreach (Telerik.Web.UI.RadWindow win in RadWindowManager1.Windows)
                win.VisibleOnPageLoad = false;

			if (saveConfirm.Value == "1")
				SavePagePlanXML(true);       

        }

		private void CreatePageFormatDropDown()
		{
            string errmsg = "";
            CCDBaccess db = new CCDBaccess();
            List<string> pageFormatList = new List<string>();
            List<int> pageFormatNupList = new List<int>();
            db.LoadNicePageFormatList(ref pageFormatList, ref pageFormatNupList, out errmsg);

            ddPageFormatList.Items.Clear();
            for (int i= 0; i< pageFormatList.Count; i++)
                ddPageFormatList.Items.Add(new Telerik.Web.UI.RadComboBoxItem(pageFormatList[i], pageFormatList[i] + "." + pageFormatNupList[i].ToString()));
            if (ddPageFormatList.Items.Count > 0)
                ddPageFormatList.SelectedIndex = 0;

            ddPageFormatList_SelectedIndexChanged(null, null);
        }


        private void SetEditionsFromPublication()
        {
            int publicationID = Globals.GetIDFromName("PublicationNameCache", ddPublicationList.SelectedValue);
            DataTable table = (DataTable)Cache["EditionNameCache"];
            ddEditionList.Items.Clear();

            if (Globals.HasAllowedEditions(publicationID) == false)
            {
                foreach (DataRow row in table.Rows)
                {
                    ddEditionList.Items.Add(new Telerik.Web.UI.RadComboBoxItem((string)row["Name"], (string)row["Name"]));
                }

                if (ddEditionList.Items.Count > 0)
                    ddEditionList.SelectedIndex = 0;

                SetSubEditionsFromPublication();
                return;
            }

            foreach (DataRow row in table.Rows)
            {
                if (Globals.IsAllowedEdition(publicationID, (int)row["ID"]))
                {
                    ddEditionList.Items.Add(new Telerik.Web.UI.RadComboBoxItem((string)row["Name"], (string)row["Name"]));
                }
            }
            if (ddEditionList.Items.Count > 0)
                ddEditionList.SelectedIndex = 0;

            SetSubEditionsFromPublication();
        }

        private void SetSubEditionsFromPublication()
        {
            int publicationID = Globals.GetIDFromName("PublicationNameCache", ddPublicationList.SelectedValue);
            DataTable table = (DataTable)Cache["EditionNameCache"];

            CheckBoxListSubEditions.Items.Clear();

            if (Globals.HasAllowedEditions(publicationID) == false)
            {
                foreach (DataRow row in table.Rows)
                {
                    if ((string)row["Name"] != ddEditionList.SelectedValue)
                        CheckBoxListSubEditions.Items.Add((string)row["Name"]);
                }

                return;
            }

            foreach (DataRow row in table.Rows)
            {
                if (Globals.IsAllowedEdition(publicationID, (int)row["ID"]))
                {
                    if ((string)row["Name"] != ddEditionList.SelectedValue)
                        CheckBoxListSubEditions.Items.Add((string)row["Name"]);
                }
            }
        }

        private void SetSectionsFromPublication()
        {
            int publicationID = Globals.GetIDFromName("PublicationNameCache", ddPublicationList.SelectedValue);

            DataTable table = (DataTable)Cache["SectionNameCache"];
            ddSectionList1.Items.Clear();
            ddSectionList2.Items.Clear();
            ddSectionList3.Items.Clear();

            if (Globals.HasAllowedSections(publicationID) == false)
            {
                // No restrictions - FillErrorEventArgs all...
                foreach (DataRow row in table.Rows)
                {
                    ddSectionList1.Items.Add(new Telerik.Web.UI.RadComboBoxItem((string)row["Name"], (string)row["Name"]));
                    ddSectionList2.Items.Add(new Telerik.Web.UI.RadComboBoxItem((string)row["Name"], (string)row["Name"]));
                    ddSectionList3.Items.Add(new Telerik.Web.UI.RadComboBoxItem((string)row["Name"], (string)row["Name"]));

                }
                return;
            }

            foreach (DataRow row in table.Rows)
            {
                if (Globals.IsAllowedSection(publicationID, (int)row["ID"]))
                {          
                    ddSectionList1.Items.Add(new Telerik.Web.UI.RadComboBoxItem((string)row["Name"], (string)row["Name"]));
                    ddSectionList2.Items.Add(new Telerik.Web.UI.RadComboBoxItem((string)row["Name"], (string)row["Name"]));
                    ddSectionList3.Items.Add(new Telerik.Web.UI.RadComboBoxItem((string)row["Name"], (string)row["Name"]));
                }
            }

            ddSectionList1.SelectedIndex = 0;
            ddSectionList2.SelectedIndex = ddSectionList2.Items.Count > 0 ? 1 : 0;
            ddSectionList3.SelectedIndex = ddSectionList3.Items.Count > 1 ? 2 : 0;
        }
    
        private void CreatePublicationDropDown()
        {
            DataTable table = (DataTable)Cache["PublicationNameCache"];

            ddPublicationList.Items.Clear();

            string pubsallowed = (string)Session["PublicationsAllowed"];
            string[] publist = pubsallowed.Split(',');

            foreach (DataRow row in table.Rows)
            {
                string thisPublication = (string)row["Name"];

                if (pubsallowed == "*")
                    ddPublicationList.Items.Add(new Telerik.Web.UI.RadComboBoxItem(thisPublication, thisPublication));
                else
                {
                    foreach (string sp in publist)
                    {
                        if (sp == thisPublication)
                        {
                            ddPublicationList.Items.Add(new Telerik.Web.UI.RadComboBoxItem(thisPublication, thisPublication));
                            break;
                        }
                    }
                }
            }
        }

        private void CreateImpositionDropDown()
        {

            CCDBaccess db = new CCDBaccess();
            string errmsg = "";

            List<string> arr = db.GetPressTemplateList((int)Session["DefaultPressID"], out errmsg);
            string s1 = "Not used";
            ddImpositionList2.Items.Add(new Telerik.Web.UI.RadComboBoxItem(s1, s1));
            ddImpositionList3.Items.Add(new Telerik.Web.UI.RadComboBoxItem(s1, s1));

            ddImpositionList1.Items.Clear();

            foreach (string s in arr)
            {
                ddImpositionList1.Items.Add(new Telerik.Web.UI.RadComboBoxItem(s, s));
                ddImpositionList2.Items.Add(new Telerik.Web.UI.RadComboBoxItem(s, s));
                ddImpositionList3.Items.Add(new Telerik.Web.UI.RadComboBoxItem(s, s));
            }
            //SelectPageFormatFromPublication();	
        }


        protected void SetLanguage()
		{
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            SetRadToolbarLabel("Item1", "LabelPlanHeader", Global.rm.GetString("txtPlanHeader"));

            lblCover.Text = Global.rm.GetString("txtCover");

            lblPublication.Text = Global.rm.GetString("txtPublication");
            lblEdition.Text = Global.rm.GetString("txtEdition");

            lblPubdate.Text = Global.rm.GetString("txtPubDate2");
			btnSavePlan.Text = Global.rm.GetString("txtSavePlan");
			btnSavePlan.ToolTip = Global.rm.GetString("txtTooltipSavePlan");
    		btnCancel.Text =  Global.rm.GetString("txtCancel");

            lblPageFormat.Text = Global.rm.GetString("txtPageFormat");

            lblImpositionSection1.Text = Global.rm.GetString("txtImposition") + " " + Global.rm.GetString("txtSection") + " 1";
            lblImpositionSection2.Text = Global.rm.GetString("txtImposition") + " " + Global.rm.GetString("txtSection") + " 2";
            lblImpositionSection3.Text = Global.rm.GetString("txtImposition") + " " + Global.rm.GetString("txtSection") + " 3";

            dateChooserPubDate.Culture = new System.Globalization.CultureInfo("fr-FR", true);
            dateChooserPubDate.DateInput.DisplayDateFormat = "dd/MM/yyyy";

        }

        private void ResetInputs()
        {
            ddPageFormatList.SelectedIndex = 0;
            Page.SetFocus(ddPageFormatList.ID);

            lblInfo.Text = "";
            lblError.Text = "";
        }

        private void ButtonCancel_Click(object sender, System.EventArgs e)
		{
            ResetInputs();
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
			this.ddPublicationList.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(this.ddPublicationList_SelectedIndexChanged);

			this.btnSavePlan.Click += new System.EventHandler(this.btnSavePlan_Click);
			this.btnCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		
		
		//--------------------------------------
		// Save plan to database
		//--------------------------------------

		private void btnSavePlan_Click(object sender, System.EventArgs e)
		{
            //            if ((bool)Application["PlanUseXml"])
            //                SavePagePlanXML(false);
            //            else
            SavePagePlanXML(false);
		}

        private string GeneratePageName(string publication, DateTime pubDate, string pageName)
        {
            string finalPubName = publication.ToUpper();
            if (finalPubName.Contains("AUTRE"))
                finalPubName = "AT" + finalPubName.Substring(5, 1);
            finalPubName = finalPubName.Replace("-1", "");
            return string.Format("{0}-{1}_{2:00}{3:00}", finalPubName, pageName, pubDate.Day, pubDate.Month);
        }

        private string GeneratePressEditionName(string publication, string edition)
        {
            string pubUpper = publication.ToUpper();
            if (pubUpper.Contains("AUTRE") && pubUpper.Length >= 6)
                pubUpper = "AUTR" + pubUpper[5];
            string edUpper = edition.ToUpper();
            if (edUpper.Contains(" "))
                edUpper = edUpper.Substring(0,edUpper.IndexOf(" "));

            if (pubUpper.Contains("NICE-MATIN"))
                    pubUpper = "NMATI";
            if (pubUpper.Length > 5)
                pubUpper = pubUpper.Substring(0, 5);
            return pubUpper + "." + edUpper;

        }
        

        private void SavePagePlanXML(bool overwriteconfirmed)
        {
            CCDBaccess db = new CCDBaccess();
            string errmsg = "";
            lblInfo.Text = "";
            lblError.Text = "";

            bool bExistingProduction = false;

            DateTime selectedDateTime = (DateTime)dateChooserPubDate.SelectedDate;
            DateTime pubdate = new DateTime(selectedDateTime.Year, selectedDateTime.Month, selectedDateTime.Day, 0, 0, 0, 0);
            int publicationID = Globals.GetIDFromName("PublicationNameCache", ddPublicationList.SelectedValue);
            string pageFormat = ddPageFormatList.SelectedValue;
            int n = pageFormat.IndexOf(".");
            if (n != -1)
                pageFormat = pageFormat.Substring(0, n).Trim();

            List<TemplateInfo> templateList = new List<TemplateInfo>();
            if (db.LoadNiceTemplatePageFormatList(pageFormat, ref templateList, out errmsg) == false)
            {
                lblError.Text = "LoadNiceTemplatePageFormatList - " +errmsg;
                lblError.ForeColor = Color.Red;
                return;
            }



            if (templateList.Count == 0)
            {
                lblError.Text = "No templates associated with pageformat '"+ pageFormat+"'";
                lblError.ForeColor = Color.Red;
                return;
            }
            /*  double latestHours = db.GetLatestPossibleUpdate(publicationID, out errmsg);
              if (latestHours > 0)
              {
                  DateTime hotTime = pubdate.AddHours(-1.0 * latestHours);
                  if (hotTime < DateTime.Now)
                  {
                      lblError.Text = Global.rm.GetString("txtProductionLocked");
                      lblError.ForeColor = Color.Orange;
                      return;
                  }
              }*/

            Pageplan pagePlan = new Pageplan();


            pagePlan.publicationName = ddPublicationList.SelectedValue;


            pagePlan.publicationDate = selectedDateTime;
            pagePlan.planName = string.Format("{0:00}-{1:00}-{2:0000} {3}", selectedDateTime.Day, selectedDateTime.Month, selectedDateTime.Year, ddPublicationList.SelectedValue);
            pagePlan.updatetime = DateTime.Now;

            PlanDataEdition edition = new PlanDataEdition(ddEditionList.SelectedValue);
            edition.editionCopy = 1;
            edition.editionComment = "";
            edition.editionSequenceNumber = 1;
            pagePlan.editionList.Add(edition);

            string sInfo = "";



            int runThroughPagination = 1;
            int pageID = 1;
            int pageCount = 0;
            string fileName = txtFilename.Text.Trim().Replace(".pdf", "").Replace(".PDF", "");

            for (int sec = 1; sec <= 3; sec++)
            {
                string thisSection = ddSectionList1.SelectedValue;
                int thispageCount = 32;// Globals.TryParse(RadComboBoxPagecount1.SelectedValue, 2);
             /*   if (sec == 2)
                {
                    thisSection = ddSectionList2.SelectedValue;
                    thispageCount = Globals.TryParse(RadComboBoxPagecount2.SelectedValue, 0);
                }
                else if (sec == 3)
                {
                    thisSection = ddSectionList3.SelectedValue;
                    thispageCount = Globals.TryParse(RadComboBoxPagecount3.SelectedValue, 0);
                }*/
                if (thispageCount > 0 && thisSection != "")
                {
                    PlanDataSection section = new PlanDataSection(thisSection);
                    section.pagesInSection = (int)thispageCount;
                    

                    for (int pg = 1; pg <= section.pagesInSection; pg++)
                    {
                        PlanDataPage page = new PlanDataPage();
                        page.pagination = runThroughPagination++;
                        
                        page.comment = "";
                        page.miscstring1 = "";
                        page.miscstring2 = "";

                        page.pageFormat = pageFormat;

                        page.uniquePage = PageUniqueType.Unique;
                        page.pageIndex = (bool)RadCheckBoxCover.Checked ? pg + 2 : pg;
                        page.pageName = page.pageIndex.ToString();

                        if (fileName.Trim() != "")
                            page.fileName = string.Format("{0}_{1:000}", fileName.Trim(), page.pageIndex);
                        else
                            page.fileName = GeneratePageName(pagePlan.publicationName, pagePlan.publicationDate, page.pageName);

                        if (fileName.Trim() != "" && Globals.PlanningSetPageNameAsPlanPageName)
                            page.pageName = page.fileName;
                        page.pageID = pageID.ToString();
                        pageID++;
                        page.masterPageID = page.pageID;

                        page.colorList.Add(new PlanDataPageSeparation() { colorName = "C" });
                        page.colorList.Add(new PlanDataPageSeparation() { colorName = "M" });
                        page.colorList.Add(new PlanDataPageSeparation() { colorName = "Y" });
                        page.colorList.Add(new PlanDataPageSeparation() { colorName = "K" });
                        section.pageList.Add(page);
                    }
                    pageCount += section.pagesInSection;


                    pagePlan.editionList[0].sectionList.Add(section);
                }
            }

            //string pageOutputFile = (string)Application["PressTemplateOutputFolder"] + @"\" + string.Format("WebCenter_{0}_{1:00}-{2:00}_{3}_{4}_{5}", ddPublicationList.SelectedValue, pubdate.Day, pubdate.Month, ddEditionList.SelectedValue, Globals.DateTime2TimeStamp(DateTime.Now), (string)Session["UserName"]);

            int pressSectionNumber = 1;
            foreach (TemplateInfo tinfo in templateList)
            {
                string pressName = Globals.GetNameFromID("PressNameCache", tinfo.pressID);
                foreach (PlanDataEdition ed in pagePlan.editionList)
                {
                    PlanDataPress pressitem = new PlanDataPress(pressName);
                    pressitem.copies = 1;
                    pressitem.paperCopies = 0;
                    pressitem.pressRunTime = DateTime.MinValue;
                    ed.pressList.Add(pressitem);
                    string PressEditionName = GeneratePressEditionName(pagePlan.publicationName, ed.editionName);
                    pagePlan.GenerateSheets(ed.editionName, tinfo, false, pressSectionNumber++, PressEditionName);
                }

                pagePlan.sXmlFile = string.Format("{0:yyyy-MM-dd}", pagePlan.publicationDate) + "_" + pagePlan.publicationName + "_" + pressName + "_" + Globals.GenerateTimeStamp() + ".xml";
                pagePlan.planID = pagePlan.publicationName + " " + string.Format("{0:yyyy-MM-dd}", pagePlan.publicationDate) + " " + pressName;


                if (pagePlan.GenerateXML(pressName, false, true, false, out errmsg) == false)
                {
                    lblError.Text = "Error writing XML plan file " + pagePlan.sXmlFile + " - " + errmsg;
                    lblError.ForeColor = Color.Red;
                    return;
                }

            }
              
            updateTree = 0;
            Session["RefreshTree"] = false;

            existingProductionPrompt = 0;
            existingProductionPrompt2 = 0;
            saveConfirm.Value = "0";

            lblInfo.Text = "" + string.Format("Plan added to import queue..({0}-{1:00}.{2:00} {3} pages)", ddPublicationList.SelectedValue, pubdate.Day, pubdate.Month,  pageCount);

            lblError.ForeColor = Color.Green;
            updateTree = 1;
            Session["RefreshTree"] = true;

            string userName = (string)Session["UserName"];
            if ((string)Session["UserDomain"] != "")
                userName = (string)Session["UserDomain"] + @"\" + (string)Session["UserName"];

            db.InsertUserHistory(userName, bExistingProduction ? 4 : 2, string.Format("{0}-{1:00}.{2:00}-{3}-{4}", ddPublicationList.SelectedValue, pubdate.Day, pubdate.Month, ddEditionList.SelectedValue, sInfo), out errmsg);

            db.InsertLogEntry((int)Application["ProcessID"],
                                            (int)Globals.EventCodes.PlanStartCreate,
                                            "WebCenter " + userName,
                                            string.Format("{0}-{1:00}{2:00}", ddPublicationList.SelectedValue, pubdate.Day, pubdate.Month),sInfo, 0, out errmsg);
        }
		
		protected void ddPublicationList_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
		{
			SetEditionsFromPublication();
            SetSectionsFromPublication();
            Session["SubEditions"] = null;
        }

		private void LinkButtonAddNewPublication_Click(object sender, System.EventArgs e)
		{
			doPopupPublicationWindow();
		}

		private void doPopupPublicationWindow()
		{
			string popupScript =
				"<script language='javascript'>" +
				"var xpos = 300;" + 
				"var ypos = 300;" +
				"if(window.screen) { xpos = (screen.width-500)/2; ypos = (screen.height-310)/2; }" + 
				"var s = 'status=no,top='+ypos+',left='+xpos+',width=500,height=310';" +
				"var PopupWindow = window.open('ManagePublications.aspx','Publications',s);" + 	
				"if (parseInt(navigator.appVersion) >= 4) PopupWindow.focus();" +
				"</script>";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "PopupScript", popupScript);
		}

		private void LinkButtonAddNewPageformat_Click(object sender, System.EventArgs e)
		{
			doPopupPageformatWindow();
		}

		private void doPopupPageformatWindow()
		{
			string popupScript =
				"<script language='javascript'>" +
				"var xpos = 300;" + 
				"var ypos = 300;" +
				"if(window.screen) { xpos = (screen.width-500)/2; ypos = (screen.height-260)/2; }" + 
				"var s = 'status=no,top='+ypos+',left='+xpos+',width=500,height=260';" +
				"var PopupWindow = window.open('ManagePageFormats.aspx','Pageformats',s);" + 	
				"if (parseInt(navigator.appVersion) >= 4) PopupWindow.focus();" +
				"</script>";

		    Page.ClientScript.RegisterStartupScript(this.GetType(), "PopupScript", popupScript);
        }


        private bool HasRadItemByText(Telerik.Web.UI.RadComboBoxItemCollection items, string element)
        {
            foreach (Telerik.Web.UI.RadComboBoxItem item in items)
            {
                if (item.Text == element)
                    return true;
            }
            return false;
        }

        private bool HasRadItemByValue(Telerik.Web.UI.RadComboBoxItemCollection items, string element)
        {
            foreach (Telerik.Web.UI.RadComboBoxItem item in items)
            {
                if (item.Value == element)
                    return true;
            }
            return false;
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

        private bool SetPublicationCombo(string s)
        {
            foreach (Telerik.Web.UI.RadComboBoxItem item in ddPublicationList.Items)
            {
                if (item.Text == s)
                {
                    ddPublicationList.SelectedValue = s;
                    return true;
                }
            }
            return false;
        }

        private bool SetEditionCombo(string s)
        {
            foreach (Telerik.Web.UI.RadComboBoxItem item in ddEditionList.Items)
            {
                if (item.Text == s)
                {
                    ddEditionList.SelectedValue = s;
                    return true;
                }
            }

            return false;
        }

        private bool SetSectionCombos(string s1, string s2, string s3)
        {
            bool found = false;
            if (s1 != "")
            {
                foreach (Telerik.Web.UI.RadComboBoxItem item in ddSectionList1.Items)
                {
                    if (item.Text == s1)
                    {
                        ddSectionList1.SelectedValue = s1;
                        found = true;
                        break;
                    }
                }
            }


            if (s2 != "")
            {
                foreach (Telerik.Web.UI.RadComboBoxItem item in ddSectionList2.Items)
                {
                    if (item.Text == s2)
                    {
                        ddSectionList2.SelectedValue = s2;
                        found = true;
                        break;
                    }
                }
            }

            if (s3 != "")
            {
                foreach (Telerik.Web.UI.RadComboBoxItem item in ddSectionList3.Items)
                {
                    if (item.Text == s3)
                    {
                        ddSectionList3.SelectedValue = s3;
                        found = true;
                        break;
                    }
                }
            }

            return found;
        }

        protected void ddPageFormatList_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {          
            int maxPageCount = 128;
            string pageFormat = ddPageFormatList.SelectedValue;
            int n = pageFormat.IndexOf(".");
            int nUP = 1;
            if (n != -1)
                nUP = Globals.TryParse(pageFormat.Substring(n + 1).Trim(), 1);
         /*   RadComboBoxPagecount1.Items.Clear();           
            for (int i = nUP*2; i <= maxPageCount; i += (nUP * 2))
                RadComboBoxPagecount1.Items.Add(new Telerik.Web.UI.RadComboBoxItem(i.ToString(),i.ToString()));
            RadComboBoxPagecount1.SelectedIndex = 0;

            RadComboBoxPagecount2.Items.Clear();
            for (int i = 0; i <= maxPageCount; i += (nUP * 2))
                RadComboBoxPagecount2.Items.Add(new Telerik.Web.UI.RadComboBoxItem(i.ToString(), i.ToString()));
            RadComboBoxPagecount2.SelectedIndex = 0;

            RadComboBoxPagecount3.Items.Clear();
            for (int i = 0; i <= maxPageCount; i += (nUP * 2))
                RadComboBoxPagecount3.Items.Add(new Telerik.Web.UI.RadComboBoxItem(i.ToString(), i.ToString()));
            RadComboBoxPagecount3.SelectedIndex = 0;*/
        }

        protected void ddEditionList_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            SetSubEditionsFromPublication();
        }
    }
}
                                           