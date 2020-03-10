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
using WebCenter4.Classes;
using System.Text;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace WebCenter4.Views
{
	/// <summary>
	/// Summary description for AddEditions.
	/// </summary>
    public partial class ChangeEditionInfo : System.Web.UI.Page
	{

	
		private void Page_Load(object sender, System.EventArgs e)
		{
			ViewState["CurrentPublicationID"] = 0;

			if (!this.IsPostBack)
			{
                Session["SubEditions"] = null;
                LoadEditionInfo();
			}

            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);
            btnCancel.Text = Global.rm.GetString("txtCancel");
			btnSave.Text = Global.rm.GetString("txtSave");

            SetRadToolbarLabel("Item1", "LabelEditionsHeader", Global.rm.GetString("txtEditSubeditions"));

			DoDataBind();

		}

        private bool LoadEditionInfo()
        {
           int publicationID =  Globals.GetIDFromName("PublicationNameCache", (string)Session["SelectedPublication"]);
           DateTime pubDate = (DateTime)Session["SelectedPubDate"];


           ArrayList aEditions = new ArrayList();
           //aEditions =   GetListOfEditionsFromGrid(ref  aPresses);


           if (aEditions.Count == 0)
               return false;

           string mainEdtion = (string)aEditions[0];

           DataSet ds = new DataSet();
           DataTable dt = new DataTable("SubEditions");
           DataColumn newColumn;
           newColumn = dt.Columns.Add("Section", Type.GetType("System.String"));
           newColumn = dt.Columns.Add("Page", Type.GetType("System.String"));
           for (int i = 0; i < aEditions.Count; i++)
           {
               newColumn = dt.Columns.Add((string)aEditions[i], Type.GetType("System.String"));
           }

           return true;
        }

		private bool IsInList(string [] array, string toFind)
		{
			foreach (string s in array)
			{
				if (s == toFind)
					return true;
			}
			return false;
		}

		private bool IsInList(ArrayList al, string toFind)
		{
			foreach (string s in al)
			{
				if (s == toFind)
					return true;
			}
			return false;
		}

	
		private string GetMainEdtion()
		{
			DataSet dsEditions = (DataSet)Session["SubEditions"];
			if (dsEditions == null)
				return "";
			return dsEditions.Tables[0].Columns[2].ColumnName;
			
		}


		private ArrayList GetSubEditions()
		{
			ArrayList al = new ArrayList();
			DataSet dsEditions = (DataSet)Session["SubEditions"];
			if (dsEditions == null)
				return al;
			DataTable dt = dsEditions.Tables[0];

			//string mainEdition = GetMainEdtion();

			int i = 0;
			foreach (DataColumn col in dt.Columns)
			{
				if (i++ < 3)
					continue;

				al.Add(col.ColumnName);
			}

			return al;			
		}

		private int FindEditionColumnIndex(string editionname)
		{
			DataSet dsEditions = (DataSet)Session["SubEditions"];
			if (dsEditions == null)
				return 0;

			DataTable dt = dsEditions.Tables[0];

			int i = 0;
			foreach (DataColumn col in dt.Columns)
			{
                if (i > 2)
                {
                    if (col.ColumnName == editionname)
                        return i;
                }
                i++;
			}
			return 0;
			
		}


		private void DoDataBind()
		{
			DataSet dsEditions = (DataSet)Session["SubEditions"];

			if (dsEditions == null)
			{
				lblError.Text = "No editions defined";
				return;
			}

			DataTable dt = dsEditions.Tables[0];

			if (dt.Columns.Count < 2)
			{
				lblInfo.Text = "No editions defined";
				return;
			}

            DataGridEditions.DataSource = dt;
            DataGridEditions.DataBind();
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
			this.DataGridEditions.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGridEditions_ItemCommand);
			this.DataGridEditions.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DataGridEditions_ItemDataBound);
			this.DataGridEditions.SelectedIndexChanged += new System.EventHandler(this.DataGridEditions_SelectedIndexChanged);
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void DataGridEditions_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			DataGridEditions.EditItemIndex = -1;

		}

		private void DataGridEditions_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Header)
			{
				e.Item.Cells[0].Text = Global.rm.GetString("txtSection");
				e.Item.Cells[1].Text = Global.rm.GetString("txtPage");

                for (int i=0; i<e.Item.Cells.Count; i++)
                    e.Item.Cells[i].Width = i < 3 ? 50: 60;
				//e.Item.Cells[0].BackColor = Color.Azure;
				//e.Item.Cells[1].BackColor = Color.Azure;
			}

			if ((e.Item.ItemType == ListItemType.Item) || 
				(e.Item.ItemType == ListItemType.AlternatingItem) || 
				(e.Item.ItemType == ListItemType.SelectedItem))
			{
				int i = 0;
				string mainEdition = GetMainEdtion();
				ArrayList arrSubEdtionNames = GetSubEditions();
				DataSet dsEditions = (DataSet)Session["SubEditions"];
				DataTable dt = dsEditions.Tables[0];

				foreach (DataColumn col in dt.Columns)
				{
					// Skip colums Section and Page
					string section = e.Item.Cells[0].Text;
					string pagename = e.Item.Cells[1].Text;

					if (i< 2)
					{
						i++;
						continue;
					}

					string thisEdition = col.ColumnName;
					string thisEditionMaster = e.Item.Cells[i].Text;		// Current selected page edition name in cell for this subedition
					
					// Is this main edition column (column 3)
					if (thisEdition == mainEdition)
					{	
						e.Item.Cells[i].BackColor = Color.LightGreen;
						i++;
						continue;
					}


					// Rest handles sub-edition columns

					System.Web.UI.WebControls.DropDownList dropDownList = new System.Web.UI.WebControls.DropDownList();
					dropDownList.Items.Add(mainEdition); // 0

					foreach (string s in arrSubEdtionNames)					
						dropDownList.Items.Add(s);
	
					if (dropDownList.Items.FindByValue(thisEditionMaster) != null)
						dropDownList.SelectedValue = thisEditionMaster;
					else
						dropDownList.SelectedIndex = 0;

					string storedSubEditionPage = GetMasterEditionForPage(thisEdition, section, pagename);
						if (dropDownList.Items.FindByValue(storedSubEditionPage) != null)
							dropDownList.SelectedValue = storedSubEditionPage;
					
					// Form ID name as ddEA1		(E: edition, A: Section, 1: Page number)
					dropDownList.ID = "dd" + thisEdition + e.Item.Cells[0].Text + e.Item.Cells[1].Text;
					dropDownList.EnableViewState = true;
					dropDownList.AutoPostBack = false;

					e.Item.Cells[i].BackColor = thisEditionMaster == thisEdition ? Color.LightGreen : Color.LightBlue;


					e.Item.Cells[i++].Controls.Add(dropDownList);
				}
			} 

			if (e.Item.ItemType == ListItemType.Footer)
			{
				int i = 0;
				DataSet dsEditions = (DataSet)Session["SubEditions"];
				DataTable dt = dsEditions.Tables[0];
				foreach (DataColumn col in dt.Columns)
				{
					if (i<3)
					{
						i++;
						continue;
					}
					System.Web.UI.WebControls.LinkButton linkButton = new System.Web.UI.WebControls.LinkButton();
					linkButton.ID = "lnk" + col.ColumnName;
                    linkButton.Text = Global.rm.GetString("txtAllUnique");
					linkButton.CommandName = "Unique";
					linkButton.CommandArgument =  col.ColumnName;
					e.Item.Cells[i].Controls.Add(linkButton);

                    System.Web.UI.WebControls.Label label = new System.Web.UI.WebControls.Label();
                    label.Text = "<br>";

                    e.Item.Cells[i].Controls.Add(label);

                    System.Web.UI.WebControls.LinkButton linkButton2 = new System.Web.UI.WebControls.LinkButton();
                    linkButton2.ID = "lnk2" + col.ColumnName;
                    linkButton2.Text = Global.rm.GetString("txtAllCommon");
                    linkButton2.CommandName = "Common";
                    linkButton2.CommandArgument = col.ColumnName;
                    e.Item.Cells[i].Controls.Add(linkButton2);

                    i++;
				}
				e.Item.Cells[0].BackColor = Color.White;
				e.Item.Cells[1].BackColor = Color.White;
				e.Item.Cells[2].BackColor = Color.White;
			}
		}

        private void DataGridEditions_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            if (e.CommandName == "Unique")
            {
                lblError.Text = "";
                    lblInfo.Text = "";
                int colIndex =  FindEditionColumnIndex((string)e.CommandArgument);
                if (colIndex < 3)
                    return;
                DataSet dsEditions = (DataSet)Session["SubEditions"];
                DataTable dt = dsEditions.Tables[0];
                foreach (DataRow row in dt.Rows)
                {
                    row[colIndex] = (string)e.CommandArgument;
                }
                DoDataBind();
            }

            if (e.CommandName == "Common")
            {
                lblError.Text = "";
                lblInfo.Text = "";
                int colIndex = FindEditionColumnIndex((string)e.CommandArgument);
                if (colIndex < 3)
                    return;
                string mainEdition = GetMainEdtion();
                DataSet dsEditions = (DataSet)Session["SubEditions"];
                DataTable dt = dsEditions.Tables[0];
                foreach (DataRow row in dt.Rows)
                {
                    row[colIndex] = mainEdition;
                }
                DoDataBind();
            }
    
        }

		private string GetMasterEditionForPage(string subEdition, string section, string pageName)
		{
			DataSet dsEditions = (DataSet)Session["SubEditions"];
			DataTable dt = dsEditions.Tables[0];

			int i=0;
			bool found = false;
			foreach (DataColumn col in dt.Columns)
			{
				// Skip colums Section and Page
				if (i< 2)
				{
					i++;
					continue;
				}

				if (col.ColumnName == subEdition)
				{
					found = true;
					break;
				}
			}

			if (found == false)
				return subEdition;

			foreach (DataRow row in dt.Rows)
			{
				if ((string)row["Section"] == section && (string)row["Page"] == pageName)
					return (string)row[subEdition];
			}
																		
			return subEdition;
		}


		private void CopyGridToSession()
		{
			foreach (DataGridItem dataItem in DataGridEditions.Items)
			{
				try 
				{
					int subEditions = dataItem.Cells.Count;
					for (int i=3; i<subEditions; i++)
					{
						string section = (string)dataItem.Cells[0].Text;
						string pagename = (string)dataItem.Cells[1].Text;
						string mainEdition = (string)dataItem.Cells[2].Text;

						System.Web.UI.WebControls.DropDownList ddl = (System.Web.UI.WebControls.DropDownList)dataItem.Cells[i].Controls[0];
						
						SetEditionPage(i,section, pagename, mainEdition, ddl.SelectedValue); // Last param is edition name of this subedition page
					}

				}
				catch
				{
				}

			}
		}

        private void SetEditionPage(int idx, string section, string pagename, string mainEdition, string editionNameofPage)
        {
            DataSet dsEditions = (DataSet)Session["SubEditions"];
            if (dsEditions == null)
            {
                lblError.Text = "Error setting main/sub edition reference";
                return;
            }
            DataTable dt = dsEditions.Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                if ((string)row["Section"] == section && (string)row["Page"] == pagename)
                {
                    row[idx] = editionNameofPage;
                    return;
                }
            }
        }


        protected void btnSave_Click(object sender, System.EventArgs e)
		{
			CopyGridToSession();
            InjectScript.Text = "<script>CloseOnReload()</" + "script>";
        }

        protected void btnCancel_Click(object sender, System.EventArgs e)
		{
            InjectScript.Text = "<script>CloseOnReload()</" + "script>";

		}

        private void SetRadToolbarLabel(string buttonID, string labelID, string text)
        {
            Telerik.Web.UI.RadToolBarItem item = RadToolBar1.FindItemByValue(buttonID);
            if (item == null)
                return;
            Label label = (Label)item.FindControl(labelID);
            if (label == null)
                return;
            label.Text = text;
        }
	}
}
