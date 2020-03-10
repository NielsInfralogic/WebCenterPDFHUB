<%@ Page language="c#" Codebehind="UploadPlan.aspx.cs" AutoEventWireup="true" Inherits="WebCenter4.Views.UploaPlan" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>Upload plan file</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
        <script src="../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
        <script src="../Scripts/jquery.MultiFile.js" type="text/javascript"></script>
		<script language="JavaScript" type="text/javascript">
		<!--
			var initWidth;
						
			function CloseOnReload()
			{
				GetRadWindow().Close();
			}
			
			function RefreshParentPage() 
			{			
				GetRadWindow().BrowserWindow.document.forms[0].submit(); 
				GetRadWindow().Close();
			}
			
			function SizeToFit()
			{
				var oWnd;
				var theTop;
				var theLeft;
				
				window.setTimeout(
					function()
					{
						oWnd = GetRadWindow();
					//	if (oWnd.BrowserWindow.initWidth == null)
					//		oWnd.BrowserWindow.initWidth = document.body.scrollWidth+20;
						if (initWidth == null)
							initWidth = document.body.scrollWidth+20;
						//alert(initWidth);
						oWnd.SetSize(initWidth,  document.body.scrollHeight+100);
						
					}, 600);
			}
			   
			function GetRadWindow()
			{
				var oWindow = null;
				if (window.radWindow) oWindow = window.radWindow;
				else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
				return oWindow;
			}

		//-->
        </script>
        <style type="text/css">  
            html, body, form  
            {  
                height: 100%;  
                margin: 2px;  
                padding: 0px;  
                overflow: hidden;  
            }  
            
            .style2
            {
                width: 150px;
                height: 30px;
            }
            .style3
            {
                height: 30px;
            }
            .DropZone1
            {
                width: 300px;
                height: 90px;
                background-color: #357A2B;
                border-color: #CCCCCC;
                color: #767676;
                float: left;
                text-align: center;
                font-size: 16px;
                color: white;
            }
            
            .file-list {
                margin: 20px 0 0 0;
                display: none;
            }
            .file-list ul {
                margin: 10px 0 0 0;
                padding: 0;
                list-style: none;
            }
            .file-list li {
                margin: 10px 0 0 0;
            }

            .RadAsyncUpload
            {
                margin-left: 250px;
                margin-bottom: 28px;
            }

            #FileData {
                width: 332px;
            }

        </style> 
	</head>
	<body>
		<form id="Form1" method="post" runat="server" encType="multipart/form-data" >
              <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
            <TABLE style="WIDTH: 550px" id="Table1" border="0" cellSpacing="1" cellPadding="1" width="550"
				align="left">
				<TR>
					<TD class="style2"><asp:label id="lblTemplate" runat="server" CssClass="HeaderText">Planfile to upload</asp:label></TD>
					<TD class="style3"><INPUT id="FileData" name="FileData" type="file" runat="server"></TD>
				</TR>
				<TR>
					<TD class="style2"><asp:label id="Label1" runat="server" CssClass="HeaderText">Comment</asp:label></TD>
					<TD class="style3"><asp:textbox id="txtFilename" runat="server" Width="252px"></asp:textbox></TD>
				</TR>
				<TR>
					<TD class="style2"><asp:label id="Label2" runat="server" CssClass="HeaderText">Folder</asp:label></TD>
					<TD class="style3"><asp:label id="lblFolder" runat="server"></asp:label></TD>
				</TR>
				<TR>
					<TD colSpan="2" align="center" class="style3">
						<input id="Submit1" name="Submit1" value="Upload" type="submit" runat="server"></TD>
				</TR>
				<TR>
					<TD colSpan="2" class="style3"><asp:label id="lblInfo" runat="server" CssClass="LabelNormal"></asp:label></TD>
				</TR>
				<TR>
					<TD colSpan="2" class="style3"><asp:label id="lblInfo2" runat="server" CssClass="LabelNormal"></asp:label></TD>
				</TR>
				<TR>
					<TD colSpan="2" class="style3" align="center">
                        <telerik:RadButton id="bntApply" runat="server"  Text="Apply" EnableViewState="False"
										Width="70px" OnClick="bntApply_Click" Skin="Office2010Blue"></telerik:RadButton>
					</TD>
				</TR>
			</TABLE>
			<asp:Label id="InjectScript" runat="server"></asp:Label>
		</form>
	</body>
</html>
