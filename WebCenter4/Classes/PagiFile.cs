using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace WebCenter4.Classes
{
    public class PagiFile
    {
        public static readonly int PAGI_PUBDATE = 0;
        public static readonly int PAGI_EDITION = 1;
        public static readonly int PAGI_MASTEREDITION = 2;
        public static readonly int PAGI_PAGINATION = 3;
        public static readonly int PAGI_PAGEINDEX = 4;
        public static readonly int PAGI_PAGENAMECOMMENT = 5;
        public static readonly int PAGI_PAGENAME = 6;
        public static readonly int PAGI_SECTION = 7;
        public static readonly int PAGI_PRIORITY = 8;
        public static readonly int PAGI_UNIQUEPAGE = 9;
        public static readonly int PAGI_FILENAME = 10;
        public static readonly int PAGI_COLORS = 11;
        public static readonly int PAGI_PLATETEXT = 12;
        public static readonly int PAGI_PAGETYPE = 13;
        public static readonly int PAGI_PUBLICATION = 14;
        public static readonly int PAGI_LAYOUT = 15;
        public static readonly int PAGI_PECOMPUBLICATION = 16;    
        public static readonly int PAGI_PECOMEDITION = 17;


        public PagiPlanData plan = null;

        public  PagiFile()
        {
            plan = new PagiPlanData();
        }

        private DateTime String2DateTime(string datestr)
        {
            if (datestr.Length != 10)
                return DateTime.MinValue;
            try
            {
                return new DateTime(Int32.Parse(datestr.Substring(6, 4)), Int32.Parse(datestr.Substring(3, 2)), Int32.Parse(datestr.Substring(0, 2)));
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        public bool ParseFile(string pageFileName, ref int paginationstart)
        {
            paginationstart = 1;
            string [] lines;

            if (File.Exists(pageFileName) == false)
                return false;

            try
            {
                lines = File.ReadAllLines(pageFileName);
            }
            catch 
            {
                return false;
            }

            int pageoffset = -1;

            string thisEdition = "";
            string thisSection = "";
            int pageCount = 1;
            int pageIndex = 1;
            PagiPlanDataSection sec = null;
            PagiPlanDataEdition ed = null;
            foreach (string line in lines)
            {
                string[] elements = line.Split('\t');
                if (elements.Length < 15)
                    continue;
               
                if (elements[PAGI_EDITION] != thisEdition)
                {
                    ed = new PagiPlanDataEdition(elements[PAGI_EDITION]);
                    ed.masterEdition = (elements[PAGI_EDITION] == elements[PAGI_MASTEREDITION]);
                    ed.editionComment = ed.editionName;


                    plan.editionList.Add(ed);
                    thisEdition = ed.editionName;
                }

                string section = elements[PAGI_SECTION];
                if (section.IndexOf("SUP") < 0)
                    section = "Main";

                if (section != thisSection)
                {
                    sec = new PagiPlanDataSection(section);
                    sec.pagesInSection = 0;

                    ed.sectionList.Add(sec); 
                    pageCount = 1;
                    thisSection = section;
                }

                PagiPlanDataPage page = new PagiPlanDataPage(elements[PAGI_PAGENAME]);
                page.fileName = elements[PAGI_FILENAME];
                page.masterEdition = elements[PAGI_MASTEREDITION];
                page.pageIndex = Globals.TryParse(elements[PAGI_PAGEINDEX], pageCount);
                
                int pagination = Globals.TryParse(elements[PAGI_PAGINATION], pageCount++);
                if (pageoffset == -1)
                    pageoffset = pagination;

                // Adjust pagination for covers to start at 1 
                page.pagination = pagination - (pageoffset - 1);
                if (pagination > 1)
                    page.pageIndex = page.pagination;
                page.priority = Globals.TryParse(elements[PAGI_PRIORITY], 0);
                page.uniquePage = (elements[PAGI_UNIQUEPAGE] == "X") ? PageUniqueType.Unique : PageUniqueType.Common;
                page.pageType = (elements[PAGI_PAGETYPE] != "") ? PageType.Panorama : PageType.Normal;

                page.monoPage = elements[PAGI_COLORS] != "Q";
                page.comment = elements[PAGI_PAGENAMECOMMENT];
                page.miscstring1 = elements[PAGI_LAYOUT];
                page.miscstring2 = elements[PAGI_PLATETEXT];
                page.version = 0;

                page.pageID = pageIndex.ToString();
                page.masterPageID = pageIndex.ToString();
                pageIndex++;

                plan.publicationDate = String2DateTime(elements[PAGI_PUBDATE]);
                plan.publicationName = elements[PAGI_PUBLICATION];
                plan.publicationAlias = plan.publicationName;
            
                if (elements.Length > 16)
                    plan.publicationAlias = elements[PAGI_PECOMPUBLICATION];

                if (elements.Length > 17)
                    ed.editionComment = elements[PAGI_PECOMEDITION];

                sec.pageList.Add(page);

            }

            ResolveCommonPages();

            paginationstart = pageoffset > 0 ? pageoffset : 1;

            return true;
        }

        // resolve unique/common page relations..
        private void ResolveCommonPages()
        {
            foreach (PagiPlanDataEdition ed in plan.editionList)
            {
                foreach (PagiPlanDataSection sec in ed.sectionList)
                {
                    foreach (PagiPlanDataPage page in sec.pageList)
                    {
                        if (page.uniquePage == PageUniqueType.Common)
                        {
                            PagiPlanDataEdition mastered = plan.GetEditionObject(page.masterEdition);
                            if (mastered != null)
                            {
                                bool found = false;
                                foreach (PagiPlanDataSection mastersec in mastered.sectionList)
                                {
                                    foreach (PagiPlanDataPage masterpage in mastersec.pageList)
                                    {
                                        if (masterpage.pageName == page.pageName)
                                        {
                                            page.masterPageID = masterpage.pageID;
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (found)
                                        break;
                                }
                            }
                        }
                        else
                            page.masterPageID = page.pageID;

                    }
                }
            }
        }

        public string GetPagiSummary()
        {
            string sEditions = "";
            string sSections = "";


            foreach (PagiPlanDataEdition ed in plan.editionList)
            {
                if (sEditions != "")
                    sEditions += ", ";
                sEditions += "Edition: " + ed.editionName;
                sSections = "";
                foreach (PagiPlanDataSection sec in ed.sectionList)
                {
                    if (sSections != "")
                        sSections += ", ";
                    sSections += sec.sectionName + "(" + sec.pageList.Count + " pages)";
                }
                sEditions +=  "   Sections: " + sSections;
            }

            return sEditions;
        }

        public bool GeneratePagiFile( string pagiFile, string fileNamePrefix, int paginationstart)
        {
            List<string> lines = new List<string>();

            string atproductname = "";
            if (plan.publicationName.StartsWith("Autre"))
                atproductname = "AT" + plan.publicationName.Substring(5,1);

            foreach (PagiPlanDataEdition ed in plan.editionList)
            {
                foreach (PagiPlanDataSection sec in ed.sectionList)
                {
                    foreach (PagiPlanDataPage page in sec.pageList)
                    {
                        string fileName = page.fileName;
                        string pageName = page.pageName;

                        // Add pubdate to planpagename 
                         
                        // Change pagename if Autre-plan..
                        int n = pageName.IndexOf('-');
                        if (n != -1 && atproductname != "")
                        {
                            pageName = string.Format("{0}-{1}", atproductname, page.pagination + paginationstart - 1);
                        }

                        // Add pubdate to planpagename
                        n = fileName.IndexOf('_');
                        if (n >= 0)
                            fileName = string.Format("{0}_{1:00}{2:00}", pageName, plan.publicationDate.Day, plan.publicationDate.Month);


                        // Custom filename overrules all..
                        if (fileNamePrefix != "")
                        {
                            fileName = string.Format("{0}_{1:000}", fileNamePrefix, page.pagination + paginationstart - 1);
                            pageName = string.Format("{0}_{1:000}", fileNamePrefix, page.pagination + paginationstart - 1);
                        }

                        


                        lines.Add(string.Format("{0:00}/{1:00}/{2:0000}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\t{18}\t{19}",

                                 plan.publicationDate.Day,
                                 plan.publicationDate.Month,
                                 plan.publicationDate.Year,
                                 ed.editionName,
                                 page.masterEdition,
                                 page.pagination,
                                 page.pageIndex,
                                 page.comment,
                                 pageName,
                                 sec.sectionName,
                                 page.priority,
                                 page.uniquePage == PageUniqueType.Unique ? "X" : "",
                                 fileName,
                                 page.monoPage ? "N" : "Q",
                                 page.miscstring2,
                                 page.pageType == PageType.Panorama ? "PANO" : "",
                                 plan.publicationName,
                                 page.miscstring1,
                                 plan.publicationAlias,
                                 ed.editionComment));
                    }
                }
            }
            if (lines.Count == 0)
                return false;
            try
            {
                string s = "";
                foreach (string line in lines)
                    s += line + '\r';
                File.WriteAllText(pagiFile, s);

               // File.WriteAllLines(pagiFile, lines);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }


    public class PagiPlanDataPage
    {
        public string pageName;
        public string fileName;
        public string pageID;
        public PageType pageType;
        public int pagination;
        public int pageIndex;
        public string comment;
        public bool monoPage;
        public PageUniqueType uniquePage;
        public string masterPageID;

        public bool approve;
        public bool hold;
        public int priority;
        public int version;
        public string masterEdition;
        public string miscstring1;
        public string miscstring2;
        public int miscint;


        public PagiPlanDataPage()
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
            monoPage = false;
        }

        public PagiPlanDataPage(string sPageName)
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

            monoPage = false;
        }
    }

    public class PagiPlanDataSection
    {
        public string sectionName;
        public List<PagiPlanDataPage> pageList;
        public int pagesInSection;

        public PagiPlanDataSection()
        {
            sectionName = "";
            pagesInSection = 0;

            pageList = new List<PagiPlanDataPage>();
        }

        public PagiPlanDataSection(string sSectionName)
        {
            sectionName = sSectionName;
            pagesInSection = 0;

            pageList = new List<PagiPlanDataPage>();
        }

        public PagiPlanDataPage GetPageObject(string pageName)
        {
            for (int i = 0; i < pageList.Count; i++)
            {
                PagiPlanDataPage p = pageList[i];
                if (pageName == p.pageName)
                    return p;
            }

            return null;
        }


        public PagiPlanDataPage GetPageObject(int pageIndex)
        {
            for (int i = 0; i < pageList.Count; i++)
            {
                PagiPlanDataPage p = pageList[i];
                if (pageIndex == p.pageIndex)
                    return p;
            }

            return null;
        }

    }

    public class PagiPlanDataEdition
    {

        public string editionName;

        public List<PagiPlanDataSection> sectionList;

        public bool masterEdition;
        public int editionCopy;
        public int editionSequenceNumber;
        public string editionComment;


        public PagiPlanDataEdition()
        {
            editionName = "";
            editionCopy = 0;
            masterEdition = false;
          
            sectionList = new List<PagiPlanDataSection>();
            editionSequenceNumber = 1;
            editionComment = "";
            
        }


        public PagiPlanDataEdition(string seditionName)
        {
            editionName = seditionName;
            editionCopy = 0;
            masterEdition = false;
         
            sectionList = new List<PagiPlanDataSection>();
            editionSequenceNumber = 1;
            editionComment = "";
        }

        public PagiPlanDataSection GetSectionObject(string sectionName)
        {
            for (int i = 0; i < sectionList.Count; i++)
            {
                PagiPlanDataSection p = sectionList[i];
                if (sectionName == p.sectionName)
                    return p;
            }

            return null;
        }
    }


    public class PagiPlanData
    {
        public string planName;
        public string publicationName;
        public string publicationAlias;
        public DateTime publicationDate;
        public DateTime updatetime;
        public string defaultColors;

        public List<string> arrSectionNames;

        public List<PagiPlanDataEdition> editionList;


        public PagiPlanData()
        {
            arrSectionNames = new List<string>();
            editionList = new List<PagiPlanDataEdition>();
        }

        public PagiPlanDataEdition GetEditionObject(string edition)
        {
            foreach (PagiPlanDataEdition ed in editionList)
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
                return string.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);

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

       
    }

    
}