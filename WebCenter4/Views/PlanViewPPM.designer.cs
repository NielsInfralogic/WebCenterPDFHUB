using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace WebCenter4.Views
{
    public partial class PlanViewPPM
    {
        protected global::System.Web.UI.WebControls.Panel PanelMainActionButtons;
        protected global::System.Web.UI.WebControls.Panel PanelAddPlan;

        protected global::System.Web.UI.WebControls.DataGrid DataGridProductionList;

        protected global::System.Web.UI.WebControls.Label lblComment;
        protected global::System.Web.UI.WebControls.Label lblImposition;
        protected global::System.Web.UI.WebControls.Label lblError;

        protected global::System.Web.UI.WebControls.Label lblPublication;
        protected global::System.Web.UI.WebControls.Label lblPubdate;
        protected global::System.Web.UI.WebControls.Label lblEdition;
        protected global::System.Web.UI.WebControls.Label lblAddPagePlan;
        protected global::System.Web.UI.WebControls.Label LblPlanUpdate;
        protected global::System.Web.UI.WebControls.Label lblDeletePlan;
        protected global::System.Web.UI.WebControls.Label lblInfo;
        protected global::System.Web.UI.WebControls.Label lblCirculation;
        protected global::System.Web.UI.WebControls.Label lblPressGroup;
        protected global::System.Web.UI.WebControls.Label lblPaper;
        protected global::System.Web.UI.WebControls.Label lblPageFormat;

        protected global::System.Web.UI.WebControls.Label lblSectionA;
        protected global::System.Web.UI.WebControls.Label lblSectionB;
        protected global::System.Web.UI.WebControls.Label lblSectionC;
        protected global::System.Web.UI.WebControls.Label lblSectionD;
        protected global::System.Web.UI.WebControls.Label lblSectionE;
        protected global::System.Web.UI.WebControls.Label lblSectionF;
        protected global::System.Web.UI.WebControls.Label lblSectionG;
        protected global::System.Web.UI.WebControls.Label lblSectionH;
        protected global::System.Web.UI.WebControls.Label lblSectionI;

        protected global::System.Web.UI.WebControls.Label lblPackageFiles;

        protected Telerik.Web.UI.RadButton btnSavePlan;
        protected Telerik.Web.UI.RadButton btnDeletePlan;
        protected Telerik.Web.UI.RadButton btnCancel;
        protected Telerik.Web.UI.RadButton btnAddPlan;

        protected global::System.Web.UI.WebControls.TextBox txtComment;

        protected global::System.Web.UI.HtmlControls.HtmlInputHidden saveConfirm;

        protected global::Telerik.Web.UI.RadComboBox ddPublicationList;
        protected global::Telerik.Web.UI.RadComboBox ddEditionList;
        protected global::Telerik.Web.UI.RadComboBox ddPaperList;
        protected global::Telerik.Web.UI.RadComboBox ddPageFormatList;
        protected global::Telerik.Web.UI.RadComboBox ddPressGroupList;

        protected global::Telerik.Web.UI.RadDatePicker dateChooserPubDate;
        protected global::Telerik.Web.UI.RadNumericTextBox RadNumericTextBoxCirculation;

        protected global::Telerik.Web.UI.RadNumericTextBox RadNumericTextBoxPages1;
        protected global::Telerik.Web.UI.RadNumericTextBox RadNumericTextBoxPages2;
        protected global::Telerik.Web.UI.RadNumericTextBox RadNumericTextBoxPages3;
        protected global::Telerik.Web.UI.RadNumericTextBox RadNumericTextBoxPages4;
        protected global::Telerik.Web.UI.RadNumericTextBox RadNumericTextBoxPages5;
        protected global::Telerik.Web.UI.RadNumericTextBox RadNumericTextBoxPages6;
        protected global::Telerik.Web.UI.RadNumericTextBox RadNumericTextBoxPages7;
        protected global::Telerik.Web.UI.RadNumericTextBox RadNumericTextBoxPages8;
        protected global::Telerik.Web.UI.RadNumericTextBox RadNumericTextBoxPages9;

        protected global::Telerik.Web.UI.RadNumericTextBox RadNumericTextBoxTrimWidth;
        protected global::Telerik.Web.UI.RadNumericTextBox RadNumericTextBoxTrimHeight;

        protected global::Telerik.Web.UI.RadToolBar RadToolBar1;

        protected global::Telerik.Web.UI.RadButton RadButtonSaveFile;
        protected global::Telerik.Web.UI.RadAsyncUpload RadAsyncUpload1;



        protected global::System.Web.UI.HtmlControls.HtmlInputHidden hiddenUploadPath;
        protected global::System.Web.UI.WebControls.GridView GridView1;
        protected global::System.Web.UI.WebControls.Literal ltrNoResults;


    }
}