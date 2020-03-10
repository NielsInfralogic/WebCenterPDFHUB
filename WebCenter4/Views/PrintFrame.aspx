<%@ Page language="c#" Codebehind="PrintFrame.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.Views.PrintFrame" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>PrintFrame</title>
	</head>
	<body>
		<script language="JavaScript" type="text/javascript">
			var w = null;
			if (w && !w.closed)
				w.close();
				
			w = open ('', 'imagePrint', 'menubar=1,locationbar=0,statusbar=0,resizable=1,scrollbars=1,width=50,height=50');
			var html = '';
			html += '<html><body ONLOAD="if (window.print) window.print(); '
					+ 'setTimeout(\'window.close();\', 10000);">';
			html += '<img SRC="<%= imagePath %>">';
			html += '<\/BODY>;<\/HTML>';
			w.document.open();
			w.document.write(html);
			w.document.close();
		</script>
		
	</body>
</html>
