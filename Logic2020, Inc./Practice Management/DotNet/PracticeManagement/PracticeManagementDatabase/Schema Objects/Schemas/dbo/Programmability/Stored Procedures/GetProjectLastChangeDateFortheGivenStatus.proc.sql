CREATE PROCEDURE [dbo].GetProjectLastChangeDateFortheGivenStatus
(
 @ProjectId      INT,
 @ProjectStatusId INT
)
AS
BEGIN
   SELECT MAX(StartDate) AS StartDate
   FROM dbo.ProjectStatusHistory 
   WHERE ProjectId=@ProjectId AND ProjectStatusId=@ProjectStatusId 
END
