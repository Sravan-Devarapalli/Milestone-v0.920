CREATE FUNCTION [dbo].[IsProjectGroupInUse]
(
	@ProjectGroupId int
)
RETURNS INT
AS
BEGIN

	IF EXISTS(SELECT TOP 1 1 FROM Project WHERE GroupId = @ProjectGroupId)
	BEGIN
		RETURN 1
	END
	ELSE IF EXISTS(SELECT TOP 1 1 FROM Opportunity WHERE GroupId = @ProjectGroupId)
	BEGIN
		RETURN 1
	END
	ELSE IF EXISTS(SELECT TOP 1 1 FROM dbo.ChargeCode cc INNER JOIN dbo.TimeTrack tt on tt.ChargeCodeId = cc.Id AND cc.ProjectGroupId = @ProjectGroupId)
	BEGIN
		RETURN 1
	END
	RETURN 0
	
END
