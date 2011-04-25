-- =============================================
-- Author:		Skip Sailors
-- Create date: 5-10-2008
-- Description:	reactivate a recruiter role
-- =============================================
CREATE PROCEDURE [dbo].[RecruiterReactivate]
	@RecruiterId int
AS
BEGIN
	SET NOCOUNT ON;

	Update Recruiter
		set Inactive = 0
		where RecruiterId = @RecruiterId
END

