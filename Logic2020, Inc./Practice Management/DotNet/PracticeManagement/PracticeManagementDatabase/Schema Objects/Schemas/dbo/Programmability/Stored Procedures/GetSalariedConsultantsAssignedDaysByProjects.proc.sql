﻿-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 02-27-2012
-- Description:	Gets Salaried Consultants Assigned Days By Projects
-- =============================================

CREATE PROCEDURE [dbo].[GetSalariedConsultantsAssignedDaysByProjects]
AS
BEGIN
	WITH CTE AS 
	(
		SELECT	P.ProjectNumber,
				P.Name , 
				Pers.LastName,
				Pers.FirstName ,
				P.ProjectId,
				C.Date,
				Pers.PersonId
		FROM Project P
		JOIN Milestone M ON M.ProjectId = P.ProjectId
		JOIN MilestonePerson MP ON MP.MilestoneId = M.MilestoneId
		JOIN Person Pers ON Pers.PersonId = MP.PersonId AND Pers.IsStrawman = 0
		JOIN MilestonePersonEntry MPE ON MPE.MilestonePersonId = MP.MilestonePersonId
		JOIN Calendar C  ON C.Date Between MPE.StartDate AND MPE.EndDate
		JOIN Pay pay ON pay.Person = MP.PersonId AND pay.Timescale = 2 AND C.Date Between pay.StartDate AND ISNULL(pay.EndDate,dbo.GetFutureDate())

		WHERE P.ProjectStatusId = 3 AND PerS.PersonStatusId =1  AND P.ProjectId !=174 AND P.EndDate >= CAST('20120101' AS DATETIME)
		GROUP BY P.ProjectId,P.ProjectNumber,Pers.LastName ,Pers.FirstName ,C.Date,P.Name,Pers.PersonId 


	) 

	SELECT	P.LastName + ', '+ P.FirstName AS [Consultant Name],
			P.ProjectNumber,
			P.Name AS [Project Name],
			MIN(P.Date)[Roll on Date],
			MAX(P.Date)[Roll off Date],			
			dbo.GetProjectManagerNames(P.ProjectId) AS ProjectManagers,
			COUNT(*) AS [Days]
			
	FROM CTE AS P

	GROUP BY P.ProjectId,P.ProjectNumber,P.LastName, P.FirstName,P.Name,P.PersonId
	ORDER BY P.LastName, P.FirstName,[Days]
END

GO

