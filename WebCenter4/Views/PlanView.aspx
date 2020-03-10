<%@ Page language="c#" Codebehind="PlanView.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.PlanView" %>

<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Plans</title>
		<link rel="stylesheet" type="text/css" href="../Styles/WebCenter.css" />
		<script language="JavaScript" type="text/javascript"> 
		    if ( <%=updateTree %> > 0) { 
			    parent.parent.tree.document.Form1.action = 'Tree.aspx?refresh=1';
			    parent.parent.tree.document.Form1.submit();
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
        <style type="text/css">  
            html, body, form  
            {  
                margin: 0px;  
                padding: 0px;  
            }  
        </style> 
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

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

            <input id="HiddenNewPageformat" value="0" type="hidden" name="HiddenNewPageformat" runat="server"/>
            <input id="HiddenNewPubname" value="0" type="hidden" name="HiddenNewPubname" runat="server" />
            <input id="saveConfirm2" value="0" type="hidden" name="saveConfirm2" runat="server" />
            <input id="saveConfirm" value="0" type="hidden" name="saveConfirm" runat="server" />
            <div style="DISPLAY: none; FONT-SIZE: 10pt; color: green" id="ProgressBar"><img alt="" src="../images/indicator.gif" />&nbsp;&nbsp;Storing plan..</div>
            <div style="height:12px;"></div>
            <asp:panel id="PanelMainActionButtons" runat="server" Width="800" HorizontalAlign="Left" Wrap="False"  EnableTheming="False">	
				<table id="mainactionbuttontable" style="border: 0px; text-align:left;" cellspacing="1" cellpadding="1" align="left">
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
                           <telerik:RadButton ID="btnUploadPlan" runat="server" Text="Upload page plan" ToolTip="Upload page plan to system" Skin="Office2010Blue" Width="120" OnClick="btnUploadPlan_Click"></telerik:RadButton>

                        </td>
						<td>
                            <asp:label id="lblUploadPlan" runat="server" CssClass="LabelNormal">Upload temporary page list plan to system</asp:label>
                        </td>
						
					</tr>
					<tr>
						<td style="width: 200px;text-align:center;">
                            <telerik:RadButton id="btnEditPlan" runat="server" Text="Edit page plan"
	            				ToolTip="Edit existing intermediate page plan to system" Skin="Office2010Blue" Visible="False" Width="120"></telerik:RadButton>
                        </td>
						<td>
                            <asp:label id="lblEditPlan" runat="server" CssClass="LabelNormal" Visible="False">Edit existing temporary page list plan to system</asp:label>
                        </td>
						<td></td>
					</tr>
					<tr>
						<td style="width: 200px;text-align:center;">
                            <telerik:RadButton id="btnDeletePlan" runat="server" Text="Delete page plan"
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
            <asp:panel id="PanelDeletePlan" runat="server" Width="800" HorizontalAlign="Left" Wrap="False" >							   
				<div>
					<asp:DataGrid id="DataGridProductionList" runat="server" BackColor="White" BorderStyle="None"
							BorderWidth="1px" CssClass="LabelNormal" AutoGenerateColumns="False" GridLines="Vertical"
							CellPadding="3" BorderColor="#999999" AllowPaging="True">
						<FooterStyle ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
						<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
						<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
						<ItemStyle Wrap="False" ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
						<HeaderStyle Font-Bold="True" ForeColor="White" BackColor="#000084"></HeaderStyle>
						<Columns>
							<asp:ButtonColumn Text="&lt;img src='./../Images/wastewhite.gif' onclick='return confirm_delete();'&gt;"
								CommandName="Delete">
								<ItemStyle Wrap="False"></ItemStyle>
							</asp:ButtonColumn>
							<asp:BoundColumn DataField="Name" SortExpression="Name" HeaderText="Production"></asp:BoundColumn>
							<asp:BoundColumn Visible="False" DataField="ID" HeaderText="ID"></asp:BoundColumn>
						</Columns>
						<PagerStyle HorizontalAlign="Center" ForeColor="Black" BackColor="#999999" Mode="NumericPages"></PagerStyle>
					</asp:DataGrid>
                </div>
				<div style="text-align: left; padding-top: 12px;">
					<telerik:RadButton id="btlCloseDeletePlan" runat="server" Text="Close" Skin="Office2010Blue"></telerik:RadButton>
                </div>
			</asp:panel>
            <asp:panel id="PanelEditPlan" runat="server" Width="800" HorizontalAlign="Left" Wrap="False"  >
                <div>
					<asp:DataGrid id="DataGridProductionListEdit" runat="server" BackColor="White" BorderStyle="None"
							BorderWidth="1px" CssClass="LabelNormal" AutoGenerateColumns="False" GridLines="Vertical"
							CellPadding="3" BorderColor="#999999" AllowPaging="True">
						<FooterStyle ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
						<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
						<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
						<ItemStyle Wrap="False" ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
						<HeaderStyle Font-Bold="True" ForeColor="White" BackColor="#000084"></HeaderStyle>
						<Columns>
							<asp:ButtonColumn Text="&lt;img src='./../Images/edit.gif' &gt;" CommandName="Edit">
								<ItemStyle Wrap="False"></ItemStyle>
							</asp:ButtonColumn>
							<asp:BoundColumn DataField="Name" SortExpression="Name" HeaderText="Production"></asp:BoundColumn>
							<asp:BoundColumn Visible="False" DataField="ID" HeaderText="ID"></asp:BoundColumn>
						</Columns>
						<PagerStyle HorizontalAlign="Center" ForeColor="Black" BackColor="#999999" Mode="NumericPages"></PagerStyle>
					</asp:DataGrid>
                </div>
				<div style="text-align: left; padding-top: 12px;">
					<telerik:RadButton id="btlCloseEditPlan" runat="server" Text="Close" Skin="Office2010Blue"></telerik:RadButton>
                </div>						
			</asp:panel>        
            <asp:panel id="PanelAddPlan" runat="server" Width="900" HorizontalAlign="Left" Wrap="False" >                               
				<table id="mainparametertable" style="border: 1px;text-align: left; width:inherit;" cellspacing="1" cellpadding="1">								   
					<tr id="publication">
                        <td style="width: 167px; height: 14px; white-space:nowrap;">
	    					<asp:Label id="lblPublication" runat="server" CssClass="LabelNormal">Publication</asp:Label>
                        </td>		    							
						<td style="width: 247px; white-space:nowrap;">										
                            <telerik:RadComboBox id="ddPublicationList" runat="server" Width="230px" AutoPostBack="True" SkinsPath="~/RadControls/ComboBox/Skins" Skin="Default" OnSelectedIndexChanged="ddPublicationList_SelectedIndexChanged"></telerik:RadComboBox>	    									
                        </td>
                        <td style="width: 161px; height: 14px; white-space:nowrap;"><asp:LinkButton id="LinkButtonAddNewPublication" runat="server">Add new</asp:LinkButton></td>
                        <td style="width: 247px; height: 14px; white-space:nowrap;"></td>
                    </tr>
                    <tr id="pageformat">
                        <td style="width: 167px; height: 14px; white-space:nowrap;">
							<asp:Label id="lblPlanPageFormat" runat="server" CssClass="LabelNormal" Enabled="True">Page format</asp:Label>
                        </td>
                        <td style="width: 247px; height: 14px; white-space:nowrap;">
							<telerik:RadComboBox id="ddPageFormatList" runat="server" Width="180px" SkinsPath="~/RadControls/ComboBox/Skins" Skin="Default"></telerik:RadComboBox>	    									
                        </td>
                            <td style="width: 161px; height: 14px; white-space:nowrap;"><asp:LinkButton id="LinkButtonAddNewPageformat" runat="server" Visible="True" Enabled="True">Add new</asp:LinkButton></td>
                        <td style="width: 247px; height: 14px; white-space:nowrap;"></td>
					</tr>
					<tr id="pubdateweeknumber">
                        <td style="width: 167px; white-space:nowrap;">
							<asp:Label id="lblPubdate" runat="server" CssClass="LabelNormal">Publication date</asp:Label>
                        </td>
                        <td style="width: 247px; white-space:nowrap;">
							<telerik:RadDatePicker id="dateChooserPubDate" ToolTip="Select publication date" Runat="server" Culture="da-DK"></telerik:RadDatePicker>
                        </td> 
                        <td style="width: 161px; white-space:nowrap;">
							<asp:Label id="lblWeekNumber" runat="server" CssClass="LabelNormal">Week number</asp:Label>
                        </td>
						<td style="width: 247px; white-space:nowrap;">
							<telerik:RadNumericTextBox id="txtWeekNumber" Width="58px" CssClass="LabelNormal" AutoPostBack="True" Runat="server"
									ShowSpinButtons="True" MaxLength="2" MinValue="0" MaxValue="52" Culture="(Default)">
								<NumberFormat AllowRounding="True" GroupSizes="3" DecimalSeparator="," NegativePattern="-n" GroupSeparator="."
									DecimalDigits="0" PositivePattern="n"></NumberFormat>
							</telerik:RadNumericTextBox>
							<asp:Label id="lblWeekNumber2" runat="server" CssClass="LabelNormal">(0: Not used)</asp:Label>
                        </td>
					</tr>
					<tr id="circulation">
						<td style="width: 167px; height: 1px; white-space:nowrap;">
							<asp:Label id="lblCirculation" runat="server" CssClass="LabelNormal">Circulation</asp:Label>
                        </td>
                        <td style="width: 247px; white-space:nowrap;">
							<telerik:RadNumericTextBox id="RadNumericTextBoxCirculation" Width="120px" CssClass="LabelNormal" Runat="server"
									ShowSpinButtons="True" MaxLength="9" MinValue="0" MaxValue="10000000">
								<NumberFormat AllowRounding="True" GroupSizes="3" DecimalSeparator="," NegativePattern="-n" GroupSeparator="."
									DecimalDigits="0" PositivePattern="n"></NumberFormat>
								<IncrementSettings Step="100"></IncrementSettings>
							</telerik:RadNumericTextBox>
                        </td>

						<td style="width: 161px; white-space:nowrap;">
							<asp:Label id="lblCirculation2" runat="server" CssClass="LabelNormal">Additional circulation</asp:Label>
                        </td>
						<td style="width: 247px; height: 1px; white-space: nowrap;">
							<telerik:RadNumericTextBox id="RadNumericTextBoxCirculation2" Width="120px" Runat="server" ShowSpinButtons="True"
									MaxLength="9" MinValue="0" MaxValue="10000000">
								<NumberFormat AllowRounding="True" GroupSizes="3" DecimalSeparator="," NegativePattern="-n" GroupSeparator="."
									DecimalDigits="0" PositivePattern="n"></NumberFormat>
								<IncrementSettings Step="100"></IncrementSettings>
							</telerik:RadNumericTextBox>
                        </td>
					</tr>
					<tr id="comment">
						<td style="width: 167px; height: 53px; white-space: nowrap; vertical-align: top;" align="left">
							<asp:Label id="lblComment" runat="server" CssClass="LabelNormal">Comment</asp:Label>
                        </td>
						<td style="white-space: nowrap;" colspan="3">
							<asp:TextBox id="txtComment" runat="server" Height="40px" Width="600px" ToolTip="Comment related to production e.g. number of copies to print"
								Columns="40" TextMode="MultiLine"></asp:TextBox>
                        </td>
					</tr>
					<tr id="deadlineinfo">
						<td style="width: 167px; height: 23px; white-space: nowrap; vertical-align: top;" align="left">
							<asp:Label id="lblDeadline" runat="server" CssClass="LabelNormal">Deadline</asp:Label>
                        </td>
						<td style="height: 23px; white-space: nowrap;" colspan="3">
							<asp:Label id="lblDeadlineInfo" runat="server" CssClass="LabelNormal"></asp:Label>
                        </td>
					</tr>
					<tr id="approvalchoise">
						<td style="width: 167px; height: 23px; white-space: nowrap; vertical-align: top" align="left">
							<asp:Label id="lblApproval" runat="server" CssClass="LabelNormal">Approval required</asp:Label>
                        </td>
						<td style="height: 23px; white-space: nowrap;" colspan="3">
							<asp:CheckBox id="checkApprovalRequired" runat="server" CssClass="LabelNormal"></asp:CheckBox>
                        </td>
					</tr>
					<tr id="planupdatesettings">
						<td style="width: 167px; height: 23px; white-space: nowrap; vertical-align: top;" align="left">
							<asp:Label id="LblPlanUpdate" runat="server" CssClass="LabelNormal">Plan update settings</asp:Label>
                        </td>
						<td style="height: 23px; white-space: nowrap;" colspan="3">
							<table style="width: 600px; height: 20px" id="Table7" border="0" cellspacing="0" cellpadding="0" width="600" align="left">
								<tr>
									<td style="white-space: nowrap;">
										<asp:CheckBox id="cbKeepColors" runat="server" Text="Keep existing colors" CssClass="LabelNormal"></asp:CheckBox>
                                    </td>
									<td style="white-space: nowrap;">
										<asp:CheckBox id="cbKeepApproval" runat="server" Text="Keep existing approval" CssClass="LabelNormal"></asp:CheckBox>
                                    </td>
									<td style="white-space: nowrap;">
										<asp:CheckBox id="cbKeepUnique" runat="server" Text="Keep unique/common page settings" CssClass="LabelNormal"></asp:CheckBox>
                                    </td>
								</tr>
							</table>
						</td>
					</tr>
					<tr id="spacer1">
						<td colspan="4">
							<img height="10" src="../images/spacer.gif" width="1" alt="" />		
                        </td>						    
					</tr>
					<tr id="combinesections">
						<td style="width: 167px; height: 8px"></td>
						<td style="height: 8px; white-space: nowrap;" colspan="2">
							<asp:CheckBox id="CheckBoxCombineSections" runat="server" Text="Combine sections to one press run"
								CssClass="LabelNormal"></asp:CheckBox>
                        </td>
						<td></td>
					</tr>
                    <tr id="presseditionmatrix">
    					<td style="width: 167px; height: 8px; vertical-align:top; white-space: nowrap;" align="left">
							<asp:Label id="lblPressEditionMatrix" runat="server" CssClass="LabelNormal" Visible="True">Press/edition matrix</asp:Label>
                        </td>
	    				<td style="vertical-align: top; white-space: nowrap;" colspan="3" align="left">
							<asp:DataGrid id="DataGridPressEditionMatrix" runat="server" BackColor="White" BorderStyle="None" Width="430px"
										BorderWidth="1px" CssClass="LabelNormal" AutoGenerateColumns="False" GridLines="Vertical" CellPadding="3" BorderColor="#999999" OnItemDataBound="DataGridPressEditionMatrix_ItemDataBound">
								<FooterStyle ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
								<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
								<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
								<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
								<HeaderStyle Font-Bold="True" ForeColor="White" BackColor="#000084"></HeaderStyle>
								<Columns>
									<asp:BoundColumn DataField="Edition" ReadOnly="True" HeaderText="Edition"></asp:BoundColumn>
                                    <asp:TemplateColumn HeaderText="Press1">
										<ItemTemplate>
											<asp:CheckBox id="Press1" runat="server"></asp:CheckBox>
										</ItemTemplate>
									</asp:TemplateColumn>

                                    <asp:TemplateColumn HeaderText="Press2">
										<ItemTemplate>
											<asp:CheckBox id="Press2" runat="server"></asp:CheckBox>
										</ItemTemplate>
									</asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Press3">
										<ItemTemplate>
											<asp:CheckBox id="Press3" runat="server"></asp:CheckBox>
										</ItemTemplate>
									</asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Press4">
										<ItemTemplate>
											<asp:CheckBox id="Press4" runat="server"></asp:CheckBox>
										</ItemTemplate>
									</asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Press5">
										<ItemTemplate>
											<asp:CheckBox id="Press5" runat="server"></asp:CheckBox>
										</ItemTemplate>
									</asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Press6">
										<ItemTemplate>
											<asp:CheckBox id="Press6" runat="server"></asp:CheckBox>
										</ItemTemplate>
									</asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Press7">
										<ItemTemplate>
											<asp:CheckBox id="Press7" runat="server"></asp:CheckBox>
										</ItemTemplate>
									</asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Press8">
										<ItemTemplate>
											<asp:CheckBox id="Press8" runat="server"></asp:CheckBox>
										</ItemTemplate>
									</asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Press9">
										<ItemTemplate>
											<asp:CheckBox id="Press9" runat="server"></asp:CheckBox>
										</ItemTemplate>
									</asp:TemplateColumn>
                                    <asp:TemplateColumn HeaderText="Press10">
										<ItemTemplate>
											<asp:CheckBox id="Press10" runat="server"></asp:CheckBox>
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
							</asp:DataGrid>
						</td>
					</tr>
                        <tr id="spacer2">
						<td colspan="4">
							<img height="10" src="../images/spacer.gif" width="1" alt="" />		
                        </td>						    
					</tr>
					<tr id="sectionpagecount">
						<td style="width: 167px; vertical-align: top;" align="left">
							<asp:Label id="lblSections" runat="server" CssClass="LabelNormal">Section(s)</asp:Label>
                        </td>
						<td style="white-space: nowrap; vertical-align: top;" colspan="3" align="left">
							<asp:DataGrid id="DataGridSections" runat="server" BackColor="White" BorderStyle="None" Width="640px"
								BorderWidth="1px" CssClass="LabelNormal" AutoGenerateColumns="False" GridLines="Vertical"
								CellPadding="3" BorderColor="#999999">
								<FooterStyle ForeColor="Black" CssClass="LabelNormal" BackColor="#CCCCCC"></FooterStyle>
								<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
								<EditItemStyle CssClass="LabelNormal"></EditItemStyle>
								<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
								<ItemStyle ForeColor="Black" CssClass="LabelNormal" BackColor="#EEEEEE"></ItemStyle>
								<HeaderStyle ForeColor="White" CssClass="LabelNormal" BackColor="#000084"></HeaderStyle>
								<Columns>
									<asp:TemplateColumn HeaderText="Use">
										<ItemTemplate>
											<asp:CheckBox id="CheckBoxUseSection" runat="server" ToolTip="Add this section to the plan" Width="60"></asp:CheckBox>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:BoundColumn DataField="Section" HeaderText="Section">
										<ItemStyle Wrap="False" HorizontalAlign="Center" Width="60"></ItemStyle>
									</asp:BoundColumn>
									<asp:TemplateColumn HeaderText="Pages">
										<ItemTemplate>
											<asp:TextBox id="txtNumberOfPages" runat="server" ToolTip="Enter number of pages in section"
												Width="48px" Columns="4"></asp:TextBox>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Offset">
										<ItemTemplate>
											<asp:TextBox id="txtPageOffset" runat="server" Width="48px" Columns="4"></asp:TextBox>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Pagename prefix">
										<ItemTemplate>
											<asp:TextBox id="txtPagePrefix" runat="server" Width="48px" Columns="4"></asp:TextBox>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Pagename postfix">
										<ItemTemplate>
											<asp:TextBox id="txtPagePostfix" runat="server" Width="48px" Columns="4"></asp:TextBox>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Colors">
										<ItemTemplate>
											<table id="Table4" cellspacing="1" cellpadding="1" width="100%" border="0">
												<tr>
													<td>
														<asp:RadioButtonList id="RadioButtonListColorMode" runat="server" Width="300px" CssClass="LabelNormal"
															RepeatDirection="Horizontal">
															<asp:ListItem Value="CMYK" Selected="True">CMYK</asp:ListItem>
															<asp:ListItem Value="Mono">Mono</asp:ListItem>
															<asp:ListItem Value="PDF">PDF</asp:ListItem>
																		                
														</asp:RadioButtonList></td>
																               
												</tr>
											</table>
										</ItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn Visible="False" HeaderText="Copies">
										<ItemTemplate>
											<asp:TextBox id="txtCopies" runat="server" Font-Names="Verdana" Font-Size="10pt" Width="48px"
												Columns="3"></asp:TextBox>
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
								<PagerStyle HorizontalAlign="Center" ForeColor="Black" BackColor="#999999" Mode="NumericPages"></PagerStyle>
							</asp:DataGrid>
                        </td>
					</tr>
                     <tr id="spacer21">
						<td colspan="4">
							<img height="10" src="../images/spacer.gif" width="1" alt="" />		
                        </td>						    
					</tr>
					<tr id="specialeditionbutton">
						<td style="width: 167px;" ></td>
						<td colspan="3">
						<telerik:RadButton id="btnSpecialEditions" runat="server" 
								ToolTip="Adjust special common/unique page relationships in editions" Text="Special edition pages" Skin="Office2010Blue"></telerik:RadButton>
                        </td>
					</tr>
					<tr id="save">
						<td style="width: 167px;  white-space: nowrap;"><img height="8" src="../images/spacer.gif" width="1" alt="" /></td>
						<td style="white-space: nowrap; vertical-align: top;" colspan="3" align="left">
							<table id="savecancelbuttonstable" style="width: 728px; height: 83px"  border="0" cellspacing="1" cellpadding="1" width="728">
								<tr>
									<td style="width: 127px" height="40" align="left"></td>
									<td style="width: 46px" height="40" align="left"></td>
									<td height="40" align="left"></td>
								</tr>
								<tr>
									<td style="width: 127px" height="40" align="left">
										<telerik:RadButton id="btnSavePlan" runat="server" ToolTip="Add intermediate page plan to system" Text="Save plan" Skin="Office2010Blue"></telerik:RadButton>
                                    </td>
									<td style="WIDTH: 46px" height="40" align="left">
										<telerik:RadButton id="btnCancel" runat="server" Text="Cancel" Skin="Office2010Blue"></telerik:RadButton>
                                    </td>
									<td height="40" align="center"></td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</asp:panel>
                 

            <telerik:RadWindowManager id="RadWindowManager1" runat="server" Skin="Vista">
				<Windows>
                    <telerik:RadWindow runat="server" Height="610px" Width="460"  ID="radWindowSpecialEditionPages"
						Skin="Office2010Blue" DestroyOnClose="True" Left="" NavigateUrl="AddEditionInfo.aspx" Behaviors="Close"
						VisibleStatusbar="False" Modal="True" Top="" Title="Special edition pages" MaxHeight="1200" MaxWidth="1600">
                    </telerik:RadWindow>
                    <telerik:RadWindow runat="server" Height="300px" Width="500"  ID="radWindowUploadPlan"
						 Skin="Office2010Blue" DestroyOnClose="True" Left="" NavigateUrl="UploadPlan.aspx" Behaviors="Close" VisibleStatusbar="False"
						Modal="True" Top="" Title="Upload plan file" >
                    </telerik:RadWindow>
				</Windows>
			</telerik:RadWindowManager>                 
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
                if (<%=existingProductionPrompt2 %> > 0) { 
                    //ret = confirm("Warning - plan is already in system - continue?");
                    ret = confirm("Advarsel - plan er allerede oprettet - forsæt?");
                    if(ret)
                        document.Form1.saveConfirm2.value = '1';
                    else
                        document.Form1.saveConfirm2.value = '0';
                    document.Form1.submit();
                }
                if (<%=oddpagecountPrompt %> > 0) { 			        //ret = confirm("Warning - there is an odd number of pages in section - continue?");
                    ret = confirm("Advarsel - der er et ulige antal sider definereret i sektion. Fortsæt?");
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
