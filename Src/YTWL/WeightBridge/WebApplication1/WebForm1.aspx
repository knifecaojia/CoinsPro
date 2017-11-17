<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WebApplication1.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style>
        table,table tr th, table tr td { border:1px solid #000000; }
        table { margin: 0 auto;width: 960px; min-height: 25px; line-height: 25px; text-align: center; border-collapse: collapse;}   
        .auto-style1 {
            width: 120px;
        }
        .auto-style2 {
            width: 200px;
        }
        .auto-style4 {
            text-align: center;
        }
        .auto-style5 {
            width: 120px;
            height: 28px;
        }
        .auto-style6 {
            width: 200px;
            height: 28px;
        }
        .auto-style7 {
            height: 28px;
        }
    </style>
</head>
<body>
<h1 style="text-align:center;">
	<strong><span style="font-size:32px;">内蒙古华云新材料有限公司过磅单</span></strong>
</h1>
    <div style="width:100%" class="auto-style4">
	<span style="width:960px; font-size:12px; text-align:center;">&nbsp; &nbsp;磅单号：$1&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; 打印日期：$2&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp; &nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;单位：&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;吨</span>
   </div>
        <br />
	</br>
	<table   style="font-size:12px;">
		<tbody>
			<tr>
				<td class="auto-style1">
					货物名称
				</td>
				<td class="auto-style2">
	$3$
				</td>
				<td class="auto-style1">
					发货单位
				</td>
				<td class="auto-style2">
	<span style="width:960px; font-size:12px; text-align:center;">$4$</span><br />
				</td>
				<td>
					收货单位
				</td>
				<td class="auto-style2">
	<span style="width:960px; font-size:12px; text-align:center;">$5$</span><br />
				</td>
			</tr>
			<tr>
				<td class="auto-style5">
					车&nbsp;&nbsp; 号 </td>
				<td class="auto-style6">
	$6$
				</td>
				<td class="auto-style5">
					毛&nbsp;&nbsp; 重 
				<td class="auto-style6">
$7$
				</td>
				<td class="auto-style7">
					毛重时间
				</td>
				<td class="auto-style6">
	$8$
				</td>
			</tr>
			<tr>
				<td class="auto-style1">
					件&nbsp;&nbsp; 数 </td>
				<td class="auto-style2">
$9$
				</td>
				<td class="auto-style1">
					皮&nbsp;&nbsp; 重 </td>
	<td class="auto-style2">
                    $10$
				</td>
				<td>
					皮重时间
				</td>
				<td class="auto-style2">
	$11$
				</td>
			</tr>
			<tr>
				<td class="auto-style1">
					检 斤 员 </td>
				<td class="auto-style2">
	$12
				</td>
				<td class="auto-style1">
					净&nbsp;&nbsp; 重 </td>
	<td class="auto-style2">
                $13$
				</td>
				<td>
					审核状态
				</td>
				<td class="auto-style2">
	$14$
				</td>
			</tr>
			<tr>
				<td class="auto-style1">
					毛重司磅员
				</td>
				<td class="auto-style2">
	$15
				
				</td>
				<td class="auto-style1">
					皮重司磅员</td>
			<td class="auto-style2">$16$
				</td>
				<td>
					经办人 </td>
				<td class="auto-style2">
	$17$
				</td>
			</tr>
		</tbody>
	</table>
<br />

</body>
</html>
