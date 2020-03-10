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
	public partial class AddEditions : System.Web.UI.Page
	{

       

		protected int doClose;
		

	
		private void Page_Load(object sender, System.EventArgs e)
		{
			doClose = 0;
			ViewState["CurrentPublicationID"] = 0;

			if (!this.IsPostBack)
			{	
				if (Request.QueryString["PublicationID"] != null) 
				{
					string s = Request.QueryString["PublicationID"];
					ViewState["CurrentPublicationID"] = Globals.TryParse(s, 0);
				}
				lblError.Text = "";
				lblInfo.Text = "";

				
				CreateEditionDropDown(GetMainEdtion());
			}

            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);
            btnCancel.Text = Global.rm.GetString("txtCancel");
			btnSave.Text = Global.rm.GetString("txtSave");

            SetRadToolbarLabel("Item1", "LabelEditionsHeader", Global.rm.GetString("txtAddSubedition"));
			
			lblEdition.Text = Global.rm.GetString("txtEditionName");
			lblDefaultPageType.Text = Global.rm.GetString("txtDefaultPagetypeForEdition");
			lblEditions.Text = Global.rm.GetString("txtEditions");
			brnAdd.Text = Global.rm.GetString("txtAdd");
			lblCirculation.Text = Global.rm.GetString("txtCirculation");
			lblOrderNumber.Text = Global.rm.GetString("txtOrder");
			lblCirculation2.Text = Global.rm.GetString("txtCirculation2");

			lblCirculation.Visible = (bool)Application["HidePlanCirculation"] == false;
			lblCirculation2.Visible = (bool)Application["HidePlanCirculation2"] == false;

			RadNumericTextBoxCirculation.Visible = (bool)Application["HidePlanCirculation"] == false;
			RadNumericTextBoxCirculation2.Visible = (bool)Application["HidePlanCirculation2"] == false;

			lblOrderNumber.Visible = (bool)Application["OrderSystem"] == false;
			txtOrderNumber.Visible = (bool)Application["OrderSystem"] == false;

			ddlEditionsFrom.Enabled = false;

			DoDataBind();

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

		private void CreateEditionDropDown(string edtionToExclude)
		{
			int publicationIDSelected = (int)ViewState["CurrentPublicationID"]; 

			DataTable table = (DataTable) Cache["EditionNameCache"];

			ArrayList alreadyDefinedEditions = GetSubEditions();

			ddlEditions.Items.Clear();

			string sEditionsAllowed = (string)Session["EditionsAllowed"];
			string [] sEditionsAllowedList = sEditionsAllowed.Split(new char[]{','});
			
			foreach (DataRow row in table.Rows)
			{
				string s = (string)row["Name"];

				if (IsInList(alreadyDefinedEditions, s))
					continue;

				if (s != edtionToExclude)
					if (sEditionsAllowed == "" || sEditionsAllowed == "*" || IsInList(sEditionsAllowedList, s))
						if (publicationIDSelected == 0 || Globals.IsAllowedEdition(publicationIDSelected, (int)row["ID"]))
							ddlEditions.Items.Add(s);
			}		
		}

		private void SetEditionInfo()
		{
			DataSet dsEditions = (DataSet)Session["SubEditions"];
			if (dsEditions == null)
				return ;
			if (dsEditions.Tables.Count < 2)
				return;
			DataTable dt = dsEditions.Tables[1];
			foreach (DataRow row in dt.Rows)
			{
				string s = (string)row["Edition"];
				if (s == ddlEditions.SelectedValue)
				{
					RadNumericTextBoxCirculation.Value = (int)row["Circulation"];
					RadNumericTextBoxCirculation2.Value = (int)row["Circulation2"];
					txtOrderNumber.Text = (string)row["OrderNumber"];
                    txtComment.Text = (string)row["Comment"];

				}
			}	
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


			string mainEdition = GetMainEdtion();

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
				//col.Ordinal
				if (i++ < 2)
					continue;

				if (col.ColumnName == editionname)
					return i;
			}

			return 0;
			
		}

		private void RemoveEdition(string editionname)
		{
			try 
			{
				DataSet dsEditions = (DataSet)Session["SubEditions"];
				if (dsEditions == null)
					return;

				DataTable dt = dsEditions.Tables[0];
				dsEditions.Tables[0].Columns.Remove(editionname);
			}
			catch
			{
			}
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

//			DataGridEditions.CurrentPageIndex = (int)Session["ImportPageNumber"];
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
			this.RadioButtonListEditionType.SelectedIndexChanged += new System.EventHandler(this.RadioButtonListEditionType_SelectedIndexChanged);
			this.brnAdd.Click += new System.EventHandler(this.brnAdd_Click);
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

		private void DataGridEditions_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			lblError.Text = "";
			lblInfo.Text = "";

			string editionName = (string) e.Item.Cells[1].Text;
			

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

			if (editionName == GetMainEdtion())
			{
				lblInfo.Text = "Main edition cannot be deleted";
				return;
			}
		
			foreach (DataRow row in dt.Rows)
			{
				
				if ((string)row["Edition"] == editionName)
				{
					row.Delete();
					break;
				}
			}	


			DataTable dt2 = dsEditions.Tables[1];
			foreach(DataRow row2 in dt2.Rows)
			{
				if ((string)row2["Edition"] == editionName)
				{
					row2.Delete();
					break;
				}
			}

			OrganizeTimedEditions();
			DoDataBind();

		}

		private bool IsTimed(string editionName)
		{
			DataSet dsEditions = (DataSet)Session["SubEditions"];
			if (dsEditions == null)
				return false;
			DataTable dt2 = dsEditions.Tables[1];
			if (dt2 == null || dt2.Rows.Count == 0)
				return false;
			foreach(DataRow row2 in dt2.Rows)
			{
				if ((string)row2["Edition"] == editionName && (int)row2["TimedFrom"] > 0)
					return true;
			}

			return false;
		}



		private void DataGridEditions_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Header)
			{
				e.Item.Cells[0].Text = Global.rm.GetString("txtSection");
				e.Item.Cells[1].Text = Global.rm.GetString("txtPage");
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

					if (IsTimed(thisEdition))
						e.Item.Cells[i].BackColor = Color.LightYellow;

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
					linkButton.Text =  Global.rm.GetString("txtDelete");
					linkButton.CommandName = "Delete";
					linkButton.CommandArgument =  col.ColumnName;
					e.Item.Cells[i++].Controls.Add(linkButton);
				}
				e.Item.Cells[0].BackColor = Color.White;
				e.Item.Cells[1].BackColor = Color.White;
				e.Item.Cells[2].BackColor = Color.White;
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

		private void brnAdd_Click(object sender, System.EventArgs e)
		{
			lblError.Text = "";
			lblInfo.Text = "";

			CopyGridToSession();
			DataSet dsEditions = (DataSet)Session["SubEditions"];
			if (dsEditions == null)
			{
				lblError.Text = "No editions defined";
				return;
			}

			if (FindEditionColumnIndex(ddlEditions.SelectedValue) > 0)
			{
				lblInfo.Text = "Edition is already added";
				return;
			}

			string mainEdition = GetMainEdtion();

			DataTable dt = dsEditions.Tables[0];

			DataColumn newColumn;
			newColumn = dt.Columns.Add(ddlEditions.SelectedValue, Type.GetType("System.String"));

			foreach (DataRow row in dt.Rows)
				row[ddlEditions.SelectedValue] = ddDeafultPageUnique.SelectedIndex == 0 ? mainEdition : ddlEditions.SelectedValue;				

			DataTable dt2 = dsEditions.Tables[1];
			
			DataRow newRow2 = null;
			bool existingRow = false;
			foreach(DataRow row2 in dt2.Rows)
			{
				if ((string)row2["Edition"] == ddlEditions.SelectedValue)
				{
					existingRow = true;
					newRow2 = row2;
					break;
				}
			}
			if (existingRow == false)
				newRow2 = dt2.NewRow();

			newRow2["Edition"] = ddlEditions.SelectedValue;
			newRow2["Circulation"] = (int)RadNumericTextBoxCirculation.Value;
			newRow2["Circulation2"] = (int)RadNumericTextBoxCirculation2.Value;
			newRow2["OrderNumber"] = txtOrderNumber.Text;
            newRow2["Comment"] = txtComment.Text;
            if (RadioButtonListEditionType.SelectedIndex == 0)
			{
				newRow2["TimedFrom"] = 0;
				newRow2["TimedTo"] = 0;
				newRow2["EditionSequence"] = 0;
			}
			else
			{
				newRow2["TimedFrom"] = Globals.GetIDFromName("EditionNameCache", ddlEditionsFrom.SelectedValue);
				newRow2["TimedTo"] = 0;
				newRow2["EditionSequence"] = 0;
			}

			if (existingRow == false)
				dt2.Rows.Add(newRow2);


			OrganizeTimedEditions();

			FillTimedEditionCombo();

			DoDataBind();

		}

		private void OrganizeTimedEditions()
		{
			if (Session["SubEditions"] == null)
				return;
			DataSet dsEditions = (DataSet)Session["SubEditions"];
			if (dsEditions == null)
				return;

			// Set Timed to in other end..
			foreach(DataRow edRow in dsEditions.Tables[1].Rows)
			{
				string edition = (string)edRow["Edition"];
				int timedEditionFrom = (int)edRow["TimedFrom"];
				int timedEditionTo = (int)edRow["TimedTo"];
				int timedEditionSequence = timedEditionFrom>0 || timedEditionTo>0 ? (int)edRow["EditionSequence"] : 0;

				if (timedEditionFrom > 0) 
				{
					
					foreach(DataRow edRow2 in dsEditions.Tables[1].Rows)
					{						
						if (timedEditionFrom == Globals.GetIDFromName("EditionNameCache", (string)edRow2["Edition"])) 
						{
							edRow2["TimedTo"] = Globals.GetIDFromName("EditionNameCache", edition);
							break;
						}
					}
				}
			}

			// Set sequence
			foreach(DataRow edRow in dsEditions.Tables[1].Rows)
			{	
				string edition = (string)edRow["Edition"];
				int timedEditionFrom = (int)edRow["TimedFrom"];
				int timedEditionTo = (int)edRow["TimedTo"];

				if (timedEditionFrom == 0 && timedEditionTo == 0)
					edRow["EditionSequence"] = 0; 
				else if (timedEditionFrom == 0)
					edRow["EditionSequence"] = 1;	// Start of sequence
				else 
				{
					foreach(DataRow edRow2 in dsEditions.Tables[1].Rows)
					{						
						if (timedEditionFrom == Globals.GetIDFromName("EditionNameCache", (string)edRow2["Edition"])) 
						{
							edRow["EditionSequence"] = (int)edRow2["EditionSequence"] + 1;
							break;
						}
					}
				}
			}
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

		private void btnSave_Click(object sender, System.EventArgs e)
		{
			CopyGridToSession();
			doClose = 1;
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

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			doClose = 1;
		}

		private void DataGridEditions_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (e.CommandName == "Delete")
			{
				lblError.Text = "";
				lblInfo.Text = "";
				RemoveEdition((string)e.CommandArgument);
				DoDataBind();
			}
		}

		private void RadioButtonListEditionType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ddDeafultPageUnique.Enabled = RadioButtonListEditionType.SelectedIndex == 0;
			if (RadioButtonListEditionType.SelectedIndex == 0)
				ddDeafultPageUnique.SelectedIndex = 0;

			ddlEditionsFrom.Enabled = RadioButtonListEditionType.SelectedIndex == 1;

			if (RadioButtonListEditionType.SelectedIndex == 1)
				FillTimedEditionCombo();

		
		}

		private void FillTimedEditionCombo()
		{
			ddlEditionsFrom.Items.Clear();

			DataSet dsEditions = (DataSet)Session["SubEditions"];
			DataTable dt = dsEditions.Tables[0];

			int i = 0;
			foreach (DataColumn col in dt.Columns)
			{
				// Skip colums Section,Page
				if (i< 2)
				{
					i++;
					continue;
				}

				ddlEditionsFrom.Items.Add(col.ColumnName);
			}

			ddlEditionsFrom.SelectedIndex = ddlEditionsFrom.Items.Count - 1;
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
