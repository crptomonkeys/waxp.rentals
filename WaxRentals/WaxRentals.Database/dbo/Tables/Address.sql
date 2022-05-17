CREATE TABLE [dbo].[Address]
(
	AddressId  INT      NOT NULL CONSTRAINT PK_Address         PRIMARY KEY
	,Address   CHAR(64) NOT NULL CONSTRAINT UQ_Address_Address UNIQUE (Address)
	,Work      CHAR(16)     NULL
)
