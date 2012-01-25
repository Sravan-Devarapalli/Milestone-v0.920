CREATE FUNCTION [dbo].[IsProjectGroupInUse]
(
	@ProjectGroupId int
)
RETURNS INT
AS
BEGIN

	IF EXISTS(SELECT TOP 1 1 FROM dbo.Project WHERE GroupId = @ProjectGroupId)
	BEGIN
		RETURN 1
	END
	ELSE IF EXISTS(SELECT TOP 1 1 FROM dbo.Opportunity WHERE GroupId = @ProjectGroupId)
	BEGIN
		RETURN 1
	END
	ELSE IF EXISTS(SELECT TOP 1 1 FROM dbo.ChargeCode CC INNER JOIN dbo.TimeEntry TE on TE.ChargeCodeId = CC.Id AND cc.ProjectGroupId = @ProjectGroupId)
	BEGIN
		RETURN 1
	END
	RETURN 0
	
END
