CREATE PROCEDURE [dbo].[PersonDivisionHistory]
	(
		@PersonID NVARCHAR(100)
	)
AS
BEGIN
		DECLARE @FutureDate DATETIME
		SELECT @FutureDate = dbo.GetFutureDate()
		SELECT p.FirstName +', ' +p.LastName AS Person,
				CONVERT(DATE,dh.StartDate) AS Startdate,
				ISNULL(DATEADD(dd,-1,dh.EndDate),@FutureDate) AS Enddate,
				ISNULL(pd.DivisionName,'') AS Division
		FROM dbo.v_DivisionHistory dh
		INNER JOIN dbo.Person P ON dh.PersonId = p.PersonId AND p.EmployeeNumber = @PersonID
		LEFT JOIN dbo.PersonDivision pd ON pd.DivisionId = dh.DivisionId
END
