<%@ Page language="c#" Codebehind="Message.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.Message" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Density map of page</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
		<script type="text/javascript">
			function doClose()
			{
				<% if (doClose>0)  { %>
			        opener.document.Form1.HiddenReturendFromPopup.value = '1';
					opener.document.Form1.submit();				
					parent.window.close();
				<% } %>
			
			}
			function GetRadWindow()
			{
				var oWindow = null;
				if (window.radWindow)
					oWindow = window.radWindow;
				else if (window.frameElement.radWindow)
					oWindow = window.frameElement.radWindow;
				return oWindow;
			}
			
			function CloseOnReload()
			{
				GetRadWindow().Close();
			}
			
			function RefreshParentPage()
			{
				GetRadWindow().BrowserWindow.location.reload();
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
	<body onload="doClose()">
		<form id="Form1" method="post" runat="server">
                        <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>

			<TABLE id="Table1" cellSpacing="1" cellPadding="1" width="700" border="0" style="HEIGHT: 540px">
				<TR>
					<TD vAlign="top" align="left" width="700" colSpan="2"></TD>
				</TR>
				<TR>
					<TD align="left" colSpan="2" vAlign="top" style="HEIGHT: 31px" width="700">
						<asp:Label id="lblText" runat="server" Font-Bold="False" Font-Size="11" ForeColor="IndianRed">Label</asp:Label></TD>
				</TR>
				<TR>
					<TD vAlign="top" align="center" style="WIDTH: 574px">
						<asp:Image id="imgPreview" runat="server" Height="440px" ImageAlign="Middle"></asp:Image></TD>
					<td vAlign="top" align="right">
						<asp:Panel id="Panel1" runat="server">
							<TABLE style="WIDTH: 125px; HEIGHT: 160px" id="Table2" border="0" cellSpacing="0" cellPadding="0"
								width="125">
								<TR>
									<TD noWrap>&nbsp;</TD>
									<TD colSpan="2" noWrap>
										<asp:Label id="LabelDensities" runat="server" Font-Size="Smaller" CssClass="Text">Density map</asp:Label></TD>
								</TR>
								<TR>
									<TD style="HEIGHT: 18px" bgColor="#ffffff" width="20" noWrap></TD>
									<TD style="HEIGHT: 18px" width="5" noWrap></TD>
									<TD style="HEIGHT: 18px" noWrap>
										<asp:Label id="Label1" runat="server" CssClass="Text">D = 0</asp:Label></TD>
								</TR>
								<TR>
									<TD bgColor="#ffffc0" noWrap></TD>
									<TD noWrap></TD>
									<TD noWrap>
										<asp:Label id="Label2" runat="server" CssClass="Text">D <                    100</asp:Label></TD>
								</TR>
								<TR>
									<TD bgColor="#ffff80" noWrap></TD>
									<TD noWrap></TD>
									<TD noWrap>
										<asp:Label id="Label3" runat="server" CssClass="Text">100 < D <                                                          200</asp:Label></TD>
								</TR>
								<TR>
									<TD bgColor="#ffff00" noWrap></TD>
									<TD noWrap></TD>
									<TD noWrap>
										<asp:Label id="Label4" runat="server" CssClass="Text">200 < D <                                                          225</asp:Label></TD>
								</TR>
								<TR>
									<TD bgColor="#ffc040" noWrap></TD>
									<TD noWrap></TD>
									<TD noWrap>
										<asp:Label id="Label5" runat="server" CssClass="Text">225 < D <                                                          250</asp:Label></TD>
								</TR>
								<TR>
									<TD bgColor="#ff8000" noWrap></TD>
									<TD noWrap></TD>
									<TD noWrap>
										<asp:Label id="Label6" runat="server" CssClass="Text">250 < D <                                                          275</asp:Label></TD>
								</TR>
								<TR>
									<TD bgColor="#ff4000" noWrap></TD>
									<TD noWrap></TD>
									<TD noWrap>
										<asp:Label id="Label7" runat="server" CssClass="Text">250 < D <                                                          275</asp:Label></TD>
								</TR>
								<TR>
									<TD bgColor="#ff0000" noWrap></TD>
									<TD noWrap></TD>
									<TD noWrap>
										<asp:Label id="Label8" runat="server" CssClass="Text">275 <                    D</asp:Label></TD>
								</TR>
							</TABLE>
						</asp:Panel>
						<asp:Panel id="Panel2" runat="server">
							<TABLE style="WIDTH: 125px; HEIGHT: 160px" id="Table3" border="0" cellSpacing="0" cellPadding="0"
								width="125">
								<TR>
									<TD noWrap>&nbsp;</TD>
									<TD colSpan="2" noWrap>
										<asp:Label id="Label9" runat="server" Font-Size="Smaller" CssClass="Text">Density map</asp:Label></TD>
								</TR>
								<TR>
									<TD style="HEIGHT: 18px" bgColor="#ffffff" width="20" noWrap></TD>
									<TD style="HEIGHT: 18px" width="5" noWrap></TD>
									<TD style="HEIGHT: 18px" noWrap>
										<asp:Label id="Label10" runat="server" CssClass="Text">D <                    10</asp:Label></TD>
								</TR>
								<TR>
									<TD bgColor="#e8e8e8" noWrap></TD>
									<TD noWrap></TD>
									<TD noWrap>
										<asp:Label id="Label12" runat="server" CssClass="Text">10 < D <                                                          200</asp:Label></TD>
								</TR>
								<TR>
									<TD bgColor="#d4d4d4" noWrap></TD>
									<TD noWrap></TD>
									<TD noWrap>
										<asp:Label id="Label13" runat="server" CssClass="Text">200 < D <                                                          225</asp:Label></TD>
								</TR>
								<TR>
									<TD bgColor="#c0c0c0" noWrap></TD>
									<TD noWrap></TD>
									<TD noWrap>
										<asp:Label id="Label14" runat="server" CssClass="Text">225 < D <                                                          250</asp:Label></TD>
								</TR>
								<TR>
									<TD bgColor="#ff0000" noWrap></TD>
									<TD noWrap></TD>
									<TD noWrap>
										<asp:Label id="Label15" runat="server" CssClass="Text">250 <                    D</asp:Label></TD>
								</TR>
							</TABLE>
						</asp:Panel>
					</td>
				</TR>
				<TR>
					<TD colspan="2" align="center">
						<asp:button id="btnCancel" runat="server" Text="Close" BackColor="LightBlue" EnableViewState="False"
							Width="60px" Height="24px" Visible="False"></asp:button>
					</TD>
				</TR>
			</TABLE>
		</form>
	</body>
</html>
