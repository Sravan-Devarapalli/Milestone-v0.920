CREATE PROCEDURE dbo.PersonListAllSeniorityFilter
(
	@PracticeId    INT, 
	@ShowAll       BIT,
	@PageSize      INT,
	@PageNo        INT,
	@Looked		   NVARCHAR(40),
    @StartDate     DATETIME,
    @EndDate       DATETIME,
	@RecruiterId   INT,
	@MaxSeniorityLevel	INT
)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @FirstRecord INT
	SET @FirstRecord = @PageSize * @PageNo

	IF @Looked IS NOT NULL
		SET @Looked = '%' + RTRIM(@Looked) + '%'
	ELSE
		SET @Looked = '%'

    DECLARE @flags bit
    IF (@StartDate IS NULL) OR (@EndDate IS NULL)
		SET @flags = 1
	ELSE
		SET @flags = 0

	IF @FirstRecord IS NULL
	BEGIN
		-- Listing all records
		SELECT p.PersonId,
			   p.FirstName,
			   p.LastName,
			   p.PTODaysPerAnnum,
			   p.HireDate,
			   p.TerminationDate,
			   p.Alias,
			   p.DefaultPractice,
			   p.PracticeName,
			   p.PersonStatusId,
		       p.PersonStatusName,
			   p.EmployeeNumber,
		       p.SeniorityId,
		       p.SeniorityName,
			   p.ManagerId,
			   p.ManagerAlias,
			   p.ManagerFirstName,
			   p.ManagerLastName,
			   p.PracticeOwnedId,
			   p.PracticeOwnedName,
			   p.TelephoneNumber
		  FROM dbo.v_Person AS p
		 WHERE (   ( (p.PersonStatusId = 1 AND @ShowAll = 0) AND @PracticeId IS NULL )
		        OR ((p.PersonStatusId = 1 AND @ShowAll = 0) AND @PracticeId = p.DefaultPractice)
		        OR ( @ShowAll = 1 AND @PracticeId IS NULL )
		        OR ( @ShowAll = 1 AND @PracticeId = p.DefaultPractice ) )
		   AND ( p.FirstName LIKE @Looked OR p.LastName LIKE @Looked OR p.EmployeeNumber LIKE @Looked )
           AND ( @flags = 1 OR dbo.GetOverlappingPlacementDays(p.HireDate, ISNULL(p.TerminationDate, dbo.GetFutureDate()), @StartDate, @EndDate) = 1)
		   AND (   @RecruiterId IS NULL
		        OR EXISTS (SELECT 1
		                     FROM dbo.RecruiterCommission AS c
		                    WHERE c.RecruitId = p.PersonId AND c.RecruiterId = @RecruiterId))
			AND ((@MaxSeniorityLevel IS NULL) OR (@MaxSeniorityLevel >= p.SeniorityId))
		ORDER BY p.LastName, p.FirstName
	END
	ELSE
	BEGIN
		-- Listing a specified page
		DECLARE @LastRecord INT
		SET @LastRecord = @FirstRecord + @PageSize

		SELECT tmp.PersonId,
			   tmp.FirstName,
			   tmp.LastName,
			   tmp.PTODaysPerAnnum,
			   tmp.HireDate,
			   tmp.TerminationDate,
			   tmp.Alias,
			   tmp.DefaultPractice,
			   tmp.PracticeName,
			   tmp.PersonStatusId,
		       tmp.PersonStatusName,
			   tmp.EmployeeNumber,
		       tmp.SeniorityId,
		       tmp.SeniorityName,
			   tmp.ManagerId,
			   tmp.ManagerAlias,
			   tmp.ManagerFirstName,
			   tmp.ManagerLastName,
			   tmp.PracticeOwnedId,
			   tmp.PracticeOwnedName,
			   tmp.TelephoneNumber
		  FROM (
				SELECT TOP (@LastRecord)
					   p.PersonId,
					   p.FirstName,
					   p.LastName,
					   p.PTODaysPerAnnum,
					   p.HireDate,
					   p.TerminationDate,
					   p.Alias,
					   p.DefaultPractice,
					   p.PracticeName,
					   p.PersonStatusId,
		               p.PersonStatusName,
					   p.EmployeeNumber,
					   ROW_NUMBER() OVER(ORDER BY p.LastName, p.FirstName) - 1 AS rownum,
		               p.SeniorityId,
		               p.SeniorityName,
					   p.ManagerId,
					   p.ManagerAlias,
					   p.ManagerFirstName,
					   p.ManagerLastName,
					   p.PracticeOwnedId,
					   p.PracticeOwnedName,
					   p.TelephoneNumber
				  FROM dbo.v_Person AS p
				 WHERE (   ( p.PersonStatusId = 1 AND @ShowAll = 0 AND @PracticeId IS NULL )
		                OR ( p.PersonStatusId = 1 AND @ShowAll = 0 AND @PracticeId = p.DefaultPractice)
		                OR ( @ShowAll = 1 AND @PracticeId IS NULL )
		                OR ( @ShowAll = 1 AND @PracticeId = p.DefaultPractice ) ) 
					AND ( p.FirstName LIKE @Looked OR p.LastName LIKE @Looked OR p.EmployeeNumber LIKE @Looked )
		            AND (   @RecruiterId IS NULL
		                 OR EXISTS (SELECT 1
		                              FROM dbo.RecruiterCommission AS c
		                             WHERE c.RecruitId = p.PersonId AND c.RecruiterId = @RecruiterId))
					AND ((@MaxSeniorityLevel IS NULL) OR (@MaxSeniorityLevel >= p.SeniorityId))
				ORDER BY p.LastName, p.FirstName
		       ) AS tmp
		 WHERE tmp.rownum BETWEEN @FirstRecord AND @LastRecord - 1
		ORDER BY tmp.LastName, tmp.FirstName
	END
END

