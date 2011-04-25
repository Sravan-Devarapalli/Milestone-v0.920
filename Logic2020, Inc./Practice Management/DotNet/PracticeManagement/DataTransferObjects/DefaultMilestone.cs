using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [DataContract]
    [Serializable]
    public class DefaultMilestone
    {

        /// <summary>
        /// Id of the Default Milestone
        /// </summary>
        [DataMember]
		public int MilestoneId
		{
			get;
			set;
		}

        /// <summary>
        /// Project Id of the Milstone.
        /// </summary>
        [DataMember]
        public int ProjectId
        {
            get;
            set;
        }

        /// <summary>
        /// Client Id of the Default Milestone's Project.
        /// </summary>
        [DataMember]
        public int ClientId
        {
            get;
            set;
        }
        
        /// <summary>
        /// Date on which milestone is set as Default milestone.
        /// </summary>
        [DataMember]
        public DateTime ModifiedDate
        {
            get;
            set;
        }

        /// <summary>
        /// No. of days to be used as lower bound to select a milestone.
        /// </summary>
        [DataMember]
        public int LowerBound
        {
            get;
            set;
        }

        /// <summary>
        ///  No. of days to be used as upper bound to select a milestone.
        /// </summary>
        [DataMember]
        public int UpperBound
        {
            get;
            set;
        }
    }
}

