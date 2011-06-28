
-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 8-04-2008
-- Updated by:	Anatoliy Lokshin
-- Update date: 9-09-2008
-- Description:	Verifies the UserName uniquness for the dbo.Person table.
-- =============================================
CREATE VIEW dbo.v_Person
AS
	SELECT p.PersonId,
	       p.FirstName,
	       p.LastName,
	       p.PTODaysPerAnnum,
	       p.HireDate,
	       p.TerminationDate,
	       p.TelephoneNumber,
	       p.Alias,
	       p.DefaultPractice,
	       r.Name AS PracticeName,
		   p.PersonStatusId,
		   s.Name as PersonStatusName,
		   p.EmployeeNumber,
	       p.SeniorityId,
		   e.SeniorityValue,
	       e.Name AS SeniorityName,
	       p.ManagerId,
		   p.IsDefaultManager,
	       manager.Alias AS 'ManagerAlias',
	       manager.FirstName AS 'ManagerFirstName',
	       manager.LastName AS 'ManagerLastName',
	       -1 AS 'PracticeOwnedId', -- Obsolete, never used
	       '' AS 'PracticeOwnedName', -- Obsolete, never used
		   (SELECT  practice.PracticeId AS '@Id', 
					practice.[Name] AS '@Name' 
			FROM dbo.Practice as practice 			
			WHERE practice.PracticeManagerId = p.PersonId
			FOR XML PATH('Practice'), ROOT('Practices')) AS 'PracticesOwned'
	  FROM dbo.Person AS p
	       LEFT JOIN dbo.Practice AS r ON p.DefaultPractice = r.PracticeId
		   INNER JOIN dbo.PersonStatus AS s ON p.PersonStatusId = s.PersonStatusId
	       LEFT JOIN dbo.Seniority AS e ON p.SeniorityId = e.SeniorityId
	       LEFT JOIN dbo.Person AS manager ON p.ManagerId = manager.PersonId


