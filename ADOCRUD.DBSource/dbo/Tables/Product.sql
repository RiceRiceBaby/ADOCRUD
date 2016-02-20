CREATE TABLE [dbo].[Product] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [CategoryId]  INT            NOT NULL,
    [Name]        VARCHAR (50)   NOT NULL,
    [Price]       DECIMAL (5, 2) NOT NULL,
    [Description] VARCHAR (255)  NOT NULL,
    CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Product_Category] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Category] ([Id])
);

