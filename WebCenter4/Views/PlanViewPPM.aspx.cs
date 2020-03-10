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
using Telerik.Web.UI;
using System.IO;

namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for PlanView.
	/// </summary>
	public partial class PlanViewPPM : System.Web.UI.Page
	{
        // Variables for javascript communication

        protected int updateTree; 
        protected int existingProductionPrompt;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if ((string)Session["UserName"] == null)
				Response.Redirect("~/SessionTimeout.htm");

			if ((string)Session["UserName"] == "")
				Response.Redirect("/Denied.htm");

            string uploadFolder = "";
            uploadFolder = Global.sVirtualPPMUploadFolder;
            hiddenUploadPath.Value = uploadFolder;


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
				Globals.ForcePPMCacheReload();

				Session["RefreshTree"] = false;
                Session["UploadedFiles"] = "";

                btnAddPlan.Visible = (bool)Application["HideAddPlanButton"] == false;
                lblAddPagePlan.Visible = (bool)Application["HideAddPlanButton"] == false;
                PanelMainActionButtons.Visible = true;

                btnDeletePlan.Visible = (bool)Application["HideDeletePlanButton"] == false;
				lblDeletePlan.Visible = (bool)Application["HideDeletePlanButton"] == false;
                PanelAddPlan.Visible = false;

				lblInfo.Text = "";
				lblError.Text = "";

				existingProductionPrompt = 0;
				saveConfirm.Value = "0";
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

                CreateEditionDropDown();
                CreatePaperDropDown();
                CreatePageFormatDropDown();
                CreatePressGroupDropDown();
                CreatePublicationDropDown();

                DateTime t = DateTime.Now;
                t = t.AddDays(1.0);
                dateChooserPubDate.SelectedDate = t;


          /*      if ((int)Session["DefaultPublicationID"] > 0)
				{
					string defaultPublicationName = Globals.GetNameFromID("PublicationNameCache", (int)Session["DefaultPublicationID"]);
					if (defaultPublicationName != "")
						ddPublicationList.SelectedValue = defaultPublicationName;
				}
                */
                // Final adjustment of press....
                ddPublicationList_SelectedIndexChanged(null, null);

                ddPublicationList.Enabled = true;

			}

			if (saveConfirm.Value == "1")
				SavePPMPlan(true);            
		}

        private string GetAnyPress()
        {
            DataTable dt = (bool)Application["UsePressGroups"] ? (DataTable)HttpContext.Current.Cache["PressGroupNameCache"] : (DataTable)HttpContext.Current.Cache["PressNameCache"];
            if (dt.Rows.Count > 0)
                return (string)dt.Rows[0]["Name"];

            return "";

        }


        private void SetPageFormatFromPublication()
        {
            if (ddPublicationList.SelectedIndex < 0)
                return;

            PPMPublication publication = Globals.LookupPPMPublication(ddPublicationList.SelectedValue);
            if (publication == null)
                return;

            if (publication.Default_PageFormat != "")
                ddPageFormatList.SelectedValue = publication.Default_PageFormat;
            else
                ddPageFormatList.SelectedIndex = 0;
        }
        
        private void SetEditionsFromPublication()
        {
            if (ddPublicationList.SelectedIndex < 0)
                return;

            PPMPublication publication = Globals.LookupPPMPublication(ddPublicationList.SelectedValue);
            if (publication == null)
                return;

            if (publication.Default_Zone != "")
                ddEditionList.SelectedValue = publication.Default_Zone;
            else
                ddEditionList.SelectedIndex = 0;

        }

        private void SetPaperFromPublication()
        {
            if (ddPublicationList.SelectedIndex < 0)
                return;

            PPMPublication publication = Globals.LookupPPMPublication(ddPublicationList.SelectedValue);
            if (publication == null)
                return;

            if (publication.Default_Paper != "")
                ddPaperList.SelectedValue = publication.Default_Paper;
            else
                ddPaperList.SelectedIndex = 0;
        }

        private void SetCirculationFromPublication()
        {
            if (ddPublicationList.SelectedIndex < 0)
                return;

            PPMPublication publication = Globals.LookupPPMPublication(ddPublicationList.SelectedValue);
            if (publication == null)
                return;

            if (publication.Default_copies > 0)
                RadNumericTextBoxCirculation.Value = publication.Default_copies;

        }


        private void SetPressFromPublication()
        {
            if (ddPublicationList.SelectedIndex < 0)
                return;

            PPMPublication publication = Globals.LookupPPMPublication(ddPublicationList.SelectedValue);
            if (publication == null)
                return;

            if (publication.Default_PressGroup != "")
                ddPressGroupList.SelectedValue = publication.Default_Zone;
            else
                ddPressGroupList.SelectedIndex = 0;

        }

        private void CreatePressGroupDropDown()
        {
            List<string> editions = (List<string>)Cache["PPMPressNamesCache"];
            if (editions == null)
                return;

            ddPressGroupList.Items.Clear();
            foreach (string s in editions)
                ddPressGroupList.Items.Add(new Telerik.Web.UI.RadComboBoxItem(s, s));
        }

        private void CreateEditionDropDown()
        {
            List<string> editions = (List<string>)Cache["PPMEditionNamesCache"];
            if (editions == null)
                return;

            ddEditionList.Items.Clear();
            foreach(string s in editions)
                ddEditionList.Items.Add(new Telerik.Web.UI.RadComboBoxItem(s, s));
        }

        private void CreatePaperDropDown()
        {
            List<string> papers = (List<string>)Cache["PPMPaperNamesCache"];
            if (papers == null)
                return;

            ddPaperList.Items.Clear();
            foreach (string s in papers)
                ddPaperList.Items.Add(new Telerik.Web.UI.RadComboBoxItem(s, s));
        }

        private void CreatePageFormatDropDown()
        {
            List<PPMPageFormat> pageFormats = (List<PPMPageFormat>)Cache["PPMPageFormatNamesCache"];
            if (pageFormats == null)
                return;

            ddPageFormatList.Items.Clear();
            foreach (PPMPageFormat pageFormat in pageFormats)
                ddPageFormatList.Items.Add(new Telerik.Web.UI.RadComboBoxItem(string.Format("{0} x {1} {2}", pageFormat.Width, pageFormat.Height, pageFormat.Name),pageFormat.Name ));
        }

        private void CreatePublicationDropDown()
        {
            List<PPMPublication> publications = (List<PPMPublication>)Cache["PPMPublicationNamesCache"];
            if (publications == null)
                return;

            ddPublicationList.Items.Clear();

            string pubsallowed = (string)Session["PublicationsAllowed"];
            string[] publist = pubsallowed.Split(',');

            foreach (PPMPublication publication in publications)
            {
                string s = publication.Short_name;
                if (publication.Short_name != publication.Long_name)
                    s = publication.Short_name + " " + publication.Long_name;

                if (pubsallowed == "*")
                {
                    ddPublicationList.Items.Add(new Telerik.Web.UI.RadComboBoxItem(s, publication.Short_name));
                }
                else
                {
                    foreach (string sp in publist)
                    {
                        if (string.Equals(sp, publication.Long_name, StringComparison.InvariantCultureIgnoreCase) ||
                            string.Equals(sp, publication.Short_name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            ddPublicationList.Items.Add(new Telerik.Web.UI.RadComboBoxItem(s, publication.Short_name));
                            break;
                        }
                    }
                }
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
            lblEdition.Text = Global.rm.GetString("txtZone");

            lblPubdate.Text = Global.rm.GetString("txtPubDate2");
			lblComment.Text = Global.rm.GetString("txtComment");
			btnSavePlan.Text = Global.rm.GetString("txtSavePlan");
			btnSavePlan.ToolTip = Global.rm.GetString("txtTooltipSavePlan");
            lblSectionA.Text = Global.rm.GetString("txtSection") + " A";
            lblSectionB.Text = Global.rm.GetString("txtSection") + " B";
            lblSectionC.Text = Global.rm.GetString("txtSection") + " C";
            lblSectionD.Text = Global.rm.GetString("txtSection") + " D";
            lblSectionE.Text = Global.rm.GetString("txtSection") + " E";
            lblSectionF.Text = Global.rm.GetString("txtSection") + " F";
            lblSectionG.Text = Global.rm.GetString("txtSection") + " G";
            lblSectionH.Text = Global.rm.GetString("txtSection") + " H";
            lblSectionI.Text = Global.rm.GetString("txtSection") + " I";


			btnCancel.Text =  Global.rm.GetString("txtCancel");	
		
			btnDeletePlan.Text = Global.rm.GetString("txtDeletePlan"); 
			lblDeletePlan.Text = Global.rm.GetString("txtDeletePlanText");

            lblPageFormat.Text = Global.rm.GetString("txtPageFormat");

            lblCirculation.Text = Global.rm.GetString("txtCirculation");
            lblPressGroup.Text = Global.rm.GetString("txtPress");

            lblPaper.Text = Global.rm.GetString("txtPaper");

            RadNumericTextBoxPages1.Label = Global.rm.GetString("txtPages") + " ";
            RadNumericTextBoxPages2.Label = Global.rm.GetString("txtPages") + " ";
            RadNumericTextBoxPages3.Label = Global.rm.GetString("txtPages") + " ";
            RadNumericTextBoxPages4.Label = Global.rm.GetString("txtPages") + " ";
            RadNumericTextBoxPages5.Label = Global.rm.GetString("txtPages") + " ";
            RadNumericTextBoxPages6.Label = Global.rm.GetString("txtPages") + " ";
            RadNumericTextBoxPages7.Label = Global.rm.GetString("txtPages") + " ";
            RadNumericTextBoxPages8.Label = Global.rm.GetString("txtPages") + " ";
            RadNumericTextBoxPages9.Label = Global.rm.GetString("txtPages") + " ";

            lblPackageFiles.Text = Global.rm.GetString("txtPackageFiles");
            RadButtonSaveFile.Text = Global.rm.GetString("txtSaveFiles");
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
			this.ddPublicationList.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(this.ddPublicationList_SelectedIndexChanged);
			this.btnSavePlan.Click += new System.EventHandler(this.btnSavePlan_Click);
			this.btnCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
        #endregion

        void RadAsyncUpload1_FileUploaded(object sender, FileUploadedEventArgs e)
        {
            e.IsValid = true;
            return;
        }


        protected void RadButtonSaveFile_Click(object sender, EventArgs e)
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

                    DateTime selectedDateTime = (DateTime)dateChooserPubDate.SelectedDate;
                    title = string.Format("{0}_{1:0000}-{2:00}-{3:00}_{4}_", ddPublicationList.SelectedValue, selectedDateTime.Year, selectedDateTime.Month, selectedDateTime.Day, ddPressGroupList.SelectedValue) + title;
                    string destFile = physicalDestFolder + @"\" + title;

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
                    if ((string)Session["UploadedFiles"] != "")
                        Session["UploadedFiles"] = (string)Session["UploadedFiles"]  + ";";
                    Session["UploadedFiles"] = (string)Session["UploadedFiles"] + title;
                    

                    dt.Rows.Add(row);

                }
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
            else
            {
                GridView1.Visible = false;
                ltrNoResults.Visible = true;
            }
        }

        //--------------------------------------
        // Save plan to database
        //--------------------------------------

        private void btnSavePlan_Click(object sender, System.EventArgs e)
		{
            SavePPMPlan(false);
		}



        private void SavePPMPlan(bool overwriteconfirmed)
        {
            PPMDBaccess ppmdb = new PPMDBaccess();
            CCDBaccess db = new CCDBaccess();
            string errmsg = "";
            lblInfo.Text = "";
            lblError.Text = "";

            int totalPages = (int)RadNumericTextBoxPages1.Value + (int)RadNumericTextBoxPages2.Value +
                            (int)RadNumericTextBoxPages3.Value + (int)RadNumericTextBoxPages4.Value +
                            (int)RadNumericTextBoxPages5.Value + (int)RadNumericTextBoxPages6.Value +
                            (int)RadNumericTextBoxPages7.Value + (int)RadNumericTextBoxPages8.Value + (int)RadNumericTextBoxPages9.Value;

            if (totalPages == 0)
            {
                lblError.Text = Global.rm.GetString("txtNoSectionsDefined");
                lblError.ForeColor = Color.Red;
                return;
            }

            PPMPlan plan = new PPMPlan
            {
                Publication = ddPublicationList.SelectedValue,
                PressGroup = ddPressGroupList.SelectedValue,
                PageFormat = ddPageFormatList.SelectedValue,
                Paper = ddPaperList.SelectedValue,
                AllCommonSubeditions = true,
                Editions = ddEditionList.SelectedValue,
                Comment = txtComment.Text,
                Circulation = (int)RadNumericTextBoxCirculation.Value,
                Sections = "",
                TrimWidth = (int)RadNumericTextBoxTrimWidth.Value,
                TrimHeight = (int)RadNumericTextBoxTrimHeight.Value
            };

            if (plan.Comment.Length > 200)
                plan.Comment = plan.Comment.Substring(0, 200);

            if (RadNumericTextBoxPages1.Value > 0)
            {
                plan.Sections = string.Format("A:{0}", (int)RadNumericTextBoxPages1.Value);
            }
            if (RadNumericTextBoxPages2.Value > 0)
            {
                if (plan.Sections != "")
                    plan.Sections += ",";
                plan.Sections += string.Format("B:{0}", (int)RadNumericTextBoxPages2.Value);
            }
            if (RadNumericTextBoxPages3.Value > 0)
            {
                if (plan.Sections != "")
                    plan.Sections += ",";
                plan.Sections += string.Format("C:{0}", (int)RadNumericTextBoxPages3.Value);
            }
            if (RadNumericTextBoxPages4.Value > 0)
            {
                if (plan.Sections != "")
                    plan.Sections += ",";
                plan.Sections += string.Format("D:{0}", (int)RadNumericTextBoxPages4.Value);
            }
            if (RadNumericTextBoxPages5.Value > 0)
            {
                if (plan.Sections != "")
                    plan.Sections += ",";
                plan.Sections += string.Format("E:{0}", (int)RadNumericTextBoxPages5.Value);
            }
            if (RadNumericTextBoxPages6.Value > 0)
            {
                if (plan.Sections != "")
                    plan.Sections += ",";
                plan.Sections += string.Format("F:{0}", (int)RadNumericTextBoxPages6.Value);
            }
            if (RadNumericTextBoxPages7.Value > 0)
            {
                if (plan.Sections != "")
                    plan.Sections += ",";
                plan.Sections += string.Format("G:{0}", (int)RadNumericTextBoxPages7.Value);
            }
            if (RadNumericTextBoxPages8.Value > 0)
            {
                if (plan.Sections != "")
                    plan.Sections += ",";
                plan.Sections += string.Format("H:{0}", (int)RadNumericTextBoxPages8.Value);
            }
            if (RadNumericTextBoxPages9.Value > 0)
            {
                if (plan.Sections != "")
                    plan.Sections += ",";
                plan.Sections += string.Format("I:{0}", (int)RadNumericTextBoxPages9.Value);
            }

            DateTime selectedDateTime = (DateTime)dateChooserPubDate.SelectedDate;
            DateTime pubdate = new DateTime(selectedDateTime.Year, selectedDateTime.Month, selectedDateTime.Day, 0, 0, 0, 0);
            plan.PubDate = pubdate;

            int publicationID = Globals.GetIDFromName("PublicationNameCache", plan.Publication);

            int pressID = Globals.GetIDFromName((bool)Application["UsePressGroups"] ? "PressGroupNameCache" : "PressNameCache", plan.PressGroup);
            if ((bool)Application["UsePressGroups"])
                pressID = Globals.GetFirstPressIDFromPPressGroup(pressID);


            // To do - check if already in ControlCenter 
            // To do - check if already in PPM
            int ppmID = ppmdb.LookupPlan(plan.PressGroup, plan.Publication, plan.PubDate, out errmsg);
            if (ppmID > 0 && overwriteconfirmed == false)
            {
                existingProductionPrompt = 1;
                saveConfirm.Value = "0";
                return;
            }

            /*            if ((int)Session["MaxPlanPages"] > 0 && totalPages > (int)Session["MaxPlanPages"])
                        {
                            lblError.Text = Global.rm.GetString("txtMaxPlanPagesAllowed") + ": " + (int)Session["MaxPlanPages"];
                            lblError.ForeColor = Color.Red;
                            return;
                        }
                        */

            plan.UploadedFiles = (string)Session["UploadedFiles"];
            if (ppmdb.InsertPlan(plan, out errmsg) == false)
            {
                lblError.Text = "PPM.InsertPlan() - " + errmsg;
                lblError.ForeColor = Color.Red;
                return;
            }

            updateTree = 0;
            Session["RefreshTree"] = false;

            existingProductionPrompt = 0;
            saveConfirm.Value = "0";

            lblError.Text = Global.rm.GetString("txtPlanAdded");

            lblError.ForeColor = Color.Green;
            updateTree = 1;
            Session["RefreshTree"] = true;

            lblAddPagePlan.Visible = true;
            btnAddPlan.Visible = true;
            PanelMainActionButtons.Visible = true;

            PanelAddPlan.Visible = false;
           
            btnAddPlan.Visible = (bool)Application["HideAddPlanButton"] == false;
            lblAddPagePlan.Visible = (bool)Application["HideAddPlanButton"] == false;

        }
		


		protected void ddPublicationList_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
		{
			SetPressFromPublication();
            SetEditionsFromPublication();
            SetPaperFromPublication();
            SetPageFormatFromPublication();
            SetCirculationFromPublication();
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
