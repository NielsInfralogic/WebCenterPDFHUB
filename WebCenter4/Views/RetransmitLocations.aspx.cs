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
    public partial class RetransmitLocations : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (Request.QueryString["MasterCopySeparationSet"] != null)
                {
                    try
                    {
                        txtMasterCopySeparationSet.Text = (string)Request.QueryString["MasterCopySeparationSet"];
                    }
                    catch (Exception e1)
                    {
                        ;
                    }
                }
                else
                {
                    InjectScript.Text = "<script>CloseOnReload()</" + "script>";
                    return;
                }

                int masterCopySeparationSet = Globals.TryParse(txtMasterCopySeparationSet.Text, 0);
                if (masterCopySeparationSet == 0)
                {
                    InjectScript.Text = "<script>CloseOnReload()</" + "script>";
                    return;
                }

                string errmsg = "";
                CCDBaccess db = new CCDBaccess();

                ArrayList arrPresses = db.GetPressesForMasterSet(masterCopySeparationSet, out errmsg);
                if (errmsg != "")
                {
                    InjectScript.Text = "<script>CloseOnReload()</" + "script>";
                    return;
                }
                if (arrPresses.Count == 1)
                {
                    db.RetransmitMasterCopySeparationSetTX((int)arrPresses[0], masterCopySeparationSet, out errmsg);
                    InjectScript.Text = "<script>RefreshParentPage()</" + "script>";
                    return;
                }

                // else more than one press..
                PopulateLocationDataGrid(arrPresses);
            }

            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            lblLocations.Text = Global.rm.GetString("txtLocations");
            btnApply.Text = Global.rm.GetString("txtApply");
            btnCancel.Text = Global.rm.GetString("txtCancel");

            if (Global.rm.GetString("txtRetransmittolocations") != "")
                lblRetransmittolocations.Text = Global.rm.GetString("txtRetransmittolocations");
        }



        private void PopulateLocationDataGrid(ArrayList arrPresses)
        {


            DataColumn newColumn;
            DataTable dtLocations = new DataTable();
            newColumn = dtLocations.Columns.Add("Use", Type.GetType("System.Boolean"));
            newColumn = dtLocations.Columns.Add("Location", Type.GetType("System.String"));

            Session["ThisTXLocationList"] = arrPresses;

            foreach (int pressID in arrPresses)
            {
                DataRow newRow = dtLocations.NewRow();
                newRow["Location"] = Globals.GetNameFromID("PressNameCache", pressID);
                newRow["Use"] = false;

                dtLocations.Rows.Add(newRow);
            }

            DataGridLocations.DataSource = dtLocations;
            DataGridLocations.DataBind();
        }

        private ArrayList GetSelectedLocations()
        {
            ArrayList arr = new ArrayList();
            foreach (DataGridItem dataItem in DataGridLocations.Items)
            {
                CheckBox cb = (CheckBox)dataItem.Cells[0].FindControl("CheckBoxUseLocation");
                if (cb.Checked)
                {
                    try
                    {
                        int id = Globals.GetIDFromName("PressNameCache", dataItem.Cells[1].Text);
                        if (id > 0)
                            arr.Add(id);
                    }
                    catch
                    {
                        return arr;
                    }
                }
            }
            return arr;
        }

        protected void bntApply_Click(object sender, System.EventArgs e)
		{
            CCDBaccess db = new CCDBaccess();
            string errmsg = "";
            ArrayList selectedPressIDList = GetSelectedLocations();

            int masterCopySeparationSet = Globals.TryParse(txtMasterCopySeparationSet.Text, 0);

            foreach (int pressID in selectedPressIDList)
            {
                db.RetransmitMasterCopySeparationSetTX(pressID, masterCopySeparationSet, out errmsg);
            }

            InjectScript.Text = "<script>RefreshParentPage()</" + "script>";

        }

        protected void btnCancel_Click(object sender, System.EventArgs e)
		{
			InjectScript.Text="<script>CloseOnReload()</" + "script>";
		}

        private void DataGridLocations_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                e.Item.Cells[0].Text = Global.rm.GetString("txtUse");
                e.Item.Cells[1].Text = Global.rm.GetString("txtLocation");
            }
            if ((e.Item.ItemType == ListItemType.Item) ||
                (e.Item.ItemType == ListItemType.AlternatingItem) ||
                (e.Item.ItemType == ListItemType.SelectedItem))
            {
                //				TableCell cell = (TableCell)e.Item.Cells[1];
            }
        }
    }

}