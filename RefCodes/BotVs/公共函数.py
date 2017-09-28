# botvs@8dd9d76593f72d4cbb1f5fdc0cafd09e
def Test():
    Log("template call")
def Cut(num,c): #浮点数截断 num是待截断浮点  c截断位数
    if num%1==0:
        return num
    c=10**(-c)
    d=(num//c)*c  
    return d  
def GetStrFromList(arr): #将list 转变为以回车为分界的字符串
    #Log(type(arr))
    return "\n".join(arr)
    #将pa.dataframe 转为支持logstatus可显示的表格 返回数据为dic
def GetTableDict(df,title,columns):
        #Log(type(df))
        str={'type': 'table', 'title': title, 'cols': [],'rows':[]}
        str["cols"]=columns 
        d= eval(df.to_json(orient='split'))    
        d=d["data"]
        if title=="交易所":
            
            for i in range(len(d)):
                for j in range(len(d[i])):
                    if j>2:
                        d[i][j]=round(d[i][j],2)
        str["rows"]=d
        return str

ext.Test = Test # 导出Test函数, 主策略可以通过ext.Test()调用
ext.Cut=Cut
ext.GetStrFromList=GetStrFromList
ext.GetTableDict=GetTableDict