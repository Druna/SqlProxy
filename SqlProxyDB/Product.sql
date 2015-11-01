﻿CREATE TABLE [dbo].[Product]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Price] DECIMAL(18, 2) NOT NULL, 
    [Name] NVARCHAR(150) NOT NULL
)