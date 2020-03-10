<%@ Page language="c#" Codebehind="ManagePageFormats.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.ManagePageFormats" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Manage Page formats</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
		<script  type="text/javascript">
		    function doClose()
			{
				<% if (doClose>0)  { %>
					opener.document.Form1.HiddenNewPageformat.value = '<% =newPageformatName %>';
					opener.document.Form1.submit();				
					parent.window.close();
				<% } %>
			
			}
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
		<form id="Form1" method="post" runat="server" defaultfocus="txtPageformatName">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>


            <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;">
                <Items>
                    <telerik:RadToolBarButton runat="server" Value="Item1">
                        <ItemTemplate>
                            <asp:Label ID="LabelPageformatHeader" runat="server" Text="Add new page format" CssClass="RadToolbarLabel"></asp:Label>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>
                </Items>
            </telerik:RadToolBar>
                     
			<table id="Table1" style="width: 480px; height: 216px;" cellspacing="1" cellpadding="1" width="490" border="0" > 
				<tr>
					<td style="width: 204px; height: 8px" align="left"></td>
					<td style="height: 8px" align="left"></td>
				</tr>
				<tr>
					<td style="width: 204px; height: 8px" align="left">
                        <asp:label id="lblPageformatName" runat="server" CssClass="LabelNormal">New page format name</asp:label>
					</td>
					<td style="height: 8px" align="left">
                        <asp:textbox id="txtPageformatName" runat="server"></asp:textbox>
                        <

					</td>
				</tr>
				<tr>
                    
					<td style="width: 204px; height: 8px" align="left"><asp:label id="lblPageformatWidth" runat="server" CssClass="LabelNormal">Page format width (mm)</asp:label></td>
					<td style="height: 8px" align="left">
                        <telerik:RadNumericTextBox ID="RadNumericEditWidth" runat="server" ToolTip="Trimmed page width (without bleed)" Type="Number"></telerik:RadNumericTextBox>
                            

					</td>
				</tr>
				<tr>
					<td style="width: 204px; height: 24px" align="left">
                        <asp:label id="lblPageformatHeight" runat="server" CssClass="LabelNormal">Page format height (mm)</asp:label></td>
					<td style="height: 24px" align="left">
                        <telerik:RadNumericTextBox ID="RadNumericEditHeight" runat="server" ToolTip="Trimmed page height (without bleed)" Type="Number"></telerik:RadNumericTextBox>
                       
				</tr>
				<tr>
					<td style="width: 204px; height: 8px" align="left">
                        <asp:label id="lblPageformatBleed" runat="server" CssClass="LabelNormal">Bleed margin (mm)</asp:label></td>
					<td style="height: 8px" align="left">
                        <telerik:RadNumericTextBox ID="RadNumericEditBleed" runat="server" Type="Number"></telerik:RadNumericTextBox>
                   </td>
				</tr>
				<tr>
					<td style="width: 204px; height: 13px" align="left"></td>
					<td style="height: 13px" align="left"></td>
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
					</td>
				</tr>
			</table>
		</form>
	</body>
</html>
