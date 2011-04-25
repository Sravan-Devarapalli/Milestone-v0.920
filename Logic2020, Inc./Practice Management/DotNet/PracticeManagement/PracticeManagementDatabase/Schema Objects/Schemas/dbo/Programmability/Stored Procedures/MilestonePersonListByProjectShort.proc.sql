-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE dbo.MilestonePersonListByProjectShort 
	@ProjectId   VARCHAR(1000)
AS
BEGIN
	SET NOCOUNT ON
	
	SELECT mp.MilestonePersonId,
	       mp.PersonId,
	       mp.FirstName,
	       mp.LastName,
	       mp.ProjectId,
		   mp.SeniorityId
	  FROM dbo.v_MilestonePerson AS mp
	 WHERE mp.ProjectId IN (SELECT * FROM dbo.ConvertStringListIntoTable(@ProjectId))
END

