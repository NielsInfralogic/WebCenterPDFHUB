<%@ Page language="c#" Codebehind="Logwindow.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Logwindow"  %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Logwindow</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
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
	<body >
		<form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>

			<table id="Table1" cellspacing="0" cellpadding="0" width="100%" border="0" align="center">
				<tr>
					<td style="white-space: nowrap">
					
                        <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;">
                            <Items>
                                <telerik:RadToolBarButton runat="server" Value="Refresh" Text="Refresh" ImageUrl="../Images/refresh16.gif" ImagePosition="Left" ToolTip="Refresh thumbnail view"  PostBack="true">
                                </telerik:RadToolBarButton>
                            </Items>
                        </telerik:RadToolBar>
                        </td>
				<tr>
					<td style="white-space: nowrap">
						<asp:TextBox id="TextBox" runat="server" Width="100%" Height="460px" ReadOnly="True" EnableViewState="False"
							BorderStyle="Solid" BorderColor="CornflowerBlue" BorderWidth="1px" TextMode="MultiLine" Font-Size="Smaller"></asp:TextBox></td>
				</tr>
				<tr>
					<td style="white-space: nowrap">
						<asp:Label id="lblError" runat="server" ForeColor="Red"></asp:Label></td>
				</tr>
			</table>
		</form>
	</body>
</html>
