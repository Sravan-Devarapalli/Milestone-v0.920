CREATE PROCEDURE [dbo].[CheckIfProjectNumberExists]
(
	@ProjectNumber	NVARCHAR(MAX)
)
AS
BEGIN

	IF EXISTS(SELECT 1 FROM dbo.Project WHERE ProjectNumber = @ProjectNumber)
	BEGIN
			SELECT CONVERT(BIT,1) Result 
	END
	ELSE
	BEGIN
			SELECT CONVERT(BIT,0) Result
	END

END
