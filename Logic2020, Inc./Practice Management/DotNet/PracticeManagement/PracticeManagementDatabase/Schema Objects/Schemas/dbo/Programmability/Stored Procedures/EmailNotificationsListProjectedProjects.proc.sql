



-- =============================================
-- Author:		Artem Dolya
-- Create date: 2010-06-11
-- Description:	Retruns Template with recipients of Projected project
-- =============================================
CREATE PROCEDURE [dbo].[EmailNotificationsListProjectedProjects]
	@EmailTemplateId int		
AS 
    BEGIN
        SET NOCOUNT ON ;
	
	SELECT  distinct
		pr.ProjectId,
		p.Alias as ProjectOwner, 
		sp.Alias as Salesperson, 
		pm.Alias as PracticeOwner, 
		c.Name AS ClientName, 
		pr.Name as ProjectName, 
		pr.StartDate
		
		FROM Project pr 
			INNER JOIN Person p ON p.PersonId = pr.ProjectManagerId
			INNER JOIN Practice pa ON pr.PracticeId = pa.PracticeId
			INNER JOIN Person pm ON pm.PersonId = pa.PracticeManagerId
			INNER JOIN Client c ON c.ClientId = pr.ClientId 
			INNER JOIN Person sp ON sp.PersonId = c.DefaultSalespersonID
			
		WHERE pr.ProjectStatusId = 2 AND pr.StartDate <= GETDATE()
    
		SELECT EmailTemplateSubject, EmailTemplateBody FROM EmailTemplate  et WHERE et.EmailTemplateId = @EmailTemplateId  
    END



