<%@ Page language="c#" Codebehind="Error.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Error" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Error</title>
		<link rel="shortcut icon" href="images/IL.ico" />
		<link href="Styles/WebCenter.css" type="text/css" rel="stylesheet" />
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<table id="Table1" cellspacing="1" cellpadding="1" width="534" align="center" border="0"
				style="WIDTH: 534px; HEIGHT: 107px">
				<tr>
					<td valign="middle" align="center">
						<asp:Label id="Label1" runat="server" Font-Bold="True" ForeColor="Red">Error occured reading database</asp:Label></td>
				</tr>
				<tr>
					<td valign="middle" align="center">
						<asp:Label id="lblMessage" runat="server" Font-Bold="True" ForeColor="Red">Message</asp:Label></td>
				</tr>
				<tr>
					<td align="center">
						<asp:Label id="Label2" runat="server">Consult your system administrator</asp:Label></td>
				</tr>
			</table>
		</form>
	</body>
</html>
