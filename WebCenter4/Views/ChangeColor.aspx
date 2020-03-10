<%@ Page language="c#" Codebehind="ChangeColor.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.ChangeColor" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Change colors</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
		<script  type="text/javascript">
		<!--
			var initWidth;
						
			function CloseOnReload()
			{
			    //				GetRadWindow().BrowserWindow.document.Form1.txtReturnedFromColor.value = '0';
			    GetRadWindow().BrowserWindow.document.Form1.HiddenReturendFromPopup.value = '0';
				GetRadWindow().Close();
			}
			
			function RefreshParentPage() 
			{			
			    //GetRadWindow().BrowserWindow.document.Form1.txtReturnedFromColor.value = '1';
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

			<table id="Table1" style="WIDTH: 240px; HEIGHT: 68px" cellspacing="1" cellpadding="1" width="240"
				border="0">
				<tr>
					<td align="center"><asp:label id="LabelHeader" runat="server">Colors for page X</asp:label></td>
				</tr>
				<tr>
					<td style="height: 10px;"></td>
				</tr>
				<tr>
					<td align="center">
						<table id="Table3" style="WIDTH: 240px; HEIGHT: 24px" cellspacing="0" cellpadding="0" width="240"
							border="0">
							<tr>
								<td noWrap align="center" bgcolor="#80FFFF" ><asp:checkbox id="CheckBoxC" runat="server" ForeColor="Black" BackColor="#80FFFF" Text="C"></asp:checkbox></td>
								<td noWrap align="center" bgcolor="#ff00ff"><asp:checkbox id="CheckBoxM" runat="server" ForeColor="Black" BackColor="Magenta" Text="M"></asp:checkbox></td>
								<td noWrap align="center" bgcolor="#ffff00"><asp:checkbox id="CheckBoxY" runat="server" ForeColor="Black" BackColor="Yellow" Text="Y"></asp:checkbox></td>
								<td noWrap align="center" bgcolor="#000000" ><asp:checkbox id="CheckBoxK" runat="server" ForeColor="White"  BackColor="Black" Text="K"></asp:checkbox></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td style="height: 10px;"></td>
				</tr>
				<tr>
					<td align="center">
						<table id="Table2" cellspacing="1" cellpadding="1" width="200" border="0">
							<tr>
								<td align="center"><telerik:RadButton id="bntApply" runat="server" Text="Apply" EnableViewState="False" Skin="Office2010Blue" OnClick="bntApply_Click"
										 Width="70px"></telerik:RadButton></td>
								<td align="center"><telerik:RadButton id="btnCancel" runat="server"  Text="Cancel" EnableViewState="False" Skin="Office2010Blue" OnClick="btnCancel_Click"
										 Width="70px"></telerik:RadButton></td>
							</tr>
						</table>
						<asp:label id="lblError" runat="server" ForeColor="Red"></asp:label>
						<asp:Label id="InjectScript" runat="server"></asp:Label></td>
				</tr>
			</table>
            <input runat="server" id="hiddenMasterCopySeparationSet" type="hidden" value="0" />
            <input runat="server" id="hiddenMasterCopySeparationSet2" type="hidden" value="0" />
		</form>
	</body>
</html>
