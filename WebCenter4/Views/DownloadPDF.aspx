<%@ Page language="c#" Codebehind="DownloadPDF.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.DownloadPDF" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Download previews</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet">
		<script type="text/javascript">
            function CloseOnReload() {
                GetRadWindow().Close();
            }

            function RefreshParentPage() {
                GetRadWindow().BrowserWindow.document.forms[0].submit();
                GetRadWindow().Close();
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
                margin: 2px;  
                padding: 0px;  
                overflow: hidden;  
            }  
        </style> 

	</head>

	<body onload="doClose()" >
        
		<form id="Form1" method="post" runat="server" enctype="multipart/form-data">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
           
            <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;height: 24px;">
                <Items>
                    <telerik:RadToolBarButton runat="server" Value="Item1">
                        <ItemTemplate>
                            <asp:Label ID="lblDownloadPDF" runat="server" Text="Download PDF page" CssClass="RadToolbarLabel"></asp:Label>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>
                </Items>
            </telerik:RadToolBar>
			<table id="Table1" style="width: 480px; height: 216px" cellspacing="1" cellpadding="1" width="480" border="0">
				<tr>
					<td>
						<asp:Label id="Label1" runat="server">Page to download</asp:Label>
					</td>
					<td colspan="2">
                        <asp:Label id="lblFileName" runat="server" EnableViewState="true"></asp:Label>
					</td>
				</tr>
				
				<tr>
					<td style="height: 22px" valign="top" align="left">
						<asp:Label id="lblPageTypes" runat="server">PDF type</asp:Label>

					</td>
					<td valign="top" align="left" colspan="2" >
						<asp:RadioButtonList id="RadioButtonListPreviewtype" runat="server" Width="183px" CellPadding="0" CellSpacing="0"
							RepeatDirection="Horizontal" RepeatLayout="Flow">
							<asp:ListItem Value="PDFLowres" Selected="True">Lowres PDF (RGB)</asp:ListItem>
							<asp:ListItem Value="PDFHighres" Selected="False">Highres PDF (RGB)</asp:ListItem>
							<asp:ListItem Value="PDFPrint" Selected="False">Print PDF (CMYK)</asp:ListItem>
						</asp:RadioButtonList></td>
				</tr>
				<tr>
					<td colspan="3" align="center">
						<table id="Table2" style="width: 185px; height: 24px" cellspacing="1" cellpadding="1" width="185"
							border="0">
							<tr>
								<td align="center">
									<telerik:RadButton id="btnDownload" runat="server" Text="Download" Skin="Office2010Blue" OnClick="btnSave_Click"
										EnableViewState="False" Width="70px"></telerik:RadButton></td>
								<td align="center">
									<telerik:RadButton id="btnClose" runat="server" Width="70px" Skin="Office2010Blue" EnableViewState="False" OnClick="btnClose_Click"
										Text="Cancel"></telerik:RadButton></td>
							</tr>
						</table>
					</td>
				</tr>
                <tr>
                    <td colspan="3" >
                          <asp:Label id="lblError" runat="server" EnableViewState="true" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
			</table>
            <asp:label id="InjectScript" runat="server"></asp:label>
             <input runat="server" id="hiddenMasterCopySeparationSet" type="hidden" value="0" />
           
		</form>
	</body>
</html>
