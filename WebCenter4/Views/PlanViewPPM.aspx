<%@ Page language="c#" Codebehind="PlanViewPPM.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.PlanViewPPM" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>PPM Plan</title>
		<link rel="stylesheet" type="text/css" href="../Styles/WebCenter.css" />
        <script type="text/javascript" src="../Scripts/jquery-1.11.3.min.js"></script>
		<script language="JavaScript" type="text/javascript"> 
            <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
            
               function OnClientFileUploadFailed(sender, args) {
		            if (args.get_message() == "error") {
		                args.set_handled(true);
		            }
		        }

		        function OnClientFileUploaded (radAsyncUpload, args) {
		            var contentType = args.get_fileInfo().ContentType;

		            uploadedfileCount = radAsyncUpload._uploadedFiles.length;
		     
		            if (contentType == null)
		                return;
		            var row = args.get_row();
		            if (row == null)
		                return;

		            var filename = args.get_fileName();
			       
		            var inputName = radAsyncUpload.getAdditionalFieldID("TextBox");
		            if (inputName == null)
		                return;
		     
		            var inputType = "text";
		            var inputID = inputName;
		       
		            var input = document.createElement('input');
		            input.id = inputID;
		            input.type = 'text';
		            input.name = inputName;
		            input.value = filename;

		            //var input = createInput('text', inputID, inputName, filename);
		            if (input == null)
		                return;
	
		            var label = createLabel(inputID);
		            if (label == null)
		                return;
		
		            var br = document.createElement("br");
		            if (br == null)
		                return;
		            row.appendChild(br);
		            row.appendChild(label);
		            row.appendChild(input);
		
		        }

		        function createInput(inputType, inputID, inputName,value) {
		            var input = document.createElement("input");
		            input.setAttribute("type", inputType);
		            input.setAttribute("id", inputID);
		            input.setAttribute("name", inputName);
		            input.setAttribute("value", value);
		            input.style.setAttribute("width", "200px");
		            return input;
		        }

		        function createLabel(forArrt) {
		            var label = document.createElement("label");
		            label.setAttribute("for", forArrt);
		            label.innerHTML = "Endelige navn: ";
		            return label;
		        }

		        function submitPage() {
		            var uploadingRows = $(".RadAsyncUpload").find(".ruUploadProgress");
		            for (var i = 0; i < uploadingRows.length; i++) {
		                if (!$(uploadingRows[i]).hasClass("ruUploadCancelled") && !$(uploadingRows[i]).hasClass("ruUploadFailure") && !$(uploadingRows[i]).hasClass("ruUploadSuccess")) {
		                    alert("you could not submit the page during upload :)");
		                    return
		                }
		            }

		            theForm.submit();
		        }


	            function ShowProgress()
	            {
		            var progress = document.getElementById("ProgressBar");
		            progress.style.display = "";
	            }
	            function StopProgress()
	            {
		            var progress = document.getElementById("ProgressBar");
		            progress.style.display = "none";
	            }	
	
	            function confirm_delete() {
		            if (confirm("Slet denne plan?")==false)
			            return false;
	            }
    		</script>
        </telerik:RadCodeBlock>
        
        <style type="text/css">  
            html, body, form  
            {  
                margin: 0px;  
                padding: 0px;  
            }  
            tr.border_bottom td 
            {
                border-bottom:1pt solid  #ddd;
            }
        </style> 
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
          <telerik:RadScriptManager ID="RadScriptManager1" runat="server" EnableTheming="True">
                <Scripts>
                    <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js">
                    </asp:ScriptReference>
                    <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js">
                    </asp:ScriptReference>
                    <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js">
                    </asp:ScriptReference>
                </Scripts>
            </telerik:RadScriptManager>

            <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;" CssClass="smallToolBar">
                <Items>
                    <telerik:RadToolBarButton runat="server" Value="Item1">
                        <ItemTemplate>
                            <div style="height:22px;margin-top:6px;">
                                <asp:Label ID="LabelPlanHeader" runat="server" Text="Add new plan" CssClass="RadToolbarLabel" Height="22"></asp:Label>
                            </div>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>
                </Items>
            </telerik:RadToolBar>


            <input id="saveConfirm" value="0" type="hidden" name="saveConfirm" runat="server" />
            <div style="DISPLAY: none; FONT-SIZE: 10pt; color: green" id="ProgressBar"><img alt="" src="../images/indicator.gif" />&nbsp;&nbsp;Storing plan..</div>
            <div style="height:12px;"></div>
            <asp:panel id="PanelMainActionButtons" runat="server" Width="600" HorizontalAlign="Left" Wrap="False"  EnableTheming="False">	
				<table id="mainactionbuttontable" style="border: 0px; text-align:left;" cellspacing="1" cellpadding="1" align="left" >
					<tr>
						<td style="width: 200px;text-align:center;">
                           <telerik:RadButton ID="btnAddPlan" runat="server" Text="Add page plan" ToolTip="Add intermediate page plan to system" Skin="Office2010Blue" Width="120"></telerik:RadButton>

                        </td>
						<td>
                            <asp:label id="lblAddPagePlan" runat="server" CssClass="LabelNormal">Add temporary page list plan to system</asp:label>
                        </td>
						
					</tr>
					
					<tr>
						<td style="width: 200px;text-align:center;">
                            <telerik:RadButton id="btnDeletePlan" runat="server" Text="Delete page plan" Enabled="false"
            					ToolTip="Delete intermediate page plan to system" Skin="Office2010Blue" Width="120"></telerik:RadButton>
                        </td>
						<td>
                            <asp:label id="lblDeletePlan" runat="server" CssClass="LabelNormal">
                            Delete temporary page list plan to system</asp:label>
                        </td>
						<td></td>
					</tr>
				</table>
            </asp:panel>
            <asp:label id="lblError" runat="server" Font-Names="Verdana" Font-Size="10pt" ForeColor="Red" Font-Bold="True"></asp:label>
            <asp:label id="lblInfo" runat="server" Font-Names="Verdana" Font-Size="10pt" ForeColor="Blue" Font-Bold="True"></asp:label>
            <input runat="server" type="hidden" id="hiddenUploadPath" value="" />

            <asp:panel id="PanelAddPlan" runat="server"  HorizontalAlign="Left" Wrap="False" >                               
				<table id="mainparametertable" style="border: 1px;text-align: left; width:600px; margin-left:3px;" cellspacing="1" cellpadding="1">								   
					<tr id="publication">
                        <td style="width: 130px; height: 14px; white-space:nowrap;">
	    					<asp:Label id="lblPublication" runat="server" CssClass="LabelNormal">Publication</asp:Label>
                        </td>		    							
						<td style="white-space:nowrap;" colspan="3">										
                            <telerik:RadComboBox id="ddPublicationList" runat="server" Width="220px" AutoPostBack="True" SkinsPath="~/RadControls/ComboBox/Skins" Skin="Default" OnSelectedIndexChanged="ddPublicationList_SelectedIndexChanged"></telerik:RadComboBox>	    									
                        </td>
                    </tr>
					<tr id="pubdateweeknumber">
                        <td style="width: 130px; white-space:nowrap;">
							<asp:Label id="lblPubdate" runat="server" CssClass="LabelNormal">Publication date</asp:Label>
                        </td>
                        <td style="white-space:nowrap;" colspan="3">
							<telerik:RadDatePicker id="dateChooserPubDate" ToolTip="Select publication date" Runat="server" Culture="da-DK"></telerik:RadDatePicker>
                        </td> 
					</tr>
					<tr id="press">
                        <td style="width: 130px; height: 14px; white-space:nowrap;">
	    					<asp:Label id="lblPressGroup" runat="server" CssClass="LabelNormal">Press group</asp:Label>
                        </td>		    							
						<td style="white-space:nowrap;" colspan="3">										
                            <telerik:RadComboBox id="ddPressGroupList" runat="server" Width="130px" AutoPostBack="False" SkinsPath="~/RadControls/ComboBox/Skins" Skin="Default"></telerik:RadComboBox>
                          </td>
                    </tr>
					<tr id="edition">
                        <td style="width: 130px; height: 14px; white-space:nowrap;">
	    					<asp:Label id="lblEdition" runat="server" CssClass="LabelNormal">Edition</asp:Label>
                        </td>		    							
						<td style="white-space:nowrap;" colspan="3">										
                            <telerik:RadComboBox id="ddEditionList" runat="server" Width="130px" AutoPostBack="False" SkinsPath="~/RadControls/ComboBox/Skins" Skin="Default"></telerik:RadComboBox>	    									
                        </td>
                    </tr>
                    <tr class="border_bottom">
						<td colspan="4">
							<img height="2" src="../images/spacer.gif" width="1" alt="" />		
                        </td>						    
					</tr>
                    <tr>
						<td colspan="4">
							<img height="1" src="../images/spacer.gif" width="1" alt="" />		
                        </td>						    
					</tr>

                    <tr id="pageformat">
                        <td style="width: 130px; height: 14px; white-space:nowrap;">
	    					<asp:Label id="lblPageFormat" runat="server" CssClass="LabelNormal">Pageformat</asp:Label>
                        </td>		    							
						<td style="white-space:nowrap;" colspan="3">										
                            <telerik:RadComboBox id="ddPageFormatList" runat="server" Width="130px" AutoPostBack="False" SkinsPath="~/RadControls/ComboBox/Skins" Skin="Default"></telerik:RadComboBox>	    									
                        </td>
                    </tr>
                    <tr id="trim">
                        <td style="width: 130px; height: 14px; white-space:nowrap;">
	    					<asp:Label id="lblTrim" runat="server" CssClass="LabelNormal">Trim</asp:Label>
                        </td>		    							
						<td style="white-space:nowrap;" colspan="3">										
                            <telerik:RadNumericTextBox id="RadNumericTextBoxTrimWidth" Width="115px" CssClass="LabelNormal" Runat="server" Label="Width " Value="0"
									ShowSpinButtons="True" MaxLength="4" MinValue="0" MaxValue="999">
								<NumberFormat AllowRounding="True" GroupSizes="3" DecimalSeparator="," NegativePattern="-n" GroupSeparator="."
									DecimalDigits="0" PositivePattern="n"></NumberFormat>
							</telerik:RadNumericTextBox>
                            <telerik:RadNumericTextBox id="RadNumericTextBoxTrimHeight" Width="115px" CssClass="LabelNormal" Runat="server" Label="Height " Value="0"
									ShowSpinButtons="True" MaxLength="4" MinValue="0" MaxValue="999">
								<NumberFormat AllowRounding="True" GroupSizes="3" DecimalSeparator="," NegativePattern="-n" GroupSeparator="."
									DecimalDigits="0" PositivePattern="n"></NumberFormat>
							</telerik:RadNumericTextBox>
	    					<asp:Label id="Label2" runat="server" CssClass="LabelNormal">  (0,0=ingen trim)</asp:Label>
                        </td>
                    </tr>
                   <tr class="border_bottom">
						<td colspan="4">
							<img height="2" src="../images/spacer.gif" width="1" alt="" />		
                        </td>						    
					</tr>
                    <tr>
						<td colspan="4">
							<img height="1" src="../images/spacer.gif" width="1" alt="" />		
                        </td>						    
					</tr>

                     <tr id="paper">
                        <td style="width: 130px; height: 14px; white-space:nowrap;">
	    					<asp:Label id="lblPaper" runat="server" CssClass="LabelNormal">Paper</asp:Label>
                        </td>		    							
						<td style="white-space:nowrap;" colspan="3">										
                            <telerik:RadComboBox id="ddPaperList" runat="server" Width="130px" AutoPostBack="False" SkinsPath="~/RadControls/ComboBox/Skins" Skin="Default"></telerik:RadComboBox>	    									
                        </td>
                    </tr>
                    <tr id="circulation">
						<td style="width: 130px; height: 1px; white-space:nowrap;">
							<asp:Label id="lblCirculation" runat="server" CssClass="LabelNormal">Circulation</asp:Label>
                        </td>
                        <td style="white-space:nowrap;"  colspan="3">
							<telerik:RadNumericTextBox id="RadNumericTextBoxCirculation" Width="80px" CssClass="LabelNormal" Runat="server"
									ShowSpinButtons="True" MaxLength="9" MinValue="0" MaxValue="10000000">
								<NumberFormat AllowRounding="True" GroupSizes="3" DecimalSeparator="," NegativePattern="-n" GroupSeparator="."
									DecimalDigits="0" PositivePattern="n"></NumberFormat>
								<IncrementSettings Step="100"></IncrementSettings>
							</telerik:RadNumericTextBox>
                        </td>
					</tr>
                    <tr class="border_bottom">
						<td colspan="4">
							<img height="2" src="../images/spacer.gif" width="1" alt="" />		
                        </td>						    
					</tr>
                    <tr>
						<td colspan="4">
							<img height="1" src="../images/spacer.gif" width="1" alt="" />		
                        </td>						    
					</tr>
					
                    <tr id="section1">
                        <td style="width: 130px; height: 14px; white-space:nowrap;">
							<asp:Label id="lblSectionA" runat="server" CssClass="LabelNormal">Section A</asp:Label>
                        </td>
                        <td style="height: 14px; white-space:nowrap;">
                            <telerik:RadNumericTextBox id="RadNumericTextBoxPages1" Width="80px" CssClass="LabelNormal" Runat="server" Label="Pages " Value="0"
									ShowSpinButtons="True" MaxLength="3" MinValue="0" MaxValue="192">
								<NumberFormat AllowRounding="True" GroupSizes="3" DecimalSeparator="," NegativePattern="-n" GroupSeparator="."
									DecimalDigits="0" PositivePattern="n"></NumberFormat>
								<IncrementSettings Step="2"></IncrementSettings>
							</telerik:RadNumericTextBox>
                        </td>
                        <td style="width: 100px; height: 14px; white-space:nowrap;">
							<asp:Label id="lblSectionE" runat="server" CssClass="LabelNormal">Section E</asp:Label>
                        </td>
                        <td style="height: 14px; white-space:nowrap; text-align:left;">
                            <telerik:RadNumericTextBox id="RadNumericTextBoxPages5" Width="80px" CssClass="LabelNormal" Runat="server" Label="Pages" Value="0"
									ShowSpinButtons="True" MaxLength="3" MinValue="0" MaxValue="192">
								<NumberFormat AllowRounding="True" GroupSizes="3" DecimalSeparator="," NegativePattern="-n" GroupSeparator="."
									DecimalDigits="0" PositivePattern="n"></NumberFormat>
								<IncrementSettings Step="2"></IncrementSettings>
							</telerik:RadNumericTextBox>
                        </td>

					</tr>
                    <tr id="section2">
                        <td style="width: 130px; height: 14px; white-space:nowrap;">
							<asp:Label id="lblSectionB" runat="server" CssClass="LabelNormal">Section B</asp:Label>
                        </td>
                        <td style="height: 14px; white-space:nowrap;">
                            <telerik:RadNumericTextBox id="RadNumericTextBoxPages2" Width="80px" CssClass="LabelNormal" Runat="server" Label="Pages " Value="0"
									ShowSpinButtons="True" MaxLength="3" MinValue="0" MaxValue="192">
								<NumberFormat AllowRounding="True" GroupSizes="3" DecimalSeparator="," NegativePattern="-n" GroupSeparator="."
									DecimalDigits="0" PositivePattern="n"></NumberFormat>

								<IncrementSettings Step="2"></IncrementSettings>
							</telerik:RadNumericTextBox>
                        </td>
                        <td style="width: 80px; height: 14px; white-space:nowrap;">
							<asp:Label id="lblSectionF" runat="server" CssClass="LabelNormal">Section F</asp:Label>
                        </td>
                        <td style="height: 14px; white-space:nowrap; text-align:left;">
                            <telerik:RadNumericTextBox id="RadNumericTextBoxPages6" Width="80px" CssClass="LabelNormal" Runat="server" Label="Pages" Value="0"
									ShowSpinButtons="True" MaxLength="3" MinValue="0" MaxValue="192">
								<NumberFormat AllowRounding="True" GroupSizes="3" DecimalSeparator="," NegativePattern="-n" GroupSeparator="."
									DecimalDigits="0" PositivePattern="n"></NumberFormat>
								<IncrementSettings Step="2"></IncrementSettings>
							</telerik:RadNumericTextBox>
                        </td>

					</tr>
                    <tr id="section3">
                        <td style="width: 130px; height: 14px; white-space:nowrap;">
							<asp:Label id="lblSectionC" runat="server" CssClass="LabelNormal">Section C</asp:Label>
                        </td>
                        <td style="height: 14px; white-space:nowrap;">
                            <telerik:RadNumericTextBox id="RadNumericTextBoxPages3" Width="80px" CssClass="LabelNormal" Runat="server" Label="Pages" Value="0" 
									ShowSpinButtons="True" MaxLength="3" MinValue="0" MaxValue="192">
								<NumberFormat AllowRounding="True" GroupSizes="3" DecimalSeparator="," NegativePattern="-n" GroupSeparator="."
									DecimalDigits="0" PositivePattern="n"></NumberFormat>
								<IncrementSettings Step="2"></IncrementSettings>
							</telerik:RadNumericTextBox>
                        </td>
                        <td style="width: 80px; height: 14px; white-space:nowrap;">
							<asp:Label id="lblSectionG" runat="server" CssClass="LabelNormal">Section G</asp:Label>
                        </td>
                        <td style="height: 14px; white-space:nowrap; text-align:left;">
                            <telerik:RadNumericTextBox id="RadNumericTextBoxPages7" Width="80px" CssClass="LabelNormal" Runat="server" Label="Pages" Value="0"
									ShowSpinButtons="True" MaxLength="3" MinValue="0" MaxValue="192">
								<NumberFormat AllowRounding="True" GroupSizes="3" DecimalSeparator="," NegativePattern="-n" GroupSeparator="."
									DecimalDigits="0" PositivePattern="n"></NumberFormat>
								<IncrementSettings Step="2"></IncrementSettings>
							</telerik:RadNumericTextBox>
                        </td>

					</tr>
                    <tr id="section4">
                        <td style="width: 130px; height: 14px; white-space:nowrap;">
							<asp:Label id="lblSectionD" runat="server" CssClass="LabelNormal">Section D</asp:Label>
                        </td>
                        <td style="height: 14px; white-space:nowrap;">
                            <telerik:RadNumericTextBox id="RadNumericTextBoxPages4" Width="80px" CssClass="LabelNormal" Runat="server" Label="Pages" Value="0"
									ShowSpinButtons="True" MaxLength="3" MinValue="0" MaxValue="192">
								<NumberFormat AllowRounding="True" GroupSizes="3" DecimalSeparator="," NegativePattern="-n" GroupSeparator="."
									DecimalDigits="0" PositivePattern="n"></NumberFormat>
								<IncrementSettings Step="2"></IncrementSettings>
							</telerik:RadNumericTextBox>
                        </td>
                        <td style="width: 80px; height: 14px; white-space:nowrap;">
							<asp:Label id="lblSectionH" runat="server" CssClass="LabelNormal">Section H</asp:Label>
                        </td>
                        <td style="height: 14px; white-space:nowrap; text-align:left;">
                            <telerik:RadNumericTextBox id="RadNumericTextBoxPages8" Width="80px" CssClass="LabelNormal" Runat="server" Label="Pages" Value="0"
									ShowSpinButtons="True" MaxLength="3" MinValue="0" MaxValue="192">
								<NumberFormat AllowRounding="True" GroupSizes="3" DecimalSeparator="," NegativePattern="-n" GroupSeparator="."
									DecimalDigits="0" PositivePattern="n"></NumberFormat>
								<IncrementSettings Step="2"></IncrementSettings>
							</telerik:RadNumericTextBox>
                        </td>
					</tr>
                     <tr id="section9">
                        <td style="width: 130px; height: 14px; white-space:nowrap;">
                        </td>
                         <td></td>
                        <td style="height: 14px; white-space:nowrap;">
							<asp:Label id="lblSectionI" runat="server" CssClass="LabelNormal">Section I</asp:Label>
                        </td>
                        <td style="height: 14px; white-space:nowrap;">
                            <telerik:RadNumericTextBox id="RadNumericTextBoxPages9" Width="80px" CssClass="LabelNormal" Runat="server" Label="Pages" Value="0"
									ShowSpinButtons="True" MaxLength="3" MinValue="0" MaxValue="192">
								<NumberFormat AllowRounding="True" GroupSizes="3" DecimalSeparator="," NegativePattern="-n" GroupSeparator="."
									DecimalDigits="0" PositivePattern="n"></NumberFormat>
								<IncrementSettings Step="2"></IncrementSettings>
							</telerik:RadNumericTextBox>
                        </td>
                         <td></td>
                         <td></td>
					</tr>
					<tr class="border_bottom">
						<td colspan="4">
							<img height="2" src="../images/spacer.gif" width="1" alt="" />		
                        </td>						    
					</tr>
                    
					<tr id="comment">
						<td style="width: 130px; height: 53px; white-space: nowrap; vertical-align: central;" align="left">
							<asp:Label id="lblComment" runat="server" CssClass="LabelNormal">Comment</asp:Label>
                        </td>
						<td style="white-space: nowrap;" colspan="3">
							<asp:TextBox id="txtComment" runat="server" Height="40px" Width="470px" ToolTip="Comment related to production e.g. number of copies to print"
								Columns="40" TextMode="MultiLine"></asp:TextBox>
                        </td>
					</tr>
					<tr class="border_bottom">
						<td colspan="4">
							<img height="2" src="../images/spacer.gif" width="1" alt="" />		
                        </td>						    
					</tr>
                    
					<tr id="upload">
						<td style="width: 130px; height: 53px; white-space: nowrap; vertical-align:  central;" align="left">
							<asp:Label id="lblPackageFiles" runat="server" CssClass="LabelNormal">Packaging</asp:Label>
                        </td>
						<td style="white-space: nowrap;" colspan="3">
                            <div style="margin-top:10px;">

                               <telerik:RadAsyncUpload ID="RadAsyncUpload1" runat="server"  MultipleFileSelection="Automatic" 
                                   TemporaryFolder="~/UploadedFiles" TargetFolder="~/UploadedFiles/Saved"  
                                   EnableViewState="true"   Skin="Office2010Blue" 
                                   OnClientFileUploaded="OnClientFileUploaded"  OnClientFileUploadFailed="OnClientFileUploadFailed" 
                                   PostbackTriggers="RadButtonSaveFile" Localization-Select="Browse" OverwriteExistingFiles="true"
                                   HideFileInput="false" ToolTip="Vælg fil(er) der skal oploades"></telerik:RadAsyncUpload>
                            </div>
                            <div style="margin-top:10px;">
                                <telerik:RadButton id="RadButtonSaveFile" runat="server"  Width="80px" EnableViewState="True" Text="Gem fil(er)" Skin="Office2010Blue" OnClick="RadButtonSaveFile_Click" ></telerik:RadButton>
                            </div>
                        </td>
					</tr>
                    <tr>
                        <td style="width: 130px;  white-space: nowrap; vertical-align: central;" align="left">
						</td>
                        <td  colspan="3">
                            <div class="uploadedfiles">
                               <asp:Literal runat="server" ID="ltrNoResults" Visible="True" Text="<strong>Ingen filer tilknyttet</strong>" />
                                <asp:GridView ID="GridView1" runat="server" CellPadding="2" ForeColor="#333333" GridLines="Both" AutoGenerateColumns="False">
                                     <Columns>
                                        <asp:BoundField DataField="FileName" HeaderText="FileName" ReadOnly="True">
                                            <ItemStyle Width="200px" Wrap="False" Height="14" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Size" HeaderText="Size" ReadOnly="True" >
                                            <ItemStyle Width="80px" Wrap="False" Height="14" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="Status" HeaderText="Status" ReadOnly="True" >
                                            <ItemStyle Width="200px" Wrap="False" Height="14" />
                                        </asp:BoundField>
                                    </Columns>
                                    <EditRowStyle BackColor="#999999" />
                                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" Font-Names="Tahoma" Font-Size="Small" />
                                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" Font-Names="Tahoma" Font-Size="Small"/>
                                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" Font-Names="Tahoma" Font-Size="Small" />
                                    <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                    <SortedAscendingCellStyle BackColor="#E9E7E2" />
                                    <SortedAscendingHeaderStyle BackColor="#506C8C" />
                                    <SortedDescendingCellStyle BackColor="#FFFDF8" />
                                    <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                                </asp:GridView>
                            </div>           
                        </td>
                    </tr>
					<tr class="border_bottom">
						<td colspan="4">
							<img height="2" src="../images/spacer.gif" width="1" alt="" />		
                        </td>						    
					</tr>
                    <tr>
						<td colspan="4">
							<img height="1" src="../images/spacer.gif" width="1" alt="" />		
                        </td>						    
					</tr>					                   
					<tr id="save">
						<td style="width: 130px;  white-space: nowrap;"></td>
						<td style="white-space: nowrap; vertical-align: top;" colspan="3" align="left">
							<table id="savecancelbuttonstable" style="width: 470px; height: 50px"  border="0" cellspacing="1" cellpadding="1" width="470">
								<tr>
									<td style="width: 127px" height="40" align="left">
										<telerik:RadButton id="btnSavePlan" runat="server"  Width="80" ToolTip="Add intermediate page plan to system" Text="Save plan" Skin="Office2010Blue"></telerik:RadButton>
                                    </td>
									<td style="WIDTH: 46px" height="40" align="left">
										<telerik:RadButton id="btnCancel" runat="server" Width="80" Text="Cancel" Skin="Office2010Blue"></telerik:RadButton>
                                    </td>
									<td height="40" align="center"></td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</asp:panel>

            <script type="text/javascript">

                if (<%=existingProductionPrompt %> > 0) { 
                    //ret = confirm("Warning - plan is already in system - continue?");
                    ret = confirm("Advarsel - plan er allerede oprettet - forsæt?");
                    if(ret)
                        document.Form1.saveConfirm.value = '1';
                    else
                        document.Form1.saveConfirm.value = '0';
                    document.Form1.submit();
                } 
               
            </script>
        </form>
	</body>
</html>
