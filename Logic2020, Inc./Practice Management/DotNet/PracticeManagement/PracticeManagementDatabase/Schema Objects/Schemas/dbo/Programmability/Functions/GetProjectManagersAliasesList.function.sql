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
	JOIN ProjectManagers PM ON PM.ProjectId = P.ProjectId
	JOIN Person Pers ON Pers.Personid = PM.ProjectManagerId
	WHERE P.ProjectId = @ProjectId

	RETURN @Temp

END
