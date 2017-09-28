<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="MinerDOG.index" EnableEventValidation = "false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style> 
body{ text-align:center} 
.div{ margin:0 auto; width:1280px; height:100px;} 
.pl
{
position: fixed;
width: 100%;
height:500px;
border:1px solid #F00;
background:#FFFFFF;

    }
    .pl2
{
    
position:fixed;
display:none;
width: 100%;
height:500px;
border:1px solid #F00;
background:#FFFFFF;
z-index:100;

    }
    .cs
{

width: 80vw;


    }
    .gv
    {
        width:100%;
        }
/* css注释：为了观察效果设置宽度 边框 高度等样式 */ 
        .style1
        {
            
        }
    </style> 
<script src="https://cdn.hcharts.cn/highcharts/highcharts.js"></script>
<script type="text/javascript">
    /**　　    window.onload = function () {
    var height = document.body.scrollHeight;

    var logo_wrap = document.getElementById("Panel1");
    var margin_top = (height - 500)/2;       //因为此div在页面中只用了一次且以后不会改变，所以写了数值，如果是不确定的，获取到高度放着这里就可以
    logo_wrap.style.top = margin_top+"px";
    logo_wrap.style.left = "0px";
    };

    * Highcharts 在 4.2.0 开始已经不依赖 jQuery 了，直接用其构造函数既可创建图表
    **/
    function closediv() {
        var logo_wrap = document.getElementById("hchart");
        logo_wrap.style.display = "none";
    }
    function chart(x, y1, y2, y3) {
        var height = document.body.scrollHeight;

        var logo_wrap = document.getElementById("hchart");
        var margin_top = (height - 1500) / 2;       //因为此div在页面中只用了一次且以后不会改变，所以写了数值，如果是不确定的，获取到高度放着这里就可以
        logo_wrap.style.top = margin_top + "px";
        logo_wrap.style.left = "0px";
        logo_wrap.style.display = "block";
        var chart = new Highcharts.Chart('container', {
            chart: {
                zoomType: 'x'
            },
            title: {
                text: '设备24小时温度曲线',
                x: -20
            },
            
            xAxis: {
                categories: x
            },
            yAxis: {
                title: {
                    text: '温度 (°C)'
                },
                plotLines: [{
                    value: 0,
                    width: 1,
                    color: '#808080'
                }]
            },
            tooltip: {
                valueSuffix: '°C'
            },
            legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'middle',
                borderWidth: 0
            },
            series: [{
                name: '板卡1',
                data: y1
            }, {
                name: '板卡2',
                data: y2
            }, {
                name: '板卡3',
                data: y3
            }]
        });
    }
    function chart1(x, y1, y2, y, title) {

        var chart = new Highcharts.Chart('th' + y, {
            chart: {
                zoomType: 'x'
            },
            title: {
                text: title
            },
            subtitle: {
                text: document.ontouchstart === undefined ?
            '鼠标拖动可以进行缩放' : '手势操作进行缩放'
            },
            xAxis: {
                categories: x
            },
            tooltip: {
                dateTimeLabelFormats: {
                    millisecond: '%H:%M:%S.%L',
                    second: '%H:%M:%S',
                    minute: '%H:%M',
                    hour: '%H:%M',
                    day: '%Y-%m-%d',
                    week: '%m-%d',
                    month: '%Y-%m',
                    year: '%Y'
                }
            },
            yAxis: [{
                title: {
                    text: '温度'
                },
                
                labels: {
                    align: 'left',
                    x: 3,
                    y: 16,
                    format: '{value:.,0f}'
                }
            }, {
                               
                opposite: true, 
                title: {
                    text: '湿度'
                },
               
                labels: {
                    align: 'right',
                    x: -3,
                    y: 16,
                    format: '{value:.,0f}'
                }
            }],
            legend: {
                enabled: false
            },
            plotOptions: {
                area: {
                    fillColor: {
                        linearGradient: {
                            x1: 0,
                            y1: 0,
                            x2: 0,
                            y2: 1
                        },
                        stops: [
                        [0, Highcharts.getOptions().colors[0]],
                        [1, Highcharts.Color(Highcharts.getOptions().colors[0]).setOpacity(0).get('rgba')]
                    ]
                    },
                    marker: {
                        radius: 2
                    },
                    lineWidth: 1,
                    states: {
                        hover: {
                            lineWidth: 1
                        }
                    },
                    threshold: null
                }
            },
            series: [{
                yAxis: 0,
                type: 'line',
                name: '温度',
                data: y1
            }, {
                yAxis: 1,
                type: 'line',
                name: '湿度',
                data: y2
            }]
        });
    }
</script>
</head>
<body>
    <form id="form1" runat="server">
    
    
        <br />
        <h4>
        <asp:Label ID="Label4" runat="server" Text="Label"></asp:Label>
        </h4>
        <h4>
        <asp:Label ID="Label3" runat="server" Text="Label"></asp:Label>
        <br /></h4>
    
    
        <asp:GridView ID="GridView1" runat="server" BackColor="White" 
            BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="4" 
            ForeColor="Black" GridLines="Horizontal" CssClass="gv" 
            onrowdatabound="OnRowDataBound1">
            <FooterStyle BackColor="#CCCC99" ForeColor="Black" />
            <HeaderStyle BackColor="#333333" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="White" ForeColor="Black" HorizontalAlign="Right" />
            <SelectedRowStyle BackColor="#CC3333" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#F7F7F7" />
            <SortedAscendingHeaderStyle BackColor="#4B4B4B" />
            <SortedDescendingCellStyle BackColor="#E5E5E5" />
            <SortedDescendingHeaderStyle BackColor="#242121" />
        </asp:GridView>
        
        <br /> 国道矿场运行情况<br />
        <h4>
      <asp:Label ID="Label1" runat="server" Text="Label" style="text-align: left"></asp:Label>
        </h4>
        <p>
          <h4>  <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label></h4>
        </p>
      <br />
        <asp:GridView ID="GridView2" runat="server" BackColor="White" 
            BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="4" 
            ForeColor="Black" GridLines="Horizontal" onrowdatabound="OnRowDataBound" 
            onselectedindexchanged="OnSelectedIndexChanged" CssClass="gv">
            <FooterStyle BackColor="#CCCC99" ForeColor="Black" />
            <HeaderStyle BackColor="#333333" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="White" ForeColor="Black" HorizontalAlign="Right" />
            <SelectedRowStyle BackColor="#CC3333" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#F7F7F7" />
            <SortedAscendingHeaderStyle BackColor="#4B4B4B" />
            <SortedDescendingCellStyle BackColor="#E5E5E5" />
            <SortedDescendingHeaderStyle BackColor="#242121" />
        </asp:GridView>
    
    
  
         <asp:Panel ID="Panel1" runat="server" Visible="False" class="pl" >
       
            
    
             <asp:Chart ID="Chart1" runat="server" Height="400px" Width="960px">
                 <series>
                     <asp:Series ChartType="Line" Legend="Legend1" Name="1号板卡">
                     </asp:Series>
                     <asp:Series ChartArea="ChartArea1" ChartType="Line" Legend="Legend1" 
                         Name="2号板卡">
                     </asp:Series>
                     <asp:Series ChartArea="ChartArea1" ChartType="Line" Legend="Legend1" 
                         Name="3号板卡">
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
             <br/>
    
             <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="关闭" />

            

        </asp:Panel>

    
    

    
   
            <div id="hchart" class="pl2" >
            <div id="container" style="min-width:400px;height:400px"></div>
            <input id="Button2" type="button" value="关闭" onclick="closediv();"/>

        </div>
         <div>
             <p></p>
                 <table style="width:100%" border="1" cellspacing="0" cellpadding="0">
                     <tr>
                         <td id="th1"  width="50%" height="400">
                         
                             </td>
                         <td id="th2"  width="50%" height="400">
                         
                             </td>
                         
                     </tr>
                     <tr>
                         <td id="th3" width="50%" height="400">
                          </td>
                         <td id="th4" width="50%" height="400">
                            </td>
                        
                     </tr>
                    
                 </table>
             
             </div>
         </form>
</body>

</html>
