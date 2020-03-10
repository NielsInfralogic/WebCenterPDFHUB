<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RenameFiles.aspx.cs" Inherits="WebCenter4.Views.RenameFiles" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Rename files</title>
    <link rel="stylesheet" type="text/css" href="../Style/WebCenter.css" />

    <script  type="text/javascript">
        var initWidth;

            function CloseOnReload() {
        //    GetRadWindow().BrowserWindow.document.Form1.txtReturnedFromPopup.value = '1';
            GetRadWindow().BrowserWindow.document.Form1.submit();
            GetRadWindow().Close();
        }

        function RefreshParentPage() {
         //   GetRadWindow().BrowserWindow.document.Form1.txtReturnedFromPopup.value = '1';
            GetRadWindow().BrowserWindow.document.Form1.submit();
            GetRadWindow().Close();
        }


        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }
	</script>
     <style type="text/css">  
            html, body, form  
            {  
                height: 100%;  
                margin: 5px;  
                padding: 0px;  
                overflow: hidden;  
            }  
            .auto-style1
            {
                width: 224px;
                height: 75px;
            }
            .auto-style2
            {
                height: 75px;
            }
        </style> 

</head>
<body>
  <form id="form1" runat="server">
  <div>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;">
      <Items>
        <telerik:RadToolBarButton runat="server" Value="Item1">
          <ItemTemplate>
            <asp:Label ID="LabelRenameFilesHeader" runat="server" Text="Rename files" CssClass="RadToolbarLabel"></asp:Label>
          </ItemTemplate>
        </telerik:RadToolBarButton>
      </Items>
    </telerik:RadToolBar>

    <table style="width: 600px; border: 0px;" id="Table1" cellspacing="1" cellpadding="1">
	  <tr>
        <td style="vertical-align: top" align="center" colspan="3">
          <telerik:RadGrid ID="RadGridRename" runat="server"></telerik:RadGrid>
                  
        </td>
      </tr>
	  <tr>
		<td style="height: 17px" align="left" colspan="2">
        <asp:label id="lblError" runat="server" ForeColor="Red" Font-Size="10pt"></asp:label>
        <asp:label id="lblInfo" runat="server" ForeColor="LimeGreen" Font-Size="10pt"></asp:label>
        </td>
		<td style="height: 17px" align="center"></td>
	</tr>
	<tr>
	<td style="height: 30px; vertical-align: top;"  align="center" colspan="2" >
		<table style="height: 26px; width: 176px; border: 0px" id="Table2" cellspacing="0" cellpadding="1">
		<tr>
			<td align="center">
            <telerik:RadButton id="btnSave" runat="server"   Width="70px" Text="Save" Skin="Office2010Blue" OnClick="btnSave_Click"
								EnableViewState="False"></telerik:RadButton>
            </td>
			<td align="center">
            <telerik:RadButton id="btnCancel" runat="server"   Width="70px" Text="Cancel" OnClick="btnCancel_Click"
								EnableViewState="False" CausesValidation="False" Skin="Office2010Blue"></telerik:RadButton>
            </td>
		</tr>
		</table>
	</td>
	<td style="height: 30px; vertical-align: top;" align="center">
    	<asp:Label id="InjectScript" runat="server"></asp:Label>
	</td>
	</tr>
</table>
     </div>
    </form>
</body>
</html>
