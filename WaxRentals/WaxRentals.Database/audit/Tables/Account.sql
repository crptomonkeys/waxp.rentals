CREATE TABLE [audit].[Account]
(
    [AuditId]         INT          NOT NULL IDENTITY(1,1) CONSTRAINT [PK_Audit_Creator]                 PRIMARY KEY
	,[AuditEvent]     CHAR(6)      NOT NULL
	,[AuditTimestamp] DATETIME2(0) NOT NULL               CONSTRAINT [DF_Audit_Creator__AuditTimestamp] DEFAULT (GETUTCDATE())

	,AccountId   INT           NULL
	,Inserted    DATETIME2(0)  NULL
	,WaxAccount  VARCHAR(12)   NULL
	,CPU         DECIMAL(18,8) NULL
	,NET         DECIMAL(18,8) NULL
	,PaidThrough DATETIME2(0)  NULL
)
