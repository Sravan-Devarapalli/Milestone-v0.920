CREATE PROCEDURE dbo.CompanyLogoDataSave
(
	@Title                NVARCHAR(256),
	@FileName            NVARCHAR(256),
	@FilePath            NVARCHAR(1024),
	@Data                VARBINARY(MAX)
)
AS
	SET NOCOUNT ON
	BEGIN
		DECLARE @COUNT INT 
		SELECT @COUNT=COUNT(*) 
		FROM dbo.CompanyLogoSetting
		IF(@COUNT = 0)
		BEGIN
			INSERT INTO CompanyLogoSetting(Title,FileName,FilePath,Data)
       		VALUES (@Title,@FileName,@FilePath,@Data)
		END
		ELSE
		BEGIN
			UPDATE  CompanyLogoSetting
			SET  Title = ISNULL(@Title,Title),
				 FileName = ISNULL(@FileName,FileName),
				 FilePath = ISNULL(@FilePath,FilePath),
				 Data = ISNULL(@Data,Data)
		END
	END

