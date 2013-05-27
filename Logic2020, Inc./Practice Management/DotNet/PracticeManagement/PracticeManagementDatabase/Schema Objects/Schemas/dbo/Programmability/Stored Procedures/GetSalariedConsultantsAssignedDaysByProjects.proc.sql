-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 02-27-2012
-- Description:	Gets Salaried Consultants Assigned Days By Projects
-- Updated by : Sainath.CH
-- Update Date: 03-30-2012
-- =============================================

CREATE PROCEDURE [dbo].[GetSalariedConsultantsAssignedDaysByProjects]
(
	@StartDate DATETIME  = '20130101',
	@Enddate DATETIME    = '20130601'
)
AS
BEGIN
	DECLARE @FutureDate DATETIME 
	SELECT @FutureDate = dbo.GetFutureDate()

	;WITH CTE AS 
	(
		SELECT	T.Name AS PayType,
				PD.DivisionName AS Division,
				Pers.HireDate,
				Pers.EmployeeNumber,
				Pers.FirstName ,
				Pers.LastName,
				Cli.Name As Account,
				BG.Name AS BusinessGroup,
				PG.Name AS BusinessUnit,
				P.ProjectNumber,
				P.ProjectId,
				C.Date,
				Pers.PersonId,
				Po.LastName + ', '+ Po.FirstName AS ProjectOwner
		FROM dbo.Project P
		INNER JOIN dbo.Milestone M ON M.ProjectId = P.ProjectId
		INNER JOIN dbo.MilestonePerson MP ON MP.MilestoneId = M.MilestoneId
		INNER JOIN dbo.Person Pers ON Pers.PersonId = MP.PersonId AND Pers.IsStrawman = 0 AND Pers.DivisionId =  2-- Consulting
		INNER JOIN dbo.MilestonePersonEntry MPE ON MPE.MilestonePersonId = MP.MilestonePersonId
		INNER JOIN dbo.Calendar C  ON C.Date Between MPE.StartDate AND MPE.EndDate AND C.Date Between @StartDate AND @Enddate
		INNER JOIN dbo.Pay pay ON pay.Person = MP.PersonId AND pay.Timescale = 2 AND C.Date Between pay.StartDate AND ISNULL(pay.EndDate-1,@FutureDate)
		INNER JOIN dbo.Timescale T ON pay.Timescale = T.TimescaleId
		INNER JOIN dbo.PersonDivision pd ON pd.DivisionId = Pers.DivisionId
		INNER JOIN dbo.Client Cli ON Cli.ClientId = P.ClientId
		INNER JOIN dbo.ProjectGroup PG ON PG.GroupId = P.GroupId
		INNER JOIN dbo.BusinessGroup BG ON BG.BusinessGroupId = PG.BusinessGroupId
		Left JOIN dbo.Person PO ON PO.PersonId = P.ProjectOwnerId
		WHERE P.ProjectStatusId NOT IN (1,5) -- not in inactive and experimental
		 AND PerS.PersonStatusId IN(1,5)  AND P.ProjectId !=174 AND MPE.StartDate <= @Enddate AND @StartDate <= MPE.EndDate
		GROUP BY P.ProjectId,P.ProjectNumber,Pers.LastName ,Pers.FirstName ,C.Date,P.Name,Pers.PersonId,T.Name,PD.DivisionName,Pers.HireDate,Pers.EmployeeNumber,Cli.Name,BG.Name,PG.Name,Po.LastName,Po.FirstName
	) 

	SELECT	P.PayType AS [Pay Type],
			P.Division AS [Division],
			CONVERT(DATE,P.HireDate) AS [Hire Date],
			P.EmployeeNumber AS [Person ID],
			P.FirstName AS [First Name] ,
			P.LastName AS [Last Name],
			P.Account,
			P.BusinessGroup AS [Business Group],
			P.BusinessUnit AS [Business Unit],
			P.ProjectNumber AS Project,
			P.ProjectOwner AS [Project Owner],
			CONVERT(DATE,MIN(P.Date))[Roll on Date],
			CONVERT(DATE,MAX(P.Date))[Roll off Date],
			--dbo.GetProjectManagerNames(P.ProjectId) AS ProjectManagers,
			COUNT(*) AS [Days]
	FROM CTE AS P
	GROUP BY P.PayType,
			P.Division,
			P.HireDate,
			P.EmployeeNumber,
			P.FirstName ,
			P.LastName,
			P.Account,
			P.BusinessGroup,
			P.BusinessUnit,
			P.ProjectNumber,
			P.ProjectOwner
	ORDER BY P.LastName, P.FirstName,[Days]
END

GO

