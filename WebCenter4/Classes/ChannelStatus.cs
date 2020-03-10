using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCenter4.Classes
{
    public class ChannelProgress
    {
       // public int ProductionID { get; set; } = 0;
        public int ChannelID { get; set; } = 0;
        public string Name { get; set; } = "";
        public int Pages { get; set; } = 0;
        public int PagesSent { get; set; } = 0;

        public int PagesWithError { get; set; } = 0;
        public string Comment { get; set; } = "";
        public string Alias { get; set; } = "";
    }
}