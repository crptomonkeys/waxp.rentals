CREATE TABLE [dbo].[Account]
(
	AccountId   INT          IDENTITY(1,1) NOT NULL CONSTRAINT PK_Account            PRIMARY KEY                     ,
	Inserted    DATETIME2(0)               NOT NULL CONSTRAINT DF_Account_Inserted   DEFAULT ( GETUTCDATE()         ),
	WaxAccount  VARCHAR(12)                NOT NULL CONSTRAINT CK_Account_WaxAddress CHECK   ( LEN(WaxAccount) >  0 ),
	CPU         INT                        NOT NULL CONSTRAINT CK_Account_Cpu        CHECK   ( CPU             >= 0 ),
	NET         INT                        NOT NULL CONSTRAINT CK_Account_Net        CHECK   ( NET             >= 0 ),
	                                                CONSTRAINT CK_Account_CPU_NET    CHECK   ( CPU + NET       >  0 ),
	PaidThrough DATETIME2(0)                   NULL                                                                  
)
