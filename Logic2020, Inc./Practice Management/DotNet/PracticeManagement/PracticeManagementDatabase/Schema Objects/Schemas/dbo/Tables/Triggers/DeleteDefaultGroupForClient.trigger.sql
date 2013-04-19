CREATE TRIGGER DeleteDefaultGroupForClient 
   ON  dbo.Client 
   AFTER DELETE
AS 
BEGIN
	SET NOCOUNT ON;

	DELETE FROM dbo.ProjectGroup 
	WHERE ClientId IN ( SELECT d.ClientId FROM deleted AS d)

	DELETE FROM dbo.PricingList 
	WHERE ClientId IN ( SELECT d.ClientId FROM deleted AS d)

	DELETE FROM dbo.BusinessGroup 
	WHERE ClientId IN ( SELECT d.ClientId FROM deleted AS d)
END

