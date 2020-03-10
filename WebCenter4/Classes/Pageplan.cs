using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;






namespace WebCenter4.Classes
{
    public enum PageType { Normal, Panorama, AntiPanorama, Dummy};
    public enum PageUniqueType { Common, Unique, Forced };

    public class TemplateInfo
    {
        public int templateID { get; set; } = 0;
        public string templateName { get; set; } = "";

        public string markGroups { get; set; } = "";
        public int pressID { get; set; } = 1;
        public int nUP { get; set; } = 1;
        public bool assembleVertical { get; set; } = false;
        public string frontPageList { get; set; } = "";
        public string backPageList { get; set; } = "";
        public string frontPageListHalfWeb { get; set; } = "";
        public string backPageListHalfWeb { get; set; } = "";
        public int plateCopies { get; set; } = 1;
    }
    public class PlanDataPageSeparation
    {
        public string colorName;

	    public PlanDataPageSeparation()
	    {
	    }

	    public PlanDataPageSeparation(string color) 
	    {
		    colorName = color;
	    }
    }


    public class PlanDataPage
    {
	    public string  pageName;
	    public string  fileName;
	    public string  pageID;
	    public PageType	pageType;
	    public int		pagination;
	    public int		pageIndex;
	    public string comment;
	    public List<PlanDataPageSeparation>  colorList;
	    public PageUniqueType		uniquePage;
	    public string  masterPageID;

	    public bool	approve;
	    public bool	hold;
	    public int		priority;
	    public int		version;
	    public string  masterEdition;
	    public string  miscstring1;
	    public string  miscstring2;
	    public int	    miscint;

        public string pageFormat;


	    public PlanDataPage() 
	    {
		    pageName = "";
		    fileName = "";
		    pageID = "";
		    pageType = PageType.Normal;
		    pagination = 0;
		    pageIndex = 0;
		    comment = "";

		    masterPageID = "";
		    masterEdition = "";
		    approve = false;
		    hold = false;
		    priority = 50;
		    version = 0;
		    uniquePage = PageUniqueType.Unique;
		    miscstring1 = "";
		    miscstring2 = "";
		    miscint = 0;
            pageFormat = "";

            colorList = new List<PlanDataPageSeparation>();

	    }

        public PlanDataPage(string sPageName) 
	    {
		    pageName = sPageName;
		    fileName = "";
		    pageID = "";
		    pageType = PageType.Normal;
		    pagination = 0;
		    pageIndex = 0;
		    comment = "";

		    masterPageID = "";
		    masterEdition = "";
		    approve = false;
		    hold = false;
		    priority = 50;
		    version = 0;
		    uniquePage = PageUniqueType.Unique;
		    miscstring1 = "";
		    miscstring2 = "";
		    miscint = 0;

		    colorList = new List<PlanDataPageSeparation>();
	    }


        public bool ClonePage(ref PlanDataPage pCopy)
	    {
		    if (pCopy == null)
			    return false;

		    pCopy.approve = approve;
		    pCopy.comment = comment;
		    pCopy.fileName = fileName;
		    pCopy.hold = hold;
		    pCopy.masterEdition = masterEdition;
		    pCopy.masterPageID = masterPageID;
		    pCopy.miscint = miscint;
		    pCopy.miscstring1 = miscstring1;
		    pCopy.miscstring2 = miscstring2;
		    pCopy.pageID = pageID;
		    pCopy.pageIndex = pageIndex;
		    pCopy.pageName = pageName;
		    pCopy.pageType = pageType;
		    pCopy.pagination = pagination;
		    pCopy.priority = priority;
		    pCopy.uniquePage = uniquePage; 
		    pCopy.version = version;
		    pCopy.colorList.Clear();

            for(int color=0; color<pCopy.colorList.Count; color++)
            {
			    PlanDataPageSeparation org = colorList[color];
			    PlanDataPageSeparation sep = new PlanDataPageSeparation(org.colorName);  
			    pCopy.colorList.Add(sep);
		    } 

		    return true;
	    }
    }

    public class PlanDataSection
    {
	    public string sectionName;	
	    public List<PlanDataPage> pageList;
	    public int pagesInSection;

        public PlanDataSection()
	    {
		    sectionName = "";
		    pagesInSection = 0;

		    pageList = new List<PlanDataPage>();
	    }

        public PlanDataSection(string sSectionName)
	    {
		    sectionName = sSectionName;
		    pagesInSection = 0;

		    pageList = new List<PlanDataPage>();
	    }

        public PlanDataPage GetPageObject(string pageName)
	    {
		    for(int i=0; i<pageList.Count; i++) {
			    PlanDataPage p = pageList[i];
			    if (pageName == p.pageName)
				    return p;
		    }

		    return null;
	    }


        public PlanDataPage GetPageObject(int pageIndex)
	    {
		    for(int i=0; i<pageList.Count; i++) {
			    PlanDataPage p = pageList[i];
			    if (pageIndex == p.pageIndex)
				    return p;
		    }

		    return null;
	    }

        public int GetPageIndexOffset()
        {
            int offset = 9999;
            foreach (PlanDataPage page in pageList)
            {
                if (page.pageIndex < offset)
                    offset = page.pageIndex;
            }

            return offset - 1;
        }

    }

    public class PlanDataPress
    {
	    public string	pressName;
	    public int		    copies;
	    public DateTime	pressRunTime;
	    public int		paperCopies;
	    public string   postalUrl;

        public PlanDataPress() {
		    pressName = "";
		    copies = 1;
		    pressRunTime = DateTime.MinValue;
		    paperCopies = 0;
		    postalUrl = "";

	    }

	    public PlanDataPress(string  spressName) {
            pressName = spressName;
		    copies = 1;
		    pressRunTime = DateTime.MinValue;
		    paperCopies = 0;
   		    postalUrl = "";
    	}
    }

    public class PlanDataSheetPressCylinder
    {

        public string pressCylinder;
        public string colorName;
        public string formID;
        public string plateID;
        public string name;
        public string sortingPosition;

        public PlanDataSheetPressCylinder()
        {
            name = "";
            pressCylinder = "";
            colorName = "K";
            formID = "";
            plateID = "";
            sortingPosition = "";
        }

        PlanDataSheetPressCylinder(string colorNameIn)
        {
            name = "";
            pressCylinder = "";
            colorName = colorNameIn;
            formID = "";
            plateID = "";
            sortingPosition = "";
        }
    };
    public class PlanDataSheetItem
    {
        public int pagePositionX;
        public int pagePositionY;
        // CString pageName;
        public string pageID;
        public string masterPageID;

        public PlanDataSheetItem()
        {
            pagePositionX = 1;
            pagePositionY = 1;
            pageID = "";
            masterPageID = "";
        }
    };
    public class PlanDataSheetSide
    {
        public string sortingPosition;
        public string pressTower;
        public string pressZone;
        public string pressHighLow;
        public int activeCopies;

        public List<PlanDataSheetItem> sheetItems;
        public List<PlanDataSheetPressCylinder> pressCylinders;

        public PlanDataSheetSide()
        {
            sortingPosition = "";
            pressTower = "";
            pressZone = "";
            pressHighLow = "";
            activeCopies = 1;

            sheetItems = new List<PlanDataSheetItem>();
            pressCylinders = new List<PlanDataSheetPressCylinder>();
        }
    };
    public class PlanDataSheet
    {
        public string sheetName;
        public string templateName;
        public int pagesOnPlate;
        public string markGroups;

        public bool hasback;
        public PlanDataSheetSide frontSheet;
        public PlanDataSheetSide backSheet;

        public int pressSectionNumber;
        public PlanDataSheet()
        {
            sheetName = "";
            templateName = "";
            pagesOnPlate = 1;
            markGroups = "";
            hasback = true;

            frontSheet = new PlanDataSheetSide();
            backSheet = new PlanDataSheetSide();
            pressSectionNumber = 1;

        }
    };
    public class PlanDataEdition
    {

        public string editionName;

        public List<PlanDataPress> pressList;
        public List<PlanDataSection> sectionList;
        public List<PlanDataSheet> sheetList;

        public bool masterEdition;
        public int editionCopy;
        public int editionSequenceNumber;
        public string editionComment;


        public string timedFrom;
        public string timedTo;
        public bool IsTimed;
        public string zoneMaster;
        public string locationMaster;

        public PlanDataEdition()
        {
            editionName = "";
            editionCopy = 0;
            masterEdition = false;
            pressList = new List<PlanDataPress>();
            sectionList = new List<PlanDataSection>();
            sheetList = new List<PlanDataSheet>();
            editionSequenceNumber = 1;
            editionComment = "";
            IsTimed = false;
            timedTo = "";
            timedFrom = "";
            zoneMaster = "";
            locationMaster = "";
        }


        public PlanDataEdition(string seditionName)
        {
            editionName = seditionName;
            editionCopy = 0;
            masterEdition = false;
            pressList = new List<PlanDataPress>();
            sectionList = new List<PlanDataSection>();
            sheetList = new List<PlanDataSheet>();
            editionSequenceNumber = 1;
            editionComment = "";
            IsTimed = false;
            timedTo = "";
            timedFrom = "";
            zoneMaster = "";
            locationMaster = "";

        }


        public PlanDataPress GetPressObject(string pressName)
        {
            for (int i = 0; i < pressList.Count; i++)
            {
                PlanDataPress p = pressList[i];
                if (pressName == pressList[i].pressName)
                    return p;
            }

            return null;
        }

        public bool IsLastPress(string pressName)
        {
            return pressList[pressList.Count - 1].pressName == pressName;
        }

        public PlanDataSection GetSectionObject(string sectionName)
        {
            for (int i = 0; i < sectionList.Count; i++)
            {
                PlanDataSection p = sectionList[i];
                if (sectionName == p.sectionName)
                    return p;
            }

            return null;
        }

        public bool CloneEdition(ref PlanDataEdition pCopy)
        {
            if (pCopy == null)
                return false;
            pCopy.editionName = editionName;
            pCopy.editionCopy = editionCopy;
            pCopy.masterEdition = masterEdition;

            pCopy.timedFrom = timedFrom;
            pCopy.timedTo = timedTo;
            pCopy.IsTimed = IsTimed;
            pCopy.zoneMaster = zoneMaster;
            pCopy.editionComment = editionComment;

            for (int i = 0; i < pressList.Count; i++)
            {
                PlanDataPress pPress = pressList[i];
                PlanDataPress pPressCopy = new PlanDataPress();
                pPressCopy.copies = pPress.copies;
                pPressCopy.paperCopies = pPress.paperCopies;
                pPressCopy.postalUrl = pPress.postalUrl;
                pPressCopy.pressName = pPress.pressName;
                pPressCopy.pressRunTime = pPress.pressRunTime;
                pCopy.pressList.Add(pPressCopy);
            }

            return true;
        }
    }

    public class Pageplan
    {
        public string sXmlFile;
        public string planName;
        public string planID;
        public string publicationName;
        public string publicationAlias;
        public DateTime publicationDate;
        public int weekNumber;
        public int version;
        //int state;
        public string pagenameprefix;
        public DateTime updatetime;
        public DateTime deadline;

        public string sender;
        public string  defaultColors;
        public string  customername;
        public string  customeralias;
        public List<string> arrSectionNames;
        public List<string> arrSectionColors;

        public List<PlanDataEdition> editionList;
        

        public Pageplan()
        {
            arrSectionNames = new List<string>();   
            arrSectionColors = new List<string>();   
            editionList = new List<PlanDataEdition>(); 
            weekNumber = 0;
            version = 1;
            sender = "WebCenter";
        }

        public PlanDataEdition GetEditionObject(string edition)
        {
            foreach (PlanDataEdition ed in editionList)
            {
                if (ed.editionName == edition)
                    return ed;
            }

            return null;
        }

        private string GetTimeStampString(DateTime dt)
        {
            if (dt.Year < 2000)
                return "1975-01-01T00:00:00";
            else
                return string.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}", dt.Year,dt.Month,dt.Day,dt.Hour,dt.Minute,dt.Second);

        }

        private string GetDateString(DateTime dt)
        {
            return string.Format("{0:0000}-{1:00}-{2:00}", dt.Year, dt.Month, dt.Day);
        }

        private string GetPageTypeString(PageType pageType)
        {
            switch (pageType) 
            {
                case PageType.Dummy:
                    return "Dummy";
                case PageType.Panorama:
                    return "Panorama";
                case PageType.AntiPanorama:
                    return "AntiPanorama";
                default:
                    return "Normal";
            }
        }

        private string GetPageUniqueTypeString(PageUniqueType pageUniqueType)
        {
            switch (pageUniqueType) 
            {
                case PageUniqueType.Common:
                    return "Common";
                case PageUniqueType.Forced:
                    return "Forced";
                default:
                    return "Unique";
            }
        }


        public bool GenerateXML(string pressName, bool forcePDF, bool isLast, bool forceunapplied, out string errmsg)
        {
            errmsg = "";
            if (editionList.Count < 1)
            {
                errmsg = "No editions in plan";
                return false;
            }

            foreach (PlanDataEdition ed in editionList)
            {
                PlanDataPress press = ed.GetPressObject(pressName);
                if (press == null)
                {
                    errmsg = "Unknown press " + pressName;
                    return false;
                }
            }

            try
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings()
                {
                    Indent = true,
                    IndentChars = "\t",
                    NewLineOnAttributes = false,
                    Encoding = Encoding.UTF8
                };

                string outputFile = Global.sRealPlanFolder + "\\" + sXmlFile;
                if ((string)HttpContext.Current.Application["PressTemplateOutputFolder"] != "")
                    outputFile = (string)HttpContext.Current.Application["PressTemplateOutputFolder"] + "\\" + sXmlFile;

                XmlWriter writer = XmlWriter.Create(outputFile, xmlWriterSettings);
                Global.logging.WriteLog("XML plan name: " + outputFile);
                //  writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                writer.WriteStartDocument();

                writer.WriteStartElement("Plan");
                writer.WriteAttributeString("Version", version.ToString());
                writer.WriteAttributeString("Command", forceunapplied ? "ForceUnapplied" : "AddPlan");

                if (isLast == false)
                    writer.WriteAttributeString("IgnorePostCommand", "1");
                writer.WriteAttributeString("ID", planID);
                writer.WriteAttributeString("Name", planName);
                writer.WriteAttributeString("UpdateTime", GetTimeStampString(DateTime.Now));
                writer.WriteAttributeString("Sender", sender);
                // writer.WriteAttributeString("xmlns", "http://tempuri.org/ImportCenter.xsd");

                writer.WriteStartElement("Publication");
                writer.WriteAttributeString("PubDate", GetDateString(publicationDate));
                writer.WriteAttributeString("WeekReference", weekNumber.ToString());

                writer.WriteAttributeString("Name", publicationName);
                writer.WriteAttributeString("Alias", publicationAlias);

                writer.WriteAttributeString("Customer", customername);
                writer.WriteAttributeString("CustomerAlias", customeralias);

                writer.WriteStartElement("Issues");
                writer.WriteStartElement("Issue");
                writer.WriteAttributeString("Name", "Main");

                writer.WriteStartElement("Editions");

                for (int eds = 0; eds < editionList.Count(); eds++)
                {
                    PlanDataEdition planDataEdition = editionList[eds];

                    writer.WriteStartElement("Edition");
                    writer.WriteAttributeString("Name", planDataEdition.editionName);
                    writer.WriteAttributeString("EditionCopies", planDataEdition.editionCopy.ToString());
                    writer.WriteAttributeString("IsTimed", planDataEdition.IsTimed ? "true" : "false");
                    writer.WriteAttributeString("TimedFrom", planDataEdition.timedFrom);
                    writer.WriteAttributeString("TimedTo", planDataEdition.timedTo);
                    writer.WriteAttributeString("EditionOrderNumber", planDataEdition.editionSequenceNumber.ToString());
                    writer.WriteAttributeString("EditionComment", planDataEdition.editionComment);
                    writer.WriteAttributeString("ZoneMasterEdition", planDataEdition.masterEdition ? "" : planDataEdition.zoneMaster);

                    writer.WriteStartElement("IntendedPresses");
                    PlanDataPress planDataPress = planDataEdition.GetPressObject(pressName);
                    if (planDataPress == null)
                    {
                        errmsg = "Unknown press " + pressName;
                        return false;
                    }
                    writer.WriteStartElement("IntendedPress");
                    writer.WriteAttributeString("Name", planDataPress.pressName);
                    writer.WriteAttributeString("PressTime", GetTimeStampString(planDataPress.pressRunTime));
                    writer.WriteAttributeString("PlateCopies", planDataPress.copies.ToString());
                    writer.WriteAttributeString("Copies", planDataPress.paperCopies.ToString());
                    writer.WriteAttributeString("PostalUrl", planDataPress.postalUrl);
                    writer.WriteEndElement();  //IntendedPress
                    writer.WriteEndElement();  //IntendedPresses

                    writer.WriteStartElement("Sections");

                    for (int section = 0; section < planDataEdition.sectionList.Count; section++)
                    {
                        PlanDataSection planDataSection = planDataEdition.sectionList[section];
                        writer.WriteStartElement("Section");
                        writer.WriteAttributeString("Name", planDataSection.sectionName);

                        writer.WriteStartElement("Pages");

                        for (int page = 0; page < planDataSection.pageList.Count; page++)
                        {
                            PlanDataPage planDataPage = planDataSection.pageList[page];
                            writer.WriteStartElement("Page");
                            writer.WriteAttributeString("Name", planDataPage.pageName);
                            writer.WriteAttributeString("Pagination", planDataPage.pagination.ToString());
                            writer.WriteAttributeString("PageType", GetPageTypeString(planDataPage.pageType));

                            PageUniqueType unique = planDataPage.uniquePage;
                            if (planDataPage.pageID == planDataPage.masterPageID)
                                unique = PageUniqueType.Unique;

                            writer.WriteAttributeString("Unique", GetPageUniqueTypeString(unique));
                            writer.WriteAttributeString("PageIndex", planDataPage.pageIndex.ToString());
                            writer.WriteAttributeString("FileName", planDataPage.fileName);
                            writer.WriteAttributeString("Comment", planDataPage.comment);
                            writer.WriteAttributeString("Approve", planDataPage.approve ? "1" : "0");
                            writer.WriteAttributeString("Hold", planDataPage.hold ? "1" : "0");
                            writer.WriteAttributeString("MiscInt", planDataPage.miscint.ToString());
                            writer.WriteAttributeString("MiscString1", planDataPage.miscstring1);
                            writer.WriteAttributeString("MiscString2", planDataPage.miscstring2);
                            writer.WriteAttributeString("PageID", planDataPage.pageID);
                            writer.WriteAttributeString("MasterPageID", planDataPage.masterPageID != "" ? planDataPage.masterPageID : planDataPage.pageID);
                            writer.WriteAttributeString("Priority", planDataPage.priority.ToString());
                            writer.WriteAttributeString("Version", planDataPage.version.ToString());
                            writer.WriteAttributeString("PageFormat", planDataPage.pageFormat);
                            

                            writer.WriteStartElement("Separations");

                            if (forcePDF)
                            {
                                writer.WriteStartElement("Separation");
                                writer.WriteAttributeString("Name", "PDF");
                                writer.WriteEndElement();  //Separation
                            }
                            else
                            {
                                for (int col = 0; col < planDataPage.colorList.Count; col++)
                                {
                                    writer.WriteStartElement("Separation");
                                    writer.WriteAttributeString("Name", planDataPage.colorList[col].colorName);
                                    writer.WriteEndElement();  //Separation
                                } // for (int col..
                            }
                            writer.WriteEndElement();  //Separations
                            writer.WriteEndElement();  //Page
                        } // for (int page..
                        writer.WriteEndElement();  //Pages
                        writer.WriteEndElement();  //Section
                    } // for (int section..

                    writer.WriteEndElement();// Sections

                    if (planDataEdition.sheetList.Count > 0)
                    {
                        writer.WriteStartElement("Sheets");
                        foreach (PlanDataSheet sheet in planDataEdition.sheetList)
                        {
                            writer.WriteStartElement("Sheet");
                            writer.WriteAttributeString("Name", sheet.sheetName);
                            writer.WriteAttributeString("Template", sheet.templateName);
                            writer.WriteAttributeString("MarkGroups", sheet.markGroups);
                            writer.WriteAttributeString("PagesOnPlate", sheet.pagesOnPlate.ToString());
                            writer.WriteAttributeString("PressSectionNumber", sheet.pressSectionNumber.ToString());

                            // Front sheet

                            writer.WriteStartElement("SheetFrontItems");
                            writer.WriteAttributeString("SortingPosition", sheet.frontSheet.sortingPosition);
                            writer.WriteAttributeString("PressHighLow", sheet.frontSheet.pressHighLow);
                            writer.WriteAttributeString("PressTower", sheet.frontSheet.pressTower);
                            writer.WriteAttributeString("PressZone", sheet.frontSheet.pressZone);
                            writer.WriteAttributeString("ActiveCopies", planDataPress.copies.ToString());
                            foreach (PlanDataSheetItem sheetItem in sheet.frontSheet.sheetItems)
                            {
                                writer.WriteStartElement("SheetFrontItem");
                                writer.WriteAttributeString("PageID", sheetItem.pageID);
                                writer.WriteAttributeString("MasterPageID", sheetItem.masterPageID);
                                writer.WriteAttributeString("PosX", sheetItem.pagePositionX.ToString());
                                writer.WriteAttributeString("PosY", sheetItem.pagePositionY.ToString());
                                writer.WriteEndElement();// SheetFrontItem
                            }

                            writer.WriteStartElement("PressCylindersFront");
                            foreach (PlanDataSheetPressCylinder cylinder in sheet.frontSheet.pressCylinders)
                            {
                                writer.WriteStartElement("PressCylinderFront");
                                writer.WriteAttributeString("Color", cylinder.colorName);
                                writer.WriteAttributeString("Name", cylinder.name);
                                writer.WriteAttributeString("FormID", cylinder.formID);
                                writer.WriteAttributeString("PlateID", cylinder.plateID);
                                writer.WriteAttributeString("SortingPosition", cylinder.sortingPosition);

                                writer.WriteEndElement();// PressCylinderFront
                            }
                            writer.WriteEndElement();// PressCylindersFront

                            writer.WriteEndElement();// SheetFrontItems

                            // Back sheet

                            if (sheet.hasback && sheet.backSheet != null)
                            {
                                writer.WriteStartElement("SheetBackItems");
                                writer.WriteAttributeString("SortingPosition", sheet.backSheet.sortingPosition);
                                writer.WriteAttributeString("PressHighLow", sheet.backSheet.pressHighLow);
                                writer.WriteAttributeString("PressTower", sheet.backSheet.pressTower);
                                writer.WriteAttributeString("PressZone", sheet.backSheet.pressZone);
                                writer.WriteAttributeString("ActiveCopies", planDataPress.copies.ToString());

                                foreach (PlanDataSheetItem sheetItem in sheet.backSheet.sheetItems)
                                {
                                    writer.WriteStartElement("SheetBackItem");
                                    writer.WriteAttributeString("PageID", sheetItem.pageID);
                                    writer.WriteAttributeString("MasterPageID", sheetItem.masterPageID);
                                    writer.WriteAttributeString("PosX", sheetItem.pagePositionX.ToString());
                                    writer.WriteAttributeString("PosY", sheetItem.pagePositionY.ToString());
                                    writer.WriteEndElement();// SheetBackItem
                                }

                                writer.WriteStartElement("PressCylindersBack");
                                foreach (PlanDataSheetPressCylinder cylinder in sheet.backSheet.pressCylinders)
                                {
                                    writer.WriteStartElement("PressCylinderBack");
                                    writer.WriteAttributeString("Color", cylinder.colorName);
                                    writer.WriteAttributeString("Name", cylinder.name);
                                    writer.WriteAttributeString("FormID", cylinder.formID);
                                    writer.WriteAttributeString("PlateID", cylinder.plateID);
                                    writer.WriteAttributeString("SortingPosition", cylinder.sortingPosition);

                                    writer.WriteEndElement();// PressCylinderBack
                                }
                                writer.WriteEndElement();// PressCylindersBack

                                writer.WriteEndElement();// SheetBackItems
                            }
                            writer.WriteEndElement();// Sheet
                        }
                        writer.WriteEndElement();// Sheets

                    } //   if (planDataEdition.sheetList.Count > 0)..


                    writer.WriteEndElement();// </Edition>
                } // for (int eds..

                writer.WriteEndElement();// </Editions>
                writer.WriteEndElement();// </Issue>
                writer.WriteEndElement();// </Issues>
                writer.WriteEndElement();// </Publication>
                writer.WriteEndElement();// </Plan>
                writer.Close();
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return false;
            }
            return true;

        }

        public int GenerateSheets(string editionName, TemplateInfo templateInfo, bool forcePDF, int pressSectionNumber, string pressEditionName)
        {
            int nUP = templateInfo.nUP;
            PlanDataEdition edition = GetEditionObject(editionName);
            if (edition == null)
                return 0;

            edition.sheetList.Clear();



            int sheetNumber = 1;
            switch (nUP)
            {
                case 1:

                    foreach (PlanDataSection section in edition.sectionList)
                    {
                        for (int pg = 0; pg < section.pageList.Count; pg += 2)
                        {
                            PlanDataPage pagefront = section.pageList[pg];
                            PlanDataPage pageback = section.pageList[pg + 1];

                            PlanDataSheet sheet = new PlanDataSheet();
                            sheet.sheetName = sheetNumber.ToString();
                            sheet.pagesOnPlate = 1;
                            sheet.templateName = templateInfo.templateName;
                            sheet.markGroups = templateInfo.markGroups;
                            sheet.hasback = true;
                            sheet.pressSectionNumber = pressSectionNumber;
                            edition.sheetList.Add(sheet);


                            sheet.frontSheet = new PlanDataSheetSide();
                            sheet.frontSheet.activeCopies = templateInfo.plateCopies;
                            if (forcePDF)
                            {
                                sheet.frontSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "PDF", name = "PDF", formID = pressEditionName });
                            }
                            else
                            {
                                sheet.frontSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "C", name = "C" });
                                sheet.frontSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "M", name = "M" });
                                sheet.frontSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "Y", name = "Y" });
                                sheet.frontSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "K", name = "K" });
                            }
                            sheet.frontSheet.sheetItems.Add(new PlanDataSheetItem { pageID = pagefront.pageID, masterPageID = pagefront.masterPageID, pagePositionX = 1, pagePositionY = 1 });

                            sheet.backSheet = new PlanDataSheetSide();
                            sheet.backSheet.activeCopies = templateInfo.plateCopies;
                            if (forcePDF)
                            {
                                sheet.frontSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "PDF", name = "PDF" });
                            }
                            else
                            {
                                sheet.backSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "C", name = "C", formID = pressEditionName });
                                sheet.backSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "M", name = "M", formID = pressEditionName });
                                sheet.backSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "Y", name = "Y", formID = pressEditionName });
                                sheet.backSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "K", name = "K", formID = pressEditionName });
                            }
                            sheet.backSheet.sheetItems.Add(new PlanDataSheetItem { pageID = pageback.pageID, masterPageID = pageback.masterPageID, pagePositionX = 1, pagePositionY = 1 });
                        }
                    }

                    break;
                case 2:
                    foreach (PlanDataSection section in edition.sectionList)
                    {
                        int pageIndexOffset = section.GetPageIndexOffset(); // zero-based

                        int numberOfSheets = section.pageList.Count / 4;

                        for (int pg = 0; pg < numberOfSheets; pg++)
                        {
                            PlanDataSheet sheet = new PlanDataSheet();
                            sheet.sheetName = sheetNumber.ToString();
                            sheet.pagesOnPlate = 2;
                            sheet.templateName = templateInfo.templateName;
                            sheet.markGroups = templateInfo.markGroups;
                            sheet.hasback = true;
                            sheet.pressSectionNumber = pressSectionNumber;
                            edition.sheetList.Add(sheet);

                            PlanDataPage pagefront1 = section.pageList[2 * pg];
                            int matepageIndex = (section.pageList.Count - (pagefront1.pageIndex - pageIndexOffset)) + 1 + pageIndexOffset;
                            PlanDataPage pagefront2 = section.GetPageObject(matepageIndex);

                            PlanDataPage pageback1 = section.pageList[2 * pg + 1];
                            matepageIndex = (section.pageList.Count - (pageback1.pageIndex - pageIndexOffset)) + 1 + pageIndexOffset;
                            PlanDataPage pageback2 = section.GetPageObject(matepageIndex);

                            sheet.frontSheet = new PlanDataSheetSide();
                            sheet.frontSheet.activeCopies = templateInfo.plateCopies;
                            if (forcePDF)
                            {
                                sheet.frontSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "PDF", name = "PDF", formID = pressEditionName });
                            }
                            else
                            {
                                sheet.frontSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "C", name = "C", formID = pressEditionName });
                                sheet.frontSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "M", name = "M", formID = pressEditionName });
                                sheet.frontSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "Y", name = "Y", formID = pressEditionName });
                                sheet.frontSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "K", name = "K", formID = pressEditionName });
                            }
                            sheet.frontSheet.sheetItems.Add(new PlanDataSheetItem { pageID = pagefront1.pageID, masterPageID = pagefront1.masterPageID, pagePositionX = 2, pagePositionY = 1 });
                            if (pagefront2 != null)
                                sheet.frontSheet.sheetItems.Add(new PlanDataSheetItem { pageID = pagefront2.pageID, masterPageID = pagefront2.masterPageID, pagePositionX = 1, pagePositionY = 1 });
                            else
                                sheet.frontSheet.sheetItems.Add(new PlanDataSheetItem { pageID = "Dummy", masterPageID = "Dummy", pagePositionX = 1, pagePositionY = 1 });

                            sheet.backSheet = new PlanDataSheetSide();
                            sheet.backSheet.activeCopies = templateInfo.plateCopies; 
                            if (forcePDF)
                            {
                                sheet.frontSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "PDF", name = "PDF" });
                            }
                            else
                            {
                                sheet.backSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "C", name = "C", formID = pressEditionName });
                                sheet.backSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "M", name = "M", formID = pressEditionName });
                                sheet.backSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "Y", name = "Y", formID = pressEditionName });
                                sheet.backSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "K", name = "K", formID = pressEditionName });
                            }
                            sheet.backSheet.sheetItems.Add(new PlanDataSheetItem { pageID = pageback1.pageID, masterPageID = pageback1.masterPageID, pagePositionX = 2, pagePositionY = 1 });
                            if (pageback2 != null)
                                sheet.backSheet.sheetItems.Add(new PlanDataSheetItem { pageID = pageback2.pageID, masterPageID = pageback2.masterPageID, pagePositionX = 1, pagePositionY = 1 });
                            else
                                sheet.backSheet.sheetItems.Add(new PlanDataSheetItem { pageID = "Dummy", masterPageID = "Dummy", pagePositionX = 1, pagePositionY = 1 });

                            sheetNumber++;
                        }

                        // halfweb sheet?
                        if (section.pageList.Count > numberOfSheets * 4)
                        {
                            PlanDataSheet sheet = new PlanDataSheet();
                            sheet.sheetName = sheetNumber.ToString();
                            sheet.pagesOnPlate = 2;
                            sheet.templateName = templateInfo.templateName;
                            sheet.markGroups = templateInfo.markGroups;
                            sheet.hasback = true;
                            sheet.pressSectionNumber = pressSectionNumber;

                            PlanDataPage pagefront1 = section.pageList[2 * numberOfSheets];
                            PlanDataPage pageback1 = section.pageList[section.pageList.Count - 2 * numberOfSheets - 1];
                            sheet.frontSheet = new PlanDataSheetSide();
                            sheet.frontSheet.activeCopies = templateInfo.plateCopies;
                            sheet.frontSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "C", name = "C", formID = pressEditionName });
                            sheet.frontSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "M", name = "M", formID = pressEditionName });
                            sheet.frontSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "Y", name = "Y", formID = pressEditionName });
                            sheet.frontSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "K", name = "K", formID = pressEditionName });
                            sheet.frontSheet.sheetItems.Add(new PlanDataSheetItem { pageID = pagefront1.pageID, masterPageID = pagefront1.masterPageID, pagePositionX = 2, pagePositionY = 1 });
                            sheet.frontSheet.sheetItems.Add(new PlanDataSheetItem { pageID = "Dummy", masterPageID = "Dummy", pagePositionX = 1, pagePositionY = 1 });

                            sheet.backSheet = new PlanDataSheetSide();
                            sheet.backSheet.activeCopies = templateInfo.plateCopies;
                            sheet.backSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "C", name = "C", formID = pressEditionName });
                            sheet.backSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "M", name = "M", formID = pressEditionName });
                            sheet.backSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "Y", name = "Y", formID = pressEditionName });
                            sheet.backSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "K", name = "K", formID = pressEditionName });
                            sheet.backSheet.sheetItems.Add(new PlanDataSheetItem { pageID = pageback1.pageID, masterPageID = pageback1.masterPageID, pagePositionX = 1, pagePositionY = 1 });
                            sheet.backSheet.sheetItems.Add(new PlanDataSheetItem { pageID = "Dummy", masterPageID = "Dummy", pagePositionX = 2, pagePositionY = 1 });

                        }
                    }
                    break;
               

                case 4:
                    foreach (PlanDataSection section in edition.sectionList)
                    {
                        int pageIndexOffset = section.GetPageIndexOffset(); // zero-based

                        int numberOfSheets = (section.pageList.Count + 4) / 8; // will include halfweb sheet

                        
                        for (int sh = 0; sh < numberOfSheets; sh++)
                        //for (int sh = numberOfSheets - 1; sh >= 0; sh--)
                        {
                            int pg1 = 1, pg2 = 1, pg3 = 1, pg4 = 1;
                            PlanDataSheet sheet = new PlanDataSheet();
                            sheet.sheetName = sheetNumber.ToString();
                            sheet.pagesOnPlate = 4;
                            sheet.templateName = templateInfo.templateName;
                            sheet.markGroups = templateInfo.markGroups;
                            sheet.hasback = true;
                            sheet.pressSectionNumber = pressSectionNumber;
                            edition.sheetList.Add(sheet);

                            Fixed4upSheet(templateInfo, section.pageList.Count, sh + 1, false, ref pg1, ref pg2, ref pg3, ref pg4);

                            PlanDataPage pagefront1 = pg1 > 0 ? section.pageList[pg1 - 1] : null;
                            PlanDataPage pagefront2 = pg2 > 0 ? section.pageList[pg2 - 1] : null;
                            PlanDataPage pagefront3 = pg3 > 0 ? section.pageList[pg3 - 1] : null;
                            PlanDataPage pagefront4 = pg4 > 0 ? section.pageList[pg4 - 1] : null;

                            Fixed4upSheet(templateInfo, section.pageList.Count, sh + 1, true, ref pg1, ref pg2, ref pg3, ref pg4);

                            PlanDataPage pageback1 = pg1 > 0 ? section.pageList[pg1 - 1] : null;
                            PlanDataPage pageback2 = pg2 > 0 ? section.pageList[pg2 - 1] : null;
                            PlanDataPage pageback3 = pg3 > 0 ? section.pageList[pg3 - 1] : null;
                            PlanDataPage pageback4 = pg4 > 0 ? section.pageList[pg4 - 1] : null;

                            sheet.frontSheet = new PlanDataSheetSide();
                            sheet.frontSheet.activeCopies = templateInfo.plateCopies;
                            if (forcePDF)
                            {
                                sheet.frontSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "PDF", name = "PDF", formID = pressEditionName });
                            }
                            else
                            {
                                sheet.frontSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "C", name = "C", formID = pressEditionName });
                                sheet.frontSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "M", name = "M", formID = pressEditionName });
                                sheet.frontSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "Y", name = "Y", formID = pressEditionName });
                                sheet.frontSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "K", name = "K", formID = pressEditionName });
                            }

                            sheet.frontSheet.sheetItems.Add(new PlanDataSheetItem { pageID = pagefront1 != null ? pagefront1.pageID : "Dummy", masterPageID = pagefront1 != null ? pagefront1.masterPageID : "Dummy", pagePositionX = 1, pagePositionY = 1 }); // 1
                            sheet.frontSheet.sheetItems.Add(new PlanDataSheetItem { pageID = pagefront2 != null ? pagefront2.pageID : "Dummy", masterPageID = pagefront2 != null ? pagefront2.masterPageID : "Dummy", pagePositionX = 2, pagePositionY = 1 }); // 8
                            sheet.frontSheet.sheetItems.Add(new PlanDataSheetItem { pageID = pagefront3 != null ? pagefront3.pageID : "Dummy", masterPageID = pagefront3 != null ? pagefront3.masterPageID : "Dummy", pagePositionX = 1, pagePositionY = 2 }); // 5
                            sheet.frontSheet.sheetItems.Add(new PlanDataSheetItem { pageID = pagefront4 != null ? pagefront4.pageID : "Dummy", masterPageID = pagefront4 != null ? pagefront4.masterPageID : "Dummy", pagePositionX = 2, pagePositionY = 2 }); // 4

                            sheet.backSheet = new PlanDataSheetSide();
                            sheet.backSheet.activeCopies = templateInfo.plateCopies;
                            if (forcePDF)
                            {
                                sheet.backSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "PDF", name = "PDF" });
                            }
                            else
                            {
                                sheet.backSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "C", name = "C", formID = pressEditionName });
                                sheet.backSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "M", name = "M", formID = pressEditionName });
                                sheet.backSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "Y", name = "Y", formID = pressEditionName });
                                sheet.backSheet.pressCylinders.Add(new PlanDataSheetPressCylinder { colorName = "K", name = "K", formID = pressEditionName });
                            }

                            sheet.backSheet.sheetItems.Add(new PlanDataSheetItem { pageID = pageback1 != null ? pageback1.pageID : "Dummy", masterPageID = pageback1 != null ? pageback1.masterPageID : "Dummy", pagePositionX = 1, pagePositionY = 1 }); // 2
                            sheet.backSheet.sheetItems.Add(new PlanDataSheetItem { pageID = pageback2 != null ? pageback2.pageID : "Dummy", masterPageID = pageback2 != null ? pageback2.masterPageID : "Dummy", pagePositionX = 2, pagePositionY = 1 }); // 7
                            sheet.backSheet.sheetItems.Add(new PlanDataSheetItem { pageID = pageback3 != null ? pageback3.pageID : "Dummy", masterPageID = pageback3 != null ? pageback3.masterPageID : "Dummy", pagePositionX = 1, pagePositionY = 2 }); // 6
                            sheet.backSheet.sheetItems.Add(new PlanDataSheetItem { pageID = pageback4 != null ? pageback4.pageID : "Dummy", masterPageID = pageback4 != null ? pageback4.masterPageID : "Dummy", pagePositionX = 2, pagePositionY = 2 }); // 3

                            sheetNumber++;
                        }
                    }
                    break;
            }
            return 1;

        }
        private void Fixed4upSheet(TemplateInfo templateInfo, int totalPages, int sheetNumber, bool backSide, ref int pg1, ref int pg2, ref int pg3, ref int pg4)
        {
            pg1 = 0;
            pg2 = 0;
            pg3 = 0;
            pg4 = 0;

            if (templateInfo == null)
                return;

            int flatNumber = sheetNumber * 2 - 1;
            if (backSide)
                ++flatNumber;

            string[] frontPages = templateInfo.frontPageList.Split(',');
            string[] backPages = templateInfo.backPageList.Split(',');
            string[] frontPagesHalfWeb = templateInfo.frontPageListHalfWeb.Split(',');
            string[] backPagesHalfWeb = templateInfo.backPageListHalfWeb.Split(',');

            bool hasHalfweb = (totalPages % 8) > 0;
            bool lastsheet = sheetNumber * 8 >= totalPages - 3;

            int[] pgOffset = new int[4];

            for (int i = 0; i < 4; i++)
            {
                int n = backSide ? Globals.TryParse(backPages[i], 0) : Globals.TryParse(frontPages[i], 0);

                if (lastsheet && hasHalfweb)
                {
                    n = backSide ? Globals.TryParse(backPagesHalfWeb[i], 0) : Globals.TryParse(frontPagesHalfWeb[i], 0);
                    if (n == 0)
                    {
                        pgOffset[i] = 0;
                        continue;
                    }
                    if (n == 1 || n == 2)
                        pgOffset[i] = n + (sheetNumber - 1) * 2;  // count up
                    if (n == 3 || n == 4)
                        pgOffset[i] = totalPages + n - 4 - (sheetNumber - 1) * 2; // count down

                    continue;

                }

                // lowest!
                if (n == 1 || n == 2)
                    pgOffset[i] = n + (sheetNumber - 1) * 2;  // count up
                // low mid
                if (n == 3 || n == 4)
                    pgOffset[i] = totalPages / 2 + n - 4 - (sheetNumber - 1) * 2;   // count down
                // high mid
                if (n == 5 || n == 6)
                    pgOffset[i] = totalPages / 2 + n - 4 + (sheetNumber - 1) * 2;  // count up 
                if (n == 7 || n == 8)
                    pgOffset[i] = totalPages + n - 8 - (sheetNumber - 1) * 2; // count down
            }

            pg1 = pgOffset[0];
            pg2 = pgOffset[1];
            pg3 = pgOffset[2];
            pg4 = pgOffset[3];

        }


    }

}