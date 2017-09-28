# botvs@aeb919da31253101019d226ad2de751c
import numpy as np
import pandas as pa
import json
import time
import copy
import threading
import traceback
exchangesnamelist=[]
exchangespairlnamelist=[]#交易市场对儿
exchangesdf=pa.DataFrame()
exchangesdiffdf=pa.DataFrame()
tradetasklistdf=pa.DataFrame()
exchangesdfcolumnsname=["Name","Delay","Buy","Sell","Balance","Stocks","FrozenBalance","FrozenStocks","InitBalance","InitStocks","InitNET","NET","Profit","UpdateTimes"]
exchangesdfcolumnsnameshow=["名字","延时","买","卖","现金","货币","冻金","冻币","初金","初币","初资产","现资产","浮盈","更新次数"]
starttime=0#策略开始时间
exchangesdiffcolumnsname=["KeyName","ABuy","ASell","BBuy","BSell","InDiff","OutDiff","InMax","OutMin","InAvg","OutAvg","Go"]
exchangesdiffcolumnsnameshow=["交易对","左买","左卖","右买","右卖","进差","出差","进最大","出最小","进平均","出平均","信号"]

tradetasklistcolumnsname=["KeyName","Ae","Be","Status","ABuy","ASell","BBuy","BSell","InSell","InBuy","OutBuy","OutSell","Profit","CompleteCount"]
tradetasklistcolumnsnameshow=["交易对","左","右","左买","左卖","右买","右卖","状态","进买","进卖","出买","出卖","盈利","赢/输/总"]

histradelistcolumnsnameshow=["左","右","进","出","进买价","进买量","进卖价","进卖量","出买价","出买量","出卖价","出卖量","进盈利","出盈利","进费","出费","净盈利"]
histradelist=[]
tradinglist={} #保存当前正在进行的进场交易
EMPair={}
tradepairdict={}
Status={"up":[],"tables":[],"bottom":[]}
def init():#初始化 完成多个df 表格的格式化工作，该方案导致无法重复进场
    #SetErrorFilter("GetAccount|GetDepth|GetTicker|GetRecords|GetTrades|GetOrders|SetContractType")
    _CDelay(300)
    LogReset()
    LogProfitReset()
    starttime=time.time()
    exchangesnamelist = [_C(e.GetName) for e in exchanges] #取得交易所名称列表
    for i in exchangesnamelist:
        for j in exchangesnamelist:
            if i!=j:
                keyname=i+":"+j
                exchangespairlnamelist.append(i+":"+j)
                EMPair[keyname]={"InDiffPrice":[],"OutDiffPrice":[],"TradePair":{"InDiffPrice":0,"InAmount":0,"InFee":0}}

    exchangesdf=pa.DataFrame(columns=exchangesdfcolumnsname,index=exchangesnamelist)
    exchangesdiffdf=pa.DataFrame(columns=exchangesdiffcolumnsname,index=exchangespairlnamelist)
    tradetasklistdf=pa.DataFrame(columns=tradetasklistcolumnsname,index=exchangespairlnamelist)
    tradetasklistdf.append(exchangespairlnamelist)
    tradetasklistdf=tradetasklistdf.fillna(value=0)



def GetHisTable():
    str={'type': 'table', 'title': '历史记录', 'cols': [],'rows':[]}
    str["cols"]=histradelistcolumnsnameshow
    rows=[]
    for i in range(len(histradelist)):
        row=[]
        row.append(histradelist[i].AeName)
        row.append(histradelist[i].BeName)
        row.append(_D(histradelist[i].InTime))
        row.append(_D(histradelist[i].OutTime))

        row.append(round(histradelist[i].InBuyPrice,4))
        row.append(round(histradelist[i].InBuyAmount,4))
        row.append(round(histradelist[i].InSellPrice,4))
        row.append(round(histradelist[i].InSellAmount,4))
        row.append(round(histradelist[i].OutBuyPrice,4))
        row.append(round(histradelist[i].OutBuyAmount,4))
        row.append(round(histradelist[i].OutSellPrice,4))
        row.append(round(histradelist[i].OutSellAmount,4))
        row.append(round(histradelist[i].InProfit,4))
        row.append(round(histradelist[i].OutProfit,4))
        row.append(round(histradelist[i].InFee,4))
        row.append(round(histradelist[i].OutFee,4))
        row.append(round(histradelist[i].Profit,4))
        rows.append(row)
    str["rows"]=rows
    return str
def GetInFieldTable():
    str={'type': 'table', 'title': '在场监控', 'cols': [],'rows':[]}
    str["cols"]=histradelistcolumnsnameshow
    rows=[]
    for i in tradinglist:
        row=[]
        row.append(tradinglist[i].AeName)
        row.append(tradinglist[i].BeName)
        row.append(_D(tradinglist[i].InTime))
        if tradinglist[i].OutTime>0:
            row.append(_D(tradinglist[i].OutTime))
        else:
            row.append(tradinglist[i].OutTime)
        row.append(round(tradinglist[i].InBuyPrice,4))
        row.append(round(tradinglist[i].InBuyAmount,4))
        row.append(round(tradinglist[i].InSellPrice,4))
        row.append(round(tradinglist[i].InSellAmount,4))
        row.append(round(tradinglist[i].OutBuyPrice,4))
        row.append(round(tradinglist[i].OutBuyAmount,4))
        row.append(round(tradinglist[i].OutSellPrice,4))
        row.append(round(tradinglist[i].OutSellAmount,4))
        row.append(round(tradinglist[i].InProfit,4))
        row.append(round(tradinglist[i].OutProfit,4))
        row.append(round(tradinglist[i].InFee,4))
        row.append(round(tradinglist[i].OutFee,4))
        row.append(round(tradinglist[i].Profit,4))
        rows.append(row)
    str["rows"]=rows
    return str
def main():

    runtime=0
    Log("系统启动，初始化市场...")
    em=ExchangeManage()
    while abs(em.NETDiff)<abs(MaxLost):#双向终止操作，盈利和损失都计入
        runtime+=1
        Status["bottom"]=[]
        Status["up"]=[]
        Status["tables"]=[]
        try:
        #if True:
            times=time.time()
            em.Update()
            timeet=time.time()
            # if runtime>100:
            #     if em.CheckOut():
            #         continue
            #     if em.CheckIn():
            #         continue
            #     em.CheckGoingIn()




            timee=time.time()
            Status["up"].append(em.GetAdvExcInfo())
            Status["bottom"].append("访问次数:"+str(em.UpdateTime))
            Status["tables"].append(ext.GetTableDict(exchangesdf,"交易所",exchangesdfcolumnsnameshow))
            Status["tables"].append(ext.GetTableDict(exchangesdiffdf,"信号监控",exchangesdiffcolumnsnameshow))
            Status["tables"].append(ext.GetTableDict(tradetasklistdf,"交易监控",tradetasklistcolumnsnameshow))
            Status["tables"].append(GetInFieldTable())
            Status["tables"].append(GetHisTable())
            Status["bottom"].append(em.CalProfit())
            timeeu=time.time()
            TDelay=int((timeet-times)*1000)#取得更新延迟
            TDelayT=int((timee-timeet)*1000)#取得交易延迟
            TDelayU=int((timeeu-timee)*1000)#取得显示延迟

            Status["bottom"].append("发起更新时间:"+time.strftime("%Y-%m-%d %X", time.localtime(times))+" 更新延迟:"+str(TDelay)+"ms"+" 交易延迟:"+str(TDelayT)+"ms"+" 显示延迟:"+str(TDelayU)+"ms")

            LogStatus(ext.GetStrFromList(Status["up"]),'\n`'+json.dumps(Status["tables"])+'`\n',ext.GetStrFromList(Status["bottom"]))
        except Exception,e:
            Log("err:",e,traceback.print_exc())
        Sleep(300)
    if em.NETDiff>0:
        Log("系统判断止盈终止条件，结束交易...@")
    else:
        Log("系统判断止损终止条件，结束交易...@")

class TradePair:
    def __init__(self):
        self.ID=0
        self.AeName=""
        self.BeName=""
        self.InTime=0
        self.OutTime=0
        self.InSellPrice=0
        self.InBuyPrice=0
        self.InSellAmount=0
        self.InBuyAmount=0
        self.InFee=0
        self.InProfit=0
        self.OutSellPrice=0
        self.OutBuyPrice=0
        self.OutSellAmount=0
        self.OutBuyAmount=0
        self.OutFee=0
        self.OutProfit=0
        self.Profit=0
        self.InDiffPrice=0
        self.OutDiffPrice=0
        self.InSellCheck=False
        self.InBuyCheck=False
        self.OutSellCheck=False
        self.OutBuyCheck=False
        self.WinTimes=0
        self.LoseTimes=0

#交易市场管理类，设计一个趋势判断

class ExchangeManage:
    def __init__(self):
        self.EM={}
        self.UpdateTime=1
        self.exchangenames=[]
        self.EMPair={}
        self.TotalInAmount=0
        self.Profit=0
        self.NETDiff=0
        self.TotalFee=0
        self.NoOpNET=0#无操作
        self.TotalInitBalance=0
        self.TotalInitStocks=0
        self.TotalInitNET=0
        try:
            while True:
                self.TotalInitBalance=0
                self.TotalInitStocks=0
                self.TotalInitNET=0
                for i in range(len(exchanges)):
                    ex=AdvExchange(exchanges[i])
                    self.EM[ex.Name]=ex
                    self.TotalInitBalance+=self.EM[ex.Name].InitBalance
                    self.TotalInitStocks+=self.EM[ex.Name].InitStocks
                    self.TotalInitNET+=self.EM[ex.Name].InitNET
                    self.exchangenames.append(ex.Name)
                if len(exchanges)==len(self.EM):
                    break
        except Exception,e:
            Log("err:",e,traceback.print_exc())
    def Update(self):
        threadpool=[]
        for (d,x) in self.EM.items():
            t=threading.Thread(target=x.UpdateDepth,name=x.Name,args=(self.UpdateTime,))
            threadpool.append(t)
        for t in threadpool:
            t.start()
        for t in threadpool:
            t.join()
        #for (d,x) in self.EM.items():
        #    self.EM[d].UpdateDepth(self.UpdateTime)
        self.UpdateTime+=1
        self.GetExchangeDiff()
    def GetExchangeDiff(self):
        for i in self.EM:
            InDiffPrice = 0
            InMax = InDiffPrice
            InAvg = InDiffPrice
            OutMin = InDiffPrice
            OutAvg = InDiffPrice
            Ins = InDiffPrice
            Outs = InDiffPrice
            ABuy=0
            ASell=0
            BBuy=0
            BSell=0
            Go=0
            for j in self.EM:
                if i != j:
                    keyname = self.EM[i].Name + ":" + self.EM[j].Name
                    InDiffPrice = ext.Cut((self.EM[i].Buy.Price - self.EM[j].Sell.Price),3)  # 进场差价 在高价市场卖出，在低价市场买入 对应A B市场，A->B 进场 A卖出 B买入
                    OutDiffPrice = ext.Cut((self.EM[j].Buy.Price-self.EM[i].Sell.Price),3)    # 出场差价 A买入 B卖出
                    InAmount = self.EM[i].Buy.Amount#进场量
                    OutAmount = self.EM[j].Buy.Amount#出场量
                    ABuy=self.EM[i].Buy.Price
                    ASell=self.EM[i].Sell.Price
                    BBuy=self.EM[j].Buy.Price
                    BSell=self.EM[j].Sell.Price
                    if self.UpdateTime<2 :
                        InMax = InDiffPrice
                        InAvg = InDiffPrice
                        OutMin = InDiffPrice
                        OutAvg = InDiffPrice
                        Ins = InDiffPrice
                        Outs = InDiffPrice
                        Go=0
                    else:
                        # Log(LDF)
                        div=self.UpdateTime
                        if self.UpdateTime<WatchWindow:
                            EMPair[keyname]["InDiffPrice"].append(InDiffPrice)
                            EMPair[keyname]["OutDiffPrice"].append(OutDiffPrice)
                        else:
                            div=WatchWindow
                            EMPair[keyname]["InDiffPrice"]=copy.copy(EMPair[keyname]["InDiffPrice"][1:])
                            EMPair[keyname]["OutDiffPrice"]=copy.copy(EMPair[keyname]["OutDiffPrice"][1:])
                            EMPair[keyname]["InDiffPrice"].append(InDiffPrice)
                            EMPair[keyname]["OutDiffPrice"].append(OutDiffPrice)
                        InMax =  max(EMPair[keyname]["InDiffPrice"])
                        OutMin =  min(EMPair[keyname]["OutDiffPrice"])
                        InAvg = ext.Cut((sum(EMPair[keyname]["InDiffPrice"]) / div),2)
                        OutAvg = ext.Cut((sum(EMPair[keyname]["OutDiffPrice"])/ div),2)
                        Ins = ext.Cut((InMax+InAvg)/InST,2) #根据进场门限设定进场信号门限  进场最大与进场平均 除以系数
                        if (InDiffPrice>0 and (InDiffPrice-OutMin>0)):
                            Go=ext.Cut(((InDiffPrice-Ins)*(InDiffPrice-OutMin)),3)
                        else:
                            Go=0
                        if Go<0:
                            Go=0
                        if keyname in tradetasklistdf.index and tradetasklistdf.loc[keyname]["Status"]!="waiting in":
                            Go=0
                    exchangesdiffdf.ix[keyname, 'KeyName'] = keyname
                    exchangesdiffdf.ix[keyname, 'ABuy'] = ABuy
                    exchangesdiffdf.ix[keyname, 'ASell'] = ASell
                    exchangesdiffdf.ix[keyname, 'BBuy'] = BBuy
                    exchangesdiffdf.ix[keyname, 'BSell'] = BSell
                    exchangesdiffdf.ix[keyname, 'InDiff'] = InDiffPrice
                    exchangesdiffdf.ix[keyname, 'OutDiff'] = OutDiffPrice
                    exchangesdiffdf.ix[keyname, 'InMax'] = InMax
                    exchangesdiffdf.ix[keyname, 'OutMin'] = OutMin
                    exchangesdiffdf.ix[keyname, 'InAvg'] = InAvg
                    exchangesdiffdf.ix[keyname, 'OutAvg'] = OutAvg
                    exchangesdiffdf.ix[keyname, 'GO'] = Go


                    tradetasklistdf.ix[keyname,"KeyName"]=keyname
                    tradetasklistdf.ix[keyname,"Ae"]=self.EM[i].Name+":"+str(self.EM[i].GetDepthDelay)
                    tradetasklistdf.ix[keyname,"Be"]=self.EM[j].Name+":"+str(self.EM[j].GetDepthDelay)

                    tradetasklistdf.ix[keyname,"ABuy"]=self.EM[i].Buy.Price
                    tradetasklistdf.ix[keyname,"ASell"]=self.EM[i].Sell.Price
                    tradetasklistdf.ix[keyname,"BBuy"]=self.EM[j].Buy.Price
                    tradetasklistdf.ix[keyname,"BSell"]=self.EM[i].Sell.Price
                    if self.UpdateTime<3:
                        tradetasklistdf.ix[keyname,"Status"]="waiting in"
                        tradetasklistdf.ix[keyname,"InBuy"]="--"
                        tradetasklistdf.ix[keyname,"InSell"]="--"
                        tradetasklistdf.ix[keyname,"OutBuy"]="--"
                        tradetasklistdf.ix[keyname,"OutSell"]="--"
                        tradetasklistdf.ix[keyname,"Profit"]=0
                        tradetasklistdf.ix[keyname,"CompleteCount"]=0
    def CheckOut(self):#出场检测
        df=tradetasklistdf.ix[tradetasklistdf["Status"]=="waiting out"]
        ret=False
        if df is not None and len(df.index)>0:
            for index in range(len(df.index)):
                keyname=df.ix[index, 'KeyName']
                i=keyname.split(":")[0]
                j=keyname.split(":")[1]
                InDiffPrice=tradinglist[keyname].InDiffPrice
                AlreadyOutSellAmount=tradinglist[keyname].OutSellAmount #取得已出场数额
                AlreadyOutSellPrice=tradinglist[keyname].OutSellPrice #取得出场卖价
                AlreadyOutBuyAmount=tradinglist[keyname].OutBuyAmount#取得已出场买入数额
                AlreadyOutBuyPrice=tradinglist[keyname].OutBuyPrice #取得出场买入价格

                AlreadyInSellAmount=tradinglist[keyname].InSellAmount #取得已出场数额
                AlreadyInBuyAmount=tradinglist[keyname].InBuyAmount#取得已出场买入数额
                shouldoutamount=min(AlreadyInSellAmount,AlreadyInBuyAmount)

                DiffOutSellAmount=round((shouldoutamount-AlreadyOutSellAmount),5) #出场卖出需要的数量
                DiffOutBuyAmount=round((shouldoutamount-AlreadyOutBuyAmount),5) #出场买入需要的数量
                #是否进行出场交易 需要判断 是否存在盈利的可能
                amount=max(AlreadyOutSellAmount,AlreadyOutBuyAmount)
                MayOutProfit=0
                MayOutFee=0

                if amount==0:
                    amount=MaxCoinOnce
                    MayOutFee=amount*self.EM[i].Sell.Price*self.EM[i].Fee.Sell/100+amount*self.EM[j].Buy.Price*self.EM[j].Fee.Buy/100
                    MayOutProfit=((MaxCoinOnce*self.EM[j].Buy.Price)-(MaxCoinOnce*self.EM[i].Sell.Price))-MayOutFee
                else:
                    MayOutFee=amount*self.EM[i].Sell.Price*self.EM[i].Fee.Sell/100+amount*self.EM[j].Buy.Price*self.EM[j].Fee.Buy/100
                    MayOutProfit=AlreadyOutSellAmount*AlreadyOutSellPrice+(amount-AlreadyOutSellAmount)*self.EM[j].Buy.Price-AlreadyOutBuyAmount*AlreadyOutBuyPrice-(amount-AlreadyOutBuyAmount)*self.EM[i].Sell.Price-MayOutFee

                    #MayOutProfit=((amount*tradinglist[keyname].OutSellPrice+(MaxCoinOnce-amount)*self.EM[j].Buy.Price)-(amount*tradinglist[keyname].OutBuyPrice+(MaxCoinOnce-amount)*self.EM[i].Sell.Price))-MayOutFee
                #MayOutFee=MaxCoinOnce*self.EM[i].Sell.Price*self.EM[i].Fee.Sell/100+MaxCoinOnce*self.EM[j].Buy.Price*self.EM[j].Fee.Buy/100

                if tradinglist[keyname].OutDiffPrice==0:
                    tradinglist[keyname].OutDiffPrice=exchangesdiffdf.ix[keyname, 'OutDiff']
                OutDiffPrice=exchangesdiffdf.ix[keyname, 'OutDiff'] #获得出场价差


            #Log("INS:",tradinglist[keyname].InSellPrice,"INB",tradinglist[keyname].InBuyPrice,"InDiffPrice:",InDiffPrice,"OutDiffPrice:",OutDiffPrice,"MayOutProfit:",MayOutProfit)
                if tradinglist[keyname].InProfit+MayOutProfit>0.1:#总盈利大于1毛就搞


                    if tradinglist[keyname].OutDiffPrice==0:
                        tradinglist[keyname].OutDiffPrice=OutDiffPrice

                    InAmount=self.EM[j].Buy.Amount
                    OutBuyAmount=self.EM[i].Sell.Amount

                    SellErrstr=""
                    SellAmount=min(DiffOutSellAmount,InAmount)
                    if self.EM[j].Stocks<SellAmount:
                        SellErrstr+="【缺币啦】市场:"+self.EM[j].Name
                    #if AlreadyOutSellPrice>0 and self.EM[j].Buy.Price<AlreadyOutSellPrice:
                    #    SellErrstr+="出场卖价小于已入场卖价！"
                    #if AlreadyOutBuyPrice>0:
                    #    if self.EM[j].Buy.Price-AlreadyOutBuyPrice<tradinglist[keyname].OutDiffPrice:
                    #         SellErrstr+="【出场】入场卖价与已入场买价，差价不符合要求！"




                    BuyErrstr=""
                    BuyAmount=min(DiffOutBuyAmount,OutBuyAmount)
                    canbyamount=ext.Cut(self.EM[i].Balance/self.EM[i].Sell.Price,4)
                    if canbyamount<BuyAmount:
                         BuyErrstr+="【缺钱啦】市场:"+self.EM[i].Name
                    #if AlreadyOutBuyPrice>0 and self.EM[i].Sell.Price>AlreadyOutBuyPrice:
                    #     BuyErrstr+="出场买价大于已入场买价！"
                    #if AlreadyOutSellPrice>0:
                    #    if AlreadyOutSellPrice-self.EM[i].Sell.Price<tradinglist[keyname].OutDiffPrice:
                    #         BuyErrstr+="【出场】买价与已入场卖价，差价不符合要求！"
                    if len(SellErrstr)>0 or len(BuyErrstr)>0:
                        Log("SellErrstr",SellErrstr,"BuyErrstr",BuyErrstr)
                        return
                    noweastocks=self.EM[i].Stocks
                    nowebstocks=self.EM[j].Stocks#交易前两个市场的币
                    Log("InProfit",tradinglist[keyname].InProfit,"OutDiffPrice",OutDiffPrice,"MayOutProfit",MayOutProfit,"amount:",amount,"SellAmount",SellAmount,"BuyAmount",BuyAmount,"MayOutFee",MayOutFee)
                    #如果通过了检查 则进行交易
                    if  SellAmount>=self.EM[j].MinStock:
                        Log("准备【出场-卖出】市场：",self.EM[j].Name,"Sell At:",self.EM[j].Buy.Price,"Amount:",SellAmount,"AlreadyOutSellPrice",AlreadyOutSellPrice,"AlreadyOutBuyPrice",AlreadyOutBuyPrice,"AlreadyOutSellAmount",AlreadyOutSellAmount,"AlreadyOutBuyAmount",AlreadyOutBuyAmount)
                        sorderid=self.EM[j].e.Sell(self.EM[j].Buy.Price,SellAmount)
                        ret=True
                    if  BuyAmount>=self.EM[i].MinStock:
                        Log("准备【出场-买入】市场：",self.EM[i].Name,"Buy At:",self.EM[i].Sell.Price,"Amount:",BuyAmount,"AlreadyOutSellPrice",AlreadyOutSellPrice,"AlreadyOutBuyPrice",AlreadyOutBuyPrice,"AlreadyOutSellAmount",AlreadyOutSellAmount,"AlreadyOutBuyAmount",AlreadyOutBuyAmount)
                    #self.EM[i].e.Buy(self.EM[i].Sell.Price,BuyAmount/(1-self.EM[i].Fee.Buy/100))
                        borderid=self.EM[i].e.Buy(self.EM[i].Sell.Price,BuyAmount/(1-self.EM[i].Fee.Buy/94))
                        ret=True
                #休眠100毫秒
                    if not ret:
                        continue
                # 检查账户成交
                    eatradeamount=ebtradeamount=0
                    if SellAmount>=self.EM[j].MinStock:

                        ebtradeamount=self.EM[j].CheckTrade(SellAmount,sorderid,"s")
                        if(ebtradeamount+tradinglist[keyname].OutSellAmount)>0:
                            tradinglist[keyname].OutSellPrice=round(((tradinglist[keyname].OutSellAmount*tradinglist[keyname].OutSellPrice+ebtradeamount*self.EM[j].Buy.Price)/(tradinglist[keyname].OutSellAmount+ebtradeamount)),2)
                        tradinglist[keyname].OutSellAmount=round(tradinglist[keyname].OutSellAmount+ebtradeamount,5)
                        Log("检查",self.EM[j].Name,"【出场-卖出】原币数",nowebstocks,"当前币数",self.EM[j].Stocks,"成交数",ebtradeamount,"OutSellPrice",tradinglist[keyname].OutSellPrice,"OutSellAmount", tradinglist[keyname].OutSellAmount)

                    if BuyAmount>=self.EM[i].MinStock:

                        eatradeamount=self.EM[i].CheckTrade(BuyAmount,borderid,"b")

                        if(eatradeamount+tradinglist[keyname].OutBuyAmount)>0:
                            tradinglist[keyname].OutBuyPrice=round(((tradinglist[keyname].OutBuyAmount*tradinglist[keyname].OutBuyPrice+eatradeamount*self.EM[i].Sell.Price)/(tradinglist[keyname].OutBuyAmount+eatradeamount)),2)
                        tradinglist[keyname].OutBuyAmount=round(tradinglist[keyname].OutBuyAmount+eatradeamount,5)
                        Log("检查",self.EM[i].Name,"【出场-买入】原币数",noweastocks,"当前币数",self.EM[i].Stocks,"成交数",eatradeamount,"OutBuyPrice",tradinglist[keyname].OutBuyPrice,"OutBuyAmount",tradinglist[keyname].OutBuyAmount)
                    if tradinglist[keyname].OutSellAmount<MaxCoinOnce:
                        tradetasklistdf.ix[keyname,"OutSell"]="in"
                    elif tradinglist[keyname].OutSellAmount==MaxCoinOnce:
                        tradetasklistdf.ix[keyname,"OutSell"]="ok"
                    if tradinglist[keyname].OutBuyAmount<MaxCoinOnce:
                        tradetasklistdf.ix[keyname,"OutBuy"]="in"
                    elif tradinglist[keyname].OutBuyAmount==MaxCoinOnce:
                        tradetasklistdf.ix[keyname,"OutBuy"]="ok"

                    minout=min(tradinglist[keyname].OutSellAmount,tradinglist[keyname].OutBuyAmount)



                    if tradinglist[keyname].OutSellAmount>=MaxCoinOnce and tradinglist[keyname].OutBuyAmount>=MaxCoinOnce:
                        tradetasklistdf.ix[keyname,"Status"]="waiting in"
                        fee=(tradinglist[keyname].OutSellAmount*tradinglist[keyname].OutSellPrice)*self.EM[j].Fee.Sell/100+ tradinglist[keyname].OutBuyPrice*( tradinglist[keyname].OutBuyAmount/(1-self.EM[i].Fee.Buy/100))*self.EM[i].Fee.Buy/100
                        tradinglist[keyname].OutFee=ext.Cut(fee,4)
                        tradinglist[keyname].OutProfit=tradinglist[keyname].OutSellPrice*tradinglist[keyname].OutSellAmount-tradinglist[keyname].OutBuyPrice*tradinglist[keyname].OutBuyAmount-tradinglist[keyname].OutFee
                        tradinglist[keyname].OutProfit=ext.Cut(tradinglist[keyname].OutProfit,2)
                        tradinglist[keyname].Profit= tradinglist[keyname].InProfit+ tradinglist[keyname].OutProfit
                        if tradinglist[keyname].Profit>=0:
                            tradinglist[keyname].WinTimes+=1
                        else:
                            tradinglist[keyname].LoseTimes+=1
                        self.Profit+=tradinglist[keyname].Profit
                        LogProfit(ext.Cut(self.Profit,2))
                        #Log("盈利:",ext.Cut(self.Profit,2),"@")
                        tradinglist[keyname].OutTime=time.time()
                        tradetasklistdf.ix[keyname,"Status"]="waiting in"
                        tradetasklistdf.ix[keyname,"InBuy"]="--"
                        tradetasklistdf.ix[keyname,"InSell"]="--"
                        tradetasklistdf.ix[keyname,"OutBuy"]="--"
                        tradetasklistdf.ix[keyname,"OutSell"]="--"
                        tradetasklistdf.ix[keyname,"Profit"]+=tradinglist[keyname].Profit
                        tradetasklistdf.ix[keyname,"CompleteCount"]=str(tradinglist[keyname].WinTimes)+"|"+str(tradinglist[keyname].LoseTimes)+"|"+str(tradinglist[keyname].WinTimes+tradinglist[keyname].LoseTimes)
                        histradelist.append(copy.copy(tradinglist[keyname]))
                        self.TotalFee+=tradinglist[keyname].InFee+tradinglist[keyname].OutFee
                        tradinglist[keyname].InTime=0
                        tradinglist[keyname].OutTime=0
                        tradinglist[keyname].InSellPrice=0
                        tradinglist[keyname].InBuyPrice=0
                        tradinglist[keyname].InSellAmount=0
                        tradinglist[keyname].InBuyAmount=0
                        tradinglist[keyname].InFee=0
                        tradinglist[keyname].InProfit=0
                        tradinglist[keyname].OutSellPrice=0
                        tradinglist[keyname].OutBuyPrice=0
                        tradinglist[keyname].OutSellAmount=0
                        tradinglist[keyname].OutBuyAmount=0
                        tradinglist[keyname].OutFee=0
                        tradinglist[keyname].OutProfit=0
                        tradinglist[keyname].Profit=0
                        tradinglist[keyname].OutDiffPrice=0
                        tradinglist[keyname].InDiffPrice=0
                    if minout>0 and minout<tradinglist[keyname].InSellAmount:
                        tradetasklistdf.ix[keyname,"Status"]="waiting out"
                        fee=(minout*tradinglist[keyname].OutSellPrice)*self.EM[j].Fee.Sell/100+ tradinglist[keyname].OutBuyPrice*(minout/(1-self.EM[i].Fee.Buy/100))*self.EM[i].Fee.Buy/100
                        tradinglist[keyname].OutFee=ext.Cut(fee,4)
                        tradinglist[keyname].OutProfit=tradinglist[keyname].OutSellPrice*minout-tradinglist[keyname].OutBuyPrice*minout-tradinglist[keyname].OutFee
                        tradinglist[keyname].OutProfit=ext.Cut(tradinglist[keyname].OutProfit,2)
                        tradinglist[keyname].Profit= tradinglist[keyname].InProfit+ tradinglist[keyname].OutProfit
                        if tradinglist[keyname].Profit>=0:
                            tradinglist[keyname].WinTimes+=1
                        else:
                            tradinglist[keyname].LoseTimes+=1
                        self.Profit+=tradinglist[keyname].Profit
                        LogProfit(ext.Cut(self.Profit,2))
                        #Log("盈利:",ext.Cut(self.Profit,2),"@")
                        tradinglist[keyname].OutTime=time.time()
                        tradetasklistdf.ix[keyname,"Status"]="waiting out"
                        tradetasklistdf.ix[keyname,"InBuy"]="ok"
                        tradetasklistdf.ix[keyname,"InSell"]="ok"
                        tradetasklistdf.ix[keyname,"OutBuy"]="--"
                        tradetasklistdf.ix[keyname,"OutSell"]="--"
                        tradetasklistdf.ix[keyname,"Profit"]+=tradinglist[keyname].Profit
                        tradetasklistdf.ix[keyname,"CompleteCount"]=str(tradinglist[keyname].WinTimes)+"|"+str(tradinglist[keyname].LoseTimes)+"|"+str(tradinglist[keyname].WinTimes+tradinglist[keyname].LoseTimes)

                        self.TotalFee+=tradinglist[keyname].InFee+tradinglist[keyname].OutFee
                        #tradinglist[keyname].InTime=0
                        tradinglist[keyname].OutTime=time.time()
                        #tradinglist[keyname].InSellPrice=0
                        #tradinglist[keyname].InBuyPrice=0
                        tradinglist[keyname].InSellAmount=tradinglist[keyname].InSellAmount-minout
                        tradinglist[keyname].InBuyAmount=tradinglist[keyname].InBuyAmount-minout
                        tradinglist[keyname].InFee=tradinglist[keyname].InFee-tradinglist[keyname].InFee*(minout/tradinglist[keyname].InSellAmount)
                        tradinglist[keyname].InProfit=tradinglist[keyname].InProfit-tradinglist[keyname].InProfit*(minout/tradinglist[keyname].InSellAmount)
                        tradinglist[keyname].OutSellAmount=tradinglist[keyname].OutSellAmount-minout
                        tradinglist[keyname].OutBuyAmount=tradinglist[keyname].OutBuyAmount-minout
                        if tradinglist[keyname].OutSellAmount==0:
                            tradinglist[keyname].OutSellPrice=0
                        if tradinglist[keyname].OutBuyAmount==0:
                            tradinglist[keyname].OutBuyPrice=0
                        histradelist.append(copy.copy(tradinglist[keyname]))
                        tradinglist[keyname].OutFee=0
                        tradinglist[keyname].OutProfit=0
                        tradinglist[keyname].Profit=0
                        tradinglist[keyname].OutDiffPrice=0

                        histradelist[len(histradelist)-1].InSellAmount=minout
                        histradelist[len(histradelist)-1].InBuyAmount=minout
                        histradelist[len(histradelist)-1].OutSellAmount=minout
                        histradelist[len(histradelist)-1].OutBuyAmount=minout
                    if minout==tradinglist[keyname].InSellAmount:
                        tradetasklistdf.ix[keyname,"Status"]="waiting in"
                        fee=(tradinglist[keyname].OutSellAmount*tradinglist[keyname].OutSellPrice)*self.EM[j].Fee.Sell/100+ tradinglist[keyname].OutBuyPrice*( tradinglist[keyname].OutBuyAmount/(1-self.EM[i].Fee.Buy/100))*self.EM[i].Fee.Buy/100
                        tradinglist[keyname].OutFee=ext.Cut(fee,4)
                        tradinglist[keyname].OutProfit=tradinglist[keyname].OutSellPrice*tradinglist[keyname].OutSellAmount-tradinglist[keyname].OutBuyPrice*tradinglist[keyname].OutBuyAmount-tradinglist[keyname].OutFee
                        tradinglist[keyname].OutProfit=ext.Cut(tradinglist[keyname].OutProfit,2)
                        tradinglist[keyname].Profit= tradinglist[keyname].InProfit+ tradinglist[keyname].OutProfit
                        if tradinglist[keyname].Profit>=0:
                            tradinglist[keyname].WinTimes+=1
                        else:
                            tradinglist[keyname].LoseTimes+=1
                        self.Profit+=tradinglist[keyname].Profit
                        LogProfit(ext.Cut(self.Profit,2))
                        #Log("盈利:",ext.Cut(self.Profit,2),"@")
                        tradinglist[keyname].OutTime=time.time()
                        tradetasklistdf.ix[keyname,"Status"]="waiting in"
                        tradetasklistdf.ix[keyname,"InBuy"]="--"
                        tradetasklistdf.ix[keyname,"InSell"]="--"
                        tradetasklistdf.ix[keyname,"OutBuy"]="--"
                        tradetasklistdf.ix[keyname,"OutSell"]="--"
                        tradetasklistdf.ix[keyname,"Profit"]+=tradinglist[keyname].Profit
                        tradetasklistdf.ix[keyname,"CompleteCount"]=str(tradinglist[keyname].WinTimes)+"|"+str(tradinglist[keyname].LoseTimes)+"|"+str(tradinglist[keyname].WinTimes+tradinglist[keyname].LoseTimes)
                        histradelist.append(copy.copy(tradinglist[keyname]))
                        self.TotalFee+=tradinglist[keyname].InFee+tradinglist[keyname].OutFee
                        tradinglist[keyname].InTime=0
                        tradinglist[keyname].OutTime=0
                        tradinglist[keyname].InSellPrice=0
                        tradinglist[keyname].InBuyPrice=0
                        tradinglist[keyname].InSellAmount=0
                        tradinglist[keyname].InBuyAmount=0
                        tradinglist[keyname].InFee=0
                        tradinglist[keyname].InProfit=0
                        tradinglist[keyname].OutSellPrice=0
                        tradinglist[keyname].OutBuyPrice=0
                        tradinglist[keyname].OutSellAmount=0
                        tradinglist[keyname].OutBuyAmount=0
                        tradinglist[keyname].OutFee=0
                        tradinglist[keyname].OutProfit=0
                        tradinglist[keyname].Profit=0
                        tradinglist[keyname].OutDiffPrice=0
                        tradinglist[keyname].InDiffPrice=0
                        #tradinglist[keyname].InDiffPrice=0
        return ret

    def CheckIn(self):#进场检测
        df=exchangesdiffdf.loc[exchangesdiffdf["GO"]>GoT]
        #Log(df)
        ret=False
        if df is not None and len(df.index)>0:
            Go=df.head()['GO'].values
            if len(Go)==0:
                return
            Go=Go[0]
            keyname=df.ix[0, 'KeyName']
            i=keyname.split(":")[0]
            j=keyname.split(":")[1]
            InDiffPrice=df.ix[0, 'InDiff']
            if InDiffPrice<MaxCoinOnce*0.004*self.EM[i].Buy.Price:#进场差价太小
                #Log("进场差价倒挂:",InDiffPrice)
                return
            OutAvg=df.ix[0, 'OutAvg']
            #检测反向市场入场情况exchangesdiffdf
            if tradetasklistdf.loc[j+":"+i]["Status"]!="waiting in":
                #Log("存在反向在场交易对儿！滚！")
                return
            if not(keyname in tradinglist): #监控中的交易列表没有这个 就加上，属于新建
                tradinglist[keyname]=TradePair()
            #if tradetasklistdf.loc[keyname]["Status"]=="waiting in" or tradetasklistdf.loc[keyname]["Status"]=="going in":#等待入场 或是 已完成部分入场
            if tradetasklistdf.loc[keyname]["Status"]=="waiting in":#等待入场 或是 已完成部分入场
                if tradetasklistdf.loc[keyname]["Status"]=="waiting in":
                    tradinglist[keyname].InDiffPrice=InDiffPrice
                    tradinglist[keyname].InTime=time.time()
                    tradetasklistdf.ix[keyname,"Status"]="going in"
                    tradinglist[keyname].AeName=i
                    tradinglist[keyname].BeName=j
                AlreadyInSellAmount=tradinglist[keyname].InSellAmount #取得已进场数额
                AlreadyInSellPrice=tradinglist[keyname].InSellPrice #取得进场卖价
                AlreadyInBuyAmount=tradinglist[keyname].InBuyAmount#取得已进场买入数额
                AlreadyInBuyPrice=tradinglist[keyname].InBuyPrice #取得进场买入价格
                DiffInSellAmount=round((MaxCoinOnce-AlreadyInSellAmount),5) #进场卖出需要的数量
                DiffInBuyAmount=round((MaxCoinOnce-AlreadyInBuyAmount),5) #进场买入需要的数量
                InAmount=self.EM[i].Buy.Amount   #Ea 市场买入份额，高价卖出对应的市场深度
                OutBuyAmount=self.EM[j].Sell.Amount #Eb 市场卖出的份额，低价买入对应的市场深度

                SellErrstr=""
                SellAmount=min(DiffInSellAmount,InAmount)

                if AlreadyInSellPrice>0 and self.EM[i].Buy.Price<AlreadyInSellPrice:
                    SellErrstr+="入场卖价小于已入场卖价！"
                if AlreadyInBuyPrice>0:
                    if self.EM[i].Buy.Price-AlreadyInBuyPrice<tradinglist[keyname].InDiffPrice:
                         SellErrstr+="入场卖价与已入场买价，差价不符合要求！"

                BuyErrstr=""
                BuyAmount=min(DiffInBuyAmount,OutBuyAmount)

                if AlreadyInBuyPrice>0 and self.EM[j].Sell.Price>AlreadyInBuyPrice:
                     BuyErrstr+="入场买价大于已入场买价！"
                if AlreadyInBuyPrice==0 and AlreadyInSellPrice>0:
                    if AlreadyInSellPrice-self.EM[j].Sell.Price<tradinglist[keyname].InDiffPrice:
                         BuyErrstr+="入场买价与已入场卖价，差价不符合要求！"

                if len(SellErrstr)>0 or len(BuyErrstr)>0:
                    #Log("SellErrstr",SellErrstr,"BuyErrstr",BuyErrstr)
                    return

                noweastocks=self.EM[i].Stocks
                nowebstocks=self.EM[j].Stocks#交易前两个市场的币

                #如果通过了检查 则进行交易
                if SellAmount>=self.EM[i].MinStock:
                    Log("准备【入场-卖出】市场：",self.EM[i].Name,"Sell At:",self.EM[i].Buy.Price,"Amount:",SellAmount,"AlreadyInSellPrice",AlreadyInSellPrice,"AlreadyInBuyPrice",AlreadyInBuyPrice)
                    sorderid=self.EM[i].e.Sell(self.EM[i].Buy.Price,SellAmount)
                    ret=True
                if BuyAmount>=self.EM[j].MinStock:
                    Log("准备【入场-买入】市场：",self.EM[j].Name,"Buy At:",self.EM[j].Sell.Price,"Amount:",BuyAmount,"AlreadyInSellPrice",AlreadyInSellPrice,"AlreadyInBuyPrice",AlreadyInBuyPrice)
                    #self.EM[j].e.Buy(self.EM[j].Sell.Price,BuyAmount/(1-self.EM[j].Fee.Buy/100))
                    borderid=self.EM[j].e.Buy(self.EM[j].Sell.Price,BuyAmount/(1-self.EM[j].Fee.Buy/94))
                    ret=True
                #休眠100毫秒

                if ret:
                    Sleep(100)
                # 检查账户成交
                eatradeamount=ebtradeamount=0
                if SellAmount>=self.EM[i].MinStock:

                    eatradeamount=self.EM[i].CheckTrade(SellAmount,sorderid,"s")
                    if(eatradeamount+tradinglist[keyname].InSellAmount)>0:
                        tradinglist[keyname].InSellPrice=round(((tradinglist[keyname].InSellAmount*tradinglist[keyname].InSellPrice+eatradeamount*self.EM[i].Buy.Price)/(tradinglist[keyname].InSellAmount+eatradeamount)),2)
                    tradinglist[keyname].InSellAmount=round(tradinglist[keyname].InSellAmount+eatradeamount,5)
                    Log("检查",self.EM[i].Name,"【入场-卖出】原币数",noweastocks,"当前币数",self.EM[i].Stocks,"成交数",eatradeamount,"InSellPrice",tradinglist[keyname].InSellPrice,"InSellAmount",tradinglist[keyname].InSellAmount)

                if BuyAmount>=self.EM[j].MinStock:
                    ebtradeamount=self.EM[j].CheckTrade(BuyAmount,borderid,"b")
                    if(ebtradeamount+tradinglist[keyname].InBuyAmount)>0:
                        tradinglist[keyname].InBuyPrice=round(((tradinglist[keyname].InBuyAmount*tradinglist[keyname].InBuyPrice+ebtradeamount*self.EM[j].Sell.Price)/(tradinglist[keyname].InBuyAmount+ebtradeamount)),2)
                    tradinglist[keyname].InBuyAmount=round(tradinglist[keyname].InBuyAmount+ebtradeamount,5)
                    Log("检查",self.EM[j].Name,"【入场-买入】原币数",nowebstocks,"当前币数",self.EM[j].Stocks,"成交数",ebtradeamount,"InBuyPrice",tradinglist[keyname].InBuyPrice,"InBuyAmount", tradinglist[keyname].InBuyAmount)

                if tradinglist[keyname].InSellAmount<MaxCoinOnce:
                    tradetasklistdf.ix[keyname,"InSell"]="in"
                elif tradinglist[keyname].InSellAmount>=MaxCoinOnce-self.EM[i].MinStock:
                    tradetasklistdf.ix[keyname,"InSell"]="ok"
                if tradinglist[keyname].InBuyAmount<MaxCoinOnce:
                    tradetasklistdf.ix[keyname,"InBuy"]="in"
                elif tradinglist[keyname].InBuyAmount>=MaxCoinOnce-self.EM[j].MinStock:
                    tradetasklistdf.ix[keyname,"InBuy"]="ok"
                fee=(tradinglist[keyname].InSellAmount*tradinglist[keyname].InSellPrice)*self.EM[i].Fee.Sell/100+ tradinglist[keyname].InBuyPrice*(tradinglist[keyname].InBuyAmount/(1-self.EM[j].Fee.Buy/100))*self.EM[j].Fee.Buy/100
                tradinglist[keyname].InFee=round(fee,4)
                tradinglist[keyname].InProfit=tradinglist[keyname].InSellPrice*tradinglist[keyname].InSellAmount-tradinglist[keyname].InBuyPrice*tradinglist[keyname].InBuyAmount-tradinglist[keyname].InFee
                tradinglist[keyname].InProfit=ext.Cut(tradinglist[keyname].InProfit,2)


                if tradinglist[keyname].InSellAmount>=MaxCoinOnce-self.EM[i].MinStock and tradinglist[keyname].InBuyAmount>=MaxCoinOnce-self.EM[j].MinStock:
                    tradetasklistdf.ix[keyname,"Status"]="waiting out"
        return ret
    def CheckGoingIn(self):#进场检测
        df=tradetasklistdf.loc[tradetasklistdf["Status"]=="going in"]
        ret=False
        if df is not None and len(df.index)>0:
            for index in range(len(df.index)):
                keyname=df.ix[index, 'KeyName']
                i=keyname.split(":")[0]
                j=keyname.split(":")[1]
                AlreadyInSellAmount=tradinglist[keyname].InSellAmount #取得已进场数额
                AlreadyInSellPrice=tradinglist[keyname].InSellPrice #取得进场卖价
                AlreadyInBuyAmount=tradinglist[keyname].InBuyAmount#取得已进场买入数额
                AlreadyInBuyPrice=tradinglist[keyname].InBuyPrice #取得进场买入价格
                DiffInSellAmount=round((MaxCoinOnce-AlreadyInSellAmount),5) #进场卖出需要的数量
                DiffInBuyAmount=round((MaxCoinOnce-AlreadyInBuyAmount),5) #进场买入需要的数量
                InAmount=self.EM[i].Buy.Amount   #Ea 市场买入份额，高价卖出对应的市场深度
                OutBuyAmount=self.EM[j].Sell.Amount #Eb 市场卖出的份额，低价买入对应的市场深度

                SellErrstr=""
                SellAmount=min(DiffInSellAmount,InAmount)

                if AlreadyInSellPrice>0 and self.EM[i].Buy.Price<AlreadyInSellPrice and tradetasklistdf.ix[keyname,"InSell"]=="in":
                    SellErrstr+="入场卖价小于已入场卖价！"
                if AlreadyInBuyPrice>0:
                    if self.EM[i].Buy.Price-AlreadyInBuyPrice<tradinglist[keyname].InDiffPrice:
                        SellErrstr+="入场卖价与已入场买价，差价不符合要求！"

                BuyErrstr=""
                BuyAmount=min(DiffInBuyAmount,OutBuyAmount)

                if AlreadyInBuyPrice>0 and self.EM[j].Sell.Price>AlreadyInBuyPrice and tradetasklistdf.ix[keyname,"InBuy"]=="in":
                    BuyErrstr+="入场买价大于已入场买价！"
                if AlreadyInBuyPrice==0 and AlreadyInSellPrice>0:
                    if AlreadyInSellPrice-self.EM[j].Sell.Price<tradinglist[keyname].InDiffPrice:
                        BuyErrstr+="入场买价与已入场卖价，差价不符合要求！"
                if len(SellErrstr)>0 or len(BuyErrstr)>0:
                    #Log("SellErrstr",SellErrstr,"BuyErrstr",BuyErrstr)
                    return
                noweastocks=self.EM[i].Stocks
                nowebstocks=self.EM[j].Stocks#交易前两个市场的币

                #如果通过了检查 则进行交易
                if SellAmount>=self.EM[i].MinStock:
                    Log("准备【入场补充-卖出】市场：",self.EM[i].Name,"Sell At:",self.EM[i].Buy.Price,"Amount:",SellAmount,"AlreadyInSellPrice",AlreadyInSellPrice,"AlreadyInBuyPrice",AlreadyInBuyPrice,"【差价】",tradinglist[keyname].InDiffPrice)
                    sorderid=self.EM[i].e.Sell(self.EM[i].Buy.Price,SellAmount)
                    ret=True
                if BuyAmount>=self.EM[j].MinStock:
                    Log("准备【入场补充-买入】市场：",self.EM[j].Name,"Buy At:",self.EM[j].Sell.Price,"Amount:",BuyAmount,"AlreadyInSellPrice",AlreadyInSellPrice,"AlreadyInBuyPrice",AlreadyInBuyPrice,"【差价】",tradinglist[keyname].InDiffPrice)
                    #self.EM[j].e.Buy(self.EM[j].Sell.Price,BuyAmount/(1-self.EM[j].Fee.Buy/100))
                    borderid=self.EM[j].e.Buy(self.EM[j].Sell.Price,BuyAmount/(1-self.EM[j].Fee.Buy/94))
                    ret=True
                #休眠100毫秒
                if ret:
                    Sleep(100)
                # 检查账户成交
                eatradeamount=ebtradeamount=0
                if SellAmount>=self.EM[i].MinStock:
                    eatradeamount=self.EM[i].CheckTrade(SellAmount,sorderid,"s")
                    if(eatradeamount+tradinglist[keyname].InSellAmount)>0:
                        tradinglist[keyname].InSellPrice=round(((tradinglist[keyname].InSellAmount*tradinglist[keyname].InSellPrice+eatradeamount*self.EM[i].Buy.Price)/(tradinglist[keyname].InSellAmount+eatradeamount)),2)
                    tradinglist[keyname].InSellAmount=round(tradinglist[keyname].InSellAmount+eatradeamount,5)
                    Log("检查",self.EM[i].Name,"【入场补充-卖出】原币数",noweastocks,"当前币数",self.EM[i].Stocks,"成交数",eatradeamount,"InSellPrice",tradinglist[keyname].InSellPrice,"InSellAmount",tradinglist[keyname].InSellAmount)

                if BuyAmount>=self.EM[j].MinStock:
                    ebtradeamount=self.EM[j].CheckTrade(BuyAmount,borderid,"b")
                    if(ebtradeamount+tradinglist[keyname].InBuyAmount)>0:
                        tradinglist[keyname].InBuyPrice=round(((tradinglist[keyname].InBuyAmount*tradinglist[keyname].InBuyPrice+ebtradeamount*self.EM[j].Sell.Price)/(tradinglist[keyname].InBuyAmount+ebtradeamount)),2)
                    tradinglist[keyname].InBuyAmount=round(tradinglist[keyname].InBuyAmount+ebtradeamount,5)
                    Log("检查",self.EM[j].Name,"【入场补充-买入】原币数",nowebstocks,"当前币数",self.EM[j].Stocks,"成交数",ebtradeamount,"InBuyPrice",tradinglist[keyname].InBuyPrice,"InBuyAmount", tradinglist[keyname].InBuyAmount)

                if tradinglist[keyname].InSellAmount<MaxCoinOnce:
                    tradetasklistdf.ix[keyname,"InSell"]="in"
                elif tradinglist[keyname].InSellAmount>=MaxCoinOnce-self.EM[i].MinStock:
                    tradetasklistdf.ix[keyname,"InSell"]="ok"
                if tradinglist[keyname].InBuyAmount<MaxCoinOnce:
                    tradetasklistdf.ix[keyname,"InBuy"]="in"
                elif tradinglist[keyname].InBuyAmount>=MaxCoinOnce-self.EM[j].MinStock:
                    tradetasklistdf.ix[keyname,"InBuy"]="ok"
                fee=(tradinglist[keyname].InSellAmount*tradinglist[keyname].InSellPrice)*self.EM[i].Fee.Sell/100+ tradinglist[keyname].InBuyPrice*(tradinglist[keyname].InBuyAmount/(1-self.EM[j].Fee.Buy/100))*self.EM[j].Fee.Buy/100
                tradinglist[keyname].InFee=ext.Cut(fee,4)
                tradinglist[keyname].InProfit=tradinglist[keyname].InSellPrice*tradinglist[keyname].InSellAmount-tradinglist[keyname].InBuyPrice*tradinglist[keyname].InBuyAmount-tradinglist[keyname].InFee
                tradinglist[keyname].InProfit=ext.Cut(tradinglist[keyname].InProfit,2)



                if tradinglist[keyname].InSellAmount>=MaxCoinOnce-self.EM[i].MinStock and tradinglist[keyname].InBuyAmount>=MaxCoinOnce-self.EM[j].MinStock:
                    tradetasklistdf.ix[keyname,"Status"]="waiting out"


    def GetAdvExcInfo(self):
        _str=""
        totalexchange=0
        totalbalance=0
        totalstocks=0
        totalfrozenbalance=0
        totalfrozenstocks=0
        totalnet=0
        #totalstartnet=0
        noopnet=0
        #totalinitbalance=0
        #totalinitstocks=0
        for i in self.EM:
            #totalinitbalance+=self.EM[i].InitBalance+self.EM[i].InitFrozenBalance
            #totalinitstocks+=self.EM[i].InitStocks+self.EM[i].InitFrozenStocks
            totalexchange+=1
            totalbalance+=self.EM[i].Balance
            totalstocks+=self.EM[i].Stocks
            totalfrozenbalance+=self.EM[i].FrozenBalance
            totalfrozenstocks+=self.EM[i].FrozenStocks
            totalnet+=self.EM[i].NET
            #totalstartnet+=self.EM[i].InitNET
            noopnet+=self.EM[i].InitBalance+self.EM[i].InitFrozenBalance+(self.EM[i].InitStocks+self.EM[i].InitFrozenStocks)*self.EM[i].Buy.Price
        self.NoOpNET=ext.Cut((noopnet-self.TotalInitNET),2)
        self.NETDiff=ext.Cut((totalnet-self.TotalInitNET),2)
        _str="市场总数"+str(totalexchange)+"初市值"+str(self.TotalInitNET)+"初钱"+str(self.TotalInitBalance)+"初币"+str(self.TotalInitStocks)+"总钱"+str(totalbalance)+"总冻结钱"+str(totalfrozenbalance)+"总币"+str(totalstocks)+"总冻结币"+str(totalfrozenstocks)+"总市值"+str(totalnet)+"无操浮盈"+str(self.NoOpNET)+"运行净值差"+str(self.NETDiff)+"总费"+str(self.TotalFee)
        return _str
    #计算收益率
    def CalProfit(self):
        total=0
        today=0
        thisweek=0
        mstr=""
        nowtimestamp=time.time()
        todaystarttimestamp=nowtimestamp-86400 #从当前算起 1日起点
        thisweektimestamp=nowtimestamp-86400*7#从当前算起 7日起点
        for i in range(len(histradelist)):

            if histradelist[i].OutTime>todaystarttimestamp:
                today+=histradelist[i].Profit
            if histradelist[i].OutTime>todaystarttimestamp:
                thisweek+=histradelist[i].Profit
            total+=histradelist[i].Profit
        dayp=today/self.TotalInitNET*100
        #sdayp=thisweek/self.TotalInitNET*100
        yp=dayp*365
        mp=dayp*30
        tp=total/self.TotalInitNET*100
        mstr+="1日收："+str(ext.Cut(today,2))+"元 7日收："+str(ext.Cut(thisweek,2))+" 日"+str(ext.Cut(dayp,4))+"% 月"+str(ext.Cut(mp,2))+"% 年"+str(ext.Cut(yp,2))+"%\n"
        mstr+="总收："+str(ext.Cut(total,2))+"元 "+str(ext.Cut(tp,2))+"%"
        return mstr
class AdvExchange:
    def __init__(self,exchange):
        ExchangeFeesDic=eval(ExchangeFees)
        self.e=exchange
        self.Name=self.e.GetName()
        self.MinStock=self.e.GetMinStock()
        self.MinPrice=self.e.GetMinPrice()
        self.UpdateTime=0
        self.e.SetPrecision(2, 3);
        self.SellList=[]
        self.BuyList=[]
        times=time.time()
        self.Fee=self.e.GetFee()
        self.Fee.Buy=0.2
        self.Fee.Sell=0.2
        global ExchangeFeesDic
        #Log(ExchangeFeesDic)
        self.MakerFee=ExchangeFeesDic[self.Name]["Maker"]
        self.TakerFee=ExchangeFeesDic[self.Name]["Taker"]
        #Log("Exchange:",self.Name,"Fee:",self.Fee,"MakerFee",self.MakerFee,"TakerFee",self.TakerFee)
        account=self.e.GetAccount()
        #account=_C(self.e.GetAccount)
        self.InitBalance=ext.Cut(account.Balance,2)#初始账户现金余额
        self.InitFrozenBalance=ext.Cut(account.FrozenBalance,2)#初始账户现金冻结余额
        self.InitStocks=ext.Cut(account.Stocks,4)#初始账户币余额
        self.InitFrozenStocks=ext.Cut(account.FrozenStocks,4)#初始账户冻结币余额
        self.Balance=ext.Cut(account.Balance,2)#初始账户现金余额
        self.FrozenBalance=ext.Cut(account.FrozenBalance,2)#初始账户现金冻结余额
        self.Stocks=ext.Cut(account.Stocks,4)#初始账户币余额
        self.FrozenStocks=ext.Cut(account.FrozenStocks,4)#初始账户冻结币余额
        timee=time.time()
        self.GetAccountDelay=int((timee-times)*1000)#取得账户信息延迟
        times=time.time()
        depth=self.e.GetDepth()
        #depth=_C(self.e.GetDepth)
        self.Sell=depth.Asks[0]
        self.SellList.append(depth.Asks[0])
        self.Buy=depth.Bids[0]
        self.BuyList.append(depth.Bids[0])
        self.InitPrice=self.Buy.Price#初始资产计算价格 取初始时 市场深度买一价格
        self.InitNET=ext.Cut((self.InitBalance+self.InitFrozenBalance+(self.InitStocks+self.InitFrozenStocks)*self.InitPrice),2)#初始净资产
        self.NET=ext.Cut((self.Balance+self.FrozenBalance+(self.Stocks+self.FrozenStocks)*self.Buy.Price),2)
        timee=time.time()
        self.GetDepthDelay=int((timee-times)*1000)#取得深度信息延迟
        #self.Fee#初始化后交易总费用
        self.Delay=self.GetDepthDelay+self.GetAccountDelay
        self.QueryFailedTimes=0#网络通信错误
        self.Profit=0#账户盈利
        self.LockStocks=0#账户锁定的币
        self.LockBalance=0#账户锁定的资金
        self.AvailableStocks=self.Stocks-self.LockStocks#账户可用的币
        self.AvailableBalance=self.Balance-self.LockBalance#账户可用的钱
        self.Trend=0#市场趋势判断
        self.LongAvgPrice=0#趋势判断长期平均价格
        self.ShortAvgPrice=0#趋势判断短期平均价格
        self.TrendDiff=0#趋势判断短期长期价格差
        self.TrendAvgDiff=0#趋势判断差价平均
        self.TrendMacd=0
        self.UpdateDf()
    #删除订单
    def cancelAll(self):
        ret = False
        while True:
            n = 0
            for order in _C(self.e.GetOrders):
                ret = True
                self.e.CancelOrder(order.Id)
                n+=1
            if n == 0:
                break
        return ret
    #更新深度信息
    def UpdateDepth(self,updatetimes):
        times=time.time()
        #dwait=self.e.Go("GetDepth")
        #depth=dwait.wait()[0]
        depth=self.e.GetDepth()

        dtimee=time.time()
        self.Sell=depth.Asks[0]
        self.Buy=depth.Bids[0]
        self.MinStock=max(self.MinStock,self.MinPrice/self.Buy.Price)

        if updatetimes<WatchWindow:
            self.SellList.append(depth.Asks[0])
            self.BuyList.append(depth.Bids[0])
        else:
            self.BuyList=copy.copy(self.BuyList[1:])
            self.SellList=copy.copy(self.SellList[1:])
            self.SellList.append(depth.Asks[0])
            self.BuyList.append(depth.Bids[0])
        self.GetDepthDelay=int((dtimee-times)*1000)#取得深度信息延迟
        self.NET=ext.Cut((self.Balance+self.FrozenBalance+(self.Stocks+self.FrozenStocks)*self.Buy.Price),2)
        self.Profit=self.NET-self.InitNET#账户浮盈
        #计算该市场的趋势
        avgprice=(self.Sell.Price+self.Buy.Price)/2
        if self.UpdateTime<TrendLongTrem and self.UpdateTime>0:
            self.LongAvgPrice=(self.LongAvgPrice*(self.UpdateTime-1)+avgprice)/self.UpdateTime
            
            if self.UpdateTime>(TrendLongTrem-TrendShortTrem):
                
                self.ShortAvgPrice=(self.ShortAvgPrice*(self.UpdateTime-1-(TrendLongTrem-TrendShortTrem))+avgprice)/(self.UpdateTime-(TrendLongTrem-TrendShortTrem))

            self.TrendDiff=self.ShortAvgPrice-self.LongAvgPrice #快线减去慢线
            if self.UpdateTime>TrendLongTrem-TrendDiffTrem:
                self.TrendAvgDiff=(self.TrendAvgDiff*(self.UpdateTime-1-(TrendLongTrem-TrendDiffTrem))+self.TrendDiff)/(self.UpdateTime-(TrendLongTrem-TrendDiffTrem))
                self.TrendMacd=(self.TrendDiff-self.TrendAvgDiff)*2
        elif self.UpdateTime>=TrendLongTrem:
            self.LongAvgPrice=(self.LongAvgPrice*(TrendLongTrem-1)+avgprice)/TrendLongTrem
            self.ShortAvgPrice=(self.ShortAvgPrice*(TrendShortTrem-1)+avgprice)/TrendShortTrem
            self.TrendDiff=self.ShortAvgPrice-self.LongAvgPrice #快线减去慢线
            
            self.TrendAvgDiff=(self.TrendAvgDiff*(TrendDiffTrem-1)+self.TrendDiff)/TrendDiffTrem
            self.TrendMacd=(self.TrendDiff-self.TrendAvgDiff)*2
            
            ext.PlotDot(self.Name+"TrendMacd",self.TrendMacd)
            ext.PlotLine(self.Name+"TrendAvgDiff", self.TrendAvgDiff)
            ext.PlotLine(self.Name+"TrendDiff", self.TrendDiff)
            
        #ext.PlotLine("LongAvgPrice", self.LongAvgPrice)
        #ext.PlotLine("ShortAvgPrice", self.ShortAvgPrice)
            
        #计算完毕市场趋势
        self.UpdateTime+=1
        self.UpdateDf()
    def UpdateAccout(self):
        times=time.time()
        #await=self.e.Go("GetAccount")
        #account=await.wait()[0]
        account=_C(self.e.GetAccount)
        #account=self.e.GetAccount()
        atimee=time.time()
        self.Balance=ext.Cut(account.Balance,2)#当前账户现金余额
        self.FrozenBalance=ext.Cut(account.FrozenBalance,2)#当前账户现金冻结余额
        self.Stocks=ext.Cut(account.Stocks,4)#当前账户币余额
        self.FrozenStocks=ext.Cut(account.FrozenStocks,4)#当前账户冻结币余额
        self.GetAccountDelay=int((atimee-times)*1000)#取得账户信息延迟

        self.UpdateDf()
    #检测市场成交 返回0, 未成交 交易已经取消 返回>0 成交额
    def CheckTrade(self,aimamount,orderid,direction):
        looptime=0
        if aimamount<0.01:
            #return TradeResault(direction,0,0)
            return 0
        order=self.e.GetOrder(orderid)
        if order is None:
            Log(self.Name," do not get order!, #ff0000")
            #return TradeResault(direction,0,0)
            return 0
        ret=False
        ostocks=self.Stocks
        obalance=self.Balance
        diffstocks=0
        self.UpdateAccout()
        while direction=="s" and  ostocks<self.Stocks:
            Sleep(200)
            Log("ostocks",ostocks,"self.Stocks",self.Stocks)
            self.UpdateAccout()
        while direction=="b" and  ostocks>self.Stocks:
            Sleep(200)
            Log("ostocks",ostocks,"self.Stocks",self.Stocks)
            self.UpdateAccout()
        diffstocks=abs(round((ostocks-self.Stocks-self.FrozenStocks),2))

        #while diffstocks!=(ostocks-self.Stocks)


        while (self.FrozenStocks>0 or self.FrozenBalance>0) or diffstocks==0:
            Log("市场：",self.Name,"存在挂起交易FrozenBalance:",self.FrozenBalance,"FrozenStocks:",self.FrozenStocks,"diffstocks:",diffstocks)
            while self.FrozenStocks>0 or self.FrozenBalance>0:
                Log("循环市场：",self.Name,"检查存证挂起交易FrozenBalance:",self.FrozenBalance,"FrozenStocks:",self.FrozenStocks,"diffstocks:",diffstocks)
                self.cancelAll()
                ret=True

                Sleep(200)
                self.UpdateAccout()
            Sleep(150)
            self.UpdateAccout()
            diffstocks=abs(round((ostocks-self.Stocks),2))
            if ret==False and diffstocks==0:
                continue
            else:
                break

        #if diffstocks>aimamount:
        #    diffstocks=aimamount
        #return TradeResault(direction,diffstocks,0)
        return diffstocks



    def UpdateDf(self):
        exchangesdf.ix[self.Name,'Name']=self.Name
        exchangesdf.ix[self.Name,'Delay']=str(self.Delay)+"("+str(self.GetAccountDelay)+":"+str(self.GetDepthDelay)+")"
        exchangesdf.ix[self.Name,'Buy']=self.Buy.Price
        exchangesdf.ix[self.Name,'Sell']=self.Sell.Price
        exchangesdf.ix[self.Name,'Balance']=self.Balance
        exchangesdf.ix[self.Name,'Stocks']=self.Stocks
        exchangesdf.ix[self.Name,'FrozenBalance']=self.FrozenBalance
        exchangesdf.ix[self.Name,'FrozenStocks']=self.FrozenStocks
        exchangesdf.ix[self.Name,'InitBalance']=self.InitBalance
        exchangesdf.ix[self.Name,'InitStocks']=self.InitStocks
        exchangesdf.ix[self.Name,'InitNET']=self.InitNET
        exchangesdf.ix[self.Name,'NET']=self.NET
        exchangesdf.ix[self.Name,'Profit']=self.Profit
        exchangesdf.ix[self.Name,'UpdateTime']=self.UpdateTime


class TradeResault:
    def __init__(self,_direction,_amount,_price):
        self.Direction=_direction
        self.Amount=_amount
        self.Price=_price
