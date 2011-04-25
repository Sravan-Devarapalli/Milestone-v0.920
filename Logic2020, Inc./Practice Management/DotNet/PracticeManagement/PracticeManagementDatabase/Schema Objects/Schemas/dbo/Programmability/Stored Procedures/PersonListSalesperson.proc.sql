CREATE PROCEDURE dbo.PersonListSalesperson
    (
      @IncludeInactive BIT ,
      @PersonId INT = NULL
    )
AS 
    SET NOCOUNT ON

    DECLARE @Now DATETIME
    SET @Now = dbo.Today()

	-- Table variable to store list of salespersons user is allowed to see	
    DECLARE @SalespersonPermissions TABLE ( PersonId INT NULL )
	-- Populate is with the data from the permissions table
	--		TargetType = 3 means that we are looking for the salespersons in the permissions table
    INSERT  INTO @SalespersonPermissions
            ( PersonId
	      )
            SELECT  prm.TargetId
            FROM    v_permissions AS prm
            WHERE   prm.PersonId = @PersonId
                    AND prm.PermissionTypeId = 3
	
	-- If there are nulls in permission table, it means that eveything is allowed to be seen,
	--		so set @PersonId to NULL which will extract all records from the table
    DECLARE @NullsNumber INT
    SELECT  @NullsNumber = COUNT(*)
    FROM    @SalespersonPermissions AS sp
    WHERE   sp.PersonId IS NULL 

    IF @NullsNumber > 0 
        SET @PersonId = NULL

        ;WITH    Salespersons
                  AS ( SELECT   p.PersonId ,
                                p.FirstName ,
                                p.LastName ,
                                p.PTODaysPerAnnum ,
                                p.HireDate ,
                                p.TerminationDate ,
                                p.Alias ,
                                p.DefaultPractice ,
                                p.PracticeName ,
                                p.PersonStatusId ,
                                p.PersonStatusName ,
                                p.EmployeeNumber ,
                                p.SeniorityId ,
                                p.SeniorityName ,
                                p.ManagerId ,
                                p.ManagerAlias ,
                                p.ManagerFirstName ,
                                p.ManagerLastName ,
                                p.PracticeOwnedId ,
                                p.PracticeOwnedName,
                                p.TelephoneNumber
                       FROM     dbo.v_Person AS p
                       WHERE    ( p.PersonStatusId = 1 /* Active person only */
                                  OR @IncludeInactive = 1
                                )
                                AND ( @PersonId IS NULL
                                      OR p.PersonId IN (
                                      SELECT    *
                                      FROM      @SalespersonPermissions )
                                    )
                                AND EXISTS ( SELECT 1
                                             FROM   dbo.DefaultCommission AS c
                                             WHERE  p.PersonId = c.PersonId
                                                    AND [type] = 1
                                                    AND ( @IncludeInactive = 1
                                                          OR ( @Now >= c.StartDate
                                                              AND @Now < c.EndDate
                                                             )
                                                        ) )
                       UNION ALL
                       SELECT   p.PersonId ,
                                p.FirstName ,
                                p.LastName ,
                                p.PTODaysPerAnnum ,
                                p.HireDate ,
                                p.TerminationDate ,
                                p.Alias ,
                                p.DefaultPractice ,
                                p.PracticeName ,
                                p.PersonStatusId ,
                                p.PersonStatusName ,
                                p.EmployeeNumber ,
                                p.SeniorityId ,
                                p.SeniorityName ,
                                p.ManagerId ,
                                p.ManagerAlias ,
                                p.ManagerFirstName ,
                                p.ManagerLastName ,
                                p.PracticeOwnedId ,
                                p.PracticeOwnedName,
                                p.TelephoneNumber
                       FROM     dbo.v_Person AS p
                                INNER JOIN dbo.aspnet_Users AS u ON p.Alias = u.UserName
                                INNER JOIN dbo.aspnet_UsersInRoles AS ur ON u.UserId = ur.UserId
                                INNER JOIN dbo.aspnet_Roles AS r ON ur.RoleId = r.RoleId
                       WHERE    r.RoleName = 'Salesperson' AND 
								(p.PersonStatusId = 1 OR p.PersonStatusId = 3)
                                AND ( @PersonId IS NULL
                                      OR p.PersonId IN (
                                      SELECT    *
                                      FROM      @SalespersonPermissions )
                                    )
                     )
        SELECT  DISTINCT *
        FROM    Salespersons AS s
        ORDER BY s.LastName ,
                s.FirstName

