CREATE PROCEDURE [dbo].[CategoryItemListByCategoryType]
(
	@CategoryTypeId	INT,
	@Year			INT
)
AS
BEGIN
	IF(@CategoryTypeId = 1) -- Client Director
	BEGIN
		SELECT 
				P.PersonId,
				P.LastName,
				P.FirstName,
				dbo.MakeDate(YEAR(C.Date),MONTH(C.Date),1) MonthStartDate,
				CIB.Amount
		FROM dbo.Person P
		JOIN dbo.Calendar C ON C.Date BETWEEN P.HireDate AND ISNULL(P.TerminationDate,dbo.GetFutureDate()) 
								AND YEAR(C.Date) = @Year
		LEFT JOIN dbo.PersonStatusHistory PSH 
			ON PSH.PersonId = P.PersonId AND PSH.PersonStatusId =1  AND C.Date >= PSH.StartDate AND (C.Date <= PSH.EndDate OR PSH.EndDate IS NULL)
		LEFT JOIN dbo.aspnet_Users U ON P.Alias = U.UserName
		LEFT JOIN dbo.aspnet_UsersRolesHistory  UIR
		ON UIR.UserId = U.UserId  AND C.Date >= UIR.StartDate AND (C.Date <= UIR.EndDate OR UIR.EndDate IS NULL)
		LEFT JOIN dbo.aspnet_Roles UR ON UIR.RoleId = UR.RoleId AND UR.RoleName='Client Director'
		LEFT JOIN dbo.Project Proj ON proj.DirectorId = P.PersonId AND C.Date BETWEEN Proj.StartDate 
				AND ISNULL(Proj.EndDate,dbo.GetFutureDate())
		LEFT JOIN dbo.CategoryItemBudget CIB ON CIB.CategoryTypeId = @CategoryTypeId 
						AND YEAR(CIB.MonthStartDate) = @Year AND MONTH(CIB.MonthStartDate) = MONTH(C.Date)
							AND CIB.ItemId = P.PersonId 
		WHERE (UR.RoleId IS NOT NULL OR Proj.ProjectId IS NOT NULL)
			  AND (PSH.PersonStatusId = 1 OR (PSH.PersonId IS NULL AND Proj.ProjectId IS NOT NULL ))
		GROUP BY P.PersonId,
				 P.LastName,
				 P.FirstName,
				 CIB.Amount,
				 YEAR(C.Date),
				 MONTH(C.Date)
		ORDER BY  P.LastName ,
					P.FirstName

	END
	ELSE IF (@CategoryTypeId = 3) --Business Development Manager
	BEGIN
		
		SELECT 
				P.PersonId,
				P.LastName,
				P.FirstName,
				dbo.MakeDate(YEAR(C.Date),MONTH(C.Date),1) MonthStartDate,
				CIB.Amount
		FROM dbo.Person P
		JOIN dbo.Calendar C ON C.Date BETWEEN P.HireDate AND ISNULL(P.TerminationDate,dbo.GetFutureDate()) 
								AND YEAR(C.Date) = @Year
		LEFT JOIN dbo.PersonStatusHistory PSH 
			ON PSH.PersonId = P.PersonId AND PSH.PersonStatusId =1  AND C.Date >= PSH.StartDate AND (C.Date <= PSH.EndDate OR PSH.EndDate IS NULL)
		LEFT JOIN dbo.aspnet_Users U ON P.Alias = U.UserName
		LEFT JOIN dbo.aspnet_UsersRolesHistory  UIR
		ON UIR.UserId = U.UserId  AND C.Date >= UIR.StartDate AND (C.Date <= UIR.EndDate OR UIR.EndDate IS NULL)
		LEFT JOIN dbo.aspnet_Roles UR ON UIR.RoleId = UR.RoleId AND UR.RoleName='Salesperson'
		LEFT JOIN dbo.Commission Com ON Com.PersonId = P.PersonId AND Com.CommissionType = 1
		LEFT JOIN dbo.Project Proj ON Proj.ProjectId = Com.ProjectId AND C.Date BETWEEN Proj.StartDate  AND ISNULL(Proj.EndDate,dbo.GetFutureDate())
		LEFT JOIN dbo.CategoryItemBudget CIB ON CIB.CategoryTypeId = @CategoryTypeId 
						AND YEAR(CIB.MonthStartDate) = @Year AND MONTH(CIB.MonthStartDate) = MONTH(C.Date)
							AND CIB.ItemId = P.PersonId 
		WHERE (UR.RoleId IS NOT NULL OR Com.PersonId IS NOT NULL AND Proj.ProjectId IS NOT NULL)
			  AND (PSH.PersonStatusId IS NOT NULL OR(Com.PersonId IS NOT NULL AND Proj.ProjectId IS NOT NULL))
		GROUP BY P.PersonId,
				 P.LastName,
				 P.FirstName,
				 CIB.Amount,
				 YEAR(C.Date),
				 MONTH(C.Date)
		ORDER BY  P.LastName ,
					P.FirstName

	END
	ELSE -- Practice Area
	BEGIN
			SELECT  P.PracticeId,
					p.Name,
					dbo.MakeDate(YEAR(C.Date),MONTH(C.Date),1) MonthStartDate,
					CIB.Amount
			FROM dbo.Practice P
			JOIN dbo.Calendar C ON @Year = YEAR(C.Date)
			LEFT JOIN dbo.PracticeStatusHistory PSH ON P.PracticeId = PSH.PracticeId AND Psh.IsActive = 1
			LEFT JOIN dbo.Project proj
			ON Proj.PracticeId = P.PracticeId  
					AND C.Date BETWEEN Proj.StartDate  AND ISNULL(Proj.EndDate,dbo.GetFutureDate())
			LEFT JOIN dbo.CategoryItemBudget CIB 
			ON CIB.ItemId = P.PracticeId AND CIB.CategoryTypeId = @CategoryTypeId
				AND YEAR(CIB.MonthStartDate) = @Year AND MONTH(CIB.MonthStartDate) = MONTH(C.Date)
			WHERE PSH.PracticeId IS NOT NULL OR Proj.ProjectId IS NOT NULL
			GROUP BY P.PracticeId,
					 p.Name,
					 CIB.Amount,
					 YEAR(C.Date),
					 MONTH(C.Date)
			ORDER BY  p.Name
	END
END
