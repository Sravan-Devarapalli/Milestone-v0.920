-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-21-2008
-- Updated by:	Anton Kramarenko
-- Update date: 02-20-2009
-- Updated by:	Anton Kolesnikov
-- Update date: 07-16-2009
-- Description:	List projects by the specified Client
-- =============================================
CREATE PROCEDURE dbo.ProjectsListByClient
(
	@ClientId INT,
	@Alias VARCHAR(100) = NULL 
)
AS
	SET NOCOUNT ON

	IF @Alias IS NULL   
		SELECT p.ClientId,
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
			   p.ClientIsChargeable,
			   p.ProjectIsChargeable,
			   p.ProjectManagerId,
			   p.ProjectManagerFirstName,
			   p.ProjectManagerLastName,
			   p.DirectorId,
			   p.DirectorFirstName,
			   p.DirectorLastName
		  FROM dbo.v_Project AS p
		 WHERE p.ClientId = @ClientId
		ORDER BY p.Name
	ELSE 
	BEGIN 
		DECLARE @PersonId INT 
		
		SELECT @PersonId = PersonId 
		FROM dbo.Person
		WHERE Alias = @Alias;
	
		WITH Perms AS (
			SELECT TargetId,
				   TargetType
			FROM dbo.Permission 
			WHERE PersonId = @PersonId
		)
		SELECT p.ClientId,
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
			   p.ClientIsChargeable,
			   p.ProjectIsChargeable,
			   p.ProjectManagerId,
			   p.ProjectManagerFirstName,
			   p.ProjectManagerLastName,
			   p.DirectorId,
			   p.DirectorFirstName,
			   p.DirectorLastName
		  FROM dbo.v_Project AS p
		 WHERE 
			p.ClientId = @ClientId AND
			(
				p.GroupId IN (SELECT TargetId FROM Perms AS s WHERE s.TargetType = 2) OR
				p.PracticeId IN (SELECT TargetId FROM Perms AS s WHERE s.TargetType = 5) OR
				p.PracticeManagerId IN (SELECT TargetId FROM Perms AS s WHERE s.TargetType = 4) OR
				EXISTS ( SELECT	1
							  FROM	 dbo.v_PersonProjectCommission AS c
							  WHERE	 c.ProjectId = p.ProjectId
										AND c.PersonId IN (SELECT TargetId FROM Perms AS s WHERE s.TargetType = 3)
										AND c.CommissionType = 1)
			)
		ORDER BY p.NAME
	END 

