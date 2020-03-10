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
using System.Configuration;
using WebCenter4.Classes;
using System.Text;
using System.Globalization;
using System.Resources;
using System.Threading;


namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for ChangePriority.
	/// </summary>
	public class ShowMessage : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label Label1;
		protected System.Web.UI.WebControls.Button btnAddToMessage;
		protected System.Web.UI.WebControls.TextBox txtMessage;
		protected System.Web.UI.WebControls.DataGrid DataGrid1;
		protected System.Web.UI.WebControls.Label lblError;
		protected System.Web.UI.WebControls.Label LblFrom;
		protected System.Web.UI.WebControls.TextBox txtFrom;
		protected System.Web.UI.WebControls.Label lblTo;
		protected System.Web.UI.WebControls.TextBox txtTo;
		protected System.Web.UI.WebControls.Label Label2;
		protected System.Web.UI.WebControls.DropDownList ddProductions;
		protected System.Web.UI.WebControls.Label Label3;
		protected System.Web.UI.WebControls.Label lblSent;
		protected System.Web.UI.WebControls.RadioButtonList RadioButtonListPrio;
		protected System.Web.UI.WebControls.Label LblEventTime;
		protected System.Web.UI.WebControls.Label lblSubject;
		protected System.Web.UI.WebControls.TextBox txtSubject;
		protected System.Web.UI.WebControls.Label lblMessage;
		protected System.Web.UI.WebControls.Panel panelFields;
		protected System.Web.UI.WebControls.Button btnCancel;
		protected System.Web.UI.WebControls.Label lblID;
		protected System.Web.UI.WebControls.Label lblMessageID;
		protected System.Web.UI.WebControls.Button btnNewMessage;
		protected System.Web.UI.WebControls.Button btnReply;

		protected int doClose;
	
		private void Page_Load(object sender, System.EventArgs e)
		{

            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);
            btnCancel.Text = Global.rm.GetString("txtClose");

			//txtMessage.Attributes.Add("style", "height:400px;width:600px;");
			
			lblError.Text = "";

			if (Page.IsPostBack == false)
			{
				ViewState["ReplyMode"] = "0";
				Session["AllowProductions"] = null;
				FillProductionCombo();
				GetAllMessages();
				panelFields.Visible = false;
				btnNewMessage.Visible = true;
				btnReply.Visible = false;
				btnAddToMessage.Visible = false;
			}
			
		}


		private bool FillProductionCombo()
		{
			ddProductions.Items.Clear();

			ddProductions.Items.Add(new ListItem("None","0"));

			CCDBaccess db = new CCDBaccess();
			string errmsg = "";

			ProdList prodList = new ProdList();

			if (db.GetAllowedProductions(ref prodList, out errmsg) == false)
			{
				lblError.Text = errmsg;
				return false;
			}
			Session["AllowProductions"] = prodList;

			foreach (ProdItem item in prodList)
			{
				ddProductions.Items.Add(new ListItem(string.Format("{0} {1:00}-{2:00}-{3:0000}", Globals.GetNameFromID("PublicationNameCache", item.publicationID), item.pubDate.Day, item.pubDate.Month, item.pubDate.Year),item.productionID.ToString()));
			}
			
			return true;
		}

		private bool GetAllMessages()
		{
			CCDBaccess db = new CCDBaccess();
			string errmsg = "";

			DataTable dt = db.GetMessages((string)Session["UserName"], out errmsg);
			if (dt == null || errmsg != "")
			{
				lblError.Text = errmsg;
				return false;
			}
	
			DataGrid1.DataSource = dt;
			DataGrid1.DataBind();

			return true;
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
			this.DataGrid1.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGrid1_ItemCommand);
			this.DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGrid1_PageIndexChanged);
			this.DataGrid1.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DataGrid1_ItemDataBound);
			this.DataGrid1.SelectedIndexChanged += new System.EventHandler(this.DataGrid1_SelectedIndexChanged);
			this.btnNewMessage.Click += new System.EventHandler(this.btnNewMessage_Click);
			this.btnAddToMessage.Click += new System.EventHandler(this.btnAddToMessage_Click);
			this.btnReply.Click += new System.EventHandler(this.btnReply_Click);
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		
		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			panelFields.Visible = false;
			btnNewMessage.Visible = true;

		}

		private void btnAddToMessage_Click(object sender, System.EventArgs e)
		{
			CCDBaccess db = new CCDBaccess();
			string errmsg = "";
				
			string msg =  "";

			int nID = Globals.TryParse(lblMessageID.Text,0);

			if (nID != 0)
				msg =  "\r\n--------------------------------------------------------\r\n" + LblEventTime.Text + ":\r\n" + txtMessage.Text;

			DateTime pubDate = DateTime.Now;
			int publicationID = 0;
			if (ddProductions.SelectedValue != "0")
			{
				int productionID = Globals.TryParse(ddProductions.SelectedValue,0);					
				ProdList prodList = (ProdList)Session["AllowProductions"];
				
				if (prodList != null)
					prodList.FindPubAndPubDate(productionID, out publicationID, out pubDate);

			}

			db.UpdateMessage(nID, (string)Session["UserName"],txtTo.Text,txtSubject.Text, msg, RadioButtonListPrio.SelectedIndex  == 1, pubDate, publicationID, out errmsg);

			panelFields.Visible = false;
			btnNewMessage.Visible = true;
			GetAllMessages();
		}

		private void btnNewMessage_Click(object sender, System.EventArgs e)
		{
			ViewState["ReplyMode"] = "1";

			lblMessageID.Text = "0";
			txtFrom.Text = (string)Session["UserName"];
			txtTo.Text = "";
			txtSubject.Text = "";
			txtMessage.Text = "";
			RadioButtonListPrio.SelectedIndex = 0;
			LblEventTime.Text = DateTime.Now.ToString("G");
			panelFields.Visible = true;
			btnReply.Visible = false;
			btnAddToMessage.Visible = true;

			ddProductions.Enabled = true;
			txtFrom.Enabled = true;
			txtTo.Enabled = true;
			txtSubject.Enabled = true;
			txtMessage.Enabled = true;
			RadioButtonListPrio.Enabled = true;

		}


		private void DataGrid1_SelectedIndexChanged(object sender, System.EventArgs e)
		{

		}

		private void DataGrid1_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			if ((e.Item.ItemType == ListItemType.Item) ||
				(e.Item.ItemType == ListItemType.AlternatingItem) ||
				(e.Item.ItemType == ListItemType.SelectedItem))
			{
				
				System.Web.UI.WebControls.Image img =(System.Web.UI.WebControls.Image)e.Item.FindControl("imgSeverity");
				string s = e.Item.Cells[2].Text;

				bool unread = false;
				if (s == "1" || s == "3")
					img.ImageUrl = "../Images/mailread.gif";
				else if (s == "2")
				{
					unread = true;
					img.ImageUrl = "../Images/severemail.gif";
				}
				else
				{
					unread = true;
					img.ImageUrl = "../Images/mailunread.gif";
				}	

				e.Item.Cells[3].Font.Bold = unread;
				e.Item.Cells[4].Font.Bold = unread;
				e.Item.Cells[5].Font.Bold = unread;
				e.Item.Cells[6].Font.Bold = unread;

			}
		}

		private void DataGrid1_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			// Set CurrentPageIndex to the page the user clicked.
			DataGrid1.CurrentPageIndex = e.NewPageIndex;

			// Rebind the data to refresh the DataGrid control. 
			DataBind();
		}

		private void DataGrid1_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			string sID = e.Item.Cells[7].Text;
			int nID = Globals.TryParse(sID, 0);
			if (nID <= 0)
				return;
				
			if (e.CommandName == "Delete")
			{
				CCDBaccess db = new CCDBaccess();
				string errmsg = "";

				if (db.DeleteMessage(nID, out errmsg) == false)
				{
					lblError.Text = errmsg;
					return;
				}
				GetAllMessages();

			}
			if (e.CommandName == "Markread")
			{
				CCDBaccess db = new CCDBaccess();
				string errmsg = "";

				if (db.MarkMessageRead(nID, out errmsg) == false)
				{
					lblError.Text = errmsg;
					return;
				}
				GetAllMessages();
			}
			if (e.CommandName == "Select")
			{
				CCDBaccess db = new CCDBaccess();
				string errmsg = "";

				int severity;
				int isRead;
				string title;
				string message;
				string sender;
				string receiver;
				int publicationID;
				DateTime pubDate;
				DateTime eventTime;

				if (db.GetMessageDetails(nID, out severity, out isRead, out sender, out receiver, out message, out eventTime, out pubDate, out title, out publicationID, out errmsg) == false)
				{
					lblError.Text = errmsg;
					return;

				}

				lblMessageID.Text = nID.ToString();
				txtFrom.Text = sender;
				txtTo.Text = receiver;
				txtSubject.Text = title;
				txtMessage.Text = message;
				RadioButtonListPrio.SelectedIndex = severity;
				LblEventTime.Text = eventTime.ToString("G");


				panelFields.Visible = true;
				btnNewMessage.Visible = false;
				btnReply.Visible = true;

				ddProductions.Enabled = false;
				txtFrom.Enabled = false;
				txtTo.Enabled = false;
				txtSubject.Enabled = false;
				txtMessage.Enabled = false;
				RadioButtonListPrio.Enabled = false;

			}

		}

		private void btnReply_Click(object sender, System.EventArgs e)
		{
			txtTo.Enabled = true;
			txtTo.Text = txtFrom.Text;
			txtFrom.Enabled = false;
			txtFrom.Text = (string)Session["UserName"];
			txtSubject.Enabled = true;
			txtMessage.Enabled = true;
			RadioButtonListPrio.Enabled = true;
			ddProductions.Enabled = true;

			btnNewMessage.Visible = true;
			btnReply.Visible = false;
		
		}

	}
}
