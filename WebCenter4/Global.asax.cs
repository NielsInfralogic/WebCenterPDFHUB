using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.SessionState;
using System.Globalization;
using System.Data;
using System.Text;
using System.Collections;
using System.Resources;
using System.Threading;
using System.Diagnostics;
using WebCenter4.Classes;
using System.Configuration;
using System.Security.Principal;

namespace WebCenter4
{
    public class Global : System.Web.HttpApplication
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        /// 
        static Cache _cache = null;                                                                                   

        //		private static Timer timer;

        //private static Timer messageTimer;

        public static Logger logging = null;


        public enum DatabaseType
        {
            ControlCenter = 0, EskoNet15, EskoNet21, Generic
        };

        public static ResourceManager rm = null;
        public static string encoding = "utf-8";
        public static string language = "en";
        public static string culture = "en-US";
        public static bool pagesInTree = false;
        public static string sVirtualImageFolder = "CCPreviews";
        public static string sVirtualRasterImageFolder = "CCPreviews2";
        public static string sVirtualReadViewImageFolder = "CCreadviewpreviews";
        public static string sVirtualThumbnailFolder = "CCThumbnails";

        public static string sVirtualFlatImageFolder = "CCFlatPreviews";
        public static string sVirtualFlatThumbnailsFolder = "CCFlatThumbnails";

        public static string sVirtualHiResFolder = "CCFiles";
        public static string sVirtualReportFolder = "CCReports";
        public static string sVirtualUploadFolder = "CCUpload";
        public static string sVirtualPPMUploadFolder = "PPMUpload";

        public static string sVirtualArchiveFolder = "CCarchive";

        public static string sRealImageFolder = "";
        public static string sRealRasterImageFolder = "";
        public static string sRealReadViewImageFolder = "";
        public static string sRealThumbnailFolder = "";
        public static string sRealFlatImageFolder = "";
        public static string sRealFlatThumbnailFolder = "";

        public static string sRealUploadFolder = "";
        public static string sRealPPMUploadFolder = "";
        public static string sRealHiresFolder = "";
        public static string sRealArchiveFolder = "";
        
        public static int databaseVersion = 2;
        public static int databaseVersionMinor = 2;

        public static string sVirtualPdfLogFolder = "CCPDFlogs";
        public static string sVirtualPdfFolder = "CCPDFfiles";
        public static string sRealPdfLogFolder = "";
        public static string sRealPdfFolder = "";
        public static string sRealReportFolder = "";

        public static string sVirtualAlwanLogFolder = "CCAlwanlogs";
        public static string sRealAlwanLogFolder = "";
        

        public static string sVirtualLogFolder = "CCLogs";
        public static string sRealLogFolder = "";

        public static string sVirtualPlanFolder = "CCPlans";
        public static string sRealPlanFolder = "";

        public static int queryRetries = 5;
        public static int queryBackoffTime = 500;

        public static int spParamExists_spThumbnailPageList2_PressID = 0;
        public static int spParamExists_spPressRunStatEx_IgnoreDirty = 0;
        



       // private System.ComponentModel.IContainer components = null;

        private static int refreshCacheInterval = 60000 * 5; // five minutes
        //	private static int	refreshMessageInterval = 10000; // ten secs



        public Global()
        {
            //InitializeComponent();
        }

        protected void Application_Start(Object sender, EventArgs e)
        {
            _cache = Context.Cache; // Save reference for later

            string s = (string)ConfigurationManager.AppSettings["DatabaseVersion"];
            if (s != null)
            {
                databaseVersion = Int32.Parse(s);
            }

            string sm = (string)ConfigurationManager.AppSettings["DatabaseVersionMinor"];
            if (sm != null)
            {
                databaseVersionMinor = Int32.Parse(sm);
            }

            sVirtualImageFolder = "/" + ConfigurationManager.AppSettings["VirtualImageFolder"];
            sVirtualRasterImageFolder = "/" + ConfigurationManager.AppSettings["VirtualRasterImageFolder"];
            sVirtualThumbnailFolder = "/" + ConfigurationManager.AppSettings["VirtualThumbnailFolder"];
            sVirtualReadViewImageFolder = "/" + ConfigurationManager.AppSettings["VirtualReadViewFolder"];
            sVirtualHiResFolder = "/" + ConfigurationManager.AppSettings["VirtualHiResFolder"];


            sVirtualReportFolder = "/" + ConfigurationManager.AppSettings["VirtualReportFolder"];

            sVirtualLogFolder = "/" + ConfigurationManager.AppSettings["VirtualLogFolder"];
            sRealLogFolder = HttpContext.Current.Server.MapPath(sVirtualLogFolder);


            queryRetries = Globals.ReadConfigInt32("QueryRetries", 5);
            queryBackoffTime = Globals.ReadConfigInt32("QueryBackoffTime", 5000);


            sRealImageFolder = HttpContext.Current.Server.MapPath(sVirtualImageFolder);
            sRealRasterImageFolder = HttpContext.Current.Server.MapPath(sVirtualRasterImageFolder);
            sRealThumbnailFolder = HttpContext.Current.Server.MapPath(sVirtualThumbnailFolder);
            sRealHiresFolder = HttpContext.Current.Server.MapPath(sVirtualHiResFolder);
            sRealReadViewImageFolder = HttpContext.Current.Server.MapPath(sVirtualReadViewImageFolder);

            sVirtualPdfLogFolder = "/" + ConfigurationManager.AppSettings["VirtualPdfLogFolder"];
            sVirtualPdfFolder = "/" + ConfigurationManager.AppSettings["VirtualPdfFolder"];
            sRealPdfLogFolder = HttpContext.Current.Server.MapPath(sVirtualPdfLogFolder);
            sRealPdfFolder = HttpContext.Current.Server.MapPath(sVirtualPdfFolder);
            sRealReportFolder = HttpContext.Current.Server.MapPath(sVirtualReportFolder);


            if (ConfigurationManager.AppSettings["VirtualAlwanLogFolder"] != null)
            {
                sVirtualAlwanLogFolder = "/" + ConfigurationManager.AppSettings["VirtualAlwanLogFolder"];
                sRealAlwanLogFolder = HttpContext.Current.Server.MapPath(sVirtualAlwanLogFolder);
            }

            if (ConfigurationManager.AppSettings["VirtualFlatFolder"] != null)
            {
                sVirtualFlatImageFolder = "/" + ConfigurationManager.AppSettings["VirtualFlatFolder"];
                sRealFlatImageFolder = HttpContext.Current.Server.MapPath(sVirtualFlatImageFolder);
            }


            if (ConfigurationManager.AppSettings["VirtualFlatThumbnailsFolder"] != null)
            {
                sVirtualFlatThumbnailsFolder = "/" + ConfigurationManager.AppSettings["VirtualFlatThumbnailsFolder"];
                sRealFlatThumbnailFolder = HttpContext.Current.Server.MapPath(sVirtualFlatThumbnailsFolder);
            }

            if (ConfigurationManager.AppSettings["VirtualUploadFolder"] != null)
            {
                sVirtualUploadFolder = "/" + ConfigurationManager.AppSettings["VirtualUploadFolder"];
                sRealUploadFolder = HttpContext.Current.Server.MapPath(sVirtualUploadFolder);
            }

            if (ConfigurationManager.AppSettings["VirtualPPMUploadFolder"] != null)
            {
                sVirtualPPMUploadFolder = "/" + ConfigurationManager.AppSettings["VirtualPPMUploadFolder"];
                sRealPPMUploadFolder = HttpContext.Current.Server.MapPath(sVirtualPPMUploadFolder);
            }

            if (ConfigurationManager.AppSettings["VirtualArchiveFolder"] != null)
            {
                sVirtualArchiveFolder = "/" + ConfigurationManager.AppSettings["VirtualArchiveFolder"];
                sRealArchiveFolder = HttpContext.Current.Server.MapPath(sVirtualArchiveFolder);
            }

            if (ConfigurationManager.AppSettings["VirtualPlanFolder"] != null)
            {
                sVirtualPlanFolder = "/" + ConfigurationManager.AppSettings["VirtualPlanFolder"];
                sRealPlanFolder = HttpContext.Current.Server.MapPath(sVirtualPlanFolder);
            }

            Application["LogoWidth"] = Globals.ReadConfigInt32("LogoWidth", 200);

            Application["UseAdminGroups"] = Globals.ReadConfigBoolean("UseAdminGroups", false);
            Application["UseFlatThumbnails"] = Globals.ReadConfigBoolean("UseFlatThumbnails", false);
            Application["OnlyUseFlatThumbnails"] = Globals.ReadConfigBoolean("OnlyUseFlatThumbnails", false);

            Application["UseTreeState"] = false;

            Application["DefaultTreeExpansion"] = Globals.ReadConfigInt32("DefaultTreeExpansion", 2);

  
            Application["ProcessID"] = Globals.ReadConfigInt32("ProcessID", 99);
            Application["LogPlanning"] = Globals.ReadConfigBoolean("LogPlanning", false);

            refreshCacheInterval = Globals.ReadConfigInt32("RefreshCacheInterval", 0);

            Application["ReportShowViewTime"] = Globals.ReadConfigBoolean("ReportShowViewTime", false);
            Application["ReportShowDeadline"] = Globals.ReadConfigBoolean("ReportShowDeadline", false);
            Application["ReportShowAfterDeadline"] = Globals.ReadConfigBoolean("ReportShowAfterDeadline", false);

            Application["ReportShowCMYKInk"] = Globals.ReadConfigBoolean("ReportShowCMYKInk", false);

            Application["ReportShowReadyTime"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["ReportShowReadyTime"]) == 1;

            Application["ExtendedThumbnailViewShowFTP"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["ExtendedThumbnailViewShowFTP"]) == 1;
            Application["ExtendedThumbnailViewShowPreflight"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["ExtendedThumbnailViewShowPreflight"]) == 1;
            Application["ExtendedThumbnailViewShowRIP"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["ExtendedThumbnailViewShowRIP"]) == 1;
            Application["ExtendedThumbnailViewShowColorWarning"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["ExtendedThumbnailViewShowColorWarning"]) == 1;

            Application["ExtendedThumbnailViewShowInkSave"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["ExtendedThumbnailViewShowInkSave"]) == 1;

            Application["UploadAdminOnly"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["UploadAdminOnly"]) == 1;
            Application["HideDoubleBurnColors"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["HideDoubleBurnColors"]) == 1;

            Application["ShowDeadline"] = Globals.ReadConfigBoolean("ShowDeadline", false);

            Application["BlankOnEntry"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["BlankOnEntry"]) == 1;
            Application["MessageSystem"] = Globals.ReadConfigBoolean("MessageSystem", false);

            Application["HidePlanWeekNumber"] = Globals.ReadConfigBoolean("HidePlanWeekNumber", false);
            Application["HidePlanPressTime"] = Globals.ReadConfigBoolean("HidePlanPressTime", true);
            Application["HidePlanCirculation"] = Globals.ReadConfigBoolean("HidePlanCirculation", false);
            Application["HidePlanCirculation2"] = Globals.ReadConfigBoolean("HidePlanCirculation2", false);

            Application["HidePlanPageOffset"] = Globals.ReadConfigBoolean("HidePlanPageOffset", false);
            Application["HidePlanPagePrefix"] = Globals.ReadConfigBoolean("HidePlanPagePrefix", false);
            Application["HidePlanPagePostfix"] = Globals.ReadConfigBoolean("HidePlanPagePostfix", false);

            Application["HidePlanCombineSections"] = Globals.ReadConfigBoolean("HidePlanCombineSections", false);

            Application["HideDeletePlanButton"] = Globals.ReadConfigBoolean("HideDeletePlanButton", false);
            Application["HideEditPlanButton"] = Globals.ReadConfigBoolean("HideEditPlanButton", false);
            Application["HideAddPlanButton"] = Globals.ReadConfigBoolean("HideAddPlanButton", false);
            Application["PlanPdfColorAllowed"] = Globals.ReadConfigBoolean("PlanPdfColorAllowed", true);
            Application["PlanMonoColorAllowed"] = Globals.ReadConfigBoolean("PlanMonoColorAllowed", true);
            Application["PlanSpecialColorAllowed"] = Globals.ReadConfigBoolean("PlanSpecialColorAllowed", true);
            Application["HidePlanPriority"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["HidePlanPriority"]) == 1;
            Application["HidePlanPress"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["HidePlanPress"]) == 1;
            Application["HidePlanTemplate"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["HidePlanTemplate"]) == 1;
            Application["HidePlanPageFormat"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["HidePlanPageFormat"]) == 1;

            Application["PlanLockSystem"] = Globals.ReadConfigInt32("PlanLockSystem", 0);

            Application["UseVersionPreviews"] = Globals.ReadConfigBoolean("UseVersionPreviews", true);


            Application["GrayModeHeatMap"] = Globals.ReadConfigBoolean("GrayModeHeatMap", true);

            Application["ShowHistory"] = Globals.ReadConfigBoolean("ShowHistory", false);
            Application["NoCache"] = Globals.ReadConfigBoolean("NoCache", false);
            Application["HideDownload"] = Globals.ReadConfigBoolean("HideDownload", false);
            Application["AllowCustomReports"] = Globals.ReadConfigBoolean("AllowCustomReports", false);
            Application["ExportPlanFile"] = Globals.ReadConfigInt32("ExportPlanFile", 0);
            Application["ShowPitstopReports"] = Globals.ReadConfigBoolean("ShowPitstopReports", false);
            Application["ShowInksaveReports"] = Globals.ReadConfigBoolean("ShowInksaveReports", false);
            Application["ShowAnnotatedPDF"] = Globals.ReadConfigBoolean("ShowAnnotatedPDF", false);
            Application["FlatLook"] = Globals.ReadConfigBoolean("FlatLook", false);
            Application["PlanPageNameIsFileName"] = Globals.ReadConfigBoolean("PlanPageNameIsFileName", true);
            Application["UseWeeknumberAsComment"] = Globals.ReadConfigBoolean("UseWeeknumberAsComment", true);

            Application["IncludePageHistory"] = Globals.ReadConfigBoolean("IncludePageHistory", false);
            Application["CheckFlatProofStatus"] = Globals.ReadConfigBoolean("CheckFlatProofStatus", false);

            Application["OrderSystem"] = Globals.ReadConfigBoolean("OrderSystem", false);
            Application["OrderNumberFormat"] = Globals.ReadConfigString("OrderNumberFormat", "%M");
            Application["UniqueOrderNumberPerPressRun"] = Globals.ReadConfigBoolean("UniqueOrderNumberPerPressRun", false);

            Application["LogUserAccess"] = Globals.ReadConfigBoolean("LogUserAccess", false);
            Application["FlashVars"] = Globals.ReadConfigString("FlashVars","");
            Application["UseFlashViewer"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["UseFlashViewer"]) == 1;
            Application["TranslateTableHeaders"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["TranslateTableHeaders"]) == 1;

            Application["ShowLocationSelector"] = Globals.ReadConfigBoolean("ShowLocationSelector", false);
            Application["ShowPressSelector"] = Globals.ReadConfigBoolean("ShowPressSelector", false);


            Application["PressesToIgnore"] = Globals.ReadConfigString("PressesToIgnore", "");


            Application["ApproveOnNextButton"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["ApproveOnNextButton"]) == 1;

            Application["AllowPartialProofs"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["AllowPartialProofs"]) == 1;

            Application["ShowWeeknumberInTree"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["ShowWeeknumberInTree"]) == 1;
            Application["ShowCustomerInTree"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["ShowCustomerInTree"]) == 1;

            
            Application["ShowEditionCommentInTree"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["ShowEditionCommentInTree"]) == 1;

            Application["ReportShowArchive"] = Globals.ReadConfigBoolean("ReportShowArchive", false);

            Application["ShowOrdernumberInTree"] = Globals.ReadConfigBoolean("ShowOrdernumberInTree", false);

            Application["ShowAliasInTree"] = Globals.ReadConfigBoolean("ShowAliasInTree", false);

            Application["ShowAliasInTreePrefix"] = Globals.ReadConfigBoolean("ShowAliasInTreePrefix", false);
            

            Application["ShowInkAliasInTree"] = Globals.ReadConfigBoolean("ShowInkAliasInTree", false);
            
            Application["ShowCommentInTree"] = Globals.ReadConfigBoolean("ShowCommentInTree", false);

            Application["ShowAliasInTreeChar"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["ShowAliasInTreeChar"]);


            Application["ReportShowMonthly"] = Globals.ReadConfigBoolean("ReportShowMonthly", false);
            Application["RunViewPageSize"] = Globals.ReadConfigInt32("RunViewPageSize", 15);

            Application["LocationIsPress"] = Globals.ReadConfigBoolean("LocationIsPress", false);
            Application["SimpleFlatView"] = Globals.ReadConfigBoolean("SimpleFlatView", true);
            Application["FlatViewShowSheetNumber"] = Globals.ReadConfigBoolean("FlatViewShowSheetNumber", false);

            Application["RotateFlats"] = Globals.ReadConfigInt32("RotateFlats", 0);

            Application["LocationsToIgnore"] = Globals.ReadConfigString("LocationsToIgnore", "");

            Application["UpdateApproveTimeOnRelease"] = Globals.ReadConfigBoolean("UpdateApproveTimeOnRelease", false);
            Application["FlatViewShowApproveButton"] = Globals.ReadConfigBoolean("FlatViewShowApproveButton", false);

            Application["ShowPdfIfExists"] = Globals.ReadConfigBoolean("ShowPdfIfExists", false);

            Application["HidePlanWeekNumber"] = Globals.ReadConfigBoolean("HidePlanWeekNumber", false);

            Application["AlwaysAutoApply"] = Globals.ReadConfigInt32("AlwaysAutoApply", 0); // Changed from Globals.ReadConfigBoolean("AlwaysAutoApply", false);

            Application["CheckJpegComment"] = Globals.ReadConfigBoolean("CheckJpegComment", true);

            Application["ReTransmitButton"] = Globals.ReadConfigBoolean("ReTransmitButton", false);

            Application["HidePlanProofer"] = Globals.ReadConfigBoolean("HidePlanProofer", true);

            Application["ThumbnailShowStatusColors"] = Globals.ReadConfigBoolean("ThumbnailShowStatusColors", false);

            Application["ThumbnailShowMask"] = Globals.ReadConfigBoolean("ThumbnailShowMask", true);

            Application["MustUsePressRunMask"] = Globals.ReadConfigBoolean("MustUsePressRunMask", true);

            Application["DefaultToMaskImage"] = Globals.ReadConfigBoolean("DefaultToMaskImage", true);
            Application["HideCopyInfo"] = Globals.ReadConfigBoolean("HideCopyInfo", true);


            Application["HideListView"] = Globals.ReadConfigBoolean("HideListView", true);
            Application["ListViewAdminOnly"] = Globals.ReadConfigBoolean("ListViewAdminOnly", false);
            


            Application["FieldExists_WebUserLog_Message"] = false;
            Application["StoredProcParamExists_spThumbnailPageList2_PressID"] = false;

            Application["UseVersionThumbnails"] = Globals.ReadConfigBoolean("UseVersionThumbnails", false);

            Application["UseVersionFlats"] = Globals.ReadConfigBoolean("UseVersionFlats", false);

            Application["ShowApproveAllButton"] = Globals.ReadConfigBoolean("ShowApproveAllButton", false);
            Application["ShowRetransmitAllButton"] = Globals.ReadConfigBoolean("ShowRetransmitAllButton", false);

            Application["UseChannels"] = Globals.ReadConfigBoolean("UseChannels", false);


            Application["AlwaysPlanCMYK"] = Globals.ReadConfigBoolean("AlwaysPlanCMYK", false);

            Application["PlanCMYKColorAllowed"] = Globals.ReadConfigBoolean("PlanCMYKColorAllowed", true);
            if ((bool)Application["PlanCMYKColorAllowed"] == false)
                Application["AlwaysPlanCMYK"] = false;

            Application["AllowPDFView"] = Globals.ReadConfigBoolean("AllowPDFView", false);
            Application["AllowPDFDownload"] = Globals.ReadConfigBoolean("AllowPDFDownload", false);

            Application["AllowColorChange"] = Globals.ReadConfigBoolean("AllowColorChange", true);

            Application["AllowReproof"] = Globals.ReadConfigBoolean("AllowReproof", true);

            Application["CommonPageIndication"] = Globals.ReadConfigBoolean("CommonPageIndication", true);

            Application["IndicatePreflightError"] = Globals.ReadConfigBoolean("IndicatePreflightError", false);

            Application["PlanByTemplate"] = Globals.ReadConfigBoolean("PlanByTemplate", false);

            Application["PublicationPlanLockSystem"] = Globals.ReadConfigInt32("PublicationPlanLockSystem", 0);

            if ((bool)Application["PlanByTemplate"])
                Application["HidePlanTemplate"] = false;

            Application["UploaderUrl"] = Globals.ReadConfigString("UploaderUrl", "");

            //  Application["DevicesToIgnore"] = (string)ConfigurationManager.AppSettings["DevicesToIgnore"];
            //  Application["DevicesToAdd"] = (string)ConfigurationManager.AppSettings["DevicesToAdd"];


            Application["LogLocations"] = Globals.ReadConfigString("LogLocations", "");
            Application["MaxLogMessageLength"] = Globals.ReadConfigInt32("MaxLogMessageLength", 50);
            Application["MaxLogEntries"] = Globals.ReadConfigInt32("MaxLogEntries", 150);


            Application["RunCustomRelease"] = Globals.ReadConfigBoolean("RunCustomRelease", false);

            Application["FlatViewShowPlateNumber"] = Globals.ReadConfigBoolean("FlatViewShowPlateNumber", false);

            Application["AllowFlatproof"] = Globals.ReadConfigBoolean("AllowFlatproof", false);

            Application["AllowPageDelete"] = Globals.ReadConfigBoolean("AllowPageDelete", false);
            Application["AllowPageLock"] = Globals.ReadConfigBoolean("AllowPageLock", false);
            Application["ShowBigLock"] = Globals.ReadConfigBoolean("ShowBigLock", false);

            Application["ShowCustomAction"] = Globals.ReadConfigBoolean("ShowCustomAction", false);
            Application["ShowReadyAction"] = Globals.ReadConfigBoolean("ShowReadyAction", false);

            Application["HideApproveButton"] = Globals.ReadConfigBoolean("HideApproveButton", false);

            Application["LogReportPeriodHours"] = Globals.ReadConfigInt32("LogReportPeriodHours", 48);

            Application["FlatDependOnExtStatus"] = Globals.ReadConfigBoolean("FlatDependOnExtStatus", false);
            Application["FlatDependOnExtStatusNumber"] = Globals.ReadConfigInt32("FlatDependOnExtStatusNumber", 30);

            Application["NextButtonOrderByPageIndex"] = Globals.ReadConfigBoolean("NextButtonOrderByPageIndex", true);

            Application["SmallEventIcons"] = Globals.ReadConfigBoolean("SmallEventIcons", false);

            Application["HideOldProducts"] = Globals.ReadConfigBoolean("HideOldProducts", false);
            Application["UsePressGroups"] = Globals.ReadConfigBoolean("UsePressGroups", false);

            Application["PlanRunPostProcedure"] = Globals.ReadConfigBoolean("PlanRunPostProcedure", true);
            Application["PlanRunPostProcedure2"] = Globals.ReadConfigBoolean("PlanRunPostProcedure2", false);
            Application["PlanPressSpecificPages"] = Globals.ReadConfigBoolean("PlanPressSpecificPages", false);
            Application["PlanAlwaysPDF"] = Globals.ReadConfigBoolean("PlanAlwaysPDF", false);
            Application["PlanRestrictToOneSection"] = Globals.ReadConfigBoolean("PlanRestrictToOneSection", true);
     


            Application["PlanRunPreProcedure"] = Globals.ReadConfigBoolean("PlanRunPreProcedure", true);

			Application["AlwaysSetPressRunPubDate"] = Globals.ReadConfigBoolean("AlwaysSetPressRunPubDate", false);
            Application["SetPressRunPubDateToTreePubdate"] = Globals.ReadConfigBoolean("SetPressRunPubDateToTreePubdate", false);

            Application["PressRunShowInkComment"] = Globals.ReadConfigBoolean("PressRunShowInkComment", false);

            Application["PressRunPrePollStatus"] = Globals.ReadConfigBoolean("PressRunPrePollStatus", false);
            Application["PressRunSortedStatus"] = Globals.ReadConfigBoolean("PressRunSortedStatus", false);

            Application["PlanViewAdminOnly"] = Globals.ReadConfigBoolean("PlanViewAdminOnly", false);
            Application["StatisticsAdminOnly"] = Globals.ReadConfigBoolean("StatisticsAdminOnly", false);
            Application["LogViewAdminOnly"] = Globals.ReadConfigBoolean("LogViewAdminOnly", false);
            Application["FlatViewAdminOnly"] = Globals.ReadConfigBoolean("FlatViewAdminOnly", false);
            Application["TableViewAdminOnly"] = Globals.ReadConfigBoolean("TableViewAdminOnly", false);
            Application["ReadViewAdminOnly"] = Globals.ReadConfigBoolean("ReadViewAdminOnly", false);
            Application["RunViewAdminOnly"] = Globals.ReadConfigBoolean("RunViewAdminOnly", false);
            Application["UnknownFilesAdminOnly"] = Globals.ReadConfigBoolean("UnknownFilesAdminOnly", false);

            Application["ShowPlanPageName"] = Globals.ReadConfigBoolean("ShowPlanPageName", false);
            Application["PlanPageNameFormat"] = Globals.ReadConfigString("PlanPageNameFormat", "%Q");
            if ((string)Application["PlanPageNameFormat"] == "")
                Application["PlanPageNameFormat"] = "%Q";
            Application["PlanPageNameDateFormat"] = Globals.ReadConfigString("PlanPageNameDateFormat", "DDMM");
            Application["ShowPlanPageNameAsTooltip"] = Globals.ReadConfigBoolean("ShowPlanPageNameAsTooltip", false);

            Application["SuperUserMaySeeAll"] = Globals.ReadConfigBoolean("SuperUserMaySeeAll", false);

            Application["HideTableView"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["HideTableView"]) == 1;
            Application["HideFlatView"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["HideFlatView"]) == 1;
            Application["HideRunView"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["HideRunView"]) == 1;
            Application["HideLogView"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["HideLogView"]) == 1;
            Application["HidePlanView"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["HidePlanView"]) == 1;
            Application["HideReadView"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["HideReadView"]) == 1;
            Application["HideStatistics"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["HideStatistics"]) == 1;
            Application["HideUnknownFiles"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["HideUnknownFiles"]) == 1;

            Application["DefaultUploadFolder"] = Globals.ReadConfigString("DefaultUploadFolder", "Uploads");
            Application["RunViewHideDevices"] = Globals.ReadConfigBoolean("RunViewHideDevices", true);
            Application["ShowPageCountInTree"] = Convert.ToInt32((string)ConfigurationManager.AppSettings["ShowPageCountInTree"]);
            Application["PlanUseXml"] = Globals.ReadConfigBoolean("PlanUseXml", true);

            Application["ColumnOrder"] = Globals.ReadConfigString("ColumnOrder", "");
            SetDefaultColumnOrder();

            Application["ShowCustomMenu"] = Globals.ReadConfigBoolean("ShowCustomMenu", true);
            Application["ShowCustomMenuScript"] = Globals.ReadConfigString("ShowCustomMenuScript", "ShowSpecialLog.aspx"); 
            Application["ShowCustomMenuName"] = Globals.ReadConfigString("ShowCustomMenuName", "Show special log");

            Application["Loglevel"] = Globals.ReadConfigInt32("Loglevel", 0);

            Application["DefaultTreeExpansionShowAllEditions"] = Globals.ReadConfigBoolean("DefaultTreeExpansionShowAllEditions", false);
            Application["DefaultTreeExpansionShowAllSections"] = Globals.ReadConfigBoolean("DefaultTreeExpansionShowAllSections", false);

//            Application["UseChannels"] = Globals.ReadConfigBoolean("UseChannels", true);
            Application["UseExtendedPlanning"] = Globals.ReadConfigInt32("UseExtendedPlanning", 0);

            Application["ShowResetApprove"] = Globals.ReadConfigBoolean("ShowResetApprove", false);
            Application["ShowAutoapprovedGreen"] = Globals.ReadConfigBoolean("ShowAutoapprovedGreen", false);

             Application["PlanPageNameInputTimeFormat"] = Globals.ReadConfigInt32("PlanPageNameInputTimeFormat", 1);

             Application["FlatHideRelease"] = Globals.ReadConfigBoolean("FlatHideRelease", false);

             Application["OldFileNames"] = Globals.ReadConfigBoolean("OldFileNames", false);
             Application["PubDateFormat"] = Globals.ReadConfigInt32("PubDateFormat", 0);
             Application["WarningTemplateID"] = Globals.ReadConfigInt32("WarningTemplateID", 0);
             Application["AutoReproofSystem"] = Globals.ReadConfigBoolean("AutoReproofSystem", false);

             Application["ShowSectionCommentInTree"] = Globals.ReadConfigBoolean("ShowSectionCommentInTree", false);

             Application["CheckThumbnailSize"] = Globals.ReadConfigBoolean("CheckThumbnailSize", false);


             Application["HideUploadPlanButton"] = Globals.ReadConfigBoolean("HideUploadPlanButton", true);

             Application["AllowDisapproveForward"] = Globals.ReadConfigBoolean("AllowDisapproveForward", false);

             Application["AllowUpload"] = Globals.ReadConfigBoolean("AllowUpload", false); 
             Application["AllowReprocess"] = Globals.ReadConfigBoolean("AllowReprocess", false);

             Application["HidePlanApprovalRequired"] = Globals.ReadConfigBoolean("HidePlanApprovalRequired", false);

             Application["ShowWeeknumberInTreeFilter"] = Globals.ReadConfigBoolean("ShowWeeknumberInTreeFilter", false);

             Application["UploadViewUseNameRule"] = Globals.ReadConfigBoolean("UploadViewUseNameRule", false);

            Application["UsePageNameAsUploadFileName"] = Globals.ReadConfigBoolean("UsePageNameAsUploadFileName", false);
            Application["AddExtensionToUploadFileName"] = Globals.ReadConfigString("AddExtensionToUploadFileName", "");

            Application["PlanPreferredEdition"] = Globals.ReadConfigString("PlanPreferredEdition", "");
            Application["PlanAlwaysUseDefaultPress"] = Globals.ReadConfigBoolean("PlanAlwaysUseDefaultPress", true);
         
            Application["CustomUploadFileNames"] = Globals.ReadConfigInt32("CustomUploadFileNames", 0);

            Application["UseAD"] = Globals.ReadConfigBoolean("UseAD", false);
            Application["SimpleAD"] = Globals.ReadConfigBoolean("SimpleAD", true);            
            Application["ADpath"] = Globals.ReadConfigString("ADpath", "");
            Application["ADdomain"] = Globals.ReadConfigString("ADdomain", "");
            Application["ADpersistantcookie"] = Globals.ReadConfigBoolean("ADpersistantcookie", true);
            Application["AutoCreateADuser"] = Globals.ReadConfigBoolean("AutoCreateADuser", false);
            Application["AutoCreateADuserAdGroupMapping"] = Globals.ReadConfigString("AutoCreateADuserAdGroupMapping", "Administrative Groups Infralogic=MANAGER,Droits Infralogic AutreA=PREPRESS");
            
            Application["PressTemplateFolder"] = Globals.ReadConfigString("PressTemplateFolder", "");
            Application["PressTemplateFileExtension"] = Globals.ReadConfigString("PressTemplateFileExtension", "");
            Application["PressTemplateOutputFolder"] = Globals.ReadConfigString("PressTemplateOutputFolder", "");

            Application["SetCommentInPrePollPageTable"] = Globals.ReadConfigBoolean("SetCommentInPrePollPageTable", true);

            Application["PDFBookShowHeader"] = Globals.ReadConfigInt32("PDFBookShowHeader", 1);

            Application["PDFBookCreator"] = Globals.ReadConfigString("PDFBookCreator", "WebCenter");
            Application["PDFBookAuthor"] = Globals.ReadConfigString("PDFBookAuthor", "InfraLogic");
            Application["PDFbookHeaderDefinition"] = Globals.ReadConfigString("PDFbookHeaderDefinition", "%P-%D-%E-%S-%N    %T");
            Application["PDFbookHeaderPubDateDefinition"] = Globals.ReadConfigString("PDFbookHeaderPubDateDefinition", "DD.MM.YYYY");
            Application["PDFbookHeaderLogo"] = Globals.ReadConfigString("PDFbookHeaderLogo", "");
            Application["PDFbookHeaderLogoPosition"] = Globals.ReadConfigString("PDFbookHeaderLogoPosition", "0,0");
            Application["PDFBookHeaderXpos"] = Globals.ReadConfigInt32("PDFBookHeaderXpos", 20);
            Application["PDFBookHeaderYpos"] = Globals.ReadConfigInt32("PDFBookHeaderYpos", 5);
            Application["PDFBookHeaderFontSize"] = Globals.ReadConfigInt32("PDFBookHeaderFontSize", 14);

            Application["DisableAdminUser"] = Globals.ReadConfigBoolean("DisableAdminUser", false);

            Application["AllowInsertPages"] = Globals.ReadConfigBoolean("AllowInsertPages", false);

            Application["AllowInsertPagesFileNamePattern"] = Globals.ReadConfigString("AllowInsertPagesFileNamePattern", "%P-%D-%E-%S-%N.pdf");
            Application["AllowInsertPagesFileNameDatePattern"] = Globals.ReadConfigString("AllowInsertPagesFileNameDatePattern", "DDMMYY");

            Application["ExcelSavetype"] = Globals.ReadConfigInt32("ExcelSavetype", 0);
            Application["ExcelTimeFormat"] = Globals.ReadConfigString("ExcelTimeFormat", "dd/MM/yyyy HH:mm:ss");
            Application["ExcelDateFormat"] = Globals.ReadConfigString("ExcelDateFormat", "dd/MM/yyyy");
            Application["ShowRasterImage"] = Globals.ReadConfigBoolean("ShowRasterImage", false);


            Application["TreeWhiteStatusUntilAllReceived"] = Globals.ReadConfigBoolean("TreeWhiteStatusUntilAllReceived", false);
            Application["PressRunDefaultToAllDates"] = Globals.ReadConfigBoolean("PressRunDefaultToAllDates", false);

            Application["TreePreventAllSelection"] = Globals.ReadConfigBoolean("TreePreventAllSelection", false);

            Application["ReadyActionSetComment"] = Globals.ReadConfigBoolean("ReadyActionSetComment", false);

            Application["ThumbnailsCommentAsPageNumber"] = Globals.ReadConfigBoolean("ThumbnailsCommentAsPageNumber", false);

           // Application["AutoDetectFlash"] = Globals.ReadConfigBoolean("AutoDetectFlash", false);
            Application["ForceHTML5"] = Globals.ReadConfigBoolean("ForceHTML5", true);

            Application["UseInputTimeInThumbnailName"] = Globals.ReadConfigBoolean("UseInputTimeInThumbnailName", false);

            // 1: ProductDeleteService TIFF retry, 2: FileCenter PDF retry
            Application["AutoRetryPlanFiles"] = Globals.ReadConfigInt32("AutoRetryPlanFiles", 1);


            Application["PlanningCustomPressSelection"] = Globals.ReadConfigBoolean("PlanningCustomPressSelection", false);

            Application["AllowPermanentDelete"] = Globals.ReadConfigBoolean("AllowPermanentDelete", false);

            Application["ExtendedThumbnailViewShowPDFTypes"] = Globals.ReadConfigBoolean("ExtendedThumbnailViewShowPDFTypes", false);

            
            // CustomUploadNameFormat

            logging = new Logger(sRealLogFolder + "\\WebCenter4.log");
            logging.WriteLog("WebCenter init..");

            rm = new ResourceManager("WebCenter4.strings", typeof(Login).Assembly);
            encoding = (string)ConfigurationManager.AppSettings["Encoding"];
            language = (string)ConfigurationManager.AppSettings["Language"];

            culture = Globals.GetCulture(language);
            encoding = Globals.GetEncoding(language);


            pagesInTree = (string)ConfigurationManager.AppSettings["PagesInTree"] == "1";
            
            ExamineDatabaseVersion();

            RefreshEditionNameCache(null, null, 0);
            RefreshColorNameCache(null, null, 0);
            RefreshSectionNameCache(null, null, 0);
            RefreshStatusNameCache(null, null, 0);
            RefreshExtStatusNameCache(null, null, 0);

            RefreshPublicationNameCache(null, null, 0);
            RefreshIssueNameCache(null, null, 0);
            RefreshPressNameCache(null, null, 0);
            RefreshLocationNameCache(null, null, 0);
            RefreshDeviceNameCache(null, null, 0);
            RefreshTemplateNameCache(null, null, 0);
            RefreshTemplatePressNameCache(null, null, 0);
            RefreshProofNameCache(null, null, 0);
          //  RefreshPlanNameCache(null, null, 0);
            RefreshEventNameCache(null, null, 0);
            RefreshPageFormatCache(null, null, 0);
            RefreshTemplatePageFormatCache(null, null, 0);
            RefreshHardProofNameCache(null, null, 0);
            RefreshPublicationEditionsCache(null, null, 0);
            RefreshPublicationSectionsCache(null, null, 0);
            RefreshPublicationTemplateCache(null, null, 0);
            RefreshPressGroupNameCache(null, null, 0);

            RefreshInputAliasCache(null, null, 0);
            RefreshInkAliasCache(null, null, 0);

            RefreshPublicationNamingConvensionCache(null, null, 0);
            RefreshRipSetupNameCache(null, null, 0);
            RefreshPreflightSetupNameCache(null, null, 0);
            RefreshInksaveSetupNameCache(null, null, 0);

            if ((bool)Application["UseChannels"])
            {
                RefreshChannelNameCache(null, null, 0);
                RefreshPublisherNameCache(null, null, 0);

            }
            logging.WriteLog("Name caches loaded");

            if ((int)Application["UseExtendedPlanning"] == 3)
            {
                RefreshPPMPressNamesCache(null, null, 0);
                RefreshPPMPublicationNamesCache(null, null, 0);
                RefreshPPMPaperNamesCache(null, null, 0);
                RefreshPPMEditionNamesCache(null, null, 0);
                RefreshPPMPageFormatNamesCache(null, null, 0);


            }

            /*
                if (timer == null && refreshCacheInterval > 0)
                {
                    timer = new Timer(new TimerCallback(ScheduledRefreshCacheCallback), HttpContext.Current, refreshCacheInterval, refreshCacheInterval);
                    ScheduledRefreshCacheCallback(HttpContext.Current);
                }

                if (messageTimer == null && refreshMessageInterval > 0)
                {
                    messageTimer = new Timer(new TimerCallback(ScheduledRefreshMessageCallback), HttpContext.Current, refreshMessageInterval, refreshMessageInterval);
                    ScheduledRefreshMessageCallback(HttpContext.Current);
                }
            */
        }

        
        private void SetDefaultColumnOrder()
        {
            string[] colNames = {	"Edition",	"Section",	"Page",			"Color",		"Status",		"Version", 
									"Approval",		"Hold",		"Priority", "Template", "Device",	"ExternalStatus","CopyNumber", "Pagination",	"Press",	"LastError",	"Comment",
									"DeadLine",		"ProofStatus", "Location", "SheetNumber", "SheetSide", "PagePositions", "PageType", "PagesPerPlate",
									"InputTime", "ApproveTime", "OutputTime", "VerifyTime", "Active", "Unique", "MasterCopySeparationSet", "SeparationSet",
									"FlatSeparationSet", "FlatSeparation", "Separation"};
            string[] colTypes =	{	"String",	"String",	"String",		"String",		"String",		"Int32", 
									"Int32",		"String",	"Int32",	"String",	"String",	"String",		"Int32",		"Int32",		"String",	"String",		"String",
									"String",		"Int32",	"String",	"Int32",	"String",	"String",		"Int32",		"Int32",	
									"String",		"String",	"String",	"String",	"Boolean",	"Boolean",		"Int32",		"Int32",	
									"Int32",		"Int64",	"Int64"};

            if ((string)Application["ColumnOrder"] == "")
            {
                string s = "";
                foreach (string s1 in colNames)
                {
                    if (s != "")
                        s += ",";
                    s += s1;
                }
                Application["ColumnOrder"] = s;
            }
        }

        public static void ScheduledRefreshCacheCallback(object sender)
        {
            HttpContext context = (HttpContext)sender;

            Globals.ForceCacheReloads();
        }

        public static void ScheduledRefreshMessageCallback(object sender)
        {
            HttpContext context = (HttpContext)sender;
        }

        public static void ExamineDatabaseVersion()
        {
            CCDBaccess db = new CCDBaccess();
            string errmsg = "";

            if (db.FieldExists("UserGroupNames", "MayUpload", out errmsg) == 1)
                HttpContext.Current.Application["FieldExists_UserGroupNames_MayUpload"] = true;
            else
                HttpContext.Current.Application["FieldExists_UserGroupNames_MayUpload"] = false;

             if (db.FieldExists("UserNames", "UseHTML5", out errmsg) == 1)
                HttpContext.Current.Application["FieldExists_UserNames_UseHTML5"] = true;
             else
                HttpContext.Current.Application["FieldExists_UserNames_UseHTML5"] = false;

            if (db.FieldExists("UserNames", "LastLoginTime", out errmsg) == 1)
                HttpContext.Current.Application["FieldExists_UserNames_LastLoginTime"] = true;
            else
                HttpContext.Current.Application["FieldExists_UserNames_LastLoginTime"] = false;

            if (db.FieldExists("WebUserLog", "Message", out errmsg) == 1)
                HttpContext.Current.Application["FieldExists_WebUserLog_Message"] = true;
            else
                HttpContext.Current.Application["FieldExists_WebUserLog_Message"] = false;

            if (db.StoredProcParameterExists("spThumbnailPageList2", "PressID", out errmsg) == 1)
                HttpContext.Current.Application["StoredProcParamExists_spThumbnailPageList2_PressID"] = true;
            else
                HttpContext.Current.Application["StoredProcParamExists_spThumbnailPageList2_PressID"] = false;

            if (db.TableExists("PublicationLocks", out errmsg) == 1)
                HttpContext.Current.Application["TableExists_PublicationLocks"] = true;
            else
                HttpContext.Current.Application["TableExists_PublicationLocks"] = false;

            if (db.StoredProcParameterExists("spCustomRelease", "ColorID", out errmsg) == 1)
                HttpContext.Current.Application["StoredProcParamExists_spCustomRelease_ColorID"] = true;
            else
                HttpContext.Current.Application["StoredProcParamExists_spCustomRelease_ColorID"] = false;

            if (db.TableExists("UserPresses", out errmsg) == 1)
                HttpContext.Current.Application["TableExists_UserPresses"] = true;
            else
                HttpContext.Current.Application["TableExists_UserPresses"] = false;

            if (db.TableExists("UserPublishers", out errmsg) == 1)
                HttpContext.Current.Application["TableExists_UserPublishers"] = true;
            else
                HttpContext.Current.Application["TableExists_UserPublishers"] = false;

            if (db.FieldExists("PageTable", "FlatMaster", out errmsg) == 1)
                HttpContext.Current.Application["FieldExists_PageTable_FlatMaster"] = true;
            else
                HttpContext.Current.Application["FieldExists_PageTable_FlatMaster"] = false;

            if (db.FieldExists("PageTable", "PageFormatID", out errmsg) == 1)
                HttpContext.Current.Application["FieldExists_PageTable_PageFormatID"] = true;
            else
                HttpContext.Current.Application["FieldExists_PageTable_PageFormatID"] = false;


            if (db.StoredProcParameterExists("pPressRunStatEx", "IgnoreDirty", out errmsg) == 1)
                HttpContext.Current.Application["spParamExists_spPressRunStatEx_IgnoreDirty"] = true;
            else
                HttpContext.Current.Application["spParamExists_spPressRunStatEx_IgnoreDirty"] = false;

            if (db.StoredProcedureExists("spPageStatusList", out errmsg) == 1)
                HttpContext.Current.Application["spExists_spPageStatusList"] = true;
            else
                HttpContext.Current.Application["spExists_spPageStatusList"] = false;

            if (db.StoredProcedureExists("spFullProductionStatistics", out errmsg) == 1)
                HttpContext.Current.Application["spExists_spFullProductionStatistics"] = true;
            else
                HttpContext.Current.Application["spExists_spFullProductionStatistics"] = false;
        }

        public static void RefreshEditionNameCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetNamesCollection("EditionNames", "GetEditionNames", out errmsg);

            if (dt != null && errmsg == "")
            {
                if (dt.Rows.Count > 0)
                {
                    _cache.Insert( "EditionNameCache", dt);
                }
            }
        }

        public static void RefreshPublicationNameCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetPublicationCollection(out errmsg);

            if (dt != null && errmsg == "")
            {
                if (dt.Rows.Count > 0)
                {
                    
                    _cache.Insert("PublicationNameCache", dt);

                }
            }
        }



        public static void RefreshSectionNameCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetNamesCollection("SectionNames", "GetSectionNames", out errmsg);

            if (dt != null && errmsg == "")
            {
                if (dt.Rows.Count > 0)
                {
                    _cache.Insert("SectionNameCache", dt);
                }
            }
        }

        public static void RefreshIssueNameCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetNamesCollection("IssueNames", "GetIssueNames", out errmsg);

            if (dt != null && errmsg == "")
            {
                if (dt.Rows.Count > 0)
                {
                    _cache.Insert("IssueNameCache", dt);
                }
            }
        }

        public static void RefreshPublicationNamingConvensionCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetPublicationNamingConvensionCollection(out errmsg);

            if (dt != null && errmsg == "")
            {
                if (dt.Rows.Count > 0)
                {
                    _cache.Insert("PublicationNamingConvensionCache", dt);
                }
            }
        }


        public static void RefreshProofNameCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetNamesCollection("ProofNames", "GetProofNames", out errmsg);

            if (dt != null && errmsg == "")
            {
                if (dt.Rows.Count > 0)
                {

                    _cache.Insert("ProofNameCache",dt);
                }
            }
        }

        public static void RefreshHardProofNameCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;

            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetHardProofCollection(out errmsg);

            if (dt != null && errmsg == "")
            {
                if (dt.Rows.Count > 0)
                {
                     _cache.Insert("HardProofNameCache", dt);
                }
            }
        }

        public static void RefreshPublicationEditionsCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;

            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetPublicationEditionsCollection(out errmsg);

            if (dt != null && errmsg == "")
            {
                if (dt.Rows.Count > 0)
                {
                    _cache.Insert("PublicationEditionsCache", dt);
                }
            }
        }

        public static void RefreshPublicationSectionsCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;

            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetPublicationSectionsCollection(out errmsg);

            if (dt != null && errmsg == "")
            {
                if (dt.Rows.Count > 0)
                {
                    _cache.Insert("PublicationSectionsCache", dt);
                }
            }
        }

        public static void RefreshPressNameCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

//            DataTable dt = db.GetNamesExCollection("PressNames", "GetPressNames", out errmsg);

            DataTable dt = db.GetPressCollection(out errmsg);

            if (dt != null && errmsg == "")
            {
                if (dt.Rows.Count > 0)
                {
                    _cache.Insert("PressNameCache", dt);
                }
            }
        }

        public static void RefreshPressGroupNameCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetPressGroupCollection(out errmsg);

            if (dt != null && errmsg == "")
            {
                if (dt.Rows.Count > 0)
                {
                    _cache.Insert("PressGroupNameCache", dt);
                }
            }
        }

        public static void RefreshRipSetupNameCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetRipSetupCollection(1,out errmsg);

            if (dt != null && errmsg == "")
            {
                if (dt.Rows.Count > 0)
                {
                    _cache.Insert("RipSetupNamesCache", dt);
                }
            }
        }

        public static void RefreshPreflightSetupNameCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetRipSetupCollection(2, out errmsg);

            if (dt != null && errmsg == "")
            {
                if (dt.Rows.Count > 0)
                {
                    _cache.Insert("PreflightSetupNamesCache", dt);
                }
            }
        }

        public static void RefreshInksaveSetupNameCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetRipSetupCollection(3, out errmsg);

            if (dt != null && errmsg == "")
            {
                if (dt.Rows.Count > 0)
                {
                    _cache.Insert("InksaveSetupNamesCache", dt);
                }
            }
        }

        public static void RefreshPageFormatCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetNamesCollection("PageFormatNames", "GetPageFormatNames", out errmsg);

            if (dt != null && errmsg == "")
            {
                if (dt.Rows.Count > 0)
                {
                    _cache.Insert("PageFormatCache", dt);
                }
            }
        }


       

        public static void RefreshPublicationPageFormatCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetNamesCollection("PublicationPageFormats", "GetPublicationPageFormats", out errmsg);

            if (dt != null && errmsg == "")
            {
                if (dt.Rows.Count > 0)
                {
                    _cache.Insert("PublicationPageFormatCache", dt);
                }
            }
        }

        public static void RefreshPublicationProofCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetNamesCollection("PublicationProof", "GetPublicationProofs", out errmsg);

            if (dt != null && errmsg == "")
            {
                _cache.Insert("PublicationProofCache", dt);
            }
        }

        public static void RefreshPublicationTemplateCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;

            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetPublicationTemplateCollection(out errmsg);

            if (dt != null && errmsg == "")
            {
                if (dt.Rows.Count > 0)
                {
                    _cache.Insert(
                        "PublicationTemplateCache",
                        dt,
                        new CacheDependency("C:\\Changed.PublicationTemplateCache"),
                        Cache.NoAbsoluteExpiration,
                        Cache.NoSlidingExpiration,
                        CacheItemPriority.Default,
                        new CacheItemRemovedCallback(RefreshPublicationTemplateCache)
                        );
                }
            }
        }

        public static void RefreshTemplatePageFormatCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetTemplatePageFormatCollection(out errmsg);

            if (dt != null && errmsg == "")
            {
                _cache.Insert("TemplatePageFormatCache", dt);
            }
        }

/*
        public static void RefreshPlanNameCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;

            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetPlanCollection(out errmsg);
            if (dt != null && errmsg == "")
            {
                _cache.Insert("PlanNameCache", dt);
            }
        }
*/
        public static void RefreshLocationNameCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetNamesCollection("LocationNames", "GetLocationNames", out errmsg);

            if (dt != null && errmsg == "")
            {
                if (dt.Rows.Count > 0)
                {
                    _cache.Insert("LocationNameCache", dt);
                }
            }
        }


        public static void RefreshDeviceNameCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetNamesExCollection("DeviceNames", "GetDeviceNames", out errmsg);

            if (dt != null && errmsg == "")
            {
                if (dt.Rows.Count > 0)
                {
                    _cache.Insert("DeviceNameCache", dt);
                }
            }
        }

        public static void RefreshTemplateNameCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetNamesCollection("TemplateNames", "GetTemplateNames", out errmsg);

            if (dt != null && errmsg == "")
            {
                if (dt.Rows.Count > 0)
                {
                    _cache.Insert("TemplateNameCache", dt);
                }
            }
        }


        public static void RefreshTemplatePressNameCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetTemplateCollection(out errmsg);

            if (dt != null && errmsg == "")
            {
                if (dt.Rows.Count > 0)
                {
                    _cache.Insert("TemplatePressCache",dt);
                }
            }
        }

        public static void RefreshColorNameCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetColorCollection("ColorNames", out errmsg);

            if (dt != null && errmsg == "")
            {
                if (dt.Rows.Count > 0)
                {
                    _cache.Insert("ColorNameCache", dt);
                }
            }
        }

        //String dataSetName, int nStatusType, out string errmsg)

        public static void RefreshStatusNameCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetStatusCollection("StatusNames", 0, out errmsg);

            if (dt != null && errmsg == "")
            {
                if (dt.Rows.Count > 0)
                {
                    _cache.Insert("StatusNameCache", dt);
                }
            }
        }

        public static void RefreshExtStatusNameCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetStatusCollection("ExtStatusNames", 1, out errmsg);

            if (dt != null && errmsg == "")
            {
                if (dt.Rows.Count > 0)
                {
                    _cache.Insert("ExtStatusNameCache",dt);
                }
            }
        }

        public static void RefreshEventNameCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;

            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetEventCollection("EventCodes", out errmsg);

            if (dt != null && errmsg == "")
            {
                _cache.Insert("EventNameCache", dt);
            }
        }


        public static void RefreshInputAliasCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetAliasCollection(out errmsg);

            if (dt != null && errmsg == "")
            {
                _cache.Insert("InputAliasCache", dt);
            }
        }

        public static void RefreshInkAliasCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetInkAliasCollection(out errmsg);

            if (dt != null && errmsg == "")
            {
                _cache.Insert("InkAliasCache", dt);
            }
        }


        public static void RefreshChannelNameCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetChannelCollection(out errmsg);

            if (dt != null && errmsg == "")
            {
                _cache.Insert("ChannelNameCache", dt);
            }
        }
        
        public static void RefreshPublisherNameCache(string key, object item, CacheItemRemovedReason reason)
        {
            string errmsg;
            CCDBaccess db = new CCDBaccess();

            DataTable dt = db.GetPublisherCollection(out errmsg);

            if (dt != null && errmsg == "")
            {
                _cache.Insert("PublisherNameCache", dt);
            }
        }

        public static void RefreshPPMPublicationNamesCache(string key, object item, CacheItemRemovedReason reason)
        {
            PPMDBaccess db = new PPMDBaccess();
            List<PPMPublication> publications = new List<PPMPublication>();
            if (db.LoadPublications(ref publications, out string errmsg))
            { 
                if (publications.Count > 0)
                {
                    _cache.Insert("PPMPublicationNamesCache", publications);
                }
            }
        }

        public static void RefreshPPMEditionNamesCache(string key, object item, CacheItemRemovedReason reason)
        {
            PPMDBaccess db = new PPMDBaccess();
            List<string> editions = new List<string>();
            if (db.LoadEditions(ref editions, out string errmsg))
            {
                if (editions.Count > 0)
                {
                    _cache.Insert("PPMEditionNamesCache", editions);
                }
            }
        }

        public static void RefreshPPMPaperNamesCache(string key, object item, CacheItemRemovedReason reason)
        {
            PPMDBaccess db = new PPMDBaccess();
            List<string> papers = new List<string>();
            if (db.LoadPapers(ref papers, out string errmsg))
            {
                if (papers.Count > 0)
                {
                    _cache.Insert("PPMPaperNamesCache", papers);
                }
            }
        }

        public static void RefreshPPMPressNamesCache(string key, object item, CacheItemRemovedReason reason)
        {
            PPMDBaccess db = new PPMDBaccess();
            List<string> presses = new List<string>();
            if (db.LoadPresses(ref presses, out string errmsg))
            {
                if (presses.Count > 0)
                {
                    _cache.Insert("PPMPressNamesCache", presses);
                }
            }
        }

        public static void RefreshPPMPageFormatNamesCache(string key, object item, CacheItemRemovedReason reason)
        {
            PPMDBaccess db = new PPMDBaccess();
            List<PPMPageFormat> pageFormats = new List<PPMPageFormat>();
            if (db.LoadPageFormats(ref pageFormats, out string errmsg))
            {
                if (pageFormats.Count > 0)
                {
                    _cache.Insert("PPMPageFormatNamesCache", pageFormats);
                }
            }
        }


        protected void Session_Start(Object sender, EventArgs e)
        {
            Session["culture"] = culture;
            Session["language"] = language;
            Session["encoding"] = encoding;
        }

        void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        /*
        void Application_BeginRequest(object sender, EventArgs e)
        {
            // Trying restore browser cookies because Flash Player do not send them in non-IE browsers

        try
            {
                string cookieParamName = "MultiPowUpload_browserCookie";			
			
                NameValueCollection browserCookie = new NameValueCollection();
                if(HttpContext.Current.Request.Form[cookieParamName] != null)
                {
                    browserCookie = getCookieArray(HttpContext.Current.Request.Form[cookieParamName]);
                    foreach (string s in browserCookie) 
                        UpdateCookie(s, browserCookie[s]);	
                }
            }
            catch (Exception ex)
            {			
                Response.Write("Error initializing session and (or) setting authentication info."+ex.Message);
            }
        }
        void UpdateCookie(string cookieName, string cookieValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies.Get(cookieName);
            if (cookie == null)
            {
                cookie = new HttpCookie(cookieName);
                HttpContext.Current.Request.Cookies.Add(cookie);
            }
            cookie.Value = cookieValue;
            HttpContext.Current.Request.Cookies.Set(cookie);
        }

        NameValueCollection getCookieArray(string cookie)
        {
            string[] split = cookie.Split(';');
            string[] splitParam = null;
            NameValueCollection  returnArr = new NameValueCollection();
            foreach (string s in split) 
            {
                splitParam = s.Split('=');	
                if(splitParam.Length > 1)
                    returnArr.Add(splitParam[0].Trim(), splitParam[1].Trim());			
                else
                    returnArr.Add(splitParam[0].Trim(), "");			

            }
            return returnArr;

        }
*/

        protected void Application_EndRequest(Object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            if ((bool)Application["UseAD"] && (bool)Application["SimpleAD"] == false)
            {
                string cookieName = FormsAuthentication.FormsCookieName;
                HttpCookie authCookie = Context.Request.Cookies[cookieName];

                if (null == authCookie)
                {
                    //There is no authentication cookie.
                    return;
                }
                FormsAuthenticationTicket authTicket = null;
                try
                {
                    authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                }
                catch // (Exception ex)
                {
                    //Write the exception to the Event Log.
                    return;
                }
                if (null == authTicket)
                {
                    //Cookie failed to decrypt.
                    return;
                }
                //When the ticket was created, the UserData property was assigned a
                //pipe-delimited string of group names.
                string[] groups = authTicket.UserData.Split(new char[] { '|' });
                //Create an Identity.
                GenericIdentity id = new GenericIdentity(authTicket.Name, "LdapAuthentication");
                //This principal flows throughout the request.
                GenericPrincipal principal = new GenericPrincipal(id, groups);
                Context.User = principal;
            }
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception objErr = Server.GetLastError().GetBaseException();
            string err = "Error Caught in Application_Error event\n" +
                "Error in: " + Request.Url.ToString() +
                "\nError Message:" + objErr.Message.ToString() +
                "\nStack Trace:" + objErr.StackTrace.ToString();
        //    EventLog.WriteEntry("WebCenter", err, EventLogEntryType.Error);

            //Server.ClearError();
            //additional actions...

        }

        protected void Session_End(Object sender, EventArgs e)
        {
            string errMsg = "";
            CCDBaccess db = new CCDBaccess();
            db.InsertUserHistory((string)Session["UserName"], 0, "Session timeout", out errMsg);
        }

        protected void Application_End(Object sender, EventArgs e)
        {

        }

    }
}
