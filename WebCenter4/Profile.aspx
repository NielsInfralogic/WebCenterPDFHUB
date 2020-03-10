<%@ Page language="c#" Codebehind="Profile.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Profile" %>

<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Profile</title>
		<link rel="shortcut icon" href="images/IL.ico" />
		<link href="Styles/WebCenter.css" type="text/css" rel="stylesheet" />
		<script language="JavaScript" type="text/javascript" src="Styles/selectbox.js"></script>
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
			<asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;">
                <Items>
                    <telerik:RadToolBarButton runat="server" Value="Item1">
                        <ItemTemplate>
                            <asp:Label ID="MyProfile" runat="server" Text="My Profile" CssClass="RadToolbarLabel"></asp:Label>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>
                </Items>
            </telerik:RadToolBar>
 			<table id="Table1" style="width: 460px; height: 240px" cellspacing="1" cellpadding="1" width="460" border="0">
				<tr>
					<td style="height:60px; vertical-align: middle;">
						<table id="Table2" cellspacing="1" cellpadding="1" width="450" align="left" border="0">
							<tr>
								<td style="width: 81px;"><img alt="" src="./Images/user_big.gif" border="0" /></td>
								<td style="width: 104px;"><asp:label id="lblProfileFor" runat="server">Profile for </asp:label></td>
								<td><asp:label id="lblUserName" runat="server" ForeColor="#C04000" Font-Bold="True"></asp:label></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td>
						<table id="Table3" cellspacing="1" cellpadding="1" width="450" align="left" border="0">
							<tr>
								<td><asp:label id="lblRealName" runat="server" Font-Size="10pt">Real name</asp:label></td>
								<td><asp:textbox id="txtRealname" runat="server" BorderStyle="Solid" BorderWidth="1px" Width="270px"></asp:textbox></td>
								<td></td>
							</tr>
							<tr>
								<td><asp:label id="lblEmail" runat="server" Font-Size="10pt">Email</asp:label></td>
								<td><asp:textbox id="txtEmail" runat="server" BorderStyle="Solid" BorderWidth="1px" Width="270px"></asp:textbox></td>
								<td></td>
							</tr>
							<tr>
								<td><asp:label id="lblImagesPerRow" runat="server" Font-Size="10pt">Pages per row</asp:label></td>
								<td>
									<asp:dropdownlist id="DropdownlistImagePerRow" runat="server">
										<asp:ListItem Value="4">4</asp:ListItem>
										<asp:ListItem Value="6">6</asp:ListItem>
										<asp:ListItem Value="8" Selected="True">8</asp:ListItem>
										<asp:ListItem Value="10">10</asp:ListItem>
										<asp:ListItem Value="12">12</asp:ListItem>
										<asp:ListItem Value="14">14</asp:ListItem>
										<asp:ListItem Value="16">16</asp:ListItem>
										<asp:ListItem Value="18">18</asp:ListItem>
										<asp:ListItem Value="20">20</asp:ListItem>
										<asp:ListItem Value="22">22</asp:ListItem>
										<asp:ListItem Value="24">24</asp:ListItem>
									</asp:dropdownlist></td>
								<td></td>
							</tr>
							
							<tr>
								<td style="height: 18px"><asp:label id="lblPlateSize" runat="server" Font-Size="10pt">Plates per row</asp:label></td>
								<td style="height: 18px" align="left">
                                    <asp:dropdownlist id="DropDownListPlateSize" runat="server">
										<asp:ListItem Value="2">2</asp:ListItem>
                                        <asp:ListItem Value="3">3</asp:ListItem>
										<asp:ListItem Value="4" Selected="True">4</asp:ListItem>
										<asp:ListItem Value="5">5</asp:ListItem>
                                        <asp:ListItem Value="6">6</asp:ListItem>
                                        <asp:ListItem Value="7">7</asp:ListItem>
                                        <asp:ListItem Value="8">8</asp:ListItem>
                                        <asp:ListItem Value="9">9</asp:ListItem>
                                        <asp:ListItem Value="10">10</asp:ListItem>
                                        <asp:ListItem Value="11">11</asp:ListItem>
                                        <asp:ListItem Value="12">12</asp:ListItem>
                                    
									</asp:dropdownlist></td>
								<td style="height: 18px"></td>
							</tr>
                            <tr>
								<td><asp:label id="lblRefreshTime" runat="server" Font-Size="10pt">Auto-refresh time</asp:label></td>
								<td><asp:textbox id="txtRefreshTime" runat="server" BorderStyle="Solid" BorderWidth="1px" Width="30px"
										MaxLength="2"></asp:textbox></td>
								<td></td>
							</tr>
		
                               <tr>
								<td><asp:label id="lblDefaultViewer" runat="server" Font-Size="10pt">Default viewer</asp:label></td>
								<td>
                                    <asp:DropDownList ID="DropDownListViewer" runat="server">
                                        <asp:ListItem Value="0">Flash</asp:ListItem>
                                        <asp:ListItem Value="1">HTML5</asp:ListItem>
                                    </asp:DropDownList>

								</td>
								<td></td>
							</tr>
		
                            
                            					
							<tr>
								<td style="HEIGHT: 18px"><asp:label id="lblAdmin" runat="server" Font-Size="10pt">Administrator tasks</asp:label></td>
								<td style="HEIGHT: 18px" align="left"><asp:button id="btnReloadCaches" runat="server" BackColor="PowderBlue" BorderStyle="Outset"
										BorderWidth="1px" Width="136px" Text="Reload caches" ToolTip="Re-load cached names from database"></asp:button></td>
								<td style="HEIGHT: 18px"></td>
							</tr>
							<tr>
								<td></td>
								<td align="right"><asp:label id="lblError" runat="server" ForeColor="Red"></asp:label><asp:button id="btnSave" runat="server" BorderStyle="Solid" BackColor="LightCyan" BorderWidth="1px"
										Text="Save" Width="82px" ToolTip="Save settings to database"></asp:button></td>
								<td></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
	</body>
</html>
