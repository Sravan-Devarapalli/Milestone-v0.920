CREATE FUNCTION [dbo].[GetDivisionIdsForPractice]
(
	@PracticeId INT
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
  
	DECLARE @Temp NVARCHAR(MAX) = ''
  
	SELECT @Temp = @Temp + CAST(DPA.DivisionId AS NVARCHAR(5))+','
	FROM DivisionPracticeArea DPA
	WHERE DPA.PracticeId=@PracticeId

	RETURN @Temp
END
GO
