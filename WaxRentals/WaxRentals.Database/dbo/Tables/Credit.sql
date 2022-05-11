CREATE TABLE [dbo].[Credit]
(
	CreditId           INT           IDENTITY(1,1) NOT NULL CONSTRAINT PK_Credit                    PRIMARY KEY
	,Inserted          DATETIME2(0)                NOT NULL CONSTRAINT DF_Credit__Inserted          DEFAULT (GETUTCDATE())
	,AccountId         INT                         NOT NULL CONSTRAINT FK_Credit__Account           FOREIGN KEY REFERENCES dbo.Account (AccountId)
	,Banano            DECIMAL(18,8)               NOT NULL CONSTRAINT CK_Credit__Banano            CHECK   (Banano                 >= 0 )
	,BananoTransaction CHAR(64)                    NOT NULL CONSTRAINT CK_Credit__BananoTransaction CHECK   (LEN(BananoTransaction)  = 64)
	                                                        CONSTRAINT UQ_Credit__BananoTransaction UNIQUE (BananoTransaction)
	,StatusId          INT                         NOT NULL CONSTRAINT FK_Credit__Status            FOREIGN KEY REFERENCES dbo.Status (StatusId)
	                                                        CONSTRAINT DF_Credit__StatusId          DEFAULT (1)
)
