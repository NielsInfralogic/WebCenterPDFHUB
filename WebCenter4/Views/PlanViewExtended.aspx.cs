using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
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
	public class PlanViewExtended : System.Web.UI.Page
	{
        protected System.Web.UI.WebControls.Panel PanelMainActionButtons;
        protected System.Web.UI.WebControls.Panel PanelAddPlan;
        protected System.Web.UI.WebControls.Panel PanelDeletePlan;
        protected System.Web.UI.WebControls.Panel PanelEditPlan;
        protected System.Web.UI.WebControls.Panel PanelEditions;
        
        protected System.Web.UI.WebControls.DataGrid DataGridProductionList;
        protected System.Web.UI.WebControls.DataGrid DataGridProductionListEdit;

        protected System.Web.UI.WebControls.Label lblComment;
        protected System.Web.UI.WebControls.Label lblImposition;
        protected System.Web.UI.WebControls.Label lblError;
        protected System.Web.UI.WebControls.Label lblPublication;
        protected System.Web.UI.WebControls.Label lblPubdate;
        protected System.Web.UI.WebControls.Label lblEdition;
        protected System.Web.UI.WebControls.Label lblApproval;
        protected System.Web.UI.WebControls.Label lblAddPagePlan;
        protected System.Web.UI.WebControls.Label LblPlanUpdate;
        protected System.Web.UI.WebControls.Label lblDeletePlan;
        protected System.Web.UI.WebControls.Label lblInfo;
        protected System.Web.UI.WebControls.Label lblCirculation;
        protected System.Web.UI.WebControls.Label lblCirculation2;
        protected System.Web.UI.WebControls.Label lblWeekNumber;
        protected System.Web.UI.WebControls.Label lblWeekNumber2;
        protected System.Web.UI.WebControls.Label lblDeadline;
        protected System.Web.UI.WebControls.Label lblDeadlineInfo;

        protected System.Web.UI.WebControls.Label lblImpositionSection1;
        protected System.Web.UI.WebControls.Label lblImpositionSection2;
        protected System.Web.UI.WebControls.Label lblImpositionSection3;
        
        protected System.Web.UI.WebControls.CheckBox checkApprovalRequired;
        protected System.Web.UI.WebControls.CheckBox cbKeepColors;
        protected System.Web.UI.WebControls.CheckBox cbKeepApproval;
        protected System.Web.UI.WebControls.CheckBox cbKeepUnique;

        protected Telerik.Web.UI.RadButton btnSavePlan;
        protected Telerik.Web.UI.RadButton btnDeletePlan;
        protected Telerik.Web.UI.RadButton btlCloseDeletePlan;
        protected Telerik.Web.UI.RadButton btlCloseEditPlan;
        protected Telerik.Web.UI.RadButton btnCancel;
        protected Telerik.Web.UI.RadButton btnAddPlan;

        protected System.Web.UI.WebControls.TextBox txtComment;

        protected System.Web.UI.HtmlControls.HtmlInputHidden saveConfirm;
        protected System.Web.UI.HtmlControls.HtmlInputHidden saveConfirm2;
        
        protected Telerik.Web.UI.RadComboBox ddPublicationList;
        protected Telerik.Web.UI.RadComboBox ddEditionList;
        protected Telerik.Web.UI.RadComboBox ddImpositionList;
        protected Telerik.Web.UI.RadComboBox ddImpositionList2;
        protected Telerik.Web.UI.RadComboBox ddImpositionList3;
        protected Telerik.Web.UI.RadDatePicker dateChooserPubDate;
        protected Telerik.Web.UI.RadNumericTextBox RadNumericTextBoxCirculation;
        protected Telerik.Web.UI.RadNumericTextBox RadNumericTextBoxCirculation2;
        protected Telerik.Web.UI.RadNumericTextBox txtWeekNumber;

        protected Telerik.Web.UI.RadWindowManager RadWindowManager1;
        protected Telerik.Web.UI.RadToolBar RadToolBar1;

        // Variables for javascript communication

        protected int updateTree;
        protected int existingProductionPrompt;
        protected int existingProductionPrompt2;
        protected int oddpagecountPrompt;
        protected bool hasOnlyOnePerssAndEdition;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ((string)Session["UserName"] == null)
				Response.Redirect("~/SessionTimeout.htm");

			if ((string)Session["UserName"] == "")
				Response.Redirect("/Denied.htm");

			SetLanguage();

        //    hasOnlyOnePerssAndEdition = false;
       //     hasOnlyOneSection = false;
			
            updateTree = 0;

			Session["RefreshTree"] = false;

			btnAddPlan.Attributes.Add("onClick", "document.getElementById('ProgressBar').style.display = '';");
            
			btnSavePlan.Attributes.Add("onClick", "document.getElementById('ProgressBar').style.display = '';");

			if (!this.IsPostBack) 
			{
                dateChooserPubDate.Culture = CultureInfo.CurrentCulture;
				Globals.ForceCacheReloadsSmall();

				Session["RefreshTree"] = false;
                Session["SubEditions"] = null;

                btnAddPlan.Visible = (bool)Application["HideAddPlanButton"] == false;
                lblAddPagePlan.Visible = (bool)Application["HideAddPlanButton"] == false;
                PanelMainActionButtons.Visible = true;


                btnDeletePlan.Visible = (bool)Application["HideDeletePlanButton"] == false;
				lblDeletePlan.Visible = (bool)Application["HideDeletePlanButton"] == false;
                PanelDeletePlan.Visible = false;
                PanelEditPlan.Visible = false;
                PanelAddPlan.Visible = false;

                lblWeekNumber.Visible = (bool)Application["HidePlanWeekNumber"] == false;
                lblWeekNumber2.Visible = (bool)Application["HidePlanWeekNumber"] == false;
                txtWeekNumber.Visible = (bool)Application["HidePlanWeekNumber"] == false;
                txtWeekNumber.Value = 0;
                
                lblCirculation.Visible = (bool)Application["HidePlanCirculation"] == false;
				RadNumericTextBoxCirculation.Visible = (bool)Application["HidePlanCirculation"] == false;

				lblCirculation2.Visible = (bool)Application["HidePlanCirculation2"] == false;
				RadNumericTextBoxCirculation2.Visible = (bool)Application["HidePlanCirculation2"] == false;

                lblImpositionSection1.Visible = true;
                lblImpositionSection1.Enabled = true;

                bool hideEdition = Globals.GetCacheRowCount("EditionNameCache") < 2;
                bool hideSection = Globals.GetCacheRowCount("SectionNameCache") < 2;

                if (hideEdition)
                {
                    lblEdition.Visible = false;
                    ddEditionList.Visible = false;
                }
                if (hideSection)
                {
                    lblImpositionSection2.Visible = false;
                    ddImpositionList2.Visible = false;
                    lblImpositionSection3.Visible = false;
                    ddImpositionList2.Visible = false;
                }

				cbKeepColors.Checked = (bool)Session["KeepExistingColors"];
				cbKeepUnique.Checked = (bool)Session["KeepExistingUnique"];
				cbKeepApproval.Checked = (bool)Session["KeepExistingApproval"];

				lblInfo.Text = "";
				lblError.Text = "";

				existingProductionPrompt = 0;
				existingProductionPrompt2 = 0;
                oddpagecountPrompt = 0;

				saveConfirm.Value = "0";
                saveConfirm2.Value = "0";

                updateTree = 0;

				txtWeekNumber.Value = 0;

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

				checkApprovalRequired.Checked = false;
                checkApprovalRequired.Visible = (bool)Application["HidePlanApprovalRequired"] == false;
                lblApproval.Visible = (bool)Application["HidePlanApprovalRequired"] == false;

				CreatePublicationDropDown();

                DateTime t = DateTime.Now;
                t = t.AddDays(1.0);
                dateChooserPubDate.SelectedDate = t;


                if ((int)Session["DefaultPublicationID"] > 0)
				{
					string defaultPublicationName = Globals.GetNameFromID("PublicationNameCache", (int)Session["DefaultPublicationID"]);
					if (defaultPublicationName != "")
						ddPublicationList.SelectedValue = defaultPublicationName;

					SetDeadlineInfo();
				}
							                
				// Final adjustment of press....
				SelectPressFromPublication();

                CreateImpositionDropDown();

                SetEditionsFromPublication();
                SetSectionsFromPublication();
                SetApprovalFromPublication();
                GetDeadlineFromPublication();
                ddPublicationList.Enabled = true;

			}

            foreach (Telerik.Web.UI.RadWindow win in RadWindowManager1.Windows)
                win.VisibleOnPageLoad = false;

			if (saveConfirm.Value == "1")
				SavePagePlan(true);            
		}

		private void CreateImpositionDropDown()
		{

            CCDBaccess db = new CCDBaccess();
            string errmsg = "";
 
            List<string> arr = db.GetPressTemplateList((int)Session["DefaultPressID"], out errmsg);
            string s1 = "Not used";
            ddImpositionList2.Items.Add(new Telerik.Web.UI.RadComboBoxItem(s1, s1));
            ddImpositionList3.Items.Add(new Telerik.Web.UI.RadComboBoxItem(s1, s1));
	
			ddImpositionList.Items.Clear();

            foreach (string s in arr)
            {
                ddImpositionList.Items.Add(new Telerik.Web.UI.RadComboBoxItem(s, s));
                ddImpositionList2.Items.Add(new Telerik.Web.UI.RadComboBoxItem(s, s));
                ddImpositionList3.Items.Add(new Telerik.Web.UI.RadComboBoxItem(s, s));
            }
			//SelectPageFormatFromPublication();	
		}

    /*    private void SelectPageFormatFromPublication()
        {
            if (ddPublicationList.SelectedIndex < 0)
                return;

            if (Global.databaseVersion < 2)
            {
                DataTable table = (DataTable)Cache["PublicationPageFormatCache"];
                foreach (DataRow row in table.Rows)
                {
                    if ((string)row["Name"] == ddPublicationList.SelectedValue)
                    {
                        if ((int)row["ID"] > 0)
                        {
                            string s = Globals.GetNameFromID("PageFormatCache", (int)row["ID"]);
                            if (s != "")
                                ddPageFormatList.SelectedValue = s;
                            else
                                ddPageFormatList.SelectedIndex = 0;
                        }
                    }
                }
            }
            else
            {
                DataRow row = Globals.GetPublicationRow(ddPublicationList.SelectedValue);
                if (row == null)
                    return;

                if ((int)row["PageFormatID"] > 0)
                {
                    string s = Globals.GetNameFromID("PageFormatCache", (int)row["PageFormatID"]);
                    if (s != "")
                        ddPageFormatList.SelectedValue = s;
                    else
                        ddPageFormatList.SelectedIndex = 0;
                }
            }
        }
        */
		
		private  DateTime GetDeadlineFromPublication()
		{
			if (ddPublicationList.SelectedIndex < 0)
				return DateTime.MinValue;

			DataTable dt = (DataTable) HttpContext.Current.Cache["PublicationNameCache"];
			if (dt == null || dt.HasErrors)
				return DateTime.MinValue;

			if (dt.Columns.Contains("Deadline") == false)
				return DateTime.MinValue;

			foreach (DataRow row in dt.Rows)
			{
				if ((string)row["Name"] == ddPublicationList.SelectedValue)
					return (DateTime)row["Deadline"];
			}

			return DateTime.MinValue;
		}

		private void SetDeadlineInfo()
		{
			DateTime selectedDateTime = (DateTime)dateChooserPubDate.SelectedDate;
			DateTime pubdate = new DateTime(selectedDateTime.Year,selectedDateTime.Month,selectedDateTime.Day,0,0,0,0);

			DateTime defaultDeadline;
			DateTime deadlineFromPub = GetDeadlineFromPublication();
            
            lblDeadlineInfo.Text = "n/a";

			if (deadlineFromPub.Year > 1975)
			{
                try
                {
                    defaultDeadline = new DateTime(pubdate.Year, pubdate.Month, pubdate.Day, deadlineFromPub.Hour, deadlineFromPub.Minute, 0);
                    defaultDeadline = defaultDeadline.AddDays(-1 * (deadlineFromPub.Day - 1));

                    lblDeadlineInfo.Text = defaultDeadline.ToString("G");
                }
                catch
                {
                    Global.logging.WriteLog(string.Format("Error setting deadline info"));
                }
			} 
			else
				lblDeadlineInfo.Visible = false;

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

        private void SetApprovalFromPublication()
        {
            int publicationID = Globals.GetIDFromName("PublicationNameCache", ddPublicationList.SelectedValue);
            int n = Globals.GetPublicationApprovalRequired(publicationID);
            checkApprovalRequired.Checked = n > 0;
        }

        private void SetSectionsFromPublication()
        {
            int publicationID = Globals.GetIDFromName("PublicationNameCache", ddPublicationList.SelectedValue);


            if (Globals.HasAllowedSections(publicationID) == false)
            {
                lblImpositionSection2.Visible = true;
                ddImpositionList2.Visible = true;
                lblImpositionSection3.Visible = true;
                ddImpositionList3.Visible = true;

                return;
            }

            int sectionID2 = Globals.GetIDFromName("SectionNameCache", "B");
            if (sectionID2 <= 0)
               sectionID2 = Globals.GetIDFromName("SectionNameCache", "2");
            lblImpositionSection2.Visible = Globals.IsAllowedSection(publicationID, sectionID2) ;
            ddImpositionList2.Visible = Globals.IsAllowedSection(publicationID, sectionID2) ;;

            int sectionID3 = Globals.GetIDFromName("SectionNameCache", "C");
            if (sectionID3 <= 0)
                sectionID3 = Globals.GetIDFromName("SectionNameCache", "3");
            lblImpositionSection3.Visible = Globals.IsAllowedSection(publicationID, sectionID3);
            ddImpositionList3.Visible = Globals.IsAllowedSection(publicationID, sectionID3); ;
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

		private bool GotTemplateForPress(string press)
		{
			DataTable table = (DataTable) Cache["TemplatePressCache"];
			
			foreach (DataRow row in table.Rows)
			{
				if ((string)row["Press"] == press /*&& (int)row["Nup"] == 1*/)
				{
					return true;
				}
			}
	
			return false;

		}

		private string GetTemplateForPress(string press)
		{
			DataTable table = (DataTable) Cache["TemplatePressCache"];
			
			foreach (DataRow row in table.Rows)
			{
				if ((string)row["Press"] == press /*&& (int)row["Nup"] == 1*/)
				{
					return (string)row["Name"];
				}
			}
	
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

			btnAddPlan.Text = Global.rm.GetString("txtAddPlan");
			btnAddPlan.ToolTip = Global.rm.GetString("txtTooltipAddPlan");
			lblAddPagePlan.Text = Global.rm.GetString("txtAddPlan2");
			lblAddPagePlan.ToolTip = Global.rm.GetString("txtTooltipAddPlan2");
			lblPublication.Text = Global.rm.GetString("txtPublication");
            lblEdition.Text = Global.rm.GetString("txtEdition");

            lblPubdate.Text = Global.rm.GetString("txtPubDate2");
            lblApproval.Text = Global.rm.GetString("txtApproval"); // Global.rm.GetString("txtApprovalRequired");
			lblComment.Text = Global.rm.GetString("txtComment");
			btnSavePlan.Text = Global.rm.GetString("txtSavePlan");
			btnSavePlan.ToolTip = Global.rm.GetString("txtTooltipSavePlan");
            lblImpositionSection1.Text = Global.rm.GetString("txtImposition") + " " + Global.rm.GetString("txtSection") + " 1";
            lblImpositionSection2.Text = Global.rm.GetString("txtImposition") + " " + Global.rm.GetString("txtSection") + " 2";
            lblImpositionSection3.Text = Global.rm.GetString("txtImposition") + " " + Global.rm.GetString("txtSection") + " 3";
    		
			LblPlanUpdate.Text = Global.rm.GetString("txtPlanUpdateSettings"); 

			cbKeepColors.Text =  Global.rm.GetString("txtKeepExistingColors"); 
			cbKeepApproval.Text =  Global.rm.GetString("txtKeepExistingApproval"); 
			cbKeepUnique.Text =  Global.rm.GetString("txtKeepExistingUnique"); 

			btnCancel.Text =  Global.rm.GetString("txtCancel");	
	
			btlCloseDeletePlan.Text = Global.rm.GetString("txtClose");
	
			btnDeletePlan.Text = Global.rm.GetString("txtDeletePlan"); 
			lblDeletePlan.Text = Global.rm.GetString("txtDeletePlanText"); 
             

			lblCirculation.Text = Global.rm.GetString("txtCirculation");
			lblCirculation2.Text = Global.rm.GetString("txtCirculation2");

			lblWeekNumber.Text =  Global.rm.GetString("txtWeekNumber");
			lblWeekNumber2.Text = Global.rm.GetString("lblWeekNumber2");

			lblDeadline.Text =  Global.rm.GetString("txtDeadLine");

		}

		private void ButtonAddPlan_Click(object sender, System.EventArgs e)
		{
			// Reload caches for publication, edition and sections
			StartPlanInput(true);
		}

		private void StartPlanInput(bool addPlan)
		{
			lblInfo.Text = "";
			lblError.Text = "";

			lblAddPagePlan.Visible = false;
			btnAddPlan.Visible = false;
			btnDeletePlan.Visible = false;
			lblDeletePlan.Visible = false;
			
			PanelAddPlan.Visible = true;
            PanelMainActionButtons.Visible = false;

            Page.SetFocus(ddPublicationList.ID);
		
			RadNumericTextBoxCirculation.Value = 0;
			RadNumericTextBoxCirculation2.Value = 0;

        }

		private void ButtonCancel_Click(object sender, System.EventArgs e)
		{
			lblAddPagePlan.Visible = true;
			btnAddPlan.Visible = true;
			PanelAddPlan.Visible = false;
            PanelMainActionButtons.Visible = true;

			btnDeletePlan.Visible = (bool)Application["HideDeletePlanButton"] == false;
			lblDeletePlan.Visible = (bool)Application["HideDeletePlanButton"] == false;

			lblInfo.Text = "";
			lblError.Text = "";
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
			this.btnAddPlan.Click += new System.EventHandler(this.ButtonAddPlan_Click);
			this.btnDeletePlan.Click += new System.EventHandler(this.btnDeletePlan_Click);
			this.DataGridProductionList.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGridProduction_ItemCommand);
			this.DataGridProductionList.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGridProductionList_PageIndexChanged);
			this.DataGridProductionList.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DataGridProductionList_ItemDataBound);
			this.DataGridProductionList.SelectedIndexChanged += new System.EventHandler(this.DataGridProductionList_SelectedIndexChanged);
			this.btlCloseDeletePlan.Click += new System.EventHandler(this.btlCloseDeletePlan_Click);
			this.DataGridProductionListEdit.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGridProductionEdit_ItemCommand);
			this.DataGridProductionListEdit.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGridProductionListEdit_PageIndexChanged);
			this.DataGridProductionListEdit.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DataGridProductionListEdit_ItemDataBound);
			this.DataGridProductionListEdit.SelectedIndexChanged += new System.EventHandler(this.DataGridProductionListEdit_SelectedIndexChanged);
			this.btlCloseEditPlan.Click += new System.EventHandler(this.btlCloseEditPlan_Click);
			this.ddPublicationList.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(this.ddPublicationList_SelectedIndexChanged);
	           this.dateChooserPubDate.SelectedDateChanged += new Telerik.Web.UI.Calendar.SelectedDateChangedEventHandler(this.dateChooserPubDate_SelectedDateChanged);
			this.txtWeekNumber.TextChanged += new System.EventHandler(this.txtWeekNumber_TextChanged);

			this.btnSavePlan.Click += new System.EventHandler(this.btnSavePlan_Click);
			this.btnCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

       

		

		// -----------------------------------------------------
		// Pages-color checkbox grid 
		// -----------------------------------------------------

       // private string currentSection = "";
        //private int currentPages = 0;
        //private int currentOffset = 1;
     //   private string currentPrefix = "";
	//	private string currentPostfix = "";


		
		
		//--------------------------------------
		// Save plan to database
		//--------------------------------------

		private void btnSavePlan_Click(object sender, System.EventArgs e)
		{
//            if ((bool)Application["PlanUseXml"])
//                SavePagePlanXML(false);
//            else
    			SavePagePlan(false);

		}

     

        private void SavePagePlan(bool overwriteconfirmed)
        {
            CCDBaccess db = new CCDBaccess();
            string errmsg = "";
            lblInfo.Text = "";
            lblError.Text = "";

            int nProductionID = 0;
            bool bExistingProduction = false;
            bool bIsLockedProduction = false;
            ArrayList aSections = new ArrayList();
            ArrayList aEditions = new ArrayList();
            ArrayList aPages = new ArrayList();
            ArrayList aPageOffset = new ArrayList();
            ArrayList aPressRuns = new ArrayList();
            ArrayList aPressTemplates = new ArrayList();
         
            DateTime selectedDateTime = (DateTime)dateChooserPubDate.SelectedDate;
            DateTime pubdate = new DateTime(selectedDateTime.Year, selectedDateTime.Month, selectedDateTime.Day, 0, 0, 0, 0);

            int publicationID = Globals.GetIDFromName("PublicationNameCache", ddPublicationList.SelectedValue);
          
            //  aEditions = GetListOfEditionsFromGrid(ref aPresses);

            double latestHours = db.GetLatestPossibleUpdate(publicationID, out errmsg);
            if (latestHours > 0)
            {
                DateTime hotTime = pubdate.AddHours(-1.0 * latestHours);
                if (hotTime < DateTime.Now)
                {
                    lblError.Text = Global.rm.GetString("txtProductionLocked");
                    lblError.ForeColor = Color.Orange;
                    PanelAddPlan.Visible = false;
                    lblAddPagePlan.Visible = true;
                    btnAddPlan.Visible = true;
                    PanelMainActionButtons.Visible = true;
                    return;
                }
            }

            int weekNumber = (bool)Application["UseWeeknumberAsComment"] == false ? (int)txtWeekNumber.Value : 0;
            if ((bool)Application["HidePlanWeekNumber"])
                weekNumber = 0;

            errmsg = "";
            lblError.Text = "";
            int nSectionIndex = 0;
            int editionindex = 0;
            int totalPages = 0;

            if (ddEditionList.Text != "")
                aEditions.Add(ddEditionList.Text);
            else
                aEditions.Add(Globals.GetNameFromID("EditionNameCache", 1));

            aSections.Add(Globals.GetNameFromID("SectionNameCache", 1));
            aPressTemplates.Add(ddImpositionList.Text);
            if (ddImpositionList2.Visible && ddImpositionList2.Enabled && ddImpositionList2.Text != "Not used")
            {
                aSections.Add(Globals.GetNameFromID("SectionNameCache", 2));
                aPressTemplates.Add(ddImpositionList2.Text);
            }
            if (ddImpositionList3.Visible && ddImpositionList3.Enabled && ddImpositionList3.Text != "Not used") 
            {
                aSections.Add(Globals.GetNameFromID("SectionNameCache", 3));
                aPressTemplates.Add(ddImpositionList3.Text);
            }


            // Get all defaults!
            int pressID = Globals.GetIDFromName((bool)Application["UsePressGroups"] ? "PressGroupNameCache" : "PressNameCache", (string)Session["SelectedPress"]);
            
            // SAFETY!!!!!!!!!!!!!!
            int pressIDFromPressTemplate = db.GetPressTemplatePressID((string)aPressTemplates[0], out errmsg);
            if (pressIDFromPressTemplate > 0)
                pressID = pressIDFromPressTemplate;

            int pageFormatID = Globals.GetPublicationPageformatID(publicationID);
            int proofID = Globals.GetPublicationProofID(publicationID);
            int nPriority = 50;
            int paginationModeNotUsed = 0;
            bool onHold = true;
            bool insertedSectionsNotUsed = false;
            bool separateRunsNotUsed = false;
            int flatProofID = 0;
            int copies = 1;
            int templateID = 0;
            bool approvalRequired = (Globals.GetPublicationApprovalRequired(publicationID) == 1) ? true : false;
            bool forcePDF = false;
            string ripSetupString = "";
            bool autoApply = db.GetAutoApply(publicationID, pressID, ref paginationModeNotUsed, ref onHold, ref insertedSectionsNotUsed, 
                            ref separateRunsNotUsed, ref flatProofID, ref nPriority, ref templateID, ref copies, ref forcePDF, ref ripSetupString, out errmsg);

            if ((bool)Application["PlanAlwaysPDF"])
                forcePDF = true;

            int dummytemplateID = 0;
            if (templateID == 0)
                templateID = dummytemplateID;

            int ripSetupID = 0;
            int preflightSetupID = 0;
            int inksaveSetupID = 0;
            if (ripSetupString != "")
            {
                string[] srip = ripSetupString.Split(';');
                if (srip.Length > 0)
                    ripSetupID = Globals.GetIDFromName("RipSetupNamesCache", srip[0]);
                if (srip.Length > 1)
                    preflightSetupID = Globals.GetIDFromName("PreflightSetupNamesCache", srip[1]);
                if (srip.Length > 2)
                    inksaveSetupID = Globals.GetIDFromName("InksaveSetupNamesCache", srip[2]);
                ripSetupID += (preflightSetupID << 8) + (inksaveSetupID << 16);
            }

            int circulation = (int)RadNumericTextBoxCirculation.Value;
            int circulation2 = (int)RadNumericTextBoxCirculation2.Value;
            int timedEditionFrom = 0;
            int timedEditionTo = 0;
            int timedEditionSequence = 0;

            if ((bool)Application["PlanRunPreProcedure"])
                db.PreImport(publicationID, pubdate, pressID, 0, out errmsg);

            //aEditions = null; //GetListOfEditionsFromGridForPress(press);
            //aEditions.Add(ddEditionList.SelectedValue);


            bool cropInputPage = false;

            int planLockMode = (int)Application["PlanLockSystem"];
            bool publicationPlanLockMode = (int)Application["PublicationPlanLockSystem"] > 0 && (bool)Application["TableExists_PublicationLocks"];

            int bCurrentPlanLock = -1;

            string sClientName = "";
            DateTime tClientTime = DateTime.MinValue;

            if (planLockMode > 0)
            {
                if (db.PlanLock((string)Session["UserName"] + "-" + Request.UserHostAddress, 1, ref bCurrentPlanLock, ref sClientName, ref tClientTime, out errmsg) == true)
                {
                    if (bCurrentPlanLock == 0)
                    {
                        lblError.Text = "Planning lock is currently set by client " + sClientName + " at " + tClientTime.ToShortTimeString() + ". Please try again later.."; //Global.rm.GetString("txtProductionLocked");
                        lblError.ForeColor = Color.Orange;
                        return;
                    }
                }
            }

            int bCurrentPublicationPlanLock = -1;
            if (publicationPlanLockMode)
            {
                int nTimeOut = 10;

                while (bCurrentPublicationPlanLock == -1 && --nTimeOut >= 0)
                {
                    if (db.PublicationPlanLock(publicationID, pubdate, (string)Session["UserName"] + "-" + Request.UserHostAddress, 1, ref bCurrentPublicationPlanLock, ref sClientName, ref tClientTime, out errmsg) == true)
                    {
                        if (bCurrentPublicationPlanLock == 1)
                            break;
                        else
                            System.Threading.Thread.Sleep(1000);
                    }
                }
                if (bCurrentPublicationPlanLock == 0)
                {
                    lblError.Text = "Publication planning lock is currently set by client " + sClientName + " at " + tClientTime.ToShortTimeString() + ". Please try again later.."; //Global.rm.GetString("txtProductionLocked");
                    lblError.ForeColor = Color.Orange;
                    if (planLockMode > 0 && bCurrentPlanLock == 1)
                        db.PlanLock((string)Session["UserName"] + "-" + Request.UserHostAddress, 0, ref bCurrentPlanLock, ref sClientName, ref tClientTime, out errmsg);
                    return;
                }
            }

            //int totalPageCount = 0;

            // Accuire ProductionID

            if (db.CreatePagePlan(publicationID, pubdate, aEditions.Count,
                    pressID, templateID, proofID,
                    checkApprovalRequired.Checked,
                    nPriority,
                    aSections.Count, totalPages, out  bExistingProduction, out bIsLockedProduction, out  nProductionID, false, out  errmsg) == false)
            {
                lblError.Text = errmsg;
                lblError.ForeColor = Color.Red;
                if (planLockMode > 0 && bCurrentPlanLock == 1)
                    db.PlanLock((string)Session["UserName"] + "-" + Request.UserHostAddress, 0, ref bCurrentPlanLock, ref sClientName, ref tClientTime, out errmsg);
                if (publicationPlanLockMode && bCurrentPublicationPlanLock == 1)
                    db.PublicationPlanLock(publicationID, pubdate, (string)Session["UserName"] + "-" + Request.UserHostAddress, 0, ref bCurrentPublicationPlanLock, ref sClientName, ref tClientTime, out errmsg);
                return;
            }

            if (bIsLockedProduction)
            {
                lblError.Text = Global.rm.GetString("txtProductionLocked");
                lblError.ForeColor = Color.Orange;
                PanelAddPlan.Visible = false;
                lblAddPagePlan.Visible = true;
                btnAddPlan.Visible = true;
                PanelMainActionButtons.Visible = true;
                if (planLockMode > 0 && bCurrentPlanLock == 1)
                    db.PlanLock((string)Session["UserName"] + "-" + Request.UserHostAddress, 0, ref bCurrentPlanLock, ref sClientName, ref tClientTime, out errmsg);

                if (publicationPlanLockMode && bCurrentPublicationPlanLock == 1)
                    db.PublicationPlanLock(publicationID, pubdate, (string)Session["UserName"] + "-" + Request.UserHostAddress, 0, ref bCurrentPublicationPlanLock, ref sClientName, ref tClientTime, out errmsg);
                return;
            }

            if (bExistingProduction && overwriteconfirmed == false)
            {
                existingProductionPrompt = 1;
                saveConfirm.Value = "0";
                if (planLockMode > 0 && bCurrentPlanLock == 1)
                    db.PlanLock((string)Session["UserName"] + "-" + Request.UserHostAddress, 0, ref bCurrentPlanLock, ref sClientName, ref tClientTime, out errmsg);
                if (publicationPlanLockMode && bCurrentPublicationPlanLock == 1)
                    db.PublicationPlanLock(publicationID, pubdate, (string)Session["UserName"] + "-" + Request.UserHostAddress, 0, ref bCurrentPublicationPlanLock, ref sClientName, ref tClientTime, out errmsg);
                return;
            }

            if (bExistingProduction)
                db.SetProductionDirtyFlag(nProductionID, out  errmsg);

            string orderNumber = "";
            string productionOrderNumber = orderNumber;

            updateTree = 0;
            Session["RefreshTree"] = false;

            existingProductionPrompt = 0;
            existingProductionPrompt2 = 0;
            saveConfirm.Value = "0";


            string pressruncomment = txtComment.Text;
            DateTime defaultDeadline;
            DateTime deadlineFromPub = GetDeadlineFromPublication();
            if (deadlineFromPub.Year < 1975)
                defaultDeadline = new DateTime(1975, 1, 1, 0, 0, 0);
            else
            {
                defaultDeadline = new DateTime(pubdate.Year, pubdate.Month, pubdate.Day, deadlineFromPub.Hour, deadlineFromPub.Minute, 0);
                defaultDeadline = defaultDeadline.AddDays(-1 * (deadlineFromPub.Day - 1));
            }

            int copyseparationset = 0;
            int incopyseparationset = 0;
            int incopyflatseparationset = 0;
            int copyflatseparationset = 0;
            int pageindexglobal = 0;

            int presssectionnumberglobal = 0;
            int m_presssectionnumber = 0;
            
            foreach (string edition in aEditions)
            {
                int editionID = Globals.GetIDFromName("EditionNameCache", edition);
                int sectionIndex = 0;
                m_presssectionnumber++;
                foreach (string pressTemplate in aPressTemplates)
                {
                    List<PageTableEntry> plateList = new List<PageTableEntry>();
                    int numberOfRuns = 0;
                    db.LoadPressTemplate(pressTemplate, ref plateList, ref  numberOfRuns, out  errmsg);

                    ArrayList sectionsInTemplate = new ArrayList();
                    foreach(PageTableEntry item in plateList)
                    {
                        if (Globals.IsInArray(sectionsInTemplate, item.m_sectionID) == false)
                            sectionsInTemplate.Add(item.m_sectionID);
                    }

                    int nPages = 0;
                    int nOffset = 0;

                    if (sectionsInTemplate.Count == 1)
                    {
                        nPages = PagesInPlanSection(ref plateList, plateList[0].m_sectionID, ref nOffset);
                        aPageOffset.Add(nOffset);
                        aPages.Add(nPages);
                    }

                    int nPressRunID = 1;
                    errmsg = "";
                    bool combineSections = false;
                    string section = (string)aSections[sectionIndex];

                    int sectionID = Globals.GetIDFromName("SectionNameCache", section);

                    if (combineSections == false && sectionIndex > 0)
                    {
                        m_presssectionnumber++;
                        pageindexglobal = 0;
                    }
                    if (db.GetPressRunID(publicationID, pubdate, editionID, sectionID, pressID, combineSections, pressruncomment, orderNumber,
                              circulation, circulation2, out  nPressRunID, m_presssectionnumber, weekNumber,
                              timedEditionFrom, timedEditionTo, timedEditionSequence, pageFormatID, cropInputPage ? 1 : 0, out errmsg) == false)
                    {
                        lblError.Text = errmsg;
                        lblError.ForeColor = Color.Red;
                        if (planLockMode > 0 && bCurrentPlanLock == 1)
                            db.PlanLock((string)Session["UserName"] + "-" + Request.UserHostAddress, 0, ref bCurrentPlanLock, ref sClientName, ref tClientTime, out errmsg);
                        if (publicationPlanLockMode && bCurrentPublicationPlanLock == 1)
                            db.PublicationPlanLock(publicationID, pubdate, (string)Session["UserName"] + "-" + Request.UserHostAddress, 0, ref bCurrentPublicationPlanLock, ref sClientName, ref tClientTime, out errmsg);
                        return;
                    }

                    if (Globals.IsInArray(aPressRuns, nPressRunID) == false)
                       aPressRuns.Add(nPressRunID);

                    int nExistingPages = nPages;
                    if (bExistingProduction)
                    {
                        int nPg = 0;
                        if (db.GetPageCount(nPressRunID, out nPg, out errmsg))
                            nExistingPages = nPg;
                    }

                    int prevSheetNumber = -1;
                    int prevSheetSide = -1;
                    bool isFirstPagePosition = false;
                    int prevPressSectionNumber = 0;
                    foreach(PageTableEntry item in plateList)
                    {

                        // defaults for all separations..
                        isFirstPagePosition = false;
                        if (item.m_sheetnumber != prevSheetNumber || item.m_sheetside != prevSheetSide)
                        {
                            isFirstPagePosition = true;
                        }

                        if (item.m_presssectionnumber != prevPressSectionNumber)
                        {
                            presssectionnumberglobal++;
                        }

                        prevSheetNumber = item.m_sheetnumber;
                        prevSheetSide = item.m_sheetside;
                        prevPressSectionNumber = item.m_presssectionnumber;

                        item.m_presssectionnumber = m_presssectionnumber; //presssectionnumberglobal;

                        item.m_pagecountchange = nExistingPages != nPages;

                        item.m_ripSetupID = ripSetupID;
                        item.m_pageFormatID = pageFormatID;
                        item.m_deadline = defaultDeadline;
                       // item.m_templateID = templateID;
                        item.m_pressID = pressID;
                        item.m_productionID = nProductionID;
                        item.m_publicationID = publicationID;
                        item.m_issueID = 1;
                        item.m_proofID = proofID;
                        item.m_pubdate = pubdate;
                        item.m_weeknumber = weekNumber;
                        item.m_locationID = Globals.GetTypeFromName("PressNameCache", (string)Session["SelectedPress"]);	// Returns LocationID!				
                        item.m_version = 0;
                        item.m_layer = 1;
                        item.m_approved = approvalRequired ? 0 : 1; //checkApprovalRequired.Checked ? 0 : -1;

                        item.m_deviceID = 0;
                        item.m_hold = onHold;
                        item.m_priority = nPriority;

                        int numberOfcolors = ColorsOnPlate(ref plateList, item.m_presssectionnumber, item.m_sheetnumber, item.m_sheetside);                        

                        item.m_pressrunID = nPressRunID;
                        item.m_sectionID = sectionID;
                        item.m_editionID = Globals.GetIDFromName("EditionNameCache", edition);
                        
                        item.m_mastereditionID = 0; //Globals.GetIDFromName("EditionNameCache", edition);
                            // Globals.GetIDFromName("EditionNameCache", getMasterEditionForPage(edition, section, item.m_pagename));


                        item.m_customerID = Globals.GetPublicationCustomerID(publicationID);
                        item.m_issuesequencenumber = item.m_presssectionnumber;

                        item.m_pagesonplate = Globals.GetPagesOnPlateFromTemplate(item.m_templateID);

//                        item.m_uniquepage = (item.m_mastereditionID == item.m_editionID);
                        if (db.InsertSeparationExtended(item, incopyseparationset, out copyseparationset, incopyflatseparationset, out copyflatseparationset, numberOfcolors, isFirstPagePosition,
                            copies, true, true, true, out errmsg) == false)
                        {
                            lblError.Text = errmsg;
                            lblError.ForeColor = Color.Red;
                            if (planLockMode > 0 && bCurrentPlanLock == 1)
                                db.PlanLock((string)Session["UserName"] + "-" + Request.UserHostAddress, 0, ref bCurrentPlanLock, ref sClientName, ref tClientTime, out errmsg);
                            if (publicationPlanLockMode && bCurrentPublicationPlanLock == 1)
                                db.PublicationPlanLock(publicationID, pubdate, (string)Session["UserName"] + "-" + Request.UserHostAddress, 0, ref bCurrentPublicationPlanLock, ref sClientName, ref tClientTime, out errmsg);
                            return;
                        }

                        incopyseparationset = copyseparationset;
                        incopyflatseparationset = copyflatseparationset;

                    }
                    
                    // re-aquire lock!
                    if (planLockMode > 0 && bCurrentPlanLock == 1)
                        db.PlanLock((string)Session["UserName"] + "-" + Request.UserHostAddress, 1, ref bCurrentPlanLock, ref sClientName, ref tClientTime, out errmsg);

                    if (publicationPlanLockMode && bCurrentPublicationPlanLock == 1)
                        db.PublicationPlanLock(publicationID, pubdate, (string)Session["UserName"] + "-" + Request.UserHostAddress, 1, ref bCurrentPublicationPlanLock, ref sClientName, ref tClientTime, out errmsg);

                    sectionIndex++;
                }
                editionindex++;

              }

              db.UpdateProductionOrderNumber(nProductionID, productionOrderNumber, out errmsg);

            if ((int)Application["AutoRetryPlanFiles"] > 0)
                db.AddAutoRetryRequest(nProductionID, (int)Application["AutoRetryPlanFiles"], out errmsg);



            if ((bool)Application["PlanRunPostProcedure"])
              {
                foreach (int pressRunID in aPressRuns)
                {
                    if (db.PostImport(pressRunID,  out errmsg) == false)
                    {
                        lblError.Text = errmsg;
                        lblError.ForeColor = Color.Red;
                    }
                }
              }

              if ((bool)Application["PlanRunPostProcedure2"])
              {
                if (db.PostImportProduction(nProductionID, out errmsg) == false)
                {
                    lblError.Text = errmsg;
                    lblError.ForeColor = Color.Red;
                }
              }

              if ((bool)Application["LogPlanning"])
              {
                string editionString = "";
                if (aEditions.Count == 1)
                  editionString = (string)aEditions[0];
                else
                {
                  foreach (string edition in aEditions)
                  {
                    if (editionString != "")
                      editionString += "&";
                    editionString += edition;
                  }

                  editionString = "(" + editionString + ")";
                }
                string sectionString = "";
                string pageString = "";
                if (aSections.Count == 1)
                {
                  sectionString = (string)aSections[0];
                  pageString = (string)aSections[0] + " " + string.Format("{0} {1}-{2}", (string)aSections[0], (int)aPageOffset[0], (int)aPages[0] - (int)aPageOffset[0] - 1);
                }
                else
                {
                  int v = 0;
                  foreach (string section in aSections)
                  {
                    if (sectionString != "")
                      sectionString += "&";
                    sectionString += section;

                    if (pageString != "")
                      pageString += " , ";
                    pageString += string.Format("{0} {1}-{2}", (string)aSections[v], (int)aPageOffset[v], (int)aPages[v] - (int)aPageOffset[v] - 1);
                    v++;
                  }

                  sectionString = "(" + sectionString + ")";
                }

                db.UpdateMessage(-1, (string)Session["Username"], "",
                            string.Format("Plan created - {0} {1:00}-{2:00}", ddPublicationList.SelectedValue, pubdate.Day, pubdate.Month),
                            string.Format("Plan created : {0}-{1:00}{2:00}-{3}-{4}  {5}", ddPublicationList.SelectedValue, pubdate.Day, pubdate.Month, editionString, sectionString, pageString),
                            false, pubdate, Globals.GetIDFromName("PublicationNameCache", ddPublicationList.SelectedValue), out errmsg);

                string userName = (string)Session["UserName"];
                if ((string)Session["UserDomain"] != "")
                    userName = (string)Session["UserDomain"] + @"\" + (string)Session["UserName"];

                db.InsertUserHistory(userName, bExistingProduction ? 4 : 2, string.Format("{0}-{1:00}.{2:00}-{3}-{4}", ddPublicationList.SelectedValue, pubdate.Day, pubdate.Month, ddEditionList.SelectedValue, pageString), out errmsg);

                db.InsertLogEntry((int)Application["ProcessID"],
                     bExistingProduction ? (int)Globals.EventCodes.PlanEdit : (int)Globals.EventCodes.PlanCreate,
                                            "WebCenter " + userName,
                                            string.Format("{0}-{1:00}{2:00}-{3}", ddPublicationList.SelectedValue, pubdate.Day, pubdate.Month, editionString),
                                            pageString,
                                            nProductionID, out errmsg);
              }

              //	db.DeleteDirtyFlag(nProductionID, out errmsg);
              if (planLockMode > 0 && bCurrentPlanLock == 1)
              {
                if (planLockMode > 1)
                  db.DeleteAllDirtyPages(nProductionID, out errmsg);
                db.PlanLock((string)Session["UserName"] + "-" + Request.UserHostAddress, 0, ref bCurrentPlanLock, ref sClientName, ref tClientTime, out errmsg);
              }
              if (publicationPlanLockMode && bCurrentPublicationPlanLock == 1)
                db.PublicationPlanLock(publicationID, pubdate, (string)Session["UserName"] + "-" + Request.UserHostAddress, 0, ref bCurrentPublicationPlanLock, ref sClientName, ref tClientTime, out errmsg);

              lblError.Text = Global.rm.GetString("txtPlanAdded");

            lblError.ForeColor = Color.Green;
            updateTree = 1;
            Session["RefreshTree"] = true;

            lblAddPagePlan.Visible = true;
            btnAddPlan.Visible = true;
            PanelMainActionButtons.Visible = true;

            PanelAddPlan.Visible = false;

            btnDeletePlan.Visible = (bool)Application["HideDeletePlanButton"] == false;
            lblDeletePlan.Visible = (bool)Application["HideDeletePlanButton"] == false;
   
            btnAddPlan.Visible = (bool)Application["HideAddPlanButton"] == false;
            lblAddPagePlan.Visible = (bool)Application["HideAddPlanButton"] == false;


            Session["SubEditions"] = null;

            string s = string.Format("{0}-{1:00}.{2:00}-{3}-", ddPublicationList.SelectedValue, pubdate.Day, pubdate.Month, (string)aEditions[0]);
            for (int i = 0; i < aSections.Count; i++)
            {
                if (i > 0)
                    s += ",";
                s += string.Format("{0} {1} pages", aSections[i], aPages[i]);
            }
            db.InsertUserHistory((string)Session["UserName"], bExistingProduction ? 4 : 2, s, out errmsg);


        }


        private int PagesInPlanSection(ref List<PageTableEntry> plateList, int sectionID, ref int offset)
        {
            offset = 9999;
            ArrayList pagesInsectionTemplate = new ArrayList();
            foreach (PageTableEntry item in plateList)
            {
                if (Globals.IsInArray(pagesInsectionTemplate, item.m_pagename) == false)
                    pagesInsectionTemplate.Add(item.m_pagename);

                if (item.m_pageindex < offset + 1)
                    offset = item.m_pageindex - 1;
            }

            return pagesInsectionTemplate.Count;
        }


        private int SheetsInPlan(ref List<PageTableEntry> plateList)
        {
            ArrayList sheetNumbers = new ArrayList();

            foreach (PageTableEntry entry in plateList)
            {
                if (Globals.IsInArray(sheetNumbers, entry.m_sheetnumber) == false)
                    sheetNumbers.Add(entry.m_sheetnumber);
            }

            return sheetNumbers.Count;
        }

        private int SectionsInPlan(ref List<PageTableEntry> plateList)
        {
            ArrayList sections = new ArrayList();

            foreach (PageTableEntry entry in plateList)
            {
                if (Globals.IsInArray(sections, entry.m_presssectionnumber) == false)
                    sections.Add(entry.m_presssectionnumber);
            }

            return sections.Count;
        }

        private int RunsInPlan(ref List<PageTableEntry> plateList)
        {
            ArrayList runs = new ArrayList();

            foreach (PageTableEntry entry in plateList)
            {
                if (Globals.IsInArray(runs, entry.m_presssectionnumber) == false)
                    runs.Add(entry.m_presssectionnumber);
            }

            return runs.Count;
        }

        private int ColorsOnPlate(ref List<PageTableEntry> plateList, int presssectionnumber, int sheetNumber, int sheetSide)
        {
            ArrayList colors = new ArrayList();

            foreach (PageTableEntry entry in plateList)
            {
                if (entry.m_presssectionnumber == presssectionnumber && entry.m_sheetside == sheetSide && entry.m_sheetnumber == sheetNumber)
                    if (Globals.IsInArray(colors, entry.m_colorID) == false)
                        colors.Add(entry.m_colorID);
            }

            return colors.Count; 
        }

        private int CopiesOnPlate(ref List<PageTableEntry> plateList, int presssectionnumber, int sheetNumber, int sheetSide)
        {
            ArrayList copies = new ArrayList();

            foreach (PageTableEntry entry in plateList)
            {
                if (entry.m_presssectionnumber == presssectionnumber && entry.m_sheetside == sheetSide && entry.m_sheetnumber == sheetNumber)
                    if (Globals.IsInArray(copies, entry.m_copynumber) == false)
                        copies.Add(entry.m_copynumber);
            }

            return copies.Count;
        }

        private void SavePagePlanXML(bool overwriteconfirmed)
        {
            CCDBaccess db = new CCDBaccess();
            string errmsg = "";
            lblInfo.Text = "";
            lblError.Text = "";
            int pageID = 1;

            int nProductionID = 0;
            bool bExistingProduction = false;
            bool bIsLockedProduction = false;
            ArrayList aPresses = new ArrayList();
            ArrayList aSections = new ArrayList();
            ArrayList aEditions = new ArrayList();
            ArrayList aPages = new ArrayList();
            ArrayList aPageOffset = new ArrayList();
            ArrayList aPagePrefix = new ArrayList();
            ArrayList aPagePostfix = new ArrayList();
            ArrayList aColors = new ArrayList();
            ArrayList aPressRuns = new ArrayList();
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
                    PanelAddPlan.Visible = false;
                    lblAddPagePlan.Visible = true;
                    btnAddPlan.Visible = true;

                    return;
                }
            }
            aEditions = null;// GetListOfEditionsFromGrid(ref aPresses);

            int weekNumber = (bool)Application["UseWeeknumberAsComment"] == false ? (int)txtWeekNumber.Value : 0;
            if ((bool)Application["HidePlanWeekNumber"])
                weekNumber = 0;

            errmsg = "";
            lblError.Text = "";
            int nSectionIndex = 0;
            int editionindex = 0;
            int totalPages = 0;

            // Retrieve pagecount for each section

          /*  foreach (DataGridItem dataItem in DataGridSections.Items)
            {
                CheckBox cb = (CheckBox)dataItem.Cells[0].FindControl("CheckBoxUseSection");
                if (cb.Checked)
                {
                    try
                    {
                        string section = (string)dataItem.Cells[1].Text;
                        TextBox pages = (TextBox)dataItem.Cells[2].FindControl("txtNumberOfPages");
                        TextBox pageoffset = (TextBox)dataItem.Cells[3].FindControl("txtPageOffset");
                        //TextBox pageprefix = (TextBox)dataItem.Cells[4].FindControl("txtPagePrefix");
                        //TextBox pagepostfix = (TextBox)dataItem.Cells[5].FindControl("txtPagePostfix");
                        RadioButtonList colorSelect = (RadioButtonList)dataItem.Cells[6].FindControl("RadioButtonListColorMode");
                        int n = 1;
                        int offs = 1;
                        try
                        {
                            n = Int32.Parse(pages.Text);

                            if (n % 2 == 1 && overwriteconfirmed == false)
                            {
                                oddpagecountPrompt = 1;
                                saveConfirm.Value = "0";
                                return;
                            }

                            totalPages += n;
                            offs = Int32.Parse(pageoffset.Text);
                        }
                        catch (Exception e2)
                        {
                        }

                        if (n > 0)
                        {
                            aSections.Add((string)section);
                            aPages.Add((int)n);
                            aPageOffset.Add((int)offs);
                         //   aPagePrefix.Add((string)pageprefix.Text);
                         //   aPagePostfix.Add((string)pagepostfix.Text);
                            aColors.Add((string)colorSelect.SelectedValue != "" ? colorSelect.SelectedValue : "CMYK");
                        }
                        else
                        {
                            lblError.Text = "Pagecount 0 not allowed for section " + dataItem.Cells[1].Text;
                            lblError.ForeColor = Color.Red;
                            return;
                        }
                    }
                    catch (Exception e1)
                    { }

                    nSectionIndex++;
                }
            } */

            if (aSections.Count == 0)
            {
                lblError.Text = Global.rm.GetString("txtNoSectionsDefined");
                lblError.ForeColor = Color.Red;
                return;
            }

            if ((int)Session["MaxPlanPages"] > 0 && totalPages > (int)Session["MaxPlanPages"])
            {
                lblError.Text = Global.rm.GetString("txtMaxPlanPagesAllowed") + ": " + (int)Session["MaxPlanPages"];
                lblError.ForeColor = Color.Red;
                return;
            }

        
            // Start of plan object generation for XML generation. One XML per press..

            for (int pressNumber = 0; pressNumber < aPresses.Count; pressNumber++)
            {
                string press = (string)aPresses[pressNumber];
                int pressID = Globals.GetIDFromName((bool)Application["UsePressGroups"] ? "PressGroupNameCache" : "PressNameCache", press);
                if ((bool)Application["UsePressGroups"])
                    pressID = Globals.GetFirstPressIDFromPPressGroup(pressID);

                aEditions = null; // GetListOfEditionsFromGridForPress(press);

                int templateID = 0;
                bool forcePDF = false;


                Pageplan plan = new Pageplan();
                plan.publicationName = ddPublicationList.SelectedValue;
                plan.publicationDate = selectedDateTime;
                plan.planName = string.Format("{0:00}-{1:00}-{2:0000} {3} {4}", selectedDateTime.Day, selectedDateTime.Month, selectedDateTime.Year, ddPublicationList.SelectedValue, press);
                plan.planID = "1";
                plan.updatetime = DateTime.Now;
                plan.version = 1;
                plan.weekNumber = weekNumber;
                plan.defaultColors = "CMYK";
                plan.customername = "";
                plan.customeralias = "";
                plan.sender = "WebCenter";
                plan.sXmlFile = string.Format("WebCenter-{0:00}{1:00}{2:0000}-{3}-{4}.xml", selectedDateTime.Day, selectedDateTime.Month, selectedDateTime.Year, ddPublicationList.SelectedValue, press);
                int dummytemplateID = Globals.GetDummyTemplateID(press);
                if (templateID == 0)
                    templateID = dummytemplateID;

                int pageFormatID = 0;
               
                    pageFormatID = Globals.GetPublicationPageformatID(publicationID);

                int proofID = Globals.GetPublicationProofID(publicationID);
              
                updateTree = 0;
                Session["RefreshTree"] = false;

                existingProductionPrompt = 0;
                existingProductionPrompt2 = 0;
                saveConfirm.Value = "0";

                string pressruncomment = txtComment.Text;
                
                DateTime defaultDeadline;
                DateTime deadlineFromPub = GetDeadlineFromPublication();
                if (deadlineFromPub.Year < 1975)
                    defaultDeadline = new DateTime(1975, 1, 1, 0, 0, 0);
                else
                {
                    defaultDeadline = new DateTime(pubdate.Year, pubdate.Month, pubdate.Day, deadlineFromPub.Hour, deadlineFromPub.Minute, 0);
                    defaultDeadline = defaultDeadline.AddDays(-1 * (deadlineFromPub.Day - 1));
                }
                plan.deadline = defaultDeadline;

                int editionSeqNumber = 0;
                foreach (string edition in aEditions)
                {
                    PlanDataEdition edItem = new PlanDataEdition(edition);
                    edItem.editionSequenceNumber = ++editionSeqNumber;
                    edItem.editionCopy = 1;

                    //  int editionID = Globals.GetIDFromName("EditionNameCache", edition);
                    edItem.masterEdition = editionSeqNumber == 1;
                    PlanDataPress prs = new PlanDataPress(press);
                    prs.paperCopies = (int)RadNumericTextBoxCirculation.Value;
                    edItem.pressList.Add(prs);
                    plan.editionList.Add(edItem);
                    int sectionindex = 0;
                    int pageindexglobal = 0;

                    int circulation = (int)RadNumericTextBoxCirculation.Value;
                    int circulation2 = (int)RadNumericTextBoxCirculation2.Value;
                    int timedEditionFrom = 0;
                    int timedEditionTo = 0;
                    int timedEditionSequence = 0;

                    foreach (string section in aSections)
                    {
                        PlanDataSection secItem = new PlanDataSection(section);
                        edItem.sectionList.Add(secItem);

                        secItem.pagesInSection = 0;

                        int sectionID = Globals.GetIDFromName("SectionNameCache", section);
                        string colormode = (string)aColors[sectionindex];
                        int nPages = (int)aPages[sectionindex];
                        int nPageOffset = (int)aPageOffset[sectionindex];
                        errmsg = "";

                        // Get color info for section
                        PageSection pagesection = null;
                        switch (colormode)
                        {
                            case "Mono":
                                pagesection = GetMonoColorArray(nPages, nPageOffset);
                                break;
                            case "PDF":
                                pagesection = GetPDFColorArray(nPages, nPageOffset);
                                break;
                            default:
                                pagesection = GetCMYKColorArray(nPages, nPageOffset);
                                break;
                        }
                        if (forcePDF)
                            pagesection = GetPDFColorArray(nPages, nPageOffset);

                        secItem.pagesInSection = nPages;
                        for (int pageindex = 0; pageindex < nPages; pageindex++)
                        {
                            pageID++;
                            PlanDataPage pageItem = new PlanDataPage();
                            secItem.pageList.Add(pageItem);
                            pageItem.pageID = pageID.ToString();
                            pageItem.pageName = pagesection.aPageNameList[pageindex];
                            pageItem.pagination = pageindexglobal + 1;
                            pageItem.approve = checkApprovalRequired.Checked;
                            pageItem.comment = "";
                            pageItem.fileName = "";
                            pageItem.hold = false;
                            pageItem.masterEdition = ""; // getMasterEditionForPage(edition, section, pageItem.pageName);
                            pageItem.miscint = 0;
                            pageItem.miscstring1 = "";
                            pageItem.miscstring2 = "";
                            pageItem.pageIndex = pageindex + 1;
                            pageItem.pageType = 0;
                            pageItem.priority = 50;
                            pageItem.uniquePage = pageItem.masterEdition == edition ? PageUniqueType.Unique : PageUniqueType.Common;
                            pageItem.version = 0;
                            if (pageItem.uniquePage == PageUniqueType.Unique)
                                pageItem.masterPageID = pageItem.pageID;
                            else
                            {
                                PlanDataEdition masterEdition = plan.GetEditionObject(pageItem.masterEdition);
                                if (masterEdition != null)
                                {
                                    PlanDataSection masterSection = masterEdition.GetSectionObject(section);
                                    if (masterSection != null)
                                    {
                                        PlanDataPage masterPage = masterSection.GetPageObject(pageItem.pageIndex);
                                        if (masterPage != null)
                                            pageItem.masterPageID = masterPage.pageID;
                                    }
                                }
                            }


                            pageindexglobal++;


                            int nNumberOfColors = pagesection.GetNumberOfPageColors(pageindex);

                            for (int color = 0; color < nNumberOfColors; color++) 
                            {
                                bool bIsActive;
                                int colorID = pagesection.GetPageColorID(pageindex, color, out bIsActive);
                            }
                        } // for (pageindex...
                        sectionindex++;

                    } // for (section...
                    editionindex++;
                    // re-aquire lock!
                   
                } // for (edition...

                plan.GenerateXML(press, false, aPresses.Count == pressNumber + 1, true, out errmsg);    

                if ((bool)Application["LogPlanning"])
                {
                    string editionString = "";
                    if (aEditions.Count == 1)
                        editionString = (string)aEditions[0];
                    else
                    {

                        foreach (string edition in aEditions)
                        {
                            if (editionString != "")
                                editionString += "&";
                            editionString += edition;
                        }

                        editionString = "(" + editionString + ")";
                    }
                    string sectionString = "";
                    string pageString = "";
                    if (aSections.Count == 1)
                    {
                        sectionString = (string)aSections[0];
                        pageString = (string)aSections[0] + " " + string.Format("{0} {1}-{2}", (string)aSections[0], (int)aPageOffset[0], (int)aPages[0] - (int)aPageOffset[0] - 1);

                    }
                    else
                    {

                        int v = 0;
                        foreach (string section in aSections)
                        {
                            if (sectionString != "")
                                sectionString += "&";
                            sectionString += section;

                            if (pageString != "")
                                pageString += " , ";
                            pageString += string.Format("{0} {1}-{2}", (string)aSections[v], (int)aPageOffset[v], (int)aPages[v] - (int)aPageOffset[v] - 1);

                            v++;

                        }

                        sectionString = "(" + sectionString + ")";
                    }

                    db.UpdateMessage(-1, (string)Session["Username"], "",
                            string.Format("Plan created - {0} {1:00}-{2:00}", ddPublicationList.SelectedValue, pubdate.Day, pubdate.Month),
                            string.Format("Plan created : {0}-{1:00}{2:00}-{3}-{4}  {5}", ddPublicationList.SelectedValue, pubdate.Day, pubdate.Month, editionString, sectionString, pageString),
                            false, pubdate, Globals.GetIDFromName("PublicationNameCache", ddPublicationList.SelectedValue), out errmsg);

                    db.InsertLogEntry((int)Application["ProcessID"],
                                            bExistingProduction ? (int)Globals.EventCodes.PlanEdit : (int)Globals.EventCodes.PlanCreate,
                                            (string)Session["UserName"],
                                            string.Format("{0}-{1:00}{2:00}-{3}-{4}", ddPublicationList.SelectedValue, pubdate.Day, pubdate.Month, editionString, sectionString),
                                            pageString,
                                            nProductionID, out errmsg);
                }


            } // for presses

            lblError.Text = Global.rm.GetString("txtPlanAdded");

            lblError.ForeColor = Color.Green;
            updateTree = 1;
            Session["RefreshTree"] = true;

            lblAddPagePlan.Visible = true;
            btnAddPlan.Visible = true;
            PanelMainActionButtons.Visible = true;

            PanelAddPlan.Visible = false;

            btnDeletePlan.Visible = (bool)Application["HideDeletePlanButton"] == false;
            lblDeletePlan.Visible = (bool)Application["HideDeletePlanButton"] == false;
           
            btnAddPlan.Visible = (bool)Application["HideAddPlanButton"] == false;
            lblAddPagePlan.Visible = (bool)Application["HideAddPlanButton"] == false;


            Session["SubEditions"] = null;

            string s = string.Format("{0}-{1:00}.{2:00}-{3}-", ddPublicationList.SelectedValue, pubdate.Day, pubdate.Month, (string)aEditions[0]);
            for (int i = 0; i < aSections.Count; i++)
            {
                if (i > 0)
                    s += ",";
                s += string.Format("{0} {1} pages", aSections[i], aPages[i]);
            }
            db.InsertUserHistory((string)Session["UserName"], bExistingProduction ? 4 : 2, s, out errmsg);


        }
		


		//private PageSection GetCMYKColorArray(int nNumberOfPages, int offset, string prefix, string postfix) 
        private PageSection GetCMYKColorArray(int nNumberOfPages, int offset) 
		{
			PageSection  pagesection = new PageSection(nNumberOfPages);
			
			int[] cols = new int[4];
			bool[] active = new bool[4];
			cols[0] = Globals.GetIDFromName("ColorNameCache","C");
			cols[1] = Globals.GetIDFromName("ColorNameCache","M");
			cols[2] = Globals.GetIDFromName("ColorNameCache","Y");
			cols[3] = Globals.GetIDFromName("ColorNameCache","K");
			for (int i=0; i<4; i++)
				active[i] = true;

			for (int i=0; i<nNumberOfPages; i++)
			{
				int n = i+offset;
				//pagesection.SetPageName(i, prefix + n.ToString() + postfix);
                pagesection.SetPageName(i, n.ToString());
				pagesection.SetColors(i, 4, cols, active);

			}
			return pagesection;
		}

	//	private PageSection GetPDFColorArray(int nNumberOfPages, int offset, string prefix, string postfix) 
        private PageSection GetPDFColorArray(int nNumberOfPages, int offset) 
		{
			PageSection  pagesection = new PageSection(nNumberOfPages);
			
			int[] cols = new int[1];
			bool[] active = new bool[1];
			cols[0] = Globals.GetIDFromName("ColorNameCache","PDF");
			active[0] = true;
			for (int i=0; i<nNumberOfPages; i++)
			{
				int n = i+offset;
				//pagesection.SetPageName(i, prefix + n.ToString() + postfix);
                pagesection.SetPageName(i, n.ToString());
				pagesection.SetColors(i, 1, cols, active);

			}
			return pagesection;
		}

		
//		private PageSection GetMonoColorArray(int nNumberOfPages, int offset, string prefix, string postfix) 
           private PageSection GetMonoColorArray(int nNumberOfPages, int offset) 
		{
			PageSection  pagesection = new PageSection(nNumberOfPages);
			
			int m = 1;
			if ((bool)Session["AlwaysPlanCMYK"]) // mono
				m = 4;

			int[] cols = new int[m];
			bool[] active = new bool[m];
			
			if (m == 4) 
			{
				cols[0] = Globals.GetIDFromName("ColorNameCache","C");
				active[0] = false;
				cols[1] = Globals.GetIDFromName("ColorNameCache","M");
				active[1] = false;
				cols[2] = Globals.GetIDFromName("ColorNameCache","Y");
				active[2] = false;
				cols[3] = Globals.GetIDFromName("ColorNameCache","K");
				active[3] = true;
			} 
			else
			{
				cols[0] = Globals.GetIDFromName("ColorNameCache","K");
				active[0] = true;
			}
			for (int i=0; i<nNumberOfPages; i++)
			{
				int n = i+offset;
				//pagesection.SetPageName(i, prefix + n.ToString() + postfix);
                pagesection.SetPageName(i, n.ToString());
				pagesection.SetColors(i, m, cols, active);
			}
			return pagesection;
		}

		protected void ddPublicationList_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
		{
			SelectPressFromPublication();
            SetEditionsFromPublication();
            SetSectionsFromPublication();
            SetApprovalFromPublication();

            SetDeadlineInfo();
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

		

        private string SectionsDefinedInEditionSession()
        {
            string s = "";
            if (Session["SubEditions"] == null)
                return "";

            DataSet dsEditions = (DataSet)Session["SubEditions"];
            DataTable dt = dsEditions.Tables[0];

            int npages = 0;
            string thisSection = "";
            foreach (DataRow row in dt.Rows)
			{
				if ((string)row["Section"] != thisSection)
                {
                    if (thisSection != "")
                    {
                        if (s != "")
                            s += ",";
                        s += npages.ToString();
                        npages = 0;
                    }
                    thisSection = (string)row["Section"];

                }
                npages++;
            }
            if (npages > 0)
            {
                if (s != "")
                    s += ",";
                s += npages.ToString();
            }

            return s;
        }

  

		private void btnDeletePlan_Click(object sender, System.EventArgs e)
		{
			lblError.Text = "";

		
			PanelDeletePlan.Visible = true;
			lblAddPagePlan.Visible = false;
			btnAddPlan.Visible = false;
			btnDeletePlan.Visible = false;
			lblDeletePlan.Visible = false;

            PanelMainActionButtons.Visible = false;

			Session["SubEditions"] = null;

			PlanListDataBind(1);
		}

		private void PlanListDataBind(int view)
		{
			lblInfo.Text = "";
			lblError.Text = "";
			string errmsg = "";			
			CCDBaccess	db = new CCDBaccess();
			DataTable dbTable = db.GetProductionList(true, out errmsg);
			if (errmsg != "")
			{
				lblError.Text = errmsg;
				return;
			}

			if (dbTable.Rows.Count == 0)
			{
				lblInfo.Text = "No products available";
				return;

			}

			if (view == 2)
			{
				DataGridProductionListEdit.DataSource = dbTable;
				DataGridProductionListEdit.DataBind();
			}
			else
			{
				DataGridProductionList.DataSource = dbTable;
				DataGridProductionList.DataBind();
			}
		}

		private void btlCloseDeletePlan_Click(object sender, System.EventArgs e)
		{
			lblInfo.Text = "";
			lblError.Text = "";

			PanelDeletePlan.Visible = false;
		

			lblAddPagePlan.Visible = true;
			btnAddPlan.Visible = true;
			PanelAddPlan.Visible = false;
            PanelMainActionButtons.Visible = true;
			
			btnDeletePlan.Visible = true;
			lblDeletePlan.Visible = true;

      
		}



		private void DataGridProduction_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (e.CommandName == "Delete")
			{
				string sID = e.Item.Cells[2].Text;
				int productionID = Globals.TryParse(sID, 0);
				if (productionID > 0)
				{
					string errmsg = "";			
					lblInfo.Text = "";
					lblError.Text = "";

					CCDBaccess	db = new CCDBaccess();

					int pages = 0;
					if (db.GetPagesArrived(productionID, out pages, out errmsg) == false)
					{
						lblError.Text = errmsg;
						return;
					}
					int publicationID = 0;
					DateTime pubDate = DateTime.MinValue;
					if (db.GetPublicationFromProduction(productionID, out publicationID, out pubDate, out errmsg) == false)
					{
						lblError.Text = errmsg;
						return;
					}
					db.UpdateMessage(-1,(string)Session["Username"], "", string.Format("Plan deleted - {0} {1:00}-{2:00}",Globals.GetNameFromID("PublicationNameCache", publicationID),pubDate.Day,pubDate.Month), string.Format("Plan deleted - {0} {1:00}-{2:00}",Globals.GetNameFromID("PublicationNameCache", publicationID),pubDate.Day,pubDate.Month),false,pubDate,publicationID, out errmsg);

					if (pages > 0 && publicationID > 0 && pubDate != DateTime.MinValue)
						db.DeleteProductionPages(productionID, publicationID, pubDate, out errmsg);
					else
						db.DeleteProduction(productionID, out errmsg);
										
					PlanListDataBind(1);
					updateTree = 1;
					Session["RefreshTree"] = true;

					db.InsertUserHistory((string)Session["UserName"],3, e.Item.Cells[1].Text,out errmsg);
				}
			}
		}

		private void DataGridProductionList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			DataGridProductionList.EditItemIndex = -1;
		}

		private void DataGridProductionList_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			DataGridProductionList.SelectedIndex = -1;
			DataGridProductionList.EditItemIndex = -1;
			DataGridProductionList.CurrentPageIndex = e.NewPageIndex;
			PlanListDataBind(1);
		}

		private void DataGridProductionList_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Header)
			{
				e.Item.Cells[1].Text = Global.rm.GetString("txtProduct");
			}
		}

		private void btnEditPlan_Click(object sender, System.EventArgs e)
		{
			Session["SubEditions"] = null;
			lblInfo.Text = "";
			lblError.Text = "";

		
			PanelDeletePlan.Visible = false;
			lblAddPagePlan.Visible = false;
			btnAddPlan.Visible = false;
			btnDeletePlan.Visible = false;
			lblDeletePlan.Visible = false;
            PanelMainActionButtons.Visible = false;



			PlanListDataBind(2);
		}

		private void btlCloseEditPlan_Click(object sender, System.EventArgs e)
		{
			lblInfo.Text = "";
			lblError.Text = "";

			PanelDeletePlan.Visible = false;
		

			lblAddPagePlan.Visible = true;
			btnAddPlan.Visible = true;
			PanelAddPlan.Visible = false;
            PanelMainActionButtons.Visible = true;
			
			btnDeletePlan.Visible = (bool)Application["HideDeletePlanButton"] == false;
			lblDeletePlan.Visible = (bool)Application["HideDeletePlanButton"] == false;

			
		}

        private void DataGridProductionEdit_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            if (e.CommandName != "Edit")
                return;
/*
            string sID = e.Item.Cells[2].Text;
            int nID = Globals.TryParse(sID, 0);
            if (nID == 0)
                return;

            btlCloseEditPlan_Click(null, null);

            // Recover plan data from database
            lblError.Text = "";
            string errmsg = "";
            CCDBaccess db = new CCDBaccess();

            ArrayList allOrders = new ArrayList();
            if (db.GetJobDetailsForEdit(nID, ref allOrders, out errmsg) == false)
            {
                lblError.Text = errmsg;
                return;
            }

            // Enable plan entry panel

            StartPlanInput(false);

            // Set section details in data table
            string mainEdition = "";
            string subedition = "";
            string miscString3 = "";

            // Construct list of editions.
            DataSet ds = new DataSet();
            DataTable dt = new DataTable("Editions");
            DataColumn newColumn;
            newColumn = dt.Columns.Add("Section", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("Page", Type.GetType("System.String"));


            // Insert main edition

            //public bool GetEditionUniqueFlags(ref ArrayList arrUniqueList, int pressRunID, int editionID, int sectionID, out string errmsg)

            foreach (DataGridItem dataItem in DataGridSections.Items)
            {
                CheckBox cb = (CheckBox)dataItem.Cells[0].FindControl("CheckBoxUseSection");
                if (cb.Checked)
                {
                    try
                    {
                        string section = (string)dataItem.Cells[1].Text;
                        TextBox pages = (TextBox)dataItem.Cells[2].FindControl("txtNumberOfPages");
                        TextBox pageoffset = (TextBox)dataItem.Cells[3].FindControl("txtPageOffset");
                        int noffset = 0;
                        int npages = 0;
                        try
                        {
                            noffset = Int32.Parse(pageoffset.Text);
                            npages = Int32.Parse(pages.Text);
                        }
                        catch { }

                        for (int i = noffset; i < npages + noffset; i++)
                        {
                            DataRow newRow = dt.NewRow();
                            newRow["Section"] = section;
                            newRow["Page"] = i.ToString();
                            newRow[ddEditionList.SelectedValue] = ddEditionList.SelectedValue;
                            dt.Rows.Add(newRow);
                        }
                    }
                    catch
                    {
                    }
                }
            }

            DataTable dt2 = new DataTable("EditionInfo");
            DataColumn newColumn2;
            newColumn2 = dt2.Columns.Add("Edition", Type.GetType("System.String"));
            newColumn2 = dt2.Columns.Add("OrderNumber", Type.GetType("System.String"));
            newColumn2 = dt2.Columns.Add("Comment", Type.GetType("System.String"));
            newColumn2 = dt2.Columns.Add("Circulation", Type.GetType("System.Int32"));
            newColumn2 = dt2.Columns.Add("Circulation2", Type.GetType("System.Int32"));
            newColumn2 = dt2.Columns.Add("TimedFrom", Type.GetType("System.Int32"));
            newColumn2 = dt2.Columns.Add("TimedTo", Type.GetType("System.Int32"));
            newColumn2 = dt2.Columns.Add("EditionSequence", Type.GetType("System.Int32"));

            DataRow newRow2 = dt2.NewRow();
            newRow2["Edition"] = ddEditionList.SelectedValue;
            newRow2["OrderNumber"] = "";
            newRow2["Comment"] = txtComment.Text;
            newRow2["Circulation"] = (int)RadNumericTextBoxCirculation.Value;
            newRow2["Circulation2"] = (int)RadNumericTextBoxCirculation2.Value;
            newRow2["TimedFrom"] = 0;
            newRow2["TimedTo"] = 0;
            newRow2["EditionSequence"] = 0;

            dt2.Rows.Add(newRow2);

            ArrayList editionPageList = new ArrayList();

            for (int i = 3; i < dt.Columns.Count; i++)
            {
                //int sectionID = Globals.GetNameFromID("SectionNameCache", (string)newRow["Section"]);
                int editionID = Globals.GetIDFromName("EditionNameCache", dt.Columns[i].Caption);
                if (editionID > 0)
                {

                    newRow2 = dt2.NewRow();
                    newRow2["Edition"] = dt.Columns[i].Caption;
                    newRow2["OrderNumber"] = "";
                    newRow2["Circulation"] = (int)RadNumericTextBoxCirculation.Value;
                    newRow2["Circulation2"] = (int)RadNumericTextBoxCirculation2.Value;
                    newRow2["Comment"] = txtComment.Text;
                    newRow2["TimedFrom"] = 0;
                    newRow2["TimedTo"] = 0;
                    newRow2["EditionSequence"] = 0;
                    dt2.Rows.Add(newRow2);


                    foreach (OrderEntry order in allOrders)
                    {
                        if (order.m_editionID == editionID)
                        {
                            newRow2["TimedFrom"] = order.m_timedEditionFrom;
                            newRow2["TimedTo"] = order.m_timedEditionTo;
                            newRow2["EditionSequence"] = order.m_timedEditionSequence;
                            break;
                        }
                    }

                    db.GetEditionJobDetailsForEdit(Globals.GetIDFromName("PublicationNameCache", ddPublicationList.SelectedValue), 
                        (DateTime)dateChooserPubDate.SelectedDate, editionID, 0 , ref editionPageList, out errmsg);

                    if (editionPageList.Count > 0)
                    {
                        int j = 0;
                        foreach (DataRow row in dt.Rows)
                        {
                            if (editionPageList.Count > j)
                                row[i] = editionPageList[j];
                            j++;
                        }
                    }
                }
            }

            ds.Tables.Add(dt);
            ds.Tables.Add(dt2);

            Session["SubEditions"] = ds;

            updateTree = 0;
            Session["RefreshTree"] = true;

            PopulaterSubeditionDataGrid();
    */
        }


		private void DataGridProductionListEdit_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			DataGridProductionListEdit.SelectedIndex = -1;
			DataGridProductionListEdit.EditItemIndex = -1;
			DataGridProductionListEdit.CurrentPageIndex = e.NewPageIndex;
			PlanListDataBind(2);
		}

		private void DataGridProductionListEdit_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			DataGridProductionListEdit.EditItemIndex = -1;
		}

		private void DataGridProductionListEdit_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Header)
			{
				e.Item.Cells[1].Text = Global.rm.GetString("txtProduct");
			}
		}



		public static int GetWeekNumber(DateTime dtPassed)
		{
			CultureInfo ciCurr = CultureInfo.CurrentCulture;
			int weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
			return weekNum;
		}

		public DateTime GetLastOccurenceOfDay(DateTime value, DayOfWeek dayOfWeek)
		{
			int daysToAdd = dayOfWeek - value.DayOfWeek;
			if(daysToAdd < 1) 
			{
				daysToAdd -= 7;
			} 
			return value.AddDays(daysToAdd);
		}

		public DateTime GetFirstDayOfWeek(int year, int weekNumber, DayOfWeek dayOfWeek)
		{
			return GetLastOccurenceOfDay(new DateTime(year,1,1).AddDays(7*weekNumber), dayOfWeek);
		}

		public DateTime GetFirstDayOfWeek(int weekNumber)
		{
			return GetFirstDayOfWeek(DateTime.Today.Year,weekNumber, DayOfWeek.Monday);
		}

		private void txtWeekNumber_TextChanged(object sender, System.EventArgs e)
		{
			if (txtWeekNumber.Value >= 1 && txtWeekNumber.Value <= 52 && (bool)Application["UseWeeknumberAsComment"] == false)
			{
				DateTime temp = GetFirstDayOfWeek((int)txtWeekNumber.Value); 
				dateChooserPubDate.SelectedDate  = GetFirstDayOfWeek((int)txtWeekNumber.Value); 
			}
		}

        private void dateChooserPubDate_SelectedDateChanged(object sender, Telerik.Web.UI.Calendar.SelectedDateChangedEventArgs e)
		{
			txtWeekNumber.Value = 0;
			SetDeadlineInfo();
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



	}
}
