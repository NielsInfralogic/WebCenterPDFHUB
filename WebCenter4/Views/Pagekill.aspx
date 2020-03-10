<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Pagekill.aspx.cs" Inherits="WebCenter4.Views.Pagekill" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Page kill</title>
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
                margin: 0px;  
                padding: 0px;  
                overflow: hidden;  
            }  
        </style> 
</head>
<body>
        <form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <div style="width: 100%;height:50px;text-align: center;padding-top:10px;">
			    <asp:Label id="lblKillPage" runat="server">Label</asp:Label>
			</div>
            <div style="width: 100%;height:50px;text-align: center;padding-top:20px;">
                <asp:CheckBox ID="cbPermanentKill" runat="server" Text="Delete page permanently" />
			</div>
            <div style="width: 100%;height:30px;text-align: center;padding-bottom:20px;padding-top:20px;">
                    <div style="width: 100px;float:left; padding-left:10px;">
						<telerik:RadButton id="bntApply" runat="server" EnableViewState="False" Text="Apply" Width="70px" Skin="Office2010Blue" OnClick="bntApply_Click"></telerik:RadButton>
					</div>
					<div style="width: 100px;float: right; padding-right:10px">
						<telerik:RadButton id="btnCancel" runat="server" EnableViewState="False" Text="Cancel" Width="70px" Skin="Office2010Blue" OnClick="btnCancel_Click"></telerik:RadButton>
					</div>
                </div>
			<asp:Label id="InjectScript" runat="server"></asp:Label>
            <input runat="server" id="hiddenMasterCopySeparationSet" type="hidden" value="0" />
		</form>
    </body>
</html>
