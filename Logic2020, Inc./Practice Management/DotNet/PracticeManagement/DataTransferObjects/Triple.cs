using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    /// <summary>
    /// Provides a basic utility class that is used to store 3 typed objects
    /// </summary>
    [DataContract]
    [Serializable]
    public class Triple<A, B, C>
    {
        #region Properties

	    /// <summary>
        /// Gets or sets the First object.
        /// </summary>
        [DataMember]
        public A First
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Second object.
        /// </summary>
        [DataMember]
        public B Second
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Third object.
        /// </summary>
        [DataMember]
        public C Third
        {
            get;
            set;
        }

	    #endregion    

        /// <summary>
        /// Init constructor of Triple.
        /// </summary>
            public Triple(A first, B second, C third)
        {
            this.First = first;
            this.Second = second;
            this.Third = third;
        }
    }
}

