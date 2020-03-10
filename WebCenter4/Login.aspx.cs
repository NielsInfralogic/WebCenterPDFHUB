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
using System.Configuration;
using WebCenter4.Classes;
using System.Text;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Web.Security;
using System.Collections.Generic;

namespace WebCenter4
{
	/// <summary>
	/// Summary description for Login.
	/// </summary>
	public partial class Login : System.Web.UI.Page
	{
       
        public string sAdmin = "";

        protected static int MAXLOGINRETRIES = 3;
		private void Page_Load(object sender, System.EventArgs e)
		{
            sAdmin = ConfigurationManager.AppSettings["AdminEmail"];

            Session["mobiledevice"] = false;
            if (HiddenIOS.Value == "1") 
                Session["mobiledevice"] = true;

            if (HiddenX.Value != "" && HiddenY.Value != "")
            {
                Session["BrowserWidth"] = Globals.TryParse(HiddenX.Value, 0);
                Session["BrowserHeight"] = Globals.TryParse(HiddenY.Value, 0);
            }

		
			if (Request.QueryString["logout"] != null) 
			{
				string s = (string)Session["UserName"];
					
				if (s != null)
					if ((bool)Application["LogUserAccess"] == true)
						LogUserLogout(s);
			}
			
			if (!IsPostBack) 
			{

                imgLogo.Width = (int)Application["LogoWidth"];
                Session["culture"] = Global.culture;
                Session["language"] = Global.language;
                Session["encoding"] = Global.encoding;
               

				DateTime dtNow = DateTime.Now;
                ddLanguage.SelectedValue = Global.language;
                ddLanguage.Enabled = (bool)Globals.AllowSetLanguage;
                lblLanguage.Enabled = (bool)Globals.AllowSetLanguage;

                // Language from cookie?
                if (HiddenLang.Value != "")
                {
                    if (Globals.IsValidLanguage(HiddenLang.Value))
                    {
                        ddLanguage.SelectedValue = HiddenLang.Value;
                        Session["culture"] = Globals.GetCulture(ddLanguage.SelectedValue);
                        Session["language"] = ddLanguage.SelectedValue;
                        Session["encoding"] = Globals.GetEncoding(ddLanguage.SelectedValue);
                    }

                }

                SetLanguage();


                imgLogo.Width = Globals.LogoWidth;
                //lblDate.Text = dtNow.ToString("d");

                //if (Request.UserHostAddress != "::1")
                //    lblDate.Text = Server.HtmlEncode(Request.UserHostAddress) + "  " + Server.HtmlEncode(Request.UserHostName);

            

                if ((bool)Application["UseAD"] == false)
                {
                    lblDomain.Enabled = false;
                    txtDomain.Enabled = false;
                }


                Session["InitialLoad"] = true;

                Session["ADgroup"] = "";
				ViewState["Tries"] = "0";
                Session["TabSelected"] = 0;
				// Initialize all session variables
				Session["UserName"] = "";
				Session["IsAdmin"] = false;
				Session["PagesPerRow"] = 8;
                Session["PlatesPerRow"] = 4;
                Session["FlatPageSize"] = 100;
				Session["RefreshTime"] = 60;
                Session["RefreshTimeSaved"] = 60;
                Session["MayApprove"] = false;
				Session["MayReimage"] = false;
				Session["MayRelease"] = false;
                Session["MayUpload"] = true;
                Session["MayReprocess"] = true;

				Session["MayHardProof"] = true; 
				Session["MayFlatProof"] = true; 

				Session["ColumnOrder"] = "";

				// Tree element selected

                Session["TabSelected"] = 0;
                Session["RefreshTree"] = true;
                Session["PressRunHideOld"] = false;


                Session["SelectedPublication"] = "";
				Session["SelectedSection"] = "";
				Session["SelectedEdition"] = "";
				Session["SelectedIssue"] = "";
                Session["SelectedProduction"] = 0;
                Session["SelectedPress"] = "";
                Session["SelectedPublisher"] = "";
				DateTime t = new DateTime(1975,1,1,0,0,0);

                Session["SelectedPubDate"] = t;
                Session["CurrentProduct"] = "";
				Session["HideInputTime"] = false;
				Session["HideApproveTime"] = false;
				Session["HideOutputTime"] = false;
                Session["HideFinished"] = false;

				Session["MonitorLocation"] = "Default";

                Session["SelectedLogLocation"] = "";


                Session["DefaultPressID"] = 0;
				Session["DefaultPublicationID"] = 0;
				Session["SelectedPlanPress"] = "";
                Session["SelectedChannel"] = "";

                Session["PressRunPubDate"] = DateTime.Now;

				Session["IsMac"] = false;
                Session["SendMail"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["SendMail"]) == 1;
                Session["LogDisapprove"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["LogRejectEvent"]) == 1;
                Session["LogApprove"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["LogApproveEvent"]) == 1;
                Session["CloseZoomAfterApprove"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["CloseZoomAfterApprove"]) == 1;

				//Session["ShowOrdernumberInTree"] = Globals.ReadConfigBoolean("ShowOrdernumberInTree", false);
                Session["AllowHardproof"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["AllowHardproof"]) == 1;
				if ((bool)Session["AllowHardproof"] == false)
					Session["MayHardProof"] = false;

                Session["HideOld"] = false;

                Session["KeepExistingColors"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["KeepExistingColors"]) == 1;
                Session["KeepExistingApproval"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["KeepExistingApproval"]) == 1;
                Session["KeepExistingUnique"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["KeepExistingUnique"]) == 1;

				Session["DefaultHideDuplicates"] = Globals.ReadConfigBoolean("DefaultHideDuplicates", false);
				Session["RefreshTree"] = Globals.ReadConfigBoolean("RefreshTree", false);

                Session["UseHTML5"] = Globals.ReadConfigBoolean("UseHTML5", false);

				Session["UserSpecificViews"] = Globals.ReadConfigBoolean("UserSpecificViews", false);
				Session["UserView_ReadView"] = Globals.ReadConfigBoolean("UserView_ReadView", false);
				Session["UserView_ListView"] = Globals.ReadConfigBoolean("UserView_ListView", false);
				Session["UserView_FlatView"] = Globals.ReadConfigBoolean("UserView_FlatView", false);
				Session["UserView_PressView"] = Globals.ReadConfigBoolean("UserView_PressView", false);
				Session["UserView_PlanView"] = Globals.ReadConfigBoolean("UserView_PlanView", false);
				Session["UserView_LogView"] = Globals.ReadConfigBoolean("UserView_LogView", false);
				Session["UserView_ReportView"] = Globals.ReadConfigBoolean("UserView_ReportView", false);
                Session["UserView_UnknownFiles"] = Globals.ReadConfigBoolean("UserView_UnknownFiles", false);

                Session["HideCommonPages"] = Globals.ReadConfigBoolean("DefaultHideCommonPages", false);
                Session["HideCommon"] = Globals.ReadConfigBoolean("DefaultHideCommonPages", false);
                Session["HideCopies"] = Globals.ReadConfigBoolean("DefaultHideCopies", false);
                Session["SelectedAllCopies"] = Globals.ReadConfigBoolean("DefaultSelectedAllCopies", false);

                Session["SelectedAllSeps"] = Globals.ReadConfigBoolean("DefaultSelectedAllSeps", false);
		
				Session["SetCommentOnDisapproval"] = Globals.ReadConfigBoolean("SetCommentOnDisapproval", false);

				// Future filters
				Session["SelectedLocation"] = "";
				Session["SelectedStatus"] = -1;
				Session["SelectedApproval"] = -1;
				Session["SelectedHold"] = -1;
				Session["ChoosedPage"] = "";
				Session["LastError"] = "";
				Session["HideApproved"] = false;

				Session["ShowInactive"] = false;
                Session["Log1Selector"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["LogView1"]);
                Session["Log2Selector"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["LogView2"]);
                Session["Log3Selector"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["LogView3"]);

                Session["PublicationsAllowed"] = "*";
                Session["EditionsAllowed"] = "*";
                Session["SectionsAllowed"] = "*";
                Session["LocationsAllowed"] = "*";
                Session["PressesAllowed"] = "*";
                Session["PublishersAllowed"] = "*";

                Session["PubDateFilter"] = (bool)Session["HideOld"] ? DateTime.MaxValue : DateTime.MinValue;

                Session["ADgroup"] = "";
                Session["UserDomain"] = "";

                if (Request.QueryString["auto"] != null)
                {

                    if (Request.Form["product"] != null && Request.Form["pubdate"] != null)
                    {
                        string product = (string)Request.Form["product"];
                        string pubdate = (string)Request.Form["pubdate"];
                        if (product != "" && pubdate != "")
                        {
                            DateTime pubDate = Globals.ParsePubDate(pubdate);

                            string publicationName = Globals.LookupInputAliasShortToLong("Publication", product);
                            int publicationID = Globals.GetIDFromName("PublicationNameCache", publicationName);
                            if (publicationID > 0 && pubDate.Year > 2000)
                            {

                                CCDBaccess db = new CCDBaccess();
                                string errmsg = "";

                                int pressID = Globals.GetIDFromName("PressNameCache", (string)Session["SelectedPress"]);
                                //if ((bool)Application["UsePressGroups"])
                                //    pressID = Globals.GetIDFromName("PressGroupNameCache", (string)Session["SelectedPress"]);
                                int productionID = db.GetProductionIDAnyPress(publicationID, pubDate, ref pressID, out errmsg);

                                if (productionID > 0 && pressID > 0)
                                {
                                    Session["SelectedPublication"] = publicationName;
                                    Session["SelectedPubDate"] = pubDate;
                                    Session["SelectedPress"] = Globals.GetNameFromID("PressNameCache", pressID);
                                    Session["SelectedLocation"] = Globals.GetLocationFromPress(pressID);

                                    //if ((bool)Application["UsePressGroups"])
                                    //    Session["SelectedPress"] = Globals.GetNameFromID("PressGroupNameCache", pressID);
                                }

                            }

                        }

                    }



                    if ((Request.Form["name"] != null || Request.Form["username"] != null) && Request.Form["password"] != null)
                    {

                        string name = "";
                        if (Request.Form["name"] != null)
                            name =  (string)Request.Form["name"];
                        if (name == "" && Request.Form["username"] != null)
                            name = (string)Request.Form["username"];
                        string password = (string)Request.Form["password"];

                        if (name != "" && password != "")
                        {
                            txtUserName.Text = name;
                            txtPassword.Text = password;
                            bntLogin_Click(null, null);
                        }
                    }



                }

                if (Request.QueryString["username"] != null && Request.QueryString["password"] != null)
                {
                    string name = (string)Request.QueryString["username"];
                    string password = (string)Request.QueryString["password"];

                    if (name != "" && password != "")
                    {
                        txtUserName.Text = name;
                        txtPassword.Text = password;
                        bntLogin_Click(null, null);
                    }
                }

               
            }
        }

        private bool CheckForSQLInjection(string s)
        {
            if (s.IndexOf(';') != -1 || s.IndexOf('\'') != -1)
                return false;
            return true;
        }

        private void SetLanguage()
        {
            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            lblLoginHeader.Text = Global.rm.GetString("txtLogin");
            bntLogin.Text = Global.rm.GetString("txtLogin");
            lblUsername.Text = Global.rm.GetString("txtUserName");
            lblPassword.Text = Global.rm.GetString("txtPassword");
            lblDomain.Text = Global.rm.GetString("txtDomain");
            txtDomain.Text = (string)Application["ADdomain"];
            lblLanguage.Text = Global.rm.GetString("txtLanguage");

            //Hyperlink1.Text = Global.rm.GetString("txtContactAdmin");
            //	lnkNewPassword.Text = Global.rm.GetString("txtChangePassword");
            lnkNotes.Text = Global.rm.GetString("txtNotes");
            RequiredFieldValidator1.Text = Global.rm.GetString("txtMustHavePassword");
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
			this.bntLogin.Click += new System.EventHandler(this.bntLogin_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
        #endregion

        protected void bntLogin_Click(object sender, System.EventArgs e)
		{
            //ExamineDatabaseVersion();
            bool adAutheticationOK = true;
            bool adAuthenticationBypassNormalLogin = false;

            if ((bool)Globals.AllowSetLanguage)
            {
                string language = ddLanguage.SelectedValue;
                Session["culture"] = Globals.GetCulture(language);
                Session["language"] = language;
                Session["encoding"] = Globals.GetEncoding(language);

               
            }
            if (CheckForSQLInjection(txtUserName.Text.Trim()) == false)
            { 
                lblStatus.Text = "Illegal user name format";
                return;
            }

            if (CheckForSQLInjection(txtPassword.Text.Trim()) == false)
            {
                lblStatus.Text = "Illegal password format";
                return;
            }

            if (CheckForSQLInjection(txtDomain.Text.Trim()) == false)
            {
                lblStatus.Text = "Illegal domain name format";
                return;
            }

            if ((bool)Application["DisableAdminUser"] && string.Compare(txtUserName.Text.Trim(), "Admin", true) == 0)
            {
                lblStatus.Text = "Admin login is disabled";
                return;
            }

            Session["UserDomain"] = "";

            if ((bool)Application["UseAD"] && (string)Application["ADpath"] != "")
            {
                adAutheticationOK = false;
                LdapAuthentication adAuth = new LdapAuthentication((string)Application["ADpath"]);

                Session["ADgroup"] = "";
                Session["UserDomain"] = txtDomain.Text;

                if ((bool)Application["SimpleAD"])
                {
                    string ADgroup = "";
                    if (adAuth.IsAuthenticatedSimple(txtDomain.Text, txtUserName.Text, txtPassword.Text, ref ADgroup) == true)
                    {
                        adAutheticationOK = true;
                        adAuthenticationBypassNormalLogin = true;

                        Global.logging.WriteLog("User " + txtUserName.Text + " is member of group(s) " + ADgroup);
                        Session["ADgroup"] = ADgroup;
                    }
                    else                          
                        lblStatus.Text = "AD Authentication did not succeed. Check user name and password.";

                    // SIMULATE
                  //  Session["ADgroup"] = "ad gruppe x";
                  //  adAutheticationOK = true;
                  //  adAuthenticationBypassNormalLogin = true;

                }
                else
                {

                    try
                    {
                        if (adAuth.IsAuthenticated(txtDomain.Text, txtUserName.Text, txtPassword.Text) == true)
                        {
                            string groups = adAuth.GetGroups();

                            //Create the ticket, and add the groups.
                            bool isCookiePersistent = (bool)Application["ADpersistantcookie"];
                            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1,
                                      txtUserName.Text, DateTime.Now, DateTime.Now.AddMinutes(60), isCookiePersistent, groups);

                            //Encrypt the ticket.
                            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

                            //Create a cookie, and then add the encrypted ticket to the cookie as data.
                            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                            if (true == isCookiePersistent)
                                authCookie.Expires = authTicket.Expiration;

                            //Add the cookie to the outgoing cookies collection.
                            Response.Cookies.Add(authCookie);

                            //You can redirect now.
                            //Response.Redirect(FormsAuthentication.GetRedirectUrl(txtUsername.Text, false));
                            adAutheticationOK = true;
                            adAuthenticationBypassNormalLogin = true;

                        }
                        else
                        {
                            lblStatus.Text = "AD Authentication did not succeed. Check user name and password.";
                        }
                    }
                    catch (Exception ex)
                    {
                        lblStatus.Text = "Error authenticating. " + ex.Message;
                    }
                }
            }

            if ((bool)Session["mobiledevice"])
                Session["UseHTML5"] = true;            
            
            Session["UserName"] = "";
			Session["IsAdmin"] = false;
			Session["IsSuperUser"] = false;


            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            // Check the user name.

            int nTries = Globals.TryParse((string)ViewState["Tries"], 0) + 1;

            ViewState["Tries"] = nTries.ToString();
			
            if (adAutheticationOK == false)
            {
                if (nTries > MAXLOGINRETRIES)
                    Response.Redirect("Denied.htm");
                return;
            }
			bool bIsAdmin  = false;
			string IPrange = "";
			bool bIsSuperUser = false;

			CCDBaccess db = new CCDBaccess();
            string errmsg = "";

			string sUserName = txtUserName.Text;
			string sPassWord = txtPassword.Text;

			string illegalchars = "\'\",";
			

			if (sUserName.IndexOfAny(illegalchars.ToCharArray()) != -1 || sPassWord.IndexOfAny(illegalchars.ToCharArray()) != -1 )
			{
				lblStatus.Text = Global.rm.GetString("txtUserNotFound");
				//UpdateUserLog("Invalid user name", "Attempt number: " + sTries);
				if (nTries > MAXLOGINRETRIES) 
					Response.Redirect("Denied.htm");
				return;
			}


            // AD authentication ok - if user does not exists - create it and inherent from default user..
            if ((bool)Application["UseAD"] && (bool)Application["AutoCreateADuser"] && adAutheticationOK && (string)Session["ADgroup"] != "" /*&& db.UserExists(txtUserName.Text, out errmsg) == 0*/)
            {
                List<string> adGroups = CreateADGroupList((string)Session["ADgroup"]);
                List<string> finalAdminGroupList = new List<string>();
                // 1: Match to any existing AdminGroups?
                foreach (string adGroup in adGroups)
                {
                    if (db.HasAdministrativeGroup(adGroup, out errmsg) > 0)
                        finalAdminGroupList.Add(adGroup);
                }
                // 2. Find default user group
                string userGroupName = "";
                foreach (string adminGroup in finalAdminGroupList)
                {
                    db.GetAdminGroupUserGroupRelations(adminGroup, ref userGroupName, out errmsg);
                    if (userGroupName != "")
                        break;
                }
                string finalAdminGroupListString = Globals.StringListToCSVString(finalAdminGroupList);
                int userGroupID = db.GetUserGroupID(userGroupName, out errmsg);
                if (finalAdminGroupList.Count > 0 && userGroupID > 0 && db.GetUserGroupID(userGroupName, out errmsg) > 0)
                {
                    // bingo!
                    if (db.UserExists(txtUserName.Text, out errmsg) == 0)
                    {
                        if (db.CreateUser(txtUserName.Text, userGroupID, finalAdminGroupList, out errmsg) == true)
                        {
                            Global.logging.WriteLog("Auto-created user: " + txtUserName.Text + "   User group: " + userGroupName + "   Admin group(s): " + finalAdminGroupListString);
                        }
                        else
                            Global.logging.WriteLog("ERROR: CreateUser() - " + errmsg);
                    }
                    else
                    {
                        // user exists - adjust to (ne) usergroup and admin groups
                        if (db.ChangeUserGroupForExistingUser(txtUserName.Text, userGroupID, out errmsg) == true)
                        {
                            Global.logging.WriteLog("Changed user: " + txtUserName.Text + "  New user group: " + userGroupName);
                        }
                        else
                            Global.logging.WriteLog("ERROR: ChangeUserGroupForExistingUser() - " + errmsg);

                        // user exists - adjust to (ne) usergroup and admin groups
                        if (db.ChangeAdminGroupForExistingUser(txtUserName.Text, finalAdminGroupList, out errmsg) == true)
                        {
                            Global.logging.WriteLog("Changed user: " + txtUserName.Text + "  New admin group(s): " + finalAdminGroupListString);
                        }
                        else
                            Global.logging.WriteLog("ERROR: ChangeAdmGroupsForExistingUser() - " + errmsg);

                    }

                }
                // User does now exist..
            }


            int ret = db.PasswordValidation(txtUserName.Text, txtPassword.Text, out bIsAdmin, out IPrange, out bIsSuperUser, adAuthenticationBypassNormalLogin);
			if (ret == -1)
			{
				lblStatus.Text = "ERROR - cannot connect to database!";
			}
			else if (ret == 0)
			{				
				lblStatus.Text = Global.rm.GetString("txtUserNotFound");
				//UpdateUserLog("Invalid user name", "Attempt number: " + sTries);
				if (nTries > MAXLOGINRETRIES) 
					Response.Redirect("Denied.htm");
			}
			else if (ret == 1)
			{
				lblStatus.Text = Global.rm.GetString("txtPasswordNotMatch");
				//UpdateUserLog("Invalid password", "Attempt number: " + sTries + ", Password supplied: " + txtPassword.Text);
				if (nTries > MAXLOGINRETRIES) 
					Response.Redirect("Denied.htm");
			}
			else if (ret == 2)
			{
				lblStatus.Text = Global.rm.GetString("txtAccountDisabled");
				//UpdateUserLog("Invalid password", "Attempt number: " + sTries + ", Password supplied: " + txtPassword.Text);
				if (nTries > MAXLOGINRETRIES) 
					Response.Redirect("Denied.htm");
			}
			else  
			{
                if ((bool)Application["UseAdminGroups"] && (bool)Application["UseAD"] && (string)Session["ADgroup"] != "")
                {
                    bool foundAdmGroup = false;
                    List<string> adgroupList = CreateADGroupList((string)Session["ADgroup"]);
                    foreach (string adm in adgroupList)
                    {
                        if (db.HasAdministrativeGroup(adm, out errmsg) == 1)
                        {
                            foundAdmGroup = true;
                            break;
                        }
                    }
                    if (foundAdmGroup == false)
                    {
                        lblStatus.Text = Global.rm.GetString("txtUserHasNoAdmgroup");
                        //UpdateUserLog("Invalid password", "Attempt number: " + sTries + ", Password supplied: " + txtPassword.Text);
                        if (nTries > MAXLOGINRETRIES)
                            Response.Redirect("Denied.htm");

                        return;

                    }

                }

				if (IPrange != "") 
				{
					bool isInRange = false;
					string[] sranges = IPrange.Split(';');
					foreach (string range in sranges)
					{
						string myrange = range.Trim();
						if (myrange.IndexOf("-") == -1)
						{	// single address
							if (Request.UserHostAddress == myrange)
							{
								isInRange = true;
								break;
							}
						}
						else
						{ // address range
							string[] s = myrange.Split('-');
							if (s.Length < 2)
								continue;
							Int64 incoming = IPstring2Int64(Request.UserHostAddress);
							if (incoming == 0)
								continue;
							Int64 startaddr = IPstring2Int64(s[0]);
							if (startaddr == 0)
								continue;
							Int64 endaddr = IPstring2Int64(s[1]);
							if (endaddr == 0)
								continue;

							if (incoming >= startaddr && incoming <= endaddr) 
							{
								isInRange = true;
								break;
							}
						}
					}
					if (isInRange == false)
					{
						lblStatus.Text = Global.rm.GetString("txtIPoutofrange");
						//UpdateUserLog("Invalid password", "Attempt number: " + sTries + ", Password supplied: " + txtPassword.Text);
						if (nTries > MAXLOGINRETRIES) 
							Response.Redirect("Denied.htm");

						return;
					}
				}

				Session["UserName"] = txtUserName.Text;
				Session["IsAdmin"] = bIsAdmin;		
				Session["IsSuperUser"] = bIsSuperUser;
				
				if ((bool)Application["LogUserAccess"] == true)
					LogUserLogin(txtUserName.Text);

                if ((bool)Application["FieldExists_UserNames_LastLoginTime"])
                    db.RegisterUserNameLoginTime(txtUserName.Text, out errmsg);

                LoadUserProfileAndRedirect();
			
			}
            // Keep track of tries.
        }

        private List<string> CreateADGroupList(string ADgroups)
        {
            List<string> adgroupList = new List<string>();
            if (ADgroups != "")
            {
                string ss = ADgroups;
                string[] ADgroupList = ss.Split('|');
                Global.logging.WriteLog("ADGrouplognames:");
                foreach (string s in ADgroupList)
                {
                    if (s.Trim() != "")
                    {
                        adgroupList.Add(s.Trim());
                        Global.logging.WriteLog(s.Trim());
                    }
                }
                Global.logging.WriteLog(".");
            }
            return adgroupList;
        }

        private Int64 IPstring2Int64(string s)
		{
			string[] saddr = s.Split('.');
			if (saddr.Length != 4)
				return (Int64)0;

			return Int64.Parse(saddr[0]) * 256 * 256 * 256 + Int64.Parse(saddr[1]) * 256 * 256 + Int64.Parse(saddr[2]) * 256 + Int64.Parse(saddr[3]);
		}


		private void LogUserLogin(string userName)
		{
			string errMsg = "";
			CCDBaccess db = new CCDBaccess();
			db.InsertUserHistory(userName,1, "",out errMsg);
		}

		private void LogUserLogout(string userName)
		{
			string errMsg = "";
			CCDBaccess db = new CCDBaccess();
			db.InsertUserHistory(userName,0,"",out errMsg);
		}

        private void LoadUserProfileAndRedirect()
        {
            string errmsg = "";
            // User state
            CCDBaccess db = new CCDBaccess();

            //Session["UserName"]  = "user";
            string fullUserName;
            string email;
            int nPagesPerRow = 8;
            int nRefreshTime = 60;
            int nPlatesPerRow = 4;
            bool bMayApprove = true;
            bool bMayReimage = true;
            bool bMayRunProduction = false;
            bool bMayHardProof = false;
            bool bMayFlatProof = false;
            bool bMayUpload = true;
            bool bMayReprocess = true;

            bool bMayChangeColor = false;
            bool bMayDeleteProduction = false;
            bool bMayConfigure = false;
            int nMaxPlanPages = 0;

            string columnOrder = "";
            int customerID = 0;
            int defaultPressID = 0;
            int defaultPublicationID = 0;
            bool bHideOld = false;
            string userGroup = "";

            string defaultPublisher = Globals.GetNameFromID("PublisherNameCache", 1);

            bool ok = db.GetUserProfile((string)Session["UserName"], out fullUserName, out email, out  nPagesPerRow, out  nRefreshTime,
                out bMayApprove, out bMayReimage, out bMayRunProduction, out nPlatesPerRow, out columnOrder,
                out  customerID, out  defaultPressID, out  defaultPublicationID, out bMayHardProof, out bMayFlatProof,
                out  bMayChangeColor, out  bMayDeleteProduction, out  bMayConfigure, out nMaxPlanPages, out bHideOld, out bMayUpload, out bMayReprocess,
                out userGroup, out  errmsg);
            if (ok)
            {
                //Session["IsAdmin"] = true;
                Session["PagesPerRow"] = nPagesPerRow;
                if ((int)Session["PagesPerRow"] == 0)
                    Session["PagesPerRow"] = 8;

                if ((bool)Session["mobiledevice"])
                {
                    if ((int)Session["PagesPerRow"] >= 4)
                        Session["PagesPerRow"] = 4;
                }

                Session["PlatesPerRow"] = nPlatesPerRow;
                if ((int)Session["PlatesPerRow"] == 0 || (int)Session["PlatesPerRow"] > 24)
                    Session["PlatesPerRow"] = nPagesPerRow / 2;
                Session["RefreshTime"] = nRefreshTime;
                Session["RefreshTimeSaved"] = nRefreshTime;
                Session["MayApprove"] = bMayApprove;
                Session["MayReimage"] = bMayReimage;
                Session["MayKillColor"] = bMayChangeColor;
                Session["MayRunProducts"] = bMayRunProduction;

                Session["MayRelease"] = (bool)Application["LocationIsPress"] ? bMayApprove : bMayReimage;
                Session["AllowPageDelete"] = bMayDeleteProduction;
                Session["MayUpload"] = bMayUpload;
                Session["MayReprocess"] = bMayReprocess;

                if (columnOrder == " ")
                    columnOrder = "";

             //   Session["ColumnOrder"] = columnOrder;	// Not currently used...


                if ((bool)Session["AllowHardproof"])
                    Session["MayHardProof"] = bMayHardProof;
                else
                    Session["MayHardProof"] = false;

                Session["CustomerID"] = customerID;

                Session["DefaultPressID"] = defaultPressID;
                Session["DefaultPublicationID"] = defaultPublicationID;

                Session["MaxPlanPages"] = nMaxPlanPages;
                Session["HideOld"] = (bool)Application["HideOldProducts"];//bHideOld;
                Session["PubDateFilter"] = (bool)Session["HideOld"] ? DateTime.MaxValue : DateTime.MinValue;
            }
            else
            {
                // Defaults
                Session["IsAdmin"] = false;
                Session["IsSuperUser"] = false;
                Session["PagesPerRow"] = 8;
                Session["PlatesPerRow"] = 4;
                Session["RefreshTime"] = 60;
                Session["MayApprove"] = false;
                Session["MayReimage"] = false;
                Session["MayKillColor"] = false;
                Session["MayRunProducts"] = false;
                Session["MayDeleteProducts"] = false;
                Session["AllowPageDelete"] = false;
                Session["ColumnOrder"] = "";
                Session["CustomerID"] = 0;
                Session["MayRelease"] = false;
                Session["MaxPlanPages"] = 0;
                Session["MayUpload"] = true;
                Session["MayReprocess"] = true;
                Session["HideOld"] = false;
            }


            if ((bool)Session["IsAdmin"] || ((bool)Session["IsSuperUser"] && (bool)Application["SuperUserMaySeeAll"]))
            {
                Session["PublicationsAllowed"] = "*";
                Session["EditionsAllowed"] = "*";
                Session["SectionsAllowed"] = "*";
                Session["LocationsAllowed"] = "*";
                Session["PressesAllowed"] = "*";
                Session["PublishersAllowed"] = "*";
            }
            else if ((bool)Application["UseAdminGroups"] && db.TableExists("AdmGroupUsers", out errmsg) == 1)
            {
                Session["EditionsAllowed"] = "*";
                Session["SectionsAllowed"] = "*";
                Session["LocationsAllowed"] = "*";
                Session["PressesAllowed"] = "*";
                Session["PublishersAllowed"] = "*";

                DataTable dt;
                if ((string)Session["ADgroup"] != "")
                {
                    List<string> ADgroupList = CreateADGroupList((string)Session["ADgroup"]);
                    string sPublicationList = "";
                    Session["PublicationsAllowed"] = "None";
                    foreach (string grp in ADgroupList)
                    {
                        Global.logging.WriteLog("Testing rights for AD group " + grp+" ..");

                        dt = db.GetPublicationsInAdmGroup(grp.Trim(), "UserPublications", out errmsg);
                        if (dt != null && dt.HasErrors == false)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                if (sPublicationList != "")
                                    sPublicationList += ",";
                                sPublicationList += (string)row["Publication"];
                            }
                        }
                    }
                    Session["PublicationsAllowed"] = sPublicationList;
                    Global.logging.WriteLog("Final pub-list fro AD groups:" + sPublicationList + " ..");
                    Global.logging.WriteLog("User group:" + userGroup);
               }
                else
                {
                    dt = db.GetUserPublicationsAdmGroup((string)Session["UserName"], "UserPublications", out errmsg);

                    if (dt != null && dt.HasErrors == false)
                    {
                        string sPublicationList = "";
                        foreach (DataRow row in dt.Rows)
                        {
                            if (sPublicationList != "")
                                sPublicationList += ",";
                            sPublicationList += (string)row["Publication"];
                        }
                        Session["PublicationsAllowed"] = sPublicationList;
                    }
                    else
                    {
                        Session["PublicationsAllowed"] = "None";
                    }
                }

                if ((string)Session["ADgroup"] == "")
                {
                    ArrayList al3 = db.GetUserLocationsAdmGroup((string)Session["UserName"], out errmsg);
                    if (errmsg == "" && al3.Count > 0)
                    {
                        string sLocationList = "";
                        foreach (string s in al3)
                        {
                            if (sLocationList != "")
                                sLocationList += ",";
                            sLocationList += s;
                        }
                        Session["LocationsAllowed"] = sLocationList;
                    }
                    else
                    {
                        Session["LocationsAllowed"] = "";
                    }
                }


            }
            else
            {
                DataTable dt = db.GetUserPublications((string)Session["UserName"], "UserPublications", out errmsg);
                if (dt != null && dt.HasErrors == false)
                {
                    string sPublicationList = "";
                    foreach (DataRow row in dt.Rows)
                    {
                        if (sPublicationList != "")
                            sPublicationList += ",";
                        sPublicationList += (string)row["Publication"];
                    }
                    Session["PublicationsAllowed"] = sPublicationList;
                }
                else
                {
                    Session["PublicationsAllowed"] = "None";
                }

                ArrayList al = db.GetUserEditions((string)Session["UserName"], out errmsg);
                if (errmsg == "")
                {
                    string sEditionList = "";
                    foreach (string s in al)
                    {
                        if (sEditionList != "")
                            sEditionList += ",";
                        sEditionList += s;
                    }
                    Session["EditionsAllowed"] = sEditionList;
                }
                else
                {
                    Session["EditionsAllowed"] = "";
                }

                ArrayList al2 = db.GetUserSections((string)Session["UserName"], out errmsg);
                if (errmsg == "")
                {
                    string sSectionList = "";
                    foreach (string s in al2)
                    {
                        if (sSectionList != "")
                            sSectionList += ",";
                        sSectionList += s;
                    }
                    Session["SectionsAllowed"] = sSectionList;
                }
                else
                {
                    Session["SectionsAllowed"] = "";
                }

                Session["LocationsAllowed"] = "";
                if (Globals.GetCacheRowCount("LocationNameCache") > 1)
                {
                    ArrayList al3 = db.GetUserLocations((string)Session["UserName"], out errmsg);
                    if (errmsg == "" && al3.Count > 1)      // !!!!!
                    {
                        string sLocationList = "";
                        foreach (string s in al3)
                        {
                            if (sLocationList != "")
                                sLocationList += ",";
                            sLocationList += s;
                        }
                        Session["LocationsAllowed"] = sLocationList;
                    }
                }

                if ((bool)Application["TableExists_UserPresses"] == false)
                {
                    Session["PressesAllowed"] = "";
                } 
                else
                {
                    ArrayList al4 = db.GetUserPresses((string)Session["UserName"], out errmsg);
                    if (errmsg == "" && al4.Count > 0)
                    {
                        string sPressList = "";
                        foreach (string s in al4)
                        {
                            if (sPressList != "")
                                sPressList += ",";
                            sPressList += s;
                        }
                        Session["PressesAllowed"] = sPressList;
                    }
                    else
                    {
                        Session["PressesAllowed"] = "";
                    }
                }

                if ((bool)Application["TableExists_UserPublishers"] == false)
                {
                    Session["PublishersAllowed"] = "";
                }
                else
                {
                    ArrayList al4 = db.GetUserPublishers((string)Session["UserName"], out errmsg);
                    if (errmsg == "" && al4.Count > 0)
                    {
                        defaultPublisher = (string)al4[0];
                        string sList = "";
                        foreach (string s in al4)
                        {
                            if (sList != "")
                                sList += ",";
                            sList += s;
                        }
                        Session["PublishersAllowed"] = sList;
                    }
                    else
                    {
                        Session["sList"] = "";
                    }
                }


            }





            Session["LastError"] = "";

            // General preferences

            // Tab view seleted
            Session["TabSelected"] = 0;

            // Tree element selected
           
            Session["SelectedSection"] = "";
            Session["SelectedEdition"] = "";
            Session["SelectedIssue"] = "";

            //Session["SelectedProduction"] = 0;

            // Future filters
            if ((string)Session["SelectedPress"] == "") {
                if ((int)Session["DefaultPressID"] > 0)
                    Session["SelectedPress"] = (bool)Application["UsePressGroups"] ? Globals.GetNameFromID("PressGroupNameCache", (int)Session["DefaultPressID"]) : Globals.GetNameFromID("PressNameCache", (int)Session["DefaultPressID"]);
            }
            if ((bool)Application["UseChannels"])
                Session["SelectedPublisher"] = defaultPublisher;

            //Session["SelectedLocation"] = "";
            Session["SelectedStatus"] = -1;
            Session["SelectedApproval"] = -1;
            Session["SelectedHold"] = -1;
//            Session["SelectedAllCopies"] = 0;
  //          Session["SelectedAllSeps"] = false;

            bool showFlatView = false;
            bool showListView = false;
            bool showReadView = false;
            bool showPressView = false;
            bool showPlanView = false;
            bool showLogView = false;
            bool showReportView = false;
            bool showUnknownView = false;

            Session["UserSpecificViews"] = false;

            ok = db.GetUserProfileEx((string)Session["UserName"], out  showFlatView, out  showListView, out  showReadView,
                                        out  showPressView, out  showPlanView, out  showLogView, out  showReportView, out showUnknownView, out  errmsg);
            if (ok)
            {
                Session["UserSpecificViews"] = true;
                Session["UserView_ReadView"] = showReadView;
                Session["UserView_ListView"] = showListView;
                Session["UserView_FlatView"] = showFlatView;
                Session["UserView_PressView"] = showPressView;
                Session["UserView_PlanView"] = showPlanView;
                Session["UserView_LogView"] = showLogView;
                Session["UserView_ReportView"] = showReportView;
                Session["UserView_UnknownFiles"] = showUnknownView;
            }

            bool useHTML5 = false;
            if ((bool)Application["FieldExists_UserNames_UseHTML5"])
                db.GetUserDefaultViewer((string)Session["UserName"], out useHTML5, out errmsg);
            Session["UseHTML5"] = useHTML5;

/*            if (HiddenFlash.Value == "0" && (bool)Application["AutoDetectFlash"])
            {
                Session["UseHTML5"] = false;
                Global.logging.WriteLog("Flash detection reported FALSE - forcing HTML5 view for user " + (string)Session["UserName"]);

            }
*/

            Global.logging.WriteLog("User " + (string)Session["UserName"] + " - allowed presses " + (string)Session["PressesAllowed"]);
            Global.logging.WriteLog("Mobile device: " + (bool)Session["mobiledevice"] + "  HTML5: " + (bool)Session["UseHTML5"]);

            Response.Redirect("FrameSet.aspx");
        }

        protected void ddLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((bool)Globals.AllowSetLanguage)
            {
                string language = ddLanguage.SelectedValue;
                Session["culture"] = Globals.GetCulture(language);
                Session["language"] = language;
                Session["encoding"] = Globals.GetEncoding(language);
                HiddenLang.Value = language;
            }
            SetLanguage();
        }
    }
}