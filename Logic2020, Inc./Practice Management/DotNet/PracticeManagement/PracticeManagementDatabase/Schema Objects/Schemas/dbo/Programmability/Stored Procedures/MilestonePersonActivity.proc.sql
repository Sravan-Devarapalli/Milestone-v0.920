-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-01-25
-- Description:	Report on milestone-person activity
-- =============================================
CREATE PROCEDURE dbo.MilestonePersonActivity
    @MilestonePersonId INT
AS 
    BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
        SET NOCOUNT ON ;

        SELECT  tes.TimeEntryId,
                tes.ObjectLastName,
                tes.ObjectFirstName,
                tes.ProjectName,
                tes.MilestoneName,
                tes.HoursPerDay,
                tes.ActualHours,
                tes.MilestoneDate
        FROM    dbo.v_TimeEntries AS tes
        WHERE   tes.MilestonePersonId = @MilestonePersonId
        ORDER BY tes.MilestoneDate
    END

