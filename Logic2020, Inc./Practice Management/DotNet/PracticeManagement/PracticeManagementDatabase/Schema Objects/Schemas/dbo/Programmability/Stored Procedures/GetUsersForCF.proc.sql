CREATE PROCEDURE [dbo].[GetUsersForCF]
AS
BEGIN

	DECLARE @Today		DATETIME
	SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETUTCDATE())))

	SELECT P.PersonId,
		   P.EmployeeNumber,
		   P.FirstName,
		   P.PreferredFirstName,
		   P.LastName,
		   P.PaychexID,
		   P.Alias,		   
		   P.locationcode,
		   P.TitleId,
		   title.PositionId,
		   --T.TimescaleId,
		   T.TimescaleCode,
		   P.HireDate,
		   P.ManagerADPID AS ManagerEmployeeNumber,
		   CASE WHEN GCP.Timescale IN (3,4) THEN (SELECT DivisionId FROM dbo.Division_CF WHERE DivisionCode = 'CO3-1099') ELSE CF.CFDivisionId END AS CFDivisionId,
		   CASE WHEN GCP.Timescale IN (3,4) THEN (SELECT DivisionCode FROM dbo.Division_CF WHERE DivisionCode = 'CO3-1099') ELSE DCF.DivisionCode END AS DivisionCode,
		   P.PracticeLeadershipADPID AS PracticeLeadershipEmployeeNumber,
		   p.PracticeLeadershipId,
		   P.IsMBO,
		   Drctr.PaychexID AS PracticeDirector,
		   pd.PracticeDirectorId
	FROM v_Person P
	JOIN dbo.GetCurrentPayTypeTable() GCP ON GCP.PersonId = P.PersonId
	JOIN dbo.Timescale T ON T.TimescaleId = GCP.Timescale
	JOIN dbo.Title title ON title.TitleId = P.TitleId
	LEFT JOIN dbo.CFDivisionMapping CF ON CF.DivisionId = P.DivisionId AND CF.PracticeId = P.DefaultPractice AND (CF.TitleId IS NULL OR CF.TitleId = P.TitleId)
	LEFT JOIN dbo.Division_CF DCF ON DCF.DivisionId = CF.CFDivisionId
	LEFT JOIN dbo.PersonDivision PD ON P.DivisionId = PD.DivisionId
	LEFT JOIN dbo.Person Drctr ON Drctr.PersonId = PD.PracticeDirectorId
	LEFT JOIN dbo.GetCurrentPayTypeTable() DGCP ON DGCP.PersonId = Drctr.PersonId
	WHERE GCP.Timescale IN (1,2,3,4) -- W2-Hourly, W2-Salary
		  AND P.PersonStatusId IN (1,5) -- Active, Termination Pending
		  AND P.IsStrawman = 0
		  AND title.PositionId IS NOT NULL
   ORDER BY P.LastName,P.FirstName

    ;With Temp
	AS
	(
		SELECT P.PersonId,
				pf.ProjectId,
				pf.ReviewPeriodStartDate,
				pf.ReviewPeriodEndDate,
				row_number() over (partition by p.personid order by pf.ReviewPeriodEndDate) as RNo
		FROM dbo.ProjectFeedback PF
		JOIN v_Person P ON P.PersonId = PF.PersonId
		JOIN dbo.Project Pro ON Pro.ProjectId = PF.ProjectId
		JOIN dbo.GetCurrentPayTypeTable() GCP ON GCP.PersonId = P.PersonId
		JOIN dbo.Title title ON title.TitleId = P.TitleId
		LEFT JOIN dbo.PersonFeedbacksInCSFeed PFCF ON PFCF.PersonId = P.PersonId AND PFCF.ProjectId = PF.ProjectId AND PFCF.ReviewStartDate = pf.ReviewPeriodStartDate AND PFCF.ReviewEndDate = pf.ReviewPeriodEndDate 
		WHERE GCP.Timescale IN (1,2) -- W2-Hourly, W2-Salary
				AND P.PersonStatusId IN (1,5) -- Active, Termination Pending
				AND P.IsStrawman = 0
				AND PF.ReviewPeriodEndDate >= '20140701'
				AND CONVERT(NVARCHAR(10), @Today, 111) = CONVERT(NVARCHAR(10), PF.ReviewPeriodEndDate, 111)
				AND Pro.ProjectStatusId IN (3,4)
				AND PFCF.PersonId IS NULL
				AND title.PositionId IS NOT NULL
				AND @Today >= '20151001'
) 
    INSERT INTO dbo.PersonFeedbacksInCSFeed(PersonId,ProjectId,ReviewStartDate,ReviewEndDate,Count)
    SELECT P.PersonId, 
		   P.ProjectId,
		   p.ReviewPeriodStartDate,
		   p.ReviewPeriodEndDate,
		   (SELECT COUNT(1)+p.RNo FROM PersonFeedbacksInCSFeed F WHERE F.PersonId = P.PersonId) 
	FROM Temp P
	WHERE @Today >= '20151001'

    SELECT P.PersonId,
		  pf.ProjectId,
		  Pro.ProjectNumber,
		  Pro.Name as ProjectName,
		  pf.ReviewPeriodStartDate as ReviewStartDate,
		  pf.ReviewPeriodEndDate as ReviewEndDate,
		  PM.PersonId AS ProjectManagerId,
		  EM.PersonId AS EngagementManagerId,
		  EC.PersonId AS ExecutiveInChargeId,
		  CASE WHEN PMGCP.Timescale IN (3,4) THEN PM.EmployeeNumber ELSE PM.PaychexID END AS ProjectManagerUserId,
		  CASE WHEN EMGCP.Timescale IN (3,4) THEN EM.EmployeeNumber ELSE EM.PaychexID END AS EngagementManagerUserId,
		  CASE WHEN ECGCP.Timescale IN (3,4) THEN EC.EmployeeNumber ELSE EC.PaychexID END AS ExecutiveInChargeUserId,
		  F.Count 
   FROM dbo.ProjectFeedback PF
   JOIN v_Person P ON P.PersonId = PF.PersonId
   JOIN dbo.Project Pro ON Pro.ProjectId = PF.ProjectId
   JOIN dbo.GetCurrentPayTypeTable() GCP ON GCP.PersonId = P.PersonId
   JOIN dbo.PersonFeedbacksInCSFeed F ON F.PersonId = P.PersonId
   JOIN dbo.Title title ON title.TitleId = P.TitleId
   LEFT JOIN dbo.Person PM ON PM.PersonId = Pro.ProjectManagerId
   LEFT JOIN dbo.GetCurrentPayTypeTable() PMGCP ON PMGCP.PersonId = PM.PersonId
   LEFT JOIN dbo.Person EM ON EM.PersonId = Pro.EngagementManagerId
   LEFT JOIN dbo.GetCurrentPayTypeTable() EMGCP ON EMGCP.PersonId = EM.PersonId
   LEFT JOIN dbo.Person EC ON EC.PersonId = Pro.ExecutiveInChargeId
   LEFT JOIN dbo.GetCurrentPayTypeTable() ECGCP ON ECGCP.PersonId = EC.PersonId
   WHERE GCP.Timescale IN (1,2) -- W2-Hourly, W2-Salary
		  AND P.PersonStatusId IN (1,5) -- Active, Termination Pending
		  AND P.IsStrawman = 0
		  AND PF.ReviewPeriodEndDate >= '20140701'
		  AND CONVERT(NVARCHAR(10), @Today, 111) = CONVERT(NVARCHAR(10), PF.ReviewPeriodEndDate, 111)
		  AND Pro.ProjectStatusId IN (3,4)
		  AND F.ProjectId = PF.ProjectId AND F.ReviewEndDate = PF.ReviewPeriodEndDate AND F.ReviewStartDate = PF.ReviewPeriodStartDate
		  AND F.Count <= 8
		  AND title.PositionId IS NOT NULL
		  AND @Today >= '20151001'

    SELECT	F.PersonId,
			Pr.ProjectId,
			Pr.ProjectNumber,
			Pr.Name AS ProjectName,
			F.ReviewStartDate,
			F.ReviewEndDate,
			manager.LastName+', '+manager.FirstName AS ProjectManager
	FROM dbo.PersonFeedbacksInCSFeed F
	JOIN dbo.Person P ON P.PersonId = F.PersonId
	JOIN dbo.Project Pr ON Pr.ProjectId = F.ProjectId
	JOIN dbo.Person manager ON manager.PersonId = Pr.ProjectManagerId
	JOIN dbo.GetCurrentPayTypeTable() GCP ON GCP.PersonId = F.PersonId
	JOIN dbo.Title title ON title.TitleId = P.TitleId
	WHERE F.Count > 8 AND
	      CONVERT(NVARCHAR(10), @Today, 111) = CONVERT(NVARCHAR(10), F.ReviewEndDate, 111) AND 
		  GCP.Timescale IN (1,2) -- W2-Hourly, W2-Salary	
		  AND title.PositionId IS NOT NULL 
		  AND @Today >= '20151001'

END

