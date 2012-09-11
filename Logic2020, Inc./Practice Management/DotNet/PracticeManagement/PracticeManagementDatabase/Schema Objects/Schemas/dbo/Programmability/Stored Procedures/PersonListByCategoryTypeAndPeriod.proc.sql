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
				P.IsDefaultManager	
		 FROM	dbo.aspnet_Users U 
				INNER JOIN dbo.Person P ON P.Alias = U.UserName AND
										   P.HireDate <=  @EndDateLocal AND 
											ISNULL(P.TerminationDate,@FutureDateLocal) >= @StartDateLocal
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
				P.IsDefaultManager
		 FROM dbo.Person AS P 
		 INNER JOIN dbo.Commission Com ON Com.PersonId = P.PersonId AND Com.CommissionType = 1
		 INNER JOIN dbo.Project Proj ON Proj.ProjectId = Com.ProjectId AND 
										Proj.StartDate <=  @EndDateLocal AND 
										ISNULL(Proj.EndDate,@FutureDateLocal) >= @StartDateLocal

	 )



	     SELECT P.PersonId,
				P.LastName,
				P.FirstName,
				P.IsDefaultManager				
		 FROM PersonCTE P
		 UNION 
		  SELECT P.PersonId,
				P.LastName,
				P.FirstName,
				P.IsDefaultManager				
		 FROM ProjectSalesPerson P	
	END

END

