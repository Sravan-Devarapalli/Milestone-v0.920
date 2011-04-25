-- =============================================
-- Author:		SkipSailors
-- Create date: 5-10-2008
-- Description:	Add a recruiter to the system
-- =============================================
CREATE PROCEDURE [dbo].[RecruiterInsert] 
	@RecruiterId int
AS
BEGIN
	SET NOCOUNT ON;

	Insert Recruiter (RecruiterId) values (@RecruiterId)
END

