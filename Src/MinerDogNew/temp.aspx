<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="temp.aspx.cs" Inherits="MinerDOG.temp" %>

<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .style1
        {
            text-align: center;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="style1">
    
        <h1>
            <strong>设备24小时温度变化曲线</strong></h1>
    
    
    <asp:Chart ID="Chart1" runat="server" Width="1400px">
        <series>
            <asp:Series ChartType="Line" Name="1号板卡" Legend="Legend1">
            </asp:Series>
            <asp:Series ChartArea="ChartArea1" ChartType="Line" Name="2号板卡" 
                Legend="Legend1">
            </asp:Series>
            <asp:Series ChartArea="ChartArea1" ChartType="Line" Name="3号板卡" 
                Legend="Legend1">
            </asp:Series>
        </series>
        <chartareas>
            <asp:ChartArea Name="ChartArea1">
                <AxisY Maximum="100" Minimum="50">
                </AxisY>
                <AxisX IntervalAutoMode="VariableCount" IsLabelAutoFit="False" 
                    IsStartedFromZero="False">
                </AxisX>
            </asp:ChartArea>
        </chartareas>
        <Legends>
            <asp:Legend Name="Legend1">
            </asp:Legend>
        </Legends>
    </asp:Chart>
    </form>
    </div>
</body>
</html>
