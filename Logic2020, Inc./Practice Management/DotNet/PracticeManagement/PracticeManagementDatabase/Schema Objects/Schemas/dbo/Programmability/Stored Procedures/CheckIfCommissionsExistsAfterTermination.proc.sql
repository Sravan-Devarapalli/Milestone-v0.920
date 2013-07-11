CREATE PROCEDURE [dbo].[CheckIfCommissionsExistsAfterTermination]
	(
	@PersonId	INT,
	@TerminationDate	DATETIME
	)
AS
BEGIN
	
	SELECT	DISTINCT P.ProjectNumber,
			P.Name
	FROM	dbo.Attribution A
	INNER JOIN dbo.Project P ON A.ProjectId = P.ProjectId
	WHERE	A.AttributionRecordTypeId = 1 AND A.TargetId = @PersonId AND A.EndDate > @TerminationDate

END
