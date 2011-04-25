-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2009-11-05
-- Description:	Cleans up unused headers
-- =============================================
CREATE PROCEDURE DefaultRecruiterCommissionCleanup
    (
      @DefaultRecruiterCommissionHeaderId INT
    )
AS
BEGIN
	SET NOCOUNT ON;
	
	-- Remove headers from headers table if there's no 
	--   corresponding values in the items table
    DECLARE @headersCount INT

    SELECT  @headersCount = COUNT(*)
    FROM    dbo.DefaultRecruiterCommissionItem
    WHERE   DefaultRecruiterCommissionHeaderId = @DefaultRecruiterCommissionHeaderId

    IF @headersCount = 0 
        BEGIN
            DELETE  FROM dbo.DefaultRecruiterCommissionHeader
            WHERE   DefaultRecruiterCommissionHeaderId = @DefaultRecruiterCommissionHeaderId
        END
END

