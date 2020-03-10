<%@ Page language="c#" Codebehind="ChangeColorPDF.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.ChangeColorPDF" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Change PDF colors</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
		<script  type="text/javascript">
		<!--
			var initWidth;
						
			function CloseOnReload()
			{
			    GetRadWindow().BrowserWindow.document.Form1.HiddenReturendFromPopup.value = '0';
				GetRadWindow().Close();
			}
			
			function RefreshParentPage() 
			{			
			    GetRadWindow().BrowserWindow.document.Form1.HiddenReturendFromPopup.value = '1';
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
	<body>
		<form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
			<table id="Table1" style="WIDTH: 240px; HEIGHT: 88px" cellspacing="1" cellpadding="1" width="240"
				border="0">
				<tr>
					<td align="center"><asp:label id="LabelHeader" runat="server">Colors for page X</asp:label></td>
				</tr>
				<tr>
					<td align="center">

						<asp:RadioButtonList id="RadioButtonListPDFcolors" runat="server" Width="159px">
							<asp:ListItem Value="PDF color" Selected="True">PDF color</asp:ListItem>
							<asp:ListItem Value="PDF mono">PDF mono</asp:ListItem>
						</asp:RadioButtonList>
					</td>
				</tr>
				<tr>
					<td align="center">
						<table id="Table2" cellspacing="1" cellpadding="1" width="200" border="0">
							<tr>
								<td align="center"><telerik:RadButton id="bntApply" runat="server" Text="Apply" EnableViewState="False" Skin="Office2010Blue" OnClick="bntApply_Click"
										 Width="70px"></telerik:RadButton></td>
								<td align="center"><telerik:RadButton id="btnCancel" runat="server" Text="Cancel" EnableViewState="False" Skin="Office2010Blue" OnClick="btnCancel_Click"
										 Width="70px"></telerik:RadButton></td>
							</tr>
						</table>
						<asp:label id="lblError" runat="server" ForeColor="Red"></asp:label>
						<asp:Label id="InjectScript" runat="server"></asp:Label></td>
				</tr>
			</table>
		</form>
	</body>
</html>
