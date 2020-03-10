<%@ Page language="c#" Codebehind="ShowMessage.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.ShowMessage" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Message</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
		<script language="JavaScript" type="text/javascript">
			function doClose()
			{
				<% if (doClose>0)  { %>
			        opener.document.Form1.HiddenReturendFromPopup.value = '1';
					opener.document.Form1.submit();				
					parent.window.close();
				<% } %>
			
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

			<table id="Table1" style="WIDTH: 600px; HEIGHT: 750px" cellspacing="1" cellpadding="1"
				width="600" border="0">
				<tr>
					<td style="HEIGHT: 25px" align="left" colspan="2"><asp:label id="Label1" runat="server" CssClass="content">Message</asp:label></td>
				</tr>
				<tr>
					<td style="HEIGHT: 1px" align="left" colspan="2"><asp:datagrid id="DataGrid1" runat="server" CssClass="Text" BackColor="White" BorderColor="#999999"
							BorderStyle="None" BorderWidth="1px" CellPadding="3" AllowSorting="True" AutoGenerateColumns="False" HorizontalAlign="Left" AllowPaging="True">
							<FooterStyle ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" BackColor="#000084"></HeaderStyle>
							<Columns>
								<asp:ButtonColumn Text="&lt;img src='./../Images/Edit.gif' &gt;" CommandName="Select"></asp:ButtonColumn>
								<asp:ButtonColumn Text="&lt;img src='./../Images/wastewhite.gif' onclick='return confirm_delete();'&gt;"
									CommandName="Delete"></asp:ButtonColumn>
								<asp:TemplateColumn HeaderText="!">
									<ItemTemplate>
										<asp:ImageButton id="imgSeverity" runat="server" CommandName="Markread" ImageUrl="../Images/mail.gif"></asp:ImageButton>
									</ItemTemplate>
									<EditItemTemplate>
										<asp:TextBox id=TextBox1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Severity") %>'>
										</asp:TextBox>
									</EditItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn DataField="EventTime" SortExpression="EventTime" HeaderText="Time"></asp:BoundColumn>
								<asp:BoundColumn DataField="Title" SortExpression="Title" HeaderText="Subject"></asp:BoundColumn>
								<asp:BoundColumn DataField="Sender" SortExpression="Sender" HeaderText="Sender"></asp:BoundColumn>
								<asp:BoundColumn DataField="Product" HeaderText="Product"></asp:BoundColumn>
								<asp:BoundColumn DataField="Message" SortExpression="Message" HeaderText="Message"></asp:BoundColumn>
								<asp:BoundColumn Visible="False" DataField="MessageID" HeaderText="ID"></asp:BoundColumn>
							</Columns>
							<PagerStyle HorizontalAlign="Center" ForeColor="Black" BackColor="#999999" Mode="NumericPages"></PagerStyle>
						</asp:datagrid></td>
				</tr>
				<tr>
					<td style="HEIGHT: 1px" align="left" colspan="2">
						<asp:button id="btnNewMessage" runat="server" BackColor="PowderBlue" Width="90px" Height="24px"
							EnableViewState="False" Text="New message"></asp:button></td>
				</tr>
				<tr>
					<td style="HEIGHT: 1px" align="left" colspan="2"><asp:label id="lblError" runat="server" ForeColor="Red"></asp:label></td>
				</tr>
				<tr>
					<td style="HEIGHT: 134px" valign="top" align="left" colspan="2"><asp:panel id="panelFields" runat="server">
							<table id="Table2" style="WIDTH: 488px; HEIGHT: 49px" cellspacing="1" cellpadding="1" width="488"
								border="0">
								<tr>
									<td>
										<asp:label id="LblFrom" runat="server" CssClass="Text">From</asp:label></td>
									<td>
										<asp:textbox id="txtFrom" runat="server" Width="144px" Rows="30"></asp:textbox></td>
									<td>
										<asp:Label id="lblTo" runat="server" CssClass="Text">To</asp:Label></td>
									<td>
										<asp:TextBox id="txtTo" runat="server" Width="134px" Rows="30"></asp:TextBox></td>
								</tr>
								<tr>
									<td>
										<asp:Label id="Label2" runat="server" CssClass="Text">Production</asp:Label></td>
									<td>
										<asp:DropDownList id="ddProductions" runat="server" Width="152px"></asp:DropDownList></td>
									<td>
										<asp:Label id="Label3" runat="server" CssClass="Text">Prio</asp:Label></td>
									<td valign="middle" align="left">
										<asp:RadioButtonList id="RadioButtonListPrio" runat="server" CssClass="Text" CellPadding="0" RepeatDirection="Horizontal"
											CellSpacing="0">
											<asp:ListItem Value="Normal" Selected="True">Normal</asp:ListItem>
											<asp:ListItem Value="High">High</asp:ListItem>
										</asp:RadioButtonList></td>
								</tr>
								<tr>
									<td>
										<asp:Label id="lblSent" runat="server" CssClass="Text">Sent</asp:Label></td>
									<td>
										<asp:Label id="LblEventTime" runat="server" CssClass="Text">EventTime</asp:Label></td>
									<td>
										<asp:Label id="lblID" runat="server" CssClass="Text">ID</asp:Label></td>
									<td>
										<asp:Label id="lblMessageID" runat="server" CssClass="Text" Enabled="False"></asp:Label></td>
								</tr>
								<tr>
									<td>
										<asp:Label id="lblSubject" runat="server" CssClass="Text">Subject</asp:Label></td>
									<td colspan="3">
										<asp:TextBox id="txtSubject" runat="server" Width="367px" Rows="30"></asp:TextBox></td>
								</tr>
								<tr>
									<td>
										<asp:Label id="lblMessage" runat="server" CssClass="Text">Message</asp:Label></td>
									<td colspan="3"></td>
								</tr>
								<tr>
									<td colspan="4">
										<asp:textbox id="txtMessage" runat="server" Height="116px" Width="590px" Rows="3" TextMode="MultiLine"
											Wrap="False"></asp:textbox></td>
								</tr>
								<tr>
									<td align="center" colspan="4">
										<table id="Table3" cellspacing="0" cellpadding="0" width="200" border="0">
											<tr>
												<td noWrap align="center">
													<asp:button id="btnAddToMessage" runat="server" BackColor="PowderBlue" Text="Send" EnableViewState="False"
														Height="24px" Width="90px"></asp:button></td>
												<td noWrap align="center">
													<asp:button id="btnReply" runat="server" BackColor="PowderBlue" Text="Reply" EnableViewState="False"
														Height="24px" Width="90px"></asp:button></td>
												<td noWrap align="center">
													<asp:button id="btnCancel" runat="server" BackColor="LightGray" Text="Cancel" EnableViewState="False"
														Height="24px" Width="90px" CausesValidation="False"></asp:button></td>
											</tr>
										</table>
									</td>
								</tr>
							</table>
						</asp:panel></td>
				</tr>
			</table>
		</form>
	</body>
</html>
