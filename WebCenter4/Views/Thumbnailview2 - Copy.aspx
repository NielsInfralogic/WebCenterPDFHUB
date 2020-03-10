<%@ Page language="c#" Codebehind="Thumbnailview2.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.Thumbnailview2" enableViewState="True" smartNavigation="False"%>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
	<head runat="server">
		<title>Thumbnail view</title>
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
        
		//	        if (navigator.userAgent.toLowerCase().indexOf("ipad") > 0)
			//            appletHeight = appletHeight - 20;

			        if (appletHeight < 400)
			            appletHeight = 400;
			        if (appletWidth < 600)
			            appletWidth = 600;

			        var today = new Date();
			        var expire = new Date();
			        expire.setTime(today.getTime() + 1000 * 60 * 60 * 24 * 365);
			        document.cookie = "ScreenHeightPages=" + escape(appletHeight) + ((expire == null) ? "" : ("; expires=" + expire.toGMTString()));
			        document.cookie = "ScreenWidthPages=" + escape(appletWidth) + ((expire == null) ? "" : ("; expires=" + expire.toGMTString()));			       
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
	


	                var olddate = new Date();
	                olddate.setDate(olddate.getDate() - 1);
	                document.cookie = "ScrollY=0; expires=" + olddate.toGMTString() + ";path=/";
				    document.cookie = "ScrollY=" + escape(scrollY) + "; path=/";

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
        html, body, form {  
            height: 100%;  
            margin: 0px;  
            padding: 0px;  
            overflow:auto;
        }  
        
        </style> 
	</head>
     	<body onresize="GetBrowserDim()">

		<form id="Form1" method="post" runat="server" >
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

                <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;" OnButtonClick="RadToolBar1_ButtonClick" CausesValidation="False" CssClass="smallToolBar" OnClientButtonClicked="OnClientButtonClicked" >
                <Items>
                    <telerik:RadToolBarButton runat="server" Value="Refresh" Text="Refresh" ImageUrl="../Images/refresh16.gif" ImagePosition="Left" ToolTip="Refresh thumbnail view"  PostBack="true">
                    </telerik:RadToolBarButton>
                     <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Value="ApproveAll" Text="Approve all" ImageUrl="../Images/approve16.gif" ImagePosition="Left" ToolTip="Approve all pages"  PostBack="true" >
                    </telerik:RadToolBarButton>
                   
                    <telerik:RadToolBarButton runat="server" Value="CustomAction" Text="" ToolTip=""  ImageUrl="../Images/custom16.gif" ImagePosition="Left" PostBack="true" >
                    </telerik:RadToolBarButton>

                    <telerik:RadToolBarSplitButton runat="server" Text="Pages per row" Value="PagesPerRowSelector" EnableRoundedCorners="true" EnableShadows="true" PostBack="true" EnableDefaultButton="True">
                        <Buttons>                            
                            <telerik:RadToolBarButton Text="Pages per row 4" Value="PagesPerRow4" Width="130">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Pages per row 6" Value="PagesPerRow6" Width="130">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Pages per row 8" Value="PagesPerRow8" Width="130">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Pages per row 10" Value="PagesPerRow10" Width="130">
                            </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton Text="Pages per row 12" Value="PagesPerRow12" Width="130">
                            </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton Text="Pages per row 14" Value="PagesPerRow14" Width="130">
                            </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton Text="Pages per row 16" Value="PagesPerRow16" Width="130">
                            </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton Text="Pages per row 18" Value="PagesPerRow18" Width="130">
                            </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton Text="Pages per row 20" Value="PagesPerRow20" Width="130">
                            </telerik:RadToolBarButton>
                        </Buttons>
                    </telerik:RadToolBarSplitButton>
                     
                     <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                    </telerik:RadToolBarButton>

                     <telerik:RadToolBarSplitButton runat="server" Text="Pages per row" Value="RefreshtimeSelector" PostBack="true" EnableDefaultButton="True" CssClass="RadToolbarSplitButton">
                        <Buttons>                            
                            <telerik:RadToolBarButton Text="Refresh time 10" Value="RefreshTime10" Width="150" CssClass="RadToolbarSplitButton" >
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Refresh time 20" Value="RefreshTime20" Width="150">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Refresh time 30" Value="RefreshTime30" Width="150">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Refresh time 40" Value="RefreshTime40" Width="150">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Refresh time 50" Value="RefreshTime50" Width="150">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Refresh time 60" Value="RefreshTime60" Width="150">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Refresh time 70" Value="RefreshTime70" Width="150">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Refresh time 80" Value="RefreshTime80" Width="150">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Refresh time 90" Value="RefreshTime90" Width="150">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Refresh time 100" Value="RefreshTime100" Width="150">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Refresh time 110" Value="RefreshTime110" Width="150">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton Text="Refresh time 120" Value="RefreshTime120" Width="150">
                            </telerik:RadToolBarButton>
                        </Buttons>
                    </telerik:RadToolBarSplitButton>
                     <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                          </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Value="Messages" Text="Messages" ImageUrl="../Images/mail16.gif" ImagePosition="Left" ToolTip="View message(s)" PostBack="true"   >
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Value="HideApproved" Text="Hide approved pages" ImageUrl="../Images/spacer16.gif" ImagePosition="Left" Tooltip="Hide pages already approved" CheckOnClick="true" AllowSelfUnCheck="true" Group="1" PostBack="true"  >
                    </telerik:RadToolBarButton>
                                        
                      <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                          </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Value="HideCommon" Text="Hide duplicates" ImageUrl="../Images/spacer16.gif" TImagePosition="Left" ToolTip="Hide duplicated sub-edition pages" CheckOnClick="true" AllowSelfUnCheck="true" Group="2" PostBack="true" >
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Value="UploadFiles" Text="Upload files" ImageUrl="../Images/upload16.gif" ImagePosition="Left" ToolTip="Upload files to central server" PostBack="true"  >
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Value="Download" Text="Download all" ImageUrl="../Images/download16.gif" ImagePosition="Left" ToolTip="Download all previews as PDF document" PostBack="true"  >
                    </telerik:RadToolBarButton>

                     <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Value="Item3">
                        <ItemTemplate> 
                            <div style="padding-top:5px;padding-left: 5px">
                                <asp:Label ID="FilterLabel" runat="server" Text="" ForeColor="DarkGreen" Height="20" CssClass="RadToolbarLabel"></asp:Label>
                            </div>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>

                </Items>
            </telerik:RadToolBar>

            <div id="mainDiv" style="overflow: auto;border: 0; background-color: #f0f8ff;width: 100%;">
                        <asp:label id="lblChooseProduct" runat="server" ForeColor="Teal" Font-Size="Larger">Choose a product in the tree</asp:label>			
			            <asp:datalist style="Z-INDEX: 0" id="datalistImages" runat="server" BorderStyle="Groove" BackColor="#F0F4F8" BorderWidth="0px" Width="<%# nImageWidth %>" RepeatColumns="<%# nImagesPerRow %>" RepeatDirection="Horizontal" HorizontalAlign="Left" ShowHeader="False" CellPadding="0" CellSpacing="2" BorderColor="Transparent">				            
                            <SelectedItemStyle BorderWidth="0px" BorderStyle="None" BorderColor="SteelBlue" BackColor="LightBlue"></SelectedItemStyle>
				            <FooterStyle BorderStyle="Outset"></FooterStyle>
				            <SeparatorStyle BorderWidth="0px"></SeparatorStyle>
				            <ItemTemplate>
        					    
								            <table id="Table2" style="border:inset; border-color: #7a96df #b0c4de #6495ed #7a96df;"  cellspacing="0" cellpadding="0" width="<%# nImageWidth %>" align="center">
									            <tr>
										            <td style="background: #f0f4f8; vertical-align: top;text-align:center;" align="center">
											            <asp:Panel id="pnlThumbnail" runat="server" BackColor="#F0F4F8" Font-Names="Verdana" Font-Size="9pt"
												            Height="16" EnableViewState="False" Font-Bold="True" BackImageUrl="../Images/greengradient.gif">
												            <%# DataBinder.Eval(Container.DataItem, "ImageDesc") %>
												            <asp:ImageButton id="ImgbtnAlarm" runat="server" BackColor="Transparent" Height="16" BorderWidth="0px" ImageUrl="../Images/Alarm.gif" CommandName="Deadline" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ImageNumber") %>' ToolTip="Deadline alarm" AlternateText="Deadline alarm"></asp:ImageButton>
                                                            <asp:ImageButton id="ImgbtnLocked" runat="server" BackColor="Transparent" Height="16" BorderWidth="0px" ImageUrl="../Images/lock.gif" ToolTip="Page locked" AlternateText="Page locked"></asp:ImageButton>
												            <input id="hiddenImageID" value='<%# DataBinder.Eval(Container.DataItem, "ImageNumber") %>' type="hidden" name="hiddenImageID" runat="server"/>
												            <input id="hiddenImageID2" value='<%# DataBinder.Eval(Container.DataItem, "CanPrint") %>' type="hidden" name="hiddenImageID2" runat="server"/>
												            <input id="hiddenCanUpload" value='<%# DataBinder.Eval(Container.DataItem, "CanUpload") %>' type="hidden" name="hiddenCanUpload" runat="server"/>
												            <input id="hiddenImageID3" value='<%# DataBinder.Eval(Container.DataItem, "IsColorLocked") %>' type="hidden" name="hiddenImageID3" runat="server"/>
												            <input id="hiddenImageID4" value='<%# DataBinder.Eval(Container.DataItem, "IsApprovalLocked") %>' type="hidden" name="hiddenImageID4" runat="server"/>
												            <input id="hiddenImageID6" value='<%# DataBinder.Eval(Container.DataItem, "ShowHistory") %>' type="hidden" name="hiddenImageID6" runat="server"/>
												            <input id="hiddenFTPStatus" value='<%# DataBinder.Eval(Container.DataItem, "FTPStatus") %>' type="hidden" name="hiddenFTPStatus" runat="server"/>
												            <input id="hiddenInkSaveStatus" value='<%# DataBinder.Eval(Container.DataItem, "InkSaveStatus") %>' type="hidden" name="hiddenInkSaveStatus" runat="server"/>					
                                                            <input id="hiddenPreflightStatus" value='<%# DataBinder.Eval(Container.DataItem, "PreflightStatus") %>' type="hidden" name="hiddenPreflightStatus" runat="server"/>
												            <input id="hiddenRIPStatus" value='<%# DataBinder.Eval(Container.DataItem, "RIPStatus") %>' type="hidden" name="hiddenRIPStatus" runat="server"/>
												            <input id="hiddenColorStatus" value='<%# DataBinder.Eval(Container.DataItem, "ColorStatus") %>' type="hidden" name="hiddenColorStatus" runat="server"/>
												            <input id="hiddenDeadlineStatus" value='<%# DataBinder.Eval(Container.DataItem, "DeadlineStatus") %>' type="hidden" name="hiddenDeadlineStatus" runat="server"/>
                                                            <input id="hiddenUniquePage" value='<%# DataBinder.Eval(Container.DataItem, "UniquePage") %>' type="hidden" name="hiddenUniquePage" runat="server"/>
											                <input id="hiddenLocked"  value='<%# DataBinder.Eval(Container.DataItem, "Locked") %>' type="hidden" name="hiddenLocked" runat="server" />
											            </asp:Panel>
                                                    </td>
									            </tr>
									            <tr>
										            <td style="background-color: #f0f4f8; vertical-align: top;text-align:center;" align="center">
											            <asp:Panel id="pnlPlanPageName" runat="server" BackColor="#F0F4F8" Font-Names="Verdana" Font-Size="7pt"
												            Height="16" EnableViewState="False" Font-Bold="False" BackImageUrl="../Images/greengradient.gif">
												            <%# DataBinder.Eval(Container.DataItem, "ImageDesc3") %>
											            </asp:Panel>
                                                    </td>
									            </tr>
									            <tr>
										            <td style="background-color: #f0f4f8; vertical-align: top;text-align:center;" align="center">
                                                        <a href='Thumbnailview2.aspx?<%# DataBinder.Eval(Container.DataItem, "ImageQueryString") %>'>
                                                            <img id='<%# DataBinder.Eval(Container.DataItem, "ImageNumber") %>' title="<%# DataBinder.Eval(Container.DataItem, "Tooltip") %>" border="0" name='<%# DataBinder.Eval(Container.DataItem, "ImageNumber") %>' align="middle" src='<%# DataBinder.Eval(Container.DataItem, "ImageName") %>' width="<%# nImageWidth %>" height="<%# nImageHeight %>" alt="" onclick="GetScrollPosition();" />
                                                        </a>
                                                    </td>
                                                 </tr>
									             <tr>
										            <td style="background-color: #f0f4f8; vertical-align: top;text-align:center;" align="center">
											            <asp:Panel id="pnlFooter" runat="server" BackColor="#F0F4F8" Font-Names="Verdana" Font-Size="7pt" Height="16" EnableViewState="False" BackImageUrl="../Images/greengradient.gif">
												            <%# DataBinder.Eval(Container.DataItem, "ImageDesc2") %>
											            </asp:Panel>
                                                    </td>
									            </tr>
									            <tr>
										            <td style="vertical-align: top;text-align:center;" align="center">
											            <table id="Table9" border="0" cellspacing="0" cellpadding="1" width="100%">
												            <tr>
													            <td style="white-space:nowrap;" align="center">
														            <asp:ImageButton id="ImageButtonFTPInfo" runat="server" BackColor="Transparent" Height="16px" ImageUrl="../Images/FtpNone.gif" CommandName="FTPinfo" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "FTPmessage") %>' ToolTip="Click to see FTP status message" ></asp:ImageButton>
                                                                </td>
        													    <td style="white-space:nowrap;" align="center">
														            <asp:ImageButton id="ImageButtonPreflightInfo" runat="server" BackColor="Transparent" Height="16px" ImageUrl="../Images/PreNone.gif" CommandName="PreflightInfo" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PreflightMessage") %>' ToolTip="Click to see preflight message"></asp:ImageButton>
                                                                </td>
													            <td style="white-space:nowrap;" align="center">
														            <asp:ImageButton id="ImageButtonInkSaveInfo" runat="server" BackColor="Transparent" Height="16px" ToolTip="Click to see inksave message" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "InkSaveMessage") %>' CommandName="InkSaveInfo" ImageUrl="../Images/InkNone.gif"></asp:ImageButton>
													            </td>

													            <td style="white-space:nowrap;" align="center">
														            <asp:ImageButton id="ImageButtonRipInfo" runat="server" Height="16px" ImageUrl="../Images/RipNone.gif" CommandName="RipInfo" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "RIPMessage") %>' ToolTip="Click to see RIP message"></asp:ImageButton>
                                                                </td>
													            <td style="white-space:nowrap;" align="center">
														            <asp:ImageButton id="ImageButtonColorInfo" runat="server" Height="16px" ImageUrl="../Images/InkNone.gif" CommandName="ColorInfo" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ColorMessage") %>' ToolTip="Click to see Color message"></asp:ImageButton>
                                                                </td>
												            </tr>
											            </table>
                                                        </td>
                                                    </tr>
    									            <tr>
	    									            <td style="vertical-align: top;text-align:center;" align="center">

											            <table id="Table6" border="0" cellspacing="0" cellpadding="0" width="100%">
												            <tr>
													            <td height="16" align="left">
														            <asp:imagebutton id="btnColor" runat="server" BackColor="Transparent" Height="16" ImageUrl="../Images/Colorcmyk2.gif" CommandName="Color" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ImageNumber") %>' ToolTip="Click to change color" ImageAlign="Middle"></asp:imagebutton>
                                                                </td>
													            <td height="16" align="center">
														            <asp:imagebutton id="btnPDF" runat="server" BackColor="Transparent" Height="16" BorderWidth="0px" ImageUrl="../Images/pdfsmall.gif" CommandName="ViewPDF" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ImageNumber") %>' ToolTip="Click to view pdf file" Visible="False"></asp:imagebutton>
                                                                </td>
													            <td height="16" align="center">
														            <asp:imagebutton id="btnRetransmit" runat="server" BackColor="Transparent" Height="16" BorderWidth="0px" ImageUrl="../Images/reimage.gif" CommandName="Retransmit" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ImageNumber") %>' ToolTip="Click to retransmit file" Visible="True"></asp:imagebutton>
                                                                </td>
                                                                <td height="16" align="center">
														            <asp:imagebutton id="btnReproof" runat="server" BackColor="Transparent" Height="16" BorderWidth="0px" ToolTip="Click to reproof file" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ImageNumber") %>' CommandName="Reproof" ImageUrl="../Images/reimage.gif" Visible="False"></asp:imagebutton>
														        </td>
                                                                <td height="16" align="center">
														            <asp:imagebutton id="btnReprocess" runat="server" BackColor="Transparent" Height="16" BorderWidth="0px" ToolTip="Click to re-process page" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ImageNumber") %>' CommandName="Reprocess" ImageUrl="../Images/cog.gif" Visible="False"></asp:imagebutton>
														        </td>
													            <td height="16" align="center">
														            <asp:imagebutton id="btnKill" runat="server" BackColor="Transparent" Height="16" BorderWidth="0px" ToolTip="Click to kill page" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ImageNumber") %>' CommandName="Kill" ImageUrl="../Images/waste.gif" Visible="True"></asp:imagebutton>
														        </td>
													            <td height="16" align="center">
														            <asp:imagebutton id="btnPrinter" runat="server" BackColor="Transparent" Height="16" BorderWidth="0px" ImageUrl="../Images/Printer.gif" CommandName="HardProof" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ImageNumber") %>' ToolTip="Click to order hardproof"></asp:imagebutton>
                                                                </td>
													            <td height="16" align="center">
														            <asp:imagebutton id="btnHistory" runat="server" BackColor="Transparent" Height="16" BorderWidth="0px" ImageUrl="../Images/pagehistory.gif" CommandName="History" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "History") %>' ToolTip="Click to view page history"></asp:imagebutton>
                                                                </td>
                                                                 <td height="16" width="34" align="right">
														            <table id="Table1" border="0" cellspacing="0" cellpadding="0" width="34">
															            <tr>
																            <td style="white-space:nowrap;">
																	            <asp:imagebutton id="btnLock" runat="server" BackColor="Transparent" Height="16" BorderWidth="0px" ImageUrl="../Images/lock.gif" CommandName="Lock" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ImageNumber") %>' ToolTip="Click to lock page" ImageAlign="Left"></asp:imagebutton>
                                                                            </td>
																            <td style="white-space:nowrap;">
																	            <asp:imagebutton id="btnUnlock" runat="server" BackColor="Transparent" Height="16" BorderWidth="0px" ImageUrl="../Images/unlock.gif" CommandName="Unlock" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ImageNumber") %>' ToolTip="Click to unlock page"></asp:imagebutton>
                                                                            </td>
															            </tr>
														            </table>
													            </td>
													            <td height="16" width="34" align="right">
														            <table id="Table8" border="0" cellspacing="0" cellpadding="0" width="34">
															            <tr>
																            <td style="white-space:nowrap;">
																	            <asp:imagebutton id="btnApprove" runat="server" BackColor="Transparent" Height="16" BorderWidth="0px" ImageUrl="../Images/Approve2.gif" CommandName="Approve" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ImageNumber") %>' ToolTip="Click to approve page" ImageAlign="Left" ></asp:imagebutton>
                                                                            </td>
																            <td style="white-space:nowrap;">
																	            <asp:imagebutton id="btnDisapprove" runat="server" BackColor="Transparent" Height="16" BorderWidth="0px" ImageUrl="../Images/Disapprove.gif" CommandName="Disapprove" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ImageNumber") %>' ToolTip="Click to disapprove page"></asp:imagebutton>
                                                                            </td>
															            </tr>
														            </table>
													            </td>

                                                               
												            </tr>
											            </table>
										            </td>
									            </tr>
								            </table>
							           
				            </ItemTemplate>
			            </asp:datalist>
                </div>
                    <div style="text-align: center;">
                        <asp:label id="lblError" runat="server" ForeColor="Red"></asp:label>
			        </div>          
                         <telerik:RadWindowManager id="RadWindowManager1" runat="server" Skin="Vista" RenderMode="Lightweight">
				            <Windows>
				                <telerik:RadWindow  runat="server" Height="470px"  ID="radWindowShowRipStatus" 
                                    Width="620px" Skin="Office2010Blue" DestroyOnClose="True" Left="" NavigateUrl="MessageSimple.aspx" Behaviors="Close"
					    	        VisibleStatusbar="False" Modal="True" Top="" Title="RIP message" ></telerik:RadWindow>
						        <telerik:RadWindow runat="server" Height="470px" ID="radWindowShowFtpStatus"
							        Width="620px" Skin="Office2010Blue" DestroyOnClose="True" Left="" NavigateUrl="MessageSimple.aspx" Behaviors="Close"
							        VisibleStatusbar="False" Modal="True" Top="" Title="FTP message" ></telerik:RadWindow>
						        <telerik:RadWindow runat="server" Height="470px"  ID="radWindowShowPreflightStatus"
							        Width="620px" Skin="Office2010Blue" DestroyOnClose="True" Left="" NavigateUrl="MessageSimple.aspx" Behaviors="Close"
							        VisibleStatusbar="False" Modal="True" Top="" Title="Preflight message" ></telerik:RadWindow>
                                <telerik:RadWindow runat="server" Height="470px"  ID="radWindowShowInkSaveStatus"
								    Width="620px" Skin="Office2010Blue" DestroyOnClose="True" Left="" NavigateUrl="MessageSimple.aspx" Behaviors="Close"
								    VisibleStatusbar="False" Modal="True" Top="" Title="InkSave message"></telerik:RadWindow>
						        <telerik:RadWindow runat="server" Height="560px"  ID="radWindowShowColorStatus"  IconUrl="../Images/dns16.gif"
							        Width="752px" Skin="Office2010Blue" DestroyOnClose="True" Left="" NavigateUrl="Message.aspx" Behaviors="Close"
							        VisibleStatusbar="False" Modal="True" Top="" Title="Color density status" ></telerik:RadWindow>
						        <telerik:RadWindow runat="server" ID="radWindowPitstopReport" DestroyOnClose="True" Skin="Office2010Blue" 
							        Title="Pitstop Server report" Width="800px" Height="600px" Modal="True" Behaviors="Close" Top=""
							        NavigateUrl="PitstopReportview.aspx" Left="" ></telerik:RadWindow>
						        <telerik:RadWindow runat="server" ID="radWindowAlwanReport" DestroyOnClose="True" Skin="Office2010Blue" 
							        Title="Alwan report" Width="800px" Height="600px" Modal="True" Behaviors="Close" Top=""
							        NavigateUrl="AlwanReportview.aspx" Left="" ></telerik:RadWindow>

						        <telerik:RadWindow runat="server" Height="470px" ID="radWindowShowPageHistory" IconUrl="../Images/pagehistory.gif"
							        Width="620px" Skin="Office2010Blue" DestroyOnClose="True" Left="" NavigateUrl="ShowPageHistory.aspx"
							        Behaviors="Close" VisibleStatusbar="False" Modal="True" Top="" Title="Page history" ></telerik:RadWindow>
						     
						        <telerik:RadWindow runat="server" Height="200px" Width="280"  ID="radWindowChangeColor" IconUrl="../Images/colorcmyk16.gif"
							        Skin="Office2010Blue" DestroyOnClose="True" Left="" NavigateUrl="ChangeColor.aspx" Behaviors="Close" VisibleStatusbar="False"
							        Modal="True" Top="" Title="Change color for page" ></telerik:RadWindow>
						        <telerik:RadWindow runat="server" Height="200px" Width="380" ID="radWindowHardProof" IconUrl="../Images/Printer.gif"
							        Skin="Office2010Blue" DestroyOnClose="True" Left="" NavigateUrl="HardProof.aspx" Behaviors="Close" VisibleStatusbar="False"
							        Modal="True" Top="" Title="Page hardproof" ></telerik:RadWindow>
						        <telerik:RadWindow runat="server" Height="250px" Width="380" ID="radWindowReprocess" IconUrl="../Images/cog.gif"
							        Skin="Office2010Blue" DestroyOnClose="True" Left="" NavigateUrl="ReprocessPage.aspx" Behaviors="Close" VisibleStatusbar="False"
							        Modal="True" Top="" Title="Re-process page" ></telerik:RadWindow>
                                <telerik:RadWindow runat="server" Height="300px" Width="500"  ID="radWindowUploadFile"
							        Skin="Office2010Blue" DestroyOnClose="True" Left="" NavigateUrl="UploadFile.aspx" Behaviors="Close" VisibleStatusbar="False"
							        Modal="True" Top="" Title="Upload PDF file" ></telerik:RadWindow>
                                <telerik:RadWindow runat="server" Height="250px" Width="500"  ID="radWindowApproveAll" IconUrl="../Images/Approve2.gif"
								   Skin="Office2010Blue" DestroyOnClose="True" Left="" NavigateUrl="ApproveAll.aspx" Behaviors="Close" VisibleStatusbar="False"
								 Modal="True" Top="" Title="Approve all" ></telerik:RadWindow>
                               <telerik:RadWindow runat="server" Height="200px" Width="250"  ID="radWindowKill" IconUrl="../Images/waste.gif"
								Skin="Office2010Blue" DestroyOnClose="True" Left="" NavigateUrl="Pagekill.aspx" Behaviors="Close" VisibleStatusbar="False"
								Modal="True" Top="" Title="Page kill" ></telerik:RadWindow>
							    <telerik:RadWindow runat="server" Height="200px" Width="500"  ID="radWindowCustomAction"
								Skin="Office2010Blue" DestroyOnClose="True" Left="" NavigateUrl="CustomAction.aspx" Behaviors="Close"
								VisibleStatusbar="False" Modal="True" Top="" Title="Custom Action" ></telerik:RadWindow>

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
