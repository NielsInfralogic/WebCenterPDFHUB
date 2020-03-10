<%@ Page language="c#" Codebehind="ReportView.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.ReportView" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Reports</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
		<script language="JavaScript" type="text/javascript">
             function GetBrowserDim() {
               var appletHeight;
               var appletWidth;

		        if (navigator.userAgent.toLowerCase().indexOf("mac_") > 0) {
			        appletWidth = document.body.clientWidth - 5;
			        appletHeight = document.body.clientHeight - 40;
		        } else if (navigator.userAgent.toLowerCase().indexOf("msie") > 0) {
			        if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) {
	                        // IE 6+
	                        appletWidth = document.documentElement.clientWidth - 5;
	                        appletHeight = document.documentElement.clientHeight - 30;
	                    } else {
	                        appletHeight = document.body.clientHeight - 30;
	                        appletWidth = document.body.clientWidth - 5;
	                    }
		        } else if ((navigator.userAgent.toLowerCase().indexOf("macintosh")> 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla")> 0)) {
			        //This is the only way we know how to set WIDTH and HEIGHT for an applet on a Mac
			        appletWidth = window.innerWidth - 5;
			        appletHeight = window.innerHeight - 20;
		        } else if ((navigator.userAgent.toLowerCase().indexOf("safari")> 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla")> 0)) {
			        //This is the only way we know how to set WIDTH and HEIGHT for an applet on a Mac
			        appletWidth = window.innerWidth - 20;
			        appletHeight = window.innerHeight - 20;
		        } else if (navigator.userAgent.toLowerCase().indexOf("netscape6")> 0) {
			        //This is the only way we know how to set WIDTH and HEIGHT for an applet on a Mac
			        appletWidth = window.innerWidth - 25;
			        appletHeight = window.innerHeight - 35;
		        } else {
			        //Netscape percents do not work on applets inside tables so we need to work out the size.
			        appletHeight = window.innerHeight - 35;
			        appletWidth = window.innerWidth - 25;
		        }

               var today = new Date();
               var expire = new Date();
               expire.setTime(today.getTime() + 1000*60*60*24*365);
               document.cookie = "ScreenHeight=" + escape(appletHeight) + ((expire == null) ? "" : ("; expires=" + expire.toGMTString()));
               document.cookie = "ScreenWidth=" + escape(appletWidth) + ((expire == null) ? "" : ("; expires=" + expire.toGMTString()));

           }
           
           function DOMCall(name) {
               if (document.getElementById) //checks getElementById  
                   return document.getElementById(name);
               else if (document.layers) //checks document.layers  
                   return document.layers[name];
               else if (document.all) //checks document.all  
                   return document.all[name];
           } 

		</script>
         <style type="text/css">  
            html, body, form  
            {  
                height: 100%;  
                margin: 0px;  
                padding: 0px;  
                overflow: hidden;  
            }  
        </style> 
    </head>
	<body onresize="GetBrowserDim()" onload="GetBrowserDim()">
		<script language="JavaScript" type="text/javascript">
			var appletHeight;
			var appletWidth;
			var sizeTag;
			var sizeTag2;
			var topToolbarHeight = 30;
		
			if (navigator.userAgent.toLowerCase().indexOf("mac_") > 0) {
				appletWidth = document.body.clientWidth - 5;
				appletHeight = document.body.clientHeight - 40;
			} else if (navigator.userAgent.toLowerCase().indexOf("msie") > 0) {
				//IE5.5 percents do not work on applets inside tables defined by percents so we need to work out the size.
				appletHeight = document.body.clientHeight - 30;
				appletWidth = document.body.clientWidth - 5;
			} else if ((navigator.userAgent.toLowerCase().indexOf("macintosh")> 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla")> 0)) {
				//This is the only way we know how to set WIDTH and HEIGHT for an applet on a Mac
				appletWidth = window.innerWidth - 5;
				appletHeight = window.innerHeight - 20;
			} else if ((navigator.userAgent.toLowerCase().indexOf("safari")> 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla")> 0)) {
				//This is the only way we know how to set WIDTH and HEIGHT for an applet on a Mac
				appletWidth = window.innerWidth - 20;
				appletHeight = window.innerHeight - 20;
			} else if (navigator.userAgent.toLowerCase().indexOf("netscape6")> 0) {
				//This is the only way we know how to set WIDTH and HEIGHT for an applet on a Mac
				appletWidth = window.innerWidth - 25;
				appletHeight = window.innerHeight - 35;
			} else {
				//Netscape percents do not work on applets inside tables so we need to work out the size.
				appletHeight = window.innerHeight - 35;
				appletWidth = window.innerWidth - 25;
			}	
			sizeTag = 'WIDTH="' + (appletWidth) + '" HEIGHT="' + (appletHeight) + '"';
			sizeTag2 = 'WIDTH="' + (appletWidth) + '" HEIGHT="' + (appletHeight - topToolbarHeight) + '"';
			
		</script>
		<form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;" OnButtonClick="RadToolBar1_ButtonClick" CssClass="smallToolBar" >
                <Items>
                    <telerik:RadToolBarButton runat="server" Value="Refresh" Text="Refresh" ImageUrl="../Images/refresh16.gif" ImagePosition="Left" ToolTip="Refresh thumbnail view"  PostBack="true">
                    </telerik:RadToolBarButton>
                        <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Value="Item3">
                        <ItemTemplate> 
                            <div style="padding-left: 30px">
                                <asp:Label ID="FilterLabel" runat="server" Text="All" ForeColor="DarkGreen" CssClass="RadToolbarLabel"></asp:Label>
                            </div>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>

                </Items>
            </telerik:RadToolBar>
            <table id="mainTable" style="border: 0px;" cellspacing="0" cellpadding="0"  align="left" >
                <tr id="mainTR" style="vertical-align: top;" align="left" >
                    <td id="mainTD" style="background-color: #f0f8ff; vertical-align: top;" align="left">
            			<table style="height: 100%; width: 100%; border: 0px;" id="Table6" cellspacing="1" cellpadding="1">
				            <tr>
					            <td style="height: 13px" align="center">
                                    <asp:label id="lblChooseProduct" runat="server" CssClass="HeaderText">Choose a product in the tree</asp:label>
                                </td>
            					<td style="height: 13px" align="center"></td>
				            </tr>
				            <tr>
					            <td style="height: 314px" align="center">
                                    <img id="chart" title="<%# chartTitle %>" height="100" alt="" src="ChartImage.aspx?width=100&amp;height=100" width="100" align="middle" border="0"  runat="server" />
                                </td>
					            <td style="width: 291px; vertical-align: top;" align="left">
						            <table id="Table1" cellspacing="1" cellpadding="1" width="290" align="left" border="0">
							            <tr>
								            <td style="white-space: nowrap;"><asp:label id="lblNumberOfPages" runat="server" CssClass="Text">Number of pages</asp:label></td>
								            <td style="white-space: nowrap;"><asp:label id="lblNumberOfPagesDate" runat="server" CssClass="Text">na</asp:label></td>
							            </tr>
							            <tr>
								            <td style="height: 12px; white-space: nowrap;" colspan="2"></td>
							            </tr>
							            <tr>
								            <td style="white-space: nowrap;"><asp:label id="lblPagesInput" runat="server" CssClass="Text">Pages input</asp:label></td>
								            <td style="white-space: nowrap;"><asp:label id="lblPagesInputData" runat="server" CssClass="Text">na</asp:label></td>
							            </tr>
							            <tr>
								            <td style="white-space: nowrap;"><asp:label id="lblFirstPageInput" runat="server" CssClass="Text">First page input</asp:label></td>
								            <td style="white-space: nowrap;"><asp:label id="lblFirstPageInputData" runat="server" CssClass="Text">DD-MM hh:mm:ss</asp:label></td>
							            </tr>
							            <tr>
								            <td style="white-space: nowrap;"><asp:label id="lblLastPageInput" runat="server" CssClass="Text">Last page input</asp:label></td>
								            <td style="white-space: nowrap;"><asp:label id="lblLastPageInputData" runat="server" CssClass="Text">na</asp:label></td>
							            </tr>
							            <tr>
								            <td style="height: 12px; white-space: nowrap;" colspan="2"></td>
							            </tr>
							            <tr>
								            <td style="white-space: nowrap;"><asp:label id="lblPagesApproved" runat="server" CssClass="Text">Pages approved</asp:label></td>
								            <td style="white-space: nowrap;"><asp:label id="lblPagesApprovedData" runat="server" CssClass="Text">na</asp:label></td>
							            </tr>
							            <tr>
								            <td style="white-space: nowrap;"><asp:label id="lblFirstPageApproved" runat="server" CssClass="Text">First page approved</asp:label></td>
								            <td style="white-space: nowrap;"><asp:label id="lblFirstPageApprovedData" runat="server" CssClass="Text">na</asp:label></td>
							            </tr>
							            <tr>
								            <td style="white-space: nowrap;"><asp:label id="lblLastPageApproved" runat="server" CssClass="Text">Last page approved</asp:label></td>
								            <td style="white-space: nowrap;"><asp:label id="lblLastPageApprovedData" runat="server" CssClass="Text">na</asp:label></td>
							            </tr>
							            <tr>
								            <td style="height: 12px; white-space: nowrap;" colspan="2" ></td>
							            </tr>
							            <tr>
								            <td style="white-space: nowrap;"><asp:label id="lblPagesOutput" runat="server" CssClass="Text">Pages output</asp:label></td>
								            <td style="white-space: nowrap;"><asp:label id="lblPagesOutputData" runat="server" CssClass="Text">na</asp:label></td>
							            </tr>
							            <tr>
								            <td style="white-space: nowrap;">
                                                <asp:label id="lblFirstPageOutput" runat="server" CssClass="Text">First page output</asp:label>
                                            </td>
								            <td style="white-space: nowrap;">
                                                <asp:label id="lblFirstPageOutputData" runat="server" CssClass="Text">na</asp:label>
                                            </td>
							            </tr>
							            <tr>
								            <td style="white-space: nowrap;">
                                                <asp:label id="lblLastPageOutput" runat="server" CssClass="Text">Last page output</asp:label>
                                            </td>
								            <td style="white-space: nowrap;">
                                                <asp:label id="lblLastPageOutputData" runat="server" CssClass="Text">na</asp:label>
                                            </td>
							            </tr>
							            <tr>
								            <td style="height: 12px; white-space: nowrap;" colspan="2"></td>
							            </tr>
							            <tr>
								            <td style="white-space: nowrap;">
                                                <asp:label id="lblNumberOfPlates" runat="server" CssClass="Text">Number of plates</asp:label>
                                            </td>
								            <td style="white-space: nowrap;">
                                                <asp:label id="lblNumberOfPlatesData" runat="server" CssClass="Text">na</asp:label>
                                            </td>
							            </tr>
							            <tr>
								            <td style="white-space: nowrap;">
                                                <asp:label id="lblPlatesOutput" runat="server" CssClass="Text">Plates output</asp:label>
                                            </td>
								            <td style="white-space: nowrap;">
                                                <asp:label id="lblPlatesOutputData" runat="server" CssClass="Text">na</asp:label>
                                            </td>
							            </tr>
							            <tr>
								            <td style="white-space: nowrap;">
                                                <asp:label id="lblPlatesUsed" runat="server" CssClass="Text">Plates actually used</asp:label>
                                            </td>
								             <td style="white-space: nowrap;">
                                                <asp:label id="lblPlatesUsedData" runat="server" CssClass="Text">na</asp:label>
                                            </td>
							            </tr>
                                        <tr>
								            <td style="white-space: nowrap;">
                                                <asp:label id="lblPlatesUsedUpdates" runat="server" CssClass="Text">Plates updated</asp:label>
                                            </td>
								             <td style="white-space: nowrap;">
                                                <asp:label id="lblPlatesUsedUpdatesData" runat="server" CssClass="Text">na</asp:label>
                                            </td>
							            </tr>
                                          <tr>
								            <td style="white-space: nowrap;">
                                                <asp:label id="lblPlatesUsedDamaged" runat="server" CssClass="Text">Plates damaged</asp:label>
                                            </td>
								             <td style="white-space: nowrap;">
                                                <asp:label id="lblPlatesUsedDamagedData" runat="server" CssClass="Text">na</asp:label>
                                            </td>
							            </tr>
							            <tr>
								             <td style="white-space: nowrap;" colspan="2"></td>
							            </tr>
							            <tr>
								             <td style="white-space: nowrap;">
									            <asp:label id="lblDeadline" runat="server" CssClass="Text">Deadline</asp:label>
                                            </td>
								             <td style="white-space: nowrap;">
									            <asp:label id="LblDeadlineData" runat="server" CssClass="Text">na</asp:label>
                                            </td>
							            </tr>
						            </table>
					            </td>
				            </tr>
				            <tr>
					            <td  align="center">
                                    <img id="chart2" title="<%# chartTitle2 %>" height="100" alt="" src="ChartImage2.aspx?width=100&amp;height=100" width="100" align="middle" border="0"  runat="server" />
                                </td>
					            <td style="width: 291px; height: 314px" valign="top" align="left">
						            <table id="Table2" cellspacing="0" cellpadding="0" width="291" border="0">
							            <tr>
								            <td style="height: 12px; white-space: nowrap;"></td>
							            </tr>
							            <tr>
								             <td style="white-space: nowrap;">
                                                <asp:label id="lblPageVersionStat" runat="server" Font-Bold="True" CssClass="Text">Page version statistics</asp:label>
                                            </td>
						            	</tr>
			            				<tr>
            								<td style="height: 12px; white-space: nowrap;"></td>
							            </tr>
    							        <tr>
	        		    					 <td style="white-space: nowrap;">
                                                <asp:datagrid id="DataGrid1" runat="server" BorderStyle="None" BackColor="White" PageSize="50"
			           							    BorderColor="#999999" BorderWidth="1px" CellPadding="2" GridLines="Vertical" CssClass="Text">
										            <FooterStyle ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
										            <SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
										            <AlternatingItemStyle Font-Names="Tahoma" Wrap="False" ForeColor="Black" Width="120px" BackColor="#DCDCDC"></AlternatingItemStyle>
										            <ItemStyle Font-Names="Tahoma" Wrap="False" ForeColor="Black" Width="120px" BackColor="#EEEEEE"></ItemStyle>
										            <HeaderStyle Font-Underline="True" Font-Names="Verdana" Font-Bold="True" Wrap="False" ForeColor="White"
											            Width="120px" BackColor="#000084"></HeaderStyle>
										            <PagerStyle HorizontalAlign="Center" ForeColor="Black" BackColor="#999999" Mode="NumericPages"></PagerStyle>
									            </asp:datagrid>
                                            </td>
							            </tr>
							            <tr>
                                            <td style="height: 12px; white-space: nowrap;" align="center">
                                                <asp:checkbox id="CheckBoxOrderByPlate" runat="server" Visible="False" Text="Sort report sheet-wise"
										            CssClass="Text"></asp:checkbox>
                                            </td>
							            </tr>
					            		<tr>
            								 <td style="white-space: nowrap;" align="center">
                                                <telerik:RadButton id="btnExcelReport" runat="server" Text="Excel report" Width="174px" Skin="Office2010Blue" OnClick="btnExcelReport_Click"></telerik:RadButton>
                                            </td>
							            </tr>
							            
							            <tr>
								            <td style="height: 8px; white-space: nowrap;" align="center"></td>
							            </tr>
							            <tr>
								            <td style="white-space: nowrap;" align="center">
									            <!--	<a href="/CCReports/Statistics.csv" id="lnkCustomReport">Get summarized statistics</a> -->
									            <telerik:RadButton id="btnShowArchive" runat="server" Text="Report archive" Width="174px" Skin="Office2010Blue" OnClick="btnShowArchive_Click"></telerik:RadButton>
                                            </td>
							            </tr>
							            <tr>
								            <td style="height: 8px; white-space: nowrap;" align="center"></td>
							            </tr>
							            <tr>
								            <td style="white-space: nowrap;" align="center">
									            <!--	<a href="/CCReports/Statistics.csv" id="lnkCustomReport">Get summarized statistics</a> -->
									            <telerik:RadButton id="btnShowMonthlyReport" runat="server" Text="Monthly summary" Width="174px" Skin="Office2010Blue" OnClick="btnShowMonthlyReport_Click"></telerik:RadButton>
                                            </td>
							            </tr>
						            </table>
					            </td>
				            </tr>
				            <tr>
					            <td>
                                    <asp:label id="lblError" runat="server" ForeColor="Red" Font-Bold="True"></asp:label>
                                </td>
				            </tr>
			            </table>
			        </td>
                </tr>
            </table>
			<telerik:RadWindowManager id="RadWindowManager1" runat="server" Skin="Vista">
				<Windows>
					<telerik:RadWindow Height="480px" Width="430" SkinsPath="~/RadControls/Window/Skins" ID="radWindowReportArchive"
						Skin="Vista" DestroyOnClose="True" Left="" NavigateUrl="ReportArchive.aspx" Behaviors="Close"
						VisibleStatusbar="False" Modal="True" Top="" Title="Report Archive">
                    </telerik:RadWindow>
				</Windows>
			</telerik:RadWindowManager>
            <telerik:RadCodeBlock ID="RadCodeBlock3" runat="server">
			    <script language="JavaScript" type="text/javascript">

/*			        var n = DOMCall('mainTable');

			        if (n && appletHeight > 0 && appletWidth > 0) {
			            n.style.height = appletHeight;
			            n.style.width = appletWidth;
			        }
			        n = DOMCall('mainTR');
			        if (n && appletHeight > 0 && appletWidth > 0) {
			            n.style.height = appletHeight;
			            n.style.width = appletWidth - 30;
			        }
			        n = DOMCall('mainTD');
			        if (n && appletHeight > 0 && appletWidth > 0) {
			            n.style.height = appletHeight;
			            n.style.width = appletWidth - 30;
			        }*/
			    </script>
             </telerik:RadCodeBlock>  
		</form>
	</body>
</html>
