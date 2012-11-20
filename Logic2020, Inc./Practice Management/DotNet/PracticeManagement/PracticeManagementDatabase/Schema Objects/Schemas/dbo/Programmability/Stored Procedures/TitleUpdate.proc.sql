CREATE PROCEDURE [dbo].[TitleUpdate]
(
	@TitleId		INT,
	@Title			NVARCHAR(100),
	@TitleTypeId	INT,
	@SortOrder		INT,
	@PTOAccrual		INT,
	@MinimumSalary	INT,
	@MaximumSalary	INT,
	@UserLogin  NVARCHAR(255) 
)
AS
BEGIN
	EXEC SessionLogPrepare @UserLogin = @UserLogin

	UPDATE dbo.Title
	SET Title = @Title,
	TitleTypeId = @TitleTypeId,
	SortOrder = @SortOrder,
	PTOAccrual = @PTOAccrual,
	MinimumSalary = @MinimumSalary,
	MaximumSalary = @MaximumSalary
	WHERE TitleId = @TitleId

	EXEC SessionLogUnprepare 
END

