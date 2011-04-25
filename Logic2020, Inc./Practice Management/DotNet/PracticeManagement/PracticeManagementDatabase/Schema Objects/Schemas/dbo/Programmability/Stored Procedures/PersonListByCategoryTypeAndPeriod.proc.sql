CREATE PROCEDURE [dbo].[PersonListByCategoryTypeAndPeriod]
	(
	@CategoryTypeId			INT,
	@StartDate				DATETIME,
	@EndDate				DATETIME
	)
AS
BEGIN
	DECLARE  @CategoryTypeIdLocal		INT,
			 @StartDateLocal			DATETIME,
			 @EndDateLocal				DATETIME


	SELECT 
	 @CategoryTypeIdLocal			=	@CategoryTypeId,
	 @StartDateLocal				=	@StartDate,
	 @EndDateLocal					=	@EndDate
	
	IF(@CategoryTypeId = 3) --Business Development managers
	BEGIN
	 SELECT DISTINCT 
				P.PersonId,
				P.LastName,
				P.FirstName,
				P.IsDefaultManager				
			FROM dbo.Person P
			JOIN dbo.Calendar C ON C.Date BETWEEN P.HireDate AND ISNULL(P.TerminationDate,dbo.GetFutureDate()) 
									AND C.Date  BETWEEN @StartDateLocal AND	 @EndDateLocal
			LEFT JOIN dbo.PersonStatusHistory PSH 
				ON PSH.PersonId = P.PersonId AND PSH.PersonStatusId =1  AND C.Date >= PSH.StartDate AND (C.Date <= PSH.EndDate OR PSH.EndDate IS NULL)
			LEFT JOIN dbo.aspnet_Users U ON P.Alias = U.UserName
			LEFT JOIN dbo.aspnet_UsersRolesHistory  UIR
			ON UIR.UserId = U.UserId  AND C.Date >= UIR.StartDate AND (C.Date <= UIR.EndDate OR UIR.EndDate IS NULL)
			LEFT JOIN dbo.aspnet_Roles UR ON UIR.RoleId = UR.RoleId AND UR.RoleName='Salesperson'
			LEFT JOIN dbo.Commission Com ON Com.PersonId = P.PersonId AND Com.CommissionType = 1
			LEFT JOIN dbo.Project Proj ON Proj.ProjectId = Com.ProjectId AND C.Date BETWEEN Proj.StartDate  AND ISNULL(Proj.EndDate,dbo.GetFutureDate())
			WHERE (UR.RoleId IS NOT NULL OR Com.PersonId IS NOT NULL AND Proj.ProjectId IS NOT NULL)
				  AND (PSH.PersonStatusId IS NOT NULL OR(Com.PersonId IS NOT NULL AND Proj.ProjectId IS NOT NULL))
	END

END

