<%@ Page language="c#" Codebehind="ReportArchive.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.ReportArchive" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Report Archive</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet"/>
		<script language="JavaScript" type="text/javascript">
			
            function doClose()
			{
				 if (<%=doClose %> > 0)  { 
					parent.window.close();
				}			
			}

            function confirm_delete() {
		        if (confirm("Detele report?")==false)
			        return false;
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
			<table id="Table1" style="height: 400px; width: 400px; border: 0px" cellspacing="1" cellpadding="1" align="center">
				<tr align="center">
					<td style="vertical-align: top" align="center" colspan="2">
						<asp:datagrid id="DataGrid1" runat="server" CssClass="Text" AllowPaging="True" HorizontalAlign="Left"
							AutoGenerateColumns="False" AllowSorting="True" CellPadding="3" BorderWidth="1px" BorderStyle="None"
							BorderColor="#999999" BackColor="White" PageSize="11">
							<FooterStyle ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" Wrap="False" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
							<EditItemStyle Wrap="False"></EditItemStyle>
							<AlternatingItemStyle Wrap="False" BackColor="Gainsboro"></AlternatingItemStyle>
							<ItemStyle Wrap="False" ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
							<HeaderStyle Font-Bold="True" Wrap="False" ForeColor="White" BackColor="#000084"></HeaderStyle>
							<Columns>
								<asp:TemplateColumn>
									<ItemTemplate>
										<asp:LinkButton id="LinkButton1" runat="server" Text="<img src='./../Images/download.gif' >" CausesValidation="false"
											CommandName="Download" ToolTip="Download">
											<img src='./../Images/download.gif'></asp:LinkButton>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:TemplateColumn>
									<ItemTemplate>
										<asp:LinkButton id="LinkButton2" runat="server" Text="<img src='./../Images/wastewhite.gif' onclick='return confirm_delete();'>"
											CausesValidation="false" CommandName="Delete" ToolTip="Delete">
											<img src='./../Images/wastewhite.gif' onclick='return confirm_delete();'></asp:LinkButton>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn DataField="Product" SortExpression="Product" HeaderText="Product"></asp:BoundColumn>
							</Columns>
							<PagerStyle Font-Size="Small" HorizontalAlign="Center" ForeColor="Black" Position="TopAndBottom"
								BackColor="#999999" Mode="NumericPages"></PagerStyle>
						</asp:datagrid>
                    </td>
				</tr>
				<tr>
					<td style="height: 1px" align="left" colspan="2">
						<asp:Label id="LblError" runat="server" ForeColor="Red"></asp:Label>
                    </td>
				</tr>
			</table>
		</form>
	</body>
</html>
