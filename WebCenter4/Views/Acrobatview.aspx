<%@ Page language="c#" Codebehind="Acrobatview.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.Acrobatview" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Zoomview</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet"/>
        <script language="JavaScript" type="text/javascript">

        	    var frameset = top.document.getElementById("mainframeset");
        	    if (frameset != null) {
        	        origCols = frameset.cols;
        	        frameset.cols = "0, *";
        	    }

        		var appletHeight;
        		var appletWidth;
             
        		function GetBrowserDim() {
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

        		    if (appletHeight < 600)
        		        appletHeight = 600;
        		    if (appletWidth < 800)
        		        appletWidth = 800;
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
                margin: 2px;  
                padding: 0px;  
                overflow: hidden;  
            }  
        </style>
	</head>
	<body onresize="GetBrowserDim()" onload="GetBrowserDim()">
		<form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            
            <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;" OnButtonClick="RadToolBar1_ButtonClick" OnClientButtonClicked="onButtonClicked">
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
                            <div style="padding-left: 30px">
                                <asp:Label ID="PageName" runat="server" Text="All" CssClass="RadToolbarLabel" ForeColor="DarkGreen"></asp:Label>
                            </div>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>
                </Items>
            </telerik:RadToolBar>                            
			<div id="mainDiv" style="background-color: #f0f8ff;width: 100%; vertical-align: top; text-align:left;">
                <iframe src="PDFview.aspx" width="100%" height="100%"></iframe>
            </div>
            <telerik:RadCodeBlock ID="RadCodeBlock3" runat="server">
			    <script language="JavaScript" type="text/javascript">
			        GetBrowserDim();
			    </script>
            </telerik:RadCodeBlock>  
		</form>
	</body>
</html>
