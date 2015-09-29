-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 02-27-2012
-- Description:	Gets Project Manager Names By ; separated for a single project
-- =============================================

CREATE FUNCTION [dbo].[GetProjectManagerNames]
(
  @ProjectId INT
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
  
	DECLARE @Temp NVARCHAR(MAX) = ''
  
	SELECT @Temp = @Temp + Pers.LastName + ', ' +ISNULL(Pers.PreferredFirstName,Pers.FirstName) +'; '
	FROM Project P
	JOIN ProjectAccess PM ON PM.ProjectId = P.ProjectId
	JOIN Person Pers ON Pers.Personid = PM.ProjectAccessId
	WHERE P.ProjectId = @ProjectId

	RETURN @Temp
END


GO

