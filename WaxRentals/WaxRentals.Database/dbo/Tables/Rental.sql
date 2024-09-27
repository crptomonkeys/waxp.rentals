﻿CREATE TABLE [dbo].[Rental]
(
	RentalId                INT          IDENTITY(1,1) NOT NULL CONSTRAINT PK_Rental                         PRIMARY KEY
	,Inserted               DATETIME2(0)               NOT NULL CONSTRAINT DF_Rental__Inserted               DEFAULT GETUTCDATE()
	
	,TargetWaxAccount       VARCHAR(12)                NOT NULL CONSTRAINT CK_Rental__TargetWaxAddress       CHECK   (LEN(TargetWaxAccount)       >  0 )
    ,RentalDays             INT                        NOT NULL CONSTRAINT CK_Rental__RentalDays             CHECK   (RentalDays                  >  0 )
	,CPU                    DECIMAL(18,8)              NOT NULL CONSTRAINT CK_Rental__Cpu                    CHECK   (CPU                         >= 0 )
	,NET                    DECIMAL(18,8)              NOT NULL CONSTRAINT CK_Rental__Net                    CHECK   (NET                         >= 0 )
	                                                            CONSTRAINT CK_Rental__CPU_NET                CHECK   (CPU + NET                   >  0 )

	,Banano                 DECIMAL(18,8)              NOT NULL CONSTRAINT CK_Rental__Banano                 CHECK   (Banano                      >= 0 )
	,SweepBananoTransaction CHAR(64)                       NULL CONSTRAINT CK_Rental__SweepBananoTransaction CHECK   (LEN(SweepBananoTransaction)  = 64)
	,Paid                   DATETIME2(0)                   NULL
	,Staked                 DATETIME2(0)                   NULL
	,PaidThrough                                                AS DATEADD(day, RentalDays, Staked) PERSISTED
															  
	,SourceWaxAccount       CHAR(12)                       NULL CONSTRAINT CK_Rental__SourceWaxAddress       CHECK   (LEN(SourceWaxAccount)       >  0 )
	,StakeWaxTransaction    CHAR(64)                       NULL CONSTRAINT CK_Rental__StakeWaxTransaction    CHECK   (LEN(StakeWaxTransaction)     = 64)
	,UnstakeWaxTransaction  CHAR(64)                       NULL CONSTRAINT CK_Rental__UnstakeWaxTransaction  CHECK   (LEN(UnstakeWaxTransaction)   = 64)

	,StatusId               INT                        NOT NULL CONSTRAINT FK_Rental__Status                 FOREIGN KEY REFERENCES dbo.Status (StatusId)
	                                                            CONSTRAINT DF_Rental__StatusId               DEFAULT 1
)
