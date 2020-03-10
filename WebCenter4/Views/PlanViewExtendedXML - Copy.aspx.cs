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
	public class PlanViewExtendedXMLOld : System.Web.UI.Page
	{
        protected System.Web.UI.WebControls.Panel PanelMainActionButtons;
        protected System.Web.UI.WebControls.Panel PanelEditions;
        

        protected System.Web.UI.WebControls.Label lblTemplate;
        protected System.Web.UI.WebControls.Label lblError;
        protected System.Web.UI.WebControls.Label lblPublication;
        protected System.Web.UI.WebControls.Label lblPubdate;
        protected System.Web.UI.WebControls.Label lblEdition;
        protected System.Web.UI.WebControls.Label lblInfo;

        protected System.Web.UI.WebControls.Label lblSection1;
        protected System.Web.UI.WebControls.Label lblSection2;
        protected System.Web.UI.WebControls.Label lblSection3;
        protected System.Web.UI.WebControls.Label lblImpositionInfo;

        protected Telerik.Web.UI.RadButton btnSavePlan;
        protected Telerik.Web.UI.RadButton btnCancel;

        protected System.Web.UI.HtmlControls.HtmlInputHidden saveConfirm;
        protected System.Web.UI.HtmlControls.HtmlInputHidden saveConfirm2;
        
        protected Telerik.Web.UI.RadComboBox ddPublicationList;
        protected Telerik.Web.UI.RadComboBox ddEditionList;
        protected Telerik.Web.UI.RadComboBox ddImpositionList;
        protected Telerik.Web.UI.RadComboBox ddSectionList1;
        protected Telerik.Web.UI.RadComboBox ddSectionList2;
        protected Telerik.Web.UI.RadComboBox ddSectionList3;
        protected Telerik.Web.UI.RadTextBox txtFilename;

        protected Telerik.Web.UI.RadDatePicker dateChooserPubDate;
        protected Telerik.Web.UI.RadWindowManager RadWindowManager1;
        protected Telerik.Web.UI.RadToolBar RadToolBar1;

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
               
                if (hideEdition)
                {
                    lblEdition.Visible = false;
                    ddEditionList.Visible = false;
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

                lblSection1.Visible = true;
                ddSectionList1.Visible = true;
                lblSection2.Visible = true;
                ddSectionList2.Visible = true;
                lblSection3.Visible = true;
                ddSectionList3.Visible = true;

                // Final adjustment of press....
                SelectPressFromPublication();

                SetEditionsFromPublication();
                SetSectionsFromPublication();

                CreateImpositionDropDown();

                ddPublicationList.Enabled = true;

                ResetInputs();

            }

            foreach (Telerik.Web.UI.RadWindow win in RadWindowManager1.Windows)
                win.VisibleOnPageLoad = false;

			if (saveConfirm.Value == "1")
				SavePagePlanXML(true);       

        }

		private void CreateImpositionDropDown()
		{
            string errmsg = "";
 
            List<string> arr = Globals.GetFileList((string)Application["PressTemplateFolder"], (string)Application["PressTemplateFileExtension"], true, out errmsg);

            ddImpositionList.Items.Clear();
            ddImpositionList.Items.Add("<Select plan template>");

            foreach (string s in arr)
                ddImpositionList.Items.Add(new Telerik.Web.UI.RadComboBoxItem(s, s + "." + (string)Application["PressTemplateFileExtension"]));
		}
	
       
        private string GetAnyPress()
        {
            DataTable dt = (bool)Application["UsePressGroups"] ? (DataTable)HttpContext.Current.Cache["PressGroupNameCache"] : (DataTable)HttpContext.Current.Cache["PressNameCache"];
            if (dt.Rows.Count > 0)
                return (string)dt.Rows[0]["Name"];

            return "";

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
        }

        private void SelectPressFromPublication()
        {
            int publicationID = Globals.GetIDFromName("PublicationNameCache", ddPublicationList.SelectedValue);
            string press = GetDefaultPressFromPublicationDefault(publicationID);
            if (press != "")
                Session["SelectedPlanPress"] = press;

            if ((string)Session["SelectedPlanPress"] == "")
            {
                if ((string)Session["SelectedPress"] != "")
                    Session["SelectedPlanPress"] = (string)Session["SelectedPress"];
                else
                    Session["SelectedPlanPress"] = GetAnyPress();
            }
        }

        // PS: Returnes PressGroup if UsePressGroups=true
        private string  GetDefaultPressFromPublicationDefault(int publicationID)
        {
            DataRow row = Globals.GetPublicationRow(publicationID);
            if (row == null)
                return "";

            int nPressID = (int)row["DefaultPressID"];
            if (nPressID > 0)
                return (bool)Application["UsePressGroups"] ? Globals.GetNameFromID("PressGroupNameCache", nPressID) : Globals.GetNameFromID("PressNameCache", nPressID);

            return "";
        }      


        private string GetPressFromDefaultPublicationTemplate(string publication)
        {
            string press = "";
            ArrayList templateList = Globals.GetPublicationTemplates(ddPublicationList.SelectedValue);
            foreach (int templateID in templateList)
            {
                press = Globals.GetPressFromTemplate(templateID);
                if (press != "")
                    return press;
            }

            return press;
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

        private bool HasPublicationInPressGroup(string publication)
        {
            if ((string)Session["SelectedPlanPress"] == "")     // may be pressgroup!
                return true;
            DataRow row = Globals.GetPublicationRow(publication);
            if (row == null)
                return true;

            if ((bool)Application["UsePressGroups"])
            {
                int pressGroupID = Globals.GetIDFromName("PressGroupNameCache", (string)Session["SelectedPlanPress"]);
                if (pressGroupID == 0)
                    return true;

                int defaultPressGroupIDForPublication = (int)row["DefaultPressID"];
                if (defaultPressGroupIDForPublication == 0)
                    return true;

                return defaultPressGroupIDForPublication == pressGroupID;
            }
            else
            {
                int pressID = Globals.GetIDFromName("PressNameCache", (string)Session["SelectedPlanPress"]);
                if (pressID == 0)
                    return true;

                int defaultPressIDForPublication = (int)row["DefaultPressID"];
                if (defaultPressIDForPublication == 0)
                    return true;

                return defaultPressIDForPublication == pressID;
            }
        }


		protected void SetLanguage()
		{
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            SetRadToolbarLabel("Item1", "LabelPlanHeader", Global.rm.GetString("txtPlanHeader"));
             
			lblPublication.Text = Global.rm.GetString("txtPublication");
            lblEdition.Text = Global.rm.GetString("txtEdition");

            lblPubdate.Text = Global.rm.GetString("txtPubDate2");
			btnSavePlan.Text = Global.rm.GetString("txtSavePlan");
			btnSavePlan.ToolTip = Global.rm.GetString("txtTooltipSavePlan");
    		btnCancel.Text =  Global.rm.GetString("txtCancel");		
			
            lblTemplate.Text = Global.rm.GetString("txtTemplate");
            lblSection1.Text = Global.rm.GetString("txtSection") + " 1";
            lblSection2.Text = Global.rm.GetString("txtSection") + " 2";
            lblSection3.Text = Global.rm.GetString("txtSection") + " 3";

            dateChooserPubDate.Culture = new System.Globalization.CultureInfo("fr-FR", true);
            dateChooserPubDate.DateInput.DisplayDateFormat = "dd/MM/yyyy";

        }

        private void ResetInputs()
        {
            ddImpositionList.SelectedIndex = 0;
            Page.SetFocus(ddImpositionList.ID);

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

            double latestHours = db.GetLatestPossibleUpdate(publicationID, out errmsg);
            if (latestHours > 0)
            {
                DateTime hotTime = pubdate.AddHours(-1.0 * latestHours);
                if (hotTime < DateTime.Now)
                {
                    lblError.Text = Global.rm.GetString("txtProductionLocked");
                    lblError.ForeColor = Color.Orange;
                    return;
                }
            }

            PagiFile pagiFile = new PagiFile();
            string pagiPath = (string)Application["PressTemplateFolder"] + @"\" + ddImpositionList.Text;

            if ((string)Application["PressTemplateFileExtension"] != "")
                pagiPath += "." + (string)Application["PressTemplateFileExtension"];
            int paginationstart = 1;
            bool loadStatus = pagiFile.ParseFile(pagiPath, ref paginationstart);

            if (loadStatus == false)
            {
                lblError.Text = "Error loading PAGI plan file " + pagiPath;
                lblError.ForeColor = Color.Red;
                return;
            }

            // Set current selected publication, pubdate, edition and section(s)
            string oldPublication = pagiFile.plan.publicationName;
            pagiFile.plan.publicationName = ddPublicationList.SelectedValue;

            if (oldPublication != ddPublicationList.SelectedValue)
            {
                string s = pagiFile.plan.publicationName.ToUpper();
                // AutreX -> AUTRX
                if (s.IndexOf("AUTR") != -1 && s.Length > 5)
                    pagiFile.plan.publicationAlias = "AUTR" + s.Substring(5, 1);
            }

            pagiFile.plan.publicationDate = selectedDateTime;
            pagiFile.plan.planName = string.Format("{0:00}-{1:00}-{2:0000} {3}", selectedDateTime.Day, selectedDateTime.Month, selectedDateTime.Year, ddPublicationList.SelectedValue);
            pagiFile.plan.updatetime = DateTime.Now;


            string sInfo = "";
            if (pagiFile.plan.editionList.Count > 0)
            {
                string oldEdition = pagiFile.plan.editionList[0].editionName;
                pagiFile.plan.editionList[0].editionName = ddEditionList.SelectedValue;

                if (oldEdition != ddEditionList.SelectedValue)
                {
                    if (pagiFile.plan.editionList[0].editionName.IndexOf(' ') == 2)
                        pagiFile.plan.editionList[0].editionComment = pagiFile.plan.editionList[0].editionName.Substring(0, 2);
                }

                foreach (PagiPlanDataSection sec in pagiFile.plan.editionList[0].sectionList)
                {
                    foreach (PagiPlanDataPage page in sec.pageList)
                    {
                        if (page.masterEdition == oldEdition)
                            page.masterEdition = ddEditionList.SelectedValue;

                    }

                    if (sInfo != "")
                        sInfo += ", ";
                    sInfo += string.Format("{0} {1} pages", sec.sectionName, sec.pageList.Count);
                }

                if (pagiFile.plan.editionList[0].sectionList.Count > 0)
                    pagiFile.plan.editionList[0].sectionList[0].sectionName = ddSectionList1.SelectedValue;
                if (pagiFile.plan.editionList[0].sectionList.Count > 1)
                    pagiFile.plan.editionList[0].sectionList[1].sectionName = ddSectionList2.SelectedValue;
                if (pagiFile.plan.editionList[0].sectionList.Count > 2)
                    pagiFile.plan.editionList[0].sectionList[2].sectionName = ddSectionList3.SelectedValue;
            }


            string pageOutputFile = (string)Application["PressTemplateOutputFolder"] + @"\" + string.Format("WebCenter_{0}_{1:00}-{2:00}_{3}_{4}_{5}", ddPublicationList.SelectedValue, pubdate.Day, pubdate.Month, ddEditionList.SelectedValue, Globals.DateTime2TimeStamp(DateTime.Now), (string)Session["UserName"]);
            
            if (pagiFile.GeneratePagiFile(pageOutputFile, txtFilename.Text.Trim(), paginationstart) == false)
            {
                lblError.Text = "Error writing PAGI plan file " + pageOutputFile;
                lblError.ForeColor = Color.Red;
                return;
            }
              
            updateTree = 0;
            Session["RefreshTree"] = false;

            existingProductionPrompt = 0;
            existingProductionPrompt2 = 0;
            saveConfirm.Value = "0";

            lblInfo.Text = "Plan added to import queue..(" + string.Format("{0}-{1:00}.{2:00}-{3}", ddPublicationList.SelectedValue, pubdate.Day, pubdate.Month, ddEditionList.SelectedValue);

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
			SelectPressFromPublication();
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

        protected void ddImpositionList_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (ddImpositionList.SelectedIndex == 0)
                return;
            PagiFile pagiFile = new PagiFile();
            string pagiPath = (string)Application["PressTemplateFolder"] + @"\" + ddImpositionList.Text;

            if ((string)Application["PressTemplateFileExtension"] != "")
                pagiPath += "." + (string)Application["PressTemplateFileExtension"];

            lblImpositionInfo.Text = "";
            int paginationstart = 1;
            if (pagiFile.ParseFile(pagiPath, ref paginationstart) == false)
            {
                lblError.Text = "Error loading PAGI master file";
                return;
            }


            lblImpositionInfo.Text = pagiFile.GetPagiSummary();

            SetPublicationCombo(pagiFile.plan.publicationName);
            SetEditionsFromPublication();
            SetSectionsFromPublication();

            lblSection1.Visible = true;
            ddSectionList1.Visible = true;
            lblSection2.Visible = true;
            ddSectionList2.Visible = true;
            lblSection3.Visible = true;
            ddSectionList3.Visible = true;

            if (pagiFile.plan.editionList.Count > 0)
            {
                SetEditionCombo(pagiFile.plan.editionList[0].editionName);

                string s1 = "", s2 = "", s3 = "";

                if (pagiFile.plan.editionList[0].sectionList.Count > 0)
                    s1 = pagiFile.plan.editionList[0].sectionList[0].sectionName;
                if (pagiFile.plan.editionList[0].sectionList.Count > 1)
                    s2 = pagiFile.plan.editionList[0].sectionList[1].sectionName;
                if (pagiFile.plan.editionList[0].sectionList.Count > 2)
                    s3 = pagiFile.plan.editionList[0].sectionList[2].sectionName;

                SetSectionCombos(s1, s2, s3);

                if (pagiFile.plan.editionList[0].sectionList.Count < 3)
                {
                    lblSection3.Visible = false;
                    ddSectionList3.Visible = false;
                }

                if (pagiFile.plan.editionList[0].sectionList.Count < 2)
                {
                    lblSection2.Visible = false;
                    ddSectionList2.Visible = false;
                }

            }

        }

       
    }
}
                                           