<%@ Page language="c#" Codebehind="PlanViewExtendedXMLplateplans.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.PlanViewExtendedXMLplateplans" validateRequest="false" %>

<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Plte plans</title>
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
                                      
            <asp:panel id="PanelAddPlan" runat="server" Width="500" HorizontalAlign="Left" Wrap="False" >                               
				<table id="mainparametertable" style="border: 1px;text-align: left; width:inherit;margin-left:20px;" cellspacing="1" cellpadding="1" >								   
					
                    <tr id="pageformat">
                        <td style="width: 250px; height: 14px; white-space:nowrap;">
							<asp:Label id="lblPageFormat" runat="server" CssClass="LabelNormal" Width="167" >Page format</asp:Label>
                        </td>
                        <td style="width: 80px;height: 14px; white-space:nowrap;" >
							<telerik:RadComboBox id="ddPageFormatList" runat="server" Width="150px" SkinsPath="~/RadControls/ComboBox/Skins" Skin="Default" AutoPostBack="True" OnSelectedIndexChanged="ddPageFormatList_SelectedIndexChanged"></telerik:RadComboBox>	    									
                        </td>
                        <td></td>
					</tr>

                    <tr id="cover">
                        <td style="height: 14px; white-space:nowrap;">
							<asp:Label id="lblCover" runat="server" CssClass="LabelNormal">Product with cover</asp:Label>
                        </td>
                        <td style="height: 14px; white-space:nowrap;" >
                            <telerik:RadCheckBox ID="RadCheckBoxCover" runat="server" Text=""></telerik:RadCheckBox>
                        </td>
                        <td></td>
					</tr>

                    <tr id="publication">
                        <td style="height: 14px; white-space:nowrap;">
	    					<asp:Label id="lblPublication" runat="server" CssClass="LabelNormal">Publication</asp:Label>
                        </td>		    							
						<td style="white-space:nowrap;" >										
                            <telerik:RadComboBox id="ddPublicationList" runat="server" Width="150px" AutoPostBack="True" SkinsPath="~/RadControls/ComboBox/Skins" Skin="Default" OnSelectedIndexChanged="ddPublicationList_SelectedIndexChanged"></telerik:RadComboBox>	    									
                        </td>
                        <td></td>
                    </tr>

                    <tr id="pubdateweeknumber">
                        <td style="white-space:nowrap;">
							<asp:Label id="lblPubdate" runat="server" CssClass="LabelNormal">Publication date</asp:Label>
                        </td>
                        <td style="white-space:nowrap;">
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
                            </telerik:RadDatePicker>
                        </td> 
                       <td></td>
					</tr>

					<tr id="edition">
                        <td style="height: 14px; white-space:nowrap;">
	    					<asp:Label id="lblEdition" runat="server" CssClass="LabelNormal">Edition</asp:Label>
                        </td>		    							
						<td style="white-space:nowrap;" >										
                            <telerik:RadComboBox id="ddEditionList" runat="server" Width="100px" AutoPostBack="True" SkinsPath="~/RadControls/ComboBox/Skins" Skin="Default" OnSelectedIndexChanged="ddEditionList_SelectedIndexChanged"></telerik:RadComboBox>	    									
                        </td>
                        <td></td>
                    </tr>

					<tr id="section1">
                        <td style="height: 14px; white-space:nowrap;">
	    					<asp:Label id="lblImpositionSection1" runat="server" CssClass="LabelNormal">Section 1: Number of pages</asp:Label>
                        </td>
						 <td style="height: 14px; white-space:nowrap;" colspan="2">							
                            <telerik:RadComboBox id="ddSectionList1" runat="server" Width="100px" AutoPostBack="False" SkinsPath="~/RadControls/ComboBox/Skins" Skin="Default"></telerik:RadComboBox>&nbsp;&nbsp;	    									
                            <telerik:RadComboBox id="ddImpositionList1" runat="server" Width="300px" SkinsPath="~/RadControls/ComboBox/Skins" Skin="Default"></telerik:RadComboBox>	    									
                      </td>
                    </tr>
					<tr id="section2">
                        <td style="height: 14px; white-space:nowrap;">
	    					<asp:Label id="lblImpositionSection2" runat="server" CssClass="LabelNormal">Section 2: Number of pages</asp:Label>
                        </td>		    
                         <td style="height: 14px; white-space:nowrap;" colspan="2">								
                            <telerik:RadComboBox id="ddSectionList2" runat="server" Width="100px" AutoPostBack="False" SkinsPath="~/RadControls/ComboBox/Skins" Skin="Default"></telerik:RadComboBox>&nbsp;&nbsp;	    									
							<telerik:RadComboBox id="ddImpositionList2" runat="server" Width="300px" SkinsPath="~/RadControls/ComboBox/Skins" Skin="Default"></telerik:RadComboBox>	    									
                        </td>  							
                  </tr>
					<tr id="section3">
                        <td style="height: 14px; white-space:nowrap;">
	    					<asp:Label id="lblImpositionSection3" runat="server" CssClass="LabelNormal">Section 3: Number of pages</asp:Label>
                        </td>		    							
 						 <td style="height: 14px; white-space:nowrap;" colspan="2">							
                          <telerik:RadComboBox id="ddSectionList3" runat="server" Width="100px" AutoPostBack="False" SkinsPath="~/RadControls/ComboBox/Skins" Skin="Default"></telerik:RadComboBox>&nbsp;&nbsp;	    									
						  <telerik:RadComboBox id="ddImpositionList3" runat="server" Width="300px" SkinsPath="~/RadControls/ComboBox/Skins" Skin="Default"></telerik:RadComboBox>	    									
                       </td>
                    </tr>
                    <tr id="Subeditions">
                        <td style="height: 14px; white-space:nowrap;">
	    					<asp:Label id="Label1" runat="server" CssClass="LabelNormal">Subeditions</asp:Label>
                        </td>		    							
						<td style="white-space:nowrap;" colspan="2" >
                            <asp:CheckBoxList ID="CheckBoxListSubEditions" runat="server">
                            </asp:CheckBoxList>
                        </td>
               
                    </tr>


					<tr id="spacer1">
						<td colspan="3">
							<img height="10" src="../images/spacer.gif" width="1" alt="" />		
                        </td>						    
					</tr>
                    
					<tr id="save">

						<td style="white-space: nowrap; vertical-align: top;" colspan="3">
							<table id="savecancelbuttonstable" style="height: 83px;"  border="0" cellspacing="1" cellpadding="1" >
								
								<tr>
									<td style="width: 175px;height:40px;text-align:center;margin-left:50px;" >
										<telerik:RadButton id="btnSavePlan" runat="server" Width="120" ToolTip="Add intermediate page plan to system" Text="Save plan" Skin="Office2010Blue"></telerik:RadButton>
                                    </td>
									<td style="width: 175px;height:40px;text-align:center;margin-left:50px;">
										<telerik:RadButton id="btnCancel" runat="server" Width="120" Text="Cancel" Skin="Office2010Blue"></telerik:RadButton>
                                    </td>
								
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
                    ret = confirm("Warning - plan is already in system - continue?");
                    //ret = confirm("Advarsel - plan er allerede oprettet - forsæt?");
                    if(ret)
                        document.Form1.saveConfirm.value = '1';
                    else
                        document.Form1.saveConfirm.value = '0';
                    document.Form1.submit();
                } 
                if (<%=existingProductionPrompt2 %> > 0) { 
                    ret = confirm("Warning - plan is already in system - continue?");
                    //ret = confirm("Advarsel - plan er allerede oprettet - forsæt?");
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
