CREATE PROCEDURE [dbo].PersonGetCountByCommaSeparatedIdsList
(
	@PracticeIdsList     NVARCHAR(MAX), 
	@ShowAll       BIT = 0,
	@Looked		   NVARCHAR(40) = NULL,
	@RecruiterIdsList  NVARCHAR(MAX),
	@TimescaleIdsList   NVARCHAR(MAX),
	@Projected		BIT,
	@Terminated		BIT,
	@TerminationPending BIT,
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

	DECLARE @NOW DATETIME
	SET @NOW = GETDATE()

	SELECT COUNT(*) AS num
	  FROM dbo.v_Person AS p
	 WHERE (   (p.PersonStatusId = 1 AND @ShowAll = 0)  --@ShowAll is reverse of Active
				OR (p.PersonStatusId = 2 AND @Terminated = 1)
				OR (p.PersonStatusId = 3 AND @Projected = 1)
				OR (p.PersonStatusId = 5 AND @TerminationPending  = 1) 
			) 
		AND (@PracticeIdsList IS NULL OR p.DefaultPractice  IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable] (@PracticeIdsList)))
		AND (p.FirstName LIKE @Looked OR p.LastName LIKE @Looked OR p.EmployeeNumber LIKE @Looked )
		AND ( @RecruiterIdsList IS NULL OR p.RecruiterId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable] (@RecruiterIdsList)))
		AND (@TimescaleIdsList IS NULL
			OR EXISTS (SELECT 1 
						FROM (SELECT TOP 1 pay.Timescale
								FROM dbo.v_Pay AS pay
								WHERE pay.PersonId = p.PersonId 
								AND((@Now >= pay.StartDate AND @Now < pay.EndDateOrig) OR @Now < pay.StartDate)
								ORDER BY pay.StartDate
								) d
						WHERE d.Timescale IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable] (@TimescaleIdsList))))
		AND ( p.LastName LIKE @Alphabet )
		AND p.IsStrawman = 0  

