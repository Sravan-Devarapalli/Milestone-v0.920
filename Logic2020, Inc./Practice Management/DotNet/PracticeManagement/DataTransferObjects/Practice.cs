using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web;

namespace DataTransferObjects
{
    /// <summary>
    /// Practices categorize projects
    /// </summary>
    [DataContract]
    [Serializable]
    public class Practice : IEquatable<Practice>
    {
        #region Constants

        /// <summary>
        /// An ID of the Offshore practice.
        /// </summary>
        public const int OffshorePractice = 1;

        /// <summary>
        /// An ID of the Admin practice.
        /// </summary>
        public const int AdminPractice = 4;

        #endregion Constants

        #region Constructors

        /// <summary>
        /// Init constructor of Practice.
        /// </summary>
        public Practice()
        {
        }

        /// <summary>
        /// Init constructor of Practice.
        /// </summary>
        public Practice(int id)
        {
            Id = id;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Identifies the practice
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Disaplay name of the practice
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Abbreviation for the practice.
        /// </summary>
        [DataMember]
        public string Abbreviation { get; set; }

        /// <summary>
        /// Display Encoded name of the practice
        /// </summary>
        public string HtmlEncodedName
        {
            get
            {
                return HttpUtility.HtmlEncode(Name);
            }
        }

        /// <summary>
        /// Practice manager
        /// </summary>
        [DataMember]
        public Person PracticeOwner { get; set; }

        /// <summary>
        /// Is the practice in Use
        /// </summary>
        [DataMember]
        public bool InUse { get; set; }

        /// <summary>
        /// Is Active Capabilities Exists for the given practice.
        /// </summary>
        [DataMember]
        public bool IsActiveCapabilitiesExists { get; set; }

        /// <summary>
        /// Is the practice in active
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public bool IsNotesRequiredForTimeEnry { get; set; }

        /// <summary>
        /// Is the practice Company Internal
        /// </summary>
        [DataMember]
        public bool IsCompanyInternal { get; set; }

        /// <summary>
        /// This Property is solely used for Group by Practice Manager Report.
        /// </summary>
        [DataMember]
        public Dictionary<DateTime, ComputedFinancials> ProjectedFinancialsByMonth
        {
            get;
            set;
        }

        /// <summary>
        /// This Property is solely used for Group by Practice Manager Report.
        /// </summary>
        [DataMember]
        public ComputedFinancials ComputedFinancials
        {
            get;
            set;
        }

        [DataMember]
        public List<PracticeManagerHistory> PracticeManagers
        {
            get;
            set;
        }

        [DataMember]
        public string PracticeOwnerName { get; set; }

        [DataMember]
        public List<PracticeCapability> PracticeCapabilities
        {
            get;
            set;
        }

        public int PracticeManagerId
        {
            get { return PracticeOwner != null && PracticeOwner.Id.HasValue ? PracticeOwner.Id.Value : -1; }
        }

        [DataMember]
        public string DivisionIds { get; set; }

        [DataMember]
        public string ProjectDivisionIds { get; set; }

        #endregion Properties

        #region Formatting

        public string PracticeWithOwner
        {
            get
            {
                return string.Format(
                    "{0} ({1})",
                    Name,
                    PracticeOwner.PersonLastFirstName);
            }
        }

        #endregion Formatting

        public bool Equals(Practice other)
        {
            if (other == null)
                return false;

            return Id == other.Id;
        }

        public static int Compare(Practice obj1, Practice obj2)
        {
            if (obj1 == null && obj2 != null)
            {
                return -1;
            }
            else if (obj1 != null && obj2 == null)
            {
                return 1;
            }
            else if (obj1 == null && obj2 == null)
            {
                return 0;
            }
            else
            {
                return string.Compare(obj1.Name, obj2.Name);
            }
        }
    }
}

