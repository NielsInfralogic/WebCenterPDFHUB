<%@ Page language="c#" Codebehind="Tree.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.ProductionTree" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>Tree</title>
		<link rel="shortcut icon" href="images/IL.ico" />
		<link rel="icon" href="images/IL.ico" type="image/ico" />
		<link href="Styles/WebCenter.css" type="text/css" rel="stylesheet" />
        <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
		    <script type="text/javascript">
		        var appletHeight;
		        var appletWidth;
		        var y = 0;

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
					    appletWidth = window.innerWidth - 5;
					    appletHeight = window.innerHeight - 20;
				    } else if ((navigator.userAgent.toLowerCase().indexOf("safari")> 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla")> 0)) {
					    appletWidth = window.innerWidth - 20;
					    appletHeight = window.innerHeight - 25;
				    } else if (navigator.userAgent.toLowerCase().indexOf("netscape6")> 0) {
				        appletWidth = window.innerWidth - 25;
				        appletHeight = window.innerHeight - 35;
				    } else if ((window.opera) || (document.all && (!(document.compatMode && document.compatMode == "CSS1Compat"))))  {
				        appletHeight = document.body.clientHeight - 30;
				        appletWidth = document.body.clientWidth - 5;
				    } else {  // Fallback
				        appletWidth = window.innerWidth - 25;
				        appletHeight = window.innerHeight - 35;
				    }
			    }

			    function SaveWindowSize() {
			        var n = DOMCall('HiddenX');
			        var m = DOMCall('HiddenY');

			        if (n && m && appletHeight > 0 && appletWidth > 0) {
			            n.value = appletWidth;
			            m.value = appletHeight;
			        }
			    }

			    function SaveWindowSizeOnLoad() {
			        GetBrowserDim();
			        SaveWindowSize(); // Will not succeed on initial load ..
			    }
		
			    function RefreshMain() {
				   // GetBrowserDim(); 				
    			     <% if (updateMain) { %>
			            var today = new Date();
			            var expire = new Date();
			            expire.setTime(today.getTime() + 1000 * 60 * 60 * 24 * 365);
			            document.cookie = "ScrollPosY=0; expires=" + expire.toGMTString();
			            parent.main.location.href = './Main.aspx';
			            parent.menu.location.href = './Menu.aspx';
			        <% } %>		
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
        </telerik:RadCodeBlock>

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
	<body style="background-color: #f0f8ff;" onresize="SaveWindowSizeOnLoad()"  onload="RefreshMain()">
		<form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>

                <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;" OnButtonClick="RadToolBar1_ButtonClick" CssClass="smallToolBar">
                    <Items>
                        <telerik:RadToolBarButton runat="server" Value="Refresh"  ImageUrl="./Images/refresh16.gif"  ToolTip="Refresh tree view"  PostBack="true" ImagePosition="Left" >
                        </telerik:RadToolBarButton>
                        <telerik:RadToolBarButton runat="server" Value="Expand"  Width="20" ToolTip="Expand tree"  ImageUrl="./Images/expand16.gif" >
                        </telerik:RadToolBarButton>
                        <telerik:RadToolBarButton runat="server" Value="Collapse"  Width="20" ToolTip="Collapse tree"  ImageUrl="./Images/collapse16.gif" >
                        </telerik:RadToolBarButton>              
                        <telerik:RadToolBarButton runat="server" Value="Item1" PostBack="true">
                            <ItemTemplate>
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td style="padding-top: 4px;">
                                            <asp:Label ID="txtPress" runat="server" Text="Publisher" CssClass="RadToolbarLabel"></asp:Label>
                                        </td>
                                        <td>
                                            <div style="padding-top:4px;padding-left:3px;">   
                                                <asp:DropDownList ID="PressSelector" runat="server" EnableViewState="true" AutoPostBack="true" OnSelectedIndexChanged="OnSelChangePublisher"></asp:DropDownList>                                           
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>   
                       </telerik:RadToolBarButton> 
                    </Items>
                </telerik:RadToolBar>                                              
                <telerik:RadToolBar ID="RadToolBar2" Runat="server" Skin="Office2010Blue" style="width: 100%;" OnButtonClick="RadToolBar1_ButtonClick" CssClass="smallToolBar">
                <Items>
                        <telerik:RadToolBarButton runat="server" Value="Item2" PostBack="true">
                            <ItemTemplate>
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td style="padding-top: 4px;">
                                            <asp:Label ID="txtPress" runat="server" Text="Channel" CssClass="RadToolbarLabel"></asp:Label>
                                        </td>
                                        <td>
                                            <div style="padding-top:4px;padding-left:3px;">   
                                                <asp:DropDownList ID="ChannelSelector" runat="server" EnableViewState="true" AutoPostBack="true" Width="120" OnSelectedIndexChanged="OnSelChangeChannel"></asp:DropDownList>                                           
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>     
                        </telerik:RadToolBarButton> 
                </Items>
            </telerik:RadToolBar>

            <div id="Div2" >
                <telerik:RadTreeView ID="RadTreeView1" runat="server" Skin="Simple" OnNodeClick="RadTreeView1_NodeClick1" Width="100%">
                </telerik:RadTreeView>
            </div>

            <input runat="server" id="HiddenX" type="hidden" value="" />
            <input runat="server" id="HiddenY" type="hidden" value="" />
                    
            <telerik:RadCodeBlock ID="RadCodeBlock2" runat="server">
			    <script type="text/javascript">
			        SaveWindowSizeOnLoad();

			        <% if (forcedreload) { %>
			        document.forms[0].submit();
//    			        parent.tree.location.href = './Tree.aspx';

                    <% } %>
                </script>
            </telerik:RadCodeBlock>
        </form>
	</body>
</html>
