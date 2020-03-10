<%@ Page language="c#" Codebehind="ChangeTemplate.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.ChangeTemplate" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>ChangeTemplate</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
		<script  type="text/javascript">
		
			function doClose()
			{
				<% if (doCloseTemplate==2)  { %>
			    opener.document.Form1.HiddenReturendFromPopup.value = '1';
					opener.document.Form1.submit();
					parent.window.close();
				<% } %>
				
				<% if (doCloseTemplate==1)  { %>
			    opener.document.Form1.HiddenReturendFromPopup.value = '0';
					parent.window.close();
				<% } %>
			}
		
		</script>
        <style type="text/css">  
            html, body, form  
            {  
                height: 100%;  
                margin: 5px;  
                padding: 0px;  
                overflow: hidden;  
            }  
        </style> 
	</head>
	<body onload="doClose()">
		<form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>

			<table id="Table1" cellspacing="1" cellpadding="1" width="300" border="0" align="left">
				<tr>
					<td>
						<asp:Label id="lblTemplate" runat="server" Font-Names="Verdana" Font-Size="10pt">Template</asp:Label></td>
					<td>
						<asp:DropDownList id="DropDownList1" runat="server"></asp:DropDownList></td>
				</tr>
				<tr>
					<td></td>
					<td>
						<asp:CheckBox id="CheckBoxAllCopies" runat="server" Text="All copies" Font-Names="Verdana" Font-Size="10pt"
							ToolTip="Apply template change to all copies"></asp:CheckBox></td>
				</tr>
				<tr>
					<td colspan="2">
						<table id="Table2" cellspacing="1" cellpadding="1" width="200" border="0" align="center">
							<tr>
								<td align="center">
									<telerik:RadButton id="bntApply" runat="server"  EnableViewState="False" OnClick="bntApply_Click"
										Text="Apply" Width="70px" Skin="Office2010Blue"></telerik:RadButton></td>
								<td align="center">
									<telerik:RadButton id="btnCancel" runat="server"  EnableViewState="False" OnClick="btnCancel_Click" 
										Text="Cancel" Width="70px" Skin="Office2010Blue"></telerik:RadButton></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
	</body>
</html>
