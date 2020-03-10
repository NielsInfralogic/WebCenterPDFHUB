<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReprocessPressRun.aspx.cs" Inherits="WebCenter4.Views.ReprocessPressRun" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Re-process pages</title>
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

        <table id="Table1" style="width: 380px;border:none;" cellspacing="1" cellpadding="1">
				<tr>
					<td style="text-align: left;">
						<table id="Table3" style="width: 375px; border:none; vertical-align:top;" cellspacing="0" cellpadding="2" >
                            <tr>
                                <td style="text-align: left;">
                                    <asp:Label ID="Label4" runat="server" Text="Press"></asp:Label></td>
                                <td style="text-align: left;"><asp:DropDownList ID="DropDownListPress" runat="server" AutoPostBack="True"></asp:DropDownList></td>
                                </tr>
							<tr>
                                <td style="text-align: left;">
                                    <asp:Label ID="Label1" runat="server" Text="Preflight setting"></asp:Label></td>
                                <td style="text-align: left;"><asp:DropDownList ID="DropDownListPreflight" runat="server"></asp:DropDownList></td>
                                </tr>
                            <tr>
                                <td style="text-align: left;">
                                     <asp:Label ID="Label2" runat="server" Text="Inksave setting"></asp:Label></td>
                                <td style="text-align: left;"><asp:DropDownList ID="DropDownListInksave" runat="server"></asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td style="text-align: left;">
                                 <asp:Label ID="Label3" runat="server" Text="RIP setting"></asp:Label></td>
                                <td style="text-align: left;"><asp:DropDownList ID="DropDownListRipSetup" runat="server"></asp:DropDownList></td>

							</tr>
                            <tr>
                                <td style="text-align: left;">
                                    
                                    <asp:Label ID="lblApplyToAllEditions" runat="server" Text="Apply to all editions"></asp:Label></td>
                                <td style="text-align: left;"><asp:CheckBox ID="CheckBoxAllEditions" runat="server" TextAlign="Right" /></td>
                                </tr>
                            <tr>
                                <td style="text-align: left;">
                                     <asp:Label ID="lblApplyToAllSections" runat="server" Text="Apply to all sections"></asp:Label></td>
                                <td style="text-align: left;"><asp:CheckBox ID="CheckBoxAllSections" runat="server" TextAlign="Right" /></td>
                            </tr>
						</table>
					</td>
				</tr>
				<tr >
					<td style="padding-left:65px">
						<table id="Table2" style="width: 250px; border:none;text-align: center;" cellspacing="1" cellpadding="1" >
							<tr style="text-align: center;">
								<td style="text-align: center;"><telerik:RadButton id="bntApply" runat="server" BackColor="LightBlue" Text="Re-process" EnableViewState="False"
										Width="70px" OnClick="bntApply_Click" Skin="Office2010Blue"></telerik:RadButton></td>
								<td style="text-align: center;"><telerik:RadButton id="btnCancel" runat="server" BackColor="LightGray" Text="Cancel" EnableViewState="False"
										 Width="70px" OnClick="btnCancel_Click" Skin="Office2010Blue"></telerik:RadButton></td>
							</tr>
						</table>
                    </td>
                </tr>
                <tr>
                    <td>
                    	<asp:TextBox id="txtPublication" runat="server" Width="2px" Visible="False"></asp:TextBox>
						<asp:TextBox id="txtSection" runat="server" Width="2px" Visible="False"></asp:TextBox>
						<asp:TextBox id="txtEdition" runat="server" Width="2px" Visible="False"></asp:TextBox>
						<asp:TextBox id="txtPubDate" runat="server" Width="2px" Visible="False"></asp:TextBox>
						<asp:TextBox id="txtPress" runat="server" Width="2px" Visible="False"></asp:TextBox>
						<asp:TextBox id="txtRipSetupID" runat="server" Width="2px" Visible="False"></asp:TextBox>

						<asp:label id="lblError" runat="server" ForeColor="Red"></asp:label>
						<asp:Label id="InjectScript" runat="server"></asp:Label>

                    </td>
				</tr>
			</table>
    </form>
</body>
</html>
