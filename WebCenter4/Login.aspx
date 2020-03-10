<%@ Page language="c#" Codebehind="Login.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Login" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>InfraLogic WebCenter Login</title>
		<link rel="shortcut icon" href="images/IL.ico" />
		<link rel="icon" href="images/IL.ico" type="image/ico" />
		<link href="Styles/WebCenter.css" type="text/css" rel="stylesheet" />
       
  
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
	<body style="background-color: #99ccff;">
		<form id="Form1" method="post" runat="server" defaultfocus="txtUserName">
               <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            
		    <table id="Table1" Style="height: 100%; width: 100%; border: 0;background-color: #f0f8ff;vertical-align:middle;text-align:center;" cellspacing="1" cellpadding="1" width="100%" align="center" border="0">
                  <tr>
					<td style="text-align:center;vertical-align:bottom;" align="center" colspan="2">
                        <asp:Image ID="imgLogo" runat="server" ImageAlign="middle" ImageUrl="./images/customerlogo.gif" Width="200" />
					</td>
				</tr>
                <tr>
				    <td style="vertical-align:middle;text-align:center;" align="center" colspan="2">
					    <table id="Table2x" style="width: 400px; border: 0px" cellspacing="1" cellpadding="1" align="center" border="0">
                          
							<tr>
							    <td colspan="2">
                                    <img height="20" src="./images/spacer.gif" width="1" alt="" />
							    </td>
					        </tr>
                            <tr>
							    <td style="white-space:nowrap;text-align:center;vertical-align:middle;" valign="middle" align="center" colspan="2">
								    <asp:label id="LabelControlCenterTitle" runat="server" Font-Bold="True" Font-Size="Large">WebCenter PDFHUB</asp:label>
                                </td>
					        </tr>
					        <tr>
				                <td style="text-align:center;vertical-align:middle;" align="center" colspan="2">
					                <asp:label id="lblLoginHeader" runat="server" Font-Bold="True" Font-Size="Large">User login</asp:label>
                                </td>
							</tr>
                            <tr>
					            <td colspan="2">
                                    <img height="20" src="./images/spacer.gif" width="1" alt="" />
                                </td>
						    </tr>
                              <tr>
						        <td style="width: 105px; height: 16px" align="left" width="105">
						            <asp:label id="lblLanguage" runat="server" Font-Bold="True">Language</asp:label>
                                </td>
						        <td align="left">
                                    <asp:DropDownList ID="ddLanguage" runat="server" Height="27" Width="138px" AutoPostBack="True" OnSelectedIndexChanged="ddLanguage_SelectedIndexChanged">
                                        <asp:ListItem Selected="True" Value="en">English</asp:ListItem>
                                        <asp:ListItem Value="da">Danish</asp:ListItem>
                                        <asp:ListItem Value="no">Norwegian</asp:ListItem>
                                        <asp:ListItem Value="sw">Swedish</asp:ListItem>
                                        <asp:ListItem Value="ge">German</asp:ListItem>
                                        <asp:ListItem Value="fr">French</asp:ListItem>
                                        <asp:ListItem Value="ko">Korean</asp:ListItem>
                                        <asp:ListItem Value="ch">Chinese</asp:ListItem>
                                    </asp:DropDownList>
						            
                                </td>
							</tr>
                            <tr>
						        <td style="width: 105px; height: 16px" align="left" width="105">
						            <asp:label id="lblDomain" runat="server" Font-Bold="True">Domain</asp:label>
                                </td>
						        <td align="left">
						            <asp:textbox id="txtDomain" runat="server" Height="27" Width="138px"></asp:textbox>
                                </td>
							</tr>
						    <tr>
						        <td style="width: 125px; height: 16px" align="left" width="105">
						            <asp:label id="lblUsername" runat="server" Font-Bold="True">Username</asp:label>
                                </td>
						        <td align="left">
						            <asp:textbox id="txtUserName" runat="server" Height="27" Width="138px"></asp:textbox>
                                </td>
							</tr>
						    <tr>
					            <td style="width: 170px; height: 16px" align="left" width="105">
                                    <asp:label id="lblPassword" runat="server" Font-Bold="True">Password</asp:label>
                                </td>
						        <td align="left">
                                    <asp:textbox id="txtPassword" runat="server" Width="138px" Height="27px" TextMode="Password"></asp:textbox>
                                </td>
						    </tr>
				            <tr>
						        <td align="center" colspan="2">
                                    <asp:requiredfieldvalidator id="RequiredFieldValidator1" runat="server" ErrorMessage="You must supply a password"
													Display="Dynamic" ControlToValidate="txtPassword" ></asp:requiredfieldvalidator>
                                    <img height="10" src="./images/spacer.gif" width="1" alt=""/>
                                </td>
							</tr>
				            <tr>
				                <td align="center" colspan="2">
						            <telerik:RadButton id="bntLogin" runat="server" Width="100px" Text="Login"
													EnableViewState="False" Skin="Office2010Blue" OnClick="bntLogin_Click"></telerik:RadButton>
                                </td>
						    </tr>
							<tr>
								<td  align="left">
									<asp:hyperlink id="Hyperlink1" runat="server" NavigateUrl="ChangePassword.aspx?Reg=0" Target="_self"
										Visible="False" CssClass="Link">Contact Admin</asp:hyperlink>
                                </td>
								<td align="right"><asp:hyperlink id="Hyperlink2" runat="server" NavigateUrl="ChangePassword.aspx?Reg=0" Target="_self"
										Visible="False" CssClass="Link">Change password</asp:hyperlink><img height="1" src="./images/spacer.gif" width="40" alt="" />
                                </td>
							</tr>
							<tr>
								<td align="center" colspan="2">
                                    <asp:label id="lblStatus" runat="server" Font-Bold="True" Width="346px" ForeColor="Red"></asp:label>
                                </td>
							</tr>
							<tr>
                                <td colspan="2">
                                    <img height="20" src="./images/spacer.gif" width="1" alt="" />
                                </td>											
							</tr>
							
							<tr>
								<td align="center" colspan="2" height="12">
									<asp:hyperlink id="lnkNotes" runat="server" EnableViewState="False" Target="_self" NavigateUrl="Manual.pdf"
										ToolTip="Download instructions (PDF)" CssClass="Link">Download instructions</asp:hyperlink>
                                </td>
							</tr>
                                <tr>
                                <td colspan="2">
                                    <img height="20" src="./images/spacer.gif" width="1" alt="" />
                                </td>											
							</tr>
                                <tr>
                                <td align="center" colspan="2">
                                    <img id="InfraLogicLogo" height="59" alt="" src="./images/logoweb.gif" align="middle" />
                                </td>
							</tr>
                                <tr>
                                <td colspan="2">
                                    <img height="10" src="./images/spacer.gif" width="1" alt="" />
                                </td>			
                                    </tr>
                                <tr>
                                <td style="text-align:center;" align="center" colspan="2">
									<asp:Label id="lblDate" runat="server" Font-Size="Smaller"></asp:Label>				
                                </td>				
							</tr>
						</table>
                    </td>
                </tr>
            </table>
								
            <input runat="server" id="HiddenX" type="hidden" value="" />
            <input runat="server" id="HiddenY" type="hidden" value="" />
            <input runat="server" id="HiddenIOS" type="hidden" value="" />
            <input runat="server" id="HiddenLang" type="hidden" value="" />
            <script type="text/javascript">

                function setCookie(name, value, days) {
                    var expires = "";
                    if (days) {
                        var date = new Date();
                        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
                        expires = "; expires=" + date.toUTCString();
                    }
                    document.cookie = escape(name) + "=" + (escape(value) || "") + escape(expires )+ "; path=/";
                }

                function getCookie(name) {
                    var nameEQ = name + "=";
                    var ca = document.cookie.split(';');
                    for (var i = 0; i < ca.length; i++) {
                        var c = ca[i];
                        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
                        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
                    }
                    return null;
                }

                function clearCookie(name, domain, path) {
                    var domain = domain || document.domain;
                    var path = path || "/";
                    document.cookie = escape(name) + "=; expires=" + new Date + "; domain=" + escape(domain) + "; path=" + escape(path);
                };

                function DOMCall(name) {
                    if (document.layers) //checks document.layers  
                        return document.layers[name];
                    else if (document.all) //checks document.all  
                        return document.all[name];
                    else if (document.getElementById) //checks getElementById  
                        return document.getElementById(name);
                }

                var lg = DOMCall('HiddenLang');
                if (lg != null) {
                    var lgc = getCookie('WebCenterLanguage');
                    if (lgc != null)
                        if (lgc != "")
                            lg.value = lgc;

                    if (lg.value != "")
                        setCookie("WebCenterLanguage", lg.value, 100);
                }


                var appletHeight;
                var appletWidth;
                var isMac = 0;
                var isNav = 0;
                var isIOS = 0;
               // alert(navigator.userAgent.toLowerCase());
                if (navigator.userAgent.toLowerCase().indexOf("ipad") > 0 || navigator.userAgent.toLowerCase().indexOf("iphone") > 0)
                    isIOS = 1

                if (navigator.userAgent.toLowerCase().indexOf("mac_") > 0) {
                    appletWidth = document.body.clientWidth - 5;
                    appletHeight = document.body.clientHeight - 40;
                    isMac = 1;
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
                    appletWidth = window.innerWidth - 5;
                    appletHeight = window.innerHeight - 20;
                    isMac = 1;
                } else if ((navigator.userAgent.toLowerCase().indexOf("safari") > 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla") > 0)) {
                    //This is the only way we know how to set WIDTH and HEIGHT for an applet on a Mac
                    appletWidth = window.innerWidth - 20;
                    appletHeight = window.innerHeight - 25;
                    isMac = 1;
                } else if (navigator.userAgent.toLowerCase().indexOf("netscape6") > 0) {
                    //This is the only way we know how to set WIDTH and HEIGHT for an applet on a Mac
                    appletWidth = window.innerWidth - 25;
                    appletHeight = window.innerHeight - 35;
                    isNav = 1;
                } else if ((window.opera) || (document.all && (!(document.compatMode && document.compatMode == "CSS1Compat")))) {
                    appletHeight = document.body.clientHeight - 30;
                    appletWidth = document.body.clientWidth - 5;
                } else {
                    //Netscape percents do not work on applets inside tables so we need to work out the size.
                    appletHeight = window.innerHeight - 35;
                    appletWidth = window.innerWidth - 25;
                    isNav = 1;
                }

                var today = new Date();
                var expire = new Date();
                var y = 0;
                expire.setTime(today.getTime() + 1000 * 60 * 60 * 24);

                var heightextra = 0;
                if (isMac || isNav)
                    heightextra = 10;

                heightextra += 28;

                var olddate = new Date();
                olddate.setDate(olddate.getDate() - 1);

                document.cookie = "ScreenHeight=" + escape(appletHeight - heightextra) + ((expire == null) ? "" : ("; expires=" + expire.toGMTString()));
                document.cookie = "ScreenWidth=" + escape(appletWidth - 210) + ((expire == null) ? "" : ("; expires=" + expire.toGMTString()));
                document.cookie = "IsMac=" + escape(isMac) + ((expire == null) ? "" : ("; expires=" + expire.toGMTString()));

                clearCookie("ScrollY", "", "");
                clearCookie("ScrollReadviewY", "", "");
                clearCookie("ScrollFlatY", "", "");

                var n = DOMCall('HiddenX');
                var m = DOMCall('HiddenY');
                var k = DOMCall('HiddenIOS');

                if (n && m && appletHeight > 0 && appletWidth > 0) {
                    n.value = appletHeight;
                    m.value = appletWidth;
                }
                if (k)
                    k.value = isIOS;

            
            </script>                   
        </form>
	</body>
</html>
