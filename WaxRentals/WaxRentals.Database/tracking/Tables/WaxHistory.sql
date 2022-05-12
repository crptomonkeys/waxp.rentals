CREATE TABLE [tracking].[WaxHistory]
(
	WaxHistoryId INT          IDENTITY(1,1) NOT NULL CONSTRAINT PK_tracking_WaxHistory PRIMARY KEY
	,Inserted    DATETIME2(0)               NOT NULL CONSTRAINT DF_tracking__Inserted  DEFAULT (GETUTCDATE())
	,LastRun     DATETIME2(3)               NOT NULL
)
