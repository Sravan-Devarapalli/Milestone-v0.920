-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 10-10-2008
-- Updated by:	
-- Update date:	
-- Description:	Saves the billing info for the project.
-- =============================================
CREATE PROCEDURE [dbo].[ProjectBillingInfoSave]
(
	@ProjectId         INT,
	@BillingContact    NVARCHAR(100),
	@BillingPhone      NVARCHAR(25),
	@BillingEmail      NVARCHAR(100),
	@BillingType       NVARCHAR(25),
	@BillingAddress1   NVARCHAR(100),
	@BillingAddress2   NVARCHAR(100),
	@BillingCity       NVARCHAR(50),
	@BillingState      NVARCHAR(50),
	@BillingZip        NVARCHAR(10),
	@PurchaseOrder     NVARCHAR(25)
)
AS
	SET NOCOUNT ON

	IF EXISTS(SELECT 1 FROM dbo.ProjectBillingInfo WHERE ProjectId = @ProjectId)
	BEGIN
		UPDATE dbo.ProjectBillingInfo
		   SET BillingContact = @BillingContact,
		       BillingPhone = @BillingPhone,
		       BillingEmail = @BillingEmail,
		       BillingType = @BillingType,
		       BillingAddress1 = @BillingAddress1,
		       BillingAddress2 = @BillingAddress2,
		       BillingCity = @BillingCity,
		       BillingState = @BillingState,
		       BillingZip = @BillingZip,
		       PurchaseOrder = @PurchaseOrder
		 WHERE ProjectId = @ProjectId
	END
	ELSE
	BEGIN
		INSERT INTO dbo.ProjectBillingInfo
		            (ProjectId, BillingContact, BillingPhone, BillingEmail, BillingType, BillingAddress1, BillingAddress2,
		             BillingCity, BillingState, BillingZip, PurchaseOrder)
		     VALUES (@ProjectId, @BillingContact, @BillingPhone, @BillingEmail, @BillingType, @BillingAddress1, @BillingAddress2,
		             @BillingCity, @BillingState, @BillingZip, @PurchaseOrder)
	END

