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
	@IsFromMilestoneDetail BIT = 0,
	@MilestoneCloneId   INT OUTPUT
)
AS
	SET NOCOUNT ON

	BEGIN TRY
	BEGIN TRAN  Tran_MilestoneClone

	DECLARE @Today DATETIME
	SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETUTCDATE())))
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

    IF @IsFromMilestoneDetail = 1
	BEGIN
	--insert milestonepersonentries for Persons in milestone cloning in milestonedetail page.
	INSERT INTO dbo.MilestonePersonEntry
	            (MilestonePersonId, StartDate, EndDate, PersonRoleId, Amount, HoursPerDay)
	     (SELECT mp.MilestonePersonId, mp.StartDate, mp.ProjectedDeliveryDate, mp.PersonRoleId, mp.Amount, mp.HoursPerDay
	       FROM (
	             SELECT mpc.MilestonePersonId,
	                    CASE WHEN m.StartDate > PH.HireDate THEN m.StartDate ELSE PH.HireDate END AS StartDate,
	                    CASE WHEN PH.TerminationDate IS NULL OR m.ProjectedDeliveryDate < PH.TerminationDate THEN m.ProjectedDeliveryDate ELSE PH.TerminationDate END AS ProjectedDeliveryDate,
	                    mpe.PersonRoleId,
	                    mpe.Amount,
	                    mpe.HoursPerDay,
	                    ROW_NUMBER() OVER(PARTITION BY mp.PersonId,PH.PersonId,PH.HireDate ORDER BY mpe.StartDate DESC) AS RowNum
	               FROM dbo.MilestonePersonEntry AS mpe
	                    INNER JOIN dbo.MilestonePerson AS mp
	                        ON mp.MilestonePersonId = mpe.MilestonePersonId AND mp.MilestoneId = @MilestoneId
	                    INNER JOIN dbo.MilestonePerson AS mpc
	                        ON mp.PersonId = mpc.PersonId AND mpc.MilestoneId = @MilestoneCloneId
	                    INNER JOIN dbo.Milestone AS m
	                        ON m.MilestoneId = mpc.MilestoneId
						INNER JOIN dbo.Person AS p on p.PersonId = mp.PersonId AND p.IsStrawman = 0						
						INNER JOIN v_PersonHistory AS PH ON PH.PersonId = mp.PersonId AND PH.HireDate <= M.ProjectedDeliveryDate AND (PH.TerminationDate IS NULL OR M.StartDate <= PH.TerminationDate)
	            ) AS mp
	      -- Take a last mileston-person entry if several exists
	      WHERE mp.RowNum = 1
		UNION ALL		
		--insert milestonepersonentries for Strawman.
		  SELECT mpc.MilestonePersonId,
	            m.StartDate,
	            m.ProjectedDeliveryDate,
	            mpe.PersonRoleId,
	            mpe.Amount,
	            mpe.HoursPerDay
	        FROM dbo.MilestonePersonEntry AS mpe
	            INNER JOIN dbo.MilestonePerson AS mp
	                ON mp.MilestonePersonId = mpe.MilestonePersonId AND mp.MilestoneId = @MilestoneId
	            INNER JOIN dbo.MilestonePerson AS mpc
	                ON mp.PersonId = mpc.PersonId AND mpc.MilestoneId = @MilestoneCloneId
	            INNER JOIN dbo.Milestone AS m
	                ON m.MilestoneId = mpc.MilestoneId
				INNER JOIN dbo.Person AS p on p.PersonId = mp.PersonId AND p.IsStrawman = 1 
				)
	END
	ELSE
	BEGIN
	--insert milestonepersonentries for Persons in project cloning.
	INSERT INTO dbo.MilestonePersonEntry
	            (MilestonePersonId, StartDate, EndDate, PersonRoleId, Amount, HoursPerDay)
	     (
	             SELECT mpc.MilestonePersonId,
						CASE WHEN mpe.StartDate > PH.HireDate THEN mpe.StartDate ELSE PH.HireDate END,
	                    CASE WHEN PH.TerminationDate IS NULL OR mpe.EndDate < PH.TerminationDate THEN mpe.EndDate ELSE PH.TerminationDate END,
	                    mpe.PersonRoleId,
	                    mpe.Amount,
	                    mpe.HoursPerDay
	               FROM dbo.MilestonePersonEntry AS mpe
	                    INNER JOIN dbo.MilestonePerson AS mp
	                        ON mp.MilestonePersonId = mpe.MilestonePersonId AND mp.MilestoneId = @MilestoneId
	                    INNER JOIN dbo.MilestonePerson AS mpc
	                        ON mp.PersonId = mpc.PersonId AND mpc.MilestoneId = @MilestoneCloneId	
						INNER JOIN v_PersonHistoryAndStrawman AS PH ON PH.PersonId = mp.PersonId AND PH.HireDate <= mpe.EndDate AND (PH.TerminationDate IS NULL OR mpe.StartDate <= PH.TerminationDate)
		)
	END


	--exec dbo.ExpensesClone @OldMilestoneId = @MilestoneId, @NewMilestoneId = @MilestoneCloneId
	
	exec dbo.NotesClone @OldTargetId = @MilestoneId, @NewTargetId = @MilestoneCloneId

	IF @MilestoneCloneId IS NOT NULL
	BEGIN
		EXEC dbo.InsertProjectFeedbackByMilestonePersonId @MilestonePersonId = NULL,@MilestoneId = @MilestoneCloneId
	END

	UPDATE dbo.Project
	SET CreatedDate = @Today
	WHERE ProjectId = @ProjectId

    COMMIT TRAN Tran_MilestoneClone
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN Tran_MilestoneClone
		
		DECLARE @ErrorMessage NVARCHAR(MAX)
		SET @ErrorMessage = ERROR_MESSAGE()

		RAISERROR(@ErrorMessage, 16, 1)
	END CATCH

