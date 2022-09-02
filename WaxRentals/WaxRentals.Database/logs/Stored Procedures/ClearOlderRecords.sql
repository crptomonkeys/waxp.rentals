CREATE PROCEDURE [logs].[ClearOlderRecords]
AS
BEGIN

    DELETE FROM logs.Error WHERE Inserted < DATEADD(day, -1, GETUTCDATE());
    DELETE FROM logs.Message WHERE Inserted < DATEADD(hour, -12, GETUTCDATE());

END
