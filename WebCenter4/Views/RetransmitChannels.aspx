<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RetransmitChannels.aspx.cs" Inherits="WebCenter4.Views.RetransmitChannels" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Retransmit to Channels</title>
    <link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript">

        var initWidth;

        function CloseOnReload() {
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
                            <asp:Label id="lblRetransmittochannels"  runat="server" Text="Re-transmit to channels" CssClass="RadToolbarLabel"></asp:Label>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>
                </Items>
            </telerik:RadToolBar>

            <table id="Table1" cellSpacing="1" cellPadding="1" border="0" style="width: 500px; height: 400px;">
		    <tr>
			    <td style="WIDTH: 82px; vertical-align: top;text-align: left;">
				    <asp:Label id="lblChannels" runat="server" Font-Names="Verdana" Font-Size="10pt">Channels</asp:Label></td>
                    <td>
				    <asp:datagrid id="DataGridChannels" runat="server" BackColor="White" BorderStyle="None" Width="256px"
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
								    <asp:CheckBox id="CheckBoxUseChannel" runat="server"></asp:CheckBox>
							    </ItemTemplate>
						    </asp:TemplateColumn>
						    <asp:BoundColumn DataField="Channel" SortExpression="Channel" ReadOnly="True" HeaderText="Channel"></asp:BoundColumn>
					    </Columns>
					    <PagerStyle HorizontalAlign="Center" ForeColor="Black" BackColor="#999999" Mode="NumericPages"></PagerStyle>
				    </asp:datagrid>
			    </td>
            </tr>
		    <tr>
			    <td colspan="2" align="center">
				    <table id="Table2" cellSpacing="1" cellPadding="1" border="0">
					    <tr>
						    <td align="center">
                                <telerik:RadButton id="btnApply" runat="server"   Width="70px" Text="Apply" Skin="Office2010Blue" OnClick="bntApply_Click"
										EnableViewState="False"></telerik:RadButton>
                            </td>
						    <td align="center">
                                <telerik:RadButton id="btnCancel" runat="server"   Width="70px" Text="Cancel" OnClick="btnCancel_Click"
										EnableViewState="False" CausesValidation="False" Skin="Office2010Blue"></telerik:RadButton>
                            </td>
					    </tr>
				    </table>
					<asp:TextBox id="txtProductionID" runat="server" Width="2px" Visible="False"></asp:TextBox>
					<asp:TextBox id="txtChannels" runat="server" Width="2px" Visible="False"></asp:TextBox>
					<asp:TextBox id="txtPublicationID" runat="server" Width="2px" Visible="False"></asp:TextBox>
					<asp:TextBox id="txtMasterCopySeparationSet" runat="server" Width="2px" Visible="False"></asp:TextBox>
			    </td>
		    </tr>
	    </table>
        <asp:Label id="InjectScript" runat="server"></asp:Label>
        </div>
    </form>
</body>
</html>
