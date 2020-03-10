/* Copyright (c) 2004 PowerUp ASP Limited, All Rights Reserved */

function PowerUpTreeViewScrollSave()
{
	var scroll = new PowerUpTreeField("PowerUpTreeViewScrollData");
	
	if (scroll.exists())
		scroll.setValue(window.document.body.scrollTop);
}

function PowerUpTreeViewScrollLoad()
{
	var scroll = new PowerUpTreeField("PowerUpTreeViewScrollData");
	
	if (scroll.exists())
	{
		scroll = scroll.getValue();
	
		if (scroll != null)
		{
			var y = parseInt(scroll);
			
			if (!isNaN(y))
				window.document.body.scrollTop = y;
		}
	}
}

function PowerUpTreeEventManager()
{
	this.arrays = new Object();
}

PowerUpTreeEventManager.prototype.fire = function(key)
{
	var array = this.arrays[key];
	
	for (var i = 0; i < array.length; i++)
	{
		try
		{
			if (typeof(array[i]) == "function")
				array[i]();
			else
				eval(array[i]);
		}
		catch (e)
		{
		}
	}
}

PowerUpTreeEventManager.prototype.add = function(key, fn)
{
	var array = this.arrays[key];
	
	if (array == null)
	{
		this.arrays[key] = array = new Array();
		
		array[array.length] = window[key];
		
		window[key] = new Function("PowerUpTreeEvents.fire(\"" + key + "\");");
	}
	
	array[array.length] = fn;
}

PowerUpTreeEvents = new PowerUpTreeEventManager();

/* PowerUpTreeImage */

function PowerUpTreeImage(span)
{
	if (span.tagName == "IMG")
		span = span.parentNode;
		
	this.span = span;
}

PowerUpTreeImage.prototype.post = function()
{
	var post = this.getAttribute("POSTBACK");
	
	if (post != "")
		eval(post);
}

PowerUpTreeImage.prototype.setCssClassOver = function()
{
	var css = "";
	
	if (PowerUpTreeViewDragObject != null)
	{
		if (PowerUpTreeViewDragObject.getType() == "TreeImage")
			if (PowerUpTreeViewDragObject.span == this.span)
				return;
			
		css = this.getAttribute("CSSDRAGOVER");
	}
	
	if (css == "")
		css = this.getAttribute("CSSOVER");
	
	if (css != this.span.className)
		this.span.setAttribute("CSSNORMAL", this.span.className);
	
	if (css != "")
		this.span.className = css;
}

PowerUpTreeImage.prototype.setCssClassOut = function()
{
	var css = this.getAttribute("CSSNORMAL");
	
	if (css != "")
		this.span.className = css;
}

PowerUpTreeImage.prototype.getSpan = function()
{
	return this.span;
}

PowerUpTreeImage.prototype.getType = function()
{
	return "TreeImage";
}

PowerUpTreeImage.prototype.getDragGroup = function()
{
	return this.getAttribute("DRAGGROUP");
}

PowerUpTreeImage.prototype.getDropGroup = function()
{
	return this.getAttribute("DROPGROUP");
}

PowerUpTreeImage.prototype.getAttribute = function(name)
{
	var attribute = this.span.getAttribute(name);

	return attribute == null ? "" : attribute;
}

/* PowerUpTreeView */

PowerUpTreeViewCaptureMouseMoveOld = null;
PowerUpTreeViewCaptureMouseUpOld = null;
PowerUpTreeViewSpan = null;
PowerUpTreeViewDragObject = null;
PowerUpTreeViewArray = new Array();

PowerUpTreeViewScrollFixed = navigator.userAgent.indexOf("Mac") != -1 && navigator.userAgent.indexOf("Safari") != -1;

function getTreeParent(event)
{
	var element = event.srcElement ? event.srcElement : event.target;
	
	for (var i = 0; i < PowerUpTreeViewArray.length; i++)
	{
		var tree = PowerUpTreeViewArray[i];
		
		if (tree.isElementParent(element))
			return tree;
	}
	
	return null;
}

function getTreeNodeParent(event)
{
	var element = event.srcElement ? event.srcElement : event.target;
	
	while (element != null)
	{
		if (element.getAttribute == null)
		{
			element = element.parentNode;
			
			continue;
		}
			
		var index = parseInt(element.getAttribute("_WTV_INDEX"));
		
		if (!isNaN(index))
		{
			for (var i = 0; i < PowerUpTreeViewArray.length; i++)
			{
				var tree = PowerUpTreeViewArray[i];
				
				if (tree.isElementParent(element))
					return tree.nodes[index];
			}
			
			return null;
		}
			
		element = element.parentNode;
	}
}

function PowerUpTreeViewEnsureSpan()
{
	if (PowerUpTreeViewSpan == null)
	{
		document.body.appendChild(PowerUpTreeViewSpan = document.createElement("SPAN"));
	
		PowerUpTreeViewSpan.unselectable = "on";
		PowerUpTreeViewSpan.style.position = 'absolute';
		PowerUpTreeViewSpan.style.MozOpacity = 0.7;
		PowerUpTreeViewSpan.style.zIndex = 99999;
	}
}

function PowerUpTreeViewImageMouseOver(image, event)
{
	image = new PowerUpTreeImage(image);
	
	image.setCssClassOver();
}

function PowerUpTreeViewImageMouseOut(image, event)
{
	image = new PowerUpTreeImage(image);
	
	image.setCssClassOut();
}

function PowerUpTreeViewImageMouseMove(event)
{
	try
	{
		var element = event.srcElement ? event.srcElement : event.target;
		
		PowerUpTreeViewEnsureSpan();
		
		if (PowerUpTreeViewSpan.innerHTML == "")
		{
			PowerUpTreeViewSpan.innerHTML = PowerUpTreeViewDragObject.getSpan().innerHTML;
			
			PowerUpTreeViewSpan.firstChild.id = null;
			
			PowerUpTreeViewSpan.className = PowerUpTreeViewDragObject.getSpan().className;
		}

		PowerUpTreeViewSpan.style.left = event.clientX + (PowerUpTreeViewScrollFixed ? 0 : document.body.scrollLeft) + 10;
		PowerUpTreeViewSpan.style.top = event.clientY + (PowerUpTreeViewScrollFixed ? 0 : document.body.scrollTop) + 10;
		
		PowerUpTreeViewCaptureMouseMoveHelper(event);
	}
	catch (e)
	{
	}
}

function PowerUpTreeViewCancelEvent()
{
	return false;
}

function PowerUpTreeViewImageMouseDown(image, event)
{
	PowerUpTreeViewCaptureMouseMoveOld = window.document.onmousemove;
	PowerUpTreeViewCaptureMouseUpOld = window.document.onmouseup;
	
	window.document.onmousemove = new Function("args", "PowerUpTreeViewImageMouseMove(args ? args : event);");
	window.document.onmouseup = new Function("args", "PowerUpTreeViewCaptureMouseUp(args ? args : event);");

	PowerUpTreeViewDragObject = new PowerUpTreeImage(image);
	
	var element = event.srcElement ? event.srcElement : event.target;
	
	element.onselectstart = PowerUpTreeViewCancelEvent;
	element.ondragstart = PowerUpTreeViewCancelEvent;
	element.ondraggesture = PowerUpTreeViewCancelEvent;
	
	return false;
}

function PowerUpTreeViewIsAttributeNull(element, name)
{
	if (element == null)
		return false;
	
	if (element.getAttribute)
	{
		var attribute = element.getAttribute(name);
	
		return attribute == null || attribute == "";
	}
	
	return false;
}

function PowerUpTreeViewCaptureMouseUp(event)
{
	PowerUpTreeViewCaptureMouseUpHelper();
	
	window.document.onmousemove = PowerUpTreeViewCaptureMouseMoveOld;
	window.document.onmouseup = PowerUpTreeViewCaptureMouseUpOld;
	
	if (PowerUpTreeViewSpan != null)
	{
		PowerUpTreeViewSpan.style.left = -100;
		PowerUpTreeViewSpan.style.top = -1000;
		
		PowerUpTreeViewSpan.innerHTML = "";
	}
	
	var element = event.srcElement ? event.srcElement : event.target;
	
	if (document.elementFromPoint)
	{
		element = document.elementFromPoint(event.clientX, event.clientY);
		
		var dummy = new Object();
		
		for (var key in event)
			dummy[key] = event[key];
		
		dummy.srcElement = element;
		
		event = dummy;
	}
	
	var image = null;
	
	if (!PowerUpTreeViewIsAttributeNull(element, "_WTV_DRAG"))
		image = new PowerUpTreeImage(element);
	else if (!PowerUpTreeViewIsAttributeNull(element.parentNode, "_WTV_DRAG"))
		image = new PowerUpTreeImage(element.parentNode);
		
	var node = getTreeNodeParent(event);
	
	var field = new PowerUpTreeField("_wtv_dragsource");
	
	var buttons = new PowerUpTreeField("_wtv_dragbuttons");
	
	buttons.setValue((event.shiftKey ? 1: 0) | (event.ctrlKey ? 2 : 0) | (event.altKey ? 4 : 0));
	
	if (PowerUpTreeViewDragObject.getType() == "TreeNode")
		field.setValue("N:" + PowerUpTreeViewDragObject.parentTree.id + ":" + PowerUpTreeViewDragObject.state[1]);
	else
		field.setValue("I:" + PowerUpTreeViewDragObject.getSpan().id);
	
	if (node != null && (!PowerUpTreeViewIsAttributeNull(element, "_WTV_ICON") || (!node.state[25] && !PowerUpTreeViewIsAttributeNull(element, "_WTV_NODE"))))
	{
		if (PowerUpTreeViewDragObject.getDragGroup() != node.getDropGroup())
			return;
			
		if (PowerUpTreeViewDragObject == node)
			return;
			
		PowerUpTreeViewArgsNode = node;
	
		if (!node.execute("onClientDragDropStart", "PowerUpTreeViewDragObject,PowerUpTreeViewArgsNode"))
			return;
	
		node.execute("onClientDragDrop", "PowerUpTreeViewDragObject,PowerUpTreeViewArgsNode");
		
		node.post("DRAGDROP");
	}
	else
	{
		if (image != null)
		{
			if (PowerUpTreeViewDragObject.getDragGroup() != image.getDropGroup())
				return;
		
			if (PowerUpTreeViewDragObject.getType() == "TreeImage")
				if (image.getSpan() == PowerUpTreeViewDragObject.getSpan())
					return;
		
			image.post();
		}
		else
		{
			var parent = getTreeParent(event);
			
			if (parent != null)
			{
				if (parent.dragDropBackground)
					parent.postHelper("DRAGDROPBACK");
			
				parent.execute("onClientDragDropBackground", "PowerUpTreeViewDragObject");	
			}
		}
	}
	
	PowerUpTreeViewDragObject = null;
}

PowerUpTreeViewDragMoveID = null;

function PowerUpTreeViewCaptureMouseUpHelper()
{
	for (var i = 0; i < PowerUpTreeViewArray.length; i++)
		PowerUpTreeViewArray[i].flushScroll();
}

function PowerUpTreeViewMouseDownHelper()
{
	
}

function PowerUpTreeViewCaptureMouseMoveHelper(event)
{
	var height = PowerUpTreeViewScrollFixed ? window.innerHeight : document.body.clientHeight;
	var clientY = event.clientY + (PowerUpTreeViewScrollFixed ? -document.body.scrollTop : 0);
	
	if (clientY > height - 10)
		window.scrollBy(0, 10);
	else if (clientY < 10)
		window.scrollBy(0, -10);
	
	PowerUpTreeViewDragMoveX = event.clientX + (PowerUpTreeViewScrollFixed ? -document.body.scrollLeft : 0);
	PowerUpTreeViewDragMoveY = event.clientY + (PowerUpTreeViewScrollFixed ? -document.body.scrollTop : 0);
	
	window.clearTimeout(PowerUpTreeViewDragMoveID);
	PowerUpTreeViewDragMoveID = window.setTimeout(PowerUpTreeViewDragMoveWorker, 100);
}

function PowerUpTreeViewDragMoveWorker()
{
	for (var i = 0; i < PowerUpTreeViewArray.length; i++)
		PowerUpTreeViewArray[i].scrollCheck();
}

function PowerUpTreeViewCaptureMouseMove(event)
{
	var element = event.srcElement ? event.srcElement : event.target;
	
	PowerUpTreeViewEnsureSpan();
	
	if (PowerUpTreeViewSpan.innerHTML == "")
	{
		PowerUpTreeViewSpan.className = "";
		
		if (PowerUpTreeViewDragObject.state[25])
		{
			var image = PowerUpTreeViewDragObject.getFastElement("IMG", "_WTV_ICON");
			
			PowerUpTreeViewSpan.innerHTML = image.parentNode.innerHTML;
		}
		else
		{
			var array = new Array();
		
			PowerUpTreeViewDragObject.build(array);
		
			PowerUpTreeViewSpan.innerHTML = array.join("");
		}
	}

	PowerUpTreeViewSpan.style.left = event.clientX + (PowerUpTreeViewScrollFixed ? 0 : document.body.scrollLeft) + 10;
	PowerUpTreeViewSpan.style.top = event.clientY + (PowerUpTreeViewScrollFixed ? 0 : document.body.scrollTop) + 10;
	
	PowerUpTreeViewCaptureMouseMoveHelper(event);
}

function PowerUpTreeViewMouseDown(tree, event)
{
	var element = event.srcElement ? event.srcElement : event.target;
	
	if (element.tagName == "DIV")
		return;
	
	if (element.tagName == "INPUT")
	{
		
	}
	else
	{
		var fast = PowerUpTreeNodeFast(tree, event);
		
		if (fast.node != null)
		{
			if (!fast.node.state[25])
			{
				fast.node.drag();
				
				return false;
			}
		}
		else
		{
			fast = PowerUpTreeNodeIndex(tree, event);
			
			if (fast.node != null)
			{
				var index = parseInt(element.getAttribute("_WTV_ICON"));
				
				if (!isNaN(index))
				{
					element.ondragstart = PowerUpTreeViewCancelEvent;
					element.onselectstart = PowerUpTreeViewCancelEvent;
					element.ondraggesture = PowerUpTreeViewCancelEvent;
					
					fast.node.drag();
					
					return false;
				}
			}
		}
	}
}

function PowerUpTreeViewSelectStart(tree, event)
{
	var element = event.srcElement ? event.srcElement : event.target;
	
	if (element.tagName == "TD")
		return false;
		
	if (element.tagName == "TABLE")
		return false;
		
	if (element.tagName == "DIV")
		return false;
	
	if (element.tagName == "SPAN")
		return false;
}

function PowerUpTreeViewZeros(size)
{
	if (size < 1)
		return "";
	
	var zeros = "0000000000000";
	
	while (zeros.length < size)
		zeros += zeros;
	
	return zeros.substring(0, size);
}

function PowerUpTreeViewBuildRendered(parentTree, parentNode, parentState)
{
	for (var i = 0; i < parentState.length; i++)
	{
		var state = parentState[i];
		
		var node = new PowerUpTreeNode(parentTree, parentNode, state);
			
		parentTree.nodes[state[1]] = node;
		
		PowerUpTreeViewBuildRendered(parentTree, node, state[0]);
	}
}

function PowerUpTreeViewGatherTag(div, tree, tag)
{
	var containers = div.getElementsByTagName(tag);
	
	for (var i = 0; i < containers.length; i++)
	{
		var container = containers[i];
		
		var index = parseInt(container.getAttribute("_WTV_INDEX"));

		if (!isNaN(index))
			tree.nodes[index].container = container;
	}
}

function PowerUpTreeViewGather(div, tree)
{
	PowerUpTreeViewGatherTag(div, tree, "DIV");
	PowerUpTreeViewGatherTag(div, tree, "TABLE");
}

function PowerUpTreeViewGatherNodes(tree, div)
{
	var nodes = new Array();
	
	if (div == null)
		return nodes;
	
	var divs = div.childNodes;
	
	for (var i = 0; i < divs.length; i++)
	{
		var child = divs[i];
	
		if (child)
		{
			var index = parseInt(child.getAttribute("_WTV_INDEX"));
			
			if (!isNaN(index))
				nodes[nodes.length] = tree.nodes[index];
		}
	}
	
	return nodes;
}

function PowerUpTreeViewGenerate(div, parentTree, parentNode, parentState)
{
	var array = new Array();
	
	PowerUpTreeViewBuild(array, parentTree, parentNode, parentState);
	
	if (div)
	{
		div.innerHTML = array.join("");
		
		PowerUpTreeViewGather(div, parentTree);
	}
	else
	{
		document.write(array.join(""));
	}
}

function PowerUpTreeViewBuild(array, parentTree, parentNode, parentState)
{
	var depth = parentTree.showLines || parentNode == null ? null : parentTree.showLinesIndentDepth * (1 + parentNode.getDepth());
	
	for (var i = 0; i < parentState.length; i++)
	{
		var state = parentState[i];
		
		if (state[14] != 1)
			continue;
		
		var node = new PowerUpTreeNode(parentTree, parentNode, state);
		
		array[array.length] = "<DIV"
		array[array.length] = " _WTV_INDEX=\"";
		array[array.length] = state[1];
		array[array.length] = "\" class=\"" + parentTree.divClass + "\">";
		
		if (depth == null)
			node.appendImages(array);
		
		array[array.length] = node.imgTag;
		
		if (depth != null)
		{
			array[array.length] = " style=\"margin-left:"
			array[array.length] = depth;
			array[array.length] = "px\"";
		}
		
		array[array.length] = " src=\"";
		array[array.length] = parentTree.lineImages[state[19]];
		array[array.length] = "\"";
		
		if (state[0].length > 0 || state[15])
		{
			array[array.length] = " _WTV_IMAGE=\"";
			array[array.length] = state[1];
			array[array.length] = "\"";
		}
		
		array[array.length] = ">";
		
		if (parentTree.unselected)
			state[22] = 0;
		
		node.build(array);
		
		array[array.length] = "</DIV>";
		
		parentTree.nodes[state[1]] = node;
		
		if (state[20])
		{
			node.state[26] = true;
			array[array.length] = "<DIV><DIV>";
			PowerUpTreeViewBuild(array, parentTree, node, state[0]);
			array[array.length] = "</DIV></DIV>";	
		}
	}
}

function PowerUpTreeNodeFastElement(tree, event, key)
{
	var element = event.srcElement ? event.srcElement : event.target;
	
	while (element != null)
	{
		if (element.tagName == "BODY")
			return;
		
		if (element.getAttribute)
		{
			var index = parseInt(element.getAttribute(key));
			
			if (!isNaN(index))
			{
				this.node = tree.nodes[index];
				
				this.element = element;
				
				return this;
			}
		}
		
		element = element.parentNode;
	}
}

function PowerUpTreeNodeIndex(tree, event)
{
	return new PowerUpTreeNodeFastElement(tree, event, "_WTV_INDEX");
}

function PowerUpTreeNodeFast(tree, event)
{
	return new PowerUpTreeNodeFastElement(tree, event, "_WTV_NODE");
}

function PowerUpTreeViewMouseOver(tree, event)
{
	if (!tree.built)
		return;
		
	var fast = PowerUpTreeNodeFast(tree, event);
	
	if (fast.node != null)
	{
		if (!fast.node.state[22])
		{
			var css = fast.node.resolveCss(5, "nodeCssClassOver");
		
			if (css != "")
				fast.element.className = css;
		}
	}
}

function PowerUpTreeViewMouseOut(tree, event)
{
	if (!tree.built)
		return;
		
	var fast = PowerUpTreeNodeFast(tree, event);
	
	if (fast.node != null)
	{
		var css = fast.node.resolveCombinedCss();
		
		if (css != "")
			fast.element.className = css;
	}
}

function PowerUpTreeViewContextMenu(tree, event)
{
	if (!tree.built)
		return;

	var fast = PowerUpTreeNodeFast(tree, event);
	
	var node = fast.node;
	
	if (node != null)
	{
		PowerUpTreeViewArgsNode = node;
		
		node.execute("onClientContextClickStart", "PowerUpTreeViewArgsNode", true);
		node.execute("onClientContextClick", "PowerUpTreeViewArgsNode", true);
	
		return false;
	}
}

function PowerUpTreeViewScroll(tree, event)
{
	if (!tree.built)
		return;
		
	var div = document.getElementById(tree.idu + "div");
	
	tree.scrollXField.value = div.scrollLeft;
	tree.scrollYField.value = div.scrollTop;
}

function PowerUpTreeViewDoubleClick(tree, event)
{
	if (!tree.built)
		return;
		
	var fast = PowerUpTreeNodeFast(tree, event);
	
	var node = fast.node;
	
	if (node != null)
	{
		PowerUpTreeViewArgsNode = node;
		
		node.execute("onClientDoubleClickStart", "PowerUpTreeViewArgsNode", true);
		node.execute("onClientDoubleClick", "PowerUpTreeViewArgsNode", true);
	}
}

function PowerUpTreeViewClick(tree, event)
{
	if (!tree.built)
		return;
		
	var element = event.srcElement ? event.srcElement : event.target;
	
	var fast = PowerUpTreeNodeFast(tree, event);
	
	var node = fast.node;
	
	if (node != null)
	{
		if (node.state[25] == 1)
			return;
	
		PowerUpTreeViewArgsNode = node;
	
		if (!node.execute("onClientClickStart", "PowerUpTreeViewArgsNode", true))
			return;
		
		if (tree.multipleSelect)
		{
			if (!event.ctrlKey && !event.shiftKey)
			{
				tree.unselectAll();
				
				node.select();
			}
			else
			{
				if (node.isSelected())
					node.unselect();
				else
					node.select();
			}
		}
		else
		{
			tree.unselectAll();
			
			node.select();
		}
		
		node.navigate();
		
		if (node.getBoolean(9, "autoPostBack"))
		{
			node.post("CLICK");
		
			return;
		}
		
		if (node.getBoolean(11, "autoToggle"))
			node.toggleAndCollapseSiblings();
		
		node.execute("onClientClick", "PowerUpTreeViewArgsNode", true);
	}
		
	if (element.tagName == "IMG")
	{
		var index = parseInt(element.getAttribute("_WTV_IMAGE"));
		
		if (!isNaN(index))
		{
			var node = tree.nodes[index];
			
			PowerUpTreeViewArgsNode = node;
			
			if (!node.execute("onClientToggleStart", "PowerUpTreeViewArgsNode"))
				return;
			
			node.toggleAndCollapseSiblings();
			
			node.execute("onClientToggle", "PowerUpTreeViewArgsNode");
		}
		
		index = parseInt(element.getAttribute("_WTV_ICON"));
		
		if (!isNaN(index))
		{
			var node = tree.nodes[index];
			
			PowerUpTreeViewArgsNode = node;
			
			node.execute("onClientImageClick", "PowerUpTreeViewArgsNode");
		}
	}
	
	if (element.tagName == "INPUT")
	{
		var index = parseInt(element.getAttribute("_WTV_INPUT"));
		
		if (!isNaN(index))
		{
			var node = tree.nodes[index];
			
			PowerUpTreeViewArgsNode = node;
			
			var cancel = !node.execute("onClientCheckStart", "PowerUpTreeViewArgsNode");
		
			if (cancel)
			{
				if (node.isChecked())
					node.check();
				else
					node.uncheck();
			}
			else
			{
				if (node.isChecked())
					node.uncheck();
				else
					node.check();
			}
		
			if (!cancel)
			{
				if (node.getBoolean(10, "autoCheckChildren"))	
				{
					if (node.isChecked())
						node.childNodesCheck();
					else
						node.childNodesUncheck();
				}
		
				node.execute("onClientCheck", "PowerUpTreeViewArgsNode");
			}
		}
	}
}

function PowerUpTreeView(id, state, css, images, expanded, selected, checked, types, keys, templates)
{
	this.id = id;
	this.idu = id + "_";
	this.state = eval(state);
	this.expanded = new PowerUpTreeField(this.idu + "expanded");
	this.selected = new PowerUpTreeField(this.idu + "selected");
	this.checked = new PowerUpTreeField(this.idu + "checked");
	this.loadedstate = new PowerUpTreeField(this.idu + "loadedstate");
	this.loadednodes = new PowerUpTreeField(this.idu + "loadednodes");
	this.action = new PowerUpTreeField(this.idu + "action");
	this.actionindex = new PowerUpTreeField(this.idu + "actionindex");
	
	this.expanded.append(expanded);
	this.selected.append(selected);
	this.checked.append(checked);
	
	this.css = eval("[" + css + "]");
	this.images = eval("[" + images + "]");
	this.types = eval("[" + types + "]");
	this.keys = eval("[" + keys + "]");
	this.templates = eval("[" + templates + "]");
	this.nodes = new Array();
	this.scrollXField = document.getElementById(this.idu + "scrollX");
	this.scrollYField = document.getElementById(this.idu + "scrollY");
	this.eventField = document.getElementById(this.idu + "event");
	this.cssField = document.getElementById(this.idu + "css");
	this.imagesField = document.getElementById(this.idu + "images");
	this.typesField = document.getElementById(this.idu + "types");
	this.keysField = document.getElementById(this.idu + "keys");
	this.templatesField = document.getElementById(this.idu + "templates");
	this.cssField.value = css;
	this.imagesField.value = images;
	this.typesField.value = types;
	this.keysField.value = keys;
	this.templatesField.value = templates;
	
	this.lineImages = new Array();
	this.lineImagesLookup = new PowerUpTreeImagesLookup();
	
	document.getElementById(this.idu + "viewstate").value = state;
}

PowerUpTreeView.prototype.loadScroll = function()
{	
	var div = document.getElementById(this.idu + "div");
	
	var x = parseInt(this.scrollXField.value);
	var y = parseInt(this.scrollYField.value);
	
	if (!isNaN(y))
		div.scrollTop = y;
	
	if (!isNaN(x))
		div.scrollLeft = x;
}

PowerUpTreeView.prototype.getSelectedNodes = function()
{
	var array = new Array();
	
	for (var i = 0; i < this.nodes.length; i++)
	{
		var node = this.nodes[i];
		
		if (node != null && node.state[22])
		{
			array[array.length] = node;
		}
	}
	
	return array;
}

PowerUpTreeView.prototype.unselectAll = function()
{
	this.unselected = true;

	for (var i = 0; i < this.nodes.length; i++)
	{
		var node = this.nodes[i];
		
		if (node != null && node.state[22])
		{
			node.getFastNode().className = node.resolveCss(3, "nodeCssClass");
			
			node.state[22] = false;
		}
	}

	this.selected.zero();
}

PowerUpTreeView.prototype.execute = function(handler, args)
{
	var tree = this;
	var method = tree[handler];
	
	if (method != "")
		return eval(method + "(" + args + ");") != false;
	
	return true;
}

PowerUpTreeView.prototype.isPageValid = function()
{
	if (typeof(Page_ClientValidate) != 'function')
		return true;
	
	return Page_ClientValidate();
}

if (typeof(encodeURIComponent) == 'function')
{
	PowerUpTreeViewEscape = encodeURIComponent;
}
else
{
	PowerUpTreeViewEscape = function(text)
	{
		return escape(text).replace("\\x2B", "%2B");
	}
}

PowerUpTreeView.prototype.populateOnDemand = function(node)
{
	var xmlhttp;
	
	if (window.attachEvent)
		xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
	else
		xmlhttp = new XMLHttpRequest();
	
	var url = this.populateOnDemandUrl + "&_wtvdepth=" + node.getDepth() + "&_wtvvalue=" + escape(node.getValue()) + "&_wtvdatakey=" + escape(node.getDataKey()) + "&_wtvindex=" + this.index + "&_wtvpath=" + node.state[30] + "&_wtvchecked=" + node.isChecked();
	
	xmlhttp.open("POST", url, false);
	xmlhttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
	xmlhttp.send("_wtvcss=" + PowerUpTreeViewEscape(this.cssField.value) + "&_wtvimages=" + PowerUpTreeViewEscape(this.imagesField.value) + "&_wtvtypes=" + PowerUpTreeViewEscape(this.typesField.value) + "&_wtvkeys=" + PowerUpTreeViewEscape(this.keysField.value));
	
	return xmlhttp.responseText;
}

function PowerUpTreeViewCoords(element)
{
	this.y = element.offsetTop;
	this.x = element.offsetLeft;
		
	while ((element = element.offsetParent) != null)
	{
		this.y += element.offsetTop;
		this.x += element.offsetLeft;
		
		if (element.scrollTop != null)
			this.y -= element.scrollTop;
			
		if (element.scrollLeft != null)
			this.x -= element.scrollLeft;
	}
	
	return this; 
}

PowerUpTreeView.prototype.scrollCheck = function()
{
	var div = document.getElementById(this.idu + "div");
	
	var coords = PowerUpTreeViewCoords(div);
	
	if (PowerUpTreeViewDragMoveX < coords.x  || PowerUpTreeViewDragMoveX > coords.x + div.clientWidth)
	{
		this.flushScroll();
		
		return;
	}
	
	var bottom = PowerUpTreeViewDragMoveY - coords.y;
	var top = coords.y - PowerUpTreeViewDragMoveY + div.clientHeight;
	
	if (bottom > 0 && bottom < 20)
		this.scrollWorkerSet((bottom - 20) / 2);
	else if (top > 0 && top < 20)
		this.scrollWorkerSet((20 - top) / 2);
	else
		this.flushScroll();
}

PowerUpTreeView.prototype.flushScroll = function()
{
	window.clearTimeout(this.scrollTimerID);
}

PowerUpTreeView.prototype.scrollWorkerSet = function(position)
{	
	this.scrollY = position;

	this.flushScroll();
	
	this.scrollTimerID = window.setTimeout(this.id + ".scrollWorker()", 50);
}

PowerUpTreeView.prototype.scrollWorker = function()
{
	var div = document.getElementById(this.idu + "div");
	
	div.scrollTop += this.scrollY;
		
	this.scrollWorkerSet(this.scrollY);
}

PowerUpTreeView.prototype.resolveUrl = function(url)
{
	if (url == null)
		return null;
		
	if (url.substring(0, 2) == "~/")
		url = this.applicationRoot + url.substring(2, url.length);
	
	return url;
}


PowerUpTreeView.prototype.imagePreLoad = function(images)
{
	for (var i = 0; i < images.length; i++)
	{
		var url = this.resolveUrl(images[i]);
			
		var image = new Image();
		
		image.src = url;
		
		images[i] = image.src;
	}
}

PowerUpTreeView.prototype.postHelper = function(a)
{
	this.action.setValue(a);

	if (this.causesValidation)
		if (!this.parentTree.isPageValid())
			return;
	
	this.post();	
}

PowerUpTreeView.prototype.build = function()
{
	this.divClass = "wtvdiv" + this.globalID;
	this.iconClass = "wtvicon" + this.globalID;
	this.inputClass = "wtvinput" + this.globalID;
	
	var lookup = "ABCDEFGHIJKLMNOP";
	
	for (var i = 0; i < lookup.length; i++)
	{
		var image = new Image();
		
		image.src = this.lineImagesPath + lookup.charAt(i) + ".gif";
		
		this.lineImages[i] = image.src;
	}
	
	this.imagePreLoad(this.images);

	if (this.rendered)
		PowerUpTreeViewBuildRendered(this, null, this.state);
	else
		PowerUpTreeViewGenerate(null, this, null, this.state);
	
	PowerUpTreeViewArray[PowerUpTreeViewArray.length] = this;
	
	PowerUpTreeEvents.add("onload", new Function("window.setTimeout(\"" + this.id + ".loadScroll();\"" + ",250);"));

	this.built = true;
}

PowerUpTreeView.prototype.isElementParent = function(element)
{
	var div = document.getElementById(this.idu + "div");
	
	while (element != null)
	{
		if (div == element)
			return true;
			
		element = element.parentNode;
	}
	
	return false;
}

PowerUpTreeView.prototype.getNodes = function()
{
	return PowerUpTreeViewGatherNodes(this, document.getElementById(this.idu + "div").firstChild);
}

PowerUpTreeView.prototype.getType = function()
{
	return "TreeView";
}

PowerUpTreeView.prototype.expandAll = function()
{
	var nodes = this.getNodes();
	
	for (var i = 0; i < nodes.length; i++)
		nodes[i].expandAll();
}

PowerUpTreeView.prototype.collapseAll = function()
{
	var nodes = this.getNodes();
	
	for (var i = 0; i < nodes.length; i++)
		nodes[i].collapseAll();
}

PowerUpTreeView.prototype.checkAll = function()
{
	var nodes = this.getNodes();
	
	for (var i = 0; i < nodes.length; i++)
		nodes[i].checkAll();
}

PowerUpTreeView.prototype.uncheckAll = function()
{
	var nodes = this.getNodes();
	
	for (var i = 0; i < nodes.length; i++)
		nodes[i].uncheckAll();
}

PowerUpTreeView.prototype.selectAll = function()
{
	var nodes = this.getNodes();
	
	for (var i = 0; i < nodes.length; i++)
		nodes[i].selectAll();
}

/* PowerUpTreeNode */

function PowerUpTreeNode(parentTree, parentNode, state)
{
	this.parentTree = parentTree;
	this.parentNode = parentNode;
	this.state = state;
	this.imgTag = this.state[25] == 1 ? "<IMG" : "<IMG align=absmiddle";
}

PowerUpTreeNode.prototype.getType = function()
{
	return "TreeNode";
}

PowerUpTreeNode.prototype.getParentNodes = function()
{
	if (this.parentNode != null)
		return this.parentNode.getNodes();
		
	return this.parentTree.getNodes(); 
}

PowerUpTreeNode.prototype.getStyle= function(array, style)
{
	if (style == null)
		return;
	
	for (var i = 0; i < style.length; i += 2)
	{
		array[array.length] = this.parentTree.keys[style[i]];
		array[array.length] = ":";
		array[array.length] = style[i + 1];
		array[array.length] = ";";
	}
}

PowerUpTreeNode.prototype.getAttributes = function(array, attributes)
{
	if (attributes == null)
		return;
	
	for (var i = 0; i < attributes.length; i += 2)
	{
		array[array.length] = " ";
		array[array.length] = this.parentTree.keys[attributes[i]];
		array[array.length] = "=\"";
		array[array.length] = attributes[i + 1];
		array[array.length] = "\"";
	}
}

PowerUpTreeNode.prototype.getNodes = function()
{
	if (!this.state[26])
	{
		if (this.state[15])
			return new Array();
			
		this.buildNodes();
	}

	var span = this.getChildNodesDiv();
	
	if (span == null)
		return new Array();

	return PowerUpTreeViewGatherNodes(this.parentTree, this.getChildNodesDiv().firstChild);
}

PowerUpTreeNode.prototype.getString = function(index)
{
	return this.state[index] == null ? "" : this.state[index];
}

PowerUpTreeNode.prototype.getDragGroup = function(index)
{
	return this.getString(27);
}

PowerUpTreeNode.prototype.getDropGroup = function(index)
{
	return this.getString(28);
}

PowerUpTreeNode.prototype.raiseCommandEvent = function(name, argument)
{
	this.post("COMMAND:" + name + ":" + argument);
}

PowerUpTreeNode.prototype.drag = function()
{
	PowerUpTreeViewMouseDownHelper();
	
	if (this.getBoolean(29, "enableDragging"))
	{
		PowerUpTreeViewArgsNode = this;
	
		if (!this.execute("onClientDragStart", "PowerUpTreeViewArgsNode"))
			return;
		
		PowerUpTreeViewCaptureMouseMoveOld = window.document.onmousemove;
		PowerUpTreeViewCaptureMouseUpOld = window.document.onmouseup;
			
		window.document.onmousemove = new Function("args", "PowerUpTreeViewCaptureMouseMove(args ? args : event);");
		window.document.onmouseup = new Function("args", "PowerUpTreeViewCaptureMouseUp(args ? args : event);");
		
		PowerUpTreeViewDragObject = this;
		
		this.execute("onClientDrag", "PowerUpTreeViewArgsNode");
	}
}

PowerUpTreeNode.prototype.getTemplateName = function()
{
	return this.state[32];
}

PowerUpTreeNode.prototype.getParentTreeView = function()
{
	return this.parentTree;
}

PowerUpTreeNode.prototype.getParentTreeNode = function()
{
	return this.parentNode;
}

PowerUpTreeNode.prototype.appendTemplateImages = function(array)
{
	if (this.parentNode)
	{
		if (!this.templateImages)
		{
			var images = new Array();
			
			this.parentNode.appendTemplateImages(images);
			
			images[images.length] = "<IMG src=\"";
			images[images.length] = this.parentTree.lineImages[this.parentNode.state[23]];
			images[images.length] = "\">";
			
			this.templateImages = images.join("");
		}
		
		array[array.length] = this.templateImages
	}
}

PowerUpTreeNode.prototype.appendNonTemplateImages = function(array)
{
	if (this.parentNode)
	{
		if (!this.images)
		{
			var images = new Array();
			
			this.parentNode.appendNonTemplateImages(images);
			
			images[images.length] = "<IMG align=\"absmiddle\" src=\"";
			images[images.length] = this.parentTree.lineImages[this.parentNode.state[23]];
			images[images.length] = "\">";
			
			this.images = images.join("");
		}
		
		array[array.length] = this.images
	}
}

PowerUpTreeNode.prototype.appendImages = function(array)
{
	if (this.state[25] == 1)
		this.appendTemplateImages(array);
	else
		this.appendNonTemplateImages(array)
}

PowerUpTreeNode.prototype.getFastInput = function()
{
	var elements = this.container.getElementsByTagName("INPUT");
	
	for (var i = 0; i < elements.length; i++)
	{
		var element = elements[i];
		
		var index = parseInt(element.getAttribute("_WTV_INPUT"));
		
		if (!isNaN(index))
			return element;
	}
}

PowerUpTreeNode.prototype.getFastNode = function()
{
	var elements = this.container.getElementsByTagName(this.state[25] ? "TD" : "SPAN");
	
	for (var i = 0; i < elements.length; i++)
	{
		var element = elements[i];
		
		var index = parseInt(element.getAttribute("_WTV_NODE"));
		
		if (!isNaN(index))
			return element;
	}
}

PowerUpTreeNode.prototype.setChecked = function(state)
{
	var input = this.getFastInput();
	
	if (input != null)
	{
		this.parentTree.checked.setAt(this.state[1], state ? "1" : "0");
		
		this.state[21] = state;
		
		input.checked = state;
	}
}

PowerUpTreeNode.prototype.checkAll = function()
{
	this.check();
	this.childNodesCheck();
}

PowerUpTreeNode.prototype.childNodesCheck = function()
{
	var nodes = this.getNodes();
	
	for (var i = 0; i < nodes.length; i++)
	{
		nodes[i].check();
		nodes[i].childNodesCheck();
	}
}

PowerUpTreeNode.prototype.uncheckAll = function()
{
	this.uncheck();
	this.childNodesUncheck();
}

PowerUpTreeNode.prototype.childNodesUncheck = function()
{
	var nodes = this.getNodes();
	
	for (var i = 0; i < nodes.length; i++)
	{
		nodes[i].uncheck();
		nodes[i].childNodesUncheck();
	}
}

PowerUpTreeNode.prototype.selectAll = function()
{
	this.select();
	this.childNodesSelect();
}

PowerUpTreeNode.prototype.childNodesSelect = function()
{
	var nodes = this.getNodes();
	
	for (var i = 0; i < nodes.length; i++)
	{
		nodes[i].select();
		nodes[i].childNodesSelect();
	}
}

PowerUpTreeNode.prototype.unselectAll = function()
{
	this.unselect();
	this.childNodesUnselect();
}

PowerUpTreeNode.prototype.childNodesUnselect = function()
{
	var nodes = this.getNodes();
	
	for (var i = 0; i < nodes.length; i++)
	{
		nodes[i].unselect();
		nodes[i].childNodesUnselect();
	}
}

PowerUpTreeNode.prototype.expandAll = function()
{
	this.expand();
	this.childNodesExpand();
}

PowerUpTreeNode.prototype.childNodesExpand = function()
{
	var nodes = this.getNodes();
	
	for (var i = 0; i < nodes.length; i++)
	{
		nodes[i].expand();
		nodes[i].childNodesExpand();
	}
}

PowerUpTreeNode.prototype.collapseAll = function()
{
	this.collapse();
	this.childNodesCollapse();
}

PowerUpTreeNode.prototype.childNodesCollapse = function()
{
	var nodes = this.getNodes();
	
	for (var i = 0; i < nodes.length; i++)
	{
		nodes[i].collapse();
		nodes[i].childNodesCollapse();
	}
}

PowerUpTreeNode.prototype.isTemplateDataBound = function()
{
	return this.state[25] == 1;
}

PowerUpTreeNode.prototype.isSelectable = function()
{
	return !this.isTemplateDataBound();
}

PowerUpTreeNode.prototype.isCheckable = function()
{
	return this.getBoolean(24, "showCheckBoxes");
}

PowerUpTreeNode.prototype.isChecked = function()
{
	return this.state[21];
}

PowerUpTreeNode.prototype.check = function()
{
	this.setChecked(true);
}

PowerUpTreeNode.prototype.uncheck = function()
{
	this.setChecked(false);
}

PowerUpTreeNode.prototype.setSelected = function(state)
{
	if (this.state[25])
		return;
	
	this.parentTree.selected.setAt(this.state[1], state ? "1" : "0");
	
	this.state[22] = state;
	
	this.getFastNode().className = this.resolveCombinedCss();
}

PowerUpTreeNode.prototype.isSelected = function()
{
	return this.state[22];
}

PowerUpTreeNode.prototype.unselect = function()
{
	this.setSelected(false);
}

PowerUpTreeNode.prototype.select = function()
{
	this.setSelected(true);
}

PowerUpTreeNode.prototype.resolveCss = function(index, name)
{
	var css = this.state[index];
	
	if (css != null)
		return this.parentTree.css[css];
		
	return this.parentTree[name];
}

PowerUpTreeNode.prototype.resolveCombinedCss = function()
{
	var css = "";
	
	if (this.state[22])
		css = this.resolveCss(4, "nodeCssClassSelected");
	
	if (css != "")
		return css;
		
	return this.resolveCss(3, "nodeCssClass");
}

PowerUpTreeNode.prototype.resolveImageUrl = function(index, name)
{
	var image = this.state[index];
	
	if (image != null)
		return this.parentTree.images[image];
		
	return this.parentTree[name];
}

PowerUpTreeNode.prototype.resolveCombinedImageUrl = function()
{
	var image = "";
	
	if (this.state[20])
		image = this.resolveImageUrl(8, "nodeImageUrlExpanded");
		
	if (image != "")
		return image;
		
	return this.resolveImageUrl(7, "nodeImageUrl");
}

PowerUpTreeNode.prototype.build = function(array)
{
	var image = this.resolveCombinedImageUrl();
		
	if (image != "")
	{
		array[array.length] = "<IMG class=\""
		array[array.length] = this.parentTree.iconClass;
		array[array.length] = "\" src=\"";
		array[array.length] = image;
		array[array.length] = "\" _WTV_ICON=\""
		array[array.length] = this.state[1];
		array[array.length] = "\">"
	}
	
	if (this.getBoolean(24, "showCheckBoxes"))
	{
		array[array.length] = "<INPUT type=\"checkbox\" class=\""
		array[array.length] = this.parentTree.inputClass;
		array[array.length] = "\""
		
		if (this.state[21])
			array[array.length] = " CHECKED"
		
		array[array.length] = " _WTV_INPUT=\""
		array[array.length] = this.state[1];
		array[array.length] = "\">"
	}

	array[array.length] = "<SPAN unselectable=\"on\" _WTV_NODE=\"";
	array[array.length] = this.state[1];
	array[array.length] = "\"";
	this.getAttributes(array, this.state[36]);
	array[array.length] = " style=\""
	this.getStyle(array, this.state[37]);
	array[array.length] = "\" class=\""
	array[array.length] = this.resolveCombinedCss();
	array[array.length] = "\""
	
	if (this.state[38] != null)
	{
		array[array.length] = " title=\"";
		array[array.length] = this.state[38];
		array[array.length] = "\"";
	}
	
	array[array.length] = ">"
	array[array.length] = this.getText();
	array[array.length] = "</SPAN>";
}

PowerUpTreeNode.prototype.getFastElement = function(tag, key)
{
	var images = this.container.getElementsByTagName(tag);
	
	for (var i = 0; i < images.length; i++)
	{
		var image = images[i];
		
		var index = parseInt(image.getAttribute(key));
		
		if (!isNaN(index))
			return image;
	}
}

PowerUpTreeNode.prototype.getChildNodesDiv = function()
{
	return this.container.nextSibling;
}

PowerUpTreeNode.prototype.isExpanded = function()
{
	return this.state[20];
}

PowerUpTreeNode.prototype.navigate = function()
{
	var url = this.state[31];

	if (url != null)
	{
		var target = this.state[17];
		
		if (target == null)
			target = this.parentTree.target;
		
		if (this.parentTree.navigateUrlBase != "")
			url = this.parentTree.navigateUrlBase + url;
			
		if (target == "")
			target = "_self";
		
		window.open(this.parentTree.resolveUrl(url), target);
	}
}

PowerUpTreeNode.prototype.expand = function()
{
	if (!this.isExpanded())
		this.toggle();	
}

PowerUpTreeNode.prototype.getToolTip = function()
{
	return this.state[38];
}

PowerUpTreeNode.prototype.collapse = function()
{
	if (this.isExpanded())
		this.toggle();	
}

PowerUpTreeNode.prototype.toggle = function()
{
	if (this.populating)
		return;

	if (this.state[0].length < 1 && this.state[15] != 1)
		return;

	if (this.getBoolean(12, "toggleOnServer"))
	{
		this.post("TOGGLE");
		
		return;
	}
	
	if (!this.state[26])
		this.buildNodes();
	
	var image = this.getFastElement("IMG", "_WTV_IMAGE");
	
	image.src = this.parentTree.lineImagesLookup.toggle(image.src);
	
	var expanded = this.isExpanded();
	
	this.parentTree.expanded.setAt(this.state[1], expanded ? "0" : "1");
	
	this.state[20] = !expanded;
	
	image = this.getFastElement("IMG", "_WTV_ICON");
	
	if (image != null)
		image.src = this.resolveCombinedImageUrl();
	
	if (this.parentTree.animate)
	{
		if (expanded)
			this.startCollapse();
		else
			this.startExpand();
	}
	else
	{
		this.getChildNodesDiv().style.display = expanded ? "none" : "block";
	}
	
	this.toggleTreeImages();
}

PowerUpTreeNode.prototype.toggleTreeImages = function()
{
	if (!this.isTemplateDataBound())
		return;

	var cells = this.container.rows[0].cells;
	
	var images = cells[cells.length - 1].getElementsByTagName("IMG");
	
	for (var i = 0; i < images.length; i++)
	{
		var image = images[i];
		var imageUrl = image.getAttribute("IMAGEURL");
	
		if (imageUrl != null)
		{
			var src = image.src;
			image.src = imageUrl;
			image.setAttribute("IMAGEURL", src);
		}
	}
}

PowerUpTreeNode.prototype.getBoolean = function(index, name)
{
	var state = this.state[index];
	
	if (state == null)
		return this.parentTree[name];
		
	return state == 1;
}

PowerUpTreeNode.prototype.toggleAndCollapseSiblings = function()
{
	if (!this.isExpanded() && this.parentTree.singleExpand)
		this.collapseSiblings();
		
	this.toggle();
}

PowerUpTreeNode.prototype.collapseSiblings = function()
{
	var nodes = this.getParentNodes();
	
	for (var i = 0; i < nodes.length; i++)
	{
		if (nodes[i] != this && nodes[i].populating != true)
			nodes[i].collapse();
	}
}

PowerUpTreeNode.prototype.buildNodeLoading = function()
{
	var array = new Array();
	
	this.appendImages(array);
	
	array[array.length] = "<IMG align=\"absmiddle\" src=\"";
	array[array.length] = this.parentTree.lineImages[this.state[23]];
	array[array.length] = "\"><IMG align=\"absmiddle\" src=\"";
	array[array.length] = this.parentTree.lineImages[4];
	array[array.length] = "\"><SPAN class=\"";
	array[array.length] = this.parentTree.nodeCssClassLoading;
	array[array.length] = "\">" + this.parentTree.loadingText;
	array[array.length] = "</SPAN>";
	
	this.container.nextSibling.firstChild.innerHTML = array.join("");
}

PowerUpTreeNode.prototype.resetPopulateOnDemand = function()
{
	if (!this.populating)
	{
		this.collapse();
		
		this.state[26] = false;
		this.state[15] = 1;
	}
}

PowerUpTreeNode.prototype.buildPopulateOnDemand = function()
{
	var data = this.parentTree.populateOnDemand(this);
	
	if (data.charAt(0) == "[")
	{
		var state = eval(data);
		
		this.parentTree.index = state[0];
		
		this.parentTree.loadedstate.append(state[1]);
		this.parentTree.expanded.append(state[2]);
		this.parentTree.selected.append(this.parentTree.unselected ? PowerUpTreeViewZeros(state[3].length) : state[3]);
		this.parentTree.checked.append(state[4]);
		this.parentTree.loadednodes.increment(this.state[1]);
		
		if (state[5])
		{
			this.parentTree.css = eval("[" + state[5] + "]");
			this.parentTree.cssField.value = state[5];
		}
		
		if (state[6])
		{
			this.parentTree.images = eval("[" + state[6] + "]");
			this.parentTree.imagesField.value = state[6];
		}
		
		if (state[7])
		{
			this.parentTree.keys = eval("[" + state[7] + "]");
			this.parentTree.keysField.value = state[7];
		}
		
		if (state[8])
		{
			this.parentTree.types = eval("[" + state[8] + "]");
			this.parentTree.typesField.value = state[8];
		}
		
		this.state[0] = state = eval(state[1]);
		
		if (state.length > 0)
		{
			PowerUpTreeViewGenerate(this.container.nextSibling.firstChild, this.parentTree, this, state);
			
			this.populating = false;
		}
		else
		{
			this.populating = false;
			
			this.toggle();
			
			var image = this.getFastElement("IMG", "_WTV_IMAGE");
			
			image.setAttribute("_WTV_IMAGE", null);
			
			image.src = this.parentTree.lineImagesLookup.getEmpty(image.src);
		}
	}
	else
	{
		document.write(data);
	}
}

function PowerUpTreeNodePopulateOnDemand(tree, index)
{
	var node = tree.nodes[index];
	
	try
	{
		tree.nodes[index].buildPopulateOnDemand();
	}
	catch (e)
	{
		node.post("TOGGLE");
	}
}

function PowerUpTreeNodeToggle(tree, index)
{
	tree.nodes[index].post("TOGGLE");
}

PowerUpTreeNode.prototype.populateOnDemand = function()
{
	this.populating = true;

	this.buildNodeLoading();
	
	if (this.state[16])
	{
		window.setTimeout("PowerUpTreeNodePopulateOnDemand(" + this.parentTree.id + "," + this.state[1] + ");", 350);
	}
	else
	{
		if (this.parentTree.animate)
			window.setTimeout("PowerUpTreeNodeToggle(" + this.parentTree.id + "," + this.state[1] + ");", 350);
		else
			this.post("TOGGLE");
	}
}

if (typeof(decodeURIComponent) == 'function')
{
	PowerUpTreeNode.prototype.getText = function()
	{
		if (this.state[2] == null)
			return "";

		try
		{
			var s = this.state[2];
			
			s = decodeURIComponent(s);
			
			return s;
		}
		catch (e)
		{
			return this.state[2];
		}
	}
}
else
{
	PowerUpTreeNode.prototype.getText = function()
	{
		if (this.state[2] == null)
			return "";

		try
		{
			var s = this.state[2];
			
			s = unescape(s);
			
			return s;
		}
		catch (e)
		{
			return this.state[2];
		}
	}
}

PowerUpTreeNode.prototype.resolveNodeElement = function(tag)
{
	var elements = this.container.getElementsByTagName(tag);
	
	for (var i = 0; i < elements.length; i++)
	{
		var element = elements[i]
		
		if (!PowerUpTreeViewIsAttributeNull(element, "_WTV_NODE"))
			return element;
	}
}

PowerUpTreeNode.prototype.setText = function(text)
{
	if (!this.isTemplateDataBound())
	{
		var element = this.resolveNodeElement("SPAN");
		
		if (element != null)
			element.innerHTML = text;
			
		this.state[2] = text;
	}
}

PowerUpTreeNode.prototype.buildNodes = function()
{
	var container = this.container;
	
	if (!this.state[26])
	{
		this.state[26] = true;
		
		var child = document.createElement("DIV");
		
		child.style.display = "none";
		
		container.parentNode.insertBefore(child, container.nextSibling);
		
		child.innerHTML = "<DIV></DIV>";
		
		child.className = this.parentTree.divClass;
		
		if (this.state[15])
			this.populateOnDemand(child.firstChild);
		else
			PowerUpTreeViewGenerate(child.firstChild, this.parentTree, this, this.state[0]);
	}
}

PowerUpTreeNode.prototype.getDepth = function()
{
	return this.getAncestors().length;
}

PowerUpTreeNode.prototype.getValue = function()
{
	return this.state[18];
}

PowerUpTreeNode.prototype.getDataKey = function()
{
	return this.state[35];
}

PowerUpTreeNode.prototype.getAncestors = function()
{
	var ancestors = new Array();
	
	var parentNode = this;
	
	while ((parentNode = parentNode.parentNode) != null)
		ancestors[ancestors.length] = parentNode;
	
	ancestors.reverse();
	
	return ancestors;
}

PowerUpTreeNode.prototype.post = function(a)
{
	this.parentTree.action.setValue(a);
	this.parentTree.actionindex.setValue(this.state[1]);

	if ((this.state[13] == 1) || (this.state[13] == null && this.parentTree.causesValidation))
		if (!this.parentTree.isPageValid())
			return;
	
	this.parentTree.post();	
}

/* PowerUpTreeField */

function PowerUpTreeField(name)
{
	for (var i = 0; i < document.forms.length; i++)
	{
		this.field = document.forms[i][name];
		
		if (this.field != null)
			break;
	}
}

PowerUpTreeField.prototype.exists = function()
{
	return this.field != null;
}

PowerUpTreeField.prototype.increment = function(text)
{
	if (this.field.value != "")
		this.field.value += ",";
	
	this.field.value += text;
}

PowerUpTreeField.prototype.append = function(text)
{
	this.field.value += text;
}

PowerUpTreeField.prototype.getAt = function(index)
{
	return parseInt(this.field.value.charAt(index));
}

PowerUpTreeField.prototype.zero = function()
{
	this.field.value = PowerUpTreeViewZeros(this.field.value.length);
}

PowerUpTreeField.prototype.setValue = function(value)
{
	this.field.value = value;
}

PowerUpTreeField.prototype.getValue = function(value)
{
	return this.field.value;
}

PowerUpTreeField.prototype.setAt = function(index, value)
{
	var copy = this.field.value;
	
	this.field.value = (index > 0 ? copy.substring(0, index) : "") + value + (index + 1 < copy.length ? copy.substring(index + 1, copy.length) : "");
}

/* Animation */

PowerUpTreeNode.prototype.startExpand = function()
{
	window.clearTimeout(this.collapseTimerID);
	window.clearTimeout(this.expandTimerID);
	
	var div = this.container;
	
	var groupDiv = div.nextSibling;
	
	if (groupDiv.style.height == "")
		groupDiv.style.height = "1px";
	
	groupDiv.style.overflow = "hidden";
	groupDiv.style.display = "block";
	
	var scrollDiv = groupDiv.firstChild;
	
	groupDiv.style.position = "relative";
	scrollDiv.style.position = "relative";
	
	this.expandTimerID = window.setTimeout("PowerUpTreeNodeExpand(" + this.parentTree.id + "," + this.state[1] + ")", 25);
}

PowerUpTreeNode.prototype.execute = function(handler, args, skipTemplate)
{
	if (skipTemplate && this.state[25])
		return true;

	return this.parentTree.execute(handler, args);
}

PowerUpTreeNode.prototype.startCollapse = function()
{
	window.clearTimeout(this.expandTimerID);
	window.clearTimeout(this.collapseTimerID);

	var div = this.container;
	
	var groupDiv = div.nextSibling;
	
	groupDiv.style.overflow = "hidden";
	groupDiv.style.display = "block";
	
	var scrollDiv = groupDiv.firstChild;
	
	groupDiv.style.position = "relative";
	scrollDiv.style.position = "relative";
	
	groupDiv.style.height = scrollDiv.offsetHeight + "px";
		
	this.collapseTimerID = window.setTimeout("PowerUpTreeNodeCollapse(" + this.parentTree.id + "," + this.state[1] + ")", 25);
}

function PowerUpTreeNodeExpand(bar, index)
{
	var item = bar.nodes[index];
	
	var div = item.container;
	
	var groupDiv = div.nextSibling;
	
	var scrollDiv = groupDiv.firstChild;
	
	var height = parseInt(groupDiv.style.height.replace("px", ""));
	
	if (isNaN(height))
		height = 0;
	
	var step = Math.abs((scrollDiv.offsetHeight - height) / 3);
	
	if (step < 2)
		step = 2;
	
	height += step;
		
	var top = height - scrollDiv.offsetHeight;
	
	scrollDiv.style.top = top;
	
	if (height < scrollDiv.offsetHeight)
	{
		groupDiv.style.height = height + "px";
		
		item.expandTimerID = window.setTimeout("PowerUpTreeNodeExpand(" + item.parentTree.id + "," + item.state[1] + ")", 20);
	}
	else
	{
		groupDiv.style.position = "";
		scrollDiv.style.position = "";
		groupDiv.style.height = "";
		groupDiv.style.overflow = "";
		groupDiv.style.display = "";
	}
}

function PowerUpTreeNodeCollapse(bar, index)
{
	var item = bar.nodes[index];
	
	var div = item.container;
	
	var groupDiv = div.nextSibling;
	
	var scrollDiv = groupDiv.firstChild;
	
	var height = parseInt(groupDiv.style.height.replace("px", ""));
	
	if (isNaN(height))
		height = 0;
	
	if (height > scrollDiv.offsetHeight)
		height = scrollDiv.offsetHeight;
	
	var step = Math.abs((scrollDiv.offsetHeight - Math.abs(scrollDiv.offsetHeight - height)) / 3);
	
	if (step < 2)
		step = 2;
	
	height -= step;
	
	scrollDiv.style.top = height - scrollDiv.offsetHeight;
	
	if (height > 0)
	{
		groupDiv.style.height = height + "px";
		
		item.expandTimerID = window.setTimeout("PowerUpTreeNodeCollapse(" + item.parentTree.id + "," + item.state[1] + ")", 20);
	}
	else
	{
		groupDiv.style.position = "";
		scrollDiv.style.position = "";
		groupDiv.style.height = "";
		groupDiv.style.display = "none";
	}
}

/* PowerUpTreeImagesLookup */

function PowerUpTreeImagesLookup()
{
	var lookup = "GLHMINJOKP";
	
	for (var i = 0; i < lookup.length; i += 2)
	{
		this[lookup.charAt(i)] = lookup.charAt(i + 1);
		this[lookup.charAt(i + 1)] = lookup.charAt(i);
	}
	
	var empty = "GAHBICJEKFLAMBNCOEPF";
	
	this.empty = new Object();
	
	for (var i = 0; i < empty.length; i += 2)
		this.empty[empty.charAt(i)] = empty.charAt(i + 1);
}

PowerUpTreeImagesLookup.prototype.toggle = function(src)
{
	return src.substring(0, src.length - 5) + this[src.charAt(src.length - 5)] + src.substring(src.length - 4, src.length);
}

PowerUpTreeImagesLookup.prototype.getEmpty = function(src)
{
	return src.substring(0, src.length - 5) + this.empty[src.charAt(src.length - 5)] + src.substring(src.length - 4, src.length);
}