CREATE TRIGGER CreateDefaultGroupForClient 
   ON  dbo.Client 
   AFTER INSERT
AS 
BEGIN
	SET NOCOUNT ON;
	INSERT INTO dbo.BusinessGroup (
		ClientId,
		[Name],
		Code
	)  ( 
		SELECT i.ClientId, 'Default' AS [Name], 'BG0001' 
		FROM inserted AS i
	) 

	INSERT INTO dbo.ProjectGroup (
		ClientId,
		[Name],
		Code,
		BusinessGroupId
	)  ( 
		SELECT i.ClientId, 'Default Group' AS [Name], 'B0001',(SELECT [BusinessGroupId] FROM BusinessGroup BG WHERE Code='BG0001' AND BG.ClientId=i.ClientId)
		FROM inserted AS i
	) 
	INSERT INTO dbo.PricingList
	(
		ClientId,
		[Name],
		IsDefault
	)  ( 
		SELECT i.ClientId, 'Default' AS [Name],1
		FROM inserted AS i
	) 

	
END

