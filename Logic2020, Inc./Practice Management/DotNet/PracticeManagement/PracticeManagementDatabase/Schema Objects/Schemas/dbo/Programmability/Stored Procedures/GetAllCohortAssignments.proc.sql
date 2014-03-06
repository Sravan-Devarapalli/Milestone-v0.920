CREATE PROCEDURE [dbo].[GetAllCohortAssignments]
AS
BEGIN
	
	SELECT	CohortAssignmentId,
			Name AS CohortAssignmentName 
	FROM dbo.CohortAssignment

END
