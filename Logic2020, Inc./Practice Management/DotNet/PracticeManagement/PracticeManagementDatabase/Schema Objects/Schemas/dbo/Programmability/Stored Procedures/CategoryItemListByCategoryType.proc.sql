CREATE PROCEDURE [dbo].[CategoryItemListByCategoryType]
(
	@CategoryTypeId	INT,
	@Year			INT
)
AS
BEGIN
	DECLARE @FutureDate DATETIME
	SELECT @FutureDate = dbo.GetFutureDate()

	IF(@CategoryTypeId = 1) -- Client Director
	BEGIN
		SELECT  P.PersonId,
				P.LastName,
				ISNULL(P.PreferredFirstName,P.FirstName) AS FirstName,
				C.MonthStartDate MonthStartDate,
				CIB.Amount
		FROM dbo.Person P
		INNER JOIN dbo.v_PersonHistory PH ON PH.PersonId = P.PersonId
		INNER JOIN dbo.Calendar C ON YEAR(C.Date) = @Year and c.Date = c.MonthStartDate AND (C.Date BETWEEN PH.HireDate AND ISNULL(PH.TerminationDate, @FutureDate) or Year(C.Date) BETWEEN Year(PH.HireDate) AND ISNULL(Year(PH.TerminationDate), Year(@FutureDate))
		AND Month(C.Date) BETWEEN Month(PH.HireDate) AND ISNULL(Month(PH.TerminationDate), Month(@FutureDate)) ) 
		LEFT JOIN dbo.PersonStatusHistory PSH 
			ON PSH.PersonId = P.PersonId AND PSH.PersonStatusId IN (1,5)  AND (C.Date BETWEEN PSH.StartDate and ISNULL(PSH.EndDate, @FutureDate) or Year(C.Date) BETWEEN Year(PSH.StartDate) AND ISNULL(Year(PSH.EndDate), Year(@FutureDate))
		AND Month(C.Date) BETWEEN Month(PSH.StartDate) AND ISNULL(Month(PSH.EndDate), Month(@FutureDate)) )
		LEFT JOIN dbo.aspnet_Users U ON P.Alias = U.UserName
		LEFT JOIN dbo.aspnet_UsersRolesHistory  UIR
		ON UIR.UserId = U.UserId  AND (C.Date between UIR.StartDate and ISNULL(uir.EndDate, @FutureDate) or Year(C.Date) BETWEEN Year(UIR.StartDate) AND ISNULL(Year(UIR.EndDate), Year(@FutureDate))
		AND Month(C.Date) BETWEEN Month(UIR.StartDate) AND ISNULL(Month(UIR.EndDate), Month(@FutureDate)))
		LEFT JOIN dbo.aspnet_Roles UR ON UIR.RoleId = UR.RoleId AND UR.RoleName='Client Director'
		LEFT JOIN dbo.Project Proj ON proj.ExecutiveInChargeId = P.PersonId AND (C.Date BETWEEN Proj.StartDate 
				AND ISNULL(Proj.EndDate, @FutureDate) or Year(C.Date) BETWEEN Year(Proj.StartDate) AND ISNULL(Year(Proj.EndDate), Year(@FutureDate))
		AND Month(C.Date) BETWEEN Month(Proj.StartDate) AND ISNULL(Month(Proj.EndDate), Month(@FutureDate)))
		LEFT JOIN dbo.CategoryItemBudget CIB ON CIB.CategoryTypeId = @CategoryTypeId 
						AND YEAR(CIB.MonthStartDate) = @Year AND MONTH(CIB.MonthStartDate) = MONTH(C.Date)
							AND CIB.ItemId = P.PersonId 
		WHERE (UR.RoleId IS NOT NULL OR Proj.ProjectId IS NOT NULL)
			  AND (PSH.PersonStatusId = 1 OR (PSH.PersonId IS NULL AND Proj.ProjectId IS NOT NULL ))
		GROUP BY P.PersonId,
				 P.LastName,
				 ISNULL(P.PreferredFirstName,P.FirstName),
				 CIB.Amount,
				 C.MonthStartDate
		ORDER BY  P.LastName ,
					ISNULL(P.PreferredFirstName,P.FirstName),
					c.MonthStartDate

	END
	ELSE IF (@CategoryTypeId = 3) --Business Development Manager
	BEGIN
		
		SELECT 
				P.PersonId,
				P.LastName,
				ISNULL(P.PreferredFirstName,P.FirstName) AS FirstName,
				C.MonthStartDate MonthStartDate,
				CIB.Amount
		FROM dbo.Person P
		INNER JOIN dbo.v_PersonHistory PH ON PH.PersonId = P.PersonId
		INNER JOIN dbo.Calendar C ON YEAR(C.Date) = @Year and c.Date = c.MonthStartDate AND (C.Date BETWEEN PH.HireDate AND ISNULL(PH.TerminationDate, @FutureDate) or Year(C.Date) BETWEEN Year(PH.HireDate) AND ISNULL(Year(PH.TerminationDate), Year(@FutureDate))
		AND Month(C.Date) BETWEEN Month(PH.HireDate) AND ISNULL(Month(PH.TerminationDate), Month(@FutureDate)) ) 
		LEFT JOIN dbo.PersonStatusHistory PSH 
			ON PSH.PersonId = P.PersonId AND PSH.PersonStatusId IN (1,5)  AND (C.Date BETWEEN PSH.StartDate and ISNULL(PSH.EndDate, @FutureDate) or Year(C.Date) BETWEEN Year(PSH.StartDate) AND ISNULL(Year(PSH.EndDate), Year(@FutureDate))
		AND Month(C.Date) BETWEEN Month(PSH.StartDate) AND ISNULL(Month(PSH.EndDate), Month(@FutureDate)) )
		LEFT JOIN dbo.aspnet_Users U ON P.Alias = U.UserName
		LEFT JOIN dbo.aspnet_UsersRolesHistory  UIR
		ON UIR.UserId = U.UserId AND (C.Date between UIR.StartDate and ISNULL(uir.EndDate, @FutureDate) or Year(C.Date) BETWEEN Year(UIR.StartDate) AND ISNULL(Year(UIR.EndDate), Year(@FutureDate))
		AND Month(C.Date) BETWEEN Month(UIR.StartDate) AND ISNULL(Month(UIR.EndDate), Month(@FutureDate)))
		LEFT JOIN dbo.aspnet_Roles UR ON UIR.RoleId = UR.RoleId AND UR.RoleName='Salesperson'
		LEFT JOIN dbo.Project Proj ON (C.Date BETWEEN Proj.StartDate 
				AND ISNULL(Proj.EndDate, @FutureDate) or Year(C.Date) BETWEEN Year(Proj.StartDate) AND ISNULL(Year(Proj.EndDate), Year(@FutureDate))
		AND Month(C.Date) BETWEEN Month(Proj.StartDate) AND ISNULL(Month(Proj.EndDate), Month(@FutureDate)))
		LEFT JOIN dbo.CategoryItemBudget CIB ON CIB.CategoryTypeId = @CategoryTypeId 
						AND YEAR(CIB.MonthStartDate) = @Year AND MONTH(CIB.MonthStartDate) = MONTH(C.Date)
							AND CIB.ItemId = P.PersonId 
		WHERE (UR.RoleId IS NOT NULL OR Proj.SalesPersonId IS NOT NULL AND Proj.ProjectId IS NOT NULL)
			  AND (PSH.PersonStatusId IS NOT NULL OR(Proj.SalesPersonId IS NOT NULL AND Proj.ProjectId IS NOT NULL))
		GROUP BY P.PersonId,
				 P.LastName,
				 ISNULL(P.PreferredFirstName,P.FirstName),
				 CIB.Amount,
				 C.MonthStartDate
		ORDER BY  P.LastName ,
					ISNULL(P.PreferredFirstName,P.FirstName),
					c.MonthStartDate

	END
	ELSE -- Practice Area
	BEGIN
			SELECT  P.PracticeId,
					p.Name,
					C.MonthStartDate MonthStartDate,
					CIB.Amount
			FROM dbo.Practice P
			JOIN dbo.Calendar C ON @Year = YEAR(C.Date)
			LEFT JOIN dbo.PracticeStatusHistory PSH ON P.PracticeId = PSH.PracticeId AND Psh.IsActive = 1
			LEFT JOIN dbo.Project proj
			ON Proj.PracticeId = P.PracticeId  
					AND C.Date BETWEEN Proj.StartDate  AND ISNULL(Proj.EndDate, @FutureDate)
			LEFT JOIN dbo.CategoryItemBudget CIB 
			ON CIB.ItemId = P.PracticeId AND CIB.CategoryTypeId = @CategoryTypeId
				AND YEAR(CIB.MonthStartDate) = @Year AND MONTH(CIB.MonthStartDate) = MONTH(C.Date)
			WHERE PSH.PracticeId IS NOT NULL OR Proj.ProjectId IS NOT NULL
			GROUP BY P.PracticeId,
					 p.Name,
					 CIB.Amount,
					 C.MonthStartDate
			ORDER BY  p.Name,
			c.MonthStartDate
	END
END

