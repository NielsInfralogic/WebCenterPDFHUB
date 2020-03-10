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
    public partial class ReleasePresses : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (Request.QueryString["CopyFlatSeparationSet"] != null)
                {
                    try
                    {
                        txtCopyFlatSeparationSet.Text = (string)Request.QueryString["CopyFlatSeparationSet"];
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

                int CopyFlatSeparationSet = Globals.TryParse(txtCopyFlatSeparationSet.Text, 0);
                if (CopyFlatSeparationSet == 0)
                {
                    InjectScript.Text = "<script>CloseOnReload()</" + "script>";
                    return;
                }

                string errmsg = "";
                CCDBaccess db = new CCDBaccess();

                ArrayList masterCopySeparationSetList = new ArrayList();

                if (db.GetMasterSetFromCopyFlatSeparationSetMulti(CopyFlatSeparationSet, ref  masterCopySeparationSetList, out  errmsg) == false)
                {
                    int MasterCopySeparationSet = db.GetMasterSetFromCopyFlatSeparationSet(CopyFlatSeparationSet, out errmsg);
                    masterCopySeparationSetList.Add(MasterCopySeparationSet);
                }
                ArrayList arrPresses = new ArrayList();
                int masterCopySeparationSet = 0;
                int masterCopySeparationSet2 = 0;
                if (masterCopySeparationSetList.Count >= 1)
                {
                    masterCopySeparationSet = (int)masterCopySeparationSetList[0];
                    arrPresses = db.GetPressesForMasterSet(masterCopySeparationSet, out errmsg);
                }
                if (errmsg != "")
                {
                    InjectScript.Text = "<script>CloseOnReload()</" + "script>";
                    return;
                }

                ArrayList arrPresses2 = new ArrayList();
                if (masterCopySeparationSetList.Count > 1)
                {
                    masterCopySeparationSet2 = (int)masterCopySeparationSetList[1];
                    arrPresses2 = db.GetPressesForMasterSet(masterCopySeparationSet2, out errmsg);
                }
                if (errmsg != "")
                {
                    InjectScript.Text = "<script>CloseOnReload()</" + "script>";
                    return;
                }
                txtMasterCopySeparationSet.Text = masterCopySeparationSet.ToString();
                txtMasterCopySeparationSet2.Text = masterCopySeparationSet2.ToString();


                // else more than one press..
                foreach (int s2 in arrPresses2)
                {
                    bool found = false;
                    foreach (int s1 in arrPresses)
                    {
                        if (s1 == s2)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found == false)
                        arrPresses.Add(s2);
                }

                PopulateLocationDataGrid(arrPresses);
            }

            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            lblLocations.Text = Global.rm.GetString("txtLocations");
            btnCancel.Text = Global.rm.GetString("txtCancel");
            bntApply.Text = Global.rm.GetString("txtApply");

            //if (Global.rm.GetString("txtRetransmittolocations") != "")
            //	lblRetransmittolocations.Text = Global.rm.GetString("txtRetransmittolocations");
            //txtRetransmittolocations
            if (Global.rm.GetString("txtReleasetolocations") != "")
                lblRetransmittolocations.Text = Global.rm.GetString("txtReleasetolocations");
            //txtReleasetolocations
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
            this.DataGridLocations.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DataGridLocations_ItemDataBound);
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion


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
            int masterCopySeparationSet2 = Globals.TryParse(txtMasterCopySeparationSet2.Text, 0);

            foreach (int pressID in selectedPressIDList)
            {
                db.ReleaseMasterCopySeparationSetTX(pressID, masterCopySeparationSet, out errmsg);
                if (masterCopySeparationSet2 > 0)
                    db.ReleaseMasterCopySeparationSetTX(pressID, masterCopySeparationSet2, out errmsg);
            }

            InjectScript.Text = "<script>RefreshParentPage()</" + "script>";

        }

        protected void btnCancel_Click(object sender, System.EventArgs e)
        {
            InjectScript.Text = "<script>CloseOnReload()</" + "script>";
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