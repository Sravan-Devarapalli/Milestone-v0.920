CREATE PROCEDURE [dbo].[SaveReportFilterValues]
	@CurrentUserId INT,
	@ReportId	   INT,
	@ReportFilters NVARCHAR (MAX),
	@PreviousUserId INT
AS
BEGIN
	IF(NOT EXISTS(SELECT * FROM ReportFilterValues WHERE CurrentUserId=@CurrentUserId AND ReportId=@ReportId AND PreviousUserId=@PreviousUserId))
		BEGIN 
			INSERT INTO ReportFilterValues (Id,CurrentUserId,ReportId,ReportFilters,PreviousUserId) 
			VALUES (NEWID(),@CurrentUserId,@ReportId,@ReportFilters,@PreviousUserId)
		END
	ELSE
		BEGIN
			UPDATE ReportFilterValues SET ReportFilters=@ReportFilters WHERE CurrentUserId=@CurrentUserId AND ReportId=@ReportId AND PreviousUserId=@PreviousUserId
		END
END

