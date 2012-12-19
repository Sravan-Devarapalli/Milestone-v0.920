CREATE PROCEDURE dbo.PersonListRecruiter
AS 
    SELECT  p.PersonId ,
            p.FirstName ,
            p.LastName ,
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
    INNER JOIN  v_UsersInRoles AS ur ON ur.UserName = p.Alias
                                    AND ur.RoleName = 'Recruiter' 
	WHERE   p.PersonStatusId IN (1,5)	-- Active person only
    ORDER BY p.LastName ,
            p.FirstName

