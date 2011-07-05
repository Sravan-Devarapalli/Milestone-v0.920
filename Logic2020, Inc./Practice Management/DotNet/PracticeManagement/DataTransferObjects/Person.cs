using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    /// <summary>
    /// Data Transfer Object for a Person entity
    /// </summary>
    [DataContract]
    [Serializable]
    public class Person : IEquatable<Person>, IIdNameObject, IComparable<Person>
    {

        #region Constants

        public const string RecruitingOverheadName = "Recruiting";
        public const string PersonNameFormat = "{0}, {1}";

        #endregion

        #region Fields

        private int _ptoDays;
        private DateTime? _terminationDate;
        private string _alias;
        private Practice _defaultPractice;
        private List<Commission> _commissionList = new List<Commission>();
        private List<BilledTime> _billedTimeList = new List<BilledTime>();
        private Pay _currentPay;


        #endregion

        #region Properties - data members

        /// <summary>
        /// Identifies the person in the system
        /// </summary>
        /// <remarks>
        /// This value can be null if the person has not been entered in the system.  If
        /// a Person is saved with a <value>null</value> Id the system should assign a
        /// new id and and change the Id value to that new id.
        /// </remarks>
        [DataMember]
        public int? Id
        {
            get;
            set;
        }

        [DataMember]
        public bool IsWelcomeEmailSent
        {
            get;
            set;
        }

        public string Name
        {
            get { return PersonLastFirstName; }
            set { throw new NotImplementedException("Unable to set person name"); }
        }

        /// <summary>
        /// First name of the person, should be able to identify the person in the system along with the LastName
        /// </summary>
        [DataMember]
        public string FirstName
        {
            get;
            set;
        }

        /// <summary>
        /// Last name of the person, should be able to identify the person in the system along with the FirstName
        /// </summary>
        [DataMember]
        public string LastName
        {
            get;
            set;
        }

        /// <summary>
        /// Person status 
        /// </summary>
        [DataMember]
        public PersonStatus Status
        {
            get;
            set;
        }

        /// <summary>
        /// Person Locked-Out value 
        /// </summary>
        [DataMember]
        public bool LockedOut
        {
            get;
            set;
        }

        /// <summary>
        /// Number of PTO days per year this person has
        /// </summary>
        [DataMember]
        public int PtoDays
        {
            get { return _ptoDays; }
            set { _ptoDays = value; }
        }

        /// <summary>
        /// Date of hire
        /// </summary>
        [DataMember]
        public DateTime HireDate
        {
            get;
            set;
        }

        /// <summary>
        /// Employee number
        /// </summary>
        [DataMember]
        public string EmployeeNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Person telephone number.
        /// </summary>
        /// <remarks>
        /// <value>null</value> indicates the person as no phone
        /// </remarks>
        [DataMember]
        public string TelephoneNumber
        {
            get;
            set;
        }


        /// <summary>
        /// Date person no longer works for firm.
        /// </summary>
        /// <remarks>
        /// <value>null</value> indicates the person is still employed
        /// </remarks>
        [DataMember]
        public DateTime? TerminationDate
        {
            get { return _terminationDate; }
            set { _terminationDate = value; }
        }

        /// <summary>
        /// email address of the person, should be able to identify the person
        /// </summary>
        [DataMember]
        public string Alias
        {
            get { return _alias; }
            set { _alias = value; }
        }
        [DataMember]
        public bool IsDefaultManager
        {
            get;
            set;
        }


        [DataMember]
        public Practice DefaultPractice
        {
            get { return _defaultPractice; }
            set { _defaultPractice = value; }
        }

        /// <summary>
        /// Gets or sets a default commission's value if the <see cref="Person"/> receives any.
        /// </summary>
        [DataMember]
        public List<DefaultRecruiterCommission> DefaultPersonRecruiterCommission
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the commissions for the recruiter of the <see cref="Person"/>.
        /// </summary>
        [DataMember]
        public List<RecruiterCommission> RecruiterCommission
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets default <see cref="Person"/>'s commissions if applicable.
        /// </summary>
        [DataMember]
        public List<DefaultCommission> DefaultPersonCommissions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="Person"/>'s commissions.
        /// </summary>
        [DataMember]
        public List<Commission> CommissionList
        {
            get { return _commissionList; }
            set { _commissionList = value; }
        }

        [DataMember]
        public List<BilledTime> BilledTimeList
        {
            get { return _billedTimeList; }
            set { _billedTimeList = value; }
        }

        [DataMember]
        public Pay CurrentPay
        {
            get { return _currentPay; }
            set { _currentPay = value; }
        }

        /// <summary>
        /// Gets or sets a list of the <see cref="Pay"/> objects to determine the payment history for the person.
        /// </summary>
        [DataMember]
        public List<Pay> PaymentHistory
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a list of overheads for the given person.
        /// </summary>
        [DataMember]
        public List<PersonOverhead> OverheadList
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
        /// Gets or sets a list of roles the person is assigned to.
        /// </summary>
        [DataMember]
        public string[] RoleNames
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a person's seniority.
        /// </summary>
        [DataMember]
        public Seniority Seniority
        {
            get;
            set;
        }

        /// <summary>
        /// List of practices owned by this person
        /// </summary>
        [DataMember]
        public List<Practice> PracticesOwned { get; set; }

        /// <summary>
        /// Gets or sets a person's manager (see #1419).
        /// </summary>
        [DataMember]
        public Person Manager { get; set; }

        [DataMember]
        public DateTime? LastLogin { get; set; }

        [DataMember]
        public int OpportunityPersonTypeId
        {
            get;
            set;
        }
        #endregion

        #region Properties - calculated

        /// <summary>
        /// Gets a person's Raw Hourly Rate
        /// </summary>
        public PracticeManagementCurrency RawHourlyRate
        {
            get
            {
                return CurrentPay != null ? CurrentPay.AmountHourly : 0;
            }
        }

        /// <summary>
        /// Gets a person's Total Overhead
        /// </summary>
        /// <remarks>The OverheadList property must be correctly set before calculating.</remarks>
        public PracticeManagementCurrency TotalOverhead
        {
            get
            {
                decimal result = 0;
                if (OverheadList != null)
                {
                    foreach (PersonOverhead overhead in OverheadList.FindAll(OH => !OH.IsMLF))
                    {
                        result += overhead.HourlyValue;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a person's Total Overhead exluding the recruiting commissions.
        /// </summary>
        /// <remarks>The OverheadList property must be correctly set before calculating.</remarks>
        public PracticeManagementCurrency OverheadWithoutRecruiting
        {
            get
            {
                decimal result = 0;
                if (OverheadList != null)
                {
                    foreach (PersonOverhead overhead in OverheadList)
                    {
                        if (string.Compare(overhead.Name, RecruitingOverheadName, true) != 0)
                        {
                            result += overhead.HourlyValue;
                        }
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a person's Loaded hourly rate.
        /// </summary>
        /// <remarks>The OverheadList property must be correctly set before calculating.</remarks>
        public PracticeManagementCurrency LoadedHourlyRate
        {
            get
            {
                return RawHourlyRate + TotalOverhead;
            }
        }

        /// <summary>
        /// Gets a person's Loaded Rate excluding the recruiting commissions.
        /// </summary>
        /// <remarks>The OverheadList property must be correctly set before calculating.</remarks>
        public PracticeManagementCurrency LoadedHourlyRateWithoutRecruiting
        {
            get
            {
                return RawHourlyRate + OverheadWithoutRecruiting;
            }
        }

        /// <summary>
        /// Substitution for ToString
        /// Used where it's not allowed to use methods, but properties
        /// </summary>
        public string PersonLastFirstName
        {
            get
            {
                return this.ToString();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Init constructor of Person.
        /// </summary>
        public Person(int? id)
        {
            this.Id = id;
        }

        /// <summary>
        /// Init constructor of Person.
        /// </summary>
        public Person()
        {
        }

        #endregion

        #region Methods

        public bool Equals(Person other)
        {
            if (other == null)
                return false;

            if (Id.HasValue && other.Id.HasValue)
                return Id.Value == other.Id.Value;

            return false;
        }

        public int CompareTo(Person other)
        {
            return other == null ? 1 : PersonLastFirstName.CompareTo(other.PersonLastFirstName);
        }

        public override string ToString()
        {
            return string.Format(
                PersonNameFormat,
                LastName, FirstName);
        }

        #endregion

    }
}

