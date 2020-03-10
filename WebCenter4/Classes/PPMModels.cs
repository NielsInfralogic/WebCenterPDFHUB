using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCenter4.Classes
{
    public class PPMPublication
    {
        public string Long_name { get; set; } = "";
        public string Short_name { get; set; } = "";

        public string Default_PressGroup { get; set; } = "ODE";
        public string Default_Zone { get; set; } = "1";
        public string Default_PageFormat { get; set; } = "Tabloid";

        public int Default_copies { get; set; } = 1000;

        public string Default_Paper { get; set; } = "40g";
    }

    public class PPMPageFormat
    {
        public string Name { get; set; } = "";
        public int Height { get; set; } = 0;
        public int Width { get; set; } = 0;

    }

    public class PPMPlan
    {
        public string PressGroup { get; set; } = "";
        public string Publication { get; set; } = "";
        public DateTime PubDate { get; set; } = DateTime.MinValue;

        public string Editions { get; set; } = "";
        public string Sections { get; set; } = "";
        public string PageFormat { get; set; } = "";
        public string Paper { get; set; } = "";

        public bool AllCommonSubeditions { get; set; } = true;

        public string Comment { get; set; } = "";

        public int Circulation { get; set; } = 0;

        public string UploadedFiles { get; set; } = "";

        public int TrimWidth { get; set; } = 0;
        public int TrimHeight { get; set; } = 0;
    }
}