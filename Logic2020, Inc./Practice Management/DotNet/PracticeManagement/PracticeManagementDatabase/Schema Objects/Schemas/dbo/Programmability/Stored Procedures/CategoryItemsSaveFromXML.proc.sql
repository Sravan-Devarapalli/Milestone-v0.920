
CREATE PROCEDURE [dbo].[CategoryItemsSaveFromXML]
(
	@CategoryItemsXML XML,
	@Year			  INT
)
AS
BEGIN

	/*
	<Root>
		<Category Id="1">
			<Item Id="1">
				<Month Id="1">
					<Amount>100</Amount>
				</Month>
				<Month Id="2">
					<Amount>200</Amount>
				</Month>
					.
					.
			</Item>
				.
				.
		</Category>
			.
			.
	<Root>
	*/
	DECLARE @CategoryItemsXMLLocal XML
	SELECT @CategoryItemsXMLLocal = @CategoryItemsXML-- CONVERT(XML,@CategoryItemsXML)
	DECLARE  @TempCategoryItemBudget TABLE
	(
		[ItemId]	INT,
        [CategoryTypeId] INT,
        [MonthStartDate] DATETIME,
        [Amount] DECIMAL(18,2),
        IsNewEntry	BIT
	)
	INSERT INTO @TempCategoryItemBudget
	SELECT Temp.ItemId,
		   Temp.CategoryTypeId,
		   Temp.MonthStartDate,
		   Temp.Amount,
		   CASE WHEN CIB.ItemId IS NULL THEN 1 ELSE 0 END IsNewEntry
	FROM 
	(SELECT 
		T.C.value('..[1]/@Id','INT') ItemId,
		T.C.value('..[1]/..[1]/@Id','INT') CategoryTypeId,
		dbo.MakeDate(@Year, T.C.value('@Id','INT'),1) [MonthStartDate],
		T.C.value('Amount[1]','DECIMAL(18,2)') Amount
	FROM @CategoryItemsXMLLocal.nodes('/Root/Category/Item/Month') T(C)) Temp
	LEFT JOIN [dbo].[CategoryItemBudget] CIB
	ON Temp.CategoryTypeId = CIB.CategoryTypeId  AND Temp.ItemId = CIB.ItemId
		AND Temp.MonthStartDate = CIB.MonthStartDate
	
	INSERT INTO [dbo].[CategoryItemBudget]
		 (  [ItemId]
           ,[CategoryTypeId]
           ,[MonthStartDate]
           ,[Amount])
    SELECT 
			[ItemId]
           ,[CategoryTypeId]
           ,[MonthStartDate]
           ,[Amount]
	FROM @TempCategoryItemBudget
	WHERE IsNewEntry = 1 AND Amount<> 0
	
	UPDATE CIB
	SET CIB.Amount = Temp.Amount
	FROM  [dbo].[CategoryItemBudget] CIB
	JOIN @TempCategoryItemBudget Temp
	ON Temp.CategoryTypeId = CIB.CategoryTypeId  AND Temp.ItemId = CIB.ItemId
		AND Temp.MonthStartDate = CIB.MonthStartDate AND Temp.IsNewEntry = 0
END
