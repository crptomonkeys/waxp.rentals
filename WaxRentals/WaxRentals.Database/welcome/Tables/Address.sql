CREATE TABLE [welcome].[Address]
(
	AddressId  INT      NOT NULL CONSTRAINT PK_welcome_Address         PRIMARY KEY
	,Address   CHAR(64) NOT NULL CONSTRAINT UQ_welcome_Address_Address UNIQUE (Address)
)
