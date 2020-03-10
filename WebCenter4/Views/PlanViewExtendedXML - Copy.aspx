<%@ Page language="c#" Codebehind="PlanViewExtendedXMLOld.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.PlanViewExtendedXMLOld" validateRequest="false" %>

<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Plans</title>
		<link rel="stylesheet" type="text/css" href="../Styles/WebCenter.css" />
		<script language="JavaScript" type="text/javascript"> 
		    if ( <%=updateTree %> > 0) { 
			    parent.parent.tree.document.Form1.action = 'Tree.aspx?refresh=1';
			    parent.parent.tree.document.Form1.submit();
		    }
		 

	        function ShowProgress()
	        {
		        var progress = document.getElementById("ProgressBar");
		        progress.style.display = "";
	        }
	        function StopProgress()
	        {
		        var progress = document.getElementById("ProgressBar");
		        progress.style.display = "none";
	        }	
	
	        function confirm_delete() {
		        if (confirm("Slet denne plan?")==false)
			        return false;
	        }
		</script>
        <style type="text/css">  
            html, body, form  
            {  
                margin: 0px;  
                padding: 0px;  
            }  
        </style> 
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

            <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;" CssClass="smallToolBar">
                <Items>
                    <telerik:RadToolBarButton runat="server" Value="Item1">
                        <ItemTemplate>
                            <div style="height:22px;margin-top:6px;">
                                <asp:Label ID="LabelPlanHeader" runat="server" Text="Add new plan" CssClass="RadToolbarLabel" Height="22"></asp:Label>
                            </div>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>
                </Items>
            </telerik:RadToolBar>


            <input id="saveConfirm2" value="0" type="hidden" name="saveConfirm2" runat="server" />
            <input id="saveConfirm" value="0" type="hidden" name="saveConfirm" runat="server" />
            <div style="DISPLAY: none; FONT-SIZE: 10pt; color: green" id="ProgressBar"><img alt="" src="../images/indicator.gif" />&nbsp;&nbsp;Storing plan..</div>
            <div style="height:12px;"></div>
                                      
            <asp:panel id="PanelAddPlan" runat="server" Width="900" HorizontalAlign="Left" Wrap="False" >                               
				<table id="mainparametertable" style="border: 1px;text-align: left; width:inherit;" cellspacing="1" cellpadding="1">								   
					
                    <tr id="imposition1">
                        <td style="width: 161px; height: 14px; white-space:nowrap;">
							<asp:Label id="lblTemplate" runat="server" CssClass="LabelNormal">Template</asp:Label>
                        </td>
                        <td style="height: 14px; white-space:nowrap;" >
							<telerik:RadComboBox id="ddImpositionList" runat="server" Width="300px" SkinsPath="~/RadControls/ComboBox/Skins" Skin="Default" OnSelectedIndexChanged="ddImpositionList_SelectedIndexChanged" AutoPostBack="True"></telerik:RadComboBox>	    									
                            <asp:Label ID="lblImpositionInfo" runat="server" CssClass="LabelNormal"></asp:Label>
                        </td>
					</tr>

                    <tr id="publication">
                        <td style="width: 167px; height: 14px; white-space:nowrap;">
	    					<asp:Label id="lblPublication" runat="server" CssClass="LabelNormal">Publication</asp:Label>
                        </td>		    							
						<td style="white-space:nowrap;" >										
                            <telerik:RadComboBox id="ddPublicationList" runat="server" Width="180px" AutoPostBack="True" SkinsPath="~/RadControls/ComboBox/Skins" Skin="Default" OnSelectedIndexChanged="ddPublicationList_SelectedIndexChanged"></telerik:RadComboBox>	    									
                        </td>
                    </tr>

                    <tr id="pubdateweeknumber">
                        <td style="width: 167px; white-space:nowrap;">
							<asp:Label id="lblPubdate" runat="server" CssClass="LabelNormal">Publication date</asp:Label>
                        </td>
                        <td style="width: 247px; white-space:nowrap;">
							<telerik:RadDatePicker id="dateChooserPubDate" ToolTip="Select publication date" Runat="server" Culture="en-US">
                                <Calendar EnableWeekends="True" FastNavigationNextText="&amp;lt;&amp;lt;" UseColumnHeadersAsSelectors="False" UseRowHeadersAsSelectors="False" runat="server">
                                </Calendar>
                                <DateInput DateFormat="dd/MM/yyyy" DisplayDateFormat="dd/MM/yyyy" LabelWidth="40%" runat="server">
                                    <EmptyMessageStyle Resize="None" />
                                    <ReadOnlyStyle Resize="None" />
                                    <FocusedStyle Resize="None" />
                                    <DisabledStyle Resize="None" />
                                    <InvalidStyle Resize="None" />
                                    <HoveredStyle Resize="None" />
                                    <EnabledStyle Resize="None" />
                                </DateInput>
                                <DatePopupButton HoverImageUrl="" ImageUrl="" />
                            </telerik:RadDatePicker>
                        </td> 
                       
					</tr>

					<tr id="edition">
                        <td style="width: 167px; height: 14px; white-space:nowrap;">
	    					<asp:Label id="lblEdition" runat="server" CssClass="LabelNormal">Edition</asp:Label>
                        </td>		    							
						<td style="white-space:nowrap;" >										
                            <telerik:RadComboBox id="ddEditionList" runat="server" Width="180px" AutoPostBack="True" SkinsPath="~/RadControls/ComboBox/Skins" Skin="Default"></telerik:RadComboBox>	    									
                        </td>
                    </tr>

					<tr id="section1">
                        <td style="width: 167px; height: 14px; white-space:nowrap;">
	    					<asp:Label id="lblSection1" runat="server" CssClass="LabelNormal">Section 1</asp:Label>
                        </td>		    							
						<td style="white-space:nowrap;" >										
                            <telerik:RadComboBox id="ddSectionList1" runat="server" Width="180px" AutoPostBack="True" SkinsPath="~/RadControls/ComboBox/Skins" Skin="Default"></telerik:RadComboBox>	    									
                        </td>
                    </tr>
					<tr id="section2">
                        <td style="width: 167px; height: 14px; white-space:nowrap;">
	    					<asp:Label id="lblSection2" runat="server" CssClass="LabelNormal">Section 2</asp:Label>
                        </td>		    							
						<td style="white-space:nowrap;" >										
                            <telerik:RadComboBox id="ddSectionList2" runat="server" Width="180px" AutoPostBack="True" SkinsPath="~/RadControls/ComboBox/Skins" Skin="Default"></telerik:RadComboBox>	    									
                        </td>
                    </tr>
					<tr id="section3">
                        <td style="width: 167px; height: 14px; white-space:nowrap;">
	    					<asp:Label id="lblSection3" runat="server" CssClass="LabelNormal">Section 3</asp:Label>
                        </td>		    							
						<td style="white-space:nowrap;" >										
                            <telerik:RadComboBox id="ddSectionList3" runat="server" Width="180px" AutoPostBack="True" SkinsPath="~/RadControls/ComboBox/Skins" Skin="Default"></telerik:RadComboBox>	    									
                        </td>
                    </tr>
                    <tr id="filename">
                        <td style="width: 167px; height: 14px; white-space:nowrap;">
	    					<asp:Label id="Label1" runat="server" CssClass="LabelNormal">Filename prefix</asp:Label>
                        </td>		    							
						<td style="white-space:nowrap;" >
                            <telerik:RadTextBox ID="txtFilename" runat="server" Width="300px"></telerik:RadTextBox>										
                        </td>
                    </tr>

					<tr id="spacer1">
						<td colspan="2">
							<img height="10" src="../images/spacer.gif" width="1" alt="" />		
                        </td>						    
					</tr>
                    
					<tr id="save">
						<td style="width: 167px;  white-space: nowrap;"><img height="8" src="../images/spacer.gif" width="1" alt="" /></td>
						<td style="white-space: nowrap; vertical-align: top;" align="left">
							<table id="savecancelbuttonstable" style="width: 728px; height: 83px"  border="0" cellspacing="1" cellpadding="1" width="728">
								
								<tr>
									<td style="width: 127px" height="40" align="left">
										<telerik:RadButton id="btnSavePlan" runat="server" ToolTip="Add intermediate page plan to system" Text="Save plan" Skin="Office2010Blue"></telerik:RadButton>
                                    </td>
									<td style="WIDTH: 46px" height="40" align="left">
										<telerik:RadButton id="btnCancel" runat="server" Text="Cancel" Skin="Office2010Blue"></telerik:RadButton>
                                    </td>
									<td height="40" align="center"></td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</asp:panel>
            <asp:Panel id="Messages" runat="server" HorizontalAlign="Left" Wrap="False" Width="900">
                <asp:label id="lblError" runat="server" Font-Names="Verdana" Font-Size="10pt" ForeColor="Red" Font-Bold="True"></asp:label>
                <asp:label id="lblInfo" runat="server" Font-Names="Verdana" Font-Size="10pt" ForeColor="Blue" Font-Bold="True"></asp:label>
             </asp:Panel>
            <telerik:RadWindowManager id="RadWindowManager1" runat="server" Skin="Vista">
				<Windows>
                    <telerik:RadWindow runat="server" Height="610px" Width="460"  ID="radWindowSpecialEditionPages"
						Skin="Vista" DestroyOnClose="True" Left="" NavigateUrl="AddEditionInfo.aspx" Behaviors="Close"
						VisibleStatusbar="False" Modal="True" Top="" Title="Special edition pages" MaxHeight="1200" MaxWidth="1600">
                    </telerik:RadWindow>
				</Windows>
			</telerik:RadWindowManager>                 
            <script type="text/javascript">

                if (<%=existingProductionPrompt %> > 0) { 
                    //ret = confirm("Warning - plan is already in system - continue?");
                    ret = confirm("Advarsel - plan er allerede oprettet - forsæt?");
                    if(ret)
                        document.Form1.saveConfirm.value = '1';
                    else
                        document.Form1.saveConfirm.value = '0';
                    document.Form1.submit();
                } 
                if (<%=existingProductionPrompt2 %> > 0) { 
                    //ret = confirm("Warning - plan is already in system - continue?");
                    ret = confirm("Advarsel - plan er allerede oprettet - forsæt?");
                    if(ret)
                        document.Form1.saveConfirm2.value = '1';
                    else
                        document.Form1.saveConfirm2.value = '0';
                    document.Form1.submit();
                }
                
            </script>
        </form>
	</body>
</html>
