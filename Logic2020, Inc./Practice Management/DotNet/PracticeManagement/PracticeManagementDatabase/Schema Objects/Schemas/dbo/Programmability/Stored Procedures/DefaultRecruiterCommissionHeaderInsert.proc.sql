-- =============================================
-- Author:        Anatoliy Lokshin
-- Create date: 9-02-2008
-- Update by:     Anatoliy Lokshin
-- Update date:   11-17-2008
-- Description:   Inserts a person's default actual recruiting commisions
-- =============================================
CREATE PROCEDURE dbo.DefaultRecruiterCommissionHeaderInsert
(
      @PersonId                           INT,
      @StartDate                          DATETIME,
      @EndDate                            DATETIME,
      @DefaultRecruiterCommissionHeaderId INT OUT
)
AS
      SET NOCOUNT ON

	  DECLARE @ErrorMessage NVARCHAR(2048)
      SET @EndDate = ISNULL(@EndDate, dbo.GetFutureDate())

      IF EXISTS(SELECT 1
                  FROM dbo.DefaultRecruiterCommissionHeader AS h
                 WHERE h.PersonId = @PersonId AND h.StartDate >= @StartDate AND h.EndDate <= @EndDate)
      BEGIN
            -- The record overlaps from beginning
			SELECT @ErrorMessage = [dbo].[GetErrorMessage](70013)
			RAISERROR (@ErrorMessage, 16, 1)
            RETURN
      END
      ELSE IF EXISTS(SELECT 1
                       FROM dbo.DefaultRecruiterCommissionHeader AS h
                      WHERE h.PersonId = @PersonId AND h.EndDate <= @EndDate AND h.EndDate > @StartDate)
      BEGIN
            -- The record overlaps from ending
			SELECT @ErrorMessage = [dbo].[GetErrorMessage](70014)
			RAISERROR (@ErrorMessage, 16, 1)
            RETURN
      END
      ELSE IF EXISTS(SELECT 1
                       FROM dbo.DefaultRecruiterCommissionHeader AS h
                      WHERE h.PersonId = @PersonId AND h.StartDate <= @StartDate AND h.EndDate > @EndDate)
      BEGIN
            -- The record overlaps within the period
			SELECT @ErrorMessage = [dbo].[GetErrorMessage](70015)
			RAISERROR (@ErrorMessage, 16, 1)
            RETURN
      END

      BEGIN TRAN

      UPDATE dbo.DefaultRecruiterCommissionHeader
         SET EndDate = @StartDate
       WHERE PersonId = @PersonId AND EndDate > @StartDate AND StartDate < @StartDate

      UPDATE dbo.DefaultRecruiterCommissionHeader
         SET StartDate = @EndDate
       WHERE PersonId = @PersonId AND StartDate < @EndDate AND EndDate > @EndDate

      INSERT INTO dbo.DefaultRecruiterCommissionHeader
                  (PersonId, StartDate, EndDate)
           VALUES (@PersonId, @StartDate, @EndDate)

      SET @DefaultRecruiterCommissionHeaderId = SCOPE_IDENTITY()

      COMMIT TRAN

