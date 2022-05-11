CREATE TABLE [dbo].[Payment]
(
	PaymentId         INT           IDENTITY(1,1) NOT NULL CONSTRAINT PK_Payment                   PRIMARY KEY                                 ,
	Inserted          DATETIME2(0)                NOT NULL CONSTRAINT DF_Payment_Inserted          DEFAULT ( GETUTCDATE()                 )    ,
	WaxAccount        VARCHAR(12)                 NOT NULL CONSTRAINT CK_Payment_WaxAddress        CHECK   ( LEN(WaxAccount)        >  0  )    ,
	Wax               DECIMAL(18,8)               NOT NULL CONSTRAINT CK_Payment_Wax               CHECK   ( Wax                    >= 0  )    ,
	WaxTransaction    CHAR(64)                    NOT NULL CONSTRAINT CK_Payment_WaxTransaction    CHECK   ( LEN(WaxTransaction)     = 64 )    
	                                                       CONSTRAINT UQ_Payment_WaxTransaction    UNIQUE (WaxTransaction)                     ,
    BananoAddress     CHAR(64)                        NULL CONSTRAINT CK_Payment_BananoAddress     CHECK   ( LEN(BananoAddress)      = 64 )    ,
	Banano            DECIMAL(18,8)               NOT NULL CONSTRAINT CK_Payment_Banano            CHECK   ( Banano                 >= 0  )    ,
	BananoTransaction CHAR(64)                        NULL CONSTRAINT CK_Payment_BananoTransaction CHECK   ( LEN(BananoTransaction)  = 64 )     
	                                                       CONSTRAINT UQ_Payment_BananoTransaction UNIQUE (BananoTransaction)                  ,
	StatusId          INT                         NOT NULL CONSTRAINT FK_Payment_Status            FOREIGN KEY REFERENCES dbo.Status (StatusId)
	                                                       CONSTRAINT DF_Payment_StatusId          DEFAULT ( 1                            )
)
