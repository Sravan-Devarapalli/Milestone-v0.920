CREATE PROCEDURE [dbo].[GetReportFilterValues]
	@CurrentUserId INT,
	@ReportId	   INT,
	@PreviousUserId INT
AS
BEGIN
	SELECT ReportFilters 
	FROM ReportFilterValues 
	WHERE CurrentUserId=@CurrentUserId AND ReportId=@ReportId AND PreviousUserId=@PreviousUserId

END

