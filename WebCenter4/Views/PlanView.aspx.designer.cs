
namespace WebCenter4.Views
{
    public partial class PlanView
    {
        protected global::System.Web.UI.HtmlControls.HtmlForm Form1;
        protected global::System.Web.UI.WebControls.Panel PanelMainActionButtons;
        protected global::System.Web.UI.WebControls.Panel PanelAddPlan;
        protected global::System.Web.UI.WebControls.Panel PanelDeletePlan;
        protected global::System.Web.UI.WebControls.Panel PanelEditPlan;
        protected global::System.Web.UI.WebControls.Panel PanelEditions;

        protected global::System.Web.UI.WebControls.DataGrid DataGridSections;
        protected global::System.Web.UI.WebControls.DataGrid DataGridProductionList;
        protected global::System.Web.UI.WebControls.DataGrid DataGridProductionListEdit;
        protected global::System.Web.UI.WebControls.DataGrid DataGridSubEditions;
        protected global::System.Web.UI.WebControls.DataGrid DataGridPressEditionMatrix;

        protected global::System.Web.UI.WebControls.Label lblComment;
        protected global::System.Web.UI.WebControls.Label lblPlanPageFormat;
        protected global::System.Web.UI.WebControls.Label lblError;
        protected global::System.Web.UI.WebControls.Label lblPublication;
        protected global::System.Web.UI.WebControls.Label lblPubdate;
        protected global::System.Web.UI.WebControls.Label lblApproval;
        protected global::System.Web.UI.WebControls.Label lblSections;
        protected global::System.Web.UI.WebControls.Label lblAddPagePlan;
        protected global::System.Web.UI.WebControls.Label lblEditionInfo;
        protected global::System.Web.UI.WebControls.Label LblPlanUpdate;
        protected global::System.Web.UI.WebControls.Label lblDeletePlan;
        protected global::System.Web.UI.WebControls.Label lblEditPlan;
        protected global::System.Web.UI.WebControls.Label lblInfo;
        protected global::System.Web.UI.WebControls.Label lblCirculation;
        protected global::System.Web.UI.WebControls.Label lblCirculation2;
        protected global::System.Web.UI.WebControls.Label lblWeekNumber;
        protected global::System.Web.UI.WebControls.Label lblWeekNumber2;
        protected global::System.Web.UI.WebControls.Label lblSubeditions;
        protected global::System.Web.UI.WebControls.Label lblDeadline;
        protected global::System.Web.UI.WebControls.Label lblDeadlineInfo;
        protected global::System.Web.UI.WebControls.Label lblUploadPlan;

        protected global::System.Web.UI.WebControls.CheckBox checkApprovalRequired;
        protected global::System.Web.UI.WebControls.CheckBox CheckBoxCombineSections;
        protected global::System.Web.UI.WebControls.CheckBox cbKeepColors;
        protected global::System.Web.UI.WebControls.CheckBox cbKeepApproval;
        protected global::System.Web.UI.WebControls.CheckBox cbKeepUnique;

        protected global::Telerik.Web.UI.RadButton btnSavePlan;
        protected global::Telerik.Web.UI.RadButton btnSpecialEditions;
        protected global::Telerik.Web.UI.RadButton btnDeletePlan;
        protected global::Telerik.Web.UI.RadButton btlCloseDeletePlan;
        protected global::Telerik.Web.UI.RadButton btnEditPlan;
        protected global::Telerik.Web.UI.RadButton btlCloseEditPlan;
        protected global::Telerik.Web.UI.RadButton btnCancel;
        protected global::Telerik.Web.UI.RadButton btnAddPlan;
        protected global::Telerik.Web.UI.RadButton btnUploadPlan;

        protected global::System.Web.UI.WebControls.TextBox txtComment;

        protected global::System.Web.UI.WebControls.LinkButton LinkButtonAddNewPublication;
        protected global::System.Web.UI.WebControls.LinkButton LinkButtonAddNewPageformat;

        protected global::System.Web.UI.HtmlControls.HtmlInputHidden saveConfirm;
        protected global::System.Web.UI.HtmlControls.HtmlInputHidden saveConfirm2;
        protected global::System.Web.UI.HtmlControls.HtmlInputHidden HiddenNewPubname;
        protected global::System.Web.UI.HtmlControls.HtmlInputHidden HiddenNewPageformat;

        protected global::Telerik.Web.UI.RadComboBox ddPublicationList;
        protected global::Telerik.Web.UI.RadComboBox ddPageFormatList;
        protected global::Telerik.Web.UI.RadDatePicker dateChooserPubDate;
        protected global::Telerik.Web.UI.RadNumericTextBox RadNumericTextBoxCirculation;
        protected global::Telerik.Web.UI.RadNumericTextBox RadNumericTextBoxCirculation2;
        protected global::Telerik.Web.UI.RadNumericTextBox txtWeekNumber;

        protected global::Telerik.Web.UI.RadWindowManager RadWindowManager1;
        protected global::Telerik.Web.UI.RadToolBar RadToolBar1;

    }
}