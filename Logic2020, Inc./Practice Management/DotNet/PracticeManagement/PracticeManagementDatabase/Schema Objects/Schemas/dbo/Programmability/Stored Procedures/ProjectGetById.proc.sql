﻿-- =============================================
-- Description:	Get project Details.
-- Updated By: ThulasiRam.P
-- Updated Date: 2012-05-21
-- =============================================
CREATE PROCEDURE [dbo].[ProjectGetById]
(
	@ProjectId	         INT,
	@SalespersonId       INT = NULL,
	@PracticeManagerId   INT = NULL
)
AS
	SET NOCOUNT ON

	SELECT person.LastName+', '+person.FirstName AS PracticeOwnerName,
		   p.ClientId,
		   p.IsMarginColorInfoEnabled,
	       p.ProjectId,
	       p.Discount,
	       p.Terms,
	       p.Name,
	       p.PracticeManagerId,
	       p.PracticeId,
	       p.StartDate,
	       p.EndDate,
	       p.ClientName,
	       p.PracticeName,
	       p.ProjectStatusId,
	       p.ProjectStatusName,
		   p.ProjectNumber,
	       p.BuyerName,
	       p.OpportunityId,
	       p.GroupId,
	       p.ProjectIsChargeable,
	       p.ClientIsChargeable,
		   p.ProjectManagersIdFirstNameLastName,
		   p.DirectorId,
		   p.DirectorLastName,
		   p.DirectorFirstName,
		   pg.Name AS GroupName,
		   p.Description,
		   1 InUse,
		   CASE WHEN A.ProjectId IS NOT NULL THEN 1 
					ELSE 0 END AS HasAttachments,
		   p.CanCreateCustomWorkTypes,
		   p.IsInternal,
		   p.ClientIsInternal,
		   p.ProjectOwnerId,
		   CASE (SELECT COUNT(*) 
				FROM dbo.ChargeCode CC 
				INNER JOIN TimeEntry TE ON TE.ChargeCodeId = CC.Id AND CC.ProjectId = p.ProjectId) 
			WHEN 0 THEN CAST(0 AS BIT)
			ELSE CAST(1 AS BIT) END AS [HasTimeEntries],
			p.IsNoteRequired
	  FROM dbo.v_Project AS p
	  INNER JOIN dbo.ProjectGroup AS pg ON p.GroupId = pg.GroupId
	  INNER JOIN Person AS person ON p.PracticeManagerId = person.PersonId
	  OUTER APPLY (SELECT TOP 1 ProjectId FROM ProjectAttachment as pa WHERE pa.ProjectId = p.ProjectId) A
	  WHERE p.ProjectId = @ProjectId
       AND (   (@SalespersonId IS NULL AND @PracticeManagerId IS NULL)
	        OR EXISTS (SELECT 1
	                      FROM dbo.v_PersonProjectCommission AS c
	                     WHERE c.ProjectId = p.ProjectId AND c.PersonId = @SalespersonId AND c.CommissionType = 1
	                   UNION ALL
	                   SELECT 1
	                     FROM dbo.v_PersonProjectCommission AS c
	                    WHERE c.ProjectId = p.ProjectId AND c.PersonId = @PracticeManagerId AND c.CommissionType = 2 
	                   )
	       )

