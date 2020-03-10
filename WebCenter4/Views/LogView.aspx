<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogView.aspx.cs" Inherits="WebCenter4.Views.LogView" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Log</title>
    <link href="../Style/WebCenter.css" type="text/css" rel="stylesheet"/>
            <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

    		<script  type="text/javascript">

    		    var appletHeight = 0;
    		    var appletWidth = 0;
    		    var scrollY = 0;
    		    var CoffeeParse =<%=nRefreshTime%>

    		    function DOMCall(name) {
    		        if (document.getElementById) //checks getElementById  
    		            return document.getElementById(name);
    		        else if (document.layers) //checks document.layers  
    		            return document.layers[name];
    		        else if (document.all) //checks document.all  
    		            return document.all[name];
    		    }


    
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
    		        } else if ((navigator.userAgent.toLowerCase().indexOf("macintosh") > 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla") > 0)) {
    		            //This is the only way we know how to set WIDTH and HEIGHT for an applet on a Mac
    		            appletWidth = window.innerWidth - 10;
    		            appletHeight = window.innerHeight - 30;
    		        } else if ((navigator.userAgent.toLowerCase().indexOf("chrome") > 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla") > 0)) {
    		            appletWidth = window.innerWidth - 20;
    		            appletHeight = window.innerHeight - 30;
    		        } else if ((navigator.userAgent.toLowerCase().indexOf("safari") > 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla") > 0)) {
    		            //This is the only way we know how to set WIDTH and HEIGHT for an applet on a Mac
    		            appletWidth = window.innerWidth - 20;
    		            appletHeight = window.innerHeight - 30;
    		        } else if (navigator.userAgent.toLowerCase().indexOf("netscape6") > 0) {
    		            //This is the only way we know how to set WIDTH and HEIGHT for an applet on a Mac
    		            appletWidth = window.innerWidth - 25;
    		            appletHeight = window.innerHeight - 35;
    		        } else if ((window.opera) || (document.all && (!(document.compatMode && document.compatMode == "CSS1Compat")))) {
    		            appletHeight = document.body.clientHeight - 30;
    		            appletWidth = document.body.clientWidth - 5;
    		        } else {
    		            //Netscape percents do not work on applets inside tables so we need to work out the size.
    		            appletHeight = window.innerHeight - 35;
    		            appletWidth = window.innerWidth - 25;
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
	<body onscroll="GetScrollPosition()" onresize="GetBrowserDim()">

		<form id="Form1" method="post" runat="server">
			
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
			
            <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;" OnButtonClick="RadToolBar1_ButtonClick">
                <Items>
                    <telerik:RadToolBarButton runat="server" Value="Refresh" Text="Refresh" ImageUrl="../Images/refresh16.gif" ImagePosition="Left" ToolTip="Refresh thumbnail view"  PostBack="true">
                    </telerik:RadToolBarButton>  
                    <telerik:RadToolBarButton runat="server" Value="Item1" PostBack="true">
                        <ItemTemplate>
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td style="padding-top: 4px;">
                                        <asp:Label ID="txtPress" runat="server" Text="Press" CssClass="RadToolbarLabel"></asp:Label>
                                    </td>
                                    <td>
                                        <div style="padding-top:4px;padding-left:3px;">   
                                            <asp:DropDownList ID="PressSelector" runat="server" EnableViewState="true" AutoPostBack="true" OnSelectedIndexChanged="OnSelChangeLocation"></asp:DropDownList>                                           
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </ItemTemplate>     
                    </telerik:RadToolBarButton>     
               
                </Items>
            </telerik:RadToolBar>

            <div id="mainDiv" style="overflow: Auto;background-color: #f0f8ff;width: 100%;height: 500px">
  
			<asp:datagrid id="DataGrid1" runat="server" Font-Size="10pt" Font-Names="Verdana" BorderStyle="None"
				BackColor="White" Width="100%" BorderColor="#999999" BorderWidth="1px" AllowSorting="True"
				GridLines="Vertical" CellPadding="1" HorizontalAlign="Left" CellSpacing="1" AutoGenerateColumns="False" OnSelectedIndexChanged="DataGrid1_SelectedIndexChanged">
				<FooterStyle Wrap="False" Height="22px" ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
				<SelectedItemStyle Font-Bold="True" Wrap="False" Height="20px" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
				<EditItemStyle Wrap="False"></EditItemStyle>
                <AlternatingItemStyle Font-Names="Tahoma" Wrap="False" ForeColor="Black" BackColor="#DCDCDC"></AlternatingItemStyle>
                <ItemStyle Font-Names="Tahoma" Wrap="False" ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
				<HeaderStyle Font-Names="Tahoma" Wrap="False"  ForeColor="White" BackColor="#000084"></HeaderStyle>
				<Columns>
                    <asp:BoundColumn DataField="Location" SortExpression="Location" HeaderText="Location"></asp:BoundColumn>
					<asp:BoundColumn DataField="Time" SortExpression="Time" HeaderText="Time"></asp:BoundColumn>
					<asp:BoundColumn DataField="Status" SortExpression="Status" HeaderText="Status"></asp:BoundColumn>
					<asp:BoundColumn DataField="FileName" SortExpression="FileName" HeaderText="FileName"></asp:BoundColumn>
					<asp:BoundColumn DataField="Message" SortExpression="Message" HeaderText="Message"></asp:BoundColumn>
				</Columns>
				<PagerStyle HorizontalAlign="Center" ForeColor="Black" BackColor="#999999" Wrap="False" Mode="NumericPages"></PagerStyle>
			</asp:datagrid>
		</div>
			<div style="text-align: center;">
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

			        var n = DOMCall('mainDiv');
			        if (n && appletHeight > 0 && appletWidth > 0) {
			            var s = appletHeight-6;
			            n.style.height = s + "px";
			            s = appletWidth;
			            n.style.width = s + "px";
			        }

			    </script>
            </telerik:RadCodeBlock>

		</form>
    </body>
</html>
