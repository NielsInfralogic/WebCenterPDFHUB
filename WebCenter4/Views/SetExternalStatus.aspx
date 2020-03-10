<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetExternalStatus.aspx.cs" Inherits="WebCenter4.Views.SetExternalStatus" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Set External Status</title>
    <link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript">

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
                            <asp:Label ID="LabelSetExternalStatusHeader" runat="server" Text="Set External Status" CssClass="RadToolbarLabel"></asp:Label>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>
                </Items>
            </telerik:RadToolBar>

            <table id="Table1" cellSpacing="1" cellPadding="1" border="0" style="width: 290px; height: 400px;">
		    <tr>
                <td style="text-align: center;">
                    <telerik:RadListBox ID="RadListBox1" runat="server" Skin="Vista">
                    </telerik:RadListBox>	   
			    </td>
            </tr>
		    <tr>
			    <td align="center">
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
				    <asp:TextBox id="txtMasterCopySeparationSetList" runat="server" Width="2px" Visible="False"></asp:TextBox>
			    </td>
		    </tr>
	    </table>
        <asp:Label id="InjectScript" runat="server"></asp:Label>
        </div>
    </form>
</body>
</html>
