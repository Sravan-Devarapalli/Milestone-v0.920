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
  
SELECT @Temp = @Temp + Pers.LastName + ', ' +Pers.FirstName +'; '
FROM Project P
JOIN ProjectManagers PM ON PM.ProjectId = P.ProjectId
JOIN Person Pers ON Pers.Personid = PM.ProjectManagerId
WHERE P.ProjectId = @ProjectId
RETURN @Temp
END


GO

