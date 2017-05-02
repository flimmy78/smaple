﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TestToolTipsClient.aspx.cs"
    Inherits="TestToolTipsClient" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>无标题页</title> 
     <style type="text/css">
        .tooltips
        {
            max-width:150px;
            border:solid 1px blue;
            padding:4px;
            background-color:White;
            word-break:break-all;
        }
        span
        {
	        text-decoration: underline;
	        color: #000080;
	        cursor:hand;
        }
        
        #Monitor{
            position:absolute;
            top:150px;
            width:150px;
            height:30px;
            padding:6px;
            color:White;
            background-color:Blue;
        }
    </style>
    <script type="text/javascript">
        function onToolTipsShow(){
            var content;
            if(event.srcElement){
                content = event.srcElement.firstChild.nodeValue;
            }
            else if(event.target){
                content = event.target.firstNode.innerText;
            }
            $get('Monitor').innerText = content + ' has overed';
        }
        
        function pageLoad(){
            Sys.Application.add_init(function() {
                $create(AJAXControls.ToolTips, {"popupElement":'MicrosoftDescription'}, {"show":onToolTipsShow}, null, $get("Microsoft"));
            });
            
            var tooltips = new  AJAXControls.ToolTips($get('Vista'));
            tooltips.set_popupElement('VistaDescription');
            tooltips.add_show(onToolTipsShow);
            tooltips.initialize();
            Sys.Application.addComponent(tooltips);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Scripts>
                <asp:ScriptReference Assembly="Microsoft.Web.Preview" Name="PreviewScript.js" />
                <asp:ScriptReference Path="ToolTipsControl.js" />
            </Scripts>
        </asp:ScriptManager>
        <div>
            <span id="Microsoft">微软公司</span>昨日宣布，<span id="Vista">Vista</span>操作系统提供完全免费的版本，此举对<span id="Linux">Linux</span>必将造成不良影响。
        </div>
        <div id="MicrosoftDescription" class="tooltips">Microsoft is the greatest software corparation.</div>
        <div id="VistaDescription" class="tooltips">Vista is a modern computer operation system.a product of Microsoft.</div>
        <div id="LinuxDescription" class="tooltips">Linux is a computer operation system too.</div>
        <div id="Monitor"></div>
        <script type="text/xml-script">
            <page xmlns:script="http://schemas.microsoft.com/xml-script/2005">
                <components>
                    <tooltips id="Linux" popupElement="LinuxDescription" show="onToolTipsShow"/>
                </components>
            </page>
        </script>
    </form>
</body>
</html>
