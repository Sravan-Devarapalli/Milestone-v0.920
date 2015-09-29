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
			 @EndDateLocal				DATETIME,
			 @FutureDateLocal			DATETIME


	SELECT 
	 @CategoryTypeIdLocal			=	@CategoryTypeId,
	 @StartDateLocal				=	@StartDate,
	 @EndDateLocal					=	@EndDate,
	 @FutureDateLocal				=   dbo.GetFutureDate()
	
	IF(@CategoryTypeId = 3) --Business Development managers
	BEGIN
	 
	 ;WITH PersonCTE AS
	 (
	     SELECT P.PersonId,
				P.LastName,
				P.FirstName,
				p.PreferredFirstName,
				P.IsDefaultManager	
		 FROM	dbo.aspnet_Users U 
				INNER JOIN dbo.Person P ON P.Alias = U.UserName 
				INNER JOIN dbo.v_PersonHistory PH ON PH.PersonId = P.PersonId AND
													   PH.HireDate <=  @EndDateLocal AND 
													   ISNULL(PH.TerminationDate,@FutureDateLocal) >= @StartDateLocal
				INNER JOIN dbo.aspnet_UsersInRoles UR ON UR.UserId = u.UserId 
				INNER JOIN dbo.aspnet_UsersRolesHistory URH ON U.UserId = URH.UserId  AND 
															   URH.StartDate <=  @EndDateLocal AND 
															   ISNULL(URH.EndDate,@FutureDateLocal) >= @StartDateLocal
				INNER JOIN dbo.aspnet_Roles R ON URH.RoleId = R.RoleId AND R.RoleName = 'Salesperson'
				INNER JOIN dbo.PersonStatusHistory PSH ON PSH.PersonId = P.PersonId AND 
														 PSH.PersonStatusId IN (1,5)  AND 
														 PSH.StartDate <=  @EndDateLocal AND 
														 ISNULL(PSH.EndDate,@FutureDateLocal) >= @StartDateLocal

	 ),
	 ProjectSalesPerson 
	 AS
	 (
	     SELECT P.PersonId,
				P.LastName,
				P.FirstName,
				p.PreferredFirstName,
				P.IsDefaultManager
		 FROM dbo.Person AS P 
		 INNER JOIN dbo.Project Proj ON P.PersonId = Proj.SalesPersonId AND 
										Proj.StartDate <=  @EndDateLocal AND 
										ISNULL(Proj.EndDate,@FutureDateLocal) >= @StartDateLocal

	 )



	     SELECT P.PersonId,
				P.LastName,
				P.FirstName,
				P.PreferredFirstName,
				P.IsDefaultManager				
		 FROM PersonCTE P
		 UNION 
		  SELECT P.PersonId,
				P.LastName,
				P.FirstName,
				P.PreferredFirstName,
				P.IsDefaultManager				
		 FROM ProjectSalesPerson P	
		 ORDER BY 2,3
	END

END

