<%@ Page language="c#" Codebehind="ZoomviewReadFlash2.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.ZoomviewReadFlash2" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>Zoomview spreads</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
		<script type="text/javascript">

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
		            //Netscape percents do not work on applets inside tables so we need to work out the size.
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
	<body>
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
                        <telerik:RadToolBarButton runat="server" Value="CMYK" Text="CMYK" ImageUrl="../Images/separations16.gif" ImagePosition="Left" ToolTip="Show combined colors"  PostBack="true">
                        </telerik:RadToolBarButton>
                        <telerik:RadToolBarButton runat="server" Value="Mask" Text="Mask" ImageUrl="../Images/mask16.gif" ImagePosition="Left" ToolTip="Show image with trim mask"  PostBack="true">
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
                                <div style="padding-left: 30px">
                                    <asp:Label ID="PageName" runat="server" Text="All" CssClass="RadToolbarLabel" ForeColor="DarkGreen"></asp:Label>
                                </div>
                            </ItemTemplate>
                        </telerik:RadToolBarButton>

                        <telerik:RadToolBarButton runat="server" Value="Backward" Text="Previous" ImageUrl="../Images/gobackgray16.gif" ImagePosition="Left" ToolTip="View previous two pages"  PostBack="true">
                        </telerik:RadToolBarButton>
                        <telerik:RadToolBarButton runat="server" Value="Forward" Text="Forward" ImageUrl="../Images/gogray16.gif" ImagePosition="Left" ToolTip="Go to next two pages"  PostBack="true">
                        </telerik:RadToolBarButton>

                        <telerik:RadToolBarButton runat="server" Value="Item4">
                            <ItemTemplate>
                                <table cellpadding="0" cellspacing="0"><tr><td>
                                <asp:Label ID="CommentLabel" runat="server" Text="Comment" CssClass="RadToolbarLabel"></asp:Label>
                                    </td>
                                    <td>
                                        <div style="padding-top:2px;padding-left:5px;">
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
            <div id="mainDiv" style="background-color: #f0f8ff;width: 100%;">
                <script type="text/javascript" src="openseadragon.min.js"></script>
			    <script type="text/javascript">			                
			        GetBrowserDim();

			        var topToolbarHeight = 32;              
			        var sizeTagx = 'width:' + appletWidth + 'px;height:' + (appletHeight - topToolbarHeight) + 'px;';
				
                    if (<%= simpleFlash %> == 2) {
				        document.write(' <iframe src="PDFview.aspx" style="overflow: auto;" width="100%" height="100%"></iframe>');
				    } else if (<%= simpleFlash %> == 1) {
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
