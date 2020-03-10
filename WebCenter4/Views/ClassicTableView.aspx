<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClassicTableView.aspx.cs" Inherits="WebCenter4.Views.ClassicTableView" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Tableview</title>
        <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
		    <script type="text/javascript">
		        var appletHeight = 0;
		        var appletWidth = 0;
		        var scrollY = 0;
		        var CoffeeParse =<%=nRefreshTime%>

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
                        //This is the only way we know how to set WIDTH and HEIGHT for an applet on a Mac
                        appletWidth = window.innerWidth - scrollWidth;
                        appletHeight = window.innerHeight - 30;
                    } else if ((navigator.userAgent.toLowerCase().indexOf("chrome") > 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla") > 0)) {
                        appletWidth = window.innerWidth - scrollWidth;
                        appletHeight = window.innerHeight - 30;
                    } else if ((navigator.userAgent.toLowerCase().indexOf("safari") > 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla") > 0)) {
                        //This is the only way we know how to set WIDTH and HEIGHT for an applet on a Mac
                        appletWidth = window.innerWidth - scrollWidth;
                        appletHeight = window.innerHeight - 30;
                    } else if (navigator.userAgent.toLowerCase().indexOf("netscape6") > 0) {
                        //This is the only way we know how to set WIDTH and HEIGHT for an applet on a Mac
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

		        function SelectAllCheckboxes(chkAll ){
		            var xState=chkAll.checked;   
		            var elm=chkAll.form.elements;
		
		            for(i=0;i<elm.length;i++) {
			            if(elm[i].type=="checkbox" && elm[i].id!=chkAll.id) {
				            if(elm[i].checked!=xState)
					            elm[i].click();
			            }
		            }
		        }
		        function HighlightRow(chkB) {
		            var oItem = chkB;
		            var xState;
		            xState = oItem.checked;
		            if (xState) {
		                chkB.parentElement.parentElement.style.backgroundColor = '#008A8C';
		                chkB.parentElement.parentElement.style.color = '#D3D3D3';
		                chkB.parentElement.parentElement.parentElement.style.backgroundColor = '#008A8C';
		                chkB.parentElement.parentElement.parentElement.style.color = '#D3D3D3';
		                chkB.parentElement.parentElement.parentElement.parentElement.style.backgroundColor = '#008A8C';
		                chkB.parentElement.parentElement.parentElement.parentElement.style.color = '#D3D3D3';
		            } else {
		                chkB.parentElement.parentElement.style.backgroundColor = '#DCDCDC';
		                chkB.parentElement.parentElement.style.color = '#000000';
		                chkB.parentElement.parentElement.parentElement.style.backgroundColor = '#DCDCDC';
		                chkB.parentElement.parentElement.parentElement.style.color = '#000000';
		                chkB.parentElement.parentElement.parentElement.parentElement.style.backgroundColor = '#DCDCDC';
		                chkB.parentElement.parentElement.parentElement.parentElement.style.color = '#000000';
		            }
		        }

		        function SetScrollPosition() {
		            if (  <%= nScollPos %> > 0) { 
                			window.scrollTo(0, <%= nScollPos %>);
			         }
		        }

		        function SaveWindowSize() {
		            var n = DOMCall('HiddenX');
		            var m = DOMCall('HiddenY');
		            if (n && m && appletHeight > 0 && appletWidth > 0) {
		                //  alert(appletWidth);
		                //  alert(appletHeight);
		                n.value = appletWidth;
		                m.value = appletHeight;
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
    <link rel="stylesheet" type="text/css" href="../Styles/WebCenter.css" />
    <style type="text/css">  
        html, body, form  {  
                height: 100%;  
                margin: 0px;  
                padding: 0px;  
                overflow: inherit;  
        }  
    </style>
</head>
<body onscroll="GetScrollPosition()" onresize="GetBrowserDim()">

    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%; " OnButtonClick="RadToolBar1_ButtonClick" CssClass="smallToolBar" >
                    <Items>

                        <telerik:RadToolBarButton runat="server" Value="Refresh" Text="Refresh" ImageUrl="../Images/refresh16.gif" ImagePosition="Left" ToolTip="Refresh thumbnail view"  PostBack="true">
                        </telerik:RadToolBarButton>
                        <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                        </telerik:RadToolBarButton>
                        <telerik:RadToolBarButton runat="server" Value="Separations" Text="Separations" ImageUrl="../Images/separations.gif" ImagePosition="Left" CheckOnClick="true" AllowSelfUnCheck="true" Group="1" ToolTip="Show all separations color by color"  PostBack="true">
                        </telerik:RadToolBarButton>

                        <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                        </telerik:RadToolBarButton>

                        <telerik:RadToolBarButton runat="server" Value="HideCommon" Text="Hide duplicates" ImageUrl="../Images/spacer16.gif" TImagePosition="Left" ToolTip="Hide duplicated sub-edition pages" CheckOnClick="true" AllowSelfUnCheck="true" Group="2" PostBack="true" >
                        </telerik:RadToolBarButton>
                         <telerik:RadToolBarButton runat="server" Value="ShowCopies" Text="Show copies" ImageUrl="../Images/spacer16.gif" TImagePosition="Left" ToolTip="Show all plate copies" CheckOnClick="true" AllowSelfUnCheck="true" Group="5" PostBack="true" >
                        </telerik:RadToolBarButton>
                        <telerik:RadToolBarButton runat="server" Value="HideApproved" Text="Hide approved" ImageUrl="../Images/spacer16.gif" ImagePosition="Left" CheckOnClick="true" AllowSelfUnCheck="true" ToolTip="Hide approved pages" Group="3"  PostBack="true">
                        </telerik:RadToolBarButton>
                        <telerik:RadToolBarButton runat="server" Value="HideFinished" Text="Hide finished" ImageUrl="../Images/spacer16.gif" ImagePosition="Left" CheckOnClick="true" AllowSelfUnCheck="true" ToolTip="Hide pages done" Group="4"   PostBack="true">
                        </telerik:RadToolBarButton>

                        <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                        </telerik:RadToolBarButton>

                        <telerik:RadToolBarButton runat="server" Value="Reimage" Text="Re-image" ImageUrl="../Images/reimage16.gif" ImagePosition="Left" ToolTip="Re-image selected plate"  PostBack="true">
                        </telerik:RadToolBarButton>

                        <telerik:RadToolBarButton runat="server" IsSeparator="True" Text="Sep">
                        </telerik:RadToolBarButton>

                    </Items>
                </telerik:RadToolBar>
                   
            <div id="mainDiv" style="overflow: Auto;background-color: #f0f8ff;width: 100%;height: 500px">

                <asp:datagrid id="DataGrid1" runat="server" Font-Size="10pt" Font-Names="Verdana" BorderStyle="None"
				    BackColor="White" CellSpacing="1" HorizontalAlign="Left" Width="100%" BorderWidth="1px" BorderColor="#999999"
				    CellPadding="1" GridLines="Vertical" AllowSorting="True" OnItemDataBound="DataGrid1_ItemDataBound" OnSortCommand="DataGrid1_SortCommand" OnItemCreated="DataGrid1_ItemCreated">
				    <FooterStyle Wrap="False" Height="22px" ForeColor="Black" BackColor="#CCCCCC"></FooterStyle>
				    <SelectedItemStyle Font-Bold="True" Wrap="False" Height="20px" ForeColor="White" BackColor="#008A8C"></SelectedItemStyle>
				    <EditItemStyle Wrap="False" Height="20px"></EditItemStyle>
				    <AlternatingItemStyle Wrap="False" Height="20px" BackColor="Gainsboro"></AlternatingItemStyle>
				    <ItemStyle Wrap="False" Height="20px" ForeColor="Black" BackColor="#EEEEEE"></ItemStyle>
				    <HeaderStyle Font-Size="Medium" Font-Names="Verdana" Font-Bold="False" Wrap="False" Height="22px"
					    ForeColor="White" BackColor="#000084"></HeaderStyle>
				    <Columns>
					    <asp:TemplateColumn>
						    <HeaderStyle Wrap="False"></HeaderStyle>
						    <ItemStyle Wrap="False"></ItemStyle>
						    <HeaderTemplate>
							    <asp:CheckBox id="chkAll" onclick="javascript:SelectAllCheckboxes(this);" runat="server" ToolTip="Select/de-select All"
								    AutoPostBack="false"></asp:CheckBox>
						    </HeaderTemplate>
						    <ItemTemplate>
							    <asp:CheckBox id="chkSelect" runat="server" onclick="javascript:HighlightRow(this);" OnCheckedChanged="DataGrid1_CheckedChanged"
								    AutoPostBack="false"></asp:CheckBox>
						    </ItemTemplate>
					    </asp:TemplateColumn>
				    </Columns>
				    <PagerStyle HorizontalAlign="Center" ForeColor="Black" BackColor="#999999" Wrap="False" Mode="NumericPages"></PagerStyle>
    			</asp:datagrid>

            </div>
            <div style="text-align: center;">
                <asp:label id="lblChooseProduct" runat="server" ForeColor="Teal" Font-Size="Larger">Choose a product in the tree</asp:label>
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
