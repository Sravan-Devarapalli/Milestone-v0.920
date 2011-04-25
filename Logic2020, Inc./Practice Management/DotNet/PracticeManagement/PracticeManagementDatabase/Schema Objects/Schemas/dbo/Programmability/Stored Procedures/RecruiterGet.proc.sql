-- =============================================
-- Author:		Skip Sailors
-- Create date: 5-10-2008
-- Description:	Does a recruiter exist for a person?
-- =============================================
CREATE PROCEDURE [dbo].[RecruiterGet] 
	@RecruiterId int,
	@ShowAll bit = 0,
	@HasRecruiter bit = 0 output
AS
BEGIN
	SET NOCOUNT ON;

	if @ShowAll = 1
		select @HasRecruiter = count(*)
			from Recruiter
			where RecruiterId = @RecruiterId
	else
		select @HasRecruiter = count(*)
			from Recruiter
			where RecruiterId = @RecruiterId
				and Inactive = 0
	select @HasRecruiter
END

