-- =============================================
-- Author:		Srinivas.M
-- Create date: 06-05-2012
-- Updated By:	
-- Updated Date: 
-- Description:	Return whether user is Project Owner or not.
-- =============================================
CREATE PROCEDURE [dbo].[AttachOpportunityToProject]
(
	@ProjectId			INT,
	@OpportunityId      INT,
	@UserLogin          NVARCHAR(255),
	@PricingListId      INT=NULL,
	@Link				BIT
)
AS
BEGIN
	
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin
	IF @Link = 1
	BEGIN
		DECLARE @BusinessType INT
		SELECT @BusinessType = BusinessTypeId FROM dbo.Project WHERE ProjectId = @ProjectId

		UPDATE O
			SET O.ProjectId = NULL
		FROM dbo.Opportunity O
		INNER JOIN dbo.Project P ON P.OpportunityId = O.OpportunityId AND P.ProjectId = @ProjectId
		
		EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

		UPDATE P
			SET P.OpportunityId = @OpportunityId
		FROM dbo.Project P
		WHERE P.ProjectId = @ProjectId
		
		EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

		DECLARE @OpportunityNumber NVARCHAR(255)

		UPDATE O
			SET O.ProjectId = @ProjectId,
			    O.PricingListId=@PricingListId,
				@OpportunityNumber = O.OpportunityNumber,
				O.BusinessTypeId = @BusinessType
		FROM dbo.Opportunity O
		WHERE O.OpportunityId = @OpportunityId

		SELECT @OpportunityNumber
		
	END
	ELSE
	BEGIN
		UPDATE P
			SET P.OpportunityId = NULL
		FROM dbo.Project P
		WHERE P.ProjectId = @ProjectId
		
		EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

		UPDATE O
			SET O.ProjectId = NULL
		FROM dbo.Opportunity O
		WHERE O.OpportunityId = @OpportunityId
	END
	
	-- End logging session
	EXEC dbo.SessionLogUnprepare
END

