-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-29-2008
-- Updated by:	Anatoliy Lokshin
-- Update date: 11-06-2008
-- Description:	Calculates a number of the persons.
-- =============================================
CREATE PROCEDURE [dbo].[PersonGetCount]
(
	@PracticeId    INT = NULL, 
	@ShowAll       BIT = 0,
	@Looked		   NVARCHAR(40) = NULL,
	@RecruiterId   INT,
	@TimeScaleId   INT = NULL,
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

	SELECT COUNT(*) AS num
	  FROM dbo.v_Person AS p
	 WHERE (   (p.PersonStatusId = 1 AND @ShowAll = 0)  --@ShowAll is reverse of Active
				OR (p.PersonStatusId = 2 AND @Terminated = 1)
				OR (p.PersonStatusId = 3 AND @Projected = 1)
				OR (p.PersonStatusId = 4 AND @Inactive = 1) 
			) 
		AND ( p.DefaultPractice = @PracticeId OR @PracticeId IS NULL )
		AND (p.FirstName LIKE @Looked OR p.LastName LIKE @Looked OR p.EmployeeNumber LIKE @Looked )
		AND (   @RecruiterId IS NULL
	        OR EXISTS (SELECT 1
	                     FROM dbo.RecruiterCommission AS c
	                    WHERE c.RecruitId = p.PersonId AND c.RecruiterId = @RecruiterId))
		AND (	@TimeScaleId IS NULL
			OR EXISTS (SELECT 1
						FROM dbo.v_Pay AS pay
						WHERE pay.PersonId = p.PersonId AND pay.Timescale = @TimeScaleId))
		AND ( p.LastName LIKE @Alphabet )



GO




