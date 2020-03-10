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
using System.IO;
using System.Configuration;
using System.Text;
using System.Globalization;
using System.Resources;
using System.Threading;
using WebCenter4.Classes;

namespace WebCenter4
{
	/// <summary>
	/// Summary description for Logwindow.
	/// </summary>
	public partial class Logwindow : System.Web.UI.Page
	{

        private void Page_Load(object sender, System.EventArgs e)
		{
			
			if (!IsPostBack)
			{
                Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

                SetRadToolbarLabel("Refresh", Global.rm.GetString("txtRefresh"));
		
				int logNumber = 0;
				if (Request.QueryString["log"] != null) 
				{
					logNumber = Globals.TryParse(Request.QueryString["log"], 0);
				}	
				Session["LogToView"] = logNumber;
				LoadLog(logNumber);
			}
		}

		private void LoadLog(int logNumber)
		{
			if (logNumber > 0)
			{
				string logFile = "";
				switch (logNumber) 
				{
					case 1:
                        logFile = (string)ConfigurationManager.AppSettings["InputLog"];
						break;
					case 2:
                        logFile = (string)ConfigurationManager.AppSettings["TransmitLog"];
						break;
					case 3:
                        logFile = (string)ConfigurationManager.AppSettings["OutputLog"];
						break;
				}

				// (Re-)connect to server
                /*	NetworkDrive oNetDrive = new NetworkDrive();						
                    try
                    {
                        //set propertys
                        oNetDrive.Force = false;
                        oNetDrive.Persistent = true;
                        oNetDrive.LocalDrive = "";
                        oNetDrive.PromptForCredentials = false;
                        oNetDrive.ShareName = ConfigurationSettings.AppSettings["ConfigFolder"];
                        oNetDrive.SaveCredentials = true;

                        string userName = ConfigurationSettings.AppSettings["ConfigFolderUserName"];
                        if (userName == null)
                            userName = "";
                        string passWord = ConfigurationSettings.AppSettings["ConfigFolderPassword"];
                        if (passWord == null)
                            passWord = "";
                        //match call to options provided
                        if(userName == "" && passWord == "")
                        {					
                            oNetDrive.MapDrive();
                        }
                        else if(userName == "")
                        {
                            oNetDrive.MapDrive(passWord);					
                        }
                        else
                        {
                            oNetDrive.MapDrive(userName,passWord);
                        }
                    }
                    catch(Exception err)
                    {
                        lblError.Text = "Cannot connect to config folder";
                        oNetDrive = null;
                    }
                    oNetDrive = null;

                    if (!File.Exists(logFile)) 
                    {
                        lblError.Text = "Logfile not found";
                    }
                    else
                    {
                        StreamReader sr = File.OpenText(logFile);
                        TextBox.Text = sr.ReadToEnd();
                        sr.Close();
                    }*/
            }					
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
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion

        protected void RadToolBar1_ButtonClick(object sender, Telerik.Web.UI.RadToolBarEventArgs e)
		{
			LoadLog((int)Session["LogToView"]);
		}

        private void SetRadToolbarLabel(string buttonID, string text)
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue(buttonID);
            if (item == null)
                return;
            item.Text = text;
        }

	}
}
