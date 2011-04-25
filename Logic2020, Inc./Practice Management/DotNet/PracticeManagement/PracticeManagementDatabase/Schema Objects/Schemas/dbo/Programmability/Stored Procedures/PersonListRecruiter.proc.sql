-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 8-04-2008
-- Updated by:	Anatoliy Lokshin
-- Update date: 12-01-2008
-- Description:	Retrives the list of Persons who receives the Recruiting commissions.
-- =============================================
CREATE PROCEDURE dbo.PersonListRecruiter
    (
      @PersonId INT ,
      @HireDate DATETIME
    )
AS 
    SET NOCOUNT ON

    IF @HireDate IS NULL
        AND @PersonId IS NOT NULL 
        BEGIN
            SELECT  @HireDate = p.HireDate
            FROM    dbo.Person AS p
            WHERE   PersonId = @PersonId
        END
    ELSE 
        IF @HireDate IS NULL 
            BEGIN
                SET @HireDate = GETDATE()
            END

    SELECT  p.PersonId ,
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
    FROM    dbo.v_Person AS p
    WHERE   p.PersonStatusId = 1	-- Active person only
            /*AND ( EXISTS ( SELECT   *
                           FROM     v_UsersInRoles AS ur
                           WHERE    ur.UserName = p.Alias
                                    AND ur.RoleName = 'Recruiter' ) )*/
	   AND (   EXISTS (SELECT 1
	                     FROM dbo.DefaultRecruiterCommissionHeader AS c
	                    WHERE p.PersonId = c.PersonId
	                      AND @HireDate >= c.StartDate
	                      AND @HireDate < c.EndDate)
	        OR EXISTS (SELECT 1
	                     FROM dbo.RecruiterCommission AS rc
	                    WHERE rc.RecruitId = @PersonId AND rc.RecruiterId = p.PersonId)
	       )
    ORDER BY p.LastName ,
            p.FirstName

