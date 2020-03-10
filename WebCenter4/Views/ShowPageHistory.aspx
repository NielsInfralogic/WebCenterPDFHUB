<%@ Page language="c#" Codebehind="ShowPageHistory.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.ShowPageHistory" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Page history</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
		<script language="JavaScript" type="text/javascript">
			function doClose()
			{
				 if (<%=doClose %> > 0)  { 
				     opener.document.Form1.HiddenReturendFromPopup.value = '1';
					opener.document.Form1.submit();				
					parent.window.close();
			    }
			
			}
		</script>
		<style type="text/css">.toolbarHeader { WIDTH: 400px }
		</style>
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
	<body onload="doClose()" >
		<form id="Form1" method="post" runat="server">

            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <div style="width: 100%; display: block; float: none;">                            
                <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;">
                    <Items>
                        <telerik:RadToolBarButton runat="server" Value="Item1">
                            <ItemTemplate>
                                <asp:Label ID="LabelPageHistoryHeader" runat="server" Text="Page history" CssClass="RadToolbarLabel"></asp:Label>
                            </ItemTemplate>
                        </telerik:RadToolBarButton>
                    </Items>
                </telerik:RadToolBar>                        
			</div>
             <div>
                 <asp:label id="lblHeader" runat="server" CssClass="header">Page History</asp:label>
             </div>
			<div style="height: 308px; width: 604px; text-align: left; vertical-align: top;">
                    <telerik:RadGrid ID="RadGridPageHistory" runat="server" Skin="Office2010Blue" Width="600px" Height="300px">
                        <ClientSettings>
                            <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                        </ClientSettings>
                    </telerik:RadGrid>
            </div>
                <div style="height:14px;">
					<asp:label id="lblError" runat="server" ForeColor="Red"></asp:label>
                </div>
            <input runat="server" id="hiddenMasterCopySeparationSet" type="hidden" value="0" />
		</form>
	</body>
</html>
