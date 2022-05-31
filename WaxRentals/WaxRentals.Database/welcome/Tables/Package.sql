CREATE TABLE [welcome].[Package]
(
	PackageId               INT          IDENTITY(1,1) NOT NULL CONSTRAINT PK_welcome_Package                         PRIMARY KEY
	,Inserted               DATETIME2(0)               NOT NULL CONSTRAINT DF_welcome_Package__Inserted               DEFAULT GETUTCDATE()
	
	,TargetWaxAccount       VARCHAR(12)                NOT NULL CONSTRAINT CK_welcome_Package__TargetWaxAddress       CHECK   (LEN(TargetWaxAccount)       >  0 )
	,Memo                   VARCHAR(20)                NOT NULL CONSTRAINT CK_welcome_Package__Memo                   CHECK   (LEN(Memo)                   >  0 )
	,Wax                    DECIMAL(18,8)              NOT NULL CONSTRAINT CK_welcome_Package__Wax                    CHECK   (Wax                         >= 0 )
    
	,Banano                 DECIMAL(18,8)              NOT NULL CONSTRAINT CK_welcome_Package__Banano                 CHECK   (Banano                      >= 0 )
	,SweepBananoTransaction CHAR(64)                       NULL CONSTRAINT CK_welcome_Package__SweepBananoTransaction CHECK   (LEN(SweepBananoTransaction)  = 64)
	,Paid                   DATETIME2(0)                   NULL
															  
	,FundTransaction        CHAR(64)                       NULL CONSTRAINT CK_welcome_Package__FundTransaction        CHECK   (LEN(FundTransaction)         = 64)
	,NftTransaction         CHAR(64)                       NULL CONSTRAINT CK_welcome_Package__NftTransaction         CHECK   (LEN(NftTransaction)          = 64)
	,RentalId               INT                            NULL CONSTRAINT FK_welcome_Package__Rental                 FOREIGN KEY REFERENCES dbo.Rental (RentalId)

	,StatusId               INT                        NOT NULL CONSTRAINT FK_welcome_Package__Status                 FOREIGN KEY REFERENCES dbo.Status (StatusId)
	                                                            CONSTRAINT DF_welcome_Package__StatusId               DEFAULT 1
)
