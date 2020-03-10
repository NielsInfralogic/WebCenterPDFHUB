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
using System.Text;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Configuration;
using WebCenter4.Classes;
using Telerik.Web.UI;
    
namespace WebCenter4
{
	/// <summary>
	/// Summary description for tree.
	/// </summary>
	public partial class ProductionTree : System.Web.UI.Page
	{

		public bool updateMain;
        public bool forcedreload;
		public string nodeClicked;

        DataTable dbTreeStateTable = null;

        private void Page_Load(object sender, System.EventArgs e)
		{
            forcedreload = false;
            
            if ((string)Session["UserName"] == "")
				Response.Redirect("Denied.htm");

			updateMain = false;
			if (!Page.IsPostBack)
			{
                CreatePublisherDropDownTelerik();
                CreateChannelDropDownTelerik();                
				SetLanguage();
                //SetSplitButtonValue();

				nodeClicked = "";

				PopulateTree(true, false,false);
				DefaultTreeExpansion();
				Session["RefreshTree"] = false;				
			}

			if ((bool)Session["RefreshTree"])
			{
				SetLanguage();
				PopulateTree(false,false,false);
				DefaultTreeExpansion();
				Session["RefreshTree"] = false;
			}

			// Retrieve screen size from client side cookies
            int h = 0;
           // bool setTreeHeight = true;
            if (HiddenY == null)
                Global.logging.WriteLog("HiddenY==null !");
            else {
                if (HiddenY.Value != "")
                {
                    h = Globals.TryParse(HiddenY.Value, 0);
                    Session["BrowserHeight"] = h;
                }
            }
   
            if (h <= 0 && Session["BrowserHeight"] != null)
            {
             //   setTreeHeight = false;
                h = (int)Session["BrowserHeight"] > 0 ? (int)Session["BrowserHeight"] : 600 ;     // Subtract tollbar heights and menu height
            }

            if (h < 300)
                h = 300;

            if (RadTreeView1 != null)
                RadTreeView1.Height = h - 6; //- 6;


            if ((bool)Session["InitialLoad"] == true)
            {
                Session["InitialLoad"] = false;
                // force reload..
                forcedreload = true;
            }


		}


        private void CreatePublisherDropDownTelerik()
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue("Item1");
            if (item == null)
                return;
            DropDownList DropDownList1 = (DropDownList)item.FindControl("PressSelector");
            if (DropDownList1 == null)
                return;
            DropDownList1.Items.Clear();

            string publishersallowed = (string)Session["PublishersAllowed"];
            string[] publisherlist = publishersallowed.Split(',');
            DataTable dt = (DataTable)Cache["PublisherNameCache"];

            foreach (DataRow row in dt.Rows)
            {
                string name = (string)row["Name"];
                bool found = false;
                foreach (string sp in publisherlist)
                {
                    if (name == sp)
                    {
                        found = true;
                        break;
                    }
                }

                if (found || publishersallowed == "*" || publishersallowed == "")
                {
                    DropDownList1.Items.Add(name); // 0
                }
            }


            if ((string)Session["SelectedPublisher"] == "" || (string)Session["SelectedPublisher"] == "*")
            {
                DropDownList1.SelectedIndex = 0;
                Session["SelectedPress"] = DropDownList1.SelectedValue;
            }
            else// if (selectedFound)
                DropDownList1.SelectedValue = (string)Session["SelectedPublisher"];

        }

        private void CreateChannelDropDownTelerik()
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar2.FindItemByValue("Item2");
            if (item == null)
                return;
            DropDownList DropDownList1 = (DropDownList)item.FindControl("ChannelSelector");
            if (DropDownList1 == null)
                return;
            DropDownList1.Items.Clear();

            DataTable dt = (DataTable)Cache["ChannelNameCache"];
            DropDownList1.Items.Add("Alle");
            foreach (DataRow row in dt.Rows)
            {
                string name = (string)row["Name"];
               
                DropDownList1.Items.Add(name); // 0
            }


            if ((string)Session["SelectedChannel"] == "" || (string)Session["SelectedChannel"] == "*")
            {
                DropDownList1.SelectedIndex = 0;
            }
            else// if (selectedFound)
                DropDownList1.SelectedValue = (string)Session["SelectedChannel"];

        }

        public void OnSelChangePublisher(object sender, System.EventArgs e) 
		{

            /*            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue("Item1");
                        if (item == null)
                            return;
                        DropDownList1 = (DropDownList)item.FindControl("PressSelector");
                        if (DropDownList1 == null)
                            return;
              */
            System.Web.UI.WebControls.DropDownList dropdown = (System.Web.UI.WebControls.DropDownList)sender;
			string s = dropdown.SelectedItem.Text;
            if ((bool)Application["LocationIsPress"] || (bool)Application["ShowPressSelector"])
			{
				Session["SelectedPress"] = s;
				Session["SelectedLocation"] = Globals.GetLocationFromPress(Globals.GetIDFromName("LocationNameCache", s));
			}
            else if ((bool)Application["UsePressGroups"])
            {
                Session["SelectedPress"] = s;
                Session["SelectedLocation"] = (s == "All") ? "" : s;
            }
            else
            {
                Session["SelectedLocation"] = (s == "All") ? "" : s;
            }

            if ((bool)Application["UseChannels"])
            {
                Session["SelectedPublisher"] = s;
            }

            if ((DateTime)Session["PubDateFilter"] != DateTime.MinValue && (DateTime)Session["PubDateFilter"] != DateTime.MaxValue)
                Session["PubDateFilter"] = (bool)Session["HideOld"] ? DateTime.MaxValue : DateTime.MinValue;

			PopulateTree(false,true,true);
			DefaultTreeExpansion();
            updateMain = true;
            Session["SelectedChannel"] = "";
		}

        public void OnSelChangeChannel(object sender, System.EventArgs e)
        {
            System.Web.UI.WebControls.DropDownList dropdown = (System.Web.UI.WebControls.DropDownList)sender;
            string s = dropdown.SelectedItem.Text;
            Session["SelectedChannel"] = (s == "All") ? "" : s;

            if ((DateTime)Session["PubDateFilter"] != DateTime.MinValue && (DateTime)Session["PubDateFilter"] != DateTime.MaxValue)
                Session["PubDateFilter"] = (bool)Session["HideOld"] ? DateTime.MaxValue : DateTime.MinValue;

            PopulateTree(false, true,true);
            DefaultTreeExpansion();
            updateMain = true;
        }

        private void DefaultTreeExpansion()
		{
            int nTreeLevel = (int)Application["DefaultTreeExpansion"];
            bool showAllEditions = (bool)Application["DefaultTreeExpansionShowAllEditions"];
            bool showAllSections = (bool)Application["DefaultTreeExpansionShowAllSections"];
            RadTreeView1.CollapseAllNodes();
            if (nTreeLevel >= 1)
                RadTreeView1.Nodes[0].Expanded = true;

            int n = RadTreeView1.Nodes.Count;
			if (nTreeLevel >= 2)
			{
                foreach (RadTreeNode node1 in RadTreeView1.Nodes)
				{
					if (node1.Nodes.Count > 0)
					{
                        foreach (RadTreeNode node2 in node1.Nodes)
						{
                            node2.Expanded = true;

                            if (showAllEditions)
                            {
                                foreach (RadTreeNode node3 in node2.Nodes)
                                {
                                    if (node3.Nodes.Count > 1)
                                        node3.Expanded = true;

                                    if (showAllSections)
                                    {
                                        foreach (RadTreeNode node4 in node3.Nodes)
                                        {
                                            if (node4.Nodes.Count > 1)
                                                node4.Expanded = true;
                                        }
                                    }
                                }
                            }
						}
					}
				}
			}


			if (nTreeLevel >= 3)
			{
                foreach (RadTreeNode node1 in RadTreeView1.Nodes)
				{
					if (node1.Nodes.Count > 0)
					{
                        foreach (RadTreeNode node2 in node1.Nodes)
						{
							if (node2.Nodes.Count > 0)
							{
                                foreach (RadTreeNode node3 in node2.Nodes)
								{
                                    node3.Expanded = true;

                                    if (showAllSections)
                                    {
                                        foreach (RadTreeNode node4 in node3.Nodes)
                                        {
                                            if (node4.Nodes.Count > 1)
                                                node4.Expanded = true;
                                        }
                                    }
								}
							}
						}
					}
				}
			}

			if (nTreeLevel >= 4)
			{
                RadTreeView1.ExpandAllNodes();
			}

		}

		private void SetLanguage()
		{
			Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
			Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            RadTreeView1.ToolTip = Global.rm.GetString("txtTooltipClickTreeNode"); //Click a publication /edition or section to show page data

//            SetRadToolbarLabel("Item1", "ProductionTree", Global.rm.GetString("txtProductionTree"));
            SetRadToolbarTooltip("Refresh", Global.rm.GetString("txtRefresh"));
            SetRadToolbarTooltip("Expand", Global.rm.GetString("txtTooltipExpand"));
            SetRadToolbarTooltip("Collapse", Global.rm.GetString("txtTooltipCollapse"));

            string pressSelText = Global.rm.GetString("txtPress");
            if ((bool)Application["UseChannels"])
                pressSelText = Global.rm.GetString("txtPublisher");
            else if ((bool)Application["LocationIsPress"] == false)
                pressSelText = Global.rm.GetString("txtLocation");

            if (pressSelText.Length > 8)
                pressSelText = "";
            SetRadToolbarLabel("Item1", "txtPress", pressSelText);
        }

        enum TreeStateColor {Unknown = -1, Missing = 0, Ready = 1, Error = 2, Imaged = 3, Common = 4, ApprovalPending = 5, AllApproved=6};

        private TreeStateColor GetTreeState(DateTime pubDate, string publication, string edition, string section)
        {
            if (dbTreeStateTable == null)
                return TreeStateColor.Unknown;
            if (dbTreeStateTable.Rows.Count == 0)
                return TreeStateColor.Unknown;

            foreach (DataRow row in dbTreeStateTable.Rows)
            {
                if ((DateTime)row["PubDate"] == pubDate && (string)row["Publication"] == publication)
                {
                    if ((string)row["Edition"] == edition || edition == "")
                    {
                        if ((string)row["Section"] == section || section == "")
                        {
                            if ((int)row["AnyError"] > 0)
                                return TreeStateColor.Error;
                            if ((int)row["NeedApproval"] > 0)
                                return TreeStateColor.ApprovalPending;
                            if ((int)row["AnyUniquePage"] == 0)
                                return TreeStateColor.Common;
                            if ((int)row["AnyReady"] > 0)
                                return TreeStateColor.Ready;
                            if ((int)row["HasBeenImaged"] > 0)
                                return TreeStateColor.Imaged;
                            if ((int)row["MaxStatus"] == 0)
                                return TreeStateColor.Missing;
                            else
                                return TreeStateColor.Ready;
                        }

                    }
                }
            }
            return TreeStateColor.Unknown;
        }

        private string AddPageCountStringToNode(string nodeTextParent, int pageCountChild, int pagesReceivedChild)
        {
            string txt = nodeTextParent;

            int n = nodeTextParent.IndexOf('[');
            if (n == -1)
                return nodeTextParent + " [" + pageCountChild.ToString() + "/" + pagesReceivedChild.ToString() + "]";

            int m = nodeTextParent.IndexOf(']');
            int k = nodeTextParent.IndexOf('/');
            if (m != -1 && k != -1)
            {
                int parentPageCount = Globals.TryParse(nodeTextParent.Substring(n+1,k-n-1),0);
                int parentPagesReceived = Globals.TryParse(nodeTextParent.Substring(k + 1, m - k - 1), 0);

                parentPageCount += pageCountChild;
                parentPagesReceived += pagesReceivedChild;
                txt = nodeTextParent.Substring(0, n) + " [" + parentPageCount.ToString() + "/" + parentPagesReceived.ToString() + "]";

            }
            return txt;
        }

		private void PopulateTree(bool firstTime, bool changedLocation, bool changedChannel)
		{
      //      Global.logging.WriteLog("Enter PopulateTree()");
            RadTreeView1.Nodes.Clear();			
//			currentTree.AppendChildNode(TreeView1.Nodes[0]);
//            Object expandedState = RadTreeView1.SaveExpandedState();

			CCDBaccess db = new CCDBaccess();
			string errmsg;

            bool hasExistingSelectionPublication = false;
            bool hasExistingSelectionPubdate = false;
            bool hasExistingSelectionEdition = false;
            bool hasExistingSelectionSection = false;
		
			// If server was restarted cahces may have been flushed.
			if (Globals.GetCacheRowCount("PublicationNameCache") < 1 || Globals.GetCacheRowCount("EditionNameCache") < 1 || Globals.GetCacheRowCount("SectionNameCache") < 1)
				Globals.ForceCacheReloads();

			DataTable dbTable = db.GetActiveProductionCollection("ActiveProductions", out errmsg);
            RadTreeView1.DataSource = dbTable;

    //        Global.logging.WriteLog("Enter PopulateTree() 2 ");
            
//            if ((bool)Application["UseTreeState"])
 //               dbTreeStateTable = db.CacheTreeState(out errmsg);

			bool hideEdition = Globals.GetCacheRowCount("EditionNameCache") < 2;
			bool hideSection = Globals.GetCacheRowCount("SectionNameCache") < 2;
			bool hideIssue = Globals.GetCacheRowCount("IssueNameCache") < 2;

            RadTreeView1.DataMember = "ActiveProductions";

			//tree.DataBind();
	
			// Load Tree nodes
			//string thisProduction = "";
			DateTime thisPubDate = new DateTime(1975,1,1,0,0,0);
			string thisPublication = "";
			string thisIssue = "";
			string thisEdition = "";
			string thisSection = "";

			//int nProductions = 0;
			int nPublications = 0;
			int nPubDates = 0;
			int nIssues = 0;
			int nEditions = 0;
			int nSections = 0;
			int nStatus = 0;
            int nMaxStatus = 0;
            int nPageCount = 0;
            int nPagesReceived = 0;

            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            string sAll = Global.rm.GetString("txtAll");
            string weekString = Global.rm.GetString("txtWeek");
			string sNone = Global.rm.GetString("txtNoPublications");
			
		//	DataSet dataSet = null;
			
		//	if (Global.pagesInTree)
		//		dataSet = db.GetThumbnailPageCollection(false, false, true, out errmsg);

            RadTreeNode node = new RadTreeNode();

			node.Text = dbTable.Rows.Count>0 ? sAll : sNone; 
			node.SelectedImageUrl = "./Images/world.gif";
            node.ImageUrl = node.ImageUrl;
           // node.PostBack = false;

            RadTreeView1.Nodes.Add(node);
            RadTreeView1.Nodes[0].Expanded = true;

  //          Global.logging.WriteLog("Enter PopulateTree() 3 ");


            if ((int)Application["ShowPageCountInTree"] > 0)
            {
                foreach (DataRow row in dbTable.Rows)
                {
                  
                    thisPubDate = (DateTime)row["PubDate"];
                    thisPublication = (string)row["Publication"];
                    thisEdition = (string)row["Edition"];

                    int nPageCountPublication = 0;
                    int nPagesReceivedPublication = 0;
                    foreach (DataRow row2 in dbTable.Rows)
                    {
                        if ((DateTime)row2["PubDate"] == thisPubDate && (string)row2["Publication"] == thisPublication) // && (string)row2["Edition"] == thisEdition)
                        {
                            nPageCountPublication += (int)row2["PageCount"];
                            nPagesReceivedPublication += (int)row2["PagesReceived"];
                        }
                    }
                    row["PageCountInPublication"] = nPageCountPublication;
                    row["PagesReceivedInPublication"] = nPagesReceivedPublication;

                    int nPageCountEdition = 0;
                    int nPagesReceivedEdition = 0;
                  
                    foreach (DataRow row2 in dbTable.Rows)
                    {
                        if ((DateTime)row2["PubDate"] == thisPubDate && (string)row2["Publication"] == thisPublication && (string)row2["Edition"] == thisEdition)
                        {
                            nPageCountEdition += (int)row2["PageCount"];
                            nPagesReceivedEdition += (int)row2["PagesReceived"];                            
                        }                        
                    }
                    row["PageCountInEdition"] = nPageCountEdition;
                    row["PagesReceivedInEdition"] = nPagesReceivedEdition;

                }
            }

//            Global.logging.WriteLog("Enter PopulateTree() 4 ");


            foreach (DataRow row in dbTable.Rows)
            {

                thisPubDate = (DateTime)row["PubDate"];
                thisPublication = (string)row["Publication"];
                thisEdition = (string)row["Edition"];

                int nMinStatusPublication = 999;
                int nMaxStatusPublication = 0;
                int nErrorEvent = 0;

                foreach (DataRow row2 in dbTable.Rows)
                {
                    if ((DateTime)row2["PubDate"] == thisPubDate && (string)row2["Publication"] == thisPublication) // && (string)row2["Edition"] == thisEdition)
                    {
                        if ((int)row2["Status"] < nMinStatusPublication)
                            nMinStatusPublication = (int)row2["Status"];

                        if ((int)row2["MaxStatus"] > nMaxStatusPublication)
                            nMaxStatusPublication = (int)row2["MaxStatus"];

                        if ((int)row2["MaxStatus"] == 46)
                            nMinStatusPublication = 46;
                        
                        if ((int)row2["ErrorEvent"] > 0)
                            nErrorEvent = (int)row2["ErrorEvent"];
                    }
                }
                           
                row["StatusPublication"] = nMinStatusPublication;
                row["MaxStatusPublication"] = nMaxStatusPublication;
                row["ErrorEventPublication"] = nErrorEvent;

                int nMinStatusEdition = 999;
                int nMaxStatusEdition = 0;
                nErrorEvent = 0;

                foreach (DataRow row2 in dbTable.Rows)
                {
                    if ((DateTime)row2["PubDate"] == thisPubDate && (string)row2["Publication"] == thisPublication && (string)row2["Edition"] == thisEdition)
                    {
                        if ((int)row2["Status"] < nMinStatusEdition)
                            nMinStatusEdition = (int)row2["Status"];

                        if ((int)row2["MaxStatus"] > nMaxStatusEdition)
                            nMaxStatusEdition = (int)row2["MaxStatus"];

                        if ((int)row2["MaxStatus"] == 46)
                            nMinStatusEdition = 46;

                        if ((int)row2["ErrorEvent"] > 0)
                            nErrorEvent = (int)row2["ErrorEvent"];
                    }
                }
                row["StatusEdition"] = nMinStatusEdition;
                row["MaxStatusEdition"] = nMaxStatusEdition;
                row["ErrorEventEdition"] = nErrorEvent;


                if ((bool)Application["UseChannels"])
                {
                    row["ErrorEvent"] = 0;
                    row["ErrorEventEdition"] = 0;
                    row["ErrorEventPublication"] = 0;
                }  
            }

            //Global.logging.WriteLog("Enter PopulateTree() 5 ");

            thisPubDate = new DateTime(1975, 1, 1, 0, 0, 0);
            thisPublication = "";
            thisIssue = "";
            thisEdition = "";
            thisSection = "";
            nPublications = 0;
            nPubDates = 0;
            nIssues = 0;
            nEditions = 0;
            nSections = 0;

			foreach (DataRow row in dbTable.Rows)
			{

                // Level 0
				if ((DateTime)row["PubDate"] != thisPubDate)
				{
					thisPubDate = (DateTime)row["PubDate"];

                    node = new RadTreeNode();
                    if ((int)Application["PubDateFormat"] == 0)
					    node.Text = thisPubDate.ToShortDateString();
                    else if ((int)Application["PubDateFormat"] == 1)
                        node.Text = string.Format("{0:00}.{1:00}.{2:0000}", thisPubDate.Day, thisPubDate.Month, thisPubDate.Year);
                    else if ((int)Application["PubDateFormat"] == 2)
                        node.Text = string.Format("{0:00}.{1:00}.{2:00}", thisPubDate.Day, thisPubDate.Month, thisPubDate.Year - 2000);
                    else if ((int)Application["PubDateFormat"] == 3)
                        node.Text = string.Format("{0:00}-{1:00}-{2:0000}", thisPubDate.Day, thisPubDate.Month, thisPubDate.Year);
                    else if ((int)Application["PubDateFormat"] == 4)
                        node.Text = string.Format("{0:00}-{1:00}-{2:00}", thisPubDate.Day, thisPubDate.Month, thisPubDate.Year - 2000);
                    else if ((int)Application["PubDateFormat"] == 5)
                        node.Text = string.Format("{0:00}/{1:00}/{2:0000}", thisPubDate.Day, thisPubDate.Month, thisPubDate.Year);
                    else if ((int)Application["PubDateFormat"] == 6)
                        node.Text = string.Format("{0:00}/{1:00}/{2:00}", thisPubDate.Day, thisPubDate.Month, thisPubDate.Year-200);
                    else if ((int)Application["PubDateFormat"] == 7)
                    {
                        DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                        System.Globalization.Calendar cal = dfi.Calendar;
                        int week = cal.GetWeekOfYear(thisPubDate, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                        node.Text = string.Format("{0} {1:00}", weekString, week);
                    }


                    RadTreeView1.Nodes[0].Nodes.Add(node);
                    RadTreeView1.Nodes[0].Nodes[nPubDates].ImageUrl = "./Images/Issue.gif";
                    RadTreeView1.Nodes[0].Nodes[nPubDates].SelectedImageUrl = RadTreeView1.Nodes[0].Nodes[nPubDates].ImageUrl;
                    RadTreeView1.Nodes[0].Nodes[nPubDates].Expanded = true;
                    RadTreeView1.Nodes[0].Nodes[nPubDates].Value = Globals.ToStandardDateString(thisPubDate); //(string)row["Press"];

                    if ((DateTime)Session["SelectedPubDate"] == thisPubDate)
                        hasExistingSelectionPubdate = true;

					if (nPubDates == 0 && (bool)Application["BlankOnEntry"] == false) 
					{
						Session["SelectedPubDate"] = thisPubDate;
						Session["SelectedPublication"] = (string)row["Publication"];
						Session["SelectedPress"] = (string)row["Press"];
					} 

					nPubDates++;
					
					thisPublication = "";
					thisSection = "";
					thisEdition = "";
					nPublications = 0;
					nEditions = 0;
					nSections = 0;

				}
				if ((string)row["Publication"] != thisPublication)
				{
					thisPublication = (string)row["Publication"];

                    int thisPlanType = (int)row["PlanType"];
                    int thisAllPagesReadyStatus = thisPlanType / 10000;
                    thisPlanType = thisPlanType - 10000 * thisAllPagesReadyStatus;

                    int thisVisioLinkStatus = thisPlanType / 100;
                    thisPlanType = thisPlanType - 100 * thisVisioLinkStatus;

                    string press = (string)row["Press"];
                    int pressID = Globals.GetIDFromName("PressNameCache",(string)row["Press"]);

                    if ((string)Session["SelectedPublication"] == thisPublication)
                        hasExistingSelectionPublication = true;

                    nStatus = (int)row["StatusPublication"];
                    nMaxStatus = (int)row["MaxStatusPublication"];

                    nPageCount = (int)row["PageCountInPublication"];
                    nPagesReceived = (int)row["PagesReceivedInPublication"];

					string imgurl = "./Images/productionyellow.gif";
					if (nStatus == 46)
						imgurl = "./Images/productionred.gif";
					else if (nStatus == 50)
						imgurl = "./Images/productiongreen.gif";
                    else if (nMaxStatus == 50)
                        imgurl = "./Images/productionlightgreen.gif";
                    else if (nStatus == 0 && (nMaxStatus == 0 || (bool)Application["TreeWhiteStatusUntilAllReceived"]))
                        imgurl = "./Images/production.gif";

                    if ((int)row["ErrorEvent"] > 0 || (int)row["ErrorEventPublication"] > 0)
                        imgurl = "./Images/productionred.gif";


					string nodeText = thisPublication;
                    node = new RadTreeNode();
					
					string pubNameAlias = Globals.LookupInputAlias("Publication", thisPublication);
                    string pubNameInkAlias = Globals.LookupInkAlias("Publication", thisPublication, pressID);

                    if ((bool)Application["ShowInkAliasInTree"] && pubNameInkAlias != thisPublication)
                    {
                        nodeText += " (" + pubNameInkAlias + ")";
                    }


					if ((bool)Application["ShowAliasInTree"] && pubNameAlias != thisPublication) 
					{
						if ((int)Application["ShowAliasInTreeChar"] != 0)
						{
							pubNameAlias += "                   ";
							pubNameAlias = pubNameAlias.Substring(0, (int)Application["ShowAliasInTreeChar"]);
						}

						nodeText += " (" + pubNameAlias + ")";
					}


                    if ((bool)Application["ShowAliasInTreePrefix"] && pubNameAlias != thisPublication)
                    {
                        nodeText = pubNameAlias + " " + nodeText;
                    }

                    if ((bool)Application["ShowOrdernumberInTree"] && (string)row["Production"] != "" )
						nodeText += " / " + (string)row["Production"];	

					int weekNumber = (int)row["Weeknumber"];
					if ((bool)Application["ShowWeeknumberInTree"] && weekNumber != 0)
						nodeText += " / " + weekNumber.ToString();

					if ((bool)Application["ShowCustomerInTree"] && (string)row["Customer"] != "")
						nodeText += " / " + (string)row["Customer"];

                    if ((bool)Application["ShowCommentInTree"] && (string)row["Comment"] != "")
                        nodeText += " / " + (string)row["Comment"];

                    if ((int)Application["ShowPageCountInTree"] > 0 && nPageCount > 0)
                    {
                        nodeText += " [" + nPagesReceived.ToString() + "/" + nPageCount.ToString() + "]";
                    }
                    
					node.Text = nodeText;
                    if (thisPlanType == 0)
                        node.ForeColor = Color.DarkRed;

                    if (thisPlanType == 2)
                        node.ForeColor = Color.DarkOrange;

                  //  if ((bool)Application["UseChannels"])
                  //  {
                        if (thisVisioLinkStatus == 1)
                            node.BackColor = Color.LemonChiffon;
                        else if (thisVisioLinkStatus == 2)
                            node.BackColor = Color.PaleTurquoise;
                        else if (thisVisioLinkStatus == 3)
                            node.BackColor = Color.DeepSkyBlue;
                        else if (thisVisioLinkStatus == 4)
                            node.BackColor = Color.Orchid;
                        else if (thisVisioLinkStatus == 5)
                            node.BackColor = Color.LightGreen;
                        else if (thisVisioLinkStatus == 6)
                            node.BackColor = Color.Red;
                 //   }
                        if (thisAllPagesReadyStatus > 1)
                            node.BackColor = Color.LightGreen;


                    RadTreeView1.Nodes[0].Nodes[nPubDates - 1].Nodes.Add(node);
                    RadTreeView1.Nodes[0].Nodes[nPubDates - 1].Nodes[nPublications].ImageUrl = imgurl;
                    RadTreeView1.Nodes[0].Nodes[nPubDates - 1].Nodes[nPublications].SelectedImageUrl = imgurl;
                    RadTreeView1.Nodes[0].Nodes[nPubDates - 1].Nodes[nPublications].Expanded = true;
                    RadTreeView1.Nodes[0].Nodes[nPubDates - 1].Nodes[nPublications].Value = (string)row["Press"];
					
					nPublications++;
					
					thisSection = "";
					thisEdition = "";
					thisIssue = "";
					nIssues = 0;
					nEditions = 0;
					nSections = 0;
				}

				// Level 2
/*				if (hideIssue == false)
				{

					if ((string)row["Issue"] != thisIssue) 
					{
						thisIssue = (string)row["Issue"];
						nStatus = (int)row["Status"];

                        node = new Node();
                        node.Text = thisIssue;
                        RadTreeView1.Nodes[0].Nodes[nPubDates - 1].Nodes[nPublications - 1].Nodes.Add(node);
                        RadTreeView1.Nodes[0].Nodes[nPubDates - 1].Nodes[nPublications - 1].Nodes[nIssues].ImageUrl = "./Images/Issue2.gif";
                        RadTreeView1.Nodes[0].Nodes[nPubDates - 1].Nodes[nPublications - 1].Nodes[nIssues].SelectedImageUrl = "./Images/Issue2.gif";
                        RadTreeView1.Nodes[0].Nodes[nPubDates - 1].Nodes[nPublications - 1].Nodes[nIssues].Expanded = true;
                        RadTreeView1.Nodes[0].Nodes[nPubDates - 1].Nodes[nPublications - 1].Nodes[nIssues].Tag = (string)row["Press"];
					
						nIssues++;
						thisEdition = "";
						thisSection = "";
						nEditions = 0;
						nSections = 0;
					}
				}
                */
			
				// Level 3 or 2
				if (hideEdition == false)
				{
					if ((string)row["Edition"] != thisEdition) 
					{
						thisEdition = (string)row["Edition"];

                        if ((string)Session["SelectedEdition"] == thisEdition)
                            hasExistingSelectionEdition = true;

						nStatus = (int)row["StatusEdition"];
                        nMaxStatus = (int)row["MaxStatusEdition"];
						bool timedEdition = (int)row["TimedFrom"] > 0;

                        nPageCount = (int)row["PageCountInEdition"];
                        nPagesReceived = (int)row["PagesReceivedInEdition"];

						string imgurl = timedEdition ? "./Images/editionyellow_timed.gif" : "./Images/editionyellow.gif";
                        if (nStatus == 46)
							imgurl =  timedEdition ? "./Images/editionred_timed.gif" : "./Images/editionred.gif";
						else if (nStatus == 50)
							imgurl = timedEdition ? "./Images/editiongreen_timed.gif" : "./Images/editiongreen.gif";
                        else if (nMaxStatus == 10)
                            imgurl = timedEdition ? "./Images/editionlightgreen_timed.gif" : "./Images/editionlightgreen.gif";
                        else if (nStatus == 0 && (nMaxStatus == 0 || (bool)Application["TreeWhiteStatusUntilAllReceived"]))
                            imgurl = timedEdition ? "./Images/edition_timed.gif" : "./Images/edition.gif";

                        if ((int)row["ErrorEvent"] > 0 || (int)row["ErrorEventEdition"] > 0)
                            imgurl = timedEdition ? "./Images/editionred_timed.gif" : "./Images/editionred.gif";

                        string nodeText = thisEdition;
                        if ((int)Application["ShowPageCountInTree"] > 1 && nPageCount > 0)
                        {
                            nodeText += " [" + nPagesReceived.ToString() + "/" + nPageCount.ToString() + "]";
                        }

                        string comment = (string)row["EComment"];
                        if ((bool)Application["ShowEditionCommentInTree"] && comment != "")
                        {
                            nodeText += " [" + comment + "]";
                        }

                        if (hideIssue)
						{
                            node = new RadTreeNode();
                            node.Text = nodeText;
                            RadTreeView1.Nodes[0].Nodes[nPubDates - 1].Nodes[nPublications - 1].Nodes.Add(node);
                            RadTreeView1.Nodes[0].Nodes[nPubDates - 1].Nodes[nPublications - 1].Nodes[nEditions].ImageUrl = imgurl;
                            RadTreeView1.Nodes[0].Nodes[nPubDates - 1].Nodes[nPublications - 1].Nodes[nEditions].SelectedImageUrl = imgurl;
                            RadTreeView1.Nodes[0].Nodes[nPubDates - 1].Nodes[nPublications - 1].Nodes[nEditions].Expanded = true;
                            RadTreeView1.Nodes[0].Nodes[nPubDates - 1].Nodes[nPublications - 1].Nodes[nEditions].Value = (string)row["Press"];
						}
						else
						{
                            node = new RadTreeNode();
                            node.Text = thisIssue;
                            RadTreeView1.Nodes[0].Nodes[nPubDates - 1].Nodes[nPublications - 1].Nodes[nIssues - 1].Nodes.Add(node);

							if (nStatus == 0) 
							{
                                RadTreeView1.Nodes[0].Nodes[nPubDates - 1].Nodes[nPublications - 1].Nodes[nIssues - 1].Nodes[nEditions].ImageUrl = imgurl;
                                RadTreeView1.Nodes[0].Nodes[nPubDates - 1].Nodes[nPublications - 1].Nodes[nIssues - 1].Nodes[nEditions].SelectedImageUrl = imgurl;								
							}
                            RadTreeView1.Nodes[0].Nodes[nPubDates - 1].Nodes[nPublications - 1].Nodes[nIssues - 1].Nodes[nEditions].Expanded = true;
                            RadTreeView1.Nodes[0].Nodes[nPubDates - 1].Nodes[nPublications - 1].Nodes[nIssues - 1].Nodes[nEditions].Value = (string)row["Press"];
						}
						nEditions++;
						thisSection = "";
						nSections = 0;
					}
				}

				// Level 3
				if (hideSection == false /* || Global.pagesInTree */)
				{
					if ((string)row["Section"] != thisSection) 
					{
						thisSection = (string)row["Section"];
                        if ((string)Session["SelectedSection"] == thisSection)
                            hasExistingSelectionSection = true;
                        RadTreeNode nextNode = null;
						if (hideIssue)
						{
							if (hideEdition == false)
							{
                                nextNode = RadTreeView1.Nodes[0].Nodes[nPubDates - 1].Nodes[nPublications - 1].Nodes[nEditions - 1];	
							}
							else
							{
                                nextNode = RadTreeView1.Nodes[0].Nodes[nPubDates - 1].Nodes[nPublications - 1];	
							}
						}
						else
						{
							if (hideEdition == false)
							{
                                nextNode = RadTreeView1.Nodes[0].Nodes[nPubDates - 1].Nodes[nPublications - 1].Nodes[nIssues - 1].Nodes[nEditions - 1];	
							}
							else
							{
                                nextNode = RadTreeView1.Nodes[0].Nodes[nPubDates - 1].Nodes[nPublications - 1].Nodes[nIssues - 1];	
							}
						}

						nStatus = (int)row["Status"];
                        nMaxStatus = (int)row["MaxStatus"];
                        nPageCount = (int)row["PageCount"];
                        nPagesReceived = (int)row["PagesReceived"];
                        string comment = (string)row["PComment"];

						string imgurl = "./Images/sectionyellow.gif";
                        if (nStatus == 46 || (int)row["ErrorEvent"] > 0)
							imgurl = "./Images/sectionred.gif";
						else if (nStatus == 50)
							imgurl = "./Images/sectiongreen.gif";
                        else if (nStatus >= 10)
                            imgurl = "./Images/sectionlightgreen.gif";
                        else if (nStatus == 0 && nMaxStatus == 0)
							imgurl = "./Images/section.gif";


                        node = new RadTreeNode();

                        string nodeText = thisSection;
                        if ((int)Application["ShowPageCountInTree"] > 1 && nPageCount > 0)
                        {
                            nodeText += " [" + nPagesReceived.ToString() + "/" + nPageCount.ToString() + "]";
                        }
                        if ((bool)Application["ShowSectionCommentInTree"] && comment != "")
                        {
                            nodeText += " [" + comment + "]";
                        }
                        node.Text = nodeText;
						nextNode.Nodes.Add(node);
						nextNode.Nodes[nSections].ImageUrl = imgurl;
                        nextNode.Nodes[nSections].SelectedImageUrl = imgurl;
						nextNode.Nodes[nSections].Value = (string)row["Press"];

						nSections++;

					}
				}
			}

			if (firstTime == false)
			{
                //RadTreeView1.LoadExpandedState(expandedState);
			}

            if ((changedLocation || changedChannel) && hasExistingSelectionPublication && hasExistingSelectionPubdate &&
                    (hasExistingSelectionEdition || (string)Session["SeletedEdition"] == "*") &&
                    (hasExistingSelectionSection || (string)Session["SeletedSection"] == "*"))
            {
                updateMain = true;
            }

        //    Global.logging.WriteLog("Exit PopulateTree() ");

		}


		private DataView GetFilteredTable(DataTable dataTable, DateTime tPubDate, string sPublication, string sIssue, string sEdition, string sSection)
		{
            string language = (string)Session["language"];
			bool dayFirstInDate = language == "fr" || language == "fi" || language == "da" || language == "sv" || language == "de" || language == "no";
			bool yearMonthDay = language.IndexOf("zh") != -1 ? true : false;
			string errmsg = "";
			if (dataTable == null)
			{
                Global.logging.WriteLog("Tree: GetFilteredTable() " + errmsg);
				return null;
			}
			
			string filterstring = "PageType<2";
	
			if (tPubDate.Year > 2000) 
			{
			//	string s = tPubDate.Month + "/" + tPubDate.Day + "/" + tPubDate.Year;

				string s;
				if (dayFirstInDate)
					s = tPubDate.Day + "." + tPubDate.Month + "." + tPubDate.Year;
				else
					s = tPubDate.Month + "/" + tPubDate.Day + "/" + tPubDate.Year;

				if (yearMonthDay)
					s = tPubDate.Year + "-" + tPubDate.Month + "-" + tPubDate.Day;

				filterstring += " AND PubDate='"+s+"'";
			}	
			if (sPublication != "")
				filterstring += " AND Publication='"+sPublication+"'";

			if (sEdition != "")
				filterstring += " AND Edition='"+sEdition+"'";

			if (sSection != "")
				filterstring += " AND Section='"+sSection+"'";

			string sortstring = "PubDate,Publication,Issue,Edition,Section,Pagination";

            DataView dv = new DataView(dataTable, filterstring, sortstring, DataViewRowState.CurrentRows)
            {
                AllowDelete = true,
                AllowEdit = true,
                AllowNew = true
            };
            return dv;
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
	        this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion  

        private string GetSanitizedNameOfNode(string name)
        {
            string txt = name;
            int n;
            char[] seps = { '/', '(', '{','[' };

            if ((bool)Application["ShowAliasInTreePrefix"] && name.IndexOf(" ") != -1)
            {
                n = txt.IndexOf(" ");
                txt = txt.Substring(n + 1);
            }
            n = txt.IndexOfAny(seps);
            if (n != -1)
                txt = txt.Substring(0, n);
            return txt.Trim();
        }

        protected void RadTreeView1_NodeClick1(object sender, RadTreeNodeEventArgs e)
 		{
			//int n = -1;
			bool hideSection = Globals.GetCacheRowCount("SectionNameCache") < 2;
			bool hideEdition = Globals.GetCacheRowCount("EditionNameCache") < 2;
			bool hideIssue = Globals.GetCacheRowCount("IssueNameCache") < 2;

			if (hideIssue)
				Session["SelectedIssue"] =  "";
			if (hideEdition)
				Session["SelectedEdition"] =  "";
			if (hideSection) 
				Session["SelectedSection"] =  "";

           // Session["SelectedChannel"] = "";

//			string txt = "";
			updateMain = false;
			string delimStr = " /.-(";
			char [] delimiter = delimStr.ToCharArray();

			bool dayFirstInDate = Global.language == "fr" || Global.language == "fi" || Global.language == "da" || Global.language == "sv" || Global.language == "de" || Global.language == "no";
			bool yearMonthDay =  Global.language.IndexOf("zh") != -1 ? true : false;

			//
			//0 All							All						All
			//1 -- PubDate					-- PubDate				-- Pubdate
			//2    -- Publication			   -- Publication		   -- Publication
			//3       -- Issue					  -- Edition			  -- Issue
			//4          -- Edition					 -- Section
			//5             -- Section
			//int mm,dd,yy;
            switch (e.Node.Level)
			{
				case 0:	// All
					Session["SelectedPublication"] = "";
					Session["SelectedIssue"] = "";
					Session["SelectedEdition"] = "";
					Session["SelectedSection"] = "";
					break;
				case 1: // Pubdate node
					Session["SelectedPublication"] = "";
					Session["SelectedIssue"] = "";
					Session["SelectedEdition"] = "";
					Session["SelectedSection"] = "";
					Session["SelectedPubDate"] = Globals.FromStandardDateString((string)e.Node.Value); //dt;

                    if (e.Node.Nodes.Count > 0 && (bool)Application["LocationIsPress"] == false && (bool)Application["UsePressGroups"] == false)
                        Session["SelectedPress"] = (string)e.Node.Nodes[0].Value;
					break;
				case 2: // Publication node
					Session["SelectedEdition"] = "";
					Session["SelectedSection"] = "";
					

                    Session["SelectedPublication"] = GetSanitizedNameOfNode(e.Node.Text);
					string sss = (string)Session["SelectedPublication"];
					Session["SelectedPubDate"] = Globals.FromStandardDateString((string)e.Node.ParentNode.Value); //dt2;
                    if ((bool)Application["UsePressGroups"] == false)
                        Session["SelectedPress"] = (string)e.Node.Value;

					break;

				case 3: // Issue or Edition node or (if no edition section node) or (if no ed/sec) page node
					if (hideIssue == false)
					{
                        Session["SelectedIssue"] = GetSanitizedNameOfNode(e.Node.Text);
						Session["SelectedSection"] = "";
						Session["SelectedEdition"] = "";
					}
					else
					{ // No Issues!
						if (hideEdition == false)
						{
                            Session["SelectedEdition"] = GetSanitizedNameOfNode(e.Node.Text);
							Session["SelectedSection"] = "";
						}
						else 
						{
							// No editions!
							if (hideSection == false)
							{
                                Session["SelectedSection"] = GetSanitizedNameOfNode(e.Node.Text);
							} 
						}
					}

                    Session["SelectedPublication"] = GetSanitizedNameOfNode(e.Node.ParentNode.Text);
					Session["SelectedPubDate"] = Globals.FromStandardDateString((string)e.Node.ParentNode.ParentNode.Value); //dt3;
                    if ((bool)Application["UsePressGroups"] == false)
                        Session["SelectedPress"] = (string)e.Node.Value;
					break;
				case 4:	// Section node
					if (hideIssue == false)
					{	
						if (hideEdition == false)
						{
							if (hideSection == false)	
							{
								Session["SelectedSection"] = GetSanitizedNameOfNode(e.Node.Text);
                                Session["SelectedEdition"] = GetSanitizedNameOfNode(e.Node.ParentNode.Text);
                                Session["SelectedIssue"] = GetSanitizedNameOfNode(e.Node.ParentNode.ParentNode.Text);
							}
							else
							{
								Session["SelectedEdition"] = GetSanitizedNameOfNode(e.Node.Text);
                                Session["SelectedIssue"] = GetSanitizedNameOfNode(e.Node.ParentNode.Text);
							}
						}
						else
						{
							if (hideSection == false)	
							{
                                Session["SelectedSection"] = GetSanitizedNameOfNode(e.Node.Text);
                                Session["SelectedIssue"] = GetSanitizedNameOfNode(e.Node.ParentNode.Text);
							}
							else
							{
                                Session["SelectedIssue"] = GetSanitizedNameOfNode(e.Node.Text);
							}
						}
					}
					else
					{
						// No issue
						if (hideEdition == false)
						{
							if (hideSection == false)	
							{
                                Session["SelectedSection"] = GetSanitizedNameOfNode(e.Node.Text);
                                Session["SelectedEdition"] = GetSanitizedNameOfNode(e.Node.ParentNode.Text);
							}
							else
							{
                                Session["SelectedEdition"] = GetSanitizedNameOfNode(e.Node.Text);
							}
						}
						else
						{
                            Session["SelectedSection"] = GetSanitizedNameOfNode(e.Node.Text);
						}
					}

                    Session["SelectedPublication"] = GetSanitizedNameOfNode(e.Node.ParentNode.ParentNode.Text);

					Session["SelectedPubDate"] =  Globals.FromStandardDateString((string)e.Node.ParentNode.ParentNode.ParentNode.Value); //dt4;
                    if ((bool)Application["LocationIsPress"] == false && (bool)Application["UsePressGroups"] == false)
                        Session["SelectedPress"] = (string)e.Node.Value;
					break;
			}
            string prodLabel = Globals.ToShortDateString((DateTime)Session["SelectedPubDate"]);
            if ((int)Application["PubDateFormat"] == 7)
            {

                Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);
                DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                System.Globalization.Calendar cal = dfi.Calendar;
                int week = cal.GetWeekOfYear((DateTime)Session["SelectedPubDate"], dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                prodLabel = string.Format("{0} {1:00}", Global.rm.GetString("txtWeek"), week);

            }
            prodLabel += "  " + (string)Session["SelectedPublication"]; ;
            if ((string)Session["SelectedEdition"] != "")
                prodLabel += " " + (string)Session["SelectedEdition"];
            if ((string)Session["SelectedSection"] != "")
                prodLabel += " " + (string)Session["SelectedSection"];

            DateTime deadline = DateTime.MinValue;
            CCDBaccess db = new CCDBaccess();
            db.GetProductionDeadLine((DateTime)Session["SelectedPubDate"], (string)Session["SelectedPublication"], (string)Session["SelectedChannel"], ref deadline, out string errmsg);
            Session["CurrentProduct"] = prodLabel.Length <= 32 ? prodLabel : prodLabel.Substring(0, 31);

            if (deadline.Year > 2000)
                Session["CurrentProduct"] += string.Format(" Release: {0:00}-{1:00} {2:00}:{3:00}", deadline.Month, deadline.Day, deadline.Hour, deadline.Minute);
            updateMain = true;					
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

        private void SetRadToolbarTooltip(string buttonID, string text)
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue(buttonID);
            if (item == null)
                return;
            item.ToolTip = text;
        }

        protected void RadToolBar1_ButtonClick(object sender, Telerik.Web.UI.RadToolBarEventArgs e)
        {
            if (e.Item.Value.IndexOf("Press") != -1)
            {
                string press = e.Item.Value;
                press = press.Replace("Press", "");

                Session["SelectedPublisher"] = press;
                PopulateTree(false, true,true);
                DefaultTreeExpansion();
                updateMain = true;
            }

            if (e.Item.Value == "Refresh")
            {
                //  SetLanguage();
                PopulateTree(false, false,false);
                DefaultTreeExpansion();
            }
            if (e.Item.Value == "Expand")
            {
                RadTreeView1.ExpandAllNodes();
            }
            else
                if (e.Item.Value == "Collapse")
                {
                    RadTreeView1.CollapseAllNodes();
                    RadTreeView1.Nodes[0].Expanded = true;
                }
            Session["ChoosedPage"] = "";
            nodeClicked = "";
        }
	}
}
