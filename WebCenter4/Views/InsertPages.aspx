<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InsertPages.aspx.cs" Inherits="WebCenter4.Views.InsertPages" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Insæt sider i produkt</title>
    <link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet"/>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript">

        function CloseOnReload()
        {
            GetRadWindow().BrowserWindow.document.Form1.HiddenReturendFromPopup.value = '1';
			GetRadWindow().Close();
		}
		function RefreshParentPage() 
        {
		    GetRadWindow().BrowserWindow.document.Form1.HiddenReturendFromPopup.value = '1';
			GetRadWindow().BrowserWindow.document.forms[0].submit(); 
			GetRadWindow().Close();
		}
		function SizeToFit()
		{
			var oWnd;
			var theTop;
			var theLeft;
				
			window.setTimeout(
                function()
                {
                    oWnd = GetRadWindow();
                    if (initWidth == null)
                        initWidth = document.body.scrollWidth+20;
                    oWnd.SetSize(initWidth,  document.body.scrollHeight+100);
						
                }, 600);
		}
			   
		function GetRadWindow()
		{
			var oWindow = null;
			if (window.radWindow) oWindow = window.radWindow;
			else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
			return oWindow;
		}
	</script>
    </telerik:RadCodeBlock>
    <style type="text/css">  
        html, body, form  
        {  
            height: 100%;  
            margin: 0px;  
            padding: 0px;                 
        }  
        </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;">
            <Items>
                <telerik:RadToolBarButton runat="server" Value="Item1">
                    <ItemTemplate>
                        <asp:Label ID="lblInsertPagesHeader" runat="server" Text="Sett inn sider i produkt" CssClass="RadToolbarLabel"></asp:Label>
                    </ItemTemplate>
                </telerik:RadToolBarButton>
            </Items>
        </telerik:RadToolBar>
        <telerik:RadWizard ID="RadWizard1" runat="server" DisplayCancelButton="True" DisplayProgressBar="False" OnNextButtonClick="RadWizard1_NextButtonClick" DisplayNavigationBar="False" OnFinishButtonClick="RadWizard1_FinishButtonClick" OnCancelButtonClick="RadWizard1_CancelButtonClick" OnPreviousButtonClick="RadWizard1_PreviousButtonClick" Localization-Cancel="Cancel" Localization-Next="Next" Localization-Finish="Finish" Localization-Previous="Previous">
            <WizardSteps>
                <telerik:RadWizardStep ID="RadWizardStep1" runat="server" Title="Define pages to insert">
                    <table style="width: 590px; border: 0px;" id="Table1" cellspacing="5" cellpadding="5">
                        <tr>
                            <td style="vertical-align: top;text-align: left;">
                                <asp:Label ID="lblProductLabel" runat="server" Text="Product"></asp:Label>
                            </td>
                            <td style="vertical-align: top;text-align: left;">
                                <telerik:RadComboBox ID="RadComboBoxProduct" runat="server"></telerik:RadComboBox>
                            </td>
                        </tr>
                        <tr>
                            <td style="vertical-align: top;text-align: left;">
                                <asp:Label ID="lblPubDateFrom" runat="server" Text="Pubdate from"></asp:Label>
                            </td>
                            <td>
                                <table style="border: 0px; margin: 0px 0px; padding: 0px 0px;">
                                    <tr>
                                        <td style="vertical-align: top;text-align: left;">
                                            <telerik:RadDatePicker ID="RadDatePickerPubdateFrom" runat="server"></telerik:RadDatePicker>
                                      </td>
                                        <td style="vertical-align: top;text-align: center;">
                                            <asp:Label ID="lblPubDateTo" runat="server" Text="to"></asp:Label>
                                        </td>
                                        <td style="vertical-align: top;text-align: left;">
                                            <telerik:RadDatePicker ID="RadDatePickerPubdateTo" runat="server"></telerik:RadDatePicker>
                                        </td>
                                    </tr>
                               </table>
                            </td>
                        </tr>  
                        <tr>
                            <td style="vertical-align: top;text-align: left;">
                                <asp:Label ID="lblChooseEdition" runat="server" Text="Choose edition"></asp:Label>
                            </td>
                            <td style="vertical-align: top;text-align: left;">
                                <telerik:RadComboBox ID="RadComboBoxEdition" runat="server"></telerik:RadComboBox>
                            </td>
                        </tr>              
                        <tr>
                            <td style="vertical-align: top;text-align: left;">
                                <asp:Label ID="lblChooseSection" runat="server" Text="Choose section"></asp:Label>
                            </td>
                            <td style="vertical-align: top;text-align: left;">
                                <telerik:RadComboBox ID="RadComboBoxSection" runat="server"></telerik:RadComboBox>
                            </td>
                        </tr>
				        <tr>
                            <td style="vertical-align: top;text-align: left;">
                                <asp:Label ID="lblChooseChannels" runat="server" Text="Choose channels"></asp:Label>
                            </td>
                            <td style="vertical-align: top;text-align: left;">
                                <div id="" style="overflow-y:scroll; height:180px;width:300px;">
                                  <telerik:RadCheckBoxList ID="CheckBoxListChannels" Runat="server" AutoPostBack="False" CausesValidation="False" Height="170px" Width="300px" EnableViewState="true">
                                    </telerik:RadCheckBoxList>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td style="vertical-align:middle;text-align: left;">
                              
                                <asp:Label ID="lblPagesToInsert" runat="server" Text="Pages to insert"></asp:Label>
                            </td>
                            <td style="vertical-align: top;text-align: left;">
                                <table><tr><td>
                                <telerik:RadTextBox ID="txtNumberOfPage" runat="server" InputType="Number" LabelWidth="0px" MaxLength="3" Width="50px" Text="1" ></telerik:RadTextBox>
                                    </td><td>
                                     <asp:Label ID="lblLetter" runat="server" Text="Postfix letter"></asp:Label>
                                        </td>
                                    <td>
                                <telerik:RadDropDownList ID="RadDropDownListLetters" runat="server"  SelectedText="A" Width="50px">
                                    <Items>
                                        <telerik:DropDownListItem runat="server" Selected="True" Text="A" />
                                        <telerik:DropDownListItem runat="server" Text="B" />
                                        <telerik:DropDownListItem runat="server" Text="C" />
                                        <telerik:DropDownListItem runat="server" Text="D" />
                                        <telerik:DropDownListItem runat="server" Text="E" />
                                        <telerik:DropDownListItem runat="server" Text="F" />
                                        <telerik:DropDownListItem runat="server" Text="G" />
                                        <telerik:DropDownListItem runat="server" Text="H" />
                                        <telerik:DropDownListItem runat="server" Text="I" />
                                        <telerik:DropDownListItem runat="server" Text="J" />
                                        <telerik:DropDownListItem runat="server" Text="K" />
                                    </Items>
                                </telerik:RadDropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>

                        <tr>
                            <td style="vertical-align: top;text-align: left;">
                                <asp:Label ID="lblPositionOfPage" runat="server" Text="Position of page"></asp:Label>
                            </td>
                            <td style="vertical-align: top;text-align: left;">
                                <telerik:RadRadioButtonList ID="RadRadioButtonList1" runat="server" AutoPostBack="False" >
                                    <Items>
                                      <telerik:ButtonListItem Text="First in document" />
                                      <telerik:ButtonListItem Text="Last in document" />
                                      <telerik:ButtonListItem Text="In middle of document" />
                                      <telerik:ButtonListItem Text="Special position" Selected="true"/>
                                    </Items>
                                </telerik:RadRadioButtonList>
                            </td>                    
                        </tr>
                        <tr>
                            <td>                        
                            </td>
                            <td style="vertical-align: top;text-align: left;">
                                <asp:Label ID="lblAfterPage" runat="server" Text="Position of page"></asp:Label>
                                <telerik:RadTextBox ID="RadTextBoxAfterPage" runat="server" InputType="Number" LabelCssClass=""  MaxLength="3" Width="50px" Wrap="False" ></telerik:RadTextBox>
                            </td>
                        </tr>
                    </table>
                </telerik:RadWizardStep>
                <telerik:RadWizardStep ID="RadWizardStep2" runat="server" Title="Upload pages to insert">
                    <table style="width: 600px; border: 0px;" id="Table2" cellspacing="5" cellpadding="5">
                         <tr>
                            <td style="vertical-align: top;text-align: left;">
                                <asp:Label ID="lblProductLabel2" runat="server" Text="Product"></asp:Label>
                            </td>
                            <td style="vertical-align: top;text-align: left;">
                                <asp:Label ID="lblProduct2" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div id="popup" style="max-height:600px;overflow-y:scroll;">
                                    <asp:GridView runat="server" ID="gridview1" AutoGenerateColumns="False">
                                        <Columns> 
                                            <asp:BoundField DataField="PageNumber" HeaderText="Page number">
                                            </asp:BoundField>  
                                             <asp:BoundField DataField="FinalName" HeaderText="Final Name">
                                            <ItemStyle HorizontalAlign="Left" />
                                            </asp:BoundField>                       
                                            <asp:TemplateField HeaderText="Upload">
                                                <ItemTemplate >
                                                    <asp:FileUpload ID="FileUpload1" runat="server"  />
                                                    <asp:Button ID="Button1" runat="server" Text="Upload" OnClick="Upload" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>                         
                                    </asp:GridView>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Label ID="UploadStatusLabel" runat="server" Text=""></asp:Label>                        
                            </td>
                        </tr>
                    </table>
                </telerik:RadWizardStep>
            </WizardSteps>
        </telerik:RadWizard>

        <asp:Label id="InjectScript" runat="server"></asp:Label>
        <asp:TextBox id="txtProductionID" runat="server" Width="2px" Visible="False"></asp:TextBox>
        <asp:TextBox id="txtPressRunID" runat="server" Width="2px" Visible="False"></asp:TextBox>
    </form>
</body>
</html>
