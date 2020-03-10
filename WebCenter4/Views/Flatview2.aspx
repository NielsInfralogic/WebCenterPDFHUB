<%@ Page language="c#" Codebehind="Flatview2.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.Flatview2" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>Flatview2</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
        <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
  		    <script  type="text/javascript">

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
                    document.cookie = "ScreenHeightFlat=" + escape(appletHeight) + ((expire == null) ? "" : ("; expires=" + expire.toGMTString()));
                    document.cookie = "ScreenWidthFlat=" + escape(appletWidth) + ((expire == null) ? "" : ("; expires=" + expire.toGMTString()));
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
	
		            document.cookie = "ScrollFlatY=" + escape(scrollY) + "; path=/";


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
            <asp:ScriptManager ID="ScriptManager1" runat="server"> </asp:ScriptManager>
                <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;" OnButtonClick="RadToolBar1_ButtonClick" CssClass="smallToolBar"  OnClientButtonClicked="OnClientButtonClicked">
                    <Items>
                        <telerik:RadToolBarButton runat="server" Value="Refresh" Text="Refresh" ImageUrl="../Images/refresh16.gif" ImagePosition="Left" ToolTip="Refresh plate view"  PostBack="true">
                        </telerik:RadToolBarButton>
                    
                        <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                        </telerik:RadToolBarButton>

                         <telerik:RadToolBarSplitButton runat="server" Text="Plates per row" Value="PlatePerRowSelector" EnableRoundedCorners="true" EnableShadows="true" PostBack="true" EnableDefaultButton="True" >
                            <Buttons>
                                <telerik:RadToolBarButton Text="Plates per row 2" Value="PlatePerRow2">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton Text="Plates per row 4" Value="PlatePerRow4">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton Text="Plates per row 6" Value="PlatePerRow6">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton Text="Plates per row 8" Value="PlatePerRow8">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton Text="Plates per row 10" Value="PlatePerRow10" >
                                </telerik:RadToolBarButton>
                            </Buttons>
                        </telerik:RadToolBarSplitButton>

                          <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                        </telerik:RadToolBarButton>

                        <telerik:RadToolBarButton runat="server" Value="ReleaseAll" Text="Release all" ImageUrl="../Images/approve16.gif" ImagePosition="Left" PostBack="true" ToolTip="Release all plates">
                        </telerik:RadToolBarButton>
                   
                        <telerik:RadToolBarButton runat="server" Value="RetransmitAll" Text="Retransmit all" ImageUrl="../Images/reimage16.gif" ImagePosition="Left" PostBack="true" ToolTip="Re-transmit all forms" >
                        </telerik:RadToolBarButton>

                        <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                        </telerik:RadToolBarButton>

                        <telerik:RadToolBarButton runat="server" Value="HideCommon" Text="Hide duplicates" ImageUrl="../Images/spacer16.gif" ImagePosition="Left" ToolTip="Hide duplicated sub-edition pages" CheckOnClick="true" AllowSelfUnCheck="true" PostBack="true" >
                        </telerik:RadToolBarButton>

                        <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                        </telerik:RadToolBarButton>

                        <telerik:RadToolBarButton runat="server" Value="Item3">
                            <ItemTemplate> 
                                <div style="padding-top:2px;padding-left: 5px">
                                    <asp:Label ID="FilterLabel" runat="server" Text="" ForeColor="DarkGreen" Height="22" CssClass="RadToolbarLabel"></asp:Label>
                                </div>
                            </ItemTemplate>
                        </telerik:RadToolBarButton>

                       
                    </Items>
                </telerik:RadToolBar>
                <div id="mainDiv" style="overflow: Auto;border:0; background-color: #f0f8ff;width: 100%;">
				        <asp:datalist id="FlatList" runat="server" BackColor="#F0F4F8" BorderStyle="Groove" BorderColor="#F0F4F8" CellSpacing="2" CellPadding="0" ShowHeader="False" HorizontalAlign="Left" RepeatDirection="Horizontal" RepeatColumns="<%# nImagesPerRow %>" Width="<%# nImageWidth %>" BorderWidth="0px">

					        <SelectedItemStyle BorderWidth="0px" BorderStyle="None" BorderColor="LightBlue" BackColor="LightBlue"></SelectedItemStyle>
					        <FooterStyle BorderStyle="Outset"></FooterStyle>
				            <SeparatorStyle BorderWidth="0px"></SeparatorStyle>
					        
					        <ItemTemplate>
                                <table id="TableFlats" runat="server" style="background: #f0f4f8;border:inset; border-color: #7a96df #b0c4de #6495ed #7a96df;"  cellspacing="0" cellpadding="0" width="<%# nImageWidth %>" align="center">
							        <tr>
								        <td style="background: #f0f4f8; vertical-align: top;text-align:center; height:18px;" align="center">
                                            <asp:Panel id="pnlHeader" runat="server" BackColor="#F0F4F8" Font-Names="Verdana" Font-Size="8pt"
												   Height="18" EnableViewState="False" Font-Bold="True" BackImageUrl="../Images/greengradient.gif">
										        <table id="Table21" cellspacing="0" cellpadding="0" width="<%# nImageWidth %>">  
											        <tr>
												        <td style="vertical-align: top;text-align:left" align="left"  >
													        <%# DataBinder.Eval(Container.DataItem, "ImageDesc") %>
												        </td>
												        <td style="vertical-align: top;text-align:center" align="center">
													        <%# DataBinder.Eval(Container.DataItem, "ImageDescB") %>
												        </td>
												        <td style="vertical-align: top;text-align:right" align="right" >
													        <%# DataBinder.Eval(Container.DataItem, "ImageDescC") %>
												        </td>
											        </tr>
										        </table>
										        <input id="hiddenImageID" type="hidden" value='<%# DataBinder.Eval(Container.DataItem, "ImageInfo") %>' name="hiddenImageID" runat="server" />
									        </asp:Panel>
								        </td>
							        </tr>
							        <tr>
								        <td style="background-color: #f0f4f8; vertical-align: top;text-align:center;" align="center">
									        <a href='Flatview2.aspx?<%# DataBinder.Eval(Container.DataItem, "ImageQueryString") %>'>
										        <img id='<%# DataBinder.Eval(Container.DataItem, "ImageNumber") %>' title="<%# tooltipClickImage %>"  src='<%# DataBinder.Eval(Container.DataItem, "ImageName") %>' width="<%# nImageWidth %>"  align="middle" border="0" alt="" onclick="GetScrollPosition();" />
									        </a>
								        </td>
							        </tr>
							        <tr>
								       <td style="vertical-align: top;text-align:center;" align="center">
									        <table id="Table3" border="0" cellspacing="0" cellpadding="0" width="100%">
										        <tr align="center">
                                                    <td height="16" align="center">
												        <asp:imagebutton id="btnPrinter" runat="server" BackColor="Transparent" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ImageNumber") %>' CommandName="HardProof" ImageUrl="../Images/Printer.gif" ToolTip="Click to order hardproof" CausesValidation="False">
												        </asp:imagebutton></td>
											        <td width="20" align="left">
												        <asp:ImageButton id="imgApprove" runat="server" BackColor="Transparent" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ImageNumber") %>' CommandName="Approve" ImageUrl="../Images/Approve.gif" ToolTip="Approve all pages on form" CausesValidation="False">
												        </asp:ImageButton></td>
											        <td width="20" align="left">
												        <asp:ImageButton id="imgDisapprove" runat="server" BackColor="Transparent" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ImageNumber") %>' CommandName="Disapprove" ImageUrl="../Images/Disapprove.gif" ToolTip="Disapprove all pages on form" CausesValidation="False">
												        </asp:ImageButton></td>
											        <td width="<%# nImageWidth-96 %>" align="center">
												        <asp:Panel id="pnlFooter" runat="server" BackColor="LightSkyBlue" HorizontalAlign="Center" Wrap="False"></asp:Panel>
												        <asp:ImageButton id="imgReimage" runat="server" BackColor="Transparent" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ImageNumber") %>' CommandName="Reimage" ImageUrl="../Images/Reimage.gif" ToolTip="Re-image one or more colors" CausesValidation="False">
												        </asp:ImageButton></td>
											        <td width="20" align="right">
												        <asp:ImageButton id="imgRelease" runat="server" BackColor="Transparent" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ImageNumber") %>' CommandName="Release" ImageUrl="../Images/Go.gif" ToolTip="Release this form" CausesValidation="False">
												        </asp:ImageButton></td>
                                                    <td align="right">
	        											<asp:ImageButton id="imgReleaseBlack" runat="server" BackColor="Transparent" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ImageNumber") %>' CommandName="ReleaseBlack" ImageUrl="../Images/GoBlack.gif" ToolTip="Release black color only" CausesValidation="False">
    		    										</asp:ImageButton></td>
											        <td width="20" align="right">
												        <asp:ImageButton id="imgHold" runat="server" BackColor="Transparent" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ImageNumber") %>' CommandName="Hold" ImageUrl="../Images/hold.gif" ToolTip="Hold this form" CausesValidation="False">
												        </asp:ImageButton></td>											        
										        </tr>
									        </table>
								        </td>
							        </tr>
							        <tr>
								        <td style="vertical-align: top;" align="center">
									        <asp:Panel id="pnlBottom" runat="server" BackColor="White" Font-Names="Verdana" Font-Size="8pt"
										        BorderWidth="0px" Wrap="False" Height="16" EnableViewState="False">
										        <table id="Table23" cellspacing="0" cellpadding="0" width="<%# nImageWidth %>">
											        <tr>
												        <td align="center">
													        <%# DataBinder.Eval(Container.DataItem, "ImageDesc2") %>
												        </td>
											        </tr>
										        </table>
									        </asp:Panel>
								        </td>
							        </tr>
						        </table>
					        </ItemTemplate>	       
				        </asp:datalist>                    
                  </div>
                <div style="text-align: center;">
					<asp:Label id="lblChooseProduct" runat="server" ForeColor="Teal" Font-Size="Larger">Choose a product in the tree</asp:Label>
                    <asp:label id="lblError" runat="server" ForeColor="Red"></asp:label>
                </div>
                    
			    <telerik:RadWindowManager id="RadWindowManager1" runat="server" Skin="Vista">
				    <Windows>
				        <telerik:RadWindow runat="server" Height="230px" Width="280" SkinsPath="~/RadControls/Window/Skins" ID="radWindowReimage"
					        Skin="Vista" DestroyOnClose="True" Left="" NavigateUrl="Reimage.aspx" Behaviors="Close" VisibleStatusbar="False"
					        Modal="True" Top="" Title="Re-image plate(s)"></telerik:RadWindow>
				        <telerik:RadWindow runat="server" Height="200px" Width="280" SkinsPath="~/RadControls/Window/Skins" ID="radWindowReimagePDF"
					        Skin="Vista" DestroyOnClose="True" Left="" NavigateUrl="ReimagePDF.aspx" Behaviors="Close" VisibleStatusbar="False"
					        Modal="True" Top="" Title="Re-image plate(s)"></telerik:RadWindow>
				        <telerik:RadWindow runat="server" Height="200px" Width="380" SkinsPath="~/RadControls/Window/Skins" ID="radWindowHardProof"
					        Skin="Vista" DestroyOnClose="True" Left="" NavigateUrl="HardProof.aspx" Behaviors="Close" VisibleStatusbar="False"
					        Modal="True" Top="" Title="Flat hardproof"></telerik:RadWindow>
				        <telerik:RadWindow runat="server" Height="260px" Width="520px" SkinsPath="~/RadControls/Window/Skins" ID="radWindowReleaseAll"
					        Skin="Vista" DestroyOnClose="True" Left="" NavigateUrl="ReleaseAll.aspx" Behaviors="Close" VisibleStatusbar="False"
					        Modal="True" Top="" Title="Release all"></telerik:RadWindow>
				        <telerik:RadWindow runat="server" Height="200px" Width="500" SkinsPath="~/RadControls/Window/Skins" ID="radWindowRetransmitAll"
					        Skin="Vista" DestroyOnClose="True" Left="" NavigateUrl="RetransmitAll.aspx" Behaviors="Close"
					        VisibleStatusbar="False" Modal="True" Top="" Title="Retransmit all"></telerik:RadWindow>
			        </Windows>						   
			    </telerik:RadWindowManager>

             <input runat="server" id="HiddenX" type="hidden" value="" />
             <input runat="server" id="HiddenY" type="hidden" value="" />
             <input runat="server" id="HiddenScrollPos" type="hidden" value="" />
             <input runat="server" id="HiddenReturendFromPopup" type="hidden" value="0" />
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