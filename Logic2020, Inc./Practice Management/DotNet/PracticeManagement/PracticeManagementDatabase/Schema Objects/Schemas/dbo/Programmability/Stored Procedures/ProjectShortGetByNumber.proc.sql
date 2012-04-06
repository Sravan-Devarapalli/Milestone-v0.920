﻿-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 03-28-2012
-- Updated by : Sainath.CH
-- Update Date: 04-06-2012
-- =============================================
CREATE PROCEDURE dbo.ProjectShortGetByNumber
(
	@ProjectNumber NVARCHAR(12)
)
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE @ProjectBilling NVARCHAR(12)
	SET @ProjectBilling = ''

	  SELECT @ProjectBilling = CASE WHEN MIN(CAST(M.IsHourlyAmount AS INT)) IS NULL OR MIN(CC.TimeEntrySectionId) <> 1 THEN ''
									WHEN (MIN(CAST(M.IsHourlyAmount AS INT)) = MAX(CAST(M.IsHourlyAmount AS INT)) AND MAX(CAST(M.IsHourlyAmount AS INT)) = 0) THEN 'Fixed'
									WHEN (MIN(CAST(M.IsHourlyAmount AS INT)) = MAX(CAST(M.IsHourlyAmount AS INT)) AND MAX(CAST(M.IsHourlyAmount AS INT)) = 1) THEN 'Hourly'
									ELSE 'Both' END 
	  FROM  dbo.Milestone AS M 
	  INNER JOIN dbo.Project AS P ON M.ProjectId = P.ProjectId
	  INNER JOIN dbo.ChargeCode CC ON CC.ProjectId = P.ProjectId 
	  WHERE P.ProjectNumber = @ProjectNumber  

	SELECT P.ProjectId,
		   P.StartDate,
		   P.EndDate,
		   P.Name,
		   C.Name AS ClientName,
		   PG.Name AS GroupName,
		   PS.Name AS ProjectStatusName,
		   @ProjectBilling AS BillingType	      
	  FROM dbo.Project AS P
	  INNER JOIN dbo.Client AS C ON C.ClientId = P.ClientId
	  INNER JOIN dbo.ProjectGroup AS PG ON PG.GroupId = P.GroupId
	  INNER JOIN dbo.ProjectStatus AS PS ON PS.ProjectStatusId = P.ProjectStatusId
	  WHERE P.ProjectNumber = @ProjectNumber     

END
