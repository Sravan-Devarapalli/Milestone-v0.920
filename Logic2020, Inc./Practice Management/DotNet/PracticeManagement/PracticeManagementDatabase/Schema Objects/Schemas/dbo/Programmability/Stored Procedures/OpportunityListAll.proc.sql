CREATE PROCEDURE [dbo].[OpportunityListAll]
(
	@ActiveOnly      BIT,
	@Looked		     NVARCHAR(50),
	@ClientId        INT,
	@SalespersonId   INT,
	@TargetPersonId	 INT = NULL, -- Used to get all opportunities for a person
	@CurrentId		 INT = NULL,  -- Used to extract prev and next opportunities,
	@IsDiscussionReview2	BIT=0
)
AS
BEGIN
	SET NOCOUNT ON

	IF @Looked IS NULL
	BEGIN
		SET @Looked = '%'
	END
	ELSE
	BEGIN
		SET @Looked = '%' + RTRIM(@Looked) + '%'
	END		

	/*
		Go to issue #2432 for more details on default opportunities order
	*/
	;WITH CTE
	AS
	(
	SELECT ROW_NUMBER() OVER(PARTITION BY O.ClientName + isnull(O.BuyerName, '') ORDER BY CASE OP.sortOrder WHEN 0 THEN 1000 ELSE OP.sortOrder END, O.SalespersonLastName) RowNumber,
			o.OpportunityId,
			o.Name,		   
			o.ClientId,
			o.SalespersonId,
			o.OpportunityStatusId,
			o.Priority,
			o.ProjectedStartDate,
			o.ProjectedEndDate,
			o.OpportunityNumber,
			o.Description,
			o.PracticeId,
			o.BuyerName,
			o.CreateDate,
			o.Pipeline,
			o.Proposed,
			o.SendOut,
			o.ClientName,
			o.SalespersonFirstName,
			o.SalespersonLastName,
			ps.Name AS SalespersonStatus,
			o.OpportunityStatusName,
			o.PracticeName,
			o.ProjectId,
			o.OpportunityIndex,
			o.RevenueType,
			o.OwnerId,
			o.LastUpdate,
			o.GroupId,
			o.GroupName,
			o.PracticeManagerId,
			p.LastName as 'OwnerLastName',
			p.FirstName as 'OwnerFirstName',
			os.Name as 'OwnerStatus',
			o.EstimatedRevenue,
			o.OutSideResources,
			OP.sortOrder
		FROM v_Opportunity O
		LEFT JOIN dbo.Person p ON o.OwnerId = p.PersonId
		LEFT JOIN dbo.PersonStatus ps ON ps.PersonStatusId = o.SalespersonStatusId  
		LEFT JOIN dbo.PersonStatus os ON os.PersonStatusId = o.OwnerStatusId 
		INNER JOIN dbo.OpportunityPriorities AS op ON op.Id = o.PriorityId 
	WHERE ((@IsDiscussionReview2 = 1 AND o.OpportunityStatusId = 1)
			OR @IsDiscussionReview2 = 0 AND  (o.OpportunityStatusId = 1 OR @ActiveOnly = 0)
			)
		AND (o.ClientId = @ClientId OR @ClientId IS NULL)
		AND (o.SalespersonId = @SalespersonId OR @SalespersonId IS NULL)
		AND (o.Name LIKE @Looked OR o.Description LIKE @Looked OR o.ClientName LIKE @Looked OR o.OpportunityNumber LIKE @Looked OR o.BuyerName LIKE @Looked)	
		AND (o.OpportunityId in (select ot.OpportunityId from OpportunityTransition ot where ot.TargetPersonId = @TargetPersonId) OR @TargetPersonId IS NULL)
		)
		
		SELECT 
			B.*
		FROM CTE A
		JOIN CTE B
		ON (A.ClientName =B.ClientName AND isnull(A.BuyerName, '')  = isnull(B.BuyerName, '') 
			AND A.RowNumber=1 AND A.sortOrder!=0 AND B.SortOrder != 0 ) OR (A.OpportunityId = B.OpportunityId AND A.sortOrder=0)
		ORDER BY A.sortOrder,A.SalespersonLastName,B.ClientName,isnull(B.BuyerName, ''),B.sortOrder,B.SalespersonLastName
		
END

