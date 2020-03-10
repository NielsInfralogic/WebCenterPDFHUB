<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MobileView.aspx.cs" Inherits="WebCenter4.Views.MobileView" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Mobile view</title>
    <link href="../Style/WebCenter.css" type="text/css" rel="stylesheet" />
    <style type="text/css">  
            html, body, form  
            {  
                height: 100%;  
                margin: 0px;  
                padding: 0px;  
                overflow: hidden;  
                align-content: center;
            }  
        </style> 
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label id="Label1" runat="server"></asp:Label>
    </div>
    </form>
</body>
</html>
