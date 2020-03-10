<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReprocessPage.aspx.cs" Inherits="WebCenter4.Views.ReprocessPage" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Re-process page</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
    <script  type="text/javascript">
        var initWidth;

        function CloseOnReload() {
            GetRadWindow().BrowserWindow.document.Form1.HiddenReturendFromPopup.value = '0';
            GetRadWindow().Close();
        }

        function RefreshParentPage() {
            GetRadWindow().BrowserWindow.document.Form1.HiddenReturendFromPopup.value = '1';
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

            <table id="Table1" style="WIDTH: 380px; HEIGHT: 200px; border:none;" cellspacing="1" cellpadding="1" >
				<tr>
					<td align="center"><asp:label id="LabelHeader" runat="server" CssClass="HeaderText">Reprocess page</asp:label></td>
				</tr>
				<tr>
					<td align="center">
						<table id="Table3" style="width: 375px; border:none;" cellspacing="0" cellpadding="0">
							<tr>
                                <td style="text-align: left;">
                                    <asp:Label ID="Label1" runat="server" Text="Preflight setting"></asp:Label></td>
                                <td style="text-align: left;"><asp:DropDownList ID="DropDownListPreflight" runat="server" Width="120px"></asp:DropDownList></td>
                                </tr>
                            <tr>
                                <td style="text-align: left;">
                                        <asp:Label ID="Label2" runat="server" Text="Inksave setting"></asp:Label></td>
                                <td style="text-align: left;"><asp:DropDownList ID="DropDownListInksave" runat="server" Width="120px"></asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td style="text-align: left;">
                                    <asp:Label ID="Label3" runat="server" Text="RIP setting"></asp:Label></td>
                                <td style="text-align: left;"><asp:DropDownList ID="DropDownListRipSetup" runat="server" Width="120px"></asp:DropDownList></td>

							</tr>
						</table>
					</td>
				</tr>
				
              
				<tr>
					<td align="center">
						<table id="Table2" cellspacing="1" cellpadding="1" width="200" border="0">
							<tr>
								<td align="center"><telerik:RadButton id="bntApply" runat="server" Text="Re-process" EnableViewState="False"
										 Width="70px" OnClick="bntApply_Click" Skin="Office2010Blue"></telerik:RadButton></td>
								<td align="center"><telerik:RadButton id="btnCancel" runat="server" Text="Cancel" EnableViewState="False"
										Width="70px" OnClick="btnCancel_Click" Skin="Office2010Blue"></telerik:RadButton></td>
							</tr>
						</table>
						<asp:label id="lblError" runat="server" ForeColor="Red"></asp:label>
						<asp:Label id="InjectScript" runat="server"></asp:Label></td>
				</tr>
			</table>

            <input runat="server" id="hiddenMasterCopySeparationSet" type="hidden" value="0" />
        </form>
    </body>
</html>
