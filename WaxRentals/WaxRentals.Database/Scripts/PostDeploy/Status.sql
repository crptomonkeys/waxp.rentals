CREATE TABLE #Status (StatusId INT, Description VARCHAR(16));
INSERT INTO  #Status VALUES
(1, 'New'      ),
(2, 'Pending'  ),
(3, 'Processed'),
(4, 'Closed'   );

MERGE dbo.Status
USING #Status src
ON    Status.StatusId = src.StatusId
WHEN  NOT MATCHED
      THEN INSERT (    StatusId,     Description)
           VALUES (src.StatusId, src.Description);
