CREATE PROCEDURE [dbo].[PullNextCredit]
AS
BEGIN

    -- https://stackoverflow.com/a/37800044/128217
	UPDATE Credit
    SET    StatusId = 2
    OUTPUT INSERTED.*
    WHERE  CreditId =
    (
        SELECT TOP 1 CreditId
        FROM Credit WITH (UPDLOCK)
        WHERE StatusId = 1
        ORDER BY CreditId
    );

END
