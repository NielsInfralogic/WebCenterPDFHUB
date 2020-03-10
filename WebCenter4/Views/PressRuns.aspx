<%@ Page language="c#" Codebehind="PressRuns.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.PressRuns" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>PressRuns</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
		<script language="JavaScript" type="text/javascript">
  
            var appletHeight;
            var appletWidth;
    
            function GetBrowserDim() {

                var scrollWidth = getScrollerWidth();

                if (navigator.userAgent.toLowerCase().indexOf("mac_") > 0) {
                    appletWidth = document.body.clientWidth - scrollWidth;
                    appletHeight = document.body.clientHeight - 40;
                } else if (navigator.userAgent.toLowerCase().indexOf("msie") > 0) {
                    if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) {
                        // IE 6+
                        appletWidth = document.documentElement.clientWidth - scrollWidth;
                        appletHeight = document.documentElement.clientHeight - 30;
                    } else {
                        appletHeight = document.body.clientHeight - 30;
                        appletWidth = document.body.clientWidth - scrollWidth;
                    }
                } else if ((navigator.userAgent.toLowerCase().indexOf("macintosh") > 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla") > 0)) {
                    appletWidth = window.innerWidth - scrollWidth;
                    appletHeight = window.innerHeight - 30;
                } else if ((navigator.userAgent.toLowerCase().indexOf("chrome") > 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla") > 0)) {
                    appletWidth = window.innerWidth - scrollWidth;
                    appletHeight = window.innerHeight - 30;
                } else if ((navigator.userAgent.toLowerCase().indexOf("safari") > 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla") > 0)) {
                    appletWidth = window.innerWidth - scrollWidth;
                    appletHeight = window.innerHeight - 20;
                } else if (navigator.userAgent.toLowerCase().indexOf("netscape6") > 0) {
                    appletWidth = window.innerWidth - scrollWidth;
                    appletHeight = window.innerHeight - 35;
                } else if ((window.opera) || (document.all && (!(document.compatMode && document.compatMode == "CSS1Compat")))) {
                    appletHeight = document.body.clientHeight - 30;
                    appletWidth = document.body.clientWidth - scrollWidth;
                } else {
                    //Netscape percents do not work on applets inside tables so we need to work out the size.
                    appletHeight = window.innerHeight - 35;
                    appletWidth = window.innerWidth - scrollWidth;
                }

                var today = new Date();
                var expire = new Date();
                expire.setTime(today.getTime() + 1000*60*60*24*365);
                document.cookie = "ScreenHeight=" + escape(appletHeight) + ((expire == null) ? "" : ("; expires=" + expire.toGMTString()));
                document.cookie = "ScreenWidth=" + escape(appletWidth) + ((expire == null) ? "" : ("; expires=" + expire.toGMTString()));              
            }


            function getScrollerWidth() {
                var scr = null;
                var inn = null;
                var wNoScroll = 0;
                var wScroll = 0;

                // Outer scrolling div
                scr = document.createElement('div');
                scr.style.position = 'absolute';
                scr.style.top = '-9999px';
                scr.style.left = '-9999px';
                scr.style.width = '100px';
                scr.style.height = '50px';
                // Start with no scrollbar
                scr.style.overflow = 'hidden';

                // Inner content div
                inn = document.createElement('div');
                inn.style.width = '100%';
                inn.style.height = '200px';

                // Put the inner div in the scrolling div
                scr.appendChild(inn);
                // Append the scrolling div to the doc
                document.body.appendChild(scr);

                // Width of the inner div sans scrollbar
                wNoScroll = inn.offsetWidth;
                // Add the scrollbar
                scr.style.overflow = 'auto';
                // Width of the inner div width scrollbar
                wScroll = inn.offsetWidth;

                // Remove the scrolling div from the doc
                document.body.removeChild(document.body.lastChild);

                // Pixel width of the scroller

                if (wNoScroll - wScroll > 0)
                    return (wNoScroll - wScroll);
                else if ((navigator.userAgent.toLowerCase().indexOf("macintosh") > 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla") > 0))
                    return 15;
                else
                    return 5;
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
                overflow: inherit;   
            }  
        </style> 
	</head>

	<body onresize="GetBrowserDim()">
		
		<form id="Form1" method="post" runat="server">

            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>

            <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;" OnButtonClick="RadToolBar1_ButtonClick" CssClass="smallToolBar">
                <Items>
                    <telerik:RadToolBarButton runat="server" Value="Refresh" Text="Refresh" ImageUrl="../Images/refresh16.gif" ImagePosition="Left" ToolTip="Refresh thumbnail view"  PostBack="true">
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Value="Item1">
                        <ItemTemplate>
                            <table cellpadding="0" cellspacing="0"><tr><td>
                                <asp:Label ID="txtPubDate" runat="server" Text="Publication Date" CssClass="RadToolbarLabel"></asp:Label>
                                </td>
                                <td>
                                <div style="padding-top:2px;padding-left:5px;">   
                                    <asp:DropDownList ID="PubDateFilter" runat="server" EnableViewState="true" AutoPostBack="true" OnSelectedIndexChanged="OnSelChangePubDate"></asp:DropDownList>                                           
                                </div>
                                </td></tr>
                            </table>
                        </ItemTemplate>     
                    </telerik:RadToolBarButton>

                </Items>
            </telerik:RadToolBar>
            <table id="mainTable" cellspacing="0" cellpadding="0"  border="0" align="left">
                <tr id="mainTR" style="vertical-align: top;" align="left" >
                    <td id="mainTD" style="background-color: #f0f8ff; vertical-align: top;" align="left">

            			<asp:datagrid id="DataGrid1" runat="server" Font-Size="10pt" Font-Names="Verdana" BackColor="White"
				            BorderStyle="None" BorderColor="#E7E7FF" BorderWidth="1px" HorizontalAlign="Left" AutoGenerateColumns="False"
				            AllowPaging="True" PageSize="20" GridLines="Horizontal" CellPadding="3">
				            <FooterStyle ForeColor="#4A3C8C" BackColor="#B5C7DE"></FooterStyle>
				            <SelectedItemStyle Font-Bold="True" Wrap="False" ForeColor="#F7F7F7" BackColor="#738A9C"></SelectedItemStyle>
				            <EditItemStyle Wrap="False"></EditItemStyle>
				            <AlternatingItemStyle Wrap="False" BackColor="#F7F7F7"></AlternatingItemStyle>
				            <ItemStyle Wrap="False" ForeColor="#4A3C8C" BackColor="#E7E7FF"></ItemStyle>
				            <HeaderStyle Font-Bold="False" ForeColor="#F7F7F7" BackColor="#4A3C8C"></HeaderStyle>
				            <Columns>
					            <asp:TemplateColumn>
						            <ItemStyle Wrap="False"></ItemStyle>
						            <ItemTemplate>
							            <table id="Table2" cellspacing="0" cellpadding="0" width="100%" border="0">
								            <tr>
									            <td style="white-space:nowrap;" align="center" width="18">
										            <asp:ImageButton id="ImageButtonGo" runat="server" ImageUrl="../Images/Go.gif" CommandName="Go" ToolTip="Click to release this press run"></asp:ImageButton></td>
									            <td style="white-space:nowrap;" align="center" width="18">
										            <asp:ImageButton id="ImageButtonHold" runat="server" ImageUrl="../Images/Hold.gif" CommandName="Hold"
											            ToolTip="Click to hold this press run"></asp:ImageButton></td>
									            <td style="white-space:nowrap;" align="center" width="18">
										            <asp:ImageButton id="ImageButtonPriority" runat="server" ImageUrl="../Images/Priority.gif" CommandName="Priority"
											            ToolTip="Click to change priority"></asp:ImageButton></td>
                                                <td style="white-space:nowrap;" align="center" width="18">
										            <asp:ImageButton id="ImageButtonReprocess" runat="server" ImageUrl="../Images/cog.gif" CommandName="Reprocess"
											            ToolTip="Click to re-process all pages"></asp:ImageButton></td>
								            </tr>
							            </table>
						            </ItemTemplate>
					            </asp:TemplateColumn>
					            <asp:TemplateColumn HeaderText="State">
						            <ItemStyle Wrap="False"></ItemStyle>
						            <ItemTemplate>
							            <asp:Panel id="panelState" runat="server" HorizontalAlign="Center" Wrap="False">
								            <asp:Label id="labelState" runat="server" BackColor="Transparent" Text='<%# DataBinder.Eval(Container, "DataItem.State") %>'>
								            </asp:Label>
							            </asp:Panel>
						            </ItemTemplate>
					            </asp:TemplateColumn>
					            <asp:BoundColumn DataField="Press" HeaderText="Press">
						            <ItemStyle Wrap="False"></ItemStyle>
					            </asp:BoundColumn>
					            <asp:BoundColumn DataField="PubDate" HeaderText="PubDate">
						            <ItemStyle Wrap="False"></ItemStyle>
					            </asp:BoundColumn>
					            <asp:BoundColumn DataField="Publication" HeaderText="Publication">
						            <ItemStyle Wrap="False"></ItemStyle>
					            </asp:BoundColumn>
					            <asp:BoundColumn DataField="Edition" HeaderText="Edition">
						            <ItemStyle Wrap="False"></ItemStyle>
					            </asp:BoundColumn>
					            <asp:BoundColumn DataField="Section" HeaderText="Section">
						            <ItemStyle Wrap="False"></ItemStyle>
					            </asp:BoundColumn>
					            <asp:BoundColumn DataField="Copies" HeaderText="Cpy"></asp:BoundColumn>

					            <asp:TemplateColumn HeaderText="FTP">
						            <ItemStyle Wrap="False"></ItemStyle>
						            <ItemTemplate>
							            <asp:Panel id="PanelFTPProgress" runat="server" HorizontalAlign="Center" Width="100px" ToolTip="Number of pages received FTP"
								            Wrap="False" >
								            <asp:label id="labelFTPProgress" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.InputFTP") %>'>
								            </asp:label>
							            </asp:Panel>
						            </ItemTemplate>
					            </asp:TemplateColumn>

					            <asp:TemplateColumn HeaderText="Preflight">
						            <ItemStyle Wrap="False"></ItemStyle>
						            <ItemTemplate>
							            <asp:Panel id="PanelPREProgress" runat="server" HorizontalAlign="Center" Width="100px" ToolTip="Number of pages preflighted"
								            Wrap="False" >
								            <asp:label id="labelPREProgress" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.InputPRE") %>'>
								            </asp:label>
							            </asp:Panel>
						            </ItemTemplate>
					            </asp:TemplateColumn>

					            <asp:TemplateColumn HeaderText="Inksave">
						            <ItemStyle Wrap="False"></ItemStyle>
						            <ItemTemplate>
							            <asp:Panel id="PanelINKProgress" runat="server" HorizontalAlign="Center" Width="100px" ToolTip="Number of pages inksaved"
								            Wrap="False" >
								            <asp:label id="labelINKProgress" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.InputINK") %>'>
								            </asp:label>
							            </asp:Panel>
						            </ItemTemplate>
					            </asp:TemplateColumn>

					            <asp:TemplateColumn HeaderText="Ripping">
						            <ItemStyle Wrap="False"></ItemStyle>
						            <ItemTemplate>
							            <asp:Panel id="PanelRIPProgress" runat="server" HorizontalAlign="Center" Width="100px" ToolTip="Number of pages ripped"
								            Wrap="False" >
								            <asp:label id="labelRIPProgress" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.InputRIP") %>'>
								            </asp:label>
							            </asp:Panel>
						            </ItemTemplate>
					            </asp:TemplateColumn>

					            <asp:TemplateColumn HeaderText="Arrived">
						            <ItemStyle Wrap="False"></ItemStyle>
						            <ItemTemplate>
							            <asp:Panel id="PanelInputProgress" runat="server" HorizontalAlign="Center" Width="100px" ToolTip="Number of pages input"
								            Wrap="False" >
								            <asp:label id="labelInputProgress" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Input") %>'>
								            </asp:label>
							            </asp:Panel>
						            </ItemTemplate>
					            </asp:TemplateColumn>
					            <asp:TemplateColumn HeaderText="Approved">
						            <ItemStyle Wrap="False"></ItemStyle>
						            <ItemTemplate>
							            <asp:Panel id="PanelApproveProgress" runat="server" HorizontalAlign="Center" Width="100px"
								            ToolTip="Number of pages approved" Wrap="False">
								            <asp:label id="labelApproveProgress" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Approved") %>'>
								            </asp:label>
							            </asp:Panel>
						            </ItemTemplate>
					            </asp:TemplateColumn>
					            <asp:TemplateColumn HeaderText="Output">
						            <ItemStyle Wrap="False"></ItemStyle>
						            <ItemTemplate>
							            <asp:Panel id="PanelOutputProgress" runat="server" Width="100px" HorizontalAlign="Center" ToolTip="Number of pages imaged"
								             Wrap="False">
								            <asp:label id="labelOutputProgress" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Output") %>'>
								            </asp:label>
							            </asp:Panel>
						            </ItemTemplate>
					            </asp:TemplateColumn>
					            <asp:BoundColumn DataField="Priority" HeaderText="Prio">
						            <ItemStyle Wrap="False"></ItemStyle>
					            </asp:BoundColumn>
					            <asp:BoundColumn DataField="Devices" HeaderText="Devices">
						            <ItemStyle Wrap="False"></ItemStyle>
					            </asp:BoundColumn>
					            <asp:BoundColumn DataField="Preflight" HeaderText="Preflight">
						            <ItemStyle Wrap="False"></ItemStyle>
					            </asp:BoundColumn>
					            <asp:BoundColumn DataField="Inksave" HeaderText="Inksave">
						            <ItemStyle Wrap="False"></ItemStyle>
					            </asp:BoundColumn>
					            <asp:BoundColumn DataField="Ripping" HeaderText="Ripping">
						            <ItemStyle Wrap="False"></ItemStyle>
					            </asp:BoundColumn>


				            </Columns>
				            <PagerStyle HorizontalAlign="Right" ForeColor="#4A3C8C" BackColor="#E7E7FF" Mode="NumericPages"></PagerStyle>
			            </asp:datagrid>
                    </td>
                </tr>
                <tr style="height: 350px">
				    <td style="height: 350px">
					    <asp:label id="lblError" runat="server" ForeColor="Red"></asp:label>
					</td>
			    </tr>
            </table>
                        
            <input runat="server" id="HiddenReturendFromPopup" type="hidden" value="0" />
            <telerik:RadWindowManager id="RadWindowManager1" runat="server" Skin="Vista">
				<Windows>
					<telerik:RadWindow Height="160px" Width="280"  ID="radWindowPriority" IconUrl="../Images/Priority.gif"
						Skin="Office2010Blue" DestroyOnClose="True" Left="" NavigateUrl="ChangePriority.aspx" Behaviors="Close"
						VisibleStatusbar="False" Modal="True" Top="" Title="Change priority">
                    </telerik:RadWindow>
                    <telerik:RadWindow Height="230px" Width="380"  ID="radWindowReprocess" IconUrl="../Images/cog.gif"
						Skin="Office2010Blue" DestroyOnClose="True" Left="" NavigateUrl="ReprocessPressRun.aspx" Behaviors="Close" 
						VisibleStatusbar="False" Modal="True" Top="" Title="Reprocess pages">
                    </telerik:RadWindow>
				</Windows>
			</telerik:RadWindowManager>
            <telerik:RadCodeBlock ID="RadCodeBlock3" runat="server">
			    <script language="JavaScript" type="text/javascript">

			        GetBrowserDim();

                    /*
			        var n = DOMCall('mainTable');
			        if (n && appletHeight > 0 && appletWidth > 0) {
			            n.style.height = appletHeight;
			            n.style.width = appletWidth;
			        }
			        n = DOMCall('mainTR');
			        if (n && appletHeight > 0 && appletWidth > 0) {
			            n.style.height = appletHeight - 30;
			            n.style.width = appletWidth ;
			        }
			        n = DOMCall('mainTD');
			        if (n && appletHeight > 0 && appletWidth > 0) {
			            n.style.height = appletHeight - 30;
			            n.style.width = appletWidth;
			        }*/
			    </script>
            </telerik:RadCodeBlock>  
        </form>
    </body>
</html>
