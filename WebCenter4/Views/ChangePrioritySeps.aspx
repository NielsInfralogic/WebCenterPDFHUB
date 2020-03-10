<%@ Page language="c#" Codebehind="ChangePrioritySeps.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.ChangePrioritySeps" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
  <head>
		<title>Change priority</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
		<script  type="text/javascript">
		<!--
			function doClose()
			{
				<% if (doClosePrio==2)  { %>
			    opener.document.Form1.HiddenReturendFromPopup.value = '1';
					opener.document.Form1.submit();
					parent.window.close();
				<% } %>
				<% if (doClosePrio==1)  { %>
			    opener.document.Form1.HiddenReturendFromPopup.value = '0';
					parent.window.close();
				<% } %>
			}
		//-->
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

			<table id="Table1" cellspacing="1" cellpadding="1" width="240" border="0" style="WIDTH: 240px; HEIGHT: 88px">
				<tr>
					<td style="WIDTH: 42px">
						<asp:Label id="lblPriority" runat="server" Font-Names="Verdana" Font-Size="10pt">Priority</asp:Label></td>
					<td>
						<asp:TextBox id="txtPrioValue" runat="server" ToolTip="Priority values: 100:highest, 0:lowest"
							Width="48px"></asp:TextBox></td>
				</tr>
				<tr>
					<td height="14" style="WIDTH: 42px"></td>
					<td height="14">
						<asp:CheckBox id="CheckBoxAllColors" runat="server" Text="All page colors" Font-Names="Verdana"
							Font-Size="10pt"></asp:CheckBox></td>
				</tr>
				<tr>
					<td height="14" style="WIDTH: 42px"></td>
					<td height="14">
						<asp:CheckBox id="CheckBoxAllPages" runat="server" Text="All pages on plate" Font-Names="Verdana"
							Font-Size="10pt"></asp:CheckBox></td>
				</tr>
				<tr>
					<td height="14" style="WIDTH: 42px"></td>
					<td height="14">
						<asp:CheckBox id="CheckBoxAllCopies" runat="server" Text="All copies" Font-Names="Verdana" Font-Size="10pt"></asp:CheckBox></td>
				</tr>
				<tr>
					<td height="14" style="WIDTH: 42px"></td>
					<td height="14"></td>
				</tr>
				<tr>
					<td colspan="2" align="center">
						<table id="Table2" cellspacing="1" cellpadding="1" width="200" border="0">
							<tr>
								<td align="center">

                                <td align="center">
									<telerik:RadButton id="bntApply" runat="server" Text="Apply" EnableViewState="False" Skin="Office2010Blue" OnClick="bntApply_Click"
										Width="60px" ></telerik:RadButton></td>
								<td align="center">
									<telerik:RadButton id="btnCancel" runat="server" Text="Cancel" EnableViewState="False" Skin="Office2010Blue" OnClick="btnCancel_Click"
										Width="60px" ></telerik:RadButton></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
	</body>
</html>
