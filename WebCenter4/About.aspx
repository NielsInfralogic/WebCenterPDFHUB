<%@ Page language="c#" Codebehind="About.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.About" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>About ControlCenter</title>
		<link rel="shortcut icon" href="images/IL.ico" />
   		<link href="./Styles/WebCenter.css" type="text/css" rel="stylesheet" />


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
	<body>
		<form id="Form1" method="post" runat="server">
			<table id="Table1" style="width:300px; border:0;" cellspacing="0" cellpadding="0" width="300" border="0">
				<tr>
					<td style="width: 77px"><img alt="" src="./Images/NilsErik.gif" align="middle" /></td>
					<td>
						<table style="height:100%; width:100%; border:0;"  id="Table2" cellspacing="2" cellpadding="2" width="100%" border="0" height="100%">
							<tr>
								<td align="center"><img alt="" src="./Images/spacer.gif" height="50" width="200" /></td>
							</tr>
							<tr>
								<td align="center">
									<asp:Label id="lblTitle" runat="server" Font-Bold="True" Font-Names="Verdana">WebCenter</asp:Label></td>
							</tr>
							<tr>
								<td align="center"><img alt="" src="./Images/spacer.gif" height="20"/></td>
							</tr>
							<tr>
								<td align="center" height="40">
									<asp:Label id="lblVersion" runat="server" Font-Names="Verdana" Font-Size="9pt">Version 1.6</asp:Label></td>
							</tr>
							<tr>
								<td align="center" height="40">
									<asp:Label id="lblCopyRight" runat="server" Font-Names="Verdana" Font-Size="6pt">(C) Copyright InfraLogic 2011</asp:Label></td>
							</tr>
							<tr>
								<td align="center"><img alt="" src="./Images/spacer.gif" height="30" /></td>
							</tr>
							<tr>
								<td align="center"><img alt="" src="./Images/logoweb.gif" /></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
	</body>
</html>
