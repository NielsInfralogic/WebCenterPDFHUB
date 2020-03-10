<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RetransmitLocations.aspx.cs" Inherits="WebCenter4.Views.RetransmitLocations" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Retransmit to Channels</title>
    <link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript">

        var initWidth;

        function CloseOnReload() {
            GetRadWindow().BrowserWindow.document.forms[0].HiddenReturendFromPopup.value = '0';
            GetRadWindow().Close();
        }
        function RefreshParentPage() {
            GetRadWindow().BrowserWindow.document.forms[0].HiddenReturendFromPopup.value = '1';
            GetRadWindow().BrowserWindow.document.forms[0].submit();
            GetRadWindow().Close();
        } 
        function SizeToFit() {
            var oWnd;
            var theTop;
            var theLeft;

            window.setTimeout(
                function () {
                    oWnd = GetRadWindow();
                    //	if (oWnd.BrowserWindow.initWidth == null)
                    //		oWnd.BrowserWindow.initWidth = document.body.scrollWidth+20;
                    if (initWidth == null)
                        initWidth = document.body.scrollWidth + 20;
                    //alert(initWidth);
                    oWnd.SetSize(initWidth, document.body.scrollHeight + 100);

                }, 600);
        }

        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }

    </script>

    <style type="text/css">  
    html, body, form  
    {  
        height: 100%;  
        margin: 0px;  
        padding: 0px;  
               
    }  
    .auto-style1
    {
        width: 224px;
        height: 75px;
    }
    .auto-style2
    {
        height: 75px;
    }
    </style>

</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>

            <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;">
                <Items>
                    <telerik:RadToolBarButton runat="server" Value="Item1">
                        <ItemTemplate>
                            <asp:Label id="lblRetransmittolocations"  runat="server" Text="Re-transmit to locations" CssClass="RadToolbarLabel"></asp:Label>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>
                </Items>
            </telerik:RadToolBar>

            <table id="Table1" cellSpacing="1" cellPadding="1" border="0" style="width: 500px; height: 400px;">
		    <tr>
			    <td style="WIDTH: 82px; vertical-align: top;text-align: left;">
				    <asp:Label id="lblLocations" runat="server" Font-Names="Verdana" Font-Size="10pt">Locations</asp:Label></td>
                    <td>
				    <asp:DataGrid id="DataGridLocations" runat="server" BackColor="White" BorderStyle="None" Width="256px"
							BorderWidth="1px" CssClass="LabelNormal" AutoGenerateColumns="False" GridLines="Vertical"
							CellPadding="3" BorderColor="#999999">
						<FooterStyle ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
						<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
						<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
						<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
						<HeaderStyle Font-Bold="True" ForeColor="White" BackColor="#000084"></HeaderStyle>
						<Columns>
							<asp:TemplateColumn HeaderText="Use">
								<HeaderStyle Width="30px"></HeaderStyle>
								<ItemTemplate>
									<asp:CheckBox id="CheckBoxUseLocation" runat="server"></asp:CheckBox>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:BoundColumn DataField="Location" SortExpression="Location" ReadOnly="True" HeaderText="Location"></asp:BoundColumn>
						</Columns>
						<PagerStyle HorizontalAlign="Center" ForeColor="Black" BackColor="#999999" Mode="NumericPages"></PagerStyle>
					</asp:DataGrid>
			    </td>
            </tr>
		    <tr>
				<td colspan="2" align="center">
					<table id="Table2" cellSpacing="1" cellPadding="1" border="0">
						<tr>
							<td align="center">
								<asp:button id="btnApply" runat="server" Text="Apply" BackColor="LightBlue" EnableViewState="False"
									Width="60px" Height="24px"></asp:button></td>
							<td align="center">
								<asp:button id="btnCancel" runat="server" Text="Cancel" BackColor="LightGray" EnableViewState="False"
									Width="60px" Height="24px"></asp:button></td>
						</tr>
					</table>
					<asp:TextBox id="txtMasterCopySeparationSet" runat="server" Width="2px" Visible="False"></asp:TextBox>
				</td>
			</tr>
	    </table>
        <asp:Label id="InjectScript" runat="server"></asp:Label>
        </div>
    </form>
</body>
</html>
