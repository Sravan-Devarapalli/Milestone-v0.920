using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    [Serializable]
    [DataContract]
    public class Seniority
    {
        #region Constants

        private static readonly int Separation = Settings.SenioritySeparationRange;

        #endregion

        /// <summary>
        /// Gets or sets an ID of the seniority
        /// </summary>
        [DataMember]
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a Name of the seniority
        /// </summary>
        [DataMember]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// It's the value used to compare and decide permissions
        /// </summary>
        [DataMember]
        public int SeniorityValue
        {
            get;
            set;
        }

        public bool OtherHasGreaterOrEqualSeniority(Seniority other)
        {
            //  Currently in our database 
            //  GREATER number means LOWER seniority
            //
            //      Settings.SenioritySeparationRange added according to #1457
            var greaterOrEqualSeniority = GetSeniorityValueById(other.Id) <= ValueWithSeparation;

            return greaterOrEqualSeniority;
        }

        public int ValueWithSeparation
        {
            get { return GetSeniorityValueById(Id) + Separation; }
        }

        public static int GetSeniorityValueById(int id)
        {
            int seniorityValue = id;
            switch (id)
            {
                case 1:
                case 15:
                case 25:
                case 35:
                case 45:
                case 55:
                case 65:
                case 75:
                case 85:
                case 95:
                case 105: seniorityValue = id; break;
                case 106: seniorityValue = 55; break; //Architect = Senior Manager
                case 107: seniorityValue = 65; break; //Lead Developer = Manager
                case 108: seniorityValue = 75; break;//Senior Developer = Senior Consultant
                case 109: seniorityValue = 85; break;//Developer = Consultant
                default: break;
            }

            return seniorityValue;
        }
    }
}

