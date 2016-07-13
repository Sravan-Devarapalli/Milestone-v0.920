CREATE PROCEDURE [dbo].[SaveReportFilterValues]
	@CurrentUserId INT,
	@ReportId	   INT,
	@ReportFilters NVARCHAR (MAX),
	@PreviousUserId INT,
	@SessionId     NVARCHAR (MAX)
AS
BEGIN

	DECLARE @ExpiresOn DATETIME
			
	SET @ExpiresOn = DATEADD(day,2,GETDATE())

	IF(NOT EXISTS(SELECT * FROM ReportFilterValues WHERE CurrentUserId=@CurrentUserId AND ReportId=@ReportId AND PreviousUserId=@PreviousUserId AND SessionId=@SessionId))
		BEGIN 
			INSERT INTO ReportFilterValues (Id,CurrentUserId,ReportId,ReportFilters,PreviousUserId,SessionId,ExpiresOn) 
			VALUES (NEWID(),@CurrentUserId,@ReportId,@ReportFilters,@PreviousUserId,@SessionId,@ExpiresOn)
		END
	ELSE
		BEGIN
			UPDATE ReportFilterValues SET ReportFilters=@ReportFilters,ExpiresOn= @ExpiresOn WHERE CurrentUserId=@CurrentUserId AND ReportId=@ReportId AND PreviousUserId=@PreviousUserId AND SessionId=@SessionId
		END
END

