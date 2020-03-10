<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReleasePresses.aspx.cs" Inherits="WebCenter4.Views.ReleasePresses" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Release to presses</title>
    <link rel="stylesheet" type="text/css" href="../Style/WebCenter.css" />
    <script  type="text/javascript">
        var initWidth;

        function CloseOnReload() {
            GetRadWindow().BrowserWindow.document.Form1.txtReturnedFromPopup.value = '1';
            GetRadWindow().BrowserWindow.document.Form1.submit();
            GetRadWindow().Close();
        }

        function RefreshParentPage() {
            GetRadWindow().BrowserWindow.document.Form1.txtReturnedFromPopup.value = '1';
            GetRadWindow().BrowserWindow.document.Form1.submit();
            GetRadWindow().Close();
        }

        function SizeToFit() {
            var oWnd;
            var theTop;
            var theLeft;

            window.setTimeout(
                function () {
                    oWnd = GetRadWindow();
                    //	if (oWnd.BrowserWindow.initWidth == null)
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
                margin: 5px;  
                padding: 0px;  
                overflow: hidden;  
            }  
        </style> 
</head>
<body>
    <form id="form1" runat="server">
         <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
        <table style="height: 400px; width: 500px;" id="Table1" border="0" cellSpacing="1" cellPadding="1" width="500">
				<tr>
					<td style="width: 82px" vAlign="top" align="left"></td>
					<td vAlign="top" align="left"><asp:label style="Z-INDEX: 0" id="lblRetransmittolocations" runat="server" Font-Names="Verdana"
							Font-Size="10pt">Release to specific locations</asp:label></td>
				</tr>
				<tr>
					<td style="WIDTH: 82px" vAlign="top" align="left"><asp:label id="lblLocations" runat="server" Font-Names="Verdana" Font-Size="10pt">Locations</asp:label></td>
					<td vAlign="top" align="left">
                        <asp:datagrid id="DataGridLocations" runat="server" BorderColor="#999999" CellPadding="3" GridLines="Vertical"
							AutoGenerateColumns="False" CssClass="LabelNormal" BorderWidth="1px" Width="256px" BorderStyle="None" BackColor="White">
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
						</asp:datagrid>
					</td>
                </tr>
				<tr>
					<td colSpan="2" align="center">
						<table id="Table2" border="0" cellSpacing="1" cellPadding="1">
							<tr>
								<td align="center"><telerik:RadButton id="bntApply" runat="server" Width="60px" EnableViewState="False"
										Text="Apply" Skin="Office2010Blue" OnClick="bntApply_Click"></telerik:RadButton></td>
								<td align="center"><telerik:RadButton id="btnCancel" runat="server" Width="60px" EnableViewState="False"
										Text="Cancel" Skin="Office2010Blue" OnClick="btnCancel_Click"></telerik:RadButton></td>
							</tr>
						</table>
				</tr>
			</table>
			<asp:textbox id="txtMasterCopySeparationSet" runat="server" Width="2px" Visible="false"></asp:textbox><asp:textbox id="txtCopyFlatSeparationSet" runat="server" Width="2px" Visible="false"></asp:textbox>
			<asp:textbox id="txtMasterCopySeparationSet2" runat="server" Width="2px" Visible="false"></asp:textbox><asp:textbox id="Textbox2" runat="server" Width="2px" Visible="false"></asp:textbox>
			<asp:label id="InjectScript" runat="server"></asp:label>       
    </form>
</body>
</html>
