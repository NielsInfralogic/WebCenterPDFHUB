<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomAction.aspx.cs" Inherits="WebCenter4.Views.CustomAction" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
	    <title>Finish product</title>
            <link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
	     <script  type="text/javascript">
        var initWidth;

        function CloseOnReload() {
            GetRadWindow().BrowserWindow.document.Form1.HiddenReturendFromPopup.value = '0';
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
            .style2 { WIDTH: 150px; HEIGHT: 30px }
	        .style3 { HEIGHT: 30px }
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
        <form id="Form1" enctype="multipart/form-data" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>

	        <table style="width: 480px;text-align:left;" id="Table1" border="0" cellspacing="1" cellpadding="1">
		        <tr>
			        <td class="style2"><asp:label id="lblProductLabel" runat="server">Product</asp:label></td>
			        <td class="style3"><asp:label id="lblProductText" runat="server">Product</asp:label></td>
		        </tr>
		        <tr>
			        <td class="style2"><asp:label id="lblVersion" runat="server"> Version</asp:label></td>
			        <td class="style3">
				        <asp:TextBox style="z-index: 0" id="txtVersion" runat="server" Height="24px" Width="40px"></asp:TextBox></td>
		        </tr>
                  <tr>
			        <td class="style2"><asp:label id="Label1" runat="server"> Push varsel</asp:label></td>
			        <td class="style3">
				        <asp:TextBox style="z-index: 0" id="txtPushMessage" runat="server" Height="24px" Width="160px"></asp:TextBox></td>
		        </tr>
		        <tr>
			        <td class="style2"></td>
			        <td class="style3"></td>
		        </tr>
		        <tr>
			        <td style="text-align: center;" colspan="2">
				        <table style="width: 200px;" id="Table2" border="0" cellspacing="1" cellpadding="1">
					        <tr>
						        <td style="text-align: center;">
                                    <telerik:RadButton id="bntApply" runat="server" Width="70px" EnableViewState="False" Skin="Office2010Blue"
								            Text="Apply" OnClick="bntApply_Click"></telerik:RadButton>
						        </td>
						        <td style="text-align: center;">
                                    <telerik:RadButton id="btnCancel" runat="server" Width="70px" EnableViewState="False" Skin="Office2010Blue"
								           Text="Cancel"  OnClick="btnCancel_Click"></telerik:RadButton>
						        </td>
					        </tr>
				        </table>
				        <asp:label style="z-index: 0" id="lblInfo" runat="server" CssClass="LabelNormal"></asp:label></td>
		        </tr>
	        </table>
	        <asp:label id="InjectScript" runat="server"></asp:label>
        </form>
    </body>
</html>
