using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebCenter4.Classes;
using System.Globalization;
using System.Resources;
using System.Text;

namespace WebCenter4
{
    public partial class ShowFileDistLog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            CCDBaccess db = new CCDBaccess();
            string errmsg = "";
            string result = "";

            db.GetCustomLog(DropDownList1.SelectedValue, ref result, out errmsg);

            if (result != "")
                DownloadLog(result, DropDownList1.SelectedValue);
        }


        private void DownloadLog(string log, string title)
        {
            string fileName = string.Format("{0:00}{1:00}{2:00}{3:00}{4:00}{5:00}-{6}.txt", DateTime.Now.Day, DateTime.Now.Month, 2000 - DateTime.Now.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second,title);
            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;

            byte[] unicodeBytes = unicode.GetBytes(log);
            byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);
            char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
            ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
            string asciiString = new string(asciiChars);


            // Send to browser..
            Response.Clear();
            Response.Buffer = false;
            Response.Charset = "";
            Response.ContentType = "text/plain";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.Write(asciiString);
            Response.Flush();
            Response.End();
        }
    }
}