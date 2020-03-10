<%@ Page language="c#" Codebehind="Readview.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.Readview" enableViewState="True"%>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>Readview</title>
		
        <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
            <script type="text/javascript">

                var frameset = top.document.getElementById("mainframeset");
                if (frameset != null) {
                    origCols = frameset.cols;
                    frameset.cols = "210, *";
                }
            </script>
        </telerik:RadCodeBlock>
        
         <telerik:RadCodeBlock ID="RadCodeBlock2" runat="server">
		    <script type="text/javascript">

		        var appletHeight = 0;
		        var appletWidth = 0;
		        var scrollY = 0;
		        var CoffeeParse=<%=nRefreshTime%>

                 function OnClientButtonClicked(sender, args) {
                     GetScrollPosition();
                 }   

		        function window_onload() {
		            GetBrowserDim();
		            SaveWindowSize();
		            refreshtime();
		            SetScrollPosition();
		        }

		        function window_onsubmit() {
		            GetScrollPosition();
		        }

		        window.onload = window_onload;
		        window.onsubmit = window_onsubmit;
		        window.onresize = GetBrowserDim;                


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
                    } else if ((window.opera) || (document.all && (!(document.compatMode && document.compatMode == "CSS1Compat"))))  {
                        appletHeight = document.body.clientHeight - 30;
                        appletWidth = document.body.clientWidth - scrollWidth;
                    } else {
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
                    document.cookie = "ScreenHeightReadview=" + escape(appletHeight) + ((expire == null) ? "" : ("; expires=" + expire.toGMTString()));
                    document.cookie = "ScreenWidthReadview=" + escape(appletWidth) + ((expire == null) ? "" : ("; expires=" + expire.toGMTString()));
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
		        
			     function DOMCall(name)  {    
			         if (document.getElementById) 
			             return document.getElementById(name);
			         else if (document.layers)   
			             return document.layers[name];  
			         else if (document.all)  
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
			         var maindiv = DOMCall("mainDiv")
			         if (maindiv == null)
			             return;

			         if (  <%= nScollPos %> > 0) {                         
			             //maindiv.scrollTo(0, <%= nScollPos %>);
			             if (typeof maindiv.scrollTop !== 'undefined')
			                 maindiv.scrollTop = <%= nScollPos %>;
			             else if (typeof maindiv.pageYOffset !== 'undefined')
			                 maindiv.pageYOffset = <%= nScollPos %>;

			         } else {
			             var n = DOMCall('HiddenScrollPos');
			             if (n && n.value > 0) {
			                 maindiv.scrollTop = n.value;

			                 if (typeof maindiv.scrollTop !== 'undefined')
			                     maindiv.scrollTop = n.value;
			                 else if (typeof maindiv.pageYOffset !== 'undefined')
			                     maindiv.pageYOffset = n.value;
			             }
			         }        			            
			     }
			
			     function GetScrollPosition() {
			         var maindiv = DOMCall("mainDiv")
			         if (maindiv == null)
			             return;

			         scrollY = (typeof maindiv.scrollTop !== 'undefined') ? maindiv.scrollTop : maindiv.pageYOffset;
			         document.cookie = "ScrollReadviewY=" + escape(scrollY) + "; path=/";


			         var n = DOMCall('HiddenScrollPos');
			         if (n)
			             n.value = scrollY;
			     }
			
               

                 function refreshtime() {
                     if (!document.images)
                         return; 


                     if (CoffeeParse==1) {
                         GetScrollPosition();
                         window.location.href = window.location.href;
                         // window.location.reload();
                     }
                     else { 
                         CoffeeParse-=1;
                         currentminutes=Math.floor(CoffeeParse/60);
                         currentsec=CoffeeParse%60;
                         if (currentminutes>0)
                             currenttime=currentminutes+" minutes and "+currentsec+" seconds until page refresh!";
                         else
                             currenttime=currentsec+" seconds left until page refresh!";
                         // window.status=currenttime;
                         setTimeout("refreshtime()",1000);
                     }
                 }

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
	<body onresize="GetBrowserDim()">

		<form id="Form1" method="post" runat="server">
		
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>

                   <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;" OnButtonClick="RadToolBar1_ButtonClick" CssClass="smallToolBar" OnClientButtonClicked="OnClientButtonClicked">
                    <Items>
                    <telerik:RadToolBarButton runat="server" Value="Refresh" Text="Refresh" ImageUrl="../Images/refresh16.gif" ImagePosition="Left" ToolTip="Refresh thumbnail view"  PostBack="true">
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarSplitButton runat="server" Text="Pages per row" Value="PagesPerRowSelector" EnableRoundedCorners="true" EnableShadows="true" PostBack="true" EnableDefaultButton="True">
                        <Buttons>                            
                            <telerik:RadToolBarButton Text="Pages per row 4" Value="PagesPerRow4" Width="120">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Pages per row 6" Value="PagesPerRow6" Width="120">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Pages per row 8" Value="PagesPerRow8" Width="120">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Pages per row 10" Value="PagesPerRow10" Width="120">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Pages per row 12" Value="PagesPerRow12" Width="120">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Pages per row 14" Value="PagesPerRow14" Width="120">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Pages per row 16" Value="PagesPerRow16" Width="120">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Pages per row 18" Value="PagesPerRow18" Width="120">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Pages per row 20" Value="PagesPerRow20" Width="120">
                            </telerik:RadToolBarButton>
                        </Buttons>
                    </telerik:RadToolBarSplitButton>

                     <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                    </telerik:RadToolBarButton>

                     <telerik:RadToolBarSplitButton runat="server" Text="Pages per row" Value="RefreshtimeSelector" EnableRoundedCorners="true" EnableShadows="true" PostBack="true" EnableDefaultButton="True" >
                        <Buttons>                            
                            <telerik:RadToolBarButton Text="Refresh time 10" Value="RefreshTime10" Width="140">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Refresh time 20" Value="RefreshTime20" Width="140">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Refresh time 30" Value="RefreshTime30" Width="140">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Refresh time 40" Value="RefreshTime40" Width="140">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Refresh time 50" Value="RefreshTime50" Width="140">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Refresh time 60" Value="RefreshTime60" Width="140">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Refresh time 70" Value="RefreshTime70" Width="140">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Refresh time 80" Value="RefreshTime80" Width="140">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Refresh time 90" Value="RefreshTime90" Width="140">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Refresh time 100" Value="RefreshTime100" Width="140">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Refresh time 110" Value="RefreshTime110" Width="140">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Refresh time 120" Value="RefreshTime120" Width="140">
                            </telerik:RadToolBarButton>
                        </Buttons>
                    </telerik:RadToolBarSplitButton>

                    <telerik:RadToolBarButton runat="server" Value="HideApproved" Text="Hide approved pages" ImageUrl="../Images/spacer16.gif" ImagePosition="Left" Tooltip="Hide pages already approved" CheckOnClick="true" AllowSelfUnCheck="true" Group="1"  PostBack="true"  >
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Value="HideCommon" Text="Hide duplicates" ImageUrl="../Images/spacer16.gif" ImagePosition="Left" ToolTip="Hide duplicated sub-edition pages" CheckOnClick="true" AllowSelfUnCheck="true" Group="2" PostBack="true" >
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Value="Download" Text="Download all" ImageUrl="../Images/download16.gif" ImagePosition="Left" ToolTip="Download all previews as PDF document" PostBack="true"  >
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Value="Item3">
                        <ItemTemplate> 
                             <div style="padding-top:2px;padding-left: 5px">
                                <asp:Label ID="FilterLabel" runat="server" Text="All" CssClass="RadToolbarLabel" ForeColor="DarkGreen" Height="22"></asp:Label>
                            </div>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>

                </Items>
            </telerik:RadToolBar>
            <div id="mainDiv" style="overflow: Auto;border:0; background-color: #f0f8ff;width: 100%;">
			            <asp:datalist id="datalistImages" runat="server" BackColor="#F0F4F8" BorderStyle="Groove" BorderColor="#F0F4F8" CellSpacing="2" CellPadding="0" ShowHeader="False" HorizontalAlign="Left" RepeatDirection="Horizontal" RepeatColumns="<%# nImagesPerRow/2 %>" Width="<%# nImageWidth*2 %>" BorderWidth="0px">
    				        <SelectedItemStyle BorderWidth="0px" BorderStyle="None" BorderColor="SteelBlue" BackColor="LightBlue"></SelectedItemStyle>
				            <FooterStyle BorderStyle="Outset"></FooterStyle>
				            <SeparatorStyle BorderWidth="0px"></SeparatorStyle>
				            <ItemTemplate>

					            <table id="TableThumb" runat="server" style="border:inset; border-color: #7a96df #b0c4de #6495ed #7a96df;padding:0px;border-spacing:0px;text-align: center;"  cellspacing="0" cellpadding="0" width="<%# nImageWidth*2 %>" align="center">
						            <tr>
							            <td style="background-color: White; vertical-align: top;text-align:center;" align="center">
								            <asp:Panel id="pnlThumbnail" runat="server" BackColor="#F0F4F8" Font-Names="Verdana" Font-Size="9pt"
									            Height="16" EnableViewState="False" Font-Bold="True" >
									            <table id="Table2x" cellspacing="0" cellpadding="0" width="<%# nImageWidth*2 %>">
										            <tr>
											            <td align="left">
												            <%# DataBinder.Eval(Container.DataItem, "ImageDesc") %>
											            </td>
											            <td align="right">
												            <%# DataBinder.Eval(Container.DataItem, "ImageDescB") %>
											            </td>
										            </tr>
									            </table>
									            <input id="hiddenImageID" type="hidden" value='<%# DataBinder.Eval(Container.DataItem, "ImageNumbers") %>' name="hiddenImageID" runat="server" />
								            </asp:Panel>
                                        </td>
						            </tr>
						            <tr>
                                       <td style="background-color: #f0f4f8; vertical-align: top;text-align:center;" align="center">
								            <table id="Table2y" cellspacing="0" cellpadding="0">
									            <tr>
										            <td><a href='Readview.aspx?<%# DataBinder.Eval(Container.DataItem, "ImageQueryString") %>'>
												            <img id='<%# DataBinder.Eval(Container.DataItem, "ImageNumber") %>' title="<%# tooltipClickImage %>" height='<%# nImageHeight %>' src='<%# DataBinder.Eval(Container.DataItem, "ImageName") %>' width='<%# DataBinder.Eval(Container.DataItem, "ImageWidth") %>' align="right" border="0" onclick="GetScrollPosition();"  />
											            </a>
										            </td>
										            <td><a href='Readview.aspx?<%# DataBinder.Eval(Container.DataItem, "ImageQueryString") %>'>
												            <img id='<%# DataBinder.Eval(Container.DataItem, "ImageNumber2") %>' title="<%# tooltipClickImage %>" height='<%# nImageHeight %>' src='<%# DataBinder.Eval(Container.DataItem, "ImageName2") %>' width='<%# DataBinder.Eval(Container.DataItem, "ImageWidth2") %>' align="left" border="0" onclick="GetScrollPosition();" />
											            </a>
										            </td>
									            </tr>
								            </table>
							            </td>
						            </tr>
						            <tr>
							            <td style="vertical-align: top;text-align:center;" align="center">
                                            <asp:Panel id="pnlFooter2" runat="server"  BorderWidth="0" BorderStyle="None">

								            <table id="Table6" cellspacing="0" cellpadding="0" width="100%" border="0">
									            <tr>
										            <td align="left" height="16">
											            <asp:imagebutton id="btnColor" runat="server" BackColor="Transparent" Height="16" ImageUrl="../Images/Colorcmyk2.gif" ImageAlign="Middle" CommandName="Color" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ImageNumbers") %>'>
											            </asp:imagebutton></td>
										            <td align="right" width="20" height="16">
											            <asp:imagebutton id="btnApprove" runat="server" BackColor="Transparent" Height="16" BorderWidth="0px" ImageUrl="../Images/Approve2.gif" ImageAlign="Left" CommandName="Approve" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ImageNumbers") %>' ToolTip="Click to approve pages">
											            </asp:imagebutton></td>
										            <td align="right" width="20" height="16">
											            <asp:imagebutton id="btnDisapprove" runat="server" BackColor="Transparent" Height="16" BorderWidth="0px" ImageUrl="../Images/Disapprove.gif" CommandName="Disapprove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ImageNumbers") %>' ToolTip="Click to disapprove pages">
											            </asp:imagebutton></td>
										            <td align="right" width="20" height="16">
											            <asp:imagebutton id="btnResetApproval" runat="server" BackColor="Transparent" Height="16" BorderWidth="0px" ImageUrl="../Images/Resetapprove.gif" CommandName="Resetapprove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ImageNumbers") %>' ToolTip="Click to reset approve state">
											            </asp:imagebutton></td>
									            </tr>
								            </table>
                                               </asp:Panel>
							            </td>
						            </tr>
						            <tr>
							            <td valign="top" align="center">
								            <asp:Panel id="pnlFooter" runat="server" BackColor="White" Font-Names="Verdana" Font-Size="7pt"
									            Height="16" EnableViewState="False">
									            <%# DataBinder.Eval(Container.DataItem, "ImageDesc2") %>
								            </asp:Panel></td>
						            </tr>
					            </table>
				            </ItemTemplate>
			            </asp:datalist>
                    </div>
                
                     <div style="text-align: center;">
                        <asp:label id="lblError" runat="server" ForeColor="Red"></asp:label>
                        <asp:label id="lblChooseProduct" runat="server" ForeColor="Teal" Font-Size="Larger">Choose a product in the tree</asp:label>			
			        </div>  
			<telerik:RadWindowManager id="RadWindowManager1" runat="server" Skin="Vista">
				<Windows>
					<telerik:RadWindow runat="server" Height="200px" Width="280" SkinsPath="~/RadControls/Window/Skins" ID="radWindowChangeColor"
						Skin="Vista" DestroyOnClose="True" Left="" NavigateUrl="ChangeColor.aspx" Behaviors="Close" VisibleStatusbar="False"
						Modal="True" Top="" Title="Change color for page"></telerik:RadWindow>
				</Windows>
			    </telerik:RadWindowManager>
                <input runat="server" id="HiddenReturendFromPopup" type="hidden" value="0" />
                <input runat="server" id="HiddenX" type="hidden" value="" />
                <input runat="server" id="HiddenY" type="hidden" value="" />
                <input runat="server" id="HiddenScrollPos" type="hidden" value="" />

            <telerik:RadCodeBlock ID="RadCodeBlock3" runat="server">
			   <script type="text/javascript">
			       GetBrowserDim();
			       SaveWindowSize();

			       var m = DOMCall('mainDiv');
			       if (m && appletHeight > 0) {
			           appletHeight = appletHeight - 4;
			           m.style.height = appletHeight + 'px';
			       }
			       SetScrollPosition();
			    </script>
            </telerik:RadCodeBlock>  
		</form>
	</body>
</html>
