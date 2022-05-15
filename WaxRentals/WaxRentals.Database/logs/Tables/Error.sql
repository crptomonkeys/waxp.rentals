CREATE TABLE [logs].[Error]
(
    ErrorId          INT          NOT NULL IDENTITY(1,1) CONSTRAINT [PK_logs_Error]           PRIMARY KEY
	,Inserted        DATETIME2(0) NOT NULL               CONSTRAINT [DF_logs_Error__Inserted] DEFAULT (GETUTCDATE())

	,Context         VARCHAR(MAX)     NULL

	,Message         VARCHAR(MAX) NOT NULL
	,StackTrace      VARCHAR(MAX) NOT NULL
	,TargetSite      VARCHAR(MAX) NOT NULL
	,InnerExceptions VARCHAR(MAX)     NULL
)
