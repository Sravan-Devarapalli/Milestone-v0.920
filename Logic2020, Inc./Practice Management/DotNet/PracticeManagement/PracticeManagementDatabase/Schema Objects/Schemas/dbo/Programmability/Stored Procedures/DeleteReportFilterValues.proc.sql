CREATE PROCEDURE [dbo].[DeleteReportFilterValues]
	@CurrentUserId INT,
	@PreviousUserId INT,
	@SessionId     NVARCHAR (MAX)
AS
	BEGIN

	DELETE FROM ReportFilterValues 
		   WHERE CurrentUserId=@CurrentUserId AND PreviousUserId=@PreviousUserId AND SessionId=@SessionId
	END

