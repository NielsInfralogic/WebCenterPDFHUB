<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowFileDistLog.aspx.cs" Inherits="WebCenter4.ShowFileDistLog" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Vis mottak-log</title>
    <link rel="shortcut icon" href="images/IL.ico" />
    <link href="Styles/WebCenter.css" type="text/css" rel="stylesheet" />
    <style type="text/css">  
        html, body, form  
        {  
            height: 100%;  
            margin: 1px;  
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
                            <asp:Label ID="lblShowLog" runat="server" Text="Download FTP log" CssClass="RadToolbarLabel"></asp:Label>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>
                </Items>
                
            </telerik:RadToolBar>
        <table>
            <tr>
                <td>
                    <asp:label id="lblAdmin" runat="server" Font-Size="10pt">Presse</asp:label></td><td>
                    <asp:DropDownList ID="DropDownList1" runat="server">
				        <asp:ListItem Value="STO" Selected="True">STO</asp:ListItem>
                        <asp:ListItem Value="STK">STK</asp:ListItem>
				        <asp:ListItem Value="STB" >STB</asp:ListItem>
                        <asp:ListItem Value="STS" >STS</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:button id="btnDownload" runat="server" BackColor="PowderBlue" BorderStyle="Outset"
				    						BorderWidth="1px" Width="136px" Text="Download" OnClick="btnDownload_Click"></asp:button>
                </td>
            </tr>
        </table>
	</form>
</body>
</html>
