<%@ Page language="c#" Codebehind="ChangePriority.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.ChangePriority" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>ChangePriority</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
		<script type="text/javascript">
		<!--
			var initWidth;
						
			function CloseOnReload()
			{
			    GetRadWindow().BrowserWindow.document.forms[0].HiddenReturendFromPopup.value = '0';
				GetRadWindow().Close();
			}
			
			function RefreshParentPage() 
			{			
			    GetRadWindow().BrowserWindow.document.forms[0].HiddenReturendFromPopup.value = '1';
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
		//-->
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
	<body >
		<form id="Form1" method="post" runat="server">
        
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>

			<table id="Table1" cellspacing="1" cellpadding="1" width="280" border="0" style="WIDTH: 200px; HEIGHT: 88px">
				<tr>
					<td style="white-space:nowrap;">
						<asp:Label id="lblPriority" runat="server" Font-Names="Verdana" Font-Size="10pt" >Priority</asp:Label></td>
					<td>                        
						<telerik:RadNumericTextBox id="RadNumerictxtPrioValue" Width="100px" ToolTip="Priority values: 100:highest, 0:lowest"
							Runat="server" MaxValue="100" MinValue="0" ShowSpinButtons="True" Skin="Default">
							<NumberFormat AllowRounding="True" GroupSizes="3" DecimalSeparator="," NegativePattern="-n" GroupSeparator="."
								DecimalDigits="0" PositivePattern="n"></NumberFormat>
						</telerik:RadNumericTextBox></td>
				</tr>
				<tr>
					<td height="14" style="WIDTH: 82px"></td>
					<td height="14"></td>
				</tr>
				<tr>
					<td colspan="2" align="center">
						<table id="Table2" cellspacing="1" cellpadding="1" border="0">
							<tr>
								<td align="center">
									<telerik:RadButton id="bntApply" runat="server" Text="Apply" EnableViewState="False" Skin="Office2010Blue" OnClick="bntApply_Click"
										Width="60px" >
									</telerik:RadButton></td>
								<td align="center">
									<telerik:RadButton id="btnCancel" runat="server" Text="Cancel" EnableViewState="False" Skin="Office2010Blue" OnClick="btnCancel_Click"
										Width="60px" >
									</telerik:RadButton>
								</td>
							</tr>
						</table>
						<asp:TextBox id="txtPublication" runat="server" Width="2px" Visible="False"></asp:TextBox>
						<asp:TextBox id="txtSection" runat="server" Width="2px" Visible="False"></asp:TextBox>
						<asp:TextBox id="txtEdition" runat="server" Width="2px" Visible="False"></asp:TextBox>
						<asp:TextBox id="txtPubDate" runat="server" Width="2px" Visible="False"></asp:TextBox>
						<asp:TextBox id="txtPress" runat="server" Width="2px" Visible="False"></asp:TextBox>
					</td>
				</tr>
			</table>
			<asp:Label id="InjectScript" runat="server"></asp:Label>
         
		</form>
	</body>
</html>
