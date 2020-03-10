<%@ Page language="c#" Codebehind="HardProof.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.HardProof" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Hardproof</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
		<script  type="text/javascript">
			var initWidth;
						
			function CloseOnReload()
			{
				//GetRadWindow().BrowserWindow.document.Form1.txtReturnedFromHardProof.value = '0';
				GetRadWindow().Close();
			}
			
			function RefreshParentPage() 
			{			
				//GetRadWindow().BrowserWindow.document.Form1.txtReturnedFromHardProof.value = '1';
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

			<table id="Table1" cellspacing="1" cellpadding="1" width="304" border="0" align="left"
				style="width: 304px; height: 88px">
				<tr>
					<td>
						<asp:Label id="lblTemplate" runat="server" Font-Names="Verdana" Font-Size="10pt">Hardproof configuration</asp:Label></td>
					<td>
						<asp:DropDownList id="DropDownList1" runat="server"></asp:DropDownList></td>
				</tr>
				<tr>
					<td></td>
					<td>
						<asp:CheckBox id="CheckBoxIgnoreApproval" runat="server" Text="Ignore approval" Font-Names="Verdana"
							Font-Size="10pt" ToolTip="Print hardproof regardless of page approval"></asp:CheckBox></td>
				</tr>
				<tr>
					<td colspan="2">
						<table id="Table2" cellspacing="1" cellpadding="1" width="200" border="0" align="center">
							<tr>
								<td align="center">
									<telerik:RadButton  id="bntApply" runat="server" EnableViewState="False" OnClick="bntApply_Click" Skin="Office2010Blue" 
										Text="Apply" Width="70px"></telerik:RadButton></td>
								<td align="center">
									<telerik:RadButton id="btnCancel" runat="server"  EnableViewState="False" Skin="Office2010Blue" OnClick="btnCancel_Click"
										Text="Cancel" Width="70px"></telerik:RadButton></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			<asp:Label id="InjectScript" runat="server"></asp:Label>
		</form>
	</body>
</html>
