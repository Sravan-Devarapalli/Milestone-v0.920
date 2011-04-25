CREATE VIEW dbo.v_OpportunityHistory
AS 
SELECT opt.OpportunityId, opt.OpportunityNumber, stat.[Name] AS 'Status', trans.TransitionDate, 
		pers.FirstName + ' ' + pers.LastName AS 'Responsible', pers.PersonId, trans.NoteText
FROM dbo.Opportunity AS opt
LEFT OUTER JOIN dbo.OpportunityTransition AS trans ON trans.OpportunityId = opt.OpportunityId
INNER JOIN dbo.OpportunityTransitionStatus AS stat ON stat.OpportunityTransitionStatusId = trans.OpportunityTransitionStatusId
INNER JOIN dbo.Person AS pers ON pers.PersonId = trans.PersonId
