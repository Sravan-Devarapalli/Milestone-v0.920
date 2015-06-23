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
				Ttle.Title,
				Mngr.FirstName+' '+Mngr.LastName AS Manager,
				Cli.Name As Account,
				BG.Name AS BusinessGroup,
				PG.Name AS BusinessUnit,
				P.ProjectNumber,
				P.Name AS ProjectName,
				P.ProjectId,
				P.ProjectStatusId,
				PS.Name AS Status,
				C.Date,
				Pers.PersonId,
				Po.LastName + ', '+ Po.FirstName AS ProjectOwner,
				director.LastName + ', '+ director.FirstName AS ClientDirector
		FROM dbo.Project P
		INNER JOIN dbo.Milestone M ON M.ProjectId = P.ProjectId
		INNER JOIN dbo.MilestonePerson MP ON MP.MilestoneId = M.MilestoneId
		INNER JOIN dbo.Person Pers ON Pers.PersonId = MP.PersonId AND Pers.IsStrawman = 0 AND Pers.DivisionId =  2-- Consulting
		INNER JOIN dbo.MilestonePersonEntry MPE ON MPE.MilestonePersonId = MP.MilestonePersonId
		INNER JOIN dbo.Calendar C  ON C.Date Between MPE.StartDate AND MPE.EndDate AND C.Date Between @StartDate AND @Enddate 
		INNER JOIN dbo.Pay pay ON pay.Person = MP.PersonId AND C.Date Between pay.StartDate AND ISNULL(pay.EndDate-1,@FutureDate)
		INNER JOIN dbo.GetCurrentPayTypeTable() CPT on CPT.PersonId = Pers.PersonId
		INNER JOIN dbo.Timescale T ON CPT.Timescale = T.TimescaleId
		INNER JOIN dbo.PersonDivision pd ON pd.DivisionId = Pers.DivisionId
		INNER JOIN dbo.Client Cli ON Cli.ClientId = P.ClientId
		INNER JOIN dbo.ProjectGroup PG ON PG.GroupId = P.GroupId
		INNER JOIN dbo.BusinessGroup BG ON BG.BusinessGroupId = PG.BusinessGroupId
		INNER JOIN dbo.ProjectStatus PS ON PS.ProjectStatusId = P.ProjectStatusId
		LEFT JOIN dbo.Person PO ON PO.PersonId = P.ProjectManagerId
		LEFT JOIN dbo.Person Mngr ON Mngr.PersonId = Pers.ManagerId
		LEFT JOIN dbo.Person director ON director.PersonId = P.ExecutiveInChargeId
		LEFT JOIN dbo.Title Ttle ON Ttle.TitleId = Pers.TitleId
		WHERE P.ProjectStatusId NOT IN (1,5) -- not in inactive and experimental
		 AND PerS.PersonStatusId IN(1,5)  AND P.ProjectId !=174 AND MPE.StartDate <= @Enddate AND @StartDate <= MPE.EndDate
		GROUP BY P.ProjectId,P.ProjectNumber,P.Name,Pers.LastName ,Ttle.Title,Pers.FirstName ,C.Date,P.Name,Mngr.FirstName,Mngr.LastName,Pers.PersonId,T.Name,PD.DivisionName,Pers.HireDate,Pers.EmployeeNumber,Cli.Name,BG.Name,PG.Name,Po.LastName,Po.FirstName,P.ProjectStatusId,PS.Name,director.FirstName,director.LastName
	) 

	SELECT	P.PayType AS [Pay Type],
			P.Division AS [Division],
			CONVERT(DATE,P.HireDate) AS [Hire Date],
			P.EmployeeNumber AS [Person ID],
			P.FirstName AS [First Name] ,
			P.LastName AS [Last Name],
			P.FirstName +' '+ P.LastName AS Consultant,
			P.Title,
			P.Account,
			P.BusinessGroup AS [Business Group],
			P.BusinessUnit AS [Business Unit],
			P.ProjectNumber AS [Project Number],
			P.Status,
			P.ProjectName AS [Project Name],
			ISNULL(P.ClientDirector,'') AS [Executive in Charge],
			ISNULL(P.ProjectOwner,'') AS [Project Manager],
			dbo.GetProjectManagerNames(P.ProjectId) AS [Project Access],
			P.Manager AS [Career Manager],
			CONVERT(DATE,MIN(P.Date))[Roll on Date],
			CONVERT(DATE,MAX(P.Date))[Roll off Date],
			COUNT(*) AS [Days]
	FROM CTE AS P
	GROUP BY P.PayType,
			P.Division,
			P.HireDate,
			P.EmployeeNumber,
			P.FirstName ,
			P.LastName,
			P.Account,
			P.Title,
			P.Manager,
			P.BusinessGroup,
			P.BusinessUnit,
			P.ProjectNumber,
			P.Status,
			P.ProjectName,
			P.ProjectOwner,
			P.ProjectId,
			P.ClientDirector
	ORDER BY P.FirstName, P.LastName,P.ProjectNumber
END

GO

