# botvs@87afb0bb6ca4dada56ad5b1f51d1e60d
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
exchangesdfcolumnsname=["Name","Delay","Buy","Sell","Balance","Stocks","FrozenBalance","FrozenStocks","LockBalance","LockStocks","InitBalance","InitStocks","InitNET","NET","Profit","UpdateTimes"]
exchangesdfcolumnsnameshow=["名字","延时","买","卖","现金","货币","冻金","冻币","锁金","锁币","初金","初币","初资产","现资产","浮盈","更新次数"]
starttime=0#策略开始时间
exchangesdiffcolumnsname=["KeyName","ABuy","ACoin","BSell","BBalance","InDiff","Go","Profit","CompleteCount"]
exchangesdiffcolumnsnameshow=["交易对","左买","可用币","右卖","可用钱","进差","距离","盈利","赢/输/总"]

tradetasklistcolumnsname=["ID","Ae","Be","InSellPrice","InSellAmount","InBuyPrice","InBuyAmount","Log"]
tradetasklistcolumnsnameshow=["ID","左","右","卖价","卖量","买价","买量","日志"]
histradelistcolumnsnameshow=["ID","左","右","进","出","进买价","进买量","进卖价","进卖量","进盈利","进费","净盈利","日志"]
histradelist=[]
tradinglist={} #保存当前正在进行的进场交易

tradepairdict={}#保存在场交易的dict 通过ID来查找具体的交易对儿
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
               

    exchangesdf=pa.DataFrame(columns=exchangesdfcolumnsname,index=exchangesnamelist)
    exchangesdiffdf=pa.DataFrame(columns=exchangesdiffcolumnsname,index=exchangespairlnamelist)
    tradetasklistdf=pa.DataFrame(columns=tradetasklistcolumnsname,index=exchangespairlnamelist)
    tradetasklistdf.append(exchangespairlnamelist)
    tradetasklistdf=tradetasklistdf.fillna(value=0)
    SetErrorFilter("502:|503:|tcp|character|unexpected|network|timeout|WSARecv|Connect|GetAddr|no such|reset|http|received|EOF|reused|GetOrder|CancelOrder");


def GetHisTable():
    str={'type': 'table', 'title': '历史记录', 'cols': [],'rows':[]}
    str["cols"]=histradelistcolumnsnameshow
    rows=[]
    for i in range(len(histradelist)):
        row=[]
        row.append(histradelist[i].ID)
        row.append(histradelist[i].AeName)
        row.append(histradelist[i].BeName)
        row.append(_D(histradelist[i].InTime))
        row.append(_D(histradelist[i].OutTime))
        row.append(round(histradelist[i].InBuyPrice,4))
        row.append(round(histradelist[i].InBuyAmount,4))
        row.append(round(histradelist[i].InSellPrice,4))
        row.append(round(histradelist[i].InSellAmount,4))

        row.append(round(histradelist[i].InProfit,4))

        row.append(round(histradelist[i].InFee,4))
        row.append(round(histradelist[i].Profit,4))
        row.append(histradelist[i].Log)
        rows.append(row)
    str["rows"]=rows
    return str
def GetInFieldTable():
    str={'type': 'table', 'title': '任务监控', 'cols': [],'rows':[]}
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

        row.append(round(tradinglist[i].InProfit,4))

        row.append(round(tradinglist[i].InFee,4))

        row.append(round(tradinglist[i].Profit,4))
        rows.append(row)
    str["rows"]=rows
    return str
def CheckTaskDic():
    removeindex=[]
    str={'type': 'table', 'title': '任务监控', 'cols': [],'rows':[]}
    str["cols"]=histradelistcolumnsnameshow
    rows=[]
    for i in tradepairdict:
        row=[]
        row.append(i)
        row.append(tradepairdict[i].AeName)
        row.append(tradepairdict[i].BeName)
        row.append(_D(tradepairdict[i].InTime))
        if tradepairdict[i].OutTime>0:
            row.append(_D(tradepairdict[i].OutTime))
        else:
            row.append(tradepairdict[i].OutTime)
        row.append(round(tradepairdict[i].InBuyPrice,4))
        row.append(round(tradepairdict[i].InBuyAmount,4))
        row.append(round(tradepairdict[i].InSellPrice,4))
        row.append(round(tradepairdict[i].InSellAmount,4))

        row.append(round(tradepairdict[i].InProfit,4))

        row.append(round(tradepairdict[i].InFee,4))

        row.append(round(tradepairdict[i].Profit,4))
        row.append(tradepairdict[i].Log)
        if tradepairdict[i].Status==3 or tradepairdict[i].Status==2:#成了或者撤了都要从字典里面删掉
            removeindex.append(i)
        rows.append(row)
    str["rows"]=rows
    #Log(removeindex)
    if len(removeindex)>0:
        #Log(tradepairdict)
        for j in range(len(removeindex)):
            if tradepairdict[removeindex[j]].Status==2:
                histradelist.append(tradepairdict[removeindex[j]])
            tradepairdict.pop(removeindex[j])
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
        tasktablestr=""
        #try:
        if True:
            times=time.time()
            em.Update()
            timeet=time.time()
            if runtime>100:

                em.CheckIn()
                tasktablestr=CheckTaskDic()





            timee=time.time()
            Status["up"].append(em.GetAdvExcInfo())
            Status["bottom"].append("访问次数:"+str(em.UpdateTime))
            Status["tables"].append(ext.GetTableDict(exchangesdf,"交易所",exchangesdfcolumnsnameshow))
            Status["tables"].append(ext.GetTableDict(exchangesdiffdf,"信号监控",exchangesdiffcolumnsnameshow))
            Status["tables"].append(tasktablestr)
            #Status["tables"].append(GetInFieldTable())
            Status["tables"].append(GetHisTable())
            Status["bottom"].append(em.CalProfit())
            timeeu=time.time()
            TDelay=int((timeet-times)*1000)#取得更新延迟
            TDelayT=int((timee-timeet)*1000)#取得交易延迟
            TDelayU=int((timeeu-timee)*1000)#取得显示延迟

            Status["bottom"].append("发起更新时间:"+time.strftime("%Y-%m-%d %X", time.localtime(times))+" 更新延迟:"+str(TDelay)+"ms"+" 交易延迟:"+str(TDelayT)+"ms"+" 显示延迟:"+str(TDelayU)+"ms")

            LogStatus(ext.GetStrFromList(Status["up"]),'\n`'+json.dumps(Status["tables"])+'`\n',ext.GetStrFromList(Status["bottom"]))
        #except Exception,e:
        #    Log("err:",e,traceback.print_exc())
        Sleep(100)
    if em.NETDiff>0:
        Log("系统判断止盈终止条件，结束交易...@")
    else:
        Log("系统判断止损终止条件，结束交易...@")

class TradePair:
    def __init__(self,_id,_ea,_eb,_planamount,_indiff):
        self.ID=_id
        self.PlanAmount=_planamount
        self.AeName=_ea.Name
        self.BeName=_eb.Name
        self.ACoin=0
        self.BBalance=0
        self.InTime=time.time()
        self.OutTime=0
        self.InSellPrice=0
        self.InBuyPrice=0
        self.InSellAmount=0
        self.InBuyAmount=0
        self.InBuyFlag=False
        self.InSellFlag=False
        self.InFee=0
        self.InProfit=0
        self.EA=_ea
        self.EB=_eb
        self.LockStocks=round(self.PlanAmount,6)
        self.EA.LockStocks+=self.LockStocks
        self.LockBalance=round((self.PlanAmount*self.EB.Sell.Price/(1-self.EB.MakerFee)),6)+1
        self.EB.LockBalance+=self.LockBalance
        self.Profit=0
        self.InDiffPrice=_indiff
        self.CurrentInDiffPrice=_indiff
        self.TradeMode=0#0委挂，1委拍
        self.Status=0#0=交易中1=部分成交2=完全成交3=撤退
        self.Log=""
        self.InSellFee=0
        self.InBuyFee=0
    #根据当前交易任务成交情况，计算此时如果出场的化差价情况
    def CalCurrentInDiff(self):
        self.EA.UpdateDepth(300)
        self.EB.UpdateDepth(300)
        #全部未成交&#全部成交
        newdiff=((self.PlanAmount-self.InSellAmount)*self.EA.Buy.Price+self.InSellAmount*self.InSellPrice-((self.PlanAmount-self.InBuyAmount)*self.EB.Sell.Price+self.InBuyAmount*self.InBuyPrice))/((self.PlanAmount-self.InSellAmount)*self.EA.Buy.Price+self.InSellAmount*self.InSellPrice)
        self.CurrentInDiffPrice=newdiff
        
        # if self.InBuyFlag==self.InSellFlag:
        #     return ext.Cut((self.EA.Buy.Price - self.EB.Sell.Price)/self.EA.Buy.Price,6)
        # #部分成交
        # else:
        #     if self.InSellFlag:#卖出完成，此时差价和市场买入相关
        #         return ext.Cut((self.InSellPrice - self.EB.Sell.Price)/self.InSellPrice,6)
        #     if self.InBuyFlag:
        #         return ext.Cut((self.EA.Buy.Price - self.InBuyPrice)/self.EA.Buy.Price,6)
        


    def Trade(self):
        times=time.time()
        CurrentSellPrice=self.EA.Buy.Price
        CurrentBuyPrice=self.EB.Sell.Price
        buysignal=None
        sellsignal=None
        Log("ID:",self.ID,"线程交易:",self.EA.Name,"->",self.EB.Name," PlanAmount:",self.PlanAmount,"开启委挂模式")
        self.Log+="[模式委挂]"
        TrendDiff=(self.EA.TrendDiff+self.EB.TrendDiff)/2#两个市场的趋势指标
        TrendMacd=(self.EA.TrendMacd+self.EB.TrendMacd)/2
        Trend=0
        MakerTime=10
        if TrendDiff>0 and TrendMacd>0:
            Trend=1
        
        #交易分两种情况，一是通过趋势判断挂单
        bidtime=0
        while True:
            self.EA.UpdateDepth(300)
            self.EB.UpdateDepth(300)
            CurrentSellPrice=self.EA.Buy.Price
            CurrentBuyPrice=self.EB.Sell.Price
            
            if MakerTime>1:
                #检查单边入场情况
                if self.Status==1 or self.Status==0:
                    self.CalCurrentInDiff()
                    
                    if self.CurrentInDiffPrice-DiffAim<GoT and self.Status==0:#如果当前的价差已经小于预设目标，这个时候就要退场了
                        self.Log+="差价变小了(撤)"
                        Log("ID:",self.ID,"差价变小了(撤) Status=0 撤退！CurrentInDiffPrice:",self.CurrentInDiffPrice)
                        self.OutTime=time.time()
                        self.EA.LockStocks-=self.LockStocks
                        self.EB.LockBalance-=self.LockBalance
                        self.Status=3
                        return
                    if self.CurrentInDiffPrice-self.InDiffPrice<0 and self.Status==1:#如果当前的价差已经小于进场差价，这个时候就要改成委拍了
                        self.Log+="预警委挂单边入场(改)"   
                        Log("BreakOut ID :",self.ID)          
                        break
                #检查循环bid时间
              
                if time.time()-bidtime<=BidWaitTime:
                    Sleep(50)
                    continue
                bidtime=time.time()
                SellAmount=self.PlanAmount-self.InSellAmount
                BuyAmount=(self.PlanAmount-self.InBuyAmount)/(1-self.EB.MakerFee)
                Log("ID:",self.ID,"准备交易:SellAmount",SellAmount,"BuyAmount",BuyAmount)
                SellPrice=0
                BuyPrice=0
                self.Log+="追"
                if Trend==1:#涨势先买入
                    BuyPrice=CurrentBuyPrice-Slippage*MakerTime/10
                    SellPrice=CurrentSellPrice+Slippage*MakerTime/10
                    if not self.InBuyFlag:
                        Log("ID:",self.ID,self.EB.Name,"挂BuyPrice：",BuyPrice,"BuyAmount",BuyAmount,"Total",round((BuyPrice*BuyAmount),2),"Market Price:",self.EB.Sell.Price,"Coins",self.EB.Stocks,"Balance",self.EB.Balance)
                        buysignal=self.EB.e.Go("Buy",BuyPrice, BuyAmount)
                    if not self.InSellFlag:
                        Log("ID:",self.ID,self.EA.Name,"挂SellPrice",SellPrice,"SellAmount",SellAmount,"Total",round((SellPrice*SellAmount),2),"Market Price:",self.EA.Buy.Price,"Coins",self.EA.Stocks,"Balance",self.EA.Balance)
                        sellsignal=self.EA.e.Go("Sell",SellPrice, SellAmount)
                else:
                    BuyPrice=CurrentBuyPrice-Slippage*MakerTime/10
                    SellPrice=CurrentSellPrice+Slippage*MakerTime/10
                    if not self.InSellFlag:
                        Log("ID:",self.ID,self.EA.Name,"挂SellPrice",SellPrice,"SellAmount",SellAmount,"Total",round((SellPrice*SellAmount),2),"Market Price:",self.EA.Buy.Price,"Coins",self.EA.Stocks,"Balance",self.EA.Balance)
                        sellsignal=self.EA.e.Go("Sell",SellPrice, SellAmount)
                    if not self.InBuyFlag:    
                        Log("ID:",self.ID,self.EB.Name,"挂BuyPrice：",BuyPrice,"BuyAmount",BuyAmount,"Total",round((BuyPrice*BuyAmount),2),"Market Price:",self.EB.Sell.Price,"Coins",self.EB.Stocks,"Balance",self.EB.Balance)
                        buysignal=self.EB.e.Go("Buy",BuyPrice, BuyAmount)
                if not self.InBuyFlag:
                    buyorderid=buysignal.wait()[0]
                if not self.InSellFlag:
                    sellorderid=sellsignal.wait()[0]
                Sleep(500)
                #检查委挂成交情况
                if not self.InSellFlag:
                    #SellOrder=_C(self.EA.e.GetOrder, sellorderid)
                    SellOrder=self.EA.e.GetOrder(sellorderid)
                    #Log("SellOrder:",SellOrder)
                if not self.InBuyFlag:    
                    #BuyOrder=_C(self.EB.e.GetOrder, buyorderid)
                    BuyOrder=self.EB.e.GetOrder(buyorderid)
                    #Log("BuyOrder:",BuyOrder)
                if not self.InSellFlag:
                    if SellOrder.DealAmount>0:
                        self.Status=1
                        AvgPrice=SellOrder.AvgPrice
                        self.InSellPrice=(AvgPrice*SellOrder.DealAmount+self.InSellPrice*self.InSellAmount)/(self.InSellAmount+SellOrder.DealAmount)
                        #self.EA.LockStocks-=SellOrder.DealAmount
                        self.InSellAmount+=SellOrder.DealAmount
                        self.InSellFee+=SellOrder.DealAmount*AvgPrice*self.EA.MakerFee
                        self.EA.UpdateAccout()
                    if SellOrder.DealAmount<SellOrder.Amount:
                        self.EA.e.CancelOrder(sellorderid)
                    if SellOrder.Amount-SellOrder.DealAmount<=SlipCoin:
                        self.Log+="卖委挂成"
                        Log("ID:",self.ID,"卖委挂成")
                        self.InSellFlag=True
                        
                if not self.InBuyFlag:
                    if BuyOrder.DealAmount>0:
                        self.Status=1
                        AvgPrice=BuyOrder.AvgPrice
                        self.InBuyPrice=(AvgPrice*BuyOrder.DealAmount+self.InBuyPrice*self.InBuyAmount)/(self.InBuyAmount+BuyOrder.DealAmount)
                        #self.EB.LockBalance-=BuyOrder.DealAmount*AvgPrice
                        self.InBuyAmount+=BuyOrder.DealAmount
                        self.InBuyFee+=BuyOrder.DealAmount*AvgPrice*self.EB.MakerFee
                        self.EB.UpdateAccout()
                    if BuyOrder.DealAmount<BuyOrder.Amount:
                        self.EB.e.CancelOrder(buyorderid)
                    if BuyOrder.Amount-BuyOrder.DealAmount<=SlipCoin:
                        self.Log+="买委挂成"
                        Log("卖委挂成")
                        self.InBuyFlag=True

                Log("ID:",self.ID,"检查成交情况:InSellAmount",self.InSellAmount,"InBuyAmount",self.InBuyAmount)
               
                #委挂全部成交
                if self.PlanAmount-self.InSellAmount<=SlipCoin and self.PlanAmount-self.InBuyAmount<=SlipCoin:
                    
                   
                    self.Status=2
                    break
                #检查当前价格差距，如果没有成交，且价格的差距已经很小了，这个时候要撤退
                #NewInDiffPrice = ext.Cut((self.EA.Buy.Price - self.EB.Sell.Price)/self.EA.Buy.Price,6)
                #委挂全部未成交 且差价变小不符合套利需要 则撤退 

                
                MakerTime-=1  
                continue 
            else:
                break
        self.EA.UpdateDepth(300)
        self.EB.UpdateDepth(300)
        self.CalCurrentInDiff()
        if self.Status==0: #如果委挂模式搞不定，
            
            if self.CurrentInDiffPrice-DiffAim<GoT:#如果当前的价差已经小于预设目标，这个时候就要退场了
                self.Log+="差价变小了(撤)"
                self.OutTime=time.time()
                self.EA.LockStocks-=self.LockStocks
                self.EB.LockBalance-=self.LockBalance
                self.Status=3
                return

        if self.Status==1:
            
            self.Log+="单边入场[改委拍]"
            Log("ID:",self.ID,"[改委拍] 差价：",self.CurrentInDiffPrice)

        BidsTimes=0
        while True and self.Status!=2:
            #如果不是通过多线程的方式访问的化，需要对EA，EB进行UPDate

            #Log("self.InSellFlag:",self.InSellFlag,"self.InBuyFlag:",self.InBuyFlag)
            self.EA.UpdateDepth(300)
            self.EB.UpdateDepth(300)
            self.CalCurrentInDiff()
            SellAmount=self.PlanAmount-self.InSellAmount
            BuyAmount=(self.PlanAmount-self.InBuyAmount)/(1-self.EB.TakerFee)
            #BuyAmount=(self.PlanAmount-self.InBuyAmount)
            BuyPrice=self.EB.Sell.Price
            SellPrice=self.EA.Buy.Price
            if EatMode:
                BuyPrice=self.EB.Sell.Price+BidsTimes*Slippage
                SellPrice=self.EA.Buy.Price-BidsTimes*Slippage
            else:

                BuyPrice=self.EB.Sell.Price
                SellPrice=self.EA.Buy.Price             
                #假如不是吃单模式，需要价格符合预期
                #不是吃单模式，下单应该就只有一次，等待成交 防止出现问题，应该就按照当前的市场价格进行下单    
                if not self.InSellFlag:#入场还没有实现全部卖出 检查已经卖出的价格
                    if self.InSellPrice>0:#已经部分卖出了
                        #当前卖出的价格必须大于等于已经实现部分卖出的价格
                        if self.InSellPrice>self.EA.Buy.Price:
                            SellPrice=0#设置卖价为0 不进行卖出操作
                    else:#假如未卖出要查看买入的情况
                        if self.InBuyPrice==0:#也没有买入
                            #NewInDiffPrice = ext.Cut((self.EA.Buy.Price - self.EB.Sell.Price)/self.EA.Buy.Price,6)    
                            if self.CurrentInDiffPrice-DiffAim<GoT and self.Status==0:#如果当前的价差已经小于预设目标，这个时候就要退场了
                                self.Log+="差价变小了(撤)"
                                self.OutTime=time.time()
                                self.EA.LockStocks-=self.LockStocks
                                self.EB.LockBalance-=self.LockBalance
                                self.Status=3
                                return
                        else:
                            NewInDiffPrice = ext.Cut((self.EA.Buy.Price - self.InBuyPrice)/self.EA.Buy.Price,6) 
                            if NewInDiffPrice-DiffAim<GoT:#如果当前的价差已经小于预设目标，这个时候就要退场了
                                SellPrice=0#设置卖价为0 不进行卖出操作
                if not self.InBuyFlag:#入场还没实现全部买入
                    if self.InBuyPrice>0:#已经部分买入了
                        #当前买入的价格必须小于等于已经实现部分买入的价格
                        if self.InBuyPrice>self.EB.Sell.Price:
                            BuyPrice=0#设置卖价为0 不进行卖出操作
                    else:#假如未买入要查看卖出的情况
                        if self.InSellPrice==0:#也没有卖出
                            #NewInDiffPrice = ext.Cut((self.EA.Buy.Price - self.EB.Sell.Price)/self.EA.Buy.Price,6)  
                            if self.CurrentInDiffPrice-DiffAim<GoT and self.Status==0:#如果当前的价差已经小于预设目标，这个时候就要退场了
                                self.Log+="差价变小了(撤)"
                                self.OutTime=time.time()
                                self.EA.LockStocks-=self.LockStocks
                                self.EB.LockBalance-=self.LockBalance
                                self.Status=3
                                return
                        else:
                            NewInDiffPrice = ext.Cut((self.InSellPrice - self.EB.Sell.Price)/self.InSellPrice,6) 
                            if NewInDiffPrice-DiffAim<GoT:#如果当前的价差已经小于预设目标，这个时候就要退场了
                                BuyPrice=0#设置卖价为0 不进行卖出操作               
                # if self.InSellPrice>0 :
                #     NewInDiffPrice = ext.Cut((self.InSellPrice - self.EB.Sell.Price)/self.InSellPrice,6)
                #     if NewInDiffPrice-DiffAim<GoT and (not self.InSellFlag):#此时的价格差不符合预期
                #         continue

                # if self.InBuyPrice>0:   
                #     NewInDiffPrice = ext.Cut((self.EA.Buy.Price - self.InBuyPrice)/self.EA.Buy.Price,6)
                #     if NewInDiffPrice-DiffAim<GoT and (not self.InBuyFlag):#此时的价格差不符合预期
                #         continue                 
                if self.InBuyFlag and SellPrice==0:
                    return
                if self.InSellFlag and BuyPrice==0:
                    return 
                Log("ID:",self.ID,"EatMode:",EatMode,"InSellPrice",self.InSellPrice,"SellPrice",SellPrice,"InBuyPrice",self.InBuyPrice,"BuyPrice",BuyPrice)            
            if Trend==1:

                if not self.InBuyFlag and BuyPrice>0:
                    Log("ID:",self.ID,self.EB.Name,"拍BuyPrice：",BuyPrice,"BuyAmount",BuyAmount,"Total",round((BuyPrice*BuyAmount),2),"Market Price:",self.EB.Sell.Price,"Coins",self.EB.Stocks,"Balance",self.EB.Balance)
                    buysignal=self.EB.e.Go("Buy",BuyPrice, BuyAmount)
                if not self.InSellFlag and SellPrice>0:
                    Log("ID:",self.ID,self.EA.Name,"拍SellPrice",SellPrice,"SellAmount",SellAmount,"Total",round((SellPrice*SellAmount),2),"Market Price:",self.EA.Buy.Price,"Coins",self.EA.Stocks,"Balance",self.EA.Balance)
                    sellsignal=self.EA.e.Go("Sell",SellPrice, SellAmount)
            else:

                if not self.InSellFlag and SellPrice>0:
                    Log("ID:",self.ID,self.EA.Name,"拍SellPrice",SellPrice,"SellAmount",SellAmount,"Total",round((SellPrice*SellAmount),2),"Market Price:",self.EA.Buy.Price,"Coins",self.EA.Stocks,"Balance",self.EA.Balance)
                    sellsignal=self.EA.e.Go("Sell",SellPrice, SellAmount)
                if not self.InBuyFlag and BuyPrice>0:   
                    Log("ID:",self.ID,self.EB.Name,"拍BuyPrice：",BuyPrice,"BuyAmount",BuyAmount,"Total",round((BuyPrice*BuyAmount),2),"Market Price:",self.EB.Sell.Price,"Coins",self.EB.Stocks,"Balance",self.EB.Balance)
                    buysignal=self.EB.e.Go("Buy",BuyPrice, BuyAmount)
            if not self.InBuyFlag and BuyPrice>0:
                buyorderid=buysignal.wait()[0]
            if not self.InSellFlag and SellPrice>0:
                sellorderid=sellsignal.wait()[0]
            if EatMode:
                Sleep(2000)#委拍单200ms等待成交
            else:
                Sleep(60000)
            #检查委挂成交情况
            if not self.InSellFlag and SellPrice>0:
                #SellOrder=_C(self.EA.e.GetOrder, sellorderid)
                SellOrder=self.EA.e.GetOrder(sellorderid)
                #Log("SellOrder:",SellOrder)
            if not self.InBuyFlag and BuyPrice>0:    
                #BuyOrder=_C(self.EB.e.GetOrder, buyorderid)
                BuyOrder=self.EB.e.GetOrder(buyorderid)
                #Log("BuyOrder:",BuyOrder)
            if not self.InSellFlag and SellPrice>0:
                if SellOrder.DealAmount>0:
                    AvgPrice=SellOrder.AvgPrice
                    self.InSellPrice=(AvgPrice*SellOrder.DealAmount+self.InSellPrice*self.InSellAmount)/(self.InSellAmount+SellOrder.DealAmount)
                    #self.EA.LockStocks-=SellOrder.DealAmount
                    self.InSellAmount+=SellOrder.DealAmount
                    self.InSellFee+=SellOrder.DealAmount*AvgPrice*self.EA.TakerFee
                    self.EA.UpdateAccout()
                if SellOrder.DealAmount<SellOrder.Amount:
                    self.EA.e.CancelOrder(sellorderid)
                if SellOrder.Amount-SellOrder.DealAmount<=SlipCoin:
                    self.Log+="卖委拍成"
                    self.InSellFlag=True
            if not self.InBuyFlag and BuyPrice>0:
                if BuyOrder.DealAmount>0:
                    AvgPrice=BuyOrder.AvgPrice    
                    self.InBuyPrice=(AvgPrice*BuyOrder.DealAmount+self.InBuyPrice*self.InBuyAmount)/(self.InBuyAmount+BuyOrder.DealAmount)
                    #self.EB.LockBalance-=BuyOrder.DealAmount*AvgPrice
                    self.InBuyAmount+=BuyOrder.DealAmount
                    self.InBuyFee+=BuyOrder.DealAmount*AvgPrice*self.EB.TakerFee
                    self.EB.UpdateAccout()
                if BuyOrder.DealAmount<BuyOrder.Amount:
                    self.EB.e.CancelOrder(buyorderid)
                if BuyOrder.Amount-BuyOrder.DealAmount<=SlipCoin:
                    self.Log+="买委拍成"
                    self.InBuyFlag=True


           
            #委挂全部成交
            if self.PlanAmount-self.InSellAmount<=SlipCoin and self.PlanAmount-self.InBuyAmount<=SlipCoin:
                #self.Log+="(成)"
                self.Status=2
                break     
            BidsTimes+=1 
        if self.Status==2:
            self.Log+="[成]"
            Log("InSellAmount",self.InSellAmount,"InSellPrice",self.InSellPrice,"InBuyAmount",self.InBuyAmount,"InBuyPrice",self.InBuyPrice)
            self.InProfit=self.InSellAmount*self.InSellPrice-self.InBuyAmount*self.InBuyPrice
            self.InFee=self.InSellFee+self.InBuyFee
            self.Profit=self.InProfit-self.InFee
            self.OutTime=time.time()
            keyname=self.AeName+":"+self.BeName
            Log("update exchangediff keyname:",keyname)
            CompleteCountstr=exchangesdiffdf.ix[keyname,"CompleteCount"]  
            CompleteCountlist=CompleteCountstr.split("|",3)
            win=int(CompleteCountlist[0])
            lost=int(CompleteCountlist[1])
            total=win+lost
            exchangesdiffdf.ix[keyname,"Profit"]=exchangesdiffdf.ix[keyname,"Profit"]+self.Profit
            if self.Profit>=0:
                win+=1
                total=win+lost
                exchangesdiffdf.ix[keyname,"CompleteCount"]=str(win)+"|"+str(lost)+"|"+str(total)  

            else:
                lost+=1
                total=win+lost
                exchangesdiffdf.ix[keyname,"CompleteCount"]=str(win)+"|"+str(lost)+"|"+str(total) 
            self.EA.LockStocks-=self.LockStocks
            self.EB.LockBalance-=self.LockBalance
            Log("ID:",self.ID,"成：收益:",self.Profit)        
        return


#交易市场管理类，设计一个趋势判断

class ExchangeManage:
    def __init__(self):
        self.EM={}
        self.UpdateTime=1
        self.TotalInAmount=0
        self.Profit=0
        self.NETDiff=0
        self.TotalFee=0
        self.NoOpNET=0#无操作
        self.TotalInitBalance=0
        self.TotalInitStocks=0
        self.TotalInitNET=0
        self.DiffCoins=0
        self.DiffCoinsCost=0#币差成本
        self.TaskID=1#任务编号
        self.TaskQueue={}#正在进行的工作任务
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
        self.DiffCoins=0
        for i in self.EM:
            self.DiffCoins+=self.EM[i].Stocks-self.EM[i].InitStocks
            InDiffPrice = 0
            InMax = InDiffPrice
            InAvg = InDiffPrice

            ABuy=0
            ASell=0
            BBuy=0
            BSell=0
            Go=0
            for j in self.EM:
                if i != j:
                    keyname = self.EM[i].Name + ":" + self.EM[j].Name
                    InDiffPrice = ext.Cut((self.EM[i].Buy.Price - self.EM[j].Sell.Price)/self.EM[i].Buy.Price,6)  # 进场差价 在高价市场卖出，在低价市场买入 对应A B市场，A->B 进场 A卖出 B买入


                    ABuy=self.EM[i].Buy.Price
                    ASell=self.EM[i].Sell.Price
                    BBuy=self.EM[j].Buy.Price
                    BSell=self.EM[j].Sell.Price
                    Go=InDiffPrice-DiffAim
                    
                    

                    exchangesdiffdf.ix[keyname, 'KeyName'] = keyname
                    exchangesdiffdf.ix[keyname, 'ABuy'] = ABuy
              
                    exchangesdiffdf.ix[keyname, 'ACoin'] = self.EM[i].Stocks-self.EM[i].LockStocks
       
                    exchangesdiffdf.ix[keyname, 'BSell'] = BSell
                    exchangesdiffdf.ix[keyname, 'BBalance'] = self.EM[j].Balance-self.EM[j].LockBalance#账户留10块
                    exchangesdiffdf.ix[keyname, 'InDiff'] = InDiffPrice
                   
                    
                    exchangesdiffdf.ix[keyname, 'Go'] = Go
                    if self.UpdateTime<5:
                        exchangesdiffdf.ix[keyname,"Profit"]=0   
                        exchangesdiffdf.ix[keyname,"CompleteCount"]="0|0|0"              
                        #exchangesdiffdf.ix[keyname,"CompleteCount"]=str(tradinglist[keyname].WinTimes)+"|"+str(tradinglist[keyname].LoseTimes)+"|"+str(tradinglist[keyname].WinTimes+tradinglist[keyname].LoseTimes)

    def CheckIn(self):#进场检测
        df=exchangesdiffdf.loc[exchangesdiffdf["Go"]>GoT]
        #Log(df)
        ret=False
        if df is not None and len(df.index)>0:
            
            keyname=df.ix[0, 'KeyName']
            i=keyname.split(":")[0]
            j=keyname.split(":")[1]
            InDiffPrice=df.ix[0, 'InDiff']

           
         

            InSellAmount=self.EM[i].Buy.Amount   #Ea 市场买入份额，高价卖出对应的市场深度
            InBuyAmount=self.EM[j].Sell.Amount #Eb 市场卖出的份额，低价买入对应的市场深度


            CanBuyAmount=(exchangesdiffdf.ix[keyname, 'BBalance']-self.EM[j].Sell.Price)/self.EM[j].Sell.Price# 低价市场可以买入的币的总数

            PlanAmount=min((exchangesdiffdf.ix[keyname, 'ACoin']),InSellAmount/2,CanBuyAmount,InBuyAmount/2)#在市场深度 持有的数字货币和经费中选择最小的计划成交数量

            if PlanAmount<MinCoin:#计划交易的数额太小了，直接放弃
                return

            #进入交易模式 锁定 市场钱和币 开线程进入交易状态



            Log("开始交易:",self.EM[i].Name,"->",self.EM[j].Name," PlanAmount:",PlanAmount,"ACoin:",exchangesdiffdf.ix[keyname, 'ACoin'],"BBalance:",exchangesdiffdf.ix[keyname, 'BBalance'])

            tradepairdict[str(self.TaskID)]=TradePair(self.TaskID,self.EM[i],self.EM[j],PlanAmount,InDiffPrice)
            #tradepairdict[str(self.TaskID)].Trade()
            t=threading.Thread(target=tradepairdict[str(self.TaskID)].Trade,name=str(self.TaskID))
            t.start()
            #任务序号加加
            self.TaskID+=1

            #检查tradepairdict[key].Status==1的情况，部分成交
            # Log("准备【入场-卖出】市场：",self.EM[i].Name,"Sell At:",self.EM[i].Buy.Price,"Amount:",SellAmount,"AlreadyInSellPrice",AlreadyInSellPrice,"AlreadyInBuyPrice",AlreadyInBuyPrice)
            # sorderid=self.EM[i].e.Sell(self.EM[i].Buy.Price,SellAmount)
          
            # Log("准备【入场-买入】市场：",self.EM[j].Name,"Buy At:",self.EM[j].Sell.Price,"Amount:",BuyAmount,"AlreadyInSellPrice",AlreadyInSellPrice,"AlreadyInBuyPrice",AlreadyInBuyPrice)
            
            # borderid=self.EM[j].e.Buy(self.EM[j].Sell.Price,BuyAmount/(1-self.EM[j].Fee.Buy/94))
            #     ret=True
            # #休眠100毫秒

            # if ret:
            #     Sleep(100)
            # # 检查账户成交
            # eatradeamount=ebtradeamount=0
            # if SellAmount>=self.EM[i].MinStock:

            #     eatradeamount=self.EM[i].CheckTrade(SellAmount,sorderid,"s")
            #     if(eatradeamount+tradinglist[keyname].InSellAmount)>0:
            #         tradinglist[keyname].InSellPrice=round(((tradinglist[keyname].InSellAmount*tradinglist[keyname].InSellPrice+eatradeamount*self.EM[i].Buy.Price)/(tradinglist[keyname].InSellAmount+eatradeamount)),2)
            #     tradinglist[keyname].InSellAmount=round(tradinglist[keyname].InSellAmount+eatradeamount,5)
            #     Log("检查",self.EM[i].Name,"【入场-卖出】原币数",noweastocks,"当前币数",self.EM[i].Stocks,"成交数",eatradeamount,"InSellPrice",tradinglist[keyname].InSellPrice,"InSellAmount",tradinglist[keyname].InSellAmount)

            # if BuyAmount>=self.EM[j].MinStock:
            #     ebtradeamount=self.EM[j].CheckTrade(BuyAmount,borderid,"b")
            #     if(ebtradeamount+tradinglist[keyname].InBuyAmount)>0:
            #         tradinglist[keyname].InBuyPrice=round(((tradinglist[keyname].InBuyAmount*tradinglist[keyname].InBuyPrice+ebtradeamount*self.EM[j].Sell.Price)/(tradinglist[keyname].InBuyAmount+ebtradeamount)),2)
            #     tradinglist[keyname].InBuyAmount=round(tradinglist[keyname].InBuyAmount+ebtradeamount,5)
            #     Log("检查",self.EM[j].Name,"【入场-买入】原币数",nowebstocks,"当前币数",self.EM[j].Stocks,"成交数",ebtradeamount,"InBuyPrice",tradinglist[keyname].InBuyPrice,"InBuyAmount", tradinglist[keyname].InBuyAmount)

            # if tradinglist[keyname].InSellAmount<MaxCoinOnce:
            #     tradetasklistdf.ix[keyname,"InSell"]="in"
            # elif tradinglist[keyname].InSellAmount>=MaxCoinOnce-self.EM[i].MinStock:
            #     tradetasklistdf.ix[keyname,"InSell"]="ok"
            # if tradinglist[keyname].InBuyAmount<MaxCoinOnce:
            #     tradetasklistdf.ix[keyname,"InBuy"]="in"
            # elif tradinglist[keyname].InBuyAmount>=MaxCoinOnce-self.EM[j].MinStock:
            #     tradetasklistdf.ix[keyname,"InBuy"]="ok"
            # fee=(tradinglist[keyname].InSellAmount*tradinglist[keyname].InSellPrice)*self.EM[i].Fee.Sell/100+ tradinglist[keyname].InBuyPrice*(tradinglist[keyname].InBuyAmount/(1-self.EM[j].Fee.Buy/100))*self.EM[j].Fee.Buy/100
            # tradinglist[keyname].InFee=round(fee,4)
            # tradinglist[keyname].InProfit=tradinglist[keyname].InSellPrice*tradinglist[keyname].InSellAmount-tradinglist[keyname].InBuyPrice*tradinglist[keyname].InBuyAmount-tradinglist[keyname].InFee
            # tradinglist[keyname].InProfit=ext.Cut(tradinglist[keyname].InProfit,2)


            # if tradinglist[keyname].InSellAmount>=MaxCoinOnce-self.EM[i].MinStock and tradinglist[keyname].InBuyAmount>=MaxCoinOnce-self.EM[j].MinStock:
            #     tradetasklistdf.ix[keyname,"Status"]="waiting out"
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
        _str="市场总数"+str(totalexchange)+"初市值"+str(self.TotalInitNET)+"初钱"+str(self.TotalInitBalance)+"初币"+str(self.TotalInitStocks)+"总钱"+str(totalbalance)+"总冻结钱"+str(totalfrozenbalance)+"总币"+str(totalstocks)+"总冻结币"+str(totalfrozenstocks)+"总市值"+str(totalnet)+"无操浮盈"+str(self.NoOpNET)+"运行净值差"+str(self.NETDiff)
        return _str
    #计算收益率
    def CalProfit(self):
        total=0
        today=0
        thisweek=0
        totaltimes=0
        totalwintimes=0
        totallosetimes=0
        totalfee=0
        daywintimes=0
        daylosetimes=0
        mstr=""
        nowtimestamp=time.time()
        todaystarttimestamp=nowtimestamp-86400 #从当前算起 1日起点
        thisweektimestamp=nowtimestamp-86400*7#从当前算起 7日起点
        for i in range(len(histradelist)):

            if histradelist[i].OutTime>todaystarttimestamp:
                if histradelist[i].Profit>0:
                    daywintimes+=1
                else:
                    daylosetimes+=1
                today+=histradelist[i].Profit
            if histradelist[i].OutTime>thisweektimestamp:
                thisweek+=histradelist[i].Profit
            if histradelist[i].Profit>0:
                totalwintimes+=1
            else:
                totallosetimes+=1
            total+=histradelist[i].Profit
            totalfee+=histradelist[i].InFee
        dayp=today/self.TotalInitNET*100
        #sdayp=thisweek/self.TotalInitNET*100
        yp=dayp*365
        mp=dayp*30
        tp=total/self.TotalInitNET*100
        daywinp=0
        if daywintimes>0:
            daywinp=100*daywintimes/(daywintimes+daylosetimes)
        totalwinp=0
        if totalwintimes>0:
            totalwinp=100*totalwintimes/(totalwintimes+totallosetimes)
        mstr+="总交易费用估计折合CNY:"+str(round(totalfee,2))+"元\n"
        mstr+="1日收："+str(ext.Cut(today,2))+"元 7日收："+str(ext.Cut(thisweek,2))+" 日"+str(ext.Cut(dayp,4))+"% 月"+str(ext.Cut(mp,2))+"% 年"+str(ext.Cut(yp,2))+"%\n"
        mstr+="总收："+str(ext.Cut(total,2))+"元 "+str(ext.Cut(tp,2))+"%\n"
        mstr+="1日操作:"+str(daywintimes+daylosetimes)+"次"+str(daywintimes)+"胜"+str(daylosetimes)+"负 胜率:"+str(daywinp)+"%\n"
        mstr+="总操作:"+str(totalwintimes+totallosetimes)+"次"+str(totalwintimes)+"胜"+str(totallosetimes)+"负 胜率:"+str(totalwinp)+"%\n"
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
        
        self.LastUpdateTime=0
        self.AccountUpdateTimes=0
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
        if time.time()-self.LastUpdateTime<0.050:
            return
        times=time.time()
        #dwait=self.e.Go("GetDepth")
        #depth=dwait.wait()[0]
        depth=self.e.GetDepth()

        dtimee=time.time()
        self.LastUpdateTime=dtimee
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
        self.AccountUpdateTimes+=1
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
        exchangesdf.ix[self.Name,'LockBalance']=self.LockBalance
        exchangesdf.ix[self.Name,'LockStocks']=self.LockStocks
        exchangesdf.ix[self.Name,'InitBalance']=self.InitBalance
        exchangesdf.ix[self.Name,'InitStocks']=self.InitStocks
        exchangesdf.ix[self.Name,'InitNET']=self.InitNET
        exchangesdf.ix[self.Name,'NET']=self.NET
        exchangesdf.ix[self.Name,'Profit']=self.Profit
        exchangesdf.ix[self.Name,'UpdateTime']=str(self.UpdateTime)+"|"+str(self.AccountUpdateTimes)


class TradeResault:
    def __init__(self,_direction,_amount,_price):
        self.Direction=_direction
        self.Amount=_amount
        self.Price=_price
