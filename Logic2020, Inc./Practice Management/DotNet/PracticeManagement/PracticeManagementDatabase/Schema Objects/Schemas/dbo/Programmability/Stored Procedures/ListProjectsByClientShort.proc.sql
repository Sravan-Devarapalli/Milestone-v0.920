CREATE PROCEDURE dbo.ListProjectsByClientShort
(
	@ClientId INT,
	@IsOnlyActiveAndProjective BIT,
	@IsOnlyActiveAndInternal	BIT = 0
)
AS
BEGIN
	SET NOCOUNT ON
	/*
		1	Inactive
		2	Projected
		3	Active
		4	Completed
		5	Experimental
		6	Internal
	*/

	SELECT p.ClientId,
			p.ProjectId,
			p.Name,
			p.ClientName,
			p.ProjectNumber,
			p.BuyerName,
			p.Description
	FROM dbo.v_Project AS p
	WHERE p.ClientId = @ClientId AND p.IsAllowedToShow = 1
		AND ( (@IsOnlyActiveAndProjective = 1 AND @IsOnlyActiveAndInternal = 0 AND p.ProjectStatusId IN (2,3))
			OR (@IsOnlyActiveAndProjective = 0 AND @IsOnlyActiveAndInternal = 1 AND p.ProjectStatusId IN (3,6))
			OR (@IsOnlyActiveAndProjective = 1 AND @IsOnlyActiveAndInternal = 1 AND p.ProjectStatusId IN (2,3,6))
			OR (@IsOnlyActiveAndInternal = 0 AND @IsOnlyActiveAndProjective = 0)
			)
	ORDER BY p.Name
END
	

