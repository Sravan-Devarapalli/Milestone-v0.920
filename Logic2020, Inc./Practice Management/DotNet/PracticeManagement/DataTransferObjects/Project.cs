using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    internal class ProjectEqualityComparer : EqualityComparer<Project>
    {
        #region Overrides of EqualityComparer<Project>

        /// <summary>
        /// When overridden in a derived class, determines whether two objects of type <paramref name="T"/> are equal.
        /// </summary>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        /// <param name="x">The first object to compare.
        ///                 </param><param name="y">The second object to compare.
        ///                 </param>
        public override bool Equals(Project x, Project y)
        {
            return x.Equals(y);
        }

        /// <summary>
        /// When overridden in a derived class, serves as a hash function for the specified object for hashing algorithms and data structures, such as a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the specified object.
        /// </returns>
        /// <param name="obj">The object for which to get a hash code.
        ///                 </param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.
        ///                 </exception>
        public override int GetHashCode(Project obj)
        {
            return obj.Id.Value;
        }

        #endregion
    }

    /// <summary>
	/// Data Transfer Object for a Project entity
	/// </summary>
	[DataContract]
	[Serializable]
	[DebuggerDisplay("DataTransferObjects.Project; Name = {Name}")]
	public class Project : IEquatable<Project>, IIdNameObject
	{
		#region Properties

		/// <summary>
		/// Gets or sets an unique identifier of the project int the system.
		/// </summary>
		[DataMember]
		public int? Id
		{
			get;
			set;
		}

        /// <summary>
        /// Gets or sets an OpportunityId of the related Opportunity.
        /// </summary>
        [DataMember]
        public int? OpportunityId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Director to which this project is associated.
        /// </summary>
        [DataMember]
        public Person Director
        {
            get;
            set;
        }


		/// <summary>
		/// Gets or sets a reference to the client the project intended for.
		/// </summary>
		[DataMember]
		public Client Client
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a discount for the project.
		/// </summary>
		[DataMember]
		public decimal Discount
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets terms for the project.
		/// </summary>
		[DataMember]
		public int Terms
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a name for the project.
		/// </summary>
		[DataMember]
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a project's practice.
		/// </summary>
		[DataMember]
		public Practice Practice
		{
			get;
			set;
		}

        /// <summary>
        /// Gets or sets a Project Number for the <see cref="Project"/>.
        /// </summary>
        [DataMember]
        public string ProjectNumber
        {
            get;
            set;
        }

		/// <summary>
		/// Gets or sets a project's Start Date.
		/// </summary>
		[DataMember]
		public DateTime? StartDate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a project's End Date.
		/// </summary>
		[DataMember]
		public DateTime? EndDate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a list of the <see cref="Milestone"/>s for the <see cref="Project"/>.
		/// </summary>
		[DataMember]
		public List<Milestone> Milestones
		{
			get;
			set;
		}

        /// <summary>
        /// Gets or sets a list of the <see cref="MilestonePerson"/>s for the <see cref="Project"/>.
        /// </summary>
        [DataMember]
        public List<MilestonePerson> ProjectPersons
        {
            get;
            set;
        }

		/// <summary>
		/// Gets or sets a project's sales commission
		/// </summary>
		[DataMember]
		public List<Commission> SalesCommission
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a project's management commission.
		/// </summary>
		[DataMember]
		public Commission ManagementCommission
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a <see cref="Project"/> status.
		/// </summary>
		[DataMember]
		public ProjectStatus Status
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="Project"/>'s computed interest values.
		/// </summary>
		[DataMember]
		public ComputedFinancials ComputedFinancials
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a list of the projected financial indicators for the interest period.
		/// </summary>
		[DataMember]
		public Dictionary<DateTime, ComputedFinancials> ProjectedFinancialsByMonth
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a Practice Manager Commission for the project.
		/// </summary>
		[DataMember]
		[Obsolete]
		public PracticeManagementCurrency PracticeManagerCommission
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets an access level for the project.
		/// </summary>
		[DataMember]
		public Seniority AccessLevel
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a billing info for the project.
		/// </summary>
		[DataMember]
		public BillingInfo BillingInfo
		{
			get;
			set;
		}

        /// <summary>
        /// Gets or sets a project manager for the project.
        /// </summary>
        [DataMember]
        public Person ProjectManager
        {
            get;
            set;
        }

		/// <summary>
		/// Gets or sets a bayer name.
		/// </summary>
		[DataMember]
		public string BuyerName
		{
			get;
			set;
		}

	    /// <summary>
	    /// Gets or sets a project Group.
	    /// </summary>
	    [DataMember]
        public ProjectGroup Group
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a SalesPersonName
        /// </summary>
        [DataMember]
        public String SalesPersonName
        {
            get;
            set;
        }
        /// <summary>
		/// Gets a default start date for a new milestone.
		/// </summary>
		public DateTime MilestoneDefaultStartDate
		{
			get
			{
				return Milestones != null && Milestones.Count > 0 ?
					Milestones.Max(milestone => milestone.ProjectedDeliveryDate).AddDays(1) :
					DateTime.Today;
			}
		}

        /// <summary>
        /// Milestones in this project are chargeable by default.
        /// </summary>
        [DataMember]
        public bool IsChargeable { get; set; }              

		#endregion

        #region Formatting

	    public string DetailedProjectTitle
	    {
	        get
	        {
                return 
                    String.Format(Constants.Formatting.ProjectDetailedNameFormat,
                                 ProjectNumber,
                                 Client.Name,
                                 BuyerName,
                                 Name);
	        }
	    }

        public string ProjectNameNumber
        {
            get
            {
                return
                    String.Format(Constants.Formatting.ProjectNameNumberFormat,
                                    Name, ProjectNumber);
            }
        }

        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region IEquatable<Project> Members

        public bool Equals(Project other)
        {
            return Id.HasValue && other.Id.HasValue && Id == other.Id;
        }

        #endregion
    }
}

