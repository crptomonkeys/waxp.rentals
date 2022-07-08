CREATE VIEW [welcome].[Packages]
AS
	SELECT Package.*, Address
	FROM welcome.Package
		 LEFT JOIN welcome.Address ON Package.PackageId = Address.AddressId;
