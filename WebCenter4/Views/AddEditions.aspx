<%@ Page language="c#" Codebehind="AddEditions.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.AddEditions" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>AddEditions</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
			<script type="text/javascript">

			    function doClose()
			    {
				    if (<%= doClose %> >0)  {
					    opener.document.Form1.submit();	
					    parent.window.close();
				    }			
			    }

			</script>

            <style type="text/css">  
            html, body, form  
            {  
                height: 100%;  
                margin: 0px;  
                padding: 0px;  
               
            }  
                .auto-style1
                {
                    width: 224px;
                    height: 75px;
                }
                .auto-style2
                {
                    height: 75px;
                }
        </style>
	</head>
	<body onload="doClose()">
		<form id="Form1" method="post" runat="server" defaultfocus="ddlEditions">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>

            <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;">
                <Items>
                    <telerik:RadToolBarButton runat="server" Value="Item1">
                        <ItemTemplate>
                            <asp:Label ID="LabelEditionsHeader" runat="server" Text="Add subeditions to planned product" CssClass="RadToolbarLabel"></asp:Label>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>
                </Items>
            </telerik:RadToolBar>
    			
            <table style="height: 568px; width: 440px; border: 0px;" id="Table1" cellspacing="1" cellpadding="1">
				<tr>
					<td style="height: 8px; width: 224px;" align="left"></td>
                    <td style="height: 8px;" align="left"></td>
					<td style="height: 8px;" align="left"></td>
				</tr>
				<tr>
					<td style="height: 25px; width: 224px;" align="left">
                        <asp:label id="lblEdition" runat="server" CssClass="LabelNormal">Edition name</asp:label>
                    </td>
					<td style="height: 25px;" align="left">
                        <asp:dropdownlist id="ddlEditions" runat="server" Width="152px"></asp:dropdownlist>
                    </td>
					<td style="height: 25px;" align="left"></td>
				</tr>
				<tr>
                    <td style="height: 14px; width: 224px;" align="left">
                        <asp:label id="Label1" runat="server" CssClass="LabelNormal">Edition type</asp:label>
                    </td>
					<td style="height: 14px;" align="left">
						<asp:RadioButtonList id="RadioButtonListEditionType" runat="server" Height="14px" Width="144px" RepeatDirection="Horizontal"
							Font-Names="Verdana" Font-Size="10pt" BorderStyle="None" RepeatLayout="Flow" AutoPostBack="True">
							<asp:ListItem Value="Zoned" Selected="True">Zoned</asp:ListItem>
							<asp:ListItem Value="Timed">Timed</asp:ListItem>
						</asp:RadioButtonList>
                    </td>
                    <td style="height: 14px;" align="left"></td>
				</tr>
				<tr>
					<td style="height: 14px; width: 224px;" align="left">
						<asp:label style="Z-INDEX: 0" id="Label2" runat="server" CssClass="LabelNormal">Timed edition parent</asp:label></td>
					<td style="height: 14px;" align="left">
						<asp:dropdownlist style="Z-INDEX: 0" id="ddlEditionsFrom" runat="server" Width="152px"></asp:dropdownlist></td>
					<td style="height: 14px;" align="left"></td>
				</tr>
				<tr>
					<td align="left" class="auto-style1"><asp:label id="lblDefaultPageType" runat="server" CssClass="LabelNormal">Default page type for new subedition</asp:label></td>
					<td align="left" class="auto-style2"><asp:dropdownlist id="ddDeafultPageUnique" runat="server" Width="152px">
							<asp:ListItem Value="Common page" Selected="True">Common page</asp:ListItem>
							<asp:ListItem Value="Unique page">Unique page</asp:ListItem>
						</asp:dropdownlist></td>
					<td align="left" class="auto-style2"></td>
				</tr>
                <tr>
					<td align="left" width="224">
						<asp:label style="Z-INDEX: 0" id="lblComment" runat="server" CssClass="LabelNormal">Comment</asp:label></td>
					<td align="left">
						<telerik:RadTextBox style="Z-INDEX: 0" id="txtComment" Width="200px" Skin="Default" Runat="server"
							Wrap="False" Rows="1"></telerik:RadTextBox></td>
					<td align="left" height="8"></td>
				</tr>
				<tr>
					<td style="width: 224px" align="left">
						<asp:label id="lblCirculation" runat="server" CssClass="LabelNormal">Circulation</asp:label></td>
					<td align="left">
						<telerik:RadNumericTextBox id="RadNumericTextBoxCirculation" Width="144px" Runat="server" Skin="Default"
							ShowSpinButtons="True" Value="0" MinValue="0">
							<NumberFormat AllowRounding="True" GroupSizes="3" DecimalSeparator="," NegativePattern="-n" GroupSeparator="."
								DecimalDigits="0" PositivePattern="n"></NumberFormat>
							<IncrementSettings Step="100"></IncrementSettings>
						</telerik:RadNumericTextBox></td>
					<td style="height: 8px" align="left">
                        
                    </td>
				</tr>
				<tr>
					<td style="width: 224px" align="left">
						<asp:label id="lblCirculation2" runat="server" CssClass="LabelNormal">Additional circulation</asp:label></td>
					<td align="left">
                        
						<telerik:RadNumericTextBox id="RadNumericTextBoxCirculation2" Width="144px" Runat="server" Skin="Default"
							ShowSpinButtons="True" MinValue="0" ShowButton="False" Value="0">
							<NumberFormat AllowRounding="True" GroupSizes="3" DecimalSeparator="," NegativePattern="-n" GroupSeparator="."
								DecimalDigits="0" PositivePattern="n"></NumberFormat>
							<IncrementSettings Step="100"></IncrementSettings>
						</telerik:RadNumericTextBox></td>
					<td style="height: 8px" align="left"></td>
				</tr>
				<tr>
					<td style="width: 224px" align="left">
						<asp:label id="lblOrderNumber" runat="server" CssClass="LabelNormal">Order number</asp:label></td>
					<td align="left">
						<telerik:RadTextBox id="txtOrderNumber" Width="124px" Runat="server" Skin="Default" Rows="1" Wrap="False"></telerik:RadTextBox></td>
					<td style="height: 8px" align="left"></td>
				</tr>
				<tr>
					<td style="width: 224px; height: 8px;" align="left"></td>
					<td style="height: 8px" align="center"><asp:button id="brnAdd" runat="server" Height="24px" BackColor="LightBlue" Width="60px" Text="Add"
							EnableViewState="False"></asp:button></td>
					<td style="height: 8px" align="left"></td>
				</tr>
				<tr>
					<td style="width: 224px; height: 8px;" align="left"><asp:label id="lblEditions" runat="server" CssClass="LabelNormal">Editions</asp:label></td>
					<td style="height: 8px" align="left"></td>
					<td style="height: 8px" align="left"></td>
				</tr>
				<tr>
					<td style="vertical-align: top" align="center" colspan="3"><asp:datagrid id="DataGridEditions" runat="server" BackColor="White" BorderStyle="None" Font-Size="10pt"
							Font-Names="Verdana" ShowFooter="True" CellSpacing="2" HorizontalAlign="Left" GridLines="Horizontal" BorderWidth="1px" BorderColor="#E7E7FF"
							CellPadding="3">
							<FooterStyle ForeColor="#4A3C8C" BackColor="#B5C7DE"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="#F7F7F7" BackColor="#738A9C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="#F7F7F7"></AlternatingItemStyle>
							<ItemStyle ForeColor="#4A3C8C" BackColor="#E7E7FF"></ItemStyle>
							<HeaderStyle Font-Bold="False" ForeColor="#F7F7F7" BackColor="#4A3C8C"></HeaderStyle>
							<PagerStyle HorizontalAlign="Right" ForeColor="#4A3C8C" BackColor="#E7E7FF" Mode="NumericPages"></PagerStyle>
						</asp:datagrid>
                    </td>
				</tr>
				<tr>
					<td style="height: 17px" align="left" colspan="2">
                        <asp:label id="lblError" runat="server" ForeColor="Red" Font-Size="10pt"></asp:label>
                        <asp:label id="lblInfo" runat="server" ForeColor="LimeGreen" Font-Size="10pt"></asp:label>
                    </td>
					<td style="height: 17px" align="center"></td>
				</tr>
				<tr>
					<td style="height: 30px; vertical-align: top;"  align="center" colspan="2" >
						<table style="height: 26px; width: 176px; border: 0px" id="Table2" cellspacing="0" cellpadding="1">
							<tr>
								<td align="center">
                                    <asp:button id="btnSave" runat="server" Height="24px" BackColor="LightBlue" Width="60px" Text="Save"
										EnableViewState="False"></asp:button>
                                </td>
								<td align="center">
                                    <asp:button id="btnCancel" runat="server" Height="24px" BackColor="LightGray" Width="60px" Text="Cancel"
										EnableViewState="False" CausesValidation="False"></asp:button>
                                </td>
							</tr>
						</table>
					</td>
					<td style="height: 30px; vertical-align: top;" align="center"></td>
				</tr>
			</table>
		</form>
	</body>
</html>
