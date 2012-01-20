CREATE PROCEDURE dbo.DeleteTimeTrack
	@ClientId INT,
	@ProjectId INT,
	@TimeTypeId INT,
	@StartDate DATETIME,
	@EndDate DATETIME,
	@personId INT
AS
BEGIN

	DELETE TT 
	FROM dbo.TimeTrack TT INNER JOIN dbo.ChargeCode cc
		ON TT.ChargeCodeId = cc.Id AND cc.ClientId = @ClientId AND cc.ProjectId =  @ProjectId AND cc.TimeTypeId = @TimeTypeId 
	WHERE TT.ChargeCodeDate BETWEEN @StartDate AND @EndDate AND TT.PersonId = @personId
END
