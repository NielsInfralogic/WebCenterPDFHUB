<%@ Page language="c#" Codebehind="Menu.aspx.cs" AutoEventWireup="true" Inherits="WebCenter4.Menu" %>

<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<title>Menu</title>
		<link href="images/IL.ico" rel="shortcut icon" />
		<link href="images/IL.ico" type="image/ico" rel="icon" />
		<link href="Styles/WebCenter.css" type="text/css" rel="stylesheet" />
		<script  type="text/javascript">

		    function onButtonClicked(sender, args) {
                if (args.get_item().get_value() == "User")
		            OnProfile();
                else if (args.get_item().get_value() == "Help")
		            OnHelp();
                else if (args.get_item().get_value() == "About")
                    OnAbout();
		    }

             if (<%= updateTreeMenu %>) {
		        parent.tree.document.Form1.action = 'Tree.aspx?refresh=1';
		        parent.tree.document.Form1.submit();
	         } 

		    function OnAbout() {
				var AboutWindow;
				var xpos = 100;
				var ypos = 100;
				if(window.screen) {
					xpos = (screen.width-400)/2;
					ypos = (screen.height-330)/2; 	  
				}		 
				var s = "status=no,top="+ypos+",left="+xpos+",width=400,height=330";
				AboutWindow = window.open("./About.aspx","About",s);	
				if (parseInt(navigator.appVersion) >= 4) 
					AboutWindow.focus();
		    }

		    function OnHelp() {
				var HelpWindow;
				 var xpos = 100;
				var ypos = 100;
				if(window.screen) {
					xpos = (screen.width-640)/2;
					ypos = (screen.height-680)/2; 	  
				}		 
				var s = "status=no,top="+ypos+",left="+xpos+",width=640,height=680";
				HelpWindow = window.open("./iHelp/Help.htm","Help",s);	
				if (parseInt(navigator.appVersion) >= 4) 
					HelpWindow.focus();
	    	}

    		function OnProfile() {
				var PrefWindow;
				var xpos = 100;
				var ypos = 100;
				if(window.screen) {
					xpos = (screen.width-480)/2;
					ypos = (screen.height-300)/2; 	  
				}		 
				var s = "status=no,top="+ypos+",left="+xpos+",width=480,height=300";
				PrefWindow = window.open("Profile.aspx","Userprefs", s);
				if (parseInt(navigator.appVersion) >= 4) 		
					PrefWindow.focus();
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
            <asp:ScriptManager ID="ScriptManager2" runat="server">
            </asp:ScriptManager>
            <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;" OnClientButtonClicked="onButtonClicked" OnButtonClick="RadToolBar1_ButtonClick" CssClass="smallToolBar">
                <Items>
                    
                    <telerik:RadToolBarButton runat="server" Value="User" Text="My profile" ImageUrl="./Images/user16.gif"  ToolTip="Edit my login profile" Target="_top" PostBack="false" ImagePosition="Left"  >
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Value="HideOld" CheckOnClick="true"  AllowSelfUnCheck="true" Text="Hide old products" ToolTip="Hide products with pubdata older than today"  ImageUrl="./Images/clock16.gif" ImagePosition="Left" Visible="false" >
                    </telerik:RadToolBarButton>

                    <telerik:RadToolBarButton runat="server" Value="Item1">
                        <ItemTemplate>
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                   <td style="padding-top: 4px;">
                                       <asp:Label ID="txtPubDate" runat="server" Text="Date" CssClass="RadToolbarLabel"></asp:Label>
                                   </td>
                                  <td>
                                     <div style="padding-top:4px;padding-left: 3px">
                                        <asp:DropDownList ID="PubDateFilter" runat="server" EnableViewState="true" AutoPostBack="true" OnSelectedIndexChanged="OnSelChangePubDate"></asp:DropDownList>                                           
                                     </div>
                                  </td>
                                </tr>
                            </table>

                        </ItemTemplate>     
                    </telerik:RadToolBarButton>

                    <telerik:RadToolBarButton runat="server" Value="Logout" Text="Logout" ImageUrl="./Images/logout16.gif" ToolTip="Log out of WebCenter" Target="_top" NavigateUrl="Login.aspx?logout=1" >
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Value="Help" Text="Help" ImageUrl="./Images/help16.gif"  Target="_top"  PostBack="false"  >
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Value="About" Text="About" ImageUrl="./Images/weblogosmall16.gif" Target="_top" PostBack="false" >
                    </telerik:RadToolBarButton>
                    <telerik:RadToolBarButton runat="server" Value="CustomMenu" Text="CustomMenu"  Target="_top"  Visible="false">
                    </telerik:RadToolBarButton>

                    <telerik:RadToolBarButton runat="server" Value="Label">
                        <ItemTemplate> 
                            <div id="headerlabeldiv" style="padding-top:4px;padding-left: 500px">
                                <asp:Label ID="LabelContent" runat="server" Text="" Height="22" CssClass="RadToolbarLabelBold"></asp:Label>
                            </div>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>

                </Items>
            </telerik:RadToolBar>
			<telerik:RadWindowManager id="RadWindowManager1" runat="server" Skin="Vista" 
                Modal="True" DestroyOnClose="True">
				<Windows>
					<telerik:RadWindow Height="520px" SkinsPath="~/RadControls/Window/Skins" ID="radWindowShowMessage"
						Width="640px" Skin="Vista" DestroyOnClose="True" Left="" NavigateUrl="Views/ShowMessage.aspx"
						Behaviors="Close" VisibleStatusbar="False" Modal="True" Top="" Title="Chat message"></telerik:RadWindow>
				</Windows>
			</telerik:RadWindowManager>

             <telerik:RadCodeBlock ID="RadCodeBlock3" runat="server">
			    <script type="text/javascript">

			        var appletWidth;

			        if (navigator.userAgent.toLowerCase().indexOf("mac_") > 0) {
			            appletWidth = document.body.clientWidth - 5;
			        } else if (navigator.userAgent.toLowerCase().indexOf("msie") > 0) {
			            if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight)) {
			                appletWidth = document.documentElement.clientWidth - 5;
			            } else {
			                appletWidth = document.body.clientWidth - 5;
			            }
			        } else if ((navigator.userAgent.toLowerCase().indexOf("macintosh") > 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla") > 0)) {
			            appletWidth = window.innerWidth - 5;
			        } else if ((navigator.userAgent.toLowerCase().indexOf("safari") > 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla") > 0)) {
			            appletWidth = window.innerWidth - 20;
			        } else if (navigator.userAgent.toLowerCase().indexOf("netscape6") > 0) {
			            appletWidth = window.innerWidth - 25;
			        } else {
			            appletWidth = window.innerWidth - 25;
			        }

			        var n = DOMCall('headerlabeldiv');
                    
                    if (n && appletWidth > 0) {                                          
                        n.style.paddingLeft = appletWidth- 650;
                    }
                 </script>
             </telerik:RadCodeBlock>  
        </form>
	</body>
</html>
