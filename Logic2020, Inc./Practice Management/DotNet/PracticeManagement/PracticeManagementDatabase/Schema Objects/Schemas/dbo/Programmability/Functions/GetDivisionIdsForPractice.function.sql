CREATE FUNCTION [dbo].[GetDivisionIdsForPractice]
(
	@PracticeId INT,
	@IsProject INT
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
  
	DECLARE @Temp NVARCHAR(MAX) = ''

  IF @IsProject=0 --person division mappings
  BEGIN
	SELECT @Temp = @Temp + CAST(DPA.DivisionId AS NVARCHAR(5))+','
	FROM DivisionPracticeArea DPA
	WHERE DPA.PracticeId=@PracticeId
  END

  ELSE --project division mappings
  BEGIN 
	SELECT @Temp = @Temp + CAST(DPA.ProjectDivisionId AS NVARCHAR(5))+','
	FROM ProjectDivisionPracticeMapping DPA
	WHERE DPA.PracticeId=@PracticeId
  END

	RETURN @Temp
END
GO

