namespace CoinNews.Northwind {
    export enum OrderShippingState {
        NotShipped = 0,
        Shipped = 1
    }
    Serenity.Decorators.registerEnumType(OrderShippingState, 'CoinNews.Northwind.OrderShippingState', 'Northwind.OrderShippingState');
}

