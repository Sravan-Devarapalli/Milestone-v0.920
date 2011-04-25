using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

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

        #endregion

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

        #endregion

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
        /// Is the practice in active
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; }

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

        #endregion

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

        #endregion

        public bool Equals(Practice other)
        {
            if (other == null)
                return false;

            return Id == other.Id;
        }
    }
}
