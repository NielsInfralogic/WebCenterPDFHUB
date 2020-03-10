<%@ Page language="c#" Codebehind="FrameSet.aspx.cs" AutoEventWireup="false" Inherits="WebCenter4.FrameSet" %>
<!DOCTYPE html>

<html>
	<head>
		<title>InfraLogic WebCenter</title>
		<link rel="shortcut icon" href="images/IL.ico" />
		<link rel="icon" href="images/IL.ico" type="image/ico" />
		<link href="Styles/WebCenter.css" type="text/css" rel="stylesheet" />
	</head>
	<frameset id="masterframeset" border="0" framespacing="0" rows="30,*" frameborder="0" >
		<frame name="menu" src="Menu.aspx" frameborder="0" noresize scrolling="no" marginwidth="0" marginheight="0"/>
		<frameset id="mainframeset" border="0" framespacing="0" frameborder="0" cols="210,*" >
			<frame name="tree" src="Tree.aspx" frameborder="0"  scrolling="no"  marginwidth="0" marginheight="0"/>
			<frame name="main" src="Main.aspx" frameborder="0" scrolling="no"  marginwidth="0" marginheight="0"/>
			<noframes>
				<p id="p1">
					This HTML frameset displays multiple Web pages. To view this frameset, use a 
					Web browser that supports HTML 4.0 and later.
				</p>
			</noframes>
		</frameset>
	</frameset>
</html>
