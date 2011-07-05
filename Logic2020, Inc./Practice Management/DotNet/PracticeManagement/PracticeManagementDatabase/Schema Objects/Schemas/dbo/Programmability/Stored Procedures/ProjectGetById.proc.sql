-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-21-2008
-- Updated by:	Anton Kramarenko
-- Update date: 02-20-2009
-- Updated by:	Anton Kolesnikov
-- Update date: 07-16-2009
-- Description:	Retrives a Project record
-- =============================================
-- Arguments:
--     @ProjectId - an ID of the project to be retrieved
--     @SalespersonId - determines an ID of the salesperson to filter the list for
--     @PracticeManagerId - determines an ID of the practice manager to filter the list for
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
		   p.ProjectManagerId,
		   p.ProjectManagerFirstName,
		   p.ProjectManagerLastName,
		   p.DirectorId,
		   p.DirectorLastName,
		   p.DirectorFirstName,
		   pg.Name AS GroupName,
		   1 InUse,
		   pa.[FileName],
		   DATALENGTH(PA.AttachmentData) AS AttachmentSize,
		   pa.UploadedDate
	  FROM dbo.v_Project AS p
	  INNER JOIN dbo.ProjectGroup AS pg ON p.GroupId = pg.GroupId
	  INNER JOIN Person AS person ON p.PracticeManagerId = person.PersonId
	  LEFT JOIN dbo.ProjectAttachment AS pa ON p.ProjectId = pa.ProjectId
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


GO

