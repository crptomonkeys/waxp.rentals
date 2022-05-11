CREATE TABLE [dbo].[Account]
(
	AccountId    INT          IDENTITY(1,1) NOT NULL CONSTRAINT PK_Account             PRIMARY KEY
	                                                 CONSTRAINT FK_Account__Address    FOREIGN KEY REFERENCES dbo.Address (AddressId)
	,Inserted    DATETIME2(0)               NOT NULL CONSTRAINT DF_Account__Inserted   DEFAULT (GETUTCDATE()        )
	,WaxAccount  VARCHAR(12)                NOT NULL CONSTRAINT CK_Account__WaxAddress CHECK   (LEN(WaxAccount) >  0)
	,CPU         DECIMAL(18,8)              NOT NULL CONSTRAINT CK_Account__Cpu        CHECK   (CPU             >= 0)
	,NET         DECIMAL(18,8)              NOT NULL CONSTRAINT CK_Account__Net        CHECK   (NET             >= 0)
	                                                 CONSTRAINT CK_Account__CPU_NET    CHECK   (CPU + NET       >  0)
	,PaidThrough DATETIME2(0)                   NULL
)
GO

CREATE TRIGGER [dbo].[Account__Audit]
ON [dbo].[Account]
AFTER INSERT, UPDATE
AS
BEGIN
    DECLARE @Event CHAR(6) = 'INSERT'
	SELECT @Event = 'UPDATE' FROM [DELETED];

	INSERT INTO [audit].[Account] (AuditEvent, AccountId, Inserted, WaxAccount, CPU, NET, PaidThrough)
	SELECT @Event, * FROM INSERTED;
END