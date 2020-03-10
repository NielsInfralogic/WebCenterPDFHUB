<%@ Page language="c#" Codebehind="Main.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Main" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>InfraLogic WebCenter</title>
		<link rel="shortcut icon" href="images/IL.ico" />
		<link rel="icon" href="images/IL.ico" type="image/ico" />
		<link href="Styles/WebCenter.css" type="text/css" rel="stylesheet" />
        <telerik:RadCodeBlock ID="RadCodeBlock2" runat="server">
            <script language="JavaScript" type="text/javascript">

                var frameset = top.document.getElementById("mainframeset");
                if (frameset != null) {
                    origCols = frameset.cols;
                    frameset.cols = "210, *";
                }
            </script>
        </telerik:RadCodeBlock>
        <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
		    <script type="text/javascript">

                 var appletHeight;
                 var appletWidth;
                 var isMac = 0;

                 function clearCookie(name, domain, path) {
                     var domain = domain || document.domain;
                     var path = path || "/";
                     document.cookie = name + "=; expires=" + +new Date + "; domain=" + domain + "; path=" + path;
                 };

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

//                     if (navigator.userAgent.toLowerCase().indexOf("ipad") > 0)
//                         appletHeight = appletHeight - 20;

                     if (appletHeight < 400)
                         appletHeight = 400;
                     if (appletWidth < 600)
                         appletWidth = 600;

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
		    
		        function DOMCall(name) {
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

		        function OnClientTabSelected(sender, eventArgs) {
		            var olddate = new Date();
		            olddate.setDate(olddate.getDate() - 1);
		            clearCookie("ScrollY", "", "");
		            clearCookie("ScrollReadviewY", "", "");
		            clearCookie("ScrollFlatY", "", "");
                }


            </script>
        </telerik:RadCodeBlock>

        <style type="text/css">  
            html, body, form  
            {  
                height: 100%;  
                margin: 0px;  
                padding: 0px;  
                overflow: hidden;  
            }  
            iframe 
            {
                overflow: hidden;
            }
        </style> 

	<meta name="description" content="WebCenter main page" />
</head>
	<body onresize="GetBrowserDim()" >
		<form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
                <telerik:RadMultiPage id="RadMultiPage1" runat="server"  Width="100%" ScrollBars="None">
                    <telerik:RadPageView id="PageViewPages" runat="server" ContentUrl="Views/Thumbnailview2.aspx"  >
                        <iframe id="IFrame1" runat="server" frameborder="0" marginwidth="0" marginheight="0" vspace="0" hspace="0" style="width: 100%; height: 100%;overflow: hidden;" src="Views/Thumbnailview2.aspx" scrolling="no" allowTransparency="true"></iframe>
                    </telerik:RadPageView>
                    <telerik:RadPageView id="PageViewSpreads" runat="server" ContentUrl="Views/Readview.aspx">
                        <iframe id="IFrame2" runat="server" frameborder="0" marginwidth="0" marginheight="0" vspace="0" hspace="0" style="width: 100%; height: 100%;overflow: hidden;" src="Views/Readview.aspx" scrolling="no" allowTransparency="true"></iframe>
                    </telerik:RadPageView>

                    <telerik:RadPageView id="PageViewFlats" runat="server" ContentUrl="Views/Flatview3.aspx">
                        <iframe id="IFrame3" runat="server" frameborder="0" marginwidth="0" marginheight="0" vspace="0" hspace="0" style="width: 100%; height: 100%;overflow: hidden;" src="Views/Flatview3.aspx" scrolling="no" allowTransparency="true"></iframe>
                    </telerik:RadPageView>
                                        
      
                    <telerik:RadPageView id="PageViewList" runat="server" ContentUrl="Views/ClassicTableView.aspx">
                        <iframe id="IFrame4" runat="server" frameborder="0" marginwidth="0" marginheight="0" vspace="0" hspace="0" style="width: 100%; height: 100%;overflow: hidden;" src="Views/ClassicTableView.aspx" scrolling="no" allowTransparency="true"></iframe>
                    </telerik:RadPageView>
                    <telerik:RadPageView id="PageViewStatus" runat="server" ContentUrl="Views/TableView.aspx">
                        <iframe id="IFrame5" runat="server" frameborder="0" marginwidth="0" marginheight="0" vspace="0" hspace="0" style="width: 100%; height: 100%;overflow: hidden;" src="Views/TableView.aspx" scrolling="no" allowTransparency="true"></iframe>
                    </telerik:RadPageView>
                    <telerik:RadPageView id="PageViewRuns" runat="server" ContentUrl="Views/PressRuns.aspx">
                        <iframe id="IFrame6" runat="server" frameborder="0" marginwidth="0" marginheight="0" vspace="0" hspace="0" style="width: 100%; height: 100%;overflow: hidden;" src="Views/PressRuns.aspx" scrolling="no" allowTransparency="true"></iframe>
                    </telerik:RadPageView>
                    <telerik:RadPageView id="PageViewPlan" runat="server" ContentUrl="Views/PlanView.aspx">
                        <iframe id="IFrame7" runat="server" frameborder="0" marginwidth="0" marginheight="0" vspace="0" hspace="0" style="width: 100%; height: 100%;overflow: hidden;" src="Views/PlanView.aspx" scrolling="no" allowTransparency="true"></iframe>
                    </telerik:RadPageView>
                    <telerik:RadPageView id="PageViewStatistics" runat="server" ContentUrl="Views/ReportView.aspx">
                        <iframe id="IFrame8" runat="server" frameborder="0" marginwidth="0" marginheight="0" vspace="0" hspace="0" style="width: 100%; height: 100%;overflow: hidden;" src="Views/ReportView.aspx" scrolling="no" allowTransparency="true"></iframe>
                    </telerik:RadPageView>
                    <telerik:RadPageView id="PageViewUnknownFiles" runat="server" ContentUrl="Views/UnknownFiles.aspx">
                        <iframe id="IFrame9" runat="server" frameborder="0" marginwidth="0" marginheight="0" vspace="0" hspace="0" style="width: 100%; height: 100%;overflow: hidden;" src="Views/UnknownFiles.aspx" scrolling="no" allowTransparency="true"></iframe>
                    </telerik:RadPageView>
                    <telerik:RadPageView id="PageViewLogs" runat="server" ContentUrl="Views/LogView.aspx">
                        <iframe id="IFrame10" runat="server" frameborder="0" marginwidth="0" marginheight="0" vspace="0" hspace="0" style="width: 100%; height: 100%;overflow: hidden;" src="Views/LogView.aspx" scrolling="no" allowTransparency="true"></iframe>
                    </telerik:RadPageView>
                     <telerik:RadPageView id="PageViewUpload" runat="server" ContentUrl="Views/Uploadview.aspx">
                        <iframe id="IFrame11" runat="server" frameborder="0" marginwidth="0" marginheight="0" vspace="0" hspace="0" style="width: 100%; height: 100%;overflow: hidden;" src="Views/Uploadview.aspx" scrolling="no" allowTransparency="true"></iframe>
                    </telerik:RadPageView>
                </telerik:RadMultiPage>

                <telerik:RadTabStrip ID="RadTabStrip1" runat="server" Orientation="HorizontalBottom" Skin="Windows7" Width="100%"   AutoPostBack="True" OnTabClick="RadTabStrip1_TabClick" MultiPageID="RadMultiPage1"  OnClientTabSelected="OnClientTabSelected" >
                    <Tabs>
                        <telerik:RadTab runat="server" Text="Pages" ToolTip="View thumbnail pages" Value="Pages" PageViewID="PageViewPages" ImageUrl="./Images/Pages16.gif">
                        </telerik:RadTab>
                        <telerik:RadTab runat="server" Text="Spreads" ToolTip="View pages two by two" Value="Spreads" PageViewID="PageViewSpreads" ImageUrl="./Images/Spreads16.gif">
                        </telerik:RadTab>
                        <telerik:RadTab runat="server" Text="Plates" ToolTip="View flat (plate) status" Value="Flats" PageViewID="PageViewFlats" ImageUrl="./Images/Flats16.gif">
                        </telerik:RadTab>
                        <telerik:RadTab runat="server" Text="List" ToolTip="Page/separation list" Value="List" PageViewID="PageViewList" ImageUrl="./Images/List16.gif">
                        </telerik:RadTab>
                        <telerik:RadTab runat="server" Text="Status" ToolTip="Status table" Value="Status" PageViewID="PageViewStatus" ImageUrl="./Images/Status16.gif">
                        </telerik:RadTab>
                        <telerik:RadTab runat="server" Text="Press runs" ToolTip="Active press run" Value="Runs" PageViewID="PageViewRuns" ImageUrl="./Images/press16.gif">
                        </telerik:RadTab>
                        <telerik:RadTab runat="server" Text="Plan" ToolTip="Add page plans to system" Value="Plan" PageViewID="PageViewPlan" ImageUrl="./Images/Plan16.gif">
                        </telerik:RadTab>
                        <telerik:RadTab runat="server" Text="Statistics" ToolTip="View input/output statistical data" Value="Statistics" PageViewID="PageViewStatistics" ImageUrl="./Images/Stat16.gif">
                        </telerik:RadTab>
                        <telerik:RadTab runat="server" Text="Unknown files" ToolTip="View unknown input files" Value="UnknownFiles" PageViewID="PageViewUnknownFiles" ImageUrl="./Images/unknown16.gif">
                        </telerik:RadTab>
                        <telerik:RadTab runat="server" Text="Logging" ToolTip="See transmission logs" Value="Logs" PageViewID="PageViewLogs" ImageUrl="./Images/log.gif">
                        </telerik:RadTab>
                        <telerik:RadTab runat="server" Text="Upload" ToolTip="Upload files" Value="Upload" PageViewID="PageViewUpload" ImageUrl="./Images/upload16.gif">
                        </telerik:RadTab>
                    </Tabs>
                </telerik:RadTabStrip>
                <input runat="server" id="HiddenX" type="hidden" value="" />
                <input runat="server" id="HiddenY" type="hidden" value="" />
               

            <script language="JavaScript" type="text/javascript">
                GetBrowserDim();
                SaveWindowSize();
     
            </script>
		</form>
	</body>
</html>
