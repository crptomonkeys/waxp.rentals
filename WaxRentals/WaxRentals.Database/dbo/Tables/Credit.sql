CREATE TABLE [dbo].[Credit]
(
	CreditId          INT           IDENTITY(1,1) NOT NULL CONSTRAINT PK_Credit                   PRIMARY KEY                                   ,
	Inserted          DATETIME2(0)                NOT NULL CONSTRAINT DF_Credit_Inserted          DEFAULT ( GETUTCDATE()                )       ,
	AccountId         INT                         NOT NULL CONSTRAINT FK_Credit_Account           FOREIGN KEY REFERENCES dbo.Account (AccountId),
	Banano            DECIMAL(18,8)               NOT NULL CONSTRAINT CK_Credit_Banano            CHECK   ( Banano       >= 1           )       ,
	BananoTransaction CHAR(64)                        NULL CONSTRAINT CK_Credit_BananoTransaction CHECK   ( LEN(BananoTransaction) = 64 )       ,
	                                                       CONSTRAINT UQ_Credit_BananoTransaction UNIQUE (BananoTransaction)                    ,
	StatusId          INT                         NOT NULL CONSTRAINT FK_Credit_Status            FOREIGN KEY REFERENCES dbo.Status (StatusId)
	                                                       CONSTRAINT DF_Credit_StatusId          DEFAULT ( 1                           )
)
