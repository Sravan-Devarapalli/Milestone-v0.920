
CREATE PROCEDURE [dbo].[EmailNotificationsListExpiredPersons]
	@EmailTemplateId int		
AS 
    BEGIN
        SET NOCOUNT ON ;
	
	SELECT  distinct
		pers.PersonId as 'ProjectId',
		pers.LastName + pers.FirstName as 'PersonName',
		isnull(recr.Alias, 'seanc@logic2020.com') as PersonRecruter, 
		isnull(mngr.Alias, 'seanc@logic2020.com') as LineManager, 
		isnull(pown.Alias, 'seanc@logic2020.com') as PracticeOwner, 
		pers.HireDate
	FROM
		Person as pers
		inner join Person as mngr on pers.ManagerId = mngr.PersonId
		left join Practice as pr on pers.DefaultPractice = pr.PracticeId
		left join Person as pown on pr.PracticeManagerId = pown.PersonId
		left join Person as recr on pers.RecruiterId = recr.PersonId
		where pers.PersonStatusId = 3		
    
		SELECT EmailTemplateSubject, EmailTemplateBody FROM EmailTemplate  et WHERE et.EmailTemplateId = @EmailTemplateId  
    END



