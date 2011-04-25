-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-2-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	9-01-2008
-- Description:	Sets a person's commision for the specific project
-- =============================================
CREATE PROCEDURE [dbo].[CommissionSet]
(
	@CommissionId       INT,
	@ProjectId          INT,
	@PersonId           INT,
	@FractionOfMargin   DECIMAL(18,2),
	@CommissionType     INT,
	@ExpectedDatePaid   DATETIME,
	@ActualDatePaid     DATETIME,
	@MarginTypeId       INT
)
AS
	SET NOCOUNT ON
	
	if @CommissionId IS NULL AND @PersonId IS NOT NULL
	BEGIN
		-- Add a new record
		INSERT INTO dbo.Commission
		            (ProjectId, PersonId, FractionOfMargin, CommissionType,
		             ExpectedDatePaid, ActualDatePaid, MarginTypeId)
		     VALUES (@ProjectId, @PersonId, @FractionOfMargin, @CommissionType,
		             @ExpectedDatePaid, @ActualDatePaid, @MarginTypeId)
	END
	ELSE IF @PersonId IS NOT NULL
	BEGIN
		-- Change an existing record
		UPDATE dbo.Commission
		   SET PersonId = @PersonId,
			   FractionOfMargin = @FractionOfMargin,
			   ExpectedDatePaid = @ExpectedDatePaid,
			   ActualDatePaid = @ActualDatePaid,
			   MarginTypeId = @MarginTypeId
		 WHERE CommissionId = @CommissionId
	END
	ELSE
	BEGIN
		-- @PersonId IS NULL
		-- Delete an existing record
		DELETE
		  FROM dbo.Commission
		 WHERE CommissionId = @CommissionId
	END

