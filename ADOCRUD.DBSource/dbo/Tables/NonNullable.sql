CREATE TABLE [dbo].[NonNullable] (
    [Id]                  INT                IDENTITY (1, 1) NOT NULL,
    [Integer64Value]      BIGINT             NOT NULL,
    [ByteValue]           TINYINT            NOT NULL,
    [ByteArrayValue]      VARBINARY (255)    NOT NULL,
    [BoolValue]           BIT                NOT NULL,
    [CharValue]           VARCHAR (1)        NOT NULL,
    [DateTimeValue]       DATETIME           NOT NULL,
    [DateTimeOffsetValue] DATETIMEOFFSET (7) NOT NULL,
    [DecimalValue]        DECIMAL (5, 2)     NOT NULL,
    [FloatValue]          FLOAT (53)         NOT NULL,
    [DoubleValue]         FLOAT (53)         NOT NULL,
    [Integer32Value]      INT                NOT NULL,
    [Single]              REAL               NOT NULL,
    [Integer16Value]      SMALLINT           NOT NULL,
    [GuidValue]           UNIQUEIDENTIFIER   NOT NULL,
    [StringValue]         VARCHAR (50)       NOT NULL,
    CONSTRAINT [PK_NonNullable] PRIMARY KEY CLUSTERED ([Id] ASC)
);



