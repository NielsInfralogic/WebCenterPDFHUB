using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Configuration;
using WebCenter4.Classes;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace WebCenter4.Views
{
    public partial class ChangeChannels : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (Request.QueryString["Channels"] != null)
                {
                    try
                    {
                        txtChannels.Text = (string)Request.QueryString["Channels"];
                        txtProductionID.Text = (string)Request.QueryString["ProductionID"];
                        txtPublicationID.Text = (string)Request.QueryString["PublicationID"];
                    }
                    catch //(Exception e1)
                    {
                        ;
                    }
                }
                else
                {
                    InjectScript.Text = "<script>CloseOnReload()</" + "script>";
                    return;
                }

                PopulateChannelDataGrid(txtChannels.Text);

            }

            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);
            lblChannels.Text = Global.rm.GetString("txtChannels");
            btnCancel.Text = Global.rm.GetString("txtCancel");
            btnApply.Text = Global.rm.GetString("txtApply");

        }
    


        private void PopulateChannelDataGrid(string channellist)
		{
			string errmsg = "";
			CCDBaccess	db = new CCDBaccess();
			
			DataTable dtchannelsdb = db.GetChannelCollection(out errmsg);

			DataColumn newColumn;
			DataTable dtChannels = new DataTable();
			newColumn = dtChannels.Columns.Add("Use",Type.GetType("System.Boolean"));
			newColumn = dtChannels.Columns.Add("Channel",Type.GetType("System.String"));

			ArrayList al = new ArrayList();
			al.Clear();
			if (channellist == "")
			{
				int	publicationID = Globals.TryParse(txtPublicationID.Text ,0);
				if (publicationID > 0)
					db.GetPublicationChannelsForPublication(publicationID, ref al, out errmsg);
			}
			else
			{
				string [] ss = channellist.Split(',');
				foreach (string sss in ss)
				{
					int qq = Globals.TryParse(sss,0);
					if (qq > 0)
						al.Add(qq);
				}
			}
			Session["ThisChannelList"] = al;

			

			foreach (DataRow row in dtchannelsdb.Rows)
			{
				DataRow newRow = dtChannels.NewRow();
				newRow["Channel"] = (string)row["Name"];
				newRow["Use"] = false;
				foreach (int no in al)
				{
					if (no == (int)row["ID"]) 
					{
						newRow["Use"] = true;
						break;
					}
				}
				
				dtChannels.Rows.Add(newRow);
			}

			DataGridChannels.DataSource = dtChannels;
			DataGridChannels.DataBind();
		}

		private string GetSelectedChannels()
		{
			string channelIDList = "";
			foreach (DataGridItem dataItem in DataGridChannels.Items)
			{
				CheckBox cb = (CheckBox)dataItem.Cells[0].FindControl("CheckBoxUseChannel");
				if (cb.Checked)
				{
					try 
					{
						int id = Globals.GetIDFromName("ChannelNameCache", dataItem.Cells[1].Text);
						if (id > 0)
						{
							if (channelIDList != "")
								channelIDList += ",";
							channelIDList += id.ToString();
						}					
					}
					catch
					{
						return "";
					}
				}
			}
			return channelIDList;
		}
		
		protected void bntApply_Click(object sender, System.EventArgs e)
		{
			//int nPrio = Globals.TryParse(txtPrioValue.Text, -1);
			
				CCDBaccess db = new CCDBaccess();

				string errmsg = "";
				string miscString3 = GetSelectedChannels();

				int nProductionID = Globals.TryParse(txtProductionID.Text,0);

				if (nProductionID > 0 && miscString3 != "")
				{
					db.UpdateMiscString3(nProductionID, miscString3, out errmsg);
					
					db.UpdateProductionOrderNumber(nProductionID, miscString3, out errmsg);
				}				
			

			InjectScript.Text="<script>RefreshParentPage()</" + "script>";
			
		}

        protected void btnCancel_Click(object sender, System.EventArgs e)
		{
			InjectScript.Text="<script>CloseOnReload()</" + "script>";
		}

		private void DataGridChannels_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Header)
			{
				e.Item.Cells[0].Text = Global.rm.GetString("txtUse");
				e.Item.Cells[1].Text = Global.rm.GetString("txtChannel");
			}
			if ((e.Item.ItemType == ListItemType.Item) ||
				(e.Item.ItemType == ListItemType.AlternatingItem) ||
				(e.Item.ItemType == ListItemType.SelectedItem))
			{
				TableCell cell = (TableCell)e.Item.Cells[1];
				CheckBox use = (CheckBox)e.Item.FindControl("CheckBoxUseChannel");
				use.Checked = false;
				int id = Globals.GetIDFromName("ChannelNameCache", cell.Text);

				if (id > 0 && Session["ThisChannelList"] != null)
				{
					try 
					{
						ArrayList al = (ArrayList)Session["ThisChannelList"];
						foreach (int no in al)
						{
							if (no == id) 
							{
								use.Checked = true;
								break;
							}
						}
					}
					catch
					{
					}
				}
			}
		}
	}
}