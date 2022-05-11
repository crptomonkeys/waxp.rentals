CREATE PROCEDURE [dbo].[PullNextPayment]
AS
BEGIN

    -- https://stackoverflow.com/a/37800044/128217
	UPDATE Payment
    SET    StatusId = 2
    OUTPUT INSERTED.*
    WHERE  PaymentId =
    (
        SELECT TOP 1 PaymentId
        FROM Payment WITH (UPDLOCK)
        WHERE StatusId = 1
        ORDER BY PaymentId
    );

END
