CREATE TABLE [dbo].[Nullable] (
    [Id]                  INT                IDENTITY (1, 1) NOT NULL,
    [Integer64Value]      BIGINT             NULL,
    [ByteValue]           TINYINT            NULL,
    [ByteArrayValue]      VARBINARY (255)    NULL,
    [BoolValue]           BIT                NULL,
    [CharValue]           VARCHAR (1)        NULL,
    [DateTimeValue]       DATETIME           NULL,
    [DateTimeOffsetValue] DATETIMEOFFSET (7) NULL,
    [DecimalValue]        DECIMAL (5, 2)     NULL,
    [FloatValue]          FLOAT (53)         NULL,
    [DoubleValue]         FLOAT (53)         NULL,
    [Integer32Value]      INT                NULL,
    [Single]              REAL               NULL,
    [Integer16Value]      SMALLINT           NULL,
    [GuidValue]           UNIQUEIDENTIFIER   NULL,
    [StringValue]         VARCHAR (50)       NULL,
    CONSTRAINT [PK_Nullable] PRIMARY KEY CLUSTERED ([Id] ASC)
);



