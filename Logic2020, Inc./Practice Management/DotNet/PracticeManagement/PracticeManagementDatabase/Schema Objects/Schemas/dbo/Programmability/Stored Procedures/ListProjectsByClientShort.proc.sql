CREATE PROCEDURE dbo.ListProjectsByClientShort
(
	@ClientId INT,
	@IsOnlyActiveAndProjective BIT
)
AS
BEGIN
	SET NOCOUNT ON
	IF (@IsOnlyActiveAndProjective = 1)
	BEGIN
		SELECT p.ClientId,
			   p.ProjectId,
			   p.Name,
			   p.ClientName,
			   p.ProjectNumber,
			   p.BuyerName
		FROM dbo.v_Project AS p
		WHERE p.ClientId = @ClientId 
			   AND p.ProjectStatusId IN (2,3)
		ORDER BY p.Name
	END
	ELSE
	BEGIN
		SELECT p.ClientId,
			   p.ProjectId,
			   p.Name,
			   p.ClientName,
			   p.ProjectNumber,
			   p.BuyerName
		FROM dbo.v_Project AS p
		WHERE p.ClientId = @ClientId 
		ORDER BY p.Name
	END
END
	
