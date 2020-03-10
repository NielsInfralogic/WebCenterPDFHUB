using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.Caching;
using System.Data;
using System.Web.SessionState;
using System.Configuration;
using WebCenter4.Classes;
using System.IO;
using System.Text;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Globalization;


namespace WebCenter4.Classes
{
	/// <summary>
	/// Summary description for Globals.
	/// </summary>
	public static class Globals
	{
		public  enum EventCodes {PlanStartCreate = 988, PlanStartDelete = 989, PlanCreate = 991, PlanEdit = 992, PlanDelete = 993};

        public static int LogoWidth { get; set; }
        public static bool AllowUpload { get; set; }
        public static bool HideDownload { get; set; }
        public static bool AllowPageLock { get; set; }
        public static bool AllowReproof { get; set; }
        public static bool AllowColorChange { get; set; }
        public static bool AllowPartialProofs { get; set; }
        

        public static bool MessageSystem { get; set; }
        public static bool ShowApproveAllButton { get; set; }
        public static bool ShowCustomAction { get; set; }
        public static bool ShowReadyAction { get; set; }
        public static bool ShowDeadline { get; set; }
        public static bool ShowPitstopReports { get; set; }

        public static bool ShowInksaveReports { get; set; }
        

        public static bool UsePressGroups { get; set; }

        public static bool UseChannels { get; set; }

      
        public static bool LocationIsPress { get; set; }

        public static bool FlatLook { get; set; }
        public static bool SmallEventIcons { get; set; }

        

        public static bool OldFileNames { get; set; }

        public static bool UseVersionPreviews { get; set; }
        public static bool UseVersionThumbnails { get; set; }

        public static bool CommonPageIndication { get; set; }

        public static bool UseInputTimeInThumbnailName { get; set; }
        public static bool ShowPdfIfExists { get; set; }

        public static bool ShowHistory { get; set; }
        public static bool ShowBigLock { get; set; }

        public static bool ThumbnailsCommentAsPageNumber { get; set; }
        public static bool ShowPlanPageNameAsTooltip { get; set; }
        public static bool ShowPlanPageName { get; set; }
        
        public static int PlanPageNameInputTimeFormat { get; set; }
        
        public static string PlanPageNameFormat { get; set; }
        public static string PlanPageNameFormatSpecial { get; set; }
        public static int PlanPageNameFormatSpecialPressID { get; set; }
        
        public static string PlanPageNameDateFormat { get; set; }

        public static double InkUsagePer1000Copies { get; set; }
        public static double InkUsagePlateImageSize { get; set; }
        
        public static bool PlanningSetPageNameAsPlanPageName { get; set; }

        public static bool AllowSetLanguage { get; set; }
        static Globals()
		{
            PlanningSetPageNameAsPlanPageName = ReadConfigBoolean("PlanningSetPageNameAsPlanPageName", true);

            LogoWidth = ReadConfigInt32("LogoWidth", 150);
            AllowUpload = ReadConfigBoolean("AllowUpload", false);
            HideDownload = ReadConfigBoolean("HideDownload", false);
            AllowPageLock = ReadConfigBoolean("AllowPageLock", false);
            AllowReproof = ReadConfigBoolean("AllowReproof", true);
            AllowColorChange = ReadConfigBoolean("AllowColorChange", true);

            AllowPartialProofs = ReadConfigBoolean("AllowPartialProofs", false);  

            MessageSystem = ReadConfigBoolean("MessageSystem", false);
            ShowApproveAllButton = ReadConfigBoolean("ShowApproveAllButton", false);
            ShowCustomAction = ReadConfigBoolean("ShowCustomAction", false);
            ShowReadyAction = ReadConfigBoolean("ShowReadyAction", false);
            ShowDeadline = ReadConfigBoolean("ShowDeadline", false);
            ShowPitstopReports = ReadConfigBoolean("ShowPitstopReports", false);
            ShowInksaveReports = ReadConfigBoolean("ShowInksaveReports", false);
            ShowBigLock = ReadConfigBoolean("ShowBigLock", false);

            ShowPdfIfExists = ReadConfigBoolean("ShowPdfIfExists", false);
            CommonPageIndication = ReadConfigBoolean("CommonPageIndication", true);

            UseInputTimeInThumbnailName = ReadConfigBoolean("UseInputTimeInThumbnailName", false);
            ThumbnailsCommentAsPageNumber = ReadConfigBoolean("ThumbnailsCommentAsPageNumber", false);
            ShowPlanPageNameAsTooltip = ReadConfigBoolean("ShowPlanPageNameAsTooltip", false);
            ShowPlanPageName = ReadConfigBoolean("ShowPlanPageName", false);
            PlanPageNameInputTimeFormat = ReadConfigInt32("PlanPageNameInputTimeFormat", 1);
            PlanPageNameDateFormat = ReadConfigString("PlanPageNameDateFormat", "DDMM");

            PlanPageNameFormat = ReadConfigString("PlanPageNameFormat", "%I");
            PlanPageNameFormatSpecial = ReadConfigString("PlanPageNameFormatSpecial", "%I");
            PlanPageNameFormatSpecialPressID = ReadConfigInt32("PlanPageNameFormatSpecialPressID", 0);

            UsePressGroups = ReadConfigBoolean("UsePressGroups", false);
            UseChannels = ReadConfigBoolean("UseChannels", false);
            LocationIsPress = ReadConfigBoolean("LocationIsPress", false);

            FlatLook = ReadConfigBoolean("FlatLook", false);
            SmallEventIcons = ReadConfigBoolean("SmallEventIcons", false);
            OldFileNames = ReadConfigBoolean("OldFileNames", false);
            UseVersionPreviews = ReadConfigBoolean("UseVersionPreviews", true);
            UseVersionThumbnails = ReadConfigBoolean("UseVersionThumbnails", false);

            ShowHistory = ReadConfigBoolean("ShowHistory", false);


            InkUsagePer1000Copies = ReadConfigDouble("InkUsagePer1000Copies", 1500.0);
            InkUsagePlateImageSize = ReadConfigDouble("InkUsagePlateImageSize", 0.2);

            AllowSetLanguage = ReadConfigBoolean("AllowSetLanguage", false);
            
        }

        public static bool IsValidLanguage(string language)
        {
            return (language == "en" || language == "da" || language == "no" || language == "de" || language == "fr" || language == "sv" || language == "ko" || language == "ch");
        }
        public static string GetCulture(string language)
        {

            if (language == "no")
                return "nb-NO";
            else if (language == "da")
                return "da-DK";
            else if (language == "fr")
                return "fr-FR";
            else if (language == "ko")
                return "ko-KR";
            else if (language == "de")
                return "de-DE";
            else if (language == "sv")
                return "sv-SE";
            else if (language == "cn")
                return "zh-cn";

            return "en-US";
        }

        public static string GetEncoding(string language)
        {
            if (language == "ko")
                return "ks_c_5601-1987";
            else if (language == "cn")
                return "gb23127";

            return "utf-8";
        }

        public static string DoubleToString(double f, int decimals)
        {
            if (decimals == 0)
            {
                int n = (int)Math.Round(f);
                return n.ToString();
            }
            if (decimals == 1)
                return string.Format("{0:0.0}", f);
            if (decimals == 2)
                return string.Format("{0:0.00}", f);

             return string.Format("{0:0.000}", f);

        }

        public static DateTime ParsePubDate(string pubDateString)
        {
            int year = 1975;
            int month = 1;
            int day = 1;
            if (pubDateString.Length == 10)
            {
                // 2014-12-24
                // 0    5  8
                if (pubDateString.Substring(4, 1) == "-" && pubDateString.Substring(7, 1) == "-")
                {
                    Int32.TryParse(pubDateString.Substring(0, 4), out year);
                    Int32.TryParse(pubDateString.Substring(5, 2), out month);
                    Int32.TryParse(pubDateString.Substring(8, 2), out day);
                }
                // 24-12-2014
                // 0  3  6  
                else if (pubDateString.Substring(2, 1) == "-" && pubDateString.Substring(5, 1) == "-")
                {
                    Int32.TryParse(pubDateString.Substring(0, 2), out day);
                    Int32.TryParse(pubDateString.Substring(3, 2), out month);
                    Int32.TryParse(pubDateString.Substring(6, 4), out year);
                }
            }

            return new DateTime(year, month, day);
        }


        public static bool ReadConfigBoolean(string setting, bool defaultValue)
		{
			bool ret = defaultValue;
            if (ConfigurationManager.AppSettings[setting] != null)
			{
				try 
				{
                    return Convert.ToInt32((string)ConfigurationManager.AppSettings[setting]) == 1;
				}
				catch {}
			}

			return ret;
		}



		public static int ReadConfigInt32(string setting, int defaultValue)
		{
			int ret = defaultValue;
            if (ConfigurationManager.AppSettings[setting] != null)
			{
				try 
				{
                    return Convert.ToInt32((string)ConfigurationManager.AppSettings[setting]);
				}
				catch {}
			}

			return ret;
		}

        public static double ReadConfigDouble(string setting, double defaultValue)
        {
            double ret = defaultValue;
            if (ConfigurationManager.AppSettings[setting] != null)
            {
                try
                {
                    return Convert.ToDouble((string)ConfigurationManager.AppSettings[setting]);
                }
                catch { }
            }

            return ret;
        }

        public static string ReadConfigString(string setting, string defaultValue)
		{
			string ret = defaultValue;
            if (ConfigurationManager.AppSettings[setting] != null)
			{
				try 
				{
                    return (string)ConfigurationManager.AppSettings[setting];
				}
				catch {}
			}

			return ret;
		}

		public static int TryParse(string s, int defaultValue)
		{
            if (s == "")
                return defaultValue;

            int n = defaultValue;
			try
			{
				n = Int32.Parse(s);
			}
			catch
			{}

			return n;
		}

        public static double TryParseDouble(string s, double defaultValue)
        {
            if (s == "")
                return defaultValue;

            double f = defaultValue;
            try
            {
                f = Double.Parse(s);
            }
            catch
            { }

            return f;
        }

        public static int TryParseCookie(HttpRequest request, string s, int defaultValue)
        {
            int n = defaultValue;

            if (request == null)
                return defaultValue;

            if (request.Cookies[s] == null)
                return defaultValue;
            try
            {
                n = Int32.Parse(request.Cookies[s].Value);
            }
            catch
            { }

            return n;
        }

        public static List<string> GetFileList(string folder, string extension, bool removeExtension, out string errmsg)
        {
            errmsg = "";
            List<string> files = new List<string>();
            try
            {
                DirectoryInfo dirInfo = new System.IO.DirectoryInfo(folder);
                {
                    if (dirInfo.Exists == false)
                    {
                        errmsg = "Directory not found";
                        return files;
                    }

                    foreach (FileInfo f in dirInfo.GetFiles())
                    {
                        FileAttributes attributes = File.GetAttributes(f.FullName);
                        if ((attributes & FileAttributes.Directory) == FileAttributes.Directory ||
                                (attributes & FileAttributes.Hidden) == FileAttributes.Hidden ||
                                (attributes & FileAttributes.System) == FileAttributes.System ||
                                f.Name[0] == '.' || f.Length == 0)
                            continue;
                        if (f.Extension == "" || string.Equals(f.Extension, extension, StringComparison.CurrentCultureIgnoreCase))
                        {
                            files.Add(removeExtension && f.Name.LastIndexOf('.') > 0 ? f.Name.Substring(0, f.Name.LastIndexOf('.')) : f.Name);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                errmsg = "Error getting template file list -" + ex.Message;
            }

            return files;
        }
        public static string GenerateTimeStamp()
        {
            DateTime dt = DateTime.Now;
            return string.Format("{0:0000}{1:00}{2:00}{3:00}{4:00}{5:00}{6:00}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
        }

        public static int MoveAllFiles(string sourceDir, string destDir, out string errmsg)
        {
            errmsg = "";
            List<string> files = new List<string>();
            try
            {
                DirectoryInfo dirInfo = new System.IO.DirectoryInfo(sourceDir);
                {
                    if (dirInfo.Exists == false)
                    {
                       errmsg = "Source directory does not exist";
                       return -1;
                    }

                    foreach (FileInfo f in dirInfo.GetFiles())
                    {
                        FileAttributes attributes = File.GetAttributes(f.FullName);
                        if ((attributes & FileAttributes.Directory) == FileAttributes.Directory ||
                                (attributes & FileAttributes.Hidden) == FileAttributes.Hidden ||
                                (attributes & FileAttributes.System) == FileAttributes.System ||
                                f.Name[0] == '.' || f.Length == 0)
                            continue;

                        files.Add(f.Name);                      
                    }

                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return -1;
            }

            foreach (string f in files)
            {
                try
                {
                    File.Move(sourceDir + @"\" + f, destDir + @"\" + f);
                }
                catch (Exception ex)
                {
                    errmsg = ex.Message;
                    return -1;
                }
            }

            return files.Count;

        }

        public static int GetCacheRowCount(string nameCache)
		{
			DataTable dt = (DataTable) HttpContext.Current.Cache[nameCache];
			if (dt != null && dt.HasErrors == false)
				return dt.Rows.Count;
			else
				return 0;
		}


		public static string LookupInputAlias(string aliasType, string longName)
		{
			DataTable dt = (DataTable) HttpContext.Current.Cache["InputAliasCache"];

			if (dt != null && dt.HasErrors == false)
			{

				foreach (DataRow row in dt.Rows)
				{
					if ((string)row["Type"] == aliasType && (string)row["LongName"] == longName)
						return (string)row["ShortName"];
				}
			} 
			else
				return longName;

			return longName;
		}

        public static string LookupInputAliasShortToLong(string aliasType, string shortName)
        {
            DataTable dt = (DataTable)HttpContext.Current.Cache["InputAliasCache"];

            if (dt != null && dt.HasErrors == false)
            {

                foreach (DataRow row in dt.Rows)
                {
                    if ((string)row["Type"] == aliasType && (string)row["ShortName"] == shortName)
                        return (string)row["LongName"];
                }
            }
            else
                return shortName;

            return shortName;
        }
        public static string LookupInkAlias(string aliasType, string longName, int pressID)
        {
            DataTable dt = (DataTable)HttpContext.Current.Cache["InkAliasCache"];

            if (dt != null && dt.HasErrors == false)
            {

                foreach (DataRow row in dt.Rows)
                {
                    if ((string)row["Type"] == aliasType && (string)row["LongName"] == longName && (int)row["PressID"] == pressID)
                        return (string)row["ShortName"];
                }
            }
            else
                return longName;

            return longName;
        }


		public static int LookupDefaultTemplateFromPageFormat(int nPageFormatID, int nPressID, bool oneUpPreferred)
		{
			DataTable dt = (DataTable) HttpContext.Current.Cache["TemplatePageFormatCache"];

			if (dt != null && dt.HasErrors == false)
			{

				if (oneUpPreferred)
				{
					foreach (DataRow row in dt.Rows)
					{
						if ((int)row["PageFormatID"] == nPageFormatID && (int)row["PressID"] == nPressID)
						{
							int nTemplateID = (int)row["TemplateID"];
							int nUP = GetPagesOnPlateFromTemplate(nTemplateID);
							if (nUP == 1)
								return nTemplateID;
						}
					}
				}
				foreach (DataRow row in dt.Rows)
				{
					if ((int)row["PageFormatID"] == nPageFormatID && (int)row["PressID"] == nPressID)
						return (int)row["TemplateID"];
				}
			} 
			

			return 0;
		}

        /*
		public static void GetPressPlanSectionSizes(string sPlanName, out string sections, out string pages)
		{	
			sections = "";
			pages = "";
			
			DataTable dt = (DataTable) HttpContext.Current.Cache["PlanNameCache"];
			if (dt != null && dt.HasErrors == false)
			{

				foreach (DataRow row in dt.Rows)
				{
					if ((string)row["Name"] == sPlanName)
					{
						sections = (string)row["Sections"];
						pages = (string)row["Pages"];
						return;
					}
				}
			}
		}
        */
        public static string GetNameFromID(string nameCache, int nID)
        {
            DataTable dt = (DataTable)HttpContext.Current.Cache[nameCache];
            if (dt != null && dt.HasErrors == false)
            {

                foreach (DataRow row in dt.Rows)
                {
                    if ((int)row["ID"] == nID)
                        return (string)row["Name"];
                }

                if (nameCache == "PublicationNameCache")
                {
                    Global.RefreshPublicationNameCache(null, null, 0);
                    Global.RefreshPublicationEditionsCache(null, null, 0);
                    Global.RefreshPublicationSectionsCache(null, null, 0);
                }
                else if (nameCache == "EditionNameCache")
                    Global.RefreshEditionNameCache(null, null, 0);
                else if (nameCache == "IssueNameCache")
                    Global.RefreshIssueNameCache(null, null, 0);
                else if (nameCache == "SectionNameCache")
                    Global.RefreshSectionNameCache(null, null, 0);
                else if (nameCache == "PressNameCache")
                    Global.RefreshPressNameCache(null, null, 0);
                else if (nameCache == "DeviceNameCache")
                    Global.RefreshDeviceNameCache(null, null, 0);
                else if (nameCache == "TemplateNameCache")
                    Global.RefreshTemplateNameCache(null, null, 0);
                else if (nameCache == "ProofNameCache")
                    Global.RefreshProofNameCache(null, null, 0);
                else if (nameCache == "PageFormatCache")
                    Global.RefreshPageFormatCache(null, null, 0);
                else if (nameCache == "TemplatePageFormatCache")
                    Global.RefreshTemplatePageFormatCache(null, null, 0);
                else if (nameCache == "LocationNameCache")
                    Global.RefreshLocationNameCache(null, null, 0);
                else if (nameCache == "PublicationPageFormatCache")
                    Global.RefreshPublicationPageFormatCache(null, null, 0);

                else if (nameCache == "HardProofNameCache")
                    Global.RefreshHardProofNameCache(null, null, 0);
                
                else if (nameCache == "PublicationProofCache")
                    Global.RefreshPublicationProofCache(null, null, 0);

                else if (nameCache == "PressGroupNameCache")
                    Global.RefreshPressGroupNameCache(null, null, 0);

                else if (nameCache == "ChannelNameCache" && (bool)HttpContext.Current.Application["UseChannels"])
                    Global.RefreshChannelNameCache(null, null, 0);

                else if (nameCache == "PublisherNameCache" && (bool)HttpContext.Current.Application["UseChannels"])
                    Global.RefreshPublisherNameCache(null, null, 0);  

                if (nameCache == "PublicationNameCache" || nameCache == "EditionNameCache" || nameCache == "SectionNameCache")
                {
                    Global.RefreshInputAliasCache(null, null, 0);
                    Global.RefreshInkAliasCache(null, null, 0);
                }

                dt = (DataTable)HttpContext.Current.Cache[nameCache];
                if (dt != null && dt.HasErrors == false)
                {
                    foreach (DataRow row in dt.Rows)  // retry..
                    {
                        if ((int)row["ID"] == nID)
                            return (string)row["Name"];
                    }
                }

            }
            return "";
        }

        public static void ForcePPMCacheReload()
        {
            Global.RefreshPPMPublicationNamesCache(null, null, 0);
            Global.RefreshPPMPaperNamesCache(null, null, 0);
            Global.RefreshPPMEditionNamesCache(null, null, 0);
            Global.RefreshPPMPageFormatNamesCache(null, null, 0);
            Global.RefreshPPMPressNamesCache(null, null, 0);

        }


        public static void ForceCacheReloadsSmall()
		{
			Global.RefreshEditionNameCache ( null, null, 0);
			Global.RefreshSectionNameCache ( null, null, 0);
			Global.RefreshPublicationNameCache ( null, null, 0);
			Global.RefreshPublicationEditionsCache(null, null, 0);
			Global.RefreshPublicationSectionsCache(null, null, 0);
			Global.RefreshInputAliasCache(null, null, 0);
            Global.RefreshProofNameCache(null, null, 0);
			Global.RefreshPageFormatCache(null, null, 0);
		}

		public static void ForceCacheReloads()
		{
			Global.RefreshEditionNameCache ( null, null, 0);
			Global.RefreshColorNameCache ( null, null, 0);
			Global.RefreshSectionNameCache ( null, null, 0);
			Global.RefreshStatusNameCache ( null, null, 0);
			Global.RefreshExtStatusNameCache ( null, null, 0);
			Global.RefreshEventNameCache(null, null, 0);

			Global.RefreshPublicationNameCache ( null, null, 0);
			Global.RefreshIssueNameCache ( null, null, 0);
			Global.RefreshPressNameCache ( null, null, 0);
			Global.RefreshLocationNameCache ( null, null, 0);
			Global.RefreshDeviceNameCache ( null, null, 0);
			Global.RefreshTemplateNameCache ( null, null, 0);
			Global.RefreshTemplatePressNameCache( null, null, 0);
			Global.RefreshProofNameCache( null, null, 0);
			Global.RefreshPageFormatCache(null, null, 0);
			Global.RefreshTemplatePageFormatCache(null, null, 0);
			Global.RefreshLocationNameCache(null,null,0);
			Global.RefreshHardProofNameCache(null, null, 0);
			Global.RefreshPublicationEditionsCache(null, null, 0);
			Global.RefreshPublicationSectionsCache(null, null, 0);
			if (Global.databaseVersion < 2)
			{
				Global.RefreshPublicationPageFormatCache(null, null, 0);
				Global.RefreshPublicationProofCache(null, null, 0);
			}
			Global.RefreshInputAliasCache(null, null, 0);
            Global.RefreshInkAliasCache(null, null, 0);
            Global.RefreshPublicationNamingConvensionCache(null, null, 0);
            Global.RefreshPressGroupNameCache(null, null, 0);

            Global.RefreshRipSetupNameCache(null, null, 0);
            Global.RefreshPreflightSetupNameCache(null, null, 0);
            Global.RefreshInksaveSetupNameCache(null, null, 0);
            if ((bool)HttpContext.Current.Application["UseChannels"])
            {
                Global.RefreshChannelNameCache(null, null, 0);
                Global.RefreshPublisherNameCache(null, null, 0);
            }
			
		}

        public static bool HasAllowedEditions(int publicationID)
        {
            if (publicationID == 0)
                return false;

            DataTable dt = (DataTable)HttpContext.Current.Cache["PublicationEditionsCache"];
            if (dt == null)
                return false;
            if (dt.HasErrors)
                return false;

            foreach (DataRow row in dt.Rows)
            {
                if ((int)row["PublicationID"] == publicationID)
                {
                    return true;
                }
            }

            return false;
        }

		public static bool IsAllowedEdition(int publicationID, int editionID)
		{
			DataTable dt = (DataTable) HttpContext.Current.Cache["PublicationEditionsCache"];
			if (dt == null)
				return true;
			if (dt.HasErrors)
				return true;

			bool hasEntries = false;

			foreach (DataRow row in dt.Rows)
			{
				if ((int)row["PublicationID"] == publicationID)
				{
					hasEntries = true;
					break;
				}
			}
			if (hasEntries == false)
				return false;

			// We have editionID entries to filter with
			bool allowed = false;
			foreach (DataRow row in dt.Rows)
			{
				if ((int)row["PublicationID"] == publicationID && (int)row["EditionID"] == editionID)
				{
					allowed = true;
					break;
				}
			}

			return allowed;
		}

        public static bool HasAllowedSections(int publicationID)
        {
            if (publicationID == 0)
                return false;

            DataTable dt = (DataTable)HttpContext.Current.Cache["PublicationSectionsCache"];
            if (dt == null)
                return false;
            if (dt.HasErrors)
                return false;

            foreach (DataRow row in dt.Rows)
            {
                if ((int)row["PublicationID"] == publicationID)
                {
                    return true;
                }
            }

            return false;
        }

		public static bool IsAllowedSection(int publicationID, int sectionID)
		{
			DataTable dt = (DataTable) HttpContext.Current.Cache["PublicationSectionsCache"];
			if (dt == null)
				return true;
			if (dt.HasErrors)
				return true;

			bool hasEntries = false;

			foreach (DataRow row in dt.Rows)
			{
				if ((int)row["PublicationID"] == publicationID)
				{
					hasEntries = true;
					break;
				}
			}
			if (hasEntries == false)
				return false;

			// We have editionID entries to filter with
			bool allowed = false;
			foreach (DataRow row in dt.Rows)
			{
				if ((int)row["PublicationID"] == publicationID && (int)row["SectionID"] == sectionID)
				{
					allowed = true;
					break;
				}
			}

			return allowed;
		}

        public static ArrayList GetPublicationTemplates(string publication)
        {
            return GetPublicationTemplates(GetIDFromName("PublicationNameCache", publication));
        }

        public static ArrayList GetPublicationTemplates(int publicationID)
        {
            ArrayList alist = new ArrayList();

            DataTable dt = (DataTable)HttpContext.Current.Cache["PublicationTemplateCache"];
            if (dt == null)
                return alist;
            if (dt.HasErrors)
                return alist;

            foreach (DataRow row in dt.Rows)
            {
                if ((int)row["PublicationID"] == publicationID)
                    alist.Add((int)row["TemplateID"]);
            }
            return alist;
        }

        public static string GetFirstPressGroupForPress(string pressName)
        {
            int pressID = GetIDFromName("PressNameCache", pressName);
            if (pressID == 0)
                return "";

            ArrayList pressGroupNames = GetArrayFromCache("PressGroupNameCache");
            foreach (string pressGroup in pressGroupNames)
            {
                ArrayList pressesInPressGroup = GetPressIDListFromPressGroup(GetIDFromName("PressGroupNameCache", pressGroup));
                foreach (int pressIDInGroup in pressesInPressGroup)
                {
                    if (pressID == pressIDInGroup)
                        return pressGroup;
                }
            }

            return "";

        }

        public static int GetFirstPressIDFromPPressGroup(int pressGroupID)
        {
            ArrayList al = GetPressIDListFromPressGroup(pressGroupID);
            if (al.Count == 0)
                return 0;

            return (int)al[0];
        }

        public static bool IsPressInGroup(int pressID, int pressGroupID)
        {
            ArrayList al = GetPressIDListFromPressGroup(pressGroupID);
            foreach (int id in al)
            {
                if (id == pressID)
                    return true;
            }

            return false;
        }


        public static ArrayList GetPressIDListFromPressGroup(int pressGroupID)
        {
            ArrayList alist = new ArrayList();

            DataTable dt = (DataTable)HttpContext.Current.Cache["PressGroupNameCache"];
            if (dt == null)
                return alist;
            if (dt.HasErrors)
                return alist;

            foreach (DataRow row in dt.Rows)
            {
                if ((int)row["ID"] == pressGroupID)
                {
                    string slist = (string)row["PressIDList"];

                    string[] ss = slist.Split(',');
                    foreach (string s in ss)
                    {
                        int n = TryParse(s, 0);
                        if (n > 0)
                            alist.Add(n);
                    }
                    break;
                }
            }
            return alist;
        }
        
        public static int GetPublicationTemplate(int publicationID, int pressID)
        {
            DataTable dt = (DataTable)HttpContext.Current.Cache["PublicationTemplateCache"];
            if (dt == null)
                return 0;
            if (dt.HasErrors)
                return 0;
            foreach (DataRow row in dt.Rows)
            {
                if ((int)row["PublicationID"] == publicationID && ((int)row["PressID"] == pressID || pressID == 0))
                    return (int)row["TemplateID"];
            }

            return 0;
        }

        public static int GetPublicationTemplateFileType(int publicationID, int pressID)
        {
            DataTable dt = (DataTable)HttpContext.Current.Cache["PublicationTemplateCache"];
            if (dt == null)
                return 0;
            if (dt.HasErrors)
                return 0;
            foreach (DataRow row in dt.Rows)
            {
                if ((int)row["PublicationID"] == publicationID && ((int)row["PressID"] == pressID || pressID == 0))
                {
                    int n = (int)row["SeparateRuns"];
                    return ((n & 0x02) > 0) ? 1 : 0;
                }
            }

            return 0;
        }

        public static int GetPublicationPageformatID(int publicationID)
        {
            DataRow row = GetPublicationRow(publicationID);
            if (row == null)
                return 0;
            return (int)row["PageFormatID"];
        }

        public static int GetPublicationCustomerID(int publicationID)
        {
            DataRow row = GetPublicationRow(publicationID);
            if (row == null)
                return 0;
            return (int)row["CustomerID"];
        }

        public static int GetPublicationProofID(int publicationID)
        {
            DataRow row = GetPublicationRow(publicationID);
            if (row == null)
                return 0;
            return (int)row["DefaultProofID"];
        }

        public static int GetPublicationApprovalRequired(int publicationID)
        {
            DataRow row = GetPublicationRow(publicationID);
            if (row == null)
                return -1;
            return (int)row["DefaultApprove"];
        }
        
        public static ArrayList GetArrayFromCache(string nameCache)
		{
			ArrayList al = new ArrayList();
			DataTable dt = (DataTable) HttpContext.Current.Cache[nameCache];
			if (dt != null && dt.HasErrors == false)
			{
				foreach (DataRow row in dt.Rows)
				{
					al.Add(row["Name"]);
				}
			}
			return al;
		}

		public static int GetTypeFromID(string nameCache, int nID)
		{
			DataTable dt = (DataTable) HttpContext.Current.Cache[nameCache];
			if (dt != null && dt.HasErrors == false)
			{
				foreach (DataRow row in dt.Rows)
				{
					if ((int)row["ID"] == nID)
						return (int)row["Type"];
				}
			}
			return 0;
		}

		public static DataRow GetPublicationRow(string name)
		{
			DataTable dt = (DataTable) HttpContext.Current.Cache["PublicationNameCache"];
			if (dt != null && dt.HasErrors == false)
			{
				foreach (DataRow row in dt.Rows)
				{
					if ((string)row["Name"] == name)
						return row;
				}
			}
			return null;
		}

		public static DataRow GetPublicationRow(int ID)
		{
			DataTable dt = (DataTable) HttpContext.Current.Cache["PublicationNameCache"];
			if (dt != null && dt.HasErrors == false)
			{
				foreach (DataRow row in dt.Rows)
				{
					if ((int)row["ID"] == ID)
						return row;
				}
			}
			return null;
		}

        public static int GetIDFromName(string nameCache, string name)
        {
            if (name == "")
                return 0;

            DataTable dt = (DataTable)HttpContext.Current.Cache[nameCache];
            if (dt != null && dt.HasErrors == false)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if ((string)row["Name"] == name)
                        return (int)row["ID"];
                }
            }

            if (nameCache == "PublicationNameCache")
            {
                Global.RefreshPublicationNameCache(null, null, 0);
                Global.RefreshSectionNameCache(null, null, 0);
                Global.RefreshEditionNameCache(null, null, 0);
                Global.RefreshPublicationSectionsCache(null, null, 0);
                Global.RefreshPublicationEditionsCache(null, null, 0);
            }
            else if (nameCache == "EditionNameCache")
            {
                Global.RefreshEditionNameCache(null, null, 0);
                Global.RefreshPublicationEditionsCache(null, null, 0);
            }
            else if (nameCache == "IssueNameCache")
                Global.RefreshIssueNameCache(null, null, 0);
            else if (nameCache == "SectionNameCache")
            {
                Global.RefreshSectionNameCache(null, null, 0);
                Global.RefreshPublicationSectionsCache(null, null, 0);
            }
            else if (nameCache == "PressNameCache")
                Global.RefreshPressNameCache(null, null, 0);
            else if (nameCache == "DeviceNameCache")
                Global.RefreshDeviceNameCache(null, null, 0);
            else if (nameCache == "TemplateNameCache")
                Global.RefreshTemplateNameCache(null, null, 0);
            else if (nameCache == "ProofNameCache")
                Global.RefreshProofNameCache(null, null, 0);
            else if (nameCache == "PageFormatCache")
                Global.RefreshPageFormatCache(null, null, 0);
            else if (nameCache == "LocationNameCache")
                Global.RefreshLocationNameCache(null, null, 0);

            dt = (DataTable)HttpContext.Current.Cache[nameCache];
            if (dt != null && dt.HasErrors == false)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if ((string)row["Name"] == name)
                        return (int)row["ID"];
                }
            }

            return 0;
        }

		public static int GetTypeFromName(string nameCache, string name)
		{
			DataTable dt = (DataTable) HttpContext.Current.Cache[nameCache];
			if (dt != null && dt.HasErrors == false)
			{
				foreach (DataRow row in dt.Rows)
				{
					if ((string)row["Name"] == name)
						return (int)row["Type"];
				}
			}
			return 0;
		}

		public static int GetPagesOnPlateFromTemplate(int templateID)
		{
			DataTable dt = (DataTable) HttpContext.Current.Cache["TemplatePressCache"];
			if (dt != null && dt.HasErrors == false)
			{
				foreach (DataRow row in dt.Rows)
				{
					if ((int)row["ID"] == templateID)
						return (int)row["Nup"];
				}
			}
			return 1;
		}

		public static int GetDummyTemplateID(string pressName)
		{
			DataTable dt = (DataTable) HttpContext.Current.Cache["TemplatePressCache"];
			if (dt != null && dt.HasErrors == false)
			{
				foreach (DataRow row in dt.Rows)
				{
					if ((string)row["Press"] == pressName && (bool)row["IsDummy"] == true)
						return (int)row["ID"];
				}
				// No dummy template defined - return a 1-up template if it exists
				foreach (DataRow row in dt.Rows)
				{
					if ((string)row["Press"] == pressName && (int)row["Nup"] == 1)
						return (int)row["ID"];
				}

			}

			return 0;
		}

		public static string GetPressFromTemplate(int templateID)
		{
			DataTable dt = (DataTable) HttpContext.Current.Cache["TemplatePressCache"];
			if (dt != null && dt.HasErrors == false)
			{
				foreach (DataRow row in dt.Rows)
				{
					if ((int)row["ID"] == templateID)
						return (string)row["Press"];
				}
			}
			return "";
		}

		public static string GetLocationFromPress(int pressID)
		{
			DataTable dt = (DataTable) HttpContext.Current.Cache["PressCache"];
			if (dt != null && dt.HasErrors == false)
			{
				foreach (DataRow row in dt.Rows)
				{
					if ((int)row["ID"] == pressID)
						return (string)row["Location"];
				}
			}
			return "";
		}

		public static string GetStatusName(int nStatusNumber, int nStatusType)
		{
			DataTable dt = nStatusType == 0 ? (DataTable) HttpContext.Current.Cache["StatusNameCache"] : (DataTable) HttpContext.Current.Cache["ExtStatusNameCache"];
			if (dt != null && dt.HasErrors == false)
			{
				foreach (DataRow row in dt.Rows)
				{
					if ((int)row["StatusNumber"] == nStatusNumber)
						return (string)row["StatusName"];
				}
			}
			return "";
		}

		public static int GetStatusID(string sStatusName, int nStatusType)
		{
			DataTable dt = nStatusType == 0 ? (DataTable) HttpContext.Current.Cache["StatusNameCache"] : (DataTable) HttpContext.Current.Cache["ExtStatusNameCache"];
			if (dt != null && dt.HasErrors == false)
			{
				foreach (DataRow row in dt.Rows)
				{
					if ((string)row["StatusName"] == sStatusName)
						return (int)row["StatusNumber"];
				}
			}
			return 0;
		}

		public static System.Drawing.Color GetStatusColorFromName(string sStatusName, int nStatusType)
		{
			DataTable dt = nStatusType == 0 ? (DataTable) HttpContext.Current.Cache["StatusNameCache"] : (DataTable) HttpContext.Current.Cache["ExtStatusNameCache"];
			if (dt != null && dt.HasErrors == false)
			{
				foreach (DataRow row in dt.Rows)
				{
					if ((string)row["StatusName"] == sStatusName)
					{
						string s = (string)row["StatusColor"];		
						if (s.Length == 6)
							return System.Drawing.Color.FromArgb(
								Int32.Parse(s.Substring(0,2), System.Globalization.NumberStyles.HexNumber),
								Int32.Parse(s.Substring(2,2), System.Globalization.NumberStyles.HexNumber),
								Int32.Parse(s.Substring(4,2), System.Globalization.NumberStyles.HexNumber));
						else
							return System.Drawing.Color.White;
					}
				}
			}
			return System.Drawing.Color.White;
		}

		public static System.Drawing.Color GetStatusColor(int nStatusNumber, int nStatusType)
		{
			DataTable dt = nStatusType == 0 ? (DataTable) HttpContext.Current.Cache["StatusNameCache"] : (DataTable) HttpContext.Current.Cache["ExtStatusNameCache"];
			if (dt != null && dt.HasErrors == false)
			{
				foreach (DataRow row in dt.Rows)
				{
					if ((int)row["StatusNumber"] == nStatusNumber)
					{
						string s = (string)row["StatusColor"];		
						if (s.Length == 6)
							return System.Drawing.Color.FromArgb(
								Int32.Parse(s.Substring(0,2), System.Globalization.NumberStyles.HexNumber),
								Int32.Parse(s.Substring(2,2), System.Globalization.NumberStyles.HexNumber),
								Int32.Parse(s.Substring(4,2), System.Globalization.NumberStyles.HexNumber));
						else
							return System.Drawing.Color.White;
					}
				}
			}
			return System.Drawing.Color.White;
		}

		public static System.Drawing.Color GetColorFromID(int nID)
		{
			string sColorName = GetNameFromID("ColorNameCache", nID);
			if (sColorName == "")
				return System.Drawing.Color.White;
			return GetColorFromName(sColorName);
		}

		public static System.Drawing.Color GetColorFromName(string sColorName)
		{
			DataTable dt = (DataTable) HttpContext.Current.Cache["ColorNameCache"];
			if (dt != null && dt.HasErrors == false)
			{
				foreach (DataRow row in dt.Rows)
				{
					if ((string)row["Name"] == sColorName)
					{
						/*sColorName = sColorName.ToUpper();
						// Is this a known color?
						

						if (sColorName == "C" || sColorName == "CYAN" || sColorName == "C2")
							return System.Drawing.Color.Cyan;
						if (sColorName == "M" || sColorName == "MAGENTA" || sColorName == "M2")
							return System.Drawing.Color.Magenta;
						if (sColorName == "Y" || sColorName == "YELLOW" || sColorName == "Y2")
							return System.Drawing.Color.Yellow;
						if (sColorName == "K" || sColorName == "BLACK" || sColorName == "K2")
							return System.Drawing.Color.Black;
						*/
						// Unknown - find in spot table. Spot c,m,y,k percent values are coded to string "CCMMYYKK"
						int cy =255*100 - 255* (int)row["C"];
						int m = 255*100 - 255* (int)row["M"];
						int y = 255*100 - 255* (int)row["Y"];
						int k = 255*100 - 255* (int)row["K"];
						int r = cy*k/100;
						int gr = m*k/100;
						int b = y*k/100;
						return System.Drawing.Color.FromArgb(r/(255*100), gr/(255*100), b/(255*100));
					}			
				}
			}
			return System.Drawing.Color.White;
		}

		public static bool IsProcessColor(string sColorName)
		{
			DataTable dt = (DataTable) HttpContext.Current.Cache["ColorNameCache"];
			if (dt != null && dt.HasErrors == false)
			{

				foreach (DataRow row in dt.Rows)
				{
					if ((string)row["Name"] == sColorName)
					{
						sColorName = sColorName.ToUpper();
						// Is this a known color?
						if ((int)row["C"] == 100 && (int)row["M"] == 0 && (int)row["Y"] == 0 && (int)row["K"] == 0)
							return true;
						if ((int)row["C"] == 0 && (int)row["M"] == 100 && (int)row["Y"] == 0 && (int)row["K"] == 0)
							return true;
						if ((int)row["C"] == 0 && (int)row["M"] == 0 && (int)row["Y"] == 100 && (int)row["K"] == 0)
							return true;
						if ((int)row["C"] == 0 && (int)row["M"] == 0 && (int)row["Y"] == 0 && (int)row["K"] == 100)
							return true;
/*						if (sColorName == "C" || sColorName == "CYAN")
							return true;
						if (sColorName == "M" || sColorName == "MAGENTA")
							return true;
						if (sColorName == "Y" || sColorName == "YELLOW")
							return true;
						if (sColorName == "K" || sColorName == "BLACK")
							return true;*/
					}			
				}
			}
			return false;


		}


		public static bool IsBlackColor(string sColorName)
		{
			DataTable dt = (DataTable) HttpContext.Current.Cache["ColorNameCache"];
			if (dt != null && dt.HasErrors == false)
			{

				foreach (DataRow row in dt.Rows)
				{
					if ((string)row["Name"] == sColorName)
					{
						if ((int)row["C"] == 0 && (int)row["M"] == 0 && (int)row["Y"] == 0 && (int)row["K"] == 100)
							return true;
					}			
				}
			}
			return false;
		}


		public static bool IsCyanColor(string sColorName)
		{
			DataTable dt = (DataTable) HttpContext.Current.Cache["ColorNameCache"];
			if (dt != null && dt.HasErrors == false)
			{

				foreach (DataRow row in dt.Rows)
				{
					if ((string)row["Name"] == sColorName)
					{
						if ((int)row["C"] == 100 && (int)row["M"] == 0 && (int)row["Y"] == 0 && (int)row["K"] == 0)
							return true;
					}			
				}
			}
			return false;
		}

		public static bool IsMagentaColor(string sColorName)
		{
			DataTable dt = (DataTable) HttpContext.Current.Cache["ColorNameCache"];
			if (dt != null && dt.HasErrors == false)
			{

				foreach (DataRow row in dt.Rows)
				{
					if ((string)row["Name"] == sColorName)
					{
						if ((int)row["C"] == 0 && (int)row["M"] == 100 && (int)row["Y"] == 0 && (int)row["K"] == 0)
							return true;
					}			
				}
			}
			return false;
		}


		public static bool IsYellowColor(string sColorName)
		{
			DataTable dt = (DataTable) HttpContext.Current.Cache["ColorNameCache"];
			if (dt != null && dt.HasErrors == false)
			{

				foreach (DataRow row in dt.Rows)
				{
					if ((string)row["Name"] == sColorName)
					{
						if ((int)row["C"] == 0 && (int)row["M"] == 0 && (int)row["Y"] == 100 && (int)row["K"] == 0)
							return true;
					}			
				}
			}
			return false;
		}



		public static bool IsProcessColor(int nColorID)
		{
			DataTable dt = (DataTable) HttpContext.Current.Cache["ColorNameCache"];
			if (dt != null && dt.HasErrors == false)
			{
				foreach (DataRow row in dt.Rows)
				{
					if ((int)row["ID"] == nColorID)
					{
						string sColorName = (string)row["Name"];
						sColorName = sColorName.ToUpper();
						// Is this a known color?
						/* if (sColorName == "C" || sColorName == "CYAN" || sColorName == "C2")
							return true;
						if (sColorName == "M" || sColorName == "MAGENTA" || sColorName == "M2")
							return true;
						if (sColorName == "Y" || sColorName == "YELLOW" || sColorName == "Y2")
							return true;
						if (sColorName == "K" || sColorName == "BLACK" || sColorName == "K2")
							return true; */

						if ((int)row["C"] == 100 && (int)row["M"] == 0 && (int)row["Y"] == 0 && (int)row["K"] == 0)
							return true;
						if ((int)row["C"] == 0 && (int)row["M"] == 100 && (int)row["Y"] == 0 && (int)row["K"] == 0)
							return true;
						if ((int)row["C"] == 0 && (int)row["M"] == 0 && (int)row["Y"] == 100 && (int)row["K"] == 0)
							return true;
						if ((int)row["C"] == 0 && (int)row["M"] == 0 && (int)row["Y"] == 0 && (int)row["K"] == 100)
							return true;
					}			
				}
			}
			return false;
		}


        public static PPMPublication LookupPPMPublication(string publicationName)
        {
            List<PPMPublication> publications = (List<PPMPublication>)HttpContext.Current.Cache["PPMPublicationNamesCache"];

            if (publications != null)
            {
                foreach (PPMPublication publication in publications)
                {
                    if (string.Equals(publication.Long_name, publicationName, StringComparison.InvariantCultureIgnoreCase) || 
                        string.Equals(publication.Short_name, publicationName, StringComparison.InvariantCultureIgnoreCase))
                        return publication;
                }
            }
            return null;
        }

        public static PPMPageFormat LookupPPMPageFormat(string pageFormatName)
        {
            List<PPMPageFormat> pageFormats = (List<PPMPageFormat>)HttpContext.Current.Cache["PPMPageFormatNamesCache"];

            if (pageFormats != null)
            {
                foreach (PPMPageFormat pageFormat in pageFormats)
                {
                    if (string.Equals(pageFormatName, pageFormat.Name, StringComparison.InvariantCultureIgnoreCase))
                        return pageFormat;
                }
            }
            return null;
        }

        public static string DateTime2String(DateTime t)
		{
			return string.Format("{0:0000}{1:00}{2:00}T{3:00}{4:00}{5:00}", t.Year,t.Month,t.Day,t.Hour,t.Minute,t.Second);
		}

        public static string DateTime2TimeStamp(DateTime t)
        {
            return string.Format("{0:0000}{1:00}{2:00}{3:00}{4:00}{5:00}", t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);
        }

        public static string DateTime2StringShort(DateTime t)
        {
            return string.Format("{0:00}-{1:00} {2:00}:{3:00}:{4:00}",  t.Day,t.Month,t.Hour, t.Minute, t.Second);
        }

        public static DateTime FirstDateOfWeek(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);

            int daysOffset = (int)CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;

            DateTime firstMonday = jan1.AddDays(daysOffset);

            int firstWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(jan1, CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule, CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);

            if (firstWeek <= 1)
            {
                weekOfYear -= 1;
            }

            return firstMonday.AddDays(weekOfYear * 7);
        }


		public static string RemoveExtension(string sInput)
		{
			string s = sInput;

			int m = s.LastIndexOf('.');
			if (m != -1)
				s = s.Substring(0,m);
			return s;
		}



		public static DateTime String2DateTime(string s)
		{

			DateTime t = new DateTime(1975,1,1,0,0,0);

			s = s.Replace("T","");
			s = s.Replace("F","");

			if (s.Length != 14)
				return t;

			int year = Globals.TryParse(s.Substring(0,4),0);
			int month = Globals.TryParse(s.Substring(4,2), 0);
			int day = Globals.TryParse(s.Substring(6,2), 0);
			int hour = Globals.TryParse(s.Substring(8,2), 0);
			int minute = Globals.TryParse(s.Substring(10,2), 0);
			int second = Globals.TryParse(s.Substring(12,2) ,0);
		
			if (year < 1900 ||year > 2100 || month < 1 ||month > 12 || day < 1 ||day > 31 ||hour >23 || minute > 59 || second > 59)
				return t;

			try
			{
				t = new DateTime(year,month,day,hour,minute,second);
			}
			catch
			{
				return new DateTime(1975,1,1,0,0,0);
			}

			return t;

		}


		public static string FindFirstFile(string folder, string searchMask)
		{
			if (folder == "" || searchMask == "")
				return "";

			string fileNameFound = "";
			try 
			{
				string[] files = System.IO.Directory.GetFiles(folder , searchMask);
				
				if (files.Length == 0)
					return "";
		
				foreach (string file in files) 
				{	
					// Additional check
					if (System.IO.File.Exists(file))
					{
						fileNameFound = file;
						break;
					}
				}
			} 
			catch 
			{
				return "";
			}

			return fileNameFound;
		}


        public static bool CheckTileXML(string folder, string publication, DateTime pubDate)
        {
         
            string file = folder + "\\ImageProperties.xml";

            
            if (System.IO.File.Exists(file) == false)
                return false;

      //      if ((bool)HttpContext.Current.Application["UseSeaDragon"] && System.IO.File.Exists(folder + "\\image.xml") == false)
      //          return false;

            if ((bool)HttpContext.Current.Application["CheckJpegComment"] == false)
                return true;

            string sFile = "";
            using (System.IO.StreamReader sr = System.IO.File.OpenText(file))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    sFile += s;
                }
            }

            int n = sFile.IndexOf("PUBLICATION=\"");
            if (n == -1)
                n = sFile.IndexOf("publication=\"");
            if (n == -1)
                return false;

            n += 13;
            int m = sFile.IndexOf("\"", n);
            if (m == -1)
                return false;

            string publicationXML = SanitizeString(sFile.Substring(n, m - n).ToLower());

            n = sFile.IndexOf("PUBDATE=\"", m);
            if (n == -1)
                n = sFile.IndexOf("pubdate=\"");
            if (n == -1)
                return false;

            n += 9;
            m = sFile.IndexOf("\"", n);
            if (m == -1)
                return false;

            string pubDateXML = sFile.Substring(n, m - n);

            string pubACSII = SanitizeString(publication.ToLower());

            string pubDateASCII = string.Format("{0:0000}{1:00}{2:00}", pubDate.Year, pubDate.Month, pubDate.Day);

            Global.logging.WriteLog(string.Format("XML Image comment for {0} : {1} {2} compare to {3} {4}", file, publicationXML, pubDateXML, pubACSII, pubDateASCII));

            return publicationXML == pubACSII && pubDateASCII == pubDateXML;

        }

        public static string SanitizeString(string input)
        {
            string output = "";

            for (int i = 0; i < input.Length; i++)
                if (input[i] == ' ' || (input[i] >= 'a' && input[i] <= 'z') || (input[i] >= 'A' && input[i] <= 'Z') || (input[i] >= '0' && input[i] <= '9'))
                    output += input[i].ToString();

            return output;

        }

        public static string MakePreviewGUID(int publicationID, DateTime pubDate)
        {
            return string.Format("{0:0000}{1:00}{2:00}{3:00}", publicationID, pubDate.Year - 2000, pubDate.Month, pubDate.Day);
        }

        public static string EncodePreviewName(string file)
        {
            string s = file;
            s = s.Replace("#","%23");

            return s;
            
        }

        public static bool CheckJPGComment(string fileName, string publication, DateTime pubDate)
        {
            return true;

            ///// NOT USED!!!!!!!!!!!!!!!!!
      
            /*
            if ((bool)HttpContext.Current.Application["CheckJpegComment"] == false)
                return true;

            string pubACSII = SanitizeString(publication.ToLower());
            string pubDateASCII = string.Format("{0:0000}{1:00}{2:00}", pubDate.Year, pubDate.Month, pubDate.Day);


            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (BinaryReader br = new BinaryReader(fs, new ASCIIEncoding()))
                {
                    byte[] readbuffer;
                    StringBuilder dumptext = new StringBuilder(256);

                    readbuffer = br.ReadBytes(208);
                    if (readbuffer.Length < 200)
                        return true;

                    if (readbuffer[0] != 0xFF || readbuffer[1] != 0xD8)
                        return true;

                    if (readbuffer[20] != 0xFF)
                        return true;

                    if (readbuffer[21] != 0xFE && readbuffer[21] != 0xEE)
                        return true;


                    int n = 0;
	                int pos = 26;

//                    while (pos < 30)
//                    {
 //                       if (readbuffer[pos] == 0x23 && readbuffer[pos + 1] == 0x23)
 //                       {
 //                           pos += 2;
 //                           break;
 //                       }
 //                       pos++;
 //                   }
   
	                while (pos < 200 && n < 99) 
                    {
		                if (readbuffer[pos] == 0x23 && readbuffer[pos+1] == 0x23)
			                break;
                        dumptext.Append((char)readbuffer[pos++]);
                    }

                    // Old previews do not write comment tag
                    string s = dumptext.ToString();
                    if (s.Length < 7)
                        return true;

                    // Format ##publication#YYYYMMDD#E#S#P##

                    if (s[0] != '#' || s[1] != '#')
                        return true;

                    s = s.Substring(2);
                    string[] elements = s.Split('#');
                    if (elements.Length < 2)
                        return true;

                    string pubJFIF = SanitizeString(elements[0].ToLower());
                    string pubdateJFIF = elements[1];
                    Global.logging.WriteLog(string.Format("Image comment for {0} : {1} {2} to compare: {3} {4}", fileName, pubJFIF, pubdateJFIF, pubACSII, pubDateASCII));


                    return pubACSII == pubJFIF && pubDateASCII == pubdateJFIF;

                }
            }

            return true;
    */

        }

        public static bool HasPdfPage(int masterCopySeparationSet, out string realPath, out string virtualPath)
        {
            return HasPdfPage(masterCopySeparationSet, out realPath, out virtualPath, false);
        }

        public static bool HasPdfPage(int masterCopySeparationSet, out string realPath, out string virtualPath, bool tryannotatedreport)
        {

            virtualPath = ""; realPath = "";

            if (tryannotatedreport)
            {
                realPath = Globals.FindFirstFile(Global.sRealPdfFolder, "*_annotated_" + masterCopySeparationSet.ToString() + ".pdf");
                if (realPath != "")
                {                
                    virtualPath = Global.sVirtualPdfFolder + "/" + System.IO.Path.GetFileName(realPath);
                    return true;
                }
            }

            realPath = Globals.FindFirstFile(Global.sRealPdfFolder, "*_" + masterCopySeparationSet.ToString() + ".pdf");
            if (realPath != "")
            {                
                virtualPath = Global.sVirtualPdfFolder + "/" + System.IO.Path.GetFileName(realPath);
                return true;
            } 

            if (Global.sRealArchiveFolder != "")
            {
                if (HasPdfInArchive(masterCopySeparationSet, (string)HttpContext.Current.Session["SelectedPublication"], (DateTime)HttpContext.Current.Session["SelectedPubDate"], ref realPath, ref virtualPath) == true)
                {
                    if (realPath != "" && virtualPath != "")
                        return true;
                }

            }

            return false;
        }

        public static bool HasPdfLog(int masterCopySeparationSet, out string realPath, out string virtualPath)
        {
            realPath = "";
            virtualPath = "";

            realPath = Globals.FindFirstFile(Global.sRealPdfLogFolder, "*_log_" + masterCopySeparationSet.ToString() + ".pdf");
            if (realPath != "")
            {
                virtualPath = Global.sVirtualPdfLogFolder + "/" + System.IO.Path.GetFileName(realPath);
                return true;

            }

            return false;
        }

        public static bool HasAlwanLog(int masterCopySeparationSet, out string realPath, out string virtualPath)
        {
            realPath = "";
            virtualPath = "";

            realPath = Globals.FindFirstFile(Global.sRealAlwanLogFolder, "*_log_" + masterCopySeparationSet.ToString() + ".pdf");
            if (realPath != "")
            {
                virtualPath = Global.sVirtualAlwanLogFolder + "/" + System.IO.Path.GetFileName(realPath);
                return true;

            }

            return false;
        }


        public static bool CheckJPGComment(int masterCopySeparationSet, int version, int publicationID, DateTime pubDate )
        {
            if ((bool)HttpContext.Current.Application["CheckJpegComment"] == false)
                return true;

            string realPath = "";
            string virtualPath = "";
            string fileToTest = "";
            string guid = MakePreviewGUID(publicationID, pubDate);
            string filetitle;
            bool hasGUIDfile = false;
 
            if (version > 0)
            {
                filetitle = guid + "====" + masterCopySeparationSet.ToString() + "-" + version.ToString() + ".jpg";
                fileToTest = Global.sRealImageFolder + "\\" + filetitle;
                hasGUIDfile = (bool)HttpContext.Current.Application["OldFileNames"] == false && System.IO.File.Exists(fileToTest);
                if (hasGUIDfile == false)
                {
                    filetitle = masterCopySeparationSet.ToString() + "-" + version.ToString() + ".jpg";
                    fileToTest = Global.sRealImageFolder + "\\" + filetitle;
                }

                if (hasGUIDfile || System.IO.File.Exists(fileToTest))
                {
                    virtualPath = Global.sVirtualImageFolder + "/" + EncodePreviewName(filetitle);
                    realPath = fileToTest;
                }
            }
            else
            {
                filetitle = guid + "====" + masterCopySeparationSet.ToString() + ".jpg";
                fileToTest = Global.sRealImageFolder + "\\" + filetitle;
                hasGUIDfile = (bool)HttpContext.Current.Application["OldFileNames"] == false && System.IO.File.Exists(fileToTest);
                if (hasGUIDfile == false)
                {
                    filetitle = masterCopySeparationSet.ToString() + ".jpg";
                    fileToTest = Global.sRealImageFolder + "\\" + filetitle;
                }
                if (hasGUIDfile || System.IO.File.Exists(fileToTest))
                {
                    virtualPath = Global.sVirtualImageFolder + "/" + EncodePreviewName(filetitle);
                    realPath = fileToTest;
                }

            }

            if (realPath == "")
                return false;

            return CheckJPGComment(realPath, Globals.GetNameFromID("PublicationNameCache", publicationID), pubDate);
        }

        public static bool HasPreview(int masterCopySeparationSet, int version, int publicationID, DateTime pubDate, bool masked, ref string realPath, ref string virtualPath, string color)
        {
            realPath = "";
            virtualPath = "";
            string fileToTest;
            if (color.Length > 0)
                color = "_" + color;
            string guid = MakePreviewGUID(publicationID, pubDate);
            string filetitle;
            bool hasGUIDfile;

            if (masked == false)
            {
                if (version > 0)
                {
                    filetitle = guid + "====" + masterCopySeparationSet.ToString() + "-" + version.ToString() + color + ".jpg";
                    fileToTest = Global.sRealImageFolder + "\\" + filetitle;
                    hasGUIDfile = (bool)HttpContext.Current.Application["OldFileNames"] == false && System.IO.File.Exists(fileToTest);
                    if (hasGUIDfile == false)
                    {
                        filetitle = masterCopySeparationSet.ToString() + "-" + version.ToString() + color + ".jpg";
                        fileToTest = Global.sRealImageFolder + "\\" + filetitle;
                    }
                    if (hasGUIDfile || System.IO.File.Exists(fileToTest))
                    {
                        virtualPath = Global.sVirtualImageFolder + "/" + EncodePreviewName(filetitle);
                        realPath = fileToTest;

                        return true;
                    }
                }

                filetitle = guid + "====" + masterCopySeparationSet.ToString() + color + ".jpg";
                fileToTest = Global.sRealImageFolder + "\\" + filetitle;
                hasGUIDfile = (bool)HttpContext.Current.Application["OldFileNames"] == false && System.IO.File.Exists(fileToTest);
                if (hasGUIDfile == false)
                {
                    filetitle = masterCopySeparationSet.ToString() + color + ".jpg";
                    fileToTest = Global.sRealImageFolder + "\\" + filetitle;
                }
                if (hasGUIDfile || System.IO.File.Exists(fileToTest))
                {
                    virtualPath = Global.sVirtualImageFolder + "/" + EncodePreviewName(filetitle);
                    realPath = fileToTest;
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {

                if (version > 0)
                {
                    filetitle = guid + "====" + masterCopySeparationSet.ToString() + "-" + version.ToString() + "_mask.jpg";
                    fileToTest = Global.sRealImageFolder + "\\" + filetitle;
                    hasGUIDfile = (bool)HttpContext.Current.Application["OldFileNames"] == false && System.IO.File.Exists(fileToTest);
                    if (hasGUIDfile == false)
                    {
                        filetitle = masterCopySeparationSet.ToString() + "-" + version.ToString() + "_mask.jpg";
                        fileToTest = Global.sRealImageFolder + "\\" + filetitle;
                    }

                    if (hasGUIDfile || System.IO.File.Exists(fileToTest))
                    {
                        virtualPath = Global.sVirtualImageFolder + "/" + EncodePreviewName(filetitle);;
                        realPath = fileToTest;
                        return true;
                    }
                }

                filetitle = guid + "====" + masterCopySeparationSet.ToString() + "_mask.jpg";
                fileToTest = Global.sRealImageFolder + "\\" + filetitle;
                hasGUIDfile = (bool)HttpContext.Current.Application["OldFileNames"] == false && System.IO.File.Exists(fileToTest);
                if (hasGUIDfile == false)
                {
                    filetitle = masterCopySeparationSet.ToString() + "_mask.jpg";
                    fileToTest = Global.sRealImageFolder + "\\" + filetitle;
                }

                if (hasGUIDfile || System.IO.File.Exists(fileToTest))
                {
                    virtualPath = Global.sVirtualImageFolder + "/" + EncodePreviewName(filetitle);
                    realPath = fileToTest;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            //return false;
        }

        public static bool HasTileFolder(int masterCopySeparationSet, int version, int publicationID, DateTime pubDate, bool masked, ref string realPath, ref string virtualPath)
        {
            realPath = "";
            virtualPath = "";
            string folderToTest;
            string guid = MakePreviewGUID(publicationID, pubDate);
            string foldertitle;
            bool hasGUIDfolder;

            if (masked == false)
            {
                // 1. Check with version information
                if (version > 0)
                {
                    foldertitle = guid + "====" + masterCopySeparationSet.ToString() + "-" + version.ToString();
                    folderToTest = Global.sRealImageFolder + "\\" + foldertitle  + "\\";
                    hasGUIDfolder = (bool)HttpContext.Current.Application["OldFileNames"] == false && System.IO.Directory.Exists(folderToTest);
                    if (hasGUIDfolder == false)
                    {
                        foldertitle = masterCopySeparationSet.ToString() + "-" + version.ToString();
                        folderToTest = Global.sRealImageFolder + "\\" + foldertitle + "\\";
                    }

                    if (hasGUIDfolder || System.IO.Directory.Exists(folderToTest))
                    {
                        if (Globals.CheckTileXML(folderToTest, Globals.GetNameFromID("PublicationNameCache", publicationID), pubDate))
                        {
                            virtualPath = Global.sVirtualImageFolder + "/" + EncodePreviewName(foldertitle);
                            realPath = EncodePreviewName(folderToTest);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    // No version folder - fall back to un-versioned folder..
                }

                // 2. Check without version information
                foldertitle = guid + "====" + masterCopySeparationSet.ToString();
                folderToTest = Global.sRealImageFolder + "\\" + foldertitle + "\\";
                hasGUIDfolder = (bool)HttpContext.Current.Application["OldFileNames"] == false && System.IO.Directory.Exists(folderToTest);
                if (hasGUIDfolder == false)
                {
                    foldertitle = masterCopySeparationSet.ToString();
                    folderToTest = Global.sRealImageFolder + "\\" + foldertitle + "\\";
                }
                if (hasGUIDfolder || System.IO.Directory.Exists(folderToTest))
                {
                    if (Globals.CheckTileXML(folderToTest, Globals.GetNameFromID("PublicationNameCache", publicationID), pubDate))
                    {
                        virtualPath = Global.sVirtualImageFolder + "/" + EncodePreviewName(foldertitle);
                        realPath = EncodePreviewName(folderToTest);
                        return true;
                    }
                    else
                    {

                        return false;
                    }
                }
            }
            else
            {
                // 1. Check with version information
                if (version > 0)
                {
                    foldertitle = guid + "====" + masterCopySeparationSet.ToString() + "-" + version.ToString() + "_mask";
                    folderToTest = Global.sRealImageFolder + "\\" +foldertitle + "\\";
                    hasGUIDfolder = (bool)HttpContext.Current.Application["OldFileNames"] == false && System.IO.Directory.Exists(folderToTest);
                    if (hasGUIDfolder == false)
                    {
                        foldertitle = masterCopySeparationSet.ToString() + "-" + version.ToString() + "_mask";
                        folderToTest = Global.sRealImageFolder + "\\" +foldertitle + "\\";
                    }
                    if (hasGUIDfolder || System.IO.Directory.Exists(folderToTest))
                    {
                        if (Globals.CheckTileXML(folderToTest, Globals.GetNameFromID("PublicationNameCache", publicationID), pubDate))
                        {
                            virtualPath = Global.sVirtualImageFolder + "/" + EncodePreviewName(foldertitle);
                            realPath = EncodePreviewName(folderToTest);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    // No version folder - fall back to un-versioned folder..
                }

                // 2. Check without foler information

                foldertitle = guid + "###" + masterCopySeparationSet.ToString() + "_mask";
                folderToTest = Global.sRealImageFolder + "\\" + foldertitle + "\\";
                hasGUIDfolder = (bool)HttpContext.Current.Application["OldFileNames"] == false && System.IO.Directory.Exists(folderToTest);
                if (hasGUIDfolder == false)
                {
                    foldertitle = masterCopySeparationSet.ToString() + "_mask";
                    folderToTest = Global.sRealImageFolder + "\\" + foldertitle + "\\";
                }


                if (hasGUIDfolder || System.IO.Directory.Exists(folderToTest))
                {
                    if (Globals.CheckTileXML(folderToTest, Globals.GetNameFromID("PublicationNameCache", publicationID), pubDate))
                    {
                        virtualPath = Global.sVirtualImageFolder + "/" + EncodePreviewName(foldertitle);
                        realPath = EncodePreviewName(folderToTest);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        public static bool HasRasterTileFolder(int masterCopySeparationSet, int version, int publicationID, DateTime pubDate, ref string realPath, ref string virtualPath)
        {

            if (Global.sVirtualRasterImageFolder == "" || Global.sVirtualRasterImageFolder == "/" || Global.sRealRasterImageFolder == "")
                return false;

            realPath = "";
            virtualPath = "";
            string folderToTest;
            string guid = MakePreviewGUID(publicationID, pubDate);
            string foldertitle;
            bool hasGUIDfolder;

            // 1. Check with version information
            if (version > 0)
            {
                foldertitle = guid + "====" + masterCopySeparationSet.ToString() + "-" + version.ToString();
                folderToTest = Global.sRealRasterImageFolder + "\\" + foldertitle + "\\";
                hasGUIDfolder = (bool)HttpContext.Current.Application["OldFileNames"] == false && System.IO.Directory.Exists(folderToTest);
                if (hasGUIDfolder == false)
                {
                    foldertitle = masterCopySeparationSet.ToString() + "-" + version.ToString();
                    folderToTest = Global.sRealRasterImageFolder + "\\" + foldertitle + "\\";
                }

                if (hasGUIDfolder || System.IO.Directory.Exists(folderToTest))
                {
                    if (Globals.CheckTileXML(folderToTest, Globals.GetNameFromID("PublicationNameCache", publicationID), pubDate))
                    {
                        virtualPath = Global.sVirtualRasterImageFolder + "/" + EncodePreviewName(foldertitle);
                        realPath = EncodePreviewName(folderToTest);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                // No version folder - fall back to un-versioned folder..
            }

            // 2. Check without version information
            foldertitle = guid + "====" + masterCopySeparationSet.ToString();
            folderToTest = Global.sRealRasterImageFolder + "\\" + foldertitle + "\\";
            hasGUIDfolder = (bool)HttpContext.Current.Application["OldFileNames"] == false && System.IO.Directory.Exists(folderToTest);
            if (hasGUIDfolder == false)
            {
                foldertitle = masterCopySeparationSet.ToString();
                folderToTest = Global.sRealRasterImageFolder + "\\" + foldertitle + "\\";
            }
            if (hasGUIDfolder || System.IO.Directory.Exists(folderToTest))
            {
                if (Globals.CheckTileXML(folderToTest, Globals.GetNameFromID("PublicationNameCache", publicationID), pubDate))
                {
                    virtualPath = Global.sVirtualRasterImageFolder + "/" + EncodePreviewName(foldertitle);
                    realPath = EncodePreviewName(folderToTest);
                    return true;
                }
                else
                {

                    return false;
                }
            }

            return true;
        }




        public static bool HasTileFolderReadview(int masterCopySeparationSetLeft, int masterCopySeparationSetRight, int publicationID, DateTime pubDate, bool masked, ref string realPath, ref string virtualPath)
        {
            realPath = "";
            virtualPath = "";
            string folderToTest;
            string guid = MakePreviewGUID(publicationID, pubDate);
            string foldertitle;
            bool hasGUIDfolder;

            if (masked == false)
            {
                foldertitle = guid + "====" + masterCopySeparationSetLeft.ToString() + "_" + masterCopySeparationSetRight.ToString();
                folderToTest = Global.sRealReadViewImageFolder + "\\" + foldertitle + "\\";
                hasGUIDfolder = (bool)HttpContext.Current.Application["OldFileNames"] == false && System.IO.Directory.Exists(folderToTest);
                if (hasGUIDfolder == false)
                {
                    foldertitle = masterCopySeparationSetLeft.ToString() + "_" + masterCopySeparationSetRight.ToString();
                    folderToTest = Global.sRealReadViewImageFolder + "\\" + foldertitle + "\\";
                }
                if (hasGUIDfolder || System.IO.Directory.Exists(folderToTest))
                {
                    if (Globals.CheckTileXML(folderToTest, Globals.GetNameFromID("PublicationNameCache", publicationID), pubDate))
                    {
                        virtualPath = Global.sVirtualReadViewImageFolder + "/" + EncodePreviewName(foldertitle);
                        realPath = EncodePreviewName(folderToTest);
                        return true;
                    }

                }
                return false;
            }
            else
            {
                foldertitle = guid + "====" + masterCopySeparationSetLeft.ToString() + "_" + masterCopySeparationSetRight.ToString() + "_mask";
                folderToTest = Global.sRealReadViewImageFolder + "\\" + foldertitle + "\\";
                hasGUIDfolder = (bool)HttpContext.Current.Application["OldFileNames"] == false && System.IO.Directory.Exists(folderToTest);
                if (hasGUIDfolder == false)
                {
                    foldertitle = masterCopySeparationSetLeft.ToString() + "_" + masterCopySeparationSetRight.ToString() + "_mask";
                    folderToTest = Global.sRealReadViewImageFolder + "\\" + foldertitle + "\\";
                }
                if (hasGUIDfolder || System.IO.Directory.Exists(folderToTest))
                {
                    if (Globals.CheckTileXML(folderToTest, Globals.GetNameFromID("PublicationNameCache", publicationID), pubDate))
                    {
                        virtualPath = Global.sVirtualReadViewImageFolder + "/" + EncodePreviewName(foldertitle);
                        realPath = EncodePreviewName(folderToTest);
                        return true;
                    }
                }

                return false;
            }

        }

        public static bool HasPreviewReadview(int masterCopySeparationSetLeft, int masterCopySeparationSetRight, int publicationID, DateTime pubDate, bool masked, ref string realPath, ref string virtualPath)
        {
            realPath = "";
            virtualPath = "";
            string fileToTest;

            string guid = MakePreviewGUID(publicationID, pubDate);
            string foldertitle;
            bool   hasGUIDfile;

            if (masked == false)
            {
                foldertitle = guid + "====" + masterCopySeparationSetLeft.ToString() + "_" + masterCopySeparationSetRight.ToString();
                fileToTest = Global.sRealReadViewImageFolder + "\\" + foldertitle;
                hasGUIDfile = (bool)HttpContext.Current.Application["OldFileNames"] == false && System.IO.File.Exists(fileToTest);
                if (hasGUIDfile == false)
                {
                    foldertitle = masterCopySeparationSetLeft.ToString() + "_" + masterCopySeparationSetRight.ToString();
                    fileToTest = Global.sRealReadViewImageFolder + "\\" + foldertitle;
                }
                if (hasGUIDfile || System.IO.File.Exists(fileToTest))
                {
                    virtualPath = Global.sVirtualReadViewImageFolder + "/" + EncodePreviewName(foldertitle);
                    realPath = EncodePreviewName(fileToTest);
                    return true;
                }

                return false;


            }
            else
            {
                foldertitle = guid + "====" + masterCopySeparationSetLeft.ToString() + "_" + masterCopySeparationSetRight.ToString() + "_mask.jpg";
                fileToTest = Global.sRealReadViewImageFolder + "\\" + foldertitle;
                hasGUIDfile = (bool)HttpContext.Current.Application["OldFileNames"] == false && System.IO.File.Exists(fileToTest);
                if (hasGUIDfile == false)
                {
                    foldertitle = masterCopySeparationSetLeft.ToString() + "_" + masterCopySeparationSetRight.ToString() + "_mask.jpg";
                    fileToTest = Global.sRealReadViewImageFolder + "\\" + foldertitle;
                }
                if (hasGUIDfile || System.IO.File.Exists(fileToTest))
                {
                    virtualPath = Global.sVirtualReadViewImageFolder + "/" + EncodePreviewName(foldertitle);
                    realPath = EncodePreviewName(fileToTest);
                    return true;
                }


                return false;
            }

            //return false;

        }


        public static bool HasPreviewFlatview(int copyFlatSeparationSet, bool masked, ref string realPath, ref string virtualPath, string color)
        {
            realPath = "";
            virtualPath = "";

            if (color != "")
                color = "_" + color;

            if (Global.sRealFlatImageFolder == "" || Global.sVirtualFlatImageFolder == "")
                return false;

            string fileToTest;
            if (masked == false)
            {
                fileToTest = Global.sRealFlatImageFolder + "\\" + copyFlatSeparationSet.ToString() + color + ".jpg";

                if (System.IO.File.Exists(fileToTest))
                {
                    virtualPath = Global.sVirtualFlatImageFolder + "/" + copyFlatSeparationSet.ToString() + color + ".jpg";
                    realPath = fileToTest;
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {


                fileToTest = Global.sRealFlatImageFolder + "\\" + copyFlatSeparationSet.ToString() + "_mask.jpg";
                if (System.IO.File.Exists(fileToTest))
                {
                    virtualPath = Global.sVirtualFlatImageFolder + "/" + copyFlatSeparationSet.ToString() + "_mask.jpg";
                    realPath = fileToTest;
                    return true;
                }
                else
                {

                    return false;
                }


            }

            //return false;

        }


        public static bool HasTileFolderFlatview(int copyFlatSeparationSet, int nVersion, int publicationID, DateTime pubDate, bool masked, ref string realPath, ref string virtualPath)
        {
            realPath = "";
            virtualPath = "";
            if (Global.sRealFlatImageFolder == "" || Global.sVirtualFlatImageFolder == "")
                return false;

            string folderToTest;
            if (masked == false)
            {

                if (nVersion > 0)
                    folderToTest = Global.sRealFlatImageFolder + "\\" + copyFlatSeparationSet.ToString() + "-" + nVersion.ToString() + "\\";
                else
                    folderToTest = Global.sRealFlatImageFolder + "\\" + copyFlatSeparationSet.ToString() + "\\";
                if (System.IO.Directory.Exists(folderToTest))
                {



                    if (Globals.CheckTileXML(folderToTest, Globals.GetNameFromID("PublicationNameCache", publicationID), pubDate))
                    {
                        if (nVersion > 0)
                            virtualPath = Global.sVirtualFlatImageFolder + "/" + copyFlatSeparationSet.ToString() + "-" + nVersion.ToString();
                        else
                            virtualPath = Global.sVirtualFlatImageFolder + "/" + copyFlatSeparationSet.ToString();
                        realPath = folderToTest;
                        return true;
                    }
                    else
                    {

                        return false;
                    }


                }
                else
                    return false;
            }
            else
            {
                if (nVersion > 0)
                    folderToTest = Global.sRealFlatImageFolder + "\\" + copyFlatSeparationSet.ToString() + "-" + nVersion.ToString() + "_mask\\";
                else
                    folderToTest = Global.sRealFlatImageFolder + "\\" + copyFlatSeparationSet.ToString() + "_mask\\";
                if (System.IO.Directory.Exists(folderToTest))
                {
                    if (System.IO.File.Exists(folderToTest + "\\ImageProperties.xml") == false)
                        return false;

                    if (nVersion > 0)
                        virtualPath = Global.sVirtualFlatImageFolder + "/" + copyFlatSeparationSet.ToString() + "-" + nVersion.ToString() + "_mask";
                    else
                        virtualPath = Global.sVirtualFlatImageFolder + "/" + copyFlatSeparationSet.ToString() + "_mask";
                    realPath = folderToTest;
                    return true;
                }
                else
                    return false;
            }

        }


        public static bool HasPdfInArchive(int masterCopySeparationSet, string publication, DateTime pubDate, ref string pdfPath, ref string pdfVirtualPath)
        {
            bool found = false;
            pdfPath = "";

            if (Global.sVirtualArchiveFolder == "")
                return false;

            CCDBaccess db = new CCDBaccess();
            string errmsg;

            string pdfName;

            if (db.GetFileCenterPDFname(masterCopySeparationSet, true, out pdfName, out errmsg) == false)           // archive event
                if (db.GetFileCenterPDFname(masterCopySeparationSet, false, out pdfName, out errmsg) == false)      // ordinary FTP event
                    return false;
                

            if (pdfName == "")
                return false;

            if (pdfName.IndexOf("\\") != -1)
            {
                pdfVirtualPath = Global.sVirtualArchiveFolder + "/" + pdfName;
                pdfVirtualPath.Replace("\\", "/");
                pdfPath = Global.sRealArchiveFolder + "\\" + pdfName;
                Global.logging.WriteLog("HasPdfInArchive() - Testing file exists: " + pdfPath);
                found = File.Exists(pdfPath);
            }
            else
            {
                string pubNameAlias = Globals.LookupInputAlias("Publication", publication);
                string pubDateName = string.Format("{0:0000}-{1:00}-{2:00}", pubDate.Year, pubDate.Month, pubDate.Day);

                pdfPath = Global.sRealArchiveFolder + "\\" + pubDateName + "\\" + pubNameAlias + "\\" + pdfName;
                pdfVirtualPath = Global.sVirtualArchiveFolder + "/" + pubDateName + "/" + pubNameAlias + "/" + pdfName;

                Global.logging.WriteLog("HasPdfInArchive() - Testing file exists: " + pdfPath);
                found = File.Exists(pdfPath);
                if (found == false)
                {
                    pdfPath = Global.sRealArchiveFolder + "\\" + pubDateName + "\\" + publication + "\\" + pdfName;
                    pdfVirtualPath = Global.sVirtualArchiveFolder + "/" + pubDateName + "/" + publication + "/" + pdfName;
                    Global.logging.WriteLog("HasPdfInArchive() - Testing file exists: " + pdfPath);
                    found = File.Exists(pdfPath);
                }
            }

            return found;
        }

        public static string ToStandardDateString(DateTime dt)
        {
            return string.Format("{0:0000}{1:00}{2:00}", dt.Year, dt.Month, dt.Day);
        }

        public static string ToShortDateString(DateTime dt)
        {
            return string.Format("{0:00}-{1:00}", dt.Day, dt.Month);
        }

        public static DateTime FromStandardDateString(string s)
        {
            return new DateTime(Convert.ToInt32(s.Substring(0, 4)), Convert.ToInt32(s.Substring(4, 2)), Convert.ToInt32(s.Substring(6, 2)), 0, 0, 0);
        }

        public static bool IsMobileClient(HttpRequest request, HttpResponse Response)
        {
            if ((HttpContext.Current.Request.UserAgent.ToLower().IndexOf("ipad") != -1) ||
               (HttpContext.Current.Request.UserAgent.ToLower().IndexOf("iphone") != -1))
                return true;

            return false;
        }

        public static bool IsInArray(ArrayList array, string toFind)
        {
            foreach (string s in array)
            {
                if (s == toFind)
                    return true;
            }
            return false;
        }

        public static bool IsInArray(ArrayList array, int toFind)
        {
            foreach (int n in array)
            {
                if (n == toFind)
                    return true;
            }
            return false;
        }

        public static bool IsInList(string[] array, string toFind)
        {
            foreach (string s in array)
            {
                if (s == toFind)
                    return true;
            }
            return false;
        }


        public static void AddToArray(ref ArrayList  array, int elementToAdd)
        {
            if (IsInArray(array, elementToAdd) == false)
                array.Add(elementToAdd);
        }

        public static void AddToArray(ref ArrayList array, string elementToAdd)
        {
            if (elementToAdd == "")
                return;
            if (IsInArray(array, elementToAdd) == false)
                array.Add(elementToAdd);
        }

        public static string StringListToCSVString(List<string> stringList)
        {
            string sfinal = "";
            foreach (string s in stringList)
            {
                if (sfinal != "")
                    sfinal += ",";
                sfinal += s;
            }
            return sfinal;
        }


    }

	public class PageSection 
	{
	
		public int numberofpages;
		public string[] aPageNameList;
		public int[] aPaginationList;
		public int[][] aColorIDList;
		public bool[][] aColorActiveList;
		
		public PageSection(int nNumberOfPages) 
		{ 
			numberofpages = nNumberOfPages;
			aPageNameList = new string[nNumberOfPages];
			aColorIDList = new int[nNumberOfPages][];
			aColorActiveList = new bool[nNumberOfPages][];
			aPaginationList = new int[nNumberOfPages];

		}

		public void SetColors(int index, int nColors, int[] col, bool[] active)
		{	
			aColorIDList[index] = new int[nColors];
			aColorActiveList[index] = new bool[nColors];
			for(int j=0; j<nColors; j++)
			{
				aColorIDList[index][j] = col[j];
				aColorActiveList[index][j] = active[j];
			}
		}

		public void SetPageName(int index, string pageName)
		{	
			aPageNameList[index] = pageName;
		}

		public void SetPagination(int index, int pagination)
		{	
			aPaginationList[index] = pagination;
		}

		public string GetPageName(int index)
		{	
			if (index > numberofpages-1)
				return "";
			return aPageNameList[index];
		}

		public string GetPageNameFromPagination(int pagination)
		{
			int index;
			for ( index=0; index<numberofpages; index++)
				if (aPaginationList[index] == pagination)
					break;

			if (index > numberofpages-1)
				return "";
			return aPageNameList[index];
		}

		public int GetPagination(int index)
		{	
			if (index > numberofpages-1)
				return 1;
			return aPaginationList[index];
		}

		public int GetNumberOfPageColors(int index)
		{	
			if (index > numberofpages-1)
				return 0;
			return aColorIDList[index].Length;
		}
		
		public int GetNumberOfPageColorsFromPagination(int pagination)
		{	
			int index;
			for ( index=0; index<numberofpages; index++)
				if (aPaginationList[index] == pagination)
					break;

			if (index > numberofpages-1)
				return 0;
			return aColorIDList[index].Length;
		}

		public int GetPageColorID(int index, int colindex, out bool active)
		{	
			active = false;
			if (index > numberofpages-1)
				return 0;
			if (colindex >aColorActiveList[index].Length - 1)
				return 0;
			active = aColorActiveList[index][colindex];
			return aColorIDList[index][colindex];
		}

		public int GetPaginationColorID(int pagination, int colindex, out bool active)
		{	
			active = false;
			int index;
			for (index=0; index<numberofpages; index++)
				if (aPaginationList[index] == pagination)
					break;

			if (index > numberofpages-1)
				return 0;
			if (colindex >aColorActiveList[index].Length - 1)
				return 0;
			active = aColorActiveList[index][colindex];
			return aColorIDList[index][colindex];
		}

		public bool HasCMYKpage()
		{	
			for(int i=0; i<numberofpages; i++)
			{
				int nProcessColors = 0;
				for (int j=0; i<aColorIDList.Length; j++)
				{
					if (aColorActiveList[i][j])
						if (Globals.IsProcessColor((int)aColorIDList[i][j]))
							nProcessColors++;
				}
				if (nProcessColors == 4)
					return true;
			}

			return false;
		}

		public bool AllMonoPages()
		{	
			for(int i=0; i<numberofpages; i++)
			{
				int nColors = 0;
				for (int j=0; i<aColorIDList.Length; j++)
					if (aColorActiveList[i][j])
						nColors++;

				if (nColors >1)
					return false;
			}

			return true;
		}


		
	}


	[Serializable]
	public class OrderEntry
	{
		public OrderEntry()
		{
			this.Init();
		}
		public string	m_customerOrderNumber;
		public string	m_customerOrderNumberPressRun;
		public string	m_customerOrderStatus;
		public DateTime	m_customerOrderTime;
		public int		m_customerID;
		public int		m_pressID;
		public int		m_publicationID;
		public DateTime	m_pubDate;
		public int		m_editionID;
		public string	m_sectionIDList;
		public string	m_pagesInSectionList;
		public bool		m_inserted;
		public DateTime	m_pressTime;
		public string	m_comment;
		public int		m_orderID;
		public int		m_circulation;
		public int		m_circulation2;
		public int		m_timedEditionFrom;
		public int		m_timedEditionTo;
		public int		m_timedEditionSequence;
        public string m_miscString3;
	
		public void Init()
		{	
			m_orderID = 0;
			m_customerOrderNumber = "";
			m_customerOrderNumberPressRun = "";
			m_customerOrderStatus = "Unknown";
			m_customerOrderTime = DateTime.MinValue;
			m_customerID = 0;
			m_pressID = 0;
			m_publicationID = 0;
			m_pubDate = DateTime.MinValue;
			m_editionID = 0;
			m_sectionIDList = "";
			m_pagesInSectionList = "";
			m_inserted = false;
			m_pressTime = DateTime.MinValue;
			m_comment = "";
			m_circulation = 0;
			m_circulation2 = 0;
			m_timedEditionFrom = 0;
			m_timedEditionTo = 0;
			m_timedEditionSequence = 0;
            m_miscString3 = "";
        }

	};

    
    public class PageTableEntry 
	{
	
		public PageTableEntry() 
		{ 
			this.Init(); 
		}
	
		
		public int		m_copyseparationset;
		public int		m_separationset;
		public int		m_separation;
		public int		m_copyflatseparationset;
		public int		m_flatseparationset;
		public int		m_flatseparation;

		public int		m_status;
		public int		m_externalstatus;
		public int		m_publicationID;
		public int		m_sectionID;
		public int		m_editionID;
		public int		m_issueID;
		public DateTime	m_pubdate;

		public string	m_pagename;	// page number!
		public int		m_colorID;
		public int		m_templateID;
		public int		m_proofID;
		public int		m_deviceID;
		public int		m_version;
		public int		m_layer;	
		public int		m_copynumber;
		public int		m_pagination;
		public int		m_approved;
		public bool 	m_hold;
		public bool		m_active;
		public int		m_priority;
		public int		m_pageposition;	// NOT USED!
		public int		m_pagetype;
		public int		m_pagesonplate;
		public int		m_sheetnumber;
		public int		m_sheetside;
		public int		m_pressID;
		public int		m_presssectionnumber;
		public string	m_sortingposition;
		public string	m_presstower;
		public string	m_presszone;
		public string	m_presscylinder;
		public string	m_presshighlow;
		public int		m_productionID;
		public int		m_pressrunID;
		public int		m_proofstatus;
		public int		m_inkstatus;
		public string	m_planpagename;
		public int		m_issuesequencenumber;
		public int		m_mastercopyseparationset;
		public bool		m_uniquepage;
		public int		m_locationID;
		public int		m_flatproofID;
		public int		m_flatproofstatus;
		public double	m_creep;
		public string	m_markgroups;
		public int		m_pageindex;
		public bool		m_gutterimage;
		public int		m_outputversion;
		public int		m_hardproofID;
		public int		m_hardproofstatus;
		public string	m_fileserver;
		public bool		m_dirty;

		public string	m_comment;
		public DateTime	m_deadline;
		public string	m_pagepositions;

		public int		m_mastereditionID;
		public int		m_colorIndex;
		public bool		m_pagecountchange;

		public int		m_weeknumber;

        public int      m_ripSetupID;
        public int      m_pageFormatID;
        public int      m_customerID;

		public void Init()
		{
			m_copyseparationset = 1;
			m_separationset = 101;
			m_separation = 10101;
			m_copyflatseparationset = 1;
			m_flatseparationset = 101;
			m_flatseparation = 10101;
			m_status = 0;
			m_externalstatus = 0;

			m_publicationID = 0;
			m_sectionID = 0;
			m_editionID = 0;
			m_issueID = 0;
			m_pubdate = DateTime.Now;	// Default to neext day
			m_pubdate = m_pubdate.AddDays(1.0);
			m_pagename = "1";
			m_colorID = 1;
			m_templateID = 0;
			m_proofID = 0;
			m_deviceID = 0;
			m_version = 0;
			m_layer = 1;
			m_copynumber = 1;
			m_pagination = 0;
			m_approved = 0;
			m_hold = false;
			m_active = true;
			m_priority = 50;
			m_pageposition = 1;
			m_pagetype = 0;
			m_pagesonplate = 1;
			m_sheetnumber = 1;
			m_sheetside = 0;
			m_pressID = 0;
			m_presssectionnumber = 1;
			m_sortingposition =  "";
			m_presstower = "1";
			m_presszone = "1";
			m_presscylinder = "1";
			m_presshighlow = "H";
			m_productionID = 0;
			m_pressrunID = 0;
			m_proofstatus = 0;
			m_inkstatus = 0;
			m_planpagename = "";
			m_issuesequencenumber = 1;
			m_mastercopyseparationset = 1;
			m_uniquepage = true;
			m_locationID = 0;
			m_flatproofID = 0;
			m_flatproofstatus = 0;
			m_creep = 0.0;
			m_markgroups = "";
			m_pageindex = 1;
			m_gutterimage = false;
			m_outputversion = 0;
			m_hardproofID = 0;
			m_hardproofstatus = 0;
			m_fileserver = "";
			m_dirty = false;
			m_comment = "";
			m_deadline = DateTime.Now;
			m_pagepositions = "1";
			m_mastereditionID = 0;
			m_colorIndex = 1;
			m_pagecountchange = false;
			m_weeknumber = 0;
            m_ripSetupID = 0;
            m_pageFormatID = 0;
            m_customerID = 0;
		}

	};


	public class Logger
	{
		string myLogPath = "";
		public Logger(string logPath)
		{
			myLogPath = logPath;
		}

		public void WriteLog(string logoutput)
		{
			if ((int)HttpContext.Current.Application["Loglevel"] > 0)
			{
				// Always log to stdout
				Console.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ": " + logoutput);			

				if (myLogPath.ToLower() != "" && myLogPath.ToLower() != "stdout") 
				{
					try 
					{
						StreamWriter w = File.AppendText(myLogPath);
						w.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + logoutput);
						w.Flush();
						w.Close();
					}
					catch (Exception)
					{
					}
				}
			}
		}
	}

     

	public class PageHistory
	{
		private int _version;
		private DateTime _inputTime;
		private int _approveState;
		private DateTime _approveTime;
		private string _approveUser;
        private string _message;

		public PageHistory()
		{
			version = 0;
			_inputTime = DateTime.MinValue;
			_approveState = -1;
			_approveTime = DateTime.MinValue;
			_approveUser = "";
            _message = "";
		}

		public PageHistory(int version, DateTime inputTime, int approveState, DateTime approveTime, string approveUser, string message)
		{
			_version = version;
			_inputTime = inputTime;
			_approveState = approveState;
			_approveTime = approveTime;
			_approveUser = approveUser;
            _message = message;
		}

		public int version
		{
			get 
			{
				return _version;
			}
			set
			{
				_version = value;
			}
		}

		public DateTime inputTime
		{
			get 
			{
				return _inputTime;
			}
			set
			{
				_inputTime = value;
			}
		}

		public int approveState
		{
			get 
			{
				return _approveState;
			}
			set
			{
				_approveState = value;
			}
		}

		public DateTime approveTime
		{
			get 
			{
				return _approveTime;
			}
			set
			{
				_approveTime = value;
			}
		}

		public string approveUser
		{
			get 
			{
				return _approveUser;
			}
			set
			{
				_approveUser = value;
			}
		}

        public string message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
            }
        }

    
    }

    public class ChannelInfo
    {
        public int ChannelID { get; set; } = 0;
        public int PDFType { get; set; } = 0;
        
        
    }


	
}
