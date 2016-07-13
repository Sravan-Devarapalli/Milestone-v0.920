CREATE PROCEDURE [dbo].[ProjectListByVendor]
(
	@VendorId INT
)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @VendorName NVARCHAR(100)

	SELECT @VendorName= Name FROM dbo.Vendor WHERE Id=@VendorId

	SELECT	   p.ProjectId,
			   p.Name,
			   p.ProjectNumber,
			   p.ClientId,
			   p.ClientName,
			   p.DivisionId,
			   p.DivisionName,
			   p.PracticeId,
			   p.PracticeName,
			   p.StartDate,
			   p.EndDate,
			   p.ProjectStatusId,
			   p.ProjectStatusName,
			   p.GroupId,
			   pg.Name AS GroupName,
			   p.ProjectIsChargeable
			     
		  FROM dbo.v_Project AS p
		  INNER JOIN dbo.ProjectGroup AS pg ON p.GroupId = pg.GroupId
		  LEFT JOIN dbo.Channel as C ON C.ChannelId=p.ChannelId 
		  WHERE  p.IsAllowedToShow = 1 AND C.IsSubChannelVendor=1 AND p.SubChannel=@VendorName

END
