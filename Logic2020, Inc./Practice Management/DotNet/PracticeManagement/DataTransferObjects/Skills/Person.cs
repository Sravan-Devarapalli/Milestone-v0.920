using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Skills
{
    [DataContract]
    [Serializable]
    public class Person
    {
        /// <summary>
        /// gets or sets person's Id.
        /// </summary>
        [DataMember]
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// gets or sets email of the person.
        /// </summary>
        [DataMember]
        public string Alias
        {
            get;
            set;
        }

        /// <summary>
        /// gets or sets first name of the person.
        /// </summary>
        [DataMember]
        public string FirstName
        {
            get;
            set;
        }

        /// <summary>
        /// gets or sets middle name of the person.
        /// </summary>
        [DataMember]
        public string MiddleName
        {
            get;
            set;
        }

        /// <summary>
        /// gets or sets last name of the person.
        /// </summary>
        [DataMember]
        public string LastName
        {
            get;
            set;
        }

        /// <summary>
        /// gets or sets if the person is Manager.
        /// </summary>
        [DataMember]
        public bool IsManager
        {
            get;
            set;
        }
        /// <summary>
        /// gets or sets Manager of the person.
        /// </summary>
        [DataMember]
        public Person Manager
        {
            get;
            set;
        }
        /// <summary>
        /// gets or sets url of the person Image.
        /// </summary>
        [DataMember]
        public string ImageUrl
        {
            get;
            set;
        }

        /// <summary>
        /// gets or sets date and time when the person's record is modified last time.
        /// </summary>
        [DataMember]
        public DateTime? ModificationDate
        {
            get;
            set;
        }

        /// <summary>
        /// gets or sets tenant Id to wwhich the person belongs to.
        /// </summary>
        [DataMember]
        public int? ClientId
        {
            get;
            set;
        }

        /// <summary>
        /// gets or sets Level of the person in the organization.
        /// </summary>
        [DataMember]
        public Title Title
        {
            get;
            set;
        }

        /// <summary>
        /// gets or sets list of industries in which person has experience.
        /// </summary>
        [DataMember]
        public List<PersonIndustry> Industries
        {
            get;
            set;
        }

        /// <summary>
        /// gets or sets practice of the person.
        /// </summary>
        [DataMember]
        public Practice Practice
        {
            get;
            set;
        }

        /// <summary>
        /// gets or sets list of documents of the person.
        /// </summary>
        [DataMember]
        public List<PersonDocument> Documents
        {
            get;
            set;
        }

        [DataMember]
        public List<PersonContactInfo> ContactInfoList
        {
            get;
            set;
        }

        [DataMember]
        public List<PersonQualification> Qualifications
        {
            get;
            set;
        }

        [DataMember]
        public List<PersonSkill> Skills
        {
            get;
            set;
        }

        [DataMember]
        public List<PersonTraining> Trainings
        {
            get;
            set;
        }

        [DataMember]
        public List<PersonEmployer> Employers
        {
            get;
            set;
        }

    }
}

