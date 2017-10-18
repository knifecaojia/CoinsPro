using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCAPI_WSS
{
    public enum MessageTypes
    {

        TRADE = 0,
        FEEDNEWS = 1,
        CURRENT = 2,
        LOADCOMPLATE = 3,
        COINPAIRS = 4,
        CURRENTAGG = 5,
        TOPLIST = 6,
        TOPLISTCHANGE = 7,
        ORDERBOOK = 8,
        FULLORDERBOOK = 9,
        ACTIVATION = 10,
        TRADECATCHUP = 100,
        NEWSCATCHUP = 101,
        TRADECATCHUPCOMPLETE = 300,
        NEWSCATCHUPCOMPLETE = 301,

    }
    public enum TradeFlags
    {

        SEL = 0x1, // hex for binary 1
        BUY = 0x2, // hex for binary 10
        UNKNOWN = 0x4, // hex for binary 100
    }
    public enum TradeFields
    {

        T = 0x0,  // hex for binary 0, it is a special case of fields that are always there TYPE
        M = 0x0, // hex for binary 0, it is a special case of fields that are always there MARKET
        FSYM = 0x0,  // hex for binary 0, it is a special case of fields that are always there FROM SYMBOL
        TSYM = 0x0,  // hex for binary 0, it is a special case of fields that are always there TO SYMBOL
        F = 0x0,  // hex for binary 0, it is a special case of fields that are always there FLAGS
        ID = 0x1, // hex for binary 1                                                       ID
        TS = 0x2, // hex for binary 10                                                      TIMESTAMP
        Q = 0x4, // hex for binary 100                                                     QUANTITY
        P = 0x8,  // hex for binary 1000                                                    PRICE
        TOTAL = 0x10,// hex for binary 10000            
    }
    public enum CurrentFlags
    {

        PRICEUP = 0x1,    // hex for binary 1
        PRICEDOWN = 0x2,   // hex for binary 10
        PRICEUNCHANGED = 0x4,   // hex for binary 100
        BIDUP = 0x8,    // hex for binary 1000
        BIDDOWN = 0x10,   // hex for binary 10000
        BIDUNCHANGED = 0x20,   // hex for binary 100000
        OFFERUP = 0x40,   // hex for binary 1000000
        OFFERDOWN = 0x80,  // hex for binary 10000000
        OFFERUNCHANGED = 0x100,  // hex for binary 100000000
        AVGUP = 0x200, // hex for binary 1000000000
        AVGDOWN = 0x400,  // hex for binary 10000000000
        AVGUNCHANGED = 0x800,  // hex for binary 100000000000

    }
    public enum CurrentFields
    {
        TYPE = 0x0,      // hex for binary 0, it is a special case of fields that are always there
        MARKET = 0x0,     // hex for binary 0, it is a special case of fields that are always there
        FROMSYMBOL = 0x0,    // hex for binary 0, it is a special case of fields that are always there
        TOSYMBOL = 0x0,    // hex for binary 0, it is a special case of fields that are always there
        FLAGS = 0x0,     // hex for binary 0, it is a special case of fields that are always there
        PRICE = 0x1,     // hex for binary 1
        BID = 0x2,     // hex for binary 10
        OFFER = 0x4,    // hex for binary 100
        LASTUPDATE = 0x8,    // hex for binary 1000
        AVG = 0x10,    // hex for binary 10000
        LASTVOLUME = 0x20,    // hex for binary 100000
        LASTVOLUMETO = 0x40,    // hex for binary 1000000
        LASTTRADEID = 0x80,    // hex for binary 10000000
        VOLUMEHOUR = 0x100,    // hex for binary 100000000
        VOLUMEHOURTO = 0x200,   // hex for binary 1000000000
        VOLUME24HOUR = 0x400,  // hex for binary 10000000000
        VOLUME24HOURTO = 0x800,   // hex for binary 100000000000
        OPENHOUR = 0x1000,   // hex for binary 1000000000000
        HIGHHOUR = 0x2000,  // hex for binary 10000000000000
        LOWHOUR = 0x4000,   // hex for binary 100000000000000
        OPEN24HOUR = 0x8000,  // hex for binary 1000000000000000
        HIGH24HOUR = 0x10000,  // hex for binary 10000000000000000
        LOW24HOUR = 0x20000,  // hex for binary 100000000000000000
        LASTMARKET = 0x40000,  // hex for binary 1000000000000000000, this is a special case and will only appear on CCCAGG messages
    }
    class Utility
    {
    }
}
