-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 10-10-2008
-- Updated by:	
-- Update date:	
-- Description:	Retrives the billing info for the specified project.
-- =============================================
CREATE PROCEDURE [dbo].[ProjectBillingInfoGetById]
(
	@ProjectId   INT
)
AS
	SET NOCOUNT ON

	SELECT bi.ProjectId,
	       bi.BillingContact,
	       bi.BillingPhone,
	       bi.BillingEmail,
	       bi.BillingType,
	       bi.BillingAddress1,
	       bi.BillingAddress2,
	       bi.BillingCity,
	       bi.BillingState,
	       bi.BillingZip,
	       bi.PurchaseOrder
	  FROM dbo.ProjectBillingInfo AS bi
	 WHERE bi.ProjectId = @ProjectId

