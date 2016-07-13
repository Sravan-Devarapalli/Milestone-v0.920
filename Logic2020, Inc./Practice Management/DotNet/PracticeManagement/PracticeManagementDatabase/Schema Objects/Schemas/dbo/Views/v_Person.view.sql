-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 8-04-2008
-- Updated by:	Srinivas.M
-- Update date: 05-21-2012
-- Description:	Verifies the UserName uniquness for the dbo.Person table.
-- =============================================
CREATE VIEW dbo.v_Person
AS

	SELECT p.PersonId,
	       p.FirstName,
	       p.LastName,
		   p.PreferredFirstName,
		   ISNULL(p.PreferredFirstName,p.FirstName) AS First,
		   P.IsOffshore,
		   p.PaychexID,
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
		   p.IsWelcomeEmailSent,
		   p.IsStrawman,
	       manager.Alias AS 'ManagerAlias',
	       manager.FirstName AS 'ManagerFirstName',
		   manager.PreferredFirstName AS 'ManagerPrfFirstName',
	       manager.LastName AS 'ManagerLastName',
		   manager.EmployeeNumber AS 'ManagerEmployeeNumber',
		   manager.PaychexID AS 'ManagerADPID',
		   MGCP.Timescale AS 'ManagerCurrentPayType',
	       -1 AS 'PracticeOwnedId', -- Obsolete, never used
	       '' AS 'PracticeOwnedName', -- Obsolete, never used
		   (SELECT  practice.PracticeId AS '@Id', 
					practice.[Name] AS '@Name' 
			FROM dbo.Practice as practice 			
			WHERE practice.PracticeManagerId = p.PersonId
			FOR XML PATH('Practice'), ROOT('Practices')) AS 'PracticesOwned',
			p.DivisionId,
			d.DivisionName,
			p.TerminationReasonId,
			p.RecruiterId,
			p.TitleId,
			T.Title,
			T.ShowInMeetingReport,
			p.JobSeekerStatusId,
			p.SourceId,
			p.TargetedCompanyId,
			p.EmployeeReferralId,
			empRef.FirstName AS EmployeeReferralFirstName,
			empRef.LastName AS EmployeeReferralLastName,
			p.CohortAssignmentId,
			CA.Name AS CohortAssignmentName,
			p.LocationId,
			L.LocationCode,
			L.LocationName,
			p.IsMBO,
			p.PracticeLeadershipId,
			PLeadrsh.EmployeeNumber as PracticeLeadershipEmployeeNumber,
			PLeadrsh.PaychexID as PracticeLeadershipADPID,
			PGCP.Timescale as PracticeLeadershipCurrentyPayType,
			p.IsInvestmentResource,
			p.TargetUtilization
	  FROM dbo.Person AS p
	       LEFT JOIN dbo.Practice AS r ON p.DefaultPractice = r.PracticeId
		   LEFT JOIN dbo.PersonDivision d ON p.DivisionId=d.DivisionId
		   INNER JOIN dbo.PersonStatus AS s ON p.PersonStatusId = s.PersonStatusId
	       LEFT JOIN dbo.Seniority AS e ON p.SeniorityId = e.SeniorityId
	       LEFT JOIN dbo.Person AS manager ON p.ManagerId = manager.PersonId
		   LEFT JOIN dbo.Title AS T ON p.TitleId = T.TitleId
		   LEFT JOIN dbo.Person AS empRef ON empRef.PersonId = p.EmployeeReferralId
		   LEFT JOIN dbo.CohortAssignment AS CA ON CA.CohortAssignmentId = p.CohortAssignmentId 
		   LEFT JOIN dbo.Location L ON L.LocationId = p.LocationId
		   LEFT JOIN dbo.Person AS PLeadrsh ON PLeadrsh.PersonId = p.PracticeLeadershipId
		   LEFT JOIN dbo.GetCurrentPayTypeTable() MGCP ON MGCP.PersonId = manager.PersonId
		   LEFT JOIN dbo.GetCurrentPayTypeTable() PGCP ON PGCP.PersonId = PLeadrsh.PersonId

