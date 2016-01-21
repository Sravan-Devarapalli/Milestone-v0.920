CREATE PROCEDURE [dbo].[DeleteReportFilterValues]
	@CurrentUserId INT,
	@PreviousUserId INT
AS
	BEGIN

	DELETE FROM ReportFilterValues 
		   WHERE CurrentUserId=@CurrentUserId AND PreviousUserId=@PreviousUserId
	END

