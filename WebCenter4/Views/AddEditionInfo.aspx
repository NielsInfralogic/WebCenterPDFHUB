<%@ Page language="c#" Codebehind="AddEditionInfo.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.AddEditionInfo" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Edition details</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
			<script type="text/javascript">

			    function CloseOnReload()
			    {		      
			        GetRadWindow().Close();
			    }
			    function RefreshParentPage() 
			    {			
			        GetRadWindow().BrowserWindow.document.forms[0].submit(); 
			        GetRadWindow().Close();
			    }
			    function SizeToFit()
			    {
			        var oWnd;
			        var theTop;
			        var theLeft;
				
			        window.setTimeout(
                        function()
                        {
                            oWnd = GetRadWindow();
                            //	if (oWnd.BrowserWindow.initWidth == null)
                            //		oWnd.BrowserWindow.initWidth = document.body.scrollWidth+20;
                            if (initWidth == null)
                                initWidth = document.body.scrollWidth+20;
                            //alert(initWidth);
                            oWnd.SetSize(initWidth,  document.body.scrollHeight+100);
						
                        }, 600);
			    }
			   
			    function GetRadWindow()
			    {
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
		<form id="Form1" method="post" runat="server" defaultfocus="ddlEditions">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>

            <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;">
                <Items>
                    <telerik:RadToolBarButton runat="server" Value="Item1">
                        <ItemTemplate>
                            <asp:Label ID="LabelEditionsHeader" runat="server" Text="Edit edition details" CssClass="RadToolbarLabel"></asp:Label>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>
                </Items>
            </telerik:RadToolBar>

			<table style="width: 600px; border: 0px;" id="Table1" cellspacing="1" cellpadding="1">
				<tr>
                    <td style="vertical-align: top" align="center" colspan="3">
                        <asp:datagrid id="DataGridEditions" runat="server" BackColor="White" BorderStyle="None" Font-Size="10pt"
						    Font-Names="Verdana" ShowFooter="True" CellSpacing="2" HorizontalAlign="Left" GridLines="Horizontal" BorderWidth="1px" BorderColor="#E7E7FF"
						    CellPadding="3">
						    <FooterStyle ForeColor="#4A3C8C" BackColor="#B5C7DE" Wrap="true"></FooterStyle>
						    <SelectedItemStyle Font-Bold="True" ForeColor="#F7F7F7" BackColor="#738A9C"></SelectedItemStyle>
						    <AlternatingItemStyle BackColor="#F7F7F7"></AlternatingItemStyle>
						    <ItemStyle ForeColor="#4A3C8C" BackColor="#E7E7FF"></ItemStyle>
						    <HeaderStyle Font-Bold="False" ForeColor="#F7F7F7" BackColor="#4A3C8C"></HeaderStyle>
						    <PagerStyle HorizontalAlign="Right" ForeColor="#4A3C8C" BackColor="#E7E7FF" Mode="NumericPages"></PagerStyle>
	    				</asp:datagrid>
                    </td>
				</tr>
				<tr>
					<td style="height: 17px" align="left" colspan="2">
                        <asp:label id="lblError" runat="server" ForeColor="Red" Font-Size="10pt"></asp:label>
                        <asp:label id="lblInfo" runat="server" ForeColor="LimeGreen" Font-Size="10pt"></asp:label>
                    </td>
					<td style="height: 17px" align="center"></td>
				</tr>
				<tr>
					<td style="height: 30px; vertical-align: top;"  align="center" colspan="2" >
						<table style="height: 26px; width: 176px; border: 0px" id="Table2" cellspacing="0" cellpadding="1">
							<tr>
								<td align="center">
                                    <telerik:RadButton id="btnSave" runat="server"   Width="70px" Text="Save" Skin="Office2010Blue" OnClick="btnSave_Click"
										EnableViewState="False"></telerik:RadButton>
                                </td>
								<td align="center">
                                    <telerik:RadButton id="btnCancel" runat="server"   Width="70px" Text="Cancel" OnClick="btnCancel_Click"
										EnableViewState="False" CausesValidation="False" Skin="Office2010Blue"></telerik:RadButton>
                                </td>
							</tr>
						</table>
					</td>
					<td style="height: 30px; vertical-align: top;" align="center">
    				<asp:Label id="InjectScript" runat="server"></asp:Label></td>

				</tr>
			</table>
		</form>
	</body>
</html>
