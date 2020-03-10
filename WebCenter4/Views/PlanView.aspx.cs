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
using WebCenter4.Classes;
using System.Text;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Configuration;
using Telerik.Web.UI;

namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for PlanView.
	/// </summary>
	public partial class PlanView : System.Web.UI.Page
	{
      
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

            hasOnlyOnePerssAndEdition = false;
            hasOnlyOneSection = false;
			
            updateTree = 0;

			Session["RefreshTree"] = false;

			btnAddPlan.Attributes.Add("onClick", "document.getElementById('ProgressBar').style.display = '';");
            btnSpecialEditions.Attributes.Add("onClick", "document.getElementById('ProgressBar').style.display = 'none';");
			btnSavePlan.Attributes.Add("onClick", "document.getElementById('ProgressBar').style.display = '';");

			if (!this.IsPostBack) 
			{
                dateChooserPubDate.Culture = CultureInfo.CurrentCulture;
                dateChooserPubDate.AutoPostBack = (bool)Application["PlanningCustomPressSelection"];

                Globals.ForceCacheReloadsSmall();

				Session["RefreshTree"] = false;
                Session["SubEditions"] = null;

				lblWeekNumber.Visible = (bool)Application["HidePlanWeekNumber"] == false;
				lblWeekNumber2.Visible = (bool)Application["HidePlanWeekNumber"] == false;
				txtWeekNumber.Visible = (bool)Application["HidePlanWeekNumber"] == false;

				txtWeekNumber.Value = 0;

				btnDeletePlan.Visible = (bool)Application["HideDeletePlanButton"] == false;
				lblDeletePlan.Visible = (bool)Application["HideDeletePlanButton"] == false;

			//	btnEditPlan.Visible = (bool)Application["HideEditPlanButton"] == false;
			//	lblEditPlan.Visible = (bool)Application["HideEditPlanButton"] == false;

                btnEditPlan.Visible = false;
                lblEditPlan.Visible = false;

                btnAddPlan.Visible = (bool)Application["HideAddPlanButton"] == false;
                lblAddPagePlan.Visible = (bool)Application["HideAddPlanButton"] == false;

                btnUploadPlan.Visible = (bool)Application["HideUploadPlanButton"] == false;
                lblUploadPlan.Visible = (bool)Application["HideUploadPlanButton"] == false;

                PanelMainActionButtons.Visible = true;

                LinkButtonAddNewPublication.Visible = false;
				LinkButtonAddNewPageformat.Visible = false;

				lblWeekNumber.Visible = (bool)Application["HidePlanWeekNumber"] == false;
				txtWeekNumber.Visible = (bool)Application["HidePlanWeekNumber"] == false;
				lblWeekNumber2.Visible = (bool)Application["HidePlanWeekNumber"] == false;

				lblCirculation.Visible = (bool)Application["HidePlanCirculation"] == false;
				RadNumericTextBoxCirculation.Visible = (bool)Application["HidePlanCirculation"] == false;

				lblCirculation2.Visible = (bool)Application["HidePlanCirculation2"] == false;
				RadNumericTextBoxCirculation2.Visible = (bool)Application["HidePlanCirculation2"] == false;

				CheckBoxCombineSections.Visible = (bool)Application["HidePlanCombineSections"] == false;

				PanelDeletePlan.Visible = false;
				PanelEditPlan.Visible = false;

				ddPageFormatList.Visible = (Globals.GetCacheRowCount("PageFormatCache") > 0) &&  ((bool)Application["HidePlanPageFormat"] == false);
				ddPageFormatList.Enabled = true;
				lblPlanPageFormat.Visible = ddPageFormatList.Visible;
				lblPlanPageFormat.Enabled = true;

				cbKeepColors.Checked = (bool)Session["KeepExistingColors"];
				cbKeepUnique.Checked = (bool)Session["KeepExistingUnique"];
				cbKeepApproval.Checked = (bool)Session["KeepExistingApproval"];

				lblInfo.Text = "";
				lblError.Text = "";

				PanelAddPlan.Visible = false;

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

				checkApprovalRequired.Checked = true;
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
				
				CreateSectionGrid();
                CreateEditionPressMatrixGrid();
				CreatePageFormatDropDown();

				// Final adjustment of press....
				SelectPressFromPublication();

                if ((string)Application["PlanPreferredEdition"] != "")
                    RestrictEditionsInGridForPress((string)Session["SelectedPlanPress"], (string)Application["PlanPreferredEdition"]);


                ddPublicationList.Enabled = true;

			}

			if (HiddenNewPubname.Value != "0") 
			{
				CreatePageFormatDropDown();
				CreatePublicationDropDown();

				if (HasRadItemByValue(ddPublicationList.Items, HiddenNewPubname.Value)) 
					ddPublicationList.SelectedValue = HiddenNewPubname.Value;
				SelectPageFormatFromPublication();	
				
				SetDeadlineInfo();
			}

			if (HiddenNewPageformat.Value != "0")
			{
				CreatePageFormatDropDown();
				if (HasRadItemByValue(ddPageFormatList.Items, HiddenNewPageformat.Value)) 
					ddPageFormatList.SelectedValue = HiddenNewPageformat.Value;
			}

            foreach (Telerik.Web.UI.RadWindow win in RadWindowManager1.Windows)
                win.VisibleOnPageLoad = false;


			HiddenNewPubname.Value = "0";
			HiddenNewPageformat.Value = "0";

			if (saveConfirm.Value == "1")
				SavePagePlan(true);            
		}



		private void CreatePageFormatDropDown()
		{
			DataTable table = (DataTable) Cache["PageFormatCache"];
	
			ddPageFormatList.Items.Clear();

			foreach (DataRow row in table.Rows)
			{
				ddPageFormatList.Items.Add(new Telerik.Web.UI.RadComboBoxItem((string)row["Name"],(string)row["Name"] ));
			}

			SelectPageFormatFromPublication();	
		}




        private void SelectPageFormatFromPublication()
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
			lblPubdate.Text = Global.rm.GetString("txtPubDate2");
            lblApproval.Text = Global.rm.GetString("txtApproval"); // Global.rm.GetString("txtApprovalRequired");
			lblSections.Text = Global.rm.GetString("txtSections");
			lblComment.Text = Global.rm.GetString("txtComment");
			CheckBoxCombineSections.Text = Global.rm.GetString("txtCombineSections");
			btnSavePlan.Text = Global.rm.GetString("txtSavePlan");
			btnSavePlan.ToolTip = Global.rm.GetString("txtTooltipSavePlan");
			lblPlanPageFormat.Text = Global.rm.GetString("txtPageFormat"); 
			LinkButtonAddNewPublication.Text = Global.rm.GetString("txtAddNew"); 
			LinkButtonAddNewPageformat.Text = Global.rm.GetString("txtAddNew"); 
			LblPlanUpdate.Text = Global.rm.GetString("txtPlanUpdateSettings"); 

			cbKeepColors.Text =  Global.rm.GetString("txtKeepExistingColors"); 
			cbKeepApproval.Text =  Global.rm.GetString("txtKeepExistingApproval"); 
			cbKeepUnique.Text =  Global.rm.GetString("txtKeepExistingUnique"); 
            
            btnSpecialEditions.Text = Global.rm.GetString("txtSpecialEditionPages"); 

			btnCancel.Text =  Global.rm.GetString("txtCancel");	
	
			btlCloseDeletePlan.Text = Global.rm.GetString("txtClose");
            btlCloseEditPlan.Text = Global.rm.GetString("txtClose");
	
			btnDeletePlan.Text = Global.rm.GetString("txtDeletePlan"); 
			lblDeletePlan.Text = Global.rm.GetString("txtDeletePlanText"); 
             
			btnEditPlan.Text = Global.rm.GetString("txtEditPlan"); 
			lblEditPlan.Text = Global.rm.GetString("txtEditPlanText"); 

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
			
			btnEditPlan.Visible = false;
			lblEditPlan.Visible = false;

			PanelAddPlan.Visible = true;
            PanelMainActionButtons.Visible = false;

            Page.SetFocus(ddPublicationList.ID);

			ddPageFormatList.Visible = (Globals.GetCacheRowCount("PageFormatCache") > 0) &&  ((bool)Application["HidePlanPageFormat"] == false);
			ddPageFormatList.Enabled = true;
			lblPlanPageFormat.Visible = ddPageFormatList.Visible;
			lblPlanPageFormat.Enabled = true;


			//DataGridPlanSections.Visible = false;
			DataGridSections.Visible = true;
			CheckBoxCombineSections.Visible = (bool)Application["HidePlanCombineSections"] == false;

			RadNumericTextBoxCirculation.Value = 0;
			RadNumericTextBoxCirculation2.Value = 0;

			Session["SubEditions"] = null;

        }

		private void ButtonCancel_Click(object sender, System.EventArgs e)
		{
			lblAddPagePlan.Visible = true;
			btnAddPlan.Visible = true;
			PanelAddPlan.Visible = false;
            PanelMainActionButtons.Visible = true;


			btnDeletePlan.Visible = (bool)Application["HideDeletePlanButton"] == false;
			lblDeletePlan.Visible = (bool)Application["HideDeletePlanButton"] == false;
            btnEditPlan.Visible = false; // (bool)Application["HideEditPlanButton"] == false;
            lblEditPlan.Visible = false; // (bool)Application["HideEditPlanButton"] == false;

			Session["SubEditions"] = null;
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
			this.btnEditPlan.Click += new System.EventHandler(this.btnEditPlan_Click);
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
			this.LinkButtonAddNewPublication.Click += new System.EventHandler(this.LinkButtonAddNewPublication_Click);
			this.LinkButtonAddNewPageformat.Click += new System.EventHandler(this.LinkButtonAddNewPageformat_Click);
            this.dateChooserPubDate.SelectedDateChanged += new Telerik.Web.UI.Calendar.SelectedDateChangedEventHandler(this.dateChooserPubDate_SelectedDateChanged);
			this.txtWeekNumber.TextChanged += new System.EventHandler(this.txtWeekNumber_TextChanged);
			this.DataGridSections.ItemCreated += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DataGridSections_ItemCreated);
			this.DataGridSections.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGridSections_ItemCommand);
			this.DataGridSections.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DataGridSections_ItemDataBound);
			this.DataGridSections.SelectedIndexChanged += new System.EventHandler(this.DataGridSections_SelectedIndexChanged);

            this.btnSpecialEditions.Click += new System.EventHandler(this.btnAddEditions_Click);
			this.btnSavePlan.Click += new System.EventHandler(this.btnSavePlan_Click);
			this.btnCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

        private string GetEditionMatrixColumnPressNane(int colindex)
        {
            if (DataGridPressEditionMatrix.Columns.Count <= colindex)
                return "";
            string s = DataGridPressEditionMatrix.Columns[colindex].HeaderText;
            if ((bool)Application["PlanningCustomPressSelection"])
            {
                if (s == "PTT-GOSS")
                    s = "GOSS";
                if (s == "PTT-KBA")
                    s = "KBA";
            }

            return s;
        }

        private void CreateEditionPressMatrixGrid()
        {
            hasOnlyOnePerssAndEdition = false;

            DataTable dt = new DataTable();

            DataColumn newColumn;
            newColumn = dt.Columns.Add("Edition", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("Press1", Type.GetType("System.Boolean"));
            newColumn = dt.Columns.Add("Press2", Type.GetType("System.Boolean"));
            newColumn = dt.Columns.Add("Press3", Type.GetType("System.Boolean"));
            newColumn = dt.Columns.Add("Press4", Type.GetType("System.Boolean"));
            newColumn = dt.Columns.Add("Press5", Type.GetType("System.Boolean"));
            newColumn = dt.Columns.Add("Press6", Type.GetType("System.Boolean"));
            newColumn = dt.Columns.Add("Press7", Type.GetType("System.Boolean"));
            newColumn = dt.Columns.Add("Press8", Type.GetType("System.Boolean"));
            newColumn = dt.Columns.Add("Press9", Type.GetType("System.Boolean"));
            newColumn = dt.Columns.Add("Press10", Type.GetType("System.Boolean"));

            // Form list of allowed press(groups)
            ArrayList presslist = new ArrayList();

            CCDBaccess db = new CCDBaccess();
            string errmsg = "";
            int publicationIDSelected = Globals.GetIDFromName("PublicationNameCache", ddPublicationList.SelectedValue);

            string hardcodedpressforpublication = "";
            if ((bool)Application["PlanAlwaysUseDefaultPress"])
                hardcodedpressforpublication = GetDefaultPressFromPublicationDefault(publicationIDSelected);

            if (hardcodedpressforpublication != "")
            {
                presslist.Add(hardcodedpressforpublication);
            }
            else
            {

                string userpressesallowed = (string)Session["PressesAllowed"];
                if (userpressesallowed == "*" || userpressesallowed == "")
                {
                    ArrayList allpresses = Globals.GetArrayFromCache((bool)Application["UsePressGroups"] ? "PressGroupNameCache" : "PressNameCache");
                    foreach (string pressname in allpresses)
                        presslist.Add(pressname);
                }
                else
                {
                    string[] presslistx = userpressesallowed.Split(',');
                    foreach (string pressname in presslistx)
                        presslist.Add(pressname);
                }
            }
            // Take out press(group) from list if no valid template is defiend in PublicationTemplates.


            // This is a Int32 arrays...
            ArrayList aValidDefaultPressIDs = db.GetValidDefaultPressesForPublication(publicationIDSelected, out  errmsg);

            if (aValidDefaultPressIDs.Count == 0)
            {
                string fallbackPress = GetDefaultPressFromPublicationDefault(publicationIDSelected);
                if (fallbackPress == "")
                    fallbackPress = (string)Session["SelectedPlanPress"];
                int fallbackPressID = (bool)Application["UsePressGroups"] ? Globals.GetIDFromName("PressGroupNameCache", fallbackPress) : Globals.GetIDFromName("PressNameCache", fallbackPress);

                if (fallbackPressID > 0)
                    aValidDefaultPressIDs.Add(fallbackPressID);
            }
               

            ArrayList aValidDefaultPressesGroups = new ArrayList();

            if ((bool)Application["UsePressGroups"]  == false)
            {
                for (int ii = 0; ii < presslist.Count; ii++)
                {
                    if (Globals.IsInArray(aValidDefaultPressIDs, Globals.GetIDFromName("PressNameCache", (string)presslist[ii])) == false)
                         presslist[ii] = "";
                }
            }
            else
            {

                foreach(int pr in aValidDefaultPressIDs) 
                {
                    string pressGroupToTest = Globals.GetFirstPressGroupForPress(Globals.GetNameFromID("PressNameCache", pr));

                    Globals.AddToArray(ref aValidDefaultPressesGroups, pressGroupToTest);
                }

                for (int ii = 0; ii < presslist.Count; ii++ )
                {                    
                    if (Globals.IsInArray(aValidDefaultPressesGroups, (string)presslist[ii]) == false)
                        presslist[ii] = "";

                }

            }

            // Ser columnt headers to press names..
            int i=0;
            int c=0;
            int pressesInTotal = 0;
            foreach (DataGridColumn col in DataGridPressEditionMatrix.Columns)
            {
                if (c++ == 0)
                    continue;

                col.HeaderText = "";
                col.Visible = false;

                if (i < presslist.Count)
                {
                    if ((string)presslist[i] !="")
                    {
                        col.HeaderText = (string)presslist[i];
                        if ((bool)Application["PlanningCustomPressSelection"])
                        {
                            if (col.HeaderText == "GOSS")
                                col.HeaderText = "PTT-GOSS";
                            if (col.HeaderText == "KBA")
                                col.HeaderText = "PTT-KBA";
                        }
                        col.Visible = true;
                        pressesInTotal++;
                    }
                }
                i++;

            }
            
            // Press(group) columns are now in place..
            string defaultpress = GetDefaultPressFromPublicationDefault(publicationIDSelected);



            // Polaris Heimdal special.-.
            if ((bool)Application["PlanningCustomPressSelection"])
            {
                string customPressToUse = "";
                DateTime selectedDateTime = (DateTime)dateChooserPubDate.SelectedDate;
                DateTime pubdate = new DateTime(selectedDateTime.Year, selectedDateTime.Month, selectedDateTime.Day, 0, 0, 0, 0);

                if (db.GetCustomPlanningPresses(publicationIDSelected, pubdate, ref customPressToUse, out errmsg) == true)
                {
                    if (customPressToUse != "")
                        defaultpress = customPressToUse;
                }
            }

            if (defaultpress == "")
                defaultpress = (string)Session["SelectedPlanPress"];
            if (defaultpress == "")
                if ((string)Session["SelectedPress"] != "" && (string)Session["SelectedPress"] != "*")
                    defaultpress = (string)Session["SelectedPress"];

            ArrayList alleditions = Globals.GetArrayFromCache( "EditionNameCache");
            string sEditionsAllowed = (string)Session["EditionsAllowed"]; // From user setup
            string[] sEditionsAllowedList = sEditionsAllowed.Split(new char[] { ',' });

            bool bUsePublicationEditionList = Globals.HasAllowedEditions(publicationIDSelected);

            bool defaultToFirstEdition = (sEditionsAllowed == "" || sEditionsAllowed == "*");

            // Heimdal hack..
            if (bUsePublicationEditionList)
                defaultToFirstEdition = false;

            int editionsInTotal = 0;
            foreach (string edition in alleditions)
            {
                int editionID = Globals.GetIDFromName("EditionNameCache", edition);
                if ( (bUsePublicationEditionList && Globals.IsAllowedEdition(publicationIDSelected, editionID)) ||
                     (bUsePublicationEditionList == false && Globals.IsInList(sEditionsAllowedList, edition)) || 
                     defaultToFirstEdition)
                {
                    DataRow dr = dt.NewRow();
                    int j = 0;
                    dr[j++] = edition;
                    editionsInTotal++;
                    foreach (string press in presslist)
                    {
                        dr[j] = (bool)(press == defaultpress || presslist.Count == 1 || (defaultpress == "" && j == 1));                       
                        j++;
                    }
                    dt.Rows.Add(dr);
                }
                defaultToFirstEdition = false;              
            }
            if (pressesInTotal == 1 && editionsInTotal == 1)
                hasOnlyOnePerssAndEdition = true;

            DataGridPressEditionMatrix.Width = 200 + pressesInTotal * 80;
            DataGridPressEditionMatrix.DataSource = dt.DefaultView;
            DataGridPressEditionMatrix.DataBind();

            btnSpecialEditions.Visible = editionsInTotal > 1;
        }

        protected void DataGridPressEditionMatrix_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
			if (e.Item.ItemType == ListItemType.Header)
			{
				e.Item.Cells[0].Text = Global.rm.GetString("txtEdition");
                if (hasOnlyOnePerssAndEdition)
                {
                    e.Item.Cells[1].Enabled = false;
                }
			}
            if ((e.Item.ItemType == ListItemType.Item) ||
                (e.Item.ItemType == ListItemType.AlternatingItem) ||
                (e.Item.ItemType == ListItemType.SelectedItem))
            {
                //				DataTable dt = (DataTable) Cache["EditionNameCache"];
                //DataRow rowfirstsection = dt.Rows[0];
                int publicationIDSelected = Globals.GetIDFromName("PublicationNameCache", ddPublicationList.SelectedValue);
                string defaultpress = GetDefaultPressFromPublicationDefault(publicationIDSelected);

                // Polaris Heimdal special.-.
                if ((bool)Application["PlanningCustomPressSelection"])
                {
                    string customPressToUse = "";
                    DateTime selectedDateTime = (DateTime)dateChooserPubDate.SelectedDate;
                    DateTime pubdate = new DateTime(selectedDateTime.Year, selectedDateTime.Month, selectedDateTime.Day, 0, 0, 0, 0);

                    CCDBaccess db = new CCDBaccess();
                    string errmsg = "";

                    if (db.GetCustomPlanningPresses(publicationIDSelected, pubdate, ref customPressToUse, out errmsg) == true)
                    {
                        if (customPressToUse != "")
                            defaultpress = customPressToUse;
                    }
                }

                if (defaultpress == "")
                    defaultpress = (string)Session["SelectedPlanPress"];
                if (defaultpress == "")
                    if ((string)Session["SelectedPress"] != "" && (string)Session["SelectedPress"] != "*")
                        defaultpress = (string)Session["SelectedPress"];

                if (defaultpress != "")
                {
                    CheckBox use1 = (CheckBox)e.Item.FindControl("Press1");
                    CheckBox use2 = (CheckBox)e.Item.FindControl("Press2");
                    CheckBox use3 = (CheckBox)e.Item.FindControl("Press3");
                    CheckBox use4 = (CheckBox)e.Item.FindControl("Press4");
                    CheckBox use5 = (CheckBox)e.Item.FindControl("Press5");
                    CheckBox use6 = (CheckBox)e.Item.FindControl("Press6");
                    CheckBox use7 = (CheckBox)e.Item.FindControl("Press7");
                    CheckBox use8 = (CheckBox)e.Item.FindControl("Press8");
                    CheckBox use9 = (CheckBox)e.Item.FindControl("Press9");
                    CheckBox use10 = (CheckBox)e.Item.FindControl("Press10");

                    use1.Checked = GetEditionMatrixColumnPressNane(1) == defaultpress;
                    if (use1.Checked && hasOnlyOnePerssAndEdition)
                        use1.Enabled = false;
                    use2.Checked = GetEditionMatrixColumnPressNane(2) == defaultpress;
                    if (use2.Checked && hasOnlyOnePerssAndEdition)
                        use2.Enabled = false;
                    use3.Checked = GetEditionMatrixColumnPressNane(3) == defaultpress;
                    if (use3.Checked && hasOnlyOnePerssAndEdition)
                        use3.Enabled = false;
                    use4.Checked = GetEditionMatrixColumnPressNane(4) == defaultpress;
                    if (use4.Checked && hasOnlyOnePerssAndEdition)
                        use4.Enabled = false;
                    use5.Checked = GetEditionMatrixColumnPressNane(5) == defaultpress;
                    if (use5.Checked && hasOnlyOnePerssAndEdition)
                        use5.Enabled = false;
                    use6.Checked = GetEditionMatrixColumnPressNane(6) == defaultpress;
                    if (use6.Checked && hasOnlyOnePerssAndEdition)
                        use6.Enabled = false;
                    use7.Checked = GetEditionMatrixColumnPressNane(7) == defaultpress;
                    if (use7.Checked && hasOnlyOnePerssAndEdition)
                        use7.Enabled = false;
                    use8.Checked = GetEditionMatrixColumnPressNane(8) == defaultpress;
                    if (use8.Checked && hasOnlyOnePerssAndEdition)
                        use8.Enabled = false;
                    use9.Checked = GetEditionMatrixColumnPressNane(9) == defaultpress;
                    if (use9.Checked && hasOnlyOnePerssAndEdition)
                        use9.Enabled = false;
                    use10.Checked = GetEditionMatrixColumnPressNane(10) == defaultpress;
                    if (use10.Checked && hasOnlyOnePerssAndEdition)
                        use10.Enabled = false;
                    
                }
            }
        }
		
		private bool hasOnlyOneSection = false;
		private void CreateSectionGrid()
		{
			DataTable dt = new DataTable();

			DataColumn newColumn;
			newColumn = dt.Columns.Add("Use",Type.GetType("System.Boolean"));
			newColumn = dt.Columns.Add("Section",Type.GetType("System.String"));
			newColumn = dt.Columns.Add("Pages",Type.GetType("System.Int32"));
			newColumn = dt.Columns.Add("Colors",Type.GetType("System.Int32"));

			DataTable table = (DataTable) Cache["SectionNameCache"];
			
			
			string sSectionsAllowed = (string)Session["SectionsAllowed"];
			string [] sSectionsAllowedList = sSectionsAllowed.Split(new char[]{','});

			int publicationIDSelected = Globals.GetIDFromName("PublicationNameCache", ddPublicationList.SelectedValue); 

			bool bHasElements = false;
			bool bFirst = true; 
            bool usePubSections = Globals.HasAllowedSections(publicationIDSelected);

			foreach (DataRow row in table.Rows)
			{
				if (sSectionsAllowed == "" || sSectionsAllowed == "*" || Globals.IsInList(sSectionsAllowedList, (string)row["Name"]))
				{
                    if (usePubSections == false || (usePubSections && Globals.IsAllowedSection(publicationIDSelected, (int)row["ID"])))
					{
						DataRow dr = dt.NewRow();
						dr["Use"] = bFirst;
						bFirst = false;
						dr["Section"] = (string)row["Name"];
						dr["Pages"] = 0;
						dr["Colors"] = 0;
						dt.Rows.Add(dr);
						bHasElements = true;      
                        if (usePubSections == false && (bool)Application["PlanRestrictToOneSection"])
                            break;
					}
				}
			}		

			// Emergency - no sections available...
			if (bHasElements == false)
			{
				foreach (DataRow row in table.Rows)
				{
					DataRow dr = dt.NewRow();
					dr["Use"] = bFirst;
					bFirst = false;
					dr["Section"] = (string)row["Name"];
					dr["Pages"] = 0;
					dr["Colors"] = 0;
					dt.Rows.Add(dr);
				}
			}

            hasOnlyOneSection = dt.Rows.Count == 1;

            int colHidden = 0;
            if ((bool)Application["HidePlanPageOffset"])
                colHidden++;
            if ((bool)Application["HidePlanPagePrefix"])
                colHidden++;
            if ((bool)Application["HidePlanPagePostfix"])
                ++colHidden;

            DataGridSections.Columns[3].Visible = (bool)Application["HidePlanPageOffset"] == false;
            DataGridSections.Columns[4].Visible = (bool)Application["HidePlanPagePrefix"] == false;
            DataGridSections.Columns[5].Visible = (bool)Application["HidePlanPagePostfix"] == false;

            int nColorOptions = 0;
            if ((bool)Application["PlanPdfColorAllowed"])
                nColorOptions++;
			if ((bool)Application["PlanMonoColorAllowed"])
                nColorOptions++;
            DataGridSections.Columns[6].Visible = nColorOptions > 1;

            if (nColorOptions <= 1)
                colHidden++;

            DataGridSections.Width = 640 - 80*colHidden;
			DataGridSections.DataSource = dt.DefaultView;
			DataGridSections.DataBind();
		}

		private void DataGridSections_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			DataGridSections.SelectedIndex = e.Item.ItemIndex;
		}

		private void DataGridSections_ItemCreated(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
	
		}

		private void DataGridSections_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Header)
			{
				e.Item.Cells[0].Text = Global.rm.GetString("txtUse");
				e.Item.Cells[1].Text = Global.rm.GetString("txtSection");
				e.Item.Cells[2].Text = Global.rm.GetString("txtPages");
				e.Item.Cells[3].Text = Global.rm.GetString("txtOffset");
				e.Item.Cells[4].Text = Global.rm.GetString("txtPagePrefix");
				e.Item.Cells[5].Text = Global.rm.GetString("txtPagePostfix");
				e.Item.Cells[6].Text = Global.rm.GetString("txtColors");
				e.Item.Cells[7].Text = Global.rm.GetString("txtCopies");
			}
			if ((e.Item.ItemType == ListItemType.Item) ||
				(e.Item.ItemType == ListItemType.AlternatingItem) ||
				(e.Item.ItemType == ListItemType.SelectedItem))
			{
				DataTable dt = (DataTable) Cache["SectionNameCache"];
				DataRow rowfirstsection = dt.Rows[0];

				TableCell cell = (TableCell)e.Item.Cells[1];
				CheckBox use = (CheckBox)e.Item.FindControl("CheckBoxUseSection");
				TextBox pages = (TextBox)e.Item.FindControl("txtNumberOfPages");
				TextBox pageoffset = (TextBox)e.Item.FindControl("txtPageOffset");
				TextBox pageprefix = (TextBox)e.Item.FindControl("txtPagePrefix");
				TextBox pagepostfix = (TextBox)e.Item.FindControl("txtPagePostfix");

				pageoffset.Visible = (bool)Application["HidePlanPageOffset"] == false;
				pageprefix.Visible = (bool)Application["HidePlanPagePrefix"] == false;
				pagepostfix.Visible = (bool)Application["HidePlanPagePostfix"] == false;

				TextBox pagecopies  = (TextBox)e.Item.FindControl("txtCopies");
				RadioButtonList rbColorModeList =   (RadioButtonList)e.Item.FindControl( "RadioButtonListColorMode");
			

				if ((bool)Application["PlanPdfColorAllowed"] == false)
				{
					rbColorModeList.Items.Remove("PDF");
				}
				if ((bool)Application["PlanMonoColorAllowed"] == false)
				{
					rbColorModeList.Items.Remove("Mono");
				}
			
				pageoffset.Text = "1";
				pagecopies.Text = "1";

				bool checkDefaultSection = false;
				if (cell.Text == (string)rowfirstsection["Name"])
					checkDefaultSection = true;

				use.Checked = checkDefaultSection;
				pages.Text = checkDefaultSection ? "2" : "0";
                if (hasOnlyOneSection)
                {
                    use.Enabled = false;
                    use.Checked = true;
                }
			}
		}

	
		private void DataGridSections_SelectedIndexChanged(object sender, System.EventArgs e)
		{

            
		}

		// -----------------------------------------------------
		// Pages-color checkbox grid 
		// -----------------------------------------------------

      //  private string currentSection = "";
     //   private int currentPages = 0;
     //   private int currentOffset = 1;
     //   private string currentPrefix = "";
	//	private string currentPostfix = "";


		
		
		//--------------------------------------
		// Save plan to database
		//--------------------------------------

		private void btnSavePlan_Click(object sender, System.EventArgs e)
		{
            if ((bool)Application["PlanUseXml"])
                SavePagePlanXML(false);
            else
    			SavePagePlan(false);

		}

        // Get Editions to be planned for particular press
        private ArrayList GetListOfEditionsFromGridForPress(string press)
        {
            ArrayList aEditions = new ArrayList();
            int colNumber = 0;
            int colNumberOfPress = -1;
            foreach (DataGridColumn col in DataGridPressEditionMatrix.Columns)
            {
                if (colNumber > 0)
                {
                    string s = col.HeaderText;

                    if ((bool)Application["PlanningCustomPressSelection"])
                    {
                        if (s == "PTT-GOSS")
                            s = "GOSS";
                        if (s == "PTT-KBA")
                            s = "KBA";
                    }
                    if (press == s)
                    {
                        colNumberOfPress = colNumber;
                        break;
                    }
                }
                colNumber++;
            }

            if ( colNumberOfPress == -1)
                return aEditions;

            foreach (DataGridItem dataItem in DataGridPressEditionMatrix.Items)
            {
                string edition = dataItem.Cells[0].Text;
                if (edition == "")
                    continue;

                CheckBox cb = (CheckBox)dataItem.Cells[colNumberOfPress].FindControl("Press" + colNumberOfPress.ToString());
                if (cb == null)
                    continue;
                if (cb.Visible == true && cb.Checked)
                   aEditions.Add(edition);
    
            }
            return aEditions;
        }

        private void RestrictEditionsInGridForPress(string press, string preferredSingleEdition)
        {
            ArrayList aEditions = new ArrayList();
            int colNumber = 0;
            int colNumberOfPress = -1;
            foreach (DataGridColumn col in DataGridPressEditionMatrix.Columns)
            {
                if (colNumber > 0)
                {
                    if (press == col.HeaderText)
                    {
                        colNumberOfPress = colNumber;
                        break;
                    }
                }
                colNumber++;
            }

            if (colNumberOfPress == -1)
                return;

            bool hasPreferredEdition = false;
            foreach (DataGridItem dataItem in DataGridPressEditionMatrix.Items)
            {

                string edition = dataItem.Cells[0].Text;
                if (edition == "")
                    continue;
                CheckBox cb = (CheckBox)dataItem.Cells[colNumberOfPress].FindControl("Press" + colNumberOfPress.ToString());
                if (cb == null)
                    continue;

                if (edition == preferredSingleEdition)
                {
                    cb.Checked = true;
                    hasPreferredEdition = true;
                }
                else
                    cb.Checked = false;
            }

            // preferred edition not found - defalut to first in list
            if (hasPreferredEdition == false)
            {
                foreach (DataGridItem dataItem in DataGridPressEditionMatrix.Items)
                {
                    string edition = dataItem.Cells[0].Text;
                    if (edition == "")
                        continue;
                    CheckBox cb = (CheckBox)dataItem.Cells[colNumberOfPress].FindControl("Press" + colNumberOfPress.ToString());
                    if (cb == null)
                        continue;

                    cb.Checked = true;
                    break;
                }
            }
        }

        private ArrayList GetListOfEditionsFromGrid(ref ArrayList aPresses)
        {
            ArrayList aEditions = new ArrayList();

            ArrayList potentialPressesCol = new ArrayList();
            ArrayList potentialPresses = new ArrayList();
            aPresses.Clear();

            int colNumber = 0;
            foreach (DataGridColumn col in DataGridPressEditionMatrix.Columns)
            {
                if (colNumber > 0)
                {
                    if (col.HeaderText != "")
                    {
                        potentialPressesCol.Add(colNumber);
                        string s = col.HeaderText;

                        if ((bool)Application["PlanningCustomPressSelection"])
                        {                            
                            if (s == "PTT-GOSS")
                                s = "GOSS";
                            if (s == "PTT-KBA")
                                s = "KBA";
                        }
                        potentialPresses.Add(s);
                    }
                }
                colNumber++;
            }

            foreach (DataGridItem dataItem in DataGridPressEditionMatrix.Items)
            {
                string edition = dataItem.Cells[0].Text;
                if (edition == "")
                    continue;

                for (int i = 0; i < potentialPresses.Count; i++)
                {
                    colNumber = (int)potentialPressesCol[i];
                    CheckBox cb = (CheckBox)dataItem.Cells[colNumber].FindControl("Press" + colNumber.ToString());
                    if (cb == null)
                        continue;
                    if (cb.Visible == true && cb.Checked)
                    {
                        aEditions.Add(edition);
                        Globals.AddToArray(ref aPresses, (string)potentialPresses[i]); 
                    }
                }
            }
            return aEditions;
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
            aEditions = GetListOfEditionsFromGrid(ref aPresses);


            // Polaris Heimdal special.-.
/*            if ((bool)Application["PlanningCustomPressSelection"])
            {
                string customPressToUse = "";
                
                if (db.GetCustomPlanningPresses(publicationID, pubdate, ref customPressToUse, out errmsg) == true)
                {
                    if (customPressToUse != "")
                    {
                        aPresses.Clear();
                        aPresses.Add(customPressToUse);
                    }
                }
            }
*/

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
                        TextBox pageprefix = (TextBox)dataItem.Cells[4].FindControl("txtPagePrefix");
                        TextBox pagepostfix = (TextBox)dataItem.Cells[5].FindControl("txtPagePostfix");
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
                        catch //(Exception e2)
                        {
                        }

                        if (n > 0)
                        {                            
                            aSections.Add((string)section);
                            aPages.Add((int)n);
                            aPageOffset.Add((int)offs);
                            aPagePrefix.Add((string)pageprefix.Text);
                            aPagePostfix.Add((string)pagepostfix.Text);
                            aColors.Add((string)colorSelect.SelectedValue != "" ? colorSelect.SelectedValue  : "CMYK");
                        }
                        else
                        {
                            lblError.Text = "Pagecount 0 not allowed for section " + dataItem.Cells[1].Text;
                            lblError.ForeColor = Color.Red;
                            return;
                        }
                    }
                    catch //(Exception e1)
                    { }

                    nSectionIndex++;
                }
            }

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
            
            int nPriority = 50;
            int paginationMode = 0;
            bool onHold = true;
            bool insertedSections = false;
            bool separateRuns = false;
            int flatProofID = 0;
            int copies = 1;

           // bool forcePDF = false;
            for (int pressNumber = 0; pressNumber < aPresses.Count; pressNumber++)
            {
                string press = (string)aPresses[pressNumber];
                int pressID = Globals.GetIDFromName((bool)Application["UsePressGroups"] ? "PressGroupNameCache" : "PressNameCache", press);
                if ((bool)Application["UsePressGroups"])
                    pressID = Globals.GetFirstPressIDFromPPressGroup(pressID);

//if (Globals.GetPublicationTemplateFileType(publicationID, pressID) > 0)
              //      forcePDF = true;
                
                aEditions = GetListOfEditionsFromGridForPress(press);

                int templateID = 0;
                bool forcePDF = false;
                string ripSetupString = "";
                bool autoApply = db.GetAutoApply(publicationID, pressID, ref paginationMode, ref onHold, ref insertedSections, ref separateRuns, ref flatProofID, ref nPriority, ref templateID, ref copies, ref forcePDF, ref ripSetupString, out errmsg);
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


                if ((bool)Application["PlanRunPreProcedure"])
                    db.PreImport(publicationID, pubdate, pressID, autoApply ? 1 : 0, out errmsg);

                if ((bool)Application["PlanAlwaysPDF"])
                    forcePDF = true;

                int dummytemplateID = 0;
                if (autoApply == false)
                    dummytemplateID = Globals.GetDummyTemplateID(press);
                if (templateID == 0)
                    templateID = dummytemplateID;

                // Use dummy template if unapplied plan (page plan)
                if (dummytemplateID > 0 && (autoApply == false && (int)Application["AlwaysAutoApply"] == 0))
                    templateID = dummytemplateID;



                //string template = Globals.GetNameFromID("TemplateNameCache", templateID);

                int pageFormatID = 0;
                if (ddPageFormatList.Visible)
                    pageFormatID = Globals.GetIDFromName("PageFormatCache", ddPageFormatList.SelectedValue);
                else
                    pageFormatID = Globals.GetPublicationPageformatID(publicationID);

                int proofID = Globals.GetPublicationProofID(publicationID);
             
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
                int totalPageCount = 0;

                if (db.CreatePagePlan(publicationID, pubdate, aEditions.Count,
                    pressID, templateID, proofID,
                    checkApprovalRequired.Checked,
                    nPriority,
                    aSections.Count, totalPages, /*aColors,*/
                    out  bExistingProduction, out bIsLockedProduction, out  nProductionID, true, out  errmsg) == false)
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

                int m_presssectionnumber = 1;

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

                foreach (string edition in aEditions)
                {
                    int editionID = Globals.GetIDFromName("EditionNameCache", edition);
                    PageTableEntry item = new PageTableEntry();

                    item.m_ripSetupID = ripSetupID;
                    item.m_pageFormatID = pageFormatID;
                    item.m_deadline = defaultDeadline;
                    item.m_templateID = templateID;
                    item.m_pressID = pressID;
                    item.m_productionID = nProductionID;
                    item.m_pagesonplate = 1;
                    item.m_locationID = Globals.GetTypeFromName("PressNameCache", press);	// Returns LocationID!				
                    item.m_publicationID = publicationID;
                    item.m_issueID = 1;
                    item.m_editionID = editionID;
                    item.m_proofID = proofID;
                    item.m_pubdate = pubdate;
                    item.m_weeknumber = weekNumber;
                    item.m_deviceID = 0;
                    item.m_version = 0;
                    item.m_layer = 1;
                    item.m_approved = checkApprovalRequired.Checked ? 0 : -1;
                    item.m_hold = onHold;
                    item.m_priority = nPriority;
                    item.m_pagetype = 0;

                    int sectionindex = 0;
                    int copyseparationset = 0;
                    int incopyseparationset = 0;
                    int incopyflatseparationset = 0;
                    int copyflatseparationset = 0;
                    int pageindexglobal = 0;

                    int circulation = (int)RadNumericTextBoxCirculation.Value;
                    int circulation2 = (int)RadNumericTextBoxCirculation2.Value;
                    int timedEditionFrom = 0;
                    int timedEditionTo = 0;
                    int timedEditionSequence = 0;

                    foreach (string section in aSections)
                    {
                        int sectionID = Globals.GetIDFromName("SectionNameCache", section);
                        string colormode = (string)aColors[sectionindex];
                        int nPages = (int)aPages[sectionindex];
                        int nPageOffset = (int)aPageOffset[sectionindex];

                        int nPressRunID = 1;
                        errmsg = "";
                        //bool combineSections = (bool)Application["HidePlanCombineSections"] ? false : CheckBoxCombineSections.Checked;

                        // HARDWIRE TO NON-COMBINED IF NOT AUTOAPPLY
                        bool combineSections = false;
                        //if (autoApply && separateRuns == false)
                        //	combineSections = true;


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
                        aPressRuns.Add(nPressRunID);

                        int nExistingPages = nPages;
                        if (bExistingProduction)
                        {
                            int nPg = 0;
                            if (db.GetPageCount(nPressRunID, out nPg, out errmsg))
                                nExistingPages = nPg;
                        }
                        item.m_pagecountchange = nExistingPages != nPages;

                        item.m_presssectionnumber = m_presssectionnumber;
                        if (combineSections == false)
                        {
                            m_presssectionnumber++;
                            pageindexglobal = 0;
                        }

                        item.m_pressrunID = nPressRunID;
                        item.m_sectionID = sectionID;

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


                        for (int pageindex = 0; pageindex < nPages; pageindex++)
                        {
                            item.m_pagination = pageindexglobal + 1;
                            pageindexglobal++;
                            item.m_sheetside = pageindex % 2;
                            item.m_sheetnumber = (pageindex + 2) / 2;

                            int nNumberOfColors = pagesection.GetNumberOfPageColors(pageindex);

                            for (int color = 0; color < nNumberOfColors; color++)
                            {
                                item.m_colorIndex = color + 1;

                                bool bIsActive = false;
                                item.m_colorID = pagesection.GetPageColorID(pageindex, color, out bIsActive);
                                item.m_active = bIsActive;

                                item.m_pageindex = pageindex + nPageOffset;
                                //	 item.m_pagename = (string)aPagePrefix[sectionindex] + item.m_pagination.ToString() + (string)aPagePostfix[sectionindex];
                                item.m_pagename = pagesection.GetPageName(pageindex);

                                item.m_mastereditionID = Globals.GetIDFromName("EditionNameCache", getMasterEditionForPage(edition, section, item.m_pagename));

                                item.m_uniquepage = (item.m_mastereditionID == item.m_editionID);


                                if (db.InsertSeparation(item, incopyseparationset, out copyseparationset, incopyflatseparationset, out copyflatseparationset, 4, true,
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
                        }
                        sectionindex++;

                    }
                    editionindex++;
                    // re-aquire lock!
                    if (planLockMode > 0 && bCurrentPlanLock == 1)
                        db.PlanLock((string)Session["UserName"] + "-" + Request.UserHostAddress, 1, ref bCurrentPlanLock, ref sClientName, ref tClientTime, out errmsg);

                    if (publicationPlanLockMode && bCurrentPublicationPlanLock == 1)
                        db.PublicationPlanLock(publicationID, pubdate, (string)Session["UserName"] + "-" + Request.UserHostAddress, 1, ref bCurrentPublicationPlanLock, ref sClientName, ref tClientTime, out errmsg);

                }

                /*                foreach (int pressRunID in aPressRuns)
                                {
                                    db.UpdatePressTime(pressRunID, (DateTime)WebDateTimePressTime.SelectedDate, out errmsg);
                                    int w = (int)txtWeekNumber.Value;
                                    if ((bool)Application["UseWeeknumberAsComment"] && w > 0)
                                        db.UpdatePressRunOrderNumber(pressRunID, w.ToString(), out errmsg);

                                }
                */
                // do not report if error on this DB update...

                db.UpdateProductionOrderNumber(nProductionID, productionOrderNumber, out errmsg);

                if ((int)Application["AutoRetryPlanFiles"] > 0)
                     db.AddAutoRetryRequest(nProductionID, (int)Application["AutoRetryPlanFiles"], out  errmsg);

                if ((int)Application["ExportPlanFile"] == 1)
                {
                    db.AddPlanExportJob(nProductionID, 0, out errmsg);
                }
                else if ((int)Application["ExportPlanFile"] == 2)
                {
                    foreach (int pressRunID in aPressRuns)
                        db.AddPlanExportJob(nProductionID, pressRunID, out errmsg);
                }

                if (autoApply || (int)Application["AlwaysAutoApply"]>0)
                {
                    Global.logging.WriteLog("AutoApplyProduction " + nProductionID.ToString() + " " + paginationMode.ToString());
                    if (db.AutoApplyProduction(nProductionID, paginationMode, insertedSections, separateRuns, flatProofID, templateID, (int)Application["AlwaysAutoApply"], out errmsg) == false)
                    {
                        lblError.Text = errmsg;
                        lblError.ForeColor = Color.Red;
                    }
                }
                
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


                    db.InsertLogEntry((int)Application["ProcessID"],
                                            bExistingProduction ? (int)Globals.EventCodes.PlanEdit : (int)Globals.EventCodes.PlanCreate,
                                            "WebCenter " + userName,
                                            string.Format("{0}-{1:00}{2:00}-{3}-{4}", ddPublicationList.SelectedValue, pubdate.Day, pubdate.Month, editionString, sectionString),
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
            btnEditPlan.Visible = false; // (bool)Application["HideEditPlanButton"] == false;
            lblEditPlan.Visible = false; // (bool)Application["HideEditPlanButton"] == false;
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
            aEditions = GetListOfEditionsFromGrid(ref aPresses);

            int weekNumber = (bool)Application["UseWeeknumberAsComment"] == false ? (int)txtWeekNumber.Value : 0;
            if ((bool)Application["HidePlanWeekNumber"])
                weekNumber = 0;

            errmsg = "";
            lblError.Text = "";
            int nSectionIndex = 0;
            int editionindex = 0;
            int totalPages = 0;

            // Retrieve pagecount for each section

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
                        catch //(Exception e2)
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
                    catch //(Exception e1)
                    { }

                    nSectionIndex++;
                }
            }

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

                aEditions = GetListOfEditionsFromGridForPress(press);

                int templateID = 0;
                bool forcePDF = false;


                Pageplan plan = new Pageplan();
                plan.publicationName = ddPublicationList.SelectedValue;
                plan.publicationAlias = db.LookupInkAlias(plan.publicationName, pressID, out errmsg);
                if (plan.publicationAlias != "")
                    plan.publicationName = plan.publicationAlias;
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
                if (ddPageFormatList.Visible)
                    pageFormatID = Globals.GetIDFromName("PageFormatCache", ddPageFormatList.SelectedValue);
                else
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
                            pageItem.masterEdition = getMasterEditionForPage(edition, section, pageItem.pageName);
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
            btnEditPlan.Visible = false;// (bool)Application["HideEditPlanButton"] == false;
            lblEditPlan.Visible = false;//(bool)Application["HideEditPlanButton"] == false;
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

        private void SetApprovalFromPublication()
        {
            int publicationID = Globals.GetIDFromName("PublicationNameCache", ddPublicationList.SelectedValue);
            int n = Globals.GetPublicationApprovalRequired(publicationID);
            checkApprovalRequired.Checked = n > 0;
        }

		protected void ddPublicationList_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
		{
			SelectPageFormatFromPublication();
            SetApprovalFromPublication();
			SelectPressFromPublication();
			CreateSectionGrid();
            CreateEditionPressMatrixGrid();
            if ((string)Application["PlanPreferredEdition"] != "")
                RestrictEditionsInGridForPress((string)Session["SelectedPlanPress"],(string)Application["PlanPreferredEdition"]);


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

		private int SectionsAdded()
		{
			int numberOfSectios = 0;
			foreach (DataGridItem dataItem in DataGridSections.Items)
			{
				CheckBox cb = (CheckBox)dataItem.Cells[0].FindControl("CheckBoxUseSection");
				if (cb.Checked)
				{
					try 
					{
						TextBox pages  = (TextBox)dataItem.Cells[2].FindControl("txtNumberOfPages");
						int npages = 0;
						if (pages != null)
							npages = Globals.TryParse(pages.Text, 0);
						
						if (npages > 0)
							numberOfSectios++;
					}
					catch
					{
						return numberOfSectios;
					}
				}
			}
			return numberOfSectios;
		}

        private string SectionsDefinedInSecionGrid()
        {
            string s = "";
            foreach (DataGridItem dataItem in DataGridSections.Items)
            {
                CheckBox cb = (CheckBox)dataItem.Cells[0].FindControl("CheckBoxUseSection");
                if (cb.Checked)
                {
                    try
                    {
                        TextBox pages = (TextBox)dataItem.Cells[2].FindControl("txtNumberOfPages");
                        int npages = 0;
                        if (pages != null)
                            npages = Globals.TryParse(pages.Text, 0);

                        if (npages > 0)
                        {
                            if (s != "")
                                s += ",";
                            s += npages.ToString();
                        }
                    }
                    catch
                    {
                        return "";
                    }
                }
            }
            return s;
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

        private int InitializeSubEditionSession(bool allunique)
        {
            int totalpages = 0;
            Session["SubEditions"] = null;

            ArrayList aPresses = new ArrayList();
            ArrayList aEditions = GetListOfEditionsFromGrid(ref  aPresses);

            if (aEditions.Count == 0)
                return 0;

            string mainEdtion = (string)aEditions[0];

            DataSet ds = new DataSet();
            DataTable dt = new DataTable("SubEditions");
            DataColumn newColumn;
            newColumn = dt.Columns.Add("Section", Type.GetType("System.String"));
            newColumn = dt.Columns.Add("Page", Type.GetType("System.String"));
            for (int i = 0; i < aEditions.Count; i++ )
            {
                newColumn = dt.Columns.Add((string)aEditions[i], Type.GetType("System.String"));
            }
            // Insert main edition

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
                            for (int j = 0; j < aEditions.Count; j++)
                                newRow[(string)aEditions[j]] = allunique ? (string)aEditions[j] : mainEdtion;
                            dt.Rows.Add(newRow);
                        }

                        totalpages += npages;
                    }
                    catch
                    {
                    }
                }
            }

            ds.Tables.Add(dt);

            Session["SubEditions"] = ds;


            return totalpages;
            
        }

		private void btnAddEditions_Click(object sender, System.EventArgs e)
		{
            lblInfo.Text = "";
            lblError.Text = "";
            int totalpages = 0;
            string totalPagesInSectionGrid = SectionsDefinedInSecionGrid();
            string totalPagesInSession = SectionsDefinedInEditionSession();
            bool differentpagecount = totalPagesInSectionGrid != totalPagesInSession;
            if (Session["SubEditions"] == null || differentpagecount)
                totalpages = InitializeSubEditionSession(false);
            doPopupEditionWindow(totalpages);
        }


		private string getMasterEditionForPage(string edition,  string section, string pagename)
		{
			DataSet dsEditions = (DataSet)Session["SubEditions"];
			if (dsEditions == null)
			{
				lblError.Text = "Error setting main edition reference";
				return edition;
			}
			DataTable dt = dsEditions.Tables[0];

			int idx = 3;
			bool found = false;
			for (idx=3; idx<dt.Columns.Count; idx++)
			{
				if (dt.Columns[idx].ColumnName == edition)
				{
					found = true;
					break;
				}
			}

			if (found == false)
				return edition;

			foreach (DataRow row in dt.Rows)
			{
				if ((string)row["Section"] == section && (string)row["Page"] == pagename)
				{
					return (string)row[idx];
				}
			}

			return edition;
		}

		private void doPopupEditionWindow(int totalPages)
        {


            int publicationIDSelected = Globals.GetIDFromName("PublicationNameCache", ddPublicationList.SelectedValue);

            Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowSpecialEditionPages");
            int maxHeight = 1000;
            if (Session["WindowHeight"] != null)
                if ((int)Session["WindowHeight"] > 0)
                    maxHeight = (int)Session["WindowHeight"] - 50;
            if (totalPages > 0)
                mywindow.Height = 400 + totalPages * 20 < maxHeight ? 400 + totalPages * 20 : maxHeight;
            mywindow.NavigateUrl = "AddEditionInfo.aspx?publicationID=" + publicationIDSelected.ToString();
            mywindow.VisibleOnPageLoad = true;
/*
			string popupScript =
				"<script language='javascript'>" +
				"var xpos = 300;" + 
				"var ypos = 300;" +
				"if(window.screen) { xpos = (screen.width-460)/2; ypos = (screen.height-610)/2; }" + 
				"var s = 'status=no,resizable=yes,scrollbars=yes,top='+ypos+',left='+xpos+',width=460,height=610';" +
				"var PopupWindow = window.open('AddEditionInfo.aspx?publicationID="+publicationIDSelected.ToString()+"','Editions',s);" + 	
				"if (parseInt(navigator.appVersion) >= 4) PopupWindow.focus();" +
				"</script>";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "PopupScript", popupScript);
*/
		}



		private void btnDeletePlan_Click(object sender, System.EventArgs e)
		{
			lblError.Text = "";

			PanelEditPlan.Visible = false;
			PanelDeletePlan.Visible = true;
			lblAddPagePlan.Visible = false;
			btnAddPlan.Visible = false;
			btnDeletePlan.Visible = false;
			lblDeletePlan.Visible = false;

            PanelMainActionButtons.Visible = false;

			btnEditPlan.Visible = false;
			lblEditPlan.Visible = false;

			Session["SubEditions"] = null;

			PlanListDataBind(1);
		}

		private void PlanListDataBind(int view)
		{
			lblInfo.Text = "";
			lblError.Text = "";
			string errmsg = "";			
			CCDBaccess	db = new CCDBaccess();
			DataTable dbTable = db.GetProductionList((int)Application["AlwaysAutoApply"] > 0, out errmsg);
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
			PanelEditPlan.Visible = false;

			lblAddPagePlan.Visible = true;
			btnAddPlan.Visible = true;
			PanelAddPlan.Visible = false;
            PanelMainActionButtons.Visible = true;
			
			btnDeletePlan.Visible = true;
			lblDeletePlan.Visible = true;

            btnEditPlan.Visible = false;// (bool)Application["HideEditPlanButton"] == false;
            lblEditPlan.Visible = false;//(bool)Application["HideEditPlanButton"] == false;
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

			PanelEditPlan.Visible = true;
			PanelDeletePlan.Visible = false;
			lblAddPagePlan.Visible = false;
			btnAddPlan.Visible = false;
			btnDeletePlan.Visible = false;
			lblDeletePlan.Visible = false;
            PanelMainActionButtons.Visible = false;

			btnEditPlan.Visible = false;
			lblEditPlan.Visible = false;

			PlanListDataBind(2);
		}

		private void btlCloseEditPlan_Click(object sender, System.EventArgs e)
		{
			lblInfo.Text = "";
			lblError.Text = "";

			PanelDeletePlan.Visible = false;
			PanelEditPlan.Visible = false;

			lblAddPagePlan.Visible = true;
			btnAddPlan.Visible = true;
			PanelAddPlan.Visible = false;
            PanelMainActionButtons.Visible = true;
			
			btnDeletePlan.Visible = (bool)Application["HideDeletePlanButton"] == false;
			lblDeletePlan.Visible = (bool)Application["HideDeletePlanButton"] == false;

			btnEditPlan.Visible = true;
			lblEditPlan.Visible = true;	

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
            if ((bool)Application["PlanningCustomPressSelection"])
                CreateEditionPressMatrixGrid();
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

        protected void btnUploadPlan_Click(object sender, EventArgs e)
        {
            Telerik.Web.UI.RadWindow mywindow = GetRadWindow("radWindowUploadPlan");
            mywindow.VisibleOnPageLoad = true;

        }


	}
}
