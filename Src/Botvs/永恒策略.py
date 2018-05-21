# botvs@7190d0470fa27a9a68d3f4a6e700197a
import numpy as np
import pandas as pa
import json
import time
import copy
import threading
import traceback
#Vstoplostpct 策略止损百分比 可设置参数

def main():
    LastTradePrice=0 #上次成交价格
    LastTradeTime=time.time()#上次成交时间
    SellOut=False
    Log(exchange.GetAccount())
    while True:
    
        ticker=exchange.GetTicker()
        account=exchange.GetAccount()
        canbuyamount=_N(account["Balance"]/ticker["Sell"],0)
        if LastTradePrice==0:
            Log("初始买入 amount:",canbuyamount,"price:",ticker["Sell"])
            exchange.Buy(ticker["Sell"],canbuyamount);
            LastTradePrice=ticker["Sell"]
            LastTradeTime=time.time()
            account=exchange.GetAccount()
            Log(exchange.GetAccount())
            continue
        
        if ((ticker["Last"]/LastTradePrice)<(1-Vstoplostpct)) and (SellOut==False):
            Log("卖出信号 当前价：",ticker["Last"],"上次成交价：",LastTradePrice,"比值：",ticker["Last"]/LastTradePrice," amount:",account["Stocks"],"price:",ticker["Sell"])
            exchange.Sell(ticker["Buy"], account["Stocks"])
            LastTradePrice=ticker["Buy"]
            LastTradeTime=time.time()
            SellOut=True
            account=exchange.GetAccount()
            Log(exchange.GetAccount())
            continue
        
        if SellOut and (ticker["Sell"]>LastTradePrice*0.99) and (ticker["Sell"]<LastTradePrice*1.01):
            Log("买入信号 当前价：",ticker["Sell"],"上次成交价：",LastTradePrice,"比值：",ticker["Last"]/LastTradePrice," amount:",canbuyamount,"price:",ticker["Sell"])
            exchange.Buy(ticker["Sell"],canbuyamount);
            LastTradePrice=ticker["Sell"]
            LastTradeTime=time.time()
            SellOut=False
            account=exchange.GetAccount()
            Log(exchange.GetAccount())
            continue
        
        Sleep(60*1000)

    


