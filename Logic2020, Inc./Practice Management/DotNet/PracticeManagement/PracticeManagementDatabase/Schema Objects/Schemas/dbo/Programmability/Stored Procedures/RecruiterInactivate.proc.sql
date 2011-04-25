-- =============================================
-- Author:		Skip Sailors
-- Create date: 5-10-2008
-- Description:	Inactivare recruiter role
-- =============================================
CREATE PROCEDURE [dbo].[RecruiterInactivate] 
	-- Add the parameters for the stored procedure here
	@RecruiterId int
AS
BEGIN
	SET NOCOUNT ON;

	Update Recruiter
		set Inactive = 1
		where RecruiterId = @RecruiterId
END

