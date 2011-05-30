using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class OverHeadHistory
    {
        #region Properties

      
        [DataMember]
        public Decimal W2Salary_Rate
        {
            get;
            set;
        }


        [DataMember]
        public Decimal W2Hourly_Rate
        {
            get;
            set;
        }

        [DataMember]
        public Decimal _1099_Hourly_Rate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a start date when the overhead acts.
        /// </summary>
        [DataMember]
        public DateTime StartDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an end date when the overhaed acts.
        /// </summary>
        [DataMember]
        public DateTime? EndDate
        {
            get;
            set;
        }

        #endregion
    }
}

