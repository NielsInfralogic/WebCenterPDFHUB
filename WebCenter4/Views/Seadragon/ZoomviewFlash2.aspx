<%@ Page language="c#" Codebehind="ZoomviewFlash2.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.ZoomviewFlash2" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>Zoomview</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
		<script type="text/javascript">

            var frameset = top.document.getElementById("mainframeset");
            if (frameset != null) {
                origCols = frameset.cols;
                frameset.cols = "0, *";
            }

            var appletHeightZ;
            var appletWidthZ;
            var isOldMac = 0;
            var isSafari = 0;

            function GetBrowserDim() {
                if (navigator.userAgent.toLowerCase().indexOf("mac_") > 0) {
                    appletWidthZ = document.body.clientWidth;
                    appletHeightZ = document.body.clientHeight;
                } else if (navigator.userAgent.toLowerCase().indexOf("msie") > 0) {
                    if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) {
                        // IE 6+
                        appletWidthZ = document.documentElement.clientWidth;
                        appletHeightZ = document.documentElement.clientHeight;
                    } else {
                        appletWidthZ = document.body.clientWidth;
                        appletHeightZ = document.body.clientHeight;
                    }
                } else if ((navigator.userAgent.toLowerCase().indexOf("macintosh") > 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla") > 0)) {
                    appletWidthZ = window.innerWidth;
                    appletHeightZ = window.innerHeight;
                } else if ((navigator.userAgent.toLowerCase().indexOf("chrome") > 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla") > 0)) {
                    appletWidthZ = window.innerWidth;
                    appletHeightZ = window.innerHeight;
                } else if ((navigator.userAgent.toLowerCase().indexOf("safari") > 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla") > 0)) {
                    appletWidthZ = window.innerWidth;
                    appletHeightZ = window.innerHeight;
                    isSafari = 1;
                } else if (navigator.userAgent.toLowerCase().indexOf("netscape6") > 0) {
                    appletWidthZ = window.innerWidth;
                    appletHeightZ = window.innerHeight;
                } else if ((window.opera) || (document.all && (!(document.compatMode && document.compatMode == "CSS1Compat"))))  {                    
                    appletWidthZ = document.body.clientWidth;
                    appletHeightZ = document.body.clientHeight;
                } else {
                    //Netscape percents do not work on applets inside tables so we need to work out the size.                    
                    appletWidthZ = window.innerWidth ;
                    appletHeightZ = window.innerHeight;
                }

                if (appletHeightZ < 400)
                    appletHeightZ = 400;
                if (appletWidthZ < 800)
                    appletWidthZ = 800;

                if (navigator.userAgent.toLowerCase().indexOf("mac_") > 0 && navigator.userAgent.toLowerCase().indexOf("msie") > 0)
                    isOldMac = 1;
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
	<body onresize="GetBrowserDim()" >
		<form id="Form1" method="post" runat="server">
			<asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
			     <div style="width: 100%; display: block; float: none; height: 32px;">                            
                    <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;" OnButtonClick="RadToolBar1_ButtonClick" OnClientButtonClicked="onButtonClicked" CssClass="smallToolBar" >
                       <Items>
                            <telerik:RadToolBarButton runat="server" Value="Close" Text="Close" ImageUrl="../Images/close16.gif" ImagePosition="Left" ToolTip="Back to page gallery"  PostBack="true">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                            </telerik:RadToolBarButton>                    
                            <telerik:RadToolBarButton runat="server" Value="Approve" Text="Approve" ImageUrl="../Images/approve16.gif" Tooltip="Approve the page" PostBack="true" ImagePosition="Left" >
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton runat="server" Value="Disapprove" Text="Disapprove" ImageUrl="../Images/reject16.gif" Tooltip="Reject the page" PostBack="true" ImagePosition="Left" >
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton runat="server" Value="Item3">
                                <ItemTemplate> 
                                    <div style="padding-top:3px;padding-left: 10px;padding-right: 10px;">
                                        <asp:Label ID="PageName" runat="server" Text="All" CssClass="RadToolbarLabel" ForeColor="DarkGreen"></asp:Label>
                                    </div>
                                </ItemTemplate>
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                            </telerik:RadToolBarButton>
                           <telerik:RadToolBarButton runat="server" Value="Item5">
                                <ItemTemplate> 
                                    <div style="padding-top:3px;padding-left: 10px;padding-right: 10px;">
                                        <asp:Label ID="PageFormat" runat="server" Text="All" CssClass="RadToolbarLabel" ForeColor="DarkGreen"></asp:Label>
                                    </div>
                                </ItemTemplate>
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton runat="server" Value="Item4">
                                <ItemTemplate>
                                    <table cellpadding="0" cellspacing="0"><tr><td>
                                    <asp:Label ID="CommentLabel" runat="server" Text="Comment" CssClass="RadToolbarLabel"></asp:Label>
                                        </td>
                                        <td>
                                            <div style="padding-top:3px;padding-left:10px;">
                                                <asp:TextBox ID="Comment" runat="server" EnableViewState="true" ToolTip="Save comment on Approve/disapprove" Width="200"></asp:TextBox>
                                            </div>
                                        </td></tr></table>
                                </ItemTemplate>
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton runat="server" Value="SendMail" Text="Send Mail" ImageUrl="../Images/sendmail16.gif" ToolTip="Send mail if Rejected" CheckOnClick="true" AllowSelfUnCheck="true" PostBack="true" ImagePosition="Left">
                            </telerik:RadToolBarButton>

                            <telerik:RadToolBarButton runat="server" Value="Print" Text="Print" ImageUrl="../Images/print16.gif" ToolTip="Print the preview (72dpi only)" >
                            </telerik:RadToolBarButton>
                        </Items>
                    </telerik:RadToolBar>
                            
			    </div>
                <div style="width: 100%; display: block; float: none; height: 32px;">

                    <telerik:RadToolBar ID="RadToolBar2" Runat="server" Skin="Office2010Blue" style="width: 100%;" OnButtonClick="RadToolBar2_ButtonClick" CssClass="smallToolBar" >
                       <Items>
                            <telerik:RadToolBarButton runat="server" Value="CMYK" Text="CMYK" ImageUrl="../Images/separations16.gif" ImagePosition="Left" ToolTip="Show combined colors"  PostBack="true">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton runat="server" Value="Mask" Text="Mask" ImageUrl="../Images/mask16.gif" ImagePosition="Left" ToolTip="Show image with trim mask"  PostBack="true">
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
                            <telerik:RadToolBarButton runat="server" Value="PDF" Text="PDF" ImageUrl="../Images/pdf16.gif" ImagePosition="Left" ToolTip="Show PDF original"  PostBack="true">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton runat="server" Value="PDFCMYK" Text="CMYK/PDF split" ImageUrl="../Images/cmykpdf16.gif" ImagePosition="Left" ToolTip="Show ripped and PDF side-by-side"  PostBack="true">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton runat="server" Value="Raster" Text="Raster" ImageUrl="../Images/dotview16.gif" ImagePosition="Left" ToolTip="Show raster image"  PostBack="true">
                            </telerik:RadToolBarButton>

                            <telerik:RadToolBarButton runat="server" Value="BackwardOnly" Text="Previous" ImageUrl="../Images/gobackgray16.gif" ImagePosition="Left" ToolTip="Go to previous available page"  PostBack="true">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton runat="server" Value="ForwardOnly" Text="Forward" ImageUrl="../Images/gogray16.gif" ImagePosition="Left" ToolTip="Go to next available page"  PostBack="true">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton runat="server" Value="Backward" Text="Appr. & Previous" ImageUrl="../Images/goback16.gif" ImagePosition="Left" ToolTip="Approve and goto previous available page"  PostBack="true">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton runat="server" Value="Forward" Text="Appr. & Forward" ImageUrl="../Images/go16.gif" ImagePosition="Left" ToolTip="Approve and goto next available page"  PostBack="true">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton runat="server" Value="BackwardDis" Text="Reject & Previous" ImageUrl="../Images/gobackdis16.gif" ImagePosition="Left" ToolTip="Reject and goto previous available page"  PostBack="true">
                            </telerik:RadToolBarButton>
                            <telerik:RadToolBarButton runat="server" Value="ForwardDis" Text="Reject & Forward" ImageUrl="../Images/godis16.gif" ImagePosition="Left" ToolTip="Reject and goto next available page"  PostBack="true">
                            </telerik:RadToolBarButton>
                        </Items>
                    </telerik:RadToolBar>
                </div>      
                <div id="mainDiv" style="background-color: #f0f8ff;width: 100%;">
                    <script type="text/javascript" src="openseadragon.min.js"></script>
 			        <script type="text/javascript">
 			            GetBrowserDim();

 			            var topToolbarHeight = 32+32;
 			            var sizeTagx = 'width:' + appletWidthZ + 'px;height:' + (appletHeightZ - topToolbarHeight) + 'px;';
 			            var sizeTagxHalf = 'width:' + appletWidthZ/2 + 'px;height:' + (appletHeightZ - topToolbarHeight) + 'px;';
   
                        if (<%= simpleFlash %> == 3) {
                            document.write('<div id="openseadragon0" style="' + sizeTagx + '"></div>');
                            var viewer = OpenSeadragon({
                                id: "openseadragon0",
                                prefixUrl: "images/",       
                                tileSources: {
                                    type: "image",
                                    url:  "<%=sImagePath%>",
                                    buildPyramid: false,
                                }
                            });
                        } else if (<%= simpleFlash %> == 2) {
                            document.write(' <iframe src="PDFview.aspx" style="overflow: auto;'+ sizeTagx + ';" ></iframe>');
                        } else if (<%= simpleFlash %> == 1) {
                            document.write('<div style="width:50%;float:left;">');
                            document.write('<div id="openseadragon12" style="' + sizeTagxHalf + '"></div>');
                            var viewer = OpenSeadragon({
                                id: "openseadragon12",
                                prefixUrl: "images/",       
                                showNavigator:  false,                                                               
                                tileSources: "<%=sImagePath%>image.xml"
                            });
                            document.write('</div><div style="width:50%;float:left;">');
                            document.write(' <iframe src="PDFview.aspx?mode=2" style="overflow: auto;'+ sizeTagxHalf + ';" ></iframe></div>');
                        } else {
                            document.write('<div id="openseadragon1" style="' + sizeTagx + '"></div>');
                            var viewer = OpenSeadragon({
                                id: "openseadragon1",
                                prefixUrl: "images/",       
                                showNavigator:  false,             
                                tileSources: "<%=sImagePath%>image.xml"
                            });
                        }
                </script>                                     
            </div>
        </form>
	</body>
</html>
