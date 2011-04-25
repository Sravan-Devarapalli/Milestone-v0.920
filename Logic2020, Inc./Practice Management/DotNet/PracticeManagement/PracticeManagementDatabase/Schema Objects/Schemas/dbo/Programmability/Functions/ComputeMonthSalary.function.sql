-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 7-28-2008
-- Updated by:  
-- Update date: 
-- Description:	Computes an actual salary for the month
-- =============================================
CREATE FUNCTION [dbo].[ComputeMonthSalary]
(
	@YearAmount        DECIMAL,
	@HireDate          DATETIME,
	@TerminationDate   DATETIME,
	@Date              DATETIME
)
RETURNS DECIMAL
AS
BEGIN
	DECLARE @MonthAmount DECIMAL
	DECLARE @Year INT
	DECLARE @Month INT
	DECLARE @DaysInMonth INT
	DECLARE @ActualDays INT

	SET @MonthAmount = @YearAmount / 12
	SET @Year = YEAR(@Date)
	SET @Month = MONTH(@Date)
	SET @DaysInMonth = dbo.GetDaysInMonth(@Date)
	SET @ActualDays = @DaysInMonth
	
	IF @Month = MONTH(@HireDate) AND @Year = YEAR(@HireDate)
	BEGIN
		SET @ActualDays = @ActualDays - DAY(@HireDate) + 1
	END

	IF @Month = MONTH(@TerminationDate) AND @Year = YEAR(@TerminationDate)
	BEGIN
		SET @ActualDays = @ActualDays - (@DaysInMonth - DAY(@TerminationDate))
	END

	RETURN @MonthAmount * @ActualDays / @DaysInMonth
END

