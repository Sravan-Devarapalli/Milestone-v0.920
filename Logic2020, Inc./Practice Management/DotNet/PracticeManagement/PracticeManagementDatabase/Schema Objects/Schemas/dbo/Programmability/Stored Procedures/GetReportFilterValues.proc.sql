CREATE PROCEDURE [dbo].[GetReportFilterValues]
	@CurrentUserId INT,
	@ReportId	   INT,
	@PreviousUserId INT,
	@SessionId     NVARCHAR (MAX)
AS
BEGIN
	SELECT ReportFilters 
	FROM ReportFilterValues 
	WHERE CurrentUserId=@CurrentUserId AND ReportId=@ReportId AND PreviousUserId=@PreviousUserId AND SessionId=@SessionId

END

