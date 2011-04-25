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

        public bool OtherHasGreaterOrEqualSeniority(Seniority other)
        {
            //  Currently in our database 
            //  GREATER number means LOWER seniority
            //
            //      Settings.SenioritySeparationRange added according to #1457
            var greaterOrEqualSeniority = other.Id <= ValueWithSeparation;        

            return greaterOrEqualSeniority;
        }

	    public int ValueWithSeparation
	    {
	        get { return Id + Separation; }
	    }
	}
}

