CREATE TABLE [dbo].[Payment]
(
	PaymentId          INT           IDENTITY(1,1) NOT NULL CONSTRAINT PK_Payment                    PRIMARY KEY
	,Inserted          DATETIME2(0)                NOT NULL CONSTRAINT DF_Payment__Inserted          DEFAULT (GETUTCDATE())
	,WaxAccount        VARCHAR(12)                 NOT NULL CONSTRAINT CK_Payment__WaxAddress        CHECK   (LEN(WaxAccount)        >  0 )
	,Wax               DECIMAL(18,8)               NOT NULL CONSTRAINT CK_Payment__Wax               CHECK   (Wax                    >= 0 )
	,WaxTransaction    CHAR(64)                    NOT NULL CONSTRAINT CK_Payment__WaxTransaction    CHECK   (LEN(WaxTransaction)     = 64)
	                                                        CONSTRAINT UQ_Payment__WaxTransaction    UNIQUE (WaxTransaction)
    ,BananoAddress     CHAR(64)                        NULL CONSTRAINT CK_Payment__BananoAddress     CHECK   (LEN(BananoAddress)      = 64)
	,Banano            DECIMAL(18,8)               NOT NULL CONSTRAINT CK_Payment__Banano            CHECK   (Banano                 >= 0 )
	,BananoTransaction CHAR(64)                        NULL CONSTRAINT CK_Payment__BananoTransaction CHECK   (LEN(BananoTransaction)  = 64)
	                                                        CONSTRAINT UQ_Payment__BananoTransaction UNIQUE (BananoTransaction)
	,StatusId          INT                         NOT NULL CONSTRAINT FK_Payment__Status            FOREIGN KEY REFERENCES dbo.Status (StatusId)
	                                                        CONSTRAINT DF_Payment__StatusId          DEFAULT (1)
)
