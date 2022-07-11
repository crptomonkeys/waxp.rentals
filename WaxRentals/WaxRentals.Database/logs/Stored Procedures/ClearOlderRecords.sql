CREATE PROCEDURE [logs].[ClearOlderRecords]
AS
BEGIN

    DECLARE @ClearDate DATETIME = DATEADD(day, -1, GETUTCDATE());

    DELETE FROM logs.Error WHERE Inserted < @ClearDate;
    DELETE FROM logs.Message WHERE Inserted < @ClearDate;

END
