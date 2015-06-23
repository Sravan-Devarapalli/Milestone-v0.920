CREATE FUNCTION [dbo].[GetProjectManagersAliasesList]
(
	@ProjectId	INT
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	
	DECLARE @Temp NVARCHAR(MAX) = ''
  
	SELECT @Temp = @Temp + Pers.Alias +','
	FROM Project P
	JOIN ProjectAccess PM ON PM.ProjectId = P.ProjectId
	JOIN Person Pers ON Pers.Personid = PM.ProjectAccessId
	WHERE P.ProjectId = @ProjectId

	RETURN @Temp

END
