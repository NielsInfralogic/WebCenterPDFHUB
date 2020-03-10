<%@ Page language="c#" Codebehind="PDFview.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.PDFview" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>PDFview</title>
		<link href="../Styles/WebCenter.css" type="text/css" rel="stylesheet" />
        <script type="text/javascript">
            var isSafari = 0;
            if ((navigator.userAgent.toLowerCase().indexOf("safari") > 0) && (navigator.userAgent.toLowerCase().indexOf("ozilla") > 0))
                isSafari = 1;
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
     <meta name="viewport" content="width=device-width; initial-scale=1.0;" /> 
	</head>
	<body>
		<form id="Form1" method="post" runat="server">            
            <object  id="myPdf" type="application/pdf" data="<%=pdfDoc%>#pagemode=none&amp;toolbar=0&amp;statusbar=0&amp;navpanes=0&amp;scrollbar=1&amp;page=1&amp;view=Fit" height="100%" width="100%"></object> 
		</form>
	</body>
</html>
