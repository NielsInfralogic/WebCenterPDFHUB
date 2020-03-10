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
using Telerik.Web.UI;

namespace WebCenter4.Views
{
    public partial class SetExternalStatus : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (Request.QueryString["MasterCopySeparationSetList"] != null)
                {
                    try
                    {
                        txtMasterCopySeparationSetList.Text = (string)Request.QueryString["MasterCopySeparationSetList"];
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

                DataTable dtExternalStatus = (DataTable)HttpContext.Current.Cache["ExtStatusNameCache"];

                var arr = new string[dtExternalStatus.Rows.Count];
                int i = 0;
                foreach (DataRow row in dtExternalStatus.Rows)
                    arr[i++] = (string)row["StatusName"];

                RadListBox1.DataSource = arr;
                RadListBox1.DataBind();

            }

            Response.ContentEncoding = Encoding.GetEncoding((string)Session["encoding"]);
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture((string)Session["language"]);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)Session["language"]);

            btnCancel.Text = Global.rm.GetString("txtCancel");
            btnApply.Text = Global.rm.GetString("txtApply");

        }

		
		protected void bntApply_Click(object sender, System.EventArgs e)
		{
			CCDBaccess db = new CCDBaccess();

			string errmsg = "";

            string extstatus = RadListBox1.SelectedValue;

            

            string s = txtMasterCopySeparationSetList.Text;
            string[] sa = s.Split(',');
            foreach (string ss in sa)
            {
                int n = 0;
                Int32.TryParse(ss, out n);
                if (n > 0)
                {
                    db.UpdateExternalStatus(n, Globals.GetStatusID(extstatus, 1), out errmsg);
                }

            }


			InjectScript.Text="<script>RefreshParentPage()</" + "script>";
			
		}

        protected void btnCancel_Click(object sender, System.EventArgs e)
		{
			InjectScript.Text="<script>CloseOnReload()</" + "script>";
		}

	}
}