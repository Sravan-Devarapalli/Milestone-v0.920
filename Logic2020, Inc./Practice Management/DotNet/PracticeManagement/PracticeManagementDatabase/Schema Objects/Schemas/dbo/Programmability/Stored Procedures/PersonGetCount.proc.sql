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
	@RecruiterId   INT
)
AS
	SET NOCOUNT ON

	IF @Looked IS NOT NULL
		SET @Looked = '%' + RTRIM(@Looked) + '%'
	ELSE
		SET @Looked = '%'

	SELECT COUNT(*) AS num
	  FROM dbo.v_Person AS p
	 WHERE (   ( p.PersonStatusId = 1 AND @ShowAll = 0 AND @PracticeId IS NULL )
	        OR ( p.PersonStatusId = 1 AND @ShowAll = 0 AND @PracticeId = p.DefaultPractice)
	        OR ( @ShowAll = 1 AND @PracticeId IS NULL )
	        OR ( @ShowAll = 1 AND @PracticeId = p.DefaultPractice ) )
	   AND (p.FirstName LIKE @Looked OR p.LastName LIKE @Looked OR p.EmployeeNumber LIKE @Looked )
	   AND (   @RecruiterId IS NULL
	        OR EXISTS (SELECT 1
	                     FROM dbo.RecruiterCommission AS c
	                    WHERE c.RecruitId = p.PersonId AND c.RecruiterId = @RecruiterId))

