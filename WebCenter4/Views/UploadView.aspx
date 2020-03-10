<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadView.aspx.cs" Inherits="WebCenter4.Views.UploadView" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title></title>
        <link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet"/>

        <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
            
            <script type="text/javascript" src="../Scripts/jquery-1.11.3.min.js"></script>
            <script  type="text/javascript">

    	        var appletHeight = 0;
    	        var appletWidth = 0;
    	        var scrollY = 0;
    	        var dropZones = [];

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
    	        

    	        function ConstructFileName(filename, pagenumber, extension) {

    	            var filenamefinal = filename;
    	            var checkBox = $find("<%= RadCheckBoxUseNameRule.ClientID %>");
    	            
    	            if (checkBox.get_checked() == false)
    	                return filenamefinal;    	                               
    	            
    	            var pubdropdownlist = $find("<%= ddPublicationList.ClientID %>");
    	            if (pubdropdownlist == null)
    	                return filenamefinal;

    	            var eddropdownlist = $find("<%= ddEditionList.ClientID %>");
    	            if (eddropdownlist == null)
    	                return filenamefinal;

    	            var secdropdownlist = $find("<%= ddSectionList.ClientID %>");
    	            if (secdropdownlist == null)
    	                return filenamefinal;

    	            var datepicker = $find("<%= dateChooserPubDate.ClientID %>");
    	            if (datepicker == null)
    	                return filenamefinal;

    	            var checkboxfulldocument = $find("<%= cbFullDocument.ClientID %>");
    	            if (checkboxfulldocument == null)
    	                return filenamefinal;

    	            var pubstr = pubdropdownlist.get_selectedItem().get_text();
    	            var ii = pubstr.indexOf("[");
    	            if (ii != -1) {
    	                var jj = pubstr.indexOf("]");
    	                if (jj != -1) {
    	                    pubstr = pubstr.substring(ii+1,jj)
    	                }

    	            }

    	            var edstr = eddropdownlist.get_selectedItem().get_text();
    	            var secstr = secdropdownlist.get_selectedItem().get_text();

    	            var date = datepicker.get_selectedDate();
                    if (date == null)
                        return filenamefinal;
    	            //  var datestr = date.getFullYear() + pad(date.getMonth()) + pad(date.getDate());
                    var datestr = pad(date.getDate()) + pad(date.getMonth()+1) + pad(date.getFullYear() - 2000);

                    var finalname;

                    if (<%= customuploadNameFormat %> == 2) {
                        datestr = pad(date.getFullYear() - 2000) + pad(date.getMonth()+1) + pad(date.getDate());

                        if (checkboxfulldocument.get_checked())
                            finalname = pubstr + "-" + datestr + "_" + edstr +  + secstr + "." + extension;
                        else
                            finalname = pubstr + "-" + datestr + "-" +edstr + "-" + secstr + "-" + pagenumber + "." + extension;

                    } else {
                        if (checkboxfulldocument.get_checked())
                            finalname = pubstr + "_UPL_T_" + datestr + "_" + edstr + "_1_" + secstr + "." + extension;
                        else
                            finalname = pubstr + "_UPL_T_" + datestr + "_" +edstr + "_1_" + secstr + "_" + pagenumber + "." + extension;
                    }
                    return finalname;

    	        } 

                function pad(num) {
                    num = num + '';
                    return num.length < 2 ? '0' + num : num;
                }


    	        function RegexGetPageNumber(filename) {
    	            var re = /.*(\d{3})\..*/;
    	            var OK = re.exec(filename);
    	            if (OK)
    	                return filename.replace(re, "$1");

    	            re = /.*(\d{2})\..*/;
    	            OK = re.exec(filename);
    	            if (OK)
    	                return filename.replace(re, "$1");
    	            re = /.*(\d+)\..*/;
    	            return filename.replace(re, "$1");
    	        }
    	        function RegexGetExtension(filename) {
    	            var re = /.*\.(.*)/;
    	            return filename.replace(re, "$1");
    	        }


    	        function OnClientFileUploaded(radAsyncUpload, args) {
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
    	            if (input == null)
    	                return;
    	            input.id = inputID;
    	            input.type = 'text';
    	            input.name = inputName;
    	            input.value = ConstructFileName(filename, RegexGetPageNumber(filename), RegexGetExtension(filename));
    	            input.style.width = '250px'
    	            //var input = createInput('text', inputID, inputName, filename);

    	            var label = createLabel(inputID);
    	            if (label == null)
    	                return;

    	            var pageinputName = radAsyncUpload.getAdditionalFieldID("TextBox");
    	            if (pageinputName == null)
    	                return;
    	            var pageinputType = "text";
    	            var pageinputID = pageinputName;
    	            var pageinput = document.createElement('input');
    	            if (pageinput == null)
    	                return;
    	            pageinput.id = pageinputID;
    	            pageinput.type = 'text';
    	            pageinput.name = pageinputName;
    	            pageinput.value = RegexGetPageNumber(filename);
    	            pageinput.style.width = '20px'

    	            var checkboxfulldocument = $find("<%= cbFullDocument.ClientID %>");
    	            if (checkboxfulldocument != null) {
    	                if (checkboxfulldocument.get_checked())
    	                    pageinput.value = '';
    	            }

    	            var pagelabel = createPageNumberLabel(pageinputID);
    	            if (pagelabel == null)
    	                return;


//    	            var br = document.createElement("br");
  //  	            if (br == null)
    //	                return;

    	//            row.appendChild(br);

    	            var br = document.createElement("br");
    	            if (br == null)
    	                return;
    	            row.appendChild(br);

    	            row.appendChild(pagelabel);
    	            row.appendChild(pageinput);

    	            
    	            row.appendChild(label);
    	            row.appendChild(input);
                }

    	        function createInput(inputType, inputID, inputName, value) {
    	            var input = document.createElement("input");
    	            input.setAttribute("type", inputType);
    	            input.setAttribute("id", inputID);
    	            input.setAttribute("name", inputName);
    	            input.setAttribute("value", value);
    	            input.style.setAttribute("width", "240px");
    	            return input;
    	        }

    	        function createLabel(forArrt) {
    	            var label = document.createElement("label");
    	            label.setAttribute("for", forArrt);
    	            label.innerHTML = "  Final name: ";
    	            return label;
    	        }
    	        function createPageNumberLabel(forArrt) {
    	            var label = document.createElement("label");
    	            label.setAttribute("for", forArrt);
    	            label.innerHTML = "Page number: ";
    	            return label;
    	        }

    	        function onClientFileUploading(sender, args) {
    	            var obj = { first: 1, second: 2 };
    	            args.set_queryStringParams(obj);
    	        }


    	        function InitializtDnD(pagecount) {
    	            $ = $telerik.$;

    	            if (Telerik.Web.UI.RadAsyncUpload.Modules.FileApi.isAvailable()) {

    	                $(document).bind({ "drop": function (e) { e.preventDefault(); } });

    	                for (var i = 1; i < pagecount; i++) {
    	                    if (i == 0)
    	                        dropZones[0] = $(document).find(".DropZone1");
                            else
    	                        dropZones[i] = $(document).find("#DropZone"+ (i+1).toString());

    	                    dropZones[i].bind({ "dragenter": function (e) { dragEnterHandler(e, dropZones[i]); } })
                                     .bind({ "dragleave": function (e) { dragLeaveHandler(e, dropZones[i]); } })
                                     .bind({ "drop": function (e) { dropHandler(e, dropZones[i]); } });
                        }
    	            }
    	        }

    	        function dropHandler(e, dropZone) {
    	            dropZone[0].style.backgroundColor = "#357A2B";
    	        }

    	        function dragEnterHandler(e, dropZone) {
    	            var dt = e.originalEvent.dataTransfer;
    	            var isFile = (dt.types != null && (dt.types.indexOf ? dt.types.indexOf('Files') != -1 : dt.types.contains('application/x-moz-file')));
    	            if (isFile || $telerik.isSafari5 || $telerik.isIE10Mode || $telerik.isOpera)
    	                dropZone[0].style.backgroundColor = "#000000";
    	        }

    	        function dragLeaveHandler(e, dropZone) {
    	            if (!$telerik.isMouseOverElement(dropZone[0], e.originalEvent))
    	                dropZone[0].style.backgroundColor = "#357A2B";
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
                margin: 0px;  
                padding: 0px;  
                overflow: hidden;  
            }  

            .DropZone1 {
                width: 150px;
                height: 90px;
                background-color: #357A2B;
                border-color: #CCCCCC;
                color: #767676;
                float: left;
                text-align: center;
                font-size: 16px;
                color: white;
            }

            #DropZone2 {
                width: 150px;
                height: 90px;
                background-color: #357A2B;
                border-color: #CCCCCC;
                color: #767676;
                float: left;
                text-align: center;
                font-size: 16px;
                color: white;
            }

            .RadAsyncUpload {
                 margin-left: 20px;
                 margin-bottom: 20px;
            }

            .RadUpload .ruUploadProgress {
                width: 250px;
                display: inline-block;
                overflow: hidden;
                text-overflow: ellipsis;
            }

            div.RadUpload .ruBrowse {
                background-position: 0 -46px;
                width: 115px !important;
            }

            html div.RadUpload .ruFileWrap .ruButtonHover.ruButtonFocus,
            html div.RadUpload .ruFileWrap .ruButtonFocus,
            html div.RadUpload .ruFileWrap .ruButtonHover {
                background-position: 100% -46px;
            }

            
        </style> 
    </head>
    <body onresize="GetBrowserDim()">
	    <form id="Form1" method="post" enctype="multipart/form-data" runat="server">			
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
			
             <telerik:RadToolBar ID="RadToolBar1" Runat="server" Skin="Office2010Blue" style="width: 100%;" CssClass="smallToolBar">
                <Items>
                    <telerik:RadToolBarButton runat="server" Value="Item1">
                        <ItemTemplate>
                            <div style="height:22px;margin-top:6px;">
                                <asp:Label ID="LabelUploadHeader" runat="server" Text="Upload files" CssClass="RadToolbarLabel" Height="22"></asp:Label>
                            </div>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>
                </Items>
            </telerik:RadToolBar>
            <div id="mainDiv" style="overflow: Auto;background-color: #f0f8ff;width: 100%;">
                 <div style="margin-top:10px; margin-left:20px">
                    <table>
                        <tr>                           
                        <td style="width: 167px; height: 14px; white-space:nowrap;">
                            <telerik:RadCheckBox ID="RadCheckBoxUseNameRule" runat="server" Text="Use naming rule"  OnClick="RadCheckBoxUseNameRule_Click"></telerik:RadCheckBox>
                        </td>		    							
						<td style="white-space:nowrap;">										
                         </td>
                        </tr>
                        <tr>                           
                        <td style="width: 167px; height: 14px; white-space:nowrap;">
	    					<asp:Label id="lblPublication" runat="server" CssClass="LabelNormal">Publication</asp:Label>
                        </td>		    							
						<td style="white-space:nowrap;">										
                            <telerik:RadComboBox id="ddPublicationList" runat="server" Width="180px" AutoPostBack="True"  Skin="Default" OnSelectedIndexChanged="ddPublicationList_SelectedIndexChanged"></telerik:RadComboBox>	    									
                        </td>
                        </tr>
                        <tr>
                        <td style="width: 167px; white-space:nowrap;">
							<asp:Label id="lblPubdate" runat="server" CssClass="LabelNormal">Publication date</asp:Label>
                        </td>
                        <td style="white-space:nowrap;">
							<telerik:RadDatePicker id="dateChooserPubDate" ToolTip="Select publication date" Runat="server" Culture="da-DK"></telerik:RadDatePicker>
                        </td> 
                        </tr>
                        <tr>
                          <td style="width: 167px; height: 14px; white-space:nowrap;">
	    					<asp:Label id="lblEdition" runat="server" CssClass="LabelNormal">Edition</asp:Label>
                        </td>		    							
						<td style="white-space:nowrap;">										
                            <telerik:RadComboBox id="ddEditionList" runat="server" Width="180px" AutoPostBack="True"  Skin="Default"></telerik:RadComboBox>	    									
                        </td>
                       </tr>
                        <tr>
                          <td style="width: 167px; height: 14px; white-space:nowrap;">
	    					<asp:Label id="lblSection" runat="server" CssClass="LabelNormal">Section</asp:Label>
                        </td>		    							
						<td style="white-space:nowrap;">										
                            <telerik:RadComboBox id="ddSectionList" runat="server" Width="180px" AutoPostBack="True"  Skin="Default"></telerik:RadComboBox>	    									
                        </td>
                        </tr>
                        <tr>
                          <td style="width: 167px; height: 14px; white-space:nowrap;">
	    					<asp:Label id="lbl" runat="server" CssClass="LabelNormal">Document type</asp:Label>
                        </td>		    							
						<td style="white-space:nowrap;">	
                            <telerik:RadCheckBox  ID="cbFullDocument" runat="server" Text="Full document" Checked="false"></telerik:RadCheckBox>									
                        </td>
                        </tr>
                    </table>

                </div>
                <div style="overflow-y: scroll;max-height:500px;">
                <div style="margin-top:10px;">
                    <telerik:RadAsyncUpload ID="RadAsyncUpload1" runat="server"  MultipleFileSelection="Automatic" TemporaryFolder="~/UploadedFiles" TargetFolder="~/UploadedFiles/Saved"  EnableViewState="true"   Skin="Office2010Blue" OnClientFileUploaded="OnClientFileUploaded"  PostbackTriggers="RadButton1" Localization-Select="Browse" HideFileInput="true" ToolTip="Select file(s) to upload and optionally rename"></telerik:RadAsyncUpload>
                </div>
                <div style="margin-top:10px; margin-left:20px">
                    <telerik:RadButton id="RadButton1" runat="server"  Width="115px" EnableViewState="True" Text="Save file(s)" Skin="Office2010Blue" OnClick="RadButton_Click" ></telerik:RadButton>
                </div>
                </div>
                
		    </div>
            <div class="uploadedfiles" style="overflow: Auto;background-color: #f0f8ff;width: 100%;">
               <asp:Literal runat="server" ID="ltrNoResults" Visible="True" Text="<strong>No files uploaded</strong>" />
                <asp:GridView ID="GridView1" runat="server" CellPadding="4" ForeColor="#333333" AutoGenerateColumns="False" PageSize="100">
                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    <Columns>
                        <asp:BoundField DataField="FileName" HeaderText="FileName" ReadOnly="True">
                        <ItemStyle Width="120px" Wrap="False" />
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
		    <div style="text-align: center;">
                    <asp:label id="lblError" runat="server" ForeColor="Red" Font-Bold="True"></asp:label>
                    <asp:textbox id="txtReturnedFromPriority" runat="server" ForeColor="Transparent" BorderStyle="None"
					    BackColor="Transparent" Height="2px" BorderColor="Transparent"></asp:textbox>
                    <asp:textbox id="txtReturnedFromTemplate" runat="server" ForeColor="Transparent" BorderStyle="None"
					    BackColor="Transparent" Height="2px" BorderColor="Transparent"></asp:textbox>
		    </div>
            <input runat="server" id="HiddenX" type="hidden" value="" />
            <input runat="server" id="HiddenY" type="hidden" value="" />
            <input runat="server" id="HiddenScrollPos" type="hidden" value="" />
            <input runat="server" type="hidden" id="hiddenUploadPath" value="" />

            <telerik:RadCodeBlock ID="RadCodeBlock3" runat="server">
			    <script  type="text/javascript">
			        GetBrowserDim();
			        SaveWindowSize();
			    //    InitializtDnD(6);
			    </script>
            </telerik:RadCodeBlock>
	    </form>
    </body>
</html>
