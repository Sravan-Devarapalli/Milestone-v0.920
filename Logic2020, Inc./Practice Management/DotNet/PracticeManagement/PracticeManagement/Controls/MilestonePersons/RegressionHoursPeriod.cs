using System;
using System.Collections.Generic;
using DataTransferObjects;

namespace PraticeManagement.Controls.MilestonePersons
{
    public class ExpectedHoursPeriod : ActualHoursPeriod
    {
        #region Fields

        private List<DatePoint> _actualPoints;

        #endregion

        #region Properties

        /// <summary>
        /// Return first actual hours record (assuming that there's more than 2 TEs)
        /// </summary>
        protected DateTime FirstActualHoursRecord
        {
            get
            {
                return ActualPoints[0].Date;
            }
        }

        private List<DatePoint> ActualPoints
        {
            get
            {
                if (_actualPoints == null)
                {
                    _actualPoints = new List<DatePoint>(base.Period);       //  Get distinct points
                    _actualPoints.RemoveAll(
                        point => !point.Value.HasValue || point.DayOff);    //  Remove null && day off points
                    _actualPoints.Sort();                                   //  Sort them to get min amd max
                }

                return _actualPoints;
            }
        }

        #endregion

        #region Coef

        private double Cxx
        {
            get
            {
                var cxx = 0.0;

                foreach (DatePoint pt in ActualPoints)
                {
                    int x = DateToCoord(pt.Date);
                    cxx += x * x;
                }

                return cxx;
            }
        }

        private double Cx
        {
            get
            {
                var cx = 0.0;

                foreach (DatePoint pt in ActualPoints)
                {
                    cx += DateToCoord(pt.Date);
                }

                return cx;
            }
        }

        private double Cxy
        {
            get
            {
                double cxy = 0.0;

                foreach (DatePoint pt in ActualPoints)
                {
                    cxy += DateToCoord(pt.Date) * pt.Value.Value;
                }

                return cxy;
            }
        }

        private double Cy
        {
            get
            {
                double cy = 0.0;

                foreach (DatePoint pt in ActualPoints)
                {
                    cy += pt.Value.Value;
                }

                return cy;
            }
        }

        private double Cn
        {
            get
            {
                return ActualPoints.Count;
            }
        }

        #endregion

        #region Solution

        private double Determinant
        {
            get
            {
                return Cxx * Cn - Cx * Cx;
            }
        }

        private double A
        {
            get
            {
                return (Cxy * Cn - Cx * Cy) / Determinant;
            }
        }

        private double B
        {
            get
            {
                return (Cxx * Cy - Cxy * Cx) / Determinant;
            }
        }

        #endregion

        #region Constructor

        public ExpectedHoursPeriod(IActivityPeriod activityPeriod)
            : base(activityPeriod)
        {
        }

        #endregion

        #region ActualHoursPeriod methods

        public override IEnumerable<DatePoint> Period
        {
            get
            {
                if (Cn == 0)
                    return new List<DatePoint>(ActivityPeriod.ProjectedActivity);

                var firstActualHoursRecord = FirstActualHoursRecord;

                if (Cn == 1)
                {
                    var points = new List<DatePoint>(new List<DatePoint>(ActivityPeriod.EmptyPeriodPoints));
                    points.RemoveAll(pt => pt.Date < firstActualHoursRecord);

                    points.ForEach(delegate(DatePoint pt)
                                       {
                                           pt.Value = pt.DayOff ? 0.0 : ActualPoints[0].Value.Value;
                                       });

                    return points;
                }

                IEnumerable<DatePoint> period = 
                    new List<DatePoint>(ActivityPeriod.EmptyPeriodPoints).
                        FindAll(obj => obj.Date >= firstActualHoursRecord && !obj.DayOff);

                foreach (var pt in period)
                {
                    var val = A*DateToCoord(pt.Date) + B;
                    if (val >= 0 && val <= 24)
                    {
                        pt.Value = val;
                    }
                    else
                    {
                        pt.Value = val <= 0 ? 0 : 24.0;
                    }
                }

                return period;
            }
        }

        #endregion
    }
}

