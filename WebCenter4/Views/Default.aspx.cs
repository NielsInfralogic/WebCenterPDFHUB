using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // allows the javascript function to do a postback and call the onClick method
        // associated with the linkButton LinkButton1.
        string jscript = "function UploadComplete(){";
        jscript += string.Format("__doPostBack('{0}','');", LinkButton1.ClientID.Replace("_", "$"));
        jscript += "};";
        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "FileCompleteUpload", jscript, true);
    }
    protected string GetFlashVars()
    {
        // Adds query string info to the upload page
        // you can also do something like:
        // return "?" + Server.UrlEncode("CategoryID="+CategoryID);
        // we UrlEncode it because of how LoadVars works with flash,
        // we want a string to show up like this 'CategoryID=3&UserID=4' in
        // the uploadPage variable in flash.  If we passed this string withou
        // UrlEncode then flash would take UserID as a seperate LoadVar variable
        // instead of passing it into the uploadPage variable.
        // then in the httpHandler we get the CategoryID and UserID values from 
        // the query string. See Upload.cs in App_Code
        return "?" + Server.UrlEncode(Request.QueryString.ToString());
    }
    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        // Do something that needs to be done such as refresh a gridView
        // say you had a gridView control called gvMyGrid displaying all 
        // the files uploaded. Refresh the data by doing a databind here.
        // gvMyGrid.DataBind();
    }
}
