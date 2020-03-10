<%@ Page language="c#" Codebehind="ZoomviewFlatFlash2.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.ZoomviewFlatFlash2" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Zoomview Flat</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
		<script language="JavaScript" type="text/javascript">

		    var frameset = top.document.getElementById("mainframeset");
		    if (frameset != null) {
		        origCols = frameset.cols;
		        frameset.cols = "0, *";
		    }
		    var appletHeight;
		    var appletWidth;
		    var isOldMac = 0;

		    function GetBrowserDim() {
		        if (navigator.userAgent.toLowerCase().indexOf("mac_") > 0) {
		            appletWidth = document.body.clientWidth;
		            appletHeight = document.body.clientHeight;
		        } else if (navigator.userAgent.toLowerCase().indexOf("msie") > 0) {
		            if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) {
		                // IE 6+
		                appletWidth = document.documentElement.clientWidth;
		                appletHeight = document.documentElement.clientHeight;
		            } else {
		                appletHeight = document.body.clientHeight;
		                appletWidth = document.body.clientWidth;
		            }
		        } else if ((navigator.userAgent.toLowerCase().indexOf("macintosh") > 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla") > 0)) {
		            appletWidth = window.innerWidth;
		            appletHeight = window.innerHeight;
		        } else if ((navigator.userAgent.toLowerCase().indexOf("chrome") > 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla") > 0)) {
		            appletWidth = window.innerWidth;
		            appletHeight = window.innerHeight;
		        } else if ((navigator.userAgent.toLowerCase().indexOf("safari") > 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla") > 0)) {
		            appletWidth = window.innerWidth;
		            appletHeight = window.innerHeight;
		        } else if (navigator.userAgent.toLowerCase().indexOf("netscape6") > 0) {
		            appletWidth = window.innerWidth;
		            appletHeight = window.innerHeight;
		        } else if ((window.opera) || (document.all && (!(document.compatMode && document.compatMode == "CSS1Compat"))))  {
		            appletHeight = document.body.clientHeight;
		            appletWidth = document.body.clientWidth;
		        } else {
		            appletHeight = window.innerHeight;
		            appletWidth = window.innerWidth - 25;
		        }

		        if (appletHeight < 400)
		            appletHeight = 400;
		        if (appletWidth < 800)
		            appletWidth = 800;

		        if (navigator.userAgent.toLowerCase().indexOf("mac_") > 0 && navigator.userAgent.toLowerCase().indexOf("msie") > 0)
		            isOldMac = 1;
		    }

		    function doClose()
		    {
		        parent.window.close();
		    }		

		    function onButtonClicked(sender, args) {
		        if (args.get_item().get_value() == "Print")
		            PrintImage();
		    }
    
		    function PrintImage() 
		    {
			    var w = null;
			    if (w && !w.closed)
				    w.close();
				
			    w = open ('', 'imagePrint', 'menubar=1,locationbar=0,statusbar=0,resizable=1,scrollbars=1,width=50,height=50');
			    var html = '';
			    html += '<html><body ONLOAD="if (window.print) window.print(); '
					    + 'setTimeout(\'window.close();\', 10000);">';
			    html += '<img SRC="<%= printimagepath %>">';
			    html += '<\/BODY>;<\/HTML>';
			    w.document.open();
			    w.document.write(html);
			    w.document.close();
			}

			function DOMCall(name) {
			    if (document.layers) //checks document.layers  
			        return document.layers[name];
			    else if (document.all) //checks document.all  
			        return document.all[name];
			    else if (document.getElementById) //checks getElementById  
			        return document.getElementById(name);
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
		<form id="Form1" method="post" runat="server">        			
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
			     <div style="width: 100%; display: block; float: none; height: 32px;">
                        <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;" OnButtonClick="RadToolBar1_ButtonClick" OnClientButtonClicked="onButtonClicked" CssClass="smallToolBar" >
                           <Items>
                                <telerik:RadToolBarButton runat="server" Value="Close" Text="Close" ImageUrl="../Images/close16.gif" ImagePosition="Left" ToolTip="Back to page gallery"  PostBack="true">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                                </telerik:RadToolBarButton>                    
                                <telerik:RadToolBarButton runat="server" Value="CMYK" Text="CMYK" ImageUrl="../Images/separations16.gif" ImagePosition="Left" ToolTip="Show combined colors"  PostBack="true">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" Value="Dns" Text="TAC" ImageUrl="../Images/dns16.gif" ImagePosition="Left" ToolTip="Show Total Ink Coverage map"  PostBack="true">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" Value="C" Text="C" ImageUrl="../Images/csep16.gif" ImagePosition="Left" ToolTip="Show cyan separation"  PostBack="true">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" Value="M" Text="M" ImageUrl="../Images/msep16.gif" ImagePosition="Left" ToolTip="Show magenta separation"  PostBack="true">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" Value="Y" Text="Y" ImageUrl="../Images/ysep16.gif" ImagePosition="Left" ToolTip="Show yellow separation"  PostBack="true">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" Value="K" Text="K" ImageUrl="../Images/ksep16.gif" ImagePosition="Left" ToolTip="Show black separation"  PostBack="true">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" Value="CZ" Text="CZ" ImageUrl="../Images/czonesep16.gif" ImagePosition="Left" ToolTip="Show cyan ink zones"  PostBack="true">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" Value="MZ" Text="MZ" ImageUrl="../Images/mzonesep16.gif" ImagePosition="Left" ToolTip="Show magenta ink zones"  PostBack="true">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" Value="YZ" Text="YZ" ImageUrl="../Images/yzonesep16.gif" ImagePosition="Left" ToolTip="Show yellow ink zones"  PostBack="true">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" Value="KZ" Text="KZ" ImageUrl="../Images/kzonesep16.gif" ImagePosition="Left" ToolTip="Show black ink zones"  PostBack="true">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" Value="Approve" Text="Approve" ImageUrl="../Images/approve16.gif" Tooltip="Approve the page" PostBack="true" ImagePosition="Left" >
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" Value="Disapprove" Text="Disapprove" ImageUrl="../Images/reject16.gif" Tooltip="Reject the page" PostBack="true" ImagePosition="Left" >
                                </telerik:RadToolBarButton>                            
                                <telerik:RadToolBarButton runat="server" Value="Release" Text="Release" ImageUrl="../Images/go16.gif" Tooltip="Release the flat" PostBack="true" ImagePosition="Left" >
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" Value="ReleaseBlack" Text="Release K" ImageUrl="../Images/goblack16.gif" Tooltip="Release black plate only" PostBack="true" ImagePosition="Left" >
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" Value="Hold" Text="Hold" ImageUrl="../Images/hold16.gif" Tooltip="Hold the flat" PostBack="true" ImagePosition="Left" >
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" Value="Backward" Text="Previous" ImageUrl="../Images/gobackgray16.gif" ImagePosition="Left" ToolTip="Go to previous available flat"  PostBack="true">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" Value="ForwardO" Text="Forward" ImageUrl="../Images/gogray16.gif" ImagePosition="Left" ToolTip="Go to next available flat"  PostBack="true">
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" Value="ReleaseSpecial" Text="Release to specific press" ImageUrl="../Images/gospecial16.gif" Tooltip="Release to selecte press(es)" PostBack="true" ImagePosition="Left" >
                                </telerik:RadToolBarButton>
                                <telerik:RadToolBarButton runat="server" Value="Print" Text="Print" ImageUrl="../Images/print16.gif" ToolTip="Print the preview (72dpi only)" >
                                </telerik:RadToolBarButton>
                            </Items>
                        </telerik:RadToolBar>
                    </div>      
                    <div id="mainDiv" style="background-color: #f0f8ff;width: 100%;">
     		            <script type="text/javascript" src="ZoomifyImageViewerPro-min.js"></script>
			            <script type="text/javascript">

                            GetBrowserDim();
                            var topToolbarHeight = 32;
			                var sizeTagx = 'width:' + appletWidth + 'px;height:' + (appletHeight - topToolbarHeight) + 'px;';

                            if (<%= simpleFlash %> == 2) {
							    document.write(' <iframe src="PDFview.aspx" style="overflow: auto;" width="100%" height="100%"></iframe>');							
							} else {
							    Z.showImage("myContainer", "<%=sImagePath%>","zNavigatorVisible=1&zToolbarInternal=1&zFullPageVisible=0&zTooltipsVisible=1&zZoomSpeed=10&zPanSpeed=5&zFadeInSpeed=5&zConstrainPan=3&zClickZoom=1&zClickPan=1&zDoubleClickZoom=1&zMousePan=1&zKeys=1&zFullViewVisible=0&zHelpVisible=0&zMaxZoom=200&zSmoothPan=1"); 
								document.write('<div id="myContainer" style="' + sizeTagx + '"></div>');
                            }
			            </script>
			        </div>      
            <telerik:radwindowmanager id="RadWindowManager1" runat="server" Skin="Vista">
				<Windows>
					<telerik:RadWindow runat="server" id="radWindowReleaseLocations" title="Release to selected presses" Height="500px"
						Skin="Vista" Top="" Modal="True" VisibleStatusbar="False" Behaviors="Close" NavigateUrl="ReleasePresses.aspx"
						Left="" DestroyOnClose="True"  Width="500"></telerik:RadWindow>
				</Windows>
			</telerik:radwindowmanager>
			<asp:textbox id="txtReturnedFromPopup" runat="server" ForeColor="Transparent" BorderStyle="None"
				BackColor="Transparent" Height="2px" Width="22px" BorderColor="Transparent"></asp:textbox>
        </form>
	</body>
</html>
