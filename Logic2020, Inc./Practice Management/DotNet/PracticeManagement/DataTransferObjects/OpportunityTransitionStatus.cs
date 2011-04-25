using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Represents an Opportunity Transition Status.
	/// </summary>
	[DataContract]
	[Serializable]
	public class OpportunityTransitionStatus
	{
		/// <summary>
		/// Gets or sets an ID of the OpportunityTransitionStatus.
		/// </summary>
		[DataMember]
		public int Id
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a name of the OpportunityTransitionStatus.
		/// </summary>
		[DataMember]
		public string Name
		{
			get;
			set;
		}

	    public static OpportunityTransitionStatus FromType(OpportunityTransitionStatusType transitionStatusType)
	    {
	        return new OpportunityTransitionStatus
	                   {
	                       Id = (int) transitionStatusType
	                   };
	    }

	    public OpportunityTransitionStatusType StatusType
	    {
	        get { return (OpportunityTransitionStatusType) Id; }
	    }
	}
}

