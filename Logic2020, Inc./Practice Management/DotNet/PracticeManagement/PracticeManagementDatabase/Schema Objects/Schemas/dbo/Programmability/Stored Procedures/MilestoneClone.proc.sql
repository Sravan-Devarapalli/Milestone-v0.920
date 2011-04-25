-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 11-11-2008
-- Updated by:  
-- Update date: 
-- Description:	Clones a specified milestone and sets a specified duration to new one.
-- =============================================
CREATE PROCEDURE [dbo].[MilestoneClone]
(
	@MilestoneId        INT,
	@CloneDuration      INT,
	@ProjectId			INT = NULL,
	@MilestoneCloneId   INT OUTPUT
)
AS
	SET NOCOUNT ON

begin transaction

	-- Create a milestone record
	INSERT INTO dbo.Milestone
	            (ProjectId, Description, Amount, StartDate, ProjectedDeliveryDate, IsHourlyAmount)
	     SELECT ISNULL(@ProjectId, m.ProjectId),
	            SUBSTRING(m.Description + ' (cloned)', 1, 255),
	            m.Amount,
	            CASE 
	             WHEN @ProjectId IS NOT NULL THEN m.StartDate
	             ELSE DATEADD(dd, 1, m.ProjectedDeliveryDate)
	            END,
	            CASE 
	             WHEN @ProjectId IS NOT NULL THEN m.ProjectedDeliveryDate
	             ELSE DATEADD(dd, @CloneDuration, m.ProjectedDeliveryDate)
	            END,
	            m.IsHourlyAmount
	       FROM dbo.Milestone AS m
	      WHERE m.MilestoneId = @MilestoneId

	SET @MilestoneCloneId = SCOPE_IDENTITY()

	-- Copy the persons list
	INSERT INTO dbo.MilestonePerson
	            (MilestoneId, PersonId)
	     SELECT @MilestoneCloneId, mp.PersonId
	       FROM dbo.MilestonePerson AS mp
	      WHERE mp.MilestoneId = @MilestoneId

	INSERT INTO dbo.MilestonePersonEntry
	            (MilestonePersonId, StartDate, EndDate, PersonRoleId, Amount, HoursPerDay)
	     SELECT mp.MilestonePersonId, mp.StartDate, mp.ProjectedDeliveryDate, mp.PersonRoleId, mp.Amount, mp.HoursPerDay
	       FROM (
	             SELECT mpc.MilestonePersonId,
	                    m.StartDate,
	                    m.ProjectedDeliveryDate,
	                    mpe.PersonRoleId,
	                    mpe.Amount,
	                    mpe.HoursPerDay,
	                    ROW_NUMBER() OVER(PARTITION BY mp.PersonId ORDER BY mpe.StartDate DESC) AS RowNum
	               FROM dbo.MilestonePersonEntry AS mpe
	                    INNER JOIN dbo.MilestonePerson AS mp
	                        ON mp.MilestonePersonId = mpe.MilestonePersonId AND mp.MilestoneId = @MilestoneId
	                    INNER JOIN dbo.MilestonePerson AS mpc
	                        ON mp.PersonId = mpc.PersonId AND mpc.MilestoneId = @MilestoneCloneId
	                    INNER JOIN dbo.Milestone AS m
	                        ON m.MilestoneId = mpc.MilestoneId
	            ) AS mp
	      -- Take a last mileston-person entry if several exists
	      WHERE mp.RowNum = 1

	exec dbo.ExpensesClone @OldMilestoneId = @MilestoneId, @NewMilestoneId = @MilestoneCloneId
	
	exec dbo.NotesClone @OldTargetId = @MilestoneId, @NewTargetId = @MilestoneCloneId
	
commit transaction
