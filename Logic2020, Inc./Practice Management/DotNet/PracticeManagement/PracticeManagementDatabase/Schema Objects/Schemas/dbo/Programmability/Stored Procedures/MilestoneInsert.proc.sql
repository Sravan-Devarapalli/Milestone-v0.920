-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-22-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	11-27-2008
-- Description:	Inserts a Milestone
-- Updated By : Ravi Narsini Chnages: Added default milestone logic (#2600).
-- =============================================
CREATE PROCEDURE dbo.MilestoneInsert
(
	@MilestoneId              INT OUT,
	@ProjectId                INT,
	@Description              NVARCHAR(255),
	@Amount                   DECIMAL(18,2),
	@StartDate                DATETIME,
	@ProjectedDeliveryDate    DATETIME,
	@ActualDeliveryDate       DATETIME,
	@IsHourlyAmount           BIT,
	@UserLogin                NVARCHAR(255),
	@ConsultantsCanAdjust	  BIT,
	@IsChargeable			  BIT,
	@IsDefault				  BIT = 0
)
AS
	SET NOCOUNT ON

	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	INSERT INTO dbo.Milestone
	            (ProjectId, Description, Amount, StartDate,
	             ProjectedDeliveryDate, ActualDeliveryDate, IsHourlyAmount, ConsultantsCanAdjust, IsChargeable, IsDefault)
	     VALUES (@ProjectId, @Description, @Amount, @StartDate,
	             @ProjectedDeliveryDate, @ActualDeliveryDate, @IsHourlyAmount, @ConsultantsCanAdjust, @IsChargeable, @IsDefault)

	-- End logging session
	EXEC dbo.SessionLogUnprepare

	SET @MilestoneId = SCOPE_IDENTITY()

	
