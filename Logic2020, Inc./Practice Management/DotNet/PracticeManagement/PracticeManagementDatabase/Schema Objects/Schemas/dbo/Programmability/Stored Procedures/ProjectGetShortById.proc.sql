CREATE PROCEDURE [dbo].[ProjectGetShortById]
(
	@ProjectId	         INT
)
AS
BEGIN

	SET NOCOUNT ON

		SELECT p.ProjectId,
			   p.Name,
			   p.StartDate,
			   p.EndDate,
			   p.ProjectStatusId,
			   p.ProjectStatusName,
			   p.ProjectNumber,
			   dbo.GetProjectManagersAliasesList(p.ProjectId) AS ToAddressList
		  FROM dbo.v_Project AS p
		  WHERE p.ProjectId = @ProjectId

END
