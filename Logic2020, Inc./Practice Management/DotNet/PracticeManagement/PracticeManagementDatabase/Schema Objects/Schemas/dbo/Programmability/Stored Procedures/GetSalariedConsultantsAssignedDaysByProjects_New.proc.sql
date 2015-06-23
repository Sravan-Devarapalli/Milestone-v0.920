CREATE PROCEDURE [dbo].[GetSalariedConsultantsAssignedDaysByProjects_New]
(
	@StartDate	DATETIME,
	@Enddate	DATETIME
)
AS
BEGIN
		SET NOCOUNT ON;

		DECLARE @StartDateLocal DATETIME ,
			@EndDateLocal DATETIME ,
			@ORTTimeTypeId INT ,
			@HolidayTimeType INT ,
			@UnpaidTimeTypeId	INT,
			@FutureDate DATETIME

		SELECT @StartDateLocal = CONVERT(DATE, @StartDate), @EndDateLocal = CONVERT(DATE, @EndDate),@ORTTimeTypeId = dbo.GetORTTimeTypeId(),@HolidayTimeType = dbo.GetHolidayTimeTypeId(),@FutureDate = dbo.GetFutureDate(),@UnpaidTimeTypeId = dbo.GetUnpaidTimeTypeId()

		;WITH    
		 CTE AS 
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
				PG.Code AS BusinessUnitCode,
				PG.Name AS BusinessUnit,
				P.ProjectNumber,
				practice.Name AS [Practice Area],
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
		INNER JOIN dbo.Person Pers ON Pers.PersonId = MP.PersonId AND Pers.IsStrawman = 0 AND Pers.DivisionId in (2,5,6)-- Consulting
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
		LEFT JOIN dbo.Practice practice ON practice.PracticeId = P.PracticeId
		WHERE P.ProjectStatusId NOT IN (1,5) -- not in inactive and experimental
		 AND PerS.PersonStatusId IN(1,5)  AND P.ProjectId !=174 AND MPE.StartDate <= @Enddate AND @StartDate <= MPE.EndDate
		GROUP BY Pers.PersonId,practice.Name,P.ProjectId,PG.Code,P.ProjectNumber,P.Name,Pers.LastName ,Ttle.Title,Pers.FirstName ,C.Date,P.Name,Mngr.FirstName,Mngr.LastName,Pers.PersonId,T.Name,PD.DivisionName,Pers.HireDate,Pers.EmployeeNumber,Cli.Name,BG.Name,PG.Name,Po.LastName,Po.FirstName,P.ProjectStatusId,PS.Name,director.FirstName,director.LastName
	) ,
		
		ProjectedCTE 
		AS
		(SELECT	P.PayType AS [Pay Type],
		   P.PersonId,
			P.Division AS [Division],
			CONVERT(DATE,P.HireDate) AS [Hire Date],
			P.EmployeeNumber,
			P.FirstName AS [First Name] ,
			P.LastName AS [Last Name],
			P.FirstName +' '+ P.LastName AS Consultant,
			P.Title,
			P.Account,
			P.BusinessGroup AS [Business Group],
			P.BusinessUnitCode,
			P.BusinessUnit AS [Business Unit],
			P.ProjectNumber AS [Project Number],
			P.Status,
			P.[Practice Area],
			P.ProjectName AS [Project Name],
			ISNULL(P.ClientDirector,'') AS [Client Director],
			ISNULL(P.ProjectOwner,'') AS [Project Owner],
			dbo.GetProjectManagerNames(P.ProjectId) AS [Project Access],
			P.Manager AS [Career Manager],
			CONVERT(DATE,MIN(P.Date))[Roll on Date],
			CONVERT(DATE,MAX(P.Date))[Roll off Date],
			COUNT(*) AS [Days]
	FROM CTE AS P
	GROUP BY P.PayType,
			P.PersonId,
			P.Division,
			P.HireDate,
			P.EmployeeNumber,
			P.FirstName ,
			P.LastName,
			P.Account,
			P.Title,
			P.[Practice Area],
			P.Manager,
			P.BusinessGroup,
			P.BusinessUnit,
			P.BusinessUnitCode,
			P.ProjectNumber,
			P.Status,
			P.ProjectName,
			P.ProjectOwner,
			P.ProjectId,
			P.ClientDirector
			),

		PersonDayWiseByProjectsBillableTypes
				  AS ( SELECT   M.ProjectId ,
								MP.PersonId,
								C.Date ,
								MIN(CAST(M.IsHourlyAmount AS INT)) MinimumValue ,
								MAX(CAST(M.IsHourlyAmount AS INT)) MaximumValue
								,CASE WHEN MAX(CAST(m.IsHOurlyAmount AS INT)) = 1 AND MIN(CAST(m.IsHourlyAmount AS INT)) = 1 THEN SUM(MPE.Amount * MPE.HoursPerDay)/ SUM(MPE.HoursPerDay)
								ELSE NULL END HourlyRate
					   FROM     dbo.MilestonePersonEntry AS MPE
								INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
								INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
								INNER JOIN dbo.Calendar AS C ON C.Date BETWEEN MPE.StartDate AND MPE.EndDate
															  AND C.Date BETWEEN @StartDateLocal AND @EndDateLocal
					   WHERE    M.StartDate < @EndDateLocal
								AND @StartDateLocal < M.ProjectedDeliveryDate
					   GROUP BY M.ProjectId ,
								MP.PersonId,
								C.Date
					 ),
		TimeEntryCTE
		AS
			(SELECT  TE.PersonId,
					P.LastName+', '+P.FirstName AS Employee,
					P.PracticeName AS PracticeGroup,
					Pa.TimescaleName AS [Pay Type],
					CASE WHEN P.IsOffshore = 1 THEN 'YES' ELSE 'NO' END AS IsOffshore,
					C.Name AS [Account Name],
					BU.Code AS [Business Unit] ,
					BU.Name AS [Business Unit Name]  ,
					BG.Name AS BusinessGroup,
					PRO.ProjectNumber AS Project,
					PRO.Name AS ProjectName ,
					PS.Name AS Status,
					practice.Name AS [Practice Area],
					CC.PhaseId AS Phase,
					TT.Code AS [Work Type] ,
					TT.Name AS [Work Type Name] ,
					TE.ChargeCodeDate AS Date,
					ROUND(SUM(CASE WHEN TEH.IsChargeable = 1
								   THEN TEH.ActualHours
								   ELSE 0
							  END), 2) AS [Billable Hours] ,
					ROUND(SUM(CASE WHEN TEH.IsChargeable = 0
								   THEN TEH.ActualHours
								   ELSE 0
							  END), 2) AS [Non-billable Hours],
					ROUND(SUM(TEH.ActualHours),2) AS [Total Hours],
					CASE WHEN CC.TimeEntrySectionId = 3 THEN NULL
							ELSE PDBR.HourlyRate END [Bill Rate],
					Pa.AmountHourly AS 'Pay Rate',
					ROUND( ROUND(SUM(CASE WHEN TEH.IsChargeable = 1
								   THEN TEH.ActualHours
								   ELSE 0
							  END), 2)*Pa.AmountHourly, 0) AS 'Billable Cost',
					TE.Note
			FROM    dbo.TimeEntry AS TE
					INNER JOIN dbo.TimeEntryHours AS TEH ON TEH.TimeEntryId = TE.TimeEntryId
					INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId
					INNER JOIN dbo.ProjectGroup BU ON BU.GroupId = CC.ProjectGroupId
					INNER JOIN dbo.BusinessGroup BG ON BG.BusinessGroupId = BU.BusinessGroupId
					INNER JOIN dbo.Client C ON CC.ClientId = C.ClientId
					INNER JOIN dbo.Project PRO ON PRO.ProjectId = CC.ProjectId
					INNER JOIN dbo.ProjectStatus PS ON PRO.ProjectStatusId = PS.ProjectStatusId
					INNER JOIN dbo.TimeType TT ON TT.TimeTypeId = CC.TimeTypeId
					INNER JOIN dbo.PersonStatusHistory PTSH ON TE.ChargeCodeDate BETWEEN PTSH.StartDate
															  AND ISNULL(PTSH.EndDate,@FutureDate) AND PTSH.PersonId = TE.PersonId
					INNER JOIN v_Person P ON P.PersonId = TE.PersonId
					LEFT JOIN dbo.Practice practice ON practice.PracticeId = PRO.PracticeId
					LEFT JOIN PersonDayWiseByProjectsBillableTypes PDBR ON PDBR.ProjectId = CC.ProjectId AND TE.PersonId = PDBR.PersonId
															  AND PDBR.Date = TE.ChargeCodeDate
					LEFT JOIN dbo.v_Pay Pa ON Pa.PersonId = P.PersonId AND TE.ChargeCodeDate BETWEEN Pa.StartDate AND (ISNULL(Pa.EndDate, @FutureDate) -1)
			WHERE   P.IsStrawman = 0 
					AND P.DivisionId in (2,5,6)-- Consulting
					AND TE.ChargeCodeDate BETWEEN @StartDateLocal
										  AND     @EndDateLocal
					AND PRO.ProjectId !=174 
					AND PRO.ProjectStatusId NOT IN (1,5)
					AND ( CC.timeTypeId != @HolidayTimeType
						  OR ( CC.timeTypeId = @HolidayTimeType
							   AND PTSH.PersonStatusId IN (1,5)
							 )
						)
			GROUP BY TE.PersonId ,
					P.EmployeeNumber,
					P.FirstName,
					P.LastName,
					P.IsOffshore,
					BG.Name,
					CC.TimeEntrySectionId ,
					C.ClientId ,
					C.Name ,
					C.Code ,
					BU.Name ,
					BU.Code ,
					PRO.ProjectId ,
					PRO.Name ,
					PRO.ProjectNumber ,
					PS.Name ,
					TT.TimeTypeId ,
					TT.Name ,
					TT.Code ,
					TE.ChargeCodeDate ,
					TE.Note ,
					PDBR.MinimumValue ,
					PDBR.MaximumValue,
					PDBR.HourlyRate,
					Pa.AmountHourly,
					Pa.TimescaleName,
					P.PracticeName,
					practice.Name,
					CC.PhaseId
)

SELECT  
 isnull(T.Employee,P.[Last Name]+', '+P.[First Name]) AS Employee,
	    ISNULL(T.[Pay Type],P.[Pay Type]) AS [Pay Type],
		PD.DivisionName AS Division,
		Pers.PracticeName AS PracticeGroup,
		CASE WHEN Pers.IsOffshore = 1 THEN 'YES' ELSE 'NO' END AS IsOffshore,
		Pers.HireDate,
		Pers.EmployeeNumber,
		Ttle.Title,
		Mngr.FirstName+' '+Mngr.LastName AS Manager,
		ISNULL(P.[Project Number],T.Project) AS Project,
		ISNULL(P.[Project Name],T.ProjectName) AS ProjectName,
		ISNULL(P.[Practice Area],T.[Practice Area]) AS [Practice Area],
		ISNULL(P.Status,T.Status) AS ProjectStatus,
		ISNULL(P.Account,T.[Account Name]) AS [Account Name],
		ISNULL(P.BusinessUnitCode,T.[Business Unit]) AS [Business Unit],
		ISNULL(P.[Business Unit],T.[Business Unit Name]) AS [Business Unit Name], 
		ISNULL(P.[Business Group],T.BusinessGroup) AS [Business Group],
		Po.LastName + ', '+ Po.FirstName AS [Project Manager],
		director.LastName + ', '+ director.FirstName AS [Executive in Charge],
		dbo.GetProjectManagerNames(PRO.ProjectId) AS [Project Managers],
		P.[Roll on Date],
		P.[Roll off Date],
		P.[Days],
		T.Phase,
		T.[Work Type],
		T.[Work Type Name],
		T.Date AS ChargeCodeDate,
		T.[Billable Hours],
		T.[Non-billable Hours],
		T.[Total Hours],
		T.[Bill Rate],
		T.[Pay Rate],
		T.[Billable Cost],
		T.Note
FROM ProjectedCTE P
FULL JOIN TimeEntryCTE T ON T.PersonId = P.PersonId AND T.Project = P.[Project Number]
INNER JOIN dbo.Project PRO ON PRO.ProjectNumber = ISNULL(T.Project , P.[Project Number])
INNER JOIN  v_Person Pers ON Pers.PersonId = ISNULL(P.PersonId,T.PersonId)
INNER JOIN dbo.PersonDivision pd ON pd.DivisionId = Pers.DivisionId
LEFT JOIN dbo.Person PO ON PO.PersonId = PRO.ProjectManagerId
LEFT JOIN dbo.Title Ttle ON Ttle.TitleId = Pers.TitleId
LEFT JOIN dbo.Person director ON director.PersonId = PRO.ExecutiveInChargeId
LEFT JOIN dbo.Person Mngr ON Mngr.PersonId = Pers.ManagerId
ORDER BY ISNULL(T.Employee,P.[Last Name]+', '+P.[First Name]), T.Date
END
