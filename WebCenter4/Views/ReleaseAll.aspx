<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReleaseAll.aspx.cs" Inherits="WebCenter4.Views.ReleaseAll" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
    <head runat="server">
		<title>Release all</title>
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
		<table style="width: 480px;text-align: left;" id="Table1" border="0" cellSpacing="1" cellPadding="1" align="left">
				<tr>
					<td class="style2">
						<asp:label id="lblProductLabel" runat="server" CssClass="HeaderText">Product</asp:label></td>
					<td class="style3">
						<asp:label id="lblProductText" runat="server" CssClass="HeaderText">Product</asp:label></td>
				</tr>
				<TR>
					<TD class="style2">
						<asp:label id="lblApprovedByLabel" runat="server" CssClass="HeaderText">Released by</asp:label></TD>
					<TD class="style3">
						<asp:label id="lblApprovedByText" runat="server" CssClass="HeaderText">Released by</asp:label></TD>
				</TR>
				<TR>
					<td class="style2"><asp:label id="lclCommentLabel" runat="server" CssClass="HeaderText">Comment</asp:label></td>
					<td class="style3"><asp:textbox id="txtComment" runat="server" Width="280px" Height="28px"></asp:textbox></td>
				</TR>
				<tr>
					<td class="style2" colspan="2"></td>
				</tr>
				<tr>
					<TD colSpan="2" align="center">
						<table id="Table2" cellSpacing="1" cellPadding="1" border="0" width="200">
							<tr>
								<TD align="center"><telerik:RadButton id="bntApply" runat="server" Text="Apply" EnableViewState="False"
										 Width="70px" OnClick="bntApply_Click"  Skin="Office2010Blue"></telerik:RadButton></TD>
								<TD align="center"><telerik:RadButton id="btnCancel" runat="server" Text="Cancel" EnableViewState="False"
										 Width="70px" OnClick="btnCancel_Click"  Skin="Office2010Blue"></telerik:RadButton></TD>
							</tr>
						</table>
						<asp:label id="lblInfo" runat="server" CssClass="LabelNormal" style="Z-INDEX: 0"></asp:label>
					</TD>
				</tr>
			</table>
			<asp:Label id="InjectScript" runat="server"></asp:Label>

    		</form>
</body>
</html>
