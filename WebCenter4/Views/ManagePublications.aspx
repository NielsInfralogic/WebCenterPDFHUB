<%@ Page language="c#" Codebehind="ManagePublications.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.ManagePublications" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Manage Publications</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
		<script  type="text/javascript">
		<!--
			function doClose()
			{
				<% if (doClose>0)  { %>
					opener.document.Form1.HiddenNewPubname.value = '<% =newPubName%>';
					opener.document.Form1.submit();				
					parent.window.close();
				<% } %>
			
			}
		//-->
		</script>
         <style type="text/css">  
            html, body, form  
            {  
                height: 100%;  
                margin: 2px;  
                padding: 0px;  
                overflow: hidden;  
            }  
        </style> 
	</head>
	<body onload="doClose()" >
		<form id="Form1" method="post" runat="server" defaultfocus="txtPublicationName">
                        <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>

                    <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;">
                    <Items>
                        <telerik:RadToolBarButton runat="server" Value="Item1">
                            <ItemTemplate>
                                <asp:Label ID="LabelPublicationsHeader" runat="server" Text="Add new publication name" CssClass="RadToolbarLabel"></asp:Label>
                            </ItemTemplate>
                        </telerik:RadToolBarButton>
                    </Items>
                </telerik:RadToolBar>
    			<table id="Table1" style="width: 490px; height: 268px" cellspacing="1" cellpadding="1" width="490" border="0">
				<tr>
					<td style="width: 204px; height: 8px" align="left"></td>
					<td style="height: 8px" align="left"></td>
				</tr>
				<tr>
					<td style="width: 204px; height: 25px" align="left"><asp:label id="lblNewPublicationName" runat="server" CssClass="LabelNormal">New publication name</asp:label></td>
					<td style="height: 25px" align="left"><asp:textbox id="txtPublicationName" runat="server"></asp:textbox><asp:requiredfieldvalidator id="RequiredFieldValidator1" runat="server" ControlToValidate="txtPublicationName"
							Display="Dynamic" ErrorMessage="Required"></asp:requiredfieldvalidator></td>
				</tr>
				<tr>
					<td style="width: 204px; height: 8px" align="left"><asp:label id="lblPageFormat" runat="server" CssClass="LabelNormal">Default page format</asp:label></td>
					<td style="height: 8px" align="left"><asp:dropdownlist id="ddlPageFormat" runat="server" Width="152px"></asp:dropdownlist><asp:linkbutton id="LinkButtonAddNewPageformat" runat="server" CausesValidation="False">Add new</asp:linkbutton></td>
				</tr>
				<tr>
					<td style="width: 204px; height: 8px" align="left"><asp:label id="lblTrimToFormat" runat="server" CssClass="LabelNormal">Trim to page format</asp:label></td>
					<td style="height: 8px" align="left"><asp:checkbox id="cbTrimToFormat" runat="server"></asp:checkbox></td>
				</tr>
				<tr>
					<td style="width: 204px; height: 8px" align="left"><asp:label id="lblLatestHour" runat="server" CssClass="LabelNormal">Production lock time before pubdate (hours)</asp:label></td>
					<td style="height: 8px" align="left"><asp:textbox id="txtLatestHour" runat="server" Width="80px"></asp:textbox><asp:label id="lbl0disabled" runat="server" CssClass="LabelNormal">(0: disabled)</asp:label></td>
				</tr>
				<tr>
					<td style="width: 204px; height: 8px" align="left"><asp:label id="lblDefaultSoftproof" runat="server" CssClass="LabelNormal">Default softproof method</asp:label></td>
					<td style="height: 8px" align="left"><asp:dropdownlist id="ddlProofer" runat="server" Width="152px"></asp:dropdownlist></td>
				</tr>
				<tr>
					<td style="width: 204px; height: 8px" align="left"><asp:label id="lblDefaultHardproof" runat="server" CssClass="LabelNormal">Default hardproof method</asp:label></td>
					<td style="height: 8px" align="left"><asp:dropdownlist id="ddHardProofer" runat="server" Width="152px" Enabled="False">
							<asp:ListItem Value="none">none</asp:ListItem>
						</asp:dropdownlist></td>
				</tr>
				<tr>
					<td style="width: 204px; height: 27px" align="left"><asp:label id="lblApproveMethod" runat="server" CssClass="LabelNormal">Default approve method</asp:label></td>
					<td style="height: 13px" align="left"><asp:radiobuttonlist id="RadioButtonListApprove" runat="server" Height="8px" Width="288px" RepeatLayout="Flow"
							CellPadding="0" CellSpacing="5" RepeatDirection="Horizontal" CssClass="LabelNormal">
							<asp:ListItem Value="Must approve" Selected="True">Must approve</asp:ListItem>
							<asp:ListItem Value="No approval required">No approval required</asp:ListItem>
						</asp:radiobuttonlist></td>
				</tr>
				<tr>
					<td style="height: 17px" align="center" colspan="2"><asp:label id="lblError" runat="server" Font-Size="10pt" ForeColor="Red"></asp:label></td>
				</tr>
				<tr>
					<td valign="top" align="center" colspan="2">
						<table id="Table2" cellspacing="0" cellpadding="1" width="300" border="0">
							<tr>
								<td align="center"><asp:button id="btnSave" runat="server" BackColor="LightBlue" Height="24px" Width="60px" Text="Save"
										EnableViewState="False"></asp:button></td>
								<td align="center"><asp:button id="btnCancel" runat="server" BackColor="LightGray" Height="24px" Width="60px" Text="Cancel"
										EnableViewState="False" CausesValidation="False"></asp:button></td>
							</tr>
						</table>
						<input id="HiddenNewPageformat" type="hidden" value="0" name="HiddenNewPageformat" runat="server" /></td>
				</tr>
			</table>
		</form>
	</body>
</html>
