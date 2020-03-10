Saved<%@ Page language="c#" Codebehind="UploadFiles.aspx.cs" AutoEventWireup="true" Inherits="WebCenter4.Views.UploadFiles" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %><!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" ><html><heaUpload files</title><link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" /><script type="text/javascript" src="../Scripts/jquery-1.11.3.min.js"></script><telerik:RadCodeBlock ID="RadCodeBlock" runat="server">
		<script type="text/javascript">
			function doClose()
			{
				<% if (doClose>0)  { %>
					parent.window.close();
				<% } %>		
			}

		    var uploadedfileCount = 0;

		    function OnClientSelectingFiles(sender, args) {
		       /* if (uploadedfileCount == 0) {
		           if (args.get_count() > 16) {
		                args.set_cancel(true);

		                alert("A maximum of 16 files can be selected in one upload sequence");
		            }   
		        }
		        else if (args.get_count() > (16 - uploadedfileCount)) {
		            args.set_cancel(true);
		        }    */
		    }

		   

		    function OnClientFileUploadFailed(sender, args) {
		        if (args.get_message() == "error") {
		            args.set_handled(true);
		        }
		    }

		    function OnClientFileUploaded (radAsyncUpload, args) {
		        var contentType = args.get_fileInfo().ContentType;

		        uploadedfileCount = radAsyncUpload._uploadedFiles.length;
		     
		        if (contentType == null)
		            return;
		        var row = args.get_row();
		        if (row == null)
		            return;

		        var filename = args.get_fileName();
		
		       
		        var inputName = radAsyncUpload.getAdditionalFieldID("TextBox");
		        if (inputName == null)
		            return;

		     
		        var inputType = "text";
		        var inputID = inputName;
		       
		        var input = document.createElement('input');
		        input.id = inputID;
		        input.type = 'text';
		        input.name = inputName;
		        input.value = filename;

		        //var input = createInput('text', inputID, inputName, filename);
		        if (input == null)
		            return;
	
		        var label = createLabel(inputID);
		        if (label == null)
		            return;
		
		        var br = document.createElement("br");
		        if (br == null)
		            return;
		        row.appendChild(br);
		        row.appendChild(label);
		        row.appendChild(input);
		<
		    }

		  


		    function createInput(inputType, inputID, inputName,value) {
		        var input = document.createElement("input");
		        input.setAttribute("type", inputType);
		        input.setAttribute("id", inputID);
		        input.setAttribute("name", inputName);
		        input.setAttribute("value", value);
		        input.style.setAttribute("width", "200px");
		        return input;
		    }

		    function createLabel(forArrt) {
		        var label = document.createElement("label");
		        label.setAttribute("for", forArrt);
		        label.innerHTML = "Final name: ";
		        return label;
		    }

		    function submitPage() {
		        var uploadingRows = $(".RadAsyncUpload").find(".ruUploadProgress");
		        for (var i = 0; i < uploadingRows.length; i++) {
		            if (!$(uploadingRows[i]).hasClass("ruUploadCancelled") && !$(uploadingRows[i]).hasClass("ruUploadFailure") && !$(uploadingRows[i]).hasClass("ruUploadSuccess")) {
		                alert("you could not submit the page during upload :)");
		                return
		            }
		        }

		        theForm.submit();
		    }

		  </script>
        </telerik:RadCodeBlock>
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
	<body onload="doClose()">
		<form id="Form1" method="post" encType="multipart/form-data" runat="server">
            <telerik:RadScriptManager ID="ScriptManager1" runat="server" EnableTheming="True">
                <Scripts>
                    <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js">
                    </asp:ScriptReference>
                    <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js">
                    </asp:ScriptReference>
                    <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js">
                    </asp:ScriptReference>
                </Scripts>
            </telerik:RadScriptManager>
            <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;" SingleClick="None" CssClass="smallToolBar" >
                <Items>
                    <telerik:RadToolBarButton runat="server" Value="Item1">
                        <ItemTemplate>
                            <asp:Label ID="LabelUploadFiles" runat="server" Text="Upload files to server" CssClass="RadToolbarLabel"></asp:Label>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>
                </Items>
            </telerik:RadToolBar>                        
            <div style="overflow-y: scroll;max-height:400px;">
                <div style="margin-top:10px;">
                    <telerik:RadAsyncUpload ID="RadAsyncUpload1" runat="server"  MultipleFileSelection="Automatic" 
                        TemporaryFolder="~/UploadedFiles" TargetFolder="~/UploadedFiles/Saved" 
                        OverwriteExistingFiles="true" EnableViewState="true"   
                        Skin="Office2010Blue" OnClientFileUploaded="OnClientFileUploaded" OnClientFileUploadFailed="OnClientFileUploadFailed" 
                        OnClientFilesSelected="OnClientSelectingFiles"
                        Localization-Select="Browse" PostbackTriggers="RadButton1" ToolTip="Select file(s) to upload and optionally rename" MaxFileInputsCount="16" ></telerik:RadAsyncUpload>
                </div>
                <div style="margin-top:10px;">
                    <telerik:RadButton id="RadButton1" runat="server"  Width="80px" EnableViewState="True" Text="Save file(s)" Skin="Office2010Blue" OnClick="RadButton_Click" ></telerik:RadButton>
                </div>
            </div>
            <div class="uploadedfiles">
               <asp:Literal runat="server" ID="ltrNoResults" Visible="True" Text="<strong>No files uploaded</strong>" />
                <asp:GridView ID="GridView1" runat="server" CellPadding="4" ForeColor="#333333" GridLines="Both" AutoGenerateColumns="False" Height="300px">
                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                     <Columns>
                        <asp:BoundField DataField="FileName" HeaderText="FileName" ReadOnly="True">
                        <ItemStyle Width="250px" Wrap="False" />
                        </asp:BoundField>
                        <asp:BoundField DataField="Size" HeaderText="Size" ReadOnly="True" />
                        <asp:BoundField DataField="Status" HeaderText="Status" ReadOnly="True" />
                    </Columns>
                    <EditRowStyle BackColor="#999999" />
                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" Font-Names="Tahoma" Font-Size="Small" />
                    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" Font-Names="Tahoma" Font-Size="Small"/>
                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" Font-Names="Tahoma" Font-Size="Small" />
                    <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                    <SortedAscendingCellStyle BackColor="#E9E7E2" />
                    <SortedAscendingHeaderStyle BackColor="#506C8C" />
                    <SortedDescendingCellStyle BackColor="#FFFDF8" />
                    <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                </asp:GridView>
            </div>
            <div style="text-align:center;">                
                <telerik:RadButton id="btnClose" runat="server"   Width="60px" EnableViewState="True" Text="Close" Skin="Office2010Blue" OnClick="btnClose_Click" Enabled="false"></telerik:RadButton>
            </div>
            <input runat="server" type="hidden" id="hiddenUploadPath" value="" />
		</form>
	</body>
</html>
