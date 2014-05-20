using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTransferObjects.Financials
{
    public class QuarterRange
    {
        public RangeType QuarterRangeType { get; set; }

        public int ProjectId { get; set; }

        public List<ComputedFinancials> FinancialsList { get; set; }

        public ComputedFinancials getSummedComputedFinancials()
        {
            var finalComputedFinancials = new ComputedFinancials();
            finalComputedFinancials.FinancialDate = QuarterRangeType.StartDate;
            finalComputedFinancials.FinancialRange = QuarterRangeType;
            finalComputedFinancials.Revenue = FinancialsList.Sum(p=>p.Revenue);
            finalComputedFinancials.GrossMargin = FinancialsList.Sum(p=>p.GrossMargin);
            finalComputedFinancials.ActualRevenue = FinancialsList.Sum(p=>p.ActualRevenue);
            finalComputedFinancials.ActualGrossMargin = FinancialsList.Sum(p => p.ActualGrossMargin);
            finalComputedFinancials.FinancialRange = QuarterRangeType;
            return finalComputedFinancials;
        }

    }
}

