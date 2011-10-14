CREATE PROCEDURE [dbo].[OpportunitySearchText]
(
	@Looked              NVARCHAR(255),
	@PersonId			 INT
)
AS
BEGIN
	SET NOCOUNT ON;

	IF @Looked IS NULL
	BEGIN
		SET @Looked = '%'
	END
	ELSE
	BEGIN
		SET @Looked = '%' + RTRIM(@Looked) + '%'
	END		



	SELECT DISTINCT  o.OpportunityId,
			o.Name,		   
			o.ClientId,
			o.SalespersonId,
			o.OpportunityStatusId,
			o.[Priority],
			o.PriorityId,
			op.sortOrder PrioritySortOrder,
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
			proj.ProjectNumber,
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
			o.OutSideResources
		FROM v_Opportunity O
		LEFT JOIN dbo.Person p ON o.OwnerId = p.PersonId
		LEFT JOIN dbo.PersonStatus ps ON ps.PersonStatusId = o.SalespersonStatusId  
		LEFT JOIN dbo.PersonStatus os ON os.PersonStatusId = o.OwnerStatusId 
		LEFT JOIN dbo.Project proj ON proj.ProjectId = o.ProjectId
		INNER JOIN dbo.OpportunityPriorities AS op ON op.Id = o.PriorityId 
		WHERE (o.Name LIKE @Looked OR o.Description LIKE @Looked OR o.ClientName LIKE @Looked OR o.OpportunityNumber LIKE @Looked OR o.BuyerName LIKE @Looked)	
			
END
	
