CREATE FUNCTION [dbo].[GetProjectAccessesEmpNumbers]
(
	@ProjectId INT
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @Temp NVARCHAR(MAX) = ''
  
	SELECT @Temp = @Temp + CASE WHEN PGCP.Timescale IN (3,4) THEN Pers.EmployeeNumber ELSE Pers.PaychexID END +', '
	FROM Project P
	JOIN ProjectAccess PM ON PM.ProjectId = P.ProjectId
	JOIN Person Pers ON Pers.PersonId = PM.ProjectAccessId
	LEFT JOIN dbo.GetCurrentPayTypeTable() PGCP ON PGCP.PersonId = Pers.PersonId
	WHERE P.ProjectId = @ProjectId

	RETURN @Temp
END
