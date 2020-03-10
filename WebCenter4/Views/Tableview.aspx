<%@ Page language="c#" Codebehind="Tableview.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.Tableview" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>Tableview</title>

		 <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
		    <script type="text/javascript">

		        var appletHeight = 0;
		        var appletWidth = 0;
		        var scrollY = 0;
		        var CoffeeParse=<%=nRefreshTime%>

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
                        //This is the only way we know how to set WIDTH and HEIGHT for an applet on a Mac
                        appletWidth = window.innerWidth - scrollWidth;
                        appletHeight = window.innerHeight - 30;
                    } else if ((navigator.userAgent.toLowerCase().indexOf("chrome") > 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla") > 0)) {
                        appletWidth = window.innerWidth - scrollWidth;
                        appletHeight = window.innerHeight - 30;
                    } else if ((navigator.userAgent.toLowerCase().indexOf("safari") > 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla") > 0)) {
                        //This is the only way we know how to set WIDTH and HEIGHT for an applet on a Mac
                        appletWidth = window.innerWidth - scrollWidth;
                        appletHeight = window.innerHeight - 30;
                    } else if (navigator.userAgent.toLowerCase().indexOf("netscape6") > 0) {
                        //This is the only way we know how to set WIDTH and HEIGHT for an applet on a Mac
                        appletWidth = window.innerWidth - scrollWidth;
                        appletHeight = window.innerHeight - 35;
                    } else if ((window.opera) || (document.all && (!(document.compatMode && document.compatMode == "CSS1Compat"))))  {
                        appletHeight = document.body.clientHeight - 30;
                        appletWidth = document.body.clientWidth - scrollWidth;
                    } else {
                        //Netscape percents do not work on applets inside tables so we need to work out the size.
                        appletHeight = window.innerHeight - 35;
                        appletWidth = window.innerWidth - scrollWidth;
                    }

                    if (appletHeight < 400)
                        appletHeight = 400;
                    if (appletWidth < 800)
                        appletWidth = 800;

                    var today = new Date();
                    var expire = new Date();
                    expire.setTime(today.getTime() + 1000 * 60 * 60 * 24 * 365);
                    document.cookie = "ScreenHeight=" + escape(appletHeight) + ((expire == null) ? "" : ("; expires=" + expire.toGMTString()));
                    document.cookie = "ScreenWidth=" + escape(appletWidth) + ((expire == null) ? "" : ("; expires=" + expire.toGMTString()));
                }
                
		        function getScrollerWidth() {
		            var scr = null;
		            var inn = null;
		            var wNoScroll = 0;
		            var wScroll = 0;
		            scr = document.createElement('div');
		            scr.style.position = 'absolute';
		            scr.style.top = '-9999px';
		            scr.style.left = '-9999px';
		            scr.style.width = '100px';
		            scr.style.height = '50px';
		            scr.style.overflow = 'hidden';
		            inn = document.createElement('div');
		            inn.style.width = '100%';
		            inn.style.height = '200px';
		            scr.appendChild(inn);
		            document.body.appendChild(scr);
		            wNoScroll = inn.offsetWidth;
		            scr.style.overflow = 'auto';
		            wScroll = inn.offsetWidth;
		            document.body.removeChild(document.body.lastChild);
		            if (wNoScroll - wScroll > 0)
		                return (wNoScroll - wScroll);
		            else if ((navigator.userAgent.toLowerCase().indexOf("macintosh") > 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla") > 0)) 
		                return 15;
		            else 
		                return 5;
		        }

			     function DOMCall(name) 
			     {    
			         if (document.getElementById) //checks getElementById  
			             return document.getElementById(name);
			         else if (document.layers) //checks document.layers  
			             return document.layers[name];  
			         else if (document.all) //checks document.all  
			             return document.all[name];                       
			     } 

			     function SaveWindowSize() {
			         var n = DOMCall('HiddenX');
			         var m = DOMCall('HiddenY');

			         if (n && m && appletHeight > 0 && appletWidth > 0) {
			             n.value = appletWidth;
			             m.value = appletHeight;
			         }
			     }
			     function SetScrollPosition() {
			         if (  <%= nScollPos %> > 0) { 
			            window.scrollTo(0, <%= nScollPos %>);
 			        }
                 }
			
                 function GetScrollPosition() {
                     if(document.documentElement.scrollTop>=0)
                         scrollY=document.documentElement.scrollTop;
                     else if(document.body.scrollTop>=0)
                         scrollY=document.body.scrollTop;
                     else
                         scrollY = window.pageYOffset;
                     var today = new Date();
                     var expire = new Date();
                     expire.setTime(today.getTime() + 1000*60*60*24*365);
                     document.cookie = "ScrollPosY=" + escape(scrollY) + ((expire == null) ? "" : ("; expires=" + expire.toGMTString()));
                 }

                 function SaveScrollPos()
                 {
                     var n = DOMCall('HiddenScrollPos');
                     if (n)
                         n.value = scrollY;
                 }

                 function refreshtime()
                 {
                     if (!document.images)
                         return; 

                     if (CoffeeParse==1)
                         window.location.href = window.location.href;
                         // window.location.reload();
                     else { 
                         CoffeeParse-=1;
                         currentminutes=Math.floor(CoffeeParse/60);
                         currentsec=CoffeeParse%60;
                         if (currentminutes>0)
                             currenttime=currentminutes+" minutes and "+currentsec+" seconds until page refresh!";
                         else
                             currenttime=currentsec+" seconds left until page refresh!";
                         window.status=currenttime;
                         setTimeout("refreshtime()",1000);
                     }
                 }

            
                 function window_scroll()
                 {
                     GetScrollPosition();
                     SaveScrollPos();
                 }
                 window.onscroll = window_scroll;
                 window.onload=refreshtime;

		    </script>
        </telerik:RadCodeBlock>
        <link rel="stylesheet" type="text/css" href="../Styles/WebCenter.css" />
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
	<body onscroll="GetScrollPosition()" onresize="GetBrowserDim()">
		<form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <div style="width: 100%; display: block; float: none; height: 32px;">

                    <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%; " OnButtonClick="RadToolBar1_ButtonClick" CssClass="smallToolBar" >
                    <Items>

                        <telerik:RadToolBarButton runat="server" Value="Refresh" Text="Refresh" ImageUrl="../Images/refresh16.gif" ImagePosition="Left" ToolTip="Refresh thumbnail view"  PostBack="true">
                        </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                        </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton runat="server" Value="HideFinished" Text="Hide finished" ImageUrl="../Images/spacer16.gif" ImagePosition="Left" CheckOnClick="true" AllowSelfUnCheck="true" Group="1" ToolTip="Hide pages done"  PostBack="true">
                        </telerik:RadToolBarButton>
                        <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                        </telerik:RadToolBarButton>
                        <telerik:RadToolBarButton runat="server" Value="Item3">
                            <ItemTemplate> 
                                <div style="padding-left: 30px">
                                    <asp:Label ID="FilterLabel" runat="server" Text="" ForeColor="DarkGreen" CssClass="RadToolbarLabel"></asp:Label>
                                </div>
                            </ItemTemplate>
                        </telerik:RadToolBarButton>

                    </Items>
                </telerik:RadToolBar>
                   
		    </div>
            <div id="mainDiv" style="overflow: Auto;background-color: #f0f8ff;width: 100%;">
                <telerik:RadGrid ID="RadGrid1" runat="server" CellSpacing="0" Skin="Vista" OnItemDataBound="RadGrid1_ItemDataBound" AutoGenerateColumns="False"   >
                    <ClientSettings>
                        <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                    </ClientSettings>
                    <MasterTableView>
                    <CommandItemSettings ExportToPdfText="Export to PDF"></CommandItemSettings>

                    <RowIndicatorColumn Visible="False" FilterControlAltText="Filter RowIndicator column">
                    <HeaderStyle Width="20px"></HeaderStyle>
                    </RowIndicatorColumn>

                    <ExpandCollapseColumn Visible="False" FilterControlAltText="Filter ExpandColumn column">
                    <HeaderStyle Width="20px"></HeaderStyle>
                    </ExpandCollapseColumn>

                    <Columns>
                         <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="Ed" FilterControlAltText="" Groupable="False" HeaderText="Ed" ReadOnly="True" Reorderable="False" ShowSortIcon="False" UniqueName="Ed" ShowFilterIcon="False">
                             <HeaderStyle Width="35px" />
                             <ItemStyle HorizontalAlign="Center" Width="28px" Wrap="False" />
                        </telerik:GridBoundColumn>
                         <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="Sec" FilterControlAltText="" Groupable="False" HeaderText="Sec" ReadOnly="True" Reorderable="False" ShowSortIcon="False" UniqueName="Sec">
                            <HeaderStyle Width="35px" />
                               <ItemStyle HorizontalAlign="Center" Width="28px" Wrap="False" />
                         </telerik:GridBoundColumn>
                         <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="Page" FilterControlAltText="" Groupable="False" HeaderText="Page" ReadOnly="True" Reorderable="False" ShowSortIcon="False" UniqueName="Page">
                           <HeaderStyle Width="38px" />
                                <ItemStyle HorizontalAlign="Center"  Width="28px" Wrap="False" />
                         </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="FTP" FilterControlAltText="" Groupable="False" HeaderText="FTP" ReadOnly="True" Reorderable="False" ShowSortIcon="False" UniqueName="FTP">
                            <HeaderStyle Width="35px" />
                             <ItemStyle HorizontalAlign="Center" Width="28px" Wrap="False" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="PRE" FilterControlAltText="" Groupable="False" HeaderText="PRE" ReadOnly="True" Reorderable="False" ShowSortIcon="False" UniqueName="PRE">
                          <HeaderStyle Width="35px" />
                               <ItemStyle HorizontalAlign="Center" Width="28px" Wrap="False" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="INK" FilterControlAltText="" Groupable="False" HeaderText="INK" ReadOnly="True" Reorderable="False" ShowSortIcon="False" UniqueName="INK">
                         <HeaderStyle Width="35px" />
                                <ItemStyle HorizontalAlign="Center" Width="28px" Wrap="False" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="RIP" FilterControlAltText="" Groupable="False" HeaderText="RIP" ReadOnly="True" Reorderable="False" ShowSortIcon="False" UniqueName="RIP">
                          <HeaderStyle Width="35px" />
                               <ItemStyle HorizontalAlign="Center" Width="28px" Wrap="False" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="Rdy" FilterControlAltText="" Groupable="False" HeaderText="Rdy" ReadOnly="True" Reorderable="False" ShowSortIcon="False" UniqueName="Rdy">
                          <HeaderStyle Width="35px" />
                               <ItemStyle HorizontalAlign="Center" Width="28px" Wrap="False" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="Appr" FilterControlAltText="" Groupable="False" HeaderText="Appr" ReadOnly="True" Reorderable="False" ShowSortIcon="False" UniqueName="Appr">
                         <HeaderStyle Width="38px" />
                                <ItemStyle HorizontalAlign="Center" Width="28px" Wrap="False" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="CTP" FilterControlAltText="" Groupable="False" HeaderText="CTP" ReadOnly="True" Reorderable="False" ShowSortIcon="False" UniqueName="CTP">
                         <HeaderStyle Width="35px" />
                                <ItemStyle HorizontalAlign="Center" Width="28px" Wrap="False" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="Bend" FilterControlAltText="" Groupable="False" HeaderText="Bend" ReadOnly="True" Reorderable="False" ShowSortIcon="False" UniqueName="Bend">
                          <HeaderStyle Width="38px" />
                               <ItemStyle HorizontalAlign="Center" Width="28px" Wrap="False"/>
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="Preset" FilterControlAltText="" Groupable="False" HeaderText="Preset" ReadOnly="True" Reorderable="False" ShowSortIcon="False" UniqueName="Preset">
                         <HeaderStyle Width="45px" />
                                <ItemStyle HorizontalAlign="Center" Width="28px" Wrap="False"/>
                        </telerik:GridBoundColumn>
                         <telerik:GridTemplateColumn AllowFiltering="False" FilterControlAltText="" Groupable="False" Reorderable="False" ShowSortIcon="False" UniqueName="XXX" Resizable="False">
                              <HeaderStyle Width="16px" />
                                   <ItemStyle Width="14px"  Wrap="False" />
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="Ed2" FilterControlAltText="" Groupable="False" HeaderText="Ed" ReadOnly="True" Reorderable="False" ShowSortIcon="False" UniqueName="Ed2">
                            <HeaderStyle Width="35px" />
                            <ItemStyle HorizontalAlign="Center" Width="28px" Wrap="False" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="Sec2" FilterControlAltText="" Groupable="False" HeaderText="Sec" ReadOnly="True" Reorderable="False" ShowSortIcon="False" UniqueName="Sec2">            
                            <HeaderStyle Width="35px" />
                            <ItemStyle HorizontalAlign="Center" Width="28px" Wrap="False"/>
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="Page2" FilterControlAltText="" Groupable="False" HeaderText="Page" ReadOnly="True" Reorderable="False" ShowSortIcon="False" UniqueName="Page2">
                            <HeaderStyle Width="38px" />
                            <ItemStyle HorizontalAlign="Center" Width="28px" Wrap="False"/>
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="FTP2" FilterControlAltText="" Groupable="False" HeaderText="FTP" ReadOnly="True" Reorderable="False" ShowSortIcon="False" UniqueName="FTP2">
                            <HeaderStyle Width="35px" />
                            <ItemStyle HorizontalAlign="Center" Width="28px" Wrap="False"/>
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="PRE2" FilterControlAltText="" Groupable="False" HeaderText="PRE" ReadOnly="True" Reorderable="False" ShowSortIcon="False" UniqueName="PRE2">
                            <HeaderStyle Width="35px" />
                            <ItemStyle HorizontalAlign="Center" Width="28px" Wrap="False"/>
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="INK2" FilterControlAltText="" Groupable="False" HeaderText="INK" ReadOnly="True" Reorderable="False" ShowSortIcon="False" UniqueName="INK2">
                           <HeaderStyle Width="35px" />
                            <ItemStyle HorizontalAlign="Center" Width="28px" Wrap="False"/>
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="RIP2" FilterControlAltText="" Groupable="False" HeaderText="RIP" ReadOnly="True" Reorderable="False" ShowSortIcon="False" UniqueName="RIP2">
                           <HeaderStyle Width="35px" />
                            <ItemStyle HorizontalAlign="Center" Width="28px" Wrap="False"/>
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="Rdy2" FilterControlAltText="" Groupable="False" HeaderText="Rdy" ReadOnly="True" Reorderable="False" ShowSortIcon="False" UniqueName="Rdy2">
                            <HeaderStyle Width="35px" />
                            <ItemStyle HorizontalAlign="Center" Width="28px" Wrap="False"/>
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="Appr2" FilterControlAltText="" Groupable="False" HeaderText="Appr" ReadOnly="True" Reorderable="False" ShowSortIcon="False" UniqueName="Appr2">
                            <HeaderStyle Width="38px" />
                            <ItemStyle HorizontalAlign="Center" Width="28px" Wrap="False"/>
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="CTP2" FilterControlAltText="" Groupable="False" HeaderText="CTP" ReadOnly="True" Reorderable="False" ShowSortIcon="False" UniqueName="CTP2">
                            <HeaderStyle Width="35px" />
                            <ItemStyle HorizontalAlign="Center" Width="28px" Wrap="False"/>
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="Bend2" FilterControlAltText="" Groupable="False" HeaderText="Bend" ReadOnly="True" Reorderable="False" ShowSortIcon="False" UniqueName="Bend2">
                            <HeaderStyle Width="38px" />
                            <ItemStyle HorizontalAlign="Center" Width="28px" Wrap="False"/>
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn AllowFiltering="False" AllowSorting="False" DataField="Preset2" FilterControlAltText="" Groupable="False" HeaderText="Preset" ReadOnly="True" Reorderable="False" ShowSortIcon="False" UniqueName="Preset2">
                            <HeaderStyle Width="45px" />
                            <ItemStyle HorizontalAlign="Center" Width="28px" Wrap="False"/>
                        </telerik:GridBoundColumn>

                    </Columns>

                    <EditFormSettings>
                    <EditColumn FilterControlAltText="Filter EditCommandColumn column"></EditColumn>
                    </EditFormSettings>

                    <PagerStyle PageSizeControlType="RadComboBox"></PagerStyle>
                    </MasterTableView>

                    <PagerStyle PageSizeControlType="RadComboBox"></PagerStyle>

                    <FilterMenu EnableImageSprites="False"></FilterMenu>
                </telerik:RadGrid>

            </div>
            <div style="text-align: center;">
                <asp:label id="lblChooseProduct" runat="server" ForeColor="Teal" Font-Size="Larger">Choose a product in the tree</asp:label>
                <asp:label id="lblError" runat="server" ForeColor="Red" Font-Bold="True"></asp:label>
            </div>
            <input runat="server" id="HiddenX" type="hidden" value="" />
            <input runat="server" id="HiddenY" type="hidden" value="" />
            <input runat="server" id="HiddenScrollPos" type="hidden" value="" />
            <telerik:RadCodeBlock ID="RadCodeBlock3" runat="server">
			    <script  type="text/javascript">
			        GetBrowserDim();
			        SaveWindowSize();
			        SetScrollPosition();
			    </script>
            </telerik:RadCodeBlock>
        </form>
	</body>
</html>
