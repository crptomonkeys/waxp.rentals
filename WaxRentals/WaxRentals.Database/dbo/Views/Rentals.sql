CREATE VIEW [dbo].[Rentals]
AS
	SELECT Rental.*, Address
	FROM dbo.Rental
		 LEFT JOIN dbo.Address ON Rental.RentalId = Address.AddressId;
