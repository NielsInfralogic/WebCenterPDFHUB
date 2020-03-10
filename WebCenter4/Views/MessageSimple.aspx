<%@ Page language="c#" Codebehind="MessageSimple.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.MessageSimple" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Message</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
		<script  type="text/javascript">
		<!--
			function doClose()
			{
				<% if (doClose>0)  { %>
			    opener.document.Form1.HiddenReturendFromPopup.value = '1';
					opener.document.Form1.submit();				
					parent.window.close();
				<% } %>
			
			}
		//-->
		</script>
         <style type="text/css">  
            html, body, form  
            {  
                height: 100%;  
                margin: 2px;  
                padding: 0px;  
                overflow: hidden;  
            }  
        </style> 
	</head>
	<body onload="doClose()">
		<form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>

			<table id="Table1" cellspacing="1" cellpadding="1" width="604" border="0" style="HEIGHT: 404px">
				<tr>
					<td align="center" colspan="2">
						<asp:TextBox id="TextBox" runat="server" Width="600px" Height="400px" TextMode="MultiLine" Wrap="False"></asp:TextBox></td>
				</tr>
				<tr>
					<td colspan="2" align="center">
						<telerik:RadButton id="btnCancel" runat="server" Text="Close" EnableViewState="False" Skin="Office2010Blue" OnClick="btnCancel_Click"
							Width="70px"  Visible="False"></telerik:RadButton>
					</td>
				</tr>
			</table>
		</form>
	</body>
</html>
