CREATE PROCEDURE [dbo].[PullNextPurchase]
AS
BEGIN

    -- https://stackoverflow.com/a/37800044/128217
	UPDATE Purchase
    SET    StatusId = 2
    OUTPUT INSERTED.*
    WHERE  PurchaseId =
    (
        SELECT TOP 1 PurchaseId
        FROM Purchase WITH (UPDLOCK)
        WHERE StatusId = 1
        ORDER BY PurchaseId
    );

END
