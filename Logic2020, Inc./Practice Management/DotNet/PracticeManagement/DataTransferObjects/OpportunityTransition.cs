using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
	/// <summary>
	/// Represents an opportunity transition.
	/// </summary>
	[DataContract]
	[Serializable]
    [KnownType(typeof(Opportunity))]
    [KnownType(typeof(OpportunityTransitionStatus))]
    [KnownType(typeof(Person))]
    public class OpportunityTransition
	{
		/// <summary>
		/// Gets or sets a transition ID.
		/// </summary>
		[DataMember]
		public int? Id
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets an ID of the parent <see cref="Opportunity"/>.
		/// </summary>
		[DataMember]
		public Opportunity Opportunity
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets an opportunity transition status.
		/// </summary>
		[DataMember]
		public OpportunityTransitionStatus OpportunityTransitionStatus
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a date/time when the transition occurs.
		/// </summary>
		[DataMember]
		public DateTime TransitionDate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a <see cref="Person"/> who makes a transition.
		/// </summary>
		[DataMember]
		public Person Person
		{
			get;
			set;
		}

        /// <summary>
        /// Gets or sets a <see cref="Person"/> who is a target of given transition.
        /// </summary>
        [DataMember]
        public Person TargetPerson
        {
            get;
            set;
        }

		/// <summary>
		/// Gets or sets a transition notes.
		/// </summary>
		[DataMember]
		public string NoteText
		{
			get;
			set;
		}

        [IgnoreDataMember]
	    public string TransitionText
	    {
	        get { return TargetPerson == null ? NoteText : TargetPerson.PersonLastFirstName; }
	    }
	}
}

