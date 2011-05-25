CREATE PROCEDURE [dbo].PersonGetCountByCommaSeparatedIdsList
(
	@PracticeIdsList     NVARCHAR(MAX), 
	@ShowAll       BIT = 0,
	@Looked		   NVARCHAR(40) = NULL,
	@RecruiterIdsList  NVARCHAR(MAX),
	@TimescaleIdsList   NVARCHAR(MAX),
	@Projected		BIT,
	@Terminated		BIT,
	@Inactive		BIT,
	@Alphabet		NVARCHAR(5)
)
AS
	SET NOCOUNT ON

	IF @Looked IS NOT NULL
		SET @Looked = '%' + RTRIM(@Looked) + '%'
	ELSE
		SET @Looked = '%'
		
	IF @Alphabet IS NOT NULL
		SET @Alphabet = @Alphabet + '%'
	ELSE
		SET @Alphabet = '%'

	DECLARE @PracticeIdsTable TABLE
	(
		PracticeId INT
	)

	INSERT INTO @PracticeIdsTable
	SELECT ResultId 
	FROM [dbo].[ConvertStringListIntoTable] (@PracticeIdsList)
	
	DECLARE @TimeScaleIdsTable TABLE
	(
		TimeScaleId INT
	)

	INSERT INTO @TimeScaleIdsTable
	SELECT ResultId 
	FROM [dbo].[ConvertStringListIntoTable] (@TimescaleIdsList)

	DECLARE @RecruiterIdsTable TABLE
	(
		RecruiterId INT
	)

	INSERT INTO @RecruiterIdsTable
	SELECT ResultId 
	FROM [dbo].[ConvertStringListIntoTable] (@RecruiterIdsList)

	SELECT COUNT(*) AS num
	  FROM dbo.v_Person AS p
	 WHERE (   (p.PersonStatusId = 1 AND @ShowAll = 0)  --@ShowAll is reverse of Active
				OR (p.PersonStatusId = 2 AND @Terminated = 1)
				OR (p.PersonStatusId = 3 AND @Projected = 1)
				OR (p.PersonStatusId = 4 AND @Inactive = 1) 
			) 
		AND ( p.DefaultPractice  IN (SELECT PracticeId FROM @PracticeIdsTable)  OR @PracticeIdsList IS NULL )
		AND (p.FirstName LIKE @Looked OR p.LastName LIKE @Looked OR p.EmployeeNumber LIKE @Looked )
		AND ( @RecruiterIdsList IS NULL
	        OR EXISTS (SELECT 1
	                     FROM dbo.RecruiterCommission AS c
	                    WHERE c.RecruitId = p.PersonId AND c.RecruiterId IN (SELECT RecruiterId FROM @RecruiterIdsTable)))
		AND (@TimescaleIdsList IS NULL
			OR EXISTS (SELECT 1
						FROM dbo.v_Pay AS pay
						WHERE pay.PersonId = p.PersonId AND pay.Timescale IN (SELECT TimeScaleId FROM @TimeScaleIdsTable)))
		AND ( p.LastName LIKE @Alphabet )
