CREATE TABLE [dbo].[UserProduct] (
    [UserId]    INT NOT NULL,
    [ProductId] INT NOT NULL,
    CONSTRAINT [PK_UserProduct] PRIMARY KEY CLUSTERED ([UserId] ASC, [ProductId] ASC)
);

