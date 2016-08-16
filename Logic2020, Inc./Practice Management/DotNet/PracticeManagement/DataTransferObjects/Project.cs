using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using DataTransferObjects.Financials;
using DataTransferObjects.TimeEntry;

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
            if (obj != null && obj.Id != null) return obj.Id.Value;
            return base.GetHashCode();
        }

        #endregion Overrides of EqualityComparer<Project>
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

        [DataMember]
        public bool IsNoteRequired { get; set; }

        [DataMember]
        public bool IsClientTimeEntryRequired { get; set; }

        /// <summary>
        /// Gets or sets an OpportunityId of the related Opportunity.
        /// </summary>
        [DataMember]
        public int? OpportunityId
        {
            get;
            set;
        }

        [DataMember]
        public String OpportunityNumber
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

        [DataMember]
        public string DirectorName
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

        public string HtmlEncodedName
        {
            get
            {
                return HttpUtility.HtmlEncode(Name);
            }
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

        [DataMember]
        public DateTime? RecentCompletedStatusDate
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
        /// Gets or sets a <see cref="Project"/> status.
        /// </summary>
        [DataMember]
        public ProjectStatus Status
        {
            get;
            set;
        }

        [DataMember]
        public string ToAddressList
        {
            get;
            set;
        }

        [DataMember]
        public string OwnerAlias
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

        [DataMember]
        public Dictionary<RangeType, ComputedFinancials> ProjectedFinancialsByRange
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

        [DataMember]
        public string ProjectManagerIdsList
        {
            get;
            set;
        }

        [DataMember]
        public string PONumber
        {
            get;
            set;
        }

        [DataMember]
        public List<Person> ProjectManagers
        {
            get;
            set;
        }

        [DataMember]
        public String ProjectManagerNames
        {
            get;
            set;
        }

        [DataMember]
        public string ProjectCapabilityIds
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

        [DataMember]
        public int SalesPersonId
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

        [DataMember]
        public bool IsCSATEligible { get; set; }

        [DataMember]
        public bool HasAttachments { get; set; }

        [DataMember]
        public List<ProjectAttachment> Attachments { get; set; }

        /// <summary>
        /// Gets or sets a description for the Project.
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }

        [DataMember]
        public bool HasMultipleCSATs { get; set; }

        [DataMember]
        public bool CanCreateCustomWorkTypes { get; set; }

        [DataMember]
        public bool IsInternal { get; set; }

        public List<TimeTypeRecord> ProjectWorkTypes { get; set; }

        [DataMember]
        public string ProjectWorkTypesList { get; set; }

        [DataMember]
        public bool HasTimeEntries { get; set; }

        [DataMember]
        public bool IsPTOProject { get; set; }

        [DataMember]
        public bool IsSickLeaveProject { get; set; }

        [DataMember]
        public bool IsHolidayProject { get; set; }

        [DataMember]
        public bool IsAssignedProject { get; set; }

        [DataMember]
        public bool IsORTProject { get; set; }

        [DataMember]
        public bool IsUnpaidProject { get; set; }

        [DataMember]
        public int TimeTypeId { get; set; }

        [DataMember]
        public int TimeEntrySectionId { get; set; }

        [DataMember]
        public string BillableType { get; set; }

        [DataMember]
        public Person ProjectOwner { get; set; }

        [DataMember]
        public decimal? SowBudget { get; set; }

        [DataMember]
        public decimal? POAmount { get; set; }

        [DataMember]
        public PricingList PricingList { get; set; }

        [DataMember]
        public BusinessType BusinessType { get; set; }

        [DataMember]
        public List<ProjectCSAT> CSATList { get; set; }

        [DataMember]
        public List<Attribution> AttributionList { get; set; }

        [DataMember]
        public int CSATOwnerId { get; set; }

        [DataMember]
        public string CSATOwnerName { get; set; }

        [DataMember]
        public int SeniorManagerId { get; set; }

        [DataMember]
        public string SeniorManagerName { get; set; }

        [DataMember]
        public BusinessGroup BusinessGroup { get; set; }

        [DataMember]
        public bool IsSeniorManagerUnassigned { get; set; }

        [DataMember]
        public string MailBody { get; set; }

        [DataMember]
        public bool IsBusinessDevelopment { get; set; }

        [DataMember]
        public string Capabilities { get; set; }

        [DataMember]
        public string ProjectAccessesEmpNumbers { get; set; }

        [DataMember]
        public string EngagementManagerUserID { get; set; }

        [DataMember]
        public string ProjectManagerUserId { get; set; }

        [DataMember]
        public string ExecutiveInChargeUserId { get; set; }

        [DataMember]
        public int EngagementManagerID { get; set; }

        [DataMember]
        public int ProjectManagerId { get; set; }

        [DataMember]
        public int ExecutiveInChargeId { get; set; }

        [DataMember]
        public string ExecutiveInChargeName { get; set; }

        [DataMember]
        public ProjectDivision Division { get; set; }

        [DataMember]
        public Channel Channel { get; set; }

        [DataMember]
        public string SubChannel { get; set; }

        [DataMember]
        public Revenue RevenueType { get; set; }

        [DataMember]
        public Offering Offering { get; set; }


        [DataMember]
        public Project PreviousProject
        {
            get;
            set;
        }

        [DataMember]
        public int OutsourceId
        {
            get;
            set;
        }

        public bool HasMilestones
        {
            get
            {
                return Milestones != null && Milestones.Any();
            }
        }

        #endregion Properties

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

        public string ProjectNumberName
        {
            get
            {
                return
                    String.Format(Constants.Formatting.ProjectNameNumberFormat,
                                    ProjectNumber, Name);
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public string ProjectRange
        {
            get
            {
                if (StartDate.HasValue && EndDate.HasValue)
                {
                    return StartDate.Value.ToString(Constants.Formatting.EntryDateFormat) + " - " + EndDate.Value.ToString(Constants.Formatting.EntryDateFormat);
                }
                return string.Empty;
            }
        }

        #endregion Formatting

        #region IEquatable<Project> Members

        public bool Equals(Project other)
        {
            return Id.HasValue && other.Id.HasValue && Id == other.Id;
        }

        #endregion IEquatable<Project> Members
    }
}

