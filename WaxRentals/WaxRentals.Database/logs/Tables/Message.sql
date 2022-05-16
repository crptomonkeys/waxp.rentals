CREATE TABLE [logs].[Message]
(
    MessageId      INT              NOT NULL IDENTITY(1,1) CONSTRAINT [PK_logs_Message]            PRIMARY KEY
	,Inserted      DATETIME2(0)     NOT NULL               CONSTRAINT [DF_logs_Message__Inserted]  DEFAULT GETUTCDATE()

	,RequestId     UNIQUEIDENTIFIER NOT NULL

	,Url           VARCHAR(MAX)     NOT NULL               CONSTRAINT [CK_logs_Message__Url]       CHECK   (LEN(Url) > 0)
	,Direction     VARCHAR(3)       NOT NULL               CONSTRAINT [CK_logs_Message__Direction] CHECK   (Direction IN ('In', 'Out'))
	,MessageObject VARCHAR(MAX)     NOT NULL
)
