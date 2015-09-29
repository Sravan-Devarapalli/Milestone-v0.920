CREATE PROCEDURE dbo.MilestonePersonListByProjectShort 
(
	@ProjectId   NVARCHAR(MAX)
)
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT mp.MilestonePersonId,
	       mp.PersonId,
	       mp.FirstName,
		   mp.PreferredFirstName,
	       mp.LastName,
	       mp.ProjectId,
		   mp.SeniorityId
	  FROM dbo.v_MilestonePerson AS mp
	 WHERE mp.ProjectId IN (SELECT * FROM dbo.ConvertStringListIntoTable(@ProjectId))
END

