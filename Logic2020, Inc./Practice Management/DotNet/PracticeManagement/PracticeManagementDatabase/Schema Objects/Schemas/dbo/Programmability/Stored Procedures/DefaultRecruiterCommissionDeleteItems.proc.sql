CREATE PROCEDURE [dbo].[DefaultRecruiterCommissionDeleteItems]
    (
      @DefaultRecruiterCommissionHeaderId INT
    )
AS 
    BEGIN 
        SET NOCOUNT ON

	-- Remove items from items table
        DELETE  FROM dbo.DefaultRecruiterCommissionItem
        WHERE   DefaultRecruiterCommissionHeaderId = @DefaultRecruiterCommissionHeaderId
		
    END

