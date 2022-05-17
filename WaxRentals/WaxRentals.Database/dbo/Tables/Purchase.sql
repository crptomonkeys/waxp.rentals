CREATE TABLE [dbo].[Purchase]
(
	PurchaseId            INT           IDENTITY(1,1) NOT NULL CONSTRAINT PK_Purchase                       PRIMARY KEY
	,Inserted             DATETIME2(0)                NOT NULL CONSTRAINT DF_Purchase__Inserted             DEFAULT GETUTCDATE()

	,Wax                  DECIMAL(18,8)               NOT NULL CONSTRAINT CK_Purchase__Wax                  CHECK   (Wax                       >= 0 )
	,WaxTransaction       CHAR(64)                    NOT NULL CONSTRAINT CK_Purchase__WaxTransaction       CHECK   (LEN(WaxTransaction)        = 64)
	                                                           CONSTRAINT UQ_Purchase__WaxTransaction       UNIQUE  (WaxTransaction)

    ,PaymentBananoAddress CHAR(64)                        NULL CONSTRAINT CK_Purchase__PaymentBananoAddress CHECK   (LEN(PaymentBananoAddress)  = 64)

	,Banano               DECIMAL(18,8)               NOT NULL CONSTRAINT CK_Purchase__Banano               CHECK   (Banano                    >= 0 )
	,BananoTransaction    CHAR(64)                        NULL CONSTRAINT CK_Purchase__BananoTransaction    CHECK   (LEN(BananoTransaction)     = 64)

	,StatusId             INT                         NOT NULL CONSTRAINT FK_Purchase__Status               FOREIGN KEY REFERENCES dbo.Status (StatusId)
	                                                           CONSTRAINT DF_Purchase__StatusId             DEFAULT 1
)
