CREATE PROCEDURE [dbo].[PersonReportWithFilters]
(
	@PracticeIdsList     NVARCHAR(MAX), 
	@DivisionIdsList     NVARCHAR(MAX),
	@Looked		   NVARCHAR(40),
	@RecruiterIdsList  NVARCHAR(MAX),
	@TimescaleIdsList   NVARCHAR(MAX),
	@Active   BIT,
	@Projected		BIT,
	@TerminationPending BIT,
	@Terminated		BIT
) 
AS
BEGIN
	SET NOCOUNT ON;
	IF @Looked IS NOT NULL
		SET @Looked = '%' + RTRIM(@Looked) + '%'
	ELSE
		SET @Looked = '%'

		DECLARE @Now DATETIME
		SELECT  @Now=GETDATE()
		
				SELECT 
					   p.PersonId,
					   p.EmployeeNumber,
					   ISNULL(p.PreferredFirstName,p.FirstName) + ' ' + p.LastName AS 'Person Name',
					   p.HireDate AS 'Hire Date',
					   p.PersonStatusName AS 'Satus',
					   p.DivisionName AS 'Division',
					   p.PracticeName AS 'Practice Area',
					   TS.TimescaleName AS 'Pay Type',
			           TS.AmountHourly As 'Hourly Pay Rate',
		               p.Title
				  FROM dbo.v_Person AS p
				  OUTER APPLY 
						(SELECT TOP 1 
							   pay.AmountHourly,
							   pay.TimescaleName,
							   pay.Timescale
						  FROM dbo.v_Pay AS pay
						 WHERE p.PersonId = pay.PersonId
						   AND ((@Now >= pay.StartDate
						   AND @Now < pay.EndDateOrig) OR @Now < pay.StartDate)
						   ORDER BY pay.StartDate
						) TS
				 WHERE (   (p.PersonStatusId = 1 AND @Active = 1) 
							OR (p.PersonStatusId = 2 AND @Terminated = 1)
							OR (p.PersonStatusId = 3 AND @Projected = 1)
							OR (p.PersonStatusId = 5 AND @TerminationPending = 1) 
						) 
		            AND (@PracticeIdsList IS NULL OR p.DefaultPractice IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable] (@PracticeIdsList)))
					AND (@DivisionIdsList IS NULL OR p.DivisionId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable] (@DivisionIdsList)))
					AND ( p.FirstName LIKE @Looked OR p.LastName LIKE @Looked OR p.EmployeeNumber LIKE @Looked OR p.PreferredFirstName LIKE @Looked)
					AND p.IsStrawman = 0  
		            AND (  @RecruiterIdsList IS NULL OR p.RecruiterId IN (SELECT ResultId FROM [dbo].[ConvertStringListIntoTable] (@RecruiterIdsList)))
		            AND (@TimescaleIdsList IS NULL OR	(TS.Timescale IN ((SELECT ResultId FROM [dbo].[ConvertStringListIntoTable] (@TimescaleIdsList)))) 	)
					ORDER BY  PersonId
	END

