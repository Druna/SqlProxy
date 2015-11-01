CREATE TABLE [dbo].[OrderItem]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ProductId] INT NOT NULL, 
    [OrderId] INT NOT NULL, 
    [Quantity] INT NOT NULL, 
    CONSTRAINT [FK_OrderItem_Order] FOREIGN KEY ([OrderId]) REFERENCES [Order]([Id]), 
    CONSTRAINT [FK_OrderItem_Product] FOREIGN KEY ([ProductId]) REFERENCES [Product]([Id])
)
