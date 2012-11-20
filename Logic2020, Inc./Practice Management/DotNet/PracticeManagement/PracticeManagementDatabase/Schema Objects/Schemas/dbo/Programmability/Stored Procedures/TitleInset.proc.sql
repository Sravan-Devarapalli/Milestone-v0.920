CREATE PROCEDURE [dbo].[TitleInset]
(
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

	INSERT INTO dbo.Title(Title,TitleTypeId,SortOrder,PTOAccrual,MinimumSalary,MaximumSalary)
	SELECT @Title,@TitleTypeId,@SortOrder,@PTOAccrual,@MinimumSalary,@MaximumSalary
	
	EXEC SessionLogUnprepare 
END

